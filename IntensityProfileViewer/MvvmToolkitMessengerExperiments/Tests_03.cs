//
// Tests_03.cs
//

using Xunit ;
using FluentAssertions ;

// using Microsoft.Toolkit.Mvvm.Messaging ;

// https://docs.microsoft.com/en-us/dotnet/api/microsoft.toolkit.mvvm.messaging.imessengerextensions?view=win-comm-toolkit-dotnet-7.0
using static Microsoft.Toolkit.Mvvm.Messaging.IMessengerExtensions ;
using System.Threading.Tasks;
using System.Threading;

// using Microsoft.Toolkit.Mvvm.Messaging.Messages;

// See tests here in the repo :
// https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/rel/7.0.0/UnitTests/UnitTests.Shared/Mvvm/Test_Messenger.cs

namespace MvvmToolkitMessengerExperiments_03
{

  // Here we're using the simplest form where we assume a default 'token'.

  public record MyNotificationMessageA ( string Text ) ;

  public record MyNotificationMessageB ( int Value ) ;

  public class MyRequestMessage : Microsoft.Toolkit.Mvvm.Messaging.Messages.RequestMessage<int> 
  { }

  public record MyMessageSender ( Microsoft.Toolkit.Mvvm.Messaging.IMessenger Messenger )
  {
    public void SendMessageA ( string messageText )
    {
      Messenger.Send(
        new MyNotificationMessageA(messageText)
      ) ;
    }
    public void SendMessageB ( int value )
    {
      Messenger.Send(
        new MyNotificationMessageB(value)
      ) ;
    }
    public void SendValueChangedMessage ( string text )
    {
      Messenger.Send(
        new Microsoft.Toolkit.Mvvm.Messaging.Messages.ValueChangedMessage<string>(text)
      ) ;
    }
    public void SendPropertyChangedMessage ( string text )
    {
      Messenger.Send(
        new Microsoft.Toolkit.Mvvm.Messaging.Messages.PropertyChangedMessage<string>(
          this,
          "MyPropertyName",
          "old-value",
          text
        )
      ) ;
    }
    public MyRequestMessage SendRequest ( )
    {
      return Messenger.Send(
        new MyRequestMessage()
      ) ;
    }
    public Microsoft.Toolkit.Mvvm.Messaging.Messages.AsyncRequestMessage<string> SendRequestAsync ( )
    {
      return Messenger.Send(
        new Microsoft.Toolkit.Mvvm.Messaging.Messages.AsyncRequestMessage<string>()
      ) ;
    }
  }

  public abstract class MyMessageRecipient
  {
    public string? A { get ; set ; }
    public int? B    { get ; set ; }
  }

  public class MyMessageRecipient_UsingRecipientInterface : MyMessageRecipient
  , Microsoft.Toolkit.Mvvm.Messaging.IRecipient<MyNotificationMessageA>
  , Microsoft.Toolkit.Mvvm.Messaging.IRecipient<MyNotificationMessageB>
  , Microsoft.Toolkit.Mvvm.Messaging.IRecipient<MyRequestMessage>
  {
    public MyMessageRecipient_UsingRecipientInterface ( Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger )
    {
      messenger.RegisterAll(this) ;
    }
    // Here we're responding to a 'notification' sent via the messenger
    public void Receive ( MyNotificationMessageA message )
    {
      A = message.Text ;
    }
    public void Receive ( MyNotificationMessageB message )
    {
      B = message.Value ;
    }
    // Here we're responding to a 'request' that has been sent via the messenger.
    // We respond by sending a 'reply' containing a value.
    public void Receive ( MyRequestMessage message )
    {
      message.Reply(100) ;
    }
  }

  public class MyMessageRecipient_UsingLambdas : MyMessageRecipient
  {
    public MyMessageRecipient_UsingLambdas ( Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger )
    {
      messenger.Register<MyNotificationMessageA>(
        this,
        (recipient,message) => {
          A = message.Text ;
        }
      ) ;
      messenger.Register<MyNotificationMessageB>(
        this,
        (recipient,message) => {
          B = message.Value ;
        }
      ) ;
      messenger.Register<MyRequestMessage>(
        this,
        (recipient,message) => {
          if ( message.HasReceivedResponse )
          {
            // Another recipient has already responded
            // with a Reply, so don't do anything ...
          }
          else
          {
            message.Reply(
              100
            ) ;
          }
        }
      ) ;
      messenger.Register<Microsoft.Toolkit.Mvvm.Messaging.Messages.AsyncRequestMessage<string>>(
        this,
        (recipient,message) => {
          if ( message.HasReceivedResponse )
          {
            // Another recipient has already responded
            // with a Reply, so don't do anything ...
          }
          else
          {
            message.Reply(
              "hello"
            ) ;
          }
        }
      ) ;
    }
  }

