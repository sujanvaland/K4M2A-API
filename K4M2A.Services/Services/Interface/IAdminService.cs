using K4M2A.Entities.Model;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface IAdminService
    {
        public Task<JsonResponse> AllUserList(string Name, int PageNo, int Record);
        public Task<JsonResponse> GetAllStats();
        public Task<JsonResponse> BanUnBanUser(int Id);
        public Task<JsonResponse> GetAllReportByReportedId(ReportDetailReq req);
        public Task<JsonResponse> ReportList(ReportReq req);
        public Task<JsonResponse> SaveNotificationTemplate(NotiTemReq req);
    }
}
