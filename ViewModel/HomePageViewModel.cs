using sensusProducts.Service;
using sensusProducts.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace sensusProducts.ViewModel
{
    public class HomePageViewModel
    {
        #region Properties

        public ICommand OpenAddProductPageCommand { get; }
        #endregion

        public HomePageViewModel()
        {
            OpenAddProductPageCommand = new RelayCommand(OpenAddProductWindow);
        }

        private void OpenAddProductWindow()
        {
            // Create and show the "AddProductView" window
            var addProductWindow = new AddProductView();
            addProductWindow.InitializeComponent();
            addProductWindow.Show();
        }
    }
}
