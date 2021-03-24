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
using Windows.UI.Input ;

using Common.ExtensionMethods ;

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

  public record Gesture ( ) ;

  public record PanGesture ( ) : Gesture ;

  public record ZoomGesture ( ) : Gesture ;

  public record PanGesture_Starting ( ) : PanGesture ;

  public record PanGesture_Changing ( FractionalXY deltaFromStartPoint ) : PanGesture ;

  public record PanGesture_Finished ( ) : PanGesture ;

  public record ZoomInGesture ( FractionalXY AnchorPoint ) : ZoomGesture ;

  public record ZoomOutGesture ( FractionalXY AnchorPoint ) : ZoomGesture ;

  public record PositionChangeNotification ( FractionalXY? CurrentPosition ) : Gesture ;

  public record FractionalXY ( double X, double Y ) ;

  public class MouseEventHandler
  {

    private UIElement m_target ;

    private enum MouseEventType {
      Entered,
      Moved,
      WheelChanged,
      Pressed,
      Released,
      Exited,
      Canceled,
      CaptureLost
    }

    private record IncomingMouseEventDescriptor (
      MouseEventType       EventType,
      MouseStateDescriptor CurrentMouseState,
      MouseStateDescriptor PreviousMouseState
    ) ;

    public record MouseStateDescriptor (
        FractionalXY FractionalPosition,
        bool         IsLeftButtonPressed,
        bool         IsShiftKeyDown
    ) {
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
            Windows.System.VirtualKey.Shift
          ).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down)
        ) ;
        return new(
          fractionalPosition,
          isLeftButtonPressed,
          isShiftKeyDown
        ) ;
      }
    }

    private System.Action<Gesture> m_gestureRecognisedAction ;

    public MouseStateDescriptor CurrentMouseState ;

    public MouseEventHandler ( UIElement target, System.Action<Gesture> gestureRecognisedAction )
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

    private void HandleMouseEvent ( IncomingMouseEventDescriptor mouseEventDescriptor )
    {
    }

    private Point? m_mostRecentlyReportedMousePosition = null ;

    private void HandleMouseEvent ( object sender, PointerRoutedEventArgs pointerEventArgs, MouseEventType eventType )
    {

      // Prevent most handlers along the event route
      // from handling the same event again.

      pointerEventArgs.Handled = true ;

      //
      // The 'Pointer' class has the following properties :
      //
      //  PointerId         integer
      //  PointerDeviceType (Touch,Pen,Mouse)
      //  TimeStamp         microsecs since boot
      //  IsInContact
      //  IsInRange
      //

      // Pointer pointer = pointerEventArgs.Pointer ;

      //
      // The 'PointerPoint' class has the following properties :
      //
      //  PointerId         integer
      //  Position          In 'client' coordinates relative to a specified UI element
      //  Properties        PointerPointProperties - 'extended properties'
      //                    - PointerUpdateKind
      //                        Other                = 0,
      //                        LeftButtonPressed    = 1,
      //                        LeftButtonReleased   = 2,
      //                        RightButtonPressed   = 3,
      //                        RightButtonReleased  = 4,
      //                        MiddleButtonPressed  = 5,
      //                        MiddleButtonReleased = 6,
      //                    - IsLeftButtonPressed
      //                    - IsRightButtonPressed
      //                    - IsMiddleButtonPressed
      //                    - MouseWheelDelta
      //                    - Pressure, Orientation, Twist,
      //                      XTilt, YTilt, ZDistance,
      //                      IsBarrelButtonPressed, IsEraser
      //                    - IsLeftButtonPressed
      //
      // Also (not so interesting)
      //  IsInContact
      //  PointerDevice Can report max number of contacts, for a Touch device
      //  

      // PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(target) ;

      var previousMouseState = CurrentMouseState ;
      CurrentMouseState = MouseStateDescriptor.Create(
        pointerEventArgs,
        m_target
      ) ;
      HandleMouseEvent(
        new(
          eventType,
          CurrentMouseState,
          previousMouseState
        )
      ) ;
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
    //
    // PointerReleased fires when a button ceases to be pressed.
    //

    private void Target_PointerPressed ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MouseEventType.Pressed) ;
    }

    private void Target_PointerEntered ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MouseEventType.Entered) ;
    }

    private void Target_PointerMoved ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MouseEventType.Moved) ;
    }

    private void Target_PointerWheelChanged ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MouseEventType.WheelChanged) ;
    }

    private void Target_PointerReleased ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MouseEventType.Released) ;
    }

    private void Target_PointerCaptureLost ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MouseEventType.CaptureLost) ;
    }

    private void Target_PointerCanceled ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MouseEventType.Canceled) ;
    }

    private void Target_PointerExited ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      HandleMouseEvent(sender,pointerEventArgs,MouseEventType.Exited) ;
    }

  }

}
