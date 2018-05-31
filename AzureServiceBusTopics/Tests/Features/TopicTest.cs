namespace AzureServiceBusTopics.Tests.Features
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.InteropExtensions;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    using NUnit.Framework;

    using Shouldly;

    using TestStack.BDDfy;

    using SubscriptionClient = Microsoft.Azure.ServiceBus.SubscriptionClient;
    using TopicClient = Microsoft.Azure.ServiceBus.TopicClient;

    class TopicTest
    {
        public const string TEST_MESSAGE_SUB = "TestMessageSub";

        private const string ServiceBusConnectionString =
            "Endpoint=sb://helensbnstopics.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mBSqgAKVbAiiSxieIJ1yqfaKhlO+IFg2T8u7fbyZink=";

        private const string SubscriptionName = "testsubscription";

        private const string TopicName = "firsttopic";

        private static TopicClient testTopicClient;

        private static SubscriptionClient testSubscriptionClient;

        private static OnMessageOptions options;

        private static NamespaceManager namespaceManager;

        private static Message message;

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

            // create test subscription
            namespaceManager = NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
            namespaceManager.CreateSubscription(TopicName, TEST_MESSAGE_SUB);

            options = new OnMessageOptions() { AutoComplete = false, AutoRenewTimeout = TimeSpan.FromMinutes(1), };
        }

        private void TheMessageIsPublished()
        {
            string receivedMessage = message.GetBody<string>();

            receivedMessage.ShouldBe("Test Message");
        }

        private void TheMessageIsSent()
        {
            testTopicClient.SendAsync(message);
        }
    }
}
