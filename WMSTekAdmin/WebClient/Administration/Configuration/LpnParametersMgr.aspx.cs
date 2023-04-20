using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Configuration;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.AdminApp.Warehousing;

namespace Binaria.WMSTek.WebClient.Administration.Configuration
{
    public partial class LpnParametersMgr : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<CfgParameter> cfgParameterViewDTO;
        private GenericViewDTO<ParameterHistory> parameterHistoryViewDTO;
        private GenericViewDTO<Owner> ownViewDTO;
        private GenericViewDTO<Warehouse> whsViewDTO;
        private GenericViewDTO<CfgParameter> parameters;
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
        public string CurrentOwner
        {
            get
            {
                if (ValidateViewState("Owner"))
                    return (string)ViewState["Owner"];
                else
                    return string.Empty;
            }

            set { ViewState["Owner"] = value; }
        }
        public string CurrentWarehouse
        {
            get
            {
                if (ValidateViewState("Warehouse"))
                    return (string)ViewState["Warehouse"];
                else
                    return string.Empty;
            }

            set { ViewState["Warehouse"] = value; }
        }

        //Propiedad para obtener el numero o nivel de grupo
        private int grpLevel
        {
            get { return (int)(ViewState["grpLevel"] ?? 0); }
            set { ViewState["grpLevel"] = value; }
        }

        public int CurrentIdWarehouse
        {
            get
            {
                if (ValidateViewState("idWhs"))
                    return (int)ViewState["idWhs"];
                else
                    return -1;
            }

            set { ViewState["idWhs"] = value; }
        }

        public int CurrentIdOwner
        {
            get
            {
                if (ValidateViewState("idOwn"))
                    return (int)ViewState["idOwn"];
                else
                    return -1;
            }

            set { ViewState["idOwn"] = value; }
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

                        hsMasterDetail.LeftPanel.WidthDefault = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .2);

                        User objUser = (User)Session[WMSTekSessions.Global.LoggedInUser];

                        //Setea el nombre del nodo principal y el tooltip con el nombre  de la compañia
                        if (treevLocation.Nodes[0] != null)
                        {
                            treevLocation.Nodes[0].Text = "<b>" + objUser.Company.Name + "</b>";
                            treevLocation.Nodes[0].ToolTip = objUser.Company.Name;
                        }

