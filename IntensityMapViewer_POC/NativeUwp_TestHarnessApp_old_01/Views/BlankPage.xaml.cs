using System;

using NativeUwp_TestHarnessApp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace NativeUwp_TestHarnessApp.Views
{
    public sealed partial class BlankPage : Page
    {
        private BlankViewModel ViewModel
        {
            get { return ViewModelLocator.Current.BlankViewModel; }
        }

        public BlankPage()
        {
            InitializeComponent();
        }
    }
}
