using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using CDTLib;
using DevExpress.XtraEditors.Repository;
using System.Windows.Forms;
namespace TinhTrangDeNghiGH
{
    public class TinhTrangDeNghiGH:ICControl
    {
        #region ICControl Members
        private DataCustomFormControl data;
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        Database db = Database.NewDataDatabase();
        public void AddEvent()
        {
            if (data.BsMain.DataSource != null && data.BsMain != null)
            {
                data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
                BsMain_DataSourceChanged(data.BsMain, new EventArgs());
            }
            //them chuc nang Tach phu kien
            LayoutControl lcMain = data.FrmMain.Controls.Find("lcMain", true)[0] as LayoutControl;
            SimpleButton btnTachPK = new SimpleButton();
            btnTachPK.Name = "btnTachPK";   //phai co name cua control
            btnTachPK.Text = "Tách phụ kiện";
            LayoutControlItem lci = lcMain.AddItem("", btnTachPK);
            lci.Name = "cusTachPK"; //phai co name cua item, bat buoc phai co "cus" phai truoc
            btnTachPK.Click += new EventHandler(btnTachPK_Click);

        }

        void btnTachPK_Click(object sender, EventArgs e)
        {
            GridView gv = (data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            if (!gv.Editable)
            {
                XtraMessageBox.Show("Cần thực hiện chức năng này ở chế độ thêm/sửa dữ liệu", Config.GetValue("PackageName").ToString());
                return;
            }
            DataTable dtSO = (data.BsMain.DataSource as DataSet).Tables[1];
            DataTable dt = dtSO.Clone();
            RepositoryItemGridLookUpEdit ri = (gv.Columns["PKienID"].ColumnEdit as RepositoryItemGridLookUpEdit);
            DataTable dtPK = (ri.DataSource as BindingSource).DataSource as DataTable;
            object o = dtSO.Compute("max(DTSOID)", "");
            int maxId = (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o) + 1;
            for (int i = 0; i < gv.DataRowCount; i++)
            {
                DataRow dr = gv.GetDataRow(i);
                string mavtPK = dr["PKienID"].ToString();
                if (mavtPK == "")
                    continue;
                DataRow[] drs = dtPK.Select("MaVT = '" + mavtPK + "'");
                if (drs.Length == 0)
                    continue;
                DataRow drNew = dt.NewRow();
                drNew["DTSOID"] = maxId + i;
                drNew["Code"] = drs[0]["PNo"];
                drNew["ChieuDay"] = dr["DaiPK"];
                drNew["ChieuRong"] = dr["RongPK"];
                drNew["ChieuCao"] = dr["CaoPK"];
                drNew["SoLuong"] = dr["SLPK"];
                dt.Rows.Add(drNew);
            }
            foreach (DataRow dr in dt.Rows)
            {
                gv.AddNewRow();
                gv.UpdateCurrentRow();
                gv.SetFocusedRowCellValue(gv.Columns["DTSOID"], dr["DTSOID"]);
                gv.SetFocusedRowCellValue(gv.Columns["Code"], dr["Code"]);
                gv.SetFocusedRowCellValue(gv.Columns["ChieuDay"], dr["ChieuDay"]);
                gv.SetFocusedRowCellValue(gv.Columns["ChieuRong"], dr["ChieuRong"]);
                gv.SetFocusedRowCellValue(gv.Columns["ChieuCao"], dr["ChieuCao"]);
                gv.SetFocusedRowCellValue(gv.Columns["SoLuong"], dr["SoLuong"]);
                gv.SetFocusedRowCellValue(gv.Columns["GhiChu"], "Phụ kiện");
            }
        }

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataSet ds = data.BsMain.DataSource as DataSet;
            if (ds == null)
                return;
            ds.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(TinhTrangDeNghiGH_ColumnChanged);
        }

        void TinhTrangDeNghiGH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Deleted)
                return;
            if (e.Column.ColumnName.ToUpper().Equals("SOPHIEU"))
            {
                if (e.Row["SOPHIEU"].ToString() != "")
                {
                    e.Row["TinhTrang"] = 4;
                    e.Row.EndEdit();
                }
                else if (e.Row["SOPHIEU"].ToString() == "")
                {
                    e.Row["TinhTrang"] = 3;
                    e.Row.EndEdit();
                }

            }
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
