using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Inbound.Consult
{
    public partial class ReceiptServiceLevel : BasePage
    {
        #region "Declaración de Variables"
        //private GenericViewDTO<Receipt> receiptViewDTO = new GenericViewDTO<Receipt>();
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
                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
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
                        //LoadReceiptDetail(currentIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
            }
        }

        // Carga la grilla, filtrada por el criterio de busqueda ingresado
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
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
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
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
                    //LoadReceiptDetail(index);
                    //detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                }
                else
                {
                    detailTitle = null;
                }

                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
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
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                //divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                //divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                //divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                //divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                //divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
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
                }
            }
            catch (Exception ex)
            {
                receiptDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "ReceiptConsult";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                //hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
                UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.InboundConsult.ReceiptServiceLevelList))
                {
                    receiptDetailViewDTO = (GenericViewDTO<ReceiptDetail>)Session[WMSTekSessions.InboundConsult.ReceiptServiceLevelList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //FILTRO BASICO
            //Centro
            this.Master.ucMainFilter.warehouseVisible = true;
            //Dueño
            this.Master.ucMainFilter.ownerVisible = true;
            //InboundType
            this.Master.ucMainFilter.inboundTypeVisible = true;
            //ReferenceDocType
            this.Master.ucMainFilter.referenceDocTypeVisible = true;
            //NºDoc
            this.Master.ucMainFilter.documentVisible = true;
            //Recepcion Desde
            this.Master.ucMainFilter.dateFromVisible = true;
            //Recepcion Hasta
            this.Master.ucMainFilter.dateToVisible = true;
            //Cod Item
            this.Master.ucMainFilter.itemVisible = true;

            //FILTRO AVANZADO
            // Habilita el Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = false;
            //Tab Fecha
            this.Master.ucMainFilter.tabDatesVisible = false;
            this.Master.ucMainFilter.expirationDateVisible = false;
            this.Master.ucMainFilter.expectedDateVisible = false;
            //Tab Documento
            this.Master.ucMainFilter.tabDocumentVisible = false;
            this.Master.ucMainFilter.vendorVisible = false;
            this.Master.ucMainFilter.carrierVisible = false;
            this.Master.ucMainFilter.driverVisible = false;
            //TabGrupos
            this.Master.ucMainFilter.tabItemGroupVisible = false;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.ReceiptDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.ReceiptDaysAfter;

            //Setea los filtros para que no tengan la propiedad de autopostback
            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
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

        private void UpdateSession()
        {
            // carga todas las recepciones 
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            receiptDetailViewDTO = iReceptionMGR.GetReceiptServiceLevel(context);

            if (!receiptDetailViewDTO.hasError() && receiptDetailViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InboundConsult.ReceiptServiceLevelList, receiptDetailViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(receiptDetailViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(receiptDetailViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!receiptDetailViewDTO.hasConfigurationError() && receiptDetailViewDTO.Configuration != null && receiptDetailViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, receiptDetailViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = receiptDetailViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(receiptDetailViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                //divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
            }
        }

        //Formatea campos de la grilla
        protected string GetFormatedNumber(object number)
        {
            if (number != null)
            {
                if (number.GetType() == typeof(Decimal))
                {
                    return String.Format("{0:N3}", number);
                }
                else
                {
                    return number.ToString();
                }
            }
            return "0";
        }

        #endregion
    }
}
