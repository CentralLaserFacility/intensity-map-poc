//
// PointerHandlingPage.xaml.cs
//

//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//  The MIT License (MIT)
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System ;
using System.Collections.Generic ;
using System.Linq ;
using Windows.Foundation ;
using Windows.UI.Input ;
using Windows.UI.Xaml ;
using Windows.UI.Xaml.Controls ;
using Windows.UI.Xaml.Documents ;
using Windows.UI.Xaml.Input ;
using Windows.UI.Xaml.Media ;

// For some devices, once the maximum number of contacts is reached,
// additional contacts might be ignored (PointerPressed not fired).

namespace IntensityProfileViewer
{

  public sealed partial class PointerHandlingPage : Page
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

    private Dictionary<uint,Windows.UI.Xaml.Input.Pointer> m_activeContactsDictionary = new() ;

    public PointerHandlingPage ( )
    {
      this.InitializeComponent() ;

      // Declare the pointer event handlers

      m_targetRectangle.PointerPressed      += new PointerEventHandler(Target_PointerPressed) ;
      m_targetRectangle.PointerEntered      += new PointerEventHandler(Target_PointerEntered) ;
      m_targetRectangle.PointerReleased     += new PointerEventHandler(Target_PointerReleased) ;
      m_targetRectangle.PointerExited       += new PointerEventHandler(Target_PointerExited) ;
      m_targetRectangle.PointerCanceled     += new PointerEventHandler(Target_PointerCanceled) ;
      m_targetRectangle.PointerCaptureLost  += new PointerEventHandler(Target_PointerCaptureLost) ;
      m_targetRectangle.PointerMoved        += new PointerEventHandler(Target_PointerMoved) ;
      m_targetRectangle.PointerWheelChanged += new PointerEventHandler(Target_PointerWheelChanged) ;

      buttonClear.Click += new RoutedEventHandler(ButtonClear_Click) ;
    }

    private void ButtonClear_Click ( object sender,RoutedEventArgs e )
    {
      m_eventLogPanel.Blocks.Clear() ;
    }

    private void AddLineToEventLogPanel ( string textLine )
    {
      Paragraph newParagraph = new() ;
      newParagraph.Inlines.Add(
        new Run() {
          Text = textLine
        }
      ) ;
      m_eventLogPanel.Blocks.Insert(
        0,
        newParagraph
      ) ;
    }

    private void AddPointerInfoTextToCanvas ( PointerPoint pointerPoint )
    {
      m_canvas.Children.Add(
        new TextBlock() {
          Name            = pointerPoint.PointerId.ToString(),
          // Tag             = pointerPoint.PointerId, // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
          Foreground      = new SolidColorBrush(Windows.UI.Colors.White),
          FontFamily      = new FontFamily("Consolas"),
          Text            = GetPointerInfoToDisplay(pointerPoint),
          RenderTransform = new TranslateTransform() {
            X = pointerPoint.Position.X + 10,
            Y = pointerPoint.Position.Y + 10
          }
        }
      ) ;
    }

    private void UpdatePointerInfoTextOnCanvas ( PointerPoint pointerPoint )
    {
      if ( true )
      {
        foreach ( var child in m_canvas.Children )
        {
          if ( child.GetType().ToString() == "Windows.UI.Xaml.Controls.TextBlock" )
          {
            TextBlock textBlock = (TextBlock) child ;
            if ( textBlock.Name == pointerPoint.PointerId.ToString() )
            {
              // To get pointer location details, we need extended pointer info.
              // We get the pointer info through the 'GetCurrentPoint' method
              // of the event argument.
              TranslateTransform x = new TranslateTransform() ;
              x.X = pointerPoint.Position.X + 20 ;
              x.Y = pointerPoint.Position.Y + 20 ;
              child.RenderTransform = x ;
              textBlock.Text = GetPointerInfoToDisplay(pointerPoint) ;
            }
          } 
        }
      }
      else
      {
        foreach ( var child in m_canvas.Children )
        {
          if ( child is TextBlock textBlock )
          {
            if ( textBlock.Name == pointerPoint.PointerId.ToString() )
            {
              // To get pointer location details, we need extended pointer info.
              // We get the pointer info through the 'GetCurrentPoint' method
              // of the 'event' argument.
              TranslateTransform x = new TranslateTransform() ;
              x.X = pointerPoint.Position.X + 20 ;
              x.Y = pointerPoint.Position.Y + 20 ;
              child.RenderTransform = x ;
              textBlock.Text = GetPointerInfoToDisplay(pointerPoint) ;
            }
          }
        }
      }
    }

