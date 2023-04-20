<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="InboundOrderConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inbound.Consult.InboundOrderWebConsult" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language='Javascript'>
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
        initializeGridDragAndDrop('InboundOrder_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr', 'InboundOrderConsult');
    }

    function setDivsAfter() {
        var heightDivBottom = $("#__hsMasterDetailRD").height();
        var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetailTitle").height();
        var extraSpace = 160;

        var totalHeight = heightDivBottom - heightLabelsBottom - extraSpace;
        $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");
    }
</script> 

    <div  id="divPrincipal" style=" width:100%; height:100%; margin:0px;margin-bottom:80px">
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
                                            DataKeyNames="Id" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged"
                                            AllowPaging="True" 
                                            EnableViewState="false"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>
                                                        <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" ReadOnly="True"
                                                            SortExpression="Id" AccessibleHeaderText="Id" ItemStyle-Wrap="false" />
                                                        <asp:TemplateField HeaderText="Tipo Doc." AccessibleHeaderText="InboundType" SortExpression="InboundType"
                                                            HeaderStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblInboundType" runat="server" Text='<%# Eval ( "InboundType.Name" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                            <ItemStyle Wrap="False"></ItemStyle>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Number" HeaderText="Nº Doc." AccessibleHeaderText="Number"
                                                            SortExpression="Number" ItemStyle-Wrap="false" ItemStyle-CssClass="text">
                                                            <ItemStyle Wrap="False"></ItemStyle>
                                                        </asp:BoundField>
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
                                                        <asp:TemplateField HeaderText="Proveedor" AccessibleHeaderText="Vendor" SortExpression="Vendor"
                                                            HeaderStyle-HorizontalAlign="Left">
                                                            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                            <ItemStyle Wrap="false" />
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval ( "Vendor.Name" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Comment" HeaderText="Comentarios" AccessibleHeaderText="Comment"
                                                            SortExpression="Comment" HeaderStyle-HorizontalAlign="Left">
                                                            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                            <ItemStyle Wrap="False"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Esperada" AccessibleHeaderText="DateExpected" SortExpression="DateExpected">
                                                            <ItemStyle Wrap="false" />
                                                            <ItemTemplate>
                                                                <center>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label ID="lblDateExpected" runat="server" Text='<%# ((DateTime) Eval ("DateExpected") > DateTime.MinValue)? Eval("DateExpected", "{0:d}"):"" %>' />
                                                                   </div>
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>                         
                                                        <asp:TemplateField HeaderText="Emisión" AccessibleHeaderText="EmissionDate" SortExpression="EmissionDate">
                                                            <ItemStyle Wrap="false" />
                                                            <ItemTemplate>
                                                                <center>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label ID="lblEmissionDate" runat="server" Text='<%# ((DateTime) Eval ("EmissionDate") > DateTime.MinValue)? Eval("EmissionDate", "{0:d}"):"" %>' />
                                                                   </div>
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>                        
                                                        <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                                            <ItemStyle Wrap="false" />
                                                            <ItemTemplate>
                                                                <center>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                                   </div>
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="InboundTrack" SortExpression="InboundTrack"
                                                            HeaderStyle-HorizontalAlign="Left">
                                                            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                            <ItemStyle Wrap="false" />
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblInboundTrack" runat="server" Text='<%# Eval ( "LatestInboundTrack.Type.Name" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status" SortExpression="Status">
                                                            <ItemStyle Wrap="false" />
                                                            <ItemTemplate>
                                                                <center>
                                                                    <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# Eval ( "Status" ) %>'
                                                                        Enabled="false" />
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Doc. Salida" AccessibleHeaderText="OutboundOrderNumber"
                                                            SortExpression="IdOutboundOrderSource"  ItemStyle-CssClass="text">
                                                            <ItemStyle Wrap="false" />
                                                            <ItemTemplate>
                                                                <center>
                                                                     <div style="word-wrap: break-word;">
                                                                        <asp:Label ID="lblOutboundOrder" runat="server" Text='<%# Eval ("OutboundOrder.Number") %>' />
                                                                     </div>
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Es Asn" AccessibleHeaderText="IsAsn" SortExpression="IsAsn">
                                                            <ItemStyle Wrap="false" />
                                                            <ItemTemplate>
                                                                <center>
                                                                    <asp:CheckBox ID="chkIsAsn" runat="server" Checked='<%# Eval ( "IsAsn" ) %>' Enabled="false" />
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="% Lpn Inspección" AccessibleHeaderText="PercentLpnInspection"
                                                            SortExpression="PercentLpnInspection">
                                                            <ItemStyle Wrap="false" />
                                                            <ItemTemplate>
                                                                <center>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label ID="lblPercentLpnInspection" runat="server" Text='<%# ((int) Eval ("PercentLpnInspection") == -1)?" ":Eval ("PercentLpnInspection") %>' />
                                                                    </div>
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="% QA" AccessibleHeaderText="PercentQA" SortExpression="PercentQA">
                                                            <ItemStyle Wrap="false" />
                                                            <ItemTemplate>
                                                                <center>
                                                                    <div style="word-wrap: break-word;">
                                                                        <asp:Label ID="lblPercentQA" runat="server" Text='<%# ((int) Eval ("PercentQA") == -1)?" ":Eval ("PercentQA") %>' />
                                                                    </div>
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="ShiftNumber" HeaderText="Turno" AccessibleHeaderText="ShiftNumber"
                                                            SortExpression="ShiftNumber" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                                            <ItemStyle Wrap="False"></ItemStyle>
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="SpecialField1" HeaderText="Campo. Esp. 1" AccessibleHeaderText="SpecialField1"
                                                            SortExpression="SpecialField1" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                                            <ItemStyle Wrap="False"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SpecialField2" HeaderText="Campo. Esp. 2" AccessibleHeaderText="SpecialField2"
                                                            SortExpression="SpecialField2" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                                            <ItemStyle Wrap="False"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SpecialField3" HeaderText="Campo. Esp. 3" AccessibleHeaderText="SpecialField3"
                                                            SortExpression="SpecialField3" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                                            <ItemStyle Wrap="False"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SpecialField4" HeaderText="Campo. Esp. 4" AccessibleHeaderText="SpecialField4"
                                                            SortExpression="SpecialField4" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                                            <ItemStyle Wrap="False"></ItemStyle>
                                                        </asp:BoundField>

                                                         <asp:TemplateField HeaderText="% Recepción" AccessibleHeaderText="PercentReceipted" SortExpression="PercentReceipted">
                                                        
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblPercentReceipted" runat="server" Text=' <%# GetFormatedNumber(((decimal) Eval ("PercentReceipted") == -1 ) ? " ": Eval ("PercentReceipted")) %>' />
                                                                </div>
                                                            </center>
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
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <%-- Panel Grilla Detalle --%>
                                <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                            <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                                <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Doc: " />
                                                <asp:Label ID="lblNroDoc" runat="server" Text="" />
                                            </div>
                                            <asp:GridView ID="grdDetail" runat="server" DataKeyNames="Id" EnableViewState="false"
                                                SkinID="grdDetail" OnRowCreated="grdDetail_RowCreated"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                AutoGenerateColumns="false"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                                        SortExpression="Id" AccessibleHeaderText="Id" />
                                                    <asp:BoundField DataField="LineNumber" HeaderText="Nº Línea" AccessibleHeaderText="LineNumber"
                                                        SortExpression="LineNumber" ItemStyle-HorizontalAlign="Center" />
                                                    <asp:BoundField DataField="LineCode" HeaderText="Cód. Línea" AccessibleHeaderText="LineCode" ItemStyle-CssClass="text"
                                                        SortExpression="LineCode" ItemStyle-HorizontalAlign="Center" />
                                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode"  ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ("Item.Code") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                        
                                                <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                                            
                                                    <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="ItemDescription"
                                                        SortExpression="ItemDescription">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval ("Item.Description") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="LotNumber" HeaderText="Lote" AccessibleHeaderText="LotNumber"
                                                        SortExpression="LotNumber" ItemStyle-HorizontalAlign="Center" />
                                                    <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CategoryItem" SortExpression="CategoryItemName">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblCategoryItemName" runat="server" Text='<%# Eval ("CategoryItem.Name") %>' />
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                        
                                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty" SortExpression="ItemQty">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblItemQty" runat="server" Text='<%# GetFormatedNumber(Eval("ItemQty")) %>' />
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Recibido" AccessibleHeaderText="Received" SortExpression="Received">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblReceived" runat="server" Text='<%# GetFormatedNumber(Eval("Received")) %>' />
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                        
                                                   <%-- <asp:BoundField DataField="ItemQty" HeaderText="Cantidad" AccessibleHeaderText="ItemQty"
                                                        SortExpression="ItemQty" />--%>
                                                    <%--<asp:BoundField DataField="Received" HeaderText="Recibido" AccessibleHeaderText="Received"
                                                        SortExpression="Received" />--%>
                                                    <asp:BoundField DataField="LineComment" HeaderText="Comentarios" AccessibleHeaderText="LineComment"
                                                        SortExpression="LineComment" />
                                            
                                                    <asp:templatefield headertext="Fifo" accessibleHeaderText="FifoDate">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:label ID="lblFifoDate" runat="server" text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                                                </div>
                                                            </center>    
                                                    </itemtemplate>
                                                    </asp:templatefield>              
                                        
                                                    <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:label ID="lblExpirationDate" runat="server" text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                                </div>
                                                            </center>    
                                                    </itemtemplate>
                                                    </asp:templatefield> 
                                        
                                                    <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:label ID="lblFabricationDate" runat="server" text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                                               </div>
                                                            </center>    
                                                    </itemtemplate>
                                                    </asp:templatefield>
                                                    <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="LPN"
                                                        SortExpression="LPN">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblLPN" runat="server" Text='<%# Eval ("LpnCode") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="Precio" AccessibleHeaderText="Price" SortExpression="Price">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblPrice" runat="server" Text=' <%# GetFormatedNumber(((decimal) Eval ("Price") == -1 )?" ": Eval ("Price")) %>' />
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="Weight" SortExpression="Weight">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblWeight" runat="server" Text=' <%# GetFormatedNumber(((decimal) Eval ("Weight") == -1 )?" ": Eval ("Weight")) %>' />
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="SpecialField1" HeaderText="Campo. Esp. 1" AccessibleHeaderText="SpecialField1"
                                                        SortExpression="SpecialField1" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                                        <ItemStyle Wrap="False"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SpecialField2" HeaderText="Campo. Esp. 2" AccessibleHeaderText="SpecialField2"
                                                        SortExpression="SpecialField2" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                                        <ItemStyle Wrap="False"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SpecialField3" HeaderText="Campo. Esp. 3" AccessibleHeaderText="SpecialField3"
                                                        SortExpression="SpecialField3" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                                        <ItemStyle Wrap="False"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SpecialField4" HeaderText="Campo. Esp. 4" AccessibleHeaderText="SpecialField4"
                                                        SortExpression="SpecialField4" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                                        <ItemStyle Wrap="False"></ItemStyle>
                                                    </asp:BoundField>

                                                     <asp:TemplateField HeaderText="Presentación por Defecto" AccessibleHeaderText="ItemUomName" SortExpression="ItemUomName">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblItemUomName" runat="server" Text='<%# Eval ("Item.ItemUom.Name") %>' />
                                                                </div>
                                                            </center>
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
