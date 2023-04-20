<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdjustmentConsult.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Inventory.InventoryWebConsult" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("TaskConsult_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("TaskConsult_FindAll", "ctl00_MainContent_grdMgr");
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
                                    AllowSorting="False" 
                                    OnRowCreated="grdMgr_RowCreated"
                                    EnableViewState="false"
                                    AutoGenerateColumns="false"
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false">
                        
                                     <Columns>
                                         <asp:TemplateField HeaderText="Id" AccessibleHeaderText="Id" SortExpression="Id">
                                             <itemtemplate>
                                                <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval ( "Task.Id" ) %>' ></asp:Label>
                                                </div>  
                                                 </center>
                                             </itemtemplate>
                                         </asp:TemplateField>
                                                 
                                         <asp:TemplateField HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" SortExpression="WarehouseCode" ItemStyle-CssClass="text">
                                             <itemtemplate>
                                                <center>
                                                 <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Task.Warehouse.Code" ) %>' ></asp:Label>
                                                 </div> 
                                                 </center>
                                             </itemtemplate>
                                         </asp:TemplateField>
                             
                                         <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse" SortExpression="Warehouse">
                                             <itemtemplate>
                                                <center>
                                                 <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Task.Warehouse.ShortName" ) %>' ></asp:Label>
                                                </div>
                                                 </center>
                                             </itemtemplate>
                                         </asp:TemplateField>
                                                  
                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Task.Owner.Code" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnerTradeName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnerTradeName" runat="server" text='<%# Eval ( "Task.Owner.Name" ) %>'></asp:Label>
                                                </div>  
                                            </ItemTemplate>
                                        </asp:TemplateField>          
                            
                                        <asp:TemplateField HeaderText="Tipo Tarea" AccessibleHeaderText="TypeCode">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblTypeCode" runat="server" text='<%# Eval ( "Task.TypeCode" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>     
                            
                                        <asp:TemplateField HeaderText="Descripción Tarea" AccessibleHeaderText="Description">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblDescription" runat="server" text='<%# Eval ( "Task.Description" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                                       

                                       <asp:templatefield headertext="Prioridad" accessibleHeaderText="TaskPriority" SortExpression="TaskPriority">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblPriority" runat="server" 
                                                        text='<%# ((int) Eval ("Task.Priority") == -1)?" ":Eval ("Task.Priority") %>' />
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>
                            
                                        <asp:templatefield headertext="Creación" accessibleHeaderText="CreateDate" SortExpression="CreateDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblCreateDate" runat="server"  
                                                        text='<%# ((DateTime) Eval ("Task.CreateDate") > DateTime.MinValue)? Eval("Task.CreateDate", "{0:d}"):"" %>' />
                                                    </div>
                                                </center>    
                                        </itemtemplate>
                                        </asp:templatefield>           
                            
                                        <asp:templatefield headertext="Inicio Prop." accessibleHeaderText="ProposalStartDate" SortExpression="ProposalStartDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblProposalStartDate" runat="server"  
                                                        text='<%# ((DateTime) Eval ("Task.ProposalStartDate") > DateTime.MinValue)? Eval("Task.ProposalStartDate", "{0:d}"):"" %>' />
                                                    </div>
                                                </center>    
                                        </itemtemplate>
                                        </asp:templatefield>    
                              
                                        <asp:templatefield headertext="Inicio Real" accessibleHeaderText="RealStartDate" SortExpression="RealStartDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblRealStartDate" runat="server"  
                                                        text='<%# ((DateTime) Eval ("Task.RealStartDate") > DateTime.MinValue)? Eval("Task.RealStartDate", "{0:d}"):"" %>' />
                                                   </div>
                                                </center>    
                                        </itemtemplate>
                                        </asp:templatefield>                                              
                            
                                        <asp:templatefield headertext="Activo" accessibleHeaderText="Status" SortExpression="Status">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkStatus" runat="server" checked='<%# Eval ( "Task.Status" ) %>'
                                                     Enabled="false"/>
                                                </center>    
                                        </itemtemplate>
                                        </asp:templatefield>   
                            
                                       <asp:templatefield headertext="Op. Asignados" accessibleHeaderText="WorkersAssigned" SortExpression="WorkersAssigned">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblWorkersAssigned" runat="server" 
                                                        text='<%# ((int) Eval ("Task.WorkersAssigned") == -1)?" ":Eval ("Task.WorkersAssigned") %>' />
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>                        
                                                
                                       <asp:templatefield headertext="Op. Requeridos" accessibleHeaderText="WorkersRequired" SortExpression="WorkersRequired">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblWorkersRequired" runat="server" 
                                                        text='<%# ((int) Eval ("Task.WorkersRequired") == -1)?" ":Eval ("Task.WorkersRequired") %>' />
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>  
                                                     
                                         <asp:TemplateField HeaderText="Id Det." AccessibleHeaderText="TaskDetailId" SortExpression="TaskDetailId">
                                             <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblTaskDetailId" runat="server" Text='<%# Eval ( "TaskDetail.Id" ) %>' ></asp:Label>
                                                    </div>
                                                 </center>
                                             </itemtemplate>
                                         </asp:TemplateField>
                                                     
                                        <asp:templatefield headertext="Completa" accessibleHeaderText="IsComplete" SortExpression="IsComplete">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkTaskDetailIsComplete" runat="server" checked='<%# Eval ( "Task.IsComplete" ) %>' Enabled="false"/>
                                                </center>    
                                        </itemtemplate>
                                        </asp:templatefield>       
                            
                                        <asp:templatefield headertext="Categoría" accessibleHeaderText="CategoryItemName" SortExpression="CategoryItemName">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblTaskDetailCategoryItemName" runat="server" text='<%# Eval ("TaskDetail.CategoryItem.Name") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>                    
                                                 
                                        <asp:templatefield headertext="Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblTaskDetailItemCode" runat="server" text='<%# Eval ("TaskDetail.Item.Code") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>

                                        <asp:templatefield headertext="Nombre" accessibleHeaderText="ItemLongName" SortExpression="ItemLongName">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblTaskDetailItemLongName" runat="server" text='<%# Eval ("TaskDetail.Item.LongName") %>' />
                                               </div>
                                            </itemtemplate>
                                        </asp:templatefield>                                           
                                                       
                                        <asp:templatefield headertext="Descripción" accessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblTaskDetailItemName" runat="server" text='<%# Eval ("TaskDetail.Item.Description") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>  
                            
                                        <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocSourceProposal" ItemStyle-CssClass="text">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblIdLocSourceProposal" runat="server" text='<%# Eval ( "TaskDetail.IdLocSourceProposal" ) %>'></asp:Label>
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
                            
                                        <asp:TemplateField HeaderText="Cant. Conteo" AccessibleHeaderText="ProposalQty">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblProposalQty" runat="server" 
                                                        text='<%# GetFormatedNumber(((decimal)Eval("TaskDetail.ProposalQty") == -1) ? " " : Eval("TaskDetail.ProposalQty"))%>'>
                                                    </asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>   
                                                                 
                                        <asp:templatefield headertext="Cant. Ajuste" accessibleHeaderText="RealQty" SortExpression="RealQty">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblRealQty" runat="server"
                                                        text='<%# GetFormatedNumber(((decimal)Eval("TaskDetail.RealQty") == -1) ? " " : Eval("TaskDetail.RealQty"))%>'>
                                                    </asp:Label>               
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>  
                            
                                        <asp:templatefield headertext="Usuario Asignado" accessibleHeaderText="UserAssigned" SortExpression="UserAssigned" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label runat="server" ID="lblUserAssigned" text='<%# Eval ("TaskDetail.UserAssigned") %>' /></asp:Label>        
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>   
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
     
    <%-- Mensajes de Confirmacion y Auxiliares --%>
	<asp:Label id="lblFilterDate" runat="server" Text="Creación" Visible="false" />    	
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>