using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Plugins;
using CDTDatabase;
using DevExpress.XtraEditors;
using CDTLib;
using System.Windows.Forms;

namespace ThuHoaDon
{
    public class ThuHoaDon : ICData
    {
        public ThuHoaDon()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();
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
              string tableName = _data.DrTableMaster["TableName"].ToString();

            if (tableName == "MT31" || tableName == "MT32" || tableName == "MT33")
            {
                CheckInvoice();
            }

            if (tableName == "MT11" || tableName == "MT15" || tableName == "MT51")
            {
                if (CheckPayment())
                {
                    UpdateInvoices();
                    //_info.Result = true;
                }
                else
                {
                    _info.Result = XtraMessageBox.Show("Số tiền của phiếu không khớp với chi tiết công nợ\n" +
                        "Bạn có muốn lưu chứng từ không?", Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo) == DialogResult.Yes;
                }
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion

        private bool CheckPayment()
        { 
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Deleted)
                return true;
            DataView dv1 = null, dv2 = null;
            int n = (_data.DrTableMaster["TableName"].ToString() == "MT51") ? 4 : 2;
            if (drMaster.RowState == DataRowState.Added)
            {
                dv1 = new DataView(_data.DsData.Tables[1]);
                dv2 = new DataView(_data.DsData.Tables[n]);
                dv1.RowStateFilter = DataViewRowState.Added;
                dv2.RowStateFilter = DataViewRowState.Added;
            }
            if (drMaster.RowState == DataRowState.Modified || drMaster.RowState == DataRowState.Unchanged)
            {
                string pk = _data.DrTableMaster["Pk"].ToString();
                string mtid = drMaster[pk].ToString();
                dv1 = new DataView(_data.DsData.Tables[1]);
                dv2 = new DataView(_data.DsData.Tables[n]);
                dv1.RowFilter = pk + " = '" + mtid + "'";
                dv2.RowFilter = "MT11ID = '" + mtid + "'";
            }
            if (dv1 == null || dv2 == null)
                return true;
            DataTable dt1 = dv1.ToTable();
            DataTable dt2 = dv2.ToTable();
            if (dt2.Rows.Count == 0)
                return true;
            object o1 = dt1.Compute("sum(Ps)", "");
            object o2 = dt2.Compute("sum(TT)", "");
            if (o1 == null || o1.ToString() == "" ||
                o2 == null || o2.ToString() == "")
                return true;
            return (decimal.Parse(o1.ToString()) == decimal.Parse(o2.ToString()));
        }

