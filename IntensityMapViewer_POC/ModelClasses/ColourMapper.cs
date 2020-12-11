//
// ColourMapper.cs
//

using Common.ExtensionMethods ;
using System.Collections.Generic ;
using System.Linq ;

namespace IntensityMapViewer
{

  public abstract class ColourMapper : IColourMapper 
  {

    public static IColourMapper InstanceFor ( ColourMapOption option ) 
    => option switch 
    {
    ColourMapOption.GreyScale  => ColourMapper_GreyScale.Instance,
    ColourMapOption.JetColours => ColourMapper_JetColours.Instance,
    _                          => throw new System.ApplicationException()
    } ;

    //
    // Rather than invoking a complex pixel-mapping function every time
    // a 'mapping' function is invoked, we pre-compute the result for each
    // possible input value and save it in a lookup table.
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
