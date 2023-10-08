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
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace sensusProducts.ViewModel
{
    public class AddProductViewModel:INotifyPropertyChanged
    {
        #region Properties
        private readonly IProductService productService;

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
                    // Update IsSubmitButtonEnabled based on text in ProductLink
                    IsSubmitButtonEnabled = !string.IsNullOrEmpty(productLink);
                }
            }
        }

        private bool isSubmitButtonEnabled;

        public bool IsSubmitButtonEnabled
        {
            get { return isSubmitButtonEnabled; }
            set
            {
                if (isSubmitButtonEnabled != value)
                {
                    isSubmitButtonEnabled = value;
                    OnPropertyChanged(nameof(IsSubmitButtonEnabled));
                }
            }
        }

        #endregion


        [Obsolete]
        public AddProductViewModel()
        {
            addProductCommand = new AddproductCommand(Add_Product);
            IsSubmitButtonEnabled = false;
            ImageTextBoxes = new ObservableCollection<ImageTextBox>();
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

        public string ProductFindDocLink { get; set; }

        public string ProductDocLink { get; set; }

        public List<string> ProductImgLinks { get; set; }

        public ObservableCollection<ImageTextBox> ImageTextBoxes { get; set; } 

        public List<UtilityType> UtilityTypesList { get; set; }

        private string _selectedItemCount;
        public string SelectedItemCount
        {
            get { return _selectedItemCount; }
            set
            {
                if (_selectedItemCount != value)
                {
                    _selectedItemCount = value.Substring(value.Length - 1);
                    GenerateTextBoxes();
                    OnPropertyChanged(nameof(SelectedItemCount));
                    
                }
            }
        }

        private bool isWaterChecked;
        public bool IsWaterChecked
        {
            get { return isWaterChecked; }
            set
            {
                if (isWaterChecked != value)
                {
                    isWaterChecked = value;
                    OnPropertyChanged(nameof(IsWaterChecked));
                }
            }
        }

        private bool isGasChecked;
        public bool IsGasChecked
        {
            get { return isGasChecked; }
            set
            {
                if (isGasChecked != value)
                {
                    isGasChecked = value;
                    OnPropertyChanged(nameof(IsGasChecked));
                }
            }
        }

        private bool isElectricChecked;
        public bool IsElectricChecked
        {
            get { return isElectricChecked; }
            set
            {
                if (isElectricChecked != value)
                {
                    isElectricChecked = value;
                    OnPropertyChanged(nameof(IsElectricChecked));
                }
            }
        }


        #endregion


        [Obsolete]
        public async void Add_Product()
        {           
            
            if (selectedOption == "Source")
            {
                if(ProductLink == null)
                {
                    return;
                }
                ProductDataScraping scrapData = new ProductDataScraping();
                Product product = await scrapData.ScrapData(ProductLink);

            }
            else
            {
                UtilityTypesList = GetCheckedUtilityTypes();
                ProductImgLinks = GetImgLinks();

                Product product = new Product
                {
                    UtilityTypes = UtilityTypesList,
                    Name = ProductName,
                    Description = ProductDescription,
                    FindDocLink = ProductFindDocLink,
                    Features = ProductFeatures,
                    DocLink = ProductDocLink,
                    ImgLinks = ProductImgLinks
                };
            }
            
           
        }

        public List<UtilityType> GetCheckedUtilityTypes()
        {
            List<UtilityType> checkedUtilityTypes = new List<UtilityType>();

            if (IsWaterChecked)
            {
                checkedUtilityTypes.Add(UtilityType.Water);
            }
            if (IsGasChecked)
            {
                checkedUtilityTypes.Add(UtilityType.Gas);
            }
            if (IsElectricChecked)
            {
                checkedUtilityTypes.Add(UtilityType.Electric);
            }

            return checkedUtilityTypes;
        }

        public List<string> GetImgLinks()
        {
            List<string> imgLinks = new List<string>();
            foreach (var imageTextBox in ImageTextBoxes)
            {
                string textBoxText = imageTextBox.TextBox.Text;
                imgLinks.Add(textBoxText);
            }
            return imgLinks;
        }

        public void GenerateTextBoxes()
        {
            ImageTextBoxes.Clear();

            for(int i=0; i< int.Parse(SelectedItemCount); i++)
            {
                TextBox textBox = new TextBox();
                String label = "Image Link " + (i + 1)+": ";
                textBox.Name = "textBox" + (i + 1);
                textBox.Width = 200;
                textBox.Margin = new System.Windows.Thickness(0, 5, 0, 5);
                ImageTextBox imageTextBox = new ImageTextBox
                {
                    Label = label,
                    TextBox = textBox,
                };

                ImageTextBoxes.Add(imageTextBox);
            }
        }


    }
    public class ImageTextBox
    {
        public string Label { get; set; }
        public TextBox TextBox { get; set; }
    }


}
