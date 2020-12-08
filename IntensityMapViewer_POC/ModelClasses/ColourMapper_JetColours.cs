//
// ColourMapper.cs
//

namespace IntensityMapViewer
{
  public class ColourMapper_JetColours : ColourMapper 
  {
    public static readonly IColourMapper Instance = new ColourMapper_JetColours() ;
    public ColourMapper_JetColours ( ) :
    base(
      ColourMapper.MapIntensityToJet_EncodedAsARGB
    ) {
    }
  }

}
