using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;


namespace EvolucionBrowser
{
    public partial class favorites : PhoneApplicationPage
    {
        public favorites()
        {
            InitializeComponent();
        }



        public const string ConnectionString = @"isostore:/evolucionBrowserDB.sdf";
        private string name = "";
        private string uri = "";
        private ObservableCollection<Favorite> aux;
        //evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString);

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            string texto = "";
            string texto1 = "";

            if (NavigationContext.QueryString.TryGetValue("name", out texto))
                this.name = texto;
            if (NavigationContext.QueryString.TryGetValue("uri", out texto1))
                this.uri = texto1;


            //---------------------------------------------------------------------------------------------------------

            

            using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
            {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(uri)){
                        Favorite fav = new Favorite { Name = name, Uri = uri };
                        context.Favorites.InsertOnSubmit(fav);
                        context.SubmitChanges();
                }

                // Define query to fetch all customers in database.
                var favv = from Favorite todo in context.Favorites
                           select todo;

                // Execute query and place results into a collection.
                aux = new ObservableCollection<Favorite>(favv);
                listBox.ItemsSource = aux.ToList();
            }

        }
  


        private void image1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the element?",
          "Warning", MessageBoxButton.OKCancel);
           

            if (result == MessageBoxResult.OK)
            {
                using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
                {
                    var aux = (from Favorite x in context.Favorites
                               where x.Id == Int32.Parse(((Image)sender).Tag.ToString())
                               select x).Single();



                    context.Favorites.DeleteOnSubmit(aux);
                    context.SubmitChanges();
                    // NavigationService.Navigate(new Uri("/favorite.xaml",UriKind.Relative));
                }
                reloadList();
            }
        }

        public void reloadList() {
            using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
            {
                var aux = from Favorite x in context.Favorites
                          select x;
                listBox.ItemsSource = aux.ToList();
            }
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Favorite aa = (Favorite)((TextBlock)sender).DataContext;
            NavigationService.Navigate(new Uri("/MainPage.xaml?m=openbrowser&uri=" + aa.Uri,UriKind.Relative));
        }


    }




   
}