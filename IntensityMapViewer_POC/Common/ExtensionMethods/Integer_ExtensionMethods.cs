//
// Integer_ExtensionMethods.cs
//

namespace Common.ExtensionMethods
{

  public static partial class Integer_ExtensionMethods
  {

    public static void Repeat ( this int n, System.Action action )
    {
      for ( int i = 0 ; i < n ; i++ )
      {
        action() ;
      }
    }

    public static void Repeat ( this int n, System.Action<int> action, int initialIndex = 0 )
    {
      for ( int i = 0 ; i < n ; i++ )
      {
        action(
          i + initialIndex
        ) ;
      }
    }

  }

}
