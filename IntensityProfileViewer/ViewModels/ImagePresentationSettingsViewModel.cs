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
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableRecipient
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
        if (
          base.SetProperty(
            ref m_colourMapOption,
            value
          ) 
        ) {
          // OnPropertyChanged(
          //   nameof(ColourMapOptionName)
          // ) ;
        } ;
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
              Parent.CurrentSource.MostRecentlyAcquiredIntensityMap?.MaximumIntensityValue ?? 254
            ) ;
          }
        }
      }
    }

    private byte m_normalisationValue = (byte) 120 ; // TODO - should accept null here ???

    public byte NormalisationValue => m_normalisationValue ;

    private int m_recursionLevel = 0 ;

    public void SetNormalisationValue ( byte value )
    {
      // Yikes, this is getting called RECURSIVELY  !!!
      // But only in a UWP build. In Uno-UWP there's one
      // recursive call on startup, in plain UWP the recursive
      // call happens repeatedly ...
      m_recursionLevel++ ;
      if ( m_recursionLevel > 1 )
      {
        Common.DebugHelpers.WriteDebugLines(
          $"Recursive call (#{m_recursionLevel}) of SetNormalisationValue({value})"
        ) ;
      }
      // Note that a recursive call that sets the same value
      // will *not* result in 'PropertyChanged' being raised,
      // so no damage wil be done ...
      base.SetProperty(
        ref m_normalisationValue,
        value,
        nameof(NormalisationValue)
      ) ;
      m_recursionLevel-- ;
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
