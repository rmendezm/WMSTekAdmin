using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Configuration;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.AdminApp.Warehousing;
using Binaria.WMSTek.Framework.Base;
using System.Drawing;
using System.Data;

namespace Binaria.WMSTek.WebClient.Administration.Configuration
{
    public partial class ItemParametersMgr : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<CfgParameter> cfgParameterViewDTO;
        private GenericViewDTO<ParameterHistory> parameterHistoryViewDTO;
        private GenericViewDTO<Owner> ownViewDTO;
        private GenericViewDTO<Warehouse> whsViewDTO;
        private GenericViewDTO<CfgParameter> parameters;
        public GenericViewDTO<Item> itemViewDTO;
        public GenericViewDTO<GrpItem1> grpItem1ViewDTO;
        public GenericViewDTO<GrpItem2> grpItem2ViewDTO;
        public GenericViewDTO<GrpItem3> grpItem3ViewDTO;
        public GenericViewDTO<GrpItem4> grpItem4ViewDTO;
        public List<String> filters = null;

        private bool isValidViewDTO = false;
        private String code = string.Empty;
        private String name = string.Empty;
        private String description = string.Empty;
        private List<int> checkedItemsCurrentView = new List<int>();

        //Propiedad para controlar el nro de pagina activa en la grilla principal
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
        //Propiedad para controlar el nro de pagina activa en la grilla  de items
        public int currentIndexItem
        {
            get
            {
                if (ValidateViewState("indexItem"))
                    return (int)ViewState["indexItem"];
                else
                    return 0;
            }

            set { ViewState["indexItem"] = value; }
        }
        //Propiedad para controlar el indice presionado de la grilla
        public int currentIndex
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

        //Propiedad para obtener el owner con el que se trabaja
        public string currentNameOwner
        {
            get
            {
                if (ValidateViewState("nameOwn"))
                    return (string)ViewState["nameOwn"];
                else
                    return string.Empty;
            }

            set { ViewState["nameOwn"] = value; }
        }
        //Propiedad para obtener el centro con el que se trabaja
        public string currentNameWarehouse
        {
            get
            {
                if (ValidateViewState("nameWhs"))
                    return (string)ViewState["nameWhs"];
                else
                    return string.Empty;
            }

            set { ViewState["nameWhs"] = value; }
        }

        //Propiedad para obtener el id del owner seleccionado
        public int currentIdOwner
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
        //Propiedad para obtener el id del centro seleccionado
        public int currentIdWhs
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

        public int currentIdItem
        {
            get
            {
                if (ValidateViewState("idItem"))
                    return (int)ViewState["idItem"];
                else
                    return -1;
            }

            set { ViewState["idItem"] = value; }
        }

        //Propiedad para obtener el id de groupItem1
        private int idGrpItem1
        {
            get { return (int)(ViewState["idGrpItem1"] ?? -1); }
            set { ViewState["idGrpItem1"] = value; }
        }
        //Propiedad para obtener el id de groupItem2
        private int idGrpItem2
        {
            get { return (int)(ViewState["idGrpItem2"] ?? -1); }
            set { ViewState["idGrpItem2"] = value; }
        }
        //Propiedad para obtener el id de groupItem3
        private int idGrpItem3
        {
            get { return (int)(ViewState["idGrpItem3"] ?? -1); }
            set { ViewState["idGrpItem3"] = value; }
        }
        //Propiedad para obtener el id de groupItem4
        private int idGrpItem4
        {
            get { return (int)(ViewState["idGrpItem4"] ?? -1); }
            set { ViewState["idGrpItem4"] = value; }
        }
        //Propiedad para obtener el numero o nivel de grupo
        private int level
        {
            get { return (int)(ViewState["grpLevel"] ?? 0); }
            set { ViewState["grpLevel"] = value; }
        }

        private bool showItems
        {
            get { return (bool)(Session["showItems"] ?? false); }
            set { Session["showItems"] = value; }
        }


        //Propiedad para almacenar el codigo seleccionado en el filtro de la grilla
        private string idParameter
        {
            get
            {
                if (ValidateViewState("pId"))
                    return ViewState["pId"].ToString();
                else
                    return string.Empty;
            }

            set { ViewState["pId"] = value; }
        }

        //Propiedad para almacenar el valor seleccionado en el filtro de la grilla
        private string parameterValue
        {
            get
            {
                if (ValidateViewState("pValue"))
                    return ViewState["pValue"].ToString();
                else
                    return "-1"; 
            }

            set { ViewState["pValue"] = value; }
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
                        //Da el ancho por defecto al panel de la izquierda
                        hsMasterDetail.LeftPanel.WidthDefault = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .2);

                        //obtiene el objeto usuario
                        User objUser = (User)Session[WMSTekSessions.Global.LoggedInUser];

                        //Setea el nombre del nodo principal y el tooltip con el nombre  de la compañia
                        if (treevLocation.Nodes[0] != null)
                        {
                            //setea el nodo 0 con el nombre de la compañia
                            treevLocation.Nodes[0].Text = objUser.Company.Name;
                            treevLocation.Nodes[0].ToolTip = objUser.Company.Name;
                            treevLocation.Font.Bold= true; 
                            level = 0;

                            loadFristTree();
                        }

