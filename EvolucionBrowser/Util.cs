using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;

namespace EvolucionBrowser
{
    public class Util
    {

        private IsolatedStorageFile almacenamientoArchivos = IsolatedStorageFile.GetUserStoreForApplication();

        public void setUserAgent_file(string opt)
        {
            StreamWriter file = new StreamWriter(new IsolatedStorageFileStream("UserAgent.txt", FileMode.OpenOrCreate, almacenamientoArchivos));

            file.Write(opt);
            file.Close();

        }

        public string readUserAgent_file()
        {
            string text = "";
            if (almacenamientoArchivos.FileExists("UserAgent.txt"))
            {
                StreamReader file = new StreamReader(almacenamientoArchivos.OpenFile("UserAgent.txt", FileMode.Open, FileAccess.Read));

                text = file.ReadLine();
                file.Close();
            }
            return text;

        }

        public void setCustomUserAgent_file(string UA)
        {
            StreamWriter file = new StreamWriter(new IsolatedStorageFileStream("CustomUserAgent.txt", FileMode.OpenOrCreate, almacenamientoArchivos));

            file.Write(UA);
            file.Close();

        }

        public string readCustomUserAgent_file()
        {
            string text = "";
            if (almacenamientoArchivos.FileExists("CustomUserAgent.txt"))
            {
                StreamReader file = new StreamReader(almacenamientoArchivos.OpenFile("CustomUserAgent.txt", FileMode.Open, FileAccess.Read), Encoding.UTF8);

                text = file.ReadLine();
                file.Close();
            }
            return text;

        }

       

        public void SaveBrowserContent(string uri, string UrlFilename, string indexTap)
        {
            try
            {
               

                StreamWriter file = new StreamWriter(new IsolatedStorageFileStream(UrlFilename, FileMode.Create, almacenamientoArchivos), Encoding.UTF8);

                file.WriteLine(indexTap);
                file.WriteLine(uri);
                file.Close();
            }catch {}
        }

        public string[] ReadUrlContent(string filename)
        {
            string[] text = new string[2];


            if (almacenamientoArchivos.FileExists(filename))
            {
                StreamReader file = new StreamReader(almacenamientoArchivos.OpenFile(filename, FileMode.Open), Encoding.UTF8);

                text[0] = file.ReadLine();
                text[1] = file.ReadLine();

                file.Close();
            }
            return text;

        }



        //---------------------------------------------------------------------------------------------------------------------------
        public void setSEngine_file(string se)
        {
            StreamWriter file = new StreamWriter(new IsolatedStorageFileStream("SearchEngine.txt", FileMode.Create, almacenamientoArchivos));

            file.Write(se);
            file.Close();

        }

        public string readSEngine_file()
        {
            string text = "";
            if (almacenamientoArchivos.FileExists("SearchEngine.txt"))
            {
                StreamReader file = new StreamReader(almacenamientoArchivos.OpenFile("SearchEngine.txt", FileMode.Open, FileAccess.Read), Encoding.UTF8);

                text = file.ReadLine();
                file.Close();
            }
            return text;

        }
       
    }


}
