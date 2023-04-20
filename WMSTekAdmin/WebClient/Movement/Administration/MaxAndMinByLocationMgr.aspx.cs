using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.I18N;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.DTO;
using System.IO;
using System.Data;

namespace Binaria.WMSTek.WebClient.Outbound.Administration
{
    public partial class MaxAndMinByLocationMgr : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<ItemLocation> itemLocationViewDTO = new GenericViewDTO<ItemLocation>();
        private GenericViewDTO<Item> itemSearchViewDTO;
        private GenericViewDTO<Location> locationSearchViewDTO;

        private bool isValidViewDTO = false;

        // Propiedad para controlar el nro de pagina activa en la grilla
        public int currentPage
        {
            get
            {
                if (ValidateViewState("pageMaxAndMinByLocationMgr"))
                    return (int)ViewState["pageMaxAndMinByLocationMgr"];
                else
                    return 0;
            }

            set { ViewState["pageMaxAndMinByLocationMgr"] = value; }
        }

        public int currentPageItems
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

        public int currentPageLocations
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
        public int currentIdOwn
        {
            get
            {
                if (ValidateViewState("IdOwn"))
                    return (int)ViewState["IdOwn"];
                else
                    return -1;
            }

            set { ViewState["IdOwn"] = value; }
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
                }
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void imgBtnSearchItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {

                    //pnlError.Visible = false;
                    //int idOwner = this.Master.ucMainFilter.idOwn;
                    int idOwner = int.Parse(this.ddlIdOwner.SelectedValue);

                    ucFilterItem.Clear();
                    ucFilterItem.Initialize();

                    // Setea el filtro con el Item ingresado
                    if (txtItemCode.Text.Trim() != string.Empty)
                    {
                        FilterItem filterItem = new FilterItem("%" + txtItemCode.Text + "%");
                        filterItem.Selected = true;
                        ucFilterItem.FilterItems[0] = filterItem;

                        ucFilterItem.LoadCurrentFilter(ucFilterItem.FilterItems);
                        SearchItem();
                    }
                    // Si no se ingresó ningún item, no se ejecuta la búsqueda
                    else
                        ClearGridItem();

                    // Esto evita un bug de ajax
                    //valAddItem.Enabled = false;

                    divLookupItem.Visible = true;
                    mpLookupItem.Show();

                    if (ddlPagesSearchItems.Items.Count > 0)
                        ddlPagesSearchItems.SelectedIndex = 0;

                    InitializePageCountItems();

                }

            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void btnSearchItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    SearchItem();

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    //if (isValidViewDTO)
                    //{
                        divLookupItem.Visible = true;
                        mpLookupItem.Show();
                        currentPageItems = 0;

                        if (ddlPagesSearchItems.Items.Count > 0)
                            ddlPagesSearchItems.SelectedIndex = 0;
                        
