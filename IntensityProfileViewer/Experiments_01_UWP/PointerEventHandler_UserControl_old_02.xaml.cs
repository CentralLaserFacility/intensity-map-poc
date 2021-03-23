using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Input ;

using Common.ExtensionMethods ;

//
// Adapted from this sampe code which shows how to handle 'Pointer' events
//   https://docs.microsoft.com/en-us/windows/uwp/design/input/handle-pointer-input
//
// Also see these examples, which also demonstrate Gesture handling ...
//   https://github.com/microsoft/Windows-universal-samples/tree/master/Samples/BasicInput/cs
//
// Matteo : which of these are the most up to date ??
//

namespace Experiments_01_UWP
{

  public sealed partial class PointerEventHandler_UserControl_old_02 : UserControl
  {

    //
    // This dictionary maintains information about each active 'contact'.
    //
    // An entry is added during these events : 
    //
    //   PointerPressed
    //   PointerEntered
    //
    // ... and removed during these events :
    //
    //   PointerReleased
    //   PointerCaptureLost
    //   PointerCanceled
    //   PointerExited
    //

    //
    // A pointer raises various events :
    //
    //   PointerEntered      
    //   PointerExited       
    //
    //   PointerMoved
    //
    //   PointerPressed      
    //   PointerReleased     
    //
    //   PointerWheelChanged 
    //   PointerCanceled
    //
    //   PointerCaptureLost
    // 

    private readonly Dictionary<uint,Windows.UI.Xaml.Input.Pointer> m_activeContactsDictionary = new() ;

    public PointerEventHandler_UserControl_old_02 ( )
    {
      this.InitializeComponent() ;

      m_target.PointerEntered      += new PointerEventHandler(Target_PointerEntered) ;
      m_target.PointerMoved        += new PointerEventHandler(Target_PointerMoved) ;
      m_target.PointerWheelChanged += new PointerEventHandler(Target_PointerWheelChanged) ;
      m_target.PointerPressed      += new PointerEventHandler(Target_PointerPressed) ;
      m_target.PointerReleased     += new PointerEventHandler(Target_PointerReleased) ;
      m_target.PointerExited       += new PointerEventHandler(Target_PointerExited) ;
      m_target.PointerCanceled     += new PointerEventHandler(Target_PointerCanceled) ;
      m_target.PointerCaptureLost  += new PointerEventHandler(Target_PointerCaptureLost) ;

    }

    private void DeclarePointerContactStarted ( Windows.UI.Xaml.Input.Pointer pointer )
    {
      if ( ! m_activeContactsDictionary.ContainsKey(pointer.PointerId) )
      {
        m_activeContactsDictionary[pointer.PointerId] = pointer ;
      }
    }

    private void DeclarePointerContactFinished ( Windows.UI.Xaml.Input.Pointer pointer )
    {
      if ( m_activeContactsDictionary.ContainsKey(pointer.PointerId) )
      {
        m_activeContactsDictionary[pointer.PointerId] = null ;
        m_activeContactsDictionary.Remove(pointer.PointerId) ;
      }
    }

    private int m_messageNumber = 0 ;

    private void WriteLogMessage ( string textLine )
    {
      Common.DebugHelpers.WriteDebugLines(
        $"[{++m_messageNumber:D03}] {textLine}"
      ) ;
    }

    private static double TextBlockOffsetXY = 20.0 ;

    private void AddPointerInfoTextToCanvas ( PointerPoint pointerPoint )
    {
      m_canvas.Children.Add(
        new TextBlock() {
          Tag             = pointerPoint.PointerId,
          Foreground      = new SolidColorBrush(Windows.UI.Colors.White),
          FontFamily      = new FontFamily("Consolas"),
          Text            = GetPointerInfoToDisplay(pointerPoint),
          RenderTransform = new TranslateTransform() {
            X = pointerPoint.Position.X + TextBlockOffsetXY,
            Y = pointerPoint.Position.Y + TextBlockOffsetXY
          }
        }
      ) ;
    }

