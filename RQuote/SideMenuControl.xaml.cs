using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace RQuote
{
    /// <summary>
    /// Interaction logic for SideMenuControl.xaml
    /// </summary>
    /// 
    //[System.Windows.Markup.ContentProperty("SubContent")]
    public partial class SideMenuControl : UserControl
    {
        private bool isAnimating = false;
        private Grid menuContainerGrid = null;
        public SideMenuControl()
        {
            InitializeComponent();
            Loaded += SideMenuControl_Loaded;
        }

        private void SideMenuControl_Loaded(object sender, RoutedEventArgs e)
        {
            menuContainerGrid= (Grid)Template.FindName("menuContainerGrid", this);
        }

        public static readonly DependencyProperty MenuItemsSourceProperty =
        DependencyProperty.Register("MenuItemsSource", typeof(IList<object>),
        typeof(SideMenuControl), new PropertyMetadata(new List<object>()));

        public IList<object> MenuItemsSource
        {
            get { return GetValue(MenuItemsSourceProperty) as IList<object>; }
            set { SetValue(MenuItemsSourceProperty, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if(isAnimating)
            //{
            //    return;
            //}
            //isAnimating = true;
            VisualStateManager.GoToState(this, "Open", false);
            //if (menuContainerGrid.Width < 200)
            //{
            //    (this.Resources["MenuOpenAnimation"] as Storyboard).Begin();
            //}
            //else
            //{
            //    (this.Resources["MenuCloseAnimation"] as Storyboard).Begin();
            //}
        }

        private void DoubleAnimationUsingKeyFrames_Completed(object sender, EventArgs e)
        {
            isAnimating = false;
        }
    }
}
