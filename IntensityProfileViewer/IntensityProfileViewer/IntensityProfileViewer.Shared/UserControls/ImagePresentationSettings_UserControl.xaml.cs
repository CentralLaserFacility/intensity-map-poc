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

namespace IntensityProfileViewer
{

  public sealed partial class ImagePresentationSettings_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.IImagePresentationSettingsViewModel), 
      typeof(ImagePresentationSettings_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityProfileViewer.IImagePresentationSettingsViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityProfileViewer.IImagePresentationSettingsViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public ImagePresentationSettings_UserControl ( )
    {
      this.InitializeComponent();
      ColourMapBindingHelper = new(
        valueChanged  : (value) => ViewModel.ColourMapOption = value,
        valueToString : (value) => $"Show {value}"
      ) ;
      NormalisationModeBindingHelper = new(
        (value) => ViewModel.NormalisationMode = value
      ) ;
      NormalisationValueBindingHelper = new(
        getActualValueAsDouble   : (byteValue) => byteValue,
        setActualValueFromDouble : (doubleValue) => {
          // In normal operation, the slider will be disabled if we're
          // in Automatic mode, so this action will never be triggered.
          // However, during the initialisation of the Slider control,
          // in a 'Droid' build, this action *does* get invoked, with 
          // a value corresponsing to the slider 'Minimum'. So we need
          // to protect against the value being propagated to the ViewModel
          // in this particular case.
          if ( ViewModel.NormalisationMode == NormalisationMode.Manual )
          {
            ViewModel.SetNormalisationValue(
              (byte) doubleValue
            ) ;
          }
        }
      ) {
        Minimum = 8.0,
        Maximum = 255.0
      } ;
    }

    public EnumBindingHelper<IntensityProfileViewer.ColourMapOption> ColourMapBindingHelper { get ; }

    public EnumBindingHelper<IntensityProfileViewer.NormalisationMode> NormalisationModeBindingHelper { get ; }

    public SliderValueBindingHelper<byte> NormalisationValueBindingHelper { get ; }

    public string GetNormalisationValueHeaderText ( byte value )
    => $"Normalisation value ({value})" ;

    // public double GetNormalisationValue ( byte value ) => value ;
    
    // public void SetNormalisationValue ( double value )
    // {
    //   ViewModel.SetNormalisationValue(
    //     (byte) value 
    //   ) ;
    // }

    // private void NormalisationValueSlider_ValueChanged ( object sender, RangeBaseValueChangedEventArgs e )
    // {
    //   ViewModel.SetNormalisationValue(
    //     (byte) e.NewValue 
    //   ) ;
    // }

  }

}
