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

  public sealed partial class ImagePresentationSettings_UserControl_2 : UserControl
  {

    // private IntensityMapViewer.IImagePresentationSettingsViewModel ViewModel => DataContext as IntensityMapViewer.IImagePresentationSettingsViewModel ;

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

    // private IntensityMapViewer.IDisplayPanelViewModel RootViewModel ;

    // public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
    //   nameof(ViewModel), 
    //   typeof(IntensityMapViewer.IImagePresentationSettingsViewModel), 
    //   typeof(ImagePresentationSettings_UserControl), 
    //   new PropertyMetadata(
    //     null, 
    //     OnViewModelChanged
    //   )
    // ) ;
    // 
    // public IntensityMapViewer.IImagePresentationSettingsViewModel ViewModel
    // {
    //   get => (IntensityMapViewer.IImagePresentationSettingsViewModel) GetValue(ViewModelProperty) ;
    //   set => SetValue(ViewModelProperty,value) ;
    // }
    // 
    // private static void OnViewModelChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    // {
    //   var viewModel = d as IntensityMapViewer.IImagePresentationSettingsViewModel ;
    //   if ( viewModel != null )
    //   {
    //   }
    // }

    public ImagePresentationSettings_UserControl_2()
    {
      this.InitializeComponent();
      DataContextChanged += (s,e) => {
        System.Diagnostics.Debug.WriteLine(
          $"{this.GetType()} DataContext => {DataContext?.GetType().ToString()??"null"}"
        ) ;
        // if ( ViewModel != null ) 
        // {
        //   // Yikes - gotta call this explicitly ? WTF !!
        //   // Hmm, if this is not called initially, most things work,
        //   // except for the binding to 'IsEnabled'.
        //   // BUT - even when this is called :
        //   // 1. Exceptions get thrown (to do with combo box values being null) CURRENTLY DISABLED !!
        //   // 2. Consequently the displayed combo box values are empty
        //   this.Bindings.Update() ; 
        // }
      } ;
    }

    // ----------------------------------------

    public System.Collections.Generic.IEnumerable<string> ColourMapOptionNames { get ; } 
     = System.Enum.GetNames(typeof(IntensityMapViewer.ColourMapOption)) ;

    // public string ColourMapOptionName => ViewModel.ColourMapOption.ToString() ;

    public string GetColourMapOptionName ( IntensityMapViewer.ColourMapOption colourMapOption ) => colourMapOption.ToString() ;

    public void SetColourMapOptionFromName ( object name )
    => ViewModel.ColourMapOption = (IntensityMapViewer.ColourMapOption) System.Enum.Parse(
      typeof(IntensityMapViewer.ColourMapOption),
      name as string
    ) ;

    // ------------------------------

    public IEnumerable<
      Common.EnumValueDescriptor<IntensityMapViewer.ColourMapOption>
    > ColourMapOptionDescriptors { get ; }
    = Common.EnumValueDescriptor<
      IntensityMapViewer.ColourMapOption
    >.EnumValueDescriptors ;

    // ----------------------------------------

    // Hopefully these helpers can be moved to a Utilities class ...

    public System.Collections.Generic.IEnumerable<string> GetOptionNamesForType ( 
      object enumInstance
    ) => System.Enum.GetNames(
      enumInstance.GetType()
    ) ;

    public System.Collections.Generic.IEnumerable<string> GetOptionNamesForEnumType ( 
      System.Type enumType
    ) => System.Enum.GetNames(
      enumType
    ) ;

    public void SetColourMapOptionFromName2 ( object name, object other )
    => ViewModel.ColourMapOption = (IntensityMapViewer.ColourMapOption) System.Enum.Parse(
      typeof(IntensityMapViewer.ColourMapOption),
      name as string
    ) ;

    public IntensityMapViewer.ColourMapOption GetColourMapOptionFromName ( object name )
    => (IntensityMapViewer.ColourMapOption) System.Enum.Parse(
      typeof(IntensityMapViewer.ColourMapOption),
      name as string
    ) ;

    // ----------------------------------------

    public List<
      Common.EnumItemsSource<IntensityMapViewer.ColourMapOption>
    > ColourMapOptions { get ; }
    = Common.EnumItemsSource<IntensityMapViewer.ColourMapOption>.ToList() ;

    // public CollectionViewSource ColourMapOptions { get ; }
    // = new CollectionViewSource(){
    //   Source = Common.EnumItemsSource<IntensityMapViewer.ColourMapOption>.ToList() 
    // } ;

    // ------------------------------------------

    public List<Common.EnumItemsSource<IntensityMapViewer.NormalisationMode>> NormalisationModeOptions 
    => Common.EnumItemsSource<IntensityMapViewer.NormalisationMode>.ToList() ;

    private void Slider_ValueChanged ( object sender, RangeBaseValueChangedEventArgs e )
    {
      ViewModel.SetNormalisationValue(
        (byte) m_normalisationValueSlider.Value
      ) ;
    }

    // private void m_normalisationValueSlider_IsEnabledChanged ( object sender, DependencyPropertyChangedEventArgs e )
    // {
    //   System.Diagnostics.Debug.WriteLine(
    //     $"Slider IsEnabled => {m_normalisationValueSlider.IsEnabled}"
    //   ) ;
    // }

  }

}
