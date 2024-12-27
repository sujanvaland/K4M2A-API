using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.API.Hubs;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IRepository<PostComment> _prostComment;
        private readonly IRepository<Reaction> _reaction;
        private readonly AppDbContext _appDbContext;
        private readonly IPostService _postService;
        private readonly IRepository<UserPost> _userPostRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<OnlineUsers> _onlineUsers;
        private readonly INotificationService _notificationService;
        private readonly AppDbContext _context;
        private readonly IRepository<UserFollowers> _userFollowers;

        public ReactionService(IRepository<PostComment> postComment, 
            AppDbContext appDbContext,
            IRepository<Reaction> reaction,
            IPostService postService,
            IRepository<UserPost> userPostRepository,
            IRepository<User> userRepository,
            IRepository<OnlineUsers> onlineUsers,
            INotificationService notificationService,
            AppDbContext context,
            IRepository<UserFollowers> userFollowers) 
        {
            _onlineUsers = onlineUsers;
            _userRepository = userRepository;
            _userPostRepository = userPostRepository;
            _postService = postService;
            _reaction = reaction;
            _prostComment = postComment;
            _appDbContext = appDbContext;
            _notificationService = notificationService;
            _context = context;
            _userFollowers = userFollowers;
        }

        public async Task<JsonResponse> GetAllComments(int PostId)
        {
            try
            {

                SqlParameter postparam = new SqlParameter("@postid", PostId);

                var Result = await _appDbContext.PostResponses
                    .FromSqlRaw("GetComments @postid", postparam)
                    .ToListAsync();

                return new JsonResponse(200, true, "Success", Result);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetAllBookmarksByUserId(int userid)
        {
            try
            {
                SqlParameter userparam = new SqlParameter("@UserId", userid);
                var Result = await _context.PostResponses
                    .FromSqlRaw("GetBookmarkTimeLine @UserId", userparam)
                    .ToListAsync();
                return new JsonResponse(200, true, "Success", Result);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> ToggleBookmark(int postid, int userid)
        {
            try
            {
                var data = await _reaction.Table
                    .Where(x => x.IsDeleted == false &&
                    x.UserId == userid &&
                    x.PostId == postid)
                    .FirstOrDefaultAsync();

                NodeAddPost NodePostId = new NodeAddPost();

                if (data != null)
                {
                    _reaction.DeleteHard(data);

                    NodePostId.Id = postid;
                    await _notificationService.SendPostToNode(NodePostId);

                    return new JsonResponse(200, true, "Success", data);
                }

                Reaction reaction = new Reaction();
                reaction.PostId = postid;
                reaction.UserId = userid;
                reaction.Type = "bookmark";

                await _reaction.InsertAsync(reaction);

                NodePostId.Id = postid;
                await _notificationService.SendPostToNode(NodePostId);

                return new JsonResponse(200,true,"Success",reaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> ToggleLike(int PostId, int UserId)
        {
            try
            {
                var data = await _userPostRepository.Table
               .Where(x => x.Id == PostId)
               .FirstOrDefaultAsync();

                var postReaction = await _reaction.Table
                    .Where(x => x.IsDeleted == false && x.PostId == PostId)
                    .ToListAsync();

                var like = postReaction.Where(x => x.Type == "like" && x.UserId == UserId).FirstOrDefault();

                if(like != null)
                {
                    _reaction.DeleteHard(like);
                    await _postService.UpdateCount(PostId,"like",0);

                    NodeAddPost NodePostId = new NodeAddPost();
                    NodePostId.Id = PostId;
                    await _notificationService.SendPostToNode(NodePostId);
                }

                if (like == null)
                {
                    Reaction reaction = new Reaction();
                    reaction.PostId = PostId;
                    reaction.UserId = UserId;
                    reaction.Type = "like";
                    await _reaction.InsertAsync(reaction);
                    await _postService.UpdateCount(PostId, "like", 1);
                    like = reaction;

                    NotificationRes notification = new NotificationRes();
                    notification.PostId = PostId;
                    notification.ActionByUserId = UserId;
                    notification.ActionType = "like";
                    notification.RefId1 = data.UserId.ToString();
                    notification.RefId2 = "";
                    notification.Message = "";
                    notification.PushAttribute = "pushlikepost";
					notification.EmailAttribute = "emaillikepost";
					await _notificationService.SaveNotification(notification);

					return new JsonResponse(200, true, "Success", notification);
				}

				//var childPosts = await  _userPostRepository.Table.Where(x=>x.ParentId == PostId).ToListAsync();
				//foreach (var childPost in childPosts)
				//{
				//    var childpostReaction = await _reaction.Table
				//    .Where(x => x.IsDeleted == false && x.PostId == childPost.Id)
				//    .ToListAsync();

				//    var childlike = childpostReaction.Where(x => x.Type == "like" && x.UserId == UserId).FirstOrDefault();

				//    if (childlike != null)
				//    {
				//        _reaction.DeleteHard(childlike);
				//        await _postService.UpdateCount(childPost.Id, "like", 0);
				//    }

				//    if (childlike == null)
				//    {
				//        Reaction reaction = new Reaction();
				//        reaction.PostId = childPost.Id;
				//        reaction.UserId = UserId;
				//        reaction.Type = "like";
				//        await _reaction.InsertAsync(reaction);
				//        await _postService.UpdateCount(childPost.Id, "like", 1);
				//        like = reaction;

				//        NotificationRes notification = new NotificationRes();
				//        notification.PostId = childPost.Id;
				//        notification.ActionByUserId = UserId;
				//        notification.ActionType = "like";
				//        notification.RefId1 = data.UserId.ToString();
				//        notification.RefId2 = "";
				//        notification.Message = "";
				//        await _notificationService.SaveNotification(notification);

				//    }
				//}
				return new JsonResponse(200,true,"Success",null);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public class NotificationModal
        {
            public UserPost post { get; set; }
            public string message { get; set; }
            public string type { get; set; }
            public List<string> ConnectionIds { get; set; }
        }

        public async Task<JsonResponse> GetAllReaction(int PostId)
        {
            try
            {
                SqlParameter postparam = new SqlParameter("@PostId", PostId);
                var Result = await _appDbContext.Reactions
                    .FromSqlRaw("GetReaction @PostId", postparam)
                    .ToListAsync();

                return new JsonResponse(200, true, "Success", Result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> InsertCommentAsync(CommentInsertModel commentInsertModel, string username)
        {
            try
            {
                UserPost userPost = new UserPost();
                userPost.Type = commentInsertModel.Category;
                userPost.PostMessage = "";
                userPost.UserId = commentInsertModel.UserId;
                userPost.ParentId = commentInsertModel.ParentId;

                await _userPostRepository.InsertAsync(userPost);

                var data = await _userRepository.GetByIdAsync(userPost.Id);

                await _postService.UpdateCount((int)commentInsertModel.ParentId, "comment",1);

                return new JsonResponse(200,true,"Success",userPost);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            throw new NotImplementedException();
        }

        public async Task<JsonResponse> GetAllLikeList(int PostId,int LoginId)
        {
            try
            {
                var LikeList = await (from ur in _reaction.Table
                                        join u in _userRepository.Table on ur.UserId equals u.Id
                                        join uf in _userFollowers.Table.Where(x => x.UserId == LoginId) on u.Id equals uf.FollowToUserId into ufGroup
                                        from uf in ufGroup.DefaultIfEmpty()
                                        where ur.PostId == PostId && ur.IsDeleted == false && u.IsDeleted == false
                                        select new LikeList
                                        {
                                            UserId = u.Id,
                                            UserName = u.UserName,
                                            Name = u.FirstName + " " + u.LastName,
                                            ProfileImg = u.ProfileImg,
                                            IsFollowing = uf != null,
                                            IsDisplay = !(uf != null),
                                            IsBusinessAccount = u.IsBusinessAccount,
                                        }
                                    ).ToListAsync();

                LikeListResponse likeListResponse = new LikeListResponse();
                likeListResponse.PostId = PostId;
                likeListResponse.LikeCount = LikeList.Count();
                likeListResponse.UserList = LikeList;
                return new JsonResponse(200, true, "Success", likeListResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
