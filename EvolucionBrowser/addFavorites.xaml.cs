using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;

namespace EvolucionBrowser
{
    public partial class addFavorites : PhoneApplicationPage
    {
        public const string ConnectionString = @"isostore:/evolucionBrowserDB.sdf";
        private string name = "";
        private string uri = "";

        public addFavorites()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            string texto = "";
            string texto1 = "";

            if (NavigationContext.QueryString.TryGetValue("name", out texto))
               this.name = texto;
            if (NavigationContext.QueryString.TryGetValue("uri", out texto1))
                this.uri = texto1;



            textBox1.Text = this.name;
            textBox2.Text = this.uri;




            //---------------------------------------------------------------------------------------------------------

            //evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString);

            //Favorite fav = new Favorite { Name = textBox1.Text, Uri = textBox2.Text };
            //context.Favorites.InsertOnSubmit(fav);
            //context.SubmitChanges();


            //// Define query to fetch all customers in database.
            //var favv = from Favorite todo in context.Favorites
            //                    select todo;

            //// Execute query and place results into a collection.
            //aux = new ObservableCollection<Favorite>(favv);

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //listBox.ItemsSource = aux.ToList();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/favorites.xaml?name=" + textBox1.Text + "&uri=" + textBox2.Text, UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

       
       


    }
}