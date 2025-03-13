using K4M2A.Entities.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using K4M2A.Entities.CommonModel;
using K4M2A.Services.Interface;
using K4M2A.Services;

namespace K4M2A.AdminApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ApiBaseController
    {
        private IUserService _userService;
       
        public UserController(ILogger<UserController> logger,
            IUserService userService)
        {
            this._userService = userService;
        }


        [AllowAnonymous]
        [HttpPost(Name = "SignIn")]
        public async Task<JsonResponse> SignIn(LoginRequest loginRequest)
        {
            try
            {
                if (String.IsNullOrEmpty(loginRequest.Username)
                    || String.IsNullOrEmpty(loginRequest.Password))
                {
                    return new JsonResponse(200, false, "Invalid Credential", null);
                }

                return await _userService.SignIn(loginRequest.Username, loginRequest.Password,"",0);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetAllUsers")]
        public async Task<JsonResponse> GetAllUsers(SearchReqByPage req)
        {
            try
            {
                var response = await _userService.GetAllUsers(req.Name, req.PageNo, req.Records);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
