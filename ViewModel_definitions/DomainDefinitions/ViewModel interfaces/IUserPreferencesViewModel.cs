//
// IUserPreferencesViewModel.cs
//

namespace IntensityMapViewer
{

  public interface IUserPreferencesViewModel : IViewModel
  {

    System.Drawing.Color CrossSectionLineColour { get ; set ; }

    float CrossSectionLineThickness { get ; set ; }

    PanAndZoomMode PanAndZoomMode { get ; set ; }

    PanAndZoomBehaviourOnSourceChange PanAndZoomBehaviourOnSourceChange { get ; set ; } 

    HowToDrawProfileGraph HowToDrawProfileGraph { get ; set ; } 

    HowToShowSourceSettingsPanel HowToShowSourceSettingsPanel { get ; set ; } 

    ThemeChoice ThemeChoice { get ; set ; }

    float FontSizeMultiplier { get ; set ; }
    
    bool ShowHorizontalAndVerticalScrollBarsOnImagePanel { get ; set ; }

    // JSON persistence

    string RenderAsJsonString ( ) ;

    void PopulateFromJsonString ( string jsonString ) ;

  }

}
