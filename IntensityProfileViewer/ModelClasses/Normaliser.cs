//
// Normaliser.cs
//

using System.Collections.Generic;
using System.Linq;

namespace IntensityProfileViewer
{

  public class Normaliser
  {

    //
    // The normaliser applies a 'gain factor' to a stream of image intensity bytes
    // in order to make pixels that have a low value, near zero, appear to be brighter.
    //
    // Typically you'd create a Normaliser with a 'normalisationValue' that represents
    // the brightest pixel value in an image. Once the Normaliser is applied,
    // the pixel intensities will have been multiplied by a gain factor that
    // makes the new brightest pixel have a value of 255 (configurable).
    //

    private readonly float m_gainToApply ;

    public Normaliser ( byte normalisationValue, byte desiredBrightestIntensityValue = 255 )
    {
      // If the normalisationValue is 127, and the desired brightest value is 255,
      // we'll apply a gain of 255/127 to each pixel ie a gain of approximately 2.
      m_gainToApply = ( (float) desiredBrightestIntensityValue ) / normalisationValue ;
    }

    public byte[] ApplyTo ( byte[] imageBytes )
    {
      int nBytes = imageBytes.Length ;
      byte[] scaledResult = new byte[nBytes] ;
      for ( int i = 0 ; i < nBytes ; i++ )
      {
        float scaledByte = imageBytes[i] * m_gainToApply ;
        scaledResult[i] = (
          scaledByte > 255.0f
          ? (byte) 255
          : (byte) scaledByte
        ) ;
      }
      return scaledResult ;
    }

    public IReadOnlyList<byte> ApplyTo ( IReadOnlyList<byte> imageBytes )
    {
      int nBytes = imageBytes.Count ;
      byte[] scaledResult = new byte[nBytes] ;
      try
      {
        for ( int i = 0 ; i < nBytes ; i++ )
        {
          float scaledByte = imageBytes[i] * m_gainToApply ;
          scaledResult[i] = (
            scaledByte > 255.0f
            ? (byte) 255
            : (byte) scaledByte
          ) ;
        }
        return scaledResult ;
      }
      catch ( System.Exception x )
      {
        throw ;
      }
    }

  }

  namespace ExtensionMethods
  {

    public static class ApplyingNormaliser
    {

      public static IReadOnlyList<byte> WithNormalisationApplied ( this IReadOnlyList<byte> imageBytes, Normaliser normaliser )
      => normaliser.ApplyTo(imageBytes) ;

      public static byte[] WithNormalisationApplied ( this byte[] imageBytes, Normaliser normaliser )
      => normaliser.ApplyTo(imageBytes) ;

    }

  }

}
