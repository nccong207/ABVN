using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Plugins;
using CDTDatabase;

namespace GiaVon
{
    public class GiaVon : ICData
    {
        public GiaVon()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }
        private InfoCustomData _info;
        private DataCustomData _data;
        #region ICData Members

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            
        }

        public void ExecuteBefore()
        {
            string table = _data.DrTableMaster["TableName"].ToString();
            if (table == "MT24" || table == "MT32" || table == "MT33" || table == "MT41"
                || table == "MT42" || table == "MT43" || table == "MT44" || table == "MT45" || table == "MT36" || table == "MTGiaCong")
            {
                ApGiaPhieuXuat();
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion

        private void ApGiaPhieuXuat()
        {
            string table = _data.DrTableMaster["TableName"].ToString();
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            if ((table == "MT24" || table == "MT33" || table == "MT41" || table == "MT42") && !Boolean.Parse(drMaster["NhapTB"].ToString()))
                return;
                
            Database dbData = _data.DbData;
            string pk = _data.DrTableMaster["PK"].ToString();
            string pkDT = _data.DrTable["PK"].ToString();
            string mtID = drMaster[pk].ToString();
            DateTime denNgay = DateTime.Parse(drMaster["NgayCT"].ToString());
            DateTime tuNgay = denNgay.AddDays(-denNgay.Day + 1);
            DataView dvDetail = _data.DsData.Tables[1].DefaultView;
            dvDetail.RowFilter = pk + " = '" + mtID + "'";
            foreach (DataRowView drv in dvDetail)
            {
                string maKho = (table == "MT44" || table == "MTGiaCong") ? drMaster["MaKho"].ToString() : drv["MaKho"].ToString();
                string maVT = drv["MaVT"].ToString();
                string slXuat = table == "MTGiaCong" ? drv["SLQuyDoi"].ToString().Replace(",", ".") : drv["SoLuong"].ToString().Replace(",", ".");
                object[] os;
                if ((table == "MT24" || table == "MT33" || table == "MT41" || table == "MT42") && Boolean.Parse(drMaster["NhapTB"].ToString()))
                {
                    if (drv.Row.RowState == DataRowState.Modified || drv.Row.RowState == DataRowState.Unchanged)
                        os = dbData.GetValueByStore("TinhGiaBQDD",
                            new string[] { "MTIDDT", "TuNgay", "DenNgay", "MaKho", "MaVT", "DonGia" },
                            new object[] { drv[pkDT], tuNgay, denNgay, maKho, maVT, null },
                            new SqlDbType[] { SqlDbType.UniqueIdentifier, SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Decimal },
                            new ParameterDirection[] { ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Output });
                    else
                        os = dbData.GetValueByStore("TinhGiaBQDD",
                            new string[] { "MTIDDT", "TuNgay", "DenNgay", "MaKho", "MaVT", "DonGia" },
                            new object[] { null, tuNgay, denNgay, maKho, maVT, null },
                            new SqlDbType[] { SqlDbType.UniqueIdentifier, SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Decimal },
                            new ParameterDirection[] { ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Output });
                }
                else
                {
                    if (drv.Row.RowState == DataRowState.Modified || drv.Row.RowState == DataRowState.Unchanged)
                        os = dbData.GetValueByStore("TinhGia",
                            new string[] { "MTIDDT", "TuNgay", "DenNgay", "MaKho", "MaVT", "SlXuat", "DonGia" },
                            new object[] { drv[pkDT], tuNgay, denNgay, maKho, maVT, slXuat, null },
                            new SqlDbType[] { SqlDbType.UniqueIdentifier, SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Decimal, SqlDbType.Decimal },
                            new ParameterDirection[] { ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Output });
                    else
                        os = dbData.GetValueByStore("TinhGia",
                            new string[] { "MTIDDT", "TuNgay", "DenNgay", "MaKho", "MaVT", "SlXuat", "DonGia" },
                            new object[] { null, tuNgay, denNgay, maKho, maVT, slXuat, null },
                            new SqlDbType[] { SqlDbType.UniqueIdentifier, SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Decimal, SqlDbType.Decimal },
                            new ParameterDirection[] { ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Input, ParameterDirection.Output });
                }
                if (os == null || os.Length == 0)
                    continue;
                string giaVon = os[0].ToString();
                if (giaVon == "-1")
                    continue;
                if (drv.Row.Table.Columns.Contains("DGQuyDoi"))
                    drv.Row["DGQuyDoi"] = giaVon;
                //else
                //    if (drv.Row.Table.Columns.Contains("Gia"))
                //        drv.Row["Gia"] = giaVon;
            }
            dvDetail.RowFilter = "";
            dvDetail.RowStateFilter = DataViewRowState.CurrentRows;
        }
    }
}
