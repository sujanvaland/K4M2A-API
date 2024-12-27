using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Services
{
	public interface IContractUserService
	{
		public UserContract CreateUpdateUserContract(UserContractRequest userContract);
	}
	public class ContractUserService : IContractUserService
	{
		private readonly IRepository<UserContract> _userContractRepo;
		public ContractUserService(IRepository<UserContract> userContractRepo)
		{
			_userContractRepo = userContractRepo;
		}

		public UserContract CreateUpdateUserContract(UserContractRequest userContract)
		{
			var contract = _userContractRepo.Table.Where(x => x.Address == userContract.Address).FirstOrDefault();
			if(contract == null)
			{
				contract = new UserContract();
				contract.Address = userContract.Address;
				contract.UserId = userContract.UserId;
				contract.AmountPaid = userContract.AmountPaid;
				contract.CoinPaid = userContract.CoinPaid;
				contract.ChainId = userContract.ChainId;
				contract.NoOfCoins = userContract.NoOfCoins;
				_userContractRepo.Insert(contract);
			}
			else
			{
				contract.Address = userContract.Address;
				contract.UserId = userContract.UserId;
				contract.AmountPaid = userContract.AmountPaid;
				contract.CoinPaid = userContract.CoinPaid;
				contract.ChainId = userContract.ChainId;
				contract.NoOfCoins = userContract.NoOfCoins;
				_userContractRepo.Update(contract);
			}
			
			return contract;
		}
	}
}
