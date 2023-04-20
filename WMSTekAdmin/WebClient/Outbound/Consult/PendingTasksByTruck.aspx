<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="PendingTasksByTruck.aspx.cs" 
    Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.PendingTasksByTruck" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">

        $(document).ready(function () {
            initializeGridDragAndDrop("Task_FindAllPendingTasksByTruck", "ctl00_MainContent_grdMgr");
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
            initializeGridDragAndDrop("Task_FindAllPendingTasksByTruck", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();
        }

    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" >  
                            <asp:GridView ID="grdMgr" runat="server" 
                                OnRowCreated="grdMgr_RowCreated"
                                AllowPaging="True"
                                AutoGenerateColumns="false"
                                OnRowDataBound="grdMgr_RowDataBound"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">

                                <Columns>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ( "Task.Owner.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="WhsName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Task.Warehouse.ShortName" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Camión" AccessibleHeaderText="LocCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLocCode" runat="server" Text='<%# Eval ( "Task.StageSource.Code" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo Tarea" AccessibleHeaderText="TaskTypeCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTaskTypeCode" runat="server" Text='<%# Eval ( "Task.TypeCode" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Doc. Salida" AccessibleHeaderText="OutboundNumber" SortExpression="OutboundNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Eval ( "OutboundOrder.Number" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo Doc. Salida" AccessibleHeaderText="OutboundTypeName" SortExpression="OutboundTypeName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "OutboundOrder.OutboundType.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerName" SortExpression="CustomerName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "OutboundOrder.Customer.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:templatefield HeaderText="LPN Propuesto Origen" AccessibleHeaderText="IdLpnSourceProposal" >
                                        <itemtemplate>
                                            <asp:label ID="lblIdLpnSourceProposal" runat="server" text='<%# Eval ( "TaskDetail.IdLpnSourceProposal" ) %>' />
                                        </itemtemplate>
                                    </asp:templatefield>

                                    <asp:templatefield HeaderText="LPN Propuesto Destino" AccessibleHeaderText="IdLpnTargetProposal" >
                                        <itemtemplate>
                                            <asp:label ID="lblIdLpnTargetProposal" runat="server" text='<%# Eval ( "TaskDetail.IdLpnTargetProposal" ) %>' />
                                        </itemtemplate>
                                    </asp:templatefield>

                                    <asp:templatefield HeaderText="Ubicación Propuesta Origen" AccessibleHeaderText="IdLocSourceProposal" >
                                        <itemtemplate>
                                            <asp:label ID="lblIdLocSourceProposal" runat="server" text='<%# Eval ( "TaskDetail.IdLocSourceProposal" ) %>' />
                                        </itemtemplate>
                                    </asp:templatefield>

                                    <asp:templatefield HeaderText="Ubicación Propuesta Destino" AccessibleHeaderText="IdLocTargetProposal" >
                                        <itemtemplate>
                                            <asp:label ID="lblIdLocTargetProposal" runat="server" text='<%# Eval ( "TaskDetail.IdLocTargetProposal" ) %>' />
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
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

    <asp:Label id="lblTruck" runat="server" Text="Camión" Visible="false" /> 

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
     <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>
