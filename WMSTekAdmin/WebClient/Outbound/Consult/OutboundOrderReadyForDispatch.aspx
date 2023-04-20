<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="OutboundOrderReadyForDispatch.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.OutboundOrderReadyForDispatch" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    function resizeDiv() {
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
        initializeGridDragAndDrop('ReadyForDispatch_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
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
                                        AllowPaging="True"
                                        EnableViewState="False"
                                        AutoGenerateColumns="false"
                                        OnRowDataBound="grdMgr_RowDataBound"
                                        CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                        EnableTheming="false">
                                    <Columns>
                            
                                        <asp:templatefield HeaderText="ID Doc Salida" AccessibleHeaderText="IdOutboundOrder" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                               <asp:label ID="lblIdOutboundOrder" runat="server" text='<%# Eval ( "Id" ) %>' />
                                            </itemtemplate>
                                         </asp:templatefield>
                            
                                        <asp:templatefield HeaderText="ID Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                               <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Outbound.Warehouse.Id" ) %>' />
                                            </itemtemplate>
                                         </asp:templatefield>
                                             
                                        <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                            <itemtemplate>
                                               <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Outbound.Warehouse.Name" ) %>' />
                                            </itemtemplate>
                                         </asp:templatefield>
                             
                                        <asp:TemplateField HeaderText="ID Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Outbound.Owner.Id" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Outbound.Owner.Name" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                
                                        <asp:TemplateField AccessibleHeaderText="OutboundNumber" HeaderText="Nro Documento">
                                            <ItemTemplate>
                                                <asp:label ID="lblOutboundNumber" runat="server" text='<%# Eval ( "Outbound.Number" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                           
                                        <asp:TemplateField AccessibleHeaderText="IdOutboundType" HeaderText="ID Tipo Docto">
                                            <ItemTemplate>
                                                <asp:label ID="lblOutboundTypeId" runat="server" text='<%# ((int)Eval ( "Outbound.OutboundType.Id" )== -1)?"":Eval ( "Outbound.OutboundType.Id" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="OutboundTypeName" HeaderText="Nombre Docto">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "Outbound.OutboundType.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="IdTrackOutboundType" HeaderText="ID Traza">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTrackOutboundType" runat="server" Text='<%# ((int)Eval ( "Outbound.LatestOutboundTrack.Type.Id" )== -1)?"":Eval ( "Outbound.LatestOutboundTrack.Type.Id" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="NameTrackOutboundType" HeaderText="Traza">
                                            <ItemTemplate>
                                                <asp:Label ID="lblNameTrackOutboundType" runat="server" Text='<%# Eval ( "Outbound.LatestOutboundTrack.Type.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="Request" HeaderText="Solicitados">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRequest" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval ( "totRequest" )== -1)?"":Eval ( "totRequest" )) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="Prepared" HeaderText="Preparados">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPrepared" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval ( "totPicking" )== -1)?"":Eval ( "totPicking" )) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="Diference" HeaderText="Diferencia">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDiference" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval ( "totDiference" )== -1)?"":Eval ( "totDiference" )) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                        
                                        <asp:TemplateField AccessibleHeaderText="EmissionDate" HeaderText="Emitido">
                                            <ItemTemplate>
                                                <asp:Label ID="lblEmissionDate" runat="server" Text='<%# ((DateTime) Eval ("Outbound.EmissionDate") > DateTime.MinValue)? Eval("Outbound.EmissionDate", "{0:d}"):"" %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="ExpectedDate" HeaderText="Compromiso">
                                            <ItemTemplate>
                                                <asp:Label ID="lblExpectedDate" runat="server" Text='<%# ((DateTime) Eval ("Outbound.ExpectedDate") > DateTime.MinValue)? Eval("Outbound.ExpectedDate", "{0:d}"):"" %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="CustomerCode" HeaderText="Codigo Cliente">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ( "Outbound.CustomerCode" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="CustomerName" HeaderText="Cliente">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "Outbound.CustomerName" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Sucursal" AccessibleHeaderText="Branch" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblbranchName" runat="server" text='<%# Eval ( "Outbound.Branch.Name" ) %>'/>
                                                </div>  
                                            </itemtemplate>
                                        </asp:TemplateField>  
                            
                                        <asp:TemplateField AccessibleHeaderText="DeliveryAddress1" HeaderText="Direccion Entrega">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDeliveryAddress1" runat="server" Text='<%# Eval ( "Outbound.DeliveryAddress1" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="IdCountryDelivery" HeaderText="ID Pais">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdCountryDelivery" runat="server" Text='<%# ((int)Eval ( "Outbound.CountryDelivery.Id" )== -1)?"":Eval ( "Outbound.CountryDelivery.Id" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="CountryName" HeaderText="Pais">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCountryName" runat="server" Text='<%# Eval ( "Outbound.CountryDelivery.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="IdStateDelivery" HeaderText="ID Region">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdStateDelivery" runat="server" Text='<%# ((int)Eval ( "Outbound.StateDelivery.Id" )== -1)?"":Eval ( "Outbound.StateDelivery.Id" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="StateName" HeaderText="Region">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStateName" runat="server" Text='<%# Eval ( "Outbound.StateDelivery.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="IdCityDelivery" HeaderText="ID Ciudad">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdCityDelivery" runat="server" Text='<%# ((int)Eval ( "Outbound.CityDelivery.Id" )== -1)?"":Eval ( "Outbound.CityDelivery.Id" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="CityName" HeaderText="Ciudad">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCityName" runat="server" Text='<%# Eval ( "Outbound.CityDelivery.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="CarrierCode" HeaderText="Transporte">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCarrierCode" runat="server" Text='<%# Eval ( "Outbound.Carrier.Code" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="RouteCode" HeaderText="Ruta">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRouteCode" runat="server" Text='<%# Eval ( "Outbound.RouteCode" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                                                    
                                        <asp:TemplateField AccessibleHeaderText="SpecialField1" HeaderText="Campo. Esp. 1">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Eval ( "Outbound.SpecialField1" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="SpecialField2" HeaderText="Campo. Esp. 2">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField2" runat="server" Text='<%# Eval ( "Outbound.SpecialField2" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="SpecialField3" HeaderText="Campo. Esp. 3">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField3" runat="server" Text='<%# Eval ( "Outbound.SpecialField3" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="SpecialField4" HeaderText="Campo. Esp. 4">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField4" runat="server" Text='<%# Eval ( "Outbound.SpecialField4" ) %>' />
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
	                                    <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                      </div>
                                        <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                            DataKeyNames="Id" 
                                            EnableViewState="False" 
                                            AutoGenerateColumns="False"
                                            OnRowCreated="grdDetail_RowCreated"
                                            OnRowDataBound="grdDetail_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                       
                                            <Columns>
                                 
                                                <asp:TemplateField HeaderText="ID Centro" AccessibleHeaderText="IdWhsDetail" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdWhsDetail" runat="server" text='<%# Eval ( "Outbound.Warehouse.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                    
                                                <asp:TemplateField HeaderText="ID Detalle" AccessibleHeaderText="IdOutboundDetail" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdOutboundDetail" runat="server" text='<%# Eval ( "OutboundDetail.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                    
                                                <asp:TemplateField HeaderText="ID Documento" AccessibleHeaderText="IdOutboundOrderDetail" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdOutboundOrderDetail" runat="server" text='<%# Eval ( "Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                    
                                                <asp:TemplateField HeaderText="Nro Linea" AccessibleHeaderText="LineNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLineNumber" runat="server" text='<%# ((int)Eval ( "OutboundDetail.LineNumber" )== -1)?"":Eval ( "OutboundDetail.LineNumber" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                    
                                                <asp:TemplateField HeaderText="ID Item" AccessibleHeaderText="IdItem" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdItem" runat="server" text='<%# Eval ( "OutboundDetail.Item.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                                                                            
                                                <asp:TemplateField HeaderText="Codigo Item" AccessibleHeaderText="ItemCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "OutboundDetail.Item.Code" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                    
                                                <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName" SortExpression="LongItemName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ("OutboundDetail.Item.LongName") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                                    
                                                <asp:TemplateField HeaderText="Solicitado" AccessibleHeaderText="ItemQty">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(((decimal)Eval ( "OutboundDetail.ItemQty" )== -1)?"":Eval ( "OutboundDetail.ItemQty" )) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                   
                                                <asp:TemplateField HeaderText="Preparado" AccessibleHeaderText="Prepared">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPrepared" runat="server" text='<%# GetFormatedNumber(((decimal)Eval ( "PickPending" )== -1)?"":Eval ( "PickPending" )) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                    
                                                <asp:TemplateField HeaderText="Diferencia" AccessibleHeaderText="DiferenceDetail">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDiferenceDetail" runat="server" text='<%# GetFormatedNumber(((decimal)Eval ( "StockInPick" )== -1)?"":Eval ( "StockInPick" )) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                                                                                                                 
                                                <asp:TemplateField AccessibleHeaderText="SpecialField1" HeaderText="Campo. Esp. 1">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Eval ( "OutboundDetail.SpecialField1" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SpecialField2" HeaderText="Campo. Esp. 2">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSpecialField2" runat="server" Text='<%# Eval ( "OutboundDetail.SpecialField2" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SpecialField3" HeaderText="Campo. Esp. 3">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSpecialField3" runat="server" Text='<%# Eval ( "OutboundDetail.SpecialField3" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SpecialField4" HeaderText="Campo. Esp. 4">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSpecialField4" runat="server" Text='<%# Eval ( "OutboundDetail.SpecialField4" ) %>' />
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
    <asp:Label id="lblFilterDate" runat="server" Text="Fec Entrega" Visible="false" /> 
    <asp:Label ID="lblDescription" runat="server" Text="Nombre Cliente" Visible="false" />
    <asp:Label ID="lblName" runat="server" Text="Cod. Cliente" Visible="false" />   	
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>