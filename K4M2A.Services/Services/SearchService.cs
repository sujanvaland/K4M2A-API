using Microsoft.EntityFrameworkCore;
using K4M2A.Entities.Model;
using K4M2A.Services.Interface;
using K4M2A.Entities;
using K4M2A.Entities.CommonModel;
using System.Text.RegularExpressions;

namespace K4M2A.Services
{
    public class SearchService : ISearchService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<OnlineUsers> _onlineuserRepository;
        private readonly IRepository<UserNetwork> _userNetworkRepository;
        private readonly IRepository<HashTag> _hashTagRepository;
        private readonly IRepository<UserFollowers> _userFollowers;
        private readonly IRepository<UserMuteBlockList> _blockmuteRepository;

        public SearchService(IRepository<User> userRepository, 
            IRepository<OnlineUsers> onlineuserRepository,
            IRepository<UserNetwork> userNetworkRepository,
            IRepository<HashTag> hashTagRepository,
            IRepository<UserFollowers> userFollowers,
            IRepository<UserMuteBlockList> blockmuteRepository)
        {
            _userNetworkRepository = userNetworkRepository;
            _userRepository = userRepository;
            _onlineuserRepository = onlineuserRepository;
            _hashTagRepository = hashTagRepository;
            _userFollowers = userFollowers;
            _blockmuteRepository = blockmuteRepository;
        }

