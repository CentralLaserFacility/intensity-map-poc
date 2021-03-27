//
// StringBindingHelper.cs
//

using System.Collections.Generic;
using System.Linq;

namespace Common
{

  public class StringBindingHelper<T> 
  {

    private readonly IEnumerable<T> m_options ;

    private readonly System.Func<T,string> m_valueToStringFunc ;

    private readonly System.Action<T> m_valueChanged ;
    
    public StringBindingHelper ( 
      IEnumerable<T>         options,
      System.Action<T>       valueChanged, 
      System.Func<T,string>? valueToString = null 
    ) {
      m_valueChanged = valueChanged ;
      m_valueToStringFunc = valueToString ?? ( (value) => value.ToString() ) ;
      m_options = options.ToList() ;
      m_valueChanged(
        options.First()
      ) ;
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
