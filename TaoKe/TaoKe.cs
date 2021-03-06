using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using System.Windows.Forms;
using System.Data;
using Plugins;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraEditors;
namespace TaoKe
{
    public class TaoKe:ICData
    {
        #region ICData Members

        private DataCustomData data;
        private InfoCustomData info;
        Database db = Database.NewDataDatabase();

        public TaoKe()
        {
            info = new InfoCustomData(IDataType.Single);
        }

        public DataCustomData Data
        {
            set { data = value; }
        }

        public void ExecuteAfter()
        {
            if (data.CurMasterIndex < 0)
                return;
            string sql = "";
            DataTable dt = new DataTable();
            DataRow drMaster = data.DsData.Tables[0].Rows[data.CurMasterIndex];
            if (drMaster == null)
                return;
            if (!drMaster.Table.Columns.Contains("SoKe"))
                return;
            if (drMaster.RowState == DataRowState.Deleted)
            {
                sql = "select * from dmke where makho = '" + drMaster["MaKho", DataRowVersion.Original].ToString() + "'";
                dt = db.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                    XtraMessageBox.Show("Kho này đã được sử dụng, cần xóa các dữ liệu liên quan trước!", Config.GetValue("PackageName").ToString());
                info.Result = false;
                return;
            }
            if (drMaster["SoKe"].ToString() == "")
                return;            
            string makho = drMaster["MaKho"].ToString();
            int soke = int.Parse(drMaster["SoKe"].ToString());
            data.DbData.EndMultiTrans();
            if (drMaster.RowState == DataRowState.Added)
            {
                //tạo kệ
                InsertKe(makho, 0, soke);
                //tạo ngăn
                frmKe frm = new frmKe();
                frm.makho = makho;
                frm.soke = 0;
                frm.Text = "Danh mục kệ";
                frm.ShowDialog();
            }
            else if (drMaster.RowState == DataRowState.Modified)
            {
                int sokeOrg = int.Parse(drMaster["SoKe", DataRowVersion.Original].ToString());
                int sokeCur = int.Parse(drMaster["SoKe", DataRowVersion.Current].ToString());
                if (sokeOrg != sokeCur)
                {
                    //lớn hơn thì thêm mới những cái thiếu
                    if (sokeCur > sokeOrg)
                    {
                        //xử lý tình huống thêm vào 5, sửa thành 2, sau đó sửa thành 4.
                        sql = "select * from dmke where makho = '" + drMaster["MaKho"].ToString() + "'";
                        dt = db.GetDataTable(sql);
                        //nếu giá trị sau mà > tổng số kệ ==>them moi
                        if (dt.Rows.Count < sokeCur)
                        {
                            InsertKe(makho, sokeOrg, sokeCur);
                            //tạo ngăn -- chỉ load những ngăn mới thêm
                            frmKe frm = new frmKe();
                            frm.makho = makho;
                            frm.soke = sokeOrg;
                            frm.Text = "Danh mục kệ";
                            frm.ShowDialog();
                        }
                        else // active lại những kệ đã inactive
                            ActiveKe(sokeOrg, sokeCur, false, makho);
                    }
                    else //nhỏ hơn thì disable những cái dư                                            
                        ActiveKe(sokeCur, sokeOrg, true, makho);
                }
            }
        }

        void ActiveKe(int begin, int end, bool isActive, string makho)
        {
            begin++;
            string sql = "";
            if (isActive)
            {
                for (int i = begin; i <= end; i++)
                {
                    sql = "update dmke set isActive = '1' where make = '" + i.ToString() + "' and MaKho = '" + makho + "'";
                    db.UpdateByNonQuery(sql);
                }
            }
            else
            {
                for (int i = begin; i <= end; i++)
                {
                    sql = "update dmke set isActive = '0' where make = '" + i.ToString() + "' and MaKho = '" + makho + "'";
                    db.UpdateByNonQuery(sql);
                }
            }
        }

        void InsertKe(string makho, int begin, int end)
        {
            string sql = "";
            begin++;
            for (int i = begin; i <= end; i++)
            {
                sql = "insert into dmke(make,tenke,makho) values ('" + i.ToString() + "','" + i.ToString() + "','" + makho + "')";
                db.UpdateByNonQuery(sql);
            }
        }
        
        public void ExecuteBefore()
        {
           
        }

        public InfoCustomData Info
        {
            get { return info; }
        }

        #endregion
    }
}
