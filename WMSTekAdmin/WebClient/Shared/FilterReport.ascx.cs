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
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class FilterReport : BaseUserControl
    {
        #region "Declaración de Variables"

        private List<EntityFilter> mainFilter;
        private CfgParameterName dateBefore;
        private CfgParameterName dateAfter;

        public event EventHandler ddlWareHouseIndexChanged;

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

        private int idBscGrpItem1
        {
            get
            {
                if (this.ddlBscGrpItm1.Items.Count > 0)
                    return Convert.ToInt32(this.ddlBscGrpItm1.SelectedValue);
                else
                    return -1;
            }
        }

        private int idBscGrpItem2
        {
            get
            {
                if (this.ddlBscGrpItm2.Items.Count > 0)
                    return Convert.ToInt32(this.ddlBscGrpItm2.SelectedValue);
                else
                    return -1;
            }
        }

        private int idBscGrpItem3
        {
            get
            {
                if (this.ddlBscGrpItm3.Items.Count > 0)
                    return Convert.ToInt32(this.ddlBscGrpItm3.SelectedValue);
                else
                    return -1;
            }
        }

        private int idBscGrpItem4
        {
            get
            {
                if (this.ddlBscGrpItm4.Items.Count > 0)
                    return Convert.ToInt32(this.ddlBscGrpItm4.SelectedValue);
                else
                    return -1;
            }
        }

        public bool divBscGroupItem
        {
            get { return this.divBscGroupItems.Visible; }
            set { this.divBscGroupItems.Visible = value; }
        }
        
        public bool ownerNotIncludeAll;
        public bool ownerIncludeNulls;
        public bool changeDefaultWhsAsData = false;

        public List<EntityFilter> MainFilter
        {
            get { return mainFilter; }
            set { mainFilter = value; }
        }

        public int idWhs
        {
            get { return Convert.ToInt32(this.ddlFilterWarehouse.SelectedValue); }
        }

        public bool rfvWarehouseEnabled
        {
            set { this.rfvFilterWarehouse.Enabled = value; }
        }
        public bool rfvOwnerEnabled
        {
            set { this.rfvFilterOwner.Enabled = value; }
        }

        public int idHangar
        {
            get { return Convert.ToInt32(this.ddlFilterHangar.SelectedValue); }
        }

        public bool searchVisible
        {
            get { return this.btnSearch.Visible; }
            set { this.btnSearch.Visible = value; }
        }

        public bool codeVisible
        {
            get { return this.divFilterCode.Visible; }
            set { this.divFilterCode.Visible = value; }
        }

        public bool nameVisible
        {
            get { return this.divFilterName.Visible; }
            set { this.divFilterName.Visible = value; }
        }

        public bool lotnumberVisible
        {
            get { return this.divFilterLotNumber.Visible; }
            set { this.divFilterLotNumber.Visible = value; }
        }

        public bool sealnumberVisible
        {
            get { return this.divFilterSealNumber.Visible; }
            set { this.divFilterSealNumber.Visible = value; }
        }
        
        public bool descriptionVisible
        {
            get { return this.divFilterDescription.Visible; }
            set { this.divFilterDescription.Visible = value; }
        }

        public bool warehouseVisible
        {
            get { return this.divFilterWarehouse.Visible; }
            set { this.divFilterWarehouse.Visible = value; }
        }

        public bool ownerVisible
        {
            get { return this.divFilterOwner.Visible; }
            set { this.divFilterOwner.Visible = value; }
        }
        public bool ownerUserVisible
        {
            get { return this.divFilterOwnerUser.Visible; }
            set { this.divFilterOwnerUser.Visible = value; }
        }
        public bool ownerValidateVisible
        {
            get { return this.rfvFilterOwner.Visible; }
            set { this.rfvFilterOwner.Visible = value; }
        }

        public bool categoryItemVisible
        {
            get { return this.divFilterCategoryItem.Visible; }
            set { this.divFilterCategoryItem.Visible = value; }
        }
        public bool hangarVisible
        {
            get { return this.divFilterHangar.Visible; }
            set
            {
                this.divFilterHangar.Visible = value;
                this.ddlFilterWarehouse.AutoPostBack = true;
                this.ddlFilterWarehouse.SelectedIndexChanged += new EventHandler(ddlWarehouse_SelectedIndexChanged);
                this.ddlFilterWarehouse.Attributes.Add("onchange", "ClearCanvas()");
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
        public bool locationVisible
        {
            get { return this.divFilterLocation.Visible; }
            set { this.divFilterLocation.Visible = value; }
        }
        public bool dateVisible
        {
            get { return this.divFilterDate.Visible; }
            set { this.divFilterDate.Visible = value; }
        }
        public bool dateFromVisible
        {
            get { return this.divFilterDateFrom.Visible; }
            set { this.divFilterDateFrom.Visible = value; }
        }

        public bool expirationDateVisible
        {
            get { return this.divFilterExpirationDate.Visible; }
            set { this.divFilterExpirationDate.Visible = value; }
        }

        public bool fabricationDateVisible
        {
            get { return this.divFilterFabricationDate.Visible; }
            set { this.divFilterFabricationDate.Visible = value; }
        }        

        public bool chkFabricationDateEnabled
        {
            get { return this.chkFilterFabricationDate.Enabled; }
            set { this.chkFilterFabricationDate.Enabled = value; }
        }
        public bool chkExpirationDateEnabled
        {
            get { return this.chkFilterExpirationDate.Enabled; }
            set { this.chkFilterExpirationDate.Enabled = value; }
        }
        public bool chkFilterExpirationDateChecked
        {
            get { return this.chkFilterExpirationDate.Checked; }
            set { this.chkFilterExpirationDate.Checked = value; }
        }
        public bool chkFilterFabricationDateChecked
        {
            get { return this.chkFilterFabricationDate.Checked; }
            set { this.chkFilterFabricationDate.Checked = value; }
        }

        public bool dateToVisible
        {
            get { return this.divFilterDateTo.Visible; }
            set { this.divFilterDateTo.Visible = value; }
        }

        public bool fifoDateFromVisible
        {
            get { return this.divFilterFifoDateFrom.Visible; }
            set { this.divFilterFifoDateFrom.Visible = value; }
        }

        public bool fifoDateToVisible
        {
            get { return this.divFilterFifoDateTo.Visible; }
            set { this.divFilterFifoDateTo.Visible = value; }
        }
        public bool workZoneVisible
        {
            get { return this.divFilterWorkZone.Visible; }
            set { 
                this.divFilterWorkZone.Visible = value;
                this.ddlFilterWarehouse.AutoPostBack = true;
                this.ddlFilterWarehouse.SelectedIndexChanged += new EventHandler(ddlFilterWarehouse_SelectedIndexChanged1);
            }
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

        public bool inboundTypeVisible
        {
            get { return this.divFilterInboundType.Visible; }
            set { this.divFilterInboundType.Visible = value; }

        }
        public bool reqFilterInboundTypeEnabled
        {
            get { return this.reqFilterInboundType.Enabled; }
            set { this.reqFilterInboundType.Enabled = value; }
        }

        public bool parameterScopeVisible
        {
            get { return this.divFilterScope.Visible; }
            set { this.divFilterScope.Visible = value; }

        }

        public bool statusVisible
        {
            get { return this.divFilterStatus.Visible; }
            set { this.divFilterStatus.Visible = value; }
        }

        public bool locationTypeVisible
        {
            get { return this.divFilterLocationType.Visible; }
            set { this.divFilterLocationType.Visible = value; }
        }

        public bool FilterWarehouseAutoPostBack
        {
            get { return this.ddlFilterWarehouse.AutoPostBack; }
            set { this.ddlFilterWarehouse.AutoPostBack = value; }
        }
        protected void OnddlWareHouseIndexChanged(EventArgs e)
        {
            if (ddlWareHouseIndexChanged != null)
            {
                ddlWareHouseIndexChanged(this, e);
            }
        }

        public bool advancedFilterVisible
        {
            get { return this.divAdvancedFilterOptions.Visible; }

            set
            {
                //this.divAdvancedFilterOptions.Visible = value;
                //this.divAdvancedFilterChk.Visible = value;
                //this.pnlAdvancedFilter.Visible = value;
                //this.cpeAdvancedFilter.Enabled = value;
                this.ddlFilterWarehouse.AutoPostBack = true;
                this.ddlFilterWarehouse.SelectedIndexChanged += new EventHandler(ddlWarehouse_SelectedIndexChanged);
                this.ddlFilterOwner.AutoPostBack = true;
                this.ddlFilterOwner.SelectedIndexChanged += new EventHandler(ddlFilterOwner_SelectedIndexChanged);
            }
        }
        //public bool tabLayoutVisible
        //{
        //    get { return this.tabLayout.Visible; }
        //    set { this.tabLayout.Visible = value; }
        //}

        //public bool tabLocationVisible
        //{
        //    get { return this.tabLocation.Visible; }
        //    set { this.tabLocation.Visible = value; }
        //}

        //public bool tabGroup1Visible
        //{
        //    get { return this.tabGroup1.Visible; }
        //    set { this.tabGroup1.Visible = value; }
        //}

        //public bool tabDatesVisible
        //{
        //    get { return this.tabDates.Visible; }
        //    set { this.tabDates.Visible = value; }
        //}

        public bool YearVisible
        {
            get { return this.divFilterYear.Visible; }
            set { this.divFilterYear.Visible = value; }
        }

        public bool typeReportVisible
        {
            get { return this.divFilterTypeReport.Visible; }
            set
            {
                this.divFilterTypeReport.Visible = value;
                this.ddlFilterTypeReport.AutoPostBack = true;
            }
        }

        public string emptyRowLabelText
        {
            get { return this.lblEmptyRow.Text; }
            set { this.lblEmptyRow.Text = value; }
        }

        public bool reqTxtFilterEnabled
        {
            get { return this.rqTxtFilterCode.Enabled; }
            set { this.rqTxtFilterCode.Enabled = value; }
        }

        #region "TAB DOCUMENT TYPE"
        //public bool tabDocumentVisible
        //{
        //    get { return this.tabDocument.Visible; }
        //    set { this.tabDocument.Visible = value; }
        //}
        //public bool documentTypeVisible
        //{
        //    get { return this.divDocumentType.Visible; }
        //    set { this.divDocumentType.Visible = value; }
        //}
        //public bool vendorVisible
        //{
        //    get { return this.divVendor.Visible; }
        //    set { this.divVendor.Visible = value; }
        //}
        //public bool carrierVisible
        //{
        //    get { return this.divCarrier.Visible; }
        //    set { this.divCarrier.Visible = value; }
        //}
        //public bool driverVisible
        //{
        //    get { return this.divDriver.Visible; }
        //    set { this.divDriver.Visible = value; }
        //}
        //#endregion

        //public bool tabDispatchingVisible
        //{
        //    get { return this.tabDispatching.Visible; }
        //    set { this.tabDispatching.Visible = value; }
        //}

        //public bool fabricationDateVisible
        //{
        //    get { return this.divFabricationDate.Visible; }
        //    set { this.divFabricationDate.Visible = value; }
        //}

        //public bool expirationDateVisible
        //{
        //    get { return this.divExpirationDate.Visible; }
        //    set { this.divExpirationDate.Visible = value; }
        //}

        //public bool expectedDateVisible
        //{
        //    get { return this.divExpectedDate.Visible; }
        //    set { this.divExpectedDate.Visible = value; }
        //}

        //public bool shipmentDateVisible
        //{
        //    get { return this.divShipmentDate.Visible; }
        //    set { this.divShipmentDate.Visible = value; }
        //}

        //public bool lotNumberVisible
        //{
        //    get { return this.divLotNumber.Visible; }
        //    set { this.divLotNumber.Visible = value; }
        //}

        public string codeLabel
        {
            get { return this.lblCode.Text; }
            set { this.lblCode.Text = value; }
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
            set { this.lblDateFrom.Text = value; }
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

        public string typeReports
        {
            get { return this.ddlFilterTypeReport.SelectedValue; }
            set { this.ddlFilterTypeReport.SelectedValue = value; }
        }

        public event EventHandler BtnSearchClick;
        public event EventHandler DdlTypeReportSelectedIndexChanged;

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

        #endregion

        #endregion

        #region "Eventos"

        private void InitializeContext()
        {
                context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
        }

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                InitializeContext();

                base.Page_Init(sender, e);
                /// Recupera el estado actual del Filtro Principal 
                mainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];

                btnSearch.Attributes.Add("onmouseover", "this.src='" + ResolveUrl("~/WebResources/Images/Buttons/TaskBar/icon_search_on.png") + "'");
                btnSearch.Attributes.Add("onmouseout", "this.src='" + ResolveUrl("~/WebResources/Images/Buttons/TaskBar/icon_search.png") + "'");
            }
            catch (Exception ex)
            {

                ErrorDTO error = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(error);
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
                //this.Master.ucError.ShowError(error);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                //ClearAdvancedControls();
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int idWhs = Convert.ToInt16(ddlFilterWarehouse.SelectedValue);

                if (advancedFilterVisible)
                {
                    //LoadHangar(idWhs);
                    LoadWorkZone(idWhs);
                    LoadLocationType(idWhs);
                }
                else
                {
                    LoadHangarBase(idWhs);
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlFilterOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadVendor(Convert.ToInt16(ddlFilterOwner.SelectedValue));

                if (divBscGroupItem)
                {
                    int idOwner = (Convert.ToInt16(ddlFilterOwner.SelectedValue));

                    if (idOwner != 0)
                    {
                        ((BasePage)Page).LoadCategoryItemByOwner(ddlCategoryItem, Convert.ToInt16(ddlFilterOwner.SelectedValue), true, true, this.lblEmptyRow.Text);
                        ((BasePage)Page).ConfigureDDLGrpItem1(ddlBscGrpItm1, true, idBscGrpItem1, this.lblEmptyRow.Text, false, idOwner);
                        ((BasePage)Page).ConfigureDDLGrpItem2(ddlBscGrpItm2, true, idBscGrpItem1, idBscGrpItem2, this.lblEmptyRow.Text, false, idOwner);
                        ((BasePage)Page).ConfigureDDLGrpItem3(ddlBscGrpItm3, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, this.lblEmptyRow.Text, false, idOwner);
                        ((BasePage)Page).ConfigureDDLGrpItem4(ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, idOwner);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlFilterOwnerUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadVendor(Convert.ToInt16(ddlFilterOwnerUser.SelectedValue));

                if (divBscGroupItem)
                {
                    int idOwner = (Convert.ToInt16(ddlFilterOwnerUser.SelectedValue));

                    if (idOwner != 0)
                    {
                        ((BasePage)Page).LoadCategoryItemByOwner(ddlCategoryItem, Convert.ToInt16(ddlFilterOwnerUser.SelectedValue), true, true, this.lblEmptyRow.Text);
                        ((BasePage)Page).ConfigureDDLGrpItem1(ddlBscGrpItm1, true, idBscGrpItem1, this.lblEmptyRow.Text, false, idOwner);
                        ((BasePage)Page).ConfigureDDLGrpItem2(ddlBscGrpItm2, true, idBscGrpItem1, idBscGrpItem2, this.lblEmptyRow.Text, false, idOwner);
                        ((BasePage)Page).ConfigureDDLGrpItem3(ddlBscGrpItm3, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, this.lblEmptyRow.Text, false, idOwner);
                        ((BasePage)Page).ConfigureDDLGrpItem4(ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, idOwner);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlBscGrpItm1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int idOwn = 0;

                if (this.ddlFilterOwner.SelectedValue == "-1")
                {
                    //Captura el owner de la session
                   // idown = context.SessionInfo.Owner.Id; 

                    //TODO: Cuando se implemente el Owner en session Info, se debe cambiar esto.
                    idOwn = 4;
                }
                else
                {
                    idOwn = Convert.ToInt16(ddlFilterOwner.SelectedValue);
                }

                ((BasePage)Page).ConfigureDDLGrpItem2(ddlBscGrpItm2, true, idBscGrpItem1, idBscGrpItem2, this.lblEmptyRow.Text, false, idOwn);
                ((BasePage)Page).ConfigureDDLGrpItem3(ddlBscGrpItm3, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, this.lblEmptyRow.Text, false, idOwn);
                ((BasePage)Page).ConfigureDDLGrpItem4(ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, idOwn);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlBscGrpItm2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idOwn = 0;
            try
            {
                if (this.ddlFilterOwner.SelectedValue == "-1")
                {
                    //Captura el owner de la session
                    // idown = context.SessionInfo.Owner.Id; 
                    //TODO: Cuando se implemente el Owner en session Info, se debe cambiar esto.
                    idOwn = 4;
                }
                else
                {
                    idOwn = Convert.ToInt16(ddlFilterOwner.SelectedValue);
                }

                ((BasePage)Page).ConfigureDDLGrpItem3(ddlBscGrpItm3, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, this.lblEmptyRow.Text, false, idOwn);
                ((BasePage)Page).ConfigureDDLGrpItem4(ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, idOwn);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlBscGrpItm3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idOwn = 0;
            try
            {
                if (this.ddlFilterOwner.SelectedValue == "-1")
                {
                    //Captura el owner de la session
                    // idown = context.SessionInfo.Owner.Id; 

                    //TODO: Cuando se implemente el Owner en session Info, se debe cambiar esto.
                    idOwn = 4;
                }
                else
                {
                    idOwn = Convert.ToInt16(ddlFilterOwner.SelectedValue);
                }

                ((BasePage)Page).ConfigureDDLGrpItem4(ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, idOwn);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(error);
            }
        }


        protected void ddlFilterTypeReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                OnDdlTypeReportSelectedIndexChanged(e);
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(error);
            }
        }
        //protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Si cambia el pais, cambia el estado y la ciudad
        //        ((BasePage)Page).ConfigureDDlState(this.ddlState, true, -1, idCountry, this.lblEmptyRow.Text);
        //        ((BasePage)Page).ConfigureDDlCity(this.ddlCity, true, -1, idState, idCountry, this.lblEmptyRow.Text);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorDTO error = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(error);
        //    }
        //}

        //protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Si cambia el estado, solo cambia la ciudad.
        //        ((BasePage)Page).ConfigureDDlCity(this.ddlCity, true, idCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorDTO error = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(error);
        //    }
        //}

        protected void OnBtnSearchClick(EventArgs e)
        {
            if (BtnSearchClick != null)
            {
                BtnSearchClick(this, e);
            }
        }

        protected void OnDdlTypeReportSelectedIndexChanged(EventArgs e)
        {
            if (DdlTypeReportSelectedIndexChanged != null)
            {
                DdlTypeReportSelectedIndexChanged(this, e );
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
            if (init)
            {
                PopulateLists();
                LoadDefaultsToControls();

                // Si está activa la opción 'KeepFilter' y el objeto MainFilter está Activo
                // carga en los controles los valores actuales del objeto MainFilter
                if (context.SessionInfo.FilterKeep)
                {
                    if (context.SessionInfo.FilterActive)
                    {
                        LoadControlsFromFilterObject();
                    }
                    else
                    {
                        LoadControlValuesToFilterObject();
                        context.SessionInfo.FilterActive = true;
                    }
                }
                // Si NO está activa la opción 'KeepFilter', limpia el objeto MainFilter
                // y carga en los controles los valores por defecto
                else
                {
                    ClearFilterObject();
                    LoadDefaultsToControls();
                    LoadControlValuesToFilterObject();
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

        /// <summary>
        /// Carga Controles con los valores por defecto dados por configuración
        /// </summary>
        private void LoadDefaultsToControls()
        {
            int index;
            int days = 0;

            //Si es recepcion de orden de compra con documento entonces selecciona el item de la lista
            if (Convert.ToInt16(Request.QueryString["IT"]) == (int)InboundTypeName.Traspaso)
            {
                //TODO: Poner el valor seleccionado al filtro de IdInboundtype = 3
                if (this.inboundTypeVisible)
                {
                    // Selecciona inboundType por defecto del item seleccionado en el menu
                    ((BasePage)Page).SelectDefaultinboundType(ddlFilterInboundType, (int)InboundTypeName.Traspaso);
                }
            }

            // Warehouse 
            if (this.warehouseVisible && !changeDefaultWhsAsData)
            {
                ddlFilterWarehouse.ClearSelection();
                // Selecciona Warehouse por defecto del Usuario loggeado
                ((BasePage)Page).SelectDefaultWarehouse(ddlFilterWarehouse);
            }

            // Hangar
            if (this.hangarVisible)
            {
                // Muestra Hangares del Warehouse por defecto
                int idWhs = Convert.ToInt16(ddlFilterWarehouse.SelectedValue);
                LoadHangarBase(idWhs);
            }
            // Date Today
            if (this.dateVisible)
            {
                //index = Convert.ToInt16(dateBefore);
                days = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));

                // TODO: parametrizar para pasar o no un valor inicial
                //calDate.SelectedDate = DateTime.Now.AddDays(-days);
                //txtFilterDate.Text = calDate.SelectedDate.Value.Date.ToShortDateString();

                txtFilterDate.Text = DateTime.Now.ToShortDateString();
            }
            // Date From
            if (this.dateFromVisible)
            {
                //index = Convert.ToInt16(dateBefore);
                days = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));

                // TODO: parametrizar para pasar o no un valor inicial
                //caleDateFrom.SelectedDate = DateTime.Now.AddDays(-days);
                //txtFilterDateFrom.Text = caleDateFrom.SelectedDate..Date.ToShortDateString();

                txtFilterDateFrom.Text = DateTime.Now.AddDays(-days).ToShortDateString();
                caleDateFrom.SelectedDate = DateTime.Now.AddDays(-days);
                caleDateFrom.DateFirstMonth = DateTime.Now.AddDays(-days);  
            }
            // Date To
            if (this.dateToVisible)
            {
                //index = Convert.ToInt16(dateAfter);
                days = Convert.ToInt16(GetCfgParameter(dateAfter.ToString()));

                // TODO: parametrizar para pasar o no un valor inicial
                //caleDateTo.SelectedDate = DateTime.Now.AddDays(days);
                //txtFilterDateTo.Text = caleDateTo.SelectedDate.Value.ToShortDateString();

                txtFilterDateTo.Text = DateTime.Now.AddDays(-days).ToShortDateString();
                caleDateTo.SelectedDate = DateTime.Now.AddDays(-days);
                caleDateTo.DateFirstMonth = DateTime.Now.AddDays(-days);  
            }

            //// Languague 
            if (this.translateVisible)
            {
                // Selecciona Languague por defecto del Usuario loggeado
                ((BasePage)Page).SelectDefaultLanguague(ddlTranslate);
            }

            // Year
            if (this.YearVisible)
            {
                txtFilterYear.Text = DateTime.Now.Year.ToString();
            }

            // Fabrication Date
            if (this.fabricationDateVisible)
            {
                days = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));

                txtFilterFabricationDate.Text = DateTime.Now.AddDays(-days).ToShortDateString();
                calFabricationDate.SelectedDate = DateTime.Now.AddDays(-days);
                calFabricationDate.DateFirstMonth = DateTime.Now.AddDays(-days);                
            }

            // Expiration Date
            if (this.expirationDateVisible)
            {
                days = Convert.ToInt16(GetCfgParameter(dateBefore.ToString()));

                txtFilterExpirationDate.Text = DateTime.Now.AddDays(-days).ToShortDateString();
                calFabricationDate.SelectedDate = DateTime.Now.AddDays(-days);
                calFabricationDate.DateFirstMonth = DateTime.Now.AddDays(-days);                
            }

            if (this.workZoneVisible)
            {
                // Muestra Hangares del Warehouse por defecto
                int idWhs = Convert.ToInt16(ddlFilterWarehouse.SelectedValue);
                LoadWorkZone(idWhs);
            }
        }

        /// <summary>
        /// Carga las listas y valores por defecto
        /// </summary>
        private void PopulateLists()
        {
            #region "FILTRO BÁSICO"

            //TypeReport
            if(this.typeReportVisible)
                ((BasePage)Page).LoadTypeReports(this.ddlFilterTypeReport, lblEmptyRow.Text, "-1", false);

            // Warehouse 
            if (this.warehouseVisible && !changeDefaultWhsAsData)
                ((BasePage)Page).LoadUserWarehouses(this.ddlFilterWarehouse, lblEmptyRow.Text, "-1", true);

            if (this.warehouseVisible && changeDefaultWhsAsData)
                ((BasePage)Page).LoadLogicalWarehouses(this.ddlFilterWarehouse, lblEmptyRow.Text, "-1");

            // Owner
            if (this.ownerVisible)
                ((BasePage)Page).LoadOwner(this.ddlFilterOwner, true, lblEmptyRow.Text);

            // OwnerUser
            if (this.ownerUserVisible)
                if (!this.ownerNotIncludeAll)
                    ((BasePage)Page).LoadUserOwners(this.ddlFilterOwnerUser, lblEmptyRow.Text, "-1", true, "", ownerIncludeNulls);
                else
                    ((BasePage)Page).LoadUserOwners(this.ddlFilterOwnerUser, lblEmptyRow.Text, "-1", false, "", ownerIncludeNulls);

            // CategoryItem
            if (this.categoryItemVisible)
                ((BasePage)Page).LoadCategoryItemByOwner(this.ddlCategoryItem, -2, true, false, lblEmptyRow.Text);

            // LpnType
            if (this.lpnTypeVisible)
                ((BasePage)Page).LoadLpnType(this.ddlFilterLpnType, true, lblEmptyRow.Text);

            // WorkZone
            if (this.workZoneVisible)
                ((BasePage)Page).LoadWorkZoneByWhsWhitOutSession(this.ddlFilterWorkZone, Convert.ToInt16(this.ddlFilterWarehouse.SelectedValue), true, lblEmptyRow.Text);
            //((BasePage)Page).LoadWorkZone(this.ddlFilterWorkZone, true, lblEmptyRow.Text);

            // TruckType
            if (this.truckTypeVisible)
                ((BasePage)Page).LoadTruckType(this.ddlFilterTruckType, true, lblEmptyRow.Text);

            #region Translation

            if (this.translateVisible)
                ((BasePage)Page).LoadLanguageDefined(this.ddlTranslate, true, lblEmptyRow.Text);

            if (this.moduleVisible)
                ((BasePage)Page).LoadModule(this.ddlModule, true, lblEmptyRow.Text);

            if (this.typeObjectVisible)
                ((BasePage)Page).LoadTypeObject(this.ddlTypeObject, true, lblEmptyRow.Text);

            if (this.propertyVisible)
                ((BasePage)Page).LoadProperty(this.ddlProperty, true, lblEmptyRow.Text);

            #endregion

            //InboundType
            if (inboundTypeVisible)
            {
                switch (Convert.ToInt16(Request.QueryString["IT"]))
                {
                    case ((int)InboundTypeName.Todos):
                        ((BasePage)Page).LoadInboundType(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                        break;
                    case ((int)InboundTypeName.DevolucionConDoc):
                        ((BasePage)Page).LoadInboundTypeDev(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                        break;
                    case (int)InboundTypeName.Produccion:
                        ((BasePage)Page).LoadInboundTypeProd(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                        break;
                    case (int)InboundTypeName.Traspaso:
                        ((BasePage)Page).LoadInboundTypeTrasp(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                        break;
                    default:
                        ((BasePage)Page).LoadInboundType(this.ddlFilterInboundType, true, lblEmptyRow.Text);
                        break;
                }
            }

            //Scope
            if (parameterScopeVisible)
                ((BasePage)Page).LoadScope(this.ddlFilterScope, true, lblEmptyRow.Text);


            if (this.divBscGroupItem)
            {
                if (!string.IsNullOrEmpty(ddlFilterOwner.SelectedValue))
                {
                    int idOwner = (Convert.ToInt16(ddlFilterOwner.SelectedValue));

                    // GroupItem 1...4
                    ((BasePage)Page).ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idBscGrpItem1, this.lblEmptyRow.Text, false, idOwner);
                    ((BasePage)Page).ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idBscGrpItem1, idBscGrpItem2, this.lblEmptyRow.Text, false, idOwner);
                    ((BasePage)Page).ConfigureDDLGrpItem3(this.ddlBscGrpItm3, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, this.lblEmptyRow.Text, false, idOwner);
                    ((BasePage)Page).ConfigureDDLGrpItem4(this.ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, false, idOwner);
                }
            }
            #endregion

            #region "FILTRO AVANZADO"
            //if (this.advancedFilterVisible)
            //{
            //    if (this.tabLayoutVisible)
            //    {
            //        // Hangar
            //        LoadHangar(context.SessionInfo.Warehouse.Id);

            //        // WorkZone
            //        LoadWorkZone(context.SessionInfo.Warehouse.Id);

            //        // Location
            //        LoadLocationType(-1);
            //    }

            //    // Inbound Type
            //    if (documentTypeVisible)
            //        LoadInboundType();

            //    if (this.tabDocumentVisible)
            //    {
            //        // Vendor
            //        if (vendorVisible)
            //            LoadVendor(Convert.ToInt16(ddlFilterOwner.SelectedValue));

            //        // Carrier
            //        if (carrierVisible)
            //            LoadCarrier();

            //        //Driver
            //        if (driverVisible)
            //            ((BasePage)Page).LoadDriver(this.lstDriver, true, lblEmptyRow.Text);
            //    }

            //    if (this.divGroupItem1.Visible)
            //    {
            //        // GroupItem 1...4 (distintas a las del filtro avanzado)
            //        ((BasePage)Page).ConfigureDDLGrpItem1(this.ddlGrpItem1, true, idGrpItem1, this.lblEmptyRow.Text, true, -1);
            //        ((BasePage)Page).ConfigureDDLGrpItem2(this.ddlGrpItem2, true, idGrpItem1, idGrpItem2, this.lblEmptyRow.Text, true, -1);
            //        ((BasePage)Page).ConfigureDDLGrpItem3(this.ddlGrpItem3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.lblEmptyRow.Text, true, -1);
            //        ((BasePage)Page).ConfigureDDLGrpItem4(this.ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.lblEmptyRow.Text, true, -1);
            //    }

            //    if (this.tabDispatchingVisible)
            //    {
            //        // Country / State / City
            //        ((BasePage)Page).FindAllPlaces();
            //        ((BasePage)Page).ConfigureDDlCountry(this.ddlCountry, true, idCountry, this.Master.EmptyRowText);
            //        ((BasePage)Page).ConfigureDDlState(this.ddlState, true, idState, idCountry, this.Master.EmptyRowText);
            //        ((BasePage)Page).ConfigureDDlCity(this.ddlCity, true, idCity, idState, idCountry, this.Master.EmptyRowText);
            //    }
            //}
            #endregion
        }

        /// <summary>
        /// Carga lista de Hangares, segun Warehouse seleccionado - FILTRO AVANZADO
        /// </summary>
        /// <param name="idWhs"></param>
        //public void LoadHangar(int idWhs)
        //{
        //    // TODO: cargar desde session en BasePage
        //    GenericViewDTO<Hangar> hangarViewDTO = new GenericViewDTO<Hangar>();

        //    hangarViewDTO = iLayoutMGR.GetHangarByWhs(idWhs, context);

        //    lstHangar.DataSource = hangarViewDTO.Entities;
        //    lstHangar.DataTextField = "Name";
        //    lstHangar.DataValueField = "Id";
        //    lstHangar.DataBind();

        //    lstHangar.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
        //    lstHangar.Items[0].Selected = true;
        //}

        /// <summary>
        /// Carga lista de Hangares, segun Warehouse seleccionado - FILTRO BASICO
        /// </summary>
        /// <param name="idWhs"></param>
        public void LoadHangarBase(int idWhs)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<Hangar> hangarViewDTO = new GenericViewDTO<Hangar>();

            hangarViewDTO = iLayoutMGR.GetHangarByWhs(idWhs, context);

            ddlFilterHangar.DataSource = hangarViewDTO.Entities;
            ddlFilterHangar.DataTextField = "Name";
            ddlFilterHangar.DataValueField = "Id";
            ddlFilterHangar.DataBind();

            ddlFilterHangar.Items.Insert(0, new ListItem(lblEmptyRowSelect.Text, "-1"));
            ddlFilterHangar.Items[0].Selected = true;
        }

        /// <summary>
        /// Carga lista de WorkZones, segun Warehouse seleccionado
        /// </summary>
        /// <param name="idWhs"></param>
        public void LoadWorkZone(int idWhs)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<WorkZone> workZoneViewDTO = new GenericViewDTO<WorkZone>();

            workZoneViewDTO = iLayoutMGR.GetWorkZoneByWhs(idWhs, context);
            
            ddlFilterWorkZone.DataSource = workZoneViewDTO.Entities;
            ddlFilterWorkZone.DataTextField = "Name";
            ddlFilterWorkZone.DataValueField = "Id";
            ddlFilterWorkZone.DataBind();

            ddlFilterWorkZone.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            ddlFilterWorkZone.Items[0].Selected = true;
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
                //lstLocationType.DataSource = locationTypeViewDTO.Entities;

                //lstLocationType.DataTextField = "LocTypeCode";
                //lstLocationType.DataValueField = "LocTypeCode";
                //lstLocationType.DataBind();

                //lstLocationType.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
                //lstLocationType.Items[0].Selected = true;
            }
            else
            {
                ddlFilterLocationType.DataSource = locationTypeViewDTO.Entities;

                ddlFilterLocationType.DataTextField = "LocTypeCode";
                ddlFilterLocationType.DataValueField = "LocTypeCode";
                ddlFilterLocationType.DataBind();

                ddlFilterLocationType.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
                ddlFilterLocationType.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Carga lista de InboundType
        /// </summary>
        public void LoadInboundType()
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<InboundType> inboundTypeViewDTO = new GenericViewDTO<InboundType>();

            inboundTypeViewDTO = iReceptionMGR.FindAllInboundType(context);

            //lstInboundType.DataSource = inboundTypeViewDTO.Entities;
            //lstInboundType.DataTextField = "Code";
            //lstInboundType.DataValueField = "Id";
            //lstInboundType.DataBind();

            //lstInboundType.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            //lstInboundType.Items[0].Selected = true;
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

            //lstVendor.DataSource = vendorViewDTO.Entities;
            //lstVendor.DataTextField = "Name";
            //lstVendor.DataValueField = "Id";
            //lstVendor.DataBind();

            //lstVendor.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            //lstVendor.Items[0].Selected = true;
        }

        /// <summary>
        /// Carga lista de Carrier
        /// </summary>
        public void LoadCarrier()
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<Carrier> carrierViewDTO = new GenericViewDTO<Carrier>();

            carrierViewDTO = iWarehousingMGR.FindAllCarrier(context);

            //lstCarrier.DataSource = carrierViewDTO.Entities;
            //lstCarrier.DataTextField = "Name";
            //lstCarrier.DataValueField = "Id";
            //lstCarrier.DataBind();

            //lstCarrier.Items.Insert(0, new ListItem(lblEmptyRow.Text, "-1"));
            //lstCarrier.Items[0].Selected = true;
        }

        /// <summary>
        /// Carga el objeto 'Main Filter' con los valores seleccionados en los elementos del control de usuario
        /// </summary>
        private void LoadControlValuesToFilterObject()
        {
            // FILTRO BASICO
            LoadControlValuesBase();

            // FILTRO AVANZADO
            // Solo se utiliza si esta seleccionada la opcion 'Usar Filtro Avanzado'
            //if (chkUseAdvancedFilter.Checked && advancedFilterVisible)
            //{
            //    LoadControlValuesAdvanced();
            //}
        }

        private void LoadControlValuesBase()
        {
            int index;
            string dateTo = string.Empty;

            // Year (genérico)
            if (this.txtFilterYear.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Description);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterYear.Text));
            }

            // Code (genérico)
            if (txtFilterCode.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Code);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterCode.Text));
            }

            // Name (genérico)
            if (txtFilterName.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Name);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterName.Text));
            }

            // Description (genérico)
            if (txtFilterDescription.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Description);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterDescription.Text));
            }

            // Wharehouse
            if (ddlFilterWarehouse.SelectedIndex != 0 && ddlFilterWarehouse.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Warehouse);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterWarehouse.SelectedIndex, ddlFilterWarehouse.SelectedValue));
            }

            // Hangar
            if (ddlFilterHangar.SelectedIndex != 0 && ddlFilterHangar.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Hangar);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterHangar.SelectedIndex, ddlFilterHangar.SelectedValue));
            }

            // Owner
            if (ddlFilterOwner.SelectedIndex != 0 && ddlFilterOwner.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Owner);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterOwner.SelectedIndex, ddlFilterOwner.SelectedValue));
            }

            // OwnerUser
            if (ddlFilterOwnerUser.SelectedIndex != 0 && ddlFilterOwnerUser.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Owner);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterOwnerUser.SelectedIndex, ddlFilterOwnerUser.SelectedValue));
            }

            // Lpn - Type
            if (ddlFilterLpnType.SelectedIndex != 0 && ddlFilterLpnType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.LpnType);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterLpnType.SelectedIndex, ddlFilterLpnType.SelectedValue));
            }
            // WorkZone
            if (ddlFilterWorkZone.SelectedIndex != 0 && ddlFilterWorkZone.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.WorkZone);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterWorkZone.SelectedIndex, ddlFilterWorkZone.SelectedValue));
            }

            // TruckType
            if (ddlFilterTruckType.SelectedIndex != 0 && ddlFilterTruckType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.TruckType);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterTruckType.SelectedIndex, ddlFilterTruckType.SelectedValue));
            }
            // Scope
            if (ddlFilterScope.SelectedIndex != 0 && ddlFilterScope.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Scope);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterScope.SelectedIndex, ddlFilterScope.SelectedValue));
            }
            // Status
            if (ddlFilterStatus.SelectedIndex != 0 && ddlFilterStatus.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Status);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlFilterStatus.SelectedIndex, ddlFilterStatus.SelectedValue));
            }

            // Item
            if (txtFilterItem.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Item);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterItem.Text));
            }

            // Document Nbr
            if (txtFilterDocumentNumber.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.DocumentNbr);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterDocumentNumber.Text));
            }
            //LOCATION
            if (txtFilterLocation.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.Location);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterLocation.Text));
            }

            // Dates From and To (genéric)
            if (txtFilterDateFrom.Text != string.Empty || txtFilterDateTo.Text != string.Empty)
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterDateFrom.Text));

                    // Agrega un dia menos 1 segundo a 'Date To' para abarcar todo el dia seleccionado
                    if (txtFilterDateTo.Text != string.Empty)
                        dateTo = Convert.ToDateTime(txtFilterDateTo.Text).AddDays(1).Add(System.TimeSpan.FromSeconds(-1)).ToString();

                    mainFilter[index].FilterValues.Add(new FilterItem(0, dateTo));
                    dateTo = string.Empty;
                }
            }

            // Location Type
            if (ddlFilterLocationType.SelectedIndex != 0 && ddlFilterLocationType.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.LocationType);

                if (mainFilter[index].FilterValues != null)
                {
                    foreach (ListItem item in ddlFilterLocationType.Items)
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
                        foreach (ListItem item in ddlFilterInboundType.Items)
                            if (item.Selected)
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                }
                else
                {
                    index = Convert.ToInt16(EntityFilterName.InboundType);
                    if (mainFilter[index].FilterValues != null)
                        foreach (ListItem item in ddlFilterInboundType.Items)
                            if (item.Value != "-1")
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                }
            }
            else
            {
                if (ddlFilterInboundType.SelectedIndex != 0)
                {
                    index = Convert.ToInt16(EntityFilterName.InboundType);
                    if (mainFilter[index].FilterValues != null)
                        foreach (ListItem item in ddlFilterInboundType.Items)
                            if (item.Selected)
                                mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
                }
            }
            //Translations
            #region Translations
            if (ddlTranslate.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Translate);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlTranslate.SelectedIndex, ddlTranslate.SelectedValue));
            }

            if (ddlModule.SelectedIndex != 0 && ddlModule.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.Module);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlModule.SelectedIndex, ddlModule.SelectedValue));
            }

            if (ddlTypeObject.SelectedIndex != 0 && ddlTypeObject.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.ObjectType);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlTypeObject.SelectedIndex, ddlTypeObject.SelectedValue));
            }

            if (ddlProperty.SelectedIndex != 0 && ddlProperty.SelectedIndex != -1)
            {
                index = Convert.ToInt16(EntityFilterName.PropertyName);

                if (mainFilter[index].FilterValues != null)
                    mainFilter[index].FilterValues.Add(new FilterItem(ddlProperty.SelectedIndex, ddlProperty.SelectedValue));
            }

            #endregion


            // Goup Items
            if (this.divBscGroupItem)
            {
                // Group Item 1
                if (ddlBscGrpItm1.SelectedIndex != 0 && ddlBscGrpItm1.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem1);

                    if (mainFilter[index].FilterValues != null)
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlBscGrpItm1.SelectedIndex, ddlBscGrpItm1.SelectedValue));
                }

                // Group Item 2
                if (ddlBscGrpItm2.SelectedIndex != 0 && ddlBscGrpItm2.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem2);

                    if (mainFilter[index].FilterValues != null)
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlBscGrpItm2.SelectedIndex, ddlBscGrpItm2.SelectedValue));
                }

                // Group Item 3
                if (ddlBscGrpItm3.SelectedIndex != 0 && ddlBscGrpItm3.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem3);

                    if (mainFilter[index].FilterValues != null)
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlBscGrpItm3.SelectedIndex, ddlBscGrpItm3.SelectedValue));
                }

                // Group Item 4
                if (ddlBscGrpItm4.SelectedIndex != 0 && ddlBscGrpItm4.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.GroupItem4);

                    if (mainFilter[index].FilterValues != null)
                        mainFilter[index].FilterValues.Add(new FilterItem(ddlBscGrpItm4.SelectedIndex, ddlBscGrpItm4.SelectedValue));
                }

            }
        }

        protected void chkFilterFabricationDate_CheckedChanged(object sender, EventArgs e)
        {
            this.txtFilterFabricationDate.Enabled = chkFilterFabricationDate.Checked;
        }

        protected void chkFilterExpirationDate_CheckedChanged(object sender, EventArgs e)
        {
            this.txtFilterExpirationDate.Enabled = chkFilterExpirationDate.Checked;
        }

        //private void LoadControlValuesAdvanced()
        //{
        //    int index;
        //    string dateTo = string.Empty;

        //    context.SessionInfo.FilterUseAdvanced = true;

        //    // DISPATCHING TAB
        //    if (this.tabDispatchingVisible)
        //    {
        //        // Priority Range
        //        if (txtPriorityFrom.Text != string.Empty || txtPriorityTo.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.PriorityRange);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtPriorityFrom.Text));
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtPriorityTo.Text));
        //            }
        //        }

        //        // Customer
        //        if (txtCustomer.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.Customer);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(txtCustomer.Text));
        //        }

        //        // Carrier
        //        if (txtCarrier.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.Carrier);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(txtCarrier.Text));
        //        }

        //        // Route
        //        if (txtRoute.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.Route);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(txtRoute.Text));
        //        }

        //        // Country
        //        if (ddlCountry.SelectedIndex != 0 && ddlCountry.SelectedIndex != -1)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.Country);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(ddlCountry.SelectedIndex, ddlCountry.SelectedValue));
        //        }

        //        // State
        //        if (ddlState.SelectedIndex != 0 && ddlState.SelectedIndex != -1)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.State);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(ddlState.SelectedIndex, ddlState.SelectedValue));
        //        }

        //        // City
        //        if (ddlCity.SelectedIndex != 0 && ddlCity.SelectedIndex != -1)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.City);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(ddlCity.SelectedIndex, ddlCity.SelectedValue));
        //        }
        //    }

        //    // LAYOUT TAB
        //    if (this.tabLayoutVisible)
        //    {
        //        // Hangar
        //        if (lstHangar.SelectedIndex != 0 && lstHangar.SelectedIndex != -1)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.Hangar);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                foreach (ListItem item in lstHangar.Items)
        //                {
        //                    if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
        //                }
        //            }
        //        }

        //        // WorkZone
        //        if (lstWorkZone.SelectedIndex != 0 && lstWorkZone.SelectedIndex != -1)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.WorkZone);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                foreach (ListItem item in lstWorkZone.Items)
        //                {
        //                    if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
        //                }
        //            }
        //        }

        //        // Location Type
        //        if (!lstLocationType.Items[0].Selected)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.LocationType);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                foreach (ListItem item in lstLocationType.Items)
        //                {
        //                    if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
        //                }
        //            }
        //        }
        //    }

        //    // LOCATION TAB
        //    if (this.tabLocationVisible)
        //    {
        //        // Location
        //        if (txtLocation.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.Location);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(txtLocation.Text));
        //        }

        //        // Location Range
        //        if (txtLocationFrom.Text != string.Empty || txtLocationTo.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.LocationRange);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationFrom.Text));
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationTo.Text));
        //            }
        //        }

        //        // Location Row
        //        if (txtLocationRow.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.LocationRow);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationRow.Text));
        //        }

        //        // Location Column
        //        if (txtLocationColumn.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.LocationColumn);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationColumn.Text));
        //        }

        //        // Location Level
        //        if (txtLocationLevel.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.LocationLevel);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationLevel.Text));
        //        }

        //        // Location Aisle
        //        if (txtLocationAisle.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.LocationAisle);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtLocationAisle.Text));
        //        }
        //    }

        //    // GROUP 1 TAB
        //    if (this.tabGroup1Visible)
        //    {
        //        // Group Item 1
        //        if (ddlGrpItem1.SelectedIndex != 0 && ddlGrpItem1.SelectedIndex != -1)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.GroupItem1);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(ddlGrpItem1.SelectedIndex, ddlGrpItem1.SelectedValue));
        //        }

        //        // Group Item 2
        //        if (ddlGrpItem2.SelectedIndex != 0 && ddlGrpItem2.SelectedIndex != -1)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.GroupItem2);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(ddlGrpItem2.SelectedIndex, ddlGrpItem2.SelectedValue));
        //        }

        //        // Group Item 3
        //        if (ddlGrpItem3.SelectedIndex != 0 && ddlGrpItem3.SelectedIndex != -1)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.GroupItem3);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(ddlGrpItem3.SelectedIndex, ddlGrpItem3.SelectedValue));
        //        }

        //        // Group Item 4
        //        if (ddlGrpItem4.SelectedIndex != 0 && ddlGrpItem4.SelectedIndex != -1)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.GroupItem4);

        //            if (mainFilter[index].FilterValues != null)
        //                mainFilter[index].FilterValues.Add(new FilterItem(ddlGrpItem4.SelectedIndex, ddlGrpItem4.SelectedValue));
        //        }

        //    }

        //    // DATE TAB
        //    if (this.tabDatesVisible)
        //    {
        //        // Fabrication Date
        //        if (txtFabricationDateFrom.Text != string.Empty || txtFabricationDateTo.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.FabricationDateRange);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtFabricationDateFrom.Text));

        //                // Agrega un dia menos 1 segundo a 'Date To' para abarcar todo el dia seleccionado
        //                if (txtFabricationDateTo.Text != string.Empty)
        //                    dateTo = Convert.ToDateTime(txtFabricationDateTo.Text).AddDays(1).Add(System.TimeSpan.FromSeconds(-1)).ToString();

        //                mainFilter[index].FilterValues.Add(new FilterItem(0, dateTo));
        //                dateTo = string.Empty;
        //            }
        //        }

        //        // Expiration Date
        //        if (txtExpirationDateFrom.Text != string.Empty || txtExpirationDateTo.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.ExpirationDateRange);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtExpirationDateFrom.Text));

        //                // Agrega un dia menos 1 segundo a 'Date To' para abarcar todo el dia seleccionado
        //                if (txtExpirationDateTo.Text != string.Empty)
        //                    dateTo = Convert.ToDateTime(txtExpirationDateTo.Text).AddDays(1).Add(System.TimeSpan.FromSeconds(-1)).ToString();

        //                mainFilter[index].FilterValues.Add(new FilterItem(0, dateTo));
        //                dateTo = string.Empty;
        //            }
        //        }

        //        // Expected Date
        //        if (txtExpectedDateFrom.Text != string.Empty || txtExpectedDateTo.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.ExpectedDateRange);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtExpectedDateFrom.Text));

        //                // Agrega un dia menos 1 segundo a 'Date To' para abarcar todo el dia seleccionado
        //                if (txtExpectedDateTo.Text != string.Empty)
        //                    dateTo = Convert.ToDateTime(txtExpectedDateTo.Text).AddDays(1).Add(System.TimeSpan.FromSeconds(-1)).ToString();

        //                mainFilter[index].FilterValues.Add(new FilterItem(0, dateTo));
        //                dateTo = string.Empty;
        //            }
        //        }

        //        // Shipment Date
        //        if (txtShipmentDateFrom.Text != string.Empty || txtShipmentDateTo.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.ShipmentDateRange);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtShipmentDateFrom.Text));

        //                // Agrega un dia menos 1 segundo a 'Date To' para abarcar todo el dia seleccionado
        //                if (txtShipmentDateTo.Text != string.Empty)
        //                    dateTo = Convert.ToDateTime(txtShipmentDateTo.Text).AddDays(1).Add(System.TimeSpan.FromSeconds(-1)).ToString();

        //                mainFilter[index].FilterValues.Add(new FilterItem(0, dateTo));
        //                dateTo = string.Empty;
        //            }
        //        }

        //        // Lot Nbr Range
        //        if (txtLotNumberFrom.Text != string.Empty || txtLotNumberTo.Text != string.Empty)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.LotNumberRange);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtLotNumberFrom.Text));
        //                mainFilter[index].FilterValues.Add(new FilterItem(0, txtLotNumberTo.Text));
        //            }
        //        }
        //    }

        //    // DOCUMENT TAB
        //    if (this.tabDocumentVisible)
        //    {
        //        //TODO: Se ha eliminado este filtro de aqui por que ya existe en el filtro Basico
        //        // Inbound Type
        //        //if (!lstInboundType.Items[0].Selected)
        //        //{
        //        //    index = Convert.ToInt16(EntityFilterName.DocumentType);

        //        //    if (mainFilter[index].FilterValues != null)
        //        //    {
        //        //        foreach (ListItem item in lstInboundType.Items)
        //        //        {
        //        //            if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
        //        //        }
        //        //    }
        //        //}

        //        // Vendor
        //        if (!lstVendor.Items[0].Selected)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.Vendor);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                foreach (ListItem item in lstVendor.Items)
        //                {
        //                    if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
        //                }
        //            }
        //        }

        //        // Carrier
        //        if (!lstCarrier.Items[0].Selected)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.Carrier);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                foreach (ListItem item in lstCarrier.Items)
        //                {
        //                    if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
        //                }
        //            }
        //        }

        //        // Driver
        //        if (!lstDriver.Items[0].Selected)
        //        {
        //            index = Convert.ToInt16(EntityFilterName.Driver);

        //            if (mainFilter[index].FilterValues != null)
        //            {
        //                foreach (ListItem item in lstDriver.Items)
        //                {
        //                    if (item.Selected) mainFilter[index].FilterValues.Add(new FilterItem(item.Value));
        //                }
        //            }
        //        }
        //    }
        //}

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
            this.txtFilterName.Text = string.Empty;
            this.txtFilterDescription.Text = string.Empty;
            this.txtFilterDocumentNumber.Text = string.Empty;
            this.txtFilterLocation.Text = string.Empty;
            this.txtFilterItem.Text = string.Empty;
            this.ddlFilterStatus.SelectedIndex = 0;
            if (this.ddlFilterLocationType.Items.Count > 0) this.ddlFilterLocationType.SelectedIndex = 0;
            if (this.ddlFilterLpnType.Items.Count > 0) this.ddlFilterLpnType.SelectedIndex = 0;
            if (this.ddlFilterWorkZone.Items.Count > 0) this.ddlFilterWorkZone.SelectedIndex = 0;
            if (this.ddlFilterWarehouse.Items.Count > 0) this.ddlFilterWarehouse.SelectedIndex = 0;
            if (this.ddlFilterHangar.Items.Count > 0) this.ddlFilterHangar.SelectedIndex = 0;
            if (this.ddlFilterOwner.Items.Count > 0) this.ddlFilterOwner.SelectedIndex = 0;
            if (this.ddlFilterOwnerUser.Items.Count > 0) this.ddlFilterOwnerUser.SelectedIndex = 0;
            if (this.ddlFilterTruckType.Items.Count > 0) this.ddlFilterTruckType.SelectedIndex = 0;
            if (this.ddlFilterInboundType.Items.Count > 0) this.ddlFilterInboundType.SelectedIndex = 0;
            if (this.ddlFilterScope.Items.Count > 0) this.ddlFilterScope.SelectedIndex = 0;

            //if (advancedFilterVisible)
            //{
            //    ClearAdvancedControls();
            //}
            //if (this.tabGroup1Visible)
            //{
            //    this.ddlGrpItem1.SelectedIndex = 0;
            //}

            this.chkUseAdvancedFilter.Checked = false;
            context.SessionInfo.FilterUseAdvanced = false;

            //Translate
            if (this.ddlTranslate.Items.Count > 0) this.ddlTranslate.SelectedIndex = 0;
            if (this.ddlTypeObject.Items.Count > 0) this.ddlTypeObject.SelectedIndex = 0;
            if (this.ddlModule.Items.Count > 0) this.ddlModule.SelectedIndex = 0;
            if (this.ddlProperty.Items.Count > 0) this.ddlProperty.SelectedIndex = 0;



            // TODO: limpiar fechas?
            //RE: dejar fecha al dia actual, creo que eso seria lo correcto
        }

        /// <summary>
        /// Limpia valores anteriores en los textboxes y listas del filtro avanzado
        /// </summary>
        //public void ClearAdvancedControls()
        //{
        //    if (this.tabDispatchingVisible)
        //    {
        //        this.txtPriorityFrom.Text = string.Empty;
        //        this.txtPriorityTo.Text = string.Empty;
        //        this.txtCustomer.Text = string.Empty;
        //        this.txtCarrier.Text = string.Empty;
        //        this.txtRoute.Text = string.Empty;
        //    }

        //    if (this.tabLayoutVisible)
        //    {
        //        this.lstHangar.SelectedIndex = 0;
        //        this.lstWorkZone.SelectedIndex = 0;
        //        this.lstLocationType.SelectedIndex = 0;
        //    }

        //    if (this.tabLocationVisible)
        //    {
        //        this.txtLocation.Text = string.Empty;
        //        this.txtLocationFrom.Text = string.Empty;
        //        this.txtLocationTo.Text = string.Empty;
        //        this.txtLocationRow.Text = string.Empty;
        //        this.txtLocationColumn.Text = string.Empty;
        //        this.txtLocationLevel.Text = string.Empty;
        //        this.txtLocationAisle.Text = string.Empty;
        //    }

        //    if (this.tabGroup1Visible)
        //    {
        //        this.ddlGrpItem1.SelectedIndex = 0;
        //    }

        //    if (this.tabDatesVisible)
        //    {
        //        this.txtFabricationDateFrom.Text = string.Empty;
        //        this.txtFabricationDateTo.Text = string.Empty;
        //        this.txtExpirationDateFrom.Text = string.Empty;
        //        this.txtExpirationDateTo.Text = string.Empty;
        //        this.txtExpectedDateFrom.Text = string.Empty;
        //        this.txtExpectedDateTo.Text = string.Empty;
        //        this.txtShipmentDateFrom.Text = string.Empty;
        //        this.txtShipmentDateTo.Text = string.Empty;
        //        this.txtLotNumberFrom.Text = string.Empty;
        //        this.txtLotNumberTo.Text = string.Empty;
        //    }

        //    if (this.tabDocumentVisible)
        //    {

        //        if (this.inboundTypeVisible) this.lstInboundType.SelectedIndex = 0;
        //        if (this.documentTypeVisible) this.ddlFilterInboundType.SelectedIndex = 0;
        //        if (this.vendorVisible) this.lstVendor.SelectedIndex = 0;
        //        if (this.carrierVisible) this.lstCarrier.SelectedIndex = 0;
        //        if (this.driverVisible) this.lstDriver.SelectedIndex = 0;
        //    }
        //}

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

            // OwnerUser
            if (ownerUserVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Owner);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterOwnerUser.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterOwnerUser.SelectedIndex = 0;
            }

            // CategoryItem
            if (categoryItemVisible)
            {
                index = Convert.ToInt16(EntityFilterName.CategoryItem);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlCategoryItem.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlCategoryItem.SelectedIndex = 0;
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
            // WorkZone
            if (workZoneVisible)
            {
                index = Convert.ToInt16(EntityFilterName.WorkZone);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    ddlFilterWorkZone.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                else
                    ddlFilterWorkZone.SelectedIndex = 0;

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

            // Code (genérico) 
            if (codeVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Code);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    this.txtFilterCode.Text = mainFilter[index].FilterValues[0].Value;
            }

            // Item
            if (itemVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Item);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    this.txtFilterItem.Text = mainFilter[index].FilterValues[0].Value;
            }

            // Document Nbr
            if (documentVisible)
            {
                index = Convert.ToInt16(EntityFilterName.DocumentNbr);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    this.txtFilterDocumentNumber.Text = mainFilter[index].FilterValues[0].Value;
            }
            // Location
            if (locationVisible)
            {
                index = Convert.ToInt16(EntityFilterName.Location);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                    this.txtFilterLocation.Text = mainFilter[index].FilterValues[0].Value;
            }
            // Date Today (genérico) 
            if (dateVisible)
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                {
                    txtFilterDate.Text = mainFilter[index].FilterValues[0].Value;
                    txtFilterDate.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
                }
            }
            // Dates From and To (genérico) 
            if (dateFromVisible && dateToVisible)
            {
                index = Convert.ToInt16(EntityFilterName.DateRange);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                {
                    txtFilterDateFrom.Text = mainFilter[index].FilterValues[0].Value;
                    txtFilterDateTo.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
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
                    ((BasePage)Page).ConfigureDDLGrpItem3(ddlBscGrpItm3, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, this.lblEmptyRow.Text, true, -1);
                    ddlBscGrpItm3.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                }

                // Group Item 4
                index = Convert.ToInt16(EntityFilterName.GroupItem4);

                if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
                {
                    ((BasePage)Page).ConfigureDDLGrpItem4(ddlBscGrpItm4, true, idBscGrpItem1, idBscGrpItem2, idBscGrpItem3, idBscGrpItem4, this.lblEmptyRow.Text, true, -1);
                    ddlBscGrpItm4.SelectedIndex = mainFilter[index].FilterValues[0].Index;
                }
            }


            #endregion

            #region "FILTRO AVANZADO"

            //if (advancedFilterVisible)
            //{
            //    this.chkUseAdvancedFilter.Checked = context.SessionInfo.FilterUseAdvanced;

            //    // DISPATCHING TAB
            //    if (this.tabDispatchingVisible)
            //    {
            //        // Priority Range
            //        index = Convert.ToInt16(EntityFilterName.PriorityRange);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        {
            //            txtPriorityFrom.Text = mainFilter[index].FilterValues[0].Value;
            //            txtPriorityTo.Text = mainFilter[index].FilterValues[1].Value;
            //        }

            //        // Customer
            //        index = Convert.ToInt16(EntityFilterName.Customer);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //            this.txtCustomer.Text = mainFilter[index].FilterValues[0].Value;

            //        // Carrier
            //        index = Convert.ToInt16(EntityFilterName.Carrier);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //            this.txtCarrier.Text = mainFilter[index].FilterValues[0].Value;

            //        // Route
            //        index = Convert.ToInt16(EntityFilterName.Route);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //            this.txtRoute.Text = mainFilter[index].FilterValues[0].Value;
            //    }

            //    //LAYOUT TAB
            //    if (this.tabLayoutVisible)
            //    {
            //        // Hangar
            //        LoadListValues(EntityFilterName.Hangar, lstHangar);

            //        // WorkZone
            //        LoadListValues(EntityFilterName.WorkZone, lstWorkZone);

            //        // Location Type
            //        LoadListValues(EntityFilterName.LocationType, lstLocationType);
            //    }

            //    //LOCATION TAB
            //    if (this.tabLocationVisible)
            //    {
            //        // Location
            //        index = Convert.ToInt16(EntityFilterName.Location);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //            this.txtLocation.Text = mainFilter[index].FilterValues[0].Value;

            //        // Location Range
            //        index = Convert.ToInt16(EntityFilterName.LocationRange);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        {
            //            this.txtLocationFrom.Text = mainFilter[index].FilterValues[0].Value;
            //            this.txtLocationTo.Text = mainFilter[index].FilterValues[1].Value;
            //        }

            //        // Location Row
            //        index = Convert.ToInt16(EntityFilterName.LocationRow);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //            this.txtLocationRow.Text = mainFilter[index].FilterValues[0].Value;

            //        // Location Column
            //        index = Convert.ToInt16(EntityFilterName.LocationColumn);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //            this.txtLocationColumn.Text = mainFilter[index].FilterValues[0].Value;

            //        // Location Level
            //        index = Convert.ToInt16(EntityFilterName.LocationLevel);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //            this.txtLocationLevel.Text = mainFilter[index].FilterValues[0].Value;

            //        // Location Aisle
            //        index = Convert.ToInt16(EntityFilterName.LocationAisle);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //            this.txtLocationAisle.Text = mainFilter[index].FilterValues[0].Value;
            //    }

            //    //GROUP 1 TAB
            //    if (this.tabGroup1Visible)
            //    {
            //        // Group Item 1
            //        index = Convert.ToInt16(EntityFilterName.GroupItem1);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //            ddlGrpItem1.SelectedIndex = mainFilter[index].FilterValues[0].Index;

            //        // Group Item 2
            //        index = Convert.ToInt16(EntityFilterName.GroupItem2);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        {
            //            ((BasePage)Page).ConfigureDDLGrpItem2(ddlGrpItem2, true, idGrpItem1, idGrpItem2, this.lblEmptyRow.Text, true, -1);
            //            ddlGrpItem2.SelectedIndex = mainFilter[index].FilterValues[0].Index;
            //        }

            //        // Group Item 3
            //        index = Convert.ToInt16(EntityFilterName.GroupItem3);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        {
            //            ((BasePage)Page).ConfigureDDLGrpItem3(ddlGrpItem3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.lblEmptyRow.Text, true, -1);
            //            ddlGrpItem3.SelectedIndex = mainFilter[index].FilterValues[0].Index;
            //        }

            //        // Group Item 4
            //        index = Convert.ToInt16(EntityFilterName.GroupItem4);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        {
            //            ((BasePage)Page).ConfigureDDLGrpItem4(ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.lblEmptyRow.Text, true, -1);
            //            ddlGrpItem4.SelectedIndex = mainFilter[index].FilterValues[0].Index;
            //        }
            //    }

            //    // DATE TAB
            //    if (this.tabDatesVisible)
            //    {
            //        // Fabrication Date
            //        index = Convert.ToInt16(EntityFilterName.FabricationDateRange);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        {
            //            txtFabricationDateFrom.Text = mainFilter[index].FilterValues[0].Value;
            //            txtFabricationDateTo.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
            //        }

            //        // Expiration Date
            //        index = Convert.ToInt16(EntityFilterName.ExpirationDateRange);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        {
            //            txtExpirationDateFrom.Text = mainFilter[index].FilterValues[0].Value;
            //            txtExpirationDateTo.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
            //        }

            //        // Expected Date
            //        index = Convert.ToInt16(EntityFilterName.ExpectedDateRange);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        {
            //            txtExpectedDateFrom.Text = mainFilter[index].FilterValues[0].Value;
            //            txtExpectedDateTo.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
            //        }

            //        // Shipment Date
            //        index = Convert.ToInt16(EntityFilterName.ShipmentDateRange);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        {
            //            txtShipmentDateFrom.Text = mainFilter[index].FilterValues[0].Value;
            //            txtShipmentDateTo.Text = Convert.ToDateTime(mainFilter[index].FilterValues[1].Value).ToShortDateString();
            //        }

            //        // Lot Nbr Range
            //        index = Convert.ToInt16(EntityFilterName.LotNumberRange);

            //        if (mainFilter[index].FilterValues != null && mainFilter[index].FilterValues.Count > 0)
            //        {
            //            txtLotNumberFrom.Text = mainFilter[index].FilterValues[0].Value;
            //            txtLotNumberTo.Text = mainFilter[index].FilterValues[1].Value;
            //        }
            //    }

            //    // DOCUMENT TAB
            //    if (this.tabDocumentVisible)
            //    {
            //        if (divDocumentType.Visible == true)
            //        {
            //            // Inbound Type
            //            LoadListValues(EntityFilterName.DocumentType, lstInboundType);
            //        }
            //        if (divVendor.Visible)
            //        {
            //            // Vendor
            //            LoadVendor(Convert.ToInt16(ddlFilterOwner.SelectedValue));
            //            LoadListValues(EntityFilterName.Vendor, lstVendor);
            //        }
            //        if (divCarrier.Visible)
            //        {
            //            // Carrier
            //            LoadListValues(EntityFilterName.Carrier, lstCarrier);
            //        }
            //        if (divDriver.Visible)
            //        {
            //            // Carrier
            //            LoadListValues(EntityFilterName.Driver, lstDriver);
            //        }
            //    }

            #endregion
            }



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
            //limpia los registros de a grilla y la sesión
            Session.Remove(WMSTekSessions.Shared.ItemList);
            grdSearchItemsControl.DataSource = null;
            grdSearchItemsControl.DataBind();
            InitializePageCountItems();
            mpLookupItemControl.Show();
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
                            txtFilterItem.Text = row.Cells[4].Text;

                            int index = Convert.ToInt16(EntityFilterName.Item);

                            if (mainFilter[index].FilterValues != null)
                            {
                                mainFilter[index].FilterValues.Clear();
                                mainFilter[index].FilterValues.Add(new FilterItem(0, txtFilterItem.Text));
                            }
                        }
                    }
                }
                grdSearchItemsControl.Columns[4].Visible = true;
                grdSearchItemsControl.DataSource = null;
                grdSearchItemsControl.DataBind();
                grdSearchItemsControl.Columns[4].Visible = false;

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
                //InitializeGridItems();
                grdSearchItemsControl.Columns[4].Visible = true;
                grdSearchItemsControl.DataSource = itemSearchViewDTO.Entities;
                grdSearchItemsControl.DataBind();
                grdSearchItemsControl.Columns[4].Visible = false;
                mpLookupItemControl.Show();

                grdSearchItemsControl.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
                grdSearchItemsControl.EmptyDataText = string.Empty; // this.Master.EmptyGridText;

                InitializePageCountItems();
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
        protected void ddlFilterWarehouse_SelectedIndexChanged1(object sender, EventArgs e)
        {
            if (workZoneVisible)
            {
                int idWhs = Convert.ToInt16(ddlFilterWarehouse.SelectedValue);
                LoadWorkZone(idWhs);                
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

       

    }
        #endregion
    }
        #endregion


//Esto se usa en el designer NO BORRAR.
//     public Binaria.WMSTek.WebClient.Shared.WMSTekContent Master
//     {
//         get
//         {
//             return ((Binaria.WMSTek.WebClient.Shared.WMSTekContent)((Binaria.WMSTek.WebClient.Base.BasePage)Page).Master);
//         }
//     }