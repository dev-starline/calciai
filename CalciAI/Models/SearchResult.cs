using System.Collections.Generic;

namespace CalciAI.Models
{
    public class SearchResult<T>
    {
        public SearchResult(long totalDocuments, List<T> documents)
        {
            Documents = documents;
            Page = new Page
            {
                Count = (int)totalDocuments
            };
        }

        public List<T> Documents { get; }

        public Page Page { get; set; }
    }
}
