//
// Tests_02.cs
//

using Xunit ;
using FluentAssertions ;

// using Microsoft.Toolkit.Mvvm.Messaging ;

// https://docs.microsoft.com/en-us/dotnet/api/microsoft.toolkit.mvvm.messaging.imessengerextensions?view=win-comm-toolkit-dotnet-7.0
using static Microsoft.Toolkit.Mvvm.Messaging.IMessengerExtensions ;

// See tests here in the repo :
// https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/rel/7.0.0/UnitTests/UnitTests.Shared/Mvvm/Test_Messenger.cs

namespace MvvmToolkitMessengerExperiments_02
{

  // Here we're using the simplest form where we assume a default 'token'.

  public record MyMessageA ( string Text ) ;

  public record MyMessageB ( int Value ) ;

  public record MyMessageSender ( Microsoft.Toolkit.Mvvm.Messaging.IMessenger Messenger )
  {
    public void SendMessageA ( string messageText )
    {
      Messenger.Send(
        new MyMessageA(messageText)
      ) ;
    }
    public void SendMessageB ( int value )
    {
      Messenger.Send(
        new MyMessageB(value)
      ) ;
    }
  }

  public abstract class MyMessageRecipient
  {
    public string? A { get ; set ; }
    public int? B    { get ; set ; }
  }

  public class MyMessageRecipient_UsingRecipientInterface : MyMessageRecipient
  , Microsoft.Toolkit.Mvvm.Messaging.IRecipient<MyMessageA>
  , Microsoft.Toolkit.Mvvm.Messaging.IRecipient<MyMessageB>
  {
    public MyMessageRecipient_UsingRecipientInterface ( Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger )
    {
      messenger.RegisterAll(this) ;
    }
    public void Receive ( MyMessageA message )
    {
      A = message.Text ;
    }
    public void Receive ( MyMessageB message )
    {
      B = message.Value ;
    }
  }

  public class MyMessageRecipient_UsingLambdas : MyMessageRecipient
  {
    public MyMessageRecipient_UsingLambdas ( Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger )
    {
      messenger.Register<MyMessageA>(
        this,
        (recipient,message) => {
          A = message.Text ;
        }
      ) ;
      messenger.Register<MyMessageB>(
        this,
        (recipient,message) => {
          B = message.Value ;
        }
      ) ;
    }
  }

  public class Tests_02
  {

    [Fact]
    public void Test_01 ( )
    {
      // 'Default' is a thread-safe implementation built into the toolkit
      Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger = (
        Microsoft.Toolkit.Mvvm.Messaging
        //.WeakReferenceMessenger
        .StrongReferenceMessenger
        .Default 
      ) ;
      var messageSender = new MyMessageSender(messenger) ;
      var recipient = new MyMessageRecipient_UsingLambdas(messenger) ;
      var recipient2 = new MyMessageRecipient_UsingLambdas(messenger) ;
      messageSender.SendMessageA("hello") ;
      messageSender.SendMessageB(123) ;
      recipient.A.Should().Be("hello") ;
      recipient.B.Should().Be(123) ;
      recipient2.A.Should().Be("hello") ;
      recipient2.B.Should().Be(123) ;
    }

    [Fact]
    public void Test_02 ( )
    {
      // 'Default' is a thread-safe implementation built into the toolkit
      Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger = (
        Microsoft.Toolkit.Mvvm.Messaging
        //.WeakReferenceMessenger
        .StrongReferenceMessenger
        .Default 
      ) ;
      var messageSender = new MyMessageSender(messenger) ;
      var recipient = new MyMessageRecipient_UsingRecipientInterface(messenger) ;
      messageSender.SendMessageA("hello") ;
      recipient.A.Should().Be("hello") ;
      messageSender.SendMessageB(123) ;
      recipient.B.Should().Be(123) ;
    }

  }

}
