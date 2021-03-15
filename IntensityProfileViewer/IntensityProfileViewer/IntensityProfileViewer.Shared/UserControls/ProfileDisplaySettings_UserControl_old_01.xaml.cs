//
// ProfileDisplaySettings_UserControl_old_01.cs
//

using IntensityProfileViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

  public sealed partial class ProfileDisplaySettings_UserControl_old_01 : UserControl
  {

    //
    // The 'ViewModel' property has to be set up as a DependencyProperty 
    // in order that we can use x:Bind to SET the value in XAML.
    //
    // If we were using some other technique to set the ViewModel,
    // eg Dependency Injection ... it could be an 'ordinary' property ...
    //

    public IntensityProfileViewer.IProfileDisplaySettingsViewModel? ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityProfileViewer.IProfileDisplaySettingsViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.IProfileDisplaySettingsViewModel), 
      typeof(ProfileDisplaySettings_UserControl_old_01), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as ProfileDisplaySettings_UserControl_old_01 ;
          userControlThatOwnsThisViewModelProperty.OnViewModelPropertyChanged(
            propertyChangedEventArgs.OldValue as IntensityProfileViewer.IProfileDisplaySettingsViewModel,
            propertyChangedEventArgs.NewValue as IntensityProfileViewer.IProfileDisplaySettingsViewModel
          ) ;
        }
      )
    ) ;

    //
    // For the time being, we're declaring these as 'fields',
    // and x:Bind to their properties seems to work ...
    //
    // Hmm, these really should be Dependency Properties
    // since we're binding to them using x:Bind ???
    //
    // Or at least, we should be implementing INotifyProperyChanged
    // if we were to replace these instances.
    //

    private readonly NumericValueViewModel XPositionViewModel ;

    private readonly NumericValueViewModel YPositionViewModel ;

    public ProfileDisplaySettings_UserControl_old_01 ( )
    {
      this.InitializeComponent() ;
      // Hmm, need to initialise these from the ViewModel ...
      // however the ViewModel is 'null' at this point
      XPositionViewModel = new() {
        DisplayName = "X position (from left)",
        MinValue    = 0.0
      } ;
      YPositionViewModel = new() {
        DisplayName  = "Y position (from top)",
        MinValue     = 0.0
      } ;
    }

    private void OnViewModelPropertyChanged ( 
      IntensityProfileViewer.IProfileDisplaySettingsViewModel? oldViewModel,
      IntensityProfileViewer.IProfileDisplaySettingsViewModel? newViewModel
    ) {
      // TODO : we should properly handle a change to a different view model,
      // by de-registering the event handler for a non-null 'oldViewModel' ...
      if ( newViewModel != null )
      {
        XPositionViewModel.MaxValue = newViewModel.Parent.MostRecentlyAcquiredIntensityMap.Dimensions.Width - 1 ;
        YPositionViewModel.MaxValue = newViewModel.Parent.MostRecentlyAcquiredIntensityMap.Dimensions.Height - 1 ;
        XPositionViewModel.ValueChanged = SetReferencePosition ;
        YPositionViewModel.ValueChanged = SetReferencePosition ;
        XPositionViewModel.CurrentValue = XPositionViewModel.MaxValue / 2 ;
        YPositionViewModel.CurrentValue = YPositionViewModel.MaxValue / 2 ;
        newViewModel.ProfileGraphsReferencePositionChanged += ()=> {
          // GetReferencePosition() ;
          // Hmm, trickiness here as there can be several parties updating the value !!
          // If someone else updates the value, we need to keep our ViewModels in sync
          // but we definitely don't want to set 'CurrentValue' as that will have the effect
          // of pushing *our* current value !!!
          // TODO - make sure this is bulletproof ...
          // IDEALLY WE SHOULD JUST SET THE CURRENT VALUE HERE ???
          // Probably the issue is being caused by having a nested call, 
          // we could probably avoid it by detecting that situation.
          if ( newViewModel.ProfileGraphsReferencePosition.HasValue )
          {
            XPositionViewModel.OnCurrentValueChangedExternally(newViewModel.ProfileGraphsReferencePosition.Value.X) ;
            YPositionViewModel.OnCurrentValueChangedExternally(newViewModel.ProfileGraphsReferencePosition.Value.Y) ;
          }
        } ;
      }

      void SetReferencePosition ( double value )
      {
        newViewModel.ProfileGraphsReferencePosition = new System.Drawing.Point(
          (int) XPositionViewModel.CurrentValue,
          (int) YPositionViewModel.CurrentValue 
        ) ;
      }

      void GetReferencePosition ( )
      {
        if ( newViewModel.ProfileGraphsReferencePosition.HasValue )
        {
          XPositionViewModel.CurrentValue = newViewModel.ProfileGraphsReferencePosition.Value.X ;
          YPositionViewModel.CurrentValue = newViewModel.ProfileGraphsReferencePosition.Value.Y ;
        }
      }
    }

    // private void OnProfileGraphsReferencePositionChanged ( )
    // {
    // 
    // }

  }

}
