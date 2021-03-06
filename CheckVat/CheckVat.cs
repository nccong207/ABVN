using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace CheckVat
{
    public class CheckVat: ICData
    {
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
            string[] tmp = new string[] {"MT21", "MT22", "MT23", "MT24", "MT25", "MT31", "MT32", "MT33"};
            List<string> lstTable = new List<string>();
            lstTable.AddRange(tmp);
            if (!lstTable.Contains(table))
                return;
            if (_data.CurMasterIndex < 0)
                return;
            //kiem tra tong tien
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            decimal thueSuat = LayThueSuat(drMaster["MaThue"].ToString());
            decimal tthue = Math.Round(Decimal.Parse(drMaster["TThue"].ToString()),0);
            decimal ttienh = Math.Round(Decimal.Parse(drMaster["TTienH"].ToString()),0);
            if (drMaster.Table.Columns.Contains("TCK"))
                ttienh = ttienh - Math.Round(Decimal.Parse(drMaster["TCK"].ToString()), 0);
            if (table == "MT23")
                ttienh += Math.Round(Decimal.Parse(drMaster["ThueNK"].ToString()),0);
            if (tthue != Math.Round(ttienh * thueSuat/100, 0))
                _info.Result = XtraMessageBox.Show("Tổng tiền thuế chưa khớp với tổng tiền hàng và thuế suất đã chọn ở trên!\n" +
                    "Nhấn Có để lưu số liệu ngay lập tức, hoặc nhấn Không để kiểm tra lại số liệu thuế GTGT trước khi lưu", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes;
            else
                _info.Result = true;
            //kiem tra nha cung cap
            if (!_info.Result)  //tong tien khong khop, khong can tiep tuc kiem tra
                return;
            string makh = drMaster["MaKh"].ToString();
            string mtid = drMaster[_data.DrTableMaster["Pk"].ToString()].ToString();
            DataView dvVat = new DataView(_data.DsData.Tables[2]);
            dvVat.RowFilter = "MTID = '" + mtid + "' and MaKH <> '" + makh + "'";
            if (dvVat.Count > 0)
                _info.Result = XtraMessageBox.Show("Đối tượng trong khai báo thuế GTGT chưa khớp với đối tượng đã chọn ở trên!\n" +
                    "Nhấn Có để lưu số liệu ngay lập tức, hoặc nhấn Không để kiểm tra lại số liệu thuế GTGT trước khi lưu", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes;
            else
                _info.Result = true;
            //kiem tra thong tin hoa don
            dvVat.RowFilter = "MTID = '" + mtid + "' or MTID is null";
            
                foreach (DataRowView drv in dvVat)
                {
                    if (drv["MaHD"].ToString() != drMaster["MaHD"].ToString())
                        drv["MaHD"] = drMaster["MaHD"];
                    if (drv["KHMauHD"].ToString() != drMaster["KHMauHD"].ToString())
                        drv["KHMauHD"] = drMaster["KHMauHD"];
                    if (table != "MT23")
                    {
                        if (drv["NgayCT"].ToString() != drMaster["NgayCT"].ToString())
                            drv["NgayCT"] = drMaster["NgayCT"];
                        if (drv["NgayHD"].ToString() != drMaster["NgayHD"].ToString())
                            drv["NgayHD"] = drMaster["NgayHD"];
                        if (drv["SoHoaDon"].ToString() != drMaster["SoHoaDon"].ToString())
                            drv["SoHoaDon"] = drMaster["SoHoaDon"];
                        string seri = table.StartsWith("MT2") ? "SoSeries" : "SoSerie";
                        if (drv[seri].ToString() != drMaster["SoSeri"].ToString())
                            drv[seri] = drMaster["SoSeri"];
                        if (drv["TKThue"].ToString() != drMaster["TKThue"].ToString())
                            drv["TKThue"] = drMaster["TkThue"];
                        string tkdu = ((table.StartsWith("MT2") && table != "MT24") || table == "MT33") ? "TkCo" : "TkNo";
                        if (drv["TkDU"].ToString() != drMaster[tkdu].ToString())
                            drv["TkDU"] = drMaster[tkdu];
                        if (drv["DienGiai"].ToString() == "")
                            drv["DienGiai"] = drMaster["DienGiai"];
                    }
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
        public CheckVat()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        private decimal LayThueSuat(string MaThue)
        {
            Database dbData = Database.NewDataDatabase();
            object value = dbData.GetValue("select ThueSuat from DMThueSuat where MaThue = '" + MaThue + "'");
            if (value == null)
                return 0;
            return (Decimal.Parse(value.ToString()));
        }
    }
}
