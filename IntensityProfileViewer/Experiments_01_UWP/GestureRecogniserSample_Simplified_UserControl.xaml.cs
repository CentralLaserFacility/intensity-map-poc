using Windows.Foundation ;
using Windows.UI.Xaml ;
using Windows.UI.Xaml.Controls ;
using Windows.UI.Xaml.Input ;
using Windows.UI.Xaml.Media ;
using Windows.UI.Input ;

namespace Experiments_01_UWP
{

  public sealed partial class GestureRecogniserSample_Simplified_UserControl : UserControl
  {

    private ManipulationInputProcessor m_manipulationInputProcessor ;

    public GestureRecogniserSample_Simplified_UserControl ( )
    {
      this.InitializeComponent() ;

      m_manipulationInputProcessor = new ManipulationInputProcessor(
        target         : manipulateMe, 
        referenceFrame : mainCanvas
      ) ;
    }

    void resetButton_Pressed ( object sender, RoutedEventArgs e )
    {
      m_manipulationInputProcessor.Reset() ;
    }

    // Our ManipulationInputProcessor listens for events on the 'target' element
    // process them, and update the target's position, size, and rotation.

    class ManipulationInputProcessor
    {

      // Create a GestureRecognizer which will
      // process the manipulations done on the rectangle

      private GestureRecognizer m_gestureRecognizer = new GestureRecognizer() ;

      private UIElement m_targetElement ;

      private UIElement m_referenceElement ;

      private TransformGroup m_cumulativeTransform ;

      private MatrixTransform m_previousTransform ;

      private CompositeTransform m_deltaTransform ;

      public ManipulationInputProcessor (
        UIElement target, 
        UIElement referenceFrame
      ) {
        m_targetElement    = target ;
        m_referenceElement = referenceFrame ;

        // Initialize the transforms that will be used to manipulate the shape

        InitializeTransforms() ;

        // GestureSettings dictate what manipulation events
        // the Gesture Recognizer will listen to

        m_gestureRecognizer.GestureSettings = (
          GestureSettings.ManipulationTranslateX
        | GestureSettings.ManipulationTranslateY 
        | GestureSettings.ManipulationRotate 
        | GestureSettings.ManipulationScale 
        | GestureSettings.Drag
        // | GestureSettings.ManipulationTranslateInertia 
        // | GestureSettings.ManipulationRotateInertia 
        // | GestureSettings.ManipulationScaleInertia 
        ) ;

        // These event handlers receive input events that are used by the gesture recognizer.

        m_targetElement.PointerPressed      += OnPointerPressed ;
        m_targetElement.PointerMoved        += OnPointerMoved ;
        m_targetElement.PointerReleased     += OnPointerReleased ;
        m_targetElement.PointerCanceled     += OnPointerCanceled ;
        m_targetElement.PointerWheelChanged += OnPointerWheelChanged ;

        // These event handlers to respond to the gesture recognizer's output

        m_gestureRecognizer.ManipulationStarted         += OnManipulationStarted ;
        m_gestureRecognizer.ManipulationUpdated         += OnManipulationUpdated ;
        m_gestureRecognizer.ManipulationCompleted       += OnManipulationCompleted ;
        m_gestureRecognizer.ManipulationInertiaStarting += OnManipulationInertiaStarting ;
        m_gestureRecognizer.Dragging                    += Dragging ;
      }

      public void InitializeTransforms ( )
      {

        //
        // Our manipulations are going to affect the RenderTransform
        // that is being applied to the target element.
        //
        // We treat this as a group of two transforms that are applied in sequence,
        // first the 'cumulative transform so far', and secondly the 'delta' transform
        // associated with the most recent manipulation.
        //
        // Since we need to access these two transforms individually, it's convenient
        // to maintain reference variables that refer to the two 'Child' elements
        // of the render transform.
        //

        m_cumulativeTransform = new TransformGroup() ;
        m_previousTransform   = new MatrixTransform() { 
          Matrix = Matrix.Identity 
        } ;
        m_deltaTransform = new CompositeTransform() ;
        m_cumulativeTransform.Children.Add(m_previousTransform) ;
        m_cumulativeTransform.Children.Add(m_deltaTransform) ;
        
        m_targetElement.RenderTransform = m_cumulativeTransform ;

        // m_targetElement.RenderTransform = m_cumulativeTransform = new TransformGroup(){
        //   Children = {
        //     (
        //       m_previousTransform = new MatrixTransform() { 
        //         Matrix = Matrix.Identity 
        //       }
        //     ),
        //     (
        //       m_deltaTransform = new CompositeTransform()
        //     )
        //   }
        // } ;

      }

      // Process the change resulting from a manipulation

      void OnManipulationUpdated ( object sender, ManipulationUpdatedEventArgs updateArgs )
      {
        m_previousTransform.Matrix = (
          // (m_targetElement.RenderTransform as TransformGroup).Value
          m_cumulativeTransform.Value 
        ) ;
        m_deltaTransform.CenterX = updateArgs.Position.X ;
        m_deltaTransform.CenterY = updateArgs.Position.Y ;
        // Look at the Delta property of the ManipulationDeltaRoutedEventArgs
        // to retrieve the rotation, X, and Y changes
        m_deltaTransform.Rotation   = updateArgs.Delta.Rotation ;
        m_deltaTransform.TranslateX = updateArgs.Delta.Translation.X ;
        m_deltaTransform.TranslateY = updateArgs.Delta.Translation.Y ;
        // Hmm, the description of Scale is WRONG ??? (Matteo??)
        m_deltaTransform.ScaleX = updateArgs.Delta.Scale ;
        m_deltaTransform.ScaleY = updateArgs.Delta.Scale ;
      }

