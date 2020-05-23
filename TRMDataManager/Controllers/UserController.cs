
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;
using TRMDataManager.Models;

namespace TRMDataManager.Controllers
{
    /// <summary>
    /// Geting your user id.
    /// </summary>
    [Authorize]
    public class UserController : ApiController
    {

        // GET: User/GetById
        [HttpGet]
        public UserModel GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId(); // getting user Id from user who is logged in. 
            UserData data = new UserData(); // api model is display models and library is data access model. you might add some attribute to display model. we use automapper to add true speration.
            return data.GetUserById(userId).First();
             
        }

        //getting user roles.
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = userManager.Users.ToList();
                var roles = context.Roles.ToList();

                foreach (var user in users)
                {
                    ApplicationUserModel u = new ApplicationUserModel
                    {
                        Id = user.Id,
                        Email = user.Email
                    };

                    foreach (var r in user.Roles)
                    {
                        //adds role id and role name for every role user has.
                        u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name); 
                    }

                    output.Add(u);

                }
            }

            return output; 
        }
        
    }
}
