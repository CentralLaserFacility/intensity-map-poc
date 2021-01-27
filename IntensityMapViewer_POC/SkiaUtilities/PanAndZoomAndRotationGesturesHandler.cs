//
// PanAndZoomAndRotationGesturesHandler.cs
//

namespace UwpSkiaUtilities
{

    public class PanAndZoomAndRotationGesturesHandler
    {

        private SkiaScene.ISKScene m_scene;

        private SkiaScene.TouchManipulation.ITouchGestureRecognizer m_touchGestureRecognizer;

        private SkiaScene.TouchManipulation.ISceneGestureResponder m_sceneGestureResponder;

#if __ANDROID__
        private TouchTracking.Droid.TouchHandler m_touchHandler = new();
#elif NETFX_CORE
        private TouchTracking.UWP.TouchHandler m_touchHandler = new();
#endif

        private SkiaSharp.Views.UWP.SKXamlCanvas m_canvas;

        public PanAndZoomAndRotationGesturesHandler(
          SkiaSharp.Views.UWP.SKXamlCanvas canvas,
          SkiaScene.ISKSceneRenderer sceneRenderer
        ) :
        this(
          canvas,
          new SkiaScene.SKScene(
            sceneRenderer
          )
          {
              // The defaults are fine ...
              // MinScale = ...
              // MaxScale = ...
          }
        )
        {
        }

        public PanAndZoomAndRotationGesturesHandler(
          SkiaSharp.Views.UWP.SKXamlCanvas canvas,
          SkiaScene.ISKScene scene
        )
        {
            
            m_canvas = canvas;
            m_canvas.PaintSurface += OnPaintSurface;
            m_canvas.PointerMoved += OnPointerMoved;
            // m_canvas.
            m_canvas.PointerWheelChanged += OnPointerWheelChanged;
            m_scene = scene;

#if __ANDROID__ || NETFX_CORE
            m_touchHandler.RegisterEvents(m_canvas);
            m_touchHandler.TouchAction += OnTouch;
#endif
            ////////////////////
            OnWindowSizeChanged();
            /////////////////////
            m_touchGestureRecognizer = new SkiaScene.TouchManipulation.TouchGestureRecognizer();
            m_touchGestureRecognizer.OnPan += (s, e) =>
            {
                // m_renderer.DebugInfoToDraw_01 = e.TouchActionType.ToString() ;
                // m_renderer.DebugInfoToDraw_02 = "" ;
                if (
                   e.TouchActionType == TouchTracking.TouchActionType.Moved
                || e.TouchActionType == TouchTracking.TouchActionType.Released
                )
                {
                    // var matrix = ((SkiaSceneEx) m_scene).CurrentTransformMatrix ;
                    // m_renderer.DebugInfoToDraw_02 = $"Matrix Trans = ({matrix.TransX},{matrix.TransY})" ;
                }
            };
            m_sceneGestureResponder = new SkiaScene.TouchManipulation.SceneGestureRenderingResponder(
              () => m_canvas.Invalidate(),
              m_scene,
              m_touchGestureRecognizer
            )
            {
                TouchManipulationMode = SkiaScene.TouchManipulation.TouchManipulationMode.IsotropicScale,
                MaxFramesPerSecond = 30,
            };
            m_sceneGestureResponder.StartResponding();
        }

        private void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //
        }

        public void OnWindowSizeChanged()
        {
            m_scene.ScreenCenter = new SkiaSharp.SKPoint(
              m_canvas.CanvasSize.Width / 2,
              m_canvas.CanvasSize.Height / 2
            );
        }

        private void OnPaintSurface(object sender, SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs args)
        {
            SkiaSharp.SKImageInfo imageInfo = args.Info;
            SkiaSharp.SKSurface surface = args.Surface;
            SkiaSharp.SKCanvas canvas = surface.Canvas;
            m_scene.Render(canvas);
        }

