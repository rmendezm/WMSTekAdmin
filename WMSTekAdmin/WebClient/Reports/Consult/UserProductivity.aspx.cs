using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using System.Linq;
using Binaria.WMSTek.AdminApp.Warehousing;
using System.Collections.Generic;
using System.Globalization;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Reports
{
    public partial class UserProductivity : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<MovementWeb> movementWebViewDTO = new GenericViewDTO<MovementWeb>();
        
        public struct MovementUserProductivityStruct
        {
            private int idWhs;
            private string whsCode;
            private string whsName;
            private int idOwn;
            private string ownCode;
            private string ownName;
            private string movementName;
            private string period;
            private string userName;
            private decimal totalOrders;
            private decimal totalItems;
            private decimal totalLPNs;
            private decimal totalQty;
            public decimal OrdersAvg { get; set; }
            public decimal LinesAvg { get; set; }
            public decimal LpnsAvg { get; set; }
            public decimal QtyItemsAvg { get; set; }

            public int IdWhs
            {
                get { return idWhs; }
                set { idWhs = value; }
            }
            public string WhsCode
            {
                get { return whsCode; }
                set { whsCode = value; }
            }
            public string WhsName
            {
                get { return whsName; }
                set { whsName = value; }
            }
            public int IdOwn
            {
                get { return idOwn; }
                set { idOwn = value; }
            }
            public string OwnCode
            {
                get { return ownCode; }
                set { ownCode = value; }
            }
            public string OwnName
            {
                get { return ownName; }
                set { ownName = value; }
            }
            public string MovementName
            {
                get { return movementName; }
                set { movementName = value; }
            }
            public string Period
            {
                get { return period; }
                set { period = value; }
            }
            public string UserName
            {
                get { return userName; }
                set { userName = value; }
            }
            public decimal TotalOrders
            {
                get { return totalOrders; }
                set { totalOrders = value; }
            }
            public decimal TotalItems
            {
                get { return totalItems; }
                set { totalItems = value; }
            }
            public decimal TotalLPNs
            {
                get { return totalLPNs; }
                set { totalLPNs = value; }
            }
            public decimal TotalQty
            {
                get { return totalQty; }
                set { totalQty = value; }
            }
        }

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
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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

                ////Realiza un renderizado de la pagina
                //ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "SetDivs();", true);
            }
            catch (Exception ex)
            {
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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

                    //LoadReceiptDetail(index);
                }
            }
            catch (Exception ex)
            {
                movementWebViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
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
            context.SessionInfo.IdPage = "UserProductivity";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                //hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
                //UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.MovementConsult.MovementLogList))
                {
                    movementWebViewDTO = (GenericViewDTO<MovementWeb>)Session[WMSTekSessions.MovementConsult.MovementLogList];
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
            //Tipo Movimientos
            this.Master.ucMainFilter.movementTypeVisible = true;
            //Usuario
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.nameLabel = "Usuario";
            // WMSProcess
            this.Master.ucMainFilter.listWmsProcessType = new System.Collections.Generic.List<string>();
            this.Master.ucMainFilter.listWmsProcessType = GetConst("UserProductivity");

            //Recepcion Desde
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateLabel = "Fecha";
            //Recepcion Hasta
            this.Master.ucMainFilter.dateToVisible = true;

            this.Master.ucMainFilter.ddlPeriodVisible = true;
            this.Master.ucMainFilter.ddlViewVisible = true;

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

            movementWebViewDTO = iWarehousingMGR.FindMovementUserProductivity(context);

            if (!movementWebViewDTO.hasError() && movementWebViewDTO.Entities != null)
            {

                Session.Add(WMSTekSessions.MovementConsult.MovementLogList, movementWebViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(movementWebViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(movementWebViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!movementWebViewDTO.hasConfigurationError() && movementWebViewDTO.Configuration != null && movementWebViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, movementWebViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = UserProductivityCalculate(movementWebViewDTO, this.Master.ucMainFilter.valuePeriod, this.Master.ucMainFilter.valueView);
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(grdMgr.Rows.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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

        private object UserProductivityCalculate(GenericViewDTO<MovementWeb> movementWebViewDTO_aux, string period, string view)
        {
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            List<MovementUserProductivityStruct> lista = null;

            var range = CalculateRangeFromDates(period, movementWebViewDTO_aux.Entities);

            if (range == 0)
            {
                ucStatus.ShowError(lblFilterDateError.Text);
                return null;
            }

            if (view == "USER")
            {
                switch (period)
                {
                    case "DAY":

                        var mvmtDAY = from movement in movementWebViewDTO_aux.Entities
                                      group movement by new
                                      {
                                          idWhs = movement.Warehouse.Id,
                                          whsCode = movement.Warehouse.Code,
                                          whsName = movement.Warehouse.Name,
                                          IdOwn = movement.Owner.Id,
                                          ownCode = movement.Owner.Code,
                                          ownName = movement.Owner.Name,
                                          movementName = movement.MovementType.Name,
                                          userName = movement.UserName,
                                          period = movement.EndTime.Date.ToString("dd MMMM yyyy")
                                      } into movementGroup
                                      select new MovementUserProductivityStruct
                                      {
                                          IdWhs = movementGroup.Key.idWhs,
                                          WhsCode = movementGroup.Key.whsCode,
                                          WhsName = movementGroup.Key.whsName,
                                          IdOwn = movementGroup.Key.IdOwn,
                                          OwnCode = movementGroup.Key.ownCode,
                                          OwnName = movementGroup.Key.ownName,
                                          MovementName = movementGroup.Key.movementName,
                                          UserName = movementGroup.Key.userName,
                                          Period = movementGroup.Key.period.ToString(),
                                          TotalOrders = movementGroup.Select(x => x.DocumentNumber).Distinct().Count(),
                                          TotalItems = movementGroup.Select(x => x.Item.Id).Distinct().Count(),
                                          TotalLPNs = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count(),
                                          TotalQty = movementGroup.Sum(x => x.ItemQtyMov),
                                          OrdersAvg = movementGroup.Select(x => x.DocumentNumber).Distinct().Count() / range,
                                          LinesAvg = movementGroup.Select(x => x.DocumentLineNumber).Count() / range,
                                          LpnsAvg = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count() / range,
                                          QtyItemsAvg = movementGroup.Sum(x => x.ItemQtyMov) / range
                                      };
                        lista = mvmtDAY.ToList();
                        break;
                    case "WEEK":

                        var mvmtWEEK = from movement in movementWebViewDTO_aux.Entities
                                       group movement by new
                                       {
                                           idWhs = movement.Warehouse.Id,
                                           whsCode = movement.Warehouse.Code,
                                           whsName = movement.Warehouse.Name,
                                           IdOwn = movement.Owner.Id,
                                           ownCode = movement.Owner.Code,
                                           ownName = movement.Owner.Name,
                                           movementName = movement.MovementType.Name,
                                           userName = movement.UserName,
                                           period = "Semana N°" + cal.GetWeekOfYear(movement.EndTime.Date, System.Globalization.CalendarWeekRule.FirstDay, System.DayOfWeek.Sunday).ToString()
                                       } into movementGroup
                                       select new MovementUserProductivityStruct
                                       {
                                           IdWhs = movementGroup.Key.idWhs,
                                           WhsCode = movementGroup.Key.whsCode,
                                           WhsName = movementGroup.Key.whsName,
                                           IdOwn = movementGroup.Key.IdOwn,
                                           OwnCode = movementGroup.Key.ownCode,
                                           OwnName = movementGroup.Key.ownName,
                                           MovementName = movementGroup.Key.movementName,
                                           UserName = movementGroup.Key.userName,
                                           Period = movementGroup.Key.period.ToString(),
                                           TotalOrders = movementGroup.Select(x => x.DocumentNumber).Distinct().Count(),
                                           TotalItems = movementGroup.Select(x => x.Item.Id).Distinct().Count(),
                                           TotalLPNs = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count(),
                                           TotalQty = movementGroup.Sum(x => x.ItemQtyMov),
                                           OrdersAvg = movementGroup.Select(x => x.DocumentNumber).Distinct().Count() / range,
                                           LinesAvg = movementGroup.Select(x => x.DocumentLineNumber).Count() / range,
                                           LpnsAvg = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count() / range,
                                           QtyItemsAvg = movementGroup.Sum(x => x.ItemQtyMov) / range
                                       };
                        lista = mvmtWEEK.OrderBy(x => Convert.ToInt32(x.Period.Substring(x.Period.LastIndexOf('°') + 1))).ToList();
                        break;
                    case "MONTH":

                        var mvmtMONTH = from movement in movementWebViewDTO_aux.Entities
                                        group movement by new
                                        {
                                            idWhs = movement.Warehouse.Id,
                                            whsCode = movement.Warehouse.Code,
                                            whsName = movement.Warehouse.Name,
                                            IdOwn = movement.Owner.Id,
                                            ownCode = movement.Owner.Code,
                                            ownName = movement.Owner.Name,
                                            movementName = movement.MovementType.Name,
                                            userName = movement.UserName,
                                            period = movement.EndTime.ToString("MMMM").ToUpper()
                                        } into movementGroup
                                        select new MovementUserProductivityStruct
                                        {
                                            IdWhs = movementGroup.Key.idWhs,
                                            WhsCode = movementGroup.Key.whsCode,
                                            WhsName = movementGroup.Key.whsName,
                                            IdOwn = movementGroup.Key.IdOwn,
                                            OwnCode = movementGroup.Key.ownCode,
                                            OwnName = movementGroup.Key.ownName,
                                            MovementName = movementGroup.Key.movementName,
                                            UserName = movementGroup.Key.userName,
                                            Period = movementGroup.Key.period.ToString(),
                                            TotalOrders = movementGroup.Select(x => x.DocumentNumber).Distinct().Count(),
                                            TotalItems = movementGroup.Select(x => x.Item.Id).Distinct().Count(),
                                            TotalLPNs = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count(),
                                            TotalQty = movementGroup.Sum(x => x.ItemQtyMov),
                                            OrdersAvg = movementGroup.Select(x => x.DocumentNumber).Distinct().Count() / range,
                                            LinesAvg = movementGroup.Select(x => x.DocumentLineNumber).Count() / range,
                                            LpnsAvg = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count() / range,
                                            QtyItemsAvg = movementGroup.Sum(x => x.ItemQtyMov) / range
                                        };
                        lista = mvmtMONTH.ToList();
                        break;
                }
            }
            else 
            {
                switch (period)
                {
                    case "DAY":

                        var mvmtDAY = from movement in movementWebViewDTO_aux.Entities
                                      group movement by new
                                      {
                                          idWhs = movement.Warehouse.Id,
                                          whsCode = movement.Warehouse.Code,
                                          whsName = movement.Warehouse.Name,
                                          IdOwn = movement.Owner.Id,
                                          ownCode = movement.Owner.Code,
                                          ownName = movement.Owner.Name,
                                          movementName = movement.MovementType.Name,
                                          //userName = movement.UserName,
                                          period = movement.EndTime.Date.ToString("dd MMMM yyyy")
                                      } into movementGroup
                                      select new MovementUserProductivityStruct
                                      {
                                          IdWhs = movementGroup.Key.idWhs,
                                          WhsCode = movementGroup.Key.whsCode,
                                          WhsName = movementGroup.Key.whsName,
                                          IdOwn = movementGroup.Key.IdOwn,
                                          OwnCode = movementGroup.Key.ownCode,
                                          OwnName = movementGroup.Key.ownName,
                                          MovementName = movementGroup.Key.movementName,
                                          //UserName = movementGroup.Key.userName,
                                          Period = movementGroup.Key.period.ToString(),
                                          TotalOrders = movementGroup.Select(x => x.DocumentNumber).Distinct().Count(),
                                          TotalItems = movementGroup.Select(x => x.Item.Id).Distinct().Count(),
                                          TotalLPNs = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count(),
                                          TotalQty = movementGroup.Sum(x => x.ItemQtyMov),
                                          OrdersAvg = movementGroup.Select(x => x.DocumentNumber).Distinct().Count() / range,
                                          LinesAvg = movementGroup.Select(x => x.DocumentLineNumber).Count() / range,
                                          LpnsAvg = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count() / range,
                                          QtyItemsAvg = movementGroup.Sum(x => x.ItemQtyMov) / range
                                      };
                        lista = mvmtDAY.ToList();
                        break;
                    case "WEEK":
                        
                        var mvmtWEEK = from movement in movementWebViewDTO_aux.Entities
                                       group movement by new
                                       {
                                           idWhs = movement.Warehouse.Id,
                                           whsCode = movement.Warehouse.Code,
                                           whsName = movement.Warehouse.Name,
                                           IdOwn = movement.Owner.Id,
                                           ownCode = movement.Owner.Code,
                                           ownName = movement.Owner.Name,
                                           movementName = movement.MovementType.Name,
                                           //userName = movement.UserName,
                                           period = "Semana N°" + cal.GetWeekOfYear(movement.EndTime.Date, System.Globalization.CalendarWeekRule.FirstDay, System.DayOfWeek.Sunday).ToString()

                                       } into movementGroup
                                       select new MovementUserProductivityStruct
                                       {
                                           IdWhs = movementGroup.Key.idWhs,
                                           WhsCode = movementGroup.Key.whsCode,
                                           WhsName = movementGroup.Key.whsName,
                                           IdOwn = movementGroup.Key.IdOwn,
                                           OwnCode = movementGroup.Key.ownCode,
                                           OwnName = movementGroup.Key.ownName,
                                           MovementName = movementGroup.Key.movementName,
                                           //UserName = movementGroup.Key.userName,
                                           Period = movementGroup.Key.period.ToString(),
                                           TotalOrders = movementGroup.Select(x => x.DocumentNumber).Distinct().Count(),
                                           TotalItems = movementGroup.Select(x => x.Item.Id).Distinct().Count(),
                                           TotalLPNs = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count(),
                                           TotalQty = movementGroup.Sum(x => x.ItemQtyMov),
                                           OrdersAvg = movementGroup.Select(x => x.DocumentNumber).Distinct().Count() / range,
                                           LinesAvg = movementGroup.Select(x => x.DocumentLineNumber).Count() / range,
                                           LpnsAvg = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count() / range,
                                           QtyItemsAvg = movementGroup.Sum(x => x.ItemQtyMov) / range
                                       };
                        lista = mvmtWEEK.OrderBy(x => Convert.ToInt32(x.Period.Substring(x.Period.LastIndexOf('°') + 1))).ToList();
                        break;
                    case "MONTH":

                        var mvmtMONTH = from movement in movementWebViewDTO_aux.Entities
                                        group movement by new
                                        {
                                            idWhs = movement.Warehouse.Id,
                                            whsCode = movement.Warehouse.Code,
                                            whsName = movement.Warehouse.Name,
                                            IdOwn = movement.Owner.Id,
                                            ownCode = movement.Owner.Code,
                                            ownName = movement.Owner.Name,
                                            movementName = movement.MovementType.Name,
                                            //userName = movement.UserName,
                                            period = movement.EndTime.ToString("MMMM").ToUpper()
                                        } into movementGroup
                                        select new MovementUserProductivityStruct
                                        {
                                            IdWhs = movementGroup.Key.idWhs,
                                            WhsCode = movementGroup.Key.whsCode,
                                            WhsName = movementGroup.Key.whsName,
                                            IdOwn = movementGroup.Key.IdOwn,
                                            OwnCode = movementGroup.Key.ownCode,
                                            OwnName = movementGroup.Key.ownName,
                                            MovementName = movementGroup.Key.movementName,
                                            //UserName = movementGroup.Key.userName,
                                            Period = movementGroup.Key.period.ToString(),
                                            TotalOrders = movementGroup.Select(x => x.DocumentNumber).Distinct().Count(),
                                            TotalItems = movementGroup.Select(x => x.Item.Id).Distinct().Count(),
                                            TotalLPNs = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count(),
                                            TotalQty = movementGroup.Sum(x => x.ItemQtyMov),
                                            OrdersAvg = movementGroup.Select(x => x.DocumentNumber).Distinct().Count() / range,
                                            LinesAvg = movementGroup.Select(x => x.DocumentLineNumber).Count() / range,
                                            LpnsAvg = movementGroup.Select(x => x.IdLpnCodeTarget).Distinct().Count() / range,
                                            QtyItemsAvg = movementGroup.Sum(x => x.ItemQtyMov) / range
                                        };
                        lista = mvmtMONTH.ToList();
                        break;
                }
            }
            return lista;
        }

        private int CalculateRangeFromDates(string period, List<MovementWeb> data)
        {
            int range = 0;

            var filterDateRange = Master.ucMainFilter.MainFilter.Where(f => f.Name == "DateRange").FirstOrDefault();

            var selectedDaysInRange = SelectedFilterDate(filterDateRange);

            var startDate = GetStartDate(selectedDaysInRange, data, filterDateRange);
            var endDate = GetEndDate(selectedDaysInRange, data, filterDateRange);

            switch (period)
            {
                case "DAY":
                    range = CalculateOnlyWeekDays(startDate, endDate);
                    break;
                case "WEEK": 
                    range = CalculateWeeksInDateRange(startDate, endDate);
                    break;
                case "MONTH": 
                    range = CalculateMonthsInDateRange(startDate, endDate);
                    break;
            }

            return range;
        }

        private int CalculateOnlyWeekDays(DateTime start, DateTime end)
        {
            int cont = 0;

            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    cont++;
                }
            }

            return cont;
        }

        private int CalculateMonthsInDateRange(DateTime startDate, DateTime endDate)
        {
            return ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
        }

        private int CalculateWeeksInDateRange(DateTime startDate, DateTime endDate)
        {
            var daysSinceMonday = ((int)startDate.DayOfWeek + 6) % 7;
            return ((endDate - startDate).Days + daysSinceMonday) / 7;
        }

        private DateTime GetStartDate(eSelectedFilterDate selectedDaysInRange, List<MovementWeb> data, EntityFilter filterDateRange)
        {
            DateTime startDate = DateTime.MinValue;

            if (selectedDaysInRange == eSelectedFilterDate.Both || selectedDaysInRange == eSelectedFilterDate.OnlyStart)
                DateTime.TryParseExact(filterDateRange.FilterValues[0].Value, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out startDate);
            else
                startDate = data.Select(d => d.EndTime).Min();

            return startDate;
        }

        private DateTime GetEndDate(eSelectedFilterDate selectedDaysInRange, List<MovementWeb> data, EntityFilter filterDateRange)
        {
            DateTime endDate = DateTime.MinValue;

            if (selectedDaysInRange == eSelectedFilterDate.Both || selectedDaysInRange == eSelectedFilterDate.OnlyEnd)
                DateTime.TryParseExact(filterDateRange.FilterValues[1].Value, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out endDate);
            else
                endDate = data.Select(d => d.EndTime).Max();

            return endDate;
        }        

        private eSelectedFilterDate SelectedFilterDate(EntityFilter filterDateRange)
        {
            eSelectedFilterDate? selected = null;

            if (filterDateRange.FilterValues.Count == 0)
            {
                selected = eSelectedFilterDate.None;
            }
            else if (filterDateRange.FilterValues.Count == 2 && string.IsNullOrEmpty(filterDateRange.FilterValues[0].Value) && !string.IsNullOrEmpty(filterDateRange.FilterValues[1].Value))
            {
                selected = eSelectedFilterDate.OnlyEnd;
            }
            else if (filterDateRange.FilterValues.Count == 2 && !string.IsNullOrEmpty(filterDateRange.FilterValues[0].Value) && string.IsNullOrEmpty(filterDateRange.FilterValues[1].Value))
            {
                selected = eSelectedFilterDate.OnlyStart;
            }
            else if (filterDateRange.FilterValues.Count == 2 && !string.IsNullOrEmpty(filterDateRange.FilterValues[0].Value) && !string.IsNullOrEmpty(filterDateRange.FilterValues[1].Value))
            {
                selected = eSelectedFilterDate.Both;
            }

            return (eSelectedFilterDate)selected;
        }

        private enum eSelectedFilterDate
        {
            None,
            OnlyStart,
            OnlyEnd,
            Both
        }

        #endregion
    }
}
