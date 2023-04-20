<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="DispatchAdvanceBatchConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.DispatchAdvanceBatchConsult" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" language='Javascript'>
        function resizeDiv() {
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("divPrincipal").style.height = h;
            document.getElementById("divPrincipal").style.width = w;
        }
        window.onresize = resizeDiv;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDiv);

        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('DispatchAdvanced_FindAllBatch', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }
    </script>

     <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
         <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
             <TopPanel ID="topPanel" HeightMin="50">
                 <Content>
                     <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <asp:GridView ID="grdMgr" runat="server" 
                                            DataKeyNames="Id" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            EnableViewState="False"                
                                            AllowPaging="True" 
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>

                                                <asp:TemplateField HeaderText="Id Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Outbound.Warehouse.Id" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="WhsName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Outbound.Warehouse.ShortName" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                 <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerId" runat="server" Text='<%# Eval ( "Outbound.Owner.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ( "Outbound.Owner.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:templatefield HeaderText="N° Batch" AccessibleHeaderText="IdBatch">
                                                    <itemtemplate>
                                                       <div style="word-wrap: break-word;">
                                                            <asp:label ID="lblIdBatch" runat="server" text='<%# Eval ( "Outbound.Id" ) %>' />
                                                       </div>
                                                    </itemtemplate>
                                                </asp:templatefield>

                                                <asp:templatefield headertext="Creada" accessibleHeaderText="CreateDate">
                                                    <ItemStyle Wrap="false" />
                                                    <itemtemplate>
                                                        <center>
                                                            <asp:label ID="lblEmissionDate" runat="server" text='<%# ((DateTime) Eval ("Outbound.EmissionDate") > DateTime.MinValue)? Eval("Outbound.EmissionDate", "{0:d}"):"" %>' />
                                                        </center>    
                                                    </itemtemplate>
                                                </asp:templatefield>  

                                                <asp:TemplateField HeaderText="Solicitado" accessibleHeaderText="QtySolicitado" SortExpression="QtySolicitado">
                                                     <ItemTemplate>
                                                          <asp:Label ID="lblQtySolicitado" runat="server" Text='<%# GetFormatedNumber( Eval("QtySolicitado")) %>'></asp:Label>
                                                      </ItemTemplate>
                                                    <ItemStyle Wrap="false"/>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Liberado" AccessibleHeaderText="QtyRelease" SortExpression="QtyRelease">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQtyRelease" runat="server" Text='<%# GetFormatedNumber( Eval("QtyRelease")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="false" />
                                                </asp:TemplateField>

                                                <asp:BoundField HeaderText="Liberado %" AccessibleHeaderText="PctRelease" DataField="PctRelease" />

                                                <asp:TemplateField HeaderText="Picking Uni" AccessibleHeaderText="QtyPicking" SortExpression="QtyPicking">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQtyPicking" runat="server" Text='<%# GetFormatedNumber( Eval("QtyPicking")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="false"/>
                                                </asp:TemplateField>

                                                <asp:BoundField HeaderText="Picking %" accessibleHeaderText="PctPicking" DataField="PctPicking" />

                                                <asp:TemplateField HeaderText="Packing Uni" AccessibleHeaderText="QtyPacking" SortExpression="QtyPacking">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQtyPacking" runat="server" Text='<%#GetFormatedNumber( Eval("QtyPacking")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="false"/>
                                                </asp:TemplateField>

                                                <asp:BoundField HeaderText="Packing %" accessibleHeaderText="PctPacking" DataField="PctPacking" />

                                                <asp:TemplateField HeaderText="Routing Uni" AccessibleHeaderText="QtyRouting" SortExpression="QtyRouting">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQtyRouting" runat="server" Text='<%# GetFormatedNumber( Eval("QtyRouting")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="false"/>
                                                </asp:TemplateField>

                                                <asp:BoundField HeaderText="Routing %" accessibleHeaderText="PctRouting" DataField="PctRouting" />

                                                <asp:TemplateField HeaderText="Loading Uni" AccessibleHeaderText="QtyLoading" SortExpression="QtyLoading">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQtyLoading" runat="server" Text='<%# GetFormatedNumber( Eval("QtyLoading")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="false"/>
                                                </asp:TemplateField>

                                                <asp:BoundField HeaderText="Loading %" accessibleHeaderText="PctLoading" DataField="PctLoading" />

                                                <asp:TemplateField HeaderText="Shipping Uni" AccessibleHeaderText="QtyShipping" SortExpression="QtyShipping">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQtyShipping" runat="server" Text='<%# GetFormatedNumber( Eval("QtyShipping")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="false"/>
                                                </asp:TemplateField>
                               
                                                <asp:BoundField HeaderText="Shipping %" accessibleHeaderText="PctShipping" DataField="PctShipping" />

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
                                <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                            <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
	                                            <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Batch: " />
	                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                            </div>

                                            <asp:GridView ID="grdDetail" runat="server" 
                                                DataKeyNames="Id" EnableViewState="false" 
                                                OnRowCreated="grdDetail_RowCreated" 
                                                SkinID="grdDetail"
                                                AutoGenerateColumns="false"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>

                                                    <asp:TemplateField HeaderText="Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                                          <ItemTemplate>
                                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("OutboundDetail.Item.Code") %>'></asp:Label>
                                                          </ItemTemplate>
                                                          <ItemStyle Wrap="false"/>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="ShortName" SortExpression="ShortName">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("OutboundDetail.Item.ShortName") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>     

                                                    <asp:TemplateField HeaderText="Categoría" accessibleHeaderText="CategoryItem" SortExpression="CategoryItem">
                                                          <ItemTemplate>
                                                            <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval("OutboundDetail.CategoryItem.Name") %>'></asp:Label>
                                                          </ItemTemplate>
                                                          <ItemStyle Wrap="false"/>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Cant. Solicitada" accessibleHeaderText="QtyRequest" SortExpression="QtyRequest">
                                                          <ItemTemplate>
                                                            <asp:Label ID="lblItemQty" runat="server" Text='<%# GetFormatedNumber( Eval("QtyRequest")) %>'></asp:Label>
                                                          </ItemTemplate>
                                                          <ItemStyle Wrap="false"/>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Liberado" AccessibleHeaderText="QtyRelease" SortExpression="QtyRelease">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQtyRelease" runat="server" Text='<%# GetFormatedNumber(Eval("QtyRelease")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Wrap="false" />
                                                    </asp:TemplateField>

                                                    <asp:BoundField HeaderText="Liberado %" AccessibleHeaderText="PctRelease" DataField="PctRelease" />

                                                    <asp:TemplateField HeaderText="Picking Uni" AccessibleHeaderText="QtyPicking" SortExpression="QtyPicking">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQtyPicking" runat="server" Text='<%# GetFormatedNumber( Eval("QtyPicking")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Wrap="false"/>
                                                    </asp:TemplateField>
        
                                                    <asp:BoundField HeaderText="Picking %" AccessibleHeaderText="PctPicking" DataField="PctPicking" />

                                                     <asp:TemplateField HeaderText="Packing Uni" AccessibleHeaderText="QtyPacking" SortExpression="QtyPacking">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQtyPacking" runat="server" Text='<%# GetFormatedNumber( Eval("QtyPacking")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Wrap="false"/>
                                                    </asp:TemplateField>

                                                    <asp:BoundField HeaderText="Packing %" AccessibleHeaderText="PctPacking" DataField="PctPacking" />

                                                    <asp:TemplateField HeaderText="Routing Uni" AccessibleHeaderText="QtyRouting" SortExpression="QtyRouting">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQtyRouting" runat="server" Text='<%# GetFormatedNumber( Eval("QtyRouting")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Wrap="false"/>
                                                    </asp:TemplateField>

                                                    <asp:BoundField HeaderText="Routing %" AccessibleHeaderText="PctRouting" DataField="PctRouting" />

                                                    <asp:TemplateField HeaderText="Loading Uni" AccessibleHeaderText="QtyLoading" SortExpression="QtyLoading">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQtyLoading" runat="server" Text='<%# GetFormatedNumber( Eval("QtyLoading")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Wrap="false"/>
                                                    </asp:TemplateField>
           
                                                    <asp:BoundField HeaderText="Loading %" AccessibleHeaderText="PctLoading" DataField="PctLoading" />

                                                    <asp:TemplateField HeaderText="Shipping Uni" AccessibleHeaderText="QtyShipping" SortExpression="QtyShipping">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQtyShipping" runat="server" Text='<%#GetFormatedNumber( Eval("QtyShipping")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Wrap="false"/>
                                                    </asp:TemplateField>
         
                                                    <asp:BoundField HeaderText="Shipping %" AccessibleHeaderText="PctShipping" DataField="PctShipping" />

                                                    <asp:templatefield headertext="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                                        <itemtemplate>
                                                            <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "OutboundDetail.LotNumber" ) %>'></asp:Label>
                                                        </itemtemplate>
                                                    </asp:templatefield>
                            
                                                    <asp:templatefield headertext="Fifo" AccessibleHeaderText="FifoDate">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <asp:label ID="lblFifoDate" runat="server" text='<%# ((DateTime) Eval ("OutboundDetail.FifoDate") > DateTime.MinValue)? Eval("OutboundDetail.FifoDate", "{0:d}"):"" %>' />
                                                            </center>    
                                                        </itemtemplate>
                                                    </asp:templatefield> 
                            
                                                    <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <asp:label ID="lblExpirationDate" runat="server" text='<%# ((DateTime) Eval ("OutboundDetail.ExpirationDate") > DateTime.MinValue)? Eval("OutboundDetail.ExpirationDate", "{0:d}"):"" %>' />
                                                            </center>    
                                                        </itemtemplate>
                                                    </asp:templatefield> 
                                                      
                                                    <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <asp:label ID="lblFabricationDate" runat="server" text='<%# ((DateTime) Eval ("OutboundDetail.FabricationDate") > DateTime.MinValue)? Eval("OutboundDetail.FabricationDate", "{0:d}"):"" %>' />
                                                            </center>    
                                                        </itemtemplate>
                                                    </asp:templatefield> 

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
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                     </div>

                     <asp:UpdateProgress ID="uprGridDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="30" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgressGridDetail" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprGridDetail" runat="server" ControlToOverlayID="divDetail" CssClass="updateProgress" TargetControlID="uprGridDetail" />

                 </Content>
                  <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
             </BottomPanel>
         </spl:HorizontalSplitter>
     </div>

     <asp:Label id="lblFilterDate" runat="server" Text="Emisión" Visible="false" />    	
     <asp:Label id="lblFilterDoc" runat="server" Text="Nº Batch" Visible="false" /> 
     <asp:Label ID="lblBatchNbr" runat="server" Text="N° Batch" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>  
</asp:Content>
