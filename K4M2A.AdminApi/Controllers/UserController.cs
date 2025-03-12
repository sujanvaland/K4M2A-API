using K4M2A.AdminApi.Model;
using K4M2A.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.Entities.CommonModel;

namespace K4M2A.AdminApi.Controllers
{
    public class UserController : Controller
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

                return await _userService.SignIn(loginRequest.Username, loginRequest.Password, 0);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
