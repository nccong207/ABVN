namespace LichLamViec
{
    partial class FrmQLLich
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraScheduler.TimeRuler timeRuler1 = new DevExpress.XtraScheduler.TimeRuler();
            DevExpress.XtraScheduler.TimeRuler timeRuler2 = new DevExpress.XtraScheduler.TimeRuler();
            this.scMain = new DevExpress.XtraScheduler.SchedulerControl();
            this.ssMain = new DevExpress.XtraScheduler.SchedulerStorage(this.components);
            this.cbeView = new DevExpress.XtraEditors.ComboBoxEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.LookupNhanVien = new DevExpress.XtraEditors.LookUpEdit();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.cbePrintStyle = new DevExpress.XtraEditors.ComboBoxEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ssMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeView.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LookupNhanVien.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbePrintStyle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // scMain
            // 
            this.scMain.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.WorkWeek;
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 44);
            this.scMain.Name = "scMain";
            this.scMain.OptionsView.FirstDayOfWeek = DevExpress.XtraScheduler.FirstDayOfWeek.Monday;
            this.scMain.Size = new System.Drawing.Size(866, 448);
            this.scMain.Start = new System.DateTime(2012, 7, 9, 0, 0, 0, 0);
            this.scMain.Storage = this.ssMain;
            this.scMain.TabIndex = 0;
            this.scMain.Text = "schedulerControl1";
            this.scMain.Views.DayView.TimeRulers.Add(timeRuler1);
            this.scMain.Views.WorkWeekView.ShowFullWeek = true;
            this.scMain.Views.WorkWeekView.TimeRulers.Add(timeRuler2);
            this.scMain.Views.WorkWeekView.WorkTime.Start = System.TimeSpan.Parse("08:00:00");
            this.scMain.InitAppointmentDisplayText += new DevExpress.XtraScheduler.AppointmentDisplayTextEventHandler(this.scMain_InitAppointmentDisplayText);
            this.scMain.InitNewAppointment += new DevExpress.XtraScheduler.AppointmentEventHandler(this.scMain_InitNewAppointment);
            // 
            // cbeView
            // 
            this.cbeView.EditValue = "Tuần làm việc";
            this.cbeView.Location = new System.Drawing.Point(87, 7);
            this.cbeView.Name = "cbeView";
            this.cbeView.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbeView.Properties.Items.AddRange(new object[] {
            "Theo ngày",
            "Theo tuần",
            "Theo tháng",
            "Tuần làm việc",
            "Theo thời gian"});
            this.cbeView.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbeView.Size = new System.Drawing.Size(111, 20);
            this.cbeView.StyleController = this.layoutControl1;
            this.cbeView.TabIndex = 12;
            this.cbeView.SelectedIndexChanged += new System.EventHandler(this.cbeView_SelectedIndexChanged);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.LookupNhanVien);
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.cbePrintStyle);
            this.layoutControl1.Controls.Add(this.cbeView);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(866, 44);
            this.layoutControl1.TabIndex = 13;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // LookupNhanVien
            // 
            this.LookupNhanVien.Location = new System.Drawing.Point(289, 7);
            this.LookupNhanVien.Name = "LookupNhanVien";
            this.LookupNhanVien.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.LookupNhanVien.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.LookupNhanVien.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TenNV", "Tên Nhân Viên", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TenCV", "Chức vụ", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None)});
            this.LookupNhanVien.Properties.NullText = "Tất cả";
            this.LookupNhanVien.Size = new System.Drawing.Size(131, 20);
            this.LookupNhanVien.StyleController = this.layoutControl1;
            this.LookupNhanVien.TabIndex = 14;
            this.LookupNhanVien.EditValueChanged += new System.EventHandler(this.LookupNhanVien_EditValueChanged);
            this.LookupNhanVien.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LookupNhanVien_KeyDown);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(654, 7);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(51, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 14;
            this.btnPrint.Text = "In";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // cbePrintStyle
            // 
            this.cbePrintStyle.EditValue = "Danh sách việc";
            this.cbePrintStyle.Location = new System.Drawing.Point(511, 7);
            this.cbePrintStyle.Name = "cbePrintStyle";
            this.cbePrintStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbePrintStyle.Properties.Items.AddRange(new object[] {
            "Mỗi ngày",
            "Mỗi tuần",
            "Mỗi tháng",
            "Kết hợp",
            "Danh sách việc",
            "Chi tiết việc"});
            this.cbePrintStyle.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbePrintStyle.Size = new System.Drawing.Size(132, 20);
            this.cbePrintStyle.StyleController = this.layoutControl1;
            this.cbePrintStyle.TabIndex = 14;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem4,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(866, 44);
            this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cbeView;
            this.layoutControlItem1.CustomizationFormText = "Chọn view";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem1.Size = new System.Drawing.Size(202, 42);
            this.layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Text = "Xem theo";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(75, 20);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.LookupNhanVien;
            this.layoutControlItem4.CustomizationFormText = "Chọn nhân viên";
            this.layoutControlItem4.Location = new System.Drawing.Point(202, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem4.Size = new System.Drawing.Size(222, 42);
            this.layoutControlItem4.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem4.Text = "Chọn nhân viên";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(75, 20);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.cbePrintStyle;
            this.layoutControlItem2.CustomizationFormText = "Chọn in lịch";
            this.layoutControlItem2.Location = new System.Drawing.Point(424, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem2.Size = new System.Drawing.Size(223, 42);
            this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Text = "Chọn in lịch";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(75, 20);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnPrint;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(647, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem3.Size = new System.Drawing.Size(62, 42);
            this.layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem3.Text = "layoutControlItem3";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(709, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.emptySpaceItem1.Size = new System.Drawing.Size(155, 42);
            this.emptySpaceItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // FrmQLLich
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 492);
            this.Controls.Add(this.scMain);
            this.Controls.Add(this.layoutControl1);
            this.Name = "FrmQLLich";
            this.Text = "FrmQLLich";
            this.Load += new System.EventHandler(this.FrmQLLich_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmQLLich_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ssMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeView.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LookupNhanVien.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbePrintStyle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraScheduler.SchedulerControl scMain;
        private DevExpress.XtraScheduler.SchedulerStorage ssMain;
        private DevExpress.XtraEditors.ComboBoxEdit cbeView;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.ComboBoxEdit cbePrintStyle;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.LookUpEdit LookupNhanVien;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}