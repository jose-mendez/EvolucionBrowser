using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Devices.Sensors;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Text.RegularExpressions;
using Microsoft.Phone.Shell;


namespace EvolucionBrowser
{

    
    public partial class MainPage : PhoneApplicationPage
    {
        Stack<Uri> _navigationStack1 = new Stack<Uri>();
        Stack<Uri> _navigationStack2 = new Stack<Uri>();
        Stack<Uri> _navigationStack3 = new Stack<Uri>();
        Stack<Uri> _navigationStack4 = new Stack<Uri>();

        public const string ConnectionString = @"isostore:/evolucionBrowserDB.sdf";
       // evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString);
        private List<History> hist
        {

            set { }

            get
            {
                try
                {
                    using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
                    {
                        return (from x in context.Histories
                                select x).ToList();
                    }
                }
                catch { return null; }
            }
        } 
       

        private Accelerometer accelerometer = new Accelerometer();
        private AccelerometerSensorWithShakeDetection _shakeSensor = new AccelerometerSensorWithShakeDetection();
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _shakeSensor = new AccelerometerSensorWithShakeDetection();//instanciamos el acelerometro

            //esto de alguna forma pone escuchar el evento ShakeDetected y llama al eventhandler "private void ShakeDetected(object sender, EventArgs e)"
            Loaded += (sender, args) =>
            {
                _shakeSensor.ShakeDetected += ShakeDetected;
                _shakeSensor.Start();
            };
            Unloaded += (sender, args) =>
            {
                _shakeSensor.ShakeDetected -= ShakeDetected;
                _shakeSensor.Stop();
            };




            Util util = new Util();
            if (String.IsNullOrEmpty(util.readUserAgent_file()))
            {
                util.setUserAgent_file("0");

            }



            var button0 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            var button1 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            var button2 = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            var button3 = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            button0.Text = Cadenas.history;
            button1.Text = Cadenas.addFav;
            button2.Text = Cadenas.fav;
            button3.Text = Cadenas.share;

           var item0 = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
           var item1 = (ApplicationBarMenuItem)ApplicationBar.MenuItems[1]; 
            var item2 = (ApplicationBarMenuItem)ApplicationBar.MenuItems[2]; 
            var item3 = (ApplicationBarMenuItem)ApplicationBar.MenuItems[3];
            var item4 = (ApplicationBarMenuItem)ApplicationBar.MenuItems[4];
            var item5 = (ApplicationBarMenuItem)ApplicationBar.MenuItems[5];

            item0.Text = Cadenas.setUA;
            item1.Text = Cadenas.saveImg;
            item2.Text =Cadenas.cookies;
            item3.Text = Cadenas.scode;
            item4.Text = Cadenas.setting;
            item5.Text = Cadenas.about;




        }


