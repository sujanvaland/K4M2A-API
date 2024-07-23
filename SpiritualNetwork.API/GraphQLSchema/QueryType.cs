using SpiritualNetwork.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SpiritualNetwork.API.GraphQLSchema
{
    public class QueryType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor.Field(q => q.GetHello()).Type<StringType>();
        }
    }
    public class Query
    {

        public Query()
        {
                
        }
        public string GetHello() => "Hello from GraphQL!";

        public List<User> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
