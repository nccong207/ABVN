using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using CDTLib;
using DevExpress;
using DevExpress.XtraEditors;
using Plugins;
namespace TimKiemCatalog
{
    public class  TimKiemCatalog: IC
    {
        #region IC Members
        
        private List<InfoCustom> _lstInfo = new List<InfoCustom>();

        public TimKiemCatalog()
        {
            InfoCustom ic = new InfoCustom(1176, "Tìm kiếm theo catalog", "Tìm kiếm");
            _lstInfo.Add(ic);
        }

        public void Execute(System.Data.DataRow drMenu)
        {
            int menuID = Int32.Parse(drMenu["MenuPluginID"].ToString());
            if (_lstInfo[0].CType == ICType.Custom && _lstInfo[0].MenuID == menuID)
            {
                frmShowDialog frm = new frmShowDialog();
                frm.Text = "Tham số tìm kiếm";
                frm.ShowDialog();
            }
        }

        public List<InfoCustom> LstInfo
        {
            get { return _lstInfo; }
        }

        #endregion
    }
}
