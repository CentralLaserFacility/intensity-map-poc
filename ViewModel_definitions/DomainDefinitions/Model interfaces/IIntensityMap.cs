//
// IIntensityMap.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace IntensityMapViewer
{

  //
  // Provides the raw data for the image display panel.
  //
  //
  // This provides an abstraction of the image data we'll acquire from a camera.
  // It defines the image pixels in a low level form that is platform-independent,
  // namely an ordered sequence of byte values plus the height and width.
  //
  // In the demo, the concrete class will load an image from a '.pgm' file or similar.
  // In the 'real' app, images will be acquired from an Epics PV.
  //

  public interface IIntensityMap
  {

    System.DateTime TimeStamp { get ; }

    System.Drawing.Size Dimensions { get ; }

    // The first byte represents the pixel at the top left, and subsequent bytes
    // scan along the top row and then step down, as if we're reading words on a page.
    // Each byte represents an unsigned greyscale value 0-255. 

    IReadOnlyList<byte> IntensityValues { get ; }

    public byte MinimumIntensityValue { get ; }

    public byte MaximumIntensityValue { get ; }

    // These properties could be implemented via extension methods,
    // but that would have performance implications as we'd be relying on
    // the 'IntensityBytes' property and would not have the opportunity
    // to optimise access to the rows and columns via caching.

    // Hmm, not as clear as a named function
    // byte this [ int xAcross, int yDown ] { get ; } 

    byte GetIntensityValueAt ( int xAcross, int yDown ) ;

    IReadOnlyList<byte> VerticalSliceAtColumn ( int xAcross ) ;

    IReadOnlyList<byte> HorizontalSliceAtRow ( int yDown ) ;

  }

  // namespace ExtensionMethods
  // {
  //   public static class IIntensityMap_ExtensionMethods
  //   {
  //     public static byte GetByteAt ( this IIntensityMap intensityMap, int x, int y )
  //     {
  //       throw new System.NotImplementedException() ;
  //     }
  //     public static IReadOnlyList<byte> HorizontalSliceAtRow ( this IIntensityMap intensityMap, int yRow )
  //     {
  //       throw new System.NotImplementedException() ;
  //     }
  //     public static IReadOnlyList<byte> VerticalSliceAtColumn ( this IIntensityMap intensityMap, int xColumn )
  //     {
  //       throw new System.NotImplementedException() ;
  //     }
  //   }
  // }

}
