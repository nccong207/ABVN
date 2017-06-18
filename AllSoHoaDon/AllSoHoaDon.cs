using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;

namespace AllSoHoaDon
{
    public class AllSoHoaDon:ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();
        Database dbCDT = Database.NewStructDatabase();
        #region ICData Members

        public AllSoHoaDon()
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
            CreateCT();
        }

        private void CreateCT()
        {
            string ma = "";
            if (_data.CurMasterIndex < 0)
                return;
            if (_data.DsData == null)
                return;
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster == null)
                return;
            if (drMaster.RowState != DataRowState.Added)
                return;
            if (drMaster.Table.Columns.Contains("SoCT"))
            {
                ma = "SoCT";
            }
            else if (drMaster.Table.Columns.Contains("SoPhieuDN"))
            {
                ma = "SoPhieuDN";
            }
            else
                return;

            string tbName = _data.DrTableMaster["TableName"].ToString();
            string sql ="";

            string l_SoDDH  = drMaster[ma].ToString();

            string SoDDH ="";
            // TH1: Số PT rỗng
            if (string.IsNullOrEmpty(l_SoDDH))
            {
                // Lấy Số PT cao nhất và tăng lên 1
                sql = @"SELECT	top 1 "+ma+ @"
                        FROM	" + tbName + @"
                        WHERE	PATINDEX('%[a-z]/-_.:+ %',REVERSE(SoCT)) = 0
                        ORDER BY "+ma+" DESC";

                DataTable dt = _data.DbData.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                    SoDDH = GetNewValue(dt.Rows[0][ma].ToString());
                else
                    SoDDH = "HT00001";
               
            }
            else
            {
                // TH2: Số PT <> rỗng
                // Trùng Số CT

                sql = @"SELECT	top 1 " + ma + @"
                        FROM	" + tbName + @"
                        WHERE   "+ma+" = '" + l_SoDDH + "'";
                DataTable dt = _data.DbData.GetDataTable(sql);

                // Số CT trùng
                if (dt.Rows.Count != 0)
                {
                    int i = l_SoDDH.Length - 1;
                    for (; i > 0; i--)
                    {
                        if (!Char.IsNumber(l_SoDDH, i))
                        {
                            i++;
                            break;
                        }
                    }
                    if (i > 0)
                    {
                        string sFormat = l_SoDDH.Substring(0, i);

                        sql = string.Format(@"SELECT top 1 " + ma + @"
                            FROM	" + tbName + @"
                            WHERE	" + ma + @" LIKE '{0}%' AND ISNUMERIC(substring(" + ma + @",len('{1}')+1,len(" + ma + @"))) = 1
                            ORDER BY CAST(substring(" + ma + @",len('{2}')+1,len(" + ma + @")) as int) DESC", sFormat, sFormat, sFormat);
                    }
                    else
                    {
                        // truong hop SoCT: 099 -> 0100
                        sql = @"SELECT	top 1 " + ma + @"
                                FROM	" + tbName + @"
                                WHERE	PATINDEX('%[a-z]/-_.:+ %',REVERSE(" + ma + @")) = 0
                                ORDER BY CONVERT(INT," + ma + ") DESC";
                    }

                    dt = _data.DbData.GetDataTable(sql);                    
                    if (dt.Rows.Count > 0)
                    {
                        SoDDH = GetNewValue(dt.Rows[0][ma].ToString());
                    }
                }
            }

            if (SoDDH != "")
                drMaster[ma] = SoDDH;
            
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
