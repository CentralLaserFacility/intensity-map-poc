//
// GraphicsHelpers.cs
//

using System.Collections;
using System.Collections.Generic;

namespace Common
{

  public static class GraphicsHelpers
  {

    public static void ForEachPixel (
      int                        nX,
      int                        nY,
      System.Action<int,int,int> action
    ) {
      int i = 0 ;
      for ( int y = 0 ; y < nY ; y++ )
      {
        for ( int x = 0 ; x < nX ; x++ )
        {
          action(x,y,i) ;
          i++ ;
        }
      }
    }

    public static void ForEachPixel (
      int                    nX,
      int                    nY,
      System.Action<int,int> action
    ) {
      for ( int y = 0 ; y < nY ; y++ )
      {
        for ( int x = 0 ; x < nX ; x++ )
        {
          action(x,y) ;
        }
      }
    }

    public static IEnumerable<T> ForEachPixel<T> (
      int                    nX,
      int                    nY,
      System.Func<int,int,T> pixelValueFunc
    ) {
      for ( int y = 0 ; y < nY ; y++ )
      {
        for ( int x = 0 ; x < nX ; x++ )
        {
          yield return pixelValueFunc(x,y) ;
        }
      }
    }

  }

}
