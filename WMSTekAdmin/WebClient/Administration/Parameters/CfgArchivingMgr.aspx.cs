using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Kpi;
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
    public partial class CfgArchivingMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<CfgArchiving> cfgArchivingKpiViewDTO = new GenericViewDTO<CfgArchiving>();
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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
                cfgArchivingKpiViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
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

            cfgArchivingKpiViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(cfgArchivingKpiViewDTO.MessageStatus.Message);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void DeleteRow(int index)
        {
            cfgArchivingKpiViewDTO = (GenericViewDTO<CfgArchiving>)Session[WMSTekSessions.CfgTablesMGR.CfgArchivingKpiList];

            cfgArchivingKpiViewDTO = cfgTablesMGR.MaintainCfgArchivingKpi(CRUD.Delete, cfgArchivingKpiViewDTO.Entities[index], context);

            if (cfgArchivingKpiViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(cfgArchivingKpiViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        private void UpdateSession(bool showError)
        {
            if (showError)
            {
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
                cfgArchivingKpiViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            cfgArchivingKpiViewDTO = cfgTablesMGR.FindAllCfgArchivingKpi(context);

            if (!cfgArchivingKpiViewDTO.hasError() && cfgArchivingKpiViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.CfgTablesMGR.CfgArchivingKpiList, cfgArchivingKpiViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(cfgArchivingKpiViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgArchivingKpiViewDTO.Errors);
            }

            upGrid.Update();
            CallJsGridView();
        }

        private void PopulateGrid()
        {
            cfgArchivingKpiViewDTO = (GenericViewDTO<CfgArchiving>)Session[WMSTekSessions.CfgTablesMGR.CfgArchivingKpiList];

            if (cfgArchivingKpiViewDTO != null)
            {
                grdMgr.PageIndex = currentPage;

                grdMgr.DataSource = cfgArchivingKpiViewDTO.Entities;
                grdMgr.DataBind();

                ucStatus.ShowRecordInfo(cfgArchivingKpiViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
                cfgArchivingKpiViewDTO = (GenericViewDTO<CfgArchiving>)Session[WMSTekSessions.CfgTablesMGR.CfgArchivingKpiList];

                hidEditId.Value = cfgArchivingKpiViewDTO.Entities[index].Id.ToString();
                txtCode.Text = cfgArchivingKpiViewDTO.Entities[index].ArchivingCode;
                txtName.Text = cfgArchivingKpiViewDTO.Entities[index].ArchivingName;
                txtDescription.Text = cfgArchivingKpiViewDTO.Entities[index].Description;
                chkStatus.Checked = cfgArchivingKpiViewDTO.Entities[index].Status;

                if (cfgArchivingKpiViewDTO.Entities[index].StartDate != DateTime.MinValue)
                {
                    txtStartDate.Text = cfgArchivingKpiViewDTO.Entities[index].StartDate.ToString("dd/MM/yyyy");
                    txtStartHour.Text = cfgArchivingKpiViewDTO.Entities[index].StartTime.ToString("HH:mm");
                }

                txtEndDate.Text = string.Empty;
                txtEndHour.Text = string.Empty;

                if (cfgArchivingKpiViewDTO.Entities[index].EndDate != DateTime.MinValue)
                {
                    txtEndDate.Text = cfgArchivingKpiViewDTO.Entities[index].EndDate.ToString("dd/MM/yyyy");
                    txtEndHour.Text = cfgArchivingKpiViewDTO.Entities[index].EndTime.ToString("HH:mm");
                }

                txtFrequenceInterval.Text = cfgArchivingKpiViewDTO.Entities[index].FrequenceInterval.ToString();
                txtPagination.Text = cfgArchivingKpiViewDTO.Entities[index].Pagination.ToString();

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
            var cfgArchiving = new CfgArchiving();
            cfgArchiving.ArchivingCode = txtCode.Text.Trim();
            cfgArchiving.ArchivingName = txtName.Text.Trim();
            cfgArchiving.Description = txtDescription.Text.Trim();
            cfgArchiving.Status = chkStatus.Checked;
            cfgArchiving.FrequenceInterval = int.Parse(txtFrequenceInterval.Text.Trim());

            DateTime finalStartDate;
            bool flagStartDate = DateTime.TryParseExact(txtStartDate.Text.Trim() + " " + txtStartHour.Text.Trim(), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out finalStartDate);
            cfgArchiving.StartDate = finalStartDate;
            cfgArchiving.StartTime = finalStartDate;

            bool flagEndDate = true;
            if (!string.IsNullOrEmpty(txtEndDate.Text.Trim()))
            {
                DateTime finalEndDate;
                flagEndDate = DateTime.TryParseExact(txtEndDate.Text.Trim() + " " + txtEndHour.Text.Trim(), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out finalEndDate);
                cfgArchiving.EndDate = finalEndDate;
                cfgArchiving.EndTime = finalEndDate;
            }
            else
            {
                cfgArchiving.EndDate = DateTime.MinValue;
                cfgArchiving.EndTime = DateTime.MinValue;
            }

            if (!flagStartDate || !flagEndDate)
            {
                ucStatus.ShowMessage(lblErrorDateFormat.Text);
                return;
            }

            cfgArchiving.Pagination = int.Parse(txtPagination.Text.Trim());

            if (hidEditId.Value == "0")
            {
                cfgArchivingKpiViewDTO = cfgTablesMGR.MaintainCfgArchivingKpi(CRUD.Create, cfgArchiving, context);
            }
            else
            {
                cfgArchiving.Id = int.Parse(hidEditId.Value);
                cfgArchivingKpiViewDTO = cfgTablesMGR.MaintainCfgArchivingKpi(CRUD.Update, cfgArchiving, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (cfgArchivingKpiViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(cfgArchivingKpiViewDTO.MessageStatus.Message);
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