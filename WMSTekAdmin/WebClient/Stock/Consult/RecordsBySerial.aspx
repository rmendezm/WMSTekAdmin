<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="RecordsBySerial.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Stock.Consult.RecordsBySerial" %>

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
            initializeGridDragAndDrop('SerialHeader_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }
    </script>

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="40">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <asp:GridView ID="grdMgr" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            AllowPaging="True"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>

                                                 <asp:templatefield headertext="Serial" accessibleHeaderText="SerialNumber" SortExpression="SerialNumber">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblSerialNumber" runat="server" text='<%# Eval ("SerialNumber") %>' />
                                                       </div>
                                                    </itemtemplate>
                                                </asp:templatefield>    

                                                <asp:templatefield headertext="Track" accessibleHeaderText="NameTrackSerialType" SortExpression="NameTrackSerialType">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblNameTrackSerialType" runat="server" text='<%# Eval ("NameTrackSerialType") %>' />
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

                                                 <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                            <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:templatefield headertext="Nº Doc Entrada" accessibleHeaderText="InboundNumber" SortExpression="InboundNumber">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblInboundNumber" runat="server" text='<%# Eval ("InboundOrder.Number") %>' />
                                                        </div>
                                                    </itemtemplate>
                                                </asp:templatefield>     
                                                
                                                <asp:templatefield headertext="Tipo Doc Entrada" accessibleHeaderText="InboundTypeName" SortExpression="InboundTypeName">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblInboundTypeName" runat="server" text='<%# Eval ("InboundOrder.InboundType.Name") %>' />
                                                        </div>
                                                    </itemtemplate>
                                                </asp:templatefield>  

                                                 <asp:templatefield headertext="Fecha Recepción" accessibleHeaderText="ReceiptDate" SortExpression="ReceiptDate">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblReceiptDate" runat="server" text='<%# (((DateTime)Eval ( "Stock.Receipt.ReceiptDate" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Stock.Receipt.ReceiptDate", "{0:d}" )) %>' />
                                                        </div>
                                                    </itemtemplate>
                                                </asp:templatefield>  
                        
                                                <asp:templatefield headertext="N° Doc Salida" accessibleHeaderText="OutboundNumber" SortExpression="Fifo">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblOutboundNumber" runat="server" text='<%# Eval ("OutboundOrder.Number") %>' />
                                                        </div>
                                                    </itemtemplate>
                                                </asp:templatefield>    

                                                <asp:templatefield headertext="Tipo Doc Salida" accessibleHeaderText="OutboundTypeName" SortExpression="OutboundTypeName">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblOutboundTypeName" runat="server" text='<%# Eval ("OutboundOrder.OutboundType.Name") %>' />
                                                        </div>
                                                    </itemtemplate>
                                                </asp:templatefield>  

                                                <asp:templatefield headertext="Tipo Despacho" accessibleHeaderText="DispatchTypeName" SortExpression="DispatchTypeName">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblDispatchTypeName" runat="server" text='<%# Eval ("Dispatch.TypeCode") %>' />
                                                        </div>
                                                    </itemtemplate>
                                                </asp:templatefield>  

                                                <asp:templatefield headertext="Fecha Despacho" accessibleHeaderText="TrackOutboundDate" SortExpression="TrackOutboundDate">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblTrackOutboundDate" runat="server" text='<%# (((DateTime)Eval ( "Dispatch.TrackOutboundDate" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Dispatch.TrackOutboundDate", "{0:d}" )) %>' />
                                                        </div>
                                                    </itemtemplate>
                                                </asp:templatefield>  

                                                <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ShortItemName" SortExpression="ShortItemName">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ("Item.Code") %>' />
                                                         </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>                             

                                                <asp:templatefield headertext="Descripción" accessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblItemDescription" runat="server" text='<%# Eval ("Item.Description") %>' />
                                                       </div>
                                                    </itemtemplate>
                                                </asp:templatefield>   

                                                <asp:templatefield headertext="LPN" accessibleHeaderText="lpnCode" SortExpression="lpnCode">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblLpnCode" runat="server" text='<%# Eval ("Stock.Lpn.Code") %>' />
                                                       </div>
                                                    </itemtemplate>
                                                </asp:templatefield>   
                                     
                                                <asp:templatefield headertext="Qty" accessibleHeaderText="Qty" SortExpression="Qty" Visible="false">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">                                
                                                           <asp:label ID="lblQty" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("Stock.Qty")== -1) ? " " : Eval("Stock.Qty"))%>' />
                                                       </div>
                                                    </itemtemplate>
                                                </asp:templatefield>

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
                                        <div class="col-md-5">
                                            <asp:Label ID="lblGridDetail" runat="server" Text="Serie : " />
                                            <asp:Label ID="lblSerial" runat="server" Text=""/> 
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="True" 
                                                AutoGenerateColumns="False"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false"
                                                AllowPaging="false">  

                                                <Columns>

                                                    <asp:templatefield headertext="Movimiento" accessibleHeaderText="MovementName" SortExpression="MovementName">
                                                        <itemtemplate>
                                                            <div style="word-wrap: break-word;">                                
                                                               <asp:label ID="lblMovementName" runat="server" text='<%# Eval ("MovementType.Name") %>' />
                                                           </div>
                                                        </itemtemplate>
                                                    </asp:templatefield>   

                                                    <asp:templatefield headertext="Fecha Movimiento" accessibleHeaderText="MovementDate" SortExpression="MovementDate">
                                                        <itemtemplate>
                                                            <div style="word-wrap: break-word;">                                
                                                               <asp:label ID="lblMovementDate" runat="server" text='<%# (((DateTime)Eval ( "MovementDate" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "MovementDate", "{0:dd/MM/yyyy HH:mm:ss}" )) %>' />
                                                           </div>
                                                        </itemtemplate>
                                                    </asp:templatefield>   

                                                    <asp:templatefield headertext="Usuario" accessibleHeaderText="UserName" SortExpression="UserName">
                                                        <itemtemplate>
                                                            <div style="word-wrap: break-word;">                                
                                                               <asp:label ID="lblUserName" runat="server" text='<%# Eval ("UserName") %>' />
                                                           </div>
                                                        </itemtemplate>
                                                    </asp:templatefield>  

                                                    <asp:templatefield headertext="Ubicación Origen" accessibleHeaderText="IdLocCodeSource" SortExpression="IdLocCodeSource">
                                                        <itemtemplate>
                                                            <div style="word-wrap: break-word;">                                
                                                               <asp:label ID="lblIdLocCodeSource" runat="server" text='<%# Eval ("IdLocCodeSource") %>' />
                                                           </div>
                                                        </itemtemplate>
                                                    </asp:templatefield>  

                                                    <asp:templatefield headertext="Ubicación Destino" accessibleHeaderText="IdLocCodeTarget" SortExpression="IdLocCodeTarget">
                                                        <itemtemplate>
                                                            <div style="word-wrap: break-word;">                                
                                                               <asp:label ID="lblIdLocCodeTarget" runat="server" text='<%# Eval ("IdLocCodeTarget") %>' />
                                                           </div>
                                                        </itemtemplate>
                                                    </asp:templatefield>  

                                                    <asp:templatefield headertext="LPN Origen" accessibleHeaderText="IdLpnCodeSource" SortExpression="IdLpnCodeSource">
                                                        <itemtemplate>
                                                            <div style="word-wrap: break-word;">                                
                                                               <asp:label ID="lblIdLpnCodeSource" runat="server" text='<%# Eval ("IdLpnCodeSource") %>' />
                                                           </div>
                                                        </itemtemplate>
                                                    </asp:templatefield>  

                                                    <asp:templatefield headertext="LPN Destino" accessibleHeaderText="IdLpnCodeTarget" SortExpression="IdLpnCodeTarget">
                                                        <itemtemplate>
                                                            <div style="word-wrap: break-word;">                                
                                                               <asp:label ID="lblIdLpnCodeTarget" runat="server" text='<%# Eval ("IdLpnCodeTarget") %>' />
                                                           </div>
                                                        </itemtemplate>
                                                    </asp:templatefield>  

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

    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
     <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
