using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;

namespace TrungSoBG
{
    public class TrungSoBG:ICData
    {
        #region ICData Members
        private InfoCustomData _info=new InfoCustomData (IDataType.MasterDetailDt);
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();
        Database dbCDT = Database.NewStructDatabase();
        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
           
        }

        public void ExecuteBefore()
        {
           // CreateCT();
        }
        private void CreateCT()
        {
            if (_data.CurMasterIndex < 0)
                return;
            if (_data.DsData == null)
                return;
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster == null)
                return;
            if (drMaster.RowState == DataRowState.Modified || drMaster.RowState == DataRowState.Deleted)
                return;
            if (!drMaster.Table.Columns.Contains("SoCT"))
                return;


            string l_SoCT = drMaster["SoCT"].ToString();
            //  string l_SoCT = "12.08.275";
            string sql = @" SELECT	top 1 SoCT 
                            FROM	MT63
                            WHERE	SoCT='" + l_SoCT + "'" +
                            " ORDER BY SoCT DESC";


          
            DataTable dt = _data.DbData.GetDataTable(sql);
            String SoCT = "";
                //!string.IsNullOrEmpty(l_SoCT) ? l_SoCT : "001";

            if (dt.Rows.Count > 0)
            {
                int i = l_SoCT.Length - 1;
                for (; i > 0; i--)
                {
                    if (!Char.IsNumber(l_SoCT, i))
                    {
                        i++;
                        break;
                    }
                }
                SoCT = dt.Rows[0]["SoCT"].ToString().Substring(i);
                SoCT = (int.Parse(SoCT) + 1).ToString();
                SoCT = dt.Rows[0]["SoCT"].ToString().Substring(0, i) + SoCT; ;

            }
            else
            {
                return;
            }

            if (SoCT != "")
                drMaster["SoCT"] = SoCT;

        }
        public InfoCustomData Info
        {
              get { return _info; }
        }

        #endregion
    }
}
