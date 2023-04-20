using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.WebClient.Base;
using System.Xml;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class TruckMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Truck> truckViewDTO = new GenericViewDTO<Truck>();
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

                        if (isValidViewDTO)
                        {
                            PopulateLists();
                        }
                    }

                    if (ValidateSession(WMSTekSessions.TruckMgr.TruckList))
                    {
                        truckViewDTO = (GenericViewDTO<Truck>)Session[WMSTekSessions.TruckMgr.TruckList];
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
        //        truckViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(truckViewDTO.Errors);
        //    }
        //}

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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                truckViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(truckViewDTO.Errors);
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
                this.Master.ucError.ShowError(truckViewDTO.Errors);
                truckViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            truckViewDTO = iWarehousingMGR.FindAllTruck(context);

            if (!truckViewDTO.hasError() && truckViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.TruckMgr.TruckList, truckViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(truckViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(truckViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            base.LoadTruckType(this.ddlTruckType, isNew, this.Master.EmptyRowText);
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;


            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!truckViewDTO.hasConfigurationError() && truckViewDTO.Configuration != null && truckViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, truckViewDTO.Configuration);

            grdMgr.DataSource = truckViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(truckViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.truckTypeVisible = true;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.statusVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.codeLabel = this.lblFilterCodeLabel.Text;

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
            // Editar Proveedor
            if (mode == CRUD.Update)
            {
                isNew = false;

                //Recupera los datos de la entidad a editar
                hidEditId.Value = truckViewDTO.Entities[index].IdCode;

                PopulateLists();
                txtIdCode.Text = truckViewDTO.Entities[index].IdCode;
                txtIdCode.Enabled = false;
                txtDescription.Text = truckViewDTO.Entities[index].Description;
                ddlTruckType.SelectedValue = (truckViewDTO.Entities[index].Type.Id).ToString();

                if (truckViewDTO.Entities[index].FabricationYear != -1)
                    txtFabricationYear.Text = truckViewDTO.Entities[index].FabricationYear.ToString();

                txtTruckMark.Text = truckViewDTO.Entities[index].TruckMark;
                txtTruckModel.Text = truckViewDTO.Entities[index].TruckModel;
                chkStatus.Checked = truckViewDTO.Entities[index].Status;

                lblNew.Visible = false;
                lblEdit.Visible = true;

            }

            // Nueva Proveedor
            if (mode == CRUD.Create)
            {
                isNew = true;
                hidEditId.Value = "0";
                lblNew.Visible = true;
                lblEdit.Visible = false;

                PopulateLists();
                txtIdCode.Enabled = true;
                txtIdCode.Text = string.Empty;
                txtDescription.Text = string.Empty;
                ddlTruckType.SelectedValue = "-1";
                txtFabricationYear.Text = string.Empty;
                txtTruckMark.Text = string.Empty;
                txtTruckModel.Text = string.Empty;
                chkStatus.Checked = true;
            }

            if (truckViewDTO.Configuration != null && truckViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(truckViewDTO.Configuration, true);
                else
                    base.ConfigureModal(truckViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            
            Truck truck = new Truck();
            truck.Type = new TruckType();

            truck.IdCode = txtIdCode.Text.Trim();
            truck.Status = chkStatus.Checked;
            truck.TruckMark = txtTruckMark.Text;
            truck.TruckModel = txtTruckModel.Text;
            if (!String.IsNullOrEmpty(txtFabricationYear.Text)) truck.FabricationYear = Convert.ToInt32(txtFabricationYear.Text);
            truck.Type.Id = Convert.ToInt32(ddlTruckType.SelectedValue);
            truck.Description = txtDescription.Text.Trim();

            if (hidEditId.Value == "0")
                truckViewDTO = iWarehousingMGR.MaintainTruck(CRUD.Create, truck, context);
            else
                truckViewDTO = iWarehousingMGR.MaintainTruck(CRUD.Update, truck, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (truckViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(truckViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            truckViewDTO = iWarehousingMGR.MaintainTruck(CRUD.Delete, truckViewDTO.Entities[index], context);

            if (truckViewDTO.hasError())
            {
                if (truckViewDTO.Errors.OriginalMessage.IndexOf("FK_Receipt_Truck") > 0)
                {
                    truckViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Truck.InvalidDelete.Truck, context));
                    this.Master.ucError.ShowError(truckViewDTO.Errors);
                }
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(truckViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        
        #endregion
   }
}
