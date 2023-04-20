<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="OutboundOrdersPendingReplenishment.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.OutboundOrdersPendingReplenishment" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">

        function resizeDiv() {
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("divPrincipal").style.height = h;
            document.getElementById("divPrincipal").style.width = w;
        }
        window.onresize = resizeDiv;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDiv);

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            //clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('TaskMgr_GetTasksPendingReplenishment', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }

    </script>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="40">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="grdMgr" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            AllowPaging="True"
                                            AutoGenerateColumns="false"
                                            EnableViewState="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">

                                                <Columns>

                                                    <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Task.Warehouse.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="OwnName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblOwner" runat="server" text='<%# Eval ( "Task.Owner.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Doc. Salida" AccessibleHeaderText="OutboundNumber" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblOutboundNumber" runat="server" text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Tipo Doc. Salida" AccessibleHeaderText="OutboundType" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblOutboundType" runat="server" text='<%# Eval ( "OutboundOrder.OutboundType.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                   <%-- <asp:templatefield HeaderText="Track Tarea" AccessibleHeaderText="TrackTaskType" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblTrackTaskType" runat="server" text='<%# Eval ( "TrackTaskType.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>--%>

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

                    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

                </Content>
            </TopPanel>
            <BottomPanel HeightMin="120">
                <Content>          
                    <asp:UpdatePanel ID="upGridDetail" runat="server"  UpdateMode="Conditional">
                        <ContentTemplate>     
                            <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-3">
                                            <asp:Label ID="lblGridDetail" runat="server" Text="Orden: " />
                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>  
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="True" 
                                                AutoGenerateColumns="False"
                                                OnRowCreated="grdDetail_RowCreated"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false"
                                                AllowPaging="true">  

                                                <Columns>

                                                    <asp:TemplateField HeaderText="ID" AccessibleHeaderText="IdTaskDetail" ItemStyle-CssClass="text" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblIdTaskDetail" runat="server" text='<%# Eval ( "Id" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:templatefield HeaderText="Tipo Tarea" AccessibleHeaderText="TypeCode" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblTypeCode" runat="server" text='<%# Eval ( "Task.Description" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Track Tarea" AccessibleHeaderText="TrackTaskType" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblTrackTaskType" runat="server" text='<%# Eval ( "Task.TrackTaskType.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:TemplateField HeaderText="Cód. Item" AccessibleHeaderText="ItemCode" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="Item" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItem" runat="server" text='<%# Eval ( "Item.ShortName" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Categoria Item" AccessibleHeaderText="CategoryItem" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCategoryItem" runat="server" text='<%# Eval ( "CategoryItem.Name" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "LotNumber" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Fecha Fifo" AccessibleHeaderText="FifoDate" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue) ? Eval("FifoDate", "{0:d}"):"" %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Fecha Fabricación" AccessibleHeaderText="FabricationDate" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFabricationDate" runat="server" Text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue) ? Eval("FabricationDate", "{0:d}"):"" %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Fecha Expiración" AccessibleHeaderText="ExpirationDate" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue) ? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Ubicación Origen Propuesta" AccessibleHeaderText="IdLocSourceProposal" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblIdLocSourceProposal" runat="server" text='<%# Eval ( "IdLocSourceProposal" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Ubicación Destino Propuesta" AccessibleHeaderText="IdLocTargetProposal" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblIdLocTargetProposal" runat="server" text='<%# Eval ( "IdLocTargetProposal" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Cant. Propuesta" AccessibleHeaderText="ProposalQty" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblProposalQty" runat="server" text='<%# Eval ( "ProposalQty" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                </Columns>

                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
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
                        </Triggers>
                    </asp:UpdatePanel>

                    <asp:UpdateProgress ID="uprGridDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprGridDetail" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGridDetail" />

                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>
    </div>

    <asp:Label id="lblDocName" runat="server" Text="Nº Doc." Visible="false" />    
    <asp:Label id="lblFilterDate" runat="server" Text="Emisión" Visible="false" />  

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
