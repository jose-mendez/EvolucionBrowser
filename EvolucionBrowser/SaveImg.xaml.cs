using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework.Media;
using System.Text.RegularExpressions;
using System.Windows.Resources;
using Microsoft.Phone.Tasks;

namespace EvolucionBrowser
{
    public partial class SaveImg : PhoneApplicationPage
    {
        private int r;
        public SaveImg()
        {
            InitializeComponent();
            r = 0;
        }






        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            string texto = "";
            if (NavigationContext.QueryString.TryGetValue("url", out texto))
                textBlock1.Text = texto;

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Cadenas.HoldMsg);

            try
            {
                string[] aux1 = (string[])((System.Windows.Controls.Frame)this.Parent).DataContext;
                string[] aux = aux1.Distinct().ToArray();
                int count = aux.Count();


                for (int i = 0; i < count; i++)
                {
                    BitmapImage bmi = new BitmapImage(new Uri(aux[i], UriKind.RelativeOrAbsolute));//"Images/fullscreen.jpg"

                    bmi.ImageOpened += new EventHandler<RoutedEventArgs>(bmi_ImageOpened);

                    Image img = new Image();
                    img = new Image();
                    img.Name = "img" + i;
                    img.Source = bmi;

                    listaElementos.Add(new Elemento { Identificador = i, Nombre = aux[i], Imagen = aux[i] });

                    listBox2.Items.Add(img);
                }
            }
            catch { }

            // this.DataContext = listaElementos;
        }




        void bmi_ImageOpened(object sender, RoutedEventArgs e)
        {
            Image img = new Image();

            if (((BitmapImage)sender).PixelWidth < 100 || ((BitmapImage)sender).PixelHeight < 100)
            {
                var aaaa = listaElementos.Distinct();
                Elemento elem = aaaa.Where(x => x.Imagen.ToString() == ((BitmapImage)sender).UriSource.ToString()).FirstOrDefault();
                listaElementos.Remove(elem);
                listBox1.ItemsSource = listaElementos;


            }
            r++;
        }
        string path;
        Uri imageUri;



        void img_Hold(string name)
        {

            path = name;

            string uri = path;
            WebClient m_webClient = new WebClient();
            imageUri = new Uri(uri, UriKind.Absolute);
            m_webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_ImageOpenReadCompleted);
            m_webClient.OpenReadAsync(imageUri);
        }



        void webClient_ImageOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
             try
            {
            using (IsolatedStorageFile myIsf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsf.FileExists(path))
                {
                    myIsf.DeleteFile(path);
                }

                Match aux = Regex.Match(path, "[a-z A-Z 0-9 _ -]+\\.(jpg|png)");
                IsolatedStorageFileStream fileStream = myIsf.CreateFile(aux.ToString());

               // StreamResourceInfo sri = null;
               // sri = Application.GetResourceStream(imageUri);

                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(e.Result);
                WriteableBitmap wb = new WriteableBitmap(bitmap);

                // Encode WriteableBitmap object to a JPEG stream.
                Extensions.SaveJpeg(wb, fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                fileStream.Close();


                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fileStream1 = myIsolatedStorage.OpenFile(aux.ToString(), FileMode.Open, FileAccess.Read))
                    {
                        MediaLibrary mediaLibrary = new MediaLibrary();
                        Picture pic = mediaLibrary.SavePicture(aux.ToString(), fileStream1);
                        fileStream1.Close();
                    }
                }
            }

            MessageBox.Show(Cadenas.ImgSaved + " " + path);


            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Show();

            }
             catch 
             {
                 MessageBox.Show(Cadenas.ErrorSavingImg);
             }
        }


   




        
        public class Elemento
        {
            public int Identificador { get; set; }
            public string Nombre { get; set; }
            // public string url { get; set; }
            public string Imagen { get; set; }
        }

        List<Elemento> listaElementos = new List<Elemento>();




        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           
               

                MenuItem ElementoElegido = sender as MenuItem;
                string name = ElementoElegido.Tag.ToString();
               // Match aux = Regex.Match(name, "[a-z A-Z 0-9 _ -]+\\.(jpg|png)");

                img_Hold(name);
           
        }

       
    }
}