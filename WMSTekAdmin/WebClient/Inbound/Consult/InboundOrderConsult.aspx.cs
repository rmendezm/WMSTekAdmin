using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Inbound.Consult
{
    public partial class InboundOrderWebConsult : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<InboundOrder> inboundOrderViewDTO = new GenericViewDTO<InboundOrder>();
        private GenericViewDTO<InboundDetail> inboundDetailViewDTO = new GenericViewDTO<InboundDetail>();
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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

                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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

                    LoadInboundOrderDetail(index);
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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

                // Capturo la posicion de la fila activa
                //int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                //if (index != -1)
                //{
                //    LoadInboundOrderDetail(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                //base.ExportToExcel(grdMgr, grdDetail, detailTitle);
                base.ExportToExcel(grdMgr, null, "_Entrada");
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            GridView grdMgrAux = new GridView();
            GenericViewDTO<InboundOrder> inboundOrderAuxViewDTO = new GenericViewDTO<InboundOrder>();
            ContextViewDTO contextAux = new ContextViewDTO();
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
                    contextAux.MainFilter = this.Master.ucMainFilter.MainFilter;
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.InboundType)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(inboundOrderViewDTO.Entities[index].Warehouse.Id.ToString()));
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(inboundOrderViewDTO.Entities[index].Owner.Id.ToString()));
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.InboundType)].FilterValues.Add(new FilterItem(inboundOrderViewDTO.Entities[index].InboundType.Id.ToString()));
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Add(new FilterItem(inboundOrderViewDTO.Entities[index].Number.ToString()));

                    inboundOrderAuxViewDTO = iReceptionMGR.FindAllInboundOrder(context);
                    grdMgrAux.DataSource = inboundOrderAuxViewDTO.Entities;
                    grdMgrAux.DataBind();

                    LoadInboundOrderDetail(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + "_Entrada" + string.Empty;

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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

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

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "InboundOrderConsult";

            InitializeFilter(!Page.IsPostBack, false);
            InitializeTaskBar();
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
                //UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.InboundConsult.InboundOrderList))
                {
                    inboundOrderViewDTO = (GenericViewDTO<InboundOrder>)Session[WMSTekSessions.InboundConsult.InboundOrderList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Retorna el detalle de cada doc de entrada
        /// </summary>
        /// <param name="index"></param>
        protected void LoadInboundOrderDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                int id = inboundOrderViewDTO.Entities[index].Id;

                inboundDetailViewDTO = iReceptionMGR.LoadDetailByIdInbound(context, id);
                this.lblNroDoc.Text = inboundOrderViewDTO.Entities[index].Number;

                if (inboundDetailViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!inboundDetailViewDTO.hasConfigurationError() && inboundDetailViewDTO.Configuration != null && inboundDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, inboundDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = inboundDetailViewDTO.Entities;
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

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.inboundTypeVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.trackInboundTypeVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.InboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.InboundDaysAfter;

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
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Carga lista de Inbound
            inboundOrderViewDTO = iReceptionMGR.FindAllInboundOrder(context);

            if (!inboundOrderViewDTO.hasError() && inboundOrderViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InboundConsult.InboundOrderList, inboundOrderViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(inboundOrderViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!inboundOrderViewDTO.hasConfigurationError() && inboundOrderViewDTO.Configuration != null && inboundOrderViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, inboundOrderViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = inboundOrderViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(inboundOrderViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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

        private void CallJsGridViewHeader()
        {
           ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('InboundOrderDetail_ById', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }
        #endregion
    }
}