    // Destroy the pointer info popup

    private void RemovePointerInfoTextFromCanvas ( PointerPoint pointerPoint )
    {
      foreach ( var pointerDetails in m_canvas.Children )
      {
        if ( pointerDetails.GetType().ToString() == "Windows.UI.Xaml.Controls.TextBlock")
        {
          TextBlock textBlock = (TextBlock) pointerDetails ;
          if ( textBlock.Name == pointerPoint.PointerId.ToString() )
          {
            m_canvas.Children.Remove(pointerDetails) ;
          }
        }
      }
    }

    //
    // PointerPressed and PointerReleased don't always occur in pairs.
    //
    // Your app should listen for and handle any event that can conclude
    // a pointer down (PointerExited, PointerCanceled, PointerCaptureLost).
    // 

    private void Target_PointerPressed ( object sender, PointerRoutedEventArgs e )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      e.Handled = true ;

      PointerPoint pointerPoint = e.GetCurrentPoint(m_targetRectangle) ;

      AddLineToEventLogPanel(
        "Down: " + pointerPoint.PointerId
      ) ;

      // Lock the pointer to the target.
      m_targetRectangle.CapturePointer(e.Pointer) ;

      AddLineToEventLogPanel(
        "Pointer captured: " + pointerPoint.PointerId
      ) ;

      // Check if the pointer exists in our dictionary,
      // ie, did the 'Enter' occur prior to the 'Press'.

      if ( ! m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        // Add contact to dictionary.
        m_activeContactsDictionary[pointerPoint.PointerId] = e.Pointer ;
      }

      // Change the background color when pointer contact is detected.

      m_targetRectangle.Fill = new SolidColorBrush(Windows.UI.Colors.Green) ;

