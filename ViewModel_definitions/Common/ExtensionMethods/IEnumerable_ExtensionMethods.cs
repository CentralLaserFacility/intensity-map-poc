//
// IEnumerable_ExtensionMethods.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace Common.ExtensionMethods
{

  public static class IEnumerable_ExtensionMethods
  {

    public static bool IsEmpty<T> ( this IEnumerable<T> sequence )
    => sequence.Any() is false ;

    public static void ForEachItem<T> ( this IEnumerable<T> sequence, System.Action<T> action )
    {
      foreach ( var item in sequence )
      {
        action(item) ;
      }
    }

    public static void ForEachItem<T> ( this IEnumerable<T> sequence, System.Action<T,int> action )
    {
      int index = 0 ;
      foreach ( var item in sequence )
      {
        action(
          item,
          index++
        ) ;
      }
    }

  }

}
