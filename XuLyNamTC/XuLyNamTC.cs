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
        DateEdit deNgayCT;
        private DataCustomFormControl _data;
        private InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
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
            deNgayCT.EditValueChanged += DeNgayCT_EditValueChanged;
        }

        private void DeNgayCT_EditValueChanged(object sender, EventArgs e)
        {

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

            }
        }
    }
}
