//
// IntensityMapTestViewModel.cs
//

// using System.Windows.Input;

//
// The class implementing this interface expects to be 'woken up'
// at regular intervals with a call to 'OnWakeupNotification'.
//

public interface IExpectsTimerWakeupNotifications
{
  double DesiredWakeupPeriodMillisecs { get ; set ; }
  event System.Action? DesiredWakeupPeriodChanged ;
  void OnWakeupNotification ( System.DateTime currentTime ) ;
}
