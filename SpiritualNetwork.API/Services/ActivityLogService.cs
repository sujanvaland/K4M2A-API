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
                    .Where(x => x.UserId == UserId && x.ActivityType == "keywords" && x.Type == "search")
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Message)
                    .Distinct()              
                    .Take(10).ToListAsync();

                var users = (from x in _activityRepository.Table
                            join u in _userRepository.Table on x.RefId1 equals u.Id
                            where x.UserId == UserId && x.IsDeleted == false && x.Type == "search" && x.ActivityType == "user"
                            select new SearchedUserRes
                            {
                                Id = u.Id,
                                UserName = u.UserName,
                                FirstName = u.FirstName,   
                                LastName = u.LastName,
                                ProfileImgUrl = u.ProfileImg,
                                IsBusinessAccount = u.IsBusinessAccount,
                            })
                            .Distinct()     
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

    }
}
