using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;

namespace TrungSoDDH
{
    public class TrungSoDDH:ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();
        Database dbCDT = Database.NewStructDatabase();
        #region ICData Members

        public TrungSoDDH()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }
        
        public InfoCustomData Info
        {
            get { return _info; }
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
                            WHERE	SoCT='"+l_SoCT+"'"+
                            " ORDER BY SoCT DESC";

          
//            if (i > 0)
//            {
//                string sFormat = l_SoCT.Substring(0, i);

//                sql = string.Format(@"SELECT top 1 SoCT
//                            FROM	MT63
//                            WHERE	SoCT LIKE '{0}%'
//                            ORDER BY CAST(substring(SoCT,3,len(SoCT)) as INT) DESC", sFormat);
//            }
//            else
//            {
//                 sql = @"SELECT	top 1 SoCT
//                        FROM	MT63
//                        WHERE	PATINDEX('%[a-z]%',REVERSE(SoCT)) = 0
//                        ORDER BY CONVERT(INT,SoCT) DESC";
//            }

            DataTable dt = _data.DbData.GetDataTable(sql);
            String SoCT = !string.IsNullOrEmpty(l_SoCT) ? l_SoCT : "001";

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

        private string GetNewValue(string OldValue)
        {
            try
            {
                int i = OldValue.Length - 1;
                for (; i > 0; i--)
                    if (!Char.IsNumber(OldValue, i))
                        break;
                if (i == OldValue.Length - 1)
                {
                    int NewValue = Int32.Parse(OldValue) + 1;
                    return NewValue.ToString();
                }
                string PreValue = OldValue.Substring(0, i + 1);
                string SufValue = OldValue.Substring(i + 1);
                int intNewSuff = Int32.Parse(SufValue) + 1;
                string NewSuff = intNewSuff.ToString().PadLeft(SufValue.Length, '0');
                return (PreValue + NewSuff);
            }
            catch
            {
                return string.Empty;
            }
        }
        
        #endregion
    }
}
