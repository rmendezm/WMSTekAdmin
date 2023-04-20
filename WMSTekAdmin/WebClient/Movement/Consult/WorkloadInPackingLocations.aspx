<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="WorkloadInPackingLocations.aspx.cs" 
    Inherits="Binaria.WMSTek.WebClient.Movement.Consult.WorkloadInPackingLocations" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">

        $(document).ready(function () {
            initializeGridDragAndDrop("TaskStatistics_FindAll", "ctl00_MainContent_grdMgr");
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
            initializeGridDragAndDrop("TaskStatistics_FindAll", "ctl00_MainContent_grdMgr");
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
                                AutoGenerateColumns="false"
                                EnableViewState="false"
                                AllowPaging="True"
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDataBound="grdMgr_RowDataBound"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Cód Ubicación" AccessibleHeaderText="IdLocCode" SortExpression="IdLocCode">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ( "Location.IdCode" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="Description" SortExpression="Description">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ( "Location.Description" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Docs a Ejecutar" AccessibleHeaderText="DocsToExecute" SortExpression="DocsToExecute">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblDocsToExecute" runat="server" Text='<%#GetFormatedNumber( Eval("DocsToExecute")) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Items a Ejecutar" AccessibleHeaderText="QtyToExecute" SortExpression="QtyToExecute">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblQtyToExecute" runat="server" Text='<%#GetFormatedNumber( Eval("QtyToExecute")) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Docs Ejecutandose" AccessibleHeaderText="DocsExecuting" SortExpression="DocsExecuting">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblDocsExecuting" runat="server" Text='<%#GetFormatedNumber( Eval("DocsExecuting")) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Items Ejecutandose" AccessibleHeaderText="QtyExecuting" SortExpression="QtyExecuting">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblQtyExecuting" runat="server" Text='<%#GetFormatedNumber( Eval("QtyExecuting")) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Líneas Doc. a Ejecutar" AccessibleHeaderText="LinesToExecute" SortExpression="LinesToExecute">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblLinesToExecute" runat="server" Text='<%#GetFormatedNumber( Eval("LinesToExecute")) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Líneas Ejecutandose" AccessibleHeaderText="LinesExecuting" SortExpression="LinesExecuting">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblLinesExecuting" runat="server" Text='<%#GetFormatedNumber( Eval("LinesExecuting")) %>'></asp:Label>
                                                </center>
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

    <asp:Label id="lblFilterLocation" runat="server" Text="Ubicación" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
