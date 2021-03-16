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
using SkiaUtilities;
using Microsoft.Toolkit.Mvvm.Messaging;
using IntensityProfileViewer.ExtensionMethods;
using SkiaSharp;

namespace IntensityProfileViewer
{

  public sealed partial class HorizontalProfileGraph_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.ISourceViewModel), 
      typeof(HorizontalProfileGraph_UserControl), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as HorizontalProfileGraph_UserControl ;
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

    public HorizontalProfileGraph_UserControl ( )
    {
      this.InitializeComponent();
      Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register(
        this,
        (HorizontalProfileGraph_UserControl self, ReferencePositionChangedMessage message) => {
          m_latestReferencePositionChangedMessage = message ;
        }
      ) ;
      m_executionTimingStopwatch.Start() ;
    }

    private readonly System.Diagnostics.Stopwatch m_executionTimingStopwatch = new() ;

    private void OnViewModelPropertyChanged ( 
      IntensityProfileViewer.ISourceViewModel? oldViewModel,
      IntensityProfileViewer.ISourceViewModel? newViewModel
    ) {
      m_skiaCanvas.PaintSurface += (s,paintSurfaceEventArgs) => {
        DrawHorizontalProfileGraph_IndividualLines(
          paintSurfaceEventArgs.Surface.Canvas
        ) ;
        // SkiaUtilities.DrawingHelpers.DrawBoundingBox(
        //   paintSurfaceEventArgs.Surface.Canvas
        // ) ;
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

    private void DrawHorizontalProfileGraph_IndividualLines (
      SkiaSharp.SKCanvas skiaCanvas
    ) {

      skiaCanvas.SetMatrix(
        SkiaSceneRenderer.GetTransformParameters_HorizontalOnly(
          ViewModel.Parent.PanAndZoomParameters
        )
      ) ;

      SkiaSharp.SKRect canvasRect = SkiaSharp.SKRect.Create(
        new SkiaSharp.SKSize(
          skiaCanvas.DeviceClipBounds.Width,
          skiaCanvas.DeviceClipBounds.Height
        )
      ) ;

      System.TimeSpan timeBeforeRenderStarted = m_executionTimingStopwatch.Elapsed ;

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
      float zoomCompensationFactor = 1.0f / skiaCanvas.TotalMatrix.ScaleX ; 
      var special = new SkiaSharp.SKPaint(){
        Color = SkiaSharp.SKColors.Blue,
        StrokeWidth = zoomCompensationFactor * 3
      } ;
      int iSpecial = m_latestReferencePositionChangedMessage?.X ?? -1 ;

      //
      // Nominally we'll draw the graph into the entire space available in our Canvas :
      //
      //   +-----------------------------+
      //   |                             |
      //   |                             |
      //   |                             |
      //   +-----------------------------+
      //
      // However if the Image has had to be rendered into a rectangle that is
      // smaller than the entire Canvas of the Image control, in order to preserve
      // the aspect ratio, then we'll need to adjust this so that we only use
      // an appropriate 'width' for the graph :
      //
      //   +------------------+ - - - - -+
      //   |                  |          |
      //   |                  |          |
      //   |                  |          |
      //   +------------------+ - - - - -+
      //

      canvasRect.UnpackVisibleCornerPoints(
        out SkiaSharp.SKPoint topLeftPoint,    
        out SkiaSharp.SKPoint topRightPoint,   
        out SkiaSharp.SKPoint bottomLeftPoint, 
        out SkiaSharp.SKPoint bottomRightPoint
      ) ;

      // We only want to make our graph as wide as the rectangle in which we're drawing the Image

      bottomRightPoint.X = bottomLeftPoint.X + IntensityMapImage_UserControl.RectInWhichToDrawBitmap.Width ;
      topRightPoint.X    = topLeftPoint.X    + IntensityMapImage_UserControl.RectInWhichToDrawBitmap.Width ;

      int nPoints = ViewModel.MostRecentlyAcquiredIntensityMap.Dimensions.Width ;      if ( nPoints < 2 )
      {
        return ;
      }
      List<SkiaSharp.SKPoint> points = new() ;
      var intensityValues = ViewModel.MostRecentlyAcquiredIntensityMap.HorizontalSliceAtRow(
        ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.Y
      ).WithNormalisationApplied(
        new IntensityProfileViewer.Normaliser(
          ViewModel.Parent.ImagePresentationSettings.NormalisationValue
        )
      ) ;
      intensityValues.ForEachItem(
        (value,i) => {
          float lineLength = (
            (
              canvasRect.Height 
            )
          * value / 255.0f 
          ) ;
          var bottomAnchorPoint = SkiaUtilities.DrawingHelpers.GetPointAtFractionalPositionAlongLine(
            bottomLeftPoint.MovedBy(0,1),
            bottomRightPoint.MovedBy(0,1),
            i / (float) ( nPoints - 1 ) // ??????????????
          ) ;
          skiaCanvas.DrawVerticalLineUp(
            bottomAnchorPoint,
            lineLength,
            i == iSpecial ? special : normal
          ) ;
          points.Add(
            bottomAnchorPoint.MovedBy(0,-lineLength)
          ) ;
        }
      ) ;
      skiaCanvas.DrawPoints(
        SkiaSharp.SKPointMode.Polygon,
        points.ToArray(),
        normal
      ) ;

      System.TimeSpan timeAfterRenderCompleted = m_executionTimingStopwatch.Elapsed ;
      System.TimeSpan renderTimeElapsed = timeAfterRenderCompleted - timeBeforeRenderStarted ;

      skiaCanvas.SetMatrix(
        SkiaSharp.SKMatrix.CreateIdentity()
      ) ;
      skiaCanvas.DrawText(
        $"Render time (mS) {renderTimeElapsed.TotalMilliseconds:F1}",
        new(2.0f,14.0f),
        normal
      ) ;

    }

  }

}
