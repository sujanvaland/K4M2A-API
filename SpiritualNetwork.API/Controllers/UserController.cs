using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Net;
using System.Net.Http;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ApiBaseController
    {
        private readonly ILogger<UserController> logger;
        private IUserService _userService;
        private IQuestion _question;
        private readonly RabbitMQService _rabbitMQService;

        public UserController(ILogger<UserController> logger, 
            IUserService userService, IQuestion question, RabbitMQService rabbitMQService)
        {
            this.logger = logger;
            this._userService = userService;
            _question = question;
            _rabbitMQService = rabbitMQService;
        }

        [AllowAnonymous]
        [HttpPost(Name = "CheckUsername")]
        public async Task<JsonResponse> CheckUsername(CheckUsernameRequest req)
        {
            try
            {
                var response = await _userService.CheckUsername(req.Username);

                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "PreSignUp")]
        public async Task<JsonResponse> PreSignUp(PreSignupRequest presignupRequest)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                presignupRequest.IpAddress = ip;
                var response = await _userService.PreSignUp(presignupRequest);
                
                var qresponse = await _question.InsertAnswerAsync(response.Id, presignupRequest.Answers);

                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "SignUp")]
        public async Task<JsonResponse> SignUp(SignupRequest signupRequest)
        {
            try
            {
                var response = await _userService.SignUpNew(signupRequest);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "SignIn")]
        public async Task<JsonResponse> SignIn(LoginRequest loginRequest)
        {
            try
            {
                if (!String.IsNullOrEmpty(loginRequest.Mobile))
                {
					return await _userService.SignIn(loginRequest.Mobile, loginRequest.Password,loginRequest.LoginMethod, 1);
				}
				return await _userService.SignIn(loginRequest.Username, loginRequest.Password, loginRequest.LoginMethod, 0);
			}
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "GetUserByName")]
        public async Task<JsonResponse> GetUserByName(GetUserByNameReq getUserByNameReq)
        {
            try
            {
                var user = await _userService.GetUserByName(getUserByNameReq.UserName);
                return new JsonResponse(200, true, "Success", user);
            }
            catch(Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "ForgotPassword")]
        public async Task<JsonResponse> ForgotPassword(string Email)
        {
            try
            {
                return await _userService.ForgotPasswordRequest(Email);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "ValidateOTP")]
        public async Task<JsonResponse> ValidateOTP(ValidateOTP req)
        {
            try
            {
                return await _userService.ValidateOTP(req.EncryptedOtp, req.EncryptedUserId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "VerifyEmail")]
        public async Task<JsonResponse> VerifyEmail(ValidateOTP req)
        {
            try
            {
                return await _userService.VerifyEmail(req.EncryptedOtp, req.EncryptedUserId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "DownloadImage")]
        public async Task<IActionResult> DownloadImage(string url)
        {
            try
            {
                var image = System.IO.File.OpenRead(url);
                return File(image, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error downloading image: {ex.Message}");
            }
        }

        [HttpGet(Name = "FollowUnFollowUser")]
        public async Task<JsonResponse> FollowUnFollowUser(int userId)
        {
            try
            {
                await _userService.FollowUnFollowUser(userId, user_unique_id);
                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "BlockMuteUser")]
        public JsonResponse BlockMuteUser(int userId,string type)
        {
            try
            {
                _userService.BlockMuteUser(userId, user_unique_id,type);
                return new JsonResponse(200, true, "Success", "");
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "OnlineOfflineUsers")]
        public async Task<JsonResponse> OnlineOfflineUsers(ConnectionIdReq Req)
        {
            try
            {
                return await _userService.OnlineOfflineUsers(user_unique_id,Req.ConnectionId, Req.Type);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

		[HttpPost(Name = "SaveRemoveDeviceToken")]
		public async Task<JsonResponse> SaveRemoveDeviceToken(DeviceTokenReq Req)
		{
			try
			{
				return await _userService.SaveRemoveDeviceToken(user_unique_id, Req.Token, Req.Type);
			}
			catch (Exception ex)
			{
				return new JsonResponse(200, false, "Fail", ex.Message);
			}
		}
		
		[HttpGet(Name = "GetOnlineUsers")]
        public async Task<JsonResponse> GetOnlineUsers()
        {
            try
            {
                return await _userService.GetOnlineUsers(user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        //[HttpGet(Name = "FollowUnFollowUser")]
        //public JsonResponse GetWhoToFollow(int userId)
        //{
        //    try
        //    {
        //        _userService.FollowUnFollowUser(userId, user_unique_id);
        //        return new JsonResponse(200, true, "Success", null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResponse(200, false, "Fail", ex.Message);
        //    }
        //}
        [HttpPost(Name = "StoreUserConnections")]
        public async Task<JsonResponse> StoreUserConnections(UserNetworkReq req)
        {
            try
            {
                return await _userService.StoreUserNetwork(req, user_unique_id);
            }
            catch (Exception ex) 
            {
                return new JsonResponse(200,true,"Fail",ex);
            }
        }

        [HttpPost(Name = "SendInvitation")]
        public async Task<JsonResponse> SendInvitation(InvitationReq req)
        {
            try
            {
                var Flag = true;
                if(req.Email.Length > 0)
                {
                    Flag = await _userService.SendInvitationMail(req.Email, user_unique_id, req.Id);
                }
                if (req.PhoneNumber.Length > 0)
                {
                    Flag = await _userService.SendSmsMail(req.PhoneNumber, user_unique_id, req.Id);
                }

                return new JsonResponse(200, true, "Success", Flag);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, true, "Fail", ex);
            }
        }

        [HttpPost(Name = "UpdateLocation")]
        public async Task<JsonResponse> UpdateLocation(UpdateLocationReq req)
        {
            try
            {
                return await _userService.UpdateLocation(req.Latitude, req.Longitude, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, true, "Fail", ex);
            }
        }

        [HttpPost(Name = "UpdateUserAttribute")]
        public async Task<JsonResponse> UpdateUserAttribute(UserAttributeRequest req)
        {
            try
            {
                req.UserId = user_unique_id;
                return await _userService.UpdateUserAttribute(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, true, "Fail", ex);
            }
        }

        [HttpGet(Name = "GetUserAttribute")]
        public async Task<JsonResponse> GetUserAttribute()
        {
            try
            {
                return await _userService.GetUserAttribute(user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, true, "Fail", ex);
            }
        }

        [HttpGet(Name = "BlockedUsersList")]
        public async Task<JsonResponse> BlockedUsersList()
        {
            try
            {
                return await _userService.GetBlockUserList(user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, true, "Fail", ex);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "Invitation")]
        public async Task<JsonResponse> Invitation(string name)
        {
            try
            {
                return await _userService.Invitation(name);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, true, "Fail", ex);
            }
        }

        [HttpPost(Name = "ChangePassword")]
        public async Task<JsonResponse> ChangePassword(ChangePasswordReq req)
        {
            try
            {
                var response = await _userService.ChangePassword(req, user_unique_id);

                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        // public Task<JsonResponse> EmailVerificationRequest(EmailVerificationReq req);
        [AllowAnonymous]
		[HttpPost(Name = "EmailVerification")]
		public async Task<JsonResponse> EmailVerification(EmailVerificationReq req)
		{
			try
			{
				var response = await _userService.EmailVerificationReq(req);
				return response;
			}
			catch (Exception ex)
			{
				return new JsonResponse(200, false, "Fail", ex.Message);
			}
		}
		[AllowAnonymous]
		[HttpPost(Name = "VerifiedEmailReq")]
		public async Task<JsonResponse> VerifiedEmailReq(VerifiedEmail req)
		{
			try
			{
				var response = await _userService.VerifiedEmailReq(req);
				return response;
			}
			catch (Exception ex)
			{
				return new JsonResponse(200, false, "Fail", ex.Message);
			}
		}

        [AllowAnonymous]
        [HttpPost(Name = "PhoneVerification")]
        public async Task<JsonResponse> PhoneVerification(PhoneVerificationReq req)
        {
            try
            {
                var response = await _userService.PhoneVerificationReq(req);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost(Name = "VerifiedPhoneReq")]
        public async Task<JsonResponse> VerifiedPhoneReq(VerifiedPhone req)
        {
            try
            {
                var response = await _userService.VerifiedPhoneReq(req);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
		[HttpGet(Name = "GetTagsList")]
		public async Task<JsonResponse> GetTagsList()
		{
			try
			{
				var response = await _userService.getTagsList();
				return response;
			}
			catch (Exception ex)
			{
				return new JsonResponse(200, false, "Fail", ex.Message);
			}
		}
	}
}
