<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FilterReport.ascx.cs" 
    Inherits="Binaria.WMSTek.WebClient.Shared.FilterReport" %>
<%@ Register TagPrefix="obout" Namespace="OboutInc.Calendar2" Assembly="obout_Calendar2_NET" %>
<%-- FILTRO BASICO --%>
<div class="mainFilterPanel">
<%--Seccion de filtros para Translate --%>
<%-- Translate --%>
<div id="divTranslate" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblTranslate" runat="server" Text="Idioma" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlTranslate" runat="server" Width="114px" />
</div>
<%-- Module --%>
<div id="divModule" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblModule" runat="server" Text="Módulo" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlModule" runat="server" Width="114px" />
</div>
<%-- TypeObject --%>
<div id="divTypeObject" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblTypeObject" runat="server" Text="Tipo Objeto" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlTypeObject" runat="server" Width="250px" />
</div>
<%-- Property --%>
<div id="divProperty" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblProperty" runat="server" Text="Propiedad" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlProperty" runat="server" Width="114px" />
</div>
<%--FIN Seccion de filtros para Translate --%>

<%-- Type Reports --%>
<div id="divFilterTypeReport" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblFilterTypeReport" runat="server" Text="Reporte" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterTypeReport" runat="server" Width="200px" 
    onselectedindexchanged="ddlFilterTypeReport_SelectedIndexChanged"/>
</div>

<%-- Wharehouse --%>
<div id="divFilterWarehouse" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblWarehouse" runat="server" Text="Centro Distr." /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterWarehouse" runat="server" Width="114px" 
        onselectedindexchanged="ddlFilterWarehouse_SelectedIndexChanged1"/>
    <asp:RequiredFieldValidator ID="rfvFilterWarehouse" runat="server" 
        ControlToValidate="ddlFilterWarehouse" ErrorMessage="*" InitialValue="-1"></asp:RequiredFieldValidator>
</div>
<%-- Hangar --%>
<div id="divFilterHangar" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblHangarBase" runat="server" Text="Bodega"  /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterHangar" runat="server" Width="114px" onchange="GetLayoutMapClient(this)" />
</div>
<%-- Owner --%>
<div id="divFilterOwner" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="lblOwner" runat="server" Text="Dueño" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterOwner" runat="server" Width="114px" 
        AutoPostBack="true"
        onselectedindexchanged="ddlFilterOwner_SelectedIndexChanged" />
        <asp:RequiredFieldValidator ID="rfvFilterOwner" runat="server" 
        ControlToValidate="ddlFilterOwner" ErrorMessage="*" InitialValue="-1"></asp:RequiredFieldValidator>
</div>

<%-- OwnerUser --%>
<div id="divFilterOwnerUser" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblOwnerUser" runat="server" Text="Dueño" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterOwnerUser" runat="server" Width="114px" 
    AutoPostBack="false" SelectionMode="Multiple" 
    onselectedindexchanged="ddlFilterOwnerUser_SelectedIndexChanged" />
    <asp:RequiredFieldValidator ID="rfvFilterOwnerUser" runat="server" 
        ControlToValidate="ddlFilterOwnerUser" ErrorMessage="*" InitialValue="-1"></asp:RequiredFieldValidator>
</div>

<%-- CategoryItem --%>
<div id="divFilterCategoryItem" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblCategoryItem" runat="server" Text="Categoría" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlCategoryItem" runat="server" Width="114px"
        SelectionMode="Multiple"  />
</div>
<%-- WorkZone --%>
<div id="divFilterWorkZone" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblWorkZone" runat="server" Text="Zona" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterWorkZone" runat="server" Width="114px"
        SelectionMode="Multiple"  />
</div>
<%-- Location --%>
<div id="divFilterLocation" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblLocation" runat="server" Text="Ubicación" /><br />
    <asp:TextBox SkinID="txtFilter" ID="txtFilterLocation" runat="server" Width="60px" />
</div>
<%-- LpnType --%>
<div id="divFilterLpnType" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblLpnType" runat="server" Text="Tipo LPN" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterLpnType" runat="server" Width="114px"
        SelectionMode="Multiple" />
