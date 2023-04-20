using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Entities.Ptl;

namespace Binaria.WMSTek.WebClient.Administration.LogisticsResources
{
    public partial class LocationMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Location> locationViewDTO = new GenericViewDTO<Location>();
        private BaseViewDTO configurationViewDTO = new BaseViewDTO();
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


        private List<string> lstTypeLoc
        {
            get 
            {
                return GetConst("LocationTypeDif").ConvertAll(c => c.ToUpper());
            }
        }

        private List<string> lstTypeLocNOLpn
        {
            get
            {
                return GetConst("LocationTypeNOLpn").ConvertAll(c => c.ToUpper());
            }
        }

        private List<string> lstTypeLocAssignPrinters 
        {
            get
            {
                return GetConst("LocationTypeForAssignPrinters").ConvertAll(c => c.ToUpper());
            }
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
                        // Nota: este mantenedor no carga inicialmente la grilla
                        context.MainFilter = this.Master.ucMainFilter.MainFilter;
                        PopulateLists();

                        this.tabGeneral.HeaderText = this.lbltabGeneral.Text;
                        this.tabDetails.HeaderText = this.lbltabDetails.Text;
                        this.tabWorkZones.HeaderText = this.lbltabWorkZones.Text;
                        this.tabAssignPrinter.HeaderText = this.lbltabAssignPrinters.Text;
                    }
                    else
                    {
                        if (ValidateSession(WMSTekSessions.LocationMgr.LocationList))
                        {
                            locationViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.LocationMgr.LocationList];
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
        //        workZoneViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(workZoneViewDTO.Errors);
        //    }
        //}

        /// <summary>
        /// Persiste los cambios en la entidad (nueva o editada)
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void btnSaveRange_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChangesRange();
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        /// <summary>
        /// Abre la ventana modal para crear una nueva entidad
        /// </summary>
        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowModal(-1, CRUD.Create);
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void btnNewRange_Click(object sender, EventArgs e)
        {
            try
            {
                ShowModalRange();
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null)
                    {
                        btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";
                    }

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;
                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a eliminar
                int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;
                DeleteRow(deleteIndex);
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void chkGenerateCod_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool IsValid = true;
                this.lblError2.Visible = false;
                this.divWarning.Visible = false;

                if ((this.ddlWarehouse.SelectedValue != "-1") && (this.ddlHangar.SelectedValue != "-1") && 
                    (this.txtColumnLoc.Text != "") && (this.txtLevelLoc.Text != "") && (this.txtRowLoc.Text != ""))
                    IsValid = true;
                 else
                    IsValid = false;

                if (chkGenerateCod.Checked && IsValid)//lo autogenera
                    {
                        this.txtLocCode.Text = this.GenerateIdLoc();
                        this.txtLocCode.Enabled = false;

                        this.rfvRowLoc.Enabled = true;
                        this.rfvColumnLoc.Enabled = true;
                        this.rfvLevelLoc.Enabled = true;
                    }
                if (chkGenerateCod.Checked == false)
                    {
                        this.txtLocCode.Text = string.Empty;
                        this.txtLocCode.Enabled = true;

                    }
                if (chkGenerateCod.Checked && (!IsValid))//lanza error
                {
                    this.divWarning.Visible = true;
                    this.lblError2.Visible = true;
                    this.txtLocCode.Text = string.Empty;
                    this.chkGenerateCod.Checked = false;

                    this.rfvRowLoc.Enabled = false;
                    this.rfvColumnLoc.Enabled = false;
                    this.rfvLevelLoc.Enabled = false;
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void chkOnlyLPN_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                validateChkOnlyLPN();

                divEditNew.Visible = true;
                modalPopUp.Show();

            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void chkOnlyLPNRange_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                validateChkOnlyLPNRange();
                
                divNewRange.Visible = true;
                modalRangePopUp.Show();

            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void ddlWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Busca solo los hangares activos
            base.LoadHangarActive(this.ddlHangar, Convert.ToInt16(this.ddlWarehouse.SelectedValue), true, lblDefaultText.Text);

            string locationType = ddlLocTypeCode.SelectedItem.Value.ToUpper().Trim();
            if (locationType == "FKL")
            {
                if (ValidateSession(WMSTekSessions.LocationMgr.LocationList))
                {
                    locationViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.LocationMgr.LocationList];
                    int index = int.Parse(hidEditIndex.Value);

                    if (locationViewDTO.Entities != null && index > 0)
                    {
                        if (locationViewDTO.Entities[index].WorkZones != null)
                        {
                            locationViewDTO.Entities[index].WorkZones = null;
                        }
                    }

                }

                LoadWorkZones(-1);
                LoadPrinters(-1);
            }

            this.modalPopUp.Show();
        }

        protected void ddlWarehouseRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Busca solo los hangares activos
            base.LoadHangarActive(this.ddlHangarRange, Convert.ToInt16(this.ddlWarehouseRange.SelectedValue), true, lblDefaultText.Text);

            this.modalRangePopUp.Show();
        }

        protected void ddlLocTypeCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string locationType = ddlLocTypeCode.SelectedItem.Value.ToUpper().Trim();

            this.txtRowLoc.Enabled = true;
            this.txtColumnLoc.Enabled = true;
            this.txtLevelLoc.Enabled = true;
            this.txtAisle.Enabled = true;

            this.rfvRowLoc.Enabled = false;
            this.rfvColumnLoc.Enabled = false;
            this.rfvLevelLoc.Enabled = false;

            this.rfvAisle.Enabled = true;
            this.rvRowLoc.Enabled = true;
            this.rvColumnLoc.Enabled = true;
            this.rvLevelLoc.Enabled = true;
            this.revAisle.Enabled = true;
            this.chkGenerateCod.Enabled = true;
    
            this.rfvCapacityLPN.Enabled = true;
            
            this.txtCapacityLPN.Enabled = true;
            this.tabWorkZones.Visible = false;
            this.tabAssignPrinter.Visible = false;         
        
            //VALIDA QUE EL TIPO DE UBICACION SEA Truck,Forklift,Stage SEGUN LA LISTA DE UBICACIONES
            //QUE SE ENCUENTRA EN LAS CONSTANTES DE LA APLICACION
            if (lstTypeLoc.Contains(locationType))
            {
                this.txtRowLoc.Enabled = false;
                this.txtColumnLoc.Enabled = false;
                this.txtLevelLoc.Enabled = false;
                this.txtAisle.Enabled = false;

                this.rfvRowLoc.Enabled = false;
                this.rfvColumnLoc.Enabled = false;
                this.rfvLevelLoc.Enabled = false;
                this.rfvAisle.Enabled = false;
                this.chkGenerateCod.Enabled = false;

                this.txtRowLoc.Text = "";
                this.txtColumnLoc.Text = "";
                this.txtLevelLoc.Text = "";
                this.txtAisle.Text = "";
            }


            if (!lstTypeLocNOLpn.Contains(locationType))
            {
                this.chkOnlyLPN.Checked = true;
                this.chkOnlyLPN.Enabled = false;
                this.txtCapacityLPN.Enabled = false;
                this.rfvCapacityLPN.Enabled = false;
            }
            else
            {
                this.chkOnlyLPN.Checked = false;
                this.chkOnlyLPN.Enabled = true;
                this.txtCapacityLPN.Enabled = true;
                this.rfvCapacityLPN.Enabled = true;
            }
            validateChkOnlyLPN();

            if (ValidateSession(WMSTekSessions.LocationMgr.LocationList))
            {
                locationViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.LocationMgr.LocationList];
                int index = int.Parse(hidEditIndex.Value);

