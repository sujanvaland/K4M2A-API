using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;
using SpiritualNetwork.Entities;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using Azure;
using Twilio;
using Twilio.Rest.IpMessaging.V2.Service.Channel;
using Twilio.Rest.Chat.V1.Service.Channel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Text.RegularExpressions;
using System.Linq;
using Npgsql;
using SpiritualNetwork.API.AppContext;
using Microsoft.Extensions.DependencyInjection;

namespace SpiritualNetwork.API.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRepository<PasswordResetRequest> _passwordResetRequestRepository;
        private readonly IRepository<EmailVerificationRequest> _emailVerificationRequestRepository;
        private readonly IRepository<PhoneVerificationRequest> _phoneVerificationRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly IGlobalSettingService _globalSettingService;
        private readonly IRepository<PreRegisteredUser> _preRegisteredUserRepository;
        private readonly IRepository<UserFollowers> _userFollowersRepository;
        private readonly IRepository<UserMuteBlockList> _userMuteBlockListRepository;
        private readonly IQuestion _question;
        private readonly IRepository<OnlineUsers> _onlineUsers;
        private readonly IProfileService _profileService;
        private readonly IRepository<UserNetwork> _userNetworkRepository;
        private readonly IRepository<UserAttribute> _userAttributeRepository;
        private readonly IRepository<Invitation> _InvitationRepository;
		private readonly IRepository<Tags> _tagsRepository;
		private readonly IRepository<DeviceToken> _deviceTokenRepository;
        private readonly IRepository<InviteRequest> _inviteRequest;
        private readonly IRepository<ActivityLog> _activityRepository;
        private readonly AppDbContext _context;
        private readonly IRepository<UserNotification> _userNotificationRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public UserService(
            IRepository<OnlineUsers> onlineUsers,
            IRepository<PreRegisteredUser> preregistereduserrepository,
            IRepository<User> userRepository,
            IMapper mapper,
            IConfiguration configuration,
            IRepository<PasswordResetRequest> passwordResetRequestRepository,
            INotificationService notificationService, 
            IGlobalSettingService globalSettingService,
            IQuestion question,
            IRepository<UserFollowers> userFollowersRepository,
            IRepository<UserMuteBlockList> userMuteBlockListRepository,
            IProfileService profileService,
            IRepository<UserNetwork> userNetworkRepository,
            IRepository<UserAttribute> userAttributeRepository,
            IRepository<Invitation> invitationRepository,
			IRepository<EmailVerificationRequest> emailVerificationRequestRepository,
			IRepository<Tags> tagsRepository,
			IRepository<DeviceToken> deviceTokenRepository,
            IRepository<PhoneVerificationRequest> phoneVerificationRequest,
			IRepository<InviteRequest> inviteRequest,
            IRepository<ActivityLog> activityRepository,
            AppDbContext context,
            IRepository<UserNotification> userNotificationRepository,
            IRepository<Notification> notificationRepository,
            IServiceScopeFactory serviceScopeFactory)
        {
            _userNetworkRepository = userNetworkRepository;
            _onlineUsers = onlineUsers;
            _preRegisteredUserRepository = preregistereduserrepository;
            _passwordResetRequestRepository = passwordResetRequestRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _notificationService = notificationService;
            _globalSettingService = globalSettingService;
            _question = question;
            _userFollowersRepository = userFollowersRepository;
            _userMuteBlockListRepository = userMuteBlockListRepository;
            _profileService = profileService;
            _userAttributeRepository = userAttributeRepository;
            _InvitationRepository = invitationRepository;
            _emailVerificationRequestRepository = emailVerificationRequestRepository;
            _tagsRepository = tagsRepository;
            _deviceTokenRepository = deviceTokenRepository;
            _phoneVerificationRequestRepository = phoneVerificationRequest;
            _inviteRequest = inviteRequest;
            _activityRepository = activityRepository;
            _context = context;
            _userNotificationRepository = userNotificationRepository;
            _notificationRepository = notificationRepository;
            _serviceScopeFactory = serviceScopeFactory;

        }

        public async Task<JsonResponse> OnlineOfflineUsers(int UserId, string ConnectionId, string Type)
        {
            try
            {

               var data = await _onlineUsers.Table
                    .Where(x => x.IsDeleted == false && x.UserId == UserId)
                    .FirstOrDefaultAsync();

                if (Type == "save")
                {
                    if(data == null)
                    {
						OnlineUsers onlineUsers = new OnlineUsers();
						onlineUsers.UserId = UserId;
						onlineUsers.ConnectionId = ConnectionId;
						await _onlineUsers.InsertAsync(onlineUsers);
						return new JsonResponse(200, true, "Success", onlineUsers);
                    }
                    else
                    {
						data.ConnectionId = ConnectionId;
						await _onlineUsers.UpdateAsync(data);
						return new JsonResponse(200, true, "Success", data);
					}
				}
                else
                {
                    if(data != null)
                    {
						 _onlineUsers.DeleteHard(data);
					}
                }
				return new JsonResponse(200, true, "Success", null);


			}
			catch (Exception ex)
            {
                throw ex;
            }
        }

		//public async Task<JsonResponse> SaveRemoveDeviceToken(int UserId, string? Token, string Type )
		//{
		//	try
		//	{
		//		var check = await _deviceTokenRepository.Table.Where(x => x.UserId == UserId && x.Token == Token && x.IsDeleted == false).FirstOrDefaultAsync();

		//		if (Type == "save")
  //              {
  //                  if (check == null)
  //                  {
  //                      DeviceToken deviceToken = new DeviceToken();
  //                      deviceToken.UserId = UserId;
  //                      deviceToken.Token = Token;
  //                      await _deviceTokenRepository.InsertAsync(deviceToken);
  //                  }

  //                  if (check.Token == Token)
  //                  {
  //                      DeviceToken deviceToken = new DeviceToken();
  //                      deviceToken.UserId = UserId;
  //                      deviceToken.Token = Token;
  //                      await _deviceTokenRepository.InsertAsync(deviceToken);
  //                  }

  //              }
  //              else
  //              {
  //                  if (check != null) 
  //                  { 
  //                      await _deviceTokenRepository.DeleteAsync(check);
  //                  }

  //              }
		//		return new JsonResponse(200, true, "Success", null);
		//	}
		//	catch (Exception ex)
		//	{
		//		throw ex;
		//	}
		//}


        public async Task<JsonResponse> SaveRemoveDeviceToken(int userId, string? token, string type)
        {
            // Validate the token input
            if (string.IsNullOrWhiteSpace(token))
            {
                return new JsonResponse(400, false, "Invalid token.", null);
            }

            try
            {
                // Look up the token across all users (ignoring soft-deleted entries)
                var existingToken = await _deviceTokenRepository.Table
                    .Where(x => x.Token == token && !x.IsDeleted)
                    .FirstOrDefaultAsync();

                if (type.Equals("save", StringComparison.OrdinalIgnoreCase))
                {
                    if (existingToken != null)
                    {
                        // If the token exists but belongs to a different user, update it
                        if (existingToken.UserId != userId)
                        {
                            existingToken.UserId = userId;
                            // Optionally update other properties like ModifiedDate
                            await _deviceTokenRepository.UpdateAsync(existingToken);
                        }
                        // Else: token already exists for this user. No action needed.
                    }
                    else
                    {
                        // Insert a new record if the token does not exist
                        var newToken = new DeviceToken
                        {
                            UserId = userId,
                            Token = token,
                            IsDeleted = false,
                            // You can also set CreatedDate = DateTime.UtcNow, etc.
                        };
                        await _deviceTokenRepository.InsertAsync(newToken);
                    }
                }
                else if (type.Equals("remove", StringComparison.OrdinalIgnoreCase))
                {
                    // Only remove the token if it exists and belongs to this user
                    if (existingToken != null && existingToken.UserId == userId)
                    {
                        // For soft delete:
                        existingToken.IsDeleted = true;
                        await _deviceTokenRepository.UpdateAsync(existingToken);

                        // Alternatively, for a hard delete, you might call:
                        // await _deviceTokenRepository.DeleteAsync(existingToken);
                    }
                }
                else
                {
                    return new JsonResponse(400, false, "Invalid type provided.", null);
                }

                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                // Log the exception as needed, then rethrow preserving the stack trace.
                throw;
            }
        }

        private async Task<User> AuthenticateWithMobile(string username)
		{
			try
			{
				var data = await _userRepository.Table
					.Where(x => x.PhoneNumber.ToLower() == username.ToLower()).FirstOrDefaultAsync();
                return data;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		private async Task<User> Authenticate(string username,string LoginMethod, string password)
        {
            try
            {
                var data = await _userRepository.Table
                    .Where(x => x.UserName.ToLower() == username.ToLower()
                    || x.Email.ToLower() == username.ToLower()).FirstOrDefaultAsync();

                if (data != null)
                {
                    var passwordMatch = false;
                    if(LoginMethod == "google" || LoginMethod == "facebook")
                    {
						passwordMatch = PasswordHelper.VerifyPassword(password, data.SecondaryPassword);
                    }
                    else
                    {
						passwordMatch = PasswordHelper.VerifyPassword(password, data.Password);
					}
					if (passwordMatch)
                    {
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<User> IsUserExist(string username)
        {
            var data = await _userRepository.Table.Where(x => x.UserName == username 
            || x.Email.ToLower() == username.ToLower()
			|| x.PhoneNumber.ToLower() == username.ToLower()).FirstOrDefaultAsync();
            return data;
        }

		public async Task<JsonResponse> SignIn(string username, string password,string LoginMethod, int isMobile)
		{
			User user = await IsUserExist(username);
			if (user == null)
			{
				return new JsonResponse(204, true, "Not Exist", null);
			}

			bool isAuthenticated = false;

			// Handle mobile login
			if (isMobile > 0)
			{
				var response = await VerifiedPhoneReq(new VerifiedPhone { OTP = password, Phone = username });
				if (response.Success && response.Message == "Phone Number Verified")
				{
					isAuthenticated = true;
				}
				else
				{
					return new JsonResponse(200, true, "InvalidPhone", new LoginResponse());
				}
			}
			// Handle non-mobile login
			else
			{
				user = await Authenticate(username, LoginMethod, password);
				if (user != null)
				{
					isAuthenticated = true;
				}
				else
				{
					return new JsonResponse(200, true, "UnAuthenticated", new LoginResponse());
				}
			}

			if (isAuthenticated)
			{
				var profileModal = _profileService.GetUserProfile(user);
				var authClaims = new List<Claim>
		        {
			        new Claim("Username", username),
			        new Claim("Id", user.Id.ToString()),
			        new Claim("Exp", DateTime.Now.AddMonths(1).ToString())
		        };
				var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

				var token = new JwtSecurityToken(
					issuer: _configuration["JWT:ValidIssuer"],
					audience: _configuration["JWT:ValidAudience"],
					expires: DateTime.Now.AddMonths(1),
					claims: authClaims,
					signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
				);

				return new JsonResponse(200, true, "Success", new LoginResponse
				{
					Token = new JwtSecurityTokenHandler().WriteToken(token),
					Profile = profileModal
				});
			}

			return new JsonResponse(200, true, "UnAuthenticated", new LoginResponse());
		}

		string GenerateRandomPassword(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }

            return password.ToString();
        }

        public async Task<JsonResponse> ForgotPasswordRequest(string email)
        {
            var user = _userRepository.Table.Where(x => x.Email == email).FirstOrDefault();
            if (user == null)
            {
                return new JsonResponse(200, false, "Bad Request", null);
            }

            PasswordResetRequest passwordResetRequest = new PasswordResetRequest();
            passwordResetRequest.UserId = user.Id;
            passwordResetRequest.OTP = StringHelper.GenerateRandomNumber;
            passwordResetRequest.ActivationDate = DateTime.Now;
            passwordResetRequest.ExpirtionDate = DateTime.Now.AddMinutes(15);
            passwordResetRequest.IsUsed = false;
            await _passwordResetRequestRepository.InsertAsync(passwordResetRequest);

            // string encryptedotp = CommonHelper.EncryptString(passwordResetRequest.OTP.ToString());
            // string encrypteduserid = CommonHelper.EncryptString(passwordResetRequest.UserId.ToString());

            var byteotp = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(passwordResetRequest.OTP));
            string encryptedotp = Convert.ToBase64String(byteotp);
            var byteuserid = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(passwordResetRequest.UserId));
            string encrypteduserid = Convert.ToBase64String(byteuserid);

            EmailRequest emailRequest = new EmailRequest();
            emailRequest.USERNAME = user.UserName;
            emailRequest.CONTENT1 = "Oops, it happens to the best of us! If you've forgotten your password, don't worry. We're here to help you regain access to your " + GlobalVariables.SiteName + " account.";
            emailRequest.CONTENT2 = "If you have any questions, we're here to help. Just reach out.";
            emailRequest.CTALINK = GlobalVariables.SiteUrl + "/forgotPassword/" + encryptedotp + "/" + encrypteduserid;
            emailRequest.CTATEXT = "Click here to reset your password";
            emailRequest.ToEmail = user.Email;
            emailRequest.Subject = "Password Reset Request : " + GlobalVariables.SiteName;

            SMTPDetails smtpDetails = new SMTPDetails();
            smtpDetails.Username = GlobalVariables.SMTPUsername;
            smtpDetails.Host = GlobalVariables.SMTPHost;
            smtpDetails.Password = GlobalVariables.SMTPPassword;
            smtpDetails.Port = GlobalVariables.SMTPPort;
            smtpDetails.SSLEnable = GlobalVariables.SSLEnable;
            var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);
            return new JsonResponse(200, true, "Success", null);
        }

        public async Task<JsonResponse> ChangePassword(ChangePasswordReq req, int UserId)
        {
            try
            {
                var user = await _userRepository.Table.Where(x => x.Id == UserId &&
                x.IsDeleted == false).FirstOrDefaultAsync();

                if (PasswordHelper.VerifyPassword(req.CurrentPassword, user.Password))
                {
                    user.Password = PasswordHelper.EncryptPassword(req.NewPassword);
                    await _userRepository.UpdateAsync(user);
                    return new JsonResponse(200, true, "Password Changed Successfully!", null);
                }
                else
                {
                    return new JsonResponse(200, false, "Incorrect Current Password!", null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> ValidateOTP(string encryptedotp, string encrypteduserid)
        {
            if (!string.IsNullOrEmpty(encryptedotp) && !string.IsNullOrEmpty(encrypteduserid))
            {
                var resetpasswordreq = await _passwordResetRequestRepository.Table
                    .Where(x => x.UserId.ToString() == encrypteduserid && x.OTP == encryptedotp && x.IsUsed == false).FirstOrDefaultAsync();

                resetpasswordreq.IsUsed = true;

                await _passwordResetRequestRepository.UpdateAsync(resetpasswordreq);

                if (resetpasswordreq != null)
                {

                    if (DateTime.Now < resetpasswordreq.ActivationDate || DateTime.Now > resetpasswordreq.ExpirtionDate)
                    {
                        return new JsonResponse(200, false, "Link Expired", null);
                    }
                    else
                    {
                        var newpassword = GenerateRandomPassword(10);

                        var user = _userRepository.GetById(Convert.ToInt32(encrypteduserid));

                        user.Password = PasswordHelper.EncryptPassword(newpassword.ToString());

                        await _userRepository.UpdateAsync(user);

                        EmailRequest emailRequest = new EmailRequest();
                        emailRequest.USERNAME = user.UserName;
                        emailRequest.CONTENT1 = "Your new password, please use below pasword to login to " + GlobalVariables.SiteName + " account.";
                        emailRequest.CONTENT2 = "New Password: " + newpassword;
                        emailRequest.CTALINK = GlobalVariables.SiteUrl + "/Login";
                        emailRequest.CTATEXT = "Click here to login";
                        emailRequest.ToEmail = user.Email;
                        emailRequest.Subject = "Welcome to " + GlobalVariables.SiteName;

                        SMTPDetails smtpDetails = new SMTPDetails();
                        smtpDetails.Username = GlobalVariables.SMTPUsername;
                        smtpDetails.Host = GlobalVariables.SMTPHost;
                        smtpDetails.Password = GlobalVariables.SMTPPassword;
                        smtpDetails.Port = GlobalVariables.SMTPPort;
                        smtpDetails.SSLEnable = GlobalVariables.SSLEnable;
                        var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);

                        return new JsonResponse(200, true, "Success", null);
                    }


                }
            }
            return new JsonResponse(200, true, "Something went wrong", null);
        }

        public async Task<JsonResponse> SignUpNew(SignupRequest request)
        {
			request.Email = request.Email.TrimEnd().TrimStart().ToLower();
            if(!String.IsNullOrEmpty(request.Email))
            {
				if (request.Email.Contains(" "))
				{
					return new JsonResponse(200, false, "Space not allowed in Email", null);
				}

				var existingUser = _userRepository.Table.FirstOrDefault(u => u.Email == request.Email);

				if (existingUser != null)
				{
					if (request.LoginMethod == "google" && String.IsNullOrEmpty(existingUser.GoogleId))
					{
						// Existing manual signup, allow linking with social account
						existingUser.GoogleId = request.Password;
						existingUser.ProfileImg = request.ProfileImg;
						existingUser.SecondaryPassword = PasswordHelper.EncryptPassword(request.Password);
						await _userRepository.UpdateAsync(existingUser);
						return new JsonResponse(200, true, "Account linked to social login successfully", null);
					}
					else if (request.LoginMethod != "google" && !String.IsNullOrEmpty(existingUser.GoogleId))
					{
						// Existing social signup, allow linking with manual account
						existingUser.Password = PasswordHelper.EncryptPassword(request.Password);
						await _userRepository.UpdateAsync(existingUser);
						return new JsonResponse(200, true, "Account linked to manual login successfully", null);
					}
					else
					{
						return new JsonResponse(200, false, "Account already exists", null);
					}

				}
			}

            // Validate unique mobile number
            if (!String.IsNullOrEmpty(request.PhoneNumber))
            {
                var existingUserByMobile = _userRepository.Table.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
                if (existingUserByMobile != null)
                {
                    return new JsonResponse(200, false, "Phone number already registered.", null);
                }
            }

			// Create a new user
			var user = _mapper.Map<User>(request);
			user.InviterId = 0;
            if (!String.IsNullOrEmpty(request.FirstName))
            {
				user.UserName = GenerateUniqueUsername(request.FirstName, request.LastName);
			}
			user.Password = PasswordHelper.EncryptPassword(request.Password);
			user.ProfileImg = request.ProfileImg;
			user.PaymentStatus = "";
			user.PaymentRef1 = "";
			user.PaymentRef2 = "";
			user.Status = "";
			await _userRepository.InsertAsync(user);

			PasswordResetRequest passwordResetRequest = new PasswordResetRequest();
			passwordResetRequest.UserId = user.Id;
			passwordResetRequest.OTP = StringHelper.GenerateRandomNumber;
			passwordResetRequest.ActivationDate = DateTime.Now;
			passwordResetRequest.ExpirtionDate = DateTime.Now.AddMinutes(15);
			passwordResetRequest.IsUsed = false;
			await _passwordResetRequestRepository.InsertAsync(passwordResetRequest);

			// string encryptedotp = CommonHelper.EncryptString(passwordResetRequest.OTP.ToString());
			// string encrypteduserid = CommonHelper.EncryptString(passwordResetRequest.UserId.ToString());
			try
			{
				var byteotp = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(passwordResetRequest.OTP));
				string encryptedotp = Convert.ToBase64String(byteotp);
				var byteuserid = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(passwordResetRequest.UserId));
				string encrypteduserid = Convert.ToBase64String(byteuserid);

				EmailRequest emailRequest = new EmailRequest();
				emailRequest.USERNAME = request.UserName;
				emailRequest.CONTENT1 = "Welcome aboard! We're delighted to have you as a part of our " + GlobalVariables.SiteName + " family. Get ready for an exciting journey with us!";
				emailRequest.CONTENT2 = "If you have any questions, we're here to help. Just reach out.";
				emailRequest.CTALINK = GlobalVariables.SiteUrl + "/Verifyemail/" + encryptedotp + "/" + encrypteduserid;
				emailRequest.CTATEXT = "Verify Email";
				emailRequest.ToEmail = request.Email;
				emailRequest.Subject = "Welcome to " + GlobalVariables.SiteName;

				SMTPDetails smtpDetails = new SMTPDetails();
				smtpDetails.Username = GlobalVariables.SMTPUsername;
				smtpDetails.Host = GlobalVariables.SMTPHost;
				smtpDetails.Password = GlobalVariables.SMTPPassword;
				smtpDetails.Port = GlobalVariables.SMTPPort;
				smtpDetails.SSLEnable = GlobalVariables.SSLEnable;

				var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);

				/* var Inviter_User = await _userRepository.Table.Where(x => x.UserName == signupRequest.InviterName)
						 .FirstOrDefaultAsync();

				 var totalCount = await _userRepository.Table
								 .Where(x => x.InviterId == Inviter_User.Id && x.IsDeleted == false)
								 .CountAsync();

				 await _notificationService.SendEmailNotification("newreferral", Inviter_User);*/
			}
			catch (Exception ex)
			{
				return new JsonResponse(200, true, "Success", user);
			}

			return new JsonResponse(200, true, "Success", user);
		}

		public  string GenerateUniqueUsername(string firstName, string lastName)
		{
			// Generate initial username by concatenating first and last name
			string baseUsername = $"{firstName.ToLower()}.{lastName.ToLower()}".Replace(" ", "");
			string username = baseUsername;
			int suffix = 1;

			List<string> existingUsernames = _userRepository.Table
	        .Where(x => x.FirstName == firstName && x.LastName == lastName)
	        .Select(x => x.UserName)
	        .ToList() ?? new List<string>();
			// Ensure the username is unique
			while (existingUsernames.Contains(username))
			{
				username = $"{baseUsername}{suffix}";
				suffix++;
			}

			return username;
		}
		public async Task<JsonResponse> SignUp(SignupRequest signupRequest)
		{
			try
			{
				signupRequest.UserName = signupRequest.UserName.TrimEnd().TrimStart().ToLower();
				if (signupRequest.UserName.Contains(" "))
				{
					return new JsonResponse(200, false, "Space not allowed in username", null);
				}

				var query = _userRepository.Table;
				if (!String.IsNullOrEmpty(signupRequest.PhoneNumber))
				{
					query = query.Where(x => x.PhoneNumber == signupRequest.PhoneNumber);
				}

				var data = await query.Where(x => x.IsDeleted == false &&
				(x.UserName.ToLower().Trim() == signupRequest.UserName.ToLower().Trim()
				|| x.Email.ToLower().Trim() == signupRequest.Email.ToLower().Trim()
				)).FirstOrDefaultAsync();

				if (data != null)
				{
					return new JsonResponse(200, false, "Username or Email already exists", null);
				}

				if (data == null)
				{
					User user = _mapper.Map<User>(signupRequest);
					user.InviterId = 0;
					user.Password = PasswordHelper.EncryptPassword(signupRequest.Password);
					user.ProfileImg = signupRequest.ProfileImg;
					user.PaymentStatus = "";
					user.PaymentRef1 = "";
					user.PaymentRef2 = "";
					user.Status = "";

					await _userRepository.InsertAsync(user);

					//var qresponse = await _question.InsertAnswerAsync(user.Id, signupRequest.Answers);

					PasswordResetRequest passwordResetRequest = new PasswordResetRequest();
					passwordResetRequest.UserId = user.Id;
					passwordResetRequest.OTP = StringHelper.GenerateRandomNumber;
					passwordResetRequest.ActivationDate = DateTime.Now;
					passwordResetRequest.ExpirtionDate = DateTime.Now.AddMinutes(15);
					passwordResetRequest.IsUsed = false;
					await _passwordResetRequestRepository.InsertAsync(passwordResetRequest);

					// string encryptedotp = CommonHelper.EncryptString(passwordResetRequest.OTP.ToString());
					// string encrypteduserid = CommonHelper.EncryptString(passwordResetRequest.UserId.ToString());
					try
					{
						var byteotp = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(passwordResetRequest.OTP));
						string encryptedotp = Convert.ToBase64String(byteotp);
						var byteuserid = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(passwordResetRequest.UserId));
						string encrypteduserid = Convert.ToBase64String(byteuserid);

						EmailRequest emailRequest = new EmailRequest();
						emailRequest.USERNAME = signupRequest.UserName;
						emailRequest.CONTENT1 = "Welcome aboard! We're delighted to have you as a part of our " + GlobalVariables.SiteName + " family. Get ready for an exciting journey with us!";
						emailRequest.CONTENT2 = "If you have any questions, we're here to help. Just reach out.";
						emailRequest.CTALINK = GlobalVariables.SiteUrl + "/Verifyemail/" + encryptedotp + "/" + encrypteduserid;
						emailRequest.CTATEXT = "Verify Email";
						emailRequest.ToEmail = signupRequest.Email;
						emailRequest.Subject = "Welcome to " + GlobalVariables.SiteName;

						SMTPDetails smtpDetails = new SMTPDetails();
						smtpDetails.Username = GlobalVariables.SMTPUsername;
						smtpDetails.Host = GlobalVariables.SMTPHost;
						smtpDetails.Password = GlobalVariables.SMTPPassword;
						smtpDetails.Port = GlobalVariables.SMTPPort;
						smtpDetails.SSLEnable = GlobalVariables.SSLEnable;

						var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);

						/* var Inviter_User = await _userRepository.Table.Where(x => x.UserName == signupRequest.InviterName)
                                 .FirstOrDefaultAsync();

                         var totalCount = await _userRepository.Table
                                         .Where(x => x.InviterId == Inviter_User.Id && x.IsDeleted == false)
                                         .CountAsync();

                         await _notificationService.SendEmailNotification("newreferral", Inviter_User);*/
					}
					catch (Exception ex)
					{
						return new JsonResponse(200, true, "Success", user);
					}

					return new JsonResponse(200, true, "Success", user);

				}
				else
				{
					return null;
				}

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<PreRegisteredUser> PreSignUp(PreSignupRequest req)
        {
            try
            {
                var res = await _preRegisteredUserRepository.Table
                    .Where(x => x.UserName == req.UserName || 
                    x.Email.ToLower().Trim() == req.Email.ToLower().Trim())
                    .FirstOrDefaultAsync();

                if (res == null)
                {
                    PreRegisteredUser preRegisteredUser = new PreRegisteredUser();
                    preRegisteredUser.UserName = req.UserName;
                    preRegisteredUser.FirstName = req.FirstName;
                    preRegisteredUser.LastName = req.LastName;
                    preRegisteredUser.Email = req.Email;
                    preRegisteredUser.IpAddress = req.IpAddress;

                    await _preRegisteredUserRepository.InsertAsync(preRegisteredUser);

                    return preRegisteredUser;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    
        public async Task<JsonResponse> CheckUsername(string username)
        {
            try
            {
                var data = await _userRepository.Table
                    .Where(x => x.UserName.Trim() == username)
                    .FirstOrDefaultAsync();

                if (data == null)
                {
                    return new JsonResponse(200, true, "Success", false);
                }
                else
                {
                    return new JsonResponse(200, true, "Success", true);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> VerifyEmail(string encryptedotp, string encrypteduserid)
        {
            if (!string.IsNullOrEmpty(encryptedotp) && !string.IsNullOrEmpty(encrypteduserid))
            {
                var resetpasswordreq = await _passwordResetRequestRepository.Table
                    .Where(x => x.UserId.ToString() == encrypteduserid && x.OTP == encryptedotp && x.IsUsed == false).FirstOrDefaultAsync();

                resetpasswordreq.IsUsed = true;

                await _passwordResetRequestRepository.UpdateAsync(resetpasswordreq);

                if (resetpasswordreq != null)
                {

                    if (DateTime.Now < resetpasswordreq.ActivationDate || DateTime.Now > resetpasswordreq.ExpirtionDate)
                    {
                        return new JsonResponse(200, false, "Link Expired", null);
                    }
                    else
                    {
                        var user = _userRepository.GetById(Convert.ToInt32(encrypteduserid));
                        user.IsEmailVerified = 1;
                        await _userRepository.UpdateAsync(user);

                        EmailRequest emailRequest = new EmailRequest();
                        emailRequest.USERNAME = user.UserName;
                        emailRequest.CONTENT1 = "Thankyou for verifying your Email with " + GlobalVariables.SiteName + " account.";
                        emailRequest.CONTENT2 = "We wish you good luck in your spirtual journey";
                        emailRequest.CTALINK = GlobalVariables.SiteUrl + "/Login";
                        emailRequest.CTATEXT = "Click here to login";
                        emailRequest.ToEmail = user.Email;
                        emailRequest.Subject = "Email Verified " + GlobalVariables.SiteName;

                        SMTPDetails smtpDetails = new SMTPDetails();
                        smtpDetails.Username = GlobalVariables.SMTPUsername;
                        smtpDetails.Host = GlobalVariables.SMTPHost;
                        smtpDetails.Password = GlobalVariables.SMTPPassword;
                        smtpDetails.Port = GlobalVariables.SMTPPort;
                        smtpDetails.SSLEnable = GlobalVariables.SSLEnable;
                        var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);

                        return new JsonResponse(200, true, "Success", true);
                    }


                }
            }
            return new JsonResponse(200, true, "Something went wrong", null);
        }
    
        public async Task FollowUnFollowUser(int userId,int loginUserId)
        {
            var exists = _userFollowersRepository.Table.Where(x => x.UserId == loginUserId && x.FollowToUserId == userId).FirstOrDefault();
            if(userId == loginUserId)
            {
                return;
            }
            if (exists == null)
            {
                UserFollowers follower = new UserFollowers();
                follower.UserId = loginUserId;
                follower.FollowToUserId = userId;
                _userFollowersRepository.Insert(follower);

               

                NotificationRes notification = new NotificationRes();
                notification.PostId = 0;
                notification.ActionByUserId = loginUserId;
                notification.ActionType = "follow";
                notification.RefId1 = userId.ToString();
                notification.RefId2 = "";
                notification.Message = "";
                notification.PushAttribute = "pushfollowyou";
				notification.EmailAttribute = "emailfollowyou";

				await _notificationService.SaveNotification(notification);
            }
            else
            {
                ActivityLog activity = new ActivityLog();
                activity.UserId = loginUserId;
                activity.RefId1 = userId;
                activity.ActivityType = "unfollow";
                activity.Type = "profile";

                var noti = await _notificationRepository.Table.Where(x => x.ActionByUserId == loginUserId && x.RefId1 == userId.ToString()
                   && x.ActionType == "follow").FirstOrDefaultAsync();
                if (noti != null)
                {
                    var unoti = await _userNotificationRepository.Table.Where(x => x.NotificationId == noti.Id).ToListAsync();
                    _notificationRepository.DeleteHard(noti);
                    _userNotificationRepository.DeleteHardRange(unoti);
                }

                _userFollowersRepository.DeleteHard(exists);

                _activityRepository.Insert(activity);
            }
        }

        public async Task FollowUsers(List<int> userIds, int loginUserId)
        {
            foreach (var userId in userIds)
            {
                var exists = _userFollowersRepository.Table
                    .FirstOrDefault(x => x.UserId == loginUserId && x.FollowToUserId == userId && x.IsDeleted == false);

                if (exists == null)
                {
                    UserFollowers follower = new UserFollowers
                    {
                        UserId = loginUserId,
                        FollowToUserId = userId
                    };
                    _userFollowersRepository.Insert(follower);

                    NotificationRes notification = new NotificationRes
                    {
                        PostId = 0,
                        ActionByUserId = loginUserId,
                        ActionType = "follow",
                        RefId1 = userId.ToString(),
                        RefId2 = "",
                        Message = "",
                        PushAttribute = "pushfollowyou",
                        EmailAttribute = "emailfollowyou"
                    };

                    await _notificationService.SaveNotification(notification);
                }
            }
        }



        public void BlockMuteUser(int userId, int loginUserId,string type)
        {
            var query = _userMuteBlockListRepository.Table.Where(x => x.UserId == loginUserId);

            if (type == "mute")
            {
                var exist = _userMuteBlockListRepository.Table
                    .Where(x=> x.UserId == loginUserId && x.MuteedUserId == userId).FirstOrDefault();
                if(exist == null)
                {
                    UserMuteBlockList mutedUser = new UserMuteBlockList();
                    mutedUser.UserId = loginUserId;
                    mutedUser.MuteedUserId = userId;
                    _userMuteBlockListRepository.Insert(mutedUser);
                }
                else
                {
                    _userMuteBlockListRepository.DeleteHard(exist);
                }
            }
            else
            {
                var exist = _userMuteBlockListRepository.Table.Where(x => x.UserId == loginUserId && x.BlockedUserId == userId).FirstOrDefault();
                if (exist == null)
                {
                    UserMuteBlockList blockedUser = new UserMuteBlockList();
                    blockedUser.UserId = loginUserId;
                    blockedUser.BlockedUserId = userId;
                    _userMuteBlockListRepository.Insert(blockedUser);
                }
                else
                {
                    _userMuteBlockListRepository.DeleteHard(exist);
                }
            }
        }
        
        public async Task<User> GetUserByName(string Username)
        {
			var data = await _userRepository.Table
	            .Where(x => x.IsDeleted == false && x.UserName == Username)
	            .FirstOrDefaultAsync();

			if (data == null)
			{
				data = await _userRepository.Table
					.Where(x => x.IsDeleted == false && x.Email == Username)
					.FirstOrDefaultAsync();
			}
			return data;
		}

		public async Task<JsonResponse> GetOnlineUsers(int Id)
        {
            try
            {
                var data = await (from uf in _userFollowersRepository.Table
                            join ou in _onlineUsers.Table on uf.FollowToUserId equals ou.UserId
                            join user in _userRepository.Table on ou.UserId equals user.Id
                            where uf.UserId == Id
                            select new
                            {
                                 Id = ou.Id,
                                 UserId = ou.UserId,
                                 ConnectionId = ou.ConnectionId,
                                 Username = user.UserName,
                                 FirstName = user.FirstName,
                                 LastName = user.LastName,
                                 ProfileImg = user.ProfileImg,
                                 IsBusinessAccount = user.IsBusinessAccount,
                                 }).ToListAsync();

                //var data = await (from onlineuser in _onlineUsers.Table.Where(x => x.IsDeleted == false)
                //             join
                //             user in _userRepository.Table.Where(x => x.IsDeleted == false)
                //             on onlineuser.UserId equals user.Id
                //             select new
                //             {
                //                 Id = onlineuser.Id,
                //                 UserId = onlineuser.UserId,
                //                 ConnectionId = onlineuser.ConnectionId,
                //                 Username = user.UserName,
                //                 FirstName = user.FirstName,
                //                 LastName = user.LastName
                //             }).ToListAsync();

                return new JsonResponse(200,true,"Success",data);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> StoreUserNetwork(UserNetworkReq req, int inviterId)
        {
            try
            {
                List<UserNetwork> list = new List<UserNetwork>();
                foreach (var item in req.list)
                {
                    UserNetwork userNetwork = new UserNetwork();

                    userNetwork.InviterId = inviterId;
                    userNetwork.UniqueId = item.UniqueId;
                    userNetwork.PhoneNumber = item.PhoneNumber;
                    userNetwork.FirstName = item.FirstName;
                    userNetwork.LastName = item.LastName;
                    userNetwork.FullName = item.FullName;
                    userNetwork.Email = item.Email;
                    userNetwork.Photo = item.Photo;

                    list.Add(userNetwork);
                }

                var DbUserNetwork = await _userNetworkRepository.Table.Where(x => x.InviterId == inviterId
                            && x.IsDeleted == false).ToListAsync();

                list = list.Where(nuser => !DbUserNetwork.Any(user => 
                    user.UniqueId == nuser.UniqueId &&
                    user.PhoneNumber == nuser.PhoneNumber &&
                    user.Email == nuser.Email &&
                    user.InviterId == inviterId
                    )).ToList();

                /* 
                 * var AvailableUsers = await _userRepository.Table.ToListAsync();

                var results = list.Where(nuser => 
                AvailableUsers.Any(user => user.Email != nuser.Email || user.PhoneNumber != nuser.PhoneNumber))
                    .ToList(); */

                await _userNetworkRepository.InsertRangeAsync(list);

                return new JsonResponse(200,true,"Success",list);
            }
            catch(Exception ex)
            {
                throw ex;
            }   
        }

        public static string NormalizePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return phoneNumber;

            string normalized = Regex.Replace(phoneNumber, @"[^\d+]", "");

            return normalized; 
        }


        public async Task<JsonResponse> getUserInviteList(int UserId)
            {
            
                try
                {
                var userIdParam = new NpgsqlParameter("@userid", UserId);
                var result = await _context.InviteUserRes
                              .FromSqlRaw("SELECT * FROM dbo.getUserInvitesList(@userid)", userIdParam)
                              .ToListAsync();

                return new JsonResponse(200, true, "Success", result);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        public async Task<JsonResponse> GetUserFromYourContact(int UserId)
        {
            try
            {
                var userIdParam = new NpgsqlParameter("@UserId", UserId);
                var result = await _context.ContactUserRes
                              .FromSqlRaw("SELECT * FROM dbo.getUserWithMatchingPhoneNumber(@UserId)", userIdParam)
                              .ToListAsync();

                List<ProfileModel> userList = new List<ProfileModel>();
                
                var userIds = result.Select(x=> x.Id).ToList();

               

                var spuserList = (await Task.WhenAll(userIds.Select(async id =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope()) 
                    {
                        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>(); 
                        return await profileService.GetUserInfoBoxByUserId(id, UserId); 
                    }
                }))).ToList();

                userList.AddRange(spuserList);

                if (userList.Count < 20)
                {
                    int remainingCount = 20 - userList.Count;
                    var whoToFollowResponse = await _profileService.GetWhoToFollow(UserId, 1); // Assuming page=1 for simplicity

                    if (whoToFollowResponse.Result is List<ProfileModel> followList)
                    {
                        // Filter out users already in userList
                        //var existingUserIds = new HashSet<int>(userList.Select(u => u.Id));
                        var filteredFollowList = followList.Where(u => !userIds.Contains(u.Id))
                                                           .Take(remainingCount)
                                                           .ToList();

                        userList.AddRange(filteredFollowList);
                    }
                }

                return new JsonResponse(200, true, "Success", userList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SendInvitationMail(string Emailreq, int UserId, int Id)
        {
            try
            {
                var User = await _userNetworkRepository.GetByIdAsync(Id);

                EmailRequest emailRequest = new EmailRequest();
                emailRequest.USERNAME = User.FullName;
                emailRequest.CONTENT1 = "Welcome aboard! We're delighted to have you as a part of our " + GlobalVariables.SiteName + " family. Get ready for an exciting journey with us!";
                emailRequest.CONTENT2 = "If you have any questions, we're here to help. Just reach out.";
                emailRequest.CTALINK = "This is invitation";
                emailRequest.CTATEXT = "Verify Email";
                emailRequest.ToEmail = Emailreq;
                emailRequest.Subject = "Welcome to " + GlobalVariables.SiteName;

                SMTPDetails smtpDetails = new SMTPDetails();
                /*smtpDetails.Username = GlobalVariables.SMTPUsername;
                smtpDetails.Host = GlobalVariables.SMTPHost;
                smtpDetails.Password = GlobalVariables.SMTPPassword;
                smtpDetails.Port = GlobalVariables.SMTPPort;
                smtpDetails.SSLEnable = GlobalVariables.SSLEnable;*/

                smtpDetails.Username = "support@generositymatrix.net";
                smtpDetails.Host = "mail.generositymatrix.net";
                smtpDetails.Password = "Owc9$5qmf29sFywoM";
                smtpDetails.Port = "25";
                smtpDetails.SSLEnable = "true";

                var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);

                User.IsInvited = true;

                await _userNetworkRepository.UpdateAsync(User);

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SendSmsMail(string Phonenumber, int UserId, int Id)
        {
            // Twilio Account SID and Auth Token
            const string accountSid = "AC107b3a6bc246fe30993669cac660fd52";
            const string authToken = "3b304b4ff0466a9b0e068e0eb0c82276";

            // Initialize Twilio client
            TwilioClient.Init(accountSid, authToken);

            // Phone numbers
            string fromPhoneNumber = "+91" + "9737443888";
            string toPhoneNumber = "+91" +  Phonenumber;

            // Message content
            string messageBody = "This is a test message from your C# application!";

            try
            {
                var message = Twilio.Rest.Api.V2010.Account.MessageResource.Create(
                body: messageBody,
                from: new Twilio.Types.PhoneNumber(fromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(toPhoneNumber));


                var DbUser = await _userNetworkRepository.GetByIdAsync(Id);
                DbUser.IsInvited = true;

                await _userNetworkRepository.UpdateAsync(DbUser);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        public async Task<JsonResponse> UpdateLocation(string Latitude, string Longitude, int UserId)
        {
            try
            {
                var User = await _userRepository.GetByIdAsync(UserId);
                User.Latitude = Latitude;
                User.Longitude = Longitude;
                await _userRepository.UpdateAsync(User);
                return new JsonResponse(200, true, "Success", null);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> UpdateUserAttribute(UserAttributeRequest req)
        {
            try
            {
                var UserAttribute = await _userAttributeRepository.Table.Where(x=>x.KeyName == req.KeyName && x.UserId == req.UserId).FirstOrDefaultAsync();
                if(UserAttribute != null) 
                {    
                    UserAttribute.Value = req.Value;
                    await _userAttributeRepository.UpdateAsync(UserAttribute);
                    return new JsonResponse(200, true, "Success", null);
                }
                else
                {
                    UserAttribute newrequest = new UserAttribute();
                    newrequest.UserId = req.UserId;
                    newrequest.KeyName = req.KeyName;
                    newrequest.Value = req.Value;
                    await _userAttributeRepository.InsertAsync(newrequest);
                }
                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetUserAttribute(int UserId)
        {
            try
            {
                var User = await _userAttributeRepository.Table.Where(x => x.UserId == UserId).ToListAsync();
                return new JsonResponse(200, true, "Success", User);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetBlockUserList(int UserId)
        {
            try
            {
                var query = await (from bl in _userMuteBlockListRepository.Table 
                            join us in _userRepository.Table on bl.BlockedUserId equals us.Id
                            where bl.UserId == UserId
                            select new BlockUserRes
                            {
                                Id = us.Id,
                                FullName = us.FirstName+" "+us.LastName,
                                ProfileImgUrl = us.ProfileImg,
                                UserName = us.UserName,
                                IsBusinessAccount = us.IsBusinessAccount,
                            }).ToListAsync();
                return new JsonResponse(200, true, "Success", query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> Invitation(string name)
        {
            try
            {
                _InvitationRepository.Insert(new Entities.Invitation
                {
                    Name = name
                });
                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		public async Task<JsonResponse> EmailVerificationReq(EmailVerificationReq req)
		{
            var check = await _emailVerificationRequestRepository.Table.Where(x=> x.Email ==  req.Email && x.IsUsed == false && x.IsDeleted == false).FirstOrDefaultAsync();
            if (req.Email == null)
            {
                return new JsonResponse(200, false, "Bad Request", null);
            }

            EmailVerificationRequest EmailRequest = new EmailVerificationRequest();
			EmailRequest.Email = req.Email;
			//EmailRequest.OTP = StringHelper.GenerateRandomNumber;
            EmailRequest.OTP = "123456";
            EmailRequest.ActivationDate = DateTime.Now;
			EmailRequest.ExpirtionDate = DateTime.Now.AddMinutes(15);
			EmailRequest.IsUsed = false;
			if (check != null)
			{
                check.OTP = EmailRequest.OTP;
                check.ActivationDate = EmailRequest.ActivationDate;
                check.ExpirtionDate= EmailRequest.ExpirtionDate;
                await _emailVerificationRequestRepository.UpdateAsync(check);
            }
            else
            {
				await _emailVerificationRequestRepository.InsertAsync(EmailRequest);
			}


			EmailRequest emailRequest = new EmailRequest();
			emailRequest.USERNAME = "Dear "+ req.FirstName + " " +req.LastName +",";
			emailRequest.CONTENT1 = "Welcome to " + GlobalVariables.SiteName + " ! We're excited to have you as part of our community. To complete the registration process and activate your account, please verify your email address.";
			emailRequest.CONTENT2 = "If you have any questions, we're here to help. Just reach out.";
			emailRequest.CTALINK = EmailRequest.OTP;
			emailRequest.CTATEXT = "Please enter this OTP on the verification page in the app to confirm your email address, This code is valid for 15 minutes ";
			emailRequest.ToEmail = req.Email;
			emailRequest.Subject = " Verify Your Account: Your OTP for " + GlobalVariables.SiteName;

			SMTPDetails smtpDetails = new SMTPDetails();
			smtpDetails.Username = GlobalVariables.SMTPUsername;
			smtpDetails.Host = GlobalVariables.SMTPHost;
			smtpDetails.Password = GlobalVariables.SMTPPassword;
			smtpDetails.Port = GlobalVariables.SMTPPort;
			smtpDetails.SSLEnable = GlobalVariables.SSLEnable;
			var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);
			return new JsonResponse(200, true, "Success", null);
		}


		public async Task<JsonResponse> VerifiedEmailReq(VerifiedEmail req)
		{
			try
			{
                var check = await _emailVerificationRequestRepository.Table.Where(x=> x.Email ==  req.Email && x.OTP == req.OTP && x.IsUsed == false && x.IsDeleted == false).FirstOrDefaultAsync();
                if (check != null) 
                {
                    check.IsUsed = true;
                    await _emailVerificationRequestRepository.UpdateAsync(check);
			    	return new JsonResponse(200, true, "Email Verified", null);
				}
				return new JsonResponse(200, false, "Invalid OTP", null);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}


        public async Task<JsonResponse> PhoneVerificationReq(PhoneVerificationReq req)
        {
            var check = await _phoneVerificationRequestRepository.Table.Where(x => x.PhoneNumber == req.Phone && x.IsUsed == false 
            && x.IsDeleted == false).FirstOrDefaultAsync();

            if (req.Phone == null)
            {
                return new JsonResponse(200, false, "Bad Request", null);
            }

            PhoneVerificationRequest phoneRequest = new PhoneVerificationRequest();
            phoneRequest.PhoneNumber = req.Phone;
            if(req.Phone == "+919404437591" || req.Phone == "+919958355307" || req.Phone == "+919423405704" || req.Phone == "+919423405704")
            {
				phoneRequest.OTP = StringHelper.GenerateRandomNumber;
            }
            else
            {
				phoneRequest.OTP = "123456";
			}
            phoneRequest.ActivationDate = DateTime.Now;
            phoneRequest.ExpirtionDate = DateTime.Now.AddMinutes(15);
            phoneRequest.IsUsed = false;
            if (check != null)
            {
                check.OTP = phoneRequest.OTP;
                check.ActivationDate = phoneRequest.ActivationDate;
                check.ExpirtionDate = phoneRequest.ExpirtionDate;
                await _phoneVerificationRequestRepository.UpdateAsync(check);
            }
            else
            {
                await _phoneVerificationRequestRepository.InsertAsync(phoneRequest);
            }

            if(phoneRequest.OTP != "123456")
            {
                string accountSid = GlobalVariables.TwilioaccountSid;
                string authToken = GlobalVariables.TwilioauthToken;

				TwilioClient.Init(accountSid, authToken);

				var message = Twilio.Rest.Api.V2010.Account.MessageResource.Create(
					body: phoneRequest.OTP + " is your login OTP for K4M2A App.Do not share it with anyone. kgRDFNv939x - K4M2A",
					from: new Twilio.Types.PhoneNumber("+17348905624"), // Your Twilio phone number
					to: new Twilio.Types.PhoneNumber(req.Phone) // Recipient's phone number
				);
			}

			var user = _userRepository.Table.Where(x => x.PhoneNumber == phoneRequest.PhoneNumber).FirstOrDefault();
            if(user == null)
            {
                SignupRequest request = new SignupRequest();
                request.PhoneNumber = phoneRequest.PhoneNumber;
                request.InviterName = "";
                request.FirstName = "";
                request.LastName = "";
                request.Email = "";
                request.UserName = "";
                request.Password = "";
                await SignUpNew(request);
            }
            return new JsonResponse(200, true, "Success", null);
        }


        public async Task<JsonResponse> VerifiedPhoneReq(VerifiedPhone req)
        {
            try
            {
                var check = await _phoneVerificationRequestRepository.Table.Where(x => x.PhoneNumber == req.Phone && 
                x.OTP == req.OTP && x.IsUsed == false && x.IsDeleted == false).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.IsUsed = true;
                    await _phoneVerificationRequestRepository.UpdateAsync(check);

                    if (DateTime.Now > check.ExpirtionDate)
                    {
                        return new JsonResponse(200, false, "OTP has expired", null);
                    }
                    
                    return new JsonResponse(200, true, "Phone Number Verified", null);
                }
                return new JsonResponse(200, false, "Invalid OTP", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> getTagsList()
		{
			try
			{
                var tags = await _tagsRepository.Table.ToListAsync();
				return new JsonResponse(200, true, "Success", tags);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<JsonResponse> RequestInvite(RequestInviteRequest request)
		{

            if(request.id == 0)
            {
                var exist = _inviteRequest.Table.Where(x => x.Email.ToLower() == request.email.ToLower()).FirstOrDefault();
                if (exist !=null)
                {
					return new JsonResponse(200, true, "Fail", "Already registered");
				}
				var user = new InviteRequest();
				if (!String.IsNullOrEmpty(request.inviter))
				{
					byte[] decodedBytes = Convert.FromBase64String(request.inviter);
					int originalUserId = Convert.ToInt16(System.Text.Encoding.UTF8.GetString(decodedBytes));
					user.InviterId = originalUserId;
				}
				user.Email = request.email;
				user.CreatedDate = DateTime.UtcNow;
				_inviteRequest.Insert(user);

				var byteuserid = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(user.Id));
				string encrypteduserid = Convert.ToBase64String(byteuserid);

				EmailRequest emailRequest = new EmailRequest();
				emailRequest.ToEmail = request.email;
				emailRequest.Subject = " Thank You for Your Interest in " + GlobalVariables.SiteName;
				emailRequest.SITETITLE = GlobalVariables.SiteName;
				SMTPDetails smtpDetails = new SMTPDetails();
				smtpDetails.Username = GlobalVariables.SMTPUsername;
				smtpDetails.Host = GlobalVariables.SMTPHost;
				smtpDetails.Password = GlobalVariables.SMTPPassword;
				smtpDetails.Port = GlobalVariables.SMTPPort;
				smtpDetails.SSLEnable = GlobalVariables.SSLEnable;
				var body = EmailHelper.SendEmailRequestWithtemplate(emailRequest, smtpDetails,"mailtemplate/requestInvite.html");

				return new JsonResponse(200, true, "Success", user);
			}
            else
            {
                var phoneexist = _inviteRequest.Table.Where(x => x.Phone == request.phone).FirstOrDefault();
                if (phoneexist == null)
                {
                    var user = _inviteRequest.GetById(request.id);
                    user.Name = request.name;
                    user.Phone = request.phone;
                    user.City = request.city;
                    user.Journey = request.journey;
                    _inviteRequest.Update(user);
					return new JsonResponse(200, true, "Success", user);
                }
                else
                {
					var user = _inviteRequest.GetById(request.id);
					user.Name = request.name;
					user.Phone = request.phone;
					user.City = request.city;
					user.Journey = request.journey;
					_inviteRequest.Update(user);
					return new JsonResponse(200, true, "Fail", "Phone number already registered");
				}
			}

           
		}
	}
}
