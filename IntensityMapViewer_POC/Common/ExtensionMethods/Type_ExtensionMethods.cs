//
// Type_ExtensionMethods.cs
//

using System.Linq ;

namespace Common.ExtensionMethods
{

  public static partial class Type_ExtensionMethods
  {

    public static void ForEachEnumerableValue<T> (
      this System.Type enumType,
      System.Action<T> action
    )
    where T : System.Enum
    {
      System.Array enumValues = System.Enum.GetValues(
        typeof(T)
      ) ;
      foreach ( T enumValue in enumValues.Cast<T>() )
      {
        action(enumValue) ;
      }
    }

  }

}
