<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="WebParametersMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Configuration.WebParametersMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("CfgParameter_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("CfgParameter_FindAll", "ctl00_MainContent_grdMgr");
        }
    </script>

    <div id="div1ValidationSummary" class="divValidationSummary" runat="server">
        <asp:ValidationSummary ID="valSummary" runat="server" ValidationGroup="EditNew" />
    </div>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            
                            <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" OnRowCreated="grdMgr_RowCreated"
                                AllowPaging="True" EnableViewState="False" OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Id" />
                                    <asp:TemplateField HeaderText="Parámetro" AccessibleHeaderText="Code">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCode" runat="server" Text='<%# Eval ( "ParameterCode" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                        
                                    <asp:TemplateField HeaderText="Valor Actual">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtParameterValue" Enabled="true" Width="50px" runat="server" Text='<%# Bind ( "ParameterValue" ) %>' ></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Tipo" AccessibleHeaderText="Type">
                                        <ItemTemplate>
                                            <asp:Label ID="lblType" runat="server" Text='<%# Eval ( "Type" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Valor Min." AccessibleHeaderText="MinValue">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMinValue" runat="server" Text='<%# Eval ( "MinValue" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Valor Max." AccessibleHeaderText="MaxValue">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMaxValue" runat="server" Text='<%# Eval ( "MaxValue" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Valor por defecto" AccessibleHeaderText="DefaultValue">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDefaultValue" runat="server" Text='<%# Eval ( "DefaultValue" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ámbito" AccessibleHeaderText="Scope">
                                        <ItemTemplate>
                                            <asp:Label ID="lblScope" runat="server" Text='<%# Eval ( "Scope" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Módulo" AccessibleHeaderText="Module">
                                        <ItemTemplate>
                                            <asp:Label ID="lblModuleName" runat="server" Text='<%# Eval ( "Module.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Editable" AccessibleHeaderText="AllowEdit">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkAllowEdit" runat="server" Checked='<%# Eval ( "AllowEdit" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                
                            <div id="divWarning" class="divWarning" runat="server" visible="false">
                                <asp:Label ID="lblError" Visible="false" runat="server" Text="Error en el ingreso de datos para el Parametro: " />ç
                                <asp:Label ID="lblErrorTitle" Visible="false" runat="server" Text="Dato no válido" />
                    
                            </div>             
                        </div>
                        <%-- FIN Grilla Principal --%>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
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
    <%-- Barra de Estado --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop"
        CssClass="updateProgress" TargetControlID="uprGrid" />
    <%--ParameterValue  --%>
    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar Parametro --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlVendor" BackgroundCssClass="modalBackground" PopupDragHandleControlID="VendorCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
            </div>
            <%-- Pop up Editar/Nuevo Parametro --%>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
        </Triggers>
    </asp:UpdatePanel>
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop"
        CssClass="updateProgress" TargetControlID="uprEditNew" />
    <%-- FIN Modal Update Progress --%>
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>        
    <asp:Label ID="lblFilterName" runat="server" Text="Parámetro" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Módulo Web" Visible="false"/>
    <asp:Label ID="lblReset" runat="server" Text="Debe reiniciar la aplicación para activar los cambios realizados. <br><br> ¿Desea reiniciar ahora?" Visible="false"/> 
    <asp:Label ID="lblPoliciyPasswordTitle" runat="server" Text="Error Política Contraseñas Parámetro DefaultPassword" Visible="false"/>
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>        
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
