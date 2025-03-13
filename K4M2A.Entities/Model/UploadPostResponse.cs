using K4M2A.Entities;

namespace K4M2A.Entities.Model
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
