using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.Common
{
    public static class EmailHelper
    {
        public static string DownloadTemplate(string templateUrl)
        {
            string body = string.Empty;
            using (WebClient client = new WebClient())
            {
                var templatecode = client.DownloadString(templateUrl);
                body = templatecode;
            }
            return body;
        }

        public static string ReadLocalTemplate(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    string htmlContent = File.ReadAllText(fileName);
                    return htmlContent;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static bool SendEmailRequest(EmailRequest emailRequest, SMTPDetails sMTPDetails)
        {
            var body = ReadLocalTemplate("emailtemplate.html");
            body = body.Replace("{{SITETITLE}}", emailRequest.SITETITLE);
            body = body.Replace("{{USERNAME}}", emailRequest.USERNAME);
            body = body.Replace("{{CONTENT1}}", emailRequest.CONTENT1);
            body = body.Replace("{{CTALINK}}", emailRequest.CTALINK);
            body = body.Replace("{{CTATEXT}}", emailRequest.CTATEXT);
            body = body.Replace("{{CONTENT2}}", emailRequest.CONTENT2);
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = emailRequest.Subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(new MailAddress(emailRequest.ToEmail));
            mailMessage.From = new MailAddress("support@k4m2a.com", emailRequest.SITETITLE);
            if (!String.IsNullOrEmpty(emailRequest.BCCEMAIL))
            {
                mailMessage.Bcc.Add(new MailAddress(emailRequest.BCCEMAIL));
            }
            var result = SendEmail(mailMessage, sMTPDetails);
            return result;
        }

		public static bool SendEmailRequestWithtemplate(EmailRequest emailRequest, SMTPDetails sMTPDetails,string template)
		{
			var body = ReadLocalTemplate(template);
			MailMessage mailMessage = new MailMessage();
			mailMessage.Subject = emailRequest.Subject;
			mailMessage.Body = body;
			mailMessage.IsBodyHtml = true;
			mailMessage.To.Add(new MailAddress(emailRequest.ToEmail));
			mailMessage.From = new MailAddress("support@k4m2a.com", emailRequest.SITETITLE);
			if (!String.IsNullOrEmpty(emailRequest.BCCEMAIL))
			{
				mailMessage.Bcc.Add(new MailAddress(emailRequest.BCCEMAIL));
			}
			var result = SendEmail(mailMessage, sMTPDetails);
			return result;
		}

		public static bool SendEmail(MailMessage mailMessage, SMTPDetails sMTPDetails)
        {
            int port = Convert.ToInt32(sMTPDetails.Port);
            bool SSLEnable = Convert.ToBoolean(sMTPDetails.SSLEnable);
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.EnableSsl = SSLEnable;
            smtpClient.Port = port;
            NetworkCredential networkCredential = new NetworkCredential(sMTPDetails.Username, sMTPDetails.Password);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = networkCredential;
            smtpClient.Host = sMTPDetails.Host;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {

            }
         
            return false;
        }
    }
}
