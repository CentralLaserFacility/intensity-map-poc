//
// MainPage.xaml.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace UwpUtilities_TestApp
{

  public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
  {

    private Windows.UI.Xaml.DispatcherTimer m_timer = new Windows.UI.Xaml.DispatcherTimer(){
      Interval = System.TimeSpan.FromMilliseconds(200)
    } ;

    public MainPage ( )
    {
      this.InitializeComponent() ;
      this.Loaded += MainPage_Loaded ;
    }

    private void MainPage_Loaded ( object sender, Windows.UI.Xaml.RoutedEventArgs e )
    {
      // Just so we can verify that we can create a VM and all its children,
      // by setting a breakpoint here and looking with the debugger ...
      // var mainViewModel = new IntensityMapViewer.DisplayPanelViewModel() ;
    }

    private int m_nClicks = 0 ;

    private void ButtonClicked ( )
    {

      // Hacky tests, just to try stuff out ...

      m_nClicks++ ;
      if ( m_nClicks > 8 )
      {
        m_nClicks = 1 ;
      }
      m_image.Source = m_nClicks switch 
      {
      1 => UwpUtilities.BitmapHelpers.CreateWriteableBitmap_ForTesting_A(), 
      2 => UwpUtilities.BitmapHelpers.CreateWriteableBitmap_ForTesting_B(), 
      3 => UwpUtilities.BitmapHelpers.CreateWriteableBitmap_ForTesting_C(), 
      4 => UwpUtilities.BitmapHelpers.CreateWriteableBitmap_ForTesting_D(0xffff0000), // R
      5 => UwpUtilities.BitmapHelpers.CreateWriteableBitmap_ForTesting_D(0xff00ff00), // G
      6 => UwpUtilities.BitmapHelpers.CreateWriteableBitmap_ForTesting_D(0xff0000ff), // B
      7 => UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
             new IntensityMapViewer.DisplayPanelViewModel(
             ).CurrentSource.MostRecentlyAcquiredIntensityMap
           ),
      8 => UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
        IntensityMapViewer.IntensityMap.CreateSynthetic_UsingSincFunction(
          sincFactor : 5
        )
      ),
      _ => throw new System.ApplicationException()
      } ;
      return ;

      if ( m_timer.IsEnabled is false )
      {
        m_timer.Start() ;
        m_timer.Tick += (s,e) => {
          m_image.Source = UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
            IntensityMapViewer.IntensityMap.CreateSynthetic_UsingSincFunction_Cyclic() 
          ) ; 
        } ;
      }
      return ;

      m_image.Source = UwpUtilities.BitmapHelpers.CreateWriteableBitmap(
        IntensityMapViewer.IntensityMap.CreateSynthetic_UsingSincFunction_Cyclic() 
      ) ;
      return ;


    }

  }

}
