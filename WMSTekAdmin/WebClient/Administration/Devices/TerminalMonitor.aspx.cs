using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class TerminalMonitor : BasePage
    {
        #region "Declaración de Variables"

        GenericViewDTO<Monitor> monitorViewDTO = new GenericViewDTO<Monitor>();
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
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
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
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "setHeightGrid();", true);
            }
            catch (Exception ex)
            {
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
            }
        }

        /// <summary>
        /// Limpia el filtro y carga los datos desde base de datos
        /// </summary>
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
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
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
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
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
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
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
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
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
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
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
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
        //        monitorViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(monitorViewDTO.Errors);
        //    }
        //}

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDisconnect = e.Row.FindControl("btnDisconnect") as ImageButton;
                    ImageButton btnTerminalLog = e.Row.FindControl("btnTerminalLog") as ImageButton;

                    if (btnDisconnect != null)
                    {
                        btnDisconnect.OnClientClick = "if(!confirm('" + lblConfirmDisconnect.Text + DataBinder.Eval(e.Row.DataItem, "TerminalCode") + "?')) return false;";
                    }

                    if (btnTerminalLog != null)
                    {
                        string infoLog = string.Empty;

                        if ((Boolean)DataBinder.Eval(e.Row.DataItem, "ActivateLogTerminalRF") == true)
                        {
                            infoLog = lblConfirmDisabledLog.Text;
                        }
                        else
                        {
                            infoLog = lblConfirmActiveLog.Text;
                        }

                        btnTerminalLog.OnClientClick = "if(!confirm('" + infoLog + DataBinder.Eval(e.Row.DataItem, "TerminalCode") + "?')) return false;";
                    }

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
            }
        }

        protected void grdMgr_DataBound(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow fila in grdMgr.Rows)
                {
                    Label lblTerminalStatus = (Label)fila.FindControl("lblTerminalStatus");
                    ImageButton btnDisconnect = (ImageButton)fila.FindControl("btnDisconnect");
                    ImageButton btnTerminalLog = (ImageButton)fila.FindControl("btnTerminalLog");
                    CheckBox chkStatusMonitor = (CheckBox)fila.FindControl("chkStatusMonitor") as CheckBox;
                    CheckBox chkActivateLog = (CheckBox)fila.FindControl("chkActivateLog") as CheckBox;

                    string strTerminalStatus = lblTerminalStatus.Text;

                    if (strTerminalStatus == TerminalStatus.Connected.ToString())
                    {
                        if (chkStatusMonitor.Checked == false)
                        {
                            btnDisconnect.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_connect.png";
                            btnDisconnect.Enabled = true;
                            btnDisconnect.ToolTip = lblDisconnect.Text;
                            lblTerminalStatus.ForeColor = Color.Green;
                            lblTerminalStatus.Font.Bold = true;
                        }
                        else
                        {
                            btnDisconnect.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_disconnect_dis.png";
                            btnDisconnect.Enabled = false;
                            btnDisconnect.ToolTip = lblDisconnect2.Text;
                            lblTerminalStatus.ForeColor = Color.Red;
                        }

                        if (chkActivateLog.Checked == false)
                        {
                            btnTerminalLog.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_cancel.png";
                            btnTerminalLog.Enabled = true;
                            btnTerminalLog.ToolTip = lblActiveLog.Text;
                        }
                        else
                        {
                            btnTerminalLog.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_apply_inventory.png";
                            btnTerminalLog.Enabled = true;
                            btnTerminalLog.ToolTip = lblDisabledLog.Text;
                        }
                    }
                    else
                    {
                        btnDisconnect.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_disconnect_dis.png";
                        btnDisconnect.Enabled = false;
                        btnDisconnect.ToolTip = lblDisconnected.Text;
                        lblTerminalStatus.ForeColor = Color.Red;

                        //Log Terminal
                        btnTerminalLog.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_cancel_dis.png";
                        btnTerminalLog.Enabled = false;
                        btnTerminalLog.ToolTip = lblLogTerminal.Text;
                    }
                 }
            }
            catch (Exception ex)
            {
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index;
                if (e.CommandName == "Disconnect")
                {
                    index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);
                    int TerminalId = monitorViewDTO.Entities[index].TerminalId;
                    string nomTerm = monitorViewDTO.Entities[index].TerminalName;

                    if (TerminalId != 0)
                    {
                        var disconnectViewDTO = iDeviceMGR.UpdateContext(TerminalId, context.PathClassRemoting, context);

                        if (disconnectViewDTO.hasError())
                        {
                            isValidViewDTO = false;
                            this.Master.ucError.ShowError(disconnectViewDTO.Errors);
                        }
                        else
                        {
                            UpdateSession(false);
                            PopulateGrid();
                            ucStatus.ShowMessage(this.lblDisconnectedNew.Text.Replace("X", nomTerm));
                        }
                    }
                }
                else if (e.CommandName == "TerminalLog")
                {
                    index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);
                    int TerminalId = monitorViewDTO.Entities[index].TerminalId;
                    string nomTerm = monitorViewDTO.Entities[index].TerminalName;

                    if (TerminalId != 0)
                    {
                        iDeviceMGR.ActivateLogTerminalRF(TerminalId, context.PathClassRemoting);
                        
                        UpdateSession(false);
                        PopulateGrid();

                        if (monitorViewDTO.Entities[index].ActivateLogTerminalRF)
                            ucStatus.ShowMessage(this.lblInfoLogActive.Text.Replace("X", nomTerm));
                        else
                            ucStatus.ShowMessage(this.lblInfoLogDisabled.Text.Replace("X", nomTerm));
                    }
                }
            }
            catch (Exception ex)
            {
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
            }
        }

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
            }
            catch (Exception ex)
            {
                monitorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            this.Master.ucMainFilter.LoadControlValuesToFilterObject();
            UpdateSession(false);

            //bandera = true;

            // Si es un ViewDTO valido, carga la grilla y las listas
            if (isValidViewDTO)
            {
                // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                PopulateGrid();
            }
        }

        /// <summary>
        /// Carga en sesion la lista de la Entidad
        /// </summary>
        /// <param name="showError">Determina si se mostrara el error producido en una operacion anterior</param>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
                monitorViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            ClearFilter("TerminalStatus");

            var ddlTerminalStatus = (DropDownList)this.Master.ucMainFilter.FindControl("ddlTerminalStatus");

            if (ddlTerminalStatus != null)
                CreateFilterByList("TerminalStatus", new List<string> { ddlTerminalStatus.SelectedValue });

            monitorViewDTO = iDeviceMGR.FindAllTerminalMonitor(context);

            if (!monitorViewDTO.hasError() && monitorViewDTO.Entities != null)
            {
                isValidViewDTO = true;
                divGrid.Visible = true;

                if (!crud)
                    ucStatus.ShowMessage(monitorViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(monitorViewDTO.Errors);
            }
        }

        /// <summary>
        /// Configuracion inicial del Filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.statusVisible = true;

            this.Master.ucMainFilter.codeAltVisible = true;
            this.Master.ucMainFilter.codeLabelAlt = lblRfOperatorUserNameFilterMsg.Text;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.DocumentNumberLabel = lblMaqUserFilterMsg.Text;
            this.Master.ucMainFilter.divTerminalStatusVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;
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

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!monitorViewDTO.hasConfigurationError() && monitorViewDTO.Configuration != null && monitorViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, monitorViewDTO.Configuration);

            grdMgr.DataSource = monitorViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(monitorViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }

        #endregion
    }
}