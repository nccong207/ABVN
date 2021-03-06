using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using CDTDatabase;
using DevExpress.XtraEditors;
using CDTLib;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;
using System.Data;

namespace TaoNhom
{
    public class TaoNhom : ICControl
    {
        Database db = Database.NewDataDatabase();
        private DataCustomFormControl data;
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        GridControl gcCT;
        GridView gvCT;
        bool flag = true;

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
            gcCT = (data.FrmMain.Controls.Find("DTGiaCongCT", true)[0] as GridControl);
            gvCT = gcCT.MainView as GridView;
            gvCT.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gvMain_CellValueChanged);
            RepositoryItemGridLookUpEdit riMaVT = gcCT.RepositoryItems["MaVT"] as RepositoryItemGridLookUpEdit;
            riMaVT.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(riMaVT_EditValueChanging);
            RepositoryItemGridLookUpEdit riCode = gcCT.RepositoryItems["Code"] as RepositoryItemGridLookUpEdit;
            riCode.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(riCode_CloseUp);
        }

        void riCode_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            if (e.CloseMode != PopupCloseMode.Normal)
                return;
            object o = gvCT.GetFocusedRowCellValue("NX");
            if (o == null || o.ToString() == "N")
                return;
            GridLookUpEdit glu = sender as GridLookUpEdit;
            if (glu.Properties.View.FocusedRowHandle < 0)
                return;
            DataRowView drHH = glu.Properties.View.GetRow(glu.Properties.View.FocusedRowHandle) as DataRowView;
            string code = drHH["PNo"].ToString();
            string nhom = drHH["Nhom"].ToString();
            string bac = drHH["Bac"].ToString();
            if (nhom == "" || bac == "")
                return;
            DataTable dt = (glu.Properties.DataSource as BindingSource).DataSource as DataTable;
            DataRow[] drs = dt.Select("Pno = '" + code + "' and Nhom = '" + nhom + "' and Bac > " + bac, "Bac asc");
            if (drs.Length == 0)
                return;
            if (XtraMessageBox.Show("Bạn có muốn xuất tất cả các bậc còn lại không?",
                Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            gvCT.SetFocusedRowCellValue(gvCT.Columns["Code"], code);
            gvCT.AddNewRow();
            for (int i = 0; i < drs.Length; i++)
            {
                DataRow dr = drs[i];
                gvCT.UpdateCurrentRow();
                gvCT.SetFocusedRowCellValue(gvCT.Columns["NX"], "X");
                gvCT.SetFocusedRowCellValue(gvCT.Columns["Code"], dr["Pno"]);
                gvCT.SetFocusedRowCellValue(gvCT.Columns["NgayNhap"], dr["NgayCT"]);
                gvCT.SetFocusedRowCellValue(gvCT.Columns["Nhom"], dr["Nhom"]);
                gvCT.SetFocusedRowCellValue(gvCT.Columns["Rong"], dr["Rong"]);
                gvCT.SetFocusedRowCellValue(gvCT.Columns["Dai"], dr["Dai"]);
                gvCT.SetFocusedRowCellValue(gvCT.Columns["Day"], dr["Day"]);
                gvCT.SetFocusedRowCellValue(gvCT.Columns["MaKe"], dr["MaKe"]);
                gvCT.SetFocusedRowCellValue(gvCT.Columns["MaNgan"], dr["MaNgan"]);
                if (i < drs.Length - 1)
                    gvCT.AddNewRow();
            }
        }

        void riMaVT_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            object o = gvCT.GetFocusedRowCellValue("NX");
            if (o == null || o.ToString() == "")
            {
                XtraMessageBox.Show("Vui lòng chọn loại nhập xuất trước!", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                e.Cancel = true;
            }
        }

        private int LayNhomMoi(string nhomcu, string code, GridView gv)
        {
            //DataRow drMaster = (data.BsMain.Current as DataRowView).Row;
            //DataSet ds = data.BsMain.DataSource as DataSet;
            //DataRow[] drs = ds.Tables[0].Select("");
            //DataView dv = new DataView(ds.Tables[2]);
            //dv.Sort = "Nhom desc";
            //string nhom = Convert.ToInt32(dv[0].Row["Nhom"]);            
            
            nhomcu = nhomcu.Substring(0, 5);
            gv.ActiveFilterString = "NX = 'N' and Code = '" + code + "' and Nhom like '" + nhomcu + "%'";
            int max = 0;
            int temp = 0;
            if (gv.DataRowCount > 0)
            {
                for (int i = 0; i < gvCT.DataRowCount; i++)
                {
                    DataRow dr = gv.GetDataRow(i);
                    string[] aa = dr["nhom"].ToString().Split('-');
                    if (aa.Length > 1)
                    {
                        if (Int32.TryParse(aa[1], out temp))
                            temp = temp;
                    }
                    if (temp > max)
                    {
                        max = temp;
                    }              
                }                
            }
            gv.ActiveFilterString = "";
 
            int max2 = 0;
            object o = db.GetValue("select top 1 Nhom from wblbac where nhom like '" + nhomcu + "%' order by Nhom desc");
            string[] ss = o.ToString().Split('-');
            if (ss.Length > 1)
            {
                if (Int32.TryParse(ss[1], out max2))
                    max2 = max2;
            }
            int n = 0;
            if (max > max2)
                n = max;
            if (max <= max2)
                n = max2;
            return n + 1;

        }

        void gvMain_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string fn = e.Column.FieldName.ToUpper();
            GridView gv = sender as GridView;
            if (fn == "NX" && e.Value.ToString() == "N" && flag)
            {
                int pr = -1;
                //if (e.RowHandle > 0)
                pr = e.RowHandle - 1;
                if (e.RowHandle < 0 && gv.DataRowCount > 0)
                    pr = gv.DataRowCount - 1;
                string nx = pr == -1 ? "" : gv.GetRowCellValue(pr, "NX").ToString();
                if (nx == "" || nx == "N")
                {
                    XtraMessageBox.Show("Vui lòng chọn mặt hàng xuất trước khi tạo các bậc mới!",
                        Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                    return;
                }
                FrmBac frm = new FrmBac();
                frm.ShowDialog();
                string code = gv.GetRowCellValue(pr, "Code").ToString();
                string sl = gv.GetRowCellValue(pr, "SL").ToString();
                string nhom = gv.GetRowCellValue(pr, "Nhom").ToString();
                string ngaynhap = gv.GetRowCellValue(pr, "NgayNhap").ToString();
                string MaKe = gv.GetRowCellValue(pr, "MaKe").ToString();
                string MaNgan = gv.GetRowCellValue(pr, "MaNgan").ToString();
                if (nhom == "")
                {
                    XtraMessageBox.Show("Mặt hàng chưa có nhóm, vui lòng tạo nhóm trước khi xuất",
                        Config.GetValue("PackageName").ToString());
                    return;
                }
                flag = false;
                //so bac nhap lai
                for (int i = 0; i < frm.Bac; i++)
                {
                    gv.UpdateCurrentRow();
                    gv.SetFocusedRowCellValue(gv.Columns["Code"], code);
                    gv.SetFocusedRowCellValue(gv.Columns["SL"], sl);
                    gv.SetFocusedRowCellValue(gv.Columns["NgayNhap"], ngaynhap);
                    gv.SetFocusedRowCellValue(gv.Columns["Nhom"], nhom);
                    gv.SetFocusedRowCellValue(gv.Columns["Rong"], null);
                    gv.SetFocusedRowCellValue(gv.Columns["Dai"], null);
                    gv.SetFocusedRowCellValue(gv.Columns["SLQD"], null);
                    gvCT.SetFocusedRowCellValue(gv.Columns["MaKe"], MaKe);
                    gvCT.SetFocusedRowCellValue(gv.Columns["MaNgan"], MaNgan);
                    gv.SetFocusedRowCellValue(gv.Columns["NX"], "N");
                    if (i < frm.Bac - 1)
                        gv.AddNewRow();
                }
                //nhap nhom moi
                if (frm.dtNhom.Rows.Count > 0)
                {
                    int n = LayNhomMoi(nhom, code, gv);
                    for (int i = 0; i < frm.dtNhom.Rows.Count; i++)
                    {
                        gv.AddNewRow();
                        int m = n + i;

                        int sb = Int32.Parse(frm.dtNhom.Rows[i]["Bac"].ToString());
                        for (int j = 0; j < sb; j++)
                        {
                            flag = false;
                            gv.UpdateCurrentRow();
                            gv.SetFocusedRowCellValue(gv.Columns["Code"], code);
                            gv.SetFocusedRowCellValue(gv.Columns["SL"], sl);
                            gv.SetFocusedRowCellValue(gv.Columns["NgayNhap"], ngaynhap);
                            gv.SetFocusedRowCellValue(gv.Columns["Nhom"], nhom.Substring(0, 5) + "-" + m.ToString("D2"));
                            gv.SetFocusedRowCellValue(gv.Columns["Rong"], null);
                            gv.SetFocusedRowCellValue(gv.Columns["Dai"], null);
                            gv.SetFocusedRowCellValue(gv.Columns["SLQD"], null);
                            gvCT.SetFocusedRowCellValue(gv.Columns["MaKe"], MaKe);
                            gvCT.SetFocusedRowCellValue(gv.Columns["MaNgan"], MaNgan);
                            gv.SetFocusedRowCellValue(gv.Columns["NX"], "N");
                            gv.SetFocusedRowCellValue(gv.Columns["Cat"], true);
                            if (j < sb - 1)
                                gv.AddNewRow();
                        }
                    }
                }
                flag = true;
                gv.FocusedColumn = gv.Columns["Rong"];
            }
        }
    }
}
