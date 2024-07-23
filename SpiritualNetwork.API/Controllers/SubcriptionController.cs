using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
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
