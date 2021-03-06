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
using DevExpress.XtraGrid.Views.Grid;

namespace DuyetBaoGia
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        DataTable _tb;
        string _tinhTrang;
        int _duyetBG;
        GridView _gv;
        Database db = Database.NewDataDatabase();
        public XtraForm1(DataTable tb, string tinhTrang, int duyetBG, GridView gv)
        {
            InitializeComponent();
            _tb = tb;
            _gv = gv;
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
            dayngayduyet.EditValue = DateTime.Today;
        }

        private void btdongy_Click(object sender, EventArgs e)
        {
            if (dayngayduyet.EditValue == null || combotinhtrang.EditValue == null)
            {
                XtraMessageBox.Show("Cần nhập thông tin phê duyệt");
                return;
            }
            foreach (DataRow row in _tb.Rows)
            {
                string sql = "";
                var ttCu = row["CheckDuyet"].ToString();
                var ttMoi = combotinhtrang.EditValue.ToString();
                var gridRow = _gv.GetDataRow(Convert.ToInt32(row["gridId"]));

                if (ttCu == "Chờ duyệt" || (ttCu == "Duyệt cấp 1" && ttMoi == "Đã duyệt"))
                {
                    sql = "update MT63 set CheckDuyet =N'" + ttMoi + "', NgayDuyet='" + dayngayduyet.EditValue + "', Note=N'" + txtghichu.EditValue + "',NguoiDuyet='" + Config.GetValue("UserName") + "' where MT63ID='" + row["id"] + "'";
                    if (db.UpdateByNonQuery(sql) == true)
                    {
                        gridRow["CheckDuyet"] = ttMoi;
                        gridRow["NgayDuyet"] = dayngayduyet.EditValue;
                        gridRow["Note"] = txtghichu.EditValue;
                        gridRow["NguoiDuyet"] = Config.GetValue("UserName");
                    }
                }

                else if (ttCu == "Duyệt cấp 1" && ttMoi == "Bỏ duyệt")
                {
                    sql = "update MT63 set CheckDuyet = N'Chờ duyệt', Note = null, NgayDuyet = null, NguoiDuyet = null where MT63ID='" + row["id"] + "'";
                    if (db.UpdateByNonQuery(sql) == true)
                    {
                        gridRow["CheckDuyet"] = "Chờ duyệt";
                        gridRow["NgayDuyet"] = DBNull.Value;
                        gridRow["Note"] = DBNull.Value;
                        gridRow["NguoiDuyet"] = DBNull.Value;
                    }
                }

                else if (ttCu == "Đã duyệt" && ttMoi == "Bỏ duyệt")
                {
                    sql = "update MT63 set CheckDuyet = N'Duyệt cấp 1', Note = null, NgayDuyet = null, NguoiDuyet = null where MT63ID='" + row["id"] + "'";
                    if (db.UpdateByNonQuery(sql) == true)
                    {
                        gridRow["CheckDuyet"] = "Duyệt cấp 1";
                        gridRow["NgayDuyet"] = DBNull.Value;
                        gridRow["Note"] = DBNull.Value;
                        gridRow["NguoiDuyet"] = DBNull.Value;
                    }
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