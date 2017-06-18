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

namespace DuyetBaoGiaDt
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        DataRow _dr;
        Database db = Database.NewDataDatabase();
        public XtraForm1(DataRow dr)
        {
            InitializeComponent();
            _dr = dr;
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
            string sql = "";
            if (_dr["CheckDuyet"].ToString() == "Chờ duyệt" && combotinhtrang.EditValue.ToString() == "Đã duyệt")
            {
                sql = "update MT63 set CheckDuyet =N'" + combotinhtrang.EditValue + "',NgayDuyet='" + dayngayduyet.EditValue + "',Note=N'" + txtghichu.EditValue + "',NguoiDuyet='" + Config.GetValue("UserName") + "' where MT63ID='" + _dr["MT63ID"] + "'";
                if (db.UpdateByNonQuery(sql) == true)
                    check = true;
            }
            if (_dr["CheckDuyet"].ToString() == "Đã duyệt" && combotinhtrang.EditValue.ToString() == "Hủy duyệt")
            {
                sql = "update MT63 set CheckDuyet =N'Chờ duyệt',GhiChu=null,NgayDuyet=null,NguoiDuyet=null where MT63ID='" + _dr["MT63ID"] + "'";
                if (db.UpdateByNonQuery(sql) == true)
                    check = true;
            }
            this.Close();
        }

        private void btthoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}