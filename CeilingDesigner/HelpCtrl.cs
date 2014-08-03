using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Drawing;
using System.IO;
//using System.Data;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CeilingDesigner
{
    public partial class HelpCtrl : UserControl
    {
        public HelpCtrl()
        {
            InitializeComponent();
        }

        private void KnowLabel_Click(object sender, EventArgs e)
        {
            PalaceForm pfrm = this.ParentForm as PalaceForm;
            pfrm.SetDoMenu(pfrm.CurOrderGraph);
            pfrm.SetOtherMenu(true);
            pfrm.SetWallMenu(true);
            pfrm.SetManageMenu(true);

            //pfrm.SetEditMenu(false);
            //pfrm.SetCeilingMenu(true);
            //pfrm.SetTileModuleMenu(false);

            this.Dispose();
        }

        static private String getsetting()
        {
            return ShareData.GetDataPath() + "/setting.txt";
        }

        static public bool IsHide()
        {
            List<string> lines = SettingFile.Read();
            if (lines == null)
                return false;
            string help = SettingFile.GetParamValue(lines, "help");
            if (help == "off")
                return true;

            return false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SettingFile.Write("Help", "off");
        }
    }
}

//Byte[] bytes = new Byte[256];
//string file = getsetting();
//if (!File.Exists(file))
//    return false;

//FileStream stream = File.OpenRead(file);
//StreamReader sr = new StreamReader(stream);

//String str = sr.ReadLine();
//while (str != null)
//{
//    MatchCollection ms = Regex.Matches(str, @"\s?Help\s?:\s?([a-zA-z]+)");
//    if (ms.Count > 0)
//    {
//        String off = ms[0].Groups[1].Value.ToString().ToLower();
//        if (off == "off")
//            return true;
//    }
//}

//stream.Read(bytes, 0, 256);
//String str = Encoding.Default.GetString(bytes);

//MatchCollection ms1 = Regex.Matches(str, @"Help:");
//MatchCollection ms = Regex.Matches(str, @"Help:\s?([a-zA-z]+)");
//if (ms.Count > 0)
//{
//    String off = ms[0].Groups[1].Value.ToString().ToLower();
//    if (off == "off")
//        return true;
//}

//return false;

//List<string> lines = SettingFile.Read();
//int helpline = lines != null ? SettingFile.GetParamLine(lines, "help") : -1;

//if (lines == null)
//    lines = new List<string>();
//if (helpline == -1)
//{
//    lines.Add("help:off");
//}
//else
//{
//    lines[helpline] = "Help: off";
//}

//SettingFile.Write(lines);

//FileStream stream = File.OpenWrite(getsetting());

//Byte[] rbytes = new Byte[256];
//stream.Read(rbytes, 0, 256);
//String str = Encoding.Default.GetString(rbytes);
//MatchCollection ms1 = Regex.Matches(str, @"Help:");

//Byte[] wbytes = Encoding.Default.GetBytes("Help: off\n");
//stream.Write(wbytes, 0, wbytes.Length);
//stream.Close();
