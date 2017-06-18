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

namespace HuyBaoGia
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        DataTable _tb;
        Database db = Database.NewDataDatabase();
        public XtraForm1(DataTable tb)
        {
            InitializeComponent();
            dateEdit1.EditValue = DateTime.Now;
            _tb = tb;
        }
        public bool check = false;
        private void btdongy_Click(object sender, EventArgs e)
        {
            if (txtghichu.EditValue != null)
            {


                if (XtraMessageBox.Show("Khi hủy báo giá thì không thể phục hồi", Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (_tb.Rows.Count > 0)
                    {
                        foreach (DataRow row in _tb.Rows)
                        {
                            string sql = "";
                            if (row["id63"].ToString() != "")
                            {
                                sql = "update MT63 set TinhTrangBG='2',NgayHuy='" + dateEdit1.EditValue + "',GhiChu='" + txtghichu.EditValue + "' where MT63ID='" + row["id63"] + "'";
                                if (db.UpdateByNonQuery(sql) == true)
                                    check = true;

                            }
                            if (row["idso"].ToString() != "")
                            {
                                sql = "update MTSO set TinhTrang='2',NgayHuy='" + dateEdit1.EditValue + "',GhiChu='" + txtghichu.EditValue + "' where DNID='" + row["idso"] + "'";
                                db.UpdateByNonQuery(sql);
                            }
                        }

                    }
                    this.Close();
                }
            }
            else XtraMessageBox.Show("Bắt buộc nhập lý do hủy", Config.GetValue("PackageName").ToString());
        }

        private void btthoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}