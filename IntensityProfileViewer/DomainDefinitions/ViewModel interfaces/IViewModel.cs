//
// IViewModel.cs
//

using System.Collections.Generic ;

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

  //
  // A ViewModel has access to a Message Hub that it can use to
  //  - (A) publish messages that may be of interest to other elements in the tree,
  //  - (B) register its interest in handling certain messages
  //

  public interface IViewModel : System.ComponentModel.INotifyPropertyChanged
  { 
    // IRootViewModel GetRootViewModel<T> ( ) where T : IRootViewModel ;
    // IViewModel? GetParent<T> ( ) where T : IViewModel ;
    // IMessageHub MessageHub { get ; }
    IEnumerable<IViewModel> ChildViewModels { get ; }
  }

  //
  // A 'child' ViewModel has a reference to its Parent.
  //

  public interface IChildViewModel<TParent> : IViewModel where TParent : IViewModel
  { 
    TParent Parent { get ; }
    // IRootViewModel GetRootViewModel<T> ( ) where T : IRootViewModel ;
    // IViewModel? GetParent<T> ( ) where T : IViewModel ;
    // IMessageHub MessageHub { get ; }
  }

  //
  // This tag identifies the 'root' ViewModel in a tree.
  //
  // The root ViewModel holds an instance of a Messenger that acts as a hub for
  // message-based communication between the ViewModels in the tree.
  //
  // Any ViewModel can navigate to the Root.
  //

  public interface IRootViewModel : IViewModel
  {
    
  }



}
