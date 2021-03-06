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

namespace TaoKe
{
    public partial class frmKe : DevExpress.XtraEditors.XtraForm
    {
        public string makho = "";
        public int soke = 0;
        public frmKe()
        {                        
            InitializeComponent();
            
        }

        Database db = Database.NewDataDatabase();

        private void frmKe_Load(object sender, EventArgs e)
        {
            string sql = "";
            sql = string.Format("select * from dmke where {0} {1}", "makho = '" + makho + "'", soke == 0 ? "" : "and make > '" + soke.ToString() + "'");
            DataTable dt = db.GetDataTable(sql);
            gcKe.DataSource = dt;            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string sql = "";
            DataTable dt = gcKe.DataSource as DataTable;
            if (dt.Rows.Count == 0)
                return;
            foreach (DataRow row in dt.Rows)
            {
                if (row["SoNgan"].ToString().Equals(""))
                    continue;
                int songan = int.Parse(row["SoNgan"].ToString());
                for (int i = 1; i <= songan; i++)
                {
                    sql = "insert into dmngan (mangan,tenngan,make, makho) values ('" + i.ToString() + "','" + i.ToString() + "','" + row["MaKe"].ToString() + "','"+row["Makho"].ToString()+"')";
                    db.UpdateByNonQuery(sql);
                }
            }
            XtraMessageBox.Show("Tạo ngăn thành công!",Config.GetValue("PackageName").ToString());
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}