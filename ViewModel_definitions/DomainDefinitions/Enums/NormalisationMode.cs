//
// NormalisationMode.cs
//

namespace IntensityMapViewer
{

  public enum NormalisationMode {
    Automatic_FromBrightestIntensityInImageMap, // Detect highest intensity value
    Manual_FromUserDefinedValue                 // Highest intensity is specified via a slider
  }

}
