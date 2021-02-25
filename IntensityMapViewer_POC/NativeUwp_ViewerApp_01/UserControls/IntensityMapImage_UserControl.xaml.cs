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

    public IntensityMapImage_UserControl()
    {
      this.InitializeComponent();
      // This event is getting raised even when we're shutting down,
      // and at that point the ViewModel can have been set to null ...
      // Maybe we should deregister the event handler in 'OnUnloaded' ??
      m_skiaCanvas.PaintSurface += DrawSkiaContent ;
    }

    private void OnViewModelPropertyChanged ( 
      IntensityMapViewer.ISourceViewModel? oldViewModel,
      IntensityMapViewer.ISourceViewModel? newViewModel
    ) {
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
        // SkiaSharp.SKRect rectInWhichToDrawBitmap = new SkiaSharp.SKRect(
        //   left   : 100.0f,
        //   top    : 10.0f,
        //   right  : 100.0f + intensityMap.Dimensions.Width/2,
        //   bottom : 10.0f  + intensityMap.Dimensions.Height/2 
        // ) ;
        SkiaSharp.SKRect rectInWhichToDrawBitmap = new SkiaSharp.SKRect(
          left   : 0.0f,
          top    : 0.0f,
          right  : deviceClipBounds.Width, // intensityMap.Dimensions.Width,
          bottom : deviceClipBounds.Height // intensityMap.Dimensions.Height
        ) ;
        skiaCanvas.DrawBitmap(
          bitmap,
          rectInWhichToDrawBitmap
        ) ;
      }
    }

  }

}
