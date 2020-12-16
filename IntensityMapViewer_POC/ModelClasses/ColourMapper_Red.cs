//
// ColourMapper_Red.cs
//

namespace IntensityMapViewer
{

  public class ColourMapper_Red : ColourMapper 
  {

    public static readonly IColourMapper Instance = new ColourMapper_Red() ;

    public ColourMapper_Red ( ) :
    base(
      ColourMappingHelpers.EncodeARGB_Red
    ) {
    }

  }

}
