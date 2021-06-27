//
// IMessageHub.cs
//

namespace IntensityProfileViewer
{

  public interface IPublishesMessages<TMessage> where TMessage : class
  {
    void PublishOutgoingMessage<TSender> ( 
      TSender  sender, 
      TMessage message 
    ) where TSender : class ;
  }

  public interface IRespondsToIncomingMessages<TMessage> where TMessage : class
  {
    void RegisterInterestInIncomingMessages<TRecipient> ( 
      TRecipient              recipient,
      System.Action<TMessage> onMessage 
    ) where TRecipient : class ;
  }

  //
  // A message hub can maintain a record of which instances are
  // publishing messages of various types, and who has registered
  // their interest in receiving them.
  //
  // It can also maintain a record of the messages that have been sent.
  //
  // All this may be pretty useful for debugging.
  //

  public interface IMessageHub<TMessage> where TMessage : class
  , IPublishesMessages<TMessage>
  , IRespondsToIncomingMessages<TMessage>
  {  
  }

}
