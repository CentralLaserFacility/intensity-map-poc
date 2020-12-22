using Microsoft.Extensions.DependencyInjection;
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


        private void ConfigureServices()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<ShellViewModel>();
            serviceCollection.AddScoped<MainViewModel>();
            serviceCollection.AddScoped<SettingsViewModel>();
            serviceCollection.AddSingleton<NavigationServiceEx>();

            container = serviceCollection.BuildServiceProvider();
        }

        public void Register<VM, V>() where VM : class
        {
            NavigationService.Configure(typeof(VM).FullName, typeof(V));
        }


        public ShellViewModel ShellViewModel => container.GetService<ShellViewModel>();

        public NavigationServiceEx NavigationService => container.GetService<NavigationServiceEx>();

        public SettingsViewModel SettingsViewModel => container.GetService<SettingsViewModel>();

        public MainViewModel MainViewModel => container.GetService<MainViewModel>();

    }
}
