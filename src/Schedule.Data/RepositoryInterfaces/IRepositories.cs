using Schedule.Model;

namespace Schedule.Data.RepositoryInterfaces
{
    public class IRepositories
    {
        public interface IScheduleRepository : IEntityBaseRepository<Model.Schedule> { }

        public interface IUserRepository : IEntityBaseRepository<User> { }

        public interface IAttendeeRepository : IEntityBaseRepository<Attendee> { }
    }
}
