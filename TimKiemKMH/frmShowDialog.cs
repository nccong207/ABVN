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

namespace TimKiemKMH
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
                XtraMessageBox.Show("Vui lòng chọn ký mã hiệu!", Config.GetValue("PackageName").ToString());
                return;
            }           
            if (Config.GetValue("GroupName") != null && Config.GetValue("GroupName").ToString() == "PKD" && LookupKH.EditValue == null)
            {
                XtraMessageBox.Show("Chưa chọn khách hàng!", Config.GetValue("PackageName").ToString());
                return;
            }
            string mavt = lookUpEditVT.EditValue != null ? lookUpEditVT.EditValue.ToString() : "";
            string makh = LookupKH.EditValue != null ? LookupKH.EditValue.ToString() : "";
            string makt = lookUpEditKT.EditValue != null ? lookUpEditKT.EditValue.ToString() : "";

            string sql = "insert into NKTimKiem(UserName,Ngay,MaVT,SL,MaKH) values (@UserName, @Ngay, @MaVT, @SL, @MaKH)";
            string[] paraNames = new string[] { "@UserName", "@Ngay", "@MaVT", "@SL", "@MaKH" };
            object[] paraValues = new object[] { Config.GetValue("UserName"), DateTime.Today, lookUpEditVT.EditValue, 0, LookupKH.EditValue != null ? LookupKH.EditValue : DBNull.Value };
            db.UpdateDatabyPara(sql, paraNames, paraValues);


            DataTable dt = GetData(mavt, makt);
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
            //Vật tư - ký hiệu
            sql = "select * from dmvt where InActive = '0' ";
            dt = db.GetDataTable(sql);
            lookUpEditVT.Properties.DataSource = dt;
            lookUpEditVT.Properties.DisplayMember = "TenVT2"; 
            lookUpEditVT.Properties.ValueMember = "MaVT";
            if(dt.Rows.Count>0)
                lookUpEditVT.EditValue=dt.Rows[0]["MaVT"].ToString();
            //kích thước
            sql = @"select cast(convert(int,Day) as nvarchar)+'x'+cast(convert(int,Rong) as nvarchar)+'x'+ cast(convert(int,Dai) as nvarchar) as KichThuoc 
                    from blvt
                    where Dai <> 0 and Rong <> 0 and Day <> 0 ";                    
            if (lookUpEditVT.EditValue != null)
                sql += " and MaVT = '" +lookUpEditVT.EditValue.ToString()+"'";
            sql += " group by cast(convert(int,Day) as nvarchar)+'x'+cast(convert(int,Rong) as nvarchar)+'x'+ cast(convert(int,Dai) as nvarchar)";
            dt = db.GetDataTable(sql);
            lookUpEditKT.Properties.DataSource = dt;
            lookUpEditKT.Properties.DisplayMember = "KichThuoc";
            lookUpEditKT.Properties.ValueMember = "KichThuoc";
        }

        private DataTable GetData(string mavt, string makt)
        {
            decimal Day = 0, Rong = 0, Dai = 0;
            if (makt != "")
            {
                string[] arr = makt.Split('x');
                if (arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (i == 0 && arr[i] != "")
                            Day = decimal.Parse(arr[i]);
                        else if (i == 1 && arr[i] != "")
                            Rong = decimal.Parse(arr[i]);
                        else if (i == 2 && arr[i] != "")
                            Dai = decimal.Parse(arr[i]);
                    }
                }
            }
            DataTable dt = new DataTable();           
            string sql = @"select x.MaVT, x.TenVT, x.TenVT2, x.PNo, x.Day, x.Rong, x.Dai, SLTon as Con,  
                        x.TenDVT, VT.GiaBan as DonGia, VT.GiaOME, VT.GiaDTB, x.Make , x.MaNgan, x.MaKho, '' as GhiChuVT , x.MaKe + ' - ' + cast(convert(int,SLTon) as nvarchar) as ViTri, 
                        cast(convert(int,x.Day) as nvarchar)+'x'+cast(convert(int,x.Rong) as nvarchar)+'x'+ cast(convert(int,x.Dai) as nvarchar) as KichThuoc ,
                        x.Bac, x.nhom 
                        from wTonKhoTheoBac x inner join DMVT VT on VT.MaVT = x.MaVT
                        where VT.MaVT = '" + mavt + @"' and SLTon > 0 and x.Day >= '" + Day.ToString() + @"' and x.Rong >= '" + Rong.ToString() + @"' and x.Dai >= '" + Dai.ToString() + @"'
                        union all
                        select mavt, tenvt, tenvt2, pno, 0, 0, 0, 0, null, GiaBan, GiaOME, GiaDTB,null,null,null,null,null,null,null,null
                        from dmvt where mavt ='" + mavt + @"'";
            dt = db.GetDataTable(sql);
            return dt;
        }        


        private void lookUpEditVT_EditValueChanged(object sender, EventArgs e)
        {
            ListKT("0","0","0");
        }

        private void ListKT(string day, string rong, string dai)
        {
            if (rong == "" || dai == "")
                return;
            string cond = "";
            if (day != "")
                cond = " Day >= '" + day + "' and Rong >= '" + rong + "' and Dai >= '" + dai + "' ";
            else
                cond = " Rong >= '" + rong + "' and Dai >= '" + dai + "' ";
            string sql = @"select cast(convert(int,Day) as nvarchar) + 'x' + cast(convert(int,Rong) as nvarchar) 
                           + 'x' + cast(convert(int,Dai) as nvarchar) as KichThuoc 
                         from wTonKhoTheoBac where " + cond;
            if (lookUpEditVT.EditValue != null)
                sql += " and MaVT = '" + lookUpEditVT.EditValue.ToString() + "' ";
            sql += @" group by  cast(convert(int,Day) as nvarchar) + 'x' + cast(convert(int,Rong) as nvarchar) 
                           + 'x' + cast(convert(int,Dai) as nvarchar)";
            DataTable dt = db.GetDataTable(sql);
            lookUpEditKT.Properties.DataSource = dt;
            lookUpEditKT.Properties.DisplayMember = "KichThuoc";
            lookUpEditKT.Properties.ValueMember = "KichThuoc";
            if (dt.Rows.Count > 0)
                lookUpEditKT.EditValue = dt.Rows[0]["KichThuoc"];
        }
    }
}