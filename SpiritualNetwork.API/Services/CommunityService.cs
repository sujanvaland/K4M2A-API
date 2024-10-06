using AutoMapper;
using AutoMapper.Execution;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace SpiritualNetwork.API.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly IRepository<Community> _communityRepository;
        private readonly IRepository<CommunityMember> _communityMemberRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<CommunityRules> _rulesRepository;
        private readonly IRepository<CommunityReportPost> _communityReportRepository;
        private readonly IProfileService _profileService;
        private readonly IPostService _postService;
        private readonly INotificationService _notificationService;
        private readonly IRepository<OnlineUsers> _onlineUsers;


        public CommunityService(IRepository<Community> communityRepository, 
            IRepository<CommunityMember> communityMemberRepository,
            IRepository<User> userRepository,
            IProfileService profileService,
            IRepository<CommunityRules> rulesRepository,
            IRepository<CommunityReportPost> communityReportRepository,
            IPostService postService,
            INotificationService notificationService,
            IRepository<OnlineUsers> onlineUsers)
        {
            _communityRepository = communityRepository;
            _communityMemberRepository = communityMemberRepository;
            _userRepository = userRepository;
            _profileService = profileService;
            _rulesRepository = rulesRepository;
            _communityReportRepository = communityReportRepository;
            _postService = postService;
            _notificationService = notificationService;
            _onlineUsers = onlineUsers;
        }

        public async Task<Community> AddUpdateCommunity (Community req)
        {
            if (req.Id == 0)
            {
                await _communityRepository.InsertAsync(req);
              
                CommunityMember member = new CommunityMember();
                member.CommunityId = req.Id;
                member.UserId = req.CreatedBy;
                member.IsModerator = true;
                await _communityMemberRepository.InsertAsync(member);

                string[] Rules = { "Be kind and respectful.", "Keep posts on topic.", "Explore and share."};
                
                foreach (string Rule in Rules)
                {
                    CommunityRules newRule = new CommunityRules();
                    newRule.CommunityId = req.Id;
                    newRule.CreatedBy = req.CreatedBy;
                    newRule.Rule = Rule;
                    newRule.Descriptions = "";
                    await _rulesRepository.InsertAsync(newRule);
                }
                return req;
            }
            else
            {
                await _communityRepository.UpdateAsync(req);
                return req;
            }
        }
        public async Task<JsonResponse> ReportCommunityPost(CommunityReportPostReq req, int UserId)
        {
            var check = await _communityReportRepository.Table.Where(x => x.UserId == UserId
                            && x.CommunityId == req.CommunityId && x.PostId == req.PostId
                            && x.Type == req.Type).FirstOrDefaultAsync();
            if (check != null)
            {
                check.ReportDetail = req.ReportDetails;
                check.IsDeleted = false;
                await _communityReportRepository.UpdateAsync(check);
            }
            else
            {
                CommunityReportPost Report = new CommunityReportPost();
                Report.ReportDetail = req.ReportDetails;
                Report.UserId = UserId;
                Report.PostId = req.PostId;
                Report.Type = req.Type;
                Report.CommunityId = req.CommunityId;
                await _communityReportRepository.InsertAsync(Report);
            }
            
            return new JsonResponse(200, true, "Success", null);
        }
        public async Task<JsonResponse> ActionCommunityReportPost(CommunityReportPostAction req, int UserId)
        {
            var Admin = await _communityMemberRepository.Table.Where(x => x.UserId == UserId
                 && x.CommunityId == req.CommunityId && x.IsModerator
                 && x.IsDeleted == false).FirstOrDefaultAsync();
            
            if (Admin == null)
            {
                return new JsonResponse(200, true, "Only Admin Can Take Action", null);
            }

            var check = await _communityReportRepository.Table.Where(x => x.CommunityId ==
                        req.CommunityId && req.PostId == req.PostId && x.Type == "post").ToListAsync();
            if (check != null)
            {
                 _communityReportRepository.DeleteHardRange(check);
             
                if(req.ActionType == "delete")
                {
                   await _postService.DeletePostAsync(req.PostId);
                }
            }

            return new JsonResponse(200, true, "Success", null);
        }
        public async Task<JsonResponse> GetAllCommunityReportPost(int CommunityId, int UserId)
        {
            var Admin = await _communityMemberRepository.Table.Where(x => x.UserId == UserId
                            && x.CommunityId == CommunityId && x.IsModerator
                            && x.IsDeleted == false).FirstOrDefaultAsync();

            if (Admin == null)
            {
                return new JsonResponse(200, true, "Only Admin Can Take Action", null);
            }

            var Report = await _communityReportRepository.Table.Where(x => x.IsDeleted == false
                                && x.CommunityId == CommunityId && x.Type == "post").ToListAsync();
            
            var query =  (from u in Report
                               select new CommunityReportPostModel
                               {
                                   CommunityId = u.CommunityId,
                                   PostId = u.PostId,
                                   Type = u.Type,
                               }).Distinct();

            List<CommunityReportPostModel> result = new List<CommunityReportPostModel>();

            foreach(var post in query)
            { 
                var member = Report.Where(x=> x.PostId == post.PostId).Select(x => x.UserId).ToList();

                post.Members = await (from m in _communityReportRepository.Table
                                      join u in _userRepository.Table on m.UserId equals u.Id
                                      where member.Contains(u.Id)
                                      && m.PostId == post.PostId && m.IsDeleted == false
                                      select new CommunityMemberModels
                                      {
                                          UserId = u.Id,
                                          FirstName = u.FirstName,
                                          LastName = u .LastName,
                                          UserName = u.UserName,
                                          Answer = m.ReportDetail,
                                          ProfileImgUrl = u.ProfileImg,
                                          IsBusinessAccount = u.IsBusinessAccount,
                                      }).ToListAsync();
                result.Add(post);
            }

            return new JsonResponse(200, true, "Success", result);
        }

        public async Task<JsonResponse> AddUpdateRules (CommunityRules req, int UserId)
        {
            var Admin = await _communityMemberRepository.Table.Where(x => x.UserId == UserId
                  && x.CommunityId == req.CommunityId && x.IsModerator
                  && x.IsDeleted == false).FirstOrDefaultAsync();
           
            if (Admin == null)
            {
                return new JsonResponse(200, true, "Only Admin Can Take Action", null);
            }

            var limit = await _rulesRepository.Table.Where(x => x.CommunityId == req.CommunityId && x.IsDeleted == false).CountAsync();

            if (limit >= 10)
            {
                return new JsonResponse(200, true, "Community can have up to 10 rules Only", null);
            }

            

            if (req.Id == 0)
            {
                 await _rulesRepository.InsertAsync(req);
            }
            else
            {
                await _rulesRepository.UpdateAsync(req);
            }

            return new JsonResponse(200, true, "Success", null);
        }
        public async Task<JsonResponse> DeleteRules(DeleteRules req, int UserId)
        {
            var Admin = await _communityMemberRepository.Table.Where(x => x.UserId == UserId
                  && x.CommunityId == req.CommunityId && x.IsModerator
                  && x.IsDeleted == false).FirstOrDefaultAsync();
           
            var check = await _rulesRepository.Table.Where(x => x.Id == req.Id && x.CommunityId == req.CommunityId
                       && x.IsDeleted == false).FirstOrDefaultAsync();
           
            if (Admin == null)
            {
                return new JsonResponse(200, true, "Only Admin Can Take Action", null);
            }

             _rulesRepository.DeleteHard(check);

            return new JsonResponse(200, true, "Success", null);
        }
        public async Task<JsonResponse> AddUpdateCommunityMember (AddRevomeMember req, int UserId)
        {
            
            var Admin = await _communityMemberRepository.Table.Where(x => x.UserId == UserId
                              && x.CommunityId == req.CommunityId && x.IsModerator 
                              && x.IsDeleted == false).FirstOrDefaultAsync();
            
            var community = await _communityRepository.Table.Where(x => x.Id == req.CommunityId && x.IsDeleted == false).FirstOrDefaultAsync();

            if (Admin == null)
            {
                return new JsonResponse(200, true, "Only Admin Can Take Action", null);
            }
            if (req.Action == "add")
            {
                foreach (var id in req.UserId)
                {
                    var ever = await _communityMemberRepository.Table.Where(x => x.UserId == id
                             && x.CommunityId == req.CommunityId).FirstOrDefaultAsync();

                    if (ever?.IsDeleted == false)
                    {
                        continue;
                    }

                    if (ever == null)
                    {
                        CommunityMember member = new CommunityMember();
                        member.CommunityId = req.CommunityId;
                        member.UserId = id;
                        member.Isjoin = false;
                        member.IsModerator = false;
                        await _communityMemberRepository.InsertAsync(member);
                    }
                    else
                    {
                        ever.IsModerator = false;
                        ever.IsDeleted = false;
                        ever.Isjoin = false;
                        await _communityMemberRepository.UpdateAsync(ever);
                    }
                }
                return new JsonResponse(200, true, "Success", null);
            }

            if (req.Action == "invite")
            {
                foreach (var id in req.UserId)
                {
                    var ever = await _communityMemberRepository.Table.Where(x => x.UserId == id
                             && x.CommunityId == req.CommunityId).FirstOrDefaultAsync();

                    if (ever?.IsInvited == true)
                    {
                        continue;
                    }

                    if (ever == null)
                    {
                        CommunityMember member = new CommunityMember();
                        member.CommunityId = req.CommunityId;
                        member.UserId = id;
                        member.IsInvited = true;
                        member.IsDeleted = true;
                        await _communityMemberRepository.InsertAsync(member);
                    }
                    else
                    {
                        ever.Isjoin = false;
                        ever.IsInvited = true;
                        await _communityMemberRepository.UpdateAsync(ever);
                    }
                }
                return new JsonResponse(200, true, "Success", null);
            }
            var check = await _communityMemberRepository.Table.Where(x => x.UserId == req.UserId[0]
                              && x.CommunityId == req.CommunityId && x.Isjoin == false).FirstOrDefaultAsync();

            if (req.Action == "makeadminremove" && check != null)
            {
                if (check.IsModerator)
                {
                    check.IsModerator = false;
                }
                else
                {
                    check.IsModerator = true;
                }
                await _communityMemberRepository.UpdateAsync(check);
            }
            if (req.Action == "remove" && check != null)
            {
                check.IsDeleted = true;
                check.Answer = "";
                check.IsInvited = false;
                await _communityMemberRepository.UpdateAsync(check);
            }

            var isReq = await _communityMemberRepository.Table.Where(x => x.UserId == req.UserId[0]
                 && x.CommunityId == req.CommunityId && x.Isjoin == true && x.IsDeleted == false).FirstOrDefaultAsync();

            if (req.Action == "approve" && isReq != null)
            {
                isReq.Isjoin = false;
                await _communityMemberRepository.UpdateAsync(isReq);
            }

            if (req.Action == "deny" && isReq != null)
            {
                isReq.Answer = "";
                isReq.IsDeleted = true;
                isReq.Isjoin = false;
                await _communityMemberRepository.UpdateAsync(isReq);
            }
            return new JsonResponse(200, true, "Success", null);
        }
        public async Task<JsonResponse> JoinLeaveCommunity(JoinLeaveReq req)
        {
            var check = await _communityMemberRepository.Table.Where(x => x.UserId == req.UserId
                              && x.CommunityId == req.CommunityId && x.IsDeleted == false && x.Isjoin == false).FirstOrDefaultAsync();

            var community = await _communityRepository.Table.Where(x=> x.Id == req.CommunityId && x.IsDeleted == false).FirstOrDefaultAsync();

            if (community == null)
            {
                return new JsonResponse(200, true, "Community Not Found", null);
            }

            if (check != null)
            {
                check.Answer = "";
                check.IsModerator = false;
                check.IsDeleted = true;
                check.Isjoin = false;
                await _communityMemberRepository.UpdateAsync(check);
            }
            else
            {
                var ever = await _communityMemberRepository.Table.Where(x => x.UserId == req.UserId
                             && x.CommunityId == req.CommunityId).FirstOrDefaultAsync();

                if (ever == null)
                {
                    CommunityMember member = new CommunityMember();
                    member.CommunityId = req.CommunityId;
                    member.UserId = req.UserId;
                    member.Answer = req.Answer;
                    member.IsModerator = false;
                    member.Isjoin = community.Type == "restricted" ? true : false;
                    await _communityMemberRepository.InsertAsync(member);
                }
                else
                {
                    ever.Answer = req.Answer;
                    ever.IsModerator = false;
                    ever.IsDeleted = false;
                    ever.Isjoin = ever.IsInvited ? false : community.Type == "restricted" ? true : false;
                    await _communityMemberRepository.UpdateAsync(ever);
                }

                return new JsonResponse(200, true, "Success", null);
            }

            var Admin = await _communityMemberRepository.Table.Where(x => x.CommunityId == req.CommunityId 
                              && x.IsModerator && x.IsDeleted == false).CountAsync();

            if (Admin == 0)
            {
                var makeAdmin = await _communityMemberRepository.Table.Where(x => x.CommunityId == req.CommunityId
                        && x.IsDeleted == false).FirstOrDefaultAsync();

                if (makeAdmin != null)
                {
                    makeAdmin.IsModerator = true;
                    await _communityMemberRepository.UpdateAsync(makeAdmin);
                }
            }

            return new JsonResponse(200, true, "Success", null);
        }
        public async Task<JsonResponse> ApproveDenyCommunityMember(AddRevomeMember req, int UserId)
        {

            var Admin = await _communityMemberRepository.Table.Where(x => x.UserId == UserId
                              && x.CommunityId == req.CommunityId && x.IsModerator
                              && x.IsDeleted == false).FirstOrDefaultAsync();

            var check = await _communityMemberRepository.Table.Where(x => x.UserId == req.UserId[0]
                  && x.CommunityId == req.CommunityId && x.Isjoin == true && x.IsDeleted == false).FirstOrDefaultAsync();

            var community = await _communityRepository.Table.Where(x => x.Id == req.CommunityId && x.IsDeleted == false).FirstOrDefaultAsync();

            if (Admin == null && community.CreatedBy != UserId)
            {
                return new JsonResponse(200, true, "Only Admin Can Take Action", null);
            }

            if(check != null)
            {
                if (req.Action == "approve")
                {
                    check.Isjoin = false;
                    await _communityMemberRepository.UpdateAsync(check);
                }
                else
                {
                    check.IsDeleted = true;
                    check.Isjoin = true;
                    await _communityMemberRepository.UpdateAsync(check);
                }

                var receiverConnectionId = await _onlineUsers.Table
                   .Where(x => x.IsDeleted == false && x.UserId == req.UserId[0])
                   .FirstOrDefaultAsync();

                NotificationRes notification = new NotificationRes();
                notification.PostId = req.CommunityId;
                notification.ActionByUserId = UserId;
                notification.RefId1 = req.UserId[0].ToString();
                notification.RefId2 = req.Action;
                notification.Message = "Your Request to Join " + community.Name + " Community is " + (req.Action == "approve" ? "Approved" : "Deny");
                notification.connectionIds.Add(receiverConnectionId.ConnectionId ?? " ");
                notification.ActionType = "communityReqApprove";
				notification.PushAttribute = " ";
				notification.EmailAttribute = " ";
				await _notificationService.SaveNotification(notification);
            }

            return new JsonResponse(200, true, "Success", null);
        }
        public async Task<JsonResponse> MyCommunityList(int UserId)
        {
            try
            {

				var list = await (from cm in _communityMemberRepository.Table.Where(x => x.UserId == UserId && !x.IsDeleted && !x.Isjoin)
								  join c in _communityRepository.Table on cm.CommunityId equals c.Id
								  join crp in _communityReportRepository.Table on c.Id equals crp.CommunityId into crpGroup
								  from crp in crpGroup.DefaultIfEmpty()
								  join cmr in _communityMemberRepository.Table on c.Id equals cmr.CommunityId into cmrGroup
								  from cmr in cmrGroup.DefaultIfEmpty()
								  where !c.IsDeleted && cmr.Isjoin && cmr.IsDeleted
								  select new
								  {
									  Community = c,
									  CommunityMember = cm,
									  CrpGroup = crpGroup,
									  CmrGroup = cmrGroup
								  }).ToListAsync();

				// Process the result on the client side
				var result = list.Select(x => new CommunityModels
				{
					Id = x.Community.Id,
					Name = x.Community.Name,
					Purpose = x.Community.Purpose,
					BackgroundImgUrl = x.Community.BackgroundImgUrl,
					ParentId = x.Community.ParentId,
					ProfileImgUrl = x.Community.ProfileImgUrl,
					Question = x.Community.Question,
					Type = x.Community.Type,
					IsJoined = true,
					IsPending = false,
					CreatedDate = x.Community.CreatedDate,
					IsModerator = x.CommunityMember.IsModerator,
					IsNotification = x.CommunityMember.IsModerator && (x.CrpGroup.Count() > 0 || x.CmrGroup.Count() > 0)
				}).ToList();

				return new JsonResponse(200, true, "Success", list);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex);
            }
        }
        public async Task<JsonResponse> GetCommunityById(int CommunityId, int UserId)
        {
            try
            {
                var community = await (from c in _communityRepository.Table.Where(x=> x.Id == CommunityId 
                                       && x.IsDeleted == false)
                                       join u in _userRepository.Table on c.CreatedBy equals u.Id
                                       select new CommunityModels
                                       {
                                           Id = c.Id,
                                           Name = c.Name,
                                           Purpose = c.Purpose,
                                           BackgroundImgUrl = c.BackgroundImgUrl,
                                           ParentId = c.ParentId,
                                           ProfileImgUrl = c.ProfileImgUrl,
                                           Question = c.Question,
                                           Type = c.Type,
                                           CreatedBy = u.UserName,
                                           CreatedById = u.Id,
                                           CreatedDate = c.CreatedDate
                                       }).FirstOrDefaultAsync();

                var Members = await _communityMemberRepository.Table.Where(x => x.IsDeleted == false
                                                && x.CommunityId == CommunityId).ToListAsync();

                community.TotalMembers = Members.Where(x=> x.Isjoin == false).Count();
                community.IsModerator = Members.Where(x=> x.IsModerator && x.Isjoin == false && x.UserId == UserId).Count() > 0;
                community.IsJoined = Members.Where(x => x.UserId == UserId && x.Isjoin == false).Count() > 0;
                community.IsPending = Members.Where(x => x.UserId == UserId && x.Isjoin == true).Count() > 0;

                if (Members.Where(x => x.IsModerator && x.Isjoin == false && x.UserId == UserId).Count() > 0)
                {
                    community.MemberReqCount = Members.Where(x => x.Isjoin == true).Count();
                    community.ReportPostCount = _communityReportRepository.Table.Where(x => x.CommunityId == CommunityId && x.IsDeleted == false).Count();
                }
                
                var mmbList = Members.Where(x => x.Isjoin == false).Take(8);
                List<CommunityMemberModels> memberlist = new List<CommunityMemberModels>();
                    foreach (var member in mmbList)
                    {
                        var user = await _profileService.GetUserInfoBoxByUserId(member.UserId, UserId);
                        CommunityMemberModels infoBox = new CommunityMemberModels();
                        infoBox.Id = user.Id;
                        infoBox.UserId = user.Id;
                        infoBox.FirstName = user.FirstName;
                        infoBox.LastName = user.LastName;
                        infoBox.UserName = user.UserName;
                        infoBox.ProfileImgUrl = user.ProfileImg;
                        infoBox.IsPremium = user.IsPremium;
                        infoBox.IsModerator = member.IsModerator;
                        infoBox.IsFollowing = Convert.ToBoolean(user.IsFollowedByLoginUser);
                        infoBox.IsFollower = Convert.ToBoolean(user.IsFollowingLoginUser);
                        infoBox.IsBusinessAccount =user.IsBusinessAccount;
                        memberlist.Add(infoBox);
                    }
                community.MemberList = memberlist;

                var mdList = Members.Where(x => x.IsModerator && x.Isjoin == false).Take(8);
                List<CommunityMemberModels> modlist = new List<CommunityMemberModels>();
                    foreach (var member in mdList)
                    {
                        var user = await _profileService.GetUserInfoBoxByUserId(member.UserId, UserId);
                        CommunityMemberModels infoBox = new CommunityMemberModels();
                        infoBox.Id = user.Id;
                        infoBox.UserId = user.Id;
                        infoBox.FirstName = user.FirstName;
                        infoBox.LastName = user.LastName;
                        infoBox.UserName = user.UserName;
                        infoBox.ProfileImgUrl = user.ProfileImg;
                        infoBox.IsPremium = user.IsPremium;
                        infoBox.IsModerator = member.IsModerator;
                        infoBox.IsFollowing = Convert.ToBoolean(user.IsFollowedByLoginUser);
                        infoBox.IsFollower = Convert.ToBoolean(user.IsFollowingLoginUser);
                        infoBox.IsBusinessAccount = user.IsBusinessAccount;

                    modlist.Add(infoBox);
                    }
                community.Moderators = modlist;
               
                List<CommunityRulesModel> rulesModels = new List<CommunityRulesModel>();
               
                var ruleQuery = await _rulesRepository.Table.Where(x => x.IsDeleted == false
                                    && x.CommunityId == CommunityId).ToListAsync();
        
                foreach (var rule in ruleQuery)
                {
                    CommunityRulesModel ruleBox = new CommunityRulesModel();
                    ruleBox.Id = rule.Id;
                    ruleBox.Rules = rule.Rule;
                    ruleBox.Descriptions = rule.Descriptions;
                    rulesModels.Add(ruleBox);
                }
                community.Rules = rulesModels;
               
                return new JsonResponse(200, true, "Success", community);

            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex);

            }
        }
        public async Task<JsonResponse> ExploreCommunityList(ExploreCommunityReq req,int Size, int UserId)
        {
            try
            {
                if(req.Search.Trim().Length > 0 )
                {
                    var Searchlist = await (from c in _communityRepository.Table.Where(x => x.IsDeleted == false)
                                      where c.Name.ToLower().Contains(req.Search.ToLower()) || c.Purpose.ToLower().Contains(req.Search.ToLower())
                                      select new CommunityModels
                                      {
                                          Id = c.Id,
                                          Name = c.Name,
                                          Purpose = c.Purpose,
                                          BackgroundImgUrl = c.BackgroundImgUrl,
                                          ParentId = c.ParentId,
                                          ProfileImgUrl = c.ProfileImgUrl,
                                          Question = c.Question,
                                          Type = c.Type,
                                          IsJoined = false,
                                          IsPending = false,
                                          CreatedDate = c.CreatedDate,
                                      }).Skip((req.PageNo - 1) * Size).Take(Size).ToListAsync();

                    foreach (var count in Searchlist)
                    {
                        count.TotalMembers = await _communityMemberRepository.Table.Where(x => x.IsDeleted == false
                                                    && x.CommunityId == count.Id).CountAsync();
                        count.MemberImgUrl = (from cm in _communityMemberRepository.Table.Where(x => x.IsDeleted == false
                                              && x.Isjoin == false && x.CommunityId == count.Id)
                                              join u in _userRepository.Table on cm.UserId equals u.Id
                                              where u.IsDeleted == false
                                              select new
                                              {
                                                  u.Id,
                                                  u.ProfileImg,
                                              }).Select(x => x.ProfileImg).Take(8).ToList();
                    }

                    return new JsonResponse(200, true, "Success", Searchlist);

                }
                var list = await (from c in _communityRepository.Table.Where(x => x.IsDeleted == false)
                                  where !(_communityMemberRepository.Table.Any(cm => cm.CommunityId == c.Id && cm.IsDeleted == false && cm.UserId == UserId && cm.Isjoin == false))
                                  select new CommunityModels
                                  {
                                      Id = c.Id,
                                      Name = c.Name,
                                      Purpose = c.Purpose,
                                      BackgroundImgUrl = c.BackgroundImgUrl,
                                      ParentId = c.ParentId,
                                      ProfileImgUrl = c.ProfileImgUrl,
                                      Question = c.Question,
                                      Type = c.Type,
                                      IsJoined = false,
                                      IsPending = false, 
                                      CreatedDate = c.CreatedDate,
                                  }).Distinct().Skip((req.PageNo - 1) * Size).Take(Size).ToListAsync();

                foreach (var count in list)
                {
                    count.TotalMembers = await _communityMemberRepository.Table.Where(x => x.IsDeleted == false     
                                                && x.CommunityId == count.Id).CountAsync();
                    count.MemberImgUrl = (from cm in _communityMemberRepository.Table.Where(x => x.IsDeleted == false
                                          && x.Isjoin == false && x.CommunityId == count.Id)
                                         join u in _userRepository.Table on cm.UserId equals u.Id
                                         where u.IsDeleted == false
                                         select new
                                         {
                                             u.Id,
                                             u.ProfileImg,
                                         }).Select(x => x.ProfileImg).Take(8).ToList();
                }

                return new JsonResponse(200, true, "Success", list);

            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex);

            }
        }
        public async Task<JsonResponse> MemberReqList(MemberListReq req)
        {
            try
            {
                var query = await _communityMemberRepository.Table.Where(x => x.IsDeleted == false
                                && x.CommunityId == req.CommunityId && x.Isjoin == true).ToListAsync();

                List<CommunityMemberModels> memberlist = new List<CommunityMemberModels>();

                foreach (var member in query)
                {
                    var user = await _profileService.GetUserInfoBoxByUserId(member.UserId, req.UserId);
                    CommunityMemberModels infoBox = new CommunityMemberModels();
                    infoBox.Id = user.Id;
                    infoBox.UserId = user.Id;
                    infoBox.FirstName = user.FirstName;
                    infoBox.LastName = user.LastName;
                    infoBox.UserName = user.UserName;
                    infoBox.ProfileImgUrl = user.ProfileImg;
                    infoBox.Answer = member.Answer;
                    infoBox.IsPremium = user.IsPremium;
                    infoBox.IsModerator = member.IsModerator;
                    infoBox.IsFollowing = Convert.ToBoolean(user.IsFollowedByLoginUser);
                    infoBox.IsFollower = Convert.ToBoolean(user.IsFollowingLoginUser);
                    infoBox.IsBusinessAccount = user.IsBusinessAccount;

                    memberlist.Add(infoBox);
                }
                return new JsonResponse(200, true, "Success", memberlist);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex);
            }
        }
        public async Task<JsonResponse> AllMemberList(MemberListReq req)
        {
            try
            {
                var query = await _communityMemberRepository.Table.Where(x => x.IsDeleted == false && x.Isjoin == false
                                && x.CommunityId == req.CommunityId).ToListAsync();

                List<CommunityMemberModels> memberlist = new List<CommunityMemberModels>();
                foreach (var member in query)
                {
                    var user = await _profileService.GetUserInfoBoxByUserId(member.UserId, req.UserId);
                    CommunityMemberModels infoBox = new CommunityMemberModels();
                    infoBox.Id = user.Id;
                    infoBox.UserId = user.Id;
                    infoBox.FirstName = user.FirstName;
                    infoBox.LastName = user.LastName;
                    infoBox.UserName = user.UserName;
                    infoBox.ProfileImgUrl = user.ProfileImg;
                    infoBox.IsPremium = user.IsPremium;
                    infoBox.IsModerator = member.IsModerator;
                    infoBox.IsFollowing = Convert.ToBoolean(user.IsFollowedByLoginUser);
                    infoBox.IsFollower = Convert.ToBoolean(user.IsFollowingLoginUser);
                    infoBox.IsBusinessAccount = user.IsBusinessAccount;

                    memberlist.Add(infoBox);
                }

                return new JsonResponse(200, true, "Success", memberlist);

            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex);
            }
        }
        public async Task<JsonResponse> AllModeratorList(MemberListReq req)
        {
            try
            {
                var query = await _communityMemberRepository.Table.Where(x => x.IsDeleted == false && x.Isjoin == false
                                && x.CommunityId == req.CommunityId && x.IsModerator).ToListAsync();

                List<CommunityMemberModels> memberlist = new List<CommunityMemberModels>();

                foreach (var member in query)
                {
                    var user = await _profileService.GetUserInfoBoxByUserId(member.UserId, req.UserId);
                    CommunityMemberModels infoBox = new CommunityMemberModels();
                    infoBox.Id = user.Id;
                    infoBox.UserId = user.Id;
                    infoBox.FirstName = user.FirstName;
                    infoBox.LastName = user.LastName;
                    infoBox.UserName = user.UserName;
                    infoBox.ProfileImgUrl = user.ProfileImg;
                    infoBox.IsPremium = user.IsPremium;
                    infoBox.IsModerator = member.IsModerator;
                    infoBox.IsFollowing = Convert.ToBoolean(user.IsFollowedByLoginUser);
                    infoBox.IsFollower = Convert.ToBoolean(user.IsFollowingLoginUser);
                    infoBox.IsBusinessAccount = user.IsBusinessAccount;

                    memberlist.Add(infoBox);
                }
                return new JsonResponse(200, true, "Success", memberlist);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex);
            }
        }

        public async Task<JsonResponse> DeleteCommunity(int CommunityId, int UserId)
        {
            try
            {
                var check = await _communityRepository.Table.Where(x=> x.Id == CommunityId
                            && x.IsDeleted == false).FirstOrDefaultAsync();    
                
                if(check == null)
                {
                    return new JsonResponse(200, true, "Community Not Found",null);
                }
                if(check.CreatedBy != UserId)
                {
                    return new JsonResponse(200, true, "Only Admin Can Delete This Community",null);

                }

                var member = await _communityMemberRepository.Table.Where(x => x.CommunityId == CommunityId).ToListAsync();
                await _communityRepository.DeleteAsync(check);
                await _communityMemberRepository.DeleteRangeAsync(member);

                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex);
            }
        }

    }
}
