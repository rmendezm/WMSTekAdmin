<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="MaxAndMinByLocationMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Administration.MaxAndMinByLocationMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc1" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("ItemLocation_MaxAnMinByLocation", "ctl00_MainContent_grdMgr");
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
        initializeGridDragAndDrop("ItemLocation_MaxAnMinByLocation", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop(true);
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

    function ShowMessage(title, message) {
        var position = (document.body.clientWidth - 400) / 2 + "px";
        document.getElementById("divFondoPopup").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.marginLeft = position;

        document.getElementById("ctl00_MainContent_lblDialogTitle").innerHTML = title;
        document.getElementById("ctl00_MainContent_divDialogMessage").innerHTML = message;

        return false;
    }

    function HideMessage() {
        document.getElementById("divFondoPopup").style.display = 'none';
        document.getElementById("ctl00_MainContent_divMensaje").style.display = 'none';
        return false;
    }
</script>

<%--<div id="divPrincipal" style="margin:0px;margin-bottom:80px">--%>
    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >  <%--onresize="SetDivs();">--%>
                            <asp:GridView 
                                ID="grdMgr" 
                                runat="server" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing" 
                                OnPageIndexChanging="grdMgr_PageIndexChanging"
                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                OnRowCreated="grdMgr_RowCreated"
                                AllowPaging="True" 
                                EnableViewState="False" 
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="IdWhs" AccessibleHeaderText="IdWhs">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdWhs" runat="server" Text='<%# Eval ( "Warehouse.Id" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>     
                                           
                                    <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="IdOwn" AccessibleHeaderText="IdOwn">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ( "Owner.Id" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                             
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Código Item" AccessibleHeaderText="ItemCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "Item.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                       
                                    <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="IdItem">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdItem" runat="server" Text='<%# Eval ( "Item.Id" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                              
                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ( "Item.LongName" ) %>' />
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fila" AccessibleHeaderText="RowLoc">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblRowLoc" runat="server" Text='<%# Eval ( "Location.Row" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Columna" AccessibleHeaderText="ColumnLoc">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblColumnLoc" runat="server" Text='<%# Eval ( "Location.Column" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nivel" AccessibleHeaderText="LevelLoc">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLevelLoc" runat="server" Text='<%# Eval ( "Location.Level" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>                        
                                    <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ( "Location.IdCode" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>  
                        
                                    <asp:TemplateField HeaderText="Mínimo" AccessibleHeaderText="ReOrderPoint">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblReOrderPoint" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval("ReOrderPoint") == -1) ? " " : Eval("ReOrderPoint"))%>' />
                                            </div>                                                                               
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Máximo" AccessibleHeaderText="ReOrderQty">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblReOrderQty" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval("ReOrderQty") == -1) ? " " : Eval("ReOrderQty"))%>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <div style="width: 60px">
                                                <center>
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CommandName="Edit" ToolTip="Editar" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CommandName="Delete" ToolTip="Eliminar" />
                                                </center>
                                            </div>
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
            <div id="divUpNew" runat="server" visible="false">
                <asp:Panel ID="pnlPanelEditNew" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <%--<asp:Panel ID="pnlUser" runat="server" CssClass="modalBox">--%>
                        <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                            <div class="divCaption">
                                <asp:Label ID="lblNew" runat="server" Text="Nuevo máximo y mínimo" />
                                <asp:Label ID="lblEdit" runat="server" Text="Editar máximo y mínimo" />
                                <asp:ImageButton ID="btnClose" runat="server" Visible="false" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" ToolTip="Cerrar" TabIndex="9" OnClick="btnCancel_Click"/>
                            </div>
                        </asp:Panel>
                        
                        <div class="modalBoxContent">
                        <div class="divCtrsFloatLeft">
                            <%--<div class="modalControls">--%>
                                <asp:HiddenField ID="hidEditIdLoc" runat="server" Value="-1" />
                                <asp:HiddenField ID="hidEditIdItem" runat="server" Value="-1" />
                            <%--<div class="divCtrsFloatLeft">   --%>                 
                            <div id="divWarehouse" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWarehouse" runat="server" Text="Centro Dist."></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlWarehouse" runat="server" Enabled ="false" TabIndex="0"/>
                                    <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ControlToValidate="ddlWarehouse"
                                        InitialValue="-1" ValidationGroup="EditNew"  Text=" * " ErrorMessage="Centro es requerido" />
                                </div>
                            </div>                       
                            <div id="divOwner" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                </div>
                                <asp:DropDownList ID="ddlIdOwner" runat="server" TabIndex="1" />
                                <asp:RequiredFieldValidator ID="rfvIdOwn" runat="server" ControlToValidate="ddlIdOwner"
                                    InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />
                            </div>
                            <div id="divItemCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblItemCode" runat="server" Text="Código Item" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtItemCode" runat="server" Width="80px" TabIndex="2" MaxLength="100" />
                                    <asp:ImageButton ID="imgBtnSearchItem" runat="server" 
                                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" Width="17px" 
                                        onclick="imgBtnSearchItem_Click" />
                                    <asp:TextBox ID="txtItemName" runat="server" TabIndex="3" ForeColor="Silver" />
                                    <asp:RequiredFieldValidator ID="rfvItemCode" runat="server" ControlToValidate="txtItemCode"
                                        ValidationGroup="EditNew" Text=" * "  ErrorMessage="El código del item es requerido" />
                                </div>
                            </div>
                                                     
                            <div id="divLocCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLocCode" runat="server" Text="Código Ubicación" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLocCode" runat="server" MaxLength="50" Width="150px" TabIndex="4" />
                                    <asp:ImageButton ID="imgBtnSearchLocation" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" Width="16px"
                                    OnClick="imgBtnSearchLocation_Click" />
                                    <asp:RequiredFieldValidator ID="rfvtxtLocCode" runat="server" ControlToValidate="txtLocCode"
                                        ValidationGroup="EditNew" Text=" * "  ErrorMessage="Código de ubicación es requerido" />
                                    <%--<asp:RequiredFieldValidator ID="rfvLocCode" runat="server" ControlToValidate="txtLocCode"
                                        ValidationGroup="EditNew" Text=" * "  ErrorMessage="Código de ubicación es requerido" />--%>
                                    <%--<asp:RegularExpressionValidator ID="revtxtLocCode" runat="server" 
                                        ControlToValidate="txtLocCode"
                                        ErrorMessage="Código Ubicación permite ingresar solo caracteres alfanuméricos" 
                                        ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
                                        ValidationGroup="EditNew" Text=" * ">--%>
                                    </asp:RegularExpressionValidator>
                                
                                </div>
                            </div>
                            <div id="divReOrderPoint" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblReOrderPoint" runat="server" Text="Mínimo" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtReOrderPoint" runat="server" MaxLength="13" Width="50px" TabIndex="5" />
                                    <asp:RequiredFieldValidator ID="rfvtxtReOrderPoint" runat="server" ControlToValidate="txtReOrderPoint"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Mínimo es requerido" />
                                    <asp:RangeValidator ID="rvReOrderPoint" runat="server" ControlToValidate="txtReOrderPoint"
                                        ErrorMessage="Mínimo no contiene un número válido" MaximumValue="99999999" MinimumValue="0"
                                        ValidationGroup="EditNew" Type="Double" Display="Dynamic">*</asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divReOrderQty" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblReOrderQty" runat="server" Text="Máximo" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtReOrderQty" runat="server" MaxLength="13" Width="50px" Rows="3"
                                        TabIndex="6" />
                                    <asp:RequiredFieldValidator ID="rfvtxtReOrderQty" runat="server" ControlToValidate="txtReOrderQty"
                                        ErrorMessage="Máximo es requerido"  ValidationGroup="EditNew" Text=" * " />
                                    <asp:RangeValidator ID="rvReOrderQty" runat="server" ControlToValidate="txtReOrderQty"
                                        ErrorMessage="Máximo no contiene un número válido" MaximumValue="99999999" MinimumValue="1"
                                        ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                    <asp:CompareValidator ID="cvTxtReOrderQty" ControlToValidate="txtReOrderQty" ControlToCompare="txtReOrderPoint" Operator="GreaterThan" Type="Double" Display="Dynamic" Text="Máximo debe ser mayor a Mínimo" ErrorMessage="Máximo debe ser mayor a Mínimo" runat="server"></asp:CompareValidator>
                                    
                                </div></div>
                        </div>
                        <%--</div>--%>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" ShowMessageBox="false"
                                CssClass="modalValidation" />
                        </div>
                        <%--<div style="clear: both">
                        </div>--%>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" TabIndex="7"/>
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" TabIndex="8" OnClick="btnCancel_Click"/>
                        </div>
                        <%-- Fin Mensajes de error --%>
                    </div>
                </asp:Panel>
            </div>
                    <%--</asp:Panel>--%>

                
            <%--</div>--%>
            <%-- Panel Nuevo/Editar Documento --%>
            
            <%-- Lookup Items --%>
            <div id="divLookupItem" runat="server" visible="false">
            <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
            <!-- Boton 'dummy' para propiedad TargetControlID -->
            <ajaxToolkit:ModalPopupExtender ID="mpLookupItem" runat="server" TargetControlID="btnDummy2"
                PopupControlID="pnlLookupItem" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupItem" Drag="true">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlLookupItem" runat="server" CssClass="modalBox">

                <asp:Panel ID="pnlHeadBar2" runat="server" CssClass="modalHeader">
                    <div class="divCaption">
                        <asp:Label ID="lblAddItem" runat="server" Text="Buscar Item"/>
                        <asp:ImageButton ID="ImageButton2" runat="server" ToolTip="Cerrar" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                    </div>   
                               
                </asp:Panel>
                
                <div class="modalControls">
                    <asp:HiddenField ID="hidItemId" runat="server" Value="-1" />
                    <webUc1:ucLookUpFilter ID="ucFilterItem" runat="server" />                        
                    <div class="divCtrsFloatLeft">
                        <div class="divLookupGrid">
                            <asp:GridView ID="grdSearchItems" runat="server" DataKeyNames="Id" 
                                OnRowCommand="grdSearchItems_RowCommand" AllowPaging="true"
                                OnRowDataBound="grdSearchItems_RowDataBound"
                                AutoGenerateColumns="False"
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
                        </div>
                        <div style="clear:both" />
                    </div>
                </div>
                
                <div id="divPageGrdSearchItems" runat="server" class="modalActions">
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
            </div>
            <%-- FIN Lookup Items --%> 
            
            <%-- Lookup Locations --%>
            <div id="divLookupLocation" runat="server" visible="false">
            <asp:Button ID="btnDummy3" runat="server" Style="display: none" />
            <!-- Boton 'dummy' para propiedad TargetControlID -->
            <ajaxToolkit:ModalPopupExtender ID="mpLookupLocation" runat="server" TargetControlID="btnDummy3"
                PopupControlID="pnlLookupLocation" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupLocation" Drag="true">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlLookupLocation" runat="server" CssClass="modalBox">
                <asp:Panel ID="pnlHeadBar3" runat="server" CssClass="modalHeader">
                    <div class="divCaption">
                        <asp:Label ID="lblAddLocation" runat="server" Text="Buscar Ubicación" />
                        <asp:ImageButton ID="ImageButton3" runat="server" ToolTip="Cerrar" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                    </div>
                    
                </asp:Panel>
                <div class="modalControls">
                    <asp:HiddenField ID="hidLocationId" runat="server" Value="-1" />
                    <webUc1:ucLookUpFilter ID="ucFilterLocation" runat="server" /> 
                    <div class="divCtrsFloatLeft">
                        <div class="divLookupGrid">
                            <asp:GridView ID="grdSearchLocations" runat="server" DataKeyNames="idCode" OnRowCommand="grdSearchLocations_RowCommand" AllowPaging="true"
                                OnRowDataBound="grdSearchLocations_RowDataBound"
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField AccessibleHeaderText="IdCode" DataField="IdCode" HeaderText="Id" InsertVisible="False" SortExpression="IdCode" />
                                    <asp:TemplateField AccessibleHeaderText="Code" HeaderText="Codigo">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCode0" runat="server" Text='<%# Eval ("Code") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo" AccessibleHeaderText="LocTypeName" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLocType0" runat="server" Text='<%# Eval ("Type.LocTypeName") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Descripcion" AccessibleHeaderText="Description" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLocDesc0" runat="server" Text='<%# Eval ("Description") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <center>
                                                <asp:ImageButton ID="imgBtnAddLocation" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                    Width="20px" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div style="clear:both" />
                    </div>
                </div>
                <div id="divPageGrdSearchLocations" runat="server" class="modalActions">
                    <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                        <asp:ImageButton ID="btnFirstGrdSearchLocations" runat="server" OnClick="btnFirstGrdSearchLocations_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                        <asp:ImageButton ID="btnPrevGrdSearchLocations" runat="server" OnClick="btnPrevGrdSearchLocations_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                        Pág. 
                        <asp:DropDownList ID="ddlPagesSearchLocations" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchLocationsSelectedIndexChanged" SkinID="ddlFilter" /> 
                        de 
                        <asp:Label ID="lblPageCountSearchLocations" runat="server" Text="" />
                        <asp:ImageButton ID="btnNextGrdSearchLocations" runat="server" OnClick="btnNextGrdSearchLocations_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                        <asp:ImageButton ID="btnLastGrdSearchLocations" runat="server" OnClick="btnLastGrdSearchLocations_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                    </div>
                </div>
                
            </asp:Panel>
            </div>
            <%-- FIN Lookup Locations --%>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
            <%--<asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnSave" EventName="Click" />--%>
            
            <%--<asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowEditing" />--%>
            
         <%--   <asp:AsyncPostBackTrigger ControlID="grdSearchItems" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="grdSearchLocations" EventName="RowCommand" />--%>
            
            <asp:AsyncPostBackTrigger ControlID="imgBtnSearchItem" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnSearchLocation" EventName="Click" />
            
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnCancel" EventName="Click" />
            
        </Triggers>
    </asp:UpdatePanel>

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

    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
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
                            <asp:Label ID="Label1" runat="server" Text="Carga Masiva de Config. Ubicaciones" />
                             <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Config%20Ubicacion.xlsx" 
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Registro?" Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="Código Item" Visible="false" />
    <asp:Label ID="lblFilterDescription" runat="server" Text="Nombre Item" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre Item" Visible="false" />
    <asp:Label ID="lblNullOwnerRow" runat="server" Text="(Sin Dueño)" Visible="false" />
    
    <asp:Label ID="lblFilterCodeLoc" runat="server" Text="Id" Visible="false" />
    <asp:Label ID="lblFilterDescriptionLoc" runat="server" Text="Nombre Ubicación" Visible="false" />
    <asp:Label ID="lblFilterNameLoc" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Config. Ubicaciones" Visible="false" />
    <asp:Label ID="lblNotFileLoad" runat="server" Text="Seleccione el archivo que desea cargar." Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es valído." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblValidateMin" runat="server" Text="debe ser mayor o igual a 0 en ubicacion [LOCCODE]" Visible="false" />
    <asp:Label ID="lblValidateFormatMin" runat="server" Text="debe ser un número en ubicacion [LOCCODE]" Visible="false" />
    <asp:Label ID="lblValidateMax" runat="server" Text="debe ser mayor a 0 en ubicacion [LOCCODE]" Visible="false" />
    <asp:Label ID="lblValidateFormatMax" runat="server" Text="debe ser un número en ubicacion [LOCCODE]" Visible="false" />
    <asp:Label ID="lblValidateMinToMax" runat="server" Text="Min. no debe ser mayor que Max. en ubicacion [LOCCODE]" Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma exitosa." Visible="false" />
    <asp:Label ID="lblNotItemLocationsInFile" runat="server" Text="No existen config. de ubicaciones en el archivo." Visible="false" />
    <asp:Label ID="lblValidateRepeatedItems" runat="server" Text="No deben existir config. repetida con mismo OwnCode, ItemCode y LocCode." Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