        private void UpdateInvoices()
        {
            string tableName = _data.DrTableMaster["TableName"].ToString();
            Database dbData = _data.DbData;

            DataView dvMT11 = new DataView(_data.DsData.Tables[0]);
            dvMT11.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.CurrentRows;
            DataRowView drvMaster = dvMT11[_data.CurMasterIndex];
            string pk = _data.DrTableMaster["Pk"].ToString();
            string pkValue = drvMaster[pk].ToString();
            string MaKH = drvMaster["MaKH"].ToString();
            int n = tableName == "MT51" ? 4 : 2;
            DataView dvDT34 = new DataView(_data.DsData.Tables[n]);
            dvDT34.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.CurrentRows;
            dvDT34.RowFilter = "MT11ID = '" + pkValue + "'";
            //Kiểm tra các chứng từ hợp lệ mới xử lý tiếp
            bool flag = true;
            if (drvMaster.Row.RowState != DataRowState.Deleted)
                flag = CheckCorrect(dvDT34, MaKH);
            if (!flag)
            {
                _info.Result = false;
                return;
            }
            foreach (DataRowView drvData in dvDT34)
            {
                string MT31ID = drvData["MT31ID"].ToString();
                string mt31idold = "";
                string TTNT = "0", TT = "0";
                string TTNT1 = "0", TT1 = "0";
                if (drvData.Row.RowState == DataRowState.Added)
                {
                    TTNT = drvData["TTNT"].ToString().Replace(",", ".");
                    TT = drvData["TT"].ToString().Replace(",", ".");
                }

                if (drvData.Row.RowState == DataRowState.Unchanged)
                    continue;

                if (drvData.Row.RowState == DataRowState.Modified)
                {
                    //giá trị thanh toán trước khi sửa
                    mt31idold = drvData.Row["MT31ID", DataRowVersion.Original].ToString();
                    TTNT1 = drvData.Row["TTNT", DataRowVersion.Original].ToString().Replace(",", ".");
                    TT1 = drvData.Row["TT", DataRowVersion.Original].ToString().Replace(",", ".");
                    // giá trị thanh toán sau khi sửa
                    TTNT = drvData.Row["TTNT", DataRowVersion.Current].ToString().Replace(",", ".");
                    TT = drvData.Row["TT", DataRowVersion.Current].ToString().Replace(",", ".");
                }

                if (drvData.Row.RowState == DataRowState.Deleted)
                {
                    TTNT1 = drvData.Row["TTNT", DataRowVersion.Original].ToString().Replace(",", ".");
                    TT1 = drvData.Row["TT", DataRowVersion.Original].ToString().Replace(",", ".");
                }
                if (mt31idold == "" || MT31ID == mt31idold)    //khong thay doi chung tu cong no
                {
                    string sql = "update {0} set DaTTNT = DaTTNT - (" + TTNT1 + ")+(" + TTNT + "), DaTT = DaTT - (" + TT1 + ")+(" + TT + ") where {1} = '" + MT31ID + "'";
                    int rec = 0;
                    dbData.UpdateByNonQuery(string.Format(sql, "MT31", "MT31ID"), ref rec);
                    //if (rec == 0)
                    //{
                        if (TT.Contains("-") || TT1.Contains("-"))
                            dbData.UpdateByNonQuery(string.Format(sql + " and HuyHD = 0", "MT32", "MT32ID"), ref rec);
                        else
                            dbData.UpdateByNonQuery(string.Format(sql, "MT32", "MT32ID"), ref rec);
                    //}
                    //if (rec == 0)
                        dbData.UpdateByNonQuery(string.Format(sql, "MT33", "MT33ID"), ref rec);
                    //if (rec == 0)
                        dbData.UpdateByNonQuery(string.Format(sql, "OBHD", "MTID"), ref rec);
                    //_info.Result = (rec == 1);
                    //if (_info.Result == false)
                    //    break;
                    //if (rec != 1)
                    //    break;
                }
                else                 //thay doi chung tu cong no
                {
                    string sql1 = "update {0} set DaTTNT = DaTTNT +(" + TTNT + "), DaTT = DaTT +(" + TT + ") where {1} = '" + MT31ID + "'";
                    string sql2 = "update {0} set DaTTNT = DaTTNT - (" + TTNT1 + "), DaTT = DaTT - (" + TT1 + ") where {1} = '" + mt31idold + "'";
                    int rec = 0;
                    dbData.UpdateByNonQuery(string.Format(sql1, "MT31", "MT31ID"), ref rec);
                    dbData.UpdateByNonQuery(string.Format(sql2, "MT31", "MT31ID"), ref rec);
                    //if (rec == 0)
                    //{
                        if (TT.Contains("-") || TT1.Contains("-"))
                        {
                            dbData.UpdateByNonQuery(string.Format(sql1 + " and HuyHD = 0", "MT32", "MT32ID"), ref rec);
                            dbData.UpdateByNonQuery(string.Format(sql2 + " and HuyHD = 0", "MT32", "MT32ID"), ref rec);
                        }
                        else
                        {
                            dbData.UpdateByNonQuery(string.Format(sql1, "MT32", "MT32ID"), ref rec);
                            dbData.UpdateByNonQuery(string.Format(sql2, "MT32", "MT32ID"), ref rec);
                        }
                    //}
                    //if (rec == 0)
                    //{
                        dbData.UpdateByNonQuery(string.Format(sql1, "MT33", "MT33ID"), ref rec);
                        dbData.UpdateByNonQuery(string.Format(sql2, "MT33", "MT33ID"), ref rec);
                    //}
                    //if (rec == 0)
                    //{
                        dbData.UpdateByNonQuery(string.Format(sql1, "OBHD", "MTID"), ref rec);
                        dbData.UpdateByNonQuery(string.Format(sql2, "OBHD", "MTID"), ref rec);
                    //}
                    //_info.Result = (rec == 1);
                    //if (_info.Result == false)
                    //    break;
                    //if (rec != 1)
                    //    break;
                }
            }
        }

