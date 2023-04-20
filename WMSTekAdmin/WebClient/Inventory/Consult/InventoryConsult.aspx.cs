using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Inventory;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.WebClient.Base;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Inventory.Consult
{
    public partial class InventoryConsult : BasePage
    {
        #region "Declaración de Variables"
        private List<EntityFilter> mainFilter;
        private GenericViewDTO<InventoryOrder> inventoryViewDTO = new GenericViewDTO<InventoryOrder>();
        private GenericViewDTO<InventoryDetail> inventoryDetailViewDTO = new GenericViewDTO<InventoryDetail>();
        private bool isValidViewDTO = false;


        private GenericViewDTO<Task> taskViewDTO;
        private GenericViewDTO<TaskDetail> taskDetailViewDTO;

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

        // Propiedad para actualizar o no grilla principal
        public bool showDetail
        {
            get
            {
                if (ValidateViewState("showDetail"))
                    return (bool)ViewState["showDetail"];
                else
                    return false;
            }

            set { ViewState["showDetail"] = value; }
        }
        
        #endregion

        #region "Eventos"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    context.SessionInfo.IdPage = "InventoryConsult";

                    InitializeFilter(!Page.IsPostBack, false);
                    InitializeTaskBar();
                    InitializeStatusBar();
                    InitializeGrids();

                    if (!Page.IsPostBack)
                    {
                        UpdateSession();
                        PopulateLists();
                    }
                    else
                    {
                        if (ValidateSession(WMSTekSessions.InventoryConsult.InventoryList))
                        {
                            inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryConsult.InventoryList];
                            isValidViewDTO = true;

                            if (!showDetail) PopulateGrid();
                        }
                    }                
                }
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                        if (!showDetail) PopulateGrid();
                        PopulateInventoryDetail();
                    }
                }


                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Capturo la posicion de la fila 
                    currentIndex = grdMgr.SelectedIndex;
                    currentPage = 0;

                    // Limpia el filtro de detalle de Inventario
                    mainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];
                    ClearFilterObject();
                    this.ddlLocationType.SelectedIndex = -1;
                    this.txtIdLocCode.Text = string.Empty;
                    this.ddlCounted.SelectedIndex = -1;
                    this.ddlEmpty.SelectedIndex = -1;
                    this.txtItemCode.Text = string.Empty;
                    this.txtDifQty.Text = string.Empty;
                    this.txtDifAmount.Text = string.Empty;

                    // Mensaje a mostrar en las páginas que no cargan inicialmente la grilla
                    inventoryViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
                    ucStatus.ShowMessage(inventoryViewDTO.MessageStatus.Message);

                    // Oculta la grilla detalle (se muestra al Buscar en el filtro detalle)
                    divGrid.Visible = false;

                    // TODO: ver por qué no oculta el nro de registros
                    ucStatus.HideRecordInfo();

                    // Muestra información del inventario en el encabezado del detalle
                    this.lblNroInv.Text = inventoryViewDTO.Entities[currentIndex].Number + " - " + inventoryViewDTO.Entities[currentIndex].Description;
                }
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (grdDetail.Visible == true)
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        if (inventoryDetailViewDTO.Entities != null && inventoryDetailViewDTO.Entities.Count > 0)
                        {
                            ImageButton btnGenerateRecountTask = e.Row.FindControl("btnInventoryRecount") as ImageButton;

                            if (btnGenerateRecountTask != null)
                            {
                                //if (inventoryDetailViewDTO.Entities[e.Row.DataItemIndex].RecountTask == 0 && inventoryDetailViewDTO.Entities[e.Row.DataItemIndex].Id != -1)
                                if (inventoryViewDTO.Entities[currentIndex].TrackInventoryType.Id == (int)TrackInventoryTypeName.EnEjecucion &&
                                    inventoryDetailViewDTO.Entities[e.Row.DataItemIndex].InventoryLocation.WasCounted && inventoryDetailViewDTO.Entities[e.Row.DataItemIndex].Id > 0 &&
                                    inventoryDetailViewDTO.Entities[e.Row.DataItemIndex].RecountTask <= 0)
                                {
                                    btnGenerateRecountTask.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_apply_inventory_on.png";
                                    btnGenerateRecountTask.Enabled = true;

                                }
                                else
                                {
                                    bool exists = false;

                                    var x = inventoryDetailViewDTO.Entities.Where(w => w.Location.IdCode == inventoryDetailViewDTO.Entities[e.Row.DataItemIndex].Location.IdCode &&
                                                  w.LPN.Code == inventoryDetailViewDTO.Entities[e.Row.DataItemIndex].LPN.Code).ToList();

                                    if (x.Count > 1)
                                    {
                                        if (inventoryViewDTO.Entities[currentIndex].TrackInventoryType.Id == (int)TrackInventoryTypeName.EnEjecucion)
                                        {
                                            var y = (from a in x.ToList()
                                                     where a.InventoryLocation.WasCounted && a.Id > 0 && a.RecountTask <= 0
                                                     select a).ToList();

                                            if (y.Count > 0)
                                            {
                                                btnGenerateRecountTask.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_apply_inventory_on.png";
                                                btnGenerateRecountTask.Enabled = true;
                                                exists = true;
                                            }
                                        }
                                    }

                                    if (!exists)
                                    {
                                        btnGenerateRecountTask.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_apply_inventory_dis.png";
                                        btnGenerateRecountTask.Enabled = false;
                                    }

                                }
                            }                        
                        }
                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Attributes.Add("onclick", "gridViewDetailOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                        e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                        e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    }
                }
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void grdDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                inventoryDetailViewDTO = (GenericViewDTO<InventoryDetail>)Session[WMSTekSessions.InventoryConsult.InventoryDetailList];
                int index = grdDetail.PageSize * grdDetail.PageIndex + ((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex;
                //int index = ((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex;
                //int editId = (Convert.ToInt32(this.grdDetail.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));
                
                string taskLocation = inventoryDetailViewDTO.Entities[index].Location.IdCode;
                string taskLPN = inventoryDetailViewDTO.Entities[index].LPN.IdCode;
                //int taskItem = inventoryDetailViewDTO.Entities[index].Item.Id;

                if (e.CommandName == "GenerateRecountTask")
                {
                    string typeTaskCode = string.Empty;
                    //List<string> listaConstTypeTask = new List<string>();
                    //listaConstTypeTask = GetConst("TypeTaskCodes");
                    //typeTaskCode = listaConstTypeTask[Convert.ToInt16(TypeTaskCode.RECO)];
                    typeTaskCode = TypeTaskCode.RECO.ToString();
                    
                    GenericViewDTO<InventoryDetail> inventoryDetailViewDTO_tasks = new GenericViewDTO<InventoryDetail>();

                    foreach (InventoryDetail inventoryDetail in inventoryDetailViewDTO.Entities)
                    {
                        if (inventoryDetail.Location.IdCode == taskLocation && 
                            inventoryDetail.LPN.IdCode == taskLPN && inventoryDetail.Id > 0 )
                            //inventoryDetail.Item.Id == taskItem)
                        {
                            inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryConsult.InventoryList];

                            InventoryOrder inventoryOrder = inventoryViewDTO.Entities.Find(p => p.Id == inventoryDetail.InventoryOrder.Id);
                            inventoryDetail.InventoryOrder.Owner.Id = inventoryOrder.Owner.Id;

                            inventoryDetailViewDTO_tasks.Entities.Add(inventoryDetail);
                         }
                     }

                    //Llama a Generar tareas de reconteo.
                    GenericViewDTO<Task> taskViewDTO = iTasksMGR.GenerateTaskInventoryRecount(context, inventoryDetailViewDTO_tasks, typeTaskCode);

                    if (!taskViewDTO.hasError())
                    {
                        // Valida variable de sesion del Usuario Loggeado
                        if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                        {
                            SearchInventoryDetail();
                        }
                    }
                    else
                    {
                        this.Master.ucError.ShowError(taskViewDTO.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                    e.Row.TableSection = TableRowSection.TableHeader;

                if (grdDetail.Visible == true)
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        //Declara y asigna variables
                        Label lblId = (Label)e.Row.FindControl("lblId");//el id del detalle
                        Image ImgCount1 = (Image)e.Row.FindControl("ImgCount");
                        Label lblDifferenceQty = (Label)e.Row.FindControl("lblDifferenceQty");//lo real en stock

                        // Controla que no venga ninguno vacio
                        if (lblDifferenceQty != null && !string.IsNullOrEmpty( lblDifferenceQty.Text.Trim()))
                        {
                            // Si el stock esta por debajo del minimo saldrá rojo
                            if (Convert.ToDecimal(lblDifferenceQty.Text) < 0)
                            {
                                lblDifferenceQty.ForeColor = System.Drawing.Color.Red;
                            }
                            // Si el stock esta por encima del maximo saldrá azul
                            else if (Convert.ToDecimal(lblDifferenceQty.Text) > 0)
                            {
                                lblDifferenceQty.ForeColor = System.Drawing.Color.Blue;
                            }
                            // Si el stock no se desborda ni falta saldrá negro.
                            else
                            {
                                lblDifferenceQty.ForeColor = System.Drawing.Color.Black;
                            }
                        }

                        // Verifica las ubicaciones que han sido contadas y les pone una imagen.
                        if (ImgCount1 != null)
                        {
                            if (lblId == null || lblId.Text == "-1")
                                ImgCount1.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_notcounted.png";
                            else
                                ImgCount1.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_counted.png";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void grdDetail_OnDataBound(object sender, EventArgs e)
        {
            try
            {
                for (int i = grdDetail.Rows.Count - 1; i > 0; i--)
                {
                    int pageInd = grdDetail.PageSize * grdDetail.PageIndex;

                    InventoryDetail inventoryDetailRow = inventoryDetailViewDTO.Entities[i + pageInd];
                    InventoryDetail inventoryDetailpreviousRow = inventoryDetailViewDTO.Entities[(i + pageInd) - 1];

                    GridViewRow row = grdDetail.Rows[i];
                    GridViewRow previousRow = grdDetail.Rows[i - 1];

                    if (inventoryDetailRow.Location.IdCode == inventoryDetailpreviousRow.Location.IdCode)
                    {
                        //Ubicación
                        if (previousRow.Cells[3].RowSpan == 0)
                        {
                            if (row.Cells[3].RowSpan == 0)
                            {
                                previousRow.Cells[3].RowSpan += 2;
                            }
                            else
                            {
                                previousRow.Cells[3].RowSpan = row.Cells[3].RowSpan + 1;
                            }
                            row.Cells[3].Visible = false;
                        }

                        
                    }

                    if (inventoryDetailRow.LPN.IdCode == inventoryDetailpreviousRow.LPN.IdCode)
                    {
                        if (inventoryDetailRow.LPN.IdCode != null && inventoryDetailpreviousRow.LPN.IdCode != null)
                        {
                            //LPN
                            if (previousRow.Cells[6].RowSpan == 0)
                            {
                                if (row.Cells[6].RowSpan == 0)
                                {
                                    previousRow.Cells[6].RowSpan += 2;
                                }
                                else
                                {
                                    previousRow.Cells[6].RowSpan = row.Cells[6].RowSpan + 1;
                                }
                                row.Cells[6].Visible = false;
                            }
                        }
                        if (inventoryDetailRow.Location.IdCode == inventoryDetailpreviousRow.Location.IdCode)
                        {
                            ////Generar Reconteo
                            if (previousRow.Cells[7].RowSpan == 0)
                            {
                                if (row.Cells[7].RowSpan == 0)
                                {
                                    previousRow.Cells[7].RowSpan += 2;
                                }
                                else
                                {
                                    previousRow.Cells[7].RowSpan = row.Cells[7].RowSpan + 1;
                                }
                                row.Cells[7].Visible = false;
                            }
                            
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                showDetail = true;
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                showDetail = true;
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                showDetail = true;
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                showDetail = true;
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdDetail.PageCount - 1;
                showDetail = true;
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        // TODO: Implementar en Fase 3
        //protected void ucStatus_pageSizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        grdDetail.PageSize = ucStatus.PageSize;
        //    }
        //    catch (Exception ex)
        //    {
        //        inventoryViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(inventoryViewDTO.Errors);
        //    }
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
                //grdDetail.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                //currentIndex = grdMgr.SelectedIndex;

                //if (currentIndex != -1)
                //{
                //    PopulateInventoryDetail();
                //    detailTitle = lblGridDetail.Text + lblNroInv.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                //base.ExportToExcel(grdMgr, grdDetail, detailTitle);
                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
                //grdDetail.AllowPaging = true;
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            GridView grdMgrAux = new GridView();
            GenericViewDTO<InventoryOrder> inventoryAuxViewDTO = new GenericViewDTO<InventoryOrder>();
            ContextViewDTO contextAux = new ContextViewDTO();
            string detailTitle;

            try
            {
                // Capturo la posicion de la fila activa
                currentIndex = grdMgr.SelectedIndex;

                if (currentIndex != -1)
                {
                    grdMgr.AllowPaging = false;
                    grdDetail.AllowPaging = false;
                    PopulateGrid();

                    grdMgrAux = grdMgr;
                    contextAux.MainFilter = this.Master.ucMainFilter.MainFilter;
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(inventoryViewDTO.Entities[currentIndex].Warehouse.Id.ToString()));
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(inventoryViewDTO.Entities[currentIndex].Owner.Id.ToString()));
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Add(new FilterItem(inventoryViewDTO.Entities[currentIndex].Number.ToString()));
                    inventoryAuxViewDTO = iInventoryMGR.FindAllInventoryAndCalcPercentProgr(context);
                    grdMgrAux.DataSource = inventoryAuxViewDTO.Entities;
                    grdMgrAux.DataBind();

                    PopulateInventoryDetail();
                    detailTitle = lblGridDetail.Text + lblNroInv.Text + string.Empty;

                    base.ExportToExcel(grdMgrAux, grdDetail, detailTitle);
                    grdMgr.AllowPaging = true;
                    grdDetail.AllowPaging = true;
                }
                
                
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        /// <summary>
        /// Opción Buscar para detalle del Inventario
        /// </summary>
        protected void btnSearch1_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    SearchInventoryDetail();
                }
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        /// <summary>
        /// Muestra el detalle de cada Inventario
        /// </summary>
        protected void PopulateInventoryDetail()
        {
            grdDetail.PageIndex = currentPage;

            if (currentIndex != -1)
            {
                divDetail.Visible = true;

                if (ValidateSession(WMSTekSessions.InventoryConsult.InventoryDetailList))
                {
                    inventoryDetailViewDTO = (GenericViewDTO<InventoryDetail>)Session[WMSTekSessions.InventoryConsult.InventoryDetailList];

                    int id = inventoryViewDTO.Entities[currentIndex].Id;

                    // Configura ORDEN de las columnas de la grilla
                    if (!inventoryDetailViewDTO.hasConfigurationError() && inventoryDetailViewDTO.Configuration != null && inventoryDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, inventoryDetailViewDTO.Configuration);

                    grdDetail.DataSource = inventoryDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();

                    ucStatus.ShowRecordInfo(inventoryDetailViewDTO.Entities.Count, grdDetail.PageSize, grdDetail.PageCount, currentPage, grdDetail.AllowPaging);
                }
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
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.codeLabel = this.lblNroInventoryFilter.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.InventoryDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.InventoryDaysAfter;

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
        private void InitializeGrids()
        {
            grdMgr.EmptyDataText = this.Master.EmptyGridText;

            grdDetail.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdDetail.EmptyDataText = this.Master.NoDetailsText;
        }

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            showDetail = false;
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Carga lista de Inventarios
            // Modificacion realizada 14-10-2016
            inventoryViewDTO = iInventoryMGR.FindAllInventory(context);
            //inventoryViewDTO = iInventoryMGR.FindAllInventoryAndCalcPercentProgr(context);

            if (!inventoryViewDTO.hasError() && inventoryViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InventoryConsult.InventoryList, inventoryViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(inventoryViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            base.LoadLocationType(this.ddlLocationType, false, this.Master.AllRowsText);
        }

        private void PopulateGrid()
        {
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN y VISIBILIDAD  de las columnas de la grilla
            if (!inventoryViewDTO.hasConfigurationError() && inventoryViewDTO.Configuration != null && inventoryViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, inventoryViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = inventoryViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                currentPage = 0;
                currentIndex = -1;
            }
        }

        protected void SearchInventoryDetail()
        {
            if (currentIndex != -1)
            {
                // Muestra la grilla detalle (se oculta al cambiar de fila en la grilla de inventarios)
                divGrid.Visible = true;
                currentPage = 0;

                // Recupera el objeto filtro principal de memoria
                mainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];

                // Limpia el objeto 'Main Filter'
                ClearFilterObject();

                // Carga el objeto 'Main Filter' con los valores seleccionados
                LoadControlValuesToFilterObject();

                // Salva los criterios seleccionados
                context.MainFilter = mainFilter;

                // Trae el detalle del Inventario seleccionado
                int id = inventoryViewDTO.Entities[currentIndex].Id;
                
                inventoryDetailViewDTO = iInventoryMGR.GetInventoryDetailById(inventoryViewDTO.Entities[currentIndex], context);

                if (ddlCounted.SelectedValue == "1")
                {
                    inventoryDetailViewDTO.Entities = inventoryDetailViewDTO.Entities.Where(w => w.Id != -1).ToList();
                }
                               

                if (!inventoryDetailViewDTO.hasError() && inventoryDetailViewDTO.Entities != null)
                {
                    Session.Add(WMSTekSessions.InventoryConsult.InventoryDetailList, inventoryDetailViewDTO);
                }
                else
                {
                    Session.Remove(WMSTekSessions.InventoryConsult.InventoryDetailList);
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "setHeightDivDetail", "setDivsAfter();", true);

                // Actualiza mensajes de Status
                ucStatus.ShowMessage(inventoryDetailViewDTO.MessageStatus.Message);
            }
        }

        /// <summary>
        /// Limpia el filtro de detalle de Inventario
        /// </summary>
        public void ClearFilterObject()
        {
            foreach (EntityFilter entityFilter in mainFilter)
            {
                entityFilter.FilterValues.Clear();
            }
        }

        /// <summary>
        /// Carga los criterios de búsqueda seleccionados en el filtro de detalle de Inventario
        /// </summary>
        private void LoadControlValuesToFilterObject()
        {
            int index;

            // Warehouse
            index = Convert.ToInt16(EntityFilterName.Warehouse);

            if (mainFilter[index].FilterValues != null)
            {
                mainFilter[index].FilterValues.Clear();
                mainFilter[index].FilterValues.Add(new FilterItem(0, inventoryViewDTO.Entities[currentIndex].Warehouse.Id.ToString()));
            }

            // IdLocCode 
            if (txtIdLocCode.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Code);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtIdLocCode.Text.Trim()));
                }
            }

            // Location Type
            if (ddlLocationType.SelectedIndex != 0 && ddlLocationType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.LocationType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, ddlLocationType.SelectedValue));
                }
            }

            // Counted
            if (ddlCounted.SelectedIndex != 0 && ddlCounted.SelectedValue == "0")
            {
                index = Convert.ToInt16(EntityFilterName.Counted);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlCounted.SelectedIndex, "-1"));
                }
            }

            // Empty
            if (ddlEmpty.SelectedIndex != 0 && ddlEmpty.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Empty);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlEmpty.SelectedIndex, ddlEmpty.SelectedValue));
                }
            }

            // Item Code 
            if (txtItemCode.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Item);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtItemCode.Text.Trim()));
                }
            }

            // Dif Qty
            if (txtDifQty.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.DifQty);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtDifQty.Text.Trim()));
                }
            }

            // DifAmount
            if (txtDifAmount.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.DifAmount);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtDifAmount.Text.Trim()));
                }
            }
        }

        #endregion

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('Inventory_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr', 'InventoryConsult');", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridWithNoDragAndDropCustom();", true);
        }
    }
}

