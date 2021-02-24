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
    
    private bool m_mostRecentSetAttemptFailed = false ;
    public bool MostRecentSetAttemptFailed
    {
      get => m_mostRecentSetAttemptFailed ;
      private set => SetProperty(
        ref m_mostRecentSetAttemptFailed,
        value
      ) ;
    }
    
    public double MinValue { get ; set ; } = 0.0 ;

    public double MaxValue { get ; set ; } = 1.0 ;

    public int NSteps { get ; set ; } = 50 ;

    public double StepDelta => ( MaxValue - MinValue ) / NSteps ;

    public double CurrentValue { get ; private set ; } = 0.0 ;

    public void SetCurrentValue ( double value )
    {
      if ( 
         value >= MinValue 
      && value <= MaxValue
      ) {
        CurrentValue = value ;
        OnPropertyChanged(nameof(CurrentValue)) ;
        OnPropertyChanged(nameof(CurrentValueAsString)) ;
        MostRecentSetAttemptFailed = false ;
        Common.DebugHelpers.WriteDebugLines($"Set to {value}") ;
        ValueChanged?.Invoke() ;
      }
      else
      {
        MostRecentSetAttemptFailed = true ;
      }
    }

    // public string CurrentValueAsString => CurrentValue.ToString() ;

    public string CurrentValueAsString 
    {
      get => CurrentValue.ToString() ;
      set => SetCurrentValueFromString(value) ;
    }

    public void SetCurrentValueFromString ( string value )
    {
      if (
        double.TryParse(
          value,
          out var parsedValue
        )
      ) {
        SetCurrentValue(
          parsedValue
        ) ;
      }
      else
      {
        MostRecentSetAttemptFailed = true ;
      }
    }

    public event System.Action? ValueChanged ;

  }

}
