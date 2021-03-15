//
// IViewModel.cs
//

namespace IntensityProfileViewer
{

  //
  // This 'IViewModel' interface acts as a 'tag' to indicate
  // that the concrete class will be a 'ViewModel' inheriting from
  // the Community Toolkit 'ObservableObject' base class.
  //
  // Each ViewModel will have a 'View', typically implemented as a UserControl.
  //
  // When any 'property' suffers a change, a PropertyChange notification is raised
  // and the appropriate elements in the View should respond by repainting themselves.
  //

  public interface IViewModel : System.ComponentModel.INotifyPropertyChanged
  { }

}
