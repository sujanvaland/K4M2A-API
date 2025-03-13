using K4M2A.Entities.Model;
using K4M2A.Entities;
using K4M2A.Entities.CommonModel;

namespace K4M2A.Services.Interface
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
