using SpiritualNetwork.Entities;
using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
	public class UserContractRequest
	{
		[MaxLength(200)]
		public string Address { get; set; }
		public int UserId { get; set; }
		public decimal AmountPaid { get; set; }
		[MaxLength(20)]
		public string CoinPaid { get; set; }
		[MaxLength(20)]
		public string ChainId { get; set; }
		public decimal NoOfCoins { get; set; }
	}
}
