using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Windows.Forms;
using System.Data;
using CDTDatabase;
using CDTLib;

namespace ChiPhi
{
    public class ChiPhi : ICData
    {
        #region ICData Members
        private DataCustomData _data;
        private InfoCustomData _info;
        public ChiPhi()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
        }

        public void ExecuteBefore()
        {
            string tableName = _data.DrTableMaster["TableName"].ToString();
            _dbData = _data.DbData;
            if (tableName == "MT22" || tableName == "MT23")
            {
                DataView dvDt = new DataView(_data.DsData.Tables[1]);
                dvDt.RowStateFilter = DataViewRowState.Deleted;
                if (dvDt.Count > 0)
                {
                    string pkDt = _data.DrTable["Pk"].ToString();
                    foreach (DataRowView drv in dvDt)
                    {
                        string dtid = drv[pkDt].ToString();
                        string sql = "select * from DT25 where dt22ID = '" + dtid + "'";
                        DataTable dt = _dbData.GetDataTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            _info.Result = false;
                            string msg = "Không thể xóa vì chứng từ này đã phân bổ chi phí mua hàng!" +
                                "\nNếu thực sự muốn xóa, vui lòng tìm và xóa tất cả chứng từ chi phí mua hàng liên quan!";
                            if (Config.GetValue("Language").ToString() == "1")
                                msg = UIDictionary.Translate(msg);
                            MessageBox.Show(msg);
                            return;
                        }
                    }
                }
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion

        Database _dbData;

    }
}

