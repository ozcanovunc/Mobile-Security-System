using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mobile_Security_System.Controllers
{
    class UserController
    {
        private static readonly HttpClient ControllerClient = new HttpClient
        {
            BaseAddress = new Uri("http://mobilesecuritysystem.somee.com/get/")
        };

        public static async Task<bool> AddUser(string email, string password)
        {
            string requestString = "adduser?email=" + email + "&password=" + password;
            string requestResult = await ControllerClient.GetStringAsync(requestString);

            return JsonConvert.DeserializeObject<bool>(requestResult);
        }

        public static async Task<bool> DeleteUser(string email)
        {
            string requestString = "deleteuser?email=" + email;
            string requestResult = await ControllerClient.GetStringAsync(requestString);

            return JsonConvert.DeserializeObject<bool>(requestResult);
        }

        public static async Task<bool> UpdatePassword
            (string email, string old_password, string new_password)
        {
            string requestString = "updatepassword?email=" + email + 
                "&old_password=" + old_password + "&new_password=" + new_password;
            string requestResult = await ControllerClient.GetStringAsync(requestString);

            return JsonConvert.DeserializeObject<bool>(requestResult);
        }

        public static async Task<bool> GetUser(string email, string password)
        {
            string requestString = "getuser?email=" + email + "&password=" + password;
            string requestResult = await ControllerClient.GetStringAsync(requestString);

            return JsonConvert.DeserializeObject<bool>(requestResult);
        }
    }
}
