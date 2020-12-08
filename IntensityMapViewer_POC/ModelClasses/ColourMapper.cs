//
// ColourMapper.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace IntensityMapViewer
{

  public abstract class ColourMapper : IColourMapper 
  {

    public static IColourMapper InstanceFor ( ColourMapOption option ) 
    => option switch {
    ColourMapOption.GreyScale => ColourMapper_GreyScale.Instance,
    ColourMapOption.JetColours => ColourMapper_JetColours.Instance,
    _ => throw new System.ApplicationException()
    } ;

    public static uint EncodeARGB ( byte red, byte green, byte blue )
    => (uint) (
      ( 0xff  << 24 ) // A : most significant byte is 'alpha'
    | ( red   << 16 ) // R
    | ( green << 8  ) // G
    | ( blue  << 0  ) // B
    ) ;

    public IReadOnlyList<uint> MapByteValuesToEncodedARGB ( 
      IReadOnlyList<byte> byteValues,
      ColourMapOption     colourMapOption 
    ) {
      return byteValues.Select(
        b => MapIntensityToJet_EncodedAsARGB(b) 
      ).ToList() ;
    }

    private System.Func<byte,uint> m_pixelMappingFunc ;

    protected ColourMapper ( System.Func<byte,uint> pixelMappingFunc )
    {
      m_pixelMappingFunc = pixelMappingFunc ;
    }

    public static uint MapIntensityToGrey_EncodedAsARGB ( byte greyValue )
    {
      return EncodeARGB(
        greyValue,
        greyValue,
        greyValue
      ) ;
    }

    public static uint MapIntensityToJet_EncodedAsARGB ( byte greyValue )
    {
      //
      // Formula for the 'JET' mapping ...
      // https://stackoverflow.com/questions/7706339/grayscale-to-red-green-blue-matlab-jet-color-scale
      // The colour goes from Blue -> Cyan -> Green -> Yellow -> Red.
      // It basically represents a walk along the edges of the 'RGB color cube' 
      // starting at Blue and ending at Red, interpolating the values along this path.
      double r ;
      double g ;
      double b ;
      double v = greyValue / 255.0 ;
      if ( v < 0.25 ) 
      {
        r = 0.0 ;
        g = 4.0 * v ;
        b = 1.0 ;
      } 
      else if ( v < 0.5 ) 
      {
        r = 0.0 ;
        g = 1.0 ;
        b = 1.0 - 4.0 * ( v - 0.25 ) ;
      } 
      else if ( v < 0.75 ) 
      {
        r = 4.0 * ( v - 0.5 ) ;
        g = 1.0 ;
        b = 0.0 ;
      } 
      else 
      {
        r = 1.0 ;
        g = 1.0 - 4.0 * ( v - 0.75 ) ;
        b = 0.0 ;
      }
      return EncodeARGB(
        (byte) ( r * 255.0 ),
        (byte) ( g * 255.0 ),
        (byte) ( b * 255.0 )
      ) ;
    }

  }

}
