using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using CDTDatabase;
using System.Data;
using DevExpress.XtraEditors.Repository;
namespace LocCTCatTheoBac
    {
    public class LocCTCatTheoBac : ICControl
    {
        #region ICControl Members
        private DataCustomFormControl _data;
        private InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
        public void AddEvent()
        {
            XtraTabControl tcMain = _data.FrmMain.Controls.Find("tcMain", true)[0] as XtraTabControl;
            tcMain.SelectedPageChanged += new TabPageChangedEventHandler(tcMain_SelectedPageChanged);
        }
        GridView gvGiaCong;
        GridControl gcCT;
        GridView gvCT;

        void tcMain_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            XtraTabControl tcMain = sender as XtraTabControl;
            if (tcMain.TabPages[0] == e.Page)
                return;
            gvGiaCong = (_data.FrmMain.Controls.Find("GcMain", true)[0] as GridControl).MainView as GridView;
            if (gvGiaCong.DataRowCount > 0)
            {

                gcCT = (_data.FrmMain.Controls.Find("DTGiaCongCT", true)[0] as GridControl);
                gvCT = gcCT.MainView as GridView;
                RepositoryItemGridLookUpEdit riMaVT = gcCT.RepositoryItems["Code"] as RepositoryItemGridLookUpEdit;
                riMaVT.Popup += new EventHandler(riMaVT_Popup);

            }
        }



        void riMaVT_Popup(object sender, EventArgs e)
        {
            string fillter = "";
            for (int i = 0; i < gvGiaCong.DataRowCount; i++)
            {
                DataRow drGiaCong = gvGiaCong.GetDataRow(i);
                fillter += "PNo = '";
                fillter += drGiaCong["Code"].ToString();
                fillter += "'";
                if (i < gvGiaCong.DataRowCount - 1)
                    fillter += " or ";
            }
            GridLookUpEdit lok = sender as GridLookUpEdit;
            lok.Properties.View.ActiveFilterString = fillter;
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
