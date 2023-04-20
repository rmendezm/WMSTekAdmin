<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="UsefulLifeMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.UsefulLifeMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>


<asp:Content runat="server" ID="content1" Visible="true" ContentPlaceHolderID="MainContent">

    <script type="text/javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("UsefulLife_FindAll", "ctl00_MainContent_grdMgr");

            Sys.Application.add_init(appl_init);
        });

        function appl_init() {
            var pgRegMgr = Sys.WebForms.PageRequestManager.getInstance();
            pgRegMgr.add_beginRequest(beforeAsyncPostBack);
            pgRegMgr.add_endRequest(afterAsyncPostBack);
        }

        function beforeAsyncPostBack() {

        }

        function afterAsyncPostBack() {
            initializeGridDragAndDrop("UsefulLife_FindAll", "ctl00_MainContent_grdMgr");
        }

        function appl_init() {
            var pgRegMgr = Sys.WebForms.PageRequestManager.getInstance();
            pgRegMgr.add_beginRequest(beforeAsyncPostBack);
            pgRegMgr.add_endRequest(afterAsyncPostBack);
        }
    </script>
    <style>

        #ctl00_MainContent_pnlPanelPoUp{
            max-height: 400px !important;
        }

        .divLookupGrid{
            overflow: auto;
        }	
	
	    .divLookupGrid .froze-header-grid {
		    max-height: 280px;
	    }

        #ctl00_MainContent_pnlLookupItem, #ctl00_MainContent_pnlLookupCustomer{
            overflow: hidden !important;
        }
    </style>
    <div class="container">
        <div class="row">
            <div class="col-md-12">
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                         <%--Grilla Principal--%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                        <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" 
                            OnRowDeleting="grdMgr_RowDeleting" OnRowEditing="grdMgr_RowEditing" OnPageIndexChanging="grdMgr_PageIndexChanging"
                            OnRowDataBound="grdMgr_RowDataBound"
                            AllowPaging="True" EnableViewState="False" 
                            AutoGenerateColumns="False"
                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                            EnableTheming="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 60px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CausesValidation="false" CommandName="Delete" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwner" runat="server" Text='<%# Eval ( "Owner.Name" ) %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Código Cliente" AccessibleHeaderText="CustomerCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCodigoCustomer" runat="server" Text='<%# Eval ( "Customer.Code" ) %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="Customer">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCustomer" runat="server" Text='<%# Eval ( "Customer.Name" ) %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Sucursal" AccessibleHeaderText="Branch">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblBranch" runat="server" Text='<%# Eval ( "Branch.Name" ) %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="Item">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "Item.Code" ) %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Descripcion" AccessibleHeaderText="ItemDescription">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval ( "Item.Description" ) %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Centro Distr." AccessibleHeaderText="whsName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIWhsName" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Cantidad Dias" AccessibleHeaderText="DayQty">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblDayQty" runat="server" Text='<%# Eval ( "DayQty" ) %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                </Columns>
                            </asp:GridView>
                        </div>
                        <%-- FIN Grilla Principal --%>

                        <%-- Pop up Editar/Nueva vida util --%>
                        <div id="divModal" runat="server" visible="false">    
                            <!-- Boton 'dummy' para propiedad TargetControlID -->
                            <%--<ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="btnDummy"
                                PopupControlID="pnlOwner" BackgroundCssClass="modalBackground" PopupDragHandleControlID="OwnerCaption" Drag="true">
                            </ajaxToolkit:ModalPopupExtender>--%>
                            <asp:Panel ID="PnlOwner" runat="server" CssClass="modalBox">
                                <%-- Encabezado --%>
                                <asp:Panel ID="OwnerCaption" runat="server" CssClass="modalHeader">
                                    <div class="divCaption">
                                        <asp:Label ID="lblNew" runat="server" Text="Configuración Vida Útil Cliente" />
                                        <asp:Label ID="lblEdit" runat="server" Text="Editar " />
                                        <%--<asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />--%>
                                    </div>
                                </asp:Panel>
                                <%-- Fin Encabezado --%>
                                <div class="modalControls">
                                    <%--<asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                                    <asp:HiddenField ID="hidEditIndex" runat="server" Value="-1" />--%>
                                     <ajaxToolkit:TabContainer runat="server" ID="tabOwner" ActiveTabIndex="0">
                                        <ajaxToolkit:TabPanel runat="server" ID="tabGeneral">
                                            <ContentTemplate>
                                                <div class="">
                                                    <asp:HiddenField ID="hidEditId" runat="server" Value="0" />

                                                     <%-- Owner --%>
                                                    <div id="divWarehouses" runat="server" class="divControls">
                                                        <div class="fieldRight"><asp:Label ID="lblWarehouseGrid" runat="server" Text="Seleccione Centro de Distribucion" /></div>
                                                        <div class="fieldLeft">
                                                            <asp:DropDownList runat="server" ID="ddlWarehouse" AutoPostBack="True" />                                                
                                                            <asp:RequiredFieldValidator ID="rfvWarehousesLoad" runat="server" ControlToValidate="ddlWarehouse"
                                                                InitialValue="-1" ValidationGroup="Load" Text=" * " ErrorMessage="Centro es requerido" />
                                                        </div>
                                                    </div>

                                                    <%-- Owner --%>
                                                    <div id="divOwner" runat="server" class="divControls">
                                                        <div class="fieldRight"><asp:Label ID="lblOwnerGrid" runat="server" Text="Seleccione Dueño" /></div>
                                                        <div class="fieldLeft">
                                                            <asp:DropDownList runat="server" ID="ddlOwnerLoad" OnSelectedIndexChanged="ddlOwnerLoad_SelectedIndexChanged" AutoPostBack="True" />                                                
                                                            <asp:RequiredFieldValidator ID="rfvOwnerLoad" runat="server" ControlToValidate="ddlOwnerLoad"
                                                                InitialValue="-1" ValidationGroup="Load" Text=" * " ErrorMessage="Dueño es requerido" />
                                                        </div>
                                                    </div>

                                                    <%-- Customer --%>                            
                                                    <div id="divCustomerCode" class="divControls">
                                                        <div class="fieldRight">
                                                            <asp:Label ID="lblCustomerCode" Text="Cód. Cliente" runat="server" />
                                                        </div>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox ID="txtCustomerCode" runat="server"  />
                                                            <asp:ImageButton ID="imgBtnCustmerSearch" 
                                                                runat="server" 
                                                                Height="18px" 
                                                                ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                                                OnClick="imgBtnCustmerSearch_Click" 
                                                                Width="18px" 
                                                                ToolTip="Buscar Cliente"/>
                                                            <asp:RequiredFieldValidator ID="rfvCustomerCode" ControlToValidate="txtCustomerCode" ValidationGroup="EditNew"
                                                                runat="server" Text=" * " ErrorMessage="Código Cliente es requerido"></asp:RequiredFieldValidator>
                                                        </div>
                                                    </div> 
                                                    <div id="divCustomerName" class="divControls">
                                                        <div class="fieldRight">
                                                            <asp:Label ID="lblCustomerName" Text="Nombre Cliente" runat="server" />
                                                        </div>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox ID="txtCustomerName" runat="server"  Enabled="false"  />                                                
                                                        </div>
                                                    </div>

                                                    <%-- Customer --%>
                                                    <%--<div id="divCustomer" runat="server" class="divControls">
                                                        <div class="fieldRight"><asp:Label ID="lblCustomer" runat="server" Text="Seleccione Cliente" /></div>
                                                        <div class="fieldLeft">
                                                            <asp:DropDownList runat="server" ID="ddlCustomerLoad" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="True">
                                                            </asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="rfvCustomerLoad" runat="server" ControlToValidate="ddlCustomerLoad"
                                                                InitialValue="-1" ValidationGroup="Load" Text=" * " ErrorMessage="Cliente es requerido" />
                                                        </div>
                                                    </div>--%>

                                                    <%-- Branch --%>
                                                    <div id="divBranch" class="divControls">
                                                        <div class="fieldRight">
                                                            <asp:Label ID="lblBranch" Text="Sucursal Entrega" runat="server" /></div>
                                                        <div class="fieldLeft">
                                                            <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="True"/>
                                                            <asp:RequiredFieldValidator ID="rfvBrachLoad" runat="server" ControlToValidate="ddlBranch"
                                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Branch es requerido" />
                                                        </div>
                                                    </div>

                                                    <%-- ItemCode --%>
                                                    <div id="divItemCode" runat="server" class="divControls">
                                                        <div class="fieldRight">
                                                            <asp:Label ID="lblItemCode" runat="server" Text="Cód. Item" />
                                                        </div>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox ID="txtItemCode" runat="server"  />
                                                            <asp:ImageButton ID="imgbtnSearchItem" runat="server" 
                                                                Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                                                OnClick="imgBtnSearchItem_Click" 
                                                                ToolTip="Buscar Item" Width="18px"  />
                                                            <asp:RequiredFieldValidator ID="rfvItemCode" runat="server" ControlToValidate="txtItemCode"
                                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Código Item es requerido" />
                                                        </div>
                                           
                                                    </div> 

                                                    <%-- DayQty --%>
                                                    <div id="divDayQty" runat="server" class="divControls">
                                                        <div class="fieldRight"><asp:Label ID="lblDayQtyGrid" runat="server" Text="Cantidad días" /></div>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox ID="txtDayQty" runat="server" MaxLength="20" Width="150" />
                                                            <asp:RequiredFieldValidator ID="rvDayQty" runat="server" ControlToValidate="txtDayQty"
                                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Cantidad es requerida" />
                                                            <asp:RegularExpressionValidator ID="revtxtDayQty" runat="server" ControlToValidate="txtDayQty"
                                                                ErrorMessage="cantidad permite ingresar solo números" 
                                                                ValidationExpression="[0-99999999999]*" ValidationGroup="EditNew" Text=" * ">
                                                            </asp:RegularExpressionValidator>    
                                                        </div>
                                                    </div>
                                                    <div class="divCtrsFloatLeft">
                                                        <div class="mainFilterPanelItem">
                                                            <asp:ValidationSummary ID="valEditNew" runat="server" ValidationGroup="EditNew" CssClass="modalValidation" />
                                                        </div>
                                                    </div>
                                                    <div style="clear: both">
                                                    </div>   
                                                    
                                            </ContentTemplate>    
                                        </ajaxToolkit:TabPanel>
                                    </ajaxToolkit:TabContainer>
                                    <div id="div6" runat="server" class="modalActions">
                                        <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancelar" OnClick="btnCloseNewEdit_Click"/>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                        <%-- FIN Pop up Editar/Nuevo Owner --%>

                        <%-- Lookup Items --%>
                        <div id="divLookupItem" runat="server" visible="false">
                            <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                            <!-- Boton 'dummy' para propiedad TargetControlID -->
                            <ajaxToolkit:ModalPopupExtender ID="mpLookupItem" runat="server" TargetControlID="btnDummy"
                                PopupControlID="pnlLookupItem" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupItem"
                                Drag="true">
                            </ajaxToolkit:ModalPopupExtender>
                            <asp:Panel ID="pnlLookupItem" runat="server" CssClass="modalBox">
                                <asp:Panel ID="pnlHeadBar2" runat="server" CssClass="modalHeader">
                                    <div class="divCaption">
                                        <asp:Label ID="lblAddItem" runat="server" Text="Buscar Item" />
                                        <asp:ImageButton ID="imgBtnCloseItemSearch" runat="server" ImageAlign="Top" CssClass="closeButton"
                                            ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                                    </div>
                                    <div id="divPageGrdSearchItems" runat="server">
                                        <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                            <asp:ImageButton ID="btnFirstGrdSearchItems" runat="server" OnClick="btnFirstGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                            <asp:ImageButton ID="btnPrevGrdSearchItems" runat="server" OnClick="btnPrevGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                                            Pág. 
                                            <asp:DropDownList ID="ddlPagesSearchItems" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchItemsSelectedIndexChanged" SkinID="ddlFilter" /> 
                                            de 
                                            <asp:Label ID="lblPageCountSearchItems" runat="server" Text="" />
                                            <asp:ImageButton ID="btnNextGrdSearchItems" runat="server" OnClick="btnNextGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                            <asp:ImageButton ID="btnLastGrdSearchItems" runat="server" OnClick="btnLastGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <div class="modalControls">
                                    <asp:HiddenField ID="hidItemId" runat="server" Value="-1" />
                                    <webUc:ucLookUpFilter ID="ucFilterItem" runat="server" />
                                    <div class="divCtrsFloatLeft">
                                        <div class="divLookupGrid">
                                            <asp:GridView ID="grdSearchItems" runat="server" DataKeyNames="Id" OnRowCommand="grdSearchItems_RowCommand" AllowPaging="true"
                                                AutoGenerateColumns="False"
                                                OnRowDataBound="grdSearchItems_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:BoundField AccessibleHeaderText="Id" DataField="Id" HeaderText="ID" InsertVisible="True"
                                                        SortExpression="Id" />
                                                    <asp:TemplateField AccessibleHeaderText="ItemCode" HeaderText="Cód.">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCode0" runat="server" Text='<%# Eval ("Code") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="Description" HeaderText="Item">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemName0" runat="server" Text='<%# Eval ("Description") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="imgBtnAddItem" ToolTip="Agregar Cliente" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                                    Width="20px" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div style="clear: both" />
                                </div>
                            </asp:Panel>
                        </div>
                        <%-- FIN Lookup Items --%>

                        <%-- Lookup Customers --%>
                        <div id="divLookupCustomer" runat="server" visible="false">
                            <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
                            <!-- Boton 'dummy' para propiedad TargetControlID -->
                            <ajaxToolkit:ModalPopupExtender ID="mpLookupCustomer" runat="server" TargetControlID="btnDummy2"
                                PopupControlID="pnlLookupCustomer" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupCustomer"
                                Drag="true">
                            </ajaxToolkit:ModalPopupExtender>
                            <asp:Panel ID="pnlLookupCustomer" runat="server" CssClass="modalBox">
                                <asp:Panel ID="pnlHeadBarCustomer" runat="server" CssClass="modalHeader">
                                    <div class="divCaption">
                                        <asp:Label ID="lblAddCustomer" runat="server" Text="Buscar Cliente" />
                                        <asp:ImageButton ID="imgBtnCloseCustomerSearch" runat="server" ImageAlign="Top" CssClass="closeButton"
                                            ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                                    </div>
                                    <div id="divPageGrdSearchCustomers" runat="server">
                                        <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                            <asp:ImageButton ID="btnFirstGrdSearchCustomers" runat="server" OnClick="btnFirstGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                            <asp:ImageButton ID="btnPrevGrdSearchCustomers" runat="server" OnClick="btnPrevGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                                            Pág. 
                                            <asp:DropDownList ID="ddlPagesSearchCustomers" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchCustomersSelectedIndexChanged" SkinID="ddlFilter" /> 
                                            de 
                                            <asp:Label ID="lblPageCountSearchCustomers" runat="server" Text="" />
                                            <asp:ImageButton ID="btnNextGrdSearchCustomers" runat="server" OnClick="btnNextGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                            <asp:ImageButton ID="btnLastGrdSearchCustomers" runat="server" OnClick="btnLastGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <div class="modalControls">
                                    <asp:HiddenField ID="hidCustomerId" runat="server" Value="-1" />
                                    <webUc:ucLookUpFilter ID="ucFilterCustomer" runat="server" />
                                    <div class="divCtrsFloatLeft">
                                        <div class="divLookupGrid">
                                            <asp:GridView ID="grdSearchCustomers" runat="server" DataKeyNames="Id" OnRowCommand="grdSearchCustomers_RowCommand" AllowPaging="true"
                                                AutoGenerateColumns="False"
                                                OnRowDataBound="grdSearchCustomers_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:BoundField AccessibleHeaderText="Id" DataField="Id" HeaderText="ID" InsertVisible="True"
                                                        SortExpression="Id" Visible="false" />
                                                    <asp:TemplateField AccessibleHeaderText="IdOwn" HeaderText="Id Dueño" Visible="false">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ("Owner.Id") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="OwnName" HeaderText="Dueño">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval ("Owner.Name") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CustomerCode" HeaderText="Código">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ("Code") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CustomerName" HeaderText="Cliente">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ("Name") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="imgBtnAddCustomer" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                                    Width="20px" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div style="clear: both" />
                                </div>
                            </asp:Panel>
                        </div>
                        <%-- FIN Lookup Customers --%>


                   </ContentTemplate>
                   <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />    
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
                  </Triggers>
                </asp:UpdatePanel>       
            </div>
        </div>
    </div> 

     <%-- Mensaje--%>
    <div id="divFondoPopup" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;">
    </div>
    <div id="divMensaje" class="modalBox" style="z-index: 400001; display: none; position: absolute; 
        width: 400px;  top: 200px; margin-top: 0;"  runat="server">
        
        <div id="divDialogTitleMessage" runat="server" class="modalHeader">
			<div class="divCaption">
			    <asp:Label ID="lblDialogTitle" runat="server" />
            </div>
	    </div>
	    <div id="divPanelMessage" class="divDialogPanel" runat="server">
        
            <div class="divDialogMessage">
                <asp:Image id="Image1" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
            </div>
            <div id="divDialogMessage" runat="server" class="divDialogMessage" />        
            <div id="divAlert" runat="server" visible="true" class="divDialogButtons">
                <asp:Button ID="btnMessageInfo" runat="server" Text="Aceptar"  OnClientClick="return HideMessage();" />
            </div>    
        </div>               
    </div>
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar?" Visible="false" />
    <asp:Label ID="lbltabGeneralNuevo" runat="server" Text="Nueva Configuración" Visible="false" />    
    <asp:Label ID="lbltabGeneralModificar" runat="server" Text="Modificar Dias" Visible="false" />    
    <asp:Label ID="lblTitleCodigoCustomer" runat="server" Text="Código Cliente" Visible="false" />  
    <asp:Label ID="lblFilterCode" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Items Clientes" Visible="false" />
    <asp:Label ID="lblMessajeSelectedOwner" runat="server" Text="Debe seleccionar un Dueño." Visible="false" />
    <%--<asp:Label ID="lbltabDocSalida" runat="server" Text="Correlativo Salida" Visible="false" />  
    <asp:Label ID="lblExisteCorrelativo" runat="server" Text="Correlativo existente" Visible="false" /> --%>
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- FIN Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
