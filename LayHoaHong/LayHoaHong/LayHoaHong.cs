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

namespace LayHoaHong
{
    public class LayHoaHong : ICControl
    {

        Database db = Database.NewDataDatabase();
        InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        DataCustomFormControl data;

        #region ICControl Members

        CheckEdit ckHoaHong;
        CalcEdit calTienHH;
        public void AddEvent()
        {
            ckHoaHong = data.FrmMain.Controls.Find("DaChiHH",true) [0] as CheckEdit;
            ckHoaHong.EditValueChanged += new EventHandler(ckHoaHong_EditValueChanged);
            calTienHH = data.FrmMain.Controls.Find("DaChi", true)[0] as CalcEdit ;
            
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