</div>
<%-- TruckType --%>
<div id="divFilterTruckType" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblTruckType" runat="server" Text="Tipo de Camión" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterTruckType" runat="server" Width="114px"
        SelectionMode="Multiple" />
</div>

<%-- InboundType --%>
<div id="divFilterInboundType" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblInboundType" runat="server" Text="Tipo Doc. Entrada" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterInboundType" runat="server" Width="114px" />
    <asp:RequiredFieldValidator ID="reqFilterInboundType" runat="server" 
        ControlToValidate="ddlFilterInboundType" ErrorMessage="*" InitialValue="-1" Enabled="false"></asp:RequiredFieldValidator>
</div>

<%-- Scope --%>
<div id="divFilterScope" runat="server" class="mainFilterPanelItem" visible="false">
    <asp:Label ID="lblScope" runat="server" Text="Ámbito" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterScope" runat="server" Width="114px" />
    <br />
</div>
<%-- Doc Number --%>
<div id="divFilterDocumentNumber" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblDocumentNumber" runat="server" Text="Nº Doc." /><br />
    <asp:TextBox SkinID="txtFilter" ID="txtFilterDocumentNumber" runat="server" Width="70px" />
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
        ControlToValidate="txtFilterDocumentNumber" ErrorMessage="*"></asp:RequiredFieldValidator>
</div>
<%-- Location Type --%>
<div id="divFilterLocationType" runat="server" visible="false" class="mainFilterPanelItem">
    <asp:Label ID="lblLocationType" runat="server" Text="Tipo" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterLocationType" runat="server" Width="70px" />
</div>
<%-- Code (genérico) --%>
<div id="divFilterCode" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblCode" runat="server" Text="Código" /><br />
    <asp:TextBox SkinID="txtFilter" ID="txtFilterCode" runat="server" Width="70px" />
    <asp:RequiredFieldValidator ID="rqTxtFilterCode" runat="server" 
        ControlToValidate="txtFilterCode" ErrorMessage="*" Enabled="false"></asp:RequiredFieldValidator>
</div>

<%-- Item --%>
<%--<div id="divFilterItem" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblItem" runat="server" Text="Cód. Item" /><br />
    <asp:TextBox SkinID="txtFilter" ID="txtFilterItem" runat="server" Width="70px" />
</div>--%>

    <div id="divFilterItem" runat="server" class="mainFilterPanelItem" visible="false">
        <div runat="server" style="float:left">
            <asp:Label ID="lblItem" runat="server" Text="Cód. Item" /><br />
            <asp:TextBox SkinID="txtFilter" ID="txtFilterItem" runat="server" Width="75px"  />            
        </div> 
        <div runat="server" style="float:left; vertical-align: bottom;">
            <%--<asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Always">
                <ContentTemplate>--%>
                    <asp:ImageButton ID="btnBuscarItem" ToolTip="Buscar" runat="server" 
                    ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" 
                    onclick="btnBuscarItem_Click" Height="19px" Width="20px" />     
                <%--</ContentTemplate>
            </asp:UpdatePanel> --%>    
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
    &nbsp;<asp:Label ID="lblName" runat="server" Text="Nombre" /><br />
    <asp:TextBox SkinID="txtFilter" ID="txtFilterName" runat="server" Width="70px" />
</div>
<%-- Description (genérico) --%>
<div id="divFilterDescription" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblDescription" runat="server" Text="Descripción" /><br />
    <asp:TextBox SkinID="txtFilter" ID="txtFilterDescription" runat="server" Width="70px" />
</div>

<%-- LotNumber (genérico) --%>
<div id="divFilterLotNumber" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblLotNumber" runat="server" Text="Nro Lote" /><br />
    <asp:TextBox SkinID="txtFilter" ID="txtFilterLotNumber" runat="server" Width="70px" />
</div>

<%-- SealNumber (genérico) --%>
<div id="divFilterSealNumber" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblSealNumber" runat="server" Text="SSCC" /><br />
    <asp:TextBox SkinID="txtFilter" ID="txtFilterSealNumber" runat="server" Width="70px" />
