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

namespace LocHoaDon
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        public frmMain()
        {
            InitializeComponent();
        }
        Database db = Database.NewDataDatabase();
        public DataTable dtSource = new DataTable();
        public DateTime NgayCT;
        private void frmMain_Load(object sender, EventArgs e)
        {
            GetNhomKH();
            GetKhachHang("");
            dtpTuNgay.EditValue = dtpDenNgay.EditValue = DateTime.Today;
        }

        private void GetNhomKH()
        {
            string sql = "select * from dmnhomkh";
            DataTable dt = db.GetDataTable(sql);
            lookupNhomKH.Properties.DataSource = dt;
            lookupNhomKH.Properties.ValueMember = "MaNhomKH";
            lookupNhomKH.Properties.DisplayMember = "MaNhomKH";
        }

        private void GetKhachHang(string nhomkh)
        {
            string sql = "";
            if (nhomkh == "")
                sql = "select MaKH,TenKH from dmkh where InActive2 = 0";
            else
                sql = "select MaKH,TenKH from dmkh where InActive2 = 0 and Nhom1 = '" + nhomkh + "'";
            DataTable dt = db.GetDataTable(sql);
            gridLKKH.Properties.DataSource = dt;
            gridLKKH.Properties.DisplayMember = "TenKH";
            gridLKKH.Properties.ValueMember = "MaKH";
            gridLookUpEdit1View.BestFitColumns();
        }

        private void lookupNhomKH_EditValueChanged(object sender, EventArgs e)
        {
            if (lookupNhomKH.EditValue != null)
                GetKhachHang(lookupNhomKH.EditValue.ToString());
            else
                GetKhachHang("");
        }

        private void gcHoaDon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            dtSource = new DataTable();
            dtSource = gcHoaDon.DataSource as DataTable;
            NgayCT = new DateTime();
            NgayCT = dtpDenNgay.DateTime;
            this.Close();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string sql = @"SELECT [MT31].[MT31ID] as MTID, MaCT, [SoCT], [SoHoaDon], [NgayCT], [MT31].[MaKH], DMKH.TenKH,  
                            [NgayTT] = case when MT31.HanTT is not null then dateadd(day,MT31.HanTT,NgayHD) end, 
                            [DienGiai], [TKNO], [MaNT], [TyGia], [Ttien], [TtienNT], DaTTNT, DaTT, Nhom1 as NhomKH, 
                            [ConLaiNT] = [TTienNT] - [DaTTNT], [ConLai] = [Ttien] - [DaTT], 
                            MT31.HanTT, MaBP, MaVV, MaPhi, MaCongTrinh, DMKH.LoaiCongNo
                            FROM [MT31] inner join [DT31] on MT31.MT31ID = DT31.MT31ID
                            inner join DMKH on DMKH.MaKH = MT31.MaKH
                            WHERE TKNO IN (SELECT TK FROM DMTK WHERE TKCONGNO = 1)
                            --and (([TTienNT] - [DaTTNT])<>0 or ([Ttien] - [DaTT])<>0) 
                            and MT31.NgayCT between '" + dtpTuNgay.EditValue.ToString() + @"' and '" + dtpDenNgay.EditValue.ToString() + @"'                             
                            union
                            SELECT [MT31].[MT31ID], MaCT, [SoCT], [SoHoaDon], [NgayCT], [MT31].[MaKH], DMKH.TenKH, 
                            [NgayTT] = case when [MT31].HanTT is not null then dateadd(day,[MT31].HanTT,NgayHD) end, 
                            [DienGiai], [TKNO], [MaNT], [TyGia], [Ttien], [TtienNT], DaTTNT, DaTT, Nhom1 as NhomKH, 
                            [ConLaiNT] = [TTienNT] - [DaTTNT], [ConLai] = [Ttien] - [DaTT], 
                            [MT31].HanTT, null as MaBP, null as MaVV, null as MaPhi, null as MaCongTrinh, DMKH.LoaiCongNo  
                            FROM [MT31] inner join DMKH on DMKH.MaKH = MT31.MaKH
                            WHERE TKNO IN (SELECT TK FROM DMTK WHERE TKCONGNO = 1)
                            --and (([TTienNT] - [DaTTNT])<>0 or ([Ttien] - [DaTT])<>0) 
                            and MT31.NgayCT between '" + dtpTuNgay.EditValue.ToString() + @"' and '" + dtpDenNgay.EditValue.ToString() + @"'                             
                            union 
                            SELECT [DT32].[DT32ID] as MTID, MaCT, [SoCT], [SoHoaDon], [NgayCT], [MaKHDT] as MaKH, DMKH.TenKH,  
                            [NgayTT] = case when MT32.HanTT is not null then dateadd(day,MT32.HanTT,NgayCT) end, 
                            [DienGiai], [TKNO], [MaNT], [TyGia], 
                            case when [PS]-[TienGiam]-isnull([CK],0) >0 then [PS]-[TienGiam]-isnull([CK],0) else 0 end as Ttien, 
                            case when [PSNT]-[TienGiam]-isnull([CK],0)>0 then [PSNT]-[TienGiam]-isnull([CK],0) else 0 end as TtienNT, 
                            DTToanNT, DTToan, Nhom1 as NhomKH, 
                            case when [PSNT] - ([DTToanNT]+[TienGiam]+isnull([CK],0)) > 0 then [PSNT] - ([DTToanNT]+[TienGiam]+isnull([CK],0)) else 0 end as [ConLaiNT], 
                            case when [PS] - ([DTToan]+[TienGiam] + isnull([CK],0)) > 0 then [PS] - ([DTToan]+[TienGiam] + isnull([CK],0)) else 0 end as [ConLai] , 
                            MT32.HanTT, MaBP, MaVV, MaPhi, MaCongTrinh, DMKH.LoaiCongNo
                            FROM [MT32] inner join [DT32] on MT32.MT32ID = DT32.MT32ID
                            inner join DMKH on DMKH.MaKH = DT32.MaKHDT
                            WHERE TKNO IN (SELECT TK FROM DMTK WHERE TKCONGNO = 1)
                            --and (([PSNT] - ([DaTTNT]+[TienGiam]+isnull([CK],0)))<>0 or ([PS] - ([DaTT]+[TienGiam] + isnull([CK],0)))<>0)
                            and MT32.NgayCT between '" + dtpTuNgay.EditValue.ToString() + @"' and '" + dtpDenNgay.EditValue.ToString() + @"'                             
                            union
                            SELECT [MT33].[MT33ID] as MTID, MaCT, [SoCT], [SoHoaDon], [NgayCT], MT33.[MaKH], DMKH.TenKH,  
                            [NgayTT] = case when MT33.HanTT is not null then dateadd(day,MT33.HanTT,NgayCT) end, 
                            [DienGiai], [TKCO], [MaNT], [TyGia], -[Ttien], -[TtienNT], DaTTNT, DaTT,  Nhom1 as NhomKH, 
                            [ConLaiNT] = -[TTienNT] - [DaTTNT], [ConLai] = -[Ttien] - [DaTT] , 
                            MT33.HanTT, MaBP, MaVV, MaPhi, MaCongTrinh, DMKH.LoaiCongNo
                            FROM [MT33] inner join [DT33] on MT33.MT33ID = DT33.MT33ID
                            inner join DMKH on DMKH.MaKH = MT33.MaKH
                            WHERE TKCO IN (SELECT TK FROM DMTK WHERE TKCONGNO = 1)
                            --and ((-[TTienNT] - [DaTTNT])<>0 or (-[Ttien] - [DaTT])<>0)
                            and MT33.NgayCT between '" + dtpTuNgay.EditValue.ToString() + @"' and '" + dtpDenNgay.EditValue.ToString() + "'";
            DataTable dt = db.GetDataTable(sql);
            DataColumn chon = new DataColumn("Chon", typeof(bool));
            chon.DefaultValue = false;
            dt.Columns.Add(chon);
            DataView dvCongNo = new DataView(dt);
            string con = string.Format("{0}", lookupNhomKH.EditValue == null ? "" : "NhomKH = '" + lookupNhomKH.EditValue.ToString() + "'");
            if (gridLKKH.EditValue != null && gridLKKH.EditValue.ToString()!="")
            {
                if (con != "")
                    con += " and MaKH = '" + gridLKKH.EditValue.ToString() + "'";
                else
                    con = "MaKH = '" + gridLKKH.EditValue.ToString() + "'";
            }
            if (con != "")
                con += " and LoaiCongNo = '" + spinLoaiCN.EditValue.ToString() + "'";
            else
                con = "LoaiCongNo = '" + spinLoaiCN.EditValue.ToString() + "'";
            if (txtSoCT.EditValue != null && txtSoCT.Text.Trim() != "")
            {
                if (con != "")
                    con += " and SoCT = '" + txtSoCT.Text + "'";
                else
                    con = "SoCT = '" + txtSoCT.Text + "'";
            }
            if (con != "")
                con += " and (ConLaiNT <>0 or ConLai<>0)";
            else
                con = " ConLaiNT <>0 or ConLai<>0";
            if (con != "")
                dvCongNo.RowFilter = con;
            gcHoaDon.DataSource = dvCongNo.ToTable();
            gvHoaDon.BestFitColumns();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            DataTable dt = gcHoaDon.DataSource as DataTable;
            foreach (DataRow row in dt.Rows)
                row["Chon"] = chkAll.Checked;
        }               
    }
}