using SpiritualNetwork.Entities;
using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
	public class UserContractRequest
	{
		public int UserId { get; set; }
		public decimal AmountPaid { get; set; }
		public string CoinPaid { get; set; }
		public string ChainId { get; set; }
		public decimal NoOfCoins { get; set; }
		public string WalletAddress { get; set; }
		public string Signature { get; set; }
		public string Message { get; set; }
	}
}
