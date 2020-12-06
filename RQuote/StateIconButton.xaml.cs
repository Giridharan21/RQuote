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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RQuote
{
    /// <summary>
    /// Interaction logic for StateIconButton.xaml
    /// </summary>
    public partial class StateIconButton : System.Windows.Controls.UserControl
    {
        public event RoutedEventHandler Click;

        public StateIconButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string),
        typeof(StateIconButton), new PropertyMetadata(String.Empty));

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public static readonly DependencyProperty NormalImageProperty =
        DependencyProperty.Register("NormalImage", typeof(ImageSource),
        typeof(StateIconButton), new PropertyMetadata(null));

        public ImageSource NormalImage
        {
            get { return GetValue(NormalImageProperty) as ImageSource; }
            set
            {
                SetValue(NormalImageProperty, value);
            }
        }

        public static readonly DependencyProperty HoverImageProperty =
        DependencyProperty.Register("HoverImage", typeof(ImageSource),
        typeof(StateIconButton), new PropertyMetadata(null));

        public ImageSource HoverImage
        {
            get { return GetValue(HoverImageProperty) as ImageSource; }
            set
            {
                SetValue(HoverImageProperty, value);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this,null);
        }
    }
}
