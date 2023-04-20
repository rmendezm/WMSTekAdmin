using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Movement.Consult
{
    public partial class ItemRotation : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<MovementWeb> movementViewDTO = new GenericViewDTO<MovementWeb>();
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

        #region Events

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                if (base.webMode == WebMode.Normal)
                {
                    if (isValidViewDTO)
                    {
                        PopulateGrid();
                    }
                }

                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
        }


        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.movementTypeVisible = true;

            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;

            this.Master.ucMainFilter.DateBefore = CfgParameterName.LogDaysBeforeQuery;//dias antes
            this.Master.ucMainFilter.DateAfter = CfgParameterName.LogDaysAfterQuery;//dias despues

            this.Master.ucMainFilter.chkDateToEnabled = false;
            this.Master.ucMainFilter.chkDateFromEnabled = false;

            this.Master.ucMainFilter.divRotationItemFilterVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);

            movementViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(movementViewDTO.MessageStatus.Message);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void PopulateGrid()
        {
            movementViewDTO = (GenericViewDTO<MovementWeb>)Session[WMSTekSessions.RotationItemMgr.RotationItemList];

            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            //if (!countryViewDTO.hasConfigurationError() && countryViewDTO.Configuration != null && countryViewDTO.Configuration.Count > 0)
            //    base.ConfigureGridOrder(grdMgr, countryViewDTO.Configuration);

            grdMgr.DataSource = movementViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(movementViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession(false);

            if (isValidViewDTO)
            {
                currentIndex = -1;
                currentPage = 0;
            }
        }

        private void UpdateSession(bool showError)
        {
            if (showError)
            {
                this.Master.ucError.ShowError(movementViewDTO.Errors);
                movementViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            //var filterItem = context.MainFilter.Where(f => f.Name == "Item").FirstOrDefault();

            //if (filterItem != null && filterItem.FilterValues.Count > 0)
            //{
            //    var itemCode = filterItem.FilterValues.FirstOrDefault().Value;

            //    if (!string.IsNullOrEmpty(itemCode))
            //    {
            //        var filterOwner = context.MainFilter.Where(f => f.Name == "Owner").FirstOrDefault();
            //        var idOwn = int.Parse(filterOwner.FilterValues.First().Value);

            //        var itemViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, itemCode, idOwn, true);

            //        if (!itemViewDTO.hasError() && itemViewDTO.Entities.Count > 0)
            //        {
            //            filterItem.FilterValues.First().Value = itemViewDTO.Entities.First().Id.ToString();
            //        }
            //    }
            //}

            var filterTypeRotation = context.MainFilter.Where(f => f.Name == Master.ucMainFilter.ID_ROTATION_TYPE_FILTER).FirstOrDefault();

            if (filterTypeRotation != null)
            {
                var filterTypeRotationSelected = filterTypeRotation.FilterValues.FirstOrDefault().Value;

                if (filterTypeRotationSelected == "item")
                {
                    movementViewDTO = iWarehousingMGR.FindRotationItem(context);
                }
                else if (filterTypeRotationSelected == "location")
                {
                    movementViewDTO = iWarehousingMGR.FindRotationLocation(context);
                }
            }

            if (!movementViewDTO.hasError() && movementViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.RotationItemMgr.RotationItemList, movementViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(movementViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }

            upGrid.Update();
            CallJsGridView();
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "afterAsyncPostBack();", true);
        }

        #endregion
    }
}