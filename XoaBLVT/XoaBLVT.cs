using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;

namespace XoaBLVT
{
    public class XoaBLVT:ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();    
    
        #region ICData Members
 
        public XoaBLVT()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            Delete(); 
        }

        void Delete()
        {
            if (_data.CurMasterIndex < 0)
                return;
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Added)
                return;            
            string id = "", sql = "";
            if (drMaster.RowState == DataRowState.Deleted)
                id = drMaster[_data.DrTableMaster["Pk"].ToString(), DataRowVersion.Original].ToString();
            DataView dv23 = new DataView(_data.DsData.Tables[1]);                        
            dv23.RowFilter = " MT23ID = '" + id + "'";
            dv23.RowStateFilter = DataViewRowState.Deleted;

            _data.DbData.EndMultiTrans();
            foreach (DataRowView drv in dv23)
            {
                if (drv.Row.RowState == DataRowState.Deleted)
                {
                    sql = "delete from blvt where MTID = '" + id + "'";
                    db.UpdateByNonQuery(sql);
                }
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
