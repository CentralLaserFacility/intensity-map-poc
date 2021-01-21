//
// MainPage.xaml.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace UwpUtilities_TestApp
{

  public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
  {

    private IntensityMapViewer.DisplayPanelViewModel ViewModel = new IntensityMapViewer.DisplayPanelViewModel() ;

    private Windows.UI.Xaml.DispatcherTimer m_timer = new Windows.UI.Xaml.DispatcherTimer(){
      Interval = System.TimeSpan.FromMilliseconds(100)
    } ;

    public Windows.UI.Xaml.Media.ImageSource ImageSource { get ; private set ; }

    public MainPage ( )
    {
      this.InitializeComponent() ;
      this.Loaded += MainPage_Loaded ;
      ImageSource = UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
        ViewModel.CurrentSource.MostRecentlyAcquiredIntensityMap
      ) ;
    }

    //
    // This helper is invoked via x:Bind in the XAML.
    // The expectation is that it'll be triggered every time a change
    // is detected in one of the parameters supplied in the x:Bind call.
    //
    // Hmm, looking at the app's memory usage we see that allocating all these images
    // takes quite a bit of memory, and the GC kicks in every few seconds. The collection
    // takes enough time that we see a glitch in the otherwise continuous display.
    // It'd be a good idea to Dispose the existing image prior to creating a new one,
    // as that would smooth over the GC work which is currently occurring in one chunk
    // every few seconds.
    //

    private Windows.UI.Xaml.Media.Imaging.WriteableBitmap m_writeableBitmap ;

    private Windows.UI.Xaml.Media.ImageSource CreateImageSource ( 
      IntensityMapViewer.IIntensityMap   intensityMap,
      IntensityMapViewer.ColourMapOption colourMapOption
    ) 
    => UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
      intensityMap,
      colourMapOption
    ) ;

    private void MainPage_Loaded ( object sender, Windows.UI.Xaml.RoutedEventArgs e )
    {
      // Just so we can verify that we can create a VM and all its children,
      // by setting a breakpoint here and looking with the debugger ...
      // var mainViewModel = new IntensityMapViewer.DisplayPanelViewModel() ;
    }

    private int m_nClicks = -1 ;

    private IntensityMapViewer.IntensityMapSequence m_intensityMapSequence_A = 
    IntensityMapViewer.IntensityMapSequence.CreateInstance_WithProgressivelyIncreasingSincFactor(
      60,
      (5.0,10.0)
    ) ;

    private IntensityMapViewer.IntensityMapSequence m_intensityMapSequence_B = 
    IntensityMapViewer.IntensityMapSequence.CreateInstance_WithNoiseAdded(
      60,
      sincFactor     : 10.0,
      noiseAmplitude : 64
    ) ;

    private IntensityMapViewer.IntensityMapSequence m_intensityMapSequence_C = 
    IntensityMapViewer.IntensityMapSequence.CreateInstance_BlobRotatingAroundCircle(
      60
    ) ;

    private void DynamicImageButtonClicked ( )
    {
      if ( m_timer.IsEnabled is false )
      {
        m_timer.Start() ;
        m_timer.Tick += (s,e) => {
          m_dynamicImage.Source = UwpUtilities.BitmapHelpers.LoadOrCreateWriteableBitmap(
            // IntensityMapViewer.IntensityMapHelpers.CreateSynthetic_UsingSincFunction_Cyclic() 
            ref m_writeableBitmap,
            m_intensityMapSequence_C.GetCurrent_MoveNext()
          ) ; 
        } ;
      }
    }

    private void StaticImageButtonClicked ( )
    {
      // Hacky tests, just to try stuff out ...

      if ( m_nClicks is -1 )
      {
        (m_image.Source,m_imageLabel.Text) = (
          UwpUtilities.BitmapHelpers_ForTesting.CreateWriteableBitmap_ForTesting_A_SolidColour(
            (0xff,0x00,0x00) // red
          ), 
          "Expecting solid red"
        ) ;
        m_nClicks++ ;
        return ;
      }

      m_nClicks++ ;
      if ( m_nClicks > 8 )
      {
        m_nClicks = 1 ;
      }

      // Here we trigger a change in the 'ColourMapOption' property,
      // and since that property is mentioned as an x:Bind parameter
      // in the XAML, the Source gets re-evaluated. Note that Mode should be
      // set to 'OneWay' as the default binding mode is 'OneTime'.
      // https://docs.microsoft.com/en-us/windows/uwp/data-binding/function-bindings
      ViewModel.ImagePresentationSettings.ColourMapOption = (
        m_nClicks % 2 == 0
        ? IntensityMapViewer.ColourMapOption.GreyScale
        : IntensityMapViewer.ColourMapOption.JetColours
      ) ;

      m_image.Source = m_nClicks switch 
      {
      1 => UwpUtilities.BitmapHelpers_ForTesting.CreateWriteableBitmap_ForTesting_A_SolidColour(
        (0xff,0x00,0x00) // red
      ), 
      2 => UwpUtilities.BitmapHelpers_ForTesting.CreateWriteableBitmap_ForTesting_A_SolidColour(
        (0x00,0xff,0x00) // green
      ), 
      3 => UwpUtilities.BitmapHelpers_ForTesting.CreateWriteableBitmap_ForTesting_A_SolidColour(
        (0x00,0x00,0xff) // blue
      ), 
      4 => UwpUtilities.BitmapHelpers_ForTesting.CreateWriteableBitmap_ForTesting_B(
        (x,y) => (0xff,0x00,0x00)
      ),
      5 => UwpUtilities.BitmapHelpers_ForTesting.CreateWriteableBitmap_ForTesting_C(0xff00ff00), // G
      6 => UwpUtilities.BitmapHelpers_ForTesting.CreateWriteableBitmap_ForTesting_C(0xff0000ff), // B
      7 => UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
             new IntensityMapViewer.DisplayPanelViewModel(
             ).CurrentSource.MostRecentlyAcquiredIntensityMap
           ),
      8 => UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
        new IntensityMapViewer.IntensityMap.CreatedFromSincFunction(
          sincFactor : 5
        )
      ),
      _ => throw new System.ApplicationException()
      } ;
      return ;
    
      // m_image.Source = UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
      //   IntensityMapViewer.IntensityMapHelpers.CreateSynthetic_UsingSincFunction_Cyclic() 
      // ) ;
      // return ;
    
    }

    // private void ButtonClicked ( )
    // {
    // 
    //   // Hacky tests, just to try stuff out ...
    // 
    //   m_nClicks++ ;
    //   ViewModel.ImagePresentationSettings.ColourMapOption = (
    //     m_nClicks % 2 == 0
    //     ? IntensityMapViewer.ColourMapOption.GreyScale
    //     : IntensityMapViewer.ColourMapOption.JetColours
    //   ) ;
    //   if ( m_nClicks > 8 )
    //   {
    //     m_nClicks = 1 ;
    //   }
    //   m_image.Source = m_nClicks switch 
    //   {
    //   1 => UwpUtilities.BitmapHelpers_old_01.CreateWriteableBitmap_ForTesting_A(), 
    //   2 => UwpUtilities.BitmapHelpers_old_01.CreateWriteableBitmap_ForTesting_B(), 
    //   3 => UwpUtilities.BitmapHelpers_old_01.CreateWriteableBitmap_ForTesting_C(), 
    //   4 => UwpUtilities.BitmapHelpers_old_01.CreateWriteableBitmap_ForTesting_D(0xffff0000), // R
    //   5 => UwpUtilities.BitmapHelpers_old_01.CreateWriteableBitmap_ForTesting_D(0xff00ff00), // G
    //   6 => UwpUtilities.BitmapHelpers_old_01.CreateWriteableBitmap_ForTesting_D(0xff0000ff), // B
    //   7 => UwpUtilities.BitmapHelpers_old_01.CreateWriteableBitmap(
    //          new IntensityMapViewer.DisplayPanelViewModel(
    //          ).CurrentSource.MostRecentlyAcquiredIntensityMap
    //        ),
    //   8 => UwpUtilities.BitmapHelpers_old_01.CreateWriteableBitmap(
    //     IntensityMapViewer.IntensityMapHelpers.CreateSynthetic_UsingSincFunction(
    //       sincFactor : 5
    //     )
    //   ),
    //   _ => throw new System.ApplicationException()
    //   } ;
    //   return ;
    // 
    //   if ( m_timer.IsEnabled is false )
    //   {
    //     m_timer.Start() ;
    //     m_timer.Tick += (s,e) => {
    //       m_image.Source = UwpUtilities.BitmapHelpers_old_01.CreateWriteableBitmap(
    //         IntensityMapViewer.IntensityMapHelpers.CreateSynthetic_UsingSincFunction_Cyclic() 
    //       ) ; 
    //     } ;
    //   }
    //   return ;
    // 
    //   m_image.Source = UwpUtilities.BitmapHelpers_old_01.CreateWriteableBitmap(
    //     IntensityMapViewer.IntensityMapHelpers.CreateSynthetic_UsingSincFunction_Cyclic() 
    //   ) ;
    //   return ;
    // 
    // }

  }

}
