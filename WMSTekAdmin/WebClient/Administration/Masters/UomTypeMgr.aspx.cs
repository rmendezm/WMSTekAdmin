using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class UomTypeMgr : BasePage

    {
        #region "Declaracion de variables"

        private GenericViewDTO<UomType> uomTypeViewDTO;
        private bool isValidViewDTO = false;

        //Propiedad para controlar el nro de pagina activa en la grilla
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
                        PopulateLists();
                        UpdateSession(false);
                    }

                    if (ValidateSession(WMSTekSessions.UomTypeMgr.UomTypeList))
                    {
                        uomTypeViewDTO = (GenericViewDTO<UomType>)Session[WMSTekSessions.UomTypeMgr.UomTypeList];
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
        //        uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
            }
        }

        protected void chkIsVariableWeight_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                reqOverPickingAllowed.Enabled = chkIsVariableWeight.Checked;
                rvOverPickingAllowed.Enabled = chkIsVariableWeight.Checked;

                lblOverPickingAllowed.Visible = chkIsVariableWeight.Checked;
                txtOverPickingAllowed.Visible = chkIsVariableWeight.Checked;

                if (!chkIsVariableWeight.Checked)
                    txtOverPickingAllowed.Text = string.Empty;

                //divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
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
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
                uomTypeViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            uomTypeViewDTO = iWarehousingMGR.FindAllUomType(context);

            if (!uomTypeViewDTO.hasError() && uomTypeViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.UomTypeMgr.UomTypeList, uomTypeViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(uomTypeViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;


            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!uomTypeViewDTO.hasConfigurationError() && uomTypeViewDTO.Configuration != null && uomTypeViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, uomTypeViewDTO.Configuration);

            grdMgr.DataSource = uomTypeViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(uomTypeViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateLists()
        {
            //Carga lista de Owner
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.nameVisible = true;

            this.Master.ucMainFilter.nameLabel = this.lblNameFilter.Text;

            // TODO: personalizar vista del Filtro
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
            //Como no es Actualizacion, setea la variable para que muestre "Listo"
            crud = false;
            //Actualiza la grilla
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
                //Recupera los datos de la entidad a editar
                hidEditId.Value = uomTypeViewDTO.Entities[index].Id.ToString();
                txtName.Text = uomTypeViewDTO.Entities[index].Name;
                this.ddlOwner.Enabled = false;
                this.ddlOwner.SelectedValue = uomTypeViewDTO.Entities[index].Owner.Id.ToString();
                chkHandleDecimal.Checked = uomTypeViewDTO.Entities[index].HandlesDecimal;
                chkIsVariableWeight.Checked = uomTypeViewDTO.Entities[index].IsVariableWeight;
                reqOverPickingAllowed.Enabled = false;  //uomTypeViewDTO.Entities[index].IsVariableWeight;
                rvOverPickingAllowed.Enabled = true; //uomTypeViewDTO.Entities[index].IsVariableWeight;
                txtOverPickingAllowed.Text = uomTypeViewDTO.Entities[index].OverPickingAllowed == -1 ? string.Empty : uomTypeViewDTO.Entities[index].OverPickingAllowed.ToString();
                lblOverPickingAllowed.Visible = true; //uomTypeViewDTO.Entities[index].IsVariableWeight;
                txtOverPickingAllowed.Visible = true; //uomTypeViewDTO.Entities[index].IsVariableWeight;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nuevo Cliente
            if (mode == CRUD.Create)
            {
                //si es nuevo Seteo valores

                //General
                hidEditId.Value = "0";
                txtName.Text = string.Empty;
                this.ddlOwner.SelectedValue = this.Master.ucMainFilter.idOwn.ToString();
                this.ddlOwner.Enabled = false;
                chkHandleDecimal.Checked = false;
                chkIsVariableWeight.Checked = false; 
                reqOverPickingAllowed.Enabled = false;
                rvOverPickingAllowed.Enabled = true;
                txtOverPickingAllowed.Text = string.Empty;
                lblOverPickingAllowed.Visible = true;
                txtOverPickingAllowed.Visible = true;

                //De la página
                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (uomTypeViewDTO.Configuration != null && uomTypeViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(uomTypeViewDTO.Configuration, true);
                else
                    base.ConfigureModal(uomTypeViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {

            UomType uomType = new UomType();
            uomType.Owner = new Owner();

            uomType.Name = txtName.Text.Trim();
            uomType.Owner.Id = Convert.ToInt32(ddlOwner.SelectedValue);

            uomType.HandlesDecimal = chkHandleDecimal.Checked;
            uomType.IsVariableWeight = chkIsVariableWeight.Checked;

            if (string.IsNullOrEmpty(txtOverPickingAllowed.Text.Trim()))
            {
                uomType.OverPickingAllowed = -1;
            }
            else
            {
                uomType.OverPickingAllowed = int.Parse(txtOverPickingAllowed.Text.Trim());
            }

            if (hidEditId.Value == "0")
            {
                uomTypeViewDTO = iWarehousingMGR.MaintainUomType(CRUD.Create, uomType, context);
            }
            else
            {
                uomType.Id = Convert.ToInt32(hidEditId.Value);
                uomTypeViewDTO = iWarehousingMGR.MaintainUomType(CRUD.Update, uomType, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (uomTypeViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(uomTypeViewDTO.MessageStatus.Message);
                //Actualiza
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            uomTypeViewDTO = iWarehousingMGR.MaintainUomType(CRUD.Delete, uomTypeViewDTO.Entities[index], context);

            //Muestra mensaje de status
            crud = true;
            ucStatus.ShowMessage(uomTypeViewDTO.MessageStatus.Message);

            //Actualiza la session
            if (uomTypeViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(uomTypeViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

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
                uomTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(uomTypeViewDTO.Errors);
            }
        }

        #endregion
    }
}
