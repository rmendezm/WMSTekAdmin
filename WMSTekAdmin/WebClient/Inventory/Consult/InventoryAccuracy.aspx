<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="InventoryAccuracy.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inventory.Consult.InventoryAccuracy" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);
</script>

 <%--   <div style="width:100%;height:100%;margin:0px;margin-bottom:80px">--%>
<%--    <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default">
        <TopPanel ID="topPanel" HeightMin="50">--%>
          <%--  <Content>--%>
                <%-- Panel Grilla Principal --%>
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>  
                    <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">    
                        <asp:GridView ID="grdMgr" runat="server" 
                                DataKeyNames="Id" 
                                OnRowCreated="grdMgr_RowCreated"
                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                EnableViewState="false"                
                                AllowPaging="True" >
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" accessibleHeaderText="Id" Visible="true"/>
                                
                                <asp:TemplateField HeaderText="Cód. CD" AccessibleHeaderText="WhsCode" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <asp:Label ID="lblWhsCode" runat="server" text='<%# Eval ( "InventoryOrder.Warehouse.Code" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Nombre CD" AccessibleHeaderText="WhsName" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <asp:Label ID="lblWhsName" runat="server" text='<%# Eval ( "InventoryOrder.Warehouse.Name" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>                                
                                                              
                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnCode" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOwnCode" runat="server" text='<%# Eval ( "InventoryOrder.Owner.Code" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOwnName" runat="server" text='<%# Eval ( "InventoryOrder.Owner.Name" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                  <ItemTemplate>
                                    <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("Item.Code") %>'></asp:Label>
                                  </ItemTemplate>
                                  <ItemStyle Wrap="false"/>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Descrip. Item" AccessibleHeaderText="LongItemName" SortExpression="Description" >
                                   <ItemTemplate>
                                       <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                   </ItemTemplate>
                                </asp:TemplateField>                                  
                                
                                <asp:TemplateField HeaderText="Nº Inventario" accessibleHeaderText="IdInventoryOrder" SortExpression="IdInventoryOrder"  ItemStyle-CssClass="text">
                                    <itemtemplate>
                                       <asp:label ID="lblIdInventoryOrder" runat="server" text='<%# Eval ( "InventoryOrder.Id" ) %>' />
                                    </itemtemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>                                
                                
                                <asp:TemplateField HeaderText="Ubicación" accessibleHeaderText="IdLocCode" SortExpression="IdLocCode">
                                    <ItemTemplate>
                                        <asp:label ID="lblIdLocCode" runat="server" text='<%# Eval ( "Location.IdCode" ) %>' />
                                    </ItemTemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Tipo Ubicación" accessibleHeaderText="LocTypeName" SortExpression="LocTypeName">
                                    <ItemTemplate>
                                        <asp:label ID="lblLocTypeName" runat="server" text='<%# Eval ( "Location.Type.LocTypeName" ) %>' />
                                    </ItemTemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>                      
                            
                                <asp:BoundField DataField="itemQty" HeaderText="Cant. Real" accessibleHeaderText="InvQty" 
                                SortExpression="InvQty" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right"/>
                               
                                <asp:BoundField DataField="stockQty" HeaderText="Cant. Sist." accessibleHeaderText="StockQty" 
                                SortExpression="StockQty" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign ="Right"/>
                                                 
                                <asp:templatefield headertext="% Exactitud" accessibleHeaderText="Accuracy">
                                    <ItemStyle Wrap="false" HorizontalAlign="Right" />
                                    <itemtemplate>
                                            <asp:label ID="lblAccuracy" runat="server" text='<%# string.Format("{0:0.00}",((Decimal) Eval("stockQty") / (Decimal) Eval("itemQty") * 100)) %>' />
                                </itemtemplate>
                                </asp:templatefield>
                                
                                <asp:templatefield headertext="% Error" accessibleHeaderText="Error">
                                    <ItemStyle Wrap="false" HorizontalAlign="Right" />
                                    <itemtemplate>
                                            <asp:label ID="lblError" runat="server" text='<%# string.Format("{0:0.00}",(100 - (Decimal) Eval("stockQty") / (Decimal) Eval("itemQty") * 100)) %>' />
                                </itemtemplate>
                                </asp:templatefield>
                                <asp:TemplateField HeaderText="Cant.Comp" AccessibleHeaderText="CantComp" >
                                    <ItemTemplate>
                                        <div style="width: 60px">
                                            <center>
                                                    <asp:Image ID="InventarioOK" runat="server" />
                                            </center>
                                        </div>
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
                    </Triggers>
                </asp:UpdatePanel>  
                <%-- FIN Panel Grilla Principal --%>
           <%-- </Content>--%>
<%--        </TopPanel>
    </spl:HorizontalSplitter>--%>
<%--</div>  --%>
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label id="lblFilterDate" runat="server" Text="Recep." Visible="false" />
    <asp:Label ID="lblInventoryCode" runat="server" Text="Nro. Inventario" Visible="false" /> 	
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
<%-- Barra de Estado --%>        
<webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>