using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface ISearchService
    {
        public Task<JsonResponse> SearchUser(string Name, int PageNo, int Record);
        public Task<JsonResponse> SearchUserProfile(string Name);
        Task<JsonResponse> MentionSearchUser(string Name, int PageNo, int Record);
        public Task<JsonResponse> GetMyGoogleContactList(int UserId);
        public Task<JsonResponse> RemoveContactList(int UserId);


    }
}
