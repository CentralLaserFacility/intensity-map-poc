//
// Helpers.cs
//

using System.Collections.Generic ;
using System.Linq ;
using System.Diagnostics.CodeAnalysis ;

using Common.ExtensionMethods ;

namespace Common
{

  public static class Helpers
  {

    public static void Swap<T> ( ref T a, ref T b )
    {
      T tmp = b ;
      b = a ;
      a = tmp ;
    }

  }

}
