using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EventController : ApiBaseController
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }


        [HttpPost(Name = "SaveEvent")]
        public async Task<JsonResponse> SaveEvent(EventReq eventdata)
        {
            try
            {
                var response = await _eventService.SaveEvent(eventdata);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [HttpPost(Name = "GetEventList")]
        public async Task<JsonResponse> GetEventList(EventListReq req)
        {
            try
            {
                return await _eventService.GetEventList(req, 15, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetMyEventList")]
        public async Task<JsonResponse> GetMyEventList(EventListReq req)
        {
            try
            {
                return await _eventService.GetMyEventList(req, 15, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [HttpGet(Name = "GetEventTypeList")]
        public async Task<JsonResponse> GetEventTypeList()
        {
            try
            {
                return await _eventService.GetEventTypeList();
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "SaveAttendEvent")]
        public async Task<JsonResponse> SaveAttendEvent(EventAttendee eventAttendee)
        {
            try
            {
                return await _eventService.SaveAttendEvent(eventAttendee);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "EventAttendeesRequestList")]
        public async Task<JsonResponse> EventAttendeesRequestList(int EventId)
        {
            try
            {
                return await _eventService.EventAttendeesRequestList(user_unique_id,EventId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "AttendeesApprovDeny")]
        public async Task<JsonResponse> AttendeesApprovDeny(EventAttendRequest eventAttendee)
        {
            try
            {
                return await _eventService.AttendeesApprovDeny(eventAttendee,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "AddUpdateEventMember")]
        public async Task<JsonResponse> AddUpdateEventMember(AddRemoveEventMember req)
        {
            try
            {
                return await _eventService.AddUpdateEventMember(req, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "GetEventById")]
        public async Task<JsonResponse> GetEventById(int Id)
        {
            try
            {
                int userId = user_unique_id > 0 ? user_unique_id : 0 ;
                var response = await _eventService.GetEventById(Id, userId);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }


        [HttpPost(Name = "SaveEventSpeaker")]
        public async Task<JsonResponse> SaveEventSpeaker(EventSpeakers eventdata)
        {
            try
            {
                var response = await _eventService.SaveEventSpeaker(eventdata);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "SaveEventComment")]
        public async Task<JsonResponse> SaveEventComment(EventComment req)
        {
            try
            {
                var response = await _eventService.SaveEventComment(req);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "DeleteEvent")]
        public async Task<JsonResponse> DeleteEvent(int id)
        {
            try
            {
                return await _eventService.DeleteEvent(id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }

           
        }
    }
}
