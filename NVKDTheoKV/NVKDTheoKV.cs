using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using DevExpress.XtraEditors;
using CDTLib;


namespace NVKDTheoKV
{
    public class NVKDTheoKV:ICData
    {
        #region ICData Members

        DataCustomData _data;
        InfoCustomData _info = new InfoCustomData(IDataType.Single);
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
           
            // Database data = _data.DbData;
            //string sql = "";
            //DataTable dt = new DataTable();
            //DataTable mtkh = _data.DsData.Tables[0];

            //if (_data.CurMasterIndex < 0)
            //    return;
            //DataRow mrkh = mtkh.Rows[_data.CurMasterIndex];

            //if (mrkh.RowState == DataRowState.Added || mrkh.RowState == DataRowState.Modified)
            //{
            //    if (mrkh["KhuVuc"].ToString() != "")
            //    {
            //        sql = "select distinct MaNV from NVKDTheoKV where DMTP='" + mrkh["KhuVuc"] + "'";
            //        DataTable tbkv = data.GetDataTable(sql);
            //        if (tbkv.Rows.Count == 1)
            //        {
            //            mrkh["MaNV"] = tbkv.Rows[0]["MaNV"].ToString();
            //            //sql = "insert into NVKDTheoKV(MaNV,DMTP) values ('" + mrkh["ManV"].ToString() + "','" + mrkh["KhuVuc"].ToString() + "')";
            //            // db.UpdateByNonQuery(sql);
            //        }
            //        else if (tbkv.Rows.Count > 1)
            //        {

            //            XtraMessageBox.Show("Khu vực này thuộc hai nhân viên kinh doanh", Config.GetValue("PackageName").ToString());
            //            _info.Result = false;
            //            return;

            //        }
            //    }
            //}
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
