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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

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
        DisplayName = "X position (%)",
        MinValue    = 0.0,
        MaxValue    = 100.0
      } ;
      YPositionViewModel = new() {
        DisplayName  = "Y position (%)",
        MinValue     = 0.0,
        MaxValue     = 100.0
      } ;
      XPositionViewModel.ValueChanged = SetReferencePosition ;
      YPositionViewModel.ValueChanged = SetReferencePosition ;
      // Loaded += (s,e) => {
      //   // We need to wait until the Loaded event has fired before accessing the ViewModel.
      //   // Hmm, better to hook into a ViewModel changed event ...
      //   ViewModel.ProfileGraphsReferencePositionChanged += ()=> {
      //     if ( ViewModel.ProfileGraphsReferencePosition.HasValue )
      //     {
      //       XPositionViewModel.CurrentValue = (
      //         ViewModel.ProfileGraphsReferencePosition.Value.X * 100.0 / ViewModel.HorizontalProfileIntensityValues.Count 
      //       ) ;
      //       YPositionViewModel.CurrentValue = (
      //         ViewModel.ProfileGraphsReferencePosition.Value.Y * 100.0 / ViewModel.VerticalProfileIntensityValues.Count 
      //       ) ;
      //     }
      //   } ;
      // } ;
      void SetReferencePosition ( )
      {
        ViewModel.ProfileGraphsReferencePosition = new System.Drawing.Point(
          (int) (
            XPositionViewModel.CurrentValue * ViewModel.HorizontalProfileIntensityValues.Count / 100.0
          ),
          (int) (
            YPositionViewModel.CurrentValue * ViewModel.VerticalProfileIntensityValues.Count / 100.0
          )
        ) ;
      }
    }

    private void OnViewModelPropertyChanged ( 
      IntensityMapViewer.IProfileDisplaySettingsViewModel? oldViewModel,
      IntensityMapViewer.IProfileDisplaySettingsViewModel? newViewModel
    ) {
      // TODO : we should properly handle a change to a different view model,
      // by de-registering the event handler for a non-null 'oldViewModel' ...
      if ( newViewModel != null )
      {
        newViewModel.ProfileGraphsReferencePositionChanged += ()=> {
          if ( newViewModel.ProfileGraphsReferencePosition.HasValue )
          {
            XPositionViewModel.CurrentValue = (
              newViewModel.ProfileGraphsReferencePosition.Value.X * 100.0 / newViewModel.HorizontalProfileIntensityValues.Count 
            ) ;
            YPositionViewModel.CurrentValue = (
              newViewModel.ProfileGraphsReferencePosition.Value.Y * 100.0 / newViewModel.VerticalProfileIntensityValues.Count 
            ) ;
          }
        } ;
      }
    }

    // private void OnProfileGraphsReferencePositionChanged ( )
    // {
    // 
    // }

  }

}
