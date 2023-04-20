<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainFilterContent.ascx.cs"
    Inherits="Binaria.WMSTek.WebClient.Shared.MainFilterContent" %>
<%@ Register TagPrefix="obout" Namespace="OboutInc.Calendar2" Assembly="obout_Calendar2_NET" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<style type="text/css">
   .ajax__calendar_container { z-index: 10000; }
</style>

<%-- FILTRO BASICO --%>
<div class="mainFilterPanel">
    <%-- Group Item 1 (Sector) --%>
    <div id="divPrint" runat="server" visible="false" class="mainFilterPanelItem">
        <asp:ImageButton ID="imbPrint" runat="server" Enabled="false" Height="24px" 
            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_print.png" 
            onclick="imbPrint_Click"/>
    </div>
    <div id="divBscGroupItems" runat="server" visible="false">
        <div id="divBscGrpItm1" runat="server" visible="true" class="mainFilterPanelItem">
            <asp:Label ID="lblBscGrpItm1" runat="server" Text="Sector" /><br />
            <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm1" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="ddlBscGrpItm1_SelectedIndexChanged" Width="120px" Height="19px"/>
        </div>
        <%-- Group Item 2 (Rubro) --%>
        <div id="divBscGrpItm2" runat="server" visible="true" class="mainFilterPanelItem">
            <asp:Label ID="lblBscGrpItm2" runat="server" Text="Rubro" /><br />
            <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm2" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="ddlBscGrpItm2_SelectedIndexChanged" Width="120px" Height="19px" />
        </div>
        <%-- Group Item 3 (Familia) --%>
        <div id="divBscGrpItm3" runat="server" visible="true" class="mainFilterPanelItem">
            <asp:Label ID="lblBscGrpItm3" runat="server" Text="Familia" /><br />
            <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm3" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="ddlBscGrpItm3_SelectedIndexChanged" Width="120px" Height="19px" />
        </div>
        <%-- Group Item 4 (Subfamilia) --%>
        <div id="divBscGrpItm4" runat="server" visible="true" class="mainFilterPanelItem">
            <asp:Label ID="lblBscGrpItm4" runat="server" Text="Subfamilia" /><br />
            <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm4" runat="server" Width="120px" Height="19px" />
        </div>
    </div>
    <%--Seccion de filtros para Translate --%>
    <%-- Translate --%>
    <div id="divTranslate" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblTranslate" runat="server" Text="Idioma" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlTranslate" runat="server" Width="120px" Height="19px"/>
    </div>
    <%-- Module --%>
    <div id="divModule" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblModule" runat="server" Text="Módulo" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlModule" runat="server" Width="120px" Height="19px"/>
    </div>
    <%-- TypeObject --%>
    <div id="divTypeObject" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblTypeObject" runat="server" Text="Tipo Objeto" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlTypeObject" runat="server" Width="120px" Height="19px" />
    </div>
    <%-- Property --%>
    <div id="divProperty" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblProperty" runat="server" Text="Propiedad" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlProperty" runat="server" Width="120px" Height="19px"/>
    </div>
    <%-- Container --%>
    <div id="divContainer" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblObjectContainer" runat="server" Text="Objeto Contenedor" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlObjectContainer" runat="server" Width="300px" Height="19px"/>
    </div>
    <%--FIN Seccion de filtros para Translate --%>
    <%-- Wharehouse --%>
    <div id="divFilterWarehouse" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblWarehouse" runat="server" Text="Centro Distr." /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterWarehouse" runat="server" 
            Width="120px" Height="19px"
            onselectedindexchanged="ddlFilterWarehouse_SelectedIndexChanged1" />
    </div>
    <%-- Hangar --%>
    <div id="divFilterHangar" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblHangarBase" runat="server" Text="Bodega" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterHangar" runat="server" Width="120px" Height="19px"
         OnSelectedIndexChanged="ddlFilterHangar_SelectedIndexChanged1" />
    </div>
    <%-- Owner --%>
    <div id="divFilterOwner" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblOwner" runat="server" Text="Dueño" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterOwner" runat="server" Width="120px" Height="19px"
            OnSelectedIndexChanged="ddlFilterOwner_SelectedIndexChanged1"/>
    </div>
    <%-- LpnType --%>
    <div id="divFilterLpnType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblLpnType" runat="server" Text="Tipo LPN" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterLpnType" runat="server" Width="120px" Height="19px"/>
    </div>
    <%-- TruckType --%>
    <div id="divFilterTruckType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblTruckType" runat="server" Text="Tipo de Camión" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterTruckType" runat="server" Width="120px" Height="19px"/>
    </div>
    <%-- MovementType --%>
    <div id="divFilterMovementType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblMovementType" runat="server" Text="Movimiento" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterMovementType" runat="server" Width="180px" Height="19px"/>
    </div>
    <%-- Task Type --%>
    <div id="divFilterTaskType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblTaskType" runat="server" Text="Tarea" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterTaskType" runat="server" Width="120px" Height="19px"/>
    </div>
    <%-- Track Task Type --%>
    <div id="divFilterTrackTaskType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblTrackTaskType" runat="server" Text="Traza" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterTrackTaskType" runat="server" Width="120px" Height="19px"/>
    </div>
    <%-- InboundType --%>
    <div id="divFilterInboundType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblInboundType" runat="server" Text="Tipo Documento" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterInboundType" runat="server" Width="130px" Height="19px"/>
    </div>
     <%--TrackInboundType--%>
    <div id="divFilterTrackInboundType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblFilterTrackInboundType" runat="server" Text="Traza Documento" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterTrackInboundType" runat="server" Width="130px" Height="19px"/>
    </div>
    <%--ReferenceDocType--%>
    <div id="divFilterReferenceDocType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblReferenceDocType" runat="server" Text="Doc. Referencia" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterReferenceDocType" runat="server" Width="130px" Height="19px" />
    </div>
    <%--OutboundType--%>
    <div id="divFilterOutboundType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblOutboundType" runat="server" Text="Tipo Doc. Salida" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterOutboundType" runat="server" Width="130px" Height="19px"/>
    </div>
    <%--TrackOutboundType--%>
    <div id="divFilterTrackOutboundType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblTrackOutboundType" runat="server" Text="Traza Documento" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterTrackOutboundType" runat="server"
            Width="130px" Height="19px"/>
    </div>
     <%--DispatchType--%>
    <div id="divFilterDispatchType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblDispatchType" runat="server" Text="Traza" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterDispatchType" runat="server" Width="130px" Height="19px"/>
    </div>
    <%-- Scope --%>
    <div id="divFilterScope" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblScope" runat="server" Text="Ámbito" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterScope" runat="server" Width="120px" Height="19px"/>
        <br />
    </div>
     <%-- Kardex Type --%>
    <div id="divFilterKardexType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblKardexType" runat="server" Text="Tipo Kardex" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterKardexType" runat="server" Width="120px" Height="19px"/>
        <br />
    </div>
    <%-- Location Type --%>
    <div id="divFilterLocationType" runat="server" visible="false" class="mainFilterPanelItem">
        <asp:Label ID="lblLocationType" runat="server" Text="Tipo" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterLocationType" runat="server" Width="130px" Height="19px"/>
    </div>
    
    <%-- Zone Type --%>
    <div id="divFilterZoneType" runat="server" visible="false" class="mainFilterPanelItem">
        <asp:Label ID="lblFilterZoneType" runat="server" Text="Tipo Zona" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterZoneType" runat="server" Width="120px" Height="19px"/>
    </div>
    
    <%-- Reason --%>
    <div id="divFilterReason" runat="server" visible="false" class="mainFilterPanelItem">
        <asp:Label ID="lblFilterReason" runat="server" Text="Tipo Razón" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterReason" runat="server" Width="120px" Height="19px"/>
    </div>
    
    <%-- Code (genérico) --%>
    <div id="divFilterCode" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblCode" runat="server" Text="Código" /><br />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterCode" runat="server" Width="100px" />
    </div>
    <%-- Code (genérico) --%>
    <div id="divFilterCodeAlt" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblCodeAlt" runat="server" Text="Código" /><br />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterCodeAlt" runat="server" Width="80px" />
    </div>
    <%--CodeNumeric (genérico Numerico)--%>
    <div id="divFilterCodeNumeric" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblCodeNumeric" runat="server" Text="Código" /><br />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterCodeNumeric" runat="server" Width="80px" />
        <asp:RegularExpressionValidator ID="revCodeNumeric" runat="server" Text="Número Invalido"
            ControlToValidate="txtFilterCodeNumeric" Display="Dynamic" EnableClientScript="true"
            ValidationExpression="\d+" ValidationGroup="FilterSearch" />
    </div>
    <%--CodeNumeric (genérico Numerico)--%>
    <div id="divFilterDateYear" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="Label1" runat="server" Text="Fecha" /><br />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterDateYear" runat="server" Width="80px" />
        <ajaxToolkit:MaskedEditExtender ID="txtFilterDateYear_MaskedEditExtender" runat="server"
            CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat=""
            CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder=""
            CultureTimePlaceholder="" Enabled="True" Mask="9999" MaskType="Number" TargetControlID="txtFilterDateYear">
        </ajaxToolkit:MaskedEditExtender>
        <asp:CheckBox ID="chkFilterDateYear" runat="server" ToolTip="Activar Fecha" Checked="true" Visible="false" />
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Text="Número Invalido"
            ControlToValidate="txtFilterCodeNumeric" Display="Dynamic" EnableClientScript="true"
            ValidationExpression="\d+" ValidationGroup="FilterSearch" />
    </div>
    
    <%-- Item --%>
    <div id="divFilterItem" runat="server" class="mainFilterPanelItem" visible="false">
        <div runat="server" style="float:left">
            <asp:Label ID="lblItem" runat="server" Text="Cód. Item" /><br />
            <asp:TextBox SkinID="txtFilter" ID="txtFilterItem" runat="server" Width="75px"  />            
        </div> 
        <div runat="server" style="float:left; vertical-align: bottom;">
            <asp:ImageButton ID="btnBuscarItem" ToolTip="Buscar" runat="server" 
            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" 
            onclick="btnBuscarItem_Click" Height="19px" Width="20px" />          
          </div>
          <div runat="server" style="float:left;">  
            <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpLookupItemControl" runat="server" 
                TargetControlID="btnDummy" BackgroundCssClass="modalBackground" 
                oncancelscript="imgCloseItemSearchControl" PopupControlID="pnlItemsSearch"
                PopupDragHandleControlID="pnlItemsSearch">
            </ajaxToolkit:ModalPopupExtender>
        </div>
     </div>
    <%-- Name (genérico) --%>
    <div id="divFilterName" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblName" runat="server" Text="Nombre" /><br />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterName" runat="server" Width="80px" />
    </div>
    <%-- Description (genérico) --%>
    <div id="divFilterDescription" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblDescription" runat="server" Text="Descripción" /><br />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterDescription" runat="server" Width="80px" />
    </div>
    <%-- Status --%>
    <div id="divFilterStatus" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblStatus" runat="server" Text="Activo" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterStatus" runat="server" Width="70px">
            <asp:ListItem Text="(Todos)" Value="-1" Selected="True" />
            <asp:ListItem Text="Sí" Value="1" />
            <asp:ListItem Text="No" Value="0" />
        </asp:DropDownList>
    </div>
        <%-- TransactionStatus --%>
    <div id="divFilterTransactionStatus" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblTransactionStatus" runat="server" Text="Estado" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterTransactionStatus" runat="server" Width="70px">
            <asp:ListItem Text="(Todos)" Value="-1" Selected="True" />
            <asp:ListItem Text="OK" Value="OK" />
            <asp:ListItem Text="ERROR" Value="ER" />
        </asp:DropDownList>
    </div>
    <%-- SimpliRouteVisitStatus --%>
    <div id="divFilterSimpliRouteVisitStatus" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblSimpliRouteVisitStatus" runat="server" Text="Estado" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterSimpliRouteVisitStatus" runat="server" Width="100px">
            <asp:ListItem Text="(Todos)" Value="-1" Selected="True" />
            <asp:ListItem Text="Completado" Value="1" />
            <asp:ListItem Text="En Proceso" Value="0" />
        </asp:DropDownList>
    </div>
        <%-- Uom Type --%>
    <div id="divFilterUomType" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblUomType" runat="server" Text="Und. Medida" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterUomType" runat="server" Width="120px" Height="19px">
        </asp:DropDownList>
    </div>
    <%-- Doc Number --%>
    <div id="divFilterDocumentNumber" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblDocumentNumber" runat="server" Text="Nº Doc." /><br />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterDocumentNumber" runat="server" Width="100px" />
    </div>
    <%-- IdTruckCode Number --%>
    <div id="divFilterIdTruckCode" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblFilterIdTruckCode" runat="server" Text="Camión" /><br />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterIdTruckCode" runat="server" Width="100px" />
    </div>
    <%-- Date (genérico) --%>
    <div id="divFilterDate" runat="server" class="mainFilterPanelItem" visible="false" style="height:32px; vertical-align: middle;">
        <asp:Label ID="lblDate" runat="server" /><br />
        <obout:Calendar ID="calDate" runat="server" ShortDayNames="Do, Lu, Ma, Mi, Ju, Vi, Sa"
            CultureName="es-ES" TextBoxId="txtFilterDate" DatePickerMode="true" ShowYearSelector="true"
            YearSelectorType="DropDownList" YearMinScroll="2005" YearMaxScroll="2015" ShowMonthSelector="true"
            MonthSelectorType="DropDownList" ScrollBy="1" Columns="1" TitleText="" DatePickerImagePath="..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
            DatePickerSynchronize="true" TextArrowLeft="<" TextArrowRight=">" CSSDatePickerButton="calendarDatePickerButton" />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterDate" runat="server" Width="80px" Enabled="true" />
        <asp:CheckBox ID="chkFilterDate" runat="server" ToolTip="Activar Fecha" Checked="true" Visible="false" />
        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender9" runat="server" TargetControlID="txtFilterDate"
            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
            InputDirection="LeftToRight">
        </ajaxToolkit:MaskedEditExtender>
        <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="Fecha Invalida xxxxxxxxxxxxxx"
            Type="Date" MinimumValue="01-01-1901" MaximumValue="01-01-2090" Display="Dynamic"
            ControlToValidate="txtFilterDate">
        </asp:RangeValidator>
    </div>
    <%-- Date From (genérico) --%>
    <div id="divFilterDateFrom" runat="server" class="mainFilterPanelItem" visible="false" style="height:32px; vertical-align: middle;">
        <asp:Label ID="lblDateFrom" runat="server" /><br />
        <obout:Calendar ID="calDateFrom" runat="server" ShortDayNames="Do, Lu, Ma, Mi, Ju, Vi, Sa"
            CultureName="es-ES" TextBoxId="txtFilterDateFrom" DatePickerMode="true" ShowYearSelector="true"
            YearSelectorType="DropDownList" YearMinScroll="2005" YearMaxScroll="2015" ShowMonthSelector="true"
            MonthSelectorType="DropDownList" ScrollBy="1" Columns="1" TitleText="" DatePickerImagePath="..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
            DatePickerSynchronize="true" TextArrowLeft="<" TextArrowRight=">" CSSDatePickerButton="calendarDatePickerButton" />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterDateFrom" runat="server" Width="80px"
            Enabled="true" />
        <asp:CheckBox ID="chkFilterDateFrom" runat="server" ToolTip="Activar Fecha Desde"
            Checked="true" Visible="false" />
        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender7" runat="server" TargetControlID="txtFilterDateFrom"
            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
            InputDirection="LeftToRight">
        </ajaxToolkit:MaskedEditExtender>
        <asp:RangeValidator ID="RangeValidator4" runat="server" ErrorMessage="Fecha Invalida"
            Type="Date" MinimumValue="01-01-1901" MaximumValue="01-01-2090" Display="Dynamic"
            ControlToValidate="txtFilterDateFrom">
        </asp:RangeValidator>
    </div>
    <%-- Date To (genérico) --%>
    <div id="divFilterDateTo" runat="server" class="mainFilterPanelItem" visible="false" style="height:32px; vertical-align: middle;">
        <asp:Label ID="lblDateTo" runat="server" Text="Hasta" /><br />
        <obout:Calendar ID="calDateTo" runat="server" ShortDayNames="Do, Lu, Ma, Mi, Ju, Vi, Sa"
            CultureName="es-ES" TextBoxId="txtFilterDateTo" DatePickerMode="true" ShowYearSelector="true"
            YearSelectorType="DropDownList" YearMinScroll="2005" YearMaxScroll="2015" ShowMonthSelector="true"
            MonthSelectorType="DropDownList" ScrollBy="1" Columns="1" TitleText="" DatePickerImagePath="..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
            DatePickerSynchronize="true" TextArrowLeft="<" TextArrowRight=">" CSSDatePickerButton="calendarDatePickerButton" />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterDateTo" runat="server" Width="80px"
            Enabled="true" />
        <asp:CheckBox ID="chkFilterDateTo" runat="server" ToolTip="Activar Fecha Hasta" Checked="true" Visible="false" />
        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender8" runat="server" TargetControlID="txtFilterDateTo"
            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
            InputDirection="LeftToRight">
        </ajaxToolkit:MaskedEditExtender>
        <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Fecha Invalida hastaaaaaaaa"
            Type="Date" MinimumValue="01-01-1901" MaximumValue="01-01-2090" Display="Dynamic"
            ControlToValidate="txtFilterDateTo">
        </asp:RangeValidator>
    </div>
    
    <%-- Year (genérico) --%>
    <div id="divFilterYear" runat="server" class="mainFilterPanelItem" visible="false" style="height:32px; vertical-align: middle;">
        <asp:Label ID="LblYear" runat="server" Text="Año" /><br />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterYear" runat="server" Width="80px" />
        <ajaxToolkit:MaskedEditExtender ID="txtFilterYear_MaskedEditExtender" runat="server"
            CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat=""
            CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder=""
            CultureTimePlaceholder=""  Enabled="True" Mask="9999" MaskType="Number" TargetControlID="txtFilterYear">
        </ajaxToolkit:MaskedEditExtender>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" Text="Número Invalido"
            ControlToValidate="txtFilterCodeNumeric" Display="Dynamic" EnableClientScript="true"
            ValidationExpression="\d+" ValidationGroup="FilterSearch" />
        <asp:RangeValidator ID="RangeValidator_Year" runat="server" ErrorMessage="Año debe estar entre 2012 y 2030"
         ControlToValidate= "txtFilterYear" MinimumValue="2012" MaximumValue="2030" Text="*"></asp:RangeValidator>   
    </div>
    
    <%-- Uso de Ubicaciones Filtro (No Genérico) --%>
    
    <div id="divUsoUbicaciones" runat="server" visible="false" class="mainFilterPanelItem">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div style="float:left; vertical-align: bottom;">
                <asp:Label ID="lblPorcentajeUso" runat="server" Text="Pocentaje de Uso" /><br />
                <asp:CheckBox ID="rdbPorcentajeUso" runat="server" Checked="True"
                    oncheckedchanged="rdbPorcentajeUso_CheckedChanged" AutoPostBack="True" />
                <asp:TextBox SkinID="txtFilter" ID="txtPorcentajeUso" runat="server" 
                    Width="40px" ToolTip="Máximo 100" >0</asp:TextBox>
                 <ajaxToolkit:MaskedEditExtender ID="txtPorcentajeUso_MaskedEditExtender" runat="server"
                CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat=""
                CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder=""
                CultureTimePlaceholder="" Enabled="True" Mask="999" MaskType="Number" 
                    TargetControlID="txtPorcentajeUso" AutoComplete="False">
                </ajaxToolkit:MaskedEditExtender>
                <asp:RangeValidator ID="rgvPorcentajeUso" runat="server" ErrorMessage="Sólo entre 0 y 100"
                ControlToValidate= "txtPorcentajeUso" MinimumValue="0" MaximumValue="100" Text="*"></asp:RangeValidator>           
            </div>
            <div style="float:left; vertical-align: bottom; width:10px">            
            </div>
            <div style="float:left; vertical-align: bottom;">
            
                <asp:Label ID="Label2" runat="server" Text="Máximo Stock" /><br />
                    <asp:CheckBox ID="rdbMaximoStock" runat="server"
                     oncheckedchanged="rdbMaximoStock_CheckedChanged" AutoPostBack="True" />
                    <asp:TextBox SkinID="txtFilter" ID="txtMaxStock" runat="server" Width="40px" 
                        Enabled="False" />
                    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender10" runat="server"
                    CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat=""
                    CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder=""
                    CultureTimePlaceholder="" Enabled="True" Mask="999" MaskType="Number" TargetControlID="txtMaxStock">
                    </ajaxToolkit:MaskedEditExtender>
                 <br />
            </div>
        </ContentTemplate>
        </asp:UpdatePanel>
        
        
    </div>

    <div id="divTaskComplete" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblTaskComplete" runat="server" Text="Incluir Tareas Completadas" /><br />
        <asp:CheckBox ID="chkTaskComplete" runat="server" ToolTip="Incluir Tareas Completadas" Checked="false" />
    </div>

    <%-- ImpressionTail --%>
    <div id="divImpressionTail" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:RadioButtonList ID="rblSearchCriteriaFind" runat="server">  
            <asp:ListItem Selected="True" Value ="0">Lpn</asp:ListItem>             
            <asp:ListItem Value ="1">Precio</asp:ListItem>  
            <asp:ListItem Value ="2">Lista Embalaje</asp:ListItem>
        </asp:RadioButtonList>
    </div>

    <div id="divCountryFilter" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblCountryFilter" runat="server" Text="País" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlCountryFilter" runat="server" Width="120px" Height="19px" AutoPostBack="True" OnSelectedIndexChanged="ddlCountryFilter_SelectedIndexChanged" />
    </div>

    <div id="divStateFilter" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblStateFilter" runat="server" Text="Región" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlStateFilter" runat="server" Width="120px" Height="19px" OnSelectedIndexChanged="ddlStateFilter_SelectedIndexChanged" />
    </div>
        
    <div id="divCityFilter" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblCityFilter" runat="server" Text="Comuna" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlCityFilter" runat="server" Width="120px" Height="19px" />
    </div>

    <div id="divTaskQueueFilter" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblTaskQueueFilter" runat="server" Text="Traza Tarea" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlTaskQueueFilter" runat="server" Width="120px" Height="19px" />
    </div>

    <div id="divRotationItemFilter" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblRotationItemFilter" runat="server" Text="Filtro" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlRotationItemFilter" runat="server" Width="120px" Height="19px">
             <asp:ListItem Selected="True" Value="item"> Rotación Item </asp:ListItem>
             <asp:ListItem Value="location">Ubicaciones mas usadas </asp:ListItem>
        </asp:DropDownList>
    </div>

    <%-- Logical Wharehouse --%>
    <div id="divFilterLogicalWarehouse" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblLogicalWarehouse" runat="server" Text="Centro Distr." /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterLogicalWarehouse" runat="server" 
            Width="120px" Height="19px" onselectedindexchanged="ddlFilterLogicalWarehouse_SelectedIndexChanged" />
    </div>

    <div id="divEnableReserveStockOnZero" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lbldivEnableReserveStockOnZero" runat="server" Text="Incluir Reservas en 0" /><br />
        <asp:CheckBox ID="chkdivEnableReserveStockOnZero" runat="server" ToolTip="Incluir Reservas en 0" Checked="false" />
    </div>

    <div id="divOOInmediateProcess" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblOOInmediateProcess" runat="server" Text="Tipo Liberación" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlOOInmediateProcess" runat="server" Width="120px" Height="19px">
             <asp:ListItem Selected="True" Value="All">Todos</asp:ListItem>
             <asp:ListItem Value="Automatic">Automática </asp:ListItem>
             <asp:ListItem Value="Manual">Manual </asp:ListItem>
        </asp:DropDownList>
    </div>
    
     <%-- Wms Process --%>
    <div id="divWmsProcess" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblWmsProcess" runat="server" Text="Proceso" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterWmsProcess" runat="server" Width="200px">
        </asp:DropDownList>
    </div>
    
    <%-- Periodo Productividad Usuario --%>
    <div id="divPeriodUserProductivity" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblPeriodUserProductivity" runat="server" Text="Periodo" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlPeriodUserProductivity" runat="server" 
            Width="120px" Height="19px">
            <asp:ListItem Selected="True" Value="DAY"> Diario </asp:ListItem>
            <asp:ListItem Value="WEEK"> Semanal </asp:ListItem>
            <asp:ListItem Value="MONTH"> Mensual </asp:ListItem>
        </asp:DropDownList>
    </div>
    
    <%-- Vista Productividad Usuario --%>
    <div id="divViewUserProductivity" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblViewUserProductivity" runat="server" Text="Vista" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlViewUserProductivity" runat="server" 
            Width="120px" Height="19px">
            <asp:ListItem Selected="True" Value="USER"> Por Usuario </asp:ListItem>
            <asp:ListItem Value="TOTAL"> Totalizado </asp:ListItem>
        </asp:DropDownList>
    </div>
    
    <%-- Chk bicaciones Bloquedas --%>
    <div id="divFilterLockedLocation" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <asp:Label ID="lblFilterLockedLocation" runat="server" Text="Ubic. Bloqueda" /><br />
                <asp:CheckBox ID="chkFilterLockedLocation" runat="server" ToolTip="Ubic. Bloqueda" AutoPostBack="False" Checked="false" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div id="divTerminalStatus" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblTerminalStatus" runat="server" Text="Estado Conexión" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlTerminalStatus" runat="server" Width="70px">
            <asp:ListItem Text="(Todos)" Value="-1" Selected="True" />
            <asp:ListItem Text="Desconectado" Value="0" />
            <asp:ListItem Text="Conectado" Value="1" />
        </asp:DropDownList>
    </div>

    <%-- Boton 'Buscar' --%>
    <div class="mainFilterPanelItem">
        <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <asp:ImageButton ID="btnSearch" runat="server" ToolTip="Buscar" OnClick="btnSearch_Click"
                    ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search_on.png';"
                    onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search.png';"
                    ValidationGroup="FilterSearch" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="uprSearch" runat="server" AssociatedUpdatePanelID="upSearch"
            DisplayAfter="20" DynamicLayout="true">
            <ProgressTemplate>
                <div class="divProgress">
                    <img src="../../WebResources/Images/Buttons/icon_progress.gif" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <webUc:UpdateProgressOverlayExtender ID="muprSearch" runat="server" ControlToOverlayID="divTop"
            CssClass="updateProgress" TargetControlID="uprSearch" />
    </div>
    <%-- Boton 'Buscar' (Mapa 2D) --%>
    <div class="mainFilterPanelItem">
        <asp:ImageButton ID="btnSearchMap2D" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
            Visible="false" onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search_on.png';"
            onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search.png';" 
            ToolTip="Buscar" OnClientClick="GetLayoutMapClient(ctl00_ucMainFilterContent_ddlFilterHangar.selectedIndex);" />

    </div>

    <%-- Chk Pendin Orders --%>
    <div id="divFilterPendinOrders" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:UpdatePanel ID="upPendingOrders" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <asp:Label ID="lblPendindOrders" runat="server" Text="Órdenes Pendientes" /><br />
                <asp:CheckBox ID="chkFilterPendinOrders" runat="server" ToolTip="Activar Órdenes Pendientes"
                    OnCheckedChanged="chkFilterPendinOrders_CheckedChanged" AutoPostBack="true" Checked="false" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    
    <div class="mainFilterPanelItem">
        <asp:UpdatePanel ID="upSearchMap2DNew" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div style="display:inline-block" runat="server" id="divLocationsMostUsedByItem" visible="false">
                    <asp:Label ID="lblLocationsMostUsedByItem" runat="server" Text="Ubicaciones más usadas"></asp:Label> <br />
                    <asp:CheckBox runat="server" ID="chkLocationsMostUsedByItem" ToolTip="Mostrar ubicaciones más usadas" />
                </div>

                <div style="display:inline-block">
                    <asp:ImageButton ID="btnSearchMap2DNew" runat="server" ToolTip="Buscar" 
                        OnClick="btnSearchMap2DNew_Click"
                        Visible="false"
                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search_on.png';"
                        onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search.png';"
                        ValidationGroup="FilterSearch" />
                    &nbsp;&nbsp;    
                </div>
                <div style="display:inline-block">
                    <asp:ImageButton ID="btnCleanMap" runat="server" ToolTip="Limpiar" 
                        OnClick="btnCleanMap_Click"
                        Visible="false"                    
                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_clear.png" onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_clear_on.png';"
                        onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_clear.png';"
                        ValidationGroup="FilterSearch" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="uprSearchMap2DNew" runat="server" AssociatedUpdatePanelID="upSearchMap2DNew"
            DisplayAfter="20" DynamicLayout="true">
            <ProgressTemplate>
                <div class="divProgress">
                    <img src="../../WebResources/Images/Buttons/icon_progress.gif" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <webUc:UpdateProgressOverlayExtender ID="muprSearchMap2DNew" runat="server" ControlToOverlayID="divTop"
            CssClass="updateProgress" TargetControlID="uprSearchMap2DNew" />
    </div>

    <div class="mainFilterPanelItem">
        <asp:UpdatePanel ID="upSearchAux" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <asp:ImageButton ID="btnSearchAux" runat="server" ToolTip="Buscar" 
                    OnClick="btnSearchAux_Click"
                    Visible="false"
                    ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" 
                    onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search_on.png';"
                    onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search.png';"
                    ValidationGroup="FilterSearch" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="uprSearchAux" runat="server" AssociatedUpdatePanelID="upSearchAux"
            DisplayAfter="20" DynamicLayout="true">
            <ProgressTemplate>
                <div class="divProgress">
                    <img src="../../WebResources/Images/Buttons/icon_progress.gif" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <webUc:UpdateProgressOverlayExtender ID="muprSearchAux" runat="server" ControlToOverlayID="divTop"
            CssClass="updateProgress" TargetControlID="uprSearchAux" />
    </div>

    
    
    <%-- Opciones 'Filtro Avanzado' --%>
    <div id="divAdvancedFilterOptions" runat="server" visible="false" class="divAdvancedFilterOptions">
        <%-- Activar --%>
        <div id="divAdvancedFilterChk" runat="server" visible="false" style="float: left">
            <asp:CheckBox ID="chkUseAdvancedFilter" runat="server" ToolTip="Activar Filtro Avanzado" />
        </div>
        <%-- Mostrar / Ocultar --%>
        <div id="divToggleAdvancedFilter" runat="server" class="divAdvancedFilterOption">
            <asp:Label ID="lblToggleAdvancedFilter" Text="Filtro Avanzado" runat="server" />
        </div>
    </div>
