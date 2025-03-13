using Microsoft.AspNetCore.Mvc;
using K4M2A.Entities.Model;
using K4M2A.API.Services;
using K4M2A.API.Services.Interface;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class SubcriptionController : ApiBaseController
    {
       private readonly ISubcriptionService _subcriptionService;

       public SubcriptionController(ISubcriptionService subcriptionService)
        {
            _subcriptionService = subcriptionService;   
        }

        [HttpPost(Name = "BuySubcription")]
        public async Task<JsonResponse> BuySubcription(SubcriptionModel req)
        {
            try
            {
                var response = await _subcriptionService.SaveSubcription(req, user_unique_id);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
