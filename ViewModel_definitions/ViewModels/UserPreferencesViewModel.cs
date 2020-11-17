//
// UserPreferencesViewModel.cs
//

namespace IntensityMapViewer
{

  public class UserPreferencesViewModel
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , IUserPreferencesViewModel
  {

    public System.Drawing.Color CrossSectionLineColour {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public float CrossSectionLineThickness {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public PanAndZoomMode PanAndZoomMode {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public PanAndZoomBehaviourOnSourceChange PanAndZoomBehaviourOnSourceChange {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public HowToDrawProfileGraph HowToDrawProfileGraph {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    } 

    public HowToShowSourceSettingsPanel HowToShowSourceSettingsPanel {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    } 

    public ThemeChoice ThemeChoice {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public float FontSizeMultiplier {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }
    
    public bool ShowHorizontalAndVerticalScrollBarsOnImagePanel {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    // Persistence

    public string RenderAsJsonString ( ) => throw new System.NotImplementedException() ;

    public void PopulateFromJsonString ( string jsonString ) => throw new System.NotImplementedException() ;

  }

}
