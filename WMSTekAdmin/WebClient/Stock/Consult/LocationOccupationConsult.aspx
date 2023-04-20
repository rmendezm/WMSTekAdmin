<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LocationOccupationConsult.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Stocks.LocationOccupationConsult" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("GetLocationOccupation", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("GetLocationOccupation", "ctl00_MainContent_grdMgr");
        }
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>  
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
	                         <%-- Grilla Principal --%>         
                             <asp:GridView ID="grdMgr" 
                                runat="server" 
                                AllowPaging="True" 
                                AllowSorting="False" 
                                OnRowCreated="grdMgr_RowCreated"
                                EnableViewState="false"
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                    
                                 <Columns>
                                    <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WhsCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                           <asp:label ID="lblWhsCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                        </itemtemplate>
                                     </asp:templatefield>
                                         
                                    <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="WhsName">
                                        <itemtemplate>
                                           <asp:label ID="lblWhsName" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                    <asp:templatefield HeaderText="Cod. Dueño" AccessibleHeaderText="OwnCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                           <asp:label ID="lblOwnCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>' />
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                    <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <itemtemplate>
                                           <asp:label ID="lblOwnName" runat="server" text='<%# Eval ( "Owner.TradeName" ) %>' />
                                        </itemtemplate>
                                     </asp:templatefield>                         

                                    <asp:templatefield headertext="Ubicación" accessibleHeaderText="LocCode" SortExpression="LocationCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblLocCode" runat="server" text='<%# Eval ("IdCode") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <asp:templatefield headertext="Tipo Ubic." accessibleHeaderText="LocTypeName" SortExpression="LocationType">
                                        <itemtemplate>
                                               <asp:label ID="lblLocTypeName" runat="server" text='<%# Eval ("Type.LocTypeName") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                   <asp:templatefield headertext="% Ocup.Peso" accessibleHeaderText="WeightOccupationPercent" SortExpression="WeightOccupationPercent" ItemStyle-HorizontalAlign="Right">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblWeightOccupationPercent" runat="server" text='<%# (Eval ("WeightOccupationPercent"))%>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                   <asp:templatefield headertext="% Ocup.Volumen" accessibleHeaderText="VolumeOccupationPercent" SortExpression="VolumeOccupationPercent" ItemStyle-HorizontalAlign="Right">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblVolumeOccupationPercent" runat="server" text='<%# (Eval ("VolumeOccupationPercent"))%>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                   <asp:templatefield headertext="% Ocup.Cantidad" accessibleHeaderText="StockOccupationPercent" SortExpression="StockOccupationPercent" ItemStyle-HorizontalAlign="Right">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblStockOccupationPercent" runat="server" text='<%# (Eval ("StockOccupationPercent"))%>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>                                                
                        
                                     <asp:templatefield headertext="Stock Máximo(Un)" accessibleHeaderText="CapacityUnit" SortExpression="CapacityUnit" ItemStyle-HorizontalAlign="Right">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblCapacityUnit" runat="server" text='<%# GetFormatedNumber((Eval ("CapacityUnit"))) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                     <asp:templatefield headertext="Stock Actual" accessibleHeaderText="Stock" SortExpression="Stock" ItemStyle-HorizontalAlign="Right">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblStock" runat="server" text='<%# GetFormatedNumber(Eval ("Stock")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>                           
                        
                                   <asp:templatefield headertext="Stock Promedio" accessibleHeaderText="AverageStock" SortExpression="AverageStock" ItemStyle-HorizontalAlign="Right">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblAverageStock" runat="server" text='<%# GetFormatedNumber(Eval ("AverageStock"))%>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        

                                 </Columns>
                         
                              </asp:GridView>
                            <%-- FIN Grilla Principal --%>             
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
                  </Triggers>
                </asp:UpdatePanel>  
            </div>
        </div>
    </div>

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%>                   

    <%-- Mensajes de Confirmacion y Auxiliares --%>     
    <asp:Label ID="lblFilterCode" runat="server" Text="Ubicación" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>