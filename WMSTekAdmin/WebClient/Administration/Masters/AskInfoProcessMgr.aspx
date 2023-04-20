<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="AskInfoProcessMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.AskInfoProcessMgr" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="<%= Page.ResolveClientUrl("~/WebResources/Javascript/UtilMassive.js")%>"></script>

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("Country_FindAll", "ctl00_MainContent_grdMgr");
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
            initializeGridDragAndDrop("Country_FindAll", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();
        }
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                            <asp:GridView ID="grdMgr" 

                                runat="server" 
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing" 
                                OnPageIndexChanging="grdMgr_PageIndexChanging"
                                AllowPaging="True" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>

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

                                    <asp:TemplateField HeaderText="Código" AccessibleHeaderText="Code">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWmsProcessCode" runat="server" Text='<%# Eval ( "WmsProcess.Code" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Proceso" AccessibleHeaderText="Name">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWmsProcessName" runat="server" Text='<%# Eval ( "WmsProcess.Name" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="Object" HeaderText="Objeto" AccessibleHeaderText="Object" />
                                    <asp:BoundField DataField="Property" HeaderText="Propiedad" AccessibleHeaderText="Property" />
                                    <asp:BoundField DataField="Sequence" HeaderText="Secuencia" AccessibleHeaderText="Sequence" />
                                    <asp:BoundField DataField="ParameterAsk" HeaderText="Parámetro" AccessibleHeaderText="ParameterAsk" />                                                                       

                                </Columns>
                            </asp:GridView>
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

    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
             <%-- Pop up Editar/Nuevo Lpn --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlCountry" BackgroundCssClass="modalBackground" PopupDragHandleControlID="CountryCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlCountry" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="CountryCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Parametro" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Parametro" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />

                        <div class="divCtrsFloatLeft">

                            <%-- WmsProcess --%>
                            <div id="divWmsProcessType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWmsProcessType" runat="server" Text="Proceso" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlWmsProcessType" runat="server"></asp:DropDownList> 
                                     <asp:RequiredFieldValidator ID="rfvWmsProcessType" runat="server" ControlToValidate="ddlWmsProcessType"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Proceso es requerido" />  
                                </div>
                            </div>

                            <%-- Objeto --%>
                            <div id="divObject" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblObject" runat="server" Text="Objeto" /></div>
                                <div class="fieldLeft">
                                     <asp:DropDownList ID="ddlObject" runat="server"></asp:DropDownList> 
                                     <asp:RequiredFieldValidator ID="ReqrvfObject" runat="server" ControlToValidate="ddlObject"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Object es requerido" />
                                    <%--<asp:TextBox ID="txtObject" runat="server" MaxLength="60" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvObject" runat="server" ControlToValidate="txtObject"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Objeto es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtObject" runat="server" ControlToValidate="txtObject"
	                                     ErrorMessage="Objeto permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator> --%>    
                                </div>
                            </div>

                            <%-- Propiedad --%>
                            <div id="divProperty" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblProperty" runat="server" Text="Propiedad" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlProperty" runat="server"></asp:DropDownList> 
                                     <asp:RequiredFieldValidator ID="rfvProperty" runat="server" ControlToValidate="ddlProperty"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Propiedad es requerido" />
                                    <%--<asp:TextBox ID="txtProperty" runat="server" MaxLength="60" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvProperty" runat="server" ControlToValidate="txtProperty"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Propiedad es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtProperty" runat="server" ControlToValidate="txtProperty"
	                                     ErrorMessage="Propiedad permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator> --%>    
                                </div>
                            </div>                   

                            <%-- Secuencia --%>
                            <div id="divSequence" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblSequence" runat="server" Text="Secuencia" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtSequence" runat="server" MaxLength="2" Width="40" />
                                    <asp:RequiredFieldValidator ID="rfvSequence" runat="server" ControlToValidate="txtSequence"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Secuencia es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtSequence" runat="server" ControlToValidate="txtSequence"
	                                        ErrorMessage="Secuencia permite ingresar solo caracteres numéricos del 1 al 9" 
	                                        ValidationExpression="[1-9]"
	                                        ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>     
                                </div>
                            </div>

                            <%-- Parametro --%>
                            <div id="divParameterAsk" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblParameterAsk" runat="server" Text="Parámetro" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtParameterAsk" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvParameterAsk" runat="server" ControlToValidate="txtParameterAsk"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Parámetro es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtParameterAsk" runat="server" ControlToValidate="txtParameterAsk"
	                                     ErrorMessage="Parámetro permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>     
                                </div>
                            </div>

                        </div>

                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" 
                                ShowMessageBox="false" CssClass="modalValidation"/>
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

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este registro?" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Mantenedor de Parametros Por Proceso" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
