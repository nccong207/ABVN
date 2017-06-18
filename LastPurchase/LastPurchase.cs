using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using CDTDatabase;

namespace LastPurchase
{
    public class LastPurchase:ICControl
    {
        #region ICControl Members
        DataCustomFormControl _data;
        InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
        Database db = Database.NewDataDatabase();
        public void AddEvent()
        {
            _data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(_data.BsMain, new EventArgs());           
        }
        private void BsMain_DataSourceChanged(object p, EventArgs eventArgs)
        {
            DataSet ds = _data.BsMain.DataSource as DataSet;
            if (ds != null)
                ds.Tables[1].ColumnChanged += new DataColumnChangeEventHandler(Capnhap_ColumnChanged);
        }
        void Capnhap_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            
            if (e.Column.ColumnName.ToString() == "MaVT")
            {
               string ma= e.Row["MaVT"].ToString();
               string query = "select top 1 d.Gia,m.NgayCT from MT62 m,DT62 d where m.MT62ID=d.MT62ID and d.MaVT='"+ma+"' order by m.NgayCT desc";

                        DataTable tb = db.GetDataTable(query);
               if (tb.Rows.Count > 0)
               {
                   e.Row["GiaLast"] = tb.Rows[0]["Gia"].ToString();
                   e.Row["Since"] = tb.Rows[0]["NgayCT"].ToString();
               }
               else
               {
                    e.Row["GiaLast"] = "0";
                    e.Row["Since"] = DBNull.Value;
               }
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