    private void UpdatePointerInfoTextOnCanvas ( PointerPoint pointerPoint )
    {
      m_canvas.Children.OfType<TextBlock>().Where(
        textBlock => (uint) textBlock.Tag == pointerPoint.PointerId
      ).ToList().ForEach(
        textBlock => {
          textBlock.RenderTransform = new TranslateTransform() {
            X = pointerPoint.Position.X + TextBlockOffsetXY,
            Y = pointerPoint.Position.Y + TextBlockOffsetXY
          } ;
          textBlock.Text = GetPointerInfoToDisplay(pointerPoint) ;
        }
      ) ;
    }

    private void RemovePointerInfoTextFromCanvas ( PointerPoint pointerPoint )
    {
      m_canvas.Children.OfType<TextBlock>().Where(
        textBlock => (uint) textBlock.Tag == pointerPoint.PointerId
      ).ToList().ForEach(
        textBlock => {
          m_canvas.Children.Remove(textBlock) ;
        }
      ) ;
    }

    //
    // PointerPressed and PointerReleased don't always occur in pairs.
    //
    // Your app should listen for and handle
    // any event that can conclude a pointer down :
    //   PointerExited
    //   PointerCanceled
    //   PointerCaptureLost
    // 

    private void Target_PointerPressed ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      Pointer      pointer      = pointerEventArgs.Pointer ;
      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;
      Point        position     = pointerPoint.Position ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} pressed at [{position.X:F3},{position.Y:F3}]"
      ) ;

      // Attempt to 'capture' the pointer, so that subsequent 'move' events
      // will be sent to the target even though the pointer isn't over it.

      bool captureSucceeded = m_target.CapturePointer(pointer) ;

      WriteLogMessage(
        captureSucceeded
        ? $"  capture succeeded"
        : $"  capture failed"
      ) ;

      // Check if the pointer exists in our dictionary,
      // ie, did the 'Enter' occur prior to the 'Press'.

      DeclarePointerContactStarted(pointer) ;

      AddPointerInfoTextToCanvas(pointerPoint) ;

    }

    private void Target_PointerEntered ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      Pointer      pointer      = pointerEventArgs.Pointer ;
      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;
      Point        position     = pointerPoint.Position ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} entered at [{position.X:F3},{position.Y:F3}]"
      ) ;

      // Check if pointer already exists (if enter occurred prior to down).

      // if ( ! m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      // {
      //   // Add contact to dictionary.
      //   m_activeContactsDictionary[pointerPoint.PointerId] = pointerEventArgs.Pointer ;
      // }

      if ( m_activeContactsDictionary.Count == 0 )
      {
        // Change the background color when pointer contact is detected.
        //m_target.Fill = new SolidColorBrush(Windows.UI.Colors.Blue) ;
      }

      AddPointerInfoTextToCanvas(pointerPoint) ;
    }

    //
    // The docs say that ...
    //   Occurs when a pointer moves while the pointer remains within the hit test area of this element.
    //
    // https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.uielement.pointermoved?view=winrt-19041
    //
    // In practice :
    //   WHEN A MOUSE BUTTON IS DOWN WHILE THE POINTER IS OVER THE TARGET,
    //   THIS EVENT FIRES REPEATEDLY (ABOUT TWICE PER SECOND) EVEN WHEN THE MOUSE HASN'T MOVED.
    //
    //   BUT IT DOESN'T FIRE REPEATEDLY, EVEN IF THE POINTER HAS BEEN 'CAPTURED',
    //   WHEN THE MOUSE IS MOVED AWAY FROM THE TARGET REGION.
    // 
    // Aha ... explanation !!
    //
    //   Logging the exact positions reported by these 'move' events, we see that even though we've
    //   made best efforts to hold the mouse stationary, the reported position can actually be changing
    //   by a tiny amount, typically a fraction of a pixel.
    //
    //   However we do sometimes get multiple Moves reported even when the delta is absolutely zero.
    //
    //   And if the mouse has moved out of the 'target' region, we get an event that reports a coordinate
    //   that is outside the region.
    //

    private Point? m_mostRecentlyReportedMousePosition = null ;

    private void Target_PointerMoved ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      Pointer      pointer      = pointerEventArgs.Pointer ;
      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;
      Point        position     = pointerPoint.Position ;

      IList<PointerPoint> intermediatePoints = pointerEventArgs.GetIntermediatePoints(m_target) ;
      IEnumerable<Point> intermediatePositions = intermediatePoints.Select(
        pointerPoint => pointerPoint.Position
      ) ;

      //
      // Multiple, simultaneous mouse button inputs are processed here.
      //
      // Mouse input is associated with a single pointer
      // that is assigned when mouse input is first detected.
      //
      // Clicking additional mouse buttons (left, wheel, or right) during
      // the interaction creates secondary associations between those buttons
      // and the pointer through the 'pointer pressed' event.
      //
      // The 'pointer released' event is fired only when the last mouse button
      // associated with the interaction (not necessarily the initial button)
      // is released.
      //
      // Because of this exclusive association, other mouse button clicks
      // are routed through the 'pointer move' event.
      //

      string movedMessage = "" ;
      if ( pointerPoint.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse )
      {
        if ( pointerPoint.Properties.IsLeftButtonPressed )
        {
          movedMessage += "left " ;
          // AddLineToEventLogPanel($"#{pointerPoint.PointerId} moved with left button pressed") ;
        }
        if ( pointerPoint.Properties.IsMiddleButtonPressed )
        {
          movedMessage += "wheel " ;
          // AddLineToEventLogPanel($"#{pointerPoint.PointerId} moved with wheel button pressed") ;
        }
        if ( pointerPoint.Properties.IsRightButtonPressed )
        {
          movedMessage += "right " ;
          // AddLineToEventLogPanel($"#{pointerPoint.PointerId} moved with right button pressed") ;
        }
      }

      double? DeltaFromLastKnownPosition ( )
      {
        if ( m_mostRecentlyReportedMousePosition is null )
        {
          return null ;
        }
        var dx = position.X - m_mostRecentlyReportedMousePosition.Value.X ;
        var dy = position.Y - m_mostRecentlyReportedMousePosition.Value.Y ;
        return System.Math.Sqrt(
          dx * dx
        + dy * dy
        ) ;
      }
      var deltaFromLastKnownPosition = DeltaFromLastKnownPosition() ;
      string delta = (
        deltaFromLastKnownPosition.HasValue
        ? $" delta {deltaFromLastKnownPosition:F6}"
        : ""
      ) ;
      WriteLogMessage($"#{pointerPoint.PointerId} moved to [{pointerPoint.Position.X:F3},{pointerPoint.Position.Y:F3}]{delta}") ;
      if ( movedMessage.Length > 0 )
      {
        WriteLogMessage($"  with {movedMessage}pressed") ;
      }
      if ( intermediatePositions.Any() )
      {
        //
        // Hmm, we *always* get an 'intermediate position' reported.
        // The FIRST item in the sequence seems to be the most recently reported position.
        // In the case of a Mouse move, the first and only item has coordinates that are
        // exactly the same as reported by 'GetCurrentPoint'.
        // For a rapidly executed 'swipe' on a touch screen, a number of points are reported.
        // THE DOCUMENTATION INCORRECTLY STATES -
        //   The last item in the collection is equivalent to the PointerPoint object returned by GetCurrentPoint.
        // IN FACT IT'S THE *FIRST* ITEM THAT REPRESENTS THE LATEST POSITION.
        // https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.input.pointerroutedeventargs.getintermediatepoints
        // The actual behaviour makes sense, the documentation has just got it back to front.
        //
        intermediatePositions.ForEachItem(
          intermediatePosition => WriteLogMessage(
            $"  with intermediate position [{intermediatePosition.X:F3},{intermediatePosition.Y:F3}]"
          )
        ) ;
      }

      m_mostRecentlyReportedMousePosition = position ;

      UpdatePointerInfoTextOnCanvas(pointerPoint) ;
    }

    private void Target_PointerWheelChanged ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      Pointer      pointer      = pointerEventArgs.Pointer ;
      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;
      Point        position     = pointerPoint.Position ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} mouse wheel, delta = {pointerPoint.Properties.MouseWheelDelta} at [{position.X:F3},{position.Y:F3}]"
      ) ;

      // Check if pointer already exists (for example, enter occurred prior to wheel).

      if ( ! m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        m_activeContactsDictionary[pointerPoint.PointerId] = pointerEventArgs.Pointer ;
      }

      AddPointerInfoTextToCanvas(pointerPoint) ;
    }

    //
    // PointerReleased fires when a button ceases to be pressed.
    //
    // PointerPressed and PointerReleased don't always occur in pairs.
    //
    // Your app should listen for and handle
    // any event that can 'conclude' a 'pointer down' :
    // - PointerExited
    // - PointerCanceled
    // - PointerCaptureLost
    //

    private void Target_PointerReleased ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      Pointer      pointer      = pointerEventArgs.Pointer ;
      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;
      Point        position     = pointerPoint.Position ;

      this.WriteLogMessage(
        $"#{pointerPoint.PointerId} released at [{position.X:F3},{position.Y:F3}]"
      ) ;

      //
      // If the event source is a mouse or a touchpad, and the pointer
      // is still over the target, we retain pointer and pointer details,
      // and return without removing the pointer from the dictionary.
      //
      // For this example, we assume a maximum of one mouse pointer.
      //

      //
      // HOW ARE DETECTING WHETHER THE POINTER IS STILL OVER THE TARGET ???
      //

      if ( pointerPoint.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse )
      {
        //m_target.Fill = new SolidColorBrush(Windows.UI.Colors.Blue) ;
        //WriteLogMessage($"  NOT RELEASED (it's a mouse event)") ;
      }
      else
      {
        //m_target.Fill = new SolidColorBrush(Windows.UI.Colors.Red) ;

        RemovePointerInfoTextFromCanvas(pointerPoint) ;

        if ( m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
        {
          m_activeContactsDictionary[pointerPoint.PointerId] = null ;
          m_activeContactsDictionary.Remove(pointerPoint.PointerId) ;
        }

        // HMM, THIS SEEMS TO BE UNNECESSARY : WITH A PEN OR A TOUCH SCREEN
        // WE DO GET THE EXPECTED 'CAPTURE-LOST' EVENT EVEN WITHOUT CALLING 'ReleasePointerCapture'
        // https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.uielement.releasepointercapture

        ///////////// m_target.ReleasePointerCapture(
        /////////////   pointerEventArgs.Pointer
        ///////////// ) ;
        ///////////// 
        ///////////// WriteLogMessage($"  NOT A MOUSE SO WE'VE CALLED 'ReleasePointerCapture'") ;
      }
    }

    //
    // The 'pointer capture lost' event handler
    // fires for various reasons, including:
    //
    // - User interactions
    // - Programmatic capture of another pointer
    // - Captured pointer was deliberately released
    //
    // PointerCaptureLost can fire instead of PointerReleased.
    //

    private void Target_PointerCaptureLost ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      Pointer      pointer      = pointerEventArgs.Pointer ;
      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;
      Point        position     = pointerPoint.Position ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} capture-lost at [{position.X:F3},{position.Y:F3}]"
      ) ;

      if ( m_activeContactsDictionary.Count == 0 )
      {
        //m_target.Fill = new SolidColorBrush(Windows.UI.Colors.Black) ;
      }

      // Remove contact from dictionary.

      if ( m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        m_activeContactsDictionary[pointerPoint.PointerId] = null ;
        m_activeContactsDictionary.Remove(pointerPoint.PointerId) ;
      }

      RemovePointerInfoTextFromCanvas(pointerPoint) ;
    }

    //
    // The 'pointer canceled' event handler
    // fires for for various reasons, including:
    // - Touch contact canceled by pen coming into range of the surface
    // - The device doesn't report an active contact for more than 100ms
    // - The desktop is locked or the user logged off
    // - The number of simultaneous contacts exceeded the number supported by the device
    //

    //
    // HMM, HAVE NEVER SEEN THIS !!!
    //

    private void Target_PointerCanceled ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      Pointer      pointer      = pointerEventArgs.Pointer ;
      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;
      Point        position     = pointerPoint.Position ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} canceled at [{position.X:F3},{position.Y:F3}]"
      ) ;

      if ( m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        m_activeContactsDictionary[pointerPoint.PointerId] = null ;
        m_activeContactsDictionary.Remove(pointerPoint.PointerId) ;
      }

      if ( m_activeContactsDictionary.Count == 0 )
      {
        // Hmm, this is never called ...
        //m_target.Fill = new SolidColorBrush(Windows.UI.Colors.Black) ;
      }

      RemovePointerInfoTextFromCanvas(pointerPoint) ;
    }

    private void Target_PointerExited ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      Pointer      pointer      = pointerEventArgs.Pointer ;
      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;
      Point        position     = pointerPoint.Position ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} exited at [{position.X:F3},{position.Y:F3}]"
      ) ;

      // Remove contact from dictionary.

      if ( m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        m_activeContactsDictionary[pointerPoint.PointerId] = null ;
        m_activeContactsDictionary.Remove(pointerPoint.PointerId) ;
      }

      if ( m_activeContactsDictionary.Count == 0 )
      {
        //m_target.Fill = new SolidColorBrush(Windows.UI.Colors.Red) ;
      }

      RemovePointerInfoTextFromCanvas(pointerPoint) ;
    }

    private string GetPointerInfoToDisplay ( PointerPoint pointerPoint )
    {
      string pointerInfo = "" ;

      switch ( pointerPoint.PointerDevice.PointerDeviceType )
      {
      case Windows.Devices.Input.PointerDeviceType.Mouse:
        pointerInfo += $"Pointer type : MOUSE (#{pointerPoint.PointerId})" ;
        break ;
      case Windows.Devices.Input.PointerDeviceType.Pen:
        pointerInfo += $"Pointer type : PEN" ;
        if ( pointerPoint.IsInContact )
        {
          pointerInfo += $"\n  Pressure              : {pointerPoint.Properties.Pressure}" ;
          pointerInfo += $"\n  Rotation              : {pointerPoint.Properties.Orientation}" ;
          pointerInfo += $"\n  Tilt X                : {pointerPoint.Properties.XTilt}" ;
          pointerInfo += $"\n  Tilt Y                : {pointerPoint.Properties.YTilt}" ;
          pointerInfo += $"\n  Barrel button pressed : {pointerPoint.Properties.IsBarrelButtonPressed}" ;
        }
        break ;
      case Windows.Devices.Input.PointerDeviceType.Touch:
        pointerInfo += $"Pointer type : TOUCH (#{pointerPoint.PointerId})" ;
        pointerInfo += $"\n  Rotation              : {pointerPoint.Properties.Orientation}" ;
        pointerInfo += $"\n  Tilt X                : {pointerPoint.Properties.XTilt}" ;
        pointerInfo += $"\n  Tilt Y                : {pointerPoint.Properties.YTilt}" ;
        break ;
      default:
        pointerInfo += "Pointer type : OTHER" ;
        break ;
      }

      GeneralTransform transform_toCanvasCoordinates = m_target.TransformToVisual(this) ;
      Point pointOnCanvas = transform_toCanvasCoordinates.TransformPoint(
        new Point(
          pointerPoint.Position.X,
          pointerPoint.Position.Y
        )
      ) ;
      pointerInfo += (
        $"\nPointer Id                            : #{pointerPoint.PointerId}"
      + $"\nPointer location (relative to target) : [{pointerPoint.Position.X:F2},{pointerPoint.Position.Y:F2}]" 
      + $"\nPointer location (relative to canvas) : [{pointOnCanvas.X:F2},{pointOnCanvas.Y:F2}]" 
      ) ;

      return pointerInfo ;
    }

  }

}
