<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="PackagesConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.LpnConsult" %>

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

        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('Package_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }
    </script>

    <div id="divPrincipal" style=" width:100%; height:100%; margin:0px;margin-bottom:80px">
    <spl:HorizontalSplitter LiveResize="false"   CookieDays="0"  ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
        <TopPanel ID="topPanel" HeightMin="50">
            <Content>
                 <%--<div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">--%>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Principal --%>
                            <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
                                <ContentTemplate>
                                    <asp:GridView ID="grdMgr" runat="server" AllowPaging="True"
                                            OnRowCreated="grdMgr_RowCreated" 
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            EnableViewState="False"
                                            AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLPN" runat="server" Text='<%# Eval ( "LPN.Code" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cód. Tipo Lpn" AccessibleHeaderText="LpnTypeCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLpnTypeCode" runat="server" Text='<%# Eval ( "LPN.LPNType.Code" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo LPN" AccessibleHeaderText="LpnTypeName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLpnTypeName" runat="server" Text='<%# Eval ( "LPN.LPNType.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Id Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdWhs" runat="server" Text='<%# Eval ( "Warehouse.Id" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cód. CD" AccessibleHeaderText="WhsCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="ShortWhsName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWhsName" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "LPN.Owner.Code" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ( "LPN.Owner.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwner" runat="server" Text='<%# Eval ( "LPN.Owner.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nº Documento" AccessibleHeaderText="OutboundNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Eval ( "OutboundOrder.Number" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nº Referencia" AccessibleHeaderText="ReferenceNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblReferenceNumber" runat="server" Text='<%# Eval ( "OutboundOrder.ReferenceNumber" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo Salida" AccessibleHeaderText="OutboundTypeName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "OutboundOrder.OutboundType.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="NameTrackOutboundType">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNameTrackOutboundType" runat="server" Text='<%# Eval ( "TrackOutboundType.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Id Documento" AccessibleHeaderText="IdOutboundOrder" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdOutboundOrder" runat="server" Text='<%# Eval ( "OutboundOrder.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ruta" AccessibleHeaderText="RouteCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRouteCode" runat="server" Text='<%# Eval ( "OutboundOrder.RouteCode" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%-- <asp:TemplateField HeaderText="Línea" AccessibleHeaderText="OutboundLineNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOutboundLineNumber" runat="server" Text='<%# Eval ( "Stock.OutboundLineNumber" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ( "Stock.Location.IdCode" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tot ítems" AccessibleHeaderText="TotItems" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotItems" runat="server" Text='<%# GetFormatedNumber(((Decimal) Eval ( "Stock.Qty" )==-1)?"":Eval ( "Stock.Qty" ))%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fifo" AccessibleHeaderText="FifoDate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime) Eval ("LPN.Fifo") > DateTime.MinValue)? Eval("LPN.Fifo", "{0:d}"):"" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="WeightTotal" SortExpression="WeightTotal">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWeightTotal" runat="server" Text='<%# Eval ("LPN.WeightTotal") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Volúmen" AccessibleHeaderText="VolumeTotal" SortExpression="VolumeTotal">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblVolumeTotal" runat="server" Text='<%# Eval ("LPN.VolumeTotal") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cerrado" AccessibleHeaderText="IsClosed" SortExpression="IsClosed">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkIsClosed" runat="server" Checked='<%# Eval ("LPN.IsClosed") %>'
                                                            Enabled="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cod. Cliente" AccessibleHeaderText="CustomerCode" SortExpression="CustomerCode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ("OutboundOrder.CustomerCode") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerName" SortExpression="CustomerName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ("OutboundOrder.CustomerName") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fec. Esperada" AccessibleHeaderText="ExpectedDate" SortExpression="ExpectedDate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblExpectedDate" runat="server" Text='<%# ((DateTime) Eval ("OutboundOrder.ExpectedDate") > DateTime.MinValue)? Eval("OutboundOrder.ExpectedDate", "{0:d}"):"" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sucursal" AccessibleHeaderText="Branch" ItemStyle-Wrap="false">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblbranchName" runat="server" text='<%# Eval ( "OutboundOrder.Branch.Name" ) %>'/>
                                                        </div>  
                                                    </itemtemplate>
                                                </asp:TemplateField>          
                                                <asp:TemplateField HeaderText="Id Pais" AccessibleHeaderText="IdCountryDelivery" SortExpression="IdCountryDelivery">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdCountryDelivery" runat="server" Text='<%# Eval ("OutboundOrder.CountryDelivery.Id") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Pais" AccessibleHeaderText="CountryName" SortExpression="CountryName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCountryName" runat="server" Text='<%# Eval ("OutboundOrder.CountryDelivery.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Id Region" AccessibleHeaderText="IdStateDelivery" SortExpression="IdStateDelivery">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdStateDelivery" runat="server" Text='<%# Eval ("OutboundOrder.StateDelivery.Id") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Región" AccessibleHeaderText="StateName" SortExpression="StateName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStateName" runat="server" Text='<%# Eval ("OutboundOrder.StateDelivery.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Id Ciudad" AccessibleHeaderText="IdCityDelivery" SortExpression="IdCityDelivery">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdCityDelivery" runat="server" Text='<%# Eval ("OutboundOrder.CityDelivery.Id") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ciudad" AccessibleHeaderText="CityName" SortExpression="CityName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCityName" runat="server" Text='<%# Eval ("OutboundOrder.CityDelivery.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sello" AccessibleHeaderText="SealNumber" SortExpression="SealNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSealNumber" runat="server" Text='<%# Eval ("LPN.SealNumber") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Lpn Contenedor" AccessibleHeaderText="LpnParent" SortExpression="LpnParent" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLpnParent" runat="server" Text='<%# Eval ("LPN.LpnParent") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />             
                                </Triggers>
                            </asp:UpdatePanel> 
                        </div>
                    </div>
                 </div> 
                <%-- FIN Panel Grilla Principal --%>
               <%--</div>--%>
            </Content>
        </TopPanel>
        <BottomPanel HeightMin="50">
            <Content>
                <%--<div onresize="SetDivs();">--%>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Detalle --%>
                            <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">  
                                <ContentTemplate>                                                
                                    <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                        <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                            <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Docto: " />
                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                        </div>
                            
                                        <asp:GridView ID="grdDetail" runat="server" DataKeyNames="Id" EnableViewState="false"
                                            OnRowCreated="grdDetail_RowCreated" SkinID="grdDetail"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdDetail_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Cod. Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ("Item.Code") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName" SortExpression="LongItemName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Description" SortExpression="Description">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval ("Item.Description") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                    
                                                <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CtgName" SortExpression="CtgName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCategoryItemName" runat="server" Text='<%# Eval ("CategoryItem.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Status" AccessibleHeaderText="Status" SortExpression="Status">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ("Status") %>'
                                                            Enabled="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty" SortExpression="ItemQty">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <right>
                                                            <asp:Label ID="lblQty" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("Qty") == -1)?" ":Eval ("Qty")) %>' />
                                                        </right>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="TotalWeight" SortExpression="TotalWeight">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <right>
                                                            <asp:Label ID="lblTotalWeight" runat="server" text='<%# ((decimal) Eval ("TotalWeight") == -1)?" ":Eval ("TotalWeight") %>' />
                                                        </right>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Volumen" AccessibleHeaderText="TotalVolumen" SortExpression="TotalVolumen">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <right>
                                                            <asp:Label ID="lblTotalVolumen" runat="server" text='<%# ((decimal) Eval ("TotalVolumen") == -1)?" ":Eval ("TotalVolumen") %>' />
                                                        </right>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate"
                                                    SortExpression="ExpirationDate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblExpiration" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fifo" AccessibleHeaderText="FifoDate" SortExpression="FifoDate">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fabricación" AccessibleHeaderText="FabricationDate"
                                                    SortExpression="FabricationDate">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:Label ID="lblFabricationDate" runat="server" Text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode" SortExpression="IdLocCode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ("Location.IdCode") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ( "Lot" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bulto" AccessibleHeaderText="IdLpnCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdLpnCode" runat="server" Text='<%# Eval ( "Lpn.Code" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo LPN" AccessibleHeaderText="LpnTypeCode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLpnTypeCode" runat="server" Text='<%# Eval ( "Lpn.LPNType.Code" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Precio" AccessibleHeaderText="Price" SortExpression="Price">
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:Label ID="lblPrice" runat="server" Text=' <%# ((decimal) Eval ("Price") == -1 )?" ": Eval ("Price") %>' />
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nº Recepción" AccessibleHeaderText="IdReceipt" SortExpression="IdReceipt" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:Label ID="lblIdReceipt" runat="server" Text='<%# ((int) Eval ("Receipt.Id") == -1 )?" ": Eval ("Receipt.Id") %>'></asp:Label>
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Doc. Entrada" AccessibleHeaderText="InboundNumber" ItemStyle-CssClass="text"
                                                    SortExpression="InboundNumber">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInboundNumber" runat="server" Text='<%# Eval ( "InboundOrder.Number" ) %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="false" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nº Línea" AccessibleHeaderText="InboundLineNumber">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInboundLineNumber" runat="server" Text='<%# ((int) Eval ("InboundLineNumber")  == -1 )?" ": Eval ("InboundLineNumber")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Línea" AccessibleHeaderText="OutboundLineNumber">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOutboundLineNumber" runat="server" Text='<%# ((int) Eval ("OutboundLineNumber")  == -1 )?" ": Eval ("OutboundLineNumber")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sector" AccessibleHeaderText="GrpItem1Name">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblGroupItem1" Text='<%# Bind("Item.GrpItem1.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Rubro" AccessibleHeaderText="GrpItem2Name">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblGroupItem2" Text='<%# Bind("Item.GrpItem2.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Familia" AccessibleHeaderText="GrpItem3Name">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblGroupItem3" Text='<%# Bind("Item.GrpItem3.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Subfamilia" AccessibleHeaderText="GrpItem4Name">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblGroupItem4" Text='<%# Bind("Item.GrpItem4.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sello" AccessibleHeaderText="SealNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblSealNumber" Text='<%# Bind("Seal") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Razón" AccessibleHeaderText="ReasonCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblReasonCode" Text='<%# Bind("Reason") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bloqueo" AccessibleHeaderText="HoldCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblHoldCode" Text='<%# Bind("Hold") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
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
                              </Triggers>
                            </asp:UpdatePanel>  
                            <%-- FIN Panel Grilla Detalle --%>
                        </div>
                    </div>
                </div>
                <%--</div>--%>
            </Content>
            <Footer Height="67">
                <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
            </Footer>
        </BottomPanel>
    </spl:HorizontalSplitter>
</div>  
    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label id="lblFilterDate" runat="server" Text="Fec Entrega" Visible="false" />  
    <asp:Label id="lblCodeLpn" runat="server" Text="LPN" Visible="false" />  
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>  
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>
