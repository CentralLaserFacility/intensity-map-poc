//
// TimedUpdatesScheduler.cs
//

using System.Collections ;
using System.Collections.Generic ;
using System.Linq ;

namespace IntensityProfileViewer
{

  //
  // This uses an externally supplied timer to regularly invoke an 'update' action,
  // eg to dynamically cycle through a set of pre-computed settings.
  //    
  // The scheduler maintains a property that represents the desired 'wakeup period',
  // together with min and max values. The desired wakeup period can be set between 
  // those limits, either in terms of milliseconds or as frames-per-second.
  //
  // The external client is expected to invoke the 'wakeup' function at 
  // the regular intervals indicated by the DesiredWakeupPeriod property.
  //

  public interface ITimedUpdatesScheduler
  {
    System.Action? UpdateActionToBePerformed { get ; set ; }
    double DesiredWakeupPeriodMillisecs { get ; set ; }
    event System.Action? DesiredWakeupPeriodChanged ;
    double TimerPeriodInMillisecs { get ; set ; }
    double TimerPeriod_Min     { get ; } 
    double TimerPeriod_Max     { get ; } 
    double TimerPeriod_Default { get ; } 
    string TimerPeriod_AsString { get ; }
    double UpdatesPerSecond { get ; set ; }
    double UpdatesPerSecond_Max { get ; }
    double UpdatesPerSecond_Min { get ; }
    string UpdatesPerSecond_AsString { get ; }
    bool EnableTimedUpdates { get ; set ; } 
    Microsoft.Toolkit.Mvvm.Input.IRelayCommand StartTimedUpdates { get ; }
    Microsoft.Toolkit.Mvvm.Input.IRelayCommand StopTimedUpdates  { get ; }
    Microsoft.Toolkit.Mvvm.Input.IRelayCommand PerformUpdate  { get ; }
  }

