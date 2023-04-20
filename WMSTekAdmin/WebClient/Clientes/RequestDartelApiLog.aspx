<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="RequestDartelApiLog.aspx.cs" 
    Inherits="Binaria.WMSTek.WebClient.Clientes.RequestDartelApiLog" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language='Javascript'>
        window.onresize = SetDivs; 
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(SetDivs);

        $(document).ready(function () {
            initializeGridDragAndDrop("RequestApiSend_FindAll", "ctl00_MainContent_grdMgr");
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
            initializeGridDragAndDrop("RequestApiSend_FindAll", "ctl00_MainContent_grdMgr");
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
                                DataKeyNames="IdRequestSend"
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

                                         <asp:TemplateField HeaderText="Id Empleado" AccessibleHeaderText="IdEmp">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblIdEmp" runat="server" Text='<%# Eval ( "IdEmp" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Usuario" AccessibleHeaderText="UserName">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblUserName" runat="server" Text='<%# Eval ( "UserName" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Referencia" AccessibleHeaderText="ReferenceNumber">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblReferenceNumber" runat="server" Text='<%# Eval ( "ReferenceNumber" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Origen" AccessibleHeaderText="Origin">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblOrigin" runat="server" Text='<%# Eval ( "Origin" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Error" AccessibleHeaderText="ErrorMessage">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblErrorMessage" runat="server" Text='<%# Eval ( "ErrorMessage" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                          <asp:TemplateField HeaderText="Estado" AccessibleHeaderText="Status">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval ( "Status" ) %>'></asp:Label>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                          <asp:TemplateField HeaderText="Fecha" AccessibleHeaderText="SendDate">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:Label ID="lblSendDate" runat="server" Text='<%# Eval ( "SendDate" ) %>'></asp:Label>
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
    <asp:Label ID="lblDateProcessText" runat="server" Text="Fecha " Visible="false" />
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este registro?" Visible="false" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
