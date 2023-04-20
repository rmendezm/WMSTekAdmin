<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="OutboundDashboard.aspx.cs" 
Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.OutboundDashboard" Title="Untitled Page" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script src="../../WebResources/Javascript/detect-element-resize.js" type="text/javascript"></script>

<script type="text/javascript" language='Javascript'>

    //Realiza un resize para el div de contiene los graficos
    function resizeDivCharts() {
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divGroupCharts").style.height = h;
        document.getElementById("ctl00_MainContent_divGroupCharts").style.width = w;
        ChangeColorKpi();
        //Init();
    }

    
    window.onresize = resizeDivCharts;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(resizeDivCharts);


    var par = false;
    function ChangeColorKpi() {
        //newColor = '#666';
        colorRojo = '#FF3300';
        colorAmarillo = '#FFCC00';
        colorVerde = '#009900';
        //document.getElementById('lblKpiInfoTotal').style.color = newColor;

        var lblKpiReleased = document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiReleased').innerHTML;
        if (lblKpiReleased < 50) {
            newColor = colorRojo;
        } else if (lblKpiReleased >= 50 && lblKpiReleased <= 75) {
            newColor = colorAmarillo;
        } else if (lblKpiReleased > 75) {
            newColor = colorVerde;
        }
        document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiReleased').style.color = newColor;
        document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiReleasedPorcent').style.color = newColor;


        var lblKpiPicking = document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiPicking').innerHTML;
        if (lblKpiPicking < 50) {
            newColor = colorRojo;
        } else if (lblKpiPicking >= 50 && lblKpiPicking <= 75) {
            newColor = colorAmarillo;
        } else if (lblKpiPicking > 75) {
            newColor = colorVerde;
        }
        document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiPicking').style.color = newColor;
        document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiPickingPorcent').style.color = newColor;


        var lblKpiAnden = document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiAnden').innerHTML;
        if (lblKpiAnden < 50) {
            newColor = colorRojo;
        } else if (lblKpiAnden >= 50 && lblKpiAnden <= 75) {
            newColor = colorAmarillo;
        } else if (lblKpiAnden > 75) {
            newColor = colorVerde;
        }
        document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiAnden').style.color = newColor;
        document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiAndenPorcent').style.color = newColor;


        var lblKpiDispatch = document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiDispatch').innerHTML;
        if (lblKpiDispatch < 50) {
            newColor = colorRojo;
        } else if (lblKpiDispatch >= 50 && lblKpiDispatch <= 75) {
            newColor = colorAmarillo;
        } else if (lblKpiDispatch > 75) {
            newColor = colorVerde;
        }
        document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiDispatch').style.color = newColor;
        document.getElementById('ctl00_MainContent_hsMasterDetail_leftPanel_ctl01_lblKpiDispatchPorcent').style.color = newColor;
    }

    window.onload = ChangeColorKpi;

    /*
    document.getElementById("divGroupCharts")[0].onresize = function() { myFunction() };
    function myFunction() {
        document.getElementById("ChartKpiZona").style.width = '500px';
    }
    */


//    var resizeElement = document.getElementById('Splitter1_LeftP_Content'),
//      resizeCallback = function() {
//          alert('adadad');
//      };
//      addResizeListener(resizeElement, resizeCallback);


      //window.o.onload = Init();

      function Init() {
          var splitter1LD = document.getElementById('__Splitter1LD');
          splitter1LD.attachEvent("onresize", resizeChart1);

          var splitter1RD = document.getElementById('__Splitter1RD');
          splitter1RD.attachEvent("onresize", resizeChart2);
      }

      function resizeChart1() {
          var width = document.getElementById('__Splitter1LD').clientWidth;
          var heigth = document.getElementById('__Splitter1LD').clientHeight;
          
          document.getElementById("div_KpiZona").style.width = (width - 50) + 'px';
          document.getElementById("ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl00_ctl01_Splitter1_leftPanel1_ctl01_divKpiZonaChart").style.width = (width -40) + 'px';
          document.getElementById("ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl00_ctl01_Splitter1_leftPanel1_ctl01_ChartKpiZona").style.width = (width - 80)  + 'px';
      }

      function resizeChart2() {
          var width = document.getElementById('__Splitter1RD').clientWidth;
          var heigth = document.getElementById('__Splitter1RD').clientHeight;

          document.getElementById("div_KpiPicking").style.width = (width - 40) + 'px';
          document.getElementById("ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl00_ctl01_Splitter1_rightPanel1_ctl01_divKpiPickingChart").style.width = (width - 40) + 'px';
          document.getElementById("ctl00_MainContent_hsMasterDetail_ctl00_ctl01_HorizontalSplitter1_ctl00_ctl01_Splitter1_rightPanel1_ctl01_ChartKpiPicking").style.width = (width - 80) + 'px';
      }
    
</script>


<style  type="text/css">
    .rfKPIValue {
        text-shadow: 1px 1px 1px 0 #e8e8e8;
        color: #2967a6;
        font-size: 20px;
        cursor: pointer;
    }
    
    .rfKPICaption {
        color: #666;
        display: inline-block;
        font-size: 12px;
        cursor: pointer;
    }
    
    .lblKpiText
    {
    	text-shadow: 2px 2px 2px 2px #e8e8e8;
        color: #666;
        font-size: 9px;
    }            
</style>   

