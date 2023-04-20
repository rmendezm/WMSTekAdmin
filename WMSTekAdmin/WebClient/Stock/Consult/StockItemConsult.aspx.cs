using System;
using System.Data.SqlTypes;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.DTO;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Collections;

namespace Binaria.WMSTek.WebClient.Stocks
{
    public partial class StockItemConsultPage : BasePage
	{
        #region "Declaración de Variables"

        private GenericViewDTO<StockItemConsult> StockItemConsultViewDTO = new GenericViewDTO<StockItemConsult>();
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
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                        PopulateGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
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
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
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
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
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
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
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
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
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
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
            }
        }

        // TODO: Implementar en Fase 3
        //protected void ucStatus_pageSizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        grdMgr.PageSize = ucStatus.PageSize;
        //        PopulateGrid();
        //    }
        //    catch (Exception ex)
        //    {
        //        stockViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(stockViewDTO.Errors);
        //    }
        //}

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //-----> 17-03-2015
                //TextBox txtItem = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterItem");
                //if (String.IsNullOrEmpty(txtItem.Text))
                //{
                //    Master.ucDialog.ShowAlert("Parametro Requerido", "El Filtro Item el obligatorio", "");
                //    txtItem.Focus();
                //    return;
                //}

                //UpdateSession();
                    ReloadData();
            }
            catch (Exception ex)
            {
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //DropDownList ddlWhs = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterWarehouse");
                //DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
                //TextBox txtItem = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterItem");
                //DropDownList ddlKardexTipo = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterKardexType");
                //TextBox txtDateFrom = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateFrom");
                //TextBox txtDateTo = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateTo");

                //string idWhs = ddlWhs.SelectedValue.ToString();
                //string idOwn = ddlOwn.SelectedValue.ToString();
                //string idKardexT = ddlKardexTipo.SelectedValue.ToString();
                //string CodItem = txtItem.Text.Trim();
                //string dateFrom = txtDateFrom.Text.Trim();
                //string dateTo = txtDateTo.Text.Trim();
                //String[] parametros = new String[12];
               
                //Hashtable tablahash = new Hashtable();
                //tablahash.Add("IdWhs", idWhs);
                //tablahash.Add("IdOwn", idOwn);
                //tablahash.Add("ItemCode", CodItem);
                //tablahash.Add("MovementDateFrom", dateFrom);
                //tablahash.Add("MovementDateTo", dateTo);
                //tablahash.Add("KardexCode", idKardexT);

                //this.Master.ucMainFilter.LoadPrint(tablahash, TypeReport.ReportKardexPath.ToString());
            }
            catch (Exception ex)
            {
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
            }
        }

        /// <summary>
        /// Actualiza la vista actual, cargando nuevamente las variables de sesion desde base de datos. 
        /// </summary>
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                //UpdateSession();
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
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

                StockItemConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
            }

        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Métodos"


        protected void Initialize()
        {
            context.SessionInfo.IdPage = "StockItemConsult";
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                //UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.Shared.StockItemConsultList))
                {
                    StockItemConsultViewDTO = (GenericViewDTO<StockItemConsult>)Session[WMSTekSessions.Shared.StockItemConsultList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //FILTRO BASICO
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.itemVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
            //this.Master.ucMainFilter.BtnPrintClick += new EventHandler(btnPrint_Click);
            
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Estado
        /// </summary>
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);
        }

        /// <summary>
        /// Configuracion inicial de la Grilla principal
        /// </summary>
        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            // carga consulta de Stock
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            CreateFilterTypeLocationUsedForStockAvailable();

            StockItemConsultViewDTO = iWarehousingMGR.FindAllStockItemConsult(context);
            //printReport();
            if (!StockItemConsultViewDTO.hasError() && StockItemConsultViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Shared.StockItemConsultList, StockItemConsultViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(StockItemConsultViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(StockItemConsultViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!StockItemConsultViewDTO.hasConfigurationError() && StockItemConsultViewDTO.Configuration != null && StockItemConsultViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, StockItemConsultViewDTO.Configuration);

            grdMgr.DataSource = StockItemConsultViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            if (grdMgr.Rows.Count > 0)
                this.Master.ucMainFilter.printEnable = true;
            else
                this.Master.ucMainFilter.printEnable = true;

            ucStatus.ShowRecordInfo(StockItemConsultViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('StockItemConsult_FindAll', 'ctl00_MainContent_grdMgr');", true);
        }

        #endregion

    }
}
