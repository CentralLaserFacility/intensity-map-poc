//
// IntensityMapHelpers.cs
//

using System.Collections.Generic;
using System.Linq;
using Common.ExtensionMethods;

namespace IntensityMapViewer
{

  public static class IntensityMapHelpers
  {
  
    public static IntensityMap CreateDummyInstanceForTesting_16x12 ( )
    {
      throw new System.NotImplementedException() ;
    }

    //
    // Greyscale image file formats
    // https://en.wikipedia.org/wiki/Netpbm
    // Portable Greymap - '.pgm'
    // http://web.cs.iastate.edu/~smkautz/cs227f11/examples/week11/pgm_files.pdf
    // https://people.math.sc.edu/Burkardt/data/pnm/pnm.html
    //

    //
    // Might be easiest to create from string representation ?
    // Then we could more easily embed test data along with tests.
    //

    public static IntensityMap CreateFromPortableGreyMapText ( System.IO.TextReader text )
    {
      throw new System.NotImplementedException() ;
    }

    // The files referred to here would be packaged as embedded resources ??

    public static System.Collections.Generic.IEnumerable<string> AvailableIntensityMapFileNames = new[]{"fileA.pgm"} ;

    public static IntensityMap CreateSynthetic_UsingSincFunction ( 
      int    width        = 320,
      int    height       = 240,
      double sincFactor   = 10.0,
      double powerFactor  = 1.0,
      byte   maxIntensity = 255
    ) {
      var imageBytes = new byte[width*height] ;
      int jFirstByteNotYetWritten = 0 ;
      Enumerable.Range(0,height).ForEachItem(
        y => {
          Enumerable.Range(0,width).ForEachItem(
            x => {
              imageBytes[
                jFirstByteNotYetWritten++
              ] = GetPixelValue(x,y) ;
            }
          ) ;
        }
      ) ;
      return new IntensityMap(
        new System.Drawing.Size(width,height),
        imageBytes
      ) ;
      byte GetPixelValue ( int x, int y )
      {
        // Sin(x)/x around the centre
        int dx = x - width / 2 ;
        int dy = y - height / 2 ;
        double dxFrac01 = 2.0 * dx / height ; // Deliberately !! To keep circular symmetry
        double dyFrac01 = 2.0 * dy / height ;
        double r = sincFactor * System.Math.Sqrt(
          dxFrac01 * dxFrac01
        + dyFrac01 * dyFrac01
        ) ;
        double h = System.Math.Abs(
          r == 0.0
          ? 1.0
          : System.Math.Sin(r) /  r
        ) ;
        byte greyValue = (byte) ( 
          maxIntensity 
        * System.Math.Pow(h,powerFactor) 
        ) ;
        return greyValue ;
      }
    }

    private static IEnumerable<double> SincFactorSequence ( double start, double end, double delta )
    {
      double value = start ;
      yield return value ;
      while ( true )
      {
        value += delta ;
        if ( 
           value < start 
        || value > end 
        ) {
          delta = -delta ;
        }      
        yield return value ;
      }
    }

    private static IEnumerator<double> SincFactorEnumerator = SincFactorSequence(6.0,12.0,1.0).GetEnumerator() ;

    public static IntensityMap CreateSynthetic_UsingSincFunction_Cyclic ( )
    {
      SincFactorEnumerator.MoveNext() ;
      double value = SincFactorEnumerator.Current ;
      return CreateSynthetic_UsingSincFunction(
        sincFactor : value
      ) ;
    }

  }

}
