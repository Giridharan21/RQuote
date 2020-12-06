using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RQuote
{
    /// <summary>
    /// Interaction logic for AddNewProductDialog.xaml
    /// </summary>
    public partial class AddNewProductDialog : Window
    {
        Product newProduct;
        public AddNewProductDialog()
        {
            InitializeComponent();
            this.DataContext = newProduct = new Product();
        }

        private void selectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.png) | *.jpg; *.jpeg; *.jpe; *.png";
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                newProduct.Image = dlg.FileName;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(newProduct.ModelNo))
            {
                showMessageBox("Please enter Part No.", partNoTb);
                return;
            }
            if (String.IsNullOrWhiteSpace(newProduct.Image))
            {
                showMessageBox("Please select Product Image.");
                return;
            }
            if (String.IsNullOrWhiteSpace(newProduct.Details))
            {
                showMessageBox("Please enter Product Description.", productDescriptionTb);
                return;
            }
            this.DialogResult = true;
        }

        private void showMessageBox(string Message, FrameworkElement elementToFocus=null)
        {
            var res = MessageBox.Show(Message, "RQuote", MessageBoxButton.OK);
            if(res == MessageBoxResult.OK && elementToFocus != null)
            {
                elementToFocus.Focus();
            }
        }
    }
}
