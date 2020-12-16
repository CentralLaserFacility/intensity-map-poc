//
// ColourMapper_Green.cs
//

namespace IntensityMapViewer
{

  public class ColourMapper_Green : ColourMapper 
  {

    public static readonly IColourMapper Instance = new ColourMapper_Green() ;

    public ColourMapper_Green ( ) :
    base(
      ColourMappingHelpers.EncodeARGB_Green
    ) {
    }

  }

}
