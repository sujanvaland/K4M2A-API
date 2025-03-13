using K4M2A.Entities.Model;
using K4M2A.Entities;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface INotificationService
    {
        public Task SendEmailNotification(string emailType, User user);
        public Task<JsonResponse> SaveNotification(NotificationRes Res);
        public Task<JsonResponse> UserNotification(int userId, int PageNo, int Size);
        public Task<JsonResponse> GetAllNotificationCount(int User);
        public Task SendNotification(Notification request);
        public Task SendPostToNode(NodeAddPost request);
        public Task<JsonResponse> DeleteUndoNotification(string type, int userId, int Id);
        public Task<JsonResponse> ReadNotification(string type, int userId, int Id);

    }
}
