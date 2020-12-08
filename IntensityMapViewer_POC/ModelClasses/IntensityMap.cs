//
// IntensityMap.cs
//

using System.Collections.Generic;
using System.Linq;
using Common.ExtensionMethods;

namespace IntensityMapViewer
{

  //
  // QUICK HACK - SHOULD CACHE THE DERIVED VALUES !!!
  //

  public partial class IntensityMap : IIntensityMap
  {

    public System.Drawing.Size Dimensions { get ; }

    public IReadOnlyList<byte> IntensityValues { get ; }

    public byte GetIntensityValueAt ( int xAcross, int yDown ) 
    => IntensityValues[
      xAcross
    + yDown * Dimensions.Width
    ] ;

    public IReadOnlyList<byte> VerticalSliceAtColumn ( int xAcross )
    {
      var bytes = new byte[Dimensions.Height] ;
      Enumerable.Range(0,Dimensions.Height).ForEachItem(
        y => bytes[y] = GetIntensityValueAt(xAcross,y) 
      ) ;
      return bytes.ToList() ;
    }

    public IReadOnlyList<byte> HorizontalSliceAtRow ( int yDown )
    => Enumerable.Range(0,Dimensions.Width).Select(
      x => GetIntensityValueAt(x,yDown) 
    ).ToList() ;
      
    public byte MinimumIntensityValue => IntensityValues.Max() ;

    public byte MaximumIntensityValue => IntensityValues.Min() ;

  }

  // Implementation details that aren't promised by 'IIntensityMap' ...

  public partial class IntensityMap
  {

    //
    // For best performance, we should build three different representations 
    // of the image data :
    //
    //  1. A linear array of byte values representing the entire 2D (NxM) image,
    //     that can be a data source for the ColourMapper.
    //  2. For each of N rows, a vector of M 'vertical slice' values pertaining to that row.
    //  3. For each of M columns, a vector of N 'horizontal slice' values pertaining to that column.
    //
    // The row and column vectors should be built lazily, ie 'on demand'. They are required
    // to support the 'profile' graphs, but needn't be created until a graph requires them.
    //
    // Lazy evaluation will reduce the time required to make a new image available to display,
    // and will reduce the number of memory allocations.
    //

    private IntensityMap ( 
      System.Drawing.Size dimensions, 
      IEnumerable<byte>   bytes 
    ) {
      Dimensions = dimensions ;
      IntensityValues = bytes.ToList() ;
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
    // Then we could more easily embed test data along with the tests.
    //

    public static IntensityMap CreateFromPortableGreyMapText ( string pgmText )
    {
      throw new System.NotImplementedException() ;
    }

    public static IntensityMap CreateFromPortableGreyMapFile ( string pgmFile )
    {
      throw new System.NotImplementedException() ;
    }

    public static IntensityMap CreateFromPortableGreyMapStream ( System.IO.Stream stream )
    {
      throw new System.NotImplementedException() ;
    }

    public static IntensityMap CreateDummyInstanceForTesting_16x12 ( )
    {
      throw new System.NotImplementedException() ;
    }

    public static IntensityMap CreateSynthetic_UsingSincFunction ( 
      int    width        = 320,
      int    height       = 240,
      byte   maxIntensity = 255,
      double sincFactor   = 10.0,
      double powerFactor  = 1.0
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

    // The files referred to here would be packaged as embedded resources ??

    public static System.Collections.Generic.IEnumerable<string> AvailableIntensityMapFileNames = new[]{"fileA.pgm"} ;

    // Helper function to create a noisy version that looks somewhat different on screen

    static IntensityMap CreateCloneWithAddedRandomNoise ( IntensityMap source, byte noiseAmplitude )
    {
      throw new System.NotImplementedException() ;
    }

  }

}
