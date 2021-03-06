using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using CDTLib;
using System.Globalization;

namespace Inventory
{
    public partial class GiaVT : DevExpress.XtraEditors.XtraForm
    {
        private string _tkCL = "";
        private string _condition = "";
        public string makhoapgia = null;
        private int _tuThang;
        private int _denThang;
        private int _selectedIndex;

        public GiaVT(int tuthang, int denthang, int selectedIndex,string makho, string condition, string tkCL)
        {
            _tuThang = tuthang;
            _denThang = denthang;
            _condition = condition;
            _tkCL = tkCL;
            if (makho !=null)
                makhoapgia = makho;
            if (_tuThang > _denThang || _tkCL == "")
            {
                string msg = "Thông tin không hợp lệ!";
                if (Config.GetValue("Language").ToString() == "1")
                    msg = UIDictionary.Translate(msg);
                XtraMessageBox.Show(msg);
                return;
            }
            _selectedIndex = selectedIndex;
            InitializeComponent();
        }
        private void gridControlGiaVT_Load(object sender, EventArgs e)
        {
            Database dbData = Database.NewDataDatabase();
            dbData.BeginMultiTrans();
            ChenhLechTonKho cltk;
            if (_selectedIndex == 0)
            {
                if (_condition != "")
                    _condition += " and ";
                _condition += "MaVT in (select MaVT from DMVT where TonKho = 3)";
                cltk = new ChenhLechTonKho(dbData, _condition, _tkCL, _denThang);
                cltk.XoaButToan();
                GiaTrungBinh gtb = new GiaTrungBinh(dbData, _tuThang, _denThang, makhoapgia, _condition);
                Cursor.Current = Cursors.WaitCursor;
                //int solantinh = gtb.solantinh();    //tinh nhieu lan cho truong hop co dieu chuyen kho
                //for (int i = 0; i < solantinh + 1; i++)
                //{
                    gtb.TinhGia();
                //}
                Cursor.Current = Cursors.Default;
                gridControlGiaVT.DataSource = gtb.DtVatTu;
                gridColumn6.Visible = false;
                gridColumn7.Visible = false;
            }
            else
            {
                if (_selectedIndex == 1)
                {
                    if (_condition != "")
                        _condition += " and ";
                    _condition += "MaVT in (select MaVT from DMVT where TonKho = 2)";
                    cltk = new ChenhLechTonKho(dbData, _condition, _tkCL, _denThang);
                    cltk.XoaButToan();
                    GiaNTXT gntxt = new GiaNTXT(dbData, _tuThang, _denThang, _condition);
                    Cursor.Current = Cursors.WaitCursor;
                    gntxt.TinhGia();
                    Cursor.Current = Cursors.Default;
                    gridControlGiaVT.DataSource = gntxt.DtVatTu;
                    gridColumn4.FieldName = "PsCo";
                    gridColumn3.FieldName = "SoLuong_x";
                }
                else
                {
                    if (_condition != "")
                        _condition += " and ";
                    _condition += "MaVT in (select MaVT from DMVT where TonKho = 4)";
                    cltk = new ChenhLechTonKho(dbData, _condition, _tkCL, _denThang);
                    cltk.XoaButToan();
                    CultureInfo ci = Application.CurrentCulture;
                    GiaBQDD bqdd = new GiaBQDD(dbData, _tuThang, _denThang, _condition);
                    Cursor.Current = Cursors.WaitCursor;
                    bqdd.TinhGia();
                    Cursor.Current = Cursors.Default;
                    gridControlGiaVT.DataSource = bqdd.DtVatTu;
                    gridColumn4.FieldName = "PsCo";
                    gridColumn3.FieldName = "SoLuong_x";
                    Application.CurrentCulture = ci;
                }
            }
            cltk.XuLy();
            if (!dbData.HasErrors)
                dbData.EndMultiTrans();
            if (Config.GetValue("Language").ToString() == "1")
                FormFactory.DevLocalizer.Translate(this);
        }

        private void gridControlGiaVT_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

    }
}