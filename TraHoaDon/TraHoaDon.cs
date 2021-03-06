using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Plugins;
using CDTDatabase;
using DevExpress.XtraEditors;
using CDTLib;
using System.Windows.Forms;

namespace TraHoaDon
{
    public class TraHoaDon : ICData
    {
        public TraHoaDon()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();

        #region ICData Members

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            string tableName = _data.DrTableMaster["TableName"].ToString();
            if (tableName == "MT12" || tableName == "MT16" || tableName == "MT51")
            {
                UpdateCTTT();
                //_info.Result = true;
            }
        }
        private void UpdateCTTT()
        {
            string tableName = _data.DrTableMaster["TableName"].ToString();
            Database dbData = _data.DbData;

            DataView dvMT12 = new DataView(_data.DsDataCopy.Tables[0]);
            dvMT12.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.CurrentRows;
            DataRowView drvMaster = dvMT12[_data.CurMasterIndex];
            string pk = _data.DrTableMaster["Pk"].ToString();
            string pkValue = drvMaster[pk].ToString();
            string MaKH = drvMaster["MaKH"].ToString();
            string SoCT = drvMaster["SoCT"].ToString();
            

            DataView dvDT26 = new DataView(_data.DsDataCopy.Tables[3]);
            dvDT26.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.CurrentRows;
            dvDT26.RowFilter = "MT12ID = '" + pkValue + "'";

            _data.DbData.EndMultiTrans();
            foreach (DataRowView drvData in dvDT26)
            {
                string mt12ID = drvData["MT12ID"].ToString();
                if (mt12ID == pkValue)
                {
                    string mt21ID = drvData["MT21ID"].ToString();
                    string mt21idold = "";
                    
                    if (drvData.Row.RowState == DataRowState.Added)
                    {
                        /* Update CTTT (những chứng từ đã thanh toán) */

                        string sql = @" Update {0} Set CTTT = Case When ( CTTT is null OR CTTT = '' ) Then '{1}' Else CTTT + ',' + '{1}' End Where {3} = '{2}'";
                        int rec1 = 0;
                        // update table MT21
                        _data.DbData.UpdateByNonQuery(string.Format(sql, "MT21", SoCT, mt21ID, "MT21ID"), ref rec1);

                        // update table MT23
                        if (rec1 == 0)
                            _data.DbData.UpdateByNonQuery(string.Format(sql, "MT23", SoCT, mt21ID, "MT23ID"), ref rec1);

                        // update table MT25
                        if (rec1 == 0)
                            _data.DbData.UpdateByNonQuery(string.Format(sql, "MT25", SoCT, mt21ID, "MT25ID"), ref rec1);

                        //_info.Result = (rec1 == 1);
                        //if (_info.Result == false)
                        //    break;
                        if (rec1 != 1)
                            break;
                    }
                    if (drvData.Row.RowState == DataRowState.Unchanged)
                        continue;
                    if (drvData.Row.RowState == DataRowState.Deleted)
                    {
                        mt21idold = drvData.Row["MT21ID", DataRowVersion.Original].ToString();

                        /* Update CTTT (những chứng từ đã thanh toán) */
                        // Remove CTTT không còn tồn tại

                        int rec1 = 0;
                        // string remove
                        string sql = RemoveCTTT();
                        // update table MT21
                        _data.DbData.UpdateByNonQuery(string.Format(sql, SoCT, mt21idold, "MT21","MT21ID"), ref rec1);

                        // update table MT23
                        if (rec1 == 0)
                            _data.DbData.UpdateByNonQuery(string.Format(sql, SoCT, mt21idold, "MT23", "MT23ID"), ref rec1);                      

                        // update table MT25
                        if (rec1 == 0)
                            _data.DbData.UpdateByNonQuery(string.Format(sql, SoCT, mt21idold, "MT25", "MT25ID"), ref rec1);

                        //_info.Result = (rec1 == 1);
                        //if (_info.Result == false)
                        //    break;
                        if (rec1 != 1)
                            break;
                    }
                    if (drvData.Row.RowState == DataRowState.Modified)
                    {
                        //giá trị thanh toán trước khi sửa
                        mt21idold = drvData.Row["MT21ID", DataRowVersion.Original].ToString();
                        if (mt21ID != mt21idold)    //có thay doi chung tu cong no
                        {
                            /* Update CTTT (những chứng từ đã thanh toán) */
                            
                            // Remove CTTT không còn tồn tại                            
                            string sql3 = RemoveCTTT();
                            // string update new
                            string sql4 = @" declare @s nvarchar(128)
                                    declare @ex varchar(128)
                                    set @ex = '{0}'
                                    
                                    Select @s = replace(replace(ISNULL(CTTT,''),@ex,''),',,',',') From {2} Where {3} = '{1}'
                                    Set @s = case 
		                                       when left(@s,1) = ',' then substring(@s,2,len(@s))
		                                       when right(@s,1) = ',' then substring(@s,1,len(@s) - 1) 
		                                       when left(@s,1) != ',' and right(@s,1) != ',' then @s
	                                         end
                                    Set @s = case when @s = '' then '{0}' else @s + ',' + '{0}' end
                                    Update {2} Set CTTT = @s Where {3} = '{1}'";

                            int rec1 = 0;
                            // update table MT21
                            _data.DbData.UpdateByNonQuery(string.Format(sql3, SoCT, mt21idold, "MT21", "MT21ID"), ref rec1);
                            _data.DbData.UpdateByNonQuery(string.Format(sql4, SoCT, mt21ID, "MT21", "MT21ID"), ref rec1);

                            // update table MT23
                            //if (rec1 == 0)
                            //{
                                _data.DbData.UpdateByNonQuery(string.Format(sql3, SoCT, mt21idold, "MT23", "MT23ID"), ref rec1);
                                _data.DbData.UpdateByNonQuery(string.Format(sql4, SoCT, mt21ID, "MT23", "MT23ID"), ref rec1);
                            //}

                            // update table MT25
                            //if (rec1 == 0)
                            //{
                                _data.DbData.UpdateByNonQuery(string.Format(sql3, SoCT, mt21idold, "MT25", "MT25ID"), ref rec1);
                                _data.DbData.UpdateByNonQuery(string.Format(sql4, SoCT, mt21ID, "MT25", "MT25ID"), ref rec1);
                            //}
                            //_info.Result = (rec1 == 1);
                            //if (_info.Result == false)
                            //    break;
                            if (rec1 != 1)
                                break;
                        }
                    }                    
                }
            }
            /* Khi SoCT của phiếu master thay đổi thì thay đổi CTTT của những hóa đơn liên quan */
            DataRow drMaster = _data.DsDataCopy.Tables[0].Rows[_data.CurMasterIndex];
            DataRow drMaster_New = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            string SoCT_New = "";
            if (drMaster.RowState == DataRowState.Modified)
            {
                    SoCT_New = drMaster_New["SoCT"].ToString();
                    if (SoCT != SoCT_New)
                    {                    
                        string sql = @"
                                        Update MT21 Set CTTT = Replace(Replace(CTTT,'{0}','{1}'),',,',',') Where CTTT like '%{0}%';
                                        Update MT23 Set CTTT = replace(CTTT,'{0}','{1}') Where CTTT like '%{0}%';
                                        Update MT25 Set CTTT = replace(CTTT,'{0}','{1}') Where CTTT like '%{0}%';
                                     ";
                        _data.DbData.UpdateByNonQuery(string.Format(sql, SoCT, SoCT_New));
                    }
            }
            if (drMaster.RowState == DataRowState.Deleted)
            {
                SoCT = drMaster["SoCT", DataRowVersion.Original].ToString();
                string sql = @"
                                Select {1},replace(replace(ISNULL(CTTT,''),'{2}',''),',,',',') CTTT Into #TEMP From {0} Where CTTT like '%{2}%'

                                Update {0} Set CTTT =  case 
							                                   when left(t.CTTT,1) = ',' then substring(t.CTTT,2,len(t.CTTT))
							                                   when right(t.CTTT,1) = ',' then substring(t.CTTT,1,len(t.CTTT) - 1) 
							                                   when left(t.CTTT,1) != ',' and right(t.CTTT,1) != ',' then t.CTTT
						                                 end
                                From {0} m inner join #temp t on m.{1} = t.{1}
                                Drop table #temp";
                _data.DbData.UpdateByNonQuery(string.Format(sql,"MT21","MT21ID", SoCT));
                _data.DbData.UpdateByNonQuery(string.Format(sql,"MT23","MT23ID", SoCT));
                _data.DbData.UpdateByNonQuery(string.Format(sql,"MT25","MT25ID", SoCT));
            }
        }

