using Schedule.Data.DbContext;
using Schedule.Data.RepositoryInterfaces;

namespace Schedule.Data.Repositories
{
    public class ScheduleRepository : EntityBaseRepository<Model.Schedule>, IRepositories.IScheduleRepository
    {
        public ScheduleRepository(SchedulerContext context)
            : base(context)
        { }
    }
}
