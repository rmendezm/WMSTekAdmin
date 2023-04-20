using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.Globalization;
using System.Data;

namespace Binaria.WMSTek.WebClient.Administration.LogisticsResources
{
    public partial class WorkZoneMgr : BasePage
    {
        #region "Declaración de Variables"
        private List<EntityFilter> mainFilter;
        private GenericViewDTO<WorkZone> workZoneViewDTO;
        private GenericViewDTO<Location> locationAssociatedViewDTO = new GenericViewDTO<Location>();
        private GenericViewDTO<Location> locationNoAssociatedViewDTO = new GenericViewDTO<Location>();
        private GenericViewDTO<Auxiliary> auxiliaryViewDTO = new GenericViewDTO<Auxiliary>();
        private bool isValidViewDTO = false;

        // Controla el nro de pagina activa en la grilla
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

        // Controla el indice activo en la grilla
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
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        
                        // Carga inicial del ViewDTO
                        UpdateSession(false);
                        PopulateLists();
                    }

                    context.MainFilter = this.Master.ucMainFilter.MainFilter;

                    if (ValidateSession(WMSTekSessions.WorkZoneMgr.WorkZoneList))
                    {
                        workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.WorkZoneMgr.WorkZoneList];
                        isValidViewDTO = true;
                    }

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                        PopulateGrid();
                    }

                    if (ValidateSession(WMSTekSessions.WorkZoneMgr.LocationListAssociated))
                        locationAssociatedViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.WorkZoneMgr.LocationListAssociated];
                    
                    if (ValidateSession(WMSTekSessions.WorkZoneMgr.LocationListNoAssociated))
                        locationNoAssociatedViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.WorkZoneMgr.LocationListNoAssociated];

                    //PopulateLocationLists();
                }
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
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
                        PopulateLocationLists();

                        if (currentIndex > -1)
                        {
                            ScriptManager.RegisterStartupScript(Page, GetType(), "gridSelected", "gridViewOnclick('" + currentIndex + "', '" + grdMgr.ClientID + "');", true);
                        }
                    }
                }

                //Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }

        // Carga la grilla, filtrada por el criterio de busqueda ingresado
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
                ClearRightPanelNew();
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }

        /// <summary>
        /// Opción Buscar para lista de Ubicaciones asociadas
        /// </summary>
        protected void btnSearchLoc_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkNoAssociated.Checked)
                {
                    SearchLocationsNoAsocied();
                }
                if (chkAssociated.Checked)
                {
                    SearchLocationsAssociated();
                }
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
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
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
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
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
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
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
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
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
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
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount -1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }
        /// <summary>
        /// Quita todas las ubicaciones asociadas
        /// </summary>
        protected void btnQuitarTodas_Click(object sender, EventArgs e)
        {
            try
            {
                ListItem listItemSociadas = new ListItem();
                List<Location> tmpLocations = new List<Location>();

                //Recorre la lista de ubicaciones asociadas
                foreach (ListItem item in lstLocAsociated.Items)
                {
                    //Agrega todo lo que esta en la lista
                    tmpLocations.Add(new Location(item.Value));
                }
                if (tmpLocations.Count > 0)
                {
                    //Elimina las ubicaciones
                    WorkZone workZone = workZoneViewDTO.Entities[currentIndex];
                    workZone.Locations = tmpLocations;

                    workZoneViewDTO = iLayoutMGR.DeleteLocationByLoc(context, workZone);

                    workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.WorkZoneMgr.WorkZoneList];

                    // Recarga las listas de Ubicaciones asociadas y no asociadas
                    SearchLocationsAssociated();
                    SearchLocationsNoAsocied();
                    CountLocations();
                }
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }
        /// <summary>
        /// Quita las ubicaciones seleccionadas
        /// </summary>
        protected void btnQuitarSelec_Click(object sender, EventArgs e)
        {
            try
            {
                ListItem listItem = new ListItem();
                List<Location> tmpLocations = new List<Location>();

                //Recorre la lista de ubicaciones asociadas
                foreach (ListItem item in lstLocAsociated.Items)
                {
                    //Evalua si la ubicacion esta seleccionada
                    if (item.Selected)
                    {
                        tmpLocations.Add(new Location(item.Value));
                    }
                }
                if (tmpLocations.Count > 0)
                {
                //Elimina las ubicaciones
                WorkZone workZone = workZoneViewDTO.Entities[currentIndex];
                workZone.Locations = tmpLocations;

                workZoneViewDTO = iLayoutMGR.DeleteLocationByLoc(context, workZone);

                workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.WorkZoneMgr.WorkZoneList];

                // Recarga las listas de Ubicaciones asociadas y no asociadas
                SearchLocationsAssociated();
                SearchLocationsNoAsocied();
                CountLocations();
                }
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }
        /// <summary>
        /// Agrega las ubicaciones no asociadas seleccionadas
        /// </summary>
        protected void btnAgregarSelec_Click(object sender, EventArgs e)
        {
            ListItem listItem = new ListItem();
            List<Location> tmpLocations = new List<Location>();
            try
            {
                //Recorre la lista de ubicaciones no asociadas
                foreach (ListItem item in lstLocNoAsociated.Items)
                {
                    //Evalua si la ubicacion esta seleccionada
                    if (item.Selected)
                    {
                        tmpLocations.Add(new Location(item.Value));
                    }
                }
                if (tmpLocations.Count > 0)
                {
                    //Elimina las ubicaciones
                    WorkZone workZone = workZoneViewDTO.Entities[currentIndex];
                    workZone.Locations = tmpLocations;

                    workZoneViewDTO = iLayoutMGR.InsertWorkZoneLocation(workZone, context);

                    workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.WorkZoneMgr.WorkZoneList];

                    // Recarga las listas de Ubicaciones asociadas y no asociadas
                    SearchLocationsAssociated();
                    SearchLocationsNoAsocied();
                    CountLocations();
                }
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }
        /// <summary>
        /// Agrega todas las ubicaciones no asociadas
        /// </summary>
        protected void btnAgregarTodas_Click(object sender, EventArgs e)
        {
            ListItem listItem = new ListItem();
            List<Location> tmpLocations = new List<Location>();
            try
            {
                //Recorre la lista de ubicaciones asociadas
                foreach (ListItem item in lstLocNoAsociated.Items)
                {
                   tmpLocations.Add(new Location(item.Value));
                }
                if (tmpLocations.Count > 0)
                {
                    //Elimina las ubicaciones
                    WorkZone workZone = workZoneViewDTO.Entities[currentIndex];
                    workZone.Locations = tmpLocations;

                    workZoneViewDTO = iLayoutMGR.InsertWorkZoneLocation(workZone, context);

                    workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.WorkZoneMgr.WorkZoneList];

                    // Recarga las listas de Ubicaciones asociadas y no asociadas
                    SearchLocationsAssociated();
                    SearchLocationsNoAsocied();
                    CountLocations();
                }
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowModal(0, CRUD.Create);
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
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
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        DataControlFieldCell cell = e.Row.Cells[i] as DataControlFieldCell;

                        if (cell.ContainingField.AccessibleHeaderText.ToUpper().Trim() != "ACTIONS")
                            e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }

        //protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    try
        //    {
        //        //Si es una fila normal
        //        if (e.Row.RowType == DataControlRowType.DataRow)
        //        {
        //            Label lblTypeZoneW = (Label)e.Row.FindControl("lblTypeZone") as Label;
        //            string typeZone = ((WorkZone)e.Row.DataItem).TypeZone.ToString();
                    
        //            if (typeZone != "-1" && typeZone != "")
        //            {   
        //                TypeWorkZone type = (TypeWorkZone)Enum.Parse(typeof(TypeWorkZone), typeZone);

        //                lblTypeZoneW.Text = type.ToString();
        //            }
        //            else
        //            {
        //                lblTypeZoneW.Text = "";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        workZoneViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(workZoneViewDTO.Errors);
        //    }
        //}

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                currentIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;
                
                //string idWhs = this.Master.ucMainFilter.idWhs.ToString();
                //this.Master.ucMainFilter.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Clear();
                //this.Master.ucMainFilter.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(idWhs));
                
                //UpdateSession(false);
                //PopulateGrid();
                ShowModal(currentIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }

        protected void Prueba(object sender, EventArgs e)
        {
            try
            {               
                UpdateSession(false);
                PopulateGrid();

            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a eliminar
                int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;
                //int deleteIndex = int.Parse( this.grdMgr.DataKeys[e.RowIndex].Values["Id"].ToString());
                DeleteRow(deleteIndex);
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //Limpio las listas
                    lstLocAsociated.Items.Clear();
                    lstLocNoAsociated.Items.Clear();

                    lstLocAsociated.Dispose();
                    lstLocNoAsociated.Dispose();

                    locationAssociatedViewDTO.Entities = null;
                    locationNoAssociatedViewDTO.Entities = null;
                    auxiliaryViewDTO.Entities = null;
                     
                    Session.Remove(WMSTekSessions.WorkZoneMgr.LocationListAssociated);
                    Session.Remove(WMSTekSessions.WorkZoneMgr.LocationListNoAssociated);


                    // Capturo la posicion de la fila 
                    currentIndex = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    lblWorkZoneName.Text = workZoneViewDTO.Entities[currentIndex].Name;

                    LoadListFilters();

                    // Limpia el filtro de Ubicaciones asociadas y sin asociar
                    this.ddlLocationType1.SelectedIndex = -1;
                    this.txtIdLocCode1.Text = string.Empty;
                    this.txtAisle.Text = string.Empty;
                    ClearRowLevelColumn();

                    // Muestra el total de Ubicaciones asociadas y sin asociar
                    CountLocations();
                }
            }
            catch (Exception ex)
            {
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }

        private void LoadListFilters()
        {
            try
            {
                // Carga lista de Hangares, según Warehouse de la Zona actual
                if (currentIndex < 0)
                {
                    base.LoadHangar(ddlHangar1, 0, true, this.Master.AllRowsText);
                }
                else
                {
                    base.LoadHangar(ddlHangar1, workZoneViewDTO.Entities[currentIndex].Warehouse.Id, true, this.Master.AllRowsText);
                    //base.LoadLocationType2(this.ddlLocationType1, false, this.Master.AllRowsText);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        protected void ddlHangar1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Cambia las columnas, niveles y filas
            if ( Int32.Parse(ddlHangar1.SelectedValue) > 0 && workZoneViewDTO.Entities[currentIndex].Warehouse != null)
            {
                int idHng = Convert.ToInt32(ddlHangar1.SelectedValue);
                string locTypeCode = ddlLocationType1.SelectedValue;

                base.LoadRowLocationByHngAndLocType(this.ddlRowFrom, this.ddlRowTo, this.Master.EmptyRowText, "-1", true, workZoneViewDTO.Entities[currentIndex].Warehouse.Id, idHng, locTypeCode);
                base.LoadLevelLocationByHngAndLocType(this.ddlLevelFrom, this.ddlLevelTo, this.Master.EmptyRowText, "-1", true, workZoneViewDTO.Entities[currentIndex].Warehouse.Id, idHng, locTypeCode);
                base.LoadColumnLocationByHngAndLocType(this.ddlColumnFrom, this.ddlColumnTo, this.Master.EmptyRowText, "-1", true, workZoneViewDTO.Entities[currentIndex].Warehouse.Id, idHng, locTypeCode);
            }
            else
            {
                ClearRowLevelColumn();
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
                workZoneViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        /// <summary>
        /// Muestra el total de Ubicaciones asociadas y sin asociar 
        /// </summary>
        protected void CountLocations()
        {
            // Ubicaciones asociadas
            auxiliaryViewDTO = iLayoutMGR.GetLocationCountByWorkZone(workZoneViewDTO.Entities[currentIndex].Id, context);

            if (!auxiliaryViewDTO.hasError() && auxiliaryViewDTO.Entities != null && auxiliaryViewDTO.Entities.Count > 0 && auxiliaryViewDTO.Entities[0].Count > 0)
            {
                lblLocationCount1.Text = this.Master.AsociadasText + auxiliaryViewDTO.Entities[0].Count.ToString();
            }
            else
            {
                lblLocationCount1.Text = this.Master.AsociadasText + "0";
            }

            // Ubicaciones disponibles
            auxiliaryViewDTO = iLayoutMGR.GetLocationCountByNotInWorkZone(workZoneViewDTO.Entities[currentIndex].Id, workZoneViewDTO.Entities[currentIndex].Warehouse.Id, context);

            if (!auxiliaryViewDTO.hasError() && auxiliaryViewDTO.Entities != null && auxiliaryViewDTO.Entities.Count > 0 && auxiliaryViewDTO.Entities[0].Count > 0)
            {
                lblLocationCount2.Text = this.Master.SinAsociarText + auxiliaryViewDTO.Entities[0].Count.ToString();
            }
            else
            {
                lblLocationCount2.Text = this.Master.SinAsociarText + "0";
            }
        }

        protected void SearchLocationsNoAsocied()
        {
            try
            {
                if (ddlHangar1.SelectedValue != string.Empty)
                {
                    this.lblError.Text = string.Empty;
                    lblError.Visible = false;

                    // Recupera el objeto filtro principal de memoria
                    mainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];

                    // Limpia el objeto 'Main Filter'
                    ClearFilterObject();

                    // Carga el objeto 'Main Filter' con los valores seleccionados
                    LoadControlValuesToFilterObject1();

                    // Salva los criterios seleccionados
                    context.MainFilter = mainFilter;


                    bool IsValid = true;
                    int levelFrom = Convert.ToInt32(this.ddlLevelFrom.SelectedValue);
                    int levelTo = Convert.ToInt32(this.ddlLevelTo.SelectedValue);
                    int columnFrom = Convert.ToInt32(this.ddlColumnFrom.SelectedValue);
                    int columnTo = Convert.ToInt32(this.ddlColumnTo.SelectedValue);
                    int rowFrom = Convert.ToInt32(this.ddlRowFrom.SelectedValue);
                    int rowTo = Convert.ToInt32(this.ddlRowTo.SelectedValue);
                    int idWorkZone = workZoneViewDTO.Entities[currentIndex].Id;
                    string aisle = this.txtAisle.Text.Trim();

                    //validaciones
                    if (levelTo < levelFrom)
                    {
                        //this.lblError.Text = this.lblErrorLevel.Text;
                        IsValid = false;
                    }
                    if (columnTo < columnFrom)
                    {
                        //this.lblError.Text = this.lblErrorColunm.Text;
                        IsValid = false;
                    }
                    if (rowTo < rowFrom)
                    {
                        //this.lblError.Text = this.lblErrorRow.Text;
                        IsValid = false;
                    }
                    if (IsValid)
                    {
                        if (context.MainFilter.Exists(p => p.Name == "Aisle"))
                        {
                            var oldMain = context.MainFilter.Find(p => p.Name == "Aisle");
                            context.MainFilter.Remove(oldMain);
                        }
                        //locationNoAssociatedViewDTO = iLayoutMGR.GetLocationsByHngLevelColRowNotInWorkZone(idWorkZone, levelFrom, levelTo, rowFrom, rowTo, columnFrom, columnTo, context);
                        locationNoAssociatedViewDTO = iLayoutMGR.GetLocationsByHngLevelColRowAisleNotInWorkZone(idWorkZone, levelFrom, levelTo, rowFrom, rowTo, columnFrom, columnTo, aisle, context); 
                    }

                    if (!locationNoAssociatedViewDTO.hasError() && locationNoAssociatedViewDTO.Entities != null)
                    {
                        Session.Add(WMSTekSessions.WorkZoneMgr.LocationListNoAssociated, locationNoAssociatedViewDTO);
                    }
                }
                else
                {
                    this.lblError.Text = lblErrorNoZoneSelected.Text;
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                //TODO:Agregar manejador de errores
            }
        }

        protected void SearchLocationsAssociated()
        {
            try
            {
                if (ddlHangar1.SelectedValue != string.Empty)
                {
                    this.lblError.Text = string.Empty;
                    lblError.Visible = false;

                    // Recupera el objeto filtro principal de memoria
                    mainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];

                    // Limpia el objeto 'Main Filter'
                    ClearFilterObject();

                    // Carga el objeto 'Main Filter' con los valores seleccionados
                    LoadControlValuesToFilterObject1();

                    // Salva los criterios seleccionados
                    context.MainFilter = mainFilter;


                    bool IsValid = true;
                    int levelFrom = Convert.ToInt32(this.ddlLevelFrom.SelectedValue);
                    int levelTo = Convert.ToInt32(this.ddlLevelTo.SelectedValue);
                    int columnFrom = Convert.ToInt32(this.ddlColumnFrom.SelectedValue);
                    int columnTo = Convert.ToInt32(this.ddlColumnTo.SelectedValue);
                    int rowFrom = Convert.ToInt32(this.ddlRowFrom.SelectedValue);
                    int rowTo = Convert.ToInt32(this.ddlRowTo.SelectedValue);
                    int idWorkZone = workZoneViewDTO.Entities[currentIndex].Id;
                    string aisle = this.txtAisle.Text.Trim();

                    //validaciones
                    if (levelTo < levelFrom)
                    {
                        //this.lblError.Text = this.lblErrorLevel.Text;
                        IsValid = false;
                    }
                    if (columnTo < columnFrom)
                    {
                        //this.lblError.Text = this.lblErrorColunm.Text;
                        IsValid = false;
                    }
                    if (rowTo < rowFrom)
                    {
                        //this.lblError.Text = this.lblErrorRow.Text;
                        IsValid = false;
                    }
                    if (IsValid)
                    {
                        //locationAssociatedViewDTO = iLayoutMGR.GetLocationsByHngLevelColRow(idWorkZone, levelFrom, levelTo, rowFrom, rowTo, columnFrom, columnTo, context);
                        locationAssociatedViewDTO = iLayoutMGR.GetLocationsByHngLevelColRowAisle(idWorkZone, levelFrom, levelTo, rowFrom, rowTo, columnFrom, columnTo, aisle,context);
                    }

                    if (!locationAssociatedViewDTO.hasError() && locationAssociatedViewDTO.Entities != null)
                    {
                        Session.Add(WMSTekSessions.WorkZoneMgr.LocationListAssociated, locationAssociatedViewDTO);
                    }
                }
                else
                {
                    this.lblError.Text = lblErrorNoZoneSelected.Text;
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                //TODO:Agregar manejador de errores
            }
        }

        /// <summary>
        /// Limpia el filtro de Ubicaciones
        /// </summary>
        public void ClearFilterObject()
        {
            foreach (EntityFilter entityFilter in mainFilter)
            {
                entityFilter.FilterValues.Clear();
            }
        }

        /// <summary>
        /// Carga los criterios de búsqueda seleccionados en el filtro de Ubicaciones asociadas
        /// </summary>
        private void LoadControlValuesToFilterObject1()
        {
            int index;

            // Warehouse es obligatorio para cargar lista de ubicaciones
            if (workZoneViewDTO.Entities[currentIndex].Warehouse != null)
            {

                // IdLocCode 
                if (txtIdLocCode1.Text != string.Empty)
                {
                    index = Convert.ToInt16(EntityFilterName.Code);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, txtIdLocCode1.Text));
                    }
                }

                // Location Type
                index = Convert.ToInt16(EntityFilterName.LocationType);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, ddlLocationType1.SelectedValue));
                }

                // Warehouse
                index = Convert.ToInt16(EntityFilterName.Warehouse);

                if (mainFilter[index].FilterValues != null)
                {
                    mainFilter[index].FilterValues.Clear();
                    mainFilter[index].FilterValues.Add(new FilterItem(0, workZoneViewDTO.Entities[currentIndex].Warehouse.Id.ToString()));
                }

                // Hangar
                if (ddlHangar1.SelectedIndex != 0 && ddlHangar1.SelectedIndex != -1)
                {
                    index = Convert.ToInt16(EntityFilterName.Hangar);

                    if (mainFilter[index].FilterValues != null)
                    {
                        mainFilter[index].FilterValues.Clear();
                        mainFilter[index].FilterValues.Add(new FilterItem(0, ddlHangar1.SelectedValue));
                    }
                }

            }
        }

        protected void ClearRowLevelColumn()
        {
            this.ddlRowFrom.Items.Clear();
            this.ddlRowTo.Items.Clear();
            this.ddlLevelFrom.Items.Clear();
            this.ddlLevelTo.Items.Clear();
            this.ddlColumnFrom.Items.Clear();
            this.ddlColumnTo.Items.Clear();
        }

        protected void ClearRightPanel()
        {
            //Limpio las listas
            lstLocAsociated.Items.Clear();
            lstLocNoAsociated.Items.Clear();

            lstLocAsociated.Dispose();
            lstLocNoAsociated.Dispose();

            locationAssociatedViewDTO.Entities = null;
            locationNoAssociatedViewDTO.Entities = null;
            auxiliaryViewDTO.Entities = null;

            Session.Remove(WMSTekSessions.WorkZoneMgr.LocationListAssociated);
            Session.Remove(WMSTekSessions.WorkZoneMgr.LocationListNoAssociated);


            // Capturo la posicion de la fila 
            currentIndex = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
            if (currentIndex > -1)
            {
                lblWorkZoneName.Text = workZoneViewDTO.Entities[currentIndex].Name;
            }
            else
            {
                lblWorkZoneName.Text = String.Empty;
            }

            LoadListFilters();
            base.LoadHangar(ddlHangar1, 0, true, this.Master.AllRowsText);
            base.LoadLocationType2(this.ddlLocationType1, false, this.Master.AllRowsText);

            // Limpia el filtro de Ubicaciones asociadas y sin asociar
            this.ddlLocationType1.SelectedIndex = -1;
            this.txtIdLocCode1.Text = string.Empty;

            ClearRowLevelColumn();

            // Muestra el total de Ubicaciones asociadas y sin asociar
            CountLocations();
        }

        protected void ClearRightPanelNew()
        {
            //Limpio las listas
            lstLocAsociated.Items.Clear();
            lstLocNoAsociated.Items.Clear();

            lstLocAsociated.Dispose();
            lstLocNoAsociated.Dispose();

            locationAssociatedViewDTO.Entities = null;
            locationNoAssociatedViewDTO.Entities = null;
            auxiliaryViewDTO.Entities = null;

            Session.Remove(WMSTekSessions.WorkZoneMgr.LocationListAssociated);
            Session.Remove(WMSTekSessions.WorkZoneMgr.LocationListNoAssociated);

            lblWorkZoneName.Text = String.Empty;
            

            LoadListFilters();

            // Limpia el filtro de Ubicaciones asociadas y sin asociar
            this.ddlLocationType1.SelectedIndex = -1;
            this.txtIdLocCode1.Text = string.Empty;

            ClearRowLevelColumn();

            lblLocationCount1.Text = this.Master.AsociadasText +"0";
            lblLocationCount2.Text = this.Master.SinAsociarText + "0";
        }
    
        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {

                currentPage = 0;
                currentIndex = -1;
                this.Master.ucError.ClearError();
            }
        }

        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            lblLocationCount1.Text = this.Master.AsociadasText + "0";
            lblLocationCount2.Text = this.Master.SinAsociarText + "0";

            LoadTypeWorkZone(this.ddlTypeWorkZone, true, this.Master.EmptyRowText);            
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
                workZoneViewDTO.ClearError();
            }
            workZoneViewDTO = new GenericViewDTO<WorkZone>();

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            if (this.Master.ucMainFilter.idWhs == -1)
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Clear();
                foreach (ListItem item in this.Master.ucMainFilter.warehouseItems)
                {
                    context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(item.Text, item.Value));
                }
            }
            else
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem("", this.Master.ucMainFilter.idWhs.ToString()));
            }                     

           
            workZoneViewDTO = iLayoutMGR.FindAllworkZone(context);

            if (!workZoneViewDTO.hasError() && workZoneViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.WorkZoneMgr.WorkZoneList, workZoneViewDTO);
                Session.Remove(WMSTekSessions.Shared.WorkZoneList);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(workZoneViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(workZoneViewDTO.Errors);
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.statusVisible = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.zoneTypeVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);
            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
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

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!workZoneViewDTO.hasConfigurationError() && workZoneViewDTO.Configuration != null && workZoneViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, workZoneViewDTO.Configuration);

            grdMgr.DataSource = workZoneViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(workZoneViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateLocationLists()
        {
            if (locationAssociatedViewDTO.Entities != null)
            {
                lstLocAsociated.DataSource = locationAssociatedViewDTO.Entities;
                lstLocAsociated.DataTextField = "IdCode";
                lstLocAsociated.DataValueField = "IdCode";
                lstLocAsociated.DataBind();
            }

            if (locationNoAssociatedViewDTO.Entities != null)
            {
                lstLocNoAsociated.DataSource = locationNoAssociatedViewDTO.Entities;
                lstLocNoAsociated.DataTextField = "IdCode";
                lstLocNoAsociated.DataValueField = "IdCode";
                lstLocNoAsociated.DataBind();
            }
        }


    private void PopulateLists()
    {
        var lstTypeLoc = GetConst("LocationTypeZone");

        base.LoadUserWarehouses(this.ddlWarehouse, this.Master.EmptyRowText, "-1", true);
        base.LoadLocationTypeFilter(this.ddlLocationType1, false, this.Master.AllRowsText, lstTypeLoc);
        //base.LoadLocationType2(this.ddlLocationType1, false, this.Master.AllRowsText);
        base.LoadHangar(ddlHangar1, 0, true, this.Master.AllRowsText);
    }

    /// <summary>
    /// Muestra ventana modal con los datos de la entidad a Editar o Crear
    /// </summary>
    /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
    /// <param name="mode">CRUD.Create o CRUD.Update</param>
    protected void ShowModal(int index, CRUD mode)
    {
        // Editar Zona
        if (mode == CRUD.Update)
        {
            //Recupera los datos de la entidad a editar
            hidEditIndex.Value = index.ToString();
            hidEditId.Value = workZoneViewDTO.Entities[index].Id.ToString();

            txtName.Text = workZoneViewDTO.Entities[index].Name;
            txtDescription.Text = workZoneViewDTO.Entities[index].Description;
            ddlWarehouse.SelectedValue = (workZoneViewDTO.Entities[index].Warehouse.Id).ToString();
            chkCodStatus.Checked = workZoneViewDTO.Entities[index].CodStatus;
            ddlTypeWorkZone.SelectedValue = workZoneViewDTO.Entities[index].TypeZone.ToString();
            ddlTypeWorkZone.Enabled = false;

            lblNew.Visible = false;
            lblEdit.Visible = true;
        }

        // Nueva Zona
        if (mode == CRUD.Create)
        {
            context.MainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];

            // Selecciona Warehouse seleccionados en el Filtro
            this.ddlWarehouse.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues[0].Value;

            hidEditId.Value = "0";
            hidEditIndex.Value = "-1";
            this.txtName.Text = string.Empty;
            this.txtDescription.Text = string.Empty;
            this.chkCodStatus.Checked = true;
            //this.txtName.ReadOnly = false;
            ddlTypeWorkZone.Enabled = true;
            this.ddlTypeWorkZone.SelectedValue = "-1";
            
            lblNew.Visible = true;
            lblEdit.Visible = false;
        }

        if (workZoneViewDTO.Configuration != null && workZoneViewDTO.Configuration.Count > 0)
        {
            if (mode == CRUD.Create)
                base.ConfigureModal(workZoneViewDTO.Configuration, true);
            else
            {
                base.ConfigureModal(workZoneViewDTO.Configuration, false);
                this.txtName.Enabled = true;
            }
        }

        divEditNew.Visible = true;
        mpeworkZone.Show();
    }

    protected void SaveChanges()
    {
        //agrega los datos del Zona
        WorkZone workZone = new WorkZone(Convert.ToInt32(hidEditId.Value));

        workZone.Warehouse = new Warehouse(Convert.ToInt32(ddlWarehouse.SelectedValue));
        workZone.Name = txtName.Text.Trim();
        workZone.Description = txtDescription.Text.Trim();
        workZone.CodStatus = this.chkCodStatus.Checked;
        workZone.TypeZone = Convert.ToInt16(ddlTypeWorkZone.SelectedValue);

        //Locations
        int index = Convert.ToInt32(hidEditIndex.Value);

        if (index != -1
            && workZoneViewDTO.Entities[index] != null
            && workZoneViewDTO.Entities[index].Locations != null
            && workZoneViewDTO.Entities[index].Locations.Count > 0)

            workZone.Locations = locationAssociatedViewDTO.Entities;

        if (hidEditId.Value == "0")
            workZoneViewDTO = iLayoutMGR.MaintainWorkZone(CRUD.Create, workZone, false, context);
        else
        {
            auxiliaryViewDTO = iLayoutMGR.GetLocationCountByWorkZone(workZoneViewDTO.Entities[index].Id, context);

            if (auxiliaryViewDTO.Entities[0].Count > 0 && 
                int.Parse(ddlWarehouse.SelectedValue) != workZoneViewDTO.Entities[index].Warehouse.Id)
            {
                auxiliaryViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.WorkZone.InvalidUpdate.UpdateWhs, context));
                this.Master.ucError.ShowError(auxiliaryViewDTO.Errors);
                auxiliaryViewDTO.ClearError();
            }
            else
            {
                workZoneViewDTO = iLayoutMGR.MaintainWorkZone(CRUD.Update, workZone, false, context);
            }
        }

        divEditNew.Visible = false;
        mpeworkZone.Hide();

        if (workZoneViewDTO.hasError())
        {
            UpdateSession(true);
            divEditNew.Visible = true;
            mpeworkZone.Show();
        }
        else
        {
            crud = true;
            ucStatus.ShowMessage(workZoneViewDTO.MessageStatus.Message);
            UpdateSession(false);
        }
    }

    /// <summary>
    /// Elimina la entidad
    /// </summary>
    /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
    private void DeleteRow(int index)
    {
        workZoneViewDTO = iLayoutMGR.MaintainWorkZone(CRUD.Delete, workZoneViewDTO.Entities[index], false, context);

        if (workZoneViewDTO.hasError())
            UpdateSession(true);
        else
        {
            ucStatus.ShowMessage(workZoneViewDTO.MessageStatus.Message);
            crud = true;
            UpdateSession(false);
        }
        currentPage = 0;
        currentIndex = -1;
    }

    
    #endregion

    }
}
