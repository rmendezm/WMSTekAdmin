using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using System.Linq;
using System.Threading;

namespace Binaria.WMSTek.WebClient.Outbound.Administration
{
    public partial class OutboundOrderMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<OutboundOrder> outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();
        private GenericViewDTO<OutboundDetail> outboundDetailViewDTO = new GenericViewDTO<OutboundDetail>();
        private List<OutboundDetail> outboundDetails = new List<OutboundDetail>();
        private GenericViewDTO<Item> itemSearchViewDTO;
        private GenericViewDTO<Customer> customerSearchViewDTO;
        //private GenericViewDTO<Vendor> vendorViewDTO;
        private bool isNew = false;
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

        // Propiedad para controlar el nro de pagina activa en la grilla Customer
        public int currentPageCustomer
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

        public int IdCountryDelivery
        {
            get { return (int)(ViewState["IdCountryDelivery"] ?? -1); }
            set { ViewState["IdCountryDelivery"] = value; }
        }

        public int IdStateDelivery
        {
            get { return (int)(ViewState["IdStateDelivery"] ?? -1); }
            set { ViewState["IdStateDelivery"] = value; }
        }

        public int IdCityDelivery
        {
            get { return (int)(ViewState["IdCityDelivery"] ?? -1); }
            set { ViewState["IdCityDelivery"] = value; }
        }

        public int IdCountryFact
        {
            get { return (int)(ViewState["IdCountryFact"] ?? -1); }
            set { ViewState["IdCountryFact"] = value; }
        }

        public int IdStateFact
        {
            get { return (int)(ViewState["IdStateFact"] ?? -1); }
            set { ViewState["IdStateFact"] = value; }
        }

        public int IdCityFact
        {
            get { return (int)(ViewState["IdCityFact"] ?? -1); }
            set { ViewState["IdCityFact"] = value; }
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

                if (!Page.IsPostBack)
                {

                    this.tabLayout.HeaderText = this.lbltabGeneral.Text;
                    this.TabDelivery.HeaderText = this.lblTabDelivery.Text;
                    this.TabSales.HeaderText = this.lblTabSales.Text;

                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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

                        if (this.ddlIdOutboundType.SelectedValue == "5")//Armado de Kit
                        {
                            GenericViewDTO<Kit> KitViewDTO = new GenericViewDTO<Kit>();
                            itemSearchViewDTO = new GenericViewDTO<Item>();
                            Item item = new Item();
                            item.Code = this.txtCode.Text;
                            item.LongName = this.txtDescription.Text;

                            KitViewDTO = iWarehousingMGR.GetKitByItemCodeItemName(ucFilterItem.FilterItems, context, Convert.ToInt32(this.ddlOwner.SelectedValue), true);

                            //Recorre el DTO para traspasar los resultados de busqueda a entidad de tipo ITEM para poder cargar la grilla.
                            foreach (Kit kit in KitViewDTO.Entities)
                            {
                                item = new Item();
                                item = kit.ItemKit;
                                itemSearchViewDTO.Entities.Add(item);
                            }
                            ShowItem(validItem, existingItem);
                        }
                        else if (this.ddlIdOutboundType.SelectedValue == "6")//Desarme de Kit
                        {
                            itemSearchViewDTO = new GenericViewDTO<Item>();
                            itemSearchViewDTO = iWarehousingMGR.GetItemKitByItemCodeItemName(ucFilterItem.FilterItems, context, Convert.ToInt16(ddlOwner.SelectedValue), true);
                            ShowItem(validItem, existingItem);
                        }
                        else
                        {
                            itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, txtCode.Text.Trim(), Convert.ToInt16(ddlOwner.SelectedValue), false);
                            ShowItem(validItem, existingItem);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }



        protected void imgBtnCustmerSearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    bool validCustomer = false;

                    // Busca en base de datos el Customer ingresado 
                    if (txtCustomerCode.Text.Trim() != string.Empty)
                    {
                        customerSearchViewDTO = new GenericViewDTO<Customer>();
                        Customer customer = new Customer();
                        customer.Code = this.txtCustomerCode.Text;
                        //customer.Name = this.txtDescription.Text;

                        //KitViewDTO = iWarehousingMGR.GetKitByItemCodeItemName(ucFilterCustomer.FilterItems, context, Convert.ToInt32(this.ddlOwner.SelectedValue), true);
                        //customerSearchViewDTO = iWarehousingMGR.FindByOwnerCodeOwnerName(ucFilterCustomer.FilterItems, context,Convert.ToInt32(this.ddlOwner.SelectedValue));
                        validCustomer = false;

                    }

                    // Si no es válido o no se ingresó, se muestra la lista de Customers para seleccionar uno
                    if (!validCustomer)
                    {
                        ucFilterCustomer.Clear();
                        ucFilterCustomer.Initialize();

                        // Setea el filtro con el Customer ingresado
                        if (txtCustomerCode.Text.Trim() != string.Empty)
                        {
                            FilterItem filterItem = new FilterItem("%" + txtCustomerCode.Text + "%");
                            filterItem.Selected = true;
                            ucFilterCustomer.FilterItems[0] = filterItem;
                            ucFilterCustomer.LoadCurrentFilter(ucFilterCustomer.FilterItems);
                            SearchCustomer();
                        }
                        // Si no se ingresó ningún customer, no se ejecuta la búsqueda
                        else
                            ClearGridCustomer();

                        // Esto evita un bug de ajax
                        valEditNew.Enabled = false;
                        valAddCustomer.Enabled = false;
                        valSearchCustomer.Enabled = false;

                        divLookupCustomer.Visible = true;
                        mpLookupCustomer.Show();

                        InitializePageCountCustomer();
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        private void ShowCustomerButtonsPage()
        {
            if (currentPageCustomer == grdSearchCustomers.PageCount - 1)
            {
                btnNextGrdSearchCustomers.Enabled = false;
                btnNextGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastGrdSearchCustomers.Enabled = false;
                btnLastGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                btnPrevGrdSearchCustomers.Enabled = true;
                btnPrevGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                btnFirstGrdSearchCustomers.Enabled = true;
                btnFirstGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
            }
            else
            {
                if (currentPageCustomer == 0)
                {
                    btnPrevGrdSearchCustomers.Enabled = false;
                    btnPrevGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstGrdSearchCustomers.Enabled = false;
                    btnFirstGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                    btnNextGrdSearchCustomers.Enabled = true;
                    btnNextGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchCustomers.Enabled = true;
                    btnLastGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnNextGrdSearchCustomers.Enabled = true;
                    btnNextGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchCustomers.Enabled = true;
                    btnLastGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevGrdSearchCustomers.Enabled = true;
                    btnPrevGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstGrdSearchCustomers.Enabled = true;
                    btnFirstGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
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

        private void ShowItem(bool validItem, bool existingItem)
        {
            // Si el codigo de Item ingresado es válido, lo carga directamente
            if (itemSearchViewDTO.Entities != null && itemSearchViewDTO.Entities.Count == 1)
            {
                validItem = true;
                Item item = new Item(itemSearchViewDTO.Entities[0].Id);

                item.Description = itemSearchViewDTO.Entities[0].Description;
                item.Code = itemSearchViewDTO.Entities[0].Code;

                // Mantiene en memoria los datos del Item a agregar
                Session.Add("OutboundMgrNewItem", item);

                // Recorre los items ya agregados y compara con el que se quiere agregar
                if (outboundDetails != null && outboundDetails.Count > 0)
                {
                    foreach (OutboundDetail outboundDetail in outboundDetails)
                    {
                        // Si ya existe en la lista se marca
                        if (outboundDetail.Item.Code == item.Code)
                        {
                            existingItem = true;
                            pnlError.Visible = false;
                        }
                    }
                }

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
        //protected void imgbtnSearchVendor_Click(object sender, ImageClickEventArgs e)
        //{
        //    pnlPanelPoUp.Visible = true;
        //    //mpeModalPopUpVendor.Show();
        //}

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
        //        outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                    ImageButton btnCancelOrder = e.Row.FindControl("btnCancel") as ImageButton;
                    ImageButton btnPendingOrder = e.Row.FindControl("btnPending") as ImageButton;
                    ImageButton btnChangeTrack = e.Row.FindControl("btnChangeTrack") as ImageButton;

                    if (btnDelete != null)
                    {
                        btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";
                    }

                    if (btnCancelOrder != null)
                    {
                        btnCancelOrder.OnClientClick = "if(confirm('" + lblConfirmCancel.Text + "')==false){return false;}";
                    }

                    if (btnPendingOrder != null)
                    {
                        btnPendingOrder.OnClientClick = "if(confirm('" + lblConfirmPending.Text + "')==false){return false;}";
                    }

                    // Deshabilita la opcion de Editar y Eliminar si la Orden esta en estado distinto a 'Anunciada'
                    if (btnDelete != null && outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id > (int)TrackOutboundTypeName.Pending)
                    {
                        btnDelete.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_delete_dis.png";
                        btnDelete.Enabled = false;
                    }

                    // Deshabilita la opcion de Habilitar si la Orden esta en estado distinto a 'Released'
                    if (btnPendingOrder != null && outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id != (int)TrackOutboundTypeName.Released)
                    {
                        btnPendingOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_tick_dis.png";
                        btnPendingOrder.Enabled = false;
                    }

                    //Anular Documento
                    if (btnCancelOrder != null && (outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Pending ||
                        outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Shipped ||
                        outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Closed ||
                        outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Cancel ||
                        outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Load ||
                        outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Loading ||
                        outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.DeliveredToCarrier ||
                        outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.DeliveredToCustomer))
                    {
                        btnCancelOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_cancel_dis.png";
                        btnCancelOrder.Enabled = false;
                    }
                    else
                    {
                        btnCancelOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_cancel.png";
                        btnCancelOrder.Enabled = true;
                    }

                    var validTracksToEdit = ValidTracksToEdit();

                    bool validTrackToEdit = validTracksToEdit.Contains(outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id);

                    if (btnEdit != null && !validTrackToEdit)
                    {
                        btnEdit.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_edit_dis.png";
                        btnEdit.Enabled = false;
                    }

                    //Deshabilita la opcion editar, eliminar y habilitar si la orden es de tipo Wave
                    if (outboundOrderViewDTO.Entities[e.Row.DataItemIndex].OutboundType.Code == "PIKWV")
                    {
                        btnEdit.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_edit_dis.png";
                        btnEdit.Enabled = false;

                        btnDelete.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_delete_dis.png";
                        btnDelete.Enabled = false;

                        btnPendingOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_tick_dis.png";
                        btnPendingOrder.Enabled = false;
                    }

                    // Deshabilita la opcion de 'Cerrar Orden' si la Orden ya está Cerrada
                    if (btnCloseOrder != null && outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Closed)
                    {
                        btnCloseOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_close_dis.png";
                        btnCloseOrder.Enabled = false;
                    }
                    else
                    {
                        btnCloseOrder.OnClientClick = "if(confirm('" + lblConfirmClose.Text + "')==false){return false;}";
                        //   btnCloseOrder.CommandArgument = e.Row.DataItemIndex.ToString();
                    }

                    int track = outboundOrderViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id;
                    if (btnChangeTrack != null && (track >= (int)TrackOutboundTypeName.Shipped && track != (int)TrackOutboundTypeName.Closed && track != (int)TrackOutboundTypeName.Cancel))
                    {
                        btnChangeTrack.Visible = true;
                    }
                    else
                    {
                        btnChangeTrack.Visible = false;
                    }

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);
                int batch;

                batch = OrderBelongsBatch(outboundOrderViewDTO.Entities[index].Id, outboundOrderViewDTO.Entities[index].Owner.Id);
                if (batch > 0 && e.CommandName != "ChangeTrack")
                {
                    ErrorDTO errorDTO = new ErrorDTO();
                    errorDTO = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.OutboundOrder.BelongsBatch, context));
                    Master.ucDialog.ShowAlert(lblTitle.Text, errorDTO.Title + ": " + batch.ToString(), String.Empty);
                    return;
                }

                var wave = OrderBelongsWave(outboundOrderViewDTO.Entities[index]);
                if (wave != null)
                {
                    var isValid = true;

                    if (wave.LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Picking)
                        isValid = false;
                    else if (wave.LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Sorting)
                    {
                        var taskParam = new Task()
                        {
                            Warehouse = new Warehouse(wave.Warehouse.Id),
                            Owner = new Owner(wave.Owner.Id),
                            IdDocumentBound = wave.Id,
                            TypeCode = "SORT"
                        };
                        var taskViewDTO = iTasksMGR.GetTaskByAnyParameter(taskParam, context);

                        if (!taskViewDTO.hasError() && taskViewDTO.Entities.Count > 0)
                        {
                            var waveTask = taskViewDTO.Entities.First();

                            if (!waveTask.IsComplete)
                                isValid = false;
                        }
                    }

                    if (!isValid)
                    {
                        var error = lblWaveValidation.Text.Replace("[WAVEID]", wave.Id.ToString());
                        Master.ucDialog.ShowAlert(lblTitle.Text, error, String.Empty);
                        return;
                    }
                }

                if (e.CommandName == "CloseOrder")
                {
                    CloseOrder(index);
                }
                else if (e.CommandName == "CancelOrder")
                {
                    CancelOrder(index);
                }
                else if (e.CommandName == "PendingOrder")
                {
                    PendingOrder(index);
                }
                else if (e.CommandName == "ChangeTrack")
                {
                    FillDdlTracks();

                    txtDateTrack.Text = string.Empty;
                    txtHourTrack.Text = string.Empty;

                    hidIdDocumentSelected.Value = outboundOrderViewDTO.Entities[index].Id.ToString();

                    divChangeTrack.Visible = true;
                    modalPopupChangeTrack.Show();
                }

            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;
                context.SessionInfo.IdPage = "OutboundOrderConsult";
                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    SaveChanges();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega el item seleccionado de la grilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                            Session.Add("OutboundMgrNewItem", item);

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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        /// <summary>
        /// Agrega el item seleccionado de la grilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdSearchCustomers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int editIndex = (Convert.ToInt32(grdSearchCustomers.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                if (ValidateSession(WMSTekSessions.Shared.CustomerList))
                {
                    customerSearchViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.Shared.CustomerList];

                    foreach (Customer customer in customerSearchViewDTO.Entities)
                    {
                        if (customer.Id == editIndex)
                        {
                            this.txtCustomerCode.Text = customer.Code;
                            this.txtCustomerName.Text = customer.Name;

                            if (!string.IsNullOrEmpty(customer.Address1Delv))
                                txtDeliveryAddress1.Text = customer.Address1Delv;

                            if (!string.IsNullOrEmpty(customer.Address2Delv))
                                txtDeliveryAddress2.Text = customer.Address2Delv;

                            hidCustomerId.Value = customer.Id.ToString();
                            hidIdCustomer.Value = customer.Id.ToString();
                            Session.Add("OutboundMgrNewCustomer", customer);

                            // Esto evita un bug de ajax
                            valEditNew.Enabled = true;
                            valAddCustomer.Enabled = true;
                            valSearchCustomer.Enabled = true;

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ddlOwner_Changed(object sender, EventArgs e)
        {
            try
            {
                base.LoadCategoryItemByOwner(this.ddlCategoryItem, Convert.ToInt32(ddlOwner.SelectedValue), false, true, this.Master.EmptyRowText);
                this.txtCustomerCode.Text = string.Empty;
                if (this.ddlOwner.SelectedIndex == 0)
                {
                    this.imgBtnCustmerSearch.Enabled = false;
                    this.imgBtnCustmerSearch.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_search_dis.png";
                }
                else
                {
                    this.imgBtnCustmerSearch.Enabled = true;
                    this.imgBtnCustmerSearch.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_search.png";
                }

                if (ddlOwner.SelectedIndex > 0)
                    ValidateFieldsForSimpliroute(int.Parse(ddlOwner.SelectedValue), int.Parse(ddlWarehouse.SelectedValue));
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ddlWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlWarehouse.SelectedIndex > 0)
                    ValidateFieldsForSimpliroute(int.Parse(ddlOwner.SelectedValue), int.Parse(ddlWarehouse.SelectedValue));
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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

                // Recupera el Item a agregar
                Item newItem = (Item)Session["OutboundMgrNewItem"];

                // Crea el nuevo detalle de la Orden
                OutboundDetail newDetail = new OutboundDetail();

                newDetail.Item = newItem;
                newDetail.Item.Id = Convert.ToInt32(hidItemId.Value);

                if (MiscUtils.IsNumeric(txtQty.Text))
                    newDetail.ItemQty = Convert.ToDecimal(txtQty.Text);
                else
                    newDetail.ItemQty = 0;

                newDetail.Status = true;
                newDetail.OutboundOrder = new OutboundOrder();
                newDetail.OutboundOrder.Id = Convert.ToInt32(hidEditId.Value);

                if (!string.IsNullOrEmpty(ddlCategoryItem.SelectedValue) && divCategoryItem.Visible && ddlCategoryItem.SelectedValue != "-1")
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

                // Si ya existen detalles, recorre el los Items existentes y compara con el que se quiere agregar
                if (outboundDetails != null && outboundDetails.Count > 0)
                {
                    foreach (OutboundDetail outboundDetail in outboundDetails)
                    {
                        // Si ya existe en el detalle, se avisa
                        if ((outboundDetail.Item.Id == newDetail.Item.Id) && (outboundDetail.CategoryItem.Id == newDetail.CategoryItem.Id)
                            && (outboundDetail.LotNumber == newDetail.LotNumber) && (outboundDetail.FabricationDate == newDetail.FabricationDate)
                            && (outboundDetail.ExpirationDate == newDetail.ExpirationDate))
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
                    outboundDetails = new List<OutboundDetail>();
                    outboundDetails.Add(newDetail);
                }

                if (addItem) outboundDetails.Add(newDetail);

                Session.Add(WMSTekSessions.OutboundOrderMgr.OutboundDetailList, outboundDetails);

                // Limpia panel Nuevo Item
                txtCode.Text = string.Empty;
                txtDescription.Text = string.Empty;
                txtQty.Text = string.Empty;
                txtLotItem.Text = string.Empty;
                txtFabricationDateItem.Text = string.Empty;
                txtExpirationDateItem.Text = string.Empty;
                ddlCategoryItem.ClearSelection();
                rvQty.Enabled = false;

                grdItems.DataSource = outboundDetails;
                grdItems.DataBind();
                grdItems.Visible = true;

                divCategoryItem.Visible = true;
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ddlCountryDelivery_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el pais... cambiará el estado y la ciudad
                isNew = true;
                base.ConfigureDDlState(this.ddlStateDelivery, isNew, IdStateDelivery, Convert.ToInt32(this.ddlCountryDelivery.SelectedValue), this.Master.EmptyRowText);
                base.ConfigureDDlCity(this.ddlCityDelivery, isNew, IdCityDelivery, Convert.ToInt32(this.ddlStateDelivery.SelectedValue), Convert.ToInt32(this.ddlCountryDelivery.SelectedValue), this.Master.EmptyRowText);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ddlStateDelivery_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el estado, solo cambia la ciudad.
                isNew = true;
                base.ConfigureDDlCity(this.ddlCityDelivery, isNew, IdCityDelivery, Convert.ToInt32(this.ddlStateDelivery.SelectedValue), Convert.ToInt32(this.ddlCountryDelivery.SelectedValue), this.Master.EmptyRowText);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ddlCountryFact_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el pais... cambiará el estado y la ciudad
                isNew = true;
                base.ConfigureDDlState(this.ddlStateFact, isNew, IdStateFact, Convert.ToInt32(this.ddlCountryFact.SelectedValue), this.Master.EmptyRowText);
                base.ConfigureDDlCity(this.ddlCityFact, isNew, IdCityFact, Convert.ToInt32(this.ddlStateFact.SelectedValue), Convert.ToInt32(this.ddlCountryFact.SelectedValue), this.Master.EmptyRowText);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ddlStateFact_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el estado, solo cambia la ciudad.
                isNew = true;
                base.ConfigureDDlCity(this.ddlCityFact, isNew, IdCityFact, Convert.ToInt32(this.ddlStateFact.SelectedValue), Convert.ToInt32(this.ddlCountryFact.SelectedValue), this.Master.EmptyRowText);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ddlCityDelivery_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hidIdCustomer.Value == "-1" || string.IsNullOrEmpty(hidIdCustomer.Value))
                {
                    return;
                }

                var listBranches = GetBranches(Convert.ToInt32(this.ddlOwner.SelectedValue), Convert.ToInt32(hidIdCustomer.Value), Convert.ToInt32(this.ddlCityDelivery.SelectedValue));

                if (listBranches != null && listBranches.Count > 0)
                {
                    FillDdlBranches(ddlBranch, listBranches);
                }
                else
                {
                    ClearDdlBranches();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        private void ClearDdlBranches()
        {
            ddlBranch.Items.Clear();
            ddlBranch.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
            ddlBranch.Items[0].Selected = true;
        }

        private List<Branch> GetBranches(int idOwn, int idCustomer, int? idCity)
        {
            var branchParameter = new Branch()
            {
                Owner = new Owner() { Id = idOwn },
                Customer = new Customer() { Id = idCustomer},
            };

            if (idCity != null)
            {
                branchParameter.City = new City() { Id = (int)idCity };
            }

            var branchesDTO = iWarehousingMGR.GetBranchByAnyParameter(branchParameter, context);

            if (branchesDTO.hasError())
            {
                this.Master.ucError.ShowError(branchesDTO.Errors);
                return null;
            }
            else
            {
                return branchesDTO.Entities;
            }
        }

        private void FillDdlBranches(DropDownList ddl, List<Branch> listBranches)
        {
            ddl.DataSource = listBranches;
            ddl.DataTextField = "Name";
            ddl.DataValueField = "Id";
            ddl.DataBind();

            ddl.Items.Insert(0, new ListItem("Seleccione", "-1"));
            ddl.Items[0].Selected = true;
        }

        protected void ddlPagesSearchCustomersSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValidateSession(WMSTekSessions.Shared.CustomerList))
            {
                customerSearchViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.Shared.CustomerList];

                currentPageCustomer = ddlPagesSearchCustomers.SelectedIndex;
                grdSearchCustomers.PageIndex = currentPageCustomer;

                // Configura ORDEN de las columnas de la grilla
                //if (!customerSearchViewDTO.hasConfigurationError() && customerSearchViewDTO.Configuration != null && customerSearchViewDTO.Configuration.Count > 0)
                //    base.ConfigureGridOrder(grdSearchCustomers, customerSearchViewDTO.Configuration);

                // Encabezado de Recepciones
                grdSearchCustomers.DataSource = customerSearchViewDTO.Entities;
                grdSearchCustomers.DataBind();

                divLookupCustomer.Visible = true;
                mpLookupCustomer.Show();

                ShowCustomerButtonsPage();

            }
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
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearchItem_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            try
            {

                SearchCustomer();

                // Si es un ViewDTO valido, carga la grilla y las listas
                if (isValidViewDTO)
                {
                    divLookupCustomer.Visible = true;
                    mpLookupCustomer.Show();

                    InitializePageCountCustomer();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void btnFirstGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchCustomers.SelectedIndex = 0;
            ddlPagesSearchCustomersSelectedIndexChanged(sender, e);

        }

        protected void btnFirstGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = 0;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);

        }

        protected void btnPrevGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomer > 0)
            {
                ddlPagesSearchCustomers.SelectedIndex = currentPageCustomer - 1; ;
                ddlPagesSearchCustomersSelectedIndexChanged(sender, e);
            }
        }

        protected void btnPrevGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems > 0)
            {
                ddlPagesSearchItems.SelectedIndex = currentPageItems - 1; ;
                ddlPagesSearchItemsSelectedIndexChanged(sender, e);
            }
        }

        protected void btnLastGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchCustomers.SelectedIndex = grdSearchCustomers.PageCount - 1;
            ddlPagesSearchCustomersSelectedIndexChanged(sender, e);

        }

