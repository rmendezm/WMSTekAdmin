using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Inventory;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Data;

namespace Binaria.WMSTek.WebClient.Administration.Inventory
{
    public partial class InventoryMgr : BasePage
    {
        #region "Declaración de Variables"

        GenericViewDTO<InventoryOrder> inventoryViewDTO = new GenericViewDTO<InventoryOrder>();
        GenericViewDTO<Location> locationViewDTO = new GenericViewDTO<Location>();
        GenericViewDTO<InventoryLocation> inventoryLocationViewDTO = new GenericViewDTO<InventoryLocation>();
        List<Location> locationAssociated = new List<Location>();
        List<Location> locationNoAssociated = new List<Location>();
        private GenericViewDTO<Auxiliary> auxiliaryViewDTO = new GenericViewDTO<Auxiliary>();
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
        // Propiedad para controlar el indice del objeto que se está editando
        public int currentIndex
        {
            get
            {
                if (ValidateViewState("index"))
                    return (int)ViewState["index"];
                else
                    return 0;
            }

            set { ViewState["index"] = value; }
        }

        

        #endregion

        #region "Configuracion de Pagina"

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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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

                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        /// <summary>
        /// Persiste los cambios en la entidad (nueva o editada)
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    grdMgr.Enabled = true;
                    SaveChanges();
                }
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        /// <summary>
        /// Abre la ventana modal para crear una nueva entidad
        /// </summary>
        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.ucMainFilter.LoadControlValuesToFilterObject();
                context.MainFilter = this.Master.ucMainFilter.MainFilter;
                ShowModal(0, CRUD.Create);
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                    ImageButton btnNextTrack = e.Row.FindControl("btnNextTrack") as ImageButton;
                    ImageButton btnClose = e.Row.FindControl("btnClose") as ImageButton;
                    ImageButton btnAddLocation = e.Row.FindControl("btnAddLocation") as ImageButton;
                    ImageButton btnManualInventory = e.Row.FindControl("btnManualInventory") as ImageButton;

                    if (btnManualInventory != null)
                    {
                        if (inventoryViewDTO.Entities[e.Row.DataItemIndex].TrackInventoryType.Id == (int)TrackInventoryTypeName.EnEjecucion)
                        {
                            btnManualInventory.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_process.png";
                            btnManualInventory.Enabled = true;
                        }
                        else
                        {
                            btnManualInventory.ImageUrl = "~/WebResources/Images/Buttons/TaskBar/icon_process_dis.png";
                            btnManualInventory.Enabled = false;
                        }
                    }

                    if (btnDelete != null)
                    {
                        btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";
                    }

