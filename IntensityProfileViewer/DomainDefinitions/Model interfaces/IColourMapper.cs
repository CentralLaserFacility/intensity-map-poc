//
// IColourMapper.cs
//

using System.Collections.Generic;

namespace IntensityMapViewer
{

  public interface IColourMapper 
  {

    // Hmm, could be more efficient to expose the Lookup Table
    // that's implementing the mapping ; and also, the table entries
    // could be precomputed to apply a Gain Factor ...

    uint MapByteValueToEncodedARGB ( 
      byte            byteValue
    ) ;

    IReadOnlyList<uint> MapByteValuesToEncodedARGB ( 
      IReadOnlyList<byte> byteValues 
    ) ;

  }

}
