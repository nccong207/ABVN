using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using System.Windows.Forms;

namespace PhanBoCN
{
    public class PhanBoCN:IC
    {
        private List<InfoCustom> _lstInfo = new List<InfoCustom>();
        public PhanBoCN()
        {
            InfoCustom ic = new InfoCustom(7001, "Phân bổ công nợ theo hóa đơn", "Bán hàng phải thu");
            _lstInfo.Add(ic);
        }

        #region IC Members

        public void Execute(DataRow drMenu)
        {
            int menuID = Int32.Parse(drMenu["MenuPluginID"].ToString());
            if (_lstInfo[0].CType == ICType.Custom && _lstInfo[0].MenuID == menuID)
            {
                Form main = null;
                foreach (Form fr in Application.OpenForms)
                    if (fr.IsMdiContainer)
                        main = fr;
                FrmPhanBoCN frm = new FrmPhanBoCN(drMenu["ExtraSql"].ToString());
                frm.Text = drMenu["MenuName"].ToString();
                if (main == null)
                {
                    frm.WindowState = System.Windows.Forms.FormWindowState.Normal;
                    frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                    frm.ShowDialog();
                }
                else
                {
                    frm.MdiParent = main;
                    frm.Show();
                }
            }
        }

        public List<InfoCustom> LstInfo
        {
            get { return _lstInfo; }
        }

        #endregion
    }
}