  public class TimedUpdatesScheduler 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  , Common.IExpectsTimerWakeupNotifications
  , ITimedUpdatesScheduler 
  {

    private double m_desiredWakeupPeriodMillisecs ;
    public double DesiredWakeupPeriodMillisecs 
    { 
      get => m_desiredWakeupPeriodMillisecs ;
      set {
        if (
          SetProperty(
            ref m_desiredWakeupPeriodMillisecs,
            value
          )
        ) {
          DesiredWakeupPeriodChanged?.Invoke() ;
          base.OnPropertyChanged(
            nameof(TimerPeriodInMillisecs)
          ) ;
          base.OnPropertyChanged(
            nameof(TimerPeriod_AsString)
          ) ;
          base.OnPropertyChanged(
            nameof(UpdatesPerSecond)
          ) ;
          base.OnPropertyChanged(
            nameof(UpdatesPerSecond_AsString)
          ) ;
        }
      }
    }

    private bool m_enableTimedUpdates = false ;
    public bool EnableTimedUpdates
    { 
      get => m_enableTimedUpdates ;
      set {
        if (
          SetProperty(
            ref m_enableTimedUpdates,
            value
          )
        ) {
          OnPropertyChanged(
            nameof(StartTimedUpdates)
          ) ;
          OnPropertyChanged(
            nameof(StopTimedUpdates)
          ) ;
          OnPropertyChanged(
            nameof(PerformUpdate)
          ) ;
        }
      }
    }

    public event System.Action? DesiredWakeupPeriodChanged ;

    public double TimerPeriodInMillisecs
    {
      get => DesiredWakeupPeriodMillisecs ;
      set => DesiredWakeupPeriodMillisecs = value ;
    }

    public double UpdatesPerSecond
    {
      // ( 1000.0 / 20mS ) ==> 50 fps
      get => 1000.0 / TimerPeriodInMillisecs ;
      // 50 fps ==> timer period of (1000/50) ==> 20mS 
      set => TimerPeriodInMillisecs = 1000.0 * ( 1.0 / value ) ;
    }

    // Hmm, pity that the x:Bind engine doesn't understand tuples ...
    // public (double Min,double Max) TimerPeriodValidRange => (20.0,2000.0) ;

    public double TimerPeriod_Min     { get ; } = 20.0 ;
    public double TimerPeriod_Max     { get ; } = 1000.0 ;

    public double TimerPeriod_Default { get ; } = 500.0 ;

    public double UpdatesPerSecond_Max => 1000.0 / TimerPeriod_Min ; // 20mS => 50 fps
    public double UpdatesPerSecond_Min => 1000.0 / TimerPeriod_Max ; // 500mS => 2 fps

    public string TimerPeriod_AsString => $"Update requested every {TimerPeriodInMillisecs:F0}mS" ;

    public string UpdatesPerSecond_AsString => $"Updates per sec : {UpdatesPerSecond:F1}" ;

    public System.Action? UpdateActionToBePerformed { get ; set ; }

    public TimedUpdatesScheduler ( System.Action updateActionToBePerformed )
    {
      UpdateActionToBePerformed = updateActionToBePerformed ;
      m_desiredWakeupPeriodMillisecs = TimerPeriod_Default ; 
      StartTimedUpdates = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => {
          EnableTimedUpdates = true ;
          // base.IntensityMap = m_dynamicIntensityMapsSelector.GetCurrent_MoveNext() ;
          StartTimedUpdates.NotifyCanExecuteChanged() ;
          StopTimedUpdates.NotifyCanExecuteChanged() ;
          PerformUpdate.NotifyCanExecuteChanged() ;
        },
        () => EnableTimedUpdates is false
      ) ;
      StopTimedUpdates = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => {
          EnableTimedUpdates = false ;
          StartTimedUpdates.NotifyCanExecuteChanged() ;
          StopTimedUpdates.NotifyCanExecuteChanged() ;
          PerformUpdate.NotifyCanExecuteChanged() ;
        },
        () => EnableTimedUpdates is true
      ) ;
      PerformUpdate = new Microsoft.Toolkit.Mvvm.Input.RelayCommand(
        () => {
          UpdateActionToBePerformed?.Invoke() ;
        },
        () => (
           UpdateActionToBePerformed is not null
        && EnableTimedUpdates is false
        )
      ) ;
      // base.IntensityMap = m_dynamicIntensityMapsSelector.GetCurrent_MoveNext() ;
      // base.IntensityMapLabel = "This will cycle through 60 variants" ;
      // base.ColourMapOption = ColourMapOption.JetColours ;
    }

    // private Common.CyclicSelector<IntensityMapViewer.IIntensityMap> m_dynamicIntensityMapsSelector = new(
    //   1 switch 
    //   {
    //   1 => IntensityMapViewer.IntensityMapSequence.CreateInstance_RippleRotatingAroundCircle(
    //       nIntensityMaps                   : 60,
    //       sincFactor                       : 10.0,
    //       fractionalRadialOffsetFromCentre : 0.2
    //     ).IntensityMaps,
    //   2 => IntensityMapViewer.IntensityMapSequence.CreateInstance_BlobRotatingAroundCircle(
    //       60
    //     ).IntensityMaps,
    //   _ => throw new System.ApplicationException()
    //   }
    // ) ;

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand StartTimedUpdates { get ; }

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand StopTimedUpdates { get ; }

    public Microsoft.Toolkit.Mvvm.Input.IRelayCommand PerformUpdate { get ; }

    // It can be interesting to log the times taken
    // to perform the operation we're invoking, and 
    // the actual times between 'wakeups'.

    private static bool MeasureTimerTickTimings = false ;

    private List<long> m_actionExecutionTimesLog = new() ;

    private List<long> m_actualTimerWakeupIntervalsLog = new() ;

    private System.Diagnostics.Stopwatch m_timerTickStopwatch = new() ;

    private System.Diagnostics.Stopwatch m_timerTickReportStopwatch = new() ;

    private System.Diagnostics.Stopwatch m_executionTimingStopwatch = null ;

    public string UpdateExecutionTimes { get ; private set ; } = "" ;

    public string ActualTimerWakeupIntervals { get ; private set ; } = "" ;

    private bool m_isBusyHandlingWakeupNotification = false ;

    private int m_nWakeupNotificationsRejected = 0 ;

    public void OnWakeupNotification ( System.DateTime currentTime )
    {
      if ( m_isBusyHandlingWakeupNotification )
      {
        m_nWakeupNotificationsRejected++ ;
        return ;
      }
      m_isBusyHandlingWakeupNotification = true ;
      try
      {
        if ( MeasureTimerTickTimings )
        {
          if ( m_timerTickStopwatch.IsRunning )
          {
            m_actualTimerWakeupIntervalsLog.Add(
              m_timerTickStopwatch.ElapsedMilliseconds
            ) ;
            m_timerTickStopwatch.Reset() ;
          }
          else
          {
            m_timerTickStopwatch.Start() ;
            m_timerTickReportStopwatch.Start() ;
          }
          if ( m_timerTickReportStopwatch.ElapsedMilliseconds > 1000 )
          {
            ActualTimerWakeupIntervals = (
              "Actual wakeup intervals : "
            + string.Join(
                " ",
                m_actualTimerWakeupIntervalsLog.OrderBy(
                  time => time
                ).Select(
                  time => time.ToString("F0")
                )
              )
            ) ;
            Common.DebugHelpers.WriteDebugLines(
              ActualTimerWakeupIntervals
            ) ;
            base.OnPropertyChanged(nameof(ActualTimerWakeupIntervals)) ;
            m_actualTimerWakeupIntervalsLog.Clear() ;
            m_timerTickReportStopwatch.Restart() ;
          }
        }
        if ( EnableTimedUpdates )
        {
          if ( m_executionTimingStopwatch is null )
          {
            m_executionTimingStopwatch = new() ;
            m_executionTimingStopwatch.Start() ;
          }
          if ( MeasureTimerTickTimings )
          {
            var timeBeforeActionExecution = m_executionTimingStopwatch.ElapsedMilliseconds ;
            UpdateActionToBePerformed?.Invoke() ;
            var timeAfterActionExecution = m_executionTimingStopwatch.ElapsedMilliseconds ;
            m_actionExecutionTimesLog.Add(
              timeAfterActionExecution
            - timeBeforeActionExecution
            ) ;
            if ( m_actionExecutionTimesLog.Count == 20 )
            {
              UpdateExecutionTimes = (
                "Action execution times (mS) (ordered) : "
              + string.Join(
                  " ",
                  m_actionExecutionTimesLog.OrderBy(
                    time => time
                  ).Select(
                    time => time.ToString()
                  )
                )
              ) ;
              Common.DebugHelpers.WriteDebugLines(
                UpdateExecutionTimes
              ) ;
              // Hmm, not a good idea to do this in a time critical loop ...
              // base.OnPropertyChanged(nameof(UpdateExecutionTimes)) ;
              m_actionExecutionTimesLog.Clear() ;
            }
          }
          else
          {
            UpdateActionToBePerformed?.Invoke() ;
          }
        }
      }
      catch ( System.Exception x )
      {
      }
      finally
      {
        m_isBusyHandlingWakeupNotification = false ;
      }
    }

  }

}
