//
// .cs
//

namespace IntensityProfileViewer
{

  public record IntensityVisualisationChangedMessage ( ) ;

  public record NewIntensityMapAcquiredMessage ( ) ;

  public record ReferencePositionChangedMessage ( int ? X, int ? Y ) ;

  public record NumericValueChangedMessage ( ) ;

  public record PointerPositionChangedMessage ( int ? X, int ? Y ) ;

}
