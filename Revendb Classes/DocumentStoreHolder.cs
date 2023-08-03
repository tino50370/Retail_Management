using System;
using System.Configuration;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.Connection;


namespace RetailKing
{
    public static class DocumentStoreHolder
    {
        private static IDocumentStore documentStore;

        public static IDocumentStore DocumentStore
        {
            get
            {
                if (documentStore != null)
                    return documentStore;

                lock (typeof(DocumentStoreHolder))
                {
                    if (documentStore != null)
                        return documentStore;

                    documentStore = new DocumentStore
                    {
                        Url = "http://localhost:8080/",
                        DefaultDatabase = "yomoney"
                    }.Initialize();

                    //documentStore.Initialize();

                    //IndexCreation.CreateIndexes(typeof(BrainstormSocial).Assembly, documentStore);
                }

                return documentStore;
            }
        }

        private static string ConnectionStringName
        {
            get
            {
                var customConnection = ConfigurationManager.ConnectionStrings[Environment.MachineName] != null;
                var connectionStringName = customConnection ? Environment.MachineName : "yomoney";
                return connectionStringName;
            }
        }
    }
}