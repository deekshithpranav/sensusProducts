using sensusProducts.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using sensusProducts.Service;
using System.Windows;

namespace sensusProducts.ViewModel
{
    public class UpdateProductViewModel : INotifyPropertyChanged
    {
        public UpdateProductViewModel(Product Product, HomePageViewModel HomePageViewModel)
        {
            this.Product = Product;
            this.HomePageViewModel = HomePageViewModel;
            InitializeParams();
        }

        #region Properties

        public Product Product { get; set; }
        HomePageViewModel HomePageViewModel { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductFeatures { get; set; }
        public string ProductFindDocLink { get; set; }
        public string ProductDocLink { get; set; }
        public List<string> ProductImgLinks { get; set; }
        public ObservableCollection<ImageTextBox> ImageTextBoxes { get; set; }
        public List<UtilityType> UtilityTypesList { get; set; }

        // Selected item count for generating text boxes
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

        // Flags for checking utility types (e.g., Water, Gas, Electric)
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

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand submitProductCommand { get; set; }
        IProductService productService;
        public ICommand NavigateBack { get; private set; }

        #endregion

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void GenerateTextBoxes()
        {
            ImageTextBoxes.Clear();

            for (int i = 0; i < int.Parse(SelectedItemCount); i++)
            {
                TextBox textBox = new TextBox();
                String label = "Image Link " + (i + 1) + ": ";
                textBox.Name = "textBox" + (i + 1);
                textBox.Width = 500;
                textBox.Margin = new Thickness(0, 10, 0, 5);
                textBox.SetResourceReference(TextBox.StyleProperty, "TextBoxStyle");
                textBox.HorizontalAlignment = HorizontalAlignment.Left;
                if (i < Product.ImgLinks.Count)
                {
                    textBox.Text = Product.ImgLinks[i];
                }
                ImageTextBox imageTextBox = new ImageTextBox
                {
                    Label = label,
                    TextBox = textBox,
                };

                ImageTextBoxes.Add(imageTextBox);
            }
        }

        private void InitializeParams()
        {
            ProductName = Product.Name;
            ProductDescription = Product.Description;
            ProductFeatures = Product.Features;
            ProductDocLink = Product.DocLink;
            ProductFindDocLink = Product.FindDocLink;
            ImageTextBoxes = new ObservableCollection<ImageTextBox>();
            SelectedItemCount = Product.ImgLinks.Count.ToString();
            GenerateTextBoxes();
            if (Product.UtilityTypes.Contains(UtilityType.Water))
            {
                IsWaterChecked = true;
            }
            if (Product.UtilityTypes.Contains(UtilityType.Electric))
            {
                IsElectricChecked = true;
            }
            if (Product.UtilityTypes.Contains(UtilityType.Gas))
            {
                IsGasChecked = true;
            }
            submitProductCommand = new RelayCommand(updateProduct);
            NavigateBack = new RelayCommand(goBack);
            productService = new ProductServices();
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

        // Get a list of image links from text boxes
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

        public void goBack()
        {
            HomePageViewModel.RefreshProductsList();
            HomePageViewModel.GoBackToMain();
        }

        public void updateProduct()
        {
            // Get selected utility types and image links
            UtilityTypesList = GetCheckedUtilityTypes();
            ProductImgLinks = GetImgLinks();

            // Create a new product object
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

            try
            {
                productService.UpdateProduct(product, Product.Id);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Information: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            MessageBox.Show("Product updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
