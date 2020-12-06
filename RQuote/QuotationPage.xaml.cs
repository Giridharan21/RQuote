using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;


namespace RQuote
{
    /// <summary>
    /// Interaction logic for QuotationPage.xaml
    /// </summary>
    public partial class QuotationPage : Page
    {
        List<Catalog> listOfCatalogues = new List<Catalog>();
        ExcelManager excelManager = new ExcelManager();
        QuotationPageDataContext dataContext = new QuotationPageDataContext();
        List<Product> customProducts;
        public QuotationPage()
        {
            InitializeComponent();
            DoInitialSetup();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.dataContext.IsChanged)
            {
                var result = MessageBox.Show("You have unsaved changes. Are you sure?", "RQoute", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void QuotationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Closing += MainWindow_Closing;
            (Application.Current.MainWindow as MainWindow).OnShortcutKeyPress += QuotationPage_OnShortcutKeyPress;
        }

        private void QuotationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Closing -= MainWindow_Closing;
                (Application.Current.MainWindow as MainWindow).OnShortcutKeyPress -= QuotationPage_OnShortcutKeyPress;
            }
        }

        public QuotationPage(string existingQuotationPath)
        {
            InitializeComponent();
            var encryptedText = File.ReadAllText(existingQuotationPath);
            var decryptedText = Utils.Decrypt(encryptedText);
            this.dataContext = JsonConvert.DeserializeObject<QuotationPageDataContext>(decryptedText);

            DoInitialSetup();
        }

        #region private methods

        void DoInitialSetup()
        {
            this.Loaded += QuotationPage_Loaded;
            this.Unloaded += QuotationPage_Unloaded;
            this.DataContext = dataContext;
            dataContext.quotationPage = this;
            LoadQuotations();
        }

        
        void LoadQuotations()
        {
            var allCatalogDirectories = Directory.GetDirectories(Utils.CataloguesFolderPath);
            foreach (var folderPath in allCatalogDirectories)
            {
                var filePath = Path.Combine(folderPath, Path.GetFileName(folderPath) + ".xlsx");
                listOfCatalogues.Add(new Catalog() { FilePath = filePath, FolderPath = folderPath });
            }
            catalogueSelector.ItemsSource = listOfCatalogues;
            catalogueSelector.SelectedIndex = 0;
        }

