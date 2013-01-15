using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace EvolucionBrowser
{
    public partial class history : PhoneApplicationPage
    {
        public const string ConnectionString = @"isostore:/evolucionBrowserDB.sdf";
       // evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString);
        public history()
        {
            InitializeComponent();

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
                {
                    var aux = from x in context.Histories
                              select x;

                    listBox1.ItemsSource = aux.ToList();
                }
                
            }
            catch { 
            

            }
        }

    

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            History aux = (History)((TextBlock)sender).DataContext;
            NavigationService.Navigate(new Uri("/MainPage.xaml?m=openbrowser&uri=" + aux.Uri, UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the history","Warning", MessageBoxButton.OKCancel);
            

            if (result == MessageBoxResult.OK)
            {
                using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
                {
                    var a = from x in context.Histories
                            select x;
                    context.Histories.DeleteAllOnSubmit(a);
                    context.SubmitChanges();
                    listBox1.ItemsSource = null;
                }
            }

         
        }

       
    }
}