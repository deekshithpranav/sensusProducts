using sensusProducts.Model;
using sensusProducts.Service;
using sensusProducts.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace sensusProducts.ViewModel
{
    // ViewModel responsible for the home page functionality
    public class HomePageViewModel : INotifyPropertyChanged
    {
        public HomePageViewModel(ScrollViewer ScrollViewer, Frame viewProductFrame, Grid MainGrid)
        {
            // Initialize commands and services
            this.ScrollViewer = ScrollViewer;
            OpenAddProductPageCommand = new RelayCommand(OpenAddProductWindow);
            RefreshProductListCommand = new RelayCommand(() => RefreshProductsList());
            productService = new ProductServices();
            productService.LoadProductsFromDB();
            ScrollViewer.Content = new StackPanel();
            ViewProductFrame = viewProductFrame;
            this.MainGrid = MainGrid;
            // Generate the initial product list after getting the list from DB
            products = productService.LoadProductsFromDB();
            GenerateProductList();
        }

        public HomePageViewModel()
        {
            productService = new ProductServices();
        }

        #region Properties

        public ICommand OpenAddProductPageCommand { get; }

        public ICommand RefreshProductListCommand { get; }

        public Frame ViewProductFrame;

        public Grid MainGrid;

        public ICommand GasFilter { get; }

        public ICommand ElectricFilter { get; }

        public ScrollViewer ScrollViewer { get; }

        private string searchProduct;

        List<Product> products;

        public string SearchProduct
        {
            get { return searchProduct; }
            set
            {
                searchProduct = value;
                OnPropertyChanged(nameof(SearchProduct));
                findProducts();
            }
        }

        //filter properties for IsChecked
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
                    GenerateWaterProducts();
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
                    GenerateWaterProducts();
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
                    OnPropertyChanged(nameof(isElectricChecked));
                    GenerateWaterProducts();
                }
            }
        }


        private bool isVisible;
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }


        #endregion

        #region Methods

        private void OpenAddProductWindow()
        {
            // Create and show the "AddProductView" window
            var addProductWindow = new AddProductView();
            addProductWindow.InitializeComponent();
            addProductWindow.Closing += AddProductWindow_Closing;
            addProductWindow.Show();

        }

        private void AddProductWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RefreshProductsList();
        }

        private Border GenerateGridElement(Product product)
        {
            // Create a grid for displaying product information
            Grid MainGrid = new Grid();
            ColumnDefinition ImageColumn = new ColumnDefinition();
            ColumnDefinition ProductTitleColumn = new ColumnDefinition();
            MainGrid.ColumnDefinitions.Add(ImageColumn);
            MainGrid.ColumnDefinitions.Add(ProductTitleColumn);

            // Create a TextBlock for the product title
            TextBlock Title = new TextBlock();
            Title.Text = product.Name;

            // Create an Image control for displaying the product image
            Image productImage = new Image();
            if(product.ImgLinks.Count >0 )
            {
                productImage.Source = new BitmapImage(new Uri(product.ImgLinks[0]));
            }
            productImage.Height = 150;

            // Create a TextBlock for the product type
            StackPanel utypePanel = new StackPanel();
            utypePanel.Orientation = Orientation.Horizontal;
            utypePanel.Margin = new Thickness(0, 0, 0, 10);
            string baseDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "..\\..\\"));
            foreach (UtilityType utilityType in product.UtilityTypes)
            {
                string utype = utilityType.ToString();
                Image utilityImage = new Image();
                utilityImage.Source = new BitmapImage(new Uri($"{projectDirectory}Resources\\UtilityTypeImages\\{utype}.png"));
                utilityImage.Height = 20;
                utypePanel.Children.Add( utilityImage );
            }

            //Another Grid to Split the 2nd column into two rows
            Grid SubGrid = new Grid();
            RowDefinition productNameRow = new RowDefinition();
            RowDefinition UtilityTypesRow = new RowDefinition();
            SubGrid.RowDefinitions.Add(productNameRow);
            SubGrid.RowDefinitions.Add(UtilityTypesRow);

            //Add to SubGrid
            Grid.SetRow(Title, 0);


            // Create a button to view product details
            Button button = new Button();
            button.Content = "View Product";
            button.Click += (sender, e) => View_Product(sender, e, product);

            // Create a stack panel for product description
            StackPanel UTypeAndButton = new StackPanel();
            UTypeAndButton.Children.Add(utypePanel);
            UTypeAndButton.Children.Add(button);

            Grid.SetRow(UTypeAndButton, 1);
            SubGrid.Children.Add(Title);
            SubGrid.Children.Add(UTypeAndButton);


            // Add elements to the grid
            Grid.SetColumn(productImage, 0);
            MainGrid.Children.Add(productImage);
            Grid.SetColumn(SubGrid, 1);
            MainGrid.Children.Add(SubGrid);
            MainGrid.Margin = new Thickness(0, 0, 0, 10);

            UTypeAndButton.VerticalAlignment = VerticalAlignment.Bottom;
            UTypeAndButton.HorizontalAlignment = HorizontalAlignment.Right;
            // Create a border for the grid element
            Border border = new Border();
            border.BorderBrush = Brushes.Gray;
            border.BorderThickness = new Thickness(1);
            border.Padding = new Thickness(5);
            border.Child = MainGrid;

            return border;
        }

        private void GenerateProductList()
        {
            if (ScrollViewer.Content is Panel panel)
            {
                panel.Children.Clear();
            }

            
            // Add product elements to the scroll viewer
            foreach (Product product in products)
            {
                ((StackPanel)ScrollViewer.Content).Children.Add(GenerateGridElement(product));
            }
        }

        public void RefreshProductsList()
        {
            products = productService.LoadProductsFromDB();
            GenerateProductList();
        }

        //initially populate with all the products in database
       
        private void findProducts()
        {
            List<Product> matchingProductList = new List<Product>();
            foreach (Product product in products)
            {
                string str = product.Name;
                if (str.ToLower().StartsWith(SearchProduct.ToLower()))
                {
                    matchingProductList.Add(product);
                }
            }
            List<Product> tempProductList = products;
            products = matchingProductList;
            Debug.WriteLine("Invoked");
            GenerateProductList();
            products = tempProductList;
        }

        private void View_Product(object sender, RoutedEventArgs e, Product product)
        {
            // Handle the click event for viewing product details


            // Create a new window for displaying the product details
            ProductDetailsView productDetailsView = new ProductDetailsView(product, this, ViewProductFrame);

            ViewProductFrame.Navigate(productDetailsView);

            MainGrid.Visibility = Visibility.Hidden;
        }

        //filter the product list by checkbox commands

        //water filter
        public void GenerateWaterProducts()
        {
            if(IsWaterChecked || IsGasChecked || IsElectricChecked){
                List<UtilityType> checkedTypes = new List<UtilityType>();
                if (IsWaterChecked) checkedTypes.Add(UtilityType.Water);
                if (IsGasChecked) checkedTypes.Add(UtilityType.Gas);
                if (IsElectricChecked) checkedTypes.Add(UtilityType.Electric);


                List<Product> matchingProductList = new List<Product>();
                int flag = 1;
                foreach (Product product in products)
                {
                    foreach (UtilityType checkedType in checkedTypes)
                    {
                        if (!product.UtilityTypes.Contains(checkedType))
                        {
                            flag = 0;
                            break;
                        }
                    }
                    if (flag == 1)
                    {
                        matchingProductList.Add(product);
                    }
                    flag = 1;
                }


                List<Product> tempProductList = products;
                products = matchingProductList;
                Debug.WriteLine("Invoked water");
                GenerateProductList();
                products = tempProductList;
            }
        
            else
            {
                GenerateProductList();
            }
        }

        public void GoBackToMain()
        {
            // Navigate back to the previous view
            ViewProductFrame.Content = null;
            MainGrid.Visibility = Visibility.Visible; // Show the main grid again
            
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private IProductService productService;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
