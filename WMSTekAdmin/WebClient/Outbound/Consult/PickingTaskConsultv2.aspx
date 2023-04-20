<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="PickingTaskConsultv2.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.PickingTaskConsultv2" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("TaskPicking_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("TaskPicking_FindAll", "ctl00_MainContent_grdMgr");
    }

    function callIframe(url) {
        $("#ctl00_MainContent_iframe").prop("src", url);
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">
                            <%-- Grilla Tarea --%>
                            <asp:GridView ID="grdMgr" runat="server" 
                                DataKeyNames="Id"
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowEditing="grdMgr_RowEditing" 
                                OnRowDataBound="grdMgr_RowDataBound" 
                                OnPageIndexChanging="grdMgr_PageIndexChanging"
                                AllowPaging="True" 
                                EnableViewState="False" 
                                OnRowDeleting="grdMgr_RowDeleting" 
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="TaskActions">
                                        <ItemTemplate>
                                            <div style="width: 60px">
                                                <center>
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" />                                        
                                                </center>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Id Tarea" AccessibleHeaderText="TaskId" SortExpression="Task">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblTaskId" runat="server" Text='<%# Eval ( "Task.Id" ) %>'></asp:Label>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Task.Warehouse.Id" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Task.Warehouse.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerId" runat="server" Text='<%# ((int) Eval ( "Task.Owner.Id" )== -1)?" ":Eval ( "Task.Owner.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ( "Task.Owner.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ejecutada" AccessibleHeaderText="IsComplete">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="lblIsComplete" runat="server" Text='<%# Eval ( "Task.IsComplete" ) %>'></asp:Label>--%>
                                            <center>
                                                <asp:CheckBox ID="chkIsComplete" runat="server" checked='<%# Eval ( "Task.IsComplete" ) %>' Enabled="false"/>
                                            </center> 
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Codigo Tarea" AccessibleHeaderText="TaskTypeCode" SortExpression="TaskType">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblTaskType" runat="server" Text='<%# Eval ( "Task.TypeCode" ) %>'></asp:Label>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tarea" AccessibleHeaderText="TaskTypeName" SortExpression="TaskType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTaskTypeName" runat="server" Text='<%# Eval ( "TaskTypeName" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Documento" AccessibleHeaderText="IdDocumentBound" SortExpression="DocumentBound">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdDocumentBound" runat="server" Text='<%# ((int) Eval ( "Task.IdDocumentBound" )== -1)?" ":Eval ( "Task.IdDocumentBound" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Documento" AccessibleHeaderText="OutboundNumber" SortExpression="OutboundNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Eval ( "OutboundOrder.Number" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo Documento" AccessibleHeaderText="OutboundTypeName" SortExpression="OutboundTypeName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "OutboundOrder.OutboundType.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prioridad" AccessibleHeaderText="Priority" SortExpression="Priority">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPriority" runat="server" Text='<%# ((int) Eval ( "Task.Priority" )== -1)?" ":Eval ( "Task.Priority" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Creada" AccessibleHeaderText="CreateDate" SortExpression="CreateDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCreateDate" runat="server" Text='<%# ((DateTime) Eval ("Task.CreateDate") > DateTime.MinValue)? Eval("Task.CreateDate", "{0:dd/MM/yyyy HH:mm:ss}"):"" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cerrada" AccessibleHeaderText="CloseDate" SortExpression="CloseDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCloseDate" runat="server" Text='<%# ((DateTime) Eval ("Task.CloseDate") > DateTime.MinValue)? Eval("Task.CloseDate", "{0:dd/MM/yyyy HH:mm:ss}"):"" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Inicio Propuesto" AccessibleHeaderText="ProposalStartDate" SortExpression="ProposalStartDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblProposalStartDate" runat="server" Text='<%# ((DateTime) Eval ("Task.ProposalStartDate") > DateTime.MinValue)? Eval("Task.ProposalStartDate", "{0:dd/MM/yyyy HH:mm:ss}"):"" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fin Propuesto" AccessibleHeaderText="ProposalEndDate" SortExpression="ProposalEndDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblProposalEndDate" runat="server" Text='<%# ((DateTime) Eval ("Task.ProposalEndDate") > DateTime.MinValue)? Eval("Task.ProposalEndDate", "{0:dd/MM/yyyy HH:mm:ss}"):"" %>' ></asp:Label>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Inicio Real" AccessibleHeaderText="RealStartDate" SortExpression="RealStartDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRealStartDate" runat="server" Text='<%# ((DateTime) Eval ("Task.RealStartDate") > DateTime.MinValue)? Eval("Task.RealStartDate", "{0:dd/MM/yyyy HH:mm:ss}"):"" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fin Real" AccessibleHeaderText="RealEndDate" SortExpression="RealEndDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRealEndDate" runat="server" Text='<%# ((DateTime) Eval ("Task.RealEndDate") > DateTime.MinValue)? Eval("Task.RealEndDate", "{0:dd/MM/yyyy HH:mm:ss}"):"" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status" SortExpression="Status">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="lblStatus" runat="server" Text='<%# Eval ( "Task.Status" ) %>'></asp:Label>--%>
                                            <center>
                                                <asp:CheckBox ID="chkStatus" runat="server" checked='<%# Eval ( "Task.Status" ) %>' Enabled="false"/>
                                            </center> 
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Traza" AccessibleHeaderText="IdTrackTaskType" SortExpression="TrackTaskType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdTrackTaskType" runat="server" Text='<%# ((int)Eval ( "Task.IdTrackTaskType" )==-1)?"":Eval ( "Task.IdTrackTaskType" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="NameTrackTaskType" SortExpression="NameTrackTaskType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblNameTrackTaskType" runat="server" Text='<%# Eval ( "TrackTaskType.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fecha Traza" AccessibleHeaderText="DateTrackTask" SortExpression="DateTrackTask">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDateTrackTask" runat="server" Text='<%# ((DateTime) Eval ("Task.DateTrackTask") > DateTime.MinValue)? Eval("Task.DateTrackTask", "{0:dd/MM/yyyy HH:mm:ss}"):"" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ubic Origen" AccessibleHeaderText="IdLocStageSource" SortExpression="LocStageSource">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLocStageSource" runat="server" Text='<%# Eval ( "IdLocStageSource" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ubic Destino" AccessibleHeaderText="IdLocStageTarget" SortExpression="LocStageTarget">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLocStageTarget" runat="server" Text='<%# Eval ( "IdLocStageTarget" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Oper Requeridos" AccessibleHeaderText="WorkersRequired" SortExpression="WorkersRequired">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWorkersRequired" runat="server" Text='<%# ((int)Eval ( "Task.WorkersRequired" )==-1)?"":Eval ( "Task.WorkersRequired" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Oper Asignados" AccessibleHeaderText="WorkersAssigned" SortExpression="WorkersAssigned">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWorkersAssigned" runat="server" Text='<%# ((int)Eval ( "Task.WorkersAssigned" )==-1)?"":Eval ( "Task.WorkersAssigned" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Con CrossDocking" AccessibleHeaderText="AllowCrossDock" SortExpression="AllowCrossDock">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="lblAllowCrossDock" runat="server" Text='<%# Eval ( "Task.AllowCrossDock" ) %>'></asp:Label>--%>
                                            <center>
                                                <asp:CheckBox ID="chkAllowCrossDock" runat="server" checked='<%# Eval ( "Task.AllowCrossDock" ) %>' Enabled="false"/>
                                            </center> 
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <%-- Tarea Detalle --%>
                                    <asp:TemplateField HeaderText="ID Detalle" AccessibleHeaderText="IdTaskDetail" SortExpression="TaskDetail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdTaskDetail" runat="server" Text='<%# Eval ( "TaskDetail.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ejecutada" AccessibleHeaderText="IsCompleteDetail" SortExpression="IsCompleteDetail">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="lblIsCompleteDetail" runat="server" Text='<%# Eval ( "TaskDetail.IsComplete" ) %>'></asp:Label>--%>
                                            <center>
                                                <asp:CheckBox ID="chkIsCompleteDetail" runat="server" checked='<%# Eval ( "TaskDetail.IsComplete" ) %>' Enabled="false"/>
                                            </center> 
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Iniciada" AccessibleHeaderText="StartDate" SortExpression="StartDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime) Eval ("TaskDetail.StartDate") > DateTime.MinValue)? Eval("TaskDetail.StartDate", "{0:dd/MM/yyyy HH:mm:ss}"):"" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Finalizada" AccessibleHeaderText="EndDate" SortExpression="EndDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime) Eval ("TaskDetail.EndDate") > DateTime.MinValue)? Eval("TaskDetail.EndDate", "{0:dd/MM/yyyy HH:mm:ss}"):"" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Documento" AccessibleHeaderText="IdDocumentBoundDetail" SortExpression="DocumentBoundDetail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdDocumentBoundDetail" runat="server" Text='<%# ((int)Eval ( "TaskDetail.IdDocumentBound" )==-1)?"":Eval ( "TaskDetail.IdDocumentBound" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Detalle Doc" AccessibleHeaderText="IdDetailBound" SortExpression="DetailBound">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdDetailBound" runat="server" Text='<%# ((int)Eval ( "TaskDetail.IdDetailBound" )==-1)?"":Eval ( "TaskDetail.IdDetailBound" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Linea Detalle" AccessibleHeaderText="LineNumber" SortExpression="LineNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLineNumber" runat="server" Text='<%# ((int)Eval ( "TaskDetail.LineNumber" )==-1)?"":Eval ( "TaskDetail.LineNumber" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cod Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "TaskDetail.Item.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName" SortExpression="LongItemName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ( "TaskDetail.Item.LongName" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cant Propuesta" AccessibleHeaderText="ProposalQty" SortExpression="ProposalQty">
                                        <ItemTemplate>
                                            <asp:Label ID="lblProposalQty" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval ( "TaskDetail.ProposalQty" )==-1)?"":Eval ( "TaskDetail.ProposalQty" )) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cant Real" AccessibleHeaderText="RealQty" SortExpression="RealQty">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRealQty" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval ( "TaskDetail.RealQty" )==-1)?"":Eval ( "TaskDetail.RealQty" )) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Categoria" AccessibleHeaderText="IdCtgItem" SortExpression="CtgItem">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdCtgItem" runat="server" Text='<%# ((int)Eval ( "TaskDetail.CategoryItem.Id" )==-1)?"":Eval ( "TaskDetail.CategoryItem.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Categoria Item" AccessibleHeaderText="CtgName" SortExpression="CtgName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval ( "TaskDetail.CategoryItem.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prioridad" AccessibleHeaderText="PriorityDetail" SortExpression="PriorityDetail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPriorityDetail" runat="server" Text='<%# ((int)Eval ( "TaskDetail.Priority" )==-1)?"":Eval ( "TaskDetail.Priority" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Con Cross Docking" AccessibleHeaderText="MadeCrossDock" SortExpression="MadeCrossDock">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="lblMadeCrossDock" runat="server" Text='<%# Eval ( "TaskDetail.MadeCrossDock" ) %>'></asp:Label>--%>
                                            <center>
                                                <asp:CheckBox ID="chkMadeCrossDock" runat="server" checked='<%# Eval ( "TaskDetail.MadeCrossDock" ) %>' Enabled="false"/>
                                            </center> 
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Origen Propuesto" AccessibleHeaderText="IdLocSourceProposal" SortExpression="LocSourceProposal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLocSourceProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLocSourceProposal" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Maquina Propuesta" AccessibleHeaderText="IdLocForkLiftProposal" SortExpression="LocForkLiftProposal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLocForkLiftProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLocForkLiftProposal" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Destino Propuesto" AccessibleHeaderText="IdLocTargetProposal" SortExpression="LocTargetProposal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLocTargetProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLocTargetProposal" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Origen Usado" AccessibleHeaderText="IdLocSourceUsed" SortExpression="LocSourceUsed">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLocSourceUsed" runat="server" Text='<%# Eval ( "TaskDetail.IdLocSourceUsed" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Maquina Usada" AccessibleHeaderText="IdLocForkLiftUsed" SortExpression="LocForkLiftUsed">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLocForkLiftUsed" runat="server" Text='<%# Eval ( "TaskDetail.IdLocForkLiftUsed" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Destino Usado" AccessibleHeaderText="IdLocTargetUsed" SortExpression="LocTargetUsed">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLocTargetUsed" runat="server" Text='<%# Eval ( "TaskDetail.IdLocTargetUsed" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LPN Origen Propuesto" AccessibleHeaderText="IdLpnSourceProposal" SortExpression="LpnSourceProposal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLpnSourceProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLpnSourceProposal" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LPN Destino Propuesto" AccessibleHeaderText="IdLpnTargetProposal" SortExpression="LpnTargetProposal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLpnTargetProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLpnTargetProposal" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LPN Destino Usado" AccessibleHeaderText="IdLpnTargetUsed" SortExpression="LpnTargetUsed">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLpnTargetUsed" runat="server" Text='<%# Eval ( "TaskDetail.IdLpnTargetUsed" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LPN Origen Usado" AccessibleHeaderText="IdLpnSourceUsed" SortExpression="LpnSourceUsed">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLpnSourceUsed" runat="server" Text='<%# Eval ( "TaskDetail.IdLpnSourceUsed" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="StatusDetail" SortExpression="StatusDetail">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="lblStatusDetail" runat="server" Text='<%# Eval ( "TaskDetail.Status" ) %>'></asp:Label>--%>
                                            <center>
                                                <asp:CheckBox ID="chkStatusDetail" runat="server" checked='<%# Eval ( "TaskDetail.Status" ) %>' Enabled="false"/>
                                            </center> 
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ID Proceso" AccessibleHeaderText="IdPlanedProcess" SortExpression="PlanedProcess">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdPlanedProcess" runat="server" Text='<%# ((int)Eval ( "TaskDetail.IdPlanedProcess" )==-1)?"":Eval ( "TaskDetail.IdPlanedProcess" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Codigo Ola" AccessibleHeaderText="WaveCode" SortExpression="WaveCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWaveCode" runat="server" Text='<%# Eval ( "TaskDetail.WaveCode" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Etiqueta" AccessibleHeaderText="LabelCode" SortExpression="LabelCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLabelCode" runat="server" Text='<%# Eval ( "TaskDetail.LabelCode" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Carga" AccessibleHeaderText="LoadCode" SortExpression="LoadCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLoadCode" runat="server" Text='<%# Eval ( "TaskDetail.LoadCode" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Secuencia Carga" AccessibleHeaderText="LoadSeq" SortExpression="LoadSeq">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLoadSeq" runat="server" Text='<%# ((int)Eval ( "TaskDetail.LoadSeq" )==-1)?"":Eval ( "TaskDetail.LoadSeq" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Operador" AccessibleHeaderText="UserAssigned" SortExpression="UserAssigned">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUserAssigned" runat="server" Text='<%# Eval ( "TaskDetail.UserAssigned" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Serie" AccessibleHeaderText="SerialNumber" SortExpression="SerialNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSerialNumber" runat="server" Text='<%# Eval ( "TaskDetail.SerialNumber" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%-- Fin Tarea Detalle --%>
                        
                                </Columns>
                            </asp:GridView>
                            <%-- FIN Grilla Tarea --%>
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
    
    
    <asp:UpdatePanel ID="upEditTask" runat="server" UpdateMode="Conditional">
        <ContentTemplate> 
            <%-- Pop up Editar Tarea --%>
            <div id="divEditTask" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlUser" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <%-- Encabezado --%>
                <asp:Panel ID="pnlUser" runat="server" CssClass="modalBox">
                    <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblEditTask" runat="server" Text="Editar Tarea" />
                            <asp:ImageButton ID="btnCloseTask" runat="server" ImageAlign="Top" ToolTip="Cerrar ventana de edición" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidIsBaseUser" runat="server" Value="false" />
                        <div class="divCtrsFloatLeft">
                            <div id="divTaskId" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskId" runat="server" Text="Id Tarea" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskId" runat="server" MaxLength="30" Width="150" Enabled="false" />
                                </div>
                            </div>
                            <div id="divTaskWhsName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskWhsName" runat="server" Text="Centro" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskWhsName" runat="server" MaxLength="30" Width="150" Enabled="false" />    
                                </div>
                            </div>
                            <div id="divTaskOwnName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskOwnName" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskOwnName" runat="server" MaxLength="30" Width="150" Enabled="false" />
                                </div>
                            </div>
                            <div id="divTaskIsComplete" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskIsComplete" runat="server" Text="Ejecutada" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTaskIsComplete" runat="server" Enabled="false" TabIndex="1"/>
                                </div>
                            </div>
                            <div id="divTaskName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskName" runat="server" Text="Tarea" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskName" runat="server" Enabled="false" />
                                </div>
                            </div>
                            <div id="divTaskDescription" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDescription" runat="server" Text="Descripcion" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDescription" runat="server" MaxLength="60" TabIndex="2" />
                                </div>
                            </div>
                            <div id="divTaskDocumentBound" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDocumentBound" runat="server" Text="Documento" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDocumentBound" runat="server" Enabled="false" />
                                </div>
                            </div>
                            <div id="divTaskPriority" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskPriority" runat="server" Text="Prioridad" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskPriority" runat="server" Enabled="true" TabIndex="3" />
                                    <asp:RequiredFieldValidator ID="rfvTaskPriority" runat="server" ControlToValidate="txtTaskPriority" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad es requerido" />
                                    <asp:RangeValidator ID="rvTaskPriority" runat="server" ControlToValidate="txtTaskPriority" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad debe ser entre 1 y 10" MinimumValue="1" MaximumValue="10" Type="Integer" />
                                </div>
                            </div>
                            <div id="divTaskRealStartDate" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskRealStartDate" runat="server" Text="Fec Inicio" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskRealStartDate" runat="server" Enabled="false" />
                                    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender4" runat="server" TargetControlID="txtTaskRealStartDate"
                                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                                            InputDirection="LeftToRight">
                                    </ajaxToolkit:MaskedEditExtender>    
                                </div>
                            </div>
                            <div id="divTaskStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskStatus" runat="server" Text="Status" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTaskStatus" runat="server" Enabled="true" TabIndex="4" />
                                </div>
                            </div>
                         </div>
                         <div class="divCtrsFloatLeft">
                            <div id="divTaskTrackTaskType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskTrackTaskType" runat="server" Text="Traza" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList id="ddlTaskTrackTaskType" runat="server" Enabled="true" TabIndex="5" />
                                </div>
                            </div>
                            <div id="divTaskDateTrackTask" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDateTrackTask" runat="server" Text="Fec. Traza" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDateTrackTask" runat="server" Enabled="false" />
                                    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender3" runat="server" TargetControlID="txtTaskDateTrackTask"
                                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                                            InputDirection="LeftToRight">
                                    </ajaxToolkit:MaskedEditExtender>    
                                </div>
                            </div>
                            <div id="divTaskLocStageSource" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskLocStageSource" runat="server" Text="Stage Origen" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTaskLocStageSource" runat="server" Enabled="true" TabIndex="6" />
                                </div>
                            </div>
                            <div id="divTaskLocStageTarget" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskLocStageTarget" runat="server" Text="Stage Destino" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTaskLocStageTarget" runat="server" Enabled="true" TabIndex="7" />
                                </div>
                            </div>
                            <div id="divTaskWorkersRequired" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskWorkersRequired" runat="server" Text="Oper. Requeridos" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskWorkersRequired" runat="server" Enabled="true" TabIndex="8" />
                                    <asp:RequiredFieldValidator ID="rfvTaskWorkersRequired" runat="server" ControlToValidate="txtTaskWorkersRequired" ValidationGroup="EditNew" Text=" * " ErrorMessage="Oper Requeridos" />
                                    <asp:RangeValidator ID="rvTaskWorkersRequired" runat="server" ControlToValidate="txtTaskWorkersRequired" ValidationGroup="EditNew" Text=" * " ErrorMessage="Oper Requerido debe ser entre 1 y 10" MinimumValue="1" MaximumValue="10" Type="Integer"/>
                                </div>
                            </div>
                            <div id="divTaskAllowCrossDock" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskAllowCrossDock" runat="server" Text="Permite CrossDock" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTaskAllowCrossDock" runat="server" Enabled="true" TabIndex="9" />
                                </div>
                            </div>
                            <div id="divTaskDateCreated" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDateCreated" runat="server" Text="Creada el" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDateCreated" runat="server" Enabled="false" />
                                    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="txtTaskDateCreated"
                                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                                            InputDirection="LeftToRight">
                                    </ajaxToolkit:MaskedEditExtender>    
                                </div>
                            </div>
                            <div id="divTaskUserCreated" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskUserCreated" runat="server" Text="Creada por" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskUserCreated" runat="server" Enabled="false" />
                                </div>
                            </div>
                            <div id="divTaskDateModified" runat="server" class="divControls">
                                
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDateModified" runat="server" Text="Modificada en" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDateModified" runat="server" Enabled="false" />
                                    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="txtTaskDateModified"
                                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                                            InputDirection="LeftToRight">
                                    </ajaxToolkit:MaskedEditExtender>    
                                </div>
                            </div>
                            <div id="divTaskUserModified" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskUserModified" runat="server" Text="Modificada por" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskUserModified" runat="server" Enabled="false" />
                                </div>
                            </div>
                        </div>
                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"    ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSaveTask" runat="server" OnClick="btnSaveTask_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancelTask" runat="server" Text="Cancelar" />
                        </div>                        
                    </div>
                </asp:Panel>
            </div>    
            <%-- FIN Pop up Editar Password --%>       
         
            </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />         
      </Triggers>
    </asp:UpdatePanel>  
    
    

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditTask" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />    
    <%-- FIN Modal Update Progress --%>
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterDate" runat="server" Text="Creada" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Destino" Visible="false" />
    <asp:Label ID="lblEmptyRow" runat="server" Text="(Ninguno)" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
    <iframe id="iframe" runat="server" style="visibility:hidden"></iframe>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
