//
// IUserPreferencesViewModel.cs
//

namespace IntensityMapViewer
{

  // Useful to have for the POC, but needn't necessarily 
  // be viewable or editable in the UI

  public interface IUserPreferencesViewModel // : IViewModel
  {

    System.Drawing.Color CrossSectionLineColour { get ; set ; }

    float CrossSectionLineThickness { get ; set ; }

    PanAndZoomMode PanAndZoomMode { get ; set ; }

    HowToDrawProfileGraph HowToDrawProfileGraph { get ; set ; } 

    bool ShowHorizontalAndVerticalScrollBarsOnImagePanel { get ; set ; }

  }

}
