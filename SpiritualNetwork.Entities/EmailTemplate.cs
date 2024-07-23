using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class EmailTemplate : IdEntity
    {
        public string EmailType { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public string BCC { get; set; }
        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public string CTAText { get; set; }
        public string CTALink { get; set; }
    }
}
