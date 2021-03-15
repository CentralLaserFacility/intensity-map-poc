//
// ChangeHandler.cs
//

namespace IntensityProfileViewer
{

 
  public enum WhatChanged {
    ImagePresentationSetting,
    NewImageAcquired,
    ProfilePositionCoordinates
  }

  //
  // Central handler for all changes that can occur ??
  // 
  // Probably better to use the Messenger in the Toolkit ??
  //
  // BUT - should handle several instances of a DisplayPanel ...
  //
  // Maybe have an instance of ChangeHandler 'owned' by the DisplayPanelViewModel ??
  //

  public class ChangeHandler
  {
    
    public static ChangeHandler Instance = new() ;

    public void HandleChange ( WhatChanged whatChanged, DisplayPanelViewModel host )
    {
      // Do the appropriate thing ...
    }

  }

}