        void LoadCatalogue(Catalog catalog)
        {
            catalogueLoadingMask.Visibility = Visibility.Visible;
            Keyboard.ClearFocus();
            Task.Delay(50).ContinueWith(_ =>
            {
                var allProducts = excelManager.LoadProductsForCatalog(catalog);
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    dataContext.suggestionProvider.ListOfParts.Clear();
                    dataContext.suggestionProvider.ListOfParts.AddRange(allProducts);
                    dataContext.TotalProductsInCatalogue = allProducts.Count;
                    customProducts = CustomProductManager.LoadCustomCatalogue();
                    dataContext.suggestionProvider.ListOfParts.AddRange(customProducts);
                    catalogueLoadingMask.Visibility = Visibility.Collapsed;
                    partSearchBox.Focus();
                }));
            });
        }

        #endregion

        #region Event Handlers
        private void catalogueSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCatalogue((sender as ComboBox).SelectedItem as Catalog);
        }

        private void addToQuoteButton_Click(object sender, RoutedEventArgs e)
        {
            dataContext.AddQuoteLine(new QuoteLineItem(dataContext.SelectedProduct));
            partSearchBox.Text = "";
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as DataGrid).SelectedItem = null;
            //dataContext.SelectedQuoteLines = (sender as DataGrid).SelectedItems as IList<object>;
        }


        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure to delete all items?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                dataContext.ClearAllItems();
            }
        }

        private void clearSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure to delete the selected items?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                dataContext.ClearSelectedItems();
            }
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.dataContext.CustomerDetails.IsInvalid)
            {
                MessageBox.Show("Please fill in the customer details", "RQuote");
            }
            else
            {
                var dlg = new CommonOpenFileDialog();
                dlg.Title = "RQuote";
                dlg.IsFolderPicker = true;
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                dlg.AddToMostRecentlyUsedList = false;
                dlg.AllowNonFileSystemItems = false;
                dlg.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.EnsureFileExists = true;
                dlg.EnsurePathExists = true;
                dlg.EnsureReadOnly = false;
                dlg.EnsureValidNames = true;
                dlg.Multiselect = false;
                dlg.ShowPlacesList = true;

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var folderPath = dlg.FileName;
                    exportMask.Visibility = Visibility.Visible;
                    new Task(() =>
                    {
                        try
                        {
                            string fileName = this.dataContext.QuotationNumber.Replace('/', '-');
                            string pdfFileName = Path.Combine(folderPath, fileName + ".pdf");
                            string excelFileName = Path.Combine(folderPath, fileName + ".xlsx");
                            string htmlContent = HTMLManager.GetHTMLFor(this.dataContext);
                            //File.WriteAllText(pdfFileName.Replace(".pdf", ".html"), htmlContent);
                            SavePDFFromHTML(htmlContent,pdfFileName);

                            //excelManager.SaveQuotation(this.dataContext, @"C:\Users\Amresh\source\repos\RQuote\template.xlsx", excelFileName);
                            Utils.IncreaseQuoteNumber();
                            Dispatcher.Invoke(new Action(() =>
                            {
                                var result = MessageBox.Show("Successfully Exported.\n Open folder?", "RQuote", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.Yes)
                                {
                                    System.Diagnostics.Process.Start(folderPath);
                                }
                                exportMask.Visibility = Visibility.Collapsed;
                                //this.dataContext.Reset();
                            }));
                        }
                        catch(Exception ex)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                var result = MessageBox.Show("Something went wrong. Please try again", "RQuote", MessageBoxButton.OK);
                                LogUtil.Logger.Error(ex);
                            }));
                            
                        }
                    }).Start();
                }
            }
        }

        private void previewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (new HTMLPreview(this.dataContext)).Show();
            }
            catch(Exception ex)
            {
                var result = MessageBox.Show("Something went wrong. Please try again", "RQuote", MessageBoxButton.OK);
                LogUtil.Logger.Error(ex);
            }
        }

        private async void SavePDFFromHTML(string html, string filePath)
        {
            try
            {
                var rs = new LocalReporting()
                .KillRunningJsReportProcesses()
                .UseBinary(JsReportBinary.GetBinary())
                .Configure(cfg =>
                {
                    cfg.TempDirectory = Path.Combine(Utils.AppDataPath, "jsreport", "temp");
                    cfg.AllowedLocalFilesAccess();
                    cfg.FileSystemStore();
                    cfg.BaseUrlAsWorkingDirectory();
                    return cfg;
                })
                .AsUtility()
                .KeepAlive(false)
                .Create();

                var report = await rs.RenderAsync(new RenderRequest()
                {
                    Template = new Template()
                    {
                        Recipe = Recipe.ChromePdf,
                        Engine = Engine.None,
                        Content = html,
                        Chrome = new Chrome() { Format = "A4", MarginTop = "0.2in", MarginBottom = "0.2in", MarginLeft = "0.05in", MarginRight = "0.05in" }
                    }
                });

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    report.Content.CopyTo(stream);
                }
            }
            catch(Exception ex)
            {
                LogUtil.Logger.Error(ex);
            }
        }

        #endregion

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.dataContext.SaveQuotation(true);
        }


        private void quotationGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //quotationGrid.CurrentCell = new DataGridCellInfo();
        }

        private bool ignoreCheckboxPropertyChange = false;
        private bool ignoreHeaderCheckboxChange = false;
        private void CellCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!ignoreCheckboxPropertyChange)
            {
                //((sender as CheckBox).DataContext as QuoteLineItem).IsChecked = (bool)(sender as CheckBox).IsChecked;
                try
                {
                    //Update the checked status of header checkbox based on any row unselected/ all rows selected
                    bool hasAnyUnchecked = false;
                    bool isAllChecked = true;
                    foreach (QuoteLineItem model in quotationGrid.ItemsSource)
                    {
                        if (model != null && model.IsChecked == false)
                        {
                            hasAnyUnchecked = true;
                            isAllChecked = false;
                            break;
                        }
                    }
                    if (isAllChecked)
                    {
                        ignoreHeaderCheckboxChange = true;
                        this.dataContext.IsHeaderCheckboxChecked = true;
                        //(this.Resources["CheckboxHeaderDataContext"] as CheckboxSelectionModel).IsChecked = true;
                        ignoreHeaderCheckboxChange = false;
                    }
                    else if (hasAnyUnchecked)
                    {
                        ignoreHeaderCheckboxChange = true;
                        //(this.Resources["CheckboxHeaderDataContext"] as CheckboxSelectionModel).IsChecked = false;
                        this.dataContext.IsHeaderCheckboxChecked = false;
                        ignoreHeaderCheckboxChange = false;
                    }
                }
                catch (Exception ex)
                {

                }
                UpdateSelectedLines();
            }
        }

        private void HeaderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreHeaderCheckboxChange)
            {
                return;
            }
            bool isChecked = (bool)(sender as CheckBox).IsChecked;
            ignoreCheckboxPropertyChange = true;
            foreach (QuoteLineItem model in quotationGrid.ItemsSource)
            {
                model.IsChecked = isChecked;
            }
            UpdateSelectedLines();
            ignoreCheckboxPropertyChange = false;
        }

        private void UpdateSelectedLines()
        {
            this.dataContext.SelectedQuoteLines = (from item in this.dataContext.QuoteLines.AsQueryable() where item.IsChecked select item).ToList<object>();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.dataContext.IsChanged)
            {
                var result = MessageBox.Show("You have unsaved changes. Are you sure?", "RQoute", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    GoBack();
                }
            }
            else
            {
                GoBack();
            }
            
        }

        private void GoBack()
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        public bool IgnoreNewButtonClick = false;
        private void newProductButton_Click(object sender, RoutedEventArgs e)
        {
            if(IgnoreNewButtonClick)
            {
                IgnoreNewButtonClick = false;
                return;
            }
            AddNewProductDialog d = new AddNewProductDialog();
            d.Owner = Application.Current.MainWindow;
            var res = d.ShowDialog();
            if (res == true)
            {
                try
                {
                    Product p = d.DataContext as Product;
                    //Check if product is already existing
                    bool existingProduct = (from prod in customProducts where String.Equals(prod.ModelNo.ToLower(),p.ModelNo.ToLower()) select prod).Any();
                    if(existingProduct)
                    {
                        MessageBox.Show("This Product already exists.", "RQuote", MessageBoxButton.OK);
                    }
                    else
                    {
                        //Copy the image to our local folder
                        //CustomProductsImageFolderPath
                        var newImagePath = Path.Combine(Utils.CustomProductsImageFolderPath, (Guid.NewGuid().ToString() + Path.GetExtension(p.Image)));
                        File.Copy(p.Image, newImagePath);
                        p.Image = newImagePath;
                        p.ModelNo = p.ModelNo.ToUpper();

                        CustomProductManager.SaveCustomProduct(p);

                        customProducts.Add(p);
                        dataContext.suggestionProvider.ListOfParts.Add(p);
                        QuoteLineItem item = new QuoteLineItem(p);
                        dataContext.AddQuoteLine(item);
                        quotationGrid.ScrollIntoView(item);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Something went wrong. Please try again.", "RQuote", MessageBoxButton.OK);
                }
            }
        }

        #region Keyboard event handling
        private void QuotationPage_OnShortcutKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
            {
                //Focus on part lookup field
                partSearchBox.Focusable = true;
                partSearchBox.Focus();
            }
            else if(e.Key == Key.T)
            {
                //Focus on Catalogue picker
                catalogueSelector.Focus();
            }
            else if (e.Key == Key.Enter && addToQuoteButton.IsEnabled)
            {
                Task.Delay(100).ContinueWith(_ =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        addToQuoteButton.Focus();
                        addToQuoteButton_Click(null, null);
                        Task.Delay(250).ContinueWith(__ =>
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                partSearchBox.Focus();
                            }));
                        });

                    }));
                });
            }
        }
       
        #endregion
    }
}
