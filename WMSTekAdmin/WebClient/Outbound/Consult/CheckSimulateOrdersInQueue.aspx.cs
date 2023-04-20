using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Base;
using System.ServiceModel.Channels;
using Binaria.WMSTek.Framework.Entities.Ptl;
using System.IO;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class CheckSimulateOrdersInQueue : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<TaskSimulation> taskSimulationViewDTO = new GenericViewDTO<TaskSimulation>();
        //private GenericViewDTO<OutboundOrder> selectedOrdersViewDTO = new GenericViewDTO<OutboundOrder>();
        //private TaskSimulation taskSimulationSelect = new TaskSimulation();
        private GenericViewDTO<TaskDetailSimulation> taskDetailSimulationViewDTO = new GenericViewDTO<TaskDetailSimulation>();
        private GenericViewDTO<Issue> issueViewDTO = new GenericViewDTO<Issue>();
        private bool isValidViewDTO = true;
        private GenericViewDTO<WorkZone> workZoneViewDTO = new GenericViewDTO<WorkZone>();
        private GenericViewDTO<OutboundOrder> outboundDetWaveViewDTO;
        private TaskSimulation taskSimulationSelect
        {
            get
            {
                if (ValidateSession("taskSimulationSelect"))
                    return (TaskSimulation)Session["taskSimulationSelect"];
                else
                    return new TaskSimulation();
            }

            set { Session["taskSimulationSelect"] = value; }
        }

        private GenericViewDTO<OutboundOrder> selectedOrdersViewDTO
        {
            get
            {
                if (ValidateSession("selectedOrdersViewDTO"))
                    return (GenericViewDTO<OutboundOrder>)Session["selectedOrdersViewDTO"];
                else
                    return new GenericViewDTO<OutboundOrder>();
            }

            set { Session["selectedOrdersViewDTO"] = value; }
        }

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

        public int currentPageCustomer
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

        public string dispatchType
        {
            get
            {
                if (ValidateViewState("dispatchType"))
                    return (string)ViewState["dispatchType"];
                else
                    return "";
            }

            set { ViewState["dispatchType"] = value; }
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
        #endregion

        #region "Eventos"
        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal)
                {
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
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
                        PopulateGridDetail();
                    }
                }

                //Page.Form.Attributes.Add("enctype", "multipart/form-data");
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var btnCancelTaskQueue = e.Row.FindControl("btnCancel") as ImageButton;

                    if (btnCancelTaskQueue != null)
                    {
                        btnCancelTaskQueue.OnClientClick = "if(confirm('" + lblConfirmCancel.Text + "')==false){return false;}";

                        if (taskSimulationViewDTO.Entities.Count > 0 && taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Waiting)
                        {
                            btnCancelTaskQueue.Visible = true;
                        }
                        else
                        {
                            btnCancelTaskQueue.Visible = false;
                        }
                    }

                    var btnRetryTaskQueue = e.Row.FindControl("btnEdit") as ImageButton;

                    if (btnRetryTaskQueue != null)
                    {
                        btnRetryTaskQueue.OnClientClick = "if(confirm('" + lblRetryCancel.Text + "')==false){return false;}";

                        if (taskSimulationViewDTO.Entities.Count > 0 && (taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Error
                            || taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Cancel)
                            || taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Successful)
                        {
                            btnRetryTaskQueue.Visible = true;
                        }
                        else
                        {
                            btnRetryTaskQueue.Visible = false;
                        }
                    }

                    var btnProcess = e.Row.FindControl("btnProcess") as ImageButton;
                    var chkRemoveOrder = e.Row.FindControl("chkRemoveOrder") as CheckBox;

                    if (btnProcess != null)
                    {     
                        if (taskSimulationViewDTO.Entities.Count > 0 && (taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Successful) &&
                            taskSimulationViewDTO.Entities[e.Row.DataItemIndex].CompliancePct > 0)
                        {
                            string message = string.Empty;

                            if (taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TypeCode == "PIKWV")
                            {
                                if(taskSimulationViewDTO.Entities[e.Row.DataItemIndex].IsPtl)
                                    message = lblProcessWavePtl.Text.Replace("[TASK]", taskSimulationViewDTO.Entities[e.Row.DataItemIndex].Id.ToString());
                                else
                                    message = lblProcessWave.Text.Replace("[TASK]", taskSimulationViewDTO.Entities[e.Row.DataItemIndex].Id.ToString());
                            }else if (taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TypeCode == "PIKBT")
                            {
                                message = lblProcessBatch.Text.Replace("[TASK]", taskSimulationViewDTO.Entities[e.Row.DataItemIndex].Id.ToString());
                            }
                            else
                            {
                                message = lblProcess.Text.Replace("[DOC]", taskSimulationViewDTO.Entities[e.Row.DataItemIndex].OutboundOrder.Number);
                            }

                            btnProcess.OnClientClick = "if(confirm('" + message + "')==false){return false;}";

                            btnProcess.Visible = true;
                            chkRemoveOrder.Enabled = true;
                        }
                        else
                        {
                            btnProcess.Visible = false;
                            chkRemoveOrder.Enabled = false;
                        }
                    }

                    var btnLink = e.Row.FindControl("btnLink") as ImageButton;

                    if (btnLink != null)
                    {
                        var pageurl = string.Empty;
                        if (taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TypeCode == "PIKWV")
                        {
                            pageurl = "CheckOrdersWaveInQueue.aspx?WAVE=" + taskSimulationViewDTO.Entities[e.Row.DataItemIndex].OutboundOrder.Number;
                            pageurl = pageurl + "&WHS=" + taskSimulationViewDTO.Entities[e.Row.DataItemIndex].OutboundOrder.Warehouse.Id;
                            pageurl = pageurl + "&OWN=" + taskSimulationViewDTO.Entities[e.Row.DataItemIndex].OutboundOrder.Owner.Id;
                            if(taskSimulationViewDTO.Entities[e.Row.DataItemIndex].IsPtl)
                                btnLink.ToolTip = "Consultar Estado Ola Ptl en Cola";
                            else
                                btnLink.ToolTip = "Consultar Estado Ola en Cola";
                        }
                        else if(taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TypeCode == "PIKBT")
                        {
                            pageurl = "CheckBatchesInQueue.aspx?DOC=" + taskSimulationViewDTO.Entities[e.Row.DataItemIndex].OutboundOrder.Number;
                            pageurl = pageurl + "&WHS=" + taskSimulationViewDTO.Entities[e.Row.DataItemIndex].OutboundOrder.Warehouse.Id;
                            pageurl = pageurl + "&OWN=" + taskSimulationViewDTO.Entities[e.Row.DataItemIndex].OutboundOrder.Owner.Id;
                            btnLink.ToolTip = "Consultar Estado Batch en Cola";
                        }else
                        {
                            pageurl = "CheckOrdersInQueue.aspx?DOC=" + taskSimulationViewDTO.Entities[e.Row.DataItemIndex].OutboundOrder.Number;
                            pageurl = pageurl + "&WHS=" + taskSimulationViewDTO.Entities[e.Row.DataItemIndex].OutboundOrder.Warehouse.Id;
                            pageurl = pageurl + "&OWN=" + taskSimulationViewDTO.Entities[e.Row.DataItemIndex].OutboundOrder.Owner.Id;
                            btnLink.ToolTip = "Consultar Estado Doc en Cola";
                        }

                        btnLink.OnClientClick = "window.open('" + pageurl + "'); return false;";

                        if (taskSimulationViewDTO.Entities.Count > 0 && (taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Release))
                        {
                            btnLink.Visible = true;
                        }
                        else
                        {
                            btnLink.Visible = false;
                        }
                    }

                    var btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null)
                    {
                        string message = lblDelete.Text.Replace("[TASK]", taskSimulationViewDTO.Entities[e.Row.DataItemIndex].Id.ToString());
                        btnDelete.OnClientClick = "if(confirm('" + message + "')==false){return false;}";

                        if (taskSimulationViewDTO.Entities.Count > 0 && (taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue != (int)eTrackTaskQueue.InProcess && taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue != (int)eTrackTaskQueue.Waiting))
                        {
                            btnDelete.Visible = true;
                            chkRemoveOrder.Enabled = true;
                        }
                        else
                        {
                            btnDelete.Visible = false;
                            chkRemoveOrder.Enabled = false;
                        }
                    }

                    var btnWaveDetail = e.Row.FindControl("btnWaveDetail") as ImageButton;
                    if (btnWaveDetail != null)
                    {
                        if (taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TypeCode == "PIKWV")
                        {
                            btnWaveDetail.ToolTip = "Detalle Pedidos Ola";
                            btnWaveDetail.Visible = true;
                        }
                        else if(taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TypeCode == "PIKBT")
                        {
                            btnWaveDetail.ToolTip = "Detalle Pedidos Batch";
                            btnWaveDetail.Visible = true;
                        }
                        else { 
                            btnWaveDetail.Visible = false;
                        }
                    }

                    var btnExcel = e.Row.FindControl("btnExcelDetail") as ImageButton;

                    if (btnExcel != null)
                    {
                        //btnRetryTaskQueue.OnClientClick = "if(confirm('" + lblRetryCancel.Text + "')==false){return false;}";

                        if (taskSimulationViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Successful)
                        {
                            btnExcel.Visible = true;
                        }
                        else
                        {
                            btnExcel.Visible = false;
                        }
                    }

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions") || i == GetColumnIndexByName(e.Row, "CheckBox"))
                        {
                            continue;
                        }

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                        // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                        //e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);

                    }
                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Liberar")
                {
                    fullStock = true;
                    int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);

                    taskSimulationSelect = taskSimulationViewDTO.Entities[index];
                    selectedOrdersViewDTO.Entities = new List<OutboundOrder>();
                    selectedOrdersViewDTO.Entities.Add(taskSimulationSelect.OutboundOrder);
                    Session.Add("TareasSeleccionadasSimulacion", null);

                    currentWhs = taskSimulationSelect.OutboundOrder.Warehouse.Id;
                    dispatchType = taskSimulationSelect.TypeCode;

                    string errorCustomRule = string.Empty;

                    // Valida variable de sesion del Usuario Loggeado
                    if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                    {
                        // Si no hay stock suficiente para las Ordenes seleccionadas con el campo 'FullShipment' activo, no se permite continuar
                        if (!ValidateFullShipment(taskSimulationSelect))
                        {
                            Master.ucDialog.ShowAlert(lblTitle.Text, lblNoStockFullShipment.Text, string.Empty);
                        }

                        //Rescata Tarea y Detalles
                        var taskDet = iDispatchingMGR.GetTasksSimulationWihtDetail(taskSimulationSelect.Id, context);

                        if (taskDet.hasError())
                        {
                            Master.ucError.ShowError(taskDet.Errors);
                        }
                        else
                        {
                            taskSimulationSelect = taskDet.Entities[0];

                            errorCustomRule = ValidateCustomRuleItem(taskSimulationSelect, dispatchType);

                            if (!string.IsNullOrEmpty(errorCustomRule))
                            {
                                Master.ucDialog.ShowAlert(lblTitle.Text, errorCustomRule.Trim(), string.Empty);
                            }
                            else
                            {
                                if (validateTaskRelease() || validateOrdersInOtherTask(taskSimulationSelect))
                                {
                                    string error = this.lblSimulateRelease.Text.Replace("[OUTBOUND]", taskSimulationSelect.OutboundOrder.Number);
                                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, ""); 

                                }else if(validateOrdersRelease(taskSimulationSelect))
                                {
                                    string error = this.lblOrdersRelease.Text;
                                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, "");
                                }
                                else
                                {
                                    // Carga ubicaciones de Packing
                                    ddlLocStageTarget.SelectedIndex = -1;
                                    base.LoadStagingLocations(ddlLocStageTarget, currentWhs, Master.EmptyRowText);

                                    //Nuevas Ubicaciones de Anden y Embalaje
                                    ddlLocStageDispatch.SelectedIndex = -1;
                                    ddlLocDock.SelectedIndex = -1;
                                    base.LoadLocationsByWhsAndType(ddlLocStageDispatch, currentWhs, LocationTypeName.STGD.ToString(), Master.EmptyRowText, true);
                                    base.LoadLocationsByWhsAndType(ddlLocDock, currentWhs, LocationTypeName.DOCK.ToString(), Master.EmptyRowText, true);

                                    // Valida condiciones para poder Liberar los pedidos
                                    if (OrderInSumulation(taskSimulationSelect) || !ValidateStock(taskSimulationSelect))
                                    {
                                        // Si no hay stock suficiente para todas las Ordenes seleccionadas, se muestra un mensaje de advertencia
                                        if (!ValidateStock(taskSimulationSelect) && !OrderInSumulation(taskSimulationSelect))
                                        {
                                            fullStock = false;
                                            Master.ucDialog.ShowConfirm(lblTitle.Text, lblNoStock.Text, "release");
                                        }

                                        // Si alguna de las Ordenes seleccionadas ya se encuentra en otro proceso de Simulación, se muestra un mensaje de advertencia
                                        if (ValidateStock(taskSimulationSelect) && OrderInSumulation(taskSimulationSelect))
                                        {
                                            Master.ucDialog.ShowConfirm(lblTitle.Text, lblOrderInSimulation.Text.Replace("[DOC]", taskSimulationSelect.OutboundOrder.Number), "release");
                                        }

                                        // Si no hay stock suficiente Y alguna de las Ordenes se encuentra en otro proceso, muestra los dos mensajes de advertencia
                                        if (!ValidateStock(taskSimulationSelect) && OrderInSumulation(taskSimulationSelect))
                                        {
                                            fullStock = false;
                                            Master.ucDialog.ShowConfirm(lblTitle.Text, lblNoStock.Text, "alertOrderInSimulation");
                                        }
                                    }
                                    else
                                    {
                                        if (taskSimulationSelect.TypeCode == "PIKWV" || taskSimulationSelect.TypeCode == "PIKBT")
                                        {
                                            this.lblNroDocPopUp.Text = lblDescReleaseWave.Text.Replace("[TASK]", taskSimulationSelect.Id.ToString()).Replace("[TYPE]", taskSimulationSelect.TypeCode);
                                        }
                                        else
                                        {
                                            this.lblNroDocPopUp.Text = lblDescReleaseDoc.Text.Replace("[TASK]", taskSimulationSelect.Id.ToString()).Replace("[TYPE]", taskSimulationSelect.TypeCode).Replace("[DOC]", taskSimulationSelect.OutboundOrder.Number);
                                        }

                                        ShowReleaseOrdersPopUp();
                                    }
                                }
                            }
                        }
                    }
                }
                if (e.CommandName == "Eliminar")
                {
                    int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);

                    taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.CheckOrdersSimulationInQueue.List];
                    var taskSimulationQueueToCancel = taskSimulationViewDTO.Entities[index];

                    if (taskSimulationQueueToCancel.TrackTaskQueue.IdTrackTaskQueue != (int)eTrackTaskQueue.InProcess || taskSimulationQueueToCancel.TrackTaskQueue.IdTrackTaskQueue != (int)eTrackTaskQueue.Release)
                    {
                    
                            var deleteTaskQueueViewDTO = iDispatchingMGR.DeleteTaskSimulationQueue(taskSimulationQueueToCancel.Id, context);

                        if (deleteTaskQueueViewDTO.hasError())
                        {
                            Master.ucError.ShowError(deleteTaskQueueViewDTO.Errors);
                        }
                        else
                        {
                            UpdateSession(false);
                            ucStatus.ShowMessage(deleteTaskQueueViewDTO.MessageStatus.Message);
                        }
                    }
                }

                if (e.CommandName == "DetalleOla")
                {
                    //Mostrar Detalle de Documentos asociados a una OLA
                    int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);
                    taskSimulationSelect = taskSimulationViewDTO.Entities[index];

                    if (taskSimulationSelect.TypeCode == "PIKBT")
                    {
                        this.lblDetWaveBatch.Text = "Documentos del Batch";
                    }
                    else {
                        this.lblDetWaveBatch.Text = "Documentos de la Ola";
                            }

                    Dictionary<string, string> subQueryParams = new Dictionary<string, string>();
                    subQueryParams.Add("SubQueryCode", "DocumentsInWaveTaskSimulation");
                    subQueryParams.Add("idTask", taskSimulationSelect.Id.ToString());

                    GenericViewDTO<OutboundOrder> outboundOrderWaveViewDTO = iDispatchingMGR.FindAllOutboundOrder(new ContextViewDTO(), subQueryParams);

                    Session.Add("SessionListOutboundDetWave", outboundOrderWaveViewDTO);
                    grdWaveOrders.DataSource = outboundOrderWaveViewDTO.Entities;
                    grdWaveOrders.DataBind();

                    divWaveDetail.Visible = true;
                    mpWaveDetail.Show();

                    InitializePageCountCustomer();
                    //ShowCustomerButtonsPage();
                }                

                divTitleDet.Visible = false;
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        protected void btnReprocess_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    bool chkTasks = false;
                    if (taskSimulationViewDTO != null)
                    {
                        GenericViewDTO<TaskSimulation> newListTask = new GenericViewDTO<TaskSimulation>();
                        for (int i = 0; i < taskSimulationViewDTO.Entities.Count; i++)
                        {
                            GridViewRow row = grdMgr.Rows[i];

                            if (((CheckBox)row.FindControl("chkRemoveOrder")).Checked)
                            {
                                chkTasks = true;
                                var taskSelect = taskSimulationViewDTO.Entities[i];
                                if (taskSelect.CompliancePct > 0 && taskSelect.TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Successful)
                                {
                                    newListTask.Entities.Add(taskSelect);
                                }
                            }
                        }                        

                        if (chkTasks)
                        {
                            //Valida mismo tipo de tarea
                            var distinctTaskType = newListTask.Entities.Select(s => s.TypeCode).Distinct();

                            if (distinctTaskType.Count() == 0)
                            {
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblTaskTypeSuccessful.Text, string.Empty); 
                            }
                            else if (distinctTaskType.Count() == 1)
                            {
                                Session.Add("TareasSeleccionadasSimulacion", newListTask);

                                foreach (var item in newListTask.Entities)
                                {
                                    currentWhs = item.OutboundOrder.Warehouse.Id;
                                    dispatchType = item.TypeCode;

                                    // Si no hay stock suficiente para las Ordenes seleccionadas con el campo 'FullShipment' activo, no se permite continuar
                                    if (!ValidateFullShipment(item))
                                    {
                                        Master.ucDialog.ShowAlert(lblTitle.Text, lblNoStockFullShipment.Text, string.Empty);
                                        break;
                                    }

                                    //Rescata Tarea y Detalles
                                    var taskDet = iDispatchingMGR.GetTasksSimulationWihtDetail(item.Id, context);

                                    if (taskDet.hasError())
                                    {
                                        //Master.ucError.ShowError(taskDet.Errors);
                                        Master.ucDialog.ShowAlert(lblTitle.Text, lblTaskWhitoutDetail.Text.Replace("[TASK]", item.Id.ToString()), string.Empty);
                                        break;
                                    }
                                    else
                                    {
                                        taskSimulationSelect = taskDet.Entities[0];

                                        var errorCustomRule = ValidateCustomRuleItem(taskSimulationSelect, dispatchType);

                                        if (!string.IsNullOrEmpty(errorCustomRule))
                                        {
                                            Master.ucDialog.ShowAlert(lblTitle.Text, errorCustomRule.Trim(), string.Empty);
                                            break;
                                        }
                                        else
                                        {
                                            if (validateTaskRelease() || validateOrdersInOtherTask(taskSimulationSelect))
                                            {
                                                string error = this.lblSimulateRelease.Text.Replace("[OUTBOUND]", taskSimulationSelect.OutboundOrder.Number);
                                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, ""); ;
                                            }
                                            else if (validateOrdersRelease(taskSimulationSelect))
                                            {
                                                string error = this.lblOrdersRelease.Text;
                                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, "");
                                            }
                                            else
                                            {
                                                // Carga ubicaciones de Packing
                                                ddlLocStageTarget.SelectedIndex = -1;
                                                base.LoadStagingLocations(ddlLocStageTarget, currentWhs, Master.EmptyRowText);

                                                //Nuevas Ubicaciones de Anden y Embalaje
                                                ddlLocStageDispatch.SelectedIndex = -1;
                                                ddlLocDock.SelectedIndex = -1;
                                                base.LoadLocationsByWhsAndType(ddlLocStageDispatch, currentWhs, LocationTypeName.STGD.ToString(), Master.EmptyRowText, true);
                                                base.LoadLocationsByWhsAndType(ddlLocDock, currentWhs, LocationTypeName.DOCK.ToString(), Master.EmptyRowText, true);

                                                // Valida condiciones para poder Liberar los pedidos
                                                if (OrderInSumulation(taskSimulationSelect) || !ValidateStock(taskSimulationSelect))
                                                {
                                                    // Si no hay stock suficiente para todas las Ordenes seleccionadas, se muestra un mensaje de advertencia
                                                    if (!ValidateStock(taskSimulationSelect) && !OrderInSumulation(taskSimulationSelect))
                                                    {
                                                        fullStock = false;
                                                        Master.ucDialog.ShowConfirm(lblTitle.Text, lblNoStock.Text, "release");
                                                        break;
                                                    }

                                                    // Si alguna de las Ordenes seleccionadas ya se encuentra en otro proceso de Simulación, se muestra un mensaje de advertencia
                                                    if (ValidateStock(taskSimulationSelect) && OrderInSumulation(taskSimulationSelect))
                                                    {
                                                        Master.ucDialog.ShowConfirm(lblTitle.Text, lblOrderInSimulation.Text.Replace("[DOC]", item.OutboundOrder.Number), "release");
                                                        break;
                                                    }

                                                    // Si no hay stock suficiente Y alguna de las Ordenes se encuentra en otro proceso, muestra los dos mensajes de advertencia
                                                    if (!ValidateStock(taskSimulationSelect) && OrderInSumulation(taskSimulationSelect))
                                                    {
                                                        fullStock = false;
                                                        Master.ucDialog.ShowConfirm(lblTitle.Text, lblNoStock.Text, "alertOrderInSimulation");
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (item.TypeCode == "PIKWV" || item.TypeCode == "PIKBT")
                                                    {
                                                        if (newListTask.Entities.Count > 1)
                                                            this.lblNroDocPopUp.Text = lblDescReleaseWaves.Text.Replace("[TYPE]", item.TypeCode);
                                                        else
                                                            this.lblNroDocPopUp.Text = lblDescReleaseWave.Text.Replace("[TASK]", item.Id.ToString()).Replace("[TYPE]", item.TypeCode);
                                                    }
                                                    else
                                                    {
                                                        if (newListTask.Entities.Count > 1)
                                                            this.lblNroDocPopUp.Text = lblDescReleaseWaves.Text.Replace("[TYPE]", item.TypeCode);
                                                        else
                                                            this.lblNroDocPopUp.Text = lblDescReleaseDoc.Text.Replace("[TASK]", item.Id.ToString()).Replace("[TYPE]", item.TypeCode).Replace("[DOC]", item.OutboundOrder.Number);

                                                    }

                                                    ShowReleaseOrdersPopUp();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblDistinctTaskType.Text, string.Empty);
                            }
                        }
                        else
                        {
                            this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblTaskNoSelected.Text, string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        protected void btnRemove_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    bool chkTasks = false;
                    if (taskSimulationViewDTO != null)
                    {
                        GenericViewDTO<TaskSimulation> newListTask = new GenericViewDTO<TaskSimulation>();
                        for (int i = 0; i < taskSimulationViewDTO.Entities.Count; i++)
                        {
                            GridViewRow row = grdMgr.Rows[i];

                            if (((CheckBox)row.FindControl("chkRemoveOrder")).Checked)
                            {
                                chkTasks = true;
                                var taskSelect = taskSimulationViewDTO.Entities[i];
                                if (taskSelect.TrackTaskQueue.IdTrackTaskQueue != (int)eTrackTaskQueue.InProcess &&
                                    taskSelect.TrackTaskQueue.IdTrackTaskQueue != (int)eTrackTaskQueue.Waiting)
                                {
                                    newListTask.Entities.Add(taskSelect);
                                }
                            }
                        }

                        if (chkTasks)
                        {

                            if (newListTask.Entities.Count() > 0)
                            {
                                foreach (var delTask in newListTask.Entities)
                                {
                                    //Session.Add("TareasSeleccionadasSimulacion", newListTask);
                                    var deleteTaskQueueViewDTO = iDispatchingMGR.DeleteTaskSimulationQueue(delTask.Id, context);

                                    if (deleteTaskQueueViewDTO.hasError())
                                    {
                                        Master.ucError.ShowError(deleteTaskQueueViewDTO.Errors);
                                    }
                                    else
                                    {
                                        UpdateSession(false);
                                        ucStatus.ShowMessage(deleteTaskQueueViewDTO.MessageStatus.Message);
                                    }
                                }
                            }
                            else
                            {
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblDistinctTaskType.Text, string.Empty);
                            }
                        }
                        else
                        {
                            this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblTaskNoSelected.Text, string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }
        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndex = grdMgr.SelectedIndex;

                    LoadDetail(currentIndex);
                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                    e.Row.TableSection = TableRowSection.TableHeader;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var taskSimulationQueue = (TaskSimulation)e.Row.DataItem;

                    string cssClass = null;

                    switch ((eTrackTaskQueue)taskSimulationQueue.TrackTaskQueue.IdTrackTaskQueue)
                    {
                        case eTrackTaskQueue.InProcess:
                            cssClass = "blueAlert";
                            break;
                        case eTrackTaskQueue.Successful:
                            cssClass = "greenAlert";
                            break;
                        case eTrackTaskQueue.Error:
                            cssClass = "redAlert";
                            break;
                        case eTrackTaskQueue.Cancel:
                            cssClass = "yellowAlert";
                            break;
                        case eTrackTaskQueue.Release:
                            cssClass = "successAlert";
                            break;
                        default:
                            break;
                    }

                    if (!string.IsNullOrEmpty(cssClass))
                        e.Row.CssClass = cssClass;
                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                int retryIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;
                taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.CheckOrdersSimulationInQueue.List];
                var taskQueueSimulationToRetry = taskSimulationViewDTO.Entities[retryIndex];

                //taskQueueSimulationToRetry.TrackTaskQueue.IdTrackTaskQueue = (int)eTrackTaskQueue.Waiting;

                //Falta crear el update del TaskSimulation
                var retryTaskQueueViewDTO = iDispatchingMGR.UpdateTrackTaskQueue(taskQueueSimulationToRetry.Id, (int)eTrackTaskQueue.Waiting, context);

                if (retryTaskQueueViewDTO.hasError())
                {
                    Master.ucError.ShowError(retryTaskQueueViewDTO.Errors);
                }
                else
                {
                    UpdateSession(false);
                    ucStatus.ShowMessage(retryTaskQueueViewDTO.MessageStatus.Message);
                }
                divTitleDet.Visible = false;
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int cancelIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;
                taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.CheckOrdersSimulationInQueue.List];
                var taskSimulationQueueToCancel = taskSimulationViewDTO.Entities[cancelIndex];

                //taskSimulationQueueToCancel.TrackTaskQueue.IdTrackTaskQueue = (int)eTrackTaskQueue.Cancel;

                if(taskSimulationQueueToCancel.TrackTaskQueue.IdTrackTaskQueue != (int)eTrackTaskQueue.InProcess || taskSimulationQueueToCancel.TrackTaskQueue.IdTrackTaskQueue != (int)eTrackTaskQueue.Release)
                { 
                    var cancelTaskQueueViewDTO = iDispatchingMGR.UpdateTrackTaskQueue(taskSimulationQueueToCancel.Id, (int)eTrackTaskQueue.Cancel, context);

                    if (cancelTaskQueueViewDTO.hasError())
                    {
                        Master.ucError.ShowError(cancelTaskQueueViewDTO.Errors);
                    }
                    else
                    {
                        UpdateSession(false);
                        ucStatus.ShowMessage(cancelTaskQueueViewDTO.MessageStatus.Message);
                    }

                    divTitleDet.Visible = false;
                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }


        /// <summary>
        /// Respuesta desde la ventana de diálogo
        /// </summary>
        protected void btnDialogOk_Click(object sender, EventArgs e)
        {
            try
            {
                switch (Master.ucDialog.Caller)
                {
                    case "release":
                        ShowReleaseOrdersPopUp();
                        break;
                    //case "getTaskSimulation":
                    //    //PopulateSimulateGrid(false, true, false, true);
                    //    //AddToSelectedOrdersFromTaskList();
                    //    break;
                    case "alertOrderInSimulation":
                        // Si alguna de las Ordenes seleccionadas ya se encuentra en otro proceso de Simulación, se muestra un mensaje de advertencia
                        if (OrderInSumulation(taskSimulationSelect))
                            Master.ucDialog.ShowConfirm(lblTitle.Text, lblOrderInSimulation.Text, "release");
                        else
                            ShowReleaseOrdersPopUp();
                        break;                    
                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
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
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
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
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
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
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
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
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
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
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
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
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskSimulationViewDTO.Errors);
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
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }



        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    { 
                        //e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    }
                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }
        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
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


        /// <summary>
        /// Valida que la Orden seleccionada no se encuentre en otro proceso de Simulación
        /// </summary>
        private bool OrderInSumulation(TaskSimulation taskSimulation)
        {
            if (taskSimulation.OutboundOrder.InOtherSimulation)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Valida que haya Stock suficiente para las Ordenes seleccionadas con 'FullShipment' activo
        /// </summary>
        private bool ValidateFullShipment(TaskSimulation taskSimulation)
        {
            if (taskSimulation.CompliancePct < 100 && taskSimulation.OutboundOrder.FullShipment)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida que los items contengan reglas asociadas
        /// </summary>
        private string ValidateCustomRuleItem(TaskSimulation taskSim, string dispatchType)
        {
            string mensaje = string.Empty;

            foreach (TaskDetailSimulation detail in taskSim.Details.Entities)
            {
                if (detail.ExistItemCustomRules == 0)
                {
                    if (dispatchType == "PIKWV")
                    {
                        mensaje = lblNotExistCustomRuleItemsWave.Text;
                        mensaje = mensaje.Replace("[ITEM]", detail.OutboundDetail.Item.Code);
                    }
                    else
                    {
                        mensaje = lblNotExistCustomRuleItems.Text;
                        mensaje = mensaje.Replace("[DOC]", taskSim.OutboundOrder.Number).Replace("[ITEM]", detail.OutboundDetail.Item.Code);
                    }
                    return mensaje;
                }
            }

            return mensaje;
        }

        /// <summary>
        /// Valida que haya Stock suficiente para las Ordenes seleccionadas
        /// </summary>
        private bool ValidateStock(TaskSimulation taskSimulation)
        {
            if (taskSimulation.CompliancePct < 100)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Muestra opciones para configurar la Liberación de Pedidos 
        /// </summary>
        protected void ShowReleaseOrdersPopUp()
        {
            this.divPtlType.Visible = false;

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
                    rfvLocStageDispatch.Enabled = false;
                    rfvLocDock.Enabled = false;
                    divSorting.Visible = false;
                    lblLocStageDispatch.Text = lblTitleLocDispatch.Text;
                    rfvLocStageDispatch.ErrorMessage = lblTitleLocDispatch.Text + lblRequiredField.Text;                    

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
                    rfvLocStageDispatch.Enabled = true;
                    rfvLocDock.Enabled = false;
                    divSorting.Visible = false;
                    lblLocStageDispatch.Text = lblTitleLocDispatch.Text;
                    rfvLocStageDispatch.ErrorMessage = lblTitleLocDispatch.Text + lblRequiredField.Text;
                    
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
                    rfvLocStageDispatch.Enabled = true;
                    rfvLocDock.Enabled = false;
                    divSorting.Visible = false;
                    lblLocStageDispatch.Text = lblTitleLocDispatch.Text;
                    rfvLocStageDispatch.ErrorMessage = lblTitleLocDispatch.Text + lblRequiredField.Text;
                    
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
                    rfvLocStageDispatch.Enabled = true;
                    rfvLocDock.Enabled = false;
                    divSorting.Visible = false;
                    lblLocStageDispatch.Text = lblTitleLocDispatch.Text;
                    rfvLocStageDispatch.ErrorMessage = lblTitleLocDispatch.Text + lblRequiredField.Text;
                    
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
                    rfvLocStageDispatch.Enabled = true;
                    rfvLocDock.Enabled = false;

                    chkSorting.Checked = (GetCfgParameter(CfgParameterName.AllowCheckPacking.ToString()) == "1" ? true : false);
                    divSorting.Visible = true;
                    lblLocStageDispatch.Text = lblTitleLocSorting.Text;
                    rfvLocStageDispatch.ErrorMessage = lblTitleLocSorting.Text + lblRequiredField.Text;

                    if (taskSimulationSelect.IsPtl)
                    {
                        this.divPtlType.Visible = true;
                        this.LoadPtlTypes(this.ddlPtlType, true, this.Master.EmptyRowText);
                    }

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
                    rfvLocStageDispatch.Enabled = false;
                    rfvLocDock.Enabled = false;
                    divSorting.Visible = false;
                    lblLocStageDispatch.Text = lblTitleLocDispatch.Text;
                    rfvLocStageDispatch.ErrorMessage = lblTitleLocDispatch.Text + lblRequiredField.Text;
                    
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
                    rfvLocStageDispatch.Enabled = false;
                    rfvLocDock.Enabled = false;
                    divSorting.Visible = false;
                    lblLocStageDispatch.Text = lblTitleLocDispatch.Text;
                    rfvLocStageDispatch.ErrorMessage = lblTitleLocDispatch.Text + lblRequiredField.Text;
                    
                    break;
            }

            ddlLocStageTarget.SelectedIndex = 0;
            divReleaseDispatch.Visible = true;
            mpReleaseDispatch.Show();
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
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
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

                    if (ddlForkLift != null) base.LoadForkLiftLocationsInWorkZone(ddlForkLift, idWorkZone, currentWhs, Master.EmptyRowText);
                    if (ddlUser != null) base.LoadUsersInWorkZone(ddlUser, idWorkZone, Master.EmptyRowText);
                    if (ddlTargetLocation != null) base.LoadStagingLocations(ddlTargetLocation, currentWhs, Master.EmptyRowText);
                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        private void PopulateWorkZoneGrid(bool refresh)
        {
            if (refresh)
            {
                // string dispatchType = "";
                //workZoneViewDTO = iDispatchingMGR.GetWorkZonesInSimulation(currentWhs, selectedOrdersViewDTO, dispatchType, context);
                workZoneViewDTO = iDispatchingMGR.GetWorkZonesInSimulation(currentWhs, new GenericViewDTO<OutboundOrder>(), dispatchType, context);
                Session[WMSTekSessions.ReleaseOrderMgr.PIKPS.SelectedWorkZones] = workZoneViewDTO;
            }

            if (!workZoneViewDTO.hasError() && workZoneViewDTO.Entities != null)
            {
                grdWorkZones.EmptyDataText = Master.EmptyGridText;

                grdWorkZones.DataSource = workZoneViewDTO.Entities;
                grdWorkZones.DataBind();

                CallJsGridView();
            }
            else
            {
                if (workZoneViewDTO.hasError())
                {
                    Master.ucError.ShowError(workZoneViewDTO.Errors);
                    workZoneViewDTO.ClearError();
                }
            }
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "initializeGridWithNoDragAndDrop(true);", true);
        }
        #endregion

        #region "Métodos"
        protected void Initialize()
        {
            context.SessionInfo.IdPage = "CheckSimulateOrdersInQueue";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeGridDetail();
            InitializeGridWaveOrders();

            this.Master.ucDialog.BtnOkClick += new EventHandler(btnDialogOk_Click);

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .35);
            }
            else
            {
                if (ValidateSession(WMSTekSessions.CheckOrdersSimulationInQueue.List))
                {
                    taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.CheckOrdersSimulationInQueue.List];
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
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            Master.ucTaskBar.btnExcelVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            //Tipos de Tareas
            this.Master.ucMainFilter.listTaskTypeCode = new List<string>();
            this.Master.ucMainFilter.listTaskTypeCode = GetConst("SimulateOrdersInQueue");

            Master.ucMainFilter.divTaskQueueFilterVisible = false;
            Master.ucMainFilter.taskQueueFilterSimulation = false;

            // Habilita criterios a usar
            Master.ucMainFilter.warehouseNotIncludeAll = true;
            Master.ucMainFilter.warehouseVisible = true;
            Master.ucMainFilter.ownerNotIncludeAll = false;
            Master.ucMainFilter.ownerVisible = true;
            Master.ucMainFilter.documentVisible = true;
            Master.ucMainFilter.taskTypeVisible = true;

            Master.ucMainFilter.codeVisible = true;
            Master.ucMainFilter.codeLabel = lblUser.Text;

            //Master.ucMainFilter.codeNumericVisible = false;
            //Master.ucMainFilter.codeNumericLabel = lblCompliancePct.Text;
            Master.ucMainFilter.setDateLabel = true;
            Master.ucMainFilter.dateVisible = true;
            Master.ucMainFilter.dateLabel = lblCreateDate.Text;

            context.MainFilter = Master.ucMainFilter.MainFilter;

            Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            Master.ucMainFilter.FilterOwnerAutoPostBack = false;
            Master.ucMainFilter.advancedFilterVisible = true;
            Master.ucMainFilter.tabMultipleChoiceTrackTaskFiltersVisible = true;
            Master.ucMainFilter.checkAdvancedFilter();

            Master.ucMainFilter.Initialize(init, refresh);
            Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
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
            grdMgr.EmptyDataText = Master.EmptyGridText;
        }
        private void InitializeGridDetail()
        {
            try
            {
                //grdDetail.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByDetailGridPage.ToString()));
                grdDetail.EmptyDataText = Master.EmptyGridText;
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        private void InitializeGridWaveOrders()
        {
            grdWaveOrders.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdWaveOrders.EmptyDataText = this.Master.EmptyGridText;
        }
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(taskSimulationViewDTO.Errors);
                taskSimulationViewDTO.ClearError();
            }

            context.MainFilter = Master.ucMainFilter.MainFilter;

            //ClearFilter("IdTypeTask");
            //CreateFilterByList("IdTypeTask", new List<int>() { (int)eTypeTaskQueue.ReleaseOrder });

            taskSimulationViewDTO = iDispatchingMGR.GetTasksInQueue(context);

            if (!taskSimulationViewDTO.hasError() && taskSimulationViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.CheckOrdersSimulationInQueue.List, taskSimulationViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(taskSimulationViewDTO.MessageStatus.Message);
            }
            else
            {
                if (taskSimulationViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                {
                    Master.ucError.ShowError(taskSimulationViewDTO.Errors);
                }
                else
                {
                    Session.Remove(WMSTekSessions.CheckOrdersSimulationInQueue.List);
                }
            }
        }
        protected void ReloadData()
        {
            UpdateSession(false);

            if (isValidViewDTO)
            {
                divTitleDet.Visible = false;
                divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
            }
        }
        protected void LoadDetail(int index)
        {
            if (index != -1)
            {
                taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.CheckOrdersSimulationInQueue.List];

                var task = taskSimulationViewDTO.Entities[index];

                var taskDetalle = iDispatchingMGR.GetTaskDetailSimulationWihtCalculation(task.Id, context);

                if (taskDetalle.Entities[0].Details == null)
                    taskDetalle.Entities[0].Details = new GenericViewDTO<TaskDetailSimulation>();

                var detalle = taskDetalle.Entities[0].Details;

                grdDetail.DataSource = detalle.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
                if (detalle.Entities.Count == 0)
                {
                    lblNroDoc.Text = String.Empty;
                    lblNroTask.Text = String.Empty;
                    divTitleDet.Visible = false;
                }
                else
                {
                    lblNroDoc.Text = task.OutboundOrder.Number;
                    lblNroTask.Text = task.Id.ToString();

                    if (string.IsNullOrEmpty(task.OutboundOrder.Number))
                        lblGridDetail.Visible = false;
                    else
                        lblGridDetail.Visible = true;

                    if (task.TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Successful)
                        btnExcelDetail.Visible = true;
                    else
                        btnExcelDetail.Visible = false;

                    divTitleDet.Visible = true;
                }

                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
            }
        }
        private void PopulateGrid()
        {
            taskSimulationViewDTO = (GenericViewDTO<TaskSimulation>)Session[WMSTekSessions.CheckOrdersSimulationInQueue.List];

            if (taskSimulationViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            if (!taskSimulationViewDTO.hasConfigurationError() && taskSimulationViewDTO.Configuration != null && taskSimulationViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, taskSimulationViewDTO.Configuration);

            grdMgr.DataSource = taskSimulationViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(taskSimulationViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            upGrid.Update();

            //CallJsGridViewHeader();
            //upGrid.Update();
        }
        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                //issueViewDTO = (GenericViewDTO<TaskDetailSimulation>)Session[WMSTekSessions.CheckOrdersSimulationInQueue.Detail];

                //if (!issueViewDTO.hasConfigurationError() && issueViewDTO.Configuration != null && issueViewDTO.Configuration.Count > 0)
                //    base.ConfigureGridOrder(grdDetail, issueViewDTO.Configuration);

                //grdDetail.DataSource = issueViewDTO.Entities;
                //grdDetail.DataBind();

                //CallJsGridViewDetail();
                //upGridDetail.Update();
            }
        }


        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('IssuesOnOrderDetail', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail', undefined, false);", true);
        }



        #endregion

        protected void btnRelease_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    CreateFilterTypeLocationUsedForStockAvailable();
                    var tareas = (GenericViewDTO<TaskSimulation>)Session["TareasSeleccionadasSimulacion"];
                    if (tareas == null)
                    {
                        tareas = new GenericViewDTO<TaskSimulation>();
                    }

                    if (dispatchType != "PIKPS")
                    {
                        if (ddlLocStageDispatch.SelectedValue == "-1" && ddlLocDock.SelectedValue == "-1")
                        {
                            rfvLocStageTarget.IsValid = false;
                            rfvLocStageTarget.ErrorMessage = this.lblMsgErrorUbic.Text;
                            this.divReleaseDispatch.Visible = true;
                            this.mpReleaseDispatch.Show();
                        }
                        else
                        {
                            if (tareas.Entities != null && tareas.Entities.Count > 0)
                            {
                                ReleaseOrdersChecks();
                            }
                            else
                            {
                                if (validateTaskRelease() || validateOrdersInOtherTask(taskSimulationSelect))
                                {
                                    string error = this.lblSimulateRelease.Text.Replace("[OUTBOUND]", taskSimulationSelect.OutboundOrder.Number);
                                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, ""); ;
                                }
                                else if (validateOrdersRelease(taskSimulationSelect))
                                {
                                    string error = this.lblOrdersRelease.Text;
                                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, "");
                                }
                                else
                                {
                                    ReleaseOrders();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tareas.Entities != null && tareas.Entities.Count > 0)
                        {
                            ReleaseOrdersChecks();
                        }
                        else
                        {

                            if (validateTaskRelease() || validateOrdersInOtherTask(taskSimulationSelect))
                            {
                                string error = this.lblSimulateRelease.Text.Replace("[OUTBOUND]", taskSimulationSelect.OutboundOrder.Number);
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, ""); ;
                            }
                            else if (validateOrdersRelease(taskSimulationSelect))
                            {
                                string error = this.lblOrdersRelease.Text;
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, "");
                            }
                            else
                            {
                                ReleaseOrders();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        //protected void HideDetails()
        //{
        //    currentIndex = -1;
        //    currentIndexSim = -1;
        //    currentIndexSelected = -1;

        //    divDetail.Visible = false;
        //    divDetailSim.Visible = false;
        //    divSelectedDetail.Visible = false;
        //}

        /// <summary>
        /// Libera las Ordenes seleccionadas, basado en la prioridad configurada en la Simulación
        /// </summary>
        protected void ReleaseOrders()
        {
            Task taskInfo = new Task();
            Task kittingTaskInfo = new Task();
            Task vasTaskInfo = new Task();

            // Información general de todas las Tareas a generar
            if (divUserNbr.Visible)
                taskInfo.WorkersRequired = Convert.ToInt16(txtUserNbr.Text);
            else
                taskInfo.WorkersRequired = 1;

            taskInfo.WorkersAssigned = 0;

            if (dispatchType == "PIKPS")
            {
                taskInfo.Priority = Convert.ToInt16(txtPriorityPickAndPass.Text);
                taskInfo.StageTarget = new Location(ddlLocStageTarget.SelectedValue);
            }
            else
            {
                taskInfo.Priority = Convert.ToInt16(txtPriority.Text);
                taskInfo.StageSource = new Location(this.ddlLocStageDispatch.SelectedValue);
                taskInfo.StageTarget = new Location(this.ddlLocDock.SelectedValue);
            }

            taskInfo.Warehouse = taskSimulationSelect.OutboundOrder.Warehouse;
            taskInfo.Owner = taskSimulationSelect.OutboundOrder.Owner;
            taskInfo.IsComplete = false;
            taskInfo.Status = true;
            taskInfo.IdTrackTaskType = (int)TrackTaskTypeName.Liberada;
            taskInfo.TypeCode = dispatchType;

            OutboundOrder orderSelect = new OutboundOrder();
            orderSelect = taskSimulationSelect.OutboundOrder;

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
                        orderSelect.InmediateProcess = true;
                    }
                    else
                    {
                        orderSelect.InmediateProcess = false;
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
                        orderSelect.AllowBackOrder = true;
                        orderSelect.ExpectedDate = Convert.ToDateTime(txtExpDateBackOrder.Text);
                    }
                    else
                    {
                        orderSelect.AllowBackOrder = false;
                    }
                    if (chkCrossDock.Checked)
                    {
                        orderSelect.AllowCrossDock = true;
                    }
                    else
                    {
                        orderSelect.AllowCrossDock = false;
                    }

                    GenericViewDTO<OutboundOrder> orderViewDTO = new GenericViewDTO<OutboundOrder>();
                    orderViewDTO.Entities.Add(orderSelect);

                    taskSimulationViewDTO = iDispatchingMGR.CreateOrderToQueue(taskInfo, orderViewDTO, constTypeLpnClosedBox, Decimal.Parse(constPercentageTolerance), context);
                    break;

                case "PIKWV":

                    PtlType thePtlType = null;
                    string ptlTypeCode = null;
                    //Validar el tipo de documento asocido a la Ola PTL
                    if (this.ddlPtlType.SelectedValue != "-1" && taskSimulationSelect.IsPtl)
                    {
                        PtlType thePtlTypeParam = new PtlType();
                        thePtlTypeParam.PtlTypeCode = this.ddlPtlType.SelectedValue;
                        GenericViewDTO<PtlType> thePtlTypeViewDTO = iWarehousingMGR.GetPtlTypeByAnyParameter(thePtlTypeParam, context);
                        thePtlType = thePtlTypeViewDTO.Entities[0];
                        ptlTypeCode = thePtlType.PtlTypeCode;
                    }

                    //Buscar las ordenes asosiadas a la ola.
                    selectedOrdersViewDTO = iDispatchingMGR.GetOutboundOrderAndDetailInSimulation(context, taskSimulationSelect);

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

                    taskSimulationViewDTO = iDispatchingMGR.CreateWaveToQueue(taskInfo, selectedOrdersViewDTO, Convert.ToInt16(txtUserNbrSorting.Text), false, ptlTypeCode, context);

                    if (!taskSimulationViewDTO.hasError())
                    {
                        var retryTaskQueueViewDTO = iDispatchingMGR.UpdateDocument(taskSimulationSelect.Id, taskSimulationViewDTO.Entities[0].OutboundOrder.Id, context);

                        if (retryTaskQueueViewDTO.hasError())
                        {
                            taskSimulationViewDTO.Errors = retryTaskQueueViewDTO.Errors;
                        }
                    }

                    break;
                case "PIKBT":
                    //Buscar las ordenes asosiadas a la ola.
                    selectedOrdersViewDTO = iDispatchingMGR.GetOutboundOrderAndDetailInSimulation(context, taskSimulationSelect);

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

                    taskSimulationViewDTO = iDispatchingMGR.CreateBatchToQueue(taskInfo, selectedOrdersViewDTO, constTypeLpnClosedBox, Decimal.Parse(constPercentageTolerance), context);

                    if (!taskSimulationViewDTO.hasError())
                    {
                        var retryTaskQueueViewDTO = iDispatchingMGR.UpdateDocument(taskSimulationSelect.Id, taskSimulationViewDTO.Entities[0].Id, context);

                        if (retryTaskQueueViewDTO.hasError())
                        {
                            taskSimulationViewDTO.Errors = retryTaskQueueViewDTO.Errors;
                        }
                    }
                    break;
            }

            mpReleaseDispatch.Hide();

            //Muestra mensaje en la barra de status
            crud = true;


            // Actualiza la lista de Ordenes Pendientes
            if (taskSimulationViewDTO.hasError())
            {
                var retryTaskQueueViewDTO = iDispatchingMGR.UpdateTrackTaskQueue(taskSimulationSelect.Id, (int)eTrackTaskQueue.Error, context);

                if (retryTaskQueueViewDTO.hasError())
                {
                    taskSimulationViewDTO.Errors = retryTaskQueueViewDTO.Errors;
                }

                UpdateSession(true);
            }
            else
            {

                //Update del TaskSimulation
                var retryTaskQueueViewDTO = iDispatchingMGR.UpdateTrackTaskQueue(taskSimulationSelect.Id, (int)eTrackTaskQueue.Release, context);

                if (retryTaskQueueViewDTO.hasError())
                {
                    taskSimulationViewDTO.Errors = retryTaskQueueViewDTO.Errors;
                    UpdateSession(true);
                }
                else
                {

                    if (dispatchType == "PIKOR")
                    {
                        this.Master.ucDialog.linkPageVisible = true;
                        this.Master.ucDialog.linkNavigationUrl = "~/Outbound/Consult/CheckOrdersInQueue.aspx";
                        this.Master.ucDialog.linkText = "Revisar estado acá";

                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNroOrderQueuedGenerate.Text, "");
                    }
                    else if (dispatchType == "PIKWV")
                    {
                        this.Master.ucDialog.linkPageVisible = true;
                        this.Master.ucDialog.linkNavigationUrl = "~/Outbound/Consult/CheckOrdersWaveInQueue.aspx";
                        this.Master.ucDialog.linkText = "Revisar estado acá";

                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNroWaveQueuedGenerate.Text.Replace("[NROWAVE]", taskSimulationViewDTO.Entities[0].OutboundOrder.Id.ToString()), "");
                    }
                    else if (dispatchType == "PIKBT")
                    {
                        this.Master.ucDialog.linkPageVisible = true;
                        this.Master.ucDialog.linkNavigationUrl = "~/Outbound/Consult/CheckBatchesInQueue.aspx";
                        this.Master.ucDialog.linkText = "Revisar estado acá";

                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNroBatchQueuedGenerate.Text.Replace("[NROBATCH]", taskSimulationViewDTO.Entities[0].Id.ToString()), "");
                    }
                    else
                    {
                        ucStatus.ShowMessage(taskSimulationViewDTO.MessageStatus.Message);
                    }

                    UpdateSession(false);

                    // Limpia la Simulación
                    taskSimulationViewDTO = new GenericViewDTO<TaskSimulation>();
                    //taskDetailSimulationViewDTO = new GenericViewDTO<TaskDetailSimulation>();
                    selectedOrdersViewDTO = new GenericViewDTO<OutboundOrder>();

                    //InitializeSelectedRows();
                    //InitializeCheckedRows();

                    //InitializeSession();

                    //// Oculta detalle de las grillas
                    //HideDetails();
                }
            }
        }

        protected void ReleaseOrdersChecks()
        {
            bool error = false;
            var tareas = (GenericViewDTO<TaskSimulation>)Session["TareasSeleccionadasSimulacion"];
            
            if (tareas != null && tareas.Entities != null && tareas.Entities.Count > 0)
            {
                foreach (var a in tareas.Entities)
                {
                    taskSimulationSelect = a;

                    Task taskInfo = new Task();
                    Task kittingTaskInfo = new Task();
                    Task vasTaskInfo = new Task();

                    // Información general de todas las Tareas a generar
                    if (divUserNbr.Visible)
                        taskInfo.WorkersRequired = Convert.ToInt16(txtUserNbr.Text);
                    else
                        taskInfo.WorkersRequired = 1;

                    taskInfo.WorkersAssigned = 0;

                    if (dispatchType == "PIKPS")
                    {
                        taskInfo.Priority = Convert.ToInt16(txtPriorityPickAndPass.Text);
                        taskInfo.StageTarget = new Location(ddlLocStageTarget.SelectedValue);
                    }
                    else
                    {
                        taskInfo.Priority = Convert.ToInt16(txtPriority.Text);
                        taskInfo.StageSource = new Location(this.ddlLocStageDispatch.SelectedValue);
                        taskInfo.StageTarget = new Location(this.ddlLocDock.SelectedValue);
                    }

                    taskInfo.Warehouse = taskSimulationSelect.OutboundOrder.Warehouse;
                    taskInfo.Owner = taskSimulationSelect.OutboundOrder.Owner;
                    taskInfo.IsComplete = false;
                    taskInfo.Status = true;
                    taskInfo.IdTrackTaskType = (int)TrackTaskTypeName.Liberada;
                    taskInfo.TypeCode = dispatchType;

                    OutboundOrder orderSelect = new OutboundOrder();
                    orderSelect = taskSimulationSelect.OutboundOrder;

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
                                orderSelect.InmediateProcess = true;
                            }
                            else
                            {
                                orderSelect.InmediateProcess = false;
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
                                orderSelect.AllowBackOrder = true;
                                orderSelect.ExpectedDate = Convert.ToDateTime(txtExpDateBackOrder.Text);
                            }
                            else
                            {
                                orderSelect.AllowBackOrder = false;
                            }
                            if (chkCrossDock.Checked)
                            {
                                orderSelect.AllowCrossDock = true;
                            }
                            else
                            {
                                orderSelect.AllowCrossDock = false;
                            }

                            GenericViewDTO<OutboundOrder> orderViewDTO = new GenericViewDTO<OutboundOrder>();
                            orderViewDTO.Entities.Add(orderSelect);
                            if (validateTaskRelease(taskSimulationSelect) || validateOrdersInOtherTask(taskSimulationSelect))
                            {
                                string errorPk = this.lblSimulateRelease.Text.Replace("[OUTBOUND]", taskSimulationSelect.OutboundOrder.Number);
                                taskSimulationViewDTO.Errors = new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = errorPk, Code = "" };
                                //this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, ""); 
                                error = true;
                            }
                            else if (validateOrdersRelease(taskSimulationSelect))
                            {
                                string errorPK = this.lblOrdersRelease.Text;
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, errorPK, "");
                                error = true;
                            }
                            else
                            {
                                taskSimulationViewDTO = iDispatchingMGR.CreateOrderToQueue(taskInfo, orderViewDTO, constTypeLpnClosedBox, Decimal.Parse(constPercentageTolerance), context);
                            }
                            break;

                        case "PIKWV":
                            //Buscar las ordenes asosiadas a la ola.
                            selectedOrdersViewDTO = iDispatchingMGR.GetOutboundOrderAndDetailInSimulation(context, taskSimulationSelect);

                            PtlType thePtlType = null;
                            string ptlTypeCode = null;
                            //Validar el tipo de documento asocido a la Ola PTL
                            if (this.ddlPtlType.SelectedValue != "-1" && taskSimulationSelect.IsPtl)
                            {
                                PtlType thePtlTypeParam = new PtlType();
                                thePtlTypeParam.PtlTypeCode = this.ddlPtlType.SelectedValue;
                                GenericViewDTO<PtlType> thePtlTypeViewDTO = iWarehousingMGR.GetPtlTypeByAnyParameter(thePtlTypeParam, context);
                                thePtlType = thePtlTypeViewDTO.Entities[0];
                                ptlTypeCode = thePtlType.PtlTypeCode;
                            }

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
                            if (!validateTaskRelease(taskSimulationSelect) || validateOrdersInOtherTask(taskSimulationSelect))
                            {
                                string errorPk = this.lblSimulateReleaseBatch.Text.Replace("[TASK]", taskSimulationSelect.Id.ToString());
                                taskSimulationViewDTO.Errors = new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = errorPk, Code = "" };
                                //this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, ""); 
                                error = true;
                            }
                            else if (validateOrdersRelease(taskSimulationSelect))
                            {
                                string errorPK = this.lblOrdersRelease.Text;
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, errorPK, "");
                                error = true;
                            }
                            else
                            {
                                taskSimulationViewDTO = iDispatchingMGR.CreateWaveToQueue(taskInfo, selectedOrdersViewDTO, Convert.ToInt16(txtUserNbrSorting.Text), false, ptlTypeCode, context);

                                if (!taskSimulationViewDTO.hasError())
                                {
                                    var retryTaskQueueViewDTO = iDispatchingMGR.UpdateDocument(taskSimulationSelect.Id, taskSimulationViewDTO.Entities[0].OutboundOrder.Id, context);

                                    if (retryTaskQueueViewDTO.hasError())
                                    {
                                        taskSimulationViewDTO.Errors = retryTaskQueueViewDTO.Errors;
                                        error = true;
                                    }
                                }
                            }
                            break;

                        case "PIKBT":
                            selectedOrdersViewDTO = iDispatchingMGR.GetOutboundOrderAndDetailInSimulation(context, taskSimulationSelect);

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

                            if (!validateTaskRelease(taskSimulationSelect) || validateOrdersInOtherTask(taskSimulationSelect))
                            {
                                string errorPk = this.lblSimulateReleaseBatch.Text.Replace("[TASK]", taskSimulationSelect.Id.ToString());
                                taskSimulationViewDTO.Errors = new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = errorPk, Code = "" };
                                //this.Master.ucDialog.ShowAlert(this.lblTitle.Text, error, ""); 
                                error = true;
                            }
                            else if (validateOrdersRelease(taskSimulationSelect))
                            {
                                string errorPK = this.lblOrdersRelease.Text;
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, errorPK, "");
                                error = true;
                            }
                            else
                            {
                                taskSimulationViewDTO = iDispatchingMGR.CreateBatchToQueue(taskInfo, selectedOrdersViewDTO, constTypeLpnClosedBox, Decimal.Parse(constPercentageTolerance), context);

                                if (!taskSimulationViewDTO.hasError())
                                {
                                    var retryTaskQueueViewDTO = iDispatchingMGR.UpdateDocument(taskSimulationSelect.Id, taskSimulationViewDTO.Entities[0].Id, context);

                                    if (retryTaskQueueViewDTO.hasError())
                                    {
                                        taskSimulationViewDTO.Errors = retryTaskQueueViewDTO.Errors;
                                    }
                                }
                            }
                            break;
                    }

                    mpReleaseDispatch.Hide();

                    //Muestra mensaje en la barra de status
                    crud = true;

                    // Actualiza la lista de Ordenes Pendientes
                    if (taskSimulationViewDTO.hasError())
                    {
                        var retryTaskQueueViewDTO = iDispatchingMGR.UpdateTrackTaskQueue(taskSimulationSelect.Id, (int)eTrackTaskQueue.Error, context);

                        if (retryTaskQueueViewDTO.hasError())
                        {
                            taskSimulationViewDTO.Errors = retryTaskQueueViewDTO.Errors;
                            error = true;
                            break;
                        }

                        //UpdateSession(true);
                    }
                    else
                    {

                        //Update del TaskSimulation
                        var retryTaskQueueViewDTO = iDispatchingMGR.UpdateTrackTaskQueue(taskSimulationSelect.Id, (int)eTrackTaskQueue.Release, context);

                        if (retryTaskQueueViewDTO.hasError())
                        {
                            taskSimulationViewDTO.Errors = retryTaskQueueViewDTO.Errors;
                            //UpdateSession(true);
                            error = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                error = true;
            }

            if (!error)
            {
                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblReleaseSelected.Text, "");
                UpdateSession(false);

                Session.Add("TareasSeleccionadasSimulacion", null);
                // Limpia la Simulación
                taskSimulationViewDTO = new GenericViewDTO<TaskSimulation>();
                selectedOrdersViewDTO = new GenericViewDTO<OutboundOrder>();
            }else
            {
                this.Master.ucError.ShowError(taskSimulationViewDTO.Errors);
                UpdateSession(false);                           
            }
        }

        protected bool validateTaskRelease()
        {            
            TaskSimulation taskSimParam = new TaskSimulation();

            if(taskSimulationSelect.OutboundOrder.Id != -1) 
            { 
                taskSimParam.OutboundOrder = new OutboundOrder( taskSimulationSelect.OutboundOrder.Id);

                var result = iDispatchingMGR.GetTaskSimulationByAnyParameter(taskSimParam, context);

                foreach (var task in result.Entities)
                {
                    if (task.Id != taskSimulationSelect.Id)
                    {
                        if(task.TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Release)
                        {
                            return true;
                        }
                    }
                }
            }            

            return false;
        }

        protected bool validateOrdersRelease(TaskSimulation taskSim)
        {
            var result = iDispatchingMGR.ValidateTasksSimulationOrdersRelease(taskSim.Id, context);

            return result;
        }

        protected bool validateOrdersInOtherTask(TaskSimulation taskSim)
        {
            var result = iDispatchingMGR.ValidateTasksSimulationInOtherSimulation(taskSim.Id, context);

            return result;
        }
        protected bool validateTaskRelease(TaskSimulation taskSim)
        {
            TaskSimulation taskSimParam = new TaskSimulation();
            taskSimParam.OutboundOrder = new OutboundOrder(taskSim.OutboundOrder.Id);

            var result = iDispatchingMGR.GetTaskSimulationByAnyParameter(taskSimParam, context);

            foreach (var task in result.Entities)
            {
                if (task.Id != taskSim.Id)
                {
                    if (task.TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Release)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        protected void grdWaveOrders_RowCreated(object sender, GridViewRowEventArgs e)
        {
        }

        protected void grdWaveOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        protected void ddlPagesSearchWaveOrdersSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValidateSession("SessionListOutboundDetWave"))
            {              
                outboundDetWaveViewDTO = (GenericViewDTO<OutboundOrder>)Session["SessionListOutboundDetWave"];

                currentPageCustomer = ddlPagesSearchWaveOrders.SelectedIndex;
                grdWaveOrders.PageIndex = currentPageCustomer;

                // Encabezado de Recepciones
                grdWaveOrders.DataSource = outboundDetWaveViewDTO.Entities;
                grdWaveOrders.DataBind();

                divWaveDetail.Visible = true;
                mpWaveDetail.Show();

                ShowCustomerButtonsPage();

            }
        }

        protected void btnFirstGrdSearchWaveOrders_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchWaveOrders.SelectedIndex = 0;
            ddlPagesSearchWaveOrdersSelectedIndexChanged(sender, e);

        }
        protected void btnPrevGrdSearchWaveOrders_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomer > 0)
            {
                ddlPagesSearchWaveOrders.SelectedIndex = currentPageCustomer - 1; 
                ddlPagesSearchWaveOrdersSelectedIndexChanged(sender, e);
            }
        }
        protected void btnLastGrdSearchWaveOrders_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchWaveOrders.SelectedIndex = grdWaveOrders.PageCount - 1;
            ddlPagesSearchWaveOrdersSelectedIndexChanged(sender, e);

        }
        protected void btnNextGrdSearchWaveOrders_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomer < grdWaveOrders.PageCount)
            {
                ddlPagesSearchWaveOrders.SelectedIndex = currentPageCustomer + 1;
                ddlPagesSearchWaveOrdersSelectedIndexChanged(sender, e);

            }
        }
        private void ShowCustomerButtonsPage()
        {
            if (currentPageCustomer == grdWaveOrders.PageCount - 1)
            {
                btnNextGrdSearchWaveOrders.Enabled = false;
                btnNextGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastGrdSearchWaveOrders.Enabled = false;
                btnLastGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                btnPrevGrdSearchWaveOrders.Enabled = true;
                btnPrevGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                btnFirstGrdSearchWaveOrders.Enabled = true;
                btnFirstGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
            }
            else
            {
                if (currentPageCustomer == 0)
                {
                    btnPrevGrdSearchWaveOrders.Enabled = false;
                    btnPrevGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstGrdSearchWaveOrders.Enabled = false;
                    btnFirstGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                    btnNextGrdSearchWaveOrders.Enabled = true;
                    btnNextGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchWaveOrders.Enabled = true;
                    btnLastGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnNextGrdSearchWaveOrders.Enabled = true;
                    btnNextGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchWaveOrders.Enabled = true;
                    btnLastGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevGrdSearchWaveOrders.Enabled = true;
                    btnPrevGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstGrdSearchWaveOrders.Enabled = true;
                    btnFirstGrdSearchWaveOrders.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
            }
        }

        private void InitializePageCountCustomer()
        {
            if (grdWaveOrders.PageCount > 1)
            {
                int pageNumber;

                divPageGrdSearchWaveOrders.Visible = true;
                // Paginador
                ddlPagesSearchWaveOrders.Items.Clear();
                for (int i = 0; i < grdWaveOrders.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPageCustomer) lstItem.Selected = true;

                    ddlPagesSearchWaveOrders.Items.Add(lstItem);
                }
                this.lblPageCountSearchWaveOrders.Text = grdWaveOrders.PageCount.ToString();

                ShowCustomerButtonsPage();
            }
            else
            {
                divPageGrdSearchWaveOrders.Visible = false;
            }
        }

        protected void ddlPtlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlPtlType.SelectedValue != "-1")
                {
                    GenericViewDTO<Location> locationViewDTO = new GenericViewDTO<Location>();
                    Location theLocation = new Location();
                    theLocation.Warehouse = new Warehouse(currentWhs);
                    theLocation.Type = new LocationType();
                    theLocation.Type.LocTypeCode = LocationTypeName.PTLIN.ToString();
                    theLocation.PtlType = new PtlType();
                    theLocation.PtlType.PtlTypeCode = this.ddlPtlType.SelectedValue;

                    locationViewDTO = iLayoutMGR.GetLocationByAnyParameter(theLocation, context);

                    ddlLocStageDispatch.Items.Clear();

                    ddlLocStageDispatch.DataSource = locationViewDTO.Entities;
                    ddlLocStageDispatch.DataTextField = "Description";
                    ddlLocStageDispatch.DataValueField = "IdCode";
                    ddlLocStageDispatch.DataBind();

                    ddlLocStageDispatch.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
                    ddlLocStageDispatch.Items[0].Selected = true;
                }
                else
                {
                    //base.LoadLocationsByWhsAndType(this.ddlLocStageDispatch, currentWhs, LocationTypeName.STGPT.ToString(), this.Master.EmptyRowText, true);
                    ddlLocStageDispatch.Items.Clear();
                    ddlLocStageDispatch.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
                    ddlLocStageDispatch.Items[0].Selected = true;
                }

                divReleaseDispatch.Visible = true;
                mpReleaseDispatch.Show();
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }

        }

        protected void btnExcelDetail_Click(object sender, ImageClickEventArgs e)
        {
            GridView grdMgrAux = new GridView();
            try
            {
                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    grdMgr.AllowPaging = false;
                    PopulateGrid();

                    grdMgrAux = grdMgr;
                    List<TaskSimulation> w = new List<TaskSimulation>();
                    w.Add(taskSimulationViewDTO.Entities[index]);

                    grdMgrAux.DataSource = w;
                    grdMgrAux.DataBind();

                    LoadDetail(index);

                    base.ExportToExcel(grdMgrAux, grdDetail, null);

                    grdMgr.AllowPaging = true;
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                taskSimulationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskSimulationViewDTO.Errors);
            }
        }

        private void ExportToExcel(string nameReport, GridView wControl)
        {
            HttpResponse response = Response;
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            Page pageToRender = new Page();
            HtmlForm form = new HtmlForm();
            form.Controls.Add(wControl);
            pageToRender.Controls.Add(form);
            response.Clear();
            response.Buffer = true;
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=" + nameReport);
            response.Charset = "UTF-8";
            response.ContentEncoding = Encoding.Default;
            pageToRender.RenderControl(htw);
            response.Write(sw.ToString());
            response.End();
        }
    }
}