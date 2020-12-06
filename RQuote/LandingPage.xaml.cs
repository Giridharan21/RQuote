using RQuote.Logic.Models;
using RQuote.UserControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RQuote
{
    /// <summary>
    /// Interaction logic for LandingPage.xaml
    /// </summary>
    public partial class LandingPage : Page
    {
        private Boolean IsAppInitialized = Utils.IsApplicationInitialized();
        List<Product> customProducts;
        public DbAccess DbAccess { get; set; }
        public LandingPage()
        {
            InitializeComponent();
            DbAccess = new DbAccess();
            this.DataContext = new LandingPageDataContext();
            customProducts = CustomProductManager.LoadCustomCatalogue();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsAppInitialized)
            {
                initGrid.Visibility = Visibility.Hidden;
                 
                //startGrid.Visibility = Visibility.Visible;
                CustomerIdGrid.Visibility = Visibility.Visible;
            }
            else
            {

                CustomerIdGrid.Visibility = Visibility.Visible;
                startGrid.Visibility = Visibility.Hidden;
            }

            (Application.Current.MainWindow as MainWindow).OnShortcutKeyPress += QuotationPage_OnShortcutKeyPress;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).OnShortcutKeyPress -= QuotationPage_OnShortcutKeyPress;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
            {
                NavigationService.Navigate(new Uri("QuotationPage.xaml", UriKind.Relative));
            }
        }
        //Previouse Initial Button function
        //private void StartInitialSetup(object sender, RoutedEventArgs e)
        //{
        //    var isMatch = Regex.IsMatch(CustomerId.Text,@"(?im)^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$");
        //    //var isMatch = true;
        //    if (isMatch)
        //    {
        //        DoInitialSetup(CustomerId.Text);
        //        initGrid.Visibility = Visibility.Visible;
        //        startGrid.Visibility = Visibility.Hidden;
        //        CustomerIdGrid.Visibility = Visibility.Hidden;
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please enter a valid Customer ID");
        //        CustomerId.Focus();
        //    }
        //}

        //New initial function
        private async void SelectShowroom(object sender, RoutedEventArgs e)
        {
            initGrid.Visibility = Visibility.Visible;
            CustomerIdGrid.Visibility = Visibility.Hidden;
            var users =await DbAccess.GetUsersForShowRoom(ShowroomCode.Text);

            initGrid.Visibility = Visibility.Hidden;
            CustomerIdGrid.Visibility = Visibility.Visible;
            if (users.Count == 0)
            {
                MessageBox.Show("Enter a valid Showroom Code");
                return;
            }
            GridUserVerify.Visibility = Visibility.Visible;
            Username.ItemsSource = users;
            LoginBtn.Visibility = Visibility.Visible;
            SelectBtn.Visibility = Visibility.Hidden;
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            initGrid.Visibility = Visibility.Visible;
            CustomerIdGrid.Visibility = Visibility.Hidden;
            var check = await DbAccess.AuthenticateUser(Username.SelectedItem.ToString(), Password.Text);
            initGrid.Visibility = Visibility.Hidden;
            CustomerIdGrid.Visibility = Visibility.Visible;

            if (check)
            {
                //MessageBox.Show("Login Successful!");
                CustomerIdGrid.Visibility = Visibility.Hidden;
                initGrid.Visibility = Visibility.Hidden;
                startGrid.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Login failed!");

            }
        }

        private void DoInitialSetup(string customerId)
        {
            //Check for internet connection
            if(Utils.InitializeAppFolder())
            {
                SyncFromGDrive(customerId);
            }
            else
            {
                //Handle error
            }
        }

        private void InitialSetupDone()
        {
            //StartButton_Click(null, null);
            IsAppInitialized = Utils.IsApplicationInitialized();
            (this.DataContext as LandingPageDataContext).TotalFiles = 0;
            (this.DataContext as LandingPageDataContext).DownloadedFiles = 0;
            Page_Loaded(null, null);
        }

        private void HandleNoCustomerData()
        {
            initGrid.Visibility = Visibility.Hidden;
            startGrid.Visibility = Visibility.Hidden;
            CustomerIdGrid.Visibility = Visibility.Visible;
            MessageBox.Show("Please enter a valid Customer ID");
            //CustomerId.Focus();
            ShowroomCode.Focus();
        }

        private void DataGrid_Selected(object sender, RoutedEventArgs e)
        {
            SavedQuotationModel selectedQuotation = (sender as DataGrid).SelectedItem as SavedQuotationModel;
            if(selectedQuotation != null)
            {
                NavigationService.Navigate(new QuotationPage(selectedQuotation.Id));
                (sender as DataGrid).SelectedItem = null;
            }
        }

        private void Sync_Button_Click(object sender, RoutedEventArgs e)
        {
            string customerID = Utils.GetCurrentCustomerId();
            if(customerID != null)
            {
                SyncFromGDrive(customerID);
                initGrid.Visibility = Visibility.Visible;
                startGrid.Visibility = Visibility.Hidden;
                CustomerIdGrid.Visibility = Visibility.Hidden;
            }
        }

        private void SyncFromGDrive(string customerId)
        {
            DriveManager driveManager = new DriveManager();
            driveManager.LandingPageDataContext = this.DataContext as LandingPageDataContext;
            String initialFileName = driveManager.DownloadInitialFile();
            if (initialFileName != null)
            {
                var x = new Task(() =>
                {
                    ExcelManager excelManager = new ExcelManager();
                    List<string> parentFolderIds = excelManager.GetVendorCatalogParentFolderIds(initialFileName, customerId);
                    if (parentFolderIds.Count > 0)
                    {
                        List<Task> tasks = new List<Task>();
                        foreach (string parentFolderId in parentFolderIds)
                        {
                            if(!string.IsNullOrWhiteSpace(parentFolderId))
                            {
                                tasks.AddRange(driveManager.DownloadFolderContents(parentFolderId));
                                var templateId = excelManager.GetVendorTemplateId(initialFileName, customerId);
                                if (!string.IsNullOrWhiteSpace(templateId))
                                {
                                    tasks.Add(driveManager.DownloadFile(templateId, Utils.TemplateFilePath));
                                }
                            }
                        }
                        foreach (var task in tasks)
                        {
                            try
                            {
                                if (task.Status != TaskStatus.Running || task.Status != TaskStatus.WaitingToRun)
                                {
                                    task.Start();
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        Task.WaitAll(tasks.ToArray());
                        Dispatcher.Invoke(new Action(() => {
                            Utils.PersistCustomer(customerId);
                            InitialSetupDone();
                        }));
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() => {
                            HandleNoCustomerData();
                        }));
                    }

                });

                x.Start();
            }
            else
            {
                //Handle error
            }
        }

        private void newProductButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewProductDialog d = new AddNewProductDialog();
            d.Owner = Application.Current.MainWindow;
            var res = d.ShowDialog();
            if (res == true)
            {
                try
                {
                    Product p = d.DataContext as Product;
                    //Check if product is already existing
                    bool existingProduct = (from prod in customProducts where String.Equals(prod.ModelNo.ToLower(), p.ModelNo.ToLower()) select prod).Any();
                    if (existingProduct)
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

                        MessageBox.Show("Product added successfully.", "RQuote", MessageBoxButton.OK);

                        customProducts.Add(p);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong. Please try again.", "RQuote", MessageBoxButton.OK);
                }
            }
        }

        #region Keyboard event handling
        private void QuotationPage_OnShortcutKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && startGrid.Visibility == Visibility.Visible)
            {
                //Focus on quotation search box

                //quotationFilterTextBox.Focus();
            }
            else if (e.Key == Key.N && startGrid.Visibility == Visibility.Visible)
            {
                //New Quotation
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    StartButton_Click(null, null);
                }));
            }
            else if (e.Key == Key.P && startGrid.Visibility == Visibility.Visible)
            {
                //New Product
                newProductButton_Click(null, null);
            }
        }

        #endregion

        private void ShowroomCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            GridUserVerify.Visibility = Visibility.Collapsed;
            LoginBtn.Visibility = Visibility.Hidden;
            SelectBtn.Visibility = Visibility.Visible;
        }
    }
}
