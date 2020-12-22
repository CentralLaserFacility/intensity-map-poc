using System;

using UkCentralLaserPoC.ViewModels;

using Windows.UI.Xaml.Controls;

namespace UkCentralLaserPoC.Views
{
    public sealed partial class MainView : Page
    {
        public MainViewModel ViewModel => DataContext as MainViewModel;

        public MainView()
        {
            InitializeComponent();
        }
    }
}
