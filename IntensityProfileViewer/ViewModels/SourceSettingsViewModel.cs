//
// SourceSettingsViewModel.cs
//

namespace IntensityMapViewer
{

  public class SourceSettingsViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , ISourceSettingsViewModel
  {

    private ImageDimensions m_imageDimensions = ImageDimensions.Small_320x240 ;

    public ImageDimensions SourceImageDimensions { 
      get => m_imageDimensions ; 
      set {
        SetProperty(
          ref m_imageDimensions,
          value
        ) ;
      }
    } 

    private ImageUpdateRate m_updateRate = ImageUpdateRate.SingleCapture ;

    public ImageUpdateRate UpdateRate { 
      get => m_updateRate ; 
      set {
        SetProperty(
          ref m_updateRate,
          value
        ) ;
      }
    }

    private double m_currentGainValue = 1.0 ;

    public double CurrentGainValue {
      get => m_currentGainValue ;
      set => base.SetProperty(
        ref m_currentGainValue,
        value // .VerifiedInAllowedRange(ValidGainValuesRange)
      ) ;
    }

    private double m_currentExposureTimeInMicroseconds = 1.0 ;

    public double CurrentExposureTimeInMicroseconds {
      get => m_currentExposureTimeInMicroseconds ;
      set => base.SetProperty(
        ref m_currentExposureTimeInMicroseconds,
        value // .VerifiedInAllowedRange(ValidExposureTimesRange)
      ) ;
    }

    // Not currently used !! Need to add validation logic in the 'setters'

    public (double Min, double Max) ValidGainValuesRange => (0.1,10.0) ;

    public (double Min, double Max) ValidExposureTimesRange => (0.1,10.0) ;

    public ISourceViewModel Parent { get ; }

    public SourceSettingsViewModel ( ISourceViewModel parent )
    {
      Parent = parent ;
    }

  }

}
