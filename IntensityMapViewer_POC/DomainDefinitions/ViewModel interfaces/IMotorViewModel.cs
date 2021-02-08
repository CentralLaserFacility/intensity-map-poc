//
// IDisplayPanelViewModel.cs
//

namespace IntensityMapViewer
{

  public interface IMotorViewModel : IViewModel
  {

    string AxisDescription { get ; }

    // Either 'mm' or 'um'

    string Units { get ; }

    bool MotorIsMoving { get ; }

    // Tweaking left and right

    void TweakRight ( ) ;

    void TweakLeft ( ) ;

    double TweakAmount { get ; set ; }

    // Positions reported


  }

}
