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

using Common.ExtensionMethods ;
using UwpSkiaUtilities;

namespace NativeUwp_ViewerApp_01
{

  public sealed partial class VerticalProfileGraph_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.ISourceViewModel), 
      typeof(VerticalProfileGraph_UserControl), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as VerticalProfileGraph_UserControl ;
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

    public VerticalProfileGraph_UserControl()
    {
      this.InitializeComponent();
      m_skiaCanvas.PaintSurface += (s,paintSurfaceEventArgs) => {
        // UwpSkiaUtilities.DrawingHelpers.DrawBoundingBox(
        //   paintSurfaceEventArgs
        // ) ;
        DrawVerticalProfileGraph_IndividualLines(
          paintSurfaceEventArgs.Surface.Canvas//,
          // SkiaSharp.SKRect.Create(
          //   new SkiaSharp.SKSize(
          //     paintSurfaceEventArgs.Info.Width,
          //     paintSurfaceEventArgs.Info.Height
          //   )
          // ) 
        ) ;
      } ;
    }

    private void OnViewModelPropertyChanged ( 
      IntensityMapViewer.ISourceViewModel? oldViewModel,
      IntensityMapViewer.ISourceViewModel? newViewModel
    ) {
      newViewModel.NewIntensityMapAcquired += () => PerformRepaint() ;
      newViewModel.ProfileDisplaySettings.ProfileGraphsReferencePositionChanged += () => PerformRepaint() ;
      newViewModel.Parent.IntensityMapVisualisationHasChanged += () => PerformRepaint() ;
    }

    private void PerformRepaint ( )
    {
      m_skiaCanvas.Invalidate() ;
    }

    private void DrawVerticalProfileGraph_IndividualLines (
      SkiaSharp.SKCanvas skiaCanvas
    ) {

      skiaCanvas.SetMatrix(
        SkiaSceneRenderer.GetTransformParameters_VerticalOnly(
          ViewModel.Parent.PanAndZoomParameters
        )
      ) ;

      SkiaSharp.SKRect canvasRect = SkiaSharp.SKRect.Create(
        new SkiaSharp.SKSize(
          skiaCanvas.DeviceClipBounds.Width,
          skiaCanvas.DeviceClipBounds.Height
        )
      ) ;

      skiaCanvas.Clear(SkiaSharp.SKColors.LightYellow) ;

      if (
         ViewModel.MostRecentlyAcquiredIntensityMap == null
      || ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.HasValue is false
      ) {
        return ;
      }

      var red = new SkiaSharp.SKPaint(){
        Color = SkiaSharp.SKColors.Red
      } ;
      canvasRect.UnpackVisibleCornerPoints(
        out SkiaSharp.SKPoint topLeftPoint,    
        out SkiaSharp.SKPoint topRightPoint,   
        out SkiaSharp.SKPoint bottomLeftPoint, 
        out SkiaSharp.SKPoint bottomRightPoint
      ) ;
      float spaceAtTopAndBottom = 10.0f ;
      int nPoints = ViewModel.MostRecentlyAcquiredIntensityMap.Dimensions.Height ;
      List<SkiaSharp.SKPoint> points = new List<SkiaSharp.SKPoint>() ;
      var intensityValues = ViewModel.MostRecentlyAcquiredIntensityMap.VerticalSliceAtColumn(
        ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.X
      ) ;
      intensityValues.ForEachItem(
        (value,i) => {
          float lineLength = (
            (
              canvasRect.Width 
            - spaceAtTopAndBottom * 2.0f
            )
          * value / 255.0f 
          ) ;
          var leftAnchorPoint = UwpSkiaUtilities.DrawingHelpers.GetPointAtFractionalPositionAlongLine(
            topLeftPoint.MovedBy(spaceAtTopAndBottom,spaceAtTopAndBottom),
            bottomLeftPoint.MovedBy(spaceAtTopAndBottom,-spaceAtTopAndBottom),
            i / (float) nPoints
          ) ;
          skiaCanvas.DrawHorizontalLineRight(
            leftAnchorPoint,
            lineLength,
            red
          ) ;
          points.Add(
            leftAnchorPoint.MovedBy(lineLength,0)
          ) ;
        }
      ) ;
      skiaCanvas.DrawPoints(
        SkiaSharp.SKPointMode.Polygon,
        points.ToArray(),
        red
      ) ;
    }

  }

}
