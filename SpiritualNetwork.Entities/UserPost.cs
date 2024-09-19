using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class UserPost : BaseEntity
    {
        public int UserId { get; set; }
        [MaxLength(4000)]
        public string PostMessage { get; set; }
        [MaxLength(10)]
        public string Type { get; set; }
        public int? ParentId { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set;}
        public bool? IsVideo { get; set; }
    }

    public class PostFiles : BaseEntity
    {
        public int PostId { get; set; }
        public int FileId { get; set; }
    }

    public class File : BaseEntity
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string ContentType { get; set; }
        public string? ActualUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
