using System;
using System.Threading.Tasks;
using System.Windows;

namespace RQuote
{
    /// <summary>
    /// Interaction logic for HTMLPreview.xaml
    /// </summary>
    public partial class HTMLPreview : Window
    {
        QuotationPageDataContext _quotationDetails;
        public HTMLPreview(QuotationPageDataContext quotationDetails)
        {
            InitializeComponent();
            webPreview.NavigationCompleted += WebPreview_NavigationCompleted;
            _quotationDetails = quotationDetails;
            new Task(() =>
            {
                var htmlContent = HTMLManager.GetHTMLFor(quotationDetails);
                Dispatcher.Invoke(new Action(() =>
                {
                    webPreview.NavigateToString(htmlContent);
                }));
            }).Start();


        }

        private void WebPreview_NavigationCompleted(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationCompletedEventArgs e)
        {
            initGrid.Visibility = Visibility.Hidden;
            progress.IsIndeterminate = false;
            webPreview.Visibility = Visibility.Visible;
        }
    }
}
