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

namespace NativeUwp_ViewerApp_01
{

  public sealed partial class HorizontalProfileGraph_UserControl : UserControl
  {

    private IntensityMapViewer.IDisplayPanelViewModel RootViewModel => ViewModel.Parent ;

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.ISourceViewModel), 
      typeof(HorizontalProfileGraph_UserControl), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as HorizontalProfileGraph_UserControl ;
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

    // private UwpSkiaUtilities.PanAndZoomAndRotationGesturesHandler m_panAndZoomAndRotationGesturesHandler ;

    private ReferencePositionChangedMessage? m_latestReferencePositionChangedMessage = null ;

    public HorizontalProfileGraph_UserControl ( )
    {
      this.InitializeComponent();
      // Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register<
      //   HorizontalProfileGraph_UserControl,
      //   ReferencePositionChangedMessage,
      //   int
      // >(
      //   this,
      //   0,
      //   (self,message) => {
      //     // m_latestReferencePositionChangedMessage = message ;
      //   }
      // ) ;
      Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register(
        this,
        (HorizontalProfileGraph_UserControl self, ReferencePositionChangedMessage message) => {
          m_latestReferencePositionChangedMessage = message ;
        }
      ) ;
    }

    private void OnViewModelPropertyChanged ( 
      IntensityMapViewer.ISourceViewModel? oldViewModel,
      IntensityMapViewer.ISourceViewModel? newViewModel
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
      // smaller than the entire Canvas of the Image control, we'll need to adjust this
      // so that we only use an appropriate 'width' for the graph :
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

      float spaceAtTopAndBottom = 0.0f ;
      int nPoints = ViewModel.MostRecentlyAcquiredIntensityMap.Dimensions.Width ;
      List<SkiaSharp.SKPoint> points = new List<SkiaSharp.SKPoint>() ;
      var intensityValues = ViewModel.MostRecentlyAcquiredIntensityMap.HorizontalSliceAtRow(
        ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.Y
      ) ;
      intensityValues.ForEachItem(
        (value,i) => {
          float lineLength = (
            (
              canvasRect.Height 
            - spaceAtTopAndBottom * 2.0f
            )
          * value / 255.0f 
          ) ;
          var bottomAnchorPoint = SkiaUtilities.DrawingHelpers.GetPointAtFractionalPositionAlongLine(
            bottomLeftPoint.MovedBy(spaceAtTopAndBottom,-spaceAtTopAndBottom),
            bottomRightPoint.MovedBy(-spaceAtTopAndBottom,-spaceAtTopAndBottom),
            i / (float) nPoints
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
    }

  }

}
