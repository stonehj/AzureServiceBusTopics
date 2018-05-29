namespace AzureServiceBusTopics.Tests.Features
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Azure.ServiceBus;
    using AzureServiceBusTopics;

    using NUnit.Framework;

    using TestStack.BDDfy;

    class TopicTest
    {
        private const string ServiceBusConnectionString =
            "Endpoint=sb://helensbnstopics.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mBSqgAKVbAiiSxieIJ1yqfaKhlO+IFg2T8u7fbyZink=";

        private const string SubscriptionName = "testsubscription";

        private const string TopicName = "firsttopic";

        private static ITopicClient testTopicClient;

        private static ISubscriptionClient testSubscriptionClient;

        [Test]
        public void TopicHappyPathTest()
        {
            this.Given(_ => this.ATestTopicAndATestSubscription())
                .When(_ => this.TheMessageIsSent())
                .Then(_ => this.TheMessageIsPublished())
                .BDDfy();
        }

        private void ATestTopicAndATestSubscription()
        {
            testTopicClient = new TopicClient(ServiceBusConnectionString, TopicName);
            testSubscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);
        }

        private void TheMessageIsPublished()
        {
            throw new NotImplementedException();
        }

        private async Task TheMessageIsSent()
        {
            await MessageSender.SendMessagesAsync(1);
        }
    }
}
