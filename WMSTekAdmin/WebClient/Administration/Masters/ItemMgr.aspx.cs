using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
using Binaria.WMSTek.Framework.Entities.Rules;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.ComponentModel;
using System.Reflection;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{

    public partial class ItemMgr : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<Item> itemViewDTO = new GenericViewDTO<Item>();
        private BaseViewDTO configurationViewDTO = new BaseViewDTO();

        private bool isValidViewDTO = false;
        private bool isNew = true;
        private int idGrpItem1
        {
            get { return (int)(ViewState["idGrpItem1"] ?? -1); }
            set { ViewState["idGrpItem1"] = value; }
        }
        private int idGrpItem2
        {
            get { return (int)(ViewState["idGrpItem2"] ?? -1); }
            set { ViewState["idGrpItem2"] = value; }
        }
        private int idGrpItem3
        {
            get { return (int)(ViewState["idGrpItem3"] ?? -1); }
            set { ViewState["idGrpItem3"] = value; }
        }
        private int idGrpItem4
        {
            get { return (int)(ViewState["idGrpItem4"] ?? -1); }
            set { ViewState["idGrpItem4"] = value; }
        }

        private SortDirection sort
        {
            get { return (SortDirection)(ViewState["sortItemMgr"] ?? SortDirection.Ascending); }
            set { ViewState["sortItemMgr"] = value; }
        }

        // Propiedad para controlar el nro de pagina activa en la grilla
        private int currentPage
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
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        // Nota: este mantenedor no carga inicialmente la grilla
                        context.MainFilter = this.Master.ucMainFilter.MainFilter;
                        isNew = true;

                        tabGeneral.HeaderText = lbltabGeneral.Text;
                        tabDetails.HeaderText = this.lbltabDetails.Text;
                        tabWorkZones.HeaderText = this.lbltabWorkZone.Text;
                        tabRules.HeaderText = this.lbltabRules.Text;
                        tabVas.HeaderText = this.lbltabVas.Text;
                    }
                    else
                    {
                        if (ValidateSession(WMSTekSessions.ItemMgr.ItemList))
                        {
                            itemViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.ItemMgr.ItemList];
                            isValidViewDTO = true;
                        }

                        // Si es un ViewDTO valido, carga la grilla y las listas
                        if (isValidViewDTO)
                        {
                            if (!isNew)
                            {
                                this.idGrpItem1 = itemViewDTO.Entities[0].GrpItem1.Id;
                                this.idGrpItem2 = itemViewDTO.Entities[0].GrpItem2.Id;
                                this.idGrpItem3 = itemViewDTO.Entities[0].GrpItem3.Id;
                                this.idGrpItem4 = itemViewDTO.Entities[0].GrpItem4.Id;
                            }
                            else
                            {
                                this.idGrpItem1 = -1;
                                this.idGrpItem2 = -1;
                                this.idGrpItem3 = -1;
                                this.idGrpItem4 = -1;
                            }

                            // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                            PopulateGrid();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
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
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
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
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
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
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        // TODO: Implementar en Fase 3
        //protected void ucStatus_pageSizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        grdMgr.PageSize = ucStatus.PageSize;
        //    }
        //    catch (Exception ex)
        //    {
        //        itemViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(itemViewDTO.Errors);
        //    }
        //}

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
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
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
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
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
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
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        // Agrega la Zona al Item actual
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
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }
        protected void btnAddVas_Click(object sender, EventArgs e)
        {
            try
            {
                AddVas(Convert.ToInt32(hidEditIndex.Value));
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }

        }

        //Agragar Grupo de Reglas al Item Actual
        protected void btnAddRules_Click(object sender, EventArgs e)
        {
            try
            {
                AddRules(Convert.ToInt32(hidEditIndex.Value));
                divEditNew.Visible = true;
                modalPopUp.Show();

            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        // Quita la Zona del Item actual
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
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }        
        }

        // Quita la Zona del Item actual
        protected void grdVas_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                RemoveVas(Convert.ToInt32(hidEditIndex.Value), e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void grdWorkZones_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    // Deshabilita la opcion de Eliminar si es el Usuario Base del Rol Base
            //    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

            //    if (btnDelete != null && itemViewDTO.Entities[ddlWorkZones.SelectedIndex].IsBaseRole && itemViewDTO.Entities[ddlRole.SelectedIndex].Users[e.Row.RowIndex].IsBaseUser)
            //    {
            //        btnDelete.ImageUrl = "~/WebResources/Images/Buttons/icon_delete_des.gif";
            //        btnDelete.Enabled = false;
            //    }
            //}
        }

        protected void grdVas_RowCreated(object sender, GridViewRowEventArgs e)
        {
            
        }

        protected void ddlGrpItem1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var idOwn = -1;

                if (!string.IsNullOrEmpty(ddlOwner.SelectedValue) && ddlOwner.SelectedValue != "-1")
                    idOwn = int.Parse(ddlOwner.SelectedValue);

                base.ConfigureDDLGrpItem2New(ddlGrpItem2, isNew, Convert.ToInt32(this.ddlGrpItem1.SelectedValue), idGrpItem2, this.Master.EmptyRowText, false, idOwn);
                base.ConfigureDDLGrpItem3New(ddlGrpItem3, isNew, Convert.ToInt32(this.ddlGrpItem1.SelectedValue), idGrpItem2, idGrpItem3, this.Master.EmptyRowText, false, idOwn);
                base.ConfigureDDLGrpItem4New(ddlGrpItem4, isNew, Convert.ToInt32(this.ddlGrpItem1.SelectedValue), idGrpItem2, idGrpItem3, idGrpItem4, this.Master.EmptyRowText, false, idOwn);

                divEditNew.Visible = true;
                this.modalPopUp.Show();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ddlGrpItem2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var idOwn = -1;

                if (!string.IsNullOrEmpty(ddlOwner.SelectedValue) && ddlOwner.SelectedValue != "-1")
                    idOwn = int.Parse(ddlOwner.SelectedValue);

                base.ConfigureDDLGrpItem3New(ddlGrpItem3, isNew, Convert.ToInt32(this.ddlGrpItem1.SelectedValue), Convert.ToInt32(this.ddlGrpItem2.SelectedValue), idGrpItem3, this.Master.EmptyRowText, false, idOwn);
                base.ConfigureDDLGrpItem4New(ddlGrpItem4, isNew, idGrpItem1, Convert.ToInt32(this.ddlGrpItem2.SelectedValue), idGrpItem3, idGrpItem4, this.Master.EmptyRowText, false, idOwn);

                divEditNew.Visible = true;
                this.modalPopUp.Show();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void ddlGrpItem3_SelectedIndexChanged(object sender, EventArgs e)
         {
            try
            {
                var idOwn = -1;

                if (!string.IsNullOrEmpty(ddlOwner.SelectedValue) && ddlOwner.SelectedValue != "-1")
                    idOwn = int.Parse(ddlOwner.SelectedValue);

                base.ConfigureDDLGrpItem4New(ddlGrpItem4, isNew, Convert.ToInt32(this.ddlGrpItem1.SelectedValue), Convert.ToInt32(this.ddlGrpItem2.SelectedValue), Convert.ToInt32(this.ddlGrpItem3.SelectedValue), idGrpItem4, this.Master.EmptyRowText, false, idOwn);

                divEditNew.Visible = true;
                this.modalPopUp.Show();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void chkInspectionRequerid_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.ddlInspectionCode.SelectedValue = "-1";

                if (chkInspectionRequerid.Checked)
                {
                    this.ddlInspectionCode.Enabled = true;
                    this.rfvInspectionCode.Enabled = true;
                }
                else
                {                    
                    this.ddlInspectionCode.Enabled = false;
                    this.rfvInspectionCode.Enabled = false;
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
           }
       }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowModal(-1, CRUD.Create);
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            base.LoadUserOwners(this.ddlOwnerLoad, this.Master.EmptyRowText, "-1", true, string.Empty, false);

            // Selecciona owner seleccionado en el Filtro
            this.ddlOwnerLoad.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

            divLoad.Visible = true;
            modalPopUpLoad.Show();
        }

       

        protected void btnSubir2_Click(object sender, EventArgs e)
        {
            string pathAux = "";

            try
            {
                //string sExt = string.Empty;
                //string sName = string.Empty;
                string errorUp = "";

                if (uploadFile.HasFile)
                {
                    int idOwn = int.Parse(this.ddlOwnerLoad.SelectedValue);

                    string savePath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("UpLoadItemFilePath", "");
                    savePath += uploadFile.FileName;
                    pathAux = savePath;

                    try
                    {
                        uploadFile.SaveAs(savePath);
                    }
                    catch (Exception ex)
                    {
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
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
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       ItemCode = r.Field<object>("ItemCode"),
                                       AltItemCode = r.Field<object>("AltItemCode"),
                                       Description = r.Field<object>("Description"),
                                       GrpItem1Code = r.Field<object>("GrpItem1Code"),
                                       GrpItem2Code = r.Field<object>("GrpItem2Code"),
                                       GrpItem3Code = r.Field<object>("GrpItem3Code"),
                                       GrpItem4Code = r.Field<object>("GrpItem4Code"),
                                       LongItemName = r.Field<object>("LongItemName"),
                                       ShortItemName = r.Field<object>("ShortItemName"),
                                       Status = r.Field<object>("Status"),
                                       ItemComment = r.Field<object>("ItemComment"),
                                       ShelfLife = r.Field<object>("ShelfLife"),
                                       ExpirationDays = r.Field<object>("ExpirationDays"),
                                       CtrlSerialInbound = r.Field<object>("CtrlSerialInbound"),
                                       CtrlSerialInternal = r.Field<object>("CtrlSerialInternal"),
                                       CtrlSerialOutbound = r.Field<object>("CtrlSerialOutbound"),
                                       LotControlInbound = r.Field<object>("LotControlInbound"),
                                       LotControlInternal = r.Field<object>("LotControlInternal"),
                                       LotControlOutbound = r.Field<object>("LotControlOutbound"),
                                       Weight = r.Field<object>("Weight"),
                                       Volume = r.Field<object>("Volume"),
                                       Length = r.Field<object>("Length"),
                                       Width = r.Field<object>("Width"),
                                       Height = r.Field<object>("Height"),
                                       NestedVolume = r.Field<object>("NestedVolume"),
                                       InspectionRequerid = r.Field<object>("InspectionRequerid"),
                                       InspectionCode = r.Field<object>("InspectionCode"),
                                       //IdPutawayZone = r.Field<object>("IdPutawayZone"),
                                       ControlExpDate = r.Field<object>("ControlExpDate"),
                                       ControlFabDate = r.Field<object>("ControlFabDate"),
                                       Acumulable = r.Field<object>("Acumulable"),
                                       ReOrderPoint = r.Field<object>("ReOrderPoint"),
                                       ReOrderQty = r.Field<object>("ReOrderQty"),
                                       PalletQty = r.Field<object>("PalletQty"),
                                       CutMinimum = r.Field<object>("CutMinimum"),
                                       Originator = r.Field<object>("Originator"),
                                       VASProfile = r.Field<object>("VASProfile"),
                                       Hazard = r.Field<object>("Hazard"),
                                       Price = r.Field<object>("Price"),
                                       InventoryType = r.Field<object>("InventoryType"),
                                       StackingSequence = r.Field<object>("StackingSequence"),
                                       CommentControl = r.Field<object>("CommentControl"),
                                       CompatibilityCode = r.Field<object>("CompatibilityCode"),
                                       MSDSUrl = r.Field<object>("MSDSUrl"),
                                       PictureUrl = r.Field<object>("PictureUrl"),
                                       GrpClass1 = r.Field<object>("GrpClass1"),
                                       GrpClass2 = r.Field<object>("GrpClass2"),
                                       GrpClass3 = r.Field<object>("GrpClass3"),
                                       GrpClass4 = r.Field<object>("GrpClass4"),
                                       GrpClass5 = r.Field<object>("GrpClass5"),
                                       GrpClass6 = r.Field<object>("GrpClass6"),
                                       GrpClass7 = r.Field<object>("GrpClass7"),
                                       GrpClass8 = r.Field<object>("GrpClass8"),
                                       SpecialField1 = r.Field<object>("SpecialField1"),
                                       SpecialField2 = r.Field<object>("SpecialField2"),
                                       SpecialField3 = r.Field<object>("SpecialField3"),
                                       SpecialField4 = r.Field<object>("SpecialField4")

                                   };

                    itemViewDTO = new GenericViewDTO<Item>();
                    try
                    {
                        foreach (var item in lstExcel)
                        {
                            if (!IsValidExcelRow(item))
                                continue;

                            Item newItem = new Item();

                            if (!ValidateIsNotNull(item.ItemCode))
                            {
                                errorUp = "ItemCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.Code = item.ItemCode.ToString().Trim();
                            }

                            newItem.Owner = new Owner(idOwn);

                            if (ValidateIsNotNull(item.AltItemCode))
                                newItem.AltCode = item.AltItemCode.ToString();

                            if (!ValidateIsNotNull(item.Description))
                            {
                                errorUp = "ItemCode " + item.ItemCode + " - Description " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.Description = item.Description.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.GrpItem1Code))
                            {
                                newItem.GrpItem1 = new GrpItem1();
                                newItem.GrpItem1.Code = item.GrpItem1Code.ToString().Trim();
                            }
                            if (ValidateIsNotNull(item.GrpItem2Code))
                            {
                                newItem.GrpItem2 = new GrpItem2();
                                newItem.GrpItem2.Code = item.GrpItem2Code.ToString().Trim();
                            }
                            if (ValidateIsNotNull(item.GrpItem3Code))
                            {
                                newItem.GrpItem3 = new GrpItem3();
                                newItem.GrpItem3.Code = item.GrpItem3Code.ToString().Trim();
                            }
                            if (ValidateIsNotNull(item.GrpItem4Code))
                            {
                                newItem.GrpItem4 = new GrpItem4();
                                newItem.GrpItem4.Code = item.GrpItem4Code.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.LongItemName))
                            {
                                errorUp = "ItemCode " + item.ItemCode + " - LongItemName " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.LongName = item.LongItemName.ToString();
                            }

                            if (!ValidateIsNotNull(item.ShortItemName))
                            {
                                errorUp = "ItemCode " + item.ItemCode + " - ShortItemName " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.ShortName = item.ShortItemName.ToString();
                            }

                            if (!ValidateIsNotNull(item.Status))
                            {
                                errorUp = "ItemCode " + item.ItemCode + " - Status " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.Status = item.Status.ToString().Equals("1") ? true : false;
                            }

                            if (ValidateIsNotNull(item.ItemComment))
                                newItem.Comment = item.ItemComment.ToString().Trim();

                            if (ValidateIsNotNull(item.ShelfLife))
                                newItem.ShelfLife = Convert.ToInt32(item.ShelfLife);

                            if (ValidateIsNotNull(item.ExpirationDays))
                                newItem.Expiration = Convert.ToInt32(item.ExpirationDays);

                            if (ValidateIsNotNull(item.CtrlSerialInbound))
                                newItem.CtrlSerialInbound = item.CtrlSerialInbound.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.CtrlSerialInternal))
                                newItem.CtrlSerialInternal = item.CtrlSerialInternal.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.CtrlSerialOutbound))
                                newItem.CtrlSerialOutbound = item.CtrlSerialOutbound.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.LotControlInbound))
                                newItem.LotControlInbound = item.LotControlInbound.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.LotControlInternal))
                                newItem.LotControlInternal = item.LotControlInternal.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.LotControlOutbound))
                                newItem.LotControlOutbound = item.LotControlOutbound.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.Weight))
                                newItem.Weight = Convert.ToDecimal(item.Weight);

                            if (ValidateIsNotNull(item.Volume))
                                newItem.Volume = Convert.ToDecimal(item.Volume);

                            if (ValidateIsNotNull(item.Length))
                                newItem.Length = Convert.ToDecimal(item.Length);

                            if (ValidateIsNotNull(item.Width))
                                newItem.Width = Convert.ToDecimal(item.Width);

                            if (ValidateIsNotNull(item.Height))
                                newItem.Height = Convert.ToDecimal(item.Height);

                            if (ValidateIsNotNull(item.NestedVolume))
                                newItem.NestedVolume = Convert.ToDecimal(item.NestedVolume);

                            if (ValidateIsNotNull(item.InspectionRequerid))
                                newItem.InspectionRequerid = item.InspectionRequerid.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.InspectionCode))
                                newItem.InspectionCode = item.InspectionCode.ToString().Trim();

                            // newItem.IdPutawayZone = item.IdPutawayZone;
                            if (ValidateIsNotNull(item.ControlExpDate))
                                newItem.CtrlExpiration = item.ControlExpDate.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.ControlFabDate))
                                newItem.CtrlFabrication = item.ControlFabDate.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.Acumulable))
                                newItem.Acumulable = item.Acumulable.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.ReOrderPoint))
                                newItem.ReOrderPoint = Convert.ToDecimal(item.ReOrderPoint);

                            if (ValidateIsNotNull(item.ReOrderQty))
                                newItem.ReOrderQty = Convert.ToDecimal(item.ReOrderQty);

                            if (ValidateIsNotNull(item.PalletQty))
                                newItem.PalletQty = Convert.ToInt32(item.PalletQty);

                            if (ValidateIsNotNull(item.CutMinimum))
                                newItem.CutMinimum = Convert.ToInt32(item.CutMinimum);

                            if (ValidateIsNotNull(item.Originator))
                                newItem.Originator = item.Originator.ToString();

                            if (ValidateIsNotNull(item.VASProfile))
                                newItem.VasProfile = item.VASProfile.ToString();

                            if (ValidateIsNotNull(item.Hazard))
                                newItem.Hazard = item.Hazard.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.Price))
                                newItem.Price = Convert.ToDecimal(item.Price);

                            if (ValidateIsNotNull(item.InventoryType))
                                newItem.InventoryType = item.InventoryType.ToString().Trim();

                            if (ValidateIsNotNull(item.StackingSequence))
                                newItem.StackingSequence = Convert.ToInt32(item.StackingSequence);

                            if (ValidateIsNotNull(item.CommentControl))
                                newItem.CommentControl = item.CommentControl.ToString().Equals("1") ? true : false;

                            if (ValidateIsNotNull(item.CompatibilityCode))
                                newItem.CompatibilyCode = item.CompatibilityCode.ToString().Trim();

                            if (ValidateIsNotNull(item.MSDSUrl))
                                newItem.MsdsUrl = item.MSDSUrl.ToString().Trim();

                            if (ValidateIsNotNull(item.PictureUrl))
                                newItem.PictureUrl = item.PictureUrl.ToString().Trim();

                            if (ValidateIsNotNull(item.GrpClass1))
                                newItem.GrpClass1 = item.GrpClass1.ToString().Trim();

                            if (ValidateIsNotNull(item.GrpClass2))
                                newItem.GrpClass2 = item.GrpClass2.ToString().Trim();

                            if (ValidateIsNotNull(item.GrpClass3))
                                newItem.GrpClass3 = item.GrpClass3.ToString().Trim();

                            if (ValidateIsNotNull(item.GrpClass4))
                                newItem.GrpClass4 = item.GrpClass4.ToString().Trim();

                            if (ValidateIsNotNull(item.GrpClass5))
                                newItem.GrpClass5 = item.GrpClass5.ToString().Trim();

                            if (ValidateIsNotNull(item.GrpClass6))
                                newItem.GrpClass6 = item.GrpClass6.ToString().Trim();

                            if (ValidateIsNotNull(item.GrpClass7))
                                newItem.GrpClass7 = item.GrpClass7.ToString().Trim();

                            if (ValidateIsNotNull(item.GrpClass8))
                                newItem.GrpClass8 = item.GrpClass8.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField1))
                                newItem.SpecialField1 = item.SpecialField1.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField2))
                                newItem.SpecialField2 = item.SpecialField2.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField3))
                                newItem.SpecialField3 = item.SpecialField3.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField4))
                                newItem.SpecialField4 = item.SpecialField4.ToString().Trim();

                            itemViewDTO.Entities.Add(newItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }

                    if (errorUp != "")
                    {
                        ShowAlertLocal(this.lblTitle.Text, errorUp);
                        //divFondoPopupProgress.Visible = false;
                        divLoad.Visible = true;
                        modalPopUpLoad.Show();
                    }
                    else
                    {
                        if (itemViewDTO.Entities.Count > 0)
                        {
                            itemViewDTO = iWarehousingMGR.MaintainItemMassive(itemViewDTO, context);

                            if (itemViewDTO.hasError())
                            {
                                ShowAlertLocal(this.lblTitle.Text, itemViewDTO.Errors.Message);
                            }
                            else
                            {
                                ucStatus.ShowMessage(itemViewDTO.MessageStatus.Message);
                                ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                //divFondoPopupProgress.Visible = false;
                                divLoad.Visible = false;
                                modalPopUpLoad.Hide();
                            }
                        }
                        else
                        {
                            ShowAlertLocal(this.lblTitle.Text, this.lblNotItemsFile.Text);
                            //divFondoPopupProgress.Visible = false;
                            divLoad.Visible = true;
                            modalPopUpLoad.Show();
                        }
                    }

                }
                else
                {
                    ShowAlertLocal(this.lblTitle.Text, this.lblNotFileLoad.Text);
                    //divFondoPopupProgress.Visible = false;
                    divLoad.Visible = true;
                    modalPopUpLoad.Show();
                }

            }
            catch (InvalidDataException ex) 
            {
                theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                //divFondoPopupProgress.Visible = false;
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (InvalidOperationException ex)
            {
                theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                //divFondoPopupProgress.Visible = false;
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(itemViewDTO.Errors);
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


        protected void grdWorkZones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdCustomRules_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdVas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        /// <summary>
        /// Este metodo se ha sobrescrito para poder tener los nombres de los tooltip traducidos
        /// ya que al tenerlo en labels, esto si se puede traducir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    base.grdMgr_RowDataBound(sender, e);

        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        //Carga los textos del tool tip

        //        ImageButton btnEdit = e.Row.FindControl("btnEdit") as ImageButton;
        //        ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

        //        if (btnEdit != null)
        //        {
        //            btnEdit.ToolTip = this.lblToolTipEdit.Text;
        //        }
        //        if (btnDelete != null)
        //        {
        //            btnDelete.ToolTip = this.lblToolTipDelete.Text;
        //        }
        //    }
        //}

        #endregion

        #region "Métodos"

        public void Initialize()
        {
            context.SessionInfo.IdPage = "ItemMgr";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeLayout();
            InitializeTypeUnitOfMeasure_OfMass();
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(itemViewDTO.Errors);
                itemViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            itemViewDTO = iWarehousingMGR.FindAllItem(context);

            if (!itemViewDTO.hasError() && itemViewDTO.Entities != null)
            {
                //foreach (Item item in itemViewDTO.Entities)
                //{
                //    theLog.debugMessage("ItemMgr ", " itemViewDTO.Entities " + item.ShortName);
                //    theLog.debugMessage("ItemMgr ", " itemViewDTO.Entities " + item.LongName);
                //    theLog.debugMessage("ItemMgr ", " itemViewDTO.Entities " + item.AltCode);
                //    theLog.debugMessage("ItemMgr ", " itemViewDTO.Entities " + item.CompatibilyCode);
                //    theLog.debugMessage("ItemMgr ", " itemViewDTO.Entities " + item.LongName);

                //}

                if (!isNew)
                {
                    //carga los codigos Grupos 1, 2, 3 y 4
                    this.idGrpItem1 = itemViewDTO.Entities[0].GrpItem1.Id;
                    this.idGrpItem2 = itemViewDTO.Entities[0].GrpItem2.Id;
                    this.idGrpItem3 = itemViewDTO.Entities[0].GrpItem3.Id;

                }
                else
                {
                    this.idGrpItem1 = -1;
                    this.idGrpItem2 = -1;
                    this.idGrpItem3 = -1;
                    this.idGrpItem4 = -1;
                }

                Session.Add(WMSTekSessions.ItemMgr.ItemList, itemViewDTO);
                Session.Remove(WMSTekSessions.Shared.ItemList);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud) ucStatus.ShowMessage(itemViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        private void PopulateLists(int idOwn)
        {
            if(this.ddlOwner.SelectedValue != "-1")
                base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);

            base.LoadUserOwners(this.ddlOwnerLoad, this.Master.EmptyRowText, "-1", true, string.Empty, false);
            base.ConfigureDDLGrpItem1New(this.ddlGrpItem1, true, idGrpItem1, this.Master.EmptyRowText, false, idOwn);
            base.ConfigureDDLGrpItem2New(this.ddlGrpItem2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, false, idOwn);
            base.ConfigureDDLGrpItem3New(this.ddlGrpItem3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.Master.EmptyRowText, false, idOwn);
            base.ConfigureDDLGrpItem4New(this.ddlGrpItem4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.Master.EmptyRowText, false, idOwn);
            //base.LoadWorkZone(this.ddlPutawayZone, isNew, this.Master.EmptyRowText);
            base.LoadCustomRules(this.ddlRules, true, this.Master.EmptyRowText);
            base.LoadReasonFilterByTypeInOut(this.ddlInspectionCode, TypeInOut.QualityControl, true, this.Master.EmptyRowText);
        }

        private void LoadUomTypeByItem(int idItem)
        {
            try
            {
                ddlUomType.Items.Clear();
                var newContext = NewContext();
                var itemUomViewDTO = iWarehousingMGR.GetItemUomByIdItem(newContext, idItem);

                if (!itemUomViewDTO.hasError() && itemUomViewDTO.Entities.Count > 0)
                {
                    var itemUomAvailable = from itemUom in itemUomViewDTO.Entities
                                           select new  { itemUom.Name, itemUom.UomType.Id };

                    ddlUomType.DataSource = itemUomAvailable.ToList();
                    ddlUomType.DataTextField = "Name";
                    ddlUomType.DataValueField = "Id";
                    ddlUomType.DataBind();
                }

                ddlUomType.Items.Insert(0, new ListItem("Seleccione", "-1"));
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        private void LoadAllUomType()
        {
            try
            {
                LoadUomType(ddlUomType, Master.ucMainFilter.idOwn, true, Master.EmptyRowText);
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!itemViewDTO.hasConfigurationError() && itemViewDTO.Configuration != null && itemViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, itemViewDTO.Configuration);

            grdMgr.DataSource = itemViewDTO.Entities;
            grdMgr.DataBind();

            //base.ConfigureGridSort(grdMgr);
            ucStatus.ShowRecordInfo(itemViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
            this.Master.ucMainFilter.SaveOnIndexChanged = true;
            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabItemGroupVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;

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
            Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnAddVisible = true;
            Master.ucTaskBar.btnAddToolTip = this.lblAddLoadToolTip.Text;
            Master.ucTaskBar.btnExcelVisible = true;
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
            itemViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(itemViewDTO.MessageStatus.Message);
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
            if (itemViewDTO.Configuration == null)
            {
                configurationViewDTO = iConfigurationMGR.GetLayoutConfiguration("Item_FindAll", context);
                if (!configurationViewDTO.hasConfigurationError()) itemViewDTO.Configuration = configurationViewDTO.Configuration;
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


            this.lblTypeUnitMeasure.Text = type;
            this.lblTypeUnitMeasure2.Text = type;
            this.lblTypeUnitMeasure3.Text = type;

            this.lblTypeUnitOfMass.Text = typeMass;
            this.lblTypeUnitOfMass2.Text = typeVol;
            
        }

        protected void ReloadData()
        {
            //Como no es Actualizacion, setea la variable para que muestre "Listo"
            crud = false;
            //Actualiza la grilla
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        protected void SaveChanges()
        {
            Item item = new Item();
            item.Owner = new Owner();
            item.GrpItem1 = new GrpItem1();
            item.GrpItem2 = new GrpItem2();
            item.GrpItem3 = new GrpItem3();
            item.GrpItem4 = new GrpItem4();
            item.PutawayZone = new WorkZone();
            bool EsValido = false;
            
                //Agrega los datos del Item
                item.Id = Convert.ToInt32(hidEditId.Value);
                item.Code = this.txtCode.Text.Trim();
                item.Owner.Id = Convert.ToInt32(this.ddlOwner.SelectedValue);
                item.AltCode = this.txtAltCode.Text.Trim();
                item.Description = this.txtDescription.Text.Trim();
                item.GrpItem1.Id = Convert.ToInt32(this.ddlGrpItem1.SelectedValue);
                item.GrpItem2.Id = Convert.ToInt32(this.ddlGrpItem2.SelectedValue);
                item.GrpItem3.Id = Convert.ToInt32(this.ddlGrpItem3.SelectedValue);
                item.GrpItem4.Id = Convert.ToInt32(this.ddlGrpItem4.SelectedValue);
                item.LongName = this.txtLongName.Text.Trim();
                item.ShortName = this.txtShortName.Text.Trim();
                item.Status = this.chkStatus.Checked;
                if (this.txtComment.Text.Length <= 100)
                {
                    item.Comment = this.txtComment.Text.Trim();
                    EsValido = true;
                }

                else
                    EsValido = false;

                if (EsValido)
                {
                    if (this.txtShelfLife.Text != "")
                        item.ShelfLife = Convert.ToInt32(this.txtShelfLife.Text.Trim());

                    if (this.txtExpiration.Text != "")
                        item.Expiration = Convert.ToInt32(this.txtExpiration.Text.Trim());

                    item.CtrlSerialInbound = this.chkCtrlSerialInbound.Checked;
                    item.CtrlSerialInternal = this.chkCtrlSerialInternal.Checked;
                    item.CtrlSerialOutbound = this.chkCtrlSerialOutbound.Checked;
                    item.LotControlInbound = this.chkLotControlInbound.Checked;
                    item.LotControlInternal = this.chkLotControlInternal.Checked;
                    item.LotControlOutbound = this.chkLotControlOutbound.Checked;

                    if (this.txtWeight.Text != "")
                        item.Weight = Convert.ToDecimal(this.txtWeight.Text.Trim());

                    if (this.txtVolume.Text != "")
                        item.Volume = Convert.ToDecimal(this.txtVolume.Text.Trim());

                    if (this.txtLength.Text != "")
                        item.Length = Convert.ToDecimal(this.txtLength.Text.Trim());

                    if (this.txtWidth.Text != "")
                        item.Width = Convert.ToDecimal(this.txtWidth.Text.Trim());

                    if (this.txtHeight.Text != "")
                        item.Height = Convert.ToDecimal(this.txtHeight.Text.Trim());

                    if (this.txtNestedVolume.Text != "")
                        item.NestedVolume = Convert.ToDecimal(this.txtNestedVolume.Text.Trim());

                    item.InspectionRequerid = this.chkInspectionRequerid.Checked;

                    if (this.ddlInspectionCode.SelectedValue != "-1")
                        item.InspectionCode = this.ddlInspectionCode.SelectedValue;//this.txtInspectionCode.Text.Trim();
                    
                            //item.PutawayZone.Id = Convert.ToInt32(this.ddlPutawayZone.SelectedValue);
                    item.CtrlExpiration = this.chkCtrlExpiration.Checked;
                    item.CtrlFabrication = this.chkCtrlFabrication.Checked;
                    item.Acumulable = this.chkAcumulable.Checked;

                    if (this.txtReOrderPoint.Text != "")
                        item.ReOrderPoint = Convert.ToDecimal(this.txtReOrderPoint.Text.Trim());

                    if (this.txtReOrderQty.Text != "")
                        item.ReOrderQty = Convert.ToDecimal(this.txtReOrderQty.Text.Trim());

                    if (this.txtPalletQty.Text != "")
                        item.PalletQty = Convert.ToInt32(this.txtPalletQty.Text.Trim());

                    if (this.txtCutMinimum.Text != "")
                        item.CutMinimum = Convert.ToInt32(this.txtCutMinimum.Text.Trim());

                    item.Originator = this.txtOriginator.Text.Trim();
                    item.VasProfile = this.txtVASProfile.Text.Trim();
                    item.Hazard = this.chkHazard.Checked;

                    if (this.txtPrice.Text != "")
                        item.Price = Convert.ToDecimal(this.txtPrice.Text.Trim());

                    item.InventoryType = this.txtInventoryType.Text.Trim();

                    item.StackingSequence = Convert.ToInt32(ddlUomType.SelectedValue);

                    item.CommentControl = this.chkCommentControl.Checked;
                    item.CompatibilyCode = this.txtCompatibilyCode.Text.Trim();
                    item.MsdsUrl = this.txtMSDSUrl.Text.Trim();
                    item.PictureUrl = this.txtPictureUrl.Text.Trim();

                    // Zonas
                    int index = Convert.ToInt32(hidEditIndex.Value);

                    //Zonas
                    if (index != -1 && itemViewDTO.Entities[index] != null && itemViewDTO.Entities[index].WorkZones != null
                        && itemViewDTO.Entities[index].WorkZones.Count > 0)
                    {
                        item.WorkZones = itemViewDTO.Entities[index].WorkZones;
                    }
                    
                    //Reglas
                    if (index != -1 && itemViewDTO.Entities[index] != null && itemViewDTO.Entities[index].CustomRules != null
                        && itemViewDTO.Entities[index].CustomRules.Count > 0)
                    {
                        item.CustomRules = itemViewDTO.Entities[index].CustomRules;
                    }

                    //Vas
                    if (index != -1 && itemViewDTO.Entities[index] != null && itemViewDTO.Entities[index].Vas != null
                        && itemViewDTO.Entities[index].Vas.Count > 0)
                    {
                        item.Vas = itemViewDTO.Entities[index].Vas;
                    }

                    if (hidEditId.Value == "0")
                        itemViewDTO = iWarehousingMGR.MaintainItem(CRUD.Create, item, context);
                    else
                        itemViewDTO = iWarehousingMGR.MaintainItem(CRUD.Update, item, context, context.SessionInfo.Warehouse.Id);

                    divEditNew.Visible = false;
                    modalPopUp.Hide();

                    if (itemViewDTO.hasError())
                    {
                        UpdateSession(true);
                        divEditNew.Visible = true;
                        modalPopUp.Show();
                    }
                    else
                    {
                        ViewState.Remove("GrpItem1");
                        ViewState.Remove("GrpItem2");
                        ViewState.Remove("GrpItem3");
                        ViewState.Remove("GrpItem4");

                        //Muestra mensaje en la barra de status
                        crud = true;
                        ucStatus.ShowMessage(itemViewDTO.MessageStatus.Message);

                        UpdateSession(false);
                    }
                }
                else
                {
                    itemViewDTO.Errors = baseControl.handleException(new Exception("El campo de comentarios no puede contener mas de 100 caracteres"), context);
                   // throw new Exception("El campo de comentarios no puede contener mas de 100 caracteres");
                }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            //Llena las listas de los grupos
            PopulateLists(-1);

            tabItem.ActiveTabIndex = 0;
            // Editar bodega
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditIndex.Value = index.ToString();
                hidEditId.Value = itemViewDTO.Entities[index].Id.ToString();

                //asigna el id a las propiedades de grp's
                idGrpItem1 = itemViewDTO.Entities[index].GrpItem1.Id;
                idGrpItem2 = itemViewDTO.Entities[index].GrpItem2.Id;
                idGrpItem3 = itemViewDTO.Entities[index].GrpItem3.Id;
                idGrpItem4 = itemViewDTO.Entities[index].GrpItem4.Id;

                //Llena las listas de los grupos
                PopulateLists(itemViewDTO.Entities[index].Owner.Id);
                LoadUomTypeByItem(itemViewDTO.Entities[index].Id);

                //asigna valores a los controles
                this.txtCode.Text = itemViewDTO.Entities[index].Code;
                this.ddlOwner.SelectedValue = itemViewDTO.Entities[index].Owner.Id.ToString();
                this.txtAltCode.Text = itemViewDTO.Entities[index].AltCode;
                this.txtDescription.Text = itemViewDTO.Entities[index].Description;
                this.ddlGrpItem1.SelectedValue = itemViewDTO.Entities[index].GrpItem1.Id.ToString();
                this.ddlGrpItem2.SelectedValue = itemViewDTO.Entities[index].GrpItem2.Id.ToString();
                this.ddlGrpItem3.SelectedValue = itemViewDTO.Entities[index].GrpItem3.Id.ToString();
                this.ddlGrpItem4.SelectedValue = itemViewDTO.Entities[index].GrpItem4.Id.ToString();
                this.txtLongName.Text = itemViewDTO.Entities[index].LongName;
                this.txtShortName.Text = itemViewDTO.Entities[index].ShortName;
                this.chkStatus.Checked = itemViewDTO.Entities[index].Status;
                this.txtComment.Text = itemViewDTO.Entities[index].Comment;
                this.txtShelfLife.Text = itemViewDTO.Entities[index].ShelfLife.ToString();
                this.txtExpiration.Text = itemViewDTO.Entities[index].Expiration.ToString();
                this.chkCtrlSerialInbound.Checked = itemViewDTO.Entities[index].CtrlSerialInbound;
                this.chkCtrlSerialInternal.Checked = itemViewDTO.Entities[index].CtrlSerialInternal;
                this.chkCtrlSerialOutbound.Checked = itemViewDTO.Entities[index].CtrlSerialOutbound;
                this.chkLotControlInbound.Checked = itemViewDTO.Entities[index].LotControlInbound;
                this.chkLotControlInternal.Checked = itemViewDTO.Entities[index].LotControlInternal;
                this.chkLotControlOutbound.Checked = itemViewDTO.Entities[index].LotControlOutbound;
                this.txtWeight.Text = itemViewDTO.Entities[index].Weight.ToString();
                this.txtVolume.Text = itemViewDTO.Entities[index].Volume.ToString();
                this.txtLength.Text = itemViewDTO.Entities[index].Length.ToString();
                this.txtWidth.Text = itemViewDTO.Entities[index].Width.ToString();
                this.txtHeight.Text = itemViewDTO.Entities[index].Height.ToString();


                if (itemViewDTO.Entities[index].NestedVolume == -1)
                {
                    this.txtNestedVolume.Text = string.Empty;
                }
                else
                {
                    this.txtNestedVolume.Text = itemViewDTO.Entities[index].NestedVolume.ToString();
                }

                this.chkInspectionRequerid.Checked = itemViewDTO.Entities[index].InspectionRequerid;
                //this.txtInspectionCode.Text = itemViewDTO.Entities[index].InspectionCode;

                if (!string.IsNullOrEmpty(itemViewDTO.Entities[index].InspectionCode))
                    this.ddlInspectionCode.SelectedValue = itemViewDTO.Entities[index].InspectionCode;

                //this.ddlPutawayZone.SelectedValue = itemViewDTO.Entities[index].PutawayZone.Id.ToString();
                this.chkCtrlExpiration.Checked = itemViewDTO.Entities[index].CtrlExpiration;
                this.chkCtrlFabrication.Checked = itemViewDTO.Entities[index].CtrlFabrication;
                this.chkAcumulable.Checked = itemViewDTO.Entities[index].Acumulable;
                this.txtReOrderPoint.Text = itemViewDTO.Entities[index].ReOrderPoint.ToString();
                this.txtReOrderQty.Text = itemViewDTO.Entities[index].ReOrderQty.ToString();

                if (itemViewDTO.Entities[index].PalletQty == -1) this.txtPalletQty.Text = string.Empty;
                else { this.txtPalletQty.Text = itemViewDTO.Entities[index].PalletQty.ToString(); }

                if (itemViewDTO.Entities[index].CutMinimum == -1) this.txtCutMinimum.Text = string.Empty;
                else { this.txtCutMinimum.Text = itemViewDTO.Entities[index].CutMinimum.ToString(); }

                this.txtOriginator.Text = itemViewDTO.Entities[index].Originator;
                this.txtVASProfile.Text = itemViewDTO.Entities[index].VasProfile;
                this.chkHazard.Checked = itemViewDTO.Entities[index].Hazard;

                if (itemViewDTO.Entities[index].Price == -1) this.txtPrice.Text = string.Empty;
                else { this.txtPrice.Text = itemViewDTO.Entities[index].Price.ToString(); }

                this.txtInventoryType.Text = itemViewDTO.Entities[index].InventoryType;

                var idUomType = itemViewDTO.Entities[index].StackingSequence;

                if (idUomType > 0)
                    ddlUomType.SelectedValue = itemViewDTO.Entities[index].StackingSequence.ToString();
                else
                {
                    ddlUomType.Items[0].Selected = true;
                    ddlUomType.SelectedValue = "-1";
                }

                this.chkCommentControl.Checked = itemViewDTO.Entities[index].CommentControl;
                this.txtCompatibilyCode.Text = itemViewDTO.Entities[index].CompatibilyCode;
                this.txtMSDSUrl.Text = itemViewDTO.Entities[index].MsdsUrl;
                this.txtPictureUrl.Text = itemViewDTO.Entities[index].PictureUrl;
                lblNew.Visible = false;
                lblEdit.Visible = true;

                Session.Add("ITEM_SELECTED", itemViewDTO.Entities[index]);
             }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                //Limpia los controles e inicializa variables
                isNew = true;

                // Selecciona owner seleccionado en el Filtro
                this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

                //asigna el id a las propiedades de grp's
                idGrpItem1 = -1;
                idGrpItem2 = -1;
                idGrpItem3 = -1;
                idGrpItem4 = -1;

                //Llena las listas de los grupos
                PopulateLists(int.Parse(ddlOwner.SelectedValue));
                LoadAllUomType();
                //base.LoadWorkZone(this.ddlPutawayZone, isNew, this.Master.EmptyRowText);

                hidEditId.Value = "0";
                hidEditIndex.Value = "-1";

                this.txtCode.Text = string.Empty;
                this.txtAltCode.Text = string.Empty;
                this.txtDescription.Text = string.Empty;
                this.ddlGrpItem1.SelectedValue = "-1";
                this.ddlGrpItem2.SelectedValue = "-1";
                this.ddlGrpItem3.SelectedValue = "-1";
                this.ddlGrpItem4.SelectedValue = "-1";
                this.txtLongName.Text = string.Empty;
                this.txtShortName.Text = string.Empty;
                this.chkStatus.Checked = true;
                this.txtComment.Text = string.Empty;
                this.txtShelfLife.Text = string.Empty;
                this.txtExpiration.Text = string.Empty;
                this.chkCtrlSerialInbound.Checked = false;
                this.chkCtrlSerialInternal.Checked = false;
                this.chkCtrlSerialOutbound.Checked = false;
                this.chkLotControlInbound.Checked = false;
                this.chkLotControlInternal.Checked = false;
                this.chkLotControlOutbound.Checked = false;
                this.txtWeight.Text = string.Empty;
                this.txtVolume.Text = string.Empty;
                this.txtLength.Text = string.Empty;
                this.txtWidth.Text = string.Empty;
                this.txtHeight.Text = string.Empty;
                this.txtNestedVolume.Text = string.Empty;
                this.chkInspectionRequerid.Checked = false;
                
                this.ddlInspectionCode.SelectedValue = "-1";

                //this.ddlPutawayZone.SelectedValue = "-1";
                this.chkCtrlExpiration.Checked = false;
                this.chkCtrlFabrication.Checked = false;
                this.chkAcumulable.Checked = false;
                this.txtReOrderPoint.Text = string.Empty;
                this.txtReOrderQty.Text = string.Empty;
                this.txtPalletQty.Text = string.Empty;
                this.txtCutMinimum.Text = string.Empty;
                this.txtOriginator.Text = string.Empty;
                this.txtVASProfile.Text = string.Empty;
                this.chkHazard.Checked = false;
                this.txtPrice.Text = string.Empty;
                this.txtInventoryType.Text = string.Empty;
                this.ddlUomType.SelectedValue = "-1";
                this.chkCommentControl.Checked = false;
                this.txtCompatibilyCode.Text = string.Empty;
                this.txtMSDSUrl.Text = string.Empty;
                this.txtPictureUrl.Text = string.Empty;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }
            //Carga la lista de zonas
            LoadWorkZones(index);

            //Carga la lista de Vas
            LoadItemVas(index);

            //Carga la lista de Reglas
            LoadCustomRules(index);

            if (itemViewDTO.Configuration != null && itemViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(itemViewDTO.Configuration, true);
                else
                    base.ConfigureModal(itemViewDTO.Configuration, false);
            }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                this.ddlInspectionCode.Enabled = false;
                this.rfvInspectionCode.Enabled = false;
            }
            
            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            itemViewDTO = iWarehousingMGR.MaintainItem(CRUD.Delete, itemViewDTO.Entities[index], context);

            if (itemViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra mensaje de status
                crud = true;
                ucStatus.ShowMessage(itemViewDTO.MessageStatus.Message);

                //Actualiza grid
                UpdateSession(false);
            }
        }

        private void LoadWorkZones(int index)
        {
            // TODO: manejo de excepciones
            int id;
            int typeZone = (int)TypeWorkZone.Almacenamiento;
            GenericViewDTO<WorkZone> workzoneViewDTO = new GenericViewDTO<WorkZone>();

            // Limpia valores actuales de la grilla
            grdWorkZones.DataSource = null;
            grdWorkZones.DataBind();

            // Zonas asignadas al Item
            if (index != -1)
            {
                id = itemViewDTO.Entities[index].Id;

                workzoneViewDTO = iLayoutMGR.GetWorkZoneByItem(id, context);
                grdWorkZones.DataSource = workzoneViewDTO.Entities;
                grdWorkZones.DataBind();

                itemViewDTO.Entities[index].WorkZones = workzoneViewDTO.Entities;
            }
            else
            {
                id = -1;
            }

            // Zonas NO asignadas al Item
            //workzoneViewDTO = iLayoutMGR.GetWorkZoneByNotInItem(id, context);
            workzoneViewDTO = iLayoutMGR.GetWorkZoneByTypeZoneNotInItem(id, typeZone, context);
            
            ddlWorkZones.Items.Clear();
            ddlWorkZones.DataSource = workzoneViewDTO.Entities;

            foreach (WorkZone workZone in workzoneViewDTO.Entities)
                ddlWorkZones.Items.Add(new ListItem((workZone.Name), workZone.Id.ToString()));

            ddlWorkZones.Items.Insert(0, this.Master.EmptyRowText);
        }

        private void LoadItemVas(int index)
        {
            // TODO: manejo de excepciones
            int id;
            GenericViewDTO<RecipeVas> recipeVasViewDTO = new GenericViewDTO<RecipeVas>();
            GenericViewDTO<ItemVas> itemVasDTO = new GenericViewDTO<ItemVas>();
            String theWhere = "";

            // Limpia valores actuales de la grilla
            grdVas.DataSource = null;
            grdVas.DataBind();

            // Vas asignados al Item
            if (index != -1)
            {
                id = itemViewDTO.Entities[index].Id;

                theWhere = " AND V.IdOwn = " + itemViewDTO.Entities[index].Owner.Id.ToString();
                theWhere += " AND IV.IdItem = " + id.ToString();
                theWhere += " ORDER BY IV.Secuence";
                itemVasDTO = iWarehousingMGR.GetItemVasByAnyParameter(context, null, theWhere);

                foreach (ItemVas itemVas in itemVasDTO.Entities)
                {
                    RecipeVas recipeVas = new RecipeVas();
                    recipeVas.Id = itemVas.Vas.Id;
                    recipeVas.Owner.Id = itemViewDTO.Entities[index].Owner.Id;
                    recipeVas.Name = itemVas.Vas.Name;
                    recipeVas.Description = itemVas.Vas.Description;
                    recipeVas.Status = itemVas.Vas.Status;
                    recipeVas.Seceuence = itemVas.Secuence;

                    recipeVasViewDTO.Entities.Add(recipeVas);
                }

                grdVas.DataSource = recipeVasViewDTO.Entities;
                grdVas.DataBind();

                itemViewDTO.Entities[index].Vas = recipeVasViewDTO.Entities;

                theWhere = " AND V.IdOwn = " + this.ddlOwner.SelectedValue.ToString();
                theWhere += " AND V.IdVas NOT IN (SELECT IdVas FROM ItemVas WHERE IdItem = " + id.ToString() + ")";
                recipeVasViewDTO = iWarehousingMGR.GetVasByAnyParameter(context, null, theWhere);
            }
            else
            {
                id = -1;

                // Vas NO asignadas al Item
                theWhere = " AND V.IdOwn = "+ this.ddlOwner.SelectedValue.ToString() ;
                theWhere += " AND V.IdVas NOT IN (SELECT IdVas FROM ItemVas WHERE IdItem = " + id.ToString() + ")";
                recipeVasViewDTO = iWarehousingMGR.GetVasByAnyParameter(context, null, theWhere);
            }

            ddlVas.Items.Clear();
            ddlVas.DataSource = recipeVasViewDTO.Entities;

            foreach (RecipeVas recipeVas in recipeVasViewDTO.Entities)
                ddlVas.Items.Add(new ListItem((recipeVas.Name), recipeVas.Id.ToString()));

            ddlVas.Items.Insert(0, this.Master.EmptyRowText);
        }

        private void LoadCustomRules(int index)
        {
            // TODO: manejo de excepciones
            int id;

            GenericViewDTO<CustomRule> customRuleViewDTO = new GenericViewDTO<CustomRule>();

            // Limpia valores actuales de la grilla
            grdCustomRules.DataSource = null;
            grdCustomRules.DataBind();

            // Reglas asignadas al Item
            if (index != -1)
            {
                id = itemViewDTO.Entities[index].Id;

                customRuleViewDTO = iRulesMGR.GetCustomRuleByItem(id, context.SessionInfo.Warehouse.Id, context);
                grdCustomRules.DataSource = customRuleViewDTO.Entities;
                grdCustomRules.DataBind();

                itemViewDTO.Entities[index].CustomRules = customRuleViewDTO.Entities;
            }
            else
            {
                id = -1;
            }

            // Reglas NO asignadas al Item
            customRuleViewDTO = iRulesMGR.GetCustomRuleByNotInItem(id, context.SessionInfo.Warehouse.Id, context);
            ddlRules.Items.Clear();
            ddlRules.DataSource = customRuleViewDTO.Entities;

            foreach (CustomRule customRule in customRuleViewDTO.Entities)
                ddlRules.Items.Add(new ListItem((customRule.Name), customRule.Id.ToString()));

            base.AlphabeticalOrderDropDownList(ddlRules);

            ddlRules.Items.Insert(0, this.Master.EmptyRowText);
        }

        /// <summary>
        /// Agrega la Zona seleccionada a la grilla de Zonas Asignadas al Item actual (grid view)
        /// </summary>
        /// <param name="index"></param>
        protected void AddWorkZone(int index)
        {
            //TODO:Agregar mensaje de status para esta accion
            if (ddlWorkZones.SelectedIndex > 0)
            {
                WorkZone workzone = new WorkZone(Convert.ToInt32(ddlWorkZones.SelectedValue));
                workzone.Name = ddlWorkZones.SelectedItem.Text;

                // Item nuevo
                if (index == -1)
                {
                    itemViewDTO.Entities.Add(new Item());
                    index = itemViewDTO.Entities.Count - 1;

                    hidEditIndex.Value = index.ToString();

                    Session[WMSTekSessions.ItemMgr.ItemList] = itemViewDTO;
                }

                // Si es la primer Zona a agregar, crea la lista
                if (itemViewDTO.Entities[index].WorkZones == null) itemViewDTO.Entities[index].WorkZones = new List<WorkZone>();
                itemViewDTO.Entities[index].WorkZones.Add(workzone);
                grdWorkZones.DataSource = itemViewDTO.Entities[index].WorkZones;
                
                grdWorkZones.DataBind();

                // Quita la Zona seleccionada de la lista de Zonas a Asignar (drop-down list)
                ddlWorkZones.Items.RemoveAt(ddlWorkZones.SelectedIndex);
            }
        }

        /// <summary>
        /// Quita la Zona seleccionada de la grilla de Zonas Asignadas al Item actual (grid view)
        /// </summary>
        /// <param name="index"></param>
        protected void RemoveWorkZone(int index, int zoneIndex)
        {
            //TODO:Agregar mensaje de status para esta accion

            // Agrega la Zona seleccionada a la lista de Zonas a Asignar (drop-down list)
            ddlWorkZones.Items.Add(new ListItem(itemViewDTO.Entities[index].WorkZones[zoneIndex].Name, itemViewDTO.Entities[index].WorkZones[zoneIndex].Id.ToString()));

            // Quita la Zona seleccionada de la grilla de Zonas Asignadas al Item actual (grid view)
            itemViewDTO.Entities[index].WorkZones.RemoveAt(zoneIndex);

            grdWorkZones.DataSource = itemViewDTO.Entities[index].WorkZones;
            grdWorkZones.DataBind();
        }

        protected void RemoveVas(int index, int itemIndex)
        {
            //TODO:Agregar mensaje de status para esta accion

            // Agrega Vas seleccionada a la lista de Vas a Asignar (drop-down list)
            ddlVas.Items.Add(new ListItem(itemViewDTO.Entities[index].Vas[itemIndex].Name, itemViewDTO.Entities[index].Vas[itemIndex].Id.ToString()));

            // Quita la Zona seleccionada de la grilla de Zonas Asignadas al Item actual (grid view)
            itemViewDTO.Entities[index].Vas.RemoveAt(itemIndex);

            Int32 firstSecuence = 1;
            List<RecipeVas> recipeVasList = new List<RecipeVas>();
            foreach (RecipeVas recipeVas in itemViewDTO.Entities[index].Vas.OrderBy(order => order.Seceuence))
            {
                recipeVas.Seceuence = firstSecuence;
                recipeVasList.Add(recipeVas);
                firstSecuence++;
            }
            itemViewDTO.Entities[index].Vas = new List<RecipeVas>(recipeVasList);
            grdVas.DataSource = itemViewDTO.Entities[index].Vas;
            grdVas.DataBind();
        }

        protected void AddVas(int index)
        {
            //TODO:Agregar mensaje de status para esta accion
            if (ddlVas.SelectedIndex > 0)
            {
                RecipeVas vas = new RecipeVas();                
                vas.Id = Convert.ToInt32(ddlVas.SelectedValue);
                vas.Name = ddlVas.SelectedItem.Text;
                vas.DateCreated = DateTime.Now;
                vas.UserCreated = context.SessionInfo.User.UserName;               

                // Item nuevo
                if (index == -1)
                {
                    itemViewDTO.Entities.Add(new Item());
                    index = itemViewDTO.Entities.Count - 1;

                    hidEditIndex.Value = index.ToString();

                    Session[WMSTekSessions.ItemMgr.ItemList] = itemViewDTO;
                }

                // Si es la primer Vas a agregar, crea la lista
                if (itemViewDTO.Entities[index].Vas == null)
                {
                    itemViewDTO.Entities[index].Vas = new List<RecipeVas>();
                }
                //Nueva secuencia
                vas.Seceuence = itemViewDTO.Entities[index].Vas.Count + 1;

                itemViewDTO.Entities[index].Vas.Add(vas);
                grdVas.DataSource = itemViewDTO.Entities[index].Vas;
                grdVas.DataBind();

                // Quita Vas seleccionada de la lista de Vas a Asignar (drop-down list)
                ddlVas.Items.RemoveAt(ddlVas.SelectedIndex);
                
            }
        }

        protected void AddRules(int index)
        {
            //TODO:Agregar mensaje de status para esta accion
            if (this.ddlRules.SelectedIndex > 0)
            {
                CustomRule customRule = new CustomRule( );
                customRule.Id = Convert.ToInt32(ddlRules.SelectedValue);
                customRule.Name = ddlRules.SelectedItem.Text;

                // Item nuevo
                if (index == -1)
                {
                    itemViewDTO.Entities.Add(new Item());
                    index = itemViewDTO.Entities.Count - 1;

                    hidEditIndex.Value = index.ToString();

                    Session[WMSTekSessions.ItemMgr.ItemList] = itemViewDTO;
                }

                // Si es la primer Regls a agregar, crea la lista
                if (itemViewDTO.Entities[index].CustomRules == null){
                    itemViewDTO.Entities[index].CustomRules = new List<CustomRule>();
                }

                itemViewDTO.Entities[index].CustomRules.Add(customRule);
                grdCustomRules.DataSource = itemViewDTO.Entities[index].CustomRules;
                grdCustomRules.DataBind();

                // Quita la Reglas seleccionada de la lista de Reglas a Asignar (drop-down list)
                ddlRules.Items.RemoveAt(ddlRules.SelectedIndex);
            }
        }

        protected void grdCustomRules_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            //    // Deshabilita la opcion de Eliminar si es el Usuario Base del Rol Base
            //    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

            //    if (btnDelete != null && itemViewDTO.Entities[ddlWorkZones.SelectedIndex].IsBaseRole && itemViewDTO.Entities[ddlRole.SelectedIndex].Users[e.Row.RowIndex].IsBaseUser)
            //    {
            //        btnDelete.ImageUrl = "~/WebResources/Images/Buttons/icon_delete_des.gif";
            //        btnDelete.Enabled = false;
            //    }
            }
        }

        protected void grdCustomRules_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                RemoveCustomRules(Convert.ToInt32(hidEditIndex.Value), e.RowIndex);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void RemoveCustomRules(int index, int customRuleIndex)
        {
            // Agrega la Regla seleccionada a la lista de Reglas a Asignar (drop-down list)
            ddlRules.Items.Add(new ListItem(itemViewDTO.Entities[index].CustomRules[customRuleIndex].Name, itemViewDTO.Entities[index].CustomRules[customRuleIndex].Id.ToString()));
            base.AlphabeticalOrderDropDownList(ddlRules);

            // Quita la Regla seleccionada de la grilla de Reglas Asignadas al Item actual (grid view)
            itemViewDTO.Entities[index].CustomRules.RemoveAt(customRuleIndex);

            grdCustomRules.DataSource = itemViewDTO.Entities[index].CustomRules;
            grdCustomRules.DataBind();
        }


        protected void ddlOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(hidEditIndex.Value);
                loadAndRemoveVasSelectedIndexChanged(index);

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }

        protected void loadAndRemoveVasSelectedIndexChanged(int index)
        {
            GenericViewDTO<RecipeVas> recipeVasViewDTO = new GenericViewDTO<RecipeVas>();
            String theWhere = string.Empty;
            
            // Item nuevo
            if (index == -1)
            {
                itemViewDTO.Entities.Add(new Item());
                index = itemViewDTO.Entities.Count - 1;

                hidEditIndex.Value = index.ToString();

                Session[WMSTekSessions.ItemMgr.ItemList] = itemViewDTO;
            }

            //Elimina los Vas asociados si es que el owner es modificado
            if (grdVas.Rows.Count > 0)
            {
                itemViewDTO.Entities[index].Vas.Clear();
                grdVas.DataSource = itemViewDTO.Entities[index].Vas;
                grdVas.DataBind();
            }

            theWhere = " AND V.IdOwn = " + this.ddlOwner.SelectedValue.ToString();
            recipeVasViewDTO = iWarehousingMGR.GetVasByAnyParameter(context, null, theWhere);

            ddlVas.Items.Clear();
            ddlVas.DataSource = recipeVasViewDTO.Entities;

            foreach (RecipeVas recipeVas in recipeVasViewDTO.Entities)
                ddlVas.Items.Add(new ListItem((recipeVas.Name), recipeVas.Id.ToString()));

            ddlVas.Items.Insert(0, this.Master.EmptyRowText);

        }

        
        private object ReturnType(object obj)
        {
            object result = null;

            switch (obj.GetType().Name)
            {
                case "String":
                    result = Convert.ToString(obj);
                    break;

                case "Int16":
                    result = Convert.ToInt16(obj);
                    break;

                case "Int32":
                    result = Convert.ToInt32(obj);
                    break;

                case "Int64":
                    result = Convert.ToInt64(obj);
                    break;

                case "Double":
                    result = Convert.ToDouble(obj);
                    break;

                case "Decimal":
                    result = Convert.ToDecimal(obj);
                    break;

                case "DateTime":
                    result = Convert.ToDateTime(obj);
                    break;

                case "Boolean":
                    result = Convert.ToBoolean(obj);
                    break;
            }

            return result;
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
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }
        #endregion


        protected void grdMgr_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                if (sort == SortDirection.Ascending)
                {
                    itemViewDTO.Entities = (from i in itemViewDTO.Entities
                                            orderby GetDynamicSortProperty(i, e.SortExpression.ToString()) ascending
                                            select i).ToList();

                    sort = SortDirection.Descending;
                }
                else
                {
                    itemViewDTO.Entities = (from i in itemViewDTO.Entities
                                            orderby GetDynamicSortProperty(i, e.SortExpression.ToString()) descending
                                            select i).ToList();

                    sort = SortDirection.Ascending;
                }
            }
            catch (Exception ex)
            {
                itemViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }  
    }
}
