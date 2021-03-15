//
// ColourMapper.cs
//

using Common.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;

namespace IntensityProfileViewer
{

  //
  // Colour mapping involves invoking a particular function
  // to get from an 8-bit intensity value to a 32-bit colour. 
  // 
  // Having a 'class' to do this lets us implement caching.
  //
  // An alternative would be to have a static lookup table associated with 
  // each of the low level pixel-mapping functions, which would be
  // initialised in a static constructor ??
  //
  // Then we'd return a System.Func<byte,uint> here instead of an IColourMapper.
  //

  public abstract class ColourMapper : IColourMapper 
  {

    public static IColourMapper InstanceFor ( ColourMapOption option ) 
    => option switch 
    {
    ColourMapOption.GreyScale     => ColourMapper_GreyScale  .Instance,
    ColourMapOption.JetColours    => ColourMapper_JetColours .Instance,
    ColourMapOption.ShadesOfRed   => ColourMapper_Red        .Instance,
    ColourMapOption.ShadesOfGreen => ColourMapper_Green      .Instance,
    ColourMapOption.ShadesOfBlue  => ColourMapper_Blue       .Instance,
    _ => throw new System.ApplicationException()
    } ;

    //
    // Rather than invoking a potentially complex algorithm every time
    // a 'pixel-mapping' function is invoked, we pre-compute the result
    // for each possible input value and save it in a lookup table.
    //

    private readonly uint[] m_mappedResultsLookupTable ;

    protected ColourMapper ( System.Func<byte,uint> pixelMappingFunc )
    {
      m_mappedResultsLookupTable = Enumerable.Range(
        start : 0,
        count : 256
      ).Select(
        i_0_to_255 => pixelMappingFunc(
          (byte) i_0_to_255
        )
      ).ToArray() ;
    }

    // Honouring the 'IColourMapper' promises ...

    uint IColourMapper.MapByteValueToEncodedARGB ( 
      byte byteValue 
    ) 
    => m_mappedResultsLookupTable[byteValue] ;

    IReadOnlyList<uint> IColourMapper.MapByteValuesToEncodedARGB ( 
      IReadOnlyList<byte> byteValues 
    ) => byteValues.Select(
      byteValue => m_mappedResultsLookupTable[byteValue] 
    ).ToList() ;

  }

}
