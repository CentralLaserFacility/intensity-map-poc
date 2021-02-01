//
// IntensityMapProfileDisplaySettings.cs
//

namespace IntensityMapViewer
{
  public class NumericValueViewModel
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  {

    public double MaxValue => 1.0 ;

    public double MinValue => 0.0 ;

    private double m_currentValue = double.NaN ;
    public double CurrentValue 
    {
      get => m_currentValue ;
      set => SetProperty(
        ref m_currentValue,
        value
      ) ;
    }

  }

}
