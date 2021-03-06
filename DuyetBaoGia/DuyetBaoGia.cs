using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using System.Windows.Forms;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraEditors;

namespace DuyetBaoGia
{
    public class DuyetBaoGia:ICForm
    {
        #region ICForm Members
        private List<InfoCustomForm> _lstInfo = new List<InfoCustomForm>();
        private DataCustomFormControl _data;
        Database db = Database.NewDataDatabase();
        public DuyetBaoGia()
        {
            InfoCustomForm info = new InfoCustomForm(IDataType.MasterDetail, 9487, "Duyệt báo giá",
              "", "MT63");
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
                var oDuyetBG = db.GetValue(string.Format("select DuyetBG from DMNhanVien where MaNV = N'{0}'", Config.GetValue("UserName")));
                if (oDuyetBG == null || Convert.ToInt32(oDuyetBG) == 0)
                {
                    XtraMessageBox.Show("Bạn không có quyền thực hiện chức năng này");
                    return;
                }

                var duyetBG = Convert.ToInt32(oDuyetBG);

                DataTable tb = new DataTable();
                tb.Columns.Add("gridId", typeof(int));
                tb.Columns.Add("id",typeof(string));
                tb.Columns.Add("CheckDuyet", typeof(string));
                GridView gv = ((_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView);

                int[] a = gv.GetSelectedRows();
                if (a.Length == 0 || (a.Length > 0 && gv.GetDataRow(a[0])["CheckDuyet"].ToString() == "Không cần duyệt"))
                {
                    XtraMessageBox.Show("Hãy chọn báo giá cần duyệt hoặc cần bỏ duyệt");
                    return;
                }

                var tinhTrang = gv.GetDataRow(a[0])["CheckDuyet"].ToString();

                for (int i = 0; i < a.Length; i++)
                {
                    if (gv.GetDataRow(a[i])["CheckDuyet"].ToString() != tinhTrang)
                    {
                        XtraMessageBox.Show("Cần chọn báo giá có cùng tình trạng phê duyệt để xử lý");
                        return;
                    }
                    DataRow row = tb.NewRow();
                    row["gridId"] = a[i];
                    row["id"] = gv.GetDataRow(a[i])["MT63ID"];
                    row["CheckDuyet"] = gv.GetDataRow(a[i])["CheckDuyet"];
                    tb.Rows.Add(row);
                }

                if ((duyetBG == 1 && tinhTrang == "Đã duyệt")
                    || (duyetBG == 2 && tinhTrang == "Chờ duyệt"))
                {
                    XtraMessageBox.Show("Bạn không có quyền thay đổi tình trạng phê duyệt này");
                    return;
                }

                XtraForm1 frm = new XtraForm1(tb, tinhTrang, duyetBG, gv);
                frm.ShowDialog();
            }
        }

        public List<InfoCustomForm> LstInfo
        {
            get { return _lstInfo; }
        }

        #endregion
    }
}
