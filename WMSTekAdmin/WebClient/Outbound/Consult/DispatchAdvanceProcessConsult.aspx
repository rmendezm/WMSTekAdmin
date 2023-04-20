<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="DispatchAdvanceProcessConsult.aspx.cs" 
    Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.DispatchAdvanceProcessConsult" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">

        function resizeDiv() {
   
        }

        window.onresize = resizeDiv;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDiv);

        $(document).ready(function () {
            initializeGridDragAndDrop("DispatchAdvanced_FindAllByProcess", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();

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
            initializeGridDragAndDrop("DispatchAdvanced_FindAllByProcess", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();
        }
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">
                            <asp:GridView ID="grdMgr" runat="server" 
                                DataKeyNames="Id"
                                AutoGenerateColumns="false"
                                EnableViewState="false"
                                AllowPaging="True"
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDataBound="grdMgr_RowDataBound"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Cód Tipo Doc." AccessibleHeaderText="OutboudTypeCode" SortExpression="OutboudTypeCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOutboudTypeCode" runat="server" Text='<%#Eval("Outbound.OutboundType.Code") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Tipo Doc." AccessibleHeaderText="OutboudTypeName" SortExpression="OutboudTypeName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOutboudTypeName" runat="server" Text='<%#Eval("Outbound.OutboundType.Name") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Doc. Solicitados" accessibleHeaderText="OrdersQty" SortExpression="OrdersQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Lineas Doc. Solicitadas" accessibleHeaderText="LinesQty" SortExpression="LinesQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLinesQty" runat="server" Text='<%#GetFormatedNumber( Eval("LinesQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Solicitada" accessibleHeaderText="QtySolicitado" SortExpression="QtySolicitado">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtySolicitado" runat="server" Text='<%#GetFormatedNumber( Eval("QtySolicitado")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Doc. Liberados" accessibleHeaderText="OrdersReleasedQty" SortExpression="OrdersReleasedQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersReleasedQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersReleasedQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Lineas Doc. Liberados" accessibleHeaderText="OrdersReleasedLinesQty" SortExpression="OrdersReleasedLinesQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersReleasedLinesQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersReleasedLinesQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Cantidad Liberada" AccessibleHeaderText="QtyRelease" SortExpression="QtyRelease">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyRelease" runat="server" Text='<%#GetFormatedNumber(Eval("QtyRelease")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Doc. Picking" accessibleHeaderText="OrdersOnPickingQty" SortExpression="OrdersOnPickingQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersOnPickingQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersOnPickingQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Lineas Doc. Picking" accessibleHeaderText="OrdersOnPickingLinesQty" SortExpression="OrdersOnPickingLinesQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersOnPickingLinesQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersOnPickingLinesQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Picking" AccessibleHeaderText="QtyPicking" SortExpression="QtyPicking">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyPicking" runat="server" Text='<%#GetFormatedNumber( Eval("QtyPicking")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Doc. Packing" accessibleHeaderText="OrdersOnPackingQty" SortExpression="OrdersOnPackingQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersOnPackingQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersOnPackingQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Lineas Doc. Packing" accessibleHeaderText="OrdersOnPackingLinesQty" SortExpression="OrdersOnPackingLinesQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersOnPackingLinesQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersOnPackingLinesQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Packing" AccessibleHeaderText="QtyPacking" SortExpression="QtyPacking">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyPacking" runat="server" Text='<%#GetFormatedNumber( Eval("QtyPacking")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Doc. Ruteo" accessibleHeaderText="OrdersOnRoutingQty" SortExpression="OrdersOnRoutingQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersOnRoutingQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersOnRoutingQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Lineas Doc. Ruteo" accessibleHeaderText="OrdersOnRoutingLinesQty" SortExpression="OrdersOnRoutingLinesQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersOnRoutingLinesQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersOnRoutingLinesQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Ruteo" AccessibleHeaderText="QtyRouting" SortExpression="QtyRouting">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyRouting" runat="server" Text='<%#GetFormatedNumber( Eval("QtyRouting")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Doc. Carga Camión" accessibleHeaderText="OrdersOnTruckLoadQty" SortExpression="OrdersOnTruckLoadQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersOnTruckLoadQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersOnTruckLoadQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Lineas Doc. Carga Camión" accessibleHeaderText="OrdersOnTruckLoadLinesQty" SortExpression="OrdersOnTruckLoadLinesQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersOnTruckLoadLinesQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersOnTruckLoadLinesQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Carga camión" AccessibleHeaderText="QtyLoading" SortExpression="QtyLoading">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyLoading" runat="server" Text='<%#GetFormatedNumber( Eval("QtyLoading")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Doc. Despachados" accessibleHeaderText="OrdersDispatchedQty" SortExpression="OrdersDispatchedQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersDispatchedQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersDispatchedQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Lineas Doc. Despachados" accessibleHeaderText="OrdersDispatchedLinesQty" SortExpression="OrdersDispatchedLinesQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrdersDispatchedLinesQty" runat="server" Text='<%#GetFormatedNumber( Eval("OrdersDispatchedLinesQty")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Despachada" AccessibleHeaderText="QtyShipping" SortExpression="QtyShipping">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyShipping" runat="server" Text='<%#GetFormatedNumber( Eval("QtyShipping")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
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
            </div>
        </div>
    </div>

    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

    <asp:Label id="lblFilterDate" runat="server" Text="Emisión" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