</div>

<%-- Group Item 1 (Sector) --%>
<div id="divBscGroupItems" runat="server" visible="false">
    <div id="divBscGrpItm1" runat="server" visible="true" class="mainFilterPanelItem">
        <asp:Label ID="lblBscGrpItm1" runat="server" Text="Sector" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm1" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="ddlBscGrpItm1_SelectedIndexChanged" Width="114px" />
    </div>
    <%-- Group Item 2 (Rubro) --%>
    <div id="divBscGrpItm2" runat="server" visible="true" class="mainFilterPanelItem">
        <asp:Label ID="lblBscGrpItm2" runat="server" Text="Rubro" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm2" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="ddlBscGrpItm2_SelectedIndexChanged" Width="114px" />
    </div>
    <%-- Group Item 3 (Familia) --%>
    <div id="divBscGrpItm3" runat="server" visible="true" class="mainFilterPanelItem">
        <asp:Label ID="lblBscGrpItm3" runat="server" Text="Familia" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm3" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="ddlBscGrpItm3_SelectedIndexChanged" Width="114px" />
    </div>
    <%-- Group Item 4 (Subfamilia) --%>
    <div id="divBscGrpItm4" runat="server" visible="true" class="mainFilterPanelItem">
        <asp:Label ID="lblBscGrpItm4" runat="server" Text="Subfamilia" /><br />
        <asp:DropDownList SkinID="ddlFilter" ID="ddlBscGrpItm4" runat="server" Width="114px" />
    </div>
</div>
<%-- Status --%>
<div id="divFilterStatus" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblStatus" runat="server" Text="Activo" /><br />
    <asp:DropDownList SkinID="ddlFilter" ID="ddlFilterStatus" runat="server" Width="70px">
        <asp:ListItem Text="(Todos)" Value="-1" Selected="True" />
        <asp:ListItem Text="Sí" Value="1" />
        <asp:ListItem Text="No" Value="0" />
    </asp:DropDownList>
</div>

<%-- Date Today (genérico) --%>
<div id="divFilterDate" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblDate" runat="server" Text="Fecha"/><br />
   <obout:Calendar ID="caleDate" runat="server"
         ShortDayNames="Do, Lu, Ma, Mi, Ju, Vi, Sa"
         CultureName="es-ES"
         TextBoxId="txtFilterDate"
         DatePickerMode="true"
         ShowYearSelector="true"
         YearSelectorType="DropDownList"
         YearMinScroll="2005"
         YearMaxScroll="2030"
         ShowMonthSelector="true"
         MonthSelectorType="DropDownList"
         ScrollBy="1"
         Columns="1"
         TitleText=""
         DatePickerImagePath="..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
         DatePickerSynchronize="true"
         TextArrowLeft="<"
         TextArrowRight=">"
         CSSDatePickerButton="calendarDatePickerButton"   />            
    <asp:TextBox SkinID="txtFilter" ID="txtFilterDate" runat="server" Width="80px" />
    <ajaxToolkit:MaskedEditExtender ID="meDateFilter" runat="server" TargetControlID="txtFilterDate"
        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
        InputDirection="LeftToRight">
    </ajaxToolkit:MaskedEditExtender>    
    <asp:RangeValidator ID="rvDate" runat="server" ErrorMessage="Fecha Invalida" 
        Type="Date" MinimumValue="01-01-1980" MaximumValue="01-01-2050" Display="Static" ControlToValidate="txtFilterDate">
    </asp:RangeValidator>
    <asp:RequiredFieldValidator ID="rfvDate" runat="server" 
        ControlToValidate="txtFilterDate" ErrorMessage="*" ></asp:RequiredFieldValidator> 
