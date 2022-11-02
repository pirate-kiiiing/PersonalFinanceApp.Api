using System.Collections.Generic;
using System.Threading.Tasks;
using PirateKing.Models;

namespace PirateKing.Cosmos
{
    public interface ICosmosClient
    {
        /// <summary>
        /// Create a document
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">Document partitionkey</param>
        /// <param name="document">Document to be created/updated</param>
        /// <param name="disableAutomaticIdGeneration">Flag that indicates if automatic Id generation should be enabled/disabled</param>
        /// <returns></returns>
        Task<TDocument> CreateDocumentAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            TDocument document,
            bool disableAutomaticIdGeneration = true)
            where TDocument : class;

        /// <summary>
        /// Read a single document using partition key and row id
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">Document partitionkey</param>
        /// <param name="id">The document identifier</param>
        /// <returns><see cref="TDocument"></returns>
        Task<TDocument> ReadDocumentAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            string id)
            where TDocument : class;

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
        Task<ContinuableSet<TDocument>> ReadDocumentsAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            BaseFilter filter,
            int maxResults,
            string continuationToken)
            where TDocument : class;

        /// <summary>
        /// Get all documents for a given partitionKey based on sql query spec
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="filter">Documents query string </param>
        /// <returns>List of TDocument</returns>
        Task<IEnumerable<TDocument>> ReadAllDocumentsAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            BaseFilter filter)
            where TDocument : class;

        /// <summary>
        /// Update document
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="documentId">The document identifier</param>
        /// <param name="partitionKey">Document partitionkey</param>
        /// <param name="document">Document to be created/updated</param>
        /// <param name="etag">ETag used for optimistic concurrency check</param>
        /// <returns></returns>
        Task<TDocument> UpdateDocumentAsync<TDocument>(
            string databaseId,
            string collectionId,
            string documentId,
            string partitionKey,
            TDocument document,
            string etag)
            where TDocument : class;

        /// <summary>
        /// Create or update document
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">Document partitionkey</param>
        /// <param name="document">Document to be created/updated</param>
        /// <param name="eTag">ETag used for optimistic concurrency check</param>
        /// <param name="disableAutomaticIdGeneration">Flag that indicates if automatic Id generation should be enabled/disabled</param>
        Task<TDocument> UpsertDocumentAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            TDocument document,
            string etag = null,
            bool disableAutomaticIdGeneration = true)
            where TDocument : class;

        /// <summary>
        /// Hard delete document.
        /// </summary>
        /// <param name="databaseId">The database identifier</param>
        /// <param name="collectionId">The collection identifier</param>
        /// <param name="partitionKey">Document partitionkey</param>
        /// <param name="id">Unique Identifier</param>
        /// <returns></returns>
        Task<TDocument> DeleteDocumentAsync<TDocument>(
            string databaseId,
            string collectionId,
            string partitionKey,
            string id);
    }
}
