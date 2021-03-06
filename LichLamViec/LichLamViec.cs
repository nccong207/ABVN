using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;

namespace LichLamViec
{
    public class LichLamViec : IC
    {
        private List<InfoCustom> _lstInfo = new List<InfoCustom>();
        #region IC Members

        public void Execute(DataRow drMenu)
        {
            int menuID = Int32.Parse(drMenu["MenuPluginID"].ToString());
            if (_lstInfo[0].CType == ICType.Custom && _lstInfo[0].MenuID == menuID)
            {
                FrmQLLich frm = new FrmQLLich();
                frm.Text = drMenu["MenuName"].ToString();
                if (System.Windows.Forms.Application.OpenForms.Count > 1
                    && System.Windows.Forms.Application.OpenForms[1].IsMdiContainer)
                {
                    frm.MdiParent = System.Windows.Forms.Application.OpenForms[1];
                    frm.Show();
                }
                else
                    frm.ShowDialog();
            }
        }

        public List<InfoCustom> LstInfo
        {
            get { return _lstInfo; }
        }

        public LichLamViec()
        {
            InfoCustom ic = new InfoCustom(1001, "Lịch làm việc", "Quản lý kinh doanh");
            _lstInfo.Add(ic);
        }

        #endregion
    }
}
