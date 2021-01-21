using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace UkCentralLaserPoC.Shared.Controls
{

    public sealed partial class SkiaView : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }
            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(
          this,
          new PropertyChangedEventArgs(propertyName)
        );

        private UwpSkiaUtilities.PanAndZoomAndRotationGesturesHandler m_panAndZoomAndRotationGesturesHandler_00;
        private UwpSkiaUtilities.PanAndZoomAndRotationGesturesHandler m_panAndZoomAndRotationGesturesHandler_01;
        private UwpSkiaUtilities.PanAndZoomAndRotationGesturesHandler m_panAndZoomAndRotationGesturesHandler_10;
        private UwpSkiaUtilities.PanAndZoomAndRotationGesturesHandler m_panAndZoomAndRotationGesturesHandler_11;

        public SkiaView()
        {
            InitializeComponent();
            SkiaSharp.SKMatrix matrix = new();

            this.Loaded += (obj, args) =>
            {
                Windows.UI.Xaml.Window.Current.SizeChanged += OnWindowSizeChanged;
            };

            this.Unloaded += (obj, args) =>
            {
                Windows.UI.Xaml.Window.Current.SizeChanged -= OnWindowSizeChanged;
            };


            m_panAndZoomAndRotationGesturesHandler_00 = new(
              m_canvasView_00,
              new SkiaSceneRenderer_ForTesting_01()
              {
                  RenderHook = (canvas) =>
                  {
                      matrix = canvas.TotalMatrix;
                      m_canvasView_01.Invalidate();
                      m_canvasView_10.Invalidate();
                      m_canvasView_11.Invalidate();
                  }
              }
            );

            m_panAndZoomAndRotationGesturesHandler_01 = new(
              m_canvasView_01,
              new SkiaSceneRenderer_ForTesting_01()
              {
                  RenderHook = (canvas) =>
                  {
                      canvas.SetMatrix(matrix);
                  }
              }
            );

            m_panAndZoomAndRotationGesturesHandler_10 = new(
              m_canvasView_10,
              new SkiaSceneRenderer_ForTesting_01()
              {
                  RenderHook = (canvas) =>
                  {
                      var effectiveMatrix = matrix;
                      effectiveMatrix.ScaleY = 1.0f;
                      effectiveMatrix.TransY = 0.0f;
                      canvas.SetMatrix(effectiveMatrix);
                  }
              }
            );

            m_panAndZoomAndRotationGesturesHandler_11 = new(
              m_canvasView_11,
              new SkiaSceneRenderer_ForTesting_01()
              {
                  RenderHook = (canvas) =>
                  {
                      var effectiveMatrix = matrix;
                      effectiveMatrix.ScaleX = 1.0f;
                      effectiveMatrix.TransX = 0.0f;
                      canvas.SetMatrix(effectiveMatrix);
                  }
              }
            );

        }
        private void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            m_panAndZoomAndRotationGesturesHandler_00.OnWindowSizeChanged();
            m_panAndZoomAndRotationGesturesHandler_01.OnWindowSizeChanged();
            m_panAndZoomAndRotationGesturesHandler_10.OnWindowSizeChanged();
            m_panAndZoomAndRotationGesturesHandler_11.OnWindowSizeChanged();
        }

    }

}
