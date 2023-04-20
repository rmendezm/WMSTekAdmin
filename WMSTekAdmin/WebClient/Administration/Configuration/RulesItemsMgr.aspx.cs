using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Configuration;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Rules;

namespace Binaria.WMSTek.WebClient.Administration.Configuration
{
    public partial class RulesItemsMgr : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<CustomRule> customRuleViewDTO;
        private GenericViewDTO<CustomRuleItem> customRuleItemViewDTO;

        //private GenericViewDTO<CfgParameter> cfgParameterViewDTO;
        //private GenericViewDTO<ParameterHistory> parameterHistoryViewDTO;
        private GenericViewDTO<Owner> ownViewDTO;
        private GenericViewDTO<Warehouse> whsViewDTO;
        //private GenericViewDTO<CfgParameter> parameters;
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
        //private List<int> checkedItemsCurrentView = new List<int>();
        private List<CustomRule> checkedCustomRuleCurrentView = new List<CustomRule>();

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
        public string currentName
        {
            get
            {
                if (ValidateViewState("name"))
                    return (string)ViewState["name"];
                else
                    return string.Empty;
            }

            set { ViewState["name"] = value; }
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
                            treevLocation.Font.Bold = true;
                            level = 0;
                            loadFristTree();
                        }

                        //Elimina si ha quedado algo de antes y carga inicial del ViewDTO
                        showItems = false;
                        //ViewState.Remove("SessionDtoCfgParameterList");
                        UpdateSession(false);
                    }

                    // Recupera lista de parámetros
                    if (ValidateSession(WMSTekSessions.RulesItemsMgr.CustomRulesList))
                    {
                        customRuleViewDTO = (GenericViewDTO<CustomRule>)Session[WMSTekSessions.RulesItemsMgr.CustomRulesList];
                        isValidViewDTO = true;
                    }

                    // Recupera lista de items
                    if (ValidateSession(WMSTekSessions.RulesItemsMgr.ItemList))
                    {
                        itemViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.RulesItemsMgr.ItemList];
                    }

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                        PopulateGrid();
                        //if (showItems) PopulateGridItems();
                    }
                }
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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

                //Modifica el Ancho del Div de los Graficos
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                    // Muestra siempre grilla de Items (excepto para nivel 0 (company) y 1 (owner)
                    //showItems = true;
                    ClearFilter("IdOwn");

                    this.divFilterItem.Visible = true;
                    this.divDetail.Visible = true;
                    showItems = false;

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
                            // Nivel 1: Company (clic en Company)
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

                                    // Busca todos los warehouses
                                    whsViewDTO = iLayoutMGR.GetWarehouseByUser(context.SessionInfo.User.Id, context);

                                    //Carga el Tree y muestra la lista de Owners
                                    if (!ownViewDTO.hasError() && ownViewDTO.Entities != null)
                                    {
                                        foreach (Warehouse whs in whsViewDTO.Entities)
                                        {
                                            //Agrega el nuevo nodo de tipo warehouse
                                            TreeNode nodoWhs = new TreeNode(whs.ShortName.ToString(), whs.Id.ToString());
                                            company.ChildNodes.Add(nodoWhs);
                                        }
                                    }
                                }

                                //Setea los owners y warehouses
                                currentName = objUser.Company.Name;
                                currentNameWarehouse = objUser.Company.Name;

                                //Esconde todos los demas controles
                                HideFiltersAndItems();
                                ReloadData();
                                break;

                            //Nivel 2: Warehouses (clic en warehouse -> Muestra lista Grp1)
                            case 1:
                                //Setea los centros y el label de la ruta
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Text;
                                currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Text;

                                //Actualiza los id segun lo cliqueado en el treeview
                                //currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Value);
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
                                //base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);

                                //if (ddlBscGrpItm1.Items.Count <= 1)
                                //{
                                    this.divBscGrpItm1.Visible = false;
                                //}
                                //else
                                //{
                                //    //Muestra el ddl del siguiente nivel
                                //    this.divBscGrpItm1.Visible = true;
                                //    this.ddlBscGrpItm1.Visible = true;
                                //    this.lblTitleGrpItm1.Visible = true;
                                //    this.lblNameGrp1.Visible = false;
                                //}

                                //Carga grilla con los parametros por owner y warehouse
                                customRuleViewDTO = iRulesMGR.GetCustomRuleByIdWhs(currentIdWhs, context);

                                if (!customRuleViewDTO.hasError() && customRuleViewDTO.Entities.Count > 0)
                                {
                                    Session.Add(WMSTekSessions.RulesItemsMgr.CustomRulesList, customRuleViewDTO);
                                }

                                //Busca los GrpItem1(sectores) que tengan algun cambio
                                ownViewDTO = iWarehousingMGR.GetOwnersByUser(context, context.SessionInfo.User.Id);

                                //Limpia el arbol
                                tv.SelectedNode.ChildNodes.Clear();

                                //verifica que el nodo warehouse no venga null y arma el treeview
                                if (tv.SelectedNode.ChildNodes != null)
                                {
                                    //recorre el GroupItem1
                                    foreach (Owner own in ownViewDTO.Entities)
                                    {
                                        //Agrega el nuevo nodo de tipo warehouse
                                        TreeNode nodoOwn = new TreeNode(own.Name.ToString(), own.Id.ToString());
                                        tv.SelectedNode.ChildNodes.Add(nodoOwn);
                                    }
                                }

                                break;

                            //Nivel 3: Owner (clic en Owner -> Muestra lista Grp1)
                            case 2:
                                showItems = true;
                                //Setea los centros y el label de la ruta
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Text;
                                currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Text;

                                //currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Text;
                                //currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Text;

                                //Actualiza los id segun lo cliqueado en el treeview
                                currentIdWhs = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Value);

                                CreateFilterByList("IdOwn", new List<int> { currentIdOwner });

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
                                customRuleViewDTO = iRulesMGR.GetCustomRuleByIdOwn(currentIdWhs, currentIdOwner, context);

                                if (!customRuleViewDTO.hasError() && customRuleViewDTO.Entities.Count > 0)
                                {
                                    Session.Add(WMSTekSessions.RulesItemsMgr.CustomRulesList, customRuleViewDTO);
                                }

                                //Busca los GrpItem1(sectores) que tengan algun cambio
                                grpItem1ViewDTO = iWarehousingMGR.GetGrpItem1Treeview(context, currentIdOwner);
                                //grpItem1ViewDTO = new GenericViewDTO<GrpItem1>(); ////iWarehousingMGR.FindAll(context); //TODO 

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

                            //Nivel 4: Muestra lista de Sectores y habilita y carga los filtros.(clic en Grp1(sector) --> Muestra lista GroupItem2 (rubro))
                            case 3:
                                showItems = true;
                                //Setea los centros y el label de la ruta
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Text;
                                currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Text;

                                //Actualiza los id segun lo cliqueado en el treeview
                                currentIdWhs = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                                idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Value);

                                CreateFilterByList("IdOwn", new List<int> { currentIdOwner });

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
                                GetParamGrp1ByWhs(false, currentIdWhs, idGrpItem1);

                                //Busca los GrpItem2 que tengan algun cambio para llenar el arbol
                                //grpItem2ViewDTO = iWarehousingMGR.GetGrpItem2Treeview(context, currentIdOwner, idGrpItem1);
                                grpItem2ViewDTO = iWarehousingMGR.GetByIdGrpItem1(context, idGrpItem1);

                                //Limpia el arbol
                                tv.SelectedNode.ChildNodes.Clear();

                                //verifica que el nodo Grp1 no venga null y arma el treeview con los Grp1
                                if (tv.SelectedNode.ChildNodes != null)
                                {
                                    //recorre el GroupItem2
                                    foreach (GrpItem2 grpItem2 in grpItem2ViewDTO.Entities)
                                    {
                                        //Agrega el nuevo nodo de tipo warehouse
                                        TreeNode nodoGrp2 = new TreeNode(grpItem2.Name.ToString(), grpItem2.Id.ToString());
                                        tv.SelectedNode.ChildNodes.Add(nodoGrp2);
                                    }
                                }
                                break;

                            //Nivel 5: GroupItem2 (clic en un GroupItem2 -> Muestra lista GroupItem3 familia)
                            case 4:
                                showItems = true;
                                //Setea los centros y el label de la ruta
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Text;
                                currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Text;

                                //Actualiza los id según lo cliqueado en el tree
                                currentIdWhs = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Value);
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                                idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                                idGrpItem2 = Convert.ToInt32(tv.SelectedNode.Value);

                                CreateFilterByList("IdOwn", new List<int> { currentIdOwner });

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

                                //Carga grilla con los parametros por  warehouse y Grp2
                                GetParamGrp2ByOwnWhs(false, currentIdWhs, idGrpItem1, idGrpItem2);

                                //Busca los GrpItem3 que tengan algun cambio para llenar el arbol
                                //grpItem3ViewDTO = iWarehousingMGR.GetGrpItem3Treeview(context, currentIdOwner, idGrpItem1, idGrpItem2);
                                grpItem3ViewDTO = iWarehousingMGR.GetByIdGrpItem1AndIdGrpItem2(context, idGrpItem1, idGrpItem2);

                                //Limpia el arbol
                                tv.SelectedNode.ChildNodes.Clear();

                                //verifica que el nodo Grp2 no venga null y arma el treeview
                                if (tv.SelectedNode.ChildNodes != null)
                                {
                                    //recorre el GroupItem3
                                    foreach (GrpItem3 grpItem3 in grpItem3ViewDTO.Entities)
                                    {
                                        //Agrega el nuevo nodo de tipo warehouse
                                        TreeNode nodoGrp3 = new TreeNode(grpItem3.Name.ToString(), grpItem3.Id.ToString());
                                        tv.SelectedNode.ChildNodes.Add(nodoGrp3);
                                    }
                                }
                                break;

                            //Nivel 6: GroupItem3 (clic en un GroupItem3 -> Muestra lista GroupItem4 subFamilia)
                            case 5:
                                showItems = true;
                                //Actualiza las propiedades que guardan el nombre segun lo cliqueado en el tree
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Text;
                                currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Text;

                                //Actualiza los id segun lo cliqueado en el tree
                                currentIdWhs = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Parent.Value);
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Value);
                                idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                                idGrpItem2 = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                                idGrpItem3 = Convert.ToInt32(tv.SelectedNode.Value);

                                CreateFilterByList("IdOwn", new List<int> { currentIdOwner });

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
                                GetParamGrp3ByOwnWhs(false, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3);

                                //Busca los GrpItem4 que tengan algun cambio para llenar el arbol
                                //grpItem4ViewDTO = iWarehousingMGR.GetGrpItem4Treeview(context, currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3);
                                grpItem4ViewDTO = iWarehousingMGR.GetByIdGrpItem1IdGrpItem2AndAndIdGrpItem3(context, idGrpItem1, idGrpItem2, idGrpItem3);

                                //Limpia el arbol
                                tv.SelectedNode.ChildNodes.Clear();

                                //verifica que el nodo Grp3 no venga null y arma el treeview
                                if (tv.SelectedNode.ChildNodes != null)
                                {
                                    //recorre el GroupItem4
                                    foreach (GrpItem4 grpItem4 in grpItem4ViewDTO.Entities)
                                    {
                                        //Agrega el nuevo nodo de tipo warehouse
                                        TreeNode nodoGrp4 = new TreeNode(grpItem4.Name.ToString(), grpItem4.Id.ToString());
                                        tv.SelectedNode.ChildNodes.Add(nodoGrp4);
                                    }
                                }
                                break;

                            //Nivel 7: GroupItem4 (clic en un GroupItem4(subfamilia) -> En el arbol no muestra nada mas) Carga filtro de busqueda de Items
                            case 6:
                                showItems = true;
                                //Actualiza las propiedades que guardan el nombre segun lo cliqueado en el tree
                                currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Text + 
                                                       this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Text + 
                                                       this.lblSimbol.Text + tv.SelectedNode.Parent.Text + 
                                                       this.lblSimbol.Text + tv.SelectedNode.Text;
                                currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text;
                                

                                //Actualiza las propiedades que guardan el Id segun lo cliqueado en el tree
                                currentIdWhs = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Parent.Parent.Value);
                                currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Parent.Value);
                                idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Value);
                                idGrpItem2 = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                                idGrpItem3 = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                                idGrpItem4 = Convert.ToInt32(tv.SelectedNode.Value);

                                CreateFilterByList("IdOwn", new List<int> { currentIdOwner });

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
                                GetParamGrp4ByOwnWhs(false, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4);

                                break;
                        }

                        if (showItems) UpdateItemSession();
                    }
                }
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                    GetParamGrp1ByWhs(false, currentIdWhs, idGrpItem1);

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
                                            currentName = objUser.Company.Name + this.lblSimbol.Text + treevLocation.SelectedNode.Parent.Parent.Text;

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
                    GetParamGrp2ByOwnWhs(false, currentIdWhs, idGrpItem1, idGrpItem2);

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
                    GetParamGrp3ByOwnWhs(false, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3);

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
                    GetParamGrp4ByOwnWhs(false, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4);

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
                showItems = true;
                //checkedItemsCurrentView.Clear();
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

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
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
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError && customRuleViewDTO.Errors != null)
            {
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
                customRuleViewDTO.ClearError();
                ucStatus.ShowMessage(customRuleViewDTO.MessageStatus.Message);
            }
                     
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            switch (level)
            {
                case 0:
                    customRuleViewDTO = new GenericViewDTO<CustomRule>();
                    break;

                case 1:

                    customRuleViewDTO = iRulesMGR.GetCustomRuleByIdWhs(currentIdWhs, context);
                    break;

                case 2:
                    customRuleViewDTO = iRulesMGR.GetByIdOwn(currentIdWhs, currentIdOwner, context);
                    break;

                case 3:
                    customRuleViewDTO = iRulesMGR.GetByIdGrpItem1(currentIdWhs, idGrpItem1, context);
                    break;

                case 4:
                    customRuleViewDTO = iRulesMGR.GetByIdGrpItem2(currentIdWhs, idGrpItem1, idGrpItem2, context);
                    break;

                case 5:
                    customRuleViewDTO = iRulesMGR.GetByIdGrpItem3(currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3, context);
                    break;

                case 6:
                    customRuleViewDTO = iRulesMGR.GetByIdGrpItem4(currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, context);
                    break;                
            }

            if (!customRuleViewDTO.hasError() && customRuleViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.RulesItemsMgr.CustomRulesList, customRuleViewDTO);

                isValidViewDTO = true;

                //Muestra el mensaje en la barra de estado.
                if (!crud)
                    ucStatus.ShowMessage(customRuleViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        /// <summary>
        /// Actualiza lista de items a mostrar
        /// </summary>
        private void UpdateItemSession()
        {
            
            itemViewDTO = iWarehousingMGR.GetItemByGroupsAndFilters(idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, code, name, description, context);
           
            if (!itemViewDTO.hasError())
            {
                isValidViewDTO = true;

                Session.Add(WMSTekSessions.RulesItemsMgr.ItemList, itemViewDTO);
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
        //private void GetParametersOwnerById(bool showError, int idOwn)
        //{
        //    cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();

        //    // Muestra primero el error generado en la operacion anterior
        //    if (showError)
        //    {
        //        this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
        //        cfgParameterViewDTO.ClearError();
        //    }

        //    context.MainFilter = this.Master.ucMainFilter.MainFilter;

        //    cfgParameterViewDTO = iConfigurationMGR.GetParamsItemsOwner(context, idOwn);

        //    if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
        //    {
        //        Session.Add(WMSTekSessions.ItemParametersMgr.CfgParameterList, cfgParameterViewDTO);
        //        isValidViewDTO = true;
        //        PopulateGrid();
        //    }
        //    else
        //    {
        //        isValidViewDTO = false;
        //        this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
        //    }
        //}
        //private void GetParametersOwnerWarehouseById(bool showError, int idOwn, int idWhs)
        //{
        //    cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();

        //    // Muestra primero el error generado en la operacion anterior
        //    if (showError)
        //    {
        //        this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
        //        cfgParameterViewDTO.ClearError();
        //    }

        //    context.MainFilter = this.Master.ucMainFilter.MainFilter;

        //    cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndItem(context, idOwn, idWhs);

        //    if (!cfgParameterViewDTO.hasError() && cfgParameterViewDTO.Entities != null)
        //    {
        //        Session.Add(WMSTekSessions.ItemParametersMgr.CfgParameterList, cfgParameterViewDTO);
        //        isValidViewDTO = true;
        //        PopulateGrid();
        //    }
        //    else
        //    {
        //        isValidViewDTO = false;
        //        this.Master.ucError.ShowError(cfgParameterViewDTO.Errors);
        //    }
        //}
        private void GetParamGrp1ByWhs(bool showError, int idWhs, int idGrp1)
        {
            customRuleViewDTO = new GenericViewDTO<CustomRule>();

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
                customRuleViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            customRuleViewDTO = iRulesMGR.GetByIdGrpItem1(idWhs, idGrp1, context);

            if (!customRuleViewDTO.hasError() && customRuleViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.RulesItemsMgr.CustomRulesList, customRuleViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        private void GetParamGrp2ByOwnWhs(bool showError,int idWhs, int idGrp1, int idGrp2)
        {
            customRuleViewDTO = new GenericViewDTO<CustomRule>();

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
                customRuleViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            customRuleViewDTO = iRulesMGR.GetByIdGrpItem2(idWhs, idGrp1, idGrp2, context);

            if (!customRuleViewDTO.hasError() && customRuleViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.RulesItemsMgr.CustomRulesList, customRuleViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }
        private void GetParamGrp3ByOwnWhs(bool showError, int idWhs, int idGrp1, int idGrp2, int idGrp3)
        {
            customRuleViewDTO = new GenericViewDTO<CustomRule>();

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
                customRuleViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            //cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndGrp3(context, idOwn, idWhs, idGrp1, idGrp2, idGrp3);
            customRuleViewDTO = iRulesMGR.GetByIdGrpItem3(idWhs, idGrp1, idGrp2, idGrp3, context);
            
            if (!customRuleViewDTO.hasError() && customRuleViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.RulesItemsMgr.CustomRulesList, customRuleViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }
        private void GetParamGrp4ByOwnWhs(bool showError, int idWhs, int idGrp1, int idGrp2, int idGrp3, int idGrp4)
        {
            customRuleViewDTO = new GenericViewDTO<CustomRule>();

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
                customRuleViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            //cfgParameterViewDTO = iConfigurationMGR.GetParamsAndItemsByWhsOwnAndGrp4(context, idOwn, idWhs, idGrp1, idGrp2, idGrp3, idGrp4);
            customRuleViewDTO = iRulesMGR.GetByIdGrpItem4(idWhs, idGrp1, idGrp2, idGrp3, idGrp4, context);

            if (!customRuleViewDTO.hasError() && customRuleViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.RulesItemsMgr.CustomRulesList, customRuleViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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

                CallJsGridView();

                //Se configura para mostrar la barra de estado
                ucStatus.ShowRecordInfo(itemViewDTO.Entities.Count, grdItem.PageSize, grdItem.PageCount, currentPage, grdItem.AllowPaging);
            }
            else
            {
                // Configura VISIBILIDAD de las columnas de la grilla
                if (!customRuleViewDTO.hasConfigurationError() && customRuleViewDTO.Configuration != null && customRuleViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdItem, customRuleViewDTO.Configuration);
            }
            divDetail.Visible = true;
        }

        private void PopulateGrid()
        {
            customRuleViewDTO = (GenericViewDTO<CustomRule>)Session[WMSTekSessions.RulesItemsMgr.CustomRulesList];

            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!customRuleViewDTO.hasConfigurationError() && customRuleViewDTO.Configuration != null && customRuleViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, customRuleViewDTO.Configuration);

            grdMgr.DataSource = customRuleViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridView();

            if (customRuleViewDTO.Entities.Count > 0)
            {
                if (currentName == string.Empty)
                    currentName = objUser.Company.Name;

                this.lblTitle.Text = currentName + currentNameWarehouse;
            }
            else
                this.lblTitle.Text = string.Empty;

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
        //private void CollectItemsSelected()
        //{
        //    if (grdItem.Visible == true)
        //    {
        //        int post = grdItem.PageSize * grdItem.PageIndex;

        //        for (int i = 0; i < grdItem.Rows.Count; i++)
        //        {
        //            GridViewRow row = grdItem.Rows[i];

        //            if (((CheckBox)row.FindControl("chkSelectItem")).Checked)
        //            {
        //                checkedItemsCurrentView.Add(itemViewDTO.Entities[post + i].Id);
        //            }
        //        }
        //    }
            
        //}

        private void CollectCustomRulesSelected()
        {
            if (this.grdMgr.Visible == true)
            {
                int post = grdMgr.PageSize * grdMgr.PageIndex;

                for (int i = 0; i < grdMgr.Rows.Count; i++)
                {
                    GridViewRow row = grdMgr.Rows[i];

                    CustomRule newCustomRule = customRuleViewDTO.Entities[post + i];

                    if (((CheckBox)row.FindControl("chkSelectItem")).Checked)
                    {
                        customRuleViewDTO.Entities[post + i].AsigRuleItem = 1;
                    }
                    else
                    {
                        customRuleViewDTO.Entities[post + i].AsigRuleItem = -1;
                    }

                    checkedCustomRuleCurrentView.Add(customRuleViewDTO.Entities[post + i]);
                }
            }

        }


        protected void SaveChanges()
        {
            checkedCustomRuleCurrentView.Clear();

            //if (currentIdWhs > -1)
            //{
            //    CollectCustomRulesSelected();

            //    if (checkedCustomRuleCurrentView.Count == 0)
            //    {
            //        ucStatus.ShowMessage(lblNoRuleSelected.Text);
            //        return;
            //    }

            //    customRuleItemViewDTO = iRulesMGR.CreateMassiveCustomRulesAllItems(context, currentIdWhs, checkedCustomRuleCurrentView);

            //    if (customRuleItemViewDTO.hasError())
            //    {
            //        UpdateSession(true);
            //    }
            //    else
            //    {
            //        checkedCustomRuleCurrentView.Clear();
            //        crud = true;
            //        UpdateSession(false);
            //    }
            //}

            if (currentIdWhs > -1 && currentIdOwner > -1)
            {
                CollectCustomRulesSelected();

                customRuleItemViewDTO = iRulesMGR.CreateMassiveCustomRulesItems(context, currentIdWhs, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, currentIdOwner, checkedCustomRuleCurrentView);

                if (customRuleItemViewDTO.hasError())
                {
                    UpdateSession(true);
                }
                else
                {
                    checkedCustomRuleCurrentView.Clear();
                    crud = true;
                    UpdateSession(false);
                }
            }
            else
            {
                if (currentIdWhs > -1 && currentIdOwner == -1)
                {
                    grpItem1ViewDTO = iWarehousingMGR.FindAll(context);

                    if (grpItem1ViewDTO.hasError())
                    {
                        isValidViewDTO = false;
                        this.Master.ucError.ShowError(customRuleViewDTO.Errors);
                    }
                    else
                    {
                        if (grpItem1ViewDTO.Entities == null || grpItem1ViewDTO.Entities.Count == 0)
                        {
                            CollectCustomRulesSelected();
                            customRuleItemViewDTO = iRulesMGR.CreateMassiveCustomRulesAllItems(context, currentIdWhs, checkedCustomRuleCurrentView);

                            if (customRuleItemViewDTO.hasError())
                            {
                                UpdateSession(true);
                            }
                            else
                            {
                                checkedCustomRuleCurrentView.Clear();
                                crud = true;
                                UpdateSession(false);
                            }
                        }
                        else
                        {
                            this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNoGroupSelected.Text, string.Empty);
                            UpdateSession(true);
                        }
                    }
                }
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
                        foreach (Warehouse whs in whsViewDTO.Entities)
                        {
                            //Agrega el nuevo nodo de tipo warehouse
                            TreeNode nodoWhs = new TreeNode(whs.ShortName.ToString(), whs.Id.ToString());
                            company.ChildNodes.Add(nodoWhs);
                            
                            foreach (Owner own in ownViewDTO.Entities)
                            {
                                //Carga el nombre y el Id del Owner en el Tree
                                TreeNode nodoOwn = new TreeNode(own.Name.ToString(), own.Id.ToString());
                                nodoWhs.ChildNodes.Add(nodoOwn);
                            }

                            nodoWhs.Collapse();
                        }

                        //foreach (Owner own in ownViewDTO.Entities)
                        //{
                        //    //Carga el nombre y el Id del Owner en el Tree
                        //    TreeNode nodoOwn = new TreeNode(own.Name.ToString(), own.Id.ToString());
                        //    company.ChildNodes.Add(nodoOwn);

                        //    foreach (Warehouse whs in whsViewDTO.Entities)
                        //    {
                        //        //Agrega el nuevo nodo de tipo warehouse
                        //        TreeNode nodoWhs = new TreeNode(whs.ShortName.ToString(), whs.Id.ToString());
                        //        nodoOwn.ChildNodes.Add(nodoWhs);
                        //    }

                        //    nodoOwn.Collapse();
                        //}
                    }
                }

                //Setea los owners y warehouses
                currentName = objUser.Company.Name;
                currentNameWarehouse = string.Empty;

                //Esconde todos los demas controles
                HideFiltersAndItems();
                ReloadData();
            }
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridWithNoDragAndDrop(true);", true);
        }

        #endregion

    }
}
