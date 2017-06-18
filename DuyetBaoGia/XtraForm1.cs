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

namespace DuyetBaoGia
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        DataTable _tb;
        Database db = Database.NewDataDatabase();
        public XtraForm1(DataTable tb)
        {
            InitializeComponent();
            _tb = tb;
            combotinhtrang.SelectedIndex = 0;
            dayngayduyet.EditValue = DateTime.Now;
        }
        public bool check = false;
        private void btdongy_Click(object sender, EventArgs e)
        {
            if (dayngayduyet.EditValue == null || combotinhtrang.EditValue == null)
            {
                XtraMessageBox.Show("Nhập thông tin duyệt");
                return;
            }
            foreach (DataRow var in _tb.Rows)
            {

                string sql = "";
                if (var["CheckDuyet"].ToString() == "Chờ duyệt" && combotinhtrang.EditValue.ToString() == "Đã duyệt")
                {
                    //,NguoiDuyet='"+Config.GetValue("UserName")+"'
                    sql = "update MT63 set CheckDuyet =N'" + combotinhtrang.EditValue + "', NgayDuyet='" + dayngayduyet.EditValue + "', Note=N'" + txtghichu.EditValue + "',NguoiDuyet='" + Config.GetValue("UserName") + "' where MT63ID='" + var["id"] + "'";
                    if (db.UpdateByNonQuery(sql) == true)
                        check = true;
                }
                if (var["CheckDuyet"].ToString() == "Đã duyệt" && combotinhtrang.EditValue.ToString() == "Hủy duyệt")
                {
                    //,NguoiDuyet='"+Config.GetValue("UserName")+"'
                    sql = "update MT63 set CheckDuyet = N'Chờ duyệt', Note = null, NgayDuyet = null, NguoiDuyet = null where MT63ID='" + var["id"] + "'";
                    if (db.UpdateByNonQuery(sql) == true)
                        check = true;
                }
            }
            this.Close();
        }

        private void btthoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}