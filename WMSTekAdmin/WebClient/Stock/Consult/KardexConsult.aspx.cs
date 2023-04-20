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
	public partial class KardexConsult : BasePage
	{
        #region "Declaración de Variables"

        private GenericViewDTO<KardexGridView> KardexGridViewViewDTO = new GenericViewDTO<KardexGridView>();
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
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
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
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
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
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
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
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
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
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
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
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
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
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
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
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
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
                ReloadData();
            }
            catch (Exception ex)
            {
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlWhs = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterWarehouse");
                DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
                TextBox txtItem = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterItem");
                DropDownList ddlKardexTipo = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterKardexType");
                TextBox txtDateFrom = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateFrom");
                TextBox txtDateTo = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateTo");

                string idWhs = ddlWhs.SelectedValue.ToString();
                string idOwn = ddlOwn.SelectedValue.ToString();
                string idKardexT = (ddlKardexTipo.SelectedValue.ToString()== "-1" ? "" : ddlKardexTipo.SelectedValue.ToString());
                string CodItem = txtItem.Text.Trim();
                string dateFrom = txtDateFrom.Text.Trim();
                string dateTo = txtDateTo.Text.Trim();
                String[] parametros = new String[12];
               
                Hashtable tablahash = new Hashtable();
                tablahash.Add("IdWhs", idWhs);
                tablahash.Add("IdOwn", idOwn);
                tablahash.Add("ItemCode", CodItem);
                tablahash.Add("MovementDateFrom", dateFrom);
                tablahash.Add("MovementDateTo", dateTo);
                tablahash.Add("KardexCode", idKardexT);

                this.Master.ucMainFilter.LoadPrint(tablahash, TypeReport.ReportKardexPath.ToString());
            }
            catch (Exception ex)
            {
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
            }
        }

        /// <summary>
        /// Actualiza la vista actual, cargando nuevamente las variables de sesion desde base de datos. 
        /// </summary>
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
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

                KardexGridViewViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
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
            context.SessionInfo.IdPage = "KardexConsult";
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (Page.IsPostBack)
            {
                UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.Shared.KardexGridViewList))
                {
                    KardexGridViewViewDTO = (GenericViewDTO<KardexGridView>)Session[WMSTekSessions.Shared.KardexGridViewList];
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
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.parameterKardexType = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.printVisible = true;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.DateBefore = CfgParameterName.StockDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.StockDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
            this.Master.ucMainFilter.BtnPrintClick += new EventHandler(btnPrint_Click);
            
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

            //CheckBox pchkFilterDateFrom = (CheckBox)this.Master.ucMainFilter.FindControl("chkFilterDateFrom");
            //CheckBox pchkFilterDateTo = (CheckBox)this.Master.ucMainFilter.FindControl("chkFilterDateTo");

            //if (!pchkFilterDateFrom.Checked)
            //{
            //    Master.ucDialog.ShowAlert("Parametro Requerido", "El Filtro Fecha Desde el obligatorio", "");
            //    pchkFilterDateFrom.Focus();
            //    return;
            //}
            //if (!pchkFilterDateTo.Checked)
            //{
            //    Master.ucDialog.ShowAlert("Parametro Requerido", "El Filtro Fecha Hasta el obligatorio", "");
            //    pchkFilterDateTo.Focus();
            //    return;
            //}


            // carga consulta de Stock
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            KardexGridViewViewDTO = iWarehousingMGR.FindAllKardex(context);
            //printReport();
            if (!KardexGridViewViewDTO.hasError() && KardexGridViewViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Shared.KardexGridViewList, KardexGridViewViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(KardexGridViewViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(KardexGridViewViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!KardexGridViewViewDTO.hasConfigurationError() && KardexGridViewViewDTO.Configuration != null && KardexGridViewViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, KardexGridViewViewDTO.Configuration);

            grdMgr.DataSource = KardexGridViewViewDTO.Entities;
            grdMgr.DataBind();

            if (grdMgr.Rows.Count > 0)
                this.Master.ucMainFilter.printEnable = true;
            else
                this.Master.ucMainFilter.printEnable = true;

            ucStatus.ShowRecordInfo(KardexGridViewViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
            
        }

        protected void ReloadData()
        {
            CheckBox pchkFilterDateFrom = (CheckBox)this.Master.ucMainFilter.FindControl("chkFilterDateFrom");
            CheckBox pchkFilterDateTo = (CheckBox)this.Master.ucMainFilter.FindControl("chkFilterDateTo");

            if (!pchkFilterDateFrom.Checked)
            {
                Master.ucDialog.ShowAlert("Parametro Requerido", "El Filtro Fecha Desde el obligatorio", "");
                pchkFilterDateFrom.Focus();
                KardexGridViewViewDTO = new GenericViewDTO<KardexGridView>();
                return;
            }
            if (!pchkFilterDateTo.Checked)
            {
                Master.ucDialog.ShowAlert("Parametro Requerido", "El Filtro Fecha Hasta el obligatorio", "");
                pchkFilterDateTo.Focus();
                KardexGridViewViewDTO = new GenericViewDTO<KardexGridView>();
                return;
            }

            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        public void printReport()
        {
            List<ReportParameter> reportParameter;
            this.rptViewKardex.ServerReport.ReportPath = MiscUtils.ReadSetting("ReportKardexPath", string.Empty);
            this.rptViewKardex.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

            //Credenciales
            int total = rptViewKardex.ServerReport.GetDataSources().Count;
            DataSourceCredentials[] permisos = new DataSourceCredentials[total];

            ReportDataSourceInfoCollection datasources = rptViewKardex.ServerReport.GetDataSources();
            for (int j = 0; j < total; j++)
            {
                permisos[j].Name = datasources[j].Name;
                permisos[j].UserId = "";
                permisos[j].Password = "";
            }
            rptViewKardex.ServerReport.SetDataSourceCredentials(permisos);

            DropDownList ddlWhs = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterWarehouse");
            DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
            TextBox txtItem = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterItem");
            DropDownList ddlKardexTipo = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterKardexType");
            TextBox txtDateFrom = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateFrom");
            TextBox txtDateTo = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateTo");


            string idWhs = ddlWhs.SelectedValue.ToString();
            string idOwn = ddlOwn.SelectedValue.ToString();
            string idKardexT = ddlKardexTipo.SelectedValue.ToString();
            string CodItem = txtItem.Text.Trim();
            string dateFrom = txtDateFrom.Text.Trim();
            string dateTo = txtDateTo.Text.Trim();

            reportParameter = new List<ReportParameter>();
            reportParameter.Clear();

            // Añado los parámetros necesarios.
            reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
            reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
            reportParameter.Add(new ReportParameter("ItemCode", CodItem, false));
            reportParameter.Add(new ReportParameter("MovementDateFrom", dateFrom, false));
            reportParameter.Add(new ReportParameter("MovementDateTo", dateTo, false));
            reportParameter.Add(new ReportParameter("KardexCode", idKardexT, false));
            reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));

            rptViewKardex.Attributes.Add("style", "margin-bottom: 50px;");
            rptViewKardex.ShowPrintButton = false;
            // Añado el/los parámetro/s al ReportViewer.
            this.rptViewKardex.ServerReport.SetParameters(reportParameter);

            rptViewKardex.DataBind();

            divReport.Visible = true;

            rptViewKardex.ServerReport.Refresh();

        }

        #endregion

    }
}
