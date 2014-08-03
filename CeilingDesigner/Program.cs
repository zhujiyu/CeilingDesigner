using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CeilingDesigner
{
    static class Program
    {
        public static void WriteErr(Exception ex, string file)
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(file,
                    System.IO.FileMode.Create);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                sw.Write(ex.ToString());
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception _ex)
            {
                MessageErr(_ex, file);
            }
        }

        public static void MessageErr(Exception ex, string logFile)
        {
            //MessageBox.Show(ex.ToString(), ShareData.AppName);
            MessageBox.Show("发生以下错误，请联系软件开发商获得帮助。\n\n"
                + ex.Message + "\n错误日志： " + logFile
                + "\n\n将错误日志文件发送至以下联系方式："
                + "\n邮箱: zhuhz82@126.com  \nQQ:  123728555\n",
                ShareData.AppName);
        }

        public static string GetLogFile(DateTime date)
        {
            return "err" + date.Year + (date.Month < 10 ? "0" : "") + date.Month
                + (date.Day < 10 ? "0" : "") + date.Day + ".log";
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form());
                Application.Run(new PalaceForm());
            }
            catch (Exception ex)
            {
                string logFile = ShareData.GetErrPath() + GetLogFile(DateTime.Now);
                WriteErr(ex, logFile);
                MessageErr(ex, logFile);
            }
        }
    }
}