        private void ShakeDetected(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                QuitFullScreen();//esto es necesario pq el acelerometro corre en otro hilo, y el metodo QuitFullScreen() corre en el hilo de la UI
            });

        }






        #region Navigate buttons
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Navigate(textBox1, webBrowser1);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (pivot1.SelectedIndex == 0)
                {
                    button1_Click(null, null);
                    button1.Focus();
                }else if (pivot1.SelectedIndex == 1)
                {
                    button2_Click(null, null);
                    button2.Focus();
                }
                else if (pivot1.SelectedIndex == 2)
                {
                    button3_Click(null, null);
                    button3.Focus();
                }
                else if (pivot1.SelectedIndex == 3)
                {
                    button4_Click(null, null);
                    button4.Focus();
                }
            }
        }


        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Navigate(textBox2, webBrowser2);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Navigate(textBox3, webBrowser3);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Navigate(textBox4, webBrowser4);
        }
        #endregion



        #region custom methods
        private string toSearch = "";
        private Util util = new Util();
        private void Navigate(System.Windows.Controls.TextBox uri, WebBrowser wbrowser)
        {

            string UserAgentTxt = "";
            switch (util.readUserAgent_file())
            {

                case "0":
                    UserAgentTxt = "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0;";
                    break;

                case "1":
                    UserAgentTxt = "User-Agent: Mozilla/5.0 (Linux; <Android Version>; <Build Tag etc.>)AppleWebKit/<WebKit Rev> (KHTML, like Gecko) Chrome/<Chrome Rev> Mobile Safari/<WebKit Rev>";
                    break;

                case "2":
                    UserAgentTxt = "User-Agent: Mozilla/5.0 (Android; Mobile; rv:12.0) Gecko/12.0 Firefox/12.0";
                    break;

                case "3":
                    UserAgentTxt = "User-Agent: Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25";
                    break;

                case "4":
                    UserAgentTxt = "User-Agent:" + util.readCustomUserAgent_file();
                    break;
            }
            if (!String.IsNullOrEmpty(uri.Text))
            {
                if (!Regex.IsMatch(uri.Text, "^(http://|/)") && !Regex.IsMatch(uri.Text, "^(https://|/)"))
                {
                    toSearch = uri.Text;
                    uri.Text = "http://" + uri.Text;
                }

                try
                {
                    wbrowser.Navigate(new Uri(uri.Text, UriKind.Absolute), null, UserAgentTxt);
                }
                catch  {
                    
                }
              
            }
            else
            {
               MessageBox.Show("You must enter an address");
            }
        }

        private bool _isFullScren = false;
        public void setFullScreem(System.Windows.Controls.TextBox nav, System.Windows.Controls.Button button, WebBrowser browser, System.Windows.Controls.ProgressBar pb)
        {

            if (nav.Visibility != System.Windows.Visibility.Collapsed)
            {
                _isFullScren = true;
                nav.Visibility = System.Windows.Visibility.Collapsed;
                button.Visibility = System.Windows.Visibility.Collapsed;

                pb.Margin = new Thickness(10, -63, 0, 0);
                browser.Margin = new Thickness(-15, -60, 3, 3);

                ApplicationBar.IsVisible = false;

            }
            else
            {
                nav.Visibility = System.Windows.Visibility.Visible;
                button.Visibility = System.Windows.Visibility.Visible;

                browser.Margin = new Thickness(-4, 70, 14, 0);
                ApplicationBar.IsVisible = true;
                _isFullScren = false;
            }

        }



        public void QuitFullScreen()
        {
            _isFullScren = false;
            if (webBrowser1.Margin != new Thickness(-4, 70, 14, 0))
            {


                textBox1.Visibility = System.Windows.Visibility.Visible;
                button1.Visibility = System.Windows.Visibility.Visible;

                webBrowser1.Margin = new Thickness(-4, 70, 14, 0);
                ApplicationBar.IsVisible = true;
                ProgBar1.Margin = new Thickness(10,11,0,0);
            }


            if (webBrowser2.Margin != new Thickness(-4, 70, 14, 0))
            {


                textBox2.Visibility = System.Windows.Visibility.Visible;
                button2.Visibility = System.Windows.Visibility.Visible;

                webBrowser2.Margin = new Thickness(-4, 70, 14, 0);
                ApplicationBar.IsVisible = true;
                ProgBar2.Margin = new Thickness(10, 11, 0, 0);
            }


            if (webBrowser3.Margin != new Thickness(-4, 70, 14, 0))
            {


                textBox3.Visibility = System.Windows.Visibility.Visible;
                button3.Visibility = System.Windows.Visibility.Visible;

                webBrowser3.Margin = new Thickness(-4, 70, 14, 0);
                ApplicationBar.IsVisible = true;
                ProgBar3.Margin = new Thickness(10, 11, 0, 0);
            }


            if (webBrowser4.Margin != new Thickness(-4, 70, 14, 0))
            {


                textBox4.Visibility = System.Windows.Visibility.Visible;
                button4.Visibility = System.Windows.Visibility.Visible;

                webBrowser4.Margin = new Thickness(-4, 70, 14, 0);
                ApplicationBar.IsVisible = true;
                ProgBar4.Margin = new Thickness(10, 11, 0, 0);
            }
            //_shakeSensor.Stop();
        }


        private void setImageBackgroundButton(System.Windows.Controls.Button btn, string imgUri)
        {

            ImageBrush iBrushAyuda = new ImageBrush();
            iBrushAyuda.ImageSource = new BitmapImage(new Uri(imgUri, UriKind.Relative));
            btn.Background = iBrushAyuda;
        }
        #endregion



        #region webBrowser_Navigating_Navigated_ClickCancel

        private void webBrowser1_Navigating(object sender, NavigatingEventArgs e)
        {
           

            ProgBar1.Visibility = Visibility.Visible;
            setImageBackgroundButton(button1, "Images/cancel.jpg");
            button1.Click += new RoutedEventHandler(button1_ClickCancel);

        }

        void button1_ClickCancel(object sender, RoutedEventArgs e)
        {
            webBrowser1.Navigate(new Uri("about:blank", UriKind.Absolute));
        }

        private void webBrowser1_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            _navigationStack1.Push(e.Uri);

            if (webBrowser1.Source != null && webBrowser1.Source != new Uri("/app.html", UriKind.Relative) && webBrowser1.Source != new Uri("/source.html", UriKind.Relative) && webBrowser1.Source != new Uri("about:blank", UriKind.Absolute))
                textBox1.Text = webBrowser1.Source.ToString();

            saveAllinBrowser();
    

            ProgBar1.Visibility = Visibility.Collapsed;
            setImageBackgroundButton(button1, "Images/reload.jpg");
            button1.Click += new RoutedEventHandler(button1_Click);


            string tit= "";
            try
            {
                 tit = (string)webBrowser1.InvokeScript("eval", "document.title.toString()");
            }
            catch { }


            if (pivot1.SelectedIndex == 0)
            {
                using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
                {
                    History aux = new History { Name = tit, Uri = (textBox1.Text.Length >1000)? textBox1.Text.Substring(0,999):textBox1.Text };
                    context.Histories.InsertOnSubmit(aux);
                    context.SubmitChanges();
                }
            }

                
        }



        private void webBrowser2_Navigating(object sender, NavigatingEventArgs e)
        {
            ProgBar2.Visibility = Visibility.Visible;
            setImageBackgroundButton(button2, "Images/cancel.jpg");
            button2.Click += new RoutedEventHandler(button2_ClickCancel);
        }

        void button2_ClickCancel(object sender, RoutedEventArgs e)
        {
            webBrowser2.Navigate(new Uri("about:blank", UriKind.Absolute));
            //throw new NotImplementedException();
        }

        private void webBrowser2_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            _navigationStack2.Push(e.Uri);

            if (webBrowser2.Source != null && webBrowser2.Source != new Uri("/app.html", UriKind.Relative) && webBrowser2.Source != new Uri("/source.html", UriKind.Relative) && webBrowser2.Source != new Uri("about:blank", UriKind.Absolute))
                textBox2.Text = webBrowser2.Source.ToString();

            saveAllinBrowser();

            ProgBar2.Visibility = Visibility.Collapsed;
            setImageBackgroundButton(button2, "Images/reload.jpg");
            button2.Click += new RoutedEventHandler(button2_Click);


            string tit = "";
            try
            {
                tit = (string)webBrowser2.InvokeScript("eval", "document.title.toString()");
            }
            catch { }

            if (pivot1.SelectedIndex == 1 )
            {
                using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
                {
                    History aux = new History { Name = tit, Uri = (textBox2.Text.Length > 1000) ? textBox2.Text.Substring(0, 999) : textBox2.Text };
                    context.Histories.InsertOnSubmit(aux);
                    context.SubmitChanges();
                }
            }
        }


        private void webBrowser3_Navigating(object sender, NavigatingEventArgs e)
        {
            ProgBar3.Visibility = Visibility.Visible;
            setImageBackgroundButton(button3, "Images/cancel.jpg");
            button3.Click += new RoutedEventHandler(button3_ClickCancel);
        }


        void button3_ClickCancel(object sender, RoutedEventArgs e)
        {
            webBrowser3.Navigate(new Uri("about:blank", UriKind.Absolute));
            //throw new NotImplementedException();
        }


        private void webBrowser3_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

            _navigationStack3.Push(e.Uri);

            if (webBrowser3.Source != null && webBrowser3.Source != new Uri("/app.html", UriKind.Relative) && webBrowser3.Source != new Uri("/source.html", UriKind.Relative) && webBrowser3.Source != new Uri("about:blank", UriKind.Absolute))
                textBox3.Text = webBrowser3.Source.ToString();


            saveAllinBrowser();

            ProgBar3.Visibility = Visibility.Collapsed;
            setImageBackgroundButton(button3, "Images/reload.jpg");
            button3.Click += new RoutedEventHandler(button3_Click);


            string tit = "";
            try
            {
                tit = (string)webBrowser3.InvokeScript("eval", "document.title.toString()");
            }
            catch  { }

            
            if (pivot1.SelectedIndex == 2 )
            {
                using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
                {
                    History aux = new History { Name = tit, Uri = (textBox3.Text.Length > 1000) ? textBox3.Text.Substring(0, 999) : textBox3.Text };
                    context.Histories.InsertOnSubmit(aux);
                    context.SubmitChanges();
                }
            }
        }

    



        private void webBrowser4_Navigating(object sender, NavigatingEventArgs e)
        {
            ProgBar4.Visibility = Visibility.Visible;
            setImageBackgroundButton(button4, "Images/cancel.jpg");
            button4.Click += new RoutedEventHandler(button4_ClickCancel);
        }


        void button4_ClickCancel(object sender, RoutedEventArgs e)
        {
            webBrowser4.Navigate(new Uri("about:blank", UriKind.Absolute));
            //throw new NotImplementedException();
        }

        private void webBrowser4_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            _navigationStack4.Push(e.Uri);

            if (webBrowser4.Source != null && webBrowser4.Source != new Uri("/app.html", UriKind.Relative) && webBrowser4.Source != new Uri("/source.html", UriKind.Relative) && webBrowser4.Source != new Uri("about:blank", UriKind.Absolute))
                textBox4.Text = webBrowser4.Source.ToString();

            saveAllinBrowser();

         
            ProgBar4.Visibility = Visibility.Collapsed;
            setImageBackgroundButton(button4, "Images/reload.jpg");
            button4.Click += new RoutedEventHandler(button4_Click);

            string tit = "";
            try
            {
                tit = (string)webBrowser4.InvokeScript("eval", "document.title.toString()");
            }
            catch { }

            if (pivot1.SelectedIndex == 3 )
            {
                using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
                {
                    History aux = new History { Name = tit, Uri = (textBox4.Text.Length > 1000) ? textBox4.Text.Substring(0, 999) : textBox4.Text };
                    context.Histories.InsertOnSubmit(aux);
                    context.SubmitChanges();
                }
            }
        }


        #endregion



        #region next back btn

        private void btNext1_Click(object sender, RoutedEventArgs e)
        {
            if (pivot1.SelectedIndex == 0)
            {
                pivot1.SelectedIndex = 1;
            }
            else if(pivot1.SelectedIndex == 1)
            {
                pivot1.SelectedIndex = 2;
            }
            else if (pivot1.SelectedIndex == 2)
            {
                pivot1.SelectedIndex = 3;
            }
            else if (pivot1.SelectedIndex == 3)
            {
                pivot1.SelectedIndex = 0;
            }
           
        }

        private void btBack1_Click(object sender, RoutedEventArgs e)
        {
            if (pivot1.SelectedIndex == 0)
            {
                pivot1.SelectedIndex = 3;
            }
            else if (pivot1.SelectedIndex == 1)
            {
                pivot1.SelectedIndex = 0;
            }
            else if (pivot1.SelectedIndex == 2)
            {
                pivot1.SelectedIndex = 1;
            }
            else if (pivot1.SelectedIndex == 3)
            {
                pivot1.SelectedIndex = 2;
            }
           
        }

      
        #endregion



        #region utils event handled
        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/UserAgent.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (pivot1.SelectedIndex == 0)
            {
                setFullScreem(textBox1, button1, webBrowser1,ProgBar1);

            }
            else if (pivot1.SelectedIndex == 1)
            {
                setFullScreem(textBox2, button2, webBrowser2,ProgBar2);

            }
            else if (pivot1.SelectedIndex == 2)
            {
                setFullScreem(textBox3, button3, webBrowser3,ProgBar3);

            }
            else if (pivot1.SelectedIndex == 3)
            {
                setFullScreem(textBox4, button4, webBrowser4,ProgBar4);

            }
          //  MessageBox.Show(Cadenas.Shake);
        }

       
        private void pivot1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            tap.Text = Cadenas.Tap + " "+(pivot1.SelectedIndex + 1).ToString();

         
            

          
        }

        #endregion

      



        private void saveAllinBrowser()
        {

            util.SaveBrowserContent(textBox1.Text, "browser_url1.txt", pivot1.SelectedIndex.ToString());
            util.SaveBrowserContent(textBox2.Text, "browser_url2.txt", pivot1.SelectedIndex.ToString());
            util.SaveBrowserContent(textBox3.Text, "browser_url3.txt", pivot1.SelectedIndex.ToString());
            util.SaveBrowserContent(textBox4.Text, "browser_url4.txt", pivot1.SelectedIndex.ToString());
        }

        private void loadAllinBrowser()
        {


            try
            {

                IsolatedStorageFile appIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

                string[] aux = new string[2];

                aux = util.ReadUrlContent("browser_url1.txt");
                pivot1.SelectedIndex = Int32.Parse(aux[0]);
                tap.Text = "Tap " + (Int32.Parse(aux[0]) + 1).ToString();
                if (!String.IsNullOrEmpty(aux[1]))
                {
                    textBox1.Text = aux[1];
                    // if (appIsolatedStorage.FileExists("/browser1.html"))
                  //  webBrowser1.Navigate(new Uri(aux[1], UriKind.RelativeOrAbsolute));//k[0]+
                }

                aux = util.ReadUrlContent("browser_url2.txt");
                pivot1.SelectedIndex = Int32.Parse(aux[0]);
                tap.Text = "Tap " + (Int32.Parse(aux[0]) + 1).ToString();
                if (!String.IsNullOrEmpty(aux[1]))
                {
                    textBox2.Text = aux[1]; 
                 //   webBrowser2.Navigate(new Uri(aux[1], UriKind.RelativeOrAbsolute));
                }
                    
                aux = util.ReadUrlContent("browser_url3.txt");
                pivot1.SelectedIndex = Int32.Parse(aux[0]);
                tap.Text = "Tap " + (Int32.Parse(aux[0]) + 1).ToString();
                if (!String.IsNullOrEmpty(aux[1]))
                { 
                     textBox3.Text = aux[1];
                  //   webBrowser3.Navigate(new Uri(aux[1], UriKind.RelativeOrAbsolute));
                }

                 


                aux = util.ReadUrlContent("browser_url4.txt");
                pivot1.SelectedIndex = Int32.Parse(aux[0]);
                tap.Text = "Tap " + (Int32.Parse(aux[0]) + 1).ToString();
                if (!String.IsNullOrEmpty(aux[1]))
                {
                    textBox4.Text = aux[1];
                  //  webBrowser4.Navigate(new Uri(aux[1], UriKind.RelativeOrAbsolute));
                }
            }
            catch { }

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
            loadAllinBrowser();

           

           openInBrowser();
        }

        protected void openInBrowser()
        {

            string m = "";
            string uri = "";
            NavigationContext.QueryString.TryGetValue("m", out m);
            if (m == "openbrowser")
            {
              

                if (NavigationContext.QueryString.TryGetValue("uri", out uri))
                {

                    if (pivot1.SelectedIndex == 0)
                    {
                        webBrowser1.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                    }

                    if (pivot1.SelectedIndex == 1)
                    {
                        webBrowser2.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                    }

                    if (pivot1.SelectedIndex == 2)
                    {
                        webBrowser3.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                    }

                    if (pivot1.SelectedIndex == 3)
                    {
                        webBrowser4.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                    }
                }
                NavigationContext.QueryString.Clear();

            }
         
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (this.Orientation == PageOrientation.LandscapeLeft || this.Orientation == PageOrientation.LandscapeRight || this.Orientation == PageOrientation.Landscape)
            {
                textBox1.Width = 600;
                textBox2.Width = 600;
                textBox3.Width = 600;
                textBox4.Width = 600;
                btNext1.Margin = new Thickness(550, 0, 0, 0);
                btBack1.Margin = new Thickness(485, 0, 0, 0);
                Fullscreen.Margin = new Thickness(420, 0, 0, 0);
            }
            else 
            {
                btNext1.Margin = new Thickness(381, 0, 0, 0);
                btBack1.Margin = new Thickness(317, 0, 0, 0);
                Fullscreen.Margin = new Thickness(247, 0, 0, 0);
            }

             //MessageBox.Show(grid1.Width.ToString());

        }


        #region ApplicationBarMenuItem_Click_3
        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            if (pivot1.SelectedIndex == 0)
            {

                SaveImgMethod(textBox1, webBrowser1);
            }


            if (pivot1.SelectedIndex == 1)
            {

                SaveImgMethod(textBox2, webBrowser2);
            }


            if (pivot1.SelectedIndex == 2)
            {

                SaveImgMethod(textBox3, webBrowser3);
            }


            if (pivot1.SelectedIndex == 3)
            {

                SaveImgMethod(textBox3, webBrowser3);
            }

        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/about.xaml", UriKind.RelativeOrAbsolute));

        }

        private string scode;
         
        private void ApplicationBarMenuItem_Click_3(object sender, EventArgs e)
        {
            //version gratis
            //if(pivot1.SelectedIndex==0)
            //((System.Windows.Controls.Frame)this.Parent).DataContext = webBrowser1.SaveToString();

            //if (pivot1.SelectedIndex == 1)
            //    ((System.Windows.Controls.Frame)this.Parent).DataContext = webBrowser2.SaveToString();

            //if (pivot1.SelectedIndex == 2)
            //    ((System.Windows.Controls.Frame)this.Parent).DataContext = webBrowser3.SaveToString();

            //if (pivot1.SelectedIndex == 3)
            //    ((System.Windows.Controls.Frame)this.Parent).DataContext = webBrowser4.SaveToString();

            //NavigationService.Navigate(new Uri("/sourcecode.xaml", UriKind.RelativeOrAbsolute));

            if (pivot1.SelectedIndex == 0)
            {
                scode = webBrowser1.SaveToString();
               // scode = (scode.Length < 120000) ? scode : scode.Substring(0, 120000);
                webBrowser1.Navigate(new Uri("/source.html", UriKind.Relative));
            }

            if (pivot1.SelectedIndex == 1)
            {
                scode = webBrowser2.SaveToString();
                //scode = (scode.Length < 120000) ? scode : scode.Substring(0, 120000);
                webBrowser2.Navigate(new Uri("/source.html", UriKind.Relative));
            }

            if (pivot1.SelectedIndex == 2)
            {
                scode = webBrowser3.SaveToString();
               // scode = (scode.Length < 120000) ? scode : scode.Substring(0, 120000);
                webBrowser3.Navigate(new Uri("/source.html", UriKind.Relative));
            }

            if (pivot1.SelectedIndex == 3)
            {
                scode = webBrowser4.SaveToString();
               // scode = (scode.Length < 120000) ? scode : scode.Substring(0, 120000);
                webBrowser4.Navigate(new Uri("/source.html", UriKind.Relative));
            }

            scode = scode.Replace("<html", "< html");
            scode = scode.Replace("<Html", "< html");
            scode = scode.Replace("<HTML", "< html");

            scode = scode.Replace("<head", "< head");
            scode = scode.Replace("<Head", "< head");
            scode = scode.Replace("<HEAD", "< head");

            scode = scode.Replace("<body", "< body");
            scode = scode.Replace("<Body", "< body");
            scode = scode.Replace("<BODY", "< body");

            scode = scode.Replace("<pre", "< pre");
            scode = scode.Replace("<Pre", "< pre");
            scode = scode.Replace("<PRE", "< pre");

            scode = scode.Replace("</pre", "< / pre");
            scode = scode.Replace("</Pre", "< / pre");
            scode = scode.Replace("</PRE", "< / pre");




            scode = scode.Replace("<script", "< script");
            scode = scode.Replace("<Script", "< script");
            scode = scode.Replace("<SCRIPT", "< script");

            scode = scode.Replace("</script", "< / script");
            scode = scode.Replace("</Script", "< / script");
            scode = scode.Replace("</SCRIPT", "< / script");



            scode = scode.Replace("</html", "< / html");
            scode = scode.Replace("</Html", "< / html");
            scode = scode.Replace("</HTML", "< / html");


            scode = scode.Replace("</head", "< / head");
            scode = scode.Replace("</Head", "< / head");
            scode = scode.Replace("</HEAD", "< / head");

            scode = scode.Replace("</body", "< / body");
            scode = scode.Replace("</Body", "< / body");
            scode = scode.Replace("</BODY", "< / body");
        }
        
        private void ApplicationBarMenuItem_Click_4(object sender, EventArgs e)
        {
            try
            {
                var s= new CookieCollection();
                
                if (pivot1.SelectedIndex == 0)
                {
                    s = webBrowser1.GetCookies();

                    ((System.Windows.Controls.Frame)this.Parent).DataContext = s;
                    NavigationService.Navigate(new Uri("/seeCookie.xaml?url=" + textBox1.Text, UriKind.RelativeOrAbsolute));
                }
                if (pivot1.SelectedIndex == 1)
                {
                    s = webBrowser2.GetCookies();

                    ((System.Windows.Controls.Frame)this.Parent).DataContext = s;
                    NavigationService.Navigate(new Uri("/seeCookie.xaml?url=" + textBox2.Text, UriKind.RelativeOrAbsolute));
                }
                if (pivot1.SelectedIndex == 2)
                {
                    s = webBrowser3.GetCookies();

                    ((System.Windows.Controls.Frame)this.Parent).DataContext = s;
                    NavigationService.Navigate(new Uri("/seeCookie.xaml?url=" + textBox3.Text, UriKind.RelativeOrAbsolute));
                }
                if (pivot1.SelectedIndex == 3)
                {
                    s = webBrowser4.GetCookies();

                    ((System.Windows.Controls.Frame)this.Parent).DataContext = s;
                    NavigationService.Navigate(new Uri("/seeCookie.xaml?url=" + textBox4.Text, UriKind.RelativeOrAbsolute));
                }


            }

            catch{

            MessageBox.Show("No cookie to show");
            }
        }
        #endregion


        #region webBrowser_NavigationFailed
        private void webBrowser1_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            ProgBar1.Visibility = Visibility.Collapsed;
            setImageBackgroundButton(button1, "Images/reload.jpg");
            searchEngine();
        }

        private void webBrowser2_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            ProgBar2.Visibility = Visibility.Collapsed;
            setImageBackgroundButton(button2, "Images/reload.jpg");
            searchEngine();
        }

        private void webBrowser3_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            ProgBar3.Visibility = Visibility.Collapsed;
            setImageBackgroundButton(button3, "Images/reload.jpg");
            searchEngine();
        }

        private void webBrowser4_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            ProgBar4.Visibility = Visibility.Collapsed;
            setImageBackgroundButton(button4, "Images/reload.jpg");
            searchEngine();
        }
        #endregion

        private void SaveImgMethod(System.Windows.Controls.TextBox textBox, WebBrowser webBrowser)
        {

            Match host = Regex.Match(textBox.Text, "http://[ a-z|A-Z|\\:\\.\\-_0-9]*(\\/|)");

            int i = 0;
            MatchCollection Mcoll = Regex.Matches(webBrowser.SaveToString(), "src=\"[ a-z|A-Z|\\:\\/\\.\\-_0-9]*(.jpg|.png)");

            string[] aux = new string[Mcoll.Count];
            foreach (Match m in Mcoll)
            {

                if (!Regex.IsMatch(m.Value, "http://"))
                {
                    aux[i] = host.ToString() + m.Value.Replace("src=\"", "");
                    aux[i] = aux[i].Replace("//", "/");
                }
                else
                {

                    aux[i] = m.Value.Replace("src=\"", "");
                }


                i++;
            }


            ((System.Windows.Controls.Frame)this.Parent).DataContext = aux;


            NavigationService.Navigate(new Uri("/SaveImg.xaml?url=" + textBox.Text, UriKind.RelativeOrAbsolute));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (_isFullScren)
            {

                QuitFullScreen();
            }
            else
            {

                if (pivot1.SelectedIndex == 0)
                {
                    if (_navigationStack1.Count() >= 2)
                    {
                        _navigationStack1.Pop();
                        webBrowser1.Navigate(_navigationStack1.Pop());
                        e.Cancel = true;  // This prevents the default functionality of the back button.
                        return;
                    }
                }
                if (pivot1.SelectedIndex == 1)
                {
                    if (_navigationStack2.Count() >= 2)
                    {
                        _navigationStack2.Pop();
                        webBrowser2.Navigate(_navigationStack2.Pop());
                        e.Cancel = true;  // This prevents the default functionality of the back button.
                        return;
                    }
                }
                if (pivot1.SelectedIndex == 2)
                {
                    if (_navigationStack3.Count() >= 2)
                    {
                        _navigationStack3.Pop();
                        webBrowser3.Navigate(_navigationStack3.Pop());
                        e.Cancel = true;  // This prevents the default functionality of the back button.
                        return;
                    }
                }
                if (pivot1.SelectedIndex == 3)
                {
                    if (_navigationStack4.Count() >= 2)
                    {
                        _navigationStack4.Pop();
                        webBrowser4.Navigate(_navigationStack4.Pop());
                        e.Cancel = true;  // This prevents the default functionality of the back button.
                        return;
                    }
                }



                MessageBoxResult result = MessageBox.Show(Cadenas.Exit,
               Cadenas.Warning, MessageBoxButton.OKCancel);
                e.Cancel = true;

                if (result == MessageBoxResult.OK)
                {
                    e.Cancel = false;
                    base.OnBackKeyPress(e);
                }
            }

            e.Cancel = true;  // This prevents the default functionality of the back button.
        }


        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/history.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            saveAllinBrowser();

        }

        private void textBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = textBox1.Text.Length;
        }

        private void textBox2_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox2.SelectionStart = 0;
            textBox2.SelectionLength = textBox2.Text.Length;
        }

        private void textBox3_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox3.SelectionStart = 0;
            textBox3.SelectionLength = textBox3.Text.Length;
        }

        private void textBox4_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox4.SelectionStart = 0;
            textBox4.SelectionLength = textBox4.Text.Length;
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            string tit = "";
            string uri = "";



            try
            {
                if (pivot1.SelectedIndex == 0)
                {

                    uri = textBox1.Text;
                    tit = (string)webBrowser1.InvokeScript("eval", "document.title.toString()");

                }
                else if (pivot1.SelectedIndex == 1)
                {

                    uri = textBox2.Text;
                    tit = (string)webBrowser2.InvokeScript("eval", "document.title.toString()");

                }
                else if (pivot1.SelectedIndex == 2)
                {

                    uri = textBox3.Text;
                    tit = (string)webBrowser3.InvokeScript("eval", "document.title.toString()");

                }
                else if (pivot1.SelectedIndex == 3)
                {

                    uri = textBox4.Text;
                    tit = (string)webBrowser4.InvokeScript("eval", "document.title.toString()");
                }
            }
            catch { }

            NavigationService.Navigate(new Uri("/addFavorites.xaml?name=" + tit + "&uri=" + uri , UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            
             NavigationService.Navigate(new Uri("/favorites.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_3(object sender, EventArgs e)
        {
            //string aux ="";
            if(pivot1.SelectedIndex == 0)
                webBrowser1.Navigate(new Uri("/app.html", UriKind.Relative));
            if (pivot1.SelectedIndex == 1)
                webBrowser2.Navigate(new Uri("/app.html", UriKind.Relative));
            if (pivot1.SelectedIndex == 2)
                webBrowser3.Navigate(new Uri("/app.html", UriKind.Relative));
            if (pivot1.SelectedIndex == 3)
                webBrowser4.Navigate(new Uri("/app.html", UriKind.Relative));

            
          // NavigationService.Navigate(new Uri("/share.xaml?url=" + aux, UriKind.Relative));
        }

        private void webBrowser1_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (webBrowser1.Source == new Uri("/app.html", UriKind.Relative))
            {
                webBrowser1.InvokeScript("seturl", textBox1.Text);
               
            }
            if (webBrowser1.Source == new Uri("/source.html", UriKind.Relative))
            {
                   webBrowser1.InvokeScript("setcode", scode);

            }
        }

        private void webBrowser2_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (webBrowser2.Source == new Uri("/app.html", UriKind.Relative))
            {
                webBrowser2.InvokeScript("seturl", textBox2.Text);

            }
            if (webBrowser2.Source == new Uri("/source.html", UriKind.Relative))
            {
                webBrowser2.InvokeScript("setcode", scode);

            }
        }

        private void webBrowser3_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (webBrowser3.Source == new Uri("/app.html", UriKind.Relative))
            {
                webBrowser3.InvokeScript("seturl", textBox3.Text);

            }
            if (webBrowser3.Source == new Uri("/source.html", UriKind.Relative))
            {
                webBrowser3.InvokeScript("setcode", scode);

            }
        }

        private void webBrowser4_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (webBrowser4.Source == new Uri("/app.html", UriKind.Relative))
            {
                webBrowser4.InvokeScript("seturl", textBox4.Text);

            }
            if (webBrowser4.Source == new Uri("/source.html", UriKind.Relative))
            {
                webBrowser4.InvokeScript("setcode", scode);

            }
        }

        private void ApplicationBarMenuItem_Click_5(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/setting.xaml", UriKind.Relative));
        }


        private void searchEngine() 
        { 
        
            string seng = "";
                    string aux = util.readSEngine_file();
                    if (aux == "Google")
                    {
                        seng = "https://www.google.co.cr/search?q=";
                    }
                    else if (aux == "Yahoo")
                    {

                        seng = "http://search.yahoo.com/search?p=";
                    }
                    else if (aux == "Bing")
                    {

                        seng = "http://www.bing.com/search?q=";
                    }


                    if (pivot1.SelectedIndex == 0)
                    {
                        textBox1.Text = seng + toSearch;

                        this.Dispatcher.BeginInvoke(() =>
                        {
                            webBrowser1.Navigate(new Uri(seng + toSearch, UriKind.Absolute));
                        });
                        
                        

                    }
                    else if (pivot1.SelectedIndex == 1)
                    {
                        textBox2.Text = seng + toSearch;
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            webBrowser2.Navigate(new Uri(seng + toSearch, UriKind.Absolute));
                        });
                        
                        
                    }
                    else if (pivot1.SelectedIndex == 2)
                    {

                        textBox3.Text = seng + toSearch;
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            webBrowser3.Navigate(new Uri(seng + toSearch, UriKind.Absolute));
                        });
                    }
                    else if (pivot1.SelectedIndex == 3)
                    {
                        textBox4.Text = seng + toSearch;
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            webBrowser4.Navigate(new Uri(seng + toSearch, UriKind.Absolute));
                        });
                    }
        }
        

       

     
    }

     

     

}