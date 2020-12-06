using Newtonsoft.Json;
using RQuote.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RQuote
{
    public class QuotationPageDataContext : ViewModelBase
    {
        [JsonIgnore]
        public PartSuggestionProvider suggestionProvider
        {
            get;
            set;
        }

        [JsonIgnore]
        public ObservableCollection<Product> PreviewProducts
        {
            get;
            set;
        }

        public ObservableCollection<QuoteLineItem> QuoteLines
        {
            get;
            set;
        }

        private CustomerDetails customerDetail;

        public CustomerDetails CustomerDetails
        {
            get { return customerDetail; }
            set
            {
                customerDetail = value;
                OnPropertyChanged("CustomerDetail");
            }
        }

        private string quotationNumber = null;
        public string QuotationNumber
        {
            get
            {
                if(quotationNumber == null)
                {
                    quotationNumber = string.Format("RQ/{0}/{1}", DateTime.Now.Date.ToString("dd/MM/yyyy"), Utils.GetQuoteNumber().ToString("D3"));
                }
                return quotationNumber;
            }
            set
            {
                quotationNumber = value;
                OnPropertyChanged("QuotationNumber");
            }
        }

        private string quotationDate = null;
        public string QuotationDate
        {
            get
            {
                if(quotationDate == null)
                {
                    quotationDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                }
                return quotationDate;
            }
            set
            {
                quotationDate = value;
                OnPropertyChanged("QuotationDate");
            }
        }

        private IList<object> _selectedQuoteLines;
        [JsonIgnore]
        public IList<object> SelectedQuoteLines
        {
            get
            {
                return _selectedQuoteLines;
            }
            set
            {
                _selectedQuoteLines = value;
                OnPropertyChanged("SelectedQuoteLines");
                OnPropertyChanged("IsClearSelectedVisible");
            }
        }
        [JsonIgnore]
        public Boolean IsAddToQuoteEnabled
        {
            get
            {
                return PreviewProducts.Count > 0;
            }
        }
        [JsonIgnore]
        public Visibility IsClearAllVisible
        {
            get
            {
                return QuoteLines.Count > 0 ? Visibility.Visible : Visibility.Hidden;
            }
        }
        [JsonIgnore]
        public Visibility IsClearSelectedVisible
        {
            get
            {
                return SelectedQuoteLines != null && SelectedQuoteLines.Count > 0 ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private bool _isChecked = false;
        [JsonIgnore]
        public bool IsHeaderCheckboxChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                var oldValue = _isChecked;
                _isChecked = value;
                if (value != oldValue)
                {
                    OnPropertyChanged("IsHeaderCheckboxChecked");
                }
            }
        }

        private Product _selectedProduct;
        [JsonIgnore]
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                if(quotationPage != null)
                {
                    quotationPage.IgnoreNewButtonClick = true;
                }
                AddPreviewProduct(_selectedProduct);
            }
        }

        private int _totalProductsInCatalogue = 0;
        [JsonIgnore]
        public int TotalProductsInCatalogue 
        {
            get
            {
                return _totalProductsInCatalogue;
            }
            set
            {
                _totalProductsInCatalogue = value;
                OnPropertyChanged("TotalProductsInCatalogue");
            }
        }
        
        [JsonIgnore]
        public QuotationPage quotationPage;
        public QuotationPageDataContext()
        {
            suggestionProvider = new PartSuggestionProvider();
            PreviewProducts = new ObservableCollection<Product>();
            QuoteLines = new ObservableCollection<QuoteLineItem>();
            CustomerDetails = new CustomerDetails();

            IgnoredPropertiesForChange.Add("SelectedProduct");
            IgnoredPropertiesForChange.Add("PreviewProducts");
            IgnoredPropertiesForChange.Add("suggestionProvider");
            IgnoredPropertiesForChange.Add("IsDirty");
        }

        private void AddPreviewProduct(Product product)
        {
            PreviewProducts.Clear();
            if (product != null)
            {
                PreviewProducts.Add(product);
            }
            OnPropertyChanged("IsAddToQuoteEnabled");
        }

        private QuoteLineItem GetExistingQuoteLine(QuoteLineItem quoteLineItem)
        {
            return (from item in QuoteLines where item.PartNo == quoteLineItem.PartNo select item).FirstOrDefault();
        }

        public void AddQuoteLine(QuoteLineItem quoteLineItem)
        {
            var existingLine = GetExistingQuoteLine(quoteLineItem);
            if (existingLine != null)
            {
                existingLine.Quantity = existingLine.Quantity + 1;
                quotationPage.quotationGrid.ScrollIntoView(existingLine);
            }
            else
            {
                QuoteLines.Add(quoteLineItem);
                OnPropertyChanged("IsClearAllVisible");
                quotationPage.quotationGrid.ScrollIntoView(quoteLineItem);
            }
            this.IsChanged = true;
            OnPropertyChanged("IsDirty");
        }

        public void ClearSelectedItems()
        {
            foreach (var item in SelectedQuoteLines.ToArray())
            {
                QuoteLines.Remove(item as QuoteLineItem);
            }
            IsHeaderCheckboxChecked = false;
            SelectedQuoteLines = null;
            this.IsChanged = true;
            OnPropertyChanged("QuoteLines");
            OnPropertyChanged("IsClearAllVisible");
            OnPropertyChanged("IsClearSelectedVisible");
            OnPropertyChanged("IsDirty");

        }
        public void ClearAllItems()
        {
            IsHeaderCheckboxChecked = false;
            QuoteLines.Clear();
            this.IsChanged = true;
            OnPropertyChanged("QuoteLines");
            OnPropertyChanged("IsClearAllVisible");
            OnPropertyChanged("IsClearSelectedVisible");
            OnPropertyChanged("IsDirty");
        }

        public void SaveQuotation(bool showAlertOnSuccess = false)
        {
            var str = JsonConvert.SerializeObject(this);
            string encryptedString = Utils.Encrypt(str);
            string savedQuotationFolder = Utils.SavedQuotationsPath;
            string filePath = Path.Combine(savedQuotationFolder, this.QuotationNumber.Replace("/", "_") + ".rquote");
            new Task(() =>
            {
                if (!Directory.Exists(savedQuotationFolder))
                {
                    Utils.CreateDirectory(savedQuotationFolder);
                }
 
                byte[] bytes = Encoding.ASCII.GetBytes(encryptedString);

                File.WriteAllBytes(filePath, bytes);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (showAlertOnSuccess)
                    {
                        MessageBox.Show("Quotation Saved Successfully.", "RQuote");
                    }
                    this.IsChanged = false;
                    this.CustomerDetails.AcceptChanges();
                    foreach(QuoteLineItem item in this.QuoteLines)
                    {
                        item.AcceptChanges();
                    }
                    OnPropertyChanged("IsDirty");
                }));
            }).Start();
        }
    }
}