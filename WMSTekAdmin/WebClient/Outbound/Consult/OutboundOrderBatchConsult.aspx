<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="OutboundOrderBatchConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.OutboundOrderBatchConsult" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language='Javascript'>
        function resizeDiv() {
            //debugger;
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("divPrincipal").style.height = h;
            document.getElementById("divPrincipal").style.width = w;
        }
        window.onresize = resizeDiv;

        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdMgrWave';
            clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('Batch_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
            initializeGridWithNoDragAndDrop(true);
        }

        function setDivsAfter() {
            var heightDivBottom = $("#__hsMasterDetailRD").height();
            var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetailTitle").height();
            var extraSpace = 85;

            var totalHeight = heightDivBottom - heightLabelsBottom - extraSpace;
            $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdMgrWave").parent().css("max-height", totalHeight + "px");
        }
            function postbackAction(index) {
                javascript: __doPostBack('ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr', 'CancelOrder$' + index);
        }

        function cancelBatch(index) {

            var value = confirm("¿Desea anular este Batch?");

            if (value) {
                postbackAction(index);
                return false;
            } else {
                return value;
            }
        }
    
    </script> 

    <div id="divPrincipal" style="margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter   CookieDays="0"  ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel"  >
                <Header Height="1">
				    <div style="width:100%;height:100%;" >			
				    </div>
			    </Header>
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid"  runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="grdMgr" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            AllowPaging="True" 
                                            EnableViewState="False"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            OnRowCommand="grdMgr_RowCommand"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">

                                            <Columns>
                                                <asp:TemplateField ShowHeader="False" HeaderText="Acciones" HeaderStyle-HorizontalAlign="Center" AccessibleHeaderText="Actions">
                                                    <ItemTemplate>
                                                        <center>
                                                            <div style="width: 60px">
                                                                <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_cancel.png"
                                                                    CausesValidation="false" CommandName="CancelOrder" ToolTip="Anular" 
                                                                    CommandArgument="<%# Container.DataItemIndex %>" OnClientClick=<%# string.Format("return cancelBatch({0});", Container.DataItemIndex) %>  />
                                                            </div>
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Id Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Task.Warehouse.Id" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="WhsName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Task.Warehouse.ShortName" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerId" runat="server" Text='<%# Eval ( "Task.Owner.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ( "Task.Owner.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:templatefield HeaderText="N° Batch" AccessibleHeaderText="IdBatch">
                                                    <itemtemplate>
                                                       <div style="word-wrap: break-word;">
                                                            <asp:label ID="lblIdBatch" runat="server" text='<%# Eval ( "Task.Id" ) %>' />
                                                       </div>
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:TemplateField HeaderText="Creada" AccessibleHeaderText="CreateDate" SortExpression="CreateDate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCreateDate" runat="server" Text='<%# Eval ( "Task.CreateDate" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

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

                    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="30" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgressgrdMgr" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

                </Content>
            </TopPanel>
            <BottomPanel HeightMin="50">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <div runat="server" id="divGridBatch">
                                    <asp:UpdatePanel ID="upGridBatch"  runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                        <ContentTemplate>
                                            <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                                <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
	                                                <asp:Label ID="lblGridDetail" runat="server" Text="Docs. Batch: " />
	                                                <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                                </div>

                                                <asp:GridView ID="grdMgrBatch" runat="server" 
                                                    OnRowCreated="grdMgrBatch_RowCreated"
                                                    EnableViewState="False"
                                                    AutoGenerateColumns="false"
                                                    OnRowDataBound="grdMgrBatch_RowDataBound"
                                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                    EnableTheming="false">

                                                    <Columns>

                                                        <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" >
                                                            <itemtemplate>
                                                                <div style="word-wrap: break-word;">
                                                                   <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                                </div>
                                                                </itemtemplate>
                                                        </asp:templatefield>

                                                        <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                                            <itemtemplate>
                                                               <div style="word-wrap: break-word;">
                                                                <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                               </div>
                                                            </itemtemplate>
                                                         </asp:templatefield>

                                                        <asp:templatefield HeaderText="Cód. CD. Destino" AccessibleHeaderText="WarehouseTargetCode"  >
                                                            <itemtemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:label ID="lblWarehouseTargeteCode" runat="server" text='<%# Eval ( "WarehouseTarget.Code" ) %>' />
                                                                </div>
                                                            </itemtemplate>
                                                        </asp:templatefield>

                                                        <asp:templatefield HeaderText="Centro Dist. Destino" AccessibleHeaderText="WarehouseTarget">
                                                            <itemtemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:label ID="lblWarehouseTarget" runat="server" text='<%# Eval ( "WarehouseTarget.ShortName" ) %>' />
                                                               </div>
                                                            </itemtemplate>
                                                        </asp:templatefield>    
                                                        
                                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" >
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:BoundField HeaderText="Nº Doc." DataField="Number" AccessibleHeaderText="OutboundNumber" />

                                                        <asp:TemplateField AccessibleHeaderText="OutboundTypeCode" HeaderText="Tipo Doc.">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:label ID="lblOutboundType" runat="server" text='<%# Eval ( "OutboundType.Code" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Tipo Doc." AccessibleHeaderText="OutboundTypeName">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "OutboundType.Name" ) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerCode" >
                                                            <itemtemplate>
                                                                <center>
                                                                    <div style="word-wrap: break-word;">
                                                                         <asp:Label ID="lblCustomerCode" runat="server" text='<%# Eval ( "CustomerCode" ) %>'></asp:Label>
                                                                     </div>
                                                                </center>    
                                                            </itemtemplate>
                                                        </asp:TemplateField> 

                                                        <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerName" >
                                                            <itemtemplate>
                                                                <center>
                                                                    <div style="word-wrap: break-word;">
                                                                         <asp:Label ID="lblCustomerName" runat="server" text='<%# Eval ( "CustomerName" ) %>'></asp:Label>
                                                                    </div>
                                                                </center>    
                                                            </itemtemplate>
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
                                             <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="RowCommand" />
                                             <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="SelectedIndexChanged" />          
                                         </Triggers>
                                    </asp:UpdatePanel>

                                    <asp:UpdateProgress ID="uprGridBatch" runat="server" AssociatedUpdatePanelID="upGridBatch" DisplayAfter="30" DynamicLayout="true">
                                        <ProgressTemplate>
                                            <div class="divProgress">
                                                <asp:ImageButton ID="imgProgressGridBatch" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                    <webUc:UpdateProgressOverlayExtender ID="muprGridBatch" runat="server" ControlToOverlayID="divGridBatch" CssClass="updateProgress" TargetControlID="uprGridBatch" />

                                </div>
                            </div>
                        </div>
                    </div>
                </Content>
                 <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>

    </div>

    <asp:Label id="lblFilterDate" runat="server" Text="Emisión" Visible="false" />   
    <asp:Label ID="lblBatchNbr" runat="server" Text="N° Batch" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>
