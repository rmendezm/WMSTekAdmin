using System;
using System.Web.UI;
using System.Globalization;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Configuration;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.AdminApp.Warehousing;
using System.Linq;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.Entities.Base;
using DocumentFormat.OpenXml.Office2010.Excel;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Reception;
using DocumentFormat.OpenXml.Vml.Office;
using static Binaria.WMSTek.Framework.Base.WMSTekError.BusinessAdmin.Validation;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Warehouse = Binaria.WMSTek.Framework.Entities.Layout.Warehouse;
using ListItem = System.Web.UI.WebControls.ListItem;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Binaria.WMSTek.WebClient.Administration.Configuration
{
    public partial class OwnerParametersMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<CfgParameter> cfgOwnerParameterViewDTO = new GenericViewDTO<CfgParameter>();
        private GenericViewDTO<ParameterHistory> parameterHistoryViewDTO = new GenericViewDTO<ParameterHistory>();
        private bool isValidViewDTO = false;
        

        GenericViewDTO<CfgParameter> parameters;


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
        public int CurrentIndex
        {
            get
            {
                if (ValidateViewState("index"))
                    return (int)ViewState["index"];
                else
                    return 0;
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
                    }

                    this.lblError.Visible = false;
                    if (ValidateSession(WMSTekSessions.OwnerParametersMgr.CfgParameterList))
                    {
                        cfgOwnerParameterViewDTO = (GenericViewDTO<CfgParameter>)Session[WMSTekSessions.OwnerParametersMgr.CfgParameterList];
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                base.grdMgr_RowDataBound(sender, e);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                }
            }
            catch (Exception ex)
            {
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
            }
        }

        /// <summary>
        /// Respuesta desde la ventana de diálogo
        /// </summary>
        protected void btnDialogOk_Click(object sender, EventArgs e)
        {
            try
            {
                base.Logout();
            }
            catch (Exception ex)
            {
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
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
        //        cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
        //    }
        //}

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
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
                cfgOwnerParameterViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            context.MainFilter[Convert.ToInt16(EntityFilterName.Scope)].FilterValues.Add(new FilterItem("OWNER"));

            cfgOwnerParameterViewDTO = iConfigurationMGR.CfgOwnerParameterFindAll(context);
            //cfgOwnerParameterViewDTO = iConfigurationMGR.FindAllParameters(context);

            if (!cfgOwnerParameterViewDTO.hasError() && cfgOwnerParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OwnerParametersMgr.CfgParameterList, cfgOwnerParameterViewDTO);
                isValidViewDTO = true;

                //Si no viene de algún crud muestra el mensaje de estado (Listo)
                if (!crud) ucStatus.ShowMessage(cfgOwnerParameterViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;


            // Configura ORDEN de las columnas de la grilla
            if (!cfgOwnerParameterViewDTO.hasConfigurationError() && cfgOwnerParameterViewDTO.Configuration != null && cfgOwnerParameterViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, cfgOwnerParameterViewDTO.Configuration);

            grdMgr.DataSource = cfgOwnerParameterViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(cfgOwnerParameterViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            // Configura VISIBILIDAD de las columnas de la grilla
            if (!cfgOwnerParameterViewDTO.hasConfigurationError() && cfgOwnerParameterViewDTO.Configuration != null && cfgOwnerParameterViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, cfgOwnerParameterViewDTO.Configuration);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = false;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;

            // TODO: habilitar y ver por qué no funciona
            // this.Master.ucMainFilter.parameterScopeVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;

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

            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnNewVisible = true;

            this.Master.ucDialog.BtnOkClick += new EventHandler(btnDialogOk_Click);
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
            parameters = new GenericViewDTO<CfgParameter>();
            bool Error = true;
            bool Equal = true;
            var ddlWarehouse = (DropDownList)this.Master.FindControl("ddlWarehouse");
            int idWhs = int.Parse(ddlWarehouse.SelectedValue);

            List<Owner> tmpOwners = new List<Owner>();

            //Recorre la lista de ubicaciones no asociadas
            foreach (ListItem item in this.lstBoxOwners.Items)
            {
                //Evalua si la ubicacion esta seleccionada
                if (item.Selected)
                {
                    tmpOwners.Add(new Owner(int.Parse(item.Value)));
                    //locationNoAssociated.Remove(locationNoAssociated.First(w => w.IdCode == item.Value));
                }
            }
            CfgParameter sessionParameter = (CfgParameter)Session["SelectCfgParameter"];

            foreach (var own in tmpOwners)
            {
                CfgParameter CfgParameter = new CfgParameter();
                ParameterHistory history = new ParameterHistory();
                CfgParameter.Owner = new Owner();
                CfgParameter.Warehouse = new Warehouse();

                //Busca los controles y asigna sus valores
                history.ParamValueBefore = sessionParameter.ParameterValue;
                CfgParameter.Id = sessionParameter.Id;
                CfgParameter.ParameterCode = sessionParameter.ParameterCode;
                CfgParameter.Type = sessionParameter.Type;
                CfgParameter.MaxValue = sessionParameter.MaxValue;
                CfgParameter.MinValue = sessionParameter.MinValue;
                CfgParameter.ParameterValue = this.txtParamValue.Text;
                CfgParameter.Description = sessionParameter.Description;
                CfgParameter.AllowEdit = sessionParameter.AllowEdit;
                CfgParameter.Owner.Id = own.Id;
                CfgParameter.Warehouse.Id = idWhs;
                
                if (CfgParameter.ParameterValue != history.ParamValueBefore)
                {
                    Equal = false;
                }

                //Si es editable entonces hace update
                if (CfgParameter.AllowEdit)
                {
                    GenericViewDTO<CfgParameter> paramValidator = new GenericViewDTO<CfgParameter>();
                    paramValidator = GenericValidate(CfgParameter.Type, CfgParameter.ParameterValue, CfgParameter.MaxValue, CfgParameter.MinValue);
                    if (!paramValidator.hasError())
                    {
                        parameters.Entities.Add(CfgParameter);
                        parameterHistoryViewDTO.Entities.Add(history);
                        Error = false;
                    }
                    else
                    {
                        this.Master.ucError.ShowError(paramValidator.Errors);
                        isValidViewDTO = false;
                        Error = true;
                    }
                }
            }

            if (!Error && !Equal)
            {
                //Termina de recorrer y Salva
                cfgOwnerParameterViewDTO = iConfigurationMGR.CreateCfgOwnerParameter(parameters, parameterHistoryViewDTO, context);
                //cfgOwnerParameterViewDTO = iConfigurationMGR.MaintainCfgParameter(CRUD.Update, parameters, parameterHistoryViewDTO, grpLevel, false, context);

                //Marca si alguno tuvo error
                if (cfgOwnerParameterViewDTO.hasError())
                {
                    UpdateSession(true);
                }
                else
                {
                    crud = true;
                    ucStatus.ShowMessage(cfgOwnerParameterViewDTO.MessageStatus.Message);
                    UpdateSession(false);

                    // Para que los cambios tengan efecto, se pregunta al usuario si quiere reiniciar su sesión
                    this.Master.ucDialog.ShowConfirm(lblTitle.Text, lblReset.Text, "reset");

                    divEditNew.Visible = false;
                    modalPopUp.Hide();
                }
            }
            else
            {
                divEditNew.Visible = true;
                modalPopUp.Show();
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
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            //Llenar lista de Parametros
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            context.MainFilter[Convert.ToInt16(EntityFilterName.Scope)].FilterValues.Add(new FilterItem("OWNER"));

            var paramViewDTO = iConfigurationMGR.FindAllParameters(context);

            Session["ListCfgParameters"] = paramViewDTO.Entities;
            this.lstBoxParameters.DataSource = lstParameters(paramViewDTO.Entities);
            this.lstBoxParameters.DataTextField = "Text";
            this.lstBoxParameters.DataValueField = "Value";
            this.lstBoxParameters.DataBind();

            //Llenar lsita de Owner
            this.lstBoxOwners.DataSource = context.SessionInfo.User.Owners;
            this.lstBoxOwners.DataTextField = "Name";
            this.lstBoxOwners.DataValueField = "Id";
            this.lstBoxOwners.DataBind();


            this.txtParameterCode.Text = String.Empty;
            this.txtDescription.Text = String.Empty;
            this.txtType.Text = String.Empty;
            this.chkAllowEdit.Checked = false;
            this.txtMinValue.Text = String.Empty;
            this.txtMaxValue.Text = String.Empty;
            this.txtDefaultValue.Text = String.Empty;
            this.txtParamValue.Text = String.Empty;

            divEditNew.Visible = true;
            modalPopUp.Show();

            //CallJsGridViewHeader();
        }

        private ListItemCollection lstParameters(List<CfgParameter> lstParameters)
        {
            ListItemCollection lstReturn = new ListItemCollection();

            foreach (CfgParameter param in lstParameters)
            {
                ListItem newItem = new ListItem();
                newItem.Text = param.ParameterCode + " - " + param.Description;
                newItem.Value = param.Id.ToString();

                lstReturn.Add(newItem);
            }

            return lstReturn;
        }

            private ListItemCollection lstOwners(List<Owner> lstOwner)
            {
                ListItemCollection lstReturn = new ListItemCollection();

                foreach (Owner own in lstOwner)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = own.Name;
                    newItem.Value = own.Id.ToString();

                    lstReturn.Add(newItem);
                }

                return lstReturn;
            }
        #endregion

        protected void lstBoxParameters_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int idParam = int.Parse(lstBoxParameters.SelectedValue);
                int index = lstBoxParameters.SelectedIndex;

                List<CfgParameter> listParam = Session["ListCfgParameters"] as List<CfgParameter>;

                var param = listParam[index];
                Session["SelectCfgParameter"] = param;

                this.txtParameterCode.Text = param.ParameterCode;
                this.txtDescription.Text = param.Description;
                this.txtType.Text = param.Type;
                this.chkAllowEdit.Checked = param.AllowEdit;
                this.txtMinValue.Text = param.MinValue;
                this.txtMaxValue.Text = param.MaxValue;
                this.txtDefaultValue.Text = param.DefaultValue;
                this.txtParamValue.Text = String.Empty;
                this.txtParamValue.Enabled = param.AllowEdit;


                // Required Validator
                this.reqParameterValue.SetFocusOnError = true;
                this.reqParameterValue.ValidationGroup = "EditNew";
                this.reqParameterValue.Text = " * ";

                this.ranParameterValue.Enabled = true;
                
                switch (param.Type)
                {
                    case "INT":
                        this.ranParameterValue.Type = ValidationDataType.Integer;
                        break;

                    case "STRING":
                        this.ranParameterValue.Type = ValidationDataType.String;
                        this.ranParameterValue.Enabled = false;
                        break;

                    case "DEC":
                        this.ranParameterValue.Type = ValidationDataType.Double;
                        break;

                    case "BOOL":
                        this.ranParameterValue.Type = ValidationDataType.Integer;
                        break;
                }
                

                this.ranParameterValue.MinimumValue = param.MinValue;
                this.ranParameterValue.MaximumValue = param.MaxValue;
                this.ranParameterValue.SetFocusOnError = true;
                this.ranParameterValue.ValidationGroup = "EditNew";
                this.ranParameterValue.Text = " * ";
                this.ranParameterValue.ErrorMessage = "Valor está fuera de rango";

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                cfgOwnerParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgOwnerParameterViewDTO.Errors);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {           
            cfgOwnerParameterViewDTO = iConfigurationMGR.DeleteParamOwnerAndWarehouse(cfgOwnerParameterViewDTO.Entities[index], context);

            if (cfgOwnerParameterViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra el mensaje en la barra de status
                ucStatus.ShowMessage(cfgOwnerParameterViewDTO.MessageStatus.Message);
                crud = true;
                //Actualiza la grilla
                UpdateSession(false);
            }
        }

    }
       
}

