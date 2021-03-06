using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using DevExpress.XtraEditors;
using CDTLib;

namespace HuyPGH
{
    public class HuyPGH : ICData
    {
        DataCustomData _data;
        InfoCustomData _info;
        public HuyPGH()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }
        #region ICData Members

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            
        }

        public void ExecuteBefore()
        {
            DataRow drCur = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drCur.RowState != DataRowState.Modified)
                return;
            _info.Result = true;
            if ((Boolean.Parse(drCur["Huy", DataRowVersion.Current].ToString()) == true)
                && (Boolean.Parse(drCur["Huy", DataRowVersion.Original].ToString()) == false))
            {
                string sohd = drCur["SoPX"].ToString();
                if (sohd != "")
                {
                    _info.Result = false;
                    XtraMessageBox.Show("Phiếu đã xuất hóa đơn, không thể hủy. Cần hủy hóa đơn trước",
                        Config.GetValue("PackageName").ToString());
                    return;
                }
                else
                {
                    string soso = drCur["SoSO"].ToString();
                    if (soso != "")
                        _data.DbData.UpdateByNonQuery("update MTSO set sophieu = null, TinhTrang = 3 where SoPhieuDN = '" + soso + "'");
                }
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
