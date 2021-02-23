//
// EnumBindingHelper.cs
//
 
using System.Collections.Generic ;
using System.Linq ;

namespace NativeUwp_ViewerApp_01
{

  public class EnumBindingHelper<T> where T : System.Enum
  {

    private System.Action<T> m_valueChanged ;
    
    private System.Func<T,string> m_valueToStringFunc ;

    private IEnumerable<T> m_options ;

    public EnumBindingHelper ( 
      System.Action<T>       valueChanged, 
      System.Func<T,string>? valueToString = null 
    ) {
      m_valueChanged = valueChanged ;
      m_valueToStringFunc = valueToString ?? ( (value) => value.ToString() ) ;
      List<T> options = new List<T>() ;
      foreach ( 
        T option in System.Enum.GetValues(
          typeof(T)
        )
      ) {
        options.Add(option) ;
      }
      m_options = options ;
    }

    public IEnumerable<string> OptionNames 
    => m_options.Select(
      value => m_valueToStringFunc(value)
    ) ;

    public string GetOptionName ( T option ) 
    => m_valueToStringFunc(option) ;

    public T GetOptionFromName ( string optionName ) 
    => m_options.Single(
      option => m_valueToStringFunc(option) == optionName
    ) ;

    public void SetOptionFromName ( object optionName )
    {
      m_valueChanged(
        GetOptionFromName(
          optionName as string
        )
      ) ;
    }

  }

}
