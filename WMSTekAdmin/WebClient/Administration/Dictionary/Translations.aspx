<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="Translations.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Dictionary.Translations" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;

    $(document).ready(function () {
        initializeGridDragAndDrop("Dictionary_TranslateView", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("Dictionary_TranslateView", "ctl00_MainContent_grdMgr");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">   
                    <ContentTemplate>
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" runat="server" OnRowCreated="grdMgr_RowCreated" AllowPaging="True"
                                EnableViewState="False" OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Id Dic" AccessibleHeaderText="IdDiccionary">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdDiccionary" runat="server" Text='<%# Eval ( "Dictionary.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Lan" AccessibleHeaderText="IdLanguage">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLanguage" runat="server" Text='<%# Eval ( "Language.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Objeto" AccessibleHeaderText="ObjectKey">
                                        <ItemTemplate>
                                            <asp:Label ID="lblObject" runat="server" Text='<%# Eval ( "Dictionary.IdObjectKey" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Objeto Contenedor" AccessibleHeaderText="ObjectKeyContainer">
                                        <ItemTemplate>
                                            <asp:Label ID="lblObjectContainer" runat="server" Text='<%# Eval ( "Dictionary.IdObjectKeyContainer" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Control Contenedor" AccessibleHeaderText="ControlContainer">
                                        <ItemTemplate>
                                            <asp:Label ID="lblControlContainer" runat="server" Text='<%# Eval ( "Dictionary.IdControlContainer" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>                        
                                    <asp:TemplateField HeaderText="Tipo Objeto" AccessibleHeaderText="ObjectType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblObjectType" runat="server" Text='<%# Eval ( "Dictionary.ObjectType" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Propiedad" AccessibleHeaderText="TextProperty">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTextProperty" runat="server" Text='<%# Eval ( "Dictionary.TextProperty" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Texto Base" AccessibleHeaderText="TextValue">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTextBase" runat="server" Text='<%# Eval ( "Dictionary.TextValue" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Traducción" AccessibleHeaderText="TextValue">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtTranslate" Enabled="true" runat="server" Text='<%# Eval ( "TextValue" ) %>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Advertencia" AccessibleHeaderText="Warning">
                                        <ItemTemplate>
                                            <asp:Image ID="imgWarning" Width="18px" Height="18px" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_warning_small.png"
                                                Visible="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <div id="div1ValidationSummary" class="divValidationSummary" runat="server">
                                <asp:ValidationSummary ID="valSummary" runat="server" ValidationGroup="EditNew" HeaderText="" />
                            </div>
                                    <asp:ImageButton ID="btnOpenDictionary" runat="server"  Visible="false"
                                    ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" 
                                    PostBackUrl="~/Administration/Configuration/Dictionary.aspx" 
                                    style="text-align: right" Width="18px" />                
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
    <%-- Modal Update Progress --%>
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
    <asp:Label ID="lblEmptyRow" runat="server" Text="(Seleccione... )" Visible="false" />
    <asp:Label ID="lblTextBase" runat="server" Text="Texto Base" Visible="false" />
    <asp:Label ID="lblTranslate" runat="server" Text="Traducción" Visible="false" />
    <asp:Label ID="lblError1" runat="server" Text="No se ha modificado ningun objeto"
        Visible="false" />
<asp:Label ID="lblError2" runat="server" Text="El largo del texto podria ocasionar problemas de visualización."
        Visible="false" />        
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Seguro que desea eliminar?"
        Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
    

</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
    <%-- FIN Barra de Estado --%>
</asp:Content>
