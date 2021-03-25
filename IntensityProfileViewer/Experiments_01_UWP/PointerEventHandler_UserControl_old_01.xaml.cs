﻿using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Input ;

using Common.ExtensionMethods ;

namespace Experiments_01_UWP
{

  public sealed partial class PointerEventHandler_UserControl_old_01 : UserControl
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

    private readonly Dictionary<uint,Windows.UI.Xaml.Input.Pointer> m_activeContactsDictionary = new() ;

    public PointerEventHandler_UserControl_old_01 ( )
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

    private void WriteLogMessage ( string textLine )
    {
      Common.DebugHelpers.WriteDebugLines(textLine) ;
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

      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} pressed"
      ) ;

      // Lock the pointer to the target.
      m_target.CapturePointer(pointerEventArgs.Pointer) ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} captured"
      ) ;

      // Check if the pointer exists in our dictionary,
      // ie, did the 'Enter' occur prior to the 'Press'.

      if ( ! m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        // Add contact to dictionary.
        m_activeContactsDictionary[pointerPoint.PointerId] = pointerEventArgs.Pointer ;
      }

      // Change the background color when pointer contact is detected.

      //m_target.Fill = new SolidColorBrush(Windows.UI.Colors.Green) ;

      // Display pointer details
      AddPointerInfoTextToCanvas(pointerPoint) ;

    }

    // We don't capture the pointer on this event ...

    private void Target_PointerEntered ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route from handling the same event again.
      pointerEventArgs.Handled = true ;

      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} entered"
      ) ;

      // Check if pointer already exists (if enter occurred prior to down).

      if ( ! m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        // Add contact to dictionary.
        m_activeContactsDictionary[pointerPoint.PointerId] = pointerEventArgs.Pointer ;
      }

      if ( m_activeContactsDictionary.Count == 0 )
      {
        // Change the background color when pointer contact is detected.
        //m_target.Fill = new SolidColorBrush(Windows.UI.Colors.Blue) ;
      }

      AddPointerInfoTextToCanvas(pointerPoint) ;
    }

    private void Target_PointerMoved ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;

      //
      // Multiple, simultaneous mouse button inputs are processed here.
      //
      // Mouse input is associated with a single pointer
      // assigned when mouse input is first detected.
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

      if ( movedMessage.Length > 0 )
      {
        WriteLogMessage($"#{pointerPoint.PointerId} moved with {movedMessage}pressed") ;
      }

      UpdatePointerInfoTextOnCanvas(pointerPoint) ;
    }

    private void Target_PointerWheelChanged ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} mouse wheel, delta = {pointerPoint.Properties.MouseWheelDelta}"
      ) ;

      // Check if pointer already exists (for example, enter occurred prior to wheel).

      if ( ! m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        m_activeContactsDictionary[pointerPoint.PointerId] = pointerEventArgs.Pointer ;
      }

      AddPointerInfoTextToCanvas(pointerPoint) ;
    }

    //
    // The pointer released event handler.
    //
    // PointerPressed and PointerReleased don't always occur in pairs.
    //
    // Your app should listen for and handle
    // any event that can conclude a pointer down :
    // - PointerExited
    // - PointerCanceled
    // - PointerCaptureLost
    //

    private void Target_PointerReleased ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;

      // AddLineToEventLogPanel(
      //   $"#{pointerPoint.PointerId} event ..."
      // ) ;

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
        WriteLogMessage($"#{pointerPoint.PointerId} release - NOT RELEASED (it's a mouse event)") ;
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

        m_target.ReleasePointerCapture(
          pointerEventArgs.Pointer
        ) ;

        WriteLogMessage($"#{pointerPoint.PointerId} release - RELEASED") ;
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

      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} capture lost: " + pointerPoint.PointerId
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

    private void Target_PointerCanceled ( object sender, PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} canceled"
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

    private void Target_PointerExited ( object sender,PointerRoutedEventArgs pointerEventArgs )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      pointerEventArgs.Handled = true ;

      PointerPoint pointerPoint = pointerEventArgs.GetCurrentPoint(m_target) ;

      WriteLogMessage(
        $"#{pointerPoint.PointerId} exited"
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
        pointerInfo += $"\nPointer type : MOUSE (#{pointerPoint.PointerId})" ;
        break ;
      case Windows.Devices.Input.PointerDeviceType.Pen:
        pointerInfo += $"\nPointer type : PEN" ;
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
        pointerInfo += $"\nPointer type : TOUCH (#{pointerPoint.PointerId})" ;
        pointerInfo += $"\n  Rotation              : {pointerPoint.Properties.Orientation}" ;
        pointerInfo += $"\n  Tilt X                : {pointerPoint.Properties.XTilt}" ;
        pointerInfo += $"\n  Tilt Y                : {pointerPoint.Properties.YTilt}" ;
        break ;
      default:
        pointerInfo += "\nPointer type : OTHER" ;
        break ;
      }

      GeneralTransform transform_toPageCoordinates = m_target.TransformToVisual(this) ;
      Point pointOnPage = transform_toPageCoordinates.TransformPoint(
        new Point(
          pointerPoint.Position.X,
          pointerPoint.Position.Y
        )
      ) ;
      pointerInfo += (
        $"\nPointer Id                            : #{pointerPoint.PointerId}"
      + $"\nPointer location (relative to target) : [{pointerPoint.Position.X:F2},{pointerPoint.Position.Y:F2}]" 
      + $"\nPointer location (relative to canvas) : [{pointOnPage.X:F2},{pointOnPage.Y:F2}]" 
      ) ;

      return pointerInfo ;
    }

  }

}