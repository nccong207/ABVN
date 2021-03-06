using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using Plugins;
using CDTDatabase;
using CDTLib;
using System.Windows.Forms;
using System.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;

namespace AnCot
{
    public class AnCot:ICControl
    {
        #region ICControl Members

        private InfoCustomControl info = new InfoCustomControl(IDataType.Single);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();

        public void AddEvent()
        {
            data.FrmMain.Shown += new EventHandler(FrmMain_Shown);
        }

        void FrmMain_Shown(object sender, EventArgs e)
        {
            GridView gvDetail = (data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            //Xep hang len ke
            if (data.DrTable["ExtraSql"].ToString() == "")
            {
                List<string> cledit = new List<string>();
                cledit.Add("MaKe");
                cledit.Add("MaNgan");
                foreach (GridColumn gc in gvDetail.Columns)
                    if (!cledit.Contains(gc.FieldName))
                        gc.OptionsColumn.AllowEdit = false;

                gvDetail.Columns["SoHDN"].Visible = true;
                gvDetail.Columns["NgayHDN"].Visible = true;
                gvDetail.Columns["MaNCC"].Visible = true;

                gvDetail.Columns["KyHieu"].Visible = false;
                gvDetail.Columns["SL2"].Visible = false;
                gvDetail.Columns["SLQuyDoi"].Visible = false;
                gvDetail.Columns["Gia"].Visible = false;
                gvDetail.Columns["Ps"].Visible = false;
                gvDetail.Columns["CPCt"].Visible = false;
                gvDetail.Columns["MaThueNkCt"].OptionsColumn.ReadOnly = true;
                gvDetail.Columns["ThueSuatNk"].Visible = false;
                gvDetail.Columns["CtThueNk"].Visible = false;
                gvDetail.Columns["GiaKho"].Visible = false;
            }
            else if (data.DrTable["ExtraSql"].ToString().Replace("'", "").ToUpper() == "CNGB<>1")
            {
                List<string> cledit = new List<string>(new string[] 
                    { "NgayCTDt", "MaNCC", "SoHDN", "NgayHDN", "MaVT", "KyHieu", "Code", "Day", "Rong",
                    "Dai", "Gia", "MaDVT", "Ps", "CPCt", "MaThueNkCt", "GiaKho" });
                foreach (GridColumn gc in gvDetail.Columns)
                {
                    if (cledit.Contains(gc.FieldName))
                    {
                        gc.Visible = true;
                        gc.OptionsColumn.AllowEdit = false;
                    }
                    else
                        gc.Visible = false;
                }
                //hien cot
                List<string> clVisible = new List<string>(new string[] { "TLLoiNhuan", "GBMM", "DieuChinh", "GiaBanCD", "IsCapNhat" });
                foreach (GridColumn gc in gvDetail.Columns)
                    if (clVisible.Contains(gc.FieldName))
                    {
                        gc.Visible = true;
                        if (gc.FieldName.Equals("GBMM") || gc.FieldName.Equals("GiaBanCD"))
                            gc.OptionsColumn.AllowEdit = false;
                    }
            }
            gvDetail.BestFitColumns();
        }

        public DataCustomFormControl Data
        {
            set { data=value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        #endregion
    }
}
