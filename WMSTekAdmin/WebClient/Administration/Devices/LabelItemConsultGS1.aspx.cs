using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Label;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using System.Xml;
using System.Configuration;
using Binaria.WMSTek.Framework.Entities.Profile;
using System.Xml.Linq;

namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class LabelItemConsultGS1 : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Framework.Entities.Warehousing.Stock> stockViewDTO = new GenericViewDTO<Framework.Entities.Warehousing.Stock>();
        private BaseViewDTO configurationViewDTO = new BaseViewDTO();
        private GenericViewDTO<TaskLabel> taskLabelViewDTO;
        //private GenericViewDTO<LabelTemplate> labelTemplateViewDTO;
        private bool isValidViewDTO = false;
            
        // Propiedad para controlar el nro de pagina activa en la grilla
        private int currentPage
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

                    if (Page.IsPostBack)
                    {
                        if (ValidateSession(WMSTekSessions.Label.ItemGS1))
                        {
                            stockViewDTO = (GenericViewDTO<Framework.Entities.Warehousing.Stock>)Session[WMSTekSessions.Label.ItemGS1];
                            isValidViewDTO = true;
                        }

                        // Si es un ViewDTO valido, carga la grilla y las listas
                        if (isValidViewDTO)
                        {
                            // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                            PopulateGrid();
                        }
                    }

                    if (!Page.IsPostBack)
                    {
                        LoadControls();
                    }

                }
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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

                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado 
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                List<EntityFilter> lista = new List<EntityFilter>();
                foreach (EntityFilter item in this.Master.ucMainFilter.MainFilter)
                {
                    if (item.Selected && (item.Name != "Warehouse" && item.Name != "Owner" && item.Name != "UsedPercenter"))
                    {
                        lista.Add(item);
                    }
                }
                if (lista != null && lista.Count == 0)
                {
                    grdMgr.DataSource = null;
                    stockViewDTO.Entities = null;
                    Session[WMSTekSessions.Label.ItemGS1] = null;
                    isValidViewDTO = false;
                    ucStatus.ShowWarning(LabelmessageFind.Text);
                }
                else
                {
                    ReloadData();
                }
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        /// <summary>
        /// Inserta TaskLabel
        /// </summary>
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                bool hasError = false;

                int qtyCopies = 1;

                if (txtQtycopies.Text.Trim() != string.Empty)
                {
                    qtyCopies = Convert.ToInt32(txtQtycopies.Text.Trim());
                }

                taskLabelViewDTO = new GenericViewDTO<TaskLabel>();
                List<TaskLabel> taskLabels = new List<TaskLabel>();

                if (Convert.ToInt32(ddlLabelSize.SelectedValue) > 0)
                {
                    foreach (GridViewRow row in this.grdMgr.Rows)
                    {
                        CheckBox chkbox = (CheckBox)row.FindControl("chkSeleccion");

                        if (chkbox.Checked)
                        {
                            int index = grdMgr.PageSize * grdMgr.PageIndex + row.RowIndex;
                            int stockId = stockViewDTO.Entities[index].Id;
                            int idLabel = int.Parse(ddlLabelSize.SelectedItem.Value.Trim());
                            string itemCode = stockViewDTO.Entities[index].Item.Code;

                            string xmlParam = iLabelMGR.GetForItemGS1(stockId, idLabel, context);

                            System.Xml.Linq.XDocument xdoc = XDocument.Parse(xmlParam.Trim());
                            XElement param = xdoc.Root.Element("label_params");
                            param.Add(
                                    new XElement("label_param",
                                    new XElement("name", "PrintQty"),
                                    new XElement("value", Convert.ToInt32(qtyCopies)))
                                    );


                            //Valida si se genero el string para insertar el la tarea de impresion
                            if (xmlParam == null)
                            {
                                this.Master.ucDialog.ShowAlert(lblTitle.Text, lblErrorItem.Text + itemCode.Trim(), string.Empty);
                                hasError = true;
                                break;
                            }

                            TaskLabel taskLabel = new TaskLabel();
                            taskLabel.LabelTemplate = new LabelTemplate();

                            taskLabel.LabelTemplate.Id = Convert.ToInt32(ddlLabelSize.SelectedValue);
                            taskLabel.Printer = new Printer(Convert.ToInt32(ddlPrinters.SelectedValue.ToString()));
                            taskLabel.User = new User(context.SessionInfo.User.Id);
                            taskLabel.DelayPrinted = 0;
                            taskLabel.IsPrinted = false;
                            taskLabel.ParamString = xdoc.ToString().Replace("\r\n", "").Replace(" ", ""); 

                            taskLabels.Add(taskLabel);
                        }
                    }
                }

                if (!hasError)
                {
                    taskLabelViewDTO.Entities = taskLabels;

                    if (taskLabelViewDTO.Entities.Count > 0)
                    {
                        // Crea el registro a imprimir en TaskLabel
                        taskLabelViewDTO = iLabelMGR.MaintainTaskLabel(CRUD.Create, taskLabelViewDTO.Entities, context, 1);

                        if (!taskLabelViewDTO.hasError())
                        {
                            crud = true;
                            ucStatus.ShowMessage(taskLabelViewDTO.MessageStatus.Message);
                            //txtQtycopies.Text = "";
                        }
                        else
                        {
                            this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
                        }

                        //LoadControls();
                    }
                    else
                    {
                        // Si no se seleccionó ningun elemento, avisa 
                        this.Master.ucDialog.ShowAlert(lblTitle.Text, lblNoRowsSelected.Text, string.Empty);
                    }
                }

            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in this.grdMgr.Rows)
            {
                CheckBox chkbox = (CheckBox)row.FindControl("chkSeleccion");
                if (chkbox.Checked)
                {
                    chkbox.Checked = false;
                }
            }
            txtQtycopies.Text = "1";
            ucStatus.ClearStatus();
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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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
        //        stockViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(stockViewDTO.Errors);
        //    }
        //}

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Métodos"

        public void Initialize()
        {
            context.SessionInfo.IdPage = "LabelItemConsultGS1";

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
                this.Master.ucError.ShowError(stockViewDTO.Errors);
                stockViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            stockViewDTO = iWarehousingMGR.FindAllStockItemGS1(context);

            if (!stockViewDTO.hasError() && stockViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Label.ItemGS1, stockViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud) ucStatus.ShowMessage(stockViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }
        
        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!stockViewDTO.hasConfigurationError() && stockViewDTO.Configuration != null && stockViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, stockViewDTO.Configuration);

            grdMgr.DataSource = stockViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(stockViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = lblFilterCodeLabel.Text;

            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.btnRefreshVisible = true;

            Master.ucTaskBar.BtnPrintClick += new EventHandler(btnPrint_Click);
            Master.ucTaskBar.btnPrintVisible = true;
            Master.ucTaskBar.btnPrintEnabled = false;
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
            stockViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(stockViewDTO.MessageStatus.Message);
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

                if (stockViewDTO.Entities.Count > 0)
                {
                    divPrintLabel.Visible = true;
                    this.txtQtycopies.Text = "1";
                    Master.ucTaskBar.btnPrintEnabled = true;
                }
                else
                {
                    divPrintLabel.Visible = false;
                    Master.ucTaskBar.btnPrintEnabled = false;
                }
            }
        }

        protected void LoadControls()
        {
            lblNotPrinter.Visible = false;
            txtQtycopies.Text = "1";

            string nroCopys = GetCfgParameter(CfgParameterName.MaxPrintedCopy.ToString());
            this.rvQtycopies.MaximumValue = nroCopys;
            this.rvQtycopies.ErrorMessage = (this.lblRangeQtyCopy.Text + "1 y " + nroCopys + ".");

            // Carga lista de impresoras asociadas al usuario
            base.LoadUserPrinters(this.ddlPrinters);

            ReloadLabelSize();

            // Selecciona impresora por defecto
            base.SelectDefaultPrinter(this.ddlPrinters);

            if (ddlPrinters.Items.Count == 0)
            {
                lblNotPrinter.Visible = true;
                txtQtycopies.Enabled = false;
                ddlPrinters.Enabled = false;
                Master.ucTaskBar.btnPrintEnabled = false;
            }
        }

        protected void ReloadLabelSize()
        {
            if (!String.IsNullOrEmpty(ddlPrinters.SelectedValue.Trim()))
                base.LoadLabelSize(this.ddlLabelSize, Convert.ToInt32(ddlPrinters.SelectedValue.ToString()), "SHIPITEMGS1");
        }

        private List<String> getListXml(String paramXml, List<String> theListParam, int labelLinesQty)
        {
            List<Auxiliary> theAuxiliaryList = new List<Auxiliary>();
            XmlDocument xDoc = new XmlDocument();
            String xmlItems = "", xmlHeader = "";
            List<String> theXmlList = new List<string>();
            List<String> theXmlListItem = new List<string>();
            int i = 0;
            bool addColumn = false;

            xDoc.LoadXml(paramXml);

            XmlNodeList label_param = xDoc.GetElementsByTagName("label_param");

            //XmlNodeList lista = ((XmlElement)label_param[0]).GetElementsByTagName("label_param"); 
            Auxiliary theAuxiliary;
            foreach (String theParam in theListParam)
            {
                theAuxiliary = new Auxiliary();
                theAuxiliary.Name = theParam;
                theAuxiliary.Count = 0;
                theAuxiliaryList.Add(theAuxiliary);
            }

            foreach (XmlElement nodo in label_param)
            {
                addColumn = true;
                foreach (String theParam in theListParam)
                {
                    if (theParam.Equals(nodo.ChildNodes[0].InnerText))
                    {
                        addColumn = false;
                        break;
                    }

                }
                if (addColumn)
                {
                    xmlHeader += "<label_param>" + nodo.InnerXml + "</label_param>";
                }
            }


            foreach (XmlElement nodo in label_param)
            {


                foreach (String theParam in theListParam)
                {
                    if (!nextLabel(theAuxiliaryList, labelLinesQty))
                    {
                        if (theParam.Equals(nodo.ChildNodes[0].InnerText))
                        {
                            addColumn = true;
                            for (i = 0; i < theAuxiliaryList.Count; i++)
                            {
                                if (theAuxiliaryList[i].Name == nodo.ChildNodes[0].InnerText)
                                {
                                    if (theAuxiliaryList[i].Count > 0)
                                        nodo.ChildNodes[0].InnerText += theAuxiliaryList[i].Count.ToString();
                                    xmlItems += "<label_param>" + nodo.InnerXml + "</label_param>";
                                    theAuxiliaryList[i].Count++;
                                }
                            }

                        }
                    }
                }



                if (nextLabel(theAuxiliaryList, labelLinesQty))
                {
                    theXmlListItem.Add(xmlItems);
                    xmlItems = "";
                    for (i = 0; i < theAuxiliaryList.Count; i++)
                    {
                        theAuxiliaryList[i].Count = 0;
                    }
                }

            }
            if (xmlItems.Length > 0)
            {
                theXmlListItem.Add(xmlItems);
            }

            foreach (String theListItem in theXmlListItem)
            {
                theXmlList.Add("<root><label_params>" + xmlHeader + theListItem + "</label_params></root>");
            }

            return theXmlList;
        }

        private bool nextLabel(List<Auxiliary> theAuxiliaryList, int labelLinesQty)
        {
            bool nextlabel = false;
            for (int i = 0; i < theAuxiliaryList.Count; i++)
            {
                if (theAuxiliaryList[i].Count >= labelLinesQty)
                {
                    nextlabel = true;
                }
                else
                {
                    nextlabel = false;
                    break;
                }
            }
            return nextlabel;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('StockLabelGS1_FindAll', 'ctl00_MainContent_grdMgr');", true);
        }

        #endregion
    }
}
