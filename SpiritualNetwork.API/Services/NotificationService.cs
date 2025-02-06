using AutoMapper;
using Microsoft.Extensions.Configuration;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Common;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using static SpiritualNetwork.API.Services.ReactionService;
using System.Text.Json;
using SpiritualNetwork.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Azure.Core;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using RestSharp;
using static System.Collections.Specialized.BitVector32;
using Event = SpiritualNetwork.Entities.Event;
using Community = SpiritualNetwork.Entities.Community;
using System.IO.Hashing;
using SpiritualNetwork.API.AppContext;
using Org.BouncyCastle.Utilities.Collections;
using static Antlr4.Runtime.Atn.SemanticContext;

namespace SpiritualNetwork.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IGlobalSettingService _globalSettingService;
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<Reaction> _reactionRepository;
        private readonly IRepository<UserFollowers> _userFollowers;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<OnlineUsers> _onlineUserRepository;
        private readonly IRepository<UserPost> _userPostRepository;
        private readonly IRepository<ChatMessages> _chatRepository;
        private readonly IRepository<UserNotification> _userNotificationRepository;
        private readonly IRepository<EventAttendee> _eventAttendeeRepository;
        private readonly IHubContext<NotificationHub, INotificationHub> _notificationHub;
        private readonly IRepository<Community> _communityRepository;
        private readonly IRepository<CommunityMember> _communityMemberRepository;
        private readonly IRepository<CommunityReportPost> _communityReportRepository;
        private readonly IRepository<EventSpeakers> _eventspeakersRepository;
        private readonly IRepository<Event> _eventRepository;
		private readonly IRepository<UserAttribute> _userAttributeRepository;

		private readonly IRestClient _client;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public NotificationService(IRepository<EmailTemplate> emailTemplateRepository,
        IGlobalSettingService globalSettingService, IRepository<Notification> notificationRepository, IMapper mapper, IRepository<Reaction> reactionRepository, IRepository<UserFollowers> userFollowers, IRepository<User> userRepository, IRepository<OnlineUsers> onlineUserRepository,
        IHubContext<NotificationHub, INotificationHub> notificationHub, IRepository<ChatMessages> chatRepository,
        IRepository<UserPost> userPostRepository, IRepository<UserNotification> userNotificationRepository, IRestClient client,
        IRepository<EventAttendee> eventAttendeeRepository, IRepository<CommunityMember> communityMemberRepository, 
        IRepository<CommunityReportPost> communityReportRepository, IRepository<EventSpeakers> eventspeakersRepository,
        IRepository<Event> eventRepository, IRepository<Community> communityRepository, AppDbContext context,
		IRepository<UserAttribute> userAttributeRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _globalSettingService = globalSettingService;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _reactionRepository = reactionRepository;
            _userFollowers = userFollowers;
            _userRepository = userRepository;
            _onlineUserRepository = onlineUserRepository;
            _notificationHub = notificationHub;
            _userPostRepository = userPostRepository;
            _userNotificationRepository = userNotificationRepository;
            _client = client;
            _chatRepository = chatRepository;
            _eventAttendeeRepository = eventAttendeeRepository;
            _communityMemberRepository = communityMemberRepository;
            _communityReportRepository = communityReportRepository;
            _eventspeakersRepository = eventspeakersRepository;
            _eventRepository = eventRepository;
            _communityRepository = communityRepository;
            _context = context;
            _userAttributeRepository = userAttributeRepository;
        }

         
        public class NotificationReq
        {
            public int Id { get; set; }
        }
        public async Task NodeNotification(int Id)
        {
            var options = new RestClientOptions(GlobalVariables.NotificationAPIUrl)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/notification/notify", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", GlobalVariables.Token);
            NotificationReq notifyReq = new NotificationReq();
            notifyReq.Id = Id;
            var body = JsonSerializer.Serialize(notifyReq);
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine(response.Content);
        }

        public async Task SendEmailNotification(string emailType, User user)
        {
            var emailTemplate = _emailTemplateRepository.Table.Where(x => x.EmailType == emailType).FirstOrDefault();
            EmailRequest emailRequest = new EmailRequest();
            emailRequest.SITETITLE = await _globalSettingService.GetValue("SITENAME");
            emailRequest.USERNAME = user.UserName;
            emailRequest.CONTENT1 = emailTemplate.Content1;
            emailRequest.CONTENT2 = emailTemplate.Content2;
            emailRequest.CTALINK = emailTemplate.CTALink;
            emailRequest.CTATEXT = emailTemplate.CTAText;
            emailRequest.ToEmail = user.Email;
            emailRequest.Subject = emailTemplate.Subject + await _globalSettingService.GetValue("SITENAME");
            emailRequest.SUPPORTEMAIL = await _globalSettingService.GetValue("SupportEmail");

            SMTPDetails smtpDetails = new SMTPDetails();
            smtpDetails.Username = GlobalVariables.SMTPUsername;
            smtpDetails.Host = GlobalVariables.SMTPHost;
            smtpDetails.Password = GlobalVariables.SMTPPassword;
            smtpDetails.Port = GlobalVariables.SMTPPort;
            smtpDetails.SSLEnable = GlobalVariables.SSLEnable;
            EmailHelper.SendEmailRequest(emailRequest, smtpDetails);
        }

        private async Task<bool> GetNotificationPrefrence(int ToUserId, string Attribute,int FromUserId)
        {
            var check = await _userAttributeRepository.Table.Where(x=> x.UserId == ToUserId && x.KeyName == Attribute && x.IsDeleted == false).FirstOrDefaultAsync();
            var mute = await _userAttributeRepository.Table.Where(x => x.UserId == ToUserId && x.IsDeleted == false && 
                                            (x.KeyName == "muteyoudontfollow" || x.KeyName == "mutedontfollowyou" || x.KeyName == "mutenewaccount" ||
											x.KeyName == "mutedefaultprofilephoto" || x.KeyName == "muteunconfirmedemail" || x.KeyName == "muteunconfirmedphone")).ToListAsync();
            if (mute != null)
            {
				var isFollower = (from uf in _userFollowers.Table
								   join u in _userRepository.Table on uf.UserId equals u.Id
								   where uf.FollowToUserId == ToUserId && uf.UserId == FromUserId
								   select u).FirstOrDefault();

				var isFollowing = (from uf in _userFollowers.Table
								  join u in _userRepository.Table on uf.UserId equals u.Id
								  where uf.FollowToUserId == FromUserId && uf.UserId == ToUserId
								  select u).FirstOrDefault();

				var user = await _userRepository.Table.Where(x => x.Id == FromUserId).FirstOrDefaultAsync();

				DateTime createdDate = user.CreatedDate;
				DateTime currentDate = DateTime.Now;

				if (mute.Any(x => x.KeyName == "muteyoudontfollow" && x.Value == "false") && isFollowing != null || 
                    mute.Any(x => x.KeyName == "mutedontfollowyou" && x.Value == "false") && isFollower != null || 
                    mute.Any(x => x.KeyName == "mutenewaccount" && x.Value == "false") && createdDate > currentDate.AddDays(-15) || 
                    mute.Any(x => x.KeyName == "mutedefaultprofilephoto" && x.Value == "false") && (user.ProfileImg == null) || 
                    mute.Any(x => x.KeyName == "muteunconfirmedemail" && x.Value == "false") && (user.IsEmailVerified == null) || 
                    mute.Any(x => x.KeyName == "muteunconfirmedphone" && x.Value == "false") && (user.IsPhoneVerified == null))
				{
                    return false;
				}
            }

			if (check != null) {
                if (check.Value == "true")
                {
                    return true;
                }
                else if (check.Value == "false") 
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<JsonResponse> SaveNotification(NotificationRes Res)
        {
            Notification notification = _mapper.Map<Notification>(Res); 
            await _notificationRepository.InsertAsync(notification);

            if (notification.ActionType == "repost" || notification.ActionType == "inviteattendee" || notification.ActionType == "like" || notification.ActionType == "makespeaker"
                || notification.ActionType == "addattendees" || notification.ActionType == "makehost" || notification.ActionType == "follow")
            {
                UserNotification userNotification = new UserNotification();
                userNotification.NotificationId = notification.Id;
                userNotification.UserId = int.Parse(notification.RefId1);
                userNotification.IsEmail = await GetNotificationPrefrence(int.Parse(notification.RefId1), Res.EmailAttribute, Res.ActionByUserId);
                userNotification.IsPush = await GetNotificationPrefrence(int.Parse(notification.RefId1), Res.PushAttribute, Res.ActionByUserId);
				await _userNotificationRepository.InsertAsync(userNotification);
            }

			//if (notification.ActionType == "follow")
			//{
			//	UserNotification userNotification = new UserNotification();
			//	userNotification.NotificationId = notification.Id;
			//	userNotification.UserId = int.Parse(notification.RefId1);
			//	userNotification.IsEmail = await GetNotificationPrefrence(int.Parse(notification.RefId1), Res.EmailAttribute, Res.ActionByUserId);
			//	userNotification.IsPush = await GetNotificationPrefrence(int.Parse(notification.RefId1), Res.PushAttribute, Res.ActionByUserId);
			//	await _userNotificationRepository.InsertAsync(userNotification);
			//}

			if (notification.ActionType == "eventattend")
            {
                UserNotification userNotification = new UserNotification();
                userNotification.NotificationId = notification.Id;
                userNotification.UserId = int.Parse(notification.RefId1);
				await _userNotificationRepository.InsertAsync(userNotification);

                var eventHost = await _eventspeakersRepository.Table.Where(x => x.Type == "host" && x.EventId == Res.PostId && x.IsDeleted == false).Select(x => x.UserId).ToListAsync();

                foreach (var id in eventHost)
                {
                    UserNotification userNotifications = new UserNotification();
                    userNotifications.NotificationId = notification.Id;
                    userNotifications.UserId = id;
                    await _userNotificationRepository.InsertAsync(userNotifications);
                }
            }
            
            if (notification.ActionType == "comment")
            {
                var parentPost = _userPostRepository.GetById(int.Parse(notification.RefId1));
                UserNotification userNotification = new UserNotification();
                userNotification.NotificationId = notification.Id;
                userNotification.UserId = parentPost.UserId;
				userNotification.IsPush = await GetNotificationPrefrence(parentPost.UserId, Res.PushAttribute, Res.ActionByUserId);
				userNotification.IsEmail = await GetNotificationPrefrence(parentPost.UserId, Res.EmailAttribute, Res.ActionByUserId);
				await _userNotificationRepository.InsertAsync(userNotification);

                UserNotification userNotifications = new UserNotification();
                userNotifications.NotificationId = notification.Id;
                userNotifications.UserId = notification.ActionByUserId;
                userNotifications.IsPush = await GetNotificationPrefrence(parentPost.UserId, Res.PushAttribute, Res.ActionByUserId);
                userNotifications.IsEmail = await GetNotificationPrefrence(parentPost.UserId, Res.EmailAttribute, Res.ActionByUserId);
                await _userNotificationRepository.InsertAsync(userNotifications);
            }


            if (notification.ActionType == "post")
            {
                UserNotification userNotifications = new UserNotification();
                userNotifications.NotificationId = notification.Id;
                userNotifications.UserId = Res.ActionByUserId;
                userNotifications.IsPush = true;
                userNotifications.IsEmail = true;
                await _userNotificationRepository.InsertAsync(userNotifications);

                var followerConnection = (from uf in _userFollowers.Table
                                          join u in _userRepository.Table on uf.UserId equals u.Id
                                          where uf.FollowToUserId == notification.ActionByUserId
                                          select u.Id).ToList();
                foreach (var userId in followerConnection)
                {
                    UserNotification userNotification = new UserNotification();
                    userNotification.NotificationId = notification.Id;
                    userNotification.UserId = userId;
					userNotification.IsPush = await GetNotificationPrefrence(userId, Res.PushAttribute, Res.ActionByUserId);
					userNotification.IsEmail = await GetNotificationPrefrence(userId, Res.EmailAttribute, Res.ActionByUserId);
					await _userNotificationRepository.InsertAsync(userNotification);
                }

                var followToconnection = (from uf in _userFollowers.Table
                                          join u in _userRepository.Table on uf.UserId equals u.Id
                                          join ou in _onlineUserRepository.Table on uf.UserId equals ou.UserId
                                          where uf.FollowToUserId == notification.ActionByUserId
                                          select ou.ConnectionId).ToList();

                var userconnenction = _onlineUserRepository.Table.Where(x => x.UserId == notification.ActionByUserId).FirstOrDefault();
                if (userconnenction != null)
                {
                    followToconnection.Add(userconnenction.ConnectionId);
                }

                var postobj = _userPostRepository.Table.Where(x => x.Id == notification.PostId).FirstOrDefault();
                Res.PostId = postobj.Id;
                Res.connectionIds = followToconnection;
                Res.Message = JsonSerializer.Serialize(postobj);
                string strmessage = JsonSerializer.Serialize(Res);

                if (postobj != null)
                {
                    Post PostMessage = JsonSerializer.Deserialize<Post>(postobj.PostMessage);


                    Res.connectionIds = null;
                    Res.Message = "";
                    if (PostMessage.mentions != null)
                    {
                        var MentionList = PostMessage.mentions.Select(x => x.userId).ToList();
                        if (MentionList.Count() > 0)
                        {
                            Notification MentionNotitficaion = _mapper.Map<Notification>(Res);
                            MentionNotitficaion.ActionType = "mention";
                            await _notificationRepository.InsertAsync(MentionNotitficaion);
                            foreach (var id in MentionList)
                            {
                                UserNotification userNotification = new UserNotification();
                                userNotification.NotificationId = MentionNotitficaion.Id;
                                userNotification.UserId = id;
								userNotification.IsPush = await GetNotificationPrefrence(id, "pushmentionyou", Res.ActionByUserId);
								userNotification.IsEmail = await GetNotificationPrefrence(id, "emailmentionyou", Res.ActionByUserId);
								await _userNotificationRepository.InsertAsync(userNotification);
                              //  await SendEmailNotification(userNotification, notification.ActionType);

                            }
                            await NodeNotification(MentionNotitficaion.Id);
                        }
                    }
                    if (PostMessage.tagUser != null)
                    {
                        var tagList = PostMessage.tagUser.Select(x => x.id).ToList();
                        if (tagList.Count() > 0)
                        {
                            Notification tagNotitficaion = _mapper.Map<Notification>(Res);
                            tagNotitficaion.Id = 0;
                            tagNotitficaion.ActionType = "tag";
                            await _notificationRepository.InsertAsync(tagNotitficaion);
                            foreach (var id in tagList)
                            {
                                UserNotification userNotification = new UserNotification();
                                userNotification.NotificationId = tagNotitficaion.Id;
                                userNotification.UserId = id;
								userNotification.IsPush = await GetNotificationPrefrence(id, "pushtagyou", Res.ActionByUserId);
								userNotification.IsEmail = await GetNotificationPrefrence(id, "emailtagyou", Res.ActionByUserId);
								await _userNotificationRepository.InsertAsync(userNotification);
                              //  await SendEmailNotification(userNotification, notification.ActionType);
                            }
                            //await NodeNotification(tagNotitficaion.Id);
                        }
                    }

				}

            }

            if (notification.ActionType == "like" || notification.ActionType == "comment" || notification.ActionType == "post")
            {
                NodeAddPost NodePostId = new NodeAddPost();
                NodePostId.Id = notification.PostId;
                await SendPostToNode(NodePostId);
            }

            await SendNotification(notification);


			//if (notification.ActionType == "newchatmessage" || notification.ActionType == "newgroupmessage" || notification.ActionType == "eventapprovdeny"
			//    || notification.ActionType == "communityReqApproveDeny") 
			//{
			//    string strmessage = JsonSerializer.Serialize(Res);
			//    await SendNotification(Res, strmessage);
			//} else{   
			//    await NodeNotification(notification.Id);
			//}

			return new JsonResponse(200, true, "Saved Success", null);
        }

        public async Task SendEmailNotification(UserNotification usernotification, string ActionType)
        {
            var user = _userRepository.GetById(usernotification.UserId);
            EmailRequest emailRequest = new EmailRequest();
            emailRequest.USERNAME = user.FirstName + "" + user.LastName;

            var notification = _notificationRepository.GetById((int)usernotification.NotificationId);
            var ActionByName = _userRepository.GetById(notification.ActionByUserId).FirstName + "" + _userRepository.GetById(notification.ActionByUserId).LastName;
            emailRequest.CTATEXT = "View Now";
            emailRequest.CTALINK = "https://k4m2a.com";
            emailRequest.SITETITLE = "K4M2A";
            if (ActionType == "repost")
            {
                emailRequest.CONTENT1 = ActionByName + " reposted your Post.";
                emailRequest.Subject = ActionByName + " reposted your Post.";
            }
            if (ActionType == "like")
            {
                emailRequest.CONTENT1 = ActionByName + " liked your Post.";
                emailRequest.Subject = ActionByName + " liked your Post.";
            }
            if (ActionType == "follow")
            {
                emailRequest.CONTENT1 = ActionByName + " follow you.";
                emailRequest.Subject = ActionByName + " follow you.";
            }
            if (ActionType == "comment")
            {
                emailRequest.CONTENT1 = ActionByName + " commented on your Post.";
                emailRequest.Subject = ActionByName + " commented on your Post.";
            }
            if (ActionType == "post")
            {
                emailRequest.CONTENT1 = ActionByName + " posted New Post";
                emailRequest.Subject = ActionByName + " posted New Post";
            }
            if (ActionType == "mention")
            {
                emailRequest.CONTENT1 = ActionByName + " mentioned you.";
                emailRequest.Subject = ActionByName + " mentioned you.";
            }
            if (ActionType == "tag")
            {
                emailRequest.CONTENT1 = ActionByName + " tagged you.";
                emailRequest.Subject = ActionByName + " tagged you.";
            }
            if (ActionType == "newchatmessage")
            {
                emailRequest.CONTENT1 = ActionByName + " messaged you.";
                emailRequest.Subject = ActionByName + " messaged you.";
            }
            if (ActionType == "newgroupmessage")
            {
                emailRequest.CONTENT1 = "You received message from " + ActionByName;
                emailRequest.Subject = " You received message from " + ActionByName;
            }
            //emailRequest.CONTENT2 = "If you have any questions, we're here to help. Just reach out.";
            //emailRequest.CTALINK = GlobalVariables.SiteUrl + "/forgotPassword/" + encryptedotp + "/" + encrypteduserid;
            //emailRequest.CTATEXT = "Click here to reset your password";
            emailRequest.ToEmail = user.Email;
           

            SMTPDetails smtpDetails = new SMTPDetails();
            smtpDetails.Username = GlobalVariables.SMTPUsername;
            smtpDetails.Host = GlobalVariables.SMTPHost;
            smtpDetails.Password = GlobalVariables.SMTPPassword;
            smtpDetails.Port = GlobalVariables.SMTPPort;
            smtpDetails.SSLEnable = GlobalVariables.SSLEnable;
           var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);
        }


        //public async Task SendNotification(NotificationRes request, string strmessage)
        //{
        //    //if(request.ActionType == "post")
        //    //{
        //    //    foreach (var connectionId in request.connectionIds)
        //    //    {
        //    //        await _notificationHub.Clients.Client(connectionId).OnNewPost(strmessage);
        //    //    }
        //    //}
        //    //else if (request.ActionType == "newchatmessage" || request.ActionType == "newgroupmessage")
        //    //{
        //    //    foreach (var connectionId in request.connectionIds)
        //    //    {
        //    //        await _notificationHub.Clients.Client(connectionId).SendChatMessage(strmessage);
        //    //    }
        //    //    if (request.connectionIds.Count() == 0)
        //    //    {
        //    //        await _notificationHub.Clients.All.SendChatMessage(strmessage);
        //    //    }
        //    //}

        //    var options = new RestClientOptions(GlobalVariables.NotificationAPIUrl)
        //    {
        //        MaxTimeout = -1,
        //    };
        //    var client = new RestClient(options);
        //    var requests = new RestRequest("/notification/newmessage", Method.Post);
        //    requests.AddHeader("Content-Type", "application/json");
        //    requests.AddHeader("Authorization", GlobalVariables.Token);

        //    var body = JsonSerializer.Serialize(request);
        //    requests.AddStringBody(body, DataFormat.Json);
        //    RestResponse response = await client.ExecuteAsync(requests);
        //    Console.WriteLine(response.Content);
        //}

		public async Task SendNotification(Notification request)
		{
			var options = new RestClientOptions(GlobalVariables.NotificationAPIUrl)
			{
				MaxTimeout = -1,
			};
			var client = new RestClient(options);
			var requests = new RestRequest("/notification/send", Method.Post);
			requests.AddHeader("Content-Type", "application/json");
			//requests.AddHeader("Authorization", GlobalVariables.Token);

			var body = JsonSerializer.Serialize(request);
			requests.AddStringBody(body, DataFormat.Json);
			RestResponse response = await client.ExecuteAsync(requests);
			Console.WriteLine(response.Content);
		}

        public async Task SendPostToNode(NodeAddPost request)
        {
            var options = new RestClientOptions(GlobalVariables.ElasticPostNodeUrl)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var requests = new RestRequest("/elastic/addpost", Method.Post);
            requests.AddHeader("Content-Type", "application/json");
            //requests.AddHeader("Authorization", GlobalVariables.Token);

            var body = JsonSerializer.Serialize(request);
            requests.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(requests);
            Console.WriteLine(response.Content);
        }

        public async Task<JsonResponse> UserNotification(int userId, int PageNo, int Size)
        {
            var allNotification = from UN in _userNotificationRepository.Table
                                  join N in _notificationRepository.Table on UN.NotificationId equals N.Id into noti
                                  from N in noti.DefaultIfEmpty()
                                  join UD in _userRepository.Table on N.ActionByUserId equals UD.Id into user
                                  from UD in user.DefaultIfEmpty()
                                  join UP in _userPostRepository.Table on N.PostId equals UP.Id into userPost
                                  from UP in userPost.DefaultIfEmpty()
                                  join RUP in _userPostRepository.Table on UP.ParentId equals RUP.Id into reUserPost
                                  from RUP in reUserPost.DefaultIfEmpty()
                                  join PUD in _userRepository.Table on RUP.UserId equals PUD.Id into userPostDetails
                                  from PUD in userPostDetails.DefaultIfEmpty()
                                  where UN.UserId == userId && UN.IsDeleted == false && N.ActionByUserId != userId
                                  orderby UN.CreatedDate descending
                                  select new userNotificationRes
                                  {
                                      Id = UN.Id,
                                      UserDetail = _mapper.Map<UserDetails>(UD),
                                      Type = N.ActionType,
                                      PostId = N.PostId,
                                      ParentPostId = RUP.Id,
                                      RepostUserDetail = _mapper.Map<UserDetails?>(PUD),
                                      CreatedDate = N.CreatedDate, 
                                      IsRead = UN.IsRead,
                                      Message =N.Message
                                  };

			var chatHistory = await allNotification.Take(Size).Skip((PageNo - 1) * Size).ToListAsync();

            //string sql = @"
            //     UPDATE ""dbo"".""UserNotification""
            //     SET ""IsRead"" = true
            //     WHERE ""UserId"" = {0}
            //     AND ""IsRead"" = false";

            //_context.Database.ExecuteSqlRaw(sql, userId);

            return new JsonResponse(200, true, " Success", chatHistory);
        }

        public async Task<JsonResponse> DeleteUndoNotification(string type, int userId,int Id)
        {
            try
            {
                var data = await _userNotificationRepository.Table.Where(x=> x.UserId == userId &&  x.Id == Id).FirstOrDefaultAsync();
                if(type == "delete" && !data.IsDeleted)
                {
                    await _userNotificationRepository.DeleteAsync(data);
                    return new JsonResponse(200, true, "Notification Deleted Success", null);
                }
                else if(type == "undo" && data.IsDeleted)
                {
                    data.IsDeleted = false;
                    await _userNotificationRepository.UpdateAsync(data);
                    return new JsonResponse(200, true, "Notification Undo Success", null);
                }

                return new JsonResponse(200, true, " Notification not Found", null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> ReadNotification(string type, int userId, int Id)
        {
            try
            {
                if (type == "markallread")
                {
                    string sql = @"
                         UPDATE ""dbo"".""UserNotification""
                         SET ""IsRead"" = true
                         WHERE ""UserId"" = {0}
                         AND ""IsRead"" = false";

                    _context.Database.ExecuteSqlRaw(sql, userId);

                    return new JsonResponse(200, true, "Mark As All Read Success", null);
                }

                var data = await _userNotificationRepository.Table.Where(x => x.UserId == userId && x.Id == Id).FirstOrDefaultAsync();

                if (data != null)
                {
                    data.IsRead = true;
                    await _userNotificationRepository.UpdateAsync(data);
                    return new JsonResponse(200, true, "Notification Read Success", null);
                }

                return new JsonResponse(200, true, " Notification not Found", null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetAllNotificationCount(int User)
        {
            DateTime todate = DateTime.Today.Date;

            var chat = await _chatRepository.Table.Where(x => x.ReceiverId == User && x.IsDeleted == false &&
                                            x.DeleteForUserId1 != User && x.DeleteForUserId2 != User &&
                                            x.GroupId == 0 && x.IsRead == false).CountAsync();

            var notify = await _userNotificationRepository.Table.Where(x=> x.UserId == User && x.IsDeleted == false
                                            && x.IsRead == false).CountAsync();

            var eventReq = (from es in _eventspeakersRepository.Table.Where(x=> x.Type == "host" && x.UserId == User)
                           join e in _eventRepository.Table.Where(x=> x.IsDeleted == false && x.StartDate >= todate) on es.EventId equals e.Id
                           join ea in _eventAttendeeRepository.Table on e.Id equals ea.EventId
                           where ea.EventId == e.Id && ea.IsDeleted == false && ea.Isjoin == true
                           select ea).Count();

            var CommunityReq = (from cm in _communityMemberRepository.Table.Where(x => x.IsDeleted == false && x.UserId == User && x.IsModerator == true)
                                join c in _communityRepository.Table.Where(x => x.IsDeleted == false) on cm.CommunityId equals c.Id
                                join rcm in _communityMemberRepository.Table.Where(x => x.UserId != User && x.IsDeleted == false && x.Isjoin == true) on c.Id equals rcm.CommunityId
                                select rcm).Count();
            
            var ComReportPost = (from cm in _communityMemberRepository.Table.Where(x => x.IsDeleted == false && x.UserId == User && x.IsModerator == true)
                                 join c in _communityRepository.Table.Where(x => x.IsDeleted == false) on cm.CommunityId equals c.Id
                                 join rp in _communityReportRepository.Table.Where(x=> x.IsDeleted == false) on c.Id equals rp.CommunityId
                                 select rp).Count();

            NotificationCountRes Ncount = new NotificationCountRes();
            Ncount.Notification = notify;
            Ncount.EventRequest = eventReq;
            Ncount.Message = chat;
            Ncount.ReportCommunityPost = ComReportPost;
            Ncount.CommunityRequest = CommunityReq;

            return new JsonResponse(200, true, " Success", Ncount);

        }
    }
}
