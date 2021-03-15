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
using IntensityProfileViewer.ExtensionMethods;
using SkiaUtilities;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace IntensityProfileViewer
{

  public sealed partial class VerticalProfileGraph_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.ISourceViewModel), 
      typeof(VerticalProfileGraph_UserControl), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as VerticalProfileGraph_UserControl ;
          userControlThatOwnsThisViewModelProperty.OnViewModelPropertyChanged(
            propertyChangedEventArgs.OldValue as IntensityProfileViewer.ISourceViewModel,
            propertyChangedEventArgs.NewValue as IntensityProfileViewer.ISourceViewModel
          ) ;
        }
      )
    ) ;

    public IntensityProfileViewer.ISourceViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityProfileViewer.ISourceViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    private ReferencePositionChangedMessage? m_latestReferencePositionChangedMessage = null ;

    public VerticalProfileGraph_UserControl()
    {
      this.InitializeComponent();
      Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register(
        this,
        (VerticalProfileGraph_UserControl self, ReferencePositionChangedMessage message) => {
          m_latestReferencePositionChangedMessage = message ;
        }
      ) ;
    }

    private void OnViewModelPropertyChanged ( 
      IntensityProfileViewer.ISourceViewModel? oldViewModel,
      IntensityProfileViewer.ISourceViewModel? newViewModel
    ) {
      m_skiaCanvas.PaintSurface += (s,paintSurfaceEventArgs) => {
        // UwpSkiaUtilities.DrawingHelpers.DrawBoundingBox(
        //   paintSurfaceEventArgs
        // ) ;
        DrawVerticalProfileGraph_IndividualLines(
          paintSurfaceEventArgs.Surface.Canvas
        ) ;
      } ;
      newViewModel.NewIntensityMapAcquired += () => PerformRepaint() ;
      newViewModel.ProfileDisplaySettings.ProfileGraphsReferencePositionChanged += () => PerformRepaint() ;
      newViewModel.Parent.IntensityMapVisualisationHasChanged += () => PerformRepaint() ;
    }

    private void PerformRepaint ( )
    {
      m_skiaCanvas.Invalidate() ;
    }

    // Nasty HACK ...

    public IntensityMapImage_UserControl IntensityMapImage_UserControl ;

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

      var normal = new SkiaSharp.SKPaint(){
        Color = SkiaSharp.SKColors.Red
      } ;
      float zoomCompensationFactor = 1.0f / skiaCanvas.TotalMatrix.ScaleY ; 
      var special = new SkiaSharp.SKPaint(){
        Color = SkiaSharp.SKColors.Blue,
        StrokeWidth = zoomCompensationFactor * 3
      } ;
      int iSpecial = m_latestReferencePositionChangedMessage?.Y ?? -1 ;

      canvasRect.UnpackVisibleCornerPoints(
        out SkiaSharp.SKPoint topLeftPoint,    
        out SkiaSharp.SKPoint topRightPoint,   
        out SkiaSharp.SKPoint bottomLeftPoint, 
        out SkiaSharp.SKPoint bottomRightPoint
      ) ;

      // We only want to make our graph as high as the rectangle in which we're drawing the Image

      bottomLeftPoint.Y  = topLeftPoint.Y  + IntensityMapImage_UserControl.RectInWhichToDrawBitmap.Height ;
      bottomRightPoint.Y = topRightPoint.Y + IntensityMapImage_UserControl.RectInWhichToDrawBitmap.Height ;

      float spaceAtTopAndBottom = 0.0f ;
      int nPoints = ViewModel.MostRecentlyAcquiredIntensityMap.Dimensions.Height ;
      List<SkiaSharp.SKPoint> points = new List<SkiaSharp.SKPoint>() ;
      var intensityValues = ViewModel.MostRecentlyAcquiredIntensityMap.VerticalSliceAtColumn(
        ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.X
      ).WithNormalisationApplied(
        new IntensityProfileViewer.Normaliser(
          ViewModel.Parent.ImagePresentationSettings.NormalisationValue
        )
      ) ;
      intensityValues.ForEachItem(
        (value,i) => {
          float lineLength = (
            (
              canvasRect.Width - 1
            - spaceAtTopAndBottom * 2.0f
            )
          * value / 255.0f 
          ) ;
          var leftAnchorPoint = SkiaUtilities.DrawingHelpers.GetPointAtFractionalPositionAlongLine(
            topLeftPoint.MovedBy(spaceAtTopAndBottom,spaceAtTopAndBottom),
            bottomLeftPoint.MovedBy(spaceAtTopAndBottom,-spaceAtTopAndBottom),
            i / (float) nPoints
          ) ;
          skiaCanvas.DrawHorizontalLineRight(
            leftAnchorPoint,
            lineLength,
            i == iSpecial ? special : normal
          ) ;
          points.Add(
            leftAnchorPoint.MovedBy(lineLength,0)
          ) ;
        }
      ) ;
      skiaCanvas.DrawPoints(
        SkiaSharp.SKPointMode.Polygon,
        points.ToArray(),
        normal
      ) ;
    }

  }

}