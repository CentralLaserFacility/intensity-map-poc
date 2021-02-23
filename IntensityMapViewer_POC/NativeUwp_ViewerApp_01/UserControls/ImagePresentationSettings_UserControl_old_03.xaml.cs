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

  public sealed partial class ImagePresentationSettings_UserControl_old_03 : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.IImagePresentationSettingsViewModel), 
      typeof(ImagePresentationSettings_UserControl_old_03), 
      new PropertyMetadata(0)
    ) ;

    public IntensityMapViewer.IImagePresentationSettingsViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityMapViewer.IImagePresentationSettingsViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }


    public ImagePresentationSettings_UserControl_old_03()
    {
      this.InitializeComponent();
      ColourMapBindingHelper = new(
        (value) => ViewModel.ColourMapOption = value,
        (value) => $"Option {value}"
      ) ;
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

    public EnumBindingHelper_old<IntensityMapViewer.ColourMapOption> ColourMapBindingHelper { get ; }

  }

  public class EnumBindingHelper_old<T>
  {

    private System.Action<T> m_valueChanged ;
    
    private System.Func<T,string> m_valueToStringFunc ;

    private IEnumerable<T> m_options ;

    public EnumBindingHelper_old ( 
      System.Action<T>       valueChanged, 
      System.Func<T,string>? valueToString = null 
    ) {
      m_valueChanged = valueChanged ;
      m_valueToStringFunc = valueToString ?? ( (value) => value.ToString() ) ;
      List<T> options = new List<T>() ;
      foreach ( 
        T option in System.Enum.GetValues(
          typeof(T)
        )
      ) {
        options.Add(option) ;
      }
      m_options = options ;
    }

    public IEnumerable<string> OptionNames 
    => m_options.Select(
      value => m_valueToStringFunc(value)
    ) ;

    public string GetOptionName ( T option ) 
    => m_valueToStringFunc(option) ;

    public T GetOptionFromName ( string optionName ) 
    => m_options.Single(
      option => m_valueToStringFunc(option) == optionName
    ) ;

    public void SetOptionFromName ( object optionName )
    {
      m_valueChanged(
        GetOptionFromName(
          optionName as string
        )
      ) ;
    }

  }

}
