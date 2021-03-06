using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using CDTLib;
using DevExpress;
using DevExpress.XtraEditors;
using Plugins;
namespace TimKiemKMH
{
    public class  TimKiemKMH: IC
    {
        #region IC Members
        
        private List<InfoCustom> _lstInfo = new List<InfoCustom>();

        public TimKiemKMH()
        {
            InfoCustom ic = new InfoCustom(1175, "Tìm kiếm theo ký mã hiệu", "Quản lý kho");
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
