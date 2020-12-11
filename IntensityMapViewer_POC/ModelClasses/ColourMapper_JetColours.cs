//
// ColourMapper_JetColours.cs
//

namespace IntensityMapViewer
{

  public class ColourMapper_JetColours : ColourMapper 
  {

    public static readonly IColourMapper Instance = new ColourMapper_JetColours() ;

    public ColourMapper_JetColours ( ) :
    base(
      Common.GraphicsHelpers.MapIntensityToJet_EncodedAsARGB
    ) {
    }

  }

}
