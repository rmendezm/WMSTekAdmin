using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class OutboundOrderReadyForDispatch : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<OutboundOrderConsult> outboundOrderConsultViewDTO = new GenericViewDTO<OutboundOrderConsult>();
        private GenericViewDTO<OutboundOrderConsult> outboundDetailViewDTO = new GenericViewDTO<OutboundOrderConsult>();

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
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
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
                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
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
                    e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                }
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //capturo la posicion de la fila 
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndex = grdMgr.SelectedIndex;

                    LoadOutboundOrderDetail(index);
                }
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewDetailOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        // Carga la grilla, filtrada por el criterio de busqueda ingresado
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
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
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            //string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                //// Capturo la posicion de la fila activa
                //int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                //if (index != -1)
                //{
                //    LoadOutboundOrderDetail(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                //base.ExportToExcel(grdMgr, grdDetail, detailTitle);
                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            GridView grdMgrAux = new GridView();
            GenericViewDTO<OutboundOrderConsult> outboundOrderConsultAuxViewDTO = new GenericViewDTO<OutboundOrderConsult>();
            OutboundOrderConsult theOutboundOrderConsult = new OutboundOrderConsult();
            string detailTitle;

            try
            {
                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    grdMgr.AllowPaging = false;
                    PopulateGrid();

                    grdMgrAux = grdMgr;
                    theOutboundOrderConsult.Outbound = new OutboundOrder();
                    theOutboundOrderConsult.Outbound.Id = outboundOrderConsultViewDTO.Entities[index].Outbound.Id;
                    outboundOrderConsultAuxViewDTO = iDispatchingMGR.GetReadyForDispatchByAnyParameter(theOutboundOrderConsult, context);
                    grdMgrAux.DataSource = outboundOrderConsultAuxViewDTO.Entities;
                    grdMgrAux.DataBind();

                    LoadOutboundOrderDetail(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                    base.ExportToExcel(grdMgrAux, grdDetail, detailTitle);
                    grdMgr.AllowPaging = true;
                }


            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        // TODO: Implementar en Fase 3
        //protected void ucStatus_pageSizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        grdMgr.PageSize = ucStatus.PageSize;
        //        currentIndex = -1;
        //        divDetail.Visible = false;
        //        PopulateGrid();
        //    }
        //    catch (Exception ex)
        //    {
        //        outboundOrderConsultViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
        //    }
        //}

        //protected void imgbtnSearchVendor_Click(object sender, ImageClickEventArgs e)
        //{
        //    pnlPanelPoUp.Visible = true;
        //    mpeModalPopUpVendor.Show();
        //}

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "SatisfactionLevel";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
                UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.OutboundConsult.OutboundOrderList))
                {
                    outboundOrderConsultViewDTO = (GenericViewDTO<OutboundOrderConsult>)Session[WMSTekSessions.OutboundConsult.OutboundOrderList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //Tipos de Tareas
            this.Master.ucMainFilter.listTrackOutboundType = new System.Collections.Generic.List<String>();
            this.Master.ucMainFilter.listTrackOutboundType = GetConst("OutboundOrderReadyForDispatchTrackOutboundType");
            this.Master.ucMainFilter.listOutboundType = new System.Collections.Generic.List<String>();
            this.Master.ucMainFilter.listOutboundType = GetConst("OutboundOrderReadyForDispatchOutboundType");


            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.dateVisible = true;
            this.Master.ucMainFilter.chkDateVisible = false;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.outboundTypeVisible = true;
            this.Master.ucMainFilter.trackOutboundTypeNotIncludeAll = true;
            this.Master.ucMainFilter.trackOutboundTypeVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.setDateLabel = true;
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.nameLabel = lblName.Text;
            this.Master.ucMainFilter.descriptionLabel = lblDescription.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

            //Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabDispatchingVisible = true;
            this.Master.ucMainFilter.divDispatchingPriorityVisible = false;
            this.Master.ucMainFilter.txtDispatchingCustomerVisible = false;
            this.Master.ucMainFilter.OutboundTypeCode = new string[] { };

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Estado
        /// </summary>
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            // ucStatus.pageSizeChanged += new EventHandler(ucStatus_pageSizeChanged);
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
            // Carga lista de OutboundOrder
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            outboundOrderConsultViewDTO = iDispatchingMGR.FindReadyForDispatch(context);

            if (!outboundOrderConsultViewDTO.hasError() && outboundOrderConsultViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundConsult.OutboundOrderList, outboundOrderConsultViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(outboundOrderConsultViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(outboundOrderConsultViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!outboundOrderConsultViewDTO.hasConfigurationError() && outboundOrderConsultViewDTO.Configuration != null && outboundOrderConsultViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, outboundOrderConsultViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = outboundOrderConsultViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(outboundOrderConsultViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
            }
        }

        /// <summary>
        /// Retorna el detalle de cada doc de entrada
        /// </summary>
        /// <param name="index"></param>
        protected void LoadOutboundOrderDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                int id = outboundOrderConsultViewDTO.Entities[index].Id;

                outboundDetailViewDTO = iDispatchingMGR.FindReadyForDispatchDetail(context, id);

                if (outboundDetailViewDTO.Entities.Count > 0)
                    this.lblNroDoc.Text = outboundOrderConsultViewDTO.Entities[index].Outbound.Number;

                if (outboundDetailViewDTO != null && outboundDetailViewDTO.Entities.Count > 0)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!outboundDetailViewDTO.hasConfigurationError() && outboundDetailViewDTO.Configuration != null && outboundDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, outboundDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = outboundDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }

                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('ReadyForDispatchDetail_FindAll', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }

        #endregion

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}
