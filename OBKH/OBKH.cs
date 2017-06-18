using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Plugins;

namespace OBKH
{
    public class OBKH:ICData
    {
        #region ICData Members
        private DataCustomData _data;
        private InfoCustomData _info;
        public OBKH()
        {
            _info = new InfoCustomData(IDataType.Single);
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            string tableName = _data.DrTable["TableName"].ToString();
            if (tableName != "OBKH")
                return;
            DataView dv = new DataView(_data.DsDataCopy.Tables[0]);
            dv.RowStateFilter = DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent;
            foreach (DataRowView drv in dv)
            {
                string tk = drv["Tk"].ToString();
                string s = "delete from obtk where tk = '" + tk + "' and ngayct = '" + drv["NgayCT"].ToString() + "'";
                _data.DbData.UpdateByNonQuery(s);
                s = "insert into obtk(ngayct, tk, duno, duco, dunont, ducont) " +
                    " select ngayct, tk, sum(duno), sum(duco), sum(dunont), sum(ducont) " +
                    " from obkh group by ngayct, tk having tk = '" + tk + "' and ngayct = '" + drv["NgayCT"].ToString() + "'";
                _data.DbData.UpdateByNonQuery(s);
            }
        }

        public void ExecuteBefore()
        {
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
