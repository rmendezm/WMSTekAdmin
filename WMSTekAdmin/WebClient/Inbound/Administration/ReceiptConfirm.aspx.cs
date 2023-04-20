using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.WebClient.Shared;
using System.Windows.Forms;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Inbound.Administration
{
    public partial class ReceiptConfirm : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Receipt> receiptViewDTO = new GenericViewDTO<Receipt>();
        private GenericViewDTO<ReceiptDetail> receiptDetailViewDTO = new GenericViewDTO<ReceiptDetail>();

        private bool isValidViewDTO = false;

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
                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
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
                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                                       
                    //// Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.ToolTip = this.lblDetailOrder.Text;
                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                    //e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);

                    // Asinga el atributo 'onclick' a todas las columnas de la grilla, excepto a la que contiene los checkboxes
                    // IMPORTANTE: no cambiar de lugar la columna [0] que contiene los checkboxes
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
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

                    LoadReceiptDetail(index);
                }
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }  

        protected void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                //Rescata el indice de la grilla seleccionado
                int index = int.Parse(this.hdIndexGrd.Value);

                LoadReceiptDetail(index);
                //ShowModalCloseOrder(index);

                isValidViewDTO = false;
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewDetailOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                
                }
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
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
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
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
                //    LoadReceiptDetail(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            GridView grdMgrAux = new GridView();
            GenericViewDTO<Receipt> receiptAuxViewDTO = new GenericViewDTO<Receipt>();
            Receipt theReceipt = new Receipt();
            string detailTitle;

            try
            {
                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    grdMgr.AllowPaging = false;
                    PopulateGrid();

                    grdMgrAux = grdMgr;

                    theReceipt.Id = receiptViewDTO.Entities[index].Id;
                    receiptAuxViewDTO = iReceptionMGR.GetReceiptByAnyParameter(theReceipt, context);
                    grdMgrAux.DataSource = receiptAuxViewDTO.Entities;
                    grdMgrAux.DataBind();

                    LoadReceiptDetail(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;

                    base.ExportToExcel(grdMgrAux, grdDetail, detailTitle);
                    grdMgr.AllowPaging = true;
                }


            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = false;

                foreach (GridViewRow row in grdMgr.Rows)
                {
                    System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)row.Cells[0].FindControl("chkReceiptConfirm");

                    if (chk.Checked == true)
                    {
                        isValid = true;
                    }
                }

                if (isValid)
                {
                    this.ShowConfirm(this.lblConfirmReceiptHeader.Text, lblConfirmReceipt.Text);
                }
                else
                {
                    this.Master.ucDialog.ShowAlert(this.lblConfirmReceiptHeader.Text, this.lblNotSelectedReceipt.Text, "confirm");                    
                }
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }

        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                modalPopUpDialog.Hide();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }

        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            var lstRpt = new List<Receipt>();

            try 
            {

                if (ValidateSession(WMSTekSessions.InboundConsult.ReceiptList))
                {
                    List<string> lstReceiptType = GetConst("ReceiptTypeCodeComent");

                    int idReceipt = 0;
                    bool hasMotive = false;

                    receiptViewDTO = (GenericViewDTO<Receipt>)Session[WMSTekSessions.InboundConsult.ReceiptList];

                    for (int i = 0; i < grdMgr.Rows.Count; i++)
                    {
                        GridViewRow row = grdMgr.Rows[i];

                        if (((System.Web.UI.WebControls.CheckBox)row.FindControl("chkReceiptConfirm")).Checked)
                        {
                            idReceipt = receiptViewDTO.Entities[(grdMgr.PageIndex * grdMgr.PageSize) + i].Id;

                            var r = receiptViewDTO.Entities.First(s => s.Id == idReceipt);

                            if (lstReceiptType.Contains(r.ReceiptTypeCode))
                            {
                                if (string.IsNullOrEmpty(r.SpecialField1) && string.IsNullOrEmpty(r.SpecialField2))
                                {
                                    this.ddlTypeRejection.SelectedIndex = 0;
                                    this.ddlMotiveRejection.SelectedIndex = 0;
                                    this.lblReceiptType.Text = r.ReceiptTypeName.Trim();
                                    this.lblInboundOrder.Text = r.InboundOrder.Number.ToString();
                                    this.hidIdReceiptMotive.Value = idReceipt.ToString();

                                    //Valida si es que no existe mostivos de rechazo no muestra el combo
                                    List<string> lstMotive = GetConst("ReceiptTypeMotive");
                                    if (lstMotive == null || (lstMotive.Count() == 1 && string.IsNullOrEmpty(lstMotive[0].Trim())))
                                    {
                                        this.divMotiveRejection.Visible = false;
                                    }
                                    else
                                    {
                                        this.divMotiveRejection.Visible = true;
                                    }

                                    hasMotive = true;
                                    break;
                                }
                            }

                            lstRpt.Add(r);
                        }
                    }


                    if (hasMotive)
                    {
                        isValidViewDTO = false;
                        divMessaje.Visible = true;
                        mpMessaje.Show();
                    }
                    else
                    {
                        var validateDocs = iReceptionMGR.ValidateDocsIfAllowedToCreateReceiptsForNoReceiptedItems(lstRpt, context);

                        if (validateDocs.Entities.Count > 0)
                        {
                            var msg = "Cancelación Proceso. <br> Siguientes ordenes tienen un track inválido para crear notificación articulos no recepcionados: <br>";

                            foreach (var receipt in validateDocs.Entities)
                            {
                                msg += receipt.InboundOrder.Number + "<br>";
                            }

                            ucStatus.ShowWarning(msg, 10000);
                            isValidViewDTO = false;
                            return;
                        }

                        receiptViewDTO = iReceptionMGR.UpdateInterfaceById(lstRpt, context);

                        if (receiptViewDTO.hasError())
                        {
                            isValidViewDTO = false;
                            this.Master.ucError.ShowError(receiptViewDTO.Errors);
                        }
                        else
                        {
                            isValidViewDTO = true;
                            UpdateSession();

                            grdDetail.DataSource = null;
                            grdDetail.DataBind();
                            divDetail.Visible = false;
                        }
                    }
                }
                    
                
                
             }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void btnSaveConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.InboundConsult.ReceiptList))
                {
                    List<string> lstReceiptType = GetConst("ReceiptTypeCodeComent");
                    int idReceipt = Convert.ToInt32(this.hidIdReceiptMotive.Value.Trim());

                    receiptViewDTO = (GenericViewDTO<Receipt>)Session[WMSTekSessions.InboundConsult.ReceiptList];

                    receiptViewDTO.Entities.First(s => s.Id == idReceipt).SpecialField1 = this.ddlTypeRejection.SelectedValue.Trim();
                    
                    if (this.ddlMotiveRejection.SelectedValue != "-1")
                        receiptViewDTO.Entities.First(s => s.Id == idReceipt).SpecialField2 = this.ddlMotiveRejection.SelectedValue.Trim();

                    Session.Add(WMSTekSessions.InboundConsult.ReceiptList, receiptViewDTO);

                    btnOk_Click(new object(), new EventArgs());
                }

            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        private void ShowConfirm(string title, string message)
        {           
            this.divConfirm.Visible = true;

            this.lblDialogTitle.Text = title;
            this.divDialogMessage.InnerHtml = message;

            //this.Visible = true;
            modalPopUpDialog.Show();
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
            context.SessionInfo.IdPage = "ReceiptConfirm";
            //this.Master.ucDialog.BtnOkClick += new EventHandler(btnOkClick);
            //this.Master.ucDialog.BtnCancelClick +=new EventHandler(btnCancelClick);

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            PopulateList();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .35);
                //UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.InboundConsult.ReceiptList))
                {
                    receiptViewDTO = (GenericViewDTO<Receipt>)Session[WMSTekSessions.InboundConsult.ReceiptList];
                    isValidViewDTO = true;
                }

                // Si es un ViewDTO valido, carga la grilla
                if (isValidViewDTO)
                {
                    //Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                    PopulateGrid();
                    isValidViewDTO = false;

                    if (ValidateSession(WMSTekSessions.InboundConsult.ReceiptDetailList))
                    {
                        receiptDetailViewDTO = (GenericViewDTO<ReceiptDetail>)Session[WMSTekSessions.InboundConsult.ReceiptDetailList];
                        isValidViewDTO = true;
                    }

                    if(isValidViewDTO)
                    {
                        PopulateGridDetail();
                    }
                }
            }
        }

        protected void LoadReceiptDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                int id = receiptViewDTO.Entities[index].Id;

                receiptDetailViewDTO = iReceptionMGR.LoadDetailByIdReceipt(context, id);
                lblGridDetail.Visible = true;
                imbDeleteReceipt.Visible = true;
                this.lblNroDoc.Text = receiptViewDTO.Entities[index].InboundOrder.Number.ToString();

                if (receiptDetailViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!receiptDetailViewDTO.hasConfigurationError() && receiptDetailViewDTO.Configuration != null && receiptDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, receiptDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = receiptDetailViewDTO.Entities;
                    grdDetail.DataBind();
                    
                    Session.Add(WMSTekSessions.InboundConsult.ReceiptDetailList, receiptDetailViewDTO);

                    CallJsGridViewDetail();
                }

                divDetail.Visible = true;

            }
            else
            {
                divDetail.Visible = false;
            }
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;

            this.Master.ucMainFilter.inboundTypeVisible = true;
            //this.Master.ucMainFilter.trackInboundTypeVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.InboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.InboundDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);
            this.Master.ucTaskBar.BtnSaveClick += new EventHandler(btnConfirm_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;
            this.Master.ucTaskBar.btnSaveVisible = true;
            this.Master.ucTaskBar.btnSaveToolTip = this.lblBtnSaveToolTip.Text;
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            // ucStatus.pageSizeChanged += new EventHandler(ucStatus_pageSizeChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void PopulateList()
        {
            base.LoadConst(this.ddlTypeRejection, "ReceiptTypeRejection", true, this.Master.EmptyRowText);
            base.LoadConst(this.ddlMotiveRejection, "ReceiptTypeMotive", true, this.Master.EmptyRowText);
        }

        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Carga lista de InboundOrders
            receiptViewDTO = iReceptionMGR.GetReceiptHeaderToConfirm(context);

            if (!receiptViewDTO.hasError() && receiptViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InboundConsult.ReceiptList, receiptViewDTO);
                isValidViewDTO = true;
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!receiptViewDTO.hasConfigurationError() && receiptViewDTO.Configuration != null && receiptViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, receiptViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = null;
            grdMgr.DataBind();
            grdMgr.DataSource = receiptViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(receiptViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }

        private void PopulateGridDetail()
        {
            // Configura ORDEN de las columnas de la grilla
            if (!receiptDetailViewDTO.hasConfigurationError() && receiptDetailViewDTO.Configuration != null && receiptDetailViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdDetail, receiptDetailViewDTO.Configuration);

            // Detalle de Documentos de Entrada
            grdDetail.DataSource = receiptDetailViewDTO.Entities;
            grdDetail.DataBind();

            CallJsGridViewDetail();
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                currentIndex = -1;
                divDetail.Visible = false;
                divGrid.Visible = true;
                this.Master.ucError.ClearError();
            }
        }

        protected void imbDeleteReceipt_Click(object sender, ImageClickEventArgs e)
        {
            List<string> lstRpt = new List<string>();
            GenericViewDTO<ReceiptDetail> lstRptDetail = new GenericViewDTO<ReceiptDetail>();

            try
            {

                if (ValidateSession(WMSTekSessions.InboundConsult.ReceiptDetailList))
                {
                    GenericViewDTO<ReceiptDetail> receiptDetailTempDTO = new GenericViewDTO<ReceiptDetail>();
                    ReceiptDetail theReceiptDetailParam;
                    
                    bool hasSelection = false;

                    receiptDetailViewDTO = (GenericViewDTO<ReceiptDetail>)Session[WMSTekSessions.InboundConsult.ReceiptDetailList];
                    receiptViewDTO = (GenericViewDTO<Receipt>)Session[WMSTekSessions.InboundConsult.ReceiptList];

                    for (int i = 0; i < grdDetail.Rows.Count; i++)
                    {
                        GridViewRow row = grdDetail.Rows[i];

                        if (((System.Web.UI.WebControls.CheckBox)row.FindControl("chkReceiptDetailConfirm")).Checked)
                        {
                            theReceiptDetailParam = new ReceiptDetail();
                            if (theReceiptDetailParam.Receipt == null)
                                theReceiptDetailParam.Receipt = new Receipt();
                            if (theReceiptDetailParam.LPN == null)
                                theReceiptDetailParam.LPN = new LPN();
                            if (theReceiptDetailParam.Item == null)
                                theReceiptDetailParam.Item = new Item();
                            if (theReceiptDetailParam.CategoryItem == null)
                                theReceiptDetailParam.CategoryItem = new CategoryItem();
                            theReceiptDetailParam.Receipt.Id = receiptViewDTO.Entities[(grdMgr.PageIndex * grdMgr.PageSize) + grdMgr.SelectedIndex].Id;
                            theReceiptDetailParam.Id = receiptDetailViewDTO.Entities[i].Id;
                            theReceiptDetailParam.LPN.IdCode = receiptDetailViewDTO.Entities[i].LPN.IdCode;
                            theReceiptDetailParam.Item.Id = receiptDetailViewDTO.Entities[i].Item.Id;
                            theReceiptDetailParam.CategoryItem.Id = receiptDetailViewDTO.Entities[i].CategoryItem.Id;
                            theReceiptDetailParam.LotNumber = receiptDetailViewDTO.Entities[i].LotNumber;
                            theReceiptDetailParam.FifoDate = receiptDetailViewDTO.Entities[i].FifoDate;
                            theReceiptDetailParam.ExpirationDate = receiptDetailViewDTO.Entities[i].ExpirationDate;
                            theReceiptDetailParam.FabricationDate = receiptDetailViewDTO.Entities[i].FabricationDate;

                            receiptDetailTempDTO = iReceptionMGR.GetReceiptDetailByAnyParameter(theReceiptDetailParam, context);

                            //var r = receiptDetailViewDTO.Entities.First(s => s.Id == idReceiptDetail);

                            //lstRpt.Add(idReceiptDetail.ToString() + "|" + r.Receipt.Id);

                            foreach(ReceiptDetail rowReceiptDetail in receiptDetailTempDTO.Entities)
                            {
                                lstRptDetail.Entities.Add(rowReceiptDetail);
                                hasSelection = true;
                            }
                            
                        }
                    }

                   
                    if(hasSelection)
                    {
                        var typeKardexConst = GetConst("TypeKardexCancelReceipt");

                        KardexType kardexType = new KardexType();
                        GenericViewDTO<KardexType> kardexTypeViewDTO = iWarehousingMGR.FindAllKardexType(context);
                        kardexType = kardexTypeViewDTO.Entities.Find(f => f.IdKardexType.Equals(Convert.ToInt32(typeKardexConst[0])));

                        receiptViewDTO = iReceptionMGR.CancelReceiptDetailById(lstRptDetail, kardexType, context);

                        if (receiptViewDTO.hasError())
                        {
                            isValidViewDTO = false;
                            this.Master.ucError.ShowError(receiptViewDTO.Errors);
                        }
                        else
                        {
                            isValidViewDTO = true;
                            UpdateSession();

                            grdDetail.DataSource = null;
                            grdDetail.DataBind();
                            divDetail.Visible = false;
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('GetReceiptHeaderToConfirm', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDropCustom();", true);
        }
        #endregion
    }
}
