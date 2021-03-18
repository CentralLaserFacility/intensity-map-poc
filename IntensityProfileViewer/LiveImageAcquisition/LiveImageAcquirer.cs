//
// LiveImageAcquirer.cs
//

using EpicsSharp.ChannelAccess.Client;
using System;
using System.Collections.Generic;

namespace LiveImageAcquisition
{

  //
  // A quick hack to get something running ...
  //

  public class LiveImageAcquirer
  {

    public string PvName { get ; }

    public bool IsActive { get ; set ; }

    public int ImageWidth => m_imageSizeX ;

    public int ImageHeight => m_imageSizeY ;

    public int MostRecentlyAcquiredImageNumber { get ; private set ; }

    public System.DateTime MostRecentlyAcquiredImageTimeStamp { get ; private set ; }

    public byte[] MostRecentlyAcquiredImageBytes => m_imageBytes ;

    // This gets invoked on a worker thread !!!

    public System.Action<LiveImageAcquirer>? NewImageAvailable ;

    // -----------------------------

    private CAClient? m_channelAccessClient ;

    private Channel<byte[]>? m_image_PV ;
    private Channel<int>?    m_sizeX_PV ;
    private Channel<int>?    m_sizeY_PV ;

    private int    m_imageSizeX = 0 ;
    private int    m_imageSizeY = 0 ;
    private byte[] m_imageBytes = Array.Empty<byte>() ;

    public string IpAddress { get ; private set ; }

    public static IReadOnlyList<string> AvailableSourcePvNames 
    => new [] {
      "GEM:N_AMP:cam1",
      "GEM:N_AMP:cam2",
      "GEM:N_AMP:cam3",
      "GEM:N_AMP:cam4" 
    } ;

    public LiveImageAcquirer ( 
      string         ipAddress, 
      string         pvName, 
      System.Action<LiveImageAcquirer>? newImageAvailable = null
    ) {
      IpAddress = ipAddress ;
      PvName = pvName ;
      NewImageAvailable = newImageAvailable ;
    }

    public void Connect ( )
    { 

      m_channelAccessClient = new() ;
      m_channelAccessClient.Configuration.SearchAddress    = IpAddress ;
      m_channelAccessClient.Configuration.MaxSearchSeconds = 5 ;
      m_channelAccessClient.Configuration.WaitTimeout      = 5000 ;

      m_sizeX_PV = m_channelAccessClient.CreateChannel<int>($"{PvName}:image1:ArraySize0_RBV") ;
      m_sizeX_PV.MonitorChanged += (sender,sizeX) => m_imageSizeX = sizeX ;

      m_sizeY_PV = m_channelAccessClient.CreateChannel<int>($"{PvName}:image1:ArraySize1_RBV") ;
      m_sizeY_PV.MonitorChanged += (sender,sizeY) => m_imageSizeY = sizeY ;

      m_image_PV = m_channelAccessClient.CreateChannel<byte[]>($"{PvName}:image1:ArrayData") ;
      m_image_PV.MonitorChanged += (sender,imageBytes) => {
        m_imageBytes = imageBytes ;
        MostRecentlyAcquiredImageNumber++ ;
        MostRecentlyAcquiredImageTimeStamp = System.DateTime.Now ;
        NewImageAvailable?.Invoke(this) ;
      } ;

    }

  }

}