</div>
<%-- FIN FILTRO BASICO --%>
<%-- FILTRO AVANZADO --%>
<ajaxToolkit:CollapsiblePanelExtender ID="cpeAdvancedFilter" runat="Server" Enabled="false"
    TargetControlID="pnlAdvancedFilter" ExpandControlID="divToggleAdvancedFilter"
    CollapseControlID="divToggleAdvancedFilter" Collapsed="True" SuppressPostBack="true" 
    TextLabelID="lblToggleAdvancedFilter" ExpandedText="Filtro Avanzado" CollapsedText="Filtro Avanzado" ExpandedSize="180"/>
<asp:Panel ID="pnlAdvancedFilter" runat="server" Visible="false" CssClass="advancedFilterPanel"  >
    <%-- Limpiar --%>
    <div style="float: right; right: 4px; top: 6px">
        <asp:Button ID="btnReset" runat="server" Text="Limpiar" OnClick="btnReset_Click"
            SkinID="btnSmall" Visible="false" />
        <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="FilterSearch"
            ShowMessageBox="false" CssClass="modalValidation" />
    </div>
    <ajaxToolkit:TabContainer runat="server" ID="Tabs" Height="120px" Width="700px" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="tabDispatching" HeaderText="Despacho" Visible="false" >
            
   
            <%--<HeaderTemplate>
                Despacho
            </HeaderTemplate>--%>
            <ContentTemplate>
           
                <div id="divPriority" runat="server" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblPriority" runat="server" Text="Prioridad" />
                        <br />
                        <asp:Label ID="lblPriorityFrom" runat="server" Text="entre" />
                        <asp:TextBox ID="txtPriorityFrom" runat="server" Width="20px" />
                        <asp:Label ID="lblPriorityTo" runat="server" Text=" y " />
                        <asp:TextBox ID="txtPriorityTo" runat="server" Width="20px" />
                    </div>
                </div>
                <div id="divDelivery" runat="server" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblCustomer" runat="server" Text="Cliente" />
                        <br />
                        <asp:TextBox ID="txtCustomer" runat="server" Width="70px" />
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblCarrier2" runat="server" Text="Transportista" />
                        <br />
                        <asp:TextBox ID="txtCarrier" runat="server" Width="70px" />
                    </div>
                </div>
                <div id="divDelivery2" runat="server" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblRoute" runat="server" Text="Ruta" />
                        <br />
                        <asp:TextBox ID="txtRoute" runat="server" Width="70px" />
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblCountry" runat="server" Text="País" />
                        <br />
                        <asp:DropDownList ID="ddlCountry" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged"
                            SkinID="ddlFilter" Width="120px" />
                    </div>
                </div>
                <div id="divDelivery3" runat="server" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblState" runat="server" Text="Región" />
                        <br />
                        <asp:DropDownList ID="ddlState" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlState_SelectedIndexChanged"
                            SkinID="ddlFilter" Width="120px" />
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblCity" runat="server" Text="Comuna" />
                        <br />
                        <asp:DropDownList ID="ddlCity" runat="server" SkinID="ddlFilter" Width="120px" />
                    </div>
                </div>
                
            </ContentTemplate>
 
            
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabLayout" Visible="false">
        
            <ContentTemplate>
           
                <%-- Hangar --%>
                <div runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblHangar" runat="server" Text="Bodega" /><br />
                        <asp:ListBox ID="lstHangar" runat="server" Height="80px" Width="150px" SelectionMode="Multiple" />
                    </div>
                </div>
                <%-- WorkZone --%>
                <div irunat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblWorkZone" runat="server" Text="Zona" /><br />
                        <asp:ListBox ID="lstWorkZone" runat="server" Height="80px" Width="150px" SelectionMode="Multiple" />
                    </div>
                </div>
                <%-- Location Type (Multiple) --%>
                <div runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLocationTypeMultiple" runat="server" Text="Tipo Ubicación" /><br />
                        <asp:ListBox ID="lstLocationType" runat="server" Height="80px" Width="150px" SelectionMode="Multiple" />
                    </div>
                </div>
               
            </ContentTemplate>
            
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabLocation" HeaderText="Ubicación" Visible="false">
        
            <ContentTemplate>
            
                <%-- INICIO Seccion Rangos de Location: Row, Column And Level --%>
                <div id="divFilterLocation" runat="server" visible="False" class="mainFilterPanelItem">
                    <div id="divLocRowRange" runat="server" class="mainFilterPanelItem" visible="true">
                        <asp:Label ID="lblLocRowRangeFrom" runat="server" Text="Hilera Desde" /><br />
                        <asp:DropDownList SkinID="ddlFilter" ID="ddlLocRowRangeFrom" runat="server" Width="60px" />
                        <br />
                        <asp:Label ID="lblLocRowRangeTo" runat="server" Text="Hilera Hasta" /><br />
                        <asp:DropDownList SkinID="ddlFilter" ID="ddlLocRowRangeTo" runat="server" Width="60px" />
                        <br />
                    </div>
                    <div id="divLocColumnRange" runat="server" class="mainFilterPanelItem" visible="true">
                        <asp:Label ID="LocColumnRangeFrom" runat="server" Text="Columna Desde" /><br />
                        <asp:DropDownList SkinID="ddlFilter" ID="ddlLocColumnRangeFrom" runat="server" Width="60px" />
                        <br />
                        <asp:Label ID="lblLocColumnRangeTo" runat="server" Text="Columna Hasta" /><br />
                        <asp:DropDownList SkinID="ddlFilter" ID="ddlLocColumnRangeTo" runat="server" Width="60px" />
                        <br />
                    </div>
                    <div id="divLocLevelRange" runat="server" class="mainFilterPanelItem" visible="true">
                        <asp:Label ID="lblLocLevelRangeFrom" runat="server" Text="Nivel Desde" /><br />
                        <asp:DropDownList SkinID="ddlFilter" ID="ddlLocLevelRangeFrom" runat="server" Width="60px" />
                        <br />
                        <asp:Label ID="lblLocLevelRangeTo" runat="server" Text="Nivel Hasta" /><br />
                        <asp:DropDownList SkinID="ddlFilter" ID="ddlLocLevelRangeTo" runat="server" Width="60px" />
                        <br />
                    </div>
                </div>
                <%-- FIN Seccion Rangos de Location: Row, Column And Level --%>
                <div id="divTxtLocationEqual" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Location --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLocation" runat="server" Text="Ubicación" /><br />
                        <asp:TextBox ID="txtLocation" runat="server" Width="70px" />
                    </div>
                </div>
                <div id="divTxtLocationRange" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Range Location --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLocationFrom" runat="server" Text="Rango Desde" /><br />
                        <asp:TextBox ID="txtLocationFrom" runat="server" Width="70px" />
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLocationTo" runat="server" Text="Hasta" /><br />
                        <asp:TextBox ID="txtLocationTo" runat="server" Width="70px" />
                    </div>
                </div>
                <div id="divTxtLocationRowColumnEqual" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Location Row --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLocationRow" runat="server" Text="Fila" /><br />
                        <asp:TextBox ID="txtLocationRow" runat="server" Width="50" />
                        <asp:RangeValidator ID="rvLocationRow" runat="server" ControlToValidate="txtLocationRow"
                            ErrorMessage="Fila no contiene un número válido." Text=" * " MaximumValue="2147483647"
                            MinimumValue="0" ValidationGroup="FilterSearch" Type="Integer" />
                    </div>
                    <%-- Location Column --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLocationColumn" runat="server" Text="Columna" /><br />
                        <asp:TextBox ID="txtLocationColumn" runat="server" Width="50" />
                        <asp:RangeValidator ID="rvLocationColumn" runat="server" ControlToValidate="txtLocationColumn"
                            ErrorMessage="Columna no contiene un número válido." Text=" * " MaximumValue="2147483647"
                            MinimumValue="0" ValidationGroup="FilterSearch" Type="Integer" />
                    </div>
                </div>
                <div id="Div1" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Location Level --%>
                    <div id="divTxtLocationLevelEqual" runat="server" class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLocationLevel" runat="server" Text="Nivel" /><br />
                        <asp:TextBox ID="txtLocationLevel" runat="server" Width="50" />
                        <asp:RangeValidator ID="rvLocationLevel" runat="server" ControlToValidate="txtLocationLevel"
                            ErrorMessage="Nivel no contiene un número válido." Text=" * " MaximumValue="2147483647"
                            MinimumValue="0" ValidationGroup="FilterSearch" Type="Integer" />
                    </div>
                    <%-- Location Aisle --%>
                    <div id="divTxtLocationAisleEqual" runat="server" class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLocationAisle" runat="server" Text="Pasillo" /><br />
                        <asp:TextBox ID="txtLocationAisle" runat="server" Width="50" />
                    </div>
                </div>
                <div id="divLockedLocation" runat="server" visible="false" class="divMainFilterFloatLeftNarrow">
                    <div id="div2" runat="server" class="mainFilterPanelTabItem">
                        <asp:CheckBox ID="chkLockedLocation" runat="server" Checked="false" AutoPostBack="false" Text="Ubic. Bloqueda" />
                    </div>
                </div>
                
            </ContentTemplate>
            
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabItemGroup" HeaderText="Grupo Item" Visible="false">
  
            <ContentTemplate>
                <asp:UpdatePanel ID="upItemGroup" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Group Item 1 (Sector) --%>
                        <div id="divItemGroup" runat="server" visible="true" class="divMainFilterFloatLeft">
                            <div class="mainFilterPanelTabItem">
                                <asp:Label ID="lblGroupItem1" runat="server" Text="Sector" /><br />
                                <asp:DropDownList SkinID="ddlFilter" ID="ddlGrpItem1" runat="server" AutoPostBack="True"
                                    OnSelectedIndexChanged="ddlGrpItem1_SelectedIndexChanged" Width="120px" />
                            </div>
                        </div>
                        <%-- Group Item 2 (Rubro) --%>
                        <div id="divGroupItem2" runat="server" visible="true" class="divMainFilterFloatLeft">
                            <div class="mainFilterPanelTabItem">
                                <asp:Label ID="lblGroupItem2" runat="server" Text="Rubro" /><br />
                                <asp:DropDownList SkinID="ddlFilter" ID="ddlGrpItem2" runat="server" AutoPostBack="True"
                                    OnSelectedIndexChanged="ddlGrpItem2_SelectedIndexChanged" Width="120px" />
                            </div>
                        </div>
                        <%-- Group Item 3 (Familia) --%>
                        <div id="divGroupItem3" runat="server" visible="true" class="divMainFilterFloatLeft">
                            <div class="mainFilterPanelTabItem">
                                <asp:Label ID="lblGroupItem3" runat="server" Text="Familia" /><br />
                                <asp:DropDownList SkinID="ddlFilter" ID="ddlGrpItem3" runat="server" AutoPostBack="True"
                                    OnSelectedIndexChanged="ddlGrpItem3_SelectedIndexChanged" Width="120px" />
                            </div>
                        </div>
                        <%-- Group Item 4 (Subfamilia) --%>
                        <div id="divGroupItem4" runat="server" visible="true" class="divMainFilterFloatLeft">
                            <div class="mainFilterPanelTabItem">
                                <asp:Label ID="lblGroupItem4" runat="server" Text="Subfamilia" /><br />
                                <asp:DropDownList SkinID="ddlFilter" ID="ddlGrpItem4" runat="server" Width="120px" />
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnReset" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </ContentTemplate>
   
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabDates" HeaderText="Fecha" Visible="false">
        
            <ContentTemplate>
            
                <%-- Fecha Elaboración --%>
                <div id="divFabricationDate" runat="server" visible="false" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblFabricationDateFrom" runat="server" Text="Elab. Desde" /><br />
                        <asp:TextBox ID="txtFabricationDateFrom" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender5" runat="server" TargetControlID="txtFabricationDateFrom"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="calFabricationDateFrom" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtFabricationDateFrom"
                            PopupButtonID="txtFabricationDateFrom" EnableViewState="False" >
                        </ajaxToolkit:CalendarExtender>
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblFabricationDateTo" runat="server" Text="Hasta" /><br />
                        <asp:TextBox ID="txtFabricationDateTo" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender6" runat="server" TargetControlID="txtFabricationDateTo"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="CalendarExtender2" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtFabricationDateTo"
                            PopupButtonID="txtFabricationDateTo" EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>
                    </div>
                </div>
                <%-- Fecha Vencimiento --%>
                <div id="divExpirationDate" runat="server" visible="false" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblExpirationDateFrom" runat="server" Text="Venc. Desde" /><br />
                        <asp:TextBox ID="txtExpirationDateFrom" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="meExpirationDateFrom" runat="server" TargetControlID="txtExpirationDateFrom"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="calExpirationDateFrom" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtExpirationDateFrom"
                            PopupButtonID="txtExpirationDateFrom" EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblExpirationDateTo" runat="server" Text="Hasta" /><br />
                        <asp:TextBox ID="txtExpirationDateTo" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="meExpirationDateTo" runat="server" TargetControlID="txtExpirationDateTo"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="calExpirationDateTo" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtExpirationDateTo"
                            PopupButtonID="txtExpirationDateTo" EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>
                    </div>
                </div>
                <%-- Fecha Esperada --%>
                <div id="divExpectedDate" runat="server" visible="false" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblExpectedDateFrom" runat="server" Text="Esperada Desde" /><br />
                        <asp:TextBox ID="txtExpectedDateFrom" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="txtExpectedDateFrom"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="CalendarExtender1" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtExpectedDateFrom"
                            PopupButtonID="txtExpectedDateFrom" EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblExpectedDateTo" runat="server" Text="Hasta" /><br />
                        <asp:TextBox ID="txtExpectedDateTo" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="txtExpectedDateTo"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="CalendarExtender3" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtExpectedDateTo" PopupButtonID="txtExpectedDateTo"
                            EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>
                    </div>
                </div>
                <%-- Fecha Despacho --%>
                <div id="divShipmentDate" runat="server" visible="false" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblShipmentDateFrom" runat="server" Text="Despacho Desde" /><br />
                        <asp:TextBox ID="txtShipmentDateFrom" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender3" runat="server" TargetControlID="txtShipmentDateFrom"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="CalendarExtender4" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtShipmentDateFrom"
                            PopupButtonID="txtShipmentDateFrom" EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblShipmentDateTo" runat="server" Text="Hasta" /><br />
                        <asp:TextBox ID="txtShipmentDateTo" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender4" runat="server" TargetControlID="txtShipmentDateTo"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="CalendarExtender5" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtShipmentDateTo" PopupButtonID="txtShipmentDateTo"
                            EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>
                    </div>
                </div>
                <%-- Nº de Lote --%>
                <div id="divLotNumber" runat="server" visible="false" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLotNumberFrom" runat="server" Text="Lote Desde" /><br />
                        <asp:TextBox ID="txtLotNumberFrom" runat="server" Width="80px" />
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLotNumberTo" runat="server" Text="Hasta" /><br />
                        <asp:TextBox ID="txtLotNumberTo" runat="server" Width="80px" />
                    </div>
                </div>
               
            </ContentTemplate>
            
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabDocument" HeaderText="Documento" Visible="false">
        
            <ContentTemplate>
           
                <%-- Tipo de doc de ingreso--%>
                <div id="divDocumentType" runat="server" visible="false" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblDocumentType" runat="server" Text="Tipo" /><br />
                        <asp:ListBox ID="lstInboundType" runat="server" Height="80px" Width="166px" SelectionMode="Multiple" />
                    </div>
                </div>
                <%-- Vendor --%>
                <div id="divVendor" runat="server" visible="false" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblVendor" runat="server" Text="Proveedor" /><br />
                        <asp:ListBox ID="lstVendor" runat="server" Height="80px" Width="180px" SelectionMode="Multiple" />
                    </div>
                </div>
                <%-- Carrier --%>
                <div id="divCarrier" runat="server" visible="false" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblCarrier" runat="server" Text="Transportista" /><br />
                        <asp:ListBox ID="lstCarrier" runat="server" Height="80px" Width="180px" SelectionMode="Multiple" />
                    </div>
                </div>
                <%-- Driver --%>
                <div id="divDriver" runat="server" visible="false" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblDriver" runat="server" Text="Chofer" /><br />
                        <asp:ListBox ID="lstDriver" runat="server" Height="80px" Width="180px" SelectionMode="Multiple" />
                    </div>
                </div>
                
               
                
            </ContentTemplate>
            
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabReceptionLog" HeaderText="Filtro Avanzado" Visible="false">     
            <ContentTemplate>           
            
                <div id="divItemCode" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Item Code --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblItemCode" runat="server" Text="Codigo Item" /><br />
                        <asp:TextBox ID="txtItemCode" runat="server" Width="120px" />
                    </div>
                </div>
                <div id="divItemName" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Name Code --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblItemName" runat="server" Text="Nombre Item" /><br />
                        <asp:TextBox ID="txtItemName" runat="server" Width="120px" />
                    </div>
                </div>
                <div id="divDocumentNbr" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- DocumentNumber --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblDocumentNbr" runat="server" Text="Documento" /><br />
                        <asp:TextBox ID="txtDocumentNbr" runat="server" Width="120px" />
                    </div>
                </div>
                <div id="divReferenceNbr" runat="server" visible="false" class="divMainFilterFloatLeftNarrow">
                    <%-- ReferenceNumber --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblReferenceNbr" runat="server" Text="Nro Refencia" /><br />
                        <asp:TextBox ID="txtReferenceNbr" runat="server" Width="120px" />
                    </div>
                </div>
                <div id="divOperator" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Operator --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblOperator" runat="server" Text="Operador" /><br />
                        <asp:TextBox ID="txtOperator" runat="server" Width="120px" />
                    </div>
                </div>
                <div id="divSourceLocation" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- SourceLocation --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblSourceLocation" runat="server" Text="Ubic. Origen" /><br />
                        <asp:TextBox ID="txtSourceLocation" runat="server" Width="120px" />
                    </div>
                </div>
                <div id="divTargetLocation" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- TargetLocation --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblTargetLocation" runat="server" Text="Ubic. Destino" /><br />
                        <asp:TextBox ID="txtTargetLocation" runat="server" Width="120px" />
                    </div>
                </div>
                <div id="divSourceLpn" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- SourceLpn --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblSourceLpn" runat="server" Text="LPN Origen" /><br />
                        <asp:TextBox ID="txtSourceLpn" runat="server" Width="120px" />
                    </div>
                </div>
                <div id="divTargetLpn" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- TargetLpn --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblTargetLpn" runat="server" Text="LPN Destino" /><br />
                        <asp:TextBox ID="txtTargetLpn" runat="server" Width="120px" />
                    </div>
                </div>
                <div id="divPriorityTask" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Priority --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblPriorityTask" runat="server" Text="Prioridad" /><br />
                        <asp:TextBox ID="txtPriorityTask" runat="server" Width="60px" />
                        <asp:RangeValidator ID="rvPriority" runat="server" ControlToValidate="txtPriorityTask"
                            ErrorMessage="Prioridad debe estar entre 1 y 10" Text=" * " MinimumValue="1"
                            MaximumValue="10" ValidationGroup="FilterSearch" Type="Integer" />
                    </div>
                </div>
              
            </ContentTemplate>
            
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabTask" HeaderText="Tareas" Visible="false">

            <ContentTemplate>
            
                <div id="divShowDetail" runat="server" class="divMainFilterFloatLeftCheckBox" visible="false">
                    <div class="mainFilterPanelTabItem">
                        <asp:CheckBox ID="chkShowDetail" runat="server" ToolTip="Mostrar Detalle" Text="Mostrar Detalle"
                            Width="130px" />
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:CheckBox ID="chkNotComplete" runat="server" AutoPostBack="false" Checked="false"
                            ToolTip="Tareas Sin Ejecutar" Text="Sin Ejecutar" OnCheckedChanged="chkNotComplete_CheckedChanged"
                            Width="130px" />
                    </div>
                    <div class="mainFilterPanelTabItem">
                        <asp:CheckBox ID="chkComplete" runat="server" ToolTip="Tareas Ejecutadas" Text="Ejecutada"
                            OnCheckedChanged="chkComplete_CheckedChanged" Width="130px" />
                    </div>
                </div>
               
            </ContentTemplate>

        </ajaxToolkit:TabPanel>
        
        <ajaxToolkit:TabPanel runat="server" ID="tabProveedor" HeaderText="Proveedor" Visible="false">

            <ContentTemplate>
            
                <%-- Proveedor --%>
                <div id="divProveedor" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelItem">
                        <asp:Label ID="lblNombreProveedor" runat="server" Text="Proveedor" /><br />
                        <asp:TextBox ID="txtNombreProveedor" runat="server" Width="100px" Enabled="true" />
                        <asp:ImageButton ID="imgbtnSearchProveedor" runat="server" Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                            OnClick="imgBtnSearchProveedor_Click" Width="18px" ValidationGroup="searchItem" /><br />
                    </div>
                    <div class="mainFilterPanelItem">
                        <asp:ListBox ID="lstProveedor" runat="server" Height="80px" Width="350px" SelectionMode="Multiple" />
                    </div>
                </div>
                
               
            </ContentTemplate>
      
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabTransportista" HeaderText="Transportista" Visible="false">
       
            <ContentTemplate>
            
                <div id="divTransportista" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelItem">
                        <asp:Label ID="lblNombreTransportista" runat="server" Text="Transportista" /><br />
                        <asp:TextBox ID="txtNombreTransportista" runat="server" Width="100px" Enabled="true" />
                        <asp:ImageButton ID="imgbtnSearchTransportista" runat="server" Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                            OnClick="imgBtnSearchTransportista_Click" Width="18px" ValidationGroup="searchItem" />
                    </div>
                    <div class="mainFilterPanelItem">
                        <asp:ListBox ID="lstTransportista" runat="server" Height="80px" Width="350px" SelectionMode="Multiple" />
                    </div>
                </div>
               
            </ContentTemplate>

        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabChofer" HeaderText="Chofer" Visible="false">
        
            <ContentTemplate>
           
                <div id="divChofer" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelItem">
                        <asp:Label ID="lblNombreChofer" runat="server" Text="Chofer" /><br />
                        <asp:TextBox ID="txtNombreChofer" runat="server" Width="100px" Enabled="true" />
                        <asp:ImageButton ID="imgbtnSearchChofer" runat="server" Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                            OnClick="imgBtnSearchChofer_Click" Width="18px" ValidationGroup="searchItem" />
                    </div>
                    <div class="mainFilterPanelItem">
                        <asp:ListBox ID="lstChofer" runat="server" Height="80px" Width="350px" SelectionMode="Multiple" />
                    </div>
                </div>
                
            </ContentTemplate>

        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabLpn" HeaderText="LPN" Visible="false">
            <ContentTemplate>
                <div id="divLpn" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelItem">
                        <asp:Label ID="lblIdLpnCode" runat="server" Text="LPN" /><br />
                        <asp:TextBox ID="txtIdLpnCode" runat="server" Width="100px" Enabled="true" /><br />
                        <asp:CheckBox ID="chkLpnIsClosed" runat="server" AutoPostBack="false" Checked="false" ToolTip="Cerrado" Text="Cerrado" OnCheckedChanged="chkLpnIsClosed_CheckedChanged" Width="100px" /><br />
                        <asp:CheckBox ID="chkLpnIsNotClosed" runat="server" ToolTip="No Cerrado" Text="No Cerrado" OnCheckedChanged="chkLpnIsNotClosed_CheckedChanged" Width="100px" />
                    </div>
                    
                </div>
                <div id="divLpnParent" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelItem">
                        <asp:Label ID="lblLpnParent" runat="server" Text="LpnParent" /><br />
                        <asp:TextBox ID="txtLpnParent" runat="server" Width="100px" Enabled="true" /><br />
                        <asp:CheckBox ID="chkLpnIsParent" runat="server" AutoPostBack="false" Checked="false" ToolTip="Es Padre" Text="Es Padre" OnCheckedChanged="chkLpnIsParent_CheckedChanged" Width="100px" /><br />
                        <asp:CheckBox ID="chkLpnIsNotParent" runat="server" ToolTip="No es Padre" Text="No es Padre" OnCheckedChanged="chkLpnIsNotParent_CheckedChanged" Width="100px" />
                    </div>
                </div>
                <div id="divLpnSealNumber" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelItem">
                        <asp:Label ID="lblLpnSealNumber" runat="server" Text="Numero Sello" /><br />
                        <asp:TextBox ID="txtLpnSealNumber" runat="server" Width="100px" Enabled="true" />
                    </div>
                </div>
                <div id="divLpnIdLpnType" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelItem">
                        <asp:Label ID="lblLpnIdLpnType" runat="server" Text="Tipo" /><br />
                        <asp:ListBox ID="lstLpnLpnType" runat="server" Height="80px" Width="180px" SelectionMode="Multiple" />
                    </div>
                </div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        
                 <ajaxToolkit:TabPanel runat="server" ID="tabItemUnits" HeaderText="Por Unidades" Visible="false">
            <ContentTemplate>
                
                <div id="divTotalLines" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblTotalLines" runat="server" Text="Total Líneas" /><br />
                        <asp:TextBox ID="txtTotalLines" runat="server" Width="80px" /> 
                        
                        <ajaxToolkit:MaskedEditExtender ID="meTotalLines" runat="server"
                        CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" InputDirection="RightToLeft"
                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder=""
                        CultureTimePlaceholder="" Enabled="True" Mask="999" MaskType="Number" TargetControlID="txtTotalLines">
                        </ajaxToolkit:MaskedEditExtender>
                    </div>
                </div>
                <div id="divTotalItems" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblTotalItems" runat="server" Text="Total Ítems" /><br />
                        <asp:TextBox ID="txtTotalItems" runat="server" Width="80px" />   

                        <ajaxToolkit:MaskedEditExtender ID="meTotalItems" runat="server"
                        CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" InputDirection="RightToLeft"
                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder=""
                        CultureTimePlaceholder="" Enabled="True" Mask="999999" MaskType="Number" TargetControlID="txtTotalItems">
                        </ajaxToolkit:MaskedEditExtender>
                    </div>
                </div>
                <div id="divMaxPerLines" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblMaxPerLines" runat="server" Text="Maximo Por Líneas" /><br />
                        <asp:TextBox ID="txtMaxPerLines" runat="server" Width="80px" onkeypress="javascript:return solonumeros(event)" /> 

                       <ajaxToolkit:MaskedEditExtender ID="meMaxPerLines" runat="server"
                        CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" InputDirection="RightToLeft"
                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder=""
                        CultureTimePlaceholder="" Enabled="True" Mask="999999" MaskType="Number" TargetControlID="txtMaxPerLines">
                        </ajaxToolkit:MaskedEditExtender>                           
                   </div>
                </div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>

        <ajaxToolkit:TabPanel runat="server" ID="tabGS1" HeaderText="Filtro Avanzado" Visible="false">     
            <ContentTemplate>           
            
                <div id="divGtinTabGs1" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Gtin --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblGtinTabGs1" runat="server" Text="GTIN" /><br />
                        <asp:TextBox ID="txtGtinTabGs1" runat="server" Width="120px" />
                    </div>
                </div>
                
                <div id="divGsinTabGs1" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Gsin --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblGsinTabGs1" runat="server" Text="GSIN" /><br />
                        <asp:TextBox ID="txtGsinTabGs1" runat="server" Width="120px" />
                    </div>
                </div>
				               
                <div id="divLotNumberTabGs1" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <%-- Lote --%>
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblLotNumberTabGs1" runat="server" Text="Lote" /><br />
                        <asp:TextBox ID="txtLotNumberTabGs1" runat="server" Width="120px" />
                    </div>
                </div>
                
                <div id="divFabricationDateTabGs1" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblFabricationDateTabGs1" runat="server" Text="Fecha Fabricación" /><br />
                        <asp:TextBox ID="txtFabricationDateTabGs1" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID=meeFabricationDateTabGs1 runat="server" TargetControlID="txtFabricationDateTabGs1"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID=ceFabricationDateTabGs1 CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtFabricationDateTabGs1"
                            PopupButtonID="txtFabricationDateTabGs1" EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>
                    </div>
				</div>
				<div id="divExpirationDateTabGs1" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblExpirationDateTabGs1" runat="server" Text="Fecha Expiración" /><br />
                        <asp:TextBox ID="txtExpirationDateTabGs1" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="meeExpirationDateTabGs1" runat="server" TargetControlID="txtExpirationDateTabGs1"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="ceExpirationDateTabGs1" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtExpirationDateTabGs1" PopupButtonID="txtExpirationDateTabGs1"
                            EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>
                    </div>
                </div>
              
            </ContentTemplate>
            
        </ajaxToolkit:TabPanel>

        <%--Filtro Avanzado Mapa Bodega--%>
        <ajaxToolkit:TabPanel runat="server" ID="tabMapaBodega" HeaderText="Filtro Avanzado" Visible="false">
            <ContentTemplate>
            
                <div id="divMapFabricationDate" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">  
                        <asp:Label ID="lblMapFabricationDate" runat="server" Text="Fecha Elab." /><br />
                        <asp:TextBox ID="txtMapFabricationDate" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="meMapFabricationDate" runat="server" TargetControlID="txtMapFabricationDate"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="calMapFabricationDate" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtMapFabricationDate"
                            PopupButtonID="txtMapFabricationDate" EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>
                    </div>
                </div>
                
                <div id="divMapExpirationDate" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">     
                    <div class="mainFilterPanelTabItem">               
                        <asp:Label ID="lblMapExpirationDate" runat="server" Text="Fecha Venc." /><br />
                        <asp:TextBox ID="txtMapExpirationDate" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="meMapExpirationDate" runat="server" TargetControlID="txtMapExpirationDate"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="calMapExpirationDate" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtMapExpirationDate"
                            PopupButtonID="txtMapExpirationDate" EnableViewState="False">
                        </ajaxToolkit:CalendarExtender>   
                    </div>                 
                </div>
                
                <div id="divMapFifoDate" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblMapFifoDate" runat="server" Text="Fecha Fifo" /><br />
                        <asp:TextBox ID="txtMapFifoDate" runat="server" Width="80px" />
                        <ajaxToolkit:MaskedEditExtender ID="meMapFifoDate" runat="server" TargetControlID="txtMapFifoDate"
                            Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                            InputDirection="LeftToRight" />
                        <ajaxToolkit:CalendarExtender ID="calMapFifoDate" CssClass="CalMaster" runat="server"
                            Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtMapFifoDate"
                            PopupButtonID="txtMapFifoDate" EnableViewState="False" >
                        </ajaxToolkit:CalendarExtender>
                    </div>
                </div>
                
                <div id="divMapLote" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblMapLote" runat="server" Text="Lote" /><br />
                        <asp:TextBox ID="txtMapLote" runat="server" Width="80px" />                     
                    </div>
                </div>
                <div id="divMapLPN" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblMapLPN" runat="server" Text="LPN" /><br />
                        <asp:TextBox ID="txtMapLPN" runat="server" Width="80px" />                     
                    </div>
                </div>
                
                <div id="divMapCategory" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblMapCategory" runat="server" Text="Categoría" /><br />
                        <asp:TextBox ID="txtMapCategory" runat="server" Width="80px" />                      
                    </div>
                </div>
                
                <div id="divMapHoldLocation" runat="server" visible="true" class="divMainFilterFloatLeftNarrow">
                    <div class="mainFilterPanelTabItem">
                        <asp:Label ID="lblMapHoldLocation" runat="server" Text="Ubic. Bloqueada" /><br />
                        <asp:CheckBox ID="chkMapHoldLocation" runat="server" ToolTip="Ubicación Bloqueada" Text=""
                            OnCheckedChanged="chkComplete_CheckedChanged" Width="130px" />
                    </div>
                </div>
            </ContentTemplate>
            
        </ajaxToolkit:TabPanel>

        <ajaxToolkit:TabPanel runat="server" ID="tabStock" HeaderText="Stock" Visible="false">
            <ContentTemplate>
                <div id="divLockedStock" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelItem">
                        <asp:CheckBox ID="chkLockedStock" runat="server" AutoPostBack="false" Checked="false" ToolTip="Stock Bloqueado" Text="Stock Bloqueado" OnCheckedChanged="chkLpnIsClosed_CheckedChanged" Width="100px" /><br />
                    </div>
                </div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>

        <ajaxToolkit:TabPanel runat="server" ID="tabMultipleChoiceOrderFilters" HeaderText="Doc. Filtros Multi Opciones" Visible="false">
            <ContentTemplate>
                <div id="divMultipleChoiceOrderFilters" runat="server" visible="true" class="divMainFilterFloatLeft">
                     <div class="mainFilterPanelItem">
                         <asp:Label ID="lblLstTrackOutboundType" runat="server" Text="Traza" /><br />
                         <asp:ListBox ID="lstTrackOutboundType" runat="server" Height="80px" Width="150px" SelectionMode="Multiple" />
                     </div>
                </div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>

        <ajaxToolkit:TabPanel runat="server" ID="tabMultipleChoiceTrackTaskFilters" HeaderText="Task Filtros Multi Opciones" Visible="false">
            <ContentTemplate>
                <div id="div3" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">  
                        <%-- <div class="mainFilterPanelItem">--%>
                             <asp:Label ID="lblLstTrackTask" runat="server" Text="Traza Tarea" /><br />
                             <asp:ListBox ID="lstLstTrackTask" runat="server" Height="80px" Width="150px" SelectionMode="Multiple" />
                         <%--</div>--%>
                    </div>
                </div>
                 <div id="div4" runat="server" visible="true" class="divMainFilterFloatLeft">
                    <div class="mainFilterPanelTabItem">  
                            <asp:Label ID="Label3" runat="server" Text="% Simulación" />
                        <br />
                        <asp:Label ID="Label4" runat="server" Text="entre" />
                        <asp:TextBox ID="txtPercentFrom" runat="server" Width="40px" />
                        <asp:RangeValidator ID="rvPercentFrom" runat="server" ErrorMessage="% Invalida"
                            Type="Integer" MinimumValue="0" MaximumValue="100" Display="Dynamic"
                            ControlToValidate="txtPercentFrom">
                        </asp:RangeValidator>
                        <asp:Label ID="Label5" runat="server" Text=" y " />
                        <asp:TextBox ID="txtPercentTo" runat="server" Width="40px" />    
                        <asp:RangeValidator ID="rvPercentTo" runat="server" ErrorMessage="% Invalida"
                            Type="Integer" MinimumValue="0" MaximumValue="100" Display="Dynamic"
                            ControlToValidate="txtPercentTo">
                        </asp:RangeValidator>
                    </div>
                </div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>

        
    </ajaxToolkit:TabContainer>
