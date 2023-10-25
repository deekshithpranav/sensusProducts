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
    public partial class AddProductView: Window
    {
        public AddProductView() {
            InitializeComponent();
            AddProductViewModel viewModel = new AddProductViewModel();
            DataContext = viewModel;
        }
    }
}
