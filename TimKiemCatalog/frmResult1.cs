using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTLib;
using System.Diagnostics;
using System.IO;

namespace TimKiemCatalog
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

        private void gcResult_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (gvResult.IsDataRow(gvResult.FocusedRowHandle))
            {
                //noi dung file
                object o = gvResult.GetFocusedRowCellValue("TenTL");
                if (o == null)
                    return;
                byte[] file = o as byte[];
                //ten file
                o = gvResult.GetFocusedRowCellValue("TenTLFile");
                if (o == null)
                    return;
                string fn = o.ToString();
                string[] s = fn.Split('.');
                string type = s.Length > 1 ? s[s.Length - 1] : "";
                if (type == "")
                {
                    XtraMessageBox.Show("Không xác định được loại file!", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                    return;
                }
                string f = Path.ChangeExtension(Path.GetTempFileName(), type);
                File.WriteAllBytes(f, file);
                File.SetAttributes(f, FileAttributes.ReadOnly);
                try
                {
                    Process.Start(f);
                }
                catch 
                {
                    XtraMessageBox.Show("Máy tính của bạn chưa hỗ trợ đọc file có định dạng *."+type,Config.GetValue("PackageName").ToString());
                }
            }
        }

       
    }
}