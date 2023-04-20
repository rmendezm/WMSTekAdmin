<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaxAndMinConsult.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Stocks.MaxAndMinConsult" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>  
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                         <%-- Grilla Principal --%>         
                         <asp:GridView ID="grdMgr" 
                            runat="server" 
                            AllowPaging="True" 
                            OnRowCreated="grdMgr_RowCreated"
                            EnableViewState="False" onrowdatabound="grdMgr_RowDataBound"
                            AutoGenerateColumns="false"
                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                            EnableTheming="false">
                
                             <Columns>
                 
                                <asp:templatefield HeaderText="Cód.Centro" AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text">
                                    <itemtemplate>
                                       <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Stock.Warehouse.Code" ) %>' />
                                    </itemtemplate>
                                 </asp:templatefield>
                                     
                                <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                    <itemtemplate>
                                       <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Stock.Warehouse.ShortName" ) %>' />
                                    </itemtemplate>
                                 </asp:templatefield>
                                      
                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOwnerName" runat="server" text='<%# Eval ( "Item.Owner.Code" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                    
                                 <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>          
                                     
                                 <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOwnerTradeName" runat="server" text='<%# Eval ( "Item.Owner.TradeName" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>                    
                     
                                <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                    <ItemTemplate>
                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>                          
                  
                                <asp:templatefield headertext="Descripción" accessibleHeaderText="Description" SortExpression="Description">
                                    <itemtemplate>
                                           <asp:label ID="lblCategoryItemCode" runat="server" text='<%# Eval ("Item.Description") %>' />
                                    </itemtemplate>
                                </asp:templatefield>
                    
                                 <asp:templatefield headertext="Mínimo" accessibleHeaderText="ReOrderPoint" SortExpression="ReOrderPoint">
                                    <itemtemplate>
                                           <asp:label ID="lblReOrderPoint" runat="server" text='<%# GetFormatedNumber(Eval ("Item.ReOrderPoint")) %>' />
                                    </itemtemplate>
                                </asp:templatefield>  
                    
                                <asp:templatefield headertext="Máximo" accessibleHeaderText="ReOrderQty" SortExpression="ReOrderQty">
                                    <itemtemplate>
                                           <asp:label ID="lblReOrderQty" runat="server" text='<%# GetFormatedNumber(Eval ("Item.ReOrderQty")) %>' />
                                    </itemtemplate>
                                </asp:templatefield>   
                    
                                <asp:templatefield headertext="Nivel Parámetro" accessibleHeaderText="LevelConfig" SortExpression="LevelConfig">
                                    <itemtemplate>
                                           <asp:label ID="lblLevelConfig" runat="server" text='<%# Eval ("LevelConfig") %>' />
                                    </itemtemplate>
                                </asp:templatefield> 
                    
                                <asp:templatefield headertext="Ubicación" accessibleHeaderText="IdLocCode" SortExpression="IdLocCode" ItemStyle-CssClass="text">
                                    <itemtemplate>
                                           <asp:label ID="lblIdLocCode" runat="server" text='<%# Eval ("Stock.Location.IdCode") %>' />
                                    </itemtemplate>
                                </asp:templatefield>
                    
                                <asp:templatefield headertext="En Picking" accessibleHeaderText="ItemQty" SortExpression="ItemQty">
                                    <itemtemplate>
                                        <asp:label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("Stock.Qty")== -1) ? " " : Eval("Stock.Qty"))%>'></asp:label>
                                    </itemtemplate>
                                </asp:templatefield>
                                         
                                <asp:templatefield headertext="Por Reponer" accessibleHeaderText="Replenishment" SortExpression="Replenishment">
                                    <itemtemplate>
                                           <asp:label ID="lblReplenishment" runat="server" text= '<%# GetFormatedNumber(((decimal)Eval("Replenishment") == -1) ? " " : Eval("Replenishment"))%>'/>
                                    </itemtemplate>
                                </asp:templatefield>
                    
                                <asp:templatefield headertext="En Putaway" accessibleHeaderText="PutawayQty" SortExpression="PutawayQty">
                                    <itemtemplate>
                                           <asp:label ID="lblPutawayQty" runat="server" text='<%# GetFormatedNumber(((decimal)Eval("PutawayQty") == -1) ? " " : Eval("PutawayQty"))%>' />
                                    </itemtemplate>
                                </asp:templatefield>
                    
                                    <%--<asp:templatefield headertext="Nº Doc" accessibleHeaderText="InboundNumber" SortExpression="InboundNumber">
                                        <itemtemplate>
                                               <asp:label ID="lblInboundNumber" runat="server" text='<%# Eval ("Stock.InboundOrder.Number") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <asp:templatefield headertext="Fifo" accessibleHeaderText="Fifo" SortExpression="Fifo">
                                        <itemtemplate>
                                               <asp:label ID="lblFifo" runat="server" text='<%# Eval ("Stock.FifoDate") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>                         
                    
                                <asp:templatefield headertext="Sector" accessibleHeaderText="GrpItem1" SortExpression="GrpItem1">
                                    <itemtemplate>
                                           <asp:label ID="lblGrpItem1" runat="server" text='<%# Eval ("Item.GrpItem1.Name") %>' />
                                    </itemtemplate>
                                </asp:templatefield>
                    
                                <asp:templatefield headertext="Rubro" accessibleHeaderText="GrpItem2" SortExpression="GrpItem2">
                                    <itemtemplate>
                                           <asp:label ID="lblGrpItem2" runat="server" text='<%# Eval ("Item.GrpItem2.Name") %>' />
                                    </itemtemplate>
                                </asp:templatefield>
                    
                                <asp:templatefield headertext="Familia" accessibleHeaderText="GrpItem3" SortExpression="GrpItem3">
                                    <itemtemplate>
                                           <asp:label ID="lblGrpItem3" runat="server" text='<%# Eval ("Item.GrpItem3.Name") %>' />
                                    </itemtemplate>
                                </asp:templatefield>        
                    
                                <asp:templatefield headertext="Subfamilia" accessibleHeaderText="GrpItem4" SortExpression="GrpItem4">
                                    <itemtemplate>
                                           <asp:label ID="lblGrpItem4" runat="server" text='<%# Eval ("Item.GrpItem4.Name") %>' />
                                    </itemtemplate>
                                </asp:templatefield>  --%> 

                                <asp:templatefield headertext="Activo" accessibleHeaderText="Status" SortExpression="Status">
                                    <itemtemplate>
                                            <asp:CheckBox ID="chkStatus" Enabled="false" runat="server" Checked ='<%# Eval ("Item.Status") %>' />
                                    </itemtemplate>
                                </asp:templatefield>   
                    
                                <asp:templatefield headertext="Días Vigencia" accessibleHeaderText="ShelfLife" SortExpression="ShelfLife">
                                    <itemtemplate>
                                           <asp:label ID="lblShelfLife" runat="server" text='<%# ((int) Eval ("Item.ShelfLife") == -1 )?" ": Eval ("Item.ShelfLife") %>' />
                                    </itemtemplate>
                                </asp:templatefield>  
                      
                                <asp:templatefield headertext="Días Vencimiento" accessibleHeaderText="ExpirationDays" SortExpression="ExpirationDays">
                                    <itemtemplate>
                                           <asp:label ID="lblExpirationDays" runat="server" text='<%# ((int) Eval ("Item.Expiration") == -1 )?" ": Eval ("Item.Expiration") %>' />
                                    </itemtemplate>
                                </asp:templatefield>   
                                 
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
                         <%-- FIN Grilla Principal --%>             
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
            </div>
        </div>
    </div>

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%>             

    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label id="lblFilterDate" runat="server" Text="Fifo" Visible="false" />   
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>