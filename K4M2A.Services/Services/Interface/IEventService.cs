using K4M2A.Entities.Model;
using K4M2A.Entities;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface IEventService
    {
        Task<Event> SaveEvent(EventReq req);
        Task<JsonResponse> GetEventList(EventListReq req, int Size, int userId);
        
        Task<JsonResponse> GetMyEventList(EventListReq req, int Size, int userId);

        Task<JsonResponse> GetEventTypeList();

        Task<JsonResponse> SaveAttendEvent(EventAttendee Attendee);

        Task<EventModel> GetEventById(int id, int UserId);

        Task<EventSpeakers> SaveEventSpeaker(EventSpeakers eventspeaker);

        Task<JsonResponse> DeleteEvent(int id);

        Task<EventComment> SaveEventComment(EventComment req);
        Task<JsonResponse> EventAttendeesRequestList(int UserId, int EventId);
        Task<JsonResponse> AttendeesApprovDeny(EventAttendRequest req, int UserId);
        Task<JsonResponse> AddUpdateEventMember(AddRemoveEventMember req, int UserId);
    }
}
