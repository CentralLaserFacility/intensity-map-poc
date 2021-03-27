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
        referenceElement : mainCanvas
      ) ;
    }

    void resetButton_Pressed ( object sender, RoutedEventArgs e )
    {
      m_manipulationInputProcessor.Reset() ;
    }

    // Our ManipulationInputProcessor listens for events on the 'target' element
    // process them, and updates the target's position, size, and rotation
    // by manipulating its RenderTransform.

    class ManipulationInputProcessor
    {

      // Create a GestureRecognizer which will
      // process the manipulations done on the target

      private GestureRecognizer m_gestureRecognizer = new GestureRecognizer() ;

      private UIElement m_targetElement ;

      private UIElement m_referenceElement ;

      private TransformGroup m_renderTransform ;

      private MatrixTransform m_nominalTransform ;

      private CompositeTransform m_deltaTransform ;

      public ManipulationInputProcessor (
        UIElement target, 
        UIElement referenceElement
      ) {
        m_targetElement    = target ;
        m_referenceElement = referenceElement ;

        // Initialize the transforms that will be used to manipulate the shape

        InitializeTransforms() ;

        // GestureSettings dictate what manipulation events
        // the Gesture Recognizer will listen to
        // There are other properties that fine tune the behaviour,
        // but we're leavng those at the default settings.

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
        
        // These event handlers receive input events that are used by the gesture recognizer :

        m_targetElement.PointerPressed      += OnPointerPressed ;
        m_targetElement.PointerMoved        += OnPointerMoved ;
        m_targetElement.PointerReleased     += OnPointerReleased ;
        m_targetElement.PointerCanceled     += OnPointerCanceled ;
        m_targetElement.PointerWheelChanged += OnPointerWheelChanged ;

        // These event handlers respond to the Gesture Recognizer's output :

        m_gestureRecognizer.ManipulationStarted         += OnManipulationStarted ;
        m_gestureRecognizer.ManipulationUpdated         += OnManipulationUpdated ;
        m_gestureRecognizer.ManipulationCompleted       += OnManipulationCompleted ;
        m_gestureRecognizer.ManipulationInertiaStarting += OnManipulationInertiaStarting ;
        m_gestureRecognizer.Dragging                    += Dragging ;

        // Other event we could hook into :
        // m_gestureRecognizer.CrossSliding += (s,e) => { } ;
        // m_gestureRecognizer.Holding      += (s,e) => { } ;
        // m_gestureRecognizer.RightTapped  += (s,e) => { } ;
        // m_gestureRecognizer.Tapped       += (s,e) => { } ;

      }

      public void InitializeTransforms ( )
      {

        //
        // Our manipulations are going to affect the RenderTransform
        // that is being applied to the target element.
        // 
        // The RenderTransform, and the individual elements in its tree of Child transforms,
        // are all Dependency Properties. Consequently, any change we make in the tree
        // will immediately be reflected in the UI.
        //
        // We treat this as a group of two transforms that are applied in sequence,
        // first the 'nominal transform so far', and secondly the 'delta' transform
        // associated with the most recent manipulation.
        //
        // Since we need to access these two transforms individually, it's convenient
        // to maintain reference variables that refer to the two 'Child' elements
        // of the render transform, and the overall transform value.
        //

        m_targetElement.RenderTransform = m_renderTransform = new TransformGroup() ;
        m_renderTransform.Children.Add(
          m_nominalTransform = new MatrixTransform() {
            // Matrix = Matrix.Identity
          }
        ) ;
        m_renderTransform.Children.Add(
          m_deltaTransform = new CompositeTransform()
        ) ;

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
        m_nominalTransform.Matrix = (
          // (m_targetElement.RenderTransform as TransformGroup).Value
          m_renderTransform.Value 
        ) ;
        m_deltaTransform.CenterX = updateArgs.Position.X ;
        m_deltaTransform.CenterY = updateArgs.Position.Y ;
        m_deltaTransform.Rotation   = updateArgs.Delta.Rotation ;
        m_deltaTransform.TranslateX = updateArgs.Delta.Translation.X ;
        m_deltaTransform.TranslateY = updateArgs.Delta.Translation.Y ;
        // Hmm, the description of Scale is WRONG ??? (Matteo??)
        // https://docs.microsoft.com/en-us/uwp/api/windows.ui.input.manipulationdelta.scale
        // 'The change in distance between touch contacts, as a percentage.' NO, AS A FRACTION !!! 
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
          m_nominalTransform.Matrix = (
            m_renderTransform.Value 
          ) ;
          m_deltaTransform.TranslateX = 0.0 ;
          m_deltaTransform.TranslateY = 0.0 ;
          m_deltaTransform.Rotation   = 0.0 ;
          m_deltaTransform.ScaleX     = 1.0 ;
          m_deltaTransform.ScaleY     = 1.0 ;
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
        // so that only this element will fire pointer-related events
        m_targetElement.CapturePointer(args.Pointer) ;
        // Feed the current point into the gesture recognizer as a 'down' event
        m_gestureRecognizer.ProcessDownEvent(
          args.GetCurrentPoint(m_referenceElement)
        ) ;
      }

      void OnPointerMoved ( object sender, PointerRoutedEventArgs args )
      {
        // Feed the set of points into the gesture recognizer as a 'move' event
        m_gestureRecognizer.ProcessMoveEvents(
          args.GetIntermediatePoints(m_referenceElement)
        ) ;
      }

      void OnPointerReleased ( object sender, PointerRoutedEventArgs args )
      {
        // Feed the current point into the gesture recognizer as an 'up' event
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
        // MATTEO : which method is preferred ??? GetKeyState or GetAsyncKeyState ???
        // https://docs.microsoft.com/en-us/uwp/api/windows.ui.core.corewindow.getasynckeystate?view=winrt-19041
        // https://docs.microsoft.com/en-us/uwp/api/windows.ui.core.corewindow.getkeystate?view=winrt-19041
        // The docs for both flavours are identical, but ...
        // https://stackoverflow.com/questions/17770753/getkeystate-vs-getasynckeystate-vs-getch
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
        //
        // With neither SHIFT of CTRL pressed, the default behaviour
        // is to scroll up and down. However that's not very successful
        // if you scroll to a position where the pointer is no longer
        // above the target element, as the target ceases to receive
        // further wheel events. Hmm, could do better ...
        //
        m_gestureRecognizer.ProcessMouseWheelEvent(
          args.GetCurrentPoint(m_referenceElement),
          isShiftKeyDown,
          isCtrlKeyDown
        ) ;
        System.Diagnostics.Debug.WriteLine(
          $"SHIFT {isShiftKeyDown} {isShiftKeyDown_ASYNC} ; CTRL {isCtrlKeyDown} {isCtrlKeyDown_ASYNC}"
        ) ;
      }

      // When a manipulation begins, we change the color of the target
      // to indicate that a manipulation is in progress.

      void OnManipulationStarted ( object sender, ManipulationStartedEventArgs e )
      {
        Border b = m_targetElement as Border ;
        b.Background = new SolidColorBrush(Windows.UI.Colors.DeepSkyBlue) ;
      }

      // When a manipulation that's a result of inertia begins,
      // we change the color of the the object to indicate that inertia has taken over.

      void OnManipulationInertiaStarting ( object sender, ManipulationInertiaStartingEventArgs e )
      {
        Border b = m_targetElement as Border ;
        b.Background = new SolidColorBrush(Windows.UI.Colors.RoyalBlue) ;
      }

      // When a manipulation has finished, we reset the color of the target

      void OnManipulationCompleted ( object sender, ManipulationCompletedEventArgs e )
      {
        Border b = m_targetElement as Border ;
        b.Background = new SolidColorBrush(Windows.UI.Colors.LightGray) ;
      }

    }

  }

}
