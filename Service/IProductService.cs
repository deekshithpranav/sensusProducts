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
        List<Product> LoadProductPage();
        void UpdateProduct();
        void DeleteProduct();
    }
}
