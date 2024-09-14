using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.GraphQLSchema
{

	public class Query
	{
		// Dependency injection for the DbContext
		public Query(AppDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public AppDbContext DbContext { get; }

		// Query to get a list of users
		public IQueryable<User> GetUsers() => DbContext.Users;

		// Query to get a user by ID
		public User GetUserById(int id) => DbContext.Users.FirstOrDefault(user => user.Id == id);
	}


}
