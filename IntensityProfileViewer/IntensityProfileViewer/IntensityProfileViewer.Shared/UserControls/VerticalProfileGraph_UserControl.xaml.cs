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
      #if DO_RENDER_TIMING_MEASUREMENTS
      m_executionTimingStopwatch.Start() ;
      #endif
    }

    #if DO_RENDER_TIMING_MEASUREMENTS
    private readonly System.Diagnostics.Stopwatch m_executionTimingStopwatch = new() ;
    #endif

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
      newViewModel.Parent.ImagePresentationSettings.PropertyChanged += (s, e) => {
        PerformRepaint() ;
      } ;
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

      int nPoints = ViewModel.MostRecentlyAcquiredIntensityMap.Dimensions.Height ;
      if ( nPoints < 2 )
      {
        return ;
      }
      var intensityValues = ViewModel.MostRecentlyAcquiredIntensityMap.VerticalSliceAtColumn(
        ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.X
      ).WithNormalisationApplied(
        new IntensityProfileViewer.Normaliser(
          ViewModel.Parent.ImagePresentationSettings.NormalisationValue
        )
      ) ;

      #if DO_RENDER_TIMING_MEASUREMENTS
      System.TimeSpan timeBeforeDrawingStarted = m_executionTimingStopwatch.Elapsed ;
      #endif

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

      skiaCanvas.Clear(
        SkiaColourChoices.ProfileGraphBackgroundColour
      ) ;

      if (
         ViewModel.MostRecentlyAcquiredIntensityMap == null
      || ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.HasValue is false
      ) {
        return ;
      }

      float zoomCompensationFactor = 1.0f / skiaCanvas.TotalMatrix.ScaleX ; 
      var normal = new SkiaSharp.SKPaint(){
        Color = SkiaColourChoices.ProfileGraphLineColour//, // SkiaSharp.SKColors.Red
        //StrokeWidth = zoomCompensationFactor * SkiaColourChoices.ProfileGraphNominalLineWidth
      } ;
      var special = new SkiaSharp.SKPaint(){
        Color = SkiaColourChoices.ProfileGraphHighlightedLineColour, // SkiaSharp.SKColors.Blue
        StrokeWidth = zoomCompensationFactor * SkiaColourChoices.ProfileGraphHighlightedLineWidth
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

      List<SkiaSharp.SKPoint> points = new(nPoints) ;
      intensityValues.ForEachItem(
        (value,i) => {
          // Draw horizontal lines as we scan the values
          float lineLength = (
            (
              canvasRect.Width - 1
            )
          * value / 255.0f 
          ) ;
          var leftAnchorPoint = SkiaUtilities.DrawingHelpers.GetPointAtFractionalPositionAlongLine(
            topLeftPoint.MovedBy(0,0),
            bottomLeftPoint.MovedBy(0,0),
            i / (float) ( nPoints - 1 )
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
      // Now draw a line connecting all the points
      skiaCanvas.DrawPoints(
        SkiaSharp.SKPointMode.Polygon,
        points.ToArray(),
        normal
      ) ;

      #if DO_RENDER_TIMING_MEASUREMENTS
      System.TimeSpan timeAfterDrawingCompleted = m_executionTimingStopwatch.Elapsed ;
      System.TimeSpan drawingTimeElapsed = timeAfterDrawingCompleted - timeBeforeDrawingStarted ;
      Common.DebugHelpers.WriteDebugLines(
        $"Skia drawing time (mS) {drawingTimeElapsed.TotalMilliseconds:F3}"
      ) ;
      #endif

    }

  }

}
