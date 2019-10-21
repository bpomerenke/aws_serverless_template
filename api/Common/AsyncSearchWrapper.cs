using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace Common
{
    public interface IAsyncSearchWrapper<T>
    {
        bool IsDone { get; }

        Task<List<T>> GetRemainingAsync(CancellationToken cancellationToken);
    }
    
    public class AsyncSearchWrapper<T> : IAsyncSearchWrapper<T>
    {
        private readonly AsyncSearch<T> _asyncSearch;

        public AsyncSearchWrapper(AsyncSearch<T> asyncSearch)
        {
            _asyncSearch = asyncSearch;
        }

        public bool IsDone => _asyncSearch.IsDone;

        public Task<List<T>> GetRemainingAsync(CancellationToken cancellationToken)
        {
            return _asyncSearch.GetRemainingAsync(cancellationToken);
        }
    }

}