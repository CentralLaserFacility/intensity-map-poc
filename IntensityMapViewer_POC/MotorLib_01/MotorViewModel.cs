//
// MotorViewModel.cs
//

using System.Windows.Input ;

namespace MotorLib_01
{

  //
  // This concrete class will use Graham Cox's wrapper
  // to implement the various properties and commands
  // by connection to PV's
  //

  public class MotorViewModel   
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , IMotorViewModel
  {

    public string AxisDescription => throw new System.NotImplementedException() ;

    public string PositionDisplayUnits => throw new System.NotImplementedException() ;

    public bool MotorIsMoving => throw new System.NotImplementedException() ;

    public bool MotorMoveHasCompleted => throw new System.NotImplementedException() ;

    public bool SoftLimitHasBeenReached => throw new System.NotImplementedException() ;

    public bool InCalibrationMode => throw new System.NotImplementedException() ;

    public ICommand TweakRightCommand => throw new System.NotImplementedException() ;

    public ICommand TweakLeftCommand => throw new System.NotImplementedException() ;

    public double TweakAmount { 
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ; 
    }

    public double TweakAmount_MinimumPermitted => throw new System.NotImplementedException() ;

    public double TweakAmount_MaximumPermitted => throw new System.NotImplementedException() ;

    public double EncoderPosition_mm => throw new System.NotImplementedException() ;

    public double MotorPosition_mm => throw new System.NotImplementedException() ;

    public double DesiredTargetPosition_mm { 
      get => throw new System.NotImplementedException() ; 
      set => throw new System.NotImplementedException() ; 
    }
  
  }

}
