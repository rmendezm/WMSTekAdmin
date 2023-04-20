<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="GenerateAsnFileImperial.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Clientes.GenerateAsnFileImperial" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
//    function resizeDiv() {
//        var h = document.body.clientHeight + "px";
//        var w = document.body.clientWidth + "px";
//        document.getElementById("divPrincipal").style.height = h;
//        document.getElementById("divPrincipal").style.width = w;
//    }
    window.onresize = SetDivs;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("ImperialB2B_GetOrdersDispatch", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("ImperialB2B_GetOrdersDispatch", "ctl00_MainContent_grdMgr");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>  
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" runat="server" 
                                    OnRowCreated="grdMgr_RowCreated"
                                    AllowPaging="True"
                                    EnableViewState="False"
                                    AutoGenerateColumns="false"
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="OutboundNumber" HeaderText="Nro Orden de Compra" AccessibleHeaderText="OutboundNumber" SortExpression="OutboundNumber"  ItemStyle-CssClass="text"/>
                                    <asp:BoundField DataField="NroGuia" HeaderText="Nro Guía Proveedor" AccessibleHeaderText="NroGuia" SortExpression="NroGuia"  ItemStyle-CssClass="text"/>
                                    <asp:BoundField DataField="Store" HeaderText="Tienda" AccessibleHeaderText="Store" SortExpression="Store"  ItemStyle-CssClass="text"/>
                                    <asp:BoundField DataField="Lpn" HeaderText="Lpn" AccessibleHeaderText="Lpn" SortExpression="Lpn"  ItemStyle-CssClass="text"/>
                                    <asp:BoundField DataField="ItemCode" HeaderText="Código Producto Imperial" AccessibleHeaderText="ItemCode" SortExpression="ItemCode"  ItemStyle-CssClass="text"/>
                                    <asp:BoundField DataField="ItemCodeCustomer" HeaderText="Código Producto Proveedor" AccessibleHeaderText="ItemCodeCustomer" SortExpression="ItemCodeCustomer"  ItemStyle-CssClass="text"/>
                                    <asp:BoundField DataField="EAN13" HeaderText="EAN 13" AccessibleHeaderText="EAN13" SortExpression="EAN13"  ItemStyle-CssClass="text"/>
                                    <asp:BoundField DataField="UOMCode" HeaderText="Unidad Medida" AccessibleHeaderText="UOMCode" SortExpression="UOMCode"  ItemStyle-CssClass="text"/>
                                    <asp:TemplateField HeaderText="Cantidad Entrega" AccessibleHeaderText="ItemQty">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(((decimal)Eval ( "ItemQty" )== -1)?"":Eval ( "ItemQty" )) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:BoundField DataField="ItemQty" HeaderText="Cantidad Entrega" AccessibleHeaderText="ItemQty" SortExpression="ItemQty"  ItemStyle-CssClass="text"/>--%>
                                    <asp:BoundField DataField="LoteNumber" HeaderText="Lote" AccessibleHeaderText="LoteNumber" SortExpression="LoteNumber"  ItemStyle-CssClass="text"/>
                                    <asp:BoundField DataField="DeliveryAddress" HeaderText="Dirección Despacho" AccessibleHeaderText="DeliveryAddress" SortExpression="DeliveryAddress"  ItemStyle-CssClass="text"/>
                                </Columns>        
                            </asp:GridView>                
            
                        </div>
                   </ContentTemplate>
                   <Triggers>
                    <%--<asp:PostBackTrigger ControlID="btnPrint" />--%>
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
    
    
        <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label ID="lblToolTipASN" runat="server" Text="Generar ASN" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Generación Archivos ASN" Visible="false"/> 
    <asp:Label ID="lblErrorTemplate" runat="server" Text="No existe un template asociado para generar el archivo" Visible="false"/>	
    <asp:Label ID="lblErrorNotExistData" runat="server" Text="No existe detalle para crear el archivo" Visible="false"/>
    <asp:Label ID="lblErrorExistTaskOrder" runat="server" Text="No se puede generar ASN por que existen tareas pendientes."
        Visible="false" />
    <asp:Label ID="lblErrorReferenceDoc" runat="server" Text="Debe ingresar Nro. Orden de Compra" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>  

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
