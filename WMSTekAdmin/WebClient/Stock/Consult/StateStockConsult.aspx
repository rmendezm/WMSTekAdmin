<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StateStockConsult.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Stocks.StateStockConsultPage" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("GetStateStockByFilters", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("GetStateStockByFilters", "ctl00_MainContent_grdMgr");
    }
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
                                AllowSorting="False" 
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDataBound="grdMgr_RowDataBound"
                                EnableViewState="false"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">                    
                                 <Columns>
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
                         
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>        
                                                                
                                    <asp:templatefield headertext="Cód. Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblItemCode" runat="server" text='<%# Eval ("Item.Code") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>
            
                                    <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>                             

                                    <asp:templatefield headertext="Descripción" accessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                        <itemtemplate>
                                               <asp:label ID="lblItemDescription" runat="server" text='<%# Eval ("Item.LongName") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>                                           
                                                               
                                   <asp:templatefield headertext="Cant. Disponible" accessibleHeaderText="QtyStock" SortExpression="QtyStock">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQtyStock" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("QtyStock") < 0 )? 0 : Eval ("QtyStock")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <asp:templatefield headertext="Cant. en Cont. Cíclico" accessibleHeaderText="QtyCicleCount" SortExpression="QtyCicleCount">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQtyCicleCount" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("QtyCicleCount") == -1)?" ":Eval ("QtyCicleCount")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                                                                        
                                    <asp:templatefield headertext="Cant. Reservada" accessibleHeaderText="QtyReserved" SortExpression="QtyReserved">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQtyReserved" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("QtyReserved") == -1)?" ":Eval ("QtyReserved")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <asp:templatefield headertext="Cant. Recibida" accessibleHeaderText="QtyReceived" SortExpression="QtyReceived">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQtyReceived" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("QtyReceived") == -1)?" ":Eval ("QtyReceived")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <%--<asp:templatefield headertext="Cant. Pte. Recepción" accessibleHeaderText="QtyPendingReceive" SortExpression="QtyPendingReceive">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQtyPendingReceive" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("QtyPendingReceive") == -1)?" ":Eval ("QtyPendingReceive")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>   --%>
                        
                                    <asp:templatefield headertext="Cant. en Transito" accessibleHeaderText="QtyStg" SortExpression="QtyStg">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQtyStg" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("QtyStg") == -1)?" ":Eval ("QtyStg")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <asp:templatefield headertext="Cant. en Trans. Despacho" accessibleHeaderText="QtyStgd" SortExpression="QtyStgd">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQtyStgd" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("QtyStgd") == -1)?" ":Eval ("QtyStgd")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <asp:templatefield headertext="Cant. en Trans. Recep." accessibleHeaderText="QtyStgr" SortExpression="QtyStgr">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQtyStgr" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("QtyStgr") == -1)?" ":Eval ("QtyStgr")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <asp:templatefield headertext="Cant. Pte. Liberación" accessibleHeaderText="QtyPendingPicking" SortExpression="QtyPendingPicking">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQtyPendingPicking" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("QtyPendingPicking") == -1)?" ":Eval ("QtyPendingPicking")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>

                                    <asp:TemplateField HeaderText="Cant. en Tareas Picking" AccessibleHeaderText="QtyTaskPicking" SortExpression="QtyTaskPicking">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <right>
                                                <asp:Label ID="lblQtyTaskPicking" runat="server"
                                                 text='<%#GetFormatedNumber(((decimal)Eval("QtyTaskPicking")== -1)?" ":Eval ("QtyTaskPicking")) %>' />
                                            </right>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Cant. en Simulación" AccessibleHeaderText="QtyTaskSimulation" SortExpression="QtyTaskSimulation">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <right>
                                                <asp:Label ID="lblQtyTaskSimulation" runat="server"
                                                 text='<%#GetFormatedNumber(((decimal)Eval("QtyTaskSimulation")== -1)?" ":Eval ("QtyTaskSimulation")) %>' />
                                            </right>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Cant. Bloqueda" AccessibleHeaderText="QtyHolded" SortExpression="QtyHolded">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <right>
                                                <asp:Label ID="lblQtyHolded" runat="server"
                                                 text='<%#GetFormatedNumber(((decimal)Eval("QtyHolded")== -1)?" ":Eval ("QtyHolded")) %>' />
                                            </right>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                     <asp:TemplateField HeaderText="Cant. en Andén" AccessibleHeaderText="QtyDock" SortExpression="QtyDock">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <right>
                                                <asp:Label ID="lblQtyDock" runat="server"
                                                 text='<%#GetFormatedNumber(((decimal)Eval("QtyDock")== -1)?" ":Eval ("QtyDock")) %>' />
                                            </right>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                     <asp:TemplateField HeaderText="Cant. en Camión" AccessibleHeaderText="QtyTruck" SortExpression="QtyTruck">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <right>
                                                <asp:Label ID="lblQtyTruck" runat="server"
                                                 text='<%#GetFormatedNumber(((decimal)Eval("QtyTruck")== -1)?" ":Eval ("QtyTruck")) %>' />
                                            </right>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:templatefield headertext="Cant. Total" accessibleHeaderText="QtyTotal" SortExpression="QtyTotal">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQtyTotal" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("QtyTotal") == -1)?" ":Eval ("QtyTotal")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>

                                     <asp:templatefield headertext="Presentación por Defecto" accessibleHeaderText="ItemUomName" SortExpression="ItemUomName">
                                        <itemtemplate>
                                               <asp:label ID="lblItemUomName" runat="server" text='<%# Eval ("Item.ItemUom.Name") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>        

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