</div>
<%-- Date From (genérico) --%>
<div id="divFilterDateFrom" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;
    <asp:Label ID="lblDateFrom" runat="server" Text="Desde"/><br />
    <obout:Calendar ID="caleDateFrom" runat="server"
         ShortDayNames="Do, Lu, Ma, Mi, Ju, Vi, Sa"
         CultureName="es-ES"
         TextBoxId="txtFilterDateFrom"
         DatePickerMode="true"
         ShowYearSelector="true"
         YearSelectorType="DropDownList"
         YearMinScroll="2005"
         YearMaxScroll="2030"
         ShowMonthSelector="true"
         MonthSelectorType="DropDownList"
         ScrollBy="1"
         Columns="1"
         TitleText=""
         DatePickerImagePath="..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
         DatePickerSynchronize="true"
         TextArrowLeft="<"
         TextArrowRight=">"
         CSSDatePickerButton="calendarDatePickerButton"   />        
    <asp:TextBox SkinID="txtFilter" ID="txtFilterDateFrom" runat="server" Width="80px"/>
    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="txtFilterDateFrom"
        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
        InputDirection="LeftToRight">
    </ajaxToolkit:MaskedEditExtender>    
    <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Fecha Invalida" 
        Type="Date" MinimumValue="01-01-1980" MaximumValue="01-01-2050" Display="Static" ControlToValidate="txtFilterDateFrom">
    </asp:RangeValidator>    
    &nbsp;
    
<%--    <ajaxToolkit:MaskedEditExtender ID="meDateFrom" runat="server" TargetControlID="txtFilterDateFrom"
        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
        InputDirection="LeftToRight">
    </ajaxToolkit:MaskedEditExtender>
    <ajaxToolkit:CalendarExtender ID="calDateFrom" CssClass="CalMaster" runat="server"
        Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtFilterDateFrom" PopupButtonID="txtFilterDateFrom"
        EnableViewState="False">
    </ajaxToolkit:CalendarExtender>--%>
</div>
<%-- Date To (genérico) --%>
<div id="divFilterDateTo" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblDateTo" runat="server" Text="Hasta" /><br />
    <obout:Calendar ID="caleDateTo" runat="server"
         ShortDayNames="Do, Lu, Ma, Mi, Ju, Vi, Sa"
         CultureName="es-ES"
         TextBoxId="txtFilterDateTo"
         DatePickerMode="true"
         ShowYearSelector="true"
         YearSelectorType="DropDownList"
         YearMinScroll="2005"
         YearMaxScroll="2030"
         ShowMonthSelector="true"
         MonthSelectorType="DropDownList"
         ScrollBy="1"
         Columns="1"
         TitleText=""
         DatePickerImagePath="..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
         DatePickerSynchronize="true"
         TextArrowLeft="<"
         TextArrowRight=">"
         CSSDatePickerButton="calendarDatePickerButton"   />        
    <asp:TextBox SkinID="txtFilter" ID="txtFilterDateTo" runat="server" Width="80px"  Enabled="true"/>
    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="txtFilterDateTo"
        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
        InputDirection="LeftToRight">
    </ajaxToolkit:MaskedEditExtender>    
    <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="Fecha Invalida" 
        Type="Date" MinimumValue="01-01-1980" MaximumValue="01-01-2050" Display="Static" ControlToValidate="txtFilterDateTo">
    </asp:RangeValidator>    
    &nbsp;    
      &nbsp;
<%--    <ajaxToolkit:MaskedEditExtender ID="meDateTo" runat="server" TargetControlID="txtFilterDateTo"
        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
        InputDirection="LeftToRight">
    </ajaxToolkit:MaskedEditExtender>
    <ajaxToolkit:CalendarExtender ID="calDateTo" CssClass="CalMaster" runat="server"
        Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtFilterDateTo" PopupButtonID="txtFilterDateTo"
        EnableViewState="False">
    </ajaxToolkit:CalendarExtender>--%>
