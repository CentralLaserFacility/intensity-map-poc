﻿using System;
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
          paintSurfaceEventArgs.Surface.Canvas,
          SkiaSharp.SKRect.Create(
            new SkiaSharp.SKSize(
              paintSurfaceEventArgs.Info.Width,
              paintSurfaceEventArgs.Info.Height
            )
          ) 
        ) ;
      } ;
    }

    private void OnViewModelPropertyChanged ( 
      IntensityMapViewer.ISourceViewModel? oldViewModel,
      IntensityMapViewer.ISourceViewModel? newViewModel
    ) {
      newViewModel.NewIntensityMapAcquired += () => PerformRepaint() ;
      newViewModel.ProfileDisplaySettings.ProfileGraphsReferencePositionChanged += () => PerformRepaint() ;
    }

    private void PerformRepaint ( )
    {
      m_skiaCanvas.Invalidate() ;
    }

    private void DrawVerticalProfileGraph_IndividualLines (
      SkiaSharp.SKCanvas skiaCanvas,
      SkiaSharp.SKRect   canvasRect
    ) {
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
      float spaceAtTopAndBottom = 4.0f ;
      int nPoints = ViewModel.MostRecentlyAcquiredIntensityMap.Dimensions.Height ;
      List<SkiaSharp.SKPoint> points = new List<SkiaSharp.SKPoint>() ;
      var intensityValues = ViewModel.MostRecentlyAcquiredIntensityMap.VerticalSliceAtColumn(
        ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.X
      ) ;
      intensityValues.ForEachItem(
        (value,i) => {
          float lineWidth = (
            (
              canvasRect.Width 
            - spaceAtTopAndBottom * 2.0f
            )
          * value / 255.0f 
          ) ;
          var leftPoint = UwpSkiaUtilities.DrawingHelpers.GetPointAtFractionalPositionAlongLine(
            topLeftPoint.MovedBy(spaceAtTopAndBottom,0),
            bottomLeftPoint.MovedBy(spaceAtTopAndBottom,0),
            i / (float) nPoints
          ) ;
          skiaCanvas.DrawHorizontalLineRight(
            leftPoint,
            lineWidth,
            red
          ) ;
          points.Add(
            leftPoint.MovedBy(0,lineWidth)
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
