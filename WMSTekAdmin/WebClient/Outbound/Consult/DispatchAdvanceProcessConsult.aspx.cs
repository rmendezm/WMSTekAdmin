﻿using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class DispatchAdvanceProcessConsult : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<DispatchAdvanced> dispatchAdvanceViewDTO = new GenericViewDTO<DispatchAdvanced>();
        private bool isValidViewDTO = false;

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

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                }
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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

            if (Page.IsPostBack)
            {
                if (ValidateSession(WMSTekSessions.OutboundConsult.DispatchByProcessList))
                {
                    dispatchAdvanceViewDTO = (GenericViewDTO<DispatchAdvanced>)Session[WMSTekSessions.OutboundConsult.DispatchByProcessList];
                    isValidViewDTO = true;
                }
            }
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.outboundTypeVisible = true;
            this.Master.ucMainFilter.outboundTypeNotIncludeAll = false;
            this.Master.ucMainFilter.outboundTypeAll = false;
            this.Master.ucMainFilter.OutboundTypeCode = new string[] { };

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.DispatchAdvanceDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.DispatchAdvanceDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;

            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.btnExcelVisible = true;
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            //ucStatus.pageSizeChanged += new EventHandler(ucStatus_pageSizeChanged);
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

            var idOutboundTypes = new List<int>();

            var ddlFilterOutboundType = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOutboundType");

            if (ddlFilterOutboundType != null)
            {
                if (ddlFilterOutboundType.SelectedValue == "-1")
                {
                    foreach (ListItem item in ddlFilterOutboundType.Items)
                    {
                        if (item.Value != "-1")
                        {
                            idOutboundTypes.Add(int.Parse(item.Value));
                        }
                    }
                }
                else
                {
                    idOutboundTypes.Add(int.Parse(ddlFilterOutboundType.SelectedValue));
                }
            }

            dispatchAdvanceViewDTO = iDispatchingMGR.FindAllDispatchAdvancedByProcess(idOutboundTypes, context);

            if (!dispatchAdvanceViewDTO.hasError() && dispatchAdvanceViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundConsult.DispatchByProcessList, dispatchAdvanceViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(dispatchAdvanceViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            dispatchAdvanceViewDTO = (GenericViewDTO<DispatchAdvanced>)Session[WMSTekSessions.OutboundConsult.DispatchByProcessList];

            if (dispatchAdvanceViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!dispatchAdvanceViewDTO.hasConfigurationError() && dispatchAdvanceViewDTO.Configuration != null && dispatchAdvanceViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, dispatchAdvanceViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = dispatchAdvanceViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(dispatchAdvanceViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        #endregion
    }
}