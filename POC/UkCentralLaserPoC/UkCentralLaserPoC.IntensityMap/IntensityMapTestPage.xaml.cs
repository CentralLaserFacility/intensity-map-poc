//
// IntensityMapTestPage.cs
//

namespace UkCentralLaserPoC.IntensityMap
{
    public partial class IntensityMapTestPage : Windows.UI.Xaml.Controls.UserControl
    {
        public IntensityMapTestViewModel ViewModel => new IntensityMapTestViewModel();

        public IntensityMapTestPage()
        {
            this.InitializeComponent();
        }
    }
}
