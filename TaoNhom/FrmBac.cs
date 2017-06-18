using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace TaoNhom
{
    public partial class FrmBac : DevExpress.XtraEditors.XtraForm
    {
        public DataTable dtNhom = new DataTable();
        public decimal Bac;
        public FrmBac()
        {
            InitializeComponent();
            dtNhom.Columns.Add("Nhom", typeof(int));
            dtNhom.Columns.Add("Bac", typeof(int));
            gcNhom.DataSource = dtNhom;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Bac = spinEdit1.Value;
            this.Close();
        }

        private void spinEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                simpleButton1_Click(simpleButton1, new EventArgs());
            }
        }

        private void gvNhom_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            gvNhom.SetRowCellValue(e.RowHandle, "Bac", 1);
        }
    }
}