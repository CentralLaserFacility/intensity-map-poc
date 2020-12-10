using System;

using UkCentralLaserPoC.ViewModels;

using Windows.UI.Xaml.Controls;

namespace UkCentralLaserPoC.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel => new MainViewModel();
        public MainPage()
        {
            InitializeComponent();
        }
    }
}
