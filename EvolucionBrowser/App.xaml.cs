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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Phone.Marketplace;

namespace EvolucionBrowser
{
    
    public partial class App : Application
    {

        private static LicenseInformation _licenseInfo = new LicenseInformation();
        private void checkLicense()
        {
            // When debugging, we want to simulate a trial mode experience. The following conditional allows us to set the _isTrial 
            // property to simulate trial mode being on or off. 
#if DEBUG
            string message = "This sample demonstrates the implementation of a trial mode in an application." +
                               "Press 'OK' to simulate trial mode. Press 'Cancel' to run the application in normal mode.";
            if (MessageBox.Show(message, "Debug Trial",
                 MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _isTrial = true;
            }
            else
            {
                _isTrial = false;
            }
#else
            _isTrial = _licenseInfo.IsTrial();
#endif
        }
        private static bool _isTrial = true;
        public bool IsTrial
        {
            get
            {
                return _isTrial;
            }
        }

        IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
        public const string ConnectionString = @"isostore:/evolucionBrowserDB.sdf";

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            checkLicense();
            try
            {
                using (evolucionBrowserDataContext context = new evolucionBrowserDataContext(ConnectionString))
                {
                    if (!context.DatabaseExists())
                    {
                        // create database if it does not exist
                        context.CreateDatabase();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Creating data base",


                        MessageBoxButton.OK);
            }


             string[] addr = new string[3] { "/EvolucionBrowser;component/share/facebook.jpg", "/EvolucionBrowser;component/share/twitter.jpg", "/EvolucionBrowser;component/share/googleplus.jpg" };
            string[] isol = new string[3] { "facebook.jpg", "twitter.jpg", "googleplus.jpg" };

            string[] fileLocal = new string[7] { "source/source.html", "source/highlight.js", "source/highlight.css", "share/app.html", "share/jquery.min.js", "share/jquery.mobile.min.css", "share/jquery.mobile.min.js" };
            string[] fileIsol = new string[7] { "source.html", "highlight.js", "highlight.css", "app.html", "jquery.min.js", "jquery.mobile.min.css", "jquery.mobile.min.js" };

            SaveImagesToIsoStore(addr,isol);
            saveFileToIsoStore(fileLocal, fileIsol);

            Util util = new Util();
            if(string.IsNullOrEmpty(util.readSEngine_file()) )
            util.setSEngine_file("Google");

        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            checkLicense();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion




        //----------------------------------------------------------------------------------------------------------------------------------------

        private void saveFileToIsoStore(string[] localfiles, string[] isolatedfiles)
        {
            int i = 0;
            if (false == isoStore.FileExists(isolatedfiles[0]))
            {
                foreach (string f in localfiles)
                {
                    using (StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(isolatedfiles[i], FileMode.Create, FileAccess.Write, isoStore)))
                    {
                        StreamReader reader = new StreamReader(
                           TitleContainer.OpenStream(f));

                        writer.Write(reader.ReadToEnd());

                        writer.Close();
                    }
                    i++;
                }
            }
        }



        //---------------------------------------------------------------------------------------------------------------------------------------
        private void SaveImagesToIsoStore(string[] localfiles, string[] isolatedfiles)
        {


            int i = 0;
            if (false == isoStore.FileExists(isolatedfiles[0]))
            {
                foreach (string f in localfiles)
                {
                    StreamResourceInfo sr = Application.GetResourceStream(new Uri(f, UriKind.RelativeOrAbsolute));
                    using (BinaryReader br = new BinaryReader(sr.Stream))
                    {
                        byte[] data = br.ReadBytes((int)sr.Stream.Length);
                        SaveToIsoStore(isolatedfiles[i], data);
                    }

                    i++;
                }
            }
        }

        private void SaveToIsoStore(string fileName, byte[] data)
        {
            string strBaseDir = string.Empty;
            string delimStr = "/";
            char[] delimiter = delimStr.ToCharArray();
            string[] dirsPath = fileName.Split(delimiter);

            //Get the IsoStore.
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            //Re-create the directory structure.
            for (int i = 0; i < dirsPath.Length - 1; i++)
            {
                strBaseDir = System.IO.Path.Combine(strBaseDir, dirsPath[i]);
                isoStore.CreateDirectory(strBaseDir);
            }

            //Remove the existing file.
            if (isoStore.FileExists(fileName))
            {
                isoStore.DeleteFile(fileName);
            }

            //Write the file.
            using (BinaryWriter bw = new BinaryWriter(isoStore.CreateFile(fileName)))
            {
                bw.Write(data);
                bw.Close();
            }
        }
      


        

    }

    
    public class hist
        {
            
            public string title { get; set; }

            public string uri { get; set; }
        }

    public class fav
    {

        public string title { get; set; }

        public string uri { get; set; }
    }



    

}