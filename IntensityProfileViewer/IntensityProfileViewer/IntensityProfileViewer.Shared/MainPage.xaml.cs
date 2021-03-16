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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IntensityProfileViewer
{

  public sealed partial class MainPage : Page
  {

    public MainPage ( )
    {
      this.InitializeComponent();
      m_viewerPanel_UserControl.ViewModel = new IntensityProfileViewer.DisplayPanelViewModel() ;

      m_imageUpdateHandler_UserControl.CurrentIntensityMapChanged = ()=> {
        m_viewerPanel_UserControl.ViewModel.CurrentSource.SetRecentlyAcquiredIntensityMap(
          m_imageUpdateHandler_UserControl.CurrentIntensityMap
        ) ;
      } ;
      this.Loaded += (s,e) => {
        m_imageUpdateHandler_UserControl?.PerformIntensityMapUpdate() ;
      } ;
    }

  }

}
