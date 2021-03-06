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
using DevExpress.XtraScheduler.Printing;

namespace LichLamViec
{
    public partial class FrmQLLich : DevExpress.XtraEditors.XtraForm
    {
        DataTable dtCongViec;
        public FrmQLLich()
        {
            InitializeComponent();
        }

        private void cbeView_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbeView.SelectedIndex)
            {
                case 0:
                    this.scMain.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.Day;
                    break;
                case 1:
                    this.scMain.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.Week;
                    break;
                case 2:
                    this.scMain.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.Month;
                    break;
                case 3:
                    this.scMain.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.WorkWeek;
                    break;
                case 4:
                    this.scMain.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.Timeline;
                    break;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            switch (cbePrintStyle.SelectedIndex)
            {
                case 0:
                    scMain.OptionsPrint.PrintStyle = SchedulerPrintStyleKind.Daily;
                    break;
                case 1:
                    scMain.OptionsPrint.PrintStyle = SchedulerPrintStyleKind.Weekly;
                    break;
                case 2:
                    scMain.OptionsPrint.PrintStyle = SchedulerPrintStyleKind.Monthly;
                    break;
                case 3:
                    scMain.OptionsPrint.PrintStyle = SchedulerPrintStyleKind.TriFold;
                    break;
                case 4:
                    scMain.OptionsPrint.PrintStyle = SchedulerPrintStyleKind.CalendarDetails;
                    break;
                case 5:
                    scMain.OptionsPrint.PrintStyle = SchedulerPrintStyleKind.Memo;
                    if (scMain.SelectedAppointments.Count == 0)
                    {
                        XtraMessageBox.Show("Vui lòng chọn một hoặc nhiều việc trên lịch để in!", Config.GetValue("PackageName").ToString());
                        return;
                    }
                    break;
            }
            scMain.ShowPrintPreview();
        }

        private void LayDuLieu()
        {
            string un = Config.GetValue("UserName").ToString();
            string admin = Config.GetValue("Admin").ToString();
            string sql = "select cv.*, nv.TenNV from LichLamViec cv " +
                "inner join DMNhanVien nv on cv.Resource = nv.MaNV " +
                "where ('{1}' = 'True' or '{0}' in ('minhhai','tuanngoc')) or cv.Resource = '{0}'";
            sql = String.Format(sql, un, admin);
            Database db = Database.NewDataDatabase();
            dtCongViec = db.GetDataTable(sql);
            //dtTongHop = dtCongViec;
            //dvTongHop = new DataView(dtTongHop);
           
            this.ssMain.Appointments.DataSource = dtCongViec;
            this.ssMain.Appointments.Mappings.Subject = "Subject";
            this.ssMain.Appointments.Mappings.Label = "Label";
            this.ssMain.Appointments.Mappings.End = "Finish";
            this.ssMain.Appointments.Mappings.Location = "Location";
            this.ssMain.Appointments.Mappings.ResourceId = "Resource";
            this.ssMain.Appointments.Mappings.Start = "Start";
            this.ssMain.Appointments.Mappings.Description = "Content";
            DataTable dtNV = db.GetDataTable("select nv.MaNV, nv.TenNV,cv.TenCV from DMNhanVien nv left join DMChucVu cv on nv.ChucVu = cv.CVID" +
                " where ('" + admin + "' = 'True' or '" + un + "' in ('minhhai','tuanngoc')) or nv.MaNV = '" + un + "'"); 
            this.ssMain.Resources.DataSource = dtNV;
            this.ssMain.Resources.Mappings.Id = "MaNV";
            this.ssMain.Resources.Mappings.Caption = "TenNV";
            scMain.Start = DateTime.Today;

            //Người thực hiện
            LookupNhanVien.Properties.DataSource = dtNV;
            LookupNhanVien.Properties.DisplayMember = "TenNV";
            LookupNhanVien.Properties.ValueMember = "MaNV";
            LookupNhanVien.Properties.BestFit();
            if (dtNV.Rows.Count == 1)
                LookupNhanVien.EditValue = dtNV.Rows[0]["MaNV"].ToString();
        }

