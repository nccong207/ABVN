using CDTLib;
using DevExpress.XtraEditors;
using Plugins;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace XuLyNamTC
{
    public class XuLyNamTC : ICControl
    {
        DateEdit deNgayCT, deThoiHan;
        
        private DataCustomFormControl _data;
        private InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
        bool isShown = false;
        public DataCustomFormControl Data
        {
            set { _data = value; }
        }

        public InfoCustomControl Info
        {
            get { return _info; }
        }

        public void AddEvent()
        {
            deNgayCT = _data.FrmMain.Controls.Find("NgayCT", true)[0] as DateEdit;
            deThoiHan = _data.FrmMain.Controls.Find("ThoiHan", true)[0] as DateEdit;
            //deNgayCT.EditValue = DateTime.Now;
            _data.FrmMain.Shown += FrmMain_Shown;
            _data.FrmMain.FormClosed += FrmMain_FormClosed;
            deNgayCT.EditValueChanged += DeNgayCT_EditValueChanged;
        }

        private void FrmMain_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            isShown = false;
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            isShown = true;
            DeNgayCT_EditValueChanged(deNgayCT, new EventArgs());
        }

        private void DeNgayCT_EditValueChanged(object sender, EventArgs e)
        {
            if (!isShown)
            {
                return;
            }

            if (!deNgayCT.Properties.ReadOnly)
            {
                if (deNgayCT.EditValue == null)
                {
                    return;
                }

                var current = DateTime.ParseExact(deNgayCT.EditValue.ToString(), "MM/dd/yyyy hh:mm:ss", null);

                if (Config.GetValue("NamLamViec") != null)
                {
                    int year = int.Parse(Config.GetValue("NamLamViec").ToString());
                    int month = int.Parse(Config.GetValue("KyKeToan").ToString());

                    if (year != current.Year || month != current.Month)
                    {
                        XtraMessageBox.Show("Bạn đang nhập số liệu không thuộc năm tài chính hoặc kỳ kế toán hiện tại. \nVui lòng kiểm tra lại.", Config.GetValue("PackageName").ToString());
                        return;
                    }
                }

                deThoiHan.EditValue = current.AddDays(60);

            }
        }
    }
}
