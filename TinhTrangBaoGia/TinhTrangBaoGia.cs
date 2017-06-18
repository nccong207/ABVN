using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.Data;
using CDTLib;
namespace TinhTrangBaoGia
{
    public class TinhTrangBaoGia:ICControl
    {
        #region ICControl Members
         private DataCustomFormControl data;
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        Database db = Database.NewDataDatabase();
        public void AddEvent()
        {
            if (data.BsMain.DataSource != null && data.BsMain !=null)
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
            ds.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(TinhTrangBaoGia_ColumnChanged);
        }

        void TinhTrangBaoGia_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if(e.Row.RowState == DataRowState.Deleted)
                return;
            if (e.Column.ColumnName.ToUpper().Equals("SPDENGHI"))
            {
                if (e.Row["SPDeNghi"].ToString() != "")
                {
                    e.Row["TinhTrangBG"] = 1;
                    e.Row.EndEdit();
                }
              else  if (e.Row["SPDeNghi"].ToString() == "")
                {
                    e.Row["TinhTrangBG"] = 3;
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
