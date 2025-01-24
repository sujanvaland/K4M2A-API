using AutoMapper;
using Microsoft.AspNetCore.Http;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IRepository<ActivityLog> _activityRepository;
        private readonly IMapper _mapper;
        public ActivityLogService(IRepository<ActivityLog> activityRepository, IMapper mapper) 
        { 
            _activityRepository = activityRepository;
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

    }
}
