using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {

        // GET: User/GetById
        public List<UserModel> GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId(); // getting user Id from user who is logged in. 
            UserData data = new UserData();  // api model is display models and library is data access model. you might add some attribute to display model. we use automapper to add true speration.
            return data.GetUserById(userId);
             
        }

        
    }
}
