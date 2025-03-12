using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
	public class UserContract : BaseEntity
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
