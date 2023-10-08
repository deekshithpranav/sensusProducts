using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sensusProducts.Model
{
    public class Product
    {

        public string Description { get; set; }

        public string Name { get; set; }

        public string Features {  get; set; }

        public string DocLink { get; set; }

        public string FindDocLink { get; set; }

        public List<string> ImgLinks { get; set; }

        public List<UtilityType> UtilityTypes { get; set; }

        public Product() { 
        
        }

    }

    public enum UtilityType
    {
        Water,
        Gas,
        Electric,
        Software
    }
}
