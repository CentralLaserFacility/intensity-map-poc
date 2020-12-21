using System;

using GalaSoft.MvvmLight.Ioc;

using NativeUwp_TestHarnessApp.Services;
using NativeUwp_TestHarnessApp.Views;

namespace NativeUwp_TestHarnessApp.ViewModels
{
   
    [Windows.UI.Xaml.Data.Bindable] // STEVET : ???
    public class ViewModelLocator
    {
        private static ViewModelLocator _current;

        public static ViewModelLocator Current => _current ?? (_current = new ViewModelLocator());

        private ViewModelLocator()
        {
            // STEVET : Why the different overloads here ??
            // And why isn't there an INavigationServiceEx ??
            SimpleIoc.Default.Register(() => new NavigationServiceEx());
            SimpleIoc.Default.Register<ShellViewModel>();

            Register<MainViewModel, MainPage>();
            Register<BlankViewModel, BlankPage>();
            Register<SettingsViewModel, SettingsPage>();

            // Register the types of our custom Pages and the associated ViewModels.
            Register<IntensityMapViewer.IntensityMapTestViewModel, IntensityMapViewer.IntensityMapTestPage>();
            Register<IntensityMapViewer.BitmapLoadTimingDemoViewModel, IntensityMapViewer.BitmapLoadTimingTestPage>();
        }

        // STEVET : 'GetInstance' creates an instance if necessary, otherwise returns the singleton instance.

        public SettingsViewModel SettingsViewModel => SimpleIoc.Default.GetInstance<SettingsViewModel>();

        public BlankViewModel BlankViewModel => SimpleIoc.Default.GetInstance<BlankViewModel>();

        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();

        public ShellViewModel ShellViewModel => SimpleIoc.Default.GetInstance<ShellViewModel>();

        public NavigationServiceEx NavigationService => SimpleIoc.Default.GetInstance<NavigationServiceEx>();

        // All of the ViewModels for our 'custom pages' are exposed as Properties of the ViewModelLocator.
        
        public IntensityMapViewer.IntensityMapTestViewModel IntensityMapTestViewModel
        => SimpleIoc.Default.GetInstance<IntensityMapViewer.IntensityMapTestViewModel>() ;

        public IntensityMapViewer.BitmapLoadTimingDemoViewModel BitmapLoadTimingDemoViewModel
        => SimpleIoc.Default.GetInstance<IntensityMapViewer.BitmapLoadTimingDemoViewModel>() ;

        // STEVET : Here we register a ViewModel type for a Page, and the corresponding View type.
        // The ViewModel type is registered with the IoC container so that we can subsequently
        // ask it to create instances ; and the ViewModel type name is configured with the
        // navigation service, together with the type of the View that it corresponds to.

        public void Register<VM, V>()
            where VM : class
        {
            SimpleIoc.Default.Register<VM>();

            NavigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}
