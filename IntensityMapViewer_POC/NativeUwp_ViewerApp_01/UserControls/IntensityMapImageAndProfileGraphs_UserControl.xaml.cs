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
  public sealed partial class IntensityMapImageAndProfileGraphs_UserControl : UserControl
  {

    private IntensityMapViewer.IDisplayPanelViewModel ViewModel => DataContext as IntensityMapViewer.IDisplayPanelViewModel ;

    private IntensityMapViewer.IDisplayPanelViewModel RootViewModel ;


    public IntensityMapImageAndProfileGraphs_UserControl()
    {
      this.InitializeComponent();
      DataContextChanged += (s,e) => {
        System.Diagnostics.Debug.WriteLine(
          $"{this.GetType()} DataContext => {DataContext?.GetType().ToString()??"null"}"
        ) ;
        m_intensityMapImage_UserControl.DataContext = ViewModel?.CurrentSource?.MostRecentlyAcquiredIntensityMap ;
      } ;
    }
  }
}
