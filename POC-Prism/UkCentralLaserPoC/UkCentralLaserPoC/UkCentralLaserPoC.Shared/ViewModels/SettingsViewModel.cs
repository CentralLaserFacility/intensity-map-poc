using Prism.Commands;
using Prism.Regions;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using UkCentralLaserPoC.Core.Mvvm;
using UkCentralLaserPoC.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace UkCentralLaserPoC.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/pages/settings.md
    public class SettingsViewModel : ViewModelBase, INavigationAware
    {
        public SettingsViewModel(IThemeSelectorService themeSelectorService)
        {
            this.themeSelectorService = themeSelectorService;
            _elementTheme = themeSelectorService.Theme;
        }

        private ElementTheme _elementTheme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { SetProperty(ref _versionDescription, value); }
        }

        private ICommand _switchThemeCommand;
        private readonly IThemeSelectorService themeSelectorService;

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new DelegateCommand<ElementTheme?>(
                        async (param) =>
                        {
                            ElementTheme = param.Value;
                            await themeSelectorService.SetThemeAsync(param.Value);
                        });
                }
                
                return _switchThemeCommand;
            }
        }

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            await Task.CompletedTask;
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName";
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await InitializeAsync();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
