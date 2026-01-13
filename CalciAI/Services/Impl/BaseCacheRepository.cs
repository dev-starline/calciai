using Microsoft.Extensions.Logging;
using NRediSearch;
using CalciAI.Caching;
using CalciAI.Models;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalciAI.Services.Impl
{
    public abstract class BaseCacheRepository<T>
    {
        protected readonly IDatabase _database;
        protected readonly Client _client;
        protected readonly ILogger _logger;

        protected readonly string _ixName;
        protected readonly string[] _prefixNames;

        protected BaseCacheRepository(ILogger logger, string indexName, params string[] prefixNames)
        {
            _ixName = indexName;
            _prefixNames = prefixNames;

            _database = RedisStore.RedisCache;
            _client = new Client(indexName, _database);
            _logger = logger;
        }

        public async Task<bool> AddOrUpdateAsync(string docId, T detail)
        {
            var dict = Map(detail);
            return await RedisStore.RedisCache.UpdateHashAsync(docId, dict);
        }

        public async Task<bool> AddOrUpdateAsync(string docId, Dictionary<string, dynamic> dict)
        {
            return await RedisStore.RedisCache.UpdateHashAsync(docId, dict);
        }

        public async Task<bool> DeleteAsync(string docId)
        {
            return await RedisStore.RedisCache.KeyDeleteAsync(docId);
        }

        public SearchResult<T> Search(string sentence, int offset = 0, int count = 100)
        {
            return SearchRecordsAsync(sentence, offset, count).Result;
        }

        public async Task<SearchResult<T>> SearchRecordsAsync(string sentence, int offset = 0, int count = 100)
        {
            var query = new Query(sentence);
            query.Limit(offset, count);
            var result = await _client.SearchAsync(query);
            var documents = result.Documents;
            var returnResult = new SearchResult<T>(result.TotalResults, CastRedisValues(documents));
            return returnResult;
        }

        public async Task<SearchResult<string>> SearchRecordIdsAsync(string sentence, int offset = 0, int count = 100)
        {
            var query = new Query(sentence);
            query.Limit(offset, count);
            query.SetNoContent();
            var result = await _client.SearchAsync(query);
            var documents = result.Documents;
            return new SearchResult<string>(result.TotalResults, documents.Select(x => x.Id).ToList());
        }

        public T Get(string docId)
        {
            var doc = _client.GetDocument(docId);
            return Cast(doc);
        }

        public T[] Get(string[] docIds)
        {
            var returnDocs = new List<T>();

            var docs = _client.GetDocuments(docIds);

            foreach (var item in docs)
            {
                returnDocs.Add(Cast(item));
            }

            return returnDocs.ToArray();
        }

        public async Task<T> GetAsync(string docId)
        {
            var doc = await _client.GetDocumentAsync(docId);
            return Cast(doc);
        }

        public async Task<T[]> GetAsync(params string[] docIds)
        {
            var returnDocs = new List<T>();

            var docs = await _client.GetDocumentsAsync(docIds);

            foreach (var item in docs)
            {
                returnDocs.Add(Cast(item));
            }

            return returnDocs.ToArray();
        }

        public List<T> CastRedisValues(List<Document> docList)
        {
            List<T> newDoc = new();

            foreach (var item in docList)
            {
                newDoc.Add(Cast(item));
            }

            return newDoc;
        }

        public abstract Dictionary<string, dynamic> Map(T detail);

        //Try to shift DocId from repository to model
        public virtual string BuildDocId(object id, int prefixIndex = 0)
        {
            return $"{_prefixNames[prefixIndex]}{id}";
        }

        public abstract T Cast(Document doc);

        public abstract bool CreateIndex();

        public bool DropIndex()
        {
            return _client.DropIndex();
        }

        public void EnsureIndexExists()
        {
            var indexExists = false;
            try
            {
                indexExists = _client.GetInfo() != null;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error in ensure CATALOG Index exists: {Message}", ex.Message);
            }

            try
            {
                if (!indexExists)
                {
                    CreateIndex();
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error in Create CATALOG Index exists: {Message}", ex.Message);
            }
        }
    }
}