        private void LookupNhanVien_EditValueChanged(object sender, EventArgs e)
        {
            bool admin = Boolean.Parse(Config.GetValue("Admin").ToString());
            if (admin && (LookupNhanVien.EditValue == null || LookupNhanVien.EditValue.ToString() == ""))
                dtCongViec.DefaultView.RowFilter = "";
            if (!admin && (LookupNhanVien.EditValue == null || LookupNhanVien.EditValue.ToString() == ""))
                dtCongViec.DefaultView.RowFilter = "1 = 0";
            if (LookupNhanVien.EditValue != null && LookupNhanVien.EditValue.ToString() != "")
                dtCongViec.DefaultView.RowFilter = "Resource = '" + LookupNhanVien.EditValue.ToString() + "'";
        }

        private void scMain_InitNewAppointment(object sender, DevExpress.XtraScheduler.AppointmentEventArgs e)
        {
            if (e.Appointment.Start.Hour == 0)
                e.Appointment.Start = e.Appointment.Start.AddHours(9);
            e.Appointment.Duration = new TimeSpan(3, 0, 0);
            e.Appointment.StatusId = 0;
            if (ssMain.Resources.Items.Count > 0)
                e.Appointment.ResourceId = ssMain.Resources.Items[0].Id;
        }

        private bool AddData(DataRow dr)
        {
            string s = "insert into LichLamViec(Subject, Content, Location, Start, Finish, Label, Resource) values(N'{0}',N'{1}',N'{2}','{3}','{4}',{5},'{6}')";
            Database db = Database.NewDataDatabase();
            return (db.UpdateByNonQuery(String.Format(s, dr["Subject"], dr["Content"], dr["Location"], dr["Start"], dr["Finish"], dr["Label"], dr["Resource"])));
        }

        private bool DelData(DataRow dr)
        {
            string id = dr["ID", DataRowVersion.Original].ToString();
            string s = "delete from LichLamViec where ID = " + id;
            Database db = Database.NewDataDatabase();
            return (db.UpdateByNonQuery(s));
        }

        private bool UpdateData(DataRow dr)
        {
            string s = "update LichLamViec set Subject = N'{0}', Content = N'{1}', Location = N'{2}', Start = '{3}', Finish = '{4}', Label = {5}, Resource = '{6}' where ID = {7}";
            Database db = Database.NewDataDatabase();
            return (db.UpdateByNonQuery(String.Format(s, dr["Subject"], dr["Content"], dr["Location"], dr["Start"], dr["Finish"], dr["Label"], dr["Resource"], dr["ID"])));
        }

        private bool SaveData()
        {
            bool r = true;
            DataView dv = new DataView(dtCongViec);
            dv.RowStateFilter = DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent;
            foreach (DataRowView drv in dv)
            {
                switch (drv.Row.RowState)
                {
                    case DataRowState.Added:
                        r = AddData(drv.Row);
                        break;
                    case DataRowState.Deleted:
                        r = DelData(drv.Row);
                        break;
                    case DataRowState.Modified:
                        r = UpdateData(drv.Row);
                        break;
                }
                if (r == false)
                    break;
            }
            return r;
        }

        private void FrmQLLich_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!SaveData())
                XtraMessageBox.Show("Chưa cập nhật lịch làm việc thành công!", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
        }

        private void FrmQLLich_Load(object sender, EventArgs e)
        {
            LayDuLieu();
        }

        private void LookupNhanVien_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                LookupNhanVien.EditValue = null;
        }

        private void scMain_InitAppointmentDisplayText(object sender, DevExpress.XtraScheduler.AppointmentDisplayTextEventArgs e)
        {
            if (e.Appointment.ResourceId != null)
            {
                e.Text = e.Appointment.ResourceId.ToString() + ": " + e.Appointment.Subject;
            }
        }
    }
}