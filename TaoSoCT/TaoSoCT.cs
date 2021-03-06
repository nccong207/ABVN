using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;

namespace TaoSoCT
{
    public class TaoSoCT:ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();
        Database dbCDT = Database.NewStructDatabase();

        #region ICData Members
  
        public TaoSoCT()
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

        private bool KTSuaNgay(DataRow drMaster, string ngayct)
        {
            DateTime dt1 = DateTime.Parse(drMaster[ngayct, DataRowVersion.Current].ToString());
            DateTime dt2 = DateTime.Parse(drMaster[ngayct, DataRowVersion.Original].ToString());
            return (dt1.Month != dt2.Month || dt1.Year != dt2.Year);
        }

        void CreateDN()
        {
            if (_data.CurMasterIndex < 0)
                return;
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Added
                || (drMaster.RowState == DataRowState.Modified && KTSuaNgay(drMaster, "NgayGH")))
            {
                if (_data.DrTable["MaCT"].ToString() == "")
                    return;

                string sql = "", soctNew = "", mact = "", prefix = "", Nam = "";
                mact = _data.DrTable["MaCT"].ToString();
                DateTime NgayCT = (DateTime)drMaster["NgayGH"];
                // Năm: 2 số cuối của năm
                Nam = NgayCT.Year.ToString();

                Nam = Nam.Substring(2, 2);

                prefix = mact + Nam + "-";
                // Số PT HIK đề xuất
                // 3. Số phiếu thu tự nhảy.
                // PT/13/02/001
                // 13: 2 số cuối của năm
                // 02: Tháng
                // 001: STT tự tăng gồm 3 ký tự
                // Qua mỗi tháng số phiếu thu nhảy lại 001

                sql = string.Format(@" SELECT   Top 1 SoPhieu  
                                       FROM     {0}
                                       WHERE    SoPhieu LIKE '{1}%' AND  isnumeric(replace(SoPhieu,'{1}','')) = 1
                                       ORDER BY SoPhieu DESC", _data.DrTableMaster["TableName"].ToString(), prefix);
                DataTable dt = db.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                    soctNew = GetNewValue(dt.Rows[0]["SoPhieu"].ToString());
                else
                    soctNew = prefix + "0001";
                if (soctNew != "")
                    drMaster["SoPhieu"] = soctNew;
            }
        }

        void CreateCT()
        {
            if (_data.CurMasterIndex < 0)
                return;
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (!drMaster.Table.Columns.Contains("SoCT") || !drMaster.Table.Columns.Contains("NgayCT") || !drMaster.Table.Columns.Contains("MaCT"))
                return;
            if (drMaster.RowState == DataRowState.Added
                || (drMaster.RowState == DataRowState.Modified && KTSuaNgay(drMaster, "NgayCT")))
            {
                if (_data.DrTable["MaCT"].ToString() == "")
                    return;

                string sql = "", soctNew = "", mact = "", prefix = "", Thang = "", Nam = "";
                mact = _data.DrTable["MaCT"].ToString();
                DateTime NgayCT = (DateTime)drMaster["NgayCT"];
                // Tháng: 2 chữ số
                // Năm: 2 số cuối của năm
                Thang = NgayCT.Month.ToString();
                Nam = NgayCT.Year.ToString();

                if (Thang.Length == 1)
                    Thang = "0" + Thang;
                Nam = Nam.Substring(2, 2);

                prefix = mact + "/" + Nam + "/" + Thang + "/";
                // Số PT HIK đề xuất
                // 3. Số phiếu thu tự nhảy.
                // PT/13/02/001
                // 13: 2 số cuối của năm
                // 02: Tháng
                // 001: STT tự tăng gồm 3 ký tự
                // Qua mỗi tháng số phiếu thu nhảy lại 001

                sql = string.Format(@" SELECT   Top 1 SoCT  
                                       FROM     {0}
                                       WHERE    SoCT LIKE '{1}%' AND  isnumeric(replace(SoCT,'{1}','')) = 1
                                       ORDER BY SoCT DESC", _data.DrTableMaster["TableName"].ToString(), prefix);
                DataTable dt = db.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                    soctNew = GetNewValue(dt.Rows[0]["SoCT"].ToString());
                else
                    soctNew = prefix + "001";
                if (soctNew != "")
                    drMaster["SoCT"] = soctNew;
            }
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

        public void ExecuteBefore()
        {
            if (_data.DrTableMaster["TableName"].ToString() == "MTGH")
                CreateDN();
            List<string> lstTable = new List<string>(new string[] {"MT11","MT12","MT15","MT16",
                "MT21","MT22","MT23","MT24","MT25","MT31","MT32","MT33","MT41","MT42","MT43","MT44","MT45","MT46","MTGiaCong","MT51"});
            if (!lstTable.Contains(_data.DrTableMaster["TableName"].ToString()))
                return;
            CreateCT();
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
