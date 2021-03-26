using Windows.Foundation ;
using Windows.UI.Xaml ;
using Windows.UI.Xaml.Controls ;
using Windows.UI.Xaml.Input ;
using Windows.UI.Xaml.Media ;
using Windows.UI.Input ;

namespace Experiments_01_UWP
{

  public sealed partial class GestureRecogniserSample_Original_UserControl : UserControl
  {

    private ManipulationInputProcessor m_manipulationInputProcessor ;

    public GestureRecogniserSample_Original_UserControl ( )
    {
      this.InitializeComponent() ;

      InitOptions() ;

      // Create a ManipulationInputProcessor which will listen for events on the
      // rectangle, process them, and update the rectangle's position, size, and rotation

      m_manipulationInputProcessor = new ManipulationInputProcessor(
        manipulateMe, 
        mainCanvas
      ) ;
    }

    private void InitOptions ( )
    {
      movementAxis.SelectedIndex = 0 ;
      InertiaSwitch.IsOn = true ;
    }

    private void movementAxis_Changed ( object sender, SelectionChangedEventArgs e )
    {
      if ( m_manipulationInputProcessor == null )
      {
        return ;
      }

      ComboBoxItem selectedItem = (ComboBoxItem) (
        (ComboBox) sender
      ).SelectedItem ;
      switch ( selectedItem.Content.ToString() )
      {
      case "X only":
        m_manipulationInputProcessor.LockToXAxis() ;
        break ;
      case "Y only":
        m_manipulationInputProcessor.LockToYAxis() ;
        break ;
      default:
        m_manipulationInputProcessor.MoveOnXAndYAxes() ;
        break ;
      }
    }

    private void InertiaSwitch_Toggled ( object sender, RoutedEventArgs e )
    {
      if ( m_manipulationInputProcessor == null)
      {
      return ;
      }
      m_manipulationInputProcessor.UseInertia(InertiaSwitch.IsOn) ;
    }

    void resetButton_Pressed ( object sender, RoutedEventArgs e )
    {
      InitOptions() ;
      m_manipulationInputProcessor.Reset() ;
    }

  }

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

      // The GestureSettings property dictates what manipulation events
      // the Gesture Recognizer will listen to. Here we set it to a limited
      // subset of these events.

      m_gestureRecognizer.GestureSettings = GenerateDefaultSettings() ;

      // Set up pointer event handlers.
      // These receive input events
      // that are used by the gesture recognizer.

      m_targetElement.PointerPressed      += OnPointerPressed ;
      m_targetElement.PointerMoved        += OnPointerMoved ;
      m_targetElement.PointerReleased     += OnPointerReleased ;
      m_targetElement.PointerCanceled     += OnPointerCanceled ;
      m_targetElement.PointerWheelChanged += OnPointerWheelChanged ;

      // Set up event handlers to respond to gesture recognizer output

