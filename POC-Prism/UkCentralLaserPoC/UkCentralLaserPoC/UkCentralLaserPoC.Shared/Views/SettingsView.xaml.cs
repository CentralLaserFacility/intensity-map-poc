using System;

using UkCentralLaserPoC.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UkCentralLaserPoC.Views
{
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsView : Page
    {

         // STEVET_QUESTION : So the DataContext gets set by some Prism magic,
         // as a consequence of defining
         //   prismMvvm:ViewModelLocator.AutowireViewModel="True"
         // in the XAML ??

        public SettingsViewModel ViewModel => DataContext as SettingsViewModel;

        public SettingsView()
        {
            InitializeComponent();
        }
    }
}
