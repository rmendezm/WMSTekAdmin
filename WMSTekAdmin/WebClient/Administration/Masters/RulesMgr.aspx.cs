using System;
using System.Collections;
using System.Configuration;

using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Rules;
using System.Web.Services;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Layout;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class RulesMgr : BasePage
    {
        #region "Declaración de Variables"

        private bool isValidViewDTO = false;
        GenericViewDTO<CustomRule> customRuleViewDTO = new GenericViewDTO<CustomRule>();
        GenericViewDTO<GroupRule> groupRuleViewDTO = new GenericViewDTO<GroupRule>();
        GenericViewDTO<Binaria.WMSTek.Framework.Entities.Rules.Rule> ruleViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Rules.Rule>();

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

        /// <summary>
        /// Propiedad para controlar el indice activo en la grilla
        /// </summary>
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


        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        // Carga inicial del ViewDTO
                        UpdateSession(false);
                        hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .35);
                        //if (isValidViewDTO)
                        //{
                        //    //base.FindAllPlaces();
                        //    PopulateLists();
                        //}
                    }

                    if (ValidateSession(WMSTekSessions.Shared.RulesMgr.CustomRuleList))
                    {
                        customRuleViewDTO = (GenericViewDTO<CustomRule>)Session[WMSTekSessions.Shared.RulesMgr.CustomRuleList];
                        isValidViewDTO = true;
                    }

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                        PopulateGrid();
                        LoadDetailCustomGroup();
                    }
                }

            }
            catch(Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                        LoadDetailCustomGroup();
                    }
                }
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
                        LoadDetailCustomGroup();
                    }
                }

                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }



        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeGridDetail();
            InitializeFilterRules();
            PopulateLists();
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnExcelVisible = true;
            Master.ucTaskBar.btnExcelDetailVisible = true;
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

        private void InitializeGridDetail()
        {
            grdDetail.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdDetail.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.WmsProcessTypeVisible = true;
            this.Master.ucMainFilter.FilterWmsProcessAutoPostBack = true;
            this.Master.ucMainFilter.SaveOnIndexChanged = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeFilterRules()
        {
            ucFilterItem.Initialize();
            ucFilterItem.BtnSearchClick += new EventHandler(btnSearchRules_Click);

            ucFilterItem.WidthTxtSearchValue = 250;
            ucFilterItem.FilterCode = this.lblFilterCode.Text;
            ucFilterItem.FilterDescription = this.lblFilterName.Text;
        }

        private void PopulateLists()
        {
            //Carga los combos a utilizar        
            base.LoadWmsProcess(this.ddlProcess, true, this.Master.EmptyRowText);
            base.LoadWarehouses(this.ddlWarehouse, true, this.Master.EmptyRowText, "-1");
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
                    //e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    
                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                    e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                }
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {      
                    //Capturo la posicion de la fila 
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndex = grdMgr.SelectedIndex;
                                   
                    //Trae los kits encontrados
                    customRuleViewDTO = (GenericViewDTO<CustomRule>)Session[WMSTekSessions.Shared.RulesMgr.CustomRuleList];

                    // Limpia el detalle para que se recargue
                    Session.Remove(WMSTekSessions.Shared.RulesMgr.GroupRulesList);
                    
                    this.txtCode.Text = string.Empty;
                    this.txtDescription.Text = string.Empty;
                    this.lblGridDetail.Text = string.Empty;
                    this.lblGridDetail.Text = lblDetailsHead.Text + customRuleViewDTO.Entities[currentIndex].Name;
                    this.divGroupRuleDetail.Visible = true;

                    pnlError.Visible = false;
                    this.lblMesage.Text = string.Empty;

                    //Se activa funcion para el ordenamiento por Drag&Drop
                    ScriptManager.RegisterStartupScript(Page, GetType(), "Success", "ReOrderGridView();", true); 
                }
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }


        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDeleteRuleGroup") as ImageButton;

                    if (btnDelete != null)
                    {
                        btnDelete.OnClientClick = "if(confirm('" + this.lblConfirmDeleteGroupRule.Text + "')==false){return false;}";
                    }
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + this.grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + this.grdDetail.ClientID + "')");

                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    //e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + this.grdDetail.ClientID + "');");
                    //e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdDetail, "Select$" + e.Row.RowIndex);
                }

                ImageButton btnUp = e.Row.FindControl("btnUp") as ImageButton;
                ImageButton btnDown = e.Row.FindControl("btnDown") as ImageButton;

                if (btnUp != null) btnUp.CommandArgument = e.Row.DataItemIndex.ToString();
                if (btnDown != null) btnDown.CommandArgument = e.Row.DataItemIndex.ToString();

                // Deshabilita la opcion de Subir si es el primer registro
                if (btnUp != null && e.Row.DataItemIndex == 0)
                {
                    btnUp.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_up_dis.png";
                    btnUp.Enabled = false;
                }

                // Deshabilita la opcion de Bajar si es el ultimo registro
                if (btnDown != null && e.Row.DataItemIndex == groupRuleViewDTO.Entities.Count - 1)
                {
                    btnDown.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_down_dis.png";
                    btnDown.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        /// <summary>
        /// Edita la cantidad de un item en el detalle
        /// </summary>
        protected void grdDetail_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                //EditItem(e.NewEditIndex);
                //editMode = true;
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        /// <summary>
        /// Elimina un item del detalle
        /// </summary>
        protected void grdDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //GroupRule groupRule = new GroupRule();
                //Calcula la posicion en el ViewDTO de la fila a eliminar
                int deleteIndex = grdDetail.PageSize * grdDetail.PageIndex + e.RowIndex;

                if (groupRuleViewDTO.Entities.Count() == 1)
                {
                    DeleteRowRule(groupRuleViewDTO, true);
                }
                else
                {
                    groupRuleViewDTO.Entities.Remove(groupRuleViewDTO.Entities[deleteIndex]);
                    DeleteRowRule(groupRuleViewDTO, false);
                }
                
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        protected void grdSearchRules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //Compara si ya esta en la lista (repetido)
                groupRuleViewDTO = (GenericViewDTO<GroupRule>)Session[WMSTekSessions.Shared.RulesMgr.GroupRulesList];
                int editId = (Convert.ToInt32(this.grdSearchRules.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));
                bool isRepeated = false;
                if (groupRuleViewDTO != null)
                {
                    if (groupRuleViewDTO.Entities != null)
                    {
                        foreach (GroupRule kitdetail in groupRuleViewDTO.Entities)
                        {
                            if (kitdetail.Rule.Id == editId)
                            {
                                isRepeated = true;
                                // Esto evita un bug de ajax
                                pnlError.Visible = false;
                                valAddRule.Enabled = true;
                                valSearchRule.Enabled = true;
                                break;
                            }
                        }
                        if (!isRepeated)
                        {
                            //No existe en la lista por lo tanto se muestra en los cuadros de texto (antes de insertar se debe poner la cantidad)
                            //Recorre la lista para obtener la descripcion
                            if (ValidateSession(WMSTekSessions.Shared.RulesMgr.RulesList))
                            {
                                //Trae la lista de busqueda
                                ruleViewDTO = (GenericViewDTO<Rule>)Session[WMSTekSessions.Shared.RulesMgr.RulesList];

                                //recorre los items de la lista de busqueda para agregar el seleccionado
                                foreach (Rule rule in ruleViewDTO.Entities)
                                {
                                    if (rule.Id == editId)
                                    {
                                        this.txtCode.Text = rule.Code;
                                        this.txtDescription.Text = rule.Name;
                                        //cierra el modal popup de las reglas
                                        this.mpLookupRule.Hide();
                                        Session["RuleDetailNewRule"] = rule;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Envia un mensaje de advertencia y NO inserta
                            pnlError.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        protected void grdSearchRules_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    //e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdSearchRules.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdSearchRules.ClientID + "')");

                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdSearchRules.ClientID + "');");
                   // e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdSearchRules, "Select$" + e.Row.RowIndex);
                }
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

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
        /// Busca los Kits que existen en el sistema
        /// </summary>
        protected void btnSearchRules_Click(object sender, EventArgs e)
        {
            try
            {
                SearchRules();
                // Si es un ViewDTO valido, carga la grilla y las listas
                if (isValidViewDTO)
                {
                    mpLookupRule.Show();
                }
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }


        protected void btnUpdateSequence_Click(object sender, EventArgs e)
        {
            try
            {
                pnlError.Visible = false;

                if (ValidateSession(WMSTekSessions.Shared.RulesMgr.GroupRulesList))
                {
                    var newGroupRules = new GenericViewDTO<GroupRule>();

                    groupRuleViewDTO = (GenericViewDTO<GroupRule>)Session[WMSTekSessions.Shared.RulesMgr.GroupRulesList];

                    if (groupRuleViewDTO != null && groupRuleViewDTO.Entities.Count > 0)
                    {
                        int ruleOrder = 1;

                        foreach (var groupRule in groupRuleViewDTO.Entities)
                        {
                            groupRule.SequenceExecution = ruleOrder;

                            newGroupRules.Entities.Add(groupRule);
                            ruleOrder++;
                        }

                        groupRuleViewDTO = iRulesMGR.MaintainGroupRule(CRUD.Update, newGroupRules, context);

                        if (groupRuleViewDTO.hasError())
                            UpdateSession(true);
                        else
                        {
                            Session.Remove(WMSTekSessions.Shared.RulesMgr.GroupRulesList);
                            Session.Add(WMSTekSessions.Shared.RulesMgr.GroupRulesList, newGroupRules);

                            ucStatus.ShowMessage(groupRuleViewDTO.MessageStatus.Message);
                            LoadDetailCustomGroup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                groupRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(groupRuleViewDTO.Errors);
            }
        }

        protected void imgBtnAddRule_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                bool addItem = true;
                int sequence = 1;

                ruleViewDTO = new GenericViewDTO<Rule>();
                GroupRule newGroupRuleDetail = new GroupRule();
                GenericViewDTO<GroupRule> newGroupRuleDetailAux = new GenericViewDTO<GroupRule>();  

                // Si ya existen reglas y detalles los trae
                groupRuleViewDTO = (GenericViewDTO<GroupRule>)Session[WMSTekSessions.Shared.RulesMgr.GroupRulesList];
                

                // Crea el nuevo detalle del grupo de reglas
                Rule newRule = new Rule();
                newRule = (Rule)Session["RuleDetailNewRule"];
                newGroupRuleDetail.Rule = newRule;
                
                if (groupRuleViewDTO != null)
                {
                    //recorre el las Reglas existentes y compara con el que se quiere agregar
                    if (groupRuleViewDTO.Entities != null && groupRuleViewDTO.Entities.Count > 0)
                    {
                        //Optenemos la secuencia que necesita la nueva regla
                        sequence = (groupRuleViewDTO.Entities.Max(max => max.SequenceExecution) + 1);

                        foreach (GroupRule rule in groupRuleViewDTO.Entities)
                        {
                            // Si ya existe en el detalle y ademas tiene la misma cantidad, se avisa
                            if (rule.Rule.Id == newRule.Id)
                            {
                                pnlError.Visible = true;
                                addItem = false;
                                this.lblMesage.Text = this.lblErrRuleAsig.Text;
                                break;
                            }
                        }
                    }
                    else
                    {
                        addItem = true;
                    }
                }


                //si viene de selectedChanged
                if (currentIndex != -1)
                {
                    newGroupRuleDetail.IdCustomRule = customRuleViewDTO.Entities[currentIndex].Id;
                    newGroupRuleDetail.WmsProcess = customRuleViewDTO.Entities[currentIndex].WmsProcess;
                }

                //Asigna la secuencia a la nueva regla
                newGroupRuleDetail.SequenceExecution = sequence;


                if (addItem)
                {
                    GenericViewDTO<GroupRule> viewGroupRule = new GenericViewDTO<GroupRule>();
                    viewGroupRule.Entities.Add(newGroupRuleDetail);

                    newGroupRuleDetailAux = iRulesMGR.MaintainGroupRule(CRUD.Create, viewGroupRule, context);

                    if (!newGroupRuleDetailAux.hasError())
                    {
                        groupRuleViewDTO.Entities.Add(newGroupRuleDetail);
                        Session.Add(WMSTekSessions.Shared.RulesMgr.GroupRulesList, groupRuleViewDTO);

                        if (!groupRuleViewDTO.hasConfigurationError() && groupRuleViewDTO.Configuration != null && groupRuleViewDTO.Configuration.Count > 0)
                            base.ConfigureGridOrder(grdDetail, groupRuleViewDTO.Configuration);

                        this.grdDetail.DataSource = groupRuleViewDTO.Entities;
                        this.grdDetail.DataBind();

                        crud = true;
                        ucStatus.ShowMessage(newGroupRuleDetailAux.MessageStatus.Message);
                    }
                    else
                    {
                        crud = false;
                        this.Master.ucError.ShowError(newGroupRuleDetailAux.Errors);
                    }
                }
                

                // Limpia paneles Nuevo Item
                txtCode.Text = string.Empty;
                txtDescription.Text = string.Empty;
                pnlError.Visible = false;

                //if (ruleViewDTO.hasError())
                //    UpdateSession(true);
                //else
                //    UpdateSession(false);

            }
            catch (Exception ex)
            {
                groupRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(groupRuleViewDTO.Errors);
            }
        }

        protected void imgBtnAsigRule_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                bool addItem = true;
                customRuleViewDTO = new GenericViewDTO<CustomRule>();
                CustomRule newkitDetail = new CustomRule();
                //newkitDetail.ItemKit = new Item();
                //newkitDetail.ItemBase = new Item();

                // Si ya existen kits y detalles los trae
                customRuleViewDTO = (GenericViewDTO<CustomRule>)Session[WMSTekSessions.Shared.RulesMgr.CustomRuleList];
                groupRuleViewDTO = (GenericViewDTO<GroupRule>)Session[WMSTekSessions.Shared.RulesMgr.GroupRulesList];

                // Crea el nuevo detalle del Kit
                GroupRule newRule = new GroupRule();
                newRule = (GroupRule)Session["RuleDetailNewRule"];


            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }
        

        protected void imgBtnSearchRule_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                bool validRule = false;
                bool existingRule = false;
                                
                if (txtCode.Text.Trim() != string.Empty)
                {
                    string wmsProcess = string.Empty;
                    ContextViewDTO newContext = new ContextViewDTO();
                    newContext.MainFilter = context.MainFilter;

                    //Rescata los valores de los filtros de Busqueda de las reglas
                    //Codigo
                    newContext.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();
                    newContext.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Add(new FilterItem(0, txtCode.Text.Trim()));
                                       
                    //Rescata el index seleccinado en la grilla del grupo
                    //if (currentIndex != 0)
                    //{
                    //    wmsProcess = customRuleViewDTO.Entities[currentIndex].WmsProcess.Code;

                    //    newContext.MainFilter[Convert.ToInt16(EntityFilterName.WmsProcessType)].FilterValues.Clear();
                    //    newContext.MainFilter[Convert.ToInt16(EntityFilterName.WmsProcessType)].FilterValues.Add(new FilterItem(0, wmsProcess));
                    //}

                    //Realiza la busqueda de las reglas por los filtro ingresados
                    ruleViewDTO = iRulesMGR.GetRulesCodeNameWmsProcess(newContext);

                    if (ruleViewDTO.Entities != null)
                    {
                        Rule rule = new Rule();

                        //Verifica si la busqueda es exitosa (trae un solo item) para agregarlo a los controles de detalle
                        if (ruleViewDTO.Entities.Count == 1)
                        {
                            validRule = true;
                            rule = ruleViewDTO.Entities[0];

                            // Mantiene en memoria los datos de la Regla a agregar
                            Session.Add("RuleDetailNewRule", rule);

                            groupRuleViewDTO = (GenericViewDTO<GroupRule>)Session[WMSTekSessions.Shared.RulesMgr.GroupRulesList];

                            if (groupRuleViewDTO != null)
                            {
                                // Recorre los items ya agregados y compara con el que se quiere agregar
                                if (groupRuleViewDTO.Entities != null && groupRuleViewDTO.Entities.Count > 0)
                                {
                                    foreach (GroupRule groupRule in groupRuleViewDTO.Entities)
                                    {
                                        // Si ya existe en la lista se marca
                                        if (groupRule.Rule.Id == rule.Id)
                                        {
                                            existingRule = true;
                                            pnlError.Visible = false;
                                        }
                                    }
                                }
                            }

                            // Si no fue agregado, agrega el item 
                            if (!existingRule)
                            {
                                this.txtCode.Text = rule.Code.Trim();
                                this.txtDescription.Text = rule.Name.Trim();
                            }
                            else
                            {
                                pnlError.Visible = true;
                            }
                        }
                    }
                }


                if (!validRule)
                {
                    ucFilterItem.Clear();
                    ucFilterItem.Initialize();

                    // Setea el filtro con el Item ingresado
                    if (txtCode.Text.Trim() != string.Empty)
                    {
                        FilterItem filterItem = new FilterItem("%" + txtCode.Text + "%");
                        filterItem.Selected = true;
                        ucFilterItem.FilterItems[0] = filterItem;
                        ucFilterItem.LoadCurrentFilter(ucFilterItem.FilterItems);
                        SearchRules();
                    }
                    // Si no se ingresó ningún item, no se ejecuta la búsqueda
                    else
                        ClearGridItem();

                    // Esto evita un bug de ajax

                    valAddRule.Enabled = false;
                    valSearchRule.Enabled = false;

                    this.lblAddRule.Text = this.lblNewDetail.Text;
                    this.ucFilterItem.Visible = true;//Esconde los controles de busqueda de Item incluidos los Kit
                    this.mpLookupRule.Show();
                }
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
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
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        protected void grdSearchRules_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        private void ClearGridItem()
        {
            Session.Remove(WMSTekSessions.Shared.RulesMgr.RulesList);
            grdSearchRules.DataSource = null;
            grdSearchRules.DataBind();
        }

        private void PopulateGrid()
        {
            grdMgr.DataSource = null;
            grdMgr.PageIndex = currentPage;
           
            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!customRuleViewDTO.hasConfigurationError() && customRuleViewDTO.Configuration != null && customRuleViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, customRuleViewDTO.Configuration);

            grdMgr.DataSource = customRuleViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(customRuleViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
                customRuleViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            if (this.Master.ucMainFilter.idWmsProcessType == "-1")
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.WmsProcessType)].FilterValues.Clear();
                foreach (ListItem item in this.Master.ucMainFilter.listItemWmsProcessType)
                {
                    context.MainFilter[Convert.ToInt16(EntityFilterName.WmsProcessType)].FilterValues.Add(new FilterItem(item.Text, item.Value));
                }
            }
            else
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.WmsProcessType)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.WmsProcessType)].FilterValues.Add(new FilterItem("", this.Master.ucMainFilter.idWmsProcessType.ToString()));
            }

            customRuleViewDTO = iRulesMGR.FindAllCustomRule(context);

            if (!customRuleViewDTO.hasError() && customRuleViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Shared.RulesMgr.CustomRuleList, customRuleViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(customRuleViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        protected void ReloadData()
        {
            crud = false;            
            this.lblGridDetail.Text = string.Empty;
            this.txtName.Text = string.Empty;
            this.txtCode.Text = string.Empty;
            this.txtDescription.Text = string.Empty;
            this.pnlError.Visible = false;

            currentIndex = -1;
            //this.divDetail.Visible = false;
            this.divGroupRuleDetail.Visible = false;

            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                grdMgr.SelectedIndex = -1;
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
            // Configura ventana modal
            if (customRuleViewDTO.Configuration != null && customRuleViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(customRuleViewDTO.Configuration, true);
                else
                    base.ConfigureModal(customRuleViewDTO.Configuration, false);
            }

            // Editar entidad
            if (mode == CRUD.Update)
            {
                //Recupera la fila a editar
                this.hidEditId.Value = customRuleViewDTO.Entities[index].Id.ToString();

                //Carga controles
                this.txtName.Text = customRuleViewDTO.Entities[index].Name;
                this.ddlProcess.SelectedValue = customRuleViewDTO.Entities[index].WmsProcess.Code;
                this.ddlWarehouse.SelectedValue = customRuleViewDTO.Entities[index].Warehouse.Id.ToString();

                if (this.customRuleViewDTO.Entities[index].Status == 0)
                {
                    this.chkCodStatus.Checked = false;
                }
                else
                {
                    this.chkCodStatus.Checked = true;
                }

                if (this.customRuleViewDTO.Entities[index].DefaultRule == 0)
                {
                    this.chkDefaultRule.Checked = false;
                }
                else
                {
                    this.chkDefaultRule.Checked = true;
                }

                this.lblNew.Visible = false;
                this.lblEdit.Visible = true;
                this.lblView.Visible = false;
            }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                this.hidEditId.Value = "0";

                this.chkCodStatus.Checked = true;
                this.chkDefaultRule.Checked = false;
                this.txtName.Text = string.Empty;
                this.ddlProcess.SelectedIndex = 0;

                this.lblNew.Visible = true;
                this.lblEdit.Visible = false;
                this.lblView.Visible = false;
            }

            // Ver entidad
            if (mode == CRUD.Read)
            {
                ////Recupera la fila a editar
                this.hidEditId.Value = customRuleViewDTO.Entities[index].Id.ToString();

                ////el dato IsBaseRole es de solo lectura
                //hidIsBaseRole.Value = roleViewDTO.Entities[index].IsBaseRole.ToString();

                ////Carga controles
                this.chkCodStatus.Checked = customRuleViewDTO.Entities[index].Status == 1 ? true : false;
                this.chkDefaultRule.Checked = customRuleViewDTO.Entities[index].DefaultRule == 1 ? true : false;
                this.txtName.Text = customRuleViewDTO.Entities[index].Name.Trim();
                this.ddlProcess.SelectedValue = customRuleViewDTO.Entities[index].WmsProcess.Code;

                //Deshabilita todos los controles
                // TODO: hacerlo en base.ConfigureModal
                this.chkCodStatus.Enabled = false;
                this.txtName.Enabled = false;

                this.lblNew.Visible = false;
                this.lblEdit.Visible = false;
                this.lblView.Visible = true;
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }


        protected void SaveChanges()
        {
            //Agrega los datos del Grupo de Reglas
            CustomRule custom = new CustomRule();
            custom.Id = Convert.ToInt32(hidEditId.Value);
            custom.Name = this.txtName.Text.Trim();
            custom.Status = (this.chkCodStatus.Checked ? 1:0);
            custom.DefaultRule = chkDefaultRule.Checked == true ? 1 : 0;
            custom.WmsProcess = new WmsProcess();
            custom.WmsProcess.Code = this.ddlProcess.SelectedValue;
            custom.Warehouse = new Warehouse();
            custom.Warehouse.Id = int.Parse(this.ddlWarehouse.SelectedValue);

            if (hidEditId.Value == "0")
                customRuleViewDTO = iRulesMGR.MaintainCustomRule(CRUD.Create, custom, context);
            else
                customRuleViewDTO = iRulesMGR.MaintainCustomRule(CRUD.Update, custom, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (customRuleViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(customRuleViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }


        private void DeleteRow(int index)
        {
            customRuleViewDTO = iRulesMGR.MaintainCustomRule(CRUD.Delete, customRuleViewDTO.Entities[index], context);

            if (customRuleViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Mustra mensaje de barra de status
                crud = true;
                ucStatus.ShowMessage(customRuleViewDTO.MessageStatus.Message);
                //Actuualiza grilla
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la regla seleccionada
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRowRule(GenericViewDTO<GroupRule> rules, bool lastRow)
        {
            if (lastRow) 
            {
                //Elimina la ultima regla del grupo
                groupRuleViewDTO = iRulesMGR.DeleteGroupRule(rules.Entities[0], context);
                groupRuleViewDTO.Entities.Clear();
                Session.Add(WMSTekSessions.Shared.RulesMgr.GroupRulesList, groupRuleViewDTO);
            }
            else
            {
                //Elimina todas las lineas que tengan el id del grupo seleccionado
                groupRuleViewDTO = iRulesMGR.MaintainGroupRule(CRUD.Delete, rules, context);
            }
            
            //Actualiza la session
            if (groupRuleViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra mensaje de status
                crud = true;
                ucStatus.ShowMessage(groupRuleViewDTO.MessageStatus.Message);

                //Actualiza la session
                UpdateSession(false);
            }
        }

        private void SearchRules()
        {
            string wmsProcess = string.Empty;
            ContextViewDTO newContext = new ContextViewDTO();
            newContext.MainFilter = context.MainFilter;
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Clear();
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.WmsProcessType)].FilterValues.Clear();
            //Rescata los valores de los filtros de Busqueda de las reglas
            //Codigo
            if (ucFilterItem.FilterItems[0].Value != "%%")
            {
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Add(new FilterItem(0, ucFilterItem.FilterItems[0].Value.Replace("%", "").Trim()));
            }
            else
            {
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();
            }

            //Name
            if (ucFilterItem.FilterItems[1].Value != "%%")
            {
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Clear();
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Add(new FilterItem(0, ucFilterItem.FilterItems[1].Value.Replace("%", "").Trim()));
            }
            else
            {
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Clear();
            }
            
            ////Rescata el index seleccinado en la grilla del grupo
            //if (currentIndex != 0)
            //{
            //    wmsProcess = customRuleViewDTO.Entities[currentIndex].WmsProcess.Code.Trim();

            //    newContext.MainFilter[Convert.ToInt16(EntityFilterName.WmsProcessType)].FilterValues.Clear();
            //    newContext.MainFilter[Convert.ToInt16(EntityFilterName.WmsProcessType)].FilterValues.Add(new FilterItem(0, wmsProcess));
            //}

            //Realiza la busqueda de las reglas por los filtro ingresados
            ruleViewDTO = iRulesMGR.GetRulesCodeNameWmsProcess(newContext);

            Session.Remove(WMSTekSessions.Shared.RulesMgr.RulesList);
            Session.Add(WMSTekSessions.Shared.RulesMgr.RulesList, ruleViewDTO);

            if (ruleViewDTO.hasError())
                isValidViewDTO = false;
            else
                isValidViewDTO = true;

            this.grdSearchRules.DataSource = ruleViewDTO.Entities;
            this.grdSearchRules.DataBind();
        }


        protected void LoadDetailCustomGroup()
        {
            bool isValidIndex = false;

            if (currentIndex != -1)
            {
                if (ValidateSession(WMSTekSessions.Shared.RulesMgr.CustomRuleList))
                {
                    customRuleViewDTO = (GenericViewDTO<CustomRule>)Session[WMSTekSessions.Shared.RulesMgr.CustomRuleList];
                    isValidViewDTO = true;
                }
                if (customRuleViewDTO.Entities.Count >= currentIndex)
                {
                    int idCustomRule = customRuleViewDTO.Entities[currentIndex].Id;

                    if (ValidateSession(WMSTekSessions.Shared.RulesMgr.GroupRulesList))
                    {
                        groupRuleViewDTO = (GenericViewDTO<GroupRule>)Session[WMSTekSessions.Shared.RulesMgr.GroupRulesList];
                    }
                    else
                    {
                        groupRuleViewDTO = iRulesMGR.GetByIdCustomRule(idCustomRule, context);
                        Session.Add(WMSTekSessions.Shared.RulesMgr.GroupRulesList, groupRuleViewDTO);
                    }
                }
                else
                {
                    currentIndex = 0;
                }
                isValidIndex = true;
            }

            if (isValidIndex)
            {
                if (groupRuleViewDTO.Entities != null)
                {
                    //Agrega la lista en session
                    Session.Add(WMSTekSessions.Shared.RulesMgr.GroupRulesList, groupRuleViewDTO);

                    //Habilita el panel del detalle de items
                    this.divGroupRuleDetail.Visible = true;

                    // Configura ORDEN de las columnas de la grilla
                    if (!groupRuleViewDTO.hasConfigurationError() && groupRuleViewDTO.Configuration != null && groupRuleViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, groupRuleViewDTO.Configuration);
                    
                    grdDetail.DataSource = groupRuleViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();

                    //Se activa funcion para el ordenamiento por Drag&Drop
                    ScriptManager.RegisterStartupScript(Page, GetType(), "Success", "ReOrderGridView();", true);

                    //// Configura VISIBILIDAD de las columnas de la grilla
                    //if (!groupRuleViewDTO.hasConfigurationError() && groupRuleViewDTO.Configuration != null && groupRuleViewDTO.Configuration.Count > 0)
                    //    base.ConfigureGridVisible(grdDetail, groupRuleViewDTO.Configuration);
                }
                else
                {
                    this.divGroupRuleDetail.Visible = false;
                }
            }
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('CustomRule_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDropCustom();", true);
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
                ruleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ruleViewDTO.Errors);
            }
        }

        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            GridView grdMgrAux = new GridView();
            GenericViewDTO<CustomRule> customRuleAuxViewDTO = new GenericViewDTO<CustomRule>();
            GenericViewDTO<GroupRule> groupRuleAuxViewDTO = new GenericViewDTO<GroupRule>();
            //CustomRule theCustomRule = new CustomRule();
            string detailTitle;

            try
            {
                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    grdMgr.AllowPaging = false;
                    PopulateGrid();

                    //grdMgrAux = grdMgr;

                    int idCustomRule = customRuleViewDTO.Entities[currentIndex].Id;
                    groupRuleAuxViewDTO = iRulesMGR.GetByIdCustomRule(idCustomRule, context); //iReceptionMGR.GetInboundOrderByAnyParameter(theInbound, context);
                    grdMgrAux.DataSource = groupRuleAuxViewDTO.Entities;
                    grdMgrAux.DataBind();

                    LoadDetailCustomGroup();
                    detailTitle = lblGridDetail.Text + string.Empty;

                    base.ExportToExcel(grdMgrAux, grdDetail, detailTitle, true);
                    grdMgr.AllowPaging = true;
                }


            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {

                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        protected void grdDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                var argument = e.CommandArgument.ToString();

                if (!string.IsNullOrEmpty(argument))
                {
                    int index = Convert.ToInt32(e.CommandArgument);

                    if (e.CommandName == "Up") ChangeOrderPriority(index, "up");
                    if (e.CommandName == "Down") ChangeOrderPriority(index, "down");
                }
            }
            catch (Exception ex)
            {
                customRuleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customRuleViewDTO.Errors);
            }
        }

        protected void ChangeOrderPriority(int index, string action)
        {
            var selectedRule = new GroupRule();

            // Subir prioridad
            if (index > 0 && action == "up")
            {
                selectedRule = groupRuleViewDTO.Entities[index];
                groupRuleViewDTO.Entities.RemoveAt(index);
                groupRuleViewDTO.Entities.Insert(index - 1, selectedRule);
            }

            // Bajar prioridad
            if (index < groupRuleViewDTO.Entities.Count - 1 && action == "down")
            {
                selectedRule = groupRuleViewDTO.Entities[index];
                groupRuleViewDTO.Entities.RemoveAt(index);
                groupRuleViewDTO.Entities.Insert(index + 1, selectedRule);
            }

            UpdateSelectedRowsSession();
        }
        private void UpdateSelectedRowsSession()
        {
            Session[WMSTekSessions.Shared.RulesMgr.GroupRulesList] = groupRuleViewDTO;
        }
    }
}
