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
        // this.Bindings.Update() ; // Yikes - gotta call this explicitly ? WTF !!!
      } ;
      m_skiaCanvas.PaintSurface += DrawSkiaContent ;
    }

    private int m_offset = 1 ;

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
      paintSurfaceEventArgs.Surface.Canvas.DrawRect(
        deviceClipBounds.Location.X,
        deviceClipBounds.Location.Y,
        deviceClipBounds.Location.X + deviceClipBounds.Width,
        deviceClipBounds.Location.Y + deviceClipBounds.Height,
        new SkiaSharp.SKPaint(){
          Color = SkiaSharp.SKColors.Blue
        }
      ) ;
      paintSurfaceEventArgs.Surface.Canvas.DrawRect(
        deviceClipBounds.Location.X + m_offset,
        deviceClipBounds.Location.Y + m_offset,
        deviceClipBounds.Location.X + deviceClipBounds.Width  - 1 - m_offset,
        deviceClipBounds.Location.Y + deviceClipBounds.Height - 1 - m_offset,
        new SkiaSharp.SKPaint(){
          Color = SkiaSharp.SKColors.LightGreen
        }
      ) ;
      // Vertical line at the left, down from the top
      paintSurfaceEventArgs.Surface.Canvas.DrawLine(
        new SkiaSharp.SKPoint(
          // Top left
          deviceClipBounds.Location.X,
          deviceClipBounds.Location.Y
        ),
        new SkiaSharp.SKPoint(
          // Top left, down 20
          deviceClipBounds.Location.X, 
          deviceClipBounds.Location.Y + 20
        ),
        new SkiaSharp.SKPaint(){
          Color = SkiaSharp.SKColors.Red
        }
      ) ;
      // Vertical line at the right, up from the bottom
      paintSurfaceEventArgs.Surface.Canvas.DrawLine(
        new SkiaSharp.SKPoint(
          // Bottom right
          deviceClipBounds.Location.X + deviceClipBounds.Width  - 1,
          deviceClipBounds.Location.Y + deviceClipBounds.Height - 1
        ),
        new SkiaSharp.SKPoint(
          // Bottom right, up 20
          deviceClipBounds.Location.X + deviceClipBounds.Width  - 1,
          deviceClipBounds.Location.Y + deviceClipBounds.Height - 1 - 20
        ),
        new SkiaSharp.SKPaint(){
          Color = SkiaSharp.SKColors.Red
        }
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
    }

  }

}
