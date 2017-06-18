using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using CDTLib;

namespace HuyBaoGiaDt
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        DataRow _dr;
        Database db = Database.NewDataDatabase();
        public XtraForm1(DataRow dr)
        {
            InitializeComponent();
            dateEdit1.EditValue = DateTime.Now;
            _dr = dr;
        }
        public bool check = false;
        private void btdongy_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("Khi hủy báo giá thì không thể phục hồi", Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string sql = "";
                if (_dr["MT63ID"].ToString() != "")
                {
                    sql = "update MT63 set TinhTrangBG='2',NgayHuy='" + dateEdit1.EditValue + "' where MT63ID='" + _dr["MT63ID"] + "'";
                    if (db.UpdateByNonQuery(sql) == true)
                        check = true;

                }
                if (_dr["SPDeNghi"].ToString() != "")
                {
                    sql = "update MTSO set TinhTrang='2',NgayHuy='" + dateEdit1.EditValue + "' where SoPhieuDN = '" + _dr["SPDeNghi"] + "'";
                    db.UpdateByNonQuery(sql);
                }
                this.Close();
            }
        }

        private void btthoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}