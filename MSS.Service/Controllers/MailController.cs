using System.Net;
using System.Web.Http;
using System.Net.Mail;

namespace MSS.Service.Controllers
{
    public class MailController : ApiController
    {
        [Route("get/sendmail")]
        [HttpGet]
        public bool SendMail(string subject, string body, string to)
        {
            try
            {
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential
                    ("notifications.mss@gmail.com", "12345Aa!");
                smtp.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("notifications.mss@gmail.com");
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                
                smtp.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}