using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Binaria.WMSTek.Framework.Entities.Base;
using System.Web.UI.WebControls;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class TaskBarContent : UserControl
    {
        public event EventHandler BtnRefreshClick;
        public event EventHandler BtnNewClick;
        public event EventHandler BtnSaveClick;
        public event EventHandler BtnExcelClick;
        public event EventHandler BtnExcelDetailClick;
        public event EventHandler BtnPrintClick;
        public event EventHandler BtnPrintAllClick;
        public event EventHandler BtnAddClick;
        public event EventHandler BtnDownloadClick;
        public event EventHandler BtnSendDataClick;

        public TaskBarContent()
        {
            Load += new EventHandler(Page_Load);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            btnRefresh.Attributes.Add("onmouseover", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_refresh_on.png") + "'");
            btnRefresh.Attributes.Add("onmouseout", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_refresh.png") + "'");

            btnNew.Attributes.Add("onmouseover", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_new_on.png") + "'");
            btnNew.Attributes.Add("onmouseout", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_new.png") + "'");

            btnSave.Attributes.Add("onmouseover", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_save_on.png") + "'");
            btnSave.Attributes.Add("onmouseout", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_save.png") + "'");

            btnExcel.Attributes.Add("onmouseover", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_export_excel_on.png") + "'");
            btnExcel.Attributes.Add("onmouseout", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_export_excel.png") + "'");

            btnExcelDetail.Attributes.Add("onmouseover", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_export_excel_on.png") + "'");
            btnExcelDetail.Attributes.Add("onmouseout", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_export_excel.png") + "'");

            btnPrint.Attributes.Add("onmouseover", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_print_on.png") + "'");
            btnPrint.Attributes.Add("onmouseout", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_print.png") + "'");

            btnPrintAll.Attributes.Add("onmouseover", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_print_on_more.png") + "'");
            btnPrintAll.Attributes.Add("onmouseout", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/TaskBar/icon_print_more.png") + "'");

            btnAdd.Attributes.Add("onmouseover", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/GridActions/icon_add_on.png") + "'");
            btnAdd.Attributes.Add("onmouseout", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/GridActions/icon_add.png") + "'");

            btnDownload.Attributes.Add("onmouseover", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/GridActions/icon_apply_inventory_on.png") + "'");
            btnDownload.Attributes.Add("onmouseout", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/GridActions/icon_apply_inventory.png") + "'");

            btnSendData.Attributes.Add("onmouseover", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/GridActions/icon_up_on.png") + "'");
            btnSendData.Attributes.Add("onmouseout", "this.src= '" + this.Page.ResolveClientUrl("~/WebResources/Images/Buttons/GridActions/icon_up.png") + "'");
        }

        public bool btnRefreshVisible
        {
            get { return this.btnRefresh.Visible; }
            set { this.btnRefresh.Visible = value; }
        }

        public ImageButton btnRefreshTaskBar
        {
            get { return this.btnRefresh; }
        }

        public bool btnNewVisible
        {
            get { return this.btnNew.Visible; }
            set { this.btnNew.Visible = value; }
        }

        public bool btnClientNewVisible
        {
            get { return this.imgClientNew.Visible; }
            set { this.imgClientNew.Visible = value; }
        }

        public bool btnSaveVisible
        {
            get { return this.btnSave.Visible; }
            set { this.btnSave.Visible = value; }
        }
        public string btnSaveToolTip
        {
            set { this.btnSave.ToolTip = value; }
        }
        public bool btnSaveEnabled
        {
            get { return this.btnSave.Enabled; }
            set
            {
                this.btnSave.Enabled = value;

                if (value)
                    this.btnSave.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_save.png";
                else
                    this.btnSave.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_save_dis.png";
            }
        }

        public ImageButton btnSaveTaskBar
        {
            get { return btnSave; }
        }

        public bool btnExcelVisible
        {
            get { return this.btnExcel.Visible; }
            set { this.btnExcel.Visible = value; }
        }
        public string btnExcelToolTip
        {
            set { this.btnExcel.ToolTip = value; }
        }

        public bool btnExcelDetailVisible
        {
            get { return this.btnExcelDetail.Visible; }
            set { this.btnExcelDetail.Visible = value; }
        }
        public string btnExcelDetailToolTip
        {
            set { this.btnExcelDetail.ToolTip = value; }
        }

        public bool btnPrintVisible
        {
            get { return this.btnPrint.Visible; }
            set { this.btnPrint.Visible = value; }
        }

        public bool btnPrintEnabled
        {
            get { return this.btnPrint.Enabled; }
            set 
            { 
                this.btnPrint.Enabled = value; 

                if (value)
                    this.btnPrint.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_print.png"; 
                else
                    this.btnPrint.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_print_dis.png"; 
            }
        }

        public string btnPrintToolTip
        {
            set { this.btnPrint.ToolTip = value; }
        }

        public string btnPrintAllToolTip
        {
            set { this.btnPrintAll.ToolTip = value; }
        }

        public bool btnPrintAllVisible
        {
            get { return this.btnPrintAll.Visible; }
            set { this.btnPrintAll.Visible = value; }
        }

        public bool btnPrintAllEnabled
        {
            get { return this.btnPrintAll.Enabled; }
            set
            {
                this.btnPrintAll.Enabled = value;

                if (value)
                    this.btnPrintAll.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_print_more.png";
                else
                    this.btnPrintAll.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_print_dis_more.png";
            }
        }

        public bool btnAddVisible
        {
            get { return this.btnAdd.Visible; }
            set { this.btnAdd.Visible = value; }
        }

        public bool btnAddEnabled
        {
            get { return this.btnAdd.Enabled; }
            set
            {
                this.btnAdd.Enabled = value;

                if (value)
                    this.btnAdd.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_add.png";
                else
                    this.btnAdd.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_add_dis.png";
            }
        }

        public string btnAddToolTip
        {
            set { this.btnAdd.ToolTip = value; }
        }

        public bool btnDownloadEnabled
        {
            get { return this.btnDownload.Enabled; }
            set
            {
                this.btnDownload.Enabled = value;

                if (value)
                    this.btnDownload.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_apply_inventory.png";
                else
                    this.btnDownload.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_apply_inventory_dis.png";
            }
        }

        public bool btnDownloadVisible
        {
            get { return this.btnDownload.Visible; }
            set { this.btnDownload.Visible = value; }
        }
        public string btnDownloadToolTip
        {
            set { this.btnDownload.ToolTip = value; }
        }



        public bool btnSendDataEnabled
        {
            get { return this.btnSendData.Enabled; }
            set
            {
                this.btnSendData.Enabled = value;

                if (value)
                    this.btnSendData.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_up.png";
                else
                    this.btnSendData.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_up_dis.png";
            }
        }
        public bool btnSendDataVisible
        {
            get { return this.btnSendData.Visible; }
            set { this.btnSendData.Visible = value; }
        }
        public string btnSendDataToolTip
        {
            set { this.btnSendData.ToolTip = value; }
        }

        public string cssStyle
        {
            set { divTaskBar.Attributes.Add("class", value); }
        }
                    

        // Dispara los eventos que seran capturados por las paginas que implementen este control
        // -------------------------------------------------------------------------------------
        // Refresh
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
                // Valida variable de sesion del Usuario Loggeado
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                if (BtnRefreshClick != null) BtnRefreshClick(this, e);
            }
        }

        // New
        protected void btnNew_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                if (BtnNewClick != null) BtnNewClick(this, e);
            }
        }

        // Save
        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                if (BtnSaveClick != null) BtnSaveClick(this, e);
            }
        }

        // Export to Excel
        protected void btnExcel_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                if (BtnExcelClick != null) BtnExcelClick(this, e);
            }
        }

        // Export Detail to Excel
        protected void btnExcelDetail_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                if (BtnExcelDetailClick != null) BtnExcelDetailClick(this, e);
            }
        }

        // Print
        protected void btnPrint_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                if (BtnPrintClick != null) BtnPrintClick(this, e);
            }
        }

        // Add
        protected void btnAdd_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                if (BtnAddClick != null) BtnAddClick(this, e);
            }
        }

        //Download
        protected void btnDownload_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                if (BtnDownloadClick != null) BtnDownloadClick(this, e);
            }
        }

        // Print All
        protected void btnPrintAll_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                if (BtnPrintAllClick != null) BtnPrintAllClick(this, e);
            }
        }

        // Send Data
        protected void btnSendData_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                if (BtnSendDataClick != null) BtnSendDataClick(this, e);
            }
        }



    }
}
