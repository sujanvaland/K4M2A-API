using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using AutoMapper;
using AutoMapper.Execution;
using Azure;
using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.API.Hubs;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SpiritualNetwork.API.Services
{
    public class ChatService : IChatService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ChatMessages> _chatMessagesRepository;
        private readonly IRepository<OnlineUsers> _onlineUsers;
        private readonly IRepository<UserFollowers> _userFollowers;
        private readonly IProfileService _profileService;
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IRepository<Entities.File> _fileRepository;
        private readonly IRepository<MessageGroupDetails> _messageGroupDetailsRepository;
        private readonly IRepository<GroupMember> _groupMemberRepository;
        private readonly IRepository<SnoozeUser> _snoozeUserRepository;
        private readonly IRepository<UserAttribute> _userAttributeRepository;
        private readonly IRepository<Reaction> _reactionRepository;
        private readonly IRepository<UserMuteBlockList> _userMuteBlockListRepository;


        public ChatService(IRepository<ChatMessages> chatMessagesRepository, 
            IRepository<User> userRepository,
            AppDbContext context,
            INotificationService notificationService,
            IRepository<OnlineUsers> onlineUsers,
            IRepository<UserFollowers> userFollowers,
            IProfileService profileService,
            IRepository<Entities.File> fileRepository,
            IRepository<MessageGroupDetails> messageGroupDetailsRepository,
            IRepository<GroupMember> groupMemberRepository,
            IRepository<SnoozeUser> snoozeUserRepository,
            IRepository<UserAttribute> userAttributeRepository,
            IRepository<Reaction> reactionRepository,
            IRepository<UserMuteBlockList> userMuteBlockListRepository)
            {
                _onlineUsers = onlineUsers;
                _chatMessagesRepository = chatMessagesRepository;
                _userRepository = userRepository;
                _context = context;
                _notificationService = notificationService;
                _userFollowers = userFollowers;
                _profileService = profileService;
                _fileRepository = fileRepository;
                _messageGroupDetailsRepository = messageGroupDetailsRepository;
                _groupMemberRepository = groupMemberRepository;
                _snoozeUserRepository = snoozeUserRepository;
                _userAttributeRepository = userAttributeRepository;
                _reactionRepository = reactionRepository;
                _userMuteBlockListRepository = userMuteBlockListRepository;
            }

        public async Task<JsonResponse> GetChatMessages(int LoginUserId, ChatHistoryReq req,int Size)
        {
            if (req.IsGroup == 1)
            {
                var Chatquery = from cm in _chatMessagesRepository.Table
                            join file in _fileRepository.Table on cm.AttachmentId equals file.Id into fileJoin
                            from file in fileJoin.DefaultIfEmpty()
                            join cmr in _chatMessagesRepository.Table on cm.ReplyId equals cmr.Id into reply
                            from cmr in reply.DefaultIfEmpty()
                            join u in _userRepository.Table on cm.SenderId equals u.Id into user
                            from u in user.DefaultIfEmpty()
                            join ru in _userRepository.Table on cmr.SenderId equals ru.Id into replyUser
                            from ru in replyUser.DefaultIfEmpty()
                            where cm.GroupId == req.UserId && cm.IsDeleted == false 
                            select new ChatHistory
                            {
                                SenderId = cm.SenderId,
                                SenderName = u.FirstName,
                                ReceiverId = cm.ReceiverId,
                                Message = cm.Message,
                                ReplyTo = ru.FirstName,
                                ReplyMessage = cmr.Message ,
                                IsDelivered = cm.IsDelivered,
                                IsRead = cm.IsRead,
                                AttachmentId = cm.AttachmentId,
                                DeleteForUserId1 = cm.DeleteForUserId1,
                                DeleteForUserId2 = cm.DeleteForUserId2,
                                ThumbnailUrl = file != null ? file.ThumbnailUrl : null,
                                ActualUrl = file != null ? file.ActualUrl : null,
                                Id = cm.Id,
                                CreatedDate = cm.CreatedDate,
                                CreatedBy = cm.CreatedBy,
                                IsBusinessAccount = u.IsBusinessAccount
                            };
                var countGroupMessage = Chatquery.Count();

                var GroupIndex = Math.Max(0, countGroupMessage - Size * req.PageNo);

                var GroupChat = await Chatquery.Skip(GroupIndex).Take(Size).ToListAsync();

                var GroupDetails = await _messageGroupDetailsRepository.Table.Where(x=> x.Id == req.UserId).FirstOrDefaultAsync();

                var memberList = await _groupMemberRepository.Table.Where(x => x.GroupId == req.UserId
                                    && x.IsDeleted == false).CountAsync();

                ChatProfile GroupProfile = new ChatProfile();
                GroupProfile.Id = req.UserId;
                GroupProfile.Name = GroupDetails.GroupName;
                GroupProfile.UserName = "";
                GroupProfile.About = "";
                GroupProfile.createdDate = GroupDetails.CreatedDate;
                GroupProfile.profileImg = GroupDetails.ProfileImgUrl;
                GroupProfile.IsPremium = false;
                GroupProfile.NoOfFollowers = 0;
                GroupProfile.NoOfFollowing = 0;
                GroupProfile.IsGroup = true;
                GroupProfile.AllowMessageType = "everyone";
                GroupProfile.AllowMessage = true;
                GroupProfile.IsBlock = false;
                GroupProfile.IsBlockByUser = false;
                GroupProfile.MemberCount = memberList;

                ChatHistoryResponse res = new ChatHistoryResponse();
                res.ChatMessages = GroupChat;
                res.UserProfile = GroupProfile;
                res.TotalCount = countGroupMessage;
                res.Size = Size;

                return new JsonResponse(200, true, "Success", res);

            }

            var chatMessage = _chatMessagesRepository.Table.Where(x=> (x.SenderId == LoginUserId || x.ReceiverId == LoginUserId) &&
                                        (x.SenderId == req.UserId || x.ReceiverId == req.UserId) && x.IsDeleted == false &&
                                        x.DeleteForUserId1 != LoginUserId && x.DeleteForUserId2 != LoginUserId);

            var query = from cm in chatMessage
                        join file in _fileRepository.Table on cm.AttachmentId equals file.Id into fileJoin
                        from file in fileJoin.DefaultIfEmpty()
                        join cmr in _chatMessagesRepository.Table on cm.ReplyId equals cmr.Id into reply
                        from cmr in reply.DefaultIfEmpty()
                        join u in _userRepository.Table on cm.SenderId equals u.Id
                        join ru in _userRepository.Table on cmr.SenderId equals ru.Id into replyUser
                        from ru in replyUser.DefaultIfEmpty()
                        //where (cm.SenderId == LoginUserId || cm.ReceiverId == LoginUserId) &&
                        //      (cm.SenderId == req.UserId || cm.ReceiverId == req.UserId) &&
                        //      cm.IsDeleted == false
                        select new ChatHistory
                        {
                            SenderId = cm.SenderId,
                            SenderName = u.FirstName,
                            ReceiverId = cm.ReceiverId,
                            Message = cm.Message,
                            ReplyTo = ru.FirstName,
                            ReplyMessage = cmr.Message,
                            IsDelivered = cm.IsDelivered,
                            IsRead = cm.IsRead,
                            AttachmentId = cm.AttachmentId,
                            DeleteForUserId1 = cm.DeleteForUserId1,
                            DeleteForUserId2 = cm.DeleteForUserId2,
                            ThumbnailUrl = file != null ? file.ThumbnailUrl : null,
                            ActualUrl = file != null ? file.ActualUrl : null,
                            Id = cm.Id,
                            CreatedDate = cm.CreatedDate,
                            CreatedBy = cm.CreatedBy,
                            IsBusinessAccount = u.IsBusinessAccount
                        };

            string sql = @"
                            UPDATE ChatMessages
                            SET IsRead = 1
                            WHERE ReceiverId = {0}
                            AND SenderId = {1}
                            AND IsRead = 0";

            _context.Database.ExecuteSqlRaw(sql, LoginUserId, req.UserId);


            var countMessage = query.Count();
            var startIndex = Math.Max(0, countMessage - Size * req.PageNo);
            var block = await _userMuteBlockListRepository.Table.Where(x => x.UserId == LoginUserId
                        && x.BlockedUserId == req.UserId && x.IsDeleted == false).CountAsync() > 0;
            var blockbyUser = await _userMuteBlockListRepository.Table.Where(x => x.UserId == req.UserId
                        && x.BlockedUserId == LoginUserId && x.IsDeleted == false).CountAsync() > 0;
            var chatHistory = await query.Skip(startIndex).Take(Size).ToListAsync();

            var profile = await _profileService.GetUserProfileById(req.UserId);
            var Loginedprofile = await _profileService.GetUserProfileById(LoginUserId);

            var setting = await _userAttributeRepository.Table.Where(x => x.UserId == req.UserId && x.KeyName == "allowmessagefrom").FirstOrDefaultAsync();

            ChatProfile Chatprofile = new ChatProfile();
            Chatprofile.Id = profile.Id;
            Chatprofile.Name = profile.FirstName +" " + profile.LastName;
            Chatprofile.UserName = profile.UserName;
            Chatprofile.About = profile.About;
            Chatprofile.createdDate = profile.CreatedDate;
            Chatprofile.profileImg = profile.ProfileImg;
            Chatprofile.IsPremium = profile.IsPremium;
            Chatprofile.NoOfFollowers = profile.NoOfFollowers;
            Chatprofile.NoOfFollowing = profile.NoOfFollowing;
            Chatprofile.IsGroup = false;
            Chatprofile.IsBlock = block;
            Chatprofile.IsBlockByUser = blockbyUser;
            Chatprofile.IsBusinessAccount = profile.IsBusinessAccount;

            if (setting != null)
            {
                Chatprofile.AllowMessageType = setting.Value;
                if(setting.Value == "verifieduser")
                {
                    Chatprofile.AllowMessage = Loginedprofile.IsPremium;
                }
                else if(setting.Value == "noone")
                {
                    Chatprofile.AllowMessage = false;
                }
                else
                {
                    Chatprofile.AllowMessage = true;
                }
            }
            else {
                Chatprofile.AllowMessageType = "everyone";
                Chatprofile.AllowMessage = true;
            }


            ChatHistoryResponse response = new ChatHistoryResponse();
            response.ChatMessages = chatHistory;
            response.UserProfile = Chatprofile;
            response.TotalCount = countMessage;
            response.Size = Size;

            return new JsonResponse(200, true, "Success", response);

            //var onlineUser = (from uf in _userFollowers.Table
            //                  join u in _userRepository.Table on uf.FollowToUserId equals u.Id
            //                  join ou in _onlineUsers.Table on uf.FollowToUserId equals ou.UserId
            //                  where uf.UserId == UserId
            //                  select u).ToList();

        }

        public async Task<JsonResponse> GetInfolist(int Id,int loginUserId, bool IsGroup)
        {
            List<GroupInfoBoxModel> groupInfoBoxModels = new List<GroupInfoBoxModel>();
            if (IsGroup)
            {
                var memberList = await _groupMemberRepository.Table.Where(x => x.GroupId == Id
                                    && x.IsDeleted == false).ToListAsync();

                var loginDetail = memberList.Where(x => x.UserId == loginUserId).FirstOrDefault();


                foreach (var member in memberList)
                {
                    if (member.UserId == loginUserId)
                    {
                        continue;
                    }
                    var user = await _profileService.GetUserInfoBoxByUserId(member.UserId, loginUserId);
                    GroupInfoBoxModel infoBox = new GroupInfoBoxModel();
                    infoBox.Id = user.Id;
                    infoBox.FullName = user.FirstName + " " + user.LastName;
                    infoBox.UserName = user.UserName;
                    infoBox.ProfileImgUrl = user.ProfileImg;
                    infoBox.isPremium = user.IsPremium;
                    infoBox.IsAdmin = member.IsAdmin;
                    infoBox.isfollowing = user.IsFollowedByLoginUser;
                    infoBox.isPeer = user.IsFollowingLoginUser;
                    infoBox.IsBusinessAccount = user.IsBusinessAccount;
                    groupInfoBoxModels.Add(infoBox);
                }

                GroupInfoDetails groupResponse = new GroupInfoDetails();
                groupResponse.GroupInfoBoxModel = groupInfoBoxModels;
                groupResponse.IsAdmin = loginDetail?.IsAdmin ?? false;
                groupResponse.IsNotification = loginDetail?.IsNotification == 0 ? false : true;
                groupResponse.IsMention = loginDetail?.IsMention == 0 ? false : true;
                return new JsonResponse(200, true, "Success", groupResponse);
            }

            var query = await _snoozeUserRepository.Table.Where(x => x.UserId == loginUserId
                            && x.SnoozeUserId == Id && x.IsDeleted == false).FirstOrDefaultAsync();

            var User = await _profileService.GetUserInfoBoxByUserId(Id, loginUserId);

            GroupInfoBoxModel InfoBox = new GroupInfoBoxModel();
            InfoBox.Id = User.Id;
            InfoBox.FullName = User.FirstName + " " + User.LastName;
            InfoBox.UserName = User.UserName;
            InfoBox.ProfileImgUrl = User.ProfileImg;
            InfoBox.isPremium = User.IsPremium;
            InfoBox.IsAdmin = false;
            InfoBox.isfollowing = User.IsFollowedByLoginUser;
            InfoBox.isPeer = User.IsFollowingLoginUser;
            InfoBox.IsBusinessAccount =User.IsBusinessAccount;
            groupInfoBoxModels.Add(InfoBox);

            GroupInfoDetails response = new GroupInfoDetails();
            response.GroupInfoBoxModel = groupInfoBoxModels;
            response.IsAdmin = false;
            response.IsNotification = query?.IsNotification == 0 ? false : true;
            response.IsMention = false;
            return new JsonResponse(200, true, "Success", response);

        }

        public async Task<JsonResponse> SnoozeGroupUser(int Id, int loginUserId, bool IsGroup,string type)
        {
            if (IsGroup)
            {
                var memberList = await _groupMemberRepository.Table.Where(x => x.GroupId == Id
                                    && x.IsDeleted == false && x.UserId == loginUserId).FirstOrDefaultAsync();
                if(type == "notification")
                {
                     memberList.IsNotification = memberList.IsNotification == 1 ? 0 : 1;
                }
                else
                {
                    memberList.IsMention = memberList.IsMention == 1 ? 0 : 1;
                }
                memberList.ModifiedDate= DateTime.Now;

                await _groupMemberRepository.UpdateAsync(memberList);
                return new JsonResponse(200, true, "Success", null);
            }

            var query = await _snoozeUserRepository.Table.Where(x=> x.UserId == loginUserId 
                && x.SnoozeUserId == Id && x.IsDeleted == false).FirstOrDefaultAsync();

            if(query == null)
            {
                SnoozeUser user = new SnoozeUser();
                user.UserId = loginUserId;
                user.SnoozeUserId = Id;
                user.IsNotification = 1;

               await _snoozeUserRepository.InsertAsync(user);
            }
            else
            {
                query.IsNotification = query.IsNotification == 1 ? 0 : 1;
                query.ModifiedDate = DateTime.Now;
                await _snoozeUserRepository.UpdateAsync(query);
            }
            return new JsonResponse(200, true, "Success", null);

        }

        public async Task<JsonResponse> MakeAdminRemoveFromGroup(int GroupId , int userId, int LoginUserId, string Type)
        {
            var isAdmin = await _groupMemberRepository.Table.Where(x => x.GroupId == GroupId
                                && x.IsDeleted == false && x.IsAdmin == true 
                                && x.UserId == LoginUserId).FirstOrDefaultAsync();

            var memberList = await _groupMemberRepository.Table.Where(x => x.GroupId == GroupId
                              && x.IsDeleted == false && x.UserId == userId).FirstOrDefaultAsync();

            var actionUser = await _userRepository.Table.Where(x => x.Id == userId && x.IsDeleted == false).FirstOrDefaultAsync();

            ChatMessages newMessage = new ChatMessages();
            newMessage.GroupId = GroupId;
            newMessage.SenderId = 0;
            newMessage.ReceiverId = 0;

              
            if (memberList != null)
            {
                if (isAdmin != null && Type == "makeAdmin")
                { 
                    memberList.IsAdmin = true;
                    await _groupMemberRepository.UpdateAsync(memberList);
                }
                if (isAdmin != null && Type == "removeAdmin")
                {
                    memberList.IsAdmin = false;
                    await _groupMemberRepository.UpdateAsync(memberList);
                }
                if (isAdmin != null && Type == "remove")
                {
                    newMessage.Message = actionUser.FirstName + " " + actionUser.LastName + " leave the group conversation";
                    await SaveChatMessage(newMessage);
                    await _groupMemberRepository.DeleteAsync(memberList);
                }
            }
            return new JsonResponse(200, true, "Success",null);
        }

        public async Task<JsonResponse> GetUserChatList(int UserId)
        {
            SqlParameter userparam = new SqlParameter("@userId", UserId);
            var Result = await _context.UserChatResponse
                .FromSqlRaw("SpGetChatThreads @userId", userparam)
                .ToListAsync();
            return new JsonResponse(200, true, "Success", Result);
        }

        public async Task SaveChatMessage(ChatMessages chatMessages)
        {
            if (chatMessages.Id > 0)
                await _chatMessagesRepository.UpdateAsync(chatMessages);
            else
                await _chatMessagesRepository.InsertAsync(chatMessages);

            List<string> ConnectionList = new List<string>();

            if(chatMessages.GroupId != 0)
            {
                var GroupMember = await _groupMemberRepository.Table.Where(x=> x.GroupId == chatMessages.GroupId).ToListAsync();
                NotificationRes notification = new NotificationRes();
                foreach (var item in GroupMember)
                {
                    var receiverConnectionId = await _onlineUsers.Table.Where(x => x.IsDeleted == false 
                    && x.UserId == item.UserId).FirstOrDefaultAsync();

                    if (receiverConnectionId != null)
                    {
                        notification.connectionIds.Add(receiverConnectionId.ConnectionId);
                    }
                }
                notification.PostId = 0;
                notification.ActionByUserId = chatMessages.SenderId;
                notification.ActionType = "newgroupmessage";
                notification.RefId2 = chatMessages.GroupId.ToString();
                notification.RefId1 = "";
                notification.Message = "";
                await _notificationService.SaveNotification(notification);
            }
            else
            {
                var receiverConnectionId = await _onlineUsers.Table
                .Where(x => x.IsDeleted == false && x.UserId == chatMessages.ReceiverId)
                .FirstOrDefaultAsync();

                if (receiverConnectionId == null)
                    return;

                NotificationRes notification = new NotificationRes();
                notification.PostId = 0;
                notification.ActionByUserId = chatMessages.SenderId;
                notification.ActionType = "newchatmessage";
                notification.RefId1 = chatMessages.ReceiverId.ToString();
                notification.RefId2 = chatMessages.SenderId.ToString();
                notification.Message = "";
                notification.connectionIds.Add(receiverConnectionId.ConnectionId);

                await _notificationService.SaveNotification(notification);
            }
            
        }

        public async Task<JsonResponse> AddCreateMemberGroup(GroupMessageModel res, int UserId)
        {
            if (res.GroupId == 0)
            {
                MessageGroupDetails details = new MessageGroupDetails();
                details.GroupName = res.Name ?? "";
                details.ProfileImgUrl = "";
                await _messageGroupDetailsRepository.InsertAsync(details);

                var newAdmin = new GroupMember
                {
                    GroupId = details.Id,
                    UserId = UserId,
                    JoinDate = DateTime.Now,
                    IsAdmin = true,
                    IsNotification=0,
                    IsMention=0,
                };
                await _groupMemberRepository.InsertAsync(newAdmin);

                foreach (var userId in res.userId)
                {
                    var newMembership = new GroupMember
                    {
                        GroupId = details.Id,
                        UserId = userId,
                        JoinDate = DateTime.Now,
                        IsAdmin = false,
                        IsNotification = 0,
                        IsMention = 0,
                    };
                    await _groupMemberRepository.InsertAsync(newMembership);
                }

                return new JsonResponse(200, true, "Success", details);
            }

            var IsGroup = await _messageGroupDetailsRepository.Table.Where(x => x.Id == res.GroupId 
                            && x.IsDeleted == false).FirstOrDefaultAsync();

            var actionUser = await _userRepository.Table.Where(x => x.Id == UserId && x.IsDeleted == false).FirstOrDefaultAsync();

            ChatMessages newMessage = new ChatMessages();
            newMessage.GroupId = res.GroupId;
            newMessage.SenderId = 0;
            newMessage.ReceiverId = 0;

            if (IsGroup != null)
            {
                foreach (var userId in res.userId)
                {
                    var groupdetails = await _groupMemberRepository.Table.Where(x => x.GroupId == res.GroupId
                                      && x.UserId == userId).FirstOrDefaultAsync();
                    var User = await _userRepository.Table.Where(x => x.Id == userId && x.IsDeleted == false).FirstOrDefaultAsync();

                    newMessage.Message = actionUser.FirstName + " " + actionUser.LastName + " added "+User.FirstName + " " + User.LastName;
                    await SaveChatMessage(newMessage);
                    if (groupdetails == null)
                    {
                        var newMembership = new GroupMember
                        {
                            GroupId = IsGroup.Id,
                            UserId = userId,
                            JoinDate = DateTime.Now,
                            IsAdmin = false,
                        };
                        await _groupMemberRepository.InsertAsync(newMembership);
                    }else
                    {
                        groupdetails.IsDeleted = false;
                        groupdetails.ModifiedDate = DateTime.Now;
                        await _groupMemberRepository.UpdateAsync(groupdetails);
                    }
                }
            }
            return new JsonResponse(200, true, "Success", null);
        }


        public async Task<JsonResponse> LeaveGroupConversation (int GroupId, int UserId)
        {
            var user = await _groupMemberRepository.Table.Where(x => x.GroupId == GroupId
                        && x.IsDeleted == false && x.UserId == UserId).FirstOrDefaultAsync();

            var actionUser = await _userRepository.Table.Where(x => x.Id == UserId && x.IsDeleted == false).FirstOrDefaultAsync();

            ChatMessages newMessage = new ChatMessages();
            newMessage.GroupId = GroupId;
            newMessage.SenderId = 0;
            newMessage.ReceiverId = 0;
            
            if (user != null && actionUser !=null)
            {
                await _groupMemberRepository.DeleteAsync(user);
                newMessage.Message = actionUser.FirstName + " " + actionUser.LastName + " leave the group conversation";
                await SaveChatMessage(newMessage);
            }
           
            var Admin = await _groupMemberRepository.Table.Where(x => x.GroupId == GroupId
                        && x.IsDeleted == false && x.IsAdmin == true).CountAsync();
            
            if (Admin == 0)
            {
                var makeAdmin = await _groupMemberRepository.Table.Where(x => x.GroupId == GroupId
                        && x.IsDeleted == false).FirstOrDefaultAsync();
                if(makeAdmin != null)
                {
                    makeAdmin.IsAdmin = true;
                    await _groupMemberRepository.UpdateAsync(makeAdmin);
                }
            }

            return new JsonResponse(200, true, "Success", null);
        } 

        public async Task<JsonResponse> UpdateGroupDetails(UpdateGroupDetails req, int id)
        {
            var query = await _messageGroupDetailsRepository.Table.Where(x=> x.Id == req.GroupId && x.IsDeleted == false).FirstOrDefaultAsync();
            var user = await _userRepository.Table.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();

            ChatMessages newMessage = new ChatMessages();
            newMessage.GroupId = req.GroupId;
            newMessage.SenderId = 0;
            newMessage.ReceiverId = 0;
            if(query.GroupName != req.GroupName)
            {
                newMessage.Message = user.FirstName+" "+user.LastName+" changed the group name to "+ req.GroupName;
                await SaveChatMessage(newMessage);
            }
            if(query.ProfileImgUrl != req.ProfileImgUrl)
            {
                newMessage.Message = user.FirstName + " " + user.LastName + " changed the group image";
                await SaveChatMessage(newMessage);
            }
            query.GroupName = req.GroupName;
            query.ProfileImgUrl = req.ProfileImgUrl;
            query.ModifiedDate = DateTime.Now;
            await _messageGroupDetailsRepository.UpdateAsync(query);
            return new JsonResponse(200, true, "Success", null);
        }

        public void DeleteChatMessage(ChatMessages chatMessages)
        {
            if (chatMessages.Id > 0)
               _chatMessagesRepository.DeleteHard(chatMessages);
        }

        public async Task<JsonResponse> DeleteChatHistory(int UserId,int LoginUserId,int MessageId)
        {
            var history = _chatMessagesRepository.Table.Where(x => (x.SenderId == LoginUserId
            || x.ReceiverId == LoginUserId) &&
            (x.SenderId == UserId
            || x.ReceiverId == UserId));

            List<ChatMessages> list = new List<ChatMessages>();

            if (MessageId > 0)
            {
                list = await history.Where(x => x.Id == MessageId).ToListAsync();
            }
            else
            {
                list = await  history.Where(x => x.DeleteForUserId1 == 0 || x.DeleteForUserId2 == 0).ToListAsync();
            }
            foreach (var item in list)
            {
                if(item.DeleteForUserId1 == 0 && item.DeleteForUserId1 != LoginUserId)
                {
                    item.DeleteForUserId1 = LoginUserId;
                }
               else if (item.DeleteForUserId2 == 0 && item.DeleteForUserId2 != LoginUserId)
                {
                    item.DeleteForUserId2 = LoginUserId;
                }
            }
            await _chatMessagesRepository.UpdateRangeAsync(list);

            return new JsonResponse(200, true, "Success", null);
        }
    }
}
