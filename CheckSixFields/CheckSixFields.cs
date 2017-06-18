using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;

using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using CDTLib;
using System.Windows.Forms;

namespace CheckSixFields
{
    public class CheckSixFields : ICControl
    {
        private DataCustomFormControl data;
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        Database db = Database.NewDataDatabase();


        public DataCustomFormControl Data
        {
            set { data = value; }
        }
        public void AddEvent()
        {
            if (data.BsMain.DataSource != null && data.BsMain != null)
            {
                data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
                BsMain_DataSourceChanged(data.BsMain, new EventArgs());
            }
        }
        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataSet ds = data.BsMain.DataSource as DataSet;
            if (ds == null)
                return;
            ds.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(CheckSixFields_ColumnChanged);
        }

        void CheckSixFields_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Added)
            {
                if (e.Column.ColumnName.ToUpper().Equals("SOBG"))
                {
                    string SoBG = e.Row["SoBG"].ToString();
                    string sql = string.Format(@" select	sum(d.op) [cut],sum(d.pe) [prepareend],sum(d.hs) [hot],sum(d.ft) [fastener],sum(d.hp) [holepunching],sum(d.at) [splicing]
                                    from	mt63 m inner join dt63 d on d.mt63id = m.mt63id 
                                    where	m.soct = '{0}'", SoBG);
                    using(DataTable dt = db.GetDataTable(sql))
                    {
                        if (dt.Rows.Count > 0)
                        {
                            e.Row["cut"] = (decimal)dt.Rows[0]["cut"] > 0 ? 1 : 0;
                            e.Row["Prepareend"] = (decimal)dt.Rows[0]["prepareend"] > 0 ? 1 : 0;
                            e.Row["hot"] = (decimal)dt.Rows[0]["hot"] > 0 ? 1 : 0;
                            e.Row["fastener"] = (decimal)dt.Rows[0]["fastener"] > 0 ? 1 : 0;
                            e.Row["Holepunching"] = (decimal)dt.Rows[0]["holepunching"] > 0 ? 1 : 0;
                            e.Row["splicing"] = (decimal)dt.Rows[0]["splicing"] > 0 ? 1 : 0;
                        }
                    }
                }
            }
        }
        public InfoCustomControl Info
        {
            get { return info; }
        }
    }
}
