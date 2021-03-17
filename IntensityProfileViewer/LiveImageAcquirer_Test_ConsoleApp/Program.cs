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

      System.Console.WriteLine("Press ENTER to exit ...") ;
      System.Console.ReadLine() ;

    }

  }

}
