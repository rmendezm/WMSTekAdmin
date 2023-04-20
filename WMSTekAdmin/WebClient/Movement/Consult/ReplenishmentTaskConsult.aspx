<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="ReplenishmentTaskConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Movement.ReplenishmentTaskConsult" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("ReplanishmentLog_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("ReplanishmentLog_FindAll", "ctl00_MainContent_grdMgr");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>  
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <%-- Grilla Principal --%>         
                            <asp:GridView ID="grdMgr" 
                            runat="server" 
                            AllowPaging="True" 
                            OnRowCreated="grdMgr_RowCreated"
                            EnableViewState="False" onrowdatabound="grdMgr_RowDataBound"
                            AutoGenerateColumns="false"
                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                            EnableTheming="false" >
 
                                <Columns>
                                <asp:TemplateField HeaderText="Cód. Centro" AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Task.Warehouse.Code" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                    
                                <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="Warehouse">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblShortWhsName" runat="server" text='<%# Eval ( "Task.Warehouse.ShortName" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>                    
                                     
                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Task.Owner.Code" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                    
                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOwnerName" runat="server" text='<%# Eval ( "Task.Owner.Name" ) %>'></asp:Label>
                                       </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                    
                                <asp:TemplateField HeaderText="Tipo Tarea" AccessibleHeaderText="TaskTypeCode">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:label ID="lblTypeCode" runat="server" text='<%# Eval ( "Task.TypeCode" ) %>' />
                                        </div>
                                    </ItemTemplate>
                                 </asp:TemplateField>        
                     
                                <asp:TemplateField HeaderText="Descripción Tarea" AccessibleHeaderText="TaskDescription">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblDescription" runat="server" text='<%# Eval ( "Task.Description" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>          
                    
                                <asp:TemplateField HeaderText="Prioridad" AccessibleHeaderText="TaskPriority">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblPriority" runat="server" text='<%# Eval ("Task.Priority") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>   
                    
                                <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="NameTrackTaskType">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblNameTrackTaskType" runat="server" text='<%# Eval ("TrackTaskType.Name") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField> 
                       
                    
                                <asp:TemplateField HeaderText="Creado" AccessibleHeaderText="CreateDate" SortExpression="CreateDate">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblCreateDate" runat="server"  text='<%# ((DateTime) Eval ("Task.CreateDate") > DateTime.MinValue)? Eval("Task.CreateDate"):"" %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                    
                                <asp:TemplateField HeaderText="Propuesta" AccessibleHeaderText="ProposalStartDate" SortExpression="ProposalStartDate">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblProposalStartDate" runat="server"  text='<%# ((DateTime) Eval ("Task.ProposalStartDate") > DateTime.MinValue)? Eval("Task.ProposalStartDate"):"" %>' />
                                       </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                        
                                <asp:TemplateField HeaderText="Inicio Real" AccessibleHeaderText="RealStartDate">
                                    <ItemStyle Wrap="false" />
                                    <ItemTemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblRealStartDate" runat="server"  
                                            text='<%# ((DateTime) Eval ("Task.RealStartDate") > DateTime.MinValue)? Eval("Task.RealStartDate", "{0:d}"):"" %>' />
                                            </div>
                                        </center>    
                                    </ItemTemplate>
                                </asp:TemplateField>     
                    
                                <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status" SortExpression="Status">
                                    <ItemStyle Wrap="false" />
                                    <ItemTemplate>
                                        <center>
                                            <asp:CheckBox ID="chkStatus" runat="server" checked='<%# Eval ( "Task.Status" ) %>'
                                             Enabled="false"/>
                                        </center>    
                                </ItemTemplate>
                                </asp:TemplateField>     
                    
                                <asp:TemplateField HeaderText="Op. Requeridos" AccessibleHeaderText="WorkersRequired" SortExpression="WorkersRequired">
                                    <ItemTemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblWorkersRequired" runat="server" 
                                                text='<%# ((int) Eval ("Task.WorkersRequired") == -1)?" ":Eval ("Task.WorkersRequired") %>' />
                                            </div>
                                        </center>    
                                    </ItemTemplate>
                                </asp:TemplateField>    
                    
                                <asp:TemplateField HeaderText="Op. Asignados" AccessibleHeaderText="WorkersAssigned">
                                    <ItemTemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblWorkersAssigned" runat="server" 
                                                text='<%# ((int) Eval ("Task.WorkersAssigned") == -1)?" ":Eval ("Task.WorkersAssigned") %>' />
                                           </div>
                                        </center>    
                                    </ItemTemplate>
                                </asp:TemplateField>                                                                           
                    
                                <asp:TemplateField HeaderText="Id Det." AccessibleHeaderText="TaskDetailId">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdTaskDetail" runat="server" text='<%# Eval ( "TaskDetail.Id" ) %>'></asp:Label>
                                       </div>
                                    </ItemTemplate>
                                </asp:TemplateField>                                                              
                                                                             
                                <asp:TemplateField HeaderText="Completa" AccessibleHeaderText="IsComplete" SortExpression="IsComplete">
                                    <ItemStyle Wrap="false" />
                                    <ItemTemplate>
                                        <center>
                                            <asp:CheckBox ID="chkTaskDetailIsComplete" runat="server" checked='<%# Eval ( "Task.IsComplete" ) %>' Enabled="false"/>
                                        </center>    
                                    </ItemTemplate>
                                </asp:TemplateField>   
                    
                                <asp:TemplateField HeaderText="Categoria" AccessibleHeaderText="CategoryItemName" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblCtgName" runat="server" text='<%# Eval ( "TaskDetail.CategoryItem.Name" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>        
                    
                                <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblItemCode" runat="server" text='<%# Eval ("TaskDetail.Item.Code") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>          
                    
                                <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("TaskDetail.Item.LongName") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>                                
                  
                                <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblCategoryItemCode" runat="server" text='<%# Eval ("TaskDetail.Item.Description") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Ubicación Origen" AccessibleHeaderText="IdLocSourceProposal" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdLocSourceProposal" runat="server" text='<%# Eval ( "TaskDetail.IdLocSourceProposal" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>    
        
                                <asp:TemplateField HeaderText="Ubicación Destino" AccessibleHeaderText="IdLocTargetProposal" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdLocTargetProposal" runat="server" text='<%# Eval ( "TaskDetail.IdLocTargetProposal" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>      
                    
                                <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnSourceProposal" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdLpnSourceProposal" runat="server" text='<%# Eval ( "TaskDetail.IdLpnSourceProposal" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>   
                                        
                                <asp:TemplateField HeaderText="Cant. a Reponer" AccessibleHeaderText="ProposalQty">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblProposalQty" runat="server" 
                                                text='<%#GetFormatedNumber( ((decimal)Eval("TaskDetail.ProposalQty") == -1) ? " " : Eval("TaskDetail.ProposalQty"))%>'>
                                            </asp:Label>
                                         </div>
                                    </ItemTemplate>
                                </asp:TemplateField>   
                                                         
                                <asp:TemplateField HeaderText="Cant. Repuesta" AccessibleHeaderText="RealQty" SortExpression="RealQty">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblRealQty" runat="server"
                                                text='<%# GetFormatedNumber(((decimal)Eval("TaskDetail.RealQty") == -1) ? " " : Eval("TaskDetail.RealQty"))%>'>
                                            </asp:Label>               
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>  
                    
                                <asp:TemplateField HeaderText="Usuario Asignado" AccessibleHeaderText="UserAssigned" SortExpression="UserAssigned" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label runat="server" ID="lblUserAssigned" text='<%# Eval ("TaskDetail.UserAssigned") %>' /></asp:Label>        
                                       </div>
                                    </ItemTemplate>
                                </asp:TemplateField>   
                             </Columns>
                     
                          </asp:GridView>
                            <%-- FIN Grilla Principal --%>             
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

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%>                  
     
    <%-- Mensajes de Confirmacion y Auxiliares --%>
	<asp:Label id="lblFilterDate" runat="server" Text="Creación" Visible="false" />    	
	<asp:Label id="lblFilterStatus" runat="server" Text="Completada" Visible="false" />  
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>