using Mobile_Security_System.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_Security_System.Controllers
{
    class MailController
    {
        private static readonly HttpClient ControllerClient = new HttpClient
        {
            BaseAddress = new Uri("http://mobilesecuritysystem.somee.com/get/")
        };

        public static async Task<bool> SendMail(MailModel mail)
        {
            var stringContent = new StringContent(
                JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
            var response = await ControllerClient.PostAsync(
                ControllerClient .BaseAddress + "sendmail", stringContent);
            var result = await response.Content.ReadAsStringAsync();

            if (result == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}