//
// PvAccessTest.cs
//

using EpicsSharp.ChannelAccess.Client;

namespace LiveImageAcquisition
{

  public static class PvAccessTest
  {

    public static void TryConnecting<T> ( string ipAddress, string pvName )
    {
      CAClient channelAccessClient = new() ;
      channelAccessClient.Configuration.SearchAddress    = ipAddress ;
      channelAccessClient.Configuration.MaxSearchSeconds = 5 ;
      channelAccessClient.Configuration.WaitTimeout      = 5000 ;
      Channel<T> pv = channelAccessClient.CreateChannel<T>(pvName) ;
      var value = pv.Get<T>() ;
      System.Console.WriteLine(
        $"Get of '{pvName}' from '{ipAddress}' returned {value} at {System.DateTime.Now.TimeOfDay}"
      ) ;
      pv.MonitorChanged += (sender,value) => {
        System.Console.WriteLine(
          $"Monitor event : value of '{pvName}' changed to {value} at {System.DateTime.Now.TimeOfDay}"
        ) ;
      } ;
    }

    public static void TryConnecting_OLD_VERSION ( )
    {
      CAClient channelAccessClient = new() ;
      channelAccessClient.Configuration.SearchAddress    = "130.246.71.56" ;
      channelAccessClient.Configuration.MaxSearchSeconds = 5 ;
      channelAccessClient.Configuration.WaitTimeout      = 5000 ;
      Channel<double> pv = channelAccessClient.CreateChannel<double>("CLFMCC:HA0:CH2") ;
      var value = pv.Get<double>() ;
      System.Console.WriteLine(
        $"Value 'get' returned {value}"
      ) ;
      pv.MonitorChanged += (sender,value) => {
        System.Console.WriteLine(
          $"Value changed to {value}"
        ) ;
      } ;
    }

  }

}
