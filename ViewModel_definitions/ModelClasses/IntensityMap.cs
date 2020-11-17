//
// IntensityMap.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace IntensityMapViewer
{

  public partial class IntensityMap : IIntensityMap
  {

    public System.DateTime TimeStamp => throw new System.NotImplementedException() ;

    public System.Drawing.Size Dimensions => throw new System.NotImplementedException() ;

    public IReadOnlyList<byte> IntensityValues => throw new System.NotImplementedException() ;

    public byte GetIntensityValueAt ( int xAcross, int yDown ) => throw new System.NotImplementedException() ;

    public IReadOnlyList<byte> VerticalSliceAtColumn ( int xAcross ) => throw new System.NotImplementedException() ;

    public IReadOnlyList<byte> HorizontalSliceAtRow ( int yDown ) => throw new System.NotImplementedException() ;

    public byte MinimumIntensityValue => throw new System.NotImplementedException() ;

    public byte MaximumIntensityValue => throw new System.NotImplementedException() ;

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
      IEnumerable<byte>   bytes, 
      System.DateTime     timeStamp 
    ) {
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

    // The files referred to here would be packaged as embedded resources ??

    public static System.Collections.Generic.IEnumerable<string> AvailableIntensityMapFileNames = new[]{"fileA.pgm"} ;

    // Helper function to create a noisy version that looks somewhat different on screen

    static IntensityMap CreateCloneWithAddedRandomNoise ( IntensityMap source, byte noiseAmplitude )
    {
      throw new System.NotImplementedException() ;
    }

  }

}
