using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using UkCentralLaserPoC.Core.Mvvm;
using Windows.UI.Xaml.Controls;

namespace UkCentralLaserPoC.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            Title = "Main Page";
        }

    }
}
