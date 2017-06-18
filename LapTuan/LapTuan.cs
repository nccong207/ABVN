using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using System.Globalization;
using DevExpress.XtraEditors;
using CDTLib;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;

namespace LapTuan
{
    public class LapTuan : ICControl
    {
        private DataCustomFormControl data;
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        GridView gvMain;
        SpinEdit seTuan;

        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        public void AddEvent()
        {
            if (data.BsMain == null || data.BsMain.DataSource == null || data.BsMain.Current == null)
                return;
            gvMain = (data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            DataRow dr = (data.BsMain.Current as DataRowView).Row;
            if (dr.RowState == DataRowState.Unchanged && dr["Tuan"] != DBNull.Value)
                gvMain.ActiveFilterString = "TuanDT = " + dr["Tuan"].ToString();
            gvMain.InitNewRow += new InitNewRowEventHandler(gvMain_InitNewRow);
            seTuan = (data.FrmMain.Controls.Find("Tuan", true)[0] as SpinEdit);
            seTuan.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(seTuan_EditValueChanging);
            if (dr.RowState == DataRowState.Added)
                seTuan.EditValue = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            CheckEdit ceNam = (data.FrmMain.Controls.Find("CaNam", true))[0] as CheckEdit;
            ceNam.CheckedChanged += new EventHandler(ceNam_CheckedChanged);
        }

        void ceNam_CheckedChanged(object sender, EventArgs e)
        {
            if (data.BsMain == null || data.BsMain.Current == null)
                return;
            CheckEdit ce = sender as CheckEdit;
            if (ce.Checked)
            {
                string year = Config.GetValue("NamLamViec").ToString();
                DateTime dt = new DateTime(Int32.Parse(year), 1, 1);
                DataRow dr = (data.BsMain.Current as DataRowView).Row;
                dr["CaNam"] = ce.Checked;
                dr["Tuan"] = DBNull.Value;
                dr["TuNgay"] = dt;
                dr["DenNgay"] = dt.AddYears(1).AddDays(-1);
                gvMain.ActiveFilterString = "";
            }
            else
            {
                seTuan.EditValue = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            }
        }

        void gvMain_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            if (data.BsMain == null || data.BsMain.DataSource == null || data.BsMain.Current == null)
                return;
            DataRow dr = (data.BsMain.Current as DataRowView).Row;
            if (dr["Tuan"] == DBNull.Value)
                dr["Tuan"] = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["TuanDT"], dr["Tuan"]);
        }

        void seTuan_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (data.BsMain == null || data.BsMain.Current == null || e.NewValue == null)
                return;
            string s = e.NewValue.ToString();
            string w = s.EndsWith(".") || s.EndsWith(",") ? s.Substring(0, s.Length - 1) : s;
            if (Int32.Parse(w) > 52 || Int32.Parse(w) < 1)
            {
                e.Cancel = true;
                return;
            }
            DataRow dr = (data.BsMain.Current as DataRowView).Row;
            dr["CaNam"] = false;
            gvMain.ActiveFilterString = "TuanDT = " + e.NewValue.ToString();
            if (dr["Tuan"].ToString() == w)
                return;
            string year = Config.GetValue("NamLamViec").ToString();
            DateTime dt = new DateTime(Int32.Parse(year), 1, 1).AddDays(Double.Parse(w) * 7);
            DateTime dts = dt;
            while (dts.DayOfWeek != DayOfWeek.Monday)
                dts = dts.AddDays(-1);
            DateTime dte = dts.AddDays(5);
            dr["Tuan"] = w;
            dr["TuNgay"] = dts;
            dr["DenNgay"] = dte;
            gvMain.ActiveFilterString = "TuanDT = " + e.NewValue.ToString();
        }
    }
}
