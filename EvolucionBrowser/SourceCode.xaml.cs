using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace EvolucionBrowser
{
    public partial class SourceCode : PhoneApplicationPage
    {
        public SourceCode()
        {
            InitializeComponent();


        }


    

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {


            string aux = (string)((System.Windows.Controls.Frame)this.Parent).DataContext;
            SourceCod.Text = aux;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            try
            {
                EmailComposeTask emailcomposer = new EmailComposeTask();
                //emailcomposer.To = tbEmail.Text;
                emailcomposer.Subject = "Source code from:" + textBlock1.Text;
                emailcomposer.Body = SourceCod.Text.Replace("\n", ""); //arreglar aqui cuando el texto de  SourceCod.Text es > 64kb da error
                emailcomposer.Show();
                
            }
            catch (Exception h) {
                MessageBox.Show("Error sending Email: " + h.Message);
            }
           
        }

       

       

    }
}