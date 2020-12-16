using Microsoft.Extensions.DependencyInjection;
using System;


using UkCentralLaserPoC.Services;
using UkCentralLaserPoC.Views;

namespace UkCentralLaserPoC.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ConfigureServices();
            Register<MainViewModel, MainPage>();
            Register<SettingsViewModel, SettingsPage>();
        }

        private ServiceProvider container;

        private static ViewModelLocator _current;

        public static ViewModelLocator Current => _current ?? (_current = new ViewModelLocator());

        public ShellViewModel ShellViewModel => container.GetService<ShellViewModel>();

        public NavigationServiceEx NavigationService => container.GetService<NavigationServiceEx>();

        private void ConfigureServices()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<ShellViewModel>();
            serviceCollection.AddSingleton<NavigationServiceEx>();

            container = serviceCollection.BuildServiceProvider();
        }

        public void Register<VM, V>() where VM : class
        {
            NavigationService.Configure(typeof(VM).FullName, typeof(V));
        }

    }
}
