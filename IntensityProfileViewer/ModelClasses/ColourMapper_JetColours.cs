//
// ColourMapper_JetColours.cs
//

namespace IntensityProfileViewer
{

  public class ColourMapper_JetColours : ColourMapper 
  {

    public static readonly IColourMapper Instance = new ColourMapper_JetColours() ;

    public ColourMapper_JetColours ( ) :
    base(
      ColourMappingHelpers.MapIntensityToJet_EncodedAsARGB
    ) {
    }

  }

}
