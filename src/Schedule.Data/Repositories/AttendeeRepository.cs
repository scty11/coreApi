using Schedule.Data.DbContext;
using Schedule.Data.RepositoryInterfaces;
using Schedule.Model;

namespace Schedule.Data.Repositories
{
    public class AttendeeRepository : EntityBaseRepository<Attendee>, IRepositories.IAttendeeRepository
    {
        public AttendeeRepository(SchedulerContext context)
            : base(context)
        { }
    }
}
