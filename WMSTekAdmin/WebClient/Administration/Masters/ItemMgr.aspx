<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="ItemMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.ItemMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content runat="server" ID="content1" ContentPlaceHolderID="MainContent">
    
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;

    function ValidateExtension() {

        var file = document.getElementById('ctl00_MainContent_uploadFile2').value;
        var lblError = document.getElementById("ctl00_MainContent_lblMensaje2");

        if (file == null || file == '') {
            alert('Seleccione el archivo a cargar.');
            return false;
        }
        
        var allowedFiles = [".xls", ".xlsx"];
        var regex = new RegExp("([a-zA-Z0-9\s_\\.\-:])+(" + allowedFiles.join('|') + ")$");
        //(/\.(xls|xlsx)$/i
        if (regex.test(file)) {
            return true;
        } else {
            lblError.innerHTML = "Por favor, cargar archivos con extensiones: <b>" + allowedFiles.join(', ') + "</b>.";
            return false;
        }
    }


    //function showProgress() {
    //    if (document.getElementById('ctl00_MainContent_uploadFile2').value.length > 0) {
    //        document.getElementById("ctl00_MainContent_divFondoPopupProgress").style.display = 'block';
    //        return true;
    //    } else {
    //        return false;
    //    }
    //}

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

        if (document.getElementById('ctl00_MainContent_uploadFile').value.length > 0 &&
            document.getElementById("ctl00_MainContent_ddlOwnerLoad").value > 0) {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modalLoading");
                $('body').append(modal);

                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 3.5, 0);
                var left = Math.max($(window).width() / 2.6, 0);
                loading.css({ top: top, left: left });
            }, 10);
            return true;

        } else {
            return false;
        }
    }
  
    $(document).ready(function () {
        initializeGridDragAndDrop("Item_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop();

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
        initializeGridDragAndDrop("Item_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop();
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">
                            <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" AutoGenerateColumns="False"
                                OnRowCreated="grdMgr_RowCreated" 
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing"
                                OnRowDataBound="grdMgr_RowDataBound"
                                OnPageIndexChanging="grdMgr_PageIndexChanging" AllowPaging="True" EnableViewState="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">                    
                                <Columns>
                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 60px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CommandName="Edit" ToolTip="Editar" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CommandName="Delete" ToolTip="Eliminar" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="Id" AccessibleHeaderText="Id" SortExpression="Id"  />
                                    <asp:BoundField DataField="Code" HeaderText="Cód. Item" AccessibleHeaderText="Code" SortExpression="Code" />
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner" SortExpression="Owner" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="AltCode" HeaderText="Cód. Alternativo" AccessibleHeaderText="AltCode" />
                                    <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                                    <asp:TemplateField HeaderText="Sector" AccessibleHeaderText="GrpItem1">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label runat="server" ID="lblGroupItem1" Text='<%# Bind("GrpItem1.Name") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rubro" AccessibleHeaderText="GrpItem2">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label runat="server" ID="lblGroupItem2" Text='<%# Bind("GrpItem2.Name") %>' />
                                             </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Familia" AccessibleHeaderText="GrpItem3">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label runat="server" ID="lblGroupItem3" Text='<%# Bind("GrpItem3.Name") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Subfamilia" AccessibleHeaderText="GrpItem4">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label runat="server" ID="lblGroupItem4" Text='<%# Bind("GrpItem4.Name") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="LongName" HeaderText="Nombre Largo" AccessibleHeaderText="LongName" />
                                    <asp:BoundField DataField="ShortName" HeaderText="Nombre Corto" AccessibleHeaderText="ShortName" />
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Comment" HeaderText="Comentarios" AccessibleHeaderText="Comment" />
                                    <asp:BoundField DataField="ShelfLife" HeaderText="Días Vigencia" AccessibleHeaderText="ShelfLife" />
                                    <asp:BoundField DataField="Expiration" HeaderText="Días Vencimiento" AccessibleHeaderText="Expiration" />
                                    <asp:TemplateField HeaderText="Entrada con Serie" AccessibleHeaderText="CtrlSerialInbound">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkCtrlSerialInbound" runat="server" Checked='<%# Eval ( "CtrlSerialInbound" ) %>'
                                                    Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Movto. con serie" AccessibleHeaderText="CtrlSerialInternal">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkCtrlSerialInternal" runat="server" Checked='<%# Eval ( "CtrlSerialInternal" ) %>'
                                                    Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Salida con serie" AccessibleHeaderText="CtrlSerialOutbound">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkCtrlSerialOutbound" runat="server" Checked='<%# Eval ( "CtrlSerialOutbound" ) %>'
                                                    Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Movto. con Lote" AccessibleHeaderText="LotControlInternal">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkLotControlInternal" runat="server" Checked='<%# Eval ( "LotControlInternal" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Entrada con Lote" AccessibleHeaderText="LotControlInbound">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkLotControlInbound" runat="server" Checked='<%# Eval ( "LotControlInbound" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Salida con Lote" AccessibleHeaderText="LotControlOutbound">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkLotControlOutbound" runat="server" Checked='<%# Eval ( "LotControlOutbound" ) %>'
                                                Enabled="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Weight" HeaderText="Peso" AccessibleHeaderText="Weight" />
                                    <asp:BoundField DataField="Volume" HeaderText="Volumen" AccessibleHeaderText="Volume" />
                                    <asp:BoundField DataField="Length" HeaderText="Largo" AccessibleHeaderText="Length" />
                                    <asp:BoundField DataField="Width" HeaderText="Ancho" AccessibleHeaderText="Width" />
                                    <asp:BoundField DataField="Height" HeaderText="Alto" AccessibleHeaderText="Height" />
                                    <asp:TemplateField HeaderText="Volumen Apilado" AccessibleHeaderText="NestedVolume">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblNestedVolume" runat="server" Text='<%# ((decimal)Eval("NestedVolume") == -1) ? " " : Eval("NestedVolume")%>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Req. Inspección" AccessibleHeaderText="InspectionRequerid">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkInspectionRequerid" runat="server" Checked='<%# Eval ( "InspectionRequerid" ) %>'
                                                    Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="InspectionCode" HeaderText="Cód. Inspección" AccessibleHeaderText="InspectionCode" />
                                    <asp:TemplateField HeaderText="Zona Almacenaje" AccessibleHeaderText="PutawayZone">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label runat="server" ID="lblPutawayZone" Text='<%# Bind("PutawayZone.Name") %>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ctrl. Vencimiento" AccessibleHeaderText="CtrlExpiration">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkCtrlExpiration" runat="server" Checked='<%# Eval ( "CtrlExpiration" ) %>'
                                                    Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ctrl. Fabricacion" AccessibleHeaderText="CtrlFabrication">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkCtrlFabrication" runat="server" Checked='<%# Eval ( "CtrlFabrication" ) %>'
                                                    Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Acumulable" AccessibleHeaderText="Acumulable">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkAcumulable" runat="server" Checked='<%# Eval ( "Acumulable" ) %>'
                                                    Enabled="false" />
                                                 </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mín. Reposición" AccessibleHeaderText="ReOrderPoint">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblReOrderPoint" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval("ReOrderPoint") == -1) ? " " : Eval("ReOrderPoint"))%>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Máx. Reposición" AccessibleHeaderText="ReOrderQty">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblReOrderQty" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval("ReOrderQty") == -1) ? " " : Eval("ReOrderQty"))%>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cantidad por LPN" AccessibleHeaderText="PalletQty">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblPalletQty" runat="server" Text='<%# (((int)Eval("PalletQty") == -1) ? " " : Eval("PalletQty"))%>' />
                                               </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mín. Corte" AccessibleHeaderText="CutMinimum">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCutMinimum" runat="server" Text='<%# ((int)Eval("CutMinimum") == -1) ? " " : Eval("CutMinimum")%>' />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Originator" HeaderText="Origen del producto" AccessibleHeaderText="Originator" />
                                    <asp:BoundField DataField="VasProfile" HeaderText="Proceso VAS" AccessibleHeaderText="VasProfile" />
                                    <asp:TemplateField HeaderText="Precaución" AccessibleHeaderText="Hazard">
                                        <ItemTemplate>
                                            <div>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:CheckBox ID="chkHazard" runat="server" Checked='<%# Eval ( "Hazard" ) %>' Enabled="false" />
                                                    </div>
                                                </center>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Precio" AccessibleHeaderText="Price">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblPrecio" runat="server" Text='<%# ((decimal)Eval("Price")== -1) ? " ":Eval("Price")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="InventoryType" HeaderText="Tipo inventario" AccessibleHeaderText="InventoryType" />
                                    <asp:TemplateField HeaderText="Presentación por Defecto" AccessibleHeaderText="SpecialField4">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lbllItemUom" runat="server" Text='<%# Eval ( "SpecialField4" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ctrl. Comentario" AccessibleHeaderText="CommentControl">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkCommentControl" runat="server" Checked='<%# Eval ( "CommentControl" ) %>'
                                                    Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CompatibilyCode" HeaderText="Cód. Compatibilidad" AccessibleHeaderText="CompatibilyCode" />
                                    <asp:BoundField DataField="MsdsUrl" HeaderText="Url de hoja de seguridad" AccessibleHeaderText="MsdsUrl" />
                                    <asp:BoundField DataField="PictureUrl" HeaderText="Url de Foto" AccessibleHeaderText="PictureUrl" />
                                </Columns>
                                <EmptyDataTemplate>
                                    <asp:Label ID="lblEmptyGrid" runat="server" Text="No se han encontrado registros." />
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                        <%-- FIN Grilla Principal --%>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop"
        CssClass="updateProgress" TargetControlID="uprGrid" />
        
    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="PnlItem" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="PnlItem" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Item" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Item" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidEditIndex" runat="server" Value="-1" />
                        <ajaxToolkit:TabContainer runat="server" ID="tabItem" ActiveTabIndex="0">
                            <ajaxToolkit:TabPanel runat="server" ID="tabGeneral">
                                <ContentTemplate>
                                    <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
                                        <div id="divStatus" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblStatus" runat="server" Text="Activo" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkStatus" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divOwner" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList runat="server" ID="ddlOwner" AutoPostBack="True" 
                                                    OnSelectedIndexChanged="ddlOwner_SelectedIndexChanged">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwner"
                                                    InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />
                                            </div>
                                        </div>
                                        <div id="divCode" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCode" runat="server" Text="Código" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtCode" runat="server" MaxLength="30" />
                                                <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
                                               <%-- <asp:RegularExpressionValidator ID="revCode" runat="server" ControlToValidate="txtCode"
	                                                 ErrorMessage="Código permite ingresar solo caracteres alfanuméricos" 
	                                                 ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                                 ValidationGroup="EditNew" Text=" * "></asp:RegularExpressionValidator>         --%>                                           
                                            </div>
                                        </div>
                                        <div id="divAltCode" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblAltCode" runat="server" Text="Cód. Alternativo" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtAltCode" runat="server" MaxLength="30" />
                                                <asp:RequiredFieldValidator ID="rfvAltCode" runat="server" ControlToValidate="txtAltCode"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Cód. Alternativo es requerido" />
                                               <%-- <asp:RegularExpressionValidator ID="revtxtAltCode" runat="server" ControlToValidate="txtAltCode"
	                                                 ErrorMessage="Cód. Alternativo permite ingresar solo caracteres alfanuméricos" 
	                                                 ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                                 ValidationGroup="EditNew" Text=" * "></asp:RegularExpressionValidator>--%>                                                          
                                            </div>
                                        </div>
                                        <div id="divDescription" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDescription" runat="server" Text="Descripción" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtDescription" runat="server" MaxLength="255" />
                                                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Descripción es requerido" />
                                            </div>
                                        </div>
                                        <div id="divLongName" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLongName" runat="server" Text="Nombre Largo" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtLongName" runat="server" MaxLength="60" />
                                                <asp:RequiredFieldValidator ID="rfvLongName" runat="server" ControlToValidate="txtLongName"
                                                    ErrorMessage="Nombre Largo es requerido" ValidationGroup="EditNew" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divShortName" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblShortName" runat="server" Text="Nombre Corto" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtShortName" runat="server" MaxLength="20" />
                                                <asp:RequiredFieldValidator ID="rfvShortName" runat="server" ControlToValidate="txtShortName"
                                                    ErrorMessage="Nombre Corto es requerido" ValidationGroup="EditNew" Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divPrice" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPrice" runat="server" Text="Precio" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPrice" runat="server" MaxLength="13" />
                                                <asp:RequiredFieldValidator ID="rfvPrice" runat="server" ControlToValidate="txtPrice"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Precio es requerido" />
                                                <asp:RangeValidator ID="rvtxtPrice" runat="server" ErrorMessage="Precio no contiene un número válido" 
                                                    ControlToValidate="txtPrice" MinimumValue="0" MaximumValue="99999999" Type="Double" Text=" * "></asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divCompatibilyCode" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCompatibilyCode" runat="server" Text="Cód. Compatibilidad" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtCompatibilyCode" runat="server" MaxLength="5" />
                                                <asp:RequiredFieldValidator ID="rfvCompatibilyCode" runat="server" ControlToValidate="txtCompatibilyCode"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Cód. Compatibilidad es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtCompatibilyCode" runat="server" ControlToValidate="txtCompatibilyCode"
	                                                 ErrorMessage="Cód. Compatibilidad permite ingresar solo caracteres alfanuméricos" 
	                                                 ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                                 ValidationGroup="EditNew" Text=" * "></asp:RegularExpressionValidator>                                                
                                            </div>
                                        </div>
                                        
                                        <div id="divComment" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblComment" runat="server" Text="Comentarios" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtComment" runat="server" MaxLength="5" TextMode="MultiLine" />
                                                <asp:RequiredFieldValidator ID="rfvComment" runat="server" ControlToValidate="txtComment"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Comentario es requerido" />
                                                <asp:RegularExpressionValidator ID="revTexbox3" runat="server"             
                                                       ErrorMessage="En Comentario: debe ingresar hasta un maximo de 100 caracteres"     
                                                       Text=" * "      
                                                       ValidationExpression="^([\S\s]{0,100})$"             
                                                       ControlToValidate="txtComment"            
                                                       Display="Dynamic"
                                                       ValidationGroup="EditNew"></asp:RegularExpressionValidator>                                                                                                 
                                            </div>
                                        </div>
                                    </div>
                                    <div class="divCtrsFloatLeft">
                                        <asp:UpdatePanel ID="upItemGroup" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div id="divGrpItem1" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblGrpItem1" runat="server" Text="Sector" /></div>
                                                    <div class="fieldLeft">
                                                        <asp:DropDownList ID="ddlGrpItem1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlGrpItem1_SelectedIndexChanged" />
                                                        <asp:RequiredFieldValidator ID="rfvGrpItem1" runat="server" InitialValue="-1" ControlToValidate="ddlGrpItem1"
                                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Sector es requerido" />
                                                    </div>
                                                </div>
                                                <div id="divGrpItem2" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblGrpItem2" runat="server" Text="Rubro" /></div>
                                                    <div class="fieldLeft">
                                                        <asp:DropDownList ID="ddlGrpItem2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlGrpItem2_SelectedIndexChanged" />
                                                        <asp:RequiredFieldValidator ID="rfvGrpItem2" runat="server" InitialValue="-1" ControlToValidate="ddlGrpItem2"
                                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Rubro es requerido" />
                                                    </div>
                                                </div>
                                                <div id="divGrpItem3" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblGrpItem3" runat="server" Text="Familia" />
                                                    </div>
                                                    <div class="fieldLeft">
                                                        <asp:DropDownList ID="ddlGrpItem3" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlGrpItem3_SelectedIndexChanged" />
                                                        <asp:RequiredFieldValidator ID="rfvGrpItem3" runat="server" InitialValue="-1" ControlToValidate="ddlGrpItem3"
                                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Familia es requerido" />
                                                    </div>
                                                </div>
                                                <div id="divGrpItem4" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblGrpItem4" runat="server" Text="Subfamilia" />
                                                    </div>
                                                    <div class="fieldLeft">
                                                        <asp:DropDownList ID="ddlGrpItem4" runat="server" />
                                                        <asp:RequiredFieldValidator ID="rfvGrpItem4" runat="server" InitialValue="-1" ControlToValidate="ddlGrpItem4"
                                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Subfamilia es requerido" />
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <div id="divShelfLife" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblShelfLife" runat="server" Text="Dias Vigencia" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtShelfLife" runat="server" MaxLength="9" />
                                                <asp:RequiredFieldValidator ID="rfvShelfLife" runat="server" ControlToValidate="txtShelfLife"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Días Vigencia es requerido" />
                                               <asp:RangeValidator ID="rvtxtShelfLife" runat="server" ErrorMessage="Dias Vigencia no contiene un número válido" ValidationGroup="EditNew"
                                                    ControlToValidate="txtShelfLife" MinimumValue="0" MaximumValue="2147483647" Type="Integer" Text=" * "></asp:RangeValidator>                                                    
                                            </div>
                                        </div>
                                        <div id="divExpiration" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblExpiration" runat="server" Text="Días Vencimiento" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtExpiration" runat="server" MaxLength="9" />
                                                <asp:RequiredFieldValidator ID="rfvExpiration" runat="server" ControlToValidate="txtExpiration"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Días Vencimento es requerido" />
                                               <asp:RangeValidator ID="rvtxtExpiration" runat="server" ErrorMessage="Días Vencimiento no contiene un número válido" ValidationGroup="EditNew"
                                                    ControlToValidate="txtExpiration" MinimumValue="0" MaximumValue="2147483647" Type="Integer" Text=" * "></asp:RangeValidator>                                                     
                                            </div>
                                        </div>
                                        <div id="divWeight" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWeight" runat="server" Text="Peso" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtWeight" runat="server" MaxLength="13" />
                                                <asp:Label ID="lblTypeUnitOfMass" runat="server"></asp:Label>
                                                <asp:RequiredFieldValidator ID="rfvWeight" runat="server" ControlToValidate="txtWeight"
                                                     ValidationGroup="EditNew" Text=" * " ErrorMessage="Peso es requerido" />
                                                <asp:RangeValidator ID="rvtxtWeight" runat="server" ControlToValidate="txtWeight" ErrorMessage="Peso no contiene un número válido"
                                                    MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>       
                                            </div>
                                        </div>
                                        <div id="divVolume" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblVolume" runat="server" Text="Volumen" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtVolume" runat="server" MaxLength="13" />
                                                <asp:Label ID="lblTypeUnitOfMass2" runat="server"></asp:Label>
                                                <asp:RequiredFieldValidator ID="rfvVolume" runat="server" ControlToValidate="txtVolume"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Volumen es requerido" />
                                                <asp:RangeValidator ID="rvVolume" runat="server" ControlToValidate="txtVolume" ErrorMessage="Volumen no contiene un número válido"
                                                    MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divLength" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLength" runat="server" Text="Largo" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox name="txtLength" ID="txtLength" runat="server" MaxLength="13" />
                                                <asp:Label ID="lblTypeUnitMeasure" runat="server"></asp:Label>
                                                <asp:RequiredFieldValidator ID="rfvLength" runat="server" ControlToValidate="txtLength"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Largo es requerido" />
                                                <asp:RangeValidator ID="rvLength" runat="server" ControlToValidate="txtLength" ErrorMessage="Largo no contiene un número válido"
                                                    MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double"
                                                    Text=" * " />
                                            </div>
                                        </div>
                                        <div id="divWidth" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblWidth" runat="server" Text="Ancho" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtWidth" runat="server" MaxLength="13" />
                                                <asp:Label ID="lblTypeUnitMeasure2" runat="server"></asp:Label>
                                                <asp:RequiredFieldValidator ID="rfvWidth" runat="server" ControlToValidate="txtWidth"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Ancho es requerido" />
                                                <asp:RangeValidator ID="rvWidth" runat="server" ControlToValidate="txtWidth" ErrorMessage="Ancho no contiene un número válido"
                                                    MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divHeight" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblHeight" runat="server" Text="Alto" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtHeight" runat="server" MaxLength="13" />
                                                <asp:Label ID="lblTypeUnitMeasure3" runat="server"></asp:Label>
                                                <asp:RequiredFieldValidator ID="rfvHeight" runat="server" ControlToValidate="txtHeight"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Alto es requerido" />
                                                <asp:RangeValidator ID="rvHeight" runat="server" ControlToValidate="txtHeight" ErrorMessage="Alto no contiene un número válido"
                                                    MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divNestedVolume" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblNestedVolume" runat="server" Text="Volumen Apilado" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtNestedVolume" runat="server" MaxLength="13"/>
                                                    
                                                <asp:RequiredFieldValidator ID="rfvNestedVolume" runat="server" ControlToValidate="txtNestedVolume"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Volumen Apilado es requerido" />
                                                <asp:RangeValidator ID="rvNestedVolume" runat="server" ControlToValidate="txtNestedVolume"
                                                    ErrorMessage="Volumen Apilado no contiene un número válido" MaximumValue="99999999"
                                                    MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="divValidationSummary">
                                        <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"
                                            ShowMessageBox="True" CssClass="modalValidation" />
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            <ajaxToolkit:TabPanel runat="server" ID="tabDetails">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divReOrderPoint" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblReOrderPoint" runat="server" Text="Mín. Reposición" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtReOrderPoint" runat="server" MaxLength="13" />
                                                <asp:RequiredFieldValidator ID="rfvReOrderPoint" runat="server" ControlToValidate="txtReOrderPoint"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Mín. Reposición es requerido" />
                                                <asp:RangeValidator ID="rvReOrderPoint" runat="server" ControlToValidate="txtReOrderPoint"
                                                    ErrorMessage="Mín. Reposición no contiene un número válido" MaximumValue="1000000"
                                                    MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divReOrderQty" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblReOrderQty" runat="server" Text="Máx. Reposición" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtReOrderQty" runat="server" MaxLength="13" />
                                                <asp:RequiredFieldValidator ID="rfvReOrderQty" runat="server" ControlToValidate="txtReOrderQty"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Máx. Reposición es requerido" />
                                                <asp:CompareValidator ID="cvReOrderQty" runat="server" ControlToValidate="txtReOrderQty" 
                                                    ControltoCompare="txtReOrderPoint" Operator="GreaterThanEqual" ValidationGroup="EditNew" 
                                                    Type="Double" errormessage="Valor Maximo debe ser mayor que valor Minimo">*</asp:CompareValidator>
                                                <asp:RangeValidator ID="rvReOrderQty" runat="server" ControlToValidate="txtReOrderQty"
                                                    ErrorMessage="Máx. Reposición no contiene un número válido" MaximumValue="1000000"
                                                    MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divPalletQty" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPalletQty" runat="server" Text="Cantidad por LPN" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPalletQty" runat="server" MaxLength="9" />
                                                <asp:RequiredFieldValidator ID="rfvPalletQty" runat="server" ControlToValidate="txtPalletQty"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Cantidad por LPN es requerido" />
                                                <asp:RangeValidator ID="rvPalletQty" runat="server" ControlToValidate="txtPalletQty"
                                                    ErrorMessage="Cantidad por LPN  no contiene un número válido" MaximumValue="2147483640"
                                                    MinimumValue="0" ValidationGroup="EditNew" Type="Integer">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divCutMinimum" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCutMinimum" runat="server" Text="Minimo de corte" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtCutMinimum" runat="server" MaxLength="9" />
                                                <asp:RequiredFieldValidator ID="rfvCutMinimum" runat="server" ControlToValidate="txtCutMinimum"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Minimo de Corte es requerido" />
                                                <asp:RangeValidator ID="rvCutMinimum" runat="server" ControlToValidate="txtCutMinimum"
                                                    ErrorMessage="Minimo de Corte no contiene un número válido" MaximumValue="2147483640"
                                                    MinimumValue="0" ValidationGroup="EditNew" Type="Integer">*</asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divOriginator" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOriginator" runat="server" Text="Origen" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtOriginator" runat="server" MaxLength="30" />
                                                <asp:RequiredFieldValidator ID="rfvOriginator" runat="server" ControlToValidate="txtOriginator"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Origen es requerido" />
                                            </div>
                                        </div>
                                        <div id="divVASProfile" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblVASProfile" runat="server" Text="Proceso VAS" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtVASProfile" runat="server" MaxLength="10" />
