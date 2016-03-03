using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TrustLessAPI.Models;

namespace TrustLessAPI.Controllers
{
    public class LoginAuthController : ApiController
    {

        /// <summary>
        /// Verifies that a given string is a base64 encoded string.
        /// This method is to be used to verify that a send password has been base64 encoded before accepting it.
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns>Boolean indicating whether or not the string is base64 encoded. True = yes, false = no</returns>
        bool IsBase64(string base64String)
        {
            if (base64String.Replace(" ", "").Length % 4 != 0)
            {
                return false;
            }

            try
            {
                string text = Startup.Base64Decode(base64String);

                return true;
            }
            catch (Exception exception)
            {
                // Handle the exception
            }
            return false;
        }


        // Post api/values
        [Route("Regiser")]
        public HttpResponseMessage Post([FromBody]Person value)
        {
            if (value == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            if (String.IsNullOrEmpty(value.Username))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            if (String.IsNullOrEmpty(value.PublicKey))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            string password = nvc["password"];
            if (String.IsNullOrEmpty(password) || !IsBase64(password))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            password = Startup.Base64Decode(password);
            if (password.Contains("�"))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            //Open DataContext connection.
            using (DataContext dataContext = new DataContext())
            {
                var roleStore = new RoleStore<IdentityRole>((DbContext)dataContext);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var userStore = new UserStore<ApplicationUser>((DbContext)dataContext);
                var userManager = new UserManager<ApplicationUser>(userStore);

                //Attempt to create a new user.
                var user = new ApplicationUser { UserName = value.Username, PasswordHash = password };

                IdentityResult result = userManager.Create(user, password);

                //The user could not be created. This implies that the user must allready exist or that the email or password
                //couldn't get accepted.
                if (!result.Succeeded)
                {
                    return new HttpResponseMessage(HttpStatusCode.Forbidden);
                }

                dataContext.Persons.Add(value);
                dataContext.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
        }

    }
}
