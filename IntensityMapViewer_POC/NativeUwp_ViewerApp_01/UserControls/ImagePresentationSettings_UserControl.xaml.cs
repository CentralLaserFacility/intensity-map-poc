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


    public ImagePresentationSettings_UserControl()
    {
      this.InitializeComponent();
      DataContextChanged += (s,e) => {
        System.Diagnostics.Debug.WriteLine(
          $"{this.GetType()} DataContext => {DataContext?.GetType().ToString()??"null"}"
        ) ;
      } ;
    }

    public List<IntensityMapViewer.ColourMapOption> GetColourMapOptions ( ) 
    {
      List<IntensityMapViewer.ColourMapOption> options = new List<IntensityMapViewer.ColourMapOption>() ;
      foreach ( 
        IntensityMapViewer.ColourMapOption option in System.Enum.GetValues(
          typeof(IntensityMapViewer.ColourMapOption)
        )
      ) {
        options.Add(option) ;
      }
      return options ;
    }

    public IEnumerable<string> GetColourMapOptionNames ( ) 
    {
      return GetColourMapOptions().Select(
        value => GetColourMapOptionName(value)
      ) ;
    }

    public string GetColourMapOptionName ( IntensityMapViewer.ColourMapOption colourMapOption ) 
    => colourMapOption.ToString() ;

    public IntensityMapViewer.ColourMapOption GetColourMapOptionFromName ( string colourMapOptionName ) 
    => GetColourMapOptions().Single(
      option => GetColourMapOptionName(option) == colourMapOptionName
    ) ;

    public void SetColourMapOptionFromName ( object colourMapOptionName )
    {
      ViewModel.ColourMapOption = GetColourMapOptionFromName(
        colourMapOptionName as string
      ) ;
    }

  }

}
