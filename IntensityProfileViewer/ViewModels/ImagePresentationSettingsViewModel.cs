//
// ImagePresentationSettingsViewModel.cs
//

namespace IntensityProfileViewer
{

  //
  // Hmm, we need to refresh the image when any of these properties change.
  // Several ways we might do this ... TBD
  //

  public class ImagePresentationSettingsViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , IImagePresentationSettingsViewModel
  {

    // Nasty hack to to make ComboBox work ...

    // private bool m_colourMapOptionNameIsBeingSet = false ;
    // 
    // public string ColourMapOptionName {
    //   get => m_colourMapOption.ToString() ;
    //   set {
    //     // Gotta manually protect against infinite recursion, yuk.
    //     if ( m_colourMapOptionNameIsBeingSet is false )
    //     {
    //       m_colourMapOptionNameIsBeingSet = true ;
    //       m_colourMapOption = (ColourMapOption) System.Enum.Parse(
    //         typeof(ColourMapOption),
    //         value
    //       ) ;
    //       OnPropertyChanged(
    //         nameof(ColourMapOptionName)
    //       ) ;
    //       OnPropertyChanged(
    //         nameof(ColourMapOption)
    //       ) ;
    //       m_colourMapOptionNameIsBeingSet = false ;
    //     }
    //   }
    // }
    // 
    // public System.Collections.Generic.IEnumerable<string> ColourMapOptionNames { get ; } 
    // = System.Enum.GetNames(typeof(ColourMapOption)) ;

    private ColourMapOption m_colourMapOption = (
      ColourMapOption.GreyScale 
      // ColourMapOption.JetColours 
    ) ;

    public ColourMapOption ColourMapOption {
      get => m_colourMapOption ;
      set {
        base.SetProperty(
          ref m_colourMapOption,
          value
        ) ;
        // OnPropertyChanged(
        //   nameof(ColourMapOptionName)
        // ) ;
      }
    }

    private NormalisationMode m_normalisationMode = (
      NormalisationMode.Automatic
      // NormalisationMode.Manual
    ) ; 

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
          if ( m_normalisationMode == NormalisationMode.Automatic )
          {
            // We've switched from Manual to Automatic mode,
            // so reset the NormalisationValue to match the
            // maximum value in the current IntensityMap
            SetNormalisationValue(
              Parent.CurrentSource.MostRecentlyAcquiredIntensityMap?.MaximumIntensityValue ?? 255
            ) ;
          }
        }
      }
    }

    private byte m_normalisationValue = (byte) 120 ; // TODO - should accept null here ???

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
      // PropertyChanged += (s,e) => {
      //   Common.DebugHelpers.WriteDebugLines(
      //     $"ImagePresentationSettingsViewModel {e.PropertyName} CHANGED",
      //     $"ImagePresentationSettingsViewModel NormalisationMode = {this.NormalisationMode}",
      //     $"ImagePresentationSettingsViewModel CanSetNormalisationValue = {this.CanSetNormalisationValue}"
      //   ) ;
      // } ;
    }

  }

}
