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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NativeUwp_ViewerApp_01
{

  public sealed partial class HorizontalProfileGraph_UserControl : UserControl
  {

    // private IntensityMapViewer.ISourceViewModel ViewModel => DataContext as IntensityMapViewer.ISourceViewModel ;

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

    private UwpSkiaUtilities.PanAndZoomAndRotationGesturesHandler m_panAndZoomAndRotationGesturesHandler ;

    public HorizontalProfileGraph_UserControl ( )
    {
      this.InitializeComponent();
    }

    private void OnViewModelPropertyChanged ( 
      IntensityMapViewer.ISourceViewModel? oldViewModel,
      IntensityMapViewer.ISourceViewModel? newViewModel
    ) {
      // if ( IntensityMapImage_UserControl.SupportPanAndZoom )
      // {
      //   m_panAndZoomAndRotationGesturesHandler = new(
      //     m_skiaCanvas,
      //     new SkiaSceneRenderer(DrawHorizontalProfileGraph_IndividualLines){
      //       // ShowTransformMatrixInfo = true,
      //       RenderHook = (canvas) => {
      //         // var effectiveMatrix = newViewModel.Parent.PanAndZoomParameters
      //         canvas.SetMatrix(
      //           SkiaSceneRenderer.GetTransformParameters_HorizontalOnly(
      //             newViewModel.Parent.PanAndZoomParameters
      //           )
      //         ) ;
      //         // matrix = canvas.TotalMatrix ;
      //       }
      //     }
      //   ) ;
      // }
      // else
      {
        m_skiaCanvas.PaintSurface += (s,paintSurfaceEventArgs) => {
          // UwpSkiaUtilities.DrawingHelpers.DrawBoundingBox(
          //   paintSurfaceEventArgs
          // ) ;
          DrawHorizontalProfileGraph_IndividualLines(
            paintSurfaceEventArgs.Surface.Canvas// ,
            // SkiaSharp.SKRect.Create(//
            //   new SkiaSharp.SKSize(
            //     paintSurfaceEventArgs.Info.Width,
            //     paintSurfaceEventArgs.Info.Height
            //   )
            // ) 
          ) ;
        } ;
      }
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
          var bottomAnchorPoint = UwpSkiaUtilities.DrawingHelpers.GetPointAtFractionalPositionAlongLine(
            bottomLeftPoint.MovedBy(spaceAtTopAndBottom,-spaceAtTopAndBottom),
            bottomRightPoint.MovedBy(-spaceAtTopAndBottom,-spaceAtTopAndBottom),
            i / (float) nPoints
          ) ;
          skiaCanvas.DrawVerticalLineUp(
            bottomAnchorPoint,
            lineLength,
            red
          ) ;
          points.Add(
            bottomAnchorPoint.MovedBy(0,-lineLength)
          ) ;
        }
      ) ;
      skiaCanvas.DrawPoints(
        SkiaSharp.SKPointMode.Polygon,
        points.ToArray(),
        red
      ) ;
    }

    public static T TryGetValue<T> ( System.Func<T> func )
    {
      try
      {
        return func() ;
      }
      catch
      {
        return func() ;
        throw ;
      }
    }

  }

}
