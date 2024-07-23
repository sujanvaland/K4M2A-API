using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class Community : BaseEntity
    {
        public string Name { get; set; }
        public string? Purpose { get; set; }
        public int ParentId { get; set; }
        public string? Type{ get; set; }
        public string? ProfileImgUrl { get; set; }
        public string? BackgroundImgUrl{ get; set; }
        public string? Question { get; set; }

    }

    public class CommunityMember : BaseEntity
    {
        public int UserId { get; set; }
        public int CommunityId { get; set; }
        public bool IsModerator { get; set; }
        public bool Isjoin { get; set; }
        public string? Answer { get; set; }
        public bool IsInvited { get; set; }
    }

    public class CommunityRules : BaseEntity
    {
        [MaxLength(200)]
        public string Rule { get; set; }
        [MaxLength(200)]
        public string? Descriptions { get; set; }
        public int CommunityId { get; set; }

    }

    public class CommunityReportPost : BaseEntity
    {
        public int CommunityId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string ReportDetail { get; set; }
        public int PostId { get; set; }
    }

}
