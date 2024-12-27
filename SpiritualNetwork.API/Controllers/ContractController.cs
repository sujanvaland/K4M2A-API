using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ContractController : ApiBaseController
    {
		private readonly IContractUserService _contractUserService;

		public ContractController(IContractUserService contractUserService)
		{
			_contractUserService = contractUserService;
		}

		[HttpPost(Name = "CreateUpdateUserContract")]
		public async Task<JsonResponse> CreateUpdateUserContract(UserContractRequest req)
		{
			try
			{
				var response = _contractUserService.CreateUpdateUserContract(req);
				return new JsonResponse(200, true, "Success", response);
			}
			catch (Exception ex)
			{
				return new JsonResponse(200, false, "Fail", ex.Message);
			}
		}
	}
}
