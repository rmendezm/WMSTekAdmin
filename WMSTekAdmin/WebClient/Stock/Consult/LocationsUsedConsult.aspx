<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="LocationsUsedConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inbound.Consult.LocationsUsedConsult" %>
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
        initializeGridDragAndDrop('LocationsUsedConsult', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
    }

</script>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="50">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <%-- Panel Grilla Principal --%>
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                         <asp:GridView ID="grdMgr" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged"
                                            AllowPaging="True" EnableViewState="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                                    <Columns>
                                            
                                                        <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ( "IdLocCode" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="ID Centro" AccessibleHeaderText="IdWhs">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblIdWhs" runat="server" Text='<%# Eval ( "IdWhs" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Centro Dis." AccessibleHeaderText="WhsName">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblWhsName" runat="server" Text='<%# Eval("Warehouse.ShortName") %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Cod Ubicación" AccessibleHeaderText="LocCode">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblLocCode" runat="server" Text='<%# Eval ( "LocCode" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Bodega" AccessibleHeaderText="HngName">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblLocCode" runat="server" Text='<%# Eval ( "Hangar.Name" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Id Bodega" AccessibleHeaderText="IdHng">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblIdHng" runat="server" Text='<%# Eval ( "IdHng" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Fila" AccessibleHeaderText="RowLoc">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblRowLoc" runat="server" Text='<%# Eval ( "RowLoc" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Columna" AccessibleHeaderText="ColumnLoc">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblColumnLoc" runat="server" Text='<%# Eval ( "ColumnLoc" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Nivel" AccessibleHeaderText="LevelLoc">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblLevelLoc" runat="server" Text='<%# Eval ( "LevelLoc" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Pasillo" AccessibleHeaderText="Aisle">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblAisle" runat="server" Text='<%# Eval ( "Aisle" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Cod Tipo Ubic." AccessibleHeaderText="LocTypeCode">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblLocTypeCode" runat="server" Text='<%# Eval ( "LocTypeCode" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Cant Actual" AccessibleHeaderText="SumItemQty">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblSumItemQty" runat="server" Text='<%# GetFormatedNumber(Eval ( "SumItemQty" )) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Cant Máxima" AccessibleHeaderText="ReorderQty">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblReorderQty" runat="server" Text='<%# GetFormatedNumber(Eval ( "ReorderQty" )) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="% Usado" AccessibleHeaderText="PctUsed">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblPctUsed" runat="server" Text='<%# Eval ( "PctUsed" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Tipo Ubic." AccessibleHeaderText="Description">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ( "Description" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Compartida" AccessibleHeaderText="SharedItem">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblSharedItem" runat="server" Text='<%# Eval ( "SharedItem" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Solo LPN" AccessibleHeaderText="OnlyLPN">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblOnlyLPN" runat="server" Text='<%# Eval ( "OnlyLPN" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="ID Dueño" AccessibleHeaderText="IdOwn">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ( "IdOwn" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Dedicada" AccessibleHeaderText="DedicatedOwner">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblDedicatedOwner" runat="server" Text='<%# Eval ( "DedicatedOwner" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Pick Flow" AccessibleHeaderText="PickingFlow">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblPickingFlow" runat="server" Text='<%# Eval ( "PickingFlow" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Put Flow" AccessibleHeaderText="PutawayFlow">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblPutawayFlow" runat="server" Text='<%# Eval ( "PutawayFlow" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Capacidad LPN" AccessibleHeaderText="CapacityLPN">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblCapacityLPN" runat="server" Text='<%# GetFormatedNumber(Eval ( "CapacityLPN" )) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Capacidad Unid." AccessibleHeaderText="CapacityUnit">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblCapacityUnit" runat="server" Text='<%# GetFormatedNumber(Eval ( "CapacityUnit" )) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Largo" AccessibleHeaderText="Length">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblLength" runat="server" Text='<%# Eval ( "Length" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Ancho" AccessibleHeaderText="Width">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblWidth" runat="server" Text='<%# Eval ( "Width" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Alto" AccessibleHeaderText="Height">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblHeight" runat="server" Text='<%# Eval ( "Height" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Volumen" AccessibleHeaderText="Volume">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblVolume" runat="server" Text='<%# Eval ( "Volume" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="Weight">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblWeight" runat="server" Text='<%# Eval ( "Weight" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Pos X" AccessibleHeaderText="PositionX">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblPositionX" runat="server" Text='<%# Eval ( "PositionX" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Pos Y" AccessibleHeaderText="PositionY">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblPositionY" runat="server" Text='<%# Eval ( "PositionY" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Pos Z" AccessibleHeaderText="PositionZ">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblPositionZ" runat="server" Text='<%# Eval ( "PositionZ" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                            
                                                        <asp:TemplateField HeaderText="Cod. Bloqueo" AccessibleHeaderText="HoldCode">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblHoldCode" runat="server" Text='<%# Eval ( "HoldCode" ) %>' />
                                                                </div>
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
                                <%-- FIN Panel Grilla Principal --%>
                            </div>
                        </div>
                    </div>
                </Content>
            </TopPanel>
            <BottomPanel HeightMin="50">
                <Content>
                    <%-- Panel Grilla Detalle --%>
                    <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">  
                    <ContentTemplate>                                                
                        <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                            <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                            <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Ubicación: " />
                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                          </div>

                            <div class="container">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:GridView ID="grdDetail" runat="server" DataKeyNames="Id" EnableViewState="false"
                                            OnRowCreated="grdDetail_RowCreated" SkinID="grdDetail"
                                            OnRowDataBound="grdDetail_RowDataBound"
                                            AutoGenerateColumns="false"
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
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Eval ("Status") %>' />
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
                                </div>
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
                  </Triggers>
                </asp:UpdatePanel>
                    <%-- FIN Panel Grilla Detalle --%>
                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>
    </div>    

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
    <%-- FIN Barra de Estado --%>
</asp:Content>
