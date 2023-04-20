<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="UserProductivity.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Reports.UserProductivity" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">
    //window.onresize = SetDivs;
    //var prm = Sys.WebForms.PageRequestManager.getInstance();
    //prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("GetMovementUserProductivity", "ctl00_MainContent_grdMgr");
        removeFooter("#ctl00_MainContent_grdMgr");
       // changeHeigth();
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
        initializeGridDragAndDrop("GetMovementUserProductivity", "ctl00_MainContent_grdMgr"); 
    }

    function setDivsAfter() {
        removeFooter("ctl00_MainContent_grdMgr");
        var heightDiv = $("#ctl00_MainContent_divGrid").height();
        var extraSpace = 60;
        var totalHeight = heightDiv - extraSpace;

        $("#ctl00_MainContent_divGrid").parent().css("max-height", totalHeight + "px");
    }

    //function changeHeigth()
    //{
    //    $(".froze-header-grid table-responsive").removeAttr("style");
    //    $(".froze-header-grid table-responsive").css("overflow: auto;");
    //}
</script>


    <%--<div id="divGrid" runat="server" style="width:100%;height:100%;margin:0px;margin-bottom:80px" onresize="SetDivs();>--%>
    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <%-- Panel Grilla Principal --%>
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>   
                    <div id="divGrid" runat="server" visible="true" class="divGrid">
                       
                        <asp:GridView ID="grdMgr" runat="server" 
                                
                                OnRowCreated="grdMgr_RowCreated"
                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                EnableViewState="false"                
                                AllowPaging="True" 
                                AutoGenerateColumns="false"
                                OnRowDataBound="grdMgr_RowDataBound"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                            <Columns>
<%--                                <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" accessibleHeaderText="Id" Visible="false"/>--%>
                                
                                <asp:TemplateField HeaderText="Cód.Centro Dist." accessibleHeaderText="whsCode" SortExpression="whsCode"  ItemStyle-CssClass="text" Visible="false">
                                    <itemtemplate>
                                       <asp:label ID="lblWhsCode" runat="server" text='<%# Eval ( "WhsCode" ) %>' />
                                    </itemtemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Centro Dist." accessibleHeaderText="whsName" SortExpression="whsName"  ItemStyle-CssClass="text">
                                    <itemtemplate>
                                       <asp:label ID="lblWhsName" runat="server" text='<%# Eval ( "WhsName" ) %>' />
                                    </itemtemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnCode" ItemStyle-CssClass="text" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOwnCode" runat="server" text='<%# Eval ( "OwnCode" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOwnName" runat="server" text='<%# Eval ("OwnName") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Movimiento" AccessibleHeaderText="MovementName">
                                    <ItemTemplate>
                                        <asp:Label ID="lblMovementName" runat="server" text='<%# Eval ("MovementName") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>         
                                
                               <asp:templatefield headertext="Periodo" accessibleHeaderText="Period" SortExpression="Period" ItemStyle-CssClass="text" Visible="true">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <center>
                                            <asp:label ID="lblPeriod" runat="server" text='<%# Eval ("Period") %>' />
                                        </center>    
                                    </itemtemplate>
                                </asp:templatefield>
                                
                               <asp:TemplateField HeaderText="Usuario" accessibleHeaderText="UserName" SortExpression="UserName" ItemStyle-CssClass="text">
                                  <ItemTemplate>
                                    <asp:Label ID="lblUserName" runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
                                  </ItemTemplate>
                                  <ItemStyle Wrap="false"/>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Total Pedidos" AccessibleHeaderText="TotalOrders" SortExpression="TotalOrders" ItemStyle-HorizontalAlign="Right" >
                                   <ItemTemplate>
                                       <asp:Label ID="lblTotalOrders" runat="server" Text='<%# Eval ("TotalOrders") %>' />
                                   </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Total Items" AccessibleHeaderText="TotalItems" SortExpression="TotalItems" ItemStyle-HorizontalAlign="Right" >
                                   <ItemTemplate>
                                       <asp:Label ID="lblTotalItems" runat="server" Text='<%# Eval ("TotalItems") %>' />
                                   </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Total LPN's" AccessibleHeaderText="TotalLPNs" SortExpression="TotalLPNs" ItemStyle-HorizontalAlign="Right" >
                                   <ItemTemplate>
                                       <asp:Label ID="lblTotalLPNs" runat="server" Text='<%# Eval ("TotalLPNs") %>' />
                                   </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Total Qty" AccessibleHeaderText="TotalQty" SortExpression="TotalQty" ItemStyle-HorizontalAlign="Right">
                                   <ItemTemplate>
                                       <asp:Label ID="lblTotalQty" runat="server" Text='<%# GetFormatedNumber( Eval ("TotalQty") )%>' />
                                   </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Promedio Ordenes" AccessibleHeaderText="OrdersAvg" SortExpression="OrdersAvg" ItemStyle-HorizontalAlign="Right">
                                   <ItemTemplate>
                                       <asp:Label ID="lblOrdersAvg" runat="server" Text='<%# GetFormatedNumber( Eval ("OrdersAvg") )%>' />
                                   </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Promedio Lineas" AccessibleHeaderText="LinesAvg" SortExpression="LinesAvg" ItemStyle-HorizontalAlign="Right">
                                   <ItemTemplate>
                                       <asp:Label ID="lblLinesAvg" runat="server" Text='<%# GetFormatedNumber( Eval ("LinesAvg") )%>' />
                                   </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Promedio LPN" AccessibleHeaderText="LpnsAvg" SortExpression="LpnsAvg" ItemStyle-HorizontalAlign="Right">
                                   <ItemTemplate>
                                       <asp:Label ID="lblLpnsAvg" runat="server" Text='<%# GetFormatedNumber( Eval ("LpnsAvg") )%>' />
                                   </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Promedio Cantidad Items" AccessibleHeaderText="QtyItemsAvg" SortExpression="QtyItemsAvg" ItemStyle-HorizontalAlign="Right">
                                   <ItemTemplate>
                                       <asp:Label ID="lblQtyItemsAvg" runat="server" Text='<%# GetFormatedNumber( Eval ("QtyItemsAvg") )%>' />
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
            </div>
        </div>
    </div>
            

<%--</div>--%>  
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label id="lblFilterDate" runat="server" Text="Recep." Visible="false" />    	
    <asp:Label id="lblFilterDateError" runat="server" Text="Rango de fechas no válido para periodo seleccionado" Visible="false" /> 
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
<%-- Barra de Estado --%>        
<webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>