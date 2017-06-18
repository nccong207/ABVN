using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Plugins;
using CDTDatabase;
using CDTLib;
using System.Windows.Forms;

namespace LoaiFileCatalog
{
    public class LoaiFileCatalog:ICControl
    {        
        private DataCustomFormControl data;
        private InfoCustomControl info = new InfoCustomControl(IDataType.Single);
       
        #region ICControl Members

        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        public void AddEvent()
        {
            if (data.BsMain == null)
                return;

            data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(data.BsMain, new EventArgs());
        }

        private void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataTable dtData = data.BsMain.DataSource as DataTable;
            if (dtData != null)
                dtData.ColumnChanged += new DataColumnChangeEventHandler(LayLoaiFile_ColumnChanged);
        }

        private void LayLoaiFile_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName.ToUpper() == "TenTLFile".ToUpper())
            {
                if(string.IsNullOrEmpty(e.Row["TenTLFile"].ToString()))
                    return;

                string[] fileType = e.Row["TenTLFile"].ToString().Split('.');
                if (fileType.Length > 1)
                {
                    e.Row["LoaiFile"] = fileType[fileType.Length -1];
                }
            }
            
        }

        #endregion
    }
}
