using RQuote.Logic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RQuote.UserControl
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : System.Windows.Controls.UserControl
    {
        public DbAccess dbAccess { get; set; }
        public ObservableCollection<SavedQuotationModel> SavedQuotations { get; set; }
        public List<SavedQuotationModel> Quotations { get; set; }
        public Dashboard()
        {
            InitializeComponent();
            SavedQuotations = new ObservableCollection<SavedQuotationModel>();
            InitializeGrid();
        }
        private void InitializeGrid()
        {
            new Task(async () => {
                Quotations = await dbAccess.GetQuotations();
            });
            if(Quotations is null || Quotations.Count == 0)
            {
                TextInfo.Visibility = Visibility.Visible;
                QuotationGrid.Visibility = Visibility.Hidden;
                return;
            }
            SavedQuotations = new ObservableCollection<SavedQuotationModel> (Quotations);
        }
    }
}
