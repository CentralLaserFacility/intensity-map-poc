﻿//
// ProfileDisplaySettingsViewModel.cs
//

using System.Collections.Generic;
using System.Linq;
using static Microsoft.Toolkit.Mvvm.Messaging.IMessengerExtensions;

namespace IntensityProfileViewer
{

  public class ProfileDisplaySettingsViewModel 
  : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableRecipient
  , IProfileDisplaySettingsViewModel
  {

    // Use Messenger instead ?
    public event System.Action? ProfileGraphsReferencePositionChanged ;

    private System.Drawing.Point? m_profileGraphsReferencePosition = new System.Drawing.Point(10,10) ;

    public System.Drawing.Point? ProfileGraphsReferencePosition {
      get => m_profileGraphsReferencePosition ;
      set {
        if ( 
          SetProperty(
            ref m_profileGraphsReferencePosition,
            value
          )
        ) {
          ProfileGraphsReferencePositionChanged?.Invoke() ;
          OnPropertyChanged(nameof(VerticalProfileIntensityValues)) ;
          OnPropertyChanged(nameof(HorizontalProfileIntensityValues)) ;
          Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(
            new ReferencePositionChangedMessage(
              m_profileGraphsReferencePosition?.X,
              m_profileGraphsReferencePosition?.Y
            )
          ) ;
        }
      }
    }

    public System.Collections.Generic.IReadOnlyList<byte> VerticalProfileIntensityValues 
    // => GetDummyProfileIntensityValues() ;
    => (
      Parent.MostRecentlyAcquiredIntensityMap != null
      && ProfileGraphsReferencePosition.HasValue 
      ? Parent.MostRecentlyAcquiredIntensityMap.VerticalSliceAtColumn(
          ProfileGraphsReferencePosition.Value.X
        )
      : GetDummyProfileIntensityValues() 
    ) ;

    public System.Collections.Generic.IReadOnlyList<byte> HorizontalProfileIntensityValues
    // => GetDummyProfileIntensityValues() ;
    => (
      Parent.MostRecentlyAcquiredIntensityMap != null
      && ProfileGraphsReferencePosition.HasValue 
      ? Parent.MostRecentlyAcquiredIntensityMap.HorizontalSliceAtRow(
          ProfileGraphsReferencePosition.Value.Y
        )
      : GetDummyProfileIntensityValues() 
    ) ;

    private bool m_shouldShowProfileGraphs = true ;

    public bool ShouldShowProfileGraphs {
      get => m_shouldShowProfileGraphs ;
      set => base.SetProperty(
        ref m_shouldShowProfileGraphs,
        value
      ) ;
    }

    private static System.Collections.Generic.IReadOnlyList<byte>? g_dummyProfileIntensityValues ;

    private static System.Collections.Generic.IReadOnlyList<byte> GetDummyProfileIntensityValues ( )
    => (
      g_dummyProfileIntensityValues 
      ??= Enumerable.Range(0,320).Select(
        i => (byte) ( i % 256 )
      ).ToList() 
    ) ;

    public ISourceViewModel Parent { get ; }

    public System.Collections.Generic.IEnumerable<IViewModel> ChildViewModels => new IViewModel[]{} ;

    public ProfileDisplaySettingsViewModel ( ISourceViewModel parent )
    {
      Parent = parent ;
    }

  }

}
