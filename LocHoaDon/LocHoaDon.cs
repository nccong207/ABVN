using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using Plugins;
using CDTDatabase;

namespace LocHoaDon
{
    public class LocHoaDon:ICForm
    {
        private List<InfoCustomForm> _lstInfo = new List<InfoCustomForm>();
        private DataCustomFormControl _data;        

        #region ICForm Members
       
         public LocHoaDon()
        {
            InfoCustomForm info = new InfoCustomForm(IDataType.MasterDetailDt, 1008, "Lấy công nợ khách hàng",
                "", "MT34");
            _lstInfo.Add(info);            
        }

        public DataCustomFormControl Data
        {
            set { _data=value; }
        }

        public void Execute(int menuID)
        {
            if (menuID == _lstInfo[0].MenuID)
            {
                frmMain frm = new frmMain();
                frm.Text = _lstInfo[0].MenuName.ToString();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog();
                if (_data.BsMain.DataSource == null)
                    return;
                DataRow drMaster = (_data.BsMain.Current as DataRowView).Row;
                if (drMaster == null)
                    return;
                if (drMaster.RowState != DataRowState.Added)
                    return;
                GridView gvDetail = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
                if (frm.dtSource.Rows.Count > 0)
                {
                    drMaster["NgayCT"] = frm.NgayCT;
                    if (gvDetail.DataRowCount > 0)                    
                        XoaGrid(gvDetail);                       
                    foreach (DataRow row in frm.dtSource.Rows)
                    {
                        if (!bool.Parse(row["Chon"].ToString()))
                            continue;
                        gvDetail.AddNewRow();
                        gvDetail.UpdateCurrentRow();
                        gvDetail.SetFocusedRowCellValue(gvDetail.Columns["MaKhDt"], row["MaKh"]);
                        gvDetail.SetFocusedRowCellValue(gvDetail.Columns["MT31ID"], row["MTID"]);                                               
                    }
                    gvDetail.BestFitColumns();
                }                
            }            
        }

        private void XoaGrid(GridView gv)
        {
            while (gv.DataRowCount > 0)
                gv.DeleteRow(0);
        }

        public List<InfoCustomForm> LstInfo
        {
            get { return _lstInfo; }
        }

        #endregion
    }
}
