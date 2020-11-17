//
// IntensityMap.cs
//

namespace IntensityMapViewer
{

  //
  // Hmm, tacky to have this as a static class ...
  //

  public static class ColourMapper 
  {

    //
    // This will be invoked whenever we need to convert the data from an ImageMap
    // into a displayable set of pixels.
    //
    // Potentially we could cache the results in an LRU container,
    // so as to avoid repeatedly computing the same transformation ?
    //

    //
    // Formula for the 'JET' mapping ...
    // https://stackoverflow.com/questions/7706339/grayscale-to-red-green-blue-matlab-jet-color-scale
    //

    public static System.Collections.Generic.IReadOnlyList<System.Drawing.Color> MapByteValuesToRgb ( 
      System.Collections.Generic.IEnumerable<byte> byteValues,
      ColourMapOption                              colourMapOption 
    ) {
      throw new System.NotImplementedException() ;
    }

  }

}
