using sensusProducts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sensusProducts.Service
{
    public interface IProductService
    {
        void AddProduct(Product product);
        List<Product> LoadProductsFromDB();
        void UpdateProduct(Product product, int OldProductID);
        void DeleteProductInDB(int PID);
    }
}
