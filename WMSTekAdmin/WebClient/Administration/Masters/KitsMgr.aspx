<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="KitsMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.KitsMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>
<%--<%@ Register TagPrefix="webUcItem" TagName="ucLookUpFilterItem" Src="~/Shared/LookUpFilterContent.ascx" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script src="<%= Page.ResolveClientUrl("~/WebResources/Javascript/UtilMassive.js")%>"></script>
<script type="text/javascript" language="javascript">

    window.onresize = resizeDivPrincipal; 
    function resizeDivPrincipal() {
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divMainPrincipal").style.height = h;
        document.getElementById("ctl00_MainContent_divMainPrincipal").style.width = w;
    }

    $(function () {
        var gridHeader = 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr';
        if ($("#" + gridHeader).length > 0) {
            initializeGridDragAndDrop('Kits_GetByOwner', gridHeader);
        }
        initializeGridWithNoDragAndDrop(true);
    });

    function clearFilterDetail(gridDetail) {
        if ($("#" + gridDetail).length == 0) {
            if ($("div.container").length == 2) {
                //$("div.container:last div.row:first").remove();
                //$("div.container:last div.row-height-filter").remove();
            }
        }
    }

    function initializeGridDragAndDropCustom() {
        var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
        clearFilterDetail(gridDetail);
        initializeGridDragAndDrop('Kits_GetDetail', gridDetail);
        initializeGridWithNoDragAndDrop(true);
    }
