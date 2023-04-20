using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Administration.Configuration
{
    public partial class VasItemMgr : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<RecipeVas> vasViewDTO;
        public GenericViewDTO<Item> itemViewDTO;
        private bool isValidViewDTO = false;

        private string code = string.Empty;
        private string name = string.Empty;
        private string description = string.Empty;

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
        private int level
        {
            get { return (int)(ViewState["grpLevel"] ?? 0); }
            set { ViewState["grpLevel"] = value; }
        }
        private bool showItems
        {
            get { return (bool)(Session["showItems"] ?? false); }
            set { Session["showItems"] = value; }
        }
        public string currentName
        {
            get
            {
                if (ValidateViewState("name"))
                    return (string)ViewState["name"];
                else
                    return string.Empty;
            }

            set { ViewState["name"] = value; }
        }
        //public int currentIdWhs
        //{
        //    get
        //    {
        //        if (ValidateViewState("idWhs"))
        //            return (int)ViewState["idWhs"];
        //        else
        //            return -1;
        //    }

        //    set { ViewState["idWhs"] = value; }
        //}
        public string currentNameWarehouse
        {
            get
            {
                if (ValidateViewState("nameWhs"))
                    return (string)ViewState["nameWhs"];
                else
                    return string.Empty;
            }

            set { ViewState["nameWhs"] = value; }
        }
        public int currentIdOwner
        {
            get
            {
                if (ValidateViewState("idOwn"))
                    return (int)ViewState["idOwn"];
                else
                    return -1;
            }

            set { ViewState["idOwn"] = value; }
        }
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
        #endregion

        #region "Eventos"
        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        hsMasterDetail.LeftPanel.WidthDefault = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .2);

                        //var objUser = (User)Session[WMSTekSessions.Global.LoggedInUser];

                        if (treevLocation.Nodes != null && treevLocation.Nodes.Count > 0)
                        {
                            treevLocation.Nodes[0].Text = objUser.Company.Name;
                            treevLocation.Nodes[0].ToolTip = objUser.Company.Name;
                            treevLocation.Font.Bold = true;

                            level = 0;

                            loadTreeForFirstTime();
                        }

                        showItems = false;
                        UpdateSession(false);
                    }

                    if (ValidateSession(WMSTekSessions.VasItemMgr.VasList))
                    {
                        vasViewDTO = (GenericViewDTO<RecipeVas>)Session[WMSTekSessions.VasItemMgr.VasList];
                        isValidViewDTO = true;
                    }

                    if (ValidateSession(WMSTekSessions.VasItemMgr.ItemList))
                    {
                        itemViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.VasItemMgr.ItemList];
                    }

                    if (isValidViewDTO)
                    {
                        PopulateGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (base.webMode == WebMode.Normal)
                {
                    base.Page_Load(sender, e);
                }

                //ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                if (base.webMode == WebMode.Normal)
                {
                    if (isValidViewDTO)
                    {
                        PopulateGrid();

                        if (showItems)
                        {
                            PopulateGridItems();
                        }
                    }
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
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
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void treevLocation_SelectedNodeChanged(object sender, EventArgs e)
        {
            try
            {
                ClearFilter("IdOwn");

                this.divFilterItem.Visible = true;
                this.divDetail.Visible = true;
                showItems = false;

                TreeView tv = (TreeView)sender;

                string TreeId = HttpUtility.UrlEncode(tv.SelectedNode.ValuePath);

                if (!string.IsNullOrEmpty(TreeId))
                {
                    level = tv.SelectedNode.Depth;

                    switch (level)
                    {
                        case (int)eVasDepthTree.Company:

                            var company = treevLocation.FindNode(TreeId);

                            if (company != null && company.ChildNodes.Count == 0)
                            {
                                company.Text = objUser.Company.Name;
                                company.ToolTip = objUser.Company.Name;

                                var ownViewDTO = iWarehousingMGR.GetOwnersByUser(context, context.SessionInfo.User.Id);

                                if (!ownViewDTO.hasError() && ownViewDTO.Entities.Count > 0)
                                {
                                    foreach (Owner own in ownViewDTO.Entities)
                                    {
                                        TreeNode nodoOwn = new TreeNode(own.Name.ToString(), own.Id.ToString());
                                        company.ChildNodes.Add(nodoOwn);
                                    }
                                }
                            }

                            currentName = objUser.Company.Name;
                            currentNameWarehouse = objUser.Company.Name;

                            HideFiltersAndItems();
                            ReloadData();

                            break;

                        case (int)eVasDepthTree.Owner:

                            showItems = true;

                            currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Text;
                            currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Text;

                            currentIdOwner = Convert.ToInt32(tv.SelectedNode.Value);

                            CreateFilterByList("IdOwn", new List<int> { currentIdOwner });

                            this.divFilter.Visible = true;

                            this.divBscGrpItm2.Visible = false;
                            this.divBscGrpItm3.Visible = false;
                            this.divBscGrpItm4.Visible = false;

                            idGrpItem1 = -1;
                            idGrpItem2 = -1;
                            idGrpItem3 = -1;
                            idGrpItem4 = -1;

                            base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);

                            if (ddlBscGrpItm1.Items.Count <= 1)
                            {
                                this.divBscGrpItm1.Visible = false;
                            }
                            else
                            {
                                this.divBscGrpItm1.Visible = true;
                                this.ddlBscGrpItm1.Visible = true;
                                this.lblTitleGrpItm1.Visible = true;
                                this.lblNameGrp1.Visible = false;
                            }

                            vasViewDTO = iWarehousingMGR.GetListVasByIdOwn(currentIdOwner, context);

                            Session.Add(WMSTekSessions.VasItemMgr.VasList, vasViewDTO);

                            tv.SelectedNode.ChildNodes.Clear();

                            if (tv.SelectedNode.ChildNodes != null)
                            {
                                var grpItem1ViewDTO = iWarehousingMGR.GetGrpItem1Treeview(context, currentIdOwner);

                                if (!grpItem1ViewDTO.hasError() && grpItem1ViewDTO.Entities.Count > 0)
                                {
                                    foreach (GrpItem1 grpItem1 in grpItem1ViewDTO.Entities)
                                    {
                                        if (grpItem1.Id > 0)
                                        {
                                            TreeNode nodoGrp1 = new TreeNode(grpItem1.Name.ToString(), grpItem1.Id.ToString());
                                            tv.SelectedNode.ChildNodes.Add(nodoGrp1);
                                        }
                                    }
                                }
                            }

                            break;

                        case (int)eVasDepthTree.GroupItem1:

                            showItems = true;

                            currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Text;
                            currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Text;

                            currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                            idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Value);

                            CreateFilterByList("IdOwn", new List<int> { currentIdOwner });

                            idGrpItem2 = -1;
                            idGrpItem3 = -1;
                            idGrpItem4 = -1;

                            this.divFilter.Visible = true;
                            this.divBscGrpItm1.Visible = true;
                            this.divBscGrpItm3.Visible = false;
                            this.divBscGrpItm4.Visible = false;

                            this.lblNameGrp1.Text = this.lblSimbol.Text + tv.SelectedNode.Text;
                            this.ddlBscGrpItm1.Visible = false;
                            this.lblNameGrp1.Visible = true;

                            base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                            base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);

                            if (ddlBscGrpItm2.Items.Count <= 1)
                            {
                                this.divBscGrpItm2.Visible = false;
                            }
                            else
                            {
                                this.divBscGrpItm2.Visible = true;
                                this.ddlBscGrpItm2.Visible = true;
                                this.lblTitleGrpItm2.Visible = true;
                                this.lblNameGrp2.Visible = false;
                            }

                            GetParamGrp1ByOwn(false, currentIdOwner, idGrpItem1);

                            tv.SelectedNode.ChildNodes.Clear();

                            if (tv.SelectedNode.ChildNodes != null)
                            {
                                var grpItem2ViewDTO = iWarehousingMGR.GetByIdGrpItem1(context, idGrpItem1);

                                if (!grpItem2ViewDTO.hasError() && grpItem2ViewDTO.Entities.Count > 0)
                                {
                                    foreach (GrpItem2 grpItem2 in grpItem2ViewDTO.Entities)
                                    {
                                        TreeNode nodoGrp2 = new TreeNode(grpItem2.Name.ToString(), grpItem2.Id.ToString());
                                        tv.SelectedNode.ChildNodes.Add(nodoGrp2);
                                    }
                                }
                            }

                            break;

                        case (int)eVasDepthTree.GroupItem2:

                            showItems = true;

                            currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Text;
                            currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Text;

                            currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                            idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                            idGrpItem2 = Convert.ToInt32(tv.SelectedNode.Value);

                            CreateFilterByList("IdOwn", new List<int> { currentIdOwner });

                            idGrpItem3 = -1;
                            idGrpItem4 = -1;

                            this.divFilter.Visible = true;
                            this.divBscGrpItm1.Visible = true;
                            this.divBscGrpItm2.Visible = true;
                            this.divBscGrpItm4.Visible = false;

                            this.lblNameGrp2.Text = this.lblSimbol.Text + tv.SelectedNode.Text;
                            this.ddlBscGrpItm2.Visible = false;
                            this.lblNameGrp2.Visible = true;

                            base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                            base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);
                            base.ConfigureDDLGrpItem3(this.ddlBscGrpItm3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.Master.EmptyRowText, true, currentIdOwner);

                            if (ddlBscGrpItm3.Items.Count <= 1)
                            {
                                this.divBscGrpItm3.Visible = false;
                            }
                            else
                            {
                                this.divBscGrpItm3.Visible = true;
                                this.ddlBscGrpItm3.Visible = true;
                                this.lblTitleGrpItm3.Visible = true;
                                this.lblNameGrp3.Visible = false;
                            }

                            GetParamGrp2ByOwn(false, currentIdOwner, idGrpItem1, idGrpItem2);

                            tv.SelectedNode.ChildNodes.Clear();

                            if (tv.SelectedNode.ChildNodes != null)
                            {
                                var grpItem3ViewDTO = iWarehousingMGR.GetByIdGrpItem1AndIdGrpItem2(context, idGrpItem1, idGrpItem2);

                                if (!grpItem3ViewDTO.hasError() && grpItem3ViewDTO.Entities.Count > 0)
                                {
                                    foreach (GrpItem3 grpItem3 in grpItem3ViewDTO.Entities)
                                    {
                                        TreeNode nodoGrp3 = new TreeNode(grpItem3.Name.ToString(), grpItem3.Id.ToString());
                                        tv.SelectedNode.ChildNodes.Add(nodoGrp3);
                                    }
                                }
                            }

                            break;

                        case (int)eVasDepthTree.GroupItem3:

                            showItems = true;

                            currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Text;
                            currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Text;

                            currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Value);
                            idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                            idGrpItem2 = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                            idGrpItem3 = Convert.ToInt32(tv.SelectedNode.Value);

                            CreateFilterByList("IdOwn", new List<int> { currentIdOwner });

                            idGrpItem4 = -1;

                            this.divFilter.Visible = true;
                            this.divBscGrpItm1.Visible = true;
                            this.divBscGrpItm2.Visible = true;
                            this.divBscGrpItm3.Visible = true;

                            this.lblNameGrp3.Text = this.lblSimbol.Text + tv.SelectedNode.Text;
                            this.ddlBscGrpItm3.Visible = false;
                            this.lblNameGrp3.Visible = true;

                            base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                            base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);
                            base.ConfigureDDLGrpItem3(this.ddlBscGrpItm3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.Master.EmptyRowText, true, currentIdOwner);
                            base.ConfigureDDLGrpItem4(this.ddlBscGrpItm4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.Master.EmptyRowText, true, currentIdOwner);

                            if (ddlBscGrpItm4.Items.Count <= 1)
                            {
                                this.divBscGrpItm4.Visible = false;
                            }
                            else
                            {
                                this.divBscGrpItm4.Visible = true;
                                this.ddlBscGrpItm4.Visible = true;
                                this.lblTitleGrpItm4.Visible = true;
                                this.lblNameGrp4.Visible = false;
                            }

                            GetParamGrp3ByOwn(false, currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3);

                            tv.SelectedNode.ChildNodes.Clear();

                            if (tv.SelectedNode.ChildNodes != null)
                            {
                                var grpItem4ViewDTO = iWarehousingMGR.GetByIdGrpItem1IdGrpItem2AndAndIdGrpItem3(context, idGrpItem1, idGrpItem2, idGrpItem3);

                                if (!grpItem4ViewDTO.hasError() && grpItem4ViewDTO.Entities.Count > 0)
                                {
                                    foreach (GrpItem4 grpItem4 in grpItem4ViewDTO.Entities)
                                    {
                                        TreeNode nodoGrp4 = new TreeNode(grpItem4.Name.ToString(), grpItem4.Id.ToString());
                                        tv.SelectedNode.ChildNodes.Add(nodoGrp4);
                                    }
                                }
                            }

                            break;

                        case (int)eVasDepthTree.GroupItem4:

                            showItems = true;

                            currentNameWarehouse = this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Text;
                            currentName = objUser.Company.Name + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Parent.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Parent.Text + this.lblSimbol.Text + tv.SelectedNode.Text;

                            currentIdOwner = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Parent.Value);
                            idGrpItem1 = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Parent.Value);
                            idGrpItem2 = Convert.ToInt32(tv.SelectedNode.Parent.Parent.Value);
                            idGrpItem3 = Convert.ToInt32(tv.SelectedNode.Parent.Value);
                            idGrpItem4 = Convert.ToInt32(tv.SelectedNode.Value);

                            CreateFilterByList("IdOwn", new List<int> { currentIdOwner });

                            this.divFilter.Visible = true;
                            this.divBscGrpItm1.Visible = true;
                            this.divBscGrpItm2.Visible = true;
                            this.divBscGrpItm3.Visible = true;
                            this.divBscGrpItm4.Visible = true;

                            this.lblNameGrp4.Text = this.lblSimbol.Text + tv.SelectedNode.Text;
                            this.ddlBscGrpItm4.Visible = false;
                            this.lblNameGrp4.Visible = true;

                            GetParamGrp4ByOwn(false, currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4);

                            break;
                    }

                    if (showItems) 
                        UpdateItemSession();
                }
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
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
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void btnSearchItem_Click(object sender, ImageClickEventArgs e)
        {
            code = this.txtCode.Text;
            name = this.txtName.Text;
            description = this.txtDescription.Text;
            showItems = true;

            UpdateItemSession();
        }
        protected void btnItemRefresh_Click(object sender, ImageClickEventArgs e)
        {
            this.txtCode.Text = string.Empty;
            this.txtName.Text = string.Empty;
            this.txtDescription.Text = string.Empty;

            UpdateItemSession();
        }
        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
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
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
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
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
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
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdItem.PageCount - 1;
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void grdItem_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdItem.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void grdItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void grdItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        protected void ddlBscGrpItm1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                showItems = true;

                this.divFilterItem.Visible = true;
                this.divDetail.Visible = true;

                idGrpItem1 = Convert.ToInt32(ddlBscGrpItm1.SelectedValue);

                level = (int)eVasDepthTree.GroupItem1;

                this.divFilter.Visible = true;
                this.divBscGrpItm1.Visible = true;
                this.divBscGrpItm3.Visible = false;
                this.divBscGrpItm4.Visible = false;

                this.lblNameGrp1.Text = this.lblSimbol.Text + ddlBscGrpItm1.SelectedItem.ToString();
                this.ddlBscGrpItm1.Visible = false;
                this.lblNameGrp1.Visible = true;

                base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);

                if (ddlBscGrpItm2.Items.Count <= 1)
                {
                    this.divBscGrpItm2.Visible = false;
                }
                else
                {
                    this.divBscGrpItm2.Visible = true;
                    this.ddlBscGrpItm2.Visible = true;
                    this.lblTitleGrpItm2.Visible = true;
                    this.lblNameGrp2.Visible = false;
                }

                GetParamGrp1ByOwn(false, currentIdOwner, idGrpItem1);

                //owners
                foreach (TreeNode node in treevLocation.Nodes[0].ChildNodes)
                {
                    if (node.ChildNodes.Count > 0)
                    {
                        //grpitem1
                        foreach (TreeNode node2 in node.ChildNodes)
                        {
                            //Pregunta si el id del grpItem1 es igual al que esta seleccionado en el ddl
                            if (Convert.ToInt32(node2.Value) == idGrpItem1)
                            {
                                //Expande el nodo anterior
                                node.Expand();
                                //Selecciona el nodo que tiene el mismo id del ddl
                                node2.Select();
                                //Expande el nodo
                                node2.Expand();

                                currentIdOwner = Convert.ToInt32(treevLocation.SelectedNode.Parent.Value);

                                treevLocation.SelectedNode.ChildNodes.Clear();

                                if (treevLocation.SelectedNode.ChildNodes != null)
                                {
                                    var grpItem2ViewDTO = iWarehousingMGR.GetGrpItem2Treeview(context, currentIdOwner, idGrpItem1);

                                    if (!grpItem2ViewDTO.hasError() && grpItem2ViewDTO.Entities.Count > 0)
                                    {
                                        foreach (GrpItem2 grpItem2 in grpItem2ViewDTO.Entities)
                                        {
                                            TreeNode nodoGrp2 = new TreeNode(grpItem2.Name.ToString(), grpItem2.Id.ToString());
                                            treevLocation.SelectedNode.ChildNodes.Add(nodoGrp2);
                                        }
                                    }
                                }

                                currentNameWarehouse = this.lblSimbol.Text + treevLocation.SelectedNode.Parent.Text;
                                currentName = objUser.Company.Name + this.lblSimbol.Text + treevLocation.SelectedNode.Parent.Text;

                                break;
                            }
                        }
                    }
                }

                UpdateItemSession();
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void ddlBscGrpItm2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                showItems = true;

                this.divFilterItem.Visible = true;
                this.divDetail.Visible = true;

                idGrpItem2 = Convert.ToInt32(ddlBscGrpItm2.SelectedValue);

                level = (int)eVasDepthTree.GroupItem2;

                this.divFilter.Visible = true;
                this.divBscGrpItm1.Visible = true;
                this.divBscGrpItm2.Visible = true;
                this.divBscGrpItm4.Visible = false;

                this.lblNameGrp2.Text = this.lblSimbol.Text + ddlBscGrpItm2.SelectedItem.ToString();
                this.ddlBscGrpItm2.Visible = false;
                this.lblNameGrp2.Visible = true;

                base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);
                base.ConfigureDDLGrpItem3(this.ddlBscGrpItm3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.Master.EmptyRowText, true, currentIdOwner);

                if (ddlBscGrpItm3.Items.Count <= 1)
                {
                    this.divBscGrpItm3.Visible = false;
                }
                else
                { 
                    this.divBscGrpItm3.Visible = true;
                    this.ddlBscGrpItm3.Visible = true;
                    this.lblTitleGrpItm3.Visible = true;
                    this.lblNameGrp3.Visible = false;
                }

                GetParamGrp2ByOwn(false, currentIdOwner, idGrpItem1, idGrpItem2);

                //owners
                foreach (TreeNode node in treevLocation.Nodes[0].ChildNodes)
                {
                    if (node.ChildNodes.Count > 0)
                    {
                        //grpitem1
                        foreach (TreeNode node2 in node.ChildNodes)
                        {
                            if (node2.ChildNodes.Count > 0)
                            {
                                //grpitem2
                                foreach (TreeNode node3 in node2.ChildNodes)
                                {
                                    if (Convert.ToInt32(node3.Value) == idGrpItem2)
                                    {
                                        //Expande los nodos seleccionados
                                        node2.Expand();
                                        node3.Select();
                                        node3.Expand();

                                        treevLocation.SelectedNode.ChildNodes.Clear();

                                        if (treevLocation.SelectedNode.ChildNodes != null)
                                        {
                                            var grpItem3ViewDTO = iWarehousingMGR.GetGrpItem3Treeview(context, currentIdOwner, idGrpItem1, idGrpItem2);

                                            if (!grpItem3ViewDTO.hasError() && grpItem3ViewDTO.Entities.Count > 0)
                                            {
                                                foreach (GrpItem3 grpItem3 in grpItem3ViewDTO.Entities)
                                                {
                                                    TreeNode nodoGrp3 = new TreeNode(grpItem3.Name.ToString(), grpItem3.Id.ToString());
                                                    treevLocation.SelectedNode.ChildNodes.Add(nodoGrp3);
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                UpdateItemSession();
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void ddlBscGrpItm3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                showItems = true;

                this.divFilterItem.Visible = true;
                this.divDetail.Visible = true;

                idGrpItem3 = Convert.ToInt32(ddlBscGrpItm3.SelectedValue);

                level = (int)eVasDepthTree.GroupItem3;

                this.divFilter.Visible = true;
                this.divBscGrpItm1.Visible = true;
                this.divBscGrpItm2.Visible = true;
                this.divBscGrpItm3.Visible = true;

                this.lblNameGrp3.Text = this.lblSimbol.Text + ddlBscGrpItm3.SelectedItem.ToString();
                this.ddlBscGrpItm3.Visible = false;
                this.lblNameGrp3.Visible = true;

                base.ConfigureDDLGrpItem1(this.ddlBscGrpItm1, true, idGrpItem1, this.Master.EmptyRowText, true, currentIdOwner);
                base.ConfigureDDLGrpItem2(this.ddlBscGrpItm2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, true, currentIdOwner);
                base.ConfigureDDLGrpItem3(this.ddlBscGrpItm3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.Master.EmptyRowText, true, currentIdOwner);
                base.ConfigureDDLGrpItem4(this.ddlBscGrpItm4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.Master.EmptyRowText, true, currentIdOwner);

                if (ddlBscGrpItm4.Items.Count <= 1)
                {
                    this.divBscGrpItm4.Visible = false;
                }
                else
                {
                    this.divBscGrpItm4.Visible = true;
                    this.ddlBscGrpItm4.Visible = true;
                    this.lblTitleGrpItm4.Visible = true;
                    this.lblNameGrp4.Visible = false;
                }

                GetParamGrp3ByOwn(false, currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3);

                //owner
                foreach (TreeNode node in treevLocation.Nodes[0].ChildNodes)
                {
                    if (node.ChildNodes.Count > 0)
                    {
                        //grpitem1
                        foreach (TreeNode node2 in node.ChildNodes)
                        {
                            if (node2.ChildNodes.Count > 0)
                            {
                                //grpitem2
                                foreach (TreeNode node3 in node2.ChildNodes)
                                {
                                    if (node3.ChildNodes.Count > 0)
                                    {
                                        //grpitem3
                                        foreach (TreeNode node4 in node3.ChildNodes)
                                        {
                                            if (Convert.ToInt32(node4.Value) == idGrpItem3)
                                            {
                                                node3.Expand();
                                                node4.Select();
                                                node4.Expand();

                                                treevLocation.SelectedNode.ChildNodes.Clear();

                                                if (treevLocation.SelectedNode.ChildNodes != null)
                                                {
                                                    var grpItem4ViewDTO = iWarehousingMGR.GetGrpItem4Treeview(context, currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3);

                                                    if (!grpItem4ViewDTO.hasError() && grpItem4ViewDTO.Entities.Count > 0)
                                                    {
                                                        foreach (GrpItem4 grpItem4 in grpItem4ViewDTO.Entities)
                                                        {
                                                            TreeNode nodoGrp4 = new TreeNode(grpItem4.Name.ToString(), grpItem4.Id.ToString());
                                                            treevLocation.SelectedNode.ChildNodes.Add(nodoGrp4);
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                UpdateItemSession();
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        protected void ddlBscGrpItm4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                showItems = true;

                this.divFilterItem.Visible = true;
                this.divDetail.Visible = true;

                idGrpItem4 = Convert.ToInt32(ddlBscGrpItm4.SelectedValue);

                level = (int)eVasDepthTree.GroupItem4;

                this.divFilter.Visible = true;
                this.divBscGrpItm1.Visible = true;
                this.divBscGrpItm2.Visible = true;
                this.divBscGrpItm3.Visible = true;
                this.divBscGrpItm4.Visible = true;

                this.lblNameGrp4.Text = this.lblSimbol.Text + ddlBscGrpItm4.SelectedItem.ToString();
                this.ddlBscGrpItm4.Visible = false;
                this.lblNameGrp4.Visible = true;

                this.divFilterItem.Visible = true;
                this.divDetail.Visible = true;

                GetParamGrp4ByOwn(false, currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4);

                //owners
                foreach (TreeNode node in treevLocation.Nodes[0].ChildNodes)
                {
                    if (node.ChildNodes.Count > 0)
                    {
                        //grpitem1
                        foreach (TreeNode node2 in node.ChildNodes)
                        {
                            if (node2.ChildNodes.Count > 0)
                            {
                                //grpitem2
                                foreach (TreeNode node3 in node2.ChildNodes)
                                {
                                    if (node3.ChildNodes.Count > 0)
                                    {
                                        //grpitem3
                                        foreach (TreeNode node4 in node3.ChildNodes)
                                        {
                                            if (node4.ChildNodes.Count > 0)
                                            {
                                                //grpitem4
                                                foreach (TreeNode node5 in node4.ChildNodes)
                                                {
                                                    if (Convert.ToInt32(node5.Value) == idGrpItem4)
                                                    {
                                                        node4.Expand();
                                                        node5.Select();

                                                        treevLocation.SelectedNode.ChildNodes.Clear();

                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                            
                    }
                }

                UpdateItemSession();
            }
            catch (Exception ex)
            {
                vasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        #endregion

        #region "Métodos"
        public void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnSaveClick += new EventHandler(btnSave_Click);

            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnSaveVisible = true;

        }
        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.nameVisible = false;
            this.Master.ucMainFilter.descriptionVisible = false;
            this.Master.ucMainFilter.searchVisible = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
        }
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);

            // Mensaje a mostrar en los mantenedores que no cargan inicialmente la grilla
            vasViewDTO = new GenericViewDTO<RecipeVas>();
            vasViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(vasViewDTO.MessageStatus.Message);
        }
        private void InitializeGrid()
        {
            grdMgr.EmptyDataText = this.Master.EmptyGridText;

            grdItem.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdItem.EmptyDataText = this.Master.EmptyGridText;
        }
        protected void ReloadData()
        {
            UpdateSession(false);

            if (isValidViewDTO)
            {
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }
        protected void SaveChanges()
        {
            if (currentIdOwner > -1)
            {
                var listVas = GetListVas();

                if (listVas.Count > 0)
                {
                    var insertVasItem = iWarehousingMGR.CreateMassiveVasItems(context, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, currentIdOwner, listVas);

                    if (insertVasItem.hasError())
                    {
                        this.Master.ucError.ShowError(insertVasItem.Errors);
                        UpdateSession(true);
                    }
                    else
                    {
                        crud = true;
                        ucStatus.ShowMessage(insertVasItem.MessageStatus.Message);
                        UpdateSession(false);
                    }
                }
            }
        }
        private List<RecipeVas> GetListVas()
        {
            var vasList = new List<RecipeVas>();

            vasViewDTO = (GenericViewDTO<RecipeVas>)Session[WMSTekSessions.VasItemMgr.VasList];

            if (vasViewDTO != null)
            {
                if (this.grdMgr.Visible == true)
                {
                    int post = grdMgr.PageSize * grdMgr.PageIndex;

                    for (int i = 0; i < grdMgr.Rows.Count; i++)
                    {
                        GridViewRow row = grdMgr.Rows[i];

                        var vas = vasViewDTO.Entities[post + i];

                        if (((CheckBox)row.FindControl("chkSelectVas")).Checked)
                            vas.IsAssignedVasItem = true;
                        else
                            vas.IsAssignedVasItem = false;

                        vasList.Add(vas);
                    }
                }
            }

            return vasList;
        }
        private void UpdateSession(bool showError)
        {
            if (showError && vasViewDTO.Errors != null)
            {
                this.Master.ucError.ShowError(vasViewDTO.Errors);
                vasViewDTO.ClearError();
                ucStatus.ShowMessage(vasViewDTO.MessageStatus.Message);
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            switch (level)
            {
                case (int)eVasDepthTree.Company:
                    vasViewDTO = new GenericViewDTO<RecipeVas>();
                    break;

                case (int)eVasDepthTree.Owner:

                    vasViewDTO = iWarehousingMGR.GetListVasByIdOwn(currentIdOwner, context);
                    break;

                case (int)eVasDepthTree.GroupItem1:
                    vasViewDTO = iWarehousingMGR.GetListVasByIdOwnAndIdGrpItem1(currentIdOwner, idGrpItem1, context);
                    break;

                case (int)eVasDepthTree.GroupItem2:
                    vasViewDTO = iWarehousingMGR.GetListVasByIdOwnAndIdGrpItem2(currentIdOwner, idGrpItem1, idGrpItem2, context);
                    break;

                case (int)eVasDepthTree.GroupItem3:
                    vasViewDTO = iWarehousingMGR.GetListVasByIdOwnAndIdGrpItem3(currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3, context);
                    break;

                case (int)eVasDepthTree.GroupItem4:
                    vasViewDTO = iWarehousingMGR.GetListVasByIdOwnAndIdGrpItem4(currentIdOwner, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, context);
                    break;
            }

            if (!vasViewDTO.hasError() && vasViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.VasItemMgr.VasList, vasViewDTO);

                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(vasViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        private void loadTreeForFirstTime()
        {
            showItems = true;
            this.divFilterItem.Visible = true;
            this.divDetail.Visible = true;

            if (treevLocation != null && treevLocation.Nodes.Count > 0)
            {
                string TreeId = HttpUtility.UrlEncode(this.treevLocation.Nodes[0].ValuePath);

                if (!string.IsNullOrEmpty(TreeId))
                {
                    showItems = false;

                    var company = treevLocation.FindNode(TreeId);

                    if (company != null && company.ChildNodes.Count == 0)
                    {
                        if (objUser != null)
                        {
                            company.Text = objUser.Company.Name;
                            company.ToolTip = objUser.Company.Name; 
                        }

                        var ownViewDTO = iWarehousingMGR.GetOwnersByUser(context, context.SessionInfo.User.Id);

                        if (!ownViewDTO.hasError() && ownViewDTO.Entities.Count > 0)
                        {
                            foreach (var own in ownViewDTO.Entities)
                            {
                                TreeNode nodoOwn = new TreeNode(own.Name.ToString(), own.Id.ToString());
                                company.ChildNodes.Add(nodoOwn);

                                nodoOwn.Collapse();
                            }
                        }
                    }

                    if (objUser != null)
                        currentName = objUser.Company.Name;

                    currentNameWarehouse = string.Empty;

                    HideFiltersAndItems();
                    ReloadData();
                }
            }
        }
        private void HideFiltersAndItems()
        {
            this.divFilterItem.Visible = false;
            this.divFilter.Visible = false;

            this.lblNameGrp1.Visible = false;
            this.lblNameGrp2.Visible = false;
            this.lblNameGrp3.Visible = false;
            this.lblNameGrp4.Visible = false;

            this.divBscGrpItm1.Visible = false;
            this.divBscGrpItm2.Visible = false;
            this.divBscGrpItm3.Visible = false;
            this.divBscGrpItm4.Visible = false;

            this.ddlBscGrpItm1.Visible = false;
            this.ddlBscGrpItm2.Visible = false;
            this.ddlBscGrpItm3.Visible = false;
            this.ddlBscGrpItm4.Visible = false;

            this.divDetail.Visible = false;
        }
        private void PopulateGrid()
        {
            vasViewDTO = (GenericViewDTO<RecipeVas>)Session[WMSTekSessions.VasItemMgr.VasList];

            if (vasViewDTO != null)
            {
                grdMgr.PageIndex = currentPage;

                if (!vasViewDTO.hasConfigurationError() && vasViewDTO.Configuration != null && vasViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdMgr, vasViewDTO.Configuration);

                grdMgr.DataSource = vasViewDTO.Entities;
                grdMgr.DataBind();

                CallJsGridView();

                if (vasViewDTO.Entities.Count > 0)
                {
                    if (currentName == string.Empty)
                        currentName = objUser.Company.Name;

                    this.lblTitle.Text = currentName;
                }
                else
                    this.lblTitle.Text = string.Empty;
            }
        }
        private void PopulateGridItems()
        {
            grdItem.PageIndex = currentPage;

            itemViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.VasItemMgr.ItemList];

            if (itemViewDTO != null)
            {
                if (!itemViewDTO.hasConfigurationError() && itemViewDTO.Configuration != null && itemViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdItem, itemViewDTO.Configuration);

                grdItem.DataSource = itemViewDTO.Entities;
                grdItem.DataBind();

                CallJsGridView();

                ucStatus.ShowRecordInfo(itemViewDTO.Entities.Count, grdItem.PageSize, grdItem.PageCount, currentPage, grdItem.AllowPaging);
            }

            divDetail.Visible = true;
        }
        private void UpdateItemSession()
        {
            var itemViewDTO = iWarehousingMGR.GetItemByGroupsAndFilters(idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, code, name, description, context);

            if (!itemViewDTO.hasError())
            {
                isValidViewDTO = true;
                Session.Add(WMSTekSessions.VasItemMgr.ItemList, itemViewDTO);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(itemViewDTO.Errors);
            }
        }
        private void GetParamGrp1ByOwn(bool showError, int idOwn, int idGrp1)
        {
            vasViewDTO = new GenericViewDTO<RecipeVas>();

            if (showError)
            {
                this.Master.ucError.ShowError(vasViewDTO.Errors);
                vasViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            vasViewDTO = iWarehousingMGR.GetListVasByIdOwnAndIdGrpItem1(idOwn, idGrp1, context);

            if (!vasViewDTO.hasError() && vasViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.VasItemMgr.VasList, vasViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        private void GetParamGrp2ByOwn(bool showError, int idOwn, int idGrp1, int idGrp2)
        {
            vasViewDTO = new GenericViewDTO<RecipeVas>();

            if (showError)
            {
                this.Master.ucError.ShowError(vasViewDTO.Errors);
                vasViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            vasViewDTO = iWarehousingMGR.GetListVasByIdOwnAndIdGrpItem2(idOwn, idGrp1, idGrp2, context);

            if (!vasViewDTO.hasError() && vasViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.VasItemMgr.VasList, vasViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        private void GetParamGrp3ByOwn(bool showError, int idOwn, int idGrp1, int idGrp2, int idGrp3)
        {
            vasViewDTO = new GenericViewDTO<RecipeVas>();

            if (showError)
            {
                this.Master.ucError.ShowError(vasViewDTO.Errors);
                vasViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            vasViewDTO = iWarehousingMGR.GetListVasByIdOwnAndIdGrpItem3(idOwn, idGrp1, idGrp2, idGrp3, context);

            if (!vasViewDTO.hasError() && vasViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.VasItemMgr.VasList, vasViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        private void GetParamGrp4ByOwn(bool showError, int idOwn, int idGrp1, int idGrp2, int idGrp3, int idGrp4)
        {
            vasViewDTO = new GenericViewDTO<RecipeVas>();

            if (showError)
            {
                this.Master.ucError.ShowError(vasViewDTO.Errors);
                vasViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            vasViewDTO = iWarehousingMGR.GetListVasByIdOwnAndIdGrpItem4(idOwn, idGrp1, idGrp2, idGrp3, idGrp4, context);

            if (!vasViewDTO.hasError() && vasViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.VasItemMgr.VasList, vasViewDTO);
                isValidViewDTO = true;
                PopulateGrid();
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(vasViewDTO.Errors);
            }
        }
        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridWithNoDragAndDrop(true);", true);
        }
        #endregion
    }
}