//
// IViewModel.cs
//

namespace IntensityProfileViewer
{

  //
  // A 'message hub' is an object that can
  //  - (A) accept 'registrations-of-interest'

  public interface IMessageHub
  {
    void AcceptMessageForPublication<TMessage> ( TMessage message ) where TMessage : class ;
    void RegisterInterestInPublishedMessagesOfType<TMessage> ( System.Action<TMessage> onMessage ) where TMessage : class ;
  }

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
  { 
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
