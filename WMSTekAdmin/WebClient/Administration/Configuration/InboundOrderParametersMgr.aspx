<%@ Page Language="C#" AutoEventWireup="true"  CodeBehind="InboundOrderParametersMgr.aspx.cs"
MasterPageFile="~/Shared/WMSTekContent.Master"  Inherits="Binaria.WMSTek.WebClient.Administration.Configuration.InboundOrderParametersMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <style>
        .container{
            max-width: 1000px;
        }
    </style>
<script type="text/javascript" language='Javascript'>   
    function resizeDivPrincipal() {
        //debugger;
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divMainPrincipal").style.height = h;
        document.getElementById("ctl00_MainContent_divMainPrincipal").style.width = w;
    }
    window.onresize = resizeDivPrincipal;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(resizeDivPrincipal);

    $(document).ready(function () {
        initializeGridDragAndDrop("CfgParameterWarehouseDoc_FindAll", "ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdMgr");

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
        initializeGridDragAndDrop("CfgParameterWarehouseDoc_FindAll", "ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdMgr");
    }
</script> 


    <div id="divMainPrincipal" runat="server" style="width: 100%; height: 100%; margin: 0px; margin-bottom: 80px">
        <spl:Splitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default">
            <LeftPanel ID="leftPanel" WidthDefault="200" WidthMin="100">
                <Content>
                    <asp:UpdatePanel ID="upTreeView" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                    
                            <%-- Árbol --%>
                            <asp:TreeView ID="trvInboundType" runat="server" OnSelectedNodeChanged="trvInboundType_SelectedNodeChanged"
                                ShowLines="True">
                                <ParentNodeStyle Font-Bold="False" />
                                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px"
                                    VerticalPadding="0px" />
                                <Nodes>
                                    <asp:TreeNode Expanded="True" Value="Company" SelectAction="SelectExpand"></asp:TreeNode>
                                </Nodes>
                                <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="0px"
                                    NodeSpacing="0px" VerticalPadding="0px" />
                            </asp:TreeView>
                            <%-- FIN Árbol--%>
                    
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$leftPanel$ctl01$trvInboundType"
                                EventName="SelectedNodeChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="uprTreeView" runat="server" AssociatedUpdatePanelID="upTreeView" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress0" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprTreeView" runat="server" ControlToOverlayID="trvInboundType" CssClass="updateProgress" TargetControlID="uprTreeView" />

                </Content>
                <Footer Height="67">
                    <div style="color: White">
                        No Borrar - Evita que BottomPanel se solape con StatusBar
                    </div>
                </Footer>
            </LeftPanel>
            <RightPanel>
                <Content>
                    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <%-- Grilla Principal --%>
                            <div id="divGrid" runat="server" visible="true" class="divRightPanel">
                                <div class="divGridRightPanel">
                                    <div id="lpnHeadTitle" runat="server" visible="true" class="divGridTitle">
                                        <asp:Label ID="lblTitle" runat="server"></asp:Label>
                                    </div>
                                    <div class="container">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" OnRowCreated="grdMgr_RowCreated"
                                                    AllowPaging="False" EnableViewState="False" OnRowDataBound="grdMgr_RowDataBound"
                                                    AutoGenerateColumns="false"
                                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                    EnableTheming="false">
                                                    <Columns>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                                            AccessibleHeaderText="IdParameter" />
                                                        <asp:TemplateField HeaderText="Parámetro" AccessibleHeaderText="ParameterCode">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblCode" runat="server" Text='<%# Eval ( "ParameterCode" ) %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                                                        <asp:TemplateField HeaderText="Valor Actual" AccessibleHeaderText="ParameterValue">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtParameterValue" Enabled="true" Width="50px" runat="server" Text='<%# Bind ( "ParameterValue" ) %>'></asp:TextBox>
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
                                                        <asp:TemplateField HeaderText="Módulo" AccessibleHeaderText="ModuleName">
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
                                            </div>
                                        </div>
                                    </div>
                                    <div id="div1ValidationSummary" class="divValidationSummary" runat="server">
                                        <asp:ValidationSummary ID="valSummary" runat="server" ValidationGroup="EditNew" />
                                    </div>
                                    <div id="divWarning" class="divWarning" runat="server" visible="false">
                                        <asp:Label ID="lblError" Visible="false" runat="server" ForeColor="Red" />
                                        <asp:Label ID="lblErrorTitle" Visible="false" runat="server" Text="Dato no válido" />
                                    </div>
                                </div>
                            </div>
                            <%-- FIN Grilla Principal --%>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$leftPanel$ctl01$trvInboundType" EventName="SelectedNodeChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="uprItem" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress3" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprItem" runat="server" ControlToOverlayID="divGrid" CssClass="updateProgress" TargetControlID="uprItem" />
                
                </Content>
                <Footer Height="67">
                    <div style="color: White">
                        No Borrar - Evita que BottomPanel se solape con StatusBar
                    </div>
                </Footer>
            </RightPanel>
        </spl:Splitter>
    </div>

</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
