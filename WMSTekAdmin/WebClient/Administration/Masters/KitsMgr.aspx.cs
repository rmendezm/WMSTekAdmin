using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Display;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.IO;
using System.Data.OleDb;
using System.Data;

namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class KitsMgr : BasePage
    {
        #region "Declaración de Variables"

        GenericViewDTO<Kit> kitViewDTO = new GenericViewDTO<Kit>();
        private GenericViewDTO<Kit> kitDetailViewDTO = new GenericViewDTO<Kit>();
        private GenericViewDTO<Item> itemSearchViewDTO = new GenericViewDTO<Item>();
        private bool isValidViewDTO = false;
        private Kit kit = new Kit();
        private bool isRepeated = false;

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
        /// <summary>
        /// Propiedad para controlar el indice activo en la grilla
        /// </summary>
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

        /// <summary>
        /// Propiedad que identifica si se esta trabajando con el Kit o con su detalle
        /// </summary>
        public bool isMgrKit
        {
            get
            {
                if (ValidateViewState("isMgrKit"))
                    return (bool)ViewState["isMgrKit"];
                else
                    return true;
            }

            set { ViewState["isMgrKit"] = value; }
        }
        /// <summary>
        /// Propiedad que identifica si se está creando un nuevo Kit
        /// </summary>
        public bool isNewKit
        {
            get
            {
                if (ValidateViewState("isNewKit"))
                    return (bool)ViewState["isNewKit"];
                else
                    return false;
            }

            set { ViewState["isNewKit"] = value; }
        }
        /// <summary>
        /// Propiedad que identifica si se esta editando o no un detalle del kit
        /// </summary>
        public bool editMode
        {
            get
            {
                if (ValidateViewState("editMode"))
                    return (bool)ViewState["editMode"];
                else
                    return false;
            }

            set { ViewState["editMode"] = value; }
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

                    //if (!Page.IsPostBack)
                    //{
                    //    // Carga inicial del ViewDTO
                    //    UpdateSession(false);
                    //}

                    if (ValidateSession(WMSTekSessions.KitMgr.KitList))
                    {
                        kitViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitList];
                        isValidViewDTO = true;
                    }
                }
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                        LoadDetailByIdKit();
                    }
                }
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
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
                        LoadDetailByIdKit();
                    }
                }

                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        /// <summary>
        /// Busca Items para agregar al detalle (solo debe buscar items que no esten en la tabla kit)
        /// </summary>
        protected void imgBtnSearchItem_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                bool validItem = false;
                bool existingItem = false;
                pnlError.Visible = false;
                isMgrKit = false;

                //Lo primero es verificar que tiene un Owner seleccionado
                //DropDownList ddlown = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
                int intOwn = kitViewDTO.Entities[currentIndex].ItemKit.Owner.Id;

                if (intOwn != 0)
                {
                    this.lblErrorNoOwn.Visible = false;

                    // Busca en base de datos el Item ingresado 
                    if (txtCode.Text.Trim() != string.Empty)
                    {
                        //Consulta los items
                        itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwnNotInKit(context, txtCode.Text.Trim(), intOwn);

                        if (itemSearchViewDTO.Entities != null)
                        {
                            Item item = new Item();

                            //Verifica si la busqueda es exitosa (trae un solo item) para agregarlo a los controles de detalle
                            if (itemSearchViewDTO.Entities.Count == 1)
                            {
                                validItem = true;
                                item = itemSearchViewDTO.Entities[0];

                                // Mantiene en memoria los datos del Item a agregar
                                Session.Add("kitDetailNewItem", item);

                                kitDetailViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitDetailList];

                                if (kitDetailViewDTO != null)
                                {
                                    // Recorre los items ya agregados y compara con el que se quiere agregar
                                    if (kitDetailViewDTO.Entities != null && kitDetailViewDTO.Entities.Count > 0)
                                    {
                                        foreach (Kit kit in kitDetailViewDTO.Entities)
                                        {
                                            // Si ya existe en la lista se marca
                                            if (kit.ItemBase.Code == item.Code)
                                            {
                                                existingItem = true;
                                                pnlError.Visible = false;
                                            }
                                        }
                                    }
                                }

                                // Si no fue agregado, agrega el item 
                                if (!existingItem)
                                {
                                    this.txtCode.Text = item.Code;
                                    this.txtDescription.Text = item.Description;
                                }
                                else
                                {
                                    pnlError.Visible = true;
                                }
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
                            SearchItemKit();
                        }
                        // Si no se ingresó ningún item, no se ejecuta la búsqueda
                        else
                            ClearGridItem();

                        // Esto evita un bug de ajax
                        valAddItem.Enabled = false;
                        valSearchItem.Enabled = false;

                        this.lblAddItem.Text = lblNewDetail.Text;

                        mpLookupItem.Show();

                        ucFilterItem.Visible = false;//Esconde los controles de busqueda de Item incluidos los Kit
                        ucFilterItem2.Visible = true; //Muestra los controles de busqueda de Item sin items de tipo Kit

                        InitializePageCountItems();
                    }
                }
                else
                {
                    //Muestra mensaje que debe seleccionar un Owner
                    this.lblErrorNoOwn.Visible = true;
                    
                }
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }



        /// <summary>
        /// Busca Items para crear un nuevo Kit
        /// </summary>
        protected void imgBtnSearchItemKit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                bool validItem = false;
                bool existingItem = false;
                pnlErrorKit.Visible = false;
                isMgrKit = true;

                //Lo primero es verificar que tiene un Owner seleccionado
                //DropDownList ddlown = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");

                if (this.ddlOwnerItemKit.SelectedValue != "-1")
                {
                    this.lblErrorNoOwn.Visible = false;

                    // Busca en base de datos el Item ingresado 
                    if (txtCodeKit.Text.Trim() != string.Empty)
                    {
                        //Consulta los items
                        itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, txtCodeKit.Text.Trim(), int.Parse(ddlOwnerItemKit.SelectedValue), false);

                        if (itemSearchViewDTO.Entities != null)
                        {
                            Item item = new Item();

                            //Verifica si la busqueda es exitosa (trae un solo item) para agregarlo a los controles del encabezado
                            if (itemSearchViewDTO.Entities.Count == 1)
                            {
                                validItem = true;
                                item = itemSearchViewDTO.Entities[0];

                                // Mantiene en memoria los datos del Item a agregar
                                Session.Add("KitMgrNewItem", item);

                                kitViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitList];

                                if (kitViewDTO != null)
                                {
                                    // Recorre los items ya agregados y compara con el que se quiere agregar
                                    if (kitViewDTO.Entities != null && kitViewDTO.Entities.Count > 0)
                                    {
                                        foreach (Kit kit in kitViewDTO.Entities)
                                        {
                                            // Si ya existe en la lista se marca
                                            if (kit.ItemKit.Code == item.Code)
                                            {
                                                existingItem = true;
                                                pnlErrorKit.Visible = false;
                                            }
                                        }
                                    }
                                }

                                // Si no fue agregado, agrega el item 
                                if (!existingItem)
                                {
                                    this.txtCodeKit.Text = item.Code;
                                    this.txtDescriptionKit.Text = item.Description;
                                }
                                else
                                {
                                    pnlErrorKit.Visible = true;
                                }
                            }
                        }
                    }

                    // Si no es válido o no se ingresó, se muestra la lista de Items para seleccionar uno
                    if (!validItem)
                    {
                        ucFilterItem.Clear();
                        ucFilterItem.Initialize();

                        // Setea el filtro con el Item ingresado
                        if (txtCodeKit.Text.Trim() != string.Empty)
                        {
                            FilterItem filterItem = new FilterItem("%" + txtCodeKit.Text + "%");
                            filterItem.Selected = true;
                            ucFilterItem.FilterItems[0] = filterItem;
                            ucFilterItem.LoadCurrentFilter(ucFilterItem.FilterItems);
                            SearchItemKit();
                        }
                        // Si no se ingresó ningún item, no se ejecuta la búsqueda
                        else
                            ClearGridItem();

                        // Esto evita un bug de ajax
                        valAddItem.Enabled = false;
                        valSearchItem.Enabled = false;

                        this.lblAddItem.Text = lblNewKit.Text;

                        mpLookupItem.Show();

                        ucFilterItem.Visible = true;//Esconde los controles de busqueda de Item incluidos los Kit
                        ucFilterItem2.Visible = false; //Muestra los controles de busqueda de Item sin items de tipo Kit

                        InitializePageCountItems();
                    }
                }
                else
                {
                    //Muestra mensaje que debe seleccionar un Owner
                    this.lblErrorNoOwn.Visible = true;

                    ErrorDTO error = new ErrorDTO();
                    error.Title = "Selección de Dueño";
                    error.Message = this.lblErrorAllOwner.Text;
                    error.Level = ErrorLevel.Info;
                    this.Master.ucError.ShowError(error);
                }
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega el item al detalle del Kit
        /// </summary>
        protected void imgBtnAddItem_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                bool addItem = true;
                kitViewDTO = new GenericViewDTO<Kit>();
                Kit newkitDetail = new Kit();
                newkitDetail.ItemKit = new Item();
                newkitDetail.ItemBase = new Item();


                // Si ya existen kits y detalles los trae
                kitDetailViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitDetailList];
                kitViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitList];


                // Crea el nuevo detalle del Kit
                Item newItem = new Item();
                newItem = (Item)Session["kitDetailNewItem"];

                newkitDetail.ItemBase = newItem;

                //si viene de selectedChanged
                if (currentIndex != -1)
                {
                    newkitDetail.ItemKit = kitViewDTO.Entities[currentIndex].ItemKit;
                }

                if (MiscUtils.IsNumeric(txtQty.Text))
                    newkitDetail.ItemQty = Convert.ToDecimal(txtQty.Text);
                else
                    newkitDetail.ItemQty = 0;


                if (kitDetailViewDTO != null)
                {
                    //recorre el los Items existentes y compara con el que se quiere agregar
                    if (kitDetailViewDTO.Entities != null && kitDetailViewDTO.Entities.Count > 0)
                    {
                        foreach (Kit kit in kitDetailViewDTO.Entities)
                        {
                            // Si ya existe en el detalle y ademas tiene la misma cantidad, se avisa
                            if (newkitDetail.ItemBase != null && kit.ItemBase.Id == newkitDetail.ItemBase.Id) 
                            {
                                pnlError.Visible = true;
                                addItem = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        addItem = true;
                    }
                }
                if (addItem) 
                {
                    if (editMode)
                        kitViewDTO = iWarehousingMGR.MaintainKit(CRUD.Update, newkitDetail, context);
                    else
                        kitViewDTO = iWarehousingMGR.MaintainKit(CRUD.Create, newkitDetail, context);
                }
                // Limpia paneles Nuevo Item
                txtCode.Text = string.Empty;
                txtCode.Enabled = true;
                txtDescription.Text = string.Empty;
                txtQty.Text = string.Empty;
                pnlError.Visible = false;

                txtCodeKit.Text = string.Empty;
                txtCodeKit.Enabled = true;
                txtDescriptionKit.Text = string.Empty;
                pnlErrorKit.Visible = false;

                if (!kitViewDTO.hasError() && kitViewDTO.Entities != null)
                {
                    if (!crud)
                        ucStatus.ShowMessage(kitViewDTO.MessageStatus.Message);

                    kitViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitList];
                    isValidViewDTO = true;
                }
                else
                {
                    isValidViewDTO = false;
                    this.Master.ucError.ShowError(kitViewDTO.Errors);
                }

                // Limpia el detalle para que se recargue
                Session.Remove(WMSTekSessions.KitMgr.KitDetailList);
                this.lblErrorNoOwn.Visible = false;
                editMode = false;
                isNewKit = false;
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega el nuevo Kit
        /// </summary>
        protected void imgBtnAddItemKit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                isNewKit = true;

                // Crea el nuevo Kit
                Kit newkit = new Kit();
                newkit.ItemKit = new Item();
                newkit.ItemBase = new Item();

                // Recupera el Item seleccionado
                Item newItem = new Item();
                newItem = (Item)Session["kitMgrNewItem"];

                newkit.ItemKit = newItem;
                newkit.Id = newItem.Id;

                // Valida la variable session que contiene los items encontrados
                if (ValidateSession(WMSTekSessions.KitMgr.KitList))
                {
                    // Obtiene los kits existentes
                    kitViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitList];
                }

                if (!kitViewDTO.hasError() && kitViewDTO.Entities != null)
                {
                    isRepeated = false;

                    // Valida que el item no exista en la grilla de kits
                    foreach (Kit kit in kitViewDTO.Entities)
                    {
                        //compara el que se selecciono con los que estan en la lista para agregar
                        if (kit.ItemKit.Id == newkit.ItemKit.Id)
                        {
                            isRepeated = true;

                            // Esto evita un bug de ajax
                            valAddItemKit.Enabled = true;
                            valSearchItemKit.Enabled = true;
                            break;
                        }
                    }

                    if (!isRepeated) // Kit nuevo
                    {
                        //Pregunto si es el primero que se agrega
                        if (kitViewDTO.Entities.Count > 0)
                        {
                            // Si el último kit agregado está solo en memoria (no tiene ItemBase), lo sobreescribe
                            if (kitViewDTO.Entities[kitViewDTO.Entities.Count - 1].ItemBase.Code == null)
                                kitViewDTO.Entities.RemoveAt(kitViewDTO.Entities.Count - 1);

                            // Agrega el nuevo Kit a la lista en memoria
                            kitViewDTO.Entities.Add(newkit);
                            Session.Add(WMSTekSessions.KitMgr.KitList, kitViewDTO);

                            //Capturo el id del kit
                            currentIndex = kitViewDTO.Entities.Count - 1;
                            grdMgr.SelectedIndex = currentIndex;

                        }
                        else
                        {
                            // Agrega el nuevo Kit a la lista en memoria
                            kitViewDTO.Entities.Add(newkit);
                            Session.Add(WMSTekSessions.KitMgr.KitList, kitViewDTO);
                            currentIndex = 0;
                            grdMgr.SelectedIndex = currentIndex;
                        }

                        // Carga la lista vacia de detalles
                        kitDetailViewDTO = new GenericViewDTO<Kit>();
                        kitDetailViewDTO.Entities = new List<Kit>();
                        Session.Add(WMSTekSessions.KitMgr.KitDetailList, kitDetailViewDTO);
                    }
                    else //Kit Repetido
                    {
                        //Envia error que esta repetido
                        kitViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.ItemKit.Repeated, context));
                        UpdateSession(true);
                    }

                    // Limpia panel Nuevo Item
                    txtCodeKit.Text = string.Empty;
                    this.txtCodeKit.Enabled = true;
                    txtDescriptionKit.Text = string.Empty;
                    pnlErrorKit.Visible = false;

                    // Muestra el detalle vacio
                    this.lblGridDetail.Text = string.Empty;
                    this.lblGridDetail.Text = lblDetailsHead.Text + kitViewDTO.Entities[currentIndex].ItemKit.LongName;
                    divItemDetail.Visible = true;

                }
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
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
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        /// <summary>
        /// Busca los Kits que existen en el sistema
        /// </summary>
        protected void btnSearchItem_Click(object sender, EventArgs e)
        {
            try
            {
                SearchItemKit();
                // Si es un ViewDTO valido, carga la grilla y las listas
                if (isValidViewDTO)
                {
                    if (isMgrKit)
                        this.lblAddItem.Text = lblNewKit.Text;
                    else
                        this.lblAddItem.Text = lblNewDetail.Text;

                    mpLookupItem.Show();

                    InitializePageCountItems();
                }
            }
            catch (Exception ex)
            {
                itemSearchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemSearchViewDTO.Errors);
            }
        }

        /// <summary>
        /// Busca los Kits que existen en el sistema
        /// </summary>
        protected void btnSearchItem2_Click(object sender, EventArgs e)
        {
            try
            {
                SearchItemItem();
                // Si es un ViewDTO valido, carga la grilla y las listas
                if (isValidViewDTO)
                {
                    if (isMgrKit)
                        this.lblAddItem.Text = lblNewKit.Text;
                    else
                        this.lblAddItem.Text = lblNewDetail.Text;
                    
                    mpLookupItem.Show();
                    ucFilterItem.Visible = false;//Esconde los controles de busqueda de Item incluidos los Kit
                    ucFilterItem2.Visible = true; //Muestra los controles de busqueda de Item sin items de tipo Kit
                    InitializePageCountItems();
                }
            }
            catch (Exception ex)
            {
                itemSearchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemSearchViewDTO.Errors);
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
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
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
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
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
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
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
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
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
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
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
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
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
        //        kitViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(kitViewDTO.Errors);
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
                //     SaveChanges();
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;
                    ImageButton btnEdit = e.Row.FindControl("btnEdit") as ImageButton;

                    if (btnDelete != null)
                    {
                        // Deshabilita el botón borrar para un Kit creado en memoria
                        if (e.Row.DataItemIndex == kitViewDTO.Entities.Count - 1 && isNewKit)
                        {
                            btnDelete.Enabled = false;
                            btnDelete.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_delete_dis.png";
                        }
                        else
                            btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";
                    }

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    
                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                    e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                }
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a eliminar
                int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;

                DeleteRow(kitViewDTO.Entities[deleteIndex], true);
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        /// <summary>
        /// Evento sucede cuando se selecciona un kit
        /// </summary>
        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Limpia paneles Nuevo Item
                    txtCode.Text = string.Empty;
                    txtCode.Enabled = true;
                    txtDescription.Text = string.Empty;
                    txtQty.Text = string.Empty;
                    pnlError.Visible = false;

                    txtCodeKit.Text = string.Empty;
                    txtCodeKit.Enabled = true;
                    txtDescriptionKit.Text = string.Empty;
                    pnlErrorKit.Visible = false;

                    //capturo la posicion de la fila 
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndex = grdMgr.SelectedIndex;

                    //Trae los kits encontrados
                    kitViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitList];

                    // Limpia el detalle para que se recargue
                    Session.Remove(WMSTekSessions.KitMgr.KitDetailList);

                    this.lblGridDetail.Text = string.Empty;
                    this.lblGridDetail.Text = lblDetailsHead.Text + kitViewDTO.Entities[currentIndex].ItemKit.LongName;
                    divItemDetail.Visible = true;
                    editMode = false;
                    this.lblErrorNoOwn.Visible = false;
                }
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega propiedades y script a la grilla
        /// </summary>
        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete1 = e.Row.FindControl("btnDelete1") as ImageButton;

                    if (btnDelete1 != null)
                    {
                        btnDelete1.OnClientClick = "if(confirm('" + lblConfirmDeleteItem.Text + "')==false){return false;}";
                    }
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewDetailOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        /// <summary>
        /// Edita la cantidad de un item en el detalle
        /// </summary>
        protected void grdDetail_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                EditItem(e.NewEditIndex);
                editMode = true;
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        /// <summary>
        /// Elimina un item del detalle
        /// </summary>
        protected void grdDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                Kit kit = new Kit();
                //Calcula la posicion en el ViewDTO de la fila a eliminar
                int deleteIndex = grdDetail.PageSize * grdDetail.PageIndex + e.RowIndex;
                kit = kitDetailViewDTO.Entities[deleteIndex];

                // Si el kit tiene un solo detalle, lo borra completo
                if (kitDetailViewDTO.Entities.Count == 1)
                    DeleteRow(kit, true);
                else
                    DeleteRow(kit, false);
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega el nuevo "Kit" a la grilla principal
        /// </summary>
        protected void grdSearchItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                kitViewDTO = new GenericViewDTO<Kit>();
                //captura el id del item seleccionado
                int editId = (Convert.ToInt32(grdSearchItems.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));


                //Si viene de detalle, se va a la seccion detalle
                if (isMgrKit)//Es Kit
                {
                    //valida la variable session que contiene los items encontrados
                    if (ValidateSession(WMSTekSessions.KitMgr.KitList))
                    {
                        //obtiene los kits existentes
                        kitViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitList];
                    }
                    if (!kitViewDTO.hasError() && kitViewDTO.Entities != null)
                    {
                        isRepeated = false;

                        // Valida que el item no exista en la grilla de kits
                        foreach (Kit kit in kitViewDTO.Entities)
                        {
                            // Compara el que se selecciono con los que estan en la lista para agregar
                            if (kit.ItemKit.Id == editId)
                            {
                                isRepeated = true;

                                // Esto evita un bug de ajax
                                valAddItem.Enabled = true;
                                valSearchItem.Enabled = true;
                                break;
                            }
                        }

                        if (!isRepeated)//el item no existe como kit
                        {
                            // Valida la variable session que contiene los items encontrados
                            if (ValidateSession(WMSTekSessions.Shared.ItemList))
                            {
                                // Trae la lista de busqueda
                                itemSearchViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.Shared.ItemList];

                                //recorre los items de la lista de busqueda para agregar el seleccionado
                                foreach (Item item in itemSearchViewDTO.Entities)
                                {
                                    // Compara el que se selecciono con los que estan en la lista paar agregar
                                    if (item.Id == editId)
                                    {
                                        this.txtCodeKit.Text = item.Code;
                                        this.txtDescriptionKit.Text = item.Description;

                                        Session["KitMgrNewItem"] = item;
                                        break;
                                    }
                                }

                                // Cierra el modal popup de kit
                                this.mpLookupItem.Hide();
                            }
                        }
                        else//Kit Repetido
                        {
                            //Envia error que esta repetido
                            kitViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.ItemKit.Repeated, context));

                            UpdateSession(true);
                        }
                    }
                    else//kit tiene errores o esta nulo
                    {
                        isValidViewDTO = false;
                        this.Master.ucError.ShowError(kitViewDTO.Errors);
                        UpdateSession(true);
                    }
                }
                else //Es detalle, debe agregar el item en la grilla detalle
                {
                    //Compara si ya esta en la lista (repetido)
                    kitDetailViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitDetailList];

                    if (kitDetailViewDTO != null)
                    {
                        if (kitDetailViewDTO.Entities != null)
                        {
                            foreach (Kit kitdetail in kitDetailViewDTO.Entities)
                            {
                                if (kitdetail.ItemBase.Id == editId)
                                {
                                    isRepeated = true;
                                    // Esto evita un bug de ajax
                                    pnlError.Visible = false;
                                    valAddItem.Enabled = true;
                                    valSearchItem.Enabled = true;
                                    break;
                                }
                            }
                            if (!isRepeated)
                            {
                                //No existe en la lista por lo tanto se muestra en los cuadros de texto (antes de insertar se debe poner la cantidad)
                                //Recorre la lista para obtener la descripcion
                                if (ValidateSession(WMSTekSessions.Shared.ItemList))
                                {
                                    //Trae la lista de busqueda
                                    itemSearchViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.Shared.ItemList];

                                    //recorre los items de la lista de busqueda para agregar el seleccionado
                                    foreach (Item item in itemSearchViewDTO.Entities)
                                    {
                                        if (item.Id == editId)
                                        {
                                            this.txtCode.Text = item.Code;
                                            this.txtDescription.Text = item.Description;
                                            //cierra el modal popup de kit
                                            this.mpLookupItem.Hide();
                                            Session["KitDetailNewItem"] = item;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //Envia un mensaje de advertencia y NO inserta
                                pnlError.Visible = true;
                            }
                        }
                    }
                }
                //Trae los kit para que al cargar la pagina llene la grilla
                kitViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitList];
            }
            catch (Exception ex)
            {
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }
        }

        protected void btnSubir2_Click(object sender, EventArgs e)
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
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.KitMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidDataException(ex.Message);

                    }

                    DataTable dataTable;

                    try
                    {
                        dataTable = ConvertXlsToDataTable(savePath, 1);
                    }
                    catch (Exception ex)
                    {
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.KitMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       ItemCodeKit = r.Field<object>("ItemCodeKit"),
                                       ItemCodeBase = r.Field<object>("ItemCodeBase"),
                                       ItemQty = r.Field<object>("ItemQty")
                                   };

                    kitViewDTO = new GenericViewDTO<Kit>();

                    try
                    {
                        foreach (var item in lstExcel)
                        {
                            if (!IsValidExcelRow(item))
                                continue;

                            Kit newKit = new Kit();

                            if (!ValidateIsNotNull(item.ItemCodeKit))
                            {
                                errorUp = "ItemCodeKit " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newKit.ItemKit = new Item();
                                newKit.ItemKit.Code = item.ItemCodeKit.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.ItemCodeBase))
                            {
                                errorUp = "ItemCodeBase " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newKit.ItemBase = new Item();
                                newKit.ItemBase.Code = item.ItemCodeBase.ToString().Trim();
                            }

                            if (newKit.ItemBase.Code.Equals(newKit.ItemKit.Code))
                            {
                                errorUp = "ItemCodeBase e ItemCodeKit no pueden ser iguales";
                                break;
                            }

                            if (!ValidateIsNotNull(item.ItemQty))
                            {
                                errorUp = "ItemCodeKit " + item.ItemCodeKit + " - ItemQty " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                decimal qty = 0;
                                bool isValidQty = decimal.TryParse(item.ItemQty.ToString().Trim(), out qty);

                                if (isValidQty)
                                {
                                    newKit.ItemQty = qty;
                                }
                                else
                                {
                                    errorUp = "ItemCodeKit " + item.ItemCodeKit + " - ItemQty " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            kitViewDTO.Entities.Add(newKit);
                        }
                    }
                    catch (Exception ex)
                    {
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.KitMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
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
                        if (kitViewDTO.Entities.Count > 0)
                        {
                            kitViewDTO = iWarehousingMGR.MaintainKitsMassive(kitViewDTO, context);

                            if (kitViewDTO.hasError())
                            {
                                ShowAlertLocal(this.lblTitle.Text, kitViewDTO.Errors.Message);
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                ucStatus.ShowMessage(kitViewDTO.MessageStatus.Message);
                                ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                divLoad.Visible = false;
                                modalPopUpLoad.Hide();
                            }
                        }
                        else
                        {
                            ShowAlertLocal(this.lblTitle.Text, this.lblNotItemsFile.Text);
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
                theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.KitMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (InvalidOperationException ex)
            {
                theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.KitMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.KitMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, kitViewDTO.Errors.Message);
            }
            finally
            {
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }
            }
        }

        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            divLoad.Visible = true;
            modalPopUpLoad.Show();
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
                kitViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kitViewDTO.Errors);
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
            InitializeGridItems();
            InitializeFilterItem();
            PopulateLists();
        }

        /// <summary>
        /// Carga en sesion la lista de la Entidad
        /// </summary>
        /// <param name="showError">Determina si se mostrara el error producido en una operacion anterior</param>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(kitViewDTO.Errors);
                kitViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            if (this.Master.ucMainFilter.idOwn == -1)
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();
                foreach (ListItem item in this.Master.ucMainFilter.listItemOwners)
	            {
            		 context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(item.Text,item.Value));
	            } 

            }
            else
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem("",this.Master.ucMainFilter.idOwn.ToString()));
            }

            if (this.Master.ucMainFilter.itemCode.Trim() != string.Empty)
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Add(new FilterItem("", this.Master.ucMainFilter.itemCode.Trim()));
            }

            if (this.Master.ucMainFilter.itemName.Trim() != string.Empty)
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Add(new FilterItem("", this.Master.ucMainFilter.itemName.Trim()));
            }

            kitViewDTO = iWarehousingMGR.FindAllKit(context);

            if (!kitViewDTO.hasError() && kitViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.KitMgr.KitList, kitViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(kitViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }

            // Limpia el detalle para que se recargue
            Session.Remove(WMSTekSessions.KitMgr.KitDetailList);
            this.lblErrorNoOwn.Visible = false;
            editMode = false;
            isNewKit = false;
        }

        private void RefreshSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(kitViewDTO.Errors);
                kitViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            
            context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();
            context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem("", this.ddlOwnerItemKit.SelectedValue));

            if (this.Master.ucMainFilter.itemCode.Trim() != string.Empty)
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Add(new FilterItem("", this.Master.ucMainFilter.itemCode.Trim()));
            }

            if (this.Master.ucMainFilter.itemName.Trim() != string.Empty)
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Add(new FilterItem("", this.Master.ucMainFilter.itemName.Trim()));
            }

            kitViewDTO = iWarehousingMGR.FindAllKit(context);

            if (!kitViewDTO.hasError() && kitViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.KitMgr.KitList, kitViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(kitViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(kitViewDTO.Errors);
            }

            // Limpia el detalle para que se recargue
            Session.Remove(WMSTekSessions.KitMgr.KitDetailList);
            this.lblErrorNoOwn.Visible = false;
            editMode = false;
            isNewKit = false;
        }

        /// <summary>
        /// Configuracion inicial del Filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeFilterItem()
        {
            ucFilterItem.Initialize();
            ucFilterItem.BtnSearchClick += new EventHandler(btnSearchItem_Click);

            ucFilterItem.FilterCode = this.lblFilterCode.Text;
            ucFilterItem.FilterDescription = this.lblFilterName.Text;

            ucFilterItem2.Initialize();
            ucFilterItem2.BtnSearchClick += new EventHandler(btnSearchItem2_Click);

            ucFilterItem2.FilterCode = this.lblFilterCode.Text;
            ucFilterItem2.FilterDescription = this.lblFilterName.Text;
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);
            this.Master.ucTaskBar.btnAddVisible = true;
            this.Master.ucTaskBar.btnAddToolTip = this.lblAddLoadToolTip.Text;
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.btnExcelVisible = true;

        }

        /// <summary>
        /// Configuracion inicial de la Barra de Estado
        /// </summary>
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
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

        private void InitializeGridItems()
        {
            //grdSearchItems.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchItems.EmptyDataText = this.Master.EmptyGridText;
        }

        private void PopulateGrid()
        {
            grdMgr.DataSource = null;
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!kitViewDTO.hasConfigurationError() && kitViewDTO.Configuration != null && kitViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, kitViewDTO.Configuration);

            grdMgr.DataSource = kitViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(kitViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }

        private void PopulateLists()
        {
            base.LoadUserOwners(this.ddlOwnerItemKit, this.Master.EmptyRowText, "-1", false, string.Empty, false);
        }

        protected void ReloadData()
        {
            // Limpia paneles Nuevo Item
            txtCode.Text = string.Empty;
            txtCode.Enabled = true;
            txtDescription.Text = string.Empty;
            txtQty.Text = string.Empty;
            pnlError.Visible = false;

            txtCodeKit.Text = string.Empty;
            txtCodeKit.Enabled = true;
            txtDescriptionKit.Text = string.Empty;
            pnlErrorKit.Visible = false;

            currentIndex = -1;
            divItemDetail.Visible = false;

            //Como no es Actualizacion, setea la variable para que muestre "Listo"
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                grdMgr.SelectedIndex = -1;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        private void SearchItemKit()
        {
            //DropDownList ddlown = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
            //int intOwn = Convert.ToInt32(ddlown.SelectedValue.ToString());
            int intOwn = Convert.ToInt32(this.ddlOwnerItemKit.SelectedValue.ToString());

            itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwnerFilter(ucFilterItem.FilterItems, context, intOwn, true);

            if (itemSearchViewDTO.hasError())
                isValidViewDTO = false;
            else
                isValidViewDTO = true;

            Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);
            grdSearchItems.EmptyDataText = this.Master.EmptyGridText;
            grdSearchItems.DataSource = itemSearchViewDTO.Entities;
            grdSearchItems.DataBind();
        }

        private void SearchItemItem()
        {
            //DropDownList ddlown = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
            //int intOwn = Convert.ToInt32(ddlown.SelectedValue.ToString());
            //int intOwn = kitViewDTO.Entities[currentIndex].ItemBase.Owner.Id;
            int intOwn = kitViewDTO.Entities[currentIndex].ItemKit.Owner.Id;

            itemSearchViewDTO = iWarehousingMGR.GetByCodeAndOwnerFilterNotInKit(ucFilterItem2.FilterItems, context, intOwn, true);

            if (itemSearchViewDTO.hasError())
                isValidViewDTO = false;
            else
                isValidViewDTO = true;

            Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);
            grdSearchItems.EmptyDataText = this.Master.EmptyGridText;
            grdSearchItems.DataSource = itemSearchViewDTO.Entities;
            grdSearchItems.DataBind();
        }

        private void ClearGridItem()
        {
            Session.Remove(WMSTekSessions.Shared.ItemList);
            grdSearchItems.DataSource = null;
            grdSearchItems.DataBind();
        }

        protected void LoadDetailByIdKit()
        {
            bool isValidIndex = false;

            if (currentIndex != -1)
            {
                if (ValidateSession(WMSTekSessions.KitMgr.KitList))
                {
                    kitViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitList];
                    isValidViewDTO = true;
                }
                if (kitViewDTO.Entities.Count >= currentIndex)
                {
                    int id = kitViewDTO.Entities[currentIndex].ItemKit.Id;

                    if (ValidateSession(WMSTekSessions.KitMgr.KitDetailList))
                    {
                        kitDetailViewDTO = (GenericViewDTO<Kit>)Session[WMSTekSessions.KitMgr.KitDetailList];
                    }
                    else
                    {
                        kitDetailViewDTO = iWarehousingMGR.GetDetailKit(context, id);
                        Session.Add(WMSTekSessions.KitMgr.KitDetailList, kitDetailViewDTO);
                    }
                }
                else
                {
                    currentIndex = 0;
                }
                isValidIndex = true;
            }

            if (isValidIndex)
            {
                if (kitDetailViewDTO.Entities != null)
                {
                    //Agrega la lista en session
                    Session.Add(WMSTekSessions.KitMgr.KitDetailList, kitDetailViewDTO);

                    //Habilita el panel del detalle de items
                    this.divItemDetail.Visible = true;

                    // Configura ORDEN de las columnas de la grilla
                    if (!kitDetailViewDTO.hasConfigurationError() && kitDetailViewDTO.Configuration != null && kitDetailViewDTO.Configuration.Count > 0)
                        //base.ConfigureGridOrder(grdDetail, kitDetailViewDTO.Configuration);

                    grdDetail.DataSource = kitDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }
                else
                {
                    this.divItemDetail.Visible = false;
                }
            }
        }

        protected void EditItem(int itemIndex)
        {
            this.txtCode.Text = kitDetailViewDTO.Entities[itemIndex].ItemBase.Code;
            this.txtCode.Enabled = false;
            this.txtDescription.Text = kitDetailViewDTO.Entities[itemIndex].ItemBase.Description;
            this.txtQty.Text = kitDetailViewDTO.Entities[itemIndex].ItemQty.ToString();

            // Mantiene en memoria los datos del Item a agregar
            Item itm = new Item();
            itm = kitDetailViewDTO.Entities[itemIndex].ItemBase;

            Session.Add("kitDetailNewItem", itm);

            // Quita el item de la lista 
            kitDetailViewDTO.Entities.RemoveAt(itemIndex);

            // Actualiza la sesión
            Session.Add(WMSTekSessions.KitMgr.KitDetailList, kitDetailViewDTO);
        }

        /// <summary>
        /// Elimina el item del kit
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(Kit kit, bool deleteKit)
        {
            //Elimina todas las lineas que tengan el id de kit seleccionado
            kitViewDTO = iWarehousingMGR.MaintainKit(CRUD.Delete, kit, context);

            // Si está borrando el Kit completo
            if (deleteKit)
            {
                currentIndex = -1;
                divItemDetail.Visible = false;
            }

            //Actualiza la session
            if (kitViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra mensaje de status
                crud = true;
                ucStatus.ShowMessage(kitViewDTO.MessageStatus.Message);

                //Actualiza la session
                UpdateSession(false);
            }
        }

        #endregion


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


        protected void ddlOwnerItemKit_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtCodeKit.Text = string.Empty;
            this.txtDescriptionKit.Text = string.Empty;

            Session["KitMgrNewItem"] = null;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('Kits_GetByOwner', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDropCustom();", true);
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdSearchItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}