<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="PermissionMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Users.PermissionMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language='Javascript'>   
    function resizeDivPrincipal() {
        //debugger;
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divMain").style.height = h;
        document.getElementById("ctl00_MainContent_divMain").style.width = w;
    }

    $(document).ready(function () {
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
        initializeGridWithNoDragAndDrop();
    }
</script> 

    <div id="divMain" runat="server" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
 
       <spl:Splitter  LiveResize="false" CookieDays="0" ID="hsVertical" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" CSSLeftPanel="splitterLeftPanel">
         <LeftPanel ID="leftPanel" WidthDefault="500" WidthMin="100" >
                <Content>
                    <%-- Configuración de Roles --%>
                    <asp:UpdatePanel ID="upRole" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                                                       
                            <%-- Lista de Roles --%>
                            <div class="pageHeader">
                                <asp:ImageButton ID="btnSave1" runat="server" onclick="btnSave1_Click" 
                                    ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_save.png" CausesValidation="true" ValidationGroup="EditNew"
                                    onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_save_on.png';"
                                    onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_save.png';"
                                    ToolTip="Salvar cambios" />&nbsp;&nbsp;
                                    
                                <asp:Label ID="lblRoleModulo" Text="Modulo" runat="server"/>&nbsp;
                                <asp:DropDownList ID="ddlRoleModule" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRoleModule_SelectedIndexChanged" />&nbsp;&nbsp;
                                <asp:Label ID="lblRoles" Text="Rol" runat="server"/>&nbsp;
                                <asp:DropDownList ID="ddlRole" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged" />
                            </div>
                            
                            <%-- Opciones de Menu --%>
                            <div id="divPermissions" runat="server" visible="true"  class="divFloatLeft">
                                <div runat="server" style="width:340px" class="divFloatLeftHeader">
                                    <asp:Label ID="lblMenu" Text="Opciones de Menú" runat="server" />
                                </div>
                                <div style="width:350px" class="divFloatLeftBody">
                                    <asp:PlaceHolder ID="phControls" runat="server" />
                                </div>
                            </div>
                            <%-- FIN Opciones de Menu --%>                            
                           
                            <%-- Usuarios Asignados --%>
                            <div id="divUsers" runat="server" visible="true" class="divFloatLeft">
                                 <asp:GridView ID="grdUsers" runat="server"  DataKeyNames="Id" SkinID="grdDetail"
                                     ShowFooter="false" 
                                    OnRowDeleting="grdUsers_RowDeleting"  
                                    OnRowCreated="grdUsers_RowCreated" 
                                    AutoGenerateColumns="False" 
                                    onpageindexchanging="grdUsers_PageIndexChanging" 
                                    PageSize="50" 
                                    AllowPaging="True"  
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop" 
                                    EnableTheming="false" >
                                <RowStyle Font-Size="12px" Font-Bold="false" />
                                <EmptyDataRowStyle Wrap="False" />
                                <Columns>
                                    <asp:BoundField DataField="Id" ShowHeader="false" Visible="false" />
                                    <asp:TemplateField HeaderText="Usuario" AccessibleHeaderText="Usuario" ControlStyle-Width="206px">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUsuario" runat="server" Text='<%# FormatName(Eval("FirstName").ToString(), Eval("LastName").ToString(), Binaria.WMSTek.Framework.Utils.Constants.Nombre_Apellido)   %>'
                                                Width="120px" />
                                        </ItemTemplate>

                                    <ControlStyle Width="206px"></ControlStyle>
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <center>
                                                <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete" ToolTip="Eliminar"
                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                <EditRowStyle BackColor="#999999" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>                           
                           
                                    <br />
                                    <div style="margin-left: 10px; padding-bottom:20px">
                                        <asp:DropDownList ID="ddlUsers" runat="server" Width="150px"/>
                                        <asp:Button ID="btnAddUser" runat="server" Text="Asignar" OnClick="btnAddUser_Click" />
                                    </div>
                                </div>
                            </div>
                            <%-- FIN Usuarios Asignados --%>
                            
                        </ContentTemplate>  
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$rightPanel$ctl01$btnSave2" EventName="Click" />
                                                       
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="uprRole" runat="server" AssociatedUpdatePanelID="upRole"
                        DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprRole" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprRole" />
                    <%-- FIN Configuración de Roles --%>    
                </Content>
                <Footer Height="67">
                  <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>  
                       
             </LeftPanel>
             <RightPanel ID="rightPanel" WidthMin="100">
                <Content> 
                              
                    <div id="div2" runat="server" visible="true"  >
                        <div id="Div3" runat="server" style="width:250px" class="divFloatLeftHeader">
                            <asp:Label ID="Label1" Text="Seleccione Roles por Usuario" runat="server" />
                        </div>                        
                    </div>
                              
                    <%-- Asignación de Usuarios a Roles --%>
                    <asp:UpdatePanel ID="upUser" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>                    
                    <%-- Lista de Usuarios --%>
                    <div class="pageHeader">
                        <asp:ImageButton ID="btnSave2" runat="server" onclick="btnSave2_Click" 
                            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_save.png" CausesValidation="true" ValidationGroup="EditNew"
                            onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_save_on.png';"
                            onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_save.png';"
                            ToolTip="Salvar cambios" />&nbsp;&nbsp;
                        <asp:Label ID="Label2" Text="Usuario" runat="server"/>&nbsp;
                        <asp:DropDownList ID="ddlUsers2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUsers2_SelectedIndexChanged" />
                    </div>
                    
                    
                    <%-- Roles Asignados --%>
                    <div id="divRoles" runat="server" visible="false" class="divFloatLeft divGridMarginLeft" >
                            <asp:GridView ID="grdRoles" 
                                runat="server" DataKeyNames="Id" 
                                    ShowFooter="false" 
                                    OnRowDeleting="grdRoles_RowDeleting"
                                    AllowPaging="false" 
                                    PageSize="50"  
                                    onpageindexchanging="grdRoles_PageIndexChanging"
                                    AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop" 
                                    EnableTheming="false" >
                                   <RowStyle Font-Size="12px" Font-Bold="false" />
                                   <EmptyDataRowStyle Wrap="False" />
                                    <Columns>
                                        <asp:BoundField DataField="Id"  Visible="false" />
                                        <asp:TemplateField HeaderText="Usuario" AccessibleHeaderText="Usuario" ControlStyle-Width="206px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblUsuario" runat="server" Text='<%# Eval("Name") %>'
                                                    Width="120px" />
                                            </ItemTemplate>
                                        </asp:TemplateField>                                    
                                        <asp:TemplateField> 
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete" ToolTip="Eliminar"
                                                        ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <EditRowStyle BackColor="#999999" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>                            
                                
                                
                                <br />
                                <div style="margin-left: 10px">
                                    <asp:DropDownList ID="ddlRole2" runat="server" Width="150px"/>
                                    <asp:Button ID="btnAddRole" runat="server" Text="Asignar" OnClick="btnAddRole_Click" />
                                </div>
                        </div>
                    </div>
                    <%-- FIN Roles Asignados --%>    
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>    
                    <asp:UpdateProgress ID="uprUser" runat="server" AssociatedUpdatePanelID="upUser"
                        DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprUser" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprUser" />                                                          
                    <%-- FIN Asignación de Usuarios a Roles --%>
                                   
                 </Content>
                <Footer Height="67">
                  <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                 </Footer>     
              </RightPanel>
             </spl:Splitter>
    </div>    
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea quitar este Usuario?"
        Visible="false" />
    <asp:Label ID="lblSelectUser" runat="server" Text="(Seleccione Usuario)" Visible="false" />
    <asp:Label ID="lblSelectRole" runat="server" Text="(Seleccione Rol)" Visible="false" />
    <asp:Label ID="lblAlreadyAssignedUser" runat="server" Text="Usuario ya se encuentra asignado a un rol" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Permisos" Visible="false" />
    <asp:Label ID="lblToolTipEliminar" runat="server" Text="Eliminar" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
    
         
    
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
    <%-- FIN Barra de Estado --%>
</asp:Content>