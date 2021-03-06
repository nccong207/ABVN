using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using CDTDatabase;
using DevExpress.XtraEditors;
using CDTLib;

namespace CheckMaKH
{
    public class CheckMaKH : ICData
    {
        Database db = Database.NewDataDatabase();
        private DataCustomData _data;
        private InfoCustomData _info = new InfoCustomData(IDataType.MasterDetailDt);
        #region ICData Members

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            
        }

        public void ExecuteBefore()
        {
            DataRow drCur = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drCur.RowState == DataRowState.Deleted)
                return;
            DataView dv = new DataView(_data.DsData.Tables[1]);
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent | DataViewRowState.Added;
            bool rs = true;
            foreach (DataRowView drv in dv)
            {
                if (drv["MaKHCt"].ToString() == "")
                {
                    string tk = drv["TkNo"].ToString();
                    if (tk != "")
                    {
                        object o = db.GetValue("select TkCongNo from DMTK where TK = '" + tk + "'");
                        if (o != null && o.ToString() != "" && Boolean.Parse(o.ToString()))
                        {
                            XtraMessageBox.Show("Vui lòng chọn đối tượng nợ cho tài khoản " + tk,
                                Config.GetValue("PackageName").ToString());
                            rs = false;
                            break;
                        }
                    }
                }
                if (drv["MaKHCo"].ToString() == "")
                {
                    string tk = drv["TkCo"].ToString();
                    if (tk != "")
                    {
                        object o = db.GetValue("select TkCongNo from DMTK where TK = '" + tk + "'");
                        if (o != null && o.ToString() != "" && Boolean.Parse(o.ToString()))
                        {
                            XtraMessageBox.Show("Vui lòng chọn đối tượng có cho tài khoản " + tk,
                                Config.GetValue("PackageName").ToString());
                            rs = false;
                            break;
                        }
                    }
                }
            }
            _info.Result = rs;
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
