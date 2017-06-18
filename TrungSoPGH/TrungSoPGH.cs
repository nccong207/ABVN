using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;

namespace TrungSoPGH
{
    public class TrungSoPGH:ICData
    {
        #region ICData Members
        private InfoCustomData _info = new InfoCustomData(IDataType.MasterDetailDt);
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
            //CreateCT();
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
            if (!drMaster.Table.Columns.Contains("SoPhieuDN"))
                return;


            string l_SoSoPhieuDN = drMaster["SoPhieuDN"].ToString();
            //string l_SoSoPhieuDN = "SO12 - 1362";
            string sql = @" SELECT	top 1 SoPhieuDN 
                            FROM	MTSO
                            WHERE	SoPhieuDN='" + l_SoSoPhieuDN + "'" +
                            " ORDER BY SoPhieuDN DESC";



            DataTable dt = _data.DbData.GetDataTable(sql);
            String SoPhieuDN = "";
                //!string.IsNullOrEmpty(l_SoSoPhieuDN) ? l_SoSoPhieuDN : "001";

            if (dt.Rows.Count > 0)
            {
                int i = l_SoSoPhieuDN.Length - 1;
                for (; i > 0; i--)
                {
                    if (!Char.IsNumber(l_SoSoPhieuDN, i))
                    {
                        i++;
                        break;
                    }
                }
                SoPhieuDN = dt.Rows[0]["SoPhieuDN"].ToString().Substring(i);
                SoPhieuDN = (int.Parse(SoPhieuDN) + 1).ToString();
                SoPhieuDN = dt.Rows[0]["SoPhieuDN"].ToString().Substring(0, i) + SoPhieuDN; ;

            }
            else
            {
                return;
            }

            if (SoPhieuDN != "")
                drMaster["SoPhieuDN"] = SoPhieuDN;

        }
        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
