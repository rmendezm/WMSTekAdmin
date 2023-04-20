<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="MovementTaskMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Movement.Consult.MovementTaskMgr"  %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language='Javascript'>
    window.onresize = SetDivs;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("MovementTaskMgr_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop();
        removeFooter("#ctl00_MainContent_grdSearchLocations");

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
        initializeGridDragAndDrop("MovementTaskMgr_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop();
        removeFooter("#ctl00_MainContent_grdSearchLocations");
    }
</script> 

<div id="divPrincipal" style="margin:0px;margin-bottom:80px">

    <%-- Panel Grilla Principal --%>
    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
        <ContentTemplate>
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id" 
                                OnRowCreated="grdMgr_RowCreated"
                                AllowPaging="True" EnableViewState="false"
                                AutoGenerateColumns="False"
                                OnRowDataBound="grdMgr_RowDataBound"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <%--<asp:TemplateField HeaderText="Editar Tarea" AccessibleHeaderText="TaskActions">
                                        <ItemTemplate>
                                            <div style="width: 60px">
                                                <center>
                                                    <asp:ImageButton ID="btnEditTask" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false"  CommandName="EditTask" ToolTip="Editar Tarea"/>
                                        
                                                </center>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
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
                                            <asp:Label ID="lblCreateDate" runat="server" Text='<%# ((DateTime) Eval ("Task.CreateDate") > DateTime.MinValue)? Eval("Task.CreateDate", "{0:d}"):"" %>'></asp:Label>
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
                                    <%--<asp:TemplateField HeaderText="Editar movimiento" AccessibleHeaderText="TaskDetailActions">
                                        <ItemTemplate>
                                            <div style="width: 60px">
                                                <center>
                                                    <asp:ImageButton ID="btnEditTaskDetail" runat="server" CausesValidation="false" CommandName="EditTaskDetail" ToolTip="Editar Movimiento"
                                                        ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png" />
                                                </center>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="ID Detalle" AccessibleHeaderText="IdTaskDetail" SortExpression="TaskDetail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdTaskDetail" runat="server" Text='<%# Eval ( "TaskDetail.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ejecutada" AccessibleHeaderText="IsCompleteDetail" SortExpression="IsCompleteDetail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIsCompleteDetail" runat="server" Text='<%# Eval ( "TaskDetail.IsComplete" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prioridad Movto." AccessibleHeaderText="PriorityDetail" SortExpression="PriorityDetail">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPriorityDetail" runat="server" Text='<%# Eval ( "TaskDetail.Priority" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Lpn Origen" AccessibleHeaderText="IdLpnSourceProposal" SortExpression="LpnSourceProposal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLpnSourceProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLpnSourceProposal" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Lpn Destino" AccessibleHeaderText="IdLpnTargetProposal" SortExpression="LpnTargetProposal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLpnTargetProposal" runat="server" Text='<%# Eval ( "TaskDetail.IdLpnTargetProposal" ) %>'></asp:Label>
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
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <%-- FIN Grilla Principal --%>   
            
             <%-- Pop up Editar/Nuevo Tarea --%>
            <div id="divModal" runat="server" visible="false">
                <asp:Panel ID="pnlCarrier" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="CarrierCaption" runat="server" CssClass="modalHeader">
                       <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Generar Nueva Tarea" />
                            <%--<asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />--%>
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalBoxContent">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                        
                            <div id="divStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStatus" runat="server" Text="Tipo de Mov." /></div>
                                <div class="fieldLeft">
                                    <asp:RadioButton ID="rbTaskTypeItem" runat="server" Text="Por Item"  AutoPostBack="true"
                                    GroupName="groupRadioBtt" Checked="true" OnCheckedChanged="RadioButton_CheckedChanged" />
                                    <asp:RadioButton ID="rbTaskTypeLpn"  runat="server" Text="Por Lpn"  AutoPostBack="true"
                                    GroupName="groupRadioBtt" OnCheckedChanged="RadioButton_CheckedChanged"/>
                                </div>
                            </div>  
                            <br />
                            <div id="divWarehouse" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWarehouse" runat="server" Text="Centro" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlWarehouseTask" runat="server" Width="150px"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlWarehouseTask_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvWarehouseTask" runat="server" ValidationGroup="AddNew"
                                        Text=" * " ErrorMessage="Centro es requerido" ControlToValidate="ddlWarehouseTask" Display="Dynamic" 
                                        InitialValue="-1" />
                                </div>
                            </div>
                                                        
                            <div id="divOwner" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlOwnerTask" runat="server" Width="150px" 
                                     AutoPostBack="true" OnSelectedIndexChanged="ddlWarehouseTask_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvOwnerTask" runat="server" ValidationGroup="AddNew"
                                        Text=" * " ErrorMessage="Dueño es requerido" ControlToValidate="ddlOwnerTask" Display="dynamic" InitialValue="-1" />
                                </div>
                            </div>
                            
                            <div id="divPriority" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPriority" runat="server" Text="Prioridad" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPriority" runat="server" MaxLength="2"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPriority" runat="server" ValidationGroup="AddNew"
                                        Text=" * " ErrorMessage="Prioridad es requerido" ControlToValidate="txtPriority" Display="dynamic"/>
                                    <asp:RangeValidator ID="rvPriority" runat="server" ControlToValidate="txtPriority"
                                        ErrorMessage="Prioridad no contiene un número válido" MaximumValue="10" MinimumValue="1"
                                        ValidationGroup="AddNew" Type="Integer">*</asp:RangeValidator>
                                </div>
                            </div>
                            
                            <br />
                            <div  id="divTaskByItem" runat="server" class="divControls" visible="true" >
                                <div id="divSourceLocItem" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblSourceLocItem" runat="server" Text="Ubic. Origen" /></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtSourceLocItem" runat="server" MaxLength="20" Width="150" />  
                                        <asp:ImageButton ID="ImageButton1" runat="server" Height="18px" OnClick="imgBtnSearchLocation_Click"  CommandName="txtSourceLocItem"
                                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" Width="18px" ValidationGroup="searchItem" />  
                                        <asp:RequiredFieldValidator ID="rfvSourceLocItem" runat="server" ControlToValidate="txtSourceLocItem" ValidationGroup="AddNew" Text=" * " ErrorMessage="Ubicación origen es requerido" />                                                                   
                                    </div>
                                </div>
                                <div id="divSourceLpnItem" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblSourceLpnItem" runat="server" Text="Lpn Origen" /></div>
                                    <div class="fieldLeft">
                                        <asp:DropDownList ID="ddlSourceLpnItem" runat="server" Width="155px" AutoPostBack="true" OnSelectedIndexChanged="ddlSourceLpnItem_SelectedIndexChanged"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvSourceLpnItem" runat="server" ControlToValidate="ddlSourceLpnItem"
                                        InitialValue="-1" ValidationGroup="AddNew" Text=" * " ErrorMessage="Lpn origen es requerido" />
                                        
                                       <%-- <asp:TextBox ID="txtSourceLpnItem" runat="server" MaxLength="20" Width="150" Visible="false" />  
                                        <asp:RequiredFieldValidator ID="rfvSourceLpnItem" runat="server" ControlToValidate="txtSourceLpnItem" ValidationGroup="AddNew" Text=" * " ErrorMessage="Lpn origen es requerido" />    --%>                                                                   
                                    </div>
                                </div>
                                <div id="divCodItem" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblCodItem" runat="server" Text="Item" /></div>
                                    <div class="fieldLeft">
                                        <asp:DropDownList ID="ddlCodItem" runat="server" Width="155px" AutoPostBack="true" OnSelectedIndexChanged="ddlCodItem_SelectedIndexChanged"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvCodItem" runat="server" ControlToValidate="ddlCodItem"
                                        InitialValue="-1" ValidationGroup="AddNew" Text=" * " ErrorMessage="Item es requerido" />
                                        
                                       <%-- <asp:TextBox ID="txtCodItem" runat="server" MaxLength="20" Width="150" Visible="false" />
                                        <asp:ImageButton ID="imgbtnSearchItem" runat="server" Height="18px" OnClick="imgBtnSearchItem_Click" Visible="false"
                                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" Width="18px" ValidationGroup="searchItem" />
                                        &nbsp;&nbsp;                                        
                                        <asp:RequiredFieldValidator ID="rfvCodItem" runat="server" 
                                        ControlToValidate="txtTaskByItemDescription" ValidationGroup="AddNew" Text=" * " ErrorMessage="Código item es requerido" /> --%>
                                    </div>
                                </div>  
                                <div id="divItemDescription" runat="server" class="divControls">
                                    <div class="fieldRight"></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtTaskByItemDescription" runat="server" MaxLength="10" Width="250" Enabled="false"/>                                                                      
                                    </div>
                                </div>   
                                 <div id="divItemQtyTask" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblItemQtyTask" runat="server" Text="Cant. Reservada " /></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtItemQtyTask" runat="server" MaxLength="10" Width="100" />                     
                                    </div>
                                </div>  
                                <div id="divItemQtyDisp" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblItemQtyDisp" runat="server" Text="Cant. Disponible" /></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtItemQtyDisp" runat="server" MaxLength="10" Width="100" />   
                                    </div>
                                </div>
                                  
                                <div id="divQtyItem" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblQtyItem" runat="server" Text="Cantidad" /></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtQtyItem" runat="server" MaxLength="10" Width="100" />  
                                        <asp:RequiredFieldValidator ID="rfvQtyItem" runat="server" ControlToValidate="txtQtyItem" ValidationGroup="AddNew" Text=" * " ErrorMessage="Cantidad es requerido" />                                                                      
                                        <asp:RangeValidator ID="rvQtyItem" runat="server" ControlToValidate="txtQtyItem"
                                        ErrorMessage="Cantidad no contiene un número válido" MaximumValue="99999999" MinimumValue="1"
                                        ValidationGroup="AddNew" Type="Double">*</asp:RangeValidator>
                                        <asp:CompareValidator ID="cvQtyItem" runat="server" ControlToValidate="txtQtyItem" ValidationGroup="AddNew" Text="*"
                                         ControlToCompare="txtItemQtyDisp" Type="Double"  Operator="LessThanEqual" ErrorMessage="Cantidad no puede ser mayor a cant. disponible"></asp:CompareValidator>
                                    </div>
                                </div>
                                                                
                                <br />
                                
                                <div id="divDestLocItem" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblDestLocItem" runat="server" Text="Ubic. Destino" /></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtDestLocItem" runat="server" MaxLength="20" Width="150" /> 
                                        <asp:ImageButton ID="ImageButton2" runat="server" Height="18px"  OnClick="imgBtnSearchLocation_Click"  CommandName="txtDestLocItem"
                                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" Width="18px" ValidationGroup="searchItem" />                                                                      
                                        <asp:RequiredFieldValidator ID="rfvDestLocItem" runat="server" ControlToValidate="txtDestLocItem" ValidationGroup="AddNew" Text=" * " ErrorMessage="Ubicación destino es requerido" />
                                        <asp:CompareValidator ID="cvLocItem" runat="server" ControlToCompare="txtSourceLocItem" ValidationGroup="AddNew" 
                                            ControlToValidate="txtDestLocItem"  Text="*" ErrorMessage="Las ubicacones no deben ser iguales" Operator="NotEqual">
                                        </asp:CompareValidator>
                                    </div>
                                </div>
                                
                                <div id="divDestLpnItem" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblDestLpnItem" runat="server" Text="Lpn Destino" /></div>
                                    <div class="fieldLeft">
                                        <asp:DropDownList ID="ddlDestLpnItem" runat="server" Width="155px" AutoPostBack="true" OnSelectedIndexChanged="ddlSourceLpnItem_SelectedIndexChanged"></asp:DropDownList>
                                        <asp:TextBox ID="txtDestLpnItem" runat="server" MaxLength="20" Width="150" Visible="false" />                                                                       
                                    </div>
                                </div>
                                
                            </div>
                                                        
                            <div id="divTaskByLpn" runat="server" class="divControls" visible="false" >
                                <div id="divSourceLocLpn" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblSourceLocLpn" runat="server" Text="Ubic. Origen" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtSourceLocLpn" runat="server" MaxLength="20" Width="150" />     
                                        <asp:ImageButton ID="ImageButton3" runat="server" Height="18px" OnClick="imgBtnSearchLocation_Click"  CommandName="txtSourceLocLpn"
                                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" Width="18px" ValidationGroup="searchItem" />   
                                        <asp:RequiredFieldValidator ID="rfvSourceLocLpn" runat="server" ControlToValidate="txtSourceLocLpn" ValidationGroup="AddNew" Text=" * " ErrorMessage="Ubicación origen es requerido" />                                                                
                                    </div>
                                </div>
                                <div id="divSourceLpn" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblSourceLpn" runat="server" Text="Lpn Origen" /></div>
                                    <div class="fieldLeft">
                                        <asp:DropDownList ID="ddlSourceLpn" runat="server" Width="155px" ></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvSourceLpn" runat="server" ControlToValidate="ddlSourceLpn"
                                        InitialValue="-1" ValidationGroup="AddNew" Text=" * " ErrorMessage="Lpn origen es requerido" />
                                        
                                        <%--<asp:TextBox ID="txtSourceLpn" runat="server" MaxLength="20" Width="150" Visible="false" /> --%>  
                                        <%--<asp:RequiredFieldValidator ID="xx" runat="server" ControlToValidate="txtSourceLpn" ValidationGroup="AddNew" Text=" * " ErrorMessage="Lpn origen es requerido" />--%>                                                                                                                                    
                                    </div>
                                </div>
                                
                                <div id="divDestLocLpn" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblDestLocLpn" runat="server" Text="Ubic. Destino" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtDestLocLpn" runat="server" MaxLength="20" Width="150" />      
                                        <asp:ImageButton ID="ImageButton4" runat="server" Height="18px" OnClick="imgBtnSearchLocation_Click"  CommandName="txtDestLocLpn"
                                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" Width="18px" ValidationGroup="searchItem" />                                                                  
                                        <asp:RequiredFieldValidator ID="rfvDestLocLpn" runat="server" ControlToValidate="txtDestLocLpn" 
                                        ValidationGroup="AddNew" Text=" * " ErrorMessage="Ubicación destino es requerido" />
                                         <asp:CompareValidator ID="cvLocLpn" runat="server" ControlToCompare="txtSourceLocLpn" ValidationGroup="AddNew" 
                                            ControlToValidate="txtDestLocLpn"  Text="*" ErrorMessage="Las ubicacones no deben ser iguales" Operator="NotEqual">
                                        </asp:CompareValidator>
                                    </div>
                                </div>                                
                            </div>                            
                        </div>                     
                        <br /> 
                    
                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="AddNew" 
                                ShowMessageBox="false" CssClass="modalValidation"/>
                            <asp:ValidationSummary ID="valSearchItem" runat="server" ValidationGroup="searchItem" />
                        </div>                    
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="AddNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" OnClick="btnCloseNewEdit_Click" />
                        </div>                        
                    </div>
                </asp:Panel>
            </div>
            <%-- Pop up Editar/Nuevo Tarea --%>
 
            <%-- Lookup Locations --%>
            <div id="divLookupLocation" runat="server" visible="false">
                <asp:Button ID="btnDummy3" runat="server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpLookupLocation" runat="server" TargetControlID="btnDummy3"
                    PopupControlID="pnlLookupLocation" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupLocation" Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlLookupLocation" runat="server" CssClass="modalBox">
                    <asp:Panel ID="pnlHeadBar3" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblAddLocation" runat="server" Text="Buscar Ubicación" />
                            <asp:ImageButton ID="ImageButton6" runat="server" ToolTip="Cerrar" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                        
                    </asp:Panel>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidtxtSourceLocItem" runat="server" Value="-1" />                        
                        <asp:HiddenField ID="hidtxtSourceLocLpn" runat="server" Value="-1" />                    
                        <asp:HiddenField ID="hidtxtDestLocLpn" runat="server" Value="-1" /> 
                        <asp:HiddenField ID="hidtxtDestLocItem" runat="server" Value="-1" /> 
                        <webUc:ucLookUpFilter ID="ucFilterLocation" runat="server" /> 
                        <div class="divCtrsFloatLeft">
                            <%--<div class="divLookupGrid">--%>
                                <asp:GridView ID="grdSearchLocations" runat="server" DataKeyNames="idCode" OnRowCommand="grdSearchLocations_RowCommand" AllowPaging="true"
                                    AutoGenerateColumns="False"
                                    OnRowDataBound="grdSearchLocations_RowDataBound"
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                    EnableTheming="false">
                                    <Columns>
                                        <asp:BoundField AccessibleHeaderText="IdCode" DataField="IdCode" HeaderText="Id" InsertVisible="False" SortExpression="IdCode" />
                                        <asp:TemplateField AccessibleHeaderText="Code" HeaderText="Codigo">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCode0" runat="server" Text='<%# Eval ("Code") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tipo" AccessibleHeaderText="LocTypeName" >
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblLocType0" runat="server" Text='<%# Eval ("Type.LocTypeName") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Descripcion" AccessibleHeaderText="Description" >
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblLocDesc0" runat="server" Text='<%# Eval ("Description") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="imgBtnAddLocation" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                        Width="20px" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                           <%-- </div>--%>
                        </div>
                        <div style="clear:both" />
                    </div>
                    <div id="divActionsLocation" runat="server" class="modalActions">
                        <div id="divPageGrdSearchLocations" runat="server">
                            <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                <asp:ImageButton ID="btnFirstGrdSearchLocations" runat="server" OnClick="btnFirstGrdSearchLocations_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                <asp:ImageButton ID="btnPrevGrdSearchLocations" runat="server" OnClick="btnPrevGrdSearchLocations_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                                Pág. 
                                <asp:DropDownList ID="ddlPagesSearchLocations" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchLocationsSelectedIndexChanged" SkinID="ddlFilter" /> 
                                de 
                                <asp:Label ID="lblPageCountSearchLocations" runat="server" Text="" />
                                <asp:ImageButton ID="btnNextGrdSearchLocations" runat="server" OnClick="btnNextGrdSearchLocations_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                <asp:ImageButton ID="btnLastGrdSearchLocations" runat="server" OnClick="btnLastGrdSearchLocations_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Lookup Locations --%>
            
        </ContentTemplate>
        <Triggers>
             <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh"    EventName="Click" />
             <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch"  EventName="Click" />
             <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew"        EventName="Click" />
             <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst"    EventName="Click" />
             <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
             <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext"     EventName="Click" />
             <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast"     EventName="Click" />
             <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages"    EventName="SelectedIndexChanged" /> 
             <asp:AsyncPostBackTrigger ControlID="rbTaskTypeItem" EventName="CheckedChanged" />
             <asp:AsyncPostBackTrigger ControlID="rbTaskTypeLpn"  EventName="CheckedChanged" />                       
        </Triggers>
    </asp:UpdatePanel>
    <%-- FIN Panel Grilla Principal --%>
             
     <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop"
        CssClass="updateProgress" TargetControlID="uprGrid" />
    <%-- FIN Modal Update Progress --%>
                  
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterDate" runat="server" Text="Creación" Visible="false" />  
    <asp:Label ID="lblFilterCodeLoc" runat="server" Text="Id" Visible="false" />
    <asp:Label ID="lblFilterNameLoc" runat="server" Text="Código" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
    
    </div>
</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
