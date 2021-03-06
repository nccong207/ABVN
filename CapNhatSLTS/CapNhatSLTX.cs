using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using DevExpress.XtraEditors;
using CDTLib;

namespace CapNhatSLTX
{
    public class CapNhatSLTX : ICData
    {
        DataCustomData _data;
        InfoCustomData _info;
        public CapNhatSLTX()
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
            if (drCur.RowState == DataRowState.Deleted)
                return;
            string mtid = drCur["MTGCID"].ToString();
            DataTable dtgc = _data.DsData.Tables[1];
            DataView dvgc = new DataView(dtgc);
            dvgc.RowFilter = "MTGCID = '" + mtid + "' AND KC = 0"; // Mod by Đức: Removed 'RongB is not null and RongB <> 0'
            DataTable dtgcct = _data.DsData.Tables[2];
            
            List<string> tmp = new List<string>();
            bool rs = true;
                        
            foreach (DataRowView drv in dvgc)
            {
                string code = drv["Code"].ToString();
                if (tmp.Contains(code))
                    continue;
                DataRow[] drs = dtgcct.Select("MTGCID = '" + mtid + "' and Code = '" + code + "'");
                if (drs.Length == 0)
                {
                    XtraMessageBox.Show("Số lượng thực xuất của " + code + " chưa được nhập trong chi tiết cắt theo bậc.\nPhiếu xuất này không được lưu!",
                        Config.GetValue("PackageName").ToString());
                    rs = false;
                    break;
                }
                tmp.Add(code);
                object ox = null;
                object on = null;
                if (drv["RongB"].ToString() == "" || Convert.ToDecimal(drv["RongB"]) == 0)
                {
                    ox = dtgcct.Compute("sum(SL)", "MTGCID = '" + mtid + "' and Code = '" + code + "' and NX = 'X'");
                    on = dtgcct.Compute("sum(SL)", "MTGCID = '" + mtid + "' and Code = '" + code + "' and NX = 'N'");
                }
                else
                {
                    ox = dtgcct.Compute("sum(SLQD)", "MTGCID = '" + mtid + "' and Code = '" + code + "' and NX = 'X'");
                    on = dtgcct.Compute("sum(SLQD)", "MTGCID = '" + mtid + "' and Code = '" + code + "' and NX = 'N'");
                }
                decimal slqd = 0;
                if (ox != null && ox.ToString() != "")
                    slqd = decimal.Parse(ox.ToString()) - ((on == null || on.ToString() == "") ? 0 : decimal.Parse(on.ToString()));
                object o = dtgc.Compute("sum(SLQuyDoi)", "MTGCID = '" + mtid + "' and Code = '" + code + "'");
                decimal l = Math.Round(Math.Abs(decimal.Parse(o.ToString()) - slqd), 3);
                if (o == null || o.ToString() == "" || l > 0.002M)
                {
                    XtraMessageBox.Show("Số lượng thực xuất của " + code + " chưa khớp. Phiếu xuất này không được lưu!\n" +
                        string.Format("Theo SO = {0}, theo bậc = {1}, lệch = {2}.", decimal.Parse(o.ToString()).ToString("###,##0.000"), slqd.ToString("###,##0.000"), l.ToString("###,##0.000")),
                        Config.GetValue("PackageName").ToString());
                    rs = false;
                    break;
                }
            }
            _info.Result = rs;
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
