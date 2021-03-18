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
using Microsoft.Toolkit.Mvvm.Messaging;
using IntensityProfileViewer.ExtensionMethods;


namespace IntensityProfileViewer.UserControls
{

  public sealed partial class HorizontalProfileGraph_XAML_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.ISourceViewModel), 
      typeof(HorizontalProfileGraph_UserControl), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as HorizontalProfileGraph_XAML_UserControl ;
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

    public HorizontalProfileGraph_XAML_UserControl()
    {
      this.InitializeComponent();
    }

    private void OnViewModelPropertyChanged ( 
      IntensityProfileViewer.ISourceViewModel? oldViewModel,
      IntensityProfileViewer.ISourceViewModel? newViewModel
    ) {
      // m_skiaCanvas.PaintSurface += (s,paintSurfaceEventArgs) => {
      //   DrawHorizontalProfileGraph_IndividualLines(
      //     paintSurfaceEventArgs.Surface.Canvas
      //   ) ;
      //   // SkiaUtilities.DrawingHelpers.DrawBoundingBox(
      //   //   paintSurfaceEventArgs.Surface.Canvas
      //   // ) ;
      // } ;
      newViewModel.NewIntensityMapAcquired += () => PerformRepaint() ;
      newViewModel.ProfileDisplaySettings.ProfileGraphsReferencePositionChanged += () => PerformRepaint() ;
      newViewModel.Parent.IntensityMapVisualisationHasChanged += () => PerformRepaint() ;
    }

    private void PerformRepaint ( )
    {
      // m_skiaCanvas.Invalidate() ;
    }

    private Windows.UI.Xaml.Shapes.Path MyPath => new(){
      Data = new GeometryGroup()
    } ;

  }

}
