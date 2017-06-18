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
                if (!Boolean.Parse(Config.GetValue("Admin").ToString()) && !Boolean.Parse(_data.DrTable["sApprove"].ToString()))
                {
                    XtraMessageBox.Show("Không có quyền thực hiện chức năng");
                    return;
                }

                if (_data.BsMain == null || _data.BsMain.Current == null)
                    return;
                DataRow drMaster = (_data.BsMain.Current as DataRowView).Row;
                if (drMaster.RowState != DataRowState.Unchanged)
                {
                    XtraMessageBox.Show("Vui lòng lưu số liệu trước khi chọn duyệt báo giá",
                        Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                    return;
                }
                if (drMaster["CheckDuyet"].ToString() != "Chờ duyệt" && drMaster["CheckDuyet"].ToString() != "Đã duyệt")
                {
                    XtraMessageBox.Show("Hãy chọn báo giá có tình trạng 'Đang chờ' để duyệt hoặc báo giá 'Đã duyệt' để hủy duyệt", Config.GetValue("PackageName").ToString());
                    return;
                }
                XtraForm1 frm = new XtraForm1(drMaster);
                frm.ShowDialog();
                if (frm.check == true)
                {
                    if (drMaster["CheckDuyet"].ToString() == "Chờ duyệt")
                    {
                        drMaster["CheckDuyet"] = frm.combotinhtrang.EditValue;
                        drMaster["Note"] = frm.txtghichu.EditValue;
                        drMaster["ngayDuyet"] = frm.dayngayduyet.EditValue;
                        drMaster["NguoiDuyet"] = Config.GetValue("UserName");
                        XtraMessageBox.Show("Đã duyệt báo giá");
                    }
                    else if (drMaster["CheckDuyet"].ToString() == "Đã duyệt")
                    {
                        drMaster["CheckDuyet"] = "Chờ duyệt";
                        drMaster["Note"] = DBNull.Value;
                        drMaster["ngayDuyet"] = DBNull.Value;
                        drMaster["NguoiDuyet"] = DBNull.Value;
                        XtraMessageBox.Show("Báo giá đã được hủy duyệt");
                    }
                }
               // DataTable tb = new DataTable();
               // tb.Columns.Add("id",typeof(string));
               // tb.Columns.Add("CheckDuyet", typeof(string));
               // GridView gv = ((_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView);
               // //DataView dv = new DataView(gv.DataSource as DataTable);
               // //dv.RowStateFilter = DataViewRowState.
               // int[] a = gv.GetSelectedRows();
               // if (a.Length <= 0)
               // {
               //     XtraMessageBox.Show("Chọn một báo giá cần duyệt hoặc cần hủy duyệt");
               //     return;
               // }
               // for (int i = 0; i < a.Length; i++)
               // {
               //     if (gv.GetDataRow(a[i])["CheckDuyet"].ToString() == "Chờ duyệt" || gv.GetDataRow(a[i])["CheckDuyet"].ToString() == "Đã duyệt")
               //     {
               //         DataRow row = tb.NewRow();
               //         row["id"] = gv.GetDataRow(a[i])["MT63ID"];
               //         row["CheckDuyet"] = gv.GetDataRow(a[i])["CheckDuyet"];
               //         tb.Rows.Add(row);
               //     }
               //     else if (a.Length == 1)
               //     {
               //         XtraMessageBox.Show("Hãy chọn báo giá đang chờ duyệt");
               //         return;

               //     }


               // }


               // XtraForm1 frm = new XtraForm1(tb);
               //// frm.Text = "Danh mục hợp đồng";
               // frm.ShowDialog();
               // if (frm.check == true)
               // {
               //     for (int i = 0; i < a.Length; i++)
               //     {
               //         if (gv.GetDataRow(a[i])["CheckDuyet"].ToString() == "Chờ duyệt")
               //         {
               //             gv.GetDataRow(a[i])["CheckDuyet"] = frm.combotinhtrang.EditValue;
               //             gv.GetDataRow(a[i])["GhiChu"]=frm.txtghichu.EditValue;
               //             gv.GetDataRow(a[i])["ngayDuyet"]=frm.dayngayduyet.EditValue;
               //            // gv.GetDataRow(a[i])["NguoiDuyet"] = "";
               //             XtraMessageBox.Show("Đã duyệt báo giá");
               //         }
               //        else if (gv.GetDataRow(a[i])["CheckDuyet"].ToString() == "Đã duyệt")
               //         {
               //             gv.GetDataRow(a[i])["CheckDuyet"] = "Chờ duyệt";
               //             gv.GetDataRow(a[i])["GhiChu"] = "";
               //             //gv.GetDataRow(a[i])["ngayDuyet"] = DBNull.Value;
               //             // gv.GetDataRow(a[i])["NguoiDuyet"] = "";
               //             XtraMessageBox.Show("Báo giá đã được hủy duyệt");
               //         }

               //     }
               // }

               

            }
        }

        public List<InfoCustomForm> LstInfo
        {
            get { return _lstInfo; }
        }

        #endregion
    }
}
