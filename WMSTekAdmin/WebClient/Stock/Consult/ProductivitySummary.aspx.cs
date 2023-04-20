using System;
using System.Data.SqlTypes;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Stocks
{
    public partial class ProductionSummary : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<ProductivitySummary> productivitySummaryViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> toReceptionViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> receptionedViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> toCollectViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> collectingViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> collectedViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> toPackViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> packingViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> packedViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> dispatchViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> loadViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskPendingCycleCountViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskPendingAdjustViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskPendingReplenishViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskCompletedCycleCountViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskCompletedAdjustViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskCompletedReplenishViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> releaseOrderViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> transferViewDTO = new GenericViewDTO<ProductivitySummary>();
        private bool isValidViewDTO = false;
        private string filterMov = string.Empty;
        private string entityNameProperty = string.Empty;
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
                productivitySummaryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(productivitySummaryViewDTO.Errors);
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
            }
            catch (Exception ex)
            {
                productivitySummaryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(productivitySummaryViewDTO.Errors);
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
                productivitySummaryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(productivitySummaryViewDTO.Errors);
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
                productivitySummaryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(productivitySummaryViewDTO.Errors);
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
                productivitySummaryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(productivitySummaryViewDTO.Errors);
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
                productivitySummaryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(productivitySummaryViewDTO.Errors);
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
                productivitySummaryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(productivitySummaryViewDTO.Errors);
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
                productivitySummaryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(productivitySummaryViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                //currentPage = grdMgr.PageCount - 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                productivitySummaryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(productivitySummaryViewDTO.Errors);
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
        //        movementViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(movementViewDTO.Errors);
        //    }
        //}

        #endregion

        #region "Métodos"


        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.StockConsult.ProductivitySummaryList))
                {
                    productivitySummaryViewDTO = (GenericViewDTO<ProductivitySummary>)Session[WMSTekSessions.StockConsult.ProductivitySummaryList];
                    isValidViewDTO = true;
                }
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
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
                                    
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);

            entityNameProperty = "ProductivitySummary_FindAll";
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            //this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            //this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            //this.Master.ucTaskBar.btnRefreshVisible = true;
            //this.Master.ucTaskBar.btnExcelVisible = true;
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
            //grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            //grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            // carga todas las recepciones
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            toReceptionViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_ToReception");
            receptionedViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_Receptioned");
            toCollectViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_ToCollect");
            collectingViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_Collecting");
            collectedViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_Collected");
            toPackViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_ToPack");
            packingViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_Packing");
            packedViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_Packed");
            dispatchViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_Dispatch");
            loadViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_Load");
            taskPendingCycleCountViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_TaskPending_CycleCount");
            taskPendingAdjustViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_TaskPending_Adjust");
            taskPendingReplenishViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_TaskPending_Replenish");
            taskCompletedCycleCountViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_TaskCompleted_CycleCount");
            taskCompletedAdjustViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_TaskCompleted_Adjust");
            taskCompletedReplenishViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_TaskCompleted_Replenish");
            releaseOrderViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_ReleaseOrder");
            transferViewDTO = iWarehousingMGR.Find_ProductivitySummary_byQueryName(context, "ProductivitySummary_Transfer");

            if (!productivitySummaryViewDTO.hasError() && productivitySummaryViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.StockConsult.ProductivitySummaryList, productivitySummaryViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(productivitySummaryViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(productivitySummaryViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            if (toReceptionViewDTO.Entities.Count > 0)
            {
                txtToReceptionDoc.Text = toReceptionViewDTO.Entities[0].Documents.ToString();
                txtToReceptionLine.Text = toReceptionViewDTO.Entities[0].Lines.ToString();
                txtToReceptionQty.Text = toReceptionViewDTO.Entities[0].Qty.ToString();
                txtReceptionedDoc.Text = receptionedViewDTO.Entities[0].Documents.ToString();
                txtReceptionedLine.Text = receptionedViewDTO.Entities[0].Lines.ToString();
                txtReceptionedQty.Text = receptionedViewDTO.Entities[0].Qty.ToString();

                txtDispatchDoc.Text = dispatchViewDTO.Entities[0].Documents.ToString();
                txtDispatchLine.Text = dispatchViewDTO.Entities[0].Lines.ToString();
                txtDispatchQty.Text = dispatchViewDTO.Entities[0].Qty.ToString();
                txtDispatchLpn.Text = dispatchViewDTO.Entities[0].Lpn.ToString();
                txtLoadDoc.Text = loadViewDTO.Entities[0].Documents.ToString();
                txtLoadLine.Text = loadViewDTO.Entities[0].Lines.ToString();
                txtLoadQty.Text = loadViewDTO.Entities[0].Qty.ToString();
                txtLoadLpn.Text = loadViewDTO.Entities[0].Lpn.ToString();

                txtToCollectDoc.Text = toCollectViewDTO.Entities[0].Documents.ToString();
                txtToCollectLine.Text = toCollectViewDTO.Entities[0].Lines.ToString();
                txtToCollectQty.Text = toCollectViewDTO.Entities[0].Qty.ToString();
                txtCollectingDoc.Text = collectingViewDTO.Entities[0].Documents.ToString();
                txtCollectingLine.Text = collectingViewDTO.Entities[0].Lines.ToString();
                txtCollectingQty.Text = collectingViewDTO.Entities[0].Qty.ToString();
                txtCollectedDoc.Text = collectedViewDTO.Entities[0].Documents.ToString();
                txtCollectedLine.Text = collectedViewDTO.Entities[0].Lines.ToString();
                txtCollectedQty.Text = collectedViewDTO.Entities[0].Qty.ToString();
                txtCollectUser.Text = collectingViewDTO.Entities[0].Users.ToString();

                txtCycleCountPending.Text = taskPendingCycleCountViewDTO.Entities[0].Documents.ToString();
                txtCycleCountComplete.Text = taskCompletedCycleCountViewDTO.Entities[0].Documents.ToString();
                txtAdjustPending.Text = taskPendingAdjustViewDTO.Entities[0].Documents.ToString();
                txtAdjustComplete.Text = taskCompletedAdjustViewDTO.Entities[0].Documents.ToString();
                txtReplenishPending.Text = taskPendingReplenishViewDTO.Entities[0].Documents.ToString();
                txtReplenishComplete.Text = taskCompletedReplenishViewDTO.Entities[0].Documents.ToString();

                txtToPackDoc.Text = toPackViewDTO.Entities[0].Documents.ToString();
                txtToPackLine.Text = toPackViewDTO.Entities[0].Lines.ToString();
                txtToPackQty.Text = toPackViewDTO.Entities[0].Qty.ToString();
                txtToPackLpn.Text = toPackViewDTO.Entities[0].Lpn.ToString();
                txtPackingDoc.Text = packingViewDTO.Entities[0].Documents.ToString();
                txtPackingLine.Text = packingViewDTO.Entities[0].Lines.ToString();
                txtPackingQty.Text = packingViewDTO.Entities[0].Qty.ToString();
                txtPackingUser.Text = packingViewDTO.Entities[0].Users.ToString();
                txtPackedDoc.Text = packedViewDTO.Entities[0].Documents.ToString();
                txtPackedLine.Text = packedViewDTO.Entities[0].Lines.ToString();
                txtPackedQty.Text = packedViewDTO.Entities[0].Qty.ToString();
                txtPackedLpn.Text = packedViewDTO.Entities[0].Lpn.ToString();

                txtTransferDoc.Text = transferViewDTO.Entities[0].Documents.ToString();
                txtTransferQty.Text = transferViewDTO.Entities[0].Qty.ToString();
                txtTransferWeight.Text = transferViewDTO.Entities[0].Weight.ToString();
                txtTransferVolume.Text = transferViewDTO.Entities[0].Volume.ToString();
                txtReleaseOrderDoc.Text = releaseOrderViewDTO.Entities[0].Documents.ToString();
                txtReleaseOrderQty.Text = releaseOrderViewDTO.Entities[0].Qty.ToString();
                txtReleaseOrderWeight.Text = releaseOrderViewDTO.Entities[0].Weight.ToString();
                txtReleaseOrderVolume.Text = releaseOrderViewDTO.Entities[0].Volume.ToString();
                txtTotalDoc.Text = (transferViewDTO.Entities[0].Documents + releaseOrderViewDTO.Entities[0].Documents).ToString();
                txtTotalQty.Text = (transferViewDTO.Entities[0].Qty + releaseOrderViewDTO.Entities[0].Qty).ToString();
                txtTotalWeight.Text = (transferViewDTO.Entities[0].Weight + releaseOrderViewDTO.Entities[0].Weight).ToString();
                txtTotalVolume.Text = (transferViewDTO.Entities[0].Volume + releaseOrderViewDTO.Entities[0].Volume).ToString();
            }
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        #endregion
    }
}
