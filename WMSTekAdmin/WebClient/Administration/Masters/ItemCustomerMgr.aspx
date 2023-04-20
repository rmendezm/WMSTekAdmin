<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="ItemCustomerMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.ItemCustomerMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">
    //window.onresize = SetDivs;
    //var prm = Sys.WebForms.PageRequestManager.getInstance();
    //prm.add_endRequest(SetDivs);

    function HideMessage() {
        document.getElementById("divFondoPopup").style.display = 'none';
        document.getElementById("ctl00_MainContent_divMensaje").style.display = 'none';

        return false;
    }

    function ShowMessage(title, message) {
        var position = (document.body.clientWidth - 400) / 2 + "px";
        document.getElementById("divFondoPopup").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.marginLeft = position;

        document.getElementById("ctl00_MainContent_lblDialogTitle").innerHTML = title;
        document.getElementById("ctl00_MainContent_divDialogMessage").innerHTML = message;

        return false;
    }

    function showProgress() {
        if (document.getElementById('ctl00_MainContent_uploadFile').value.length > 0) {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modalLoading");
                $('body').append(modal);
       
                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 3.5, 0);
                var left = Math.max($(window).width() / 2.6, 0);
                loading.css({ top: top, left: left });
            }, 10);
            return true;

        } else {
            return false;
        }
    }


    $(document).ready(function () {
        initializeGridDragAndDrop("ItemCustomer_FindAll", "ctl00_MainContent_grdMgr");
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
        initializeGridDragAndDrop("ItemCustomer_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop(true);
    }

    function setDivsAfter() {
        var heightDiv = $("#ctl00_MainContent_divGrid").height();
        var totalHeight = heightDiv - 25;
        $("#ctl00_MainContent_divGrid").css("max-height", totalHeight + "px");
    }
           
</script>
   
<style>

    #ctl00_MainContent_pnlPanelPoUp{
        max-height: 400px !important;
    }

    .divLookupGrid{
        overflow: visible;
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

                <asp:UpdatePanel ID="upGrid" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                    <%-- Grilla Principal --%>
                    <div id="divGrid" runat="server" visible="true" class="divGrid" >  
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
                                    <asp:TemplateField HeaderText="Id Cliente" AccessibleHeaderText="CustomerId">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCustomerId" runat="server" Text='<%# Eval ( "Customer.Id" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Código Cliente" AccessibleHeaderText="CustomerCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ( "Customer.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "Customer.Name" ) %>' />
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
                        
                                    <asp:TemplateField HeaderText="Código Item Cliente" AccessibleHeaderText="ItemCodeCustomer">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblItemCodeCustomer" runat="server" Text='<%# Eval ( "ItemCodeCustomer" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>                        
                                    <asp:TemplateField HeaderText="Nombre Item Cliente" AccessibleHeaderText="LongItemName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ( "LongItemName" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Capacidad" AccessibleHeaderText="Capacity">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCapacity" runat="server" Text='<%# ((decimal)Eval("Capacity")== -1) ? " ":Eval("Capacity")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Precio" AccessibleHeaderText="Price">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblPrice" runat="server" Text='<%# ((decimal)Eval("Price")== -1) ? " ":Eval("Price")%>'></asp:Label>
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
                                    <asp:TemplateField HeaderText="Item Departamento" AccessibleHeaderText="DepartmentItem">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblDepartmentItem" runat="server" Text='<%# Eval ( "DepartmentItem" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Descripción Departamento" AccessibleHeaderText="DepartmentDescription">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblDepartmentDescription" runat="server" Text='<%# Eval ( "DepartmentDescription" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Código Barra" AccessibleHeaderText="BarCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblBarCode" runat="server" Text='<%# Eval ( "BarCode" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                        </asp:GridView>
                    </div>
        
        
                    <%-- Panel Nuevo/Editar Documento --%>
                    <div id="divModal" runat="server" visible="false">
                        <asp:Panel ID="pnlPanelPoUp" runat="server" CssClass="modalBox">
                            <%-- Encabezado --%>
                            <asp:Panel ID="OutboundCaption" runat="server" CssClass="modalHeader">
                                <div class="divCaption">
                                    <asp:Label ID="lblNew" runat="server" Text="Nuevo Item Cliente" Width="770px" />
                                    <asp:Label ID="lblEdit" runat="server" Text="Editar Item Cliente" Width="770px" />
                                    <asp:ImageButton ID="ImageButton1" runat="server" OnClick="imgCloseNewEdit_Click" ToolTip="Cerrar" CssClass="closeButton"
                                        ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                                </div>
                            </asp:Panel>
                            <%-- Fin Encabezado --%>
                    
                            <div class="modalBoxContent">
                                <ajaxToolkit:TabContainer runat="server" ID="tabItemCustomer" ActiveTabIndex="0">
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
                                        
                                                 <%-- ItemCodeCustomer --%>
                                                <div id="divItemCodeCustomer" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblItemCodeCustomer" runat="server" Text="Código"></asp:Label>
                                                    </div>
                                                    <div class="fieldLeft">
                                                        <asp:TextBox ID="txtItemCodeCustomer" runat="server" Text="" Enabled="true" MaxLength="30"/>  
                                                        <asp:RequiredFieldValidator ID="rfvItemCodeCustomer" runat="server" ControlToValidate="txtItemCodeCustomer"
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
                                        
                                                <%-- Capacity --%>
                                                <div id="divCapacity" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblCapacity" runat="server" Text="Capacidad"></asp:Label>
                                                    </div>
                                                    <div class="fieldLeft">
                                                        <asp:TextBox ID="txtCapacity" runat="server" Text="" Enabled="true"/>   
                                                        <asp:RequiredFieldValidator ID="rfvCapacity" runat="server" ControlToValidate="txtCapacity"
                                                         ValidationGroup="EditNew" Text=" * " ErrorMessage="Capacidad es requerido" />                        
                                                    </div>
                                                </div> 
                                        
                                                 <%-- Price --%>
                                                <div id="divPrice" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblPrice" runat="server" Text="Precio"></asp:Label>
                                                    </div>
                                                    <div class="fieldLeft">
                                                        <asp:TextBox ID="txtPrice" runat="server" Text="" Enabled="true"/>    
                                                        <asp:RequiredFieldValidator ID="rfvPrice" runat="server" ControlToValidate="txtPrice"
                                                         ValidationGroup="EditNew" Text=" * " ErrorMessage="Precio es requerido" />                       
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

                                                <%-- DepartmentItem --%>
                                                <div id="divDepartmentItem" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblDepartmentItem" runat="server" Text="Item Departamento"></asp:Label>
                                                    </div>
                                                    <div class="fieldLeft">
                                                        <asp:TextBox ID="txtDepartmentItem" runat="server" Text="" Enabled="true" MaxLength="50"/>                   
                                                    </div>
                                                </div>

                                                <%-- DepartmentDescription --%>
                                                <div id="divDepartmentDescription" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblDepartmentDescription" runat="server" Text="Descripción Departamento"></asp:Label>
                                                    </div>
                                                    <div class="fieldLeft">
                                                        <asp:TextBox ID="txtDepartmentDescription" runat="server" Text="" Enabled="true" MaxLength="150"/>                   
                                                    </div>
                                                </div>
                                        
                                                <%-- BarCode --%>
                                                <div id="divBarCode" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblBarCode" runat="server" Text="Código Barra"></asp:Label>
                                                    </div>
                                                    <div class="fieldLeft">
                                                        <asp:TextBox ID="txtBarCode" runat="server" Text="" Enabled="true" MaxLength="30"/>                   
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
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Grilla Principal --%>
    


    <%-- Carga masiva de itemsCustomers --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoad" runat="server" visible="false">
                <asp:Button ID="btnDummyLoad" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpLoad" runat="server" 
                    TargetControlID="btnDummyLoad"
                    PopupControlID="panelLoad" 
                    BackgroundCssClass="modalBackground" 
                    PopupDragHandleControlID="panelCaptionLoad"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelLoad" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionLoad" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label3" runat="server" Text="Carga Masiva de Items Clientes" />
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Item%20Cliente.xlsx" 
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

                             <%-- Customer --%>                            
                            <div id="divCustomerCodeLoad " class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCustomerCodeLoad" Text="Cód. Cliente" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCustomerCodeLoad" runat="server"  />
                                    <asp:ImageButton ID="imgBtnCustmerSearchLoad" 
                                        runat="server" 
                                        Height="18px" 
                                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                        OnClick="imgBtnCustmerSearchLoad_Click" 
                                        Width="18px" 
                                        ToolTip="Buscar Cliente"/>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtCustomerCodeLoad" ValidationGroup="Load"
                                        runat="server" Text=" * " ErrorMessage="Código Cliente es requerido"></asp:RequiredFieldValidator>
                                </div>
                            </div> 
                            <div id="divCustomerNameLoad" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCustomerNameLoad" Text="Nombre Cliente" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCustomerNameLoad" runat="server"  Enabled="false"  />       
                                    <asp:RequiredFieldValidator ID="rfvCustomerNameLoad" ControlToValidate="txtCustomerNameLoad" ValidationGroup="Load"
                                        runat="server" Text=" * " ErrorMessage="Nombre Cliente es requerido"></asp:RequiredFieldValidator>                                         
                                </div>
                            </div>
                        
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label5" runat="server" Text="Seleccione Archivo" />
                                </div>
                                <div class="fieldLeft">                      
                                    <asp:FileUpload ID="uploadFile" runat="server" Width="400px" ValidationGroup="Load"/>         
                                                     
                                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo"
                                    ValidationGroup="Load" ControlToValidate="uploadFile"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator id="revUpload" runat="server"  ValidationGroup="Load"
                                    ErrorMessage="Cargar solo archivos .xls ó .xlsx"  Text="*"
                                    ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$" 
                                    ControlToValidate="uploadFile">
                                    </asp:RegularExpressionValidator>                             
                                    
                                </div>  
                            </div>
                            <div class="divControls" >
                                <div class="fieldRight">
                                    <asp:Label ID="Label6" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnLoadFile" runat="server" Text="Cargar Archivo" ValidationGroup="Load" 
                                     onclick="btnLoadFile_Click" OnClientClick="showProgress();" />
                                </div>
                            </div>
                           <%-- <div style="clear: both">
                            </div>--%>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load" ShowMessageBox="false" CssClass="modalValidation" />
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
            <asp:PostBackTrigger ControlID="btnLoadFile" />
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnCloseCustomerLoad" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="grdSearchCustomersLoad" EventName="RowCommand" />
            
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprLoad" runat="server" AssociatedUpdatePanelID="upLoad" DisplayAfter="20" DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprLoad" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLoad" />


    <asp:UpdatePanel ID="UpdatePanel1" runat="server"  UpdateMode="Always">
        <ContentTemplate>

    <%-- Lookup Customers --%>
            <div id="divLookupCustomerLoad" runat="server" visible="false">
                <asp:Button ID="btnDummyLoadCustomer" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpLookupCustomerLoad" runat="server" TargetControlID="btnDummyLoadCustomer" 
                    PopupControlID="pnlLookupCustomerLoad" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupCustomerLoad"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlLookupCustomerLoad" runat="server" CssClass="modalBox">
                    <asp:Panel ID="pnlHeadBarCustomerLoad" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label1" runat="server" Text="Buscar Cliente" />
                            <asp:ImageButton ID="btnCloseCustomerLoad" runat="server" ImageAlign="Top" CssClass="closeButton"
                                ImageUrl="~/WebResources/Images/Buttons/icon_close.png" OnClick="btnCloseCustomerLoad_Click" />
                        </div>
                        <div id="divPageGrdSearchCustomersLoad" runat="server">
                            <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                <asp:ImageButton ID="btnFirstGrdSearchCustomersLoad" runat="server" OnClick="btnFirstGrdSearchCustomersLoad_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                <asp:ImageButton ID="btnPrevGrdSearchCustomersLoad" runat="server" OnClick="btnPrevGrdSearchCustomersLoad_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                                Pág. 
                                <asp:DropDownList ID="ddlPagesSearchCustomersLoad" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchCustomersSelectedIndexChangedLoad" SkinID="ddlFilter" /> 
                                de 
                                <asp:Label ID="lblPageCountSearchCustomersLoad" runat="server" Text="" />
                                <asp:ImageButton ID="btnNextGrdSearchCustomersLoad" runat="server" OnClick="btnNextGrdSearchCustomersLoad_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                <asp:ImageButton ID="btnLastGrdSearchCustomersLoad" runat="server" OnClick="btnLastGrdSearchCustomersLoad_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidCustomerIdLoad" runat="server" Value="-1" />
                        <webUc:ucLookUpFilter ID="ucFilterCustomerLoad" runat="server" />
                        <div class="divCtrsFloatLeft">
                            <div class="divLookupGrid">
                                <asp:GridView ID="grdSearchCustomersLoad" runat="server" DataKeyNames="Id" OnRowCommand="grdSearchCustomersLoad_RowCommand" AllowPaging="true"
                                    AutoGenerateColumns="False"
                                    OnRowDataBound="grdSearchCustomersLoad_RowDataBound"
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
        <%--<Triggers>
            <asp:PostBackTrigger ControlID="btnSubir" />
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
        </Triggers>--%>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upLoad" DisplayAfter="20" DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoadNew" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="UpdateProgressOverlayExtender1" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="UpdateProgress1" />
    
    
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

     <%-- Mensajes de advertencia y auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Item Cliente?" Visible="false" />  
    <asp:Label ID="lbltabGeneral" runat="server" Text="Datos Generales" Visible="false" />
    <asp:Label ID="lblCustomer" runat="server" Text="Cod. Cliente" Visible="false" /> 
    <asp:Label ID="lblCodeItemCustomer" runat="server" Text="Cod. Item Cliente" Visible="false" /> 
    <asp:Label ID="lblFilterCode" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Items Clientes" Visible="false" />
    <asp:Label ID="lblMessajeSelectedOwner" runat="server" Text="Debe seleccionar un Dueño." Visible="false" />

    <%-- Mensajes de advertencia y error Para Carga Masiva --%>
    <asp:Label ID="lblNotFileLoad" runat="server" Text="Seleccione el archivo que desea cargar." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen items en el archivo." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es valído." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblNotAccessServerFolder" runat="server" Text="No existe acceso al servidor." Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />


    <%-- Div Bloquea Pantalla al Momento de Realizar Carga Masiva --%>
    <div id="divFondoPopupProgress" class="loading" align="center" style="display: none;">
        Realizando Carga Masiva <br />Espere un momento...<br />
        <br />
        <img src="../../WebResources/Images/Buttons/ajax-loader.gif" alt="" />
    </div>
    

</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
  <webUc:ucStatus id="ucStatus" runat="server"/>
</asp:Content>
