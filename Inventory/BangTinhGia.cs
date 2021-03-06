using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraEditors;
using System.Globalization;
namespace Inventory
{
    public partial class FormHTK : DevExpress.XtraEditors.XtraForm
    {
        public string makho;
        public FormHTK()
        {
            InitializeComponent();
        }

        private bool KhoaSo(int newMonth)
        {
            if (Config.GetValue("NgayKhoaSo") == null)
                return false;
            if (Config.GetValue("NamLamViec") == null)
                return false;
            string tmp = Config.GetValue("NgayKhoaSo").ToString();
            int nam = Int32.Parse(Config.GetValue("NamLamViec").ToString());
            DateTime ngayKhoa;
            DateTimeFormatInfo dtInfo = new DateTimeFormatInfo();
            dtInfo.ShortDatePattern = "dd/MM/yyyy";
            if (DateTime.TryParse(tmp, dtInfo, DateTimeStyles.None, out ngayKhoa))
            {
                if (nam == ngayKhoa.Year && newMonth <= ngayKhoa.Month)
                {
                    string msg = "Kỳ kế toán đã khóa! Không thể chỉnh sửa số liệu!";
                    if (Config.GetValue("Language").ToString() == "1")
                        msg = UIDictionary.Translate(msg);
                    XtraMessageBox.Show(msg);
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        private void simpleButtonTinhGia_Click(object sender, EventArgs e)
        {
            int tuthang = Int32.Parse(spinEditTuThang.EditValue.ToString());
            int denthang = Int32.Parse(spinEditDenThang.EditValue.ToString());
            if (KhoaSo(denthang))
                return;
            if (grKhoApGia.EditValue == null || grKhoApGia.EditValue.ToString() == "")
            {
                XtraMessageBox.Show("Vui lòng chọn kho để tính giá xuất!",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                return;
            }
            string s = GenCondition();
            DevExpress.XtraEditors.XtraForm f = new GiaVT(tuthang,denthang, radioGroupPP.SelectedIndex,grKhoApGia.EditValue.ToString(),s,grTKCL.EditValue.ToString());
            f.ShowDialog();
        } 

        private string GenCondition()
        {
            string tmp = "";
            if (grNhomVT.EditValue != null && grNhomVT.EditValue.ToString() != "")
                tmp += (tmp == "" ? "" : " and ") + " Nhom = '" + grNhomVT.EditValue.ToString() + "'";
            if (grTkKho.EditValue != null && grTkKho.EditValue.ToString() != "")
                tmp += (tmp == "" ? "" : " and ") + " TkKho = '" + grTkKho.EditValue.ToString() + "'";
            if (grMaVT.EditValue != null && grMaVT.EditValue.ToString() != "")
                tmp += (tmp == "" ? "" : " and ") + " MaVT = '" + grMaVT.EditValue.ToString() + "'";
            if (tmp != "")
                tmp = "MaVT in (select MaVT from DMVT where " + tmp + ")";
            return tmp;
        }

        private void FormHTK_Load(object sender, EventArgs e)
        {
            Database db = Database.NewDataDatabase();
            DataTable dt = db.GetDataTable("select MaKho,TenKho from dmkho");
            grKhoApGia.Properties.DataSource = dt;
            grKhoApGia.Properties.ValueMember = "MaKho";
            grKhoApGia.Properties.DisplayMember = "MaKho";
            if (dt.Rows.Count > 0)
                grKhoApGia.EditValue = dt.Rows[0]["MaKho"];
            grNhomVT.Properties.DataSource = db.GetDataTable("select MaNhomVT, TenNhom, MaNhomVTMe from dmNhomVT");
            grTkKho.Properties.DataSource = db.GetDataTable("select TkKho, TenTk from dmvt a, dmtk b where a.TonKho <> 5 and a.tkkho = b.tk group by a.TkKho, b.TenTk order by a.tkkho");
            grMaVT.Properties.DataSource = db.GetDataTable("select MaVT, PNo, TenVT2 from DMVT where Duyet = 1 and Tonkho <> 5");
            grMaVT.Properties.ValueMember = "MaVT";
            grMaVT.Properties.DisplayMember = "PNo";
            DataTable dtTkHt = db.GetDataTable("select Tk, TenTk from DMTK where TK not in (select  TK=case when TKMe is null then '' else TKMe end from DMTK group by TKMe)");
            dtTkHt.DefaultView.RowFilter = "Tk like '632%'";
            string tkCL = dtTkHt.DefaultView.Count > 0 ? dtTkHt.DefaultView[0]["Tk"].ToString() : "";
            dtTkHt.DefaultView.RowFilter = "";
            grTKCL.Properties.DataSource = dtTkHt;
            grTKCL.EditValue = tkCL;
            grKhoApGia.Properties.View.BestFitColumns();
            grNhomVT.Properties.View.BestFitColumns();
            grTkKho.Properties.View.BestFitColumns();
            grMaVT.Properties.View.BestFitColumns();
            grTKCL.Properties.View.BestFitColumns();
            spinEditTuThang.Value = Config.GetValue("KyKeToan") == null ? DateTime.Today.Month : Int32.Parse(Config.GetValue("KyKeToan").ToString());
            spinEditDenThang.Value = Config.GetValue("KyKeToan") == null ? DateTime.Today.Month : Int32.Parse(Config.GetValue("KyKeToan").ToString());
            if (Config.GetValue("Language").ToString() == "1")
                FormFactory.DevLocalizer.Translate(this);
        }

        private void FormHTK_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.F12)
            {
                simpleButtonTinhGia_Click(simpleButtonTinhGia as object , new EventArgs());
            }
        }

        private void gridLookUp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                (sender as GridLookUpEdit).EditValue = null;
        }

    }
}