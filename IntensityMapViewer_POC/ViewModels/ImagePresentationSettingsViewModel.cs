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

    private ColourMapOption m_colourMapOption = (
      ColourMapOption.GreyScale 
      // ColourMapOption.JetColours 
    ) ;

    public ColourMapOption ColourMapOption {
      get => m_colourMapOption ;
      set => base.SetProperty(
        ref m_colourMapOption,
        value
      ) ;
    }

    private NormalisationMode m_normalisationMode = NormalisationMode.Manual ; // _FromUserDefinedValue ;

    public NormalisationMode NormalisationMode {
      get => m_normalisationMode ;
      set {
        if (
          base.SetProperty(
            ref m_normalisationMode,
            value
          )
        ) {
          OnPropertyChanged(nameof(CanSetNormalisationValue)) ;
        }
      }
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
      NormalisationMode == NormalisationMode.Manual // _FromUserDefinedValue 
    ) ;

    public IDisplayPanelViewModel Parent { get ; }

    public ImagePresentationSettingsViewModel ( IDisplayPanelViewModel parent )
    {
      Parent = parent ;
      PropertyChanged += (s,e) => {
        Common.DebugHelpers.WriteDebugLines(
          $"ImagePresentationSettingsViewModel {e.PropertyName} CHANGED",
          $"ImagePresentationSettingsViewModel NormalisationMode = {this.NormalisationMode}",
          $"ImagePresentationSettingsViewModel CanSetNormalisationValue = {this.CanSetNormalisationValue}"
        ) ;
      } ;
    }

  }

}
