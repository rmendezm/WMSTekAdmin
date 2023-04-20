using Binaria.WMSTek.AdminApp.Label;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Label;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.AdminApp.Manager;
using System.Xml;
using System.Xml.Linq;

namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class PrintQueueStatus : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<TaskLabel> taskLabelViewDTO = new GenericViewDTO<TaskLabel>();
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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

                    // Asinga el atributo 'onclick' a todas las columnas de la grilla, excepto a la que contiene los checkboxes
                    // IMPORTANTE: no cambiar de lugar la columna [0] que contiene los checkboxes
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }


        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

                ShowModal(editIndex);
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = false;

                foreach (GridViewRow row in grdMgr.Rows)
                {
                    CheckBox chk = (CheckBox)row.Cells[0].FindControl("chkAdjustConfirm");

                    if (chk.Checked == true)
                    {
                        isValid = true;
                    }
                }

                if (isValid)
                {
                    this.ShowConfirm(this.lblConfirmAdjustHeader.Text, lblConfirmAdjust.Text);
                }
                else
                {
                    this.Master.ucDialog.ShowAlert(this.lblConfirmAdjustHeader.Text, this.lblNotSelectedAdjust.Text, "confirm");
                }
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }

        }

        protected void btnSaveConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.TaskLabel.TaskLabelList))
                {
                    taskLabelViewDTO = (GenericViewDTO<TaskLabel>)Session[WMSTekSessions.TaskLabel.TaskLabelList];

                    Session.Add(WMSTekSessions.TaskLabel.TaskLabelList, taskLabelViewDTO);

                    btnOk_Click(new object(), new EventArgs());
                }

            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {

                if (ValidateSession(WMSTekSessions.TaskLabel.TaskLabelList))
                {
                    int index = 0;
                    bool isValid = true;
                    GenericViewDTO<TaskLabel> TaskLabelViewDTO = new GenericViewDTO<TaskLabel>();

                    TaskLabelViewDTO = (GenericViewDTO<TaskLabel>)Session[WMSTekSessions.TaskLabel.TaskLabelList];

                    for (int i = 0; i < grdMgr.Rows.Count; i++)
                    {
                        GridViewRow row = grdMgr.Rows[i];

                        if (((System.Web.UI.WebControls.CheckBox)row.FindControl("chkAdjustConfirm")).Checked)
                        {
                            index = (grdMgr.PageIndex * grdMgr.PageSize) + i;

                            TaskLabelViewDTO.Entities[index].DelayPrinted = 0;
                            iLabelMGR.SetIsPrinted(TaskLabelViewDTO.Entities[index], context);

                            if (TaskLabelViewDTO.hasError())
                                isValid = false;
                        }
                    }
                    if (isValid)
                    {
                        crud = true;
                        ucStatus.ShowMessage(TaskLabelViewDTO.MessageStatus.Message);

                        UpdateSession();
                    }
                    else
                    {
                        UpdateSession();
                    }
                }
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                modalPopUpDialog.Hide();
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        protected void ShowModal(int index)
        {            
            //Recupera los datos de la entidad a editar
            hidEditId.Value = taskLabelViewDTO.Entities[index].Id.ToString();
            string paramString = taskLabelViewDTO.Entities[index].ParamString;
            lblEdit.Visible = true;

            this.txtCustomerName.Text = string.Empty;
            this.txtTypeLabel.Text = string.Empty;
            this.txtDateCreated.Text = string.Empty;
            this.txtParamName.Text = string.Empty;
            this.txtParamValue.Text = string.Empty;

            this.txtCustomerName.Text = taskLabelViewDTO.Entities[index].Customer.Name;
            this.txtTypeLabel.Text = taskLabelViewDTO.Entities[index].LabelTemplate.Name;
            this.txtDateCreated.Text = taskLabelViewDTO.Entities[index].DateCreated.ToString("dd-MM-yyyy");

            this.txtCustomerName.Enabled = false;
            this.txtTypeLabel.Enabled = false;
            this.txtDateCreated.Enabled = false;
            
            XmlDocument theXdoc = new XmlDocument();
            theXdoc.LoadXml( paramString);
            XmlNodeList labelParams = theXdoc.GetElementsByTagName("label_params");
            XmlNodeList listaParam = ((XmlElement)labelParams[0]).GetElementsByTagName("label_param");

            List<string> lstParams = new List<string>();

            foreach (XmlElement nodo in listaParam)
            {
                string campo = nodo.ChildNodes[0].InnerText + ": " + nodo.ChildNodes[1].InnerText;
                lstParams.Add(campo);
            }

            lwParamString.DataSource = lstParams;
            lwParamString.DataBind();                

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            TaskLabel taskLabel = taskLabelViewDTO.Entities.Where(w => w.Id.Equals(Convert.ToInt32(hidEditId.Value))).FirstOrDefault();
            string paramString = this.hidParamString.Value.Trim();

            XDocument xdoc = XDocument.Parse(taskLabel.ParamString.Trim());
            XElement param = xdoc.Root.Element("label_params");
            param.Add(
                    new XElement("label_param",
                    new XElement("name", this.txtParamName.Text.Trim()),
                    new XElement("value", this.txtParamValue.Text.Trim()))
                    );

            taskLabel.ParamString = xdoc.ToString().Replace("\r\n", "").Replace(" ", "");

            taskLabelViewDTO = iLabelMGR.SetTaskLabelAsParamString(taskLabel, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (taskLabelViewDTO.hasError())
            {
                UpdateSession();
                divEditNew.Visible = true;
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(taskLabelViewDTO.MessageStatus.Message);
                UpdateSession();
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected void btnAcceptEditMassiveTaskLabel_Click(object sender, EventArgs e)
        {
            try
            {
                var listTaskLabel = GetListWithNewParamString(GetAllRowsSelected());

                if (listTaskLabel.Count > 0)
                {
                    var TaskLabelDTO = iLabelMGR.SetTaskLabelAsParamStringMassive(listTaskLabel, context);

                    if (TaskLabelDTO.hasError())
                    {
                        this.Master.ucError.ShowError(TaskLabelDTO.Errors);
                    }
                    else
                    {
                        UpdateSession();
                        upGrid.Update();
                        ucStatus.ShowMessage(lblEditMassiveSuccess.Text);
                        txtParamNameMassive.Text = string.Empty;
                        txtParamValueMassive.Text = string.Empty;
                    }
                }
                else
                {
                    ucStatus.ShowMessage(lblValidateEditMassive.Text);
                }
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }
        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "PrintQueueStatus";

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
                if (ValidateSession(WMSTekSessions.TaskLabel.TaskLabelList))
                {
                    taskLabelViewDTO = (GenericViewDTO<TaskLabel>)Session[WMSTekSessions.TaskLabel.TaskLabelList];
                    isValidViewDTO = true;
                }

                if (isValidViewDTO)
                {
                    //Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                    PopulateGrid();
                }
            }
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnSaveClick += new EventHandler(btnConfirm_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnSaveVisible = true;
            //this.Master.ucTaskBar.btnSaveToolTip = this.lblBtnSaveToolTip.Text;
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.descriptionLabel = lblFilterCodeLabel.Text; // lblFilterOutboundNumber.Text;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = lblFilterCustomerLabel.Text;
            this.Master.ucMainFilter.documentVisible = true;

            this.Master.ucMainFilter.impressionTailVisible = true;
            //this.Master.ucMainFilter.LabelRadiobuttonVisible = LblRadiobutton.Text;
            //this.Master.ucMainFilter.LabelRadiobuttonFalseVisible = LblRadiobuttonFalse.Text;

            // Configura textos a mostar
            ////this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.AdjustmentConsultDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.AdjustmentConsultDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
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

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!taskLabelViewDTO.hasConfigurationError() && taskLabelViewDTO.Configuration != null && taskLabelViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, taskLabelViewDTO.Configuration);

            grdMgr.DataSource = taskLabelViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(taskLabelViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                divGrid.Visible = true;
                this.Master.ucError.ClearError();
            }
        }

        private void UpdateSession()
        {
            // Carga consulta de Tareas de Ajuste
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            TextBox txtLpnCode = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDescription");
            TextBox txtOutboundOrderCode = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDocumentNumber");
            RadioButtonList rblFind = (RadioButtonList)this.Master.ucMainFilter.FindControl("rblSearchCriteriaFind");

            eTypeLabelInPrintQueue selectedValue = (eTypeLabelInPrintQueue)(int.Parse(rblFind.SelectedValue));
            taskLabelViewDTO = iLabelMGR.GetPendingTasksPrint(txtLpnCode.Text.Trim(), txtOutboundOrderCode.Text.Trim(), selectedValue, context);

            if (!taskLabelViewDTO.hasError() && taskLabelViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.TaskLabel.TaskLabelList, taskLabelViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(taskLabelViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                grdMgr.DataSource = null;
                isValidViewDTO = false;
                grdMgr.DataBind();
                //this.Master.ucError.ShowError(TaskLabelViewDTO.Errors);
            }
        }

        private void ShowConfirm(string title, string message)
        {
            this.divConfirm.Visible = true;

            this.lblDialogTitle.Text = title;
            this.divDialogMessage.InnerHtml = message;

            //this.Visible = true;
            modalPopUpDialog.Show();
        }

        private List<TaskLabel> GetListWithNewParamString(List<TaskLabel> listTaskLabel)
        {
            foreach (var taskLabel in listTaskLabel)
            {
                TaskLabelNewParamString(taskLabel);
            }

            return listTaskLabel;
        }

        private TaskLabel TaskLabelNewParamString(TaskLabel taskLabel)
        {
            XDocument xdoc = XDocument.Parse(taskLabel.ParamString.Trim());
            XElement param = xdoc.Root.Element("label_params");
            param.Add(
                    new XElement("label_param",
                    new XElement("name", txtParamNameMassive.Text.Trim()),
                    new XElement("value", txtParamValueMassive.Text.Trim()))
                    );

            taskLabel.ParamString = xdoc.ToString();

            return taskLabel;
        }

        private List<TaskLabel> GetAllRowsSelected()
        {
            var taskLabelsSelected = new List<TaskLabel>();

            for (int i = 0; i < grdMgr.Rows.Count; i++)
            {
                var row = grdMgr.Rows[i];
                var chkSelectOutboundOrder = (CheckBox)row.FindControl("chkAdjustConfirm");

                if (chkSelectOutboundOrder.Checked)
                {
                    var lblId = (Label)row.FindControl("lblId");
                    var selected = taskLabelViewDTO.Entities.Where(x => x.Id == int.Parse(lblId.Text.Trim())).FirstOrDefault();
                    taskLabelsSelected.Add(selected);
                }
            }

            return taskLabelsSelected;
        }

        private void ShowAlert(string title, string message)
        {
            string script = "ShowMessage('" + title + "','" + message + "');";
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('TaskLabel_GetPendingTasksPrint', 'ctl00_MainContent_grdMgr');", true);
        }
        #endregion
    }
}