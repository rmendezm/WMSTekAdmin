<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="InventoryConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inventory.Consult.InventoryConsult" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">

    function resizeDiv() {
        //debugger;
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("divPrincipal").style.height = h;
        document.getElementById("divPrincipal").style.width = w;
    }

    window.onresize = resizeDiv; 	
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(resizeDiv);

    function initializeGridWithNoDragAndDropCustom() {
        initializeGridWithNoDragAndDrop(true);
        removeFooter("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail");
    }

    $(document).ready(function () {
        initializeGridWithNoDragAndDropCustom(true);

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
        initializeGridWithNoDragAndDropCustom(true);
    }

    function setDivsAfter() {

        if ($("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("height")) {

            var heigthDetail = parseInt($("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("height").replace("px", ""));
            var extraSpace = 70;

            heigthDetail = heigthDetail - extraSpace;
            $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("height", heigthDetail + "px");
        }    
    }
</script>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="50" HeightDefault="200">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <%-- Panel Grilla Principal --%>
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <asp:GridView ID="grdMgr" 
                                            runat="server" DataKeyNames="Id" 
                                            OnRowCreated="grdMgr_RowCreated" 
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged"
                                            EnableViewState="false"
                                            AllowPaging="False" 
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>
                                                <asp:BoundField DataField="Id" HeaderText="ID" AccessibleHeaderText="Id" />
                                                <asp:TemplateField HeaderText="% Avance" AccessibleHeaderText="AmountProgress">
                                                    <ItemTemplate>
                                                        <center>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblAmountProgress" runat="server" Text='<%# Eval("AmountProgress")%>' />
                                                            </div>
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>                        
                                                <asp:TemplateField HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Number" HeaderText="Nº Inv." AccessibleHeaderText="Number" ItemStyle-CssClass="text" ItemStyle-HorizontalAlign="Center" />
                                                <asp:TemplateField HeaderText="Creado" AccessibleHeaderText="CreateDate">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <center>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCreateDate" runat="server" Text='<%# ((DateTime) Eval ("CreateDate") > DateTime.MinValue)? Eval("CreateDate", "{0:d}"):"" %>' />
                                                            </div>
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Inicio" AccessibleHeaderText="StartDate" SortExpression="StartDate">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <center>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime) Eval ("StartDate") > DateTime.MinValue)? Eval("StartDate", "{0:dd-MM-yyyy HH:mm}"):"" %>' />
                                                            </div>
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Término" AccessibleHeaderText="EndDate" SortExpression="EndDate">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <center>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime) Eval ("EndDate") > DateTime.MinValue)? Eval("EndDate", "{0:dd-MM-yyyy HH:mm}"):"" %>' />
                                                            </div>
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Centro Compl." AccessibleHeaderText="IsFullWhs">
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:CheckBox ID="chkIsFullWhs" runat="server" Checked='<%# Eval ( "IsFullWhs" ) %>'
                                                                Enabled="false" />
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="TrackInventoryType">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblTrackInventoryType" runat="server" Text='<%# Eval ( "TrackInventoryType.Name" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Creador" AccessibleHeaderText="UserCreated">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblUserCreated" runat="server" Text='<%# Eval ( "UserCreate.UserName" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Aprobador" AccessibleHeaderText="UserApproval">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblUserApproval" runat="server" Text='<%# Eval ( "UserApproval.UserName" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="CountQty" HeaderText="Cant. Conteos" AccessibleHeaderText="CountQty"
                                                    ItemStyle-HorizontalAlign="Center">
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status" SortExpression="Status">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="false" />
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                    <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <%-- FIN Panel Grilla Principal --%>
                            </div>
                        </div>
                    </div>  
                </Content>
            </TopPanel>
            <BottomPanel HeightMin="50">
                <Content>
                     <div class="container limitDiv">
                        <div class="row">
                            <div class="col-md-12">
                                <%-- Panel Detalle --%>
                                <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                            <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                                <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Inventario: " />
                                                <asp:Label ID="lblNroInv" runat="server" Text="" />
                                            </div>
                                
                                                    <%-- Filtro de detalle del Inventario --%>
                                
                                                    <%-- Location Type --%>
                                                    <div class="mainFilterPanelItem">
                                                        <asp:Label ID="lblLocationType1" runat="server" Text="Tipo" /><br />
                                                        <asp:DropDownList SkinID="ddlFilter" ID="ddlLocationType" runat="server" Width="70px" />
                                                    </div>
                                                    <%-- IdLocCode --%>
                                                    <div class="mainFilterPanelItem" >
                                                        &nbsp;&nbsp;<asp:Label ID="lblIdLocCode1" runat="server" Text="Ubicación" /><br />
                                                        <asp:TextBox SkinID="txtFilter" ID="txtIdLocCode" runat="server" Width="70px" />
                                                    </div>
                                                    <%-- Counted --%>
                                                    <div class="mainFilterPanelItem">
                                                        <asp:Label ID="lblCounted" runat="server" Text="Contada" /><br />
                                                        <asp:DropDownList SkinID="ddlFilter" ID="ddlCounted" runat="server" Width="70px" AutoPostBack="false">
                                                            <asp:ListItem Text="(Todos)" Value="-1" Selected="True" />
                                                            <asp:ListItem Text="Sí" Value="1" />
                                                            <asp:ListItem Text="No" Value="0" />
                                                        </asp:DropDownList>
                                                    </div>       
                                                    <%-- Empty --%>
                                                    <div class="mainFilterPanelItem">
                                                        &nbsp;<asp:Label ID="lblEmpty" runat="server" Text="Vacia" /><br />
                                                        &nbsp;<asp:DropDownList SkinID="ddlFilter" ID="ddlEmpty" runat="server" Width="70px">
                                                            <asp:ListItem Text="(Todos)" Value="-1" Selected="True" />
                                                            <asp:ListItem Text="Sí" Value="1" />
                                                            <asp:ListItem Text="No" Value="0" />
                                                        </asp:DropDownList>
                                                    </div>   
                                                    <%-- Item --%>
                                                    <div class="mainFilterPanelItem">
                                                        &nbsp;&nbsp;<asp:Label ID="lblItem" runat="server" Text="Cód. Item" /><br />
                                                        <asp:TextBox SkinID="txtFilter" ID="txtItemCode" runat="server" Width="70px" />
                                                    </div>    
                                                    <%-- Dif. Cant. --%>
                                                    <div class="mainFilterPanelItem">
                                                        &nbsp;&nbsp;<asp:Label ID="lblDifQty" runat="server" Text="Dif. Cant. >=" /><br />
                                                        <asp:TextBox SkinID="txtFilter" ID="txtDifQty" runat="server" Width="70px" />
                                                        <asp:RangeValidator ID="rvDifQty" runat="server" ControlToValidate="txtDifQty" ErrorMessage="Dif. Cant. no contiene un número válido." Text=" * " MaximumValue="2147483647" MinimumValue="0" ValidationGroup="Search1" Type="Integer" />
                                                    </div>  
                                                    <%-- Dif $ --%>
                                                    <div class="mainFilterPanelItem">
                                                        &nbsp;&nbsp;<asp:Label ID="lblDifAmount" runat="server" Text="Dif. $ >=" /><br />
                                                        <asp:TextBox SkinID="txtFilter" ID="txtDifAmount" runat="server" Width="70px" />
                                                        <asp:RangeValidator ID="rvDifAmount" runat="server" ControlToValidate="txtDifAmount" ErrorMessage="Dif. $ no contiene un número válido." Text=" * " MaximumValue="2147483647" MinimumValue="0" ValidationGroup="Search1" Type="Integer" />
                                                    </div>                                                                                                                                                                                               
                                                    <%-- Boton 'Buscar' --%>
                                                    <div class="mainFilterPanelItem">
                                                        <asp:ImageButton ID="btnSearch1" runat="server" OnClick="btnSearch1_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                                            onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search_on.png';"
                                                            onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search.png';"
                                                            ToolTip="Buscar"
                                                            ValidationGroup="Search1" />
                                                    </div>
                                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Search1"    CssClass="modalValidation" />

                                                    <%-- FIN Filtro de detalle del Inventario --%>
                                         
                                                    <%-- Grilla de detalle de Inventario --%>
                                                    <div id="divGrid" runat="server" class="textLeft">
                                                        <asp:GridView ID="grdDetail" runat="server" DataKeyNames="Id" EnableViewState="true" SkinId="grdDetail"
                                                             OnRowCreated="grdDetail_RowCreated"
                                                             OnRowCommand="grdDetail_RowCommand"
                                                             OnRowDataBound="grdDetail_RowDataBound"
                                                             OnDataBound="grdDetail_OnDataBound"
                                                             AllowPaging="True"
                                                             AutoGenerateColumns="false"
                                                             CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                             EnableTheming="false">
                                                            <Columns>
                                                                <asp:BoundField DataField="Id" HeaderText="Id2" Visible="false" />
                                                                <asp:TemplateField HeaderText="Id Detail" AccessibleHeaderText="IdDetail" Visible="false">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblId" runat="server" Text='<%# Eval ("Id") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>   
                                                                             
                                                                <asp:BoundField DataField="IdInventory" HeaderText="IdInventory" Visible="false" />     
                                                        
                                                                <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdCode" SortExpression="IdCode"  ItemStyle-CssClass="text">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ("Location.IdCode") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>                                                    
                                                                                                     
                                                                <asp:TemplateField HeaderText="Contada" AccessibleHeaderText="IsCounted" SortExpression="IsCounted" >
                                                                    <ItemTemplate>
                                                                        <center>
                                                                            <asp:Image ID="ImgCount" runat="server"/>
                                                                        </center>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                    
                                                                <asp:TemplateField HeaderText="Ubic. Vacia" AccessibleHeaderText="IsEmptyLocation" SortExpression="IsEmptyLocation">
                                                                    <ItemTemplate>
                                                                        <center>
                                                                            <asp:CheckBox ID="chkIsEmpty" runat="server" Checked='<%# Eval ( "IsEmptyLocation" ) %>' Enabled="false" />
                                                                        </center>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField> 
                                                    
                                                                <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="LpnCode" ItemStyle-CssClass="text">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblIdLpnCode" runat="server" Text='<%# Eval ( "Lpn.Code" ) %>'></asp:Label>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>  
                                                    
                                                                <asp:TemplateField HeaderText="Generar Reconteo" AccessibleHeaderText="GenerateRecountTask">
                                                                    <ItemTemplate> 
                                                                        <div style="width:60px">
                                                                            <center>
	                                                                            <asp:ImageButton ID="btnInventoryRecount" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_start_inventory.png" CausesValidation="false" CommandName="GenerateRecountTask" CommandArgument='<%#Eval("Id")%>'  />
	                                                                        </center>
                                                                        </div>	                        
                                                                    </ItemTemplate>
                                                                </asp:TemplateField> 
                                                    
                                                                <asp:templatefield headertext="Dueño" accessibleHeaderText="OwnName" SortExpression="OwnName" ItemStyle-CssClass="text">
                                                                    <itemtemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                           <asp:label ID="lblOwnName" runat="server" text='<%# Eval ("Item.Owner.Name") %>' />
                                                                        </div>
                                                                    </itemtemplate>
                                                                </asp:templatefield>

                                                                <asp:templatefield headertext="Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                                                    <itemtemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                           <asp:label ID="lblItemCode" runat="server" text='<%# Eval ("Item.Code") %>' />
                                                                        </div>
                                                                    </itemtemplate>
                                                                </asp:templatefield>  
                                                                                                                                
                                                                <asp:TemplateField HeaderText="Nombre Item" AccessibleHeaderText="ItemLongName" SortExpression=Item"LongName">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:templatefield headertext="Descripción" accessibleHeaderText="ItemDescription" SortExpression="ItemDescription" ItemStyle-CssClass="text">
                                                                    <itemtemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                           <asp:label ID="lblItemDescription" runat="server" text='<%# Eval ("Item.Description") %>' />
                                                                       </div>
                                                                    </itemtemplate>
                                                                </asp:templatefield>     
                                                                           
                                                                <asp:TemplateField HeaderText="Cant. Contada" AccessibleHeaderText="ItemQty" SortExpression="ItemQty">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblItemQty" runat="server" Text='<%# GetFormatedNumber(((int) Eval ("Id") == -1)?" ":Eval ("ItemQty")) %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Cant. Sistema" AccessibleHeaderText="StockQty" SortExpression="StockQty">
                                                                    <ItemStyle Wrap="false" />
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblSysQty" runat="server" Text='<%# GetFormatedNumber(((decimal) Eval ("StockQty") == -1)?" ":Eval ("StockQty")) %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Diferencia Cant." AccessibleHeaderText="DifQty"
                                                                    SortExpression="differenceQty">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblDifferenceQty" runat="server" Text='<%# GetFormatedNumber(((int) Eval ("Id") == -1)?" ":Eval ("DifQty")) %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:templatefield headertext="Precio" accessibleHeaderText="ItemPrice" SortExpression="ItemPrice" ItemStyle-CssClass="text">
                                                                    <itemtemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                           <asp:label ID="lblItemPrice" runat="server" text='<%# Eval ("Item.Price") %>' />
                                                                       </div>
                                                                    </itemtemplate>
                                                                </asp:templatefield>  
                                                                <asp:TemplateField HeaderText="Diferencia" AccessibleHeaderText="DifAmount" SortExpression="DifAmount">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lbldifferenceValue" runat="server" Text='<%# ((int) Eval ("Id") == -1)?" ":Eval ("DifAmount") %>' />
                                                                       </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">                                                            
                                                                            <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ( "LotNumber" ) %>'></asp:Label>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Serie" AccessibleHeaderText="SerialNumber" Visible="false">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">                                                            
                                                                            <asp:Label ID="lblSerialNumber" runat="server" Text='<%# Eval ( "SerialNumber" ) %>'></asp:Label>
                                                                       </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Fifo" AccessibleHeaderText="FifoDate" SortExpression="FifoDate">
                                                                    <ItemStyle Wrap="false" />
                                                                    <ItemTemplate>
                                                                        <center>
                                                                            <div style="word-wrap: break-word;">                                                                
                                                                                <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                                                            </div>
                                                                        </center>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblExpiration" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Elaboración" AccessibleHeaderText="FabricationDate"  SortExpression="FabricationDate">
                                                                    <ItemStyle Wrap="false" />
                                                                    <ItemTemplate>
                                                                        <center>
                                                                            <div style="word-wrap: break-word;">
                                                                                <asp:Label ID="lblFabricationDate" runat="server" Text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                                                            </div>
                                                                        </center>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CategoryItemName" SortExpression="CategoryItemName">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblCategoryItemName" runat="server" Text='<%# Eval ("CategoryItem.Name") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                                <asp:templatefield headertext="Peso" accessibleHeaderText="TotalWeight" SortExpression="TotalWeight">
                                                                    <ItemStyle Wrap="false" />
                                                                    <itemtemplate>
                                                                        <right>
                                                                            <asp:label ID="lblTotalWeight" runat="server" 
                                                                            text='<%# GetFormatedNumber(((decimal) Eval ("TotalWeight") == -1)?" ":Eval ("TotalWeight")) %>' />
                                                                        </right>    
                                                                    </itemtemplate>
                                                                </asp:templatefield>
                                                    
                                                                <asp:TemplateField HeaderText="Reconteos" AccessibleHeaderText="RetryQty" SortExpression="RetryQty">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblRetryQty" runat="server" Text='<%# Eval ("RetryQty") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Operador" AccessibleHeaderText="UserInventory" SortExpression="UserInventory" ItemStyle-CssClass="text">
                                                                    <ItemTemplate>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblOperator" runat="server" Text='<%# Eval ("UserInventory") %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>   
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                        </div>
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
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$btnSearch1" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$grdDetail" EventName="DataBound" />
                            
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                    
                    <%-- Modal Update Progress --%>
                    <asp:UpdateProgress ID="uprGridDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>                        
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                                
                    <webUc:UpdateProgressOverlayExtender ID="muprGridDetail" runat="server" ControlToOverlayID="divDetail" CssClass="updateProgress" TargetControlID="uprGridDetail" />    
                    <%-- FIN Modal Update Progress --%>
                    
                    <%-- FIN Panel Detalle --%>
                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>
    </div>    

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />
    <asp:Label ID="lblNroInventoryFilter" runat="server" Text="Nº Inventario" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
    <%-- FIN Barra de Estado --%>
</asp:Content>