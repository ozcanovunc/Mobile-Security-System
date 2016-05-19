using MSS.Service.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Http;

namespace MSS.Service.Controllers
{
    public class MailController : ApiController
    {
        [Route("get/sendmail")]
        [HttpPost]
        public bool SendMail([FromBody]MailModel value)
        {
            string  image = value.Image,
                    subject = value.Subject,
                    body = value.Body,
                    to = value.To;

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

                if (image != null)
                {
                    mail.Attachments.Add(
                        new Attachment(
                            new MemoryStream(
                                Convert.FromBase64String(image)), "Screenshot.jpg"));
                }

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