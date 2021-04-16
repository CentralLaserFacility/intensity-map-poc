//
// PanAndZoomAndRotationGesturesHandler.cs
//

using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace UwpSkiaUtilities
{

  public class PanAndZoomGesturesHandler_UWP
  {

        // https://github.com/OndrejKunc/SkiaScene/blob/master/source/TouchTracking/TouchTracking.UWP/TouchHandler.cs

        // Hmm, that code doesn't hande all the events it should, eg CaptureLost ...

        // Aha ???
        // https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BasicInput/cs/5-GestureRecognizer.xaml.cs

#if NETFX_CORE
        private TouchTracking.UWP.TouchHandler m_touchHandler = new() ;
#elif __ANDROID__
        private TouchTracking.Droid.TouchHandler m_touchHandler = new();
#endif


      public System.Func<
      TouchTracking.TouchActionType, 
      Point,             // positionInSceneCoordinates
      bool,                          // inContact
      bool                           // return true if 'handled'
    > TouchActionDetected ;

    private Canvas m_canvas ;

    public PanAndZoomGesturesHandler_UWP ( 
      Canvas canvas
    ) {
      m_canvas = canvas ;
      #if NETFX_CORE || __ANDROID__
        m_touchHandler.RegisterEvents(canvas) ;
        m_touchHandler.TouchAction += HandleTouchEvent ;
      #endif
      OnWindowSizeChanged() ;
    }

    public void ResetPanAndZoom ( )
    {
    }

    private void OnPointerMoved ( object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e )
    {
      Windows.UI.Input.PointerPoint pointerPoint = e.GetCurrentPoint(m_canvas) ;
      // Common.DebugHelpers.WriteDebugLines(
      //   $"OnPointerMoved : physical [{pointerPoint.Position.X},{pointerPoint.Position.Y}] ; scene [{positionInSceneCoordinates.X},{positionInSceneCoordinates.Y}]"
      // ) ;
    }
    // TODO : SetSceneCentreFromCanvas

    public void OnWindowSizeChanged ( )
    {
        
    }

    private void HandleTouchEvent ( object sender, TouchTracking.TouchActionEventArgs args )
    {
      Common.DebugHelpers.WriteDebugLines(
        $"HandleTouchEvent : {args.Type} ; "
      + $"Point on canvas [{args.Location.X},{args.Location.Y}] ; "
      + $"InContact={args.IsInContact}"
      ) ;
      // Invoked when our 'TouchHandler' detects a touch event.
      // var viewPoint = args.Location ;
      // SkiaSharp.SKPoint pointOnCanvas = new SkiaSharp.SKPoint(
      //   (float) ( m_skiaXamlCanvas.CanvasSize.Width  * viewPoint.X / m_skiaXamlCanvas.ActualWidth ),
      //   (float) ( m_skiaXamlCanvas.CanvasSize.Height * viewPoint.Y / m_skiaXamlCanvas.ActualHeight )
      // ) ;
      // SkiaSharp.SKPoint positionInSceneCoordinates = m_scene.GetCanvasPointFromViewPoint(
      //   pointOnCanvas
      // ) ;
      // bool handled = TouchActionDetected.Invoke(
      //   args.Type,
      //   positionInSceneCoordinates,
      //   args.IsInContact
      // ) ;
      // if ( ! handled )
      // {
      //   m_touchGestureRecognizer.ProcessTouchEvent(
      //     id   : args.Id,
      //     type : args.Type, // Action Type
      //     pointOnCanvas
      //   ) ;
      // }
    }

    private enum HowToZoom {
      ZoomFromCentre,
      ZoomFromCurrentMousePosition
    } 

    private float m_aggregatedZoomFactor = 1.0f ;

    public static bool RecogniseRotationGestures = false ;

    private void OnPointerWheelChanged ( object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e )
    {
      const float zoomFactorPerScrollWheelClick = 1.1f ;
      Windows.UI.Input.PointerPoint pointerPoint = e.GetCurrentPoint(m_canvas) ;
      int wheelDelta = pointerPoint.Properties.MouseWheelDelta ;

      // If CONTROL is down, we ROTATE ...
      // https://blog.mzikmund.com/2017/06/the-right-way-to-check-for-key-state-in-uwp-apps/
      if ( 
         RecogniseRotationGestures 
      && Windows.UI.Core.CoreWindow.GetForCurrentThread(
         ).GetAsyncKeyState(
           Windows.System.VirtualKey.Control
         ).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down)
      ) {
        // var rotationReferencePoint = m_scene.GetCanvasPointFromViewPoint(
        //   new SkiaSharp.SKPoint(
        //     (float) pointerPoint.Position.X,
        //     (float) pointerPoint.Position.Y
        //   )
        // ) ;
        // m_scene.RotateByRadiansDelta(
        //   rotationReferencePoint,
        //   (float) ( 
        //     // Each click of the wheel in a 'forward' direction,
        //     // ie with the finger that's resting on the mouse wheel
        //     // moving 'forward' towards the end of the mouse body,
        //     // gives a delta of +120.
        //     // We want one click to rotate us by 10 degrees clockwise.
        //     ( wheelDelta / 120 ) 
        //   * 10.0 
        //   * System.Math.PI / 180.0 
        //   )
        // ) ;
        // m_skiaXamlCanvas.Invalidate() ;
        return ;
      }
      // Otherwise, we ZOOM ...
      float zoomFactorToApply = (
        wheelDelta > 0 
        ? zoomFactorPerScrollWheelClick 
        : 1 / zoomFactorPerScrollWheelClick 
      ) ;
      var howToZoom = (
        // pointerPoint.Properties.IsLeftButtonPressed
        Windows.UI.Core.CoreWindow.GetForCurrentThread(
        ).GetAsyncKeyState(
          Windows.System.VirtualKey.Shift
        ) == Windows.UI.Core.CoreVirtualKeyStates.None
        ? HowToZoom.ZoomFromCurrentMousePosition
        : HowToZoom.ZoomFromCentre
      ) ;
      // SkiaSharp.SKPoint zoomReferencePoint = (
      //   howToZoom == HowToZoom.ZoomFromCentre
      //   ? m_scene.GetCenter()
      //   : m_scene.GetCanvasPointFromViewPoint(
      //       new SkiaSharp.SKPoint(
      //         (float) pointerPoint.Position.X,
      //         (float) pointerPoint.Position.Y
      //       )
      //     )
      // ) ;
      // m_scene.ZoomByScaleFactor(
      //   zoomReferencePoint,
      //   zoomFactorToApply
      // ) ;
      // m_aggregatedZoomFactor *= zoomFactorToApply ;
      // m_skiaXamlCanvas.Invalidate() ;
    }

  }

}
