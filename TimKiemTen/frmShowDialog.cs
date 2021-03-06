using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTLib;
using CDTDatabase;

namespace TimKiemTen
{
    public partial class frmShowDialog : DevExpress.XtraEditors.XtraForm
    {
        public frmShowDialog()
        {
            InitializeComponent();
            GetDS();
        }

        Database db = Database.NewDataDatabase();

        private void btnOK_Click(object sender, EventArgs e)
        {            
            if (lookUpEditVT.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng chọn tên liên quan!", Config.GetValue("PackageName").ToString());
                return;
            }
          
            if (Config.GetValue("GroupName") != null && Config.GetValue("GroupName").ToString() == "PKD" && LookupKH.EditValue == null)
            {
                XtraMessageBox.Show("Chưa chọn khách hàng!", Config.GetValue("PackageName").ToString());
                return;
            }
            string tenvt = lookUpEditVT.EditValue != null ? lookUpEditVT.EditValue.ToString() : "";
            string makh = LookupKH.EditValue != null ? LookupKH.EditValue.ToString() : "";
            string sql = "select * from dmvt where tenvt = N'" + tenvt + "'";
            DataTable dt = db.GetDataTable(sql);
            object mavt = null;
            if (dt.Rows.Count > 0)
                mavt = dt.Rows[0]["MaVT"].ToString();            
            sql = "insert into NKTimKiem(UserName,Ngay,MaVT,SL,MaKH) values (@UserName, @Ngay, @MaVT, @SL, @MaKH)";            
            string[] paraNames = new string[] { "@UserName", "@Ngay", "@MaVT", "@SL", "@MaKH" };
            object[] paraValues = new object[] { Config.GetValue("UserName"), DateTime.Today, mavt != null ? mavt : DBNull.Value, spinEdit1.Value, LookupKH.EditValue != null ? LookupKH.EditValue : DBNull.Value };
            db.UpdateDatabyPara(sql, paraNames, paraValues);

            dt = GetData(tenvt);
            frmResult1 frm = new frmResult1(dt);
            frm.Text = Config.GetValue("PackageName").ToString();
            if (!frm.hasData)
                frm.Close();
            else
                frm.ShowDialog();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void GetDS()
        {
            //khách hàng
            string sql = "select * from dmkh where iskh = '1' and InActive = '0' ";
            DataTable dt = db.GetDataTable(sql);
            LookupKH.Properties.DataSource = dt;
            LookupKH.Properties.DisplayMember = "TenKH";
            LookupKH.Properties.ValueMember = "MaKH";
            //Vật tư
            sql = "select  distinct(TenVT) as TenVT from dmvt where InActive = '0' ";
            dt = db.GetDataTable(sql);
            lookUpEditVT.Properties.DataSource = dt;
            lookUpEditVT.Properties.DisplayMember = "TenVT";
            lookUpEditVT.Properties.ValueMember = "TenVT";
            if(dt.Rows.Count>0)
                lookUpEditVT.EditValue = dt.Rows[0]["TenVT"].ToString();
        }

        #region old code
        //private DataTable GetData(string mavt, decimal soluong, string makh)
        //{
        //    DataTable dt = new DataTable();
        //    string sqlText = " and BL.MaVT = '" + mavt + "'";
        //    if (makh != "")
        //        sqlText += " and BL.MaKH = '" + makh + "'";            
            //string sql = "select x.MaVT, VT.TenVT, VT.TenVT2, VT.PNo, x.Day, x.Rong, x.Dai, Sum(sln) - sum(slx) as Con,  " +
            //           " DV.TenDVT, VT.GiaBan as DonGia, x.Make , x.MaNgan, x.MaKho, x.GhiChuVT , x.MaKe + ' - ' + cast((convert(int,Sum(sln) - sum(slx))) as nvarchar) as ViTri, " +
            //           " cast(convert(int,x.Day) as nvarchar)+'x'+cast(convert(int,x.Rong) as nvarchar)+'x'+ cast(convert(int,x.Dai) as nvarchar) as KichThuoc  " +
            //           " into abc " +
            //           " from ( " +
            //           " select BL.MaVT, BL.Day, BL.Rong, BL.Dai, BL.SoLuong as sln, 0.0 as slx, 0.0 as DonGia, BL.MaKe, BL.MaNgan, BL.MaKho, BL.GhiChuVT " +
            //           " from BLVT BL  " +
            //           " where BL.MaCT ='PNK001' and BL.SoLuong > 0 " + sqlText +
            //           " union all " +
            //           " select BL.MaVT, BL.Day, BL.Rong, BL.Dai, 0.0 as sln, BL.SoLuong_x as slx, 0.0 as DonGia, BL.MaKe, BL.MaNgan, BL.MaKho, BL.GhiChuVT " +
            //           " from BLVT BL  " +
            //           " where BL.MaKho is not null and BL.MaNgan is not null  " +
            //           " and BL.MaKe is not null and BL.SoLuong_x > 0 " + sqlText +
            //           " ) x inner join DMVT VT on x.MaVT=VT.MaVT " +
            //           " Left join DMDVT DV on DV.MaDVT=VT.MaDVT " +
            //           " group by x.MaVT, VT.TenVT, VT.TenVT2, VT.PNo, x.Day, x.Rong, x.Dai, DV.TenDVT, VT.GiaBan, " +
            //           " x.MaKe , x.MaNgan, x.MaKho, x.GhiChuVT " +
            //           " declare @tsl decimal(20,6) "+
            //           " declare @sltk decimal(20,6) "+
            //           " select @tsl=sum(con) from abc "+
            //           " set @sltk = "+soluong+
            //           " set @tsl = case when @tsl > @sltk then 0 else @sltk - @tsl end " +
            //           " select @tsl as Het,* from abc "+
            //           " drop table abc ";                        
        //    dt = db.GetDataTable(sql); 
        //    return dt;
        //}
        #endregion

        private DataTable GetData(string tenvt)
        {            
            DataTable dt = new DataTable();
            if (tenvt == "")
                tenvt = " ";
            string sql = @"select x.MaVT, x.TenVT, x.TenVT2, x.PNo, x.Day, x.Rong, x.Dai, SLTon as Con,  
                        x.TenDVT, VT.GiaBan as DonGia, VT.GiaOME, VT.GiaDTB, x.Make , x.MaNgan, x.MaKho, '' as GhiChuVT , x.MaKe + ' - ' + cast(convert(int,SLTon) as nvarchar) as ViTri, 
                        cast(convert(int,x.Day) as nvarchar)+'x'+cast(convert(int,x.Rong) as nvarchar)+'x'+ cast(convert(int,x.Dai) as nvarchar) as KichThuoc ,
                        x.Bac, x.nhom, 0.0 as Het 
                        from wTonKhoTheoBac x inner join DMVT VT on VT.MaVT = x.MaVT
                        where VT.TenVT like N'" + tenvt + @"' and SLTon > 0 
                        union all
                        select mavt, tenvt, tenvt2, pno, 0, 0, 0, 0, null, GiaBan, GiaOME, GiaDTB,null,null,null,null,null,null,null,null,null
                        from dmvt where tenvt like N'" + tenvt + @"'";
            dt = db.GetDataTable(sql);
            decimal tsl = 0;
            foreach (DataRow row in dt.Rows)
                tsl += decimal.Parse(row["Con"].ToString());
            decimal sltk = spinEdit1.EditValue != null ? decimal.Parse(spinEdit1.EditValue.ToString()) : 0;
            tsl = sltk - tsl;
            if (tsl > 0 && dt.Rows.Count > 0) // bi thieu            
                dt.Rows[0]["Het"] = tsl.ToString();            
            return dt;
        } 
    }
}