</asp:Panel>

<asp:Panel ID="pnlItemsSearch" runat="server"  CssClass="modalBox" Style="display:none">
    <asp:Panel ID="pnlHeadBar2" runat="server" CssClass="modalHeader">
        <div class="divCaption">
            <asp:Label ID="lblAddItem" runat="server" Text="Buscar Item" />
            <asp:ImageButton ID="imgBtnCloseItemSearch" runat="server" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" OnClick="imgBtnCloseItemSearch_Click" />
        </div>
        <div id="divPageGrdSearchItems" runat="server">
            <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                <asp:ImageButton ID="btnFirstGrdSearchItemsControl" runat="server" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" onclick="btnFirstGrdSearchItemsControl_Click" />
                <asp:ImageButton ID="btnPrevGrdSearchItemsControl" runat="server" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" onclick="btnPrevGrdSearchItemsControl_Click" />
                Pág. 
                <asp:DropDownList ID="ddlPagesSearchItemsControl" runat="server" AutoPostBack="true" SkinID="ddlFilter" onselectedindexchanged="ddlPagesSearchItemsControl_SelectedIndexChanged" /> 
                de 
                <asp:Label ID="lblPageCountSearchItems" runat="server" Text="" />
                <asp:ImageButton ID="btnNextGrdSearchItemsControl" runat="server" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" onclick="btnNextGrdSearchItemsControl_Click" />
                <asp:ImageButton ID="btnLastGrdSearchItemsControl" runat="server" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" onclick="btnLastGrdSearchItemsControl_Click" />
            </div>
        </div>
        </asp:Panel>
        <div class="modalControls">
            <table>
                <tr  class="mgrFilterPanelLookUp">
                    <td>
                         <asp:Label ID="lblToggleAdvancedFilter0" runat="server" Text="Dueño" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOwnerItemControl" runat="server" Width="155px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr  class="mgrFilterPanelLookUp">
                    <td>
                        <asp:RadioButtonList ID="rblSearchCriteriaControl" AutoPostBack="false" runat="server">
                            <asp:ListItem Selected="True">Código</asp:ListItem>
                            <asp:ListItem>Nombre</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td valign="top">
                        <asp:TextBox ID="txtSearchValueItem" runat="server" Width="120px"></asp:TextBox>
                        <asp:ImageButton ID="imbBuscarItemControl" ToolTip="Buscar" runat="server" OnClick="imbBuscarItemControl_Click"
                          ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" />
                    </td>
                </tr>
               <%-- <tr>
                    <td align="right">
                                                &nbsp;
                    </td>
                    <td>
                    </td>
                </tr>--%>
        </table>
            <div class="divCtrsFloatLeft">
                <div class="divLookupGrid">
                    <asp:GridView ID="grdSearchItemsControl" runat="server" DataKeyNames="Id" 
                        AllowPaging="True" 
                        onrowcommand="grdSearchItems_RowCommand" 
                        AutoGenerateColumns="False"
                        CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                        EnableTheming="false">
                        <Columns>
                            <asp:BoundField AccessibleHeaderText="Id" DataField="Id" HeaderText="ID" InsertVisible="True"
                            SortExpression="Id" />
                            <asp:TemplateField AccessibleHeaderText="ItemCode" HeaderText="Cód.">
                            <ItemTemplate>
                                <asp:Label ID="lblCode0" runat="server" Text='<%# Eval ("Code") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField AccessibleHeaderText="Description" HeaderText="Item">
                                <ItemTemplate>
                                    <asp:Label ID="lblItemName0" runat="server" Text='<%# Eval ("Description") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <center>
                                        <asp:ImageButton ID="imgBtnAddItem" ToolTip="Agregar Item" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                        Width="20px" />
                                    </center>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:BoundField AccessibleHeaderText="Codigo" DataField="Code" 
                                HeaderText="Codigo" />--%>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        <div style="clear: both" />
    </div>