                        InitializePageCountItems();
                    //}
                }
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void imgBtnSearchLocation_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {

                    //pnlError.Visible = false;
                    //int idOwner = this.Master.ucMainFilter.idOwn;
                    //int idOwner = int.Parse(this.ddlIdOwner.SelectedValue);

                    ucFilterLocation.Clear();
                    ucFilterLocation.Initialize();

                    // Setea el filtro con el Item ingresado
                    if (txtLocCode.Text.Trim() != string.Empty)
                    {
                        FilterItem filterItem = new FilterItem("%" + txtLocCode.Text + "%");
                        filterItem.Selected = true;
                        ucFilterLocation.FilterItems[0] = filterItem;

                        ucFilterLocation.LoadCurrentFilter(ucFilterLocation.FilterItems);
                        SearchLocation();
                    }
                    // Si no se ingresó ningún item, no se ejecuta la búsqueda
                    else
                        ClearGridLocation();

                    // Esto evita un bug de ajax
                    //valAddItem.Enabled = false;

                    divLookupLocation.Visible = true;
                    mpLookupLocation.Show();

                    if (ddlPagesSearchLocations.Items.Count > 0)
                        currentPageLocations = 0;
                    
                    InitializePageCountLocations();

                }

            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void btnSearchLocation_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {

                    //// Setea el filtro con el Item ingresado
                    //if (txtLocCode.Text.Trim() != string.Empty)
                    //{
                    //    FilterItem filterItem = new FilterItem("%" + txtLocCode.Text + "%");
                    //    filterItem.Selected = true;
                    //    ucFilterLocation.FilterItems[0] = filterItem;

                    //    ucFilterLocation.LoadCurrentFilter(ucFilterLocation.FilterItems);
                    //}

                    SearchLocation();

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    //if (isValidViewDTO)
                    //{
                    divLookupLocation.Visible = true;
                    mpLookupLocation.Show();

                    if (ddlPagesSearchLocations.Items.Count > 0)
                        currentPageLocations = 0;
                    
                    InitializePageCountLocations();
                    //}
                }
            }
            catch (Exception ex)
            {
                locationSearchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationSearchViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        /// <summary>
        /// Persiste los cambios en la entidad (nueva o editada)
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
                currentIndex = -1;
                //PopulateLists();
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        /// <summary>
        /// Abre la ventana modal para crear una nueva entidad
        /// </summary>
        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                currentIndex = -1;
                ShowModal(0, CRUD.Create);
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSession(false);
                divUpNew.Visible = false;
                divGrid.Visible = true;
            }
            catch(Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

                currentIndex = editIndex;

                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
                currentIndex = -1;
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    currentIndex = grdMgr.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void btnFirstGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = 0;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);
        }

        protected void btnPrevGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems > 0)
            {
                ddlPagesSearchItems.SelectedIndex = currentPageItems - 1; ;
                ddlPagesSearchItemsSelectedIndexChanged(sender, e);
            }
        }

        protected void btnNextGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems < grdSearchItems.PageCount)
            {
                ddlPagesSearchItems.SelectedIndex = currentPageItems + 1; ;
                ddlPagesSearchItemsSelectedIndexChanged(sender, e);

            }
        }

        protected void btnLastGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = grdSearchItems.PageCount - 1;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);

        }

        protected void ddlPagesSearchItemsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValidateSession(WMSTekSessions.Shared.ItemList))
            {
                itemSearchViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.Shared.ItemList];

                currentPageItems = ddlPagesSearchItems.SelectedIndex;
                grdSearchItems.PageIndex = currentPageItems;
                
                grdSearchItems.DataSource = itemSearchViewDTO.Entities;
                grdSearchItems.DataBind();

                divLookupItem.Visible = true;
                mpLookupItem.Show();

                ShowItemsButtonsPage();

            }
        }

        protected void grdSearchItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int editIndex = (Convert.ToInt32(grdSearchItems.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                if (ValidateSession(WMSTekSessions.Shared.ItemList))
                {
                    itemSearchViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.Shared.ItemList];

                    foreach (Item item in itemSearchViewDTO.Entities)
                    {
                        if (item.Id == editIndex)
                        {
                            this.txtItemCode.Text = item.Code;
                            this.txtItemName.Text = item.Description;
                            hidItemId.Value = item.Id.ToString();
                            Session.Add("MaxAndMinByLocationSearchItem", item);

                            //this.Master.ucMainFilter.MainFilter[8].FilterValues.Add(filter);
                            //context.MainFilter = this.Master.ucMainFilter.MainFilter;

                            //FilterItem filter = new FilterItem("", item.Id.ToString());
                            //ContextViewDTO contexto = new ContextViewDTO();
                            //contexto.MainFilter = this.Master.ucMainFilter.MainFilter;
                            //contexto.MainFilter[Convert.ToInt16(EntityFilterName.Item)].FilterValues.Add(filter);
                            //contexto.MainFilter[Convert.ToInt16(EntityFilterName.Item)].FilterValues.Remove(filter);

                            // Esto evita un bug de ajax
                            //valEditNew.Enabled = true;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }


        protected void btnFirstGrdSearchLocations_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchLocations.SelectedIndex = 0;
            ddlPagesSearchLocationsSelectedIndexChanged(sender, e);
        }

        protected void btnPrevGrdSearchLocations_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems > 0)
            {
                ddlPagesSearchLocations.SelectedIndex = currentPageItems - 1; ;
                ddlPagesSearchLocationsSelectedIndexChanged(sender, e);
            }
        }

        protected void btnNextGrdSearchLocations_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems < grdSearchLocations.PageCount)
            {
                ddlPagesSearchLocations.SelectedIndex = currentPageItems + 1; ;
                ddlPagesSearchLocationsSelectedIndexChanged(sender, e);

            }
        }

        protected void btnLastGrdSearchLocations_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchLocations.SelectedIndex = grdSearchLocations.PageCount - 1;
            ddlPagesSearchLocationsSelectedIndexChanged(sender, e);

        }

        protected void ddlPagesSearchLocationsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValidateSession(WMSTekSessions.Shared.LocationList))
            {
                locationSearchViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.Shared.LocationList];

                currentPageLocations = ddlPagesSearchLocations.SelectedIndex;
                grdSearchLocations.PageIndex = currentPageLocations;

                grdSearchLocations.DataSource = locationSearchViewDTO.Entities;
                grdSearchLocations.DataBind();

                divLookupLocation.Visible = true;
                divLookupItem.Visible = false;
                divUpNew.Visible = true;
                mpLookupLocation.Show();

                ShowLocationsButtonsPage();

            }
        }

        protected void grdSearchLocations_RowCommand(object sender, GridViewCommandEventArgs e)

        {
            try
            {
                string editIndex = (Convert.ToString(grdSearchLocations.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                if (ValidateSession(WMSTekSessions.Shared.LocationList))
                {
                    locationSearchViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.Shared.LocationList];

                    foreach (Location location in locationSearchViewDTO.Entities)
                    {
                        if (location.IdCode == editIndex)
                        {
                            this.txtLocCode.Text = location.Code;
                            //this.txtItemName.Text = location.Description;
                            hidItemId.Value = location.IdCode.ToString();
                            Session.Add("MaxAndMinByLocationSearchLocation", location);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                locationSearchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationSearchViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        private void ShowItemsButtonsPage()
        {
            if (currentPageItems == grdSearchItems.PageCount - 1)
            {
                btnNextGrdSearchItems.Enabled = false;
                btnNextGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastGrdSearchItems.Enabled = false;
                btnLastGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                btnPrevGrdSearchItems.Enabled = true;
                btnPrevGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                btnFirstGrdSearchItems.Enabled = true;
                btnFirstGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
            }
            else
            {
                if (currentPageItems == 0)
                {
                    btnPrevGrdSearchItems.Enabled = false;
                    btnPrevGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstGrdSearchItems.Enabled = false;
                    btnFirstGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                    btnNextGrdSearchItems.Enabled = true;
                    btnNextGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchItems.Enabled = true;
                    btnLastGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnNextGrdSearchItems.Enabled = true;
                    btnNextGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchItems.Enabled = true;
                    btnLastGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevGrdSearchItems.Enabled = true;
                    btnPrevGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstGrdSearchItems.Enabled = true;
                    btnFirstGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
            }
        }

        private void ShowLocationsButtonsPage()
        {
            if (currentPageLocations == grdSearchLocations.PageCount - 1)
            {
                btnNextGrdSearchLocations.Enabled = false;
                btnNextGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastGrdSearchLocations.Enabled = false;
                btnLastGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                btnPrevGrdSearchLocations.Enabled = true;
                btnPrevGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                btnFirstGrdSearchLocations.Enabled = true;
                btnFirstGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
            }
            else
            {
                if (currentPageLocations == 0)
                {
                    btnPrevGrdSearchLocations.Enabled = false;
                    btnPrevGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstGrdSearchLocations.Enabled = false;
                    btnFirstGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                    btnNextGrdSearchLocations.Enabled = true;
                    btnNextGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchLocations.Enabled = true;
                    btnLastGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnNextGrdSearchLocations.Enabled = true;
                    btnNextGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchLocations.Enabled = true;
                    btnLastGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevGrdSearchLocations.Enabled = true;
                    btnPrevGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstGrdSearchLocations.Enabled = true;
                    btnFirstGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
            }
        }

        protected void grdSearchItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdSearchLocations_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
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
        //        itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
        //    }
        //}

        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            try
            {
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }
        protected void btnLoadFile_Click(object sender, EventArgs e)
        {
            string pathAux = "";

            try
            {
                string errorUp = "";

                if (uploadFile.HasFile)
                {
                    string savePath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("UpLoadItemFilePath", "");
                    savePath += uploadFile.FileName;
                    pathAux = savePath;

                    try
                    {
                        uploadFile.SaveAs(savePath);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidDataException(ex.Message);
                    }

                    DataTable dataTable;
                    try
                    {
                        dataTable = base.ConvertXlsToDataTableHeader(savePath);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       OwnCode = r.Field<object>("OwnCode"),
                                       ItemCode = r.Field<object>("ItemCode"),
                                       LocCode = r.Field<object>("LocCode"),
                                       Min = r.Field<object>("Min"),
                                       Max = r.Field<object>("Max"),
                                   };

                    var itemLocations = new GenericViewDTO<ItemLocation>();

                    try
                    {
                        foreach (var minMaxConfig in lstExcel)
                        {
                            if (!IsValidExcelRow(minMaxConfig))
                                continue;

                            var newItemLocation = new ItemLocation();

                            if (!ValidateIsNotNull(minMaxConfig.OwnCode))
                            {
                                errorUp = "OwnCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItemLocation.Owner = new Owner() { Code = minMaxConfig.OwnCode.ToString().Trim() };
                            }

                            if (!ValidateIsNotNull(minMaxConfig.ItemCode))
                            {
                                errorUp = "ItemCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItemLocation.Item = new Item() { Code = minMaxConfig.ItemCode.ToString().Trim() };
                            }

                            if (!ValidateIsNotNull(minMaxConfig.LocCode))
                            {
                                errorUp = "LocCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItemLocation.Location = new Location() { Code = minMaxConfig.LocCode.ToString().Trim().Replace("'", "") };
                            }

                            if (!ValidateIsNotNull(minMaxConfig.Min))
                            {
                                errorUp = "Min " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                decimal min = 0;
                                var isMinNumber = decimal.TryParse(minMaxConfig.Min.ToString(), out min);

                                if (isMinNumber)
                                {
                                    if (min < 0)
                                    {
                                        errorUp = "Min " + this.lblValidateMin.Text.Replace("[LOCCODE]", newItemLocation.Location.Code);
                                        break;
                                    }
                                    else
                                    {
                                        newItemLocation.ReOrderPoint = min;
                                    }
                                }
                                else
                                {
                                    errorUp = "Min " + this.lblValidateFormatMin.Text.Replace("[LOCCODE]", newItemLocation.Location.Code);
                                    break;
                                }
                            }

                            if (!ValidateIsNotNull(minMaxConfig.Max))
                            {
                                errorUp = "Max " + this.lblFieldNotNull.Text.Replace("[LOCCODE]", newItemLocation.Location.Code);
                                break;
                            }
                            else
                            {
                                decimal max = 0;
                                var isMaxNumber = decimal.TryParse(minMaxConfig.Max.ToString(), out max);

                                if (isMaxNumber)
                                {
                                    if (max <= 0)
                                    {
                                        errorUp = "Max " + this.lblValidateMax.Text.Replace("[LOCCODE]", newItemLocation.Location.Code);
                                        break;
                                    }
                                    else
                                    {
                                        newItemLocation.ReOrderQty = max;
                                    }
                                }
                                else
                                {
                                    errorUp = "Max " + this.lblValidateFormatMax.Text.Replace("[LOCCODE]", newItemLocation.Location.Code);
                                    break;
                                }
                            }

                            if (newItemLocation.ReOrderPoint > newItemLocation.ReOrderQty)
                            {
                                errorUp = this.lblValidateMinToMax.Text.Replace("[LOCCODE]", newItemLocation.Location.Code);
                                break;
                            }

                            itemLocations.Entities.Add(newItemLocation);
                        }

                        if (string.IsNullOrEmpty(errorUp))
                        {
                            var repeteatedItemLocations = itemLocations.Entities.GroupBy(il => new
                            {
                                ItemCode = il.Item.Code,
                                OwnCode = il.Owner.Code,
                                LocCode = il.Location.Code
                            })
                            .Where(grp => grp.Count() > 1)
                            .Select(od => new
                            {
                                itemCode = od.Key.ItemCode,
                                ownCode = od.Key.OwnCode,
                                locCode = od.Key.LocCode,
                                count = od.Count()
                            }).ToList();

                            if (repeteatedItemLocations != null && repeteatedItemLocations.Count > 0)
                                errorUp = this.lblValidateRepeatedItems.Text;
                        }

                        if (errorUp != "")
                        {
                            ShowAlertLocal(this.lblTitle.Text, errorUp.Trim());
                            divLoad.Visible = false;
                            modalPopUpLoad.Hide();
                        }
                        else
                        {
                            if (itemLocations.Entities.Count > 0)
                            {
                                var itemLocationDTO = iWarehousingMGR.MaintainItemLocationMassive(itemLocations, context);

                                if (itemLocationDTO.hasError())
                                {
                                    ShowAlertLocal(this.lblTitle.Text, itemLocationDTO.Errors.Message);
                                }
                                else
                                {
                                    ucStatus.ShowMessage(itemLocationDTO.MessageStatus.Message);
                                    ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);
                                    divLoad.Visible = false;
                                    modalPopUpLoad.Hide();
                                }
                            }
                            else
                            {
                                ShowAlertLocal(this.lblTitle.Text, this.lblNotItemLocationsInFile.Text);
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }
                }
                else
                {
                    ShowAlertLocal(this.lblTitle.Text, this.lblNotFileLoad.Text);
                    divLoad.Visible = false;
                    modalPopUpLoad.Hide();
                }
            }
            catch (InvalidDataException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoad.Visible = false;
                modalPopUpLoad.Hide();
            }
            catch (InvalidOperationException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoad.Visible = false;
                modalPopUpLoad.Hide();
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, itemLocationViewDTO.Errors.Message);
            }
            finally
            {
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }
            }
        }

        #endregion

        #region "Metodos"
        public void Initialize()
        {
            context.SessionInfo.IdPage = "MaxAndMinByLocationMgr";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeFilterItem();
            InitializeFilterLocation();

            if (!Page.IsPostBack)
            {
                UpdateSession(false);
                PopulateLists();
            }
            else
            {

                if (ValidateSession(WMSTekSessions.ItemLocationMgr.ItemLocationList))
                {
                    itemLocationViewDTO = (GenericViewDTO<ItemLocation>)Session[WMSTekSessions.ItemLocationMgr.ItemLocationList];
                    isValidViewDTO = true;
                }

                // Si es un ViewDTO valido, carga la grilla
                if (isValidViewDTO)
                {
                    //Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                    PopulateGrid();
                }
            }
        }

        /// <summary>
        /// Carga en sesion lista de Usuarios
        /// </summary>
        /// <param name="showError"></param>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                //modalPopUp.Hide();
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
                itemLocationViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            itemLocationViewDTO = iWarehousingMGR.MaxMinByLocation(context);

            if (!itemLocationViewDTO.hasError() && itemLocationViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemLocationMgr.ItemLocationList, itemLocationViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(itemLocationViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
                Session.Remove(WMSTekSessions.ItemLocationMgr.ItemLocationList);
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar Filtro Basico
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;//Centro Obligatorio
            this.Master.ucMainFilter.ownerVisible = true;
            //this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.itemVisible = true;//Item Code
            this.Master.ucMainFilter.nameVisible = true;//Item Name

            this.Master.ucMainFilter.SetDefaultOwner = true;
            //Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabLocationVisible = true;
            this.Master.ucMainFilter.locationFilterVisible = true;

            //Esconde filtros no utilizados
            this.Master.ucMainFilter.LocationEqualVisible = false;
            this.Master.ucMainFilter.LocationRangeVisible = false;
            this.Master.ucMainFilter.LocationRowColumnEqualVisible = false;
            this.Master.ucMainFilter.LocationLevelEqualVisible = false;
            this.Master.ucMainFilter.LocationAisleEqualVisible = false;
            this.Master.ucMainFilter.ChkDisabledAndChequed = false;
            this.Master.ucMainFilter.LocationFilterType = context.SessionInfo.IdPage;
            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;


            // Configura textos a mostar
            this.Master.ucMainFilter.codeLabel = lblFilterCode.Text;
            this.Master.ucMainFilter.descriptionLabel = lblFilterDescription.Text;

            //Cargo el Objeto Filtro antes de inicializar
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeFilterItem()
        {
            ucFilterItem.Initialize();
            ucFilterItem.BtnSearchClick += new EventHandler(btnSearchItem_Click);

            ucFilterItem.FilterCode = this.lblFilterCode.Text;
            ucFilterItem.FilterDescription = this.lblFilterName.Text;
        }

        private void InitializeFilterLocation()
        {
            ucFilterLocation.Initialize();
            ucFilterLocation.BtnSearchClick += new EventHandler(btnSearchLocation_Click);

            ucFilterLocation.FilterCode = this.lblFilterCodeLoc.Text;
            ucFilterLocation.FilterDescription = this.lblFilterNameLoc.Text;
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);
            Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnAddVisible = true;
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

        private void PopulateLists()
        {
            //Carga lista de warehouses
            base.LoadWarehousesLessFilter(this.ddlWarehouse, true, this.Master.EmptyRowText, "-1");

            //Carga lista de Owner
            base.LoadUserOwners(this.ddlIdOwner, this.Master.EmptyRowText, "-1", true, lblNullOwnerRow.Text, false);
        }

        private void PopulateGrid()
        {
            try
            {
                grdMgr.PageIndex = currentPage;

                // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
                if (!itemLocationViewDTO.hasConfigurationError() && itemLocationViewDTO.Configuration != null && itemLocationViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdMgr, itemLocationViewDTO.Configuration);

                grdMgr.DataSource = itemLocationViewDTO.Entities;
                grdMgr.DataBind();

                if (divGrid.Visible)
                    ucStatus.ShowRecordInfo(itemLocationViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
                else
                    ucStatus.HideRecordInfo();

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentIndex = -1;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
            divUpNew.Visible = false;
            divGrid.Visible = true;
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            if (itemLocationViewDTO.Configuration != null && itemLocationViewDTO.Configuration.Count > 0)
            {
                //Configura PopUp
                base.ConfigureModal(itemLocationViewDTO.Configuration, false);
            }

            if (ddlIdOwner.Items.Count <= 1)
            {
                base.LoadUserOwners(this.ddlIdOwner, this.Master.EmptyRowText, "-1", false, lblNullOwnerRow.Text, false);
            }
            if (ddlWarehouse.Items.Count <= 1)
            {
                //Carga Lista de Centros pero sin filtros
                base.LoadWarehousesLessFilter(this.ddlWarehouse, true, this.Master.EmptyRowText, "-1");
            }

            // Editar Max y min
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditIdLoc.Value = itemLocationViewDTO.Entities[index].Location.IdCode;
                hidEditIdItem.Value = itemLocationViewDTO.Entities[index].Item.Id.ToString();

                //Carga controles
                this.txtItemCode.Text = itemLocationViewDTO.Entities[index].Item.Code;
                this.txtLocCode.Text = itemLocationViewDTO.Entities[index].Location.Code;
                this.txtReOrderPoint.Text = itemLocationViewDTO.Entities[index].ReOrderPoint.ToString();
                this.txtReOrderQty.Text = itemLocationViewDTO.Entities[index].ReOrderQty.ToString();
                this.txtItemName.Text = itemLocationViewDTO.Entities[index].Item.LongName;
                
                //Mustra el label y el textbox del Nombre del Item
                this.txtItemName.Visible = true;
                //this.lblItem.Visible = true;

                this.txtItemCode.Enabled = false;
                this.txtLocCode.Enabled = false;
                this.txtItemName.Enabled = false;

                //Lista de Owners
                this.ddlIdOwner.SelectedValue = itemLocationViewDTO.Entities[index].Owner.Id.ToString();
                this.ddlIdOwner.Enabled = false;

                this.lblNew.Visible = false;
                this.lblEdit.Visible = true;
            }

            // Nueva usuario
            if (mode == CRUD.Create)
            {
                hidEditIdItem.Value = "0";
                hidEditIdLoc.Value = "0";
                this.txtItemCode.Text = string.Empty;
                this.txtLocCode.Text = string.Empty;
                this.txtItemName.Text = string.Empty;
                this.txtReOrderPoint.Text = string.Empty;
                this.txtReOrderQty.Text = string.Empty;

                if (this.ddlIdOwner.Items.Count == 2)
                {
                    this.ddlIdOwner.SelectedIndex = 1;
                }
                //this.ddlIdOwner.SelectedValue = "-1";

                //Oculta el label y el texbox del Nombre del Item
                //this.txtItemName.Visible = false;
                //this.lblItem.Visible = false;
                this.txtItemName.BackColor = System.Drawing.Color.Lavender;

                this.txtItemCode.Enabled = true;
                this.txtLocCode.Enabled = true;
                this.txtItemName.Enabled = false;
                this.ddlIdOwner.Enabled = true;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (itemLocationViewDTO.Configuration != null && itemLocationViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(itemLocationViewDTO.Configuration, true);
                else
                    base.ConfigureModal(itemLocationViewDTO.Configuration, false);
            }

            this.ddlWarehouse.SelectedValue = this.Master.ucMainFilter.idWhs.ToString();
            divUpNew.Visible = true;
            divGrid.Visible = false;
            //divEditNew.Visible = true;
            //modalPopUp.Show();
        }

        /// <summary>
        /// Persiste los cambios en la entidad (modo Edit o New). 
        /// </summary>
        protected void SaveChanges()
        {

                //agrega los datos del usuario
                ItemLocation itemLocation = new ItemLocation();
                itemLocation.Item = new Item();
                itemLocation.Location = new Location();
                itemLocation.Owner = new Owner();
                itemLocation.Warehouse = new Warehouse();

                itemLocation.Item.Id = Convert.ToInt32(hidEditIdItem.Value);
                itemLocation.Item.Code = this.txtItemCode.Text;
                itemLocation.Location.IdCode = hidEditIdLoc.Value;
                itemLocation.Location.Code = this.txtLocCode.Text;
                itemLocation.Owner.Id = Convert.ToInt32(this.ddlIdOwner.SelectedValue.ToString());
                itemLocation.Warehouse.Id = Convert.ToInt32(this.ddlWarehouse.SelectedValue.ToString());
                itemLocation.ReOrderPoint = Convert.ToDecimal(this.txtReOrderPoint.Text);
                itemLocation.ReOrderQty = Convert.ToDecimal(this.txtReOrderQty.Text);

                currentIdOwn = itemLocation.Owner.Id;
                currentIdWhs = itemLocation.Warehouse.Id;

                //Nuevo Usuario
                if (hidEditIdItem.Value == "0" && hidEditIdLoc.Value == "0")
                {
                    itemLocationViewDTO = iWarehousingMGR.MaintainItemLocation(CRUD.Create, itemLocation, context);
                }
                // Editar Usuario
                else
                {
                    itemLocationViewDTO = iWarehousingMGR.MaintainItemLocation(CRUD.Update, itemLocation, context);
                }

                if (itemLocationViewDTO.hasError())
                {
                    UpdateSession(true);
                    divUpNew.Visible = true;
                    //divEditNew.Visible = true;
                    //modalPopUp.Show();

                    if (this.ddlIdOwner.SelectedValue == "-1")
                        this.ddlIdOwner.SelectedValue = currentIdOwn.ToString();

                    if (this.ddlWarehouse.SelectedValue == "-1")
                        this.ddlWarehouse.SelectedValue = currentIdWhs.ToString();
                }
                else
                {
                    divUpNew.Visible = false;
                    divGrid.Visible = true;
                    //divEditNew.Visible = false;
                    //modalPopUp.Hide();
                    crud = true;
                    ucStatus.ShowMessage(itemLocationViewDTO.MessageStatus.Message);
                    UpdateSession(false);
                }

        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            itemLocationViewDTO = iWarehousingMGR.MaintainItemLocation(CRUD.Delete, itemLocationViewDTO.Entities[index], context);

            if (itemLocationViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(itemLocationViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        private void ClearGridItem()
        {
            Session.Remove(WMSTekSessions.Shared.ItemList);
            grdSearchItems.DataSource = null;
            grdSearchItems.DataBind();
        }

        private void SearchItem()
        {
            itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwnerFilter(ucFilterItem.FilterItems, context, int.Parse(this.ddlIdOwner.SelectedValue), true);
            Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);
            InitializeGrid();

            grdSearchItems.PageIndex = 0;
            grdSearchItems.DataSource = itemSearchViewDTO.Entities;
            grdSearchItems.DataBind();
        }

        private void SearchLocation()
        {
            //locationSearchViewDTO = iLayoutMGR.GetLocationsByWhsAndType(int.Parse(this.ddlWarehouse.SelectedValue), "PICK", context);
            locationSearchViewDTO = iLayoutMGR.GetLocationByCodeAndOwnerFilterWithOutStock(ucFilterLocation.FilterItems, context, int.Parse(this.ddlWarehouse.SelectedValue), true);

            Session.Add(WMSTekSessions.Shared.LocationList, locationSearchViewDTO);
            InitializeGrid();

            grdSearchLocations.PageIndex = 0;
            grdSearchLocations.DataSource = locationSearchViewDTO.Entities;
            grdSearchLocations.DataBind();
        }

        private void ClearGridLocation()
        {
            Session.Remove(WMSTekSessions.Shared.LocationList);
            grdSearchLocations.DataSource = null;
            grdSearchLocations.DataBind();
        }

        private void InitializePageCountItems()
        {
            if (grdSearchItems.PageCount > 1)
            {
                int pageNumber;

                divPageGrdSearchItems.Visible = true;
                // Paginador
                ddlPagesSearchItems.Items.Clear();
                for (int i = 0; i < grdSearchItems.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPageItems) lstItem.Selected = true;

                    ddlPagesSearchItems.Items.Add(lstItem);
                }
                this.lblPageCountSearchItems.Text = grdSearchItems.PageCount.ToString();

                ShowItemsButtonsPage();
            }
            else
            {
                divPageGrdSearchItems.Visible = false;
            }
        }

        private void InitializePageCountLocations()
        {
            if (grdSearchLocations.PageCount > 1)
            {
                int pageNumber;

                divPageGrdSearchLocations.Visible = true;
                // Paginador
                ddlPagesSearchLocations.Items.Clear();
                for (int i = 0; i < grdSearchLocations.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPageItems) lstItem.Selected = true;

                    ddlPagesSearchLocations.Items.Add(lstItem);
                }
                this.lblPageCountSearchLocations.Text = grdSearchLocations.PageCount.ToString();

                ShowLocationsButtonsPage();
            }
            else
            {
                divPageGrdSearchLocations.Visible = false;
            }
        }

        #endregion      
    }
}
