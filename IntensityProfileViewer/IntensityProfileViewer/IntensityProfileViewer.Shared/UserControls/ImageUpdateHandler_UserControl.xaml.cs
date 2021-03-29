using IntensityProfileViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IntensityProfileViewer
{

  public sealed partial class ImageUpdateHandler_UserControl : UserControl
  {

    private readonly Windows.UI.Xaml.DispatcherTimer m_dispatcherTimer ;
    
    private readonly System.Threading.Timer m_threadingTimer ;
    
    public IntensityProfileViewer.TimedUpdatesScheduler TimedUpdatesScheduler ;

    public static int SequenceType = 1 ;

    public static int g_nIntensityMapsInSequence = 50 ;

    // private Common.CyclicSelector<IntensityProfileViewer.IIntensityMap> m_dynamicIntensityMapsSelector = new(
    //   SequenceType switch 
    //   {
    //   1 => IntensityProfileViewer.IntensityMapSequence.CreateInstance_RippleRotatingAroundCircle(
    //       g_nIntensityMapsInSequence,
    //       sincFactor                       : 10.0,
    //       fractionalRadialOffsetFromCentre : 0.2
    //     ).IntensityMaps,
    //   2 => IntensityProfileViewer.IntensityMapSequence.CreateInstance_BlobRotatingAroundCircle(
    //       g_nIntensityMapsInSequence
    //     ).IntensityMaps,
    //   3 => IntensityProfileViewer.IntensityMapSequence.CreateInstance_WithProgressivelyIncreasingSincFactor(
    //       g_nIntensityMapsInSequence
    //     ).IntensityMaps,
    //   4 => IntensityProfileViewer.IntensityMapSequence.CreateInstance_WithNoiseAdded(
    //       g_nIntensityMapsInSequence,
    //       sincFactor     : 10.0,
    //       noiseAmplitude : 30
    //     ).IntensityMaps,
    //   _ => throw new System.ApplicationException()
    //   }
    // ) ;

    private readonly Dictionary<
      string,
      Common.CyclicSelector<IntensityProfileViewer.IIntensityMap>
    > m_dynamicIntensityMapSequencesDictionary = new() {
      {
        "Rotating ripple",
        new(
          IntensityProfileViewer.IntensityMapSequence.CreateInstance_RippleRotatingAroundCircle(
            g_nIntensityMapsInSequence,
            sincFactor                       : 10.0,
            fractionalRadialOffsetFromCentre : 0.2
          ).IntensityMaps
        )
      },
      {
        "Rotating blob",
        new(
          IntensityProfileViewer.IntensityMapSequence.CreateInstance_BlobRotatingAroundCircle(
            g_nIntensityMapsInSequence
          ).IntensityMaps
        )
      },
      {
        "Contracting ripple",
        new(
          IntensityProfileViewer.IntensityMapSequence.CreateInstance_WithProgressivelyIncreasingSincFactor(
            g_nIntensityMapsInSequence
          ).IntensityMaps
        )
      },
      {
        "Noisy ripple",
        new(
          IntensityProfileViewer.IntensityMapSequence.CreateInstance_WithNoiseAdded(
            g_nIntensityMapsInSequence,
            sincFactor     : 10.0,
            noiseAmplitude : 30
          ).IntensityMaps
        )
      }
    } ;

    public IEnumerable<string> SourceOptions => m_dynamicIntensityMapSequencesDictionary.Keys ;

    private string? m_currentlySelectedSource = null ;

    public string CurrentlySelectedSource { 
      get => m_currentlySelectedSource ??= SourceOptions.First() ; 
      set {
        m_currentlySelectedSource = value ; 
        PerformIntensityMapUpdate() ;
      }
    } 

    public Common.StringBindingHelper<string> SourceOptionsBindingHelper { get ; }

    public IIntensityMap CurrentIntensityMap { get ; private set ; }

    public System.Action? CurrentIntensityMapChanged ;

    public int HowManyUpdatesPerformed { get ; private set ; } = 0 ;

    public void PerformIntensityMapUpdate ( )
    {
      CurrentIntensityMap = m_dynamicIntensityMapSequencesDictionary[
        CurrentlySelectedSource
      ].GetCurrent_MoveNext() ;
      CurrentIntensityMapChanged?.Invoke() ;
      HowManyUpdatesPerformed++ ;
    }

    public static bool UseThreadingTimer = false ;

    public ImageUpdateHandler_UserControl ( )
    {
      this.InitializeComponent() ;
      TimedUpdatesScheduler = new IntensityProfileViewer.TimedUpdatesScheduler(
        () => {
          PerformIntensityMapUpdate() ;
          // Common.DebugHelpers.WriteDebugLines(
          //   $"Performed update #{HowManyUpdatesPerformed}"
          // ) ;
        }
      ) ;
      SourceOptionsBindingHelper = new(
        SourceOptions,
        (name) => {
          CurrentlySelectedSource = name ;
        }
      ) ;
      // m_updateRateEditor.ViewModel = UpdateRateViewModel = new(){
      //   MinValue = 2.0,
      //   MaxValue = 50.0,
      //   NSteps   = 20,
      //   ValueChanged = (value)=> {
      //     TimedUpdatesScheduler.FramesPerSecond = value ;
      //   }
      // } ;
      m_dispatcherTimer = new Windows.UI.Xaml.DispatcherTimer(){
        Interval = System.TimeSpan.FromMilliseconds(
          TimedUpdatesScheduler.DesiredWakeupPeriodMillisecs
        )
      } ;
      m_threadingTimer = new(
        async (state) => {
          // TimedUpdatesScheduler.OnWakeupNotification(
          //   System.DateTime.Now
          // ) ;
          if ( UseThreadingTimer )
          {
            // Common.DebugHelpers.WriteDebugLines(
            //   "Threading timer fired !"
            // ) ;
            await this.Dispatcher.RunAsync(
              Windows.UI.Core.CoreDispatcherPriority.High,
              () => {
                TimedUpdatesScheduler.OnWakeupNotification(
                  System.DateTime.Now
                ) ;
              }
            ) ;
          }
        },
        state : null,
        dueTime : System.TimeSpan.FromMilliseconds(
          TimedUpdatesScheduler.DesiredWakeupPeriodMillisecs
        ),
        period : System.TimeSpan.FromMilliseconds(
          TimedUpdatesScheduler.DesiredWakeupPeriodMillisecs
        )
      ) ;
      TimedUpdatesScheduler.DesiredWakeupPeriodChanged += () => {
        m_dispatcherTimer.Interval = System.TimeSpan.FromMilliseconds(
          TimedUpdatesScheduler.DesiredWakeupPeriodMillisecs
        ) ;
        m_threadingTimer.Change(
          dueTime : System.TimeSpan.FromMilliseconds(
            TimedUpdatesScheduler.DesiredWakeupPeriodMillisecs
          ),
          period : System.TimeSpan.FromMilliseconds(
            TimedUpdatesScheduler.DesiredWakeupPeriodMillisecs
          )
        ) ;
      } ;
      m_dispatcherTimer.Tick += (s,e) => {
        if ( ! UseThreadingTimer )
        {
          TimedUpdatesScheduler.OnWakeupNotification(
            System.DateTime.Now
          ) ;
        }
      } ;
      m_dispatcherTimer.Start() ;
    }

  }

}
