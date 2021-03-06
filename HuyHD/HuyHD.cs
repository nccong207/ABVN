using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using CDTDatabase;
using DevExpress.XtraEditors;
using CDTLib;

namespace HuyHD
{
    public class HuyHD : ICData
    {
        DataCustomData _data;
        InfoCustomData _info;
        public HuyHD()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }
        #region ICData Members

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            Database dbData = _data.DbData;
            //DataView dvMT32 = new DataView(_data.DsData.Tables[0]);
            DataView dvMT32 = new DataView(_data.DsDataCopy.Tables[0]);            
            DataRow drvMaster = dvMT32.Table.Rows[_data.CurMasterIndex];
            //DataRowView drvMaster = dvMT32_Copy[_data.CurMasterIndex];
            if (drvMaster.RowState == DataRowState.Deleted)
                return;
            if (drvMaster.RowState == DataRowState.Modified)
            {
                if (drvMaster["HuyHD", DataRowVersion.Current].ToString() == drvMaster["HuyHD", DataRowVersion.Original].ToString())
                    return;
                string sql = "";
                // Không hủy --> Hủy
                if ((Boolean.Parse(drvMaster["HuyHD", DataRowVersion.Current].ToString()) == true)
                && (Boolean.Parse(drvMaster["HuyHD", DataRowVersion.Original].ToString()) == false))
                {
                    sql = @" declare @s nvarchar(128)

                                    Select @s = replace(replace(ISNULL(SoPx,''),'{0}',''),',,',',') From MTGH Where SoPhieu = '{1}'

                                    Update MTGH Set SoPx = case 
					                                           when left(@s,1) = ',' then substring(@s,2,len(@s))
					                                           when right(@s,1) = ',' then substring(@s,1,len(@s) - 1) 
					                                           when left(@s,1) != ',' and right(@s,1) != ',' then @s
				                                           end
                                    Where SoPhieu = '{1}'";
                }
                // Hủy --> không hủy
                else
                {                    
                    sql = @"declare @s nvarchar(128)

                            Select @s = replace(replace(ISNULL(SoPx,''),'{0}',''),',,',',') From MTGH Where SoPhieu = '{1}'
                            Set @s = case 
                                       when left(@s,1) = ',' then substring(@s,2,len(@s))
                                       when right(@s,1) = ',' then substring(@s,1,len(@s) - 1) 
                                       when left(@s,1) != ',' and right(@s,1) != ',' then @s
                                     end
                            Set @s = case when @s = '' then '{0}' else @s + ',' + '{0}' end
                            Update MTGH Set SoPx = @s Where SoPhieu = '{1}'";
                }
                string sopgh = drvMaster["So_PGH"].ToString();
                if (sopgh != "")
                {
                    dbData.EndMultiTrans();
                    //bool a = _info.Result;
                    string SoHoaDon = drvMaster["SoHoaDon"].ToString();
                    _info.Result = dbData.UpdateByNonQuery(string.Format(sql, SoHoaDon, sopgh));
                     //a = _info.Result;
                }
            }
        }

        public void ExecuteBefore()
        {
            //DataRow drCur = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            //if (drCur.RowState == DataRowState.Deleted)
            //    return;
            //decimal TTien = (decimal)drCur["Ttien"];
            //DataView dv = new DataView(_data.DsData.Tables[1]);
            //dv.RowFilter = "MT32ID = '" + drCur["MT32ID"].ToString() + "'";
            //Database db = Database.NewDataDatabase();
//            DataTable dt = db.GetDataTable(string.Format(@" SELECT DISTINCT isnull(TTien,0)TTien,(SELECT COUNT(*) FROM DTGH WHERE MTGHID = m.MTGHID) Count 
//                                                            FROM MTGH m INNER JOIN DTGH d ON m.MTGHID = d.MTGHID WHERE SoPhieu =  '{0}'",drCur["So_PGH"].ToString()));
//            if (dt.Rows.Count > 0)
//            {
//                if (int.Parse(dt.Rows[0]["Count"].ToString()) == dv.Count)
//                {
//                    if (TTien != (decimal)dt.Rows[0]["TTien"])
//                    {
//                        XtraMessageBox.Show("Số tiền của phiếu giao hàng và hóa đơn không bằng nhau", Config.GetValue("PackageName").ToString());
//                        _info.Result = false;
//                        return;
//                    }
//                }
//            }


//            if (drCur.RowState != DataRowState.Modified)
//                return;
            //if ((Boolean.Parse(drCur["HuyHD", DataRowVersion.Current].ToString()) == true)
            //    && (Boolean.Parse(drCur["HuyHD", DataRowVersion.Original].ToString()) == false))
            //{
            //    string sopgh = drCur["So_PGH"].ToString();
            //    if (sopgh != "")
            //        _data.DbData.UpdateByNonQuery("update MTGH set SoPX = null where SoPhieu = '" + sopgh + "'");
            //}
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
