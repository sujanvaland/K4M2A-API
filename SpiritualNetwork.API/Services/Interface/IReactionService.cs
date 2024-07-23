using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IReactionService
    {
        public Task<JsonResponse> GetAllReaction(int PostId);
        public Task<JsonResponse> ToggleBookmark(int postid, int userid);
        public Task<JsonResponse> GetAllBookmarksByUserId(int userid);
        public Task<JsonResponse> GetAllComments(int PostId);
        public Task<JsonResponse> ToggleLike(int PostId, int UserId);
        public Task<JsonResponse> InsertCommentAsync(CommentInsertModel commentInsertModel, string username);
        public Task<JsonResponse> GetAllLikeList(int PostId,int LoginId);

    }
}
