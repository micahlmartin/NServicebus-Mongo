using NServiceBus;

namespace MyPublisher
{
    class EndpointConfig :  IConfigureThisEndpoint, IWantCustomInitialization
    {
        public void Init()
        {
            Configure.With()
                .Log4Net()
                .DefaultBuilder()
                .XmlSerializer()
                .MsmqTransport()
                .UnicastBus()
                .LoadMessageHandlers()
                .PurgeOnStartup(false)
                .MongoPersistence()
                .MongoSubscriptionStorage()
                .MongoSagaPersister();

            SetLoggingLibrary.Log4Net(log4net.Config.XmlConfigurator.Configure);
        }
    }
}
