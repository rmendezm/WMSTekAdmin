using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class BranchMgr : BasePage
    {
        #region "Declaración de Variables"

        GenericViewDTO<Branch> branchViewDTO = new GenericViewDTO<Branch>();
        private GenericViewDTO<Customer> customerViewDTO = new GenericViewDTO<Customer>();
        
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

        public int IdCountry
        {
            get { return (int)(ViewState["IdCountryBranch"] ?? -1); }
            set { ViewState["IdCountryBranch"] = value; }
        }

        public int IdState
        {
            get { return (int)(ViewState["IdStateBranch"] ?? -1); }
            set { ViewState["IdStateBranch"] = value; }
        }

        public int IdCity
        {
            get { return (int)(ViewState["IdCityBranch"] ?? -1); }
            set { ViewState["IdCityBranch"] = value; }
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

                    // Nota: este mantenedor no carga inicialmente la grilla

                    if (Page.IsPostBack)
                    {
                        hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .5);

                        if (ValidateSession(WMSTekSessions.BranchMgr.CustomerList))
                        {
                            customerViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.BranchMgr.CustomerList];
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
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
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
                        LoadBranchByIdCustomer(currentIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
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
                        LoadBranchByIdCustomer(currentIndex);
                    }
                }
                if (!Page.IsPostBack)
                {
                    lblEmpty.Visible = false;

                    if (context != null)
                        context.MainFilter = this.Master.ucMainFilter.MainFilter;
                }

                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
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
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
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
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
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
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
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
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
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
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
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
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
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
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        protected void grdCustomer_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdCustomer.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdCustomer.ClientID + "')");

                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdCustomer.ClientID + "');");
                    e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdCustomer, "Select$" + e.Row.RowIndex);
                }
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        protected void grdCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //capturo la posicion de la fila 
                    int index = grdCustomer.PageSize * grdCustomer.PageIndex + grdCustomer.SelectedIndex;
                    currentIndex = index; //grdItem.SelectedIndex;
                    divBranch.Visible = true;
                    lblEmpty.Visible = false;
                }
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                divBranch.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                divBranch.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                divBranch.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                divBranch.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                divBranch.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            base.LoadUserOwners(this.ddlOwnerLoad, this.Master.EmptyRowText, "-1", true, string.Empty, false);

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
                    catch(Exception ex)
                    {
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       CustomerCode = r.Field<object>("CustomerCode"),
                                       BranchCode = r.Field<object>("BranchCode"),
                                       BranchName = r.Field<object>("BranchName"),
                                       BranchAddress = r.Field<object>("BranchAddress"),
                                       CountryName = r.Field<object>("CountryName"),
                                       StateName = r.Field<object>("StateName"),
                                       CityName = r.Field<object>("CityName"),
                                       Distance = r.Field<object>("Distance"),
                                       Phone = r.Field<object>("Phone"),
                                       SpecialField1 = r.Field<object>("SpecialField1"),
                                       SpecialField2 = r.Field<object>("SpecialField2"),
                                       SpecialField3 = r.Field<object>("SpecialField3"),
                                       SpecialField4 = r.Field<object>("SpecialField4")
                                   };

                    branchViewDTO = new GenericViewDTO<Branch>();

                    try
                    { 
                        foreach (var item in lstExcel)
                        {
                            Branch newBranch = new Branch();

                            newBranch.Owner = new Owner(idOwn);

                            if (!ValidateIsNotNull(item.BranchCode))
                            {
                                errorUp = "BranchCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (item.BranchCode.ToString().Trim().Length > this.txtBranchCode.MaxLength)
                                {
                                    errorUp = "BranchCode " + item.BranchCode + " - " + this.lblFieldLength.Text.Replace("[FIELD]", "BranchCode").Replace("[LENGTH]", this.txtBranchCode.MaxLength.ToString());
                                    break;
                                }
                                else
                                {
                                    newBranch.Code = item.BranchCode.ToString().Trim();
                                }
                            }

                            if (!ValidateIsNotNull(item.CustomerCode))
                            {
                                errorUp = "BranchCode " + item.BranchCode + " - CustomerCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                Customer customer = new Customer();
                                customer.Code = item.CustomerCode.ToString().Trim();
                                newBranch.Customer = customer;
                            }

                            if (!ValidateIsNotNull(item.BranchName))
                            {
                                errorUp = "BranchCode " + item.BranchCode + " - BranchName " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (item.BranchName.ToString().Trim().Length > this.txtBranchName.MaxLength)
                                {
                                    errorUp = this.lblFieldLength.Text.Replace("[FIELD]", "BranchName").Replace("[LENGTH]", this.txtBranchName.MaxLength.ToString()); 
                                    break;
                                }
                                else
                                {
                                    newBranch.Name = item.BranchName.ToString().Trim();
                                }
                            }

                            if (ValidateIsNotNull(item.BranchAddress))
                            {
                                if(item.BranchAddress.ToString().Trim().Length > this.txtBranchAddress.MaxLength)
                                {
                                    errorUp = "BranchCode " + item.BranchCode + " - " + this.lblFieldLength.Text.Replace("[FIELD]", "BranchAddress").Replace("[LENGTH]", this.txtBranchAddress.MaxLength.ToString());
                                    break;
                                }
                                else
                                {
                                    newBranch.BranchAddress = item.BranchAddress.ToString().Trim();
                                }
                            }

                            if (ValidateIsNotNull(item.CityName))
                            {
                                newBranch.City = new City() { Name = item.CityName.ToString().Trim() };
                            }

                            if (ValidateIsNotNull(item.StateName))
                            {
                                newBranch.State = new State() { Name = item.StateName.ToString().Trim() };
                            }

                            if (ValidateIsNotNull(item.CountryName))
                            {
                                newBranch.Country = new Country() { Name = item.CountryName.ToString().Trim() };
                            }

                            if (ValidateIsNotNull(item.Distance))
                            {
                                if (item.Distance.ToString().Trim().Length > this.txtDistance.MaxLength)
                                {
                                    errorUp = "BranchCode " + item.BranchCode + " - " + this.lblFieldLength.Text.Replace("[FIELD]", "Distance").Replace("[LENGTH]", this.txtDistance.MaxLength.ToString());
                                    break;
                                }
                                else
                                {
                                    newBranch.Distance = item.Distance.ToString().Trim();
                                }
                            }

                            if (ValidateIsNotNull(item.Phone))
                            {
                                if (item.Phone.ToString().Trim().Length > this.txtPhone.MaxLength)
                                {
                                    errorUp = "BranchCode " + item.BranchCode + " - " + this.lblFieldLength.Text.Replace("[FIELD]", "Phone").Replace("[LENGTH]", this.txtPhone.MaxLength.ToString());
                                    break;
                                }
                                else
                                {
                                    newBranch.Phone = item.Phone.ToString().Trim();
                                }
                            }

                            if (ValidateIsNotNull(item.SpecialField1))
                                newBranch.SpecialField1 = item.SpecialField1.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField2))
                                newBranch.SpecialField2 = item.SpecialField2.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField3))
                                newBranch.SpecialField3 = item.SpecialField3.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField4))
                                newBranch.SpecialField4 = item.SpecialField4.ToString().Trim();

                            branchViewDTO.Entities.Add(newBranch);
                        }
                    }
                    catch(Exception ex)
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
                        if (branchViewDTO.Entities.Count > 0)
                        {
                            branchViewDTO = iWarehousingMGR.MaintainBranchMassive(branchViewDTO, context);

                            if (branchViewDTO.hasError())
                            {
                                ShowAlertLocal(this.lblTitle.Text, branchViewDTO.Errors.Message);
                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                ucStatus.ShowMessage(branchViewDTO.MessageStatus.Message);
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
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, branchViewDTO.Errors.Message);
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

        protected void grdCustomer_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;

                if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Text == "Dir. Fac. Opc.")
                        {
                            e.Row.Cells[i].Text = this.lblGln.Text;
                        }
                    }
                }
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;

                if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Text == "Distancia")
                        {
                            e.Row.Cells[i].Text = this.lblGln.Text;
                        }
                    }
                }
            }
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
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "BranchMgr";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeLayout();
            PopulateList();
        }
        
        protected void LoadBranchByIdCustomer(int index)
        {
            if (index != -1)
            {
                int id = customerViewDTO.Entities[index].Id;
                this.lblCustomerName.Text = customerViewDTO.Entities[index].Name;

                branchViewDTO = iWarehousingMGR.GetBranchByIdCustomer(context, id);


                if (branchViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!branchViewDTO.hasConfigurationError() && branchViewDTO.Configuration != null && branchViewDTO.Configuration.Count > 0)
                        //base.ConfigureGridOrder(grdMgr, branchViewDTO.Configuration);

                    // Detalle de Recepciones
                    grdMgr.DataSource = branchViewDTO.Entities;
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
                this.Master.ucError.ShowError(branchViewDTO.Errors);
                branchViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            customerViewDTO = iWarehousingMGR.FindAllCustomer(context);

            if (!customerViewDTO.hasError() && customerViewDTO.Entities != null)
            {

                Session.Add(WMSTekSessions.BranchMgr.CustomerList, customerViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(customerViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
            this.Master.ucMainFilter.ownerVisible = true;

            //Si el parametro GS1 esta activado se activa nuevo filtro de busqueda por GLN
            if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
            {
                this.Master.ucMainFilter.codeNumericVisible = true;
                this.Master.ucMainFilter.codeNumericLabel = this.lblGln.Text;
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
            this.Master.ucTaskBar.btnAddToolTip = this.lblAddLoadToolTip.Text;
            this.Master.ucTaskBar.btnExcelVisible = true;
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
            customerViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(customerViewDTO.MessageStatus.Message);
        }

        /// <summary>
        /// Configuracion inicial de la Grilla principal
        /// </summary>
        private void InitializeGrid()
        {
            grdCustomer.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdCustomer.EmptyDataText = this.Master.EmptyGridText;
        }

        /// <summary>
        /// Configuracion inicial del layout
        /// </summary>
        private void InitializeLayout()
        {
            if (branchViewDTO.Configuration == null)
            {
                configurationViewDTO = iConfigurationMGR.GetLayoutConfiguration("Branch_GetByIdCustomer", context);
                if (!configurationViewDTO.hasConfigurationError()) branchViewDTO.Configuration = configurationViewDTO.Configuration;
            }
        }

        

        private void PopulateGrid()
        {
            grdCustomer.PageIndex = currentPage;
            grdCustomer.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!customerViewDTO.hasConfigurationError() && customerViewDTO.Configuration != null && customerViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdCustomer, customerViewDTO.Configuration);

            grdCustomer.DataSource = customerViewDTO.Entities;
            grdCustomer.DataBind();

            ucStatus.ShowRecordInfo(customerViewDTO.Entities.Count, grdCustomer.PageSize, grdCustomer.PageCount, currentPage, grdCustomer.AllowPaging);

            CallJsGridViewHeader();
        }

        private void PopulateList()
        {
            base.ConfigureDDlCountry(this.ddlCountry, true, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlState(this.ddlState, true, IdState, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlCity(this.ddlCity, true, IdCity, IdState, IdCountry, this.Master.EmptyRowText);
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
                divBranch.Visible = false;
                currentPage = 0;
                currentIndex = -1;
                this.Master.ucError.ClearError();
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, int indexBranch, CRUD mode)
        {
            if (currentIndex != -1)
            {

                //Editar UOM
                if (mode == CRUD.Update)
                {
                    if (indexBranch != -1) index = indexBranch;

                    IdCountry = branchViewDTO.Entities[index].Country.Id;
                    IdState = branchViewDTO.Entities[index].State.Id;
                    IdCity = branchViewDTO.Entities[index].City.Id;

                    PopulateList();

                    //Recupera los datos de la entidad a editar
                    hidEditId.Value = branchViewDTO.Entities[index].Id.ToString();

                    txtBranchCode.Text = branchViewDTO.Entities[index].Code;
                    txtBranchName.Text = branchViewDTO.Entities[index].Name;
                    txtDistance.Text = branchViewDTO.Entities[index].Distance;
                    txtPhone.Text = branchViewDTO.Entities[index].Phone;
                    txtBranchAddress.Text = branchViewDTO.Entities[index].BranchAddress;

                    if (branchViewDTO.Entities[index].Country != null)
                        this.ddlCountry.SelectedValue = branchViewDTO.Entities[index].Country.Id.ToString();

                    if (branchViewDTO.Entities[index].State != null)
                        this.ddlState.SelectedValue = branchViewDTO.Entities[index].State.Id.ToString();

                    if (branchViewDTO.Entities[index].City != null)
                        this.ddlCity.SelectedValue = branchViewDTO.Entities[index].City.Id.ToString();

                    if (!string.IsNullOrEmpty(branchViewDTO.Entities[index].SpecialField1))
                        txtSpecialField1.Text = branchViewDTO.Entities[index].SpecialField1;
                    else
                        txtSpecialField1.Text = string.Empty;

                    if (!string.IsNullOrEmpty(branchViewDTO.Entities[index].SpecialField2))
                        txtSpecialField2.Text = branchViewDTO.Entities[index].SpecialField2;
                    else
                        txtSpecialField2.Text = string.Empty;

                    if (!string.IsNullOrEmpty(branchViewDTO.Entities[index].SpecialField3))
                        txtSpecialField3.Text = branchViewDTO.Entities[index].SpecialField3;
                    else
                        txtSpecialField3.Text = string.Empty;

                    if (!string.IsNullOrEmpty(branchViewDTO.Entities[index].SpecialField4))
                        txtSpecialField4.Text = branchViewDTO.Entities[index].SpecialField4;
                    else
                        txtSpecialField4.Text = string.Empty;

                    lblNew.Visible = false;
                    lblEdit.Visible = true;
                }

                // Nueva UOM
                if (mode == CRUD.Create)
                {
                    hidEditId.Value = "0";
                    txtBranchCode.Text = string.Empty;
                    txtBranchName.Text = string.Empty;
                    txtDistance.Text = string.Empty;
                    txtPhone.Text = string.Empty;
                    txtBranchAddress.Text = string.Empty;
                    txtSpecialField1.Text = string.Empty;
                    txtSpecialField2.Text = string.Empty;
                    txtSpecialField3.Text = string.Empty;
                    txtSpecialField4.Text = string.Empty;

                    this.ddlCountry.SelectedValue = "-1";
                    this.ddlState.SelectedValue = "-1";
                    this.ddlCity.SelectedValue = "-1";                   
                    
                    lblNew.Visible = true;
                    lblEdit.Visible = false;
                }

                if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
                {
                    this.lblDistance.Text = this.lblGln.Text;
                }

                if (branchViewDTO.Configuration != null && branchViewDTO.Configuration.Count > 0)
                {
                    if (mode == CRUD.Create)
                        base.ConfigureModal(branchViewDTO.Configuration, true);
                    else
                        base.ConfigureModal(branchViewDTO.Configuration, false);
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
                pnl.Visible = true;
                lblEmpty.Visible = false;
            }
            else
            {
                divBranch.Visible = true;
                lblEmpty.Visible = true;
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el pais... cambiará el estado y la ciudad
                //isNew = true;
                IdState = 0;
                IdCity = 0;
                base.ConfigureDDlState(this.ddlState, true, IdState, Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
                base.ConfigureDDlCity(this.ddlCity, true, IdCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el estado, solo cambia la ciudad.
                //isNew = true;
                IdCity = 0;
                base.ConfigureDDlCity(this.ddlCity, true, IdCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                branchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(branchViewDTO.Errors);
            }
        }

        /// <summary>
        /// Persiste los cambios realizados en la entidad
        /// </summary>
        /// <param name="index"></param>
        protected void SaveChanges()
        {
            Branch branch = new Branch();
            customerViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.BranchMgr.CustomerList];

            //Agrega los datos del Uom

            branch.Id = Convert.ToInt32(hidEditId.Value);
            branch.Customer = new Customer(customerViewDTO.Entities[currentIndex].Id);
            branch.Owner = new Owner(customerViewDTO.Entities[currentIndex].Owner.Id);

            branch.Code = this.txtBranchCode.Text.Trim();
            branch.Name = this.txtBranchName.Text.Trim();
            branch.BranchAddress = this.txtBranchAddress.Text.Trim();
            branch.Phone = this.txtPhone.Text.Trim();
            branch.SpecialField1 = !string.IsNullOrEmpty(txtSpecialField1.Text) ? txtSpecialField1.Text.Trim() : null;
            branch.SpecialField2 = !string.IsNullOrEmpty(txtSpecialField2.Text) ? txtSpecialField2.Text.Trim() : null;
            branch.SpecialField3 = !string.IsNullOrEmpty(txtSpecialField3.Text) ? txtSpecialField3.Text.Trim() : null;
            branch.SpecialField4 = !string.IsNullOrEmpty(txtSpecialField4.Text) ? txtSpecialField4.Text.Trim() : null;

            if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))) && !string.IsNullOrEmpty(txtDistance.Text.Trim()))
            {
                string resultGLN = ValidateCodeGLN(this.txtDistance.Text.Trim());

                if (string.IsNullOrEmpty(resultGLN))
                {
                    branch.Distance = txtDistance.Text.Trim();
                }
                else
                {
                    RequiredFieldValidator val = new RequiredFieldValidator();
                    //CustomValidator val = new CustomValidator();
                    val.ErrorMessage = resultGLN;
                    val.ControlToValidate = "ctl00$MainContent$txtDistance";
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
                branch.Distance = this.txtDistance.Text.Trim();
            }                        
            
            branch.Country = new Country();
            if (this.ddlCountry.SelectedValue != "-1")
                branch.Country.Id = Convert.ToInt32(this.ddlCountry.SelectedValue);
            
            branch.State = new State();
            if (this.ddlState.SelectedValue != "-1")
                branch.State.Id = Convert.ToInt32(this.ddlState.SelectedValue);
           
            branch.City = new City();
            if (this.ddlCity.SelectedValue != "-1")
                branch.City.Id = Convert.ToInt32(this.ddlCity.SelectedValue);
            
    
            if (hidEditId.Value == "0")
                branchViewDTO = iWarehousingMGR.MaintainBranch(CRUD.Create, branch, context);
            else
                branchViewDTO = iWarehousingMGR.MaintainBranch(CRUD.Update, branch, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (branchViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(branchViewDTO.MessageStatus.Message);
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
            branchViewDTO = iWarehousingMGR.MaintainBranch(CRUD.Delete, branchViewDTO.Entities[index], context);

            if (branchViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(branchViewDTO.MessageStatus.Message);

                //Actualiza grilla
                UpdateSession(false);
            }
        }

       
        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('Customer_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdCustomer', 'BranchMgr');", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDropCustom();", true);
        }
        
        #endregion
    }
}
