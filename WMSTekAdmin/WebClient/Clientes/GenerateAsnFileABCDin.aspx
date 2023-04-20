<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="GenerateAsnFileABCDin.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Clientes.GenerateAsnFileABCDin" %>

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


    function HideMessage() {
        document.getElementById("divFondoPopup").style.display = 'none';
        document.getElementById("ctl00_MainContent_divMensaje").style.display = 'none';

        return false;
    }
   
    function ShowMessage(title, message) {
     
        var position = (document.body.clientWidth - 400) / 2 + "px";
        document.getElementById("divFondoPopup").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.marginLeft = position;

        document.getElementById("ctl00_MainContent_lblDialogTitle").innerHTML = title;
        document.getElementById("ctl00_MainContent_divDialogMessage").innerHTML = message;

        return false;
    }

    function abrirASN() {
        //document.getElementById("ctl00_MainContent_divEditNew").style.display = 'none';
        $find("modalPopUpASN").show();
        return false;
    }

    function cerrarASN() {
        //document.getElementById("ctl00_MainContent_divEditNew").style.display = 'none';
        $find("BImodalPopUpASN").hide();
        return false;
    }


    function ShowProgress() {
        document.getElementById('<% Response.Write(uprEditNew.ClientID); %>').style.display = "inline";
    }

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
          initializeGridDragAndDrop('GetDispatchSpecialHeaderABCDin', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
      }

      function setDivsAfter() {
          var heightDivBottom = $("#__hsMasterDetailRD").height();
          var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetailTitle").height();
          var extraSpace = 85;

          var totalHeight = heightDivBottom - heightLabelsBottom - extraSpace;
          $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");
      }
</script>

<link href="<%= Page.ResolveClientUrl("~/WebResources/Styles/CalendarPopUp.css")%>" rel="stylesheet" type="text/css" />

