//
// IMotorViewModel.cs
//

using System.ComponentModel ;
using System.Windows.Input ;

namespace MotorLib_01
{

  //
  // This is the interface that we'll bind to
  // from a WinUI-3 UserControl
  //

  public interface IMotorViewModel : INotifyPropertyChanged
  {

    string AxisDescription { get ; }

    // Reporting the position

    double EncoderPosition_mm { get ; }

    double MotorPosition_mm { get ; }

    // Setting the desired position

    double DesiredTargetPosition_mm { get ; set ; }

    // Tweaking the position left and right

    double TweakAmount { get ; set ; }

    double TweakAmount_MinimumPermitted { get ; }

    double TweakAmount_MaximumPermitted { get ; }

    ICommand TweakRightCommand { get ; }

    ICommand TweakLeftCommand { get ; }

    // Motion and status

    bool MotorIsMoving { get ; }

    // ?? When true, we display 'Targeting' ... ???

    bool MotorMoveHasCompleted { get ; }

    bool SoftLimitHasBeenReached { get ; }

    // Either 'mm' or 'um'. In a proper version we'll define
    // an enum, but a string will do for now ...

    string PositionDisplayUnits { get ; }

    bool InCalibrationMode { get ; }

  }

 }
