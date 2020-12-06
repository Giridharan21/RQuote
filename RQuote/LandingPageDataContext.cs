using Newtonsoft.Json;
using RQuote.Logic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RQuote
{
    
    public class LandingPageDataContext : INotifyPropertyChanged
    {
        public bool HasSavedQuotations { get; set; }
        public bool ShouldShowNewQuotations { get; set; }
        public ObservableCollection<SavedQuotationModel> SavedQuotations { get; set; }
        List<SavedQuotationModel> existingQuotations = new List<SavedQuotationModel>();
        private string filterText = null;
        private DbAccess dbAccess;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FilterText
        {
            get
            {
                return filterText;
            }
            set
            {
                var oldValue = filterText;
                filterText = value;
                if(oldValue != filterText)
                {
                    UpdateFilter(filterText);
                }
            }
        }


        private readonly object TotalFileLocker = new object();
        private int _totalFiles = 0;
        public int TotalFiles
        {
            get
            {
                lock (TotalFileLocker)
                {
                    return _totalFiles;
                }
            }
            set
            {
                lock (TotalFileLocker)
                {
                    _totalFiles = value;
                    NotifyPropertyChanged("TotalFiles");
                }
            }
        }

        private readonly object DownloadedFileLocker = new object();
        private int _downloadedFiles = 0;
        public int DownloadedFiles
        {
            get
            {
                lock (DownloadedFileLocker)
                {
                    return _downloadedFiles;
                }
            }
            set
            {
                lock (DownloadedFileLocker)
                {
                    _downloadedFiles = value;
                    NotifyPropertyChanged("DownloadedFiles");
                }
            }
        }

        public LandingPageDataContext()
        {
            dbAccess = new DbAccess();
            ShouldShowNewQuotations = false;
            LoadSavedQuotations();
        }

        private void LoadSavedQuotations()
        {
            this.SavedQuotations = new ObservableCollection<SavedQuotationModel>();
            new Task(async () =>
            {
                try
                {
                    existingQuotations =await dbAccess.GetQuotations();
                }
                catch (Exception ex)
                {

                }

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if(existingQuotations.Count > 0)
                    {
                        foreach (var q in existingQuotations)
                        {
                            SavedQuotations.Add(q);
                        }
                        HasSavedQuotations = true;
                        NotifyPropertyChanged("HasSavedQuotations");
                    }
                    else
                    {
                        ShouldShowNewQuotations = true;
                        NotifyPropertyChanged("ShouldShowNewQuotations");
                    }
                }));
            }).Start();
            
        }

        private void UpdateFilter(string text)
        {
            this.SavedQuotations = new ObservableCollection<SavedQuotationModel>(from item in existingQuotations where item.QuotationCustomer.ToLower().Contains(text.ToLower()) select item);
            NotifyPropertyChanged("SavedQuotations");
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
