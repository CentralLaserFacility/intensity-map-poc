//
// HowToDrawProfileGraph.cs
//

namespace IntensityMapViewer
{

  //
  // This makes a difference when we're zoomed in a long way
  // and each pixel occupies a significant space.
  //

  public enum HowToDrawProfileGraph {
    DrawSolidBarsCentredOnEachPixel,
    DrawLinesBetweenAdjacentPixels
  }

}
