using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace EvolucionBrowser
{
    public partial class UserAgent : PhoneApplicationPage
    {





        //-------------------------------------------------------------------------------------------------------------------------------------------------
        public UserAgent()
        {
            InitializeComponent();

            if (!String.IsNullOrEmpty(util.readUserAgent_file()))
            {
                switch (util.readUserAgent_file())
                {

                    case "0":
                        rbIEM.IsChecked = true;
                        break;

                    case "1":
                        rbCrhomeM.IsChecked = true;
                        break;

                    case "2":
                        rbMozillaM.IsChecked = true;
                        break;

                    case "3":
                        rbSafariM.IsChecked = true;
                        break;

                    case "4":
                        rbCustom.IsChecked = true;
                        break;


                }

            }
            else
            {
                if (rbIEM.IsChecked == true)
                {
                    util.setUserAgent_file("0");

                }

            }
        }

        private Util util = new Util();

        private void rbCrhomeM_Checked(object sender, RoutedEventArgs e)
        {

            util.setUserAgent_file("1");

        }

        private void rbIEM_Checked(object sender, RoutedEventArgs e)
        {
            util.setUserAgent_file("0");

        }

        private void rbMozillaM_Checked(object sender, RoutedEventArgs e)
        {
            util.setUserAgent_file("2");

        }

        private void rbSafariM_Checked(object sender, RoutedEventArgs e)
        {
            util.setUserAgent_file("3");

        }


        private void rbCustom_Checked(object sender, RoutedEventArgs e)
        {
            util.setUserAgent_file("4");
            util.setCustomUserAgent_file(textBox1.Text);
        }


       

        private void textBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            util.setCustomUserAgent_file(textBox1.Text);
        }
    }
}