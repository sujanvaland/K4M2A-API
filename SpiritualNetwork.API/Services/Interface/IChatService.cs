using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IChatService
    {
        Task<JsonResponse> GetChatMessages(int LoginUserId, ChatHistoryReq req, int Size);
        Task SaveChatMessage(ChatMessages chatMessages);
        void DeleteChatMessage(ChatMessages chatMessages);
        public Task<JsonResponse> DeleteChatHistory(int UserId, int LoginUserId, int MessageId);
        public Task<JsonResponse> GetUserChatList(int UserId);
        public Task<JsonResponse> LeaveGroupConversation(int GroupId, int UserId);
        public Task<JsonResponse> AddCreateMemberGroup(GroupMessageModel res, int UserId);
        public Task<JsonResponse> UpdateGroupDetails(UpdateGroupDetails req, int id);
        public Task<JsonResponse> GetInfolist(int GroupId, int loginUserId, bool IsGroup);
        public Task<JsonResponse> SnoozeGroupUser(int Id, int loginUserId, bool IsGroup, string type);
        public Task<JsonResponse> MakeAdminRemoveFromGroup(int GroupId, int userId, int LoginUserId, string Type);

    }
}