</div>
<%-- Fifo Date From --%>
<div id="divFilterFifoDateFrom" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblFifoDateFrom" runat="server" Text="Fifo Desde" /><br />
    <obout:Calendar ID="caleFifoDateFrom" runat="server"
         ShortDayNames="Do, Lu, Ma, Mi, Ju, Vi, Sa"
         CultureName="es-ES"
         TextBoxId="txtFilterFifoDateFrom"
         DatePickerMode="true"
         ShowYearSelector="true"
         YearSelectorType="DropDownList"
         YearMinScroll="2005"
         YearMaxScroll="2030"
         ShowMonthSelector="true"
         MonthSelectorType="DropDownList"
         ScrollBy="1"
         Columns="1"
         TitleText=""
         DatePickerImagePath="..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
         DatePickerSynchronize="true"
         TextArrowLeft="<"
         TextArrowRight=">"
         CSSDatePickerButton="calendarDatePickerButton"   />            
    <asp:TextBox ID="txtFilterFifoDateFrom" runat="server" Width="75px" SkinID="txtFilter"  Enabled="true" />
    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender3" runat="server" TargetControlID="txtFilterDateTo"
        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
        InputDirection="LeftToRight">
    </ajaxToolkit:MaskedEditExtender>    
    <asp:RangeValidator ID="RangeValidator3" runat="server" ErrorMessage="Fecha Invalida" 
        Type="Date" MinimumValue="01-01-1900" MaximumValue="01-01-2090" Display="Static" ControlToValidate="txtFilterDateTo">
    </asp:RangeValidator>      
    
</div>
<%-- Fifo Date To --%>
<div id="divFilterFifoDateTo" runat="server" class="mainFilterPanelItem" visible="false">
    &nbsp;<asp:Label ID="lblFifoDateTo" runat="server" Text="Fifo Hasta" /><br />
    <obout:Calendar ID="caleFifoDateTo" runat="server"
         ShortDayNames="Do, Lu, Ma, Mi, Ju, Vi, Sa"
         CultureName="es-ES"
         TextBoxId="txtFilterFifoDateTo"
         DatePickerMode="true"
         ShowYearSelector="true"
         YearSelectorType="DropDownList"
         YearMinScroll="2005"
         YearMaxScroll="2030"
         ShowMonthSelector="true"
         MonthSelectorType="DropDownList"
         ScrollBy="1"
         Columns="1"
         TitleText=""
         DatePickerImagePath="..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
         DatePickerSynchronize="true"
         TextArrowLeft="<"
         TextArrowRight=">"
         CSSDatePickerButton="calendarDatePickerButton"   />        
    <asp:TextBox ID="txtFilterFifoDateTo" runat="server" Width="75px" SkinID="txtFilter"  Enabled="true"/>
    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender4" runat="server" TargetControlID="txtFilterFifoDateTo"
        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
        InputDirection="LeftToRight">
    </ajaxToolkit:MaskedEditExtender>    
    <asp:RangeValidator ID="RangeValidator4" runat="server" ErrorMessage="Fecha Invalida" 
        Type="Date" MinimumValue="01-01-1900" MaximumValue="01-01-2090" Display="Static" ControlToValidate="txtFilterFifoDateTo">
    </asp:RangeValidator>       
<%--    <ajaxToolkit:MaskedEditExtender ID="meFifoDateTo" runat="server" TargetControlID="txtFilterFifoDateTo"
        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
        InputDirection="LeftToRight">
    </ajaxToolkit:MaskedEditExtender>
    <ajaxToolkit:CalendarExtender ID="calFifoDateTo" CssClass="CalMaster" runat="server"
        Enabled="true" FirstDayOfWeek="Sunday" TargetControlID="txtFilterFifoDateTo"
        PopupButtonID="txtFilterFifoDateTo" EnableViewState="False">
    </ajaxToolkit:CalendarExtender>--%>
</div>

    <%-- Year (genérico) --%>
    <div id="divFilterYear" runat="server" class="mainFilterPanelItem" visible="false">
        <asp:Label ID="LblYear" runat="server" Text="Año" /><br />
        <asp:TextBox SkinID="txtFilter" ID="txtFilterYear" runat="server" Width="80px" />
        <ajaxToolkit:MaskedEditExtender ID="txtFilterYear_MaskedEditExtender" runat="server"
            CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat=""
            CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder=""
            CultureTimePlaceholder=""  Enabled="True" Mask="9999" MaskType="Number" TargetControlID="txtFilterYear">
        </ajaxToolkit:MaskedEditExtender>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" Text="Número Invalido"
            ControlToValidate="txtFilterYear" Display="Dynamic" EnableClientScript="true"
            ValidationExpression="\d+" ValidationGroup="FilterSearch" />
     <%--   <asp:RangeValidator ID="RangeValidator_Year" runat="server" ErrorMessage="Año debe estar entre 2012 y 2030"
         ControlToValidate= "txtFilterYear" MinimumValue="2012" MaximumValue="2030" ></asp:RangeValidator> --%> 
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
        ControlToValidate="txtFilterYear" ErrorMessage="*" ></asp:RequiredFieldValidator> 
    </div>