        private void CheckInvoice()
        {
            DataView dvMT = new DataView(_data.DsData.Tables[0]);
            dvMT.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.CurrentRows;
            DataRowView drvMaster = dvMT[_data.CurMasterIndex];
            string MTID = drvMaster[_data.DrTableMaster["Pk"].ToString()].ToString();
            string sql = "select MT31ID from DT34 where MT31ID = '" + MTID + "' and TT > 0";
            Database dbData = _data.DbData;
            if (dbData.GetDataTable(sql).Rows.Count == 0)
            {
                _info.Result = true;
                return;
            }

            if (drvMaster.Row.RowState == DataRowState.Deleted)
            {
                string msg = "Không thể xóa vì hóa đơn này đã được thanh toán. Cần xóa chứng từ thu hồi công nợ trước!";
                if (Config.GetValue("Language").ToString() == "1")
                    msg = UIDictionary.Translate(msg);
                XtraMessageBox.Show(msg);
                _info.Result = false;
                return;
            }

            if (drvMaster.Row.RowState == DataRowState.Modified)
            {
                if (!drvMaster.Row["TTien", DataRowVersion.Current].Equals(drvMaster.Row["TTien", DataRowVersion.Original])
                    || !drvMaster.Row["MaKH", DataRowVersion.Current].Equals(drvMaster.Row["MaKH", DataRowVersion.Original])
                    || !drvMaster.Row["NgayCT", DataRowVersion.Current].Equals(drvMaster.Row["NgayCT", DataRowVersion.Original])
                    || !drvMaster.Row["TkNo", DataRowVersion.Current].Equals(drvMaster.Row["TkNo", DataRowVersion.Original]))
                {
                    string msg = "Không thể điều chỉnh vì hóa đơn này đã được thanh toán. Cần xóa chứng từ thu hồi công nợ trước!";
                    if (Config.GetValue("Language").ToString() == "1")
                        msg = UIDictionary.Translate(msg);
                    XtraMessageBox.Show(msg);
                    _info.Result = false;
                    return;
                }
            }
            //if (drvMaster.Row.RowState == DataRowState.Unchanged)
            //{
            //    DataView dvDT = new DataView(_data.DsData.Tables[1]);
            //    dvDT.RowStateFilter = DataViewRowState.ModifiedCurrent;
            //    dvDT.RowFilter = _data.DrTableMaster["Pk"].ToString() + " = '" + MTID + "'";
            //    if (dvDT.Count > 0)
            //    {
            //        string msg = "Không thể điều chỉnh vì hóa đơn này đã được thanh toán. Cần điều chỉnh chứng từ thu hồi công nợ trước!";
            //        if (Config.GetValue("Language").ToString() == "1")
            //            msg = UIDictionary.Translate(msg);
            //        XtraMessageBox.Show(msg); 
            //        _info.Result = false;
            //        return;
            //    }
            //}
            _info.Result = true;
        }

        private bool CheckCorrect(DataView dvChungTu, string MaKH)
        {
            string sql = "", soct = "";
            DataTable dt = new DataTable();
            foreach (DataRowView drv in dvChungTu)
            {
                if (drv.Row.RowState == DataRowState.Deleted)
                    continue;
                sql = "select * from whoadonban where MT31ID = '" + drv.Row["MT31ID", DataRowVersion.Current].ToString() + "' and MaKh = '" + MaKH + "'";
                dt = db.GetDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    sql = "select * from whoadonban where MT31ID = '" + drv.Row["MT31ID", DataRowVersion.Current].ToString() + "'";
                    dt = db.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                        soct = dt.Rows[0]["SoCT"].ToString();
                    XtraMessageBox.Show("Hóa đơn có số chứng từ " + soct + " không phải của khách hàng đã chọn.", Config.GetValue("PackageName").ToString());
                    return false;                    
                }
            }
            return true;
        }    
    }
}
