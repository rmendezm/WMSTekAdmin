<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="ActivateReplacementOnDemand.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Movement.Administration.ActivateReplacementOnDemand" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("ActivateReplacementOnDemand_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("ActivateReplacementOnDemand_FindAll", "ctl00_MainContent_grdMgr");
        }
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:Label ID="lblPendingOrders" runat="server" Text="Activar tareas de reposición" />
                <asp:ImageButton ID="btnActivate" runat="server" onclick="btnActivate_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_process_dis.png" ToolTip="Activar Tareas de reposición"/> 
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid">
                            <asp:GridView 
                                    ID="grdMgr" 
                                    runat="server" 
                                    DataKeyNames="Id" 
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    OnPageIndexChanging="grdMgr_PageIndexChanging"
                                    OnRowCreated="grdMgr_RowCreated"
                                    AllowPaging="True" 
                                    EnableViewState="True" 
                                    AutoGenerateColumns="False" 
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false">
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False" AccessibleHeaderText="Actions">
                                            <HeaderTemplate>
                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectTask', this.checked)"
                                                    id="chkAll" title="Seleccionar todos" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <center>
                                                    <div style="width: 20px">
                                                        <asp:CheckBox ID="chkSelectTask" runat="server" />
                                                    </div>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        
                                        <asp:TemplateField HeaderText="ID" AccessibleHeaderText="Id" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdTask" runat="server" Text='<%# Eval ( "Task.Id" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Task.Warehouse.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval ( "Task.Owner.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Codigo Item" AccessibleHeaderText="ItemCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "TaskDetail.Item.Code" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ( "TaskDetail.Item.Description" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ( "Task.StageSource.Code" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>  

                                        <asp:TemplateField HeaderText="Fila" AccessibleHeaderText="RowLoc">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowLoc" runat="server" Text='<%# Eval ( "Task.StageSource.Row" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Columna" AccessibleHeaderText="ColumnLoc">
                                            <ItemTemplate>
                                                <asp:Label ID="lblColumnLoc" runat="server" Text='<%# Eval ( "Task.StageSource.Column" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Nivel" AccessibleHeaderText="LevelLoc">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLevelLoc" runat="server" Text='<%# Eval ( "Task.StageSource.Level" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>  
                                        
                                        <asp:TemplateField HeaderText="Track" AccessibleHeaderText="Track">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTrack" runat="server" Text='<%# Eval ( "TrackTaskType.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>  

                                        <asp:TemplateField HeaderText="Cant. en Put" AccessibleHeaderText="PutawayQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPutawayQty" runat="server" Text='<%# Eval ( "Task.StageSource.SpecialField1" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField> 

                                          <asp:TemplateField HeaderText="Cant. Propuesta" AccessibleHeaderText="ProposalQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblProposalQty" runat="server" Text='<%# Eval ( "TaskDetail.ProposalQty" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="IdWhs" AccessibleHeaderText="IdWhs">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdWhs" runat="server" Text='<%# Eval ( "Task.Warehouse.Id" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>     

                                        <asp:TemplateField HeaderText="IdOwn" AccessibleHeaderText="IdOwn">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ( "Task.Owner.Id" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="IdItem">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdItem" runat="server" Text='<%# ((int)Eval("TaskDetail.Item.Id") == -1) ? " ":Eval("TaskDetail.Item.Id") %>' />
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
                        <asp:AsyncPostBackTrigger ControlID="btnActivate" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterCode" runat="server" Text="Código Item" Visible="false" />
    <asp:Label ID="lblFilterDescription" runat="server" Text="Nombre Item" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
     <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
