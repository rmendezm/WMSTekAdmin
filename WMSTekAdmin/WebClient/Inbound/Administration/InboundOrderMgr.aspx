<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="InboundOrderMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inbound.Administration.InboundOrderMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script language="javascript" type="text/javascript">


        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(SetDivs);
        window.onresize = SetDivs;


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
            initializeGridDragAndDrop("InboundOrder_FindAll", "ctl00_MainContent_grdMgr", "InboundOrderMgr");
            initializeGridWithNoDragAndDrop(true);
            //removeFooter("#ctl00_MainContent_grdSearchItems");

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
            initializeGridDragAndDrop("InboundOrder_FindAll", "ctl00_MainContent_grdMgr", "InboundOrderMgr");
            initializeGridWithNoDragAndDrop(true);
            //removeFooter("#ctl00_MainContent_grdSearchItems");
        }
    </script>

    <style>
        #divItemDetailsWrapper {
            overflow: auto;
            max-height: 150px;
        }
    </style>

    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid">
                            <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id"
                                AllowPaging="True"
                                EnableViewState="False"
                                OnRowCommand="grdMgr_RowCommand"
                                OnRowDeleting="grdMgr_RowDeleting"
                                OnRowDataBound="grdMgr_RowDataBound"
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowEditing="grdMgr_RowEditing"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed"
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" ReadOnly="True"
                                        SortExpression="Id" AccessibleHeaderText="Id" ItemStyle-Wrap="false">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Tipo Doc." AccessibleHeaderText="InboundType" SortExpression="InboundType"
                                        HeaderStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblInboundType" runat="server" Text='<%# Eval ( "InboundType.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Number" HeaderText="Nº Doc." AccessibleHeaderText="Number"
                                        SortExpression="Number" ItemStyle-Wrap="false">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Cód. Centro D." AccessibleHeaderText="WarehouseCode">
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
                                                <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Proveedor" AccessibleHeaderText="Vendor" SortExpression="Vendor"
                                        HeaderStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval ( "Vendor.Name" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Comment" HeaderText="Comentarios" AccessibleHeaderText="Comment"
                                        SortExpression="Comment" HeaderStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Esperada" AccessibleHeaderText="DateExpected" SortExpression="DateExpected">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblDateExpected" runat="server" Text='<%# ((DateTime) Eval ("DateExpected") > DateTime.MinValue)? Eval("DateExpected", "{0:d}"):"" %>' />
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
                                    <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="InboundTrack" SortExpression="InboundTrack">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblInboundTrack" runat="server" Text='<%# Eval ( "LatestInboundTrack.Type.Name" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status" SortExpression="Status">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# Eval ( "Status" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Doc. salida" AccessibleHeaderText="OutboundOrderNumber"
                                        SortExpression="OutboundOrder">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOutboundOrder" runat="server" Text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Es Asn" AccessibleHeaderText="IsAsn" SortExpression="IsAsn">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkIsAsn" runat="server" Checked='<%# Eval ( "IsAsn" ) %>' Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="% Lpn Inspección" AccessibleHeaderText="PercentLpnInspection"
                                        SortExpression="PercentLpnInspection">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblPercentLpnInspection" runat="server" Text='<%# ((int) Eval ("PercentLpnInspection") == -1)?" ":Eval ("PercentLpnInspection") %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="% QA" AccessibleHeaderText="PercentQA" SortExpression="PercentQA">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblPercentQA" runat="server" Text='<%# ((int) Eval ("PercentQA") == -1)?" ":Eval ("PercentQA") %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ShiftNumber" HeaderText="Turno" AccessibleHeaderText="ShiftNumber"
                                        SortExpression="ShiftNumber" ItemStyle-Wrap="false">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Observación" AccessibleHeaderText="SpecialField1">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Eval ( "SpecialField1" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="SpecialField1" HeaderText="Campo. Esp. 1" AccessibleHeaderText="SpecialField1"
                                        SortExpression="SpecialField1" ItemStyle-Wrap="false" ItemStyle-CssClass="text">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SpecialField2" HeaderText="Campo. Esp. 2" AccessibleHeaderText="SpecialField2"
                                        SortExpression="SpecialField2" ItemStyle-Wrap="false" ItemStyle-CssClass="text">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SpecialField3" HeaderText="Campo. Esp. 3" AccessibleHeaderText="SpecialField3"
                                        SortExpression="SpecialField3" ItemStyle-Wrap="false" ItemStyle-CssClass="text">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SpecialField4" HeaderText="Campo. Esp. 4" AccessibleHeaderText="SpecialField4"
                                        SortExpression="SpecialField4" ItemStyle-Wrap="false" ItemStyle-CssClass="text">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Trámites Pendientes" AccessibleHeaderText="HasIssues">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkOrderHasIssues" runat="server" Checked='<%# Eval ( "HasIssues" ) %>' Enabled="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField ShowHeader="False" HeaderText="Acciones" AccessibleHeaderText="Actions" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 85px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" ToolTip="Editar Documento"/>
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CausesValidation="false" CommandName="Delete" ToolTip="Eliminar Documento"/>
                                                    <asp:ImageButton ID="btnClose" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_close.png"
                                                        CausesValidation="false" CommandName="CloseOrder" ToolTip="Cerrar Documento"/>
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>


            <%-- FIN Grilla Principal --%>

            <%-- Panel Nuevo/Editar Documento --%>
            <div id="divModal" runat="server" visible="false">
                <asp:Panel ID="pnlPanelEditNew" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="InboundCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Documento" Width="100%" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Documento" Width="100%" />
                            <asp:ImageButton ID="imgbtnClose" runat="server" OnClick="imgCloseNewEdit_Click" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" ToolTip="Cerrar y volver al listado" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>

                    <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                    <asp:HiddenField ID="hidSelectedOwner" runat="server" Value="-1" />
                    <asp:HiddenField ID="hidLatestInboundTrackId" runat="server" Value="-1" />

                    <div class="modalBoxContent">
                        <div class="divCtrsFloatLeft">
                            <div id="divStatus" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Status" Text="Activo" runat="server" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="false" TabIndex="10" /></div>
                            </div>
                            <div id="divWarehouse" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWarehouse" runat="server" Text="Centro" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlWarehouse" runat="server" Width="150px" TabIndex="7" Enabled="false" />
                                    <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ValidationGroup="EditNew"
                                        Text=" * " ErrorMessage="Centro es requerido" ControlToValidate="ddlWarehouse" Display="Dynamic"
                                        InitialValue="-1" />
                                </div>
                            </div>
                            <div id="divOwner" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlOwner" runat="server" Width="120px" AutoPostBack="true" Enabled="false" OnSelectedIndexChanged="ddlOwner_Changed" />
                                    <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ValidationGroup="EditNew"
                                        Text=" * " ErrorMessage="Dueño es requerido" ControlToValidate="ddlOwner" Display="dynamic" InitialValue="-1" />
                                </div>
                            </div>
                            <div id="divIdInboundType" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIdInboundType" runat="server" Text="Tipo Doc." />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlIdInboundType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlInboundType_Changed">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvIdInboundType" runat="server" ValidationGroup="EditNew"
                                        Text=" * " ErrorMessage="Tipo es requerido" ControlToValidate="ddlIdInboundType" Display="dynamic" InitialValue="-1" />
                                </div>
                            </div>
                            <div id="divInboundOrder" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblInboundOrder" runat="server" Text="Nº Doc." />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtNumInboundOrder" runat="server" Enabled="true" Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvNumInbound" runat="server" ControlToValidate="txtNumInboundOrder" ValidationGroup="EditNew" Text=" * " ErrorMessage="Doc. Entrada es requerido" />
                                </div>
                            </div>
                            <div id="divVendor" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblVendor" runat="server" Text="Proveedor" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlVendor" runat="server" Width="120px">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvVendor" runat="server" ValidationGroup="EditNew"
                                        Text=" * " ErrorMessage="Proveedor es requerido" ControlToValidate="ddlVendor" Display="dynamic" InitialValue="-1" />
                                </div>
                            </div>
                            <div id="divEmissionDate" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblEmissionDate" runat="server" Text="Emisión" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtEmissionDate" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEmissionDate" runat="server" ControlToValidate="txtEmissionDate" Text=" * " ValidationGroup="EditNew" ErrorMessage="Emisión es requerido." Enabled="false" />
                                    <asp:RangeValidator ID="rvEmissionDate" runat="server" ControlToValidate="txtEmissionDate" ErrorMessage="Emisión debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MaximumValue="31/12/2040" MinimumValue="01/01/2000" ValidationGroup="EditNew" Type="Date" />
                                </div>
                                <ajaxToolkit:CalendarExtender ID="calEmissionDate" CssClass="CalMaster" runat="server"
                                    Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtEmissionDate" PopupButtonID="txtEmissionDate"
                                    Format="dd-MM-yyyy">
                                </ajaxToolkit:CalendarExtender>
                            </div>
                            <div id="divDateExpected" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDateExpected" runat="server" Text="Esperada" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtDateExpected" runat="server" Width="80px"></asp:TextBox><asp:RequiredFieldValidator ID="rfvDateExpected" runat="server" ControlToValidate="txtDateExpected" ValidationGroup="EditNew" Text=" * " ErrorMessage="Esperada es requerido." Enabled="false" />
                                    <asp:RangeValidator ID="rvDateExpected" runat="server" ControlToValidate="txtDateExpected" ErrorMessage="Esperada debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MaximumValue="31/12/2040" MinimumValue="01/01/2000" ValidationGroup="EditNew" Type="Date" />
                                    <asp:CompareValidator ID="cvDateExpected" ControlToValidate="txtDateExpected" ControlToCompare="txtEmissionDate" Type="Date" Operator="GreaterThanEqual" Text=" * " Display="Dynamic" ValidationGroup="EditNew" ErrorMessage="Fecha Esperada debe ser menor o igual a Fecha Emisión" runat="server"></asp:CompareValidator>
                                </div>
                                <ajaxToolkit:CalendarExtender ID="calDateExpected1" CssClass="CalMaster" runat="server"
                                    Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtDateExpected" PopupButtonID="txtDateExpected" Format="dd-MM-yyyy">
                                </ajaxToolkit:CalendarExtender>
                            </div>
                            <div id="divExpirationDate" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblExpirationDate" runat="server" Text="Vencimiento" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtExpirationDate" runat="server" Width="80px" TabIndex="8" />
                                    <asp:RequiredFieldValidator ID="rfvExpirationDate" runat="server" ControlToValidate="txtExpirationDate" ValidationGroup="EditNew" Text=" * " ErrorMessage="Vencimiento es requerido." Enabled="false" />
                                    <asp:RangeValidator ID="rvExpirationDate" runat="server" ControlToValidate="txtExpirationDate" ErrorMessage="Vencimiento debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MaximumValue="31/12/2040" MinimumValue="01/01/2000" ValidationGroup="EditNew" Type="Date" />
                                    <asp:CompareValidator ID="cvExpirationDate" ControlToValidate="txtExpirationDate" ControlToCompare="txtDateExpected" Type="Date" Operator="GreaterThanEqual" Text=" * " Display="Dynamic" ValidationGroup="EditNew" ErrorMessage="Fecha Vencimiento debe ser menor o igual a Fecha Esperada" runat="server"></asp:CompareValidator>
                                    <ajaxToolkit:CalendarExtender ID="calExpirationDate" CssClass="CalMaster" runat="server"
                                        Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtExpirationDate" PopupButtonID="txtExpirationDate" Format="dd-MM-yyyy">
                                    </ajaxToolkit:CalendarExtender>
                                </div>
                            </div>
                        </div>
                        <div class="divCtrsFloatLeft">
                            <div id="divNameTrackInbound" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblNameTrackInbound" runat="server" Text="Traza" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtInboundTrack" runat="server" Text='<%# Eval ( "TrackInbound.Name" ) %>'
                                        Enabled="false" />
                                </div>
                            </div>
                            <div id="divIdOutboundOrderSource" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIdOutboundOrderSource" Text="Doc. Salida" runat="server" />
                                </div>


                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtIdOutboundOrderSource" runat="server" Width="100px" TabIndex="11" AutoPostBack="false" />

                                    <asp:RequiredFieldValidator ID="rfvIdOutboundOrderSource" runat="server" ControlToValidate="txtIdOutboundOrderSource" ValidationGroup="EditNew" Text=" * " ErrorMessage="Doc. Salida es requerido" />


                                </div>


                            </div>
                            <div id="divIsAsn" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIsAsn" Text="Asn" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkIsAsn" runat="server" Checked='<%# Eval ( "IsAsn" ) %>' Enabled="false"
                                        TabIndex="12" />
                                </div>
                            </div>
                            <div id="divLpnInspection" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPercentLpnInspection" runat="server" Text="%LPN R." />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLpnInspection" runat="server" Width="50px" TabIndex="13" />
                                    <asp:RequiredFieldValidator ID="rfvLpnInspection" runat="server" ControlToValidate="txtLpnInspection" ValidationGroup="EditNew" Text=" * " ErrorMessage="%LPN R. es requerido" Enabled="false" />
                                    <asp:RangeValidator ID="rvLpnInspection" runat="server" ControlToValidate="txtLpnInspection" ErrorMessage="%LPN R. no contiene un número válido" Text=" * " MaximumValue="100" MinimumValue="0" ValidationGroup="EditNew" Type="Integer" />
                                </div>
                            </div>
                            <div id="divPercentQA" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPercentQA" runat="server" Text="% R.QA" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPercentQA" runat="server" Width="50px" TabIndex="14" />
                                    <asp:RequiredFieldValidator ID="rfvPercentQA" runat="server" ControlToValidate="txtPercentQA" ValidationGroup="EditNew" Text=" * " ErrorMessage="% R.QA es requerido" Enabled="false" />
                                    <asp:RangeValidator ID="rvPercentQA" runat="server" ControlToValidate="txtPercentQA" ErrorMessage="% R.QA no contiene un número válido" Text=" * " MaximumValue="100" MinimumValue="0" ValidationGroup="EditNew" Type="Integer" />
                                </div>
                            </div>
                            <div id="divShiftNumber" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblShiftNumber" runat="server" Text="Turno" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtShiftNumber" runat="server" TabIndex="15" />
                                    <asp:RequiredFieldValidator ID="rfvShiftNumber" runat="server" ControlToValidate="txtShiftNumber" ValidationGroup="EditNew" Text=" * " ErrorMessage="Turno es requerido" Enabled="false" />
                                </div>
                            </div>

                            <div id="divHasIssues" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblHasIssues" Text="Trámites Pendientes" runat="server" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkHasIssues" runat="server" Checked='<%# Eval ( "HasIssues" ) %>' TabIndex="16" />
                                </div>
                            </div>

                            <div id="divComments" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLineComment" runat="server" Text="Comentarios" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLineComment" runat="server" TextMode="MultiLine" Width="250px" Height="40px" TabIndex="17" />
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
                            <asp:ValidationSummary ID="valEditNew" runat="server" ValidationGroup="EditNew"
                                ShowMessageBox="false" CssClass="modalValidation" />
                        </div>

                        <%-- Panel Detalle Documento --%>
                        <div id="divItemDetailsWrapper" style="clear: both">
                            <%-- Panel Agregar/Modificar Item --%>
                            <div class="divItemDetails">
                                <div runat="server" visible="true" class="divGridTitle">
                                    <asp:Label ID="lblGridDetail" runat="server" Text="Detalle" />
                                </div>

                                <div class="mainFilterPanelItem">
                                    <asp:Label ID="lblCode" runat="server" Text="Item" /><br />
                                    <asp:TextBox ID="txtCode" runat="server" Width="100px" Enabled="true" />
                                    <asp:ImageButton ID="imgbtnSearchItem" runat="server" Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" OnClick="imgBtnSearchItem_Click" Width="18px" ValidationGroup="searchItem" />
                                    <asp:RequiredFieldValidator ID="rfvItemOwner" runat="server" ControlToValidate="ddlOwner" InitialValue="-1" ValidationGroup="searchItem" Text=" * " ErrorMessage="Debe seleccionar Dueño." />
                                </div>
                                <div class="mainFilterPanelItem">
                                    <asp:Label ID="lblDescription" runat="server" Text="Descripción" /><br />
                                    <asp:TextBox ID="txtDescription" runat="server" Width="150px" MaxLength="30" Enabled="False" ReadOnly="True" />
                                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" ValidationGroup="AddItem" Text=" * " ErrorMessage="Descripción es requerido." />
                                </div>
                                <div id="divCategoryItem" class="mainFilterPanelItem" runat="server">
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
                                    <asp:TextBox ID="txtQty" runat="server" Width="70px" Text="0" MaxLength="8" />
                                    <asp:RequiredFieldValidator ID="rfvQty" runat="server" ControlToValidate="txtQty" ValidationGroup="AddItem" Text=" * " ErrorMessage="Cantidad es requerido." />
                                    <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="txtQty"
                                        ErrorMessage="Cantidad no contiene un número válido" Text=" * " MinimumValue="1" MaximumValue="99999999"
                                        ValidationGroup="AddItem" Type="Double" Enabled="true" />
                                </div>
                                <div class="mainFilterPanelItem">
                                    <asp:Label ID="lblLpn" runat="server" Text="LPN" /><br />
                                    <asp:TextBox ID="txtLpn" runat="server" Width="100px" MaxLength="20" />
                                </div>
                                <div class="mainFilterPanelItem">
                                    <asp:Label ID="lblWeight" runat="server" Text="Peso" /><br />
                                    <asp:TextBox ID="txtWeight" runat="server" Width="70px" MaxLength="8" />
                                    <asp:RangeValidator ID="rvWeight" runat="server" ControlToValidate="txtWeight"
                                        ErrorMessage="Peso no contiene un número válido" Text=" * " MinimumValue="0" MaximumValue="99999999"
                                        ValidationGroup="AddItem" Type="Double" Enabled="true" />
                                </div>
                                <div class="mainFilterPanelItem">
                                    <br />
                                    <asp:ImageButton ID="imgBtnAddItem" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_add_item.png" OnClick="imgBtnAddItem_Click" ValidationGroup="AddItem" ToolTip="Agregar Item" />
                                </div>

                                <asp:Panel ID="pnlError" runat="server" Visible="false">
                                    <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="El item ya fue ingresado" />
                                </asp:Panel>

                                <div class="mainFilterPanelItem">
                                    <asp:ValidationSummary ID="valAddItem" runat="server" ValidationGroup="AddItem" />
                                    <asp:ValidationSummary ID="valSearchItem" runat="server" ValidationGroup="searchItem" />
                                </div>
                                <%-- FIN Panel Agregar/Modificar Item --%>

                                <div style="clear: both; margin: 2px; max-height: 180px; overflow: auto;">

                                    <%-- Grilla Detalle Items--%>
                                    <asp:GridView ID="grdItems" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                                        OnRowDeleting="grdItems_RowDeleting"
                                        OnRowEditing="grdItems_RowEditing"
                                        OnRowDataBound="grdItems_RowDataBound"
                                        EnableViewState="true"
                                        AllowPaging="false"
                                        CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed"
                                        EnableTheming="false">
                                        <Columns>
                                            <asp:BoundField DataField="Id" HeaderText="Id Det." InsertVisible="True" ReadOnly="True"
                                                SortExpression="Id" AccessibleHeaderText="DetailId" />
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
                                            <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CategoryItem" Visible="true">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCategoryItem" runat="server" Text='<%# Eval ("CategoryItem.Name") %>' />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty" SortExpression="ItemQty">
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

                                            <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="LpnCode">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblLpnCode" runat="server" Text='<%# Eval ("LpnCode") %>' />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="Weight">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblWeight" runat="server" Text='<%# GetFormatedNumber(Eval ("Weight")) %>' />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Acciones">
                                                <ItemTemplate>
                                                    <center>
                                                        <div style="width: 60px">
                                                            <asp:ImageButton ID="btnEdit" runat="server"  CommandName="Edit" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                            ToolTip="Editar Detalle"/>                                            
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
                                    <%-- FIN Grilla Detalle Items--%>
                                </div>
                            </div>
                        </div>
                        <%-- FIN Panel Detalle Documento --%>

                        <div id="divActions" runat="server" class="modalActions">
                            <div id="inboundAviso" runat="server" visible="false" class="divGrid" style="color: red;">Para continuar primero debe ingresar una o mas lineas
                             </div>
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
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
                <asp:Panel ID="pnlLookupItem" Width="500px" runat="server" CssClass="modalBox">

                    <asp:Panel ID="pnlHeadBar2" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblAddItem" runat="server" Text="Buscar Item" />
                            <asp:ImageButton ID="ImageButton2" runat="server" ToolTip="Cerrar" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
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
                        <%--<div class="divCtrsFloatLeft" >--%>
                        <div class="divLookupGrid" style="max-height: 400px; overflow: auto;">
                            <asp:GridView ID="grdSearchItems" runat="server" DataKeyNames="Id"
                                OnRowCommand="grdSearchItems_RowCommand" AllowPaging="true"
                                AutoGenerateColumns="False"
                                OnRowDataBound="grdSearchItems_RowDataBound"
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
                                                    <asp:ImageButton ID="imgBtnAddItem" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                        Width="20px" />
                                                </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <%-- </div>--%>
                        </div>
                        <div style="clear: both" />
                    </div>

                </asp:Panel>
            </div>
            <%-- FIN Lookup Items --%>

            <%-- Panel Cerrar Documento --%>
            <div id="divCloseOrder" runat="server" visible="false">
                <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpCloseOrder" runat="server" TargetControlID="btnDummy2"
                    PopupControlID="pnlCloseOrder" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlCloseOrder" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblCloseOrder" runat="server" Text="Cerrar Orden" />
                            <asp:ImageButton ID="btnClose" runat="server" ToolTip="Cerrar ventana" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                            <div id="div3" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label2" runat="server" Text="Centro Dist." />
                                </div>
                                <div class="fieldLeft">
                                    <b>
                                        <asp:Label ID="lblWarehouse2" runat="server" /></b>
                                </div>
                            </div>
                            <div id="div4" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label3" runat="server" Text="Nº Doc." />
                                </div>
                                <div class="fieldLeft">
                                    <b>
                                        <asp:Label ID="lblNroDoc2" runat="server" /></b>
                                </div>
                            </div>
                            <div id="div5" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label4" runat="server" Text="Tipo Doc." />
                                </div>
                                <div class="fieldLeft">
                                    <b>
                                        <asp:Label ID="lblInboundType2" runat="server" /></b>
                                </div>
                            </div>
                            <div id="div6" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label6" runat="server" Text="Proveedor" />
                                </div>
                                <div class="fieldLeft">
                                    <b>
                                        <asp:Label ID="lblVendor2" runat="server" /></b>
                                </div>
                            </div>
                            <div id="div2" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTraza" runat="server" Text="Traza" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTrackInbound" runat="server" TabIndex="9">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvTrackInbound" runat="server" ValidationGroup="Close" Text=" * " ControlToValidate="ddlTrackInbound" Display="dynamic" InitialValue="-1" />
                                </div>
                            </div>
                        </div>
                        <div>
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"
                                ShowMessageBox="true" CssClass="modalValidation" />
                        </div>
                        <div id="div1" runat="server" class="modalActions">
                            <asp:Button ID="btnCloseOrder" runat="server" OnClick="btnCloseOrder_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="Close" />
                            <asp:Button ID="btnCloseOrderClose" runat="server" Text="Cancelar" />
                        </div>
                </asp:Panel>
            </div>
            <%-- FIN Panel Cerrar Documento --%>
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
        </Triggers>
    </asp:UpdatePanel>
    <%-- FIN Barra de Estado --%>

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop"
        CssClass="updateProgress" TargetControlID="uprGrid" />
    <%-- FIN Modal Update Progress --%>



    <%-- Carga masiva de documentos --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoad" runat="server" visible="false">
                <asp:Button ID="btnDummyLoad" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpLoad" runat="server" TargetControlID="btnDummyLoad"
                    PopupControlID="panelLoad" BackgroundCssClass="modalBackground" PopupDragHandleControlID="panelCaptionLoad"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelLoad" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionLoad" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label1" runat="server" Text="Carga Masiva de Documentos" />
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Documento%20Entrada.xlsx"
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
                                    <asp:FileUpload ID="uploadFile" runat="server" Width="400px" ValidationGroup="Load" />
                                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo"
                                        ValidationGroup="Load" ControlToValidate="uploadFile"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revUpload" runat="server" ValidationGroup="Load"
                                        ErrorMessage="Cargar solo archivos .xls ó .xlsx" Text="*"
                                        ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$"
                                        ControlToValidate="uploadFile">
                                    </asp:RegularExpressionValidator>

                                </div>
                            </div>
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label8" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft" style="width: 400px; text-align: right;">
                                    <asp:Button ID="btnLoadFile" runat="server" Text="Cargar Archivo" ValidationGroup="Load"
                                        OnClick="btnLoadFile_Click" OnClientClick="showProgress();" />
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
            <asp:PostBackTrigger ControlID="btnLoadFile" />
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprLoad" runat="server" AssociatedUpdatePanelID="upLoad" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprLoad" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLoad" />



    <%-- Mensaje--%>
    <div id="divFondoPopup" style="display: none; position: fixed; top: 0; left: 0; z-index: 400000; width: 100%; height: 100%; background-color: Gray; filter: Alpha(Opacity=40); opacity: 0.5; text-align: center; vertical-align: middle;">
    </div>
    <div id="divMensaje" class="modalBox" style="z-index: 400001; display: none; position: absolute; width: 400px; top: 200px; margin-top: 0;"
        runat="server">

        <div id="divDialogTitleMessage" runat="server" class="modalHeader">
            <div class="divCaption">
                <asp:Label ID="lblDialogTitle" runat="server" />
            </div>
        </div>
        <div id="divPanelMessage" class="divDialogPanel" runat="server">

            <div class="divDialogMessage">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
            </div>
            <div id="divDialogMessage" runat="server" class="divDialogMessage" />
            <div id="divAlert" runat="server" visible="true" class="divDialogButtons">
                <asp:Button ID="btnMessageInfo" runat="server" Text="Aceptar" OnClientClick="return HideMessage();" />
            </div>
        </div>
    </div>

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Documento?" Visible="false" />
    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Adm. Documentos de Entrada" Visible="false" />
    <asp:Label ID="lblValDocRequeride" runat="server" Text="Doc. Salida es requerido" Visible="false" />
    <asp:Label ID="lblValDocExists" runat="server" Text="El Doc. ingresado no es válido." Visible="false" />
    <asp:Label ID="lblExpectedDateError" runat="server" Text="Fecha Esperada, debe ser menor o igual que Fecha de Emisión" Visible="false" />
    <asp:Label ID="lblExpirationDateError" runat="server" Text="Fecha Vencimiento, debe ser menor o igual que Fecha de Emisión" Visible="false" />
    <asp:Label ID="lblValidateQty" runat="server" Text="Debe ser mayor a 0" Visible="false" />
    <asp:Label ID="lblValidateRepeatedItems" runat="server" Text="No deben existir ítems repetidos con mismo ItemCode, CtgCode, LotNumber, ExpirationDate, FabricationDate y LpnCode." Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>

    <%-- Mensajes de advertencia y error Para Carga Masiva --%>
    <asp:Label ID="lblNotFileLoad" runat="server" Text="Seleccione el archivo que desea cargar." Visible="false" />
    <asp:Label ID="lblNotInboundOrderFile" runat="server" Text="No existen ordenes en el archivo." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es valído." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblNotAccessServerFolder" runat="server" Text="No existe acceso al servidor." Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />

    <%-- Div Bloquea Pantalla al Momento de Realizar Carga Masiva --%>
    <div id="divFondoPopupProgress" class="loading" align="center" style="display: none;">
        Realizando Carga Masiva
        <br />
        Espere un momento...<br />
        <br />
        <img src="../../WebResources/Images/Buttons/ajax-loader.gif" alt="" />
    </div>

</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
