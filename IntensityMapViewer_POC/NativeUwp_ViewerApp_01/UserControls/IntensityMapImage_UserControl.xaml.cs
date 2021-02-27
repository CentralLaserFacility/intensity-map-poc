using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UwpSkiaUtilities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NativeUwp_ViewerApp_01
{

  public sealed partial class IntensityMapImage_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.ISourceViewModel), 
      typeof(IntensityMapImage_UserControl), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as IntensityMapImage_UserControl ;
          userControlThatOwnsThisViewModelProperty.OnViewModelPropertyChanged(
            propertyChangedEventArgs.OldValue as IntensityMapViewer.ISourceViewModel,
            propertyChangedEventArgs.NewValue as IntensityMapViewer.ISourceViewModel
          ) ;
        }
      )
    ) ;

    public IntensityMapViewer.ISourceViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityMapViewer.ISourceViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    private UwpSkiaUtilities.PanAndZoomAndRotationGesturesHandler m_panAndZoomAndRotationGesturesHandler ;

    public static bool SupportPanAndZoom = true ;

    public IntensityMapImage_UserControl()
    {
      this.InitializeComponent() ;
      // This event is getting raised even when we're shutting down,
      // and at that point the ViewModel can have been set to null ...
      // Maybe we should deregister the event handler in 'OnUnloaded' ??
    }

    private void OnViewModelPropertyChanged ( 
      IntensityMapViewer.ISourceViewModel? oldViewModel,
      IntensityMapViewer.ISourceViewModel? newViewModel
    ) {
      if ( SupportPanAndZoom )
      {
        // SkiaSharp.SKMatrix matrix = new() ;
        m_panAndZoomAndRotationGesturesHandler = new(
          m_skiaCanvas,
          new SkiaSceneRenderer(DrawIntensityMap){
            ShowTransformMatrixInfo = true,
            RenderHook = (canvas) => {
              SkiaSceneRenderer.LoadPanAndZoomParameters(
                newViewModel.Parent.PanAndZoomParameters,
                canvas.TotalMatrix
              ) ;
              // matrix = canvas.TotalMatrix ;
            }
          }
        ) ;
      }
      else
      {
        m_skiaCanvas.PaintSurface += DrawSkiaContent ;
      }
      newViewModel.NewIntensityMapAcquired += () => PerformRepaint() ;
      newViewModel.ProfileDisplaySettings.ProfileGraphsReferencePositionChanged += () => PerformRepaint() ;
      ViewModel.Parent.ImagePresentationSettings.PropertyChanged += (s,e) => {
        PerformRepaint() ;
      } ;
    }

    private void PerformRepaint ( )
    {
      m_skiaCanvas.Invalidate() ;
    }

    private void DrawSkiaContent ( 
      object                                      sender, 
      SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs paintSurfaceEventArgs
    ) {
      SkiaSharp.SKCanvas skiaCanvas = paintSurfaceEventArgs.Surface.Canvas ;
      SkiaSharp.SKRectI deviceClipBounds = skiaCanvas.DeviceClipBounds ;
      Common.DebugHelpers.WriteDebugLines(
        $"Skia.Canvas.DeviceClipBounds : [{deviceClipBounds.Left},{deviceClipBounds.Top}] size [{deviceClipBounds.Width}x{deviceClipBounds.Height}]"
      ) ;
      // All we need is the Canvas - we can query the ImageInfo to get the dimensions
      // but that gives us the same info we'll get from the 'device clip bounds'
      SkiaSharp.SKImageInfo imageInfo = paintSurfaceEventArgs.Info ;
      Common.DebugHelpers.WriteDebugLines(
        $"SKImageInfo : size [{imageInfo.Width}x{imageInfo.Height}]"
      ) ;
      // SkiaSharp.SKRect localClipBounds = skiaCanvas.LocalClipBounds ;
      // Common.DebugHelpers.WriteDebugLines(
      //   $"Skia.Canvas.LocalClipBounds : [{localClipBounds.Left},{localClipBounds.Top}] size [{localClipBounds.Width}x{localClipBounds.Height}]"
      // ) ;
    }

    private void DrawIntensityMap ( SkiaSharp.SKCanvas skiaCanvas )
    { 
      ViewModel.Parent.RaiseIntensityMapVisualisationHasChangedEvent() ;
      var deviceClipBounds = skiaCanvas.DeviceClipBounds ;
      // Draw a diagonal line (debugging)
      // skiaCanvas.DrawLine(
      //   new SkiaSharp.SKPoint(
      //     deviceClipBounds.Location.X,
      //     deviceClipBounds.Location.Y
      //   ),
      //   new SkiaSharp.SKPoint(
      //     deviceClipBounds.Location.X + deviceClipBounds.Width  - 1,
      //     deviceClipBounds.Location.Y + deviceClipBounds.Height - 1
      //   ),
      //   new SkiaSharp.SKPaint(){
      //     Color = SkiaSharp.SKColors.Red
      //   }
      // ) ;
      if ( ViewModel != null )
      { 
        // Hmm, should try to eliminate this test ...
        var intensityMap = ViewModel.MostRecentlyAcquiredIntensityMap ;
        var bitmap = new SkiaSharp.SKBitmap(
          intensityMap.Dimensions.Width,
          intensityMap.Dimensions.Height
        ) ;
        var colourMapOption = ViewModel.Parent.ImagePresentationSettings.ColourMapOption ;
        var colourMapper = IntensityMapViewer.ColourMapper.InstanceFor(colourMapOption) ;
        bitmap.Pixels = intensityMap.IntensityValues.Select(
          intensity => new SkiaSharp.SKColor(
            colourMapper.MapByteValueToEncodedARGB(intensity)
            // red   : intensity,
            // green : intensity,
            // blue  : intensity
          )
        ).ToArray() ;
        SkiaSharp.SKRect rectInWhichToDrawBitmap = new SkiaSharp.SKRect(
          left   : 0.0f,
          top    : 0.0f,
          right  : deviceClipBounds.Width,
          bottom : deviceClipBounds.Height
        ) ;
        skiaCanvas.DrawBitmap(
          bitmap,
          rectInWhichToDrawBitmap
        ) ;
        if ( ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.HasValue )
        {
          var referencePosition = ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value ;
          int xAlongFromLeft = Scale(
            referencePosition.X,
            intensityMap.Dimensions.Width,
            deviceClipBounds.Width
          ) ;
          int yDownFromTop = Scale(
            ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.Y,
            intensityMap.Dimensions.Height,
            deviceClipBounds.Height
          ) ;
          var scaledReferencePoint = new SkiaSharp.SKPoint(
            xAlongFromLeft,
            yDownFromTop
          ) ;
          var horizontalLine = new HorizontalLine(
            scaledReferencePoint,
            0.0f,
            deviceClipBounds.Width
          ) ;
          var verticalLine = new VerticalLine(
            scaledReferencePoint,
            0.0f,
            deviceClipBounds.Height
          ) ;
          var lineStyle = new SkiaSharp.SKPaint(){
            Color       = SkiaSharp.SKColors.Red,
            StrokeWidth = 3
          } ;
          horizontalLine.Draw(skiaCanvas,lineStyle) ;
          verticalLine.Draw(skiaCanvas,lineStyle) ;
        }
        static int Scale ( double value, double nImagePixels, double nDisplayPixels )
        => (int) (
          value * nDisplayPixels / nImagePixels
        ) ;
      }
    }

  }

  // We need a 'Line' abstraction that (A) can be drawn,
  // and (B) can participate in Hit Testing ...

  public abstract record Line ( SkiaSharp.SKPoint From, SkiaSharp.SKPoint To )
  {
    public void Draw ( SkiaSharp.SKCanvas canvas, SkiaSharp.SKPaint paint )
    {
      canvas.DrawLine(
        From,
        To,
        paint
      ) ;
    }
    public abstract bool CoincidesWithMousePosition ( SkiaSharp.SKPoint mousePosition, float maxDelta = 4.0f ) ;
  }

  public record HorizontalLine : Line
  {
    public HorizontalLine (
      SkiaSharp.SKPoint pointOnLine,
      float extremeLeftX,
      float extremeRightX
    ) :
    base(
      From : new SkiaSharp.SKPoint(
        extremeLeftX,
        pointOnLine.Y
      ),
      To : new SkiaSharp.SKPoint(
        extremeRightX,
        pointOnLine.Y
      )
    ) {
    }
    public override bool CoincidesWithMousePosition ( SkiaSharp.SKPoint mousePosition, float maxDelta = 4.0f )
    => System.MathF.Abs(
      From.Y - mousePosition.Y
    ) > maxDelta ;
  }

  public record VerticalLine : Line
  {
    public VerticalLine (
      SkiaSharp.SKPoint pointOnLine,
      float extremeTopY,
      float extremeBottomY
    ) :
    base(
      From : new SkiaSharp.SKPoint(
        pointOnLine.X,
        extremeTopY
      ),
      To : new SkiaSharp.SKPoint(
        pointOnLine.X,
        extremeBottomY
      )
    ) {
    }
    public override bool CoincidesWithMousePosition ( SkiaSharp.SKPoint mousePosition, float maxDelta = 4.0f )
    => System.MathF.Abs(
      From.X - mousePosition.X
    ) > maxDelta ;
  }

}
