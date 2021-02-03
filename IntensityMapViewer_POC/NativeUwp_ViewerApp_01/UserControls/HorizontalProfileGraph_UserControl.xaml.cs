using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NativeUwp_ViewerApp_01
{
  public sealed partial class HorizontalProfileGraph_UserControl : UserControl
  {

    private IntensityMapViewer.ISourceViewModel ViewModel => DataContext as IntensityMapViewer.ISourceViewModel ;

    private IntensityMapViewer.IDisplayPanelViewModel RootViewModel => ViewModel.Parent ;

    public HorizontalProfileGraph_UserControl()
    {
      this.InitializeComponent();
      DataContextChanged += (s,e) => {
        System.Diagnostics.Debug.WriteLine(
          $"{this.GetType()} DataContext => {DataContext?.GetType().ToString()??"null"}"
        ) ;
        this.Bindings.Update() ; // Yikes - gotta call this explicitly ? WTF !!!
      } ;
      m_skiaCanvas.PaintSurface += DrawSkiaContent ;
    }

    private void DrawSkiaContent ( 
      object                                      sender, 
      SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs paintSurfaceEventArgs
    ) {
      SkiaSharp.SKImageInfo imageInfo = paintSurfaceEventArgs.Info ;
      Common.DebugHelpers.WriteDebugLines(
        $"SKImageInfo : size [{imageInfo.Width}x{imageInfo.Height}]"
      ) ;
      SkiaSharp.SKCanvas skiaCanvas = paintSurfaceEventArgs.Surface.Canvas ;
      SkiaSharp.SKRectI deviceClipBounds = skiaCanvas.DeviceClipBounds ;
      SkiaSharp.SKRect localClipBounds = skiaCanvas.LocalClipBounds ;
      Common.DebugHelpers.WriteDebugLines(
        $"Skia.Canvas.DeviceClipBounds : [{deviceClipBounds.Left},{deviceClipBounds.Top}] size [{deviceClipBounds.Width}x{deviceClipBounds.Height}]"
      ) ;
      Common.DebugHelpers.WriteDebugLines(
        $"Skia.Canvas.LocalClipBounds : [{localClipBounds.Left},{localClipBounds.Top}] size [{localClipBounds.Width}x{localClipBounds.Height}]"
      ) ;
      paintSurfaceEventArgs.Surface.Canvas.DrawLine(
        new SkiaSharp.SKPoint(
          deviceClipBounds.Location.X,
          deviceClipBounds.Location.Y
        ),
        new SkiaSharp.SKPoint(
          deviceClipBounds.Location.X + deviceClipBounds.Width  - 1,
          deviceClipBounds.Location.Y + deviceClipBounds.Height - 1
        ),
        new SkiaSharp.SKPaint(){
          Color = SkiaSharp.SKColors.Red
        }
      ) ;
      var intensityMap = ViewModel.MostRecentlyAcquiredIntensityMap ;
      var bitmap = new SkiaSharp.SKBitmap(
        intensityMap.Dimensions.Width,
        intensityMap.Dimensions.Height
      ) ;
      bitmap.Pixels = intensityMap.IntensityValues.Select(
        intensity => new SkiaSharp.SKColor(
          red   : intensity,
          green : intensity,
          blue  : intensity
        )
      ).ToArray() ;
      SkiaSharp.SKRect rectInWhichToDrawBitmap = new SkiaSharp.SKRect(
        left   : 100.0f,
        top    : 10.0f,
        right  : 100.0f + intensityMap.Dimensions.Width/2,
        bottom : 10.0f  + intensityMap.Dimensions.Height/2 
      ) ;
      skiaCanvas.DrawBitmap(
        bitmap,
        rectInWhichToDrawBitmap
      ) ;
    }

  }

}
