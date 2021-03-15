//
// IExpectsTimerWakeupNotifications.cs
//

namespace Common
{

  //
  // A class implementing this interface expects to be 'woken up'
  // at regular intervals with a call to 'OnWakeupNotification'.
  //

  public interface IExpectsTimerWakeupNotifications
  {

    double DesiredWakeupPeriodMillisecs { get ; set ; }

    event System.Action? DesiredWakeupPeriodChanged ;

    void OnWakeupNotification ( System.DateTime currentTime ) ;

    // Might be interesting to report the 'actual' timer wakeup intervals
    // that have been achieved ? The wakeup period that has been requested
    // is only an aspiration - the external code that performs the wakeups
    // might not be capable of triggering notifications at the requested rate.
    // THIS WOULD BE BEST EXPRESSED IN A DIFFERENT INTERFACE ??

  }

}
