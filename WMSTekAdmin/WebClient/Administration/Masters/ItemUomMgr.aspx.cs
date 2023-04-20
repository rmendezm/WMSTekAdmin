using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class ItemUomMgr : BasePage
    {
        #region "Declaración de Variables"

        GenericViewDTO<ItemUom> itemUomViewDTO = new GenericViewDTO<ItemUom>();
        private GenericViewDTO<Item> itemViewDTO = new GenericViewDTO<Item>();
        private BaseViewDTO configurationViewDTO = new BaseViewDTO();
        private bool isValidViewDTO = true;
        

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
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
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
                        PopulateGrid();
                        LoadUomByIdItem(currentIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
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
                        LoadUomByIdItem(currentIndex);
                    }
                }
                if (!Page.IsPostBack)
                {
                    lblEmpty.Visible = false;
                }

                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
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
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        /// <summary>
        /// Limpia el filtro y carga los datos desde base de datos
        /// </summary>
        /// 
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        /// <summary>
        /// Persiste los cambios en la entidad (nueva o editada)
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveChanges();
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        /// <summary>
        /// Abre la ventana modal para crear una nueva entidad
        /// </summary>
        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                // currentIndex --> Item actual.
                ShowModal(currentIndex, -1, CRUD.Create);
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
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
                    e.Row.Attributes.Add("onclick", "gridViewDetailOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion de la grilla de item uom
                int currentUom = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;
                //Se envian 2 pociciones, para cada entidad.
                ShowModal(currentIndex, currentUom, CRUD.Update);
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
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
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        protected void grdItem_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");

                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "');");
                    e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdItem, "Select$" + e.Row.RowIndex);
                }
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        protected void grdItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //capturo la posicion de la fila 
                    int index = grdItem.PageSize * grdItem.PageIndex + grdItem.SelectedIndex;
                    currentIndex = index; //grdItem.SelectedIndex;
                    divUom.Visible = true;
                    lblEmpty.Visible = false;
                    isValidViewDTO = true;
                    ClearGridViewDetail();
                }
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                divUom.Visible = false;
                PopulateGrid();
                ClearGridViewDetail();
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                divUom.Visible = false;
                PopulateGrid();
                ClearGridViewDetail();
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                divUom.Visible = false;
                PopulateGrid();
                ClearGridViewDetail();
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                divUom.Visible = false;
                PopulateGrid();
                ClearGridViewDetail();
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdItem.PageCount - 1;
                currentIndex = -1;
                divUom.Visible = false;
                PopulateGrid();
                ClearGridViewDetail();
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            base.LoadUserOwners(this.ddlOwnerLoad, this.Master.EmptyRowText, "-1", true, string.Empty, false);

            // Selecciona owner seleccionado en el Filtro
            //if (context.MainFilter != null)
            //{
            //    UpdateSession(false);
            //}

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.ddlOwnerLoad.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

            divLoad.Visible = true;
            modalPopUpLoad.Show();
        }

        protected void btnSubir2_Click(object sender, EventArgs e)
        {
            string pathAux = "";

            try
            {
                string errorUp = "";

                if (uploadFile2.HasFile)
                {
                    int idOwn = int.Parse(this.ddlOwnerLoad.SelectedValue);

                    string savePath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("UpLoadItemFilePath", "");
                    savePath += uploadFile2.FileName;
                    pathAux = savePath;

                    try
                    {
                        uploadFile2.SaveAs(savePath);
                    }
                    catch (Exception ex)
                    {
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemUomMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidDataException(ex.Message);
                    }


                    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                                "Data Source=" + savePath + ";" +
                                                "Extended Properties=Excel 8.0;";

                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter("SELECT * FROM [Hoja1$]", connectionString);
                    DataSet myDataSet = new DataSet();
                    DataTable dataTable;

                    try
                    {
                        dataAdapter.Fill(myDataSet);
                        dataTable = myDataSet.Tables["Table"];
                    }
                    catch (Exception ex)
                    {
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemUomMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       ItemCode = r.Field<object>("ItemCode"),
                                       UomCode = r.Field<object>("UomCode"),
                                       ConversionFactor = r.Field<object>("ConversionFactor"),
                                       BarCode = r.Field<object>("BarCode"),
                                       UomName = r.Field<object>("UomName"),
                                       Length = r.Field<object>("Length"),
                                       Width = r.Field<object>("Width"),
                                       Height = r.Field<object>("Height"),
                                       Volume = r.Field<object>("Volume"),
                                       Weight = r.Field<object>("Weight"),
                                       Status = r.Field<object>("Status"),
                                       LayoutUomQty = r.Field<object>("LayoutUomQty"),
                                       LayoutUnitQty = r.Field<object>("LayoutUnitQty"),
                                       LayoutMaxWeightUpon = r.Field<object>("LayoutMaxWeightUpon"),
                                       PutawayZone = r.Field<object>("PutawayZone"),
                                       PickArea = r.Field<object>("PickArea"),
                                       SpecialField1 = r.Field<object>("SpecialField1"),
                                       SpecialField2 = r.Field<object>("SpecialField2"),
                                       SpecialField3 = r.Field<object>("SpecialField3"),
                                       SpecialField4 = r.Field<object>("SpecialField4"),
                                       BigTicket = r.Field<object>("BigTicket")
                                   };

                    itemUomViewDTO = new GenericViewDTO<ItemUom>();

                    try
                    {
                        foreach (var item in lstExcel)
                        {
                            ItemUom newItem = new ItemUom();

                            if (!ValidateIsNotNull(item.UomCode))
                            {
                                errorUp = "UomCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.Code = item.UomCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.ItemCode))
                            {
                                errorUp = "UomCode " + item.UomCode + " - ItemCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.Item = new Item();
                                newItem.Item.Code = item.ItemCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.ConversionFactor))
                            {
                                errorUp = "UomCode " + item.UomCode + "- ConversionFactor " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                decimal ConversionFactor;
                                if (decimal.TryParse(item.ConversionFactor.ToString().Trim(), out ConversionFactor))
                                {
                                    newItem.ConversionFactor = ConversionFactor;
                                }
                                else
                                {
                                    errorUp = "UomCode " + item.UomCode + " - ConversionFactor " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (!ValidateIsNotNull(item.BarCode))
                            {
                                errorUp = "UomCode " + item.UomCode + " - BarCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.BarCode = item.BarCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.UomName))
                            {
                                errorUp = "UomCode " + item.UomCode + " - UomName " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.Name = item.UomName.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.Length))
                            {
                                decimal Length;
                                if (decimal.TryParse(item.Length.ToString().Trim(), out Length))
                                {
                                    newItem.Length = Length;
                                }
                                else
                                {
                                    errorUp = "UomCode " + item.UomCode + " - Length " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(item.Width))
                            {
                                decimal Width;
                                if (decimal.TryParse(item.Width.ToString().Trim(), out Width))
                                {
                                    newItem.Width = Width;
                                }
                                else
                                {
                                    errorUp = "UomCode " + item.UomCode + " - Width " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (!ValidateIsNotNull(item.Height))
                            {
                                errorUp = "UomCode " + item.UomCode + " - Height " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                decimal Height;
                                if (decimal.TryParse(item.Height.ToString().Trim(), out Height))
                                {
                                    newItem.Height = Height;
                                }
                                else
                                {
                                    errorUp = "UomCode " + item.UomCode + " - Height " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(item.Volume))
                            {
                                decimal Volume;
                                if (decimal.TryParse(item.Volume.ToString().Trim(), out Volume))
                                {
                                    newItem.Volume = Volume;
                                }
                                else
                                {
                                    errorUp = "UomCode " + item.UomCode + " - Volume " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(item.Weight))
                            {
                                decimal Weight;
                                if (decimal.TryParse(item.Weight.ToString().Trim(), out Weight))
                                {
                                    newItem.Weight = Weight;
                                }
                                else
                                {
                                    errorUp = "UomCode " + item.UomCode + " - Weight " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(item.LayoutUomQty))
                            {
                                int LayoutUomQty;
                                if (int.TryParse(item.LayoutUomQty.ToString().Trim(), out LayoutUomQty))
                                {
                                    newItem.UomQty = LayoutUomQty;
                                }
                                else
                                {
                                    errorUp = "UomCode " + item.UomCode + " - LayoutUomQty " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(item.LayoutUnitQty))
                            {
                                int LayoutUnitQty;
                                if (int.TryParse(item.LayoutUnitQty.ToString().Trim(), out LayoutUnitQty))
                                {
                                    newItem.UnitQty = LayoutUnitQty;
                                }
                                else
                                {
                                    errorUp = "UomCode " + item.UomCode + " - LayoutUnitQty " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(item.LayoutMaxWeightUpon))
                            {
                                int LayoutMaxWeightUpon;
                                if (int.TryParse(item.LayoutMaxWeightUpon.ToString().Trim(), out LayoutMaxWeightUpon))
                                {
                                    newItem.MaxWeightUpon = LayoutMaxWeightUpon;
                                }
                                else
                                {
                                    errorUp = "UomCode " + item.UomCode + " - LayoutMaxWeightUpon " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(item.Status))
                            {
                                bool Status;

                                if (item.Status.ToString().Trim() == "0")
                                {
                                    newItem.Status = false;
                                }
                                else if (item.Status.ToString().Trim() == "1")
                                {
                                    newItem.Status = true;
                                }
                                else
                                {
                                    if (bool.TryParse(item.Status.ToString().Trim(), out Status))
                                    {
                                        newItem.Status = Status;
                                    }
                                    else
                                    {
                                        errorUp = "UomCode " + item.UomCode + " - Status " + this.lblFieldInvalid.Text;
                                        break;
                                    }
                                }    
                            }

                            if (ValidateIsNotNull(item.PutawayZone))
                                newItem.PutawayZone = item.PutawayZone.ToString().Trim();

                            if (ValidateIsNotNull(item.PickArea))
                                newItem.PickArea = item.PickArea.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField1))
                                newItem.SpecialField1 = item.SpecialField1.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField2))
                                newItem.SpecialField2 = item.SpecialField2.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField3))
                                newItem.SpecialField3 = item.SpecialField3.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField4))
                                newItem.SpecialField4 = item.SpecialField4.ToString().Trim();

                            if (ValidateIsNotNull(item.BigTicket))
                            {
                                bool bigTicket;

                                if (item.BigTicket.ToString().Trim() == "0")
                                {
                                    newItem.BigTicket = false;
                                }
                                else if (item.BigTicket.ToString().Trim() == "1")
                                {
                                    newItem.BigTicket = true;
                                }
                                else
                                {
                                    if (bool.TryParse(item.BigTicket.ToString().Trim(), out bigTicket))
                                    {
                                        newItem.BigTicket = bigTicket;
                                    }
                                    else
                                    {
                                        errorUp = "UomCode " + item.UomCode + " - BigTicket " + this.lblFieldInvalid.Text;
                                        break;
                                    }
                                }
                            }

                            itemUomViewDTO.Entities.Add(newItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }

                    if (errorUp != "")
                    {
                        ShowAlertLocal(this.lblTitle.Text, errorUp);
                        divFondoPopupProgress.Visible = false;
                        divLoad.Visible = true;
                        modalPopUpLoad.Show();
                    }
                    else
                    {
                        if (itemUomViewDTO.Entities.Count > 0)
                        {
                            itemUomViewDTO = iWarehousingMGR.MaintainItemUomMassive(itemUomViewDTO, idOwn, context);

                            if (itemUomViewDTO.hasError())
                            {
                                //UpdateSession(true);
                                ShowAlertLocal(this.lblTitle.Text, itemUomViewDTO.Errors.Message);
                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                ucStatus.ShowMessage(itemUomViewDTO.MessageStatus.Message);
                                ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = false;
                                modalPopUpLoad.Hide();
                            }
                        }
                        else
                        {
                            ShowAlertLocal(this.lblTitle.Text, this.lblNotItemsFile.Text);
                            divFondoPopupProgress.Visible = false;
                            divLoad.Visible = true;
                            modalPopUpLoad.Show();
                        }
                    }
                }
                else
                {
                    ShowAlertLocal(this.lblTitle.Text, this.lblNotFileLoad.Text);
                    divFondoPopupProgress.Visible = false;
                    divLoad.Visible = true;
                    modalPopUpLoad.Show();
                }
            }
            catch (InvalidOperationException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divFondoPopupProgress.Visible = false;
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, itemViewDTO.Errors.Message);
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
        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeLayout();
            InitializeTypeUnitOfMeasure_OfMass();
        }

        protected void LoadUomByIdItem(int index)
        {
            if (index != -1)
            {
                int id = itemViewDTO.Entities[index].Id;

                itemUomViewDTO = iWarehousingMGR.GetItemUomByIdItem(context, id);
                this.lblItemName.Text = itemViewDTO.Entities[index].LongName;

                if (itemUomViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!itemUomViewDTO.hasConfigurationError() && itemUomViewDTO.Configuration != null && itemUomViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdMgr, itemUomViewDTO.Configuration);

                    // Detalle de Recepciones
                    grdMgr.DataSource = itemUomViewDTO.Entities;
                    grdMgr.DataBind();

                    CallJsGridViewDetail();
                }
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
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
                itemUomViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            itemViewDTO = iWarehousingMGR.FindAllItem(context);

            if (!itemViewDTO.hasError() && itemViewDTO.Entities != null)
            {

                Session.Add(WMSTekSessions.ItemUomMgr.ItemUomList, itemViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(itemViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.statusVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.codeVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;
            if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
            {
                this.Master.ucMainFilter.codeLabel = this.lblGtin.Text;
            }
            else
            {
                this.Master.ucMainFilter.codeLabel = this.lblFilterBarCode.Text;
            }

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
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            //this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);

            this.Master.ucTaskBar.btnNewVisible = true;
            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnAddVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnAddToolTip = this.lblAddLoadToolTip.Text;
            //this.Master.ucTaskBar.btnExcelDetailVisible = true;
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

            // Mensaje a mostrar en los mantenedores que no cargan inicialmente la grilla
            itemViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(itemViewDTO.MessageStatus.Message);
        }

        /// <summary>
        /// Configuracion inicial de la Grilla principal
        /// </summary>
        private void InitializeGrid()
        {
            grdItem.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdItem.EmptyDataText = this.Master.EmptyGridText;
        }

        /// <summary>
        /// Configuracion inicial del layout
        /// </summary>
        private void InitializeLayout()
        {
            if (itemViewDTO.Configuration == null)
            {
                configurationViewDTO = iConfigurationMGR.GetLayoutConfiguration("ItemUom_GetItemUomByIdItem", context);
                if (!configurationViewDTO.hasConfigurationError()) itemViewDTO.Configuration = configurationViewDTO.Configuration;
            }
        }

        private void InitializeTypeUnitOfMeasure_OfMass()
        {
            String type = string.Empty;
            String typeMass = string.Empty;

            var lstTypeLoc = GetConst("TypeOfUnitOfMeasure");
            var lstTypeMass = GetConst("TypeOfUnitOfMass");

            if (lstTypeLoc.Count == 0)
                type = "(mts)";
            else
                type = "(" + lstTypeLoc[0].Trim() + ")";

            if (lstTypeMass.Count == 0)
                typeMass = "(k)";
            else
                typeMass = "(" + lstTypeMass[0].Trim() + ")";


            this.lblTypeUnitMeasure.Text = type;
            this.lblTypeUnitMeasure2.Text = type;
            this.lblTypeUnitMeasure3.Text = type;

            this.lblTypeUnitOfMass.Text = typeMass;
            this.lblTypeUnitOfMass2.Text = typeMass;

        }

        private void PopulateGrid()
        {
            itemViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.ItemUomMgr.ItemUomList];

            if (itemViewDTO == null)
                return;

            grdItem.PageIndex = currentPage;
            grdItem.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!itemViewDTO.hasConfigurationError() && itemViewDTO.Configuration != null && itemViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdItem, itemViewDTO.Configuration);

            grdItem.DataSource = itemViewDTO.Entities;
            grdItem.DataBind();


            ucStatus.ShowRecordInfo(itemViewDTO.Entities.Count, grdItem.PageSize, grdItem.PageCount, currentPage, grdItem.AllowPaging);

            CallJsGridViewHeader();
        }

        protected void ReloadData()
        {
            //Como no es Actualizacion, setea la variable para que muestre "Listo"
            crud = false;

            //Actualiza grilla
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divUom.Visible = false;
                currentPage = 0;
                currentIndex = -1;
                this.Master.ucError.ClearError();
                ClearGridViewDetail();
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, int indexUom, CRUD mode)
        {
            if (currentIndex != -1)
            {
                itemViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.ItemUomMgr.ItemUomList];
                //Editar UOM
                if (mode == CRUD.Update)
                {
                    if (indexUom != -1) index = indexUom;  

                    //Recupera los datos de la entidad a editar
                    hidEditId.Value = itemUomViewDTO.Entities[index].Id.ToString();

                    txtUomCode.Text = itemUomViewDTO.Entities[index].Code;
                    txtConversionFactor.Text = itemUomViewDTO.Entities[index].ConversionFactor.ToString();
                    txtBarCode.Text = itemUomViewDTO.Entities[index].BarCode.ToString();
                    LoadUomType(ddlUomName, itemViewDTO.Entities[currentIndex].Owner.Id, false, lblEmpty.Text);
                    ddlUomName.SelectedValue = itemUomViewDTO.Entities[index].UomType.Id.ToString();

                    if (itemUomViewDTO.Entities[index].Length == -1) this.txtLength.Text = string.Empty;
                    else { txtLength.Text = itemUomViewDTO.Entities[index].Length.ToString(); }

                    if (itemUomViewDTO.Entities[index].Width == -1) txtWidth.Text = string.Empty;
                    else { txtWidth.Text = itemUomViewDTO.Entities[index].Width.ToString(); }

                    if (itemUomViewDTO.Entities[index].Height == -1) txtHeight.Text = string.Empty;
                    else { txtHeight.Text = itemUomViewDTO.Entities[index].Height.ToString(); }

                    if (itemUomViewDTO.Entities[index].Volume == -1) this.txtVolume.Text = string.Empty;
                    else { txtVolume.Text = itemUomViewDTO.Entities[index].Volume.ToString(); }

                    if (itemUomViewDTO.Entities[index].Weight == -1) txtWeight.Text = string.Empty;
                    else { txtWeight.Text = itemUomViewDTO.Entities[index].Weight.ToString(); }

                    chkStatus.Checked = Convert.ToBoolean(itemUomViewDTO.Entities[index].Status);

                    if (itemUomViewDTO.Entities[index].UomQty == -1) txtLayoutUomQty.Text = string.Empty;
                    else { txtLayoutUomQty.Text = itemUomViewDTO.Entities[index].UomQty.ToString(); }

                    if (itemUomViewDTO.Entities[index].UnitQty == -1) txtLayoutUnitQty.Text = string.Empty;
                    else { txtLayoutUnitQty.Text = itemUomViewDTO.Entities[index].UnitQty.ToString(); }

                    if (itemUomViewDTO.Entities[index].MaxWeightUpon == -1) txtLayoutMaxWeightUpon.Text = string.Empty;
                    else { txtLayoutMaxWeightUpon.Text = itemUomViewDTO.Entities[index].MaxWeightUpon.ToString(); }

                    chkBigTicket.Checked = itemUomViewDTO.Entities[index].BigTicket;


                    //txtPutawayZone.Text = itemUomViewDTO.Entities[index].PutawayZone;
                    //txtPickArea.Text = itemUomViewDTO.Entities[index].PickArea;

                    lblNew.Visible = false;
                    lblEdit.Visible = true;
                }

                // Nueva UOM
                if (mode == CRUD.Create)
                {
                    hidEditId.Value = "0";
                    txtUomCode.Text = string.Empty;
                    txtConversionFactor.Text = string.Empty;
                    txtBarCode.Text = string.Empty;
                    LoadUomType(ddlUomName, itemViewDTO.Entities[currentIndex].Owner.Id, false, lblEmpty.Text);
                    txtLength.Text = string.Empty;
                    txtWidth.Text = string.Empty;
                    txtHeight.Text = string.Empty;
                    txtVolume.Text = string.Empty;
                    txtWeight.Text = string.Empty;
                    chkStatus.Checked = true;
                    txtLayoutUomQty.Text = string.Empty;
                    txtLayoutUnitQty.Text = string.Empty;
                    txtLayoutMaxWeightUpon.Text = string.Empty;
                    //txtPutawayZone.Text = string.Empty;
                    //txtPickArea.Text = string.Empty;
                    lblNew.Visible = true;
                    lblEdit.Visible = false;
                    chkBigTicket.Checked = false;
                }


                if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
                {
                    this.lblBarCode.Text = this.lblGtin.Text;
                    this.rfvBarCode.ErrorMessage = this.lblGtinMessageRequired.Text;
                    this.revtxtBarCode.ErrorMessage = this.lblGtinMessageRegularExpression.Text;
                }
                else
                {
                    this.rfvBarCode.ErrorMessage = this.lblBarCodeMessageRequired.Text;
                    this.revtxtBarCode.ErrorMessage = this.lblBarCodeMessageRegularExpression.Text;
                }

                if (itemUomViewDTO.Configuration != null && itemUomViewDTO.Configuration.Count > 0)
                {
                    if (mode == CRUD.Create)
                        base.ConfigureModal(itemUomViewDTO.Configuration, true);
                    else
                        base.ConfigureModal(itemUomViewDTO.Configuration, false);
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
                pnl.Visible = true;
                lblEmpty.Visible = false;
            }
            else
            {
                divUom.Visible = true;
                lblEmpty.Visible = true;
            }
        }

        /// <summary>
        /// Persiste los cambios realizados en la entidad
        /// </summary>
        /// <param name="index"></param>
        protected void SaveChanges()
        {
            ItemUom itemUom = new ItemUom();
            itemUom.UomType = new UomType();
            itemUom.Item = new Item();
            itemUom.Item.Owner = new Owner();
            itemViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.ItemUomMgr.ItemUomList];

            //Agrega los datos del Uom
            itemUom.Id = Convert.ToInt32(hidEditId.Value);
            itemUom.Item.Id = itemViewDTO.Entities[currentIndex].Id;
            itemUom.Item.Code = itemViewDTO.Entities[currentIndex].Code;
            itemUom.Item.Owner = itemViewDTO.Entities[currentIndex].Owner;
            itemUom.Code = this.txtUomCode.Text;                      
            itemUom.UomType.Id = Convert.ToInt16(this.ddlUomName.SelectedValue);
            itemUom.UomType.Name = this.ddlUomName.SelectedItem.ToString();
            itemUom.Status = this.chkStatus.Checked;

            if (this.txtConversionFactor.Text != "")
                itemUom.ConversionFactor = Convert.ToDecimal(this.txtConversionFactor.Text);

            if (this.txtLength.Text != "")
                itemUom.Length = Convert.ToDecimal(this.txtLength.Text);

            if (this.txtWidth.Text != "")
                itemUom.Width = Convert.ToDecimal(this.txtWidth.Text);

            if (this.txtHeight.Text != "")
                itemUom.Height = Convert.ToDecimal(this.txtHeight.Text);

            if (this.txtVolume.Text != "")
                itemUom.Volume = Convert.ToDecimal(this.txtVolume.Text);

            if (this.txtWeight.Text != "")
                itemUom.Weight = Convert.ToDecimal(this.txtWeight.Text);            

            if (this.txtLayoutUomQty.Text != "")
                itemUom.UomQty = Convert.ToInt32(this.txtLayoutUomQty.Text);

            if (this.txtLayoutUnitQty.Text != "")
                itemUom.UnitQty = Convert.ToInt32(this.txtLayoutUnitQty.Text);

            if (this.txtLayoutMaxWeightUpon.Text != "")
                itemUom.MaxWeightUpon = Convert.ToDecimal(this.txtLayoutMaxWeightUpon.Text);

            itemUom.BigTicket = chkBigTicket.Checked;

            //if (this.txtPutawayZone.Text != "")
            //    itemUom.PutawayZone = this.txtPutawayZone.Text;
            //if (this.txtPickArea.Text != "")
            //    itemUom.PickArea = this.txtPickArea.Text;

            // Si el parametro se encuentra activo el codigo de barra se valida con estandar GS1
            if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
            {
                string resultBarCode = ValidateBarCodeItem(this.txtBarCode.Text.Trim());

                if (string.IsNullOrEmpty(resultBarCode))
                {
                    if (this.txtBarCode.Text.Trim().Length == 14 && Convert.ToDecimal(this.txtConversionFactor.Text.Trim()) == 1)
                    {
                        RequiredFieldValidator val = new RequiredFieldValidator();
                        val.ErrorMessage = this.lblErrorConversionFactorGTIN.Text;
                        val.ControlToValidate = "ctl00$MainContent$txtBarCode";
                        val.IsValid = false;
                        val.ValidationGroup = "EditNew";
                        this.Page.Validators.Add(val);

                        //rfvBarCode.IsValid = false;
                        divEditNew.Visible = true;
                        modalPopUp.Show();
                        return;
                    }

                    itemUom.BarCode = this.txtBarCode.Text;
                }
                else
                {
                    RequiredFieldValidator val = new RequiredFieldValidator();
                    //CustomValidator val = new CustomValidator();
                    val.ErrorMessage = resultBarCode;
                    val.ControlToValidate = "ctl00$MainContent$txtBarCode";
                    val.IsValid = false;
                    val.ValidationGroup = "EditNew";
                    this.Page.Validators.Add(val);

                    //rfvBarCode.IsValid = false;
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                    return;
                }
            }
            else
            {
                itemUom.BarCode = this.txtBarCode.Text;
            }

            if (hidEditId.Value == "0")
            {
                itemUomViewDTO = iWarehousingMGR.MaintainItemUom(CRUD.Create, itemUom, context);
            }
            else
            {
                itemUomViewDTO = iWarehousingMGR.MaintainItemUom(CRUD.Update, itemUom, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (itemUomViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            } 
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(itemUomViewDTO.MessageStatus.Message);
                //Actualiza grilla
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            itemUomViewDTO = iWarehousingMGR.MaintainItemUom(CRUD.Delete, itemUomViewDTO.Entities[index], context);

            if (itemUomViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(itemUomViewDTO.MessageStatus.Message);

                //Actualiza grilla
                UpdateSession(false);
            }
        }

       
        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('Item_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdItem');", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDropCustom();", true);
        }

        private void ClearGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "clearFilterDetail", "clearFilterDetail('ctl00_MainContent_hsMasterDetail_bottomPanel_ctl01_grdMgr');", true);
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                var itemUomViewDTO = iWarehousingMGR.FindAllItemUom(context);

                if (!itemUomViewDTO.hasError())
                {
                    base.ExportToExcel(ConvertItemsUomIntoGridview(itemUomViewDTO), null, null, true);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                itemUomViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemUomViewDTO.Errors);
            }
        }

        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            GridView grdMgrAux = new GridView();
            GenericViewDTO<ItemUom> itemUomViewDTO = new GenericViewDTO<ItemUom>();
            GenericViewDTO<Item> itemViewDTO = new GenericViewDTO<Item>();
            //CustomRule theCustomRule = new CustomRule();
            string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                grdMgrAux = grdMgr;

                if (grdMgrAux.Rows.Count > 0)
                {
                    base.ExportToExcel(grdItem, grdMgrAux, lblItemName.Text, true);
                    grdMgr.AllowPaging = true;
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        private GridView ConvertItemsUomIntoGridview(GenericViewDTO<ItemUom> itemsUom)
        {
            if (itemsUom.Entities.Count > 0)
            {
                GridView grdMgrAux = new GridView();
                grdMgrAux.AutoGenerateColumns = false;

                DataTable dt = new DataTable();

                var columns = new DataColumn[] {
                    new DataColumn("Código Item", typeof(string)),
                    new DataColumn("Nombre Item", typeof(string)),
                    new DataColumn("Alto", typeof(decimal)),
                    new DataColumn("Código Uom", typeof(string)),
                    new DataColumn("Factor de conversión", typeof(decimal)),
                    new DataColumn("Código de barra", typeof(string)),
                    new DataColumn("Nombre", typeof(string)),
                    new DataColumn("Ancho", typeof(decimal)),
                    new DataColumn("Largo", typeof(decimal)),
                    new DataColumn("Volumen", typeof(decimal)),
                    new DataColumn("Peso", typeof(decimal)),
                    new DataColumn("Max. de presentaciones por base pallet", typeof(int)),
                    new DataColumn("Max. cantidad apilada", typeof(int)),
                    new DataColumn("Max. peso apilado", typeof(int)),
                    new DataColumn("ID", typeof(int))
                };

                dt.Columns.AddRange(columns);

                foreach (ItemUom itemUom in itemsUom.Entities)
                {
                    DataRow oItem = dt.NewRow();
                    oItem[0] = itemUom.Item.Code;
                    oItem[1] = itemUom.Item.ShortName;
                    oItem[2] = itemUom.Item.Height;
                    oItem[3] = itemUom.Code;
                    oItem[4] = itemUom.ConversionFactor;
                    oItem[5] = itemUom.BarCode;
                    oItem[6] = itemUom.Name;
                    oItem[7] = itemUom.Item.Width;
                    oItem[8] = itemUom.Item.Length;
                    oItem[9] = itemUom.Item.Volume;
                    oItem[10] = itemUom.Item.Weight;
                    oItem[11] = itemUom.UomQty;
                    oItem[12] = itemUom.UnitQty;
                    oItem[13] = itemUom.MaxWeightUpon;
                    oItem[14] = itemUom.Id;
                    dt.Rows.Add(oItem);
                }

                foreach (DataColumn col in dt.Columns)
                {
                    BoundField bfield = new BoundField();
                    bfield.DataField = col.ColumnName;
                    bfield.HeaderText = col.ColumnName;
                    grdMgrAux.Columns.Add(bfield);
                }

                grdMgrAux.DataSource = dt;
                grdMgrAux.DataBind();

                return grdMgrAux;
            }
            else
            {
                return null;
            }
        }
        #endregion

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;

                if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Text == "Código de barra" || e.Row.Cells[i].Text == "C&#243;digo de barra")
                        {
                            e.Row.Cells[i].Text = this.lblGtin.Text;
                        }
                    }
                }
            }
        }

        private String ValidateBarCodeItem (string barCode)
        {
            try
            {
                string result = string.Empty;
                int lengthBar = barCode.Trim().Length;                
                Regex regIsNum = new Regex(@"^[0-9]+$");

                //Valida que el codigo de barra ingresado sea numerico
                if (!regIsNum.IsMatch(barCode.Trim()))
                {
                    result = this.lblGtinIsNotNumeric.Text;
                }
                else
                {
                    if (lengthBar < 6 || lengthBar > 14)
                    {
                        result = this.lblGtinLengthInvalid.Text;
                    }
                    else
                    {
                        if (lengthBar == 6 || lengthBar == 8 || lengthBar == 12 || lengthBar == 13 || lengthBar == 14)
                        {
                            if (lengthBar == 6)
                            {
                                result = string.Empty;
                            }
                            else
                            {
                                //Rescata digito Verificador.
                                string digVerif = barCode.Substring(lengthBar - 1, 1);
                                barCode = barCode.Substring(0, lengthBar - 1);

                                int factor = 3;
                                int sum = 0;

                                for (int index = barCode.Length; index > 0; --index)
                                {
                                    sum = sum + int.Parse(barCode.Substring(index - 1, 1)) * factor;
                                    factor = 4 - factor;
                                }

                                string resultDigit = ((1000 - sum) % 10).ToString();

                                if (resultDigit != digVerif)
                                {
                                    switch (lengthBar)
                                    {
                                        case 8:
                                            result = this.lblGtinCheckDigitGTIN8.Text;
                                            break;

                                        case 12:
                                            result = this.lblGtinCheckDigitGTIN12.Text;
                                            break;

                                        case 13:
                                            result = this.lblGtinCheckDigitGTIN13.Text;
                                            break;

                                        case 14:
                                            result = this.lblGtinCheckDigitGTIN14.Text;
                                            break;
                                    }                                    
                                }
                            }

                        }
                        else
                        {
                            result = this.lblGtinLengthInvalid.Text;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message ;
            }
        }
    }
}