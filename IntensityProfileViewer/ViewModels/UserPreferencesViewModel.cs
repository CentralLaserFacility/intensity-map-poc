﻿//
// UserPreferencesViewModel.cs
//

namespace IntensityProfileViewer
{

  //
  // Nice if this is viewable and editable for the POC,
  // but that's not necessary - fixed settings defined
  // in code are fine, so the 'setters' all throw.
  //

  public class UserPreferencesViewModel
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableRecipient
  , IUserPreferencesViewModel
  {

    public System.Drawing.Color CrossSectionLineColour {
      get => System.Drawing.Color.Blue ;
      set => throw new System.NotImplementedException() ;
    }

    public float CrossSectionLineThickness {
      get => 1.0f ;
      set => throw new System.NotImplementedException() ;
    }

    public PanAndZoomMode PanAndZoomMode {
      get => PanAndZoomMode.Constrained ;
      set => throw new System.NotImplementedException() ;
    }

    public HowToDrawProfileGraph HowToDrawProfileGraph {
      get => HowToDrawProfileGraph.DrawLinesBetweenAdjacentPixels ;
      set => throw new System.NotImplementedException() ;
    } 

    public bool ShowHorizontalAndVerticalScrollBarsOnImagePanel {
      get => false ;
      set => throw new System.NotImplementedException() ;
    }

    public bool ShowDebugInfo { 
      get => true ; 
      set => throw new System.NotImplementedException() ; 
    }

    public IDisplayPanelViewModel Parent { get ; }

    // public System.Collections.Generic.IEnumerable<IViewModel> ChildViewModels => new IViewModel[]{} ;

    public UserPreferencesViewModel ( IDisplayPanelViewModel parent )
    { 
      Parent = parent ;
    }

  }

}
