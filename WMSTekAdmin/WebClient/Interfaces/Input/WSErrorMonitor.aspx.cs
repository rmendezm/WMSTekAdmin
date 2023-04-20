using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Integration;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Interfaces.Input
{
    public partial class WSErrorMonitor : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<WServiceMessageIfz> wServiceMessageIfzDTO = new GenericViewDTO<WServiceMessageIfz>();
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
        #endregion

        #region "Eventos"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
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

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "SetDivs();", true);
            }
            catch (Exception ex)
            {
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnReprocess = e.Row.FindControl("btnReprocess") as ImageButton;

                    if (btnReprocess != null) btnReprocess.OnClientClick = "if(confirm('" + lblConfirm.Text + "')==false){return false;}";

                    
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");


                        //e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Reprocess")
                {
                    int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);
                    ReprocessWServiceMessage(index);
                }
            }
            catch (Exception ex)
            {
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
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
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                wServiceMessageIfzDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
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

            if (!Page.IsPostBack)
            {

            }
            else
            {
                if (ValidateSession(WMSTekSessions.WSErrorMonitor.List))
                {
                    wServiceMessageIfzDTO = (GenericViewDTO<WServiceMessageIfz>)Session[WMSTekSessions.WSErrorMonitor.List];
                    isValidViewDTO = true;
                }

                if (isValidViewDTO)
                {
                    PopulateGrid();
                }
            }
        }

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.codeNumericVisible = true;
            this.Master.ucMainFilter.codeNumericLabel = lblTicketId.Text;

            //this.Master.ucMainFilter.codeAltVisible = true;
            //this.Master.ucMainFilter.codeLabelAlt = this.lblReferenceNumbDoc.Text;

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
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            wServiceMessageIfzDTO = iIntegrationMGR.WServiceMessageIfzWithErrors(context);

            if (!wServiceMessageIfzDTO.hasError() && wServiceMessageIfzDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.WSErrorMonitor.List, wServiceMessageIfzDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(wServiceMessageIfzDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(wServiceMessageIfzDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            wServiceMessageIfzDTO = (GenericViewDTO<WServiceMessageIfz>)Session[WMSTekSessions.WSErrorMonitor.List];

            if (wServiceMessageIfzDTO != null)
            {
                grdMgr.PageIndex = currentPage;

                if (!wServiceMessageIfzDTO.hasConfigurationError() && wServiceMessageIfzDTO.Configuration != null && wServiceMessageIfzDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdMgr, wServiceMessageIfzDTO.Configuration);

                grdMgr.DataSource = wServiceMessageIfzDTO.Entities;
                grdMgr.DataBind();

                ucStatus.ShowRecordInfo(wServiceMessageIfzDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
            }
        }

        protected void ReloadData()
        {
            UpdateSession();

            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        protected void ReprocessWServiceMessage(int index)
        {
            wServiceMessageIfzDTO = (GenericViewDTO<WServiceMessageIfz>)Session[WMSTekSessions.WSErrorMonitor.List];

            var wServiceMessageSelected = wServiceMessageIfzDTO.Entities[index];

            var wServiceMessageIfzViewDTO = iIntegrationMGR.ReprocessWServiceMessage(wServiceMessageSelected.Id, context);

            if (wServiceMessageIfzViewDTO.hasError())
            {
                ucStatus.ShowMessage(wServiceMessageIfzViewDTO.Errors.Message);
            }
            else
            {
                UpdateSession();
                upGrid.Update();
                ucStatus.ShowMessage(wServiceMessageIfzViewDTO.MessageStatus.Message);
            }
        }

        #endregion
    }
}