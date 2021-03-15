//
// ColourMapper_Blue.cs
//

namespace IntensityProfileViewer
{

  public class ColourMapper_Blue : ColourMapper 
  {

    public static readonly IColourMapper Instance = new ColourMapper_Blue() ;

    public ColourMapper_Blue ( ) :
    base(
      ColourMappingHelpers.EncodeARGB_Blue
    ) {
    }

  }

}
