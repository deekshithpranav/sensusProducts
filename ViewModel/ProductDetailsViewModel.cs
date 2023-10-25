using sensusProducts.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using sensusProducts.Model;
using System.Windows;
using System.Windows.Controls;
using sensusProducts.Service;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;

namespace sensusProducts.ViewModel
{
    public class ProductDetailsViewModel : INotifyPropertyChanged
    {

        public ProductDetailsViewModel(Product product, HomePageViewModel homePageViewModel, Frame ViewProductFrame)
        {
            FindDocURLCommand = new RelayCommand(FindDocURL);
            ProductDocURLCommand = new RelayCommand(ProductDocURL);
            DeleteProductCommand = new RelayCommand(DeleteProduct);
            UpdateProductCommand = new RelayCommand(UpdateProduct);
            NavigateBack = new RelayCommand(GoBack);
            this.HomePageViewModel = homePageViewModel;
            this.product = product;
            this.ViewProductFrame = ViewProductFrame;
            InitializeParams();
        }

        #region Properties

        readonly Product product;

        Frame ViewProductFrame { get; set; }
        readonly ProductServices productServices = new ProductServices();
        public string FdURL { get; set; }
        public string PdURL { get; set; }

        private string productDescription;
        public string ProductDescription
        {
            get
            {
                return productDescription;
            }
            set
            {
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
            get { return productFeatures; }
            set
            {
                productFeatures = value;
                OnPropertyChanged(nameof(productFeatures));
            }
        }

        public bool _findDocLinkAbl = true;
        public bool FindDocLinkAbl
        {
            get
            {
                return _findDocLinkAbl;
            }
            set
            {
                _findDocLinkAbl = value;
                OnPropertyChanged(nameof(FindDocLinkAbl));
            }
        }

        public bool _downlaodDocLinkAbl = true;
        public bool DownloadDocLinkAbl
        {
            get
            {
                return _downlaodDocLinkAbl;
            }
            set
            {
                _downlaodDocLinkAbl = value;
                OnPropertyChanged(nameof(DownloadDocLinkAbl));
            }
        }

        public ICommand FindDocURLCommand { get; private set; }
        public ICommand ProductDocURLCommand { get; private set; }

        public ICommand NavigateBack { get; }

        HomePageViewModel HomePageViewModel { get; }

        public ICommand UpdateProductCommand { get; private set; }
        public ICommand DeleteProductCommand { get; private set; }

        readonly List<Image> ImageObjects = new List<Image>();

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
        public StackPanel UtilityList { get; set; }

        #endregion

        public void InitializeParams()
        {
            ProductName = product.Name;
            FdURL = product.FindDocLink;
            PdURL = product.DocLink;
            ProductDescription = product.Description;
            ProductFeatures = product.Features;
            ImageSources = new ObservableCollection<string>();

            if (FdURL == string.Empty)
            {
                FindDocLinkAbl = false;
            }

            if (PdURL is null || PdURL == string.Empty)
            {
                DownloadDocLinkAbl = false;
            }

            if(product.ImgLinks.Count != 0)
            {
                MainImageSource = product.ImgLinks[0];
            }
        }

        public void GoBack()
        {
            Dispose();
            HomePageViewModel.GoBackToMain();
        }

        public void DeleteProduct()
        {
            productServices.DeleteProductInDB(product.Id);
            GoBack();
            HomePageViewModel.RefreshProductsList();
        }

        private void UpdateProduct()
        {
            UpdateProductView updateProductView = new UpdateProductView();
            UpdateProductViewModel updateProductViewModel = new UpdateProductViewModel(product, HomePageViewModel);
            updateProductView.DataContext = updateProductViewModel;
            updateProductView.InitializeComponent();
            ViewProductFrame.Navigate(updateProductView);
        }

        // Generate utility list images
        public void GenerateUtilityListImages()
        {
            UtilityList.Orientation = Orientation.Horizontal;
            UtilityList.Margin = new Thickness(20, 20, 0, 10);
            string baseDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "..\\..\\"));
            foreach (UtilityType utilityType in product.UtilityTypes)
            {
                string utype = utilityType.ToString();
                Image utilityImage = new Image
                {
                    Source = new BitmapImage(new Uri($"{projectDirectory}Resources\\UtilityTypeImages\\{utype}.png")),
                    Height = 25
                };
                UtilityList.Children.Add(utilityImage);
            }
        }

        // Gallery
        public void GenerateGallery()
        {
            //dont run if ImgLinks does not have any images stored.
            if(product.ImgLinks.Count == 0) { return; }

            foreach (var item in product.ImgLinks)
            {
                if (product.ImgLinks.IndexOf(item) == 0)
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
                else if (product.ImgLinks.IndexOf(item) == product.ImgLinks.Count - 1)
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

        public void FindDocURL()
        {
            if (!string.IsNullOrEmpty(FdURL))
            {
                System.Diagnostics.Process.Start(FdURL);
            }
        }

        public void ProductDocURL()
        {
            if (!string.IsNullOrEmpty(PdURL))
            {
                System.Diagnostics.Process.Start(PdURL);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
