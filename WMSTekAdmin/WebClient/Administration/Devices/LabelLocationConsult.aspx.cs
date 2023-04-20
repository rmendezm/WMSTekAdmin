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


namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class LabelLocationConsult: BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Location> labelLocationViewDTO = new GenericViewDTO<Location>();
        private BaseViewDTO configurationViewDTO = new BaseViewDTO();
        private GenericViewDTO<TaskLabel> taskLabelViewDTO;
        private GenericViewDTO<LabelTemplate> labelTemplateViewDTO;
    
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

                    // Nota: este mantenedor no carga inicialmente la grilla por la gran cantidad de datos que devolvería
                    if (Page.IsPostBack)
                    {
                        if (ValidateSession(WMSTekSessions.Label.Location))
                        {
                            labelLocationViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.Label.Location];
                            isValidViewDTO = true;
                        }

                        // Si es un ViewDTO valido, carga la grilla y las listas
                        if (isValidViewDTO)
                        {
                            // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                            PopulateGrid();
                        }
                    }
                    else
                    {
                        LoadControls();
                    }

                }
            }
            catch (Exception ex)
            {
                labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
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
                labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
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
                labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
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
                labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
            }
        }

        /// <summary>
        /// Inserta TaskLabel
        /// </summary>
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            int qtyCopies = 1;

            if (txtQtycopies.Text.Trim() != string.Empty)
                qtyCopies = Convert.ToInt32(txtQtycopies.Text.Trim());

            taskLabelViewDTO = new GenericViewDTO<TaskLabel>();
            List<TaskLabel> taskLabels = new List<TaskLabel>();

            //labelTemplateViewDTO = iLabelMGR.GetLabelByCode("LOC", context);

            //if (labelTemplateViewDTO.hasError())
            //{
            //    this.Master.ucError.ShowError(labelTemplateViewDTO.Errors);
            //}
            //else
            if (Convert.ToInt32(ddlLabelSize.SelectedValue) > 0)
            {
                foreach (GridViewRow row in this.grdMgr.Rows)
                {
                    CheckBox chkbox = (CheckBox)row.FindControl("chkSeleccion");
                    if (chkbox.Checked)
                    {
                        int index = grdMgr.PageSize * grdMgr.PageIndex + row.RowIndex;
                        String lblIdLocCode = labelLocationViewDTO.Entities[index].IdCode;
                        //Label lblIdLocCode = (Label)row.FindControl("lblIdLocCode");

                        string xmlParam = iLabelMGR.GetForIdLocCode(lblIdLocCode.Trim());

                        TaskLabel taskLabel = new TaskLabel();

                        int Valor = Convert.ToInt32(ddlLabelSize.SelectedValue);
                        //if (!labelTemplateViewDTO.hasError())
                        //    Valor = Convert.ToInt32(labelTemplateViewDTO.Entities[0].Id.ToString());

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

                if (taskLabelViewDTO.Entities.Count > 0)
                {
                    // Crea el registro a imprimir en TaskLabel
                    taskLabelViewDTO = iLabelMGR.MaintainTaskLabel(CRUD.Create, taskLabelViewDTO.Entities, context, qtyCopies);

                    if (!taskLabelViewDTO.hasError())
                    {
                        crud = true;
                        ucStatus.ShowMessage(taskLabelViewDTO.MessageStatus.Message);
                        txtQtycopies.Text = string.Empty;
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
                    this.Master.ucDialog.ShowAlert(lblTitle.Text, lblNoRowsSelected.Text, string.Empty);
                }
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
                labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
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

            txtQtycopies.Text = string.Empty;
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
                labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
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
                labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
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
                labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
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
                labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
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
                labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
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
        //        labelLocationViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
        //    }
        //}

        #endregion

        #region "Métodos"

        public void Initialize()
        {
            context.SessionInfo.IdPage = "LabelLocationConsult";

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
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
                labelLocationViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            labelLocationViewDTO = iLayoutMGR.FindAllLocation(context);

            if (!labelLocationViewDTO.hasError() && labelLocationViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Label.Location, labelLocationViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud) ucStatus.ShowMessage(labelLocationViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(labelLocationViewDTO.Errors);
            }
        }
        
        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;


            // Configura ORDEN de las columnas de la grilla
            if (!labelLocationViewDTO.hasConfigurationError() && labelLocationViewDTO.Configuration != null && labelLocationViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, labelLocationViewDTO.Configuration);

            grdMgr.DataSource = labelLocationViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(labelLocationViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            CallJsGridViewHeader();
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
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.ownerIncludeNulls = true;
            this.Master.ucMainFilter.SetDefaultOwner = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.locationTypeVisible = true;
            this.Master.ucMainFilter.hangarVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.codeLabel = lblFilterCode.Text;
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
            labelLocationViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(labelLocationViewDTO.MessageStatus.Message);
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

                if (labelLocationViewDTO.Entities.Count > 0)
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
            this.lblNotPrinter.Visible = false;
            this.txtQtycopies.Text = "1";
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
                this.lblNotPrinter.Visible = true;
                this.txtQtycopies.Enabled = false;
                this.ddlPrinters.Enabled = false;
                Master.ucTaskBar.btnPrintEnabled = false;
            }
        }

        protected XmlDocument CreateXmlLabel(Location location)
        {
            string strPathXmlTemplate = Request.PhysicalApplicationPath + ConfigurationSettings.AppSettings["LabelLocationPath"]; 

            XmlDocument xmlDoc = new XmlDocument();
            
            try
            {
                xmlDoc.Load(strPathXmlTemplate);

                xmlDoc.SelectSingleNode(@"root/idwhs").InnerText = location.Warehouse.Id.ToString();
                xmlDoc.SelectSingleNode(@"root/aisle").InnerText = location.Aisle.ToString();
                xmlDoc.SelectSingleNode(@"root/rowloc").InnerText = location.Row.ToString();
                xmlDoc.SelectSingleNode(@"root/columnloc").InnerText = location.Column.ToString();
                xmlDoc.SelectSingleNode(@"root/levelloc").InnerText = location.Level.ToString();
                xmlDoc.SelectSingleNode(@"root/loccode").InnerText = location.IdCode.ToString();
            }
            catch (Exception ex)
            {
                theLog.errorMessage(this.GetType().ToString(), ex.Message.ToString());
            }

            return xmlDoc;
        }

        protected void ReloadLabelSize()
        {
            if (!String.IsNullOrEmpty(ddlPrinters.SelectedValue.Trim()))
                base.LoadLabelSize(this.ddlLabelSize, Convert.ToInt32(ddlPrinters.SelectedValue.ToString()), "LOC");
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('Location_FindAll', 'ctl00_MainContent_grdMgr');", true);
        }

        #endregion

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}
