//
// IntensityMapTestPage.cs
//

namespace UkCentralLaserPoC.IntensityMap
{
    public partial class IntensityMapTestView : Windows.UI.Xaml.Controls.UserControl
    {
        public IntensityMapTestViewModel? ViewModel => DataContext as IntensityMapTestViewModel;

        public IntensityMapTestView()
        {
            this.InitializeComponent();
        }
    }
}
