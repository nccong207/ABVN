using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CDTLib;

namespace PhanBoCN
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SetConfig();
            Application.Run(new FrmPhanBoCN("131"));
        }

        static private void SetConfig()
        {
            Config.NewKeyValue("DataConnection", @"Server = abvn.no-ip.org\hoatieu,1433; Database = STDABV; User = sa; Password = ht");
            Config.NewKeyValue("PackageName", "HoaTieu Business.Net");
            Config.NewKeyValue("NamLamViec", 2013);
            Config.NewKeyValue("KyKeToan", 7);
        }
    }
}