using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Administration.Configuration
{
    public partial class ReasonsLogicalWarehouseConfig : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<LogicalWarehouse> logicalWarehousesViewDTO = new GenericViewDTO<LogicalWarehouse>();
        private GenericViewDTO<ReasonLogicalWarehouse> reasonsLogicalWarehouseViewDTO = new GenericViewDTO<ReasonLogicalWarehouse>();
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

        public int currentIndexToLoadDetail
        {
            get
            {
                if (ValidateViewState("currentIndexToLoadDetail"))
                    return (int)ViewState["currentIndexToLoadDetail"];
                else
                    return -1;
            }

            set { ViewState["currentIndexToLoadDetail"] = value; }
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
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                        PopulateGridDetail();
                    }
                }

                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                    currentIndexToLoadDetail = index;
                    currentIndex = grdMgr.SelectedIndex;

                    LoadDetail(index);
                }
            }
            catch (Exception ex)
            {
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
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
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
            }
        }

        protected void grdDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "DeleteReason")
                    DeleteRow(index);
            }
            catch (Exception ex)
            {
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
            }
        }

        protected void grdDetail_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                int editIndex = grdDetail.PageSize * grdDetail.PageIndex + e.NewEditIndex;
            }
            catch (Exception ex)
            {
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
            }
        }

        protected void btnCreateReason_Click(object sender, EventArgs e)
        {
            try
            {
                var logicalWarehouseSelected = (LogicalWarehouse)Session[WMSTekSessions.LogicalWarehouses.Selected];
                chkListReasons.Items.Clear();

                var newContext = NewContext();
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Reason)].FilterValues.Add(new FilterItem(((int)TypeInOut.QualityControl).ToString()));

                var reasons = iWarehousingMGR.FindAllReason(newContext);

                if (reasons.Entities.Count > 0)
                {
                    var reasonsByLogicalWarehouse = iWarehousingMGR.GetReasonsByWhs(logicalWarehouseSelected.warehouseCode, context);

                    foreach (var reason in reasons.Entities)
                    {
                        var i = new ListItem(reason.Name, reason.Code);
                        i.Selected = reasonsByLogicalWarehouse.Entities.Where(r => r.reasonCode == reason.Code).Count() > 0;
                        chkListReasons.Items.Add(i);
                    }

                    divEditNew.Visible = true;
                    modalReason.Show();
                    upEditNew.Update();
                    isValidViewDTO = true;
                }
                else
                {
                    ShowAlert("Error", "No se encontraron razones");
                }
            }
            catch (Exception ex)
            {
                logicalWarehousesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"
        protected void btnSaveReason_Click(object sender, EventArgs e)
        {
            try
            {
                var logicalWarehouseSelected = (LogicalWarehouse)Session[WMSTekSessions.LogicalWarehouses.Selected];

                if (logicalWarehouseSelected != null)
                {
                    var warehouseCodeSelected = logicalWarehouseSelected.warehouseCode;

                    if (chkListReasons.Items.Count == 0)
                    {
                        ShowAlert("Error", "Debe seleccionar al menos una razón");
                        return;
                    }

                    var reasonsSelected = new List<ReasonLogicalWarehouse>();

                    foreach (ListItem reasonSelected in chkListReasons.Items)
                    {
                        if (reasonSelected.Selected)
                        {
                            var reasonLogicalWarehouse = new ReasonLogicalWarehouse()
                            {
                                warehouseCode = warehouseCodeSelected,
                                reasonCode = reasonSelected.Value
                            };
                            reasonsSelected.Add(reasonLogicalWarehouse);
                        }
                    }

                    var getReasonsByWareouse = iWarehousingMGR.GetReasonsByWhs(warehouseCodeSelected, context);

                    if (getReasonsByWareouse.hasError())
                    {
                        ucStatus.ShowError(getReasonsByWareouse.Errors.Message);
                        return;
                    }

                    if (getReasonsByWareouse.Entities.Count == 0)
                    {
                        var createReasons = iWarehousingMGR.MaintainMassiveReasonLogicalWarehouse(CRUD.Create, warehouseCodeSelected, reasonsSelected, context);

                        if (createReasons.hasError())
                        {
                            this.Master.ucError.ShowError(createReasons.Errors);
                            return;
                        }

                        UpdateDetailGrid(lblMessajeCreatedOK.Text);
                    }
                    else
                    {
                        var updateReasons = iWarehousingMGR.MaintainMassiveReasonLogicalWarehouse(CRUD.Update, warehouseCodeSelected, reasonsSelected, context);

                        if (updateReasons.hasError())
                        {
                            this.Master.ucError.ShowError(updateReasons.Errors);
                            return;
                        }

                        UpdateDetailGrid(lblMessajeUpdatedOK.Text);
                    }
                }

                isValidViewDTO = true;
            }
            catch (System.Threading.ThreadAbortException th)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                if (currentIndexToLoadDetail != -1)
                {
                    ReloadData();
                    LoadDetail(currentIndexToLoadDetail);
                }
                else
                {
                    ReloadData();
                }

                ShowAlert(this.lblTitle.Text, ex.Message);
            }
        }

        private void UpdateDetailGrid(string message)
        {
            upGrid.Update();
            ReloadData();

            upGridDetail.Update();
            LoadDetail(currentIndex);
            ucStatus.ShowMessage(message);
        }
        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            divEditNew.Visible = false;
            modalReason.Hide();
            upEditNew.Update();
            isValidViewDTO = true;
        }
        private void DeleteRow(int index)
        {
            var reasonsByWhs = (GenericViewDTO<ReasonLogicalWarehouse>)Session[WMSTekSessions.LogicalWarehouses.Detail];
            var reasonLogicalWarehouseId = reasonsByWhs.Entities.Where(r => r.id == index).FirstOrDefault().id;

            var deleteReason = iWarehousingMGR.MaintainReasonLogicalWarehouse(CRUD.Delete, new ReasonLogicalWarehouse { id = reasonLogicalWarehouseId }, context);

            if (deleteReason.hasError())
            {
                this.Master.ucError.ShowError(deleteReason.Errors);
            }
            else
            {
                UpdateDetailGrid(lblMessajeDeleteOK.Text);
            }
        }

        
        protected void Initialize()
        {
            context.SessionInfo.IdPage = "ReasonsLogicalWarehouseConfig";

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
                    logicalWarehousesViewDTO = (GenericViewDTO<LogicalWarehouse>)Session[WMSTekSessions.LogicalWarehouses.List];
                    isValidViewDTO = true;
                }
            }
        }

        protected void LoadDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                logicalWarehousesViewDTO = (GenericViewDTO<LogicalWarehouse>)Session[WMSTekSessions.LogicalWarehouses.List];

                var warehouseCode = logicalWarehousesViewDTO.Entities[index].warehouseCode;

                reasonsLogicalWarehouseViewDTO = iWarehousingMGR.GetReasonsByWhs(warehouseCode, context);
                this.lblSelectedLogicalWhs.Text = logicalWarehousesViewDTO.Entities[index].warehouseCode;

                if (reasonsLogicalWarehouseViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!reasonsLogicalWarehouseViewDTO.hasConfigurationError() && reasonsLogicalWarehouseViewDTO.Configuration != null && reasonsLogicalWarehouseViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, reasonsLogicalWarehouseViewDTO.Configuration);

                    // Detalles
                    grdDetail.DataSource = reasonsLogicalWarehouseViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }

                Session.Add(WMSTekSessions.LogicalWarehouses.Selected, logicalWarehousesViewDTO.Entities[index]);
                Session.Add(WMSTekSessions.LogicalWarehouses.Detail, reasonsLogicalWarehouseViewDTO);

                divDetail.Visible = true;

            }
            else
            {
                divDetail.Visible = false;
            }
        }

        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                reasonsLogicalWarehouseViewDTO = (GenericViewDTO<ReasonLogicalWarehouse>)Session[WMSTekSessions.LogicalWarehouses.Detail];

                // Configura ORDEN de las columnas de la grilla
                if (!reasonsLogicalWarehouseViewDTO.hasConfigurationError() && reasonsLogicalWarehouseViewDTO.Configuration != null && reasonsLogicalWarehouseViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, reasonsLogicalWarehouseViewDTO.Configuration);

                // Detalles
                grdDetail.DataSource = reasonsLogicalWarehouseViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
                upGridDetail.Update();
            }
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.logicalWarehouseVisible = true;
            this.Master.ucMainFilter.FilterLogicalWarehouseAutoPostBack = true;

            this.Master.ucMainFilter.Initialize(init, refresh);

            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
            this.Master.ucMainFilter.ddlLogicalWareHouseIndexChanged += new EventHandler(ddlLogicalWareHouseIndexChanged);
        }

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
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

        protected void ddlLogicalWareHouseIndexChanged(object sender, EventArgs e)
        {

        }

        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Carga lista de CD logicos
            logicalWarehousesViewDTO = iWarehousingMGR.GetAllLogicalWarehousesHaveReasonsAssociated(context);

            if (!logicalWarehousesViewDTO.hasError() && logicalWarehousesViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.LogicalWarehouses.List, logicalWarehousesViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(logicalWarehousesViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(logicalWarehousesViewDTO.Errors);
            }
        }
        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!logicalWarehousesViewDTO.hasConfigurationError() && logicalWarehousesViewDTO.Configuration != null && logicalWarehousesViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, logicalWarehousesViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = logicalWarehousesViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(logicalWarehousesViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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

        public void ShowAlert(string title, string message)
        {
            Encoding encod = Encoding.ASCII;
            string script = "ShowMessage('" + title + "','" + message.Replace("'", "") + "');";
            script = script.Replace("\\", Convert.ToChar(47).ToString());
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
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
    
        