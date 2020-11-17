//
// ISourceSettingsViewModel.cs
//

namespace IntensityMapViewer
{

  //
  // Defines the adjustable settings for a Source ie a Camera.
  //

  public interface ISourceSettingsViewModel : IViewModel 
  {

    double CurrentGainValue { get ; set ; }

    double CurrentExposureTimeInMicroseconds { get ; set ; }

    ImageAcquisitionTriggeringMode TriggeringMode { get ; set ; }

    //
    // Not implemented for the time being ... size is fixed
    //
    // System.Drawing.Size SourceImageDimensions { get ; set ; } 
    //

    // Allowable ranges for gain and exposure

    (double Min, double Max) ValidGainValuesRange { get ; }

    (double Min, double Max) ValidExposureTimesRange { get ; }

  }

}
