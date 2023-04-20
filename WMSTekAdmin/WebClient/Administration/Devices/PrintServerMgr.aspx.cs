using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Utils;

namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class PrintServerMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<PrintServer> printServerViewDTO = new GenericViewDTO<PrintServer>();
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
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        // Carga inicial del ViewDTO
                        UpdateSession(false);

                        //if (isValidViewDTO)
                        //{
                        //    PopulateLists();
                        //}
                    }

                    if (ValidateSession(WMSTekSessions.PrintServerMgr.PrintServerList))
                    {
                        printServerViewDTO = (GenericViewDTO<PrintServer>)Session[WMSTekSessions.PrintServerMgr.PrintServerList];
                        isValidViewDTO = true;
                    }

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
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
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
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "SetDivs();", true);
            }
            catch (Exception ex)
            {
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
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
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
            }
        }

        /// <summary>
        /// Limpia el filtro y carga los datos desde base de datos
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
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
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
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
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
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
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
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
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
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
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
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
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
        //        printerViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(printerViewDTO.Errors);
        //    }
        //}

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowModal(0, CRUD.Create);
            }
            catch (Exception ex)
            {
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a eliminar
                int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;

                DeleteRow(deleteIndex);
            }
            catch (Exception ex)
            {
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
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
                printServerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        public void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
                printServerViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            printServerViewDTO = iDeviceMGR.FindAllPrintServer(context);

            if (!printServerViewDTO.hasError() && printServerViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.PrintServerMgr.PrintServerList, printServerViewDTO);
                //Session.Remove(WMSTekSessions.Shared.PrinterList);

                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(printServerViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(printServerViewDTO.Errors);
            }
        }

       
        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!printServerViewDTO.hasConfigurationError() && printServerViewDTO.Configuration != null && printServerViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, printServerViewDTO.Configuration);

            grdMgr.DataSource = printServerViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(printServerViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.nameVisible = true;
            //this.Master.ucMainFilter.warehouseVisible = false;
            this.Master.ucMainFilter.SaveOnIndexChanged = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
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

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            // Editar Impresora
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = printServerViewDTO.Entities[index].Id.ToString();

                this.txtServerName.Text = printServerViewDTO.Entities[index].ServerName;
                this.txtServiceName.Text = printServerViewDTO.Entities[index].ServiceName;
                this.txtIpAddress.Text = (printServerViewDTO.Entities[index].IpAddress == "-1" ? "" : printServerViewDTO.Entities[index].IpAddress);
                this.txtIpPort.Text = (printServerViewDTO.Entities[index].IpPort == -1 ? "" : printServerViewDTO.Entities[index].IpPort.ToString());
                this.chkStatus.Checked = (printServerViewDTO.Entities[index].Status == 1 ? true : false); 
                this.txtTimeoutPrint.Text = printServerViewDTO.Entities[index].TimeoutPrint.ToString();
                this.txtIntervalPrint.Text = printServerViewDTO.Entities[index].IntervalPrint.ToString();
                this.txtTimeoutSql.Text = printServerViewDTO.Entities[index].TimeoutSql.ToString();
                this.txtIntervalSql.Text = printServerViewDTO.Entities[index].IntervalSql.ToString();
                this.txtQtyTaskPerQuery.Text = printServerViewDTO.Entities[index].QtyTaskPerQuery.ToString();
                
                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva Impresora
            if (mode == CRUD.Create)
            { 
                hidEditId.Value = "0";
                this.txtServerName.Text = string.Empty;
                this.txtServiceName.Text = string.Empty;
                this.txtIpAddress.Text = string.Empty;
                this.txtIpPort.Text = string.Empty;
                this.chkStatus.Checked = false; 
                this.txtTimeoutPrint.Text = string.Empty;
                this.txtIntervalPrint.Text = string.Empty;
                this.txtTimeoutSql.Text = string.Empty;
                this.txtIntervalSql.Text = string.Empty;
                this.txtQtyTaskPerQuery.Text = string.Empty;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (printServerViewDTO.Configuration != null && printServerViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(printServerViewDTO.Configuration, true);
                else
                    base.ConfigureModal(printServerViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            PrintServer printerServer = new PrintServer();
            printerServer.Id = (Convert.ToInt32(hidEditId.Value));
            printerServer.ServerName = this.txtServerName.Text.Trim();
            printerServer.ServiceName = this.txtServiceName.Text.Trim();
            
            if (this.txtIpAddress.Text.Trim() != string.Empty)
                printerServer.IpAddress = this.txtIpAddress.Text.Trim();
            
            if (this.txtIpPort.Text.Trim() != string.Empty)
                printerServer.IpPort = int.Parse(this.txtIpPort.Text.Trim());
         
            printerServer.Status = (this.chkStatus.Checked ? 1 : 0);
            printerServer.TimeoutPrint = int.Parse(this.txtTimeoutPrint.Text.Trim());
            printerServer.IntervalPrint = int.Parse(this.txtIntervalPrint.Text.Trim());
            printerServer.TimeoutSql = int.Parse(this.txtTimeoutSql.Text.Trim());
            printerServer.IntervalSql = int.Parse(this.txtIntervalSql.Text.Trim());
            printerServer.QtyTaskPerQuery = int.Parse(this.txtQtyTaskPerQuery.Text.Trim());           

            if (hidEditId.Value == "0")
                printServerViewDTO = iDeviceMGR.MaintainPrintServer(CRUD.Create, printerServer, context);
            else
                printServerViewDTO = iDeviceMGR.MaintainPrintServer(CRUD.Update, printerServer, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (printServerViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(printServerViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            printServerViewDTO = iDeviceMGR.MaintainPrintServer(CRUD.Delete, printServerViewDTO.Entities[index], context);

            if (printServerViewDTO.hasError())
                UpdateSession(true);
            else
            {
                ucStatus.ShowMessage(printServerViewDTO.MessageStatus.Message);
                crud = true;
                UpdateSession(false);
            }
        }
        #endregion
    }
}
