//
// SystemConstants.cs
//

namespace IntensityMapViewer
{

  public partial class SystemConstants : ISystemConstants
  {

    public float MaxZoomInFactor ( PanAndZoomMode zoomMode ) => throw new System.NotImplementedException() ;

    public float MaxZoomOutFactor ( PanAndZoomMode zoomMode ) => throw new System.NotImplementedException() ;

    public float ProfileGraphLineThickness {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public System.Drawing.Color ProfileGraphLineColour {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    public byte MaxAmplitudeOfNoiseToAddToSynthesiseImageMaps {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

    // Maybe this should be a User Preference ...

    public string DataFilesDirectory {
      get => throw new System.NotImplementedException() ;
      set => throw new System.NotImplementedException() ;
    }

  }

  public partial class SystemConstants
  {

    public static readonly SystemConstants Default = new() ;

    private SystemConstants ( )
    {
      throw new System.NotImplementedException() ;
    }

  }

}
