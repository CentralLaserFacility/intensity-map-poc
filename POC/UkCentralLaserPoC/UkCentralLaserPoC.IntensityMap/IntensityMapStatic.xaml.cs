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
using Common;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UkCentralLaserPoC.IntensityMap
{
    public sealed partial class IntensityMapStatic : UserControl
    {
        public IntensityMapStatic()
        {
            this.InitializeComponent();
        }

        public ImageSource StaticImageSource
        {
            get { return (ImageSource)GetValue(StaticImageSourceProperty); }
            set { SetValue(StaticImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StaticImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StaticImageSourceProperty =
            DependencyProperty.Register("StaticImageSource", typeof(int), typeof(IntensityMapStatic), new PropertyMetadata(null, OnStaticImageSourceChanged));

        private static void OnStaticImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IntensityMapStatic? intensityMap = d as IntensityMapStatic;
            if (intensityMap != null)
            {
                var image = intensityMap.FindName("m_image") as Image;
                image.Source = e.NewValue as ImageSource;
            }
        }

        public CyclicSelector<(ImageSource, string)> StaticImagesSource
        {
            get { return (CyclicSelector<(ImageSource, string)>)GetValue(StaticImagesSourceProperty); }
            set { SetValue(StaticImagesSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StaticImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StaticImagesSourceProperty =
            DependencyProperty.Register("StaticImagesSource", typeof(int), typeof(IntensityMapStatic), new PropertyMetadata(new CyclicSelector<(ImageSource, string)>(), OnStaticImagesSourceChanged));

        private static void OnStaticImagesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IntensityMapStatic? intensityMap = d as IntensityMapStatic;
            if (intensityMap != null)
            {
                var image = intensityMap.FindName("m_image") as Image;
                var images = e.NewValue as CyclicSelector<(ImageSource, string)>;
                var currentItem = images.GetCurrent_MoveNext();
                image.Source = currentItem.Item1;
            }
        }

        public void OnCycleClicked(object obj, RoutedEventArgs e)
        {
            var item = StaticImagesSource.GetCurrent_MoveNext();
            m_image.Source = item.Item1;
        }

    }
}
