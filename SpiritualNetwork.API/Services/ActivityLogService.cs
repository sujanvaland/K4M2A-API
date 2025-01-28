using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IRepository<ActivityLog> _activityRepository;
        private readonly IRepository<User> _userRepository;

        private readonly IMapper _mapper;
        public ActivityLogService(IRepository<ActivityLog> activityRepository,
             IRepository<User> userRepository, IMapper mapper) 
        { 
            _activityRepository = activityRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<JsonResponse> SaveUserActivity(ActivityModel activityLog) 
        {
            try
            {
                 ActivityLog activity = _mapper.Map<ActivityLog>(activityLog);

                 await _activityRepository.InsertAsync(activity);
                
                return new JsonResponse(200, true, "success", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetSearchKeywordsAndUsers(int UserId)
        {
            try
            {
                var keywords = await _activityRepository.Table
                         .Where(x => x.UserId == UserId && x.ActivityType == "keywords" && x.Type == "search" 
                          && x.IsDeleted == false && x.Message != "" )
                         .GroupBy(x => x.Message) // Group by unique Keyword (Message)
                         .Where(g => g.Any())
                         .Select(g => new SearchedKeywordsRes
                         {
                             Id = g.OrderByDescending(x => x.Id).FirstOrDefault().Id, 
                             Keywords = g.Key ?? ""
                         })
                         .OrderByDescending(x => x.Id) 
                         .Take(10) 
                         .ToListAsync();

                var users = (from x in _activityRepository.Table
                             join u in _userRepository.Table on x.RefId1 equals u.Id
                             where x.UserId == UserId && x.IsDeleted == false && x.Type == "search" && x.ActivityType == "user"
                             group u by u.Id into g 
                             where g.Any()
                             select new SearchedUserRes
                             {
                                 Id = g.OrderByDescending(u => u.Id).FirstOrDefault().Id,
                                 UserId = g.Key, 
                                 UserName = g.OrderByDescending(u => u.Id).FirstOrDefault().UserName,
                                 FirstName = g.OrderByDescending(u => u.Id).FirstOrDefault().FirstName,
                                 LastName = g.OrderByDescending(u => u.Id).FirstOrDefault().LastName,
                                 ProfileImgUrl = g.OrderByDescending(u => u.Id).FirstOrDefault().ProfileImg,
                                 IsBusinessAccount = g.OrderByDescending(u => u.Id).FirstOrDefault().IsBusinessAccount,
                             })
                             .OrderByDescending(u => u.Id) 
                             .Take(10) 
                             .ToList();

                SearchResponse results = new SearchResponse
                {
                    RecentSearchedKeywords = keywords,
                    RecentSearchedUsers = users
                };

                return new JsonResponse(200, true, "success", results);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> DeleteSearchKeywordsAndUsers(int Id,int UserId,string message,string type)
        {
            try
            {
                if (type == "all")
                {
                    var data = await _activityRepository.Table
                                .Where(x => x.UserId == UserId
                                 && (x.ActivityType == "user" || x.ActivityType == "keywords") 
                                 && x.Type == "search"
                                 && x.IsDeleted == false)
                                .ToListAsync();

                    await _activityRepository.DeleteRangeAsync(data); 

                    return new JsonResponse(200, true, "success", null);
                }
                if (string.IsNullOrEmpty(message) && type == "user")
                {
                    var user = await _activityRepository.Table
                        .Where(x => x.UserId == UserId && x.ActivityType == "user" && x.Type == "search"
                         && x.IsDeleted == false && x.RefId1 == Id)
                        .ToListAsync();

                   await _activityRepository.DeleteRangeAsync(user);
                    return new JsonResponse(200, true, "success", null);

                }
                else
                {
                    var data = await _activityRepository.Table
                       .Where(x => x.UserId == UserId && x.ActivityType == "keywords" && x.Type == "search"
                        && x.IsDeleted == false && x.Message == message)
                       .ToListAsync();

                    await _activityRepository.DeleteRangeAsync(data);
                    return new JsonResponse(200, true, "success", null);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
