using Microsoft.AspNetCore.Authorization;
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
    public class ProfileController : ApiBaseController
    {
        private readonly IProfileService _profileService;
        private readonly INotificationService _notificationService;

        public ProfileController(IProfileService profileService, INotificationService notificationService)
        {
            _profileService = profileService;
            _notificationService = notificationService;
        }

        [HttpPost(Name = "UpdateProfile")]
        public async Task<JsonResponse> UpdateProfile(ProfileReqest profileReq)
        {
            try
            {
                var response = await _profileService.UpdateProfile(profileReq,user_unique_id);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetProfile")]
        public async Task<JsonResponse> GetProfile(string username)
        {
            try
            {
                var profile = await _profileService.GetUserInfoBox(username,user_unique_id);
                return new JsonResponse(200, true, "Success", profile);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetUserInfoBox")]
        public async Task<JsonResponse> GetUserInfoBox(string username)
        {
            try
            {
                var profile = await _profileService.GetUserInfoBox(username,user_unique_id);
                return new JsonResponse(200, true, "Success", profile);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetBooksSuggestion")]
        public async Task<JsonResponse> GetBooksSuggestion(int userId)
        {
            try
            {
                return await _profileService.GetBooksSuggestion(userId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetMoviesSuggestion")]
        public async Task<JsonResponse> GetMoviesSuggestion(int userId)
        {
            try
            {
                return await _profileService.GetMoviesSuggestion(userId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetGurusSuggestion")]
        public async Task<JsonResponse> GetGurusSuggestion(int userId)
        {
            try
            {
                return await _profileService.GetGurusSuggestion(userId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetPracticesSuggestion")]
        public async Task<JsonResponse> GetPracticesSuggestion(int userId)
        {
            try
            {
                return await _profileService.GetPracticeSuggestion(userId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [AllowAnonymous]

        [HttpGet(Name = "GetBook")]
        public async Task<JsonResponse> GetBook(string search)
        {
           
            try
            {
                return await _profileService.GetBooksAsync(search);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetExperienceSuggestion")]
        public async Task<JsonResponse> GetExperienceSuggestion(int userId)
        {
            try
            {
                return await _profileService.GetExperienceSuggestion(userId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "SearchProfileSuggestion")]
        public async Task<JsonResponse> SearchProfileSuggestion(SearchProfileSuggestion req)
        {
            try
            {
                return await _profileService.SearchSuggestion(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "BookMarkSuggestion")]
        public async Task<JsonResponse> BookMarkSuggestion(BookMarkRes req)
        {
            try
            {
                return await _profileService.AddSuggestion(req,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "UpdateProfileSuggestion")]
        public async Task<JsonResponse> UpdateProfileSuggestion(DeleteUpdateProfileSuggestion req)
        {
            try
            {
                return await _profileService.UpdateIsReadSuggestion(req.Id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "DeleteProfileSuggestion")]
        public async Task<JsonResponse> DeleteProfileSuggestion(DeleteUpdateProfileSuggestion req)
        {
            try
            {
                return await _profileService.DeleteProfileSuggestion(req.Id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        
        [HttpGet(Name = "GetFollowers")]
        public async Task<JsonResponse> GetFollowers(int UserId)
        {
            try
            {
                var result = await _profileService.GetFollowers(UserId);
                return new JsonResponse(200, true, "Success", result);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetWhoToFollow")]
        public async Task<JsonResponse> GetWhoToFollow()
        {
            try
            {
                return await _profileService.GetWhoToFollow(user_unique_id,1);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetConnectionsMentions")]
        public async Task<JsonResponse> GetConnectionsMentions(int UserId)
        {
            try
            {
                var result = await _profileService.GetConnectionsMentions(UserId);
                return new JsonResponse(200, true, "Success", result);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        
        [HttpPost(Name = "UserNotification")]
        public async Task<JsonResponse> UserNotification(NotificationReq req)
        {
            try
            {
                var result = await _notificationService.UserNotification(user_unique_id,req.PageNo,req.Size);
                return new JsonResponse(200, true, "Success", result);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetAllNotificationCount")]
        public async Task<JsonResponse> GetAllNotificationCount()
        {
            try
            {
                return await _notificationService.GetAllNotificationCount(user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
