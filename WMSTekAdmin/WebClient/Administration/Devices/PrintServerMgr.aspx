<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="PrintServerMgr.aspx.cs" 
Inherits="Binaria.WMSTek.WebClient.Administration.Devices.PrintServerMgr" %>


<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;

    $(document).ready(function () {
        initializeGridDragAndDrop("PrintServer_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("PrintServer_FindAll", "ctl00_MainContent_grdMgr");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <%-- Grilla Principal --%>
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" 
                                    DataKeyNames="Id" 
                                    runat="server" 
                                    OnRowCreated="grdMgr_RowCreated"
                                    OnRowDeleting="grdMgr_RowDeleting"  
                                    OnRowEditing="grdMgr_RowEditing" 
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    OnPageIndexChanging="grdMgr_PageIndexChanging"
                                    AllowPaging="True" 
                                    EnableViewState="false"
                                    AutoGenerateColumns="False"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True" AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="ServerName" HeaderText="Nombre" AccessibleHeaderText="ServerName" />
                                    <asp:BoundField DataField="ServiceName" HeaderText="Nombre Servicio" AccessibleHeaderText="ServiceName" />
                                    <asp:BoundField DataField="IpAddress" HeaderText="Dirección Ip" AccessibleHeaderText="IpAddress" />            
                        
                                    <asp:TemplateField HeaderText="Puerto Ip" AccessibleHeaderText="IpPort">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWhsCode" runat="server" Text='<%# ((int)Eval("IpPort") == -1) ? " " : Eval("IpPort") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>    
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <center>                                
                                                <asp:CheckBox ID="chkStatus1" runat="server" Checked='<%# ((int)Eval("Status")== 1) ? true : false %>' Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="TimeoutPrint" HeaderText="Tiempo sin Impr." AccessibleHeaderText="TimeoutPrint" /> 
                                    <asp:BoundField DataField="IntervalPrint" HeaderText="Intervalo Impr." AccessibleHeaderText="IntervalPrint" /> 
                                    <asp:BoundField DataField="TimeoutSql" HeaderText="TimeOut Sql" AccessibleHeaderText="TimeoutSql" />  
                                    <asp:BoundField DataField="IntervalSql" HeaderText="Intervalo SQL" AccessibleHeaderText="IntervalSql" /> 
                                    <asp:BoundField DataField="QtyTaskPerQuery" HeaderText="Cant. Tareas " AccessibleHeaderText="QtyTaskPerQuery" /> 
                                    <asp:BoundField DataField="DateCreated" HeaderText="Fecha Creación" AccessibleHeaderText="DateCreated" />             
                                    <asp:BoundField DataField="UserCreated" HeaderText="Usuario Creación" AccessibleHeaderText="UserCreated" />          
                                    <asp:BoundField DataField="DateModified" HeaderText="Fecha Modificación" AccessibleHeaderText="DateModified" />            
                                    <asp:BoundField DataField="UserModified" HeaderText="Usuario Modificación" AccessibleHeaderText="UserModified" />
                        
                                   <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 60px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
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
                     <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
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
    <%-- FIN Grilla Principal --%>
          
    <%-- Pop up Editar/Nuevo PrintServer --%>                  
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>             
            <div id="divEditNew" runat="server" visible="false">        
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlVendor" BackgroundCssClass="modalBackground" PopupDragHandleControlID="VendorCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlVendor" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="VendorCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Servidor" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Servidor" />
                            <asp:ImageButton ID="btnClose" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">                    
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
                                       
                            <%-- ServerName --%>                                                        
                            <div id="divServerName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblServerName" runat="server" Text="Nombre" />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtServerName" runat="server" Width="120px" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvServerName" runat="server" ControlToValidate="txtServerName"
                                     ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revServerName" runat="server" ControlToValidate="txtServerName"
	                                    ErrorMessage="En el campo nombre debe ingresar solo letras de la A - Z o a - z ó números" 
	                                    ValidationExpression="[a-zA-Z 0-9ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                              
                                </div>
                            </div>
                            <%-- ServiceName --%>
                            <div id="divServiceName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblServiceName" runat="server" Text="Nombre Servicio" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtServiceName" runat="server" Width="120px" MaxLength="100" />
                                    <asp:RequiredFieldValidator ID="rfvServiceName" runat="server" ControlToValidate="txtServiceName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre servicio es requerido" />
                                    <asp:RegularExpressionValidator ID="revServiceName" runat="server"  ControlToValidate="txtServiceName"            
                                            ErrorMessage="En el campo nombre  servicio debe ingresar solo letras de la A - Z o a - z ó números" 
	                                    ValidationExpression="[a-zA-Z 0-9ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                             
                                </div>
                            </div>
                            
                            <%-- IpAddress --%>                                                        
                            <div id="divIpAddress" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIpAddress" runat="server" Text="Dirección Ip" />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtIpAddress" runat="server" Width="120px" MaxLength="15"></asp:TextBox>                                                           
                                    <asp:RegularExpressionValidator ID="revIpAddress" runat="server" ControlToValidate="txtIpAddress"
	                                    ErrorMessage="Debe ingresar una dirección ip válida" 
	                                    ValidationExpression="^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                              
                                </div>
                            </div>
                            <%-- IpPort --%>                                                        
                            <div id="divIpPort" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIpPort" runat="server" Text="Puerto Ip" />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtIpPort" runat="server" Width="120px" MaxLength="6"></asp:TextBox>                                                           
                                    <asp:RegularExpressionValidator ID="rfvIpPort" runat="server" ControlToValidate="txtIpPort"
	                                    ErrorMessage="Debe ingresar un puerto ip válido" 
	                                    ValidationExpression="[0-9]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                              
                                </div>
                            </div>
                            <%-- Status --%>                                                        
                            <div id="divStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStatus" runat="server" Text="Estado" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkStatus" runat="server" />                                                                                               
                                </div>
                            </div>
                            <%-- TimeoutPrint --%>                                                        
                            <div id="divTimeoutPrint" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTimeoutPrint" runat="server" Text="Tiempo sin Impr." />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtTimeoutPrint" runat="server" Width="120px" MaxLength="6"></asp:TextBox>  
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtTimeoutPrint"
                                     ValidationGroup="EditNew" Text=" * " ErrorMessage="Tiempo sin impresión es requerido"></asp:RequiredFieldValidator>                                                         
                                    <asp:RegularExpressionValidator ID="revTimeoutPrint" runat="server" ControlToValidate="txtTimeoutPrint"
	                                    ErrorMessage="Debe ingresar un tiempo sin impresión válido" 
	                                    ValidationExpression="[0-9]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                              
                                </div>
                            </div>
                            <%-- IntervalPrint --%>                                                        
                            <div id="divIntervalPrint" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIntervalPrint" runat="server" Text="Intervalo Impr." />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtIntervalPrint" runat="server" Width="120px" MaxLength="6"></asp:TextBox>         
                                    <asp:RequiredFieldValidator ID="rfvIntervalPrint" runat="server" ControlToValidate="txtIntervalPrint"
                                     ValidationGroup="EditNew" Text=" * " ErrorMessage="Intervalo impresión es requerido"></asp:RequiredFieldValidator>                                                     
                                    <asp:RegularExpressionValidator ID="revIntervalPrint" runat="server" ControlToValidate="txtIntervalPrint"
	                                    ErrorMessage="Debe ingresar un intervalo impresión válido" 
	                                    ValidationExpression="[0-9]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                              
                                </div>
                            </div>
                            <%-- TimeoutSql --%>                                                        
                            <div id="divTimeoutSql" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTimeoutSql" runat="server" Text="Timeout Sql" />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtTimeoutSql" runat="server" Width="120px" MaxLength="100"></asp:TextBox>      
                                    <asp:RequiredFieldValidator ID="rfvTimeoutSql" runat="server" ControlToValidate="txtTimeoutSql"
                                     ValidationGroup="EditNew" Text=" * " ErrorMessage="TimeOut sql es requerido"></asp:RequiredFieldValidator>                                                     
                                    <asp:RegularExpressionValidator ID="revTimeoutSql" runat="server" ControlToValidate="txtTimeoutSql"
	                                    ErrorMessage="Debe ingresar un timeout sql válido" 
	                                    ValidationExpression="[0-9]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                              
                                </div>
                            </div>
                            <%-- IntervalSql --%>                                                        
                            <div id="divIntervalSql" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIntervalSql" runat="server" Text="Intervalo Sql" />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtIntervalSql" runat="server" Width="120px" MaxLength="100"></asp:TextBox>   
                                    <asp:RequiredFieldValidator ID="rfvIntervalSql" runat="server" ControlToValidate="txtIntervalSql"
                                     ValidationGroup="EditNew" Text=" * " ErrorMessage="Intervalo sql es requerido"></asp:RequiredFieldValidator>                                                        
                                    <asp:RegularExpressionValidator ID="revIntervalSql" runat="server" ControlToValidate="txtIntervalSql"
	                                    ErrorMessage="Debe ingresar un intervalo sql válido" 
	                                    ValidationExpression="[0-9]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                              
                                </div>
                            </div>
                            <%-- QtyTaskPerQuery --%>                                                        
                            <div id="divQtyTaskPerQuery" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblQtyTaskPerQuery" runat="server" Text="Cant. Tareas" />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtQtyTaskPerQuery" runat="server" Width="120px" MaxLength="100"></asp:TextBox>  
                                    <asp:RequiredFieldValidator ID="rfvQtyTaskPerQuery" runat="server" ControlToValidate="txtQtyTaskPerQuery"
                                     ValidationGroup="EditNew" Text=" * " ErrorMessage="Cantid de tareas es requerido"></asp:RequiredFieldValidator>                                                         
                                    <asp:RegularExpressionValidator ID="revQtyTaskPerQuery" runat="server" ControlToValidate="txtQtyTaskPerQuery"
	                                    ErrorMessage="Debe ingresar una cantidad de tareas válida" 
	                                    ValidationExpression="[0-9]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                              
                                </div>
                            </div>
                            
                        </div>
                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"   
                                ShowMessageBox="false" CssClass="modalValidation"/>
                           <%-- <asp:ValidationSummary ID="vs" runat="server" ValidationGroup="EditNew" 
                                ShowMessageBox="false" CssClass="modalValidation"/>--%>
                        </div>                        
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>                        
                    </div>
                </asp:Panel>
            </div>
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
      </Triggers>
    </asp:UpdatePanel>  

    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />    
    <%-- FIN Pop up Editar/Nuevo impresora --%>
    
   <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta Servidor?" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>