      // Display pointer details
      AddPointerInfoTextToCanvas(pointerPoint) ;

    }

    // We do not capture the pointer on this event.

    private void Target_PointerEntered ( object sender, PointerRoutedEventArgs e )
    {
      // Prevent most handlers along the event route from handling the same event again.
      e.Handled = true ;

      PointerPoint pointerPoint = e.GetCurrentPoint(m_targetRectangle) ;

      AddLineToEventLogPanel(
        "Entered: " + pointerPoint.PointerId
      ) ;

      // Check if pointer already exists (if enter occurred prior to down).

      if ( ! m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        // Add contact to dictionary.
        m_activeContactsDictionary[pointerPoint.PointerId] = e.Pointer ;
      }

      if ( m_activeContactsDictionary.Count == 0 )
      {
        // Change the background color when pointer contact is detected.
        m_targetRectangle.Fill = new SolidColorBrush(Windows.UI.Colors.Blue) ;
      }

      // Display pointer details.
      AddPointerInfoTextToCanvas(pointerPoint) ;
    }

    private void Target_PointerMoved ( object sender, PointerRoutedEventArgs e )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      e.Handled = true ;

      PointerPoint pointerPoint = e.GetCurrentPoint(m_targetRectangle) ;

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

      if ( pointerPoint.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse )
      {
        if ( pointerPoint.Properties.IsLeftButtonPressed )
        {
          AddLineToEventLogPanel("Left button: " + pointerPoint.PointerId) ;
        }
        if ( pointerPoint.Properties.IsMiddleButtonPressed )
        {
          AddLineToEventLogPanel("Wheel button: " + pointerPoint.PointerId) ;
        }
        if ( pointerPoint.Properties.IsRightButtonPressed )
        {
          AddLineToEventLogPanel("Right button: " + pointerPoint.PointerId) ;
        }
      }

      UpdatePointerInfoTextOnCanvas(pointerPoint) ;
    }

    private void Target_PointerWheelChanged ( object sender,PointerRoutedEventArgs e )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      e.Handled = true ;

      PointerPoint pointerPoint = e.GetCurrentPoint(m_targetRectangle) ;

      AddLineToEventLogPanel(
        "Mouse wheel: " + pointerPoint.PointerId
      ) ;

      // Check if pointer already exists (for example, enter occurred prior to wheel).

      if ( ! m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        // Add contact to dictionary.
        m_activeContactsDictionary[pointerPoint.PointerId] = e.Pointer ;
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

    private void Target_PointerReleased ( object sender,PointerRoutedEventArgs e )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      e.Handled = true ;

      PointerPoint pointerPoint = e.GetCurrentPoint(m_targetRectangle) ;

      AddLineToEventLogPanel(
        "Up: " + pointerPoint.PointerId
      ) ;

      //
      // If event source is mouse or touchpad and the pointer is still
      // over the target, retain pointer and pointer details.
      //
      // Return without removing pointer from pointers dictionary.
      //
      // For this example, we assume a maximum of one mouse pointer.
      //

      if ( pointerPoint.PointerDevice.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse )
      {
        m_targetRectangle.Fill = new SolidColorBrush(Windows.UI.Colors.Red) ;

        RemovePointerInfoTextFromCanvas(pointerPoint) ;

        // Remove contact from dictionary.

        if ( m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
        {
          m_activeContactsDictionary[pointerPoint.PointerId] = null ;
          m_activeContactsDictionary.Remove(pointerPoint.PointerId) ;
        }

        // Release the pointer from the target.

        m_targetRectangle.ReleasePointerCapture(e.Pointer) ;

        AddLineToEventLogPanel("Pointer released: " + pointerPoint.PointerId) ;
      }
      else
      {
        m_targetRectangle.Fill = new SolidColorBrush(Windows.UI.Colors.Blue) ;
      }
    }

    //
    // The pointer capture lost event handler.
    //
    // Fires for various reasons, including:
    // - User interactions
    // - Programmatic capture of another pointer
    // - Captured pointer was deliberately released
    //
    // PointerCaptureLost can fire instead of PointerReleased.
    //

    private void Target_PointerCaptureLost ( object sender, PointerRoutedEventArgs e )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      e.Handled = true ;

      PointerPoint pointerPoint = e.GetCurrentPoint(m_targetRectangle) ;

      AddLineToEventLogPanel(
        "Pointer capture lost: " + pointerPoint.PointerId
      ) ;

      if ( m_activeContactsDictionary.Count == 0 )
      {
        m_targetRectangle.Fill = new SolidColorBrush(Windows.UI.Colors.Black) ;
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
    // The pointer canceled event handler
    // fires for for various reasons, including:
    // - Touch contact canceled by pen coming into range of the surface
    // - The device doesn't report an active contact for more than 100ms
    // - The desktop is locked or the user logged off
    // - The number of simultaneous contacts exceeded the number supported by the device
    //

    private void Target_PointerCanceled ( object sender, PointerRoutedEventArgs e )
    {
      // Prevent most handlers along the event route from handling the same event again.
      e.Handled = true ;

      PointerPoint pointerPoint = e.GetCurrentPoint(m_targetRectangle) ;

      AddLineToEventLogPanel(
        "Pointer canceled: " + pointerPoint.PointerId
      ) ;

      // Remove contact from dictionary.

      if ( m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        m_activeContactsDictionary[pointerPoint.PointerId] = null ;
        m_activeContactsDictionary.Remove(pointerPoint.PointerId) ;
      }

      if ( m_activeContactsDictionary.Count == 0 )
      {
        m_targetRectangle.Fill = new SolidColorBrush(Windows.UI.Colors.Black) ;
      }

      RemovePointerInfoTextFromCanvas(pointerPoint) ;
    }

    private void Target_PointerExited ( object sender,PointerRoutedEventArgs e )
    {
      // Prevent most handlers along the event route
      // from handling the same event again.
      e.Handled = true ;

      PointerPoint pointerPoint = e.GetCurrentPoint(m_targetRectangle) ;

      AddLineToEventLogPanel(
        "Pointer exited: " + pointerPoint.PointerId
      ) ;

      // Remove contact from dictionary.

      if ( m_activeContactsDictionary.ContainsKey(pointerPoint.PointerId) )
      {
        m_activeContactsDictionary[pointerPoint.PointerId] = null ;
        m_activeContactsDictionary.Remove(pointerPoint.PointerId) ;
      }

      if ( m_activeContactsDictionary.Count == 0 )
      {
        m_targetRectangle.Fill = new SolidColorBrush(Windows.UI.Colors.Red) ;
      }

      RemovePointerInfoTextFromCanvas(pointerPoint) ;
    }

    private string GetPointerInfoToDisplay_NEW ( PointerPoint pointerPoint )
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

      GeneralTransform transform_toPageCoordinates = m_targetRectangle.TransformToVisual(this) ;
      Point pointOnPage = transform_toPageCoordinates.TransformPoint(
        new Point(
          pointerPoint.Position.X,
          pointerPoint.Position.Y
        )
      ) ;
      pointerInfo += (
        $"\nPointer Id                            : {pointerPoint.PointerId}"
      + $"\nPointer location (relative to target) : [{pointerPoint.Position.X:F2},{pointerPoint.Position.Y:F2}]" 
      + $"\nPointer location (relative to canvas) : [{pointOnPage.X:F2},{pointOnPage.Y:F2}]" 
      ) ;

      return pointerInfo ;
    }

    private string GetPointerInfoToDisplay ( PointerPoint pointerPoint )
    {
      String details = "" ;

      switch ( pointerPoint.PointerDevice.PointerDeviceType )
      {
      case Windows.Devices.Input.PointerDeviceType.Mouse:
        details += "\nPointer type : MOUSE" ;
        break ;
      case Windows.Devices.Input.PointerDeviceType.Pen:
        details += "\nPointer type : PEN" ;
        if ( pointerPoint.IsInContact )
        {
          details += $"\n  Pressure              : {pointerPoint.Properties.Pressure}" ;
          details += $"\n  Rotation              : {pointerPoint.Properties.Orientation}" ;
          details += $"\n  Tilt X                : {pointerPoint.Properties.XTilt}" ;
          details += $"\n  Tilt Y                : {pointerPoint.Properties.YTilt}" ;
          details += $"\n  Barrel button pressed : {pointerPoint.Properties.IsBarrelButtonPressed}" ;
        }
        break ;
      case Windows.Devices.Input.PointerDeviceType.Touch:
        details += "\nPointer type : TOUCH" ;
        details += "\n  Rotation : " + pointerPoint.Properties.Orientation ;
        details += "\n  Tilt X   : " + pointerPoint.Properties.XTilt ;
        details += "\n  Tilt Y   : " + pointerPoint.Properties.YTilt ;
        break ;
      default:
        details += "\nPointer type: n/a" ;
        break ;
      }

      GeneralTransform transform_toScreenCoordinates = m_targetRectangle.TransformToVisual(this) ;
      Point screenPoint = transform_toScreenCoordinates.TransformPoint(
        new Point(
          pointerPoint.Position.X,
          pointerPoint.Position.Y
        )
      ) ;
      details += (
        $"\nPointer Id                   : {pointerPoint.PointerId}"
      + $"\nPointer location (target)    : [{pointerPoint.Position.X:F2},{pointerPoint.Position.Y:F2}]" 
      + $"\nPointer location (container) : [{screenPoint.X:F2},{screenPoint.Y:F2}]" 
      ) ;

      return details ;
    }
  }

}
