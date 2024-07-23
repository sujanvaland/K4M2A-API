using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class K4M2AService : IK4M2AService
     {
        private readonly IRepository<Service> _serviceRepository;
        private readonly IRepository<ServiceFAQ> _servicefaqRepository;
        private readonly IRepository<ServiceImages> _serviceimageRepository;
        private readonly IRepository<User> _userRepository;


        public K4M2AService (IRepository<Service> serviceRepository, 
            IRepository<ServiceFAQ> servicefaqRepository, 
            IRepository<ServiceImages> serviceimageRepository,
            IRepository<User> userRepository)
        {
            _serviceRepository = serviceRepository;
            _servicefaqRepository = servicefaqRepository;
            _serviceimageRepository = serviceimageRepository;
            _userRepository = userRepository;
        }

        public async Task<JsonResponse> SaveServiceDetails(ServiceReq req)
        {
            if (req.service.Id == 0)
            {
                await _serviceRepository.InsertAsync(req.service);
                
                foreach(var qa in req.FAQReq)
                {
                    ServiceFAQ FAQ = new ServiceFAQ();
                    FAQ.ServiceId = req.service.Id;
                    FAQ.Question = qa.Question;
                    FAQ.Answer = qa.Answer;
                    await _servicefaqRepository.InsertAsync(FAQ);
                }
                
                foreach (var img in req.ServiceImageUrl)
                {
                    ServiceImages Img = new ServiceImages();
                    Img.ServiceId = req.service.Id;
                    Img.ImageUrl = img;
                    await _serviceimageRepository.InsertAsync(Img);
                }
            }
            else
            {
                await _serviceRepository.UpdateAsync(req.service);
                foreach (var qa in req.FAQReq)
                {
                    var FAQ = _servicefaqRepository.Table.Where(x=>x.Id == qa.Id).FirstOrDefault();
                    FAQ.Answer = qa.Answer;
                    FAQ.Question = qa.Question;
                    await _servicefaqRepository.UpdateAsync(FAQ);
                }
            }

            return new JsonResponse(200, true, "Success", req.service);
        }

        
        public async Task<JsonResponse> UpdateServiceFAQ(ServiceFAQReq req)
        {
            var update = await _servicefaqRepository.Table.Where(x=> x.Id == req.Id).FirstOrDefaultAsync();

            if(update != null)
            {
                update.Question = req.Question;
                update.Answer = req.Answer;
                await _servicefaqRepository.UpdateAsync(update);
            }
            return new JsonResponse(200, true, "Success", null);
        }

        public async Task<JsonResponse> DeleteServiceFAQ(ServiceIdReq req)
        {
            var service = await _servicefaqRepository.Table.Where(x => x.Id == req.Id).FirstOrDefaultAsync();

            if (service != null)
            {
                await _servicefaqRepository.DeleteAsync(service);
            }
            return new JsonResponse(200, true, "Success", null);
        }

        public async Task<JsonResponse> GetServiceById(ServiceIdReq req)
        {
            var query = await (from us in _serviceRepository.Table.Where(x=> x.IsDeleted == false && x.Id == req.Id)
                        join u in _userRepository.Table on us.CreatedBy equals u.Id
                        where u.IsDeleted == false
                        select new ServiceResponse
                        {
                            Id = req.Id,
                            Name = us.Name,
                            Category = us.Category,
                            Description = us.Description,
                            Tags = us.Tags,
                            CreatedDate = us.CreatedDate,
                            ProfileImg = u.ProfileImg,
                            FullName = u.FirstName + " " + u.LastName,
                            UserName = u.UserName
                        }).FirstOrDefaultAsync();

            query.FAQReq = await(from QNA in _servicefaqRepository.Table.Where(x=> x.ServiceId == req.Id && x.IsDeleted == false)
                           select new ServiceFAQReq 
                           {
                               Id = QNA.Id,
                               ServiceId = req.Id,
                               Answer = QNA.Answer,
                               Question = QNA.Question,
                           }).ToListAsync();

            query.ServiceImageUrl = await (from SI in _serviceimageRepository.Table.Where(x => x.ServiceId == req.Id
                                          && x.IsDeleted == false)
                                           select new ImageServiceResponse
                                           {
                                               Id= SI.Id,
                                               ImageUrl = SI.ImageUrl,
                                           }).ToListAsync();

            return new JsonResponse(200, true, "Success", query);
        }

        public async Task<JsonResponse> DeleteService(ServiceIdReq req)
        {
            var service = await _serviceRepository.Table.Where(x=> x.Id == req.Id 
                                && x.IsDeleted == false).FirstOrDefaultAsync();
            if (service == null)
            {
                return new JsonResponse(200, false, "Service Not Found", null);

            }
            var ServiceImg = await _serviceimageRepository.Table.Where(x => x.ServiceId == req.Id
                                && x.IsDeleted == false).ToListAsync();
            var ServiceQNA = await _servicefaqRepository.Table.Where(x => x.ServiceId == req.Id
                                && x.IsDeleted == false).ToListAsync();
            await _serviceRepository.DeleteAsync(service);
            await _serviceimageRepository.DeleteRangeAsync(ServiceImg);
            await _servicefaqRepository.DeleteRangeAsync(ServiceQNA);

            return new JsonResponse(200, true, "Success", null);

        }

        public async Task<JsonResponse> GetAllService(GetServiceReq req)
        {
            var query = await (from us in _serviceRepository.Table
                               join u in _userRepository.Table on us.CreatedBy equals u.Id
                               where us.IsDeleted == false && u.IsDeleted == false && us.Category == req.Category
                                     && (req.Id <= 0 || us.CreatedBy == req.Id)
                               select new ServiceResponse
                               {
                                   Id = us.Id,
                                   Name = us.Name,
                                   Category = us.Category,
                                   Description = us.Description,
                                   Tags = us.Tags,
                                   CreatedDate = us.CreatedDate,
                                   ProfileImg = u.ProfileImg,
                                   FullName = u.FirstName + " " + u.LastName,
                                   UserName = u.UserName
                               }).ToListAsync();

            //foreach (var item in query)
            //{
            //    item.FAQReq = await (from QNA in _servicefaqRepository.Table.Where(x => x.ServiceId == item.Id 
            //                         && x.IsDeleted == false)
            //                          select new ServiceFAQReq
            //                          {
            //                              Id = QNA.Id,
            //                              ServiceId = item.Id,
            //                              Answer = QNA.Answer,
            //                              Question = QNA.Question,
            //                          }).ToListAsync();
            //}

            foreach (var item in query)
            {
                item.ServiceImageUrl = await (from SI in _serviceimageRepository.Table.Where(x => x.ServiceId == item.Id
                                          && x.IsDeleted == false)
                                               select new ImageServiceResponse
                                               {
                                                   Id = SI.Id,
                                                   ImageUrl = SI.ImageUrl,
                                               }).ToListAsync();
            }

            return new JsonResponse(200, true, "Success", query);
        }


    }
}
