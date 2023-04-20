using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Inbound.Administration
{
    public partial class InboundOrderCloseMgr : BasePage
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
        // Propiedad para controlar el indice activo en la grilla
     
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

                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivCharts();", true);

            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

      
        protected void btnCloseOrder_Click(object sender, EventArgs e)
        {
            try
            {
                CloseOrder();
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
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
        //        inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
        //    }
        //}

        //protected void imgbtnSearchVendor_Click(object sender, ImageClickEventArgs e)
        //{
        //    pnlPanelPoUp.Visible = true;
        //    mpeModalPopUpVendor.Show();
        //}

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
                //    LoadReceiptDetail(index);
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
            GenericViewDTO<InboundOrder> inboundAuxViewDTO = new GenericViewDTO<InboundOrder>();
            InboundOrder theInbound = new InboundOrder();
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

                    theInbound.Id = inboundOrderViewDTO.Entities[index].Id;
                    inboundAuxViewDTO = iReceptionMGR.GetInboundOrderByAnyParameter(theInbound, context);
                    grdMgrAux.DataSource = inboundAuxViewDTO.Entities;
                    grdMgrAux.DataBind();

                    LoadInboundOrderDetail(index);
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
                    ImageButton btnCloseOrder = e.Row.FindControl("btnClose") as ImageButton;

                    // Deshabilita la opcion de 'Cerrar Documento' si el documento ya está Cerrado
                    if (btnCloseOrder != null && (inboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestInboundTrack.Type.Id == (int)TrackInboundTypeName.CerradaCompleta) || (inboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestInboundTrack.Type.Id == (int)TrackInboundTypeName.CerradaIncompleta))
                    {
                        btnCloseOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_close_dis.png";
                        btnCloseOrder.Enabled = false;
                    }
                                        
                    //// Agrega atributos para cambiar el color de la fila seleccionada
                    //// Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.ToolTip = this.lblDetailOrder.Text;
                    
                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                    //e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);




                    foreach (DataControlFieldCell cell in e.Row.Cells)
                    {
                        if (cell.ContainingField.AccessibleHeaderText.ToUpper() != "ACTIONS")
                        {
                            cell.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                        }
                    }


                    //for (int i = 1; i < e.Row.Cells.Count; i++)
                    //{
                    //    e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    //}
             
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        //protected void grdMgr_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        //{
        //    try
        //    {
        //        // Valida variable de sesion del Usuario Loggeado
        //        if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
        //        {
        //            //capturo la posicion de la fila 
        //            int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
        //            currentIndex = grdMgr.SelectedIndex;

        //            LoadInboundOrderDetail(index);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
        //    }
        //}

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
        

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

               // LoadInboundOrderDetail(editIndex);
                ShowModalCloseOrder(editIndex);

            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }               


        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "CloseOrder")
                {
                    int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32( e.CommandArgument);
                    //int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    LoadInboundOrderDetail(index);
                    ShowModalCloseOrder(index);
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }


        protected void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                //Rescata el indice de la grilla seleccionado
                int index = int.Parse(this.hdIndexGrd.Value);

                LoadInboundOrderDetail(index);
                ShowModalCloseOrder(index);

                isValidViewDTO = false;
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

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "InboundOrderCloseMgr";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .35);
                //UpdateSession(false);
            }
            else
            {
                if (ValidateSession(WMSTekSessions.InboundConsult.InboundOrderList))
                {
                    inboundOrderViewDTO = (GenericViewDTO<InboundOrder>)Session[WMSTekSessions.InboundConsult.InboundOrderList];
                    isValidViewDTO = true;
                }

                // Si es un ViewDTO valido, carga la grilla
                if (isValidViewDTO)
                {
                    //Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                    PopulateGrid();
                }
            }
        }

        protected void LoadInboundOrderDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                int id = inboundOrderViewDTO.Entities[index].Id;

                inboundDetailViewDTO = iReceptionMGR.LoadDetailByIdInbound(context, id);
                lblGridDetail.Visible = true;
                this.lblNroDoc.Text = inboundOrderViewDTO.Entities[index].Number;

                if (inboundDetailViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!inboundDetailViewDTO.hasConfigurationError() && inboundDetailViewDTO.Configuration != null && inboundDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, inboundDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = inboundDetailViewDTO.Entities;
                    grdDetail.DataBind();
                    Session.Add(WMSTekSessions.InboundConsult.InboundOrderDetailsList, inboundDetailViewDTO);

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
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;

            this.Master.ucMainFilter.inboundTypeVisible = true;
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

            this.Master.ucTaskBar.btnNewVisible = false;
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

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
                inboundOrderViewDTO.ClearError();
            }
            
            this.Master.ucMainFilter.ClearFilterObject();
            this.Master.ucMainFilter.LoadControlValuesToFilterObject();

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Carga lista de InboundOrders
            inboundOrderViewDTO = iReceptionMGR.FindAllInboundOrder(context);

            if (!inboundOrderViewDTO.hasError() && inboundOrderViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InboundConsult.InboundOrderList, inboundOrderViewDTO);
                
                isValidViewDTO = true;

                if (!crud)
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
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                currentIndex = -1;
                divDetail.Visible = false;
                divGrid.Visible = true;
                this.Master.ucError.ClearError();
            }
        }

        /// <summary>
        /// Muestra ventana modal para cerrar la Orden seleccionada
        /// </summary>
        /// <param name="index"></param>
        protected void ShowModalCloseOrder(int index)
        {
            base.LoadTrackInboundClose(this.ddlTrackInbound, inboundOrderViewDTO.Entities[index].Id, true, this.Master.EmptyRowText);

            this.lblWarehouse2.Text = string.Empty;
            this.lblInboundType2.Text = string.Empty;
            this.lblNroDoc2.Text = string.Empty;
            this.lblVendor2.Text = string.Empty;

            if (inboundOrderViewDTO.Entities[index].Warehouse != null) this.lblWarehouse2.Text = inboundOrderViewDTO.Entities[index].Warehouse.Name;
            if (inboundOrderViewDTO.Entities[index].InboundType != null) this.lblInboundType2.Text = inboundOrderViewDTO.Entities[index].InboundType.Code;
            if (inboundOrderViewDTO.Entities[index].Number != null) this.lblNroDoc2.Text = inboundOrderViewDTO.Entities[index].Number;
            if (inboundOrderViewDTO.Entities[index].Vendor != null) this.lblVendor2.Text = inboundOrderViewDTO.Entities[index].Vendor.Name;

            hidEditId.Value = inboundOrderViewDTO.Entities[index].Id.ToString();

            divCloseOrder.Visible = true;
            mpCloseOrder.Show();
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            // Editar Documento
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = inboundOrderViewDTO.Entities[index].Id.ToString();
            }

            if (inboundOrderViewDTO.Configuration != null && inboundOrderViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(inboundOrderViewDTO.Configuration, true);
                else
                    base.ConfigureModal(inboundOrderViewDTO.Configuration, false);
            }

            // La propiedad Inbound Track es de solo lectura
               divGrid.Visible = false;
        }

        /// <summary>
        /// Cambia el estado (track) del documento a 'Recibido Completo', 'Completa' o 'Cerrada'
        /// </summary>
        protected void CloseOrder()
        {
            GenericViewDTO<InboundTrack> inboundTrackViewDTO = new GenericViewDTO<InboundTrack>();

            inboundOrderViewDTO = (GenericViewDTO<InboundOrder>)Session[WMSTekSessions.InboundConsult.InboundOrderList];
            var closeInboundOrder = inboundOrderViewDTO.Entities.Where(io => io.Id == Convert.ToInt32(hidEditId.Value)).First();

            //string Codcierre = ddlTrackInbound.SelectedValue;
           int codcierre = Convert.ToInt16(ddlTrackInbound.SelectedValue);

            if (codcierre.Equals((int)TrackInboundTypeName.CerradaCompleta))
            {
                if (fnc_ordencompleta())
                {
                    //Master.ucDialog.ShowAlert("Informacion", "Orden Incompleta,Debe elegir Opcion Cerrada Incompleta", "");
                    inboundOrderViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.Close2.OrdenInComplete, context));
                    this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
                    return;
                }
            }
            else 
            {
                if (!fnc_ordencompleta())
                {
                    inboundOrderViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.Close.OrdenComplete, context));
                    this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
                    return;
                }
            } 

            inboundTrackViewDTO = iReceptionMGR.ChangeInboundOrderTrack(context, (TrackInboundTypeName)Convert.ToInt16(ddlTrackInbound.SelectedValue), closeInboundOrder);

            if (inboundTrackViewDTO.hasError())
            {
                inboundOrderViewDTO.Errors = inboundTrackViewDTO.Errors;
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(inboundTrackViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }
        private Boolean fnc_ordencompleta()
        {
            GenericViewDTO<InboundDetail> inboundDetailValidation =(GenericViewDTO<InboundDetail>)Session[WMSTekSessions.InboundConsult.InboundOrderDetailsList];
            foreach (InboundDetail inbounddetailtmp in inboundDetailValidation.Entities) 
            {
                if (inbounddetailtmp.ItemQty < inbounddetailtmp.Received)
                {
                    return false;
                }
                    
            }
            return true;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('InboundOrder_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr', 'InboundOrderCloseMgr');", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDropCustom();", true);
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
