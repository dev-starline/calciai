using CalciAI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CalciAI.Persistance
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class BsonCollectionAttribute : Attribute
    {
        public string CollectionName { get; }

        public BsonCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MongoRepository<T> where T : IDocument
    {
        public readonly IMongoDatabase DbContext;
        public readonly IMongoCollection<T> DbSet;

        public MongoRepository(IMongoDatabase context)
        {
            DbContext = context;
            DbSet = DbContext.GetCollection<T>(GetCollectionName(typeof(T)));
        }

        private protected static string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault())?.CollectionName;
        }

        public IQueryable<T> AsQueryable()
        {
            return DbSet.AsQueryable();
        }

        public Task<T> Get(object id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return DbSet.FindAsync(filter).Result.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAll()
        {
            return (await DbSet.FindAsync(Builders<T>.Filter.Empty)).ToList();
        }

        public async Task<List<T>> FilterBy(Expression<Func<T, bool>> filter)
        {
            return (await DbSet.FindAsync(filter)).ToList();
        }

        public async Task<IEnumerable<TProjected>> FilterBy<TProjected>(Expression<Func<T, bool>> filter, ProjectionDefinition<T, TProjected> projection)
        {
            FindOptions<T, TProjected> findOptions = new()
            {
                Projection = projection
            };

            using var cursor = await DbSet.FindAsync(filter, findOptions);

            return await cursor.ToListAsync();
        }

        public async Task<IEnumerable<TProjected>> FilterBy<TProjected>(
            Expression<Func<T, bool>> filter,
            ProjectionDefinition<T, TProjected> projection,
            SortDefinition<T> sort)
        {
            FindOptions<T, TProjected> findOptions = new()
            {
                Projection = projection,
                Sort = sort
            };

            using var cursor = await DbSet.FindAsync(filter, findOptions);

            return await cursor.ToListAsync();
        }

        public Task<T> FindOneAsync(Expression<Func<T, bool>> filter)
        {
            return DbSet.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<TProjected> FindOneAsync<TProjected>(
            Expression<Func<T, bool>> filter,
            ProjectionDefinition<T, TProjected> projection)
        {
            FindOptions<T, TProjected> findOptions = new()
            {
                Projection = projection,
                Limit = 1
            };

            using var cursor = await DbSet.FindAsync(filter, findOptions);

            return await cursor.FirstOrDefaultAsync();
        }
        public Task<List<string>> FindDistict(Expression<Func<T, string>> fieldName, Expression<Func<T, bool>> filterExpression)
        {
            return DbSet.Distinct(fieldName, filterExpression).ToListAsync();
        }

        /// <summary>
        /// https://stackoverflow.com/questions/34520357/paging-mongodb-query-with-c-sharp-drivers
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<SearchResult<T>> GetPagerResultAsync(Expression<Func<T, bool>> filter, int page, int pageSize)
        {
            // count facet, aggregation stage of count
            var countFacet = AggregateFacet.Create("countFacet",
                PipelineDefinition<T, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<T>()
                }));

            // data facet, we’ll use this to sort the data and do the skip and limiting of the results for the paging.
            var dataFacet = AggregateFacet.Create("dataFacet",
                PipelineDefinition<T, T>.Create(new[]
                {
                    //PipelineStageDefinitionBuilder.Sort(Builders<T>.Sort.Ascending(x => x.Surname)),
                    PipelineStageDefinitionBuilder.Skip<T>((page - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<T>(pageSize),
                }));

            var aggregation = await DbSet.Aggregate()
                .Match(filter)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var count = aggregation.First()
                        .Facets.First(x => x.Name == "countFacet")
                        .Output<AggregateCountResult>().Count;

            var data = aggregation.First()
                .Facets.First(x => x.Name == "dataFacet")
                .Output<T>();

            return new SearchResult<T>(count, new List<T>(data));
        }

        public Task InsertOneAsync(T document)
        {
            return DbSet.InsertOneAsync(document);
        }

        public Task InsertManyAsync(ICollection<T> documents)
        {
            return DbSet.InsertManyAsync(documents);
        }

        public Task<UpdateResult> UpdateOneAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            return DbSet.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
        }

        public Task<UpdateResult> UpdateManyAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            return DbSet.UpdateManyAsync(filter, update, new UpdateOptions { IsUpsert = false });
        }

        public Task ReplaceOneAsync(T document)
        {
            return ReplaceOneAsync(x => x.Id == document.Id, document);
        }

        public Task ReplaceOneAsync(Expression<Func<T, bool>> filter, T document)
        {
            var options = new FindOneAndReplaceOptions<T> { IsUpsert = true };
            return DbSet.FindOneAndReplaceAsync(filter, document, options);
        }

        public Task DeleteOneAsync(Expression<Func<T, bool>> filter)
        {
            return DbSet.FindOneAndDeleteAsync(filter);
        }

        public Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            return DbSet.FindOneAndDeleteAsync(filter);
        }

        public Task DeleteManyAsync(Expression<Func<T, bool>> filter)
        {
            return DbSet.DeleteManyAsync(filter);
        }
    }
}