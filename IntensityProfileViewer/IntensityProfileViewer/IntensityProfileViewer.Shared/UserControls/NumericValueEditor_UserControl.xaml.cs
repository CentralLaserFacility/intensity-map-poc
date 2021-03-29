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

namespace IntensityProfileViewer
{

  public sealed partial class NumericValueEditor_UserControl : UserControl
  {

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.NumericValueViewModel), 
      typeof(NumericValueEditor_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityProfileViewer.NumericValueViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityProfileViewer.NumericValueViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public NumericValueEditor_UserControl ( )
    {
      this.InitializeComponent() ;
    }

    // Helpers that must be specified in some forms of two-way x:Bind ...

    public double GetDoubleValue ( double value ) => value ;

    public string GetStringValue ( string value ) => value ;

    public Brush GetErrorIndicationBrush ( bool error )
    => (
      error
      // ? new SolidColorBrush(Windows.UI.Colors.Pink)
      // : new SolidColorBrush(Windows.UI.Colors.LightGreen)
      // Matteo : this fails to find the resources !!!
      // ? this.Resources["Brush_Pink"] as Brush
      // : this.Resources["Brush_LightGreen"] as Brush
      // THIS WORKS ...
      ? Application.Current.Resources["NumericEditorInValidValueBrush"] as Brush
      : Application.Current.Resources["NumericEditorValidValueBrush"] as Brush
    ) ;

  }

}
