using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class NotificationTemplate : BaseEntity
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string Route { get; set; }

    }
}
