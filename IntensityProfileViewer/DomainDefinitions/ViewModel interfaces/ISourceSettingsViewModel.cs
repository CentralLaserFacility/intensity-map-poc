//
// ISourceSettingsViewModel.cs
//

namespace IntensityProfileViewer
{

  //
  // Defines the adjustable settings for a Source ie a Camera.
  //

  public interface ISourceSettingsViewModel : IViewModel 
  {

    ISourceViewModel Parent { get ; }

    ImageDimensions SourceImageDimensions { get ; set ; } 

    ImageUpdateRate UpdateRate { get ; set ; }

    double CurrentGainValue { get ; set ; }

    double CurrentExposureTimeInMicroseconds { get ; set ; }

    // Allowable ranges for gain and exposure

    (double Min, double Max) ValidGainValuesRange { get ; }

    (double Min, double Max) ValidExposureTimesRange { get ; }

  }

}
