using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface IHastTagService
    {
        public Task<JsonResponse> ExtractPostHashTag(int postId);
        public Task<JsonResponse> GetTrendingHashTag();
        public Task<JsonResponse> GetSearchHashTag(string searchTerm);
    }
}
