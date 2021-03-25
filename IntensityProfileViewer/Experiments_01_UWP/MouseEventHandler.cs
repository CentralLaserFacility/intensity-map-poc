//
// MouseEventHandler.cs
//

using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Input;

using Common.ExtensionMethods;

namespace Experiments_01_UWP
{

  //
  // Listens for relevant 'low-level' mouse events on a UI element,
  // and raises appropriate higher-level 'gesture' events
  // that are relevant for controlling pan/zoom settings :
  //
  //   PanGesture_Starting
  //
  //     Client should take a snapshot of current 'pan' position
  //     and save this as the 'original' pan position.
  //     Some number of 'Moves' are now expected.
  //
  //   PanGesture_Changing [dx_frac01,dy_frac01]
  //     Client should adjust the panning offsets to be 
  //     the saved 'original' position moved by the specified delta.
  //     The delta values are scaled to the nominal size of the panel.
  //
  //   PanGesture_Finished
  //
  //     Client should forget the originally saved position.
  //
  //   ZoomGesture [x_frac01,y_frac01,in/out]
  //
  //     Client should implement a zoom-in or zoom-out
  //     centred on the specified x/y position.
  //
  //   MoveNotification [x_frac01,y_frac01]
  //
  //     Client is being informed that the position of the mouse has changed.
  //     It might wish to indicate the current position with a graphic on the UI.
  //     A 'null' position value indicates the mouse is absent.
  //
  // Note that all coordinates are specified in FRACTIONAL-X-Y coordinates : 
  //
  //     [0,0]
  //       +------------------+
  //       |                  |  'top left'     is at [0.0,0.0]
  //       |                  |
  //       |                  |  'bottom right' is at [1.0,1.0]
  //       |                  |
  //       |                  |
  //       |                  |
  //       +------------------+
  //                        [1,1]
  //
  // This convention lets us decouple the coordinate system of the UI Element
  // that we're using to detect pan/zoom gestures, from the coordinate system
  // to which the pan/zoom settings will be applied (typically via a Transform).
  //

  public partial class MouseEventHandler
  {

    private UIElement m_target ;

    public enum MousePhysicalEventType {
      // Physical events
      Entered,
      Moved,
      WheelChanged,
      Pressed,
      Released,
      Exited,
      Canceled,
      CaptureLost,
      // Synthetic events ?
      // CaptureGained,
      // MovedSignificantly,
      // LeftButtonPressed,
      // LeftButtonReleased,
      // RightButtonPressed,
      // RightButtonReleased
    }

    public record IncomingMouseEventDescriptor (
      MousePhysicalEventType       EventType,
      MouseDeltaDescriptor MouseDeltaDescriptor,
      MouseStateDescriptor CurrentMouseState //,
      // MouseStateDescriptor PreviousMouseState
    ) {
      public override string ToString ( ) => $"{EventType}, new state is {CurrentMouseState}" ;
    } ;

