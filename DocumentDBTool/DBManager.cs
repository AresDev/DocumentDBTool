//-----------------------------------------------------------------------
// <copyright file="DBManager.cs" company="Intergrupo S.A.">
//    Copyright (c) Intergrupo S.A. Todos los Derechos Reservados   
//    La información aquí contenida es propietaria y confidencial.            
// </copyright>
//-----------------------------------------------------------------------
namespace DocumentDBTool
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// DB Manager
    /// </summary>
    public static class DBManager
    {
        /// <summary>
        /// database Id
        /// </summary>
        private static readonly string DatabaseId = DocumentDBTool.Properties.Settings.Default.Database;

        /// <summary>
        /// collection Id
        /// </summary>
        private static readonly string CollectionId = DocumentDBTool.Properties.Settings.Default.Collection;

        /// <summary>
        /// end point Url
        /// </summary>
        private static readonly string EndpointUrl = DocumentDBTool.Properties.Settings.Default.Url;

        /// <summary>
        /// authorization Key
        /// </summary>
        private static readonly string AuthorizationKey = DocumentDBTool.Properties.Settings.Default.Key;

        /// <summary>
        /// the collection
        /// </summary>
        private static Task<DocumentCollection> collection;

        /// <summary>
        /// the client
        /// </summary>
        private static DocumentClient client;

        /// <summary>
        /// the database
        /// </summary>
        private static Database database;

        /// <summary>
        /// Gets or sets the Collection
        /// </summary>
        public static Task<DocumentCollection> Collection
        {
            get
            {
                if (DBManager.collection == null)
                {
                    DBManager.collection = GetOrCreateCollectionAsync(DBManager.Database.SelfLink, CollectionId);
                }

                return DBManager.collection;
            }

            set
            {
                DBManager.collection = value;
            }
        }

        /// <summary>
        /// Gets or sets the Client
        /// </summary>
        public static DocumentClient Client
        {
            get
            {
                if (DBManager.client == null)
                {
                    var policy = new ConnectionPolicy
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp
                    };
                    DBManager.client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey, policy);
                }

                return DBManager.client;
            }

            set
            {
                DBManager.client = value;
            }
        }

        /// <summary>
        /// Gets or sets the database
        /// </summary>
        public static Database Database
        {
            get
            {
                if (DBManager.database == null)
                {
                    DBManager.database = GetOrCreateDatabaseAsync(DatabaseId).Result;
                }

                return DBManager.database;
            }

            set
            {
                DBManager.database = value;
            }
        }

        /// <summary>
        /// Get a DocumentCollection by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="databaseLink">The Database SelfLink property where this DocumentCollection exists / will be created</param>
        /// <param name="id">The id of the DocumentCollection to search for, or create.</param>
        /// <returns>The matched, or created, DocumentCollection object</returns>
        private static async Task<DocumentCollection> GetOrCreateCollectionAsync(string databaseLink, string id)
        {
            DocumentCollection collection = Client.CreateDocumentCollectionQuery(databaseLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = id });
            }

            return collection;
        }

        /// <summary>
        /// Get a Database by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="id">The id of the Database to search for, or create.</param>
        /// <returns>The matched, or created, Database object</returns>
        private static async Task<Database> GetOrCreateDatabaseAsync(string id)
        {
            Database database = Client.CreateDatabaseQuery().Where(db => db.Id == id).ToArray().FirstOrDefault();
            if (database == null)
            {
                database = await Client.CreateDatabaseAsync(new Database { Id = id });
            }

            return database;
        }
    }
}
