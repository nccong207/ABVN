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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;

namespace TinhChiPhi
{
    public class TinhChiPhi : ICControl
    {
        #region ICControl Members

        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();
        GridControl gcMain;
        GridView gvMain;
        GridLookUpEdit gluKH;
        ComboBoxEdit cbeDCGH;
        ComboBoxEdit cbeNLH;
        ComboBoxEdit cbeDT;

        public void AddEvent()
        {
            //bổ sung chọn địa chỉ giao hàng, người liên hệ và số điện thoại
            cbeDCGH = data.FrmMain.Controls.Find("NguoiLH", true)[0] as ComboBoxEdit;
            cbeNLH = data.FrmMain.Controls.Find("NLH", true)[0] as ComboBoxEdit;
            cbeDT = data.FrmMain.Controls.Find("SDTNL", true)[0] as ComboBoxEdit;
            cbeDCGH.QueryPopUp += new System.ComponentModel.CancelEventHandler(cbeDCGH_QueryPopUp);
            cbeNLH.QueryPopUp += new System.ComponentModel.CancelEventHandler(cbeNLH_QueryPopUp);
            cbeNLH.EditValueChanged += new EventHandler(cbeNLH_EditValueChanged);

            gluKH = data.FrmMain.Controls.Find("MaKH", true)[0] as GridLookUpEdit;


            //gluKH.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(gluKH_CloseUp);

            gcMain = data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl;
            gvMain = gcMain.MainView as GridView;
            RepositoryItemGridLookUpEdit gluDVT = gcMain.RepositoryItems["MaDVT"] as RepositoryItemGridLookUpEdit;
            gluDVT.QueryCloseUp += new System.ComponentModel.CancelEventHandler(gluDVT_QueryCloseUp);
            gvMain.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(gvMain_FocusedColumnChanged);

            data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(data.BsMain, new EventArgs());
            //them chuc nang Lam tron tien
            LayoutControl lcMain = data.FrmMain.Controls.Find("lcMain", true)[0] as LayoutControl;
            SimpleButton btnLamTron = new SimpleButton();
            btnLamTron.Name = "btnLamTron";   //phai co name cua control
            btnLamTron.Text = "Làm tròn";
            LayoutControlItem lci = lcMain.AddItem("", btnLamTron);
            lci.Name = "cusLamTron"; //phai co name cua item, bat buoc phai co "cus" phai truoc
            btnLamTron.Click += new EventHandler(btnLamTron_Click);
        }

        void gluDVT_QueryCloseUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataRow drCT = gvMain.GetDataRow(gvMain.FocusedRowHandle);
            if (drCT == null || drCT["Nhom"] == DBNull.Value)
                return;
            string nhom = drCT["Nhom"].ToString();
            if (nhom == "")
                e.Cancel = true;
            if (nhom != "FB" && nhom != "SB")
            {
                XtraMessageBox.Show("Mặt hàng này không được thay đổi đơn vị tính",
                    Config.GetValue("PackageName").ToString());
                e.Cancel = true;
            }
        }

        void cbeNLH_EditValueChanged(object sender, EventArgs e)
        {
            if (cbeNLH.EditValue == null || cbeNLH.EditValue.ToString() == "")
            {
                cbeDT.EditValue = null;
                return;
            }

            int i = cbeNLH.Properties.Items.IndexOf(cbeNLH.EditValue);
            if (i < 0 || i >= cbeDT.Properties.Items.Count)
                return;

            cbeDT.EditValue = cbeDT.Properties.Items[i];
        }

        void cbeNLH_QueryPopUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (gluKH == null || gluKH.EditValue == null || gluKH.EditValue.ToString() == "")
            {
                cbeNLH.Properties.Items.Clear();
                return;
            }

            DataTable dt = (gluKH.Properties.DataSource as BindingSource).DataSource as DataTable;
            DataRow[] drs = dt.Select("MaKH = '" + gluKH.EditValue.ToString() + "'");
            if (drs.Length == 0)
                return;

