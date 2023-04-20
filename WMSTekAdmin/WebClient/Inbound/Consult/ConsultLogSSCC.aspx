<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConsultLogSSCC.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Inbound.Consult.ConsultLogSSCC" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);
    window.onresize = SetDivs;

    $(document).ready(function () {
        initializeGridDragAndDrop("ConsultLogSSCC_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("ConsultLogSSCC_FindAll", "ctl00_MainContent_grdMgr");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">
                            <%-- Grilla Principal --%>
                            <asp:GridView ID="grdMgr" runat="server" AllowPaging="True" AllowSorting="False"
                                OnRowCreated="grdMgr_RowCreated" EnableViewState="false"
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Id Movto" AccessibleHeaderText="MovementId" SortExpression="Movement">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblMovementId" runat="server" Text='<%# Eval ( "Id" ) %>'></asp:Label>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Tipo" AccessibleHeaderText="MovementTypeId" SortExpression="MovementType">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblMovementType" runat="server" Text='<%# Eval ( "MovementType.Id" ) %>'></asp:Label>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Movto." AccessibleHeaderText="MovementTypeName" SortExpression="MovementType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMovementTypeName" runat="server" Text='<%# Eval ( "MovementType.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="StartTime" HeaderText="Inicio" AccessibleHeaderText="StartTime"
                                        SortExpression="StartTime" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="EndTime" HeaderText="Fin" AccessibleHeaderText="EndTime"
                                        SortExpression="EndTime" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="UserName" HeaderText="Operador" AccessibleHeaderText="UserName"
                                        SortExpression="UserName" ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderText="Id Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Id" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DocumentType" HeaderText="Tipo Docto." AccessibleHeaderText="DocumentType"
                                        SortExpression="DocumentType" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="DocumentNumber" HeaderText="Documento" AccessibleHeaderText="DocumentNumber"
                                        SortExpression="DocumentNumber" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="text" />
                                    <asp:BoundField DataField="DocumentLineNumber" HeaderText="Línea Docto." AccessibleHeaderText="DocumentLineNumber"
                                        SortExpression="DocumentLineNumber" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="ReferenceNumber" HeaderText="Referencia" AccessibleHeaderText="ReferenceNumber"
                                        SortExpression="DocumentLineNumber" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="text" />
                                    <asp:TemplateField HeaderText="Tipo Entrada" AccessibleHeaderText="InboundTypeName"
                                        ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInboundTypeName" runat="server" Text='<%# Eval ( "InboundType.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerId" runat="server" Text='<%# ((int) Eval ( "Owner.Id" )== -1)?" ":Eval ( "Owner.Id" )  %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="ItemId" SortExpression="ItemId"
                                        ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemId" runat="server" Text='<%# ((int) Eval ("Item.Id")== -1)?" ":Eval ("Item.Id") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Código Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode"
                                        ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ("Item.Code") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemDescription"
                                        SortExpression="ItemDescription">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval ("Item.Description") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="IdUOM" AccessibleHeaderText="IdUOM" SortExpression="IdUOM">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdUOM" runat="server" Text='<%# ((int) Eval ("ItemUom.Id")== -1)?" ":Eval("ItemUom.Id") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="UOM" AccessibleHeaderText="UOM" SortExpression="UOM">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUOM" runat="server" Text='<%# Eval ("ItemUom.Name") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="GTIN" AccessibleHeaderText="BarCode" SortExpression="BarCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblBarCode" runat="server" Text='<%# Eval ("ItemUom.BarCode") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fact. Conversión" AccessibleHeaderText="ConversionFactor"
                                        SortExpression="ConversionFactor">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblConversionFactor" runat="server" Text='<%# ((decimal) Eval ("ItemUom.ConversionFactor") == -1)?" ":Eval ("ItemUom.ConversionFactor") %>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Categoría" AccessibleHeaderText="IdCtgItem" SortExpression="IdCtgItem">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdCtgItem" runat="server" Text='<%# ((int) Eval ("CategoryItem.Id")== -1)?" ":Eval("CategoryItem.Id") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CategoryItemName"
                                        SortExpression="CategoryItemName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCategoryItemName" runat="server" Text='<%# Eval ("CategoryItem.Name") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="LotNumber" HeaderText="Lote" AccessibleHeaderText="LotNumber"
                                        SortExpression="LotNumber" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="text" />
                                    <asp:TemplateField HeaderText="Fecha Ingreso" AccessibleHeaderText="FifoDate" SortExpression="FifoDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fecha Expira." AccessibleHeaderText="ExpirationDate"
                                        SortExpression="ExpirationDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fecha Fabrica" AccessibleHeaderText="FabricationDate"
                                        SortExpression="FabricationDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblFabricationDate" runat="server" Text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdLpnCodeSource" HeaderText="SSCC Origen" AccessibleHeaderText="IdLpnCodeSource"
                                        SortExpression="IdLpnCodeSource" ItemStyle-CssClass="text" />
                                    <asp:BoundField DataField="IdLpnCodeTarget" HeaderText="SSCC Destino" AccessibleHeaderText="IdLpnCodeTarget"
                                        SortExpression="IdLpnCodeTarget" ItemStyle-CssClass="text" />
                                    <asp:BoundField DataField="LpnParentSource" HeaderText="GSIN Origen" AccessibleHeaderText="LpnParentSource"
                                        SortExpression="LpnParentSource" ItemStyle-CssClass="text" />
                                    <asp:BoundField DataField="LpnParentTarget" HeaderText="GSIN Destino" AccessibleHeaderText="LpnParentTarget"
                                        SortExpression="LpnParentTarget" ItemStyle-CssClass="text" />
                                    <asp:BoundField DataField="IdLocCodeProposal" HeaderText="Ubic. Propuesta" AccessibleHeaderText="IdLocCodeProposal"
                                        SortExpression="IdLocCodeProposal" />
                                    <asp:BoundField DataField="IdLocCodeSource" HeaderText="Ubic. Origen" AccessibleHeaderText="IdLocCodeSource"
                                        SortExpression="IdLocCodeSource" ItemStyle-CssClass="text" />
                                    <asp:BoundField DataField="IdLocCodeTarget" HeaderText="Ubic. Destino" AccessibleHeaderText="IdLocCodeTarget"
                                        SortExpression="IdLocCodeTarget" ItemStyle-CssClass="text" />
                                    <asp:TemplateField HeaderText="Qty Item" AccessibleHeaderText="ItemQtyMov" SortExpression="ItemQtyMov">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblItemQtyMov" runat="server" Text='<%# GetFormatedNumber(((decimal) Eval ("ItemQtyMov") == -1)?" ":Eval ("ItemQtyMov")) %>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Qty en Origen" AccessibleHeaderText="QtyBeforeSource"
                                        SortExpression="QtyBeforeSource">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblQtyBeforeSource" runat="server" Text='<%# GetFormatedNumber(((decimal) Eval ("QtyBeforeSource") == -1)?" ":Eval ("QtyBeforeSource")) %>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Qty en Destino" AccessibleHeaderText="QtyBeforeTarget"
                                        SortExpression="QtyBeforeTarget">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblQtyBeforeTarget" runat="server" Text='<%# GetFormatedNumber(((decimal) Eval ("QtyBeforeTarget") == -1)?" ":Eval ("QtyBeforeTarget")) %>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                   <asp:BoundField DataField="ReasonCode" HeaderText="Código Razón" AccessibleHeaderText="ReasonCode"
                                        SortExpression="ReasonCode" />
                                    <asp:BoundField DataField="HoldCode" HeaderText="Código Bloqueo" AccessibleHeaderText="HoldCode"
                                        SortExpression="HoldCode" />
                                    <asp:BoundField DataField="RoutingCode" HeaderText="Código Ruta" AccessibleHeaderText="RoutingCode"
                                        SortExpression="RoutingCode" />
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
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterDate" runat="server" Text="Inicio" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Destino" Visible="false" />
    <asp:Label ID="lblSSCC" runat="server" Text="SSCC" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>