<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="CheckBatchesInQueue.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.CheckBatchesInQueue" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" Assembly="Flan.Controls" Namespace="Flan.Controls"  %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("TaskQueue_GetBatchesInQueue", "ctl00_MainContent_grdMgr");
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
            initializeGridDragAndDrop("TaskQueue_GetBatchesInQueue", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();
        }     
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">
                        <asp:GridView ID="grdMgr" runat="server" 
                            OnRowCreated="grdMgr_RowCreated"
                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                            AllowPaging="false"
                            AutoGenerateColumns="false"
                            OnRowDataBound="grdMgr_RowDataBound"
                            OnRowEditing="grdMgr_RowEditing"
                            OnRowDeleting="grdMgr_RowDeleting"
                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                            EnableTheming="false">
                            <Columns>
                                <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                    <itemtemplate>
                                        <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.Name" ) %>' />
                                    </itemtemplate>
                                </asp:templatefield>

                                <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="OwnName" >
                                    <itemtemplate>
                                        <asp:label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>' />
                                    </itemtemplate>
                                </asp:templatefield>

                                <asp:templatefield HeaderText="N° Batch" AccessibleHeaderText="OutboundOrderNumber" >
                                    <itemtemplate>
                                        <asp:label ID="lblNumberDocumentBound" runat="server" text='<%# Eval ( "NumberDocumentBound" ) %>' />
                                    </itemtemplate>
                                </asp:templatefield>

                                <asp:templatefield HeaderText="% Avance" AccessibleHeaderText="PercCompletion" >
                                    <itemtemplate>
                                        <asp:label ID="lblPercCompletion" runat="server" text='<%# Eval ( "PercCompletion" ) %>' />
                                    </itemtemplate>
                                </asp:templatefield>

                                <asp:templatefield HeaderText="Estado" AccessibleHeaderText="NameTrackTaskQueue" >
                                    <itemtemplate>
                                        <asp:label ID="lblNameTrackTaskQueue" runat="server" text='<%# Eval ( "TrackTaskQueue.NameTrackTaskQueue" ) %>' />
                                    </itemtemplate>
                                </asp:templatefield>

                                <asp:TemplateField ShowHeader="False" HeaderText="Acciones" HeaderStyle-HorizontalAlign="Center" AccessibleHeaderText="Actions">
                                    <ItemTemplate>
                                        <center>
                                            <div style="width: 160px">
                                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_refresh.png"
                                                    CausesValidation="false" CommandName="Edit" />
                                                <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                    CausesValidation="false" CommandName="Delete" />
                                            </div>
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
    <asp:Label id="lblDocName" runat="server" Text="Nº Batch" Visible="false" />    
    <asp:Label id="lblFilterDate" runat="server" Text="Emisión" Visible="false" />    
    <asp:Label ID="lblConfirmCancel" runat="server" Text="¿Desea cancelar esta tarea?" Visible="false" />
    <asp:Label ID="lblRetryCancel" runat="server" Text="¿Desea reintentar esta tarea?" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