  public class Tests_03 
  {

    private Xunit.Abstractions.ITestOutputHelper OutputHelper ;

    public Tests_03 ( Xunit.Abstractions.ITestOutputHelper outputHelper )
    {
      OutputHelper = outputHelper ;
      // OutputHelper.WriteLine("HELPER") ;
    }

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
      var recipientX = new MyMessageRecipient_UsingLambdas(messenger) ;
      var recipientY = new MyMessageRecipient_UsingLambdas(messenger) ;
      // When we send a message, all our recipients should receive it
      messageSender.SendMessageA("hello") ;
      recipientX.A.Should().Be("hello") ;
      recipientY.A.Should().Be("hello") ;
      messageSender.SendMessageB(123) ;
      recipientX.B.Should().Be(123) ;
      recipientY.B.Should().Be(123) ;
      var requestSent = messageSender.SendRequest() ;
      requestSent.Response.Should().Be(100) ;
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

    [Fact]
    public async System.Threading.Tasks.Task Test_03 ( )
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
      var requestSent = messageSender.SendRequestAsync() ;
      var result = await requestSent.Response ;
      result.Should().Be("hello") ;
    }

    [Fact]
    public void Test_04 ( )
    {
      Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger = (
        new Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger()
        // Microsoft.Toolkit.Mvvm.Messaging
        // //.WeakReferenceMessenger
        // .StrongReferenceMessenger
        // .Default 
      ) ;
      var startTime = System.DateTime.Now.Ticks ; 
      bool started = false ;
      var taskPollingForMessage = Task.Run(
        async () => {
          OutputHelper.WriteLine(
            $"Waiting for message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
          ) ;
          var recipient = new MyMessageRecipient_UsingLambdas(messenger) ;
          int n = 0 ;
          started = true ;
          while ( 
             recipient.A is null 
          && n++ < 20
          ) {
            await Task.Delay(100) ;
          }
          recipient.A.Should().Be("hello") ;
          OutputHelper.WriteLine(
            $"Message '{recipient.A}' was received on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
          ) ;
        }
      ) ;
      while ( 
        started is false
        // taskPollingForMessage.Status != TaskStatus.Running 
      ) {
        Thread.Sleep(100) ;
      }
      OutputHelper.WriteLine(
        $"Preparing to send message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
      ) ;
      var messageSender = new MyMessageSender(messenger) ;
      messageSender.SendMessageA("hello") ;
      OutputHelper.WriteLine(
        $"Have sent message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
      ) ;
      taskPollingForMessage.Wait() ;
    }

    [Fact]
    public void Test_04a ( )
    {
      Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger = (
        new Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger()
        // Microsoft.Toolkit.Mvvm.Messaging
        // //.WeakReferenceMessenger
        // .StrongReferenceMessenger
        // .Default 
      ) ;
      var startTime = System.DateTime.Now.Ticks ; 
      var pollingHasStarted = new ManualResetEventSlim(false) ;
      var taskPollingForMessage = Task.Run(
        async () => {
          OutputHelper.WriteLine(
            $"Waiting for message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
          ) ;
          var recipient = new MyMessageRecipient_UsingLambdas(messenger) ;
          int n = 0 ;
          pollingHasStarted.Set() ;
          while ( 
             recipient.A is null 
          && n++ < 20
          ) {
            await Task.Delay(100) ;
          }
          recipient.A.Should().Be("hello") ;
          OutputHelper.WriteLine(
            $"Message '{recipient.A}' was received on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
          ) ;
        }
      ) ;
      pollingHasStarted.Wait(500).Should().BeTrue() ; ;
      OutputHelper.WriteLine(
        $"Preparing to send message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
      ) ;
      var messageSender = new MyMessageSender(messenger) ;
      messageSender.SendMessageA("hello") ;
      OutputHelper.WriteLine(
        $"Have sent message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
      ) ;
      taskPollingForMessage.Wait() ;
    }

