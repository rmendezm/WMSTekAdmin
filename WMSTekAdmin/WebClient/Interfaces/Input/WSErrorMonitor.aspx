<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="WSErrorMonitor.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Interfaces.Input.WSErrorMonitor" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language='Javascript'>
        window.onresize = SetDivs; 
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(SetDivs);

        $(document).ready(function () {
            initializeGridDragAndDrop("WSErrorMonitor_FindAll", "ctl00_MainContent_grdMgr");
            //removeFooter("ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("WSErrorMonitor_FindAll", "ctl00_MainContent_grdMgr");
            //removeFooter("ctl00_MainContent_grdMgr");
        }
    </script>

     
    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
        <ContentTemplate>
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">

                            <asp:GridView ID="grdMgr" runat="server" 
                                DataKeyNames="Id"
                                AutoGenerateColumns="false"
                                EnableViewState="false"
                                AllowPaging="True"
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDataBound="grdMgr_RowDataBound"
                                OnRowCommand="grdMgr_RowCommand"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                            <ItemTemplate>
                                                <center>
                                                    <div style="width: 60px">
                                                        <asp:ImageButton ID="btnReprocess" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_refresh.png"
                                                            CausesValidation="false" CommandName="Reprocess" ToolTip="Reprocesar" CommandArgument="<%# Container.DataItemIndex %>" />
                                                    </div>
                                                </center>
                                            </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Id" AccessibleHeaderText="Id" Visible="false">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval ( "Id" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Ticket" AccessibleHeaderText="Ticket">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblTicket" runat="server" Text='<%# Eval ( "Ticket" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Objeto" AccessibleHeaderText="IdObjeto">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblIdObjeto" runat="server" Text='<%# Eval ( "IdObjeto" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Error" AccessibleHeaderText="Error">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblError" runat="server" Text='<%# Eval ( "Error" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

    <asp:Label ID="lblDateProcessText" runat="server" Text="Fecha Procesamiento" Visible="false" />
    <asp:Label ID="lblConfirm" runat="server" Text="¿Desea reprocesar este registro?" Visible="false" />

    <asp:Label ID="lblTicketId" runat="server" Text="N° Ticket" Visible="false" /> 

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
     <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
