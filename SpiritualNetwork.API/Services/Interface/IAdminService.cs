using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IAdminService
    {
        public Task<JsonResponse> AllUserList(string Name, int PageNo, int Record);
        public Task<JsonResponse> GetAllStats();
        public Task<JsonResponse> BanUnBanUser(int Id);
        public Task<JsonResponse> GetAllReportByReportedId(ReportDetailReq req);
        public Task<JsonResponse> ReportList(ReportReq req);
    }
}
