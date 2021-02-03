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
  public sealed partial class ImagePresentationSettings_UserControl : UserControl
  {

    private IntensityMapViewer.IImagePresentationSettingsViewModel ViewModel => DataContext as IntensityMapViewer.IImagePresentationSettingsViewModel ;

    private IntensityMapViewer.IDisplayPanelViewModel RootViewModel ;

    public ImagePresentationSettings_UserControl()
    {
      this.InitializeComponent();
      DataContextChanged += (s,e) => {
        System.Diagnostics.Debug.WriteLine(
          $"{this.GetType()} DataContext => {DataContext?.GetType().ToString()??"null"}"
        ) ;
        if ( ViewModel != null ) 
        {
          // Yikes - gotta call this explicitly ? WTF !!
          // Hmm, if this is not called initially, most things work,
          // except for the binding to 'IsEnabled'.
          // BUT - even when this is called :
          // 1. Exceptions get thrown (to do with combo box values being null) CURRENTLY DISABLED !!
          // 2. Consequently the displayed combo box values are empty
          this.Bindings.Update() ; 
        }
      } ;
    }

    public List<Common.EnumItemsSource<IntensityMapViewer.ColourMapOption>> ColourMapOptions 
    => Common.EnumItemsSource<IntensityMapViewer.ColourMapOption>.ToList() ;

    public List<Common.EnumItemsSource<IntensityMapViewer.NormalisationMode>> NormalisationModeOptions 
    => Common.EnumItemsSource<IntensityMapViewer.NormalisationMode>.ToList() ;

    private void Slider_ValueChanged ( object sender, RangeBaseValueChangedEventArgs e )
    {
      ViewModel.SetNormalisationValue(
        (byte) m_normalisationValueSlider.Value
      ) ;
    }

    private void m_normalisationValueSlider_IsEnabledChanged ( object sender, DependencyPropertyChangedEventArgs e )
    {
      System.Diagnostics.Debug.WriteLine(
        $"Slider IsEnabled => {m_normalisationValueSlider.IsEnabled}"
      ) ;
    }
  }

}
