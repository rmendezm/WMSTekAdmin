using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Outbound.Administration
{
    public partial class ReleaseOrderMgr : BasePage
    {
        #region "Declaración de Variables"
        public event EventHandler pageChanged;

        private GenericViewDTO<OutboundOrder> outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();
        private GenericViewDTO<OutboundOrder> selectedOrdersViewDTO = new GenericViewDTO<OutboundOrder>();
        private GenericViewDTO<OutboundDetail> outboundDetailViewDTO = new GenericViewDTO<OutboundDetail>();
        private GenericViewDTO<OutboundDetail> selectedDetailViewDTO = new GenericViewDTO<OutboundDetail>();
        private GenericViewDTO<TaskSimulation> taskSimulationViewDTO = new GenericViewDTO<TaskSimulation>();
        private GenericViewDTO<TaskDetailSimulation> taskDetailSimulationViewDTO = new GenericViewDTO<TaskDetailSimulation>();
        private GenericViewDTO<WorkZone> workZoneViewDTO = new GenericViewDTO<WorkZone>();
        private BaseViewDTO configurationViewDTO = new BaseViewDTO();

        private List<bool> checkedOrdersCurrentView = new List<bool>();
        private List<bool> selectedOrdersCurrentView = new List<bool>();
        private bool existSelected = false;
        private bool isValidViewDTO = false;

        // Propiedad para controlar el tipo de Liberación de Pedidos (Normal - Wave - Batch)
        private String dispatchType
        {
            get
            {
                if (Request.QueryString["RT"] != null && Request.QueryString["RT"] != String.Empty)
                    return Request.QueryString["RT"];
                else
                    return "PIKOR";
            }
        }

        // Propiedad para controlar el indice activo en la grilla
        private int currentIndex
        {
            get
            {
                if (ValidateViewState("indexOrder"))
                    return (int)ViewState["indexOrder"];
                else
                    return -1;
            }

            set { ViewState["indexOrder"] = value; }
        }

        private int currentIndexSim
        {
            get
            {
                if (ValidateViewState("indexSimulation"))
                    return (int)ViewState["indexSimulation"];
                else
                    return -1;
            }

            set { ViewState["indexSimulation"] = value; }
        }

        private int currentIndexSelected
        {
            get
            {
                if (ValidateViewState("indexSelected"))
                    return (int)ViewState["indexSelected"];
                else
                    return -1;
            }

            set { ViewState["indexSelected"] = value; }
        }

        private int currentWhs
        {
            get
            {
                if (ValidateViewState("currentWhs"))
                    return (int)ViewState["currentWhs"];
                else
                    return -1;
            }

            set { ViewState["currentWhs"] = value; }
        }

        private bool releaseEnabled
        {
            get
            {
                if (ValidateViewState("releaseEnabled"))
                    return (bool)ViewState["releaseEnabled"];
                else
                    return false;
            }

            set { ViewState["releaseEnabled"] = value; }
        }

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

        private int selectedPage
        {
            get { return (int)(ViewState["selectedPage"] ?? 0); }
            set { ViewState["selectedPage"] = value; }
        }

        private bool fullStock
        {
            get
            {
                if (ValidateViewState("fullStock"))
                    return (bool)ViewState["fullStock"];
                else
                    return false;
            }

            set { ViewState["fullStock"] = value; }
        }

        public List<string> constTypeLpnAudit
        {
            get
            {
                //Rescata los tipos de lpn que se pueden auditar
                return GetConst("TypeLpnAuditAndPrecubing");
            }
        }

        public string constTypeLpnClosedBox
        {
            get
            {
                //Rescata los tipos de lpn que se pueden auditar
                return GetConst("PrecubingTypeLpnClosedBox")[0];
            }
        }
        public string constPercentageTolerance
        {
            get
            {
                //Rescata los tipos de lpn que se pueden auditar
                return GetConst("PrecubingPercentageTolerance")[0];
            }
        }

        public List<string> constAllowCrossDock
        {
            get
            {
                //Rescata si por parametro compañia permite CrossDock
                return GetConst("AllowCrossDockInReleaseOrder");
            }
        }
        public List<string> constAllowBackOrder
        {
            get
            {
                //Rescata si por parametro compañia permite BackOrder
                return GetConst("AllowBackOrderInReleaseOrder");
            }
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (base.webMode == WebMode.Normal)
                {
                    base.Page_Load(sender, e);
                    SaveCheckedRows();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                        PopulateSelectedGrid();
                        PopulateSimulateGrid(false, false, false, false);
                        LoadOutboundOrderDetails();
                        LoadSimulationDetail();
                        LoadSelectedOutboundOrderDetails();
                        UpdateGridLayout();
                        ShowConsolidationOrdersPercentage();
                    }
                }

                if (currentIndex > -1)
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "gridSelected", "gridViewOnclick('" + currentIndex + "', '" + grdMgr.ClientID + "');", true);
                }

                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Actualiza la vista de Ordenes Pendientes
        /// </summary>
        protected void UpdateGridLayout()
        {
            int index;
            for (int i = 0; i < selectedOrdersCurrentView.Count; i++)
            {
                // Si la Orden se encuentra en la lista de Ordenes Seleccionadas, deshabilita el checkbox y cambia el color de la fila
                if (selectedOrdersCurrentView[i])
                {
                    index = i / grdMgr.PageSize;

                    if (index == grdMgr.PageIndex)
                    {
                        if (grdMgr.PageIndex == 0)
                        {
                            ((CheckBox)grdMgr.Rows[i].FindControl("chkSelectOrder")).Visible = false;
                        }
                        else
                        {
                            index = i / grdMgr.PageSize;
                            index = index * grdMgr.PageSize;
                            index = Math.Abs(i - index);

                            if (i < grdMgr.PageSize)
                                ((CheckBox)grdMgr.Rows[i].FindControl("chkSelectOrder")).Visible = false;
                            else
                                ((CheckBox)grdMgr.Rows[index].FindControl("chkSelectOrder")).Visible = false;
                        }
                    }
                }
            }

            // Actualiza la vista de Ordenes Pendientes
            //     upPendingOrders.Update();
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

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    currentIndex = grdMgr.SelectedIndex + (grdMgr.PageIndex * grdMgr.PageSize);
                    LoadOutboundOrderDetails();
                    mpPendingOrders.Show();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    currentIndexSelected = grdSelected.SelectedIndex;
                    LoadSelectedOutboundOrderDetails();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }


        protected void grdWorkZones_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                ImageButton btnUp = e.Row.FindControl("btnUp") as ImageButton;
                ImageButton btnDown = e.Row.FindControl("btnDown") as ImageButton;
                ImageButton btnRemove = e.Row.FindControl("btnRemove") as ImageButton;

                if (btnUp != null) btnUp.CommandArgument = e.Row.DataItemIndex.ToString();
                if (btnDown != null) btnDown.CommandArgument = e.Row.DataItemIndex.ToString();
                if (btnRemove != null) btnRemove.CommandArgument = e.Row.DataItemIndex.ToString();

                // Deshabilita la opcion de Subir si es el primer registro
                if (btnUp != null && e.Row.DataItemIndex == 0)
                {
                    btnUp.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_up_dis.png";
                    btnUp.Enabled = false;
                }

                // Deshabilita la opcion de Bajar si es el ultimo registro
                if (btnDown != null && e.Row.DataItemIndex == workZoneViewDTO.Entities.Count - 1)
                {
                    btnDown.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_down_dis.png";
                    btnDown.Enabled = false;
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    //e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdWorkZones.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdWorkZones.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdWorkZones.ClientID + "')");

                    DropDownList ddlForkLift = e.Row.FindControl("ddlForkLift") as DropDownList;
                    DropDownList ddlUser = e.Row.FindControl("ddlUser") as DropDownList;
                    DropDownList ddlTargetLocation = e.Row.FindControl("ddlTargetLocation") as DropDownList;

                    int idWorkZone = Convert.ToInt32(grdWorkZones.DataKeys[e.Row.RowIndex].Value);

                    if (ddlForkLift != null) base.LoadForkLiftLocationsInWorkZone(ddlForkLift, idWorkZone, currentWhs, this.Master.EmptyRowText);
                    if (ddlUser != null) base.LoadUsersInWorkZone(ddlUser, idWorkZone, this.Master.EmptyRowText);
                    if (ddlTargetLocation != null) base.LoadStagingLocations(ddlTargetLocation, currentWhs, this.Master.EmptyRowText);
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdSelected_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                ImageButton btnUp = e.Row.FindControl("btnUp") as ImageButton;
                ImageButton btnDown = e.Row.FindControl("btnDown") as ImageButton;

                ImageButton btnChangeOrderRules = e.Row.FindControl("btnChangeOrderRules") as ImageButton;
                if (btnChangeOrderRules != null)
                {
                    if (dispatchType == "PIKOR" || dispatchType == "PIKBT")
                        btnChangeOrderRules.Visible = true;
                    else
                        btnChangeOrderRules.Visible = false;
                }

                if (btnUp != null) btnUp.CommandArgument = e.Row.DataItemIndex.ToString();
                if (btnDown != null) btnDown.CommandArgument = e.Row.DataItemIndex.ToString();

                // Deshabilita la opcion de Subir si es el primer registro
                if (btnUp != null && e.Row.DataItemIndex == 0)
                {
                    btnUp.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_up_dis.png";
                    btnUp.Enabled = false;
                }

                // Deshabilita la opcion de Bajar si es el ultimo registro
                if (btnDown != null && e.Row.DataItemIndex == selectedOrdersViewDTO.Entities.Count - 1)
                {
                    btnDown.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_down_dis.png";
                    btnDown.Enabled = false;
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    //e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdSelected.ClientID + "')");
                    //e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this.grdSelected, "Select$" + e.Row.RowIndex.ToString()));

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdSelected.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdSelected.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdSelected.ClientID + "')");


                    // Asinga el atributo 'onclick' a todas las columnas de la grilla, excepto a la que contiene los checkboxes
                    // IMPORTANTE: no cambiar de lugar la columna [0] que contiene los checkboxes
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        if (i!= 10)
                            e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdSelected, "Select$" + e.Row.RowIndex);
                    }
                                        
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdSelectedDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                ImageButton btnChangeOrderDetailRules = e.Row.FindControl("btnChangeOrderDetailRules") as ImageButton;
                if (btnChangeOrderDetailRules != null)
                {
                    if (dispatchType == "PIKOR" || dispatchType == "PIKBT")
                        btnChangeOrderDetailRules.Visible = true;
                    else
                        btnChangeOrderDetailRules.Visible = false;
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdSelectedDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdSelectedDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdSelectedDetail.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdSimulate_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdSimulate.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdSimulate.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdSimulate.ClientID + "')");

                    e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdSimulate, "Select$" + e.Row.RowIndex);
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdSelected_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "Up") ChangeOrderPriority(index, "up");
                if (e.CommandName == "Down") ChangeOrderPriority(index, "down");
                if (e.CommandName == "ChangeOrderRules") SelectRulesByOrder(index);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        private void SelectRulesByOrder(int idOutboundOrder)
        {
            try
            {
                var selectedOrder = selectedOrdersViewDTO.Entities.Where(oo => oo.Id == idOutboundOrder).FirstOrDefault();

                if (selectedOrder != null)
                {
                    idOutboundOrderToChangeRules.Value = idOutboundOrder.ToString();
                    idItemToChangeRules.Value = string.Empty;
                    OpenSelectRulesByOrderPopUp(selectedOrder.Warehouse.Id);
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        private void OpenSelectRulesByOrderPopUp(int idWhs)
        {
            var customRulesToSelect = iDispatchingMGR.RulesGetByWhsAndProcess(idWhs, "PICK", context);

            if (!customRulesToSelect.hasError() && customRulesToSelect.Entities.Count > 0)
            {
                rblRules.DataSource = customRulesToSelect.Entities;
                rblRules.DataTextField = "Name";
                rblRules.DataValueField = "Id";
                
                rblRules.DataBind();
                rblRules.Items.Insert(0, new ListItem(Master.EmptyRowText, "-1"));
                rblRules.Items[0].Selected = true;           

                divSelectRulesByOrder.Visible = true;
                upSelectRulesByOrder.Update();
                modalPopUpCloseSelectRulesByOrder.Show();
            }
            else
            {
                ucStatus.ShowWarning("No se encontraron grupo de reglas");
            }
        }
        protected void btnSaveSelectRulesByOrder_Click(object sender, EventArgs e)
        {
            try
            {
                var ruleSelected = int.Parse(rblRules.SelectedValue);

                var ruleByOrder = new RulesByOrder()
                {
                    IdItem = -1,
                    IdOutboundOrder = int.Parse(idOutboundOrderToChangeRules.Value),
                    IdCustomRule = ruleSelected
                };

                if (!string.IsNullOrEmpty(idItemToChangeRules.Value))
                    ruleByOrder.IdItem = int.Parse(idItemToChangeRules.Value);

                var createRuleSelected = iDispatchingMGR.CreateRulesByOrder(ruleByOrder, context);

                if (createRuleSelected.hasError())
                    ucStatus.ShowError("No se pudo crear reglas custom para orden");
                else
                {
                    if (ruleByOrder.IdItem < 1)
                        updateGridSelected();
                    else
                        updateGridSelectedDetail();

                    ucStatus.ShowMessage(createRuleSelected.MessageStatus.Message);
                }    
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void grdSelectedDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "ChangeOrderDetailRules") SelectRulesByOrderAndItem(index);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        private void SelectRulesByOrderAndItem(int idOutboundDetail)
        {
            selectedDetailViewDTO = (GenericViewDTO<OutboundDetail>)Session[WMSTekSessions.ReleaseOrderMgr.SelectedDetail];
            var selectedOrderDetail = selectedDetailViewDTO.Entities.Where(oo => oo.Id == idOutboundDetail).FirstOrDefault();

            if (selectedOrderDetail != null)
            {
                idOutboundOrderToChangeRules.Value = selectedOrderDetail.OutboundOrder.Id.ToString();
                idItemToChangeRules.Value = selectedOrderDetail.Item.Id.ToString();

                var selectedOrder = selectedOrdersViewDTO.Entities.Where(oo => oo.Id == selectedOrderDetail.OutboundOrder.Id).FirstOrDefault();

                if (selectedOrder != null)
                    OpenSelectRulesByOrderPopUp(selectedOrder.Warehouse.Id);
            }
        }
        protected void btnDeleteSelectRulesByOrder_Click(object sender, EventArgs e)
        {
            try
            {
                var ruleByOrder = new RulesByOrder()
                {
                    IdItem = -1,
                    IdOutboundOrder = int.Parse(idOutboundOrderToChangeRules.Value)
                };

                if (!string.IsNullOrEmpty(idItemToChangeRules.Value))
                    ruleByOrder.IdItem = int.Parse(idItemToChangeRules.Value);

                var deleteRules = iDispatchingMGR.RulesMaintainRulesByOrder(CRUD.Delete, ruleByOrder, context);

                if (deleteRules.hasError())
                    ucStatus.ShowError(deleteRules.Errors.Message);
                else
                {
                    if (ruleByOrder.IdItem < 1)
                        updateGridSelected();
                    else
                        updateGridSelectedDetail();

                    ucStatus.ShowMessage(deleteRules.MessageStatus.Message);
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        private void updateGridSelected()
        {
            UpdateSession(false, false);
            selectedOrdersViewDTO.Entities.Clear();
            AddToSelectedOrdersAfterChangeRules();
            PopulateSelectedGrid();
        }
        private void updateGridSelectedDetail()
        {
            selectedOrdersViewDTO.Entities[currentIndexSelected].OutboundDetails = null;
            LoadSelectedOutboundOrderDetails();
        }

        protected void grdWorkZones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "Up") ChangeWorkZonePriority(index, "up");
                if (e.CommandName == "Down") ChangeWorkZonePriority(index, "down");
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdSimulate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    currentIndexSim = grdSimulate.SelectedIndex;
                    LoadSimulationDetail();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdDetailSim_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdDetailSim.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdDetailSim.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdDetailSim.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega las Ordenes seleccionadas a la lista de Ordenes Seleccionadas para la Simulación de Liberación de Pedidos
        /// </summary>
        protected void btnAddToSelected_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Oculta detalle de las grillas
                HideDetails();

                // Valida que haya al menos una Orden seleccionada
                if (existSelected)
                {
                    // Valida que las Ordenes seleccionadas pertenezcan al mismo Warehouse
                    if (SameWarehouse())
                    {
                        // Para las Liberaciones Wave, Batch y Pick&Pass, las órdenes deben pertenecer al mismo Owner
                        if (dispatchType == "PIKOR" || dispatchType == "PIKIT" || dispatchType == "PIUNK" || dispatchType == "PIKVA")
                        {
                            AddToSelectedOrders();
                        }
                        else
                        {
                            //Valida que las ordenes pertenezcan al mismo Owner
                            if (SameOwner())
                                AddToSelectedOrders();
                            else
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNoOwnerSelected.Text, string.Empty);
                        }
                    }
                    else
                    {
                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNoWhsSelected.Text, string.Empty);
                    }
                }
                else
                {
                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNoOrdersSelected.Text, string.Empty);
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void btnFirstgrdMgr_Click(object sender, ImageClickEventArgs e)
        {
            ddlPages.SelectedIndex = 0;
            ddlPagesSelectedIndexChanged(sender, e);
            mpPendingOrders.Show();
        }

        protected void btnPrevgrdMgr_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage = currentPage - 1;
                grdMgr.PageIndex = currentPage;
                                
                // Configura ORDEN de las columnas de la grilla
                if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdMgr, outboundOrderViewDTO.Configuration);

                // Encabezado de Recepciones
                grdMgr.DataSource = outboundOrderViewDTO.Entities;
                grdMgr.DataBind();

                CallJsGridViewHeader();


                // Inicializa array de Ordenes seleccionadas en la vista actual
                InitializeCheckedRows();
                InitializeSelectedRows();
                SaveSelectedRows();

                // Es necesario cargar las grillas SIEMPRE para evitar problemas con el orden dinamico de las columnas
                PopulateGrid();
                PopulateSelectedGrid();
                PopulateSimulateGrid(false, false, false, !Page.IsPostBack);
                                
                if (currentPage > 0)
                {
                    btnNextgrdMgr.Enabled = true;
                    btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastgrdMgr.Enabled = true;
                    btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnPrevgrdMgr.Enabled = false;
                    btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstgrdMgr.Enabled = false;
                    btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                }
            }
            else
            {
                btnPrevgrdMgr.Enabled = false;
                btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                btnFirstgrdMgr.Enabled = false;
                btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
            }

            mpPendingOrders.Show();
        }

        protected void btnLastgrdMgr_Click(object sender, ImageClickEventArgs e)
        {
            ddlPages.SelectedIndex = grdMgr.PageCount-1;
            ddlPagesSelectedIndexChanged(sender, e);
            mpPendingOrders.Show();
        }

        protected void btnNextgrdMgr_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPage < grdMgr.PageCount)
            {
                
                currentPage = currentPage + 1;
                grdMgr.PageIndex = currentPage;

                // Configura ORDEN de las columnas de la grilla
                if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdMgr, outboundOrderViewDTO.Configuration);

                // Encabezado de Recepciones
                grdMgr.DataSource = outboundOrderViewDTO.Entities;
                grdMgr.DataBind();

                CallJsGridViewHeader();

                // Inicializa array de Ordenes seleccionadas en la vista actual
                InitializeCheckedRows();
                InitializeSelectedRows();
                SaveSelectedRows();

                // Es necesario cargar las grillas SIEMPRE para evitar problemas con el orden dinamico de las columnas
                PopulateGrid();
                PopulateSelectedGrid();
                PopulateSimulateGrid(false, false, false, !Page.IsPostBack);

                if (currentPage < grdMgr.PageCount)
                {
                    btnPrevgrdMgr.Enabled = true;
                    btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstgrdMgr.Enabled = true;
                    btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
                else
                {
                    btnNextgrdMgr.Enabled = false;
                    btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                    btnLastgrdMgr.Enabled = false;
                    btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                }
                                    
            }
            else
            {
                btnNextgrdMgr.Enabled = false;
                btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastgrdMgr.Enabled = false;
                btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
            }
            mpPendingOrders.Show();
        }

        protected void ddlPagesSelectedIndexChanged(object sender, EventArgs e)
        {                        
            currentPage = ddlPages.SelectedIndex;
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, outboundOrderViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = outboundOrderViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            // Inicializa array de Ordenes seleccionadas en la vista actual
            InitializeCheckedRows();
            InitializeSelectedRows();
            SaveSelectedRows();

            // Es necesario cargar las grillas SIEMPRE para evitar problemas con el orden dinamico de las columnas
            PopulateGrid();
            PopulateSelectedGrid();
            PopulateSimulateGrid(false, false, false, !Page.IsPostBack);

            if (currentPage == grdMgr.PageCount)
            {
                btnNextgrdMgr.Enabled = false;
                btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastgrdMgr.Enabled = false;
                btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
            }
            else
            {
                if (currentPage == 0)
                {
                    btnPrevgrdMgr.Enabled = false;
                    btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstgrdMgr.Enabled = false;
                    btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                }
                else
                {
                    btnNextgrdMgr.Enabled = true;
                    btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastgrdMgr.Enabled = true;
                    btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevgrdMgr.Enabled = true;
                    btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstgrdMgr.Enabled = true;
                    btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
            }
            mpPendingOrders.Show();
        }

        /// <summary>
        /// Vuelve a simular la Liberación de Pedidos, luego de los cambios de prioridad realizados por el Planner
        /// </summary>
        protected void btnReprocess_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    GenericViewDTO<OutboundOrder> newList = new GenericViewDTO<OutboundOrder>();
                    foreach (var item in selectedOrdersViewDTO.Entities)
                    {
                        newList.Entities.Add(item);
                    }
                    Session["OrdenesSeleccionadasSimulacion"] = newList;

                    if (SaveFromSelectedOrders())
                    {
                        CreateFilterTypeLocationUsedForStockAvailable();

                        releaseEnabled = true;

                        // Ejecuta nuevamente la Simulación de Liberación de Pedidos
                        PopulateSimulateGrid(true, false, false, false);

                        // Oculta detalle de las grillas
                        HideDetails();
                    }
                    else
                    {
                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNoOrdersSelected.Text, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Quita Ordenes de la lista de Ordenes Seleccionadas
        /// </summary>
        protected void btnRemove_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                bool chkRemove = false;

                for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                {
                    GridViewRow row = grdSelected.Rows[i];

                    if (((CheckBox)row.FindControl("chkRemoveOrder")).Checked)
                    {
                        chkRemove = true;
                        break;
                    }
                }

                if (chkRemove)
                {
                    // Oculta detalle de las grillas
                    HideDetails();

                    RemoveFromSelectedOrders();

                    //Vacio las tareas
                    taskSimulationViewDTO.Entities = null;

                    var configSimulateHeaderGridView = iConfigurationMGR.GetConfigByQueryName("grdSimulate_ReleaseOrder", context);

                    if (!configSimulateHeaderGridView.hasError() && configSimulateHeaderGridView.Entities.Count > 0)
                        base.ConfigureGridOrder(grdSimulate, configSimulateHeaderGridView.Entities);

                    //Refresco la grilla de la simulacion
                    grdSimulate.DataSource = taskSimulationViewDTO.Entities;
                    grdSimulate.DataBind();

                    CallJsGridViewSimulateHeader();
                }
                else
                {
                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNoOrdersSelectedRemove.Text, string.Empty);
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Muestra ventana para configurar la Liberación de Pedidos
        /// </summary>
        protected void btnReleasePopUp_Click(object sender, EventArgs e)
        {
            try
            {
                string errorCustomRule = string.Empty;
                fullStock = true;

                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    errorCustomRule = ValidateCustomRuleItem();
                    // Si no hay stock suficiente para las Ordenes seleccionadas con el campo 'FullShipment' activo, no se permite continuar
                    if (!ValidateFullShipment())
                    {
                        this.Master.ucDialog.ShowAlert(lblTitle.Text, lblNoStockFullShipment.Text, string.Empty);
                    }
                    else
                        if (!string.IsNullOrEmpty(errorCustomRule))
                        {
                            this.Master.ucDialog.ShowAlert(lblTitle.Text, errorCustomRule.Trim(), string.Empty);
                        }
                        else
                        {

                            // Carga ubicaciones de Packing
                            this.ddlLocStageTarget.SelectedIndex = -1;
                            base.LoadStagingLocations(this.ddlLocStageTarget, currentWhs, this.Master.EmptyRowText);

                            //Nuevas Ubicaciones de Anden y Embalaje
                            this.ddlLocStageDispatch.SelectedIndex = -1;
                            this.ddlLocDock.SelectedIndex = -1;
                            base.LoadLocationsByWhsAndType(this.ddlLocStageDispatch, currentWhs, LocationTypeName.STGD.ToString(), this.Master.EmptyRowText, true);
                            base.LoadLocationsByWhsAndType(this.ddlLocDock, currentWhs, LocationTypeName.DOCK.ToString(), this.Master.EmptyRowText, true);

                            // Valida condiciones para poder Liberar los pedidos
                            if (OrderInSumulation() || !ValidateStock())
                            {
                                // Si no hay stock suficiente para todas las Ordenes seleccionadas, se muestra un mensaje de advertencia
                                if (!ValidateStock() && !OrderInSumulation())
                                {
                                    fullStock = false;
                                    this.Master.ucDialog.ShowConfirm(lblTitle.Text, lblNoStock.Text, "release");
                                }

                                // Si alguna de las Ordenes seleccionadas ya se encuentra en otro proceso de Simulación, se muestra un mensaje de advertencia
                                if (ValidateStock() && OrderInSumulation())
                                    this.Master.ucDialog.ShowConfirm(lblTitle.Text, lblOrderInSimulation.Text, "release");

                                // Si no hay stock suficiente Y alguna de las Ordenes se encuentra en otro proceso, muestra los dos mensajes de advertencia
                                if (!ValidateStock() && OrderInSumulation())
                                {
                                    fullStock = false;
                                    this.Master.ucDialog.ShowConfirm(lblTitle.Text, lblNoStock.Text, "alertOrderInSimulation");
                                }
                            }
                            else
                            {
                                ShowReleaseOrdersPopUp();
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string detailTitle;

                grdSimulate.AllowPaging = false;
                // PopulateGrid();

                // Capturo la posicion de la fila activa
                int index = grdSimulate.PageSize * grdSimulate.PageIndex + grdSimulate.SelectedIndex;

                if (index != -1)
                {
                    currentIndexSim = grdSimulate.SelectedIndex;
                    LoadSimulationDetail();
                    
                    detailTitle = lblGridDetail.Text + lblNroDocSim.Text;
                }
                else
                {
                    detailTitle = null;
                }

                base.ExportToExcel(grdSimulate,this.grdDetailSim, detailTitle);
                grdSimulate.AllowPaging = true;

            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }


        /// <summary>
        /// Respuesta desde la ventana de diálogo
        /// </summary>
        protected void btnDialogOk_Click(object sender, EventArgs e)
        {
            try
            {
                switch (this.Master.ucDialog.Caller)
                {
                    case "release":
                        ShowReleaseOrdersPopUp();
                        break;
                    case "getTaskSimulation":
                        PopulateSimulateGrid(false, true, false, true);
                        AddToSelectedOrdersFromTaskList();
                        break;
                    case "alertOrderInSimulation":
                        // Si alguna de las Ordenes seleccionadas ya se encuentra en otro proceso de Simulación, se muestra un mensaje de advertencia
                        if (OrderInSumulation())
                            this.Master.ucDialog.ShowConfirm(lblTitle.Text, lblOrderInSimulation.Text, "release");
                        else
                            ShowReleaseOrdersPopUp();
                        break;
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Realiza la Liberación de Pedidos
        /// </summary>
        protected void btnRelease_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    CreateFilterTypeLocationUsedForStockAvailable();

                    if (dispatchType != "PIKPS")
                    {
                        if (this.ddlLocStageDispatch.SelectedValue == "-1" &&
                           this.ddlLocDock.SelectedValue == "-1")
                        {
                            rfvLocStageTarget.IsValid = false;
                            rfvLocStageTarget.ErrorMessage = this.lblMsgErrorUbic.Text;
                            //rfvSummary.ShowMessageBox = true;
                            //this.lblErrorMsg.Visible = true;
                            //this.lblErrorMsg.Text = this.lblMsgErrorUbic.Text;
                            this.divReleaseDispatch.Visible = true;
                            this.mpReleaseDispatch.Show();
                            
                        }
                        else
                        {
                            //this.lblErrorMsg.Visible = false;
                            // Oculta detalle de las grillas
                            HideDetails();
                            ReleaseOrders();
                        }
                    }
                    else
                    {
                        // Oculta detalle de las grillas
                        HideDetails();
                        ReleaseOrders();
                    }
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "OcultaBarraProgreso", "hideProgress();", true);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        // Carga la grilla, filtrada por el criterio de busqueda ingresado
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();

                divPendingOrders.Visible = true;
                upPendingOrders.Update();
                mpPendingOrders.Show();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                InitializeSelectedOrders();
                ReloadData();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void chkBackOrder_OnCheckedChanged(Object sender, EventArgs e)
        {
            if (chkBackOrder.Checked)
            {
                chkCrossDock.Checked = false;
                divExpDateBackOrder.Visible = true;
            }
            else
                divExpDateBackOrder.Visible = false;
            
            divReleaseDispatch.Visible = true;
            mpReleaseDispatch.Show();
        }

        protected void chkCrossDock_OnCheckedChanged(Object sender, EventArgs e)
        {
            if (chkCrossDock.Checked)
            {
                chkBackOrder.Checked = false;
                divExpDateBackOrder.Visible = false;
            }

            divReleaseDispatch.Visible = true;
            mpReleaseDispatch.Show();
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdSelected_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdSelectedDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "ReleaseOrderMgr";

            InitializeSplitters();
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeGrid();
            
            this.Master.ucDialog.BtnOkClick += new EventHandler(btnDialogOk_Click);

            if (!Page.IsPostBack)
            {
                // Setea títulos
                switch (dispatchType)
                {
                    case "PIKWV":
                        lblEdit.Text = lblTitleWave.Text;
                        lblTitle.Text = lblTitleWave.Text;
                        break;
                    case "PIKBT":
                        lblEdit.Text = lblTitleBatch.Text;
                        lblTitle.Text = lblTitleBatch.Text;
                        break;
                    case "PIKPS":
                        lblEdit.Text = lblTitlePickAndPass.Text;
                        lblTitle.Text = lblTitlePickAndPass.Text;
                        break;
                }

                InitializeSession();
                //UpdateSession(false);

                // TODO: revisar
                /*
                // Busca Simulación Pendiente para el Usuario, según el tipo de Liberación que esté realizando
                taskSimulationViewDTO = iDispatchingMGR.GetTaskSimulationByUserAndType(context, dispatchType);

                // Si hay una Simulación Pendiente, pregunta si se quiere utilizar
                if (!taskSimulationViewDTO.hasError() && taskSimulationViewDTO.Entities != null && taskSimulationViewDTO.Entities.Count > 0)
                {
                    this.Master.ucDialog.ShowConfirm(lblTitle.Text, lblPendingSimulation.Text, "getTaskSimulation");
                }
                */
            }
            else
            {
                // Recupera Ordenes pendientes
                switch (dispatchType)
                {
                    case "PIKOR":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKOR.PendingOrders))
                        {
                            isValidViewDTO = true;
                            outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKOR.PendingOrders];
                        }
                        break;

                    case "PIKVA":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKVA.PendingOrders))
                        {
                            isValidViewDTO = true;
                            outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKVA.PendingOrders];
                        }
                        break;

                    case "PIKIT":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKIT.PendingOrders))
                        {
                            isValidViewDTO = true;
                            outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKIT.PendingOrders];
                        }
                        break;

                    case "PIUNK":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIUNK.PendingOrders))
                        {
                            isValidViewDTO = true;
                            outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIUNK.PendingOrders];
                        }
                        break;

                    case "PIKWV":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKWV.PendingOrders))
                        {
                            isValidViewDTO = true;
                            outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKWV.PendingOrders];
                        }
                        break;

                    case "PIKBT":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKBT.PendingOrders))
                        {
                            isValidViewDTO = true;
                            outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKBT.PendingOrders];
                        }
                        break;

                    case "PIKPS":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKPS.PendingOrders))
                        {
                            isValidViewDTO = true;
                            outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.PendingOrders];
                        }

                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKPS.SelectedWorkZones))
                            workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.SelectedWorkZones];

                        break;
                }
            }

            // Inicializa array de Ordenes seleccionadas en la vista actual
            InitializeCheckedRows();
            InitializeSelectedRows();

            // Recupera lista de Ordenes seleccionadas hasta el momento para la Simulación
            switch (dispatchType)
            {
                case "PIKOR":
                    if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKOR.SelectedOrders))
                    {
                        selectedOrdersViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKOR.SelectedOrders];
                    }
                    break;

                case "PIKVA":
                    if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKVA.SelectedOrders))
                    {
                        selectedOrdersViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKVA.SelectedOrders];
                    }
                    break;

                case "PIKIT":
                    if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKIT.SelectedOrders))
                    {
                        selectedOrdersViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKIT.SelectedOrders];
                    }
                    break;

                case "PIUNK":
                    if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIUNK.SelectedOrders))
                    {
                        selectedOrdersViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIUNK.SelectedOrders];
                    }
                    break;

                case "PIKWV":
                    if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKWV.SelectedOrders))
                    {
                        selectedOrdersViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKWV.SelectedOrders];
                    }
                    break;

                case "PIKBT":
                    if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKBT.SelectedOrders))
                    {
                        selectedOrdersViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKBT.SelectedOrders];
                    }
                    break;

                case "PIKPS":
                    if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKPS.SelectedOrders))
                    {
                        selectedOrdersViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.SelectedOrders];
                    }
                    break;
            }

            SaveSelectedRows();

            // Es necesario cargar las grillas SIEMPRE para evitar problemas con el orden dinamico de las columnas
            PopulateGrid();
            PopulateSelectedGrid();
            PopulateSimulateGrid(false, false, false, !Page.IsPostBack);

        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //Muestra filtro avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;

            //// Configura Filtro Básico
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            //this.Master.ucMainFilter.inboundTypeVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;

            this.Master.ucMainFilter.outboundTypeNotIncludeAll = false;
            this.Master.ucMainFilter.outboundTypeVisible = true;
            this.Master.ucMainFilter.listOutboundType = new List<String>();
            this.Master.ucMainFilter.listOutboundType = getOutboundType();
            this.Master.ucMainFilter.OutboundTypeCode = new string[] { };
            this.Master.ucMainFilter.OutboundTypeCode = this.Master.ucMainFilter.listOutboundType.ToArray();
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = this.lblFilterReferenceNumber.Text;

            // Configura Filtro Avanzado
            this.Master.ucMainFilter.tabDatesVisible = true;
            this.Master.ucMainFilter.expirationDateVisible = true; 
            this.Master.ucMainFilter.expectedDateVisible = true;
            
            this.Master.ucMainFilter.tabDispatchingVisible = true;
            this.Master.ucMainFilter.shipmentDateVisible = true;
            this.Master.ucMainFilter.tabItemGroupVisible = true;
            this.Master.ucMainFilter.tabItemUnitsVisible = true;

            this.Master.ucMainFilter.codeAltVisible = true;
            this.Master.ucMainFilter.codeLabelAlt = lblFilterCustomerType.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuración inicial de los Paneles
        /// </summary>
        private void InitializeSplitters()
        {
            // Splitter vertical
            hsVertical.LeftPanel.WidthDefault = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .6);
            /*
            rceSplitterVertical.MaximumHeight = Convert.ToInt16(Session["screenY"]) - 50;
            rceSplitterVertical.MinimumHeight = rceSplitterVertical.MaximumHeight;

            rceSplitterVertical.MinimumWidth = 100;
            rceSplitterVertical.MaximumWidth = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .7);
            */
            // Splitter horizontal izquierdo
            /*
            rceSplitter.MaximumWidth = Convert.ToInt16(Session["screenX"]) - 32;
            rceSplitter.MinimumWidth = rceSplitter.MaximumWidth;

            rceSplitter.MinimumHeight = 100;
            rceSplitter.MaximumHeight = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .6);
            */
        }
                

        /// <summary>
        /// Configuracion inicial de la Grilla principal
        /// </summary>
        

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;
        }

        /// <summary>
        /// Carga configuración de la grilla de Ordenes Seleccionadas y su detalle
        /// </summary>
        private void InitializeSelectedOrders()
        {
            if (selectedDetailViewDTO.Configuration == null)
            {
                configurationViewDTO = iConfigurationMGR.GetLayoutConfiguration("SelectedOrders", context);
                if (!configurationViewDTO.hasConfigurationError()) selectedOrdersViewDTO.Configuration = configurationViewDTO.Configuration;

                configurationViewDTO = iConfigurationMGR.GetLayoutConfiguration("SelectedDetail", context);
                if (!configurationViewDTO.hasConfigurationError()) selectedDetailViewDTO.Configuration = configurationViewDTO.Configuration;

                UpdateSelectedRowsSession();

                Session[WMSTekSessions.ReleaseOrderMgr.SelectedDetail] = selectedDetailViewDTO;
            }
        }

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        /// <param name="showError">Determina si se mostrara el error producido en una operacion anterior</param>
        private void UpdateSession(bool showError, bool clearSelectedOrders = true)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(taskSimulationViewDTO.Errors);
                taskSimulationViewDTO.ClearError();
            }

            this.Master.ucMainFilter.ClearFilterObject();
            this.Master.ucMainFilter.LoadControlValuesToFilterObject();

            // Carga lista de Pedidos Pendientes
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            //outboundOrderViewDTO = iDispatchingMGR.GetPendingOutboundOrder(context, dispatchType);
            outboundOrderViewDTO = iDispatchingMGR.GetPendingOutboundOrderFilter(context, dispatchType);

            if (!outboundOrderViewDTO.hasError() && outboundOrderViewDTO.Entities != null)
            {
                switch (dispatchType)
                {
                    case "PIKOR":
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKOR.PendingOrders] = outboundOrderViewDTO;
                        break;

                    case "PIKVA":
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKVA.PendingOrders] = outboundOrderViewDTO;
                        break;

                    case "PIKIT":
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKIT.PendingOrders] = outboundOrderViewDTO;
                        break;

                    case "PIUNK":
                        Session[WMSTekSessions.ReleaseOrderMgr.PIUNK.PendingOrders] = outboundOrderViewDTO;
                        break;

                    case "PIKWV":
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKWV.PendingOrders] = outboundOrderViewDTO;
                        break;

                    case "PIKBT":
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKBT.PendingOrders] = outboundOrderViewDTO;
                        break;

                    case "PIKPS":
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.PendingOrders] = outboundOrderViewDTO;
                        break;
                }

                isValidViewDTO = true;

                if (clearSelectedOrders)
                {
                    InitializeSelectedRows();
                    SaveSelectedRows();
                }
               
                if (!crud)
                    ucStatus.ShowMessage(outboundOrderViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Salva en memoria las ordenes seleccionadas
        /// </summary>
        private void UpdateSelectedRowsSession()
        {
            switch (dispatchType)
            {
                case "PIKOR":
                    Session[WMSTekSessions.ReleaseOrderMgr.PIKOR.SelectedOrders] = selectedOrdersViewDTO;
                    break;

                case "PIKVA":
                    Session[WMSTekSessions.ReleaseOrderMgr.PIKVA.SelectedOrders] = selectedOrdersViewDTO;
                    break;

                case "PIKIT":
                    Session[WMSTekSessions.ReleaseOrderMgr.PIKIT.SelectedOrders] = selectedOrdersViewDTO;
                    break;

                case "PIUNK":
                    Session[WMSTekSessions.ReleaseOrderMgr.PIUNK.SelectedOrders] = selectedOrdersViewDTO;
                    break;

                case "PIKWV":
                    Session[WMSTekSessions.ReleaseOrderMgr.PIKWV.SelectedOrders] = selectedOrdersViewDTO;
                    break;

                case "PIKBT":
                    Session[WMSTekSessions.ReleaseOrderMgr.PIKBT.SelectedOrders] = selectedOrdersViewDTO;
                    break;

                case "PIKPS":
                    Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.SelectedOrders] = selectedOrdersViewDTO;
                    break;
            }
        }

        private void PopulateGrid()
        {
            int pageNumber;

            grdMgr.EmptyDataText = this.Master.EmptyGridText;

            //Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, outboundOrderViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = outboundOrderViewDTO.Entities;
            grdMgr.DataBind();
            grdMgr.SelectedIndex = currentIndex;

            CallJsGridViewHeader();


            lblPendingOrdersCount.Text = this.Master.TotalText + outboundOrderViewDTO.Entities.Count.ToString();

            if (grdMgr.PageCount > 1)
            {
                divPageGrdMgr.Visible = true;
                // Paginador
                ddlPages.Items.Clear();
                for (int i = 0; i < grdMgr.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPage) lstItem.Selected = true;

                    ddlPages.Items.Add(lstItem);
                }
                this.lblPageCount.Text = grdMgr.PageCount.ToString();
            }
            else
            {
                divPageGrdMgr.Visible = false;
            }

        }

        private void PopulateSelectedGrid()
        {
            grdSelected.EmptyDataText = this.Master.EmptyGridText;

            var configSelectedHeaderGridView = iConfigurationMGR.GetConfigByQueryName("grdSelected_ReleaseOrder", context);

            if (!configSelectedHeaderGridView.hasError() && configSelectedHeaderGridView.Entities.Count > 0)
                base.ConfigureGridOrder(grdSelected, configSelectedHeaderGridView.Entities);

            grdSelected.DataSource = selectedOrdersViewDTO.Entities;
            grdSelected.DataBind();
            grdSelected.SelectedIndex = currentIndexSelected;

            CallJsSelectedGridView();

            lblSelectedOrdersCount.Text = this.Master.TotalText + selectedOrdersViewDTO.Entities.Count.ToString();


            if (selectedOrdersViewDTO.Entities != null && selectedOrdersViewDTO.Entities.Count > 0)
            {

                btnReprocess.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_process.png";
                btnReprocess.Enabled = true;

                btnRemove.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_remove.png";
                btnRemove.Enabled = true;
            }
            else
            {
                btnReprocess.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_process_dis.png";
                btnReprocess.Enabled = false;

                btnRemove.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_remove_dis.png";
                btnRemove.Enabled = false;
            }
        }

        private void PopulateWorkZoneGrid(bool refresh)
        {
            if (refresh)
            {
                workZoneViewDTO = iDispatchingMGR.GetWorkZonesInSimulation(currentWhs, selectedOrdersViewDTO, dispatchType, context);
                Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.SelectedWorkZones] = workZoneViewDTO;
            }

            if (!workZoneViewDTO.hasError() && workZoneViewDTO.Entities != null)
            {
                grdWorkZones.EmptyDataText = this.Master.EmptyGridText;

                grdWorkZones.DataSource = workZoneViewDTO.Entities;
                grdWorkZones.DataBind();

                CallJsGridView();
            }
            else
            {
                if (workZoneViewDTO.hasError())
                {
                    this.Master.ucError.ShowError(workZoneViewDTO.Errors);
                    workZoneViewDTO.ClearError();
                }
            }
        }

        /// <summary>
        /// Simula la Liberación de Pedidos
        /// </summary>
        /// <param name="refresh">True: recalcula - False: carga desde sesión</param>
        /// <param name="pending">Indica si se está cargando una Simulación pendiente del Usuario</param>
        /// <param name="sort">True: ordena las Tareas por Prioridad</param>
        /// <param name="init">True: !Page.IsPostBack</param>
        private void PopulateSimulateGrid(bool refresh, bool pending, bool sort, bool init)
        {
            grdSimulate.EmptyDataText = this.Master.EmptyGridText;
            bool simulateOrderWillBeQueued = false;

            if (refresh)
            {                
                // Realiza la Simulación, según el tipo de Liberación
                switch (dispatchType)
                {
                    case "PIKOR":
                        simulateOrderWillBeQueued = SimulateOrderWillBeQueued("SimulateOrderWillBeQueued");

                        if (simulateOrderWillBeQueued)
                        {
                            taskSimulationViewDTO = iDispatchingMGR.CreateSimulateReleaseToQueue(selectedOrdersViewDTO, "PIKOR", context);
                        }
                        else
                        {
                            taskSimulationViewDTO = iDispatchingMGR.SimulateReleaseDispatch(selectedOrdersViewDTO, "PIKOR", context);
                        }
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKOR.TaskSimulation] = taskSimulationViewDTO;
                        break;

                    case "PIKVA":
                        taskSimulationViewDTO = iDispatchingMGR.SimulateReleaseDispatch(selectedOrdersViewDTO, "PIKVA", context);
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKVA.TaskSimulation] = taskSimulationViewDTO;
                        break;

                    case "PIKIT":
                        taskSimulationViewDTO = iDispatchingMGR.SimulateReleaseDispatchKit(selectedOrdersViewDTO, context);
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKIT.TaskSimulation] = taskSimulationViewDTO;
                        break;

                    case "PIUNK":
                        taskSimulationViewDTO = iDispatchingMGR.SimulateReleaseDispatch(selectedOrdersViewDTO, "PIUNK", context);
                        Session[WMSTekSessions.ReleaseOrderMgr.PIUNK.TaskSimulation] = taskSimulationViewDTO;
                        break;

                    case "PIKWV":
                        taskSimulationViewDTO = iDispatchingMGR.SimulateReleaseDispatchWave(selectedOrdersViewDTO, context);
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKWV.TaskSimulation] = taskSimulationViewDTO;
                        break;

                    case "PIKBT":
                        simulateOrderWillBeQueued = SimulateOrderWillBeQueued("SimulateBatchWillBeQueued");

                        if (simulateOrderWillBeQueued)
                        {
                            taskSimulationViewDTO = iDispatchingMGR.CreateSimulateReleaseToQueue(selectedOrdersViewDTO, "PIKBT", context);
                        }
                        else
                        {
                            taskSimulationViewDTO = iDispatchingMGR.SimulateReleaseDispatch(selectedOrdersViewDTO, "PIKBT", context);
                        }
                        
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKBT.TaskSimulation] = taskSimulationViewDTO;
                        break;

                    case "PIKPS":
                        taskSimulationViewDTO = iDispatchingMGR.SimulateReleaseDispatch(selectedOrdersViewDTO, "PIKPS", context);
                        Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.TaskSimulation] = taskSimulationViewDTO;
                        break;
                }
            }
            else
            {
                if (pending)
                {
                    // Busca Simulación Pendiente para el Usuario, según el tipo de Liberación
                    taskSimulationViewDTO = iDispatchingMGR.GetTaskSimulationByUserAndType(context, dispatchType);

                    // Si hay una Simulación Pendiente, pregunta si se quiere utilizar
                    if (!taskSimulationViewDTO.hasError() && taskSimulationViewDTO.Entities != null && taskSimulationViewDTO.Entities.Count > 0)
                    {
                        switch (dispatchType)
                        {
                            case "PIKOR":
                                Session[WMSTekSessions.ReleaseOrderMgr.PIKOR.TaskSimulation] = taskSimulationViewDTO;
                                break;

                            case "PIKVA":
                                Session[WMSTekSessions.ReleaseOrderMgr.PIKVA.TaskSimulation] = taskSimulationViewDTO;
                                break;

                            case "PIKIT":
                                Session[WMSTekSessions.ReleaseOrderMgr.PIKIT.TaskSimulation] = taskSimulationViewDTO;
                                break;

                            case "PIUNK":
                                Session[WMSTekSessions.ReleaseOrderMgr.PIUNK.TaskSimulation] = taskSimulationViewDTO;
                                break;

                            case "PIKWV":
                                Session[WMSTekSessions.ReleaseOrderMgr.PIKWV.TaskSimulation] = taskSimulationViewDTO;
                                break;

                            case "PIKBT":
                                Session[WMSTekSessions.ReleaseOrderMgr.PIKBT.TaskSimulation] = taskSimulationViewDTO;
                                break;

                            case "PIKPS":
                                Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.TaskSimulation] = taskSimulationViewDTO;
                                break;
                        }
                    }
                }

                switch (dispatchType)
                {
                    case "PIKOR":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKOR.TaskSimulation))
                            taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.ReleaseOrderMgr.PIKOR.TaskSimulation];
                        break;

                    case "PIKVA":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKVA.TaskSimulation))
                            taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.ReleaseOrderMgr.PIKVA.TaskSimulation];
                        break;

                    case "PIKIT":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKIT.TaskSimulation))
                            taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.ReleaseOrderMgr.PIKIT.TaskSimulation];
                        break;

                    case "PIUNK":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIUNK.TaskSimulation))
                            taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.ReleaseOrderMgr.PIUNK.TaskSimulation];
                        break;

                    case "PIKWV":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKWV.TaskSimulation))
                            taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.ReleaseOrderMgr.PIKWV.TaskSimulation];
                        break;

                    case "PIKBT":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKBT.TaskSimulation))
                            taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.ReleaseOrderMgr.PIKBT.TaskSimulation];
                        break;

                    case "PIKPS":
                        if (ValidateSession(WMSTekSessions.ReleaseOrderMgr.PIKPS.TaskSimulation))
                            taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.TaskSimulation];
                        break;
                }
            }

            if (taskSimulationViewDTO != null)
            {
                if (taskSimulationViewDTO.hasError())
                {
                    this.Master.ucError.ShowError(taskSimulationViewDTO.Errors);
                    taskSimulationViewDTO.ClearError();
                }
                else
                {
                    if (taskSimulationViewDTO.Entities != null)
                    {
                        if (taskSimulationViewDTO.Entities.Count > 0 
                            && (dispatchType == "PIKOR" || dispatchType == "PIKBT") && simulateOrderWillBeQueued)
                        {
                            this.Master.ucDialog.linkPageVisible = true;
                            this.Master.ucDialog.linkNavigationUrl = "~/Outbound/Consult/CheckSimulateOrdersInQueue.aspx";
                            this.Master.ucDialog.linkText = "Revisar estado acá";

                            this.Master.ucDialog.ShowAlert(this.lblTitleSimulate.Text, this.lblNroOrderQueuedSimulated.Text, "");

                            var selOrds = (GenericViewDTO<OutboundOrder>)Session["OrdenesSeleccionadasSimulacion"]; 

                            foreach (var ord in taskSimulationViewDTO.Entities)
                            {
                                if (selOrds.Entities.Exists(o=> o.Id == ord.OutboundOrder.Id))
                                {
                                    selOrds.Entities.Remove(ord.OutboundOrder);
                                }
                            }

                            selectedOrdersViewDTO.Entities = selOrds.Entities;

                            UpdateSelectedRowsSession();
                            //PopulateSelectedGrid();

                            taskSimulationViewDTO.Entities = new List<TaskSimulation>();
                        }


                        //Configura ORDEN de las columnas de la grilla
                        var configSimulateHeaderGridView = iConfigurationMGR.GetConfigByQueryName("grdSimulate_ReleaseOrder", context);

                        if (!configSimulateHeaderGridView.hasError() && configSimulateHeaderGridView.Entities.Count > 0)
                            base.ConfigureGridOrder(grdSimulate, configSimulateHeaderGridView.Entities);

                        if (sort)
                        {
                            // Ordena las Tareas de Simulación por Prioridad
                            var tasks = from task in taskSimulationViewDTO.Entities
                                        orderby task.OutboundOrder.Priority, task.OutboundOrder.Id descending
                                        select task;

                            grdSimulate.DataSource = tasks;
                        }
                        else
                        {
                            grdSimulate.DataSource = taskSimulationViewDTO.Entities;
                        }


                        grdSimulate.DataBind();
                        grdSimulate.SelectedIndex = currentIndexSim;

                        CallJsGridViewSimulateHeader();

                        if (taskSimulationViewDTO.Entities.Count > 0)
                        {
                            if (!init && releaseEnabled)
                            {
                                // Habilita el botón Liberar Pedidos
                                btnReleasePopUp.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_release.png";
                                btnReleasePopUp.Enabled = true;

                                btnExportExcel.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_export_excel.png";
                                btnExportExcel.Enabled = true;
                            }
                            else
                            {
                                // Deshabilita el botón Liberar Pedidos
                                btnReleasePopUp.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_release_dis.png";
                                btnReleasePopUp.Enabled = false;

                                btnExportExcel.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_export_excel_dis.png";
                                btnExportExcel.Enabled = false;
                            }
                        }
                        else
                        {
                            // Deshabilita el botón Liberar Pedidos
                            btnReleasePopUp.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_release_dis.png";
                            btnReleasePopUp.Enabled = false;

                            btnExportExcel.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_export_excel_dis.png";
                            btnExportExcel.Enabled = false;
                        }

                        // Para una Simulación Wave, muestra siempre los detalles de la única Task que se genera
                        if (dispatchType == "PIKWV")
                        {
                            currentIndexSim = 0;
                            grdSimulate.SelectedIndex = currentIndexSim;
                            LoadSimulationDetail();
                        }
                        //string whs = taskSimulationViewDTO.Entities[0].OutboundOrder.Warehouse.Code + " - " + taskSimulationViewDTO.Entities[0].OutboundOrder.Warehouse.ShortName;
                        //lblSimulationTitle.Text = lblSimulation.Text + whs;
                    }
                    else
                    {
                        // Deshabilita el botón Liberar Pedidos
                        btnReleasePopUp.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_release_dis.png";
                        btnReleasePopUp.Enabled = false;

                        btnExportExcel.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_export_excel_dis.png";
                        btnExportExcel.Enabled = false;
                    }
                }
            }
        }

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);
            
            // Oculta detalle de las grillas
            HideDetails();
        }

        /// <summary>
        /// Salva en un array las Ordenes ya seleccionadas
        /// </summary>
        protected void SaveSelectedRows()
        {
            if (selectedOrdersViewDTO.Entities.Count > 0)
            {
                for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
                {
                    foreach (OutboundOrder selectedOrder in selectedOrdersViewDTO.Entities)
                    {
                        if (selectedOrder.Id == outboundOrderViewDTO.Entities[i].Id)
                        {
                            selectedOrdersCurrentView[i] = true;
                            break;
                        }
                    }
                }

                // Recupera el Whs actual
                currentWhs = selectedOrdersViewDTO.Entities[0].Warehouse.Id;
            }
        }

        /// <summary>
        /// Salva en un array los checkboxes seleccionados
        /// </summary>
        protected void SaveCheckedRows()
        {
            // Recorre la lista de Ordenes, y obtiene el ID de las seleccionadas
            for (int i = 0; i < grdMgr.Rows.Count; i++)
            {
                if (checkedOrdersCurrentView.Count > (grdMgr.PageIndex * grdMgr.PageSize) + i)
                {
                    GridViewRow row = grdMgr.Rows[i];
                    checkedOrdersCurrentView[(grdMgr.PageIndex * grdMgr.PageSize) + i] = ((CheckBox)row.FindControl("chkSelectOrder")).Checked;

                    // Valida que se haya seleccionado al menos una orden
                    if (checkedOrdersCurrentView[(grdMgr.PageIndex * grdMgr.PageSize) + i]) existSelected = true;
                }
            }
            
        }

        
        protected bool SaveFromSelectedOrders()
        {
            bool chkOrders = false;
            OutboundOrder selectedOrder = new OutboundOrder();
            List<OutboundOrder> selectedTemp = new List<OutboundOrder>(selectedOrdersViewDTO.Entities.Count);

            // Limpia la listas de Ordenes seleccionadas y con check activo
            InitializeCheckedRows();
            InitializeSelectedRows();

            //foreach (OutboundOrder order in selectedOrdersViewDTO.Entities)
            //    selectedTemp.Add(order);

            //if(selectedTemp.Count() > 0)
            //{
            //    releaseEnabled = false;
            //    chkOrders = true;
            //}

            for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
            {
                GridViewRow row = grdSelected.Rows[i];

                if (((CheckBox)row.FindControl("chkRemoveOrder")).Checked)
                {
                    // Recupera la Orden seleccionada
                    selectedOrder = selectedOrdersViewDTO.Entities[i];

                    // Agrega la Orden seleccionada de la lista de Ordenes Seleccionadas
                    selectedTemp.Add(selectedOrder);

                    releaseEnabled = false;
                    chkOrders = true;
                }
            }

            if (chkOrders)
            {
                selectedOrdersViewDTO.Entities = selectedTemp;

                SaveSelectedRows();

                UpdateSelectedRowsSession();
            }

            return chkOrders;
        }


        /// <summary>
        /// Limpia la lista de checkboxes seleccionados
        /// </summary>
        protected void InitializeCheckedRows()
        {
            checkedOrdersCurrentView.Clear();

            if (outboundOrderViewDTO.Entities != null && checkedOrdersCurrentView != null)
            {
                for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
                {
                    checkedOrdersCurrentView.Add(false);
                }
            }
        }

        /// <summary>
        /// Limpia la lista de Ordenes seleccionados
        /// </summary>
        protected void InitializeSelectedRows()
        {
            selectedOrdersCurrentView.Clear();

            if (outboundOrderViewDTO.Entities != null && selectedOrdersCurrentView != null)
            {
                for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
                {
                    selectedOrdersCurrentView.Add(false);
                }
            }
        }

        /// <summary>
        /// Retorna el detalle de cada documento de Salida y lo carga en un datagrid
        /// </summary>
        /// <param name="index"></param>
        protected void LoadOutboundOrderDetails()
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (currentIndex != -1)
            {
                divDetail.Visible = true;

                this.lblNroDoc.Text = outboundOrderViewDTO.Entities[currentIndex].Number;

                int id = outboundOrderViewDTO.Entities[grdMgr.SelectedIndex].Id;

                outboundDetailViewDTO = iDispatchingMGR.GetDetailByIdOutboundWithStock(context, id);

                if (outboundDetailViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!outboundDetailViewDTO.hasConfigurationError() && outboundDetailViewDTO.Configuration != null && outboundDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, outboundDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = outboundDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }
            }
        }

        /// <summary>
        /// Retorna el detalle de una Ordenes Seleccionadas para Simulación
        /// </summary>
        protected void LoadSelectedOutboundOrderDetails()
        {
            grdSelectedDetail.EmptyDataText = this.Master.NoDetailsText;

            if (currentIndexSelected != -1)
            {
                divSelectedDetail.Visible = true;

                this.lblSelectedNroDoc.Text = selectedOrdersViewDTO.Entities[currentIndexSelected].Number;

                InitializeSelectedOrders();

                selectedDetailViewDTO = (GenericViewDTO<OutboundDetail>)Session[WMSTekSessions.ReleaseOrderMgr.SelectedDetail];

                // Si el detalle ya fue cargado, no lo busca en base de datos
                if (selectedOrdersViewDTO.Entities[grdSelected.SelectedIndex].OutboundDetails != null)
                {
                    selectedDetailViewDTO.Entities = selectedOrdersViewDTO.Entities[grdSelected.SelectedIndex].OutboundDetails;
                }
                else
                {
                    int id = selectedOrdersViewDTO.Entities[grdSelected.SelectedIndex].Id;
                    selectedDetailViewDTO = iDispatchingMGR.GetDetailByIdOutboundWithStock(context, id);
                    selectedOrdersViewDTO.Entities[grdSelected.SelectedIndex].OutboundDetails = selectedDetailViewDTO.Entities;
                }

                if (selectedDetailViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!selectedDetailViewDTO.hasConfigurationError() && selectedDetailViewDTO.Configuration != null && selectedDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdSelectedDetail, selectedDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdSelectedDetail.DataSource = selectedDetailViewDTO.Entities;
                    grdSelectedDetail.DataBind();

                    CallJsSelectedDetailGridView();
                }
            }
        }

        /// <summary>
        /// Despliega el detalle de la fila seleccionada de la grilla de Simulación 
        /// </summary>
        /// <param name="index"></param>
        protected void LoadSimulationDetail()
        {
            grdDetailSim.EmptyDataText = this.Master.NoDetailsText;

            if (currentIndexSim != -1)
            {
                if (taskSimulationViewDTO.Entities != null && taskSimulationViewDTO.Entities.Count > 0)
                {
                    divDetailSim.Visible = true;
                    lblNroDocSim.Text = taskSimulationViewDTO.Entities[currentIndexSim].OutboundOrder.Number;

                    if (taskSimulationViewDTO.Entities[grdSimulate.SelectedIndex].Details != null)
                        taskDetailSimulationViewDTO = taskSimulationViewDTO.Entities[grdSimulate.SelectedIndex].Details;

                    var configSimulateDetailGridView = iConfigurationMGR.GetConfigByQueryName("grdDetailSim_ReleaseOrder", context);

                    if (!configSimulateDetailGridView.hasError() && configSimulateDetailGridView.Entities.Count > 0)
                        base.ConfigureGridOrder(grdDetailSim, configSimulateDetailGridView.Entities);

                    // Detalle de Documentos de Entrada
                    grdDetailSim.DataSource = taskDetailSimulationViewDTO.Entities;
                    grdDetailSim.DataBind();

                    CallJsGridViewSimulateDetail();
                }
            }
        }

        

        /// <summary>
        /// Valida que las Ordenes seleccionadas pertenezcan al mismo Warehouse
        /// </summary>
        protected bool SameWarehouse()
        {
            bool sameWhs = true;
            int idWhs = 0, idWhs2;

            // Valida Ordenes seleccionadas en la vista actual
            for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
            {
                if (checkedOrdersCurrentView[i] && !selectedOrdersCurrentView[i])
                {
                    // Recupera el Whs de la primer Orden seleccionada
                    idWhs = outboundOrderViewDTO.Entities[i].Warehouse.Id;
                    break;
                }
            }

            for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
            {
                if (checkedOrdersCurrentView[i])
                {
                    idWhs2 = outboundOrderViewDTO.Entities[i].Warehouse.Id;

                    // Compara el Whs de la primer Orden seleccionada con los restantes
                    if (idWhs != idWhs2)
                    {
                        sameWhs = false;
                        break;
                    }
                }
            }

            // Valida Ordenes seleccionadas contra la lista completa de Ordenes a Liberar
            if (sameWhs)
            {
                if (selectedOrdersViewDTO.Entities.Count > 0)
                {
                    idWhs2 = selectedOrdersViewDTO.Entities[0].Warehouse.Id;

                    // Compara el Whs de la primer Orden seleccionada con el Whs de la primer Orden de la lista a Liberar
                    if (idWhs != idWhs2)
                    {
                        sameWhs = false;
                    }
                }
            }

            currentWhs = idWhs;
            return sameWhs;
        }

        /// <summary>
        /// Valida que las Ordenes seleccionadas pertenezcan al mismo Owner
        /// </summary>
        protected bool SameOwner()
        {
            bool sameOwner = true;
            int idOwner = 0, idOwner2;

            // Valida Ordenes seleccionadas en la vista actual
            for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
            {
                if (checkedOrdersCurrentView[i] && !selectedOrdersCurrentView[i])
                {
                    // Recupera el Owner de la primer Orden seleccionada
                    idOwner = outboundOrderViewDTO.Entities[i].Owner.Id;
                    break;
                }
            }

            for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
            {
                if (checkedOrdersCurrentView[i])
                {
                    idOwner2 = outboundOrderViewDTO.Entities[i].Owner.Id;

                    // Compara el Owner de la primer Orden seleccionada con los restantes
                    if (idOwner != idOwner2)
                    {
                        sameOwner = false;
                        break;
                    }
                }
            }

            // Valida Ordenes seleccionadas contra la lista completa de Ordenes a Liberar
            if (sameOwner)
            {
                if (selectedOrdersViewDTO.Entities.Count > 0)
                {
                    idOwner2 = selectedOrdersViewDTO.Entities[0].Owner.Id;

                    // Compara el Owner de la primer Orden seleccionada con el Owner de la primer Orden de la lista a Liberar
                    if (idOwner != idOwner2)
                    {
                        sameOwner = false;
                    }
                }
            }

            return sameOwner;
        }


        /// <summary>
        /// Quita Ordenes de la lista de Ordenes Seleccionadas
        /// </summary>
        protected void RemoveFromSelectedOrders()
        {
            OutboundOrder selectedOrder = new OutboundOrder();
            List<OutboundOrder> selectedTemp = new List<OutboundOrder>(selectedOrdersViewDTO.Entities.Count);

            // Limpia la listas de Ordenes seleccionadas y con check activo
            InitializeCheckedRows();
            InitializeSelectedRows();

            foreach (OutboundOrder order in selectedOrdersViewDTO.Entities)
                selectedTemp.Add(order);

            for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
            {
                GridViewRow row = grdSelected.Rows[i];

                if (((CheckBox)row.FindControl("chkRemoveOrder")).Checked)
                {
                    // Recupera la Orden seleccionada
                    selectedOrder = selectedOrdersViewDTO.Entities[i];

                    // Quita la Orden seleccionada de la lista de Ordenes Seleccionadas
                    selectedTemp.Remove(selectedOrder);

                    iDispatchingMGR.RulesDeleteAllByOrder(selectedOrder.Id, context);

                    releaseEnabled = false;
                }
            }

            iDispatchingMGR.DeleteTaskSimulation(dispatchType, context);

            selectedOrdersViewDTO.Entities = selectedTemp;

            SaveSelectedRows();

            UpdateSelectedRowsSession();
        }

        /// <summary>
        /// Arma la lista de Ordenes seleccionadas para la simulación de Liberación de Pedidos
        /// </summary>
        protected void AddToSelectedOrders()
        {
            OutboundOrder selectedOrder = new OutboundOrder();

            for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
            {
                if (checkedOrdersCurrentView[i] && !selectedOrdersCurrentView[i])
                {
                    // Recupera la Orden seleccionada
                    selectedOrder = outboundOrderViewDTO.Entities[i];

                    // Agrega la Orden seleccionada a la lista de Ordenes seleccionadas
                    selectedOrdersViewDTO.Entities.Add(selectedOrder);
                }
            }

            /*
            // Ordena las Ordenes Seleccionadas por Prioridad
            var orders = from order in selectedOrdersViewDTO.Entities
                        orderby order.Priority, order.Id descending
                        select order;

            selectedOrdersViewDTO.Entities = orders.ToList();
            */

            // Limpia la listas de Ordenes seleccionadas y con check activo
            InitializeCheckedRows();
            InitializeSelectedRows();

            SaveSelectedRows();

            UpdateSelectedRowsSession();

            releaseEnabled = false;
        }
        protected void AddToSelectedOrdersAfterChangeRules()
        {
            OutboundOrder selectedOrder = new OutboundOrder();

            for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
            {
                if (selectedOrdersCurrentView[i])
                {
                    // Recupera la Orden seleccionada
                    selectedOrder = outboundOrderViewDTO.Entities[i];

                    // Agrega la Orden seleccionada a la lista de Ordenes seleccionadas
                    selectedOrdersViewDTO.Entities.Add(selectedOrder);
                }
            }

            InitializeCheckedRows();
            InitializeSelectedRows();

            SaveSelectedRows();

            UpdateSelectedRowsSession();

            releaseEnabled = false;
        }

        /// <summary>
        /// Arma la lista de Ordenes Seleccionadas para la simulación de Liberación de Pedidos, desde una Simulación Pendiente
        /// </summary>
        protected void AddToSelectedOrdersFromTaskList()
        {
            selectedOrdersViewDTO.Entities.Clear();

            // Busca Simulación Pendiente para el Usuario, según el tipo de Liberación que esté realizando
            switch (dispatchType)
            {
                // En la Liberación por Pedidos, cada Task se corresponde con un Outbound Order
                case "PIKOR":
                    foreach (TaskSimulation task in taskSimulationViewDTO.Entities)
                    {
                        selectedOrdersViewDTO.Entities.Add(task.OutboundOrder);
                    }
                    break;

                case "PIKVA":
                    //selectedOrdersViewDTO = iDispatchingMGR.GetOutboundOrderInSimulation(context, taskSimulationViewDTO.Entities[0]);
                    // TODO: ver en caso PIKVA
                    break;

                case "PIKIT":
                    //selectedOrdersViewDTO = iDispatchingMGR.GetOutboundOrderInSimulation(context, taskSimulationViewDTO.Entities[0]);
                    // TODO: ver en caso PIKIT
                    break;

                case "PIUNK":
                    //selectedOrdersViewDTO = iDispatchingMGR.GetOutboundOrderInSimulation(context, taskSimulationViewDTO.Entities[0]);
                    // TODO: ver en caso PIUNK
                    break;

                // En la Liberación Wave, existe un solo Task para todos los pedidos, 
                // por lo que la información se rescata de la tabla TaskOutboundOrderSimulation
                case "PIKWV":
                    selectedOrdersViewDTO = iDispatchingMGR.GetOutboundOrderInSimulation(context, taskSimulationViewDTO.Entities[0]);
                    break;

                case "PIKBT":
                    //selectedOrdersViewDTO = iDispatchingMGR.GetOutboundOrderInSimulation(context, taskSimulationViewDTO.Entities[0]);
                    // TODO: ver en caso PIKBT
                    break;

                case "PIKPS":
                    //selectedOrdersViewDTO = iDispatchingMGR.GetOutboundOrderInSimulation(context, taskSimulationViewDTO.Entities[0]);
                    // TODO: ver en caso PIKPS
                    break;
            }


            // Limpia la listas de Ordenes seleccionadas y con check activo
            InitializeCheckedRows();
            InitializeSelectedRows();

            SaveSelectedRows();

            UpdateSelectedRowsSession();
        }


        /// <summary>
        /// Activa checkboxes de la lista de Ordenes seleccionadas, según las Ordenes que contiene la  Simulación pendiente recuperada.
        /// </summary>
        protected void SaveSelectedRowsFromPendingSimulation()
        {
            for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
            {
                foreach (TaskSimulation taskSimulation in taskSimulationViewDTO.Entities)
                {
                    if (taskSimulation.OutboundOrder.Id == outboundOrderViewDTO.Entities[i].Id)
                    {
                        GridViewRow row = grdMgr.Rows[i];
                        ((CheckBox)row.FindControl("chkSelectOrder")).Checked = true;
                        break;
                    }
                }
            }

            SaveCheckedRows();
        }

        /// <summary>
        /// Sube o baja la prioridad de la Orden seleccionada 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="action"></param>
        protected void ChangeOrderPriority(int index, string action)
        {
            OutboundOrder selectedOrder = new OutboundOrder();

            releaseEnabled = false;

            // Subir prioridad
            if (index > 0 && action == "up")
            {
                selectedOrder = selectedOrdersViewDTO.Entities[index];
                selectedOrdersViewDTO.Entities.RemoveAt(index);
                selectedOrdersViewDTO.Entities.Insert(index - 1, selectedOrder);
            }

            // Bajar prioridad
            if (index < selectedOrdersViewDTO.Entities.Count - 1 && action == "down")
            {
                selectedOrder = selectedOrdersViewDTO.Entities[index];
                selectedOrdersViewDTO.Entities.RemoveAt(index);
                selectedOrdersViewDTO.Entities.Insert(index + 1, selectedOrder);
            }

            UpdateSelectedRowsSession();
        }

        /// <summary>
        /// Sube o baja la prioridad de la WorkZone seleccionada 
        /// </summary>
        protected void ChangeWorkZonePriority(int index, string action)
        {
            WorkZone selectedWorkZone = new WorkZone();

            // Subir prioridad
            if (index > 0 && action == "up")
            {
                selectedWorkZone = workZoneViewDTO.Entities[index];
                workZoneViewDTO.Entities.RemoveAt(index);
                workZoneViewDTO.Entities.Insert(index - 1, selectedWorkZone);
            }

            // Bajar prioridad
            if (index < workZoneViewDTO.Entities.Count - 1 && action == "down")
            {
                selectedWorkZone = workZoneViewDTO.Entities[index];
                workZoneViewDTO.Entities.RemoveAt(index);
                workZoneViewDTO.Entities.Insert(index + 1, selectedWorkZone);
            }

            // Salva el nuevo orden
            switch (dispatchType)
            {
                case "PIKPS":
                    Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.SelectedWorkZones] = workZoneViewDTO;
                    break;
            }

            // Actualiza la grilla de WorkZones
            PopulateWorkZoneGrid(false);
            mpReleaseDispatch.Show();
        }

        /// <summary>
        /// Quita la Orden de la lista de Ordenes Seleccionadas
        /// </summary>
        protected void RemoveSelectedOrder(int index)
        {
            OutboundOrder selectedOrder = new OutboundOrder();

            if (index >= 0 && index < selectedOrdersViewDTO.Entities.Count)
            {
                selectedOrdersViewDTO.Entities.RemoveAt(index);

                // Actualiza lista de Ordenes seleccionadas
                UpdateSelectedRowsSession();
                InitializeSelectedRows();
                SaveSelectedRows();
            }
        }


        /// <summary>
        /// Valida que haya Stock suficiente para las Ordenes seleccionadas
        /// </summary>
        private bool ValidateStock()
        {
            foreach (TaskSimulation taskSimulation in taskSimulationViewDTO.Entities)
            {
                if (taskSimulation.CompliancePct < 100)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Valida que las Ordenes seleccionadas no se encuentren en otro proceso de Simulación
        /// </summary>
        private bool OrderInSumulation()
        {
            foreach (OutboundOrder order in selectedOrdersViewDTO.Entities)
            {
                if (order.InOtherSimulation)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Valida que haya Stock suficiente para las Ordenes seleccionadas con 'FullShipment' activo
        /// </summary>
        private bool ValidateFullShipment()
        {
            foreach (TaskSimulation taskSimulation in taskSimulationViewDTO.Entities)
            {
                if (taskSimulation.CompliancePct < 100 && taskSimulation.OutboundOrder.FullShipment)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Valida que los items contengan reglas asociadas
        /// </summary>
        private string ValidateCustomRuleItem()
        {
            string mensaje = string.Empty;

            foreach (TaskSimulation taskSimulation in taskSimulationViewDTO.Entities)
            {
                foreach (TaskDetailSimulation detail in taskSimulation.Details.Entities)
                {
                    if (detail.ExistItemCustomRules == 0)
                    {
                        mensaje = lblNotExistCustomRuleItems.Text;
                        mensaje = mensaje.Replace("[DOC]", taskSimulation.OutboundOrder.Number).Replace("[ITEM]",detail.OutboundDetail.Item.Code);
                        return mensaje;
                    }
                } 
                
            }

            return mensaje;
        }

        /// <summary>
        /// Muestra opciones para configurar la Liberación de Pedidos 
        /// </summary>
        protected void ShowReleaseOrdersPopUp()
        {

            switch (dispatchType)
            {
                case "PIKOR":
                    chkBackOrder.Checked = false;
                    txtExpDateBackOrder.Text = "";
                    divModalFields.Visible = true;
                    divUserNbr.Visible = true;
                    divUserNbrSorting.Visible = false;
                    divWorkZones.Visible = false;
                    divPickingTitle.Visible = false;
                    divKitting.Visible = false;
                    divVas.Visible = false;

                    divLocStageTarget.Visible = false;
                    divLocStageDispatch.Visible = true;
                    divLocDock.Visible = true;
                    divPutawayLpn.Visible = false;

                    if (fullStock)
                    {
                        divCrossDock.Visible = false;
                        divBackOrder.Visible = false;
                    }
                    else
                    {
                        if (AllowCrossDock())
                        {
                            divCrossDock.Visible = true;
                        }
                        else
                        {
                            divCrossDock.Visible = false;
                        }
                        if (AllowBackOrder())
                        {
                            divBackOrder.Visible = true;
                        }
                        else
                        {
                            divBackOrder.Visible = false;
                        }
                    }
                    divExpDateBackOrder.Visible = false;
                    this.rfvLocStageDispatch.Enabled= false;
                    this.rfvLocDock.Enabled = false;
                    this.divSorting.Visible = false;
                    this.lblLocStageDispatch.Text = this.lblTitleLocDispatch.Text;
                    this.rfvLocStageDispatch.ErrorMessage = this.lblTitleLocDispatch.Text + this.lblRequiredField.Text;
                    break;

                // Liberación VAS: se ingresa el Número de Opeararios para VAS
                case "PIKVA":
                    chkBackOrder.Checked = false;
                    txtExpDateBackOrder.Text = "";
                    divModalFields.Visible = true;
                    divUserNbr.Visible = true;
                    divUserNbrSorting.Visible = false;
                    divWorkZones.Visible = false;
                    divPickingTitle.Visible = true;
                    divKitting.Visible = false;
                    divVas.Visible = true;
                    lblVasTitle.Text = lblVasTitleText.Text;

                    divLocStageTarget.Visible = false;
                    divLocStageDispatch.Visible = true;
                    divLocDock.Visible = true;
                    divPutawayLpn.Visible = false;
                    divCrossDock.Visible = false;
                    divBackOrder.Visible = false;
                    divExpDateBackOrder.Visible = false;
                    this.rfvLocStageDispatch.Enabled = true;
                    this.rfvLocDock.Enabled = false;
                    this.divSorting.Visible = false;
                    this.lblLocStageDispatch.Text = this.lblTitleLocDispatch.Text;
                    this.rfvLocStageDispatch.ErrorMessage = this.lblTitleLocDispatch.Text + this.lblRequiredField.Text;
                    break;

                // Liberación Kit: se ingresa el Número de Opeararios para Kitting
                case "PIKIT":
                    chkBackOrder.Checked = false;
                    txtExpDateBackOrder.Text = "";
                    divModalFields.Visible = true;
                    divUserNbr.Visible = true;
                    divUserNbrSorting.Visible = false;
                    divWorkZones.Visible = false;
                    divPickingTitle.Visible = true;
                    divKitting.Visible = true;
                    divVas.Visible = false;
                    lblKittingTitle.Text = lblKittingTitleText.Text;
          
                    divLocStageTarget.Visible = false;
                    divLocStageDispatch.Visible = true;
                    divLocDock.Visible = true;
                    divPutawayLpn.Visible = true;
                    chkPutawayLpn.Checked = false;
                    divCrossDock.Visible = false;
                    divBackOrder.Visible = false;
                    divExpDateBackOrder.Visible = false;
                    this.rfvLocStageDispatch.Enabled = true;
                    this.rfvLocDock.Enabled = false;
                    this.divSorting.Visible = false;
                    this.lblLocStageDispatch.Text = this.lblTitleLocDispatch.Text;
                    this.rfvLocStageDispatch.ErrorMessage = this.lblTitleLocDispatch.Text + this.lblRequiredField.Text;
                    //OLD
                    //divLocStageTarget.Visible = true;
                    //divLocStageDispatch.Visible = false;
                    //divLocDock.Visible = false;
                    //this.rfvLocStageDispatch.Enabled = false;
                    //this.rfvLocDock.Enabled = false;
                    break;

                // Liberación Unkit: se ingresa el Número de Opeararios para Kitting
                case "PIUNK":
                    chkBackOrder.Checked = false;
                    txtExpDateBackOrder.Text = "";
                    divModalFields.Visible = true;
                    divUserNbr.Visible = true;
                    divUserNbrSorting.Visible = false;
                    divWorkZones.Visible = false;
                    divPickingTitle.Visible = true;
                    divKitting.Visible = true;
                    divVas.Visible = false;
                    lblKittingTitle.Text = lblUnkittingTitleText.Text;

                    divLocStageTarget.Visible = false;
                    divLocStageDispatch.Visible = true;
                    divLocDock.Visible = true;
                    divPutawayLpn.Visible = false;
                    divCrossDock.Visible = false;
                    divBackOrder.Visible = false;
                    divExpDateBackOrder.Visible = false;
                    this.rfvLocStageDispatch.Enabled = true;
                    this.rfvLocDock.Enabled = false;
                    this.divSorting.Visible = false;
                    this.lblLocStageDispatch.Text = this.lblTitleLocDispatch.Text;
                    this.rfvLocStageDispatch.ErrorMessage = this.lblTitleLocDispatch.Text + this.lblRequiredField.Text;
                    break;

                // Liberación Wave: se ingresa el Número de Opeararios para Sorting
                case "PIKWV":
                    chkBackOrder.Checked = false;
                    txtExpDateBackOrder.Text = "";
                    divModalFields.Visible = true;
                    divUserNbr.Visible = true;
                    divUserNbrSorting.Visible = true;
                    divWorkZones.Visible = false;
                    divPickingTitle.Visible = false;
                    divKitting.Visible = false;
                    divVas.Visible = false;

                    divLocStageTarget.Visible = false;
                    divLocStageDispatch.Visible = true;
                    divLocDock.Visible = true;
                    divPutawayLpn.Visible = false;
                    divCrossDock.Visible = false;
                    if (fullStock)
                        divBackOrder.Visible = false;
                    else
                        divBackOrder.Visible = true;
                    divExpDateBackOrder.Visible = false;
                    this.rfvLocStageDispatch.Enabled = true;
                    this.rfvLocDock.Enabled = false;

                    chkSorting.Checked = (GetCfgParameter(CfgParameterName.AllowCheckPacking.ToString()) == "1" ? true : false);
                    divSorting.Visible = true;
                    this.lblLocStageDispatch.Text = this.lblTitleLocSorting.Text;
                    this.rfvLocStageDispatch.ErrorMessage = this.lblTitleLocSorting.Text + this.lblRequiredField.Text;
                    break;

                // Para liberación Batch NO se ingresa el Número de Opeararios (siempre es 1)
                case "PIKBT":
                    chkBackOrder.Checked = false;
                    txtExpDateBackOrder.Text = "";
                    divModalFields.Visible = true;
                    divUserNbr.Visible = false;
                    divUserNbrSorting.Visible = false;
                    divWorkZones.Visible = false;
                    divPickingTitle.Visible = false;
                    divKitting.Visible = false;
                    divVas.Visible = false;

                    divLocStageTarget.Visible = false;
                    divLocStageDispatch.Visible = true;
                    divLocDock.Visible = true;
                    divPutawayLpn.Visible = false;
                    divCrossDock.Visible = false;
                    if (fullStock)
                        divBackOrder.Visible = false;
                    else
                        divBackOrder.Visible = true;
                    divExpDateBackOrder.Visible = false;
                    this.rfvLocStageDispatch.Enabled = false;
                    this.rfvLocDock.Enabled = false;
                    this.divSorting.Visible = false;
                    this.lblLocStageDispatch.Text = this.lblTitleLocDispatch.Text;
                    this.rfvLocStageDispatch.ErrorMessage = this.lblTitleLocDispatch.Text + this.lblRequiredField.Text;
                    break;

                // Liberación Pick and Pass: se muestra una grilla para configurar las zonas involucradas
                case "PIKPS":
                    chkBackOrder.Checked = false;
                    txtExpDateBackOrder.Text = "";
                    PopulateWorkZoneGrid(true);
                    divModalFields.Visible = false;
                    divWorkZones.Visible = true;

                    divLocStageTarget.Visible = false;
                    divLocStageDispatch.Visible = true;
                    divLocDock.Visible = true;
                    divPutawayLpn.Visible = false;
                    divCrossDock.Visible = false;
                    divBackOrder.Visible = false;
                    divExpDateBackOrder.Visible = false;
                    this.rfvLocStageDispatch.Enabled = false;
                    this.rfvLocDock.Enabled = false;
                    this.divSorting.Visible = false;
                    this.lblLocStageDispatch.Text = this.lblTitleLocDispatch.Text;
                    this.rfvLocStageDispatch.ErrorMessage = this.lblTitleLocDispatch.Text + this.lblRequiredField.Text;
                    break;
            }

            ddlLocStageTarget.SelectedIndex = 0;
            divReleaseDispatch.Visible = true;
            mpReleaseDispatch.Show();
        }

        /// <summary>
        /// Libera las Ordenes seleccionadas, basado en la prioridad configurada en la Simulación
        /// </summary>
        protected void ReleaseOrders()
        {
            Task taskInfo = new Task();
            Task kittingTaskInfo = new Task();
            Task vasTaskInfo = new Task();

            var releaseOrderWillBeQueued = false;

            // Información general de todas las Tareas a generar
            if (divUserNbr.Visible)
                taskInfo.WorkersRequired = Convert.ToInt16(txtUserNbr.Text);
            else
                taskInfo.WorkersRequired = 1;

            taskInfo.WorkersAssigned = 0;

            if (dispatchType == "PIKPS")
                taskInfo.Priority = Convert.ToInt16(txtPriorityPickAndPass.Text);
            else
                taskInfo.Priority = Convert.ToInt16(txtPriority.Text);

            //Nueva mModificacion de Ubicaciones -->27-11-2015
            // --> Inicio
            if (dispatchType == "PIKPS")
            {
                taskInfo.StageTarget = new Location(ddlLocStageTarget.SelectedValue);
            }
            else
            {
                taskInfo.StageSource = new Location(this.ddlLocStageDispatch.SelectedValue);
                taskInfo.StageTarget = new Location(this.ddlLocDock.SelectedValue);
            }
            //switch (dispatchType)
            //{
            //    case "PIKOR":
            //        taskInfo.StageSource = new Location(this.ddlLocStageDispatch.SelectedValue);
            //        taskInfo.StageTarget = new Location(this.ddlLocDock.SelectedValue);
            //        break;

            //    // Liberación VAS: se ingresa el Número de Opeararios para VAS
            //    case "PIKVA":
            //        taskInfo.StageTarget = new Location(ddlLocStageTarget.SelectedValue);
            //        break;

            //    // Liberación Kit: se ingresa el Número de Opeararios para Kitting
            //    case "PIKIT":
            //        taskInfo.StageTarget = new Location(ddlLocStageTarget.SelectedValue);
            //        break;

            //    // Liberación Unkit: se ingresa el Número de Opeararios para Kitting
            //    case "PIUNK":
            //        taskInfo.StageTarget = new Location(ddlLocStageTarget.SelectedValue);
            //        break;

            //    // Liberación Wave: se ingresa el Número de Opeararios para Sorting
            //    case "PIKWV":
            //        taskInfo.StageTarget = new Location(ddlLocStageTarget.SelectedValue);
            //        break;

            //    // Para liberación Batch NO se ingresa el Número de Opeararios (siempre es 1)
            //    case "PIKBT":
            //        taskInfo.StageTarget = new Location(ddlLocStageTarget.SelectedValue);
            //        break;

            //    // Liberación Pick and Pass: se muestra una grilla para configurar las zonas involucradas
            //    case "PIKPS":
            //        taskInfo.StageTarget = new Location(ddlLocStageTarget.SelectedValue);
            //        break;
            //}
            // --> Fin
            
            taskInfo.Warehouse = selectedOrdersViewDTO.Entities[0].Warehouse;
            taskInfo.Owner = selectedOrdersViewDTO.Entities[0].Owner;
            taskInfo.IsComplete = false;
            taskInfo.Status = true;
            taskInfo.IdTrackTaskType = (int)TrackTaskTypeName.Liberada;
            taskInfo.TypeCode = dispatchType;

            //Carga los tipo de lpn para realizar el precubing
            context.MainFilter[Convert.ToInt16(EntityFilterName.LpnType)].FilterValues = new List<FilterItem>();
            foreach (String item in constTypeLpnAudit)
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.LpnType)].FilterValues.Add(new FilterItem(item));
            }

            if (dispatchType == "PIKIT" || dispatchType == "PIUNK")
            {
                kittingTaskInfo.Priority = Convert.ToInt16(txtPriorityKitting.Text);
                kittingTaskInfo.WorkersRequired = Convert.ToInt16(txtUserNbrKitting.Text);
                if (dispatchType == "PIKIT")
                {
                    if (chkPutawayLpn.Checked)
                    {
                        for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                        {
                            selectedOrdersViewDTO.Entities[i].InmediateProcess = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                        {
                            selectedOrdersViewDTO.Entities[i].InmediateProcess = false;
                        }
                    }

                }
            }

            if (dispatchType == "PIKVA")
            {
                vasTaskInfo.Priority = Convert.ToInt16(txtPriorityVas.Text);
                vasTaskInfo.WorkersRequired = Convert.ToInt16(txtUserNbrVas.Text);
            }

            // Envía las Ordenes a liberar
            switch (dispatchType)
            {
                case "PIKOR":
                    if (chkBackOrder.Checked)
                    {
                        for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                        {
                            selectedOrdersViewDTO.Entities[i].AllowBackOrder = true;
                            selectedOrdersViewDTO.Entities[i].ExpirationDate = Convert.ToDateTime(txtExpDateBackOrder.Text);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                        {
                            selectedOrdersViewDTO.Entities[i].AllowBackOrder = false;
                        }
                    }
                    if(chkCrossDock.Checked)
                    {
                        for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                        {
                            selectedOrdersViewDTO.Entities[i].AllowCrossDock = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                        {
                            selectedOrdersViewDTO.Entities[i].AllowCrossDock = false;
                        }
                    }

                    releaseOrderWillBeQueued = ReleaseOrderWillBeQueued();

                    if (releaseOrderWillBeQueued)
                        taskSimulationViewDTO = iDispatchingMGR.CreateOrderToQueue(taskInfo, selectedOrdersViewDTO, constTypeLpnClosedBox, Decimal.Parse(constPercentageTolerance), context);
                    else
                        taskSimulationViewDTO = iDispatchingMGR.ReleaseDispatch(taskInfo, selectedOrdersViewDTO, constTypeLpnClosedBox, Decimal.Parse(constPercentageTolerance), context);

                    break;

                case "PIKVA":
                    taskSimulationViewDTO = iDispatchingMGR.ReleaseDispatchVAS(taskInfo, vasTaskInfo, selectedOrdersViewDTO, context);
                    break;

                case "PIKIT":
                    taskSimulationViewDTO = iDispatchingMGR.ReleaseDispatchKit(taskInfo, kittingTaskInfo, selectedOrdersViewDTO, context);
                    break;

                case "PIUNK":
                    taskSimulationViewDTO = iDispatchingMGR.ReleaseDispatchUnkit(taskInfo, kittingTaskInfo, selectedOrdersViewDTO, context);
                    break;

                case "PIKWV":
                    if (chkBackOrder.Checked)
                    {
                        for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                        {
                            selectedOrdersViewDTO.Entities[i].InmediateProcess = this.chkSorting.Checked;
                            selectedOrdersViewDTO.Entities[i].AllowBackOrder = true;
                            selectedOrdersViewDTO.Entities[i].ExpirationDate = Convert.ToDateTime(txtExpDateBackOrder.Text);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                        {
                            selectedOrdersViewDTO.Entities[i].InmediateProcess = this.chkSorting.Checked;
                            selectedOrdersViewDTO.Entities[i].AllowBackOrder = false;
                        }
                    }
                    taskSimulationViewDTO = iDispatchingMGR.ReleaseDispatchWave(taskInfo, selectedOrdersViewDTO, Convert.ToInt16(txtUserNbrSorting.Text), context);
                    break;

                case "PIKBT":
                    if (chkBackOrder.Checked)
                    {
                        for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                        {
                            selectedOrdersViewDTO.Entities[i].AllowBackOrder = true;
                            selectedOrdersViewDTO.Entities[i].ExpirationDate = Convert.ToDateTime(txtExpDateBackOrder.Text);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < selectedOrdersViewDTO.Entities.Count; i++)
                        {
                            selectedOrdersViewDTO.Entities[i].AllowBackOrder = false;
                        }
                    }

                    releaseOrderWillBeQueued = ReleaseBatchWillBeQueued();

                    if (releaseOrderWillBeQueued)
                        taskSimulationViewDTO = iDispatchingMGR.CreateBatchToQueue(taskInfo, selectedOrdersViewDTO, constTypeLpnClosedBox, Decimal.Parse(constPercentageTolerance), context);
                    else
                        taskSimulationViewDTO = iDispatchingMGR.ReleaseDispatchBatch(taskInfo, selectedOrdersViewDTO, constTypeLpnClosedBox, Decimal.Parse(constPercentageTolerance), context);

                    break;

                case "PIKPS":
                    taskInfo.TaskDetails = GetWorkZonesConfiguration();
                    taskSimulationViewDTO = iDispatchingMGR.ReleaseDispatchPickAndPass(taskInfo, selectedOrdersViewDTO, context);
                    break;
            }

            mpReleaseDispatch.Hide();

            //Muestra mensaje en la barra de status
            crud = true;        
                        

            // Actualiza la lista de Ordenes Pendientes
            if (taskSimulationViewDTO.hasError())
            {
                UpdateSession(true);
            }
            else
            {
                UpdateSession(false);

                if (dispatchType == "PIKWV")
                {
                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNroWaveGenerate.Text.Replace("[NROWAVE]", taskSimulationViewDTO.Entities[0].OutboundOrder.Id.ToString()), "");
                }
                if (dispatchType == "PIKBT")
                {
                    if (releaseOrderWillBeQueued)
                    {
                        this.Master.ucDialog.linkPageVisible = true;
                        this.Master.ucDialog.linkNavigationUrl = "~/Outbound/Consult/CheckBatchesInQueue.aspx";
                        this.Master.ucDialog.linkText = "Revisar estado acá";

                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNroBatchGenerate.Text.Replace("[NROBATCH]", taskSimulationViewDTO.Entities[0].Id.ToString()), "");
                    }
                    else
                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNroBatchGenerate.Text.Replace("[NROBATCH]", taskSimulationViewDTO.Entities[0].Id.ToString()), "");
                }
                if (dispatchType == "PIKOR" && releaseOrderWillBeQueued)
                {
                    this.Master.ucDialog.linkPageVisible = true;
                    this.Master.ucDialog.linkNavigationUrl = "~/Outbound/Consult/CheckOrdersInQueue.aspx";
                    this.Master.ucDialog.linkText = "Revisar estado acá";

                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNroOrderQueuedGenerate.Text, "");
                }
                else
                {
                    ucStatus.ShowMessage(taskSimulationViewDTO.MessageStatus.Message);
                }

                // Limpia la Simulación
                taskSimulationViewDTO = new GenericViewDTO<TaskSimulation>();
                taskDetailSimulationViewDTO = new GenericViewDTO<TaskDetailSimulation>();
                selectedOrdersViewDTO = new GenericViewDTO<OutboundOrder>();
                InitializeSelectedRows();
                InitializeCheckedRows();

                InitializeSession();                              

                // Oculta detalle de las grillas
                HideDetails();
            }
        }

        /// <summary>
        /// Oculta detalles de las grillas
        /// </summary>
        protected void HideDetails()
        {
            currentIndex = -1;
            currentIndexSim = -1;
            currentIndexSelected = -1;

            divDetail.Visible = false;
            divDetailSim.Visible = false;
            divSelectedDetail.Visible = false;
        }

        protected void InitializeSession()
        {
            switch (dispatchType)
            {
                case "PIKOR":
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKOR.SelectedOrders);
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKOR.TaskSimulation);
                    break;

                case "PIKVA":
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKVA.SelectedOrders);
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKVA.TaskSimulation);
                    break;

                case "PIKIT":
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKIT.SelectedOrders);
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKIT.TaskSimulation);
                    break;

                case "PIUNK":
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIUNK.SelectedOrders);
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIUNK.TaskSimulation);
                    break;

                case "PIKWV":
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKWV.SelectedOrders);
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKWV.TaskSimulation);
                    break;

                case "PIKBT":
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKBT.SelectedOrders);
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKBT.TaskSimulation);
                    break;

                case "PIKPS":
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKPS.SelectedOrders);
                    Session.Remove(WMSTekSessions.ReleaseOrderMgr.PIKPS.TaskSimulation);
                    break;
            }
        }

        /// <summary>
        /// Obtiene los datos configurados para cada WorkZone y los salva en una lista de Task Details
        /// </summary>
        protected List<TaskDetail> GetWorkZonesConfiguration()
        {
            List<TaskDetail> taskDetails = new List<TaskDetail>();

            foreach (GridViewRow row in grdWorkZones.Rows)
            {
                if (((CheckBox)row.FindControl("chkSelectWorkZone")).Checked)
                {
                    TaskDetail detail = new TaskDetail();

                    // Busca los controles y rescata sus valores
                    detail.IdWorkZoneProposal = Convert.ToInt32(grdWorkZones.DataKeys[row.RowIndex].Value);
                    //detail.IdLocForkLiftProposal = ((DropDownList)row.FindControl("ddlForkLift")).SelectedValue;
                    //detail.UserAssigned = ((DropDownList)row.FindControl("ddlUser")).SelectedValue;
                    detail.IdLocTargetProposal = ((DropDownList)row.FindControl("ddlTargetLocation")).SelectedValue;

                    taskDetails.Add(detail);
                }
            }

            return taskDetails;
        }

        private List<String> getOutboundType()
        {
            List<String> lstResult = new List<string>();

            switch (dispatchType)
            {
                case "PIKOR":
                    lstResult = GetConst("OutboundOrderTypeForDispatchOR").ConvertAll(s=>s.ToUpper()).ToList();
                    break;

                case "PIKVA":
                    lstResult = GetConst("OutboundOrderTypeForDispatchVA").ConvertAll(s => s.ToUpper()).ToList();
                    break;

                case "PIKIT":
                    lstResult = GetConst("OutboundOrderTypeForDispatchKIT").ConvertAll(s => s.ToUpper()).ToList();
                    break;

                case "PIUNK":
                    lstResult = GetConst("OutboundOrderTypeForDispatchUNK").ConvertAll(s => s.ToUpper()).ToList();
                    break;

                case "PIKWV":
                    lstResult = GetConst("OutboundOrderTypeForDispatchWV").ConvertAll(s => s.ToUpper()).ToList();
                    break;

                case "PIKBT":
                    lstResult = GetConst("OutboundOrderTypeForDispatchBT").ConvertAll(s => s.ToUpper()).ToList();
                    break;

                case "PIKPS":
                    lstResult = GetConst("OutboundOrderTypeForDispatchPS").ConvertAll(s => s.ToUpper()).ToList();
                    break;
            }

            return lstResult;
        }


        private bool AllowBackOrder()
        {
            bool result = false;

            var bo = from a in context.CfgParameters
                     from b in constAllowBackOrder
                     where a.ParameterCode == b && a.ParameterValue == "1"
                     select new
                     {
                         paramCode = a.ParameterCode,
                         paramValue = a.ParameterValue
                     };

            if (bo.Count() > 0)
            {
                result = true;
            }

            return result;
        }

        private bool AllowCrossDock()
        {
            bool result = false;

            var bo = from a in context.CfgParameters
                     from b in constAllowCrossDock
                     where a.ParameterCode == b && a.ParameterValue == "1"
                     select new
                     {
                         paramCode = a.ParameterCode,
                         paramValue = a.ParameterValue
                     };

            if (bo.Count() > 0)
            {
                result = true;
            }

            return result;
        }

        private bool ReleaseOrderWillBeQueued()
        {
            bool willBeQueued = false;

            var value = context.CfgParameters.Where(param => param.ParameterCode == "ReleaseOrderWillBeQueued").FirstOrDefault();

            if (value != null)
            {
                if (value.ParameterValue == "1")
                    willBeQueued = true;
            }

            return willBeQueued;
        }
        private bool SimulateOrderWillBeQueued( string parameterCode)
        {
            bool willBeQueued = false;

            var value = context.CfgParameters.Where(param => param.ParameterCode == parameterCode).FirstOrDefault();

            if (value != null)
            {
                if (value.ParameterValue == "1")
                    willBeQueued = true;
            }

            return willBeQueued;
        }
        private bool ReleaseBatchWillBeQueued()
        {
            bool willBeQueued = false;

            var value = context.CfgParameters.Where(param => param.ParameterCode == "ReleaseBatchWillBeQueued").FirstOrDefault();

            if (value != null)
            {
                if (value.ParameterValue == "1")
                    willBeQueued = true;
            }

            return willBeQueued;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('OutboundOrder_GetByTrackFilter', 'ctl00_MainContent_grdMgr', 'ReleaseOrderMgr');", true);
        }
        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('OutboundOrderDetail_ById_ItemStock', 'ctl00_MainContent_grdDetail', 'ReleaseOrderMgr');", true);
            CallJsGridView();
        }
        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "initializeGridWithNoDragAndDrop(true);", true);
        }
        private void CallJsSelectedGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridSelected", "initializeGridDragAndDrop('grdSelected_ReleaseOrder', 'ctl00_MainContent_hsVertical_leftPanel_ctl01_grdSelected');", true);
            CallJsGridView();
        }
        private void CallJsSelectedDetailGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grdSelectedDetail", "initializeGridDragAndDrop('OutboundOrderDetail_ById_ItemStock', 'ctl00_MainContent_hsVertical_leftPanel_ctl01_grdSelectedDetail', 'ReleaseOrderMgr');", true);
            CallJsGridView();
        }
        private void CallJsGridViewSimulateHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeaderSimulate", "initializeGridDragAndDrop('grdSimulate_ReleaseOrder', 'ctl00_MainContent_hsVertical_ctl00_ctl01_grdSimulate');", true);
            CallJsGridView();
        }
        private void CallJsGridViewSimulateDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetailSimulate", "initializeGridDragAndDrop('grdDetailSim_ReleaseOrder', 'ctl00_MainContent_hsVertical_ctl00_ctl01_grdDetailSim');", true);
            CallJsGridView();
        }
        protected void grdSimulate_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdDetailSim_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        private void ShowConsolidationOrdersPercentage()
        {
            var index = GetColumnIndex(grdSimulate.Columns, "ConsolidationOrdersPercentage");

            if (index > -1)
            {
                var column = grdSimulate.Columns[index];

                column.Visible = dispatchType == "PIKBT";
            }
        }



        #endregion

        
    }
}