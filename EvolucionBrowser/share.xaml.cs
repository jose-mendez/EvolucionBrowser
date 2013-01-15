using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Phone.Controls;




namespace EvolucionBrowser
{
    public partial class share : PhoneApplicationPage
    {
      
        public share()
        {
            InitializeComponent();

          
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            webBrowser1.Navigate(new Uri("/app.html", UriKind.Relative));
        }

        private void webBrowser1_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                string url;
                NavigationContext.QueryString.TryGetValue("url", out url);

                webBrowser1.InvokeScript("seturl", url);
            }
            catch (Exception r) { }
        }

        private void webBrowser1_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
         
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            webBrowser1.InvokeScript("eval", "history.Back();");
        }


    }
}