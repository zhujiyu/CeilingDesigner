using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CeilingDesigner
{
    public class ShareData
    {
        //static string comp = "奥普1+N浴顶";
        //static string comp = Palace.Properties.Settings.Default.AppName;
        static string appName = null;

        public static string AppName
        {
            get
            {
                if (appName == null)
                    appName = SettingFile.GetAppName();
                return appName;
            }
        }

        private static bool hasNetServer = false;

        public static bool HasNetServer
        {
            get { return ShareData.hasNetServer; }
            set { ShareData.hasNetServer = value; }
        }

        static string[] paths = { "", "../", "images/", "../images/", "data/", "../data/" };

        public static string[] Paths
        {
            get { return ShareData.paths; }
        }

        private static string server = "ud60187.hichina.com"; //ud60187.hichina.com/

        public static string Server
        {
            get { return ShareData.server; }
            set { ShareData.server = value; }
        }

        private static string _connectString;
        private static global::MySql.Data.MySqlClient.MySqlConnection _connection;

        public static global::MySql.Data.MySqlClient.MySqlConnection Connection
        {
            get
            {
                if (ShareData._connection == null)
                {
                    ShareData._connectString = CeilingDesigner.Properties.Settings.Default.palaceConnectionString;
                    ShareData._connection = new MySql.Data.MySqlClient.MySqlConnection();
                    ShareData._connection.ConnectionString = CeilingDesigner.Properties.Settings.Default.palaceConnectionString;
                }

                return ShareData._connection;
            }
        }

        public static string ConnectString
        {
            get { return ShareData._connectString; }
            set
            {
                ShareData._connectString = value;
                ShareData._connection.ConnectionString = value;
            }
        }

        public static object current = null;
        public static List<object> list = new List<object>();

        public static CeilingDataSet CeilingDataSet = new CeilingDataSet();
        public static PalaceForm form = null;

        static public string GetDataPath()
        {
            string baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string path = null;
            string[] paths = { "data\\", "..\\data\\" };

            for (int i = 0; i < paths.Length; i++)
            {
                if (System.IO.Directory.Exists(baseDirectory + paths[i]))
                {
                    path = baseDirectory + paths[i];
                    break;
                }
            }

            if (path == null)
            {
                path = paths[0];
                System.IO.Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string GetFilePath(string file)
        {
            string baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            for (int i = 0; i < paths.Length; i++)
            {
                if (System.IO.File.Exists(baseDirectory + paths[i] + file))
                {
                    return baseDirectory + paths[i] + file;
                }
            }

            return null;
        }

        public static string GetErrPath()
        {
            string path = GetDataPath() + "err\\";

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            return path;
        }

        public static string GetTempPath()
        {
            string path = GetDataPath() + "temp\\";

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            return path;
        }

        //public ShareData() { }
    }
}
