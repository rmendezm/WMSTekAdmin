<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="TerminalMonitor.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.TerminalMonitor" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUcStatus" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(setHeightGrid);

    window.onresize = setHeightGrid;

    $(document).ready(function () {
        initializeGridDragAndDrop("Terminal_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("Terminal_FindAll", "ctl00_MainContent_grdMgr");
        setHeightGrid();
    }

    function setHeightGrid() {
        var extraHeight = 20;
        var maxHeight = $("#ctl00_divTop").height() - $(".row-height-filter").height() - extraHeight;
        $(".froze-header-grid").css("height", maxHeight + "px");
    }

    function setDivsAfter() {

        var extraHeight = 20;
        var heightDivContainerTable = $("div.froze-header-grid").height();

        var totalHeight = heightDivContainerTable;
        totalHeight = totalHeight - extraHeight;

        $("div.froze-header-grid").css("height", totalHeight + "px");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                   <ContentTemplate>  
                        <%-- Grilla Principal --%> 
                        <div id="divGrid" runat="server" class="divGrid" onresize="SetDivs();" > 
                 
                          <asp:GridView ID="grdMgr" runat="server"
                             DataKeyNames="TerminalId"
                             AllowPaging="True"   
                             AllowSorting ="False"
                             EnableViewState="False"
                             onrowcreated="grdMgr_RowCreated"
                             onrowcommand="grdMgr_RowCommand" 
                             onrowdatabound="grdMgr_RowDataBound" 
                             ondatabound="grdMgr_DataBound"
                             AutoGenerateColumns="False"
                             CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                             EnableTheming="false">
                            <Columns>
                                <asp:TemplateField AccessibleHeaderText="TerminalId" HeaderText="ID">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTerminalId" runat="server" Text='<%# Bind("TerminalId") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="TerminalCode" HeaderText="C&oacute;digo"  AccessibleHeaderText="TerminalCode"/>
                                <asp:BoundField DataField="TerminalName" HeaderText="Nombre"  AccessibleHeaderText="TerminalName"/>
                                <asp:BoundField DataField="TerminalType" HeaderText="Tipo"  AccessibleHeaderText="TerminalType"/>
                                <asp:templatefield headertext="Activo" AccessibleHeaderText="TerminalCodStatus">
                                    <itemtemplate>
                                        <center>
                                            <asp:CheckBox ID="chkCodStatus" runat="server" checked='<%# Eval ( "TerminalCodStatus" ) %>' Enabled="false"/>
                                        </center>                        
                                    </itemtemplate>
                                </asp:templatefield>
                    
                                <asp:TemplateField AccessibleHeaderText="TerminalStatus" HeaderText="Estado Conexi&oacute;n">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTerminalStatus" runat="server" Text='<%# Bind("TerminalStatus") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                    
                                <asp:BoundField DataField="ContextProcessName" HeaderText="Nombre Proceso"  AccessibleHeaderText="ContextProcessName"/>
                                <asp:BoundField DataField="ContextStepName" HeaderText="Etapa"  AccessibleHeaderText="ContextStepName"/>
                                <asp:BoundField DataField="ContextCurrentTaskCode" HeaderText="Cód. Tarea Actual"  AccessibleHeaderText="ContextCurrentTaskCode"/>
                                        
                                <asp:BoundField DataField="ContextStartDate" HeaderText="Fecha Inicio"  AccessibleHeaderText="ContextStartDate"/>
                                <asp:BoundField DataField="SessionDateStart" HeaderText="Inicio Sesi&oacute;n"  AccessibleHeaderText="SessionDateStart"/>
                                <asp:BoundField DataField="SessionDateEnd" HeaderText="Fin Sesi&oacute;n"  AccessibleHeaderText="SessionDateEnd"/>
                    
                                <asp:BoundField DataField="RfOperatorUserName" HeaderText="Nombre Operador"  AccessibleHeaderText="RfOperatorUserName"/>
                                <asp:BoundField DataField="WarehouseName" HeaderText="Nombre Almac&eacute;n"  AccessibleHeaderText="WarehouseName"/>
                                <asp:BoundField DataField="MaqUser" HeaderText="Maq. Trabajo"  AccessibleHeaderText="MaqUser"/>
                                <asp:BoundField DataField="LPN" HeaderText="LPN Utilizando"  AccessibleHeaderText="LPN"/>

                                <asp:BoundField DataField="TaskType" HeaderText="Tipo Tarea" Visible="False"  AccessibleHeaderText="TaskType"/>
                                <asp:BoundField DataField="IdTask" HeaderText="Id Tarea"  AccessibleHeaderText="IdTask"/>
                                <asp:BoundField DataField="IdTaskDetail" HeaderText="Id Tarea Detalle"  AccessibleHeaderText="IdTaskDetail"/>
                                                   
                                <asp:templatefield AccessibleHeaderText="StatusMonitor" HeaderText="Estado Desconexi&oacute;n" Visible="false">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkStatusMonitor" runat="server" checked='<%# Eval ( "StatusMonitor" ) %>' Enabled="false"/>
                                    </ItemTemplate>
                                </asp:templatefield>                    
                                
                                <asp:templatefield AccessibleHeaderText="ActivateLogTerminalRF" HeaderText="Log Activo" Visible="false">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkActivateLog" runat="server" checked='<%# Eval ( "ActivateLogTerminalRF" ) %>' Enabled="false"/>
                                    </ItemTemplate>
                                </asp:templatefield>
                    
                                <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions" Visible="True">
                                    <ItemTemplate> 
                                        <div style="width:60px">
                                            <center>
                                                <asp:ImageButton ID="btnDisconnect" runat="server"  ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_connect_dis.png" CausesValidation="false" CommandName="Disconnect" CommandArgument="<%# Container.DataItemIndex %>"/>
                                                <asp:ImageButton ID="btnTerminalLog" runat="server"  ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_connect_dis.png" CausesValidation="false" CommandName="TerminalLog" CommandArgument="<%# Container.DataItemIndex %>"/>
                                            </center>
                                        </div>	                        
                                    </ItemTemplate>
                                </asp:TemplateField>
                    
                             </Columns>
                        </asp:GridView>
                        </div>
                        <%-- FIN Grilla Principal --%>  
            
                    <asp:Timer ID="AjxTimerUpGrid" runat="server" Enabled="true" Interval="40000"> 
                    </asp:Timer>  
        
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
    <asp:Label ID="lblConfirmDisconnect" runat="server" Text="¿Desconectar terminal: " Visible="false" />
    <asp:Label ID="lblDisconnect" runat="server" Text="Desconectar Terminal" Visible="false" />    
    <asp:Label ID="lblDisconnected" runat="server" Text="Terminal Desconectado" Visible="false" />    
    <asp:Label ID="lblDisconnect2" runat="server" Text="Desconexión ya solicitada" Visible="false" />  
    <asp:Label ID="lblDisconnectedNew" runat="server" Text="Terminal X Desconectado" Visible="false" />

    <asp:Label ID="lblConfirmActiveLog" runat="server" Text="¿Activar Log terminal: " Visible="false" />
    <asp:Label ID="lblConfirmDisabledLog" runat="server" Text="¿Desactivar Log terminal: " Visible="false" />
    <asp:Label ID="lblActiveLog" runat="server" Text="Activar Log Terminal" Visible="false" />     
    <asp:Label ID="lblDisabledLog" runat="server" Text="Desactivar Log Terminal" Visible="false" />  
    <asp:Label ID="lblLogTerminal" runat="server" Text="Log Terminal Desactivado" Visible="false" />  
    <asp:Label ID="lblInfoLogActive" runat="server" Text="Log Terminal X Activado" Visible="false" />
    <asp:Label ID="lblInfoLogDisabled" runat="server" Text="Log Terminal X Desactivado" Visible="false" />
    <asp:Label ID="lblMaqUserFilterMsg" runat="server" Text="Máquina" Visible="false" />
    <asp:Label ID="lblRfOperatorUserNameFilterMsg" runat="server" Text="Usuario" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>   
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">

    <%-- Barra de Estado --%>        
    <webUcStatus:ucStatus id="ucStatus" runat="server"/>      
    <%-- FIN Barra de Estado --%>            

</asp:Content>