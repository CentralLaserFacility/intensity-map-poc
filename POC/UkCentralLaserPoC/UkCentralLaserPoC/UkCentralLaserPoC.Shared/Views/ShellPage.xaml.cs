using System;

using UkCentralLaserPoC.ViewModels;

using Windows.UI.Xaml.Controls;

namespace UkCentralLaserPoC.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        private ShellViewModel ViewModel => ViewModelLocator.Current.ShellViewModel;

        public ShellPage()
        {
            InitializeComponent();
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
        }
    }
}