<%-- Fecha Fabricacion --%>
<div id="divFilterFabricationDate" runat="server" class="mainFilterPanelItem" style="max-width: 120px;" visible="false">
    &nbsp;
    <asp:Label ID="lblFabricationDate" runat="server" Text="Fecha Fabricación" /><br />
    <obout:Calendar ID="calFabricationDate" runat="server"
         ShortDayNames="Do, Lu, Ma, Mi, Ju, Vi, Sa"
         CultureName="es-ES"
         TextBoxId="txtFilterFabricationDate"
         DatePickerMode="true"
         ShowYearSelector="true"
         YearSelectorType="DropDownList"
         YearMinScroll="2005"
         YearMaxScroll="2030"
         ShowMonthSelector="true"
         MonthSelectorType="DropDownList"
         ScrollBy="1"
         Columns="1"
         TitleText=""
         DatePickerImagePath="..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
         DatePickerSynchronize="true"
         TextArrowLeft="<"
         TextArrowRight=">"
         CSSDatePickerButton="calendarDatePickerButton"   />        
    <asp:TextBox ID="txtFilterFabricationDate" runat="server" Width="75px" SkinID="txtFilter"  Enabled="true"/>
    <asp:CheckBox ID="chkFilterFabricationDate" runat="server" ToolTip="Activar Fecha" Checked="false" />
    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender5" runat="server" TargetControlID="txtFilterFabricationDate"
        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
        InputDirection="LeftToRight">
    </ajaxToolkit:MaskedEditExtender>    
    <asp:RangeValidator ID="RangeValidator5" runat="server" ErrorMessage="Fecha Invalida" 
        Type="Date" MinimumValue="01-01-1900" MaximumValue="01-01-2090" Display="Static" ControlToValidate="txtFilterFabricationDate">
    </asp:RangeValidator>
</div>

<%-- Fecha Expiracion --%>
<div id="divFilterExpirationDate" runat="server" class="mainFilterPanelItem" style="max-width: 120px;" visible="false">
    &nbsp;
    <asp:Label ID="lblExpirationDate" runat="server" Text="Fecha Expiración" /><br />
    <obout:Calendar ID="calExpirationDate" runat="server"
         ShortDayNames="Do, Lu, Ma, Mi, Ju, Vi, Sa"
         CultureName="es-ES"
         TextBoxId="txtFilterExpirationDate"
         DatePickerMode="true"
         ShowYearSelector="true"
         YearSelectorType="DropDownList"
         YearMinScroll="2005"
         YearMaxScroll="2030"
         ShowMonthSelector="true"
         MonthSelectorType="DropDownList"
         ScrollBy="1"
         Columns="1"
         TitleText=""
         DatePickerImagePath="..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
         DatePickerSynchronize="true"
         TextArrowLeft="<"
         TextArrowRight=">"
         CSSDatePickerButton="calendarDatePickerButton"   />        
    <asp:TextBox ID="txtFilterExpirationDate" runat="server" Width="75px" SkinID="txtFilter"  Enabled="true"/>
    <asp:CheckBox ID="chkFilterExpirationDate" runat="server" ToolTip="Activar Fecha" Checked="false" />
    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender6" runat="server" TargetControlID="txtFilterExpirationDate"
        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
        InputDirection="LeftToRight">
    </ajaxToolkit:MaskedEditExtender>    
    <asp:RangeValidator ID="RangeValidator6" runat="server" ErrorMessage="Fecha Invalida" 
        Type="Date" MinimumValue="01-01-1900" MaximumValue="01-01-2090" Display="Static" ControlToValidate="txtFilterExpirationDate">
    </asp:RangeValidator>   