    public record MouseStateDescriptor (
      FractionalXY FractionalPosition,
      bool         WasLeftButtonDown,
      bool         WasShiftKeyDown,
      bool         WasCtrlKeyDown
    ) {

      public bool InsideTargetRegion => FractionalPosition.IsInsideNominalBounds ;

      public override string ToString ( ) => (
        $"XY : {FractionalPosition}"
      + $"{(WasLeftButtonDown?" LEFT-DOWN":"")}"
      + $"{(WasShiftKeyDown?" SHIFT-DOWN":"")}" 
      + $"{(WasCtrlKeyDown?" CTRL-DOWN":"")}" 
      ) ;

      public static MouseStateDescriptor Create ( 
        PointerRoutedEventArgs pointerEventArgs,
        UIElement              target
      ) {
        PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(target) ;
        Point position = pointerPoint.Position ;
        FractionalXY fractionalPosition = new(
          position.X / target.ActualSize.X,
          position.Y / target.ActualSize.Y
        ) ;
        bool isLeftButtonPressed = pointerPoint.Properties.IsLeftButtonPressed ;
        bool isShiftKeyDown = (
          Windows.UI.Core.CoreWindow.GetForCurrentThread(
          ).GetAsyncKeyState(
            Windows.System.VirtualKey.Control
          ).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down)
        ) ;
        bool isCtrlKeyDown = (
          Windows.UI.Core.CoreWindow.GetForCurrentThread(
          ).GetAsyncKeyState(
            Windows.System.VirtualKey.Shift
          ).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down)
        ) ;
        return new(
          fractionalPosition,
          isLeftButtonPressed,
          isShiftKeyDown,
          isCtrlKeyDown
        ) ;
      }
    }

    private System.Action<MouseGesture> m_gestureRecognisedAction ;

    public System.Action<IncomingMouseEventDescriptor>? IncomingMouseEventReceived = null ;

    public MouseStateDescriptor CurrentMouseState ;

    public MouseEventHandler ( UIElement target, System.Action<MouseGesture> gestureRecognisedAction )
    {
      m_target                  = target ;
      m_gestureRecognisedAction = gestureRecognisedAction ;
      m_target.PointerEntered      += new PointerEventHandler(Target_PointerEntered) ;
      m_target.PointerMoved        += new PointerEventHandler(Target_PointerMoved) ;
      m_target.PointerWheelChanged += new PointerEventHandler(Target_PointerWheelChanged) ;
      m_target.PointerPressed      += new PointerEventHandler(Target_PointerPressed) ;
      m_target.PointerReleased     += new PointerEventHandler(Target_PointerReleased) ;
      m_target.PointerExited       += new PointerEventHandler(Target_PointerExited) ;
      m_target.PointerCanceled     += new PointerEventHandler(Target_PointerCanceled) ;
      m_target.PointerCaptureLost  += new PointerEventHandler(Target_PointerCaptureLost) ;
    }

    private IncomingMouseEventDescriptor? m_previousMouseEvent ;

    private void HandleMouseEvent ( IncomingMouseEventDescriptor mouseEventDescriptor )
    {
      IncomingMouseEventReceived?.Invoke(mouseEventDescriptor) ;
      var gesture = DoGestureRecognition(
        mouseEventDescriptor,
        m_previousMouseEvent ?? mouseEventDescriptor
      ) ;
      m_previousMouseEvent = mouseEventDescriptor ;
      if ( gesture != null )
      {
        m_gestureRecognisedAction(gesture) ;
      }
    }

    private static MouseGesture? DoGestureRecognition (
      IncomingMouseEventDescriptor latestMouseEventDescriptor,
      IncomingMouseEventDescriptor previousMouseEventDescriptor
    ) {
      return null ;
    }

    private MouseStateDescriptor? m_previousMouseState ;

    private void HandleMouseEvent ( object sender, PointerRoutedEventArgs pointerEventArgs, MousePhysicalEventType eventType )
    {

      //
      // Here we map the low level 'physical' events into higher level events.
      // Yeah it's a bit complicated and somewhat messy, but better to deal with
      // the messiness all in one place than have complicated tests sprinkled around
      // in other places ...
      //

      if ( pointerEventArgs.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse )
      {
        // We're only interested in mouse events ...
        return ;
      }

      // Let's prevent most other handlers along the event route
      // from handling thisevent again ...

      pointerEventArgs.Handled = true ;

      // This 'PointerPoint' object will give us detailed information
      // about the event, eg button states and so on ...

      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;

      //
      // Raise a Mouse Event with an appropriate 'IncomingMouseEventDescriptor'
      // that describes (A) what happened, and (B) the current state at the time
      // the event occurred .
      // The 'state' includes not only mouse-related information such as
      // the mouse pointer position and whether the left and/or right buttons
      // were down, but also other information that might influence how we
      // respond to the event, eg whether the keyboard 'shift' key was depressed.
      //

      CurrentMouseState = MouseStateDescriptor.Create(
        pointerEventArgs,
        m_target
      ) ;

      var previousMouseState = m_previousMouseState ?? CurrentMouseState ;

      var currentPosition = CurrentMouseState.FractionalPosition ;

      //FractionalXY delta = (
      //  m_previouslyReportedMousePosition is null
      //  ? FractionalXY.Zero
      //  : currentPosition - m_previouslyReportedMousePosition
      //) ;
      //m_previouslyReportedMousePosition = currentPosition ;

      MouseDeltaDescriptor? mouseDeltaDescriptor = eventType switch {
        MousePhysicalEventType.Entered => new EnteredActiveRegion(),
        MousePhysicalEventType.Moved => new PositionChanged(
          CurrentMouseState.FractionalPosition
        - previousMouseState.FractionalPosition
        ),
        MousePhysicalEventType.WheelChanged => (
          pointerPoint.Properties.MouseWheelDelta > 0
          ? new WheelNudgedForwards()
          : new WheelNudgedBackwards()
        ),
        MousePhysicalEventType.Pressed => (
          // Hmm, just a quick HACK !!!
          // Need to diff with previous ...
          pointerPoint.Properties.IsLeftButtonPressed
          ? new LeftButtonPressed()
          : pointerPoint.Properties.IsRightButtonPressed
            ? new RightButtonPressed()
            : null
        ),
        MousePhysicalEventType.Released => new LeftButtonReleased(), // new RightButtonReleased()
        MousePhysicalEventType.Exited => new ExitedActiveRegion(),
        MousePhysicalEventType.Canceled => null,
        MousePhysicalEventType.CaptureLost => null,
        _ => null
      } ;

      if ( mouseDeltaDescriptor is not null )
      {
        HandleMouseEvent(
          new IncomingMouseEventDescriptor(
            eventType,
            mouseDeltaDescriptor,
            CurrentMouseState// ,
            // previousMouseState
          )
        ) ;
      }

      m_previousMouseState = CurrentMouseState ;

    }

    // 
    // PointerEntered fires under the following circumstances :
    //
    //   - Mouse pointer moved from 'outside' the target area to 'inside'.
    //   - Touch on the target area, or swiped in from 'outside' the target area.
    //   - Pen coming in directly to be in range over the target area, or swiped in from outside.
    //
    // PointerPressed fires under the following circumstances :
    //
    //   - Mouse pointer moved from 'outside' the target area to 'inside'.
    //   - Touch on the target area, or swiped in from 'outside' the target area.
    //   - Pen coming in directly to be in range over the target area, or swiped in from outside.
    //
    // PointerPressed and PointerReleased don't always occur in pairs.
    //
    // Your app should listen for and handle any event
    // that can 'conclude' a pointer down :
    //
    //   PointerExited
    //   PointerCanceled
    //   PointerCaptureLost
    //
    // PointerMoved :
    //
    //   Multiple, simultaneous mouse button inputs are processed here.
    //   
    //   Mouse input is associated with a single pointer
    //   that is assigned when mouse input is first detected.
    //   
    //   Clicking additional mouse buttons (left, wheel, or right) during
    //   the interaction creates secondary associations between those buttons
    //   and the pointer through the 'pointer pressed' event.
    //   
    //   The 'pointer released' event is fired only when the last mouse button
    //   associated with the interaction is released ; this is not necessarily
    //   the initial button.
    //   
    //   Because of this exclusive association, other mouse button clicks
    //   are routed through the 'pointer move' event.
    //
    // PointerReleased fires when a button ceases to be pressed.
    //
    // PointerCaptureLost fires for various reasons, including :
    //
    //   - User interactions
    //   - Programmatic capture of another pointer
    //   - Captured pointer was deliberately released
    //   
    //   PointerCaptureLost can fire instead of PointerReleased.
    //
    //
    // PointerCanceled fires for for various reasons, including:
    //
    //   - The desktop is locked or the user logged off
    //   - Touch contact is canceled by a Pen coming into range of the surface
    //   - The touch device doesn't report an active contact for more than 100ms
    //   - The number of simultaneous touch contacts exceeded the number supported by the device
    //

    //
    // In all the event handlers, 'pointerEventArgs' provides Pointer information
    // with instances of the following classes :
    //
    //   Pointer      pointer      = pointerEventArgs.Pointer ;
    //   PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(target) ;
    //
    // The 'Pointer' class has the following properties :
    //
    //   PointerId         Integer
    //   PointerDeviceType Touch,Pen,Mouse
    //   TimeStamp         Microsecs since boot
    //   IsInContact       Touch only
    //   IsInRange         Pen only
    //
    // The 'PointerPoint' class has the following properties :
    //
    //   PointerId         Integer
    //   Position          In 'client' coordinates relative to a specified UI element
    //   PointerDevice     For a Touch device, report max number of contacts
    //   IsInContact
    //   Properties        PointerPointProperties : 'extended properties'
    //                     - PointerUpdateKind
    //                         Other                = 0
    //                         LeftButtonPressed    = 1
    //                         LeftButtonReleased   = 2
    //                         RightButtonPressed   = 3
    //                         RightButtonReleased  = 4
    //                         MiddleButtonPressed  = 5
    //                         MiddleButtonReleased = 6
    //                     - IsLeftButtonPressed
    //                     - IsRightButtonPressed
    //                     - IsMiddleButtonPressed
    //                     - MouseWheelDelta
    //                     - Pressure, Orientation, Twist,
    //                       XTilt, YTilt, ZDistance,
    //                       IsBarrelButtonPressed, IsEraser
    //                     - IsLeftButtonPressed
    //  

    private void Target_PointerPressed ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MousePhysicalEventType.Pressed) ;
    }

    private void Target_PointerEntered ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MousePhysicalEventType.Entered) ;
    }

    private void Target_PointerMoved ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MousePhysicalEventType.Moved) ;
    }

    private void Target_PointerWheelChanged ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MousePhysicalEventType.WheelChanged) ;
    }

    private void Target_PointerReleased ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MousePhysicalEventType.Released) ;
    }

    private void Target_PointerCaptureLost ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MousePhysicalEventType.CaptureLost) ;
    }

    private void Target_PointerCanceled ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MousePhysicalEventType.Canceled) ;
    }

    private void Target_PointerExited ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MousePhysicalEventType.Exited) ;
    }

  }

}
