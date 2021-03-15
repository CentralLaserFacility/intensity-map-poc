//
// ColourMapper_GreyScale.cs
//

namespace IntensityProfileViewer
{

  public class ColourMapper_GreyScale : ColourMapper 
  {

    public static readonly IColourMapper Instance = new ColourMapper_GreyScale() ;

    public ColourMapper_GreyScale ( ) :
    base(
      ColourMappingHelpers.EncodeARGB_Grey
    ) {
    }

  }

}