        public async Task<JsonResponse> SearchUser(string Name, int PageNo, int Record, int LoginId)
        {
            try 
            {
                Name = Regex.Replace(Name, @"^@+", "").Trim();

                if (Name.Length > 0)
                {
                    var trimmedName = string.Join("", Name.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries));

                    var data = await (from user in _userRepository.Table
                                      join onlineUser in _onlineuserRepository.Table 
                                      on user.Id equals onlineUser.UserId into onlineJoin
                                      from onlineUser in onlineJoin.DefaultIfEmpty()
                                      join uf in _userFollowers.Table.Where(x => x.UserId == LoginId) on user.Id equals uf.FollowToUserId into ufGroup
                                      from uf in ufGroup.DefaultIfEmpty()

                                      join blocked in _blockmuteRepository.Table
                                      on new { UserId = user.Id, BlockedUserId = LoginId }
                                      equals new { blocked.UserId, blocked.BlockedUserId } into blockJoin
                                      from blocked in blockJoin.DefaultIfEmpty()

                                      where blocked == null && user.UserName.ToLower().Contains(Name.ToLower()) || 
                                      user.FirstName.ToLower().Contains(Name.ToLower()) || 
                                      user.LastName.ToLower().Contains(Name.ToLower()) ||
                                      (user.FirstName.Trim() + user.LastName.Trim()).ToLower().Contains(trimmedName) && 
                                      user.IsDeleted == false
                                      select new SearchUserResModel
                                      {
                                          UniqueId = "",
                                          FullName = "",
                                          Email = "",
                                          PhoneNumber = "",
                                          Id = user.Id,
                                          FirstName=user.FirstName,
                                          LastName = user.LastName,
                                          UserName = user.UserName,
                                          ProfileImg = user.ProfileImg,
                                          Online = onlineUser != null ? true : false,
                                          IsFollowedByLoginUser = uf != null,
                                          IsInvited = false,
                                          IsBusinessAccount= user.IsBusinessAccount,
                                      })
                                    .Skip((PageNo - 1) * Record)
                                    .Take(Record)
                                    .ToListAsync();


                    if(data.Count < Record)
                    {
                        var OtherUsers = await _userNetworkRepository.Table
                                        .Where(user => 
                                        (user.FirstName.ToLower().Contains(Name.ToLower()) ||
                                        user.LastName.ToLower().Contains(Name.ToLower()) ||
                                        user.FullName.ToLower().Contains(Name.ToLower()) ||
                                        user.FullName.ToLower().Contains(Name.ToLower())) &&
                                        user.IsInvited == false)
                                        .Select(user => new SearchUserResModel
                                        {
                                            FullName = user.FullName,
                                            Email = user.Email,
                                            PhoneNumber = user.PhoneNumber,
                                            Id = user.Id,
                                            FirstName = user.FirstName,
                                            LastName = user.LastName,
                                            UserName = "",
                                            ProfileImg = user.Photo,
                                            Online = false,
                                            UniqueId = user.UniqueId,
                                            IsInvited = user.IsInvited,
                                            IsFollowedByLoginUser = false,
                                        })
                                        .Skip((PageNo - 1) * (Record - data.Count))
                                        .Take(Record - data.Count)
                                        .ToListAsync();

                        data.AddRange(OtherUsers);
                    }
                    
                    return new JsonResponse(200, true, "Success", data);

                }

                return new JsonResponse(200, true, "No User Found", null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> SearchOrganisationUser(string Name, int PageNo, int Record, int LoginId)
        {
            try
            {
                Name = Regex.Replace(Name, @"^@+", "").Trim();

                if (Name.Length > 0)
                {
                    var trimmedName = string.Join("", Name.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries));

                    var data = await (from user in _userRepository.Table
                                      join onlineUser in _onlineuserRepository.Table
                                      on user.Id equals onlineUser.UserId into onlineJoin
                                      from onlineUser in onlineJoin.DefaultIfEmpty()
                                      join uf in _userFollowers.Table.Where(x => x.UserId == LoginId) on user.Id equals uf.FollowToUserId into ufGroup
                                      from uf in ufGroup.DefaultIfEmpty()

                                      join blocked in _blockmuteRepository.Table
                                      on new { UserId = user.Id, BlockedUserId = LoginId }
                                      equals new { blocked.UserId, blocked.BlockedUserId } into blockJoin
                                      from blocked in blockJoin.DefaultIfEmpty()

                                      where blocked == null && user.UserName.ToLower().Contains(Name.ToLower()) ||
                                      user.FirstName.ToLower().Contains(Name.ToLower()) ||
                                      user.LastName.ToLower().Contains(Name.ToLower()) ||
                                      (user.FirstName.Trim() + user.LastName.Trim()).ToLower().Contains(trimmedName) &&
                                      user.IsDeleted == false && user.IsBusinessAccount == true
                                      select new SearchUserResModel
                                      {
                                          UniqueId = "",
                                          FullName = "",
                                          Email = "",
                                          PhoneNumber = "",
                                          Id = user.Id,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          UserName = user.UserName,
                                          ProfileImg = user.ProfileImg,
                                          Online = onlineUser != null ? true : false,
                                          IsFollowedByLoginUser = uf != null,
                                          IsInvited = false,
                                          IsBusinessAccount = user.IsBusinessAccount,
                                      })
                                    .Skip((PageNo - 1) * Record)
                                    .Take(Record)
                                    .ToListAsync();

                    return new JsonResponse(200, true, "Success", data);

                }
                else
                {
                        var data = (from user in _userRepository.Table
                                 join onlineUser in _onlineuserRepository.Table
                                 on user.Id equals onlineUser.UserId into onlineJoin
                                 from onlineUser in onlineJoin.DefaultIfEmpty()
                                 join uf in _userFollowers.Table.Where(x => x.UserId == LoginId) on user.Id equals uf.FollowToUserId into ufGroup
                                 from uf in ufGroup.DefaultIfEmpty()
                                 where user.IsDeleted == false && user.IsBusinessAccount == true
                                 orderby user.Id descending 
                                 select new SearchUserResModel
                                 {
                                     UniqueId = "",
                                     FullName = "",
                                     Email = "",
                                     PhoneNumber = "",
                                     Id = user.Id,
                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     UserName = user.UserName,
                                     ProfileImg = user.ProfileImg,
                                     Online = onlineUser != null,
                                     IsFollowedByLoginUser = uf != null,
                                     IsInvited = false,
                                     IsBusinessAccount = user.IsBusinessAccount,
                                 })
                                .Skip((PageNo - 1) * Record)
                                .Take(Record);

                    return new JsonResponse(200, true, "Success", data);

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<JsonResponse> GetSearchHashTag(string Name)
        {
            try
            {
                var hashTag = await _hashTagRepository.Table.Where(x => x.Name.ToLower().Contains(Name.ToLower()))
                                    .OrderByDescending(x => x.Count)    
                                    .Take(30)                           
                                    .ToListAsync();
                return new JsonResponse(200, true, "Success", hashTag);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> MentionSearchUser(string Name, int PageNo, int Record)
        {
            try
            {
                if (Name.Length > 0)
                {
                    var data = await (from user in _userRepository.Table
                                      join onlineUser in _onlineuserRepository.Table on user.Id equals onlineUser.UserId into onlineJoin
                                      from onlineUser in onlineJoin.DefaultIfEmpty()
                                      where user.UserName.Contains(Name) || user.FirstName.Contains(Name)
                                      || user.LastName.Contains(Name) && user.IsDeleted == false
                                      select new Mentions
                                      {
                                          name = user.FirstName + " " + user.LastName,
                                          avatar = (user.ProfileImg == null || user.ProfileImg == "") ? "https://www.k4m2a.com/images/img_userpic.jpg" : user.ProfileImg,
                                          link = "/profile/" + user.UserName,
                                          userId = user.Id,
                                          IsBusinessAccount = user.IsBusinessAccount,
                                          userName = user.UserName,
                                      })
                                    .Skip((PageNo - 1) * Record)
                                    .Take(Record)
                                    .ToListAsync();

                    return new JsonResponse(200, true, "Success", data);

                }

                return new JsonResponse(200, true, "No User Found", null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<JsonResponse> SearchUserProfile(string Name)
        {
            try
            {
                if (Name.Length > 0)
                {
                    var data = await _userRepository.Table.Where(x => x.UserName == Name).Select(x => new
                                              {
                                                  x.FirstName,
                                                  x.LastName,
                                                  x.UserName,
                                                  x.ProfileImg,
                                                  x.BackgroundImg,
                                                  x.CreatedDate,
                                                  x.About,
                                                  x.Skills,
                                                  x.Tags,
                                                  x.IsBusinessAccount
                                              }).FirstOrDefaultAsync();

                    return new JsonResponse(200, true, "Success", data);
                }

                return new JsonResponse(200, true, "No User Found", null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetMyGoogleContactList(int UserId)
        {
            try
            {
                var OtherUsers = await _userNetworkRepository.Table
                                        .Where(user => user.InviterId == UserId)
                                        .Select(user => new SearchUserResModel
                                        {
                                            FullName = user.FullName,
                                            Email = user.Email,
                                            PhoneNumber = user.PhoneNumber,
                                            Id = user.Id,
                                            FirstName = user.FirstName,
                                            LastName = user.LastName,
                                            UserName = "",
                                            ProfileImg = user.Photo,
                                            Online = false,
                                            UniqueId = user.UniqueId,
                                            IsInvited = user.IsInvited
                                        }).ToListAsync();

                return new JsonResponse(200, true, "Success", OtherUsers);

            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> RemoveContactList(int UserId)
        {
            try
            {
                var list = await _userNetworkRepository.Table.Where(x=>x.InviterId==UserId).ToListAsync();
                await _userNetworkRepository.DeleteRangeAsync(list);

                return new JsonResponse(200, true, "Success", list);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