<%--                                                <asp:RequiredFieldValidator ID="rfvVASProfile" runat="server" ControlToValidate="txtVASProfile"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Proceso Vas es requerido" />--%>
                                                <asp:RegularExpressionValidator ID="revtxtVASProfile" runat="server" ControlToValidate="txtVASProfile"
	                                                 ErrorMessage="Proceso Vas permite solo letras de la A - Z o a - z ó números" 
	                                                 ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                                 ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>                                                   
                                            </div>
                                        </div>
                                        <div id="divInventoryType" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblInventoryType" runat="server" Text="Tipo Inventario" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtInventoryType" runat="server" MaxLength="5" />
                                                <asp:RequiredFieldValidator ID="rfvInventoryType" runat="server" ControlToValidate="txtInventoryType"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo Inventario es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtInventoryType" runat="server" ControlToValidate="txtInventoryType"
	                                                 ErrorMessage="Tipo Inventario permite solo letras de la A - Z o a - z ó números" 
	                                                 ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                                 ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator>                                                    
                                            </div>
                                        </div>
                                        <div id="divDefaultUomType" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDefaultUomType" runat="server" Text="Presentación por Defecto" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlUomType" runat="server" />
                                                <%--<asp:RequiredFieldValidator ID="rfvItemUom" runat="server" InitialValue="-1" ControlToValidate="ddlUomType"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Presentación por Defecto es requerido" />--%>
                                            </div>
                                        </div>
                                        <div id="divInspectionCode" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblInspectionCode" runat="server" Text="Cód. Inspección" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlInspectionCode" runat="server"/>
                                                <asp:RequiredFieldValidator ID="rfvInspectionCode" runat="server" InitialValue="-1" ControlToValidate="ddlInspectionCode"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Cód. Inspección" />
                                                <%--<asp:TextBox ID="txtInspectionCode" runat="server" MaxLength="5" />
                                                <asp:RequiredFieldValidator ID="rfvInspectionCode" runat="server" ControlToValidate="txtInspectionCode"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Cód. Inspección es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtInspectionCode" runat="server" ControlToValidate="txtInspectionCode"
	                                                 ErrorMessage="Cód. Inspección permite ingresar solo caracteres alfanuméricos" 
	                                                 ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                                 ValidationGroup="EditNew" Text=" * "></asp:RegularExpressionValidator>  --%>                                                     
                                            </div>
                                        </div>
                                        <div id="divMSDSUrl" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblMSDSUrl" runat="server" Text="Url MSD" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtMSDSUrl" runat="server" MaxLength="20" />
