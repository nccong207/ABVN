using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using DevExpress.XtraEditors;
using CDTLib;
using System.Windows.Forms;
using System.Data;

namespace TaoNhomNhap
{
    public class TaoNhomNhap : ICData
    {
        private DataCustomData data;
        private InfoCustomData info = new InfoCustomData(IDataType.MasterDetailDt);

        private string LayNhomMoi()
        {
            int t = 0;
            Database db = Database.NewDataDatabase();
            object o = db.GetValue("select top 1 Nhom from wblbac order by Nhom desc");
            if (o == null || o.ToString() == "")
                return "00001";
            else
            {
                try
                {
                    if (!Int32.TryParse(o.ToString().Substring(0, 5), out t))
                    {
                        //XtraMessageBox.Show("Lỗi tạo nhóm do nhóm cũ sai định dạng: " + o.ToString(),
                        //    Config.GetValue("PackageName").ToString());
                        //return "";

                        //hvkhoi 20140916
                        DataTable dt = db.GetDataTable("select top 1 MaCT, SoCT, NgayCT, Nhom from wblbac order by Nhom desc");
                        string msg = string.Format("Lỗi tạo nhóm do Số chứng từ '{0}'{1}có nhóm '{2}' sai định dạng. ",
                            dt.Rows[0]["SoCT"].ToString(),
                            dt.Rows[0]["NgayCT"].ToString() != "" ? " và Ngày chứng từ '" + dt.Rows[0]["NgayCT"].ToString() + "' " : " ",
                            dt.Rows[0]["Nhom"].ToString());
                        XtraMessageBox.Show(msg, Config.GetValue("PackageName").ToString());
                        return "";
                        // end hvkhoi
                    }
                }
                catch (Exception)
                {
                    DataTable dt = db.GetDataTable("select top 1 MaCT, SoCT, NgayCT, Nhom from wblbac order by Nhom desc");
                    string msg = string.Format("Lỗi tạo nhóm do Số chứng từ '{0}'{1}có nhóm '{2}' sai định dạng. ",
                        dt.Rows[0]["SoCT"].ToString(), 
                        dt.Rows[0]["NgayCT"].ToString() != "" ? " và Ngày chứng từ '" + dt.Rows[0]["NgayCT"].ToString() + "' " : " ",
                        dt.Rows[0]["Nhom"].ToString());
                    XtraMessageBox.Show(msg, Config.GetValue("PackageName").ToString());
                    return "";
                }
               
            }
            t = t + 1;
            return t.ToString("D5");
        }

        #region ICData Members

        public DataCustomData Data
        {
            set { data = value; }
        }

        public void ExecuteAfter()
        {
            
        }

        public void ExecuteBefore()
        {
            DataRow drCur = data.DsData.Tables[0].Rows[data.CurMasterIndex];
            if (drCur.RowState == DataRowState.Deleted)
                return;
            DataView dv = new DataView(data.DsData.Tables[1]);
            dv.RowStateFilter = DataViewRowState.Added;
            string nhom = LayNhomMoi();
            if (nhom == "")
            {
                info.Result = false;
                return;
            }
            for (int i = 0; i < dv.Count; i++)
            {
                int n = Int32.Parse(nhom) + i;
                dv[i]["Nhom"] = n.ToString("D5");
            }
        }

        public InfoCustomData Info
        {
            get { return info; }
        }

        #endregion

    }
}
