using SpiritualNetwork.API.Model;
using SpiritualNetwork.Common;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IUserService
    {
        Task<JsonResponse> Invitation(string name);
        public Task<JsonResponse> SignUp(SignupRequest signupRequest);
        public Task<JsonResponse> SignIn(string username, string password);
        public Task<JsonResponse> ForgotPasswordRequest(string email);
        public Task<JsonResponse> ValidateOTP(string encryptedotp, string encrypteduserid);
        public Task<PreRegisteredUser> PreSignUp(PreSignupRequest req);
        public Task<JsonResponse> CheckUsername(string username);
        Task<JsonResponse> VerifyEmail(string encryptedotp, string encrypteduserid);
        void BlockMuteUser(int userId, int loginUserId, string type);
        public Task<User> GetUserByName(string Username);
        public Task<JsonResponse> OnlineOfflineUsers(int UserId, string? ConnectionId, string Type);
        public Task<JsonResponse> GetOnlineUsers(int Id);
        public Task<JsonResponse> StoreUserNetwork(UserNetworkReq req, int inviterId);
        public Task<bool> SendInvitationMail(string Emailreq, int UserId, int Id);
        public Task<bool> SendSmsMail(string Phonenumber, int UserId, int Id);
        public Task<JsonResponse> UpdateLocation(string Latitude, string Longitude, int UserId);
        public Task<JsonResponse> UpdateUserAttribute(UserAttributeRequest req);
        public Task<JsonResponse> GetUserAttribute(int UserId);
        public Task<JsonResponse> GetBlockUserList(int UserId);
        public Task<JsonResponse> ChangePassword(ChangePasswordReq req, int UserId);
        public Task<JsonResponse> EmailVerificationReq(EmailVerificationReq req);
        public Task<JsonResponse> VerifiedEmailReq(VerifiedEmail req);
        public Task<JsonResponse> getTagsList();
        public Task<JsonResponse> SaveRemoveDeviceToken(int UserId, string? Token, string Type);
        public Task FollowUnFollowUser(int userId, int loginUserId);
        public Task<JsonResponse> PhoneVerificationReq(PhoneVerificationReq req);
        public Task<JsonResponse> VerifiedPhoneReq(VerifiedPhone req);

    }
}
