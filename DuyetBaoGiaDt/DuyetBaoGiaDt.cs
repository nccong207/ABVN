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

namespace DuyetBaoGiaDt
{
    public class DuyetBaoGiaDt:ICForm
    {
        #region ICForm Members
        private List<InfoCustomForm> _lstInfo = new List<InfoCustomForm>();
        private DataCustomFormControl _data;
        Database db = Database.NewDataDatabase();
        public DuyetBaoGiaDt()
        {
            InfoCustomForm info = new InfoCustomForm(IDataType.MasterDetailDt, 9487, "Duyệt báo giá",
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

                if (_data.BsMain == null || _data.BsMain.Current == null)
                    return;
                DataRow drMaster = (_data.BsMain.Current as DataRowView).Row;
                if (drMaster.RowState != DataRowState.Unchanged)
                {
                    XtraMessageBox.Show("Vui lòng lưu số liệu trước khi chọn duyệt báo giá",
                        Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                    return;
                }
                var tinhTrang = drMaster["CheckDuyet"].ToString();
                if (tinhTrang == "Không cần duyệt")
                {
                    XtraMessageBox.Show("Hãy chọn báo giá cần duyệt hoặc cần bỏ duyệt");
                    return;
                }

                if ((duyetBG == 1 && tinhTrang == "Đã duyệt")
                    || (duyetBG == 2 && tinhTrang == "Chờ duyệt"))
                {
                    XtraMessageBox.Show("Bạn không có quyền thay đổi tình trạng phê duyệt này");
                    return;
                }

                XtraForm1 frm = new XtraForm1(drMaster, drMaster["CheckDuyet"].ToString(), duyetBG);
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
