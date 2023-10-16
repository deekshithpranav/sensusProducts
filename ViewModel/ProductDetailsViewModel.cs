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
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace sensusProducts.ViewModel
{
    public class ProductDetailsViewModel:INotifyPropertyChanged
    {

        public ProductDetailsViewModel()
        {

        }

        public ProductDetailsViewModel(Product product, HomePageViewModel homePageViewModel, Frame ViewProductFrame)
        {
            FindDocURLCommand = new RelayCommand(FindDocURL);
            ProductDocURLCommand = new RelayCommand(ProductDocURL);
            DeleteProductCommand = new RelayCommand(deleteProduct);
            UpdateProductCommand = new RelayCommand(updateProduct);
            NavigateBack = new RelayCommand(goBack);
            this.homePageViewModel = homePageViewModel;
            this.product = product;
            this.ViewProductFrame = ViewProductFrame;
            initializeParams();
        }



        #region Properties

        Product product;


        Frame ViewProductFrame { get; set; }
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

        public ICommand UpdateProductCommand { get; private set; }
        public ICommand DeleteProductCommand { get; private set; }

        List<Image> ImageObjects = new List<Image>();

        private ObservableCollection<string> _imageSources;

        public ObservableCollection<string> ImageSources
        {
            get { return _imageSources; }
            set
            {
                if (_imageSources != value)
                {
                    _imageSources = value;
                    OnPropertyChanged(nameof(ImageSources));
                }
            }
        }

        private string _mainImageSource;

        public string MainImageSource
        {
            get { return _mainImageSource; }
            set
            {
                if (_mainImageSource != value)
                {
                    _mainImageSource = value;
                    OnPropertyChanged(nameof(MainImageSource));
                }
            }
        }

        public StackPanel ProductsGallery { get; set; }
        #endregion


        public void initializeParams()
        {
            ProductName = product.Name;
            fdURL = product.FindDocLink;
            pdURL = product.DocLink;
            ProductDescription = product.Description;
            ProductFeatures = product.Features;
            ImageSources = new ObservableCollection<string>();
            
            MainImageSource = product.ImgLinks[0];
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

        private void updateProduct()
        {
           UpdateProductView updateProductView = new UpdateProductView();
           UpdateProductViewModel updateProductViewModel = new UpdateProductViewModel(product, homePageViewModel);
           updateProductView.DataContext = updateProductViewModel;
            updateProductView.InitializeComponent();
            ViewProductFrame.Navigate(updateProductView);
        }



        //gallery
        public void GenerateGallery()
        {
            List<string> Images = new List<string>() { "https://sensusglobal.wpenginepowered.com/wp-content/uploads/EasyLink_FrontTop.jpg", "https://sensusglobal.wpenginepowered.com/wp-content/uploads/ally-water-meter-right-side-sensus.jpg", "https://sensusglobal.wpenginepowered.com/wp-content/uploads/sensus-auto-adjust-turbo-meter-product-2.jpg", "https://sensusglobal.wpenginepowered.com/wp-content/uploads/fieldlogic-hand-held-device-product.jpg" };

            

            foreach (var item in product.ImgLinks)
            {
                if (Images.IndexOf(item) == 0)
                {
                    Image image = new Image()
                    {
                        Height = 60,
                        Margin = new Thickness(20, 0, 10, 0),
                        Source = new BitmapImage(new Uri(item)),
                        Opacity = 1,
                    };

                    ImageObjects.Add(image);
                    image.MouseLeftButtonDown += ChangeOpacity;
                    image.MouseLeftButtonDown += ShowImage;
                    ProductsGallery.Children.Add(image);
                }
                else if (Images.IndexOf(item) == Images.Count - 1)
                {
                    Image image = new Image()
                    {
                        Height = 60,
                        Margin = new Thickness(10, 0, 20, 0),
                        Source = new BitmapImage(new Uri(item)),
                        Opacity = 0.5
                    };

                    ImageObjects.Add(image);
                    image.MouseLeftButtonDown += ChangeOpacity;
                    image.MouseLeftButtonDown += ShowImage;
                    ProductsGallery.Children.Add(image);
                }
                else
                {
                    Image image = new Image()
                    {
                        Height = 60,
                        Margin = new Thickness(10, 0, 10, 0),
                        Source = new BitmapImage(new Uri(item)),
                        Opacity = 0.5
                    };

                    ImageObjects.Add(image);
                    image.MouseLeftButtonDown += ChangeOpacity;
                    image.MouseLeftButtonDown += ShowImage;
                    ProductsGallery.Children.Add(image);
                }
            }
            ImageObjects.FirstOrDefault().Opacity = 1;

        }

        private void ShowImage(object sender, MouseButtonEventArgs e)
        {
            string uri = string.Empty;

            if (sender is Image image)
            {
                uri = image.Source.ToString();
                image.Opacity = 1;
            }

            MainImageSource = uri;
        }

        private void ChangeOpacity(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in ImageObjects)
            {
                item.Opacity = 0.5;
            }
        }

        public void Dispose()
        {
            foreach (var item in ImageObjects)
            {
                item.MouseLeftButtonDown -= ShowImage;
                item.MouseLeftButtonDown -= ChangeOpacity;
            }
        }






        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
