using sensusProducts.Model;
using sensusProducts.Service;
using sensusProducts.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace sensusProducts.ViewModel
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        public HomePageViewModel(ScrollViewer scrollViewer)
        {
            OpenAddProductPageCommand = new RelayCommand(OpenAddProductWindow);
            refreshProductList = new RelayCommand(() => GenerateProductList(scrollViewer));
            productService = new ProductServices();
            productService.LoadProductPage();
            ScrollViewer = scrollViewer;
            ScrollViewer.Content = new StackPanel();

            GenerateProductList(scrollViewer);
        }

        #region Properties

        public ICommand OpenAddProductPageCommand { get; }

        public ICommand refreshProductList { get; }

        public ScrollViewer ScrollViewer { get; }

        #endregion

        #region Methods

        private void OpenAddProductWindow()
        {
            // Create and show the "AddProductView" window
            var addProductWindow = new AddProductView();
            addProductWindow.InitializeComponent();
            addProductWindow.Show();

        }

        private Border GenerateGridElement(Product product)
        {
            Grid MainGrid = new Grid();
            ColumnDefinition ImageColumn = new ColumnDefinition();
            ColumnDefinition ProductTitleColumn = new ColumnDefinition();
            MainGrid.ColumnDefinitions.Add(ImageColumn);
            MainGrid.ColumnDefinitions.Add(ProductTitleColumn);

            TextBlock Title = new TextBlock();

            // Create an Image control
            Image productImage = new Image();

            // Set the source of the Image control to your image path
            productImage.Source = new BitmapImage(new Uri(product.ImgLinks[0]));

            // Set other properties for the image, such as height
            productImage.Height = 150;

            // Add the Image control to your Grid
            Grid.SetColumn(productImage, 0);

            Title.Text = product.Name;
            Title.Height = 100;
            Title.Background = Brushes.Blue;

            TextBlock Type = new TextBlock();
            Type.Background = Brushes.Green;
            Type.Text = product.UtilityTypes.ToString();
            Type.Height = 50;

            Button button = new Button();
            button.Content = "View Product";
            button.Click += (sender, e) => View_Product(sender, e, product);

            StackPanel ProductDescription = new StackPanel();
            ProductDescription.Children.Add(Title);
            ProductDescription.Children.Add(Type);
            ProductDescription.Children.Add(button);

            Grid.SetColumn(productImage, 0);
            MainGrid.Children.Add(productImage);
            Grid.SetColumn(ProductDescription, 1);
            MainGrid.Children.Add(ProductDescription);
            MainGrid.Margin = new Thickness(0, 0, 0, 10);

            Border border = new Border();
            border.BorderBrush = Brushes.Gray;
            border.BorderThickness = new Thickness(1);
            border.Padding = new Thickness(5);
            border.Child = MainGrid;

            return border;
        }

        private void GenerateProductList(ScrollViewer scrollViewer)
        {
            if (scrollViewer.Content is Panel panel)
            {
                panel.Children.Clear();
            }

            List<Product> products = productService.LoadProductPage();

            foreach (Product product in products)
            {
                ((StackPanel)scrollViewer.Content).Children.Add(GenerateGridElement(product));
            }
        }

        private void View_Product(object sender, RoutedEventArgs e, Product product)
        {
            Debug.Write(product.Id);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private IProductService productService;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            }
        }
    }
    
}
