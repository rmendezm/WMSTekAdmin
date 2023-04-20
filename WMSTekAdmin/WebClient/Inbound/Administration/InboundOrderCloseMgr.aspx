<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="InboundOrderCloseMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inbound.Administration.InboundOrderCloseMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">

    function resizeDivCharts() {
        //debugger;
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("divPrincipal").style.height = h;
        document.getElementById("divPrincipal").style.width = w;
    }

    window.onresize = resizeDivCharts;

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
        initializeGridDragAndDrop('InboundOrderDetail_ById', gridDetail);
    }

</script>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="50" HeightDefault="500">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <%-- Grilla Principal --%>
                                        <div id="divGrid" runat="server" visible="true" onresize="SetDivs();" >
                                            <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id"  AutoGenerateColumns = "false"
                                                AllowPaging="True" EnableViewState="false" 
                                                OnRowCommand="grdMgr_RowCommand"
                                                OnRowCreated="grdMgr_RowCreated" 
                                                OnRowDataBound="grdMgr_RowDataBound"   
                                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" ReadOnly="True"
                                                        SortExpression="Id" AccessibleHeaderText="Id" ItemStyle-Wrap="false">
                                                        <ItemStyle Wrap="False"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Tipo Doc." AccessibleHeaderText="InboundType" SortExpression="InboundType"
                                                        HeaderStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblInboundType" runat="server" Text='<%# Eval ( "InboundType.Code" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                        <ItemStyle Wrap="False"></ItemStyle>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Number" HeaderText="Nº Doc." AccessibleHeaderText="Number"
                                                        SortExpression="Number" ItemStyle-Wrap="false">
                                                        <ItemStyle Wrap="False"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode">
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
                                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>' />
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
                                                    <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="InboundTrack" SortExpression="InboundTrack">
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
                                                    <asp:TemplateField HeaderText="Doc. salida" AccessibleHeaderText="OutboundOrderNumber"
                                                        SortExpression="OutboundOrder">
                                                        <ItemStyle Wrap="false" />
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblOutboundOrder" runat="server" Text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                                            </div>
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
                                                        SortExpression="ShiftNumber" ItemStyle-Wrap="false">
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
                                                    <asp:TemplateField ShowHeader="False" HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="width: 50px">
                                                                    <asp:ImageButton ID="btnClose" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_close.png"
                                                                    CausesValidation="false" CommandName="CloseOrder" ToolTip="Cerrar Documento"/>
                                                                </div>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <%-- FIN Grilla Principal --%>
                            
                                        <%-- Panel Cerrar Documento --%>
                            
                                        <%-- FIN Panel Cerrar Documento --%>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />  
                                         <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnCloseOrder" EventName="Click" />            
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>  
                    <%-- FIN Barra de Estado --%>
                    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$upGrid"
                        DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop"
                    CssClass="updateProgress" TargetControlID="uprGrid" />
                    
                    <%-- FIN Modal Update Progress --%>
                 </Content>
            </TopPanel>
            <BottomPanel HeightMin="50">
                <Content>    
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">            
                                 <%--   grilla detalle inicio--%>
                                 <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">       
                                        <ContentTemplate>                        
                                            <div id="divDetail" runat="server" visible="true" class="divGridDetailScroll">            
                                              <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
	                                            <asp:Label ID="lblGridDetail" runat="server" visible="false" Text="Detalle Doc: " />
	                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                              </div>
	                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
	                                                DataKeyNames="Id" EnableViewState="false"
	                                                OnRowCreated="grdDetail_RowCreated" 
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
                                                            <asp:BoundField DataField="LotNumber" HeaderText="N° Lote" AccessibleHeaderText="LotNumber"
                                                                SortExpression="LotNumber" />
                                                            <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CategoryItem" SortExpression="CategoryItemName">
                                                                <ItemTemplate>
                                                                    <center>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblCategoryItemName" runat="server" Text='<%# Eval ("CategoryItem.Name") %>' />
                                                                        </div>
                                                                    </center>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                        
                                                            <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty" SortExpression="ItemQty" >
                                                               <ItemTemplate>
                                                                   <asp:Label ID="lblItemQty" runat="server" Text='<%# GetFormatedNumber(Eval ("ItemQty")) %>' />
                                                               </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Recibido" AccessibleHeaderText="Received" SortExpression="Received">
                                                               <ItemTemplate>
                                                                   <asp:Label ID="lblReceived" runat="server" Text='<%# GetFormatedNumber(Eval ("Received")) %>' />
                                                               </ItemTemplate>
                                                            </asp:TemplateField>
                                        
                                                            <%--<asp:BoundField DataField="ItemQty" HeaderText="Cantidad" AccessibleHeaderText="ItemQty"
                                                                SortExpression="ItemQty" />
                                                            <asp:BoundField DataField="Received" HeaderText="Recibido" AccessibleHeaderText="Received"
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
                                    
                                                            <asp:TemplateField HeaderText="Precio" AccessibleHeaderText="Price" SortExpression="Price">
                                                                <ItemTemplate>
                                                                    <center>
                                                                        <div style="word-wrap: break-word;">
                                                                            <asp:Label ID="lblPrice" runat="server" Text=' <%# ((decimal) Eval ("Price") == -1 )?" ": Eval ("Price") %>' />
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
                                           <%-- <asp:AsyncPostBackTrigger ControlID="btnClose" EventName="Click" />--%>
                        
                                        </Triggers>
                                    </asp:UpdatePanel>
                                 <%--fin grilla detalle--%>
                            </div>
                        </div>  
                    </div>
                    <%--<asp:Button ID="Button1" runat="server" Text="nada" />--%>
                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
           <ContentTemplate>
               <div id="divCloseOrder" runat="server" visible="true" >
                                <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
                                <!-- Boton 'dummy' para propiedad TargetControlID -->
                                <ajaxToolkit:ModalPopupExtender ID="mpCloseOrder" runat="server" TargetControlID="btnDummy2"
                                    PopupControlID="pnlCloseOrder" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                                    Drag="true">
                                </ajaxToolkit:ModalPopupExtender>
                                <asp:Panel ID="pnlCloseOrder" runat="server"  CssClass="modalBox">
                                    <%-- Encabezado --%>
                                    <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                                        <div class="divCaption">
                                            <asp:Label ID="lblCloseOrden" runat="server" Text="Cerrar Orden" />
                                            <asp:ImageButton ID="imgbtnCloseOrder" runat="server" ToolTip="Cerrar" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                                        </div>
                                    </asp:Panel>
                                    <%-- Fin Encabezado --%>
                                    <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                                    <div class="modalControls">
                                        <div class="divCtrsFloatLeft">
                                            <div id="div3" class="divControls">
                                                <div class="fieldRight">
                                                    <asp:Label ID="Label2" runat="server" Text="Centro Dist." /></div>
                                                <div class="fieldLeft">
                                                    <b>
                                                        <asp:Label ID="lblWarehouse2" runat="server" /></b></div>
                                            </div>
                                            <div id="div4" class="divControls">
                                                <div class="fieldRight">
                                                    <asp:Label ID="Label3" runat="server" Text="Nº Doc." /></div>
                                                <div class="fieldLeft">
                                                    <b>
                                                        <asp:Label ID="lblNroDoc2" runat="server" /></b></div>
                                            </div>
                                            <div id="div5" class="divControls">
                                                <div class="fieldRight">
                                                    <asp:Label ID="Label4" runat="server" Text="Tipo Doc." /></div>
                                                <div class="fieldLeft">
                                                    <b>
                                                        <asp:Label ID="lblInboundType2" runat="server" /></b></div>
                                            </div>
                                            <div id="div6" class="divControls">
                                                <div class="fieldRight">
                                                    <asp:Label ID="Label6" runat="server" Text="Proveedor" /></div>
                                                <div class="fieldLeft">
                                                    <b>
                                                        <asp:Label ID="lblVendor2" runat="server" /></b></div>
                                            </div>
                                            <div id="div2" class="divControls">
                                                <div class="fieldRight">
                                                    <asp:Label ID="lblTraza" runat="server" Text="Traza" /></div>
                                                <div class="fieldLeft">
                                                    <asp:DropDownList ID="ddlTrackInbound" runat="server" TabIndex="9">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvTrackInbound" runat="server" ValidationGroup="Close" Text=" * " ControlToValidate="ddlTrackInbound" Display="dynamic" InitialValue="-1" />
                                                </div>
                                            </div>
                                        </div>
                                        <div>
                                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"   
                                                ShowMessageBox="false" CssClass="modalValidation"/>
                                        </div>                        
                                        <div id="div1" runat="server" class="modalActions">
                                            <asp:Button ID="btnCloseOrder" runat="server" OnClick="btnCloseOrder_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="Close" />
                                            <asp:Button ID="btnCloseOrderClose" runat="server" Text="Cancelar" />
                                        </div>                        
                                    </asp:Panel>
                            </div>
           </ContentTemplate>
           <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="RowCommand" />
           </Triggers>
        </asp:UpdatePanel>
    </div>  
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Documento?" Visible="false" />
    <asp:Label ID="lblDetailOrder" runat="server" Text="¿Ver detalle" Visible="false" />
    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Button ID="btnDetail" runat="server" Text="" OnClick="btnDetail_Click" />
    <asp:HiddenField ID="hdIndexGrd" runat="server" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
    
    <script type="text/javascript" language="javascript">
        function ViewDetail(grd) {
            var index = grd.parentElement.parentElement.parentElement.parentElement.rowIndex;
            var btnDetail = document.getElementById("ctl00$MainContent$btnDetail");
            document.getElementById('ctl00$MainContent$hdIndexGrd').value = index-1;
            
            btnDetail.click();
            return false;
        }
    </script>
    
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
