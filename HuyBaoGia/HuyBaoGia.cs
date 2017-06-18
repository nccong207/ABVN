using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using CDTLib;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;

namespace HuyBaoGia
{
    public class HuyBaoGia:ICForm
    {
        #region ICForm Members
        private List<InfoCustomForm> _lstInfo = new List<InfoCustomForm>();
        private DataCustomFormControl _data;
        Database db = Database.NewDataDatabase();
        public HuyBaoGia()
        {
            InfoCustomForm info = new InfoCustomForm(IDataType.MasterDetail, 9487, "Hủy báo giá",
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
                //string sql = "select * from CDTABV..sysUserTable a where sysUserSiteID=(select sysUserSiteID from CDTABV..sysUser a,CDTABV..sysUserSite b where a.sysUserID=b.sysUserID and a.UserName='"+Config.GetValue("UserName")+"') and sApprove='1' and sysTableID='2177'";
                //DataTable datb = db.GetDataTable(sql);
                //if (datb.Rows.Count == 0)
                //{
                //    XtraMessageBox.Show("Không có quyền thực hiện chức năng");
                //    return;
                //}
                

                DataTable tb = new DataTable();
                tb.Columns.Add("id63",typeof(string));
                tb.Columns.Add("idso", typeof(string));
                GridView gv = ((_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView);
                //DataView dv = new DataView(gv.DataSource as DataTable);
                //dv.RowStateFilter = DataViewRowState.
                int[] a = gv.GetSelectedRows();
                if (a.Length <= 0)
                {
                    XtraMessageBox.Show("Chọn một báo giá cần hủy");
                    return;
                }
                for (int i = 0; i < a.Length; i++)
                {
                    if (gv.GetDataRow(a[i])["SPDeNghi"].ToString() == "" || gv.GetDataRow(a[i])["SPDeNghi"].ToString() == null)
                    {
                        DataRow row = tb.NewRow();
                        row["id63"] = gv.GetDataRow(a[i])["MT63ID"];
                        row["idso"] = "";
                        tb.Rows.Add(row);
                    }
                }

                if (tb.Rows.Count > 0)
                {
                    XtraForm1 frm = new XtraForm1(tb);
                    frm.ShowDialog();
                    if (frm.check == true)
                    {
                        for (int i = 0; i < a.Length; i++)
                        {

                            //gv.GetDataRow(a[i])["CheckDuyet"] = frm.combotinhtrang.EditValue;
                            gv.GetDataRow(a[i])["TinhTrangBG"] = "2";
                            gv.GetDataRow(a[i])["NgayHuy"] = frm.dateEdit1.EditValue;
                            gv.GetDataRow(a[i])["GhiChu"] = frm.txtghichu.EditValue; 


                        }
                        XtraMessageBox.Show("Đã hủy báo giá", Config.GetValue("PackageName").ToString());
                    }
                  
                }
                else
                {
                    XtraMessageBox.Show("Báo giá đã tồn tại phiếu đề nghị hoặc giao hàng", Config.GetValue("PackageName").ToString());
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