                if (locationViewDTO.Entities != null && locationViewDTO.Entities.Count > 0 && index != -1)
                {
                    if (locationViewDTO.Entities[index].WorkZones != null)
                    {
                        locationViewDTO.Entities[index].WorkZones = null;
                    }

                    if (locationViewDTO.Entities[index].Printers != null)
                    {
                        locationViewDTO.Entities[index].Printers = null;
                    }
                }
                    
            }

            if (locationType == "FKL")
            {
                LoadWorkZones(-1);
                this.tabWorkZones.Visible = true;
            }

            //Valida si el tipo de ubicacion requiere impresoras asociadas
            if (lstTypeLocAssignPrinters.Contains(locationType))
            {
                LoadPrinters(-1);
                this.tabAssignPrinter.Visible = true;
            }

            divReasonCodeLoc.Visible = locationType == "STG";
            divPtlType.Visible = (locationType == "PTLIN" || locationType == "PTLOU");

             this.modalPopUp.Show();
        }

        protected void ddlLocTypeCodeRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            string locationTypeRange = ddlLocTypeCodeRange.SelectedItem.Value.ToUpper().Trim();
          
            this.txtRowLocRangeFrom.Enabled = true;
            this.txtRowLocRangeTo.Enabled = true;
            this.txtColumnLocRangeFrom.Enabled = true;
            this.txtColumnLocRangeTo.Enabled = true;
            this.txtLevelLocRangeFrom.Enabled = true;
            this.txtLevelLocRangeTo.Enabled = true;
            this.txtAisleRange.Enabled = true;

            this.rfvRowLocRangeFrom.Enabled = true;
            this.rfvRowLocRangeTo.Enabled = true;
            this.rfvColumnLocRangeFrom.Enabled = true;
            this.rfvColumnLocRangeTo.Enabled = true;
            this.rfvLevelLocRangeFrom.Enabled = true;
            this.rfvLevelLocRangeTo.Enabled = true;
            this.rfvAisleRange.Enabled = true;
            this.rvRowLocRangeFrom.Enabled = true;
            this.rvRowLocRangeTo.Enabled = true;
            this.rvColumnLocRangeFrom.Enabled = true;
            this.rvColumnLocRangeTo.Enabled = true;
            this.rvLevelLocRangeFrom.Enabled = true;
            this.rvLevelLocRangeTo.Enabled = true;
            this.revAisleRange.Enabled = true;
            
            this.rfvCapacityLPNRange.Enabled = true;
            this.chkOnlyLPNRange.Checked = true;
            this.chkOnlyLPNRange.Enabled = true;
            this.txtCapacityLPNRange.Enabled = true;

            //VALIDA QUE EL TIPO DE UBICACION SEA Truck,Forklift,Stage SEGUN LA LISTA DE UBICACIONES
            //QUE SE ENCUENTRA EN LAS CONSTANTES DE LA APLICACION
            if (lstTypeLoc.Contains(locationTypeRange))
            {
                this.txtRowLocRangeFrom.Enabled = false;
                this.txtRowLocRangeTo.Enabled = false;
                this.txtColumnLocRangeFrom.Enabled = false;
                this.txtColumnLocRangeTo.Enabled = false;
                this.txtLevelLocRangeFrom.Enabled = false;
                this.txtLevelLocRangeTo.Enabled = false;
                this.txtAisleRange.Enabled = false;

                this.rfvRowLocRangeFrom.Enabled = false;
                this.rfvRowLocRangeTo.Enabled = false;
                this.rfvColumnLocRangeFrom.Enabled = false;
                this.rfvColumnLocRangeTo.Enabled = false;
                this.rfvLevelLocRangeFrom.Enabled = false;
                this.rfvLevelLocRangeTo.Enabled = false;
                this.rfvAisleRange.Enabled = false;

                this.txtRowLocRangeFrom.Text = string.Empty;
                this.txtRowLocRangeTo.Text = string.Empty;
                this.txtColumnLocRangeFrom.Text = string.Empty;
                this.txtColumnLocRangeTo.Text = string.Empty;
                this.txtLevelLocRangeFrom.Text = string.Empty;
                this.txtLevelLocRangeTo.Text = string.Empty;
                this.txtAisleRange.Text = string.Empty;
            }

            if (!lstTypeLocNOLpn.Contains(locationTypeRange))
            {
                this.chkOnlyLPNRange.Checked = true;
                this.chkOnlyLPNRange.Enabled = false;
                this.txtCapacityLPNRange.Enabled = false;
                this.rfvCapacityLPNRange.Enabled = false;
            }
            else
            {
                this.chkOnlyLPNRange.Checked = false;
                this.chkOnlyLPNRange.Enabled = false;
            }

            validateChkOnlyLPNRange();

            this.modalRangePopUp.Show();
        }

        protected void grdWorkZones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdPrinters_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Métodos"

        public void Initialize()
        {
            context.SessionInfo.IdPage = "LocationMgr";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeLayout();
            InitializeTypeUnitOfMeasure_OfMass();
        }

        /// <summary>
        /// Carga en sesion lista de Ubicaciones
        /// </summary>
        /// <param name="showError"></param>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(locationViewDTO.Errors);
                locationViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            locationViewDTO = iLayoutMGR.FindAllLocation(context);

            if (!locationViewDTO.hasError() && locationViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.LocationMgr.LocationList, locationViewDTO);
                Session.Remove(WMSTekSessions.Shared.LocationList);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(locationViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
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
            //this.Master.ucMainFilter.ownerVisible = true;
            //this.Master.ucMainFilter.ownerIncludeNulls = true;
            this.Master.ucMainFilter.locationTypeVisible = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.hangarVisible = true;
            this.Master.ucMainFilter.SaveOnIndexChanged = true;
           
            // Configura textos a mostar
            this.Master.ucMainFilter.codeLabel = lblFilterCode.Text;
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;

            this.Master.ucMainFilter.chkFilterLockedLocationVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);
            this.Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewRange_Click);
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);


            this.Master.ucTaskBar.btnNewVisible = true;
            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnAddVisible = true;
            //Master.ucTaskBar.btnAddToolTip = this.lblAddLoadToolTip.Text;
            Master.ucTaskBar.btnExcelVisible = true;
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
                //    LoadOutboundOrderDetail(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                //base.ExportToExcel(grdMgr, grdDetail, detailTitle);

                base.ExportToExcel(grdMgr, null, null, true);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
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
            locationViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(locationViewDTO.MessageStatus.Message);
        }

        /// <summary>
        /// Configuracion inicial de la Grilla principal
        /// </summary>
        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        /// <summary>
        /// Configuracion inicial del layout
        /// </summary>
        private void InitializeLayout()
        {
            if (locationViewDTO.Configuration == null)
            {
                configurationViewDTO = iConfigurationMGR.GetLayoutConfiguration("Location_FindAll", context);
                if (!configurationViewDTO.hasConfigurationError()) locationViewDTO.Configuration = configurationViewDTO.Configuration;
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.EmptyDataText = this.Master.EmptyGridText;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!locationViewDTO.hasConfigurationError() && locationViewDTO.Configuration != null && locationViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, locationViewDTO.Configuration);

            grdMgr.DataSource = locationViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridView();

            ucStatus.ShowRecordInfo(locationViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateLists()
        {
            //Carga lista de warehouses
            base.LoadUserWarehouses(this.ddlWarehouse, this.Master.EmptyRowText, "-1", true);

            //Carga ilsta de Hangares
            //base.LoadHangars(this.ddlHangar, isNew, this.Master.EmptyRowText);

            //Carga lista de LocTypeCode
            base.LoadLocationType(this.ddlLocTypeCode, false, this.Master.EmptyRowText);

            //Carga lista de Owner
            //base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);

            //Carga lista de warehouses
            base.LoadUserWarehouses(this.ddlWarehouseRange, this.Master.EmptyRowText, "-1", true);

            //Carga ilsta de Hangares
            //base.LoadHangars(this.ddlHangar, isNew, this.Master.EmptyRowText);

            //Carga lista de LocTypeCode
            //base.LoadLocationType(this.ddlLocTypeCodeRange, false, this.Master.EmptyRowText);
            LoadLocationTypeFilterRange();

            //Carga lista de Owner
            //base.LoadUserOwners(this.ddlOwnerRange, this.Master.EmptyRowText, "-1", true, string.Empty, false);

            //Carga Lista de WmsProcess
            //base.LoadWmsProcess(this.ddlWmsProcess, false, this.Master.EmptyRowText);
            base.LoadWmsProcess(this.ddlWmsProcess, 2, false, this.Master.EmptyRowText);

            base.LoadReasonLessFilter(ddlReasonCodeLoc, true, this.Master.EmptyRowText);
        }

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                divGrid.Visible = true;
                this.Master.ucError.ClearError();
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            this.lblError.Visible = false;
            this.divWarning.Visible = false;
            this.divOwner.Visible = false;
            this.ddlOwner.Visible = false;
            this.lblOwner.Visible = false;
            tabLocation.ActiveTabIndex = 0;
            string locationType = string.Empty;

            //Carga lista con los tipos de Ptl
            this.LoadPtlTypes(this.ddlPtlType,true, this.Master.EmptyRowText);
            // Editar Ubicacion
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditIndex.Value = index.ToString();
                hidEditId.Value = locationViewDTO.Entities[index].IdCode;
                locationType = locationViewDTO.Entities[index].Type.LocTypeCode.Trim();
                                
                //Carga controles
                base.LoadReasonLessFilter(this.ddlHoldCode, true, lblDefaultText.Text);
                this.ddlHoldCode.SelectedValue = locationViewDTO.Entities[index].Reason.Code;
                this.txtLocCode.Text = locationViewDTO.Entities[index].Code;
                this.ddlWarehouse.SelectedValue = locationViewDTO.Entities[index].Warehouse.Id.ToString();
                base.LoadHangar(this.ddlHangar, Convert.ToInt16(this.ddlWarehouse.SelectedValue), false, lblDefaultText.Text);
                this.ddlHangar.SelectedValue = locationViewDTO.Entities[index].Hangar.Id.ToString();
                this.txtRowLoc.Text = locationViewDTO.Entities[index].Row.ToString();
                this.txtColumnLoc.Text = locationViewDTO.Entities[index].Column.ToString();
                this.txtLevelLoc.Text = locationViewDTO.Entities[index].Level.ToString();
                this.txtAisle.Text = locationViewDTO.Entities[index].Aisle;
                this.chkStatus.Checked = locationViewDTO.Entities[index].Status;
                this.ddlLocTypeCode.SelectedValue = locationViewDTO.Entities[index].Type.LocTypeCode;
                this.ddlPtlType.SelectedValue = locationViewDTO.Entities[index].PtlType.PtlTypeCode;
                this.txtDescription.Text = locationViewDTO.Entities[index].Description;
                this.chkSharedItem.Checked = locationViewDTO.Entities[index].SharedItem;
                this.chkOnlyLPN.Checked = locationViewDTO.Entities[index].OnlyLPN;
                this.chkLockInventory.Checked = locationViewDTO.Entities[index].LockInventory;
                //this.ddlOwner.SelectedValue = locationViewDTO.Entities[index].Owner.Id.ToString();
                //this.chkDedicatedOwner.Checked = locationViewDTO.Entities[index].DedicatedOwner;
                this.txtPickingFlow.Text = locationViewDTO.Entities[index].PickingFlow.ToString();
                this.txtPutawayFlow.Text = locationViewDTO.Entities[index].PutawayFlow.ToString();
                this.txtCapacityLPN.Text = locationViewDTO.Entities[index].CapacityLPN.ToString();
                this.txtCapacityUnit.Text = locationViewDTO.Entities[index].CapacityUnit.ToString();
                this.txtLength.Text = locationViewDTO.Entities[index].Length.ToString();
                this.txtWidth.Text = locationViewDTO.Entities[index].Width.ToString();
                this.txtHeight.Text = locationViewDTO.Entities[index].Height.ToString();
                this.txtVolume.Text = locationViewDTO.Entities[index].Volume.ToString();
                this.txtWeight.Text = locationViewDTO.Entities[index].Weight.ToString();
                ddlReasonCodeLoc.SelectedValue = string.IsNullOrEmpty(locationViewDTO.Entities[index].ReasonCodeLoc) ? "-1" : locationViewDTO.Entities[index].ReasonCodeLoc;
                divPtlType.Visible = locationViewDTO.Entities[index].Type.LocTypeCode == "PTLIN" || locationViewDTO.Entities[index].Type.LocTypeCode == "PTLOU" ? true : false;

                if (locationViewDTO.Entities[index].PositionX != -1)
                    this.txtPositionX.Text = locationViewDTO.Entities[index].PositionX.ToString();
                else
                    this.txtPositionX.Text = string.Empty;

                if (locationViewDTO.Entities[index].PositionY != -1)
                    this.txtPositionY.Text = locationViewDTO.Entities[index].PositionY.ToString();
                else
                    this.txtPositionY.Text = string.Empty;

                if (locationViewDTO.Entities[index].PositionZ != -1)
                    this.txtPositionZ.Text = locationViewDTO.Entities[index].PositionZ.ToString();
                else
                    this.txtPositionZ.Text = string.Empty;


                if (locationViewDTO.Entities[index].Type.LocTypeCode.Trim() == "FKL")
                    this.tabWorkZones.Visible = true;

                if (locationViewDTO.Entities[index].OnlyLPN)
                {
                    this.rvCapacityLPN.MinimumValue = "0";
                }
                else
                {
                    this.rvCapacityLPN.MinimumValue = "1";
                }

                if (lstTypeLocAssignPrinters.Contains(locationViewDTO.Entities[index].Type.LocTypeCode.Trim()))
                    this.tabAssignPrinter.Visible = true;
                else
                    this.tabAssignPrinter.Visible = false;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva Ubicacion
            if (mode == CRUD.Create)
            {
                // Selecciona Warehouse y Owner seleccionados en el Filtro
                if(context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Count > 0)
                    this.ddlWarehouse.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues[0].Value;

                //if (context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Count > 0)
                //{
                //    // La opción 'Sin dueño' de la lista de Owners del filtro principal se considera aparte, 
                //    //  ya que no aparece en la lista de Owners de este mantenedor.
                //    if (context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value == "-2")
                //        this.ddlOwner.SelectedValue = "-1";
                //    else
                //        this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;
                //}

                base.LoadReasonLessFilter(ddlHoldCode, true, lblDefaultText.Text);
                                
                //Busca solo los hangares activos
                base.LoadHangarActive(this.ddlHangar, Convert.ToInt16(this.ddlWarehouse.SelectedValue), true, lblDefaultText.Text);

                // Selecciona Hangar seleccionado en el Filtro
                if (context.MainFilter[Convert.ToInt16(EntityFilterName.Hangar)].FilterValues.Count > 0)
                    this.ddlHangar.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Hangar)].FilterValues[0].Value;

                hidEditId.Value = "0";
                lblNew.Visible = true;
                lblEdit.Visible = false;
                //habilita controles
                this.ddlHoldCode.Enabled = true;
                this.ddlHangar.Enabled = true;
                this.txtRowLoc.Enabled = true;
                this.txtColumnLoc.Enabled = true;
                this.txtLevelLoc.Enabled = true;
                this.chkGenerateCod.Enabled = true;

                this.txtLocCode.Text = string.Empty;
                //this.ddlHangar.SelectedValue = "-1";
                this.txtRowLoc.Text = string.Empty;
                this.txtColumnLoc.Text = string.Empty;
                this.txtLevelLoc.Text = string.Empty;
                this.txtAisle.Text = string.Empty;
                this.chkStatus.Checked = true;
                this.ddlLocTypeCode.SelectedValue = "-1";
                this.ddlPtlType.SelectedValue = "-1";
                this.txtDescription.Text = string.Empty;
                this.chkSharedItem.Checked = false;
                this.chkOnlyLPN.Checked = false;
                this.chkLockInventory.Checked = false;
                //this.chkDedicatedOwner.Checked = false;
                this.txtPickingFlow.Text = string.Empty;
                this.txtPutawayFlow.Text = string.Empty;
                this.txtCapacityLPN.Text = string.Empty;
                this.txtCapacityUnit.Text = string.Empty;
                this.txtLength.Text = string.Empty;
                this.txtWidth.Text = string.Empty;
                this.txtHeight.Text = string.Empty;
                this.txtVolume.Text = string.Empty;
                this.txtWeight.Text = string.Empty;
                this.txtPositionX.Text = string.Empty;
                this.txtPositionY.Text = string.Empty;
                this.txtPositionZ.Text = string.Empty;
                ddlReasonCodeLoc.SelectedValue = "-1";
                locationType = ddlLocTypeCode.SelectedItem.Value.ToUpper().Trim();

                this.txtCapacityLPN.Enabled = false;
                this.txtCapacityLPN.Text = "0";
                this.rfvCapacityLPN.Enabled = false;
                this.rvCapacityLPN.MinimumValue = "0";
                this.tabWorkZones.Visible = false;
                this.tabAssignPrinter.Visible = false;
            }

            if (locationViewDTO.Configuration != null && locationViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                {
                    base.ConfigureModal(locationViewDTO.Configuration, true);
                    divReasonCodeLoc.Visible = false;
                    divPtlType.Visible = false;
                }
                else
                {
                    base.ConfigureModal(locationViewDTO.Configuration, false);
                    this.ddlWarehouse.Enabled = false;
                    this.ddlHangar.Enabled = false;
                    this.txtRowLoc.Enabled = false;
                    this.txtColumnLoc.Enabled = false;
                    this.txtLevelLoc.Enabled = false;
                    divReasonCodeLoc.Visible = locationViewDTO.Entities[index].Type.LocTypeCode == "STG" ? true : false;
                }
            }

            //VALIDA QUE EL TIPO DE UBICACION SEA Truck,Forklift,Stage SEGUN LA LISTA DE UBICACIONES
            //QUE SE ENCUENTRA EN LAS CONSTANTES DE LA APLICACION
            if (lstTypeLoc.Contains(locationType))
            {
                this.rfvRowLoc.Enabled = false;
                this.rfvColumnLoc.Enabled = false;
                this.rfvLevelLoc.Enabled = false;
                this.rfvAisle.Enabled = false;

                this.rvRowLoc.Enabled = false;
                this.rvColumnLoc.Enabled = false;
                this.rvLevelLoc.Enabled = false;
                this.revAisle.Enabled = false;
                this.chkGenerateCod.Enabled = false;
                this.txtAisle.Enabled = false;

            }
            else
            {
                this.rfvRowLoc.Enabled = true;
                this.rfvColumnLoc.Enabled = true;
                this.rfvLevelLoc.Enabled = true;
                this.rfvAisle.Enabled = true;

                this.rvRowLoc.Enabled = this.chkGenerateCod.Checked; ;
                this.rvColumnLoc.Enabled = this.chkGenerateCod.Checked; ;
                this.rvLevelLoc.Enabled = this.chkGenerateCod.Checked; ;
                this.revAisle.Enabled = true;
                this.chkGenerateCod.Enabled = true;
            }

            if (mode == CRUD.Update)
            {
                this.ddlLocTypeCode.Enabled = false;
                this.chkGenerateCod.Enabled = false;
                this.txtLocCode.Enabled = false;
                this.chkOnlyLPN.Enabled = true;
            }

            //Carga la lista de zonas
            LoadWorkZones(index);

            //Carga la lista de Impresoras
            LoadPrinters(index);

            chkGenerateCod.Checked = false;
            validateChkOnlyLPN();

            divEditNew.Visible = true;
            modalPopUp.Show();
         }

        protected void ShowModalRange()
        {
            //tabLocationRange.ActiveTabIndex = 0;
            this.divOwnerRange.Visible = false;
            this.ddlOwnerRange.Visible = false;
            this.lblOwnerRange.Visible = false;

            string locationType = string.Empty;

            // Selecciona Warehouse y Owner seleccionados en el Filtro
            if (context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Count > 0)
                this.ddlWarehouseRange.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues[0].Value;

            //if (context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Count > 0)
            //{
            //    // La opción 'Sin dueño' de la lista de Owners del filtro principal se considera aparte, 
            //    //  ya que no aparece en la lista de Owners de este mantenedor.
            //    if (context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value == "-2")
            //        this.ddlOwnerRange.SelectedValue = "-1";
            //    else
            //        this.ddlOwnerRange.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;
            //}

            //Busca solo los hangares activos
            base.LoadHangarActive(this.ddlHangarRange, Convert.ToInt16(this.ddlWarehouseRange.SelectedValue), true, lblDefaultText.Text);

            // Selecciona Hangar seleccionado en el Filtro
            if (context.MainFilter[Convert.ToInt16(EntityFilterName.Hangar)].FilterValues.Count > 0)
                this.ddlHangarRange.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Hangar)].FilterValues[0].Value;

            this.ddlWarehouseRange.Enabled = true;
            //habilita controles
            this.ddlHangarRange.Enabled = true;
            this.txtRowLocRangeFrom.Enabled = true;
            this.txtRowLocRangeTo.Enabled = true;
            this.txtColumnLocRangeFrom.Enabled = true;
            this.txtColumnLocRangeTo.Enabled = true;
            this.txtLevelLocRangeFrom.Enabled = true;
            this.txtLevelLocRangeTo.Enabled = true;

            this.txtRowLocRangeFrom.Text = string.Empty;
            this.txtRowLocRangeTo.Text = string.Empty;
            this.txtColumnLocRangeFrom.Text = string.Empty;
            this.txtColumnLocRangeTo.Text = string.Empty;
            this.txtLevelLocRangeFrom.Text = string.Empty;
            this.txtLevelLocRangeTo.Text = string.Empty;
            this.txtAisleRange.Text = string.Empty;
            this.chkStatusRange.Checked = true;
            this.ddlLocTypeCodeRange.SelectedValue = "-1";
            this.txtDescriptionRange.Text = string.Empty;
            this.chkSharedItemRange.Checked = false;
            this.chkOnlyLPNRange.Checked = false;
            //this.chkDedicatedOwnerRange.Checked = false;
            this.txtPickingFlowRange.Text = string.Empty;
            this.txtPutawayFlowRange.Text = string.Empty;
            this.txtCapacityLPNRange.Text = string.Empty;
            this.txtCapacityUnitRange.Text = string.Empty;
            this.txtLengthRange.Text = string.Empty;
            this.txtWidthRange.Text = string.Empty;
            this.txtHeightRange.Text = string.Empty;
            this.txtVolumeRange.Text = string.Empty;
            this.txtWeightRange.Text = string.Empty;
            locationType = ddlLocTypeCodeRange.SelectedItem.Value.ToUpper().Trim();

            this.txtCapacityLPNRange.Enabled = false;
            this.txtCapacityLPNRange.Text = "0";
            this.rfvCapacityLPNRange.Enabled = false;
            this.rvCapacityLPNRange.MinimumValue = "0";

            validateChkOnlyLPNRange();

            divNewRange.Visible = true;
            modalRangePopUp.Show();
 
        }
        
        /// <summary>
        /// Persiste los cambios en la entidad (modo Edit o New). 
        /// </summary>
        protected void SaveChanges()
        {
            List<string> ListValidate = new List<string>();

            //ojo!!! si el control de texto esta vacio es por que el campo acepta null, 
            //por lo tanto en la validacion el vacio pasa como si fuera un número válido.

            ListValidate.Add(txtRowLoc.Text);
            ListValidate.Add(txtColumnLoc.Text);
            ListValidate.Add(txtLevelLoc.Text);
            ListValidate.Add(txtPickingFlow.Text);
            ListValidate.Add(txtPutawayFlow.Text);
            ListValidate.Add(txtCapacityLPN.Text);
            ListValidate.Add(txtCapacityUnit.Text);
            ListValidate.Add(txtLength.Text);
            ListValidate.Add(txtWeight.Text);
            ListValidate.Add(txtVolume.Text);
            ListValidate.Add(txtLength.Text);
            ListValidate.Add(txtWidth.Text);
            ListValidate.Add(txtHeight.Text);
            ListValidate.Add(txtPositionX.Text);
            ListValidate.Add(txtPositionY.Text);
            ListValidate.Add(txtPositionZ.Text);

                //si todos los datos que se ingresaron son numericos, guarda.
            if (ValidateNumber(ListValidate))
            {
                Location location = new Location();
                location.Warehouse = new Warehouse();
                location.Hangar = new Hangar();
                location.Type = new LocationType();
                location.Owner = new Owner();
                location.Reason = new Reason();

                location.Warehouse.Id = Convert.ToInt16(this.ddlWarehouse.SelectedValue);

                if (this.txtLocCode.Text.Trim() != String.Empty)
                    location.Code = this.txtLocCode.Text.Trim();
                else
                    location.Code = this.GenerateIdLoc();
                if (this.ddlHoldCode.SelectedIndex != 0)
                    location.Reason.Code = this.ddlHoldCode.SelectedValue;
                else
                    location.Reason.Code = null;
                
                location.Hangar.Id = Convert.ToInt16(this.ddlHangar.SelectedValue);
                if (this.txtRowLoc.Text.Trim() != string.Empty) location.Row = Convert.ToInt32(this.txtRowLoc.Text);
                if (this.txtColumnLoc.Text.Trim() != string.Empty) location.Column = Convert.ToInt32(this.txtColumnLoc.Text);
                if (this.txtLevelLoc.Text.Trim() != string.Empty) location.Level = Convert.ToInt32(this.txtLevelLoc.Text);
                if (this.txtAisle.Text.Trim() != string.Empty) location.Aisle = this.txtAisle.Text.Trim();
                location.Status = this.chkStatus.Checked;
                location.Type.LocTypeCode = this.ddlLocTypeCode.SelectedValue;
                location.PtlType.PtlTypeCode = this.ddlPtlType.SelectedValue;
                if (this.txtDescription.Text.Trim() != string.Empty) location.Description = this.txtDescription.Text.Trim();
                location.SharedItem = this.chkSharedItem.Checked;
                location.OnlyLPN = this.chkOnlyLPN.Checked;
                location.LockInventory = this.chkLockInventory.Checked;
                //location.Owner.Id = Convert.ToInt32(this.ddlOwner.SelectedValue);
                //location.DedicatedOwner = this.chkDedicatedOwner.Checked;
                if (this.txtPickingFlow.Text.Trim() != string.Empty) location.PickingFlow = Convert.ToInt32(this.txtPickingFlow.Text);
                if (this.txtPutawayFlow.Text.Trim() != string.Empty) location.PutawayFlow = Convert.ToInt32(this.txtPutawayFlow.Text);
                if (this.txtCapacityLPN.Text.Trim() != string.Empty) location.CapacityLPN = Convert.ToInt32(this.txtCapacityLPN.Text);
                if (this.txtCapacityUnit.Text.Trim() != string.Empty) location.CapacityUnit = Convert.ToDecimal(this.txtCapacityUnit.Text);
                if (this.txtLength.Text.Trim() != string.Empty) location.Length = Convert.ToDecimal(this.txtLength.Text);
                if (this.txtWidth.Text.Trim() != string.Empty) location.Width = Convert.ToDecimal(this.txtWidth.Text);
                if (this.txtHeight.Text.Trim() != string.Empty) location.Height = Convert.ToDecimal(this.txtHeight.Text);
                if (this.txtVolume.Text.Trim() != string.Empty) location.Volume = Convert.ToDecimal(this.txtVolume.Text);
                if (this.txtWeight.Text.Trim() != string.Empty) location.Weight = Convert.ToDecimal(this.txtWeight.Text);
                if (this.txtPositionX.Text.Trim() != string.Empty) location.PositionX = Convert.ToInt32(this.txtPositionX.Text);
                if (this.txtPositionY.Text.Trim() != string.Empty) location.PositionY = Convert.ToInt32(this.txtPositionY.Text);
                if (this.txtPositionZ.Text.Trim() != string.Empty) location.PositionZ = Convert.ToInt32(this.txtPositionZ.Text);
                if (ddlReasonCodeLoc.SelectedValue != "-1") location.ReasonCodeLoc = ddlReasonCodeLoc.SelectedValue;

                string locationType = ddlLocTypeCode.SelectedItem.Value.ToUpper().Trim();                   
        
                //VALIDA QUE EL TIPO DE UBICACION SEA Truck,Forklift,Stage SEGUN LA LISTA DE UBICACIONES
                //QUE SE ENCUENTRA EN LAS CONSTANTES DE LA APLICACION
                if (lstTypeLoc.Contains(locationType))
                {
                    location.Aisle = "0";
                }

                //Valida que la Ubicacion se de tipo Maquina
                if (locationType == "FKL")
                {
                    // Zonas
                    int index = Convert.ToInt32(hidEditIndex.Value);

                    //Zonas
                    if (index != -1 && locationViewDTO.Entities[index] != null
                        && locationViewDTO.Entities[index].WorkZones != null
                        && locationViewDTO.Entities[index].WorkZones.Count > 0)
                    {
                        location.WorkZones = locationViewDTO.Entities[index].WorkZones;
                    }
                }


                //Valida que la Ubicacion Requiera Impresoras
                if (lstTypeLocAssignPrinters.Contains(locationType))
                {
                    int index = Convert.ToInt32(hidEditIndex.Value);

                    //Impresoras
                    if (index != -1 && locationViewDTO.Entities[index] != null
                        && locationViewDTO.Entities[index].Printers != null
                        && locationViewDTO.Entities[index].Printers.Count > 0)
                    {
                        location.Printers = locationViewDTO.Entities[index].Printers;
                    }
                }
                else
                {
                    location.Printers = new List<Printer>();
                }


                //Nueva Ubicacion
                if (hidEditId.Value == "0")
                {
                    location.IdCode = GenerateIdLoc();
                    locationViewDTO = iLayoutMGR.MaintainLocation(CRUD.Create, location, context);
                }
                //Editar Ubicacion
                else
                {
                    location.IdCode = hidEditId.Value.ToString().Trim();
                    locationViewDTO = iLayoutMGR.MaintainLocation(CRUD.Update, location, context);
                }

                

                if (locationViewDTO.hasError())
                {
                    UpdateSession(true);
                    divEditNew.Visible = true;
                    divWarning.Visible = false;
                    lblError2.Visible = false;
                    modalPopUp.Show();
                }
                else
                {
                    crud = true;
                    ucStatus.ShowMessage(locationViewDTO.MessageStatus.Message);
                    UpdateSession(false);
                    divEditNew.Visible = false;
                    modalPopUp.Hide();
                }
            }
            else //hay datos que no son números
            {
                divWarning.Visible = true;
                this.lblError.Visible = true;
                divEditNew.Visible = true;
                modalPopUp.Show();
                }
        }

        protected void SaveChangesRange()
        {
            List<string> ListValidateRange = new List<string>();
            GenericViewDTO<Location> locationByRange = new GenericViewDTO<Location>();

            int putawayFlowStart = 0;
            int pickingFlowStart = 0;
            int rowStart = 0;
            int rowEnd = 0;
            int columnStart = 0;
            int columnEnd = 0;
            int levelStart = 0;
            int levelEnd = 0;

            //ojo!!! si el control de texto esta vacio es por que el campo acepta null, 
            //por lo tanto en la validacion el vacio pasa como si fuera un número válido.

            ListValidateRange.Add(txtRowLocRangeFrom.Text);
            ListValidateRange.Add(txtRowLocRangeTo.Text);
            ListValidateRange.Add(txtColumnLocRangeFrom.Text);
            ListValidateRange.Add(txtColumnLocRangeTo.Text);
            ListValidateRange.Add(txtLevelLocRangeFrom.Text);
            ListValidateRange.Add(txtLevelLocRangeTo.Text);
            ListValidateRange.Add(txtPickingFlowRange.Text);
            ListValidateRange.Add(txtPutawayFlowRange.Text);
            ListValidateRange.Add(txtCapacityLPNRange.Text);
            ListValidateRange.Add(txtCapacityUnitRange.Text);
            ListValidateRange.Add(txtLengthRange.Text);
            ListValidateRange.Add(txtWeightRange.Text);
            ListValidateRange.Add(txtVolumeRange.Text);
            ListValidateRange.Add(txtLengthRange.Text);
            ListValidateRange.Add(txtWidthRange.Text);
            ListValidateRange.Add(txtHeightRange.Text);

            //si todos los datos que se ingresaron son numericos, guarda.
            if (ValidateNumber(ListValidateRange))
            {
                Location location = new Location();
                location.Warehouse = new Warehouse();
                location.Hangar = new Hangar();
                location.Type = new LocationType();
                location.Owner = new Owner();
                location.Reason = new Reason();

                location.Warehouse.Id = Convert.ToInt16(this.ddlWarehouseRange.SelectedValue);
                location.Hangar.Id = Convert.ToInt16(this.ddlHangarRange.SelectedValue);
                if (this.txtAisleRange.Text.Trim() != string.Empty) location.Aisle = this.txtAisleRange.Text.Trim();
                location.Status = this.chkStatusRange.Checked;
                location.Type.LocTypeCode = this.ddlLocTypeCodeRange.SelectedValue;
                if (this.txtDescriptionRange.Text.Trim() != string.Empty) location.Description = this.txtDescriptionRange.Text.Trim();
                location.SharedItem = this.chkSharedItemRange.Checked;
                location.OnlyLPN = this.chkOnlyLPNRange.Checked;
                //location.Owner.Id = Convert.ToInt32(this.ddlOwnerRange.SelectedValue);
                //location.DedicatedOwner = this.chkDedicatedOwnerRange.Checked;

                if (this.txtRowLocRangeFrom.Text.Trim() != string.Empty) rowStart = Convert.ToInt32(this.txtRowLocRangeFrom.Text);
                if (this.txtRowLocRangeTo.Text.Trim() != string.Empty) rowEnd = Convert.ToInt32(this.txtRowLocRangeTo.Text);

                if (this.txtColumnLocRangeFrom.Text.Trim() != string.Empty) columnStart = Convert.ToInt32(this.txtColumnLocRangeFrom.Text);
                if (this.txtColumnLocRangeTo.Text.Trim() != string.Empty) columnEnd = Convert.ToInt32(this.txtColumnLocRangeTo.Text);

                if (this.txtLevelLocRangeFrom.Text.Trim() != string.Empty) levelStart = Convert.ToInt32(this.txtLevelLocRangeFrom.Text);
                if (this.txtLevelLocRangeTo.Text.Trim() != string.Empty) levelEnd = Convert.ToInt32(this.txtLevelLocRangeTo.Text);

                if (this.txtPickingFlowRange.Text.Trim() != string.Empty) pickingFlowStart = Convert.ToInt32(this.txtPickingFlowRange.Text);
                if (this.txtPutawayFlowRange.Text.Trim() != string.Empty) putawayFlowStart = Convert.ToInt32(this.txtPutawayFlowRange.Text);
                
                if (this.txtCapacityLPNRange.Text.Trim() != string.Empty) location.CapacityLPN = Convert.ToInt32(this.txtCapacityLPNRange.Text);
                if (this.txtCapacityUnitRange.Text.Trim() != string.Empty) location.CapacityUnit = Convert.ToDecimal(this.txtCapacityUnitRange.Text);
                if (this.txtLengthRange.Text.Trim() != string.Empty) location.Length = Convert.ToDecimal(this.txtLengthRange.Text);
                if (this.txtWidthRange.Text.Trim() != string.Empty) location.Width = Convert.ToDecimal(this.txtWidthRange.Text);
                if (this.txtHeightRange.Text.Trim() != string.Empty) location.Height = Convert.ToDecimal(this.txtHeightRange.Text);
                if (this.txtVolumeRange.Text.Trim() != string.Empty) location.Volume = Convert.ToDecimal(this.txtVolumeRange.Text);
                if (this.txtWeightRange.Text.Trim() != string.Empty) location.Weight = Convert.ToDecimal(this.txtWeightRange.Text);


                string locationType = ddlLocTypeCodeRange.SelectedItem.Value.ToUpper().Trim();

                //VALIDA QUE EL TIPO DE UBICACION SEA Truck,Forklift,Stage SEGUN LA LISTA DE UBICACIONES
                //QUE SE ENCUENTRA EN LAS CONSTANTES DE LA APLICACION
                if (lstTypeLoc.Contains(locationType))
                {
                    location.Aisle = "0";
                }

                for(int rows = rowStart; rows <= rowEnd; rows++)
                {
                    for (int columns = columnStart; columns <= columnEnd; columns++)
                    {
                        for (int levels = levelStart; levels <= levelEnd; levels++)
                        {
                            location.Row = rows;
                            location.Column = columns;
                            location.Level = levels;
                            location.PickingFlow = pickingFlowStart;
                            location.PutawayFlow = putawayFlowStart;

                            if (pickingFlowStart > 0)
                                pickingFlowStart++;

                            if (putawayFlowStart > 0)
                                putawayFlowStart++;

                            location.IdCode = location.Warehouse.Id.ToString("00") + location.Hangar.Id.ToString("00") + rows.ToString("000") + columns.ToString("00") + levels.ToString("00");
                            location.Code = location.IdCode;
                            locationByRange.Entities.Add(new Location(location));

                            //locationViewDTO = iLayoutMGR.MaintainLocation(CRUD.Create, location, context);

                        }
                    }
                }

                locationViewDTO = iLayoutMGR.MaintainLocationMassive(CRUD.Create, locationByRange, context);

                if (locationViewDTO.hasError())
                {
                    UpdateSession(true);
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                }
                else
                {
                    crud = true;
                    ucStatus.ShowMessage(locationViewDTO.MessageStatus.Message);
                    UpdateSession(false);
                }

                divEditNew.Visible = false;
                modalPopUp.Hide();

                

            }
            else //hay datos que no son números
            {
                divWarning.Visible = true;
                this.lblError.Visible = true;
                divNewRange.Visible = true;
                modalRangePopUp.Show();
            }

        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            locationViewDTO = iLayoutMGR.MaintainLocation(CRUD.Delete, locationViewDTO.Entities[index], context);

            if (locationViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Mustra mensaje de barra de status
                crud = true;
                ucStatus.ShowMessage(locationViewDTO.MessageStatus.Message);
                //Actuualiza grilla
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Funcion que retorna un codigo generado
        /// por el warehouse, el hangar, row, column, level.
        /// 2 caracteres de cada uno
        /// </summary>
        /// <returns>string </returns>
        private string GenerateIdLoc()
        {
            string warehouse;
            string hangar;
            string row;
            string column;
            string level;
            string codGenerado = string.Empty;
            List<String> ListVar = null;
            ListVar = new List<string>();
            ListVar.Add(txtRowLoc.Text);
            ListVar.Add(txtLevelLoc.Text);
            ListVar.Add(txtColumnLoc.Text);
            string locationType = ddlLocTypeCode.SelectedItem.Value.ToUpper().Trim();
           
        
            //VALIDA QUE EL TIPO DE UBICACION SEA Truck,Forklift,Stage SEGUN LA LISTA DE UBICACIONES
            //QUE SE ENCUENTRA EN LAS CONSTANTES DE LA APLICACION
            if (lstTypeLoc.Contains(locationType) || !this.chkGenerateCod.Checked)
            {
                codGenerado = this.txtLocCode.Text.Trim();
                this.divWarning.Visible = true;
                this.lblError2.Visible = true;
            }
            else
            {
                if (base.ValidateNumber(ListVar) && txtRowLoc.Text.Trim().Length > 0 && ddlHangar.SelectedIndex != 0 &&
                    txtColumnLoc.Text.Trim().Length > 0 && txtLevelLoc.Text.Trim().Length > 0 && ddlWarehouse.SelectedIndex != 0)
                {
                    System.Text.StringBuilder codeGenerate = new System.Text.StringBuilder();

                    //OBTIENE LOS PRIMEROS CARACTERES PARA FORMAR EL CODIGO
                    if (this.ddlWarehouse.SelectedValue.ToString().Length < 2)
                        warehouse = "0" + (this.ddlWarehouse.SelectedValue.ToString().Trim());
                    else
                        warehouse = (this.ddlWarehouse.SelectedValue.ToString()).Substring(0, 2);

                    if (this.ddlHangar.SelectedValue.ToString().Length < 2)
                        hangar = "0" + (this.ddlHangar.SelectedValue.ToString().Trim());
                    else
                        hangar = (this.ddlHangar.SelectedValue.ToString()).Substring(0, 2);

                    if (this.txtRowLoc.Text.Trim().Length == 1)
                        row = "00" + (this.txtRowLoc.Text.Trim());

                    else if (this.txtRowLoc.Text.Trim().Length == 2)
                        row = "0" + (this.txtRowLoc.Text.Trim());
                    else
                        row = (this.txtRowLoc.Text).Substring(0, 3);

                    if (this.txtColumnLoc.Text.Trim().Length < 2)
                        column = "0" + (this.txtColumnLoc.Text.Trim());
                    else
                        column = (this.txtColumnLoc.Text).Substring(0, 2);

                    if (this.txtLevelLoc.Text.Trim().Length < 2)
                        level = "0" + this.txtLevelLoc.Text.Trim();
                    else
                        level = this.txtLevelLoc.Text.Substring(0, 2);

                    codeGenerate.Append(warehouse);
                    codeGenerate.Append(hangar);
                    codeGenerate.Append(row);
                    codeGenerate.Append(column);
                    codeGenerate.Append(level);

                    codGenerado = codeGenerate.ToString();

                    this.divWarning.Visible = false;
                    this.lblError2.Visible = false;
                }
                else
                {
                    this.divWarning.Visible = true;
                    this.lblError2.Visible = true;
                }
            }

            return codGenerado;
        }


        // Agrega la Zona a la Ubicacion actual
        protected void btnAddWorkZone_Click(object sender, EventArgs e)
        {
            try
            {
                AddWorkZone(Convert.ToInt32(hidEditIndex.Value));
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void btnAddPrinters_Click(object sender, EventArgs e)
        {
            try
            {
                AddPrinters(Convert.ToInt32(hidEditIndex.Value));
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega la Zona seleccionada a la grilla de Zonas Asignadas a la ubicación actual (grid view)
        /// </summary>
        /// <param name="index"></param>
        protected void AddWorkZone(int index)
        {
            //TODO:Agregar mensaje de status para esta accion
            if (ddlWorkZones.SelectedIndex > 0)
            {
                WorkZone workzone = new WorkZone(Convert.ToInt32(ddlWorkZones.SelectedValue));
                workzone.Name = ddlWorkZones.SelectedItem.Text;
                workzone.UserCreated = context.SessionInfo.User.UserName;
                workzone.DateCreated = DateTime.Now;

                // Ubicacion nueva
                if (index == -1)
                {
                    locationViewDTO.Entities.Add(new Location());
                    index = locationViewDTO.Entities.Count - 1;

                    hidEditIndex.Value = index.ToString();

                    Session[WMSTekSessions.LocationMgr.LocationList] = locationViewDTO;
                }

                // Si es la primer Zona a agregar, crea la lista
                if (locationViewDTO.Entities[index].WorkZones == null) locationViewDTO.Entities[index].WorkZones = new List<WorkZone>();
                locationViewDTO.Entities[index].WorkZones.Add(workzone);
                grdWorkZones.DataSource = locationViewDTO.Entities[index].WorkZones;

                grdWorkZones.DataBind();

                // Quita la Zona seleccionada de la lista de Zonas a Asignar (drop-down list)
                ddlWorkZones.Items.RemoveAt(ddlWorkZones.SelectedIndex);
            }
        }

        private void LoadWorkZones(int index)
        {
            // TODO: manejo de excepciones
            string id;
            int typeZone = (int)TypeWorkZone.Maquina;
            int idWhs = int.Parse(this.ddlWarehouse.SelectedValue);
            GenericViewDTO<WorkZone> workzoneViewDTO = new GenericViewDTO<WorkZone>();

            // Limpia valores actuales de la grilla
            grdWorkZones.DataSource = null;
            grdWorkZones.DataBind();

            // Zonas asignadas a la ubicacion
            if (index != -1)
            {
                id = locationViewDTO.Entities[index].IdCode.Trim();

                workzoneViewDTO = iLayoutMGR.GetWorkZoneByLocationForkLift(id, idWhs, context);
                grdWorkZones.DataSource = workzoneViewDTO.Entities;
                grdWorkZones.DataBind();

                locationViewDTO.Entities[index].WorkZones = workzoneViewDTO.Entities;
            }
            else
            {
                id = "-1";
            }

            // Zonas NO asignadas a la ubicacion
            //workzoneViewDTO = iLayoutMGR.GetWorkZoneByNotInItem(id, context);
            workzoneViewDTO = iLayoutMGR.GetWorkZoneByTypeZoneNotInLocation(id, typeZone, idWhs,  context);

            ddlWorkZones.Items.Clear();
            ddlWorkZones.DataSource = workzoneViewDTO.Entities;

            foreach (WorkZone workZone in workzoneViewDTO.Entities)
                ddlWorkZones.Items.Add(new ListItem((workZone.Name), workZone.Id.ToString()));

            ddlWorkZones.Items.Insert(0, this.Master.EmptyRowText);
        }

        /// <summary>
        /// Agrega la Impresora seleccionada a la grilla de Zonas Asignadas a la ubicación actual (grid view)
        /// </summary>
        /// <param name="index"></param>
        protected void AddPrinters(int index)
        {
            //TODO:Agregar mensaje de status para esta accion
            if (ddlPrinters.SelectedIndex > 0)
            {
                Printer Printer = new Printer(Convert.ToInt32(ddlPrinters.SelectedValue));
                Printer.Name = ddlPrinters.SelectedItem.Text;
                Printer.IdWmsProcessCode = ddlWmsProcess.SelectedValue;
                Printer.WmsProcessName = ddlWmsProcess.SelectedItem.Text;
                Printer.UserCreated = context.SessionInfo.User.UserName;
                Printer.DateCreated = DateTime.Now;

                // Ubicacion nueva
                if (index == -1)
                {
                    locationViewDTO.Entities.Add(new Location());
                    index = locationViewDTO.Entities.Count - 1;

                    hidEditIndex.Value = index.ToString();

                    Session[WMSTekSessions.LocationMgr.LocationList] = locationViewDTO;
                }

                // Si es la primer Impresora a agregar, crea la lista
                if (locationViewDTO.Entities[index].Printers == null) locationViewDTO.Entities[index].Printers = new List<Printer>();
                locationViewDTO.Entities[index].Printers.Add(Printer);

                grdPrinters.DataSource = locationViewDTO.Entities[index].Printers;
                grdPrinters.DataBind();

                // Quita la Impresora seleccionada de la lista de Imoresoras a Asignar (drop-down list)
                ddlPrinters.Items.RemoveAt(ddlPrinters.SelectedIndex);
            }
        }

        private void LoadPrinters(int index)
        {
            // TODO: manejo de excepciones
            string id;
            string wmsProcess = this.ddlWmsProcess.SelectedValue;
            int idWhs = int.Parse(this.ddlWarehouse.SelectedValue);
            GenericViewDTO<Printer> printerViewDTO = new GenericViewDTO<Printer>();
           
            // Limpia valores actuales de la grilla
            grdPrinters.DataSource = null;
            grdPrinters.DataBind();

            // impresoras asignadas a la ubicacion
            if (index != -1)
            {
                id = locationViewDTO.Entities[index].IdCode.Trim();

                printerViewDTO = iDeviceMGR.GetPrintersByLocationAndWhs(idWhs, id, context);
                grdPrinters.DataSource = printerViewDTO.Entities;
                grdPrinters.DataBind();

                locationViewDTO.Entities[index].Printers = printerViewDTO.Entities;
            }
            else
            {
                id = "-1";
            }

            // Impresoras NO asignadas a la ubicacion
            printerViewDTO = iDeviceMGR.GetPrintersByNotInLocationAndWhs(idWhs, id, context);

            ddlPrinters.Items.Clear();
            ddlPrinters.DataSource = printerViewDTO.Entities;

            foreach (Printer printer in printerViewDTO.Entities)
                ddlPrinters.Items.Add(new ListItem((printer.Name), printer.Id.ToString()));

            ddlPrinters.Items.Insert(0, this.Master.EmptyRowText);
        }

        // Quita la Zona de la Ubicación actual
        protected void grdWorkZones_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                RemoveWorkZone(Convert.ToInt32(hidEditIndex.Value), e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        // Quita la Impresora de la Ubicación actual
        protected void grdPrinters_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                RemovePrinter(Convert.ToInt32(hidEditIndex.Value), e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        protected void grdWorkZones_RowCreated(object sender, GridViewRowEventArgs e)
        {
        }
        protected void grdPrinters_RowCreated(object sender, GridViewRowEventArgs e)
        {
        }

        /// <summary>
        /// Quita la Zona seleccionada de la grilla de Zonas Asignadas a la Ubicación actual (grid view)
        /// </summary>
        /// <param name="index"></param>
        protected void RemoveWorkZone(int index, int zoneIndex)
        {
            //// Agrega la Zona seleccionada a la lista de Zonas a Asignar (drop-down list)
            ddlWorkZones.Items.Add(new ListItem(locationViewDTO.Entities[index].WorkZones[zoneIndex].Name, locationViewDTO.Entities[index].WorkZones[zoneIndex].Id.ToString()));

            //// Quita la Zona seleccionada de la grilla de Zonas Asignadas al Item actual (grid view)
            locationViewDTO.Entities[index].WorkZones.RemoveAt(zoneIndex);

            grdWorkZones.DataSource = locationViewDTO.Entities[index].WorkZones;
            grdWorkZones.DataBind();
        }

        protected void RemovePrinter(int index, int printerIndex)
        {
            //// Agrega la Imoresora seleccionada a la lista de Impresoras a Asignar (drop-down list)
            ddlPrinters.Items.Add(new ListItem(locationViewDTO.Entities[index].Printers[printerIndex].Name, locationViewDTO.Entities[index].Printers[printerIndex].Id.ToString()));

            //// Quita la Impresora seleccionada de la grilla de Impresoras Asignadas al Item actual (grid view)
            locationViewDTO.Entities[index].Printers.RemoveAt(printerIndex);

            grdPrinters.DataSource = locationViewDTO.Entities[index].Printers;
            grdPrinters.DataBind();
        }

        private void validateChkOnlyLPN()
        {
            if (chkOnlyLPN.Checked)
            {
                this.txtCapacityLPN.Enabled = true;
                //this.txtCapacityLPN.Text = string.Empty;
                this.rfvCapacityLPN.Enabled = true;
                this.rvCapacityLPN.MinimumValue = "1";
            }
            else
            {
                this.txtCapacityLPN.Enabled = false;
                //this.txtCapacityLPN.Text = "0";
                this.rfvCapacityLPN.Enabled = false;
                this.rvCapacityLPN.MinimumValue = "0";
            }
        }

        private void validateChkOnlyLPNRange()
        {
            if (chkOnlyLPNRange.Checked)
            {
                this.txtCapacityLPNRange.Enabled = true;
                this.txtCapacityLPNRange.Text = string.Empty;
                this.rfvCapacityLPNRange.Enabled = true;
                this.rvCapacityLPNRange.MinimumValue = "1";
            }
            else
            {
                this.txtCapacityLPNRange.Enabled = false;
                this.txtCapacityLPNRange.Text = "0";
                this.rfvCapacityLPNRange.Enabled = false;
                this.rvCapacityLPNRange.MinimumValue = "0";
            }
            
        }

        private void InitializeTypeUnitOfMeasure_OfMass()
        {
            String type = string.Empty;
            String typeMass = string.Empty;
            String typeVol = string.Empty;

            var lstTypeLoc = GetConst("TypeOfUnitOfMeasure");
            var lstTypeMass = GetConst("TypeOfUnitOfMass");
            var lstTypeVolume = GetConst("TypeOfUnitOfVolume");

            if (lstTypeLoc.Count == 0)
                type = "(mts)";
            else
                type = "(" + lstTypeLoc[0].Trim() + ")";

            if (lstTypeMass.Count == 0)
                typeMass = "(k)";
            else
                typeMass = "(" + lstTypeMass[0].Trim() + ")";

            if (lstTypeVolume.Count == 0)
                typeVol = "(m3)";
            else
                typeVol = "(" + lstTypeVolume[0].Trim() + ")";


            this.lblHeight.Text = "Alto " + type;
            this.lblWidth.Text = "Ancho "+ type;
            this.lblLength.Text = "Largo " + type;

            this.lblWeight.Text = "Peso " + typeMass;
            this.lblVolume.Text = "Volumen " + typeVol;

        }

        private void LoadLocationTypeFilterRange()
        {
            GenericViewDTO<LocationType> typeLocationViewDTO = new GenericViewDTO<LocationType>();
            
            typeLocationViewDTO = iLayoutMGR.FindAllLocationType(context);
            typeLocationViewDTO.Entities = typeLocationViewDTO.Entities.Where(a=> !lstTypeLoc.Select(b=>b).Contains(a.LocTypeCode)).ToList();
       

            this.ddlLocTypeCodeRange.DataSource = typeLocationViewDTO.Entities;
            this.ddlLocTypeCodeRange.DataTextField = "LocTypeName";
            this.ddlLocTypeCodeRange.DataValueField = "LocTypeCode";
            this.ddlLocTypeCodeRange.DataBind();

            this.ddlLocTypeCodeRange.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
            this.ddlLocTypeCodeRange.Items[0].Selected = true;
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "initializeGridDragAndDrop('Location_FindAll', 'ctl00_MainContent_grdMgr');", true);
        }

        #endregion
     
    }
}