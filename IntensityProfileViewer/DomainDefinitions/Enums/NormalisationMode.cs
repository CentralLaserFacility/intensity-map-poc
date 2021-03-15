//
// NormalisationMode.cs
//

namespace IntensityMapViewer
{

  public enum NormalisationMode {
    Automatic, // Detect highest intensity value              _FromBrightestIntensityInImageMap
    Manual     // Highest intensity is specified via a slider _FromUserDefinedValue
  }

}
