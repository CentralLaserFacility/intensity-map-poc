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

namespace IntensityProfileViewer
{

  public sealed partial class ViewerPanel_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.IDisplayPanelViewModel), 
      typeof(ViewerPanel_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityProfileViewer.IDisplayPanelViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityProfileViewer.IDisplayPanelViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public ViewerPanel_UserControl ( )
    {
      this.InitializeComponent();
      // DataContextChanged += (s,e) => {
      //   System.Diagnostics.Debug.WriteLine(
      //     $"{this.GetType()} DataContext => {DataContext?.GetType().ToString()??"null"}"
      //   ) ;
      //   this.Bindings.Update() ; // Yikes - gotta call this explicitly ? WTF !!!
      //   // Hmm, this 'm_intensityMapImage' UserControl has an x:Bind that is meant to
      //   // bind its data context to the same ViewModel - but is that working ???
      //   // m_intensityMapImageAndProfileGraphs_UserControl.DataContext = ViewModel ;
      // } ;

      // m_imageUpdateHandler.CurrentIntensityMapChanged = ()=> {
      //   ViewModel.CurrentSource.SetRecentlyAcquiredIntensityMap(
      //     m_imageUpdateHandler.CurrentIntensityMap
      //   ) ;
      // } ;
      // this.Loaded += (s,e) => {
      //   m_imageUpdateHandler?.PerformIntensityMapUpdate() ;
      // } ;

      // MATTEO : At which point can we be certain that the entire tree of UI elements
      // has been instantiated ? Should we hook into the 'Loaded' event ??
      this.Loaded += (s,e) => {

      } ;

    }

    private void ResetPanZoomButton_Click ( object sender, RoutedEventArgs e )
    {
      // HACK ...
      m_intensityMapImageAndProfileGraphs_UserControl.ResetPanZoomButton_Click(sender,e) ;
    }

  }

}
