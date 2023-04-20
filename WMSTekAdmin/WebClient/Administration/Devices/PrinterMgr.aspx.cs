using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Entities.Label;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class PrinterMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Printer> printerViewDTO = new GenericViewDTO<Printer>();
        private GenericViewDTO<LabelPrint> labelPrintViewDTO = new GenericViewDTO<LabelPrint>();
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

                        tabPrinterFeature.HeaderText = lbltabPrinter.Text;
                        tabLabel.HeaderText = this.lbltabLabel.Text;

                        if (isValidViewDTO)
                        {
                            PopulateLists();
                        }
                    }

                    if (ValidateSession(WMSTekSessions.PrinterMgr.PrinterList))
                    {
                        printerViewDTO = (GenericViewDTO<Printer>)Session[WMSTekSessions.PrinterMgr.PrinterList];
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
            }
        }

        protected void btnAddLabel_Click(object sender, EventArgs e)
        {
            try
            {
                bool labelFind = false;
                if (this.ddlLabel.SelectedValue != "-1")
                {
                    if (ValidateSession(WMSTekSessions.PrinterMgr.LabelPrintList))
                    {
                        labelPrintViewDTO = (GenericViewDTO<LabelPrint>)Session[WMSTekSessions.PrinterMgr.LabelPrintList];

                        foreach (var item in labelPrintViewDTO.Entities)
                        {
                            if (item.IdLabel.ToString() == this.ddlLabel.SelectedValue)
                            {
                                labelFind = true;
                                ucStatus.ShowMessage(lblLabelAsig.Text);
                            }
                        }
                    }
                    if (!labelFind)
                    {
                        AddLabelPrinter();
                    }
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
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
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
            }
        }

        protected void grdLabels_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //Calcula la posicion en el ViewDTO de la fila a eliminar
                RemoveLabel(e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
            }
        }

        protected void grdLabels_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Select")
                {
                    int Id = (Convert.ToInt32(grdLabels.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                    labelPrintViewDTO = (GenericViewDTO<LabelPrint>)Session[WMSTekSessions.PrinterMgr.LabelPrintList];

                    //foreach (Warehouse warehouse in warehouseViewDTO.Entities)
                    //{
                    //    if (warehouse.Id == Id)
                    //    {
                    //        warehouse.IsDefault = true;
                    //        txtDefaultWhs.Text = "1";
                    //    }
                    //    else
                    //        warehouse.IsDefault = false;
                    //}

                    // Actualiza lista de Warehouses
                    Session.Add(WMSTekSessions.PrinterMgr.LabelPrintList, labelPrintViewDTO);

                    grdLabels.DataSource = labelPrintViewDTO.Entities;
                    grdLabels.DataBind();
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                }
            }
            catch (Exception ex)
            {
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
            }
        }

        protected void grdLabels_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
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
                this.Master.ucError.ShowError(printerViewDTO.Errors);
                printerViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            printerViewDTO = iDeviceMGR.FindAllPrinter(context);

            if (!printerViewDTO.hasError() && printerViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.PrinterMgr.PrinterList, printerViewDTO);
                Session.Remove(WMSTekSessions.Shared.PrinterList);

                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(printerViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(printerViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            base.LoadUserWarehouses(this.ddlWarehouse, this.Master.EmptyRowText, "-1", true);
            base.LoadPrintServers(this.ddlPrintServer, true, this.Master.EmptyRowText);
            base.LoadPrinterTypes(this.ddlPrinterType, true, this.Master.EmptyRowText);
            //base.LoadLabelId(this.ddlLabel, "", this.Master.EmptyRowText, true);
        }

        private void PopulateGridLabels(int index, int idPrinter)
        {
            try
            {
                if (idPrinter == -1)
                {
                    labelPrintViewDTO = new GenericViewDTO<LabelPrint>();
                    grdLabels.DataSource = labelPrintViewDTO.Entities;
                }
                else
                {
                    labelPrintViewDTO = iLabelMGR.LabelPrinter_GetByIdPrinter(idPrinter, context);
                    grdLabels.DataSource = labelPrintViewDTO.Entities;

                    ListItem item = new ListItem();

                    // Quita Etiquetas del drop-down list
                    foreach (LabelPrint labelPrint in labelPrintViewDTO.Entities)
                    {
                        item.Value = labelPrint.IdLabel.ToString();
                        item.Text = labelPrint.LabelName;

                        if (ddlLabel.Items.Contains(item))
                        {
                            ddlLabel.Items.Remove(item);
                        }
                    }
                }

                grdLabels.DataBind();

                // Actualiza lista de Etiquetas
                Session.Add(WMSTekSessions.PrinterMgr.LabelPrintList, labelPrintViewDTO);

            }
            catch (Exception ex)
            {
                printerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!printerViewDTO.hasConfigurationError() && printerViewDTO.Configuration != null && printerViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, printerViewDTO.Configuration);

            grdMgr.DataSource = printerViewDTO.Entities;
            grdMgr.DataBind();
            
            ucStatus.ShowRecordInfo(printerViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.warehouseVisible = true;
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
            base.LoadLabelId(this.ddlLabel, "", this.Master.EmptyRowText, true);

            // Editar Impresora
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = printerViewDTO.Entities[index].Id.ToString();

                this.txtName.Text = printerViewDTO.Entities[index].Name;
                this.txtDescription.Text = printerViewDTO.Entities[index].Description;
                this.ddlWarehouse.SelectedValue = (printerViewDTO.Entities[index].Warehouse.Id).ToString();
                this.ddlPrintServer.SelectedValue = printerViewDTO.Entities[index].PrintServer.Id.ToString();
                this.ddlPrinterType.SelectedValue = printerViewDTO.Entities[index].PrinterType.Id.ToString();

                // Carga grilla de Labels
                PopulateGridLabels(index, printerViewDTO.Entities[index].Id);

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva Impresora
            if (mode == CRUD.Create)
            {
                // Selecciona Warehouse seleccionados en el Filtro
                this.ddlWarehouse.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues[0].Value;

                hidEditId.Value = "0";
                txtName.Text = string.Empty;
                txtDescription.Text = string.Empty;
                this.ddlPrintServer.SelectedValue = "-1";
                this.ddlPrinterType.SelectedValue = "-1";
                lblNew.Visible = true;
                lblEdit.Visible = false;

                //Carga grilla de labels
                PopulateGridLabels(index, -1);
            }

            if (printerViewDTO.Configuration != null && printerViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(printerViewDTO.Configuration, true);
                else
                    base.ConfigureModal(printerViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void RemoveLabel(int index)
        {
            labelPrintViewDTO = (GenericViewDTO<LabelPrint>)Session[WMSTekSessions.PrinterMgr.LabelPrintList];

            // Agrega Label eliminado al drop down list
            ddlLabel.Items.Add(new ListItem(labelPrintViewDTO.Entities[index].LabelName, labelPrintViewDTO.Entities[index].IdLabel.ToString()));
            base.AlphabeticalOrderDropDownList(ddlLabel);

            //idLabelDelete = labelPrintViewDTO.Entities[index].IdLabel;

            // Quita el Label seleccionado de la grilla
            labelPrintViewDTO.Entities.RemoveAt(index);
            grdLabels.DataSource = labelPrintViewDTO.Entities;
            grdLabels.DataBind();

            //Actualiza lista de Labels
            Session.Add(WMSTekSessions.PrinterMgr.LabelPrintList, labelPrintViewDTO);

        }

        protected void SaveChanges()
        {
            Printer printer = new Printer(Convert.ToInt32(hidEditId.Value));
            printer.Warehouse = new Warehouse();
            printer.PrintServer = new PrintServer();
            printer.PrinterType = new PrinterType();

            printer.Name = txtName.Text.Trim();
            printer.Description = txtDescription.Text.Trim();
            printer.Warehouse.Id = Convert.ToInt32(ddlWarehouse.SelectedValue);                        
            printer.PrintServer.Id = int.Parse(this.ddlPrintServer.SelectedValue);
            printer.PrinterType.Id = int.Parse(this.ddlPrinterType.SelectedValue);

            labelPrintViewDTO = (GenericViewDTO<LabelPrint>)Session[WMSTekSessions.PrinterMgr.LabelPrintList];

            if (hidEditId.Value == "0")
            {
                printerViewDTO = iDeviceMGR.MaintainPrinter(CRUD.Create, printer, labelPrintViewDTO, context);
            }
            else
            {
                printerViewDTO = iDeviceMGR.MaintainPrinter(CRUD.Update, printer, labelPrintViewDTO, context);
            }
                
            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (printerViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(printerViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            printerViewDTO = iDeviceMGR.MaintainPrinter(CRUD.Delete, printerViewDTO.Entities[index], new GenericViewDTO<LabelPrint>(), context);

            if (printerViewDTO.hasError())
                UpdateSession(true);
            else
            {
                ucStatus.ShowMessage(printerViewDTO.MessageStatus.Message);
                crud = true;
                UpdateSession(false);
            }
        }

        protected void AddLabelPrinter()
        {
            if (this.ddlLabel.SelectedIndex > 0)
            {
                labelPrintViewDTO = (GenericViewDTO<LabelPrint>)Session[WMSTekSessions.PrinterMgr.LabelPrintList];

                if (labelPrintViewDTO == null)
                    labelPrintViewDTO = new GenericViewDTO<LabelPrint>();

                LabelPrint labelPrint = new LabelPrint();
                labelPrint.IdLabel = Convert.ToInt16(ddlLabel.SelectedValue);
                labelPrint.LabelName = ddlLabel.SelectedItem.Text;

                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + currentPage;
                labelPrint.IdPrinter = printerViewDTO.Entities[editIndex].Id;



                //Agrega etiqueta seleccionado a la grilla
                labelPrintViewDTO.Entities.Add(labelPrint);
                grdLabels.DataSource = labelPrintViewDTO.Entities;
                grdLabels.DataBind();

                // Quita etiqueta seleccionado de la lista de Etiquetas a Asignar (drop-down list)
                ddlLabel.Items.RemoveAt(ddlLabel.SelectedIndex);

                // Actualiza lista de Labels
                Session.Add(WMSTekSessions.PrinterMgr.LabelPrintList, labelPrintViewDTO);

            }
        }
        #endregion

        
    }
}
