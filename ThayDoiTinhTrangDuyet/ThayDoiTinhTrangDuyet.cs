using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using DevExpress.XtraEditors;
using CDTLib;
using System.Windows.Forms;

namespace ThayDoiTinhTrangDuyet
{
    public class ThayDoiTinhTrangDuyet:ICData
    {
        #region ICData Members
        DataCustomData _data;
        InfoCustomData _info = new InfoCustomData(IDataType.MasterDetailDt);
        Database db = Database.NewDataDatabase();
        public DataCustomData Data
        {
            set { _data = value; }
        }

        private bool CheckData()
        {
            DataTable mt63 = _data.DsData.Tables[0];
            DataTable dt63 = _data.DsData.Tables[1];
            if (_data.CurMasterIndex < 0)
                return true;
            DataRow mr63 = mt63.Rows[_data.CurMasterIndex];
            if (mr63.RowState == DataRowState.Deleted)
                return true;
            DataView dv = new DataView(dt63);
            if (mr63.RowState == DataRowState.Added)
                dv.RowStateFilter = DataViewRowState.Added;
            else
                dv.RowFilter = "MT63ID = '" + mr63["MT63ID"].ToString() + "'";
            foreach (DataRowView drv in dv)
            {
                decimal op = decimal.Parse(drv["OP"].ToString());
                decimal pe = decimal.Parse(drv["PE"].ToString());
                decimal hs = decimal.Parse(drv["HS"].ToString());
                decimal ft = decimal.Parse(drv["FT"].ToString());
                decimal at = decimal.Parse(drv["AT"].ToString());
                decimal hp = decimal.Parse(drv["HP"].ToString());
                if (op + pe + hs + ft + at + hp == 0)
                {
                    XtraMessageBox.Show("Vui lòng nhập ít nhất một trong sáu loại công nối cho mặt hàng " + drv["KyHieu"].ToString(),
                        Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                    return false;
                }
            }
            if (decimal.Parse(mr63["HoaHong"].ToString()) > 0)
                if (mr63["NN1"].ToString().Trim() == "" && mr63["NN2"].ToString().Trim() == "" && mr63["NN3"].ToString().Trim() == "")
                {
                    XtraMessageBox.Show("Vui lòng nhập thông tin người nhận hoa hồng",
                        Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                    return false;
                }
            return true;
        }
        
        public void ExecuteAfter()
        {
            
        }

        public void ExecuteBefore()
        {
            _info.Result = CheckData();

            Database data = _data.DbData;
            string sql = "";
            DataTable dt = new DataTable();
            DataTable mt63 = _data.DsData.Tables[0];

            if (_data.CurMasterIndex < 0)
                return;
            DataRow mr63 = mt63.Rows[_data.CurMasterIndex];
            if (mr63.RowState == DataRowState.Added || mr63.RowState == DataRowState.Modified || mr63.RowState == DataRowState.Unchanged)
            {
                if (mr63["CheckDuyet"].ToString() == "Đã duyệt")
                {
                    XtraMessageBox.Show("Số liệu báo giá đã được duyệt, không thể sửa");
                    _info.Result = false;
                    return;
                }

                DataTable dt63 = _data.DsData.Tables[1];
                DataView dv63 = new DataView(dt63);
                if (mr63.RowState == DataRowState.Added)
                    dv63.RowStateFilter = DataViewRowState.Added;
                else
                    dv63.RowFilter = "MT63ID = '" + mr63["MT63ID"] + "'";
                if (dv63.Count > 0)
                {
                    decimal ttt = 0;
                    decimal ttg = 0;
                    foreach (DataRowView var in dv63)
                    {
                        if (var["TienTangGiam"].ToString() != "")
                            ttg += decimal.Parse(var["TienTangGiam"].ToString());
                        ttt += decimal.Parse(var["TongTT"].ToString());
                    }
                    decimal hh = Convert.ToDecimal(mr63["HoaHong"]);
                    decimal th = Convert.ToDecimal(mr63["TTienSGG"]);
                    if ((ttg > 0 && ttt > 0) || (hh > 0 && th > 0 && hh/th > 0.07M))
                        mr63["CheckDuyet"] = "Chờ duyệt";
                    else
                        mr63["CheckDuyet"] = "Không cần duyệt";
                }
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
