using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Label;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Inbound.Consult
{
    public partial class LabelConsultOC : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<InboundLabel> inboundLabelViewDTO = new GenericViewDTO<InboundLabel>();
        private GenericViewDTO<InboundLabelDetail> inboundLabelDetailViewDTO = new GenericViewDTO<InboundLabelDetail>();
        private GenericViewDTO<LabelTemplate> labelTemplateViewDTO;
        private GenericViewDTO<TaskLabel> taskLabelViewDTO;
        private bool isValidViewDTO = false;
        private bool isValidViewDetailDTO = false;

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

        // Propiedad para controlar el indice activo en la grilla
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

        #region "Eventos"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                //if (base.webMode == WebMode.Normal) Initialize();

                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        LoadControls();
                    }
                    else
                    {

                        if (ValidateSession(WMSTekSessions.Shared.InboundLabel))
                        {
                            inboundLabelViewDTO = (GenericViewDTO<InboundLabel>)Session[WMSTekSessions.Shared.InboundLabel];
                            isValidViewDTO = true;
                        }

                        // Si es un ViewDTO valido, carga la grilla y las listas
                        if (isValidViewDTO)
                        {
                            // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                            PopulateGrid();
                        }

                        if (ValidateSession(WMSTekSessions.Shared.InboundLabelDetail))
                        {
                            inboundLabelDetailViewDTO = (GenericViewDTO<InboundLabelDetail>)Session[WMSTekSessions.Shared.InboundLabelDetail];
                            isValidViewDetailDTO = true;
                        }

                        // Si es un ViewDTO valido, carga la grilla y las listas
                        if (isValidViewDetailDTO)
                        {
                            // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                            PopulateGridDetail();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
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
                        //PopulateGrid();
                    }
                }


                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivCharts();", true);
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    
                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                    e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    
                }
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //capturo la posicion de la fila 
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndex = grdMgr.SelectedIndex;

                    LoadInboundOrderDetail(index);
                }
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected void ddlPrinters_Change(object sender, EventArgs e)
        {
            try
            {
                ReloadLabelSize();
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected void ReloadLabelSize()
        {
            if (!String.IsNullOrEmpty(ddlPrinters.SelectedValue.Trim()))
                base.LoadLabelSize(this.ddlLabelSize, Convert.ToInt32(ddlPrinters.SelectedValue.ToString()), "ITEM");
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            taskLabelViewDTO = new GenericViewDTO<TaskLabel>();
            List<TaskLabel> taskLabels = new List<TaskLabel>();
            List<TaskLabelCant> taskLabelCants = new List<TaskLabelCant>();

            if (Convert.ToInt32(ddlLabelSize.SelectedValue) <= 0)
            {
                ucStatus.ShowError(lblValidateLabelType.Text);
                return;
            }

            inboundLabelDetailViewDTO = (GenericViewDTO<InboundLabelDetail>)Session[WMSTekSessions.Shared.InboundLabelDetail];

            foreach (InboundLabelDetail inboundLabelDetail in inboundLabelDetailViewDTO.Entities)
            {
                if (inboundLabelDetail.Selected)
                {
                    #region Variables
                    String p_LongItemName1 = String.Empty;
                    String p_LongItemName2 = String.Empty;
                    String p_ExpirationDate = string.Empty;
                    String p_FabricationDate = string.Empty;
                    String p_ExpirationDateFormat = String.Empty;
                    String p_FabricationDateFormat = String.Empty;
                    String p_Lote = String.Empty;
                    #endregion Variables

                    #region Substring LongItenName
                    if (!String.IsNullOrEmpty(inboundLabelDetail.LongItemName))
                    {
                        if (inboundLabelDetail.LongItemName.Length >= 20)
                        {
                            if (inboundLabelDetail.LongItemName.Length > 20)
                            {
                                p_LongItemName1 = inboundLabelDetail.LongItemName.Substring(0, 20);

                                if (inboundLabelDetail.LongItemName.Length > 40)
                                    p_LongItemName2 = inboundLabelDetail.LongItemName.Substring(20, 20);
                                else
                                    p_LongItemName2 = inboundLabelDetail.LongItemName.Substring(20, inboundLabelDetail.LongItemName.Length - 20);
                            }
                            else
                                p_LongItemName1 = inboundLabelDetail.LongItemName.Substring(0, 19);
                        }
                        else
                            p_LongItemName1 = inboundLabelDetail.LongItemName;
                    }
                    #endregion Substring LongItenName

                    #region Maneja Lote
                    if (inboundLabelDetail.UseLot)
                    {
                        p_ExpirationDate = inboundLabelDetail.ExpirationDatePrint.ToShortDateString();
                        p_ExpirationDateFormat = inboundLabelDetail.ExpirationDatePrint.ToString("ddMMyy");
                        p_FabricationDate = inboundLabelDetail.FabricationDatePrint.ToShortDateString();
                        p_FabricationDateFormat = inboundLabelDetail.FabricationDatePrint.ToString("ddMMyy");
                        p_Lote = inboundLabelDetail.LotNumber;
                    }
                    #endregion Maneja Lote

                    Int32 p_idItem = inboundLabelDetail.IdItem;
                    Int32 p_idUom = inboundLabelDetail.IdUom;
                    string xmlParam = iLabelMGR.GetForItemLot(p_LongItemName1, p_LongItemName2, p_FabricationDate, p_FabricationDateFormat, p_ExpirationDate, p_ExpirationDateFormat, p_Lote, p_idItem, p_idUom);

                    TaskLabel taskLabel = new TaskLabel();
                    TaskLabelCant taskLabelCant = new TaskLabelCant();

                    int Valor = Convert.ToInt32(ddlLabelSize.SelectedValue);

                    taskLabelCant.LabelTemplate = new LabelTemplate();
                    taskLabelCant.LabelTemplate.Id = Valor;

                    taskLabelCant.Printer = new Printer(Convert.ToInt32(ddlPrinters.SelectedValue.ToString()));
                    taskLabelCant.User = new Binaria.WMSTek.Framework.Entities.Profile.User(context.SessionInfo.User.Id);

                    taskLabelCant.DelayPrinted = 0;
                    taskLabelCant.IsPrinted = false;
                    taskLabelCant.ParamString = xmlParam;
                    taskLabelCant.Cantidad = inboundLabelDetail.CopNumber;
                    taskLabelCants.Add(taskLabelCant);

                    taskLabel.LabelTemplate = new LabelTemplate();
                    taskLabel.LabelTemplate.Id = Valor;

                    taskLabel.Printer = new Printer(Convert.ToInt32(ddlPrinters.SelectedValue.ToString()));
                    taskLabel.User = new Binaria.WMSTek.Framework.Entities.Profile.User(context.SessionInfo.User.Id);

                    taskLabel.DelayPrinted = 0;
                    taskLabel.IsPrinted = false;
                    taskLabel.ParamString = xmlParam;

                    taskLabels.Add(taskLabel);
                }
            }

            taskLabelViewDTO.Entities = taskLabels;

            if (taskLabelCants.Count > 0)
            {
                // Crea el registro a imprimir en TaskLabel
                taskLabelViewDTO = iLabelMGR.MaintainTaskLabelCant(CRUD.Create, taskLabelCants, context);

                if (!taskLabelViewDTO.hasError())
                {
                    crud = true;
                    ucStatus.ShowMessage(taskLabelViewDTO.MessageStatus.Message);
                    //txtQtycopies.Text = string.Empty;
                }
                else
                {
                    this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
                }

                LoadControls();
            }
            else
            {
                // Si no se seleccionó ningun elemento, avisa 
                //this.Master.ucDialog.ShowAlert(lblTitle.Text, lblNoRowsSelected.Text, string.Empty);
            }

        }

        // Carga la grilla, filtrada por el criterio de busqueda ingresado

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        /// <summary>
        /// Actualiza la vista actual, cargando nuevamente las variables de sesion desde base de datos. 
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
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    LoadInboundOrderDetail(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                }
                else
                {
                    detailTitle = null;
                }

                base.ExportToExcel(grdMgr, grdDetail, detailTitle);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "LabelConsultOC";

            InitializeFilter(!Page.IsPostBack, false);
            InitializeTaskBar();
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .35);
            }
            else
            {
                if (ValidateSession(WMSTekSessions.Shared.InboundLabel))
                {
                    inboundLabelViewDTO = (GenericViewDTO<InboundLabel>)Session[WMSTekSessions.Shared.InboundLabel];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Retorna el detalle de cada doc de entrada
        /// </summary>
        /// <param name="index"></param>
        protected void LoadInboundOrderDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                int id = inboundLabelViewDTO.Entities[index].IdInboundOrder;

                inboundLabelDetailViewDTO = iWarehousingMGR.FindAllInboundLabelDetail(id, context);
                this.lblNroDoc.Text = inboundLabelViewDTO.Entities[index].InboundNumber;

                if (inboundLabelDetailViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!inboundLabelDetailViewDTO.hasConfigurationError() && inboundLabelDetailViewDTO.Configuration != null && inboundLabelDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, inboundLabelDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = inboundLabelDetailViewDTO.Entities;
                    grdDetail.DataBind();
                    Session.Add(WMSTekSessions.Shared.InboundLabelDetail, inboundLabelDetailViewDTO);
                    isValidViewDetailDTO = true;

                    CallJsGridViewDetail();

                    if (inboundLabelDetailViewDTO.Entities.Count > 0)
                    {
                        this.Master.ucTaskBar.btnPrintVisible = true;
                        LoadControls();
                        divPrintLabel.Visible = true;
                    }
                    else
                        this.Master.ucTaskBar.btnPrintVisible = false;

                }

                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary> 
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.referenceDocTypeVisible = true;
            this.Master.ucMainFilter.itemVisible = true;

            this.Master.ucMainFilter.inboundTypeVisible = true;


            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            //this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnPrintClick += new EventHandler(btnPrint_Click);

            //this.Master.ucTaskBar.btnPrintVisible = true;
            this.Master.ucTaskBar.btnRefreshVisible = true;
            //this.Master.ucTaskBar.btnExcelVisible = true;
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Estado
        /// </summary>
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
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

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Carga lista de Inbound
            inboundLabelViewDTO = iWarehousingMGR.FindAllInboundLabel(context);

            if (!inboundLabelViewDTO.hasError() && inboundLabelViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Shared.InboundLabel, inboundLabelViewDTO);
                isValidViewDTO = true;
                inboundLabelDetailViewDTO = new GenericViewDTO<InboundLabelDetail>();
                this.Master.ucTaskBar.btnPrintVisible = false;

                ucStatus.ShowMessage(inboundLabelViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN y Visibilidad de las columnas de la grilla
            if (!inboundLabelViewDTO.hasConfigurationError() && inboundLabelViewDTO.Configuration != null && inboundLabelViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, inboundLabelViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = inboundLabelViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(inboundLabelViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
            }

            if (isValidViewDetailDTO)
            {
                currentPage = 0;
                this.Master.ucError.ClearError();

                if (inboundLabelDetailViewDTO.Entities.Count > 0)
                {
                    divPrintLabel.Visible = true;
                    Master.ucTaskBar.btnPrintEnabled = true;
                }
                else
                {
                    divPrintLabel.Visible = false;
                    Master.ucTaskBar.btnPrintEnabled = false;
                }
            }

        }

        protected void grdDetail_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    inboundLabelDetailViewDTO =(GenericViewDTO<InboundLabelDetail>) Session[WMSTekSessions.Shared.InboundLabelDetail];
                    //capturo la posicion de la fila 
                    int index = grdDetail.PageSize * grdDetail.PageIndex + grdDetail.SelectedIndex;
                    currentIndex = grdDetail.SelectedIndex;
                    
                    inboundLabelDetailViewDTO.Entities[index].Selected = !inboundLabelDetailViewDTO.Entities[index].Selected;

                    if (inboundLabelDetailViewDTO.Entities[index].Selected)
                    {
                        GenericViewDTO<Item> item = iWarehousingMGR.GetItemLotUse(inboundLabelDetailViewDTO.Entities[currentIndex].IdItem, inboundLabelDetailViewDTO.Entities[currentIndex].IdOwn, context);
                        GenericViewDTO<ItemUom> itemUom = iWarehousingMGR.GetItemUomByIdItem(context, inboundLabelDetailViewDTO.Entities[currentIndex].IdItem);

                        if (item.Entities.Count > 0)
                        {
                            inboundLabelDetailViewDTO.Entities[currentIndex].UseLot = true;
                        }
                        if (itemUom.Entities.Count > 0)
                        {
                            inboundLabelDetailViewDTO.Entities[currentIndex].IdUom = itemUom.Entities[0].Id;
                        }
                    }
                    else
                    {
                        inboundLabelDetailViewDTO.Entities[currentIndex].Selected = false;
                        inboundLabelDetailViewDTO.Entities[currentIndex].UseLot = false;
                        inboundLabelDetailViewDTO.Entities[currentIndex].CopNumber = (Int32) inboundLabelDetailViewDTO.Entities[currentIndex].ItemQty;
                        inboundLabelDetailViewDTO.Entities[currentIndex].LotNumber = String.Empty;
                        inboundLabelDetailViewDTO.Entities[currentIndex].ExpirationDatePrint = DateTime.MinValue;
                        inboundLabelDetailViewDTO.Entities[currentIndex].FabricationDatePrint = DateTime.MinValue;
                    }

                    PopulateGridDetail();
                    EvalVisiblePrint();
                    divDetail.Visible = true;
                    Session.Add(WMSTekSessions.Shared.InboundLabelDetail, inboundLabelDetailViewDTO);
                }
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        public String DisplayDate(object objImageSm)
        {
            if (DateTime.Parse(objImageSm.ToString()).Equals(DateTime.MinValue))
                return "";
            else
                return DateTime.Parse(objImageSm.ToString()).ToShortDateString();
        }

        private void PopulateGridDetail()
        {
            //inboundLabelDetailViewDTO = (GenericViewDTO<InboundLabelDetail>)Session[WMSTekSessions.Shared.InboundLabelDetail];
            grdDetail.PageIndex = currentPage;
            grdDetail.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!inboundLabelDetailViewDTO.hasConfigurationError() && inboundLabelDetailViewDTO.Configuration != null && inboundLabelDetailViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdDetail, inboundLabelDetailViewDTO.Configuration);

            // Encabezado de Recepciones
            grdDetail.DataSource = inboundLabelDetailViewDTO.Entities;
            grdDetail.DataBind();
            upGridDetail.Update();

            //ucStatus.ShowRecordInfo(inboundLabelDetailViewDTO.Entities.Count, inboundLabelDetailViewDTO.PageSize, inboundLabelDetailViewDTO.PageCount, currentPage, inboundLabelDetailViewDTO.AllowPaging);

            CallJsGridViewDetail();
        }

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");

                    e.Row.Cells[0].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdDetail, "Select$" + e.Row.RowIndex);
                    e.Row.Cells[e.Row.Cells.Count - 1].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdDetail, "Edit$" + e.Row.RowIndex);
                 
                    // Asinga el atributo 'onclick' a todas las columnas de la grilla, excepto a la que contiene los checkboxes
                    // IMPORTANTE: no cambiar de lugar la columna [0] que contiene los checkboxes
                }
            }
            catch (Exception ex)
            {
                inboundLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundLabelViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing1(object sender, GridViewEditEventArgs e)
        {
            try
            {
                //PopulateGridDetail();
                inboundLabelDetailViewDTO = (GenericViewDTO<InboundLabelDetail>)Session[WMSTekSessions.Shared.InboundLabelDetail];

                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdDetail.PageSize * grdDetail.PageIndex + e.NewEditIndex;
                
                TextBox txtCopiasGrid = (TextBox)grdDetail.Rows[editIndex].FindControl("txtCopias");

                if (String.IsNullOrEmpty(txtCopiasGrid.Text))
                {
                    Master.ucDialog.ShowAlert("Parametro Requerido", "Debe ingresar el número de Copias", "");
                    return;
                }

                inboundLabelDetailViewDTO.Entities[editIndex].CopNumber = int.Parse(txtCopiasGrid.Text);


                if (inboundLabelDetailViewDTO.Entities[editIndex].UseLot)
                {
                    TextBox txtLotNumberGrid = (TextBox)grdDetail.Rows[editIndex].FindControl("txtLote");

                    //if (!String.IsNullOrEmpty(txtLotNumberGrid.Text))
                        inboundLabelDetailViewDTO.Entities[editIndex].LotNumber = txtLotNumberGrid.Text;
                    //else
                    //{
                    //    Master.ucDialog.ShowAlert("Parametro Requerido", "Debe ingresar el número de Lote", "");
                    //    return;
                    //}

                    TextBox txtVencimientoGrid = (TextBox)grdDetail.Rows[editIndex].FindControl("txtVencimiento");

                    if (!String.IsNullOrEmpty(txtVencimientoGrid.Text))
                    {
                        inboundLabelDetailViewDTO.Entities[editIndex].ExpirationDatePrint = DateTime.Parse(txtVencimientoGrid.Text);
                        inboundLabelDetailViewDTO.Entities[editIndex].ExpirationDate = DateTime.Parse(txtVencimientoGrid.Text);
                    }
                    //else
                    //{
                    //    Master.ucDialog.ShowAlert("Parametro Requerido", "Debe ingresar la fecha de vencimiento", "");
                    //    return;
                    //}

                    TextBox txtFabricacionGrid = (TextBox)grdDetail.Rows[editIndex].FindControl("txtFabricacion");

                    if (!String.IsNullOrEmpty(txtFabricacionGrid.Text))
                    {
                        inboundLabelDetailViewDTO.Entities[editIndex].FabricationDatePrint = DateTime.Parse(txtFabricacionGrid.Text);
                        inboundLabelDetailViewDTO.Entities[editIndex].FabricationDate = DateTime.Parse(txtFabricacionGrid.Text);
                    }
                    //else
                    //{
                    //    Master.ucDialog.ShowAlert("Parametro Requerido", "Debe ingresar la fecha de vencimiento", "");
                    //    return;
                    //}
                }

                PopulateGridDetail();
                EvalVisiblePrint();
                divDetail.Visible = true;
                Session.Add(WMSTekSessions.Shared.InboundLabelDetail, inboundLabelDetailViewDTO);

            }
            catch (Exception ex)
            {
                // NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }

        private void EvalVisiblePrint()
        {
            int a = 0;
            foreach (InboundLabelDetail detail in inboundLabelDetailViewDTO.Entities)
            {
                if (detail.Selected)
                {
                    //if (detail.UseLot)
                    //{
                    //    if (!String.IsNullOrEmpty(detail.LotNumber) && detail.ExpirationDatePrint > DateTime.MinValue)
                    //    {
                    //        a++;
                    //    }
                    //}
                    //else
                    //{
                        a++;
                    //}
                }
            }
            if (a > 0)
                Master.ucTaskBar.btnPrintEnabled = true;
            else
                Master.ucTaskBar.btnPrintEnabled = false;
        }

        protected void LoadControls()
        {
            lblNotPrinter.Visible = false;
            // txtQtycopies.Text = string.Empty;
            
            // Carga lista de impresoras asociadas al usuario
            base.LoadUserPrinters(this.ddlPrinters);

            // Selecciona impresora por defecto
            base.SelectDefaultPrinter(this.ddlPrinters);

            if (ddlPrinters.Items.Count == 0)
            {
                lblNotPrinter.Visible = true;
                //txtQtycopies.Enabled = false;
                ddlPrinters.Enabled = false;
            }
            else
            {
                ReloadLabelSize();
            }
            Master.ucTaskBar.btnPrintEnabled = false;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('InBoundLabelDetail_GetAll', 'ctl00_MainContent_hsMasterDetail_bottomPanel_ctl01_grdDetail');", true);
        }

        #endregion
    }
}

