using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class HashTag : BaseEntity
    {
		[MaxLength(50)]
        public string Name { get; set; }
        public int Count { get; set; }
    }

}
