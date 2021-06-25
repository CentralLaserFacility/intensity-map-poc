//
// Tests_01.cs
//

using Microsoft.Toolkit.Mvvm.Messaging;
using Xunit ;
using FluentAssertions ;

// using Microsoft.Toolkit.Mvvm.Messaging ;

// https://docs.microsoft.com/en-us/dotnet/api/microsoft.toolkit.mvvm.messaging.imessengerextensions?view=win-comm-toolkit-dotnet-7.0
using static Microsoft.Toolkit.Mvvm.Messaging.IMessengerExtensions ;

// See tests here in the repo :
// https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/rel/7.0.0/UnitTests/UnitTests.Shared/Mvvm/Test_Messenger.cs

namespace MvvmToolkitMessengerExperiments_01
{

  // Here we're using the simplest form where we assume a default 'token'.

  public record MyMessageA ( string Text ) ;

  public record MyMessageB ( int Value ) ;

  public static class MyMessageSender
  {
    public static void SendMessageA ( string messageText )
    {
      Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(
        new MyMessageA(messageText)
      ) ;
    }
    public static void SendMessageB ( int value )
    {
      Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(
        new MyMessageB(value)
      ) ;
    }
  }

  public abstract class MyMessageReceiver
  {
    public string A { get ; set ; } = "" ;
    public int B    { get ; set ; } = -1 ;
  }

  public class MyMessageReceiver_UsingRecipientInterface 
  : MyMessageReceiver
  , Microsoft.Toolkit.Mvvm.Messaging.IRecipient<MyMessageA>
  , Microsoft.Toolkit.Mvvm.Messaging.IRecipient<MyMessageB>
  {
    public MyMessageReceiver_UsingRecipientInterface ( )
    {
      Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.RegisterAll(this) ;
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

  public class MyMessageReceiver_UsingLambdas : MyMessageReceiver
  {
    public MyMessageReceiver_UsingLambdas ( )
    {
      Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register<MyMessageA>(
        this,
        (recipient,message) => {
          A = message.Text ;
        }
      ) ;
      Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register<MyMessageB>(
        this,
        (recipient,message) => {
          B = message.Value ;
        }
      ) ;
    }
  }

  public class Tests_01
  {

    [Fact]
    public void Test_01 ( )
    {
      var receiver = new MyMessageReceiver_UsingLambdas() ;
      MyMessageSender.SendMessageA("hello") ;
      receiver.A.Should().Be("hello") ;
      MyMessageSender.SendMessageB(123) ;
      receiver.B.Should().Be(123) ;
    }

    [Fact]
    public void Test_02 ( )
    {
      var receiver = new MyMessageReceiver_UsingRecipientInterface() ;
      MyMessageSender.SendMessageA("hello") ;
      receiver.A.Should().Be("hello") ;
      MyMessageSender.SendMessageB(123) ;
      receiver.B.Should().Be(123) ;
    }

  }

}
