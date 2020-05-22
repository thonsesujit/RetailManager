using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

/// <summary>
/// 
/// </summary>
namespace TRMDataManager.Controllers
{
    //As long as they are authorized they can get list of the products.
    //TODO: To add admin rights to controllers.
    [Authorize(Roles = "Cashier")]
    public class ProductController : ApiController
    {
        // GET api/values
        public List<ProductModel> Get()
        {
            ProductData data = new ProductData();
            return data.GetProducts();
        }
    }
}
