//
// ImagePresentationSettingsViewModel.cs
//

namespace IntensityMapViewer
{

  //
  // Hmm, we need to refresh the image when any of these properties change.
  // Several ways we might do this ... TBD
  //

  public class ImagePresentationSettingsViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , IImagePresentationSettingsViewModel
  {

    private ColourMapOption m_colourMapOption = ColourMapOption.GreyScale ;

    public ColourMapOption ColourMapOption {
      get => m_colourMapOption ;
      set => base.SetProperty(
        ref m_colourMapOption,
        value
      ) ;
    }

    private NormalisationMode m_normalisationMode = NormalisationMode.Manual_FromUserDefinedValue ;

    public NormalisationMode NormalisationMode {
      get => m_normalisationMode ;
      set => base.SetProperty(
        ref m_normalisationMode,
        value
      ) ;
    }

    private byte m_normalisationValue = (byte) 120 ;

    public byte NormalisationValue => m_normalisationValue ;

    public void SetNormalisationValue ( byte value )
    {
      base.SetProperty(
        ref m_normalisationValue,
        value,
        nameof(NormalisationValue)
      ) ;
    }

    public bool CanSetNormalisationValue => (
      NormalisationMode is NormalisationMode.Manual_FromUserDefinedValue 
    ) ;

    public DisplayPanelViewModel Parent { get ; }

    public ImagePresentationSettingsViewModel ( DisplayPanelViewModel parent )
    {
      Parent = parent ;
    }

  }

}
