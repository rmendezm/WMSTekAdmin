<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockItemConsult.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Stocks.StockItemConsultPage" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);
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
                                OnRowCreated="grdMgr_RowCreated"
                                EnableViewState="False"
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                    
                                 <Columns>
                                     <asp:templatefield HeaderText="Id. Centro Distr." AccessibleHeaderText="Idwhs" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                                               
                                                <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Idwhs" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Cod DC" AccessibleHeaderText="WhsCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                                                
                                                <asp:label ID="lblWhsCode" runat="server" text='<%# Eval ( "WhsCode" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Centro Distr." AccessibleHeaderText="ShortWhsName" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                            
                                                <asp:label ID="lblShortWhsName" runat="server" text='<%# Eval ( "ShortWhsName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Centro Distr." AccessibleHeaderText="WhsName" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblWhsName" runat="server" text='<%# Eval ( "WhsName" ) %>' />
                                           </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Id. Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblIdOwn" runat="server" text='<%# Eval ( "IdOwn" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Cod. Dueño" AccessibleHeaderText="OwnCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblOwnCodee" runat="server" text='<%# Eval ( "OwnCode" ) %>' />
                                             </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="OwnName" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                           <div style="word-wrap: break-word;">
                                                <asp:label ID="lblOwnName" runat="server" text='<%# Eval ( "OwnName" ) %>' />
                                           </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Nombre Comercial" AccessibleHeaderText="TradeName" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblTradeName" runat="server" text='<%# Eval ( "TradeName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <%--<asp:templatefield HeaderText="FIFO" AccessibleHeaderText="FifoDate" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblFifoDate" runat="server" text='<%# Eval ( "FifoDate" ) %>' />
                                           </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblLotNumber" runat="server" text='<%# Eval ( "LotNumber" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Elaboración" AccessibleHeaderText="FabricationDate" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblFabricationDate" runat="server" text='<%# Eval ( "FabricationDate" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblExpirationDate" runat="server" text='<%# Eval ( "ExpirationDate" ) %>' />
                                           </div>
                                        </itemtemplate>
                                     </asp:templatefield>                         
                       
                                     <asp:templatefield HeaderText="Id. Ctg Item" AccessibleHeaderText="IdCtgItem" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblIdCtgItem" runat="server" text='<%# Eval ( "IdCtgItem" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Cod. Categoría" AccessibleHeaderText="CtgCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblCtgCode" runat="server" text='<%# Eval ( "CtgCode" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Categoría" AccessibleHeaderText="CtgName" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblCtgName" runat="server" text='<%# Eval ( "CtgName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>--%>
                         
                                     <asp:templatefield HeaderText="Id. Item" AccessibleHeaderText="IdItem" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                           <div style="word-wrap: break-word;">
                                                <asp:label ID="lblIdItem" runat="server" text='<%# Eval ( "IdItem" ) %>' />
                                           </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Cod. Item" AccessibleHeaderText="ItemCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblItemCode" runat="server" text='<%# Eval ( "ItemCode" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Nombre" AccessibleHeaderText="LongItemName" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblLongItemName" runat="server" text='<%# Eval ( "LongItemName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                     <asp:templatefield HeaderText="Cant." AccessibleHeaderText="ItemQty" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(Eval ( "ItemQty" )) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                 </Columns>
                         
                              </asp:GridView>
                            <%-- FIN Grilla Principal --%>             
                        </div>
                        <div id="divReport" runat="server" visible="false">
                        <rsweb:ReportViewer ID="rptViewKardex" runat="server" ProcessingMode="Remote"
                            AsyncRendering="true" Height="610px" Width="100%" Style="margin-right: 11px">
                            <ServerReport />
                        </rsweb:ReportViewer>
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
    <asp:Label id="lblFilterDate" runat="server" Text="Fifo" Visible="false" />   
    <asp:Label id="LabelmessageFind" runat="server" Text="Debe ingresar codigo de item" Visible="false" />   
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>
</asp:Content>
