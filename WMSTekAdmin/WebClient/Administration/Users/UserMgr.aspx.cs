using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.I18N;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Profile
{
    public partial class UserMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();
        private GenericViewDTO<Owner> ownerViewDTO;
        private GenericViewDTO<WorkZone> workZoneViewDTO;
        private GenericViewDTO<Printer> printerViewDTO;
        private GenericViewDTO<Warehouse> warehouseViewDTO;
        private GenericViewDTO<WorkZone> workZoneListGlobal = new GenericViewDTO<WorkZone>();
        private GenericViewDTO<Vendor> vendorViewDTO;
        private bool isValidViewDTO = false;

        public int idWhsAdd
        {
            get
            {
                if (ValidateViewState("idWhsAdd"))
                    return (int)ViewState["idWhsAdd"];
                else
                    return 0;
            }

            set { ViewState["idWhsAdd"] = value; }
        }
        public int idWarehouseDelete
        {
            get
            {
                if (ValidateViewState("idWarehouseDelete"))
                    return (int)ViewState["idWarehouseDelete"];
                else
                    return 0;
            }

            set { ViewState["idWarehouseDelete"] = value; }
        }

        // Propiedad para controlar el nro de pagina activa en la grilla
        public int currentPage
        {
            get 
            {
                if (ValidateViewState("pageUserMgr"))
                    return (int)ViewState["pageUserMgr"];
                else
                    return 0;
            }

            set { ViewState["pageUserMgr"] = value; }
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
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        // Carga inicial del ViewDTO
                        UpdateSession(false);
                        PopulateLists();

                        tabLayout.HeaderText = lbltabLayout.Text;
                        tabWarehouse.HeaderText = this.lbltabWarehouse.Text;
                        tabWorkZone.HeaderText = this.lbltabWorkZone.Text;
                        tabPrinter.HeaderText = this.lbltabPrinter.Text;
                        tabOwners.HeaderText = this.lbltabOwners.Text;
                        tabVendors.HeaderText = this.lblTabVendors.Text;
                    }

                    if (ValidateSession(WMSTekSessions.UserMgr.UserList))
                    {
                        userViewDTO = (GenericViewDTO<User>)Session[WMSTekSessions.UserMgr.UserList];
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
        //        userViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(userViewDTO.Errors);
        //    }
        //}

        /// <summary>
        /// Persiste los cambios en la entidad (nueva o editada)
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
                currentIndex = -1;
                //PopulateLists();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        /// <summary>
        /// Abre la ventana modal para crear una nueva entidad
        /// </summary>
        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {

                if (context.SessionInfo.User.UserName.ToUpper().Equals("BASE"))
                {
                    // Si el usuario es el BASE valida que tenga asociado un CD antes de poder Crear otros Usuarios
                    // 15-05-2015
                    GenericViewDTO<Warehouse> whsList = new GenericViewDTO<Warehouse>();
                    whsList = iLayoutMGR.GetWarehouseByUser(context.SessionInfo.User.Id, context); ;

                    if (whsList.Entities == null || whsList.Entities.Count < 1)
                    {
                        whsList.Errors = new ErrorDTO();
                        whsList.Errors.Level = ErrorLevel.Info;
                        whsList.Errors.Message = this.lblMessajeUserBaseWhs.Text + "''"+ context.SessionInfo.User.UserName +"''" ;
                        whsList.Errors.Title = this.lblInfoMessaje.Text;
                        this.Master.ucError.ShowError(whsList.Errors);
                        return;
                    }
                }

                divWarning.Visible = false;
                currentIndex = -1;
                ShowModal(0, CRUD.Create);
                
           }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null)
                    {
                        btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";
                    }

                    // Deshabilita la opcion de Eliminar si es el Usuario Base
                    if (btnDelete != null && userViewDTO.Entities[e.Row.DataItemIndex].IsBaseUser)
                    {
                        btnDelete.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_delete_dis.png";
                        btnDelete.Enabled = false;
                    }

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
           }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;
                divWarning.Visible = false;
                currentIndex = editIndex;
                //base.LoadForeman(this.ddlForeman, true, this.Master.EmptyRowText);
                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
                currentIndex = -1;
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    currentIndex = grdMgr.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void grdWarehouses_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //Calcula la posicion en el ViewDTO de la fila a eliminar
                RemoveWarehouse(e.RowIndex);
                RemoverWorkZoneList();
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void grdWarehouses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Select")
                {
                    int Id = (Convert.ToInt32(grdWarehouses.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                    warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];

                    foreach (Warehouse warehouse in warehouseViewDTO.Entities)
                    {
                        if (warehouse.Id == Id)
                        {
                            warehouse.IsDefault = true;
                            txtDefaultWhs.Text = "1";
                        }
                        else
                            warehouse.IsDefault = false;
                    }

                    // Actualiza lista de Warehouses
                    Session.Add(WMSTekSessions.UserMgr.WarehouseList, warehouseViewDTO);

                    grdWarehouses.DataSource = warehouseViewDTO.Entities;
                    grdWarehouses.DataBind();
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                }
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void grdOwner_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Select")
                {
                    ownerViewDTO = (GenericViewDTO<Owner>)Session[WMSTekSessions.UserMgr.OwnerList];

                    int Id = (Convert.ToInt32(grdOwner.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                    // Marca el Owner seleccionado como 'por defecto' para el Usuario
                    foreach (Owner owner in ownerViewDTO.Entities)
                    {
                        if (owner.Id == Id)
                        {
                            owner.IsDefault = true;
                            txtDefaultOwner.Text = "1";
                        }
                        else
                            owner.IsDefault = false;
                    }

                    // Actualiza lista de Owners
                    Session.Add(WMSTekSessions.UserMgr.OwnerList, ownerViewDTO);
                    grdOwner.DataSource = ownerViewDTO.Entities;
                    grdOwner.DataBind();
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                }
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }
        protected void grdVendor_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Select")
                {
                    vendorViewDTO = (GenericViewDTO<Vendor>)Session[WMSTekSessions.UserMgr.VendorList];

                    // Actualiza lista de vendors
                    Session.Add(WMSTekSessions.UserMgr.VendorList, vendorViewDTO);
                    grdVendor.DataSource = vendorViewDTO.Entities;
                    grdVendor.DataBind();
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                }
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }
        protected void grdOwner_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //Calcula la posicion en el ViewDTO de la fila a eliminar
                RemoveOwner(e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }
        protected void grdVendor_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //Calcula la posicion en el ViewDTO de la fila a eliminar
                RemoveVendor(e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }
        protected void grdPrinter_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Select")
                {
                    printerViewDTO = (GenericViewDTO<Printer>)Session[WMSTekSessions.UserMgr.PrinterList];

                    int Id = (Convert.ToInt32(grdPrinter.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                    foreach (Printer printer in printerViewDTO.Entities)
                    {
                        if (printer.Id == Id)
                        {
                            printer.IsDefault = true;
                            txtDefaultPrinter.Text = "1";
                        }
                        else
                            printer.IsDefault = false;
                    }

                    // Actualiza lista de Impresoras
                    Session.Add(WMSTekSessions.UserMgr.PrinterList, printerViewDTO);

                    grdPrinter.DataSource = printerViewDTO.Entities;
                    grdPrinter.DataBind();
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                }
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void grdPrinter_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //Calcula la posicion en el ViewDTO de la fila a eliminar
                RemovePrinter(e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void grdWorkZone_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //Calcula la posicion en el ViewDTO de la fila a eliminar
                RemoveWorkZone(e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void btnAddWarehouse_Click(object sender, EventArgs e)
        {
           try
            {
                bool warehouseFind = false;
                if (this.ddlWarehouse.SelectedValue != "-1")
                {
                    if (ValidateSession(WMSTekSessions.UserMgr.WarehouseList))
                    {
                        warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];

                        foreach (var item in warehouseViewDTO.Entities)
                        {
                            if (item.Id.ToString() == this.ddlWarehouse.SelectedValue)
                            {
                                warehouseFind = true;
                                ucStatus.ShowMessage(lblWarehouseAsig.Text);
                            }
                        }
                    }
                    if (!warehouseFind)
                    {
                        AddWarehouse();
                        LoadWorkZonesByWarehouse();
                        LoadPrintersByListWarehouse(true, this.Master.EmptyRowText);
                        
                    }
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }
        
        protected void btnAddOwner_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlOwners.SelectedValue != "-1")
                {
                    AddOwner();
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void btnAddZone_Click(object sender, EventArgs e)
        {
           try
           {
                if (this.ddlZone.SelectedValue != "-1")
                {
                    AddWorkZone();
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void btnAddPrinter_Click(object sender, EventArgs e)
        {
           try
           {
                if (this.ddlPrinters.SelectedValue != "-1")
                {
                    AddPrinter();
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
           }
           catch (Exception ex)
           {
               userViewDTO.Errors = baseControl.handleException(ex, context);
               this.Master.ucError.ShowError(userViewDTO.Errors);
           }
        }
        protected void btnAddVendor_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlVendors.SelectedValue != "-1")
                {
                    AddVendor();
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void ddlTypeUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var list = (DropDownList)sender;
                var value = list.SelectedValue;

                if (value == Constants.TYPE_USER_WS)
                    ShowOrHideElementsByTypeUser(false);
                else
                    ShowOrHideElementsByTypeUser(true);

                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }
        private void ShowOrHideElementsByTypeUser(bool show)
        {
            divWorkPhone.Visible = show;
            divEmail.Visible = show;
            divLanguage.Visible = show;
            divMobilePhone.Visible = show;
            divHousePhone.Visible = show;
            divUserInternalCode.Visible = show;
            divForeman.Visible = show;
            divComment.Visible = show;

            tabWarehouse.Visible = show;
            tabWorkZone.Visible = show;
            tabPrinter.Visible = show;
            tabVendors.Visible = show;
        }
        #endregion

        #region "Métodos"

        public void Initialize()
        {
            context.SessionInfo.IdPage = "UserMgr";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }

        /// <summary>
        /// Carga en sesion lista de Usuarios
        /// </summary>
        /// <param name="showError"></param>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(userViewDTO.Errors);
                userViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            if (context.SessionInfo.User.UserName.ToUpper().Equals("BASE"))
            {
                userViewDTO = iProfileMGR.FindAllUserBase(context);
            }
            else
            {
                userViewDTO = iProfileMGR.FindAllUser(context);
            }

            if (!userViewDTO.hasError() && userViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.UserMgr.UserList, userViewDTO);
                Session.Remove(WMSTekSessions.Shared.UserList);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(userViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = true;
            if (context.SessionInfo.User.UserName.ToUpper().Equals("BASE"))
            {
                this.Master.ucMainFilter.warehouseNotIncludeAll = false;
            }
            else
            {
                this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            }
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.statusVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.codeLabel = lblFilterCode.Text;
            this.Master.ucMainFilter.descriptionLabel = lblFilterDescription.Text;

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

        protected void PopulateLists()
        {
            //Lista de Idiomas
            base.LoadLanguage(this.ddlLanguage, true, this.Master.EmptyRowText);

            //Lista de Capataces
            //base.LoadForeman(this.ddlForeman, true, this.Master.EmptyRowText);
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            //Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!userViewDTO.hasConfigurationError() && userViewDTO.Configuration != null && userViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, userViewDTO.Configuration);

            grdMgr.DataSource = userViewDTO.Entities;
            grdMgr.DataBind();
            grdMgr.ShowFooter = false;

            ucStatus.ShowRecordInfo(userViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateGridWarehouses(int index, int idUser)
        {
            try
            {
                if (idUser == -1)
                {
                    warehouseViewDTO = new GenericViewDTO<Warehouse>(); // null;
                    grdWarehouses.DataSource = null;
                }
                else
                {
                    warehouseViewDTO = iLayoutMGR.GetWarehouseByUser(idUser, context);
                    grdWarehouses.DataSource = warehouseViewDTO.Entities;

                    ListItem item = new ListItem();

                    // Quita Warehouses del usuario del drop-down list
                    foreach (Warehouse whs in warehouseViewDTO.Entities)
                    {
                        item.Value = whs.Id.ToString();
                        item.Text = whs.ShortName;

                        if (ddlWarehouse.Items.Contains(item))
                        {
                            ddlWarehouse.Items.Remove(item);                            
                        }
                    }
                }

                grdWarehouses.DataBind();

                // Actualiza lista de Warehouses
                Session.Add(WMSTekSessions.UserMgr.WarehouseList, warehouseViewDTO);

                //Dependiendo de los warehouses, carga lista de WorkZones
                if (userViewDTO.Entities.Count > 0)
                    InicializeListWorkZoneByWarehouses(warehouseViewDTO, userViewDTO.Entities[index].Id);
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        private void InicializeListWorkZoneByWarehouses(GenericViewDTO<Warehouse> warehouseViewDTO, int IdUser)
        {
            try
            {
                if (IdUser != 1)
                {
                    //Inicializa variables
                    workZoneListGlobal = new GenericViewDTO<WorkZone>();
                    GenericViewDTO<WorkZone> workZoneViewDTOLocal = new GenericViewDTO<WorkZone>();
                    workZoneViewDTO = new GenericViewDTO<WorkZone>();
                    GenericViewDTO<WorkZone> workZoneListDDL = new GenericViewDTO<WorkZone>();

                    int typeZone = (int)TypeWorkZone.Usuario;

                    //Recorre los centros para consultar por sus warehouses
                    foreach (Warehouse warehouse in warehouseViewDTO.Entities)
                    {
                        //Busca los workzones de acuerdo a los warehouses
                        workZoneViewDTOLocal = new GenericViewDTO<WorkZone>();
                        //workZoneViewDTOLocal = iLayoutMGR.GetWorkZoneByWhs(warehouse.Id, context);
                        workZoneViewDTOLocal = iLayoutMGR.GetWorkZoneByWhsAndTypeZone(warehouse.Id, typeZone, context);

                        foreach (WorkZone workZone in workZoneViewDTOLocal.Entities)
                        {
                            workZoneListGlobal.Entities.Add(workZone);
                        }
                    }

                    if (workZoneListGlobal == null)
                        workZoneListGlobal = new GenericViewDTO<WorkZone>();

                    //Agrega las zonas a una lista global
                    //foreach (WorkZone workZone in workZoneViewDTOLocal.Entities)
                    //{
                    //    workZoneListGlobal.Entities.Add(workZone);
                    //}
                    //Carga el DropDownList con la lista de Zonas
                    LoadWorkZoneByWorkZoneList(this.ddlZone, true, workZoneListGlobal, this.Master.EmptyRowText);

                    // Actualiza lista de WorkZones
                    Session.Add(WMSTekSessions.UserMgr.WorkZoneListGlobal, workZoneListGlobal);
                }
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        private void InicializeListWorkZoneByIdWhs (int IdWhs)
        {
            try
            {
                if (IdWhs != 0)
                {
                    //Inicializa variables
                    workZoneListGlobal = new GenericViewDTO<WorkZone>();
                    int typeZone = (int)TypeWorkZone.Usuario;

                    //Busca los workzones de acuerdo a los warehouses
                    //(workZoneListGlobal = iLayoutMGR.GetWorkZoneByWhs(IdWhs, context);
                    workZoneListGlobal = iLayoutMGR.GetWorkZoneByWhsAndTypeZone(IdWhs, typeZone, context);

                    //Carga el DropDownList con la lista de Zonas
                    LoadWorkZoneByWorkZoneList(this.ddlZone, true, workZoneListGlobal, this.Master.EmptyRowText);

                    // Actualiza lista de WorkZones
                    Session.Add(WMSTekSessions.UserMgr.WorkZoneListGlobal, workZoneListGlobal);
                }
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        private void PopulateGridOwners(int index, int idUser)
        {
            if (idUser == -1)
            {
                ownerViewDTO = null;
                grdOwner.DataSource = null;
            }
            else
            {
                ownerViewDTO = iWarehousingMGR.GetOwnersByUser(context, idUser);
                grdOwner.DataSource = ownerViewDTO.Entities;

                ListItem item = new ListItem();

                // Quita Owners del usuario del drop-down list
                foreach (Owner owner in ownerViewDTO.Entities)
                {
                    item.Value = owner.Id.ToString();
                    item.Text = owner.Name;

                    if (ddlOwners.Items.Contains(item))
                        ddlOwners.Items.Remove(item);
                }
            }

            grdOwner.DataBind();

            // Actualiza lista de Warehouses
            Session.Add(WMSTekSessions.UserMgr.OwnerList, ownerViewDTO);
        }
        private void PopulateGridVendors(int index, int idUser)
        {
            if (idUser == -1)
            {
                vendorViewDTO = null;
                grdVendor.DataSource = null;
            }
            else
            {
                vendorViewDTO = iWarehousingMGR.GetVendorsByUser(context, idUser);
                grdVendor.DataSource = vendorViewDTO.Entities;

                ListItem item = new ListItem();

                // Quita Owners del usuario del drop-down list
                foreach (Vendor vendor in vendorViewDTO.Entities)
                {
                    item.Value = vendor.Id.ToString();
                    item.Text = vendor.Name;

                    if (ddlVendors.Items.Contains(item))
                        ddlVendors.Items.Remove(item);
                }
            }

            grdVendor.DataBind();

            // Actualiza lista de Warehouses
            Session.Add(WMSTekSessions.UserMgr.VendorList, vendorViewDTO);
        }
        private void PopulateGridWorkZones(int index, int idUser)
        {
            try
            {
                if (idUser == -1)
                {
                    workZoneViewDTO = null;
                    grdWorkZone.DataSource = null;
                }
                else
                {
                    if (ValidateSession(WMSTekSessions.UserMgr.WorkZoneList))
                        workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.UserMgr.WorkZoneList];

                    if (workZoneViewDTO != null)
                    {
                        if (workZoneViewDTO.Entities.Count == 0)
                        {
                            workZoneViewDTO = iLayoutMGR.GetWorkZoneByUser(context, idUser);
                        }
                    }
                    else
                    {
                        workZoneViewDTO = iLayoutMGR.GetWorkZoneByUser(context, idUser);
                    }


                    grdWorkZone.DataSource = workZoneViewDTO.Entities;

                    ListItem item = new ListItem();

                    // Quita WorkZones del usuario del drop-down list
                    foreach (WorkZone workzone in workZoneViewDTO.Entities)
                    {
                        item.Value = workzone.Id.ToString();
                        item.Text = workzone.Name;

                        if (ddlZone.Items.Contains(item))
                            ddlZone.Items.Remove(item);
                    }
                }

                grdWorkZone.DataBind();

                // Actualiza lista de WorkZones
                Session.Add(WMSTekSessions.UserMgr.WorkZoneList, workZoneViewDTO);
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        private void PopulateGridPrinters(int index, int idUser)
        {
            if (idUser == -1)
            {
                printerViewDTO = null;
                grdPrinter.DataSource = null;
            }
            else
            {
                printerViewDTO = iDeviceMGR.GetPrinterByUser(context, idUser);
                grdPrinter.DataSource = printerViewDTO.Entities;

                ListItem item = new ListItem();

                // Quita Printers del usuario del drop-down list
                foreach (Printer printer in printerViewDTO.Entities)
                {
                    item.Value = printer.Id.ToString();
                    item.Text = printer.Name;

                    if (ddlPrinters.Items.Contains(item))
                        ddlPrinters.Items.Remove(item);
                }
            }

            grdPrinter.DataBind();

            //Actualiza lista de Warehouses
            Session.Add(WMSTekSessions.UserMgr.PrinterList, printerViewDTO);
        }

        protected void ReloadData()
        {
            crud = false;
            divWarning.Visible = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentIndex = -1;
                currentPage = 0;
                this.Master.ucError.ClearError();
                PopulateLists();
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            tabUser.ActiveTabIndex = 0;

            //Configura ventana modal

            Session.Remove(WMSTekSessions.UserMgr.WorkZoneList);
            Session.Remove(WMSTekSessions.UserMgr.WorkZoneListGlobal);
            Session.Remove(WMSTekSessions.Shared.OwnerList);

            if (userViewDTO.Configuration != null && userViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                {
                    base.ConfigureModal(userViewDTO.Configuration, true);

                    Session.Remove(WMSTekSessions.UserMgr.WarehouseList);
                    //Session.Remove(WMSTekSessions.UserMgr.OwnerList);
                    Session.Remove(WMSTekSessions.UserMgr.PrinterList);

                    LoadWorkZoneByWorkZoneList(this.ddlZone, true, workZoneListGlobal, this.Master.EmptyRowText);
                    //base.ConfigureModal(userViewDTO.Configuration, true);

                    //Session.Remove(WMSTekSessions.UserMgr.WarehouseList);
                    //Session.Remove(WMSTekSessions.UserMgr.OwnerList);
                    //Session.Remove(WMSTekSessions.UserMgr.PrinterList);
                    //Session.Remove(WMSTekSessions.UserMgr.WorkZoneList);
                    Session.Remove(WMSTekSessions.Shared.WarehouseList);
                    //Session.Remove(WMSTekSessions.Shared.OwnerList);
                    //Session.Remove(WMSTekSessions.Shared.WorkZoneList);
                    //Session.Remove(WMSTekSessions.Shared.PrinterList);

                    base.LoadOwner(this.ddlOwners, true, this.Master.EmptyRowText);
                    base.LoadVendor(this.ddlVendors, true, this.Master.EmptyRowText);
                    ////Lista de Printers
                    //base.LoadPrinters(this.ddlPrinters, true, this.Master.EmptyRowText);

                    //Se limpia filtro para evitar que se busquen bodegas con un where en WhsCode usando el nombre del usuario a buscar en la web
                    string userFilter = string.Empty;
                    var filterNameUser = context.MainFilter.Where(filter => filter.Name.Equals("Code")).First();
                    if (filterNameUser != null)
                    {
                        if (filterNameUser.FilterValues != null && filterNameUser.FilterValues.Count > 0)
                        {
                            userFilter = filterNameUser.FilterValues.First().Value;
                            filterNameUser.FilterValues = new List<FilterItem>();
                        } 
                    }

                    EntityFilter filterName = null;
                    string valueFilterName = null;

                    if (context.MainFilter.Exists(filter => filter.Name == "Name"))
                    {
                        filterName = context.MainFilter.Where(f => f.Name == "Name").First();

                        if (filterName.FilterValues.Count > 0)
                        {
                            valueFilterName = filterName.FilterValues.FirstOrDefault().Value;
                            filterName.FilterValues.FirstOrDefault().Value = string.Empty;
                        }
                    }

                    //Lista de Centros
                    base.LoadWarehouses(this.ddlWarehouse, true, this.Master.EmptyRowText, "-1");

                    if (filterName != null && !string.IsNullOrEmpty(valueFilterName))
                    {
                        filterName.FilterValues.FirstOrDefault().Value = valueFilterName;
                    }

                    //Se restaura filtro con nombre de usuario
                    if (!string.IsNullOrEmpty(userFilter))
                    {
                        filterNameUser.FilterValues.Add(new FilterItem() { Name = "Code", Value = userFilter });
                    }

                    ////Lista de Capataces
                    base.LoadForeman(this.ddlForeman, true, this.Master.EmptyRowText);
                    ////Dependiendo del warehouse, carga lista de WorkZones
                    //InicializeListWorkZoneByIdWhs(this.Master.ucMainFilter.idWhs);
                }
                else
                {
                    //Configura PopUp
                    base.ConfigureModal(userViewDTO.Configuration, false);
                }
            }

            // Editar usuario
            if (mode == CRUD.Update)
            {
                if (string.IsNullOrEmpty(userViewDTO.Entities[index].TypeUser) || userViewDTO.Entities[index].TypeUser == Constants.TYPE_USER_TEK)
                {
                    ddlTypeUser.SelectedValue = Constants.TYPE_USER_TEK;
                    ShowOrHideElementsByTypeUser(true);
                }
                else if (userViewDTO.Entities[index].TypeUser == Constants.TYPE_USER_WS)
                {
                    ddlTypeUser.SelectedValue = userViewDTO.Entities[index].TypeUser;
                    ShowOrHideElementsByTypeUser(false);
                }

                base.LoadForeman(this.ddlForeman, true, this.Master.EmptyRowText);
                
                //Recupera los datos de la entidad a editar
                hidEditId.Value = userViewDTO.Entities[index].Id.ToString();

                //Carga controles
                chkCodStatus.Checked = userViewDTO.Entities[index].CodStatus;
                txtUserName.Text = userViewDTO.Entities[index].UserName;
                txtFirstName.Text = userViewDTO.Entities[index].FirstName;
                txtLastName.Text = userViewDTO.Entities[index].LastName;
                txtWorkPhone.Text = userViewDTO.Entities[index].WorkPhone;
                txtEmail.Text = userViewDTO.Entities[index].Email;
                ddlLanguage.SelectedValue = (userViewDTO.Entities[index].Language.Id).ToString();
                txtMobilePhone.Text = userViewDTO.Entities[index].MobilePhone;
                txtHousePhone.Text = userViewDTO.Entities[index].HousePhone;
                txtUserInternalCode.Text = userViewDTO.Entities[index].UserInternalCode;
                if (userViewDTO.Entities[index].Foreman.Id > 0)
                {
                    if (ddlForeman.Items.FindByValue((userViewDTO.Entities[index].Foreman.FirstName.ToString())) != null)
                    {
                        ddlForeman.SelectedValue = (userViewDTO.Entities[index].Foreman.Id).ToString();
                    }
                }
                txtComment.Text = userViewDTO.Entities[index].Comment;

                lblNew.Visible = false;
                lblEdit.Visible = true;

                // Si es el Usuario Base, deshabilita la edicion de la opcion 'CodStatus'
                if (userViewDTO.Entities[index].IsBaseUser)
                    chkCodStatus.Enabled = false;
                else
                    chkCodStatus.Enabled = true;

                // Carga grilla de centros
                PopulateGridWarehouses(index, userViewDTO.Entities[index].Id);

                // Carga grilla de Owners
                PopulateGridOwners(index, userViewDTO.Entities[index].Id);

                // Carga grilla de Vendors
                PopulateGridVendors(index, userViewDTO.Entities[index].Id);

                // Carga grilla de WorkZones y //Carga Variable Global de zonas para llenar el drop down list
                PopulateGridWorkZones(index, userViewDTO.Entities[index].Id);
               
                //Lista de Printers
                base.LoadPrinters(this.ddlPrinters, true, this.Master.EmptyRowText);
                // Extrae variable del mainfilter del contexto CODE para generar las consultas             
                context.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();
                //Lista de Owners
                //base.LoadOwner(this.ddlOwners, true, this.Master.EmptyRowText);
                base.LoadOwnerNew(this.ddlOwners, true, this.Master.EmptyRowText);
                base.LoadVendorNew(this.ddlVendors, true, this.Master.EmptyRowText);
                //Lista de Centros  
                LoadWarehousesNew(this.ddlWarehouse, true, this.Master.EmptyRowText, "-1");

                //Obtiene la variable para el mainfilter CODE del contexto
                TextBox txtFilterCode = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterCode");       
                context.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Add(new FilterItem(txtFilterCode.Text.Trim()));

                GenericViewDTO<WorkZone> workZoneViewDTONew = new GenericViewDTO<WorkZone>();
                if (ValidateSession(WMSTekSessions.UserMgr.WorkZoneList))
                {
                    workZoneViewDTONew = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.UserMgr.WorkZoneList];
                    if (workZoneViewDTONew.Entities.Count > 0)
                    {
                        foreach (var item in workZoneViewDTONew.Entities)
                        {
                            if (workZoneListGlobal.Entities.Exists(w => w.Id.Equals(item.Id)))
                            {
                                workZoneListGlobal.Entities.Remove(workZoneListGlobal.Entities.Find(w => w.Id.Equals(item.Id)));
                            }
                        }
                    }
                }

                LoadWorkZoneByWorkZoneList(this.ddlZone, true, workZoneListGlobal, this.Master.EmptyRowText);

                // Carga grilla de Printers
                PopulateGridPrinters(index, userViewDTO.Entities[index].Id);
            }

            // Nueva usuario
            if (mode == CRUD.Create)
            {
                hidEditId.Value = "0";
                txtUserName.Text = string.Empty;
                txtFirstName.Text = string.Empty;
                txtLastName.Text = string.Empty;
                txtWorkPhone.Text = string.Empty;
                txtEmail.Text = string.Empty;
                ddlLanguage.SelectedValue = "-1";
                txtMobilePhone.Text = string.Empty;
                txtHousePhone.Text = string.Empty;
                txtUserInternalCode.Text = string.Empty;
                ddlForeman.SelectedValue = "-1";
                ddlWarehouse.SelectedValue = "-1";
                ddlOwners.SelectedValue = "-1";
                ddlVendors.SelectedValue = "-1";
                ddlPrinters.SelectedValue = "-1";
                ddlZone.SelectedValue = "-1";
                txtComment.Text = string.Empty;
                this.chkCodStatus.Checked = true;
                ddlTypeUser.SelectedValue = Constants.TYPE_USER_TEK;
                ShowOrHideElementsByTypeUser(true);

                lblNew.Visible = true;
                lblEdit.Visible = false;

                //Carga grilla de centros
                PopulateGridWarehouses(index, -1);

                //Carga grilla de Owners
                //ddlOwners.Items.Clear();
                //ddlOwners.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));                
                PopulateGridOwners(index, -1);
                PopulateGridVendors(index, -1);
                //base.LoadOwnerNew(this.ddlOwners, true, this.Master.EmptyRowText);

                //Carga grilla de WorkZones
                PopulateGridWorkZones(index, -1);

                //Carga grilla de Printers
                ddlPrinters.Items.Clear();
                ddlPrinters.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));               
                PopulateGridPrinters(index, -1);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
         }

        public bool ExistWarehouseDefault()
        {
            bool bandera = true;

            warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];

            if (warehouseViewDTO != null)
            {
                foreach (Warehouse warehouse in warehouseViewDTO.Entities)
                {
                    if (warehouse.IsDefault == true)
                    {
                        bandera = true;
                        break;
                    }
                    else
                    {
                        bandera = false;
                    }
                }
            }
            else
            {
                bandera = true;
            }

            return bandera;
        }

        public bool ExistOwnerDefault()
        {
            bool bandera = true;

            ownerViewDTO = (GenericViewDTO<Owner>)Session[WMSTekSessions.UserMgr.OwnerList];
            if (ownerViewDTO != null)
            {
                foreach (Owner owner in ownerViewDTO.Entities)
                {
                    if (owner.IsDefault == true)
                    {
                        bandera = true;
                        break;
                    }
                    else
                    {
                        bandera = false;
                    }
                }
            }
            else
            {
                bandera = true;
            }
            return bandera;
        }

        public bool ExistPrinterDefault()
        {
            bool bandera = true;

            printerViewDTO = (GenericViewDTO<Printer>)Session[WMSTekSessions.UserMgr.PrinterList];
            if (printerViewDTO != null)
            {
                foreach (Printer printer in printerViewDTO.Entities)
                {
                    if (printer.IsDefault == true)
                    {
                        bandera = true;
                        break;
                    }
                    else
                    {
                        bandera = false;
                    }
                }
            }
            else
            {
                bandera = true;
            }
            return bandera;
        }

        public bool isChosenLanguage()
        {
            return ddlLanguage.SelectedValue != "-1";
        }

        /// <summary>
        /// Persiste los cambios en la entidad (modo Edit o New). 
        /// </summary>
        protected void SaveChanges()
        {
            // Verifica que haya un centro, owner y printer marcado por defecto
            // TODO: usar Owner por defecto?
            bool isUserWS = false;

            if (ddlTypeUser.SelectedValue == Constants.TYPE_USER_WS)
                isUserWS = true;

            if (isUserWS)
            {
                ownerViewDTO = (GenericViewDTO<Owner>)Session[WMSTekSessions.UserMgr.OwnerList];
                if (ownerViewDTO == null)
                {
                    this.modalPopUp.Show();
                    this.divWarning.Visible = true;
                    lblErrorMustSelectAnOwner.Visible = true;
                    this.isValidViewDTO = false;
                    return;
                }

                Session.Remove(WMSTekSessions.UserMgr.VendorList);
                Session.Remove(WMSTekSessions.UserMgr.WorkZoneList);
                Session.Remove(WMSTekSessions.UserMgr.PrinterList);
                Session.Remove(WMSTekSessions.UserMgr.WarehouseList);
            }

            if (!isUserWS && !ExistWarehouseDefault()){
                this.modalPopUp.Show();
                this.divWarning.Visible = true;
                this.lblErrorCDAsig.Visible = true;
                this.isValidViewDTO = false;
                
            }else if (!ExistOwnerDefault()){
                this.modalPopUp.Show();
                this.divWarning.Visible = true;
                this.lblErrorOwnerAsig.Visible = true;
                this.isValidViewDTO = false;

            }
            else if (!isUserWS && !ExistPrinterDefault())
            {
                this.modalPopUp.Show();
                this.divWarning.Visible = true;
                this.lblErrorPrintAsig.Visible = true;
                this.isValidViewDTO = false;

            }
            else if(!isUserWS && !isChosenLanguage())
            {
                this.modalPopUp.Show();
                this.divWarning.Visible = true;
                this.lblErrorLenguageAsig.Visible = true;
                this.isValidViewDTO = false;
            }
            else
            {

                //agrega los datos del usuario
                User user = new User();
                user.Language = new Language();
                user.Roles = new List<Role>();
                user.Foreman = new User();
                user.Id = Convert.ToInt32(hidEditId.Value);
                user.UserName = txtUserName.Text;
                user.FirstName = txtFirstName.Text;
                user.LastName = txtLastName.Text;
                user.IsBaseUser = false;
                user.CodStatus = this.chkCodStatus.Checked;
                user.WorkPhone = txtWorkPhone.Text;
                user.Email = txtEmail.Text;
                user.Language.Id = isUserWS == true ? -1 : Convert.ToInt32(ddlLanguage.SelectedValue);
                user.MobilePhone = txtMobilePhone.Text;
                user.HousePhone = txtHousePhone.Text;
                user.UserInternalCode = txtUserInternalCode.Text;
                user.Foreman.Id = isUserWS == true ? -1 : (Convert.ToInt32(ddlForeman.SelectedValue));
                user.Comment = txtComment.Text;
                user.TypeUser = ddlTypeUser.SelectedValue;

                //Cargar Owners asociados
                ownerViewDTO = (GenericViewDTO<Owner>)Session[WMSTekSessions.UserMgr.OwnerList];
                if (ownerViewDTO != null)
                    user.Owners = ownerViewDTO.Entities;

                //Cargar Vendors asociados
                vendorViewDTO = (GenericViewDTO<Vendor>)Session[WMSTekSessions.UserMgr.VendorList];
                if (vendorViewDTO != null)
                    user.Vendors = vendorViewDTO.Entities;

                //Cargar Zonas asociados
                workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.UserMgr.WorkZoneList];
                if (workZoneViewDTO != null)
                    user.WorkZones = workZoneViewDTO.Entities;

                //Cargar Impresoras asociadas
                printerViewDTO = (GenericViewDTO<Printer>)Session[WMSTekSessions.UserMgr.PrinterList];
                if (printerViewDTO != null)
                    user.Printers = printerViewDTO.Entities;

                //Cargar Warehouses asociadas
                warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];
                if (warehouseViewDTO != null)
                    user.Warehouses = warehouseViewDTO.Entities;

                //Valida que el usuario al crear o editar posee una bodega asignada como minimo
                if (!isUserWS && (warehouseViewDTO == null || warehouseViewDTO.Entities == null || warehouseViewDTO.Entities.Count < 1))
                {
                    crud = true;
                    //ucStatus.ShowMessage(userViewDTO.MessageStatus.Message);
                    //UpdateSession(false);
                    modalPopUp.Show();
                    isValidViewDTO = false;
                    divWarning.Visible = true;
                    lblErrorWhs.Visible = true;
                    lblErrorOwn.Visible = false;
                    lblErrorLenguageAsig.Visible = false;
                    return;
                }

                //Valida que el usuario al crear o editar posee un owner asignado como minimo
                if (ownerViewDTO == null || ownerViewDTO.Entities == null || ownerViewDTO.Entities.Count < 1)
                {
                    crud = true;
                    modalPopUp.Show();
                    isValidViewDTO = false;
                    divWarning.Visible = true;
                    lblErrorWhs.Visible = false;
                    lblErrorOwn.Visible = true;
                    lblErrorLenguageAsig.Visible = false;
                    return;
                }

                //Nuevo Usuario
                if (hidEditId.Value == "0")
                {
                    // Password por defecto
                    //int i = Convert.ToInt16(CfgParameterName.DefaultPassword);
                    user.Password = MiscUtils.Encrypt(GetCfgParameter(CfgParameterName.DefaultPassword.ToString()));

                    userViewDTO = iProfileMGR.MaintainUser(CRUD.Create, user, context);
                }
                // Editar Usuario
                else
                {
                    userViewDTO = iProfileMGR.MaintainUser(CRUD.Update, user, context);
                }

                divEditNew.Visible = false;
                modalPopUp.Hide();

                if (userViewDTO.hasError())
                {
                    UpdateSession(true);
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                }
                else
                {
                    crud = true;
                    ucStatus.ShowMessage(userViewDTO.MessageStatus.Message);
                    UpdateSession(false);
                }
            }
            //else
            //{
            //    modalPopUp.Show();
            //    isValidViewDTO = false;

            //    // TODO: mostrar error
            //}
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            userViewDTO = iProfileMGR.MaintainUser(CRUD.Delete, userViewDTO.Entities[index], context);

            if (userViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(userViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        protected void AddWarehouse()
        {
            if (this.ddlWarehouse.SelectedIndex > 0)
            {
                warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];

                //Setea lista de WorkZones por que se carga cuando busca las zonas por warehouses
                Session.Add(WMSTekSessions.UserMgr.WorkZoneListGlobal, null);

                if (warehouseViewDTO == null)
                    warehouseViewDTO = new GenericViewDTO<Warehouse>();

                Warehouse warehouse = new Warehouse(Convert.ToInt16(ddlWarehouse.SelectedValue));
                idWhsAdd = warehouse.Id;
                warehouse.ShortName = ddlWarehouse.SelectedItem.Text;

                //if (context.SessionInfo.User.UserName.ToUpper().Equals("BASE"))
                //{
                    if (warehouseViewDTO.Entities.Count < 1)
                    {
                        warehouse.IsDefault = true;
                    }
                //}


                //Agrega el Warehouse seleccionado a la grilla
                warehouseViewDTO.Entities.Add(warehouse);
                grdWarehouses.DataSource = warehouseViewDTO.Entities;
                grdWarehouses.DataBind();

                // Quita el Warehouse seleccionado de la lista de Warehouses a Asignar (drop-down list)
                ddlWarehouse.Items.RemoveAt(ddlWarehouse.SelectedIndex);

                // Actualiza lista de Warehouses
                Session.Add(WMSTekSessions.UserMgr.WarehouseList, warehouseViewDTO);

                // Valida si hay un Centro por defecto
                if(ExistWarehouseDefault())
                    txtDefaultWhs.Text = "1";
                else
                    txtDefaultWhs.Text = "-1";
            }
        }

        protected void AddOwner()
        {
            if (this.ddlOwners.SelectedIndex > 0)
            {
                ownerViewDTO = (GenericViewDTO<Owner>)Session[WMSTekSessions.UserMgr.OwnerList];

                if (ownerViewDTO == null)
                    ownerViewDTO = new GenericViewDTO<Owner>();

                Owner owner = new Owner(Convert.ToInt16(ddlOwners.SelectedValue));
                owner.Name = ddlOwners.SelectedItem.Text;

                if (ownerViewDTO.Entities.Count < 1)
                {
                    owner.IsDefault = true;
                }

                // Agrega el Onwer seleccionado a la grilla
                ownerViewDTO.Entities.Add(owner);
                grdOwner.DataSource = ownerViewDTO.Entities.OrderBy(ord=>ord.Name);
                grdOwner.DataBind();

                // Quita el Owner seleccionado de la lista de Owners a Asignar (drop-down list)
                ddlOwners.Items.RemoveAt(ddlOwners.SelectedIndex);

                // Actualiza lista de Owners
                Session.Add(WMSTekSessions.UserMgr.OwnerList, ownerViewDTO);

                // Valida si hay un Owner por defecto
                if (ExistOwnerDefault())
                    txtDefaultOwner.Text = "1";
                else
                    txtDefaultOwner.Text = "-1";
            }
        }
        protected void AddVendor()
        {
            if (this.ddlVendors.SelectedIndex > 0)
            {
                vendorViewDTO = (GenericViewDTO<Vendor>)Session[WMSTekSessions.UserMgr.VendorList];

                if (vendorViewDTO == null)
                    vendorViewDTO = new GenericViewDTO<Vendor>();

                var vendor = new Vendor(Convert.ToInt16(ddlVendors.SelectedValue));
                vendor.Name = ddlVendors.SelectedItem.Text;

                // Agrega el Onwer seleccionado a la grilla
                vendorViewDTO.Entities.Add(vendor);
                grdVendor.DataSource = vendorViewDTO.Entities.OrderBy(ord => ord.Name);
                grdVendor.DataBind();

                // Quita el Owner seleccionado de la lista de Owners a Asignar (drop-down list)
                ddlVendors.Items.RemoveAt(ddlVendors.SelectedIndex);

                // Actualiza lista de vendors
                Session.Add(WMSTekSessions.UserMgr.VendorList, vendorViewDTO);
            }
        }
        protected void AddPrinter()
        {
            if (this.ddlPrinters.SelectedIndex > 0)
            {
                printerViewDTO = (GenericViewDTO<Printer>)Session[WMSTekSessions.UserMgr.PrinterList];

                if (printerViewDTO == null)
                    printerViewDTO = new GenericViewDTO<Printer>();

                Printer printer = new Printer(Convert.ToInt16(ddlPrinters.SelectedValue));
                printer.Name = ddlPrinters.SelectedItem.Text;

                // Agrega Printer seleccionado a la grilla
                printerViewDTO.Entities.Add(printer);
                grdPrinter.DataSource = printerViewDTO.Entities;
                grdPrinter.DataBind();

                //Quita Printer de la lista de Printers a asignar (drop-down list)
                ddlPrinters.Items.RemoveAt(ddlPrinters.SelectedIndex);

                // Actualiza lista Printers
                Session.Add(WMSTekSessions.UserMgr.PrinterList, printerViewDTO);

                // Valida si hay una Impresora por defecto
                if (ExistPrinterDefault())
                    txtDefaultPrinter.Text = "1";
                else
                    txtDefaultPrinter.Text = "-1";
            }
        }
        
        protected void AddWorkZone()
        {
            bool ExistInGrid = false;
            if (this.ddlZone.SelectedIndex > 0)
            {
                workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.UserMgr.WorkZoneList];

                if(workZoneViewDTO == null)
                    workZoneViewDTO = new GenericViewDTO<WorkZone>();

                WorkZone workZone = new WorkZone(Convert.ToInt16(ddlZone.SelectedValue));
                workZone.Name = ddlZone.SelectedItem.Text;

                //Valida que el workZone no este en la Grilla
                
                foreach (WorkZone workzoneGrid in workZoneViewDTO.Entities)
                {
                    if (workzoneGrid.Id == workZone.Id)
                    {
                        ExistInGrid = true;
                        break;
                    }
                }
                if (!ExistInGrid)
                {
                    // Agrega Workzone seleccionado a la grilla
                    workZoneViewDTO.Entities.Add(workZone);
                    grdWorkZone.DataSource = workZoneViewDTO.Entities;
                    grdWorkZone.DataBind();

                    // Actualiza lista de WorkZones
                    Session.Add(WMSTekSessions.UserMgr.WorkZoneList, workZoneViewDTO);
                }
                // Quita Workzone seleccionado de la lista de Workzone a asignar (drop-down list)
                ddlZone.Items.RemoveAt(ddlZone.SelectedIndex);
            }
        }

        protected void RemoveWarehouse(int index)
        {
            warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];
            Session.Add(WMSTekSessions.UserMgr.WorkZoneListGlobal, null);

            // Agrega Warehouse eliminado al drop down list
            ddlWarehouse.Items.Add(new ListItem(warehouseViewDTO.Entities[index].ShortName, warehouseViewDTO.Entities[index].Id.ToString()));
            base.AlphabeticalOrderDropDownList(ddlWarehouse);

            idWarehouseDelete = warehouseViewDTO.Entities[index].Id;

            // Quita el Warehouse seleccionado de la grilla
            warehouseViewDTO.Entities.RemoveAt(index);
            grdWarehouses.DataSource = warehouseViewDTO.Entities;
            grdWarehouses.DataBind();

            //Actualiza lista de warehouses
            Session.Add(WMSTekSessions.UserMgr.WarehouseList, warehouseViewDTO);
            
            // Valida si hay un Centro por defecto
            if (ExistWarehouseDefault())
                txtDefaultWhs.Text = "1";
            else
                txtDefaultWhs.Text = "-1";
        }

        

        protected void RemoveOwner(int index)
        {
            ownerViewDTO = (GenericViewDTO<Owner>)Session[WMSTekSessions.UserMgr.OwnerList];

            // Agrega Owner eliminado al drop down list
            ddlOwners.Items.Add(new ListItem(ownerViewDTO.Entities[index].Name, ownerViewDTO.Entities[index].Id.ToString()));

            // Quita el Owner seleccionado de la grilla
            ownerViewDTO.Entities.RemoveAt(index);
            grdOwner.DataSource = ownerViewDTO.Entities;
            grdOwner.DataBind();

            //Actualiza lista de owners
            Session.Add(WMSTekSessions.UserMgr.OwnerList, ownerViewDTO);

            // Valida si hay un Owner por defecto
            if (ExistOwnerDefault())
                txtDefaultOwner.Text = "1";
            else
                txtDefaultOwner.Text = "-1";
        }
        protected void RemoveVendor(int index)
        {
            vendorViewDTO = (GenericViewDTO<Vendor>)Session[WMSTekSessions.UserMgr.VendorList];

            // Agrega Owner eliminado al drop down list
            ddlVendors.Items.Add(new ListItem(vendorViewDTO.Entities[index].Name, vendorViewDTO.Entities[index].Id.ToString()));

            // Quita el Owner seleccionado de la grilla
            vendorViewDTO.Entities.RemoveAt(index);
            grdVendor.DataSource = vendorViewDTO.Entities;
            grdVendor.DataBind();

            //Actualiza lista de owners
            Session.Add(WMSTekSessions.UserMgr.VendorList, vendorViewDTO);
        }
        protected void RemovePrinter(int index)
        {
            printerViewDTO = (GenericViewDTO<Printer>)Session[WMSTekSessions.UserMgr.PrinterList];

            // Agrega Printer eliminado al drop down list
            ddlPrinters.Items.Add(new ListItem(printerViewDTO.Entities[index].Name, printerViewDTO.Entities[index].Id.ToString()));

            // Quita Printer seleccionado de la grilla
            printerViewDTO.Entities.RemoveAt(index);
            grdPrinter.DataSource = printerViewDTO.Entities;
            grdPrinter.DataBind();

            // Actualiza lista de centros
            Session.Add(WMSTekSessions.UserMgr.PrinterList, printerViewDTO);

            // Valida si hay una Impresora por defecto
            if (ExistPrinterDefault())
                txtDefaultPrinter.Text = "1";
            else
                txtDefaultPrinter.Text = "-1";
        }

        protected void RemoveWorkZone(int index)
        {
            workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.UserMgr.WorkZoneList];

            //Agrega WorkZone eliminado al drop down list
            ddlZone.Items.Add(new ListItem(workZoneViewDTO.Entities[index].Name, workZoneViewDTO.Entities[index].Id.ToString()));

            //Quita WorkZone seleccionado de la grilla
            workZoneViewDTO.Entities.RemoveAt(index);
            grdWorkZone.DataSource = workZoneViewDTO.Entities;
            grdWorkZone.DataBind();

            //Actualiza lista de WorkZones
            Session.Add(WMSTekSessions.UserMgr.WorkZoneList, workZoneViewDTO);
         }

        private void RemoverWorkZoneList()
        {
            //Carga la variable que contiene todos los workzones asociados al warehouses que estan en la grilla
            workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.UserMgr.WorkZoneList];
            if (workZoneViewDTO == null)
                workZoneViewDTO = new GenericViewDTO<WorkZone>();
            GenericViewDTO<WorkZone> workZoneViewDTONew = new GenericViewDTO<WorkZone>();


            try
            {
                if (workZoneViewDTO.Entities.Count > 0)
                {
                    foreach (WorkZone workZone in workZoneViewDTO.Entities)
                    {
                        if (workZone.Warehouse != null)
                        {
                            if (workZone.Warehouse.Id != idWarehouseDelete)
                            {
                                //Si la zona que estaba en la grilla, tiene distinto whs que se esta eliminando, 
                                //entonces se agrega a una nueva lista
                                workZoneViewDTONew.Entities.Add(workZone);
                            }
                        }
                    }
                }
                idWarehouseDelete = -1;

                //Actualiza la session de las zonas que se agregaran a la grilla
                Session.Add(WMSTekSessions.UserMgr.WorkZoneList, workZoneViewDTONew);

                //Actualiza la Grilla
                this.grdWorkZone.DataSource = workZoneViewDTONew.Entities;
                this.grdWorkZone.DataBind();

                //Recarga la lista o DropDownList de workzones
                LoadWorkZonesListByWarehouse();
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        private void LoadWorkZonesListByWarehouse()
        {
            //Carga la lista de Warehouses
            warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];
            workZoneListGlobal = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.UserMgr.WorkZoneListGlobal];
            int typeZone = (int)TypeWorkZone.Usuario;
            
            if (workZoneListGlobal == null)
                workZoneListGlobal = new GenericViewDTO<WorkZone>();

            foreach (Warehouse whs in warehouseViewDTO.Entities)
            {
                //Busca los workzones de acuerdo a los warehouses
                GenericViewDTO<WorkZone> workZoneViewDTOLocal = new GenericViewDTO<WorkZone>();
                //workZoneViewDTOLocal = iLayoutMGR.GetWorkZoneByWhs(whs.Id, context);
                workZoneViewDTOLocal = iLayoutMGR.GetWorkZoneByWhsAndTypeZone(whs.Id, typeZone, context);

                //Agrega las zonas a una lista global
                foreach (WorkZone workZone in workZoneViewDTOLocal.Entities)
                {
                    workZoneListGlobal.Entities.Add(workZone);
                }
            }
            //Carga el DropDownList con la lista de Zonas
            LoadWorkZoneByWorkZoneList(this.ddlZone, true, workZoneListGlobal, this.Master.EmptyRowText);

            // Actualiza lista de WorkZones
            Session.Add(WMSTekSessions.UserMgr.WorkZoneListGlobal, workZoneListGlobal);
        }

        private void LoadWorkZonesByWarehouse()
        {
            //Carga la lista de Warehouses
            workZoneListGlobal = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.UserMgr.WorkZoneListGlobal];
            warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];
            int typeZone = (int)TypeWorkZone.Usuario;

            if (workZoneListGlobal == null)
                workZoneListGlobal = new GenericViewDTO<WorkZone>();

            foreach (Warehouse whs in warehouseViewDTO.Entities)
            {
                //Busca los workzones de acuerdo a los warehouses
                GenericViewDTO<WorkZone> workZoneViewDTOLocal = new GenericViewDTO<WorkZone>();
                //workZoneViewDTOLocal = iLayoutMGR.GetWorkZoneByWhs(whs.Id, context);
                workZoneViewDTOLocal = iLayoutMGR.GetWorkZoneByWhsAndTypeZone(whs.Id, typeZone, context);

                //Agrega las zonas a una lista global
                foreach (WorkZone workZone in workZoneViewDTOLocal.Entities)
                {
                    workZoneListGlobal.Entities.Add(workZone);
                }
            }
            //Carga el DropDownList con la lista de Zonas
            LoadWorkZoneByWorkZoneList(this.ddlZone, true, workZoneListGlobal, this.Master.EmptyRowText);

            // Actualiza lista de WorkZones
            Session.Add(WMSTekSessions.UserMgr.WorkZoneListGlobal, workZoneListGlobal);
        }

        private void LoadPrintersByListWarehouse(bool isNew, string emptyRowText)
        {
            GenericViewDTO<Printer> printerViewDTO = new GenericViewDTO<Printer>();
            warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];

            if (ValidateSession(WMSTekSessions.UserMgr.WarehouseList))
            {
                foreach (Warehouse whs in warehouseViewDTO.Entities)
                {
                    //Busca los workzones de acuerdo a los warehouses
                    GenericViewDTO<Printer> printerViewDTOLocal = new GenericViewDTO<Printer>();
                    //Warehouse

                    FilterItem filter = new FilterItem("", whs.Id.ToString());
                    ContextViewDTO contexto = new ContextViewDTO();
                    contexto.MainFilter = this.Master.ucMainFilter.MainFilter;

                    if (this.Master.ucMainFilter.MainFilter.Find(w => w.Name == "Warehouse").FilterValues.Count() > 0)
                    {
                        contexto.MainFilter.Find(w => w.Name == "Warehouse").FilterValues.Remove(contexto.MainFilter.Find(w => w.Name == "Warehouse").FilterValues.First());
                    }

                    contexto.MainFilter.Find(s => s.Name == "Warehouse").FilterValues.Add(filter);

                    printerViewDTOLocal = iDeviceMGR.FindAllPrinter(contexto);
                    contexto.MainFilter.Find(s => s.Name == "Warehouse").FilterValues.Remove(filter);

                    //Agrega las impresoras a una lista global
                    foreach (Printer printer in printerViewDTOLocal.Entities)                    {

                        printerViewDTO.Entities.Add(printer);
                    }
                }

                if (ValidateSession(WMSTekSessions.UserMgr.PrinterList))
                {
                    GenericViewDTO<Printer> printerViewDTOGrid = (GenericViewDTO<Printer>)Session[WMSTekSessions.UserMgr.PrinterList];

                    foreach (var item in printerViewDTOGrid.Entities)
                    {
                        if (printerViewDTO.Entities.Exists(w => w.Id.Equals(item.Id)))
                        {
                            printerViewDTO.Entities.Remove(printerViewDTO.Entities.Find(w => w.Id.Equals(item.Id)));
                        }
                    }
                }
                
                Session.Add(WMSTekSessions.Shared.PrinterList, printerViewDTO);

                ddlPrinters.DataSource = printerViewDTO.Entities;
                ddlPrinters.DataTextField = "Name";
                ddlPrinters.DataValueField = "Id";
                ddlPrinters.DataBind();
            }

            if (isNew)
            {
                ddlPrinters.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                ddlPrinters.Items[0].Selected = true;
            }
        }

        #endregion

        
    }
}
