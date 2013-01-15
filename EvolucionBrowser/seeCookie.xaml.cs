using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace EvolucionBrowser
{
    public partial class seeCookie : PhoneApplicationPage
    {
        public seeCookie()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            CookieCollection aux = (CookieCollection)((System.Windows.Controls.Frame)this.Parent).DataContext;
            listBox1.ItemsSource = aux;

            foreach (Cookie it in aux) {

              var a =  it.Name;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            string texto = "";
            if (NavigationContext.QueryString.TryGetValue("url", out texto))
                ApplicationTitle.Text += texto;

          
        }

      

    }
}