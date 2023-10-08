using sensusProducts.Model;
using sensusProducts.View;
using sensusProducts.ViewModel.Helpers;
using sensusProducts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Input;
using sensusProducts.ViewModel.Commands;


namespace sensusProducts.ViewModel
{
    public class AddProductViewModel:INotifyPropertyChanged
    {
        #region Properties
        private IProductService productService;

        public event PropertyChangedEventHandler PropertyChanged;

        public OptionConverter optionConverter;

        public ICommand addProductCommand { get; }


        private string selectedOption;

        public string SelectedOption
        {
            get { return selectedOption; }
            set
            {
                selectedOption = value;
                // Notify property change to update the UI
                OnPropertyChanged(nameof(SelectedOption));
            }
        }
        private string productLink;

        public string ProductLink
        {
            get { return productLink; }
            set
            {
                if (productLink != value)
                {
                    productLink = value;
                    OnPropertyChanged(nameof(ProductLink));
                   
                }
            }
        }



        #endregion
        public AddProductViewModel()
        {
            addProductCommand = new AddproductCommand(Add_Product);
        }

        public AddProductViewModel(IProductService productService)
        {
            this.productService = productService;
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        #region Properties

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public string ProductFeatures { get; set; }

        public UtilityType ProductUtilityType { get; set; }

        public string ProductFindDocLink { get; set; }

        public string ProductDocLink { get; set; }

        public List<string> ProductImgLinks { get; set; }


        #endregion

        
        public async void Add_Product()
        {
            
            if (selectedOption == "Source")
            {
                if(ProductLink == null)
                {
                    return;
                }
                productService = new ProductServices();
              Product product = await productService.AddProductFromSource(ProductLink);
                Debug.WriteLine("Title: " + product.Name);
                Debug.WriteLine("Description:\n" + product.Description);
                Debug.WriteLine("Features:\n" + product.Features);
                Debug.WriteLine("Documents PDF link: " + product.DocLink);
                Debug.WriteLine("Images links: ");
                foreach (var imgLink in product.ImgLinks)
                {
                    Debug.WriteLine(imgLink);
                }
                Debug.WriteLine("Utility Types: " + string.Join(", ", product.UtilityTypes));


            }
            else
            {
                Debug.Write("Manual Selected");

                Product product = new Product
                {
                    uType = ProductUtilityType,
                    Name = ProductName,
                    Description = ProductDescription,
                    FindDocLink = ProductFindDocLink,
                    DocLink = ProductDocLink,
                    ImgLinks = ProductImgLinks
                };
            }
            
            

        }

    }

   

}