            cbeNLH.Properties.Items.Clear();
            cbeDT.Properties.Items.Clear();
            string nlh1 = drs[0]["NLH1"].ToString();
            string nlh2 = drs[0]["NLH2"].ToString();
            string nlh3 = drs[0]["NLH3"].ToString();
            string dt1 = drs[0]["DT1"].ToString();
            string dt2 = drs[0]["DT2"].ToString();
            string dt3 = drs[0]["DT3"].ToString();
            if (nlh1 != "")
            {
                cbeNLH.Properties.Items.Add(nlh1);
                cbeDT.Properties.Items.Add(dt1);
            }
            if (nlh2 != "")
            {
                cbeNLH.Properties.Items.Add(nlh2);
                cbeDT.Properties.Items.Add(dt2);
            }
            if (nlh3 != "")
            {
                cbeNLH.Properties.Items.Add(nlh3);
                cbeDT.Properties.Items.Add(dt3);
            }
        }

        void cbeDCGH_QueryPopUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (gluKH == null || gluKH.EditValue == null || gluKH.EditValue.ToString() == "")
            {
                cbeDCGH.Properties.Items.Clear();
                return;
            }

            DataTable dt = (gluKH.Properties.DataSource as BindingSource).DataSource as DataTable;
            DataRow[] drs = dt.Select("MaKH = '" + gluKH.EditValue.ToString() + "'");
            if (drs.Length == 0)
                return;

            cbeDCGH.Properties.Items.Clear();
            string dc1 = drs[0]["DiaChi1"].ToString();
            string dc2 = drs[0]["DiaChi2"].ToString();
            string dc3 = drs[0]["DiaChi3"].ToString();
            if (dc1 != "")
                cbeDCGH.Properties.Items.Add(dc1);
            if (dc2 != "")
                cbeDCGH.Properties.Items.Add(dc2);
            if (dc3 != "")
                cbeDCGH.Properties.Items.Add(dc3);
        }

        void gvMain_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            if (e.FocusedColumn.FieldName == "MaVT2" && gvMain.GetFocusedValue() == null)
                XtraMessageBox.Show("Vui lòng nhập phụ kiện kèm theo nếu có",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
        }

        void btnLamTron_Click(object sender, EventArgs e)
        {
            if (gvMain == null)
            {
                gcMain = data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl;
                gvMain = gcMain.MainView as GridView;
            }
            if (!gvMain.Editable)
            {
                XtraMessageBox.Show("Cần chuyển sang chế độ nhập/sửa số liệu để lưu",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                return;
            }
            for (int i = 0; i < gvMain.DataRowCount; i++)
            {
                DataRow dr = gvMain.GetDataRow(i);
                decimal tt = decimal.Parse(dr["TTCuoi"].ToString());
                decimal t1 = tt - Math.Round(tt / 1000, 0) * 1000;
                dr["TTCuoi"] = tt - t1;
            }
        }

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataSet ds = data.BsMain.DataSource as DataSet;
            if (ds == null)
                return;
            ds.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(NVKD_ColumnChanged);
            ds.Tables[1].ColumnChanged += new DataColumnChangeEventHandler(TinhChiPhi_ColumnChanged);
        }

        void NVKD_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName.ToUpper() == "MAKH")
            {
                TinhGiaCoDinh1();
                LayNVKD1();
            }
        }

        //void gluKH_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        //{
        //    if (data.BsMain == null || data.BsMain.Current == null)
        //        return;
        //    DataRowView drv = data.BsMain.Current as DataRowView;
        //    if (drv.Row.RowState == DataRowState.Deleted ||
        //        drv.Row.RowState == DataRowState.Unchanged)
        //        return;
        //    if (e.AcceptValue && e.CloseMode == PopupCloseMode.Normal)
        //    {
        //        //tính đơn giá cố định
        //        TinhGiaCoDinh1();
        //        LayNVKD1();
        //    }
        //}

        void TinhChiPhi_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Deleted)
                return;
            try
            {
                string cn = e.Column.ColumnName.ToUpper();
                //tính đơn giá cố định theo phân loại khách hàng + tính đơn giá hàng hóa phụ kiện
                //if (cn.Equals("DESCRIPTION"))
                //    LayNVKD2(e.Row);
                if (cn == "DESCRIPTION" || cn == "DAI" || cn == "RONG" || cn == "CAO" || cn == "MADVT" || cn == "SOLUONG"
                     || cn == "PE" || cn == "HS" || cn == "AT")
                    TinhGiaCoDinh2(e.Row, "Description");
                if (cn == "MAVT2" || cn == "DAIPK" || cn == "RONGPK" || cn == "CAOPK")
                    TinhGiaCoDinh2(e.Row, "MaVT2");

                //tính chi phí
                string nhom = e.Row["Nhom"].ToString();
                if (e.Column.ColumnName.ToUpper().Equals("HP") || e.Column.ColumnName.ToUpper().Equals("HS")
                    || e.Column.ColumnName.ToUpper().Equals("FT") || e.Column.ColumnName.ToUpper().Equals("AT"))
                {
                    decimal TT = 0, SL = 0;
                    if (decimal.Parse(e.Row[e.Column].ToString()) != 0 && nhom == "")
                        XtraMessageBox.Show("Mặt hàng này chưa phân nhóm, sẽ không tính được chi phí công nối!",
                            Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                    if (nhom == "SB")
                    {
                        if (e.Column.ColumnName.ToUpper().Equals("HS"))
                        {
                            SL = decimal.Parse(e.Row["HS"].ToString());
                            TT = SL * 500;
                            TT = Math.Max(TT, 10000);
                            e.Row["TTHS"] = SL == 0 ? 0 : TT;
                        }
                        if (e.Column.ColumnName.ToUpper().Equals("FT"))
                        {
                            SL = decimal.Parse(e.Row["FT"].ToString());
                            TT = SL * 1000;
                            TT = Math.Max(TT, 10000);
                            e.Row["TTFT"] = SL == 0 ? 0 : TT;
                        }
                        if (e.Column.ColumnName.ToUpper().Equals("AT"))
                        {
                            SL = decimal.Parse(e.Row["AT"].ToString());
                            TT = SL * 500;
                            TT = Math.Max(TT, 10000);
                            e.Row["TTAT"] = SL == 0 ? 0 : TT;
                            //cong them 1 trieu vao chi phi van chuyen
                            if (data.BsMain.Current != null)
                            {
                                DataRow drMaster = (data.BsMain.Current as DataRowView).Row;
                                object oat = e.Row.Table.Compute("sum(AT)", "MT63ID = '" + drMaster["MT63ID"].ToString() + "'");
                                drMaster["CPVC"] = Convert.ToDecimal(oat) > 0 ? 1000000 : 0;
                            }
                        }
                        if (e.Column.ColumnName.ToUpper().Equals("HP"))
                        {
                            SL = decimal.Parse(e.Row["HP"].ToString());
                            TT = SL * 1000000;
                            e.Row["TTOP"] = SL == 0 ? 0 : TT;
                        }

                    }
                    else if (nhom == "FB")
                    {
                        if (e.Column.ColumnName.ToUpper().Equals("HS"))
                        {
                            SL = decimal.Parse(e.Row["HS"].ToString());
                            TT = SL * 1000;
                            TT = Math.Max(TT, 10000);
                            e.Row["TTHS"] = SL == 0 ? 0 : TT;
                        }
                        if (e.Column.ColumnName.ToUpper().Equals("FT"))
                        {
                            SL = decimal.Parse(e.Row["FT"].ToString());
                            TT = SL * 1000;
                            TT = Math.Max(TT, 10000);
                            e.Row["TTFT"] = SL == 0 ? 0 : TT;
                        }
                        if (e.Column.ColumnName.ToUpper().Equals("AT"))
                        {
                            SL = decimal.Parse(e.Row["AT"].ToString());
                            TT = SL * 1000;
                            TT = Math.Max(TT, 10000);
                            e.Row["TTAT"] = SL == 0 ? 0 : TT;
                            //cong them 1 trieu vao chi phi van chuyen
                            if (data.BsMain.Current != null)
                            {
                                DataRow drMaster = (data.BsMain.Current as DataRowView).Row;
                                object oat = e.Row.Table.Compute("sum(AT)", "MT63ID = '" + drMaster["MT63ID"].ToString() + "'");
                                drMaster["CPVC"] = Convert.ToDecimal(oat) > 0 ? 1000000 : 0;
                            }
                        }
                        if (e.Column.ColumnName.ToUpper().Equals("HP"))
                        {
                            SL = decimal.Parse(e.Row["HP"].ToString());
                            TT = SL * 1000000;
                            e.Row["TTOP"] = SL == 0 ? 0 : TT;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Cập nhật đơn giá và chi phí: " + ex.Message, Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
            }
        }

        //tính khi thay đổi mã khách hàng
        private void TinhGiaCoDinh1()
        {
            if (gluKH.EditValue == null || gluKH.EditValue.ToString() == "")
                return;
            RepositoryItemGridLookUpEdit gluHH = gcMain.RepositoryItems["MaVT"] as RepositoryItemGridLookUpEdit;
            RepositoryItemGridLookUpEdit gluPK = gcMain.RepositoryItems["MaVT2"] as RepositoryItemGridLookUpEdit;
            BindingSource bs = gluKH.Properties.DataSource as BindingSource;
            if (bs == null || bs.DataSource.GetType() != typeof(DataTable))
                return;
            DataTable dt = bs.DataSource as DataTable;
            DataRow[] drs = dt.Select("MaKH = '" + gluKH.EditValue.ToString() + "'");
            if (drs.Length == 0)
                return;
            DataRow drKH = drs[0];
            if (drKH == null)
                return;
            string pl = drKH["Nhom1"].ToString();
            if (pl == "")
            {
                XtraMessageBox.Show("Khách hàng này chưa phân loại, không thể xác định giá bán cho khách hàng này!",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                return;
            }
            for (int i = 0; i < gvMain.DataRowCount; i++)
            {
                DataRow drCT = gvMain.GetDataRow(i);
                object mahh = drCT["MaVT"];
                bs = gluHH.DataSource as BindingSource;
                if (bs == null || bs.DataSource.GetType() != typeof(DataTable))
                    return;
                dt = bs.DataSource as DataTable;
                drs = dt.Select("MaVT = '" + mahh.ToString() + "'");
                if (drs.Length == 0)
                    return;
                DataRow drHH = drs[0];
                string nhom = drHH["Nhom"].ToString();
                if (nhom == "")
                    XtraMessageBox.Show("Mặt hàng " + drHH["TenVT2"].ToString() + " chưa phân nhóm, có thể ảnh hưởng đến cách tính giá bán!",
                        Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                object mapk = drCT["MaVT2"];
                int k = gluPK.GetIndexByKeyValue(mapk);
                DataRow drPK = null;
                if (k >= 0)
                    drPK = gluPK.View.GetDataRow(k);
                object ghh, gpk;
                switch (pl)
                {
                    case "OEM":
                    case "TRA":
                        ghh = drHH == null ? 0 : drHH["GiaOME"];
                        gpk = drPK == null ? 0 : drPK["GiaOME"];
                        break;
                    case "DIS":
                        ghh = drHH == null ? 0 : drHH["GiaDTB"];
                        gpk = drPK == null ? 0 : drPK["GiaDTB"];
                        break;
                    default:
                        ghh = drHH == null ? 0 : drHH["GiaBan"];
                        gpk = drPK == null ? 0 : drPK["GiaBan"];
                        break;
                }
                drCT["GiaGoc"] = ghh;
                drCT["DGPKGoc"] = gpk;
                decimal gg = decimal.Parse(drCT["GiaGoc"].ToString());
                #region cách tính cũ
                //if (drHH["MaDVT"].ToString() != "7") //mã của dvt m2
                //    drCT["Gia"] = gg;
                //else
                //{
                //decimal d = decimal.Parse(drCT["Cao"].ToString());
                //decimal r = decimal.Parse(drCT["Rong"].ToString());
                //if (nhom == "HPFB" || nhom == "SB")
                //    drCT["Gia"] = r == 0 ? gg : (r * (d + 100) / 1000000) * gg;
                //else
                //    drCT["Gia"] = r == 0 ? gg : (r * d / 1000000) * gg;
                //}
                #endregion
                decimal sl = decimal.Parse(drCT["SoLuong"].ToString());
                if (nhom == "FB" || nhom == "SB")
                {
                    decimal d = decimal.Parse(drCT["Cao"].ToString());
                    decimal r = decimal.Parse(drCT["Rong"].ToString());
                    decimal t = Convert.ToDecimal(drCT["PE"]) + Convert.ToDecimal(drCT["HS"]) + Convert.ToDecimal(drCT["AT"]);
                    if (t > 0)
                        d = (nhom == "SB") ? d + 120 : d + 80;
                    string dvt = drCT["MaDVT"].ToString();
                    if (dvt == "7")   //m2
                    {
                        drCT["Ps"] = (d * r) / 1000000 * gg;
                        if (sl > 0)
                            drCT["Gia"] = Convert.ToDecimal(drCT["Ps"]) / sl;
                        else
                            drCT["Gia"] = 0;
                    }
                    else
                        if (dvt == "10")  //m
                        {
                            drCT["Ps"] = (d * r) / 1000000 * gg;
                            if (d > 0)
                                drCT["Gia"] = Convert.ToDecimal(drCT["Ps"]) / d * 1000;
                            else
                                drCT["Gia"] = 0;
                        }
                        else
                        {
                            drCT["Ps"] = (d * r) / 1000000 * gg * sl;
                            if (sl > 0)
                                drCT["Gia"] = Convert.ToDecimal(drCT["Ps"]) / sl;
                            else
                                drCT["Gia"] = 0;
                        }
                }
                else
                {
                    drCT["Gia"] = gg;
                    drCT["Ps"] = decimal.Parse(drCT["Gia"].ToString()) * sl;
                }
                //gía phụ kiện
                gg = decimal.Parse(drCT["DGPKGOC"].ToString());
                if (drPK != null && drPK["MaDVT"].ToString() != "7") //mã của dvt m2
                    drCT["DGPK"] = gg;
                else
                {
                    decimal r = decimal.Parse(drCT["RongPK"].ToString());
                    decimal d = decimal.Parse(drCT["CaoPK"].ToString());
                    drCT["DGPK"] = r == 0 ? gg : (r * d / 1000000) * gg;
                }
            }
        }

        //cập nhật NVKD khi chọn/thay đổi khách hàng
        private void LayNVKD1()
        {
            if (gluKH.EditValue == null || gluKH.EditValue.ToString() == "")
                return;
            BindingSource bs = gluKH.Properties.DataSource as BindingSource;
            if (bs == null || bs.DataSource.GetType() != typeof(DataTable))
                return;
            DataTable dt = bs.DataSource as DataTable;
            DataRow[] drs = dt.Select("MaKH = '" + gluKH.EditValue.ToString() + "'");
            if (drs.Length == 0)
                return;
            DataRow drKH = drs[0];
            if (drKH["MaNV"] == DBNull.Value)
            {
                XtraMessageBox.Show("Khách hàng này chưa phân sales, không thể tính doanh số cho salesman!",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
            }
            RepositoryItemGridLookUpEdit gluHH = gcMain.RepositoryItems["MaVT"] as RepositoryItemGridLookUpEdit;
            bs = gluHH.DataSource as BindingSource;
            if (bs == null || bs.DataSource.GetType() != typeof(DataTable))
                return;
            dt = bs.DataSource as DataTable;
            for (int i = 0; i < gvMain.DataRowCount; i++)
            {
                DataRow drCT = gvMain.GetDataRow(i);
                if (drKH["MaNV"] == DBNull.Value)
                    drCT["NVKD"] = DBNull.Value;
                else
                    drCT["NVKD"] = drKH["MaNV"];
            }
            //            if (drKH == null)
            //                return;
            //            string pl = drKH["Nhom1"].ToString();
            //            if (pl == "")
            //            {
            //                XtraMessageBox.Show("Khách hàng này chưa phân loại, không thể tính doanh số cho salesman!",
            //                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
            //            }
            //            string kv = drKH["KhuVuc"].ToString();
            //            if (kv == "")
            //            {
            //                XtraMessageBox.Show("Khách hàng này chưa phân khu vực, không thể tính doanh số cho salesman!",
            //                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
            //            }
            //            RepositoryItemGridLookUpEdit gluHH = gcMain.RepositoryItems["MaVT"] as RepositoryItemGridLookUpEdit;
            //            bs = gluHH.DataSource as BindingSource;
            //            if (bs == null || bs.DataSource.GetType() != typeof(DataTable))
            //                return;
            //            dt = bs.DataSource as DataTable;
            //            for (int i = 0; i < gvMain.DataRowCount; i++)
            //            {
            //                DataRow drCT = gvMain.GetDataRow(i);
            //                if (pl == "" || kv == "")
            //                {
            //                    drCT["NVKD"] = DBNull.Value;
            //                    continue;
            //                }
            //                object mahh = drCT["MaVT"];
            //                drs = dt.Select("MaVT = '" + mahh.ToString() + "'");
            //                if (drs.Length == 0)
            //                    continue;
            //                DataRow drHH = drs[0];
            //                string nhom = drHH["Nhom"].ToString();
            //                if (nhom == "")
            //                {
            //                    XtraMessageBox.Show("Mặt hàng " + drHH["TenVT2"].ToString() + " chưa phân nhóm, không thể tính doanh số cho salesman!",
            //                        Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
            //                    drCT["NVKD"] = DBNull.Value;
            //                    continue;
            //                }
            //                DataTable dtNVKD = db.GetDataTable(string.Format(@"select MaNV from NVKDTheoKV where (DMTP like '{0},%' or DMTP like '%,{0}' or DMTP like '%,{0},%' or DMTP = '{0}') 
            //                    and (NhomKH like '{1},%' or NhomKH like '%,{1}' or NhomKH like '%,{1},%' or NhomKH = '{1}')
            //                    and (NhomVT like '{2},%' or NhomVT like '%,{2}' or NhomVT like '%,{2},%' or NhomVT = '{2}')", kv, pl, nhom));
            //                if (dtNVKD.Rows.Count == 0)
            //                {
            //                    XtraMessageBox.Show(string.Format("Khu vực {0}, loại khách hàng {1} và nhóm hàng {2} hiện chưa phân sales", kv, pl, nhom),
            //                        Config.GetValue("PackageName").ToString());
            //                    drCT["NVKD"] = DBNull.Value;
            //                    continue;
            //                }
            //                if (dtNVKD.Rows.Count > 1)
            //                {
            //                    XtraMessageBox.Show(string.Format("Khu vực {0}, loại khách hàng {1} và nhóm hàng {2} hiện đang phân nhiều sales cùng lúc", kv, pl, nhom),
            //                        Config.GetValue("PackageName").ToString());
            //                    drCT["NVKD"] = DBNull.Value;
            //                    continue;
            //                }
            //                drCT["NVKD"] = dtNVKD.Rows[0]["MaNV"];
            //            }
        }

        //cập nhật NVKD khi chọn mặt hàng
        //        private void LayNVKD2(DataRow drCT)
        //        {
        //            RepositoryItemGridLookUpEdit gluHH = gcMain.RepositoryItems["Description"] as RepositoryItemGridLookUpEdit;
        //            if (gluHH.OwnerEdit != null && gluHH.OwnerEdit.IsPopupOpen)
        //                return;
        //            if (gluKH.EditValue == null || gluKH.EditValue.ToString() == "")
        //                return;
        //            BindingSource bs = gluKH.Properties.DataSource as BindingSource;
        //            if (bs == null || bs.DataSource.GetType() != typeof(DataTable))
        //                return;
        //            DataTable dt = bs.DataSource as DataTable;
        //            DataRow[] drs = dt.Select("MaKH = '" + gluKH.EditValue.ToString() + "'");
        //            if (drs.Length == 0)
        //                return;
        //            DataRow drKH = drs[0];
        //            if (drKH == null)
        //                return;
        //            string pl = drKH["Nhom1"].ToString();
        //            if (pl == "")
        //            {
        //                XtraMessageBox.Show("Khách hàng này chưa phân loại, không thể tính doanh số cho salesman!",
        //                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
        //                drCT["NVKD"] = DBNull.Value;
        //                return;
        //            }
        //            string kv = drKH["KhuVuc"].ToString();
        //            if (kv == "")
        //            {
        //                XtraMessageBox.Show("Khách hàng này chưa phân khu vực, không thể tính doanh số cho salesman!",
        //                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
        //                drCT["NVKD"] = DBNull.Value;
        //                return;
        //            }
        //            object mahh = drCT["Description"];
        //            bs = gluHH.DataSource as BindingSource;
        //            if (bs == null || bs.DataSource.GetType() != typeof(DataTable))
        //                return;
        //            dt = bs.DataSource as DataTable;
        //            drs = dt.Select("PNo = '" + mahh.ToString() + "'");
        //            if (drs.Length == 0)
        //                return;
        //            DataRow drHH = drs[0];
        //            string nhom = drHH["Nhom"].ToString();
        //            if (nhom == "")
        //            {
        //                XtraMessageBox.Show("Mặt hàng " + drHH["TenVT2"].ToString() + " chưa phân nhóm, không thể tính doanh số cho salesman!",
        //                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
        //                drCT["NVKD"] = DBNull.Value;
        //                return;
        //            }
        //            dt = db.GetDataTable(string.Format(@"select MaNV from NVKDTheoKV where (DMTP like '{0},%' or DMTP like '%,{0}' or DMTP like '%,{0},%' or DMTP = '{0}') 
        //                    and (NhomKH like '{1},%' or NhomKH like '%,{1}' or NhomKH like '%,{1},%' or NhomKH = '{1}')
        //                    and (NhomVT like '{2},%' or NhomVT like '%,{2}' or NhomVT like '%,{2},%' or NhomVT = '{2}')", kv, pl, nhom));
        //            if (dt.Rows.Count == 0)
        //            {
        //                XtraMessageBox.Show(string.Format("Khu vực {0}, loại khách hàng {1} và nhóm hàng {2} hiện chưa phân sales", kv, pl, nhom),
        //                    Config.GetValue("PackageName").ToString());
        //                drCT["NVKD"] = DBNull.Value;
        //                return;
        //            }
        //            if (dt.Rows.Count > 1)
        //            {
        //                XtraMessageBox.Show(string.Format("Khu vực {0}, loại khách hàng {1} và nhóm hàng {2} hiện đang phân nhiều sales cùng lúc", kv, pl, nhom),
        //                    Config.GetValue("PackageName").ToString());
        //                drCT["NVKD"] = DBNull.Value;
        //                return;
        //            }
        //            drCT["NVKD"] = dt.Rows[0]["MaNV"];
        //        }

        //tính khi thay đổi hàng hóa hoặc phụ kiện
        private void TinhGiaCoDinh2(DataRow drCT, string cl)
        {
            RepositoryItemGridLookUpEdit gluHH = gcMain.RepositoryItems[cl] as RepositoryItemGridLookUpEdit;
            RepositoryItemGridLookUpEdit gluPK = gcMain.RepositoryItems["MaVT2"] as RepositoryItemGridLookUpEdit;
            if (gluHH.OwnerEdit != null && gluHH.OwnerEdit.IsPopupOpen)
                return;
            if (gluKH.EditValue == null || gluKH.EditValue.ToString() == "")
            {
                XtraMessageBox.Show("Chưa chọn khách hàng, không thể lấy được giá bán!",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                return;
            }
            //int n = gluKH.Properties.GetIndexByKeyValue(gluKH.EditValue);
            //if (n < 0)
            //    return;
            BindingSource bs = gluKH.Properties.DataSource as BindingSource;
            if (bs == null || bs.DataSource.GetType() != typeof(DataTable))
                return;
            DataTable dt = bs.DataSource as DataTable;
            DataRow[] drs = dt.Select("MaKH = '" + gluKH.EditValue.ToString() + "'");
            if (drs.Length == 0)
                return;
            DataRow drKH = drs[0];
            if (drKH == null)
                return;
            string pl = drKH["Nhom1"].ToString();
            if (pl == "")
            {
                XtraMessageBox.Show("Khách hàng này chưa phân loại, không thể lấy được giá bán cho khách hàng này!",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                return;
            }
            object mahh = drCT[cl];
            //object code = drCT["Description"];
            bs = gluHH.DataSource as BindingSource;
            if (bs == null || bs.DataSource.GetType() != typeof(DataTable))
                return;
            dt = bs.DataSource as DataTable;
            drs = dt.Select("MaVT = '" + mahh.ToString() + "' or PNo = '" + mahh.ToString() + "'");
            if (drs.Length == 0)
                return;
            DataRow drHH = drs[0];
            string nhom = drHH["Nhom"].ToString();
            if (nhom == "")
                XtraMessageBox.Show("Mặt hàng " + drHH["TenVT2"].ToString() + " chưa phân nhóm, có thể ảnh hưởng đến cách tính giá bán!",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
            object mapk = drCT["MaVT2"];
            int k = gluPK.GetIndexByKeyValue(mapk);
            DataRow drPK = null;
            if (k >= 0)
                drPK = gluPK.View.GetDataRow(k);
            object ghh, gpk;
            switch (pl)
            {
                case "OEM":
                case "TRA":
                    ghh = drHH == null ? 0 : drHH["GiaOME"];
                    gpk = drPK == null ? 0 : drPK["GiaOME"];
                    break;
                case "DIS":
                    ghh = drHH == null ? 0 : drHH["GiaDTB"];
                    gpk = drPK == null ? 0 : drPK["GiaDTB"];
                    break;
                default:
                    ghh = drHH == null ? 0 : drHH["GiaBan"];
                    gpk = drPK == null ? 0 : drPK["GiaBan"];
                    break;
            }
            if (cl == "Description")
            {
                string dvt = drCT["MaDVT"].ToString();
                if (dvt == "")      //chua co dvt thi chua tinh gia ban
                    return;
                drCT["GiaGoc"] = ghh;
                decimal gg = decimal.Parse(drCT["GiaGoc"].ToString());
                #region Cách tính cũ
                //if (drHH["MaDVT"].ToString() != "7") //mã của dvt m2
                //    drCT["Gia"] = gg;
                //else
                //{
                //    decimal r = decimal.Parse(drCT["Rong"].ToString());
                //    decimal d = decimal.Parse(drCT["Cao"].ToString());
                //    if (nhom == "HPFB" || nhom == "SB")
                //        drCT["Gia"] = r == 0 ? gg : (r * (d + 100) / 1000000) * gg;
                //    else
                //        drCT["Gia"] = r == 0 ? gg : (r * d / 1000000) * gg;
                //}
                #endregion
                decimal sl = decimal.Parse(drCT["SoLuong"].ToString());
                if (nhom == "FB" || nhom == "SB")
                {
                    decimal d = decimal.Parse(drCT["Cao"].ToString());
                    decimal r = decimal.Parse(drCT["Rong"].ToString());
                    decimal t = Convert.ToDecimal(drCT["PE"]) + Convert.ToDecimal(drCT["HS"]) + Convert.ToDecimal(drCT["AT"]);
                    if (t > 0)
                        d = (nhom == "SB") ? d + 120 : d + 80;
                    if (dvt == "7")   //m2
                    {
                        drCT["Ps"] = (d * r) / 1000000 * gg;
                        if (sl > 0)
                            drCT["Gia"] = Convert.ToDecimal(drCT["Ps"]) / sl;
                        else
                            drCT["Gia"] = 0;
                    }
                    else
                        if (dvt == "10")  //m
                        {
                            drCT["Ps"] = (d * r) / 1000000 * gg;
                            if (d > 0)
                                drCT["Gia"] = Convert.ToDecimal(drCT["Ps"]) / d * 1000;
                            else
                                drCT["Gia"] = 0;
                        }
                        else
                        {
                            drCT["Ps"] = (d * r) / 1000000 * gg * sl;
                            if (sl > 0)
                                drCT["Gia"] = Convert.ToDecimal(drCT["Ps"]) / sl;
                            else
                                drCT["Gia"] = 0;
                        }
                }
                else
                {
                    drCT["Gia"] = gg;
                    drCT["Ps"] = decimal.Parse(drCT["Gia"].ToString()) * sl;
                }
            }
            else
            {
                drCT["DGPKGoc"] = gpk;
                decimal gg = decimal.Parse(drCT["DGPKGOC"].ToString());
                if (drPK["MaDVT"].ToString() != "7") //mã của dvt m2
                    drCT["DGPK"] = gg;
                else
                {
                    decimal r = decimal.Parse(drCT["RongPK"].ToString());
                    decimal d = decimal.Parse(drCT["CaoPK"].ToString());
                    drCT["DGPK"] = r == 0 ? gg : (r * d / 1000000) * gg;
                }
            }
        }

        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        #endregion
    }
}
