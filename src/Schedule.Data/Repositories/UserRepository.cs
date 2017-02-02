using Schedule.Data.DbContext;
using Schedule.Data.RepositoryInterfaces;
using Schedule.Model;

namespace Schedule.Data.Repositories
{
    public class UserRepository : EntityBaseRepository<User>, IRepositories.IUserRepository
    {
        public UserRepository(SchedulerContext context)
            : base(context)
        { }
    }
}
