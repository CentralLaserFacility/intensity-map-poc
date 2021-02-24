using IntensityMapViewer;
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

  public sealed partial class ProfileDisplaySettings_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.IProfileDisplaySettingsViewModel), 
      typeof(ProfileDisplaySettings_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityMapViewer.IProfileDisplaySettingsViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityMapViewer.IProfileDisplaySettingsViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public ProfileDisplaySettings_UserControl()
    {
      this.InitializeComponent() ;
      XPositionViewModel = new() {
        DisplayName = "X position (%)",
        MinValue    = 0.0,
        MaxValue    = 100.0
      } ;
      XPositionViewModel = new() {
        DisplayName = "Y position (%)",
        MinValue    = 0.0,
        MaxValue    = 100.0
      } ;
    }

    private NumericValueViewModel XPositionViewModel ;

    private NumericValueViewModel YPositionViewModel ;

  }

}
