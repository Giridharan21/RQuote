using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for SpacedLabel.xaml
    /// </summary>
    public partial class SpacedLabel : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string),
        typeof(SpacedLabel), new PropertyMetadata(String.Empty));

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set 
            { 
                SetValue(TextProperty, value);
                UpdateText();
                UpdateTextFont();
            }
        }

        public SpacedLabel()
        {
            InitializeComponent();
            Loaded += SpacedLabel_Loaded;
        }

        private void SpacedLabel_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateText();
            UpdateTextFont();
        }

        private void UpdateText()
        {
            rootGrid.Children.Clear();
            rootGrid.RowDefinitions.Clear();
            if(!String.IsNullOrWhiteSpace(Text))
            {
                var allChars = Text.ToCharArray();
                for (int i = 0; i < allChars.Length; i++)
                {
                    rootGrid.RowDefinitions.Add(new RowDefinition());
                }

                for (int i = 0; i < allChars.Length; i++)
                {
                    Canvas c = new Canvas();

                    TextBlock l = new TextBlock();
                    l.Text = allChars[i].ToString();
                    rootGrid.Children.Add(c);

                    c.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                    c.RenderTransform = new RotateTransform(-90);
                    c.VerticalAlignment = VerticalAlignment.Top;
                    //l.Margin = new Thickness(0, -5, 0, 0);
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    c.Height = Double.NaN;
                    c.Width = Double.NaN;
                    //l.TextAlignment = TextAlignment.Justify;
                    c.Background = System.Windows.Media.Brushes.Blue;
                    Grid.SetRow(c, allChars.Length - 1 - i);
                    c.Children.Add(l);
                }
            }
        }

        private void UpdateTextFont()
        {
            foreach(var child in rootGrid.Children)
            {
                //var label = (child as Canvas).Children[0] as TextBlock;
                //if(label != null)
                //{
                //    label.FontFamily = FontFamily;
                //    label.FontSize = FontSize;
                //}
            }
        }
    }
}
