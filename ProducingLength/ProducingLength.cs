using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;

namespace ProducingLength
{
    public class ProducingLength : ICData
    {
        private DataCustomData data;
        private InfoCustomData info;

        public ProducingLength()
        {
            info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        public DataCustomData Data
        {
            set { data = value; }
        }

        public void ExecuteAfter()
        {

        }

        public void ExecuteBefore()
        {
            if (data.CurMasterIndex < 0)
                return;
            DataRow dr = data.DsData.Tables[0].Rows[data.CurMasterIndex];
            if (dr.RowState == DataRowState.Deleted)
                return;
            bool pe = Boolean.Parse(dr["PrepareEnd"].ToString());
            bool hot = Boolean.Parse(dr["Hot"].ToString()) || pe;
            bool cut = Boolean.Parse(dr["Cut"].ToString());
            DataView dv = new DataView(data.DsData.Tables[1]);
            if (dr.RowState == DataRowState.Added)
                dv.RowStateFilter = DataViewRowState.Added;
            else
                dv.RowFilter = "DNID = '" + dr["DNID"].ToString() + "'";
            foreach (DataRowView drv in dv)
            {
                if (drv["Nhom"] == DBNull.Value || drv["ChieuCao"] == DBNull.Value)
                    continue;
                string n = drv["Nhom"].ToString();
                decimal dai = Decimal.Parse(drv["ChieuCao"].ToString());
                decimal sx = 0;
                if (hot && n == "SB")
                    sx = 150;
                else
                {
                    if (cut && n == "SB")
                        sx = 0;
                    else
                    {
                        if (hot && n == "FB")
                            sx = 120;
                        else
                        {
                            if (cut && n == "FB")
                                sx = 0;
                            else
                                if (n == "EC")
                                    sx = 20;
                        }
                    }
                }
                drv.Row["ChieuDaiSX"] = dai + sx;
            }
        }

        public InfoCustomData Info
        {
            get { return info; }
        }
    }
}
