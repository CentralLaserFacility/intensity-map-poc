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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Experiments_01_UWP
{

  public class MouseEventHandlerTest_ViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
  {

    private readonly MouseEventHandler m_mouseEventHandler ;

    public MouseEventHandlerTest_ViewModel ( UIElement target )
    {
      m_mouseEventHandler = new MouseEventHandler(
        target,
        RespondToPanZoomGesture
      ) {
        IncomingMouseEventReceived = (mouseEventDescriptor) => {
          Common.DebugHelpers.WriteDebugLines(
            $"{mouseEventDescriptor}"
          ) ;
        }
      } ;
    }

    private void RespondToPanZoomGesture ( MouseGesture panZoomGesture )
    {
      Common.DebugHelpers.WriteDebugLines(
        $"{panZoomGesture}"
      ) ;
    }

  }

  public sealed partial class MouseEventHandlerTest_UserControl : UserControl
  {

    private MouseEventHandlerTest_ViewModel ViewModel ;

    private MouseEventHandler m_mouseEventHandler ;

    public MouseEventHandlerTest_UserControl ( )
    {
      this.InitializeComponent() ;
      ViewModel = new MouseEventHandlerTest_ViewModel(
        m_target
      ) ;
    }

  }

}
