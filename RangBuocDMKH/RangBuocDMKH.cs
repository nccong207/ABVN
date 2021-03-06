using System;
using System.Collections.Generic;
using System.Text;
using Plugins;

using System.Data;
using CDTDatabase;
using DevExpress.XtraEditors;
using CDTLib;

namespace RangBuocDMKH
{
    public class RangBuocDMKH : ICData
    {
        DataCustomData _data;
        InfoCustomData _info;
        public RangBuocDMKH()
        {
            _info = new InfoCustomData(IDataType.Single);
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
            if (Boolean.Parse(drCur["isKH"].ToString()) == true)
            {
                List<string> lstCheckKH = new List<string>(new string[] {"DoiTac", "SDT", "DiaChi1", 
                    "NganhNghe", "TiemNang", "NLH1", "CV1", "DT1", "KhuVuc", "MaNV", "Nhom1"});
                bool valid = true;
                string mes = "";
                foreach (string s in lstCheckKH)
                {
                    if (drCur[s].ToString() == "")
                    {
                        valid = false;
                        //drCur.SetColumnError(s, "Phải nhập");
                        mes += GetCaptionColumn(s) + "\n";
                    }
                    //else
                    //    drCur.SetColumnError(s, "");
                }                
                _info.Result = valid;
                if(_info.Result == false)
                    XtraMessageBox.Show("Vui lòng nhập đầy đủ các thông tin sau:\n" + mes, Config.GetValue("PackageName").ToString());
            }

        }
        private string GetCaptionColumn(string s)
        {
            switch (s)
            {
                case "DoiTac":
                    return "Tên viết tắt";
                    
                case "SDT":
                    return "Tel (thông tin chung)";
                    
                case "NganhNghe":
                    return "Ngành nghề";
                    
                case "DiaChi1":
                    return "Địa chỉ giao hàng 1";
                    
                case "TiemNang":
                    return "Tiềm năng";
                    
                case "NLH1":
                    return "Người liên hệ 1";
                                   
                case "CV1":
                    return "Chứ vụ 1 (thông tin liên hệ)";
                    
                case "DT1":
                    return "Tel 1 (thông tin liên hệ)";
                    
                case "KhuVuc":
                    return "Khu vực";
                    
                case "MaNV":
                    return "Thuộc NVKD";
                   
                case "Nhom1":
                    return "Phân loại";
                    
                default: return "";
            }
        }
        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
