using sensusProducts.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using sensusProducts.Model;
using System.Windows;
using System.Windows.Controls;
using sensusProducts.Service;

namespace sensusProducts.ViewModel
{
    public class ProductDetailsViewModel:INotifyPropertyChanged
    {

        public ProductDetailsViewModel()
        {

        }

        public ProductDetailsViewModel(Product product, HomePageViewModel homePageViewModel)
        {
            FindDocURLCommand = new RelayCommand(FindDocURL);
            ProductDocURLCommand = new RelayCommand(ProductDocURL);
            DeleteProductCommand = new RelayCommand(deleteProduct);
            NavigateBack = new RelayCommand(goBack);
            this.homePageViewModel = homePageViewModel;
            this.product = product;
            initializeParams();
        }

        #region Properties

        Product product;

        public event PropertyChangedEventHandler PropertyChanged;
        ProductServices productServices = new ProductServices();
        public string fdURL { get; set; }
        public string pdURL { get; set; }

        private string productDescription;
        public string ProductDescription { 
            get { 
                return productDescription; 
            } 
            set {  
                productDescription = value; 
                OnPropertyChanged(nameof(productDescription));
            } 
        }

        private string productName;
        public string ProductName
        {
            get { return productName; }
            set
            {
                productName = value;
                OnPropertyChanged(nameof(productName));
            }
        }


        private string productFeatures;
        public string ProductFeatures
        {
            get { return productFeatures;}
            set
            {
                productFeatures = value; 
                OnPropertyChanged(nameof(productFeatures));
            }
        }

        public void FindDocURL()
        {
            System.Diagnostics.Process.Start(fdURL);
        }

        public void ProductDocURL()
        {
            System.Diagnostics.Process.Start(pdURL);
        }
        public ICommand FindDocURLCommand { get; private set; }
        public ICommand ProductDocURLCommand { get; private set; }

        public ICommand NavigateBack { get; }

        HomePageViewModel homePageViewModel { get; }

        public ICommand UpdateProductCommand;
        public ICommand DeleteProductCommand { get; private set; }
    

        #endregion


        public void initializeParams()
        {
            ProductName = product.Name;
            fdURL = product.FindDocLink;
            pdURL = product.DocLink;
            ProductDescription = product.Description;
            ProductFeatures = product.Features;

        }

        

        public void goBack()
        {
            homePageViewModel.GoBackToMain();
        }

        public void deleteProduct()
        {
            productServices.DeleteProductInDB(product.Id);
            goBack();
            homePageViewModel.RefreshProductsList();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
