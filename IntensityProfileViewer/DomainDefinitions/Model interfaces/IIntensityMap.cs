//
// IIntensityMap.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace IntensityProfileViewer
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
  // In the 'real' app, images will be acquired from a camera via an Epics PV.
  //

  public interface IIntensityMap
  {

    System.Drawing.Size Dimensions { get ; }

    // The first byte represents the pixel at the top left, and subsequent bytes
    // scan along the top row and then step down, as if we're reading words on a page.
    // Each byte represents an unsigned greyscale value 0-255. 

    IReadOnlyList<byte> IntensityValues { get ; }

    public byte MinimumIntensityValue { get ; }

    public byte MaximumIntensityValue { get ; }

    byte GetIntensityValueAt ( int xAcross, int yDown ) ;

    IReadOnlyList<byte> VerticalSliceAtColumn ( int xAcross ) ;

    IReadOnlyList<byte> HorizontalSliceAtRow ( int yDown ) ;

    // string AsString => $"IntensityMap {Dimensions}" ;

  }

}
