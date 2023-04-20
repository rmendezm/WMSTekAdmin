using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Script.Services;
using System.Runtime.Serialization.Json;
using System.Web.Services;

using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.Entities.Warehousing;

namespace Binaria.WMSTek.WebClient.Stocks
{
    public partial class Map2DConsult : BasePage
    {
        #region "Declaración de Variables"
        private static GenericViewDTO<MapHangar> hangarViewDTO = new GenericViewDTO<MapHangar>();
        private static GenericViewDTO<MapLayout> mapLayoutViewDTO = new GenericViewDTO<MapLayout>();
        private static GenericViewDTO<MovementWeb> locationsMostUsedViewDTO = new GenericViewDTO<MovementWeb>();
        private bool isValidViewDTO = false;

        //Propiedad para obtener el id del centro seleccionado
        public int currentIdWhs
        {
            get
            {
                if (ValidateViewState("idWhs"))
                    return (int)ViewState["idWhs"];
                else
                    return -1;
            }

            set { ViewState["idWhs"] = value; }
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
                        Session[WMSTekSessions.Map2DConsult.CurrentHangar] = null;
                        Session[WMSTekSessions.Map2DConsult.MapLayout] = null;
                        Session[WMSTekSessions.Map2DConsult.LocationsMostUsedByItem] = null;

                        this.divMsg.Visible = true;
                        this.lblMsg.Visible = true;
                    }