    [Fact]
    public async Task Test_04b ( )
    {
      Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger = (
        new Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger()
        // Microsoft.Toolkit.Mvvm.Messaging
        // //.WeakReferenceMessenger
        // .StrongReferenceMessenger
        // .Default 
      ) ;
      var startTime = System.DateTime.Now.Ticks ; 
      var pollingHasStarted = new SemaphoreSlim(
        initialCount : 0, 
        maxCount     : 1
      ) ;
      var taskPollingForMessage = Task.Run(
        async () => {
          OutputHelper.WriteLine(
            $"Waiting for message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
          ) ;
          var recipient = new MyMessageRecipient_UsingLambdas(messenger) ;
          int n = 0 ;
          pollingHasStarted.Release() ;
          while ( 
             recipient.A is null 
          && n++ < 20
          ) {
            // The 'await' in itself does not guarantee that another thread is created,
            // but it causes everything else after the statement to run as
            // a continuation on the 'Task' or 'awaitable' that you call 'await' on.
            // More often than not, it is some sort of asynchronous operation, 
            // which could be IO completion, or something that is on another thread. 
            await Task.Delay(100).ConfigureAwait(
              continueOnCapturedContext : true
            ) ;
          }
          recipient.A.Should().Be("hello") ;
          OutputHelper.WriteLine(
            $"Message '{recipient.A}' was received on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
          ) ;
        }
      ) ;
      // bool waitSucceeded = await pollingHasStarted.WaitAsync(
      //   millisecondsTimeout: 500
      // ).ConfigureAwait(
      //   continueOnCapturedContext : true
      // ) ;
      // waitSucceeded.Should().BeTrue() ;
      (
        await pollingHasStarted.WaitAsync(
          millisecondsTimeout: 500
        ).ConfigureAwait(
          continueOnCapturedContext : true
        )
      ).Should().BeTrue() ;
      OutputHelper.WriteLine(
        $"Preparing to send message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
      ) ;
      var messageSender = new MyMessageSender(messenger) ;
      messageSender.SendMessageA("hello") ;
      OutputHelper.WriteLine(
        $"Have sent message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
      ) ;
      taskPollingForMessage.Wait() ;
    }

    //
    // Aha, here's the *right* way to do it ... with a TaskCompletionSource
    // that we can 'await' ; better than using a ManualResetEvent.
    //

    [Fact]
    public async Task Test_04c ( )
    {
      Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger = (
        new Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger()
        // Microsoft.Toolkit.Mvvm.Messaging
        // //.WeakReferenceMessenger
        // .StrongReferenceMessenger
        // .Default 
      ) ;
      var startTime = System.DateTime.Now.Ticks ; 
      var pollingHasStarted = new TaskCompletionSource() ;
      var taskPollingForMessage = Task.Run(
        async () => {
          OutputHelper.WriteLine(
            $"Waiting for message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
          ) ;
          var recipient = new MyMessageRecipient_UsingLambdas(messenger) ;
          int n = 0 ;
          pollingHasStarted.SetResult() ;
          while ( 
             recipient.A is null 
          && n++ < 20
          ) {
            await Task.Delay(100) ;
          }
          recipient.A.Should().Be("hello") ;
          OutputHelper.WriteLine(
            $"Message '{recipient.A}' was received on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
          ) ;
        }
      ) ;
      await pollingHasStarted.Task ;
      OutputHelper.WriteLine(
        $"Preparing to send message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
      ) ;
      var messageSender = new MyMessageSender(messenger) ;
      messageSender.SendMessageA("hello") ;
      OutputHelper.WriteLine(
        $"Have sent message on thread #{System.Environment.CurrentManagedThreadId} at {System.DateTime.Now.Ticks-startTime}"
      ) ;
      taskPollingForMessage.Wait() ;
    }

    // [Fact]
    // public async System.Threading.Tasks.Task Test_04a ( )
    // {
    //   Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger = (
    //     Microsoft.Toolkit.Mvvm.Messaging
    //     //.WeakReferenceMessenger
    //     .StrongReferenceMessenger
    //     .Default 
    //   ) ;
    //   var recipient = new MyMessageRecipient_UsingLambdas(messenger) ;
    //   var mainThreadId = System.Environment.CurrentManagedThreadId ;
    //   bool messageHasBeenSent = false ;
    //   Task.Run(
    //     async () => {
    //       int n = 0 ;
    //       while ( 
    //           recipient.A is null 
    //       && n++ < 20
    //       ) {
    //         await Task.Delay(100) ;
    //       }
    //       messageHasBeenSent.Should().BeTrue() ;
    //       recipient.A.Should().Be("hello") ;
    //     }
    //   ) ;
    //   await Task.Run(
    //     () => {
    //       var workerThreadId = System.Environment.CurrentManagedThreadId ;
    //       workerThreadId.Should().NotBe(mainThreadId) ; 
    //       var messageSender = new MyMessageSender(messenger) ;
    //       messageSender.SendMessageA("hello") ;
    //       messageHasBeenSent = true ;
    //     }
    //   ) ;
    // }

    [Fact]
    public async System.Threading.Tasks.Task Test_05 ( )
    {
      Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger = (
        Microsoft.Toolkit.Mvvm.Messaging
        //.WeakReferenceMessenger
        .StrongReferenceMessenger
        .Default 
      ) ;
      var recipient = new MyMessageRecipient_UsingLambdas(messenger) ;
      var messageSender = new MyMessageSender(messenger) ;
      var requestSent = messageSender.SendRequestAsync() ;
      await Task.Run(
        async () => {
          var result = await requestSent.Response ;
          result.Should().Be("hello") ;
        }
      ) ;
    }

  }

}