</asp:Panel>
    <%-- Limpiar --%>
<asp:Label ID="lblEmptyRow" runat="server" Text="(Todos)" Visible="false" />
<asp:Label ID="lblNullOwnerRow" runat="server" Text="(Sin Dueño)" Visible="false" />
<asp:Label ID="lblEmptyRowSelect" runat="server" Text="(Seleccione)" Visible="false" />
<asp:Label ID="lblFrom" runat="server" Text="Desde" Visible="false" />
<asp:Label ID="lblTo" runat="server" Text="Hasta" Visible="false" />
<asp:Label ID="lblToolTipBtnSearch" runat="server" Text="Buscar" Visible="false" />
<asp:Label ID="lbltabDispatching" runat="server" Text="Despacho" Visible="false" />
<asp:Label ID="lbltabLayout" runat="server" Text="Layout" Visible="false" />
<asp:Label ID="lbltabLocation" runat="server" Text="Ubicación" Visible="false" />
<asp:Label ID="lbltabItemGroup" runat="server" Text="Grupo Item" Visible="false" />
<asp:Label ID="lbltabDates" runat="server" Text="Fecha" Visible="false" />
<asp:Label ID="lbltabDocument" runat="server" Text="Documento" Visible="false" />
<asp:Label ID="lbltabReceptionLog" runat="server" Text="Filtro Avanzado" Visible="false" />
<asp:Label ID="lbltabTask" runat="server" Text="Tareas" Visible="false" />
<%-- Dispatching Tab --%>