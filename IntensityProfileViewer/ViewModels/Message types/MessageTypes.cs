//
// MessageTypes.cs
//

namespace IntensityProfileViewer
{

  //
  // These messages are published by ViewModels and may be acted on
  // by other ViewModels via the 'Messenger' mechanism
  // provided by the MVVM Toolkit.
  //

  public record IntensityVisualisationChangedMessage ( ) ;

  public record NewIntensityMapAcquiredMessage ( ) ;

  public record ReferencePositionChangedMessage ( int ? X, int ? Y ) ;

  public record PointerPositionChangedMessage ( int ? X, int ? Y ) ;

  // This is not specific to the Viewer ...

  public record NumericValueChangedMessage ( ) ;

}
