<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="ProductivitySummary.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Stocks.ProductionSummary" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">

    function resizeDiv() {
        //debugger;
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("divPrincipal").style.height = h;
        document.getElementById("divPrincipal").style.width = w;
    }

    window.onresize = resizeDiv; 	
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(resizeDiv);

</script>

<%--<style type="text/css">
    fieldset
    {
	    border-color: Grey;
        font-family: Verdana, Helvetica, Sans-Serif;
    }
</style>--%>

<div id="divPrincipal" class="modalBox">
    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divGrid" runat="server" visible="true"  class="modalBox" >
                <div class="divCtrsFloatLeft">
                    <asp:Panel ID="pnlLeft" runat="server" Visible="true" Width="270" Height="600" style="left: 20px; position:relative; top: 20px" >
                        <asp:Panel ID="pnlReception" runat="server" Visible="true" HorizontalAlign="Center" GroupingText="Recepcion" Width="250" >
                            <div id="divReceptionTitle" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow"></div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblToReception" runat="server" Text="Por Recepcionar" ></asp:Label>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblReceptioned" runat="server" Text="Recepcionado" ></asp:Label>
                                    </div>
                                </center>
                            </div>
                            <div id="divReceptionDoc" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblReceptionDoc" runat="server" text="Doc"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtToReceptionDoc" runat="server" Width="60"  ReadOnly="true" style="text-align:right" ></asp:TextBox>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtReceptionedDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divReceptionLine" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblReceptionLine" runat="server" text="Línea" ></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtToReceptionLine" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtReceptionedLine" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divReceptionQty" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblReceptionQty" runat="server" text="Unid." ></asp:Label>
                                    </div>
                                
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtToReceptionQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtReceptionedQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            
                        </asp:Panel>
                        
                        <asp:Panel ID="pnlLoad" runat="server" Visible="true" HorizontalAlign="Center" GroupingText="Carga" Width="250" >
                            <div id="divLoadTitle" runat="server" class="divControls">
                                
                                    <div class="divLeftNarrow"></div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblDispatch" runat="server" Text="Anden" ></asp:Label>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblLoad" runat="server" Text="Loading" ></asp:Label>
                                    </div>
                                
                            </div>
                            <div id="divLoadDoc" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblLoadDoc" runat="server" text="Doc"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtDispatchDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtLoadDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divLoadLine" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblLoadLine" runat="server" text="Línea"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtDispatchLine" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtLoadLine" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divLoadQty" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblLoadQty" runat="server" text="Unid."></asp:Label>
                                    </div>
                                
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtDispatchQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtLoadQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divLoadLpn" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblLoadLpn" runat="server" text="Bultos"></asp:Label>
                                    </div>
                                
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtDispatchLpn" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtLoadLpn" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            
                        </asp:Panel>
                    </asp:Panel>
                </div>  
                  
                <div class="divCtrsFloatLeft">
                    <asp:Panel ID="pnlCenter" runat="server" Visible="true" Width="350" Height="600" style="left: 20px; position:relative; top: 20px">
                        <asp:Panel ID="pnlPicking" runat="server" Visible="true" HorizontalAlign="Center" GroupingText="Picking" Width="330"  >
                            <div id="divCollectTitle" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow"></div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblToCollect" runat="server" Text="Por Pickear" ></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblCollecting" runat="server" Text="Pickeando" ></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblCollected" runat="server" Text="Pickeado" ></asp:Label>
                                    </div>
                                </center>
                            </div>
                            <div id="divCollectDoc" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblCollectDoc" runat="server" text="Doc"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtToCollectDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtCollectingDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtCollectedDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divCollectLine" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblCollectLine" runat="server" text="Línea"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtToCollectLine" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtCollectingLine" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtCollectedLine" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divCollectQty" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblCollectQty" runat="server" text="Unid."></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtToCollectQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtCollectingQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtCollectedQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divCollectUser" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                    </div>
                                    <div class="fieldLeftNarrow">
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblCollectUser" runat="server" text="Usuarios Activos"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtCollectUser" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            
                        </asp:Panel>
                        
                        <asp:Panel ID="pnlTask" runat="server" Visible="true" HorizontalAlign="Center" GroupingText="Tareas" Width="330"  >
                            <div id="divTaskTitle" runat="server" class="divControls">
                                <center>
                                    <div style="margin-top: 5px; margin-right: 10px; width: 70px; float: left"></div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblTaskPending" runat="server" Text="Pendiente" ></asp:Label>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblComplete" runat="server" Text="Realizado" ></asp:Label>
                                    </div>
                                </center>
                            </div>
                            <div id="divTaskCycleCount" runat="server" class="divControls">
                                <center>
                                    <div style="margin-top: 5px; margin-right: 10px; width: 70px; float: left">
                                        <asp:Label ID="lblCycleCount" runat="server" text="C. Cíclico"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtCycleCountPending" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtCycleCountComplete" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divTaskAdjust" runat="server" class="divControls">
                                <center>
                                    <div style="margin-top: 5px; margin-right: 10px; width: 70px; float: left">
                                        <asp:Label ID="lblAdjust" runat="server" text="Ajuste"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtAdjustPending" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtAdjustComplete" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divTaskReplenish" runat="server" class="divControls">
                                <center>
                                    <div style="margin-top: 5px; margin-right: 10px; width: 70px; float: left">
                                        <asp:Label ID="lblReplenish" runat="server" text="Reposición"></asp:Label>
                                    </div>
                                
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtReplenishPending" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtReplenishComplete" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                        </asp:Panel>
                    </asp:Panel>
                </div>
                
                <div class="divCtrsFloatLeft">
                    <asp:Panel ID="Panel2" runat="server" Visible="true" Width="350" Height="600" style="left: 20px; position:relative; top: 20px" >
                        <asp:Panel ID="pnlPacking"  runat="server" Visible="true" HorizontalAlign="Center" GroupingText="Embalaje" Width="330" >
                            <div id="divPackingTitle" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow"></div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblToPack" runat="server" Text="Por Embalar" ></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblPacking" runat="server" Text="Embalando" ></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblPacked" runat="server" Text="Embalado" ></asp:Label>
                                    </div>
                                </center>
                            </div>
                            <div id="divPackingDoc" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblPackingDoc" runat="server" text="Doc"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtToPackDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtPackingDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtPackedDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divPackingLine" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblPackingLine" runat="server" text="Línea"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtToPackLine" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtPackingLine" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtPackedLine" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divPackingQty" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblPackingQty" runat="server" text="Unid."></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtToPackQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtPackingQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtPackedQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divPackingLpn" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblPackingLpn" runat="server" text="Bultos"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtToPackLpn" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtPackedLpn" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divPackingUser" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                    </div>
                                    <div class="fieldLeftNarrow">
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblPackingUser" runat="server" text="Usuarios Activos"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtPackingUser" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            
                        </asp:Panel>
                        
                        <asp:Panel ID="pnlDispatch" runat="server" Visible="true" HorizontalAlign="Center" GroupingText="Despacho" Width="330" >
                            <div id="divDispatchTitle" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow"></div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblTransfer" runat="server" Text="Traspaso" ></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblReleaseOrder" runat="server" Text="Orden Despacho" ></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:Label ID="lblTotal" runat="server" Text="Total" ></asp:Label>
                                    </div>
                                </center>
                            </div>
                            <div id="divDispatchDoc" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblDispatchDoc" runat="server" text="Pedidos"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtTransferDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtReleaseOrderDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtTotalDoc" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divDispatchQty" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblDispatchQty" runat="server" text="Unid."></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtTransferQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtReleaseOrderQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtTotalQty" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divDispatchWeight" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblDispatchWeight" runat="server" text="Kilos"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtTransferWeight" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtReleaseOrderWeight" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtTotalWeight" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                            <div id="divDispatchVolume" runat="server" class="divControls">
                                <center>
                                    <div class="divLeftNarrow">
                                        <asp:Label ID="lblDispatchVolume" runat="server" text="Volumen"></asp:Label>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtTransferVolume" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtReleaseOrderVolume" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                    <div class="fieldLeftNarrow">
                                        <asp:TextBox ID="txtTotalVolume" runat="server" Width="60" ReadOnly="true" style="text-align:right"></asp:TextBox>
                                    </div>
                                </center>
                            </div>
                        </asp:Panel>
                    </asp:Panel>
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
        </Triggers>
    </asp:UpdatePanel>
</div>    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterDate" runat="server" Text="Inicio" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Destino" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
