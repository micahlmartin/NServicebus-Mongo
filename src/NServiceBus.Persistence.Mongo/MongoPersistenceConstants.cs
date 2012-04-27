namespace NServiceBus.Persistence.Mongo
{
    public static class MongoPersistenceConstants
    {
        public const int DefaultPort = 27017;
        public static string DefaultUrl
        {
            get 
            { 
                var masterNode = Configure.Instance.GetMasterNode();

                if (string.IsNullOrEmpty(masterNode))
                    masterNode = "127.0.0.1";

                return string.Format("mongodb://{0}:{1}", masterNode, DefaultPort);
            }
        }
        public const string SubscriptionCollectionName = "subscriptions";
        public const string SagaCollectionName = "sagas";
        public const string SagaUniqueIdentityCollectionName = "saga_unique_ids";
    }
}
