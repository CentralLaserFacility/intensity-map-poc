﻿using IntensityMapViewer;
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

namespace NativeUwp_ViewerApp_01
{

  public sealed partial class ImageUpdateHandler_UserControl : UserControl
  {

    // public IntensityMapViewer.NumericValueViewModel UpdateRateViewModel { get ; set ; } 
    // 
    // public IntensityMapViewer.NumericValueViewModel UpdatePeriodViewModel { get ; set ; } 

    private Windows.UI.Xaml.DispatcherTimer m_dispatcherTimer ;
    
    private System.Threading.Timer m_threadingTimer ;
    
    public IntensityMapViewer.TimedUpdatesScheduler TimedUpdatesScheduler ;

    public static int SequenceType = 1 ;

    public static int g_nIntensityMapsInSequence = 50 ;

    private Common.CyclicSelector<IntensityMapViewer.IIntensityMap> m_dynamicIntensityMapsSelector = new(
      SequenceType switch 
      {
      1 => IntensityMapViewer.IntensityMapSequence.CreateInstance_RippleRotatingAroundCircle(
          g_nIntensityMapsInSequence,
          sincFactor                       : 10.0,
          fractionalRadialOffsetFromCentre : 0.2
        ).IntensityMaps,
      2 => IntensityMapViewer.IntensityMapSequence.CreateInstance_BlobRotatingAroundCircle(
          g_nIntensityMapsInSequence
        ).IntensityMaps,
      3 => IntensityMapViewer.IntensityMapSequence.CreateInstance_WithProgressivelyIncreasingSincFactor(
          g_nIntensityMapsInSequence
        ).IntensityMaps,
      4 => IntensityMapViewer.IntensityMapSequence.CreateInstance_WithNoiseAdded(
          g_nIntensityMapsInSequence,
          sincFactor     : 10.0,
          noiseAmplitude : 30
        ).IntensityMaps,
      _ => throw new System.ApplicationException()
      }
    ) ;

    private Dictionary<
      string,
      Common.CyclicSelector<IntensityMapViewer.IIntensityMap>
    > m_dynamicIntensityMapSequencesDictionary = new() {
      {
        "Rotating ripple",
        new(
          IntensityMapViewer.IntensityMapSequence.CreateInstance_RippleRotatingAroundCircle(
            g_nIntensityMapsInSequence,
            sincFactor                       : 10.0,
            fractionalRadialOffsetFromCentre : 0.2
          ).IntensityMaps
        )
      },
      {
        "Rotating blob",
        new(
          IntensityMapViewer.IntensityMapSequence.CreateInstance_BlobRotatingAroundCircle(
            g_nIntensityMapsInSequence
          ).IntensityMaps
        )
      },
      {
        "Expanding ripple",
        new(
          IntensityMapViewer.IntensityMapSequence.CreateInstance_WithProgressivelyIncreasingSincFactor(
            g_nIntensityMapsInSequence
          ).IntensityMaps
        )
      },
      {
        "Noisy ripple",
        new(
          IntensityMapViewer.IntensityMapSequence.CreateInstance_WithNoiseAdded(
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

    // private Common.CyclicSelector<IntensityMapViewer.IIntensityMap> m_currentlySelectedSource ;

    public StringBindingHelper<string> SourceOptionsBindingHelper { get ; }

    public IIntensityMap CurrentIntensityMap { get ; private set ; }

    public System.Action? CurrentIntensityMapChanged ;

    public int HowManyUpdatesPerformed { get ; private set ; } = 0 ;

    public void PerformIntensityMapUpdate ( )
    {
      CurrentIntensityMap = (
        m_dynamicIntensityMapSequencesDictionary[CurrentlySelectedSource].GetCurrent_MoveNext()
        // m_dynamicIntensityMapsSelector.GetCurrent_MoveNext() 
      ) ;
      CurrentIntensityMapChanged?.Invoke() ;
      HowManyUpdatesPerformed++ ;
    }

    public static bool UseThreadingTimer = false ;

    public ImageUpdateHandler_UserControl ( )
    {
      this.InitializeComponent() ;
      TimedUpdatesScheduler = new IntensityMapViewer.TimedUpdatesScheduler(
        () => {
          // CurrentIntensityMap = m_dynamicIntensityMapsSelector.GetCurrent_MoveNext() ;
          // CurrentIntensityMapChanged?.Invoke() ;
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
            // await Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher
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

    private void NextButton_Click ( object sender, RoutedEventArgs e )
    {

    }

  }

}