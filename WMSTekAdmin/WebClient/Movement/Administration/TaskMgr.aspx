<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="TaskMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Movement.Consult.TaskMgr" %>
<%@ Register TagPrefix="obout" Namespace="OboutInc.Calendar2" Assembly="obout_Calendar2_NET" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("TaskMgr_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("TaskMgr_FindAll", "ctl00_MainContent_grdMgr");
    }

    function showProgress() {

        if (document.getElementById('ctl00_MainContent_uploadFile').value.length > 0 &&
            document.getElementById("ctl00_MainContent_ddlOwnerLoad").value > 0) {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modalLoading");
                $('body').append(modal);

                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 3.5, 0);
                var left = Math.max($(window).width() / 2.6, 0);
                loading.css({ top: top, left: left });
            }, 10);
            return true;

        } else {
            return false;
        }
    }

    function ShowMessage(title, message) {
        var position = (document.body.clientWidth - 400) / 2 + "px";
        document.getElementById("divFondoPopup").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.marginLeft = position;

        document.getElementById("ctl00_MainContent_lblDialogTitle").innerHTML = title;
        document.getElementById("ctl00_MainContent_divDialogMessage").innerHTML = message;

        return false;
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" 
                                runat="server" 
                                DataKeyNames="Id" OnRowCommand="grdMgr_RowCommand"
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
                                    <asp:TemplateField HeaderText="Editar Tarea" AccessibleHeaderText="TaskActions">
                                        <ItemTemplate>
                                            <div style="width: 60px">
                                                <center>
                                                    <asp:ImageButton ID="btnEditTask" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false"  CommandName="EditTask" ToolTip="Editar Tarea"/>

                                                    <asp:ImageButton ID="btnCreateExcelForPickVoicing" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_down.png"
                                                        CausesValidation="false"  CommandName="CreateExcelForPickVoicing" ToolTip="Generar Excel" />

                                                    <asp:ImageButton ID="btnReceiveExcelForPickVoicing" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_up.png"
                                                        CausesValidation="false"  CommandName="ReceiveExcelForPickVoicing" ToolTip="Recibir Excel" />
                                        
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
                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="WhsName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Task.Warehouse.ShortName" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerId" runat="server" Text='<%# Eval ( "Task.Owner.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ( "Task.Owner.Name" ) %>'></asp:Label>
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
                                    <asp:TemplateField HeaderText="Creada" AccessibleHeaderText="CreateDate" SortExpression="CreateDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCreateDate" runat="server" Text='<%# Eval ( "Task.CreateDate" ) %>'></asp:Label>
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
                                    <asp:TemplateField HeaderText="Prioridad" AccessibleHeaderText="Priority" SortExpression="Priority">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPriority" runat="server" Text='<%# Eval ( "Task.Priority" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Editar movimiento" AccessibleHeaderText="TaskDetailActions">
                                        <ItemTemplate>
                                            <div style="width: 60px">
                                                <center>
                                                    <asp:ImageButton ID="btnEditTaskDetail" runat="server" CausesValidation="false" CommandName="EditTaskDetail" ToolTip="Editar Movimiento"
                                                        ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png" />
                                                </center>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ID Detalle" AccessibleHeaderText="IdTaskDetail" SortExpression="TaskDetail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdTaskDetail" runat="server" Text='<%# Eval ( "TaskDetail.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ejecutada" AccessibleHeaderText="IsCompleteDetail" SortExpression="IsCompleteDetail">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkIsCompleteDetail" runat="server" checked='<%# Eval (  "TaskDetail.IsComplete" ) %>' Enabled="false"/>
                                            <%--<asp:Label ID="lblIsCompleteDetail" runat="server" Text='<%# Eval ( "TaskDetail.IsComplete" ) %>'></asp:Label>--%>
                                            <center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prioridad Movto." AccessibleHeaderText="PriorityDetail" SortExpression="PriorityDetail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPriorityDetail" runat="server" Text='<%# Eval ( "TaskDetail.Priority" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Documento" AccessibleHeaderText="IdDocumentBoundDetail" SortExpression="DocumentBoundDetail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdDocumentBoundDetail" runat="server" Text='<%# ((int)Eval ( "TaskDetail.IdDocumentBound" )==-1)?"":Eval ( "TaskDetail.IdDocumentBound" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Documento" AccessibleHeaderText="OutboundNumber" SortExpression="OutboundNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Eval ( "OutboundOrder.Number" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nro Referencia" AccessibleHeaderText="ReferenceNumber" SortExpression="ReferenceNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblReferenceNumber" runat="server" Text='<%# Eval ( "OutboundOrder.ReferenceNumber" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Codigo Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "TaskDetail.Item.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName" SortExpression="LongItemName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ( "TaskDetail.Item.LongName" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ProposalQty" SortExpression="ProposalQty">
                                        <ItemTemplate>
                                            <asp:Label ID="lblProposalQty" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval ( "TaskDetail.ProposalQty" )==-1)?"":Eval ( "TaskDetail.ProposalQty" )) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Categoria" AccessibleHeaderText="IdCtgItem" SortExpression="CtgItem">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdCtgItem" runat="server" Text='<%# ((int)Eval ( "TaskDetail.CategoryItem.Id" )==-1)?"":Eval ( "TaskDetail.CategoryItem.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Categoria" AccessibleHeaderText="CtgName" SortExpression="CtgName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval ( "TaskDetail.CategoryItem.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Asignado a" AccessibleHeaderText="UserAssigned" SortExpression="UserAssigned">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUserAssigned" runat="server" Text='<%# Eval ( "TaskDetail.UserAssigned" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ubic. Origen" AccessibleHeaderText="IdLocSourceProposal" SortExpression="LocSourceProposal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLocSourceProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLocSourceProposal" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ubic. Destino" AccessibleHeaderText="IdLocTargetProposal" SortExpression="LocTargetProposal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLocTargetProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLocTargetProposal" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LPN Origen" AccessibleHeaderText="IdLpnSourceProposal" SortExpression="LpnSourceProposal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLpnSourceProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLpnSourceProposal" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Usuario Creador" AccessibleHeaderText="UserCreated" SortExpression="UserCreated">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUserCreated" runat="server" Text='<%# Eval ( "Task.UserCreated" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Activa" AccessibleHeaderText="StatusDetail" SortExpression="StatusDetail">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkStatusDetail" runat="server" checked='<%# Eval (  "TaskDetail.Status" ) %>' Enabled="false"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bodega" AccessibleHeaderText="HgnCode" SortExpression="HgnCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblHgnCode" runat="server" Text='<%# Eval ( "TaskDetail.LoadCode" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Presentación por Defecto" AccessibleHeaderText="ItemUomName" SortExpression="ItemUomName">
	                                    <ItemTemplate>
		                                    <center>
			                                    <div style="word-wrap: break-word;">
				                                    <asp:Label ID="lblItemUomName" runat="server" Text='<%# Eval ("TaskDetail.Item.ItemUom.Name") %>' />
			                                    </div>
		                                    </center>
	                                    </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>    

                        </div>
                        <%-- Grilla Principal --%>
                   </ContentTemplate>
                   <Triggers>
                     <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                     <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                     <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                     <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                     <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                     <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />         
                     <asp:AsyncPostBackTrigger ControlID="btnSaveTask" EventName="Click" />
                     <asp:AsyncPostBackTrigger ControlID="btnSaveTaskDetail" EventName="Click" />
                  </Triggers>
                </asp:UpdatePanel>  
            </div>
        </div>
    </div>       
    
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%>
                  
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
                                    <asp:RequiredFieldValidator ID="rfvTaskPriority" runat="server" ControlToValidate="txtTaskPriority" 
                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad es requerido" />
                                    <asp:RangeValidator ID="rvTaskPriority" runat="server" ControlToValidate="txtTaskPriority" ValidationGroup="EditNew" Text=" * " 
                                    ErrorMessage="Prioridad debe ser entre 1 y 10" MinimumValue="1" MaximumValue="10" Type="Integer" />
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
            <%-- Pop up Editar Tarea Detalle--%>
            <div id="divEditTaskDetail" runat="server" visible="false">
                <asp:Button ID="btnDummyDetail" runat="Server" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpDetail" runat="server" TargetControlID="btnDummyDetail"
                    PopupControlID="pnlUserDetail" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <%-- Encabezado --%>
                <asp:Panel ID="pnlUserDetail" runat="server" CssClass="modalBox">
                    <asp:Panel ID="UserCaptionDetail" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblEditTaskDetail" runat="server" Text="Editar Tarea Detalle" />
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageAlign="Top" ToolTip="Cerrar ventana de detalle" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditIdDetail" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidIsBaseUserDetail" runat="server" Value="false" />
                        <div class="divCtrsFloatLeft">
                            <div id="divTaskDetailId" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailId" runat="server" Text="Id Detalle" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailId" runat="server" MaxLength="30" Width="150" Enabled="false" TabIndex="1"/>
                                </div>
                            </div>
                            <div id="divTaskDetailIdTask" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailIdTask" runat="server" Text="Id Tarea" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailIdTask" runat="server" MaxLength="30" Width="150" Enabled="false" TabIndex="2" />
                                </div>
                            </div>
                            <div id="divTaskDetailIsComplete" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailIsComplete" runat="server" Text="Ejecutada" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTaskDetailIsComplete" runat="server" MaxLength="30" Width="150" TabIndex="3" />    
                                </div>
                            </div>
                            <div id="divTaskDetailDocumentBound" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailDocumentBound" runat="server" Text="Documento" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailDocumentBound" runat="server" MaxLength="30" Width="150" Enabled="false" TabIndex="4" />
                                </div>
                            </div>
                            <div id="divTaskDetailIdItem" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailIdItem" runat="server" Text="Codigo Item" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailIdItem" runat="server" Enabled="false" TabIndex="5"/>
                                </div>
                            </div>
                            <div id="divTaskDetailLongItemName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailLongItemName" runat="server" Text="Item" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailLongItemName" runat="server" Enabled="false" TabIndex="6" />
                                </div>
                            </div>
                            <div id="divTaskDetailIdCtgItem" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailIdCtgItem" runat="server" Text="Categoria" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailIdCtgItem" runat="server" MaxLength="60" TabIndex="7" />
                                </div>
                            </div>
                            <div id="divTaskDetailPriority" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailPriority" runat="server" Text="Prioridad" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailPriority" runat="server" Enabled="false" TabIndex="8" />
                                    <asp:RequiredFieldValidator ID="rfvTaskDetailPriority" runat="server" ControlToValidate="txtTaskDetailPriority" ValidationGroup="EditNewDet" Text=" * " ErrorMessage="Prioridad es requerido" />
                                    <asp:RangeValidator ID="rvTaskDetailPriority" runat="server" ControlToValidate="txtTaskDetailPriority" ValidationGroup="EditNewDet" Text=" * " ErrorMessage="Prioridad debe ser entre 1 y 10" MinimumValue="1" MaximumValue="10" Type="Integer" />
                                </div>
                            </div>
                            <div id="divTaskDetailLocSourceProposal" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailLocSourceProposal" runat="server" Text="Ubic. Origen" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailLocSourceProposal" runat="server" Enabled="true" TabIndex="9" />
                                </div>
                            </div>
                            <div id="divTaskDetailLocForkliftProposal" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailLocForkliftProposal" runat="server" Text="Maquina" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTaskDetailLocForkliftProposal" runat="server" Enabled="true" TabIndex="10" />
                                </div>
                            </div>
                            
                         </div>
                         <div class="divCtrsFloatLeft">
                            <div id="divTaskDetailLocTargetProposal" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailLocTargetProposal" runat="server" Text="Ubic. Destino" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox id="txtTaskDetailLocTargetProposal" runat="server" Enabled="true" TabIndex="11" />
                                </div>
                            </div>
                            <div id="divTaskDetailStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailStatus" runat="server" Text="Status" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTaskDetailStatus" runat="server" Enabled="true" TabIndex="12" />
                                </div>
                            </div>
                            <div id="divTaskDetailProposalQty" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailProposalQty" runat="server" Text="Cantidad" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailProposalQty" runat="server" Enabled="true" TabIndex="13" />
                                </div>
                            </div>
                            <div id="divTaskDetailUserAssigned" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailUserAssigned" runat="server" Text="Operador Asignado" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTaskDetailUserAssigned" runat="server" Enabled="true" TabIndex="14" />
                                </div>
                            </div>
                            <div id="divTaskDetailStartDate" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailStartDate" runat="server" Text="Fec. Ejecucion" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailStartDate" runat="server" Enabled="false" />
                                    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender6" runat="server" TargetControlID="txtTaskDetailStartDate"
                                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                                            InputDirection="LeftToRight">
                                    </ajaxToolkit:MaskedEditExtender>    
                                </div>
                            </div>
                            <div id="divTaskDetailMadeCrossDock" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailMadeCrossDock" runat="server" Text="Hizo CrossDock" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailMadeCrossDock" runat="server" Enabled="true" TabIndex="15" />
                                </div>
                            </div>
                            <div id="divTaskDetailDateCreated" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailDateCreated" runat="server" Text="Creada el" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailDateCreated" runat="server" Enabled="false" />
                                    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender7" runat="server" TargetControlID="txtTaskDetailDateCreated"
                                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                                            InputDirection="LeftToRight">
                                    </ajaxToolkit:MaskedEditExtender>    
                                </div>
                            </div>
                            <div id="divTaskDetailUserCreated" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailUserCreated" runat="server" Text="Creada por" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailUserCreated" runat="server" Enabled="false" />
                                </div>
                            </div>
                            <div id="divTaskDetailDateModified" runat="server" class="divControls">
                                
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailDateModified" runat="server" Text="Modificada en" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailDateModified" runat="server" Enabled="false" />
                                    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender8" runat="server" TargetControlID="txtTaskDetailDateModified"
                                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                                            InputDirection="LeftToRight">
                                    </ajaxToolkit:MaskedEditExtender>    
                                </div>
                            </div>
                            <div id="divTaskDetailUserModified" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskDetailUserModified" runat="server" Text="Modificada por" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskDetailUserModified" runat="server" Enabled="false" />
                                </div>
                            </div>
                        </div>
                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="EditNewDet"    ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="div1" runat="server" class="modalActions">
                            <asp:Button ID="btnSaveTaskDetail" runat="server" OnClick="btnSaveTaskDetail_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancelTaskDetail" runat="server" Text="Cancelar" />
                        </div>                        
                    </div>
                </asp:Panel>
            </div>    
            <%-- FIN Pop up Editar Tarea Detalle --%>       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
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

    <%-- Carga Excel Voice Picking --%>
    <asp:UpdatePanel ID="upLoadVoicePicking" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoadVoicePicking" runat="server" visible="false">
                <asp:Button ID="btnDummyLoadVoicePicking" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpLoadVoicePicking" runat="server" TargetControlID="btnDummyLoadVoicePicking"
                    PopupControlID="panelLoad" BackgroundCssClass="modalBackground" PopupDragHandleControlID="panelCaptionLoadVoicePicking"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelLoad" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionLoadVoicePicking" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:HiddenField runat="server" ID="hidIdTaskSelected" />
                            <asp:Label ID="Label3" runat="server" Text="Carga Excel de PTL" />
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga PTL.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />   
                            <asp:ImageButton ID="ImageButton2" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                        
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label5" runat="server" Text="Seleccione Archivo" />
                                </div>
                                <div class="fieldLeft">                      
                                    <asp:FileUpload ID="uploadFile" runat="server" Width="400px" ValidationGroup="LoadVoicePicking"/>                                    
                                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo"
                                    ValidationGroup="LoadVoicePicking" ControlToValidate="uploadFile"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator id="revUpload" runat="server"  ValidationGroup="LoadVoicePicking"
                                    ErrorMessage="Cargar solo archivos .xls ó .xlsx"  Text="*"
                                    ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$" 
                                    ControlToValidate="uploadFile">
                                    </asp:RegularExpressionValidator>                             
                                    
                                </div>  
                            </div>
                            <div class="divControls" >
                                <div class="fieldRight">
                                    <asp:Label ID="Label6" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnSubirVoicePicking" runat="server" Text="Cargar Archivo" ValidationGroup="LoadVoicePicking" 
                                    OnClientClick="showProgress()" onclick="btnSubirVoicePicking_Click" />
                                </div>
                            </div>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="LoadVoicePicking"
                                ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div3" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoadbtnSubirVoicePicking" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubirVoicePicking" />
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="uprLoadVoicePicking" runat="server" AssociatedUpdatePanelID="upLoadVoicePicking" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgressVoicePicking" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprLoadVoicePicking" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLoadVoicePicking" />    
    <%-- Fin Carga Excel Voice Picking --%>

    <div id="divFondoPopup" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;">
    </div>
    <div id="divMensaje" class="modalBox" style="z-index: 400001; display: none; position: absolute; 
        width: 400px;  top: 200px; margin-top: 0;"  runat="server">
        
        <div id="divDialogTitleMessage" runat="server" class="modalHeader">
			<div class="divCaption">
			    <asp:Label ID="lblDialogTitle" runat="server" />
            </div>
	    </div>
	    <div id="divPanelMessage" class="divDialogPanel" runat="server">
        
            <div class="divDialogMessage">
                <asp:Image id="Image1" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
            </div>
            <div id="divDialogMessage" runat="server" class="divDialogMessage" />        
            <div id="divAlert" runat="server" visible="true" class="divDialogButtons">
                <asp:Button ID="btnMessageInfo" runat="server" Text="Aceptar"  OnClientClick="return HideMessage();" />
            </div>    
        </div>
               
    </div>    
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblEmptyRow" runat="server" Text="(Ninguno)" Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="Usuario" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Creacion Desde" Visible="false" />
    <asp:Label ID="lblFilterDescription" runat="server" Text="Apellido" Visible="false" />    
    <asp:Label ID="lblToolTipReset" runat="server" Text="Resetea la contraseña" Visible="false" />    
    <asp:Label ID="lblToolTipEditar" runat="server" Text="Editar Contraseña" Visible="false" />    
    <asp:Label ID="lblWarningEditTaskFromActiveProcess" runat="server" Text="Modificar una tarea en proceso, puede alterar el flujo normal de las operaciones logísticas. ¿Realmente desea continuar?" Visible="false" />
	<asp:Label ID="lblWarningEditTaskFromActiveClose" runat="server" Text="No se puede modificar una tarea en estado cerrado o en proceso de cierre" Visible="false" />    
	<asp:Label ID="lblWarningEditTaskDetailFromActiveClose" runat="server" Text="No se puede modificar el detalle de la tarea si se encuentra en proceso de cierre del detalle" Visible="false" /> 
	<asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es valído." Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Cargar Excel PTL" Visible="false" />
    <asp:Label ID="lblNotFileLoad" runat="server" Text="Seleccione el archivo que desea cargar." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblValidateChangeComplete" runat="server" Text="No se puede modificar tarea ya que esta siendo usada por usuario [USER]" Visible="false" />
    <asp:Label ID="lblBatchNbr" runat="server" Text="N° Batch" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
                
                
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
