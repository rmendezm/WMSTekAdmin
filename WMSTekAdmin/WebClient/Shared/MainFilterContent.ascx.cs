using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Configuration;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.Collections;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class MainFilterContent : BaseUserControl
    {
        #region "Declaración de Variables"

        public event EventHandler BtnSearchMap2DNewClick;
        public event EventHandler BtnCleanClick;
        public event EventHandler BtnSearchClick;
        public event EventHandler BtnSearchClickAux;
        public event EventHandler BtnPrintClick;
        public event EventHandler ddlWareHouseIndexChanged;
        public event EventHandler ddlHangarIndexChanged;
        public event EventHandler ddlOwnerIndexChanged;
        public event EventHandler chkPendinOrders;
        public event EventHandler ddlLogicalWareHouseIndexChanged;

        private GenericViewDTO<Vendor> vendorSearchViewDTO;
        private CfgParameterName dateBefore;
        private CfgParameterName dateAfter;
        public string locationFilterType = string.Empty;

        public bool warehouseNotIncludeAll;
        public bool ownerNotIncludeAll;
        public bool wmsProcessTypeEnabled = false;
        public bool setDateLabel;
        public bool taskTypeCodeEnabled = false;
        public bool ownerIncludeNulls;
        private bool chkDisabledAndChequed;
        public bool taskTypeNotIncludeAll;
        public bool trackTaskTypeNotIncludeAll;
        public bool outboundTypeNotIncludeAll;
        public bool outboundTypeAll;
        public bool trackOutboundTypeNotIncludeAll;
        public bool trackInboundTypeNotIncludeAll;
        public bool dispatchTypeNotIncludeAll;
        public bool taskQueueFilterSimulation = false;
        private bool isValidFilterLocation = false;
        private bool setDefaultOwner;
        public bool includeReasonAvailableVisible = false;
        public bool logicalWarehouseNotIncludeAll = false;

        public List<String> listTrackOutboundType;
        public List<String> listTrackInboundType;
        public List<String> listWmsProcessType;
        public List<String> listOutboundType;
        public List<String> listInboundType;
        public List<String> listTaskTypeCode;
        public List<String> listDispatchType;
        private List<EntityFilter> mainFilter;

        public string[] outboundTypeCode;
        public string[] trackTaskTypeCode;
        public TypeInOut reasonFilterWithTypeInOut;
        public int? idModule;

        public string ID_COUNTRY
        {
            get
            {
                return "IdCountry";
            }
        }

        public string ID_STATE
        {
            get
            {
                return "IdState";
            }
        }

        public string ID_CITY
        {
            get
            {
                return "IdCity";
            }
        }

        public string ID_TRACK_TASK_QUEUE
        {
            get
            {
                return "IdTrackTaskQueue";
            }
        }

        public string ID_ROTATION_TYPE_FILTER
        {
            get
            {
                return "IdRotationTypeFilter";
            }
        }

        public string CODE_ALT
        {
            get
            {
                return "CodeAlt";
            }
        }

        public string LOCKED_STOCK
        {
            get
            {
                return "LockedStock";
            }
        }

        public string LOCKED_LOCATION
        {
            get
            {
                return "LockedLocation";
            }
        }

        public string LOCKED_LOCATION_FILTER
        {
            get
            {
                return "LockedLocationFilter";
            }
        }

        public string PERCENT_RANGE
        {
            get
            {
                return "PercentRange";
            }
        }

        public int currentPageItemsControl
        {
            get
            {
                if (ViewState["pageItem"] != null && ViewState["pageItem"].ToString() != string.Empty)
                    return (int)ViewState["pageItem"];
                else
                    return 0;
            }

            set { ViewState["pageItem"] = value; }
        }

        public int idGrpItem1
        {
            get 
            {
                if (this.ddlGrpItem1.Items.Count > 0)
                    return Convert.ToInt32(this.ddlGrpItem1.SelectedValue);
                else
                    return -1;
            }
        }

        public int idGrpItem2
        {
            get
            {
                if (this.ddlGrpItem2.Items.Count > 0)
                    return Convert.ToInt32(this.ddlGrpItem2.SelectedValue);
                else
                    return -1;
            }
        }

        public int idGrpItem3
        {
            get
            {
                if (this.ddlGrpItem3.Items.Count > 0)
                    return Convert.ToInt32(this.ddlGrpItem3.SelectedValue);
                else
                    return -1;
            }
        }

        public int idGrpItem4
        {
            get
            {
                if (this.ddlGrpItem4.Items.Count > 0)
                    return Convert.ToInt32(this.ddlGrpItem4.SelectedValue);
                else
                    return -1;
            }
        }

        public int idBscGrpItem1
        {
            get
            {
                if (this.ddlBscGrpItm1.Items.Count > 0)
                    return Convert.ToInt32(this.ddlBscGrpItm1.SelectedValue);
                else
                    return -1;
            }
        }

        public int idBscGrpItem2
        {
            get
            {
                if (this.ddlBscGrpItm2.Items.Count > 0)
                    return Convert.ToInt32(this.ddlBscGrpItm2.SelectedValue);
                else
                    return -1;
            }
        }

        public int idBscGrpItem3
        {
            get
            {
                if (this.ddlBscGrpItm3.Items.Count > 0)
                    return Convert.ToInt32(this.ddlBscGrpItm3.SelectedValue);
                else
                    return -1;
            }
        }

        public int idBscGrpItem4
        {
            get
            {
                if (this.ddlBscGrpItm4.Items.Count > 0)
                    return Convert.ToInt32(this.ddlBscGrpItm4.SelectedValue);
                else
                    return -1;
            }
        }

        public bool divBasicItemGroupVisible
        {
            get { return this.divBscGroupItems.Visible; }
            set { this.divBscGroupItems.Visible = value; }
        }

        private int idCountryFilter
        {
            get
            {
                if (this.ddlCountryFilter.Items.Count > 0)
                    return Convert.ToInt32(this.ddlCountryFilter.SelectedValue);
                else
                    return -1;
            }
        }

        private int idStateFilter
        {
            get
            {
                if (this.ddlStateFilter.Items.Count > 0)
                    return Convert.ToInt32(this.ddlStateFilter.SelectedValue);
                else
                    return -1;
            }
        }

        private int idCityFilter
        {
            get
            {
                if (this.ddlCityFilter.Items.Count > 0)
                    return Convert.ToInt32(this.ddlCityFilter.SelectedValue);
                else
                    return -1;
            }
        }

        private int idCountry
        {
            get
            {
                if (this.ddlCountry.Items.Count > 0)
                    return Convert.ToInt32(this.ddlCountry.SelectedValue);
                else
                    return -1;
            }
        }

        private int idState
        {
            get
            {
                if (this.ddlState.Items.Count > 0)
                    return Convert.ToInt32(this.ddlState.SelectedValue);
                else
                    return -1;
            }
        }

        private int idCity
        {
            get
            {
                if (this.ddlCity.Items.Count > 0)
                    return Convert.ToInt32(this.ddlCity.SelectedValue);
                else
                    return -1;
            }
        }

        public int idTaskQueueFilter
        {
            get
            {
                if (this.ddlTaskQueueFilter.Items.Count > 0)
                    return Convert.ToInt32(this.ddlTaskQueueFilter.SelectedValue);
                else
                    return -1;
            }
        }

        public List<EntityFilter> MainFilter
        {
            get { return mainFilter; }
            set { mainFilter = value; }
        }

        public int idWhs
        {
            get { return Convert.ToInt32(this.ddlFilterWarehouse.SelectedValue); }
            set { this.ddlFilterWarehouse.SelectedIndex = value; }
        }
        public int selectIndexWhs
        {
            get { return this.ddlFilterWarehouse.SelectedIndex; }
        }
        public int selectIndexHangar
        {
            get { return this.ddlFilterHangar.SelectedIndex; }
        }

        public DropDownList ddlFilterHangarObject
        {
            get { return ddlFilterHangar; }
            set { ddlFilterHangar = value; }
        }

        public int idOwn
        {
            get { return string.IsNullOrEmpty(this.ddlFilterOwner.SelectedValue) ? -1 : Convert.ToInt32(this.ddlFilterOwner.SelectedValue); }
        }

        public ListItemCollection listItemOwners
        {
            get { return this.ddlFilterOwner.Items; }
        }

        public int idHangar
        {
            get { return Convert.ToInt32(this.ddlFilterHangar.SelectedValue); }
        }

        public string idTruckCode
        {
            get { return this.txtFilterIdTruckCode.Text; }
        }

        public string valuePeriod
        {
            get { return Convert.ToString(this.ddlPeriodUserProductivity.SelectedValue); }
        }

        public string valueView
        {
            get { return Convert.ToString(this.ddlViewUserProductivity.SelectedValue); }
        }

        public string idWmsProcessType
        {
            get { return this.ddlFilterWmsProcess.SelectedValue.Trim(); }
        }

        public ListItemCollection listItemWmsProcessType
        {
            get { return this.ddlFilterWmsProcess.Items; }
        }

        public int hangarIndex
        {
            get { return this.ddlFilterHangar.SelectedIndex; }
        }

        public ListItemCollection hangarItems
        {
            get { return this.ddlFilterHangar.Items; }
        }

        public ListItemCollection warehouseItems
        {
            get { return this.ddlFilterWarehouse.Items; }
        }

        public string itemCode
        {
            get { return this.txtFilterItem.Text; }
        }

        public string itemName
        {
            get { return this.txtItemName.Text; }
        }
                        
        public bool searchVisible
        {
            get { return this.btnSearch.Visible; }
            set { this.btnSearch.Visible = value; }
        }
        public bool searchAuxVisible
        {
            get { return this.btnSearchAux.Visible; }
            set { this.btnSearchAux.Visible = value; }
        }
        public bool searchMap2DVisible
        {
            get { return this.btnSearchMap2D.Visible; }
            set { this.btnSearchMap2D.Visible = value; }
        }
        public bool searchMap2DNewVisible
        {
            get { return this.btnSearchMap2DNew.Visible; }
            set { this.btnSearchMap2DNew.Visible = value; }
        }

        public bool cleanMapVisible
        {
            get { return this.btnCleanMap.Visible; }
            set { this.btnCleanMap.Visible = value; }
        }

        public bool divLocationsMostUsedByItemVisible
        {
            get { return this.divLocationsMostUsedByItem.Visible; }
            set { this.divLocationsMostUsedByItem.Visible = value; }
        }

        public bool codeVisible
        {
            get { return this.divFilterCode.Visible; }
            set { this.divFilterCode.Visible = value; }
        }

        public bool codeAltVisible
        {
            get { return this.divFilterCodeAlt.Visible; }
            set { this.divFilterCodeAlt.Visible = value; }
        }

        public bool codeNumericVisible
        {
            get { return this.divFilterCodeNumeric.Visible; }
            set { this.divFilterCodeNumeric.Visible = value; }
        }

        public bool nameVisible
        {
            get { return this.divFilterName.Visible; }
            set { this.divFilterName.Visible = value; }
        }

        public bool descriptionVisible
        {
            get { return this.divFilterDescription.Visible; }
            set { this.divFilterDescription.Visible = value; }
        }

        public bool uomTypeVisible
        {
            get { return this.divFilterUomType.Visible; }
            set { this.divFilterUomType.Visible = value; }
        }

        public bool impressionTailVisible
        {
            get { return this.divImpressionTail.Visible; }
            set { this.divImpressionTail.Visible = value; }
        }



        public bool warehouseVisible
        {
            get { return this.divFilterWarehouse.Visible; }
            set { this.divFilterWarehouse.Visible = value; }
        }

        public bool warehouseDisabled
        {
            get { return this.divFilterWarehouse.Disabled; }
            set { this.divFilterWarehouse.Disabled = value; }
        }

        public bool ownerVisible
        {
            get { return this.divFilterOwner.Visible; }
            set { this.divFilterOwner.Visible = value; }
        }

        public bool printVisible
        {
            get
            {
                return this.divPrint.Visible;
            }
            set
            {
                this.divPrint.Visible = value;
                imbPrint.Enabled = false;
            }
        }

        

        public bool printEnable
        {
            get
            {
                return this.imbPrint.Enabled;
            }
            set
            {
                imbPrint.Enabled = value;
            }
        }

        public int? IdModule
        {
            get { return this.idModule; }
            set { this.idModule = value; }
        }

        public bool WmsProcessTypeVisible
        {
            get { return this.divWmsProcess.Visible; }
            set { this.divWmsProcess.Visible = value; }
        }

        public bool WmsProcessTypeEnabled
        {
            get { return this.wmsProcessTypeEnabled; }
            set { this.wmsProcessTypeEnabled = value; }
        }
        
        public bool TaskTypeCodeEnabled
        {
            get { return this.taskTypeCodeEnabled; }
            set { this.taskTypeCodeEnabled = value; }
        }
        
        public bool ChkDisabledAndChequed
        {
          get { return chkDisabledAndChequed; }
          set { chkDisabledAndChequed = value; }
        }

        public void ActivateAdvancedFilter()
        {
            this.divToggleAdvancedFilter.Visible = true;
        }

        public bool hangarVisible
        {
            get { return this.divFilterHangar.Visible; }
            set 
            { 
                this.divFilterHangar.Visible = value;
                this.ddlFilterWarehouse.AutoPostBack = true;
                this.ddlFilterWarehouse.SelectedIndexChanged += new EventHandler(ddlFilterWarehouse_SelectedIndexChanged);
            }
        }

        public bool codeAutoPostBack
        {
            get { return this.txtFilterCode.AutoPostBack; }
            set { this.txtFilterCode.AutoPostBack = value; }
        }

        public bool logicalWarehouseVisible
        {
            get { return this.divFilterLogicalWarehouse.Visible; }
            set { this.divFilterLogicalWarehouse.Visible = value; }
        }

        public bool FilterLogicalWarehouseAutoPostBack
        {
            get { return this.ddlFilterLogicalWarehouse.AutoPostBack; }
            set { this.ddlFilterLogicalWarehouse.AutoPostBack = value; }
        }

        /// <summary>
        /// Propiedad utilizada en inboundOrderMgr y outboundOrderMgr. 
        /// Permite salvar el valor seleccionado en la lista de Whs y Own.
        /// </summary>
        private bool saveOnIndexChanged;

        public bool SaveOnIndexChanged
        {
            get { return saveOnIndexChanged; }
            set 
            {
                saveOnIndexChanged = value;
                this.ddlFilterWarehouse.AutoPostBack = true;
                this.ddlFilterWarehouse.SelectedIndexChanged += new EventHandler(ddlFilterWarehouse_SelectedIndexChanged);

                this.ddlFilterOwner.AutoPostBack = true;
                this.ddlFilterOwner.SelectedIndexChanged += new EventHandler(ddlFilterOwner_SelectedIndexChanged);

                this.ddlFilterHangar.AutoPostBack = true;
                this.ddlFilterHangar.SelectedIndexChanged += new EventHandler(ddlFilterHangar_SelectedIndexChanged);

                this.chkComplete.AutoPostBack = true;
                this.chkComplete.CheckedChanged += new EventHandler(chkComplete_CheckedChanged);

                this.chkNotComplete.AutoPostBack = true;
                this.chkNotComplete.CheckedChanged +=new EventHandler(chkNotComplete_CheckedChanged);

                this.chkLpnIsParent.AutoPostBack = true;
                this.chkLpnIsParent.CheckedChanged += new EventHandler(chkLpnIsParent_CheckedChanged);

                this.chkLpnIsNotParent.AutoPostBack = true;
                this.chkLpnIsNotParent.CheckedChanged += new EventHandler(chkLpnIsNotParent_CheckedChanged);

                this.chkLpnIsClosed.AutoPostBack = true;
                this.chkLpnIsClosed.CheckedChanged += new EventHandler(chkLpnIsClosed_CheckedChanged);

                this.chkLpnIsNotClosed.AutoPostBack = true;
                this.chkLpnIsNotClosed.CheckedChanged += new EventHandler(chkLpnIsNotClosed_CheckedChanged);
            }
        }

        public bool itemVisible
        {
            get { return this.divFilterItem.Visible; }
            set { this.divFilterItem.Visible = value; }
        }

        public bool documentVisible
        {
            get { return this.divFilterDocumentNumber.Visible; }
            set { this.divFilterDocumentNumber.Visible = value; }
        }
        
        public bool idTruckCodeVisible
        {
            get { return this.divFilterIdTruckCode.Visible; }
            set { this.divFilterIdTruckCode.Visible = value; }
        }
        

        public bool dateVisible
        {
            get { return this.divFilterDate.Visible; }
            set { this.divFilterDate.Visible = value; }
        }

        public bool dateYearVisible
        {
            get { return this.divFilterDateYear.Visible; }
            set { this.divFilterDateYear.Visible = value; }
        }

        public bool YearVisible
        {
            get { return this.divFilterYear.Visible; }
            set { this.divFilterYear.Visible = value; }
        }

        public bool chkDateEnabled
        {
            get { return this.chkFilterDate.Enabled; }
            set { this.chkFilterDate.Enabled = value; }
        }

        public bool chkDateYearEnabled
        {
            get { return this.chkFilterDateYear.Enabled; }
            set { this.chkFilterDateYear.Enabled = value; }
        }

        public bool chkFilterLockedLocationVisible
        {
            get { return this.divFilterLockedLocation.Visible; }
            set { this.divFilterLockedLocation.Visible = value; }
        }

        public bool chkFilterPendinOrdersVisible
        {
            get { return this.divFilterPendinOrders.Visible; }
            set { this.divFilterPendinOrders.Visible = value; }
        }

        public bool chkDateVisible
        {
            get { return this.chkFilterDate.Visible; }
            set { this.chkFilterDate.Visible = value; }
        }

        public bool chkDateYearVisible
        {
            get { return this.chkFilterDateYear.Visible; }
            set { this.chkFilterDateYear.Visible = value; }
        }

        public bool divPendinOrdersFilter
        {
            get { return this.divFilterPendinOrders.Visible; }
            set { this.divFilterPendinOrders.Visible = value; }
        }

        public bool dateFromVisible
        {
            get { return this.divFilterDateFrom.Visible; }
            set { this.divFilterDateFrom.Visible = value; }
        }

        public bool chkDateFromEnabled
        {
            get { return this.chkFilterDateFrom.Enabled; }
            set { this.chkFilterDateFrom.Enabled = value; }
        }

        public bool chkDateFromVisible
        {
            get { return this.chkFilterDateFrom.Visible; }
            set { this.chkFilterDateFrom.Visible = value; }
        }

        public bool dateToVisible
        {
            get { return this.divFilterDateTo.Visible; }
            set { this.divFilterDateTo.Visible = value; }
        }

        public bool chkDateToEnabled
        {
            get { return this.chkFilterDateTo.Enabled; }
            set { this.chkFilterDateTo.Enabled = value; }
        }

        public bool chkDateToVisible
        {
            get { return this.chkFilterDateTo.Visible; }
            set { this.chkFilterDateTo.Visible = value; }
        }

        public bool chkFilterDateFromChecked
        {
            get { return this.chkFilterDateFrom.Checked; }
            set { this.chkFilterDateFrom.Checked=value; }
        }

        public bool chkFilterDateToChecked
        {
            get { return this.chkFilterDateTo.Checked; }
            set { this.chkFilterDateTo.Checked = value; }
        }        

        public bool lpnTypeVisible
        {
            get { return this.divFilterLpnType.Visible; }
            set { this.divFilterLpnType.Visible = value; }
        }

        public bool truckTypeVisible
        {
            get { return this.divFilterTruckType.Visible; }
            set { this.divFilterTruckType.Visible = value; }
        }

        public bool ddlPeriodVisible
        {
            get { return this.divPeriodUserProductivity.Visible; }
            set { this.divPeriodUserProductivity.Visible = value; }
        }

        public bool ddlViewVisible
        {
            get { return this.divViewUserProductivity.Visible; }
            set { this.divViewUserProductivity.Visible = value; }
         }

        public bool movementTypeVisible
        {
            get { return this.divFilterMovementType.Visible; }
            set { this.divFilterMovementType.Visible = value; }
        }

        public bool taskTypeVisible
        {
            get { return this.divFilterTaskType.Visible; }
            set { this.divFilterTaskType.Visible = value; }
        }

        public bool trackTaskTypeVisible
        {
            get { return this.divFilterTrackTaskType.Visible; }
            set { this.divFilterTrackTaskType.Visible = value; }
        }

        private string[] TrackTaskTypeCode
        {
            get { return this.trackTaskTypeCode; }
            set { this.trackTaskTypeCode = value; }

        }

        public bool inboundTypeVisible
        {
            get { return this.divFilterInboundType.Visible; }
            set { this.divFilterInboundType.Visible = value; }

        }

        public bool trackInboundTypeVisible
        {
            get { return this.divFilterTrackInboundType.Visible; }
            set { this.divFilterTrackInboundType.Visible = value; }

        }

        public bool referenceDocTypeVisible
        {
            get { return this.divFilterReferenceDocType.Visible; }
            set { this.divFilterReferenceDocType.Visible = value; }
        }

        public bool outboundTypeVisible
        {
            get { return this.divFilterOutboundType.Visible; }
            set { this.divFilterOutboundType.Visible = value; }

        }

        //
        public string[] OutboundTypeCode
        {
            get { return this.outboundTypeCode; }
            set { this.outboundTypeCode = value; }

        }
        //

        public bool trackOutboundTypeVisible
        {
            get { return this.divFilterTrackOutboundType.Visible; }
            set { this.divFilterTrackOutboundType.Visible = value; }

        }
        public bool dispatchTypeVisible
        {
            get { return this.divFilterDispatchType.Visible; }
            set { this.divFilterDispatchType.Visible = value; }

        }

        public bool parameterScopeVisible
        {
            get { return this.divFilterScope.Visible; }
            set { this.divFilterScope.Visible = value; }
        }

        public bool parameterKardexType
        {
            get { return this.divFilterKardexType.Visible; }
            set { this.divFilterKardexType.Visible = value; }
        }
        
        public bool statusVisible
        {
            get { return this.divFilterStatus.Visible; }
            set { this.divFilterStatus.Visible = value; }
        }

        public bool transactionStatusVisible
        {
            get { return this.divFilterTransactionStatus.Visible; }
            set { this.divFilterTransactionStatus.Visible = value; }
        }

        public bool simpliRouteVisitStatusVisible
        {
            get { return this.divFilterSimpliRouteVisitStatus.Visible; }
            set { this.divFilterSimpliRouteVisitStatus.Visible = value; }
        }

        public bool locationTypeVisible
        {
            get { return this.divFilterLocationType.Visible; }
            set { this.divFilterLocationType.Visible = value; }
        }

        public bool zoneTypeVisible
        {
            get { return this.divFilterZoneType.Visible; }
            set { this.divFilterZoneType.Visible = value; }
        }

        public bool reasonVisible
        {
            get { return this.divFilterReason.Visible; }
            set { this.divFilterReason.Visible = value; }
        }
        public string nameReasonLabel
        {
            get { return this.lblFilterReason.Text; }
            set { this.lblFilterReason.Text = value; }
        }

        //Variables usadas para Filtro de Row, Column y Level de Locations
        public bool locationFilterVisible
        {
            get { return this.divFilterLocation.Visible; }
            set { this.divFilterLocation.Visible = value; }
        }

        public string LocationFilterType
        {
            get { return this.locationFilterType; }
            set { this.locationFilterType = value; }
        }

        public string RowFromRangeValue
        {
            get { return this.ddlLocRowRangeFrom.SelectedValue; }
        }

        public string RowToRangeValue
        {
            get { return this.ddlLocRowRangeTo.SelectedValue; }
        }

        public string ColumnFromRangeValue
        {
            get { return this.ddlLocColumnRangeFrom.SelectedValue; }
        }

        public string ColumnToRangeValue
        {
            get { return this.ddlLocColumnRangeTo.SelectedValue; }
        }

        public string LevelFromRangeValue
        {
            get { return this.ddlLocLevelRangeFrom.SelectedValue; }
        }

        public string LevelToRangeValue
        {
            get { return this.ddlLocLevelRangeTo.SelectedValue; }
        }
        //FIN Variables usadas para Filtro de Row, Column y Level de Locations

        public bool LocationEqualVisible
        {
            get { return this.divTxtLocationEqual.Visible; }
            set { this.divTxtLocationEqual.Visible = value; }
        }

        public bool LocationRangeVisible
        {
            get { return this.divTxtLocationRange.Visible; }
            set { this.divTxtLocationRange.Visible = value; }
        }

        public bool LocationRowColumnEqualVisible
        {
            get { return this.divTxtLocationRowColumnEqual.Visible; }
            set { this.divTxtLocationRowColumnEqual.Visible = value; }
        }

        public bool LocationLevelEqualVisible
        {
            get { return this.divTxtLocationLevelEqual.Visible; }
            set { this.divTxtLocationLevelEqual.Visible = value; }
        }

        public bool LocationAisleEqualVisible
        {
            get { return this.divTxtLocationAisleEqual.Visible; }
            set { this.divTxtLocationAisleEqual.Visible = value; }
        }

        public string LocationAisle
        {
            get { return this.txtLocationAisle.Text; }
            set { this.txtLocationAisle.Text = value; }
        }

        public bool LocationLocked
        {
            get { return this.divLockedLocation.Visible; }
            set { this.divLockedLocation.Visible = value; }
        }

        public bool FilterWarehouseAutoPostBack
        {
            get { return this.ddlFilterWarehouse.AutoPostBack; }
            set { this.ddlFilterWarehouse.AutoPostBack = value; }
        }

        public bool FilterOwnerAutoPostBack
        {
            get { return this.ddlFilterOwner.AutoPostBack; }
            set { this.ddlFilterOwner.AutoPostBack = value; }
        }

        public bool FilterHangarAutoPostBack
        {
            get { return this.ddlFilterHangar.AutoPostBack; }
            set { this.ddlFilterHangar.AutoPostBack = value; }
        }

        public bool FilterWmsProcessAutoPostBack
        {
            get { return this.ddlFilterWmsProcess.AutoPostBack; }
            set { this.ddlFilterWmsProcess.AutoPostBack = value; }
        }

        public bool chkLocationsMostUsedByItemChecked
        {
            get { return this.chkLocationsMostUsedByItem.Checked; }
            set { this.chkLocationsMostUsedByItem.Checked = value; }
        }

        public bool divTaskCompleteVisible
        {
            get { return this.divTaskComplete.Visible; }
            set { this.divTaskComplete.Visible = value; }
        }

        public bool divEnableReserveStockOnZeroVisible
        {
            get { return this.divEnableReserveStockOnZero.Visible; }
            set { this.divEnableReserveStockOnZero.Visible = value; }
        }
        public bool divOOInmediateProcessVisible
        {
            get { return this.divOOInmediateProcess.Visible; }
            set { this.divOOInmediateProcess.Visible = value; }
        }
        public bool advancedFilterVisible
        {
            get { return this.divAdvancedFilterOptions.Visible; }

            set 
            {
                this.divAdvancedFilterOptions.Visible = value;
                this.divAdvancedFilterChk.Visible = value;
                this.pnlAdvancedFilter.Visible = value;
                this.cpeAdvancedFilter.Enabled = value;
                this.ddlFilterWarehouse.AutoPostBack = true;
                this.ddlFilterWarehouse.SelectedIndexChanged += new EventHandler(ddlFilterWarehouse_SelectedIndexChanged);
                this.ddlFilterOwner.AutoPostBack = true;
                this.ddlFilterOwner.SelectedIndexChanged += new EventHandler(ddlFilterOwner_SelectedIndexChanged);
                this.chkComplete.AutoPostBack = true;
                this.chkComplete.CheckedChanged += new EventHandler(chkComplete_CheckedChanged);
                this.chkNotComplete.AutoPostBack = true;
                this.chkNotComplete.CheckedChanged +=new EventHandler(chkNotComplete_CheckedChanged);
                this.chkLpnIsParent.AutoPostBack = true;
                this.chkLpnIsParent.CheckedChanged += new EventHandler(chkLpnIsParent_CheckedChanged);
                this.chkLpnIsNotParent.AutoPostBack = true;
                this.chkLpnIsNotParent.CheckedChanged += new EventHandler(chkLpnIsNotParent_CheckedChanged);
                this.chkLpnIsClosed.AutoPostBack = true;
                this.chkLpnIsClosed.CheckedChanged += new EventHandler(chkLpnIsClosed_CheckedChanged);
                this.chkLpnIsNotClosed.AutoPostBack = true;
                this.chkLpnIsNotClosed.CheckedChanged += new EventHandler(chkLpnIsNotClosed_CheckedChanged);
            }
        }
                             
        public bool tabLayoutVisible
        {
            get { return this.tabLayout.Visible; }
            set { this.tabLayout.Visible = value; }
        }
        
        public bool tabLocationVisible
        {
            get { return this.tabLocation.Visible; }
            set { this.tabLocation.Visible = value; }
        }

        public bool tabReceptionLogVisible
        {
            get { return this.tabReceptionLog.Visible; }
            set { this.tabReceptionLog.Visible = value; }
        }

        public bool tabGS1Visible
        {
            get { return this.tabGS1.Visible; }
            set { this.tabGS1.Visible = value; }
        }

        public string tabReceptionLogHeaderText
        {
            get { return this.tabReceptionLog.HeaderText; }
            set { this.tabReceptionLog.HeaderText = value; }
        }

        public bool tabTaskVisible
        {
            get { return this.tabTask.Visible; }
            set { this.tabTask.Visible = value; }
        }

        public int tabPollo
        {
            get { return this.Tabs.ActiveTabIndex; }
            set { this.Tabs.ActiveTabIndex = value; }
        }
                
        public bool tabItemGroupVisible
        {
            get { return this.tabItemGroup.Visible;}
            set { this.tabItemGroup.Visible = value; }
        }

        public bool tabDatesVisible
        {
            get { return this.tabDates.Visible; }
            set { this.tabDates.Visible = value; }
        }
        public string tabDatesHeaderText
        {
            get { return this.tabDates.HeaderText; }
            set { this.tabDates.HeaderText = value; }
        }

        #region "TAB PROVEEDOR"
        public bool tabProveedorVisible
        {
            get { return this.tabProveedor.Visible; }
            set { this.tabProveedor.Visible = value; }
        }
        #endregion

        #region "TAB TRANSPORTISTA"
        public bool tabTransportistaVisible
        {
            get { return this.tabTransportista.Visible; }
            set { this.tabTransportista.Visible = value; }
        }
        #endregion

        #region "TAB CHOFER"
        public bool tabChoferVisible
        {
            get { return this.tabChofer.Visible; }
            set { this.tabChofer.Visible = value; }
        }
        #endregion

        #region "TAB DOCUMENT TYPE"

        public bool tabDocumentVisible
        {
            get { return this.tabDocument.Visible; }
            set { this.tabDocument.Visible = value; }
        }

        public bool documentTypeVisible
        {
            get { return this.divDocumentType.Visible; }
            set { this.divDocumentType.Visible = value; }
        }

        public bool vendorVisible
        {
            get { return this.divVendor.Visible; }
            set { this.divVendor.Visible = value; }
        }

        public bool carrierVisible
        {
            get { return this.divCarrier.Visible; }
            set { this.divCarrier.Visible = value; }
        }

        public bool driverVisible
        {
            get { return this.divDriver.Visible; }
            set { this.divDriver.Visible = value; }
        }
        #endregion

        #region "TAB DISPATCHING"
        public bool tabDispatchingVisible
        {
            get { return this.tabDispatching.Visible; }
            set { this.tabDispatching.Visible = value;}
        }

        public bool divDispatchingPriorityVisible
        {
            get { return this.divPriority.Visible; }
            set { this.divPriority.Visible = value; }
        }
        
        public bool txtDispatchingCustomerVisible
        {
            get { return this.txtCustomer.Visible; }
            set {   this.txtCustomer.Visible = value;
                    this.lblCustomer.Visible = value;
                }
        }

        public bool txtDispatchingCarrierVisible
        {
            get { return this.txtCarrier.Visible; }
            set {   this.txtCarrier.Visible = value;
                    this.lblCarrier.Visible = value;
            }
        }

        public string tabDispatchingHeaderText
        {
            get { return  this.tabDispatching.HeaderText; }
            set { this.tabDispatching.HeaderText = value; }
        }
        #endregion

        #region TAB LPN
        public bool tabLPNVisible
        {
            get { return this.tabLpn.Visible; }
            set { this.tabLpn.Visible = value; }
        }
        public bool divLpnVisible
        {
            get { return this.divLpn.Visible; }
            set { this.divLpn.Visible = value; }
        }
        public bool divLpnParentVisible
        {
            get { return this.divLpnParent.Visible; }
            set { this.divLpnParent.Visible = value; }
        }
        public bool divLpnSealNumberVisible
        {
            get { return this.divLpnSealNumber.Visible; }
            set { this.divLpnSealNumber.Visible = value; }
        }
        public bool divLpnIdLpnTypeVisible
        {
            get { return this.divLpnIdLpnType.Visible; }
            set { this.divLpnIdLpnType.Visible = value; }
        }
        public bool chkLpnIsParentVisible
        {
            get { return this.chkLpnIsParent.Visible; }
            set { this.chkLpnIsParent.Visible = value; }
        }
        public bool chkLpnIsNotParentVisible
        {
            get { return this.chkLpnIsNotParent.Visible; }
            set { this.chkLpnIsNotParent.Visible = value; }
        }
        #endregion TAB LPN

        #region TAB STOCK
        public bool tabStockVisible
        {
            get { return this.tabStock.Visible; }
            set { this.tabStock.Visible = value; }
        }
        #endregion TAB STOCK

        #region "TAB POR UNIDADES"
        public bool tabItemUnitsVisible
        {
            get { return this.tabItemUnits.Visible; }
            set { this.tabItemUnits.Visible = value; }
        }
        #endregion "TAB POR UNIDADES"

        #region TAB FILTROS DOC MULTIPLE OPCIONES
        public bool tabMultipleChoiceOrderFiltersVisible
        {
            get { return this.tabMultipleChoiceOrderFilters.Visible; }
            set { this.tabMultipleChoiceOrderFilters.Visible = value; }
        }
        #endregion

        #region TAB FILTROS TRACK TASK MULTIPLE OPCIONES
        public bool tabMultipleChoiceTrackTaskFiltersVisible
        {
            get { return this.tabMultipleChoiceTrackTaskFilters.Visible; }
            set { this.tabMultipleChoiceTrackTaskFilters.Visible = value; }
        }
        #endregion


        #region "TAB MAPA BODEGA"
        public bool tabMapaBodegaVisible 
        {
            get { return this.tabMapaBodega.Visible; }
            set { this.tabMapaBodega.Visible = value; }        
        }
        public string mapFabricationDate
        {
            get { return this.txtMapFabricationDate.Text; }
            set { this.txtMapFabricationDate.Text = value;}
        }       
        public string mapExpirationDate
        {
            get { return this.txtMapExpirationDate.Text; }
            set { this.txtMapExpirationDate.Text = value;}
        }
        public string mapFifoDate
        {
            get { return this.txtMapFifoDate.Text; }
            set { this.txtMapFifoDate.Text = value;}
        }
        public string mapLote
        {
            get { return this.txtMapLote.Text; }
            set { this.txtMapLote.Text = value;}
        }
        public string mapLPN
        {
            get { return this.txtMapLPN.Text; }
            set { this.txtMapLPN.Text = value;}
        }
        public string mapCategory
        {
            get { return this.txtMapCategory.Text; }
            set { this.txtMapCategory.Text = value;}
        }
        public bool mapHoldLocation
        {
            get { return this.chkMapHoldLocation.Checked; }
            set { this.chkMapHoldLocation.Checked = value;}
        }
                   
        #endregion

        public bool fabricationDateVisible
        {
            get { return this.divFabricationDate.Visible; }
            set { this.divFabricationDate.Visible = value; }
        }

        public bool usoUbicacionesVisible
        {
            get
            {
                return this.divUsoUbicaciones.Visible;
            }
            set 
            {
                this.divUsoUbicaciones.Visible = value;
                //this.divMaximoStock.Visible = value; 
            }
        }

        public bool expirationDateVisible
        {
            get { return this.divExpirationDate.Visible; }
            set { this.divExpirationDate.Visible = value; }
        }

        public bool expectedDateVisible
        {
            get { return this.divExpectedDate.Visible; }
            set { this.divExpectedDate.Visible = value; }
        }

        public bool shipmentDateVisible
        {
            get { return this.divShipmentDate.Visible; }
            set { this.divShipmentDate.Visible = value; }
        }

        public bool lotNumberVisible
        {
            get { return this.divLotNumber.Visible; }
            set { this.divLotNumber.Visible = value; }
        }

        public string codeLabel
        {
            get { return this.lblCode.Text; }
            set { this.lblCode.Text = value; }
        }

        public string codeLabelAlt
        {
            get { return this.lblCodeAlt.Text; }
            set { this.lblCodeAlt.Text = value; }
        }

        public string codeNumericLabel
        {
            get { return this.lblCodeNumeric.Text; }
            set { this.lblCodeNumeric.Text = value; }
        }

        public string nameLabel
        {
            get { return this.lblName.Text; }
            set { this.lblName.Text = value; }
        }

        public string descriptionLabel
        {
            get { return this.lblDescription.Text; }
            set { this.lblDescription.Text = value; }
        }

        public string dateLabel
        {
            get { return this.lblDateFrom.Text; }
            set 
            {
                if (setDateLabel)
                {
                    this.lblDateFrom.Text = value;
                    this.lblDateTo.Text = value;
                    this.lblDate.Text = value;
                }
                else
                {
                    this.lblDateFrom.Text = value + " Desde";
                    this.lblDateTo.Text = value + " Hasta";
                }
            }
        }

        public string statusLabel
        {
            get { return this.lblStatus.Text; }
            set { this.lblStatus.Text = value; }
        }

        public string DocumentNumberLabel
        {
            get { return this.lblDocumentNumber.Text; }
            set { this.lblDocumentNumber.Text = value; }
        }

        public string DocumentNbrLabel
        {
            get { return this.lblDocumentNbr.Text; }
            set { this.lblDocumentNbr.Text = value; }
        }

        //public string LabelRadiobuttonVisible
        //{
        //    get { return this.LabelRadiobutton.Text; }
        //    set { this.LabelRadiobutton.Text = value; }
        //}

        //public string LabelRadiobuttonFalseVisible
        //{
        //    get { return this.LabelRadiobuttonFalse.Text; }
        //    set { this.LabelRadiobuttonFalse.Text = value; }
        //}





        public bool showDetailVisible
        {
            get { return this.divShowDetail.Visible; }
            set { this.divShowDetail.Visible = value; }
        }

        public bool showDetailCheck
        {
            get { return this.chkShowDetail.Checked; }
            set { this.chkShowDetail.Checked = value; }
        }

        public bool tabReceptionItemCode
        {
            get { return this.divItemCode.Visible; }
            set { this.divItemCode.Visible = value; }
        }

        public bool tabReceptionItemName
        {
            get { return this.divItemName.Visible; }
            set { this.divItemName.Visible = value; }
        }

        public bool tabReceptionDocumentNbr
        {
            get { return this.divDocumentNbr.Visible; }
            set { this.divDocumentNbr.Visible = value; }
        }

        public bool tabReceptionReferenceNbr
        {
            get { return this.divReferenceNbr.Visible; }
            set { this.divReferenceNbr.Visible = value; }
        }

        public bool tabReceptionOperator
        {
            get { return this.divOperator.Visible; }
            set { this.divOperator.Visible = value; }
        }

        public bool tabReceptionSourceLocation
        {
            get { return this.divSourceLocation.Visible; }
            set { this.divSourceLocation.Visible = value; }
        }

        public bool tabReceptionTargetLocation
        {
            get { return this.divTargetLocation.Visible; }
            set { this.divTargetLocation.Visible = value; }
        }

        public bool tabReceptionSourceLpn
        {
            get { return this.divSourceLpn.Visible; }
            set { this.divSourceLpn.Visible = value; }
        }

        public bool tabReceptionTargetLpn
        {
            get { return this.divTargetLpn.Visible; }
            set { this.divTargetLpn.Visible = value; }
        }

        public bool tabReceptionTaskPriority
        {
            get { return this.divPriorityTask.Visible; }
            set { this.divPriorityTask.Visible = value; }
        }
                                
        public CfgParameterName DateBefore
        {
            get { return this.dateBefore; }
            set { this.dateBefore = value; }
        }

        public CfgParameterName DateAfter
        {
            get { return this.dateAfter; }
            set { this.dateAfter = value; }
        }

        public bool IsValidFilterLocation
        {
            get { return isValidFilterLocation; }
            set { isValidFilterLocation = value; }
        }

        public bool SetDefaultOwner
        {
            get { return setDefaultOwner; }
            set { setDefaultOwner = value; }
        }

        public string ValueDateBefore
        {
            get { return this.txtFilterDateFrom.Text.Trim(); }
        }

        public string ValueDatAfter
        {
            get { return this.txtFilterDateTo.Text.Trim(); }
        }

        #region Translations

        public bool translateVisible
        {
            get { return this.divTranslate.Visible; }
            set { this.divTranslate.Visible = value; }
        }

        public bool moduleVisible
        {
            get { return this.divModule.Visible; }
            set { this.divModule.Visible = value; }
        }

        public bool typeObjectVisible
        {
            get { return this.divTypeObject.Visible; }
            set { this.divTypeObject.Visible = value; }
        }

        public bool propertyVisible
        {
            get { return this.divProperty.Visible; }
            set { this.divProperty.Visible = value; }
        }

        public bool objectContainerVisible
        {
            get { return this.divContainer.Visible; }
            set { this.divContainer.Visible = value; }
        }
        #endregion

        public string FormatDate
        {
            get { return MiscUtils.ReadSetting("FormatDateConsultQuerys", "MM/dd/yyyy"); }
        }

        private string FormatDateTime
        {
            get { return MiscUtils.ReadSetting("FormatDateTimeConsultQuerys", "MM/dd/yyyy HH:mm:ss"); }
        }

        public bool divCountryFilterVisible
        {
            get { return this.divCountryFilter.Visible; }
            set { this.divCountryFilter.Visible = value; }
        }

        public bool divStateFilterVisible
        {
            get { return this.divStateFilter.Visible; }
            set { this.divStateFilter.Visible = value; }
        }

        public bool divCityFilterVisible
        {
            get { return this.divCityFilter.Visible; }
            set { this.divCityFilter.Visible = value; }
        }

        public bool ddlStateFilterAutoPostBack
        {
            get { return this.ddlStateFilter.AutoPostBack; }
            set { this.ddlStateFilter.AutoPostBack = value; }
        }

        public bool divTaskQueueFilterVisible
        {
            get { return this.divTaskQueueFilter.Visible; }
            set { this.divTaskQueueFilter.Visible = value; }
        }
        public bool divTaskQueueFilterVisible2
        {
            get { return this.divTaskQueueFilter.Visible; }
            set { this.divTaskQueueFilter.Visible = value; }
        }

        public bool divRotationItemFilterVisible
        {
            get { return this.divRotationItemFilter.Visible; }
            set { this.divRotationItemFilter.Visible = value; }
        }

        public bool divTerminalStatusVisible
        {
            get { return this.divTerminalStatus.Visible; }
            set { this.divTerminalStatus.Visible = value; }
        }
        #endregion

        #region "Eventos"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                btnSearch.ToolTip = this.lblToolTipBtnSearch.Text;
                base.Page_Init(sender, e);

                /// Recupera el estado actual del Filtro Principal 
                if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.MainFilter))
                    mainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];
                else
                    mainFilter = new List<EntityFilter>();

                if (!Page.IsPostBack)
                {
                    this.tabDispatching.HeaderText = this.lbltabDispatching.Text;
                    this.tabDocument.HeaderText = this.lbltabDocument.Text;
                    this.tabItemGroup.HeaderText = this.lbltabItemGroup.Text;
                    this.tabLayout.HeaderText = this.lbltabLayout.Text;
                    this.tabLocation.HeaderText = this.lbltabLocation.Text;
                    this.tabDates.HeaderText = this.lbltabDates.Text;
                    this.tabReceptionLog.HeaderText = this.lbltabReceptionLog.Text;
                    this.tabTask.HeaderText = this.lbltabTask.Text;
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
            // TODO: revisar Look & Fell de opciones del Filtro Avanzado
        //    btnActivateAdvancedFilter.Attributes.Add("onclick", "toggleButton(true, " + btnActivateAdvancedFilter.ClientID + ")");
        }
               
        protected void btnSearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Limpia el objeto 'Main Filter'
                    ClearFilterObject();

                    // Carga el objeto 'Main Filter' con los valores seleccionados
                    LoadControlValuesToFilterObject();

                    // Salva los criterios seleccionados
                    Session[WMSTekSessions.Global.MainFilter] = (object)mainFilter;

                    // Dispara el evento que será capturado por las páginas que implementen este filtro
                    OnBtnSearchClick(e);
                    
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void btnSearchAux_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Limpia el objeto 'Main Filter'
                    ClearFilterObject();

                    // Carga el objeto 'Main Filter' con los valores seleccionados
                    LoadControlValuesToFilterObject();

                    // Salva los criterios seleccionados
                    Session[WMSTekSessions.Global.MainFilter] = (object)mainFilter;

                    // Dispara el evento que será capturado por las páginas que implementen este filtro
                    OnBtnSearchAuxClick(e);

                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void btnSearchMap2DNew_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
               
                if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Limpia el objeto 'Main Filter'
                    ClearFilterObject();

                    // Carga el objeto 'Main Filter' con los valores seleccionados
                    LoadControlValuesToFilterObject();

                    // Salva los criterios seleccionados
                    Session[WMSTekSessions.Global.MainFilter] = (object)mainFilter;

                    // Dispara el evento que será capturado por las páginas que implementen este filtro
                    OnBtnSearchMap2DNewClick(e);

                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void btnCleanMap_Click(object sender, ImageClickEventArgs e)
        {
            try
            {

                if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Dispara el evento que será capturado por las páginas que implementen este filtro
                    OnBtnCleanMapClick(e);
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    ClearAdvancedControls();
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }
                
        protected void imgBtnSearchProveedor_Click(object sender, ImageClickEventArgs e)
        {
            LoadProveedor(Convert.ToInt16(ddlFilterOwner.SelectedValue),this.txtNombreProveedor.Text);
            
        }

        protected void imgBtnSearchTransportista_Click(object sender, ImageClickEventArgs e)
        {
            LoadTransportista(this.txtNombreTransportista.Text);
        }

        protected void imgBtnSearchChofer_Click(object sender, ImageClickEventArgs e)
        {
            LoadChofer(this.txtNombreChofer.Text);
        }
        
        protected void ddlFilterWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int idWhs = Convert.ToInt16(ddlFilterWarehouse.SelectedValue);

                if (advancedFilterVisible)
                {
                    LoadHangar(idWhs);
                    LoadWorkZone(idWhs);
                    LoadLocationType(idWhs);
                }
                else
                {
                    if (hangarVisible)
                    {
                        ((BasePage)Page).LoadHangar(ddlFilterHangar, idWhs, true, this.lblEmptyRow.Text);
                    }
                }

                if (saveOnIndexChanged)
                {
                    int index = Convert.ToInt16(EntityFilterName.Warehouse);

                    if (ddlFilterWarehouse.SelectedIndex != 0 && ddlFilterWarehouse.SelectedIndex != -1)
                    {
                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterWarehouse.SelectedIndex, ddlFilterWarehouse.SelectedValue));
                        }
                    }

                    // Salva los criterios seleccionados
                    Session[WMSTekSessions.Global.MainFilter] = (object)mainFilter;
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlFilterOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (advancedFilterVisible)
                {
                    if (tabDocumentVisible)
                    {
                        if (vendorVisible)
                        {
                            LoadVendor(Convert.ToInt16(ddlFilterOwner.SelectedValue));
                        }
                    }
                    if (tabProveedorVisible)
                    {
                        txtNombreProveedor.Text = string.Empty;
                        lstProveedor.Items.Clear();
                    }

                    if ( tabItemGroupVisible)
                    {
                        int idOwn = Convert.ToInt16(ddlFilterOwner.SelectedValue);

                        ((BasePage)Page).ConfigureDDLGrpItem1(this.ddlGrpItem1, true, idGrpItem1, this.lblEmptyRow.Text, false, idOwn);
                        ((BasePage)Page).ConfigureDDLGrpItem2(this.ddlGrpItem2, true, idGrpItem1, idGrpItem2, this.lblEmptyRow.Text, false, idOwn);
                        ((BasePage)Page).ConfigureDDLGrpItem3(this.ddlGrpItem3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.lblEmptyRow.Text, false, idOwn);
                        ((BasePage)Page).ConfigureDDLGrpItem4(this.ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.lblEmptyRow.Text, false, idOwn);

                    }
                }

                if (saveOnIndexChanged)
                {
                    int index = Convert.ToInt16(EntityFilterName.Owner);

                    if (ddlFilterOwner.SelectedIndex != 0 && ddlFilterOwner.SelectedIndex != -1)
                    {
                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterOwner.SelectedIndex, ddlFilterOwner.SelectedValue));
                        }
                    }

                    // Salva los criterios seleccionados
                    Session[WMSTekSessions.Global.MainFilter] = (object)mainFilter;
                }

                if (lpnTypeVisible)
                {                   
                    LoadLpnType(); 
                }

                if (uomTypeVisible)
                {
                    LoadUomType();
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
           
        }

        

        protected void ddlFilterHangar_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (saveOnIndexChanged)
                {
                    int index = Convert.ToInt16(EntityFilterName.Hangar);

                    if (ddlFilterHangar.SelectedIndex != 0 && ddlFilterHangar.SelectedIndex != -1)
                    {
                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterHangar.SelectedIndex, ddlFilterHangar.SelectedValue));
                        }
                    }

                    // Salva los criterios seleccionados
                    Session[WMSTekSessions.Global.MainFilter] = (object)mainFilter;
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlGrpItem1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ((BasePage)Page).ConfigureDDLGrpItem2(ddlGrpItem2, true, idGrpItem1, idGrpItem2, this.lblEmptyRow.Text, false, idOwn);
                ((BasePage)Page).ConfigureDDLGrpItem3(ddlGrpItem3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.lblEmptyRow.Text, false, idOwn);
                ((BasePage)Page).ConfigureDDLGrpItem4(ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.lblEmptyRow.Text, false, idOwn);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlGrpItem2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ((BasePage)Page).ConfigureDDLGrpItem3(ddlGrpItem3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.lblEmptyRow.Text, false, idOwn);
                ((BasePage)Page).ConfigureDDLGrpItem4(ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.lblEmptyRow.Text, false, idOwn);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlGrpItem3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ((BasePage)Page).ConfigureDDLGrpItem4(ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.lblEmptyRow.Text, false, idOwn);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlBscGrpItm1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ((BasePage)Page).ConfigureDDLGrpItem2(ddlBscGrpItm2, true, idBscGrpItem1, idBscGrpItem2, this.lblEmptyRow.Text, false, -1);
                ((BasePage)Page).ConfigureDDLGrpItem3(ddlBscGrpItm3, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, this.lblEmptyRow.Text, false, -1);
                ((BasePage)Page).ConfigureDDLGrpItem4(ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, -1);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlBscGrpItm2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ((BasePage)Page).ConfigureDDLGrpItem3(ddlBscGrpItm3, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, this.lblEmptyRow.Text, false, -1);
                ((BasePage)Page).ConfigureDDLGrpItem4(ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, -1);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlBscGrpItm3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ((BasePage)Page).ConfigureDDLGrpItem4(ddlBscGrpItm1, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, -1);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el pais, cambia el estado y la ciudad
                ((BasePage)Page).ConfigureDDlState(this.ddlState, true, -1, idCountry, this.lblEmptyRow.Text);
                ((BasePage)Page).ConfigureDDlCity(this.ddlCity, true, -1, idState, idCountry, this.lblEmptyRow.Text);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlCountryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el pais, cambia el estado 
                ((BasePage)Page).ConfigureDDlState(this.ddlStateFilter, true, -1, idCountryFilter, this.lblEmptyRow.Text);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlStateFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el pais, cambia el estado 
                ((BasePage)Page).ConfigureDDlCity(this.ddlCityFilter, true, -1, idStateFilter, idCountryFilter, this.lblEmptyRow.Text);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }


        protected void ddlTrackTaskQueue_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el estado, solo cambia la ciudad.
                ((BasePage)Page).ConfigureDDlTrackTaskQueue(this.ddlTaskQueueFilter, true, Convert.ToInt32(this.ddlTaskQueueFilter.SelectedValue), this.Master.EmptyRowText, false);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el estado, solo cambia la ciudad.
                ((BasePage)Page).ConfigureDDlCity(this.ddlCity, true, idCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void chkFilterDate_CheckedChanged(object sender, EventArgs e)
        {
            this.txtFilterDate.Enabled = chkFilterDate.Checked;
        }

        protected void chkFilterDateYear_CheckedChanged(object sender, EventArgs e)
        {
            this.txtFilterDateYear.Enabled = chkFilterDateYear.Checked;
        }

        protected void chkFilterDateFrom_CheckedChanged(object sender, EventArgs e)
        {
            this.txtFilterDateFrom.Enabled = chkFilterDateFrom.Checked;            
        }

        protected void chkFilterDateTo_CheckedChanged(object sender, EventArgs e)
        {
            this.txtFilterDateTo.Enabled = chkFilterDateTo.Checked;            
        }

        protected void chkNotComplete_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkNotComplete.Checked == true)
                this.chkComplete.Checked = false;

        }

        protected void chkComplete_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkComplete.Checked == true)
                this.chkNotComplete.Checked = false;

        }

        protected void chkLpnIsClosed_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkLpnIsClosed.Checked == true)
                this.chkLpnIsNotClosed.Checked = false;
        }
        protected void chkLpnIsNotClosed_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkLpnIsNotClosed.Checked == true)
                this.chkLpnIsClosed.Checked = false;
        }
        protected void chkLpnIsParent_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkLpnIsParent.Checked == true)
                this.chkLpnIsNotParent.Checked = false;
        }
        protected void chkLpnIsNotParent_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkLpnIsNotParent.Checked == true)
                this.chkLpnIsParent.Checked = false;
        }
        protected void chkFilterPendinOrders_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkPendinOrders != null)
                this.chkPendinOrders(sender, EventArgs.Empty);
        }

        protected void OnBtnSearchClick(EventArgs e)
        {
            if (BtnSearchClick != null)
            {
                BtnSearchClick(this, e);
            }
        }
        
        protected void OnBtnSearchAuxClick(EventArgs e)
        {
            if (BtnSearchClickAux != null)
            {
                BtnSearchClickAux(this, e);
            }
        }

        protected void OnBtnSearchMap2DNewClick(EventArgs e)
        {
            if (BtnSearchMap2DNewClick != null)
            {
                BtnSearchMap2DNewClick(this, e);
            }
        }

        protected void OnBtnCleanMapClick(EventArgs e)
        {
            if (BtnCleanClick != null)
            {
                BtnCleanClick(this, e);
            }
        }

        protected void OnBtnPrintClick(EventArgs e)
        {
            if (BtnPrintClick != null)
            {
                BtnPrintClick(this, e);
            }
        }

        protected void OnddlOwnerIndexChanged(EventArgs e)
        {
            if (ddlOwnerIndexChanged != null)
            {
                ddlOwnerIndexChanged(this, e);
            }
        }

        protected void OnddlWareHouseIndexChanged(EventArgs e)
        {
            if (ddlWareHouseIndexChanged != null)
            {
                ddlWareHouseIndexChanged(this, e);
            }
        }

        protected void OnddlLogicalWareHouseIndexChanged(EventArgs e)
        {
            if (ddlLogicalWareHouseIndexChanged != null)
            {
                ddlLogicalWareHouseIndexChanged(this, e);
            }
        }

        protected void OnddlHangarIndexChanged(EventArgs e)
        {
            if (ddlHangarIndexChanged != null)
            {
                ddlHangarIndexChanged(this, e);
            }
        }
        protected void imbPrint_Click(object sender, ImageClickEventArgs e)
        {
            OnBtnPrintClick(e);
        }

        protected void ddlFilterOwner_SelectedIndexChanged1(object sender, EventArgs e)
        {
            if (FilterOwnerAutoPostBack)
            {
                OnddlOwnerIndexChanged(e);
            }
        }

        protected void ddlFilterWarehouse_SelectedIndexChanged1(object sender, EventArgs e)
        {
            if (FilterWarehouseAutoPostBack)
            {
                int index = Convert.ToInt16(EntityFilterName.Warehouse);

                if (!this.warehouseNotIncludeAll)
                {

                    if (ddlFilterWarehouse.SelectedIndex != 0 && ddlFilterWarehouse.SelectedIndex != -1)
                    {
                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterWarehouse.SelectedIndex, ddlFilterWarehouse.SelectedValue));
                        }
                    }
                    else
                    {
                        // Si no se selecciona un Warehouse, filtra por todos los Warehouses asociados al usuario
                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();

                            // Agrega -1 para el caso que el usuario no tenga asociado ningún Warehouse
                            mainFilter[index].FilterValues.Add(new FilterItem(0, "-1"));

                            int i = 1;

                            foreach (Warehouse userWhs in context.SessionInfo.User.Warehouses)
                            {
                                mainFilter[index].FilterValues.Add(new FilterItem(i, userWhs.Id.ToString()));
                                i++;
                            }
                        }
                    }
                }
                else
                {
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterWarehouse.SelectedIndex, ddlFilterWarehouse.SelectedValue));
                    }
                }

                OnddlWareHouseIndexChanged(e);
            }
        }

        protected void ddlFilterHangar_SelectedIndexChanged1(object sender, EventArgs e)
        {
            try
            {
                if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Limpia el objeto 'Main Filter'
                    ClearFilterObject();

                    // Carga el objeto 'Main Filter' con los valores seleccionados
                    LoadControlValuesToFilterObject();

                    // Salva los criterios seleccionados
                    Session[WMSTekSessions.Global.MainFilter] = (object)mainFilter;

                    // Dispara el evento que será capturado por las páginas que implementen este filtro
                    OnddlHangarIndexChanged(e);
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlFilterLogicalWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FilterLogicalWarehouseAutoPostBack)
            {
                int index = Convert.ToInt16(EntityFilterName.LogicalWarehouse);

                if (!this.logicalWarehouseNotIncludeAll)
                {
                    if (ddlFilterLogicalWarehouse.SelectedIndex != 0 && ddlFilterLogicalWarehouse.SelectedIndex != -1)
                    {
                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterLogicalWarehouse.SelectedIndex, ddlFilterLogicalWarehouse.SelectedItem.Text.Trim()));
                        }
                    }
                }
                else
                {
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterLogicalWarehouse.SelectedIndex, ddlFilterLogicalWarehouse.SelectedItem.Text.Trim()));
                    }
                }

                OnddlLogicalWareHouseIndexChanged(e);
            }
        }
        #endregion

        #region "Métodos"

        /// <summary>
        /// Inicializa los objetos del Filtro y carga las listas
        /// </summary>
        /// <param name="init">True: primera vez que se ingresa a la página / False: Postback</param>
        /// <param name="refresh">Se hizo click en el botón Refresh de la página</param>
        public void Initialize(bool init, bool refresh)
        {
            try
            {
                if (init)
                {
                    ConfigureDisplay();
                    ConfigureFilterManual();
                    PopulateLists();

                    // Si está activa la opción 'KeepFilter' y el objeto MainFilter está Activo
                    // carga en los controles los valores actuales del objeto MainFilter
                    if (context.SessionInfo.FilterKeep && context.SessionInfo.FilterActive)
                    {
                        LoadControlsFromFilterObject();
                    }
                    // Si NO está activa la opción 'KeepFilter', limpia el objeto MainFilter
                    // y carga en los controles los valores por defecto
                    else
                    {
                        ClearFilterObject();
                        LoadDefaultsToControls();
                        LoadControlValuesToFilterObject();
                        //PopulateLists();
                    }
                }
                else
                {
                    // Si se hizo click en 'Refresh' de la página, limpia el objeto MainFilter y los controles
                    // y los carga con los valores por defecto
                    if (refresh)
                    {
                        ClearControls();
                        LoadDefaultsToControls();

                        ClearFilterObject();
                        LoadControlValuesToFilterObject();
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            if (context != null && context.SessionInfo != null)
            {
                context.SessionInfo.FilterActive = true;
            }
        }

        private void ConfigureFilterManual()
        {
            int index = 0;
            // WmsProcess Type
            if (listWmsProcessType != null)
            {
                if (listWmsProcessType.Count > 0)
                {
                    index = Convert.ToInt16(EntityFilterName.WmsProcessType);
                    mainFilter[index].FilterValues = new List<FilterItem>();

                    foreach (String item in listWmsProcessType)
                    {
                        mainFilter[index].FilterValues.Add(new FilterItem(item));
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.WmsProcessType);
                mainFilter[index].FilterValues = new List<FilterItem>();
            }

            index = 0;
            // Task Type Code
            if (listTaskTypeCode != null)
            {
                if (listTaskTypeCode.Count > 0)
                {
                    index = Convert.ToInt16(EntityFilterName.TaskType);
                    mainFilter[index].FilterValues = new List<FilterItem>();

                    foreach (String item in listTaskTypeCode)
                    {
                        mainFilter[index].FilterValues.Add(new FilterItem(item));
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.TaskType);
                mainFilter[index].FilterValues = new List<FilterItem>();
            }

            index = 0;
            // Track Outbound Type Id
            if (listTrackOutboundType != null)
            {
                if (listTrackOutboundType.Count > 0)
                {
                    index = Convert.ToInt16(EntityFilterName.TrackOutboundType);
                    mainFilter[index].FilterValues = new List<FilterItem>();

                    foreach (String item in listTrackOutboundType)
                    {
                        mainFilter[index].FilterValues.Add(new FilterItem(item));
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.TrackOutboundType);
                mainFilter[index].FilterValues = new List<FilterItem>();
            }

            index = 0;
            // Outbound Type Id
            if (listOutboundType != null)
            {
                if (listOutboundType.Count > 0)
                {
                    index = Convert.ToInt16(EntityFilterName.OutboundType);
                    mainFilter[index].FilterValues = new List<FilterItem>();

                    foreach (String item in listOutboundType)
                    {
                        mainFilter[index].FilterValues.Add(new FilterItem(item));
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.OutboundType);
                mainFilter[index].FilterValues = new List<FilterItem>();
            }

            index = 0;
            // Track Inbound Type Id
            if (listTrackInboundType != null)
            {
                if (listTrackInboundType.Count > 0)
                {
                    index = Convert.ToInt16(EntityFilterName.TrackInboundType);
                    mainFilter[index].FilterValues = new List<FilterItem>();

                    foreach (String item in listTrackInboundType)
                    {
                        mainFilter[index].FilterValues.Add(new FilterItem(item));
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.TrackInboundType);
                mainFilter[index].FilterValues = new List<FilterItem>();
            }

            index = 0;
            // Inbound Type Id
            if (listInboundType != null)
            {
                if (listInboundType.Count > 0)
                {
                    index = Convert.ToInt16(EntityFilterName.InboundType);
                    mainFilter[index].FilterValues = new List<FilterItem>();

                    foreach (String item in listInboundType)
                    {
                        mainFilter[index].FilterValues.Add(new FilterItem(item));
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.InboundType);
                mainFilter[index].FilterValues = new List<FilterItem>();
            }

            index = 0;
            // dispatch Type
            if (listDispatchType != null)
            {
                if (listDispatchType.Count > 0)
                {
                    index = Convert.ToInt16(EntityFilterName.DispatchType);
                    mainFilter[index].FilterValues = new List<FilterItem>();

                    foreach (string item in listDispatchType)
                    {
                        mainFilter[index].FilterValues.Add(new FilterItem(item));
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.DispatchType);
                mainFilter[index].FilterValues = new List<FilterItem>();
            }
        }

        /// <summary>
        /// Carga Controles con los valores por defecto dados por configuración
        /// </summary>
        private void LoadDefaultsToControls()
        {
            int index;
            int days = 0;

            // Warehouse 
            if (this.warehouseVisible)
            {
                ddlFilterWarehouse.ClearSelection();

                // Selecciona Warehouse por defecto del Usuario loggeado
                ((BasePage)Page).SelectDefaultWarehouse(ddlFilterWarehouse);
            }

            // Owner 
            if (this.ownerVisible)
            {
                if (!SetDefaultOwner)
                {
                    ddlFilterOwner.ClearSelection();

                    // Selecciona Owner por defecto del Usuario loggeado
                    ((BasePage)Page).SelectDefaultOwner(ddlFilterOwner);
                }

                if (this.tabItemGroupVisible)
                {
                    // GroupItem 1...4 (distintas a las del filtro básico)
                    ((BasePage)Page).ConfigureDDLGrpItem1(this.ddlGrpItem1, true, idGrpItem1, this.lblEmptyRow.Text, false, idOwn);
                    ((BasePage)Page).ConfigureDDLGrpItem2(this.ddlGrpItem2, true, idGrpItem1, idGrpItem2, this.lblEmptyRow.Text, false, idOwn);
                    ((BasePage)Page).ConfigureDDLGrpItem3(this.ddlGrpItem3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.lblEmptyRow.Text, false, idOwn);
                    ((BasePage)Page).ConfigureDDLGrpItem4(this.ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.lblEmptyRow.Text, false, idOwn);
                }

            }

            // Hangar
            if (this.hangarVisible)
            {
                // Muestra Hangares del Warehouse por defecto
                int idWhs = Convert.ToInt16(ddlFilterWarehouse.SelectedValue);
                ((BasePage)Page).LoadHangar(ddlFilterHangar, idWhs, true, this.lblEmptyRow.Text);
            }

            // Movement Type
            /*if (this.movementTypeVisible)
                ((BasePage)Page).LoadMovementType(this.ddlFilterMovementType, true, lblEmptyRow.Text);*/

            // Date
            if (this.dateVisible)
            {
                //index = Convert.ToInt16(dateBefore);
                days = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));

                txtFilterDate.Text = DateTime.Now.AddDays(-days).ToShortDateString();
                calDate.SelectedDate = DateTime.Now.AddDays(-days);
                calDate.DateFirstMonth = DateTime.Now.AddDays(-days);
            }

            // DateYear
            if (this.dateYearVisible)
            {
                //index = Convert.ToInt16( dateBefore);
                days = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));
                txtFilterDateYear.Text = DateTime.Now.Year.ToString();
            }

            // Year
            if (this.YearVisible)
            {
                txtFilterYear.Text = DateTime.Now.Year.ToString();
            }

            // Date From
            if (this.dateFromVisible)
            {
                //index = Convert.ToInt16(dateBefore);
                days = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));

                txtFilterDateFrom.Text = DateTime.Now.AddDays(-days).ToShortDateString();
                calDateFrom.SelectedDate = DateTime.Now.AddDays(-days);
                calDateFrom.DateFirstMonth = DateTime.Now.AddDays(-days);    
            }
            // Date To
            if (this.dateToVisible)
            {
                //index = Convert.ToInt16(GetCfgParameter(dateAfter.ToString()));
                days = Convert.ToInt16(GetCfgParameter(dateAfter.ToString()));

                txtFilterDateTo.Text = DateTime.Now.AddDays(days).ToShortDateString();
                calDateTo.SelectedDate = DateTime.Now.AddDays(days);
                calDateTo.DateFirstMonth = DateTime.Now.AddDays(days);    
            }

            //// Languague 
            if (this.translateVisible)
            {
                // Selecciona Languague por defecto del Usuario loggeado
                ((BasePage)Page).SelectDefaultLanguague(ddlTranslate);
            }

            if (logicalWarehouseVisible)
            {
                ddlFilterLogicalWarehouse.ClearSelection();
                ((BasePage)Page).LoadLogicalWarehouses(ddlFilterLogicalWarehouse, lblEmptyRow.Text, "-1", true);
            }
                        
        }

        public void LoadHangarToWarehouseControls(int setIdWhs)
        {
            // Warehouse 
            if (this.warehouseVisible)
            {
                ddlFilterWarehouse.ClearSelection();

                // Selecciona Warehouse por defecto del Usuario loggeado
                ((BasePage)Page).SelectDefaultWarehouse(ddlFilterWarehouse);

                ddlFilterWarehouse.SelectedValue = setIdWhs.ToString();
            }

            // Owner 
            if (this.ownerVisible)
            {
                if (!SetDefaultOwner)
                {
                    ddlFilterOwner.ClearSelection();

                    // Selecciona Owner por defecto del Usuario loggeado
                    ((BasePage)Page).SelectDefaultOwner(ddlFilterOwner);
                }

            }

            // Hangar
            if (this.hangarVisible)
            {
                // Muestra Hangares del Warehouse por defecto
                int idWhs = Convert.ToInt16(setIdWhs);
                ((BasePage)Page).LoadHangar(ddlFilterHangar, idWhs, true, this.lblEmptyRow.Text);
            }
                     

        }

        public void InitializeFilterLoc()
        {
            ((BasePage)Page).LoadListRowLoc();
            ((BasePage)Page).LoadListColumnLoc();
            ((BasePage)Page).LoadListLevelLoc();
        }

        public void InitializeFilterLocWhs()
        {
            ((BasePage)Page).LoadRowLocWihtEntities(this.ddlLocRowRangeFrom, this.ddlLocRowRangeTo, true, context);
            ((BasePage)Page).LoadColumnLocWihtEntities(this.ddlLocColumnRangeFrom, this.ddlLocColumnRangeTo, true, context);
            ((BasePage)Page).LoadLevelLocWihtEntities(this.ddlLocLevelRangeFrom, this.ddlLocLevelRangeTo, true, context);
        }

        /// <summary>
        /// Adapta el ancho de los controles segun la resolución del cliente
        /// </summary>
        private void ConfigureDisplay()
        {
            // Resolución baja --> reduce el ancho de los controles en un 15%
            if (Convert.ToInt16(Session["screenX"]) < 1200)
            {
                ddlFilterWarehouse.Width = Convert.ToInt16(ddlFilterWarehouse.Width.Value * .85);
                ddlFilterHangar.Width = Convert.ToInt16(ddlFilterHangar.Width.Value * .85);
                ddlFilterOwner.Width = Convert.ToInt16(ddlFilterOwner.Width.Value * .85);
                ddlFilterLpnType.Width = Convert.ToInt16(ddlFilterLpnType.Width.Value * .85);
                ddlFilterTruckType.Width = Convert.ToInt16(ddlFilterTruckType.Width.Value * .85);
                ddlFilterMovementType.Width = Convert.ToInt16(ddlFilterMovementType.Width.Value * .85);
                ddlFilterTaskType.Width = Convert.ToInt16(ddlFilterTaskType.Width.Value * .85);
                ddlFilterTrackTaskType.Width = Convert.ToInt16(ddlFilterTrackTaskType.Width.Value * .85);
                ddlFilterLpnType.Width = Convert.ToInt16(ddlFilterLpnType.Width.Value * .85);
                ddlFilterInboundType.Width = Convert.ToInt16(ddlFilterInboundType.Width.Value * .85);
                ddlFilterTrackInboundType.Width = Convert.ToInt16(ddlFilterTrackInboundType.Width.Value * .85);
                ddlFilterReferenceDocType.Width = Convert.ToInt16(ddlFilterReferenceDocType.Width.Value * .85);
                ddlFilterOutboundType.Width = Convert.ToInt16(ddlFilterOutboundType.Width.Value * .85);
                ddlFilterTrackOutboundType.Width = Convert.ToInt16(ddlFilterTrackOutboundType.Width.Value * .85);
                ddlFilterDispatchType.Width = Convert.ToInt16(ddlFilterDispatchType.Width.Value * .85);
                ddlFilterScope.Width = Convert.ToInt16(ddlFilterScope.Width.Value * .85);
                ddlFilterLocationType.Width = Convert.ToInt16(ddlFilterLocationType.Width.Value * .85);
                ddlFilterZoneType.Width = Convert.ToInt16(ddlFilterZoneType.Width.Value * .85);
                ddlFilterWmsProcess.Width = Convert.ToInt16(ddlFilterWmsProcess.Width.Value * .85);
                ddlFilterReason.Width = Convert.ToInt16(ddlFilterReason.Width.Value * .85);
                ddlFilterUomType.Width = Convert.ToInt16(ddlFilterUomType.Width.Value * .85);

                txtFilterCode.Width = Convert.ToInt16(txtFilterCode.Width.Value * .85);
                txtFilterCodeAlt.Width = Convert.ToInt16(txtFilterCodeAlt.Width.Value * .85);
                txtFilterCodeNumeric.Width = Convert.ToInt16(txtFilterCodeNumeric.Width.Value * .85);
                txtFilterItem.Width = Convert.ToInt16(txtFilterItem.Width.Value * .85);
                txtFilterName.Width = Convert.ToInt16(txtFilterName.Width.Value * .85);
                txtFilterDescription.Width = Convert.ToInt16(txtFilterDescription.Width.Value * .85);
                txtFilterDocumentNumber.Width = Convert.ToInt16(txtFilterDocumentNumber.Width.Value * .100);
                txtFilterIdTruckCode.Width = Convert.ToInt16(txtFilterIdTruckCode.Width.Value * .100);
                txtFilterName.Width = Convert.ToInt16(txtFilterName.Width.Value * .85);
                txtPorcentajeUso.Width = Convert.ToInt16(txtPorcentajeUso.Width.Value * .85);
                txtMaxStock.Width = Convert.ToInt16(txtMaxStock.Width.Value * .85);
                chkShowDetail.Width = Convert.ToInt16(chkShowDetail.Width.Value * .85);
                chkNotComplete.Width = Convert.ToInt16(chkNotComplete.Width.Value * .85);
                chkComplete.Width = Convert.ToInt16(chkComplete.Width.Value * .85);
                chkLpnIsNotParent.Width = Convert.ToInt16(chkLpnIsNotParent.Width.Value * .85);
                chkLpnIsParent.Width = Convert.ToInt16(chkLpnIsParent.Width.Value * .85);
                chkLpnIsNotClosed.Width = Convert.ToInt16(chkLpnIsNotClosed.Width.Value * .85);
                chkLpnIsClosed.Width = Convert.ToInt16(chkLpnIsClosed.Width.Value * .85);
                chkLockedStock.Width = Convert.ToInt16(chkLockedStock.Width.Value * .85);
            }
        }

        /// <summary>
        /// Carga las listas y valores por defecto
        /// </summary>
        private void PopulateLists()
        {
            #region "FILTRO BÁSICO"
            // Warehouse 
            if (this.warehouseVisible)
                if(!this.warehouseNotIncludeAll)
                    ((BasePage)Page).LoadUserWarehouses(this.ddlFilterWarehouse, lblEmptyRow.Text, "-1", true);
                else
                    ((BasePage)Page).LoadUserWarehouses(this.ddlFilterWarehouse, lblEmptyRow.Text, "-1", false);

            // Owner
            if (this.ownerVisible)
                if(!this.ownerNotIncludeAll)
                    ((BasePage)Page).LoadUserOwners(this.ddlFilterOwner, lblEmptyRow.Text, "-1", true, lblNullOwnerRow.Text, ownerIncludeNulls);
                else
                    ((BasePage)Page).LoadUserOwners(this.ddlFilterOwner, lblEmptyRow.Text, "-1", false, lblNullOwnerRow.Text, ownerIncludeNulls);

            // Lpn Type
            if (this.lpnTypeVisible)
            {
                //((BasePage)Page).LoadLpnType(this.ddlFilterLpnType, true, lblEmptyRow.Text);
                ((BasePage)Page).LoadLpnTypeActive(this.ddlFilterLpnType, context.SessionInfo.Owner.Id, true, lblEmptyRow.Text);
            }

            // Truck Type
            if (this.truckTypeVisible)
                ((BasePage)Page).LoadTruckType(this.ddlFilterTruckType, true, lblEmptyRow.Text);

            // Movement Type
            if (this.movementTypeVisible)
                ((BasePage)Page).LoadMovementType(this.ddlFilterMovementType, true, lblEmptyRow.Text);

            // Task Type
            if (this.taskTypeVisible)
                if (!this.taskTypeNotIncludeAll)
                {
                    ((BasePage)Page).LoadTaskType(this.ddlFilterTaskType, true, lblEmptyRow.Text);
                }
                else
                {
                    ((BasePage)Page).LoadTaskType(this.ddlFilterTaskType, false, lblEmptyRow.Text);
                }

            // Track Task Type
            if (this.trackTaskTypeVisible)
            {
                if (!this.trackTaskTypeNotIncludeAll)
                {
                    if (trackTaskTypeCode!=null && trackTaskTypeCode.Count() > 0)
                    {
                        ((BasePage)Page).LoadTrackTaskTypeFilter(this.ddlFilterTrackTaskType, true, lblEmptyRow.Text, trackTaskTypeCode);
                    }
                    else
                    {
                        ((BasePage)Page).LoadTrackTaskType(this.ddlFilterTrackTaskType, true, lblEmptyRow.Text);
                    }
                }
                else
                {
                    if (trackTaskTypeCode.Count() > 0)
                    {
                        ((BasePage)Page).LoadTrackTaskTypeFilter(this.ddlFilterTrackTaskType, false, lblEmptyRow.Text, trackTaskTypeCode);
                    }
                    else
                    {
                        ((BasePage)Page).LoadTrackTaskType(this.ddlFilterTrackTaskType, false, lblEmptyRow.Text);
                    }
                }
            }

            // Location Type
            if (this.locationTypeVisible)
                ((BasePage)Page).LoadLocationType(this.ddlFilterLocationType, true, lblEmptyRow.Text);

            // Zone Type
            if (this.zoneTypeVisible)
                ((BasePage)Page).LoadTypeWorkZone(this.ddlFilterZoneType, true, lblEmptyRow.Text);

            // Uom Type
            if (this.uomTypeVisible)
                ((BasePage)Page).LoadUomType(this.ddlFilterUomType, context.SessionInfo.Owner.Id, true, lblEmptyRow.Text);

            // Reason
            if (this.reasonVisible)
            {
                if (this.includeReasonAvailableVisible)
                {
                    ((BasePage)Page).LoadReasonFilterByTypeInOutNew(this.ddlFilterReason, reasonFilterWithTypeInOut, true, lblEmptyRow.Text);
                }
                else
                {
                    ((BasePage)Page).LoadReasonFilterByTypeInOut(this.ddlFilterReason, reasonFilterWithTypeInOut, true, lblEmptyRow.Text);
                }
            }
            // Translation
            if (this.translateVisible)
                ((BasePage)Page).LoadLanguageDefined(this.ddlTranslate, true, lblEmptyRow.Text);

            if (this.moduleVisible)
                ((BasePage)Page).LoadModule(this.ddlModule, true, lblEmptyRow.Text);

            if (this.typeObjectVisible)
                ((BasePage)Page).LoadTypeObject(this.ddlTypeObject, true, lblEmptyRow.Text);

            if (this.propertyVisible)
                ((BasePage)Page).LoadProperty(this.ddlProperty, true, lblEmptyRow.Text);

            if (this.objectContainerVisible)
                ((BasePage)Page).LoadContainer(this.ddlObjectContainer, true, lblEmptyRow.Text);

          
            //Deshabilita y deja chequeado el checkbox del filtro avanzado
            if (this.ChkDisabledAndChequed)
            {
                this.chkUseAdvancedFilter.Checked = true;
                this.chkUseAdvancedFilter.Enabled = false;
            }

            // Row - Column - Level
            if (this.locationFilterVisible)
            {
                if (this.locationFilterType == context.SessionInfo.IdPage)
                {
                    ((BasePage)Page).LoadRowLoc(this.ddlLocRowRangeFrom, this.ddlLocRowRangeTo, true);
                    ((BasePage)Page).LoadColumnLoc(this.ddlLocColumnRangeFrom, this.ddlLocColumnRangeTo,true);
                    ((BasePage)Page).LoadLevelLoc(this.ddlLocLevelRangeFrom, this.ddlLocLevelRangeTo, true);

                }
                //if (this.locationFilterType == context.SessionInfo.IdPage)
                //{
                //((BasePage)Page).LoadRowLoc(this.ddlLocRowRangeFrom, this.ddlLocRowRangeTo, true);
                //((BasePage)Page).LoadColumnLoc(this.ddlLocColumnRangeFrom, this.ddlLocColumnRangeTo, true);
                //((BasePage)Page).LoadLevelLoc(this.ddlLocLevelRangeFrom, this.ddlLocLevelRangeTo, true);
                //}
                //if (this.locationFilterType == context.SessionInfo.IdPage)
                //{
                //    ((BasePage)Page).LoadRowLocationByHngAndLocType(this.ddlLocRowRangeFrom, this.ddlLocRowRangeTo,lblEmptyRow.Text,lblEmptyRowValue.Text, true, idWhs, idHangar,loctypecode)
                //    ((BasePage)Page).LoadColumnLocationByHngAndLocType(this.ddlLocRowRangeFrom, this.ddlLocRowRangeTo,lblEmptyRow.Text,lblEmptyRowValue.Text, true, idWhs, idHangar,loctypecode)
                //    ((BasePage)Page).LoadLevelLocationByHngAndLocType(this.ddlLocRowRangeFrom, this.ddlLocRowRangeTo,lblEmptyRow.Text,lblEmptyRowValue.Text, true, idWhs, idHangar,loctypecode)
                //}
            }

            //InboundType
            if(inboundTypeVisible)
            {
                ((BasePage)Page).LoadInboundType(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                //switch (Convert.ToInt16(Request.QueryString["IT"]))
                //{
                //    case ((int)InboundTypeName.Todos):
                //        ((BasePage)Page).LoadInboundType(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                //        break;
                //    case ((int)InboundTypeName.DevolucionConDoc):
                //        ((BasePage)Page).LoadInboundTypeDev(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                //        break;
                //    case (int)InboundTypeName.Produccion:
                //        ((BasePage)Page).LoadInboundTypeProd(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                //        break;
                //    case (int)InboundTypeName.Traspaso:
                //        ((BasePage)Page).LoadInboundTypeTrasp(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                //        break;
                //    default:
                //        ((BasePage)Page).LoadInboundTypeNacInt(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                //        break;
                //}
            }

            //TrackOutboundType
            if (this.trackInboundTypeVisible)
            {
                if (!this.trackInboundTypeNotIncludeAll)
                    ((BasePage)Page).LoadTrackInbound(this.ddlFilterTrackInboundType, true, lblEmptyRow.Text);
                else
                    ((BasePage)Page).LoadTrackInbound(this.ddlFilterTrackInboundType, false, lblEmptyRow.Text);

            }

            //ReferenceDocType
            if (this.referenceDocTypeVisible)
            {
                ((BasePage)Page).LoadReferenceDocType(this.ddlFilterReferenceDocType, true, lblEmptyRow.Text);
            }

            //OutboundType
            if (this.outboundTypeVisible)
            {
                if (!this.outboundTypeNotIncludeAll)
                {
                    if (this.OutboundTypeCode.Length < 1)
                    {
                        if (this.outboundTypeAll)
                        {
                            ((BasePage)Page).LoadAllOutboundType(this.ddlFilterOutboundType, true, lblEmptyRow.Text);
                        }
                        else
                        {
                            ((BasePage)Page).LoadOutboundType(this.ddlFilterOutboundType, true, lblEmptyRow.Text);
                        }
                    }
                    else
                    {
                        ((BasePage)Page).LoadOutboundTypeFilter(this.ddlFilterOutboundType, true, lblEmptyRow.Text, OutboundTypeCode);
                    }
                }
                else
                {
                    if (this.OutboundTypeCode.Length < 1)
                    {
                        ((BasePage)Page).LoadOutboundType(this.ddlFilterOutboundType, false, lblEmptyRow.Text);
                    }
                    else
                    {
                        ((BasePage)Page).LoadOutboundTypeFilter(this.ddlFilterOutboundType, true, lblEmptyRow.Text, OutboundTypeCode);
                    }
                }
            }

            //TrackOutboundType
            if (this.trackOutboundTypeVisible)
            {
                if (!this.trackOutboundTypeNotIncludeAll)
                    ((BasePage)Page).LoadTrackOutboundType(this.ddlFilterTrackOutboundType, true, lblEmptyRow.Text);
                else
                    ((BasePage)Page).LoadTrackOutboundType(this.ddlFilterTrackOutboundType, false, lblEmptyRow.Text);

            }

            //Traza
            if (this.dispatchTypeVisible)
            {
                if (!this.dispatchTypeNotIncludeAll)
                    ((BasePage)Page).LoadDispatchType(this.ddlFilterDispatchType, true, lblEmptyRow.Text);
                else
                    ((BasePage)Page).LoadDispatchType(this.ddlFilterDispatchType, false, lblEmptyRow.Text);

            }

            //Scope
            if (parameterScopeVisible)
                ((BasePage)Page).LoadScope(this.ddlFilterScope, true, lblEmptyRow.Text);

            //Kardex Type
            if (parameterKardexType)
                ((BasePage)Page).LoadKardexType(this.ddlFilterKardexType, true, lblEmptyRow.Text);


            if (this.divBasicItemGroupVisible)
            {
                // GroupItem 1...4 (distintas a las del filtro avanzado)
                ((BasePage)Page).ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idBscGrpItem1, this.lblEmptyRow.Text, false, -1);
                ((BasePage)Page).ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idBscGrpItem1, idBscGrpItem2, this.lblEmptyRow.Text, false, -1);
                ((BasePage)Page).ConfigureDDLGrpItem3(this.ddlBscGrpItm3, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, this.lblEmptyRow.Text, false, -1);
                ((BasePage)Page).ConfigureDDLGrpItem4(this.ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, -1);
            }

            // WmsProcessType
            if (this.WmsProcessTypeVisible)
                if (this.IdModule != null && this.IdModule > 0)
                {
                    ((BasePage)Page).LoadWmsProcessModule(this.ddlFilterWmsProcess, (int)this.IdModule, true, lblEmptyRow.Text);
                }
                else {
                    ((BasePage)Page).LoadWmsProcess(this.ddlFilterWmsProcess, true, lblEmptyRow.Text);
                }

            //if (this.ddlTrackLPNTypeVisible)
            //{
            //    ((BasePage)Page).LoadTrackLPNType(this.ddlTrackLPNType, lblEmptyRow.Text);
            //}

            if (divCountryFilterVisible)
            {
                ((BasePage)Page).ConfigureDDlCountry(this.ddlCountryFilter, true, -1, "(Seleccione...)");
            }

            if (divTaskQueueFilterVisible)
            {
                if (taskQueueFilterSimulation)
                {
                    ((BasePage)Page).ConfigureDDlTrackTaskQueue(this.ddlTaskQueueFilter, true, -1, "(Seleccione...)", true);
                }
                else
                {
                    ((BasePage)Page).ConfigureDDlTrackTaskQueue(this.ddlTaskQueueFilter, true, -1, "(Seleccione...)", false);
                }
            }

			if (this.logicalWarehouseVisible)
                ((BasePage)Page).LoadLogicalWarehouses(this.ddlFilterLogicalWarehouse, lblEmptyRow.Text, "-1", true);

			if (divTaskQueueFilterVisible2)
            {
                   ((BasePage)Page).ConfigureDDlTrackTaskQueue(this.ddlTaskQueueFilter, false, -1, "(Seleccione...)", false);
            }
            #endregion

            #region "FILTRO AVANZADO"
            if (this.advancedFilterVisible)
            {
                if (this.tabLayoutVisible)
                {
                    // Hangar
                    if (context.SessionInfo.Warehouse != null)
                        LoadHangar(context.SessionInfo.Warehouse.Id);

                    // WorkZone
                    if (context.SessionInfo.Warehouse != null)
                        LoadWorkZone(context.SessionInfo.Warehouse.Id);

                    // Location
                    LoadLocationType(-1);
                }

                if (this.tabDocumentVisible)
                {
                    // Document Type
                    if (documentTypeVisible)
                        LoadOutboundType();

                    // Vendor
                    if(vendorVisible)
                        LoadVendor(Convert.ToInt16(ddlFilterOwner.SelectedValue));

                    // Carrier
                    if(carrierVisible)
                        LoadCarrier();

                    //Driver
                    if (driverVisible)
                        ((BasePage)Page).LoadDriver(this.lstDriver, true, lblEmptyRow.Text);
                }

                if (this.tabItemGroupVisible)
                {
                    // GroupItem 1...4 (distintas a las del filtro básico)
                    ((BasePage)Page).ConfigureDDLGrpItem1(this.ddlGrpItem1, true, idGrpItem1, this.lblEmptyRow.Text, false, idOwn);
                    ((BasePage)Page).ConfigureDDLGrpItem2(this.ddlGrpItem2, true, idGrpItem1, idGrpItem2, this.lblEmptyRow.Text, false, -1);
                    ((BasePage)Page).ConfigureDDLGrpItem3(this.ddlGrpItem3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.lblEmptyRow.Text, false, -1);
                    ((BasePage)Page).ConfigureDDLGrpItem4(this.ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.lblEmptyRow.Text, false, -1);
                }

                if (this.tabDispatchingVisible)
                {
                    // Country / State / City
                    ((BasePage)Page).FindAllPlaces();
                    ((BasePage)Page).ConfigureDDlCountry(this.ddlCountry, true, idCountry, this.Master.EmptyRowText);
                    ((BasePage)Page).ConfigureDDlState(this.ddlState, true, idState, idCountry, this.Master.EmptyRowText);
                    ((BasePage)Page).ConfigureDDlCity(this.ddlCity, true, idCity, idState, idCountry, this.Master.EmptyRowText);
                }

                if (this.tabLPNVisible)
                {
                    // LPN Type
                    LoadLpnLpnType();
                    
                }

                if (tabMultipleChoiceOrderFiltersVisible)
                {
                    LoadAllTrackOutboundType();
                }
                if (tabMultipleChoiceTrackTaskFiltersVisible)
                {
                    LoadAllTrackTask();
                }
            }
            #endregion
        }

        /// <summary>
        /// Carga lista de Hangares, segun Warehouse seleccionado - FILTRO AVANZADO
        /// </summary>
        /// <param name="idWhs"></param>
        public void LoadHangar(int idWhs)
        {
            GenericViewDTO<Hangar> hangarViewDTO = new GenericViewDTO<Hangar>();

            hangarViewDTO = iLayoutMGR.GetHangarByWhs(idWhs, context);

            lstHangar.DataSource = hangarViewDTO.Entities;
            lstHangar.DataTextField = "Name";
            lstHangar.DataValueField = "Id";
            lstHangar.DataBind();

            lstHangar.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            lstHangar.Items[0].Selected = true;
        }

        /// <summary>
        /// Carga lista de WorkZones, segun Warehouse seleccionado
        /// </summary>
        /// <param name="idWhs"></param>
        public void LoadWorkZone(int idWhs)
        {
            // TODO: cargar desde session en BasePage - Ver base.LoadHangar(...)
            GenericViewDTO<WorkZone> workZoneViewDTO = new GenericViewDTO<WorkZone>();

            workZoneViewDTO = iLayoutMGR.GetWorkZoneByWhs(idWhs, context);

            lstWorkZone.DataSource = workZoneViewDTO.Entities;
            lstWorkZone.DataTextField = "Name";
            lstWorkZone.DataValueField = "Id";
            lstWorkZone.DataBind();

            lstWorkZone.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            lstWorkZone.Items[0].Selected = true;
        }

        /// <summary>
        /// Carga lista de Tipos de Locations por Warehouse
        /// </summary>
        /// <param name="idWhs"></param>
        public void LoadLocationType(int idWhs)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<LocationType> locationTypeViewDTO = new GenericViewDTO<LocationType>();

            locationTypeViewDTO = iLayoutMGR.GetLocationTypeByWhs(idWhs, context);

            if (advancedFilterVisible)
            {
                lstLocationType.DataSource = locationTypeViewDTO.Entities;

                lstLocationType.DataTextField = "LocTypeName";
                lstLocationType.DataValueField = "LocTypeCode";
                lstLocationType.DataBind();

                lstLocationType.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
                lstLocationType.Items[0].Selected = true;
            }
        }

        public void LoadAllTrackOutboundType()
        {
            GenericViewDTO<TrackOutboundType> trackOutboundTypeViewDTO = new GenericViewDTO<TrackOutboundType>();

            trackOutboundTypeViewDTO = iDispatchingMGR.FindAllTrackOutboundType(context);

            if (advancedFilterVisible)
            {
                lstTrackOutboundType.DataSource = trackOutboundTypeViewDTO.Entities;

                lstTrackOutboundType.DataTextField = "Name";
                lstTrackOutboundType.DataValueField = "Id";
                lstTrackOutboundType.DataBind();

                lstTrackOutboundType.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
                lstTrackOutboundType.Items[0].Selected = true;
            }
        }

        public void LoadAllTrackTask()
        {
            ((BasePage)Page).ConfigureLstViewTrackTaskQueue(this.lstLstTrackTask, true, -1, "(Seleccione...)", true);

            //trackOutboundTypeViewDTO = uti (context);

            //if (advancedFilterVisible)
            //{
            //    lstTrackOutboundType.DataSource = trackOutboundTypeViewDTO.Entities;

            //    lstTrackOutboundType.DataTextField = "Name";
            //    lstTrackOutboundType.DataValueField = "Id";
            //    lstTrackOutboundType.DataBind();

            //    lstTrackOutboundType.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            //    lstTrackOutboundType.Items[0].Selected = true;
            //}
        }

        /// <summary>
        /// Carga lista de InboundType
        /// </summary>
        public void LoadInboundType()
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<InboundType> inboundTypeViewDTO = new GenericViewDTO<InboundType>();

            inboundTypeViewDTO = iReceptionMGR.FindAllInboundType(context);

            lstInboundType.DataSource = inboundTypeViewDTO.Entities;
            lstInboundType.DataTextField = "Code";
            lstInboundType.DataValueField = "Id";
            lstInboundType.DataBind();

            lstInboundType.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            lstInboundType.Items[0].Selected = true;
        }

        /// <summary>
        /// Carga lista de InboundType
        /// </summary>
        public void LoadOutboundType()
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<OutboundType> outboundTypeViewDTO = new GenericViewDTO<OutboundType>();

            outboundTypeViewDTO = iDispatchingMGR.FindAllOutboundType(context);

            lstInboundType.DataSource = outboundTypeViewDTO.Entities;
            lstInboundType.DataTextField = "Code";
            lstInboundType.DataValueField = "Id";
            lstInboundType.DataBind();

            lstInboundType.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            lstInboundType.Items[0].Selected = true;
        }

        /// <summary>
        /// Carga lista de Vendor, segun Owner seleccionado
        /// </summary>
        /// <param name="idOwner"></param>
        public void LoadVendor(int idOwner)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<Vendor> vendorViewDTO = new GenericViewDTO<Vendor>();

            vendorViewDTO = iWarehousingMGR.GetVendorByOwner(context, idOwner);

            lstVendor.DataSource = vendorViewDTO.Entities;
            lstVendor.DataTextField = "Name";
            lstVendor.DataValueField = "Id";
            lstVendor.DataBind();

            lstVendor.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            lstVendor.Items[0].Selected = true;
        }

        /// <summary>
        /// Carga los parametros para el envío de la impresión en un PopUp
        /// </summary>
        /// <param name="p_url"></param>
        /// <param name="p_ReportType"></param>
        public void LoadPrint(Hashtable p_tablaHash, String p_ReportType)
        {
            String parametros = String.Empty;

            foreach (DictionaryEntry de in p_tablaHash)
            {
                parametros += "&" + de.Key + "=" + de.Value;
            }

            String script = "window.open(\"/Reports/Generic/RptGenericPrint.aspx?TypeReportEnum=" + p_ReportType + parametros + "\", \"Impresión\" , \"width=820,height=700,scrollbars=NO\") ";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "alert", script, true);
            
        }

        public void LoadLpnType()
        {
            GenericViewDTO<LPNType> lpnTypeViewDTO = new GenericViewDTO<LPNType>();
            int index = Convert.ToInt16(EntityFilterName.Owner);

            lpnTypeViewDTO.Entities = new List<LPNType>();
            ContextViewDTO newContexto = new ContextViewDTO(); ;
            //newContexto = context;
            
            if (ddlFilterOwner.SelectedValue.Trim() == "-1")
            {
                if (newContexto.MainFilter == null)
                    newContexto.MainFilter = mainFilter;

                newContexto.MainFilter[index].FilterValues.Clear();

                foreach (ListItem item in this.ddlFilterOwner.Items)
                {
                    newContexto.MainFilter[index].FilterValues.Add(new FilterItem(item.Text, item.Value));
                }
            }
            else
            {
                newContexto.MainFilter = mainFilter;
            }            
            
            lpnTypeViewDTO = iWarehousingMGR.FindAllLpnType(newContexto);

            ////Buscar el id de hangar que corresponda al warehouse que se seleccionó
            //var lpnTypes = from lpnType in lpnTypeViewDTO.Entities
            //               where lpnType.Owner.Id == idOwner
            //               where lpnType.Status == true
            //               select lpnType;

            // Carga control
            ddlFilterLpnType.Items.Clear();
            ddlFilterLpnType.DataSource = lpnTypeViewDTO.Entities;
            ddlFilterLpnType.DataTextField = "Name";
            ddlFilterLpnType.DataValueField = "Id";
            ddlFilterLpnType.DataBind();

            ddlFilterLpnType.ClearSelection();
            ddlFilterLpnType.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
            ddlFilterLpnType.Items[0].Selected = true;     
        }

        public void LoadUomType()
        {
            GenericViewDTO<LPNType> lpnTypeViewDTO = new GenericViewDTO<LPNType>();
            int index = Convert.ToInt16(EntityFilterName.Owner);

            lpnTypeViewDTO.Entities = new List<LPNType>();
            ContextViewDTO newContexto = new ContextViewDTO(); ;
            //newContexto = context;

            if (ddlFilterOwner.SelectedValue.Trim() == "-1")
            {
                if (newContexto.MainFilter == null)
                    newContexto.MainFilter = mainFilter;

                newContexto.MainFilter[index].FilterValues.Clear();

                foreach (ListItem item in this.ddlFilterOwner.Items)
                {
                    newContexto.MainFilter[index].FilterValues.Add(new FilterItem(item.Text, item.Value));
                }
            }
            else
            {
                newContexto.MainFilter = mainFilter;
            }

            GenericViewDTO<UomType> uomTypeViewDTO = new GenericViewDTO<UomType>();

            uomTypeViewDTO = iWarehousingMGR.FindAllUomType(newContexto);

            // Carga control
            ddlFilterUomType.Items.Clear();
            ddlFilterUomType.DataSource = lpnTypeViewDTO.Entities;
            ddlFilterUomType.DataTextField = "Name";
            ddlFilterUomType.DataValueField = "Id";
            ddlFilterUomType.DataBind();

            ddlFilterUomType.ClearSelection();
            ddlFilterUomType.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));
            ddlFilterUomType.Items[0].Selected = true;
        }

        public void LoadLpnLpnType()
        {
            GenericViewDTO<LPNType> lpnTypeViewDTO = new GenericViewDTO<LPNType>();
            int index = Convert.ToInt16(EntityFilterName.Owner);

            lpnTypeViewDTO.Entities = new List<LPNType>();
            ContextViewDTO newContexto = new ContextViewDTO(); ;
            //newContexto = context;

            if (ddlFilterOwner.SelectedValue.Trim() == "-1")
            {
                if (newContexto.MainFilter == null)
                    newContexto.MainFilter = mainFilter;

                newContexto.MainFilter[index].FilterValues.Clear();

                foreach (ListItem item in this.ddlFilterOwner.Items)
                {
                    newContexto.MainFilter[index].FilterValues.Add(new FilterItem(item.Text, item.Value));
                }
            }
            else
            {
                newContexto.MainFilter = mainFilter;
            }

            lpnTypeViewDTO = iWarehousingMGR.FindAllLpnType(newContexto);

            // Carga control
            lstLpnLpnType.Items.Clear();
            lstLpnLpnType.DataSource = lpnTypeViewDTO.Entities;
            lstLpnLpnType.DataTextField = "Name";
            lstLpnLpnType.DataValueField = "Id";
            lstLpnLpnType.DataBind();

            lstLpnLpnType.ClearSelection();
            lstLpnLpnType.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
            lstLpnLpnType.Items[0].Selected = true;
        }

        public void LoadProveedor(int idOwner, string vendorName)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<Vendor> vendorViewDTO = new GenericViewDTO<Vendor>();

            vendorViewDTO = iWarehousingMGR.GetVendorByNameAndOwner(context, vendorName, idOwner);

            lstProveedor.DataSource = vendorViewDTO.Entities;
            lstProveedor.DataTextField = "Name";
            lstProveedor.DataValueField = "Id";
            lstProveedor.DataBind();

            lstProveedor.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            lstProveedor.Items[0].Selected = true;
                        
        }

        public void LoadTransportista(string carrierName)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<Carrier> carrierViewDTO = new GenericViewDTO<Carrier>();

            carrierViewDTO = iWarehousingMGR.GetCarrierByName(context,carrierName);

            lstTransportista.DataSource = carrierViewDTO.Entities;
            lstTransportista.DataTextField = "Name";
            lstTransportista.DataValueField = "Id";
            lstTransportista.DataBind();

            lstTransportista.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            lstTransportista.Items[0].Selected = true;
                      
        }

        /// <summary>
        /// Carga lista de Carrier
        /// </summary>
        public void LoadCarrier()
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<Carrier> carrierViewDTO = new GenericViewDTO<Carrier>();

            carrierViewDTO = iWarehousingMGR.FindAllCarrier(context);

            lstCarrier.DataSource = carrierViewDTO.Entities;
            lstCarrier.DataTextField = "Name";
            lstCarrier.DataValueField = "Id";
            lstCarrier.DataBind();

            lstCarrier.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            lstCarrier.Items[0].Selected = true;
        }

        public void LoadChofer(string driverName)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<Driver> driverViewDTO = new GenericViewDTO<Driver>();

            driverViewDTO = iWarehousingMGR.GetDriverByName(context, driverName);
            
            lstChofer.DataSource = driverViewDTO.Entities;
            lstChofer.DataTextField = "Name";
            lstChofer.DataValueField = "Id";
            lstChofer.DataBind();

            lstChofer.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            lstChofer.Items[0].Selected = true;

        }

        public void LoadLWmsProcessType()
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<WmsProcess> wmsProcessTypeViewDTO = new GenericViewDTO<WmsProcess>();

            wmsProcessTypeViewDTO = iRulesMGR.FindAllWmsProcess(context);

            if (advancedFilterVisible)
            {
                ddlFilterWmsProcess.DataSource = wmsProcessTypeViewDTO.Entities;

                ddlFilterWmsProcess.DataTextField = "Name";
                ddlFilterWmsProcess.DataValueField = "Code";
                ddlFilterWmsProcess.DataBind();

                ddlFilterWmsProcess.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
                ddlFilterWmsProcess.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Carga el objeto 'Main Filter' con los valores seleccionados en los elementos del control de usuario
        /// </summary>
        public void LoadControlValuesToFilterObject()
        {
            // FILTRO BASICO
            LoadControlValuesBase();

            // FILTRO AVANZADO
            // Solo se utiliza si esta seleccionada la opcion 'Usar Filtro Avanzado'
            if (chkUseAdvancedFilter.Checked && advancedFilterVisible)
            {
                LoadControlValuesAdvanced();
                
                
            }
        }

        private void LoadControlValuesBase()
        {
            int index;
            string dateTo = string.Empty;
            //string minDate = context.CfgParameters[Convert.ToInt16(CfgParameterName.MinDateSupported)].ParameterValue;
            //string maxDate = context.CfgParameters[Convert.ToInt16(CfgParameterName.MaxDateSupported)].ParameterValue;
            string minDate = GetCfgParameter(CfgParameterName.MinDateSupported.ToString());
            string maxDate = GetCfgParameter("MaxDateSupported");

            // Code (genérico)
            if (txtFilterCode.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Code);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterCode.Text.Trim()));
                }
            }

            // Code alternativo (genérico)
            if (txtFilterCodeAlt.Text != string.Empty)
            {
                if (mainFilter.Exists(e => e.Name == CODE_ALT))
                {
                    mainFilter.RemoveAll(e => e.Name == CODE_ALT);
                }

                mainFilter.Add(new EntityFilter() { Name = CODE_ALT, FilterValues = new List<FilterItem>() { new FilterItem() { Name = CODE_ALT, Value = txtFilterCodeAlt.Text.Trim() } } });
            }

            // CodeNumeric (genérico)
            if (txtFilterCodeNumeric.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.CodeNumeric);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterCodeNumeric.Text.Trim()));
                }
            }

            // Name (genérico)
            if (txtFilterName.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Name);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterName.Text.Trim()));
                }
            }

            // Description (genérico)
            if (txtFilterDescription.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Description);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterDescription.Text.Trim()));
                }
            }

            // Warehouse
            index = Convert.ToInt16(EntityFilterName.Warehouse);

            if (!this.warehouseNotIncludeAll)
            {

                if (ddlFilterWarehouse.SelectedIndex != 0 && ddlFilterWarehouse.SelectedIndex != -1)
                {
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterWarehouse.SelectedIndex, ddlFilterWarehouse.SelectedValue));
                    }
                }
                else
                {
                    // Si no se selecciona un Warehouse, filtra por todos los Warehouses asociados al usuario
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        // Agrega -1 para el caso que el usuario no tenga asociado ningún Warehouse
                        mainFilter[index].FilterValues.Add(new FilterItem(0, "-1"));

                        int i = 1;

                        foreach (Warehouse userWhs in context.SessionInfo.User.Warehouses)
                        {
                            mainFilter[index].FilterValues.Add(new FilterItem(i, userWhs.Id.ToString()));
                            i++;
                        }
                    }
                }
            }
            else
            {
                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterWarehouse.SelectedIndex, ddlFilterWarehouse.SelectedValue));
                }
            }

            // Hangar
            if (ddlFilterHangar.SelectedIndex != 0 && ddlFilterHangar.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Hangar);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterHangar.SelectedIndex, ddlFilterHangar.SelectedValue));
                }
            }

            // Owner
            index = Convert.ToInt16(EntityFilterName.Owner);

            if (!this.ownerNotIncludeAll)
            {
                if (ddlFilterOwner.SelectedIndex != 0 && ddlFilterOwner.SelectedIndex != -1)
                {
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterOwner.SelectedIndex, ddlFilterOwner.SelectedValue));
                    }
                }
                else
                {
                    // Si no se selecciona un Owner, filtra por todos los Owners asociados al usuario
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        // Agrega -1 para el caso que el usuario no tenga asociado ningún Owner
                        mainFilter[index].FilterValues.Add(new FilterItem(0, "-1"));

                        int i = 1;

                        foreach (Owner userOwner in context.SessionInfo.User.Owners)
                        {
                            mainFilter[index].FilterValues.Add(new FilterItem(i, userOwner.Id.ToString()));
                            i++;
                        }
                    }
                }
            }
            else
            {
                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterOwner.SelectedIndex, ddlFilterOwner.SelectedValue));
                }
            }

            // Lpn - Type
            if (ddlFilterLpnType.SelectedIndex != 0 && ddlFilterLpnType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.LpnType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterLpnType.SelectedIndex, ddlFilterLpnType.SelectedValue));
                }
            }

            // TruckType
            if (ddlFilterTruckType.SelectedIndex != 0 && ddlFilterTruckType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.TruckType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTruckType.SelectedIndex, ddlFilterTruckType.SelectedValue));
                }
            }

            // WmsProcess Type
            if (listWmsProcessType != null)
            {
                if (listWmsProcessType.Count > 0)
                {
                    index = Convert.ToInt16(EntityFilterName.WmsProcessType);
                    mainFilter[index].FilterValues = new List<FilterItem>();

                    foreach (String item in listWmsProcessType)
                    {
                        mainFilter[index].FilterValues.Add(new FilterItem(item));
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.WmsProcessType);
                mainFilter[index].FilterValues = new List<FilterItem>();
            }

            // MovementType
            if (ddlFilterMovementType.SelectedIndex != 0 && ddlFilterMovementType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.MovementType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterMovementType.SelectedIndex, ddlFilterMovementType.SelectedValue));
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.MovementType);

                // Si no se selecciona un tipo de movimiento, selecciona los que estan por default
                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();

                    // Agrega -1 para el caso que el usuario no tenga asociado ningún Owner
                    //mainFilter[index].FilterValues.Add(new FilterItem(0, "-1"));

                    foreach (ListItem item in ddlFilterMovementType.Items)
                    {
                        mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                    }
                }
            }

            // TaskType
            if (!this.taskTypeNotIncludeAll)
            {
                if (ddlFilterTaskType.SelectedIndex != 0 && ddlFilterTaskType.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.TaskType);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTaskType.SelectedIndex, ddlFilterTaskType.SelectedValue));
                    }
                }
                else
                {
                    index = Convert.ToInt16(EntityFilterName.TaskType);

                    // Si no se selecciona un tipo de tarea, selecciona los que estan por default
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        // Agrega -1 para el caso que el usuario no tenga asociado ningún Owner
                        //mainFilter[index].FilterValues.Add(new FilterItem(0, "-1"));

                        foreach (ListItem item in ddlFilterTaskType.Items)
                        {
                            if (item.Value != "-1")
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.TaskType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTaskType.SelectedIndex, ddlFilterTaskType.SelectedValue));
                }
            }

            // TrackTaskType
            if (!this.trackTaskTypeNotIncludeAll)
            {
                if (ddlFilterTrackTaskType.SelectedIndex != 0 && ddlFilterTrackTaskType.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.TrackTaskType);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTrackTaskType.SelectedIndex, ddlFilterTrackTaskType.SelectedValue));
                    }
                }
                else
                {
                    index = Convert.ToInt16(EntityFilterName.TrackTaskType);

                    // Si no se selecciona un tipo de tarea, selecciona los que estan por default
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        // Agrega -1 para el caso que el usuario no tenga asociado ningún Owner
                        //mainFilter[index].FilterValues.Add(new FilterItem(0, "-1"));

                        foreach (ListItem item in ddlFilterTrackTaskType.Items)
                        {
                            if (item.Value != "-1")
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.TrackTaskType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTrackTaskType.SelectedIndex, ddlFilterTrackTaskType.SelectedValue));
                }
            }


            // Scope
            if (ddlFilterScope.SelectedIndex != 0 && ddlFilterScope.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Scope);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterScope.SelectedIndex, ddlFilterScope.SelectedValue));
                }
            }

            // Status
            if (ddlFilterStatus.SelectedIndex != 0 && ddlFilterStatus.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Status);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterStatus.SelectedIndex, ddlFilterStatus.SelectedValue));
                }
            }

            // Transaction Status
            if (ddlFilterTransactionStatus.SelectedIndex != 0 && ddlFilterTransactionStatus.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Status);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTransactionStatus.SelectedIndex, ddlFilterTransactionStatus.SelectedValue));
                }
            }

            // SimpliRouteVisit Status
            if (ddlFilterSimpliRouteVisitStatus.SelectedIndex != 0 && ddlFilterSimpliRouteVisitStatus.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Status);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterSimpliRouteVisitStatus.SelectedIndex, ddlFilterSimpliRouteVisitStatus.SelectedValue));
                }
            }

            // Item
            if (txtFilterItem.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Item);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterItem.Text.Trim()));
                }
            }

            // UomType
            if (ddlFilterUomType.SelectedIndex != 0 && ddlFilterUomType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.UomType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterUomType.SelectedIndex, ddlFilterUomType.SelectedValue));
                }
            }

            // MaxStock
            if (!String.IsNullOrEmpty(txtMaxStock.Text) || !String.IsNullOrEmpty(txtPorcentajeUso.Text))
            {
                
                if (!String.IsNullOrEmpty(txtMaxStock.Text))
                {
                    index = Convert.ToInt16(EntityFilterName.MaximumStock);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtMaxStock.Text.Trim()));
                    }
                }
                else
                {
                    index = Convert.ToInt16(EntityFilterName.PercentageUsed);
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtPorcentajeUso.Text.Trim()));
                    }
                }               
            }


            // Document Nbr
            if (txtFilterDocumentNumber.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.DocumentNbr);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterDocumentNumber.Text.Trim()));
                }
            }


            /*

            // Id Truck Code
            if (txtFilterIdTruckCode.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.IdTruckCode);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterIdTruckCode.Text.Trim()));
                }
            }
            */
            // OutboundType
            if (!this.outboundTypeNotIncludeAll)
            {
                if (ddlFilterOutboundType.SelectedIndex != 0 && ddlFilterOutboundType.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.OutboundType);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterOutboundType.SelectedIndex, ddlFilterOutboundType.SelectedValue));
                    }
                }
                else
                {
                    index = Convert.ToInt16(EntityFilterName.OutboundType);

                    // Si no se selecciona un tipo de tarea, selecciona los que estan por default
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        foreach (ListItem item in ddlFilterOutboundType.Items)
                        {
                            if (item.Value != "-1")
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.OutboundType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterOutboundType.SelectedIndex, ddlFilterOutboundType.SelectedValue));
                }
            }

            // TrackOutboundType
            if (!this.trackOutboundTypeNotIncludeAll)
            {
                if (ddlFilterTrackOutboundType.SelectedIndex != 0 && ddlFilterTrackOutboundType.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.TrackOutboundType);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTrackOutboundType.SelectedIndex, ddlFilterTrackOutboundType.SelectedValue));
                    }
                }
                else
                {
                    index = Convert.ToInt16(EntityFilterName.TrackOutboundType);

                    // Si no se selecciona un tipo de tarea, selecciona los que estan por default
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        foreach (ListItem item in ddlFilterTrackOutboundType.Items)
                        {
                            if (item.Value != "-1")
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.TrackOutboundType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTrackOutboundType.SelectedIndex, ddlFilterTrackOutboundType.SelectedValue));
                }
            }

            // TrackInboundType
            if (!this.trackInboundTypeNotIncludeAll)
            {
                if (ddlFilterTrackInboundType.SelectedIndex != 0 && ddlFilterTrackInboundType.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.TrackInboundType);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTrackInboundType.SelectedIndex, ddlFilterTrackInboundType.SelectedValue));
                    }
                }
                else
                {
                    index = Convert.ToInt16(EntityFilterName.TrackInboundType);

                    // Si no se selecciona un tipo de tarea, selecciona los que estan por default
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        foreach (ListItem item in ddlFilterTrackInboundType.Items)
                        {
                            if (item.Value != "-1")
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.TrackInboundType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTrackInboundType.SelectedIndex, ddlFilterTrackInboundType.SelectedValue));
                }
            }
            // Traza
            if (!this.dispatchTypeNotIncludeAll)
            {
                if (ddlFilterDispatchType.SelectedIndex != 0 && ddlFilterDispatchType.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.DispatchType);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterDispatchType.SelectedIndex, ddlFilterDispatchType.SelectedValue));
                    }
                }
                else
                {
                    index = Convert.ToInt16(EntityFilterName.DispatchType);

                    // Si no se selecciona un tipo de tarea, selecciona los que estan por default
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        foreach (ListItem item in ddlFilterDispatchType.Items)
                        {
                            if (item.Value != "-1")
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.DispatchType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterDispatchType.SelectedIndex, ddlFilterDispatchType.SelectedValue));
                }
            }

            // Date (genéric)
            if ((txtFilterDate.Text != string.Empty ) && (chkFilterDate.Checked))
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);

                // Rango de Fechas por defecto para la página actual
                //int indexDaysBefore = Convert.ToInt16(dateBefore);
                int defaultDaysBefore = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));
                //string defaultDate = DateTime.Now.AddDays(-defaultDaysBefore).ToShortDateString();
                string defaultDate = DateTime.Now.AddDays(-defaultDaysBefore).ToString(FormatDate);
                
                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();

                    if (chkFilterDate.Checked)
                    {
                        try
                        {
                            // Valida Fecha Desde
                            //Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                            if (Convert.ToDateTime(txtFilterDate.Text.Trim()) < Convert.ToDateTime(minDate))
                            {
                                mainFilter[index].FilterValues.Add(new FilterItem(0, minDate));
                                txtFilterDate.Text = minDate;
                            }
                            else
                            {
                                // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fecha máxima al control
                                if (Convert.ToDateTime(txtFilterDate.Text.Trim()) > Convert.ToDateTime(maxDate))
                                {
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, maxDate));
                                    txtFilterDate.Text = maxDate;
                                }
                                else
                                {
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(txtFilterDate.Text.Trim()).ToString(FormatDate)));
                                }
                            }
                            dateTo = Convert.ToDateTime(txtFilterDate.Text).AddDays(1).Add(System.TimeSpan.FromSeconds(-60)).ToString(FormatDateTime);
                            mainFilter[index].FilterValues.Add(new FilterItem(0, dateTo));
                            
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, asigna la fecha por defecto para la consulta actual
                            mainFilter[index].FilterValues.Add(new FilterItem(0, defaultDate));
                            txtFilterDate.Text = defaultDate;
                        }
                    }
                    else
                        mainFilter[index].FilterValues.Add(new FilterItem(0, string.Empty));

                    
                }
            }

            // DateYear (genéric)
            if ((txtFilterDateYear.Text != string.Empty) && (chkFilterDateYear.Checked))
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);
                

                // Rango de Fechas por defecto para la página actual
                //int indexDaysBefore = Convert.ToInt16(dateBefore);
                int defaultDaysBefore = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));
                string defaultDate = DateTime.Now.AddDays(-defaultDaysBefore).ToShortDateString();

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();

                    if (chkFilterDateYear.Checked)
                    {
                        try
                        {
                            DateTime tiempoDesde = Convert.ToDateTime("01-01-" + txtFilterDateYear.Text.Trim());
                            DateTime tiempoHasta = Convert.ToDateTime("31-12-" + txtFilterDateYear.Text.Trim());
                            mainFilter[index].FilterValues.Add(new FilterItem(0, tiempoDesde.ToString(FormatDate)));
                            mainFilter[index].FilterValues.Add(new FilterItem(1, tiempoHasta.ToString(FormatDate)));
                            //mainFilter[index].FilterValues.Add(new FilterItem(0, tiempoDesde.ToShortDateString()));
                            //mainFilter[index].FilterValues.Add(new FilterItem(1, tiempoHasta.ToShortDateString()));

                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, asigna la fecha por defecto para la consulta actual
                            DateTime tiempoDesde = Convert.ToDateTime("01-01-" + DateTime.Now.Year.ToString());
                            DateTime tiempoHasta = Convert.ToDateTime("31-12-" + DateTime.Now.Year.ToString());
                            mainFilter[index].FilterValues.Add(new FilterItem(0, tiempoDesde.ToShortDateString()));
                            mainFilter[index].FilterValues.Add(new FilterItem(1, tiempoHasta.ToShortDateString()));
                            //mainFilter[index].FilterValues.Add(new FilterItem(0, tiempoDesde.ToShortDateString()));
                            //mainFilter[index].FilterValues.Add(new FilterItem(1, tiempoHasta.ToShortDateString()));
                        }
                    }
                    else
                        mainFilter[index].FilterValues.Add(new FilterItem(0, string.Empty));


                }
            }

            // Year (genéric)
            if (txtFilterYear.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);


                // Rango de Fechas por defecto para la página actual
                //int indexDaysBefore = Convert.ToInt16(dateBefore);
                int defaultDaysBefore = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));
                string defaultDate = DateTime.Now.AddDays(-defaultDaysBefore).ToShortDateString();

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
         
                    try
                    {
                        string AnoDesde = (int.Parse(txtFilterDateYear.Text.Trim()) - 5).ToString();
                        string AnoHasta = (txtFilterDateYear.Text.Trim());
                        mainFilter[index].FilterValues.Add(new FilterItem(0, AnoDesde));
                        mainFilter[index].FilterValues.Add(new FilterItem(1, AnoHasta));

                    }
                    catch
                    {
                        // Si el valor ingresado no es válido, asigna el Año por defecto para la consulta actual
                        string tiempoDesde = (DateTime.Now.AddYears(-5).Year.ToString());
                        string tiempoHasta =  (DateTime.Now.Year.ToString());
                        mainFilter[index].FilterValues.Add(new FilterItem(0, tiempoDesde));
                        mainFilter[index].FilterValues.Add(new FilterItem(1, tiempoHasta));
                    }
                }
            }

            // Dates From and To (genéric)
            if ((txtFilterDateFrom.Text != string.Empty || txtFilterDateTo.Text != string.Empty) && (chkFilterDateFrom.Checked || chkFilterDateTo.Checked))
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);

                // Rango de Fechas por defecto para la página actual
                //int indexDaysBefore = Convert.ToInt16(dateBefore);
                int defaultDaysBefore = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));
                string defaultDateFrom = DateTime.Now.AddDays(-defaultDaysBefore).ToShortDateString();

                //int indexDaysAfter = Convert.ToInt16(dateAfter);
                int defaultDaysAfter = Convert.ToInt16(GetCfgParameter(dateAfter.ToString()));
                string defaultDateTo = DateTime.Now.AddDays(-defaultDaysAfter).ToShortDateString();

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();

                    if (chkFilterDateFrom.Checked)
                    {
                        try
                        {
                            // Valida Fecha Desde
                            //Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                            if (Convert.ToDateTime(txtFilterDateFrom.Text) < Convert.ToDateTime(minDate))
                            {
                                mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(minDate).ToString(FormatDate)));
                                txtFilterDateFrom.Text = minDate;
                            }
                            else
                            {
                                //Valida que la fecha desde no sea mayor que la fecha hasta.
                                if (Convert.ToDateTime(txtFilterDateFrom.Text) > Convert.ToDateTime(txtFilterDateTo.Text))
                                {
                                    //Lanza un mensaje al usuario
                                    //txtFilterDateFrom.Text = txtFilterDateTo.Text;
                                    throw new Exception("La fecha desde no puede ser más grande que la fecha hasta");
                                }
                                // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fecha máxima al control
                                if (Convert.ToDateTime(txtFilterDateFrom.Text) > Convert.ToDateTime(maxDate))
                                {
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(maxDate).ToString(FormatDate)));
                                    txtFilterDateFrom.Text = maxDate;
                                }
                                else
                                {   
                                    //stg - Se modifica fecha 'desde' para que cumpla formato del servidor
                                    //stg - txtFilterDateFrom.Text
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(txtFilterDateFrom.Text.Trim()).ToString(FormatDate)));
                                }
                            }
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, asigna la fecha por defecto para la consulta actual
                            mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(defaultDateFrom).ToString(FormatDate)));
                            txtFilterDateFrom.Text = defaultDateFrom;
                        }
                    }
                    else
                        mainFilter[index].FilterValues.Add(new FilterItem(0, string.Empty));

                    if (chkFilterDateTo.Checked)
                    {
                        try
                        {
                            // Valida Fecha Hasta
                            // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fecha máxima al control
                            if (Convert.ToDateTime(txtFilterDateTo.Text) > Convert.ToDateTime(maxDate))
                            {
                                dateTo = maxDate;
                                txtFilterDateTo.Text = maxDate;
                            }
                            else
                            {
                                if (txtFilterDateTo.Text != string.Empty)
                                {
                                    // Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                                    if (Convert.ToDateTime(txtFilterDateTo.Text) < Convert.ToDateTime(minDate))
                                    {
                                        dateTo = minDate;
                                        txtFilterDateTo.Text = minDate;
                                    }
                                    else
                                    {
                                        // Agrega un dia menos 1 segundo a 'Date To' para abarcar todo el dia seleccionado
                                        //stg - se añade formato de fecha
                                        dateTo = Convert.ToDateTime(txtFilterDateTo.Text.Trim()).AddDays(1).Add(System.TimeSpan.FromSeconds(-60)).ToString(FormatDateTime);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, asigna la fecha por defecto para la consulta actual
                            dateTo = defaultDateTo;
                            txtFilterDateTo.Text = defaultDateTo;
                        }

                        mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(dateTo).ToString(FormatDateTime)));
                        dateTo = string.Empty;
                    }
                    else
                        mainFilter[index].FilterValues.Add(new FilterItem(0, string.Empty));
                }
            }

            // Location Type
            if (ddlFilterLocationType.SelectedIndex != 0 && ddlFilterLocationType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.LocationType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();

                    foreach (ListItem item in ddlFilterLocationType.Items)
                    {
                        if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                    }
                }
            }


            // Zone Type
            if (ddlFilterZoneType.SelectedIndex != 0 && ddlFilterZoneType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.ZoneType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();

                    foreach (ListItem item in ddlFilterZoneType.Items)
                    {
                        if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                    }
                }
            }

            // Reason
            if (ddlFilterReason.SelectedIndex != 0 && ddlFilterReason.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Reason);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterReason.SelectedIndex, ddlFilterReason.SelectedValue));
                }
            }
            else
            {
                index = Convert.ToInt16(EntityFilterName.Reason);

                // Si no se selecciona una Razon, selecciona los que estan por default
                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();

                    // Agrega -1 para el caso que el usuario no tenga asociado ninguna razon
                    //mainFilter[index].FilterValues.Add(new FilterItem(0, "-1"));

                    foreach (ListItem item in ddlFilterReason.Items)
                    {
                        mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                    }
                }
            }


            // Kardex Type
            if (ddlFilterKardexType.SelectedIndex != 0 && ddlFilterKardexType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.KardexType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();

                    foreach (ListItem item in ddlFilterKardexType.Items)
                    {
                        if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                    }
                }
            }


            if (Convert.ToInt16(Request.QueryString["IT"]) != ((int)InboundTypeName.Todos))
            {
                //InboundType (todos los tipos)
                if (ddlFilterInboundType.SelectedIndex != 0)
                {
                    index = Convert.ToInt16(EntityFilterName.InboundType);
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        foreach (ListItem item in ddlFilterInboundType.Items)
                            if (item.Selected)
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                    }
                }
                else
                {
                    index = Convert.ToInt16(EntityFilterName.InboundType);
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        foreach (ListItem item in ddlFilterInboundType.Items)
                            if (item.Value != "-1")
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                    }
                }
            }
            else
            {
                if (ddlFilterInboundType.SelectedIndex != 0)
                {
                    index = Convert.ToInt16(EntityFilterName.InboundType);
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        foreach (ListItem item in ddlFilterInboundType.Items)
                            if (item.Selected)
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                    }
                }
            }

            // ReferenceDocType
            if (ddlFilterReferenceDocType.SelectedIndex != 0 && ddlFilterReferenceDocType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.ReferenceDocType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterReferenceDocType.SelectedIndex, ddlFilterReferenceDocType.SelectedValue));
                }
            }
                                   

            //Translations
            #region Translations
            if (ddlTranslate.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Translate);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlTranslate.SelectedIndex, ddlTranslate.SelectedValue));
                }
            }

            if (ddlModule.SelectedIndex != 0 && ddlModule.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Module);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlModule.SelectedIndex, ddlModule.SelectedValue));
                }
            }

            if (ddlTypeObject.SelectedIndex != 0 && ddlTypeObject.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.ObjectType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlTypeObject.SelectedIndex, ddlTypeObject.SelectedValue));
                }
            }

            if (ddlProperty.SelectedIndex != 0 && ddlProperty.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.PropertyName);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlProperty.SelectedIndex, ddlProperty.SelectedValue));
                }
            }
            if (ddlObjectContainer.SelectedIndex != 0 && ddlObjectContainer.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.ObjectContainer);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlObjectContainer.SelectedIndex, ddlObjectContainer.SelectedValue));
                }
            }

            #endregion


            // Goup Items
            if (this.divBasicItemGroupVisible)
            {
                // Group Item 1
                if (ddlBscGrpItm1.SelectedIndex != 0 && ddlBscGrpItm1.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem1);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlBscGrpItm1.SelectedIndex, ddlBscGrpItm1.SelectedValue));
                    }
                }

                // Group Item 2
                if (ddlBscGrpItm2.SelectedIndex != 0 && ddlBscGrpItm2.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem2);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlBscGrpItm2.SelectedIndex, ddlBscGrpItm2.SelectedValue));
                    }
                }

                // Group Item 3
                if (ddlBscGrpItm3.SelectedIndex != 0 && ddlBscGrpItm3.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem3);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlBscGrpItm3.SelectedIndex, ddlBscGrpItm3.SelectedValue));
                    }
                }

                // Group Item 4
                if (ddlBscGrpItm4.SelectedIndex != 0 && ddlBscGrpItm4.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem4);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlBscGrpItm4.SelectedIndex, ddlBscGrpItm4.SelectedValue));
                    }
                }

            }

            // Type Kardex
            if (ddlFilterKardexType.SelectedIndex != 0 && ddlFilterKardexType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.KardexType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterKardexType.SelectedIndex, ddlFilterKardexType.SelectedValue));
                }
            }


            // WmsProcess
            if (this.ddlFilterWmsProcess.SelectedIndex != 0 && ddlFilterWmsProcess.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.WmsProcessType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterWmsProcess.SelectedIndex, ddlFilterWmsProcess.SelectedValue));
                }
            }

            if (divCountryFilterVisible)
            {
                if (context.MainFilter.Exists(e => e.Name == ID_COUNTRY))
                {
                    context.MainFilter.RemoveAll(e => e.Name == ID_COUNTRY);
                }

                if (ddlCountryFilter.SelectedIndex != 0 && ddlCountryFilter.SelectedIndex != -1)
                {
                    //index = Convert.ToInt16(EntityFilterName.Country);

                    //if (mainFilter[index].FilterValues != null)
                    //{
                    //    mainFilter[index].FilterValues.Clear();
                    //    mainFilter[index].FilterValues.Add(new FilterItem(ddlCountryFilter.SelectedIndex, ddlCountryFilter.SelectedValue));
                    //}

                    context.MainFilter.Add(new EntityFilter() { Name = ID_COUNTRY, FilterValues = new List<FilterItem>() { new FilterItem() { Name = ID_COUNTRY, Value = ddlCountryFilter.SelectedValue } } });
                }
            }

            if (divStateFilterVisible)
            {
                if (context.MainFilter.Exists(e => e.Name == ID_STATE))
                {
                    context.MainFilter.RemoveAll(e => e.Name == ID_STATE);
                }

                if (ddlStateFilter.SelectedIndex != 0 && ddlStateFilter.SelectedIndex != -1)
                {
                    //index = Convert.ToInt16(EntityFilterName.State);

                    //if (mainFilter[index].FilterValues != null)
                    //{
                    //    mainFilter[index].FilterValues.Clear();
                    //    mainFilter[index].FilterValues.Add(new FilterItem(ddlStateFilter.SelectedIndex, ddlStateFilter.SelectedValue));
                    //}

                    context.MainFilter.Add(new EntityFilter() { Name = ID_STATE, FilterValues = new List<FilterItem>() { new FilterItem() { Name = ID_STATE, Value = ddlStateFilter.SelectedValue } } });
                }
            }

            if (divCityFilterVisible)
            {
                if (context.MainFilter.Exists(e => e.Name == ID_CITY))
                {
                    context.MainFilter.RemoveAll(e => e.Name == ID_CITY);
                }

                if (ddlCityFilter.SelectedIndex != 0 && ddlCityFilter.SelectedIndex != -1)
                {
                    context.MainFilter.Add(new EntityFilter() { Name = ID_CITY, FilterValues = new List<FilterItem>() { new FilterItem() { Name = ID_CITY, Value = ddlCityFilter.SelectedValue } } });
                }
            }

            if (tabMultipleChoiceTrackTaskFiltersVisible)
            {
                if (context.MainFilter.Exists(e => e.Name == ID_TRACK_TASK_QUEUE))
                {
                    context.MainFilter.RemoveAll(e => e.Name == ID_TRACK_TASK_QUEUE);
                }

                //if (ddlTaskQueueFilter.SelectedIndex != 0 && ddlTaskQueueFilter.SelectedIndex != -1)
                //{
                //    context.MainFilter.Add(new EntityFilter() { Name = ID_TRACK_TASK_QUEUE, FilterValues = new List<FilterItem>() { new FilterItem() { Name = ID_TRACK_TASK_QUEUE, Value = ddlTaskQueueFilter.SelectedValue } } });
                //}
                if (lstLstTrackTask.SelectedIndex != 0 && lstLstTrackTask.SelectedIndex != -1)
                {
                    context.MainFilter.Add(new EntityFilter() { Name = ID_TRACK_TASK_QUEUE, FilterValues = new List<FilterItem>() { new FilterItem() { Name = ID_TRACK_TASK_QUEUE, Value = lstLstTrackTask.SelectedValue } } });
                }



                if (txtPercentFrom.Text != string.Empty || txtPercentTo.Text != string.Empty)
                {
                    if (context.MainFilter.Exists(e => e.Name == PERCENT_RANGE))
                    {
                        context.MainFilter.RemoveAll(e => e.Name == PERCENT_RANGE);
                    }

                    context.MainFilter.Add(new EntityFilter() { Name = PERCENT_RANGE, FilterValues = new List<FilterItem>() 
                        {   new FilterItem( 0,txtPercentFrom.Text.Trim()), 
                            new FilterItem( 0,txtPercentTo.Text.Trim())} 
                         });
                    
                }
            }

            if (divRotationItemFilterVisible)
            {
                if (context.MainFilter.Exists(e => e.Name == ID_ROTATION_TYPE_FILTER))
                {
                    context.MainFilter.RemoveAll(e => e.Name == ID_ROTATION_TYPE_FILTER);
                }

                context.MainFilter.Add(new EntityFilter() { Name = ID_ROTATION_TYPE_FILTER, FilterValues = new List<FilterItem>() { new FilterItem() { Name = ID_ROTATION_TYPE_FILTER, Value = ddlRotationItemFilter.SelectedValue } } });
            }

            //Location Hold
            if (chkFilterLockedLocationVisible)
            {
                if (this.chkFilterLockedLocation.Checked)
                {
                    if (MainFilter.Exists(filter => filter.Name == LOCKED_LOCATION_FILTER))
                    {
                        MainFilter.RemoveAll(filter => filter.Name == LOCKED_LOCATION_FILTER);
                    }

                    MainFilter.Add(new EntityFilter() { Name = LOCKED_LOCATION_FILTER, FilterValues = new List<FilterItem>() { new FilterItem(0, "1") } });
                    
                }
            }

            if (logicalWarehouseVisible)
            {
                index = Convert.ToInt16(EntityFilterName.LogicalWarehouse);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterLogicalWarehouse.SelectedIndex, ddlFilterLogicalWarehouse.SelectedItem.Text.Trim()));
                }
            }
        }

        private void LoadControlValuesAdvanced()
        {
            int index;
            string dateTo = string.Empty;
            //string minDate = context.CfgParameters[Convert.ToInt16(CfgParameterName.MinDateSupported)].ParameterValue;
            //string maxDate = context.CfgParameters[Convert.ToInt16(CfgParameterName.MaxDateSupported)].ParameterValue;
            string minDate = GetCfgParameter(CfgParameterName.MinDateSupported.ToString()); ;
            string maxDate = GetCfgParameter(CfgParameterName.MaxDateSupported.ToString());

            context.SessionInfo.FilterUseAdvanced = true;

            // DISPATCHING TAB
            if (this.tabDispatchingVisible)
            {
                // Priority Range
                if (txtPriorityFrom.Text != string.Empty || txtPriorityTo.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.PriorityRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtPriorityFrom.Text.Trim()));
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtPriorityTo.Text.Trim()));
                    }
                }

                // Customer
                if (txtCustomer.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.Customer);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(txtCustomer.Text.Trim()));
                    }
                }

                // Carrier
                if (txtCarrier.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.Carrier);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(txtCarrier.Text.Trim()));
                    }
                }

                // Route
                if (txtRoute.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.Route);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(txtRoute.Text.Trim()));
                    }
                }

                // Country
                if (ddlCountry.SelectedIndex != 0 && ddlCountry.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.Country);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlCountry.SelectedIndex, ddlCountry.SelectedValue));
                    }
                }

                // State
                if (ddlState.SelectedIndex != 0 && ddlState.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.State);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlState.SelectedIndex, ddlState.SelectedValue));
                    }
                }

                // City
                if (ddlCity.SelectedIndex != 0 && ddlCity.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.City);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlCity.SelectedIndex, ddlCity.SelectedValue));
                    }
                }
            }

            // LAYOUT TAB
            if (this.tabLayoutVisible)
            {
                // Hangar
                if (lstHangar.SelectedIndex != 0 && lstHangar.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.Hangar);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        foreach (ListItem item in lstHangar.Items)
                        {
                            if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }

                // WorkZone
                if (lstWorkZone.SelectedIndex != 0 && lstWorkZone.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.WorkZone);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        foreach (ListItem item in lstWorkZone.Items)
                        {
                            if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }

                // Location Type
                if (!lstLocationType.Items[0].Selected)
                {
                    index = Convert.ToInt16(EntityFilterName.LocationType);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        foreach (ListItem item in lstLocationType.Items)
                        {
                            if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }
            }

            // LOCATION TAB
            if (this.tabLocationVisible)
            {
                // Location
                if (txtLocation.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.Location);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(txtLocation.Text.Trim()));
                    }
                }

                // Location Range
                if (txtLocationFrom.Text != string.Empty || txtLocationTo.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LocationRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationFrom.Text.Trim()));
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationTo.Text.Trim()));
                    }
                }

                // Location Row
                if (txtLocationRow.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LocationRow);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationRow.Text.Trim()));
                    }
                }

                // Location Column
                if (txtLocationColumn.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LocationColumn);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationColumn.Text.Trim()));
                    }
                }

                // Location Level
                if (txtLocationLevel.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LocationLevel);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationLevel.Text.Trim()));
                    }
                }

                // Location Aisle
                if (txtLocationAisle.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LocationAisle);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationAisle.Text.Trim()));
                    }
                }

                // Location Row Range
                if (ddlLocRowRangeTo.SelectedIndex != 0 && ddlLocRowRangeFrom.SelectedIndex != 0
                    && ddlLocRowRangeTo.SelectedIndex != -1 && ddlLocRowRangeFrom.SelectedIndex != -1)
                {
                    IsValidFilterLocation = true;
                    GenericViewDTO<Auxiliary> listAuxViewDTO = new GenericViewDTO<Auxiliary>();
                    index = Convert.ToInt16(EntityFilterName.LocationRowRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, ddlLocRowRangeFrom.SelectedValue));
                        mainFilter[index].FilterValues.Add(new FilterItem(0, ddlLocRowRangeTo.SelectedValue));
                    }
                }
                // Location Column Range
                if (ddlLocColumnRangeTo.SelectedIndex != 0 && ddlLocColumnRangeFrom.SelectedIndex != 0
                    && ddlLocColumnRangeTo.SelectedIndex != -1 && ddlLocColumnRangeFrom.SelectedIndex != -1)
                {
                    GenericViewDTO<Auxiliary> listAuxViewDTO = new GenericViewDTO<Auxiliary>();
                    IsValidFilterLocation = true;
                    index = Convert.ToInt16(EntityFilterName.LocationColumnRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, ddlLocColumnRangeFrom.SelectedValue));
                        mainFilter[index].FilterValues.Add(new FilterItem(0, ddlLocColumnRangeTo.SelectedValue));
                    }
                }
                // Location Level Range
                if (ddlLocLevelRangeTo.SelectedIndex != 0 && ddlLocLevelRangeFrom.SelectedIndex != 0
                    && ddlLocLevelRangeTo.SelectedIndex != -1 && ddlLocLevelRangeFrom.SelectedIndex != -1)
                {
                    GenericViewDTO<Auxiliary> listAuxViewDTO = new GenericViewDTO<Auxiliary>();
                    IsValidFilterLocation = true;

                    index = Convert.ToInt16(EntityFilterName.LocationLevelRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, ddlLocLevelRangeFrom.SelectedValue));
                        mainFilter[index].FilterValues.Add(new FilterItem(0, ddlLocLevelRangeTo.SelectedValue));
                    }

                }

                //Location Hold
                if (this.chkLockedLocation.Checked)
                {
                    if (MainFilter.Exists(filter => filter.Name == LOCKED_LOCATION))
                    {
                        MainFilter.RemoveAll(filter => filter.Name == LOCKED_LOCATION);
                    }

                    MainFilter.Add(new EntityFilter() { Name = LOCKED_LOCATION, FilterValues = new List<FilterItem>() { new FilterItem(0, "1") } });
                }
            }
            
            // RECEPTION LOG TAB
            if (this.tabReceptionLogVisible)
            {
                if (txtItemCode.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.Item);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtItemCode.Text.Trim()));
                    }
                }

                if (txtItemName.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.Description);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtItemName.Text.Trim()));
                    }
                }

                if (txtDocumentNbr.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.DocumentNbr);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtDocumentNbr.Text.Trim()));
                    }
                }

                if (txtReferenceNbr.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.ReferenceNbr);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtReferenceNbr.Text.Trim()));
                    }
                }

                if (txtOperator.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.Driver);
                    
                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtOperator.Text.Trim()));
                    }
                }

                if (txtSourceLocation.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LocationSource);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtSourceLocation.Text.Trim()));
                    }
                }

                if (txtTargetLocation.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LocationTarget);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtTargetLocation.Text.Trim()));
                    }
                }

                if (txtSourceLpn.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LpnSource);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtSourceLpn.Text.Trim()));
                    }
                }

                if (txtTargetLpn.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LpnTarget);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtTargetLpn.Text.Trim()));
                    }
                }

                if (txtPriorityTask.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.PriorityTask);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtPriorityTask.Text.Trim()));
                    }
                }
            }

            //Tab GS1
            if (this.tabGS1Visible)
            {
                if (txtGtinTabGs1.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.Item);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtGtinTabGs1.Text.Trim()));
                    }
                }

                if (txtGsinTabGs1.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LpnParent);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtGsinTabGs1.Text.Trim()));
                    }
                }

                if (txtLotNumberTabGs1.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LotNumberRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtGsinTabGs1.Text.Trim()));
                    }
                }

                if (txtFabricationDateTabGs1.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.FabricationDateRange);

                    try
                    {
                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(txtFabricationDateTabGs1.Text.Trim()).ToString(FormatDate)));
                        }
                    }
                    catch
                    {
                        // Si el valor ingresado no es válido, deja en blanco la fecha
                        mainFilter[index].FilterValues.Add(new FilterItem(0, string.Empty));
                        txtFabricationDateTabGs1.Text = string.Empty;
                    }
                }

                if (txtExpirationDateTabGs1.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.ExpirationDateRange);

                    try
                    {
                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(txtExpirationDateTabGs1.Text.Trim()).ToString(FormatDate)));
                        }
                    }
                    catch
                    {
                        // Si el valor ingresado no es válido, deja en blanco la fecha
                        mainFilter[index].FilterValues.Add(new FilterItem(0, string.Empty));
                        txtExpirationDateTabGs1.Text = string.Empty;
                    }

                }
            }


            // TASK GROUP TAB
            if (this.tabTaskVisible)
            {
                // IsComplete
                if (chkComplete.Checked == true)
                {
                    index = Convert.ToInt16(EntityFilterName.IsComplete);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, "1"));
                    }
                }
                else
                {
                    // IsNotComplete
                    if (chkNotComplete.Checked == true)
                    {
                        index = Convert.ToInt16(EntityFilterName.IsComplete);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            mainFilter[index].FilterValues.Add(new FilterItem(0, "0"));
                        }
                    }
                }
            }
            // ITEM GROUP TAB
            if (this.tabItemGroupVisible)
            {
                // Group Item 1
                if (ddlGrpItem1.SelectedIndex != 0 && ddlGrpItem1.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem1);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlGrpItem1.SelectedIndex, ddlGrpItem1.SelectedValue));
                    }
                }

                // Group Item 2
                if (ddlGrpItem2.SelectedIndex != 0 && ddlGrpItem2.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem2);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlGrpItem2.SelectedIndex, ddlGrpItem2.SelectedValue));
                    }
                }

                // Group Item 3
                if (ddlGrpItem3.SelectedIndex != 0 && ddlGrpItem3.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem3);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlGrpItem3.SelectedIndex, ddlGrpItem3.SelectedValue));
                    }
                }

                // Group Item 4
                if (ddlGrpItem4.SelectedIndex != 0 && ddlGrpItem4.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem4);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlGrpItem4.SelectedIndex, ddlGrpItem4.SelectedValue));
                    }
                }

            }

            // DATE TAB
            if (this.tabDatesVisible)
            {
                // Fabrication Date
                if (txtFabricationDateFrom.Text != string.Empty || txtFabricationDateTo.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.FabricationDateRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        try
                        {
                            // Valida Fecha Desde
                            // Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                            if (Convert.ToDateTime(txtFabricationDateFrom.Text) < Convert.ToDateTime(minDate))
                            {
                                mainFilter[index].FilterValues.Add(new FilterItem(0, minDate));
                                txtFabricationDateFrom.Text = minDate;
                            }
                            else
                            {
                                // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fecha máxima al control
                                if (Convert.ToDateTime(txtFabricationDateFrom.Text) > Convert.ToDateTime(maxDate))
                                {
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, maxDate));
                                    txtFabricationDateFrom.Text = maxDate;
                                }
                                else
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(txtFabricationDateFrom.Text.Trim()).ToString(FormatDate)));
                            }
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, deja en blanco la fecha
                            mainFilter[index].FilterValues.Add(new FilterItem(0, string.Empty));
                            txtFabricationDateFrom.Text = string.Empty;
                        }

                        try
                        {
                            // Valida Fecha Hasta
                            // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fechamáxima control
                            if (Convert.ToDateTime(txtFabricationDateTo.Text) > Convert.ToDateTime(maxDate))
                            {
                                dateTo = maxDate;
                                txtFabricationDateTo.Text = maxDate;
                            }
                            else
                            {
                                if (txtFabricationDateTo.Text != string.Empty)
                                {
                                    // Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                                    if (Convert.ToDateTime(txtFabricationDateTo.Text) < Convert.ToDateTime(minDate))
                                    {
                                        dateTo = minDate;
                                        txtFabricationDateTo.Text = minDate;
                                    }
                                    else
                                    {
                                        // Agrega un dia menos 1 segundo a 'Date To' para abarcar todo el dia seleccionado
                                        dateTo = Convert.ToDateTime(txtFabricationDateTo.Text.Trim()).AddDays(1).Add(System.TimeSpan.FromSeconds(-60)).ToString(FormatDateTime);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, deja en blanco la fecha
                            dateTo = string.Empty;
                            txtFabricationDateTo.Text = string.Empty; 
                        }


                        mainFilter[index].FilterValues.Add(new FilterItem(0, dateTo));
                        dateTo = string.Empty;

                        // Si ambos filtros quedaron en blanco luego de las validaciones, los elimina
                        if (mainFilter[index].FilterValues[0].Value == string.Empty && mainFilter[index].FilterValues[1].Value == string.Empty)
                            mainFilter[index].FilterValues.Clear();
                    }
                }

                // Expiration Date
                if (txtExpirationDateFrom.Text != string.Empty || txtExpirationDateTo.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.ExpirationDateRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        try
                        {
                            // Valida Fecha Desde
                            // Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                            if (Convert.ToDateTime(txtExpirationDateFrom.Text) < Convert.ToDateTime(minDate))
                            {
                                mainFilter[index].FilterValues.Add(new FilterItem(0, minDate));
                                txtExpirationDateFrom.Text = minDate;
                            }
                            else
                            {
                                // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fecha máxima al control
                                if (Convert.ToDateTime(txtExpirationDateFrom.Text) > Convert.ToDateTime(maxDate))
                                {
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, maxDate));
                                    txtExpirationDateFrom.Text = maxDate;
                                }
                                else
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(txtExpirationDateFrom.Text.Trim()).ToString(FormatDate)));
                            }
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, deja en blanco la fecha
                            mainFilter[index].FilterValues.Add(new FilterItem(0, string.Empty));
                            txtExpirationDateFrom.Text = string.Empty;
                        }

                        try
                        {
                            // Valida Fecha Hasta
                            // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fechamáxima control
                            if (Convert.ToDateTime(txtExpirationDateTo.Text) > Convert.ToDateTime(maxDate))
                            {
                                dateTo = maxDate;
                                txtExpirationDateTo.Text = maxDate;
                            }
                            else
                            {
                                if (txtExpirationDateTo.Text != string.Empty)
                                {
                                    // Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                                    if (Convert.ToDateTime(txtExpirationDateTo.Text) < Convert.ToDateTime(minDate))
                                    {
                                        dateTo = minDate;
                                        txtExpirationDateTo.Text = minDate;
                                    }
                                    else
                                    {
                                        // Agrega un dia menos 1 segundo a 'Date To' para abarcar todo el dia seleccionado
                                        dateTo = Convert.ToDateTime(txtExpirationDateTo.Text.Trim()).AddDays(1).Add(System.TimeSpan.FromSeconds(-60)).ToString(FormatDateTime);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, deja en blanco la fecha
                            dateTo = string.Empty;
                            txtExpirationDateTo.Text = string.Empty;
                        }


                        mainFilter[index].FilterValues.Add(new FilterItem(0, dateTo));
                        dateTo = string.Empty;

                        // Si ambos filtros quedaron en blanco luego de las validaciones, los elimina
                        if (mainFilter[index].FilterValues[0].Value == string.Empty && mainFilter[index].FilterValues[1].Value == string.Empty)
                            mainFilter[index].FilterValues.Clear();
                    }
                }

                // Expected Date
                if (txtExpectedDateFrom.Text != string.Empty || txtExpectedDateTo.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.ExpectedDateRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        try
                        {
                            // Valida Fecha Desde
                            // Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                            if (Convert.ToDateTime(txtExpectedDateFrom.Text) < Convert.ToDateTime(minDate))
                            {
                                mainFilter[index].FilterValues.Add(new FilterItem(0, minDate));
                                txtExpectedDateFrom.Text = minDate;
                            }
                            else
                            {
                                // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fecha máxima al control
                                if (Convert.ToDateTime(txtExpectedDateFrom.Text) > Convert.ToDateTime(maxDate))
                                {
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, maxDate));
                                    txtExpectedDateFrom.Text = maxDate;
                                }
                                else
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(txtExpectedDateFrom.Text.Trim()).ToString(FormatDate)));
                            }
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, deja en blanco la fecha
                            mainFilter[index].FilterValues.Add(new FilterItem(0, string.Empty));
                            txtExpectedDateFrom.Text = string.Empty;
                        }

                        try
                        {
                            // Valida Fecha Hasta
                            // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fechamáxima control
                            if (Convert.ToDateTime(txtExpectedDateTo.Text) > Convert.ToDateTime(maxDate))
                            {
                                dateTo = maxDate;
                                txtExpectedDateTo.Text = maxDate;
                            }
                            else
                            {
                                if (txtExpectedDateTo.Text != string.Empty)
                                {
                                    // Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                                    if (Convert.ToDateTime(txtExpectedDateTo.Text) < Convert.ToDateTime(minDate))
                                    {
                                        dateTo = minDate;
                                        txtExpectedDateTo.Text = minDate;
                                    }
                                    else
                                    {
                                        // Agrega un dia menos 1 segundo a 'Date To' para abarcar todo el dia seleccionado
                                        dateTo = Convert.ToDateTime(txtExpectedDateTo.Text.Trim()).AddDays(1).Add(System.TimeSpan.FromSeconds(-60)).ToString(FormatDateTime);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, deja en blanco la fecha
                            dateTo = string.Empty;
                            txtExpectedDateTo.Text = string.Empty;
                        }


                        mainFilter[index].FilterValues.Add(new FilterItem(0, dateTo));
                        dateTo = string.Empty;

                        // Si ambos filtros quedaron en blanco luego de las validaciones, los elimina
                        if (mainFilter[index].FilterValues[0].Value == string.Empty && mainFilter[index].FilterValues[1].Value == string.Empty)
                            mainFilter[index].FilterValues.Clear();
                    }
                }

                // Shipment Date
                if (txtShipmentDateFrom.Text != string.Empty || txtShipmentDateTo.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.ShipmentDateRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();

                        try
                        {
                            // Valida Fecha Desde
                            // Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                            if (Convert.ToDateTime(txtShipmentDateFrom.Text) < Convert.ToDateTime(minDate))
                            {
                                mainFilter[index].FilterValues.Add(new FilterItem(0, minDate));
                                txtShipmentDateFrom.Text = minDate;
                            }
                            else
                            {
                                // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fecha máxima al control
                                if (Convert.ToDateTime(txtShipmentDateFrom.Text) > Convert.ToDateTime(maxDate))
                                {
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, maxDate));
                                    txtShipmentDateFrom.Text = maxDate;
                                }
                                else
                                    mainFilter[index].FilterValues.Add(new FilterItem(0, Convert.ToDateTime(txtShipmentDateFrom.Text.Trim()).ToString(FormatDate)));
                            }
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, deja en blanco la fecha
                            mainFilter[index].FilterValues.Add(new FilterItem(0, string.Empty));
                            txtShipmentDateFrom.Text = string.Empty;
                        }

                        try
                        {
                            // Valida Fecha Hasta
                            // Si el valor ingresado es mayor a la fecha máxima válida, asigna la fechamáxima control
                            if (Convert.ToDateTime(txtShipmentDateTo.Text) > Convert.ToDateTime(maxDate))
                            {
                                dateTo = maxDate;
                                txtShipmentDateTo.Text = maxDate;
                            }
                            else
                            {
                                if (txtShipmentDateTo.Text != string.Empty)
                                {
                                    // Si el valor ingresado es menor a la fecha mínima válida, asigna la fecha mínima al control
                                    if (Convert.ToDateTime(txtShipmentDateTo.Text) < Convert.ToDateTime(minDate))
                                    {
                                        dateTo = minDate;
                                        txtShipmentDateTo.Text = minDate;
                                    }
                                    else
                                    {
                                        // Agrega un dia menos 1 segundo a 'Date To' para abarcar todo el dia seleccionado
                                        dateTo = Convert.ToDateTime(txtShipmentDateTo.Text.Trim()).AddDays(1).Add(System.TimeSpan.FromSeconds(-60)).ToString(FormatDateTime);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Si el valor ingresado no es válido, deja en blanco la fecha
                            dateTo = string.Empty;
                            txtShipmentDateTo.Text = string.Empty;
                        }


                        mainFilter[index].FilterValues.Add(new FilterItem(0, dateTo));
                        dateTo = string.Empty;

                        // Si ambos filtros quedaron en blanco luego de las validaciones, los elimina
                        if (mainFilter[index].FilterValues[0].Value == string.Empty && mainFilter[index].FilterValues[1].Value == string.Empty)
                            mainFilter[index].FilterValues.Clear();
                    }
                }

                // Lot Nbr Range
                if (txtLotNumberFrom.Text != string.Empty || txtLotNumberTo.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LotNumberRange);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtLotNumberFrom.Text.Trim()));
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtLotNumberTo.Text.Trim()));
                    }
                }
            }

            // PROVEEDOR TAB
            if (this.tabProveedorVisible)
            {
                if (lstProveedor.Items.Count > 0)
                {
                    if (!lstProveedor.Items[0].Selected)
                    {
                        index = Convert.ToInt16(EntityFilterName.Vendor);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            foreach (ListItem item in lstProveedor.Items)
                            {
                                if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                            }
                        }
                    }
                }
            }

            // TRANSPORTISTA TAB
            if (this.tabTransportistaVisible)
            {
                if (lstTransportista.Items.Count > 0)
                {
                    if (!lstTransportista.Items[0].Selected)
                    {
                        index = Convert.ToInt16(EntityFilterName.Carrier);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            foreach (ListItem item in lstTransportista.Items)
                            {
                                if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                            }
                        }
                    }
                }
            }

            // CHOFER TAB
            if (this.tabChoferVisible)
            {
                if (lstChofer.Items.Count > 0)
                {
                    if (!lstChofer.Items[0].Selected)
                    {
                        index = Convert.ToInt16(EntityFilterName.Driver);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            foreach (ListItem item in lstChofer.Items)
                            {
                                if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                            }
                        }
                    }
                }
            }

            // DOCUMENT TAB
            if (this.tabDocumentVisible)
            {
                // OutboundType
                if (lstInboundType.Items.Count > 0)
                {
                    if (!lstInboundType.Items[0].Selected)
                    {
                        index = Convert.ToInt16(EntityFilterName.OutboundType);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            foreach (ListItem item in lstInboundType.Items)
                            {
                                if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                            }
                        }
                    }
                }

                // Vendor
                if (lstVendor.Items.Count > 0)
                {
                    if (!lstVendor.Items[0].Selected)
                    {
                        index = Convert.ToInt16(EntityFilterName.Vendor);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            foreach (ListItem item in lstVendor.Items)
                            {
                                if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                            }
                        }
                    }
                }

                // Carrier
                if (lstCarrier.Items.Count > 0)
                {
                    if (!lstCarrier.Items[0].Selected)
                    {
                        index = Convert.ToInt16(EntityFilterName.Carrier);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            foreach (ListItem item in lstCarrier.Items)
                            {
                                if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                            }
                        }
                    }
                }

                // Driver
                if (lstDriver.Items.Count > 0)
                {
                    if (!lstDriver.Items[0].Selected)
                    {
                        index = Convert.ToInt16(EntityFilterName.Driver);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            foreach (ListItem item in lstDriver.Items)
                            {
                                if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                            }
                        }
                    }
                }
            }

            // LPN TAB
            if (this.tabLPNVisible)
            {
                // Lpn Code
                if (txtIdLpnCode.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LpnCode);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtIdLpnCode.Text.Trim()));
                    }
                }
                // Lpn Parent
                if (txtLpnParent.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LpnParent);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtLpnParent.Text.Trim()));
                    }
                }
                // Seal Number
                if (txtLpnSealNumber.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.LpnSealNumber);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtLpnSealNumber.Text.Trim()));
                    }
                }
                // LpnType
                if (lstLpnLpnType.Items.Count > 0)
                {
                    if (!lstLpnLpnType.Items[0].Selected)
                    {
                        index = Convert.ToInt16(EntityFilterName.LpnType);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            foreach (ListItem item in lstLpnLpnType.Items)
                            {
                                if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                            }
                        }
                    }
                }
                // IsParent
                if (chkLpnIsParent.Checked == true)
                {
                    index = Convert.ToInt16(EntityFilterName.LpnIsParent);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, "1"));
                    }
                }
                else
                {
                    // IsNotParent
                    if (chkLpnIsNotParent.Checked == true)
                    {
                        index = Convert.ToInt16(EntityFilterName.LpnIsParent);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            mainFilter[index].FilterValues.Add(new FilterItem(0, "0"));
                        }
                    }
                }
                // IsClosed
                if (chkLpnIsClosed.Checked == true)
                {
                    index = Convert.ToInt16(EntityFilterName.LpnIsClosed);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, "1"));
                    }
                }
                else
                {
                    // IsNotClosed
                    if (chkLpnIsNotClosed.Checked == true)
                    {
                        index = Convert.ToInt16(EntityFilterName.LpnIsClosed);

                        if (mainFilter[index].FilterValues != null)
                        {
                            mainFilter[index].FilterValues.Clear();
                            mainFilter[index].FilterValues.Add(new FilterItem(0, "0"));
                        }
                    }
                }
            }

            if (this.tabStockVisible)
            {
                if (MainFilter.Exists(filter => filter.Name == LOCKED_STOCK))
                {
                    MainFilter.RemoveAll(filter => filter.Name == LOCKED_STOCK);
                }

                if (chkLockedStock.Checked)
                {
                    MainFilter.Add(new EntityFilter() { Name = LOCKED_STOCK, FilterValues = new List<FilterItem>() { new FilterItem(0, "1") } });
                } 
            }

            if (tabMultipleChoiceOrderFiltersVisible)
            {
                if (!lstTrackOutboundType.Items[0].Selected)
                {
                    index = Convert.ToInt16(EntityFilterName.ListTrackOutboundType);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        foreach (ListItem item in lstTrackOutboundType.Items)
                        {
                            if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }
            }

            if (tabMultipleChoiceTrackTaskFiltersVisible)
            {
                if (!lstLstTrackTask.Items[0].Selected)
                {
                    index = Convert.ToInt16(EntityFilterName.TrackTaskType);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        foreach (ListItem item in lstLstTrackTask.Items)
                        {
                            if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }
            }

            // TAB Por Unidades
            if (this.tabItemUnitsVisible)
            {
                // Total Lineas
                if (txtTotalLines.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.TotalLines);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtTotalLines.Text.Trim()));
                    }
                }
                // Total Unidades
                if (txtTotalItems.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.TotalItems);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtTotalItems.Text.Trim()));
                    }
                }
                // Max Por Lineas
                if (txtMaxPerLines.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.MaxPerLines);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtMaxPerLines.Text.Trim()));
                    }
                }
            }
        }

        /// <summary>
        /// Limpia valores anteriores 
        /// </summary>
        public void ClearFilterObject()
        {
            foreach (EntityFilter entityFilter in mainFilter)
            {
                entityFilter.FilterValues.Clear();
            }
        }

        /// <summary>
        /// Limpia valores anteriores en los textboxes y listas
        /// </summary>
        public void ClearControls()
        {
            this.txtFilterCode.Text = string.Empty;
            this.txtFilterCodeAlt.Text = string.Empty;
            this.txtFilterCodeNumeric.Text = string.Empty;
            this.txtFilterName.Text = string.Empty;
            this.txtFilterDescription.Text = string.Empty;
            this.txtFilterDocumentNumber.Text = string.Empty;
            this.txtFilterIdTruckCode.Text = string.Empty;
            this.txtMaxStock.Text = string.Empty;
            this.txtPorcentajeUso.Text = string.Empty;
            this.txtFilterItem.Text = string.Empty;
            this.txtNombreProveedor.Text = string.Empty;
            this.txtNombreTransportista.Text = string.Empty;
            this.txtNombreChofer.Text = string.Empty;
            this.ddlFilterStatus.SelectedIndex = 0;
            this.ddlFilterTransactionStatus.SelectedIndex = 0;
            this.ddlFilterSimpliRouteVisitStatus.SelectedIndex = 0;
            if (this.ddlFilterLocationType.Items.Count > 0) this.ddlFilterLocationType.SelectedIndex = 0;
            if (this.ddlFilterZoneType.Items.Count > 0) this.ddlFilterZoneType.SelectedIndex = 0;
            if (this.ddlFilterLpnType.Items.Count > 0) this.ddlFilterLpnType.SelectedIndex = 0;
            if (this.ddlFilterWarehouse.Items.Count > 0) this.ddlFilterWarehouse.SelectedIndex = 0;
            if (this.ddlFilterHangar.Items.Count > 0) this.ddlFilterHangar.SelectedIndex = 0;
            if (this.ddlFilterOwner.Items.Count > 0) this.ddlFilterOwner.SelectedIndex = 0;
            if (this.ddlFilterTruckType.Items.Count > 0) this.ddlFilterTruckType.SelectedIndex = 0;
            if (this.ddlFilterInboundType.Items.Count > 0) this.ddlFilterInboundType.SelectedIndex = 0;
            if (this.ddlFilterScope.Items.Count > 0) this.ddlFilterScope.SelectedIndex = 0;
            if (this.lstInboundType.Items.Count > 0) this.lstInboundType.SelectedIndex = 0;
            if (this.ddlFilterMovementType.Items.Count > 0) this.ddlFilterMovementType.SelectedIndex = 0;
            if (this.ddlFilterTaskType.Items.Count > 0) this.ddlFilterTaskType.SelectedIndex = 0;
            if (this.ddlFilterTrackTaskType.Items.Count > 0) this.ddlFilterTrackTaskType.SelectedIndex = 0;
            if (this.ddlFilterOutboundType.Items.Count > 0) this.ddlFilterOutboundType.SelectedIndex = 0;
            if (this.ddlFilterTrackOutboundType.Items.Count > 0) this.ddlFilterTrackOutboundType.SelectedIndex = 0;
            if (this.ddlFilterTrackInboundType.Items.Count > 0) this.ddlFilterTrackInboundType.SelectedIndex = 0;
            if (this.ddlFilterReferenceDocType.Items.Count > 0) this.ddlFilterReferenceDocType.SelectedIndex = 0;
            if (this.ddlFilterWmsProcess.Items.Count > 0) this.ddlFilterWmsProcess.SelectedIndex = 0;
            if (this.ddlFilterReason.Items.Count > 0) this.ddlFilterReason.SelectedIndex = 0;

            if (this.ddlBscGrpItm1.Items.Count > 0) this.ddlBscGrpItm1.SelectedIndex = 0;
            if (this.ddlBscGrpItm2.Items.Count > 0) this.ddlBscGrpItm2.SelectedIndex = 0;
            if (this.ddlBscGrpItm3.Items.Count > 0) this.ddlBscGrpItm3.SelectedIndex = 0;
            if (this.ddlBscGrpItm4.Items.Count > 0) this.ddlBscGrpItm4.SelectedIndex = 0;

            //Translate
            if (this.ddlTranslate.Items.Count > 0) this.ddlTranslate.SelectedIndex = 0;
            if (this.ddlTypeObject.Items.Count > 0) this.ddlTypeObject.SelectedIndex = 0;
            if (this.ddlModule.Items.Count > 0) this.ddlModule.SelectedIndex = 0;
            if (this.ddlProperty.Items.Count > 0) this.ddlProperty.SelectedIndex = 0;
            if (this.ddlObjectContainer.Items.Count > 0) this.ddlObjectContainer.SelectedIndex = 0;
            this.listWmsProcessType = null;
            if (advancedFilterVisible)
            {
                this.chkUseAdvancedFilter.Checked = false;
                context.SessionInfo.FilterUseAdvanced = false;

                ClearAdvancedControls();

                Tabs.ActiveTabIndex = 0;
            }
        }

        public void checkAdvancedFilter()
        {
            this.chkUseAdvancedFilter.Checked = true;
        }

        /// <summary>
        /// Limpia valores anteriores en los textboxes y listas del filtro avanzado
        /// </summary>
        public void ClearAdvancedControls()
        {
            if (this.tabDispatchingVisible)
            {
                this.txtPriorityFrom.Text = string.Empty;
                this.txtPriorityTo.Text = string.Empty;
                this.txtCustomer.Text = string.Empty;
                this.txtCarrier.Text = string.Empty;
                this.txtRoute.Text = string.Empty;
            }

            if (this.tabLayoutVisible)
            {
                this.lstHangar.SelectedIndex = 0;
                this.lstWorkZone.SelectedIndex = 0;
                this.lstLocationType.SelectedIndex = 0;
            }

            if (this.tabLocationVisible)
            {
                this.txtLocation.Text = string.Empty;
                this.txtLocationFrom.Text = string.Empty;
                this.txtLocationTo.Text = string.Empty;
                this.txtLocationRow.Text = string.Empty;
                this.txtLocationColumn.Text = string.Empty;
                this.txtLocationLevel.Text = string.Empty;
                this.txtLocationAisle.Text = string.Empty;
            }

            if (this.tabItemGroupVisible)
            {
                this.ddlGrpItem1.SelectedIndex = 0;
                this.ddlGrpItem2.SelectedIndex = 0;
                this.ddlGrpItem3.SelectedIndex = 0;
                this.ddlGrpItem4.SelectedIndex = 0;
            }

            if (this.tabDatesVisible)
            {
                this.txtFabricationDateFrom.Text = string.Empty;
                this.txtFabricationDateTo.Text = string.Empty;
                this.txtExpirationDateFrom.Text = string.Empty;
                this.txtExpirationDateTo.Text = string.Empty;
                this.txtExpectedDateFrom.Text = string.Empty;
                this.txtExpectedDateTo.Text = string.Empty;
                this.txtShipmentDateFrom.Text = string.Empty;
                this.txtShipmentDateTo.Text = string.Empty;
                this.txtLotNumberFrom.Text = string.Empty;
                this.txtLotNumberTo.Text = string.Empty;
            }

            if (this.tabProveedorVisible)
            {
                this.txtNombreProveedor.Text = string.Empty;
                this.lstProveedor.SelectedIndex = 0;
            }

            if (this.tabTransportistaVisible)
            {
                this.txtNombreTransportista.Text = string.Empty;
                this.lstTransportista.SelectedIndex = 0;
            }

            if (this.tabChoferVisible)
            {
                this.txtNombreChofer.Text = string.Empty;
                this.lstChofer.SelectedIndex = 0;
            }

            if (this.tabDocumentVisible)
            {
                if (this.documentTypeVisible)
                {
                    if (this.ddlFilterInboundType.Items.Count != 0)
                    {
                        this.ddlFilterInboundType.SelectedIndex = 0;
                    }
                }
                if (this.vendorVisible) this.lstVendor.SelectedIndex = 0;
                if (this.carrierVisible) this.lstCarrier.SelectedIndex = 0;
                if (this.driverVisible) this.lstDriver.SelectedIndex = 0;
            }

            if (this.tabReceptionLogVisible)
            {
                this.txtItemCode.Text = string.Empty;
                this.txtItemName.Text = string.Empty;
                this.txtDocumentNbr.Text = string.Empty;
                this.txtReferenceNbr.Text = string.Empty;
                this.txtOperator.Text = string.Empty;
                this.txtSourceLocation.Text = string.Empty;
                this.txtSourceLpn.Text = string.Empty;
                this.txtTargetLocation.Text = string.Empty;
                this.txtTargetLpn.Text = string.Empty;
                this.txtPriorityTask.Text = string.Empty;
            }

            if (this.tabGS1Visible)
            {
                this.txtLotNumberTabGs1.Text = string.Empty;
                this.txtGtinTabGs1.Text = string.Empty;
                this.txtGsinTabGs1.Text = string.Empty;
                this.txtFabricationDateTabGs1.Text = string.Empty;
                this.txtExpirationDateTabGs1.Text = string.Empty;
            }

            if (this.tabTaskVisible)
            {
                this.chkShowDetail.Checked = false;
                this.chkNotComplete.Checked = false;
                this.chkComplete.Checked = false;

            }


            //Nuevo Tab Creado para ser utilizado en al Mapa
            if (this.tabMapaBodegaVisible)
            {
                this.txtMapFabricationDate.Text= string.Empty;
                this.txtMapExpirationDate.Text = string.Empty;
                this.txtMapFifoDate.Text = string.Empty;
                this.txtMapLote.Text = string.Empty;
                this.txtMapLPN.Text = string.Empty;
                this.chkMapHoldLocation.Checked = false;
            }

            if (tabMultipleChoiceOrderFiltersVisible)
            {
                this.lstTrackOutboundType.SelectedIndex = 0;
            }
        }

        public void ClearFiltersMap()
        {
            this.txtMapFabricationDate.Text = string.Empty;
            this.txtMapExpirationDate.Text = string.Empty;
            this.txtMapFifoDate.Text = string.Empty;
            this.txtMapLote.Text = string.Empty;
            this.txtMapLPN.Text = string.Empty;
            this.txtMapCategory.Text = string.Empty;
            this.chkMapHoldLocation.Checked = false;            
        }

        /// <summary>
        /// Carga los valores seleccionados del Filtro en el Listbox correspondiente
        /// </summary>
        public void LoadListValues(EntityFilterName entityFilter, ListBox list)
        {
            int index;

            index = Convert.ToInt16(entityFilter);

            if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            {
                foreach (ListItem listItem in list.Items) listItem.Selected = false;

                foreach (FilterItem filterItem in mainFilter[index].FilterValues)
                {
                    foreach (ListItem listItem in list.Items)
                    {
                        if (filterItem.Value == listItem.Value)
                        {
                            listItem.Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Setea los textboxes y listas con el estado actual del objeto Main Filter
        /// </summary>
        public void LoadControlsFromFilterObject()
        {
            int index;
            #region "FILTRO BÁSICO"

            // Wharehouse
            if (warehouseVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Warehouse);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterWarehouse.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterWarehouse.SelectedIndex = 0;
            }

            // Hangar
            if (hangarVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Hangar);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterHangar.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterHangar.SelectedIndex = 0;
            }

            // Owner
            if (ownerVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Owner);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterOwner.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterOwner.SelectedIndex = 0;
            }
            // LpnType
            if (lpnTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.LpnType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterLpnType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterLpnType.SelectedIndex = 0;
            }

            // ZoneType
            if (zoneTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.ZoneType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterZoneType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterZoneType.SelectedIndex = 0;
            }

            // Reason
            if (reasonVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Reason);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterReason.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterReason.SelectedIndex = 0;
            }

            // TruckType
            if (truckTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.TruckType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterTruckType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterTruckType.SelectedIndex = 0;
            }

            // MovementType
            if (movementTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.MovementType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterMovementType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterMovementType.SelectedIndex = 0;
            }

            // TaskType
            if (taskTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.TaskType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterTaskType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterTaskType.SelectedIndex = 0;
            }

            // TrackTaskType
            if (trackTaskTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.TrackTaskType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterTrackTaskType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterTrackTaskType.SelectedIndex = 0;
            }

            // Scope
            if (parameterScopeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Scope);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterScope.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterScope.SelectedIndex = 0;
            }

            // InboundType (nacional - internacional)
            if (inboundTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.InboundType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterInboundType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterInboundType.SelectedIndex = 0;
            }

            // TrackInboundType
            if (trackInboundTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.TrackInboundType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterTrackInboundType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterTrackInboundType.SelectedIndex = 0;
            }

            // ReferenceDocType
            if (referenceDocTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.ReferenceDocType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterReferenceDocType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterReferenceDocType.SelectedIndex = 0;
            }

            // OutboundType
            if (outboundTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.OutboundType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterOutboundType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterOutboundType.SelectedIndex = 0;
            }

            // TrackOutboundType
            if (trackOutboundTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.TrackOutboundType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterTrackOutboundType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterTrackOutboundType.SelectedIndex = 0;
            }
            
            // Code (genérico) 
            if (codeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Code);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    this.txtFilterCode.Text = mainFilter[index].FilterValues[0].Value;
            }

            //if (codeAltVisible)
            //{
            //    index = Convert.ToInt16(EntityFilterName.CodeAlt);

            //    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        this.txtFilterCodeAlt.Text = mainFilter[index].FilterValues[0].Value;
            //}

            // CodeNumeric (generico)
            if (codeNumericVisible)
            {
                index = Convert.ToInt16(EntityFilterName.CodeNumeric);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    this.txtFilterCodeNumeric.Text = mainFilter[index].FilterValues[0].Value;
            }

            // Item
            if (itemVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Item);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    this.txtFilterItem.Text = mainFilter[index].FilterValues[0].Value;
            }

            // UomType
            if (uomTypeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.UomType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterUomType.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterUomType.SelectedIndex = 0;
            }

            // Document Nbr
            if (documentVisible)
            {
                index = Convert.ToInt16(EntityFilterName.DocumentNbr);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    this.txtFilterDocumentNumber.Text = mainFilter[index].FilterValues[0].Value;
            }

            // Truck Code
            if (idTruckCodeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.IdTruckCode);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    this.txtFilterIdTruckCode.Text = mainFilter[index].FilterValues[0].Value;
            }

            // Dates (genérico) 
            if (dateVisible)
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                {
                    txtFilterDate.Text = mainFilter[index].FilterValues[0].Value;
                }
            }

            // Dates (genérico) 
            if (dateYearVisible)
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                {
                    txtFilterDateYear.Text = mainFilter[index].FilterValues[0].Value;
                }
            }

            // Dates (genérico) 
            if (YearVisible)
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                {
                    txtFilterYear.Text = mainFilter[index].FilterValues[0].Value;
                }
            }

            // Dates From and To (genérico) 
            if (dateFromVisible && dateToVisible)
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                {
                    txtFilterDateFrom.Text = mainFilter[index].FilterValues[0].Value;
                    if (mainFilter[index].FilterValues[1].Value != string.Empty) txtFilterDateTo.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
                }
            }

            //Translations
            #region Translations
            if (translateVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Translate);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlTranslate.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlTranslate.SelectedIndex = 0;
            }
            if (moduleVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Module);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlModule.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlModule.SelectedIndex = 0;
            }
            if (typeObjectVisible)
            {
                index = Convert.ToInt16(EntityFilterName.ObjectType);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlTypeObject.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlTypeObject.SelectedIndex = 0;
            }
            if (propertyVisible)
            {
                index = Convert.ToInt16(EntityFilterName.PropertyName);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlProperty.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlProperty.SelectedIndex = 0;
            }
            if (objectContainerVisible)
            {
                index = Convert.ToInt16(EntityFilterName.ObjectContainer);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlObjectContainer.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlObjectContainer.SelectedIndex = 0;
            }

            #endregion


            //Group Items
            if (this.itemVisible)
            {
                // Group Item 1
                index = Convert.ToInt16(EntityFilterName.GroupItem1);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlBscGrpItm1.SelectedIndex = mainFilter[index].FilterValues[0].Index;

                // Group Item 2
                index = Convert.ToInt16(EntityFilterName.GroupItem2);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                {
                    ((BasePage)Page).ConfigureDDLGrpItem2(ddlBscGrpItm2, true, idBscGrpItem1, idBscGrpItem2, this.lblEmptyRow.Text, false, -1);
                    ddlBscGrpItm2.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                }

                // Group Item 3
                index = Convert.ToInt16(EntityFilterName.GroupItem3);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                {
                    ((BasePage)Page).ConfigureDDLGrpItem3(ddlBscGrpItm3, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, this.lblEmptyRow.Text, false, -1);
                    ddlBscGrpItm3.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                }

                // Group Item 4
                index = Convert.ToInt16(EntityFilterName.GroupItem4);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                {
                    ((BasePage)Page).ConfigureDDLGrpItem4(ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, -1);
                    ddlBscGrpItm4.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                }
            }

            if (logicalWarehouseVisible)
            {
                index = Convert.ToInt16(EntityFilterName.LogicalWarehouse);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterLogicalWarehouse.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterLogicalWarehouse.SelectedIndex = 0;
            }


            #endregion

            #region "FILTRO AVANZADO"

            if (advancedFilterVisible)
            {
                this.chkUseAdvancedFilter.Checked = context.SessionInfo.FilterUseAdvanced;

                // DISPATCHING TAB
                if (this.tabDispatchingVisible)
                {
                    // Priority Range
                    index = Convert.ToInt16(EntityFilterName.PriorityRange);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    {
                        txtPriorityFrom.Text = mainFilter[index].FilterValues[0].Value;
                        txtPriorityTo.Text = mainFilter[index].FilterValues[1].Value;
                    }

                    // Customer
                    index = Convert.ToInt16(EntityFilterName.Customer);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtCustomer.Text = mainFilter[index].FilterValues[0].Value;

                    // Carrier
                    index = Convert.ToInt16(EntityFilterName.Carrier);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtCarrier.Text = mainFilter[index].FilterValues[0].Value;

                    // Route
                    index = Convert.ToInt16(EntityFilterName.Route);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtRoute.Text = mainFilter[index].FilterValues[0].Value;
                }

                //LAYOUT TAB
                if (this.tabLayoutVisible)
                {
                    // Hangar
                    LoadListValues(EntityFilterName.Hangar, lstHangar);

                    // WorkZone
                    LoadListValues(EntityFilterName.WorkZone, lstWorkZone);

                    // Location Type
                    LoadListValues(EntityFilterName.LocationType, lstLocationType);
                }

                //LOCATION TAB
                if (this.tabLocationVisible)
                {
                    // Location
                    index = Convert.ToInt16(EntityFilterName.Location);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtLocation.Text = mainFilter[index].FilterValues[0].Value;

                    // Location Range
                    index = Convert.ToInt16(EntityFilterName.LocationRange);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    {
                        this.txtLocationFrom.Text = mainFilter[index].FilterValues[0].Value;
                        this.txtLocationTo.Text = mainFilter[index].FilterValues[1].Value;
                    }

                    // Location Row
                    index = Convert.ToInt16(EntityFilterName.LocationRow);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtLocationRow.Text = mainFilter[index].FilterValues[0].Value;

                    // Location Column
                    index = Convert.ToInt16(EntityFilterName.LocationColumn);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtLocationColumn.Text = mainFilter[index].FilterValues[0].Value;

                    // Location Level
                    index = Convert.ToInt16(EntityFilterName.LocationLevel);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtLocationLevel.Text = mainFilter[index].FilterValues[0].Value;

                    // Location Aisle
                    index = Convert.ToInt16(EntityFilterName.LocationAisle);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtLocationAisle.Text = mainFilter[index].FilterValues[0].Value;
                }

                //GROUP 1 TAB
                if (this.tabItemGroupVisible)
                {
                    // Group Item 1
                    index = Convert.ToInt16(EntityFilterName.GroupItem1);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        ddlGrpItem1.SelectedIndex = mainFilter[index].FilterValues[0].Index;

                    // Group Item 2
                    index = Convert.ToInt16(EntityFilterName.GroupItem2);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    {
                        ((BasePage)Page).ConfigureDDLGrpItem2(ddlGrpItem2, true, idGrpItem1, idGrpItem2, this.lblEmptyRow.Text, false, -1);
                        ddlGrpItem2.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                    }

                    // Group Item 3
                    index = Convert.ToInt16(EntityFilterName.GroupItem3);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    {
                        ((BasePage)Page).ConfigureDDLGrpItem3(ddlGrpItem3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.lblEmptyRow.Text, false, -1);
                        ddlGrpItem3.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                    }

                    // Group Item 4
                    index = Convert.ToInt16(EntityFilterName.GroupItem4);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    {
                        ((BasePage)Page).ConfigureDDLGrpItem4(ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.lblEmptyRow.Text, false, -1);
                        ddlGrpItem4.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                    }
                }

                // DATE TAB
                if (this.tabDatesVisible)
                {
                    // Fabrication Date
                    index = Convert.ToInt16(EntityFilterName.FabricationDateRange);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    {
                        txtFabricationDateFrom.Text = mainFilter[index].FilterValues[0].Value;
                        if (mainFilter[index].FilterValues[1].Value != string.Empty) txtFabricationDateTo.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
                    }

                    // Expiration Date
                    index = Convert.ToInt16(EntityFilterName.ExpirationDateRange);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    {
                        txtExpirationDateFrom.Text = mainFilter[index].FilterValues[0].Value;
                        if (mainFilter[index].FilterValues[1].Value != string.Empty) txtExpirationDateTo.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
                    }

                    // Expected Date
                    index = Convert.ToInt16(EntityFilterName.ExpectedDateRange);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    {
                        txtExpectedDateFrom.Text = mainFilter[index].FilterValues[0].Value;
                        if (mainFilter[index].FilterValues[1].Value != string.Empty) txtExpectedDateTo.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
                    }

                    // Shipment Date
                    index = Convert.ToInt16(EntityFilterName.ShipmentDateRange);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    {
                        txtShipmentDateFrom.Text = mainFilter[index].FilterValues[0].Value;
                        if (mainFilter[index].FilterValues[1].Value != string.Empty) txtShipmentDateTo.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
                    }

                    // Lot Nbr Range
                    index = Convert.ToInt16(EntityFilterName.LotNumberRange);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    {
                        txtLotNumberFrom.Text = mainFilter[index].FilterValues[0].Value;
                        txtLotNumberTo.Text = mainFilter[index].FilterValues[1].Value;
                    }
                }

                // PROVEEDOR TAB
                if (this.tabProveedorVisible)
                {
                    LoadProveedor(Convert.ToInt16(ddlFilterOwner.SelectedValue), txtNombreProveedor.Text);
                    LoadListValues(EntityFilterName.Vendor, lstProveedor);
                }

                // TRANSPORTISTA TAB
                if (this.tabTransportistaVisible)
                {
                    LoadTransportista(txtNombreTransportista.Text);
                    LoadListValues(EntityFilterName.Carrier, lstTransportista);
                }

                // CHOFER TAB
                if (this.tabChoferVisible)
                {
                    LoadChofer(txtNombreChofer.Text);
                    LoadListValues(EntityFilterName.Carrier, lstChofer);
                }

                // DOCUMENT TAB
                if (this.tabDocumentVisible)
                {
                    if (divDocumentType.Visible == true)
                    {
                        // Inbound Type
                        LoadListValues(EntityFilterName.DocumentType, lstInboundType);
                    }
                    if (divVendor.Visible)
                    {
                        // Vendor
                        LoadVendor(Convert.ToInt16(ddlFilterOwner.SelectedValue));
                        LoadListValues(EntityFilterName.Vendor, lstVendor);
                    }
                    if (divCarrier.Visible)
                    {
                        // Carrier
                        LoadListValues(EntityFilterName.Carrier, lstCarrier);
                    }
                    if (divDriver.Visible)
                    {
                        // Carrier
                        LoadListValues(EntityFilterName.Driver, lstDriver);
                    }
                }

                // RECEPTION TAB
                if (this.tabReceptionLog.Visible)
                {
                    // Item Code
                    index = Convert.ToInt16(EntityFilterName.Item);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtItemCode.Text = mainFilter[index].FilterValues[0].Value;

                    // Item Name
                    index = Convert.ToInt16(EntityFilterName.Description);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtItemName.Text = mainFilter[index].FilterValues[0].Value;

                    // Document Number
                    index = Convert.ToInt16(EntityFilterName.DocumentNbr);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtDocumentNbr.Text = mainFilter[index].FilterValues[0].Value;

                    // Reference Number
                    index = Convert.ToInt16(EntityFilterName.ReferenceNbr);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtReferenceNbr.Text = mainFilter[index].FilterValues[0].Value;

                    // Operator
                    index = Convert.ToInt16(EntityFilterName.Driver);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtOperator.Text = mainFilter[index].FilterValues[0].Value;

                    // Source Location
                    index = Convert.ToInt16(EntityFilterName.LocationSource);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtSourceLocation.Text = mainFilter[index].FilterValues[0].Value;

                    // Target Location
                    index = Convert.ToInt16(EntityFilterName.LocationTarget);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtTargetLocation.Text = mainFilter[index].FilterValues[0].Value;

                    // Source Lpn
                    index = Convert.ToInt16(EntityFilterName.LpnSource);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtSourceLpn.Text = mainFilter[index].FilterValues[0].Value;

                    // Target Lpn
                    index = Convert.ToInt16(EntityFilterName.LpnTarget);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtTargetLpn.Text = mainFilter[index].FilterValues[0].Value;

                    // Priority Task
                    index = Convert.ToInt16(EntityFilterName.PriorityTask);

                    if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                        this.txtPriorityTask.Text = mainFilter[index].FilterValues[0].Value;
                }

                if (tabMultipleChoiceOrderFiltersVisible)
                {
                    LoadListValues(EntityFilterName.ListTrackOutboundType, lstTrackOutboundType);
                }
            #endregion
            }
        }
        #endregion

        #region Metodos búsqueda de Item

        protected void btnBuscarItem_Click(object sender, ImageClickEventArgs e)
        {
            //Completa el ddl con el owner
            ddlOwnerItemControl.ClearSelection();
            ((BasePage)Page).LoadUserOwners(this.ddlOwnerItemControl, String.Empty, "-1", false, String.Empty, false);
            
            // Selecciona Owner por defecto del Usuario loggeado
            ((BasePage)Page).SelectDefaultOwner(ddlOwnerItemControl);

            txtSearchValueItem.Text = String.Empty;
            rblSearchCriteriaControl.Items[0].Selected = true;
            rblSearchCriteriaControl.Items[1].Selected = false;

            //limpia los registros de a grilla y la sesión
            Session.Remove(WMSTekSessions.Shared.ItemList);
            grdSearchItemsControl.DataSource = null;
            grdSearchItemsControl.DataBind();

            InitializePageCountItems();
            mpLookupItemControl.Show();
        }

        protected void imgBtnCloseItemSearch_Click(object sender, ImageClickEventArgs e)
        {
            mpLookupItemControl.Hide();
        }

        protected void grdSearchItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int editIndex = (Convert.ToInt32(grdSearchItemsControl.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                grdSearchItemsControl.Columns[1].Visible = true;
                if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    foreach (GridViewRow row in grdSearchItemsControl.Rows)
                    {
                        if (row.Cells[0].Text == editIndex.ToString())
                        {
                            //txtFilterItem.Text = row.Cells[4].Text;
                            txtFilterItem.Text = (row.Cells[1].FindControl("lblCode0") as Label).Text;

                            int index = Convert.ToInt16(EntityFilterName.Item);

                            if (mainFilter[index].FilterValues != null)
                            {
                                mainFilter[index].FilterValues.Clear();
                                mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterItem.Text));
                            }
                        }
                    }
                }
                //grdSearchItemsControl.Columns[4].Visible = true;
                grdSearchItemsControl.DataSource = null;
                grdSearchItemsControl.DataBind();
                //grdSearchItemsControl.Columns[4].Visible = false;
               
            }
            catch (Exception ex)
            {
                //outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void imbBuscarItemControl_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                List<FilterItem> filterItems = new List<FilterItem>();
                filterItems.Add(new FilterItem("%%"));
                filterItems.Add(new FilterItem("%%"));

                if (rblSearchCriteriaControl.Items[0].Selected == true)
                {
                    filterItems[0].Value = "%" + txtSearchValueItem.Text.Trim() + "%";
                    filterItems[0].Selected = true;
                    filterItems[1].Selected = false;
                }
                else
                {
                    filterItems[1].Value = "%" + txtSearchValueItem.Text.Trim() + "%";
                    filterItems[0].Selected = false;
                    filterItems[1].Selected = true;
                }

                GenericViewDTO<Item> itemSearchViewDTO;
                itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwnerFilter(filterItems, context, Convert.ToInt16(ddlOwnerItemControl.SelectedValue), true);

                Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);

                grdSearchItemsControl.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
                grdSearchItemsControl.EmptyDataText = this.Master.EmptyGridText;

                //InitializeGridItems();
                //grdSearchItemsControl.Columns[4].Visible = true;
                grdSearchItemsControl.DataSource = itemSearchViewDTO.Entities;
                grdSearchItemsControl.DataBind();
                //grdSearchItemsControl.Columns[4].Visible = false;                                

                InitializePageCountItems();

                mpLookupItemControl.Show();
            }
        }

        protected void btnLastGrdSearchItemsControl_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItemsControl.SelectedIndex = grdSearchItemsControl.PageCount - 1;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);
        }

        protected void btnNextGrdSearchItemsControl_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItemsControl < grdSearchItemsControl.PageCount)
            {
                ddlPagesSearchItemsControl.SelectedIndex = currentPageItemsControl + 1; ;
                ddlPagesSearchItemsSelectedIndexChanged(sender, e);
            }
        }

        protected void btnPrevGrdSearchItemsControl_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItemsControl > 0)
            {
                ddlPagesSearchItemsControl.SelectedIndex = currentPageItemsControl - 1; ;
                ddlPagesSearchItemsSelectedIndexChanged(sender, e);
            }
        }

        protected void btnFirstGrdSearchItemsControl_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItemsControl.SelectedIndex = 0;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);
        }

        protected void ddlPagesSearchItemsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Shared.ItemList))
            {
                GenericViewDTO<Item> itemSearchViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.Shared.ItemList];

                currentPageItemsControl = ddlPagesSearchItemsControl.SelectedIndex;
                grdSearchItemsControl.PageIndex = currentPageItemsControl;

                grdSearchItemsControl.DataSource = itemSearchViewDTO.Entities;
                grdSearchItemsControl.DataBind();

                //divLookupItem.Visible = true;
                ShowItemsButtonsPage();
                mpLookupItemControl.Show();
            }
        }

        private void ShowItemsButtonsPage()
        {
            if (currentPageItemsControl == grdSearchItemsControl.PageCount - 1)
            {
                btnNextGrdSearchItemsControl.Enabled = false;
                btnNextGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastGrdSearchItemsControl.Enabled = false;
                btnLastGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                btnPrevGrdSearchItemsControl.Enabled = true;
                btnPrevGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                btnFirstGrdSearchItemsControl.Enabled = true;
                btnFirstGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
            }
            else
            {
                if (currentPageItemsControl == 0)
                {
                    btnPrevGrdSearchItemsControl.Enabled = false;
                    btnPrevGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstGrdSearchItemsControl.Enabled = false;
                    btnFirstGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                    btnNextGrdSearchItemsControl.Enabled = true;
                    btnNextGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchItemsControl.Enabled = true;
                    btnLastGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnNextGrdSearchItemsControl.Enabled = true;
                    btnNextGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchItemsControl.Enabled = true;
                    btnLastGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevGrdSearchItemsControl.Enabled = true;
                    btnPrevGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstGrdSearchItemsControl.Enabled = true;
                    btnFirstGrdSearchItemsControl.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
            }
        }

        private void InitializePageCountItems()
        {
            if (grdSearchItemsControl.PageCount > 1)
            {
                int pageNumber;

                divPageGrdSearchItems.Visible = true;
                // Paginador
                ddlPagesSearchItemsControl.Items.Clear();
                for (int i = 0; i < grdSearchItemsControl.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPageItemsControl) lstItem.Selected = true;

                    ddlPagesSearchItemsControl.Items.Add(lstItem);
                }
                this.lblPageCountSearchItems.Text = grdSearchItemsControl.PageCount.ToString();

                ShowItemsButtonsPage();
            }
            else
            {
                divPageGrdSearchItems.Visible = false;
            }
        }

        protected void ddlPagesSearchItemsControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Shared.ItemList))
            {
                GenericViewDTO<Item> itemSearchViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.Shared.ItemList];

                currentPageItemsControl = ddlPagesSearchItemsControl.SelectedIndex;
                grdSearchItemsControl.PageIndex = currentPageItemsControl;

                grdSearchItemsControl.DataSource = itemSearchViewDTO.Entities;
                grdSearchItemsControl.DataBind();

                //divLookupItem.Visible = true;
                mpLookupItemControl.Show();

                ShowItemsButtonsPage();

            }
        }
        
        #endregion Metodos búsqueda de Item

        public Binaria.WMSTek.WebClient.Shared.WMSTekContent Master
        {
            get
            {
                return ((Binaria.WMSTek.WebClient.Shared.WMSTekContent)((Binaria.WMSTek.WebClient.Base.BasePage)Page).Master);
            }
        }

        protected void rdbMaximoStock_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbMaximoStock.Checked)
            {
                txtPorcentajeUso.Text = String.Empty;
                txtMaxStock.Text = "0";
                txtMaxStock.Enabled = true;
                txtPorcentajeUso.Enabled = false;
            }
            else
            {
                txtPorcentajeUso.Text = "0";
                txtMaxStock.Text = String.Empty;
                txtMaxStock.Enabled = false;
                txtPorcentajeUso.Enabled = true;
            }
            rdbPorcentajeUso.Checked = !rdbMaximoStock.Checked;
        }

        protected void rdbPorcentajeUso_CheckedChanged(object sender, EventArgs e)
        {
            rdbMaximoStock.Checked = !rdbPorcentajeUso.Checked;

            if (rdbPorcentajeUso.Checked)
            {
                txtPorcentajeUso.Text = "0";
                txtMaxStock.Text = String.Empty;
                txtMaxStock.Enabled = false;
                txtPorcentajeUso.Enabled = true;
            }
            else
            {
                txtPorcentajeUso.Text = String.Empty;
                txtMaxStock.Text = "0";
                txtMaxStock.Enabled = true;
                txtPorcentajeUso.Enabled = false;
            }
        }

        public string GetCfgParameter(string parameterName)
        {
            string result;
            try
            {
                result = context.CfgParameters.FirstOrDefault(w => w.ParameterCode.ToUpper().Equals((parameterName).ToUpper())).ParameterValue.Trim();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return result;
        }

        public void ConfigureDdlFromGrp1()
        {
            ((BasePage)Page).ConfigureDDLGrpItem1(this.ddlGrpItem1, true, idGrpItem1, this.lblEmptyRow.Text, false, idOwn);
            ((BasePage)Page).ConfigureDDLGrpItem2(this.ddlGrpItem2, true, idGrpItem1, idGrpItem2, this.lblEmptyRow.Text, false, -1);
            ((BasePage)Page).ConfigureDDLGrpItem3(this.ddlGrpItem3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.lblEmptyRow.Text, false, -1);
            ((BasePage)Page).ConfigureDDLGrpItem4(this.ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.lblEmptyRow.Text, false, -1);
        }
    }
}

    //Esto se usa en el designer NO BORRAR.
//     public Binaria.WMSTek.WebClient.Shared.WMSTekContent Master
//     {
//         get
//         {
//             return ((Binaria.WMSTek.WebClient.Shared.WMSTekContent)((Binaria.WMSTek.WebClient.Base.BasePage)Page).Master);
//         }
//     }
