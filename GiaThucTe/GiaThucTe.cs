using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using CDTLib;
using CDTDatabase;
using Plugins;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using CBSControls;
using System.Data;
using DevExpress.XtraLayout.Utils;
using FormFactory;

namespace GiaThucTe
{
    //Tạo mã học viên, tính học phí, nguồn học viên, giáo trình, quà tặng
    public class GiaThucTe:ICControl
    {        
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();        

        #region ICControl Members

        public void AddEvent()
        {            
            data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(data.BsMain, new EventArgs());                                            
        }            
             
        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataSet ds = data.BsMain.DataSource as DataSet;
            if (ds == null)
                return;
            ds.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(GiaThucTe_ColumnChanged);
        }

        void GiaThucTe_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Deleted)
                return;
            if (e.Column.ColumnName.ToUpper().Equals("MAVT") || e.Column.ColumnName.ToUpper().Equals("GIA"))
            {               
                if (!e.Row.Table.Columns.Contains("GiaBanTT"))
                    return;
                if (e.Row["MaVT"].ToString() == "")
                    return;
                string sql = "select sum(soluong) as tsl, sum(ps) tps from dt23 "+
                    "where MaVT = '"+e.Row["MaVT"].ToString()+"' group by MaVT";
                DataTable dt = db.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["tsl"].ToString() != "" && dt.Rows[0]["tps"].ToString() != "")
                    {
                        e.Row["GiaBanTT"] = decimal.Parse(dt.Rows[0]["tps"].ToString()) / decimal.Parse(dt.Rows[0]["tsl"].ToString());
                        e.Row.EndEdit();
                    }
                }
                else if (e.Column.ColumnName.ToUpper().Equals("GIA"))
                {
                    e.Row["GiaBanTT"] = e.Row["Gia"];
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
