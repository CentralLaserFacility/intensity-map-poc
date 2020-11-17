//
// IntensityMapSourceSettings.cs
//

namespace IntensityMapViewer
{

  public class IntensityMapSourceSettings 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , ISourceSettingsViewModel
  {

    public double CurrentGainValue {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public double CurrentExposureTimeInMicroseconds {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public ImageAcquisitionTriggeringMode TriggeringMode {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public (double Min, double Max) ValidExposureTimesRange => throw new System.NotImplementedException() ;

    public (double Min, double Max) ValidGainValuesRange => throw new System.NotImplementedException() ;

  }

}
