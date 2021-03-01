//
// ProfileDisplaySettings_UserControl.cs
//

using IntensityMapViewer;
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

namespace NativeUwp_ViewerApp_01
{

  public sealed partial class ProfileDisplaySettings_UserControl : UserControl
  {

    //
    // The 'ViewModel' property has to be set up as a DependencyProperty 
    // in order that we can use x:Bind to SET the value in XAML.
    //
    // If we were using some other technique to set the ViewModel,
    // eg Dependency Injection ... it could be an 'ordinary' property ...
    //

    public IntensityMapViewer.IProfileDisplaySettingsViewModel? ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityMapViewer.IProfileDisplaySettingsViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityMapViewer.IProfileDisplaySettingsViewModel), 
      typeof(ProfileDisplaySettings_UserControl), 
      new PropertyMetadata(
        defaultValue : null,
        propertyChangedCallback : (dp,propertyChangedEventArgs) => {
          var userControlThatOwnsThisViewModelProperty = dp as ProfileDisplaySettings_UserControl ;
          userControlThatOwnsThisViewModelProperty.OnViewModelPropertyChanged(
            propertyChangedEventArgs.OldValue as IntensityMapViewer.IProfileDisplaySettingsViewModel,
            propertyChangedEventArgs.NewValue as IntensityMapViewer.IProfileDisplaySettingsViewModel
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

    public ProfileDisplaySettings_UserControl ( )
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
      IntensityMapViewer.IProfileDisplaySettingsViewModel? oldViewModel,
      IntensityMapViewer.IProfileDisplaySettingsViewModel? newViewModel
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
          // but we definitiely don't want to set 'CurrentValue' as that will have the effect
          // of pushing our current value !!!
          if ( newViewModel.ProfileGraphsReferencePosition.HasValue )
          {
            XPositionViewModel.OnCurrentValueChangedExternally(newViewModel.ProfileGraphsReferencePosition.Value.X) ;
            YPositionViewModel.OnCurrentValueChangedExternally(newViewModel.ProfileGraphsReferencePosition.Value.Y) ;
          }
        } ;
      }
      void SetReferencePosition ( )
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
