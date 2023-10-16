using sensusProducts.Model;
using sensusProducts.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace sensusProducts.View
{
    public partial class ProductDetailsView:Page
    {
        
        public ProductDetailsView(Product product, HomePageViewModel homePageViewModel, Frame ViewProductFrame)
        {

            InitializeComponent();
            ProductDetailsViewModel productDetailsViewModel = new ProductDetailsViewModel(product, homePageViewModel, ViewProductFrame);
            DataContext = productDetailsViewModel;
            
            productDetailsViewModel.ProductsGallery = productsGallery;
            productDetailsViewModel.UtilityList = UtilityList;
            productDetailsViewModel.GenerateGallery();
            productDetailsViewModel.GenerateUtilityListImages();
        }
    }
}
