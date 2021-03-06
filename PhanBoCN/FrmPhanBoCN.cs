using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using CDTLib;
using System.Globalization;

namespace PhanBoCN
{
    public partial class FrmPhanBoCN : DevExpress.XtraEditors.XtraForm
    {
        NumberFormatInfo nfi = new NumberFormatInfo();
        bool isKH = true;
        bool changed = false;
        Database db = Database.NewDataDatabase();
        DataTable dtCT;
        DataTable dtHD;
        DataTable dtTT;
        DataView dvCT;
        DataView dvHD;

        public FrmPhanBoCN(string extraSql)
        {
            InitializeComponent();
            nfi.NumberDecimalSeparator = ".";
            nfi.NumberGroupSeparator = ",";
            isKH = extraSql.StartsWith("1");
        }

        private void FrmPhanBoCN_Load(object sender, EventArgs e)
        {
            SetDefault();
            GetData();
            UpdateStatusBar();
        }

        private void barManager1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            switch (e.Item.Name)
            {
                case "beiXem":
                    GetData();
                    break;
                case "beiSelectAll":
                    ChonTatCa(beiSelectAll.Caption.StartsWith("C"));
                    break;
                case "beiPhanBo":
                    PhanBo();
                    break;
                case "beiXoaPB":
                    XoaPB();
                    break;
                case "beiSave":
                    LuuPB();
                    break;
                case "beiThoat":
                    KetThuc();
                    break;
            }
        }

        private void gvCT_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            int i;
            if (gvCT.IsGroupRow(e.FocusedRowHandle))
                i = gvCT.GetDataRowHandleByGroupRowHandle(e.FocusedRowHandle);
            else
                i = e.FocusedRowHandle;
            if (i < 0)
                return;
            string makh = gvCT.GetRowCellValue(i, "MaKH").ToString();
            string mant = gvCT.GetRowCellValue(i, "MaNT").ToString();
            string tk = gvCT.GetRowCellValue(i, "TK").ToString();
            string mtid = gvCT.GetRowCellValue(i, "MTID").ToString();

