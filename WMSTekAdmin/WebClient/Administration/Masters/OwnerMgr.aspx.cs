using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class OwnerMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Owner> ownerViewDTO = new GenericViewDTO<Owner>();
        private bool isValidViewDTO = false;
        private bool isNew = false;

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

        public int IdCountry
        {
            get { return (int)(ViewState["IdCountry"] ?? -1); }
            set { ViewState["IdCountry"] = value; }
        }

        public int IdState
        {
            get { return (int)(ViewState["IdState"] ?? -1); }
            set { ViewState["IdState"] = value; }
        }

        public int IdCity
        {
            get { return (int)(ViewState["IdCity"] ?? -1); }
            set { ViewState["IdCity"] = value; }
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
                        //UpdateSession(false);
                        PopulateLists();

                        this.tabGeneral.HeaderText = lbltabGeneral.Text;
                        this.tabDocEntrada.HeaderText = this.lbltabDocEntrada.Text;
                        this.tabDocSalida.HeaderText = this.lbltabDocSalida.Text;
                    }

                    if (ValidateSession(WMSTekSessions.OwnerMgr.OwnerList))
                    {
                        ownerViewDTO = (GenericViewDTO<Owner>)Session[WMSTekSessions.OwnerMgr.OwnerList];
                        Session.Remove(WMSTekSessions.Shared.OwnerList);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
            }
        }

        /// <summary>
        /// Limpia el filtro y carga los datos desde base de datos
        /// </summary>
        /// 
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //si cambia el pais... cambiara el estado y la ciudad
                isNew = true;
                base.ConfigureDDlState(this.ddlState, isNew, IdState, Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
                base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
            }
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //si cambia el estado, solo cambia la ciudad.
                isNew = true;
                base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
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
        //        ownerViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(ownerViewDTO.Errors);
        //    }
        //}
        #endregion

        #region "Eventos"

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
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
                ownerViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            ownerViewDTO = iWarehousingMGR.FindAllOwner(context);

            if (!ownerViewDTO.hasError() && ownerViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OwnerMgr.OwnerList, ownerViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(ownerViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            base.ConfigureDDlCountry(this.ddlCountry, isNew, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlState(this.ddlState, isNew, IdState, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, IdState, IdCountry, this.Master.EmptyRowText);
            
            base.LoadUserWarehouses(this.ddlWhsInbound, this.Master.EmptyRowText, "-1", true);            
            base.LoadInboundTypeFilter(this.ddlInboundType, true, this.Master.EmptyRowText, GetConst("TypeOfInboundMgr").ToArray());

            base.LoadUserWarehouses(this.ddlWhsOutbound, this.Master.EmptyRowText, "-1", true);
            base.LoadOutboundType(this.ddlOutboundType, true, this.Master.EmptyRowText);
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!ownerViewDTO.hasConfigurationError() && ownerViewDTO.Configuration != null && ownerViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, ownerViewDTO.Configuration);

            grdMgr.DataSource = ownerViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(ownerViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;

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
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnExcelVisible = true;
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

        protected void SaveChanges()
        {
            //Agrega los datos del Dueño
            Owner owner =  new Owner(Convert.ToInt32(hidEditId.Value));
           
            owner.Name = txtName.Text.Trim();
            owner.Code = txtCode.Text.Trim();
            owner.TradeName = txtTradeName.Text.Trim(); 
            owner.Address1 = txtAddress1.Text.Trim();
            owner.Address2 = txtAddress2.Text.Trim();
            owner.Country = new Country(Convert.ToInt32(ddlCountry.SelectedValue));
            owner.State = new State(Convert.ToInt32(ddlState.SelectedValue));
            owner.City = new City(Convert.ToInt32(ddlCity.SelectedValue));
            owner.Phone1 = txtPhone1.Text.Trim();
            owner.Phone2 = txtPhone2.Text.Trim();
            owner.Fax1 = txtFax1.Text.Trim();
            owner.Fax2 = txtFax2.Text.Trim();
            owner.Email = txtEmail.Text.Trim();
            owner.AllowCourier = this.chkAllowCourier.Checked;

            if (!string.IsNullOrEmpty(txtGLN.Text.Trim()))
            {
                string resultGLN = ValidateCodeGLN(this.txtGLN.Text.Trim());

                if (string.IsNullOrEmpty(resultGLN))
                {
                    owner.GLN = txtGLN.Text;
                }
                else
                {
                    RequiredFieldValidator val = new RequiredFieldValidator();
                    val.ErrorMessage = resultGLN;
                    val.ControlToValidate = "ctl00$MainContent$txtGLN";
                    val.IsValid = false;
                    val.ValidationGroup = "EditNew";
                    this.Page.Validators.Add(val);
                    revtxtGLN.IsValid = false;
                    revtxtGLN.Validate();

                    divEditNew.Visible = true;
                    modalPopUp.Show();
                    return;
                }
            }

            int index = Convert.ToInt32(hidEditIndex.Value);

            //NumberInboundOrder
            if (index != -1 && ownerViewDTO.Entities[index] != null && ownerViewDTO.Entities[index].NumberInboundOrder != null
                && ownerViewDTO.Entities[index].NumberInboundOrder.Count > 0)
            {
                owner.NumberInboundOrder = ownerViewDTO.Entities[index].NumberInboundOrder;
            }
            //NumberOutboundOrder
            if (index != -1 && ownerViewDTO.Entities[index] != null && ownerViewDTO.Entities[index].NumberOutboundOrder != null
                && ownerViewDTO.Entities[index].NumberOutboundOrder.Count > 0)
            {
                owner.NumberOutboundOrder = ownerViewDTO.Entities[index].NumberOutboundOrder;
            }

            if (hidEditId.Value == "0")
                ownerViewDTO = iWarehousingMGR.MaintainOwnerComplete(CRUD.Create, owner, context);
            else
                ownerViewDTO = iWarehousingMGR.MaintainOwnerComplete(CRUD.Update, owner, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (ownerViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                //modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(ownerViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            tabOwner.ActiveTabIndex = 0;

            // Editar Owner
            if (mode == CRUD.Update)
            {

                // TODO: ver propiedad 'required' para un drop-down list
                IdCountry = ownerViewDTO.Entities[index].Country.Id;
                IdState = ownerViewDTO.Entities[index].State.Id;
                IdCity = ownerViewDTO.Entities[index].City.Id;
                isNew = false;

                PopulateLists();

                //Recupera los datos de la entidad a editar
                hidEditIndex.Value = index.ToString();
                hidEditId.Value = ownerViewDTO.Entities[index].Id.ToString();

                txtCode.Text = ownerViewDTO.Entities[index].Code;
                txtName.Text = ownerViewDTO.Entities[index].Name;
                txtTradeName.Text = ownerViewDTO.Entities[index].TradeName;
                txtAddress1.Text = ownerViewDTO.Entities[index].Address1;
                txtAddress2.Text = ownerViewDTO.Entities[index].Address2;
                ddlCountry.SelectedValue = (ownerViewDTO.Entities[index].Country.Id).ToString();
                ddlState.SelectedValue = (ownerViewDTO.Entities[index].State.Id).ToString();
                ddlCity.SelectedValue = (ownerViewDTO.Entities[index].City.Id).ToString();
                txtPhone1.Text = ownerViewDTO.Entities[index].Phone1;
                txtPhone2.Text = ownerViewDTO.Entities[index].Phone2;
                txtFax1.Text = ownerViewDTO.Entities[index].Fax1;
                txtFax2.Text = ownerViewDTO.Entities[index].Fax2;
                txtEmail.Text = ownerViewDTO.Entities[index].Email;
                txtGLN.Text = ownerViewDTO.Entities[index].GLN;
                this.chkAllowCourier.Checked = ownerViewDTO.Entities[index].AllowCourier;
                //Tab NumberInboundOrder
                GenericViewDTO<NumberInboundOrder> numberInboundOrderViewDTO = new GenericViewDTO<NumberInboundOrder>();

                ContextViewDTO newContext = new ContextViewDTO();
                newContext = context;
                newContext.MainFilter = this.Master.ucMainFilter.MainFilter;
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Clear();
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].AddFilterItem(new FilterItem(hidEditId.Value));

                numberInboundOrderViewDTO = iReceptionMGR.FindAllNumberInboundOrder(newContext);

                // Limpia valores actuales de la grilla
                grdNumberInboundOrder.DataSource = numberInboundOrderViewDTO.Entities;
                grdNumberInboundOrder.DataBind();
                           

                //Tab NumberOutboundOrder
                GenericViewDTO<NumberOutboundOrder> numberOutboundOrderViewDTO = new GenericViewDTO<NumberOutboundOrder>();
                numberOutboundOrderViewDTO = iDispatchingMGR.FindAllNumberOutboundOrder(newContext);

                // Limpia valores actuales de la grilla
                grdNumberOutboundOrder.DataSource = numberOutboundOrderViewDTO.Entities;
                grdNumberOutboundOrder.DataBind();

                ownerViewDTO.Entities[index].NumberInboundOrder = numberInboundOrderViewDTO.Entities;
                ownerViewDTO.Entities[index].NumberOutboundOrder = numberOutboundOrderViewDTO.Entities;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                // TODO: revisar
                IdCountry = 1;
                IdState = 1;
                IdCity = 1;
                isNew = true;

                PopulateLists();

                hidEditId.Value = "0";
                hidEditIndex.Value = "-1";

                txtCode.Text = string.Empty;
                txtName.Text = string.Empty;
                txtTradeName.Text = string.Empty;
                txtAddress1.Text = string.Empty;
                txtAddress2.Text = string.Empty;
                txtPhone1.Text = string.Empty;
                txtPhone2.Text = string.Empty;
                txtFax1.Text = string.Empty;
                txtFax2.Text = string.Empty;
                txtEmail.Text = string.Empty;
                lblNew.Visible = true;
                lblEdit.Visible = false;
                txtGLN.Text = string.Empty;
                this.chkAllowCourier.Checked = false;

                //Tab NumberInboundOrder
                this.grdNumberInboundOrder.DataSource = null;
                this.grdNumberInboundOrder.DataBind();

                //Tab NumberOutboundOrder
                this.grdNumberOutboundOrder.DataSource = null;
                this.grdNumberOutboundOrder.DataBind();
            }

            //Tab NumberInboundOrder
            ClearTabNumberInboundOrder();

            //Tab NumberOutboundOrder
            ClearTabNumberOutboundOrder();


            if (ownerViewDTO.Configuration != null && ownerViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(ownerViewDTO.Configuration, true);
                else
                    base.ConfigureModal(ownerViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();

            CallJsGridViewHeader();
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            ownerViewDTO = iWarehousingMGR.MaintainOwner(CRUD.Delete, ownerViewDTO.Entities[index], context);

            if (ownerViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(ownerViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }


        protected void grdNumberInboundOrder_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        // Quita la 
        protected void grdNumberInboundOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                RemoveNumInboundOrder(Convert.ToInt32(hidEditIndex.Value), e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
            }
        }

        protected void grdNumberInboundOrder_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //    // Deshabilita la opcion de Eliminar si es el Usuario Base del Rol Base
                //    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                //    if (btnDelete != null && itemViewDTO.Entities[ddlWorkZones.SelectedIndex].IsBaseRole && itemViewDTO.Entities[ddlRole.SelectedIndex].Users[e.Row.RowIndex].IsBaseUser)
                //    {
                //        btnDelete.ImageUrl = "~/WebResources/Images/Buttons/icon_delete_des.gif";
                //        btnDelete.Enabled = false;
                //    }
            }
        }

        protected void grdNumberOutboundOrder_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.Header)
            //    e.Row.TableSection = TableRowSection.TableHeader;
        }

        // Quita la 
        protected void grdNumberOutboundOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                RemoveNumOutboundOrder(Convert.ToInt32(hidEditIndex.Value), e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
            }
        }
        
        protected void btnAddNumberInboundOrde_Click(object sender, EventArgs e)
        {
            try
            {
                AddNumInboundOrder(Convert.ToInt32(hidEditIndex.Value));
                divEditNew.Visible = true;
                modalPopUp.Show();
                CallJsGridViewHeader();
            }
            catch (Exception ex)
            {
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
            }
        }
        protected void btnAddNumberOutboundOrde_Click(object sender, EventArgs e)
        {
            try
            {
                AddNumOutboundOrder(Convert.ToInt32(hidEditIndex.Value));
                divEditNew.Visible = true;
                modalPopUp.Show();
                CallJsGridViewHeader();
            }
            catch (Exception ex)
            {
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
            }

        }

        protected void AddNumInboundOrder(int index)
        {
            GenericViewDTO<NumberInboundOrder> numInbViewDTO = new GenericViewDTO<NumberInboundOrder>();
            NumberInboundOrder numInbOrder = new NumberInboundOrder();

            // Owner nuevo
            if (index == -1)
            {
                ownerViewDTO.Entities.Add(new Owner());
                index = ownerViewDTO.Entities.Count - 1;

                hidEditIndex.Value = index.ToString();

                Session[WMSTekSessions.OwnerMgr.OwnerList] = ownerViewDTO;
            }
                       
            numInbOrder.Warehouse = getWhsContext(int.Parse(this.ddlWhsInbound.SelectedValue));
            numInbOrder.InboundType = getInbounType(int.Parse(this.ddlInboundType.SelectedValue));
            numInbOrder.LastInboundNumber = int.Parse(this.txtLastInboundNumber.Text.Trim());
            numInbOrder.NumberLength = int.Parse(this.txtNumberLength.Text.Trim());
            numInbOrder.IsCodePrefix = this.chkIsCodePrefix.Checked ? 1 : 0;
            numInbOrder.Owner = new Owner(int.Parse(hidEditId.Value));

            numInbViewDTO = iReceptionMGR.GetNumberInboundOrderByWhsOwnIdType(context, numInbOrder);

            if (numInbViewDTO.Entities != null && numInbViewDTO.Entities.Count > 0)
            {
                CustomValidator cv = new CustomValidator();
                cv.IsValid = false;
                cv.ErrorMessage = this.lblExisteCorrelativo.Text;
                cv.EnableClientScript = false;
                cv.Display = ValidatorDisplay.None;
                cv.ValidationGroup = "EditIn";
                this.Page.Form.Controls.Add(cv);
            }
            else
            {
                // Si es el primer doc a agregar, crea la lista
                if (ownerViewDTO.Entities[index].NumberInboundOrder == null)
                {
                    ownerViewDTO.Entities[index].NumberInboundOrder = new List<NumberInboundOrder>();
                }

                numInbOrder.Warehouse.DateModified = DateTime.Now;
                ownerViewDTO.Entities[index].NumberInboundOrder.Add(numInbOrder);
                this.grdNumberInboundOrder.DataSource = ownerViewDTO.Entities[index].NumberInboundOrder;
                this.grdNumberInboundOrder.DataBind();

                ClearTabNumberInboundOrder();

                // Quita la Zona seleccionada de la lista de Zonas a Asignar (drop-down list)
                // ddlWorkZones.Items.RemoveAt(ddlWorkZones.SelectedIndex);
            }
        }

        protected void AddNumOutboundOrder(int index)
        {
            GenericViewDTO<NumberOutboundOrder> numOutbViewDTO = new GenericViewDTO<NumberOutboundOrder>();
            NumberOutboundOrder numOutbOrder = new NumberOutboundOrder();

            // Owner nuevo
            if (index == -1)
            {
                ownerViewDTO.Entities.Add(new Owner());
                index = ownerViewDTO.Entities.Count - 1;

                hidEditIndex.Value = index.ToString();

                Session[WMSTekSessions.OwnerMgr.OwnerList] = ownerViewDTO;
            }

            numOutbOrder.Warehouse = getWhsContext(int.Parse(this.ddlWhsOutbound.SelectedValue));
            numOutbOrder.OutboundType = getOutbounType(int.Parse(this.ddlOutboundType.SelectedValue));
            numOutbOrder.LastOutboundNumber = int.Parse(this.txtLastOutboundNumber.Text.Trim());
            numOutbOrder.NumberLength = int.Parse(this.txtOutNumberLength.Text.Trim());
            numOutbOrder.IsCodePrefix = this.chkOutIsCodePrefix.Checked ? 1 : 0;
            numOutbOrder.Owner = new Owner(int.Parse(hidEditId.Value));

            numOutbViewDTO = iDispatchingMGR.GetNumberOutboundByWhsOwnType(context, numOutbOrder);

            if (numOutbViewDTO.Entities != null && numOutbViewDTO.Entities.Count > 0)
            {
                CustomValidator cv = new CustomValidator();
                cv.IsValid = false;
                cv.ErrorMessage = this.lblExisteCorrelativo.Text;
                cv.EnableClientScript = false;
                cv.Display = ValidatorDisplay.None;
                cv.ValidationGroup = "EditOut";
                this.Page.Form.Controls.Add(cv);
            }
            else
            {
                // Si es el primer doc a agregar, crea la lista
                if (ownerViewDTO.Entities[index].NumberOutboundOrder == null)
                {
                    ownerViewDTO.Entities[index].NumberOutboundOrder = new List<NumberOutboundOrder>();
                }

                numOutbOrder.Warehouse.DateModified = DateTime.Now;
                ownerViewDTO.Entities[index].NumberOutboundOrder.Add(numOutbOrder);
                this.grdNumberOutboundOrder.DataSource = ownerViewDTO.Entities[index].NumberOutboundOrder;
                this.grdNumberOutboundOrder.DataBind();

                ClearTabNumberOutboundOrder();

                // Quita la Zona seleccionada de la lista de Zonas a Asignar (drop-down list)
                // ddlWorkZones.Items.RemoveAt(ddlWorkZones.SelectedIndex);
            }
        }

        protected void RemoveNumInboundOrder(int index, int inIndex)
        {
            var numberInboundOrderViewDTO = new GenericViewDTO<NumberInboundOrder>();
            var numberInboundOrderParam = new NumberInboundOrder();

            try
            {
                var ownerToModify = ownerViewDTO.Entities[index]; 
                var row = grdNumberInboundOrder.Rows[inIndex];
                string inboundTypeCode = ((HiddenField)row.FindControl("hidInboundTypeCode")).Value;
                int idWhs = int.Parse(((HiddenField)row.FindControl("hidWarehouseIdInbound")).Value);

                numberInboundOrderParam.Warehouse = new Warehouse();
                numberInboundOrderParam.Warehouse.Id = idWhs;
                numberInboundOrderParam.Owner = new Owner();
                numberInboundOrderParam.Owner.Id = ownerToModify.Id;
                numberInboundOrderParam.InboundType = new InboundType();
                numberInboundOrderParam.InboundType.Code = inboundTypeCode; 

                numberInboundOrderViewDTO = iReceptionMGR.MaintainNumberInboundOrder(CRUD.Delete, numberInboundOrderParam, context);

                if (numberInboundOrderViewDTO.hasError())
                {
                    this.Master.ucError.ShowError(numberInboundOrderViewDTO.Errors);
                }
                else
                {
                    // Quita el correlativo seleccionada de la grilla de correlativos Asignadas al owner actual (grid view)
                    ownerViewDTO.Entities[index].NumberInboundOrder.RemoveAt(inIndex);

                    grdNumberInboundOrder.DataSource = ownerViewDTO.Entities[index].NumberInboundOrder;
                    grdNumberInboundOrder.DataBind();

                    CallJsGridViewHeader();
                }  
            }
            catch (Exception ex)
            {
                numberInboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(numberInboundOrderViewDTO.Errors);
            }
        }

        protected void RemoveNumOutboundOrder(int index, int outIndex)
        {
            var numberOutboundOrderViewDTO = new GenericViewDTO<NumberOutboundOrder>();
            var numberOutboundOrderParam = new NumberOutboundOrder();

            try
            {
                var ownerToModify = ownerViewDTO.Entities[index];
                var row = grdNumberOutboundOrder.Rows[outIndex];
                string outboundTypeCode = ((HiddenField)row.FindControl("hidOutboundTypeCode")).Value;
                int idWhs = int.Parse(((HiddenField)row.FindControl("hidWarehouseIdOutbound")).Value);

                numberOutboundOrderParam.Warehouse = new Warehouse();
                numberOutboundOrderParam.Warehouse.Id = idWhs;  
                numberOutboundOrderParam.Owner = new Owner();
                numberOutboundOrderParam.Owner.Id = ownerToModify.Id;
                numberOutboundOrderParam.OutboundType = new OutboundType();
                numberOutboundOrderParam.OutboundType.Code = outboundTypeCode;

                numberOutboundOrderViewDTO = iDispatchingMGR.MaintainNumberOutboundOrder(CRUD.Delete, numberOutboundOrderParam, context);

                if (numberOutboundOrderViewDTO.hasError())
                {
                    this.Master.ucError.ShowError(numberOutboundOrderViewDTO.Errors);
                }
                else
                {
                    // Quita el correlativo seleccionada de la grilla de correlativos Asignadas al owner actual (grid view)
                    ownerViewDTO.Entities[index].NumberOutboundOrder.RemoveAt(outIndex);

                    grdNumberOutboundOrder.DataSource = ownerViewDTO.Entities[index].NumberOutboundOrder;
                    grdNumberOutboundOrder.DataBind();

                    CallJsGridViewHeader();
                }  
            }
            catch (Exception ex)
            {
                numberOutboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(numberOutboundOrderViewDTO.Errors);
            }
        }

        private Warehouse getWhsContext(int idWhs)
        {
            Warehouse returWhs = new Warehouse();

            foreach (Warehouse whs in context.SessionInfo.User.Warehouses)
            {
                if (whs.Id == idWhs)
                    returWhs = whs;
            }

            return returWhs;
        }

        private InboundType getInbounType(int idInbType)
        {
            GenericViewDTO<InboundType> inboundTypeViewDTO = new GenericViewDTO<InboundType>();

            inboundTypeViewDTO = iReceptionMGR.GetInboundTypeById(context, idInbType);

            return inboundTypeViewDTO.Entities[0];
        }

        private OutboundType getOutbounType(int idOutType)
        {
            GenericViewDTO<OutboundType> outboundTypeViewDTO = new GenericViewDTO<OutboundType>();

            outboundTypeViewDTO = iDispatchingMGR.GetByIdOutboundType(context, idOutType);

            return outboundTypeViewDTO.Entities[0];
        }

        private void ClearTabNumberInboundOrder()
        {
            this.ddlWhsInbound.SelectedValue = "-1";
            this.ddlInboundType.SelectedValue = "-1";
            this.txtLastInboundNumber.Text = string.Empty;
            this.txtNumberLength.Text = string.Empty;
            this.chkIsCodePrefix.Checked = false;
        }

        private void ClearTabNumberOutboundOrder()
        {
            this.ddlWhsOutbound.SelectedValue = "-1";
            this.ddlOutboundType.SelectedValue = "-1";
            this.txtLastOutboundNumber.Text = string.Empty;
            this.txtOutNumberLength.Text = string.Empty;
            this.chkOutIsCodePrefix.Checked = false;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridWithNoDragAndDrop();", true);
        }

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
                //    LoadOutboundOrderDetail(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                //base.ExportToExcel(grdMgr, grdDetail, detailTitle);

                base.ExportToExcel(grdMgr, null, null, true);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                ownerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ownerViewDTO.Errors);
            }
        }

        #endregion
    }
}
