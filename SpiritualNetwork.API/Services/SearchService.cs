using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using Twilio.TwiML.Voice;

namespace SpiritualNetwork.API.Services
{
    public class SearchService : ISearchService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<OnlineUsers> _onlineuserRepository;
        private readonly IRepository<UserNetwork> _userNetworkRepository;

        public SearchService(IRepository<User> userRepository, 
            IRepository<OnlineUsers> onlineuserRepository,
            IRepository<UserNetwork> userNetworkRepository)
        {
            _userNetworkRepository = userNetworkRepository;
            _userRepository = userRepository;
            _onlineuserRepository = onlineuserRepository;

        }

        public async Task<JsonResponse> SearchUser(string Name, int PageNo, int Record)
        {
            try 
            {
                if (Name.Length > 0)
                {
                    var data = await (from user in _userRepository.Table
                                      join onlineUser in _onlineuserRepository.Table 
                                      on user.Id equals onlineUser.UserId into onlineJoin
                                      from onlineUser in onlineJoin.DefaultIfEmpty()
                                      where user.UserName.ToLower().Contains(Name.ToLower()) || 
                                      user.FirstName.ToLower().Contains(Name.ToLower()) || 
                                      user.LastName.ToLower().Contains(Name.ToLower()) && 
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
                                          IsInvited = false
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
                                            IsInvited = user.IsInvited
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
                                          userId = user.Id
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
                                                  x.Tags
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
