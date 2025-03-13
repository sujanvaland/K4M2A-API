using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface ISearchService
    {
        public Task<JsonResponse> SearchUser(string Name, int PageNo, int Record, int LoginId);
        public Task<JsonResponse> SearchUserProfile(string Name);
        Task<JsonResponse> MentionSearchUser(string Name, int PageNo, int Record);
        public Task<JsonResponse> GetMyGoogleContactList(int UserId);
        public Task<JsonResponse> RemoveContactList(int UserId);
        public Task<JsonResponse> GetSearchHashTag(string Name);
        public Task<JsonResponse> SearchOrganisationUser(string Name, int PageNo, int Record, int LoginId);
    }
}
