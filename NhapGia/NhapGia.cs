using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraBars;
using CDTDatabase;
using Plugins;

namespace NhapGia
{
    public class NhapGia : ICControl
    {
        private DataCustomFormControl _data;
        private InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);

        #region ICControl Members

        public DataCustomFormControl Data
        {
            set { _data = value; }
        }

        public InfoCustomControl Info
        {
            get { return _info; }
        }

        public void AddEvent()
        {
            //string table = _data.DrTableMaster["TableName"].ToString();
            //if (table == "MT33" || table == "MT42")
            //{
            //    CheckEdit ce = _data.FrmMain.Controls.Find("NhapTB", true)[0] as CheckEdit;
            //    if (ce == null)
            //        return;
            //    ce.EditValueChanged += new EventHandler(be_EditValueChanged);
            //}
            //else
            //    if (table == "MT43" || table == "MT44" || table == "MT45")
            //    {
            //        GridView gvMain = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            //        if (gvMain.Columns["MaVT"] != null && gvMain.Columns["MaVT"].ColumnEdit != null && gvMain.Columns["MaVT"].ColumnEdit.GetType() == typeof(RepositoryItemGridLookUpEdit))
            //        {
            //            gvMain.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(View_CellValueChanged);
            //        }
            //    }
        }
        #endregion


        void be_EditValueChanged(object sender, EventArgs e)
        {
            if ((sender as BaseEdit).EditValue == null || (sender as BaseEdit).EditValue.ToString() == "")
                return;
            string table = _data.DrTableMaster["TableName"].ToString();
            GridView gvMain = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            bool nhapTB = Boolean.Parse((sender as BaseEdit).EditValue.ToString());
            if (table == "MT33") 
            {
                if (gvMain.Columns["GiaVon"] != null)
                    gvMain.Columns["GiaVon"].OptionsColumn.AllowEdit = !nhapTB;
                if (gvMain.Columns["TienVon"] != null)
                    gvMain.Columns["TienVon"].OptionsColumn.AllowEdit = !nhapTB;
            }
            else
            {
                if (gvMain.Columns["Gia"] != null)
                    gvMain.Columns["Gia"].OptionsColumn.AllowEdit = !nhapTB;
                if (gvMain.Columns["GiaNT"] != null)
                    gvMain.Columns["GiaNT"].OptionsColumn.AllowEdit = !nhapTB;
                if (gvMain.Columns["Ps"] != null)
                    gvMain.Columns["Ps"].OptionsColumn.AllowEdit = !nhapTB;
                if (gvMain.Columns["PsNT"] != null)
                    gvMain.Columns["PsNT"].OptionsColumn.AllowEdit = !nhapTB;
            }
        }

        void View_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName.ToUpper() != "MAVT" || e.Value == null)
                return;
            RepositoryItemGridLookUpEdit riGlu = e.Column.ColumnEdit as RepositoryItemGridLookUpEdit;
            int i = riGlu.GetIndexByKeyValue(e.Value);
            if (i < 0)
                return;
            if (riGlu.View.Columns["Tonkho"] == null)
                return;
            GridView gvMain = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            int tonKho = Int32.Parse(riGlu.View.GetRowCellValue(i, riGlu.View.Columns["Tonkho"]).ToString());
            if (gvMain.Columns["Gia"] != null)
                gvMain.Columns["Gia"].OptionsColumn.AllowEdit = (tonKho == 5);
            if (gvMain.Columns["GiaNT"] != null)
                gvMain.Columns["GiaNT"].OptionsColumn.AllowEdit = (tonKho == 5);
            if (gvMain.Columns["Ps"] != null)
                gvMain.Columns["Ps"].OptionsColumn.AllowEdit = (tonKho == 5);
            if (gvMain.Columns["PsNT"] != null)
                gvMain.Columns["PsNT"].OptionsColumn.AllowEdit = (tonKho == 5);
            if (gvMain.Columns["GiaVon"] != null)
                gvMain.Columns["GiaVon"].OptionsColumn.AllowEdit = (tonKho == 5);
            if (gvMain.Columns["TienVon"] != null)
                gvMain.Columns["TienVon"].OptionsColumn.AllowEdit = (tonKho == 5);
        }
    }
}
