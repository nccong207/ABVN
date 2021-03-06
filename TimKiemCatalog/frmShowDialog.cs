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

namespace TimKiemCatalog
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
            if (gridLookUpVT.EditValue == null)
            {
                XtraMessageBox.Show("Chọn hàng hóa để tìm kiếm.",Config.GetValue("PackageName").ToString());
                return;
            }
            DataTable dt = GetData(gridLookUpVT.EditValue.ToString());
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
            //Hàng hóa
            string sql = "select * from dmvt where InActive = '0' ";
            DataTable dt = db.GetDataTable(sql);
            gridLookUpVT.Properties.DataSource = dt;
            gridLookUpVT.Properties.DisplayMember = "TenVT";
            gridLookUpVT.Properties.ValueMember = "MaVT";
            if (dt.Rows.Count > 0)
                gridLookUpVT.EditValue = dt.Rows[0]["MaVT"].ToString();
        }

        private DataTable GetData(string mavt)
        {            
            DataTable dt = new DataTable();
            string sql = @"Select VT.MaVT, VT.TenVT, VT.TenVT2 , VT.PNo , C.LTCAID,
                           C.LoaiFile, C.GhiChu, C.TenTLFile, C.TenTL
                           From DMVT VT inner join LuuTruCatalog C on VT.MaVT = C.Code
                           where C.Code = '" + mavt + @"' and C.LoaiTL = 'Catalog'";
            dt = db.GetDataTable(sql);
            return dt;
        }
    }
}