        protected void btnLastGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = grdSearchItems.PageCount - 1;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);

        }

        protected void btnNextGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomer < grdSearchCustomers.PageCount)
            {
                ddlPagesSearchCustomers.SelectedIndex = currentPageCustomer + 1; ;
                ddlPagesSearchCustomersSelectedIndexChanged(sender, e);

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

        protected void grdSearchCustomers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void btnCloseChangeTrack_Click(object sender, EventArgs e)
        {
            divChangeTrack.Visible = false;
            modalPopupChangeTrack.Hide();
            upChangeTrack.Update();
        }

        protected void btnGenerateChangeTrack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(hidIdDocumentSelected.Value))
                {
                    DateTime finalDate;
                    bool isValidDate = DateTime.TryParseExact(txtDateTrack.Text.Trim() + " " + txtHourTrack.Text.Trim(), "dd/MM/yyyy hh:mm", null, System.Globalization.DateTimeStyles.None, out finalDate);

                    if (isValidDate)
                    {
                        int idOutboundOrder = int.Parse(hidIdDocumentSelected.Value);
                        int idTrackSelected = int.Parse(ddlTracks.SelectedValue);

                        TrackOutboundTypeName trackSelected = (TrackOutboundTypeName)Enum.ToObject(typeof(TrackOutboundTypeName), idTrackSelected);

                        ChangeTrack(idOutboundOrder, trackSelected, finalDate);

                        UpdateSession(false);
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
        //    catch (Exception ex)
        //    {
        //        outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
        //    }
        //}

        //private void SearchVendor()
        //{
        //    try
        //    {
        //        base.ConfigureMgrFilter(ucFilterVendor.FilterItems);
        //        vendorViewDTO = iWarehousingMGR.FindAllvendor(context);
        //        Session.Add(Constants.SessionDtoVendorList, vendorViewDTO);
        //        grdVendor.DataSource = vendorViewDTO.Entities;
        //        grdVendor.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
        //    }
        //}

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "OutboundOrderMgr";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeFilterItem();
            InitializeFilterCustomer();
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                //UpdateSession(false);
                PopulateLists();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.OutboundOrderMgr.OutboundOrderList))
                {
                    outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundOrderMgr.OutboundOrderList];
                    isValidViewDTO = true;
                }

                pnlError.Visible = false;

                //Carga la lista detalle desde la session
                if (ValidateSession(WMSTekSessions.OutboundOrderMgr.OutboundDetailList))
                    outboundDetails = (List<OutboundDetail>)Session[WMSTekSessions.OutboundOrderMgr.OutboundDetailList];

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
            this.Master.ucMainFilter.trackOutboundTypeVisible = true;
            this.Master.ucMainFilter.outboundTypeVisible = true;
            this.Master.ucMainFilter.OutboundTypeCode = new string[] { };

            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabDatesVisible = true;
            this.Master.ucMainFilter.expirationDateVisible = true;
            this.Master.ucMainFilter.expectedDateVisible = true;
            this.Master.ucMainFilter.tabDispatchingVisible = true;
            this.Master.ucMainFilter.tabDispatchingHeaderText = this.lblAdvancedFilter.Text;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

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

        private void InitializeFilterCustomer()
        {
            ucFilterCustomer.Initialize();
            ucFilterCustomer.BtnSearchClick += new EventHandler(btnSearchCustomer_Click);

            ucFilterCustomer.FilterCode = this.lblFilterCode.Text;
            ucFilterCustomer.FilterDescription = this.lblFilterName.Text;
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

        private void InitializeGridCustomer()
        {
            grdSearchCustomers.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchCustomers.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializeGridItems()
        {
            grdSearchItems.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchItems.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializePageCountCustomer()
        {
            if (grdSearchCustomers.PageCount > 1)
            {
                int pageNumber;

                divPageGrdSearchCustomers.Visible = true;
                // Paginador
                ddlPagesSearchCustomers.Items.Clear();
                for (int i = 0; i < grdSearchCustomers.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPageCustomer) lstItem.Selected = true;

                    ddlPagesSearchCustomers.Items.Add(lstItem);
                }
                this.lblPageCountSearchCustomers.Text = grdSearchCustomers.PageCount.ToString();

                ShowCustomerButtonsPage();
            }
            else
            {
                divPageGrdSearchCustomers.Visible = false;
            }
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

        private void PopulateLists()
        {
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false); ;
            base.LoadUserWarehouses(this.ddlWarehouse, this.Master.EmptyRowText, "-1", true);
            base.LoadOutboundType(this.ddlIdOutboundType, true, this.Master.EmptyRowText);
            base.LoadUserWarehouses(this.ddlWarehouseTarget, this.Master.EmptyRowText, "-1", true);
            base.LoadCarriers(ddlCarrier, this.Master.EmptyRowText, true);
        }

        private void ConfigureCountryStateCity()
        {
            base.ConfigureDDlCountry(this.ddlCountryDelivery, isNew, IdCountryDelivery, this.Master.EmptyRowText);
            base.ConfigureDDlState(this.ddlStateDelivery, isNew, IdStateDelivery, IdCountryDelivery, this.Master.EmptyRowText);
            base.ConfigureDDlCity(this.ddlCityDelivery, isNew, IdCityDelivery, IdStateDelivery, IdCountryDelivery, this.Master.EmptyRowText);

            base.ConfigureDDlCountry(this.ddlCountryFact, isNew, IdCountryFact, this.Master.EmptyRowText);
            base.ConfigureDDlState(this.ddlStateFact, isNew, IdStateFact, IdCountryFact, this.Master.EmptyRowText);
            base.ConfigureDDlCity(this.ddlCityFact, isNew, IdCityFact, IdStateFact, IdCountryFact, this.Master.EmptyRowText);
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
                outboundOrderViewDTO.ClearError();
            }

            this.Master.ucMainFilter.ClearFilterObject();
            this.Master.ucMainFilter.LoadControlValuesToFilterObject();

            // Carga lista de outboundDetail por id de outboundOrder
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            outboundOrderViewDTO = iDispatchingMGR.FindAllOutboundOrder(context);

            if (!outboundOrderViewDTO.hasError() && outboundOrderViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundOrderMgr.OutboundOrderList, outboundOrderViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(outboundOrderViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
            if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, outboundOrderViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = outboundOrderViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            if (divGrid.Visible)
                ucStatus.ShowRecordInfo(outboundOrderViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
            outboundDetails.RemoveAt(itemIndex);

            // Carga nuevamente el gridview.
            grdItems.DataSource = outboundDetails;
            grdItems.DataBind();
        }

        /// <summary>
        /// Pasa el item seleccionado al panel de edición / Solo es posible editar Cantidad y Categoría
        /// </summary>
        protected void EditItem(int itemIndex)
        {
            rvQty.Enabled = true;
            var outboundDetailSelected = outboundDetails[itemIndex];
            this.txtCode.Text = outboundDetails[itemIndex].Item.Code;
            this.txtCode.Enabled = false;
            this.txtDescription.Text = outboundDetails[itemIndex].Item.Description;
            this.txtQty.Text = outboundDetails[itemIndex].ItemQty.ToString();
            txtLotItem.Text = string.IsNullOrEmpty(outboundDetailSelected.LotNumber) ? string.Empty : outboundDetailSelected.LotNumber;
            txtFabricationDateItem.Text = outboundDetailSelected.FabricationDate > DateTime.MinValue ? outboundDetailSelected.FabricationDate.ToString("dd/MM/yyyy") : string.Empty;
            txtExpirationDateItem.Text = outboundDetailSelected.ExpirationDate > DateTime.MinValue ? outboundDetailSelected.ExpirationDate.ToString("dd/MM/yyyy") : string.Empty;

            if (outboundDetails[itemIndex].CategoryItem.Id != -1)
            {
                this.ddlCategoryItem.SelectedValue = outboundDetails[itemIndex].CategoryItem.Id.ToString();
                divCategoryItem.Visible = true;
            }
            else
            {
                this.ddlCategoryItem.SelectedIndex = 0;
                divCategoryItem.Visible = true;
            }
            //this.ddlCategoryItem.SelectedValue = outboundDetails[itemIndex].CategoryItem.Id.ToString();
            hidItemId.Value = outboundDetails[itemIndex].Item.Id.ToString();

            // Mantiene en memoria los datos del Item a agregar
            Session.Add("OutboundMgrNewItem", outboundDetails[itemIndex].Item);

            // Quita el item de la lista 
            outboundDetails.RemoveAt(itemIndex);

            // Carga nuevamente el gridview.
            grdItems.DataSource = outboundDetails;
            grdItems.DataBind();
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            // TODO: revisar logica -- mejor borrar Orden y Detalles en un solo paso
            //Recupera los datos de la entidad a editar
            hidEditId.Value = outboundOrderViewDTO.Entities[index].Id.ToString();

            //llena objeto detalle
            int id = outboundOrderViewDTO.Entities[index].Id;

            outboundDetailViewDTO = iDispatchingMGR.GetDetailByIdOutbound(context, id);
            outboundDetails = outboundDetailViewDTO.Entities;

            //agrega a sesion
            Session.Add(WMSTekSessions.OutboundOrderMgr.OutboundDetailList, outboundDetails);

            //eliminar la orden y sus detalles
            outboundOrderViewDTO = iDispatchingMGR.MaintainOutboundOrder(CRUD.Delete, (outboundOrderViewDTO.Entities[index]), context);

            if (outboundOrderViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(outboundOrderViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
            //ucFilterVendor.Clear();
        }

        private void SearchItem()
        {
            try
            {
                itemSearchViewDTO = new GenericViewDTO<Item>();

                if (this.ddlIdOutboundType.SelectedValue.ToString() == Convert.ToInt16(OutboundTypeName.ArmadoDeKit).ToString() ||
                    this.ddlIdOutboundType.SelectedValue.ToString() == Convert.ToInt16(OutboundTypeName.DesarmeDeKit).ToString()) //Armado de Kit
                {
                    GenericViewDTO<Kit> KitViewDTO = new GenericViewDTO<Kit>();
                    Item item = new Item();
                    item.Code = this.txtCode.Text;
                    item.LongName = this.txtDescription.Text;

                    KitViewDTO = iWarehousingMGR.GetKitByItemCodeItemName(ucFilterItem.FilterItems, context, Convert.ToInt32(this.ddlOwner.SelectedValue), true);

                    //Recorre el DTO para traspasar los resultados de busqueda a entidad de tipo ITEM para poder cargar la grilla.
                    foreach (Kit kit in KitViewDTO.Entities)
                    {
                        item = new Item();
                        item = kit.ItemKit;
                        itemSearchViewDTO.Entities.Add(item);
                    }
                    Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);
                    InitializeGridItems();
                    grdSearchItems.DataSource = itemSearchViewDTO.Entities;
                    grdSearchItems.DataBind();
                }
                else if (this.ddlIdOutboundType.SelectedValue.ToString() == Convert.ToInt16(OutboundTypeName.ServicioDeVas).ToString()) //Vas
                {
                    String theWhere = "";
                    GenericViewDTO<ItemVas> itemVasViewDTO = new GenericViewDTO<ItemVas>();
                    Item item = new Item();
                    item.Code = this.txtCode.Text;
                    item.LongName = this.txtDescription.Text;
                    item.Owner.Id = Convert.ToInt32(this.ddlOwner.SelectedValue);
                    if (item.Owner.Id > 0)
                        theWhere = " AND I.IdOwn = " + item.Owner.Id.ToString();
                    theWhere += " AND I.ItemCode LIKE '%" + item.Code + "%'";
                    theWhere += " AND I.LongItemName LIKE '%" + item.LongName + "%'";
                    theWhere += " ORDER BY IV.IdItem";

                    itemVasViewDTO = iWarehousingMGR.GetItemVasByAnyParameter(context, null, theWhere);

                    //Recorre el DTO para traspasar los resultados de busqueda a entidad de tipo ITEM para poder cargar la grilla.
                    Int32 lastItem = 0;
                    foreach (ItemVas itemVas in itemVasViewDTO.Entities)
                    {
                        if (lastItem != itemVas.Item.Id)
                        {
                            item = new Item();
                            item = itemVas.Item;
                            itemSearchViewDTO.Entities.Add(item);
                            lastItem = itemVas.Item.Id;
                        }
                    }
                    Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);
                    InitializeGridItems();
                    grdSearchItems.DataSource = itemSearchViewDTO.Entities;
                    grdSearchItems.DataBind();
                }
                else
                {
                    itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwnerFilter(ucFilterItem.FilterItems, context, Convert.ToInt16(ddlOwner.SelectedValue), true);
                    Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);

                    InitializeGridItems();

                    grdSearchItems.DataSource = itemSearchViewDTO.Entities;
                    grdSearchItems.DataBind();
                    isValidViewDTO = true;
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        private void SearchCustomer()
        {
            try
            {
                customerSearchViewDTO = new GenericViewDTO<Customer>();
                int idOwn = 0;
                GenericViewDTO<Customer> CustomerViewDTO = new GenericViewDTO<Customer>();
                Customer customer = new Customer();
                customer.Code = this.txtCustomerCode.Text;
                customer.Name = this.txtCustomerName.Text;

                //if (this.ddlOwner.SelectedValue == "-1")
                //{
                //idOwn = context.SessionInfo.Owner.Id;
                //else
                idOwn = Convert.ToInt32(this.ddlOwner.SelectedValue);

                CustomerViewDTO = iWarehousingMGR.FindByOwnerCodeOwnerName(ucFilterCustomer.FilterItems, context, idOwn);

                //Recorre el DTO para traspasar los resultados de busqueda a entidad de tipo ITEM para poder cargar la grilla.
                foreach (Customer _customer in CustomerViewDTO.Entities)
                {
                    customer = new Customer();
                    customer = _customer;
                    customerSearchViewDTO.Entities.Add(_customer);
                }
                Session.Add(WMSTekSessions.Shared.CustomerList, customerSearchViewDTO);
                //grdMgr.EmptyDataText = this.Master.EmptyGridText;

                InitializeGridCustomer();
                grdSearchCustomers.DataSource = customerSearchViewDTO.Entities;
                grdSearchCustomers.DataBind();
                isValidViewDTO = true;
                //}
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        private void ClearGridItem()
        {
            Session.Remove(WMSTekSessions.Shared.ItemList);
            grdSearchItems.DataSource = null;
            grdSearchItems.DataBind();
        }
        private void ClearGridCustomer()
        {
            Session.Remove(WMSTekSessions.Shared.CustomerList);
            grdSearchCustomers.DataSource = null;
            grdSearchCustomers.DataBind();
        }
        protected void LoadoutboundOrdersDetail(int index)
        {
            int id = outboundOrderViewDTO.Entities[index].Id;

            outboundDetailViewDTO = iDispatchingMGR.GetDetailByIdOutbound(context, id);
            outboundDetails = outboundDetailViewDTO.Entities;

            //agrega a sesion
            Session.Add(WMSTekSessions.OutboundOrderMgr.OutboundDetailList, outboundDetails);

            if (outboundDetails != null && outboundDetails.Count > 0)
            {
                grdItems.DataSource = outboundDetails;
                grdItems.DataBind();
                grdItems.Visible = true;
            }
            else
            {
                grdItems.Visible = false;
            }

            
        }

        private List<int> ValidTracksToEdit()
        {
            return new List<int> { 
                (int)TrackOutboundTypeName.Pending, 
                (int)TrackOutboundTypeName.PendingPlannerApproval, 
                (int)TrackOutboundTypeName.Released, 
                (int)TrackOutboundTypeName.Picking,
                (int)TrackOutboundTypeName.VASB2B,
                (int)TrackOutboundTypeName.Sorting,
                (int)TrackOutboundTypeName.Kitting,
                (int)TrackOutboundTypeName.Packing,
                (int)TrackOutboundTypeName.Vas,
                (int)TrackOutboundTypeName.MovementAnden,
                (int)TrackOutboundTypeName.Loading
            };
        }
        private void EnableControlsWhenUpdate(int idTrack)
        {
            var flag = false;

            if (idTrack == (int)TrackOutboundTypeName.Pending || 
                idTrack == (int)TrackOutboundTypeName.PendingPlannerApproval || 
                idTrack == (int)TrackOutboundTypeName.Released ||
                idTrack == (int)TrackOutboundTypeName.Picking)
            {
                flag = true;
            }

            txtReference.Enabled = flag;
            chkStatus.Enabled = flag;
            chkInmediateProcess.Enabled = flag;
            chkFullShipment.Enabled = flag;
            txtLoadCode.Enabled = flag;
            txtLoadSeq.Enabled = flag;
            txtPriority.Enabled = flag;
            txtEmissionDate.Enabled = flag;

            txtExpectedDate.Enabled = true;

            txtShipmentDate.Enabled = flag;
            txtExpirationDate.Enabled = flag;
            txtCancelDate.Enabled = flag;
            txtCancelUser.Enabled = flag;

            txtCode.Enabled = flag;
            imgbtnSearchItem.Enabled = flag;
            ddlCategoryItem.Enabled = flag;
            txtLotItem.Enabled = flag;
            txtFabricationDateItem.Enabled = flag;
            txtExpirationDateItem.Enabled = flag;
            txtQty.Enabled = flag;
            imgBtnAddItem.Enabled = flag;

            grdItems.Enabled = flag;

            txtCustomerCode.Enabled = flag;
            imgBtnCustmerSearch.Enabled = flag;
            txtCustomerName.Enabled = flag;
            txtDeliveryAddress1.Enabled = flag;
            txtDeliveryAddress2.Enabled = flag;
            ddlCountryDelivery.Enabled = flag;
            ddlStateDelivery.Enabled = flag;
            ddlCityDelivery.Enabled = flag;
            txtDeliveryPhone.Enabled = flag;
            txtDeliveryEmail.Enabled = flag;
            ddlWarehouseTarget.Enabled = flag;
            ddlCarrier.Enabled = flag;
            txtRouteCode.Enabled = flag;
            txtPlate.Enabled = flag;
            ddlBranch.Enabled = flag;

            txtFactAddress1.Enabled = flag;
            txtFactAddress2.Enabled = flag;
            txtInvoice.Enabled = flag;
            ddlCountryFact.Enabled = flag;
            ddlStateFact.Enabled = flag;
            ddlCityFact.Enabled = flag;
            txtFactPhone.Enabled = flag;
            txtFactEmail.Enabled = flag;
        }

        private void ValidateFieldsForSimpliroute(int idOwn, int idWhs)
        {
            var isIntegratedToSimpliroute = base.isIntegratedToSimpliroute(idOwn, idWhs);
            rfvCityDelivery1.Enabled = isIntegratedToSimpliroute;
            rfvDeliveryAddress11.Enabled = isIntegratedToSimpliroute;
            rfvExpectedDate1.Enabled = isIntegratedToSimpliroute;
            rfvDeliveryPhone1.Enabled = isIntegratedToSimpliroute;
            rfvCustomerName1.Enabled = isIntegratedToSimpliroute;
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            tabOutbound.ActiveTabIndex = 0;

            ClearLists();

            this.txtLineComment.Text = string.Empty;

            // Editar Documento
            if (mode == CRUD.Update)
            {
                ValidateFieldsForSimpliroute(outboundOrderViewDTO.Entities[index].Owner.Id, outboundOrderViewDTO.Entities[index].Warehouse.Id);
                // TODO: ver propiedad 'required' para un drop-down list
                IdCountryDelivery = outboundOrderViewDTO.Entities[index].CountryDelivery.Id;
                IdStateDelivery = outboundOrderViewDTO.Entities[index].StateDelivery.Id;
                IdCityDelivery = outboundOrderViewDTO.Entities[index].CityDelivery.Id;

                IdCountryFact = outboundOrderViewDTO.Entities[index].CountryFact.Id;
                IdStateFact = outboundOrderViewDTO.Entities[index].StateFact.Id;
                IdCityFact = outboundOrderViewDTO.Entities[index].CityFact.Id;

                isNew = false;

                ConfigureCountryStateCity();

                //Recupera los datos de la entidad a editar
                hidEditId.Value = outboundOrderViewDTO.Entities[index].Id.ToString();
                hidLatestOutboundTrackId.Value = outboundOrderViewDTO.Entities[index].LatestOutboundTrack.Type.Id.ToString();
                this.divNameTrackOutbound.Visible = true;

                //llamar al objeto detalle
                LoadoutboundOrdersDetail(index);

                //Cargar controles
                this.ddlOwner.SelectedValue = outboundOrderViewDTO.Entities[index].Owner.Id.ToString();
                this.ddlWarehouse.SelectedValue = outboundOrderViewDTO.Entities[index].Warehouse.Id.ToString();
                if (outboundOrderViewDTO.Entities[index].Number != null) this.txtNumOutboundOrder.Text = outboundOrderViewDTO.Entities[index].Number;

                if (outboundOrderViewDTO.Entities[index].OutboundType != null) this.ddlIdOutboundType.SelectedValue = outboundOrderViewDTO.Entities[index].OutboundType.Id.ToString();
                this.chkStatus.Checked = outboundOrderViewDTO.Entities[index].Status;

                if (outboundOrderViewDTO.Entities[index].ReferenceNumber != null) this.txtReference.Text = outboundOrderViewDTO.Entities[index].ReferenceNumber.ToString();
                if (outboundOrderViewDTO.Entities[index].LoadCode != null) this.txtLoadCode.Text = outboundOrderViewDTO.Entities[index].LoadCode.ToString();
                if (outboundOrderViewDTO.Entities[index].LoadSeq != null) this.txtLoadSeq.Text = outboundOrderViewDTO.Entities[index].LoadSeq.ToString();
                this.txtPriority.Text = outboundOrderViewDTO.Entities[index].Priority.ToString();
                this.chkInmediateProcess.Checked = outboundOrderViewDTO.Entities[index].InmediateProcess;


                if (outboundOrderViewDTO.Entities[index].EmissionDate != null && outboundOrderViewDTO.Entities[index].EmissionDate > DateTime.MinValue)
                    this.txtEmissionDate.Text = outboundOrderViewDTO.Entities[index].EmissionDate.ToString("dd-MM-yyyy");
                else
                    this.txtEmissionDate.Text = string.Empty;

                if (outboundOrderViewDTO.Entities[index].ExpectedDate != null && outboundOrderViewDTO.Entities[index].ExpectedDate > DateTime.MinValue)
                    this.txtExpectedDate.Text = outboundOrderViewDTO.Entities[index].ExpectedDate.ToString("dd-MM-yyyy");
                else
                    this.txtExpectedDate.Text = string.Empty;

                if (outboundOrderViewDTO.Entities[index].ShipmentDate != null && outboundOrderViewDTO.Entities[index].ShipmentDate > DateTime.MinValue)
                    this.txtShipmentDate.Text = outboundOrderViewDTO.Entities[index].ShipmentDate.ToString("dd-MM-yyyy");
                else
                    this.txtShipmentDate.Text = string.Empty;

                if (outboundOrderViewDTO.Entities[index].ExpirationDate != null && outboundOrderViewDTO.Entities[index].ExpirationDate > DateTime.MinValue)
                    this.txtExpirationDate.Text = outboundOrderViewDTO.Entities[index].ExpirationDate.ToString("dd-MM-yyyy");
                else
                    this.txtExpirationDate.Text = string.Empty;

                if (outboundOrderViewDTO.Entities[index].CancelDate != null && outboundOrderViewDTO.Entities[index].CancelDate > DateTime.MinValue)
                    this.txtCancelDate.Text = outboundOrderViewDTO.Entities[index].CancelDate.ToString("dd-MM-yyyy");
                else
                    this.txtCancelDate.Text = string.Empty;
                
                if (outboundOrderViewDTO.Entities[index].CancelUser != null) this.txtCancelUser.Text = outboundOrderViewDTO.Entities[index].CancelUser.ToString();
                if (outboundOrderViewDTO.Entities[index].CustomerCode != null)
                {
                    this.txtCustomerCode.Text = outboundOrderViewDTO.Entities[index].CustomerCode.ToString();
                    hidIdCustomer.Value = outboundOrderViewDTO.Entities[index].Customer.Id.ToString();
                }
                else
                    this.txtCustomerCode.Text = string.Empty;
                if (outboundOrderViewDTO.Entities[index].CustomerName != null) 
                    this.txtCustomerName.Text = outboundOrderViewDTO.Entities[index].CustomerName.ToString();
                else
                    this.txtCustomerName.Text = string.Empty;
                if (outboundOrderViewDTO.Entities[index].DeliveryAddress1 != null) this.txtDeliveryAddress1.Text = outboundOrderViewDTO.Entities[index].DeliveryAddress1;
                if (outboundOrderViewDTO.Entities[index].DeliveryAddress2 != null) this.txtDeliveryAddress2.Text = outboundOrderViewDTO.Entities[index].DeliveryAddress2;
                if (outboundOrderViewDTO.Entities[index].CountryDelivery != null) this.ddlCountryDelivery.SelectedValue = (outboundOrderViewDTO.Entities[index].CountryDelivery.Id).ToString();
                if (outboundOrderViewDTO.Entities[index].StateDelivery != null) this.ddlStateDelivery.SelectedValue = (outboundOrderViewDTO.Entities[index].StateDelivery.Id).ToString();
                if (outboundOrderViewDTO.Entities[index].CityDelivery != null) this.ddlCityDelivery.SelectedValue = (outboundOrderViewDTO.Entities[index].CityDelivery.Id).ToString();
                if (outboundOrderViewDTO.Entities[index].DeliveryPhone != null) this.txtDeliveryPhone.Text = outboundOrderViewDTO.Entities[index].DeliveryPhone.ToString();
                if (outboundOrderViewDTO.Entities[index].DeliveryEmail != null) this.txtDeliveryEmail.Text = outboundOrderViewDTO.Entities[index].DeliveryEmail.ToString();
                if ((outboundOrderViewDTO.Entities[index].WarehouseTarget != null) && outboundOrderViewDTO.Entities[index].WarehouseTarget.Id > 0) this.ddlWarehouseTarget.SelectedValue = outboundOrderViewDTO.Entities[index].WarehouseTarget.Id.ToString();
                this.chkFullShipment.Checked = outboundOrderViewDTO.Entities[index].FullShipment;

                if (outboundOrderViewDTO.Entities[index].Carrier.Code != null && this.ddlCarrier.Items.FindByValue(outboundOrderViewDTO.Entities[index].Carrier.Code) != null)
                {
                    this.ddlCarrier.SelectedValue = outboundOrderViewDTO.Entities[index].Carrier.Code;
                }

                if (outboundOrderViewDTO.Entities[index].RouteCode != null) this.txtRouteCode.Text = outboundOrderViewDTO.Entities[index].RouteCode;
                if (outboundOrderViewDTO.Entities[index].Plate != null) this.txtPlate.Text = outboundOrderViewDTO.Entities[index].Plate;
                if (outboundOrderViewDTO.Entities[index].Invoice != null) this.txtInvoice.Text = outboundOrderViewDTO.Entities[index].Invoice;
                if (outboundOrderViewDTO.Entities[index].FactAddress1 != null) this.txtFactAddress1.Text = outboundOrderViewDTO.Entities[index].FactAddress1;
                if (outboundOrderViewDTO.Entities[index].FactAddress2 != null) this.txtFactAddress2.Text = outboundOrderViewDTO.Entities[index].FactAddress2;
                if (outboundOrderViewDTO.Entities[index].CountryFact != null) this.ddlCountryFact.SelectedValue = outboundOrderViewDTO.Entities[index].CountryFact.Id.ToString();
                if (outboundOrderViewDTO.Entities[index].StateFact != null) this.ddlStateFact.SelectedValue = outboundOrderViewDTO.Entities[index].StateFact.Id.ToString();
                if (outboundOrderViewDTO.Entities[index].CityFact != null) this.ddlCityFact.SelectedValue = outboundOrderViewDTO.Entities[index].CityFact.Id.ToString();
                if (outboundOrderViewDTO.Entities[index].FactPhone != null) this.txtFactPhone.Text = outboundOrderViewDTO.Entities[index].FactPhone.ToString();
                if (outboundOrderViewDTO.Entities[index].FactEmail != null) this.txtFactEmail.Text = outboundOrderViewDTO.Entities[index].FactEmail.ToString();
                if (outboundOrderViewDTO.Entities[index].LatestOutboundTrack != null) txtOutboundTrack.Text = outboundOrderViewDTO.Entities[index].LatestOutboundTrack.Type.Name;
                if (outboundOrderViewDTO.Entities[index].Comment != null) this.txtLineComment.Text = outboundOrderViewDTO.Entities[index].Comment;
                if (outboundOrderViewDTO.Entities[index].Branch != null && outboundOrderViewDTO.Entities[index].Branch.Id != -1)
                {
                    hidIdBranch.Value = outboundOrderViewDTO.Entities[index].Branch.Id.ToString();

                    if (outboundOrderViewDTO.Entities[index].Customer != null && outboundOrderViewDTO.Entities[index].Customer.Id != -1)
                    {
                        int? idCityParam = null;
                        if (outboundOrderViewDTO.Entities[index].CityDelivery != null && outboundOrderViewDTO.Entities[index].CityDelivery.Id != -1 )
                        {
                            idCityParam = outboundOrderViewDTO.Entities[index].CityFact.Id;
                        }

                        var listBranches = GetBranches(outboundOrderViewDTO.Entities[index].Owner.Id, outboundOrderViewDTO.Entities[index].Customer.Id, idCityParam);

                        if (listBranches != null && listBranches.Count > 0)
                        {
                            FillDdlBranches(ddlBranch, listBranches);

                            if (ddlBranch.Items.Count > 0)
                            {
                                ddlBranch.SelectedValue = outboundOrderViewDTO.Entities[index].Branch.Id.ToString();
                            }
                        }
                        else
                        {
                            ClearDdlBranches();
                        }
                    }
                    else
                    {
                        ClearDdlBranches();
                    }
                }
                else
                {
                    ClearDdlBranches();
                }

                //Desactivar Detalle del Documento si el track es mayoy a pendiente
                if (outboundOrderViewDTO.Entities[index].LatestOutboundTrack.Type.Id > (int)TrackOutboundTypeName.Pending)
                {
                    this.txtCode.Enabled = false;
                    this.imgbtnSearchItem.Enabled = false;
                    this.ddlCategoryItem.Enabled = false;
                    this.txtQty.Enabled = false;
                    this.imgBtnAddItem.Enabled = false;
                    this.grdItems.Enabled = false;
                }
                else
                {
                    this.txtCode.Enabled = true;
                    this.imgbtnSearchItem.Enabled = true;
                    this.ddlCategoryItem.Enabled = true;
                    this.txtQty.Enabled = true;
                    this.imgBtnAddItem.Enabled = true;
                    this.grdItems.Enabled = true;
                }

                lblNew.Visible = false;
                lblEdit.Visible = true;

                txtLotItem.Text = string.Empty;
                txtFabricationDateItem.Text = string.Empty;
                txtExpirationDateItem.Text = string.Empty;
            }

            // Nuevo Documento
            if (mode == CRUD.Create)
            {
                // Limpia detalles anteriores   
                Session.Remove(WMSTekSessions.OutboundOrderMgr.OutboundDetailList);

                context.MainFilter = this.Master.ucMainFilter.MainFilter;

                //si es nuevo Seteo valores
                IdCountryDelivery = -1;
                IdStateDelivery = -1;
                IdCityDelivery = -1;

                IdCountryFact = -1;
                IdStateFact = -1;
                IdCityFact = -1;

                isNew = true;
                ConfigureCountryStateCity();

                var idWhsSelected = context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues[0].Value;
                var idOwnSelected = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

                // Selecciona Warehouse y Owner seleccionados en el Filtro
                this.ddlWarehouse.SelectedValue = idWhsSelected;
                this.ddlOwner.SelectedValue = idOwnSelected;

                ValidateFieldsForSimpliroute(int.Parse(idOwnSelected), int.Parse(idWhsSelected));

                // Carga listas asociadas al Owner
                base.LoadCategoryItemByOwner(this.ddlCategoryItem, Convert.ToInt32(ddlOwner.SelectedValue), false, true, this.Master.EmptyRowText);

                int outboundTrack = (int)TrackOutboundTypeName.Pending;

                hidEditId.Value = "0";
                hidLatestOutboundTrackId.Value = outboundTrack.ToString();
                this.divNameTrackOutbound.Visible = false;

                this.txtNumOutboundOrder.Text = string.Empty;
                this.ddlIdOutboundType.SelectedValue = "-1";
                this.chkStatus.Checked = true;
                this.txtReference.Text = string.Empty;
                this.txtLoadCode.Text = string.Empty;
                this.txtLoadSeq.Text = string.Empty;
                this.txtPriority.Text = string.Empty;
                this.chkInmediateProcess.Checked = false;
                this.txtEmissionDate.Text = string.Empty;
                this.txtExpectedDate.Text = string.Empty;
                this.txtShipmentDate.Text = string.Empty;
                this.txtExpirationDate.Text = string.Empty;
                this.txtCancelDate.Text = string.Empty;
                this.txtCancelUser.Text = string.Empty;
                this.txtCustomerCode.Text = string.Empty;
                this.txtCustomerName.Text = string.Empty;
                this.txtDeliveryAddress1.Text = string.Empty;
                this.txtDeliveryAddress2.Text = string.Empty;
                this.ddlCountryDelivery.SelectedValue = "-1";
                this.ddlStateDelivery.SelectedValue = "-1";
                this.ddlCityDelivery.SelectedValue = "-1";
                this.txtDeliveryPhone.Text = string.Empty;
                this.txtDeliveryEmail.Text = string.Empty;
                this.ddlWarehouseTarget.SelectedValue = "-1";
                this.chkFullShipment.Checked = false;
                this.ddlCarrier.SelectedValue = "-1";
                this.txtRouteCode.Text = string.Empty;
                this.txtPlate.Text = string.Empty;
                this.txtInvoice.Text = string.Empty;
                this.txtFactAddress1.Text = string.Empty;
                this.txtFactAddress2.Text = string.Empty;
                this.ddlCountryFact.SelectedValue = "-1";
                this.ddlStateFact.SelectedValue = "-1";
                this.ddlCityFact.SelectedValue = "-1";
                this.txtFactPhone.Text = string.Empty;
                this.txtFactEmail.Text = string.Empty;
                this.hidIdBranch.Value = string.Empty;
                this.hidIdCustomer.Value = string.Empty;
                this.ddlBranch.SelectedValue = "-1";

                this.txtCode.Enabled = true;
                this.imgbtnSearchItem.Enabled = true;
                this.ddlCategoryItem.Enabled = true;
                this.txtQty.Enabled = true;
                this.imgBtnAddItem.Enabled = true;
                this.grdItems.Enabled = true;
                txtLotItem.Text = string.Empty;
                txtFabricationDateItem.Text = string.Empty;
                txtExpirationDateItem.Text = string.Empty;

                // Reset detalle del Documento
                outboundDetails = null;
                grdItems.DataSource = outboundDetails;
                grdItems.DataBind();
                
                lblNew.Visible = true;
                lblEdit.Visible = false;

                ClearDdlBranches();
            }

            base.LoadCategoryItemByOwner(this.ddlCategoryItem, Convert.ToInt32(ddlOwner.SelectedValue), true, true, this.Master.EmptyRowText);

            if (outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
            {
                base.ConfigureModal(outboundOrderViewDTO.Configuration, (mode == CRUD.Create ? true : false));
            }
            else
            {
                BaseViewDTO configurationViewDTO = iConfigurationMGR.GetLayoutConfiguration("OutboundOrder_FindAll", context);
                outboundOrderViewDTO.Configuration = configurationViewDTO.Configuration;

                if (outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                    base.ConfigureModal(outboundOrderViewDTO.Configuration, (mode == CRUD.Create ? true : false));

            }

            // La propiedad Outbound Track es de solo lectura
            txtOutboundTrack.Enabled = false;

            divGrid.Visible = false;
            divModal.Visible = true;
            if (mode == CRUD.Update)
            {
                this.ddlOwner.Enabled = false;
                this.ddlWarehouse.Enabled = false;
                this.ddlIdOutboundType.Enabled = false;
                this.txtNumOutboundOrder.Enabled = false;

                divCategoryItem.Visible = true;

                EnableControlsWhenUpdate(outboundOrderViewDTO.Entities[index].LatestOutboundTrack.Type.Id);
            }
            else if (mode == CRUD.Create)
            {
                this.ddlOwner.Enabled = true;
                this.ddlWarehouse.Enabled = true;
                this.ddlIdOutboundType.Enabled = true;
                this.txtNumOutboundOrder.Enabled = true;

                divCategoryItem.Visible = true;
            }
        }

        public void ClearLists()
        {
            this.ddlCountryDelivery.Items.Clear();
            this.ddlStateDelivery.Items.Clear();
            this.ddlCityDelivery.Items.Clear();

            this.ddlCountryFact.Items.Clear();
            this.ddlStateFact.Items.Clear();
            this.ddlCityFact.Items.Clear();
            this.ddlBranch.Items.Clear();
        }

         /// <summary>
        /// Cambia el estado (track) de la Orden a 'Cerrada'
        /// </summary>
        protected void CloseOrder(int index)
        {
            GenericViewDTO<OutboundTrack> outboundTrackViewDTO = new GenericViewDTO<OutboundTrack>();

            outboundTrackViewDTO = iDispatchingMGR.ChangeOutboundOrderTrack(context, TrackOutboundTypeName.Closed, outboundOrderViewDTO.Entities[index]);
            

            if (outboundTrackViewDTO.hasError())
            {
                outboundOrderViewDTO.Errors = outboundTrackViewDTO.Errors;
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(outboundTrackViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        protected void CancelOrder(int index)
        {
            GenericViewDTO<OutboundTrack> outboundTrackViewDTO = new GenericViewDTO<OutboundTrack>();

            outboundTrackViewDTO = iDispatchingMGR.CancelOutboundOrder(context, outboundOrderViewDTO.Entities[index]);

            if (outboundTrackViewDTO.hasError())
            {
                outboundOrderViewDTO.Errors = outboundTrackViewDTO.Errors;
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(outboundTrackViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        protected void PendingOrder(int index)
        {
            GenericViewDTO<OutboundTrack> outboundTrackViewDTO = new GenericViewDTO<OutboundTrack>();

            outboundTrackViewDTO = iDispatchingMGR.ChangeOutboundOrderToPending(context, outboundOrderViewDTO.Entities[index]);

            if (outboundTrackViewDTO.hasError())
            {
                outboundOrderViewDTO.Errors = outboundTrackViewDTO.Errors;
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(outboundTrackViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        protected Int32 OrderBelongsBatch(int idOutboundOrder, int IdOwn)
        {
            GenericViewDTO<TaskDetail> taskDetailViewDTO = new GenericViewDTO<TaskDetail>();
            TaskDetail taskDetail = new TaskDetail();

            taskDetail.IdDocumentBound = idOutboundOrder;
            //taskDetail.Task.TypeCode = "PIKBT";

            Dictionary<string, string> subQuery = new Dictionary<string, string>();
            subQuery.Add("SubQueryCode", "TaskOwnAndListTypeCode");
            subQuery.Add("IdOwn", IdOwn.ToString());
            subQuery.Add("taskTypeCodeList", "PIKBT");             

            taskDetailViewDTO = iTasksMGR.GetTaskDetailByAnyParameter(context, taskDetail, subQuery);

            if (!taskDetailViewDTO.hasError() && taskDetailViewDTO.Entities.Count > 0)
                return taskDetailViewDTO.Entities[0].Task.Id;
            else
                return 0;
        }

        protected OutboundOrder OrderBelongsWave(OutboundOrder outboundOrder)
        {
            int index = 0;

            ContextViewDTO newContext = new ContextViewDTO();
            newContext.MainFilter = new List<EntityFilter>(this.Master.ucMainFilter.MainFilter);
                        
            foreach (EntityFilter entityFilter in newContext.MainFilter)
            {
                entityFilter.FilterValues.Clear();
            }

            index = Convert.ToInt16(EntityFilterName.Warehouse);
            newContext.MainFilter[index].FilterValues.Add(new FilterItem(outboundOrder.Warehouse.Id.ToString()));

            index = Convert.ToInt16(EntityFilterName.Owner);
            newContext.MainFilter[index].FilterValues.Add(new FilterItem(outboundOrder.Owner.Id.ToString()));
                    
            Dictionary<string, string> subQuery = new Dictionary<string, string>();
            subQuery.Add("SubQueryCode", "ExistsDocumentInWaveByIdOutboundOrder");
            subQuery.Add("idOutboundOrder", outboundOrder.Id.ToString());

            GenericViewDTO<OutboundOrder> outboundOrderWave = iDispatchingMGR.FindAllOutboundOrder(newContext, subQuery);

            if (!outboundOrderWave.hasError() && outboundOrderWave.Entities.Count > 0)
                return outboundOrderWave.Entities[0];
            else
                return null;
        }

        protected void SaveChanges()
        {
            // Asigna numero de linea a cada detalle
            if (outboundDetails != null)
            {
                int k = 0;
                foreach (OutboundDetail outboundDetail in outboundDetails)
                {
                    outboundDetail.LineNumber = k;
                    k++;
                }
            }

            if (outboundDetails.Count == 0)
            {
                ucStatus.ShowError("Debe seleccionar al menos un item al documento");
                return;
            }

            OutboundOrder outboundOrder = new OutboundOrder();

            outboundOrder.Warehouse = new Warehouse();
            outboundOrder.WarehouseTarget = new Warehouse();
            outboundOrder.Owner = new Owner();
            outboundOrder.OutboundType = new OutboundType();
            outboundOrder.CountryDelivery = new Country();
            outboundOrder.CountryFact = new Country();
            outboundOrder.StateDelivery = new State();
            outboundOrder.StateFact = new State();
            outboundOrder.CityDelivery = new City();
            outboundOrder.CityFact = new City();
            outboundOrder.Carrier = new Carrier();

            outboundOrder.LatestOutboundTrack = new OutboundTrack();
            outboundOrder.LatestOutboundTrack.Type = new TrackOutboundType();
            outboundOrder.LatestOutboundTrack.OutboundOrder = new OutboundOrder();
            outboundOrder.LatestOutboundTrack.DateTrack = DateTime.Now;

            outboundOrder.Id = Convert.ToInt32(hidEditId.Value);
            
            outboundOrder.LatestOutboundTrack.Type.Id = Convert.ToInt32(hidLatestOutboundTrackId.Value);
            outboundOrder.LatestOutboundTrack.OutboundOrder.Id = outboundOrder.Id;

            outboundOrder.Warehouse.Id = Convert.ToInt32(this.ddlWarehouse.SelectedValue);
            outboundOrder.WarehouseTarget.Id = Convert.ToInt32(this.ddlWarehouseTarget.SelectedValue);
            outboundOrder.Owner.Id = Convert.ToInt32(this.ddlOwner.SelectedValue);
            outboundOrder.Number = this.txtNumOutboundOrder.Text;
            outboundOrder.OutboundType.Id = Convert.ToInt32(this.ddlIdOutboundType.SelectedValue);
            outboundOrder.Status = chkStatus.Checked;
            outboundOrder.ReferenceNumber = this.txtReference.Text;
            outboundOrder.LoadCode = this.txtLoadCode.Text;
            outboundOrder.LoadSeq = this.txtLoadSeq.Text;
            if (!string.IsNullOrEmpty(this.txtPriority.Text)) outboundOrder.Priority = Convert.ToInt32(this.txtPriority.Text);
            outboundOrder.InmediateProcess = this.chkInmediateProcess.Checked;
            if (!string.IsNullOrEmpty(this.txtEmissionDate.Text)) 
                outboundOrder.EmissionDate = Convert.ToDateTime(this.txtEmissionDate.Text);
            else
                outboundOrder.EmissionDate = DateTime.Now;
            if (!string.IsNullOrEmpty(this.txtExpectedDate.Text)) outboundOrder.ExpectedDate = Convert.ToDateTime(this.txtExpectedDate.Text);
            if (!string.IsNullOrEmpty(this.txtShipmentDate.Text)) outboundOrder.ShipmentDate = Convert.ToDateTime(this.txtShipmentDate.Text);
            if (!string.IsNullOrEmpty(this.txtExpirationDate.Text)) outboundOrder.ExpirationDate = Convert.ToDateTime(this.txtExpirationDate.Text);
            if (!string.IsNullOrEmpty(this.txtCancelDate.Text)) outboundOrder.CancelDate = Convert.ToDateTime(this.txtCancelDate.Text);
            outboundOrder.CancelUser = this.txtCancelUser.Text;
            outboundOrder.CustomerCode = this.txtCustomerCode.Text;
            outboundOrder.CustomerName = this.txtCustomerName.Text;
            outboundOrder.DeliveryAddress1 = this.txtDeliveryAddress1.Text;
            outboundOrder.DeliveryAddress2 = this.txtDeliveryAddress2.Text;
            outboundOrder.CountryDelivery.Id = Convert.ToInt32(this.ddlCountryDelivery.SelectedValue);
            outboundOrder.StateDelivery.Id = Convert.ToInt32(this.ddlStateDelivery.SelectedValue);
            outboundOrder.CityDelivery.Id = Convert.ToInt32(this.ddlCityDelivery.SelectedValue);
            outboundOrder.DeliveryPhone = this.txtDeliveryPhone.Text;
            outboundOrder.DeliveryEmail = this.txtDeliveryEmail.Text;
            outboundOrder.FullShipment = this.chkFullShipment.Checked;
            outboundOrder.Carrier.Code = ddlCarrier.SelectedValue == "-1" ? null : ddlCarrier.SelectedValue;
            outboundOrder.RouteCode = this.txtRouteCode.Text;
            outboundOrder.Plate = this.txtPlate.Text;
            outboundOrder.Invoice = this.txtInvoice.Text;
            outboundOrder.FactAddress1 = this.txtFactAddress1.Text;
            outboundOrder.FactAddress2 = this.txtFactAddress2.Text;
            outboundOrder.CountryFact.Id = Convert.ToInt32(this.ddlCountryFact.SelectedValue);
            outboundOrder.StateFact.Id = Convert.ToInt32(this.ddlStateFact.SelectedValue);
            outboundOrder.CityFact.Id = Convert.ToInt32(this.ddlCityFact.SelectedValue);
            outboundOrder.FactPhone = this.txtFactPhone.Text;
            outboundOrder.FactEmail = this.txtFactEmail.Text;

            if (!string.IsNullOrEmpty(txtLineComment.Text))
                outboundOrder.Comment = this.txtLineComment.Text;
            
            outboundOrder.Branch = new Branch();
            //if (!string.IsNullOrEmpty(this.hidIdBranch.Value)) outboundOrder.Branch.Id = int.Parse(hidIdBranch.Value);

            if (ddlBranch.Items.Count > 0)
            {
                outboundOrder.Branch.Id = int.Parse(ddlBranch.SelectedValue);
            }
           
            outboundOrder.OutboundDetails = outboundDetails;

            if (hidEditId.Value == "0")
                outboundOrderViewDTO = iDispatchingMGR.MaintainOutboundOrder(CRUD.Create, outboundOrder, context);
            else
                outboundOrderViewDTO = iDispatchingMGR.MaintainOutboundOrder(CRUD.Update, outboundOrder, context);

            divGrid.Visible = true;
            divModal.Visible = false;

            if (outboundOrderViewDTO.hasError())
            {
                UpdateSession(true);
                divGrid.Visible = false;
                divModal.Visible = true;
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(outboundOrderViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }

            txtCode.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtQty.Text = string.Empty;
        }

        protected void ChangeTrack(int idOutboundOrder, TrackOutboundTypeName track, DateTime dateTrack)
        {
            var outboundTrackViewDTO = iDispatchingMGR.ChangeOutboundOrderTrack(context, track, outboundOrderViewDTO.Entities.Where(doc => doc.Id == idOutboundOrder).First(), dateTrack);

            if (outboundTrackViewDTO.hasError())
            {
                outboundTrackViewDTO.Errors = outboundTrackViewDTO.Errors;
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(outboundTrackViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        private void FillDdlTracks()
        {
            var newContext = new ContextViewDTO();
            newContext.MainFilter = new List<EntityFilter>();

            var trackViewDTO = iDispatchingMGR.FindAllTrackOutboundType(newContext);

            if (!trackViewDTO.hasError() && trackViewDTO.Entities.Count > 0)
            {
                ddlTracks.DataSource = trackViewDTO.Entities.Where(track => track.Id > (int)TrackOutboundTypeName.Shipped && track.Id != (int)TrackOutboundTypeName.Cancel && track.Id != (int)TrackOutboundTypeName.Closed).ToList();
                ddlTracks.DataTextField = "Name";
                ddlTracks.DataValueField = "Id";
                ddlTracks.DataBind();
                ddlTracks.Items.Insert(0, new ListItem("Seleccione", "-1"));
                ddlTracks.Items[0].Selected = true;
            }
            else
            {
                this.Master.ucError.ShowError(trackViewDTO.Errors);
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('OutboundOrder_FindAll', 'ctl00_MainContent_grdMgr', 'OutboundOrderMgr');", true);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        

        protected void btnLoadFile_Click(object sender, EventArgs e)
        {
            string pathAux = "";
            var outboundOrderMassiveViewDTO = new GenericViewDTO<OutboundOrder>();

            try
            {
                string errorUp = "";

                if (uploadFile.HasFile)
                {
                    GenericViewDTO<OutboundOrder> newOutboundOrder = new GenericViewDTO<OutboundOrder>();

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
                        dataTable = ConvertXlsToDataTableHeader(savePath);
                        dataTableDetail = ConvertXlsToDataTableDetail(savePath);
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
                                       OutboundNumber = r.Field<object>("OutboundNumber"),
                                       OutboudTypeCode = r.Field<object>("OutboudTypeCode"),
                                       Status = r.Field<object>("Status"),
                                       ReferenceNumber = r.Field<object>("ReferenceNumber"),
                                       LoadCode = r.Field<object>("LoadCode"),
                                       LoadSeq = r.Field<object>("LoadSeq"),
                                       Priority = r.Field<object>("Priority"),
                                       InmediateProcess = r.Field<object>("InmediateProcess"),
                                       EmissionDate = r.Field<object>("EmissionDate"),
                                       ExpectedDate = r.Field<object>("ExpectedDate"),
                                       ShipmentDate = r.Field<object>("ShipmentDate"),
                                       ExpirationDate = r.Field<object>("ExpirationDate"),
                                       CancelDate = r.Field<object>("CancelDate"),
                                       CancelUser = r.Field<object>("CancelUser"),
                                       CustomerCode = r.Field<object>("CustomerCode"),
                                       CustomerName = r.Field<object>("CustomerName"),
                                       DeliveryAddress1 = r.Field<object>("DeliveryAddress1"),
                                       DeliveryAddress2 = r.Field<object>("DeliveryAddress2"),
                                       CountryDeliveryName = r.Field<object>("CountryDeliveryName"),
                                       StateDeliveryName = r.Field<object>("StateDeliveryName"),
                                       CityDeliveryName = r.Field<object>("CityDeliveryName"),
                                       DeliveryPhone = r.Field<object>("DeliveryPhone"),
                                       DeliveryEmail = r.Field<object>("DeliveryEmail"),
                                       WhsTargetCode = r.Field<object>("WhsTargetCode"),
                                       FullShipment = r.Field<object>("FullShipment"),
                                       CarrierCode = r.Field<object>("CarrierCode"),
                                       RouteCode = r.Field<object>("RouteCode"),
                                       Plate = r.Field<object>("Plate"),
                                       Invoice = r.Field<object>("Invoice"),
                                       FactAddress1 = r.Field<object>("FactAddress1"),
                                       FactAddress2 = r.Field<object>("FactAddress2"),
                                       CountryFactName = r.Field<object>("CountryFactName"),
                                       StateFactName = r.Field<object>("StateFactName"),
                                       CityFactName = r.Field<object>("CityFactName"),
                                       FactPhone = r.Field<object>("FactPhone"),
                                       FactEmail = r.Field<object>("FactEmail"),
                                       AllowCrossDock = r.Field<object>("AllowCrossDock"),
                                       AllowBackOrder = r.Field<object>("AllowBackOrder"),
                                       BranchCode = r.Field<object>("BranchCode"),
                                       Comment = r.Field<object>("Comment"),
                                       SpecialField1 = r.Field<object>("SpecialField1"),
                                       SpecialField2 = r.Field<object>("SpecialField2"),
                                       SpecialField3 = r.Field<object>("SpecialField3"),
                                       SpecialField4 = r.Field<object>("SpecialField4")
                                   };

                    //Detalle
                    var lstDetail = from r in dataTableDetail.AsEnumerable()
                                    select new
                                    {
                                        OutboundNumber = r.Field<object>("OutboundNumber"),
                                        OutboundTypeCode = r.Field<object>("OutboundTypeCode"),
                                        LineNumber = r.Field<object>("LineNumber"),
                                        LineCode = r.Field<object>("LineCode"),
                                        ItemCode = r.Field<object>("ItemCode"),
                                        CtgCode = r.Field<object>("CtgCode"),
                                        ItemQty = r.Field<object>("ItemQty"),
                                        Status = r.Field<object>("Status"),
                                        LotNumber = r.Field<object>("LotNumber"),
                                        FifoDate = r.Field<object>("FifoDate"),
                                        ExpirationDate = r.Field<object>("ExpirationDate"),
                                        FabricationDate = r.Field<object>("FabricationDate"),
                                        GrpClass1 = r.Field<object>("GrpClass1"),
                                        GrpClass2 = r.Field<object>("GrpClass3"),
                                        GrpClass3 = r.Field<object>("GrpClass4"),
                                        GrpClass4 = r.Field<object>("GrpClass5"),
                                        GrpClass5 = r.Field<object>("GrpClass5"),
                                        GrpClass6 = r.Field<object>("GrpClass6"),
                                        GrpClass7 = r.Field<object>("GrpClass7"),
                                        GrpClass8 = r.Field<object>("GrpClass8"),
                                        SpecialField1 = r.Field<object>("SpecialField1"),
                                        SpecialField2 = r.Field<object>("SpecialField2"),
                                        SpecialField3 = r.Field<object>("SpecialField3"),
                                        SpecialField4 = r.Field<object>("SpecialField4")
                                    };


                    GenericViewDTO<Warehouse> warehouseViewDTOLoad = iLayoutMGR.FindAllWarehouse(context);
                    GenericViewDTO<Owner> ownViewDTOLoad = iWarehousingMGR.FindAllOwner(context);
                    GenericViewDTO<OutboundType> outboundTypeViewDTOLoad = iDispatchingMGR.FindAllOutboundType(context);
                    GenericViewDTO<Country> countryViewDTOLoad = iLayoutMGR.FindAllCountry(context);

                    try
                    {
                        foreach (var outOrder in lstExcel)
                        {
                            OutboundOrder newOutOrd = new OutboundOrder();
                            newOutOrd.LatestOutboundTrack = new OutboundTrack();
                            newOutOrd.LatestOutboundTrack.Type = new TrackOutboundType();
                            newOutOrd.LatestOutboundTrack.OutboundOrder = new OutboundOrder();
                            newOutOrd.LatestOutboundTrack.Type.Id = (int)TrackOutboundTypeName.Pending;

                            if (!ValidateIsNotNull(outOrder.OutboundNumber))
                            {
                                errorUp = "OutboundNumber " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newOutOrd.Number = outOrder.OutboundNumber.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(outOrder.WhsCode))
                            {
                                errorUp = "OutboundNumber " + newOutOrd.Number + " - WhsCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (warehouseViewDTOLoad.Entities.Exists(w => w.Code.ToUpper().Equals(outOrder.WhsCode.ToString().ToUpper().Trim())))
                                {
                                    newOutOrd.Warehouse = warehouseViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(outOrder.WhsCode.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El WhsCode " + outOrder.WhsCode + " no es valido para el sistema.";
                                    break;
                                }
                            }

                            if (!ValidateIsNotNull(outOrder.OwnCode))
                            {
                                errorUp = "OutboundNumber " + newOutOrd.Number + " - OwnCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (ownViewDTOLoad.Entities.Exists(w => w.Code.ToUpper().Equals(outOrder.OwnCode.ToString().ToUpper().Trim())))
                                {
                                    newOutOrd.Owner = ownViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(outOrder.OwnCode.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El OwnCode " + outOrder.WhsCode + " no es valido para el sistema.";
                                    break;
                                }
                            }

                            //OutboundType
                            if (!ValidateIsNotNull(outOrder.OutboudTypeCode))
                            {
                                errorUp = "OutboundNumber " + newOutOrd.Number + " - OutboudTypeCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (outboundTypeViewDTOLoad.Entities.Exists(w => w.Code.ToUpper().Equals(outOrder.OutboudTypeCode.ToString().ToUpper().Trim())))
                                {
                                    newOutOrd.OutboundType = outboundTypeViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(outOrder.OutboudTypeCode.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El OutboundTypeCode " + outOrder.OutboudTypeCode + " no es valido para el sistema.";
                                    break;
                                }
                            }

                            if (!ValidateIsNotNull(outOrder.Status))
                            {
                                errorUp = "OutboundNumber " + newOutOrd.Number + " - Status " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newOutOrd.Status = outOrder.Status.ToString().Equals("1") ? true : false;
                            }

                            if (ValidateIsNotNull(outOrder.ReferenceNumber))
                                newOutOrd.ReferenceNumber = outOrder.ReferenceNumber.ToString().Trim();

                            if (ValidateIsNotNull(outOrder.LoadCode))
                                newOutOrd.LoadCode = outOrder.LoadCode.ToString().Trim();
                                                 
                            if (ValidateIsNotNull(outOrder.LoadSeq))
                                newOutOrd.LoadSeq = outOrder.LoadSeq.ToString().Trim();
                                       
                            if (ValidateIsNotNull(outOrder.Priority))
                                newOutOrd.Priority = Convert.ToInt32(outOrder.Priority.ToString().Trim());

                            if (ValidateIsNotNull(outOrder.InmediateProcess))
                                newOutOrd.InmediateProcess = outOrder.InmediateProcess.ToString().Equals("1") ? true : false;


                            if (ValidateIsNotNull(outOrder.EmissionDate))
                                newOutOrd.EmissionDate = Convert.ToDateTime(outOrder.EmissionDate);

                            if (ValidateIsNotNull(outOrder.ExpectedDate))
                                newOutOrd.ExpectedDate = Convert.ToDateTime(outOrder.ExpectedDate);

                            if (ValidateIsNotNull(outOrder.ShipmentDate))
                                newOutOrd.ShipmentDate = Convert.ToDateTime(outOrder.ShipmentDate);

                            if (ValidateIsNotNull(outOrder.ExpirationDate))
                                newOutOrd.ExpirationDate = Convert.ToDateTime(outOrder.ExpirationDate);

                            if (ValidateIsNotNull(outOrder.CancelDate))
                                newOutOrd.CancelDate = Convert.ToDateTime(outOrder.CancelDate);

                            if (ValidateIsNotNull(outOrder.CancelUser))
                                newOutOrd.CancelUser = outOrder.CancelUser.ToString().Trim();

                            if (ValidateIsNotNull(outOrder.CustomerCode))
                            {
                                GenericViewDTO<Customer> CustomerViewDTO = new GenericViewDTO<Customer>(); 
                                CustomerViewDTO = iWarehousingMGR.GetCustomerByCodeAndOwn(context, outOrder.CustomerCode.ToString(), newOutOrd.Owner.Id);

                                if (CustomerViewDTO.Entities != null && CustomerViewDTO.Entities.Count > 0)
                                {
                                    newOutOrd.Customer = CustomerViewDTO.Entities[0];
                                    newOutOrd.CustomerCode = outOrder.CustomerCode.ToString().Trim();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El CustomerCode " + outOrder.CustomerCode + " no es valido para el sistema.";
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(outOrder.CustomerName))
                                newOutOrd.CustomerName = outOrder.CustomerName.ToString().Trim();

                            if (ValidateIsNotNull(outOrder.DeliveryAddress1))
                            {
                                var deliveryAddress1 = outOrder.DeliveryAddress1.ToString().Trim();

                                if (deliveryAddress1.Length > 60)
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - La dirección de entrega 1: " + deliveryAddress1 + " tiene demasiados caracteres. El máximo son 60.";
                                    break;
                                }

                                newOutOrd.DeliveryAddress1 = deliveryAddress1;
                            }                              

                            if (ValidateIsNotNull(outOrder.DeliveryAddress2))
                            {
                                var deliveryAddress2 = outOrder.DeliveryAddress2.ToString().Trim();

                                if (deliveryAddress2.Length > 60)
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - La dirección de entrega 2: " + deliveryAddress2 + " tiene demasiados caracteres. El máximo son 60.";
                                    break;
                                }

                                newOutOrd.DeliveryAddress2 = deliveryAddress2;
                            }

                            if (ValidateIsNotNull(outOrder.CountryDeliveryName))
                            {
                                if (countryViewDTOLoad.Entities.Exists(w => w.Name.ToUpper().Equals(outOrder.CountryDeliveryName.ToString().ToUpper().Trim())))
                                {
                                    newOutOrd.CountryDelivery = countryViewDTOLoad.Entities.Where(w => w.Name.ToUpper().Equals(outOrder.CountryDeliveryName.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El CountryDeliveryName " + outOrder.CountryDeliveryName + " no es valido para el sistema.";
                                    break;
                                }
                            }
                            else
                            {
                                newOutOrd.CountryDelivery = new Country(-1);
                            }

                            if (ValidateIsNotNull(outOrder.StateDeliveryName))
                            {
                                GenericViewDTO<State> stateViewDTOLoad = iLayoutMGR.GetStateByCountry(newOutOrd.CountryDelivery.Id, context);

                                if (stateViewDTOLoad.Entities.Exists(w => w.Name.ToUpper().Equals(outOrder.StateDeliveryName.ToString().ToUpper().Trim())))
                                {
                                    newOutOrd.StateDelivery = stateViewDTOLoad.Entities.Where(w => w.Name.ToUpper().Equals(outOrder.StateDeliveryName.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El StateDeliveryName " + outOrder.StateDeliveryName + " no es valido para el sistema.";
                                    break;
                                }
                            }
                            else
                            {
                                newOutOrd.StateDelivery = new State(-1);
                            }

                            if (ValidateIsNotNull(outOrder.CityDeliveryName))
                            {
                                GenericViewDTO<City> cityViewDTOLoad = iLayoutMGR.GetCityByStateAndCountry(newOutOrd.StateDelivery.Id, newOutOrd.CountryDelivery.Id, context);

                                if (cityViewDTOLoad.Entities.Exists(w => w.Name.ToUpper().Equals(outOrder.CityDeliveryName.ToString().ToUpper().Trim())))
                                {
                                    newOutOrd.CityDelivery = cityViewDTOLoad.Entities.Where(w => w.Name.ToUpper().Equals(outOrder.CityDeliveryName.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El CityDeliveryName " + outOrder.CityDeliveryName + " no es valido para el sistema.";
                                    break;
                                }
                            }
                            else
                            {
                                newOutOrd.CityDelivery = new City(-1);
                            }

                            if (ValidateIsNotNull(outOrder.DeliveryPhone))
                                newOutOrd.DeliveryPhone = outOrder.DeliveryPhone.ToString().ToUpper().Trim();

                            if (ValidateIsNotNull(outOrder.DeliveryEmail))
                                newOutOrd.DeliveryEmail = outOrder.DeliveryEmail.ToString().ToUpper().Trim();

                            if (ValidateIsNotNull(outOrder.WhsTargetCode))
                            {
                                if (warehouseViewDTOLoad.Entities.Exists(w => w.Code.ToUpper().Equals(outOrder.WhsTargetCode.ToString().ToUpper().Trim())))
                                {
                                    newOutOrd.WarehouseTarget = warehouseViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(outOrder.WhsTargetCode.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El WhsTargetCode " + outOrder.WhsTargetCode + " no es valido para el sistema.";
                                    break;
                                }
                            }
                            else
                            {
                                newOutOrd.WarehouseTarget = new Warehouse();
                            }

                            if (ValidateIsNotNull(outOrder.FullShipment))
                            {
                                newOutOrd.FullShipment = outOrder.FullShipment.ToString().Equals("1") ? true : false;
                            }

                            if (ValidateIsNotNull(outOrder.CarrierCode))
                            {
                                GenericViewDTO<Carrier> carrierViewDTO = iWarehousingMGR.GetCarrierByCode(context, outOrder.CarrierCode.ToString().Trim());

                                if (carrierViewDTO.Entities !=null && carrierViewDTO.Entities.Count>0)
                                {
                                    newOutOrd.Carrier = carrierViewDTO.Entities[0];
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El CarrierCode " + outOrder.CarrierCode + " no es valido para el sistema.";
                                    break;
                                }
                            }
                            else
                            {
                                newOutOrd.Carrier = new Carrier();
                            }

                            if (ValidateIsNotNull(outOrder.RouteCode))
                                newOutOrd.RouteCode = outOrder.RouteCode.ToString().ToUpper().Trim();

                            if (ValidateIsNotNull(outOrder.Plate))
                                newOutOrd.Plate = outOrder.Plate.ToString().ToUpper().Trim();

                            if (ValidateIsNotNull(outOrder.Invoice))
                                newOutOrd.Invoice = outOrder.Invoice.ToString().ToUpper().Trim();

                            if (ValidateIsNotNull(outOrder.FactAddress1))
                            {
                                var factAddress1 = outOrder.FactAddress1.ToString().Trim();

                                if (factAddress1.Length > 60)
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - La dirección de facturación 1: " + factAddress1 + " tiene demasiados caracteres. El máximo son 60.";
                                    break;
                                }

                                newOutOrd.FactAddress1 = factAddress1;
                            }

                            if (ValidateIsNotNull(outOrder.FactAddress2))
                            {
                                var factAddress2 = outOrder.FactAddress2.ToString().Trim();

                                if (factAddress2.Length > 60)
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - La dirección de facturación 2: " + factAddress2 + " tiene demasiados caracteres. El máximo son 60.";
                                    break;
                                }

                                newOutOrd.FactAddress2 = factAddress2;
                            }
                            
                            if (ValidateIsNotNull(outOrder.CountryFactName))
                            {
                                if (countryViewDTOLoad.Entities.Exists(w => w.Name.ToUpper().Equals(outOrder.CountryFactName.ToString().ToUpper().Trim())))
                                {
                                    newOutOrd.CountryFact = countryViewDTOLoad.Entities.Where(w => w.Name.ToUpper().Equals(outOrder.CountryFactName.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El CountryFactName " + outOrder.CountryFactName + " no es valido para el sistema.";
                                    break;
                                }
                            }
                            else
                            {
                                newOutOrd.CountryFact = new Country(-1);
                            }

                            if (ValidateIsNotNull(outOrder.StateFactName))
                            {
                                GenericViewDTO<State> stateViewDTOLoad = iLayoutMGR.GetStateByCountry(newOutOrd.CountryFact.Id, context);

                                if (stateViewDTOLoad.Entities.Exists(w => w.Name.ToUpper().Equals(outOrder.StateFactName.ToString().ToUpper().Trim())))
                                {
                                    newOutOrd.StateFact = stateViewDTOLoad.Entities.Where(w => w.Name.ToUpper().Equals(outOrder.StateFactName.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El StateFactName " + outOrder.StateFactName + " no es valido para el sistema.";
                                    break;
                                }
                            }
                            else
                            {
                                newOutOrd.StateFact = new State(-1);
                            }

                            if (ValidateIsNotNull(outOrder.CityFactName))
                            {
                                GenericViewDTO<City> cityViewDTOLoad = iLayoutMGR.GetCityByStateAndCountry(newOutOrd.StateFact.Id, newOutOrd.CountryFact.Id, context);

                                if (cityViewDTOLoad.Entities.Exists(w => w.Name.ToUpper().Equals(outOrder.CityFactName.ToString().ToUpper().Trim())))
                                {
                                    newOutOrd.CityFact = cityViewDTOLoad.Entities.Where(w => w.Name.ToUpper().Equals(outOrder.CityFactName.ToString().ToUpper().Trim())).First();
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + " - El CityFactyName " + outOrder.CityFactName + " no es valido para el sistema.";
                                    break;
                                }
                            }
                            else
                            {
                                newOutOrd.CityFact = new City(-1);
                            }

                            if (ValidateIsNotNull(outOrder.FactPhone))
                                newOutOrd.FactPhone = outOrder.FactPhone.ToString().ToUpper().Trim();

                            if (ValidateIsNotNull(outOrder.FactEmail))
                                newOutOrd.FactEmail = outOrder.FactEmail.ToString().ToUpper().Trim();

                            if (ValidateIsNotNull(outOrder.AllowCrossDock))
                                newOutOrd.AllowCrossDock = outOrder.AllowCrossDock.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(outOrder.AllowBackOrder))
                                newOutOrd.AllowBackOrder = outOrder.AllowBackOrder.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(outOrder.BranchCode))
                            {
                                if (ValidateIsNotNull(outOrder.CustomerCode))
                                {
                                    GenericViewDTO<Branch> branchViewDTOLoad = iWarehousingMGR.GetBranchByIdCustomer(context, newOutOrd.Customer.Id);

                                    if (branchViewDTOLoad.Entities.Exists(w => w.Code.ToUpper().Equals(outOrder.BranchCode.ToString().ToUpper().Trim())))
                                    {
                                        newOutOrd.Branch = branchViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(outOrder.BranchCode.ToString().ToUpper().Trim())).First();
                                    }
                                    else
                                    {
                                        errorUp = "OutboundNumber " + newOutOrd.Number + " - El BranchCode " + outOrder.BranchCode + " no es valido para el sistema.";
                                        break;
                                    }
                                }
                                else
                                {
                                    errorUp = "OutboundNumber " + newOutOrd.Number + "- El CustomerCode No puede ser nulo si existe un BranchCode.";
                                    break;
                                }
                            }
                            else
                            {
                                newOutOrd.Branch = new Branch();
                            }

                            if (ValidateIsNotNull(outOrder.SpecialField1))
                                newOutOrd.SpecialField1 = outOrder.SpecialField1.ToString().Trim();

                            if (ValidateIsNotNull(outOrder.SpecialField2))
                                newOutOrd.SpecialField2 = outOrder.SpecialField2.ToString().Trim();

                            if (ValidateIsNotNull(outOrder.SpecialField3))
                                newOutOrd.SpecialField3 = outOrder.SpecialField3.ToString().Trim();

                            if (ValidateIsNotNull(outOrder.SpecialField4))
                                newOutOrd.SpecialField4 = outOrder.SpecialField4.ToString().Trim();

                            if (ValidateIsNotNull(outOrder.Comment))
                                newOutOrd.Comment = outOrder.Comment.ToString().Trim();

                            //Valida que existan Detalles
                            if (lstDetail.ToList().Exists(w => w.OutboundNumber.ToString().ToUpper().Trim().Equals(outOrder.OutboundNumber.ToString().ToUpper().Trim()) &&
                            w.OutboundTypeCode.ToString().ToUpper().Trim().Equals(outOrder.OutboudTypeCode.ToString().ToUpper().Trim())))
                            {
                                var listExistItemDetail = lstDetail.ToList().Where(w => w.OutboundNumber.ToString().ToUpper().Trim().Equals(outOrder.OutboundNumber.ToString().ToUpper().Trim()) &&
                                                                                w.OutboundTypeCode.ToString().ToUpper().Trim().Equals(outOrder.OutboudTypeCode.ToString().ToUpper().Trim()));

                                var repeteatedItems = listExistItemDetail.GroupBy(od => new
                                {
                                    ItemCode = od.ItemCode.ToString(),
                                    LotNumber = od.LotNumber.ToString(),
                                    CtgCode = od.CtgCode.ToString(),
                                    FabricationDate = od.FabricationDate.ToString(),
                                    ExpirationDate = od.ExpirationDate.ToString()
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
                                    errorUp = "OutboundNumber " + newOutOrd.Number + "- Detalle Documento. " + this.lblValidateRepeatedItems.Text;
                                    break;
                                }

                                newOutOrd.OutboundDetails = new List<OutboundDetail>();

                                foreach (var outDet in listExistItemDetail)
                                {
                                    OutboundDetail newOutDet = new OutboundDetail();
                                    newOutDet.OutboundOrder = new OutboundOrder();

                                    int idOwnLoadDet = ownViewDTOLoad.Entities.Where(w => w.Code.ToUpper().Equals(outOrder.OwnCode.ToString().ToUpper().Trim())).First().Id;

                                    if (!ValidateIsNotNull(outDet.LineNumber))
                                    {
                                        errorUp = "OutboundNumber " + newOutOrd.Number + " - Detalle Documento, LineNumber " + this.lblFieldNotNull.Text;
                                        break;
                                    }
                                    else
                                    {
                                        newOutDet.LineNumber = Convert.ToInt32(outDet.LineNumber.ToString().Trim());
                                    }

                                    if (ValidateIsNotNull(outDet.LineCode))
                                        newOutDet.LineCode = outDet.LineCode.ToString().Trim();
                                                                        
                                    //Item
                                    GenericViewDTO<Item> itemViewDTOLoad = iWarehousingMGR.GetItemByCodeAndOwner(context, outDet.ItemCode.ToString().Trim(), idOwnLoadDet, false);

                                    if (!ValidateIsNotNull(outDet.ItemCode))
                                    {
                                        errorUp = "OutboundNumber " + newOutOrd.Number + " Linea " + newOutDet.LineNumber.ToString() + " - Detalle Documento, ItemCode " + this.lblFieldNotNull.Text;
                                        break;
                                    }
                                    else
                                    {
                                        if (itemViewDTOLoad.Entities != null && itemViewDTOLoad.Entities.Count > 0)
                                        {
                                            newOutDet.Item = itemViewDTOLoad.Entities[0];
                                        }
                                        else
                                        {
                                            errorUp = "OutboundNumber " + newOutOrd.Number + " Linea " + newOutDet.LineNumber.ToString() + " - Detalle Documento, ItemCode " + outDet.ItemCode + " no es valido para el sistema.";
                                            break;
                                        }
                                    }

                                    //Categoria
                                    if (ValidateIsNotNull(outDet.CtgCode))
                                    {

                                        GenericViewDTO<CategoryItem> categoryItemViewDTOLoad = iWarehousingMGR.GetCategoryItemByCodeAndOwner(outDet.CtgCode.ToString().Trim(), idOwnLoadDet, context);

                                        if (categoryItemViewDTOLoad.Entities != null && categoryItemViewDTOLoad.Entities.Count > 0)
                                        {
                                            newOutDet.CategoryItem = categoryItemViewDTOLoad.Entities[0];
                                        }
                                        else
                                        {
                                            errorUp = "OutboundNumber " + newOutOrd.Number + " Linea " + newOutDet.LineNumber.ToString() + " - Detalle Documento, CtgCode " + outDet.CtgCode + " no es valido para el sistema.";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        newOutDet.CategoryItem = new CategoryItem();
                                    }

                                    if (!ValidateIsNotNull(outDet.ItemQty))
                                    {
                                        errorUp = "OutboundNumber " + newOutOrd.Number + " Linea " + newOutDet.LineNumber.ToString() + " - Detalle Documento, ItemQty " + this.lblFieldNotNull.Text;
                                        break;
                                    }
                                    else
                                    {
                                        decimal qty = 0;
                                        var isQtyDecimal = decimal.TryParse(outDet.ItemQty.ToString(), out qty);

                                        if (isQtyDecimal)
                                        {
                                            if (qty <= 0)
                                            {
                                                errorUp = "OutboundNumber " + newOutOrd.Number + " Linea " + newOutDet.LineNumber.ToString() + " - Detalle Documento, ItemQty " + this.lblValidateQty.Text;
                                                break; 
                                            }
                                            else
                                            {
                                                newOutDet.ItemQty = qty;
                                            }
                                        }    
                                    }

                                    if (!ValidateIsNotNull(outDet.Status))
                                    {
                                        errorUp = "OutboundNumber " + newOutOrd.Number + " Linea " + newOutDet.LineNumber.ToString() + " - Detalle Documento, Status " + this.lblFieldNotNull.Text;
                                        break;
                                    }
                                    else
                                    {
                                        newOutDet.Status = outDet.Status.ToString().Equals("1") ? true : false;
                                    }

                                    
                                    if (ValidateIsNotNull(outDet.LotNumber))
                                        newOutDet.LotNumber = outDet.LotNumber.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.FifoDate))
                                        newOutDet.FifoDate = Convert.ToDateTime(outDet.FifoDate);

                                    if (ValidateIsNotNull(outDet.ExpirationDate))
                                        newOutDet.ExpirationDate = Convert.ToDateTime(outDet.ExpirationDate);

                                    if (ValidateIsNotNull(outDet.FabricationDate))
                                        newOutDet.FabricationDate = Convert.ToDateTime(outDet.FabricationDate);
                                    
                                    if (ValidateIsNotNull(outDet.GrpClass1))
                                        newOutDet.GrpClass1 = outDet.GrpClass1.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.GrpClass2))
                                        newOutDet.GrpClass2 = outDet.GrpClass2.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.GrpClass3))
                                        newOutDet.GrpClass3 = outDet.GrpClass3.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.GrpClass4))
                                        newOutDet.GrpClass4 = outDet.GrpClass4.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.GrpClass5))
                                        newOutDet.GrpClass5 = outDet.GrpClass5.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.GrpClass6))
                                        newOutDet.GrpClass6 = outDet.GrpClass6.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.GrpClass7))
                                        newOutDet.GrpClass7 = outDet.GrpClass7.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.GrpClass8))
                                        newOutDet.GrpClass8 = outDet.GrpClass8.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.SpecialField1))
                                        newOutDet.SpecialField1 = outDet.SpecialField1.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.SpecialField2))
                                        newOutDet.SpecialField2 = outDet.SpecialField2.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.SpecialField3))
                                        newOutDet.SpecialField3 = outDet.SpecialField3.ToString().Trim();

                                    if (ValidateIsNotNull(outDet.SpecialField4))
                                        newOutDet.SpecialField4 = outDet.SpecialField4.ToString().Trim();


                                    newOutOrd.OutboundDetails.Add(newOutDet);
                                }

                            }
                            else
                            {
                                errorUp = "El Nro Documento " + outOrder.OutboundNumber + " Tipo " + outOrder.OutboudTypeCode + ", NO cuenta con Detalles.";
                                break;
                            }

                            outboundOrderMassiveViewDTO.Entities.Add(newOutOrd);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }

                    if (errorUp != "")
                    {
                        ShowAlertLocal(this.lblTitle.Text, errorUp.Trim());
                        //divFondoPopupProgress.Visible = false;
                        divLoad.Visible = false;
                        modalPopUpLoad.Hide();
                    }
                    else
                    {
                        if (outboundOrderMassiveViewDTO.Entities.Count > 0)
                        {
                            outboundOrderMassiveViewDTO = iDispatchingMGR.MaintainOutboundOrderMassive(outboundOrderMassiveViewDTO, context);

                            if (outboundOrderMassiveViewDTO.hasError())
                            {
                                //UpdateSession(true);
                                ShowAlertLocal(outboundOrderMassiveViewDTO.Errors.Title, outboundOrderMassiveViewDTO.Errors.Message);
                                //divFondoPopupProgress.Visible = false;
                                divLoad.Visible = false;
                                modalPopUpLoad.Hide();
                            }
                            else
                            {
                                ucStatus.ShowMessage(outboundOrderMassiveViewDTO.MessageStatus.Message);
                                ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                //divFondoPopupProgress.Visible = false;
                                divLoad.Visible = false;
                                modalPopUpLoad.Hide();
                            }
                        }
                        else
                        {
                            ShowAlertLocal(this.lblTitle.Text, this.lblNotInboundOrderFile.Text);
                            //divFondoPopupProgress.Visible = false;
                            divLoad.Visible = false;
                            modalPopUpLoad.Hide();
                        }
                    }

                }
                else
                {
                    ShowAlertLocal(this.lblTitle.Text, this.lblNotFileLoad.Text);
                    //divFondoPopupProgress.Visible = false;
                    divLoad.Visible = false;
                    modalPopUpLoad.Hide();
                }

            }
            catch (InvalidDataException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                //divFondoPopupProgress.Visible = false;
                divLoad.Visible = false;
                modalPopUpLoad.Hide();
            }
            catch (InvalidOperationException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                //divFondoPopupProgress.Visible = false;
                divLoad.Visible = false;
                modalPopUpLoad.Hide();
            }
            catch (Exception ex)
            {
                outboundOrderMassiveViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, outboundOrderMassiveViewDTO.Errors.Message);
            }
            finally
            {
                //Pregunta si existe el archivo y lo elimina
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }
            }
        }

        private DataTable ConvertXlsToDataTableHeader(string fileXML)
        {
            DataTable dt = new DataTable();

            using (XLWorkbook workBook = new XLWorkbook(fileXML))
            {
                //Read the first Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(1);

                //Loop through the Worksheet rows.
                int columnMax = 0;
                bool firstRow = true;
                foreach (IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.

                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            if (cell.Value.ToString().Equals(String.Empty))
                            {
                                dt.Columns.Add("NULL");
                            }
                            else
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            columnMax++;
                        }
                        firstRow = false;
                    }
                    else
                    {
                        //Add rows to DataTable.
                        int i = 0;
                        int count = 0;
                        // verifica si toda la fila esta vacia
                        for (int j = 1; j <= columnMax; j++)
                        {
                            if (row.Cell(j).Value.ToString().Equals(String.Empty))
                            {
                                count++;
                            }
                        }
                        //si toda la fila esta vacia salta la fila
                        if (count >= columnMax)
                            continue;
                        for (int j = 1; j <= columnMax; j++)
                        {
                            if (i == 0) { dt.Rows.Add(); }

                            if (row.Cell(j).Value.ToString().Equals(String.Empty))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = "NULL";
                            }
                            else
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = row.Cell(j).Value.ToString();
                            }
                            i++;
                        }
                    }             
                }
            }

            return dt;
        }

        private DataTable ConvertXlsToDataTableDetail(string fileXML)
        {
            DataTable dt = new DataTable();

            using (XLWorkbook workBook = new XLWorkbook(fileXML))
            {
                //Read the second Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(2);

                //Loop through the Worksheet rows.
                int columnMax = 0;
                bool firstRow = true;
                foreach (IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.

                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            if (cell.Value.ToString().Equals(String.Empty))
                            {
                                dt.Columns.Add("NULL");
                            }
                            else
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            columnMax++;
                        }
                        firstRow = false;
                    }
                    else
                    {
                        //Add rows to DataTable.
                        int i = 0;
                        int count = 0;
                        // verifica si toda la fila esta vacia
                        for (int j = 1; j <= columnMax; j++)
                        {
                            if (row.Cell(j).Value.ToString().Equals(String.Empty))
                            {
                                count++;
                            }
                        }
                        //si toda la fila esta vacia salta la fila
                        if (count >= columnMax)
                            continue;
                        for (int j = 1; j <= columnMax; j++)
                        {
                            if (i == 0) { dt.Rows.Add(); }

                            if (row.Cell(j).Value.ToString().Equals(String.Empty))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = "NULL";
                            }
                            else
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = row.Cell(j).Value.ToString();
                            }
                            i++;
                        }
                    }

                    //if (firstRow)
                    //{
                    //    foreach (IXLCell cell in row.Cells())
                    //    {
                    //        dt.Columns.Add(cell.Value.ToString());
                    //    }
                    //    firstRow = false;
                    //}
                    //else
                    //{
                    //    //Add rows to DataTable.
                    //    dt.Rows.Add();
                    //    int i = 0;
                    //    foreach (IXLCell cell in row.Cells())
                    //    {
                    //        dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                    //        i++;
                    //    }
                    //}

                }
            }

            return dt;
        }

        public void ShowAlertLocal(string title, string message)
        {
            string script = "ShowMessage('" + title + "','" + message + "');";
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }

        private bool ValidateIsNotNull(object field)
        {
            bool result = true;

            if (field == null)
            {
                result = false;
            }
            else if (field.ToString() == "")
            {
                result = false;
            }
            else if (field.ToString() == "NULL")
            {
                result = false;
            }

            return result;
        }


        #endregion

        
    }
}