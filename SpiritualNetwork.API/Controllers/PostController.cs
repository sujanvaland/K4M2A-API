using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;


namespace SpiritualNetwork.API.Controllers
{
   
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ApiBaseController
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost(Name = "PostUpload")]
        public async Task<JsonResponse> PostUpload(IFormCollection form)
        {
            try
            {
                var response = await _postService.InsertPost(form,user_unique_id, username);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "RePost")]
        public async Task<JsonResponse> RePost(ReactionReq req)
        {
            try
            {
                var response = await _postService.RePost(req.PostId, user_unique_id);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetAllPosts")]
        public async Task<JsonResponse> GetAllPosts(GetTimelineReq req)
        {
            try
            {
                return await _postService.GetAllPostsAsync(user_unique_id, req.PageNo, req.ProfileUserId,req.Type);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "UpdateViews")]
        public async Task<JsonResponse> UpdateViews(List<int> req)
        {
            try
            {
                return await _postService.UpdateViews(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetAllImgVideoLink")]
        public async Task<JsonResponse> GetAllImgVideoLink(GetTimelineReq req)
        {
            try
            {
                return await _postService.GetAllImgVideoLink(user_unique_id, req.PageNo, req.ProfileUserId, req.Type);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetPostById")]
        public async Task<JsonResponse> GetPostById(int postId)
        {
            try
            {
                return await _postService.GetPostById(user_unique_id,postId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        

        [HttpPost(Name = "DeletePost")]
        public async Task<JsonResponse> DeletePost(DeletePostReq req)
        {
            try
            {
                return await _postService.DeletePostAsync(req.Id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "PostMentionList")]
        public async Task<JsonResponse> PostMentionList(MentionListReq req)
        {
            try
            {
                return await _postService.PostMentionList(req,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "ExtractMetaTags")]
        public async Task<JsonResponse> ExtractMetaTags(ExtractUrlMetaReq req)
        {
            var metaTags = Common.StringHelper.ExtractMetaTags(req.Url);
            return new JsonResponse(200, true, "Success", metaTags);
        }

        [HttpPost(Name = "BlockUnBlockPost")]
        public async Task<JsonResponse> BlockUnBlockPost(BlockUnBlockReq req)
        {
            try
            {
                return await _postService.BlockUnBlockPosts(req.PostId,user_unique_id);
            }
            catch (Exception ex) 
            {
                return new JsonResponse(200,true,"Fail",ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "UpdatePost")]
        public JsonResponse UpdatePost()
        {
            try
            {
                _postService.UpdatePost();
                return new JsonResponse(200, true, "Success","");
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, true, "Success", "");
            }
        }

        [HttpGet(Name = "ChangeWhoCanReply")]
        public async Task<JsonResponse> ChangeWhoCanReply(int postId,int whoCanReply)
        {
            try
            {
                var post = await _postService.ChangeWhoCanReply(postId, whoCanReply);
                return new JsonResponse(200, true, "Success", post);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "PinUnpinPost")]
        public async Task<JsonResponse> PinUnpinPost(int postId)
        {
            try
            {
                var post = await _postService.PinUnpinPost(postId,user_unique_id);
                return new JsonResponse(200, true, "Success", post);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "RemoveLocationInfo")]
        public async Task<JsonResponse> RemoveLocationInfo()
        {
            try
            {
                await _postService.RemoveLocationInfo(user_unique_id);
                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost(Name = "ReportPost")]
        public async Task<JsonResponse> ReportPost(Report req)
        {
            try
            {
                await _postService.ReportPost(req, user_unique_id);
                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "DeleteAllVideoPost")]
        public async Task<JsonResponse> DeleteAllVideoPost()
        {
            try
            {
                await _postService.DeleteAllVideoPost(user_unique_id);
                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
