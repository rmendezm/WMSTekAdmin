<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="Slotting.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Movement.Administration.Slotting" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">

        $(document).ready(function () {
            initializeGridDragAndDrop("Slotting_FindAll", "ctl00_MainContent_grdMgr");
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
            initializeGridDragAndDrop("Slotting_FindAll", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();
        }

        function setDivsAfter() {
            var height = $("#ctl00_MainContent_divGrid").height();
            var finalHeight = height - 50;
            $("#ctl00_MainContent_divGrid").css("max-height", finalHeight + "px");
        }

    </script>

    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
        <ContentTemplate>
            <div class="container">
                <div class="row" id="divBtnProcess">
                    <div class="col-md-12">
                        <asp:ImageButton ID="btnProcess" runat="server" onclick="btnProcess_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_process_dis.png" ToolTip="Generar Tareas de movimiento"/> 
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">
                            <asp:GridView ID="grdMgr" 
                                DataKeyNames="Id" 
                                runat="server" 
                                OnRowCreated="grdMgr_RowCreated"
                                AllowPaging="True" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>

                                    <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="Warehouse">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Task.Warehouse.Name" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwner" runat="server" Text='<%# Eval ( "Task.Owner.Name" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo Tarea" AccessibleHeaderText="TypeCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblTypeCode" runat="server" Text='<%# Eval ( "Task.TypeCode" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Descripcion" AccessibleHeaderText="Description">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ( "Task.Description" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Fecha Creación" AccessibleHeaderText="CreateDate">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCreateDate" runat="server" Text='<%# Eval ( "Task.CreateDate" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Cód Item" AccessibleHeaderText="ItemCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "TaskDetail.Item.Code" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Nombre Item" AccessibleHeaderText="ItemShortName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblItemShortName" runat="server" Text='<%# Eval ( "TaskDetail.Item.ShortName" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ProposalQty">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblProposalQty" runat="server" Text='<%# Eval ( "TaskDetail.ProposalQty" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Ubic. Origen" AccessibleHeaderText="IdLocSourceProposal">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdLocSourceProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLocSourceProposal" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Ubic. Destino" AccessibleHeaderText="IdLocTargetProposal">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdLocTargetProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLocTargetProposal" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="LPN Origen" AccessibleHeaderText="IdLpnSourceProposal">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdLpnSourceProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLpnSourceProposal" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="LPN Destino" AccessibleHeaderText="IdLpnTargetProposal">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdLpnTargetProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLpnTargetProposal" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Cód. Categoria Item" AccessibleHeaderText="CategoryItemCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCategoryItemCode" runat="server" Text='<%# Eval ( "TaskDetail.CategoryItem.Code" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Nombre Categoria Item" AccessibleHeaderText="CategoryItemName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCategoryItemName" runat="server" Text='<%# Eval ( "TaskDetail.CategoryItem.Name" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ( "TaskDetail.LotNumber" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Fecha Fifo" AccessibleHeaderText="FifoDate">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblFifoDate" runat="server" Text='<%# (((DateTime)Eval ( "TaskDetail.FifoDate" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "TaskDetail.FifoDate", "{0:d}" )) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Fecha Fabricación" AccessibleHeaderText="FabricationDate">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblFabricationDate" runat="server" Text='<%# (((DateTime)Eval ( "TaskDetail.FabricationDate" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "TaskDetail.FabricationDate", "{0:d}" )) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Fecha Expiración" AccessibleHeaderText="ExpirationDate">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblExpirationDate" runat="server" Text='<%# (((DateTime)Eval ( "TaskDetail.ExpirationDate" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "TaskDetail.ExpirationDate", "{0:d}" )) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>
                        </div>
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

    <asp:Label runat="server" ID="lblWarningMsg" Visible="false" Text="No hay tareas de mov. dirigido a procesar"></asp:Label>
    <asp:Label runat="server" ID="lblValidateSlottingNotFinished" Visible="false" Text="No se puede crear slotting porque hay tareas de mov. dirigido incompletas"></asp:Label>
 </asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
