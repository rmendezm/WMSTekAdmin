using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Billing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Administration.Parameters
{
    public partial class CfgBillingMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<CfgBilling> cfgBillingViewDTO = new GenericViewDTO<CfgBilling>();
        private bool isValidViewDTO = true;

        // Propiedad para controlar el nro de pagina activa en la grilla
        public int currentPage
        {
            get
            {
                if (ValidateViewState("page"))
                    return (int)ViewState["page"];
                else
                    return 0;
            }

            set { ViewState["page"] = value; }
        }

        // Propiedad para controlar el indice activo en la grilla
        public int currentIndex
        {
            get
            {
                if (ValidateViewState("index"))
                    return (int)ViewState["index"];
                else
                    return -1;
            }

            set { ViewState["index"] = value; }
        }
        #endregion

        #region Events

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                if (base.webMode == WebMode.Normal)
                {
                    if (isValidViewDTO)
                    {
                        PopulateGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    }
                }
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowModal(0, CRUD.Create);
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;

                DeleteRow(deleteIndex);
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }

        protected void txtEndDate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtEndDate.Text.Trim()))
                {
                    rfvEndDate.Enabled = false;
                    rvEndDate.Enabled = false;
                    rfvEndHour.Enabled = false;
                    revEndHour.Enabled = false;
                    txtEndHour.Text = string.Empty;
                }
                else
                {
                    rfvEndDate.Enabled = true;
                    rvEndDate.Enabled = true;
                    rfvEndHour.Enabled = true;
                    revEndHour.Enabled = true;
                }

                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                cfgBillingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }
        }
        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }

        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);

            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnNewVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = lblSearchByName.Text;
            this.Master.ucMainFilter.codeAltVisible = true;
            this.Master.ucMainFilter.codeLabelAlt = this.lblSearchByDescription.Text;

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);

            cfgBillingViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(cfgBillingViewDTO.MessageStatus.Message);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void DeleteRow(int index)
        {
            cfgBillingViewDTO = (GenericViewDTO<CfgBilling>)Session[WMSTekSessions.CfgTablesMGR.CfgBillingList];

            cfgBillingViewDTO = cfgTablesMGR.MaintainCfgBilling(CRUD.Delete, cfgBillingViewDTO.Entities[index], context);  

            if (cfgBillingViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(cfgBillingViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        private void UpdateSession(bool showError)
        {
            if (showError)
            {
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
                cfgBillingViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            cfgBillingViewDTO = cfgTablesMGR.FindAllCfgBilling(context);

            if (!cfgBillingViewDTO.hasError() && cfgBillingViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.CfgTablesMGR.CfgBillingList, cfgBillingViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(cfgBillingViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgBillingViewDTO.Errors);
            }

            upGrid.Update();
            CallJsGridView();
        }

        private void PopulateGrid()
        {
            cfgBillingViewDTO = (GenericViewDTO<CfgBilling>)Session[WMSTekSessions.CfgTablesMGR.CfgBillingList];

            if (cfgBillingViewDTO != null)
            {
                grdMgr.PageIndex = currentPage;

                grdMgr.DataSource = cfgBillingViewDTO.Entities;
                grdMgr.DataBind();

                ucStatus.ShowRecordInfo(cfgBillingViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
            }
        }

        protected void ReloadData()
        {
            UpdateSession(false);

            if (isValidViewDTO)
            {
                currentIndex = -1;
                currentPage = 0;
            }
        }

        protected void ShowModal(int index, CRUD mode)
        {
            if (mode == CRUD.Update)
            {
                cfgBillingViewDTO = (GenericViewDTO<CfgBilling>)Session[WMSTekSessions.CfgTablesMGR.CfgBillingList];

                hidEditId.Value = cfgBillingViewDTO.Entities[index].Id.ToString();
                txtCode.Text = cfgBillingViewDTO.Entities[index].BillingCode;
                txtName.Text = cfgBillingViewDTO.Entities[index].BillingName;
                txtDescription.Text = cfgBillingViewDTO.Entities[index].Description;
                chkStatus.Checked = cfgBillingViewDTO.Entities[index].Status;

                if (cfgBillingViewDTO.Entities[index].StartDate != DateTime.MinValue)
                {
                    txtStartDate.Text = cfgBillingViewDTO.Entities[index].StartDate.ToString("dd/MM/yyyy");
                    txtStartHour.Text = cfgBillingViewDTO.Entities[index].StartDate.ToString("HH:mm");
                }

                txtEndDate.Text = string.Empty;
                txtEndHour.Text = string.Empty;

                if (cfgBillingViewDTO.Entities[index].EndDate != DateTime.MinValue)
                {
                    txtEndDate.Text = cfgBillingViewDTO.Entities[index].EndDate.ToString("dd/MM/yyyy");
                    txtEndHour.Text = cfgBillingViewDTO.Entities[index].EndDate.ToString("HH:mm");
                }

                txtFrequenceInterval.Text = cfgBillingViewDTO.Entities[index].FrequenceInterval.ToString();
                txtPagination.Text = cfgBillingViewDTO.Entities[index].Pagination.ToString();

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }
            else if (mode == CRUD.Create)
            {
                hidEditId.Value = "0";

                txtCode.Text = string.Empty;
                txtName.Text = string.Empty;
                txtDescription.Text = string.Empty;
                txtStartDate.Text = string.Empty;
                txtStartHour.Text = string.Empty;
                txtEndDate.Text = string.Empty;
                txtEndHour.Text = string.Empty;
                txtFrequenceInterval.Text = string.Empty;
                txtPagination.Text = string.Empty;
                chkStatus.Checked = true;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
            CallJsGridView();
        }

        protected void SaveChanges()
        {
            var cfgBilling = new CfgBilling();
            cfgBilling.BillingCode = txtCode.Text.Trim();
            cfgBilling.BillingName = txtName.Text.Trim();
            cfgBilling.Description = txtDescription.Text.Trim();
            cfgBilling.Status = chkStatus.Checked;
            cfgBilling.FrequenceInterval = int.Parse(txtFrequenceInterval.Text.Trim());

            DateTime finalStartDate;
            bool flagStartDate = DateTime.TryParseExact(txtStartDate.Text.Trim() + " " + txtStartHour.Text.Trim(), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out finalStartDate);
            cfgBilling.StartDate = finalStartDate;
            cfgBilling.StartTime = finalStartDate;

            bool flagEndDate = true;
            if (!string.IsNullOrEmpty(txtEndDate.Text.Trim()))
            {
                DateTime finalEndDate;
                flagEndDate = DateTime.TryParseExact(txtEndDate.Text.Trim() + " " + txtEndHour.Text.Trim(), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out finalEndDate);
                cfgBilling.EndDate = finalEndDate;
                cfgBilling.EndTime = finalEndDate;
            }
            else
            {
                cfgBilling.EndDate = DateTime.MinValue;
                cfgBilling.EndTime = DateTime.MinValue;
            }

            if (!flagStartDate || !flagEndDate)
            {
                ucStatus.ShowMessage(lblErrorDateFormat.Text);
                return;
            }

            cfgBilling.Pagination = int.Parse(txtPagination.Text.Trim());

            if (hidEditId.Value == "0")
            {
                cfgBillingViewDTO = cfgTablesMGR.MaintainCfgBilling(CRUD.Create, cfgBilling, context);
            }
            else
            {
                cfgBilling.Id = int.Parse(hidEditId.Value);
                cfgBillingViewDTO = cfgTablesMGR.MaintainCfgBilling(CRUD.Update, cfgBilling, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (cfgBillingViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(cfgBillingViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "afterAsyncPostBack();", true);
        }
        #endregion
    }
}