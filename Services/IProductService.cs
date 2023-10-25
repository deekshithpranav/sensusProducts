using sensusProducts.Model;
using System.Collections.Generic;

namespace sensusProducts.Service
{
    public interface IProductService
    {
        void AddProduct(Product product);
        List<Product> LoadProductsFromDB();
        void UpdateProduct(Product product, int OldProductID);
        void DeleteProductInDB(int PID);
        bool ProductExistsInDB(string productTitle);
    }
}
