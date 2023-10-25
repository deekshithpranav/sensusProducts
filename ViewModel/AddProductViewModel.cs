using sensusProducts.Model;
using sensusProducts.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using sensusProducts.ViewModel.Commands;
using System.Data.SqlClient;

namespace sensusProducts.ViewModel
{
    public class AddProductViewModel : INotifyPropertyChanged
    {
        // Constructor
        public AddProductViewModel()
        {
            // Initialize productService and addProductCommand
            productService = new ProductServices();
            AddProductWithLinkCommand = new AddproductCommand(AddProductWithLink);
            AddProductManuallyCommand = new AddproductCommand(AddProductManually);
            IsSubmitButtonEnabled = false;
            ImageTextBoxes = new ObservableCollection<ImageTextBox>();
        }

        #region Properties

        // Event handler for property change notifications
        public event PropertyChangedEventHandler PropertyChanged;

        // Command for adding a product
        public ICommand AddProductWithLinkCommand { get; }

        // Command for adding a product
        public ICommand AddProductManuallyCommand { get; }

        // Service for managing product data
        private readonly IProductService productService;

        // Selected option for product addition (e.g., 'Source' or 'Manual')
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

        // URL or source link for adding products
        private string productLink;

        public string ProductLink
        {
            get { return productLink; }
            set
            {
                if (productLink != value)
                {
                    productLink = value;
                    // Update IsSubmitButtonEnabled based on text in ProductLink
                    IsSubmitButtonEnabled = !string.IsNullOrEmpty(productLink);
                    OnPropertyChanged(nameof(ProductLink));
                }
            }
        }

        // Flag to enable or disable the submit button
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

        // Product-related properties (e.g., name, description, etc.)
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

        #endregion

        // Method for handling property change events
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Methods

        // Method for adding a product
        public async void AddProductWithLink()
        {
            if (selectedOption == "Source")
            {
                if (ProductLink == null)
                {
                    // Display an error message for an invalid URL
                    MessageBox.Show("Invalid URL", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    // Scrape product data from the source URL
                    ProductDataScraping scrapData = new ProductDataScraping();
                    Product product = await scrapData.ScrapData(ProductLink);

                    // Add the scraped product to the service
                    if (product != null)
                    {
                        productService.AddProduct(product);
                        MessageBox.Show("Product added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions, e.g., invalid URL
                    MessageBox.Show("Invalid URL: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                ProductLink = string.Empty;
            }
        }

        void AddProductManually()
        {
            // Get selected utility types and image links
            UtilityTypesList = GetCheckedUtilityTypes();
            ProductImgLinks = GetImgLinks();

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=sensus;Trusted_Connection=True;";
            string query = @"
                    SELECT TOP 1 PID
                    FROM productIDs
                    ORDER BY PID DESC;
                ";
            int productID = 0;
            try
            {
                // Create a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a SqlCommand
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Execute the SQL command to create the table
                        object str = command.ExecuteScalar();
                        productID = (int)str + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            // Create a new product object
            Product product = new Product
            {
                Id = productID,
                UtilityTypes = UtilityTypesList,
                Name = ProductName,
                Description = ProductDescription,
                FindDocLink = ProductFindDocLink,
                Features = ProductFeatures,
                DocLink = ProductDocLink,
                ImgLinks = ProductImgLinks
            };

            // SQL query to check if the title exists in the database
            query = "SELECT COUNT(*) FROM productIDs WHERE PName = @Title";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Title", product.Name);
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // Product is already present in the database
                            System.Windows.Forms.MessageBox.Show("Product is already present in the database");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Debug.WriteLine("Error: " + ex.Message);
            }

            // Add the manual product to the service
            productService.AddProduct(product);

            //reset the entries to add new product
            UtilityTypesList = null;
            ProductImgLinks = null;
            ProductName = string.Empty;
            ProductDescription = string.Empty;
            ProductFindDocLink = string.Empty;
            ProductFeatures = null;
            ProductDocLink = string.Empty;

            // Show a success message
            MessageBox.Show("Product added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        
        }

        // Get a list of checked utility types
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

        // Generate text boxes based on the selected item count
        public void GenerateTextBoxes()
        {
            ImageTextBoxes.Clear();

            for (int i = 0; i < int.Parse(SelectedItemCount); i++)
            {
                TextBox textBox = new TextBox();
                String label = "Image Link " + (i + 1) + ": ";
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

        #endregion
    }

    // Helper class for image text boxes
    public class ImageTextBox
    {
        public string Label { get; set; }
        public TextBox TextBox { get; set; }
    }
}
