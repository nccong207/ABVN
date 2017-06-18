namespace WindowsApplication1
{
    partial class XtraReport1
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

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XtraReport1));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.winControlContainer1 = new DevExpress.XtraReports.UI.WinControlContainer();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xtraReport21 = new WindowsApplication1.XtraReport2();
            ((System.ComponentModel.ISupportInitialize)(this.xtraReport21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrSubreport1,
            this.winControlContainer1});
            this.Detail.Height = 117;
            this.Detail.Name = "Detail";
            // 
            // winControlContainer1
            // 
            this.winControlContainer1.Location = new System.Drawing.Point(433, 50);
            this.winControlContainer1.Name = "winControlContainer1";
            this.winControlContainer1.Size = new System.Drawing.Size(100, 25);
            this.winControlContainer1.WinControl = this.simpleButton1;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(0, 0);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(96, 24);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "simpleButton1";
            // 
            // PageHeader
            // 
            this.PageHeader.Height = 30;
            this.PageHeader.Name = "PageHeader";
            // 
            // PageFooter
            // 
            this.PageFooter.Height = 30;
            this.PageFooter.Name = "PageFooter";
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.Location = new System.Drawing.Point(0, 92);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = this.xtraReport21;
            this.xrSubreport1.Size = new System.Drawing.Size(642, 25);
            // 
            // XtraReport1
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.PageHeader,
            this.PageFooter});
            this.Watermark.Image = ((System.Drawing.Image)(resources.GetObject("XtraReport1.Watermark.Image")));
            this.Watermark.ImageViewMode = DevExpress.XtraPrinting.Drawing.ImageViewMode.Zoom;
            ((System.ComponentModel.ISupportInitialize)(this.xtraReport21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private XtraReport2 xtraReport21;
        private DevExpress.XtraReports.UI.WinControlContainer winControlContainer1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
    }
}