</script>

    <style>
        .divItemDetails {
	        width: 100% !important;
        }

        .ob_spl_bottompanelcontent .froze-header-grid {
            height: 155px !important;
        }
    </style>

    <div id="divMainPrincipal" runat="server" style="width: 100%; height: 100%; margin: 0px; margin-bottom: 80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server"
            StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="60">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Always">
                                    <ContentTemplate>
  
                                        <div class="divGridTitle">
                                            <asp:Label ID="lblItemsGridTitle" runat="server" Text="Kits" />
                                        </div>
                            
                                        <%-- Panel Agregar/Modificar ItemKit (Kit) --%>
                                        <div id="Div2" runat="server" visible="true" class="divGridTitle">
                                            <asp:Label ID="lblGridKit" runat="server" Text="" />
                                        </div>
                            
                                        <%--Owner--%>
                                        <div class="mainFilterPanelItem">
                                            <asp:Label ID="lblOwnerItemKit" runat="server" Text="Due&ntilde;o" /> <br />           
                                            <asp:DropDownList ID="ddlOwnerItemKit" runat="server" Width="100px" Height="21px" AutoPostBack="true"
                                             OnSelectedIndexChanged="ddlOwnerItemKit_SelectedIndexChanged"></asp:DropDownList>                       
                                        </div>
                            
                                        <%--Codigo--%>
                                        <div class="mainFilterPanelItem">
                                            <asp:Label ID="lblCodeKit" runat="server" Text="Item" /><br />
                                            <asp:TextBox ID="txtCodeKit" runat="server" Width="100px" />
                                        <%--</div>--%>
                                        <%--Boton Buscar--%>
                                        <%--<div class="mainFilterPanelItem">--%>
                                            <asp:ImageButton ID="imgbtnSearchItemKit" runat="server" Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                                OnClick="imgBtnSearchItemKit_Click" Width="18px" ValidationGroup="searchItemKit" ToolTip="Buscar Item" />
                                        </div>
                                        <%--Descripcion--%>
                                        <div class="mainFilterPanelItem">
                                            <asp:Label ID="lblDescriptionKit" runat="server" Text="Descripci&oacute;n" /><br />
                                            <asp:TextBox ID="txtDescriptionKit" runat="server" Width="150px" MaxLength="30" Enabled="False"
                                                ReadOnly="True" />
                                            <asp:RequiredFieldValidator ID="rfvDescriptionKit" runat="server" ControlToValidate="txtDescriptionKit"
                                                ValidationGroup="AddItemKit" Text=" * " ErrorMessage="Descripci&oacute;n es requerido." />
                                        </div>
                                        <%--Boton Agregar ItemKit--%>
                                        <div class="mainFilterPanelItem">
                                            <br />
                                            <asp:ImageButton ID="imgBtnAddItemKit" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_add_item.png"
                                                OnClick="imgBtnAddItemKit_Click" ValidationGroup="AddItemKit" ToolTip="Agregar Item" />
                                        </div>
                                        <%--Panel de error--%>
                                        <asp:Panel ID="pnlErrorKit" runat="server" Visible="false">
                                            <asp:Label ID="lblErrorKit" runat="server" ForeColor="Red" Text="El item ya fue ingresado" CssClass="mainFilterPanelItem"/>
                                        </asp:Panel>            
                                        <%--Panel de resumen de validacion--%>
                                        <div class="mainFilterPanelItem">
                                            <asp:ValidationSummary ID="valAddItemKit" runat="server" ValidationGroup="AddItemKit" />
                                            <asp:ValidationSummary ID="valSearchItemKit" runat="server" ValidationGroup="searchItemKit" />
                                            <asp:Label ID="Label2" Visible="false" runat="server" Text="Debe Seleccionar un Dueño" ForeColor="Red" />
                                        </div>
                                        <%-- FIN Panel Agregar/Modificar ItemKit (Kit) --%>                        
                            
                                        <div class="container">
                                            <div class="row">
                                                <div class="col-md-12">    
                                                    <%-- Grilla principal de Kit --%>
                                                    <div id="divGrid" runat="server" class="divGrid" style="padding: 6px;" onresize="SetDivs();" >
                                                        <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id" 
                                                            OnRowCreated="grdMgr_RowCreated"
                                                            OnRowDeleting="grdMgr_RowDeleting"
                                                            OnRowDataBound="grdMgr_RowDataBound"
                                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                                            AllowPaging="True" EnableViewState="False" AllowSorting="False" 
                                                            AutoGenerateColumns="false"
                                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                            EnableTheming="false">
                                                            <Columns>
                                                                <asp:BoundField DataField="Id" HeaderText="Id Item" AccessibleHeaderText="Id" />
                                                                <asp:TemplateField HeaderText="C&oacute;digo Kit" AccessibleHeaderText="ItemCode">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label runat="server" ID="lblItemCode" Text='<%# Bind("ItemKit.Code") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Descripci&oacute;n" AccessibleHeaderText="Description">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label runat="server" ID="lblDescription" Text='<%# Bind("ItemKit.Description") %>' />
                                                                       </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Nombre Largo Kit" AccessibleHeaderText="LongName">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label runat="server" ID="lblItemLongName" Text='<%# Bind("ItemKit.LongName") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>     
                                                                <asp:TemplateField HeaderText="C&oacute;digo Due&ntilde;o" AccessibleHeaderText="OwnCode">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label runat="server" ID="lblItemOwnCode" Text='<%# Bind("ItemKit.Owner.Code") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>     
                                                                <asp:TemplateField HeaderText="Due&ntilde;o" AccessibleHeaderText="Owner">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label runat="server" ID="lblItemOwnName" Text='<%# Bind("ItemKit.Owner.Name") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Sector" AccessibleHeaderText="GrpItem1Name">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label runat="server" ID="lblGroupItem1" Text='<%# Bind("ItemKit.GrpItem1.Name") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Rubro" AccessibleHeaderText="GrpItem2Name">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label runat="server" ID="lblGroupItem2" Text='<%# Bind("ItemKit.GrpItem2.Name") %>' />
                                                                         </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Familia" AccessibleHeaderText="GrpItem3Name">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label runat="server" ID="lblGroupItem3" Text='<%# Bind("ItemKit.GrpItem3.Name") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Subfamilia" AccessibleHeaderText="GrpItem4Name">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label runat="server" ID="lblGroupItem4" Text='<%# Bind("ItemKit.GrpItem4.Name") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ShowHeader="False" HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                                    <ItemTemplate>
                                                                        <center>
                                                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                                                CausesValidation="false" CommandName="Delete" />
                                                                        </center>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>  
                                        </div>
                                        <%-- FIN Grilla Principal --%>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="imgBtnAddItemKit" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>  
                </Content>
            </TopPanel>
            <BottomPanel HeightMin="40">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <%-- Panel Grilla Detalle --%>
                                <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Always" ChildrenAsTriggers="true">
                                    <ContentTemplate>
                                        <%-- Panel Detalle Kit --%>
                                        <div style="clear: both">

                                
                                            <div id="divItemDetail" visible="false" runat="server" class="divItemDetails">
                                                <%-- Panel Agregar/Modificar Item (Kit Detail) --%>
                                                <div id="Div1" runat="server" visible="true" class="divGridTitle">
                                                    <asp:Label ID="lblGridDetail" runat="server" Text="" />
                                                </div>
                                                <%--Codigo--%>
                                                <div class="mainFilterPanelItem">
                                                    <asp:Label ID="lblCode" runat="server" Text="Item" /><br />
                                                    <asp:TextBox ID="txtCode" runat="server" Width="100px" />
                                                <%--</div>--%>
                                                <%--Boton Buscar--%>
                                                <%--<div class="mainFilterPanelItem">--%>
                                                    <asp:ImageButton ID="imgbtnSearchItem" runat="server" Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                                        OnClick="imgBtnSearchItem_Click" Width="18px" ValidationGroup="searchItem" ToolTip="Buscar Item" />
                                                </div>
                                                <%--Descripcion--%>
                                                <div class="mainFilterPanelItem">
                                                    <asp:Label ID="lblDescription" runat="server" Text="Descripci&oacute;n" /><br />
                                                    <asp:TextBox ID="txtDescription" runat="server" Width="150px" MaxLength="30" Enabled="False"
                                                        ReadOnly="True" />
                                                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                                        ValidationGroup="AddItem" Text=" * " ErrorMessage="Descripci&oacute;n es requerido." />
                                                </div>
                                                <%--Cantidad--%>
                                                <div class="mainFilterPanelItem">
                                                    <asp:Label ID="lblQty" runat="server" Text="Cantidad" /><br />
                                                    <asp:TextBox ID="txtQty" runat="server" Width="120px" MaxLength="13"  />
                                                    <asp:RequiredFieldValidator ID="rfvQty" runat="server" ControlToValidate="txtQty" ValidationGroup="AddItem" Text=" * " ErrorMessage="Cantidad es requerido." />
                                                    <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="txtQty" Text=" * " 
                                                        ErrorMessage="Cantidad no contiene un número válido" Enabled="true"
                                                        MaximumValue="99999999" MinimumValue="0,0001" ValidationGroup="AddItem" Type="Double">*</asp:RangeValidator>
                                        
                                                </div>
                                                <%--Boton Agregar Item--%>
                                                <div class="mainFilterPanelItem">
                                                    <br />
                                                    <asp:ImageButton ID="imgBtnAddItem" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_add_item.png"
                                                        OnClick="imgBtnAddItem_Click" ValidationGroup="AddItem" ToolTip="Agregar Item" />
                                                </div>
                                                <%--Panel de error--%>
                                                <asp:Panel ID="pnlError" runat="server" Visible="false">
                                                    <asp:Label ID="lblMesage" runat="server" ForeColor="Red" Text="El item ya fue ingresado" />
                                                </asp:Panel>
                                                <%--Panel de resumen de validacion--%>
                                                <div class="mainFilterPanelItem">
                                                    <asp:ValidationSummary ID="valAddItem" runat="server" ValidationGroup="AddItem" ShowMessageBox="false" />
                                                    <asp:ValidationSummary ID="valSearchItem" runat="server" ValidationGroup="searchItem" />
                                                    <asp:Label ID="lblErrorNoOwn" Visible="false" runat="server" Text="Debe Seleccionar un Dueño" ForeColor="Red" />
                                                </div>
                                                <%-- FIN Panel Agregar/Modificar Item (Kit Detail) --%>
                                                <div ID="divGridDetail" style="clear: both; margin: 2px">
                                                    <%-- Grilla Detalle Kits--%>
                                                    <asp:GridView ID="grdDetail" runat="server" EnableViewState="False" DataKeyNames="Id"
                                                        OnRowDeleting="grdDetail_RowDeleting" 
                                                        OnRowEditing="grdDetail_RowEditing"
                                                        OnRowCreated="grdDetail_RowCreated"
                                                        OnRowDataBound="grdDetail_RowDataBound"
                                                        AutoGenerateColumns="False"
                                                        CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                        EnableTheming="false">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Id Item" Visible="false" AccessibleHeaderText="IdItem">
                                                                <ItemTemplate>
                                                                    <div style="width:20px">
                                                                        <asp:Label runat="server" ID="lblIdItemBase" Text='<%# Bind("ItemBase.Id") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="C&oacute;digo Componente" AccessibleHeaderText="ItemCode">
                                                                <ItemTemplate>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label runat="server" ID="lblItemCode" Text='<%# Bind("ItemBase.Code") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Descripci&oacute;n" AccessibleHeaderText="Description">
                                                                <ItemTemplate>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label runat="server" ID="lblDescription" Text='<%# Bind("ItemBase.Description") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Nombre Largo" AccessibleHeaderText="LongName">
                                                                <ItemTemplate>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label runat="server" ID="lblItemLongName" Text='<%# Bind("ItemBase.LongName") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>                                                
                                                            <asp:TemplateField HeaderText="Sector" AccessibleHeaderText="GrpItem1Name">
                                                                <ItemTemplate>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label runat="server" ID="lblGroupItem1" Text='<%# Bind("ItemBase.GrpItem1.Name") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Rubro" AccessibleHeaderText="GrpItem2Name">
                                                                <ItemTemplate>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label runat="server" ID="lblGroupItem2" Text='<%# Bind("ItemBase.GrpItem2.Name") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Familia" AccessibleHeaderText="GrpItem3Name">
                                                                <ItemTemplate>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label runat="server" ID="lblGroupItem3" Text='<%# Bind("ItemBase.GrpItem3.Name") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Subfamilia" AccessibleHeaderText="GrpItem4Name">
                                                                <ItemTemplate>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label runat="server" ID="lblGroupItem4" Text='<%# Bind("ItemBase.GrpItem4.Name") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty">
                                                                <ItemTemplate>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label runat="server" ID="lblItemQty" Text='<%# GetFormatedNumber(((decimal) Eval ("ItemQty") == -1)?" ":Eval ("ItemQty")) %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ShowHeader="False" HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                                <ItemTemplate>
                                                                    <center>
                                                                        <div style="width: 60px">
                                                                            <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                                                CausesValidation="false" CommandName="Edit" ToolTip="Editar" />
                                                                            <asp:ImageButton ID="btnDelete1" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                                                CausesValidation="false" CommandName="Delete" ToolTip="Eliminar" />
                                                                        </div>
                                                                    </center>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataTemplate>
                                                            <asp:Label ID="lblEmptyGrid" runat="server" Text="No se han encontrado registros." />
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                    <%-- FIN Grilla Detalle Kits--%>
                                                </div>
                                            </div>
                                        </div>
                                        <%-- FIN Panel Detalle Kit --%>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="RowCommand" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="imgBtnAddItem" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <%-- FIN Panel Grilla Detalle --%>
                            </div>
                        </div>
                    </div>  
                </Content>
                <Footer Height="67">
                    <div style="color: White">
                        No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>   
    </div>    

     <%-- Carga masiva de Kits --%>
    <asp:UpdatePanel ID="upLoadKit" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
             <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoad" runat="server" visible="true">
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
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Kit.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />   
                            <asp:ImageButton ID="ImageButton1" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">

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
                                    <asp:Button ID="btnSubir2" runat="server" Text="Cargar Archivo" ValidationGroup="Load" 
                                    OnClientClick="showProgress()" onclick="btnSubir2_Click" />
                                </div>
                            </div>
                        </div>
                         <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load"
                                ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div4" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubir2" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="uprLoad" runat="server" AssociatedUpdatePanelID="upLoadKit" DisplayAfter="20" 
     DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprLoad" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLoad" />
    
    <%-- Lookup  KITS--%>
    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Always" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div id="divLookupItem" runat="server" visible="true">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpLookupItem" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlLookupItem" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupItem"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlLookupItem" runat="server" Width="500px" CssClass="modalBox">
                    <asp:Panel ID="pnlHeadBar2" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblAddItem" runat="server" Text="Nuevo Kit" />
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidItemId" runat="server" Value="-1" />
                        
                        
                        
                        <webUc:ucLookUpFilter ID="ucFilterItem" runat="server" />
                        <webUc:ucLookUpFilter ID="ucFilterItem2" runat="server" />
                        <div class="divCtrsFloatLeft" >
                            <div class="divLookupGrid" style="width:470px;">
                                <asp:GridView ID="grdSearchItems" runat="server" DataKeyNames="Id"  AllowPaging="true"
                                OnRowCommand="grdSearchItems_RowCommand" AutoGenerateColumns="False" 
                                    onrowdatabound="grdSearchItems_RowDataBound"
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                    EnableTheming="false">
                                    <Columns>
                                        <asp:BoundField AccessibleHeaderText="Id" DataField="Id" HeaderText="ID" InsertVisible="True"
                                            SortExpression="Id" />
                                        <asp:TemplateField AccessibleHeaderText="ItemCode" HeaderText="C&oacute;d.">
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
                                                    <asp:ImageButton ID="imgBtnAddItem" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
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
                    <div id="divPageGrdSearchItems" runat="server" class="modalActions">
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
            </div>
        </ContentTemplate>
        <Triggers>
<%--            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$bottomPanel$divItemDetail$ctl01$grdDetail"
                EventName="RowCommand" />--%>
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <%-- FIN Lookup Items --%>
    

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
                                        
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Kit?" Visible="false" />
    <asp:Label ID="lblConfirmDeleteItem" runat="server" Text="¿Desea quitar este item del Kit?" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="C&oacute;digo" Visible="false" />
    <asp:Label ID="lblNewKit" runat="server" Text="Nuevo Kit - Seleccione Item" Visible="false" />        
    <asp:Label ID="lblNewDetail" runat="server" Text="Nuevo Detalle - Seleccione Item" Visible="false" />        
    <asp:Label ID="lblDetailsHead" runat="server" Text="Detalle de Kit: " Visible="false" />
    <asp:Label ID="lblErrorAllOwner" runat="server" Text="- Debe seleccionar un dueño" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Mantenedor de Kits" Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es valído." Visible="false" />
    <asp:Label ID="lblNotFileLoad" runat="server" Text="Seleccione el archivo que desea cargar." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen items en el archivo." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblFieldInvalid" runat="server" Text="Formato del campo no es válido." Visible="false" />
    <asp:Label ID="lblAddLoadToolTip" runat="server" Text="Carga Masiva" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
