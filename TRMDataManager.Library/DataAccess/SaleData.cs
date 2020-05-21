using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    /// <summary>
    /// Saving data to the database.
    /// </summary>
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            //TODO: make a SOLID/DRY/better
            // start filling in the models which we need to save in the database.
            //fill in the available information
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData products = new ProductData();
            var taxRate = ConfigHelper.GetTaxRate()/100;


            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity

                };

                //Get the information about this product
                var productInfo = products.GetProductById(item.ProductId);

                if(productInfo == null)
                {
                    throw new Exception($"The product Id of {item.ProductId} could not be found in the database.");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);

                }

                details.Add(detail);
            }
            //Create the sale model

            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };


            sale.Total = sale.SubTotal + sale.Tax;


            using (SqlDataAccess sql = new SqlDataAccess())
            {
                try
                {
                    sql.StartTransaction("TRMData");
                    //Save the sale model
                    sql.SaveDataInTransaction<SaleDBModel>("dbo.spSale_Insert", sale);

                    //getting ID from the sale mode
                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

                    //finish filling in the sale detail models.
                    //for 1000s of calls. use advanced dapper. where you transfter a table .
                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;
                        //save the sale detail models
                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);

                    }
                    sql.CommitTransaction();
                }
                catch 
                {

                    sql.RollbackTransaction();
                    throw;
                }

            }
      

        }
        public List<SaleReportModel> GetSaleReports()
        {
            SqlDataAccess sql = new SqlDataAccess();
            var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");
            return output;
        }
    }
}
