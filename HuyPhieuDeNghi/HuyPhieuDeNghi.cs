using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using CDTLib;

namespace HuyPhieuDeNghi
{
    public class HuyPhieuDeNghi:ICForm
    {
        #region ICForm Members
        private List<InfoCustomForm> _lstInfo = new List<InfoCustomForm>();
        private DataCustomFormControl _data;
        Database db = Database.NewDataDatabase();
        public HuyPhieuDeNghi()
        {
            InfoCustomForm info = new InfoCustomForm(IDataType.MasterDetail, 9487, "Hủy phiếu đề nghị",
              "", "MTSO");
            _lstInfo.Add(info);
        }
        public DataCustomFormControl Data
        {
            set { _data = value; }
          
        }

        public void Execute(int menuID)
        {
            if (menuID == _lstInfo[0].MenuID)
            {
                DataTable tb = new DataTable();
                tb.Columns.Add("idso", typeof(string));
                tb.Columns.Add("sobg", typeof(string));
                GridView gv = ((_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView);
                int[] a = gv.GetSelectedRows();
                if (a.Length <= 0)
                {
                    XtraMessageBox.Show("Chọn một số phiếu cần hủy", Config.GetValue("PackageName").ToString());
                    return;
                }
                bool check = true;
                bool isAdmin = Boolean.Parse(Config.GetValue("Admin").ToString());
                for (int i = 0; i < a.Length; i++)
                {
                    DateTime ngaylp = DateTime.Parse(gv.GetDataRow(a[i])["NgayLP"].ToString());
                    if (!isAdmin && ngaylp != DateTime.Today)
                    {
                        check = false;
                        break;
                    }
                    if (gv.GetDataRow(a[i])["SoPhieu"].ToString() == "" || gv.GetDataRow(a[i])["SoPhieu"].ToString() == null)
                    {
                        DataRow row = tb.NewRow();
                        row["idso"] = gv.GetDataRow(a[i])["DNID"];
                        string sobg = gv.GetDataRow(a[i])["SoBG"].ToString();
                        row["sobg"] = sobg;
                        tb.Rows.Add(row);
                    }
                }
                if (!check && !isAdmin)
                {
                    XtraMessageBox.Show("Chỉ được hủy phiếu lập trong ngày hôm nay!",
                        Config.GetValue("PackageName").ToString());
                    return;
                }
                if (tb.Rows.Count > 0)
                {
                    XtraForm1 frm = new XtraForm1(tb);
                    frm.ShowDialog();
                    if (frm.check == true)
                    {
                        for (int i = 0; i < a.Length; i++)
                        {
                            gv.GetDataRow(a[i])["TinhTrang"] = "2";
                            gv.GetDataRow(a[i])["NgayHuy"] = frm.dateEdit1.EditValue;
                            gv.GetDataRow(a[i])["Note"] = frm.txtghichu.EditValue;
                        }
                        XtraMessageBox.Show("Đã hủy phiếu",Config.GetValue("PackageName").ToString());
                    }
                }
                else
                {
                    XtraMessageBox.Show("Phiếu đề nghị đã tồn tại phiếu giao hàng", Config.GetValue("PackageName").ToString());
                }

            }
        }

        public List<InfoCustomForm> LstInfo
        {
            get { return _lstInfo; }
        }

        #endregion
    }
}
