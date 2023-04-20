using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Administration.LogisticsResources
{
    public partial class HangarMgr : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<Hangar> hangarViewDTO = new GenericViewDTO<Hangar>();
        private bool isValidViewDTO = false;
        

        // Propiedad para controlar el nro de pagina activa en la grilla
        public int currentPage
        {
            get
            {
                if (ValidateViewState("currentPage"))
                    return (int)ViewState["currentPage"];
                else
                    return 0;
            }

            set { ViewState["currentPage"] = value; }
        }

        public int currentIndex
        {
            get
            {
                if (ValidateViewState("currentIndex"))
                    return (int)ViewState["currentIndex"];
                else
                    return -1;
            }

            set { ViewState["currentIndex"] = value; }
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
                    }

                    if (ValidateSession(WMSTekSessions.HangarMgr.HangarList))
                    {
                        hangarViewDTO = (GenericViewDTO<Hangar>)Session[WMSTekSessions.HangarMgr.HangarList];
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
        //        workZoneViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(workZoneViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
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
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;
                currentPage = grdMgr.PageIndex;
                currentIndex = grdMgr.SelectedIndex;
                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeTypeUnitOfMeasure();            
        }

        private void InitializeTypeUnitOfMeasure()
        {
            String type= string.Empty;
            var lstTypeLoc = GetConst("TypeOfUnitOfMeasure");

            if (lstTypeLoc.Count == 0)
                type = "(mts)";
            else
                type = "(" + lstTypeLoc[0].Trim() + ")";


            this.lblTypeUnitMeasure.Text = type;
            this.lblTypeUnitMeasure2.Text = type;
            this.lblTypeUnitMeasure3.Text = type;
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
                hangarViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            hangarViewDTO = iLayoutMGR.FindAllHangar(context, true);

            if (!hangarViewDTO.hasError() && hangarViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.HangarMgr.HangarList, hangarViewDTO);
                Session.Remove(WMSTekSessions.Shared.HangarList);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(hangarViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.statusVisible = true;
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

            // Mensaje a mostrar en los mantenedores que no cargan inicialmente la grilla
            hangarViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(hangarViewDTO.MessageStatus.Message);
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
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!hangarViewDTO.hasConfigurationError() && hangarViewDTO.Configuration != null && hangarViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, hangarViewDTO.Configuration);

            grdMgr.DataSource = hangarViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(hangarViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateLists()
        {
            base.LoadUserWarehouses(this.ddlWarehouse, this.Master.EmptyRowText, "-1", true);
        }

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = grdMgr.PageIndex;
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
            // Editar Hangar
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = hangarViewDTO.Entities[index].Id.ToString();

                txtCode.Text = hangarViewDTO.Entities[index].Code;
                txtName.Text = hangarViewDTO.Entities[index].Name;

                ddlWarehouse.SelectedValue = hangarViewDTO.Entities[index].Warehouse.Id.ToString(); 

                if (hangarViewDTO.Entities[index].PositionX != -1) txtPositionX.Text = hangarViewDTO.Entities[index].PositionX.ToString();
                else txtPositionX.Text = string.Empty;

                if (hangarViewDTO.Entities[index].PositionY != -1) txtPositionY.Text = hangarViewDTO.Entities[index].PositionY.ToString();
                else txtPositionY.Text = string.Empty;

                if (hangarViewDTO.Entities[index].PositionZ != -1) txtPositionZ.Text = hangarViewDTO.Entities[index].PositionZ.ToString();
                else txtPositionZ.Text = string.Empty;

                if (hangarViewDTO.Entities[index].Length != -1) txtLength.Text = hangarViewDTO.Entities[index].Length.ToString();
                else txtLength.Text = string.Empty;

                if (hangarViewDTO.Entities[index].Width != -1) txtWidth.Text = hangarViewDTO.Entities[index].Width.ToString();
                else txtWidth.Text = string.Empty;

                if (hangarViewDTO.Entities[index].Height != -1) txtHeight.Text = hangarViewDTO.Entities[index].Height.ToString();
                else txtHeight.Text = string.Empty;

                chkCodStatus.Checked = hangarViewDTO.Entities[index].Status;
                this.txtGLN.Text = hangarViewDTO.Entities[index].GLN;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nuevo Hangar
            if (mode == CRUD.Create)
            {
                // Selecciona Warehouse seleccionados en el Filtro
                this.ddlWarehouse.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues[0].Value;

                this.txtCode.ReadOnly = false;
                hidEditId.Value = "0";
                txtCode.Text = string.Empty;
                txtName.Text= string.Empty;
                txtPositionX.Text= string.Empty;
                txtPositionY.Text= string.Empty;
                txtPositionZ.Text= string.Empty;
                txtLength.Text= string.Empty;
                txtWidth.Text= string.Empty;
                txtHeight.Text= string.Empty;
                chkCodStatus.Checked = true;
                lblNew.Visible = true;
                lblEdit.Visible = false;
                this.txtGLN.Text = string.Empty;
            }

            // Carga configuración de la vista
            if (hangarViewDTO.Configuration != null && hangarViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(hangarViewDTO.Configuration, true);
                else
                {
                    base.ConfigureModal(hangarViewDTO.Configuration, false);
                    this.txtCode.Enabled = false;
                }
            }

            divEditNew.Visible = true;
            mpeHangar.Show();
        }

        protected void SaveChanges()
        {
            List<string> ListValidate = new List<string>();
            if (txtPositionX.Text != "")
                ListValidate.Add(txtPositionX.Text);
            if (txtPositionY.Text != "")
                ListValidate.Add(txtPositionY.Text);
            if (txtPositionZ.Text != "")
                ListValidate.Add(txtPositionZ.Text);
            if (txtLength.Text != "")
                ListValidate.Add(txtLength.Text);
            if (txtWidth.Text != "")
                ListValidate.Add(txtWidth.Text);
            if (txtHeight.Text != "")
                ListValidate.Add(txtHeight.Text);

            //si todos los datos son numericos, entra
            if (ValidateNumber(ListValidate))
            {
                divWarning.Visible = false;
                //agrega los datos de la Bodega
                Hangar hangar = new Hangar(Convert.ToInt32(hidEditId.Value));

                hangar.Warehouse = new Warehouse(Convert.ToInt32(ddlWarehouse.SelectedValue));
                hangar.Code = txtCode.Text.Trim();
                hangar.Name = txtName.Text.Trim();

                if (txtPositionX.Text != "")
                    hangar.PositionX = Convert.ToInt32(txtPositionX.Text);
                else { hangar.PositionX = -1; }
                if (txtPositionY.Text != "")
                    hangar.PositionY = Convert.ToInt32(txtPositionY.Text);
                else { hangar.PositionY = -1; }
                if (txtPositionZ.Text != "")
                    hangar.PositionZ = Convert.ToInt32(txtPositionZ.Text);
                else { hangar.PositionZ = -1; }
                if (txtLength.Text != "")
                    hangar.Length = Convert.ToDecimal(txtLength.Text);
                else { hangar.Length = -1; }
                if (txtWidth.Text != "")
                    hangar.Width = Convert.ToDecimal(txtWidth.Text);
                else { hangar.Width = -1; }
                if (txtHeight.Text != "")
                    hangar.Height = Convert.ToDecimal(txtHeight.Text);
                else { hangar.Height = -1; }

                hangar.Status = chkCodStatus.Checked;

                if (!string.IsNullOrEmpty(txtGLN.Text.Trim()))
                {
                    string resultGLN = ValidateCodeGLN(this.txtGLN.Text.Trim());

                    if (string.IsNullOrEmpty(resultGLN))
                    {
                        hangar.GLN = txtGLN.Text;
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
                        mpeHangar.Show();
                        return;
                    }
                }

                if (hidEditId.Value == "0")
                {
                    hangarViewDTO = iLayoutMGR.MaintainHangar(CRUD.Create, hangar, context);
                }
                else
                    hangarViewDTO = iLayoutMGR.MaintainHangar(CRUD.Update, hangar, context);

                divEditNew.Visible = false;
                mpeHangar.Hide();

                if (hangarViewDTO.hasError())
                {
                    UpdateSession(true);
                    divEditNew.Visible = true;
                    mpeHangar.Show();
                }
                else
                {
                    crud = true;
                    ucStatus.ShowMessage(hangarViewDTO.MessageStatus.Message);
                    UpdateSession(false);
                }
            }
            else//hay datos que no son numeros
            {
                divWarning.Visible = true;
                divEditNew.Visible = true;
                mpeHangar.Show();
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            hangarViewDTO = iLayoutMGR.MaintainHangar(CRUD.Delete, hangarViewDTO.Entities[index], context);

            if (hangarViewDTO.hasError())
                UpdateSession(true);
            else
            {
                ucStatus.ShowMessage(hangarViewDTO.MessageStatus.Message);
                crud = true;
                UpdateSession(false);
            }
        }

        #endregion
    }
}
