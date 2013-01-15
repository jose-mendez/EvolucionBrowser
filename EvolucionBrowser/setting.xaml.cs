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

namespace EvolucionBrowser
{
    public partial class setting : PhoneApplicationPage
    { 
        String[] finder = { "Google","Yahoo",
                              "Bing"};
        Util util = new Util();
        string aux="";
        public setting()
        {
            aux = util.readSEngine_file();
           
            InitializeComponent();
            
        }

        
        private void lpkCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                util.setSEngine_file((string)lpkCountry.SelectedItem);
        }

      
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {  
            this.lpkCountry.ItemsSource = finder;
             lpkCountry.SelectedItem = !String.IsNullOrEmpty(aux) ? aux : "Google";
        }

  




          
       
            

    }
}