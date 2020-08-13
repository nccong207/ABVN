using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using CDTLib;
using System.Data;
using DevExpress.XtraEditors;

namespace DeNghiGiaoHang
{
    public class DeNghiGiaoHang:ICData
    {
        #region ICData Members
        DataCustomData _data;
        InfoCustomData _info=new InfoCustomData (IDataType.MasterDetailDt);
        Database db = Database.NewDataDatabase();
        string soct = "";
        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
           
        }

        public void ExecuteBefore()
        {

            Database data = _data.DbData;
            string sql = "";
        
            DataTable dt = new DataTable();
            DataTable dtPhieuDenghi = _data.DsData.Tables[0];
            if (_data.CurMasterIndex < 0)
                return;
            DataRow drPhieuDeNghi = dtPhieuDenghi.Rows[_data.CurMasterIndex];
             //soct=  drPhieuDeNghi["SoBG"].ToString();
            if (drPhieuDeNghi.RowState == DataRowState.Added)
            {
                var sobg = drPhieuDeNghi["SoBG"].ToString();

                if (!string.IsNullOrEmpty(sobg))
                {
                    var ngayHH = db.GetValue("select ThoiHan from MT63 where SoCT = '" + sobg + "'");
                    if (Convert.ToDateTime(ngayHH) < DateTime.Today)
                    {
                        XtraMessageBox.Show("Phiếu đề nghị không hợp lệ vì báo giá đã hết hạn!",
                            Config.GetValue("PackageName").ToString());
                        _info.Result = false;
                        return;
                    }

                    sql = "update MT63 set TinhTrangBG='1' where SoCT='" + sobg + "'";
                    db.UpdateByNonQuery(sql);
                }

            }
            if (drPhieuDeNghi.RowState == DataRowState.Deleted)
            {
                string soct = drPhieuDeNghi["SoBG", DataRowVersion.Original].ToString();
                if (!string.IsNullOrEmpty(soct))
                {
                    sql = "update MT63 set TinhTrangBG='3' where SoCT='" + soct + "'";
                    db.UpdateByNonQuery(sql);
                }

            }

            // TextEdit txtSoCT = _data.FrmMain.Controls.Find("SoCT", true)[0] as TextEdit;
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
