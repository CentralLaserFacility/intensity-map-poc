//
// NumericValueEditor_UserControl.cs
//

namespace IntensityProfileViewer
{

  public sealed partial class NumericValueEditor_UserControl : Windows.UI.Xaml.Controls.UserControl
  {

    public static readonly Windows.UI.Xaml.DependencyProperty ViewModelProperty = Windows.UI.Xaml.DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.NumericValueViewModel), 
      typeof(NumericValueEditor_UserControl), 
      new Windows.UI.Xaml.PropertyMetadata(0)
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

    public Windows.UI.Xaml.Media.Brush GetErrorIndicationBrush ( bool error )
    => (
      error
      // ? new SolidColorBrush(Windows.UI.Colors.Pink)
      // : new SolidColorBrush(Windows.UI.Colors.LightGreen)
      // Matteo : this fails to find the resources !!! THESE ARE LOCAL RESOURCES !!!!
      // ? this.Resources["Brush_Pink"] as Brush
      // : this.Resources["Brush_LightGreen"] as Brush
      // THIS WORKS ...
      ? Windows.UI.Xaml.Application.Current.Resources["NumericEditorInvalidValueBrush"] as Windows.UI.Xaml.Media.Brush // GLOBAL RESOURCE !!!
      : Windows.UI.Xaml.Application.Current.Resources["NumericEditorValidValueBrush"]   as Windows.UI.Xaml.Media.Brush
    ) ;

  }

}
