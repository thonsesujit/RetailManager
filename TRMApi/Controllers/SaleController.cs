using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SaleController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles = "Cashier")]

        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData(_config);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //RequestContext.Principal.Identity.GetUserId(); // getting user Id from user who is logged in. data given to only one cashier.

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

            SaleData data = new SaleData(_config);
            return data.GetSaleReports();
        }
    }
}
