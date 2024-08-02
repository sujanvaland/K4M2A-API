using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using SpiritualNetwork.API.Migrations;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using static HotChocolate.ErrorCodes;
using Event = SpiritualNetwork.Entities.Event;

namespace SpiritualNetwork.API.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<EventType> _eventtypeRepository;
        private readonly IRepository<EventAttendee> _eventAttendeeRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<EventSpeakers> _eventspeakersRepository;
        private readonly IRepository<EventComment> _eventCommentRepository;
        private readonly IRepository<UserFollowers> _userFollowers;
        private readonly IProfileService _profileService;
        private readonly IRepository<OnlineUsers> _onlineUsers;
        private readonly INotificationService _notificationService;

        public EventService(IRepository<Event> eventRepository,
            IRepository<EventType> eventtypeRepository,
            IRepository<EventAttendee> eventAttendeeRepository,
            IRepository<User> userRepository,
            IRepository<EventSpeakers> eventspeakersRepository,
            IRepository<EventComment> eventCommentRepository,
            IProfileService profileService,
            IRepository<UserFollowers> userFollowers,
            IRepository<OnlineUsers> onlineUsers,
            INotificationService notificationService)
        {
            _eventRepository = eventRepository;
            _eventtypeRepository = eventtypeRepository;
            _eventAttendeeRepository = eventAttendeeRepository;
            _userRepository = userRepository;
            _eventspeakersRepository = eventspeakersRepository;
            _eventCommentRepository = eventCommentRepository;
            _profileService = profileService;
            _userFollowers = userFollowers;
            _onlineUsers = onlineUsers;
            _notificationService = notificationService;
        }

        public async Task<Event> SaveEvent(EventReq req)
        {
            if (req.Event.Id == 0)
            {
                req.Event.StartDate = req.Event.StartDate.Value.Date;
                req.Event.EndDate = req.Event.EndDate.Value.Date;
                req.Event.StartTime = Convert.ToDateTime(req.Event.StartTime).ToShortTimeString();
                req.Event.EndTime = Convert.ToDateTime(req.Event.EndTime).ToShortTimeString();
                await _eventRepository.InsertAsync(req.Event);

                EventSpeakers speaker = new EventSpeakers();
                speaker.EventId = req.Event.Id;
                foreach (var UserId in req.SpeakersId)
                {
                    speaker.Id = 0;
                    speaker.Type = "speaker";
                    speaker.UserId = UserId;
                    await SaveEventSpeaker(speaker);
                }
                foreach (var UserId in req.HostsId)
                {
                    speaker.Id = 0;
                    speaker.Type = "host";
                    speaker.UserId = UserId;
                    await SaveEventSpeaker(speaker);
                }
            }
            else
            {
                await _eventRepository.UpdateAsync(req.Event);
            }

            return req.Event;
        }

        public async Task<JsonResponse> GetMyEventList(EventListReq req, int Size, int userId)
        {
            DateTime fromdate = DateTime.Today.Date, todate = DateTime.Today.Date;
            if (req.Period == 1)
            {
                fromdate = DateTime.Today.Date;
                todate = DateTime.Today.Date;
            }
            else if (req.Period == 2)
            {
                DateTime today = DateTime.Today;
                DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                fromdate = today;
                todate = endOfMonth;
            }
            else if (req.Period == 3)
            {
                DateTime today = DateTime.Today;
                DateTime firstofNextMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)).AddDays(1);
                today = firstofNextMonth;
                DateTime lastofNextMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                fromdate = firstofNextMonth;
                todate = lastofNextMonth;
            }

            var filterEvent = _eventRepository.Table;
            if (req.Period > 0)
            {
                filterEvent = filterEvent.Where(x => x.StartDate >= fromdate && x.EndDate <= todate);
            }
            if (req.EventTypeId > 0)
            {
                filterEvent = filterEvent.Where(x => x.EventTypeId == req.EventTypeId);

            }
            if (req.EventFormat != null)
            {
                filterEvent = filterEvent.Where(x => x.EventFormat == req.EventFormat);
            }

            var eventsquery = (from ev in filterEvent
                              join at in _eventAttendeeRepository.Table on ev.Id equals at.EventId into attend
                              from at in attend.DefaultIfEmpty()
                              join u in _userRepository.Table on ev.CreatedBy equals u.Id
                              where at.UserId == userId || ev.CreatedBy == userId
                              && ev.IsDeleted == false
                              select new EventModel
                              {
                                  Id = ev.Id,
                                  EventTitle = ev.EventTitle,
                                  EventFormat = ev.EventFormat,
                                  EventTypeId = ev.EventTypeId,
                                  StartDate = ev.StartDate,
                                  EndDate = ev.EndDate,
                                  StartTime = ev.StartTime,
                                  EndTime = ev.EndTime,
                                  Description = ev.Description,
                                  EventLink = ev.EventLink,
                                  Eventaddress = ev.Eventaddress,
                                  EventCoverImage = ev.EventCoverImage,
                                  Hostedby = u.FirstName + " " + u.LastName,
                                  CreatedBy = ev.CreatedBy,
                                  TimeZone = ev.TimeFrame,
                                  Country = ev.Country,
                                  State = ev.State,
                                  City = ev.City,
                                  Access = ev.Access,
                                  IsCommenting = ev.IsCommenting,
                                  IsApprove = ev.IsApprove,
                                  EventType = _eventtypeRepository.Table.Where(x => x.Id == ev.EventTypeId).Select(x => x.EventTypeName).FirstOrDefault()
                              }).Distinct();
            var totalCount = eventsquery.Count();
            var eventlist = await eventsquery.Skip((req.PageNo - 1) * Size).Take(Size).ToListAsync();

            foreach (var item in eventlist)
            {
                var eventid = item.Id;
                item.EventAttendees = (from ea in _eventAttendeeRepository.Table
                                       join us in _userRepository.Table on ea.UserId equals us.Id
                                       join uf in _userFollowers.Table.Where(x => x.UserId == userId) on us.Id equals uf.FollowToUserId into ufGroup
                                       from uf in ufGroup.DefaultIfEmpty()
                                       where ea.EventId == eventid
                                       select new EventAttend
                                       {
                                           EventId = ea.EventId,
                                           Id = us.Id,
                                           FirstName = us.FirstName,
                                           LastName = us.LastName,
                                           UserName = us.UserName,
                                           ProfileImgUrl = us.ProfileImg,
                                           IsFollowing = uf != null,
                                           IsDisplay = !(uf != null),
                                           IsBusinessAccount = us.IsBusinessAccount
                                       }).ToList();

                item.EventSpeakers = (from es in _eventspeakersRepository.Table
                                      join us in _userRepository.Table on es.UserId equals us.Id
                                      join uf in _userFollowers.Table.Where(x => x.UserId == userId) on us.Id equals uf.FollowToUserId into ufGroup
                                      from uf in ufGroup.DefaultIfEmpty()
                                      where es.EventId == eventid && es.Type == "speaker"
                                      select new EventAttend
                                      {
                                          EventId = es.EventId,
                                          Id = us.Id,
                                          FirstName = us.FirstName,
                                          LastName = us.LastName,
                                          UserName = us.UserName,
                                          ProfileImgUrl = us.ProfileImg,
                                          IsFollowing = uf != null,
                                          IsDisplay = !(uf != null),
                                          IsBusinessAccount = us.IsBusinessAccount

                                      }).ToList();

                item.EventHosts = (from es in _eventspeakersRepository.Table
                                   join us in _userRepository.Table on es.UserId equals us.Id
                                   join uf in _userFollowers.Table.Where(x => x.UserId == userId) on us.Id equals uf.FollowToUserId into ufGroup
                                   from uf in ufGroup.DefaultIfEmpty()
                                   where es.EventId == eventid && es.Type == "host"
                                      select new EventAttend
                                      {
                                          EventId = es.EventId,
                                          Id = us.Id,
                                          FirstName = us.FirstName,
                                          LastName = us.LastName,
                                          UserName = us.UserName,
                                          ProfileImgUrl = us.ProfileImg,
                                          IsFollowing = uf != null,
                                          IsDisplay = !(uf != null),
                                          IsBusinessAccount = us.IsBusinessAccount
                                      }).ToList();

                item.EventComments = (from ec in _eventCommentRepository.Table
                                      join us in _userRepository.Table on ec.UserId equals us.Id
                                      where ec.EventId == eventid
                                      select new EventCommenter
                                      {
                                          EventId = ec.EventId,
                                          UserId = ec.UserId,
                                          UserFullName = us.FirstName + " " + us.LastName,
                                          ProfileImgUrl = us.ProfileImg,
                                          UserName = us.UserName,
                                          Comment = ec.Comment,
                                          IsBusinessAccount =us.IsBusinessAccount
                                      }).ToList();

            }
            EventListModel model = new EventListModel();
            model.ListCount = totalCount;
            model.Size = Size;
            model.EventList = eventlist;

            return new JsonResponse(200, true, "Success", model);
        }
        public async Task<JsonResponse> GetEventList(EventListReq req, int Size, int userId)
        {
            DateTime fromdate=DateTime.Today.Date, todate= DateTime.Today.Date;
            if(req.Period == 1)
            {
                fromdate = DateTime.Today.Date;
                todate = DateTime.Today.Date;
            }
            else if(req.Period == 2)
            {
                DateTime today = DateTime.Today;
                DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                fromdate = today;
                todate = endOfMonth;
            }
            else if (req.Period == 3)
            {
                DateTime today = DateTime.Today;
                DateTime firstofNextMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)).AddDays(1);
                today = firstofNextMonth;
                DateTime lastofNextMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                fromdate = firstofNextMonth;
                todate = lastofNextMonth;
            }

            var filterEvent = _eventRepository.Table.Where(x=> x.StartDate >= fromdate && x.IsDeleted == false);
            if (req.Period > 0)
            {
                filterEvent = filterEvent.Where(x => x.StartDate >= fromdate && x.EndDate <= todate);
            }
            if(req.EventTypeId > 0) 
            {
                filterEvent = filterEvent.Where(x => x.EventTypeId == req.EventTypeId);
            }
            if (req.EventFormat != null)
            {
                filterEvent = filterEvent.Where(x => x.EventFormat == req.EventFormat);
            }

            if (req.Search.Trim().Length > 0)
            {
                filterEvent = filterEvent.Where(x => x.EventTitle.ToLower().Contains(req.Search.ToLower()) || 
                                                x.Description.ToLower().Contains(req.Search.ToLower()));
            }

            var count = filterEvent.Count();

            var eventsquery = from ev in filterEvent
                              join u in _userRepository.Table on ev.CreatedBy equals u.Id
                              select new EventModel
                              {
                                  Id = ev.Id,
                                  EventTitle = ev.EventTitle,
                                  EventFormat = ev.EventFormat,
                                  EventTypeId = ev.EventTypeId,
                                  StartDate = ev.StartDate,
                                  EndDate = ev.EndDate,
                                  StartTime = ev.StartTime,
                                  EndTime = ev.EndTime,
                                  Description = ev.Description,
                                  EventLink = ev.EventLink,
                                  Eventaddress = ev.Eventaddress,
                                  EventCoverImage = ev.EventCoverImage,
                                  Hostedby = u.FirstName+" "+u.LastName,
                                  CreatedBy = ev.CreatedBy,
                                  TimeZone = ev.TimeFrame,
                                  Country = ev.Country,
                                  State = ev.State,
                                  City = ev.City,
                                  Access = ev.Access,
                                  IsCommenting = ev.IsCommenting,
                                  IsApprove = ev.IsApprove,
                                  EventType = _eventtypeRepository.Table.Where(x => x.Id == ev.EventTypeId).Select(x => x.EventTypeName).FirstOrDefault() ?? ""
                              };
            var totalCount = eventsquery.Count();
            var eventlist = await eventsquery.OrderBy(x => x.StartDate).Skip((req.PageNo - 1) * Size).Take(Size).ToListAsync();
            
            foreach (var item in eventlist)
            {
                var eventid = item.Id;
                item.EventAttendees = (from ea in _eventAttendeeRepository.Table
                                       join us in _userRepository.Table on ea.UserId equals us.Id
                                       join uf in _userFollowers.Table.Where(x => x.UserId == userId) on us.Id equals uf.FollowToUserId into ufGroup
                                       from uf in ufGroup.DefaultIfEmpty()
                                       where ea.EventId == eventid
                                       select new EventAttend
                                       {
                                           EventId = ea.EventId,
                                           Id = us.Id,
                                           FirstName = us.FirstName,
                                           LastName = us.LastName,
                                           UserName = us.UserName,
                                           ProfileImgUrl = us.ProfileImg,
                                           IsFollowing = uf != null,
                                           IsDisplay = !(uf != null),
                                           IsBusinessAccount = us.IsBusinessAccount

                                       }).ToList();

                var Ishost = _eventspeakersRepository.Table.Where(x=> x.Type == "host" && x.EventId == eventid && x.IsDeleted == false && x.UserId == userId).Count() > 0; 

                if (Ishost || item.CreatedBy == userId)
                {
                    item.AttendeesReq = await _eventAttendeeRepository.Table.Where(x=> x.Isjoin == true && x.IsDeleted == false && x.EventId == eventid).CountAsync();
                }

                item.EventSpeakers = (from es in _eventspeakersRepository.Table
                                      join us in _userRepository.Table on es.UserId equals us.Id
                                      join uf in _userFollowers.Table.Where(x => x.UserId == userId) on us.Id equals uf.FollowToUserId into ufGroup
                                      from uf in ufGroup.DefaultIfEmpty()
                                      where es.EventId == eventid && es.Type =="speaker"
                                      select new EventAttend
                                      {
                                          EventId = es.EventId,
                                          Id = us.Id,
                                          FirstName = us.FirstName,
                                          LastName = us.LastName,
                                          UserName = us.UserName,
                                          ProfileImgUrl = us.ProfileImg,
                                          IsFollowing = uf != null,
                                          IsDisplay = !(uf != null),
                                          IsBusinessAccount = us.IsBusinessAccount

                                      }).ToList();

                item.EventHosts = (from es in _eventspeakersRepository.Table
                                   join us in _userRepository.Table on es.UserId equals us.Id
                                   join uf in _userFollowers.Table.Where(x => x.UserId == userId) on us.Id equals uf.FollowToUserId into ufGroup
                                   from uf in ufGroup.DefaultIfEmpty()
                                   where es.EventId == eventid && es.Type == "host"
                                    select new EventAttend
                                    {
                                        EventId = es.EventId,
                                        Id = us.Id,
                                        FirstName = us.FirstName,
                                        LastName = us.LastName,
                                        UserName = us.UserName,
                                        ProfileImgUrl = us.ProfileImg,
                                        IsFollowing = uf != null,
                                        IsDisplay = !(uf != null),
                                        IsBusinessAccount = us.IsBusinessAccount

                                    }).ToList();

                item.EventComments = (from ec in _eventCommentRepository.Table
                                        join us in _userRepository.Table on ec.UserId equals us.Id
                                        where ec.EventId == eventid
                                        select new EventCommenter
                                        {
                                            EventId = ec.EventId,
                                            UserId = ec.UserId,
                                            UserFullName = us.FirstName + " " + us.LastName,
                                            ProfileImgUrl = us.ProfileImg,
                                            UserName = us.UserName,
                                            Comment = ec.Comment,
                                            IsBusinessAccount = us.IsBusinessAccount

                                        }).ToList();

            }
            EventListModel model = new EventListModel();
            model.ListCount = totalCount;
            model.Size = Size;
            model.EventList = eventlist;

            return new JsonResponse(200, true, "Success", model);
        }

        public async Task<JsonResponse> GetEventTypeList()
        {
            var eventtypelist = await _eventtypeRepository.Table.ToListAsync();

            return new JsonResponse(200, true, "Success", eventtypelist);
        }


        public async Task<JsonResponse> AddUpdateEventMember(AddRemoveEventMember req, int UserId)
        {

            var Admin = await _eventspeakersRepository.Table.Where(x => x.UserId == UserId && x.Type == "host"
                              && x.EventId == req.EventId && x.IsDeleted == false).FirstOrDefaultAsync();

            var community = await _eventRepository.Table.Where(x => x.Id == req.EventId && x.IsDeleted == false).FirstOrDefaultAsync();

            if (Admin == null && community.CreatedBy != UserId)
            {
                return new JsonResponse(200, true, "Only Admin And Host Can Take Action", null);
            }

            if (req.Action == "addattendees")
            {
                foreach (var id in req.UserId)
                {
                    var ever = await _eventAttendeeRepository.Table.Where(x => x.UserId == id
                             && x.EventId == req.EventId).FirstOrDefaultAsync();

                    if (ever?.IsDeleted == false)
                    {
                        continue;
                    }

                    if (ever == null)
                    {
                        EventAttendee member = new EventAttendee();
                        member.EventId = req.EventId;
                        member.UserId = id;
                        await _eventAttendeeRepository.InsertAsync(member);
                    }
                    else
                    {
                        ever.IsDeleted = false;
                        ever.Isjoin = false;
                        await _eventAttendeeRepository.UpdateAsync(ever);
                    }

                    NotificationRes notification = new NotificationRes();
                    notification.PostId = req.EventId;
                    notification.ActionByUserId = UserId;
                    notification.ActionType = req.Action;
                    notification.RefId2 = req.EventId.ToString();
                    notification.RefId1 = req.UserId.ToString();
                    notification.Message = "You Have Been Added By Host To Attend Event " + community.EventTitle ;

                    await _notificationService.SaveNotification(notification);
                }
                return new JsonResponse(200, true, "Success", null);
            }

            if (req.Action == "inviteattendee")
            {
                foreach (var id in req.UserId)
                {
                    var ever = await _eventAttendeeRepository.Table.Where(x => x.UserId == id
                             && x.EventId == req.EventId).FirstOrDefaultAsync();

                    if (ever?.IsInvited == true)
                    {
                        continue;
                    }

                    if (ever == null)
                    {
                        EventAttendee member = new EventAttendee();
                        member.EventId = req.EventId;
                        member.UserId = id;
                        member.IsInvited = true;
                        member.IsDeleted = true;
                        await _eventAttendeeRepository.InsertAsync(member);
                    }
                    else
                    {
                        ever.Isjoin = false;
                        ever.IsInvited = true;
                        await _eventAttendeeRepository.UpdateAsync(ever);
                    }

                    NotificationRes notification = new NotificationRes();
                    notification.PostId = req.EventId;
                    notification.ActionByUserId = UserId;
                    notification.ActionType = req.Action;
                    notification.RefId2 = req.EventId.ToString();
                    notification.RefId1 = id.ToString();
                    notification.Message = "Invited To Attend Event " + community.EventTitle;
                    await _notificationService.SaveNotification(notification);
                }
                return new JsonResponse(200, true, "Success", null);
            }

            if(req.Action == "removeattendee")
            {
                var isattended = await  _eventAttendeeRepository.Table.Where(x => x.UserId == req.UserId[0]
                              && x.EventId == req.EventId && x.IsDeleted).FirstOrDefaultAsync();

                 _eventAttendeeRepository.DeleteHard(isattended);

                return new JsonResponse(200, true, "Remove Successfully", null);
            }

            if (community.CreatedBy != UserId)
            {
                return new JsonResponse(200, true, "Only Admin Can Take Action", null);
            }

            var check = await _eventspeakersRepository.Table.Where(x => x.UserId == req.UserId[0]
                              && x.EventId == req.EventId && x.IsDeleted).FirstOrDefaultAsync();

            if (req.Action == "makehost")
            {
                if (check.Type == "host" && check != null)
                {
                    return new JsonResponse(200, true, "This user is Already a Host", null);
                }
                else
                {
                    EventSpeakers speaker = new EventSpeakers();
                    speaker.EventId = req.EventId;
                    speaker.Id = 0;
                    speaker.Type = "host";
                    speaker.UserId = req.UserId[0];
                    await SaveEventSpeaker(speaker);

                    NotificationRes notification = new NotificationRes();
                    notification.PostId = req.EventId;
                    notification.ActionByUserId = UserId;
                    notification.ActionType = req.Action;
                    notification.RefId2 = req.EventId.ToString();
                    notification.RefId1 = req.UserId[0].ToString();
                    notification.Message = "You Are Now Host To Event" + community.EventTitle;
                    await _notificationService.SaveNotification(notification);

                    return new JsonResponse(200, true, "You Created New Host ", null);
                }

            }
            if (req.Action == "removehost")
            {
                if (check.Type == "host" && check != null)
                {
                     _eventspeakersRepository.DeleteHard(check);
                }
                return new JsonResponse(200, true, "You Remove Host", null);
            }
            if (req.Action == "makespeaker")
            {
                if (check.Type == "speaker" && check != null)
                {
                    return new JsonResponse(200, true, "This user is Already a speaker", null);
                }
                else
                {
                    EventSpeakers speaker = new EventSpeakers();
                    speaker.EventId = req.EventId;
                    speaker.Id = 0;
                    speaker.Type = "speaker";
                    speaker.UserId = req.UserId[0];
                    await SaveEventSpeaker(speaker);

                    NotificationRes notification = new NotificationRes();
                    notification.PostId = req.EventId;
                    notification.ActionByUserId = UserId;
                    notification.ActionType = req.Action;
                    notification.RefId2 = req.EventId.ToString();
                    notification.RefId1 = req.UserId[0].ToString();
                    notification.Message = "You Are Now Speaker To Event" + community.EventTitle;

                    await _notificationService.SaveNotification(notification);

                    return new JsonResponse(200, true, "You Created New speaker ", null);
                }
            }
            if (req.Action == "removespeaker")
            {
                if (check.Type == "speaker" && check != null)
                {
                    _eventspeakersRepository.DeleteHard(check);
                }
                return new JsonResponse(200, true, "You Remove speaker", null);
            }

            return new JsonResponse(200, false, "Success", null);
        }

        public async Task<EventModel> GetEventById(int id, int UserId)
        {
            var eventval = await _eventRepository.Table.Where(x => x.Id == id).FirstOrDefaultAsync();
            var hostedUser = await _userRepository.Table.Where(x => x.Id == eventval.CreatedBy).FirstOrDefaultAsync();
            var attend = await _eventAttendeeRepository.Table.Where(x => x.UserId == UserId && x.EventId == id && x.IsDeleted == false).FirstOrDefaultAsync();
            var Admin = await _eventspeakersRepository.Table.Where(x => x.UserId == UserId && x.Type == "host"
                             && x.EventId == id && x.IsDeleted == false).FirstOrDefaultAsync();

            var community = await _eventRepository.Table.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
           
            var eventModel = new EventModel();
            eventModel.Id = id;
            eventModel.EventTitle = eventval.EventTitle;
            eventModel.Eventaddress = eventval.Eventaddress;
            eventModel.EventLink = eventval.EventLink;
            eventModel.EventFormat = eventval.EventFormat;
            eventModel.StartDate = eventval.StartDate;
            eventModel.EndDate = eventval.EndDate;
            eventModel.StartTime = eventval.StartTime;
            eventModel.EndTime = eventval.EndTime;
            eventModel.Description = eventval.Description;
            eventModel.Hostedby = hostedUser.FirstName + " " + hostedUser.LastName;
            eventModel.EventTypeId = eventval.EventTypeId;
            eventModel.CreatedBy = eventval.CreatedBy;
            eventModel.TimeZone = eventval.TimeFrame;
            eventModel.Country = eventval.Country;
            eventModel.State = eventval.State;
            eventModel.City = eventval.City;
            eventModel.EventCoverImage= eventval.EventCoverImage;
            eventModel.Access = eventval.Access;
            eventModel.IsCommenting = eventval.IsCommenting;
            eventModel.IsApprove = eventval.IsApprove;
            eventModel.EventType = _eventtypeRepository.Table.Where(x => x.Id == eventval.EventTypeId).Select(x => x.EventTypeName).FirstOrDefault();
            eventModel.IsPending = attend == null ? false : attend.Isjoin;
            eventModel.IsJoin = attend != null ? attend.Isjoin == false ? true : false : false;

            if (Admin != null || community.CreatedBy == UserId)
            {
                eventModel.InvitedUsers = await _eventAttendeeRepository.Table.Where(x=> x.IsInvited == true && x.EventId == id)
                                          .Select(x=> x.UserId).ToListAsync();        
            }

            eventModel.EventAttendees = (from ea in _eventAttendeeRepository.Table
                                         join us in _userRepository.Table on ea.UserId equals us.Id
                                         join uf in _userFollowers.Table.Where(x => x.UserId == UserId) on us.Id equals uf.FollowToUserId into ufGroup
                                         from uf in ufGroup.DefaultIfEmpty()
                                         where ea.EventId == id && ea.Isjoin == false && ea.IsDeleted == false
                                         select new EventAttend
                                         {
                                             EventId = ea.EventId,
                                             Id = us.Id,
                                             FirstName = us.FirstName,
                                             LastName = us.LastName,
                                             UserName = us.UserName,
                                             ProfileImgUrl = us.ProfileImg,
                                             IsFollowing = uf != null,
                                             IsDisplay = !(uf != null),
                                             IsBusinessAccount = us.IsBusinessAccount

                                         }).ToList();

            eventModel.EventSpeakers = (from es in _eventspeakersRepository.Table
                                        join us in _userRepository.Table on es.UserId equals us.Id
                                        join uf in _userFollowers.Table.Where(x => x.UserId == UserId) on us.Id equals uf.FollowToUserId into ufGroup
                                        from uf in ufGroup.DefaultIfEmpty()
                                        where es.EventId == id && es.IsDeleted == false && es.Type == "speaker"
                                        select new EventAttend
                                        {
                                            EventId = es.EventId,
                                            Id = us.Id,
                                            FirstName = us.FirstName,
                                            LastName = us.LastName,
                                            UserName = us.UserName,
                                            ProfileImgUrl = us.ProfileImg,
                                            IsFollowing = uf != null,
                                            IsDisplay = !(uf != null),
                                          IsBusinessAccount = us.IsBusinessAccount

                                        }).ToList();
            eventModel.EventHosts = (from es in _eventspeakersRepository.Table
                                     join us in _userRepository.Table on es.UserId equals us.Id
                                     join uf in _userFollowers.Table.Where(x => x.UserId == UserId) on us.Id equals uf.FollowToUserId into ufGroup
                                     from uf in ufGroup.DefaultIfEmpty()
                                     where es.EventId == id && es.IsDeleted == false && es.Type == "host"
                                       select new EventAttend
                                       {
                                           EventId = es.EventId,
                                           Id = us.Id,
                                           FirstName = us.FirstName,
                                           LastName = us.LastName,
                                           UserName = us.UserName,
                                           ProfileImgUrl = us.ProfileImg,
                                           IsFollowing = uf != null,
                                           IsDisplay= !(uf != null),
                                           IsBusinessAccount = us.IsBusinessAccount

                                       }).ToList();

            eventModel.EventComments = (from ec in _eventCommentRepository.Table
                                          join us in _userRepository.Table on ec.UserId equals us.Id
                                          where ec.EventId == id && ec.IsDeleted == false
                                        select new EventCommenter
                                          {
                                              EventId = ec.EventId,
                                              UserId = ec.UserId,
                                              UserFullName = us.FirstName + " " + us.LastName,
                                              ProfileImgUrl = us.ProfileImg,
                                              UserName = us.UserName,
                                              Comment = ec.Comment,
                                            IsBusinessAccount = us.IsBusinessAccount

                                        }).ToList();

            return eventModel;
        }
        public async Task<JsonResponse> SaveAttendEvent(EventAttendee Attendee)
        {
            var check = _eventAttendeeRepository.Table.Where(x=> x.UserId == Attendee.UserId 
                            && x.EventId == Attendee.EventId).FirstOrDefault();
            var Event = await _eventRepository.Table.Where(x => x.Id == Attendee.EventId).FirstOrDefaultAsync();
            if (Event == null)
            {
                return new JsonResponse(200, true, "Event Not Found", null);
            }
            if (check == null) 
            { 
                Attendee.Isjoin = Event.IsApprove;
                await _eventAttendeeRepository.InsertAsync(Attendee);

                var ActionUser = await _userRepository.Table.Where(x => x.Id == Attendee.UserId
                                && x.IsDeleted == false).FirstOrDefaultAsync();

                NotificationRes notification = new NotificationRes();
                notification.PostId = Attendee.EventId;
                notification.ActionByUserId = Attendee.UserId;
                notification.ActionType = "eventattend";
                notification.RefId1 = Event.CreatedBy.ToString();
                notification.RefId2 = Attendee.EventId.ToString();
                notification.Message = ActionUser.FirstName + " " + ActionUser.LastName + (Event.IsApprove ? " is Requet to Attend Your Event " : " is going to Attend Your Event");
                await _notificationService.SaveNotification(notification);

            }
            else
            {
                check.IsDeleted = false;
                check.Isjoin = check.IsInvited ? false : Event.IsApprove ;
                await _eventAttendeeRepository.UpdateAsync(Attendee);
            }
            return new JsonResponse(200, true, "Success", Attendee);
        }

        public async Task<JsonResponse> AttendeesApprovDeny(EventAttendRequest req, int UserId)
        {
            var Admin = await _eventRepository.Table.Where(x => x.CreatedBy == UserId
                             && x.Id == req.EventId && x.IsDeleted == false).FirstOrDefaultAsync();

            if (Admin == null)
            {
                return new JsonResponse(200, true, "Only Admin Can Take Action", null);
            }

            var isReq = await _eventAttendeeRepository.Table.Where(x => x.UserId == req.UserId
                && x.EventId == req.EventId && x.Isjoin == true && x.IsDeleted == false).FirstOrDefaultAsync();

            if (req.Action == "approve" && isReq != null)
            {
                isReq.Isjoin = false;
                await _eventAttendeeRepository.UpdateAsync(isReq);
            }

            if (req.Action == "deny" && isReq != null)
            {
                isReq.IsDeleted = true;
                isReq.Isjoin = false;
                await _eventAttendeeRepository.UpdateAsync(isReq);
            }

            var receiverConnectionId = await _onlineUsers.Table
                .Where(x => x.IsDeleted == false && x.UserId == req.UserId)
                .FirstOrDefaultAsync();

            if (receiverConnectionId == null)
                return new JsonResponse(200, true, "Success", null);

            NotificationRes notification = new NotificationRes();
            notification.PostId = 0;
            notification.ActionByUserId = Admin.CreatedBy;
            notification.ActionType = "eventapprovdeny";
            notification.RefId1 = req.UserId.ToString();
            notification.RefId2 = req.Action;
            notification.Message =  "Your Request to Attend "+ Admin.EventTitle+ " Event is " +(req.Action == "approve" ? "Approved" : "Deny");
            notification.connectionIds.Add(receiverConnectionId.ConnectionId);
            await _notificationService.SaveNotification(notification);

            return new JsonResponse(200, true, "Success", null);
        }

        public async Task<JsonResponse> EventAttendeesRequestList(int UserId, int EventId)
        {
            var Admin = await _eventRepository.Table.Where(x => x.CreatedBy == UserId
                             && x.Id == EventId && x.IsDeleted == false).FirstOrDefaultAsync();
            var host = await _eventspeakersRepository.Table.Where(x => x.Type == "host" && x.IsDeleted == false && x.EventId == EventId && x.UserId == UserId).CountAsync() > 0;

            if (Admin == null && !host)
            {
                return new JsonResponse(200, true, "Only Admin Can Take Action", null);
            }

            var list = (from ea in _eventAttendeeRepository.Table
                        join us in _userRepository.Table on ea.UserId equals us.Id
                        where ea.EventId == EventId && ea.IsDeleted == false && ea.Isjoin == true
                        select new EventAttend
                        {
                            EventId = ea.EventId,
                            Id = us.Id,
                            FirstName = us.FirstName,
                            LastName = us.LastName,
                            UserName = us.UserName,
                            ProfileImgUrl = us.ProfileImg,
                            IsBusinessAccount = us.IsBusinessAccount

                        }).ToList();

            return new JsonResponse(200, true, "Success", list);
        }

        public async Task<EventSpeakers> SaveEventSpeaker(EventSpeakers eventspeaker)
        {
            var check = _eventspeakersRepository.Table.Where(x=> x.UserId == eventspeaker.UserId
                            && x.EventId == eventspeaker.EventId).FirstOrDefault();
            if (eventspeaker.Id == 0 && check == null)
            {
                await _eventspeakersRepository.InsertAsync(eventspeaker);
            }

            return eventspeaker;
        }

        public async Task<EventComment> SaveEventComment(EventComment req)
        {
            if (req.Id == 0)
            {
                await _eventCommentRepository.InsertAsync(req);
            }
            return req;
        }

        public async Task<JsonResponse> DeleteEvent(int id)
        {
            var events = await _eventRepository.Table.Where(x => x.Id == id).FirstOrDefaultAsync();
            if(events != null)
            {
                var attendess = _eventAttendeeRepository.Table.Where(x => x.EventId == id).ToList();
                await _eventAttendeeRepository.DeleteRangeAsync(attendess);

                var speakers = _eventspeakersRepository.Table.Where(x => x.EventId == id).ToList();
                await _eventspeakersRepository.DeleteRangeAsync(speakers);
                await _eventRepository.DeleteAsync(events);
            }
            return new JsonResponse(200, true, "Success", null);
        }


    }
}
