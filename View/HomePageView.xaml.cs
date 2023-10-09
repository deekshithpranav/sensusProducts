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
    public partial class HomePageView : Window
    {
        public HomePageView()
        {
            InitializeComponent();

            HomePageViewModel viewModel = new HomePageViewModel(ProductList);
            DataContext = viewModel;
            
        }
    }
}
