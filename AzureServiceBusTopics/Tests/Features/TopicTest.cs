namespace AzureServiceBusTopics.Tests.Features
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Azure.ServiceBus;

    using Mossharbor.AzureWorkArounds.ServiceBus;

    using NUnit.Framework;

    using TestStack.BDDfy;

    using SubscriptionClient = Microsoft.Azure.ServiceBus.SubscriptionClient;
    using TopicClient = Microsoft.Azure.ServiceBus.TopicClient;

    class TopicTest
    {
        public const string TEST_MESSAGE_SUB = "TestMessageSub";

        private const string ServiceBusConnectionString =
            "Endpoint=sb://helensbnstopics.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mBSqgAKVbAiiSxieIJ1yqfaKhlO+IFg2T8u7fbyZink=";

        private const string SubscriptionName = "testSubscription";

        private const string TopicName = "testTopicSubscription";

        private static TopicClient testTopicClient;

        private static SubscriptionClient testSubscriptionClient;

        private static NamespaceManager namespaceManager;

        private static Message message;

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            await testSubscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting: ");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.Endpoint}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        [Test]
        public void TopicHappyPathTest()
        {
            this.Given(_ => this.ATestTopicAndATestSubscription())
                .And(_ => this.AMessage())
                .When(_ => this.TheMessageIsSent())
                .Then(_ => this.TheMessageIsPublished())
                .BDDfy();
        }

        private void AMessage()
        {
            string messageBody = "Test Message";
            message = new Message(Encoding.UTF8.GetBytes(messageBody));
        }

        private void ATestTopicAndATestSubscription()
        {
            testTopicClient = new TopicClient(ServiceBusConnectionString, TopicName);
            testSubscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

            namespaceManager = NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
        }

        private void TheMessageIsPublished()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            testSubscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
            Assert.That(message.Body, Is.EqualTo("Test Message"));
        }

        private void TheMessageIsSent()
        {
            testTopicClient.SendAsync(message);
        }
    }
}