            string f = "[MaKH] = '" + makh + "' and [MaNT] = '" + mant + "' and [TK] = '" + tk + "'";
            if (beiLoaiXem.EditValue.ToString() == "1") //xem tương ứng chứng từ thanh toán và hóa đơn
                f += " and [MTIDTT] like '%" + mtid + "%'";
            gvHD.ActiveFilterString = f;
        }

        #region Phục vụ việc tính tổng số tiền và tổng số nợ trên status bar
        private void UpdateStatusBar()
        {
            if (dtCT != null)
            {
                dtCT.RowChanged += new DataRowChangeEventHandler(dtCT_RowChanged);
                if (dtCT.Rows.Count > 0)
                    dtCT_RowChanged(dtCT, new DataRowChangeEventArgs(dtCT.Rows[0], DataRowAction.Nothing));
            }
            if (dtHD != null)
            {
                dtHD.RowChanged += new DataRowChangeEventHandler(dtHD_RowChanged);
                if (dtHD.Rows.Count > 0)
                    dtHD_RowChanged(dtHD, new DataRowChangeEventArgs(dtHD.Rows[0], DataRowAction.Nothing));
            }
        }
        private void gvCT_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "Chon")
            {
                gvCT.SetFocusedRowCellValue(e.Column, e.Value);
                gvCT.FocusedColumn = gvCT.Columns["NgayCT"];
                gvCT.UpdateCurrentRow();
            }
        }

        private void gvHD_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "Chon")
            {
                gvHD.SetFocusedRowCellValue(e.Column, e.Value);
                gvHD.FocusedColumn = gvHD.Columns["NgayHD"];
                gvHD.UpdateCurrentRow();
            }
        }

        void dtHD_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            object o = dtHD.Compute("sum(ConLai)", "Chon = 1");
            decimal t = (o == null || o.ToString() == "") ? 0 : decimal.Parse(o.ToString());
            bsiNoChon.Caption = t.ToString("### ### ### ##0");

            o = dtHD.Compute("sum(ConLai)", "");
            t = (o == null || o.ToString() == "") ? 0 : decimal.Parse(o.ToString());
            bsiNo.Caption = t.ToString("### ### ### ##0");
        }

        void dtCT_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            object o = dtCT.Compute("sum(ConLai)", "Chon = 1");
            decimal t = (o == null || o.ToString() == "") ? 0 : decimal.Parse(o.ToString());
            bsiTienChon.Caption = t.ToString("### ### ### ##0");

            o = dtCT.Compute("sum(ConLai)", "");
            t = (o == null || o.ToString() == "") ? 0 : decimal.Parse(o.ToString());
            bsiTien.Caption = t.ToString("### ### ### ##0");
        }
        #endregion

        private void SetDefault()
        {
            beiTT.EditValue = false;
            beiLoaiPBTD.EditValue = 0;
            beiLoaiXem.EditValue = 0;
            riLoaiXem.EditValueChanged += new EventHandler(riLoaiXem_EditValueChanged);
            string sql = "select MaKH, TenKH from DMKH where Duyet = 1";
            if (isKH)
                sql += " and isKH = 1";
            else
                sql += " and isNCC = 1";
            DataTable dtKH = db.GetDataTable(sql);
            riKH.DataSource = dtKH;
            riKH.ValueMember = "MaKH";
            riKH.DisplayMember = "MaKH";
            riKH.ImmediatePopup = true;
            riKH.KeyUp += new KeyEventHandler(riKH_KeyUp);
            riKH.BestFit();
            dtTT = db.GetDataTable("select * from ThanhToanHD");

            if (Config.GetValue("KyKeToan") == null || Config.GetValue("NamLamViec") == null)
                return;
            string t = Config.GetValue("KyKeToan").ToString();
            string n = Config.GetValue("NamLamViec").ToString();
            DateTime tungay = DateTime.Parse(t + "/01/" + n);
            DateTime dengay = tungay.AddMonths(1).AddDays(-1);
            beiTuNgay.EditValue = tungay;
            beiDenNgay.EditValue = dengay;
        }

        void riLoaiXem_EditValueChanged(object sender, EventArgs e)
        {
            beiLoaiXem.EditValue = (sender as RadioGroup).SelectedIndex;
            if (beiLoaiXem.EditValue.ToString() == "2")
            {
                gcCT.Visible = false;
                splitterControl1.Visible = false;
                bXuLy.Visible = false;
                gvHD.ClearColumnsFilter();
                gvHD.OptionsView.ShowAutoFilterRow = true;
                gvHD.Columns["TenKH"].Visible = true;
                gvHD.Columns["TenKH"].GroupIndex = 0;
                gvHD.Columns["TK"].Visible = true;
                gvHD.Columns["MaNT"].Visible = true;
            }
            else
            {
                gcCT.Visible = true;
                splitterControl1.Visible = true;
                bXuLy.Visible = true;
                gvHD.OptionsView.ShowAutoFilterRow = false;
                gvHD.Columns["TenKH"].Visible = false;
                gvHD.Columns["TenKH"].GroupIndex = -1;
                gvHD.Columns["TK"].Visible = false;
                gvHD.Columns["MaNT"].Visible = false;
                gvCT_FocusedRowChanged(gvCT, new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs(-1, gvCT.FocusedRowHandle));
            }
        }

        void riKH_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                (sender as LookUpEdit).EditValue = null;
        }

        private void GetData()
        {
            if (changed)
                if (XtraMessageBox.Show("Dữ liệu hiện tại chưa được lưu, bạn có muốn xem dữ liệu khác không?",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            if (beiTuNgay.EditValue == null || beiDenNgay.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng nhập thông tin cần xem số liệu", Config.GetValue("PackageName").ToString());
                return;
            }
            string dk = isKH ? "b.TK like '131%'" : "b.TK like '331%'";
            if (beiKH.EditValue != null)
                dk += " and b.MaKH = '" + beiKH.EditValue.ToString() + "'";
            string dkdkct = dk + (isKH ? " and b.DuCo > 0 and b.DuNo = 0 " : " and b.DuNo > 0 and b.DuCo = 0 ");
            string dkct = string.Format(dk + " and b.NgayCT between '{0}' and '{1}'", beiTuNgay.EditValue, beiDenNgay.EditValue) + (isKH ? " and b.PsCo > 0 and b.PsNo = 0 " : " and b.PsNo > 0 and b.PsCo = 0 ");
            string sql = @"select Chon = cast(0 as bit), MTID, MaCT, NgayCT, SoCT, TK, MaKH, TenKH, MaNT, PS, isnull(TT,0) as DaPB, PS - isnull(TT,0) as ConLai from
                    (select MTID, MaCT, NgayCT, SoCT, TK, MaKH, TenKH, MaNT, sum(PsNo + PsCo) as PS,
                    TT = (select sum(SoTien) from ThanhToanHD tt where MTID = tt.MTIDTT)
                    from BLTK b where " + dkct +
                    @"group by MTID, MaCT, NgayCT, SoCT, TK, MaKH, TenKH, MaNT
                    union all
                    select MTID, 'DK', NgayCT, SoCT, TK, b.MaKH, kh.TenKH, MaNT, DuNo + DuCo as PS,
                    TT = (select sum(SoTien) from ThanhToanHD tt where MTID = tt.MTIDTT)
                    from OBHD b inner join DMKH kh on b.MaKH = kh.MaKH where " + dkdkct + ") t";
            if (!Boolean.Parse(beiTT.EditValue.ToString()))
                sql += " where PS - isnull(TT,0) <> 0";
            gcCT.DataSource = dtCT = db.GetDataTable(sql);
            gvCT.BestFitColumns();

            string dkdkhd = dk + (isKH ? " and b.DuNo > 0 and b.DuCo = 0 " : " and b.DuCo > 0 and b.DuNo = 0 ");
            if (beiHD.EditValue != null && beiHD.EditValue.ToString() != "")
                dk += " and m.SoHoaDon like '%" + beiHD.EditValue.ToString() + "%'";
            string dkhd = string.Format(dk + " and b.NgayCT between '{0}' and '{1}'", beiTuNgay.EditValue, beiDenNgay.EditValue) + (isKH ? " and b.PsNo > 0 and b.PsCo = 0 " : " and b.PsCo > 0 and b.PsNo = 0 ");
            sql = @"select Chon = cast(0 as bit), MTID, MaCT, NgayHD, SoHoaDon, HanTT, TK, MaKH, TenKH, MaNT, PS, isnull(TT,0) as TT, PS - isnull(TT,0) as ConLai, MTIDTT from
                    (select b.MTID, b.MaCT, m.NgayHD, m.SoHoaDon, HanTT = dateadd(dd,m.HanTT,m.NgayHD), b.TK, b.MaKH, b.TenKH, b.MaNT, sum(b.PsNo + b.PsCo) as PS,
                    TT = (select sum(SoTien) from ThanhToanHD tt where MTID = tt.MTIDHD),
                    MTIDTT = stuff((select ',' + cast(tt.MTIDTT as varchar(50)) from ThanhToanHD tt where MTID = tt.MTIDHD for xml path('')),1,1,'')
                    from BLTK b inner join MT32 m on b.MTID = m.MT32ID where " + dkhd +
                    @"group by b.MTID, b.MaCT, m.NgayHD, m.SoHoaDon, m.HanTT, b.TK, b.MaKH, b.TenKH, b.MaNT
                    union all
                    select MTID, 'DK', NgayCT, SoCT, HanTT = dateadd(dd,b.HanTT,NgayCT), TK, b.MaKH, kh.TenKH, MaNT, DuNo + DuCo as PS,
                    TT = (select sum(SoTien) from ThanhToanHD tt where MTID = tt.MTIDHD),
                    MTIDTT = stuff((select ',' + cast(tt.MTIDTT as varchar(50)) from ThanhToanHD tt where MTID = tt.MTIDHD for xml path('')),1,1,'')
                    from OBHD b inner join DMKH kh on b.MaKH = kh.MaKH where " + dkdkhd + ") t";
            gcHD.DataSource = dtHD = db.GetDataTable(sql);
            gvHD.BestFitColumns();
        }

        private void ChonTatCa(bool chon)
        {
            foreach (DataRow dr in dtCT.Rows)
                if (Boolean.Parse(dr["Chon"].ToString()) != chon)
                    dr["Chon"] = chon;
            foreach (DataRow dr in dtHD.Rows)
                if (Boolean.Parse(dr["Chon"].ToString()) != chon)
                    dr["Chon"] = chon;
            beiSelectAll.Caption = chon ? "Bỏ chọn tất cả" : "Chọn tất cả";
        }

        private bool KiemTra(bool xoa)
        {
            dvCT = new DataView(dtCT);
            dvHD = new DataView(dtHD);
            dvCT.RowFilter = dvHD.RowFilter = "Chon = 1";
            if (!xoa)
            {
                if (dvCT.Count == 0 || dvHD.Count == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn chứng từ và hóa đơn để phân bổ", Config.GetValue("PackageName").ToString());
                    return true;
                }
                return false;
            }
            else
            {
                if (dvHD.Count == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn hóa đơn công nợ cần xóa phân bổ", Config.GetValue("PackageName").ToString());
                    return true;
                }
                return false;
            }
        }

        private void PhanBo()
        {
            if (KiemTra(false))  //kiểm tra đã chọn đủ chứng từ và hóa đơn chưa
                return;
            dvCT.Sort = "MaKH, TK, MaNT, NgayCT";   //sắp xếp dữ liệu theo trình tự phân bổ
            dvHD.Sort = beiLoaiPBTD.EditValue.ToString() == "0" ? "MaKH, TK, MaNT, NgayHD" : "MaKH, TK, MaNT, HanTT";
            List<string> lstKH = new List<string>();    //lấy ra các khách hàng sẽ phân bổ
            foreach (DataRowView drvCT in dvCT)
            {
                string kh = drvCT["MaKH"].ToString() + ";" + drvCT["TK"].ToString() + ";" + drvCT["MaNT"].ToString();
                if (!lstKH.Contains(kh))
                    lstKH.Add(kh);
            }
            foreach (string kh in lstKH)        //duyệt theo từng khách hàng đã lấy ra
            {
                string[] s = kh.Split(';');
                if (s.Length != 3)  //không hợp lệ
                    continue;
                dvCT.RowFilter = dvHD.RowFilter = string.Format("Chon = 1 and MaKH = '{0}' and TK = '{1}' and MaNT = '{2}'", s[0], s[1], s[2]);
                foreach (DataRowView drvCT in dvCT)     //duyệt chứng từ
                {
                    string mtidtt = drvCT["MTID"].ToString();
                    decimal tct = decimal.Parse(drvCT["ConLai"].ToString(), nfi);
                    foreach (DataRowView drvHD in dvHD) //duyệt hóa đơn
                    {
                        if (tct <= 0)
                            break;
                        decimal thd = decimal.Parse(drvHD["ConLai"].ToString(), nfi);
                        if (thd <= 0)
                            continue;
                        decimal tt = Math.Min(tct, thd);
                        drvHD["ConLai"] = decimal.Parse(drvHD["ConLai"].ToString(), nfi) - tt;
                        drvHD["TT"] = decimal.Parse(drvHD["TT"].ToString(), nfi) + tt;
                        if (drvHD["MTIDTT"].ToString() != "")
                            drvHD["MTIDTT"] = drvHD["MTIDTT"].ToString() + "," + mtidtt;
                        else
                            drvHD["MTIDTT"] = mtidtt;
                        tct -= tt;
                        //cập nhật vào bảng thanh toán
                        string mtidhd = drvHD["MTID"].ToString();
                        DataRow[] drTTs = dtTT.Select(string.Format("MTIDTT = '{0}' and MTIDHD = '{1}'", mtidtt, mtidhd));
                        if (drTTs.Length == 0)  //chưa có -> thêm mới
                        {
                            DataRow dr = dtTT.NewRow();
                            dr["MTIDTT"] = mtidtt;
                            dr["MTIDHD"] = mtidhd;
                            dr["SoTien"] = tt;
                            dtTT.Rows.Add(dr);
                        }
                        else                    //có rồi -> chỉ cần cập nhật số tiền thanh toán
                        {
                            drTTs[0]["SoTien"] = tt;
                        }
                    }
                    drvCT["ConLai"] = Math.Max(0, tct);
                    drvCT["DaPB"] = decimal.Parse(drvCT["PS"].ToString(), nfi) - decimal.Parse(drvCT["ConLai"].ToString(), nfi);
                }
            }
            changed = true;
            ChonTatCa(false);       //bỏ chọn sau khi phân bổ
        }

        private void XoaPB()
        {
            if (KiemTra(true))
                return;
            foreach (DataRowView drvHD in dvHD)
            {
                decimal thd = decimal.Parse(drvHD["TT"].ToString(), nfi);
                string mtidhd = drvHD["MTID"].ToString();
                DataRow[] drTTs = dtTT.Select(string.Format("MTIDHD = '{0}'", mtidhd));
                foreach (DataRow drTT in drTTs)
                {
                    if (thd <= 0)
                        break;
                    string mtidtt = drTT["MTIDTT"].ToString();
                    decimal pb = decimal.Parse(drTT["SoTien"].ToString(), nfi);
                    DataRow[] drCTs = dtCT.Select("MTID = '" + mtidtt + "'");
                    if (drCTs.Length > 0)   //có chứng từ thanh toán trên giao diện -> cập nhật trên giao diện
                    {
                        drCTs[0]["DaPB"] = decimal.Parse(drCTs[0]["DaPB"].ToString(), nfi) - pb;
                        drCTs[0]["ConLai"] = decimal.Parse(drCTs[0]["ConLai"].ToString(), nfi) + pb;
                    }
                    thd -= pb;
                    //xóa thông tin phân bổ trên bảng thanh toán
                    drTT.Delete();
                }
                drvHD["MTIDTT"] = DBNull.Value;
                drvHD["TT"] = 0;
                drvHD["ConLai"] = decimal.Parse(drvHD["PS"].ToString(), nfi);
            }
            ChonTatCa(false);       //bỏ chọn sau khi xóa phân bổ
            changed = true;
        }

        private void LuuPB()
        {
            if (!changed)
                return;
            if (db.UpdateDataTable("select * from ThanhToanHD", dtTT))
            {
                XtraMessageBox.Show("Đã cập nhật kết quả phân bổ", Config.GetValue("PackageName").ToString());
                dtTT = db.GetDataTable("select * from ThanhToanHD");
            }
            changed = false;
        }

        private void KetThuc()
        {
            if (changed)
                if (XtraMessageBox.Show("Dữ liệu chưa được lưu, bạn có muốn thoát không?",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            this.Close();
        }
    }
}