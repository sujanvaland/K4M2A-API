using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class EventListModel
    {
        public int ListCount { get; set; }
        public int Size { get; set; }
        public List<EventModel>? EventList { get; set; }
    }
    public class EventModel: BaseEntity
    {
        public EventModel() {
            EventAttendees = new List<EventAttend>();
            EventSpeakers = new List<EventAttend>();
            EventComments = new List<EventCommenter>();
            EventHosts = new List<EventAttend>();

        }
        public string? EventFormat { get; set; }

        public int? EventTypeId { get; set; }
        public string? EventTitle { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? StartTime { get; set; }

        public string? EndTime { get; set; }

        public string Description { get; set; }

        public string EventLink { get; set; }

        public string Eventaddress { get; set; }

        public string EventCoverImage { get; set; }

        public string EventType { get; set; }
        public string? Hostedby { get; set; }
        public string? TimeZone { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Access { get; set; }
        public bool IsJoin { get; set; }
        public bool IsPending { get; set; }
        public bool IsCommenting { get; set; }
        public bool IsApprove { get; set; }
        public List<EventAttend>? EventAttendees { get; set; }
        public List<EventAttend>? EventSpeakers { get; set; }
        public List<EventAttend>? EventHosts { get; set; }
        public List<EventCommenter>? EventComments { get; set; }
        public List<int>? InvitedUsers { get; set; }
        public int? AttendeesReq { get; set; }
    }

    public class EventCommenter
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string? ProfileImgUrl { get; set; }
        public string Comment  { get; set; }
        public bool? IsBusinessAccount { get; set; }

    }

    public class EventAttend
    {
        public int EventId { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? ProfileImgUrl { get; set; }
        public bool? IsFollowing { get; set; }
        public bool IsDisplay { get; set; }
        public bool? IsBusinessAccount { get; set; }


    }

    public class EventReq
    {
        public Event Event { get; set; }
        public List<int> SpeakersId { get; set; }
        public List<int> HostsId { get; set; }
    }

    public class EventAttendRequest
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string? Action { get; set; }
    }


    public class EventListReq
    {
        public int PageNo { get; set; }
        public int EventTypeId { get; set; }
        public int Period { get; set; }
        public string? EventFormat { get; set; }
        public string? Search { get; set; } = string.Empty;
    }

    public class AddRemoveEventMember
    {
        public string? Action { get; set; }
        public List<int>? UserId { get; set; }
        public int EventId { get; set; }
    }

}