<div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
    <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
        <TopPanel ID="topPanel" HeightMin="50">
            <Content>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Principal --%>
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

                                        <asp:templatefield HeaderText="Seleccionar" AccessibleHeaderText="chkSelectOutboundOrder">
                                            <HeaderTemplate>
                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectOutboundOrder', this.checked)" id="chkAll" title="Seleccionar todos" />
                                            </HeaderTemplate>
                                            <itemtemplate>
                                               <asp:CheckBox ID="chkSelectOutboundOrder" runat="server"/>
                                            </itemtemplate>
                                         </asp:templatefield>
                            
                                        <asp:templatefield HeaderText="ID Despacho" AccessibleHeaderText="IdDispacth" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                               <asp:label ID="lblIdDispacth" runat="server" text='<%# Eval ( "Id" ) %>' />
                                            </itemtemplate>
                                         </asp:templatefield>
                            
                                        <asp:templatefield HeaderText="ID Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                               <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "IdWhs" ) %>' />
                                            </itemtemplate>
                                         </asp:templatefield>
                                             
                                        <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                            <itemtemplate>
                                               <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.Name" ) %>' />
                                            </itemtemplate>
                                         </asp:templatefield>
                             
                                        <asp:TemplateField HeaderText="ID Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "OutboundOrder.Owner.Id" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "OutboundOrder.Owner.Name" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                
                                        <asp:TemplateField AccessibleHeaderText="OutboundTypeName" HeaderText="Tipo Documento">
                                            <ItemTemplate>
                                                <asp:label ID="lblOutboundTypeName" runat="server" text='<%# Eval ( "OutboundOrder.OutboundType.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>    
                                
                                        <asp:TemplateField AccessibleHeaderText="OutboundNumber" HeaderText="Nro Documento">
                                            <ItemTemplate>
                                                <asp:label ID="lblOutboundNumber" runat="server" text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField AccessibleHeaderText="ReferenceNumber" HeaderText="Orden de Compra">
                                            <ItemTemplate>
                                                <asp:Label ID="lblReferenceNumber" runat="server" Text='<%# Eval ( "OutboundOrder.ReferenceNumber" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="ReferenceDoc" HeaderText="Documento Ref.">
                                            <ItemTemplate>
                                                <asp:label ID="lblReferenceDocument" runat="server" text='<%# Eval ( "ReferenceDoc.ReferenceDocNumber" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                           
                                        <asp:TemplateField AccessibleHeaderText="CustomerCode" HeaderText="Codigo Cliente">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ( "Customer.Code" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="CustomerName" HeaderText="Cliente">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "Customer.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                         <asp:TemplateField AccessibleHeaderText="DateCreated" HeaderText="Fecha Creación">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDateCreated" runat="server" Text='<%# ((DateTime) Eval ("DateCreated") > DateTime.MinValue)? Eval("DateCreated", "{0:d}"):"" %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="UserCreated" HeaderText="User">
                                            <ItemTemplate>
                                                <asp:Label ID="lblUserCreated" runat="server" Text='<%# Eval ("UserCreated") %>' />
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
                            <%-- FIN Panel Grilla Principal --%>
                        </div>
                    </div>
                </div>
                
                <asp:UpdateProgress ID="uprSelectedOrders" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
                    <ProgressTemplate>
                        <div class="divProgress">
                            <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <webUc:UpdateProgressOverlayExtender ID="muprSelectedOrders" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprSelectedOrders" />
            
            </Content>
        </TopPanel>
        <BottomPanel HeightMin="50">
            <Content>
                
                    <%-- Panel Grilla Detalle --%>
                <asp:UpdatePanel ID="upGridDetail" runat="server"  UpdateMode="Conditional">
                    <ContentTemplate>      
                        <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                        
                          <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                                      
                            <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Doc: " />
                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>
	                        &nbsp;&nbsp
	                        
	                        <asp:Label ID="lblNroDocRef" runat="server" Text="Doc Referencia: " />
	                        <asp:Label ID="lblNroDocRefDet" runat="server" Text=""/>
                                            
                          </div>
                                 
                            <div class="container">
                                <div class="row">
                                    <div class="col-md-12">                         
                                        <div id="divGrid" runat="server" class="textLeft">
                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="False" 
                                                AutoGenerateColumns="False"
                                                OnRowCreated="grdDetail_RowCreated"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">                       
                                                <Columns>
                                                    <asp:templatefield HeaderText="ID" AccessibleHeaderText="IdDispatchDetail" ItemStyle-CssClass="text">
                                                        <itemtemplate>
                                                            <asp:label ID="lblIdDispatchDetail" runat="server" text='<%# Eval ( "Id" ) %>' />
                                                        </itemtemplate>
                                                        </asp:templatefield>
                                     
                                                    <asp:TemplateField HeaderText="Nro Linea" AccessibleHeaderText="LineNumber" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLineNumber" runat="server" text='<%# Eval ( "LineNumber" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="ID Item" AccessibleHeaderText="IdItem" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblIdItem" runat="server" text='<%# Eval ( "Item.Id" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                                                                            
                                                    <asp:TemplateField HeaderText="Codigo Item" AccessibleHeaderText="ItemCode" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName" SortExpression="LongItemName">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                                    
                                                    <asp:TemplateField HeaderText="Solicitado" AccessibleHeaderText="ItemQty">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(((decimal)Eval ( "Qty" )== -1)?"":Eval ( "Qty" )) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="Cod. Barra" AccessibleHeaderText="BarCode" SortExpression="BarCode">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblBarCode" runat="server" Text='<%# Eval ("ItemUom.BarCode") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="Precio" AccessibleHeaderText="Price">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblPrice" runat="server" text='<%# ((decimal)Eval ( "Price" )== -1)?"":Eval ( "Price" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                                                                                                                                                                                       
                                                    <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "LotNumber" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="F. Fifo" AccessibleHeaderText="Fifo" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFifo" runat="server" text='<%# (((DateTime)Eval ( "Fifo" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Fifo", "{0:d}" )) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="F. Expiración" AccessibleHeaderText="Expiration" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblExpiration" runat="server" text='<%# (((DateTime)Eval ( "Expiration" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Expiration", "{0:d}" )) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="F. Fabricación" AccessibleHeaderText="Fabrication" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFabrication" runat="server" text='<%# (((DateTime)Eval ( "Fabrication" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Fabrication", "{0:d}" )) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="Lpn" AccessibleHeaderText="Lpn" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLpn" runat="server" text='<%# Eval ( "Lpn.Code" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="Cod. Tipo Lpn" AccessibleHeaderText="LpnTypeCode" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLpnTypeCode" runat="server" text='<%# Eval ( "Lpn.LPNType.Code" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="Nom. Tipo Lpn" AccessibleHeaderText="LpnTypeName" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLpnTypeName" runat="server" text='<%# Eval ( "Lpn.LPNType.Name" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>                                                                                                
                                                    </Columns>
                        
                                            </asp:GridView>
                                        </div> 
                                    </div>
                                </div>
                             </div>
	                     </div>
                   </ContentTemplate>
                   <Triggers>
                        <%--<asp:PostBackTrigger ControlID="btnGenerateASN" /> --%>                       
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
                <%-- FIN Panel Grilla Detalle --%>
                
                <asp:UpdateProgress ID="uprSelectedOrdersDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="20" DynamicLayout="true">
                    <ProgressTemplate>
                        <div class="divProgress">
                            <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <webUc:UpdateProgressOverlayExtender ID="muprSelectedOrdersDetail" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprSelectedOrdersDetail" />
                                
            </Content>
            <Footer Height="67">
                <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
            </Footer>
        </BottomPanel>
    </spl:HorizontalSplitter>
