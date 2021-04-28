//
// EpicsImageSource.cs
//

namespace IntensityProfileViewer
{

  public record ImageDataFromEpics ( 
    int    Height,
    int    Width,
    byte[] ImageBytes
  ) ;

  public interface IEpicsImageSource : System.IDisposable
  {
    System.Action<ImageDataFromEpics>? NewImageAvailable { get ; set ; }
  }

  public class EpicsImageSource : IEpicsImageSource
  {

    public System.Action<ImageDataFromEpics>? NewImageAvailable { get ; set ; }

    public EpicsImageSource ( string pvBaseName ) // Might need separate PV names for height/width/data ??
    {
      // Connect to the PV's, and set up callbacks that tell us when a new image is available ...
    }

    public void Dispose ( )
    {
      // Close down the connections etc ...
      throw new System.NotImplementedException() ;
    }

  }
    
}