        private void OnTouch(object sender, TouchTracking.TouchActionEventArgs args)
        {
            var viewPoint = args.Location;
            SkiaSharp.SKPoint point = new SkiaSharp.SKPoint(
              (float)(m_canvas.CanvasSize.Width * viewPoint.X / m_canvas.ActualWidth),
              (float)(m_canvas.CanvasSize.Height * viewPoint.Y / m_canvas.ActualHeight)
            );
            m_touchGestureRecognizer.ProcessTouchEvent(
              id: args.Id,
              type: args.Type, // Action Type
              point
            );
        }

        private enum HowToZoom
        {
            ZoomFromCentre,
            ZoomFromCurrentMousePosition
        }

        private float m_aggregatedZoomFactor = 1.0f;

        private void OnPointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            const float zoomFactorPerScrollWheelClick = 1.1f;
            Windows.UI.Input.PointerPoint pointerPoint = e.GetCurrentPoint(m_canvas);
            int wheelDelta = pointerPoint.Properties.MouseWheelDelta;

            // If CONTROL is down, we ROTATE ...
            if (
              Windows.UI.Core.CoreWindow.GetForCurrentThread(
              ).GetAsyncKeyState(
                Windows.System.VirtualKey.Control
              ) == Windows.UI.Core.CoreVirtualKeyStates.Down
            )
            {
                var rotationReferencePoint = m_scene.GetCanvasPointFromViewPoint(
                  new SkiaSharp.SKPoint(
                    (float)pointerPoint.Position.X,
                    (float)pointerPoint.Position.Y
                  )
                );
                m_scene.RotateByRadiansDelta(
                  rotationReferencePoint,
                  (float)(
                    // Each click gives a delta of 120
                    // We want one click to rotate us by 10 degrees
                    (wheelDelta / 120)
                  * 10.0
                  * System.Math.PI / 180.0
                  )
                );
                // var matrix2 = ((SkiaSceneEx) m_scene).CurrentTransformMatrix ;
                // m_renderer.DebugInfoToDraw_01 = $"Matrix Scale = ({matrix2.ScaleX},{matrix2.ScaleY})" ;
                // m_renderer.DebugInfoToDraw_02 = $"Matrix Trans = ({matrix2.TransX},{matrix2.TransY})" ;
                m_canvas.Invalidate();
                return;
            }
            // Otherwise, we ZOOM ...
            float zoomFactorToApply = (
              wheelDelta > 0
              ? zoomFactorPerScrollWheelClick
              : 1 / zoomFactorPerScrollWheelClick
            );
            var howToZoom = (
              // pointerPoint.Properties.IsLeftButtonPressed
              Windows.UI.Core.CoreWindow.GetForCurrentThread(
              ).GetAsyncKeyState(
                Windows.System.VirtualKey.Shift
              ) == Windows.UI.Core.CoreVirtualKeyStates.None
              ? HowToZoom.ZoomFromCurrentMousePosition
              : HowToZoom.ZoomFromCentre
            );
            SkiaSharp.SKPoint zoomReferencePoint = (
              howToZoom == HowToZoom.ZoomFromCentre
              ? m_scene.GetCenter()
              : m_scene.GetCanvasPointFromViewPoint(
                  new SkiaSharp.SKPoint(
                    (float)pointerPoint.Position.X,
                    (float)pointerPoint.Position.Y
                  )
                )
            );
            m_scene.ZoomByScaleFactor(
              zoomReferencePoint,
              zoomFactorToApply
            );
            // var matrix = ((SkiaSceneEx) m_scene).CurrentTransformMatrix ;
            m_aggregatedZoomFactor *= zoomFactorToApply;
            // m_renderer.DebugInfoToDraw_01 = $"Matrix Scale = ({matrix.ScaleX},{matrix.ScaleY})" ;
            // m_renderer.DebugInfoToDraw_02 = $"Matrix Trans = ({matrix.TransX},{matrix.TransY})" ;
            // m_renderer.DebugInfoToDraw_03 = $"Aggregated zoom factor is {m_aggregatedZoomFactor:F4}" ;
            m_canvas.Invalidate();
        }

    }

}
