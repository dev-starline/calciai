using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalciAI.Persistance
{
    public static class DbSetExtensions
    {
        public static async Task<T> GetById<T>(this IMongoCollection<T> collection, string id) where T : IDocument
        {
            var docs = await collection.FindAsync(Builders<T>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id)));
            return docs.SingleOrDefault();
        }

        public static async Task<IEnumerable<T>> GetAll<T>(this IMongoCollection<T> collection) where T : IDocument
        {
            var all = await collection.FindAsync(Builders<T>.Filter.Empty);
            return all.ToList();
        }
    }
}
