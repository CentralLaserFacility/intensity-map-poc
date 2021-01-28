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
using UkCentralLaserPoC.Core.Mvvm;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UkCentralLaserPoC.IntensityMap
{
    public sealed partial class IntensityMapViewer_ViewModel: ViewModelBase
    {
    }
    public sealed partial class IntensityMapViewer_UserControl : UserControl
    {
        // public IntensityMapDynamicViewModel ViewModel => DataContext as IntensityMapDynamicViewModel;

        public IntensityMapViewer_UserControl()
        {
            this.InitializeComponent();
        }
    }
}
