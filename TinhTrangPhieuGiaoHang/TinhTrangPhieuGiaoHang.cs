using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;

namespace TinhTrangPhieuGiaoHang
{
    public class TinhTrangPhieuGiaoHang:ICData
    {
        #region ICData Members
        DataCustomData _data;
        InfoCustomData _info = new InfoCustomData(IDataType.MasterDetailDt);
        Database db = Database.NewDataDatabase();
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
            DataTable dtPhieuGiaoHang = _data.DsData.Tables[0];
            if (_data.CurMasterIndex < 0)
                return;
            DataRow drPhieuGiaoHang = dtPhieuGiaoHang.Rows[_data.CurMasterIndex];
            if (drPhieuGiaoHang.RowState == DataRowState.Added)
            {
                if (!string.IsNullOrEmpty(drPhieuGiaoHang["SoSO"].ToString()))
                {
                    sql = "update MTSO set TinhTrang='4' where SoPhieuDN='" + drPhieuGiaoHang["SoSO"] + "'";
                    db.UpdateByNonQuery(sql);
                }

            }
            if (drPhieuGiaoHang.RowState == DataRowState.Deleted)
            {
                string soct = drPhieuGiaoHang["SoSO", DataRowVersion.Original].ToString();
                if (!string.IsNullOrEmpty(soct))
                {
                    sql = "update MTSO set TinhTrang='3' where SoPhieuDN='" + soct + "'";
                    db.UpdateByNonQuery(sql);
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
