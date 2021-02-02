//
// IntensityMapProfileDisplaySettings.cs
//

using System.Transactions;

namespace IntensityMapViewer
{

  //
  // Hmm, need to connect with integer values also ...
  //

  public class NumericValueViewModel
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  {

    private string m_displayName = "Value" ;
    public string DisplayName
    {
      get => m_displayName ;
      set => SetProperty(
        ref m_displayName,
        value
      ) ;
    }
    
    public double MaxValue => 1.0 ;

    public double MinValue => 0.0 ;

    private double m_currentValue = double.NaN ;
    public double CurrentValue 
    {
      get => m_currentValue ;
      set {
        if (
          SetProperty(
            ref m_currentValue,
            value
          )
        ) {
          base.OnPropertyChanged(nameof(CurrentValueAsString)) ;
          ValueChanged?.Invoke() ;
        }
      }
    }

    public string CurrentValueAsString
    {
      get => m_currentValue.ToString() ;
      set => CurrentValue = double.Parse(value) ;
    }

    public event System.Action? ValueChanged ;


  }

}
