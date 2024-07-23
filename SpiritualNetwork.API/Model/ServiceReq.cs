using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class ServiceReq
    {
        public Service? service {get; set;}
        public List<ServiceFAQReq>? FAQReq { get; set; }
        public List<string>? ServiceImageUrl { get; set; }
    }

    public class ServiceFAQReq
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
    }

    public class ServiceIdReq
    {
        public int Id { get; set; }
    }

    public class ServiceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ProfileImg { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public List<ServiceFAQReq>? FAQReq { get; set; }
        public List<ImageServiceResponse>? ServiceImageUrl { get; set; }
    }

    public class ImageServiceResponse
    {
        public int Id{ get; set; }
        public string ImageUrl { get; set; }
    }

    public class GetServiceReq
    {
        public int? Id { get; set; }
        public string Category { get; set; }
    }
}
