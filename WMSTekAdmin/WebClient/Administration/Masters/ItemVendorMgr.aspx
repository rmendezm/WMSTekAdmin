<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="ItemVendorMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.ItemVendorMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script src="<%= Page.ResolveClientUrl("~/WebResources/Javascript/UtilMassive.js")%>"></script>
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("ItemVendor_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop(true);

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
        initializeGridDragAndDrop("ItemVendor_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop(true);
    }
    
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">

    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <%-- Grilla Principal --%>
        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >  
            <asp:GridView ID="grdMgr" runat="server" 
                    OnRowCreated="grdMgr_RowCreated" 
                    OnRowDeleting="grdMgr_RowDeleting" 
                    OnRowEditing="grdMgr_RowEditing" 
                    AllowPaging="True" 
                    EnableViewState="False" 
                    AutoGenerateColumns="False"
                    OnRowDataBound="grdMgr_RowDataBound"
                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                    EnableTheming="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                            <ItemTemplate>
                                <center>
                                    <div style="width: 60px">
                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                            CommandName="Edit" ToolTip="Editar" />
                                        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                            CommandName="Delete" ToolTip="Eliminar" />
                                    </div>
                                </center>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Id Proveedor" AccessibleHeaderText="VendorId">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblVendorId" runat="server" Text='<%# Eval ( "Vendor.Id" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código Proveedor" AccessibleHeaderText="VendorCode">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblVendorCode" runat="server" Text='<%# Eval ( "Vendor.Code" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nombre Proveedor" AccessibleHeaderText="VendorName">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval ( "Vendor.Name" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="OwnerId">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblOwnerId" runat="server" Text='<%# Eval ( "Owner.Id" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código Dueño" AccessibleHeaderText="OwnerCode">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nombre Dueño" AccessibleHeaderText="OwnerName">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="ItemId">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblItemId" runat="server" Text='<%# Eval ( "Item.Id" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código Item" AccessibleHeaderText="ItemCode">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "Item.Code" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nombre Item" AccessibleHeaderText="ShortName">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblShortName" runat="server" Text='<%# Eval ( "Item.ShortName" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Código Item Proveedor" AccessibleHeaderText="ItemCodeVendor">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblItemCodeVendor" runat="server" Text='<%# Eval ( "ItemCodeVendor" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>                        
                        <asp:TemplateField HeaderText="Nombre Item Proveedor" AccessibleHeaderText="LongItemName">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ( "LongItemName" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                            <ItemTemplate>
                                <center>
                                    <div style="word-wrap: break-word;">
                                        <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="false" />
                                    </div>
                                </center>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Campo Especial 1" AccessibleHeaderText="SpecialField1">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Eval ( "SpecialField1" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Campo Especial 2" AccessibleHeaderText="SpecialField2">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblSpecialField2" runat="server" Text='<%# Eval ( "SpecialField2" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Campo Especial 3" AccessibleHeaderText="SpecialField3">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblSpecialField3" runat="server" Text='<%# Eval ( "SpecialField3" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Campo Especial 4" AccessibleHeaderText="SpecialField4">
                            <ItemTemplate>
                                <div style="word-wrap: break-word;">
                                    <asp:Label ID="lblSpecialField4" runat="server" Text='<%# Eval ( "SpecialField4" ) %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                    </Columns>
            </asp:GridView>
        </div>
        
        
        <%-- Panel Nuevo/Editar ItemVendor --%>
            <div id="divModal" runat="server" visible="false">
                <asp:Panel ID="pnlPanelPoUp" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="OutboundCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Item Proveedor" Width="770px" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Item Proveedor" Width="770px" />
                            <asp:ImageButton ID="ImageButton1" runat="server" OnClick="imgCloseNewEdit_Click" ToolTip="Cerrar" CssClass="closeButton"
                                ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    
                    <div class="modalBoxContent">
                        <ajaxToolkit:TabContainer runat="server" ID="tabItemVendor" ActiveTabIndex="0">
                            <ajaxToolkit:TabPanel runat="server" ID="tabLayout">
                                <ContentTemplate>
                                    <div id="Central" class="divCtrsFloatLeft">
                                        
                                        <%-- Owner --%>                            
                                        <div id="divOwner" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                            </div>
                                            <div class="fieldLeft"> 
                                                <asp:DropDownList runat="server" ID="ddlOwner"></asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwner"
                                                  InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Owner es requerido" />                                                              
                                            </div>
                                        </div>  
                                         
                                        <%-- Vendor --%>                            
                                        <div id="divVendorCode" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblVendorCode" Text="Cód. Proveedor" runat="server" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtVendorCode" runat="server"  />
                                                <asp:ImageButton ID="imgBtnCustmerSearch" 
                                                    runat="server" 
                                                    Height="18px" 
                                                    ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                                    OnClick="imgBtnVendorSearch_Click" 
                                                    Width="18px" 
                                                    ToolTip="Buscar Proveedor"/>
                                                <asp:RequiredFieldValidator ID="rfvVendorCode" ControlToValidate="txtVendorCode" ValidationGroup="EditNew"
                                                    runat="server" Text=" * " ErrorMessage="Código Proveedor es requerido"></asp:RequiredFieldValidator>
                                            </div>
                                        </div> 
                                        <div id="divVendorName" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblVendorName" Text="Nombre Proveedor" runat="server" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtVendorName" runat="server"  Enabled="false"  />                                                
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
                                        
                                        <div id="divItemName" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblItemName" Text="Nombre Item" runat="server" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtItemName" runat="server" Enabled="false" />
                                            </div>
                                        </div>                    
                                        
                                         <%-- ItemCodeVendor --%>
                                        <div id="div4" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblItemCodeVendor" runat="server" Text="Código"></asp:Label>
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtItemCodeVendor" runat="server" Text="" Enabled="true" MaxLength="30"/>  
                                                <asp:RequiredFieldValidator ID="rfvItemCodeVendor" runat="server" ControlToValidate="txtItemCodeVendor"
                                                 ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />                         
                                            </div>
                                        </div>  
                                        
                                        <%-- LongItemName --%>
                                        <div id="divLongItemName" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLongItemName" runat="server" Text="Nombre"></asp:Label>
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtLongItemName" runat="server" Text="" Enabled="true" MaxLength="60"/>  
                                                <asp:RequiredFieldValidator ID="rfvLongItemName" runat="server" ControlToValidate="txtLongItemName"
                                                 ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" />                         
                                            </div>
                                        </div> 
                                                                                
                                        <%-- Status --%>
                                        <div id="divStatus" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="Status" Text="Activo" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="true" TabIndex="10" />
                                            </div>
                                        </div> 
                                        
                                        <%-- SpecialField1 --%>
                                        <div id="divSpecialField1" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblSpecialField1" runat="server" Text="SpecialField1"></asp:Label>
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtSpecialField1" runat="server" Text="" Enabled="true" MaxLength="20"/>    
                                                                
                                            </div>
                                        </div>
                                        
                                        <%-- SpecialField2 --%>
                                        <div id="divSpecialField2" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblSpecialField2" runat="server" Text="SpecialField2"></asp:Label>
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtSpecialField2" runat="server" Text="" Enabled="true" MaxLength="20"/>  
                                                                                
                                            </div>
                                        </div>
                                        
                                        <%-- SpecialField3 --%>
                                        <div id="divSpecialField3" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblSpecialField3" runat="server" Text="SpecialField3"></asp:Label>
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtSpecialField3" runat="server" Text="" Enabled="true" MaxLength="20"/>    
                                                                        
                                            </div>
                                        </div>
                                        
                                        <%-- SpecialField4 --%>
                                        <div id="divSpecialField4" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblSpecialField4" runat="server" Text="SpecialField4"></asp:Label>
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtSpecialField4" runat="server" Text="" Enabled="true" MaxLength="20"/>  
                                                                                
                                            </div>
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
                        
                        <div id="divActions" runat="server" class="modalActions" >
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" OnClick="btnCloseNewEdit_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        
        
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
                                    onrowdatabound="grdSearchItems_RowDataBound"
                                    AutoGenerateColumns="False"
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
                                                    <asp:ImageButton ID="imgBtnAddItem" ToolTip="Agregar Proveedor" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
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
        
            <%-- Lookup Vendors --%>
            <div id="divLookupVendor" runat="server" visible="false">
                <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpLookupVendor" runat="server" TargetControlID="btnDummy2"
                    PopupControlID="pnlLookupVendor" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupVendor"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlLookupVendor" runat="server" CssClass="modalBox">
                    <asp:Panel ID="pnlHeadBarVendor" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblAddVendor" runat="server" Text="Buscar Proveedor" />
                            <asp:ImageButton ID="imgBtnCloseVendorSearch" runat="server" ImageAlign="Top" CssClass="closeButton"
                                ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                        <div id="divPageGrdSearchVendors" runat="server">
                            <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                <asp:ImageButton ID="btnFirstGrdSearchVendors" runat="server" OnClick="btnFirstGrdSearchVendors_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                <asp:ImageButton ID="btnPrevGrdSearchVendors" runat="server" OnClick="btnPrevGrdSearchVendors_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                                Pág. 
                                <asp:DropDownList ID="ddlPagesSearchVendors" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchVendorsSelectedIndexChanged" SkinID="ddlFilter" /> 
                                de 
                                <asp:Label ID="lblPageCountSearchVendors" runat="server" Text="" />
                                <asp:ImageButton ID="btnNextGrdSearchVendors" runat="server" OnClick="btnNextGrdSearchVendors_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                <asp:ImageButton ID="btnLastGrdSearchVendors" runat="server" OnClick="btnLastGrdSearchVendors_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidVendorId" runat="server" Value="-1" />
                        <webUc:ucLookUpFilter ID="ucFilterVendor" runat="server" />
                        <div class="divCtrsFloatLeft">
                            <div class="divLookupGrid">
                                <asp:GridView ID="grdSearchVendors" runat="server" DataKeyNames="Id" OnRowCommand="grdSearchVendors_RowCommand" AllowPaging="true"
                                    onrowdatabound="grdSearchVendors_RowDataBound"
                                    AutoGenerateColumns="False"
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
                                        <asp:TemplateField AccessibleHeaderText="VendorCode" HeaderText="Código">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblVendorCode" runat="server" Text='<%# Eval ("Code") %>' />
                                               </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="VendorName" HeaderText="Proveedor">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval ("Name") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="imgBtnAddVendor" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
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
            <%-- FIN Lookup Vendors --%>
        
        
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
            <%--<asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />--%>
        </Triggers>    
    </asp:UpdatePanel>

                </div>
            </div>
        </div>

    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Grilla Principal --%>

    <%-- Carga masiva de items --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoad" runat="server" visible="false">
                <asp:Button ID="btnDummyLoad" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpLoad" runat="server" TargetControlID="btnDummyLoad"
                    PopupControlID="panelLoad" BackgroundCssClass="modalBackground" PopupDragHandleControlID="panelCaptionLoad"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelLoad" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionLoad" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label3" runat="server" Text="Carga Masiva de Items" />
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Item%20Proveedor.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />
                            <asp:ImageButton ID="ImageButton2" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                            <div id="div2" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label4" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlOwnerLoad" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvOwnerLoad" runat="server" ControlToValidate="ddlOwnerLoad"
                                        InitialValue="-1" ValidationGroup="Load" Text=" * " ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>
                        
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label5" runat="server" Text="Seleccione Archivo" />
                                </div>
                                <div class="fieldLeft">                      
                                    <asp:FileUpload ID="uploadFile2" runat="server" Width="400px" ValidationGroup="Load"/>                                    
                                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo"
                                    ValidationGroup="Load" ControlToValidate="uploadFile2"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator id="revUpload" runat="server"  ValidationGroup="Load"
                                    ErrorMessage="Cargar solo archivos .xls ó .xlsx"  Text="*"
                                    ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$" 
                                    ControlToValidate="uploadFile2">
                                    </asp:RegularExpressionValidator>                             
                                    
                                </div>  
                            </div>
                            <div class="divControls" >
                                <div class="fieldRight">
                                    <asp:Label ID="Label6" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnSubir2" runat="server" Text="Cargar Archivo" ValidationGroup="Load" 
                                    OnClientClick="showProgress()" onclick="btnSubir2_Click" />
                                </div>
                            </div>
                           <%-- <div style="clear: both">
                            </div>--%>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load"
                                ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div1" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo --%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubir2" />
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprLoad" runat="server" AssociatedUpdatePanelID="upLoad" DisplayAfter="20" 
     DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprLoad" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLoad" />
    
    <div id="divFondoPopupProgress" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;" runat="server">
        <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoadNew" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
    </div>

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
    
     <%-- Mensajes de advertencia y auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Item Proveedor?" Visible="false" />  
    <asp:Label ID="lbltabGeneral" runat="server" Text="Datos Generales" Visible="false" />
    <asp:Label ID="lblVendor" runat="server" Text="Cod. Proveedor" Visible="false" /> 
    <asp:Label ID="lblCodeItemVendor" runat="server" Text="Cod. Item Proveedor" Visible="false" /> 
    <asp:Label ID="lblFilterCode" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Items Proveedores" Visible="false" />
    <asp:Label ID="lblMessajeSelectedOwner" runat="server" Text="Debe seleccionar un Dueño." Visible="false" />
    <asp:Label ID="lblAddLoadToolTip" runat="server" Text="Carga Masiva" Visible="false" />   
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es válido." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblNotAccessServerFolder" runat="server" Text="No existe acceso al servidor." Visible="false" />
    <asp:Label ID="lblNotFileLoad" runat="server" Text="Seleccione el archivo que desea cargar." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen items en el archivo." Visible="false" />
    <asp:Label ID="lblFieldInvalid" runat="server" Text="Formato del campo no es válido." Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
  <webUc:ucStatus id="ucStatus" runat="server"/>
</asp:Content>
