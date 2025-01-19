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
		[MaxLength(100)]
		public string? Name { get; set; }
		[MaxLength(100)]
		public string? Phone { get; set; }
		[MaxLength(100)]
		public string? City { get; set; }
		[MaxLength(500)]
		public string? Journey { get; set; }
		public int? InviterId { get; set; }
	}
}
