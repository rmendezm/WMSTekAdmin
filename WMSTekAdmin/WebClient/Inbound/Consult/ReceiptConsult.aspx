<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="ReceiptConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inbound.Consult.ReceiptWebConsult" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language='Javascript'>   
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
        initializeGridDragAndDrop('Receipt_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
    }

    function setDivsAfter() {
        var heightDivBottom = $("#__hsMasterDetailRD").height();
        var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetailTitle").height();
        var extraSpace = 160;

        var totalHeight = heightDivBottom - heightLabelsBottom - extraSpace;
        $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");
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
                                            DataKeyNames="Id" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            EnableViewState="false"                
                                            AllowPaging="True" 
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                        <Columns>
                                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" accessibleHeaderText="Id"/>
                                
                                            <asp:TemplateField HeaderText="Nº Doc." accessibleHeaderText="InboundOrder" SortExpression="InboundOrder"  ItemStyle-CssClass="text">
                                                <itemtemplate>
                                                   <asp:label ID="lblInboundNumber" runat="server" text='<%# Eval ( "InboundOrder.Number" ) %>' />
                                                </itemtemplate>
                                                <ItemStyle Wrap="false"/>
                                            </asp:TemplateField>
                                
                                            <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "InboundOrder.Owner.Code" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOwnerTradeName" runat="server" text='<%# Eval ( "InboundOrder.Owner.Name" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>                
                                
                                           <asp:templatefield headertext="Recepción" accessibleHeaderText="ReceiptDate">
                                                <ItemStyle Wrap="false" />
                                                <itemtemplate>
                                                    <center>
                                                        <asp:label ID="lblReceiptDate" runat="server" text='<%# ((DateTime) Eval ("ReceiptDate") > DateTime.MinValue)? Eval("ReceiptDate", "{0:d}"):"" %>' />
                                                    </center>    
                                                </itemtemplate>
                                            </asp:templatefield>  
                                
                                            <asp:templatefield headertext="Activo" accessibleHeaderText="Status">
                                                <itemtemplate>
                                                    <center>
                                                        <asp:CheckBox ID="chkCodStatus" runat="server" checked='<%# Eval ( "Status" ) %>' Enabled="false"/>
                                                    </center>    
                                                </itemtemplate>
                                                 <ItemStyle Wrap="false"/>
                                            </asp:templatefield>
                                
                                         <asp:BoundField DataField="ReceiptTypeCode" HeaderText="Tipo Recep." 
                                                accessibleHeaderText="ReceiptTypeCode" SortExpression="ReceiptTypeCode" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>

                                         <asp:BoundField DataField="ReceiptTypeName" HeaderText="Nombre Tipo Recep." 
                                                accessibleHeaderText="ReceiptTypeName" SortExpression="ReceiptTypeName" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>                                
                                
                                            <asp:BoundField DataField="ReferenceDoc" HeaderText="Nº Ref."  ItemStyle-CssClass="text"
                                                accessibleHeaderText="ReferenceDoc" SortExpression="ReferenceDoc" ItemStyle-Wrap="false"  ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>
                                
                                            <asp:BoundField DataField="ReferenceDocTypeName" HeaderText="Docº Ref."  ItemStyle-CssClass="text"
                                                accessibleHeaderText="ReferenceDocTypeName" SortExpression="ReferenceDocTypeName" ItemStyle-Wrap="false"  ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>
                                

                                            <asp:TemplateField HeaderText="Transportista" accessibleHeaderText="Carrier" SortExpression="Carrier">
                                                <ItemTemplate>
                                                    <asp:label ID="lblNameCarrier" runat="server" text='<%# Eval ( "Carrier.Name" ) %>' />
                                                </ItemTemplate>
                                                <ItemStyle Wrap="false"/>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Camión" accessibleHeaderText="Truck" SortExpression="Truck">
                                                <ItemTemplate>
                                                    <asp:label ID="lblIdTruck" runat="server" text='<%# Eval ( "Truck.IdCode" ) %>' />
                                                </ItemTemplate>
                                                <ItemStyle Wrap="false"/>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Cód. Chofer" accessibleHeaderText="DriverCode" SortExpression="DriverCode" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:label ID="lblDriverCode" runat="server" text='<%# Eval ( "Driver.Code" ) %>' />
                                                </ItemTemplate>                
                                                <ItemStyle Wrap="false"/>
                                            </asp:TemplateField>
                                
                                            <asp:TemplateField HeaderText="Chofer" accessibleHeaderText="Driver" SortExpression="Driver">
                                                <ItemTemplate>
                                                    <asp:label ID="lbldriverName" runat="server" text='<%# Eval ( "Driver.Name" ) %>' />
                                                </ItemTemplate>                
                                                <ItemStyle Wrap="false"/>
                                            </asp:TemplateField>
                                
                                            <asp:BoundField DataField="ShiftNumber" HeaderText="Turno" ItemStyle-Wrap="false"
                                                accessibleHeaderText="ShiftNumber" SortExpression="ShiftNumber">
                                            </asp:BoundField>
                                
                                            <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text">
                                                <itemtemplate>
                                                   <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                </itemtemplate>
                                             </asp:templatefield>
                                                 
                                            <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                                <itemtemplate>
                                                   <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                </itemtemplate>
                                             </asp:templatefield>
                                
                                            <asp:BoundField DataField="UserWms" HeaderText="Operador" ItemStyle-Wrap="false"
                                                accessibleHeaderText="UserWms" SortExpression="UserWms">
                                            </asp:BoundField>

                                           <%-- <asp:BoundField DataField="SpecialField1" HeaderText="Contenedor" ItemStyle-Wrap="false"
                                                accessibleHeaderText="SpecialField1" SortExpression="SpecialField1">
                                            </asp:BoundField>--%>

                                            <asp:TemplateField HeaderText="Observación" AccessibleHeaderText="SpecialField1">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Eval ( "InboundOrder.SpecialField1" ) %>'></asp:Label>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            
                                            <asp:TemplateField HeaderText="Campo. Esp. 1" AccessibleHeaderText="SpecialField1">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="SpecialField1" runat="server" Text='<%# Eval ( "SpecialField1" ) %>'></asp:Label>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>


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
                                    <div id="divDetail" runat="server" visible="true" class="divGridDetailScroll">            
                                      <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
	                                    <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Doc: " />
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
                                           <asp:TemplateField HeaderText="Doc. Entrada" accessibleHeaderText="InboundOrder" SortExpression="InboundOrder"  ItemStyle-CssClass="text">
                                              <ItemTemplate>
                                                <asp:Label ID="lblIdInboundOrdert" runat="server" Text='<%# Eval("InboundOrder.Number") %>'></asp:Label>
                                              </ItemTemplate>
                                               <ItemStyle Wrap="false"/>
                                            </asp:TemplateField>
                                
                                           <asp:TemplateField HeaderText="Entrada" accessibleHeaderText="ReceiptDate" SortExpression="Receipt">
                                              <ItemTemplate>
                                                <asp:label ID="lblReceiptDate" runat="server" text='<%# ((DateTime) Eval ("Receipt.ReceiptDate") > DateTime.MinValue)? Eval("Receipt.ReceiptDate", "{0:d}"):"" %>' />                                    
                                              </ItemTemplate>
                                              <ItemStyle Wrap="false"/>
                                            </asp:TemplateField>
                              
                                            <asp:TemplateField HeaderText="Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                              <ItemTemplate>
                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("Item.Code") %>'></asp:Label>
                                              </ItemTemplate>
                                              <ItemStyle Wrap="false"/>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                               <ItemTemplate>
                                                   <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                               </ItemTemplate>
                                            </asp:TemplateField>                                

                                            <asp:TemplateField HeaderText="Descripción"  AccessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                              <ItemTemplate>
                                                <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Item.Description") %>'></asp:Label>
                                              </ItemTemplate>
                                              <ItemStyle Wrap="false"/>
                                            </asp:TemplateField>
                                
                                            <asp:TemplateField HeaderText="Categoría" accessibleHeaderText="CategoryItem" SortExpression="CategoryItem">
                                              <ItemTemplate>
                                                <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval("CategoryItem.Name") %>'></asp:Label>
                                              </ItemTemplate>
                                              <ItemStyle Wrap="false"/>
                                            </asp:TemplateField>
                                
                                            <%--<asp:BoundField DataField="Received" HeaderText="Recibido" accessibleHeaderText="Received" 
                                            SortExpression="Received" ItemStyle-Wrap="false"/>--%>
                                            <asp:TemplateField HeaderText="Recibido" AccessibleHeaderText="Received" SortExpression="Received">
                                                <ItemTemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblReceived" runat="server" Text='<%# GetFormatedNumber(Eval("Received")) %>' />
                                                        </div>
                                                    </center>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                            
                                            <asp:templatefield headertext="Fifo" accessibleHeaderText="FifoDate">
                                                <ItemStyle Wrap="false" />
                                                <itemtemplate>
                                                    <center>
                                                        <asp:label ID="lblFifoDate" runat="server" text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                                    </center>    
                                            </itemtemplate>
                                            </asp:templatefield>              
                                
                                            <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate">
                                                <ItemStyle Wrap="false" />
                                                <itemtemplate>
                                                    <center>
                                                        <asp:label ID="lblExpirationDate" runat="server" text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                    </center>    
                                            </itemtemplate>
                                            </asp:templatefield> 
                                
                                            <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate">
                                                <ItemStyle Wrap="false" />
                                                <itemtemplate>
                                                    <center>
                                                        <asp:label ID="lblFabricationDate" runat="server" text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                                    </center>    
                                            </itemtemplate>
                                            </asp:templatefield>    
                                            <asp:BoundField DataField="LotNumber" HeaderText="Lote"  accessibleHeaderText="LotNumber" 
                                            SortExpression="LotNumber" ItemStyle-Wrap="false"  ItemStyle-CssClass="text"/>
                                
                                            <asp:templatefield headertext="LPN" accessibleHeaderText="LpnCode">
                                                <ItemStyle Wrap="false" />
                                                <itemtemplate>
                                                    <center>
                                                        <asp:label ID="lblLpnCode" runat="server" text='<%# Eval ("LPN.IdCode") %>' />
                                                    </center>    
                                            </itemtemplate>
                                            </asp:templatefield> 

                                            <asp:TemplateField HeaderText="Presentación por Defecto" AccessibleHeaderText="ItemUomName" SortExpression="ItemUomName">
                                               <ItemTemplate>
                                                   <asp:Label ID="lblItemUomName" runat="server" Text='<%# Eval ("Item.ItemUom.Name") %>' />
                                               </ItemTemplate>
                                            </asp:TemplateField>     

                                            <asp:templatefield headertext="Peso" accessibleHeaderText="LpnWeightTotal" SortExpression="LpnWeightTotal">
                                                <ItemStyle Wrap="false" />
                                                <itemtemplate>
                                                    <right>
                                                        <asp:label ID="lblLpnWeightTotal" runat="server" 
                                                        text='<%# GetFormatedNumber(((decimal) Eval ("LpnWeightTotal") == -1)?" ":Eval ("LpnWeightTotal")) %>' />
                                                    </right>    
                                                </itemtemplate>
                                            </asp:templatefield>

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
    <asp:Label id="lblFilterDate" runat="server" Text="Recep." Visible="false" />    	
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
<%-- Barra de Estado --%>        
<webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>
