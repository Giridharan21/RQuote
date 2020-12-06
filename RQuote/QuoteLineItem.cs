using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Windows.Foundation.Metadata;

namespace RQuote
{
    public class QuoteLineItem : ViewModelBase
    {
        private int _qty = 1;
        
        public string PartNo
        {
            get;
            set;
        }
        public string Image
        {
            get
            {
                return this.product != null ? this.product.Image : null;
            }
        }
        [JsonIgnore]
        public string ShortDetails
        {
            get
            {
                return String.IsNullOrEmpty(this.ProductDetails) ? "" : this.ProductDetails.Replace("\n", "");
            }
        }
        public string ProductDetails
        {
            get;
            set;
        }
        public string Unit
        {
            get;
            set;
        }
        public int Quantity
        {
            get
            {
                return _qty;
            }
            set
            {
                _qty = value >= 1 ? value : 1;
                CalculateTotal();
                OnPropertyChanged("Quantity");
                OnPropertyChanged("Total");
            }
        }
        //Price is UnitPrice
        private double price;
        public double Price
        {
            get
            {
                return price;
            }
            set
            {
                price = value >= 0 ? Math.Round(value,2) : 0;
                CalculateTotal();
                OnPropertyChanged("Price");
                OnPropertyChanged("Total");
            }
        }
        private double discount;
        public double Discount
        {
            get
            {
                return discount;
            }
            set
            {
                discount = value >= 0 ? (value > 100 ? 100 : Math.Round(value, 2)) : 0;
                CalculateTotal();
                OnPropertyChanged("Discount");
                OnPropertyChanged("Total");
            }
        }

        private double amount;
        public double Amount
        {
            get
            {
                return amount;
            }
            private set
            {
                amount = Math.Round(value, 2);
                OnPropertyChanged("Amount");
            }
        }
        public List<double> GST
        {
            get;
            set;
        }
        private double gstAmount;
        public double GSTAmount
        {
            get
            {
                return gstAmount;
            }
            private set
            {
                gstAmount = Math.Round(value, 2);
                OnPropertyChanged("GSTAmount");
            }
        }
        private double total;
        public double Total
        {
            get
            {
                return total;
            }
            private set
            {
                total = Math.Round(value, 2);
                OnPropertyChanged("Total");
            }
        }

        private double selectedGST;
        public string SelectedGST
        {
            get
            {
                return selectedGST + "";
            }
            set
            {
                selectedGST = Double.Parse(value.Replace("%", ""));
                CalculateTotal();
            }
        }

        private bool _isChecked = false;
        [JsonIgnore]
        public bool IsChecked
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
                    OnPropertyChanged("IsChecked");
                }
            }
        }

        public Product product;

        private List<string> gstPercents = new List<string> {
   "12",
   "18"
  };
        [JsonIgnore]
        public List<string> GSTPercent
        {
            get
            {
                return gstPercents;
            }
        }
        public QuoteLineItem(Product product)
        {
            this.PartNo = product.ModelNo;
            this.ProductDetails = product.Details;
            this.Quantity = 1;
            this.product = product;
            this.Price = product.Price;
            selectedGST = 12;
        }

        public void CalculateTotal()
        {
            double total = 0;
            if (Price > 0 && Quantity > 0)
            {
                total = Amount = Price * Quantity;
                CalculateGSTAmount();
                if (Discount > 0)
                {
                    total -= (total * Discount) / 100;
                }
                total += GSTAmount;
            }
            Total = total;
        }

        private void CalculateGSTAmount()
        {
            double total = 0;
            if (Price > 0 && Quantity > 0)
            {
                total = Amount = Price * Quantity;
                if (Discount > 0)
                {
                    total -= (total * Discount) / 100;
                }
                total = (total * selectedGST) / 100;
            }

            GSTAmount = total;
        }
    }
}