                        //Elimina si ha quedado algo de antes y carga inicial del ViewDTO
                        ViewState.Remove("SessionDtoCfgParameterList");
                        UpdateSession(false);
                    }
                    if (ValidateSession(WMSTekSessions.LpnParametersMgr.CfgParameterList))
                    {
                        cfgParameterViewDTO = (GenericViewDTO<CfgParameter>)Session[WMSTekSessions.LpnParametersMgr.CfgParameterList];
                        isValidViewDTO = true;
                    }

                     //Si es un ViewDTO valido, carga la grilla y las listas
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

                //Modifica el Ancho del Div de los Graficos
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
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

                        // Required Validator
                        RequiredFieldValidator reqParameterValue = new RequiredFieldValidator();
                        reqParameterValue.ID = "reqParameterValue";
                        reqParameterValue.ControlToValidate = "txtParameterValue";
                        reqParameterValue.ErrorMessage = lblCode.Text + " es requerido";
                        reqParameterValue.SetFocusOnError = true;
                        reqParameterValue.ValidationGroup = "EditNew";
                        reqParameterValue.Text = " req ";
                        reqParameterValue.Display = ValidatorDisplay.None;

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
                        ranParameterValue.Text = " ran ";
                        ranParameterValue.Display = ValidatorDisplay.None;

                        TableCell ranCell = new TableCell();
                        ranCell.Controls.Add(ranParameterValue);

                        e.Row.Cells.Add(ranCell);
                    }
                }
            }
            catch (Exception ex)
            {
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        protected void treevLocation_SelectedNodeChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    TreeView tv = (TreeView)sender;
                    string TreeId = HttpUtility.UrlEncode(tv.SelectedNode.ValuePath);

                    if (!string.IsNullOrEmpty(TreeId))
                    {
                        //Verifica el nivel del arbol
                        //Nivel 0: Company
                        if (tv.SelectedNode.Depth == 0)
                        {
                            grpLevel = 0;
                            CurrentIdWarehouse = -1;
                            CurrentIdOwner = -1;

                            //Busca el nodo al cual le hicimos clic
                            TreeNode company = treevLocation.FindNode(TreeId);

                            //Comprueba que no este vacio el nodo
                            if (null != company && company.ChildNodes.Count == 0)
                            {
                                //pone en negrita el titulo 
                                //Setea el nombre del nodo principal y el tooltip con el nombre  de la compañia
                                company.Text = "<b>" + objUser.Company.Name + "</b>";
                                company.ToolTip = objUser.Company.Name;

                                //busca todos los owners
                                //ownViewDTO = iWarehousingMGR.FindAllOwner(context);
                                
                                //busca todos los owners por usuario
                                ownViewDTO = iWarehousingMGR.GetOwnersByUser(context, objUser.Id);
                                
                                //Busca todos los warehouses
                                //whsViewDTO = iLayoutMGR.FindAllWarehouse(context);

                                //Busca todos los warehouses por usuario
                                whsViewDTO = iLayoutMGR.GetWarehouseByUser(objUser.Id, context);
                                
                                if (!ownViewDTO.hasError())
                                {
                                    foreach (Owner own in ownViewDTO.Entities)
                                    {
                                        //Carga el nombre y el Id del Owner en el Tree
                                        TreeNode nodoOwn = new TreeNode(own.Name.ToString(), own.Id.ToString());
                                        company.ChildNodes.Add(nodoOwn);
                                        //recorre el warehouse
                                        foreach (Warehouse whs in whsViewDTO.Entities)
                                        {
                                            //Agrega el nuevoi nodo de tipo warehouse
                                            TreeNode nodoWhs = new TreeNode(whs.Name.ToString(), whs.Id.ToString());
                                            nodoOwn.ChildNodes.Add(nodoWhs);
                                        }
                                    }
                                }
                            }
                            //Setea los owners y warehouses
                            CurrentOwner = objUser.Company.Name;
                            CurrentWarehouse = string.Empty;
                            UpdateSession(false);
                            PopulateGrid();
                        }
                        //Nivel 1: Owner
                        else if (tv.SelectedNode.Depth == 1)
                        {
                            grpLevel = 1;

                            CurrentOwner = objUser.Company.Name + " > " + tv.SelectedNode.Text;
                            CurrentWarehouse = string.Empty;
                            //Trae los parametros por id de owner 
                            GetParametersOwnerById(Convert.ToInt32(tv.SelectedNode.Value));

                            CurrentIdOwner = Convert.ToInt32(tv.SelectedNode.Value);
                        }
                        //Nivel 2: Warehouse
                        else
                        {
                            grpLevel = 2;

                            //Setea los centros
                            CurrentWarehouse = " > " + tv.SelectedNode.Text;
                            CurrentOwner = objUser.Company.Name + " > " + tv.SelectedNode.Parent.Text;

                            //Trae los parametros por owner y warehouse
                            GetParametersOwnerWarehouseById(false, Convert.ToInt32(tv.SelectedNode.Parent.Value), Convert.ToInt32(tv.SelectedNode.Value));

                            CurrentIdWarehouse = Convert.ToInt32(tv.SelectedNode.Value);
                            CurrentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Value);
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
        #endregion

        #region "Métodos"

        public void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
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

            cfgParameterViewDTO = iConfigurationMGR.FindAllParametersLpnCompany(context);

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.LpnParametersMgr.CfgParameterList, cfgParameterViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(cfgParameterViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        private void GetParametersOwnerById(int idOwn)
        {
            cfgParameterViewDTO = iConfigurationMGR.GetParametersLpnOwner(context, idOwn);

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.LpnParametersMgr.CfgParameterList, cfgParameterViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
               
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }
        private void GetParametersOwnerWarehouseById(bool showError, int idOwn, int idWhs)
        {
            cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
                cfgParameterViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            cfgParameterViewDTO = iConfigurationMGR.GetParamLpnWarehouses(context, idOwn, idWhs);

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.LpnParametersMgr.CfgParameterList, cfgParameterViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
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

            if (cfgParameterViewDTO.Entities.Count > 0)
            {
                if (CurrentOwner == string.Empty)
                    CurrentOwner = objUser.Company.Name;

                this.lblTitle.Text = CurrentOwner + CurrentWarehouse;
            }
            else
                this.lblTitle.Text = string.Empty;

        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //Deshabilita la barra de busqueda
            this.Master.ucMainFilter.searchVisible = false;

            /* TODO: habilitar filtro?
            // Habilita criterios a usar
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
             */
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
        }

        /// <summary>
        /// Configuracion inicial de la Grilla principal
        /// </summary>
        private void InitializeGrid()
        {
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

                if (treevLocation.Nodes[0] != null)
                {
                    treevLocation.Nodes[0].Selected = true;
                    treevLocation.Nodes[0].Collapse();
                }
            }
        }

        protected void SaveChanges()
        {
            int count = 0;
            bool Error = true;
            bool Equal = true;
            parameters = new GenericViewDTO<CfgParameter>();
            parameterHistoryViewDTO = new GenericViewDTO<ParameterHistory>();
            count = grdMgr.PageSize * grdMgr.PageIndex;

            foreach (GridViewRow row in grdMgr.Rows)
            {
                if (cfgParameterViewDTO.Entities.Count > count)
                {
                    CfgParameter cfgParameter = new CfgParameter();
                    cfgParameter.Owner = new Owner();
                    cfgParameter.Warehouse = new Warehouse();
                    ParameterHistory history = new ParameterHistory();

                    //Busca los controles y asigna sus valores
                    history.ParamValueBefore = cfgParameterViewDTO.Entities[count].ParameterValue;
                    cfgParameter.Id = cfgParameterViewDTO.Entities[count].Id;
                    cfgParameter.ParameterValue = Server.HtmlEncode(((TextBox)row.FindControl("txtParameterValue")).Text);
                    cfgParameter.ParameterCode = cfgParameterViewDTO.Entities[count].ParameterCode;
                    cfgParameter.AllowEdit = cfgParameterViewDTO.Entities[count].AllowEdit;
                    cfgParameter.Type = cfgParameterViewDTO.Entities[count].Type;
                    cfgParameter.MaxValue = cfgParameterViewDTO.Entities[count].MaxValue;
                    cfgParameter.MinValue = cfgParameterViewDTO.Entities[count].MinValue;
                    cfgParameter.DefaultValue = cfgParameterViewDTO.Entities[count].DefaultValue;
                    cfgParameter.Owner.Id = cfgParameterViewDTO.Entities[count].Owner.Id;
                    cfgParameter.Warehouse.Id = cfgParameterViewDTO.Entities[count].Warehouse.Id;

                    //Si los valores estan todos iguales, entonces no guarda
                    if (cfgParameter.ParameterValue != history.ParamValueBefore)
                    {
                        Equal = false;
                    }
                    //Si es editable entonces hace update
                    if (cfgParameter.AllowEdit)
                    {
                        GenericViewDTO<CfgParameter> paramValidator = new GenericViewDTO<CfgParameter>();
                        paramValidator = GenericValidate(cfgParameter.Type, cfgParameter.ParameterValue, cfgParameter.MaxValue, cfgParameter.MinValue);
                        if (!paramValidator.hasError())
                        {
                            parameters.Entities.Add(cfgParameter);
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

                    // Nivel 0 --> Company
                    if (CurrentIdWarehouse == -1 && CurrentIdOwner == -1)
                    {
                        UpdateSession(false);
                    }
                    else
                    {
                        // Nivel 1 --> Owner
                        if (CurrentIdWarehouse == -1)
                        {
                            GetParametersOwnerById(CurrentIdOwner);
                        }
                        // Nivel 2 --> Warehouse
                        else
                        {
                            
                            GetParametersOwnerWarehouseById(false, CurrentIdOwner, CurrentIdWarehouse);
                        }

                        PopulateGrid();
                    }
                }
            }

        }
        #endregion
    }
}
