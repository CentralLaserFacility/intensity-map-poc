//
// IColourMapper.cs
//

using System.Collections.Generic;

namespace IntensityMapViewer
{

  public interface IColourMapper 
  {

    uint MapByteValueToEncodedARGB ( 
      byte            byteValue
    ) ;

    IReadOnlyList<uint> MapByteValuesToEncodedARGB ( 
      IReadOnlyList<byte> byteValues 
    ) ;

  }

}
