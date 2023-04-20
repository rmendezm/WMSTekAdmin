<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="OutboundOrderMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Administration.OutboundOrderMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
<script src="<%= Page.ResolveClientUrl("~/WebResources/Javascript/UtilMassive.js")%>" type="text/javascript"></script>
<link href="<%= Page.ResolveClientUrl("~/WebResources/Styles/CalendarPopUp.css")%>" rel="stylesheet" type="text/css" />

<script type="text/javascript" language="javascript">

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);
    window.onresize = SetDivs;

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(onEndRequest);

    function onEndRequest(sender, args) {
        var error = args.get_error();
        if (error != null) {
            // Set the error handled flag to avoid a runtime error
            // reaching the user.
            args.set_errorHandled(true);
            // Remove the error name from the message
            var msg = error.message.replace("Sys.WebForms.PageRequestManagerServerErrorException: ", "");
            // alert(msg);
            pErrorDetail.innerHTML = msg;
            $find("mpeError").show();
            var background = $find("mpeError")._backgroundElement;
            background.style.zIndex = "20000";


            var errorArea = $get("ErrorArea");
            errorArea.style.zIndex = "20001";
        }
    }

    function showProgress() {
        if (document.getElementById('ctl00_MainContent_uploadFile').value.length > 0) {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modalLoading");
                $('body').append(modal);
                var loading = $(".loading");
               
                var top = Math.max($(window).height() / 3.5, 0);
                var left = Math.max($(window).width() / 2.6, 0);
                loading.css({ top: top, left: left });
                loading.show();
            }, 30);
            return true;

        } else {
            return false;
        }
    }

    $(document).ready(function () {
        initializeGridWithNoDragAndDrop(true);

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
        initializeGridWithNoDragAndDrop(true);
    }
       
</script>

    <style>
        .divGrid{
            overflow: visible !important;
        }

        .divItemDetails{
            max-height: 90px;
            overflow: auto;
        }
    </style>

      <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Grilla Principal --%>
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">
                
                            <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id" OnRowCommand="grdMgr_RowCommand"
                                OnRowDeleting="grdMgr_RowDeleting" OnRowDataBound="grdMgr_RowDataBound" AllowPaging="True"
                                EnableViewState="False" OnRowCreated="grdMgr_RowCreated" OnRowEditing="grdMgr_RowEditing" 
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id"
                                        SortExpression="Id">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                             </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cód. CD. Destino" AccessibleHeaderText="WarehouseTargetCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWarehouseTargeteCode" runat="server" Text='<%# Eval ( "WarehouseTarget.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Centro Dist. Destino" AccessibleHeaderText="WarehouseTarget">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWarehouseTarget" runat="server" Text='<%# Eval ( "WarehouseTarget.ShortName" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwner" runat="server" Text='<%# Eval ( "Owner.TradeName" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Nº Doc." ItemStyle-Wrap="false" DataField="Number" AccessibleHeaderText="OutboundNumber" />
                                    <asp:TemplateField AccessibleHeaderText="OutboundTypeCode" HeaderText="Tipo">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOutboundType" runat="server" Text='<%# Eval ( "OutboundType.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField AccessibleHeaderText="OutboundTypeName" HeaderText="Tipo Doc.">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ("OutboundType.Name") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status" SortExpression="Status">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# Eval ( "Status" ) %>'
                                                    Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ReferenceNumber" HeaderText="Nº Ref." AccessibleHeaderText="ReferenceNumber">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LoadCode" HeaderText="Cód. Carga" ItemStyle-Wrap="false"
                                        AccessibleHeaderText="LoadCode" />
                                    <asp:BoundField DataField="LoadSeq" HeaderText="Sec. Carga" AccessibleHeaderText="LoadSeq"
                                        ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Priority" HeaderText="Prioridad" AccessibleHeaderText="Priority"
                                        ItemStyle-Wrap="false" />
                                    <asp:TemplateField HeaderText="Liberación Autom." AccessibleHeaderText="InmediateProcess">
                                        <ItemStyle />
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkInmediateProcess" runat="server" Checked='<%# Eval ( "InmediateProcess" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Esperada" AccessibleHeaderText="ExpectedDate" SortExpression="ExpectedDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblExpectedDate" runat="server" Text='<%# ((DateTime) Eval ("ExpectedDate") > DateTime.MinValue)? Eval("ExpectedDate", "{0:d}"):"" %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Emisión" AccessibleHeaderText="EmissionDate" SortExpression="EmissionDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblEmissionDate" runat="server" Text='<%# ((DateTime) Eval ("EmissionDate") > DateTime.MinValue)? Eval("EmissionDate", "{0:d}"):"" %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Salida" AccessibleHeaderText="ShipmentDate" SortExpression="ShipmentDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblShipmentDate" runat="server" Text='<%# ((DateTime) Eval ("ShipmentDate") > DateTime.MinValue)? Eval("ShipmentDate", "{0:d}"):"" %>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate"
                                        SortExpression="ExpirationDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cancelación" AccessibleHeaderText="CancelDate" SortExpression="CancelDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCancelDate" runat="server" Text='<%# ((DateTime) Eval ("CancelDate") > DateTime.MinValue)? Eval("CancelDate", "{0:d}"):"" %>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CancelUser" HeaderText="Usuario Cancelación" AccessibleHeaderText="CancelUser" />
                                    <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="OutboundTrack" SortExpression="OutboundTrack">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOutboundTrack" runat="server" Text='<%# Eval ( "LatestOutboundTrack.Type.Name" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerCode" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ( "CustomerCode" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerName"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "CustomerName" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DeliveryAddress1" HeaderText="Dir. Entrega" AccessibleHeaderText="DeliveryAddress1" />
                                    <asp:BoundField DataField="DeliveryAddress2" HeaderText="Dir. Entrega Opc." AccessibleHeaderText="DeliveryAddress2" />
                                    <asp:TemplateField HeaderText="País Entrega" AccessibleHeaderText="CountryDelivery"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCountryDeliveryName" runat="server" Text='<%# Eval ( "CountryDelivery.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Región Entrega" AccessibleHeaderText="StateDelivery"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblStateDeliveryName" runat="server" Text='<%# Eval ( "StateDelivery.Name" ) %>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ciudad Entrega" AccessibleHeaderText="CityDelivery"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCityDeliveryName" runat="server" Text='<%# Eval ( "CityDelivery.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DeliveryPhone" HeaderText="Tel. Entrega" AccessibleHeaderText="DeliveryPhone" />
                                    <asp:BoundField DataField="DeliveryEmail" HeaderText="E-mail Entrega" AccessibleHeaderText="DeliveryEmail" />
                                    <asp:TemplateField HeaderText="Preparar Completo" AccessibleHeaderText="FullShipment"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkFullShipment" runat="server" Checked='<%# Eval ( "FullShipment" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Transportista" AccessibleHeaderText="CarrierCode"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCarrierCode" runat="server" Text='<%# Eval ( "Carrier.Code" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="RouteCode" HeaderText="Ruta" AccessibleHeaderText="RouteCode" />
                                    <asp:BoundField DataField="Plate" HeaderText="Patente" AccessibleHeaderText="Plate" />
                                    <asp:BoundField DataField="Invoice" HeaderText="Nº Factura" AccessibleHeaderText="Invoice" />
                                    <asp:BoundField DataField="FactAddress1" HeaderText="Dirección Factura" AccessibleHeaderText="FactAddress1" />
                                    <asp:BoundField DataField="FactAddress2" AccessibleHeaderText="FactAddress2" HeaderText="Dirección Factura Opc." />
                                    <asp:TemplateField HeaderText="País Factura" AccessibleHeaderText="CountryFact" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCountryFactName" runat="server" Text='<%# Eval ( "CountryFact.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Región Factura" AccessibleHeaderText="StateFact" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblStateFactName" runat="server" Text='<%# Eval ( "StateFact.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ciudad Factura" AccessibleHeaderText="CityFact" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCityFactName" runat="server" Text='<%# Eval ( "CityFact.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="FactPhone" HeaderText="Tel. Factura" AccessibleHeaderText="FactPhone" />
                                    <asp:BoundField DataField="FactEmail" HeaderText="E-mail Factura" AccessibleHeaderText="FactEmail" />

                                    <%-- CUSTOMER --%>
                                    <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerCustomerName" Visible ="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerCustomerName" runat="server" Text='<%# Eval ( "Customer.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
			                        <asp:TemplateField HeaderText="Dir. Entrega" AccessibleHeaderText="CustomerDeliveryAddress1" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerDeliveryAddress1" runat="server" Text='<%# Eval ( "Customer.Address1Delv" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

			                        <asp:TemplateField HeaderText="Dir. Entrega Opc." AccessibleHeaderText="CustomerDeliveryAddress2" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerDeliveryAddress2" runat="server" Text='<%# Eval ( "Customer.Address2Delv" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="País Entrega" AccessibleHeaderText="CustomerCountryDelivery" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerCountryDeliveryName" runat="server" Text='<%# Eval ( "Customer.CountryDelv.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Región Entrega" AccessibleHeaderText="CustomerStateDelivery" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerStateDeliveryName" runat="server" Text='<%# Eval ( "Customer.StateDelv.Name" ) %>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ciudad Entrega" AccessibleHeaderText="CustomerCityDelivery" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerCityDeliveryName" runat="server" Text='<%# Eval ( "Customer.CityDelv.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tel. Entrega" AccessibleHeaderText="CustomerDeliveryPhone" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerDeliveryPhone" runat="server" Text='<%# Eval ( "Customer.PhoneDelv" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
			                        <asp:TemplateField HeaderText="Fax. Entrega" AccessibleHeaderText="CustomerDeliveryFax" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerDeliveryFax" runat="server" Text='<%# Eval ( "Customer.FaxDelv" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

			                        <asp:TemplateField HeaderText="Dir. Factura" AccessibleHeaderText="CustomerFactAddress1" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerFactAddress1" runat="server" Text='<%# Eval ( "Customer.Address1Fact" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

			                        <asp:TemplateField HeaderText="Dir. Factura Opc." AccessibleHeaderText="CustomerFactAddress2" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerFactAddress2" runat="server" Text='<%# Eval ( "Customer.Address2Fact" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="País Factura" AccessibleHeaderText="CustomerCountryFact" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerCountryFactName" runat="server" Text='<%# Eval ( "Customer.CountryFact.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Región Factura" AccessibleHeaderText="CustomerStateFact" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerStateFactName" runat="server" Text='<%# Eval ( "Customer.StateFact.Name" ) %>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ciudad Factura" AccessibleHeaderText="CustomerCityFact" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerCityFactName" runat="server" Text='<%# Eval ( "Customer.CityFact.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tel. Factura" AccessibleHeaderText="CustomerFactPhone" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerFactPhone" runat="server" Text='<%# Eval ( "Customer.PhoneFact" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
			                        <asp:TemplateField HeaderText="Fax. Factura" AccessibleHeaderText="CustomerFactFax" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerFactFax" runat="server" Text='<%# Eval ( "Customer.FaxFact" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%-- FIN CUSTOMER --%>

                                    <%-- BRANCH --%>
                                    <asp:TemplateField HeaderText="Nombre Sucursal" AccessibleHeaderText="BranchName"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblBranchName" runat="server" Text='<%# Eval ( "Branch.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
			                        <asp:TemplateField HeaderText="Dir. Sucursal" AccessibleHeaderText="BranchAddress" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblBranchAddress" runat="server" Text='<%# Eval ( "Branch.BranchAddress" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="País Sucursal" AccessibleHeaderText="BranchCountry" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblBranchCountryName" runat="server" Text='<%# Eval ( "Branch.Country.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Región Sucursal" AccessibleHeaderText="BranchState" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblBranchStateName" runat="server" Text='<%# Eval ( "Branch.State.Name" ) %>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ciudad Sucursal" AccessibleHeaderText="BranchCity" Visible="false"
                                        ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblBranchCityName" runat="server" Text='<%# Eval ( "Branch.City.Name" ) %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%-- FIN BRANCH --%>

                                    <asp:TemplateField HeaderText="% Satisfacción" AccessibleHeaderText="Compliance" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCompliance" runat="server" Text='<%# ((int) Eval("OutboundOrderCompliance.Percentage") > -1) ? Eval("OutboundOrderCompliance.Percentage") : "" %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Fecha Simulación" AccessibleHeaderText="SimulationDate" SortExpression="SimulationDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblSimulationDateDate" runat="server" Text='<%# ((DateTime) Eval ("OutboundOrderCompliance.SimulationDate") > DateTime.MinValue) ? Eval("OutboundOrderCompliance.SimulationDate", "{0:d}") : "" %>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="Comment" HeaderText="Comentarios" AccessibleHeaderText="Comment" SortExpression="Comment" HeaderStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>

                                    <asp:BoundField DataField="SpecialField1" HeaderText="Campo. Esp. 1" AccessibleHeaderText="SpecialField1"
                                        SortExpression="SpecialField1" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SpecialField2" HeaderText="Campo. Esp. 2" AccessibleHeaderText="SpecialField2"
                                        SortExpression="SpecialField2" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SpecialField3" HeaderText="Campo. Esp. 3" AccessibleHeaderText="SpecialField3"
                                        SortExpression="SpecialField3" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SpecialField4" HeaderText="Campo. Esp. 4" AccessibleHeaderText="SpecialField4"
                                        SortExpression="SpecialField4" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:TemplateField ShowHeader="False" HeaderText="Acciones" HeaderStyle-HorizontalAlign="Center"
                                        AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 160px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CausesValidation="false" CommandName="Delete" />
                                                    <asp:ImageButton ID="btnClose" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_close.png"
                                                        CausesValidation="false" CommandName="CloseOrder" />
                                                    <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_cancel.png"
                                                        CausesValidation="false" CommandName="CancelOrder" ToolTip="Anular" CommandArgument="<%# Container.DataItemIndex %>" />
                                                    <asp:ImageButton ID="btnPending" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_tick_on.png" 
                                                        CausesValidation="false" CommandName="PendingOrder" ToolTip="Habilitar" CommandArgument="<%# Container.DataItemIndex %>" />
                                                    <asp:ImageButton ID="btnChangeTrack" runat="server" ToolTip="Cambiar Track" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_process.png"
                                                        CausesValidation="false" CommandName="ChangeTrack" CommandArgument="<%# Container.DataItemIndex %>" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        
                            <br />
                        </div>
                    </div>
                </div>
            </div>
            <%-- FIN Grilla Principal --%>
            <%-- Panel Nuevo/Editar Documento --%>
            <div id="divModal" runat="server" visible="false">
                <asp:Panel ID="pnlPanelPoUp" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="OutboundCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Documento" Width="770px" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Documento" Width="770px" />
                            <asp:ImageButton ID="btnClose" runat="server" OnClick="imgCloseNewEdit_Click" ToolTip="Cerrar" CssClass="closeButton"
                                ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                    <asp:HiddenField ID="hidLatestOutboundTrackId" runat="server" Value="-1" />
                    <div class="modalBoxContent" style="max-height:480px; overflow:auto;">
                        <ajaxToolkit:TabContainer runat="server" ID="tabOutbound" ActiveTabIndex="0">
                            <ajaxToolkit:TabPanel runat="server" ID="tabLayout">
                                <ContentTemplate>
                                    <asp:HiddenField runat="server" ID="hidIdBranch" />
                                    <div id="Central" class="divCtrsFloatLeft">
                                        <div id="divWarehouse" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWarehouse" runat="server" Text="Centro" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlWarehouse" runat="server" Width="150px" TabIndex="7" AutoPostBack="True" OnSelectedIndexChanged="ddlWarehouse_SelectedIndexChanged" />
                                                <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlWarehouse" Display="Dynamic" InitialValue="-1"
                                                    ErrorMessage="Centro es requerido" />
                                            </div>
                                        </div>
                                        <div id="divOwner" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOwner" runat="server" Text="Dueño" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlOwner" runat="server" Width="120px" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddlOwner_Changed" />
                                                <asp:RequiredFieldValidator ID="rfvIdOwner" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlOwner" Display="Dynamic" InitialValue="-1" ErrorMessage="Dueño es requerido" /></div>
                                        </div>
                                        <div id="divIdOutboundType" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblIdOutboundType" runat="server" Text="Tipo" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlIdOutboundType" runat="server" />
                                                <asp:RequiredFieldValidator ID="rfvIdOutboundType2" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlIdOutboundType" Display="Dynamic" InitialValue="-1"
                                                    ErrorMessage="Tipo es requerido" /></div>
                                        </div>
                                        <div id="divNumOutboundOrder" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblNumOutboundOrder" runat="server" Text="Nº Doc." /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtNumOutboundOrder" runat="server" Width="120px" MaxLength="30" />
                                                <asp:RequiredFieldValidator ID="rfvNumOutboundOrder" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="txtNumOutboundOrder" ErrorMessage="Nº Doc. es requerido"
                                                    BorderStyle="None" />
                                            </div>
                                        </div>
                                        <div id="divReference" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblReference" runat="server" Text="Nº Ref." /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtReference" runat="server" Width="120px" MaxLength="30"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvReference" ControlToValidate="txtReference" ValidationGroup="EditNew"
                                                    runat="server" Text=" * " ErrorMessage="Nº Ref. es requerido"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div id="divNameTrackOutbound" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblNameTrackOutbound" runat="server" Text="Traza" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtOutboundTrack" runat="server" Enabled="False" /></div>
                                        </div>
                                        <div id="divStatus" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="Status" Text="Activo" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="False"
                                                    TabIndex="10" /></div>
                                        </div>
                                        <div id="divInmediateProcess" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblInmediateProcess" runat="server" Text="Liberación Autom." /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkInmediateProcess" runat="server" Checked='<%# Eval ( "InmediateProcess" ) %>'
                                                    Enabled="False" TabIndex="10" /></div>
                                        </div>
                                    </div>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divFullShipment" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFullShipment" Text="Preparar Completo" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkFullShipment" runat="server" Checked='<%# Eval ( "FullShipment" ) %>'
                                                    TabIndex="10" />
                                            </div>
                                        </div>
                                        <div id="divLoadCode" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLoadCode" runat="server" Text="Cód. Carga" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtLoadCode" runat="server" Width="100px" MaxLength="30"></asp:TextBox>
                                            </div>
                                            <asp:RequiredFieldValidator ID="rfvLoadCode" ControlToValidate="txtLoadCode" ValidationGroup="EditNew"
                                                runat="server" ErrorMessage="Cód. Carga es requerido" Text=" * " />
                                            <asp:RegularExpressionValidator ID="revLoadCode" runat="server" ControlToValidate="txtLoadCode"
                                                ErrorMessage="En el campo Codigo de carga debe ingresar solo letras de la A - Z o a - z o números"
                                                ValidationExpression="[a-zA-Z 0-99999999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ.]*"
                                                ValidationGroup="EditNew" Text=" * "></asp:RegularExpressionValidator>
                                        </div>
                                        <div id="divLoadSeq" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLoadSeq" runat="server" Text="Sec. Carga" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtLoadSeq" runat="server" MaxLength="20"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvLoadSeq" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="txtLoadSeq" Display="Dynamic" ErrorMessage="Secuencia de carga es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtLoadSeq" runat="server" ControlToValidate="txtLoadSeq"
                                                    ErrorMessage="En el campo Sec. Carga debe ingresar solo letras de la A - Z o a - z o números"
                                                    ValidationExpression="[a-zA-Z 0-99999999999.]*" ValidationGroup="EditNew" Text=" * "></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div id="divPriority" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPriority" runat="server" Text="Prioridad" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPriority" runat="server" MaxLength="2" Width="20px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvPriority" runat="server" ControlToValidate="txtPriority"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad es requerido" /></div>
                                            <asp:RangeValidator ID="rvPriority" runat="server" ControlToValidate="txtPriority"
                                                ErrorMessage="Prioridad debe estar entre 0 y 10" Text=" * " MinimumValue="0"
                                                MaximumValue="10" ValidationGroup="EditNew" Type="Integer" />
                                        </div>
                                        <div id="divEmissionDate" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblEmissionDate" runat="server" Text="Emisión" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtEmissionDate" runat="server" Width="80px" Enabled="False" MaxLength="10"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvEmissionDate" runat="server" ControlToValidate="txtEmissionDate"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Emisión es requerido" /></div>
                                            <asp:RangeValidator ID="rvEmissionDate" runat="server" ControlToValidate="txtEmissionDate"
                                                ErrorMessage="Emisión debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MinimumValue="01/01/2000"
                                                MaximumValue="31/12/2040" ValidationGroup="EditNew" Type="Date" />
                                            <ajaxToolkit:CalendarExtender ID="calEmissionDate" CssClass="CalMaster" runat="server"
                                                Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtEmissionDate" PopupButtonID="txtEmissionDate"
                                                Format="dd-MM-yyyy">
                                            </ajaxToolkit:CalendarExtender>
                                        </div>
                                        <div id="divExpectedDate" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblExpectedDate" runat="server" Text="Esperada" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtExpectedDate" runat="server" Width="80px" MaxLength="10" Enabled="False"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvExpectedDate1" runat="server" ControlToValidate="txtExpectedDate"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Esperada es requerido" /></div>
                                            <asp:RangeValidator ID="rvExpectedDate" runat="server" ControlToValidate="txtExpectedDate"
                                                ErrorMessage="Esperada debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MinimumValue="01/01/2000"
                                                MaximumValue="31/12/2040" ValidationGroup="EditNew" Type="Date" />
                                            <ajaxToolkit:CalendarExtender ID="calExpectedDate" CssClass="CalMaster" runat="server"
                                                Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtExpectedDate" PopupButtonID="txtExpectedDate">
                                            </ajaxToolkit:CalendarExtender>
                                        </div>
                                        <div id="divShipmentDate" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblShipmentDate" runat="server" Text="Salida" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtShipmentDate" runat="server" Width="80px" TabIndex="8" MaxLength="10"
                                                    Enabled="False"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvShipmentDate" runat="server" ControlToValidate="txtShipmentDate"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Salida es requerido" /></div>
                                            <asp:RangeValidator ID="rvShipmentDate" runat="server" ControlToValidate="txtShipmentDate"
                                                ErrorMessage="Salida debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MinimumValue="01/01/2000"
                                                MaximumValue="31/12/2040" ValidationGroup="EditNew" Type="Date" />
                                            <ajaxToolkit:CalendarExtender ID="calShipmentDate" CssClass="CalMaster" runat="server"
                                                Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtShipmentDate" PopupButtonID="txtShipmentDate">
                                            </ajaxToolkit:CalendarExtender>
                                        </div>
                                    </div>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divExpirationDate" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblExpirationDate" runat="server" Text="Vencimiento" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtExpirationDate" runat="server" Width="80px" TabIndex="8" MaxLength="10"
                                                    Enabled="False" />
                                                <asp:RequiredFieldValidator ID="rfvExpirationDate" runat="server" ControlToValidate="txtExpirationDate"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Vencimiento es requerido" /></div>
                                            <asp:RangeValidator ID="rvExpirationDate" runat="server" ControlToValidate="txtExpirationDate"
                                                ErrorMessage="Vencimiento debe estar entre 01/01/2000 y 31/12/2040" Text=" * "
                                                MinimumValue="01/01/2000" MaximumValue="31/12/2040" ValidationGroup="EditNew"
                                                Type="Date" />
                                            <ajaxToolkit:CalendarExtender ID="calExpirationDate" CssClass="CalMaster" runat="server"
                                                Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtExpirationDate" PopupButtonID="txtExpirationDate">
                                            </ajaxToolkit:CalendarExtender>
                                        </div>
                                        <div id="divCancelDate" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCancelDate" runat="server" Text="Cancelación" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtCancelDate" runat="server" Width="80px" TabIndex="8" MaxLength="10"
                                                    Enabled="False" />
                                                <asp:RequiredFieldValidator ID="rfvCancelDate" runat="server" ControlToValidate="txtCancelDate"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Cancelación es requerido" /></div>
                                            <asp:RangeValidator ID="rvCancelDate" runat="server" ControlToValidate="txtCancelDate"
                                                ErrorMessage="Cancelación debe estar entre 01/01/2000 y 31/12/2040" Text=" * "
                                                MinimumValue="01/01/2000" MaximumValue="31/12/2040" ValidationGroup="EditNew"
                                                Type="Date" />
                                            <ajaxToolkit:CalendarExtender ID="calCancelDate" CssClass="CalMaster" runat="server"
                                                Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtCancelDate" PopupButtonID="txtCancelDate">
                                            </ajaxToolkit:CalendarExtender>
                                        </div>
                                        <div id="divCancelUser" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCancelUser" Text="Usuario Cancelación" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtCancelUser" runat="server" Width="100px" MaxLength="20" />
                                                <asp:RequiredFieldValidator ID="rfvCancelUser" ValidationGroup="EditNew" ControlToValidate="txtCancelUser"
                                                    runat="server" ErrorMessage="Usuario Cancelación es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divComments"  class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLineComment" runat="server" Text="Comentarios"/>
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtLineComment" runat="server" TextMode="MultiLine" Width="250px" Height="40px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="revTxtLineComment" runat="server"             
                                                   ErrorMessage="Comentarios permite ingresar hasta un maximo de 255 caracteres"            
                                                   ValidationExpression="^([\s\S]{0,255})$"             
                                                   ControlToValidate="txtLineComment"  
                                                   ValidationGroup="EditNew" Text="*"                                       
                                                   Display="Dynamic"></asp:RegularExpressionValidator>
                               
                                            </div>
                                        </div>
                                    </div>
                                    <div>
                                        <asp:ValidationSummary ID="valEditNew" runat="server" ValidationGroup="EditNew" CssClass="modalValidation" />
                                    </div>
                                    <div style="clear: both">
                                        <div class="divItemDetails" style="max-height: 200px;" >
                                            <div id="Div1" runat="server" class="divGridTitle">
                                                <asp:Label ID="lblGridDetail" runat="server" Text="Detalle" />
                                            </div>
                                            <div class="mainFilterPanelItem">
                                                <asp:Label ID="lblCode" runat="server" Text="Item" /><br />
                                                <asp:TextBox ID="txtCode" runat="server" Width="100px" />
                                                <asp:ImageButton ID="imgbtnSearchItem" runat="server" Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                                    OnClick="imgBtnSearchItem_Click" ToolTip="Buscar Item" Width="18px" ValidationGroup="searchItem" />
                                                <asp:RequiredFieldValidator ID="rfvItemOwner" runat="server" ControlToValidate="ddlOwner"
                                                    InitialValue="-1" ValidationGroup="searchItem" Text=" * " ErrorMessage="Debe seleccionar Dueño." />
                                            </div>
                                            <div class="mainFilterPanelItem">
                                                <asp:Label ID="lblDescription" runat="server" Text="Descripción" /><br />
                                                <asp:TextBox ID="txtDescription" runat="server" Width="150px" MaxLength="30" Enabled="False"
                                                    ReadOnly="True" />
                                                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                                    ValidationGroup="AddItem" Text=" * " ErrorMessage="Descripción es requerido." />
                                            </div>
                                            <div ID="divCategoryItem" runat="server"  class="mainFilterPanelItem">
                                                <asp:Label ID="lblCategoryItem" runat="server" Text="Categoría" /><br />
                                                <asp:DropDownList ID="ddlCategoryItem" runat="server" Width="120px" />
                                            </div>
                                            <div id="divLotItem" runat="server" class="mainFilterPanelItem"> 
                                                <asp:Label ID="lblLotItem" runat="server" Text="Lote" /><br />
                                                <asp:TextBox ID="txtLotItem" runat="server" Width="100px" />
                                            </div>
                                            <div id="divFabricationDateItem" runat="server" class="mainFilterPanelItem"> 
                                                <asp:Label ID="lblFabricationDateItem" runat="server" Text="Fecha Fabricación" /><br />
                                                <asp:TextBox ID="txtFabricationDateItem" runat="server" Width="100px" />
                                                <ajaxToolkit:CalendarExtender ID="calFabricationDateItem" CssClass="CalMaster" runat="server" 
                                                    Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtFabricationDateItem" PopupButtonID="txtFabricationDateItem" Format="dd/MM/yyyy">
                                                </ajaxToolkit:CalendarExtender>
                                            </div>
                                            <div id="divExpirationDateItem" runat="server" class="mainFilterPanelItem"> 
                                                <asp:Label ID="lblExpirationDateItem" runat="server" Text="Fecha Expiración" /><br />
                                                <asp:TextBox ID="txtExpirationDateItem" runat="server" Width="100px" />
                                                <ajaxToolkit:CalendarExtender ID="calExpirationDateItem" CssClass="CalMaster" runat="server" 
                                                    Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtExpirationDateItem" PopupButtonID="txtExpirationDateItem" Format="dd/MM/yyyy">
                                                </ajaxToolkit:CalendarExtender>
                                            </div>
                                            <div class="mainFilterPanelItem">
                                                <asp:Label ID="lblQty" runat="server" Text="Cantidad" /><br />
                                                <asp:TextBox ID="txtQty" runat="server" Width="70px" />
                                                <asp:RequiredFieldValidator ID="rfvQty" runat="server" ControlToValidate="txtQty"
                                                    ValidationGroup="AddItem" Text=" * " ErrorMessage="Cantidad es requerido." />
                                                <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="txtQty" ErrorMessage="Cantidad no contiene un número válido"
                                                    Text=" * " MinimumValue="0" ValidationGroup="AddItem" Type="Double" Enabled="False" />
                                            </div>
                                            <div class="mainFilterPanelItem">
                                                <br />
                                                <asp:ImageButton ID="imgBtnAddItem" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_add_item.png"
                                                    OnClick="imgBtnAddItem_Click" ValidationGroup="AddItem" ToolTip="Agregar Item" />
                                            </div>
                                            <asp:Panel ID="pnlError" runat="server" Visible="False">
                                                <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="El item ya fue ingresado" />
                                            </asp:Panel>
                                            <div class="mainFilterPanelItem">
                                                <asp:ValidationSummary ID="valAddItem" runat="server" ValidationGroup="AddItem" />
                                                <asp:ValidationSummary ID="valSearchItem" runat="server" ValidationGroup="searchItem" />
                                            </div>

                                            <div style="clear: both; margin: 2px; max-height:100px; overflow:auto;">
                                                <asp:GridView ID="grdItems" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                                                    OnRowDeleting="grdItems_RowDeleting" OnRowEditing="grdItems_RowEditing"
                                                    OnRowDataBound="grdItems_RowDataBound"
                                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                    EnableTheming="false">
                                                    <Columns>
                                                        <asp:BoundField DataField="Id" HeaderText="Id Det." ReadOnly="True" SortExpression="Id"
                                                            AccessibleHeaderText="DetailId" />
                                                        <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="ItemId" SortExpression="Id">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval ("Item.Id") %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblCode" runat="server" Text='<%# Eval ("Item.Code") %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="ItemName">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.Description") %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CategoryItem">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblCategoryItem" runat="server" Text='<%# Eval ("CategoryItem.Name") %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblItemQty" runat="server" Text='<%# GetFormatedNumber(Eval ("ItemQty")) %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblLotDetail" runat="server" Text='<%# Eval ("LotNumber") %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Fecha Fabricación" AccessibleHeaderText="FabricationDate">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblFabricationDateDetail" runat="server" Text='<%# ((DateTime)Eval("FabricationDate") > DateTime.MinValue) ? Eval("FabricationDate", "{0:d}") : "" %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Fecha Expiración" AccessibleHeaderText="ExpirationDate">
                                                            <ItemTemplate>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblExpirationDateDetail" runat="server" Text='<%# ((DateTime)Eval("ExpirationDate") > DateTime.MinValue) ? Eval("ExpirationDate", "{0:d}") : "" %>' />
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <%--<asp:BoundField DataField="ItemQty" HeaderText="Cantidad" AccessibleHeaderText="ItemQty"
                                                            SortExpression="ItemQty" />--%>
                                                        <asp:TemplateField HeaderText="Acciones">
                                                            <ItemTemplate>
                                                                <center>
                                                                    <div style="width: 60px">
                                                                        <asp:ImageButton ID="btnEdit" runat="server" CommandName="Edit" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                                            ToolTip="Editar Detalle" />
                                                                        <asp:ImageButton ID="btnDelete" runat="server" CommandName="Delete" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                                            ToolTip="Quitar Detalle" />
                                                                        <asp:ImageButton ID="btnBack" runat="server" CommandName="Back" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_undo.png"
                                                                            Visible="false" />
                                                                    </div>
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            <ajaxToolkit:TabPanel runat="server" ID="TabDelivery">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divCustomerCode" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCustomerCode" Text="Cód. Cliente" runat="server" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtCustomerCode" runat="server" Width="120px" />
                                                <asp:HiddenField ID="hidIdCustomer" runat="server" />
                                                <asp:ImageButton ID="imgBtnCustmerSearch" 
                                                    runat="server" 
                                                    Height="18px" 
                                                    ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                                    OnClick="imgBtnCustmerSearch_Click" 
                                                    Width="18px" 
                                                    ToolTip="Buscar Cliente"
                                                    ValidationGroup="searchCustomer" />
                                                <asp:RequiredFieldValidator ID="rfvCustomerCode" ControlToValidate="txtCustomerCode" ValidationGroup="EditNew"
                                                    runat="server" Text=" * " ErrorMessage="Cód. Cliente es requerido"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div id="divCustomerName" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCustomerName" Text="Nombre Cliente" runat="server" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtCustomerName" runat="server" Width="120px" />
                                                <asp:RequiredFieldValidator ID="rfvCustomerName1" ValidationGroup="EditNew" ControlToValidate="txtCustomerName"
                                                    runat="server" ErrorMessage="Nombre Cliente es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divDeliveryAddress1" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDeliveryAddress1" Text="Dir. Entrega" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtDeliveryAddress1" runat="server" Width="120px" />
                                                <asp:RequiredFieldValidator ID="rfvDeliveryAddress11" ValidationGroup="EditNew" ControlToValidate="txtDeliveryAddress1"
                                                    runat="server" ErrorMessage="Dir. Entrega es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divDeliveryAddress2" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDeliveryAddress2" Text="Dir. Entrega Opc." runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtDeliveryAddress2" runat="server" />
                                                <asp:RequiredFieldValidator ID="rfvDeliveryAddress2" ValidationGroup="EditNew" ControlToValidate="txtDeliveryAddress2"
                                                    runat="server" ErrorMessage="Dir. Entrega Opc. es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divCountryDelivery" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCountryDelivery" Text="País Entrega" runat="server" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlCountryDelivery" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCountryDelivery_SelectedIndexChanged"
                                                    Width="130px" />
                                               <%-- <asp:RequiredFieldValidator ID="rfvCountryDelivery" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlCountryDelivery" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="País Entrega es requerido" />--%>
                                            </div>
                                        </div>
                                        <div id="divStateDelivery" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblStateDelivery" Text="Región Entrega" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlStateDelivery" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlStateDelivery_SelectedIndexChanged"
                                                    Width="200px" />
                                                <%--<asp:RequiredFieldValidator ID="rfvStateDelivery" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlStateDelivery" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Región Entrega es requerido" />--%>
                                            </div>
                                        </div>
                                        <div id="divCityDelivery" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCityDelivery" Text="Ciudad Entrega" runat="server" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlCityDelivery" runat="server" Width="130px" AutoPostBack="true" OnSelectedIndexChanged="ddlCityDelivery_SelectedIndexChanged" />
                                                <asp:RequiredFieldValidator ID="rfvCityDelivery1" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlCityDelivery" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Ciudad Entrega es requerido" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divDeliveryPhone" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDeliveryPhone" Text="Tel. Entrega" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtDeliveryPhone" runat="server" Width="70px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvDeliveryPhone1" ValidationGroup="EditNew" ControlToValidate="txtDeliveryPhone"
                                                    runat="server" ErrorMessage="Tel. Entrega es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divDeliveryEmail" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDeliveryEmail" Text="E-mail Entrega" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtDeliveryEmail" runat="server" Width="70px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvDeliveryEmail" ValidationGroup="EditNew" ControlToValidate="txtDeliveryEmail"
                                                    runat="server" ErrorMessage="E-mail Entrega es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divWarehouseTarget" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWarehouseTarget" Text="Centro Destino" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlWarehouseTarget" runat="server" />
                                                <asp:RequiredFieldValidator ID="rfvWarehouseTarget" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlWarehouseTarget" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Centro Destino es requerido" />
                                            </div>
                                        </div>
                                        <div id="divCarrierCode" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCarrierCode" Text="Transportista" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlCarrier" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divRouteCode" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblRouteCode" Text="Ruta Despacho" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtRouteCode" runat="server" Width="70px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvRouteCode" ValidationGroup="EditNew" ControlToValidate="txtRouteCode"
                                                    runat="server" ErrorMessage="Ruta Despacho es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divPlate" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPlate" Text="Patente Camión" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPlate" runat="server" Width="70px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvPlate" ValidationGroup="EditNew" ControlToValidate="txtPlate"
                                                    runat="server" ErrorMessage="Patente Camión es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divBranch" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblBranch" Text="Sucursal Entrega" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlBranch" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                    <div class="mainFilterPanelItem">
                                        <asp:ValidationSummary ID="valAddCustomer" runat="server" ValidationGroup="AddCustomer" />
                                        <asp:ValidationSummary ID="valSearchCustomer" runat="server" ValidationGroup="searchCustomer" />
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            <ajaxToolkit:TabPanel runat="server" ID="TabSales">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divFactAddress1" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFactAddress1" Text="Dirección Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtFactAddress1" runat="server" Width="70px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvFactAddress1" ValidationGroup="EditNew" ControlToValidate="txtFactAddress1"
                                                    runat="server" ErrorMessage="Dirección Factura es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divFactAddress2" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFactAddress2" Text="Dirección Factura Opc." runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtFactAddress2" runat="server" Width="70px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvFactAddress2" ValidationGroup="EditNew" ControlToValidate="txtFactAddress2"
                                                    runat="server" ErrorMessage="Dirección Factura Opc. es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divInvoice" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblInvoice" Text="Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtInvoice" runat="server" Width="70px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvInvoice" ValidationGroup="EditNew" ControlToValidate="txtInvoice"
                                                    runat="server" ErrorMessage="Factura es requerido" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divCountryFact" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCountryFact" Text="País Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlCountryFact" runat="server" Width="130px" OnSelectedIndexChanged="ddlCountryFact_SelectedIndexChanged"
                                                    AutoPostBack="True" />
                                                <asp:RequiredFieldValidator ID="rfvCountryFact" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlCountryFact" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="País Factura es requerido" />
                                            </div>
                                        </div>
                                        <div id="divStateFact" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblStateFact" Text="Región Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlStateFact" runat="server" Width="130px" OnSelectedIndexChanged="ddlStateFact_SelectedIndexChanged"
                                                    AutoPostBack="True" />
                                                <asp:RequiredFieldValidator ID="rfvStateFact" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlStateFact" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Estado Factura es requerido" />
                                            </div>
                                        </div>
                                        <div id="divCityFact" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCityFact" Text="Ciudad Entrega" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlCityFact" runat="server" Width="130px" />
                                                <asp:RequiredFieldValidator ID="rfvCityFact" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlCityFact" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Ciudad Entrega es requerido" />
                                            </div>
                                        </div>
                                        <div id="divFactPhone" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFactPhone" Text="Tel. Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtFactPhone" runat="server" Width="100px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvFactPhone" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="txtFactPhone" Display="dynamic" ErrorMessage="Tel. Factura es requerido" />
                                            </div>
                                        </div>
                                        <div id="divFactEmail" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFactEmail" Text="E-mail Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtFactEmail" runat="server" Width="150px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvFactEmail" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="txtFactEmail" Display="dynamic" ErrorMessage="E-mail Factura es requerido" />
                                            </div>
                                        </div>
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                        </ajaxToolkit:TabContainer>
                        <div id="divActions" runat="server" class="modalActions" >
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" OnClick="btnCloseNewEdit_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Panel Nuevo/Editar Documento --%>
            <%-- Lookup Items --%>
            <div id="divLookupItem" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpLookupItem" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlLookupItem" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupItem"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlLookupItem" runat="server" CssClass="modalBox">
                    <asp:Panel ID="pnlHeadBar2" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblAddItem" runat="server" Text="Buscar Item" />
                            <asp:ImageButton ID="imgBtnCloseItemSearch" runat="server" ImageAlign="Top" CssClass="closeButton"
                                ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                        <div id="divPageGrdSearchItems" runat="server">
                            <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                <asp:ImageButton ID="btnFirstGrdSearchItems" runat="server" OnClick="btnFirstGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                <asp:ImageButton ID="btnPrevGrdSearchItems" runat="server" OnClick="btnPrevGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                                Pág. 
                                <asp:DropDownList ID="ddlPagesSearchItems" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchItemsSelectedIndexChanged" SkinID="ddlFilter" /> 
                                de 
                                <asp:Label ID="lblPageCountSearchItems" runat="server" Text="" />
                                <asp:ImageButton ID="btnNextGrdSearchItems" runat="server" OnClick="btnNextGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                <asp:ImageButton ID="btnLastGrdSearchItems" runat="server" OnClick="btnLastGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidItemId" runat="server" Value="-1" />
                        <webUc:ucLookUpFilter ID="ucFilterItem" runat="server" />
                        <div class="divCtrsFloatLeft">
                            <div class="divLookupGrid">
                                <asp:GridView ID="grdSearchItems" runat="server" DataKeyNames="Id" OnRowCommand="grdSearchItems_RowCommand" AllowPaging="true"
                                    OnRowDataBound="grdSearchItems_RowDataBound"
                                    AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                    EnableTheming="false">
                                    <Columns>
                                        <asp:BoundField AccessibleHeaderText="Id" DataField="Id" HeaderText="ID" InsertVisible="True"
                                            SortExpression="Id" />
                                        <asp:TemplateField AccessibleHeaderText="ItemCode" HeaderText="Cód.">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCode0" runat="server" Text='<%# Eval ("Code") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="Description" HeaderText="Item">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblItemName0" runat="server" Text='<%# Eval ("Description") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="imgBtnAddItem" ToolTip="Agregar Cliente" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                        Width="20px" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div style="clear: both" />
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Lookup Items --%>
            <%-- Lookup Customers --%>
            <div id="divLookupCustomer" runat="server" visible="false">
                <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpLookupCustomer" runat="server" TargetControlID="btnDummy2"
                    PopupControlID="pnlLookupCustomer" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupCustomer"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlLookupCustomer" runat="server" CssClass="modalBox">
                    <asp:Panel ID="pnlHeadBarCustomer" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblAddCustomer" runat="server" Text="Buscar Cliente" />
                            <asp:ImageButton ID="imgBtnCloseCustomerSearch" runat="server" ImageAlign="Top" CssClass="closeButton"
                                ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                        <div id="divPageGrdSearchCustomers" runat="server">
                            <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                <asp:ImageButton ID="btnFirstGrdSearchCustomers" runat="server" OnClick="btnFirstGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                <asp:ImageButton ID="btnPrevGrdSearchCustomers" runat="server" OnClick="btnPrevGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                                Pág. 
                                <asp:DropDownList ID="ddlPagesSearchCustomers" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchCustomersSelectedIndexChanged" SkinID="ddlFilter" /> 
                                de 
                                <asp:Label ID="lblPageCountSearchCustomers" runat="server" Text="" />
                                <asp:ImageButton ID="btnNextGrdSearchCustomers" runat="server" OnClick="btnNextGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                <asp:ImageButton ID="btnLastGrdSearchCustomers" runat="server" OnClick="btnLastGrdSearchCustomers_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidCustomerId" runat="server" Value="-1" />
                        <webUc:ucLookUpFilter ID="ucFilterCustomer" runat="server" />
                        <div class="divCtrsFloatLeft">
                            <div class="divLookupGrid">
                                <asp:GridView ID="grdSearchCustomers" runat="server" DataKeyNames="Id" OnRowCommand="grdSearchCustomers_RowCommand" AllowPaging="true"
                                    OnRowDataBound="grdSearchCustomers_RowDataBound"
                                    AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                    EnableTheming="false">
                                    <Columns>
                                        <asp:BoundField AccessibleHeaderText="Id" DataField="Id" HeaderText="ID" InsertVisible="True"
                                            SortExpression="Id" Visible="false" />
                                        <asp:TemplateField AccessibleHeaderText="IdOwn" HeaderText="Id Dueño" Visible="false">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ("Owner.Id") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="OwnName" HeaderText="Dueño">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval ("Owner.Name") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="CustomerCode" HeaderText="Código">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ("Code") %>' />
                                               </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="CustomerName" HeaderText="Cliente">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ("Name") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="imgBtnAddCustomer" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                        Width="20px" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div style="clear: both" />
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Lookup Customers --%>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />   
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnGenerateChangeTrack" EventName="Click" />             
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid">
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />


     <%-- Carga masiva de documentos --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoad" runat="server" visible="false">
                <asp:Button ID="btnDummyLoad" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpLoad" runat="server" 
                    TargetControlID="btnDummyLoad"
                    PopupControlID="panelLoad" 
                    BackgroundCssClass="modalBackground" 
                    PopupDragHandleControlID="panelCaptionLoad"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelLoad" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionLoad" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label1" runat="server" Text="Carga Masiva de Documentos" />
                             <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Documento%20Salida.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />                                
                            <asp:ImageButton ID="ImageButton1" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">                       
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label7" runat="server" Text="Seleccione Archivo" />
                                </div>
                                <div class="fieldLeft">                      
                                    <asp:FileUpload ID="uploadFile" runat="server" Width="400px" ValidationGroup="Load"/>                                    
                                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo"
                                    ValidationGroup="Load" ControlToValidate="uploadFile"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator id="revUpload" runat="server"  ValidationGroup="Load"
                                    ErrorMessage="Cargar solo archivos .xls ó .xlsx"  Text="*"
                                    ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$" 
                                    ControlToValidate="uploadFile">
                                    </asp:RegularExpressionValidator>                             
                                    
                                </div>  
                            </div>
                            <div class="divControls" >
                                <div class="fieldRight">
                                    <asp:Label ID="Label8" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnLoadFile" runat="server" Text="Cargar Archivo" ValidationGroup="Load" 
                                     onclick="btnLoadFile_Click" OnClientClick="showProgress();" />
                                </div>
                            </div>
                           <%-- <div style="clear: both">
                            </div>--%>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load" ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div8" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Load --%>            

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnLoadFile"/>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />            
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprLoad" runat="server" AssociatedUpdatePanelID="upLoad">
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressGrid" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprLoad" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLoad" />

    <asp:UpdatePanel ID="upChangeTrack" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divChangeTrack" runat="server" visible="false">
                <asp:Button ID="btnDummyChangeTrack" runat="Server"  Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="modalPopupChangeTrack" runat="server" 
                    TargetControlID="btnDummyChangeTrack"
                    PopupControlID="panelChangeTrack" 
                    BackgroundCssClass="modalBackground" 
                    PopupDragHandleControlID="panelCaptionChangeTrack"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelChangeTrack" runat="server" CssClass="modalBox" >
                    <asp:Panel ID="panelCaptionChangeTrack" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:HiddenField runat="server" ID="hidIdDocumentSelected" />
                            <asp:Label ID="Label2" runat="server" Text="Cambio Track Documento" />
                            <asp:ImageButton ID="ImageButton2" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>

                    <div class="modalControls" style="width:300px">
                        <div class="divCtrsFloatLeft">     
                        
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblNameTrack" runat="server" Text="Track" />
                                </div>
                                <div class="fieldRight">
                                    <asp:DropDownList ID="ddlTracks" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvTracks" runat="server" ValidationGroup="validationChangeTrack"
                                        Text=" * " ControlToValidate="ddlTracks" Display="Dynamic" InitialValue="-1" ErrorMessage="Track es requerido" />
                                </div>
                            </div>

                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDateTrack" runat="server" Text="Fecha" />
                                </div>
                                <div class="fieldLeft">     
                                    <asp:TextBox SkinID="txtFilter" ID="txtDateTrack" runat="server" Width="70px" ValidationGroup="validationChangeTrack" ToolTip="Ingrese fecha." />
                                    <asp:RequiredFieldValidator ID="rfvDateTrack" runat="server" ControlToValidate="txtDateTrack" ValidationGroup="validationChangeTrack" Text=" * " ErrorMessage="Fecha es requerido"  />
                                
                                    <asp:RangeValidator ID="rvDateDateTrack" runat="server" ControlToValidate="txtDateTrack"
                                        ErrorMessage="Fecha debe estar entre 01-01-2000 y 31-12-2040" Text=" * " MinimumValue="01/01/2000"
                                        MaximumValue="31/12/2040" ValidationGroup="validationChangeTrack" Type="Date" />
                                    <ajaxToolkit:CalendarExtender ID="calDateTrack" CssClass="CalMaster" runat="server" 
                                        Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtDateTrack" PopupButtonID="txtDateTrack" Format="dd/MM/yyyy">
                                    </ajaxToolkit:CalendarExtender>
                                </div>
                            </div>

                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblHourTrack" runat="server" Text="Hora" />
                                </div>
                                <div class="fieldLeft">     
                                    <asp:TextBox SkinID="txtFilter" ID="txtHourTrack" runat="server" Width="50px"  MaxLength="5"
                                    ToolTip="Ingrese hora formato 24hrs." ValidationGroup="validationChangeTrack" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvHourTrack" ControlToValidate="txtHourTrack" ValidationGroup="validationChangeTrack"
                                    runat="server" ErrorMessage="Hora es requerido" Text=" * " />
                                    
                                    <asp:RegularExpressionValidator ID="revHourTrack" runat="server" ControlToValidate="txtHourTrack"
                                    ErrorMessage="Hora no es valida ej: 23:30" Display="Dynamic" 
                                    ValidationExpression="^[0-2]?[0-9]:[0-5][0-9]$" ValidationGroup="validationChangeTrack" Text=" * ">
                                   </asp:RegularExpressionValidator>
                                </div>
                            </div>
                        </div>
                        <div style="clear: both"></div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsChangeTrack" runat="server" ValidationGroup="validationChangeTrack" ShowMessageBox="false" CssClass="modalValidation" />
                        </div>         
                        <div id="div2" runat="server" class="modalActions">
                            <asp:Button ID="btnGenerateChangeTrack" runat="server" Text="Aceptar" OnClick="btnGenerateChangeTrack_Click" CausesValidation="true" ValidationGroup="validationChangeTrack" />
                            <asp:Button ID="btnCloseChangeTrack" runat="server" Text="Cancelar"  OnClick="btnCloseChangeTrack_Click"  />
                        </div>
                    </div>
                </asp:Panel>     
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="uprChangeTrack" runat="server" AssociatedUpdatePanelID="upChangeTrack" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgressChangeTrack" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprChangeTrack" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprChangeTrack" />

    
    <%-- Mensaje--%>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Documento?" Visible="false" />
    <asp:Label ID="lblConfirmClose" runat="server" Text="¿Desea cerrar esta Orden?" Visible="false" />
    <asp:Label ID="lblConfirmCancel" runat="server" Text="¿Desea anular esta Orden?" Visible="false" />
    <asp:Label ID="lblConfirmPending" runat="server" Text="¿Desea habilitar esta Orden?" Visible="false" />
    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lbltabGeneral" runat="server" Text="Datos Generales" Visible="false" />
    <asp:Label ID="lblTabDelivery" runat="server" Text="Entrega" Visible="false" />
    <asp:Label ID="lblTabSales" runat="server" Text="Facturacion" Visible="false" />
    <asp:Label ID="lblAdvancedFilter" runat="server" Text="Filtros" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Adm. Documentos" Visible="false" />
    <asp:Label ID="lblWaveValidation" runat="server" Text="No se puede anular el documento debido a track inválido de Ola [WAVEID] asociada" Visible="false" />
    <asp:Label ID="lblValidateQty" runat="server" Text="Debe ser mayor a 0" Visible="false" />
    <asp:Label ID="lblValidateRepeatedItems" runat="server" Text="No deben existir ítems repetidos con mismo ItemCode, CtgCode, LotNumber, ExpirationDate y FabricationDate." Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>

    <%-- Mensajes de advertencia y error Para Carga Masiva --%>
    <asp:Label ID="lblNotFileLoad" runat="server" Text="Seleccione el archivo que desea cargar." Visible="false" />
    <asp:Label ID="lblNotInboundOrderFile" runat="server" Text="No existen ordenes en el archivo." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es valído." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma exitosa." Visible="false" />
    <asp:Label ID="lblNotAccessServerFolder" runat="server" Text="No existe acceso al servidor." Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />

    <%-- Div Bloquea Pantalla al Momento de Realizar Carga Masiva --%>
    <div id="divFondoPopupProgress" class="loading" align="center" style="display: none;">
        Realizando Carga Masiva <br />Espere un momento...<br />
        <br />
        <img src="../../WebResources/Images/Buttons/ajax-loader.gif" alt="" />
    </div>

</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
