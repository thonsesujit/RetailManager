using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        [Authorize(Roles = "Cashier")]

        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData();
            string userId = RequestContext.Principal.Identity.GetUserId(); // getting user Id from user who is logged in. data given to only one cashier.

            data.SaveSale(sale, userId);

        }

        [Authorize(Roles = "Admin,Manager")]

        //get for the sale report. api/sale/GetSalesReport 
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSaleReport()
        {

            ////Custom report if needed
            //if (RequestContext.Principal.IsInRole("Manager"))
            //{
            //    //Do Admin stuffs
            //}

            SaleData data = new SaleData();
            return data.GetSaleReports();
        }
    }
}
