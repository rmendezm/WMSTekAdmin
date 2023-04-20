<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="ZoneItemMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Configuration.ZoneItemMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/CalculateHeightWithVerticalAndHorizontalSppliter.js")%>"></script>

    <script type="text/javascript" language='Javascript'>  

        function resizeDivPrincipal() {
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("ctl00_MainContent_divMain").style.height = h;
            document.getElementById("ctl00_MainContent_divMain").style.width = w;
        }
        window.onresize = resizeDivPrincipal;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDivPrincipal);

        function validate(id) {
            var tabla = document.getElementById('ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl00_ctl01_grdMgr');
            for (i = 1; i < tabla.rows.length; i++) {
                tabla.rows[i].cells[1].firstChild.checked = true;
            }
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
            setWidthWithVerticalSplitter();
        }

    </script>

    <div id="divMain" style="width: 100%; height: 100%; margin: 0px; margin-bottom: 80px" runat="server">
        <spl:Splitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="setWidthWithVerticalSplitter();">
            <LeftPanel ID="leftPanel" WidthDefault="200" WidthMin="100">
                <Content>
                    <asp:UpdatePanel ID="upTreeView" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:TreeView ID="treevLocation" runat="server" OnSelectedNodeChanged="treevLocation_SelectedNodeChanged" ShowLines="True">
                                <ParentNodeStyle Font-Bold="False" />
                                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
                                <Nodes>
                                    <asp:TreeNode Expanded="True"  Value="Company" SelectAction="SelectExpand"></asp:TreeNode>
                                </Nodes>
                                <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                            </asp:TreeView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm1" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm2" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm3" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm4" EventName="SelectedIndexChanged" />
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
                        No Borrar - Evita que BottomPanel se solape con StatusBar
                    </div>
                </Footer>
            </LeftPanel>
            <RightPanel>
                <Content>
                     <spl:HorizontalSplitter ID="HorizontalSplitter1" CookieDays="0" runat="server" StyleFolder="../WebResources/styles/default" OnSplitterResize="SetDivs();">
                        <TopPanel HeightMin="0" HeightMax="500">
                            <Content>
                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:UpdatePanel ID="upZones" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <div id="lpnHeadTitle" runat="server" visible="true" class="divGridTitle">
                                                        <asp:Label ID="lblTitle" runat="server"></asp:Label>
                                                     </div>
                                                    <div id="divPar" runat="server" class="divGridRightPanel">

                                                        <asp:GridView ID="grdMgr" 
                                                            DataKeyNames="Id" 
                                                            runat="server"                                                 
                                                            OnRowCreated="grdMgr_RowCreated"
                                                            AllowPaging="false" 
                                                            EnableViewState="true"
                                                            AutoGenerateColumns="False"  
                                                            OnRowDataBound="grdMgr_RowDataBound"
                                                            ShowFooter="False"
                                                            CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                            EnableTheming="false">                
                                                            <Columns>
                                                                <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True" AccessibleHeaderText="Id" Visible="false" />

                                                                <asp:TemplateField ShowHeader="False" AccessibleHeaderText="Actions">
                                                                    <HeaderTemplate>
                                                                        <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectZone', this.checked)" id="chkAll" title="Seleccionar todos" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <center>
                                                                            <div style="width: 20px">
                                                                                <asp:CheckBox ID="chkSelectZone" runat="server"  Checked='<%# ((bool) Eval("IsAssignedZoneItem")) %>' />
                                                                            </div>
                                                                        </center>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Nombre Zona" AccessibleHeaderText="Name">
                                                                    <ItemTemplate>
                                                                        <div >
                                                                            <asp:Label ID="lblName" runat="server" Text='<%# Eval ( "Name" ) %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Description">
                                                                    <ItemTemplate>
                                                                        <div >
                                                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ( "Description" ) %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="C. Dist." AccessibleHeaderText="WhsName">
                                                                    <ItemTemplate>
                                                                        <div >
                                                                            <asp:Label ID="lblWhsName" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>' />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                <asp:Label ID="lblEmptyGrid" runat="server" Text="No se han encontrado registros." />
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>

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
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm1" EventName="SelectedIndexChanged" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm2" EventName="SelectedIndexChanged" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm3" EventName="SelectedIndexChanged" />
                                                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$ddlBscGrpItm4" EventName="SelectedIndexChanged" />                                            
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </div>

                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upZones" DisplayAfter="20" DynamicLayout="true">
                                    <ProgressTemplate>
                                        <div class="divProgress">
                                            <asp:ImageButton ID="imgProgress3" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <webUc:UpdateProgressOverlayExtender ID="muprItem01" runat="server" ControlToOverlayID="divPar" CssClass="updateProgress" TargetControlID="UpdateProgress1" />

                            </Content>
                        </TopPanel>
                        <BottomPanel>
                            <Content>
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divFilter" runat="server" visible="false">
                                            <%-- Group Item 1 (Sector) --%>
                                            <div id="divBscGroupItems" runat="server" visible="true">
                                                <div id="divBscGrpItm1" runat="server" visible="true" class="mainFilterPanelItem">
                                                    <b><asp:Label ID="lblTitleGrpItm1" runat="server" Visible="true" Text="Sector" /></b>
                                                    <br />
                                                    <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm1" Visible="true" runat="server"
                                                        AutoPostBack="True" OnSelectedIndexChanged="ddlBscGrpItm1_SelectedIndexChanged"
                                                        Width="130px" />
                                                    <div id="divLblGrp1">
                                                        <asp:Label ID="lblNameGrp1" runat="server" Text="" Visible="false"></asp:Label>
                                                    </div>
                                                </div>
                                                <%-- Group Item 2 (Rubro) --%>
                                                <div id="divBscGrpItm2" runat="server" visible="false" class="mainFilterPanelItem">
                                                    <b><asp:Label ID="lblTitleGrpItm2" Visible="true" runat="server" Text="Rubro" /></b>
                                                    <br />
                                                    <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm2" Visible="false" runat="server"
                                                        AutoPostBack="True" OnSelectedIndexChanged="ddlBscGrpItm2_SelectedIndexChanged"
                                                        Width="130px" />
                                                    <div id="divLblGrp2">
                                                        <asp:Label ID="lblNameGrp2" runat="server" Text="" Visible="false"></asp:Label>
                                                    </div>
                                                </div>
                                                <%-- Group Item 3 (Familia) --%>
                                                <div id="divBscGrpItm3" runat="server" visible="false" class="mainFilterPanelItem">
                                                    <b><asp:Label ID="lblTitleGrpItm3" runat="server" Text="Familia" /></b>
                                                    <br />
                                                    <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm3" Visible="true" runat="server"
                                                        AutoPostBack="True" OnSelectedIndexChanged="ddlBscGrpItm3_SelectedIndexChanged"
                                                        Width="130px" />
                                                    <div id="divLblGrp3">
                                                        <asp:Label ID="lblNameGrp3" runat="server" Text="" Visible="false"></asp:Label>
                                                    </div>
                                                </div>
                                                <%-- Group Item 4 (Subfamilia) --%>
                                                <div id="divBscGrpItm4" runat="server" visible="false" class="mainFilterPanelItem">
                                                    <b><asp:Label ID="lblTitleGrpItm4" runat="server" Text="Subfamilia" /></b>
                                                    <br />
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
                                                    &nbsp;<asp:Label ID="lblItemCode" runat="server" Text="Código" />
                                                    <br />
                                                    <asp:TextBox SkinID="txtFilter" ID="txtCode" runat="server" Width="70px" />
                                                </div>
                                                <%-- FIN Grilla Principal --%>
                                                <div id="divName" runat="server" class="mainFilterPanelItem" visible="true">
                                                    &nbsp;<asp:Label ID="lblItemName" runat="server" Text="Nombre" />
                                                    <br />
                                                    <asp:TextBox SkinID="txtFilter" ID="txtName" runat="server" Width="70px" />
                                                </div>
                                                <%-- Group Item 1 (Sector) --%>
                                                <div id="divDescription" runat="server" class="mainFilterPanelItem" visible="true">
                                                    &nbsp;<asp:Label ID="lblItemDescription" runat="server" Text="Descripción" />
                                                    <br />
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
                                                                OnRowDataBound="grdItem_RowDataBound"
                                                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                                EnableTheming="false">    
                                                                <Columns>
                                                                    <asp:BoundField DataField="Id" Visible="false" HeaderText="Id" AccessibleHeaderText="Id" />

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
                        No Borrar - Evita que BottomPanel se solape con StatusBar
                    </div>
                </Footer>
            </RightPanel>
        </spl:Splitter>
    </div>

    <asp:Label ID="lblSimbol" runat="server" Text="   >   " Visible="false" />
    <asp:Label id="lblNoGroupSelected" runat="server" Text="Debe seleccionar al menos un sector." Visible="false" /> 

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
