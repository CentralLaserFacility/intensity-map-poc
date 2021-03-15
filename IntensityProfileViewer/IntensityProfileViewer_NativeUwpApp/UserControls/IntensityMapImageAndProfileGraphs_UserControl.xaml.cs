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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NativeUwp_ViewerApp_01
{

  public sealed partial class IntensityMapImageAndProfileGraphs_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.IDisplayPanelViewModel), 
      typeof(IntensityMapImageAndProfileGraphs_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityProfileViewer.IDisplayPanelViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityProfileViewer.IDisplayPanelViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public IntensityMapImageAndProfileGraphs_UserControl ( )
    {
      InitializeComponent() ;
      // Hmm, NASTY HACK ... there's gotta be a better way of setting up this relationship.
      // In the Horizontal control, we could navigate back through the Parent properties
      // until we find this 'IntensityMapImageAndProfileGraphs' control, and then access
      // the 'Image' control and its 'RectInWhichToDrawBitmap' property.
      m_horizontalProfileGraph_UserControl .IntensityMapImage_UserControl = m_intensityMapImage_UserControl ;
      m_verticalProfileGraph_UserControl   .IntensityMapImage_UserControl = m_intensityMapImage_UserControl ;
    }

    public Visibility VisibleWhen ( bool visible )
    => (
      visible
      ? Visibility.Visible
      : Visibility.Collapsed
    ) ;

    public int ChooseIntegerValue ( 
      bool condition, 
      int valueWhenTrue, 
      int valueWhenFalse 
    ) => (
      condition 
      ? valueWhenTrue 
      : valueWhenFalse 
    ) ;

    private void ResetPanZoomButton_Click ( object sender, RoutedEventArgs e )
    {
      m_intensityMapImage_UserControl.ResetPanAndZoom() ;
    }

  }

}