</div>

       <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
            <ContentTemplate> 
            <%-- Pop up Editar/Nuevo Owner --%>
            <div id="divEditNew" runat="server" visible="false">    
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpASN" runat="server" 
                    TargetControlID="btnDummy" 
                    BehaviorID="BImodalPopUpASN"                                  
                    PopupControlID="pnlOwner" 
                    BackgroundCssClass="modalBackground" 
                    PopupDragHandleControlID="OwnerCaption" 
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="PnlOwner" runat="server" CssClass="modalBox" Style="display: none;">
                     
                
                    <%-- Encabezado --%>
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Generar ASN" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                        
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDateAsn" runat="server" Text="Fecha" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtDateAsn" runat="server" Width="70px" 
                                    ValidationGroup="ASN" ToolTip="Ingrese fecha." />
                                    <asp:RequiredFieldValidator ID="rfvDateAsn" runat="server" ControlToValidate="txtDateAsn"
                                            ValidationGroup="ASN" Text=" * " ErrorMessage="Fecha es requerido"  />
                                
                                    <asp:RangeValidator ID="rvDateAsn" runat="server" ControlToValidate="txtDateAsn"
                                        ErrorMessage="Fecha debe estar entre 01-01-2000 y 31-12-2040" Text=" * " MinimumValue="01/01/2000"
                                        MaximumValue="31/12/2040" ValidationGroup="ASN" Type="Date" />
                                    <ajaxToolkit:CalendarExtender ID="calDateAsn" CssClass="CalMaster" runat="server" 
                                        Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtDateAsn" PopupButtonID="txtDateAsn"
                                        Format="dd-MM-yyyy">
                                    </ajaxToolkit:CalendarExtender>
                                </div>
                            </div>
                            
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblHourAsn" runat="server" Text="Hora" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtHourAsn" runat="server" Width="50px"  MaxLength="5"
                                    ToolTip="Ingrese hora formato 24hrs." ValidationGroup="ASN" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvHourAsn" ControlToValidate="txtHourAsn" ValidationGroup="ASN"
                                    runat="server" ErrorMessage="Hora es requerido" Text=" * " />
                                    
                                    <asp:RegularExpressionValidator ID="revHourAsn" runat="server" ControlToValidate="txtHourAsn"
                                    ErrorMessage="Hora no es valida ej: 23:30" Display="Dynamic" 
                                    ValidationExpression="^[0-2]?[0-9]:[0-5][0-9]$" ValidationGroup="ASN" Text=" * ">
                                   </asp:RegularExpressionValidator>
                                   
                                    <%--<ajaxToolkit:MaskedEditExtender ID="meeHourAsn" runat="server"
                                    Mask="99:99" 
                                    AutoComplete="false" 
                                    MaskType="Time"                                   
                                    ClearMaskOnLostFocus="true" 
                                    UserTimeFormat="TwentyFourHour" 
                                    TargetControlID="txtHourAsn"    /> --%>
                                </div>
                            </div>  
                            <div id="divWarehouse" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWarehouseASN" runat="server" Text="Almacén" />
                                </div>
                                <div class="fieldLeft">
                                <asp:TextBox SkinID="txtFilter" ID="txtWarehouseASN" runat="server" Width="50px"  MaxLength="10"
                                    ToolTip="Ingrese Almacén." ValidationGroup="ASN" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvWarehouseASN" ControlToValidate="txtWarehouseASN" ValidationGroup="ASN"
                                    runat="server" ErrorMessage="Almacén es requerido" Text=" * " />
                                </div>                        
                            </div>
                            
                        </div>    
                        <div class="divValidationSummary" >                            
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="ASN" 
                            CssClass="modalValidation" ShowMessageBox="false" />
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnGenerateASN" runat="server" Text="Aceptar" OnClick="btnGenerateASN_Click"
                            OnClientClick="ShowProgress();" CausesValidation="true" ValidationGroup="ASN" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar"  OnClick="btnCerrar_Click"  />
                        </div>
                    </div>    
                    
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo Owner --%>
        </ContentTemplate>
           <Triggers>
                <asp:PostBackTrigger ControlID="btnGenerateASN" />   
          </Triggers>
        </asp:UpdatePanel>  

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    
    <div id="divFondoPopup" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;">
    </div>
    <div id="divMensaje" class="modalBox" style="z-index: 400001; display: none; position: absolute; 
        width: 400px;  top: 200px; margin-top: 0;"  runat="server">
        
        <div id="divDialogTitleMessage" runat="server" class="modalHeader">
			<div class="divCaption">
			    <asp:Label ID="lblDialogTitle" runat="server" />
            </div>
	    </div>
	    <div id="divPanelMessage" class="divDialogPanel" runat="server">
        
            <div class="divDialogMessage">
                <asp:Image id="Image1" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
            </div>
            <div id="divDialogMessage" runat="server" class="divDialogMessage" />        
            <div id="divAlert" runat="server" visible="true" class="divDialogButtons">
                <asp:Button ID="btnMessageInfo" runat="server" Text="Aceptar"  OnClientClick="return HideMessage();" />
            </div>    
        </div>
               
    </div>     
    

    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label id="lblFilterDate" runat="server" Text="Creación" Visible="false" />    
    <asp:Label ID="lblDescription" runat="server" Text="Nombre Cliente" Visible="false" />
    <asp:Label ID="lblName" runat="server" Text="Cod. Cliente" Visible="false" />   
    <asp:Label ID="lblReferenceDoc" runat="server" Text="Doc. Referencia" Visible="false" /> 
    <asp:Label ID="lblReferenceNumbDoc" runat="server" Text="Orden Compra" Visible="false" /> 
    <asp:Label ID="lblTitle" runat="server" Text="Generación Archivos ASN" Visible="false"/> 
    <asp:Label ID="lblErrorDetail" runat="server" Text="No existe detalle para crear el archivo." Visible="false"/>	
    <asp:Label ID="lblErrorTemplate" runat="server" Text="El cliente no posee un template para generar el archivo." Visible="false"/>
    <asp:Label ID="lblErrorExistTaskOrder" runat="server" Text="No se puede generar ASN por que existen tareas pendientes." Visible="false"/>		
    <asp:Label ID="lblErrorNotItemUom" runat="server" Text="Para el ítem [ITEM], NO existe una presentación de UNIDAD creada." Visible="false"/>
    <asp:Label ID="lblErrorMoreThanOneItemUom" runat="server" Text="Para el ítem [ITEM], existe más de una presentación de UNIDAD creada." Visible="false"/>
    <asp:Label id="lblErrorCustomerNotFound" runat="server" Text="Cliente no encontrado en el sistema" Visible="false" /> 
    <asp:Label id="lblSelectedOutboundOrderWithDifferentReferenceDocNumber" runat="server" Text="Se seleccionaron documentos de salida de mas de una orden de compra" Visible="false" /> 
    <asp:Label id="lblValidateAtLeastOneOutboundOrderSelected" runat="server" Text="Debe seleccionar al menos un documento de salida" Visible="false" /> 
    <asp:Label id="lblDoesntExistItemCustomerMatch" runat="server" Text="No existe registro en Item Customer en despacho con ID " Visible="false" />  
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
     <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
