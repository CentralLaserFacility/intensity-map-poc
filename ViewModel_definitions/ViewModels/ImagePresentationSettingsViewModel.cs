//
// ImagePresentationSettingsViewModel.cs
//

namespace IntensityMapViewer
{

  public class ImagePresentationSettingsViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , IImagePresentationSettingsViewModel
  {

    public ColourMapOption ColourMapOption {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public NormalisationMode NormalisationMode {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public byte NormalisationValue {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

  }

}
