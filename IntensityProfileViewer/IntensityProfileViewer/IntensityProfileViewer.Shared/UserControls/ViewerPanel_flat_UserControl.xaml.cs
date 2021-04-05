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

  public sealed partial class ViewerPanel_flat_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.IDisplayPanelViewModel), 
      typeof(ViewerPanel_flat_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityProfileViewer.IDisplayPanelViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityProfileViewer.IDisplayPanelViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public ViewerPanel_flat_UserControl ( )
    {
      this.InitializeComponent() ;
    }

    private void ResetPanZoomButton_Click ( object sender, RoutedEventArgs e )
    {
      // HACK ...
      // m_intensityMapImageAndProfileGraphs_UserControl.ResetPanZoomButton_Click(sender,e) ;
    }

  }

}
