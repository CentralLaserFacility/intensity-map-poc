//
// IColourMapper.cs
//

using System.Collections.Generic;

namespace IntensityMapViewer
{

  public interface IColourMapper 
  {

    //
    // This function will be invoked whenever we need to convert
    // the data from an ImageMap into a displayable set of pixels.
    //

    IReadOnlyList<uint> MapByteValuesToEncodedARGB ( 
      IReadOnlyList<byte> byteValues,
      ColourMapOption     colourMapOption 
    ) ;

    uint MapByteValueToEncodedARGB ( 
      byte            byteValue,
      ColourMapOption colourMapOption 
    ) ;

  }

}
