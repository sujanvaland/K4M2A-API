using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatController : ApiBaseController
    {
        private readonly IChatService _chatService;
        private readonly IProfileService _profileService;

        public ChatController(IChatService chatService, IProfileService profileService)
        {
            _chatService = chatService;
            _profileService = profileService;
        }

        [HttpPost(Name = "SaveChat")]
        public async Task<JsonResponse> SaveChat(ChatMessages chatMessages)
        {
            try
            {
                chatMessages.SenderId = user_unique_id;
                await _chatService.SaveChatMessage(chatMessages);
                return new JsonResponse(200,true,"Success",null);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetChatHistory")]
        public async Task<JsonResponse> GetChatHistory(ChatHistoryReq req)
        {
            try
            {
                return await _chatService.GetChatMessages(user_unique_id,req,5);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetUserChatProfile")]
        public async Task<JsonResponse> GetUserChatProfile()
        {
            try
            {
                return await _chatService.GetUserChatList(user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "CreateGroup")]
        public async Task<JsonResponse> CreateGroup(GroupMessageModel req)
        {
            try
            {
               return await _chatService.AddCreateMemberGroup(req,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "UpdateGroupDetails")]
        public async Task<JsonResponse> UpdateGroupDetails(UpdateGroupDetails req )
        {
            try
            {
                return await _chatService.UpdateGroupDetails(req,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "LeaveGroupConversation")]
        public async Task<JsonResponse> LeaveGroupConversation(leaveGroupModel req)
        {
            try
            {
                return await _chatService.LeaveGroupConversation(req.GroupId,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetInfolist")]
        public async Task<JsonResponse> GetInfolist(InfoBoxModel req)
        {
            try
            {
                return await _chatService.GetInfolist(req.Id, user_unique_id, req.IsGroup);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [HttpPost(Name = "SnoozeGroupUser")]
        public async Task<JsonResponse> SnoozeGroupUser(SnoozeUserGroupReq req)
        {
            try
            {
                return await _chatService.SnoozeGroupUser(req.Id, user_unique_id, req.IsGroup, req.Type);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "MakeAdminRemoveFromGroup")]
        public async Task<JsonResponse> MakeAdminRemoveFromGroup(ActionUserFGroup req)
        {
            try
            {
                return await _chatService.MakeAdminRemoveFromGroup(req.GroupId, req.UserId,user_unique_id,req.Type);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "DeleteChatMessage")]
        public async Task<JsonResponse> DeleteChatMessage(DeleteMessage req)
        {
            try
            {
                return await _chatService.DeleteChatHistory(req.Id,user_unique_id,req.MessageId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
