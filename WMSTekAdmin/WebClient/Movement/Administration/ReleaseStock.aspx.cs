using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Utils;

namespace Binaria.WMSTek.WebClient.Movement.Administration
{
    public partial class ReleaseStock : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> stockViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
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

        #endregion

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
                        // Carga inicial del ViewDTO
                        //UpdateSession(false);
                        isValidViewDTO = true;
                        if (isValidViewDTO)
                        {
                            PopulateLists();
                        }
                    }

                    if (ValidateSession(WMSTekSessions.StockConsult.ReleaseStockList))
                    {
                        stockViewDTO = (GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>)Session[WMSTekSessions.StockConsult.ReleaseStockList];
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
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "SetDivs();", true);
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
                ReloadData();
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
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

        //protected void ddlEstado_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //   //Casting sender to Dropdown
        //   DropDownList ddl = sender as DropDownList;
        //   //Looping through each Gridview row to find exact Row 
        //   //of the Grid from where the SelectedIndex change event is fired.
        //   foreach (GridViewRow row in grdMgr.Rows)
        //   {
        //      //Finding Dropdown control  
        //      Control ctrl = row.FindControl("ddlTest") as DropDownList;
        //        if (ctrl != null)
        //        {
        //            DropDownList ddl1 = (DropDownList)ctrl;
        //            //Comparing ClientID of the dropdown with sender
        //            if (ddl.ClientID == ddl1.ClientID)
        //            {
        //                //ClientID is match so find the Textbox 
        //                //control bind it with some dropdown data.
        //                TextBox txt = row.FindControl("txtTest") as TextBox;
        //                txt.Text = ddl1.SelectedValue;
        //                break;
        //            }
        //        }
        //   }
        //}

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        public void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            //PopulateLists();
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(stockViewDTO.Errors);
                stockViewDTO.ClearError();
            }

            this.Master.ucMainFilter.ClearFilterObject();
            this.Master.ucMainFilter.LoadControlValuesToFilterObject();

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            stockViewDTO = iWarehousingMGR.GetStockWithHoldCodeByFilters(context);

            if (!stockViewDTO.hasError() && stockViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.StockConsult.ReleaseStockList, stockViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(stockViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            base.LoadReasonFilterByTypeInOut(this.ddlHoldCode,TypeInOut.QualityControl, true, this.lblReleaseStock.Text.Trim());
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!stockViewDTO.hasConfigurationError() && stockViewDTO.Configuration != null && stockViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, stockViewDTO.Configuration);

            grdMgr.DataSource = stockViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(stockViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar            
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.reasonVisible = true;
            this.Master.ucMainFilter.reasonFilterWithTypeInOut = TypeInOut.QualityControl;
            this.Master.ucMainFilter.includeReasonAvailableVisible = true;
            this.Master.ucMainFilter.codeLabel = this.lblFilterNameLpn.Text;
            this.Master.ucMainFilter.nameLabel = this.lblFilterNameLot.Text;
            this.Master.ucMainFilter.nameReasonLabel = this.lblFilterNameReasonCode.Text;

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

            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            Master.ucTaskBar.btnExcelVisible = true;
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

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        protected void SaveChanges()
        {
            Binaria.WMSTek.Framework.Entities.Warehousing.Stock stock = new Binaria.WMSTek.Framework.Entities.Warehousing.Stock();
            bool validateBackOrder = false;
            List<String> theListLocTypeAllowBackOrder = new List<String>();

            stock.Id = (Convert.ToInt32(hidEditId.Value));

            if (this.ddlHoldCode.SelectedValue != "-1")
            {
                var theStockDTO = iWarehousingMGR.GetStockById(stock.Id, context);
                stock = theStockDTO.Entities.FirstOrDefault();

                stock.Hold = this.ddlHoldCode.SelectedValue;
                stock.Reason = this.ddlHoldCode.SelectedValue;
            }
            else
            {
                Location theLocation = new Location();
                GenericViewDTO<Location> theLocationDTO = new GenericViewDTO<Location>();
                GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> theStockDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();

                theStockDTO = iWarehousingMGR.GetStockById(stock.Id, context);
                theLocationDTO = iLayoutMGR.GetLocationByIdAndWhs(this.txtLocation.Text, theStockDTO.Entities[0].Warehouse.Id, context);
                
                theListLocTypeAllowBackOrder = GetConst("LocationTypeAllowBackOrder");

                foreach (String theLocTypeAllowBackOrder in theListLocTypeAllowBackOrder)
                {
                    if (theLocTypeAllowBackOrder.Equals(theLocationDTO.Entities[0].Type.LocTypeCode))
                    {
                        validateBackOrder = true;
                    }
                }
                stock = theStockDTO.Entities[0];
                stock.Hold = null;
                stock.Reason = null;
            }

            List<string> lst1 = GetConst("WarehouseStockAvailable");
            List<string> lst2 = GetConst("WarehouseStockLocked");
            string warehouseStockAvailable = lst1 != null && lst1.Count > 0 ? lst1[0].Trim() : string.Empty;
            string warehouseStockLocked = lst2 != null && lst2.Count > 0 ? lst2[0].Trim() : string.Empty;

            stockViewDTO = iWarehousingMGR.HoldStock(stock, validateBackOrder, warehouseStockAvailable, warehouseStockLocked, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (stockViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(stockViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }


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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

                ShowModal(editIndex);
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }
        protected void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //DropDownList ddlEstado = e.Row.FindControl("ddlEstado") as DropDownList;
                //base.LoadReasonLessFilter(ddlEstado, true, this.lblReleaseStock.Text.Trim());

                //Label x = e.Row.FindControl("lblEstado") as Label;
                //ddlEstado.SelectedValue = x.Text;
            }

            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void ShowModal(int index)
        {
            //Recupera los datos de la entidad a editar
            hidEditId.Value = stockViewDTO.Entities[index].Id.ToString();

            this.txtId.Text = stockViewDTO.Entities[index].Id.ToString();
            this.txtWarehouseCode.Text = stockViewDTO.Entities[index].Warehouse.ShortName;
            this.txtOwnerCode.Text = stockViewDTO.Entities[index].Owner.Name;
            this.txtIdLpnCode.Text = stockViewDTO.Entities[index].Lpn.Code;
            this.txtLotNumber.Text = stockViewDTO.Entities[index].Lot;
            this.txtLocation.Text = stockViewDTO.Entities[index].Location.IdCode;
            this.txtItemCode.Text = stockViewDTO.Entities[index].Item.Code;
            this.ddlHoldCode.SelectedValue = stockViewDTO.Entities[index].Hold.ToString();

            this.txtId.ReadOnly = true;
            this.txtWarehouseCode.ReadOnly = true;
            this.txtOwnerCode.ReadOnly = true;
            this.txtIdLpnCode.ReadOnly = true;
            this.txtLotNumber.ReadOnly = true;
            this.txtLocation.ReadOnly = true;
            this.txtItemCode.ReadOnly = true;

            if (stockViewDTO.Configuration != null && stockViewDTO.Configuration.Count > 0)
            {
                base.ConfigureModal(stockViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }
    }
}
