//
// IntensityMap.cs
//

using Common.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;

namespace IntensityMapViewer
{

  public partial class IntensityMap : IIntensityMap
  {

    public System.Drawing.Size Dimensions { get ; }

    public IReadOnlyList<byte> IntensityValues { get ; }

    public byte GetIntensityValueAt ( int xAcross, int yDown ) 
    => IntensityValues[
      xAcross
    + yDown * Dimensions.Width
    ] ;

    //
    // These 'derived' properties are lazily evaluated and cached
    //

    private IReadOnlyList<byte>[]? m_verticalSlices ;

    public IReadOnlyList<byte> VerticalSliceAtColumn ( int xAcross )
    {
      m_verticalSlices ??= new IReadOnlyList<byte>[Dimensions.Width] ;
      if ( m_verticalSlices[xAcross] is null )
      {
        m_verticalSlices[xAcross] = Enumerable.Range(
          0,
          Dimensions.Height
        ).Select(
          y => GetIntensityValueAt(xAcross,y) 
        ).ToList() ;
      }
      return m_verticalSlices[xAcross] ;
    }

    private IReadOnlyList<byte>[]? m_horizontalSlices ;

    public IReadOnlyList<byte> HorizontalSliceAtRow ( int yDown )
    {
      m_horizontalSlices ??= new IReadOnlyList<byte>[Dimensions.Height] ;
      if ( m_horizontalSlices[yDown] is null )
      {
        m_horizontalSlices[yDown] = Enumerable.Range(
          0,
          Dimensions.Width
        ).Select(
          x => GetIntensityValueAt(x,yDown) 
        ).ToList() ;
      }
      return m_horizontalSlices[yDown] ;
    }
      
    private byte? m_minimumIntensityValue ;

    public byte MinimumIntensityValue => m_minimumIntensityValue ??= IntensityValues.Max() ;

    private byte? m_maximumIntensityValue ;

    public byte MaximumIntensityValue => m_maximumIntensityValue ??= IntensityValues.Min() ;

  }

  // Implementation details that aren't promised by 'IIntensityMap' ...

  public partial class IntensityMap
  {

    //
    // For best performance, we build three different representations 
    // of the image data :
    //
    //  1. A linear array of byte values representing the entire 2D image,
    //     that can be a data source for the ColourMapper.
    //  2. For each of N rows, a vector of M 'vertical slice' values pertaining to that row.
    //  3. For each of M columns, a vector of N 'horizontal slice' values pertaining to that column.
    //
    // The row and column vectors are built lazily, ie 'on demand'. They are required
    // to support the 'profile' graphs, but needn't be created until a graph requires them.
    //
    // Lazy evaluation reduces the time required to make a new image available to display,
    // and will minimise the number of memory allocations. This will be important when
    // we're acquiring new images in real time.
    //

    public IntensityMap ( 
      System.Drawing.Size dimensions, 
      IEnumerable<byte>   intensityValues 
    ) {
      Dimensions = dimensions ;
      IntensityValues = intensityValues.ToList() ;
    }

    // Create a 'noisy' version of an existing instance,
    // that will look somewhat different on screen.

    private static System.Random? g_randomGenerator ; // = new() ;

    public IntensityMap CreateCloneWithAddedRandomNoise ( 
      byte           noiseAmplitude,
      System.Random? randomGenerator = null
    ) {
      randomGenerator ??= (
        g_randomGenerator ??= new()
      ) ;
      return new IntensityMap(
        Dimensions,
        IntensityValues.Select(
          value => Constrain_0_255(
            value 
          + randomGenerator.Next(
            -noiseAmplitude,
            +noiseAmplitude
            )
          )
        ).ToArray() 
      ) ;
      // We don't want the added noise to make the
      // value 'wrap around', so let's constrain it
      static byte Constrain_0_255 ( double value )
      => (byte) (
        value switch {
        < 0.0   => 0.0,
        > 255.0 => 255.0,
        _       => value
        }
      ) ;
    }

  }

}
