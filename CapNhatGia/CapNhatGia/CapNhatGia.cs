using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using DevExpress.XtraEditors;
using Plugins;
using CDTDatabase;
using CDTLib;
using System.Data;
using System.Windows.Forms;

namespace CapNhatGia
{
    public class CapNhatGia:ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();

        #region ICData Members

        public CapNhatGia()
        {
            _info = new InfoCustomData(IDataType.Single);
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
            CapNhatGiaBan();
        }

        void CapNhatGiaBan()
        {
            if (_data.CurMasterIndex < 0)
                return;
            if (_data.DsData == null)
                return;
            string sql = "";
            DataRow drCurrent = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drCurrent == null)
                return;
            if (drCurrent.RowState == DataRowState.Modified)
            {
                sql = "select * from dmvt";
                DataTable dtVT = db.GetDataTable(sql);
                DataView dvVT = new DataView(dtVT);

                if (drCurrent["IsCapNhat", DataRowVersion.Original].ToString().ToUpper() == "FALSE"
                    && drCurrent["IsCapNhat", DataRowVersion.Current].ToString().ToUpper() == "TRUE")
                {                                        
                    dvVT.RowFilter = "MaVT = '" + drCurrent["MaVT"].ToString() + "'";
                    if (dvVT.Count > 0)
                    {
                        decimal giaCD = drCurrent["GiaBanCD"].ToString().Trim() != "" ? decimal.Parse(drCurrent["GiaBanCD"].ToString()) : 0;
                        // GiaOME : @GiaBan*(100-@SLTieuThu)/100,0
                        decimal SLTT = dvVT[0]["SLTieuThu"].ToString().Trim() != "" ? decimal.Parse(dvVT[0]["SLTieuThu"].ToString()) : 0;                        
                        decimal GiaOME = Math.Round( giaCD * (100 -SLTT) / 100, 0);
                        // GiaDTD : @GiaBan*(100-@Giam)/100,0 
                        decimal Giam = dvVT[0]["Giam"].ToString().Trim() != "" ? decimal.Parse(dvVT[0]["Giam"].ToString()) : 0;
                        decimal giaDTB = Math.Round(giaCD * (100 - Giam) / 100, 0);

                        sql = @"update DMVT set GiaBan = '" + drCurrent["GiaBanCD"].ToString() + @"', GiaOME = '" + GiaOME.ToString().Replace(",", ".") + @"', 
                                GiaDTB = '" + giaDTB.ToString().Replace("'", ".") + @"' where MaVT = '" + drCurrent["MaVT"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                    }
                }
                //bổ sung thêm trường hợp trước đó đã check vào cập nhật, bây giờ chỉ chỉnh lại giá bán mới
                if (drCurrent["IsCapNhat", DataRowVersion.Current].ToString().ToUpper() == "TRUE"
                    && drCurrent["GiaBanCD", DataRowVersion.Original].ToString() != drCurrent["GiaBanCD", DataRowVersion.Current].ToString())
                {
                    dvVT.RowFilter = "MaVT = '" + drCurrent["MaVT"].ToString() + "'";
                    if (dvVT.Count > 0)
                    {
                        decimal giaCD = drCurrent["GiaBanCD"].ToString().Trim() != "" ? decimal.Parse(drCurrent["GiaBanCD"].ToString()) : 0;
                        // GiaOME : @GiaBan*(100-@SLTieuThu)/100,0
                        decimal SLTT = dvVT[0]["SLTieuThu"].ToString().Trim() != "" ? decimal.Parse(dvVT[0]["SLTieuThu"].ToString()) : 0;
                        decimal GiaOME = Math.Round(giaCD * (100 - SLTT) / 100, 0);
                        // GiaDTD : @GiaBan*(100-@Giam)/100,0 
                        decimal Giam = dvVT[0]["Giam"].ToString().Trim() != "" ? decimal.Parse(dvVT[0]["Giam"].ToString()) : 0;
                        decimal giaDTB = Math.Round(giaCD * (100 - Giam) / 100, 0);

                        sql = @"update DMVT set GiaBan = '" + drCurrent["GiaBanCD"].ToString() + @"', GiaOME = '" + GiaOME.ToString().Replace(",", ".") + @"', 
                                GiaDTB = '" + giaDTB.ToString().Replace("'", ".") + @"' where MaVT = '" + drCurrent["MaVT"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                    }
                }
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
