//
// NumericValueEditor_UserControl.cs
//

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

namespace NativeUwp_ViewerApp_01
{

  public sealed partial class NumericValueEditor_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.NumericValueViewModel), 
      typeof(NumericValueEditor_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityMapViewer.NumericValueViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityMapViewer.NumericValueViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public NumericValueEditor_UserControl ( )
    {
      this.InitializeComponent() ;
    }

    public double GetDoubleValue ( double value ) => value ;

    public string GetStringValue ( string value ) => value ;

  }

}
