using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.Web.Services;
//using DocumentFormat.OpenXml.Drawing.Charts;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using System.Linq;
using System.Threading;

namespace Binaria.WMSTek.WebClient.Inbound.Administration
{
    public partial class InboundOrderMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<InboundOrder> inboundOrderViewDTO = new GenericViewDTO<InboundOrder>();
        private GenericViewDTO<InboundDetail> inboundDetailViewDTO = new GenericViewDTO<InboundDetail>();
        private List<InboundDetail> inboundDetails = new List<InboundDetail>();
        //private GenericViewDTO<Vendor> vendorViewDTO;
        private GenericViewDTO<Item> itemSearchViewDTO;
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

        // Propiedad para controlar el nro de pagina activa en la grilla Item
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        // Carga la grilla, filtrada por el criterio de busqueda ingresado
        protected void btnSearchItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    SearchItem();

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        divLookupItem.Visible = true;
                        mpLookupItem.Show();

                        InitializePageCountItems();
                    }
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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

        protected void btnLastGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = grdSearchItems.PageCount - 1;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);

        }

        protected void btnNextGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems < grdSearchItems.PageCount)
            {
                ddlPagesSearchItems.SelectedIndex = currentPageItems + 1; ;
                ddlPagesSearchItemsSelectedIndexChanged(sender, e);

            }
        }

        /// <summary>
        /// Agrega el item ingresado, o abre una lista de selección si no se ingresó ninguno o el item ingresado no es válido
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgBtnSearchItem_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    rvQty.Enabled = true;
                    bool validItem = false;
                    bool existingItem = false;
                    pnlError.Visible = false;

                    // Busca en base de datos el Item ingresado 
                    if (txtCode.Text.Trim() != string.Empty)
                    {
                        itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, txtCode.Text.Trim(), Convert.ToInt16(ddlOwner.SelectedValue), false);

                        // Si el codigo de Item ingresado es válido, lo carga directamente
                        if (itemSearchViewDTO.Entities != null && itemSearchViewDTO.Entities.Count == 1)
                        {
                            validItem = true;
                            Item item = new Item(itemSearchViewDTO.Entities[0].Id);

                            item.Description = itemSearchViewDTO.Entities[0].Description;
                            item.Code = itemSearchViewDTO.Entities[0].Code;

                            // Mantiene en memoria los datos del Item a agregar
                            Session.Add("InboundMgrNewItem", item);

                            // Recorre los items ya agregados y compara con el que se quiere agregar
                            //if (inboundDetails != null && inboundDetails.Count > 0)
                            //{
                            //    foreach (InboundDetail inboundDetail in inboundDetails)
                            //    {
                            //        // Si ya existe en la lista se marca
                            //        if (inboundDetail.Item.Code == item.Code)
                            //        {
                            //            existingItem = true;
                            //            pnlError.Visible = false;
                            //        }
                            //    }
                            //}

                            // Si no fue agregado, agrega el item 
                            if (!existingItem)
                            {
                                this.txtCode.Text = item.Code;
                                this.txtDescription.Text = item.Description;
                                hidItemId.Value = item.Id.ToString();
                            }
                            else
                            {
                                //ClientScript.RegisterClientScriptBlock(typeof(OutboundOrderMgr), "ExistingItem", "Alert('Item existente')");
                                pnlError.Visible = true;
                            }
                        }
                    }

                    // Si no es válido o no se ingresó, se muestra la lista de Items para seleccionar uno
                    if (!validItem)
                    {
                        ucFilterItem.Clear();
                        ucFilterItem.Initialize();

                        // Setea el filtro con el Item ingresado
                        if (txtCode.Text.Trim() != string.Empty)
                        {
                            FilterItem filterItem = new FilterItem("%" + txtCode.Text + "%");
                            filterItem.Selected = true;
                            ucFilterItem.FilterItems[0] = filterItem;

                            ucFilterItem.LoadCurrentFilter(ucFilterItem.FilterItems);
                            SearchItem();
                        }
                        // Si no se ingresó ningún item, no se ejecuta la búsqueda
                        else
                            ClearGridItem();

                        // Esto evita un bug de ajax
                        valEditNew.Enabled = false;
                        valAddItem.Enabled = false;
                        valSearchItem.Enabled = false;

                        divLookupItem.Visible = true;
                        mpLookupItem.Show();

                        InitializePageCountItems();
                    }
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        //protected void btnSearchVendor_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        SearchVendor();

        //        if (isValidViewDTO)
        //        {
        //            mpeModalPopUpVendor.Show();
        //            this.Master.ucError.ClearError();
        //        }
        //    }
        //  catch (Exception ex)
        //  {
        //    inboundOrderViewDTO.Errors = baseControl.handleException(this.GetType().FullName, "btnSearchVendor_Click: ", ex, context);
        //    this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
        //  }
        //}

        //private void SearchVendor()
        //{
        //        base.ConfigureMgrFilter(ucFilterVendor.FilterItems);
        //        vendorViewDTO = iWarehousingMGR.FindAllvendor(context);
        //        Session.Add(Constants.SessionDtoVendorList, vendorViewDTO);
        //        grdVendor.DataSource = vendorViewDTO.Entities;
        //        grdVendor.DataBind();
        //}

        protected void btnCloseItem_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    divLookupItem.Visible = false;
                    mpLookupItem.Hide();

                    // Esto evita un bug de ajax
                    valEditNew.Enabled = true;
                    valAddItem.Enabled = true;
                    valSearchItem.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        protected void btnCloseOrder_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    CloseOrder();
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
        //        inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
        //    }
        //}

        //protected void imgbtnSearchVendor_Click(object sender, ImageClickEventArgs e)
        //{
        //    pnlPanelPoUp.Visible = true;
        //    mpeModalPopUpVendor.Show();
        //}

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;
                    ImageButton btnEdit = e.Row.FindControl("btnEdit") as ImageButton;
                    ImageButton btnCloseOrder = e.Row.FindControl("btnClose") as ImageButton;

                    if (btnDelete != null)
                    {
                        btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";
                    }

                    // Deshabilita la opcion de Editar y Eliminar si la Orden esta en estado distinto a 'Anunciada'
                    if (btnDelete != null && inboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestInboundTrack.Type.Id > (int)TrackInboundTypeName.Anunciado)
                    {
                        btnDelete.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_delete_dis.png";
                        btnDelete.Enabled = false;
                    }

                    if (btnEdit != null && inboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestInboundTrack.Type.Id > (int)TrackInboundTypeName.Anunciado)
                    {
                        btnEdit.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_edit_dis.png";
                        btnEdit.Enabled = false;
                    }

                    // Deshabilita la opcion de 'Cerrar Orden' si la Orden ya está Cerrada
                    if (btnCloseOrder != null && (inboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestInboundTrack.Type.Id == (int)TrackInboundTypeName.CerradaCompleta) || (inboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestInboundTrack.Type.Id == (int)TrackInboundTypeName.CerradaIncompleta))
                    {
                        btnCloseOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_close_dis.png";
                        btnCloseOrder.Enabled = false;
                    }
                    /*
                else
                {
                    btnCloseOrder.CommandArgument = e.Row.DataItemIndex.ToString();                    
                }
                */
                    // Agrega atributos para cambiar el color de la fila seleccionada

                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    //e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex.ToString()));

                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }


        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "CloseOrder")
                {
                    int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);
                    ShowModalCloseOrder(index);
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (inboundDetails != null && inboundDetails.Count > 0)
                {
                    
                    var lstIdIndoundType = GetConst("IdInboundTypeValidateOutboundOrder");
                    string indboundType = this.ddlIdInboundType.SelectedItem.Value.ToUpper().Trim();

                    // Valida variable de sesion del Usuario Loggeado
                    if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                    {

                        //if (ValidateDates())
                        //{

                        //}

                        if (lstIdIndoundType.Contains(indboundType))
                        {
                            if (ValidateDocument())
                            {
                                SaveChanges();
                            }
                        }
                        else
                        {
                            SaveChanges();
                        }


                    }
                }
                else
                {
                    inboundAviso.Visible = true;
                }

            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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

        protected void imgCloseNewEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    divGrid.Visible = true;
                    divModal.Visible = false;
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        protected void btnCloseNewEdit_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    divGrid.Visible = true;
                    divModal.Visible = false;
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        protected void grdItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                RemoveItem(e.RowIndex);
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        protected void grdItems_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                EditItem(e.NewEditIndex);
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        //protected void grdVendor_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    int editIndex = (Convert.ToInt32(grdVendor.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

        //    if (ValidateSession(Constants.SessionDtoVendorList))
        //    {
        //        vendorViewDTO = (GenericViewDTO<Vendor>)Session[Constants.SessionDtoVendorList];
        //    }

        //    foreach (Vendor vendor in vendorViewDTO.Entities)
        //    {
        //        if (vendor.Id == editIndex)
        //        {
        //            this.txtVendorCode.Text = vendor.VendorCode;
        //            HidenCodVendor.Value = vendor.Id.ToString();
        //        }
        //    }
        //    mpeModalPopUp.Show();
        //}

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
                            this.txtCode.Text = item.Code;
                            this.txtDescription.Text = item.Description;
                            hidItemId.Value = item.Id.ToString();
                            Session.Add("InboundMgrNewItem", item);

                            // Esto evita un bug de ajax
                            valEditNew.Enabled = true;
                            valAddItem.Enabled = true;
                            valSearchItem.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        protected void ddlOwner_Changed(object sender, EventArgs e)
        {
            try
            {
                base.LoadCategoryItemByOwner(this.ddlCategoryItem, Convert.ToInt32(ddlOwner.SelectedValue), true, true, this.Master.EmptyRowText);
                base.LoadVendorByOwner(this.ddlVendor, Convert.ToInt32(ddlOwner.SelectedValue), this.Master.EmptyRowText);
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        protected void ddlInboundType_Changed(object sender, EventArgs e)
        {
            try
            {
                string indboundType = this.ddlIdInboundType.SelectedItem.Value.ToUpper().Trim();
                var lstIdIndoundType = GetConst("IdInboundTypeRequeriedOutbound");
                var lstEnableIndoundType = GetConst("IdInboundTypeEnabledOutbound");


                //Valida el tipo de orden, para ver si requiere documento de salida o nop
                if (lstIdIndoundType.Contains(indboundType))
                {
                    this.rfvIdOutboundOrderSource.Enabled = true;
                }
                else
                {
                    this.rfvIdOutboundOrderSource.Enabled = false;
                }

                //Valida el tipo de orden, para ver si se desabilita el documento de salida o nop
                if (lstEnableIndoundType.Contains(indboundType))
                {
                    this.txtIdOutboundOrderSource.Enabled = true;
                }
                else
                {
                    this.txtIdOutboundOrderSource.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        protected void txtIdOutboundOrderSource_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ValidateDocument();
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }


        protected bool ValidateDocument()
        {
            bool resultado = false;
            string outboundNum = this.txtIdOutboundOrderSource.Text.Trim();
            int own = int.Parse(this.ddlOwner.SelectedValue);
            int whs = int.Parse(this.ddlWarehouse.SelectedValue);

            OutboundOrder order = new OutboundOrder();
            order.Number = outboundNum;
            order.Owner = new Owner();
            order.Owner.Id = own;
            order.Warehouse = new Warehouse();
            order.Warehouse.Id = whs;

            //Busca el Numero de Documento por Owner y Warehose
            GenericViewDTO<OutboundOrder> outboundViewDto = iDispatchingMGR.GetOutboundByNumberAndOwnerWhs(context, order);

            if (outboundViewDto.Errors != null)
            {
                this.Master.ucError.ShowError(outboundViewDto.Errors);
                resultado = false;
            }
            else
            {
                //Pregunta si existe el Numero de Documento
                if (outboundViewDto.Entities == null || outboundViewDto.Entities.Count < 1)
                {
                    resultado = false;
                    this.rfvIdOutboundOrderSource.ErrorMessage = this.lblValDocExists.Text;
                    this.rfvIdOutboundOrderSource.Visible = true;
                    this.rfvIdOutboundOrderSource.Enabled = true;
                    this.rfvIdOutboundOrderSource.IsValid = false;
                }
                else
                {
                    resultado = true;
                }

            }
            return resultado;
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

        /// <summary>
        /// Agrega el Item al detalle de la Orden
        /// </summary>
        protected void imgBtnAddItem_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                bool addItem = false;
                inboundAviso.Visible = false;

                // Recupera el Item a agregar
                Item newItem = (Item)Session["InboundMgrNewItem"];

                // Crea el nuevo detalle de la Orden
                InboundDetail newDetail = new InboundDetail();

                newDetail.Item = newItem;
                newDetail.Item.Id = Convert.ToInt32(hidItemId.Value);

                if (MiscUtils.IsNumeric(txtQty.Text))
                    newDetail.ItemQty = Convert.ToDecimal(txtQty.Text);
                else
                    newDetail.ItemQty = 0;

                newDetail.Status = true;
                newDetail.Price = 0;
                newDetail.InboundOrder = new InboundOrder();
                newDetail.InboundOrder.Id = Convert.ToInt32(hidEditId.Value);

                if (ddlCategoryItem.SelectedValue != null && ddlCategoryItem.Visible && ddlCategoryItem.SelectedValue != "-1")
                {
                    newDetail.CategoryItem = new CategoryItem(Convert.ToInt16(ddlCategoryItem.SelectedValue));
                    newDetail.CategoryItem.Name = ddlCategoryItem.SelectedItem.Text;
                }
                else
                {
                    newDetail.CategoryItem = new CategoryItem(-1);
                }

                if (!string.IsNullOrEmpty(txtLotItem.Text))
                    newDetail.LotNumber = txtLotItem.Text.Trim();

                if (!string.IsNullOrEmpty(txtFabricationDateItem.Text))
                {
                    DateTime outputFabricationDate;

                    if (DateTime.TryParseExact(txtFabricationDateItem.Text.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out outputFabricationDate))
                        newDetail.FabricationDate = outputFabricationDate;
                }

                if (!string.IsNullOrEmpty(txtExpirationDateItem.Text))
                {
                    DateTime outputExpirationDate;

                    if (DateTime.TryParseExact(txtExpirationDateItem.Text.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out outputExpirationDate))
                        newDetail.ExpirationDate = outputExpirationDate;
                }

                if (txtLpn.Text.Trim() != "")
                {
                    newDetail.LpnCode = txtLpn.Text.Trim();
                }

                if (MiscUtils.IsNumeric(txtWeight.Text))
                    newDetail.Weight = Convert.ToDecimal(txtWeight.Text);


                //Se Elimina Categoria 19-03-2015
                //else
                //    newDetail.CategoryItem = new CategoryItem(1);


                // Si ya existen detalles, recorre el los Items existentes y compara con el que se quiere agregar
                if (inboundDetails != null && inboundDetails.Count > 0)
                {
                    foreach (InboundDetail inboundDetail in inboundDetails)
                    {
                        // Si ya existe en el detalle, se avisa
                        if ((inboundDetail.Item.Id == newDetail.Item.Id) && (inboundDetail.CategoryItem.Id == newDetail.CategoryItem.Id)
                            && (inboundDetail.LotNumber == newDetail.LotNumber) && (inboundDetail.FabricationDate == newDetail.FabricationDate)
                            && (inboundDetail.ExpirationDate == newDetail.ExpirationDate) && (inboundDetail.LpnCode == newDetail.LpnCode))
                        {
                            pnlError.Visible = true;
                            addItem = false;
                            break;
                        }
                        else
                        {
                            pnlError.Visible = false;
                            addItem = true;
                        }
                    }
                }
                // Si es el primer Item del documento, lo agrega
                else
                {
                    inboundDetails = new List<InboundDetail>();
                    inboundDetails.Add(newDetail);
                }

                if (addItem) inboundDetails.Add(newDetail);

                Session.Add(WMSTekSessions.InboundOrderMgr.InboundDetailList, inboundDetails);

                // Limpia panel Nuevo Item
                this.txtCode.Text = string.Empty;
                this.txtDescription.Text = string.Empty;
                this.txtQty.Text = string.Empty;
                this.txtLotItem.Text = string.Empty;
                this.txtFabricationDateItem.Text = string.Empty;
                this.txtExpirationDate.Text = string.Empty;
                this.ddlCategoryItem.ClearSelection();
                this.txtLpn.Text = string.Empty;
                this.rvQty.Enabled = false;
                this.txtWeight.Text = string.Empty;

                // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
                //if (!inboundOrderViewDTO.hasConfigurationError() && inboundOrderViewDTO.Configuration != null && inboundOrderViewDTO.Configuration.Count > 0)
                //    base.ConfigureGridByProperties(this.grdItems, inboundOrderViewDTO.Configuration);

                this.grdItems.DataSource = inboundDetails;
                this.grdItems.DataBind();
                this.grdItems.Visible = true;

                divCategoryItem.Visible = true;
                txtCode.Enabled = true;
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        protected void grdSearchItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "InboundOrderMgr";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeFilterItem();
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                PopulateLists();
            }
            else
            {

                if (ValidateSession(WMSTekSessions.InboundOrderMgr.InboundOrderList))
                {
                    inboundOrderViewDTO = (GenericViewDTO<InboundOrder>)Session[WMSTekSessions.InboundOrderMgr.InboundOrderList];
                    isValidViewDTO = true;
                }

                pnlError.Visible = false;

                //Carga la lista detalle desde la session
                if (ValidateSession(WMSTekSessions.InboundOrderMgr.InboundDetailList))
                {
                    inboundDetails = (List<InboundDetail>)Session[WMSTekSessions.InboundOrderMgr.InboundDetailList];
                }

                // Si es un ViewDTO valido, carga la grilla
                if (isValidViewDTO)
                {
                    //Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                    PopulateGrid();
                }
            }

            pnlError.Visible = false;
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {

            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.SaveOnIndexChanged = true;
            this.Master.ucMainFilter.chkFilterDateFromChecked = true;
            this.Master.ucMainFilter.chkFilterDateToChecked = true;
            this.Master.ucMainFilter.trackInboundTypeVisible = true;
            this.Master.ucMainFilter.inboundTypeVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.InboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.InboundDaysAfter;

            //Setea los filtros que no necesitan ser auto postback
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;
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
            this.Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);
            this.Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);

            this.Master.ucTaskBar.btnAddVisible = true;
            this.Master.ucTaskBar.btnNewVisible = true;
            this.Master.ucTaskBar.btnRefreshVisible = true;
        }



        //private void InitializeFilterVendor()
        //{
        //    ucFilterVendor.Initialize();
        //    ucFilterVendor.BtnSearchClick += new EventHandler(btnSearchVendor_Click);

        //    ucFilterVendor.FilterCode = this.lblFilterRut.Text;
        //    ucFilterVendor.FilterDescription = this.lblFilterNombre.Text;
        //}

        private void InitializeFilterItem()
        {
            ucFilterItem.Initialize();
            ucFilterItem.BtnSearchClick += new EventHandler(btnSearchItem_Click);

            ucFilterItem.FilterCode = this.lblFilterCode.Text;
            ucFilterItem.FilterDescription = this.lblFilterName.Text;
        }

        private void InitializeGridItems()
        {
            grdSearchItems.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchItems.EmptyDataText = this.Master.EmptyGridText;
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
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false); ;
            base.LoadUserWarehouses(this.ddlWarehouse, this.Master.EmptyRowText, "-1", true);
            //base.LoadInboundType(this.ddlIdInboundType, true, this.Master.EmptyRowText);
            LoadInboundTypeMgr();
        }

        private void LoadInboundTypeMgr()
        {
            var lstTypeInbound = GetConst("TypeOfInboundMgr");
            GenericViewDTO<InboundType> inboundTypeViewDTO = new GenericViewDTO<InboundType>();

            inboundTypeViewDTO = iReceptionMGR.FindAllInboundType(context);

            if (inboundTypeViewDTO.hasError())
            {
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
            else
            {
                List<InboundType> auxListInboundType = new List<InboundType>();

                foreach (InboundType item in inboundTypeViewDTO.Entities)
                {
                    if (lstTypeInbound.Contains(item.Code.Trim()))
                    {
                        auxListInboundType.Add(item);
                    }
                }

                this.ddlIdInboundType.DataSource = auxListInboundType;
                this.ddlIdInboundType.DataTextField = "Name";
                this.ddlIdInboundType.DataValueField = "Id";
                this.ddlIdInboundType.DataBind();

                // TODO: parametrizar "seleccione...
                this.ddlIdInboundType.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
                this.ddlIdInboundType.Items[0].Selected = true;
            }

        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
                inboundOrderViewDTO.ClearError();
            }

            this.Master.ucMainFilter.ClearFilterObject();
            this.Master.ucMainFilter.LoadControlValuesToFilterObject();

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Carga lista de InboundOrders
            inboundOrderViewDTO = iReceptionMGR.FindAllInboundOrder(context);

            if (!inboundOrderViewDTO.hasError() && inboundOrderViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InboundOrderMgr.InboundOrderList, inboundOrderViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(inboundOrderViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga los datos en los controles textBox de la pagina
        /// </summary>
        /// <param name="index"></param>
        //private void PopulateGridVendor()
        //{
        //}

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!inboundOrderViewDTO.hasConfigurationError() && inboundOrderViewDTO.Configuration != null && inboundOrderViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, inboundOrderViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = inboundOrderViewDTO.Entities;
            grdMgr.DataBind();


            if (divGrid.Visible)
                ucStatus.ShowRecordInfo(inboundOrderViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
            else
                ucStatus.HideRecordInfo();
        }

        protected void ReloadData()
        {
            divGrid.Visible = true;
            divModal.Visible = false;

            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        protected void RemoveItem(int itemIndex)
        {
            // Quita el item de la lista
            inboundDetails.RemoveAt(itemIndex);

            // Carga nuevamente el gridview.
            grdItems.DataSource = inboundDetails;
            grdItems.DataBind();
            txtCode.Enabled = true;
        }

        /// <summary>
        /// Pasa el item seleccionado al panel de edición / Solo es posible editar Cantidad y Categoría
        /// </summary>
        protected void EditItem(int itemIndex)
        {
            rvQty.Enabled = true;
            var inboundDetailSelected = inboundDetails[itemIndex];
            this.txtCode.Text = inboundDetails[itemIndex].Item.Code;
            this.txtCode.Enabled = false;
            this.txtDescription.Text = inboundDetails[itemIndex].Item.Description;
            this.txtQty.Text = inboundDetails[itemIndex].ItemQty.ToString();
            this.hidItemId.Value = inboundDetails[itemIndex].Item.Id.ToString();
            this.txtLotItem.Text = string.IsNullOrEmpty(inboundDetailSelected.LotNumber) ? string.Empty : inboundDetailSelected.LotNumber;
            this.txtFabricationDateItem.Text = inboundDetailSelected.FabricationDate > DateTime.MinValue ? inboundDetailSelected.FabricationDate.ToString("dd/MM/yyyy") : string.Empty;
            this.txtExpirationDateItem.Text = inboundDetailSelected.ExpirationDate > DateTime.MinValue ? inboundDetailSelected.ExpirationDate.ToString("dd/MM/yyyy") : string.Empty;
            this.txtLpn.Text = string.IsNullOrEmpty(inboundDetailSelected.LpnCode) ? "" : inboundDetailSelected.LpnCode.Trim();
            this.txtWeight.Text = inboundDetails[itemIndex].Weight > 0 ? inboundDetails[itemIndex].Weight.ToString() : "0";

            if (inboundDetailSelected.CategoryItem.Id > 0)
            {
                this.ddlCategoryItem.SelectedValue = inboundDetailSelected.CategoryItem.Id.ToString();
            }
            else
            {
                this.ddlCategoryItem.SelectedIndex = 0;
            }

            // Mantiene en memoria los datos del Item a agregar
            Session.Add("InboundMgrNewItem", inboundDetails[itemIndex].Item);

            // Quita el item de la lista 
            inboundDetails.RemoveAt(itemIndex);

            // Carga nuevamente el gridview.
            grdItems.DataSource = inboundDetails;
            grdItems.DataBind();
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            //eliminar la orden y sus detalles
            inboundOrderViewDTO = iReceptionMGR.MaintainInboundOrder(CRUD.Delete, (inboundOrderViewDTO.Entities[index]), context);

            if (inboundOrderViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(inboundOrderViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
            //ucFilterVendor.Clear();
        }

        private void SearchItem()
        {
            itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwnerFilter(ucFilterItem.FilterItems, context, Convert.ToInt16(ddlOwner.SelectedValue), true);
            Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);
            InitializeGridItems();
            grdSearchItems.DataSource = itemSearchViewDTO.Entities;
            grdSearchItems.DataBind();
        }

        private void ClearGridItem()
        {
            Session.Remove(WMSTekSessions.Shared.ItemList);
            grdSearchItems.DataSource = null;
            grdSearchItems.DataBind();
        }

        /// <summary>
        /// Carga detalles de la Orden
        /// </summary>
        protected void LoadinboundOrdersDetail(int index)
        {
            int id = inboundOrderViewDTO.Entities[index].Id;

            inboundDetailViewDTO = iReceptionMGR.LoadDetailByIdInbound(context, id);
            inboundDetails = inboundDetailViewDTO.Entities;

            Session.Add(WMSTekSessions.InboundOrderMgr.InboundDetailList, inboundDetails);

            if (inboundDetails != null && inboundDetails.Count > 0)
            {
                //// Configura ORDEN y VISIBILIDAD de las columnas de la grilla
                //if (!inboundOrderViewDTO.hasConfigurationError() && inboundOrderViewDTO.Configuration != null && inboundOrderViewDTO.Configuration.Count > 0)
                //    base.ConfigureGridByProperties(this.grdItems, inboundDetailViewDTO.Configuration);

                grdItems.DataSource = inboundDetails;
                grdItems.DataBind();
                grdItems.Visible = true;
            }
            else
            {
                grdItems.Visible = false;
            }
        }

        /// <summary>
        /// Muestra ventana modal para cerrar la Orden seleccionada
        /// </summary>
        /// <param name="index"></param>
        protected void ShowModalCloseOrder(int index)
        {
            List<string> lstTrackInbound = GetConst("TrackCloseTrackInboundType");

            //base.LoadTrackCloseInbound(this.ddlTrackInbound, inboundOrderViewDTO.Entities[index].Id, true, this.Master.EmptyRowText);
            base.LoadTrackInbound(this.ddlTrackInbound, true, this.Master.EmptyRowText, lstTrackInbound.ToArray());

            if (inboundOrderViewDTO.Entities[index].Warehouse != null) this.lblWarehouse2.Text = inboundOrderViewDTO.Entities[index].Warehouse.Name;
            if (inboundOrderViewDTO.Entities[index].InboundType != null) this.lblInboundType2.Text = inboundOrderViewDTO.Entities[index].InboundType.Code;
            if (inboundOrderViewDTO.Entities[index].Number != null) this.lblNroDoc2.Text = inboundOrderViewDTO.Entities[index].Number;
            if (inboundOrderViewDTO.Entities[index].Vendor != null) this.lblVendor2.Text = inboundOrderViewDTO.Entities[index].Vendor.Name;

            hidEditId.Value = inboundOrderViewDTO.Entities[index].Id.ToString();

            divCloseOrder.Visible = true;
            mpCloseOrder.Show();
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Editar Documento
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = inboundOrderViewDTO.Entities[index].Id.ToString();
                hidLatestInboundTrackId.Value = inboundOrderViewDTO.Entities[index].LatestInboundTrack.Type.Id.ToString();

                this.divNameTrackInbound.Visible = true;
                this.txtNumInboundOrder.Text = string.Empty;
                this.txtDateExpected.Text = string.Empty;
                this.txtEmissionDate.Text = string.Empty;
                this.txtExpirationDate.Text = string.Empty;
                this.txtIdOutboundOrderSource.Text = string.Empty;
                this.txtLineComment.Text = string.Empty;
                this.txtLpnInspection.Text = string.Empty;
                this.txtNumInboundOrder.Text = string.Empty;
                this.txtPercentQA.Text = string.Empty;
                this.txtShiftNumber.Text = string.Empty;
                this.txtQty.Text = string.Empty;
                this.txtDescription.Text = string.Empty;
                this.txtCode.Text = string.Empty;
                txtLotItem.Text = string.Empty;
                txtFabricationDateItem.Text = string.Empty;
                txtExpirationDateItem.Text = string.Empty;
                this.txtLpn.Text = string.Empty;
                this.txtWeight.Text = string.Empty;

                // Carga el objeto detalle
                LoadinboundOrdersDetail(index);

                if (inboundOrderViewDTO.Entities[index].Number != null) this.txtNumInboundOrder.Text = inboundOrderViewDTO.Entities[index].Number;

                if (inboundOrderViewDTO.Entities[index].DateExpected != null && inboundOrderViewDTO.Entities[index].DateExpected > DateTime.MinValue)
                    this.txtDateExpected.Text = inboundOrderViewDTO.Entities[index].DateExpected.ToString("dd-MM-yyyy");
                else
                    this.txtDateExpected.Text = string.Empty;

                if (inboundOrderViewDTO.Entities[index].EmissionDate != null && inboundOrderViewDTO.Entities[index].EmissionDate > DateTime.MinValue)
                    this.txtEmissionDate.Text = inboundOrderViewDTO.Entities[index].EmissionDate.ToString("dd-MM-yyyy");
                else
                    this.txtEmissionDate.Text = string.Empty;

                if (inboundOrderViewDTO.Entities[index].ExpirationDate != null && inboundOrderViewDTO.Entities[index].ExpirationDate > DateTime.MinValue)
                    this.txtExpirationDate.Text = inboundOrderViewDTO.Entities[index].ExpirationDate.ToString("dd-MM-yyyy");
                else
                    this.txtExpirationDate.Text = string.Empty;

                if (inboundOrderViewDTO.Entities[index].OutboundOrder.Number != null) this.txtIdOutboundOrderSource.Text = inboundOrderViewDTO.Entities[index].OutboundOrder.Number;
                if (inboundOrderViewDTO.Entities[index].Comment != null) this.txtLineComment.Text = inboundOrderViewDTO.Entities[index].Comment;
                this.txtLpnInspection.Text = inboundOrderViewDTO.Entities[index].PercentLpnInspection.ToString();
                if (inboundOrderViewDTO.Entities[index].LatestInboundTrack.Type != null) this.txtInboundTrack.Text = inboundOrderViewDTO.Entities[index].LatestInboundTrack.Type.Name;
                this.txtPercentQA.Text = inboundOrderViewDTO.Entities[index].PercentQA.ToString();
                if (inboundOrderViewDTO.Entities[index].ShiftNumber != null) this.txtShiftNumber.Text = inboundOrderViewDTO.Entities[index].ShiftNumber;
                //this.txtVendorCode.Text = inboundOrderViewDTO.Entities[index].Vendor.VendorCode;
                if (inboundOrderViewDTO.Entities[index].InboundType != null) this.ddlIdInboundType.SelectedValue = inboundOrderViewDTO.Entities[index].InboundType.Id.ToString();
                if (inboundOrderViewDTO.Entities[index].Owner != null)
                {
                    this.ddlOwner.SelectedValue = inboundOrderViewDTO.Entities[index].Owner.Id.ToString();
                    hidSelectedOwner.Value = ddlOwner.SelectedIndex.ToString();
                }

                if (inboundOrderViewDTO.Entities[index].Warehouse != null) this.ddlWarehouse.SelectedValue = inboundOrderViewDTO.Entities[index].Warehouse.Id.ToString();

                base.LoadVendorByOwner(this.ddlVendor, Convert.ToInt32(ddlOwner.SelectedValue), this.Master.EmptyRowText);
                if (inboundOrderViewDTO.Entities[index].Vendor != null) this.ddlVendor.SelectedValue = inboundOrderViewDTO.Entities[index].Vendor.Id.ToString();

                this.chkIsAsn.Checked = inboundOrderViewDTO.Entities[index].IsAsn;
                this.chkStatus.Checked = inboundOrderViewDTO.Entities[index].Status;
                chkHasIssues.Checked = inboundOrderViewDTO.Entities[index].HasIssues;

                this.lblNew.Visible = false;
                this.lblEdit.Visible = true;

                string indboundType = this.ddlIdInboundType.SelectedItem.Value.ToUpper().Trim();
                var lstIdIndoundType = GetConst("IdInboundTypeRequeriedOutbound");
                var lstEnableIndoundType = GetConst("IdInboundTypeEnabledOutbound");


                //Valida el tipo de orden, para ver si requiere documento de salida o nop
                if (lstIdIndoundType.Contains(indboundType))
                {
                    this.rfvIdOutboundOrderSource.Enabled = true;
                }
                else
                {
                    this.rfvIdOutboundOrderSource.Enabled = false;
                }

                //Valida el tipo de orden, para ver si se desabilita el documento de salida o nop
                if (lstEnableIndoundType.Contains(indboundType))
                {
                    this.txtIdOutboundOrderSource.Enabled = true;
                }
                else
                {
                    this.txtIdOutboundOrderSource.Enabled = false;
                }

                this.ddlWarehouse.Enabled = false;
                this.ddlOwner.Enabled = false;
                this.ddlIdInboundType.Enabled = true;
                this.txtNumInboundOrder.Enabled = false;
            }

            // Nuevo Documento
            if (mode == CRUD.Create)
            {
                // Limpia detalles anteriores   
                Session.Remove(WMSTekSessions.InboundOrderMgr.InboundDetailList);

                // Selecciona Warehouse y Owner seleccionados en el Filtro
                this.ddlWarehouse.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues[0].Value;
                this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

                base.LoadVendorByOwner(this.ddlVendor, Convert.ToInt32(this.ddlOwner.SelectedValue), this.Master.EmptyRowText);

                int inboundTrack = (int)TrackInboundTypeName.Anunciado;

                hidEditId.Value = "0";
                hidLatestInboundTrackId.Value = inboundTrack.ToString();

                this.divNameTrackInbound.Visible = false;

                this.txtNumInboundOrder.Text = string.Empty;
                this.txtNumInboundOrder.Enabled = true;
                this.txtNumInboundOrder.ReadOnly = false;
                this.txtDateExpected.Text = string.Empty;
                this.txtEmissionDate.Text = string.Empty;
                this.txtExpirationDate.Text = string.Empty;

                this.txtIdOutboundOrderSource.Text = string.Empty;
                this.txtIdOutboundOrderSource.Enabled = true;
                this.txtIdOutboundOrderSource.ReadOnly = false;

                this.txtLineComment.Text = string.Empty;
                this.txtLpnInspection.Text = string.Empty;
                this.txtNumInboundOrder.Text = string.Empty;
                this.txtPercentQA.Text = string.Empty;
                this.txtShiftNumber.Text = string.Empty;
                this.txtQty.Text = string.Empty;
                this.txtDescription.Text = string.Empty;
                this.txtCode.Text = string.Empty;
                //this.txtVendorCode.Text = string.Empty;
                this.ddlVendor.SelectedValue = "-1";
                this.ddlIdInboundType.SelectedValue = "-1";
                this.chkIsAsn.Checked = false;
                this.chkStatus.Checked = true;
                this.lblNew.Visible = true;
                this.lblEdit.Visible = false;
                txtLotItem.Text = string.Empty;
                txtFabricationDateItem.Text = string.Empty;
                txtExpirationDateItem.Text = string.Empty;
                chkHasIssues.Checked = false;
                this.txtLpn.Text = string.Empty;
                this.txtWeight.Text = string.Empty;

                // Reset detalle del Documento
                inboundDetails = null;
                this.grdItems.DataSource = inboundDetails;
                this.grdItems.DataBind();

                this.ddlWarehouse.Enabled = true;
                this.ddlOwner.Enabled = true;
                this.ddlIdInboundType.Enabled = true;
                this.txtNumInboundOrder.Enabled = true;
            }

            base.LoadCategoryItemByOwner(this.ddlCategoryItem, Convert.ToInt32(ddlOwner.SelectedValue), true, true, this.Master.EmptyRowText);

            if (inboundOrderViewDTO.Configuration != null && inboundOrderViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(inboundOrderViewDTO.Configuration, true);
                else
                    base.ConfigureModal(inboundOrderViewDTO.Configuration, false);
            }
            ddlCategoryItem.Enabled = true;
            divCategoryItem.Visible = true;
            // La propiedad Inbound Track es de solo lectura
            txtInboundTrack.Enabled = false;
            pnlError.Visible = false;
            divGrid.Visible = false;
            divModal.Visible = true;
            ucStatus.ShowMessage(inboundOrderViewDTO.MessageStatus.Message);

        }

        /// <summary>
        /// Cambia el estado (track) de la Orden a 'Recibido Completo', 'Completa' o 'Cerrada'
        /// </summary>
        protected void CloseOrder()
        {
            GenericViewDTO<InboundTrack> inboundTrackViewDTO = new GenericViewDTO<InboundTrack>();
            InboundOrder closeInboundOrder = new InboundOrder();

            closeInboundOrder = new InboundOrder();
            closeInboundOrder.Id = Convert.ToInt32(hidEditId.Value);

            inboundTrackViewDTO = iReceptionMGR.ChangeInboundOrderTrack(context, (TrackInboundTypeName)Convert.ToInt16(ddlTrackInbound.SelectedValue), closeInboundOrder);

            if (inboundTrackViewDTO.hasError())
            {
                inboundOrderViewDTO.Errors = inboundTrackViewDTO.Errors;
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(inboundTrackViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        protected void SaveChanges()
        {
            //asigna numeros de linea al detalle
            if (inboundDetails != null)
            {
                int k = 0;
                foreach (InboundDetail inboundDetail in inboundDetails)
                {
                    inboundDetail.LineNumber = k;
                    k++;
                }
            }

            //agrega los datos 
            InboundOrder inboundOrder = new InboundOrder();

            inboundOrder.LatestInboundTrack = new InboundTrack();
            inboundOrder.LatestInboundTrack.Type = new TrackInboundType();
            inboundOrder.LatestInboundTrack.InboundOrder = new InboundOrder();
            inboundOrder.LatestInboundTrack.DateTrack = DateTime.Now;

            inboundOrder.OutboundOrder = new OutboundOrder();
            inboundOrder.Owner = new Owner();
            inboundOrder.Warehouse = new Warehouse();
            inboundOrder.Vendor = new Vendor();

            inboundOrder.Id = Convert.ToInt32(hidEditId.Value);

            inboundOrder.LatestInboundTrack.Type.Id = Convert.ToInt32(hidLatestInboundTrackId.Value);
            inboundOrder.LatestInboundTrack.InboundOrder.Id = inboundOrder.Id;

            inboundOrder.Number = this.txtNumInboundOrder.Text;
            inboundOrder.Comment = this.txtLineComment.Text;
            inboundOrder.InboundType = new InboundType(Convert.ToInt32(this.ddlIdInboundType.SelectedValue));
            if (!string.IsNullOrEmpty(this.txtDateExpected.Text)) inboundOrder.DateExpected = Convert.ToDateTime(this.txtDateExpected.Text);
            if (!string.IsNullOrEmpty(this.txtEmissionDate.Text))
                inboundOrder.EmissionDate = Convert.ToDateTime(this.txtEmissionDate.Text);
            else
                inboundOrder.EmissionDate = DateTime.Now;
            if (!string.IsNullOrEmpty(this.txtExpirationDate.Text)) inboundOrder.ExpirationDate = Convert.ToDateTime(this.txtExpirationDate.Text);

            if (!string.IsNullOrEmpty(this.txtIdOutboundOrderSource.Text))
            {
                OutboundOrder order = new OutboundOrder();
                order.Number = this.txtIdOutboundOrderSource.Text.Trim();
                order.Owner = new Owner();
                order.Owner.Id = int.Parse(this.ddlOwner.SelectedValue);
                order.Warehouse = new Warehouse();
                order.Warehouse.Id = int.Parse(this.ddlWarehouse.SelectedValue);

                //Busca el Numero de Documento por Owner y Warehose
                GenericViewDTO<OutboundOrder> outboundViewDto = iDispatchingMGR.GetOutboundByNumberAndOwnerWhs(context, order);

                if (outboundViewDto.Entities.Count > 0)
                {
                    inboundOrder.OutboundOrder = outboundViewDto.Entities[0];
                    //inboundOrder.OutboundOrder.Number = this.txtIdOutboundOrderSource.Text;
                }
            }

            inboundOrder.IsAsn = this.chkIsAsn.Checked;
            if (!string.IsNullOrEmpty(this.txtLpnInspection.Text)) inboundOrder.PercentLpnInspection = Convert.ToInt32(this.txtLpnInspection.Text);
            if (!string.IsNullOrEmpty(this.txtPercentQA.Text)) inboundOrder.PercentQA = Convert.ToInt32(this.txtPercentQA.Text);
            inboundOrder.ShiftNumber = this.txtShiftNumber.Text;
            inboundOrder.Owner.Id = Convert.ToInt32(this.ddlOwner.SelectedValue);
            inboundOrder.Warehouse.Id = Convert.ToInt32(this.ddlWarehouse.SelectedValue);
            //inboundOrder.Vendor.Id = 0;
            inboundOrder.Vendor.Id = Convert.ToInt32(this.ddlVendor.SelectedValue);
            inboundOrder.Status = chkStatus.Checked;
            inboundOrder.HasIssues = chkHasIssues.Checked;

            inboundOrder.InboundDetails = inboundDetails;

            divGrid.Visible = true;
            divModal.Visible = false;

            var countSufix = base.GetLPNNumberSufix();

            if (hidEditId.Value == "0")
                inboundOrderViewDTO = iReceptionMGR.MaintainInboundOrder(CRUD.Create, inboundOrder, context, countSufix);
            else
                inboundOrderViewDTO = iReceptionMGR.MaintainInboundOrder(CRUD.Update, inboundOrder, context, countSufix);

            if (inboundOrderViewDTO.hasError())
            {
                UpdateSession(true);
                divGrid.Visible = false;
                divModal.Visible = true;
            }
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(inboundOrderViewDTO.MessageStatus.Message);
                UpdateSession(false);
                Session.Remove(WMSTekSessions.InboundOrderMgr.InboundDetailList);
            }

            txtCode.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtQty.Text = string.Empty;
        }

        /// <summary>
        /// Respuesta desde la ventana de diálogo
        /// </summary>
        protected void btnDialogCancel_Click(object sender, EventArgs e)
        {
            try
            {
                switch (this.Master.ucDialog.Caller)
                {
                    case "reset":
                        ddlOwner.SelectedIndex = Convert.ToInt16(hidSelectedOwner.Value);
                        break;
                }
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
            }
        }
        #endregion



        #region Carga Masiva
        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            try
            {
                divGrid.Visible = true;
                divModal.Visible = false;

                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inboundOrderViewDTO.Errors);
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
                    GenericViewDTO<InboundOrder> newInboundOrder = new GenericViewDTO<InboundOrder>();

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
                    DataTable dataTableDetail;
                    try
                    {
                        dataTable = ConvertXlsToDataTable(savePath, 1);
                        dataTableDetail = ConvertXlsToDataTable(savePath, 2);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    //Cabezera
                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       WhsCode = r.Field<object>("WhsCode"),
                                       OwnCode = r.Field<object>("OwnCode"),
                                       InboundNumber = r.Field<object>("InboundNumber"),
                                       InboundTypeCode = r.Field<object>("InboundTypeCode"),
                                       OrderComment = r.Field<object>("OrderComment"),
                                       VendorCode = r.Field<object>("VendorCode"),
                                       DateExpected = r.Field<object>("DateExpected"),
                                       EmissionDate = r.Field<object>("EmissionDate"),
                                       ExpirationDate = r.Field<object>("ExpirationDate"),
                                       Status = r.Field<object>("Status"),
                                       OutboundNumberSource = r.Field<object>("OutboundNumberSource"),
                                       IsAsn = r.Field<object>("IsAsn"),
                                       PercentLpnInspection = r.Field<object>("PercentLpnInspection"),
                                       PercentQA = r.Field<object>("PercentQA"),
                                       ShiftNumber = r.Field<object>("ShiftNumber"),
                                       HasIssues = r.Field<object>("HasIssues"),
                                       SpecialField1 = r.Field<object>("SpecialField1"),
                                       SpecialField2 = r.Field<object>("SpecialField2"),
                                       SpecialField3 = r.Field<object>("SpecialField3"),
                                       SpecialField4 = r.Field<object>("SpecialField4")
                                   };

                    //Detalle
                    var lstDetail = from r in dataTableDetail.AsEnumerable()
                                    select new
                                    {
                                        InboundNumber = r.Field<object>("InboundNumber"),
                                        InboundTypeCode = r.Field<object>("InboundTypeCode"),
                                        LineNumber = r.Field<object>("LineNumber"),
                                        LineCode = r.Field<object>("LineCode"),
                                        ItemCode = r.Field<object>("ItemCode"),
                                        CtgCode = r.Field<object>("CtgCode"),
                                        ItemQty = r.Field<object>("ItemQty"),
                                        Status = r.Field<object>("Status"),
                                        LineComment = r.Field<object>("LineComment"),
                                        FifoDate = r.Field<object>("FifoDate"),
                                        ExpirationDate = r.Field<object>("ExpirationDate"),
                                        FabricationDate = r.Field<object>("FabricationDate"),
                                        LotNumber = r.Field<object>("LotNumber"),
                                        LpnCode = r.Field<object>("LpnCode"),
                                        Price = r.Field<object>("Price"),
                                        Weight = r.Field<object>("Weight"),
                                        OutboundNumber = r.Field<object>("OutboundNumber"),
                                        SpecialField1 = r.Field<object>("SpecialField1"),
                                        SpecialField2 = r.Field<object>("SpecialField2"),
                                        SpecialField3 = r.Field<object>("SpecialField3"),
                                        SpecialField4 = r.Field<object>("SpecialField4")
                                    };


                    GenericViewDTO<Warehouse> warehouseViewDTOLoad = iLayoutMGR.FindAllWarehouse(context);
                    GenericViewDTO<Owner> ownViewDTOLoad = iWarehousingMGR.FindAllOwner(context);
                    GenericViewDTO<InboundType> inboundTypeViewDTOLoad = iReceptionMGR.FindAllInboundType(context);

                    try
                    {
                        inboundOrderViewDTO = new GenericViewDTO<InboundOrder>();
                        foreach (var inbOrder in lstExcel)
                        {
                            InboundOrder newInbOrd = new InboundOrder();
                            newInbOrd.LatestInboundTrack = new InboundTrack();
                            newInbOrd.LatestInboundTrack.Type = new TrackInboundType();
                            newInbOrd.LatestInboundTrack.InboundOrder = new InboundOrder();
                            newInbOrd.LatestInboundTrack.Type.Id = (int)TrackInboundTypeName.Anunciado;

                            if (!ValidateIsNotNull(inbOrder.InboundNumber))
                            {
                                errorUp = "InboundNumber " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newInbOrd.Number = inbOrder.InboundNumber.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(inbOrder.WhsCode))
                            {
                                errorUp = "InboundNumber " + inbOrder.InboundNumber + " - WhsCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (warehouseViewDTOLoad.Entities.Exists(w => w.Code.ToUpper().Equals(inbOrder.WhsCode.ToString().ToUpper().Trim())))
                                {
                                    newInbOrd.Warehouse = warehouseViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(inbOrder.WhsCode.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "InboundNumber " + inbOrder.InboundNumber + " - El WhsCode " + inbOrder.WhsCode + " no es valido para el sistema.";
                                    break;
                                }
                            }

                            if (!ValidateIsNotNull(inbOrder.OwnCode))
                            {
                                errorUp = "InboundNumber " + inbOrder.InboundNumber + " - OwnCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (ownViewDTOLoad.Entities.Exists(w => w.Code.ToUpper().Equals(inbOrder.OwnCode.ToString().ToUpper().Trim())))
                                {
                                    newInbOrd.Owner = ownViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(inbOrder.OwnCode.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    Owner newOwner = new Owner()
                                    {
                                        Code = inbOrder.OwnCode.ToString(),
                                        Name = inbOrder.OwnCode.ToString(),
                                        Country = new Country(-1),
                                        State = new State(-1),
                                        City = new City(-1)
                                    };

                                    var insertOwner = iWarehousingMGR.MaintainOwner(CRUD.Create, newOwner, context);

                                    if (insertOwner.hasError())
                                    {
                                        errorUp = "InboundNumber " + inbOrder.InboundNumber + " - El OwnCode " + inbOrder.OwnCode + " no es valido para el sistema y no se pudo insertar";
                                        break;
                                    }
                                    else
                                    {
                                        var getNewOwner = iWarehousingMGR.GetOwnerByCode(context, inbOrder.OwnCode.ToString());
                                        ownViewDTOLoad.Entities.Add(getNewOwner.Entities.FirstOrDefault());

                                        newInbOrd.Owner = getNewOwner.Entities.FirstOrDefault();

                                        Vendor newVendor = new Vendor()
                                        {
                                            Name = inbOrder.VendorCode.ToString(),
                                            Code = inbOrder.VendorCode.ToString(),
                                            Owner = new Owner(getNewOwner.Entities.FirstOrDefault().Id),
                                            Country = new Country(-1),
                                            State = new State(-1),
                                            City = new City(-1)
                                        };

                                        var insertVendor = iWarehousingMGR.MaintainVendor(CRUD.Create, newVendor, context);

                                        if (insertVendor.hasError())
                                        {
                                            errorUp = "InboundNumber " + inbOrder.InboundNumber + " - El VendorCode " + inbOrder.VendorCode + " no es valido para el sistema y no se pudo insertar";
                                            break;
                                        }
                                        else
                                        {
                                            var getVendor = iWarehousingMGR.GetVendorByCodeAndOwner(context, inbOrder.VendorCode.ToString(), getNewOwner.Entities.FirstOrDefault().Id);
                                            newInbOrd.Vendor = getVendor.Entities.FirstOrDefault();
                                        }
                                    }
                                }
                            }

                            //InboundType
                            if (!ValidateIsNotNull(inbOrder.InboundTypeCode))
                            {
                                errorUp = "InboundNumber " + inbOrder.InboundNumber + " - InboundTypeCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (inboundTypeViewDTOLoad.Entities.Exists(w => w.Code.ToUpper().Equals(inbOrder.InboundTypeCode.ToString().ToUpper().Trim())))
                                {
                                    newInbOrd.InboundType = inboundTypeViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(inbOrder.InboundTypeCode.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "InboundNumber " + inbOrder.InboundNumber + " - El InboundTypeCode " + inbOrder.InboundTypeCode + " no es valido para el sistema.";
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(inbOrder.OrderComment))
                                newInbOrd.Comment = inbOrder.OrderComment.ToString().Trim();

                            //Vendor
                            if (!ValidateIsNotNull(inbOrder.VendorCode))
                            {
                                errorUp = "InboundNumber " + inbOrder.InboundNumber + " - VendorCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                int idOwnLoad = ownViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(inbOrder.OwnCode.ToString().ToUpper().Trim())).First().Id;
                                GenericViewDTO<Vendor> vendorViewDTOLoad = iWarehousingMGR.GetVendorByCodeAndOwner(context, inbOrder.VendorCode.ToString().Trim(), idOwnLoad);

                                if (vendorViewDTOLoad.Entities != null && vendorViewDTOLoad.Entities.Count > 0)
                                {
                                    newInbOrd.Vendor = vendorViewDTOLoad.Entities[0];
                                }
                                else
                                {
                                    errorUp = "InboundNumber " + inbOrder.InboundNumber + " - El VendorCode " + inbOrder.VendorCode + " no es valido para el sistema.";
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(inbOrder.DateExpected))
                                newInbOrd.DateExpected = Convert.ToDateTime(inbOrder.DateExpected);

                            if (ValidateIsNotNull(inbOrder.EmissionDate))
                                newInbOrd.EmissionDate = Convert.ToDateTime(inbOrder.EmissionDate);

                            if (ValidateIsNotNull(inbOrder.ExpirationDate))
                                newInbOrd.ExpirationDate = Convert.ToDateTime(inbOrder.ExpirationDate);

                            if (ValidateIsNotNull(inbOrder.Status))
                                newInbOrd.Status = inbOrder.Status.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(inbOrder.OutboundNumberSource))
                                //newInbOrd.OutboundOrder = new OutboundOrder(Convert.ToInt32(inbOrder.OutboundNumberSource.ToString().Trim()));
                                newInbOrd.OutboundOrder = new OutboundOrder() { Number = inbOrder.OutboundNumberSource.ToString().Trim() };
                            else
                                newInbOrd.OutboundOrder = new OutboundOrder();

                            if (ValidateIsNotNull(inbOrder.IsAsn))
                                newInbOrd.IsAsn = inbOrder.IsAsn.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(inbOrder.PercentLpnInspection))
                                newInbOrd.PercentLpnInspection = Convert.ToInt32(inbOrder.PercentLpnInspection.ToString().Trim());

                            if (ValidateIsNotNull(inbOrder.PercentQA))
                                newInbOrd.PercentQA = Convert.ToInt32(inbOrder.PercentQA.ToString().Trim());

                            if (ValidateIsNotNull(inbOrder.ShiftNumber))
                                newInbOrd.ShiftNumber = inbOrder.ShiftNumber.ToString().Trim();

                            if (ValidateIsNotNull(inbOrder.SpecialField1))
                                newInbOrd.SpecialField1 = inbOrder.SpecialField1.ToString().Trim();

                            if (ValidateIsNotNull(inbOrder.SpecialField2))
                                newInbOrd.SpecialField2 = inbOrder.SpecialField2.ToString().Trim();

                            if (ValidateIsNotNull(inbOrder.SpecialField3))
                                newInbOrd.SpecialField3 = inbOrder.SpecialField3.ToString().Trim();

                            if (ValidateIsNotNull(inbOrder.SpecialField4))
                                newInbOrd.SpecialField4 = inbOrder.SpecialField4.ToString().Trim();

                            if (ValidateIsNotNull(inbOrder.HasIssues))
                                newInbOrd.HasIssues = inbOrder.HasIssues.ToString().Equals("1") ? true : false;


                            //Valida que existan Detalles
                            if (lstDetail.ToList().Exists(w => w.InboundNumber.ToString().ToUpper().Trim().Equals(inbOrder.InboundNumber.ToString().ToUpper().Trim()) &&
                            w.InboundTypeCode.ToString().ToUpper().Trim().Equals(inbOrder.InboundTypeCode.ToString().ToUpper().Trim())))
                            {
                                var listExistItemDetail = lstDetail.ToList().Where(w => w.InboundNumber.ToString().ToUpper().Trim().Equals(inbOrder.InboundNumber.ToString().ToUpper().Trim()) &&
                                                            w.InboundTypeCode.ToString().ToUpper().Trim().Equals(inbOrder.InboundTypeCode.ToString().ToUpper().Trim())).ToList();

                                var repeteatedItems = listExistItemDetail.GroupBy(od => new
                                {
                                    ItemCode = od.ItemCode.ToString(),
                                    LotNumber = od.LotNumber.ToString(),
                                    CtgCode = od.CtgCode.ToString(),
                                    FabricationDate = od.FabricationDate.ToString(),
                                    ExpirationDate = od.ExpirationDate.ToString(),
                                    Lpn = od.LpnCode
                                })
                                .Where(grp => grp.Count() > 1)
                                .Select(od => new
                                {
                                    itemCode = od.Key.ItemCode,
                                    lot = od.Key.LotNumber,
                                    ctg = od.Key.LotNumber,
                                    fabriDate = od.Key.FabricationDate,
                                    expDate = od.Key.ExpirationDate,
                                    count = od.Count()
                                }).ToList();

                                if (repeteatedItems != null && repeteatedItems.Count > 0)
                                {
                                    errorUp = "InboundNumber " + inbOrder.InboundNumber + " - Detalle Documento. " + this.lblValidateRepeatedItems.Text;
                                    break;
                                }

                                newInbOrd.InboundDetails = new List<InboundDetail>();

                                foreach (var indDet in listExistItemDetail)
                                {
                                    InboundDetail newInbDet = new InboundDetail();
                                    newInbDet.InboundOrder = new InboundOrder();

                                    int idOwnLoadDet = ownViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(inbOrder.OwnCode.ToString().ToUpper().Trim())).First().Id;

                                    if (!ValidateIsNotNull(indDet.LineNumber))
                                    {
                                        errorUp = "InboundNumber " + inbOrder.InboundNumber + " - Detalle Documento, LineNumber " + this.lblFieldNotNull.Text;
                                        break;
                                    }
                                    else
                                    {
                                        newInbDet.LineNumber = Convert.ToInt32(indDet.LineNumber.ToString().Trim());
                                    }

                                    if (ValidateIsNotNull(indDet.LineCode))
                                        newInbDet.LineCode = indDet.LineCode.ToString().Trim();

                                    //Item
                                    GenericViewDTO<Item> itemViewDTOLoad = iWarehousingMGR.GetItemByCodeAndOwner(context, indDet.ItemCode.ToString().Trim(), idOwnLoadDet, false);

                                    if (!ValidateIsNotNull(indDet.ItemCode))
                                    {
                                        errorUp = "InboundNumber " + inbOrder.InboundNumber + " Linea " + newInbDet.LineNumber.ToString() + " - Detalle Documento, ItemCode " + this.lblFieldNotNull.Text;
                                        break;
                                    }
                                    else
                                    {
                                        if (itemViewDTOLoad.Entities != null && itemViewDTOLoad.Entities.Count > 0)
                                        {
                                            newInbDet.Item = itemViewDTOLoad.Entities[0];
                                        }
                                        else
                                        {
                                            var getOwner = newInbOrd.Owner; //iWarehousingMGR.GetOwnerByCode(context, inbOrder.OwnCode.ToString());

                                            Item newItem = new Item()
                                            {
                                                Code = indDet.ItemCode.ToString(),
                                                LongName = indDet.ItemCode.ToString(),
                                                ShortName = indDet.ItemCode.ToString(),
                                                Description = indDet.ItemCode.ToString(),
                                                Owner = new Owner(getOwner.Id)
                                            };

                                            var insertItem = iWarehousingMGR.MaintainItem(CRUD.Create, newItem, context);

                                            if (insertItem.hasError())
                                            {
                                                errorUp = "InboundNumber " + inbOrder.InboundNumber + " Linea " + newInbDet.LineNumber.ToString() + " - Detalle Documento, ItemCode " + indDet.ItemCode + " no es valido para el sistema y no se pudo insertar";
                                                break;
                                            }
                                            else
                                            {
                                                var getItem = iWarehousingMGR.GetItemByCodeAndOwner(context, indDet.ItemCode.ToString(), getOwner.Id, false);
                                                newInbDet.Item = getItem.Entities.FirstOrDefault();
                                            }
                                        }
                                    }

                                    //Categoria
                                    if (ValidateIsNotNull(indDet.CtgCode))
                                    {

                                        GenericViewDTO<CategoryItem> categoryItemViewDTOLoad = iWarehousingMGR.GetCategoryItemByCodeAndOwner(indDet.CtgCode.ToString().Trim(), idOwnLoadDet, context);

                                        if (itemViewDTOLoad.Entities != null && itemViewDTOLoad.Entities.Count > 0)
                                        {
                                            newInbDet.CategoryItem = categoryItemViewDTOLoad.Entities[0];
                                        }
                                        else
                                        {
                                            errorUp = "InboundNumber " + inbOrder.InboundNumber + " Linea " + newInbDet.LineNumber.ToString() + " - Detalle Documento, CtgCode " + indDet.CtgCode + " no es valido para el sistema.";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        newInbDet.CategoryItem = new CategoryItem();
                                    }

                                    if (!ValidateIsNotNull(indDet.ItemQty))
                                    {
                                        errorUp = "InboundNumber " + inbOrder.InboundNumber + " Linea " + newInbDet.LineNumber.ToString() + " - Detalle Documento, ItemQty " + this.lblFieldNotNull.Text;
                                        break;
                                    }
                                    else
                                    {
                                        decimal qty = 0;
                                        var isQtyDecimal = decimal.TryParse(indDet.ItemQty.ToString(), out qty);

                                        if (isQtyDecimal)
                                        {
                                            if (qty <= 0)
                                            {
                                                errorUp = "InboundNumber " + inbOrder.InboundNumber + " Linea " + newInbDet.LineNumber.ToString() + " - Detalle Documento, ItemQty " + this.lblValidateQty.Text;
                                                break;
                                            }
                                            else
                                            {
                                                newInbDet.ItemQty = qty;
                                            }
                                        }
                                    }

                                    if (!ValidateIsNotNull(indDet.Status))
                                    {
                                        errorUp = "InboundNumber " + inbOrder.InboundNumber + " Linea " + newInbDet.LineNumber.ToString() + " - Detalle Documento, Status " + this.lblFieldNotNull.Text;
                                        break;
                                    }
                                    else
                                    {
                                        newInbDet.Status = indDet.Status.ToString().Equals("1") ? true : false;
                                    }


                                    //if (ValidateIsNotNull(indDet.LineComment))
                                    //    newInbDet.LineComment = indDet.LineComment.ToString().Trim();

                                    if (ValidateIsNotNull(indDet.FifoDate))
                                        newInbDet.FifoDate = Convert.ToDateTime(indDet.FifoDate);

                                    if (ValidateIsNotNull(indDet.ExpirationDate))
                                        newInbDet.ExpirationDate = Convert.ToDateTime(indDet.ExpirationDate);

                                    if (ValidateIsNotNull(indDet.FabricationDate))
                                        newInbDet.FabricationDate = Convert.ToDateTime(indDet.FabricationDate);

                                    if (ValidateIsNotNull(indDet.LotNumber))
                                        newInbDet.LotNumber = indDet.LotNumber.ToString().Trim();

                                    if (ValidateIsNotNull(indDet.LpnCode))
                                        newInbDet.LpnCode = indDet.LpnCode.ToString().Trim();

                                    if (ValidateIsNotNull(indDet.Price))
                                        newInbDet.Price = Convert.ToDecimal(indDet.Price);

                                    if (ValidateIsNotNull(indDet.Weight))
                                        newInbDet.Weight = Convert.ToDecimal(indDet.Weight);

                                    if (ValidateIsNotNull(indDet.OutboundNumber))
                                        newInbDet.OutboundNumber = indDet.OutboundNumber.ToString().Trim();

                                    if (ValidateIsNotNull(indDet.SpecialField1))
                                        newInbDet.SpecialField1 = indDet.SpecialField1.ToString().Trim();

                                    if (ValidateIsNotNull(indDet.SpecialField2))
                                        newInbDet.SpecialField2 = indDet.SpecialField2.ToString().Trim();

                                    if (ValidateIsNotNull(indDet.SpecialField3))
                                        newInbDet.SpecialField3 = indDet.SpecialField3.ToString().Trim();

                                    if (ValidateIsNotNull(indDet.SpecialField4))
                                        newInbDet.SpecialField4 = indDet.SpecialField4.ToString().Trim();

                                    newInbOrd.InboundDetails.Add(newInbDet);
                                }

                            }
                            else
                            {
                                errorUp = "El Nro Documento " + inbOrder.InboundNumber + " Tipo " + inbOrder.InboundTypeCode + ", NO cuenta con Detalles.";
                                break;
                            }

                            inboundOrderViewDTO.Entities.Add(newInbOrd);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }

                    if (errorUp != "")
                    {
                        ShowAlertLocal(this.lblTitle.Text, errorUp);
                        divLoad.Visible = true;
                        modalPopUpLoad.Show();
                    }
                    else
                    {
                        if (inboundOrderViewDTO.Entities.Count > 0)
                        {
                            var countSufix = base.GetLPNNumberSufix();

                            inboundOrderViewDTO = iReceptionMGR.MaintainInboundOrderMassive(inboundOrderViewDTO, countSufix, context);

                            if (inboundOrderViewDTO.hasError())
                            {
                                //UpdateSession(true);
                                ShowAlertLocal(inboundOrderViewDTO.Errors.Title, inboundOrderViewDTO.Errors.Message);
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                ucStatus.ShowMessage(inboundOrderViewDTO.MessageStatus.Message);
                                ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                divLoad.Visible = false;
                                modalPopUpLoad.Hide();
                            }
                        }
                        else
                        {
                            ShowAlertLocal(this.lblTitle.Text, this.lblNotInboundOrderFile.Text);
                            divLoad.Visible = true;
                            modalPopUpLoad.Show();
                        }
                    }

                }
                else
                {
                    ShowAlertLocal(this.lblTitle.Text, this.lblNotFileLoad.Text);
                    divLoad.Visible = true;
                    modalPopUpLoad.Show();
                }

            }
            catch (InvalidDataException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (InvalidOperationException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                inboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, inboundOrderViewDTO.Errors.Message);
            }
            finally
            {
                //Pregunta si existe el archivo y lo elimina
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }

                UpdateSession(false);
            }
        }



        #endregion
    }
}