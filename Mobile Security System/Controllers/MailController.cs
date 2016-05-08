using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mobile_Security_System.Controllers
{
    class MailController
    {
        private static readonly HttpClient ControllerClient = new HttpClient
        {
            BaseAddress = new Uri("http://mobilesecuritysystem.somee.com/get/")
        };

        public static async Task<bool> SendMail(string subject, string body, string to)
        {
            string requestString = "sendmail?subject=" + subject + "&body=" + body + "&to=" + to;
            string requestResult = await ControllerClient.GetStringAsync(requestString);

            return JsonConvert.DeserializeObject<bool>(requestResult);
        }
    }
}