                        //Elimina si ha quedado algo de antes y carga inicial del ViewDTO
                        showItems = false;
                        ViewState.Remove("SessionDtoCfgParameterList");
                        UpdateSession(false);
                    }

                    // Recupera lista de parámetros
                    if (ValidateSession(WMSTekSessions.ItemParametersMgr.CfgParameterList))
                    {
                        cfgParameterViewDTO = (GenericViewDTO<CfgParameter>)Session[WMSTekSessions.ItemParametersMgr.CfgParameterList];
                        isValidViewDTO = true;
                    }

                    // Recupera lista de items
                    if (ValidateSession(WMSTekSessions.ItemParametersMgr.ItemList))
                    {
                        itemViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.ItemParametersMgr.ItemList];
                    }

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                        PopulateGrid();
                        if (showItems) PopulateGridItems();
                    }
                }
            }
            catch (Exception ex)
            {
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    base.Page_Load(sender, e);

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Recupera los items seleccionados
                        //if (showItems) CollectItemsSelected();
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
                        if (showItems)
                        {
                            PopulateGridItems();                            
                        }
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
        /// <summary>
        /// Salva el valor actualizado
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Habilita la grilla principal
                grdMgr.Enabled = true;

                // Salva los cambios
                SaveChanges();
            }
            catch (Exception ex)
            {
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }
        /// <summary>
        /// Setea la grilla para agregar los efectos de seleccion y de mouseover
        /// </summary>
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
        /// <summary>
        /// Actualiza al cambiar la pagina 
        /// </summary>
        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
            }
            catch (Exception ex)
            {
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }
 
        /// <summary>
        /// Verifica cada dato al ir llenando la grilla fila por fila
        /// </summary>
        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                base.grdMgr_RowDataBound(sender, e);

                // Agrega la celda de validación al header
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    TableCell headCell = new TableCell();
                    headCell.Text = "&nbsp;&nbsp;";
                    e.Row.Cells.AddAt(0, headCell);
                }

                //Si es una fila normal
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //Busca controles
                    CheckBox chkAllowEdit = (CheckBox)e.Row.FindControl("chkAllowEdit") as CheckBox;
                    TextBox txtValueParameter = (TextBox)e.Row.FindControl("txtParameterValue") as TextBox;

                    TextBox txtFilter = (TextBox)e.Row.FindControl("txtFilter") as TextBox;
                    txtFilter.Visible = true;

                    ImageButton btnFilter = (ImageButton)e.Row.FindControl("btnFilter") as ImageButton;
                    btnFilter.Visible = true;

                    DropDownList ddlFilterItem = (DropDownList)e.Row.FindControl("ddlFilter") as DropDownList;
                    ddlFilterItem.Visible = false;

                    AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
                    trigger.ControlID = ddlFilterItem.NamingContainer.UniqueID + "$" +ddlFilterItem.ID;
                    trigger.EventName = "SelectedIndexChanged";
                    upGrid.Triggers.Add(trigger);  

                    //Si el parametro es editable entonces habilita el txt del valor
                    if (!chkAllowEdit.Checked)
                    {
                        txtValueParameter.Enabled = false;
                    }
                    //else
                    //{
                        //Captura los valores de la grilla
                        Label lblCode = (Label)e.Row.FindControl("lblCode") as Label;
                        Label lblMinValue = (Label)e.Row.FindControl("lblMinValue") as Label;
                        Label lblMaxValue = (Label)e.Row.FindControl("lblMaxValue") as Label;
                        Label lblType = (Label)e.Row.FindControl("lblType") as Label;
                        Label lblIsDifferent = (Label)e.Row.FindControl("lblIsDifferent") as Label;
                        TextBox txtParameterValue = (TextBox)e.Row.FindControl("txtParameterValue") as TextBox;

                        // Required Validator
                        RequiredFieldValidator reqParameterValue = new RequiredFieldValidator();
                        reqParameterValue.ID = "reqParameterValue";
                        reqParameterValue.ControlToValidate = "txtParameterValue";
                        reqParameterValue.ErrorMessage = lblCode.Text + " es requerido";
                        reqParameterValue.SetFocusOnError = true;
                        reqParameterValue.ValidationGroup = "EditNew";
                        reqParameterValue.Text = " * ";
                        reqParameterValue.Display = ValidatorDisplay.Dynamic;

                        TableCell reqCell = new TableCell();
                        reqCell.Controls.Add(reqParameterValue);

                        //configura el Range Validator
                        RangeValidator ranParameterValue = new RangeValidator();
                        ranParameterValue.ID = "ranParameterValue";
                        ranParameterValue.ControlToValidate = "txtParameterValue";
                        ranParameterValue.ErrorMessage = lblCode.Text + " está fuera de rango";

                        //Valida según el tipo de dato.
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
                                ddlFilterItem.Visible = true;
                                txtFilter.Visible = false;
                                btnFilter.Visible = false;
                                break;
                        }

                        ranParameterValue.MinimumValue = lblMinValue.Text;
                        ranParameterValue.MaximumValue = lblMaxValue.Text;
                        ranParameterValue.SetFocusOnError = true;
                        ranParameterValue.ValidationGroup = "EditNew";
                        ranParameterValue.Text = " * ";
                        ranParameterValue.Display = ValidatorDisplay.Dynamic;

                        reqCell.Controls.Add(ranParameterValue);
                        e.Row.Cells.AddAt(0, reqCell);

                        //Busca si  hay diferencias y si es asi modifica el color del textbox del parametro
                        if (Convert.ToInt32(lblIsDifferent.Text) > 0)
                        {
                            //Colorea el txt gris indicando que en otro nivel el parametro es distinto al mostrado
                            txtParameterValue.BackColor = System.Drawing.Color.Beige;

                            if (showItems)
                            {
                                if (ddlFilterItem.Visible)
                                {
                                    //Habilita el ddl de filtro por valor y mantiene el que fue seleccionado
                                    if (ddlFilterItem.DataTextField == idParameter)
                                        ddlFilterItem.SelectedValue = parameterValue;
                                    else
                                        ddlFilterItem.SelectedIndex = 0;

                                    ddlFilterItem.Enabled = true;
                                }
                                else
                                {
                                    txtFilter.Enabled = true;
                                    btnFilter.Enabled = true;
                                    btnFilter.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_search.png";

                                    if (txtFilter.ValidationGroup == idParameter)
                                        txtFilter.Text = parameterValue;
                                    else
                                        txtFilter.Text = string.Empty;
                                }
                            }
                            else
                            {
                                ddlFilterItem.Enabled = false;
                                txtFilter.Enabled = false;
                                btnFilter.Enabled = false;
                                btnFilter.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_search_dis.png";
                            }
                        }
                        else
                        {
                            ddlFilterItem.Enabled = false;
                            txtFilter.Enabled = false;
                            btnFilter.Enabled = false;
                            btnFilter.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_search_dis.png";
                        }
                    //}
                }
            }
            catch (Exception ex)
            {
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "Filter")
                {
                    TextBox txtFilter = (TextBox)grdMgr.Rows[index].FindControl("txtFilter") as TextBox;
                    Label lblIdParameter = (Label)grdMgr.Rows[index].FindControl("lblIdParameter") as Label;

                    idParameter = lblIdParameter.Text;           
                    parameterValue = txtFilter.Text;

                    UpdateItemSession();
                }
            }
            catch (Exception ex)
            {
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        /// <summary>
        /// Accion que se desata al hacer clic en algun item del arbol
        /// </summary>
        protected void treevLocation_SelectedNodeChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    currentIdItem = -1;
                    // Muestra siempre grilla de Items (excepto para nivel 0 (company) y 1 (owner)
                    showItems = true;
                    this.divFilterItem.Visible = true;
                    this.divDetail.Visible = true;

                    //Obtiene el árbol
                    TreeView tv = (TreeView)sender;

                    //Obtiene el path dentro del arbol
                    string TreeId = HttpUtility.UrlEncode(tv.SelectedNode.ValuePath);

                    // Verifica que el arbol no sea nulo
                    if (!string.IsNullOrEmpty(TreeId))
                    {
                        level = tv.SelectedNode.Depth;
                        // Verifica el nivel del arbol
                        switch (level)
                        {
                            // Nivel 0: Company (clic en Company)
                            case 0:
                                showItems = false;

                                // Busca el nodo al cual le hicimos clic
                                TreeNode company = treevLocation.FindNode(TreeId);

                                // Comprueba que no este vacio el nodo
                                if (null != company && company.ChildNodes.Count == 0)
                                {
                                    // Setea el nombre del nodo principal y el tooltip con el nombre  de la compañia
                                    company.Text = objUser.Company.Name;
                                    company.ToolTip = objUser.Company.Name;

                                    // Busca los Owners asociados al usuario actual
                                    ownViewDTO = iWarehousingMGR.GetOwnersByUser(context, context.SessionInfo.User.Id);

                                    // Busca todos los warehouses
                                    whsViewDTO = iLayoutMGR.GetWarehouseByUser(context.SessionInfo.User.Id, context);

                                    //Carga el Tree y muestra la lista de Owners
                                    if (!ownViewDTO.hasError() && ownViewDTO.Entities != null)
                                    {
                                        foreach (Owner own in ownViewDTO.Entities)
                                        {
                                            //Carga el nombre y el Id del Owner en el Tree
                                            TreeNode nodoOwn = new TreeNode(own.Name.ToString(), own.Id.ToString());
                                            company.ChildNodes.Add(nodoOwn);

                                            foreach (Warehouse whs in whsViewDTO.Entities)
                                            {
                                                //Agrega el nuevo nodo de tipo warehouse
                                                TreeNode nodoWhs = new TreeNode(whs.ShortName.ToString(), whs.Id.ToString());
                                                nodoOwn.ChildNodes.Add(nodoWhs);
                                            }
                                        }
                                    }
                                }

                                //Setea los owners y warehouses
                                currentNameOwner = objUser.Company.Name;
                                currentNameWarehouse = string.Empty;

                                //Esconde todos los demas controles
                                HideFiltersAndItems();
                                ReloadData();
                                break;

                            //Nivel 1: Owner (clic en owner -> Muestra lista warehouses)
                            case 1:
                                showItems = false;

                                //Setea los centros y el label de la ruta
                                currentNameOwner = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Text;
                                currentNameWarehouse = string.Empty;

                                //Actualiza los id segun lo cliqueado en el treeview
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Value);

                                //Trae los parametros por id de owner 
                                GetParametersOwnerById(false, Convert.ToInt32(currentIdOwner));

                                //Oculta todos los demas filtros y grilla de Items
                                HideFiltersAndItems();

                                break;

                            //Nivel 2: Warehouses (clic en warehouse -> Muestra lista Grp1)
                            case 2:
                                //Setea los centros y el label de la ruta
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Text;
                                currentNameOwner = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Text;

                                //Actualiza los id segun lo cliqueado en el treeview
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                                currentIdWhs = Convert.ToInt32(tv.SelectedNode.Value);

                                //Muestra los div GrpItem
                                this.divFilter.Visible = true;

                                //Esconde los demas div
                                this.divBscGrpItm2.Visible = false;
                                this.divBscGrpItm3.Visible = false;
                                this.divBscGrpItm4.Visible = false;

                                // Resetea grpItem1...4
                                idGrpItem1 = -1;
                                idGrpItem2 = -1;
                                idGrpItem3 = -1;
                                idGrpItem4 = -1;

                                //Actualiza la lista del siguiente nivel
                                base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);

                                if (ddlBscGrpItm1.Items.Count <= 1)
                                {
                                    this.divBscGrpItm1.Visible = false;
                                }
                                else
                                {
                                    //Muestra el ddl del siguiente nivel
                                    this.divBscGrpItm1.Visible = true;
                                    this.ddlBscGrpItm1.Visible = true;
                                    this.lblTitleGrpItm1.Visible = true;
                                    this.lblNameGrp1.Visible = false;
                                }

                                //Carga grilla con los parametros por owner y warehouse
                                GetParametersOwnerWarehouseById(false, currentIdOwner, currentIdWhs);

                                //Busca los GrpItem1(sectores) que tengan algun cambio
                                grpItem1ViewDTO = iWarehousingMGR.GetGrpItem1Treeview(context, currentIdOwner);

                                //Limpia el arbol
                                tv.SelectedNode.ChildNodes.Clear();

                                //verifica que el nodo warehouse no venga null y arma el treeview
                                if (tv.SelectedNode.ChildNodes != null)
                                {
                                    //recorre el GroupItem1
                                    foreach (GrpItem1 grpItem1 in grpItem1ViewDTO.Entities)
                                    {
                                        if (grpItem1.Id > 0)
                                        {
                                            //Agrega el nuevo nodo de tipo warehouse
                                            TreeNode nodoGrp1 = new TreeNode(grpItem1.Name.ToString(), grpItem1.Id.ToString());
                                            tv.SelectedNode.ChildNodes.Add(nodoGrp1);
                                        }
                                    }
                                }

                                break;

                            //Nivel 3: Muestra lista de Sectores y habilita y carga los filtros.(clic en Grp1(sector) --> Muestra lista GroupItem2 (rubro))
                            case 3:
                                //Setea los centros y el label de la ruta
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Parent.Text;
                                currentNameOwner = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Text;

                                //Actualiza los id segun lo cliqueado en el treeview
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                                currentIdWhs = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                                idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Value);

                                // Resetea grpItem2...4
                                idGrpItem2 = -1;
                                idGrpItem3 = -1;
                                idGrpItem4 = -1;

                                //Muestra los div hasta GrpItem1
                                this.divFilter.Visible = true;
                                this.divBscGrpItm1.Visible = true;

                                //Esconde los demas div
                                this.divBscGrpItm3.Visible = false;
                                this.divBscGrpItm4.Visible = false;

                                //Setea el label del nivel actual
                                this.lblNameGrp1.Text = this.lblSimbol.Text + tv.SelectedNode.Text;
                                this.ddlBscGrpItm1.Visible = false;
                                this.lblNameGrp1.Visible = true;

                                //Actualiza la lista del siguiente nivel
                                base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                                base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);

                                if (ddlBscGrpItm2.Items.Count <= 1)
                                {
                                    this.divBscGrpItm2.Visible = false;
                                }
                                else
                                {
                                    //Muestra el ddl del siguiente nivel
                                    this.divBscGrpItm2.Visible = true;
                                    this.ddlBscGrpItm2.Visible = true;
                                    this.lblTitleGrpItm2.Visible = true;
                                    this.lblNameGrp2.Visible = false;
                                }

                                //Carga grilla con los parametros por  owner, warehouse y Grp1
                                GetParamGrp1ByOwnWhs(false, currentIdOwner, currentIdWhs, idGrpItem1);

                                //Busca los GrpItem2 que tengan algun cambio para llenar el arbol
                                grpItem2ViewDTO = iWarehousingMGR.GetGrpItem2Treeview(context, currentIdOwner, idGrpItem1);

                                //Limpia el arbol
                                tv.SelectedNode.ChildNodes.Clear();

                                //verifica que el nodo Grp1 no venga null y arma el treeview con los Grp1
                                if (tv.SelectedNode.ChildNodes != null)
                                {
                                    //recorre el GroupItem2
                                    foreach (GrpItem2 grpItem2 in grpItem2ViewDTO.Entities)
                                    {
                                        if (grpItem2.Id > 0)
                                        {
                                            //Agrega el nuevo nodo de tipo warehouse
                                            TreeNode nodoGrp2 = new TreeNode(grpItem2.Name.ToString(), grpItem2.Id.ToString());
                                            tv.SelectedNode.ChildNodes.Add(nodoGrp2);
                                        }
                                    }
                                }
                                break;

                            //Nivel 4: GroupItem2 (clic en un GroupItem2 -> Muestra lista GroupItem3 familia)
                            case 4:
                                //Setea los centros y el label de la ruta
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Text;
                                currentNameOwner = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Text;

                                //Actualiza los id según lo cliqueado en el tree
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Value);
                                currentIdWhs = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                                idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                                idGrpItem2 = Convert.ToInt32(tv.SelectedNode.Value);

                                // Resetea grpItem3...4
                                idGrpItem3 = -1;
                                idGrpItem4 = -1;

                                //Muestra los div hasta GrpItem2
                                this.divFilter.Visible = true;
                                this.divBscGrpItm1.Visible = true;
                                this.divBscGrpItm2.Visible = true;

                                //Esconde los demas div
                                this.divBscGrpItm4.Visible = false;

                                //Setea el label del nivel actual
                                this.lblNameGrp2.Text = this.lblSimbol.Text + tv.SelectedNode.Text;
                                this.ddlBscGrpItm2.Visible = false;
                                this.lblNameGrp2.Visible = true;

                                //Actualiza la lista del siguiente nivel
                                base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                                base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);
                                base.ConfigureDDLGrpItem3(this.ddlBscGrpItm3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.Master.EmptyRowText, true, currentIdOwner);

                                if (ddlBscGrpItm3.Items.Count <= 1)
                                {
                                    this.divBscGrpItm3.Visible = false;
                                }
                                else
                                {
                                    //Muestra el ddl del siguiente nivel
                                    this.divBscGrpItm3.Visible = true;
                                    this.ddlBscGrpItm3.Visible = true;
                                    this.lblTitleGrpItm3.Visible = true;
                                    this.lblNameGrp3.Visible = false;
                                }

                                //Carga grilla con los parametros por  owner, warehouse y Grp2
                                GetParamGrp2ByOwnWhs(false, currentIdOwner, currentIdWhs, idGrpItem1, idGrpItem2);

                                //Busca los GrpItem3 que tengan algun cambio para llenar el arbol
                                grpItem3ViewDTO = iWarehousingMGR.GetGrpItem3Treeview(context, currentIdOwner, idGrpItem1, idGrpItem2);

                                //Limpia el arbol
                                tv.SelectedNode.ChildNodes.Clear();

                                //verifica que el nodo Grp2 no venga null y arma el treeview
                                if (tv.SelectedNode.ChildNodes != null)
                                {
                                    //recorre el GroupItem3
                                    foreach (GrpItem3 grpItem3 in grpItem3ViewDTO.Entities)
                                    {
                                        if (grpItem3.Id > 0)
                                        {
                                            //Agrega el nuevo nodo de tipo warehouse
                                            TreeNode nodoGrp3 = new TreeNode(grpItem3.Name.ToString(), grpItem3.Id.ToString());
                                            tv.SelectedNode.ChildNodes.Add(nodoGrp3);
                                        }
                                    }
                                }
                                break;

                            //Nivel 5: GroupItem3 (clic en un GroupItem3 -> Muestra lista GroupItem4 subFamilia)
                            case 5:
                                //Actualiza las propiedades que guardan el nombre segun lo cliqueado en el tree
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Text;
                                currentNameOwner = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Parent.Text;

                                //Actualiza los id segun lo cliqueado en el tree
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Parent.Value);
                                currentIdWhs = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Value);
                                idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                                idGrpItem2 = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                                idGrpItem3 = Convert.ToInt32(tv.SelectedNode.Value);

                                // Resetea grpItem4
                                idGrpItem4 = -1;

                                //Muestra los div hasta GrpItem3
                                this.divFilter.Visible = true;
                                this.divBscGrpItm1.Visible = true;
                                this.divBscGrpItm2.Visible = true;
                                this.divBscGrpItm3.Visible = true;

                                //Setea el label del nivel actual
                                this.lblNameGrp3.Text = this.lblSimbol.Text + tv.SelectedNode.Text;
                                this.ddlBscGrpItm3.Visible = false;
                                this.lblNameGrp3.Visible = true;

                                //Actualiza la lista del siguiente nivel
                                base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                                base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);
                                base.ConfigureDDLGrpItem3(this.ddlBscGrpItm3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.Master.EmptyRowText, true, currentIdOwner);
                                base.ConfigureDDLGrpItem4(this.ddlBscGrpItm4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.Master.EmptyRowText, true, currentIdOwner);

                                // Si no hay agrupadores en el siguiente nivel, muestra los items
                                if (ddlBscGrpItm4.Items.Count <= 1)
                                {
                                    this.divBscGrpItm4.Visible = false;
                                }
                                else
                                {
                                    //Muestra el ddl del siguiente nivel
                                    this.divBscGrpItm4.Visible = true;
                                    this.ddlBscGrpItm4.Visible = true;
                                    this.lblTitleGrpItm4.Visible = true;
                                    this.lblNameGrp4.Visible = false;
                                }

                                //Carga grilla con los parametros por  owner, warehouse y Grp3
                                GetParamGrp3ByOwnWhs(false, currentIdOwner, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3);

                                //Busca los GrpItem4 que tengan algun cambio para llenar el arbol
                                grpItem4ViewDTO = iWarehousingMGR.GetGrpItem4Treeview(context, currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3);

                                //Limpia el arbol
                                tv.SelectedNode.ChildNodes.Clear();

                                //verifica que el nodo Grp3 no venga null y arma el treeview
                                if (tv.SelectedNode.ChildNodes != null)
                                {
                                    //recorre el GroupItem4
                                    foreach (GrpItem4 grpItem4 in grpItem4ViewDTO.Entities)
                                    {
                                        if (grpItem4.Id > 0)
                                        {
                                            //Agrega el nuevo nodo de tipo warehouse
                                            TreeNode nodoGrp4 = new TreeNode(grpItem4.Name.ToString(), grpItem4.Id.ToString());
                                            tv.SelectedNode.ChildNodes.Add(nodoGrp4);
                                        }
                                    }
                                }
                                break;

                            //Nivel 6: GroupItem4 (clic en un GroupItem4(subfamilia) -> En el arbol no muestra nada mas) Carga filtro de busqueda de Items
                            case 6:
                                //Actualiza las propiedades que guardan el nombre segun lo cliqueado en el tree
                                currentNameOwner = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text;
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Parent.Text;

                                //Actualiza las propiedades que guardan el Id segun lo cliqueado en el tree
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Parent.Parent.Value);
                                currentIdWhs = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Parent.Value);
                                idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Value);
                                idGrpItem2 = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                                idGrpItem3 = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                                idGrpItem4 = Convert.ToInt32(tv.SelectedNode.Value);

                                //Muestra los div hasta GrpItem4
                                this.divFilter.Visible = true;
                                this.divBscGrpItm1.Visible = true;
                                this.divBscGrpItm2.Visible = true;
                                this.divBscGrpItm3.Visible = true;
                                this.divBscGrpItm4.Visible = true;

                                //Setea el label del nivel actual
                                this.lblNameGrp4.Text = this.lblSimbol.Text + tv.SelectedNode.Text;
                                this.ddlBscGrpItm4.Visible = false;
                                this.lblNameGrp4.Visible = true;

                                //Carga la grilla de parametros
                                GetParamGrp4ByOwnWhs(false, currentIdOwner, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4);

                                break;
                        }

                        if (showItems) UpdateItemSession();
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
        /// Setea los efectos visuales en la grilla al cargarla
        /// </summary>
        protected void grdItem_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkSelect = e.Row.FindControl("chkSelectItem") as CheckBox;

                    Item rowView = (Item)e.Row.DataItem;

                    // Retrieve the state value for the current row. 
                    String idItem = rowView.Id.ToString();

                    if (chkSelect != null) chkSelect.Attributes.Add("onclick", "ChangeChecks(this, " + idItem + ");");
  

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                            continue;

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");

                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "');");
                        e.Row.Cells[i].Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdItem, "Select$" + e.Row.RowIndex);
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
        /// Carga la grilla de items segun el valor seleccionado en el filtro
        /// </summary>
        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlFilterItem = (DropDownList)sender;
                
                //Almacena el parámetro seleccionado
                idParameter = ddlFilterItem.DataTextField;
                parameterValue = ddlFilterItem.SelectedValue;

                UpdateItemSession();
            }
            catch (Exception ex)
            {
                cfgParameterViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        /// <summary>
        /// Al cambiar el ddl del grupo1 se actualiza lo mismo que al hacer clic en el arbol pero a nivel de grupo 1
        /// </summary>
        protected void ddlBscGrpItm1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Muestra los items que no tienen agrupacion a este nivel
                    showItems = true;
                    this.divFilterItem.Visible = true;
                    this.divDetail.Visible = true;

                    //Captura el valor de item seleccionado
                    idGrpItem1 = Convert.ToInt32(ddlBscGrpItm1.SelectedValue);

                    //Cambia el nivel
                    level = 3;

                    //Muestra los div hasta GrpItem1
                    this.divFilter.Visible = true;
                    this.divBscGrpItm1.Visible = true;

                    //Esconde los demas div
                    this.divBscGrpItm3.Visible = false;
                    this.divBscGrpItm4.Visible = false;

                    //Setea el label del nivel actual
                    this.lblNameGrp1.Text = this.lblSimbol.Text + ddlBscGrpItm1.SelectedItem.ToString();
                    this.ddlBscGrpItm1.Visible = false;
                    this.lblNameGrp1.Visible = true;

                    //Actualiza la lista del siguiente nivel
                    base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                    base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);

                    // Si no hay agrupadores en el siguiente nivel, lo oculta
                    if (ddlBscGrpItm2.Items.Count <= 1)
                    {
                        this.divBscGrpItm2.Visible = false;
                    }
                    else
                    {
                        //Muestra el ddl del siguiente nivel
                        this.divBscGrpItm2.Visible = true;
                        this.ddlBscGrpItm2.Visible = true;
                        this.lblTitleGrpItm2.Visible = true;
                        this.lblNameGrp2.Visible = false;
                    }

                    //Carga grilla con los parametros por  owner, warehouse y Grp1
                    GetParamGrp1ByOwnWhs(false, currentIdOwner, currentIdWhs, idGrpItem1);

                    //Busca los GrpItem2 que tengan algun cambio para llenar el arbol
                    grpItem2ViewDTO = iWarehousingMGR.GetGrpItem2Treeview(context, currentIdOwner, idGrpItem1);

                    //Actualizo TreeView - Recorre los owners
                    foreach (TreeNode node in treevLocation.Nodes[0].ChildNodes)
                    {
                        //Pregunta si tiene warehouses
                        if (node.ChildNodes.Count > 0)
                        {
                            //Rrecorre los warehouses
                            foreach (TreeNode node2 in node.ChildNodes)
                            {
                                //Pregunta si tiene grpItem 1
                                if (node2.ChildNodes.Count > 0)
                                {
                                    //Recorre los GrpItem1
                                    foreach (TreeNode node3 in node2.ChildNodes)
                                    {
                                        //Pregunta si el id del grpItem1 es igual al que esta seleccionado en el ddl
                                        if (Convert.ToInt32(node3.Value) == idGrpItem1)
                                        {
                                            //Expande el nodo anterior
                                            node2.Expand();
                                            //Selecciona el nodo que tiene el mismo id del ddl
                                            node3.Select();
                                            //Expande el nodo
                                            node3.Expand();

                                            //Actualiza los id segun lo cliqueado en el ddl
                                            currentIdOwner = Convert.ToInt32(treevLocation.SelectedNode.Parent.Parent.Value);
                                            currentIdWhs = Convert.ToInt32(treevLocation.SelectedNode.Parent.Value);

                                            //Limpia el arbol
                                            treevLocation.SelectedNode.ChildNodes.Clear();

                                            //verifica que el nodo Grp1 no venga null y arma el treeview con los Grp1
                                            if (treevLocation.SelectedNode.ChildNodes != null)
                                            {
                                                //recorre el GroupItem2
                                                foreach (GrpItem2 grpItem2 in grpItem2ViewDTO.Entities)
                                                {
                                                    //Agrega el nuevo nodo de tipo warehouse
                                                    TreeNode nodoGrp2 = new TreeNode(grpItem2.Name.ToString(), grpItem2.Id.ToString());
                                                    treevLocation.SelectedNode.ChildNodes.Add(nodoGrp2);
                                                }
                                            }

                                            //Setea los nombres de los centros y el label de la ruta
                                            currentNameWarehouse = this.lblSimbol.Text + treevLocation.SelectedNode.Parent.Text;
                                            currentNameOwner = objUser.Company.Name + this.lblSimbol.Text + treevLocation.SelectedNode.Parent.Parent.Text;

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    UpdateItemSession();
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }
        /// <summary>
        /// Al cambiar el ddl del grupo2 se actualiza lo mismo que al hacer clic en el arbol pero a nivel de grupo 2
        /// </summary>
        protected void ddlBscGrpItm2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Muestra los items que no tienen agrupacion a este nivel
                    showItems = true;
                    this.divFilterItem.Visible = true;
                    this.divDetail.Visible = true;

                    //Captura el valor de item seleccionado
                    idGrpItem2 = Convert.ToInt32(ddlBscGrpItm2.SelectedValue);

                    level = 4;

                    //Muestra los div hasta GrpItem2
                    this.divFilter.Visible = true;
                    this.divBscGrpItm1.Visible = true;
                    this.divBscGrpItm2.Visible = true;

                    //Esconde los demas div
                    this.divBscGrpItm4.Visible = false;

                    //Setea el label del nivel actual
                    this.lblNameGrp2.Text = this.lblSimbol.Text + ddlBscGrpItm2.SelectedItem.ToString();
                    this.ddlBscGrpItm2.Visible = false;
                    this.lblNameGrp2.Visible = true;

                    //Actualiza la lista del siguiente nivel
                    base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                    base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);
                    base.ConfigureDDLGrpItem3(this.ddlBscGrpItm3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.Master.EmptyRowText, true, currentIdOwner);

                    // Si no hay agrupadores en el siguiente nivel, lo oculta
                    if (ddlBscGrpItm3.Items.Count <= 1)
                    {
                        this.divBscGrpItm3.Visible = false;
                    }
                    else
                    {
                        //Muestra el ddl del siguiente nivel
                        this.divBscGrpItm3.Visible = true;
                        this.ddlBscGrpItm3.Visible = true;
                        this.lblTitleGrpItm3.Visible = true;
                        this.lblNameGrp3.Visible = false;
                    }

                    //Carga grilla con los parametros por  owner, warehouse y Grp2
                    GetParamGrp2ByOwnWhs(false, currentIdOwner, currentIdWhs, idGrpItem1, idGrpItem2);

                    //Busca los GrpItem3 que tengan algun cambio para llenar el arbol
                    grpItem3ViewDTO = iWarehousingMGR.GetGrpItem3Treeview(context, currentIdOwner, idGrpItem1, idGrpItem2);

                    //Actualizo TreeView - Recorre los owners
                    foreach (TreeNode node in treevLocation.Nodes[0].ChildNodes)
                    {
                        if (node.ChildNodes.Count > 0)
                        {
                            //Recorre los warehouses
                            foreach (TreeNode node2 in node.ChildNodes)
                            {
                                if (node2.ChildNodes.Count > 0)
                                {
                                    //Recorre el grupo 1
                                    foreach (TreeNode node3 in node2.ChildNodes)
                                    {
                                        if (node3.ChildNodes.Count > 0)
                                        {
                                            //Recorre el grupo 2
                                            foreach (TreeNode node4 in node3.ChildNodes)
                                            {
                                                //Si coincide con el seleccionado
                                                if (Convert.ToInt32(node4.Value) == idGrpItem2)
                                                {
                                                    //Expande los nodos seleccionados
                                                    node3.Expand();
                                                    node4.Select();
                                                    node4.Expand();


                                                    //Busca los GrpItem3 que tengan algun cambio para llenar el arbol
                                                    grpItem3ViewDTO = iWarehousingMGR.GetGrpItem3Treeview(context, currentIdOwner, idGrpItem1, idGrpItem2);

                                                    //Limpia el arbol
                                                    treevLocation.SelectedNode.ChildNodes.Clear();

                                                    //verifica que el nodo Grp1 no venga null y arma el treeview
                                                    if (treevLocation.SelectedNode.ChildNodes != null)
                                                    {
                                                        //recorre el GroupItem2
                                                        foreach (GrpItem3 grpItem3 in grpItem3ViewDTO.Entities)
                                                        {
                                                            //Agrega el nuevo nodo de tipo warehouse
                                                            TreeNode nodoGrp3 = new TreeNode(grpItem3.Name.ToString(), grpItem3.Id.ToString());
                                                            treevLocation.SelectedNode.ChildNodes.Add(nodoGrp3);
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    UpdateItemSession();
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }
        /// <summary>
        /// Al cambiar el ddl del grupo3 se actualiza lo mismo que al hacer clic en el arbol pero a nivel de grupo 3
        /// </summary>
        protected void ddlBscGrpItm3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Muestra los items que no tienen agrupacion a este nivel
                    showItems = true;
                    this.divFilterItem.Visible = true;
                    this.divDetail.Visible = true;

                    //Captura el valor de item seleccionado
                    idGrpItem3 = Convert.ToInt32(ddlBscGrpItm3.SelectedValue);

                    //Cambia el nivel
                    level = 5;

                    //Muestra los div hasta GrpItem3
                    this.divFilter.Visible = true;
                    this.divBscGrpItm1.Visible = true;
                    this.divBscGrpItm2.Visible = true;
                    this.divBscGrpItm3.Visible = true;

                    //Setea el label del nivel actual
                    this.lblNameGrp3.Text = this.lblSimbol.Text + ddlBscGrpItm3.SelectedItem.ToString();
                    this.ddlBscGrpItm3.Visible = false;
                    this.lblNameGrp3.Visible = true;

                    //Actualiza la lista del siguiente nivel
                    base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                    base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);
                    base.ConfigureDDLGrpItem3(this.ddlBscGrpItm3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.Master.EmptyRowText, true, currentIdOwner);
                    base.ConfigureDDLGrpItem4(this.ddlBscGrpItm4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.Master.EmptyRowText, true, currentIdOwner);

                    // Si no hay agrupadores en el siguiente nivel, lo oculta
                    if (ddlBscGrpItm4.Items.Count <= 1)
                    {
                        this.divBscGrpItm4.Visible = false;
                    }
                    else
                    {
                        //Muestra el ddl del siguiente nivel
                        this.divBscGrpItm4.Visible = true;
                        this.ddlBscGrpItm4.Visible = true;
                        this.lblTitleGrpItm4.Visible = true;
                        this.lblNameGrp4.Visible = false;
                    }

                    //Carga grilla con los parametros por  owner, warehouse y Grp3
                    GetParamGrp3ByOwnWhs(false, currentIdOwner, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3);

                    //Busca los GrpItem4 que tengan algun cambio para llenar el arbol
                    grpItem4ViewDTO = iWarehousingMGR.GetGrpItem4Treeview(context, currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3);

                    //Actualizo TreeView
                    foreach (TreeNode node in treevLocation.Nodes[0].ChildNodes)
                    {
                        if (node.ChildNodes.Count > 0)
                        {
                            foreach (TreeNode node2 in node.ChildNodes)
                            {
                                if (node2.ChildNodes.Count > 0)
                                {
                                    foreach (TreeNode node3 in node2.ChildNodes)
                                    {
                                        if (node3.ChildNodes.Count > 0)
                                        {
                                            foreach (TreeNode node4 in node3.ChildNodes)
                                            {
                                                if (node4.ChildNodes.Count > 0)
                                                {
                                                    foreach (TreeNode node5 in node4.ChildNodes)
                                                    {
                                                        if (Convert.ToInt32(node5.Value) == idGrpItem3)
                                                        {
                                                            node4.Expand();
                                                            node5.Select();
                                                            node5.Expand();


                                                            //Busca los GrpItem4 que tengan algun cambio para llenar el arbol
                                                            grpItem4ViewDTO = iWarehousingMGR.GetGrpItem4Treeview(context, currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3);

                                                            //Limpia el arbol
                                                            treevLocation.SelectedNode.ChildNodes.Clear();

                                                            //verifica que el nodo Grp3 no venga null y arma el treeview
                                                            if (treevLocation.SelectedNode.ChildNodes != null)
                                                            {
                                                                //recorre el GroupItem4
                                                                foreach (GrpItem4 grpItem4 in grpItem4ViewDTO.Entities)
                                                                {
                                                                    //Agrega el nuevo nodo de tipo warehouse
                                                                    TreeNode nodoGrp4 = new TreeNode(grpItem4.Name.ToString(), grpItem4.Id.ToString());
                                                                    treevLocation.SelectedNode.ChildNodes.Add(nodoGrp4);
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    UpdateItemSession();
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlBscGrpItm4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Muestra los items 
                    showItems = true;
                    this.divFilterItem.Visible = true;
                    this.divDetail.Visible = true;

                    //Captura el valor de item seleccionado
                    idGrpItem4 = Convert.ToInt32(ddlBscGrpItm4.SelectedValue);

                    //Cambia el nivel
                    level = 6;

                    //Muestra los div hasta GrpItem4
                    this.divFilter.Visible = true;
                    this.divBscGrpItm1.Visible = true;
                    this.divBscGrpItm2.Visible = true;
                    this.divBscGrpItm3.Visible = true;
                    this.divBscGrpItm4.Visible = true;

                    //Setea el label del nivel actual
                    this.lblNameGrp4.Text = this.lblSimbol.Text + ddlBscGrpItm4.SelectedItem.ToString();
                    this.ddlBscGrpItm4.Visible = false;
                    this.lblNameGrp4.Visible = true;

                    //Muestra el filtro de items
                    this.divFilterItem.Visible = true;
                    this.divDetail.Visible = true;

                    //Carga grilla con los parametros por  owner, warehouse y Grp3
                    GetParamGrp4ByOwnWhs(false, currentIdOwner, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4);

                    //Actualizo TreeView
                    foreach (TreeNode node in treevLocation.Nodes[0].ChildNodes)
                    {
                        if (node.ChildNodes.Count > 0)
                        {
                            foreach (TreeNode node2 in node.ChildNodes)
                            {
                                if (node2.ChildNodes.Count > 0)
                                {
                                    foreach (TreeNode node3 in node2.ChildNodes)
                                    {
                                        if (node3.ChildNodes.Count > 0)
                                        {
                                            foreach (TreeNode node4 in node3.ChildNodes)
                                            {
                                                if (node4.ChildNodes.Count > 0)
                                                {
                                                    foreach (TreeNode node5 in node4.ChildNodes)
                                                    {
                                                        if (node5.ChildNodes.Count > 0)
                                                        {
                                                            foreach (TreeNode node6 in node5.ChildNodes)
                                                            {
                                                                if (Convert.ToInt32(node6.Value) == idGrpItem4)
                                                                {
                                                                    node5.Expand();
                                                                    node6.Select();

                                                                    //Limpia el arbol
                                                                    treevLocation.SelectedNode.ChildNodes.Clear();

                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    UpdateItemSession();
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void btnSearchItem_Click(object sender, ImageClickEventArgs e)
        {
            // Valida variable de sesion del Usuario Loggeado
            if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                code = this.txtCode.Text;
                name = this.txtName.Text;
                description = this.txtDescription.Text;
                checkedItemsCurrentView.Clear();
                UpdateItemSession();
            }
        }

        protected void btnItemRefresh_Click(object sender, ImageClickEventArgs e)
        {
            // Valida variable de sesion del Usuario Loggeado
            if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                this.txtCode.Text = string.Empty;
                this.txtName.Text = string.Empty;
                this.txtDescription.Text = string.Empty;
                UpdateItemSession();
            }
        }

       #endregion

        #region "Métodos"
        /// <summary>
        /// Oculta controles
        /// </summary>
        private void HideFiltersAndItems()
        {
            this.divFilterItem.Visible = false;
            this.divFilter.Visible = false;

            this.lblNameGrp1.Visible = false;
            this.lblNameGrp2.Visible = false;
            this.lblNameGrp3.Visible = false;
            this.lblNameGrp4.Visible = false;

            this.divBscGrpItm1.Visible = false;
            this.divBscGrpItm2.Visible = false;
            this.divBscGrpItm3.Visible = false;
            this.divBscGrpItm4.Visible = false;

            this.ddlBscGrpItm1.Visible = false;
            this.ddlBscGrpItm2.Visible = false;
            this.ddlBscGrpItm3.Visible = false;
            this.ddlBscGrpItm4.Visible = false;

            this.divDetail.Visible = false;
        }
        /// <summary>
        /// Inicializa los filtros básicos
        /// </summary>
        private void InitializeFilterBasic(bool init, bool refresh)
        {
            //Configura Filtro Básico
            //Habilita criterios a usar
            this.Master.ucMainFilter.Visible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.codeVisible = true;

            //Habilita el filtro de los grupos de items
            this.Master.ucMainFilter.divBasicItemGroupVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        public void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            // ucStatus.pageSizeChanged += new EventHandler(ucStatus_pageSizeChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);

            // Mensaje a mostrar en los mantenedores que no cargan inicialmente la grilla
            itemViewDTO = new GenericViewDTO<Item>();
            itemViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(itemViewDTO.MessageStatus.Message);
        }
        /// <summary>
        /// Busca todos los parametros de la compañia por Item
        /// </summary>
        private void UpdateSession(bool showError, bool updateItem = false)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError && cfgParameterViewDTO.Errors != null)
            {
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
                cfgParameterViewDTO.ClearError();
                ucStatus.ShowMessage(cfgParameterViewDTO.MessageStatus.Message);
            }

            // Muestra primero el error generado en la operacion anterior
            if (showError && parameterHistoryViewDTO.Errors != null)
            {
                this.Master.ucError.ShowError(parameterHistoryViewDTO.Errors);
                parameterHistoryViewDTO.ClearError();
                ucStatus.ShowMessage(parameterHistoryViewDTO.MessageStatus.Message);
            }
        
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            switch (level)
            {
                case 0:
                    cfgParameterViewDTO = iConfigurationMGR.FindAllParamsItemsCompany(context);
                    break;

                case 1:
                    cfgParameterViewDTO = iConfigurationMGR.GetParamsItemsOwner(context, currentIdOwner);
                    break;

                case 2:
                    if (updateItem)
                        cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndItem(context, currentIdOwner, currentIdWhs, currentIdItem);
                    else
                        cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndItem(context, currentIdOwner, currentIdWhs);
                    break;

                case 3:
                    cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndGrp1(context, currentIdOwner, currentIdWhs, idGrpItem1);
                    break;

                case 4:
                    cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndGrp2(context, currentIdOwner, currentIdWhs, idGrpItem1, idGrpItem2);
                    break;

                case 5:
                    cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndGrp3(context, currentIdOwner, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3);
                    break;

                case 6:
                    cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndGrp4(context, currentIdOwner, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4);
                    break;

                default:
                    cfgParameterViewDTO = iConfigurationMGR.FindAllParamsItemsCompany(context);
                    break;
            }

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemParametersMgr.CfgParameterList, cfgParameterViewDTO);

                isValidViewDTO = true;

                //Muestra el mensaje en la barra de estado.
                if (!crud)
                    ucStatus.ShowMessage(cfgParameterViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        /// <summary>
        /// Actualiza lista de items a mostrar
        /// </summary>
        private void UpdateItemSession()
        {

            // Recupera lista de items
            if (parameterValue == "-1") // 'Todos'
                itemViewDTO = iWarehousingMGR.GetItemBySpecialParameters(IdOwner, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, code, name, description, context);
            else
                itemViewDTO = iWarehousingMGR.GetItemBySpecialParamAndParameterValue(idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, code, name, description, currentIdOwner, currentIdWhs, Convert.ToInt32(idParameter), parameterValue, context);

            if (!itemViewDTO.hasError())
            {
                isValidViewDTO = true;

                Session.Add(WMSTekSessions.ItemParametersMgr.ItemList, itemViewDTO);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        /// <summary>
        /// Trae los parametros por id de owner
        /// </summary>
        private void GetParametersOwnerById(bool showError, int idOwn)
        {
            cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
                cfgParameterViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            cfgParameterViewDTO = iConfigurationMGR.GetParamsItemsOwner(context, idOwn);

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemParametersMgr.CfgParameterList, cfgParameterViewDTO);
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

            cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndItem(context, idOwn, idWhs);

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemParametersMgr.CfgParameterList, cfgParameterViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }
        private void GetParamGrp1ByOwnWhs(bool showError, int idOwn, int idWhs, int idGrp1)
        {
            cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
                cfgParameterViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndGrp1(context, idOwn, idWhs, idGrp1);

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemParametersMgr.CfgParameterList, cfgParameterViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }
        private void GetParamGrp2ByOwnWhs(bool showError, int idOwn, int idWhs, int idGrp1, int idGrp2)
        {
            cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
                cfgParameterViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndGrp2(context, idOwn, idWhs, idGrp1, idGrp2);

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemParametersMgr.CfgParameterList, cfgParameterViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }
        private void GetParamGrp3ByOwnWhs(bool showError, int idOwn, int idWhs, int idGrp1, int idGrp2, int idGrp3)
        {
            cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
                cfgParameterViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndGrp3(context, idOwn, idWhs, idGrp1, idGrp2, idGrp3);

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemParametersMgr.CfgParameterList, cfgParameterViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }
        private void GetParamGrp4ByOwnWhs(bool showError, int idOwn, int idWhs, int idGrp1, int idGrp2, int idGrp3, int idGrp4)
        {
            cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
                cfgParameterViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndGrp4(context, idOwn, idWhs, idGrp1, idGrp2, idGrp3, idGrp4);

            if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemParametersMgr.CfgParameterList, cfgParameterViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
            }
        }

        private void PopulateGridItems()
        {
           // grdItem.EmptyDataText = this.Master.EmptyGridText;
            grdItem.PageIndex = currentPage;

            if (itemViewDTO != null)
            {
                // Configura ORDEN de las columnas de la grilla
                if (!itemViewDTO.hasConfigurationError() && itemViewDTO.Configuration != null && itemViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdItem, itemViewDTO.Configuration);

                grdItem.DataSource = itemViewDTO.Entities;
                grdItem.DataBind();

                CallJsGridViewDetail();

                upGrid.Update();

                //Se configura para mostrar la barra de estado
                ucStatus.ShowRecordInfo(itemViewDTO.Entities.Count, grdItem.PageSize, grdItem.PageCount, currentPage, grdItem.AllowPaging);
            }

            divDetail.Visible = true;
        }

        private void PopulateGrid()
        {

            grdMgr.PageIndex = currentPage;

            cfgParameterViewDTO = (GenericViewDTO<CfgParameter>)Session[WMSTekSessions.ItemParametersMgr.CfgParameterList];

            // Configura ORDEN de las columnas de la grilla
            if (!cfgParameterViewDTO.hasConfigurationError() && cfgParameterViewDTO.Configuration != null && cfgParameterViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, cfgParameterViewDTO.Configuration);

            grdMgr.DataSource = cfgParameterViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            //ucStatus.ShowMessage(context.StatusBarMessage);
            //ucStatus.ShowRecordInfo(cfgParameterViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            if (cfgParameterViewDTO.Entities.Count > 0)
            {
                if (currentNameOwner == string.Empty)
                    currentNameOwner = objUser.Company.Name;

                this.lblTitle.Text = currentNameOwner + currentNameWarehouse + GetCurrentItem();
            }
            else
                this.lblTitle.Text = string.Empty;

            UpdatePanel1.Update();

        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.nameVisible = false;
            this.Master.ucMainFilter.descriptionVisible = false;
            this.Master.ucMainFilter.searchVisible = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
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

            grdItem.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdItem.EmptyDataText = this.Master.EmptyGridText;
        }

        protected void ReloadData()
        {
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        /// <summary>
        /// Recorre la grilla de items, y obtiene el ID de los items seleccionados
        /// </summary>
        private void CollectItemsSelected()
        {
            if (grdItem.Visible == true)
            {
                int post = grdItem.PageSize * grdItem.PageIndex;

                for (int i = 0; i < grdItem.Rows.Count; i++)
                {
                    GridViewRow row = grdItem.Rows[i];

                    if (((CheckBox)row.FindControl("chkSelectItem")).Checked)
                    {
                        checkedItemsCurrentView.Add(itemViewDTO.Entities[post + i].Id);                        
                    }
                }
            }


            //if (grdItem.Visible == true)
            //{
            //    int post = grdItem.PageSize * grdItem.PageIndex;

            //    for (int i = 0; i < grdItem.Rows.Count; i++)
            //    {
            //        GridViewRow row = grdItem.Rows[i];

            //        if (((CheckBox)row.FindControl("chkSelectItem")).Checked)
            //        {
            //            itemViewDTO.Entities[post + i].Id

            //            Label lblId = (Label)row.FindControl("lblIdItem");

            //            if (lblId != null && lblId.Text != string.Empty)
            //            {
            //                if (lblId.Text != string.Empty)
            //                {
            //                    checkedItemsCurrentView.Add(Convert.ToInt32(lblId.Text));
            //                }
            //            }
            //        }
            //    }
            //}
        }

        protected void SaveChanges()
        {
            bool Error = true;
            bool Equal = true;
            parameters = new GenericViewDTO<CfgParameter>();
            parameterHistoryViewDTO = new GenericViewDTO<ParameterHistory>();
            
            CollectItemsSelected();

            foreach (GridViewRow row in grdMgr.Rows)
            {
                CfgParameter cfgParameter = new CfgParameter();
                cfgParameter.Owner = new Owner();
                cfgParameter.Warehouse = new Warehouse();
                cfgParameter.Item = new Item();
                cfgParameter.Item.GrpItem1 = new GrpItem1();
                cfgParameter.Item.GrpItem2 = new GrpItem2();
                cfgParameter.Item.GrpItem3 = new GrpItem3();
                cfgParameter.Item.GrpItem4 = new GrpItem4();
                ParameterHistory history = new ParameterHistory();

                int idParameter = int.Parse(((Label)row.FindControl("lblIdParameter")).Text);
                var parameterToCompare = cfgParameterViewDTO.Entities.Where(param => param.Id == idParameter).First();

                //Busca los controles y asigna sus valores
                history.ParamValueBefore = parameterToCompare.ParameterValue;
                cfgParameter.Id = parameterToCompare.Id;
                cfgParameter.ParameterValue = Server.HtmlEncode(((TextBox)row.FindControl("txtParameterValue")).Text);
                cfgParameter.ParameterCode = parameterToCompare.ParameterCode;
                cfgParameter.AllowEdit = parameterToCompare.AllowEdit;
                cfgParameter.Type = parameterToCompare.Type;
                cfgParameter.MaxValue = parameterToCompare.MaxValue;
                cfgParameter.MinValue = parameterToCompare.MinValue;
                cfgParameter.DefaultValue = parameterToCompare.DefaultValue;
                cfgParameter.Owner.Id = currentIdOwner;
                cfgParameter.Warehouse.Id = currentIdWhs;
                cfgParameter.Item.Id = parameterToCompare.Item.Id;
                cfgParameter.Item.GrpItem1.Id = idGrpItem1;
                cfgParameter.Item.GrpItem2.Id = idGrpItem2;
                cfgParameter.Item.GrpItem3.Id = idGrpItem3;
                cfgParameter.Item.  GrpItem4.Id = idGrpItem4;
                cfgParameter.Overwrite = ((CheckBox)row.FindControl("chkOverwrite")).Checked;

                //Si es editable y el valor del parametro es distinto al que tiene antes entonces lo agrega a la lista para actualizarlo
                if (cfgParameter.AllowEdit && 
                    ((cfgParameter.ParameterValue != history.ParamValueBefore) || ((parameterToCompare.Different > 0) && (cfgParameter.Overwrite && checkedItemsCurrentView.Count > 0))))
                {
                    GenericViewDTO<CfgParameter> paramValidator = new GenericViewDTO<CfgParameter>();

                    paramValidator = GenericValidate(cfgParameter.Type, cfgParameter.ParameterValue, cfgParameter.MaxValue, cfgParameter.MinValue);
                    if (!paramValidator.hasError())
                    {
                        parameters.Entities.Add(cfgParameter);
                        parameterHistoryViewDTO.Entities.Add(history);
                        Error = false;
                        Equal = false;
                    }
                    else
                    {
                        isValidViewDTO = false;
                        Error = true;
                        cfgParameterViewDTO.Errors = cfgParameterViewDTO.Errors;
                        cfgParameterViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Ready, context));
                        break;
                    }
                }
            }
            if (!Error && !Equal)
            {
                if (checkedItemsCurrentView.Count > 0)
                {
                    CreateFilterItems();
                    currentIdItem = checkedItemsCurrentView.First();
                }
                else
                    currentIdItem = -1;

                //Termina de recorrer y Salva
                cfgParameterViewDTO = iConfigurationMGR.MaintainCfgParameter(CRUD.Update, parameters, parameterHistoryViewDTO, level, checkedItemsCurrentView.Count > 0, context);

                //Marca si alguno tuvo error
                if (cfgParameterViewDTO.hasError())
                {
                    UpdateSession(true);
                }
                else
                {
                    crud = true;
                    UpdateSession(false, checkedItemsCurrentView.Count > 0);
                }
            }
            else
            {
                parameterHistoryViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Save, context));
                UpdateSession(true);
            }
        }

        private void CreateFilterItems()
        {
            int i = 0;
            foreach (EntityFilter entityFilter in context.MainFilter)
            {
                if (entityFilter.Name == "Item")
                {
                    entityFilter.FilterValues.Clear();

                    foreach (int idItem in checkedItemsCurrentView)
                    {
                        FilterItem filterGrpItem1 = new FilterItem();
                        filterGrpItem1.Index = 0;
                        filterGrpItem1.Name = null;
                        filterGrpItem1.Selected = false;
                        filterGrpItem1.Value = string.Empty;
                        filterGrpItem1.Value = idItem.ToString();
                        entityFilter.FilterValues.Add(filterGrpItem1);
                    }
                    break;
                }
                i++;
            }
        }


        protected void grdItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void grdItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = grdItem.PageSize * grdItem.PageIndex + grdItem.SelectedIndex;

                //Label lblIdItem = (Label)grdItem.Rows[index].FindControl("lblIdItem") as Label;

                int idItem = itemViewDTO.Entities[index].Id; //int.Parse(lblIdItem.Text.Trim());

                var cfgItemViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndItem(context, currentIdOwner, currentIdWhs, idItem);

                if (!cfgItemViewDTO.hasError() && cfgItemViewDTO.Entities.Count > 0)
                {
                    if (ValidateSession(WMSTekSessions.ItemParametersMgr.CfgParameterList))
                    {
                        Session.Remove(WMSTekSessions.ItemParametersMgr.CfgParameterList);
                        showItems = true;
                    }

                    currentIdItem = idItem;
                    Session.Add(WMSTekSessions.ItemParametersMgr.CfgParameterList, cfgItemViewDTO);
                }
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }


        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;                
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdItem.PageCount - 1;
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void grdItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }


        private void loadFristTree()
        {
            // Muestra siempre grilla de Items (excepto para nivel 0 (company) y 1 (owner)
            showItems = true;
            this.divFilterItem.Visible = true;
            this.divDetail.Visible = true;    

            //Obtiene el path dentro del arbol
            string TreeId = HttpUtility.UrlEncode(this.treevLocation.Nodes[0].ValuePath);

            // Verifica que el arbol no sea nulo
            if (!string.IsNullOrEmpty(TreeId))
            {

                showItems = false;

                // Busca el nodo al cual le hicimos clic
                TreeNode company = treevLocation.FindNode(TreeId);

                // Comprueba que no este vacio el nodo
                if (null != company && company.ChildNodes.Count == 0)
                {
                    // Setea el nombre del nodo principal y el tooltip con el nombre  de la compañia
                    company.Text = objUser.Company.Name;
                    company.ToolTip = objUser.Company.Name;

                    // Busca los Owners asociados al usuario actual
                    ownViewDTO = iWarehousingMGR.GetOwnersByUser(context, context.SessionInfo.User.Id);

                    // Busca todos los warehouses
                    whsViewDTO = iLayoutMGR.GetWarehouseByUser(context.SessionInfo.User.Id, context);

                    //Carga el Tree y muestra la lista de Owners
                    if (!ownViewDTO.hasError() && ownViewDTO.Entities != null)
                    {
                        foreach (Owner own in ownViewDTO.Entities)
                        {
                            //Carga el nombre y el Id del Owner en el Tree
                            TreeNode nodoOwn = new TreeNode(own.Name.ToString(), own.Id.ToString());
                            company.ChildNodes.Add(nodoOwn);

                            foreach (Warehouse whs in whsViewDTO.Entities)
                            {
                                //Agrega el nuevo nodo de tipo warehouse
                                TreeNode nodoWhs = new TreeNode(whs.ShortName.ToString(), whs.Id.ToString());
                                nodoOwn.ChildNodes.Add(nodoWhs);
                            }

                            nodoOwn.Collapse();
                        }
                    }
                }

                //Setea los owners y warehouses
                currentNameOwner = objUser.Company.Name;
                currentNameWarehouse = string.Empty;

                //Esconde todos los demas controles
                HideFiltersAndItems();
                ReloadData();
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('CfgParameterItemsCompany_FindAll', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl00_ctl01_grdMgr');", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDropCustom();", true);
        }

        private string GetCurrentItem()
        {
            string itemName = string.Empty;
            var idItem = currentIdItem;

            if (idItem > 0)
            {
                var itemViewDTO = iWarehousingMGR.GetByIdAndOwner(context, currentIdOwner, idItem);

                if (!itemViewDTO.hasError() && itemViewDTO.Entities.Count > 0)
                {
                    itemName = "  >  " + itemViewDTO.Entities.First().LongName;
                }
            }

            return itemName;
        }

        //public static void KeepSelection(GridView grid)
        //{
        //    //
        //    // se obtienen los id de producto checkeados de la pagina actual
        //    //
        //    List<int> checkedProd = (from item in grid.Rows.Cast<GridViewRow>()
        //                             let check = (CheckBox)item.FindControl("chkSelectItem")
        //                             where check.Checked
        //                             select Convert.ToInt32(grid.DataKeys[item.RowIndex].Value)).ToList();

        //    //
        //    // se recupera de session la lista de seleccionados previamente
        //    //
        //    List<int> productsIdSel = HttpContext.Current.Session["ProdSelection"] as List<int>;

        //    if (productsIdSel == null)
        //        productsIdSel = new List<int>();

        //    //
        //    // se cruzan todos los registros de la pagina actual del gridview con la lista de seleccionados,
        //    // si algun item de esa pagina fue marcado previamente no se devuelve
        //    //
        //    productsIdSel = (from item in productsIdSel
        //                     join item2 in grid.Rows.Cast<GridViewRow>()
        //                        on item equals Convert.ToInt32(grid.DataKeys[item2.RowIndex].Value) into g
        //                     where !g.Any()
        //                     select item).ToList();

        //    //
        //    // se agregan los seleccionados
        //    //
        //    productsIdSel.AddRange(checkedProd);

        //    HttpContext.Current.Session["ProdSelection"] = productsIdSel;

        //}

        //public static void RestoreSelection(GridView grid)
        //{

        //    List<int> productsIdSel = HttpContext.Current.Session["ProdSelection"] as List<int>;

        //    if (productsIdSel == null)
        //        return;

        //    //
        //    // se comparan los registros de la pagina del grid con los recuperados de la Session
        //    // los coincidentes se devuelven para ser seleccionados
        //    //
        //    List<GridViewRow> result = (from item in grid.Rows.Cast<GridViewRow>()
        //                                join item2 in productsIdSel
        //                                on Convert.ToInt32(grid.DataKeys[item.RowIndex].Value) equals item2 into g
        //                                where g.Any()
        //                                select item).ToList();

        //    //
        //    // se recorre cada item para marcarlo
        //    //
        //    result.ForEach(x => ((CheckBox)x.FindControl("chkSelectItem")).Checked = true);

        //}
        #endregion
    }
}
