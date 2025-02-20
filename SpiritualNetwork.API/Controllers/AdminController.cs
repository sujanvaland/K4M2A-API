using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ApiBaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [AllowAnonymous]
        [HttpPost(Name = "AllUserList")]
        public async Task<JsonResponse> AllUserList(SearchReqByPage req)
        {
            try
            {
                var response = await _adminService.AllUserList(req.Name, req.PageNo, req.Records);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetAllStats")]
        public async Task<JsonResponse> GetAllStats()
        {
            try
            {
                var response = await _adminService.GetAllStats();
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "BanUnBanUser")]
        public async Task<JsonResponse> BanUnBanUser(IdReq req)
        {
            try
            {
                var response = await _adminService.BanUnBanUser(req.Id);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "GetAllReportByReportedId")]
        public async Task<JsonResponse> GetAllReportByReportedId(ReportDetailReq req)
        {
            try
            {
                var response = await _adminService.GetAllReportByReportedId(req);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost(Name = "ReportList")]
        public async Task<JsonResponse> ReportList(ReportReq req)
        {
            try
            {
                var response = await _adminService.ReportList(req);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "SaveNotificationTemplate")]
        public async Task<JsonResponse> SaveNotificationTemplate(NotiTemReq req)
        {
            try
            {
                var response = await _adminService.SaveNotificationTemplate(req);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

    }
}
