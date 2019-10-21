using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace Common
{
    public interface IDynamoDbContextWrapper : IDynamoDBContext
    {
        new IAsyncSearchWrapper<T> ScanAsync<T>(IEnumerable<ScanCondition> conditions, DynamoDBOperationConfig config = null);

        new IAsyncSearchWrapper<T> QueryAsync<T>(object hashKeyValue, DynamoDBOperationConfig config = null);
    }

    public class DynamoDbContextWrapper : DynamoDBContext, IDynamoDbContextWrapper
    {
        public DynamoDbContextWrapper(IAmazonDynamoDB client) : base(client)
        {
        }

        public DynamoDbContextWrapper(IAmazonDynamoDB client, DynamoDBContextConfig config) : base(client, config)
        {
        }

        public new IAsyncSearchWrapper<T> ScanAsync<T>(IEnumerable<ScanCondition> conditions, DynamoDBOperationConfig config = null)
        {
            var result = base.ScanAsync<T>(conditions, config);

            return new AsyncSearchWrapper<T>(result);
        }

        public new IAsyncSearchWrapper<T> QueryAsync<T>(object hashKeyValue, DynamoDBOperationConfig config = null)
        {
            var result = base.QueryAsync<T>(hashKeyValue, config);

            return new AsyncSearchWrapper<T>(result);
        }
    }

}