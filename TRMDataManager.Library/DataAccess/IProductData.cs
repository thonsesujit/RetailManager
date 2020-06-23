using System.Collections.Generic;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public interface IProductData
    {
        ProductModel GetProductById(int productid);
        List<ProductModel> GetProducts();
    }
}