using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
	public class InviteRequest : BaseEntity
	{
		[MaxLength(100)]
		public string Email { get; set; }
		public int? InviterId { get; set; }
	}
}
