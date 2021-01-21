using System;

using NativeUwp_TestHarnessApp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace NativeUwp_TestHarnessApp.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return ViewModelLocator.Current.MainViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
