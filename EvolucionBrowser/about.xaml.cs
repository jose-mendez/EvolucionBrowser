 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace EvolucionBrowser
{
    public partial class about : PhoneApplicationPage
    {
        public about()
        {
            InitializeComponent();
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = "jmendez.blogspot@gmail.com";
            
            email.Subject = "Evolucion Browser Feedback";
           
            email.Show();
        }

        private void textBlock1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
             MarketplaceReviewTask review = new MarketplaceReviewTask();
            
                review.Show();

        }

        private void textBlock2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceSearchTask marketplaceSearchTask = new MarketplaceSearchTask();

            marketplaceSearchTask.SearchTerms = "evolucion_phone";

            marketplaceSearchTask.Show();
        }

        MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _marketPlaceDetailTask.Show();
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).IsTrial)
            {
                buy.Visibility = Visibility.Visible;
                buy.Content = Cadenas.BuyApp;
            }
            else 
            {
                buy.Visibility = Visibility.Collapsed;
            }
        }

     
    }
}