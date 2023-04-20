<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="Desktop.aspx.cs"  Inherits="Binaria.WMSTek.WebClient.Shared.Desktop" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register TagPrefix="webUc" TagName="ucMenu" Src="~/Shared/MainMenuContent.ascx" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

    <!-- Css's Files -->
    <link href="../WebResources/Styles/ModalPopup.css" rel="stylesheet" type="text/css" />

     <style  type="text/css">
        .rfKPIValue {
            text-shadow: 1px 1px 1px 0 #e8e8e8;
            color: #2967a6;
            font-size: 35px;
            cursor: pointer;
        }
        
        .rfKPICaption {
            color: #666;
            display: inline-block;
            font-size: 20px;
            cursor: pointer;
        }
        
        .lblKpiText
        {
        	text-shadow: 2px 2px 2px 2px #e8e8e8;
            color: #666;
            font-size: 11px;
        }

         #containerDesktop{
             margin-top: 70px;
             height: 550px;
             overflow: auto;
         }

         .border {
            margin-left: 15px
         }

         /* control panel */
         .pn {
            height: 120px;
            box-shadow: 0 2px 1px rgba(0, 0, 0, 0.2);
        }

        .darkblue-panel:hover {
            border: 1px solid;
            box-shadow: 7px 7px 4px #888888;
        }


        .darkblue-panel {
            text-align: center;
            background: #174686;
            color: white;
            margin-top: 10px;
            border-radius : 10px;
        }

        .darkblue-panel .darkblue-header {
            background: transparent;
            padding: 3px;
            margin-bottom: 10px;
            border-bottom: 1px solid;
            min-height: 55px;
        }

        .icon-panel {
            font-size: 3em;
            padding-top: 5px;
            padding-bottom: 10px;
        }

        #panelTitle {
            box-shadow: 0 0.25rem 0.75rem rgba(0, 0, 0, .05);
            background-color: #577B96;
            color: rgba(255, 255, 255, .5);
            border-radius: .25rem !important;
            padding: 3px 3px 3px 20px;
            margin-left: 15px;
        }

        #headerTitle{
            color: white;
        }

        #panelControlTitle {
            background-color: white;
            padding: 5px;
            height: 100%;
        }

        ul.nav-tabs > li {
            background-color: white;
        }

        .tab-content{
            background-color: white;
            padding-top: 10px;
            padding-left: 5px;
        }

        #ctl00_divMainFilter, #ctl00_ucTaskBarContent_divTaskBar{
            display: none;
        }

        /* panel de control */
        .tile_count {
            margin-bottom: 20px;
            margin-top: 20px;
            margin-left: 1px;
        }

        .tile_count {
            margin-bottom: 20px;
            margin-top: 20px;
            background-color: #DCDCDC;
        }

        .tile_count .tile_stats_count {
            border-bottom: 1px solid #D9DEE4;
            padding: 0 10px 0 20px;
            text-overflow: ellipsis;
            overflow: hidden;
            white-space: nowrap;
            position: relative;
            min-height: 114px;
        }

        .tile_count .tile_stats_count:hover {
            background-color: #A9A9A9;
        }

        .tile_count .tile_stats_count:before {
            content: "";
            position: absolute;
            left: 0;
            height: 100%;
            border-left: 2px solid #577B96;
            margin-top: 1px
        }

        .tile_count .tile_stats_count .count {
            font-size: 40px;
            line-height: 47px;
            font-weight: 600;
        }

        .tile_count .tile_stats_count span {
            font-size: 16px
        }

        .tile_count .tile_stats_count .count_bottom i {
            width: 12px
        }

        .labelDetailKPI{
            display: block;
        }

        #controlPanel .row{
            margin-right: 0;
            cursor: default;
        }

        #ctl00_MainContent_chkEnableTimerControlPanel + label {
            font-weight: normal;
        }

        #ctl00_MainContent_divInfoKpiChartContent{
            background-color: #ebebe0;
        }

        .hideElement {
	        visibility: hidden;
        }

        .modalControls {
            height: 500px;
            max-height: 500px;
        }
        /* fin panel de control */
     </style>   
    
        <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/Chart.min.js")%>"></script>
        <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/Chart.bundle.min.js")%>"></script>
        <script src="<%= Page.ResolveClientUrl("~/WebResources/Script/customCharts.js")%>"></script>

        <script>
            $(function () {

                $("#ctl00_imgMenu").click(function () {
                    setWidthCustom();
                });

                //LLamados KPI Charts
                $("#updchartKpiZone").click(function (e) {
                    e.preventDefault();

                    var idCanvas = "chartKpiZone";
                    $("#" + idCanvas).remove();
                    $("#kpiZone div.row").after(createCanvasDesktop(idCanvas));

                    KpiZone(parseInt($("#ctl00_ddlWarehouse").val()), $("#ctl00_MainContent_ddlKpiZone").val(), $("#ctl00_MainContent_ddlKpiZoneUnit").val());
                })
                              

                $("#updchartKpiPicking").click(function (e) {
                    e.preventDefault();

                    var idCanvas = "chartKpiPicking";
                    $("#" + idCanvas).remove();
                    $("#kpiPicking div.row").after(createCanvasDesktop(idCanvas));

                    KpiPicking(parseInt($("#ctl00_ddlWarehouse").val()), $("#ctl00_MainContent_ddlUserCharts4").val(), $("#ctl00_MainContent_ddlKpiPickingUnid").val());
                });

                $("#updchartKpiFillRate").click(function (e) {
                    e.preventDefault();                    

                    var idCanvas = "chartKpiFillRate";
                    $("#" + idCanvas).remove();
                    $("#kpiFillRate div.row").after(createCanvasDesktop(idCanvas));

                    KpiFillRate(parseInt($("#ctl00_ddlOwner").val()), parseInt($("#ctl00_ddlWarehouse").val()), $("#ctl00_MainContent_ddlCustomerCharts5").val());
                })

                $("#updChartKpiLeadTime").click(function (e) {
                    e.preventDefault();

                    var idCanvas = "chartKpiLeadTime";
                    $("#" + idCanvas).remove();
                    $("#kpiChartKpiLeadTime div.row").after(createCanvasDesktop(idCanvas));

                    KpiLeadTime(parseInt($("#ctl00_ddlOwner").val()), parseInt($("#ctl00_ddlWarehouse").val()), $("#ctl00_MainContent_ddlLeadTime").val(), $("#ctl00_MainContent_ddlLeadTimeCustomer").val());
                });
            });

            function setWidthCustom() {                
                var isHideMenu = parseInt($("#sidedrawer").css("transform").split(",")[4].trim()) == 0 ? true : false;

                if (isHideMenu == true) {
                    $(".setWidthCustom").removeClass("col-md-12").addClass("col-md-offset-1").addClass("col-md-11");
                } else {    
                    setMaxWidthCustom();
                }
            }

            function setMaxWidthCustom() {
                $(".setWidthCustom").removeClass("col-md-offset-1").removeClass("col-md-11").addClass("col-md-12");
            }

            function createCanvasDesktop(id) {
                var canvas =
                    $('<canvas/>', {
                        'id': id
                    });

                return canvas;
            }



            function loadCustomer() {
                
                var param = {idOwn: parseInt($("#ctl00_ddlOwner").val())};

                $.ajax({
                        type: "POST",
                        url: urlWSCharts() + "loadCustomer",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify(param),
                        success: function (res, status, jqXHR) {
                            //Limpia el combo
                            var theDropDown = document.getElementById("ctl00_MainContent_ddlCustomerCharts5");
                            theDropDown.innerHTML = '';

                            //Llenado del Combo
                            $.each(res.d, function (data, value) {  
                                $("#ctl00_MainContent_ddlCustomerCharts5").append($("<option></option>").val(value.Code).html(value.Name)); 
                            })

                            //Selecciona elemento Todos
                            $("#ctl00_MainContent_ddlCustomerCharts5").val("-1");
                        },
                        error: function (jqXHR, status, err) {
                            console.error("Error en loadCustomer");
                        }
                    })
            }

            function loadTypeCustomer() {
                
                var param = {idOwn: parseInt($("#ctl00_ddlOwner").val())};

                $.ajax({
                        type: "POST",
                        url: urlWSCharts() + "loadTypeCustomer",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify(param),
                        success: function (res, status, jqXHR) {
                            //Limpia el combo
                            var theDropDown = document.getElementById("ctl00_MainContent_ddlLeadTimeCustomer");
                            theDropDown.innerHTML = '';

                            //Llenado del Combo
                            $.each(res.d, function (data, value) {  
                                $("#ctl00_MainContent_ddlLeadTimeCustomer").append($("<option></option>").val(value.Value).html(value.Text)); 
                            })

                            //Selecciona elemento Todos
                            $("#ctl00_MainContent_ddlLeadTimeCustomer").val("-1");
                        },
                        error: function (jqXHR, status, err) {
                            console.error("Error en loadTypeCustomer");
                        }
                    })
            }
        </script>

      <div id="header2">

        <div id="gradiente">

        </div>

        <asp:UpdatePanel ID="upOptions" runat="server" UpdateMode="Always">
             <ContentTemplate>          
                <div id="profile">                    
                    <%--TODO: mostrar como opciones avanzadas (no visible) --%>
                    <asp:CheckBox ID="chkKeepFilter" runat="server" Text="Mantener Filtro" AutoPostBack="true" OnCheckedChanged="chkKeepFilter_CheckedChanged" ToolTip="Mantener valores del Filtro entre consultas" Visible="false"/>    
                      
                 </div>
            </ContentTemplate>    
        </asp:UpdatePanel>            
        
      </div>
      <!-- End Desktop Header Section -->    
  
        <%-- Mensajes de Confirmacion y Auxiliares --%>
        <asp:Label ID="lblEmptyRow" runat="server" Text="(Todos)" Visible="false" />
        <%-- FIN Mensajes de Confirmacion y Auxiliares --%>      

        <!-- panel -->
        <div id="containerDesktop" class="container">

            <div class="row">
                <div class="col-md-offset-1 col-md-11 setWidthCustom">

                    <ul class="nav nav-tabs border">
                        <li class="active"><a data-toggle="tab" href="#home">Home</a></li>
                        <li><a data-toggle="tab" href="#controlPanel">Panel de Control</a></li>
                        <li><a data-toggle="tab" href="#kpiZone">Zona</a></li>
                        <li><a data-toggle="tab" href="#kpiPicking">Picking</a></li>
                        <li><a data-toggle="tab" href="#kpiFillRate" onclick="loadCustomer()">Fill Rate</a></li>
                        <li><a data-toggle="tab" href="#kpiChartKpiLeadTime" onclick="loadTypeCustomer()">Lead Time</a></li>
                    </ul>

                    <div class="tab-content border">
                        <div id="home" class="tab-pane fade in active">
                            <div id="panelControlTitle" class="panel panel-default">
                                <div class="panel-heading">
                                    <h4 class="text-center">Accesos Directos</h4>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-md-2 mb">
                                            <a href="../Administration/Devices/TerminalMonitor.aspx" target="_blank">
                                                <div class="darkblue-panel pn">
                                                    <div class="darkblue-header">
                                                        <h5>Monitor de Terminales</h5>
                                                    </div>
                                                    <div class="row">
                                                        <i class="fa fa-mobile fa-lg icon-panel" aria-hidden="true"></i>
                                                    </div>
                                                </div>
                                            </a>
                                        </div>

                                         <div class="col-md-2 mb">
                                            <a href="../Inbound/Consult/InboundOrderConsult.aspx?IT=99" target="_blank">
                                                <div class="darkblue-panel pn">
                                                    <div class="darkblue-header">
                                                        <h5>Documentos de Entrada</h5>
                                                    </div>
                                                    <div class="row">
                                                        <i class="fa fa-arrow-circle-o-up fa-lg icon-panel" aria-hidden="true"></i>
                                                    </div>
                                                </div>
                                            </a>
                                        </div>

                                         <div class="col-md-2 mb">
                                            <a href="../Inbound/Administration/ReceiptConfirm.aspx?IT=99" target="_blank">
                                                <div class="darkblue-panel pn">
                                                    <div class="darkblue-header">
                                                        <h5>Confirmar Recepción</h5>
                                                    </div>
                                                    <div class="row">
                                                        <i class="fa fa-check-square-o fa-lg icon-panel" aria-hidden="true"></i>
                                                    </div>
                                                </div>
                                            </a>
                                        </div>

                                        <div class="col-md-2 mb">
                                            <a href="../Inbound/Consult/VendorServiceLevel.aspx" target="_blank">
                                                <div class="darkblue-panel pn">
                                                    <div class="darkblue-header">
                                                        <h5>Nivel de Servicios Proveedores</h5>
                                                    </div>
                                                    <div class="row">
                                                        <i class="fa fa-thermometer-three-quarters fa-lg icon-panel" aria-hidden="true"></i>
                                                    </div>
                                                </div>
                                            </a>
                                        </div>

                                        <div class="col-md-2 mb">
                                            <a href="../OutBound/Administration/ReleaseOrderMgr.aspx" target="_blank">
                                                <div class="darkblue-panel pn">
                                                    <div class="darkblue-header">
                                                        <h5>Liberar Pedidos</h5>
                                                    </div>
                                                    <div class="row">
                                                        <i class="fa fa-pencil-square-o fa-lg icon-panel" aria-hidden="true"></i>
                                                    </div>
                                                </div>
                                            </a>
                                        </div>

                                        <div class="col-md-2 mb">
                                            <a href="../OutBound/Consult/DispatchAdvanceConsult.aspx" target="_blank">
                                                <div class="darkblue-panel pn">
                                                    <div class="darkblue-header">
                                                        <h5>Avance Salidas</h5>
                                                    </div>
                                                    <div class="row">
                                                        <i class="fa fa-forward fa-lg icon-panel" aria-hidden="true"></i>
                                                    </div>
                                                </div>
                                            </a>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="controlPanel" class="tab-pane fade">
                            <div id="divInfoKpiChart" runat="server">
                                
                                <div class="row">
                                    <div class="col-md-2">
                                        <asp:Button id="btnUpdchartPanelControl" runat="server" Text="Actualizar" OnClick="btnUpdchartPanelControl_Click" CssClass="btn btn-default"></asp:Button> 
                                    </div>
                                    <div class="col-md-5">
                                        <label>Fecha Inicio</label>

                                        <asp:TextBox SkinID="txtFilter" ID="txtStartDateFilterPanelControl" runat="server" Width="70px" ToolTip="Ingrese fecha." />
                                        <asp:RangeValidator ID="rvStartDateFilterPanelControl" runat="server" ControlToValidate="txtStartDateFilterPanelControl"
                                            ErrorMessage="Fecha debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MinimumValue="01/01/2000"
                                            MaximumValue="31/12/2040" Type="Date" />
                                        <ajaxToolkit:CalendarExtender ID="calStartDateFilterPanelControl" CssClass="CalMaster" runat="server"
                                            Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtStartDateFilterPanelControl" PopupButtonID="txtStartDateFilterPanelControl"
                                            Format="dd/MM/yyyy">
                                        </ajaxToolkit:CalendarExtender>
                                        
                                    </div>
                                    <div class="col-md-5">
                                        <label>Fecha Término</label>
                                        
                                        <asp:TextBox SkinID="txtFilter" ID="txtEndDateFilterPanelControl" runat="server" Width="70px" ToolTip="Ingrese fecha." />
                                        <asp:RangeValidator ID="rvEndDateFilterPanelControl" runat="server" ControlToValidate="txtEndDateFilterPanelControl"
                                            ErrorMessage="Fecha debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MinimumValue="01/01/2000"
                                            MaximumValue="31/12/2040" Type="Date" />
                                        <ajaxToolkit:CalendarExtender ID="calEndDateFilterPanelControl" CssClass="CalMaster" runat="server"
                                            Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtEndDateFilterPanelControl" PopupButtonID="txtEndDateFilterPanelControl"
                                            Format="dd/MM/yyyy">
                                        </ajaxToolkit:CalendarExtender>

                                    </div>
                                </div>

                                <asp:Timer ID="timerRefreshControlPanel" runat="server" Interval="600000" OnTick="timerRefreshControlPanel_Tick" Enabled="false"></asp:Timer>

                                <asp:UpdatePanel ID="udpInfoKpi" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>  

                                        <div class="row" style="padding-top: 10px">
                                            <div class="col-md-2">
                                                <asp:CheckBox ID="chkEnableTimerControlPanel" AutoPostBack="true" Text="Auto Refresco" Checked="false" runat="server" OnCheckedChanged="chkEnableTimerControlPanel_CheckedChanged" />
                                            </div>
                                            <div class="col-md-10">
                                                <asp:Label ID="lblLastTimeUpdatedControlPanel" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>

                                        <div id="divInfoKpiChartContent" runat="server" visible="false">

                                              <div class="row tile_count">

                                                <div class="col-md-3 tile_stats_count" id="divKpiTotal" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Total
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiTotal" runat="server" Text="" CssClass="count"></asp:Label>
                                                    </div>
                                                    <br />
                                                    <span class="count_bottom">
                                                        Pedidos
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnTotalData" runat="server" Text="Datos" OnClick="btnTotalData_Click" />
                                                    </div>
                                                </div>

                                                <div class="col-md-3 tile_stats_count" id="divKpiDispatch" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Despachos
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiDispatch" runat="server" Text="" CssClass="count"></asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiDispatchDesc" runat="server" Text="0/0"> </asp:Label>
                                                    </div>
                                                    <span class="count_bottom">
                                                        Pedidos
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnDispatchData" runat="server" Text="Datos" OnClick="btnDispatchData_Click" />
                                                    </div>
                                                </div>

                                                <div class="col-md-3 tile_stats_count" id="divKpiPending" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Pendientes
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiPending" runat="server" Text="" CssClass="count"></asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiPendingDesc" runat="server" Text="0/0"> </asp:Label>
                                                    </div>
                                                    <span class="count_bottom">
                                                        Pedidos
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnPendingData" runat="server" Text="Datos" OnClick="btnPendingData_Click" />
                                                    </div>
                                                </div>

                                                <div class="col-md-3 tile_stats_count" id="divKpiReleased" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Liberados
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiReleased" runat="server" Text="" CssClass="count"> </asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiReleasedDesc" runat="server" Text="0/0"> </asp:Label>
                                                    </div>
                                                    <span class="count_bottom">
                                                        Pedidos
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnReleasedData" runat="server" Text="Datos" OnClick="btnReleasedData_Click" />
                                                    </div>
                                                </div>

                                            </div>

                                            <div class="row tile_count">

                                                <div class="col-md-4 tile_stats_count" id="divKpiPicking" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Picking
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiPicking" runat="server" Text="" CssClass="count"> </asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiPickingDesc" runat="server" Text="0/0"> </asp:Label>
                                                    </div>
                                                    <span class="count_bottom">
                                                        Pedidos
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnPickingData" runat="server" Text="Datos" OnClick="btnPickingData_Click" />
                                                    </div>
                                                </div>

                                                <div class="col-md-4 tile_stats_count" id="divKpiWavePicking" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Picking Ola
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiPickingWave" runat="server" Text="" CssClass="count"> </asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiPickingWaveDesc" runat="server" Text="0/0"> </asp:Label>
                                                    </div>
                                                    <span class="count_bottom">
                                                        Pedidos
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnPickingWaveData" runat="server" Text="Datos" OnClick="btnPickingWaveData_Click" />
                                                    </div>
                                                </div>

                                                <div class="col-md-4 tile_stats_count" id="divKpiPacking" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                         Packing
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiPacking" runat="server" Text="" CssClass="count" ToolTip="Total"></asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiPackingDesc" runat="server" Text="0/0"> </asp:Label>
                                                    </div>
                                                    <span class="count_bottom">
                                                        Pedidos
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnPackingData" runat="server" Text="Datos" OnClick="btnPackingData_Click" />
                                                    </div>
                                                </div>

                                            </div>


                                            <div class="row tile_count">

                                                <div class="col-md-4 tile_stats_count" id="divKpiSorting" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Distribución
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiSorting" runat="server" Text="" CssClass="count"></asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiSortingDesc" runat="server" Text="0/0"> </asp:Label>   
                                                    </div>
                                                    <span class="count_bottom">
                                                        Pedidos
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnSortingData" runat="server" Text="Datos" OnClick="btnSortingData_Click" />
                                                    </div>
                                                </div>
                 
                                                <div class="col-md-4 tile_stats_count" id="divKpiRouted" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Ruteo
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiRoute" runat="server" Text="" CssClass="count"></asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiRouteDesc" runat="server" Text="0/0"> </asp:Label>   
                                                    </div>
                                                    <span class="count_bottom">
                                                        Pedidos
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnRoutedData" runat="server" Text="Datos" OnClick="btnRoutedData_Click" />
                                                    </div>
                                                </div>

                                                <div class="col-md-4 tile_stats_count" id="divKpiPut" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Almacenamiento
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiPut" runat="server" Text="" CssClass="count"></asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiPutDesc" runat="server" Text="0/0"> </asp:Label>   
                                                    </div>
                                                    <span class="count_bottom">
                                                        Tareas
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnPutData" runat="server" Text="Datos" OnClick="btnPutData_Click" />
                                                    </div>
                                                </div>

                                            </div>

                                            <div class="row tile_count">

                                                <div class="col-md-4 tile_stats_count" id="divKpiReplanishment" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Reposición
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiReplanishment" runat="server" Text="" CssClass="count"></asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiReplanishmentDesc" runat="server" Text="0/0"> </asp:Label>   
                                                    </div>
                                                    <span class="count_bottom">
                                                        Tareas
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnReplanishmentData" runat="server" Text="Datos" OnClick="btnReplanishmentData_Click" />
                                                    </div>
                                                </div>

                                                <div class="col-md-4 tile_stats_count" id="divKpiAdjust" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Conteo Cíclicos y Ajustes
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiAdjust" runat="server" Text="" CssClass="count"></asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiAdjustDesc" runat="server" Text="0/0"> </asp:Label>   
                                                    </div>
                                                    <span class="count_bottom">
                                                        Tareas
                                                    </span>
                                                    <div>
                                                        <asp:Button ID="btnAdjustData" runat="server" Text="Datos" OnClick="btnAdjustData_Click" />
                                                    </div>
                                                </div>

                                                 <div class="col-md-4 tile_stats_count" id="divKpiLpnGuidedMovement" runat="server">
                                                    <span class="count_top">
                                                        <i class="fa fa-info-circle"></i> 
                                                        Movimiento Dirigido LPN
                                                    </span>
                                                    <div class="count">
                                                        <asp:Label ID="lblKpiGuidedMovement" runat="server" Text="" CssClass="count"></asp:Label>
                                                    </div>
                                                    <div class="labelDetailKPI">
                                                        <asp:Label ID="lblKpiGuidedMovementDesc" runat="server" Text="0/0"> </asp:Label>   
                                                    </div>
                                                    <span class="count_bottom">
                                                        Tareas
                                                    </span>
                                                     <div>
                                                        <asp:Button ID="btnGuidedMovementData" runat="server" Text="Datos" OnClick="btnGuidedMovementData_Click" />
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </ContentTemplate>
                                    <Triggers>                                    
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$ctl00$btnUpdchartPanelControl" EventName="Click" /> 
                                        <asp:AsyncPostBackTrigger ControlID="timerRefreshControlPanel" EventName="Tick" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <asp:UpdateProgress ID="udrpInfoKpi" runat="server" AssociatedUpdatePanelID="udpInfoKpi" DisplayAfter="20" DynamicLayout="true">
                                    <ProgressTemplate>
                                        <div class="divProgress">
                                            <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <webUc:UpdateProgressOverlayExtender ID="mudrpInfoKpi" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="udrpInfoKpi" />
                            </div> 
                        </div>
                        <div id="kpiZone" class="tab-pane fade">
                            <div class="row">
                                <div class="col-md-2">
                                    <button id="updchartKpiZone" class="btn btn-default">Actualizar</button> 
                                </div>
                                <div class="col-md-5">
                                    <label>Unid. de Medida</label>
                                    <asp:DropDownList ID="ddlKpiZoneUnit" runat="server" SkinID="ddlFilter"></asp:DropDownList>
                                </div>
                                <div class="col-md-5">
                                    <label>Zona</label>
                                    <asp:DropDownList ID="ddlKpiZone" runat="server" SkinID="ddlFilter"></asp:DropDownList>
                                </div>
                            </div>

                            <canvas id="chartKpiZone"></canvas>
                        </div>
                        <div id="kpiPicking" class="tab-pane fade">
                            <div class="row">
                                <div class="col-md-2">
                                    <button id="updchartKpiPicking" class="btn btn-default">Actualizar</button> 
                                </div>
                                <div class="col-md-5">
                                    <label>Tipo</label>
                                    <asp:DropDownList ID="ddlKpiPickingUnid" runat="server" SkinID="ddlFilter"></asp:DropDownList>
                                </div>
                                <div class="col-md-5">
                                    <label>Usuario</label>
                                    <asp:DropDownList ID="ddlUserCharts4" runat="server" SkinID="ddlFilter"></asp:DropDownList>
                                </div>
                            </div>

                            <canvas id="chartKpiPicking"></canvas>
                        </div>
                        <div id="kpiFillRate" class="tab-pane fade">
                            <div class="row">
                                <div class="col-md-2">
                                    <button id="updchartKpiFillRate" class="btn btn-default">Actualizar</button> 
                                </div>
                                <div class="col-md-10">
                                    <label>Cliente</label>
                                    <asp:DropDownList ID="ddlCustomerCharts5" runat="server" SkinID="ddlFilter"></asp:DropDownList>
                                </div>
                            </div>
                            
                            <canvas id="chartKpiFillRate"></canvas>
                        </div>
                        <div id="kpiChartKpiLeadTime" class="tab-pane fade">
                            <div class="row">
                                <div class="col-md-2">
                                    <button id="updChartKpiLeadTime" class="btn btn-default">Actualizar</button> 
                                </div>
                                <div class="col-md-5">
                                    <label>Tipo Cliente</label>
                                    <asp:DropDownList ID="ddlLeadTimeCustomer" runat="server" SkinID="ddlFilter"></asp:DropDownList>          
                                </div>
                                <div class="col-md-5">
                                    <label>Tipo Delta</label>
                                    <asp:DropDownList ID="ddlLeadTime" runat="server" SkinID="ddlFilter">
                                        <asp:ListItem Text="Horas" Value="H"></asp:ListItem>
                                        <asp:ListItem Text="Dias" Value="D"></asp:ListItem>                                    
                                    </asp:DropDownList>
                                </div>
                            </div>
                            
                            <canvas id="chartKpiLeadTime"></canvas>
                        </div>
                    </div>
                </div>
            </div>

        </div>
        <!-- fin panel -->

        <asp:UpdatePanel ID="upKpiOrders" runat="server" UpdateMode="Conditional" >
             <ContentTemplate>
                 <div id="divKpiOrders" runat="server" visible="false">
                     <asp:Button ID="btnDummyKpiOrders" runat="Server" Style="display: none" />

                     <ajaxToolKit:ModalPopupExtender 
	                    ID="mpKpiOrders" runat="server" TargetControlID="btnDummyKpiOrders" 
	                    PopupControlID="pnlKpiOrders"  
	                    BackgroundCssClass="modalBackground" 
	                    PopupDragHandleControlID="panelCaptionKpiOrders" Drag="true" >
	                </ajaxToolKit:ModalPopupExtender>

                     <asp:Panel ID="pnlKpiOrders" runat="server" CssClass="modalBox">
                         <asp:Panel ID="panelCaptionKpiOrders" runat="server" CssClass="modalHeader">
                            <div class="divCaption">
                                <asp:Label ID="lblTitleKpiOrders" runat="server" Text="Pedidos" />
                                <asp:ImageButton ID="ImageButtonKpiOrders" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                            </div>
                        </asp:Panel>

                         <div id="divCtrsKpiOrders" class="modalControls">
                             <div class="modalBoxContent" >  
                                 <div>
                                      <asp:ImageButton ID="btnExcelGridOrders" runat="server" onclick="btnExcelGridOrders_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_export_excel.png" CausesValidation="false" ValidationGroup="EditNew" ToolTip="Exportar a Excel" />
                                 </div>
                                 <div>
                                     <asp:Label ID="lblKpiOrdersCount" runat="server" Text="Total:" />
                                 </div>

                                 <div id="divPageGrdMgr" runat="server">
                                    <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                        <asp:ImageButton ID="btnFirstgrdMgr" runat="server" OnClick="btnFirstgrdMgr_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                        <asp:ImageButton ID="btnPrevgrdMgr" runat="server" OnClick="btnPrevgrdMgr_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_previous_dis.png" />
                                        Pág. 
                                        <asp:DropDownList ID="ddlPages" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSelectedIndexChanged" SkinID="ddlFilter" /> 
                                        de 
                                        <asp:Label ID="lblPageCount" runat="server" Text="" />
                                        <asp:ImageButton ID="btnNextgrdMgr" runat="server" OnClick="btnNextgrdMgr_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                        <asp:ImageButton ID="btnLastgrdMgr" runat="server" OnClick="btnLastgrdMgr_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                                    </div>
                                </div>

                                 <div class="container">
                                     <asp:GridView ID="grdMgr" runat="server" 
                                        OnRowCreated="grdMgr_RowCreated"
                                        AllowPaging="true" 
                                        EnableViewState="False"
                                        AutoGenerateColumns="false"
                                        OnRowDataBound="grdMgr_RowDataBound"
                                        CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                        EnableTheming="false"
                                        ShowFooter="false">
                                        <Columns>

                                            <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode">
                                                <itemtemplate>
                                                    <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                </itemtemplate>
                                            </asp:templatefield>
                                                                 
                                            <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                                <itemtemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                    </div>                                                        
                                                </itemtemplate>
                                            </asp:templatefield>

                                            <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'/>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblOwnerTradeName" runat="server" text='<%# Eval ( "Owner.Name" ) %>'/>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField HeaderText="N° Doc." ItemStyle-Wrap="false" DataField="Number" AccessibleHeaderText="OutboundNumber" > </asp:BoundField>
                                               
                                            <asp:TemplateField AccessibleHeaderText="OutboundTypeCode" HeaderText="Tipo">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblOutboundType" runat="server" text='<%# Eval ( "OutboundType.Code" ) %>' />
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

                                        </Columns>
                                    </asp:GridView>
                                 </div>
                             </div>
                        </div>

                     </asp:Panel>

                 </div>
             </ContentTemplate>
             <Triggers>
                 <asp:PostBackTrigger ControlID="btnExcelGridOrders" />      
             </Triggers>
        </asp:UpdatePanel>

        <asp:UpdateProgress ID="uprKpiOrders" runat="server" AssociatedUpdatePanelID="upKpiOrders">
            <ProgressTemplate>
                <div class="divProgress">
                    <asp:ImageButton ID="imgProgressKpiOrders" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <webUc:UpdateProgressOverlayExtender ID="muprKpiOrders" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprKpiOrders" />


        <asp:UpdatePanel ID="upKpiTasks" runat="server" UpdateMode="Conditional" >
             <ContentTemplate>
                 <div id="divKpiTasks" runat="server" visible="false">
                     <asp:Button ID="btnDummyKpiTasks" runat="Server" Style="display: none" />

                     <ajaxToolKit:ModalPopupExtender 
	                    ID="mpKpiTasks" runat="server" TargetControlID="btnDummyKpiTasks" 
	                    PopupControlID="pnlKpiTasks"  
	                    BackgroundCssClass="modalBackground" 
	                    PopupDragHandleControlID="panelCaptionKpiTasks" Drag="true" >
	                </ajaxToolKit:ModalPopupExtender>

                     <asp:Panel ID="pnlKpiTasks" runat="server" CssClass="modalBox">
                         <asp:Panel ID="panelCaptionKpiTasks" runat="server" CssClass="modalHeader">
                            <div class="divCaption">
                                <asp:Label ID="lblTitleKpiTasks" runat="server" Text="Tareas" />
                                <asp:ImageButton ID="ImageButtonKpiTasks" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                            </div>
                        </asp:Panel>

                         <div id="divCtrsKpiTasks" class="modalControls">
                             <div class="modalBoxContent" >  
                                 <div>
                                      <asp:ImageButton ID="btnExcelGridTasks" runat="server" onclick="btnExcelGridTasks_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_export_excel.png" CausesValidation="false" ValidationGroup="EditNew" ToolTip="Exportar a Excel" />
                                 </div>
                                 <div>
                                     <asp:Label ID="lblKpiTasksCount" runat="server" Text="Total:" />
                                 </div>

                                 <div id="divPageGrdMgrTasks" runat="server">
                                    <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                                        <asp:ImageButton ID="btnFirstgrdMgrTasks" runat="server" OnClick="btnFirstgrdMgrTasks_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                                        <asp:ImageButton ID="btnPrevgrdMgrTasks" runat="server" OnClick="btnPrevgrdMgrTasks_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_previous_dis.png" />
                                        Pág. 
                                        <asp:DropDownList ID="ddlPagesTasks" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesTasksSelectedIndexChanged" SkinID="ddlFilter" /> 
                                        de 
                                        <asp:Label ID="lblPageTasksCount" runat="server" Text="" />
                                        <asp:ImageButton ID="btnNextgrdMgrTasks" runat="server" OnClick="btnNextgrdMgrTasks_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                                        <asp:ImageButton ID="btnLastgrdMgrTasks" runat="server" OnClick="btnLastgrdMgrTasks_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                                    </div>
                                </div>

                                 <div class="container">
                                     <asp:GridView ID="grdMgrTasks" runat="server" 
                                        OnRowCreated="grdMgrTasks_RowCreated"
                                        AllowPaging="true" 
                                        EnableViewState="False"
                                        AutoGenerateColumns="false"
                                        OnRowDataBound="grdMgrTasks_RowDataBound"
                                        CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                        EnableTheming="false"
                                        ShowFooter="false">
                                        <Columns>

                                            <asp:TemplateField HeaderText="Id Tarea" AccessibleHeaderText="TaskId" SortExpression="Task">
                                                <ItemTemplate>
                                                    <center>
                                                        <asp:Label ID="lblTaskId" runat="server" Text='<%# Eval ( "Task.Id" ) %>'></asp:Label>
                                                    </center>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                        
                                            <asp:TemplateField HeaderText="Cód. Centro" AccessibleHeaderText="CodeWhs" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Task.Warehouse.Code" ) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="WhsName">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Task.Warehouse.ShortName" ) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="CodeOwn" ItemStyle-CssClass="text">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Task.Owner.Code" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ( "Task.Owner.Name" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="NameTrackTaskType" SortExpression="NameTrackTaskType">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNameTrackTaskType" runat="server" Text='<%# Eval ( "TrackTaskType.Name" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Creada" AccessibleHeaderText="CreateDate" SortExpression="CreateDate">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreateDate" runat="server" Text='<%# Eval ( "Task.CreateDate" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Codigo Tarea" AccessibleHeaderText="TaskTypeCode" SortExpression="TaskType">
                                                <ItemTemplate>
                                                    <center>
                                                        <asp:Label ID="lblTaskType" runat="server" Text='<%# Eval ( "Task.TypeCode" ) %>'></asp:Label>
                                                    </center>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Tarea" AccessibleHeaderText="TaskTypeName" SortExpression="TaskType">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTaskTypeName" runat="server" Text='<%# Eval ( "TaskTypeName" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Ejecutada" AccessibleHeaderText="IsCompleteDetail" SortExpression="IsCompleteDetail">
                                                <ItemTemplate>
                                                    <center>
                                                        <asp:CheckBox ID="chkTaskIsComplete" runat="server" checked='<%# Eval ( "Task.IsComplete" ) %>' Enabled="false"/>
                                                    <center>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Prioridad" AccessibleHeaderText="Priority" SortExpression="Priority">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPriority" runat="server" Text='<%# Eval ( "Task.Priority" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                 </div>
                             </div>
                        </div>

                     </asp:Panel>

                 </div>
             </ContentTemplate>
             <Triggers>
                 <asp:PostBackTrigger ControlID="btnExcelGridTasks" />      
             </Triggers>
        </asp:UpdatePanel>

        <asp:UpdateProgress ID="uprKpiTasks" runat="server" AssociatedUpdatePanelID="upKpiTasks">
            <ProgressTemplate>
                <div class="divProgress">
                    <asp:ImageButton ID="imgProgressKpiTasks" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <webUc:UpdateProgressOverlayExtender ID="muprKpiTasks" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprKpiTasks" />

</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
<%--    <webUc:ucStatus ID="ucStatus" runat="server" />--%>
</asp:Content>


