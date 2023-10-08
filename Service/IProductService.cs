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
        Task<Product> AddProductFromSource(String url);
        void AddProductManual();
        void AddFilter();
        void LoadProduct();
    }
}
