//
// IntensityMapsDemo_ViewModel.cs
//

namespace IntensityMapViewer
{

  //
  // This drives a UI that lets us exercise the visualisation of an IntensityMap.
  //
  // A concrete subclass will set the property values, and a XAML UI will
  // bind visual elements to them.
  //

  public abstract class IntensityMapsDemo_ViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  {

    private IIntensityMap m_intensityMap ;
    public IIntensityMap IntensityMap {
      get => m_intensityMap ;
      set => SetProperty(ref m_intensityMap,value) ;
    }

    private ColourMapOption m_colourMapOption ;
    public ColourMapOption ColourMapOption {
      get => m_colourMapOption ;
      set => SetProperty(ref m_colourMapOption,value) ;
    }

    private string m_intensityMapLabel ;
    public string IntensityMapLabel 
    {
      get => m_intensityMapLabel ;
      set => SetProperty(ref m_intensityMapLabel,value) ;
    }

  }

}
