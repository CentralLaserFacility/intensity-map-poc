using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;

namespace UkCentralLaserPoC.IntensityMap
{
    public class IntensityMapModule: IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("IntensityMapRegion", () => containerProvider.Resolve<IntensityMapDynamicView>());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterScoped<IntensityMapDynamicViewModel>();
        }
    }
}