                    // Deshabilita la opcion de Eliminar si el Inventory está en estado distinto a 'Pendiente'
                    // o está en estado 'Pendiente' pero no es el más reciente para el Centro
                    if ((btnDelete != null) && (this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "OWNER").FilterValues.Count > 1))
                    {
                        if (btnDelete != null && !inventoryViewDTO.Entities[e.Row.DataItemIndex].LatestPending)
                        {
                            btnDelete.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_delete_dis.png";
                            btnDelete.Enabled = false;
                        }
                    }
                    else
                        if (btnDelete != null && inventoryViewDTO.Entities[e.Row.DataItemIndex].TrackInventoryType.Id == (int)TrackInventoryTypeName.Aprobado
                            || inventoryViewDTO.Entities[e.Row.DataItemIndex].TrackInventoryType.Id == (int)TrackInventoryTypeName.Aplicado)
                        {
                            btnDelete.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_delete_dis.png";
                            btnDelete.Enabled = false;
                        }



                    // Deshabilita la opcion de Agregar Ubicacion si el Inventory está en estado menor a 'Terminado' o es inventario Full
                    if ((btnAddLocation != null && inventoryViewDTO.Entities[e.Row.DataItemIndex].TrackInventoryType.Id >= (int)TrackInventoryTypeName.EnEjecucion)
                        || (inventoryViewDTO.Entities[e.Row.DataItemIndex].IsFullWhs == true))
                    {
                        btnAddLocation.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_add_dis.png";
                        btnAddLocation.Enabled = false;
                    }
                    else
                    {
                        btnAddLocation.CommandArgument = e.Row.DataItemIndex.ToString();
                    }

                    // Deshabilita la opcion de Editar si el Inventory está en estado mayor a 'Aprobado'
                    if (btnEdit != null && inventoryViewDTO.Entities[e.Row.DataItemIndex].TrackInventoryType.Id > (int)TrackInventoryTypeName.Aprobado)
                    {
                        btnEdit.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_edit_dis.png";
                        btnEdit.Enabled = false;
                    }
                    else
                    {
                        btnEdit.CommandArgument = e.Row.DataItemIndex.ToString();
                    }

                    // Deshabilita las opciones de NextTrack y Cerrar si el Inventory está en estado 'Aplicado' o 'Cerrado sin Aplicar'
                    if (btnNextTrack != null && btnClose != null && (inventoryViewDTO.Entities[e.Row.DataItemIndex].TrackInventoryType.Id == (int)TrackInventoryTypeName.Aplicado
                        || inventoryViewDTO.Entities[e.Row.DataItemIndex].TrackInventoryType.Id == (int)TrackInventoryTypeName.CerradoSinAplicar))
                    {
                        btnNextTrack.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_apply_inventory_dis.png";
                        btnNextTrack.Enabled = false;

                        btnClose.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_close_dis.png";
                        btnClose.Enabled = false;
                    }
                    else
                    {
                        // Configura el icono y tooltip segun el estado actual del inventario
                        btnNextTrack.CommandArgument = e.Row.DataItemIndex.ToString();                    
                        btnClose.CommandArgument = e.Row.DataItemIndex.ToString();

                        string toolTip = GetNextTrackName(e.Row.DataItemIndex);
                        btnNextTrack.ToolTip = toolTip;

                        string imageUrl = GetNextTrackIcon(e.Row.DataItemIndex);
                        btnNextTrack.ImageUrl = imageUrl;

                        // Agrega una advertencia si la próxima acción a realizar sobre el Inventario es 'Aplicar Inventario'
                        if (inventoryViewDTO.Entities[e.Row.DataItemIndex].TrackInventoryType.Id == (int)TrackInventoryTypeName.Aprobado)
                        {
                            if (inventoryViewDTO.Entities[e.Row.DataItemIndex].UserApproval.Id == context.SessionInfo.User.Id)
                                btnNextTrack.OnClientClick = "if(confirm('" + lblApplyInventoryWarning.Text + "')==false){return false;}";
                            else
                                btnNextTrack.OnClientClick = "alert('" + lblUserApplyInventoryAlert.Text + "');return false;";
                        }
                        else if (inventoryViewDTO.Entities[e.Row.DataItemIndex].TrackInventoryType.Id == (int)TrackInventoryTypeName.EnEjecucion)
                        {
                            if (!ValidateCountedLocation(inventoryViewDTO.Entities[e.Row.DataItemIndex].Id))
                            {
                                btnNextTrack.OnClientClick = "alert('" + lblCountedLocationAlert.Text + "');return false;";
                            }
                        }

                        // Agrega una advertencia para la acción 'Cerrar Inventario sin Aplicar'
                        btnClose.OnClientClick = "if(confirm('" + lblCloseInventoryWarning.Text + "')==false){return false;}";
                    }

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        //protected void grdLocations_RowCreated(object sender, GridViewRowEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Row.RowType == DataControlRowType.DataRow)
        //        {
        //            // Agrega atributos para cambiar el color de la fila seleccionada
        //            e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdLocations.ClientID + "')");
        //            e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdLocations.ClientID + "')");
        //            e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdLocations.ClientID + "')");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        inventoryViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(inventoryViewDTO.Errors);
        //    }
        //}
       
        //protected void grdLocations_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Row.RowType == DataControlRowType.DataRow)
        //        {
        //            CheckBox chkAddLocation = e.Row.FindControl("chkAddLocation") as CheckBox;

        //            if (chkAddLocation != null)
        //            {
        //                if (chkAddLocation.Checked)
        //                {
        //                    //e.Row.Attributes.Add("onload", "this.style.backgroundColor='#aafdaa'");
        //                    chkAddLocation.Enabled = false;
        //                    e.Row.ForeColor = System.Drawing.Color.Gold;
        //                    e.Row.BackColor = System.Drawing.Color.Tomato;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        inventoryViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(inventoryViewDTO.Errors);

        //    }
        //}

        //protected void grdLocations_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    try
        //    {
        //        int index = grdLocations.PageSize * grdLocations.PageIndex + Convert.ToInt32(e.CommandArgument);

        //        if (e.CommandName == "Add")
        //        {

        //            //ChangeColorRow(index);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        inventoryViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(inventoryViewDTO.Errors);
        //    }
        //}

        //protected void grdLocations_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{

        //}

        //protected void grdLocations_RowUpdating(object sender, GridViewUpdateEventArgs e)
        //{

        //}

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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                TrackInventoryTypeName nextTrack;

                int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "Close")
                {

                    ChangeInventoryTrack(index, TrackInventoryTypeName.CerradoSinAplicar);
                }

                if (e.CommandName == "NextTrack")
                {
                    nextTrack = GetNextTrack(index);

                    if (nextTrack != TrackInventoryTypeName.Pendiente)
                    {
                        if (nextTrack == TrackInventoryTypeName.Aplicado)
                        {
                            ApplyInventory(index);
                        }
                        else
                            ChangeInventoryTrack(index, nextTrack);
                    }
                }
                if (e.CommandName == "Add")
                {
                    currentIndex = index;
                    LoadLocation(index);
                }

                if (e.CommandName == "Manual")
                {
                    OpenPopUpManualInventory(index);
                }
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        private void OpenPopUpManualInventory(int index)
        {
            var inventorySelected = inventoryViewDTO.Entities[index];

            hidInventoryId.Value = inventorySelected.Id.ToString();
            divManualInventory.Visible = true;
            upManualInventory.Update();
            modalPopUpManualInventory.Show();
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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
        //        inventoryViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(inventoryViewDTO.Errors);
        //    }
        //}

        protected void btnSubir_Click(object sender, EventArgs e)
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

                    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                                "Data Source=" + savePath + ";" +
                                                "Extended Properties=Excel 8.0;";

                    var dataAdapter = new OleDbDataAdapter("SELECT * FROM [Hoja1$]", connectionString);
                    var myDataSet = new DataSet();
                    DataTable dataTable;

                    try
                    {
                        dataAdapter.Fill(myDataSet);
                        dataTable = myDataSet.Tables["Table"];
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       LocCode = r.Field<object>("LocCode"),
                                       IsEmptyLocation = r.Field<object>("IsEmptyLocation"),
                                       ItemCode = r.Field<object>("ItemCode"),
                                       LotNumber = r.Field<object>("LotNumber"),
                                       SerialNumber = r.Field<object>("SerialNumber"),
                                       FifoDate = r.Field<object>("FifoDate"),
                                       ExpirationDate = r.Field<object>("ExpirationDate"),
                                       FabricationDate = r.Field<object>("FabricationDate"),
                                       Qty = r.Field<object>("Qty"),
                                       CtgCode = r.Field<object>("CtgCode"),
                                       LpnCode = r.Field<object>("LpnCode"),
                                       LpnTypeCode = r.Field<object>("LpnTypeCode")
                                   };

                    try
                    {
                        var inventoryDetails = new GenericViewDTO<InventoryDetail>();

                        foreach (var item in lstExcel)
                        {
                            if (!IsValidExcelRow(item))
                                continue;

                            var newInventoryDetail = new InventoryDetail();

                            if (!ValidateIsNotNull(item.LocCode))
                            {
                                errorUp = "LocCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newInventoryDetail.Location = new Location(item.LocCode.ToString().Trim().Replace("'", ""));
                            }

                            if (!ValidateIsNotNull(item.IsEmptyLocation))
                            {
                                errorUp = "IsEmptyLocation " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (item.IsEmptyLocation.ToString().Trim() == "1")
                                {
                                    newInventoryDetail.IsEmptyLocation = true;
                                }
                                else if (item.IsEmptyLocation.ToString().Trim() == "0")
                                {
                                    newInventoryDetail.IsEmptyLocation = false;
                                }
                                else
                                {
                                    errorUp = "IsEmptyLocation " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (!newInventoryDetail.IsEmptyLocation)
                            {
                                if (!ValidateIsNotNull(item.ItemCode))
                                {
                                    errorUp = "ItemCode " + this.lblFieldNotNull.Text;
                                    break;
                                }
                                else
                                {
                                    newInventoryDetail.Item = new Item() { Code = item.ItemCode.ToString().Trim() };
                                }

                                if (ValidateIsNotNull(item.LotNumber))
                                    newInventoryDetail.LotNumber = item.LotNumber.ToString().Trim();

                                if (ValidateIsNotNull(item.SerialNumber))
                                    newInventoryDetail.SerialNumber = item.SerialNumber.ToString().Trim();

                                if (ValidateIsNotNull(item.FifoDate))
                                {
                                    var finalDate = DateTime.MinValue;
                                    bool isValidDate = DateTime.TryParseExact(item.FifoDate.ToString().Trim(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out finalDate);

                                    if (isValidDate)
                                    {
                                        newInventoryDetail.FifoDate = finalDate;
                                    }
                                    else
                                    {
                                        errorUp = "FifoDate (dd/mm/yyyy) " + this.lblFieldInvalid.Text;
                                        break;
                                    }
                                }

                                if (ValidateIsNotNull(item.ExpirationDate))
                                {
                                    var finalDate = DateTime.MinValue;
                                    bool isValidDate = DateTime.TryParseExact(item.ExpirationDate.ToString().Trim(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out finalDate);

                                    if (isValidDate)
                                    {
                                        newInventoryDetail.ExpirationDate = finalDate;
                                    }
                                    else
                                    {
                                        errorUp = "ExpirationDate (dd/mm/yyyy) " + this.lblFieldInvalid.Text;
                                        break;
                                    }
                                }

                                if (ValidateIsNotNull(item.FabricationDate))
                                {
                                    var finalDate = DateTime.MinValue;
                                    bool isValidDate = DateTime.TryParseExact(item.FabricationDate.ToString().Trim(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out finalDate);

                                    if (isValidDate)
                                    {
                                        newInventoryDetail.FabricationDate = finalDate;
                                    }
                                    else
                                    {
                                        errorUp = "FabricationDate (dd/mm/yyyy) " + this.lblFieldInvalid.Text;
                                        break;
                                    }
                                }

                                if (!ValidateIsNotNull(item.Qty))
                                {
                                    errorUp = "Qty " + this.lblFieldNotNull.Text;
                                    break;
                                }
                                else
                                {
                                    decimal finalQty = 0;
                                    var isDecimal = decimal.TryParse(item.Qty.ToString(), out finalQty);

                                    if (isDecimal && finalQty >= 0)
                                    {
                                        newInventoryDetail.ItemQty = finalQty;
                                    }
                                    else
                                    {
                                        errorUp = "Qty " + this.lblFieldInvalid.Text;
                                        break;
                                    }
                                }

                                if (ValidateIsNotNull(item.CtgCode))
                                    newInventoryDetail.CategoryItem = new CategoryItem() { Code = item.CtgCode.ToString().Trim() };
                                else
                                    newInventoryDetail.CategoryItem = new CategoryItem(-1);

                                newInventoryDetail.LPN = new LPN() { LPNType = new LPNType() };

                                if (ValidateIsNotNull(item.LpnCode))
                                    newInventoryDetail.LPN.Code = item.LpnCode.ToString().Trim();

                                if (ValidateIsNotNull(item.LpnTypeCode))
                                    newInventoryDetail.LPN.LPNType.Code = item.LpnTypeCode.ToString().Trim();
                            }
                            else
                            {
                                newInventoryDetail.Item = new Item(-1);
                                newInventoryDetail.CategoryItem = new CategoryItem(-1);
                                newInventoryDetail.LPN = new LPN() { LPNType = new LPNType() };
                            }

                            inventoryDetails.Entities.Add(newInventoryDetail);
                        }

                        var repeteatedLpns = inventoryDetails.Entities.Where(id => !string.IsNullOrEmpty(id.LPN.Code))
                        .GroupBy(id => new
                        {
                            LpnIdCode = id.LPN.IdCode
                        })
                        .Where(grp => grp.Count() > 1)
                        .Select(od => new
                        {
                            LpnIdCode = od.Key.LpnIdCode,
                            Count = od.Count()
                        }).ToList();

                        if (repeteatedLpns != null && repeteatedLpns.Count > 0)
                        {
                            foreach (var repeteatedLpn in repeteatedLpns)
                            {
                                var locationsByLpn = inventoryDetails.Entities.Where(id => id.LPN.IdCode != null && id.LPN.IdCode.Equals(repeteatedLpn.LpnIdCode))
                                                                              .Select(rl => rl.Location.IdCode)
                                                                              .Distinct()
                                                                              .ToList();

                                if (locationsByLpn.Count > 1)
                                {
                                    errorUp = this.lblValidateRepeatedLpns.Text.Replace("[IDCODE]", repeteatedLpn.LpnIdCode);
                                    break;
                                }
                            }
                        }

                        if (errorUp != "")
                        {
                            ShowAlertLocal(this.lblTitle.Text, errorUp);
                            divFondoPopupProgress.Visible = false;
                            divManualInventory.Visible = true;
                            modalPopUpManualInventory.Show();
                        }
                        else
                        {
                            if (inventoryDetails.Entities.Count > 0)
                            {
                                int inventoryId = int.Parse(hidInventoryId.Value.Trim());
                                var intentoryDetailViewDTO = iInventoryMGR.MaintainInventoryDetailMassive(inventoryId, inventoryDetails, context);

                                if (intentoryDetailViewDTO.hasError())
                                {
                                    //UpdateSession(true);
                                    ShowAlertLocal(this.lblTitle.Text, intentoryDetailViewDTO.Errors.Message);
                                    divFondoPopupProgress.Visible = false;
                                    divManualInventory.Visible = true;
                                    modalPopUpManualInventory.Show();
                                }
                                else
                                {
                                    ucStatus.ShowMessage(intentoryDetailViewDTO.MessageStatus.Message);
                                    ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                    divFondoPopupProgress.Visible = false;
                                    divManualInventory.Visible = false;
                                    modalPopUpManualInventory.Hide();
                                }
                            }
                            else
                            {
                                ShowAlertLocal(this.lblTitle.Text, this.lblNotItemsFile.Text);
                                divFondoPopupProgress.Visible = false;
                                divManualInventory.Visible = true;
                                modalPopUpManualInventory.Show();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divFondoPopupProgress.Visible = false;
                divManualInventory.Visible = true;
                modalPopUpManualInventory.Show();
            }
            catch (InvalidOperationException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divFondoPopupProgress.Visible = false;
                divManualInventory.Visible = true;
                modalPopUpManualInventory.Show();
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, inventoryViewDTO.Errors.Message);
            }
        }

        public void ShowAlert(string title, string message)
        {
            Encoding encod = Encoding.ASCII;
            string script = "ShowMessage('" + title + "','" + message.Replace("'", "") + "');";
            script = script.Replace("\\", Convert.ToChar(47).ToString());
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "InventoryMgr";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            this.divWarning.Visible = false;

            if (!Page.IsPostBack)
            {
                UpdateSession(false);
                PopulateLists();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.InventoryMgr.InventoryList))
                {
                    inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryMgr.InventoryList];
                    PopulateGrid();
                }

                if (ValidateSession(WMSTekSessions.InventoryMgr.LocationListAssociated))
                    locationAssociated = (List<Location>)Session[WMSTekSessions.InventoryMgr.LocationListAssociated];

                if (ValidateSession(WMSTekSessions.InventoryMgr.LocationListNoAssociated))
                    locationNoAssociated = (List<Location>)Session[WMSTekSessions.InventoryMgr.LocationListNoAssociated];
            }
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
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
                inventoryViewDTO.ClearError();
            }

            this.Master.ucMainFilter.ClearFilterObject();
            this.Master.ucMainFilter.LoadControlValuesToFilterObject();
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            inventoryViewDTO = iInventoryMGR.FindAllInventory(context);

            if (!inventoryViewDTO.hasError() && inventoryViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InventoryMgr.InventoryList, inventoryViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(inventoryViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.statusVisible = true;
            this.Master.ucMainFilter.SaveOnIndexChanged = true;
            this.Master.ucMainFilter.codeVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.codeLabel = this.lblNroInventoryFilter.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.InventoryDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.InventoryDaysAfter;

            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

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

            this.Master.ucTaskBar.btnNewVisible = true;
            this.Master.ucTaskBar.btnRefreshVisible = true;
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

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!inventoryViewDTO.hasConfigurationError() && inventoryViewDTO.Configuration != null && inventoryViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, inventoryViewDTO.Configuration);

            grdMgr.DataSource = inventoryViewDTO.Entities;
            grdMgr.DataBind();

            upGrid.Update();
            ucStatus.ShowRecordInfo(inventoryViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void PopulateLists()
        {
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);;
            base.LoadUserWarehouses(this.ddlWarehouse, this.Master.EmptyRowText, "-1", true);

            // TODO: reemplazar por lista de Usuarios Aprobadores (consultar Vladi - queda para Fase 3)
            base.LoadUsersByCodStatus(this.ddlUserApproval, CodStatus.Enabled, false, true, this.Master.EmptyRowText);

            List<string> lstLocTypeInv = GetConst("TypeLocationInventory");
            base.LoadLocationTypeFilter(this.ddlLocationType, false, this.Master.AllRowsText, lstLocTypeInv);
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            // Editar entidad
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = inventoryViewDTO.Entities[index].Id.ToString();
                hidTrackInventoryTypeId.Value = inventoryViewDTO.Entities[index].TrackInventoryType.Id.ToString();

                ddlWarehouse.SelectedValue = inventoryViewDTO.Entities[index].Warehouse.Id.ToString();
                ddlOwner.SelectedValue = inventoryViewDTO.Entities[index].Owner.Id.ToString();

                if (inventoryViewDTO.Entities[index].CreateDate != null && inventoryViewDTO.Entities[index].CreateDate > DateTime.MinValue)
                    this.lblCreateDate.Text = inventoryViewDTO.Entities[index].CreateDate.ToString("dd-MM-yyyy");
                else
                    this.lblCreateDate.Text = string.Empty;

                if (inventoryViewDTO.Entities[index].StartDate != null && inventoryViewDTO.Entities[index].StartDate > DateTime.MinValue)
                {
                    this.txtStartDate.Text = inventoryViewDTO.Entities[index].StartDate.ToString("dd-MM-yyyy");
                    this.txtStartDateHours.Text = inventoryViewDTO.Entities[index].StartDate.Hour.ToString();
                    this.txtStartDateMinutes.Text = inventoryViewDTO.Entities[index].StartDate.Minute.ToString();
                }
                else
                {
                    this.txtStartDate.Text = string.Empty;
                    this.txtStartDateHours.Text = "0";
                    this.txtStartDateMinutes.Text = "0";                    
                }

                if (inventoryViewDTO.Entities[index].EndDate != null && inventoryViewDTO.Entities[index].EndDate > DateTime.MinValue) 
                {
                    this.txtEndDate.Text = inventoryViewDTO.Entities[index].EndDate.ToString("dd-MM-yyyy");
                    this.txtEndDateHours.Text = inventoryViewDTO.Entities[index].EndDate.Hour.ToString();
                    this.txtEndDateMinutes.Text = inventoryViewDTO.Entities[index].EndDate.Minute.ToString();
                }
                else
                {
                    this.txtEndDate.Text = string.Empty;
                    this.txtEndDateHours.Text = "0";
                    this.txtEndDateMinutes.Text = "0";
                }

                chkIsFullWhs.Checked = inventoryViewDTO.Entities[index].IsFullWhs;
                lblTrackInventoryType.Text = inventoryViewDTO.Entities[index].TrackInventoryType.Name;
                ddlUserApproval.SelectedValue = (inventoryViewDTO.Entities[index].UserApproval.Id).ToString();
                txtCountQty.Text = inventoryViewDTO.Entities[index].CountQty.ToString();
                chkStatus.Checked = inventoryViewDTO.Entities[index].Status;
                txtDescription.Text = inventoryViewDTO.Entities[index].Description;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                // Selecciona Warehouse y Owner seleccionados en el Filtro
                this.ddlWarehouse.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues[0].Value;
                this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

                hidEditId.Value = "0";
                hidTrackInventoryTypeId.Value = Convert.ToInt16(TrackInventoryTypeName.Pendiente).ToString();

                lblTrackInventoryType.Text = TrackInventoryTypeName.Pendiente.ToString();  // TODO: leer de BD o archivo de configuración
                this.lblCreateDate.Text = DateTime.Now.ToString();
                this.txtStartDate.Text = string.Empty;
                this.txtStartDateHours.Text = "0";
                this.txtStartDateMinutes.Text = "0";
                this.txtEndDate.Text = string.Empty;
                this.txtEndDateHours.Text = "0";
                this.txtEndDateMinutes.Text = "0";
                chkIsFullWhs.Checked = false;
                ddlUserApproval.SelectedValue = "-1";
                txtCountQty.Text = "1";
                chkStatus.Checked = true;
                txtDescription.Text = string.Empty;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }
            if (inventoryViewDTO.Configuration != null && inventoryViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                {
                    base.ConfigureModal(inventoryViewDTO.Configuration, true);
                    SetFieldsStatus(-1);
                }
                else
                {
                    base.ConfigureModal(inventoryViewDTO.Configuration, false);

                    // Habilita o deshabilita los campos según el estado del Inventory
                    SetFieldsStatus(index);
                }
            }

            this.divWarning.Visible = false;

            // Muestra ventana modal
            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        /// <summary>
        /// Habilita o deshabilita los campos según el estado del Inventory
        /// </summary>
        /// <param name="index"></param>
        protected void SetFieldsStatus(int index)
        {
            // Habilita todos los controles
            ddlWarehouse.Enabled = true;
            rfvWarehouse.Enabled = true;

            txtStartDate.Enabled = true;
            rfvStartDate.Enabled = true;
            txtStartDateHours.Enabled = true;
            rangeStartDateHours.Enabled = true;
            btnMoreStartDateHours.Enabled = true;
            btnMoreStartDateHours.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_more.png";
            btnLessStartDateHours.Enabled = true;
            btnLessStartDateHours.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_less.png";
            txtStartDateMinutes.Enabled = true;                
            rangeStartDateMinutes.Enabled = true;
            btnMoreStartDateMinutes.Enabled = true;
            btnMoreStartDateMinutes.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_more.png";
            btnLessStartDateMinutes.Enabled = true;
            btnLessStartDateMinutes.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_less.png";

            chkIsFullWhs.Enabled = true;
            chkStatus.Enabled = true;
            ddlOwner.Enabled = true;
            rfvOwner.Enabled = true;

            txtEndDate.Enabled = true;
            rfvEndDate.Enabled = true;
            txtEndDateHours.Enabled = true;
            rangeEndDateHours.Enabled = true;
            btnMoreEndDateHours.Enabled = true;
            btnMoreEndDateHours.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_more.png";
            btnLessEndDateHours.Enabled = true;
            btnLessEndDateHours.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_less.png";
            txtEndDateMinutes.Enabled = true;
            rangeEndDateMinutes.Enabled = true;
            btnMoreEndDateMinutes.Enabled = true;
            btnMoreEndDateMinutes.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_more.png";
            btnLessEndDateMinutes.Enabled = true;
            btnLessEndDateMinutes.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_less.png";

            txtCountQty.Enabled = true;
            txtCountQty.ReadOnly = false;
            rfvCountQty.Enabled = true;
            rangeCountQty.Enabled = true;
            btnMoreQty.Enabled = true;
            btnMoreQty.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_more.png";
            btnLessQty.Enabled= true;
            btnLessQty.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_less.png";

            ddlUserApproval.Enabled = true;
            txtDescription.Enabled = true;

            //Si es un inventario nuevo no sigue seteando
            if (index != -1)
            {
                if (inventoryViewDTO.Entities[index].TrackInventoryType.Id == (int)TrackInventoryTypeName.Pendiente)
                {
                    ddlWarehouse.Enabled = false;
                    rfvWarehouse.Enabled = false;
                    chkIsFullWhs.Enabled = false;
                    ddlOwner.Enabled = false;
                    rfvOwner.Enabled = false;
                }

                //// Pendiente pero no el más reciente para el Centro 
                //if (inventoryViewDTO.Entities[index].TrackInventoryType.Id == (int)TrackInventoryTypeName.Pendiente
                //    && !inventoryViewDTO.Entities[index].LatestPending)
                //{
                //    ddlWarehouse.Enabled = false;
                //    rfvWarehouse.Enabled = false;
                //}

                // En Ejecución o mayor
                if (inventoryViewDTO.Entities[index].TrackInventoryType.Id >= (int)TrackInventoryTypeName.EnEjecucion)
                {
                    ddlWarehouse.Enabled = false;
                    rfvWarehouse.Enabled = false;

                    txtStartDate.Enabled = false;
                    rfvStartDate.Enabled = false;
                    txtStartDateHours.Enabled = false;
                    rangeStartDateHours.Enabled = false;
                    btnMoreStartDateHours.Enabled = false;
                    btnMoreStartDateHours.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_more_dis.png";
                    btnLessStartDateHours.Enabled = false;
                    btnLessStartDateHours.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_less_dis.png";
                    txtStartDateMinutes.Enabled = false;
                    rangeStartDateMinutes.Enabled = false;
                    btnMoreStartDateMinutes.Enabled = false;
                    btnMoreStartDateMinutes.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_more_dis.png";
                    btnLessStartDateMinutes.Enabled = false;
                    btnLessStartDateMinutes.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_less_dis.png";

                    chkIsFullWhs.Enabled = false;
                    chkStatus.Enabled = false;
                    ddlOwner.Enabled = false;
                    rfvOwner.Enabled = false;

                    // Terminado o mayor
                    if (inventoryViewDTO.Entities[index].TrackInventoryType.Id >= (int)TrackInventoryTypeName.Terminado)
                    {
                        txtEndDate.Enabled = false;
                        rfvEndDate.Enabled = false;
                        txtEndDateHours.Enabled = false;
                        rangeEndDateHours.Enabled = false;
                        btnMoreEndDateHours.Enabled = false;
                        btnMoreEndDateHours.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_more_dis.png";
                        btnLessEndDateHours.Enabled = false;
                        btnLessEndDateHours.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_less_dis.png";
                        txtEndDateMinutes.Enabled = false;
                        rangeEndDateMinutes.Enabled = false;
                        btnMoreEndDateMinutes.Enabled = false;
                        btnMoreEndDateMinutes.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_more_dis.png";
                        btnLessEndDateMinutes.Enabled = false;
                        btnLessEndDateMinutes.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_less_dis.png";

                        txtCountQty.Enabled = false;
                        txtCountQty.ReadOnly = true;
                        rfvCountQty.Enabled = false;
                        rangeCountQty.Enabled = false;
                        btnMoreQty.Enabled = false;
                        btnMoreQty.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_more_dis.png";
                        btnLessQty.Enabled = false;
                        btnLessQty.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_less_dis.png";

                        // Aprobado o mayor
                        if (inventoryViewDTO.Entities[index].TrackInventoryType.Id >= (int)TrackInventoryTypeName.Aprobado)
                        {
                            ddlUserApproval.Enabled = false;

                            // Aplicado o mayor
                            if (inventoryViewDTO.Entities[index].TrackInventoryType.Id >= (int)TrackInventoryTypeName.Aplicado)
                            {
                                txtDescription.Enabled = false;
                            }
                        }
                    }
                }
            }
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
        /// Aplica el Inventario al Stock actual
        /// </summary>
        /// <param name="index"></param>
        protected void ApplyInventory(int index)
        { 
            InventoryOrder inventory = inventoryViewDTO.Entities[index];

            var typeKardexConst = GetConst("TypeKardexInventory");
            string whsStockAvailable = GetConst("WarehouseStockAvailable")[0];

            KardexType kardexType = new KardexType();
            GenericViewDTO<KardexType> kardexTypeViewDTO = iWarehousingMGR.FindAllKardexType(context);
            kardexType = kardexTypeViewDTO.Entities.Find(f=>f.IdKardexType.Equals(Convert.ToInt32(typeKardexConst[0])));

            inventoryViewDTO = iInventoryMGR.ApplyInventory(inventory, kardexType, whsStockAvailable, context);

            if (inventoryViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //ClientScript.RegisterClientScriptBlock(typeof(InventoryMgr), "InventoryOk", "<script>alert('" + lblInventoryOk.Text + "')</script>");
                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblInventoryOk.Text, "");
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Cambia el estado del Inventario
        /// </summary>
        /// <param name="index"></param>
        protected void ChangeInventoryTrack(int index, TrackInventoryTypeName newTrack)
        {
            InventoryOrder inventory = inventoryViewDTO.Entities[index];
            inventory.TrackInventoryType.Id = (int)newTrack;

            inventoryViewDTO = iInventoryMGR.MaintainInventory(CRUD.Update, inventory, context);

            if (inventoryViewDTO.hasError())
                UpdateSession(true);
            else
                UpdateSession(false);
        }

        /// <summary>
        /// Obtiene el próximo Track del inventario, en base al Track actual
        /// </summary>
        /// <param name="index"></param>
        protected TrackInventoryTypeName GetNextTrack(int index)
        {
            TrackInventoryTypeName currentTrack;
            TrackInventoryTypeName nextTrack;
            
            currentTrack = (TrackInventoryTypeName)inventoryViewDTO.Entities[index].TrackInventoryType.Id;
            
            switch (currentTrack)
            {
                case TrackInventoryTypeName.Pendiente:
                    TrackInventoryTypeName nextTrack1 = new TrackInventoryTypeName();
                    auxiliaryViewDTO = new GenericViewDTO<Auxiliary>();
                    
                    //Valida que tenga ubicaciones asignadas para inventariar y que inventario no se completo
                    auxiliaryViewDTO = iInventoryMGR.GetCountLocationForInventory(inventoryViewDTO.Entities[index].Id, context);

                    if (((auxiliaryViewDTO.Entities[0].Count > 0) && (!inventoryViewDTO.Entities[index].IsFullWhs)) ||
                        ((auxiliaryViewDTO.Entities[0].Count >= 0) && (inventoryViewDTO.Entities[index].IsFullWhs)))
                    {

                        //Valida que no exista otro inventario con el mismo Owner en Ejecucion
                        if (!iInventoryMGR.ValidateInventoryOrderExists(inventoryViewDTO.Entities[index].Warehouse.Id, inventoryViewDTO.Entities[index].Owner.Id, (int)TrackInventoryTypeName.Pendiente))
                        {
                            inventoryViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.InventoryOrder.InventoryAlreadyExistsWhs, context));
                            this.Master.ucError.ShowError(inventoryViewDTO.Errors);
                            nextTrack1 = TrackInventoryTypeName.Pendiente;
                            nextTrack = nextTrack1;
                            break;
                        }
                        else
                        {
                            //Valida que no existan ubicaciones con tareas pendientes
                            GenericViewDTO<Binaria.WMSTek.Framework.Entities.Inventory.InventoryLocation> invLocationViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Inventory.InventoryLocation>();
                            invLocationViewDTO = iInventoryMGR.GetInventoryLocationExistsInTask(inventoryViewDTO.Entities[index].Id, context);
                            
                            if (invLocationViewDTO.Entities != null && invLocationViewDTO.Entities.Count > 0){

                                inventoryViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.InventoryOrder.InventoryLocationsWithPendingTasks, context));
                                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
                                nextTrack1 = TrackInventoryTypeName.Pendiente;
                                nextTrack = nextTrack1;
                                break;
                            }else{
                                nextTrack1 = TrackInventoryTypeName.EnEjecucion;
                            }
                        }
                        nextTrack = nextTrack1;

                        //Valida que no exista otro inventario con el mismo Owner en Ejecucion
                        //foreach (InventoryOrder Inventory in inventoryViewDTO.Entities)
                        //{
                        //    if ((Inventory.Owner.Id == inventoryViewDTO.Entities[index].Owner.Id) && Inventory.TrackInventoryType.Id == (int)TrackInventoryTypeName.EnEjecucion)
                        //    {
                        //        inventoryViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.InventoryOrder.StartInventory, context));
                        //        this.Master.ucError.ShowError(inventoryViewDTO.Errors);
                        //        nextTrack1 = TrackInventoryTypeName.Pendiente;
                        //        break;
                        //    }
                        //    else
                        //        nextTrack1 = TrackInventoryTypeName.EnEjecucion;
                        //}
                        //nextTrack = nextTrack1;
                    }
                    else
                    {
                        nextTrack = TrackInventoryTypeName.Pendiente;
                        inventoryViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.InventoryOrder.InventoryMissingLocation, context));
                        this.Master.ucError.ShowError(inventoryViewDTO.Errors);
                    }
                    break;

                case TrackInventoryTypeName.EnEjecucion:
                    nextTrack = TrackInventoryTypeName.Terminado;
                    break;

                case TrackInventoryTypeName.Terminado:
                    nextTrack = TrackInventoryTypeName.Aprobado;
                    break;

                case TrackInventoryTypeName.Aprobado:
                    nextTrack = TrackInventoryTypeName.Aplicado;
                    break;

                default:
                    nextTrack = TrackInventoryTypeName.Pendiente;
                    break;
            }
            return nextTrack;
        }

        /// <summary>
        /// Obtiene el nombre del próximo Track del inventario, en base al Track actual
        /// </summary>
        /// <param name="index"></param>
        protected string GetNextTrackName(int index)
        {
            // TODO: leer nombres de DB o archivo de configuracion
            TrackInventoryTypeName currentTrack;
            string nextTrack;

            currentTrack = (TrackInventoryTypeName)inventoryViewDTO.Entities[index].TrackInventoryType.Id;

            switch (currentTrack)
            {
                case TrackInventoryTypeName.Pendiente:
                    nextTrack = "Iniciar Inventario";
                    break;

                case TrackInventoryTypeName.EnEjecucion:
                    nextTrack = "Terminar Inventario";
                    break;

                case TrackInventoryTypeName.Terminado:
                    nextTrack = "Aprobar Inventario";
                    break;

                case TrackInventoryTypeName.Aprobado:
                    nextTrack = "Aplicar Inventario";
                    break;

                default:
                    nextTrack = "Iniciar Inventario";
                    break;
            }

            return nextTrack;
        }
        
        /// <summary>
        /// Obtiene el icono del próximo Track del inventario, en base al Track actual
        /// </summary>
        /// <param name="index"></param>
        protected string GetNextTrackIcon(int index)
        {
            TrackInventoryTypeName currentTrack;
            string nextIcon;

            currentTrack = (TrackInventoryTypeName)inventoryViewDTO.Entities[index].TrackInventoryType.Id;

            switch (currentTrack)
            {
                case TrackInventoryTypeName.Pendiente:
                    nextIcon = "~/WebResources/Images/Buttons/GridActions/icon_start_inventory.png"; ;
                    break;

                case TrackInventoryTypeName.EnEjecucion:
                    nextIcon = "~/WebResources/Images/Buttons/GridActions/icon_stop_inventory.png"; ;
                    break;

                case TrackInventoryTypeName.Terminado:
                    nextIcon = "~/WebResources/Images/Buttons/GridActions/icon_tick_inventory.png"; ;
                    break;

                case TrackInventoryTypeName.Aprobado:
                    nextIcon = "~/WebResources/Images/Buttons/GridActions/icon_apply_inventory.png"; ;
                    break;

                default:
                    nextIcon = "~/WebResources/Images/Buttons/GridActions/icon_start_inventory.png"; ;
                    break;
            }

            return nextIcon;
        }

        /// <summary>
        /// Persiste los cambios realizados en la entidad
        /// </summary>
        /// <param name="index"></param>
        protected void SaveChanges()
        {
            InventoryOrder inventory = new InventoryOrder();

            inventory.Id = Convert.ToInt32(hidEditId.Value);
            inventory.Warehouse = new Warehouse(Convert.ToInt32(ddlWarehouse.SelectedValue));
            inventory.Owner = new Owner(Convert.ToInt32(ddlOwner.SelectedValue));

            if(!string.IsNullOrEmpty(this.txtStartDate.Text)) inventory.StartDate = Convert.ToDateTime(this.txtStartDate.Text);
            if (!string.IsNullOrEmpty(this.txtEndDate.Text)) inventory.EndDate = Convert.ToDateTime(this.txtEndDate.Text);

            // Agrega las horas ingresadas
            if (!string.IsNullOrEmpty(this.txtStartDateHours.Text))
                inventory.StartDate = inventory.StartDate.AddHours(Convert.ToInt16(this.txtStartDateHours.Text));
            if (!string.IsNullOrEmpty(this.txtEndDateHours.Text))
                inventory.EndDate = inventory.EndDate.AddHours(Convert.ToInt16(this.txtEndDateHours.Text));

            // Agrega los minutos seleccionados
            if (!string.IsNullOrEmpty(this.txtStartDateMinutes.Text))
                inventory.StartDate = inventory.StartDate.AddMinutes(Convert.ToInt16(this.txtStartDateMinutes.Text));
            if (!string.IsNullOrEmpty(this.txtEndDateMinutes.Text))
                inventory.EndDate = inventory.EndDate.AddMinutes(Convert.ToInt16(this.txtEndDateMinutes.Text));

            inventory.IsFullWhs = chkIsFullWhs.Checked;
            inventory.TrackInventoryType = new TrackInventoryType();
            inventory.TrackInventoryType.Id = Convert.ToInt16(hidTrackInventoryTypeId.Value);
            inventory.UserApproval = new Binaria.WMSTek.Framework.Entities.Profile.User(Convert.ToInt32(ddlUserApproval.SelectedValue));
            inventory.CountQty = Convert.ToInt16(txtCountQty.Text);
            inventory.Status = chkStatus.Checked;
            inventory.Description = txtDescription.Text;

            if (ValidateSave(inventory))
            {
                if (hidEditId.Value == "0")
                    inventoryViewDTO = iInventoryMGR.MaintainInventory(CRUD.Create, inventory, context);
                else
                    inventoryViewDTO = iInventoryMGR.MaintainInventory(CRUD.Update, inventory, context);

                divEditNew.Visible = false;
                modalPopUp.Hide();

                if (inventoryViewDTO.hasError())
                {
                    UpdateSession(true);
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                }
                else
                {
                    crud = true;
                    ucStatus.ShowMessage(inventoryViewDTO.MessageStatus.Message);

                    UpdateSession(false);
                }
            }
            else
            {
                this.divWarning.Visible = true;
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
        }

        /// <summary>
        /// Valida los datos ingresados por el usuario
        /// </summary>
        private bool ValidateSave(InventoryOrder inventory)
        {
            bool valid = true;

            // Fecha Inicio debe ser > a Fecha Creación (solo para inventario nuevo)
            if (inventory.StartDate > DateTime.MinValue && inventory.StartDate.Date < DateTime.Now.Date && hidEditId.Value == "0")
            {
                valid = false;
                lblErrorCode.Text = lblStartDateError.Text;
            }

            // Fecha Término debe ser >= a Fecha Inicio
            if (inventory.EndDate > DateTime.MinValue && inventory.EndDate < inventory.StartDate)
            {
                valid = false;
                lblErrorCode.Text = lblEndDateError.Text;
            }

            return valid;
        }

        private bool ValidateCountedLocation(int idInventory)
        {
            bool valid = true;
            
            auxiliaryViewDTO = new GenericViewDTO<Auxiliary>();
                    
            //Valida ubicaciones no contadas o tareas de reconteo pendientes
            auxiliaryViewDTO = iInventoryMGR.GetCountPendingLocationInventory(idInventory, context);

            if (auxiliaryViewDTO.Entities[0].Count > 0)
            {
                valid = false;
            }

            return valid;
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            inventoryViewDTO = iInventoryMGR.MaintainInventory(CRUD.Delete, inventoryViewDTO.Entities[index], context);
            if (inventoryViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(inventoryViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

       private void LoadLocation(int index)
       {
            inventoryViewDTO = new GenericViewDTO<InventoryOrder>();
            inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryMgr.InventoryList];

            // --> INICIO 
            // Comentado por la nueva funcionalidad para mostrar la pagina
            //Response.Redirect("InventoryLocation.aspx?IdWhs=" + inventoryViewDTO.Entities[index].Warehouse.Id +
            //    "&IdInventory=" + inventoryViewDTO.Entities[index].Id +
            //    "&InventoryNumber=" + inventoryViewDTO.Entities[index].Number, false);
            //--> FIN

            //string url = "InventoryLocation.aspx?IdWhs=" + inventoryViewDTO.Entities[index].Warehouse.Id +
            //             "&IdInventory=" + inventoryViewDTO.Entities[index].Id +
            //             "&InventoryNumber=" + inventoryViewDTO.Entities[index].Number;

            //HtmlControl frame1 = (HtmlControl)this.iframeLocation;
            //frame1.Attributes["src"] = url;
            //divUbicationNEW.Visible = true;
            //modalPopUpNEW.Show();


            //Nueva forma de asignar ubicaciones
            Session.Remove(WMSTekSessions.InventoryMgr.LocationListAssociated);
            Session.Remove(WMSTekSessions.InventoryMgr.LocationListNoAssociated);
            locationAssociated = new List<Location>();
            locationNoAssociated = new List<Location>();

            //Limpiar Pantalla
            this.txtLocation.Text = string.Empty;
            this.txtLocationFrom.Text = string.Empty;
            this.txtLocationTo.Text = string.Empty;

            SearchLocationsAssociated(inventoryViewDTO.Entities[index]);
            this.hfIndex.Value = index.ToString();

            base.LoadHangar(this.ddlHangar, inventoryViewDTO.Entities[index].Warehouse.Id, false, "sds");
            LoadDDLLocation();
            
            lblCountNoAssigned.Text = "0";
            lblNroInventory.Text = inventoryViewDTO.Entities[index].Number.ToString();

            divLocations.Visible = true;
            modalPopUp2.Show();
        }


        protected void ddlHangar_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDDLLocation();

                this.divLocations.Visible = true;
                this.modalPopUp2.Show();
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }

        }

        private void LoadDDLLocation()
        {
            inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryMgr.InventoryList];
            int idInvent = inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)].Id;
            int idWhs = inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)].Warehouse.Id;

            FilterItem filterHangar = new FilterItem("", this.ddlHangar.SelectedValue.Trim());
            FilterItem filterWhs = new FilterItem("", idWhs.ToString());
            FilterItem filterLocType = new FilterItem("", ddlLocationType.SelectedValue.Trim());

            ContextViewDTO contextoNew = new ContextViewDTO();
            contextoNew.MainFilter = this.Master.ucMainFilter.MainFilter;

            contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "HANGAR").FilterValues.Clear();
            contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "WAREHOUSE").FilterValues.Clear();
            contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONTYPE").FilterValues.Clear();

            //Agrega los filtro ocupados
            //if (this.Master.ucMainFilter.idHangar > 0)
            contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "HANGAR").FilterValues.Add(filterHangar);

           // if (this.Master.ucMainFilter.idWhs > 0)
            contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "WAREHOUSE").FilterValues.Add(filterWhs);

            ////Tipo Ubicacion
            contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONTYPE").FilterValues.Add(filterLocType);

            //Carga los combos Fila, Columna, Nivel
            base.LoadRowLocWihtEntitiesFilter(this.ddlRowFrom, this.ddlRowTo, true, contextoNew);
            base.LoadColumnLocWihtEntitiesFilter(this.ddlColumnFrom, this.ddlColumnTo, true, contextoNew);
            base.LoadLevelLocWihtEntitiesFilter(this.ddlLevelFrom, this.ddlLevelTo, true, contextoNew);
                 
        }

        /// <summary>
        /// Quita todas las ubicaciones asociadas
        /// </summary>
        protected void btnQuitarTodas_Click(object sender, EventArgs e)
        {
            ListItem listItem = new ListItem();
            List<Location> tmpLocations = new List<Location>();
            ListItemCollection tmpLocationsNoSelect = new ListItemCollection();
            try
            {

                //tmpLocationsNoSelect = lstLocNoAsociated.Items;
                //Recorre la lista de ubicaciones asociadas
                foreach (ListItem item in lstLocAsociated.Items)
                {
                    tmpLocations.Add(new Location(item.Value));
                    locationNoAssociated.Add(locationAssociated.First(w => w.IdCode == item.Value));
                    locationAssociated.Remove(locationAssociated.First(w => w.IdCode == item.Value));
                }

                if (tmpLocations.Count > 0)
                {
                    ////Elimina las ubicaciones
                    inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryMgr.InventoryList];
                    int idInvent = inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)].Id;

                    GenericViewDTO<Binaria.WMSTek.Framework.Entities.Inventory.InventoryLocation> invLocViewDTO = iLayoutMGR.DeleteLocation(idInvent, tmpLocations, context);

                    SearchLocationsAssociated(inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)]);

                    this.lstLocNoAsociated.DataSource = lstItems(locationNoAssociated);
                    this.lstLocNoAsociated.DataTextField = "Text";
                    this.lstLocNoAsociated.DataValueField = "Value";
                    this.lstLocNoAsociated.DataBind();

                    lblCountNoAssigned.Text = locationNoAssociated.Count().ToString();

                    Session.Add(WMSTekSessions.InventoryMgr.LocationListNoAssociated, locationNoAssociated);
                }

                this.divLocations.Visible = true;
                this.modalPopUp2.Show();
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }
        /// <summary>
        /// Quita las ubicaciones seleccionadas
        /// </summary>
        protected void btnQuitarSelec_Click(object sender, EventArgs e)
        {

            ListItem listItem = new ListItem();
            List<Location> tmpLocations = new List<Location>();
            ListItemCollection tmpLocationsNoSelect = new ListItemCollection();
            try
            {

                //tmpLocationsNoSelect = lstLocNoAsociated.Items;
                //Recorre la lista de ubicaciones asociadas
                foreach (ListItem item in lstLocAsociated.Items)
                {
                    //Evalua si la ubicacion esta seleccionada
                    if (item.Selected)
                    {
                        tmpLocations.Add(new Location(item.Value));
                        locationNoAssociated.Add(locationAssociated.First(w => w.IdCode == item.Value));
                        locationAssociated.Remove(locationAssociated.First(w => w.IdCode == item.Value));                        
                    }
                }

                if (tmpLocations.Count > 0)
                {
                    ////Elimina las ubicaciones
                    inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryMgr.InventoryList];
                    int idInvent = inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)].Id;

                    GenericViewDTO<Binaria.WMSTek.Framework.Entities.Inventory.InventoryLocation> invLocViewDTO = iLayoutMGR.DeleteLocation(idInvent, tmpLocations, context);

                    SearchLocationsAssociated(inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)]);

                    this.lstLocNoAsociated.DataSource = lstItems(locationNoAssociated);
                    this.lstLocNoAsociated.DataTextField = "Text";
                    this.lstLocNoAsociated.DataValueField = "Value";
                    this.lstLocNoAsociated.DataBind();

                    lblCountNoAssigned.Text = locationNoAssociated.Count().ToString();

                    Session.Add(WMSTekSessions.InventoryMgr.LocationListNoAssociated, locationNoAssociated);
                }

                this.divLocations.Visible = true;
                this.modalPopUp2.Show();
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega las ubicaciones no asociadas seleccionadas
        /// </summary>
        protected void btnAgregarSelec_Click(object sender, EventArgs e)
        {
            ListItem listItem = new ListItem();
            List<Location> tmpLocations = new List<Location>();
            ListItemCollection tmpLocationsNoSelect = new ListItemCollection();
            try
            {

                //tmpLocationsNoSelect = lstLocNoAsociated.Items;
                //Recorre la lista de ubicaciones no asociadas
                foreach (ListItem item in lstLocNoAsociated.Items)
                {
                    //Evalua si la ubicacion esta seleccionada
                    if (item.Selected)
                    {
                        tmpLocations.Add(new Location(item.Value));
                        locationNoAssociated.Remove(locationNoAssociated.First(w=>w.IdCode == item.Value));
                    }
                }

                if (tmpLocations.Count > 0)
                {
                    ////Elimina las ubicaciones
                    inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryMgr.InventoryList];
                    int idInvent = inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)].Id;

                    GenericViewDTO< Binaria.WMSTek.Framework.Entities.Inventory.InventoryLocation> invLocViewDTO = iLayoutMGR.AddLocation(idInvent, tmpLocations, context);

                    SearchLocationsAssociated(inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)]);

                    this.lstLocNoAsociated.DataSource = lstItems(locationNoAssociated);
                    this.lstLocNoAsociated.DataTextField = "Text";
                    this.lstLocNoAsociated.DataValueField = "Value";
                    this.lstLocNoAsociated.DataBind();

                    lblCountNoAssigned.Text = locationNoAssociated.Count().ToString();

                    Session.Add(WMSTekSessions.InventoryMgr.LocationListNoAssociated, locationNoAssociated);
                }

                this.divLocations.Visible = true;
                this.modalPopUp2.Show();
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
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
                    ////Elimina las ubicaciones
                    inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryMgr.InventoryList];
                    int idInvent = inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)].Id;

                    GenericViewDTO<Binaria.WMSTek.Framework.Entities.Inventory.InventoryLocation> invLocViewDTO = iLayoutMGR.AddLocation(idInvent, tmpLocations, context);

                    SearchLocationsAssociated(inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)]);

                    locationNoAssociated.Clear();

                    this.lstLocNoAsociated.DataSource = lstItems(locationNoAssociated);
                    this.lstLocNoAsociated.DataTextField = "Text";
                    this.lstLocNoAsociated.DataValueField = "Value";
                    this.lstLocNoAsociated.DataBind();

                    lblCountNoAssigned.Text = "0";

                    Session.Add(WMSTekSessions.InventoryMgr.LocationListNoAssociated, locationNoAssociated);
                }

                this.divLocations.Visible = true;
                this.modalPopUp2.Show();
            }
            catch (Exception ex)
            {
                inventoryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryViewDTO.Errors);
            }
        }

        protected void SearchLocationsNoAsocied()
        {
            try
            {
                //if (ddlHangar1.SelectedValue != string.Empty)
                //{
                //    this.lblError.Text = string.Empty;
                //    lblError.Visible = false;

                //    // Recupera el objeto filtro principal de memoria
                //    mainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];

                //    // Limpia el objeto 'Main Filter'
                //    ClearFilterObject();

                //    // Carga el objeto 'Main Filter' con los valores seleccionados
                //    LoadControlValuesToFilterObject1();

                //    // Salva los criterios seleccionados
                //    context.MainFilter = mainFilter;


                //    bool IsValid = true;
                //    int levelFrom = Convert.ToInt32(this.ddlLevelFrom.SelectedValue);
                //    int levelTo = Convert.ToInt32(this.ddlLevelTo.SelectedValue);
                //    int columnFrom = Convert.ToInt32(this.ddlColumnFrom.SelectedValue);
                //    int columnTo = Convert.ToInt32(this.ddlColumnTo.SelectedValue);
                //    int rowFrom = Convert.ToInt32(this.ddlRowFrom.SelectedValue);
                //    int rowTo = Convert.ToInt32(this.ddlRowTo.SelectedValue);
                //    int idWorkZone = workZoneViewDTO.Entities[currentIndex].Id;
                //    string aisle = this.txtAisle.Text.Trim();

                //    //validaciones
                //    if (levelTo < levelFrom)
                //    {
                //        //this.lblError.Text = this.lblErrorLevel.Text;
                //        IsValid = false;
                //    }
                //    if (columnTo < columnFrom)
                //    {
                //        //this.lblError.Text = this.lblErrorColunm.Text;
                //        IsValid = false;
                //    }
                //    if (rowTo < rowFrom)
                //    {
                //        //this.lblError.Text = this.lblErrorRow.Text;
                //        IsValid = false;
                //    }
                //    if (IsValid)
                //    {
                //        //locationNoAssociatedViewDTO = iLayoutMGR.GetLocationsByHngLevelColRowNotInWorkZone(idWorkZone, levelFrom, levelTo, rowFrom, rowTo, columnFrom, columnTo, context);
                //        locationNoAssociatedViewDTO = iLayoutMGR.GetLocationsByHngLevelColRowAisleNotInWorkZone(idWorkZone, levelFrom, levelTo, rowFrom, rowTo, columnFrom, columnTo, aisle, context);
                //    }

                //    if (!locationNoAssociatedViewDTO.hasError() && locationNoAssociatedViewDTO.Entities != null)
                //    {
                //        Session.Add(WMSTekSessions.WorkZoneMgr.LocationListNoAssociated, locationNoAssociatedViewDTO);
                //    }
                //}
                //else
                //{
                //    this.lblError.Text = lblErrorNoZoneSelected.Text;
                //    lblError.Visible = true;
                //}
            }
            catch (Exception ex)
            {
                //TODO:Agregar manejador de errores
            }
        }

        protected void SearchLocationsAssociated(InventoryOrder inventory)
        {
            try
            {
                locationAssociated = new List<Location>();
                GenericViewDTO<Binaria.WMSTek.Framework.Entities.Inventory.InventoryLocation> locationAssociatedViewDTO = iInventoryMGR.GetLocationByWhsAndInventory(inventory.Warehouse.Id, inventory.Id, context);

                if (locationAssociatedViewDTO.Entities != null)
                {
                    foreach (var loc in locationAssociatedViewDTO.Entities)
                    {
                        locationAssociated.Add(loc.Location);
                    }

                    lstLocAsociated.DataSource = lstItems(locationAssociated);
                    lstLocAsociated.DataTextField = "Text";
                    lstLocAsociated.DataValueField = "Value";
                    lstLocAsociated.DataBind();

                    Session.Add(WMSTekSessions.InventoryMgr.LocationListAssociated, locationAssociated);
                }

                lblCountAssigned.Text =  locationAssociated.Count().ToString();
            }
            catch (Exception ex)
            {
                //TODO:Agregar manejador de errores
            }
        }
        #endregion

        #region "Eventos"
        protected void btnCancel1_Click(object sender, EventArgs e)
       {
           divEditNew.Visible = false;
           modalPopUp.Hide();
       }

       protected void btnExit_Click(object sender, EventArgs e)
       {
           divEditNew.Visible = false;
           modalPopUp.Hide();
       }

       protected void chkIsFullWhs_CheckedChanged(object sender, EventArgs e)
       {
           if (chkIsFullWhs.Checked)
           {
               if (this.ddlOwner.Items.Count <= 2)
               {
                   ddlOwner.Enabled = false;
               }
               else
               {
                   ddlOwner.Enabled = true;
               }

               this.ddlOwner.SelectedValue = "1";               
           }
           else
           {
               ddlOwner.Enabled = true;
               this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;
           }
           divEditNew.Visible = true;
           modalPopUp.Show();
       }

        #endregion

        protected void btnSearchLocation_Click(object sender, EventArgs e)
        {
            try
            {
                bool IsValid = true;
                inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryMgr.InventoryList];
                string levelFrom = this.ddlLevelFrom.SelectedValue.Trim();
                string levelTo =this.ddlLevelTo.SelectedValue.Trim();
                string columnFrom = this.ddlColumnFrom.SelectedValue.Trim();
                string columnTo = this.ddlColumnTo.SelectedValue.Trim();
                string rowFrom = this.ddlRowFrom.SelectedValue.Trim();
                string rowTo = this.ddlRowTo.SelectedValue.Trim();
                var lstTypeLoc = GetConst("TypeLocationInventory");
                int idWhs = inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)].Warehouse.Id;
                int idInvent = inventoryViewDTO.Entities[int.Parse(this.hfIndex.Value)].Id;

                this.Master.ucMainFilter.ClearFilterObject();
                
                //Hangar
                this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "HANGAR").FilterValues.Add(new FilterItem(this.ddlHangar.SelectedValue.Trim()));

                ////Tipo Ubicacion
                this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONTYPE").FilterValues.Add(new FilterItem(ddlLocationType.SelectedValue.Trim()));

                if (!string.IsNullOrEmpty(this.txtLocation.Text.Trim()))
                {
                    this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "CODE").FilterValues.Add(new FilterItem(this.txtLocation.Text.Trim()));
                }

                if (!string.IsNullOrEmpty(txtLocationFrom.Text.Trim()) || !string.IsNullOrEmpty(txtLocationTo.Text.Trim()))
                {
                    if (!string.IsNullOrEmpty(txtLocationFrom.Text.Trim()) && !string.IsNullOrEmpty(txtLocationTo.Text.Trim()))
                    {
                        this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONRANGE").FilterValues.Add(new FilterItem(this.txtLocationFrom.Text.Trim()));
                        this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONRANGE").FilterValues.Add(new FilterItem(this.txtLocationTo.Text.Trim()));
                    }else
                    {
                        this.lblErrorLocation.Text = this.lblRangoLocation.Text;
                        IsValid = false;
                    }
                }

                //validaciones
                if (levelTo !="-1" || levelFrom != "-1")
                {
                    if(levelTo != "-1" && levelFrom != "-1")
                    {
                        if (Convert.ToInt32(levelTo) < Convert.ToInt32(levelFrom))
                        {
                            this.lblErrorLocation.Text = this.lblErrorLevel.Text;
                            IsValid = false;
                        }
                        else
                        {
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONLEVELRANGE").FilterValues.Add(new FilterItem(levelFrom));
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONLEVELRANGE").FilterValues.Add(new FilterItem(levelTo));
                        }
                    }
                    else
                    {
                        if (levelTo != "-1")
                        {
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONLEVELRANGE").FilterValues.Add(new FilterItem(levelTo));
                        }
                        else if (levelFrom != "-1")
                        {
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONLEVELRANGE").FilterValues.Add(new FilterItem(levelFrom));
                        }
                    }
                }

                if (columnTo != "-1" || columnFrom != "-1")
                {
                    if (columnTo != "-1" && columnFrom != "-1")
                    {
                        if (Convert.ToInt32(columnTo) < Convert.ToInt32(columnFrom))
                        {
                            this.lblErrorLocation.Text = this.lblErrorColunm.Text;
                            IsValid = false;
                        }
                        else
                        {
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONCOLUMNRANGE").FilterValues.Add(new FilterItem(columnFrom));
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONCOLUMNRANGE").FilterValues.Add(new FilterItem(columnTo));
                        }
                    }
                    else
                    {
                        if (columnTo != "-1")
                        {
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONCOLUMNRANGE").FilterValues.Add(new FilterItem(columnTo));
                        }
                        else if (columnFrom != "-1")
                        {
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONCOLUMNRANGE").FilterValues.Add(new FilterItem(columnFrom));
                        }
                    }
                }

                if (rowTo != "-1" || rowFrom != "-1")
                {
                    if (rowTo != "-1" && rowFrom != "-1")
                    {
                        if (Convert.ToInt32(rowTo) < Convert.ToInt32(rowFrom))
                        {
                            this.lblErrorLocation.Text = this.lblErrorRow.Text;
                            IsValid = false;
                        }
                        else
                        {                           
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONROWRANGE").FilterValues.Add(new FilterItem(rowFrom));
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONROWRANGE").FilterValues.Add(new FilterItem(rowTo));
                        }
                    }
                    else
                    {
                        if (rowTo != "-1")
                        {
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONROWRANGE").FilterValues.Add(new FilterItem(rowTo));
                        }
                        else if (rowFrom != "-1")
                        {
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONROWRANGE").FilterValues.Add(new FilterItem(rowFrom));
                        }
                    }
                }

                context.MainFilter = this.Master.ucMainFilter.MainFilter;

                if (IsValid)
                {                    
                    //iInventoryMGR.
                    GenericViewDTO<Location> locNotAssig = iLayoutMGR.GetLocationNotExistInInventoryLocation(idWhs, idInvent, context);

                    this.lstLocNoAsociated.DataSource = lstItems(locNotAssig.Entities);
                    this.lstLocNoAsociated.DataTextField = "Text";
                    this.lstLocNoAsociated.DataValueField = "Value";
                    this.lstLocNoAsociated.DataBind();

                    lblCountAssigned.Text = locationAssociated.Count().ToString();
                    lblCountNoAssigned.Text = locNotAssig.Entities.Count().ToString();

                    Session.Add(WMSTekSessions.InventoryMgr.LocationListNoAssociated, locNotAssig.Entities);

                    this.divErrorLocation.Visible = false;
                }
                else
                {
                    this.divErrorLocation.Visible = true;
                }

                divLocations.Visible = true;
                modalPopUp2.Show();
            }
            catch (Exception ex)
            {
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
            }
        }

        private ListItemCollection lstItems (List<Location> lstLocation)
        {
            ListItemCollection lstReturn = new ListItemCollection();

            foreach (Location loc in lstLocation)
            {
                ListItem newItem = new ListItem();
                newItem.Text = loc.IdCode + " - " + loc.Type.LocTypeCode;
                newItem.Value = loc.IdCode;

                lstReturn.Add(newItem);
            }

            return lstReturn;
        }

    }
}
