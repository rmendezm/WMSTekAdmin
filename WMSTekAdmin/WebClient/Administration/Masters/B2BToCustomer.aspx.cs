using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class B2BToCustomer : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<Customer> customerDTO = new GenericViewDTO<Customer>();
        private GenericViewDTO<CustomerB2B> customerB2BDTO = new GenericViewDTO<CustomerB2B>();
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

        public int currentIndexDetail
        {
            get
            {
                if (ValidateViewState("indexDetail"))
                    return (int)ViewState["indexDetail"];
                else
                    return -1;
            }

            set { ViewState["indexDetail"] = value; }
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
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
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
                        PopulateGridDetail();
                    }
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            }
        }

        protected void btnAddB2B_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ClearBranchesConfigured();
                lblTitleAddB2BCustomer.Text = "Agregar B2B";

                ImageButton btn = (ImageButton)sender;
                int index = int.Parse(btn.CommandArgument);

                int indexSelected = grdMgr.PageSize * grdMgr.PageIndex + index;
                currentIndex = indexSelected;

                var customers = (GenericViewDTO<Customer>)Session[WMSTekSessions.B2BCustomer.CustomerList];
                var customerSelected = customers.Entities[indexSelected];

                Session.Add(WMSTekSessions.B2BCustomer.CustomerSelected, customerSelected);

                PopulateLists();
                setInitialStateValidators();

                ClearPopupAddB2BCustomer();

                divDetail.Visible = false;
                grdDetail.DataSource = null;
                grdDetail.DataBind();
                upGridDetail.Update();

                grdBranches.EmptyDataText = string.Empty;
                grdBranches.DataBind();


                int idCustomer = customerDTO.Entities[index].Id;
                int idWhs = Convert.ToInt32(context.MainFilter.Find(p => p.Name == "Warehouse").FilterValues[0].Value);

                setEnableDivs(-1, idCustomer, idWhs, context);

                hidEditIdCustomerB2B.Value = "-1";

                upAddB2BCustomer.Update();

                modalAddB2BCustomer.Show();          
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            } 
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
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
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //capturo la posicion de la fila 
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndex = grdMgr.SelectedIndex;

                    var customers = (GenericViewDTO<Customer>)Session[WMSTekSessions.B2BCustomer.CustomerList];
                    var customerSelected = customers.Entities[index];
                    Session.Add(WMSTekSessions.B2BCustomer.CustomerSelected, customerSelected);

                    LoadDetail(index);
                }
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.DataItem != null)
                    {
                        Customer customer = (Customer)e.Row.DataItem;
                        ImageButton btnSchedule = e.Row.FindControl("btnAddB2B") as ImageButton;

                        //if (customer.CustomerB2B.Id != -1)
                        //{
                        //    btnSchedule.Visible = false;
                        //}
                    }

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        //No haga postback en ciertas columnas
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                        // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            }
        }

        protected void btnAcceptAddB2BCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                var idWarehouse = context.MainFilter.Find(p => p.Name == "Warehouse");
                var idCustomerB2B = hidEditIdCustomerB2B.Value;

                var customerSelected = (Customer)Session[WMSTekSessions.B2BCustomer.CustomerSelected];

                if (customerSelected == null)
                {
                    ucStatus.ShowWarning("No hay cliente seleccionado");
                    return;
                }

                customerSelected.CustomerB2B.Id = int.Parse(idCustomerB2B);

                if (customerSelected.CustomerB2B.Id < 1)
                {
                    //Insertar
                    CustomerB2B custB2B = null;
                    Customer customer = customerDTO.Entities[currentIndex];
                    CorrelativeCustomerB2B correlativeLPN = null;
                    bool flagCustomerB2B = false;
                    bool flagBranch = false;
                    bool flagCustomer = false;
                    bool flagLPN = false;
                    bool flagCorrelativeBranch = false;

                    if (rfvAsnTemplate.Enabled || rfvTemplateLpn.Enabled || rfvTemplatePrice.Enabled || rfvLabelCodeLPNPackingList.Enabled)
                    {
                        custB2B = new CustomerB2B(); 
                        
                        custB2B.IdCustomer = customer.Id;
                        custB2B.ASNFile = chkAnsFile.Checked;
                        custB2B.TemplateASNFile = chkAnsFile.Checked == true ? ddlTemplateAsn.SelectedItem.Value : null;
                        custB2B.LabelLPN = chkLabelLpn.Checked;
                        custB2B.LabelCodeLPN = chkLabelLpn.Checked == true ? ddlLabelLpn.SelectedItem.Value : null;
                        custB2B.LabelPrice = chkLabelPrice.Checked;
                        custB2B.LabelCodePrice = chkLabelPrice.Checked == true ? ddlLabelPrice.SelectedItem.Value : null;
                        custB2B.UomTypeLpn = chkUomTypeLpn.Checked;
                        custB2B.UomTypeLpnCode = chkUomTypeLpn.Checked == true ? ddlUomTypeLpnCode.SelectedItem.Value : null;
                        custB2B.LabelLPNPackingList = chkLabelLPNPackingList.Checked;
                        custB2B.LabelCodeLPNPackingList = chkLabelLPNPackingList.Checked ? ddlLabelCodeLPNPackingList.SelectedItem.Value : null;
                        custB2B.MaxLinesPackingList = chkLabelLPNPackingList.Checked ? int.Parse(txtMaxLinesPackingList.Text.Trim()) : -1;

                        flagCustomerB2B = true;
                    }

                    List<Branch> listBranches = new List<Branch>();
                    List<CorrelativeCustomerB2B> listCorrelativeCustomerB2BBranch = new List<CorrelativeCustomerB2B>();

                    for (int i = 0; i < grdBranches.Rows.Count; i++)
                    {
                        GridViewRow row = grdBranches.Rows[i];

                        TextBox txtPrefixBranch = (TextBox)row.FindControl("txtPrefixBranch");
                        Label lblIdBranch = (Label)row.FindControl("lblIdBranch");

                        if (!string.IsNullOrEmpty(txtPrefixBranch.Text))
                        {
                            Branch branch = new Branch();
                            branch.Id = int.Parse(lblIdBranch.Text.Trim());
                            branch.PrefixLabel = txtPrefixBranch.Text.Trim();
                            listBranches.Add(branch);

                            flagBranch = true;
                        }
                    }

                    listCorrelativeCustomerB2BBranch = GetBranchesConfigured();

                    if (listCorrelativeCustomerB2BBranch.Count > 0)
                        flagCorrelativeBranch = true;

                    if (!string.IsNullOrEmpty(txtCorrelativeCustomer.Text))
                    {
                        customer.PrefixLabel = txtCorrelativeCustomer.Text.Trim();
                        flagCustomer = true;
                    }

                    if (!string.IsNullOrEmpty(txtCorrelativeLPN.Text) && chkLabelLpn.Enabled)
                    {
                        if (ddlLabelLpn.SelectedItem.Value == "-1")
                        {
                            ucStatus.ShowWarning("Debe seleccionar Template Etiqueta Lpn");
                            modalAddB2BCustomer.Show();
                            return;
                        }

                        correlativeLPN = new CorrelativeCustomerB2B();
                        correlativeLPN.Owner = new Owner();
                        correlativeLPN.Owner.Id = customer.Owner.Id;
                        correlativeLPN.Warehouse = new Warehouse();
                        correlativeLPN.Warehouse.Id = Convert.ToInt32(idWarehouse.FilterValues[0].Value);
                        correlativeLPN.Customer = new Customer();
                        correlativeLPN.Customer.Id = customer.Id;
                        correlativeLPN.Branch = new Branch();
                        correlativeLPN.Branch.Id = -1;
                        correlativeLPN.LabelCodeLPN = ddlLabelLpn.SelectedItem.Value;
                        correlativeLPN.Correlative = int.Parse(txtCorrelativeLPN.Text.Trim());
                        correlativeLPN.NumberLength = string.IsNullOrEmpty(txtNumberLength.Text) ? -1 : int.Parse(txtNumberLength.Text.Trim());

                        flagLPN = true;
                    }

                    if (!flagCustomerB2B && !flagBranch && !flagCustomer && !flagLPN && !flagCorrelativeBranch)
                    {
                        ShowAlertLocal("Advertencia", lblValidatorCreateCustomerB2B.Text);
                    }
                    else
                    {
                        Customer customerParam;
                        if (flagCustomer)
                        {
                            customerParam = customer;
                        }
                        else
                        {
                            customerParam = null;
                        }

                        List<Branch> listBranchesParam;
                        if (flagBranch)
                        {
                            listBranchesParam = listBranches;
                        }
                        else
                        {
                            listBranchesParam = null;
                        }

                        customerB2BDTO = iWarehousingMGR.AddCustomerB2B(CRUD.Create, custB2B, customerParam, listBranchesParam, correlativeLPN, listCorrelativeCustomerB2BBranch, context);

                        if (customerB2BDTO.hasError())
                        {
                            this.Master.ucError.ShowError(customerB2BDTO.Errors);
                        }
                        else
                        {
                            modalAddB2BCustomer.Hide();
                            ClearPopupAddB2BCustomer();
                            upGrid.Update();
                            ReloadData();
                            ucStatus.ShowMessage(lblMessajeCreateOK.Text);
                            currentIndex = -1;
                            ClearBranchesConfigured();
                        }  
                    }                    
                } 
                else
                {
                    //Actualizar

                    Customer customer = customerDTO.Entities[currentIndex];
                    customer.PrefixLabel = string.IsNullOrEmpty(txtCorrelativeCustomer.Text) ? null : txtCorrelativeCustomer.Text.Trim();

                    var customerFacadeB2BDTO = (GenericViewDTO<FacadeCustomerB2B>)Session[WMSTekSessions.B2BCustomer.B2BDetail];
                    CustomerB2B custB2B = customerFacadeB2BDTO.Entities[currentIndexDetail].customerB2B;
                    custB2B.IdCustomer = customer.Id;
                    custB2B.ASNFile = chkAnsFile.Checked;
                    custB2B.TemplateASNFile = chkAnsFile.Checked == true ? ddlTemplateAsn.SelectedItem.Value : null;
                    custB2B.LabelLPN = chkLabelLpn.Checked;
                    custB2B.LabelCodeLPN = chkLabelLpn.Checked == true ? ddlLabelLpn.SelectedItem.Value : null;
                    custB2B.LabelPrice = chkLabelPrice.Checked;
                    custB2B.LabelCodePrice = chkLabelPrice.Checked == true ? ddlLabelPrice.SelectedItem.Value : null;
                    custB2B.UomTypeLpn = chkUomTypeLpn.Checked;
                    custB2B.UomTypeLpnCode = chkUomTypeLpn.Checked == true ? ddlUomTypeLpnCode.SelectedItem.Value : null;
                    custB2B.LabelLPNPackingList = chkLabelLPNPackingList.Checked;
                    custB2B.LabelCodeLPNPackingList = chkLabelLPNPackingList.Checked ? ddlLabelCodeLPNPackingList.SelectedItem.Value : null;
                    custB2B.MaxLinesPackingList = chkLabelLPNPackingList.Checked ? int.Parse(txtMaxLinesPackingList.Text.Trim()) : -1;

                    List<Branch> listBranches = new List<Branch>();
                    List<CorrelativeCustomerB2B> listCorrelativeCustomerB2BBranch = new List<CorrelativeCustomerB2B>();

                    for (int i = 0; i < grdBranches.Rows.Count; i++)
                    {
                        GridViewRow row = grdBranches.Rows[i];

                        TextBox txtPrefixBranch = (TextBox)row.FindControl("txtPrefixBranch");
                        Label lblIdBranch = (Label)row.FindControl("lblIdBranch");
                        Label lblIdWhs = (Label)row.FindControl("lblIdWhs");
                        TextBox txtCorrelativeBranch = (TextBox)row.FindControl("txtCorrelativeBranch");
                        TextBox txtNumberLength = (TextBox)row.FindControl("txtNumberLength");

                        Branch branch = new Branch();
                        branch.Id = int.Parse(lblIdBranch.Text.Trim());
                        branch.PrefixLabel = string.IsNullOrEmpty(txtPrefixBranch.Text) ? null : txtPrefixBranch.Text.Trim();
                        listBranches.Add(branch);
                    }

                    listCorrelativeCustomerB2BBranch = GetBranchesConfigured();

                    if (divCorrelativeLPN.Visible == true)
                    {
                        var correlativeCustomerB2BDTO = (GenericViewDTO<CorrelativeCustomerB2B>)Session[WMSTekSessions.B2BCustomer.CorrelativeCustomerB2B];

                        //Solo CorrelativeCustomerB2B con id branch null
                        if (correlativeCustomerB2BDTO.Entities.Count > 0 && correlativeCustomerB2BDTO.Entities.Where(x => x.Branch.Id == -1).ToList().Count > 0)
                        {
                            if (string.IsNullOrEmpty(txtCorrelativeLPN.Text))
                            {
                                ucStatus.ShowWarning("Debe ingresar correlativo LPN");
                                modalAddB2BCustomer.Show();
                                return;
                            }

                            customer.CorrelativeCustomerB2B = correlativeCustomerB2BDTO.Entities.Where(x => x.Branch.Id == -1).FirstOrDefault();
                            customer.CorrelativeCustomerB2B.Correlative = string.IsNullOrEmpty(txtCorrelativeLPN.Text) ? -1 : int.Parse(txtCorrelativeLPN.Text.Trim());
                            customer.CorrelativeCustomerB2B.NumberLength = string.IsNullOrEmpty(txtNumberLength.Text) ? -1 : int.Parse(txtNumberLength.Text.Trim());
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(txtCorrelativeLPN.Text))
                            {
                                if (ddlLabelLpn.SelectedItem.Value == "-1")
                                {
                                    ucStatus.ShowWarning("Debe seleccionar Template Etiqueta Lpn");
                                    modalAddB2BCustomer.Show();
                                    return;
                                }

                                customer.CorrelativeCustomerB2B = new CorrelativeCustomerB2B();
                                customer.CorrelativeCustomerB2B.Owner = new Owner();
                                customer.CorrelativeCustomerB2B.Owner.Id = customer.Owner.Id;
                                customer.CorrelativeCustomerB2B.Warehouse = new Warehouse();
                                customer.CorrelativeCustomerB2B.Warehouse.Id = Convert.ToInt32(idWarehouse.FilterValues[0].Value);
                                customer.CorrelativeCustomerB2B.Customer = new Customer();
                                customer.CorrelativeCustomerB2B.Customer.Id = customer.Id;
                                customer.CorrelativeCustomerB2B.Branch = new Branch();
                                customer.CorrelativeCustomerB2B.Branch.Id = -1;
                                customer.CorrelativeCustomerB2B.LabelCodeLPN = ddlLabelLpn.SelectedItem.Value;
                                customer.CorrelativeCustomerB2B.Correlative = int.Parse(txtCorrelativeLPN.Text.Trim());
                                customer.CorrelativeCustomerB2B.NumberLength = string.IsNullOrEmpty(txtNumberLength.Text.Trim()) ? -1 : int.Parse(txtNumberLength.Text.Trim());
                            }
                            else
                            {
                                customer.CorrelativeCustomerB2B = null;
                            }
                        }
                    }
                    else
                    {
                        customer.CorrelativeCustomerB2B = null;
                    }
                     
                    customerB2BDTO = iWarehousingMGR.AddCustomerB2B(CRUD.Update, custB2B, customer, listBranches, customer.CorrelativeCustomerB2B, listCorrelativeCustomerB2BBranch, context); 

                    if (customerB2BDTO.hasError())
                    {
                        this.Master.ucError.ShowError(customerB2BDTO.Errors);
                    }
                    else
                    {
                        modalAddB2BCustomer.Hide();
                        ClearPopupAddB2BCustomer();
                        upGridDetail.Update();
                        LoadDetail(currentIndex);
                        ucStatus.ShowMessage(lblMessajeUpdateOK.Text);
                        ClearBranchesConfigured();
                    }
                }
            }
            catch (Exception ex)
            {
                customerDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerDTO.Errors);
            }
        }

        protected void grdDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a eliminar
                int deleteIndex = grdDetail.PageSize * grdDetail.PageIndex + e.RowIndex;

                DeleteRow(deleteIndex);
            }
            catch (Exception ex)
            {
                customerB2BDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerB2BDTO.Errors);
            }
        }

        protected void chkAnsFile_CheckedChanged(object sender, EventArgs e)
        {
            this.rfvAsnTemplate.Enabled = this.chkAnsFile.Checked;
            this.ddlTemplateAsn.Enabled = this.chkAnsFile.Checked;
            this.ddlTemplateAsn.SelectedIndex = 0;

            this.divAddB2BCustomer.Visible = true;
            this.modalAddB2BCustomer.Show();
            this.ddlTemplateAsn.Focus();
        }

        protected void chkLabelLpn_CheckedChanged(object sender, EventArgs e)
        {
            this.rfvTemplateLpn.Enabled = this.chkLabelLpn.Checked;
            this.ddlLabelLpn.Enabled = this.chkLabelLpn.Checked;
            this.ddlLabelLpn.SelectedIndex = 0;

            this.divAddB2BCustomer.Visible = true;
            this.modalAddB2BCustomer.Show();
            this.ddlLabelLpn.Focus();


            var customerSelected = (Customer)Session[WMSTekSessions.B2BCustomer.CustomerSelected];
            int IdCustomer = customerSelected.Id;
            var idWarehouse = context.MainFilter.Find(p => p.Name == "Warehouse");

            var customerB2BFacadeDTO = iWarehousingMGR.GetFacadeByIdCustomerByWarehouse(IdCustomer, Convert.ToInt32(idWarehouse.FilterValues[0].Value), context);
            if (customerB2BFacadeDTO != null && customerB2BFacadeDTO.Entities.Count > 0)
            {
                divCorrelativeLPN.Visible = (customerB2BFacadeDTO.Entities.Count >= 1 ? false : true); 
            }
            else
            {
                divCorrelativeLPN.Visible = chkLabelLpn.Checked;
            }


            for (int i = 0; i < grdBranches.Rows.Count; i++)
            {
                GridViewRow row = grdBranches.Rows[i];
                TextBox txtCorrelativeBranch = (TextBox)row.FindControl("txtCorrelativeBranch");
                txtCorrelativeBranch.Enabled = chkLabelLpn.Checked;
            }
            upAddB2BCustomer.Update();
        }

        protected void chkLabelPrice_CheckedChanged(object sender, EventArgs e)
        {
            this.rfvTemplatePrice.Enabled = this.chkLabelPrice.Checked;
            this.ddlLabelPrice.Enabled = this.chkLabelPrice.Checked;
            this.ddlLabelPrice.SelectedIndex = 0;

            this.divAddB2BCustomer.Visible = true;
            this.modalAddB2BCustomer.Show();
            this.ddlLabelPrice.Focus();
        }

        protected void chkUomTypeLpn_CheckedChanged(object sender, EventArgs e)
        {
            this.rfvUomTypeLpnCode.Enabled = this.chkUomTypeLpn.Checked;
            this.ddlUomTypeLpnCode.Enabled = this.chkUomTypeLpn.Checked;
            this.ddlUomTypeLpnCode.SelectedIndex = 0;

            this.divAddB2BCustomer.Visible = true;
            this.modalAddB2BCustomer.Show();
            this.ddlUomTypeLpnCode.Focus();
        }

        protected void grdDetail_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                var idWarehouse = context.MainFilter.Find(p => p.Name == "Warehouse");

                if (currentIndex != -1)
                {
                    ClearBranchesConfigured();       
                    lblTitleAddB2BCustomer.Text = "Modificar B2B";

                    PopulateLists();

                    txtSearhBranchByCode.Text = string.Empty;
                    txtSearhBranchByName.Text = string.Empty;
                    grdBranches.DataSource = null;
                    grdBranches.DataBind();

                    //FillGridBranches(customerDTO.Entities[currentIndex].Owner.Id, customerDTO.Entities[currentIndex].Id, grdBranches, null, null, true);

                    int editIndex = grdDetail.PageSize * grdDetail.PageIndex + e.NewEditIndex;
                    currentIndexDetail = editIndex;

                    var customerB2BFacadeDTO = (GenericViewDTO<FacadeCustomerB2B>)Session[WMSTekSessions.B2BCustomer.B2BDetail];
                    CustomerB2B customerB2B = customerB2BFacadeDTO.Entities[editIndex].customerB2B;
                    hidEditIdCustomerB2B.Value = customerB2B.Id.ToString();

                    chkAnsFile.Checked = customerB2B.ASNFile;
                    rfvAsnTemplate.Enabled = customerB2B.ASNFile;
                    ddlTemplateAsn.Enabled = customerB2B.ASNFile;
                    if (!string.IsNullOrEmpty(customerB2B.TemplateASNFile))
                    {
                        var filexsdTemp = customerB2B.TemplateASNFile.Split('\\');

                        if (filexsdTemp.Length > 0)
                        {
                            var filexsd = filexsdTemp[filexsdTemp.Length - 1];
                            string selected = string.Empty;

                            foreach (ListItem item in ddlTemplateAsn.Items)
                            {
                                if (item.Text.EndsWith(filexsd))
                                {
                                    selected = item.Value;
                                    break;
                                }
                            }

                            if (!string.IsNullOrEmpty(selected))
                            {
                                ddlTemplateAsn.SelectedValue = selected;
                            }
                        }
                    }

                    chkLabelLpn.Checked = customerB2B.LabelLPN;
                    divCorrelativeLPN.Visible = customerB2B.LabelLPN;
                    rfvTemplateLpn.Enabled = customerB2B.LabelLPN;
                    ddlLabelLpn.Enabled = customerB2B.LabelLPN;
                    if (!string.IsNullOrEmpty(customerB2B.LabelCodeLPN))
                    {
                        ddlLabelLpn.SelectedValue = customerB2B.LabelCodeLPN;
                    }

                    chkLabelPrice.Checked = customerB2B.LabelPrice;
                    rfvTemplatePrice.Enabled = customerB2B.LabelPrice;
                    ddlLabelPrice.Enabled = customerB2B.LabelPrice;
                    if (!string.IsNullOrEmpty(customerB2B.LabelCodePrice))
                    {
                        ddlLabelPrice.SelectedValue = customerB2B.LabelCodePrice;
                    }

                    chkUomTypeLpn.Checked = customerB2B.UomTypeLpn;
                    rfvUomTypeLpnCode.Enabled = customerB2B.UomTypeLpn;
                    ddlUomTypeLpnCode.Enabled = true;
                    if (!string.IsNullOrEmpty(customerB2B.UomTypeLpnCode))
                    {
                        ddlUomTypeLpnCode.SelectedValue = customerB2B.UomTypeLpnCode.ToUpper();
                    }

                    chkLabelLPNPackingList.Checked = customerB2B.LabelLPNPackingList;
                    rfvLabelCodeLPNPackingList.Enabled = customerB2B.LabelLPNPackingList;
                    rqvMaxLinesPackingList.Enabled = customerB2B.LabelLPNPackingList;
                    revtxtMaxLinesPackingList.Enabled = customerB2B.LabelLPNPackingList;
                    ddlLabelCodeLPNPackingList.Enabled = customerB2B.LabelLPNPackingList;
                    txtMaxLinesPackingList.Enabled = customerB2B.LabelLPNPackingList;

                    if (!string.IsNullOrEmpty(customerB2B.LabelCodeLPNPackingList))
                    {
                        ddlLabelCodeLPNPackingList.SelectedValue = customerB2B.LabelCodeLPNPackingList;
                    }

                    if (customerB2B.MaxLinesPackingList > -1)
                    {
                        txtMaxLinesPackingList.Text = customerB2B.MaxLinesPackingList.ToString();
                    }

                    Customer customer = customerDTO.Entities[currentIndex];
                    txtCorrelativeCustomer.Text = customer.PrefixLabel;

                    CorrelativeCustomerB2B correlativeParam = new CorrelativeCustomerB2B();
                    correlativeParam.Owner = new Owner();
                    correlativeParam.Owner.Id = customer.Owner.Id;
                    correlativeParam.Warehouse = new Warehouse();
                    correlativeParam.Warehouse.Id = Convert.ToInt32(idWarehouse.FilterValues[0].Value);
                    correlativeParam.Customer = new Customer();
                    correlativeParam.Customer.Id = customer.Id;

                    var correlativeCustomerB2BDTO = iWarehousingMGR.GetCorrrelativeB2B(correlativeParam, context);

                    if (correlativeCustomerB2BDTO.hasError())
                    {
                        this.Master.ucError.ShowError(correlativeCustomerB2BDTO.Errors);
                    }
                    else
                    {
                        Session.Add(WMSTekSessions.B2BCustomer.CorrelativeCustomerB2B, correlativeCustomerB2BDTO);

                        if (correlativeCustomerB2BDTO.Entities.Count > 0 && correlativeCustomerB2BDTO.Entities.Where(x => x.Branch.Id == -1).ToList().Count > 0)
                        {
                            if (correlativeCustomerB2BDTO.Entities.Where(x => x.Branch.Id == -1).FirstOrDefault().Correlative.ToString() == "-1")
                            {
                                txtCorrelativeLPN.Text = string.Empty;
                            }
                            else
                            {
                                txtCorrelativeLPN.Text = correlativeCustomerB2BDTO.Entities.Where(x => x.Branch.Id == -1).FirstOrDefault().Correlative.ToString();
                            }
                            txtNumberLength.Text = correlativeCustomerB2BDTO.Entities.Where(x => x.Branch.Id == -1).FirstOrDefault().NumberLength == 0  ? "" : correlativeCustomerB2BDTO.Entities.Where(x => x.Branch.Id == -1).FirstOrDefault().NumberLength.ToString();
                        }

                        //var customerB2BList = iWarehousingMGR.GetFacadeByIdCustomerByWarehouse(customer.Id, Convert.ToInt32(idWarehouse.FilterValues[0].Value), context);
                        //if (customerB2BList != null && customerB2BList.Entities.Count > 0)
                        //{
                        //    setEnableDivs(customerB2BList.Entities.Count > 1 ? false : true);
                        //}
                        //else
                        //{
                        //    setEnableDivs(true);
                        //}

                        //if (!string.IsNullOrEmpty(customerB2B.LabelCodeLPN) && (!string.IsNullOrEmpty(customerB2B.LabelCodePrice) || (!string.IsNullOrEmpty(customerB2B.LabelCodeLPNPackingList)))
                        //{
                        //    setEnableDivs(true);
                        //}
                        //else
                        //{
                        //    setEnableDivs(false);
                        //}

                        setEnableDivs(customerB2B.Id, customer.Id, Convert.ToInt32(idWarehouse.FilterValues[0].Value), context);


                        upAddB2BCustomer.Update();
                        modalAddB2BCustomer.Show();
                    }     
                }
            }
            catch (Exception ex)
            {
                customerB2BDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerB2BDTO.Errors);
            }
        }

        protected void btnSearchBranch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtSearhBranchByCode.Text) && string.IsNullOrEmpty(txtSearhBranchByName.Text))
                {
                    this.Master.ucDialog.ShowAlert("Advertencia", lblValidateFiltersSearchBranch.Text, "");
                    modalAddB2BCustomer.Show();
                }
                else
                {
                    FillGridBranches(customerDTO.Entities[currentIndex].Owner.Id, customerDTO.Entities[currentIndex].Id, grdBranches, txtSearhBranchByCode.Text.Trim(), txtSearhBranchByName.Text.Trim(), false);
                    modalAddB2BCustomer.Show();
                }
            }
            catch (Exception ex)
            {
                customerB2BDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerB2BDTO.Errors);
            }
        }

        protected void chkLabelLPNPackingList_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkLabelLPNPackingList.Checked)
                {
                    ddlLabelCodeLPNPackingList.Enabled = false;
                    rfvLabelCodeLPNPackingList.Enabled = false;

                    if (ddlLabelCodeLPNPackingList.SelectedIndex > -1)
                        ddlLabelCodeLPNPackingList.SelectedIndex = 0;
                    else
                        ddlLabelCodeLPNPackingList.SelectedIndex = -1;                   

                    txtMaxLinesPackingList.Enabled = false;
                    txtMaxLinesPackingList.Text = string.Empty;
                    rqvMaxLinesPackingList.Enabled = false;
                    revtxtMaxLinesPackingList.Enabled = false;
                }
                else
                {
                    ddlLabelCodeLPNPackingList.Enabled = true;
                    rfvLabelCodeLPNPackingList.Enabled = true;

                    txtMaxLinesPackingList.Enabled = true;
                    rqvMaxLinesPackingList.Enabled = true;
                    revtxtMaxLinesPackingList.Enabled = true;
                }

                modalAddB2BCustomer.Show();
            }
            catch (Exception ex)
            {
                customerB2BDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerB2BDTO.Errors);
            }
        }
        protected void grdBranches_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditBranch")
                {
                    var branchId = Convert.ToInt32(e.CommandArgument);
                    hidBranchId.Value = branchId.ToString();

                    base.LoadLabel(this.ddlBranchLpnLabel, "AND LabelCode LIKE 'SHIPLPN%'", this.Master.EmptyRowText, true);
                    base.LoadLabel(this.ddlBranchPriceLabel, "AND LabelCode LIKE 'SHIPPRI%'", this.Master.EmptyRowText, true);

                    GridViewRow row = null;

                    for (int i = 0; i < grdBranches.Rows.Count; i++)
                    {
                        row = grdBranches.Rows[i];
                        var lblIdBranch = (Label)row.FindControl("lblIdBranch");

                        if (int.Parse(lblIdBranch.Text) == branchId)
                        {
                            break;
                        }
                    }

                    if (row != null)
                    {
                        var txtCodeCorrelativeLpnBranch = (TextBox)row.FindControl("txtCodeCorrelativeLpnBranch");
                        var txtCodeCorrelativePriceBranch = (TextBox)row.FindControl("txtCodeCorrelativePriceBranch");

                        if (!string.IsNullOrEmpty(txtCodeCorrelativeLpnBranch.Text) && ddlBranchLpnLabel.Items.Count > 0)
                        {
                            if (ddlBranchLpnLabel.Items.FindByValue(txtCodeCorrelativeLpnBranch.Text.Trim()) != null)
                            {
                                ddlBranchLpnLabel.SelectedValue = txtCodeCorrelativeLpnBranch.Text;
                            }
                        }

                        if (!string.IsNullOrEmpty(txtCodeCorrelativePriceBranch.Text) && ddlBranchPriceLabel.Items.Count > 0)
                        {
                            if (ddlBranchPriceLabel.Items.FindByValue(txtCodeCorrelativePriceBranch.Text.Trim()) != null)
                            {
                                ddlBranchPriceLabel.SelectedValue = txtCodeCorrelativePriceBranch.Text;
                            }
                        }
                    }

                    divBranchCorrelative.Visible = true;
                    modalBranch.Show();
                    upEditBranchCorrelative.Update();
                }
                else if (e.CommandName == "SaveBranch")
                {
                    var customers = (GenericViewDTO<Customer>)Session[WMSTekSessions.B2BCustomer.CustomerList];
                    var customerSelected = customers.Entities[currentIndex];

                    var idOwn = customerSelected.Owner.Id;
                    var idWhs = int.Parse(context.MainFilter.Find(p => p.Name == "Warehouse").FilterValues[0].Value);
                    var idCustomer = customerSelected.Id;

                    var correlativeByBranch = new CorrelativeCustomerB2B();
                    var branchId = Convert.ToInt32(e.CommandArgument);

                    for (int i = 0; i < grdBranches.Rows.Count; i++)
                    {
                        var row = grdBranches.Rows[i];
                        var lblIdBranch = (Label)row.FindControl("lblIdBranch");
                        var lblCode = (Label)row.FindControl("lblCode");

                        var txtCorrelativeBranch = (TextBox)row.FindControl("txtCorrelativeBranch");
                        var txtNumberLength = (TextBox)row.FindControl("txtNumberLength");
                        var txtCodeCorrelativeLpnBranch = (TextBox)row.FindControl("txtCodeCorrelativeLpnBranch");
                        var txtCodeCorrelativePriceBranch = (TextBox)row.FindControl("txtCodeCorrelativePriceBranch");
                        var txtPrefixBranch = (TextBox)row.FindControl("txtPrefixBranch");
                        var lblIdWhs = (Label)row.FindControl("lblIdWhs");

                        if (int.Parse(lblIdBranch.Text) == branchId)
                        {
                            if (!string.IsNullOrEmpty(txtCodeCorrelativeLpnBranch.Text) || !string.IsNullOrEmpty(txtCodeCorrelativePriceBranch.Text))
                            {
                                correlativeByBranch.Branch = new Branch();
                                correlativeByBranch.Branch.Id = branchId;
                                correlativeByBranch.Branch.Code = lblCode.Text.Trim();
                                correlativeByBranch.Branch.PrefixLabel = string.IsNullOrEmpty(txtPrefixBranch.Text) ? null : txtPrefixBranch.Text.Trim();

                                correlativeByBranch.Owner = new Owner();
                                correlativeByBranch.Owner.Id = idOwn;
                                correlativeByBranch.Warehouse = new Warehouse();
                                correlativeByBranch.Warehouse.Id = idWhs;
                                correlativeByBranch.Customer = new Customer();
                                correlativeByBranch.Customer.Id = idCustomer;

                                correlativeByBranch.LabelCodeLPN = string.IsNullOrEmpty(txtCodeCorrelativeLpnBranch.Text) ? null : txtCodeCorrelativeLpnBranch.Text.Trim();
                                correlativeByBranch.Correlative = string.IsNullOrEmpty(txtCorrelativeBranch.Text) ? -1 : int.Parse(txtCorrelativeBranch.Text);
                                correlativeByBranch.NumberLength = string.IsNullOrEmpty(txtNumberLength.Text) ? -1 : int.Parse(txtNumberLength.Text.Trim());
                                correlativeByBranch.LabelCodePrice = string.IsNullOrEmpty(txtCodeCorrelativePriceBranch.Text) ? null : txtCodeCorrelativePriceBranch.Text.Trim();

                                var param = new CorrelativeCustomerB2B()
                                {
                                    Warehouse = new Warehouse() { Id = idWhs },
                                    Owner = new Owner() { Id = idOwn },
                                    Customer = new Customer() { Id = idCustomer },
                                    Branch = new Branch() { Id = branchId }
                                };
                                var existsCorrelative = iWarehousingMGR.GetCorrelativeCustomerB2BByAnyParameter(param, context);

                                correlativeByBranch.isNew = existsCorrelative.Entities.Count == 0;

                                SetBranchConfigured(correlativeByBranch);

                                txtSearhBranchByName.Text = string.Empty;
                                txtSearhBranchByCode.Text = string.Empty;

                                grdBranches.DataSource = null;
                                grdBranches.DataBind();
                            }

                            break;
                        }
                    }

                    modalAddB2BCustomer.Show();
                }
            }
            catch (Exception ex)
            {
                customerB2BDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerB2BDTO.Errors);
            }
        }
        protected void btnUpdateBranch_Click(object sender, EventArgs e)
        {
            try
            {
                var branchId = Convert.ToInt32(hidBranchId.Value);
                var branchLpnLabelSelected = ddlBranchLpnLabel.SelectedItem.Value;
                var branchPriceLabel = ddlBranchPriceLabel.SelectedItem.Value;

                for (int i = 0; i < grdBranches.Rows.Count; i++)
                {
                    var row = grdBranches.Rows[i];
                    var lblIdBranch = (Label)row.FindControl("lblIdBranch");

                    if (lblIdBranch != null && int.Parse(lblIdBranch.Text.Trim()) == branchId)
                    {
                        var txtCodeCorrelativeLpnBranch = (TextBox)row.FindControl("txtCodeCorrelativeLpnBranch");
                        var txtCodeCorrelativePriceBranch = (TextBox)row.FindControl("txtCodeCorrelativePriceBranch");

                        if (txtCodeCorrelativeLpnBranch != null)
                            txtCodeCorrelativeLpnBranch.Text = branchLpnLabelSelected != "-1" ? branchLpnLabelSelected : string.Empty;

                        if (txtCodeCorrelativePriceBranch != null)
                            txtCodeCorrelativePriceBranch.Text = branchPriceLabel != "-1" ? branchPriceLabel : string.Empty;

                        break;
                    }
                }

                hidBranchId.Value = string.Empty;
                CloseBranchPopUp();

                divAddB2BCustomer.Visible = true;
                upAddB2BCustomer.Update();
                modalAddB2BCustomer.Show();
            }
            catch (Exception ex)
            {
                customerB2BDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerB2BDTO.Errors);
            }
        }
        protected void btnCancelUpdateBranch_Click(object sender, EventArgs e)
        {
            try
            {
                CloseBranchPopUp();
            }
            catch (Exception ex)
            {
                customerB2BDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerB2BDTO.Errors);
            }
        }
        private void CloseBranchPopUp()
        {
            divBranchCorrelative.Visible = false;
            modalBranch.Hide();
            upEditBranchCorrelative.Update();
        }
        #endregion

        #region "Metodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "B2BToCustomer";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
            }
            else
            {
                if (ValidateSession(WMSTekSessions.B2BCustomer.CustomerList))
                {
                    customerDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.B2BCustomer.CustomerList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            //this.Master.ucTaskBar.btnAddVisible = true;

            //this.Master.ucTaskBar.btnAddToolTip = lblAddLoadToolTip.Text;
            //this.Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;

            //Codigo Cliente
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = lblTitleCustomerCode.Text;
            //Nombre Cliente
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.nameLabel = lblTitleCustomerName.Text;

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);

            this.Master.ucMainFilter.Initialize(init, refresh);
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
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            var idWarehouse = context.MainFilter.Find(p => p.Name == "Warehouse");
            
            customerDTO = iWarehousingMGR.FindAllCustomerByWarehouse(context, Convert.ToInt32(idWarehouse.FilterValues[0].Value));

            if (customerDTO != null && customerDTO.Entities.Count > 0)
            {
                Session.Add(WMSTekSessions.B2BCustomer.CustomerList, customerDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(customerDTO.MessageStatus.Message);
            }
            else
            {
                if (customerDTO.Errors == null)
                {
                    ucStatus.ShowMessage(this.Master.EmptyGridText);
                }
                else if (customerDTO.Errors.Code == WMSTekError.DataBase.NoRowsReturned)
                {
                    ucStatus.ShowMessage(this.Master.EmptyGridText);
                }
                else
                {
                    customerDTO.Errors = baseControl.handleError(customerDTO.Errors);
                    this.Master.ucError.ShowError(customerDTO.Errors);
                }
            }
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!customerDTO.hasConfigurationError() && customerDTO.Configuration != null && customerDTO.Configuration.Count > 0)
                //base.ConfigureGridOrder(grdMgr, customerDTO.Configuration);

            // Encabezado de agendamientos
            grdMgr.DataSource = customerDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(customerDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                var customerFacadeB2BDTO = (GenericViewDTO<FacadeCustomerB2B>)Session[WMSTekSessions.B2BCustomer.B2BDetail];

                // Configura ORDEN de las columnas de la grilla
                if (!customerFacadeB2BDTO.hasConfigurationError() && customerFacadeB2BDTO.Configuration != null && customerFacadeB2BDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, customerFacadeB2BDTO.Configuration);

                // Detalle de Documentos de Entrada
                grdDetail.DataSource = customerFacadeB2BDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();

            }
        }

        /// <summary>
        /// Retorna el detalle de cada despacho
        /// </summary>
        /// <param name="index"></param>
        protected void LoadDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                int IdCustomer = customerDTO.Entities[index].Id;
                var idWarehouse = context.MainFilter.Find(p => p.Name == "Warehouse");

                var customerB2BFacadeDTO = iWarehousingMGR.GetFacadeByIdCustomerByWarehouse(IdCustomer, Convert.ToInt32(idWarehouse.FilterValues[0].Value), context);

                if (customerB2BFacadeDTO.hasError())
                {
                    this.Master.ucError.ShowError(customerB2BFacadeDTO.Errors);
                }
                else
                {
                    if (customerB2BFacadeDTO != null && customerB2BFacadeDTO.Entities.Count > 0)
                    {
                        // Configura ORDEN de las columnas de la grilla
                        if (!customerB2BFacadeDTO.hasConfigurationError() && customerB2BFacadeDTO.Configuration != null && customerB2BFacadeDTO.Configuration.Count > 0)
                            base.ConfigureGridOrder(grdDetail, customerB2BFacadeDTO.Configuration);

                        // Detalle 
                        grdDetail.DataSource = customerB2BFacadeDTO.Entities;
                        grdDetail.DataBind();

                        CallJsGridViewDetail();
                    }

                    Session.Add(WMSTekSessions.B2BCustomer.B2BDetail, customerB2BFacadeDTO);

                    divDetail.Visible = true;
                } 
            }
            else
            {
                divDetail.Visible = false;
            }
        }

        private void DeleteRow(int index)
        {
            var customerFacadeB2BDTO = (GenericViewDTO<FacadeCustomerB2B>)Session[WMSTekSessions.B2BCustomer.B2BDetail];
            var customerB2B = customerFacadeB2BDTO.Entities[index].customerB2B;
            customerB2B.IdCustomer = customerFacadeB2BDTO.Entities[index].customer.Id;

            customerB2BDTO = iWarehousingMGR.MaintainCustomerB2B(CRUD.Delete, customerB2B, context);

            if (customerB2BDTO.hasError())
            {
                this.Master.ucError.ShowError(customerB2BDTO.Errors);
            }
            else
            {
                upGrid.Update();
                ReloadData();

                upGridDetail.Update();
                LoadDetail(currentIndex);             
                ucStatus.ShowMessage(lblMessajeDeleteOK.Text);
            }
        }

        private void setInitialStateValidators()
        {
            rfvAsnTemplate.Enabled = false;
            rfvTemplateLpn.Enabled = false;
            rfvTemplatePrice.Enabled = false;
        }
        private void setEnableDivs(bool avtive)
        {
            divAsn.Visible = avtive;
            divPrice.Visible = avtive;
            divUomType.Visible = avtive;
            divCorrelative.Visible = avtive;
            divCorrelativeLPN.Visible = avtive;
            divPackingList.Visible = avtive;
            divSucursal.Visible = avtive;
        }

        private void setEnableDivs(int idCustomerB2B, int idCustomer, int idWhs, ContextViewDTO context)
        {
            bool active = true;
            string llabelLPN = string.Empty;

            var customerB2BFacadeDTO = iWarehousingMGR.GetFacadeByIdCustomerByWarehouse(idCustomer, idWhs, context);

            if (customerB2BFacadeDTO != null && customerB2BFacadeDTO.Entities.Count > 0)
            {
                ListItem item = new ListItem();

                foreach (var b2b in customerB2BFacadeDTO.Entities)
                {
                    if (idCustomerB2B != b2b.customerB2B.Id)
                    {
                        if (b2b.customerB2B != null && !string.IsNullOrEmpty(b2b.customerB2B.LabelCodeLPN) &&
                        (!string.IsNullOrEmpty(b2b.customerB2B.LabelCodePrice) || !string.IsNullOrEmpty(b2b.customerB2B.LabelCodeLPNPackingList)))
                        {
                                active = false;
                        }

                        // Quita Etiquetas del drop-down list
                        if (ddlLabelLpn.Items.FindByValue(b2b.customerB2B.LabelCodeLPN) != null)
                        {
                           ddlLabelLpn.Items.Remove(ddlLabelLpn.Items.FindByValue(b2b.customerB2B.LabelCodeLPN));
                        }
                    }
                    else
                    {
                        if (b2b.customerB2B != null && !string.IsNullOrEmpty(b2b.customerB2B.LabelCodeLPN) &&
                        (!string.IsNullOrEmpty(b2b.customerB2B.LabelCodePrice) || !string.IsNullOrEmpty(b2b.customerB2B.LabelCodeLPNPackingList)))
                        {
                            active = true;
                        }
                        else
                        {
                            if (customerB2BFacadeDTO.Entities.Count != 1)
                                active = false;
                        }

                        llabelLPN = b2b.customerB2B.LabelCodeLPN;
                    }
                }
            }

            setEnableDivs(active);

            if (idCustomerB2B > 0)
                ddlLabelLpn.SelectedValue = llabelLPN;                      
        }

        private void PopulateLists()
        {
            this.LoadTemplateAsn(this.ddlTemplateAsn);
            LoadUomTypes(ddlUomTypeLpnCode);
            base.LoadLabel(this.ddlLabelLpn, "AND LabelCode LIKE 'SHIPLPN%' OR LabelCode LIKE 'SHIPHU%'", this.Master.EmptyRowText, true);
            base.LoadLabel(this.ddlLabelPrice, "AND LabelCode LIKE 'SHIPPRI%'", this.Master.EmptyRowText, true);
            base.LoadLabel(this.ddlLabelCodeLPNPackingList, "AND LabelCode LIKE 'SHIPLPN%'", this.Master.EmptyRowText, true);
        }

        private void LoadUomTypes(DropDownList ddl)
        {
            const string UNIDAD = "UNIDAD";
            DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");

            var newContext = new ContextViewDTO();
            newContext.MainFilter = new List<EntityFilter>();
            newContext.MainFilter.Add(new EntityFilter() { Name = "Owner", FilterValues = new List<FilterItem>() { new FilterItem() { Name = "Owner", Value = ddlOwn.SelectedValue } } });

            var uomTypeViewDTO = iWarehousingMGR.FindAllUomType(newContext);

            if (!uomTypeViewDTO.hasError() && uomTypeViewDTO.Entities.Count > 0)
            {
                uomTypeViewDTO.Entities.ForEach(uomType => uomType.Name = uomType.Name.ToUpper());

                if (uomTypeViewDTO.Entities.Exists(uomType => uomType.Name == UNIDAD))
                {
                    uomTypeViewDTO.Entities.RemoveAll(uomType => uomType.Name == UNIDAD);
                }

                ddl.Items.Clear();
                ddl.DataSource = uomTypeViewDTO.Entities;
                ddl.DataTextField = "Name";
                ddl.DataValueField = "Name";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
                ddl.Items[0].Selected = true;
            } 
        }

        private void LoadTemplateAsn(DropDownList objControl)
        {
            string logPath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("B2BTemplateAsnPath", "");

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(logPath);

            objControl.Items.Clear();

            foreach (System.IO.FileInfo fi in dirInfo.GetFiles("*.*"))
            {
                objControl.Items.Insert(0, new ListItem(fi.Name, fi.FullName));
            }

            objControl.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
        }

        private void FillGridBranches(int idOwn, int idCustomer, GridView grd, string branchCode, string branchName, bool justConfiguredBranches)
        {
            var getBranchesConfigured = GetBranchesConfigured();

            Branch branchParam = new Branch();
            branchParam.Owner = new Owner();
            branchParam.Owner.Id = idOwn;
            branchParam.Customer = new Customer();
            branchParam.Customer.Id = idCustomer;

            if (!string.IsNullOrEmpty(branchCode))
            {
                branchParam.Code = branchCode;
            }

            if (!string.IsNullOrEmpty(branchName))
            {
                branchParam.Name = branchName;
            }

            var branchesDTOByCustomer = iWarehousingMGR.GetBranchByAnyParameter(branchParam, context);

            if (branchesDTOByCustomer.hasError())
            {
                this.Master.ucError.ShowError(branchesDTOByCustomer.Errors);
            }
            else if (branchesDTOByCustomer.Entities.Count == 0)
            {
                grd.EmptyDataText = lblNoBranchesFound.Text;
                lblBranch.Visible = false;
                grd.DataSource = null;
                grd.DataBind();
                upAddB2BCustomer.Update();
            }
            else
            {
                if (getBranchesConfigured != null && getBranchesConfigured.Count > 0)
                {
                    foreach (var branch in branchesDTOByCustomer.Entities)
                    {
                        var existsBranchInMemory = getBranchesConfigured.Where(b => b.Branch.Code == branch.Code).FirstOrDefault();

                        if (existsBranchInMemory != null)
                        {
                            branch.PrefixLabel = existsBranchInMemory.branch.PrefixLabel;
                            branch.correlativeB2BBranch = existsBranchInMemory;
                        }
                    }
                }
                

                if (justConfiguredBranches)
                {
                    grd.DataSource = branchesDTOByCustomer.Entities.Where(branch => branch.PrefixLabel != null && branch.correlativeB2BBranch.Correlative != -1 && branch.correlativeB2BBranch.NumberLength != 1).ToList();
                }
                else
                {
                    grd.DataSource = branchesDTOByCustomer.Entities;
                }

                grd.DataBind();
                ClearFieldsInBranchGrid();

                upAddB2BCustomer.Update();
            }
        }
        private void ClearFieldsInBranchGrid()
        {
            for (int i = 0; i < grdBranches.Rows.Count; i++)
            {
                GridViewRow row = grdBranches.Rows[i];
                TextBox txtCorrelativeBranch = (TextBox)row.FindControl("txtCorrelativeBranch");
                TextBox txtNumberLength = (TextBox)row.FindControl("txtNumberLength");
                var txtCodeCorrelativeLpnBranch = (TextBox)row.FindControl("txtCodeCorrelativeLpnBranch");
                var txtCodeCorrelativePriceBranch = (TextBox)row.FindControl("txtCodeCorrelativePriceBranch");
                txtCorrelativeBranch.Enabled = true;
                txtNumberLength.Enabled = true;
                txtCodeCorrelativeLpnBranch.Enabled = true;
                txtCodeCorrelativePriceBranch.Enabled = true;
            }
        }

        private void ClearPopupAddB2BCustomer()
        {
            chkAnsFile.Checked = false;
            chkLabelLpn.Checked = false;
            chkLabelPrice.Checked = false;
            chkUomTypeLpn.Checked = false;
            chkLabelLPNPackingList.Checked = false;

            if (ddlLabelPrice.SelectedIndex > -1)
                ddlLabelPrice.SelectedIndex = 0;
            else
                ddlLabelPrice.SelectedIndex = -1;

            if (ddlTemplateAsn.SelectedIndex > -1)
                ddlTemplateAsn.SelectedIndex = 0;
            else
                ddlTemplateAsn.SelectedIndex = -1;

            if (ddlLabelLpn.SelectedIndex > -1)
                ddlLabelLpn.SelectedIndex = 0;
            else
                ddlLabelLpn.SelectedIndex = -1;


            if (ddlUomTypeLpnCode.SelectedIndex > -1)
                ddlUomTypeLpnCode.SelectedIndex = 0;
            else
                ddlUomTypeLpnCode.SelectedIndex = -1;

            if (ddlLabelCodeLPNPackingList.SelectedIndex > -1)
                ddlLabelCodeLPNPackingList.SelectedIndex = 0;
            else
                ddlLabelCodeLPNPackingList.SelectedIndex = -1;

            txtCorrelativeCustomer.Text = string.Empty;
            grdBranches.DataSource = null;
            grdBranches.DataBind();
            txtCorrelativeLPN.Text = string.Empty;
            divCorrelativeLPN.Visible = false;
            txtNumberLength.Text = string.Empty;
            txtSearhBranchByCode.Text = string.Empty;
            txtSearhBranchByName.Text = string.Empty;
            txtMaxLinesPackingList.Text = string.Empty;
            grdBranches.DataSource = null;
            grdBranches.DataBind();

            //grdBranches.EmptyDataText = string.Empty;
            //upAddB2BCustomer.Update();
        }

        public new void ShowAlertLocal(string title, string message)
        {
            string script = "ShowMessage('" + title + "','" + message.Replace("'", "") + "');";
            script = script.Replace("\\", Convert.ToChar(47).ToString());
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('B2BToCustomerDetail', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }
        private List<CorrelativeCustomerB2B> GetBranchesConfigured()
        {
            var branchesCreated = (List<CorrelativeCustomerB2B>)Session[WMSTekSessions.B2BCustomer.BranchList];

            if (branchesCreated == null)
                branchesCreated = new List<CorrelativeCustomerB2B>();

            return branchesCreated;
        }
        private CorrelativeCustomerB2B GetBranchConfigured(string code, string name)
        {
            var branchesCreated = (List<CorrelativeCustomerB2B>)Session[WMSTekSessions.B2BCustomer.BranchList];

            if (branchesCreated != null)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var branch = branchesCreated.Where(b => b.Branch.Code.ToLower().Contains(code.ToLower())).FirstOrDefault();

                    if (branch != null)
                        return branch;
                }
                
                if(!string.IsNullOrEmpty(name))
                {
                    var branch = branchesCreated.Where(b => b.Branch.Name.ToLower().Contains(code.ToLower())).FirstOrDefault();

                    if (branch != null)
                        return branch;
                }
            }

            return null;
        }
        private void SetBranchConfigured(CorrelativeCustomerB2B correlativeCustomerB2B)
        {
            var branchesCreated = (List<CorrelativeCustomerB2B>)Session[WMSTekSessions.B2BCustomer.BranchList];

            if (branchesCreated == null)
            {
                branchesCreated = new List<CorrelativeCustomerB2B>();
                Session.Add(WMSTekSessions.B2BCustomer.BranchList, branchesCreated);
            }

            var existsBranchConfigured = branchesCreated.Where(b => b.Branch.Id == correlativeCustomerB2B.branch.Id).FirstOrDefault();

            if (existsBranchConfigured != null)
            {
                branchesCreated.RemoveAll(b => b.branch.Id == correlativeCustomerB2B.branch.Id);
                ucStatus.ShowMessage("Se modificó sucursal con código " + existsBranchConfigured.Branch.Code);
            }
            else
                ucStatus.ShowMessage("Se agregó sucursal con código " + correlativeCustomerB2B.Branch.Code);

            branchesCreated.Add(correlativeCustomerB2B);
        }
        private void ClearBranchesConfigured()
        {
            Session.Remove(WMSTekSessions.B2BCustomer.BranchList);
        }
        #endregion
    }
}