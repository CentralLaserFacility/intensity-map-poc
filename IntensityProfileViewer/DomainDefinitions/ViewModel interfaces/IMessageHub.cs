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
  // A message hub can keep a record of which instances have
  // published messages, and who is registered to receive them.
  //

  public interface IMessageHub<TMessage> where TMessage : class
  , IPublishesMessages<TMessage>
  , IRespondsToIncomingMessages<TMessage>
  {  
  }

}
