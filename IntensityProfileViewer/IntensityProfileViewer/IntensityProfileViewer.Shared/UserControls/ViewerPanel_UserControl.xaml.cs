﻿using System;
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

namespace IntensityProfileViewer
{

  public sealed partial class ViewerPanel_UserControl : UserControl
  {

    // private IntensityMapViewer.IDisplayPanelViewModel ViewModel => ViewModelBase as IntensityMapViewer.IDisplayPanelViewModel ;

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel", 
      typeof(IntensityProfileViewer.IDisplayPanelViewModel), 
      typeof(ViewerPanel_UserControl), 
      new PropertyMetadata(0)
    ) ;

    public IntensityProfileViewer.IDisplayPanelViewModel ViewModel
    {
      get => GetValue(ViewModelProperty) as IntensityProfileViewer.IDisplayPanelViewModel ;
      set => SetValue(ViewModelProperty,value) ;
    }

    public ViewerPanel_UserControl ( )
    {
      this.InitializeComponent();
      // DataContextChanged += (s,e) => {
      //   System.Diagnostics.Debug.WriteLine(
      //     $"{this.GetType()} DataContext => {DataContext?.GetType().ToString()??"null"}"
      //   ) ;
      //   this.Bindings.Update() ; // Yikes - gotta call this explicitly ? WTF !!!
      //   // Hmm, this 'm_intensityMapImage' UserControl has an x:Bind that is meant to
      //   // bind its data context to the same ViewModel - but is that working ???
      //   // m_intensityMapImageAndProfileGraphs_UserControl.DataContext = ViewModel ;
      // } ;
      m_imageUpdateHandler.CurrentIntensityMapChanged = ()=> {
        ViewModel.CurrentSource.SetRecentlyAcquiredIntensityMap(
          m_imageUpdateHandler.CurrentIntensityMap
        ) ;
      } ;
      this.Loaded += (s,e) => {
        m_imageUpdateHandler.PerformIntensityMapUpdate() ;
      } ;
    }

    private void TestButton_Click ( object sender, RoutedEventArgs e )
    {

    }

  }

}