using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Migrations;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class K4M2AServiceController : ApiBaseController
    {
        private readonly IK4M2AService _k4m2aService;

        public K4M2AServiceController(IK4M2AService k4m2aService)
        {
            _k4m2aService = k4m2aService;
        }

        [HttpPost(Name = "SaveServiceDetails")]
        public async Task<JsonResponse> SaveServiceDetails(ServiceReq req)
        {
            try
            {
                req.service.CreatedBy = user_unique_id;
                return await _k4m2aService.SaveServiceDetails(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [HttpPost(Name = "UpdateServiceFAQ")]
        public async Task<JsonResponse> UpdateServiceFAQ(ServiceFAQReq req)
        {
            try
            {
                return await _k4m2aService.UpdateServiceFAQ(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [HttpPost(Name = "DeleteServiceFAQ")]
        public async Task<JsonResponse> DeleteServiceFAQ(ServiceIdReq req)
        {
            try
            {
                return await _k4m2aService.DeleteServiceFAQ(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [HttpPost(Name = "DeleteService")]
        public async Task<JsonResponse> DeleteService(ServiceIdReq req)
        {
            try
            {
                return await _k4m2aService.DeleteService(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "GetAllService")]
        public async Task<JsonResponse> GetAllService(GetServiceReq req)
        {
            try
            {
                return await _k4m2aService.GetAllService(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetServiceById")]
        public async Task<JsonResponse> GetServiceById(ServiceIdReq req)
        {
            try
            {
                return await _k4m2aService.GetServiceById(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

    }
}
