<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="ItemParametersMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Configuration.ItemParametersMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
<script src="<%= Page.ResolveClientUrl("~/WebResources/Script/CalculateHeightWithVerticalAndHorizontalSppliter.js")%>"></script>
<script type="text/javascript" language='Javascript'>   
    function resizeDivPrincipal() {
        //debugger;
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divMain").style.height = h;
        document.getElementById("ctl00_MainContent_divMain").style.width = w;
    }


    function ChangeChecks(object, Id) {
        
        //var tabla = document.getElementById("ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl01_ctl01_grdItem");
        var list = document.getElementById("ctl00_MainContent_hidListIdItems").value;
        if (object.checked) {
            list = list + Id + '~';
        } else {
            list = list.replace(Id + '~', '');
        }
        
        document.getElementById("ctl00_MainContent_hidListIdItems").value = list;        

    }

    function initializeGridDragAndDropCustom() {
        initializeGridWithNoDragAndDrop(true);
    }

    $(document).ready(function () {
        setWidthWithVerticalSplitter();
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
        //$("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl01_ctl01_divFilter").css({ "top": "0", "z-index": "99998 !important"});
        setWidthWithVerticalSplitter();
    }

</script> 

    <style>
        .ob_spl_rightpanelcontent{
            overflow: visible !important;
        }

        #ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl01_ctl01_divFilter{
            top: 0;
            z-index: 9998 !important;
        }
    </style>

    <div id="divMain" style="width: 100%; height: 100%; margin: 0px; margin-bottom: 80px" runat="server">
        <spl:Splitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="setWidthWithVerticalSplitter();">
            <LeftPanel ID="leftPanel" WidthDefault="200" WidthMin="100">
                <Content>
                    <asp:UpdatePanel ID="upTreeView" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <%-- Árbol --%>
                            <asp:TreeView ID="treevLocation" runat="server" OnSelectedNodeChanged="treevLocation_SelectedNodeChanged"
                                ShowLines="True">
                                <ParentNodeStyle Font-Bold="False" />
                                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px"
                                    VerticalPadding="0px" />
                                <Nodes>
                                    <asp:TreeNode Expanded="True"  Value="Company" SelectAction="SelectExpand"></asp:TreeNode>
                                </Nodes>
                                <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="0px"
                                    NodeSpacing="0px" VerticalPadding="0px" />
                            </asp:TreeView>
                            <%-- FIN Árbol--%>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm1"
                                EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm2"
                                EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm3"
                                EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm4"
                                EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="uprTreeView" runat="server" AssociatedUpdatePanelID="upTreeView" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress0" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprTreeView" runat="server" ControlToOverlayID="treevLocation" CssClass="updateProgress" TargetControlID="uprTreeView" />
                    
                    
                </Content>
                <Footer Height="67">
                    <div style="color: White">
                        No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </LeftPanel>
            <RightPanel>
                <Content>
                    <spl:HorizontalSplitter ID="HorizontalSplitter1" CookieDays="0" runat="server" StyleFolder="../WebResources/styles/default" OnSplitterResize="SetDivs();">
                         <TopPanel HeightMin="0" HeightMax="700">
                            <Content>
                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <%-- Grilla Principal --%>
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <div id="divPar" runat="server" class="divGridRightPanel">
                                                        <div id="lpnHeadTitle" runat="server" visible="true" class="divGridTitle">
                                                            <asp:Label ID="lblTitle" runat="server"></asp:Label>
                                                        </div>
                                                        <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" 
                                                            OnRowCreated="grdMgr_RowCreated"                                             
                                                            OnRowDataBound="grdMgr_RowDataBound"
                                                            OnRowCommand="grdMgr_RowCommand"
                                                            AllowPaging="false" EnableViewState="false"
                                                            AutoGenerateColumns="False"  
                                                            ShowFooter="False"
                                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                            EnableTheming="false">
                                                            <Columns>
                                                                <%--IMPORTANTE: no cambiar esta columna de lugar--%>
                                                                <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                                                    AccessibleHeaderText="Id" visible = "false"/>
                                                                <asp:TemplateField HeaderText="Id" AccessibleHeaderText="IdParameter">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblIdParameter" runat="server" Text='<%# Eval ( "Id" ) %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>                                                        
                                                                <asp:TemplateField HeaderText="Parámetro" AccessibleHeaderText="ParameterCode">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblCode" runat="server" Text='<%# Eval ( "ParameterCode" ) %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                                                                <asp:TemplateField HeaderText="Valor Actual" AccessibleHeaderText="ParameterValue">
                                                                    <ItemTemplate>
                                                                        <%--<asp:CheckBox ID="chkParameterValue" runat="server" Checked='<%# (bool) Eval ( "ParameterValue" ) %>' Enabled="false" />--%>
                                                                        <asp:TextBox ID="txtParameterValue" Enabled="true" Width="50px" runat="server" Text='<%# Eval ( "ParameterValue" )  %>'></asp:TextBox>
                                                                       <%--Text='<%# ((DateTime) Eval ("EmissionDate") > DateTime.MinValue)? Eval("EmissionDate", "{0:d}"):"" %>'                                                            --%>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ShowHeader="False" AccessibleHeaderText="Overwrite">
                                                                    <HeaderTemplate>
                                                                        <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkOverwrite', this.checked)" id="chkAll" title="Seleccionar todos" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <center>
                                                                            <div style="width: 20px">
                                                                                <asp:CheckBox ID="chkOverwrite" runat="server" Visible="True" title="Aplicar valor a niveles inferiores"/>
                                                                            </div>
                                                                        </center>
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
                                                                <asp:TemplateField HeaderText="Diferencia" Visible="false" AccessibleHeaderText="Diferencias">
                                                                    <ItemTemplate>
                                                                        <center>
                                                                            <asp:Label ID="lblIsDifferent" runat="server" Text='<%# Eval ( "Different" ) %>'
                                                                                Visible="true" />
                                                                        </center>
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
                                                                <asp:TemplateField HeaderText="Filtro" AccessibleHeaderText="Filter">
                                                                    <ItemTemplate>
                                                                        <center>
                                                                            <asp:DropDownList EnableViewState="true" Visible="false" DataTextField='<%# Eval ( "Id" ) %>' ID="ddlFilter" runat="server" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged"
                                                                                Enabled="false" AutoPostBack="true">
                                                                                <asp:ListItem Value="-2" >(sel)</asp:ListItem>
                                                                                <asp:ListItem Value="-1" >Todos</asp:ListItem>
                                                                                <asp:ListItem Value="0" >0</asp:ListItem>
                                                                                <asp:ListItem Value="1" >1</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                            <asp:TextBox Visible="false" ID="txtFilter" runat="server" Width="30" ValidationGroup='<%# Eval ( "Id" ) %>'/>
                                                                            <asp:ImageButton ID="btnFilter" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search_dis.png" Visible="false" CommandName="Filter"/>
                                                                        </center>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                        <div id="div1ValidationSummary" class="divValidationSummary" runat="server">
                                                            <asp:ValidationSummary ID="valSummary" runat="server" ValidationGroup="EditNew" />
                                                        </div>
                                                        <div id="divWarning" class="divWarning" runat="server" visible="false">
                                                            <asp:Label ID="lblError" Visible="false" runat="server" ForeColor="Red" />
                                                            <asp:Label ID="lblErrorTitle" Visible="false" runat="server" Text="Dato no válido" />
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$leftPanel$ctl01$treevLocation" EventName="SelectedNodeChanged" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm1"
                                                        EventName="SelectedIndexChanged" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm2"
                                                        EventName="SelectedIndexChanged" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm3"
                                                        EventName="SelectedIndexChanged" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm4"
                                                        EventName="SelectedIndexChanged" />                                            
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </div>
                                
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="20" DynamicLayout="true">
                                    <ProgressTemplate>
                                        <div class="divProgress">
                                            <asp:ImageButton ID="imgProgress3" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <webUc:UpdateProgressOverlayExtender ID="muprItem01" runat="server" ControlToOverlayID="divPar" CssClass="updateProgress" TargetControlID="UpdateProgress1" />
                                <%-- FIN Grilla Principal --%>
                            
                            </Content>
                        </TopPanel>
                        <BottomPanel>
                            <Content>      
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divFilter" class="mainFilterPanel" runat="server" visible="false">
                                            <%-- Group Item 1 (Sector) --%>
                                            <div id="divBscGroupItems" runat="server" visible="true">
                                                <div id="divBscGrpItm1" runat="server" visible="true" class="mainFilterPanelItem">
                                                    <b><asp:Label ID="lblTitleGrpItm1" runat="server" Visible="true" Text="Sector" /></b><br />
                                                    <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm1" Visible="true" runat="server"
                                                        AutoPostBack="True" OnSelectedIndexChanged="ddlBscGrpItm1_SelectedIndexChanged"
                                                        Width="130px" />
                                                    <div id="divLblGrp1">
                                                        <asp:Label ID="lblNameGrp1" runat="server" Text="" Visible="false"></asp:Label>
                                                    </div>
                                                </div>
                                                <%-- Group Item 2 (Rubro) --%>
                                                <div id="divBscGrpItm2" runat="server" visible="false" class="mainFilterPanelItem">
                                                    <b><asp:Label ID="lblTitleGrpItm2" Visible="true" runat="server" Text="Rubro" /></b><br />
                                                    <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm2" Visible="false" runat="server"
                                                        AutoPostBack="True" OnSelectedIndexChanged="ddlBscGrpItm2_SelectedIndexChanged"
                                                        Width="130px" />
                                                    <div id="divLblGrp2">
                                                        <asp:Label ID="lblNameGrp2" runat="server" Text="" Visible="false"></asp:Label>
                                                    </div>
                                                </div>
                                                <%-- Group Item 3 (Familia) --%>
                                                <div id="divBscGrpItm3" runat="server" visible="false" class="mainFilterPanelItem">
                                                    <b><asp:Label ID="lblTitleGrpItm3" runat="server" Text="Familia" /></b><br />
                                                    <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm3" Visible="true" runat="server"
                                                        AutoPostBack="True" OnSelectedIndexChanged="ddlBscGrpItm3_SelectedIndexChanged"
                                                        Width="130px" />
                                                    <div id="divLblGrp3">
                                                        <asp:Label ID="lblNameGrp3" runat="server" Text="" Visible="false"></asp:Label>
                                                    </div>
                                                </div>
                                                <%-- Group Item 4 (Subfamilia) --%>
                                                <div id="divBscGrpItm4" runat="server" visible="false" class="mainFilterPanelItem">
                                                    <b><asp:Label ID="lblTitleGrpItm4" runat="server" Text="Subfamilia" /></b><br />
                                                    <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm4" Visible="true" runat="server"
                                                        AutoPostBack="True" OnSelectedIndexChanged="ddlBscGrpItm4_SelectedIndexChanged"
                                                        Width="130px" />
                                                    <div id="divLblGrp4">
                                                        <asp:Label ID="lblNameGrp4" runat="server" Text="" Visible="false"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="divFilterItem" runat="server" visible="false" class="divFilterItemCenter">
                                                <div id="divBtnRefresh" runat="server" class="mainFilterPanelItem" visible="true">
                                                    <asp:ImageButton ID="btnItemRefresh" runat="server" OnClick="btnItemRefresh_Click"
                                                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_refresh.png" onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_refresh_on.png';"
                                                        onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_refresh.png';"
                                                        Visible="true" ToolTip="Actualizar"/>
                                                </div>
                                                <div id="divCode" runat="server" class="mainFilterPanelItem" visible="true">
                                                    &nbsp;<asp:Label ID="lblItemCode" runat="server" Text="Código" /><br />
                                                    <asp:TextBox SkinID="txtFilter" ID="txtCode" runat="server" Width="70px" />
                                                </div>
                                                <%-- FIN Grilla Principal --%>
                                                <div id="divName" runat="server" class="mainFilterPanelItem" visible="true">
                                                    &nbsp;<asp:Label ID="lblItemName" runat="server" Text="Nombre" /><br />
                                                    <asp:TextBox SkinID="txtFilter" ID="txtName" runat="server" Width="70px" />
                                                </div>
                                                <%-- Group Item 1 (Sector) --%>
                                                <div id="divDescription" runat="server" class="mainFilterPanelItem" visible="true">
                                                    &nbsp;<asp:Label ID="lblItemDescription" runat="server" Text="Descripción" /><br />
                                                    <asp:TextBox SkinID="txtFilter" ID="txtDescription" runat="server" Width="70px" />
                                                </div>
                                                <div class="mainFilterPanelItem">
                                                    <asp:ImageButton ID="btnSearchItem" runat="server" OnClick="btnSearchItem_Click"
                                                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" ToolTip="Buscar"/>
                                                </div>
                                            </div>
                                        </div>
                                        <br />
                                        <div class="container">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div id="divGrid" runat="server" visible="true" class="divGridRightPanel">
                                                        <%-- Grilla de Items --%>
                                                        <div id="divDetail" runat="server">
                                                            <asp:GridView 
                                                                ID="grdItem" 
                                                                runat="server" 
                                                                OnRowCreated="grdItem_RowCreated" 
                                                                DataKeyNames="Id"
                                                                Visible="true" 
                                                                AllowPaging="true" 
                                                                EnableViewState="false"
                                                                AutoGenerateColumns="False"                                                    
                                                                OnPageIndexChanging="grdItem_PageIndexChanging"
                                                                OnSelectedIndexChanged="grdItem_SelectedIndexChanged"
                                                                OnRowDataBound="grdItem_RowDataBound"
                                                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                                EnableTheming="false">    
                                                                <Columns>
                                                                    <asp:BoundField DataField="Id" Visible="false" HeaderText="Id" AccessibleHeaderText="Id" />
                                                                    <asp:TemplateField ShowHeader="False" AccessibleHeaderText="Actions">
                                                                        <HeaderTemplate>
                                                                            <input type="checkbox" onclick="toggleCheckBoxes('<%= grdItem.ClientID %>', 'chkSelectItem', this.checked)" id="chkAll" title="Seleccionar todos" />
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <center>
                                                                                <div style="width: 20px">
                                                                                    <asp:CheckBox ID="chkSelectItem" runat="server"   />
                                                                                </div>
                                                                            </center>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Id" >
                                                                        <ItemTemplate>
                                                                            <asp:Label runat="server" ID="lblIdItem" Text='<%# Bind("Id") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Cód Item">
                                                                        <ItemTemplate>
                                                                            <asp:Label runat="server" ID="lblCode" Text='<%# Bind("Code") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Nombre">
                                                                        <ItemTemplate>
                                                                            <asp:Label runat="server" ID="lblLongName" Text='<%# Bind("LongName") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Descripción">
                                                                        <ItemTemplate>
                                                                            <asp:Label runat="server" ID="lblDescription" Text='<%# Bind("Description") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    <asp:Label ID="lblEmptyGrid" runat="server" Text="No se han encontrado registros." />
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                        <%--FIN Grilla de Items --%>
                                                    </div> 
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                        <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$leftPanel$ctl01$treevLocation" EventName="SelectedNodeChanged" />
                                            
                                        </Triggers>
                                    
        <%--                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$leftPanel$ctl01$treevLocation" 
                                        EventName="SelectedNodeChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$grdMgr$ctl02$ddlFilter" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$grdMgr$ctl03$ddlFilter" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$grdMgr$ctl04$ddlFilter" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$grdMgr$ctl05$ddlFilter" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$grdMgr$ctl06$ddlFilter" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$grdMgr$ctl07$ddlFilter" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$grdMgr$ctl08$ddlFilter" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$grdMgr$ctl09$ddlFilter" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$grdMgr$ctl10$ddlFilter" EventName="SelectedIndexChanged" />
                                    </Triggers>--%>
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
                        </BottomPanel>
                    </spl:HorizontalSplitter>
                </Content>
                <Footer Height="67">
                    <div style="color: White">
                        No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </RightPanel>
        </spl:Splitter>
    </div>

<%-- Mensajes de Confirmacion y Auxiliares --%>
<asp:Label ID="lblSimbol" runat="server" Text="   >   " Visible="false" />
<asp:HiddenField ID="hidListIdItems" runat="server" Value="" />
<%-- FIN Mensajes de Confirmacion y Auxiliares --%>        
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
