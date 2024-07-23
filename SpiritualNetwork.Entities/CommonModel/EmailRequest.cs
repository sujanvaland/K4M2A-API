using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities.CommonModel
{
    public class EmailRequest
    {
        public string SITETITLE { get; set; }
        public string USERNAME { get; set; }
        public string CONTENT1 { get; set; }
        public string CTALINK { get; set; }
        public string CTATEXT { get; set; }
        public string CONTENT2 { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string BCCEMAIL { get; set; }
        public string SUPPORTEMAIL { get; set; }
    }
    public class SMTPDetails
    {
        public string Username { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public string SSLEnable { get; set; }
    }
}
