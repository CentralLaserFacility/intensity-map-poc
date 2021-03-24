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
  // Listens for relevant events on a UI element,
  // and raises appropriate higher-level 'gesture' events
  // that are relevant for controlling pan/zoom settings :
  //
  //   PanGesture_Start
  //     Client should take a snapshot of current 'pan' position
  //     and save this as the 'original' pan position.
  //     Some number of 'Moves' are now expected.
  //   PanGesture_Move [dx_frac01,dy_frac01]
  //     Client should adjust the panning offsets to be 
  //     the saved 'original' position moved by the specified delta.
  //   PanGesture_End
  //     Client should forget the originally saved position.
  //
  //   ZoomGesture [x_frac01,y_frac01,in/out]
  //     Client should implement a zoom-in or zoom-out
  //     centred on the specified x/y position.
  //
  //   MoveNotification [x_frac01,y_frac01]
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
  // to which the pan/zoom settings will be applied.
  //

  public record MouseEvent ( ) ;

  public record MouseDragStartedEvent ( ) : MouseEvent ;

  public record MouseDragMoveEvent ( ) : MouseEvent ;

  public record MouseDragEndedEvent ( )  : MouseEvent ;

  public record Gesture ( ) ;
  public record PanGesture ( ) : Gesture ;
  public record ZoomGesture ( )  : Gesture;
  public record PanGesture_Starting ( ) : PanGesture ;
  public record PanGesture_Changing ( FractionalXY deltaFromStartPoint ) : PanGesture ;
  public record PanGesture_Finished ( ) : PanGesture ;
  public record ZoomInGesture ( FractionalXY AnchorPoint ) : ZoomGesture ;
  public record ZoomOutGesture ( FractionalXY AnchorPoint ) : ZoomGesture ;
  public record HoveringNotification ( FractionalXY? CurrentHoverPosition ) : Gesture ;

  public record FractionalXY ( double X_Frac01, double Y_frac01 ) ;

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
      MouseEventType EventType 
    ) ;

    public MouseEventHandler ( UIElement target )
    {
      m_target = target ;
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
      pointerEventArgs.Handled = true ;
      Pointer      pointer      = pointerEventArgs.Pointer ;
      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;
      Point        position     = pointerPoint.Position ;
      HandleMouseEvent(
        new(
          eventType
        )
      ) ;
    }

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
