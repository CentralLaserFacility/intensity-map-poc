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

    public HorizontalProfileGraph_UserControl ( )
    {
      this.InitializeComponent();
      // DataContextChanged += (s,e) => {
      //   System.Diagnostics.Debug.WriteLine(
      //     $"{this.GetType()} DataContext => {DataContext?.GetType().ToString()??"null"}"
      //   ) ;
      //   // Yikes - gotta call this explicitly ? WTF !!!
      //   // this.Bindings.Update() ; 
      // } ;
      m_skiaCanvas.PaintSurface += (s,paintSurfaceEventArgs) => {
        // UwpSkiaUtilities.DrawingHelpers.DrawBoundingBox(
        //   paintSurfaceEventArgs
        // ) ;
        DrawHorizontalProfileGraph_IndividualLines(
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

    private void DrawHorizontalProfileGraph_IndividualLines (
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
      int nPoints = ViewModel.MostRecentlyAcquiredIntensityMap.Dimensions.Width ;
      List<SkiaSharp.SKPoint> points = new List<SkiaSharp.SKPoint>() ;
      var intensityValues = TryGetValue(
        () => ViewModel.MostRecentlyAcquiredIntensityMap.HorizontalSliceAtRow(
          ViewModel.ProfileDisplaySettings.ProfileGraphsReferencePosition.Value.Y
        )
      ) ;
      intensityValues.ForEachItem(
        (value,i) => {
          float lineHeight = (
            (
              canvasRect.Height 
            - spaceAtTopAndBottom * 2.0f
            )
          * value / 255.0f 
          ) ;
          var bottomPoint = UwpSkiaUtilities.DrawingHelpers.GetPointAtFractionalPositionAlongLine(
            bottomLeftPoint.MovedBy(0,-spaceAtTopAndBottom),
            bottomRightPoint.MovedBy(0,-spaceAtTopAndBottom),
            i / (float) nPoints
          ) ;
          skiaCanvas.DrawVerticalLineUp(
            bottomPoint,
            lineHeight,
            red
          ) ;
          points.Add(
            bottomPoint.MovedBy(0,-lineHeight)
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
