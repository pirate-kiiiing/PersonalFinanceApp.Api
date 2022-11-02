using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PirateKing.Core;
using PirateKing.Exceptions;
using PirateKing.Guards;
using PirateKing.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// CosmosDB Client for PirateKing
    /// </summary>
    public class CosmosClient : ICosmosClient
    {
        private readonly IDocumentClient documentClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public CosmosClient(CosmosClientConfiguration configuration)
        {
            Validate.NotNull(configuration, nameof(configuration));

            documentClient = new DocumentClient(
                serviceEndpoint: configuration.AccountEndpoint, 
                authKeyOrResourceToken: configuration.AccountKey);
        }

        /// <summary>
        /// Create a document
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">Document partitionkey</param>
        /// <param name="document">Document to be created/updated</param>
        /// <param name="disableAutomaticIdGeneration">Flag that indicates if automatic Id generation should be enabled/disabled</param>
        /// <returns></returns>
        public async Task<TDocument> CreateDocumentAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            TDocument document,
            bool disableAutomaticIdGeneration = true)
            where TDocument : class
        {
            Validate.NotNullOrEmpty(databaseId, nameof(databaseId));
            Validate.NotNullOrEmpty(collectionId, nameof(collectionId));
            Validate.NotNullOrEmpty(partitionKey, nameof(partitionKey));
            Validate.NotNull(document, nameof(document));

            var requestOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey(partitionKey.ToLower())
            };

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

            try
            {
                ResourceResponse<Document> response = await documentClient.CreateDocumentAsync(
                    collectionUri,
                    document,
                    requestOptions,
                    disableAutomaticIdGeneration);

                return (TDocument)(dynamic)response.Resource;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == null) throw new HttpException((int)0, e.Message, e);

                throw new HttpException(e.StatusCode.Value, e.Message, e);
            }
        }

        /// <summary>
        /// Read a single document using partition key and row id
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">Document partitionkey</param>
        /// <param name="id">The document identifier</param>
        /// <returns><see cref="TDocument"></returns>
        public async Task<TDocument> ReadDocumentAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            string id)
            where TDocument : class
        {
            Validate.NotNullOrEmpty(databaseId, nameof(databaseId));
            Validate.NotNullOrEmpty(collectionId, nameof(collectionId));
            Validate.NotNullOrEmpty(partitionKey, nameof(partitionKey));
            Validate.NotNullOrEmpty(id, nameof(id));

            var requestOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey(partitionKey.ToLower())
            };

            Uri documentUri = UriFactory.CreateDocumentUri(
                databaseId,
                collectionId,
                id);

            ResourceResponse<Document> response;

            try
            {
                response = await documentClient.ReadDocumentAsync(
                    documentUri,
                    requestOptions);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == null) throw new HttpException((int) 0, e.Message, e);
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw new HttpException(e.StatusCode.Value, e.Message, e);
            }

            return (response != null) ? (TDocument)(dynamic)response.Resource : null;
        }

        /// <summary>
        /// Get documents for a given partitionKey based on sql query spec.
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="filter">Documents query string </param>
        /// <param name="maxResults">The maximum results.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>
        /// DocumentResultSet with continuation token and list of docs
        /// </returns>
        public async Task<ContinuableSet<TDocument>> ReadDocumentsAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            BaseFilter filter,
            int maxResults,
            string continuationToken)
            where TDocument : class
        {
            Validate.NotNullOrEmpty(databaseId, nameof(databaseId));
            Validate.NotNullOrEmpty(collectionId, nameof(collectionId));

            FeedOptions feedOptions = new FeedOptions()
            {
                PartitionKey = (String.IsNullOrWhiteSpace(partitionKey) == false) 
                    ? new PartitionKey(partitionKey.ToLower())
                    : null,
                MaxItemCount = maxResults,
                RequestContinuation = (String.IsNullOrWhiteSpace(continuationToken) == false)
                    ? continuationToken
                    : null,
            };

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId,
                collectionId);

            FeedResponse<dynamic> feedResponse = null;

            try
            {
                if (filter != null)
                {
                    IDocumentQuery<dynamic> documentQuery = documentClient.CreateDocumentQuery(
                                                                collectionUri,
                                                                ToSqlQuerySpec(filter),
                                                                feedOptions).AsDocumentQuery();

                    feedResponse = await documentQuery.ExecuteNextAsync<dynamic>();
                }
                else
                {
                    IDocumentQuery<Document> documentQuery = documentClient.CreateDocumentQuery(
                                                                collectionUri,
                                                                feedOptions).AsDocumentQuery();

                    feedResponse = await documentQuery.ExecuteNextAsync<dynamic>();
                }

                ContinuableSet<TDocument> response = ToContinuableSet<TDocument>(feedResponse);

                return response;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == null) throw new HttpException((int) 0, e.Message, e);
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return new ContinuableSet<TDocument>
                    {
                        Items = null,
                    };
                }

                throw new HttpException(e.StatusCode.Value, e.Message, e);
            }
        }

        /// <summary>
        /// Get all documents for a given partitionKey based on sql query spec
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="filter">Documents query string </param>
        /// <returns>List of TDocument</returns>
        public async Task<IEnumerable<TDocument>> ReadAllDocumentsAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            BaseFilter filter)
            where TDocument : class
        {
            Validate.NotNullOrEmpty(databaseId, nameof(databaseId));
            Validate.NotNullOrEmpty(collectionId, nameof(collectionId));

            var documents = new List<TDocument>();
            ContinuableSet<TDocument> continuable;

            for (string continuationToken = String.Empty; continuationToken != null; continuationToken = continuable.ContinuationToken)
            {
                continuable = await ReadDocumentsAsync<TDocument>(
                    databaseId,
                    collectionId,
                    partitionKey,
                    filter,
                    CosmosConstants.OptimalMaxResultsCount,
                    continuationToken: null);

                if (continuable == null || continuable.Items.IsNullOrEmpty() == true) break;

                documents.AddRange(continuable.Items);
            }

            return documents;
        }

        /// <summary>
        /// Update document
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">Document partitionkey</param>
        /// <param name="document">Document to be created/updated</param>
        /// <param name="etag">ETag used for optimistic concurrency check</param>
        /// <returns></returns>
        public async Task<TDocument> UpdateDocumentAsync<TDocument>(
            string databaseId,
            string collectionId,
            string documentId,
            string partitionKey,
            TDocument document,
            string etag)
            where TDocument : class
        {
            Validate.NotNullOrEmpty(databaseId, nameof(databaseId));
            Validate.NotNullOrEmpty(collectionId, nameof(collectionId));
            Validate.NotNullOrEmpty(partitionKey, nameof(partitionKey));
            Validate.NotNull(document, nameof(document));

            var requestOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey(partitionKey.ToLower()),
                AccessCondition = new AccessCondition
                {
                    Condition = etag,
                    Type = AccessConditionType.IfMatch
                },
            };

            Uri collectionUri = UriFactory.CreateDocumentUri(databaseId, collectionId, documentId);

            try
            {
                ResourceResponse<Document> response = await documentClient.ReplaceDocumentAsync(
                    collectionUri,
                    document,
                    requestOptions);

                return (TDocument)(dynamic)response.Resource;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == null) throw new HttpException((int)0, e.Message, e);

                throw new HttpException(e.StatusCode.Value, e.Message, e);
            }
        }

        /// <summary>
        /// Create or update document
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">Document partitionkey</param>
        /// <param name="document">Document to be created/updated</param>
        /// <param name="etag">ETag used for optimistic concurrency check</param>
        /// <param name="disableAutomaticIdGeneration">Flag that indicates if automatic Id generation should be enabled/disabled</param>
        /// <returns></returns>
        public async Task<TDocument> UpsertDocumentAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            TDocument document,
            string etag = null,
            bool disableAutomaticIdGeneration = true)
            where TDocument : class
        {
            Validate.NotNullOrEmpty(databaseId, nameof(databaseId));
            Validate.NotNullOrEmpty(collectionId, nameof(collectionId));
            Validate.NotNullOrEmpty(partitionKey, nameof(partitionKey));
            Validate.NotNull(document, nameof(document));

            var requestOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey(partitionKey.ToLower())
            };

            if (String.IsNullOrWhiteSpace(etag) == false)
            {
                requestOptions.AccessCondition = new AccessCondition
                {
                    Condition = etag,
                    Type = AccessConditionType.IfMatch
                };
            }

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

            try
            {
                ResourceResponse<Document> response = await documentClient.UpsertDocumentAsync(
                    collectionUri,
                    document,
                    requestOptions,
                    disableAutomaticIdGeneration);

                return (TDocument)(dynamic)response.Resource;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == null) throw new HttpException((int) 0, e.Message, e);

                throw new HttpException(e.StatusCode.Value, e.Message, e);
            }
        }

        /// <summary>
        /// Hard delete document.
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">Document partitionkey</param>
        /// <param name="id">Unique Identifier</param>
        /// <returns></returns>
        public async Task<TDocument> DeleteDocumentAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            string id)
        {
            Validate.NotNullOrEmpty(databaseId, nameof(databaseId));
            Validate.NotNullOrEmpty(collectionId, nameof(collectionId));
            Validate.NotNullOrEmpty(partitionKey, nameof(partitionKey));
            Validate.NotNullOrEmpty(id, nameof(id));

            var requestOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey(partitionKey.ToLower())
            };

            Uri documentUri = UriFactory.CreateDocumentUri(
                databaseId,
                collectionId,
                id);

            try
            {
                ResourceResponse<Document> response =
                    await documentClient.DeleteDocumentAsync(documentUri, requestOptions);

                return (TDocument)(dynamic)response.Resource;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == null) throw new HttpException((int) 0, e.Message, e);

                throw new HttpException(e.StatusCode.Value, e.Message, e);
            }
        }

        private SqlQuerySpec ToSqlQuerySpec(BaseFilter filter)
        {
            var sqlQuerySpec = new SqlQuerySpec
            {
                QueryText = filter.Query,
                Parameters = new SqlParameterCollection(
                    filter.Parameters.Select(param => new SqlParameter(param.Key, param.Value)))
            };

            return sqlQuerySpec;
        }

        private ContinuableSet<TDocument> ToContinuableSet<TDocument>(FeedResponse<dynamic> feedResponse)
        {
            var response = new ContinuableSet<TDocument>()
            {
                ContinuationToken = feedResponse.ResponseContinuation,
                Items = feedResponse.Select(doc => (TDocument)doc).ToList()
            };

            return response;
        }
    }
}
