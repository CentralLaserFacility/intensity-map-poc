//
// Program.cs
//

using System.Linq ;

namespace LiveImageAcquirer_Test_ConsoleApp
{

  class Program
  {

    static void Main ( string[] args )
    {

      try
      {

        int option = 1 ;
        switch ( option )
        {
        case 1:
          LiveImageAcquisition.PvAccessTest.TryConnecting<double>(
            "130.246.71.56",
            "CLFMCC:HA0:CH2"
          ) ;
          return ;
        case 2:
          LiveImageAcquisition.PvAccessTest.TryConnecting<string>(
            "130.246.71.15",
            "CDG:TEST_STR"
          ) ;
          return ;
        }

        System.Console.WriteLine(
          $"Creating LiveImageAcquirer on thread #{System.Environment.CurrentManagedThreadId}"
        ) ;

        var liveImageAcquirer = new LiveImageAcquisition.LiveImageAcquirer(
          "1.2.3.4",
          LiveImageAcquisition.LiveImageAcquirer.AvailableSourcePvNames[0],
          (liveImageAcquirer) => System.Console.WriteLine(
            $"Image #{liveImageAcquirer.MostRecentlyAcquiredImageNumber}"
          + $" on thread #{System.Environment.CurrentManagedThreadId}"
          + $" : {liveImageAcquirer.ImageWidth}x{liveImageAcquirer.ImageWidth}"
          + $" {liveImageAcquirer.MostRecentlyAcquiredImageBytes?.Length??-1} bytes"
          + $" at {liveImageAcquirer.MostRecentlyAcquiredImageTimeStamp.TimeOfDay}"
          )
        ) ;

        liveImageAcquirer.Connect() ;
      }
      catch ( System.Exception x )
      {
        System.Console.WriteLine(
          $"Exception : {x}"
        ) ;
      }
      finally
      {
        System.Console.WriteLine("Waiting for ENTER ...") ;
        System.Console.ReadLine() ;
      }

    }

  }

}
