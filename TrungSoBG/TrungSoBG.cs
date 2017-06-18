using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using CDTLib;

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
           CreateCT();
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

            string namlamviec = DateTime.Now.ToString("yy");
            string thang = DateTime.Now.ToString("MM");
            

            string l_SoCT = drMaster["SoCT"].ToString();
            //  string l_SoCT = "12.08.275";
            //string sql = @" SELECT	top 1 SoCT FROM	MT63 WHERE	SoCT='" + l_SoCT + "' ORDER BY SoCT DESC";

            string sql = string.Format("SELECT max(SoCT) as SoCT FROM MT63 WHERE SoCT LIKE '{0}.{1}%'", namlamviec, thang);
          
            DataTable dt = _data.DbData.GetDataTable(sql);
            String SoCT = "";
                //!string.IsNullOrEmpty(l_SoCT) ? l_SoCT : "001";

            if (dt.Rows.Count > 0)
            {
                string currentSCT = dt.Rows[0]["SoCT"].ToString();
                if (!string.IsNullOrEmpty(currentSCT))
                {
                    string[] sct = currentSCT.Split('.');
                    if (sct.Length >= 3 && !string.IsNullOrEmpty(sct[2]))
                    {
                        SoCT = namlamviec + "." + thang + "." + (int.Parse(sct[2]) + 1).ToString("D3");
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    SoCT = namlamviec + "." + thang + ".001";
                }

            }
            else
            {
               return;
            }

            if (!string.IsNullOrEmpty(SoCT))
                drMaster["SoCT"] = SoCT;

        }
        public InfoCustomData Info
        {
              get { return _info; }
        }

        #endregion
    }
}
