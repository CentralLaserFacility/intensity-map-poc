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

  //
  // Experiment that uses the same drawing logic as the Horizontal graph
  // and applies an additional Transform to rotate the drawing.
  //
  // If this works, we can refactor the Horizontal and Vertical graphs
  // to both re-use the same drawing code.
  //

  public sealed partial class VerticalProfileGraph_UserControl_v2 : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.ISourceViewModel), 
      typeof(VerticalProfileGraph_UserControl), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as VerticalProfileGraph_UserControl_v2 ;
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

    public VerticalProfileGraph_UserControl_v2()
    {
      this.InitializeComponent();
      Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register(
        this,
        (VerticalProfileGraph_UserControl_v2 self, ReferencePositionChangedMessage message) => {
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

    private void DrawVerticalProfileGraph_IndividualLines (
      SkiaSharp.SKCanvas skiaCanvas
    ) {

      SkiaSharp.SKRect canvasRect = SkiaSharp.SKRect.Create(
        new SkiaSharp.SKSize(
          skiaCanvas.DeviceClipBounds.Width,
          skiaCanvas.DeviceClipBounds.Height
        )
      ) ;

      //
      // Nominally we'll draw the graph in a 'horizontal' form :
      //
      //  [+]------Width--------+
      //   |                    |       [+] indicates the [0,0] origin
      //   |                 Height
      //   |#####               |
      //   +--------------------+
      //
      // But we want to have it rendered in a 'vertical' form :
      //
      //  [+]--------+
      //   |#        |
      //   |#        |
      //   |#        |
      //   |         |
      //   |         |
      //   |         |
      //   +---------+
      //
      // This is achieved by applying the following sequence of transforms :
      //
      // 1. Rotate clockwise by 90 degrees
      // 2. Shift right by 'Height'
      //

      SkiaSharp.SKMatrix rotateClockwise = SkiaSharp.SKMatrix.CreateRotationDegrees(
        90.0f // Positive value gives a clockwise rotation
      ) ;
      SkiaSharp.SKMatrix shiftRight = SkiaSharp.SKMatrix.CreateTranslation(
        canvasRect.Height,
        0.0f
      ) ;
      SkiaSharp.SKMatrix scale = SkiaSharp.SKMatrix.CreateScale(
        canvasRect.Width  / canvasRect.Height,
        canvasRect.Height / canvasRect.Width
      ) ;

      var panZoomMatrix = SkiaSceneRenderer.GetTransformParameters_VerticalOnly(
        ViewModel.Parent.PanAndZoomParameters
      ) ;

      skiaCanvas.SetMatrix(
        // panZoomMatrix.PostConcat(rotateClockwise).PostConcat(shiftRight).PostConcat(scale)
        rotateClockwise.PostConcat(shiftRight).PostConcat(scale).PostConcat(panZoomMatrix)
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
      int iSpecial = m_latestReferencePositionChangedMessage?.Y ?? -1 ;

      canvasRect.UnpackVisibleCornerPoints(
        out SkiaSharp.SKPoint topLeftPoint,    
        out SkiaSharp.SKPoint topRightPoint,   
        out SkiaSharp.SKPoint bottomLeftPoint, 
        out SkiaSharp.SKPoint bottomRightPoint
      ) ;
      float spaceAtTopAndBottom = 0.0f ;
      int nPoints = ViewModel.MostRecentlyAcquiredIntensityMap.Dimensions.Height ;
      List<SkiaSharp.SKPoint> points = new List<SkiaSharp.SKPoint>() ;
      var intensityValues = ViewModel.MostRecentlyAcquiredIntensityMap.VerticalSliceAtColumn(
        ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.X
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
