using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using DevExpress.XtraEditors;
using CDTLib;
using System.Windows.Forms;

namespace HuyBaoGiaDt
{
    public class HuyBaoGiaDt:ICForm
    {
        #region ICForm Members
        private List<InfoCustomForm> _lstInfo = new List<InfoCustomForm>();
        private DataCustomFormControl _data;
        Database db = Database.NewDataDatabase();
        public HuyBaoGiaDt()
        {
            InfoCustomForm info = new InfoCustomForm(IDataType.MasterDetailDt, 1001, "Hủy báo giá",
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
                if (_data.BsMain == null || _data.BsMain.Current == null)
                    return;
                DataRow drMaster = (_data.BsMain.Current as DataRowView).Row;
                if (drMaster.RowState != DataRowState.Unchanged)
                {
                    XtraMessageBox.Show("Vui lòng lưu số liệu trước khi chọn hủy báo giá",
                        Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                    return;
                }
                
                if (drMaster["SPDeNghi"].ToString() != "")
                {
                    string sql = "select DNID,SoBG,SoPhieu from MTSO where SoPhieuDN='" + drMaster["SPDeNghi"].ToString() + "'";
                    DataTable tdso = db.GetDataTable(sql);
                    if (tdso.Rows.Count > 0 && tdso.Rows[0]["SoPhieu"].ToString() != "")
                    {
                        XtraMessageBox.Show("Báo giá này đã giao hàng, không thể hủy báo giá!", Config.GetValue("PackageName").ToString());
                        return;
                    }
                    else
                    {
                        XtraMessageBox.Show("Báo giá này đã lập phiếu đề nghị giao hàng, không thể hủy báo giá!", Config.GetValue("PackageName").ToString());
                        return;
                    }
                }
                XtraForm1 frm = new XtraForm1(drMaster);
                frm.ShowDialog();
                if (frm.check == true)
                {
                    drMaster["TinhTrangBG"] = 2;
                    drMaster["NgayHuy"] = frm.dateEdit1.EditValue;
                    drMaster.AcceptChanges();
                    XtraMessageBox.Show("Đã hủy báo giá", Config.GetValue("PackageName").ToString());
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
