using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using CDTLib;
using Plugins;
using CBSControls;
using FormFactory;
using DevExpress.XtraEditors;
using System.Data;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Windows.Forms;
using CDTControl;
using System.IO;
using System.Diagnostics;

namespace LayHoaHong
{
    public class LayHoaHong : ICControl
    {

        Database db = Database.NewDataDatabase();
        InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        DataCustomFormControl data;
        GridView gvMain;
        #region ICControl Members

        CheckEdit ckHoaHong;
        CalcEdit calTienHH;
        public void AddEvent()
        {
            gvMain = (data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            ckHoaHong = data.FrmMain.Controls.Find("DaChiHH",true) [0] as CheckEdit;
            ckHoaHong.EditValueChanged += new EventHandler(ckHoaHong_EditValueChanged);
            calTienHH = data.FrmMain.Controls.Find("DaChi", true)[0] as CalcEdit ;
            LayoutControl lcMain = data.FrmMain.Controls.Find("lcMain", true)[0] as LayoutControl;

            SimpleButton btnXuatFile = new SimpleButton();
            btnXuatFile.Text = "Xuất ra Excel";
            btnXuatFile.Name = "btnXuatFile";
            btnXuatFile.Click += new EventHandler(btnXuatFile_Click);
            LayoutControlItem lci2 = lcMain.AddItem("", btnXuatFile);
            lci2.Name = "cusXuatFile";
        }

        private void btnXuatFile_Click(object sender, EventArgs e)
        {
            if (gvMain.Editable)
            {
                XtraMessageBox.Show("Vui lòng thực hiện khi đã lưu hóa đơn bán hàng",
                    Config.GetValue("PackageName").ToString());
                return;
            }

            DataRow drCurrent = (data.BsMain.Current as DataRowView).Row;

            //if (!bool.Parse(drCurrent["Duyet"].ToString()))
            //{
            //    XtraMessageBox.Show("Chỉ xuất file Excel đối với hóa đơn đã duyệt.",
            //        Config.GetValue("PackageName").ToString());
            //    return;
            //}

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.RestoreDirectory = true;
            sfd.FileName = drCurrent["SoHoaDon"].ToString();
            sfd.Filter = "Excel files (*.xls)|*.xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string sql = @"SELECT m.SoHoaDon, m.NgayCT, m.OngBa, m.TenKH, m.DiaChi, MST = '''' + kh.MST, d.TenVT,
                                d.Dai, d.Rong, d.Day,
                                dvt.TenDVT, d.SoLuong, d.Gia, d.PS, th.ThueSuat, 
                                ROUND((d.PS * th.ThueSuat)/100, 0) as TienThue, ROUND((d.PS * (th.ThueSuat + 100))/100, 0) as TongTien, kh.Email,
                                m.MaKH,  m.So_PGH, m.SoSO, d.SoPO
                                FROM MT32 m JOIN DT32 d ON m.MT32ID = d.MT32ID	
                                LEFT JOIN DMKH kh ON m.MaKH = kh.MaKH
                                LEFT JOIN DMDVT dvt ON d.MaDVT = dvt.MaDVT
                                LEFT JOIN DMThueSuat th ON m.MaThue = th.MaThue
                            WHERE m.MT32ID = '{0}' ORDER BY d.Stt";
                Database db = Database.NewDataDatabase();
               
                DataTable dtData = db.GetDataTable(string.Format(sql, drCurrent["MT32ID"]));
                string f = Application.StartupPath + "\\Reports\\HTA\\MauHoaDon.xls";

                ExportExcel exportExcel = new ExportExcel(f, sfd.FileName, dtData);
                if (exportExcel.Export() && File.Exists(sfd.FileName))
                    Process.Start(sfd.FileName);
            }
        }

        void ckHoaHong_EditValueChanged(object sender, EventArgs e)
        {
            if (ckHoaHong.Properties.ReadOnly)
                return;
            if (data.BsMain == null)
                return;
            DataRow drMaster = (data.BsMain.Current as DataRowView).Row;
            drMaster["DaChiHH"] = ckHoaHong.EditValue;
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            if (ckHoaHong.Checked)
            {
                if (drMaster["SoSO"] != DBNull.Value || drMaster["SoSO"].ToString() != "")
                {
                    decimal TTien = (decimal)drMaster["TtienH"];
                    string sql = @" --SELECT isnull(HoaHong,0) HoaHong FROM MTSO WHERE SoPhieuDN = '{0}'
                                    SELECT 
                                             bg.hoahong
                                            ,bg.TtienSGG 
                                    FROM MTSO so inner join MT63 bg on so.SoBG = bg.SoCT 
                                    WHERE so.sophieudn = '{0}'                
                                    ";
                    //hvkhoi 
//                    string sql = @"
//                                    SELECT  CAST(bg.HoaHong as Float) / CAST(bg.TTien as Float) * CAST(so.TTien as Float) [HoaHong]
//                                            ,bg.TtienSGG 
//                                    FROM MTSO so inner join MT63 bg on so.SoBG = bg.SoCT 
//                                    WHERE so.sophieudn = '{0}'                
//                                    ";
                    //end hvkhoi
                    DataTable dt = db.GetDataTable(string.Format(sql, drMaster["SoSO"].ToString()));
                    if (dt.Rows.Count > 0)
                    {
                        decimal TienHH = (decimal)dt.Rows[0]["HoaHong"];
                        decimal TtienSGG = (decimal)dt.Rows[0]["TtienSGG"];
                        //calTienHH.EditValue = (decimal)dt.Rows[0]["HoaHong"] / 100 * TTien;
                        if (TienHH < 100)
                            calTienHH.EditValue = TTien * TienHH / 100;
                        else
                            calTienHH.EditValue = TTien / TtienSGG * TienHH;
                    } 
                }
            }
            else
                calTienHH.EditValue = 0;
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
