using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreApi.Core;
using CoreApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Schedule.Data.RepositoryInterfaces;
using Schedule.Model;
using Schedule.Model.Enums;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class SchedulesController : Controller
    {
        private readonly IRepositories.IScheduleRepository _scheduleRepository;
        private readonly IRepositories.IAttendeeRepository _attendeeRepository;
        private readonly IRepositories.IUserRepository _userRepository;
        private int page = 1;
        private int pageSize = 4;

        public SchedulesController(IRepositories.IScheduleRepository scheduleRepository, IRepositories.IAttendeeRepository attendeeRepository, IRepositories.IUserRepository userRepository)
        {
            _scheduleRepository = scheduleRepository;
            _attendeeRepository = attendeeRepository;
            _userRepository = userRepository;
        }

        public IActionResult Get()
        {
            var pagination = Request.Headers["Pagination"];

            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var totalSchedules = _scheduleRepository.Count();
            var totalPages = (int)Math.Ceiling((double)totalSchedules / pageSize);

            IEnumerable<Schedule.Model.Schedule> _schedules = _scheduleRepository
            .AllIncluding(s => s.Creator, s => s.Attendees)
            .OrderBy(s => s.Id)
            .Skip((currentPage - 1) * currentPageSize)
            .Take(currentPageSize)
            .ToList();

            Response.AddPagination(page, pageSize, totalSchedules, totalPages);

            IEnumerable<ScheduleViewModel> _schedulesVM =
                Mapper.Map<IEnumerable<Schedule.Model.Schedule>, IEnumerable<ScheduleViewModel>>(_schedules);

            return new OkObjectResult(_schedulesVM);
        }
        [HttpGet("{id}",Name = "GetSchedule")]
        public IActionResult Get(int id)
        {
            var _schedule = _scheduleRepository
                .GetSingle(s => s.Id == id, s => s.Attendees, s => s.Creator);

            if (_schedule != null)
            {
                ScheduleViewModel _scheduleViewModel =
                    Mapper.Map<Schedule.Model.Schedule, ScheduleViewModel>(_schedule);
                return new OkObjectResult(_scheduleViewModel);
            }

            return NotFound();
        }
        [HttpGet("{id}/details", Name = "GetScheduleDetails")]
        public IActionResult GetScheduleDetails(int id)
        {
            var _schedule = _scheduleRepository
                .GetSingle(s => s.Id == id, s => s.Attendees, s => s.Creator);
            if (_schedule != null)
            {
                var _scheduleVm = Mapper.Map<Schedule.Model.Schedule, ScheduleDetailsViewModel>(_schedule);

                foreach (var attendee in _schedule.Attendees)
                {
                    var _user = _userRepository.GetSingle(attendee.UserId);
                    _scheduleVm.Attendees.Add(Mapper.Map<User,UserViewModel>(_user));

                    return new OkObjectResult(_scheduleVm);
                }
                
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Create(ScheduleViewModel scheduleVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Schedule.Model.Schedule _schedule = Mapper.Map<ScheduleViewModel, Schedule.Model.Schedule>(scheduleVm);
            _schedule.DateCreated = DateTime.Now;

            _scheduleRepository.Add(_schedule);
            _scheduleRepository.Commit();

            foreach (var userId in scheduleVm.Attendees)
            {
                _schedule.Attendees.Add(new Attendee { UserId = userId });
            }
            _scheduleRepository.Commit();

            scheduleVm = Mapper.Map<Schedule.Model.Schedule, ScheduleViewModel>(_schedule);

            CreatedAtRouteResult result = CreatedAtRoute("GetSchedule", new { controller = "Schedules", id = scheduleVm.Id }, scheduleVm);
            return result;

        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]ScheduleViewModel schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Schedule.Model.Schedule _scheduleDb = _scheduleRepository.GetSingle(id);

            if (_scheduleDb == null)
            {
                return NotFound();
            }
            else
            {
                _scheduleDb.Title = schedule.Title;
                _scheduleDb.Location = schedule.Location;
                _scheduleDb.Description = schedule.Description;
                _scheduleDb.Status = (ScheduleStatus)Enum.Parse(typeof(ScheduleStatus), schedule.Status);
                _scheduleDb.Type = (ScheduleType)Enum.Parse(typeof(ScheduleType), schedule.Type);
                _scheduleDb.TimeStart = schedule.TimeStart;
                _scheduleDb.TimeEnd = schedule.TimeEnd;

                // Remove current attendees
                _attendeeRepository.DeleteWhere(a => a.ScheduleId == id);

                foreach (var userId in schedule.Attendees)
                {
                    _scheduleDb.Attendees.Add(new Attendee { ScheduleId = id, UserId = userId });
                }

                _scheduleRepository.Commit();
            }

            schedule = Mapper.Map<Schedule.Model.Schedule, ScheduleViewModel>(_scheduleDb);

            return new NoContentResult();
        }
        [HttpDelete("{id}", Name = "RemoveSchedule")]
        public IActionResult Delete(int id)
        {
            Schedule.Model.Schedule _schedule = 
                _scheduleRepository.GetSingle(id);

            if (_schedule == null)
            {
                return NotFound();
            }
            //remove all of the foreign key links
            _attendeeRepository.DeleteWhere(x => x.ScheduleId == _schedule.Id);
            _scheduleRepository.Delete(_schedule);

            _scheduleRepository.Commit();

            return new ContentResult();
        }

        [HttpDelete("{id}/removeattendee/{attendee}")]
        public IActionResult Delete(int id, int attendeeId)
        {
            Schedule.Model.Schedule _schedule = 
                _scheduleRepository.GetSingle(id);

            if (_schedule == null)
            {
                return NotFound();
            }

            _attendeeRepository.DeleteWhere(a => a.ScheduleId == id && a.UserId == attendeeId);
            _attendeeRepository.Commit();

            return new ContentResult();
        }

    }
}
