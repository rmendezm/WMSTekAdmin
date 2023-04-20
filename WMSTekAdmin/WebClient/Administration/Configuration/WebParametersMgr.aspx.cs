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

namespace Binaria.WMSTek.WebClient.Administration.Configuration
{
    public partial class WebParametersMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<CfgParameter> cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();
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

        //Propiedad para obtener el numero o nivel de grupo
        private int grpLevel
        {
            get { return (int)(ViewState["grpLevel"] ?? 0); }
            set { ViewState["grpLevel"] = value; }
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
                        grpLevel = 0;

                        // Carga inicial del ViewDTO
                        UpdateSession(false);
                    }

                    this.lblError.Visible = false;
                    if (ValidateSession(WMSTekSessions.WebParametersMgr.CfgParameterList))
                    {
                        cfgParameterViewDTO = (GenericViewDTO<CfgParameter>)Session[WMSTekSessions.WebParametersMgr.CfgParameterList];
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                base.grdMgr_RowDataBound(sender, e);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkAllowEdit = (CheckBox)e.Row.FindControl("chkAllowEdit") as CheckBox;
                    TextBox txtValueParameter = (TextBox)e.Row.FindControl("txtParameterValue") as TextBox;
                    
                    if (!chkAllowEdit.Checked)
                    {
                        txtValueParameter.Enabled = false;
                    }
                    else
                    {
                        Label lblCode = (Label)e.Row.FindControl("lblCode") as Label;
                        Label lblMinValue = (Label)e.Row.FindControl("lblMinValue") as Label;
                        Label lblMaxValue = (Label)e.Row.FindControl("lblMaxValue") as Label;
                        Label lblType = (Label)e.Row.FindControl("lblType") as Label;


                        if(lblType.Text != "STRING") { 
                            // Required Validator
                            RequiredFieldValidator reqParameterValue = new RequiredFieldValidator();
                            reqParameterValue.ID = "reqParameterValue";
                            reqParameterValue.ControlToValidate = "txtParameterValue";
                            reqParameterValue.ErrorMessage = lblCode.Text + " es requerido";
                            reqParameterValue.SetFocusOnError = true;
                            reqParameterValue.ValidationGroup = "EditNew";
                            reqParameterValue.Text = " * ";
                            //reqParameterValue.Display = ValidatorDisplay.None;
                         
                            TableCell reqCell = new TableCell();
                            reqCell.Controls.Add(reqParameterValue);

                            e.Row.Cells.Add(reqCell); 

                            // Range Validator
                            RangeValidator ranParameterValue = new RangeValidator();
                        
                            ranParameterValue.ID = "ranParameterValue";
                            ranParameterValue.ControlToValidate = "txtParameterValue";
                            ranParameterValue.ErrorMessage = lblCode.Text + " está fuera de rango";

                            switch (lblType.Text)
                            { 
                                case "INT":
                                    ranParameterValue.Type = ValidationDataType.Integer;
                                    break;

                                case "STRING":
                                    ranParameterValue.Type = ValidationDataType.String;
                                    break;

                                case "DEC":
                                    ranParameterValue.Type = ValidationDataType.Double;
                                    break;

                                case "BOOL":
                                    ranParameterValue.Type = ValidationDataType.Integer;
                                    break;
                            }
                        
                            ranParameterValue.MinimumValue = lblMinValue.Text;
                            ranParameterValue.MaximumValue = lblMaxValue.Text;
                            ranParameterValue.SetFocusOnError = true;
                            ranParameterValue.ValidationGroup = "EditNew";
                            ranParameterValue.Text = " * ";
                            //ranParameterValue.Display = ValidatorDisplay.None;
                        
                            TableCell ranCell = new TableCell();
                            ranCell.Controls.Add(ranParameterValue);

                            e.Row.Cells.Add(ranCell);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
        //        cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
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
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
                cfgParameterViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            cfgParameterViewDTO = iConfigurationMGR.FindAllParameters(context);

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.WebParametersMgr.CfgParameterList, cfgParameterViewDTO);
                isValidViewDTO = true;

                //Si no viene de algún crud muestra el mensaje de estado (Listo)
                if (!crud) ucStatus.ShowMessage(cfgParameterViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;


            // Configura ORDEN de las columnas de la grilla
            if (!cfgParameterViewDTO.hasConfigurationError() && cfgParameterViewDTO.Configuration != null && cfgParameterViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, cfgParameterViewDTO.Configuration);

            grdMgr.DataSource = cfgParameterViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(cfgParameterViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            // Configura VISIBILIDAD de las columnas de la grilla
            if (!cfgParameterViewDTO.hasConfigurationError() && cfgParameterViewDTO.Configuration != null && cfgParameterViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, cfgParameterViewDTO.Configuration);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
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
            Master.ucTaskBar.BtnSaveClick += new EventHandler(btnSave_Click);

            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnSaveVisible = true;

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
                int count = 0;
                count = grdMgr.PageSize * grdMgr.PageIndex;
                bool Error = true;
                bool Equal = true;

                foreach (GridViewRow row in grdMgr.Rows)
                {
                    if (cfgParameterViewDTO.Entities.Count > count)
                    {
                        CfgParameter CfgParameter = new CfgParameter();
                        ParameterHistory history = new ParameterHistory();
                        CfgParameter.Owner = new Owner();
                        CfgParameter.Warehouse = new Warehouse();

                        //Busca los controles y asigna sus valores
                        history.ParamValueBefore = cfgParameterViewDTO.Entities[count].ParameterValue;
                        CfgParameter.ParameterValue = Server.HtmlEncode(((TextBox)row.FindControl("txtParameterValue")).Text);
                        CfgParameter.ParameterCode = cfgParameterViewDTO.Entities[count].ParameterCode;
                        CfgParameter.AllowEdit = cfgParameterViewDTO.Entities[count].AllowEdit;
                        CfgParameter.Type = cfgParameterViewDTO.Entities[count].Type;
                        CfgParameter.MaxValue = cfgParameterViewDTO.Entities[count].MaxValue;
                        CfgParameter.MinValue = cfgParameterViewDTO.Entities[count].MinValue;
                        CfgParameter.DefaultValue = cfgParameterViewDTO.Entities[count].DefaultValue;
                        CfgParameter.Owner.Id = -1;
                        CfgParameter.Warehouse.Id = -1;
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
                                break;
                            }
                        }
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (!Error && !Equal)
                {
                    var defaultPasswordParam = parameters.Entities.Where(p => p.ParameterCode.ToLower() == "defaultpassword").FirstOrDefault();

                    if (defaultPasswordParam != null)
                    {
                        var passPolicyErrors = PasswordPolicy.IsValid(defaultPasswordParam.ParameterValue);

                        if (passPolicyErrors != null && passPolicyErrors.Count > 0)
                        {
                            var errorMsg = string.Join("<br>", passPolicyErrors.ToArray());

                            this.Master.ucDialog.ShowAlert(lblPoliciyPasswordTitle.Text, errorMsg, string.Empty);
                            return;
                        }
                    }
                    
                    //Termina de recorrer y Salva
                    cfgParameterViewDTO = iConfigurationMGR.MaintainCfgParameter(CRUD.Update, parameters, parameterHistoryViewDTO, grpLevel, false, context);

                    //Marca si alguno tuvo error
                    if (cfgParameterViewDTO.hasError())
                    {
                        UpdateSession(true);
                    }
                    else
                    {
                        crud = true;
                        ucStatus.ShowMessage(cfgParameterViewDTO.MessageStatus.Message);
                        UpdateSession(false);

                        // Para que los cambios tengan efecto, se pregunta al usuario si quiere reiniciar su sesión
                        this.Master.ucDialog.ShowConfirm(lblTitle.Text, lblReset.Text, "reset");
                    }
                }
        }
        #endregion
    }
       
}

