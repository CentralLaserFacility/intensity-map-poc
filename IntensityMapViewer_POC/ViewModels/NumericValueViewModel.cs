//
// IntensityMapProfileDisplaySettings.cs
//

using Common.ExtensionMethods ;

namespace IntensityMapViewer
{

  //
  // Hmm, need an integer version of this ...
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

    public string ToolTipInfo 
    // => $"Valid range :\nMinimum {MinValue}\nMaximum {MaxValue}" ;
    => $"Valid range is {MinValue} to {MaxValue}" ;
    
    private bool m_mostRecentSetAttemptFailed = false ;
    public bool MostRecentSetAttemptFailed
    {
      get => m_mostRecentSetAttemptFailed ;
      private set => SetProperty(
        ref m_mostRecentSetAttemptFailed,
        value
      ) ;
    }
    
    private double m_minValue = 0.0 ;
    public double MinValue
    {
      get => m_minValue ;
      set => SetProperty(
        ref m_minValue,
        value
      ).WithActionOnTrueValue(
        () => {
          OnPropertyChanged(nameof(StepDelta)) ;
          OnPropertyChanged(nameof(ToolTipInfo)) ;
          SetCurrentValue(CurrentValue) ;
        }
      ) ;
    }

    private double m_maxValue = 0.0 ;
    public double MaxValue
    {
      get => m_maxValue ;
      set => SetProperty(
        ref m_maxValue,
        value
      ).WithActionOnTrueValue(
        () => {
          OnPropertyChanged(nameof(StepDelta)) ;
          OnPropertyChanged(nameof(ToolTipInfo)) ;
          SetCurrentValue(CurrentValue) ;
        }
      ) ;
    }

    public int m_nSteps = 50 ;
    public int NSteps 
    {
      get => m_nSteps ;
      set => SetProperty(
        ref m_nSteps,
        value
      ).WithActionOnTrueValue(
        () => {
          OnPropertyChanged(nameof(StepDelta)) ;
        }
      ) ;
    }
    public double StepDelta => ( MaxValue - MinValue ) / NSteps ;

    private double m_currentValue = 0.0 ;
    public double CurrentValue 
    {
      get => m_currentValue ;
      set => SetCurrentValue(value) ;
    }

    public void SetCurrentValue ( double value )
    {
      if ( 
         value >= MinValue 
      && value <= MaxValue
      ) {
        m_currentValue = value ;
        OnPropertyChanged(nameof(CurrentValue)) ;
        OnPropertyChanged(nameof(CurrentValueAsString)) ;
        MostRecentSetAttemptFailed = false ;
        // Common.DebugHelpers.WriteDebugLines($"Numeric value => {value}") ;
        ValueChanged?.Invoke() ;
      }
      else
      {
        MostRecentSetAttemptFailed = true ;
      }
    }

    public void OnCurrentValueChangedExternally ( double value )
    {
      m_currentValue = value ;
      OnPropertyChanged(nameof(CurrentValue)) ;
      OnPropertyChanged(nameof(CurrentValueAsString)) ;
    }

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

    public System.Action? ValueChanged { get ; set ; }

  }

}
