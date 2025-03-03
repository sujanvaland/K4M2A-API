using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class UploadPostResponse
    {
        public UserPost Post { get; set; }
        public List<Entities.File> Files { get; set; }
    }

    public class UploadSchedulePostResponse
    {
        public SchedulePost Post { get; set; }
        public List<Entities.File> Files { get; set; }
    }
}
