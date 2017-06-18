using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using CDTLib;
using Plugins;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
using System.Data;
using System.Windows.Forms;

namespace XepHangLenKe
{
    public class XepHangLenKe : ICControl
    {
        InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        DataCustomFormControl data;

        GridControl gcMain;
        GridView gvMain;
        RepositoryItemGridLookUpEdit riCode;

        #region ICControl Members

        public void AddEvent()
        {
            gcMain = data.FrmMain.Controls.Find("gcMain",true)[0] as GridControl ;
            gvMain = gcMain.MainView as GridView;
            riCode = gcMain.RepositoryItems["Code"] as RepositoryItemGridLookUpEdit;
            riCode.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(riCode_CloseUp);
        }

        void riCode_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            if (e.CloseMode != DevExpress.XtraEditors.PopupCloseMode.Normal)
                return;
            GridLookUpEdit glu = sender as GridLookUpEdit;
            if (glu.Properties.View.FocusedRowHandle < 0)
                return;
            DataRowView drv = glu.Properties.View.GetRow(glu.Properties.View.FocusedRowHandle) as DataRowView;
            string code = drv["Pno"].ToString() ;
            string nhom = drv["Nhom"].ToString();
            string bac = drv["Bac"].ToString();
            if (nhom == "")
                return;
            DataTable dt = (glu.Properties.DataSource as BindingSource).DataSource as DataTable;
            DataRow[] drs = dt.Select("Pno = '" + code + "' and Nhom = '" + nhom + "'");// and Bac <> '" +bac+"'
            if (drs.Length == 0)
                return;
            gvMain.SetFocusedRowCellValue(gvMain.Columns["Code"],code);
            gvMain.UpdateCurrentRow();
            gvMain.ActiveFilterString = "Pno = '" + code + "' and Nhom = '" + nhom + "'";
            for (int i = 0; i < drs.Length; i++)
            {
                DataRow dr = drs[i];
                if (gvMain.DataRowCount > 0 && dr["Bac"].ToString() == bac)
                    continue;
                gvMain.AddNewRow();
                gvMain.SetFocusedRowCellValue(gvMain.Columns["Code"], dr["Pno"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["NgayNhap"], dr["NgayCT"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["Nhom"], dr["Nhom"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["SLTon"], dr["SLTon"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["SLCuonTon"], dr["SLCuonTon"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["Rong"], dr["Rong"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["Dai"], dr["Dai"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["Day"], dr["Day"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["MaKeCu"], dr["MaKe"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["MaNganCu"], dr["MaNgan"]);
                gvMain.UpdateCurrentRow();
            }
            gvMain.ActiveFilterString = "";
            gvMain.BestFitColumns();
            }
        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        #endregion
    }
}
