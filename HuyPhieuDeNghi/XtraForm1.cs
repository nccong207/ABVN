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

namespace HuyPhieuDeNghi
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        DataTable _tb;
        Database db = Database.NewDataDatabase();
        public XtraForm1(DataTable tb)
        {
            InitializeComponent();
            dateEdit1.EditValue = DateTime.Today;
            _tb = tb;
        }

        private void btdongy_Click(object sender, EventArgs e)
        {
            if (txtghichu.EditValue != null)
            {
                if (XtraMessageBox.Show("Khi hủy phiếu thì không thể phục hồi", Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (_tb.Rows.Count > 0)
                    {
                        foreach (DataRow row in _tb.Rows)
                        {
                            string sql = "";
                            if (row["sobg"].ToString() != "")
                            {
                                sql = "update MT63 set SPDeNghi = null, TinhTrangBG = 3 where SoCT = '" + row["sobg"].ToString() + "'";
                                if (!db.UpdateByNonQuery(sql))
                                    return;
                            }
                            if (row["idso"].ToString() != "")
                            {
                                sql = "update MTSO set TinhTrang='2',NgayHuy='" + dateEdit1.EditValue + "',Note='" + txtghichu.EditValue + "' where DNID='" + row["idso"] + "'";
                                if (db.UpdateByNonQuery(sql) == true)
                                    check = true;
                            }
                        }

                    }
                    this.Close();
                }
            }
            else XtraMessageBox.Show("Bắt buộc nhập lý do hủy", Config.GetValue("PackageName").ToString());
        }
        public bool check = false;
        private void btthoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}