      private Point m_dragStartPoint ;

      private void Dragging ( GestureRecognizer sender, DraggingEventArgs args )
      {
        System.Diagnostics.Debug.WriteLine(
          $"{args.DraggingState} [{args.Position.X:F3},{args.Position.Y:F3}"
        ) ;
        switch ( args.DraggingState )
        {
        case DraggingState.Started:
          m_dragStartPoint = args.Position ;
          m_previousTransform.Matrix = (
            m_cumulativeTransform.Value 
          ) ;
          m_deltaTransform.TranslateX = 0.0 ;
          m_deltaTransform.TranslateY = 0.0 ;
          m_deltaTransform.ScaleX = 1.0 ;
          m_deltaTransform.ScaleY = 1.0 ;
          break ;
        case DraggingState.Continuing:
          m_deltaTransform.TranslateX = args.Position.X - m_dragStartPoint.X ;
          m_deltaTransform.TranslateY = args.Position.Y - m_dragStartPoint.Y ;
          break ;
        case DraggingState.Completed:
          break ;
        }
      }

      public void Reset ( )
      {
        m_gestureRecognizer.CompleteGesture() ;
        InitializeTransforms() ;
      }

      // Route the pointer events to the gesture recognizer.
      // The points are in the reference frame of the canvas
      // that contains the target element.

      void OnPointerPressed ( object sender, PointerRoutedEventArgs args )
      {
        // Set the pointer capture to the element being interacted with
        // so that only it will fire pointer-related events
        m_targetElement.CapturePointer(args.Pointer) ;
        // Feed the current point into the gesture recognizer as a down event
        m_gestureRecognizer.ProcessDownEvent(
          args.GetCurrentPoint(m_referenceElement)
        ) ;
      }

      void OnPointerMoved ( object sender, PointerRoutedEventArgs args )
      {
        // Feed the set of points into the gesture recognizer as a move event
        m_gestureRecognizer.ProcessMoveEvents(
          args.GetIntermediatePoints(m_referenceElement)
        ) ;
      }

      void OnPointerReleased ( object sender, PointerRoutedEventArgs args )
      {
        // Feed the current point into the gesture recognizer as an up event
        m_gestureRecognizer.ProcessUpEvent(
          args.GetCurrentPoint(m_referenceElement)
        ) ;
        // Release the pointer
        m_targetElement.ReleasePointerCapture(args.Pointer) ;
      }

      void OnPointerCanceled ( object sender, PointerRoutedEventArgs args )
      {
        m_gestureRecognizer.CompleteGesture() ;
        m_targetElement.ReleasePointerCapture(args.Pointer) ;
      }

      void OnPointerWheelChanged ( object sender, PointerRoutedEventArgs args )
      {
        bool isShiftKeyDown = (
          Windows.UI.Core.CoreWindow.GetForCurrentThread(
          ).GetKeyState(
            Windows.System.VirtualKey.Shift
          ).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down)
        ) ;
        bool isShiftKeyDown_ASYNC = (
          Windows.UI.Core.CoreWindow.GetForCurrentThread(
          ).GetAsyncKeyState(
            Windows.System.VirtualKey.Shift
          ).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down)
        ) ;
        bool isCtrlKeyDown = (
          Windows.UI.Core.CoreWindow.GetForCurrentThread(
          ).GetKeyState(
            Windows.System.VirtualKey.Control
          ).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down)
        ) ;
        bool isCtrlKeyDown_ASYNC = (
          Windows.UI.Core.CoreWindow.GetForCurrentThread(
          ).GetAsyncKeyState(
            Windows.System.VirtualKey.Control
          ).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down)
        ) ;
        m_gestureRecognizer.ProcessMouseWheelEvent(
          args.GetCurrentPoint(m_referenceElement),
          isShiftKeyDown,
          isCtrlKeyDown
        ) ;
        // Matteo : which method is preferred ???
        System.Diagnostics.Debug.WriteLine(
          $"SHIFT {isShiftKeyDown} {isShiftKeyDown_ASYNC} ; CTRL {isCtrlKeyDown} {isCtrlKeyDown_ASYNC}"
        ) ;
      }

      // When a manipulation begins, change the color of the object
      // to reflect that a manipulation is in progress.

      void OnManipulationStarted ( object sender, ManipulationStartedEventArgs e )
      {
        Border b = m_targetElement as Border ;
        b.Background = new SolidColorBrush(Windows.UI.Colors.DeepSkyBlue) ;
      }

      // When a manipulation that's a result of inertia begins,
      // change the color of the the object to reflect that inertia has taken over.

      void OnManipulationInertiaStarting ( object sender, ManipulationInertiaStartingEventArgs e )
      {
        Border b = m_targetElement as Border ;
        b.Background = new SolidColorBrush(Windows.UI.Colors.RoyalBlue) ;
      }

      // When a manipulation has finished, reset the color of the object

      void OnManipulationCompleted ( object sender, ManipulationCompletedEventArgs e )
      {
        Border b = m_targetElement as Border ;
        b.Background = new SolidColorBrush(Windows.UI.Colors.LightGray) ;
      }

    }

  }

}
