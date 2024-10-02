using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities.CommonModel;
using SpiritualNetwork.Entities;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Services.Interface;
using Event = SpiritualNetwork.Entities.Event;
using Community = SpiritualNetwork.Entities.Community;
using static HotChocolate.ErrorCodes;

namespace SpiritualNetwork.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<OnlineUsers> _onlineuserRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Community> _communityRepository;
        private readonly IRepository<UserNetwork> _userNetworkRepository;
        private readonly IRepository<UserPost> _postRepository;
        private readonly IRepository<Reaction> _reaction;
        private readonly IRepository<ReportEntity> _reportRepository;

        public AdminService(IRepository<User> userRepository,
            IRepository<OnlineUsers> onlineuserRepository,
            IRepository<UserNetwork> userNetworkRepository,
            IRepository<Event> eventRepository,
            IRepository<Community> communityRepository,
            IRepository<UserPost> postRepository,
            IRepository<Reaction> reaction,
            IRepository<ReportEntity> reportRepository)
        {
            _userNetworkRepository = userNetworkRepository;
            _userRepository = userRepository;
            _onlineuserRepository = onlineuserRepository;
            _eventRepository = eventRepository;
            _communityRepository = communityRepository;
            _postRepository = postRepository;
            _reaction = reaction;
            _reportRepository = reportRepository;
        }

        public async Task<JsonResponse> AllUserList(string Name, int PageNo, int Record)
        {
            try
            {
                if (Name.Length > 0)
                {
                    var query = await (from user in _userRepository.Table
                                      join onlineUser in _onlineuserRepository.Table
                                      on user.Id equals onlineUser.UserId into onlineJoin
                                      from onlineUser in onlineJoin.DefaultIfEmpty()
                                      where user.UserName.ToLower().Contains(Name.ToLower()) ||
                                      user.FirstName.ToLower().Contains(Name.ToLower()) ||
                                      user.LastName.ToLower().Contains(Name.ToLower())
                                      select new SearchUserResModel
                                      {
                                          UniqueId = "",
                                          FullName = user.FirstName+ " " + user.LastName,
                                          Email = user.Email,
                                          PhoneNumber = user.PhoneNumber,
                                          Id = user.Id,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          UserName = user.UserName,
                                          ProfileImg = user.ProfileImg,
                                          Online = onlineUser != null ? true : false,
                                          IsInvited = false,
                                          IsHidden = user.IsDeleted,
                                          Created = user.CreatedDate,
                                          IsBusinessAccount = user.IsBusinessAccount,
                                      }).ToListAsync();

                    var data = query.Skip((PageNo - 1) * Record).Take(Record).ToList();
                    var UserCount =  query.Count();

                    UserListResponse userSearchListResponse = new UserListResponse();
                    userSearchListResponse.UserCount = UserCount;
                    userSearchListResponse.searchUserResModel = data;
                    return new JsonResponse(200, true, "Success", userSearchListResponse);
                }

                var UserList = await (from user in _userRepository.Table
                                      join onlineUser in _onlineuserRepository.Table
                                      on user.Id equals onlineUser.UserId into onlineJoin
                                      from onlineUser in onlineJoin.DefaultIfEmpty()
                                      orderby user.CreatedDate
                                      select new SearchUserResModel
                                      {
                                          UniqueId = "",
                                          FullName = user.FirstName + " " + user.LastName,
                                          Email = user.Email,
                                          PhoneNumber = user.PhoneNumber,
                                          Id = user.Id,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          UserName = user.UserName,
                                          ProfileImg = user.ProfileImg,
                                          Online = onlineUser != null ? true : false,
                                          IsInvited = false,
                                          IsHidden = user.IsDeleted,
                                          Created = user.CreatedDate,
                                          IsBusinessAccount = user.IsBusinessAccount,
                                      }).Skip((PageNo - 1) * Record)
                                 .Take(Record).ToListAsync();

                var Count = await _userRepository.Table.CountAsync();

                UserListResponse userListResponse = new UserListResponse();
                userListResponse.UserCount = Count;
                userListResponse.searchUserResModel = UserList;
                return new JsonResponse(200, true, "Success", userListResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<JsonResponse> GetAllStats()
        {
            var UserCount = await _userRepository.Table.CountAsync();
            var EventCount = await _eventRepository.Table.CountAsync();
            var CommunityCount = await _communityRepository.Table.CountAsync();
            var PostCount = await _postRepository.Table.Where(x=> x.Type == "post").CountAsync();
            var CommentCount = await _postRepository.Table.Where(x => x.Type == "comment").CountAsync();
            var LikeCount = await _reaction.Table.Where(x=> x.Type =="like").CountAsync();

            AdminResponseModel adminResponseModel = new AdminResponseModel();
            adminResponseModel.LikeCount = LikeCount;
            adminResponseModel.CommentCount = CommentCount;
            adminResponseModel.UserCount = UserCount;
            adminResponseModel.PostCount = PostCount;
            adminResponseModel.CommunityCount = CommunityCount;
            adminResponseModel.EventCount = EventCount;

            return new JsonResponse(200, true, "Success", adminResponseModel);
        }

        public async Task<JsonResponse> BanUnBanUser(int Id)
        {
            var user = await _userRepository.Table.Where(x => x.Id == Id).FirstOrDefaultAsync();

            if (user == null)
            {
                return new JsonResponse(200, true, "User Not Found", null);
            }

            if (user.IsDeleted)
            {
                user.IsDeleted = false;
                await _userRepository.UpdateAsync(user);
                return new JsonResponse(200, true, "User Retrieve", null);

            }
            await _userRepository.DeleteAsync(user);
            return new JsonResponse(200, true, "User Delete", null);
        }

        public async Task<JsonResponse> ReportList(ReportReq req)
        {
            var Reports = await _reportRepository.Table.Where(x=> x.ReportType == req.Type).ToListAsync();
           
            var distinctReports =  Reports
                        .GroupBy(r => r.ReportId)
                        .Select(g => new ReportResponse
                        {
                            ReportedId = g.Key,
                            Type = g.First().ReportType,
                            ReportCount = g.Count()
                        }).ToList();

            if(req.Type == "post")
            {
                 var query = (from dr in distinctReports
                        join up in _postRepository.Table on dr.ReportedId equals up.Id
                        join u in _userRepository.Table on up.UserId equals u.Id
                        select new ReportResponse
                        {
                            UserId = u.Id,
                            UserName = u.UserName,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            ProfileImgUrl = u.ProfileImg,
                            ReportedId = dr.ReportedId,
                            ReportCount=dr.ReportCount,
                            Type = dr.Type,
                            IsBusinessAccount = u.IsBusinessAccount,
                        }).ToList();

                 return new JsonResponse(200, true, "Success", query);
            }

            if (req.Type == "event")
            {
                var query = (from dr in distinctReports
                             join up in _eventRepository.Table on dr.ReportedId equals up.Id
                             join u in _userRepository.Table on up.CreatedBy equals u.Id
                             select new ReportResponse
                             {
                                 UserId = u.Id,
                                 UserName = u.UserName,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 ProfileImgUrl = u.ProfileImg,
                                 ReportedId = dr.ReportedId,
                                 ReportCount = dr.ReportCount,
                                 Type = dr.Type,
                                 IsBusinessAccount= u.IsBusinessAccount,
                             }).ToList();
                return new JsonResponse(200, true, "Success", query);
            }

            if (req.Type == "community")
            {
                var query = (from dr in distinctReports
                             join up in _communityRepository.Table on dr.ReportedId equals up.Id
                             join u in _userRepository.Table on up.CreatedBy equals u.Id
                             select new ReportResponse
                             {
                                 UserId = u.Id,
                                 UserName = u.UserName,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 ProfileImgUrl = u.ProfileImg,
                                 ReportedId = dr.ReportedId,
                                 ReportCount = dr.ReportCount,
                                 Type = dr.Type,
                                 IsBusinessAccount = u.IsBusinessAccount,
                             }).ToList();
                return new JsonResponse(200, true, "Success", query);
            }
            return new JsonResponse(200, true, "Not Fount", null);
        }

        public async Task<JsonResponse> GetAllReportByReportedId(ReportDetailReq req)
        {
            var query = await (from m in _reportRepository.Table
                                    join u in _userRepository.Table on m.ActionUserId equals u.Id
                                    where m.ReportId == req.ReportedId  && m.IsDeleted == false
                                    select new ReportDetailsResponse
                                    {
                                        UserId = u.Id,
                                        Id = m.Id,
                                        FirstName = u.FirstName,
                                        LastName = u.LastName,
                                        UserName = u.UserName,
                                        Value = m.Value,
                                        Description = m.Description,
                                        ProfileImgUrl = u.ProfileImg,
                                        IsBusinessAccount=u.IsBusinessAccount,
                                    }).ToListAsync();

            return new JsonResponse(200, true, "Success", query);
        }
    }
}
