using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTLib;

namespace TimKiemCode
{
    public partial class frmResult1 : DevExpress.XtraEditors.XtraForm
    {
        DataTable dt = new DataTable();
        public bool hasData = true;
        public frmResult1(DataTable dtSource)
        {
            InitializeComponent();
            BindData(dtSource);
        }

        void BindData(DataTable dtSouce)
        {
            if (dtSouce.Rows.Count == 0)
            {
                XtraMessageBox.Show("Không tìm thấy dữ liệu liên quan!", Config.GetValue("PackageName").ToString());
                hasData = false;
            }
            gcResult.DataSource = dtSouce;
            gvResult.BestFitColumns();
        }

        private void gcResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

       
    }
}