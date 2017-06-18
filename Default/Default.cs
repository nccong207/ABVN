using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Plugins;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
using CDTDatabase;
using System.Windows.Forms;

namespace Default
{
    public class Default : ICControl
    {
        private Database _db = Database.NewDataDatabase();
        private DataCustomFormControl _data;
        private InfoCustomControl _info = new InfoCustomControl(IDataType.SingleDt);
        SpinEdit seHanTT;

        #region ICControl Members

        public DataCustomFormControl Data
        {
            set { _data = value; }
        }

        InfoCustomControl ICControl.Info
        {
            get { return _info; }
        }

        public void AddEvent()
        {
            _data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(_data.BsMain, new EventArgs());

            seHanTT = (_data.FrmMain.Controls.Find("HanTT", true)[0] as SpinEdit);
            GridLookUpEdit gluDKTT = (_data.FrmMain.Controls.Find("DKTT", true)[0] as GridLookUpEdit);
            gluDKTT.EditValueChanged += new EventHandler(gluDKTT_EditValueChanged);
        }

        void gluDKTT_EditValueChanged(object sender, EventArgs e)
        {
            GridLookUpEdit gluDKTT = sender as GridLookUpEdit;
            if (gluDKTT.Properties.ReadOnly || gluDKTT.EditValue == null)
                return;
            DataTable dtDKTT = (gluDKTT.Properties.DataSource as BindingSource).DataSource as DataTable;
            DataRow[] drs = dtDKTT.Select("DKID = " + gluDKTT.EditValue.ToString());
            if (drs.Length > 0)
                seHanTT.EditValue = drs[0]["HanTT"];
        }

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataTable dsData = _data.BsMain.DataSource as DataTable;
            if (dsData == null)
                return;
            dsData.TableNewRow += new DataTableNewRowEventHandler(LoaiKH_TableNewRow);
            if (_data.BsMain.Current != null)
                LoaiKH_TableNewRow(dsData, new DataTableNewRowEventArgs((_data.BsMain.Current as DataRowView).Row));
        }

        void LoaiKH_TableNewRow(object sender, DataTableNewRowEventArgs e)
        { 
            if (!_data.DrTable.Table.Columns.Contains("ExtraSql") || 
                (e.Row.RowState != DataRowState.Added && e.Row.RowState != DataRowState.Detached))
                return;
            string s = _data.DrTable["ExtraSql"].ToString();
            string pre = "";
            if (s.ToUpper().Contains("ISKH"))
            {
                pre = "KH";
                e.Row["IsKH"] = true;
            }
            if (s.ToUpper().Contains("ISNCC"))
            {
                pre = "NCC";
                e.Row["IsNcc"] = true;
            }
            if (s.ToUpper().Contains("ISNV"))
            {
                pre = "NV";
                e.Row["IsNV"] = true;
            }
            if (pre == "")
                return;
            object o = _db.GetValue("select max(makh) from dmkh where len(makh) = len('" + pre + "') + 4 and makh like '" + pre + "%'");
            string ma;
            if (o == null || o.ToString() == "")
                ma = pre + "0001";
            else
            {
                string tmp = o.ToString().Replace(pre, "");
                int n = Int32.Parse(tmp) + 1;
                ma = pre + n.ToString().PadLeft(tmp.Length, '0');
            }
            e.Row["MaKH"] = ma;
        }

        #endregion
    }
}