      m_gestureRecognizer.ManipulationStarted         += OnManipulationStarted ;
      m_gestureRecognizer.ManipulationUpdated         += OnManipulationUpdated ;
      m_gestureRecognizer.ManipulationCompleted       += OnManipulationCompleted ;
      m_gestureRecognizer.ManipulationInertiaStarting += OnManipulationInertiaStarting ;
    }

    public void InitializeTransforms ( )
    {
      m_cumulativeTransform = new TransformGroup() ;
      m_deltaTransform      = new CompositeTransform() ;
      m_previousTransform   = new MatrixTransform() { 
        Matrix = Matrix.Identity 
      } ;

      m_cumulativeTransform.Children.Add(m_previousTransform) ;
      m_cumulativeTransform.Children.Add(m_deltaTransform) ;

      m_targetElement.RenderTransform = m_cumulativeTransform ;
    }

    // Return the default GestureSettings for this sample

    GestureSettings GenerateDefaultSettings ( )
    {
      return (
        GestureSettings.ManipulationTranslateX
      | GestureSettings.ManipulationTranslateY 
      | GestureSettings.ManipulationTranslateInertia 
      | GestureSettings.ManipulationRotate 
      | GestureSettings.ManipulationRotateInertia 
      // STEVET
      | GestureSettings.ManipulationScale 
      | GestureSettings.ManipulationScaleInertia 
      | GestureSettings.Drag
      ) ;
    }

    // Route the pointer pressed event to the gesture recognizer.
    // The points are in the reference frame of the canvas that contains the rectangle element.

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

    // Route the pointer moved event to the gesture recognizer.
    // The points are in the reference frame of the canvas
    // that contains the rectangle element.

    void OnPointerMoved ( object sender, PointerRoutedEventArgs args )
    {
      // Feed the set of points into the gesture recognizer as a move event
      m_gestureRecognizer.ProcessMoveEvents(
        args.GetIntermediatePoints(m_referenceElement)
      ) ;
    }

    // Route the pointer released event to the gesture recognizer.
    // The points are in the reference frame of the canvas that contains the rectangle element.

    void OnPointerReleased ( object sender, PointerRoutedEventArgs args )
    {
      // Feed the current point into the gesture recognizer as an up event
      m_gestureRecognizer.ProcessUpEvent(
        args.GetCurrentPoint(m_referenceElement)
      ) ;
      // Release the pointer
      m_targetElement.ReleasePointerCapture(args.Pointer) ;
    }

    // Route the pointer canceled event to the gesture recognizer.
    // The points are in the reference frame of the canvas that contains the rectangle element.

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
      System.Diagnostics.Debug.WriteLine(
        $"SHIFT {isShiftKeyDown} {isShiftKeyDown_ASYNC} ; CTRL {isCtrlKeyDown} {isCtrlKeyDown_ASYNC}"
      ) ;
    }

    // When a manipulation begins, change the color of the object
    // to reflect that a manipulation is in progress

    void OnManipulationStarted ( object sender, ManipulationStartedEventArgs e )
    {
      Border b = m_targetElement as Border ;
      b.Background = new SolidColorBrush(Windows.UI.Colors.DeepSkyBlue) ;
    }

    // Process the change resulting from a manipulation

    void OnManipulationUpdated ( object sender, ManipulationUpdatedEventArgs e )
    {
      m_previousTransform.Matrix = m_cumulativeTransform.Value ;
      // Get the center point of the manipulation for rotation
      Point center = new Point(
        e.Position.X, 
        e.Position.Y
      ) ;
      m_deltaTransform.CenterX = center.X ;
      m_deltaTransform.CenterY = center.Y ;
      // Look at the Delta property of the ManipulationDeltaRoutedEventArgs
      // to retrieve the rotation, X, and Y changes
      m_deltaTransform.Rotation   = e.Delta.Rotation ;
      m_deltaTransform.TranslateX = e.Delta.Translation.X ;
      m_deltaTransform.TranslateY = e.Delta.Translation.Y ;
      // Hmm, the description of Scale is WRONG ???
      m_deltaTransform.ScaleX     = e.Delta.Scale ;
      m_deltaTransform.ScaleY     = e.Delta.Scale ;
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

    // Modify the GestureSettings property to only allow movement on the X axis

    public void LockToXAxis ( )
    {
      m_gestureRecognizer.CompleteGesture() ;
      m_gestureRecognizer.GestureSettings |= (
        GestureSettings.ManipulationTranslateY 
      | GestureSettings.ManipulationTranslateX 
      ) ;
      m_gestureRecognizer.GestureSettings ^= GestureSettings.ManipulationTranslateY ;
    }

    // Modify the GestureSettings property to only allow movement on the Y axis

    public void LockToYAxis()
    {
      m_gestureRecognizer.CompleteGesture() ;
      m_gestureRecognizer.GestureSettings |= (
        GestureSettings.ManipulationTranslateY 
      | GestureSettings.ManipulationTranslateX 
      ) ;
      m_gestureRecognizer.GestureSettings ^= GestureSettings.ManipulationTranslateX ;
    }

    // Modify the GestureSettings property
    // to allow movement on both the the X and Y axes

    public void MoveOnXAndYAxes()
    {
      m_gestureRecognizer.CompleteGesture() ;
      m_gestureRecognizer.GestureSettings |= (
        GestureSettings.ManipulationTranslateX 
      | GestureSettings.ManipulationTranslateY 
      ) ;
    }

    // Modify the GestureSettings property to enable or disable inertia based on the passed-in value

    public void UseInertia ( bool inertia )
    {
      if ( ! inertia )
      {
        m_gestureRecognizer.CompleteGesture() ;
        m_gestureRecognizer.GestureSettings ^= (
          GestureSettings.ManipulationTranslateInertia 
        | GestureSettings.ManipulationRotateInertia 
        ) ;
      }
      else
      {
        m_gestureRecognizer.GestureSettings |= (
          GestureSettings.ManipulationTranslateInertia 
        | GestureSettings.ManipulationRotateInertia 
        ) ;
      }
    }

    public void Reset ( )
    {
      m_targetElement.RenderTransform = null ;
      m_gestureRecognizer.CompleteGesture() ;
      InitializeTransforms() ;
      m_gestureRecognizer.GestureSettings = GenerateDefaultSettings() ;
    }

  }

}