                    if (ValidateSession(WMSTekSessions.Map2DConsult.CurrentHangar))
                    {
                        hangarViewDTO = (GenericViewDTO<MapHangar>)Session[WMSTekSessions.Map2DConsult.CurrentHangar];
                        mapLayoutViewDTO = (GenericViewDTO<MapLayout>)Session[WMSTekSessions.Map2DConsult.MapLayout];
                        locationsMostUsedViewDTO = (GenericViewDTO<MovementWeb>)Session[WMSTekSessions.Map2DConsult.LocationsMostUsedByItem];
                        isValidViewDTO = true;
                    }

                }
            }
            catch (Exception ex)
            {
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    base.Page_Load(sender, e);

                    // Si cambió el Warehouse seleccionado, carga la lista de Hangares
                    if (currentIdWhs != Master.ucMainFilter.idWhs && Page.IsPostBack)
                    {
                        currentIdWhs = Master.ucMainFilter.idWhs;
                        UpdateSession(false);
                    }
                }
            }
            catch (Exception ex)
            {
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }
 
        /// <summary>
        /// Limpia el filtro y carga los datos desde base de datos
        /// </summary>
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Stock/Map/Map2DConsult.aspx");
            }
            catch (Exception ex)
            {
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {

                if (this.Master.ucMainFilter.selectIndexWhs < 1 || Master.ucMainFilter.selectIndexHangar < 1)
                {
                    //ucStatus.ShowWarning("Debe seleccionar una bodega");

                    ScriptManager.RegisterStartupScript(this, typeof(Page), "carga", "ClearCanvas();", true);
                    this.divMsg.Visible = true;
                }
                else
                {
                    UpdateSession(false);

                    if (isValidViewDTO)
                    {
                        string script = "GetLayoutMapClient(" + this.Master.ucMainFilter.selectIndexHangar + ");";
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "carga", script, true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "carga", "ClearCanvas();", true);
                        this.divMsg.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }

        protected void btnClean_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.ucMainFilter.mapFabricationDate = string.Empty;
                this.Master.ucMainFilter.mapExpirationDate = string.Empty;
                this.Master.ucMainFilter.mapFifoDate = string.Empty;
                this.Master.ucMainFilter.mapLote = string.Empty;
                this.Master.ucMainFilter.mapLPN = string.Empty;
                this.Master.ucMainFilter.mapCategory = string.Empty;
                this.Master.ucMainFilter.mapHoldLocation = false;

                string script = "cleanControlsMap();";
                ScriptManager.RegisterStartupScript(this, typeof(Page), "clean", script, true);        
            }
            catch (Exception ex)
            {
                 hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }

        protected void ddlWareHouseIndexChangedMap(object sender, EventArgs e)
        {
            //Busca solo los hangares activos
            int idWhs = Convert.ToInt16(this.Master.ucMainFilter.selectIndexWhs);
            base.LoadHangar((DropDownList)this.Master.ucMainFilter.ddlFilterHangarObject, idWhs, true, this.Master.AllRowsText);
            
        }

        #endregion

        #region "Métodos"
        protected void Initialize()
        {
           // InitializeMap();
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;
        }

        /// <summary>
        /// Configuracion inicial del Filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.hangarVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.searchVisible = false;
            this.Master.ucMainFilter.searchMap2DVisible = false;
            this.Master.ucMainFilter.searchMap2DNewVisible = true;
            this.Master.ucMainFilter.cleanMapVisible = true;
            this.Master.ucMainFilter.divLocationsMostUsedByItemVisible = true;

            //Nuevo Filtro Avanzado para el Mapa
            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabMapaBodegaVisible = true;
            this.Master.ucMainFilter.ChkDisabledAndChequed = false;

            // Configura textos a mostar
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;

            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.chkDateToEnabled = false;
            this.Master.ucMainFilter.chkDateFromEnabled = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchMap2DNewClick += new EventHandler(btnSearch_Click);
            this.Master.ucMainFilter.BtnCleanClick += new EventHandler(btnClean_Click);
            this.Master.ucMainFilter.ddlWareHouseIndexChanged += new EventHandler(ddlWareHouseIndexChangedMap);
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
                hangarViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            isValidViewDTO = true;

            if (Master.ucMainFilter.chkLocationsMostUsedByItemChecked)
            {
                var dateFilter = context.MainFilter.Where(f => f.Name == "DateRange").FirstOrDefault();

                if (dateFilter == null || dateFilter.FilterValues.Count == 0)
                {
                    isValidViewDTO = false;
                    ucStatus.ShowWarning(lblFilterDateRequired.Text);
                }

                if (isValidViewDTO)
                {
                    var txtFilterItem = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterItem");

                    if (txtFilterItem != null)
                    {
                        if (string.IsNullOrEmpty(txtFilterItem.Text))
                        {
                            isValidViewDTO = false;
                            ucStatus.ShowWarning(lblFilterItemRequired.Text);
                        }
                        else
                        {
                            var filterItem = context.MainFilter.Where(f => f.Name == "Item").FirstOrDefault();

                            if (filterItem != null && filterItem.FilterValues.Count > 0)
                            {
                                var itemCode = filterItem.FilterValues.FirstOrDefault().Value;

                                if (!string.IsNullOrEmpty(itemCode))
                                {
                                    var filterOwner = context.MainFilter.Where(f => f.Name == "Owner").FirstOrDefault();
                                    var idOwn = int.Parse(filterOwner.FilterValues.First(o => o.Value != "-1").Value);

                                    var itemViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, itemCode, idOwn, true);

                                    if (!itemViewDTO.hasError() && itemViewDTO.Entities.Count > 0)
                                    {
                                        filterItem.FilterValues.First().Value = itemViewDTO.Entities.First().Id.ToString();
                                    }
                                }

                                locationsMostUsedViewDTO = iWarehousingMGR.FindRotationLocationByItem(context);

                                if (locationsMostUsedViewDTO.hasError())
                                {
                                    isValidViewDTO = false;
                                    this.Master.ucError.ShowError(locationsMostUsedViewDTO.Errors);
                                }
                                else
                                {
                                    Session.Add(WMSTekSessions.Map2DConsult.LocationsMostUsedByItem, locationsMostUsedViewDTO);
                                    isValidViewDTO = true;
                                    ucStatus.ShowMessage(locationsMostUsedViewDTO.MessageStatus.Message);
                                }

                            }
                        }
                    }
                }  
            }

            if (!isValidViewDTO)
            {
                return;
            }

            // Configuración general del Mapa 2D
            mapLayoutViewDTO = iLayoutMGR.GetMapLayout(context);

            if (!mapLayoutViewDTO.hasError() && mapLayoutViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Map2DConsult.MapLayout, mapLayoutViewDTO);
                isValidViewDTO = true;
                ucStatus.ShowMessage(mapLayoutViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(mapLayoutViewDTO.Errors);
            }

            // Lista de Bodegas del Centro seleccionado
            hangarViewDTO = iLayoutMGR.GetFullHangarByWhs(currentIdWhs, context);

            if (!hangarViewDTO.hasError() && hangarViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Map2DConsult.CurrentHangar, hangarViewDTO);
                isValidViewDTO = true;
                ucStatus.ShowMessage(hangarViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }

        protected void ReloadData()
        {
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                this.Master.ucError.ClearError();
            }
        }

        [WebMethod]
        public static MapLayout GetLayoutMap(string index)
        {
            int i;

            if (hangarViewDTO.Entities != null && hangarViewDTO.Entities.Count > 0 && mapLayoutViewDTO.Entities != null && mapLayoutViewDTO.Entities.Count > 0)
            {
                if (!String.IsNullOrEmpty(index))
                {
                    i = Convert.ToInt32(index) - 1;

                    hangarViewDTO.Entities = hangarViewDTO.Entities.OrderBy(e => e.Name).ToList();
                    mapLayoutViewDTO.Entities[0].Hangar = hangarViewDTO.Entities[i];
                }
            }

            if (locationsMostUsedViewDTO != null && locationsMostUsedViewDTO.Entities != null && locationsMostUsedViewDTO.Entities.Count > 0)
            {
                mapLayoutViewDTO.Entities[0].LocationsMostUsedByItem = locationsMostUsedViewDTO.Entities;
            }
            else
            {
                mapLayoutViewDTO.Entities[0].LocationsMostUsedByItem = null;
            }

            return mapLayoutViewDTO.Entities[0];
        }

        #endregion
    }
}

