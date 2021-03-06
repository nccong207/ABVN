using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.Data;

namespace VatIN
{
    public class VatIN : ICControl
    {
        private DataCustomFormControl _data;
        private InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
        #region ICControl Members

        public void AddEvent()
        {
            string table = _data.DrTableMaster["TableName"].ToString();
            string[] tmp = new string[] { "MT12", "MT16", "MT51"};
            List<string> lstTable = new List<string>();
            lstTable.AddRange(tmp);
            if (!lstTable.Contains(table))
                return;
            XtraTabControl tcMain = _data.FrmMain.Controls.Find("tcMain", true)[0] as XtraTabControl;
            tcMain.SelectedPageChanged += new TabPageChangedEventHandler(tcMain_SelectedPageChanged);
        }

        void tcMain_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            GridView gvDetail = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            XtraTabControl tcMain = sender as XtraTabControl;
            if (gvDetail.OptionsBehavior.Editable && tcMain.TabPages[1] == e.Page)
            {
                GridView gvVat = (_data.FrmMain.Controls.Find("VATIn", true)[0] as GridControl).MainView as GridView;
                gvVat.Columns["MaVT"].Visible = false;
                gvVat.Columns["SoLuong"].Visible = false;
                gvVat.Columns["DonGia"].Visible = false;
                gvVat.Columns["MaHD"].Visible = true;
                gvVat.Columns["KHMauHD"].Visible = true;
                if (gvDetail.DataRowCount == 0 || gvVat.DataRowCount == gvDetail.DataRowCount
                    || XtraMessageBox.Show("Bạn có muốn cập nhật thông tin từ chi tiết hạch toán vào thuế GTGT đầu vào không?", "Xác nhận", MessageBoxButtons.YesNo)
                    == DialogResult.No)
                    return;
                string table = _data.DrTableMaster["TableName"].ToString();
                if (table == "MT51")
                    gvVat.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gvVat_CellValueChanged);
                for (int i = 0; i < gvDetail.DataRowCount; i++)
                {
                    if (gvVat.DataRowCount <= i)
                    {
                        gvVat.AddNewRow();
                        gvVat.UpdateCurrentRow();
                    }
                    decimal ps = Decimal.Parse(gvDetail.GetRowCellValue(i, "Ps").ToString());
                    gvVat.SetRowCellValue(i, "DienGiai", gvDetail.GetRowCellValue(i, "DienGiaiCt"));
                    if (table == "MT12" || table == "MT16")
                        gvVat.SetRowCellValue(i, "TKDu", (_data.BsMain.Current as DataRowView)["TkCo"]);
                    else
                        gvVat.SetRowCellValue(i, "TKDu", gvDetail.GetRowCellValue(i, "TkCo"));
                    gvVat.SetRowCellValue(i, "TTien", ps);
                    gvVat.SetRowCellValue(i, "MaKH", gvDetail.GetRowCellValue(i, "MaKHCt"));
                    if (gvVat.Columns.ColumnByFieldName("MaBP") != null)
                        gvVat.SetRowCellValue(i, "MaBP", gvDetail.GetRowCellValue(i, "MaBP"));
                }
                gvVat.BestFitColumns();
            }
        }

        void gvVat_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName.ToUpper() == "THUESUAT")
            {
                GridView gvVat = sender as GridView;
                GridView gvDetail = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
                for (int i = 0; i < gvVat.DataRowCount && i < gvDetail.DataRowCount; i++)
                    gvDetail.SetRowCellValue(i, "TienThue", gvVat.GetRowCellValue(i, "Thue"));
            }
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
