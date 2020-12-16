//
// ColourMappingHelpers.cs
//

namespace IntensityMapViewer
{

  public static class ColourMappingHelpers
  {

    //
    // Encode 'rgb' byte values as a 32-bit integer.
    //
    // The 'alpha' channel is always 0xff (max opacity), and the byte ordering
    // is as required for compatibility with writing to a WinUI 'WriteableBitmap'.
    //
    // This helper is in 'Common' so that it can be employed in Model and
    // ViewModel classes that can't have a dependency on platform-specific API's,
    // but need to efficiently build representations of coloured images.
    //
    
    public static uint EncodeARGB ( byte red, byte green, byte blue )
    => (uint) (
      ( 0xff  << 24 ) // A : most significant byte is 'alpha'
    | ( red   << 16 ) // R
    | ( green << 8  ) // G
    | ( blue  << 0  ) // B
    ) ;

    public static uint EncodeARGB_Grey ( byte greyValue )
    => (uint) (
      ( 0xff      << 24 ) // A : most significant byte is 'alpha'
    | ( greyValue << 16 ) // R
    | ( greyValue << 8  ) // G
    | ( greyValue << 0  ) // B
    ) ;

    public static uint EncodeARGB_Red ( byte greyValue )
    => (uint) (
      ( 0xff      << 24 ) // A : most significant byte is 'alpha'
    | ( greyValue << 16 ) // R
    ) ;

    public static uint EncodeARGB_Green ( byte greyValue )
    => (uint) (
      ( 0xff      << 24 ) // A : most significant byte is 'alpha'
    | ( greyValue << 8  ) // G
    ) ;

    public static uint EncodeARGB_Blue ( byte greyValue )
    => (uint) (
      ( 0xff      << 24 ) // A : most significant byte is 'alpha'
    | ( greyValue << 0  ) // B
    ) ;

    //
    // Formula for the 'JET' mapping ...
    // https://stackoverflow.com/questions/7706339/grayscale-to-red-green-blue-matlab-jet-color-scale
    //
    // The mapping represents a walk along the edges of the 'RGB color cube' 
    // following the path Blue -> Cyan -> Green -> Yellow -> Red
    // interpolating the intermediate values along this path.
    //

    public static uint MapIntensityToJet_EncodedAsARGB ( byte intensityValue )
    {
      double r ;
      double g ;
      double b ;
      double v = intensityValue / 255.0 ;
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
