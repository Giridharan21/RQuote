using System.IO;
using System.Windows.Media.Imaging;

namespace RQuote
{
    public class Product : ViewModelBase
    {
        public string ModelNo { get; set; }
        private string _imagePath;
        private BitmapImage productImage;

        public BitmapImage ProductImage
        {
            get { return productImage; }
            set
            {
                productImage = value;
                OnPropertyChanged("ProductImage");
            }
        }
        public string Image
        {
            get
            {
                if(_imagePath == null)
                {
                    _imagePath = this.catalogue != null && this.catalogue.ImageFolderPath != null ? Path.Combine(this.catalogue.ImageFolderPath, this.ModelNo + ".jpg") : null;

                }
                return _imagePath;
            }
            set
            {
                _imagePath = value;
                OnPropertyChanged("Image");
            }
        }
        
        public string Details { get; set; }
        public double Price { get; set; }
        public byte[] imageData { get; set; }

        private Catalog catalogue;
        public int CatalogueId { get; set; }

        public Product(Catalog c):this()
        {
            this.catalogue = c;
        }

        public Product()
        {
            LoadImage();
        }
        private  void LoadImage()
        {
            if (imageData == null || imageData.Length == 0) 
                return ;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            productImage= image;
        }
    }
}
