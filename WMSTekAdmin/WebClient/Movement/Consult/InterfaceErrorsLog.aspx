<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="InterfaceErrorsLog.aspx.cs" 
    Inherits="Binaria.WMSTek.WebClient.Inbound.Consult.InterfaceErrorsLog" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language='Javascript'>
        window.onresize = SetDivs; 
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(SetDivs);

        $(document).ready(function () {
            initializeGridDragAndDrop("MovementIfz_FindAll", "ctl00_MainContent_grdMgr");
            removeFooter("ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("MovementIfz_FindAll", "ctl00_MainContent_grdMgr");
            removeFooter("ctl00_MainContent_grdMgr");
        }
    </script>

    <style>
        #ctl00_MainContent_divGrid{
            overflow: hidden !important;
        }
    </style>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">
                            <%-- Grilla Principal --%>
                            <asp:GridView ID="grdMgr" runat="server" 
                                DataKeyNames="Id"
                                AutoGenerateColumns="false"
                                EnableViewState="false"
                                AllowPaging="True"
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDataBound="grdMgr_RowDataBound"
                                OnRowDeleting="grdMgr_RowDeleting"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                            <ItemTemplate>
                                                <center>
                                                    <div style="width: 60px">
                                                        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                            CommandName="Delete" ToolTip="Eliminar" />
                                                    </div>
                                                </center>
                                            </ItemTemplate>
                                         </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Fecha Procesamiento" AccessibleHeaderText="Dateprocess">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblDateprocess" runat="server" Text='<%# Eval ( "Dateprocess" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Estado Proceso" AccessibleHeaderText="StatusProcess">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblStatusProcess" runat="server" Text='<%# Eval ( "StatusProcess" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Tipo Movimiento" AccessibleHeaderText="TypeMovto">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblTypeMovto" runat="server" Text='<%# Eval ( "TypeMovto" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ticket" AccessibleHeaderText="TicketTransfer">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblTicketTransfer" runat="server" Text='<%# Eval ( "IdTicketTransfer" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Mensaje" AccessibleHeaderText="StatusMessage">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblStatusMessage" runat="server" Text='<%# Eval ( "StatusMessage" ) %>'></asp:Label>
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

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblcodeLabelText" runat="server" Text="Estado Proceso" Visible="false" />
    <asp:Label ID="lblDateProcessText" runat="server" Text="Fecha " Visible="false" />
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Error?" Visible="false" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
