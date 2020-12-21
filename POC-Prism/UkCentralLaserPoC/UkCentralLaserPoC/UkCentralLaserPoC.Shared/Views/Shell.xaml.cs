using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UkCentralLaserPoC.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UkCentralLaserPoC.Shared.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        private readonly IRegionManager regionManager;

        public Shell(IRegionManager regionManager)
        {
            this.InitializeComponent();
            this.regionManager = regionManager;

            regionManager.RegisterViewWithRegion("ContentRegion", typeof(MainView));
        }

        public void ItemSelected(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                regionManager.RequestNavigate("ContentRegion", "SettingsPage");
            }
            else
            {
                switch(args.InvokedItem)
                {
                    case "Main":
                        regionManager.RequestNavigate("ContentRegion", "MainPage");
                        break;
                    case "Intensity Map":
                        regionManager.RequestNavigate("ContentRegion", "IntensityMapTestView");
                        break;
                }
            }
        }

    }
}
