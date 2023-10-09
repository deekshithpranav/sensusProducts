using sensusProducts.Model;
using sensusProducts.Service;
using sensusProducts.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        #region Properties

        public ICommand OpenAddProductPageCommand { get; }

        public ScrollViewer ScrollViewer { get; }

        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        private IProductService productService;

        public HomePageViewModel(ScrollViewer scrollViewer)
        {
            OpenAddProductPageCommand = new RelayCommand(OpenAddProductWindow);
            productService = new ProductServices();
            productService.LoadProductPage();
            ScrollViewer = scrollViewer;
            ScrollViewer.Content = new StackPanel();

            ((StackPanel)scrollViewer.Content).Children.Add(GenerateProductList());
            ((StackPanel)scrollViewer.Content).Children.Add(GenerateProductList());
            ((StackPanel)scrollViewer.Content).Children.Add(GenerateProductList());
            ((StackPanel)scrollViewer.Content).Children.Add(GenerateProductList());
            ((StackPanel)scrollViewer.Content).Children.Add(GenerateProductList());
            ((StackPanel)scrollViewer.Content).Children.Add(GenerateProductList());
            ((StackPanel)scrollViewer.Content).Children.Add(GenerateProductList());
            ((StackPanel)scrollViewer.Content).Children.Add(GenerateProductList());
        } 

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            }
        }

        private void OpenAddProductWindow()
        {
            // Create and show the "AddProductView" window
            var addProductWindow = new AddProductView();
            addProductWindow.InitializeComponent();
            addProductWindow.Show();

        }

        private Grid GenerateProductList()
        {
            Grid MainGrid = new Grid();

            ColumnDefinition ImageColumn = new ColumnDefinition();
            ColumnDefinition ProductTitleColumn = new ColumnDefinition();
            MainGrid.ColumnDefinitions.Add(ImageColumn);
            MainGrid.ColumnDefinitions.Add(ProductTitleColumn);

            TextBlock Image = new TextBlock();
            TextBlock Title = new TextBlock();
            Image.Background = Brushes.Red;
            Image.Height = 150;
            Title.Text = "Product Title";
            Title.Height = 100;
            Title.Background = Brushes.Blue;

            TextBlock Type = new TextBlock();
            Type.Background = Brushes.Green;
            Type.Text = "Utility Type";
            Type.Height = 50;

            StackPanel ProductDescription = new StackPanel();
            ProductDescription.Children.Add(Title);
            ProductDescription.Children.Add(Type);

            Grid.SetColumn(Image, 0);
            MainGrid.Children.Add(Image);
            Grid.SetColumn(ProductDescription, 1);
            MainGrid.Children.Add(ProductDescription);

            return MainGrid;
        }

    }
    
}
