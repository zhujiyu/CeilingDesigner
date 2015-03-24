using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace CeilingDesigner
{
    class SettingFile
    {
        private static String getsetting()
        {
            return ShareData.GetDataPath() + "/setting.txt";
        }

        public static string GetParamValue(List<string> lines, string reg)
        {
            reg = reg.ToLower();

            for (int i = 0; i < lines.Count; i++)
            {
                string str = lines[i].ToLower();
                if (str.Length < 1)
                    continue;
                MatchCollection ms = Regex.Matches(str, @"\s?" + reg 
                    + @"\s?[:：]\s?([\w-\(\)\ \+]+)");
                if (ms.Count > 0)
                    return ms[0].Groups[1].Value.ToString();
            }

            return null;
        }

        public static string GetPhoto()
        {
            List<string> lines = SettingFile.Read();
            string photo = lines != null ? SettingFile.GetParamValue(lines, "Server Tel") : "";
            return photo != null && photo.Length > 0 ? photo : "010-68240064";
        }

        //return lines != null ? SettingFile.GetParamValue(lines, "Server Tel") : "010-68240064";
        //return lines != null ? SettingFile.GetParamValue(lines, "Server Tel") : "010-68240064";

        public static string GetAddress()
        {
            List<string> lines = SettingFile.Read();
            string address = lines != null ? SettingFile.GetParamValue(lines, "address") : "";
            return address != null && address.Length > 0 ?
                address : "北京市海淀区复兴路乙20号汇通商务楼";
        }

        public static string GetCmpName()
        {
            List<string> lines = SettingFile.Read();
            string cmpNmae = lines != null ? SettingFile.GetParamValue(lines, "compeny") : "";
            return cmpNmae != null && cmpNmae.Length > 0 ?
                cmpNmae : "杭州奥普电器有限公司北京分公司";
        }

        public static string GetAppName()
        {
            //return Properties.Settings.Default.AppName;
            List<string> lines = SettingFile.Read();
            string appNmae = lines != null ? SettingFile.GetParamValue(lines, "AppName") : "";
            return appNmae != null && appNmae.Length > 0 ?
                appNmae.ToUpper() : Properties.Settings.Default.AppName;
        }

        public static string GetRptName()
        {
            List<string> lines = SettingFile.Read();
            string rptNmae = lines != null ? SettingFile.GetParamValue(lines, "report") : "";
            return rptNmae != null && rptNmae.Length > 0 ?
                rptNmae : "奥普吊顶装修订单";
        }

        public static List<string> Read()
        {
            string file = getsetting();
            if (!File.Exists(file))
                return null;

            FileStream stream = File.OpenRead(file);
            StreamReader sr = new StreamReader(stream, Encoding.Default);
            List<string> lines = new List<string>();

            String str = sr.ReadLine();
            while (str != null)
            {
                if (str.Length > 0 && str[0] != '#' && str[0] != ',')
                    lines.Add(str);
                str = sr.ReadLine();
            }
            stream.Close();

            return lines;
        }

        public static void Write(string key, string value)
        {
            string file = getsetting();
            if (!File.Exists(file))
                File.Create(file);
            FileStream rStream = File.OpenRead(file);
            StreamReader rStr = new StreamReader(rStream, Encoding.Default);

            List<string> rLines = new List<string>();
            String str = rStr.ReadLine();
            while (str != null)
            {
                rLines.Add(str);
                str = rStr.ReadLine();
            }
            rStream.Close();

            FileStream wStream = File.OpenWrite(file);
            StreamWriter wStr = new StreamWriter(wStream, Encoding.Default);
            bool writed = false;

            try
            {
                for (int i = 0; i < rLines.Count; i++)
                {
                    MatchCollection ms = Regex.Matches(rLines[i], @"\s?" + key
                        + @"\s?[:：]\s?([\w-\(\)\ \+]+)");
                    if (ms.Count > 0)
                    {
                        wStr.WriteLine(key + "：" + value);
                        writed = true;
                    }
                    else
                        wStr.WriteLine(rLines[i]);
                }

                if (!writed)
                {
                    wStr.WriteLine();
                    wStr.WriteLine();
                    wStr.WriteLine(key + "：" + value);
                    wStr.WriteLine();
                    wStr.WriteLine();
                }

                wStr.Flush();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
            finally
            {
                wStream.Flush();
                wStream.Close();
            }
        }
    }
}

//public static int GetParamLine(List<string> lines, string reg)
//{
//    for (int i = 0; i < lines.Count; i++)
//    {
//        string str = lines[i].ToLower();
//        MatchCollection ms = Regex.Matches(str, reg);
//        if (ms.Count > 0)
//        {
//            return i;
//        }
//    }
//    return -1;
//}
