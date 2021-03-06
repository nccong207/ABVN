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
        string _tinhTrang;
        int _duyetBG;
        Database db = Database.NewDataDatabase();
        public XtraForm1(DataRow dr, string tinhTrang, int duyetBG)
        {
            InitializeComponent();
            _dr = dr;
            _tinhTrang = tinhTrang;
            _duyetBG = duyetBG;

            if (_tinhTrang == "Chờ duyệt")
                combotinhtrang.Properties.Items.Add("Duyệt cấp 1");
            if (_tinhTrang == "Duyệt cấp 1")
            {
                if (_duyetBG == 1)
                    combotinhtrang.Properties.Items.Add("Bỏ duyệt");
                else
                    combotinhtrang.Properties.Items.Add("Đã duyệt");
            }
            if (_tinhTrang == "Đã duyệt")
                combotinhtrang.Properties.Items.Add("Bỏ duyệt");

            combotinhtrang.SelectedIndex = 0;
            dayngayduyet.EditValue = DateTime.Now;
        }

        private void btdongy_Click(object sender, EventArgs e)
        {
            if (dayngayduyet.EditValue == null || combotinhtrang.EditValue == null)
            {
                XtraMessageBox.Show("Cần nhập thông tin phê duyệt");
                return;
            }
            string sql = "";
            var ttCu = _dr["CheckDuyet"].ToString();
            var ttMoi = combotinhtrang.EditValue.ToString();


            if (ttCu == "Chờ duyệt" || (ttCu == "Duyệt cấp 1" && ttMoi == "Đã duyệt"))
            {
                sql = "update MT63 set CheckDuyet =N'" + ttMoi + "', NgayDuyet='" + dayngayduyet.EditValue + "', Note=N'" + txtghichu.EditValue + "',NguoiDuyet='" + Config.GetValue("UserName") + "' where MT63ID='" + _dr["MT63ID"] + "'";
                if (db.UpdateByNonQuery(sql) == true)
                {
                    _dr["CheckDuyet"] = ttMoi;
                    _dr["NgayDuyet"] = dayngayduyet.EditValue;
                    _dr["Note"] = txtghichu.EditValue;
                    _dr["NguoiDuyet"] = Config.GetValue("UserName");
                    _dr.AcceptChanges();
                }
            }

            else if (ttCu == "Duyệt cấp 1" && ttMoi == "Bỏ duyệt")
            {
                sql = "update MT63 set CheckDuyet = N'Chờ duyệt', Note = null, NgayDuyet = null, NguoiDuyet = null where MT63ID='" + _dr["MT63ID"] + "'";
                if (db.UpdateByNonQuery(sql) == true)
                {
                    _dr["CheckDuyet"] = "Chờ duyệt";
                    _dr["NgayDuyet"] = DBNull.Value;
                    _dr["Note"] = DBNull.Value;
                    _dr["NguoiDuyet"] = DBNull.Value;
                    _dr.AcceptChanges();
                }
            }

            else if (ttCu == "Đã duyệt" && ttMoi == "Bỏ duyệt")
            {
                sql = "update MT63 set CheckDuyet = N'Duyệt cấp 1', Note = null, NgayDuyet = null, NguoiDuyet = null where MT63ID='" + _dr["MT63ID"] + "'";
                if (db.UpdateByNonQuery(sql) == true)
                {
                    _dr["CheckDuyet"] = "Duyệt cấp 1";
                    _dr["NgayDuyet"] = DBNull.Value;
                    _dr["Note"] = DBNull.Value;
                    _dr["NguoiDuyet"] = DBNull.Value;
                    _dr.AcceptChanges();
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