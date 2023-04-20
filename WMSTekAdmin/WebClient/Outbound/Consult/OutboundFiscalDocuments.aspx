<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="OutboundFiscalDocuments.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.OutboundFiscalDocuments" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" language="javascript">
        window.onresize = resizeDivPrincipal; 
    
        function resizeDivPrincipal() {
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("ctl00_MainContent_divMainPrincipal").style.height = h;
            document.getElementById("ctl00_MainContent_divMainPrincipal").style.width = w;
        }

        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_bottomPanel_ctl01_grdDetail';
            clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('Header_FiscalDocs', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }
    </script>

    <div runat="server" id="divMainPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
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
                                            AllowPaging="True"
                                            EnableViewState="False"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>
                                                <asp:templatefield HeaderText="ID Despacho" AccessibleHeaderText="IdDispatch" >
                                                    <itemtemplate>
                                                       <asp:label ID="lblIdDispacth" runat="server" text='<%# Eval ( "Id" ) %>' />
                                                    </itemtemplate>
                                                 </asp:templatefield>
                                             
                                                <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                                    <itemtemplate>
                                                       <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.Name" ) %>' />
                                                    </itemtemplate>
                                                 </asp:templatefield>

                                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "OutboundOrder.Owner.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="OutboundTypeName" HeaderText="Tipo Documento">
                                                    <ItemTemplate>
                                                        <asp:label ID="lblOutboundTypeName" runat="server" text='<%# Eval ( "OutboundOrder.OutboundType.Name" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>    
                                
                                                <asp:TemplateField AccessibleHeaderText="OutboundNumber" HeaderText="Nro Documento">
                                                    <ItemTemplate>
                                                        <asp:label ID="lblOutboundNumber" runat="server" text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="NumberReferenceDoc" HeaderText="Nro Documento Ref.">
                                                    <ItemTemplate>
                                                        <asp:label ID="lblReferenceDoc" runat="server" text='<%# Eval ( "ReferenceDoc.ReferenceDocNumber" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="ReferenceDoc" HeaderText="Documento Ref.">
                                                    <ItemTemplate>
                                                        <asp:label ID="lblReferenceDoc" runat="server" text='<%# Eval ( "ReferenceDoc.ReferenceDocType.Name" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="CustomerCode" HeaderText="Codigo Cliente">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ( "Customer.Code" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                            
                                                <asp:TemplateField AccessibleHeaderText="CustomerName" HeaderText="Cliente">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "Customer.Name" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="DateCreated" HeaderText="Fecha Creación">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDateCreated" runat="server" Text='<%# ((DateTime) Eval ("DateCreated") > DateTime.MinValue)? Eval("DateCreated", "{0:d}"):"" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                            
                                                <asp:TemplateField AccessibleHeaderText="UserCreated" HeaderText="User">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblUserCreated" runat="server" Text='<%# Eval ("UserCreated") %>' />
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

                    <asp:UpdateProgress ID="uprSelectedOrders" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprSelectedOrders" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprSelectedOrders" />
                </Content>
            </TopPanel>
            <BottomPanel ID="bottomPanel" HeightMin="50">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <%-- Panel Grilla Detalle --%>
                                <asp:UpdatePanel ID="upGridDetail" runat="server"  UpdateMode="Conditional">
                                    <ContentTemplate>    
                                        <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                              
                                            <div id="divDetailTitle" runat="server">

                                                <div id="divGrid" runat="server" class="textLeft"> 
                                                    <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                        DataKeyNames="Id" 
                                                        EnableViewState="False" 
                                                        AutoGenerateColumns="False"
                                                        OnRowCreated="grdDetail_RowCreated"
                                                        OnRowDataBound="grdDetail_RowDataBound"
                                                        CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                        EnableTheming="false"> 
                                                        <Columns>
                                                            <asp:templatefield HeaderText="ID" AccessibleHeaderText="IdDispatchDetail" >
                                                                <itemtemplate>
                                                                   <asp:label ID="lblIdDispatchDetail" runat="server" text='<%# Eval ( "Id" ) %>' />
                                                                </itemtemplate>
                                                             </asp:templatefield>

                                                            <asp:TemplateField HeaderText="Nro Linea" AccessibleHeaderText="LineNumber" >
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLineNumber" runat="server" text='<%# Eval ( "LineNumber" ) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="ID Item" AccessibleHeaderText="IdItem" >
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblIdItem" runat="server" text='<%# Eval ( "Item.Id" ) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                                                                            
                                                            <asp:TemplateField HeaderText="Codigo Item" AccessibleHeaderText="ItemCode" >
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                    
                                                            <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName" SortExpression="LongItemName">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(((decimal)Eval ( "Qty" )== -1)?"":Eval ( "Qty" )) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Cod. Barra" AccessibleHeaderText="BarCode" SortExpression="BarCode">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblBarCode" runat="server" Text='<%# Eval ("ItemUom.BarCode") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" >
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "LotNumber" ) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                    
                                                            <asp:TemplateField HeaderText="F. Fifo" AccessibleHeaderText="Fifo" >
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblFifo" runat="server" text='<%# (((DateTime)Eval ( "Fifo" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Fifo", "{0:d}" )) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="F. Expiración" AccessibleHeaderText="Expiration" >
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblExpiration" runat="server" text='<%# (((DateTime)Eval ( "Expiration" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Expiration", "{0:d}" )) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                    
                                                            <asp:TemplateField HeaderText="F. Fabricación" AccessibleHeaderText="Fabrication" >
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblFabrication" runat="server" text='<%# (((DateTime)Eval ( "Fabrication" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Fabrication", "{0:d}" )) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                    
                                                            <asp:TemplateField HeaderText="Lpn" AccessibleHeaderText="Lpn" >
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLpn" runat="server" text='<%# Eval ( "Lpn.Code" ) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                    
                                                            <asp:TemplateField HeaderText="Cod. Tipo Lpn" AccessibleHeaderText="LpnTypeCode" >
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLpnTypeCode" runat="server" text='<%# Eval ( "Lpn.LPNType.Code" ) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                    
                                                            <asp:TemplateField HeaderText="Nom. Tipo Lpn" AccessibleHeaderText="LpnTypeName" >
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLpnTypeName" runat="server" text='<%# Eval ( "Lpn.LPNType.Name" ) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>    
                                                        </Columns>     
                                                    </asp:GridView>              
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
                            </div>
                        </div>
                    </div>

                    <asp:UpdateProgress ID="uprSelectedOrdersDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprSelectedOrdersDetail" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprSelectedOrdersDetail" />
                                
                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>
     </div>

    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label ID="lblReferenceDoc" runat="server" Text="Doc. Referencia" Visible="false" /> 
    <asp:Label ID="lblName" runat="server" Text="Cod. Cliente" Visible="false" /> 
    <asp:Label ID="lblDescription" runat="server" Text="Nombre Cliente" Visible="false" />
    <asp:Label id="lblFilterDate" runat="server" Text="Creación" Visible="false" />   
    <asp:Label id="lblDetail" runat="server" Text="Creación" Visible="false" />   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>