<asp:Timer ID="TimerKpiLeadTime" runat="server" Interval="10000" OnTick="TimerKpiLeadTime_Tick" Enabled="false" ></asp:Timer>
<asp:Timer ID="TimerKpiZona" runat="server" Interval="10000" OnTick="TimerKpiZona_Tick" Enabled="false" ></asp:Timer>
<asp:Timer ID="TimerKpiPicking" runat="server" Interval="10000" OnTick="TimerKpiPicking_Tick" Enabled="false" ></asp:Timer>            
<asp:Timer ID="TimerKpiFillRate" runat="server" Interval="10000" OnTick="TimerKpiFillRate_Tick" Enabled="false" ></asp:Timer>
<%--<asp:Timer ID="TimerTareas" runat="server" Interval="10000" OnTick="TimerTareas_Tick" Enabled="false" ></asp:Timer>--%>
<asp:Timer ID="TimerInfoKpi" runat="server" Interval="10000" OnTick="TimerInfoKpi_Tick" Enabled="false" ></asp:Timer>

<div id="divGroupCharts" runat="server" style="margin:0px;margin-bottom:80px">   

    <spl:Splitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default">
        <LeftPanel ID="leftPanel" WidthDefault="300" WidthMin="100">
            <Header Height="70">
                <div >	
                    <table >
                        <tr>                            
                            <td colspan="2" style=" text-align: right; vertical-align: middle;position: relative; bottom: 5px;">
                                <br />
                                <asp:label runat="server" id="Label10" Text="Panel de Control" ></asp:label>
                            </td>                           
                        </tr>
                        <tr>
                            <td  style="text-align:left; width:20px; vertical-align: middle;position: relative; bottom: 3px;">
                                <asp:ImageButton ID="btnRefreshInfoKpi" ToolTip="Actualizar" runat="server"  OnClick="btnRefreshInfoKpi_Click"
                                Height="20px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_refresh.png" />                                                   
                            </td>
                            <td  style=" text-align:left; vertical-align: middle;position: relative; bottom: 3px;" >                                             
                                <asp:CheckBox ID="chkAutoRefreshIndoKpi"  runat="server" style="vertical-align: middle;" ToolTip="Actualiza de forma autom&aacute;tica"
                                 AutoPostBack="true" OnCheckedChanged="chkAutoRefreshIndoKpi_CheckedChanged"/>    
                                 <asp:Label ID="Label1" runat="server" style=" font-size:10px; vertical-align: middle;" ToolTip="Actualiza de forma autom&aacute;tica">auto</asp:Label>                                          
                            </td>
                        </tr>
                    </table>		
                </div>
            </Header>
            <Content>
                <div id="divInfoKpi" class="dialog_Chart" style="width:100%; height:100%">
                    <asp:UpdatePanel ID="udpInfoKpi" runat="server" UpdateMode="Conditional"  >
                    <ContentTemplate>  
                        <div id="divInfoKpiChart" runat="server">
                        <table class="table_window_Chart">                         
                                <tr>                                                   
                                    <td class="bluelighting_content_Chart"  style="text-align:right">
                                        <table>
                                            <tr>
                                                 <td colspan="2" style=" border: 1px dotted #333;">                                                    
                                                    <div id="divKpiInfoTotal"  style="width: 100%; height: 80px; text-align:center; vertical-align:middle;" runat="server">
                                                        <div class="rfMiniKPIBorderContainer" style="height:5px;"></div> 
                                                        <div  class="rfKPICaption" style="text-align:center; width: 280px;  ">
                                                            <asp:Label ID="InfoKpiInfoTotal" runat="server" Text="Info. Total"> </asp:Label>
                                                        </div>
                                                        <div class="rfKPIValue" style=" text-align:center; width: 280px; ">
                                                            <asp:Label ID="lblKpiInfoTotal" runat="server" Text="0"> </asp:Label>
                                                        </div>
                                                        <div></div>
                                                        <div class="rfKPIValue" style="text-align:center; width: 280px; ">
                                                            Pedidos
                                                        </div>
                                                        <div class="rfMiniKPIBorderContainer" style="height:5px;"></div> 
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                                                               
                                                <td style="border: 1px dotted #333; text-align:center;">
                                                    
                                                    <div id="divKpiReleased" class="rfMiniKPICore" style="width: 100%; height: 102px; text-align:center;" runat="server">
                                                        <div class="rfMiniKPIBorderContainer" style="height:5px;"></div>  
                                                        <div class="rfKPICaption" style="width: 140px; text-align:center;">
                                                            <asp:Label ID="InfoKpiReleased" runat="server" Text="Liberados"> </asp:Label>
                                                        </div>
                                                        <div class="rfKPIValue" style="width: 140px; text-align:center;">
                                                            <asp:Label ID="lblKpiReleased" runat="server" Text="0"> </asp:Label>
                                                            <asp:Label ID="lblKpiReleasedPorcent" runat="server" Text=" %"> </asp:Label>
                                                        </div>
                                                        <div class="rfKPICaption" style="width: 140px; text-align:center;">
                                                            <asp:Label ID="lblKpiReleasedDesc" runat="server" Text="0/0"> </asp:Label>
                                                        </div>
                                                         <div class="rfKPIValue" style="width: 140px; text-align:center;">
                                                            Pedidos
                                                        </div>
                                                         <div class="rfMiniKPIBorderContainer" style="height:5px;"></div> 
                                                    </div>
                                                </td>
                                                
                                                <td style="border: 1px dotted #333; text-align:center;">
                                                    
                                                    <div id="divKpiPicking" class="rfMiniKPICore" style="width: 100%; height: 102px; text-align:center;" runat="server">
                                                       <div class="rfMiniKPIBorderContainer" style="height:5px;"></div> 
                                                        <div class="rfKPICaption" style="width: 140px; text-align:center;">
                                                            <asp:Label ID="InfoKpiPicking" runat="server" Text="Picking"> </asp:Label>
                                                        </div>
                                                        <div class="rfKPIValue" style="width: 140px; text-align:center;">
                                                            <asp:Label ID="lblKpiPicking" runat="server" Text="0"> </asp:Label>
                                                            <asp:Label ID="lblKpiPickingPorcent" runat="server" Text=" %"> </asp:Label>
                                                        </div>
                                                        <div class="rfKPICaption" style="width: 140px; text-align:center;">
                                                            <asp:Label ID="lblKpiPickingDesc" runat="server" Text="0/0"> </asp:Label>
                                                        </div>
                                                         <div class="rfKPIValue" style="width: 140px; text-align:center;">
                                                            Tareas
                                                        </div>
                                                        <div class="rfMiniKPIBorderContainer" style="height:5px;"></div> 
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" style=" border: 1px dotted #333; text-align:center;">
                                                    
                                                    <div id="divKpiPacking" class="rfMiniKPICore" style="width: 100%; height: 80px; text-align:center;" runat="server">
                                                        <div class="rfMiniKPIBorderContainer" style="height: 5px;"></div>   
                                                        <div class="rfKPICaption" style="width: 280px;  text-align:center;">
                                                            Packing
                                                        </div>
                                                        <div class="rfKPIValue" style="width: 280px; text-align:center;">
                                                            <asp:Label ID="lblKpiPacking" runat="server" Text="0"> </asp:Label>
                                                        </div>
                                                        <div class="rfKPIValue" style="width: 280px; text-align:center;">
                                                            Tareas
                                                        </div>
                                                        <div class="rfMiniKPIBorderContainer" style="height: 5px;"></div> 
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                
                                                
                                                <td style="border: 1px dotted #333;  text-align:center;" >
                                                    
                                                    <div id="divKpiAnden" class="rfMiniKPICore" style="width: 100%; height: 100px; text-align:center;" runat="server">
                                                        <div class="rfMiniKPIBorderContainer" style="height: 5px;"></div>   
                                                        <div class="rfKPICaption" style="width: 140px; text-align:center;">
                                                            Anden
                                                        </div>
                                                        <div class="rfKPIValue" style="width: 140px; text-align:center;">
                                                            <asp:Label ID="lblKpiAnden" runat="server" Text="0"> </asp:Label> 
                                                            <asp:Label ID="lblKpiAndenPorcent" runat="server" Text=" %"> </asp:Label>
                                                        </div>
                                                        <div class="rfKPICaption" style="width: 140px; text-align:center;">
                                                            <asp:Label ID="lblKpiAndenDesc" runat="server" Text="0/0"> </asp:Label>
                                                        </div>
                                                        <div class="rfKPIValue" style="width: 140px; text-align:center;">
                                                            Tareas
                                                        </div>
                                                        <div class="rfMiniKPIBorderContainer" style="height: 5px;"></div> 
                                                    </div>
                                                </td>
                                                <td style="border: 1px dotted #333;  text-align:center;">
                                                    
                                                    <div id="divKpiDispatch" class="rfMiniKPICore" style="width: 100%; height: 100px; text-align:center;" runat="server">
                                                        <div class="rfMiniKPIBorderContainer" style="height: 5px;"></div>   
                                                        <div class="rfKPICaption" style="width: 140px; text-align:center;">
                                                            Despachos
                                                        </div>
                                                        <div class="rfKPIValue" style="width: 140px; text-align:center;">
                                                            <asp:Label ID="lblKpiDispatch" runat="server" Text="0"> </asp:Label>
                                                            <asp:Label ID="lblKpiDispatchPorcent" runat="server" Text=" %"> </asp:Label>
                                                        </div>
                                                        <div class="rfKPICaption" style="width: 140px; text-align:center;">
                                                            <asp:Label ID="lblKpiDispatchDesc" runat="server" Text="0/0"> </asp:Label>
                                                        </div>
                                                        <div class="rfKPIValue" style="width: 140px; text-align:center;">
                                                            Pedidos
                                                        </div>
                                                        <div class="rfMiniKPIBorderContainer" style="height: 5px;"></div> 
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                   
                                </tr>
                           
                        </table>
                        </div>
                    </ContentTemplate>
                    <Triggers>                                    
                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$leftPanel$ctl00$btnRefreshInfoKpi" EventName="Click" /> 
                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$leftPanel$ctl00$chkAutoRefreshIndoKpi" EventName="CheckedChanged" /> 
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="TimerInfoKpi" EventName="Tick" />    
                    </Triggers>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="uprInfoKpi" AssociatedUpdatePanelID="udpInfoKpi" runat="server"  DynamicLayout="true" DisplayAfter="1">
                        <ProgressTemplate>                        
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />        
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>                                                                                                            
                    <webUc:UpdateProgressOverlayExtender ID="udproeInfoKpi" runat="server" 
                    ControlToOverlayID="divInfoKpiChart" CssClass="updateProgress" TargetControlID="uprInfoKpi" />
                </div>
            </Content>
            <Footer Height="67">
                <div style="color: White">
                    No Borrar - Evita que BottomPanel se solape con StatusBar</div>
            </Footer>
        </LeftPanel>
        <RightPanel>
            <Content>
            
                <spl:HorizontalSplitter ID="HorizontalSplitter1" CookieDays="0" runat="server" StyleFolder="../WebResources/styles/default">
                     <TopPanel HeightMin="0" HeightMax="500">
                        <Content>
                        
                            <spl:Splitter ID="Splitter1" LiveResize="false" CookieDays="0"  runat="server" StyleFolder="~/WebResources/Styles/Obout/default">
                                <LeftPanel ID="leftPanel1"  WidthMin="100">
                                    <Header Height="20">
                                        <div class="bluelighting_title_Chart">
                                            <asp:label runat="server" id="lblTitleKpiZona" Text="Ocupaci&oacute;n Por Zona"></asp:label>
                                        </div>
                                    </Header>
                                    <Content>
                                    
                                        <div id="div_KpiZona" class="dialog_Chart" style=" float:left">
                                            
                                            <table class="table_window_Chart">                                           
                                                <tr>                                                                          
                                                    <td class="bluelighting_content_Chart" style="text-align:right">
                                                                                                
                                                            <asp:UpdatePanel ID="udpKpiZone" runat="server" UpdateMode="Conditional"  >
                                                            <ContentTemplate>    
                                                                <div id="divKpiZonaChart" runat="server">
                                                                <asp:Chart ID="ChartKpiZona" Width="340px"  runat="server" OnClick="ChartKpiZona_Click"  >
                                                                    <Titles>
                                                                        <asp:Title Name="Title1" >
                                                                      </asp:Title>
                                                                    </Titles>
                                                                    <Series > 
                                                                      <asp:Series  Name="Series1" PostBackValue="#VAL" >                                                
                                                                      </asp:Series> 
                                                                    </Series> 
                                                                    <ChartAreas>
                                                                         <asp:ChartArea  Name="ChartArea1" BackGradientStyle="TopBottom" 
                                                                         BackSecondaryColor="#B6D6EC" BorderDashStyle="Solid" BorderWidth="1">
                                                                            <AxisX>
                                                                                <MajorGrid Enabled="False" />
                                                                            </AxisX>
                                                                        </asp:ChartArea>
                                                                    </ChartAreas> 
                                                                    <Legends>
                                                                       
                                                                    </Legends>
                                                                </asp:Chart> 
                                                                
                                                                <div class="divControls">
                                                                    <div class="fieldRight">                                                                        
                                                                        <asp:Label CssClass="lblKpiText" ID="lblKpiZone" runat="server" Text="Unid. de Medida"/>
                                                                    </div>
                                                                    <div class="fieldLeft">
                                                                        <asp:DropDownList ID="ddlKpiZoneUnit" runat="server" AutoPostBack="true" SkinID="ddlFilter"
                                                                                OnSelectedIndexChanged="ddlKpiZone_SelectedIndexChanged">                                        
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                    
                                                                    <div style=" width:50px" class="fieldRight">  
                                                                        <asp:Label CssClass="lblKpiText" ID="Label4" runat="server" Text="Zona"/>
                                                                    </div>
                                                                    <div class="fieldLeft">
                                                                        <asp:DropDownList ID="ddlKpiZone" runat="server" AutoPostBack="true" SkinID="ddlFilter"
                                                                                OnSelectedIndexChanged="ddlKpiZone_SelectedIndexChanged">                                        
                                                                        </asp:DropDownList>
                                                                    </div>   
                                                                                                 
                                                                   <asp:Label CssClass="lblKpiText" ID="Label3" runat="server" Text="Tipo Gr&aacute;fico" Visible="false"/>&nbsp;
                                                                    <asp:DropDownList ID="ddlTypeKpiZone" runat="server" AutoPostBack="true" SkinID="ddlFilter"
                                                                            OnSelectedIndexChanged="ddlTypeKpiZone_SelectedIndexChanged" Visible="false">                                        
                                                                    </asp:DropDownList>
                                                                   
                                                                </div>
                                                                </div>
                                                            </ContentTemplate>
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="ddlKpiZone" EventName="SelectedIndexChanged" />
                                                                <asp:AsyncPostBackTrigger ControlID="ddlKpiZoneUnit" EventName="SelectedIndexChanged" />
                                                                <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                                                <asp:AsyncPostBackTrigger ControlID="ddlTypeKpiZone" EventName="SelectedIndexChanged" /> 
                                                                <asp:AsyncPostBackTrigger ControlID="TimerKpiZona" EventName="Tick" />
                                                            </Triggers>
                                                        </asp:UpdatePanel>
                                                        
                                                       <asp:UpdateProgress ID="udprChart2" AssociatedUpdatePanelID="udpKpiZone" runat="server"  DynamicLayout="true" DisplayAfter="1">
                                                            <ProgressTemplate>                        
                                                                <div class="divProgress">
                                                                    <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />        
                                                                </div>
                                                            </ProgressTemplate>
                                                        </asp:UpdateProgress>                                                                                                            
                                                        <webUc:UpdateProgressOverlayExtender ID="udproeKpiZone" runat="server" 
                                                        ControlToOverlayID="divKpiZonaChart" CssClass="updateProgress" TargetControlID="udprChart2" />
                                                    </td>                                                
                                                </tr>                                             
                                            </table>                                            
                                        </div>                                     
                                    </Content>
                                    
                                </LeftPanel>
                                <RightPanel ID="rightPanel1">
                                    <Header Height="20">
                                        <div class="bluelighting_title_Chart">
                                            <asp:label ID="lblTitelKpiPicking" runat="server" Text="Productividad de Picking"></asp:label>
                                        </div>
                                    </Header>
                                    <Content>
                            
                                        <div id="div_KpiPicking" class="dialog_Chart" style="width:380px; float:left; ">
                                            <table class="table_window_Chart">                                           
                                                <tr>                                              
                                                    <td class="bluelighting_content_Chart" style="text-align:right">
                                                        <asp:UpdatePanel ID="udpChart4" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>   
                                                            <div id="divKpiPickingChart" runat="server">
                                                            <asp:Chart ID="ChartKpiPicking" Width="340px" runat="server" OnClick="ChartKpiPicking_Click">
                                                                <Titles>
                                                                    <asp:Title Name="Title1">
                                                                  </asp:Title>
                                                                </Titles>
                                                                <Series> 
                                                                  <asp:Series Name="Series1" PostBackValue="#" />                               
                                                                 </Series>    
                                                                  
                                                                <ChartAreas>
                                                                     <asp:ChartArea Name="ChartArea1" BackGradientStyle= "VerticalCenter" 
                                                                     BackSecondaryColor="#B6D6EC" BorderDashStyle= "Solid" BorderWidth="1">
                                                                        <AxisX>
                                                                            <MajorGrid Enabled="False" />
                                                                        </AxisX>
                                                                    </asp:ChartArea>
                                                                </ChartAreas> 
                                                            </asp:Chart>
                                                            
                                                                <div class="divControls">
                                                                    <div style="width:50px;" class="fieldRight">
                                                                        <asp:Label CssClass="lblKpiText" ID="Label9" runat="server" Text="Tipo"/>
                                                                    </div>
                                                                    <div class="fieldLeft">
                                                                        <asp:DropDownList ID="ddlKpiPickingUnid" runat="server" AutoPostBack="true" SkinID="ddlFilter"
                                                                            OnSelectedIndexChanged="ddlUserCharts4_SelectedIndexChanged">
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                    <div style="width:50px;" class="fieldRight">
                                                                        <asp:Label CssClass="lblKpiText" ID="Label2" runat="server" Text="Usuario"/>
                                                                    </div>
                                                                    <div style="width:120px;" class="fieldLeft">
                                                                        <asp:DropDownList ID="ddlUserCharts4" runat="server" AutoPostBack="true" SkinID="ddlFilter"
                                                                            OnSelectedIndexChanged="ddlUserCharts4_SelectedIndexChanged">
                                                                        </asp:DropDownList>
                                                                    </div> 
                                                            
                                                                    <asp:Label CssClass="lblKpiText" ID="Label5" runat="server" Text="Tipo Gr&aacute;fico" Visible="false"/>&nbsp;
                                                                    <asp:DropDownList ID="ddlTypeKpiPicking" runat="server" AutoPostBack="true" SkinID="ddlFilter" Visible="false"
                                                                            OnSelectedIndexChanged="ddlTypeKpiPicking_SelectedIndexChanged">                                        
                                                                    </asp:DropDownList>
                                                                </div>  
                                                            </div>
                                                        </ContentTemplate>
                                                        <Triggers>
                                                            <asp:AsyncPostBackTrigger ControlID="ddlUserCharts4" EventName="SelectedIndexChanged" />
                                                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                                            <asp:AsyncPostBackTrigger ControlID="TimerKpiPicking" EventName="Tick" />                                                                    
                                                            <asp:AsyncPostBackTrigger ControlID="ddlTypeKpiPicking" EventName="SelectedIndexChanged" /> 
                                                            <asp:AsyncPostBackTrigger ControlID="ddlKpiPickingUnid" EventName="SelectedIndexChanged" /> 
                                                            
                                                        </Triggers>
                                                        </asp:UpdatePanel>
                                                    
                                                        <asp:UpdateProgress ID="udprChart4" AssociatedUpdatePanelID="udpChart4" runat="server" DisplayAfter="1" DynamicLayout="true">
                                                            <ProgressTemplate>                        
                                                                <div class="divProgress">
                                                                    <asp:ImageButton ID="imgProgress4" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
                                                                </div>
                                                            </ProgressTemplate>
                                                        </asp:UpdateProgress>   
                                                        <webUc:UpdateProgressOverlayExtender ID="udproeKpiPicking" runat="server" 
                                                        ControlToOverlayID="divKpiPickingChart" CssClass="updateProgress" TargetControlID="udprChart4" />                                                         
                                                                                  
                                                    </td>                                             
                                                </tr>                                                  
                                            </table>                                               
                                        </div>                                    
                                    </Content>
                                   
                                </RightPanel>
                            </spl:Splitter>
                        
                        </Content>
                    </TopPanel>
                    <BottomPanel>
                        <Content>
                        
                            <spl:Splitter ID="Splitter2" LiveResize="false" CookieDays="0"  runat="server" StyleFolder="~/WebResources/Styles/Obout/default">
                                <LeftPanel ID="leftPanel2"  WidthMin="100">
                                    <Header Height="20">
                                        <div class="bluelighting_title_Chart">
                                            <asp:label ID="lblKpiFillRate" runat="server"  Text="Informaci&oacute;n de Fill Rate"></asp:label>                                 
                                        </div>
                                    </Header>
                                    <Content>                                    
                                        <div id="div_FillRate" class="dialog_Chart" style="width:380px; float:left; ">                                               
                                            <table class="table_window_Chart">                                                 
                                                <tr>                                             
                                                    <td class="bluelighting_content_Chart" style="text-align:right">
                                                        <asp:UpdatePanel ID="udpFillRate" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>  
                                                                <div id="divFillRateChart" runat="server">
                                                                <asp:Chart ID="ChartKpiFillRate" Width="340px" runat="server" OnClick="ChartKpiFillRate_Click">
                                                                    <Titles>
                                                                        <asp:Title Name="Title1" >
                                                                      </asp:Title>
                                                                    </Titles>
                                                                    <Series> 
                                                                      <asp:Series Name="Series1" PostBackValue="#"/>                               
                                                                    </Series>
                                                                    <ChartAreas>
                                                                         <asp:ChartArea Name="ChartArea1" BackGradientStyle= "VerticalCenter" 
                                                                         BackSecondaryColor="#B6D6EC" BorderDashStyle= "Solid" BorderWidth="1">
                                                                            <AxisX>
                                                                                <MajorGrid Enabled="False" />
                                                                            </AxisX>
                                                                        </asp:ChartArea>
                                                                    </ChartAreas> 
                                                                </asp:Chart>
                                                                
                                                                <div class="divControls">
                                                                    <div class="fieldRight">
                                                                        <asp:Label CssClass="lblKpiText" ID="lblCustomerCharts" runat="server" Text="Cliente"/>
                                                                    </div>
                                                                    <div class="fieldLeft">
                                                                        <asp:DropDownList ID="ddlCustomerCharts5" runat="server" AutoPostBack="true" SkinID="ddlFilter"
                                                                            OnSelectedIndexChanged="ddlCustomerCharts5_SelectedIndexChanged">
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                
                                                                    <asp:Label CssClass="lblKpiText" ID="Label6" runat="server" Text="Tipo Gr&aacute;fico" Visible="false"/>&nbsp;
                                                                    <asp:DropDownList ID="ddlTypeFillRate" runat="server" AutoPostBack="true" SkinID="ddlFilter" Visible="false"
                                                                            OnSelectedIndexChanged="ddlTypeFillRate_SelectedIndexChanged">                                        
                                                                    </asp:DropDownList>
                                                               
                                                                </div>  
                                                                </div>
                                                            </ContentTemplate>
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="ddlCustomerCharts5" EventName="SelectedIndexChanged" />
                                                                <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />                               
                                                                <asp:AsyncPostBackTrigger ControlID="TimerKpiFillRate" EventName="Tick" />
                                                                <asp:AsyncPostBackTrigger ControlID="ddlTypeFillRate" EventName="SelectedIndexChanged" /> 
                                                            </Triggers>
                                                        </asp:UpdatePanel>
                                                        
                                                        <asp:UpdateProgress ID="upFillRate" AssociatedUpdatePanelID="udpFillRate" runat="server"  DisplayAfter="1" DynamicLayout="true">
                                                            <ProgressTemplate>                        
                                                                <div class="divProgress">
                                                                    <asp:ImageButton ID="imgProgress5" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
                                                                </div>
                                                            </ProgressTemplate>
                                                        </asp:UpdateProgress>  
                                                         <webUc:UpdateProgressOverlayExtender ID="udproeFillRate" runat="server" 
                                                        ControlToOverlayID="divFillRateChart" CssClass="updateProgress" TargetControlID="upFillRate" />                                                               
                                                                              
                                                    </td>                                               
                                                </tr>                                                  
                                            </table>  
                                        </div>                                    
                                    </Content>
                                  
                                </LeftPanel>
                                <RightPanel ID="rightPanel2">
                                    <Header Height="20">
                                        <div class="bluelighting_title_Chart">
                                            <asp:label ID="lblTitleKpiLeadTime" runat="server"  Text="Informaci&oacute;n de Lead Time"></asp:label> 
                                        </div>
                                    </Header>
                                    <Content>                            
                                        <div id="div_LeadTime" class="dialog_Chart" style="width:380px; float:left">                                                
                                            <table class="table_window_Chart">                                                  
                                                <tr>                                                      
                                                    <td class="bluelighting_content_Chart" style="text-align:right">                                                        
                                                        <asp:UpdatePanel ID="udpChartLeadTime" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>     
                                                                <div id="divLeadTimeChart" runat="server">                           
                                                                <asp:Chart ID="ChartKpiLeadTime" Width="340px" runat="server" OnClick="ChartKpiLeadTime_Click">
                                                                    <Titles>
                                                                        <asp:Title Name="Title1" >
                                                                      </asp:Title>
                                                                    </Titles>
                                                                    <Series> 
                                                                      <asp:Series Name="Series1" PostBackValue="#" YValueType="Int32" XValueType="Int32" ChartType="Column" 
                                                                        CustomProperties="PieDrawingStyle=Concave, MaxPixelPointWidth=50" >                     
                                                                      </asp:Series>                       
                                                                    </Series> 
                                                                    <ChartAreas>
                                                                         <asp:ChartArea Name="ChartArea1" BackGradientStyle="TopBottom" 
                                                                         BackSecondaryColor="#B6D6EC" BorderDashStyle="Solid" BorderWidth="1">
                                                                            <AxisX>
                                                                                <MajorGrid Enabled="False" />
                                                                            </AxisX>
                                                                        </asp:ChartArea>
                                                                    </ChartAreas> 
                                                                </asp:Chart>
                                                                
                                                                <div class="divControls">
                                                                    <div class="fieldRight">
                                                                        <asp:Label CssClass="lblKpiText" ID="Label8" runat="server" Text="Tipo Cliente"/>
                                                                    </div>
                                                                    <div class="fieldLeft">
                                                                        <asp:DropDownList ID="ddlLeadTimeCustomer" runat="server" AutoPostBack="true" SkinID="ddlFilter"
                                                                            OnSelectedIndexChanged="ddlLeadTime_SelectedIndexChanged">
                                                                        </asp:DropDownList>                                        
                                                                    </div>  
                                                                    <div style=" widows:80px" class="fieldRight">  
                                                                        <asp:Label CssClass="lblKpiText" ID="lblLeadTime" runat="server" Text="Tipo Delta"/>&nbsp;
                                                                    </div>
                                                                    <div class="fieldLeft">
                                                                        <asp:DropDownList ID="ddlLeadTime" runat="server" AutoPostBack="true" SkinID="ddlFilter"
                                                                            OnSelectedIndexChanged="ddlLeadTime_SelectedIndexChanged">
                                                                            <asp:ListItem Text="Horas" Value="H"></asp:ListItem>
                                                                            <asp:ListItem Text="Dias" Value="D"></asp:ListItem>                                    
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                    
                                                                    <asp:Label CssClass="lblKpiText" ID="Label7" runat="server" Text="Tipo Gr&aacute;fico" Visible="false"/>&nbsp;
                                                                    <asp:DropDownList ID="ddlTypeLeadTime" runat="server" AutoPostBack="true" SkinID="ddlFilter" Visible="false"
                                                                            OnSelectedIndexChanged="ddlTypeLeadTime_SelectedIndexChanged">                                        
                                                                    </asp:DropDownList>
                                                                    
                                                                    </div>
                                                                </div>
                                                            </ContentTemplate>
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="ddlLeadTime" EventName="SelectedIndexChanged" />
                                                                <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" /> 
                                                                <asp:AsyncPostBackTrigger ControlID="TimerKpiLeadTime" EventName="Tick" />
                                                                <asp:AsyncPostBackTrigger ControlID="ddlTypeLeadTime" EventName="SelectedIndexChanged" /> 
                                                                <asp:AsyncPostBackTrigger ControlID="ddlLeadTimeCustomer" EventName="SelectedIndexChanged" /> 
                                                            </Triggers>
                                                        </asp:UpdatePanel>
                                                        <asp:UpdateProgress ID="udprChart"  AssociatedUpdatePanelID="udpChartLeadTime" runat="server" DisplayAfter="1" DynamicLayout="true">
                                                            <ProgressTemplate>                        
                                                                <div style="background-color:White;" class="divProgress">
                                                                    <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
                                                                </div>
                                                            </ProgressTemplate>
                                                        </asp:UpdateProgress>
                                                        
                                                         <webUc:UpdateProgressOverlayExtender ID="udproeChart" runat="server" 
                                                        ControlToOverlayID="divLeadTimeChart" CssClass="updateProgress" TargetControlID="udprChart" />
                                                        
                                                    </td>                                                        
                                                </tr>                                                   
                                            </table>                                                
                                        </div>
                                    
                                    </Content>
                                   
                                </RightPanel>
                            </spl:Splitter>
                        
                        </Content>
                    </BottomPanel>
                </spl:HorizontalSplitter>
            
            </Content>
            <Footer Height="67">
                <div style="color: White">
                    No Borrar - Evita que BottomPanel se solape con StatusBar</div>
            </Footer>
        </RightPanel>
    </spl:Splitter>
    
    
    <asp:UpdatePanel ID="udpChart3" runat="server" UpdateMode="Conditional">
    <ContentTemplate> 
        <div id="divKpiPopUp" runat="server" visible="false" >
            <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy" Drag="true"
                PopupControlID="pnlPopUpChart" BackgroundCssClass="modalBackground" PopupDragHandleControlID="PopUpCaption">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopUpChart" runat="server" CssClass="modalBox">
                <asp:Panel ID="PopUpCaption" runat="server" CssClass="modalHeader">
                    <div class="divCaption">
                        <asp:Label ID="lblTitlePopUp" runat="server" Text="" />  
                        <asp:ImageButton ID="btnClose" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                    </div>
                </asp:Panel>
                <div class="modalControls"  > 
                    <div id="divModalFields" runat="server" class="divCtrsFloatLeft" style="min-width:550px; min-height:300px; max-height:400px;max-width:800px;" >
                        
                        <div id="divGridKpiZone" runat="server" visible="false" onresize="SetDivs();" style="overflow:scroll; max-height:400px;max-width:800px;">
                            <asp:GridView ID="grdGridKpiZone" runat="server" AllowPaging="True" EnableViewState="false" 
                                DataKeyNames="WorkZoneName" PageSize="5000" >
                                <Columns>
                                    <asp:BoundField DataField="WorkZoneName" HeaderText="Zona" AccessibleHeaderText="WorkZoneName" />
                                    <asp:BoundField DataField="PercentageZone" HeaderText="Porcentaje" AccessibleHeaderText="PercentageZone" />  
                                    <asp:BoundField DataField="LocCode" HeaderText="Cod. Ubicacion" AccessibleHeaderText="LocCode" />
                                    <asp:BoundField DataField="LocTypeName" HeaderText="Tipo Ubic." AccessibleHeaderText="LocTypeName" />                                        
                                    <asp:BoundField DataField="ItemQty" HeaderText="Cant. Item" AccessibleHeaderText="ItemQty" />
                                    <asp:BoundField DataField="CapacityUnit" HeaderText="Capacidad" AccessibleHeaderText="CapacityUnit" />
                                    <asp:BoundField DataField="Volume" HeaderText="Volumen" AccessibleHeaderText="Volume" />
                                    <asp:BoundField DataField="TotalVolumen" HeaderText="Total Vol." AccessibleHeaderText="TotalVolumen" />
                                    <asp:BoundField DataField="Weight" HeaderText="Peso" AccessibleHeaderText="Weight" />   
                                    <asp:BoundField DataField="TotalWeight" HeaderText="Total Peso" AccessibleHeaderText="TotalWeight" />  
                                    <asp:BoundField DataField="OccupancyQty" HeaderText="Ocup. por Cant." AccessibleHeaderText="OccupancyQty" />   
                                    <asp:BoundField DataField="OccupancyVolume" HeaderText="Ocup. por Vol." AccessibleHeaderText="OccupancyVolume" />  
                                    <asp:BoundField DataField="OccupancyWeight" HeaderText="Ocup. por Peso" AccessibleHeaderText="OccupancyWeight" />                          
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div id="divGidKpiFillRate" runat="server" visible="false" style="overflow:scroll; max-height:400px;max-width:800px;">
                            <asp:GridView ID="grdGidKpiFillRate" runat="server" AllowPaging="True" EnableViewState="false" PageSize="5000">
                                <Columns>
                                    <asp:BoundField DataField="OutboundNumber" HeaderText="Doc. Salida" AccessibleHeaderText="OutboundNumber" />
                                    <asp:BoundField DataField="LineNumber" HeaderText="Nro. Linea" AccessibleHeaderText="LineNumber" />  
                                    <asp:BoundField DataField="QtyRequested" HeaderText="Cant. Requerida" AccessibleHeaderText="QtyRequested" />  
                                    <asp:BoundField DataField="QtyDispatched" HeaderText="Cant. Despachada" AccessibleHeaderText="QtyDispatched" />  
                                    <asp:BoundField DataField="EmissionDate" HeaderText="F. Emision" AccessibleHeaderText="EmissionDate" />  
                                    <asp:BoundField DataField="ExpectedDate" HeaderText="F. Esperada" AccessibleHeaderText="ExpectedDate" />  
                                    <asp:BoundField DataField="DateCreated" HeaderText="F. Creación" AccessibleHeaderText="DateCreated" />  
                                    <%--<asp:BoundField DataField="CreateDate" HeaderText="CreateDate" AccessibleHeaderText="CreateDate" />  --%>
                                    <asp:BoundField DataField="DateCreatedDetail" HeaderText="F. Detalle" AccessibleHeaderText="DateCreatedDetail" />  
                                    <asp:BoundField DataField="CustomerName" HeaderText="Cliente" AccessibleHeaderText="CustomerName" />  
                                    <asp:BoundField DataField="PercentSatisfaction" HeaderText="% Satisfaccion" AccessibleHeaderText="PercentSatisfaction" />  
                                    <asp:BoundField DataField="Delta" HeaderText="Delta" AccessibleHeaderText="Delta" />  
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div id="divGridKpiLeadTime" runat="server" visible="false" style="overflow:scroll; max-height:400px;max-width:800px;">
                            <asp:GridView ID="grdGridKpiLeadTime" runat="server" AllowPaging="True" EnableViewState="false" PageSize="5000" >
                                <Columns>
                                    <asp:BoundField DataField="OutboundNumber" HeaderText="Doc. Salida" AccessibleHeaderText="OutboundNumber" />
                                    <asp:BoundField DataField="LineNumber" HeaderText="Nro. Linea" AccessibleHeaderText="LineNumber" />  
                                    <asp:BoundField DataField="QtyRequested" HeaderText="Cant. Requerida" AccessibleHeaderText="QtyRequested" />  
                                    <asp:BoundField DataField="QtyDispatched" HeaderText="Cant. Despachada" AccessibleHeaderText="QtyDispatched" />  
                                    <asp:BoundField DataField="EmissionDate" HeaderText="F. Emision" AccessibleHeaderText="EmissionDate" />  
                                    <asp:BoundField DataField="ExpectedDate" HeaderText="F. Esperada" AccessibleHeaderText="ExpectedDate" />  
                                    <asp:BoundField DataField="DateCreated" HeaderText="F. Creación" AccessibleHeaderText="DateCreated" />  
                                    <%--<asp:BoundField DataField="CreateDate" HeaderText="CreateDate" AccessibleHeaderText="CreateDate" />  --%>
                                    <asp:BoundField DataField="DateCreatedDetail" HeaderText="F. Detalle" AccessibleHeaderText="DateCreatedDetail" />  
                                    <asp:BoundField DataField="CustomerName" HeaderText="Cliente" AccessibleHeaderText="CustomerName" />  
                                    <asp:BoundField DataField="Delta" HeaderText="Dias/Horas" AccessibleHeaderText="Delta" />  
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div id="divGridPicking" runat="server" visible="false" style="overflow:scroll; max-height:400px; max-width:800px;">
                            <asp:GridView ID="grdGridPicking" runat="server" AllowPaging="True" EnableViewState="false"  PageSize="5000">
                                <Columns>
                                    <asp:BoundField DataField="UserWms" HeaderText="Usuario" AccessibleHeaderText="UserWms" />
                                    <asp:BoundField DataField="Qty" HeaderText="Cantidad" AccessibleHeaderText="Qty" />  
                                    <asp:BoundField DataField="ShortItemName" HeaderText="Nombre Item" AccessibleHeaderText="ShortItemName" />  
                                    <asp:BoundField DataField="DateCreated" HeaderText="F. Creacion" AccessibleHeaderText="DateCreated" />  
                                    <asp:BoundField DataField="ExpirationDate" HeaderText="F. Expiracion" AccessibleHeaderText="ExpirationDate" />  
                                    <asp:BoundField DataField="IdLocationStage" HeaderText="Id Ubicacion" AccessibleHeaderText="IdLocationStage" />  
                                    <asp:BoundField DataField="WhsName" HeaderText="Centro Distr." AccessibleHeaderText="WhsName" />  
                                    <asp:BoundField DataField="OutboundNumber" HeaderText="Doc. Salida" AccessibleHeaderText="OutboundNumber" />  
                                    <asp:BoundField DataField="LineNumber" HeaderText="Nro. Linea" AccessibleHeaderText="LineNumber" />  
                                    <asp:BoundField DataField="IdLpnCode" HeaderText="LPN" AccessibleHeaderText="IdLpnCode" />  
                                    <asp:BoundField DataField="IdLocSourceUsed" HeaderText="Ubic. Picking" AccessibleHeaderText="IdLocSourceUsed" />                                          
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                    <div id="divActions" runat="server"  class="modalActions" >
                        <asp:Button ID="btnCancel" runat="server" Text="Cerrar" />
                        <asp:Button ID="btnKpiExportToExcel" runat="server" Text="Excel" 
                        onclick="btnKpiExportToExcel_Click" ToolTip="Exportar a Excel"/>                            
                    </div>
                </div>
            </asp:Panel>
        </div>
    </ContentTemplate> 
    
    <Triggers>                                    
        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$Splitter1$leftPanel1$ctl01$ChartKpiZona" EventName="Click" /> 
        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl00$ctl01$Splitter1$rightPanel1$ctl01$ChartKpiPicking" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$Splitter2$rightPanel2$ctl01$ChartKpiLeadTime" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$HorizontalSplitter1$ctl01$ctl01$Splitter2$leftPanel2$ctl01$ChartKpiFillRate" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>

</div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus id="ucStatus" runat="server"/>    
</asp:Content>