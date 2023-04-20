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
    public partial class LabelItemLotConsult : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<ItemUomLot> ItemUomLotViewDTO = new GenericViewDTO<ItemUomLot>();
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

                    if (!Page.IsPostBack)
                    {
                        LoadControls();
                    }
                    else
                    {
                        if (ValidateSession(WMSTekSessions.Shared.ItemUomLotList))
                        {
                            ItemUomLotViewDTO = (GenericViewDTO<ItemUomLot>)Session[WMSTekSessions.Shared.ItemUomLotList];
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
            }
            catch (Exception ex)
            {
                ItemUomLotViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
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
                ItemUomLotViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
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
                ItemUomLotViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
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
                ItemUomLotViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
            }
        }

        /// <summary>
        /// Inserta TaskLabel
        /// </summary>
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            taskLabelViewDTO = new GenericViewDTO<TaskLabel>();
            List<TaskLabel> taskLabels = new List<TaskLabel>();
            List<TaskLabelCant> taskLabelCants = new List<TaskLabelCant>();

            //labelTemplateViewDTO = iLabelMGR.GetLabelByCode("ITEM2D", context);

            //if (labelTemplateViewDTO.hasError())
            //{
            //    this.Master.ucError.ShowError(labelTemplateViewDTO.Errors);
            //}
            //else
            if (Convert.ToInt32(ddlLabelSize.SelectedValue) > 0)
            {
                int a = 0;

                foreach (ItemUomLot itemUom in ItemUomLotViewDTO.Entities)
                {
                    if (itemUom.Selected)
                    {
                        #region Variables
                        String p_LongItemName1 = String.Empty;
                        String p_LongItemName2 = String.Empty;
                        String p_ExpirationDate = string.Empty;
                        String p_ExpirationDateFormat = String.Empty;
                        String p_Lote = String.Empty;
                        #endregion Variables

                        #region Substring LongItenName
                        if (ItemUomLotViewDTO.Entities[a].LongItemName.Length >= 20)
                        {
                            if (ItemUomLotViewDTO.Entities[a].LongItemName.Length > 20)
                            {
                                p_LongItemName1 = ItemUomLotViewDTO.Entities[a].LongItemName.Substring(0, 20);

                                if (ItemUomLotViewDTO.Entities[a].LongItemName.Length > 40)
                                    p_LongItemName2 = ItemUomLotViewDTO.Entities[a].LongItemName.Substring(20, 20);
                                else
                                    p_LongItemName2 = ItemUomLotViewDTO.Entities[a].LongItemName.Substring(20, ItemUomLotViewDTO.Entities[a].LongItemName.Length - 20);
                            }
                            else
                                p_LongItemName1 = ItemUomLotViewDTO.Entities[a].LongItemName.Substring(0, 19);
                        }
                        else
                            p_LongItemName1 = ItemUomLotViewDTO.Entities[a].LongItemName;
                        #endregion Substring LongItenName

                        #region Maneja Lote
                        if (ItemUomLotViewDTO.Entities[a].UseLot)
                        {
                            p_ExpirationDate = ItemUomLotViewDTO.Entities[a].ExpirationDatePrint.ToShortDateString();
                            p_ExpirationDateFormat = ItemUomLotViewDTO.Entities[a].ExpirationDatePrint.ToString("ddMMyy");
                            p_Lote = ItemUomLotViewDTO.Entities[a].LotNumber;
                        }
                        #endregion Maneja Lote

                        Int32 p_idItem = ItemUomLotViewDTO.Entities[a].IdItem;
                        Int32 p_idUom = ItemUomLotViewDTO.Entities[a].IdUom;

                        string xmlParam = iLabelMGR.GetForItemLot(p_LongItemName1, p_LongItemName2, p_ExpirationDate, p_ExpirationDateFormat, p_Lote, p_idItem, p_idUom);

                        TaskLabel taskLabel = new TaskLabel();
                        TaskLabelCant taskLabelCant = new TaskLabelCant();

                        int Valor = Convert.ToInt32(ddlLabelSize.SelectedValue);
                        //if (!labelTemplateViewDTO.hasError())
                        //    Valor = Convert.ToInt32(labelTemplateViewDTO.Entities[0].Id.ToString());

                        taskLabelCant.LabelTemplate = new LabelTemplate();
                        taskLabelCant.LabelTemplate.Id = Valor;

                        taskLabelCant.Printer = new Printer(Convert.ToInt32(ddlPrinters.SelectedValue.ToString()));
                        taskLabelCant.User = new Binaria.WMSTek.Framework.Entities.Profile.User(context.SessionInfo.User.Id);

                        taskLabelCant.DelayPrinted = 0;
                        taskLabelCant.IsPrinted = false;
                        taskLabelCant.ParamString = xmlParam;
                        taskLabelCant.Cantidad = itemUom.CopNumber;
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
                    a++;
                }

                taskLabelViewDTO.Entities = taskLabels;

                if (taskLabelCants.Count > 0)
                {
                    // Crea el registro a imprimir en TaskLabel
                    taskLabelViewDTO = iLabelMGR.MaintainTaskLabelCant(CRUD.Create,taskLabelCants, context);

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
                
                labelTemplateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelTemplateViewDTO.Errors);
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
                ItemUomLotViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
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
                ItemUomLotViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
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
                ItemUomLotViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
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
                ItemUomLotViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
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
                ItemUomLotViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
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
        //        labelItemViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(labelItemViewDTO.Errors);
        //    }
        //}

        #endregion

        #region "Métodos"

        public void Initialize()
        {
            context.SessionInfo.IdPage = "LabelItemLotConsult";

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
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
                ItemUomLotViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            ItemUomLotViewDTO = iWarehousingMGR.FindAllItemUomLot(context);

            if (!ItemUomLotViewDTO.hasError() && ItemUomLotViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Shared.ItemUomLotList, ItemUomLotViewDTO);
                isValidViewDTO = true;
                //PopulateGrid();
                //Muestra Mensaje en barra de status
                if (!crud) ucStatus.ShowMessage(ItemUomLotViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(ItemUomLotViewDTO.Errors);
            }
        }
        
        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y Visibilidad de las columnas de la grilla
            if (!ItemUomLotViewDTO.hasConfigurationError() && ItemUomLotViewDTO.Configuration != null && ItemUomLotViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, ItemUomLotViewDTO.Configuration);

            grdMgr.DataSource = ItemUomLotViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(ItemUomLotViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            
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
            ItemUomLotViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(ItemUomLotViewDTO.MessageStatus.Message);
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

                if (ItemUomLotViewDTO.Entities.Count > 0)
                {
                    divPrintLabel.Visible = true;
                    //Master.ucTaskBar.btnPrintEnabled = true;
                }
                else
                {
                    divPrintLabel.Visible = false;
                    //Master.ucTaskBar.btnPrintEnabled = false;
                }
            }
        }

        protected void LoadControls()
        {
            lblNotPrinter.Visible = false;
           // txtQtycopies.Text = string.Empty;

            // Carga lista de impresoras asociadas al usuario
            base.LoadUserPrinters(this.ddlPrinters);

            ReloadLabelSize();

            // Selecciona impresora por defecto
            base.SelectDefaultPrinter(this.ddlPrinters);

            if (ddlPrinters.Items.Count == 0)
            {
                lblNotPrinter.Visible = true;
                //txtQtycopies.Enabled = false;
                ddlPrinters.Enabled = false;
            }
            Master.ucTaskBar.btnPrintEnabled = false;
        }


        #endregion

        protected void chkSeleccion_CheckedChanged(object sender, EventArgs e)
        {
            int a = 0;
            foreach (GridViewRow oldrow in grdMgr.Rows)
            {
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + a;

                if (((CheckBox)oldrow.FindControl("chkSeleccion")).Checked)
                {
                    ItemUomLotViewDTO.Entities[editIndex].Selected = true;

                    Int32 IdItem = ItemUomLotViewDTO.Entities[editIndex].IdItem;
                    Int32 IdOwn = ItemUomLotViewDTO.Entities[editIndex].IdOwn;
                    GenericViewDTO<Item> item = iWarehousingMGR.GetItemLotUse(IdItem, IdOwn, context);

                    if (item.Entities.Count > 0)
                    {
                        ItemUomLotViewDTO.Entities[editIndex].UseLot = true;
                    }
                }
                else
                {
                    ItemUomLotViewDTO.Entities[editIndex].Selected = false;
                    ItemUomLotViewDTO.Entities[editIndex].UseLot = false;
                    ItemUomLotViewDTO.Entities[editIndex].CopNumber = 0;
                    ItemUomLotViewDTO.Entities[editIndex].LotNumber = String.Empty;
                    ItemUomLotViewDTO.Entities[editIndex].ExpirationDatePrint = DateTime.MinValue;
                }

                a++;

            }
            EvalVisiblePrint();
        }
        
        public String DisplayDate(object objImageSm)
        {
            if(DateTime.Parse(objImageSm.ToString()).Equals(DateTime.MinValue))
                return "";
            else
                return DateTime.Parse(objImageSm.ToString()).ToShortDateString();
        }

        protected void grdMgr_RowEditing1(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

                TextBox txtCopiasGrid = (TextBox)grdMgr.Rows[editIndex].FindControl("txtCopias");

                if (String.IsNullOrEmpty(txtCopiasGrid.Text))
                {
                    Master.ucDialog.ShowAlert("Parametro Requerido", "Debe ingresar el número de Copias", "");
                    return;
                }

                ItemUomLotViewDTO.Entities[editIndex].CopNumber = int.Parse(txtCopiasGrid.Text);

                
                if (ItemUomLotViewDTO.Entities[editIndex].UseLot)
                {
                    TextBox txtLotNumberGrid = (TextBox)grdMgr.Rows[editIndex].FindControl("txtLote");

                    if (!String.IsNullOrEmpty(txtLotNumberGrid.Text))
                        ItemUomLotViewDTO.Entities[editIndex].LotNumber = txtLotNumberGrid.Text;
                    else
                    {
                        Master.ucDialog.ShowAlert("Parametro Requerido", "Debe ingresar el número de Lote", "");
                        return;
                    }

                    TextBox txtVencimientoGrid = (TextBox)grdMgr.Rows[editIndex].FindControl("txtVencimiento");

                    if (!String.IsNullOrEmpty(txtVencimientoGrid.Text))
                        ItemUomLotViewDTO.Entities[editIndex].ExpirationDatePrint = DateTime.Parse(txtVencimientoGrid.Text);
                    else
                    {
                        Master.ucDialog.ShowAlert("Parametro Requerido", "Debe ingresar la fecha de vencimiento", "");
                        return;
                    }
                }

                EvalVisiblePrint();

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
            foreach (ItemUomLot detail in ItemUomLotViewDTO.Entities)
            {
                if (detail.Selected)
                {
                    if (detail.UseLot)
                    {
                        if (!String.IsNullOrEmpty(detail.LotNumber) && detail.ExpirationDatePrint > DateTime.MinValue && detail.CopNumber>0)
                        {
                            a++;
                        }
                    }
                    else
                    {
                        a++;
                    }
                }
            }
            if (a > 0)
                Master.ucTaskBar.btnPrintEnabled = true;
            else
                Master.ucTaskBar.btnPrintEnabled = false;
        }

        protected void ReloadLabelSize()
        {
            if (!String.IsNullOrEmpty(ddlPrinters.SelectedValue.Trim()))
                base.LoadLabelSize(this.ddlLabelSize, Convert.ToInt32(ddlPrinters.SelectedValue.ToString()), "LOT");
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}
