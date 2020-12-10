using System;


using UkCentralLaserPoC.Services;

namespace UkCentralLaserPoC.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class ViewModelLocator
    {
        private static ViewModelLocator _current;

        public static ViewModelLocator Current => _current ?? (_current = new ViewModelLocator());


    }
}
