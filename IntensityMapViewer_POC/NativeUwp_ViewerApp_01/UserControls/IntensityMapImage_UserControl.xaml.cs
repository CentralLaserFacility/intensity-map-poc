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

  public sealed partial class IntensityMapImage_UserControl : UserControl
  {

    private IntensityMapViewer.IIntensityMap ViewModel => DataContext as IntensityMapViewer.IIntensityMap ;

    public IntensityMapImage_UserControl()
    {
      this.InitializeComponent();
      DataContextChanged += (s,e) => {
        System.Diagnostics.Debug.WriteLine(
          $"{this.GetType()} DataContext => {DataContext?.GetType().ToString()??"null"}"
        ) ;
      } ;
    }
  }
}