</div>
    


<%-- Boton 'Buscar' --%>
<div class="mainFilterPanelItem">
<%--    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Always">
        <ContentTemplate>--%>
            <asp:ImageButton ID="btnSearch" runat="server" ToolTip="Buscar" OnClick="btnSearch_Click" 
                ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" />
        <%--</ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprSearch" runat="server" AssociatedUpdatePanelID="upSearch"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <img src="../WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprSearch" runat="server" ControlToOverlayID="divTop"
        CssClass="updateProgress" TargetControlID="uprSearch" />--%>
</div>


<%-- Opciones 'Filtro Avanzado' --%>
<div id="divAdvancedFilterOptions" runat="server" visible="false" class="divAdvancedFilterOptions">
    <div>
        Filtro Avanzado</div>
    <%-- Mostrar / Ocultar --%>
    <div id="divToggleAdvancedFilter" runat="server" class="divAdvancedFilterOption">
        <asp:ImageButton ID="imgToggleAdvancedFilter" ToolTip="Mostrar / Ocultar" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_unchecked.png" />
        <asp:Label ID="lblToggleAdvancedFilter" Text="Visible" runat="server" />
    </div>
    <%-- Activar --%>
    <div id="divAdvancedFilterChk" runat="server" visible="false" class="divAdvancedFilterOption">
        <asp:CheckBox ID="chkUseAdvancedFilter" runat="server" Text="Activo" />
    </div>
    <%--     <asp:Button ID="btnActivateAdvancedFilter" runat="server" Text="Activo" SkinID="btnSmall" OnClientClick="return false;"/>
                <asp:Button ID="btnInactivateAdvancedFilter" runat="server" Text="Activo" SkinID="btnSmallOn" Visible="false"/>
--%>
    <%-- Limpiar --%>
    <asp:Button ID="btnReset" runat="server" Text="Limpiar" OnClick="btnReset_Click"
        SkinID="btnSmall" />
</div>



<asp:Panel ID="pnlItemsSearch" runat="server"  CssClass="modalBox" Style="display:none;" >
    <asp:Panel ID="pnlHeadBar2" runat="server" CssClass="modalHeader"  >
        <div class="divCaption">
            <asp:Label ID="lblAddItem" runat="server" Text="Buscar Item" />
            <asp:ImageButton ID="imgBtnCloseItemSearch" runat="server" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
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
                         <asp:Label ID="lblToggleAdvancedFilter0" runat="server" 
                             Text="Dueño" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOwnerItemControl" runat="server" Width="155px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr  class="mgrFilterPanelLookUp">
                    <td>
                        <asp:RadioButtonList ID="rblSearchCriteriaControl" runat="server">
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
                <tr>
                    <td align="right">
                                                &nbsp;
                    </td>
                    <td>
                    </td>
                </tr>
        </table>
            <div class="divCtrsFloatLeft">
                <div class="divLookupGrid">
                    <asp:GridView ID="grdSearchItemsControl" runat="server" DataKeyNames="Id" 
                        AllowPaging="True" onrowcommand="grdSearchItems_RowCommand" 
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
                        <asp:BoundField AccessibleHeaderText="Codigo" DataField="Code" 
                            HeaderText="Codigo" />
                    </Columns>
                    </asp:GridView>
                </div>
            </div>
        <div style="clear: both" />
    </div>
</asp:Panel>

<%-- Mensajes de Confirmacion y Auxiliares --%>
<asp:Label ID="lblEmptyRow" runat="server" Text="(Todos)" Visible="false" />
<asp:Label ID="lblEmptyRowSelect" runat="server" Text="(Seleccione)" Visible="false" />
<asp:Label ID="lblFrom" runat="server" Text="Desde" Visible="false" />
<asp:Label ID="lblTo" runat="server" Text="Hasta" Visible="false" />
<%-- FIN Mensajes de Confirmacion y Auxiliares --%>
<%-- FIN FILTRO BASICO --%>
</div>

