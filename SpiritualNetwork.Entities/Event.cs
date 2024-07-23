using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class Event : BaseEntity
    {
        public string? EventFormat { get; set; }
        public int? EventTypeId { get; set; }
        public string? EventTitle { get; set; }
        public string? Access { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string Description { get; set; }
        public string EventLink { get; set; }
        public string Eventaddress { get; set; }
        public string EventCoverImage { get; set; }
        public string TimeFrame { get; set;}
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public bool IsCommenting { get; set; }
        public bool IsApprove { get; set; }
    }

    public class EventType : BaseEntity
    {
        public string EventTypeName { get; set; }
    }


    public class EventSpeakers : BaseEntity {

        public int EventId { get; set; }
        public int UserId { get; set; }
        public string? Type { get; set; }
    }

    public class EventAttendee : BaseEntity 
    {
        public int EventId { get; set; }

        public int UserId { get; set; }

        public bool Isjoin { get; set; }

        public bool IsInvited { get; set; }

    }

    public class EventComment : BaseEntity
    { 
        public int EventId { get; set;}
        public int UserId { get; set; }
        public string Comment { get; set;}

    }


}
