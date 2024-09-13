using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
	public class Tags : BaseEntity
	{
		public string Name { get; set; }
		public string? Image { get; set; }
		public int Count { get; set; }
	}
	
}