        private string RemoveCTTT()
        {
            return @"   declare @s nvarchar(128)
                        declare @ex varchar(128)
                        set @ex = '{0}'
                    
                        Select @s = replace(replace(ISNULL(CTTT,''),@ex,''),',,',',') From {2} Where {3} = '{1}'

                        Update {2} Set CTTT =  case 
                                               when left(@s,1) = ',' then substring(@s,2,len(@s))
                                               when right(@s,1) = ',' then substring(@s,1,len(@s) - 1) 
                                               when left(@s,1) != ',' and right(@s,1) != ',' then @s
                                           end
                        Where {3} = '{1}'";
        }

        public void ExecuteBefore()
        {
            string tableName = _data.DrTableMaster["TableName"].ToString();
            if (tableName == "MT21" || tableName == "MT22" || tableName == "MT23"
                || tableName == "MT24" || tableName == "MT25")
            {
                CheckInvoice();                
            }
            if (tableName == "MT12" || tableName == "MT16" || tableName == "MT51")
            {
                if (CheckPayment())
                {
                    UpdateInvoices();                    
                    //_info.Result = true;
                }
                else
                {
                    _info.Result = XtraMessageBox.Show("Số tiền của phiếu không khớp với chi tiết công nợ\n" +
                        "Bạn có muốn lưu chứng từ không?", Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo) == DialogResult.Yes;
                }
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion

        private bool CheckPayment()
        {
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Deleted)
                return true;
            DataView dv1 = null, dv2 = null;
            if (drMaster.RowState == DataRowState.Added)
            {
                dv1 = new DataView(_data.DsData.Tables[1]);
                dv2 = new DataView(_data.DsData.Tables[3]);
                dv1.RowStateFilter = DataViewRowState.Added;
                dv2.RowStateFilter = DataViewRowState.Added;
            }
            if (drMaster.RowState == DataRowState.Modified || drMaster.RowState == DataRowState.Unchanged)
            {
                string pk = _data.DrTableMaster["Pk"].ToString();
                string mtid = drMaster[pk].ToString();
                dv1 = new DataView(_data.DsData.Tables[1]);
                dv2 = new DataView(_data.DsData.Tables[3]);
                dv1.RowFilter = pk + " = '" + mtid + "'";
                dv2.RowFilter = "MT12ID = '" + mtid + "'";
            }
            if (dv1 == null || dv2 == null)
                return true;
            DataTable dt1 = dv1.ToTable();
            DataTable dt2 = dv2.ToTable();
            if (dt2.Rows.Count == 0)
                return true;
            object o1 = dt1.Compute("sum(Ps)", "");
            object o2 = dt2.Compute("sum(TT)", "");
            if (o1 == null || o1.ToString() == "" ||
                o2 == null || o2.ToString() == "")
                return true;
            return (decimal.Parse(o1.ToString()) == decimal.Parse(o2.ToString()));
        }
         
        private void UpdateInvoices()
        {
            string tableName = _data.DrTableMaster["TableName"].ToString();
            Database dbData = _data.DbData;

            DataView dvMT12 = new DataView(_data.DsData.Tables[0]);
            dvMT12.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.CurrentRows;
            DataRowView drvMaster = dvMT12[_data.CurMasterIndex];
            string pk = _data.DrTableMaster["Pk"].ToString();
            string pkValue = drvMaster[pk].ToString();
            string MaKH = drvMaster["MaKH"].ToString();
            string SoCT = drvMaster["SoCT"].ToString();

            DataView dvDT26 = new DataView(_data.DsData.Tables[3]);
            dvDT26.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.CurrentRows;
            dvDT26.RowFilter = "MT12ID = '" + pkValue + "'";
            //Kiểm tra các chứng từ hợp lệ mới xử lý tiếp
            if (drvMaster.Row.RowState != DataRowState.Deleted)
                CheckCorrect(dvDT26, MaKH);
            //bool flag = true;
            //if (drvMaster.Row.RowState != DataRowState.Deleted)
            //    flag = CheckCorrect(dvDT26, MaKH);
            //if (!flag)
            //{
            //    _info.Result = false;
            //    return;
            //}
            
            foreach (DataRowView drvData in dvDT26)
            {
                string mt12ID = drvData["MT12ID"].ToString();
                if (mt12ID == pkValue)
                {
                    string mt21ID = drvData["MT21ID"].ToString();

                    string mt21idold = "";
                    string TTNT = "0", TT = "0";
                    string TTNT1 = "0", TT1 = "0";

                    if (drvData.Row.RowState == DataRowState.Added)
                    {
                        TTNT = drvData["TTNT"].ToString().Replace(",", ".");
                        TT = drvData["TT"].ToString().Replace(",", ".");                        
                    }
                    if (drvData.Row.RowState == DataRowState.Unchanged)
                        continue;

                    if (drvData.Row.RowState == DataRowState.Modified)
                    {
                        //giá trị thanh toán trước khi sửa
                        mt21idold = drvData.Row["MT21ID", DataRowVersion.Original].ToString();
                        TTNT1 = drvData.Row["TTNT", DataRowVersion.Original].ToString().Replace(",", ".");
                        TT1 = drvData.Row["TT", DataRowVersion.Original].ToString().Replace(",", ".");
                        // giá trị thanh toán sau khi sửa
                        TTNT = drvData.Row["TTNT", DataRowVersion.Current].ToString().Replace(",", ".");
                        TT = drvData.Row["TT", DataRowVersion.Current].ToString().Replace(",", ".");
                    }

                    if (drvData.Row.RowState == DataRowState.Deleted)
                    {
                        TTNT1 = drvData.Row["TTNT", DataRowVersion.Original].ToString().Replace(",", ".");
                        TT1 = drvData.Row["TT", DataRowVersion.Original].ToString().Replace(",", ".");
                        mt21idold = drvData.Row["MT21ID", DataRowVersion.Original].ToString();
                    }

                    if (mt21idold == "" || mt21ID == mt21idold)    //khong thay doi chung tu cong no
                    {
                        string sql = "update {0} set DaTTNT = DaTTNT - (" + TTNT1 + ")+(" + TTNT + "), DaTT = DaTT - (" + TT1 + ")+(" + TT +
                            ") where {1} = '" + mt21ID + "'";
                        int rec = 0;
                        dbData.UpdateByNonQuery(string.Format(sql, "MT21", "MT21ID"), ref rec);
                        //if (rec == 0)
                            dbData.UpdateByNonQuery(string.Format(sql, "MT22", "MT22ID"), ref rec);
                        //if (rec == 0)
                            dbData.UpdateByNonQuery(string.Format(sql, "MT23", "MT23ID"), ref rec);
                        //if (rec == 0)
                            dbData.UpdateByNonQuery(string.Format(sql, "MT24", "MT24ID"), ref rec);
                        //if (rec == 0)
                            dbData.UpdateByNonQuery(string.Format(sql, "MT25", "MT25ID"), ref rec);
                        //if (rec == 0)
                            dbData.UpdateByNonQuery(string.Format(sql, "OBHD", "MTID"), ref rec);

                        //_info.Result = (rec == 1);
                        //if (_info.Result == false)
                        //    break;
                        //if (rec != 1)
                        //    break;
                    }
                    else
                    {
                        string sql1 = "update {0} set DaTTNT = DaTTNT +(" + TTNT + "), DaTT = DaTT + (" + TT +
                            ") where {1} = '" + mt21ID + "'";
                        string sql2 = "update {0} set DaTTNT = DaTTNT - (" + TTNT1 + "), DaTT = DaTT - (" + TT1 +
                            ") where {1} = '" + mt21idold + "'";
                        int rec = 0;
                        
                        dbData.UpdateByNonQuery(string.Format(sql1, "MT21", "MT21ID"), ref rec);
                        dbData.UpdateByNonQuery(string.Format(sql2, "MT21", "MT21ID"), ref rec);
                        //if (rec == 0)
                        //{
                            dbData.UpdateByNonQuery(string.Format(sql1, "MT22", "MT22ID"), ref rec);
                            dbData.UpdateByNonQuery(string.Format(sql2, "MT22", "MT22ID"), ref rec);
                        //}
                        //if (rec == 0)
                        //{
                            dbData.UpdateByNonQuery(string.Format(sql1, "MT23", "MT23ID"), ref rec);
                            dbData.UpdateByNonQuery(string.Format(sql2, "MT23", "MT23ID"), ref rec);
                        //}
                        //if (rec == 0)
                        //{
                            dbData.UpdateByNonQuery(string.Format(sql1, "MT24", "MT24ID"), ref rec);
                            dbData.UpdateByNonQuery(string.Format(sql2, "MT24", "MT24ID"), ref rec);
                        //}
                        //if (rec == 0)
                        //{
                            dbData.UpdateByNonQuery(string.Format(sql1, "MT25", "MT25ID"), ref rec);
                            dbData.UpdateByNonQuery(string.Format(sql2, "MT25", "MT25ID"), ref rec);
                        //}
                        //if (rec == 0)
                        //{
                            dbData.UpdateByNonQuery(string.Format(sql1, "OBHD", "MTID"), ref rec);
                            dbData.UpdateByNonQuery(string.Format(sql2, "OBHD", "MTID"), ref rec);
                        //}
                        //_info.Result = (rec == 1);
                        //if (_info.Result == false)
                        //    break;
                            //if (rec != 1)
                            //    break;
                    }
                }
            }
        }

        private void CheckInvoice()
        {
            DataView dvMT = new DataView(_data.DsData.Tables[0]);
            dvMT.RowStateFilter = DataViewRowState.Deleted | DataViewRowState.CurrentRows;
            DataRowView drvMaster = dvMT[_data.CurMasterIndex];
            string MTID = drvMaster[_data.DrTableMaster["Pk"].ToString()].ToString();
            string sql = "select MT21ID from DT26 where MT21ID = '" + MTID + "'";
            Database dbData = _data.DbData;
            if (dbData.GetDataTable(sql).Rows.Count == 0)
            {
                _info.Result = true;
                return;
            }

            if (drvMaster.Row.RowState == DataRowState.Deleted)
            {
                string msg = "Không thể xóa vì hóa đơn này đã được thanh toán. Cần xóa chứng từ thanh toán trước!";
                if (Config.GetValue("Language").ToString() == "1")
                    msg = UIDictionary.Translate(msg);
                XtraMessageBox.Show(msg);
                _info.Result = false;
                return;
            }

            if (drvMaster.Row.RowState == DataRowState.Modified)
            {
                if (!drvMaster.Row["TTien", DataRowVersion.Current].Equals(drvMaster.Row["TTien", DataRowVersion.Original])
                    || !drvMaster.Row["MaKH", DataRowVersion.Current].Equals(drvMaster.Row["MaKH", DataRowVersion.Original])
                    || !drvMaster.Row["NgayCT", DataRowVersion.Current].Equals(drvMaster.Row["NgayCT", DataRowVersion.Original])
                    || !drvMaster.Row["TkCo", DataRowVersion.Current].Equals(drvMaster.Row["TkCo", DataRowVersion.Original]))
                {
                    string msg = "Không thể điều chỉnh vì hóa đơn này đã được thanh toán. Cần xóa chứng từ thanh toán trước!";
                    if (Config.GetValue("Language").ToString() == "1")
                        msg = UIDictionary.Translate(msg);
                    XtraMessageBox.Show(msg);
                    _info.Result = false;
                    return;
                }
            }
            //if (drvMaster.Row.RowState == DataRowState.Unchanged)
            //{
            //    DataView dvDT = new DataView(_data.DsData.Tables[1]);
            //    dvDT.RowStateFilter = DataViewRowState.ModifiedCurrent;
            //    dvDT.RowFilter = _data.DrTableMaster["Pk"].ToString() + " = '" + MTID + "'";
            //    if (dvDT.Count > 0)
            //    {
            //        string msg = "Không thể điều chỉnh vì hóa đơn này đã được thanh toán. Cần điều chỉnh chứng từ thanh toán trước!";
            //        if (Config.GetValue("Language").ToString() == "1")
            //            msg = UIDictionary.Translate(msg);
            //        XtraMessageBox.Show(msg);
            //        _info.Result = false;
            //        return;
            //    }
            //}
            _info.Result = true;
        }

        /*private void UpdateCTTT(string tablename)
        {
            DataView dv = new DataView(_data.DsData.Tables[0]);
            DataRow drMaster = dv[_data.CurMasterIndex];            
            string FieldNameMT = tablename + "ID";
            string MTID = drMaster[FieldNameMT].ToString();
            DataTable dtCNPT = _data.DsData.Tables[2];
            DataRow[] drCNPT = dtCNPT.Select(FieldNameMT + " = '" + MTID + "'");
            Database dbData = _data.DbData;
            string SoCT = drMaster["SoCT"].ToString();
            string sChungTu = "";            
            if (drCNPT.Length > 0)
            {
                for (int i = 0; i < drCNPT.Length; i++)
                {
                    sChungTu = drCNPT[i]["SoCT"].ToString();
                    string sql = " Update {0} Set CTTT = Case When CTTT is null Then '{1}' Else CTTT + ',' + '{1}' End Where SoCT = {2}";
                    int rec = 0;

                    // update table MT21
                    db.UpdateByNonQuery(string.Format(sql, "MT21", SoCT, sChungTu), ref rec);

                    // update table MT23
                    if(rec == 0)
                        db.UpdateByNonQuery(string.Format(sql, "MT23", SoCT, sChungTu), ref rec);

                    // update table MT25
                    if (rec == 0)
                        db.UpdateByNonQuery(string.Format(sql, "MT25", SoCT, sChungTu), ref rec);

                    _info.Result = (rec == 1);
                    if (_info.Result == false)
                        break;
                }
                
            }
        }*/

        private void CheckCorrect(DataView dvChungTu, string MaKH)
        {
            string sql = "", soct = "";
            DataTable dt = new DataTable();
            foreach (DataRowView drv in dvChungTu)
            {
                if (drv.Row.RowState == DataRowState.Deleted)
                    return;
                sql = "select * from whoadonmua where MT21ID = '" + drv.Row["MT21ID", DataRowVersion.Current].ToString() + "' and MaKh = '" + MaKH + "'";
                dt = db.GetDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    sql = "select * from whoadonmua where MT21ID = '" + drv.Row["MT21ID", DataRowVersion.Current].ToString() + "'";
                    dt = db.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                        soct = dt.Rows[0]["SoCT"].ToString();
                    XtraMessageBox.Show("Hóa đơn có số chứng từ " + soct + " không phải của khách hàng đã chọn.", Config.GetValue("PackageName").ToString());
                    _info.Result = false;
                    return;
                }
            }
        }     
    }
}
