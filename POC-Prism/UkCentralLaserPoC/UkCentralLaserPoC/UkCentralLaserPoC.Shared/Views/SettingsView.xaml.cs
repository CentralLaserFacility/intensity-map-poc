using System;

using UkCentralLaserPoC.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UkCentralLaserPoC.Views
{
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsView : Page
    {
        public SettingsViewModel ViewModel => DataContext as SettingsViewModel;

        public SettingsView()
        {
            InitializeComponent();
        }
    }
}
