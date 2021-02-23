using Common;
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

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.IImagePresentationSettingsViewModel), 
      typeof(ImagePresentationSettings_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityMapViewer.IImagePresentationSettingsViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityMapViewer.IImagePresentationSettingsViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public ImagePresentationSettings_UserControl ( )
    {
      this.InitializeComponent();
      ColourMapBindingHelper = new(
        (value) => ViewModel.ColourMapOption = value,
        (value) => $"Show {value}"
      ) ;
      NormalisationModeBindingHelper = new(
        (value) => ViewModel.NormalisationMode = value
      ) ;
      NormalisationValueBindingHelper = new(
        (byteValue) => byteValue,
        (doubleValue) => ViewModel.SetNormalisationValue(
          (byte) doubleValue
        ) 
      ) {
        Minimum = 0.0,
        Maximum = 255.0
      } ;
    }

    public EnumBindingHelper<IntensityMapViewer.ColourMapOption> ColourMapBindingHelper { get ; }

    public EnumBindingHelper<IntensityMapViewer.NormalisationMode> NormalisationModeBindingHelper { get ; }

    public SliderValueBindingHelper<byte> NormalisationValueBindingHelper { get ; }

    public double GetNormalisationValue ( byte value ) => value ;
    
    public void SetNormalisationValue ( double value )
    {
      ViewModel.SetNormalisationValue(
        (byte) value 
      ) ;
    }

    private void m_normalisationValueSlider_ValueChanged ( object sender, RangeBaseValueChangedEventArgs e )
    {
      ViewModel.SetNormalisationValue(
        (byte) e.NewValue 
      ) ;
    }

  }

}
