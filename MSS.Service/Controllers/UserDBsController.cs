using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using MSS.Service.Models;
using System.Security.Cryptography;
using System.Text;

namespace MSS.Service.Controllers
{
    public class UserDBsController : ApiController
    {
        private MobileSecuritySystemDBEntities db = new MobileSecuritySystemDBEntities();

        [Route("get/getuser")]
        [HttpGet]
        public bool GetUser(string email, string password)
        {
            string hash;

            using (MD5 md5Hash = MD5.Create())
            {
                hash = GetMd5Hash(md5Hash, password);
            }

            try
            {
                var user = db.UserDB.Where
                    (u => u.email == email && u.password == hash).First();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        [Route("get/updatepassword")]
        [HttpGet]
        public bool UpdatePassword(string email, string old_password, string new_password)
        {
            string old_password_hash, new_password_hash;

            try
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    old_password_hash = GetMd5Hash(md5Hash, old_password);
                    new_password_hash = GetMd5Hash(md5Hash, new_password);
                }

                var user = db.UserDB.Where
                    (u => u.email == email && u.password == old_password_hash).First();
                db.Entry(user).Entity.password = new_password_hash;
                db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("get/adduser")]
        [HttpGet]
        public bool AddUser(string email, string password)
        {
            string hash;

            using (MD5 md5Hash = MD5.Create())
            {
                hash = GetMd5Hash(md5Hash, password);
            }

            UserDB user_to_add = new UserDB
            {
                email = email,
                password = hash
            };

            try
            {
                // If user_to_add exists, do not add again
                if (db.UserDB.Where(u => u.email == email).Count() != 0)
                    return false;

                db.UserDB.Add(user_to_add);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("get/deleteuser")]
        [HttpGet]
        public bool DeleteUser(string email)
        {
            try
            {
                var user = db.UserDB.Where(u => u.email == email).First();
                db.Entry(user).State = EntityState.Deleted;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Source: https://msdn.microsoft.com/en-us/library/s02tk69a%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}