<%--                                                <asp:RequiredFieldValidator ID="rfvMSDSUrl" runat="server" ControlToValidate="txtMSDSUrl"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Url MSD es requerido" />--%>
                                            </div>
                                        </div>
                                        <div id="divPictureUrl" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPictureUrl" runat="server" Text="Url Foto" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPictureUrl" runat="server" MaxLength="20" />
                                                <asp:RequiredFieldValidator ID="rfvPictureUrl" runat="server" ControlToValidate="txtPictureUrl"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="URL Foto es requerido" />
                                            </div>
                                        </div>
                                        <%--<div id="divPutawayZone" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPutawayZone" runat="server" Text="Zona Almacenaje" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlPutawayZone" runat="server" />
                                                <asp:RequiredFieldValidator ID="rfvPutawayZone" runat="server" InitialValue="-1"
                                                    ControlToValidate="ddlPutawayZone" ValidationGroup="EditNew" Text=" * " ErrorMessage="Zona de Almacenaje es requerido" />
                                            </div>
                                        </div>--%>
                                    </div>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divCtrlSerialInbound" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCtrlSerialInbound" runat="server" Text="Entrada con Serie" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkCtrlSerialInbound" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divCtrlSerialInternal" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCtrlSerialInternal" runat="server" Text="Movto. con serie" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkCtrlSerialInternal" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divCtrlSerialOutbound" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCtrlSerialOutbound" runat="server" Text="Salida con serie" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkCtrlSerialOutbound" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divLotControlInternal" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLotControlInternal" runat="server" Text="Movto. con Lote" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkLotControlInternal" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divLotControlInbound" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLotControlInbound" runat="server" Text="Entrada con Lote" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkLotControlInbound" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divLotControlOutbound" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLotControlOutbound" runat="server" Text="Salida con Lote" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkLotControlOutbound" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divCtrlExpiration" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCtrlExpiration" runat="server" Text="Ctrl. Vencimiento" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkCtrlExpiration" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divCtrlFabrication" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCtrlFabrication" runat="server" Text="Ctrl. Fabricacion" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkCtrlFabrication" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divInspectionRequerid" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblInspectionRequerid" runat="server" Text="Req. Inspección" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkInspectionRequerid" runat="server" 
                                                AutoPostBack="true" OnCheckedChanged="chkInspectionRequerid_CheckedChanged" />
                                            </div>
                                        </div>
                                        <div id="divAcumulable" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblAcumulable" runat="server" Text="Acumulable" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkAcumulable" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divHazard" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblHazard" runat="server" Text="Precaución" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkHazard" runat="server" />
                                            </div>
                                        </div>
                                        <div id="divCommentControl" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCommentControl" runat="server" Text="Ctrl. Comentario" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkCommentControl" runat="server" Enabled="true" />
                                            </div>
                                        </div>
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            <ajaxToolkit:TabPanel runat="server" ID="tabWorkZones">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div class="divControls">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlWorkZones" runat="server" />
                                                <asp:Button ID="btnAddWorkZone" runat="server" Text="Asignar" OnClick="btnAddWorkZone_Click" />
                                            </div>
                                            <div class="textLeft">
                                                <asp:Label ID="Label2" runat="server" Text="Zonas Asignadas:" />
                                                <br />
                                                <%-- Grilla de WorkZones asignadas al Item --%>
                                                <asp:GridView ID="grdWorkZones" runat="server" DataKeyNames="Id" ShowFooter="false"
                                                    OnRowDeleting="grdWorkZones_RowDeleting" OnRowCreated="grdWorkZones_RowCreated"
                                                    AllowPaging="False"
                                                    AutoGenerateColumns="False"
                                                    OnRowDataBound="grdWorkZones_RowDataBound"
                                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                    EnableTheming="false">
                                                    <Columns>
                                                        <asp:BoundField DataField="Id" ShowHeader="false" Visible="false" />
                                                        <asp:BoundField DataField="Name" HeaderText="Zona" AccessibleHeaderText="Name" />
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <center>
                                                                    <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                                                        ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" />
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                <%-- FIN Grilla de WorkZones asignadas al Item --%>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            <ajaxToolkit:TabPanel runat="server" ID="tabRules" Visible= "true">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div class="divControls">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlRules" runat="server" />
                                                <asp:Button ID="btnAddRules" runat="server" Text="Asignar" OnClick="btnAddRules_Click" />
                                            </div>
                                            <div class="textLeft">
                                                <asp:Label ID="Label1" runat="server" Text="Grupo de Reglas Asignadas:" />
                                                <br />
                                                <%-- Grilla de WorkZones asignadas al Item --%>
                                                <asp:GridView ID="grdCustomRules" runat="server" DataKeyNames="Id" ShowFooter="false"
                                                    OnRowDeleting="grdCustomRules_RowDeleting" OnRowCreated="grdCustomRules_RowCreated"
                                                    AllowPaging="False" 
                                                    AutoGenerateColumns="False"
                                                    OnRowDataBound="grdCustomRules_RowDataBound"
                                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                    EnableTheming="false">
                                                    <Columns>
                                                        <asp:BoundField DataField="Id" ShowHeader="false" Visible="false" />
                                                        <asp:BoundField DataField="Name" HeaderText="Grupo" AccessibleHeaderText="Name" />
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <center>
                                                                    <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                                                        ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" />
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                <%-- FIN Grilla de WorkZones asignadas al Item --%>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            <ajaxToolkit:TabPanel runat="server" ID="tabVas">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div class="divControls">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlVas" runat="server" />
                                                <asp:Button ID="btnAddVas" runat="server" Text="Asignar" OnClick="btnAddVas_Click" />
                                            </div>
                                            <div class="textLeft">
                                                <asp:Label ID="lblVasAsignado" runat="server" Text="Vas Asignados:" />
                                                <br />
                                                <asp:GridView ID="grdVas" runat="server" DataKeyNames="Id" ShowFooter="false" OnRowDeleting="grdVas_RowDeleting" OnRowCreated="grdVas_RowCreated" AllowPaging="false"
                                                    AutoGenerateColumns="False"
                                                    OnRowDataBound="grdVas_RowDataBound"
                                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                    EnableTheming="false">
                                                    <Columns>
                                                        <asp:BoundField DataField="Id" ShowHeader="false" Visible="false" />
                                                        <asp:BoundField DataField="Name" HeaderText="Vas" AccessibleHeaderText="VasName" />
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <center>
                                                                    <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="false" CommandName="Delete" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" />
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="clear: both">
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                        </ajaxToolkit:TabContainer>

                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" CausesValidation="true" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo --%>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnCancel" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$tabItem$tabDetails$chkInspectionRequerid" EventName="CheckedChanged" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />
    
    
    <%-- Carga masiva de items --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoad" runat="server" visible="false">
                <asp:Button ID="btnDummyLoad" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpLoad" runat="server" TargetControlID="btnDummyLoad"
                    PopupControlID="panelLoad" BackgroundCssClass="modalBackground" PopupDragHandleControlID="panelCaptionLoad"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelLoad" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionLoad" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label3" runat="server" Text="Carga Masiva de Items" />
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Items.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />   
                            <asp:ImageButton ID="ImageButton1" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                            <div id="div2" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label4" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlOwnerLoad" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvOwnerLoad" runat="server" ControlToValidate="ddlOwnerLoad"
                                        InitialValue="-1" ValidationGroup="Load" Text=" * " ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>
                        
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label5" runat="server" Text="Seleccione Archivo" />
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
                                    <asp:Label ID="Label6" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnSubir2" runat="server" Text="Cargar Archivo" ValidationGroup="Load" 
                                    OnClientClick="showProgress()" onclick="btnSubir2_Click" />
                                </div>
                            </div>
                           <%-- <div style="clear: both">
                            </div>--%>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load"
                                ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div1" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo --%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubir2" />
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprLoad" runat="server" AssociatedUpdatePanelID="upLoad" DisplayAfter="20" 
     DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprLoad" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLoad" />
    
   <%-- <div id="divFondoPopupProgress" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;" runat="server">
        <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoadNew" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
    </div>--%>
    
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
    
    <%-- Mensajes de advertencia y auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Item?"
        Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre Largo" Visible="false" />
    <asp:Label ID="lbltabGeneral" runat="server" Text="Datos Generales" Visible="false" />    
    <asp:Label ID="lbltabDetails" runat="server" Text="Detalles" Visible="false" />    
    <asp:Label ID="lbltabWorkZone" runat="server" Text="Zona" Visible="false" />    
    <asp:Label ID="lbltabRules" runat="server" Text="Reglas" Visible="false" />  
    <asp:Label ID="lbltabVas" runat="server" Text="Vas" Visible="false" />
    <asp:Label ID="lblAddLoadToolTip" runat="server" Text="Carga Masiva" Visible="false" />    
    
    <%-- Messaje Errors--%>
    <asp:Label ID="lblTitle" runat="server" Text="Mantenedor de Items" Visible="false" />
    <asp:Label ID="lblNotFileLoad" runat="server" Text="Seleccione el archivo que desea cargar." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen items en el archivo." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es valído." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblNotAccessServerFolder" runat="server" Text="No existe acceso al servidor." Visible="false" />   
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
    <%-- FIN Mensajes de advertencia y auxiliares --%>


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
