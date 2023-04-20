<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaskPriorities.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Movement.Consult.TaskPriorities" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("TaskPriority_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("TaskPriority_FindAll", "ctl00_MainContent_grdMgr");
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
                                            <asp:Label ID="lblTaskTypeName" runat="server" Text='<%# Eval ( "Task.Description" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prioridad" AccessibleHeaderText="Priority" SortExpression="Priority">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPriority" runat="server" Text='<%# Eval ( "Task.Priority" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nueva Prioridad" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <div style="width: 60px">
                                                <center>
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" />
                                        
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
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Documento" AccessibleHeaderText="IdDocumentBoundDetail" SortExpression="DocumentBoundDetail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdDocumentBoundDetail" runat="server" Text='<%# ((int)Eval ( "TaskDetail.IdDocumentBound" )==-1)?"":Eval ( "TaskDetail.IdDocumentBound" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Doc. Salida" AccessibleHeaderText="OutboundNumber" SortExpression="OutboundNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Eval ( "OutboundOrder.Number" ) %>'></asp:Label>
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
                                    <asp:TemplateField HeaderText="Bodega" AccessibleHeaderText="HgnCode" SortExpression="HgnCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblHgnCode" runat="server" Text='<%# Eval ( "TaskDetail.LoadCode" ) %>'></asp:Label>
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
                     <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
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
                  
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate> 
            <%-- Pop up Editar/Nuevo Usuario --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlUser" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <%-- Encabezado --%>
                <asp:Panel ID="pnlUser" runat="server" CssClass="modalBox">
                    <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Prioridad" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidIsBaseUser" runat="server" Value="false" />
                        <div class="divCtrsFloatLeft">
                            <div id="divIdTask" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIdTask" runat="server" Text="Id Tarea" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtIdTask" runat="server" MaxLength="30" Width="150" Enabled="false" />
                                </div>
                            </div>
                            <div id="divTaskName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTaskName" runat="server" Text="Tarea" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTaskName" runat="server" MaxLength="30" Width="150" TabIndex="2" Enabled="false" />
                                </div>
                            </div>
                            <div id="divActualPriority" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblActualPriority" runat="server" Text="Prioridad" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtActualPriority" runat="server" MaxLength="30" Width="150" TabIndex="3" Enabled="false" />
                                </div>
                            </div>
                            <div id="divNewPriority" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblNewPriority" runat="server" Text="Nueva Prioridad" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtNewPriority" runat="server" MaxLength="30" Width="150" TabIndex="1"/>
                                    <asp:RequiredFieldValidator ID="rfvNewPriority" runat="server" ControlToValidate="txtNewPriority"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nueva Prioridad es requerida" />
                                    <asp:RangeValidator ID="rvNewPriority" runat="server" ControlToValidate="txtNewPriority" ValidationGroup="EditNew" Text=" * " ErrorMessage="Ingrese valores entre 1 y 10" MinimumValue="1" MaximumValue="10" Type="Integer" />
                                </div>
                            </div>
                        </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"    ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>                        
                    </div>
                </asp:Panel>
            </div>    
            <%-- FIN Pop up Editar Password --%>       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
      </Triggers>
    </asp:UpdatePanel>  

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />    
    <%-- FIN Modal Update Progress --%>
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterCode" runat="server" Text="Usuario" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Creacion Desde" Visible="false" />
    <asp:Label ID="lblFilterDescription" runat="server" Text="Apellido" Visible="false" />    
    <asp:Label ID="lblToolTipReset" runat="server" Text="Resetea la contraseña" Visible="false" />    
    <asp:Label ID="lblToolTipEditar" runat="server" Text="Editar Contraseña" Visible="false" />    
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
                
                
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>