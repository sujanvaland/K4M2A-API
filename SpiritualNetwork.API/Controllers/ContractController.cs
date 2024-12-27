using GraphQL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Signer;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
	
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Microsoft.AspNetCore.Authorization.AllowAnonymous]
	public class ContractController : ApiBaseController
    {
		private readonly IContractUserService _contractUserService;

		public ContractController(IContractUserService contractUserService)
		{
			_contractUserService = contractUserService;
		}

		[HttpPost(Name = "CreateUpdateUserContract")]
		public async Task<JsonResponse> CreateUpdateUserContract(UserContractRequest request)
		{
			try
			{
				// Ensure the wallet address and signature are provided
				if (string.IsNullOrEmpty(request.WalletAddress) || string.IsNullOrEmpty(request.Signature))
				{
					new JsonResponse(500, false, "Wallet address or signature is missing");
				}

				// Verify the signature
				if (VerifySignature(request.WalletAddress, request.Signature, request.Message))
				{
					var response = _contractUserService.CreateUpdateUserContract(request);
					return new JsonResponse(200, true, "Success", response);
				}
				else
				{
					new JsonResponse(500, false, "Invalid signature");
				}

				return new JsonResponse(500, false, "Something went wrong");
			}
			catch (Exception ex)
			{
				return new JsonResponse(200, false, "Fail", ex.Message);
			}
		}

		private bool VerifySignature(string walletAddress, string signature, string message)
		{
			try
			{
				// Recover address from the signed message
				var signer = new EthereumMessageSigner();
				var recoveredAddress = signer.EncodeUTF8AndEcRecover(message, signature);

				// Check if the recovered address matches the provided wallet address
				return string.Equals(recoveredAddress, walletAddress, StringComparison.OrdinalIgnoreCase);
			}
			catch
			{
				return false;
			}
		}
	}
}
