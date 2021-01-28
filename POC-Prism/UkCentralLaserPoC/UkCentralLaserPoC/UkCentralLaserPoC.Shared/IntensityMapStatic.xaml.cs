using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UkCentralLaserPoC.Shared
{
    public sealed partial class IntensityMapStatic : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public IntensityMapStatic()
        {
            this.InitializeComponent();
        }

        public CyclicSelector<(ImageSource, string)> Source
        {
            get { return (CyclicSelector<(ImageSource, string)>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(CyclicSelector<(ImageSource, string)>), typeof(IntensityMapStatic), 
                new PropertyMetadata(new CyclicSelector<(ImageSource, string)>(), OnSourcePropertyChanged));

        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IntensityMapStatic intensityMap = d as IntensityMapStatic;
            var image = intensityMap.FindName("m_image") as Image;
            var images = e.NewValue as CyclicSelector<(ImageSource, string)>;
            var item = images.GetCurrent_MoveNext();
            image.Source = item.Item1;

            var label = intensityMap.FindName("m_staticImageLabel") as TextBlock;
            label.Text = item.Item2;
        }

        public void OnCycleClicked(object obj, RoutedEventArgs args)
        {
            var item = Source.GetCurrent_MoveNext();
            m_image.Source = item.Item1;
            m_staticImageLabel.Text = item.Item2;
        }
    }
}
