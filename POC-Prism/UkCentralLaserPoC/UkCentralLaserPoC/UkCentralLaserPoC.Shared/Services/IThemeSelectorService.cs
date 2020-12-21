using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace UkCentralLaserPoC.Services
{
    public interface IThemeSelectorService
    {
        ElementTheme Theme { get; set; }
        Task InitializeAsync();
        Task SetRequestedThemeAsync();
        Task SetThemeAsync(ElementTheme theme);
    }
}
