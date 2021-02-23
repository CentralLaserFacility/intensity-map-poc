//
// SliderValueBindingHelper.cs
//

namespace NativeUwp_ViewerApp_01
{

  public class SliderValueBindingHelper<TActualValue> 
  {

    private System.Func<TActualValue,double> m_getActualValueAsDouble ;
    private System.Action<double>            m_setActualValueFromDouble ;

    public SliderValueBindingHelper (
      System.Func<TActualValue,double> getActualValueAsDouble,
      System.Action<double>            setActualValueFromDouble
    ) {
      m_getActualValueAsDouble   = getActualValueAsDouble ;
      m_setActualValueFromDouble = setActualValueFromDouble ;
    }

    public double Minimum { get ; init ; }

    public double Maximum { get ; init ; }

    public double GetValue ( TActualValue actualValue )
    => m_getActualValueAsDouble(actualValue) ;

    public void SetValue ( double value )
    => m_setActualValueFromDouble(value) ;

  }

}
