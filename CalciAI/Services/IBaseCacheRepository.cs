using CalciAI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalciAI.Services
{
    public interface IBaseCacheRepository<T> : IService
    {
        string BuildDocId(object id, int prefixIndex = 0);

        Task<bool> AddOrUpdateAsync(string docId, T detail);

        Task<bool> AddOrUpdateAsync(string docId, Dictionary<string, dynamic> dict);

        Task<bool> DeleteAsync(string docId);

        SearchResult<T> Search(string sentence, int offset = 0, int count = 100);

        Task<SearchResult<T>> SearchRecordsAsync(string sentence, int offset = 0, int count = 100);

        Task<SearchResult<string>> SearchRecordIdsAsync(string sentence, int offset = 0, int count = 100);

        bool CreateIndex();

        bool DropIndex();

        void EnsureIndexExists();

        Task<T> GetAsync(string docId);

        Task<T[]> GetAsync(params string[] docIds);

        T Get(string docId);
    }
}
