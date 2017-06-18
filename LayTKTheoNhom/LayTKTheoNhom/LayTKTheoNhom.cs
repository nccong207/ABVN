using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Plugins;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace LayTKTheoNhom
{
    public class LayTKTheoNhom : ICControl
    {
        private DataCustomFormControl _data;
        private InfoCustomControl _info = new InfoCustomControl(IDataType.SingleDt);
        #region ICControl Members

        public void AddEvent()
        {
            GridLookUpEdit gluNhom = (_data.FrmMain.Controls.Find("Nhom", true)[0] as GridLookUpEdit);
            gluNhom.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(gluNhom_CloseUp);
        }

        void gluNhom_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Cancel || e.Value == null || e.Value == DBNull.Value)
                return;
            GridLookUpEdit gluNhom = sender as GridLookUpEdit;
            DataTable dtData = (gluNhom.Properties.DataSource as BindingSource).DataSource as DataTable;
            DataRow[] drs = dtData.Select(string.Format("MaNhomVT = '{0}'", e.Value));
            if (drs.Length == 0)
                return;
            DataRow drMaster = (_data.BsMain.Current as DataRowView).Row;
            if (drMaster == null)
                return;
            drMaster["TKKho"] = drs[0]["TKKho"];
            drMaster["TKGV"] = drs[0]["TKGV"];
            drMaster["TKDT"] = drs[0]["TKDT"];
        }

        public DataCustomFormControl Data
        {
            set { _data = value; }
        }

        public InfoCustomControl Info
        {
            get { return _info; }
        }

        #endregion
    }
}
