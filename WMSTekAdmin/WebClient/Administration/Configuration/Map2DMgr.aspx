<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="Map2DMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Configuration.Map2DMgr" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register Assembly="Karpach.WebControls" Namespace="Karpach.WebControls" TagPrefix="webUc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

     <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate> 
            <div id="divTabs" style="height:100%;margin:6px" onload="GetColPreview();">
                <ajaxToolkit:TabContainer runat="server" ID="tabMap2D" ActiveTabIndex="0" Height="300px">
                    <ajaxToolkit:TabPanel runat="server" ID="tabMap" >
                        <ContentTemplate>
                            <%-- Map Preview --%>   
                            <div id="divMapPreviewLabel" style="top:73px;right:192px;position:absolute">
                                <input type="button" value="Vista Previa" onclick="GetMapPreview();" style="width:80px;height:19px;font-size:11px" />                                 
                            </div> 
                            
                            <div id="divMapPreview" style="top:100px;right:32px;position:absolute;width:240px;height:240px" >
                            </div>
                        
                            <%-- Map / Hangar Section --%>
                            <div id="divMapHangar" style="float:left;margin:8px;margin-right:15px">
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            <div class="pageHeader"><asp:Label ID="lblMapTitle" runat="server" Text="Mapa" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblBackColor" runat="server" Text="Fondo" /></div>
                                        </td>
                                        <td>                                            
                                            <div class="fieldLeft" ><webUc:ColorPicker ID="cpBackColor" name="cpBackColor" runat="server" /></div>
                                        </td>                                    
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblMargin" runat="server" Text="Margen" /></div>
                                        </td>
                                        <td>                                            
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtMargin" runat="server" MaxLength="10" Width="30" />
                                                <asp:RequiredFieldValidator ID="rfvMargin" runat="server" ControlToValidate="txtMargin" ValidationGroup="EditNew" Text=" * " ErrorMessage="Margen es requerido" />
                                                <asp:RangeValidator ID="rvMargin" runat="server" ErrorMessage="Margen debe estar entre 0 y 1000." Type="Integer" MaximumValue="1000" MinimumValue="0" Text="*" ControlToValidate="txtMargin" ValidationGroup="EditNew" />
                                                <ajaxToolkit:FilteredTextBoxExtender ID="ftMargin" runat="server" TargetControlID="txtMargin" FilterType="Numbers" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <div class="pageHeader"><asp:Label ID="lblHangarTitle" runat="server" Text="Bodega" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblHangarBackColor" runat="server" Text="Fondo" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpHangarBackColor" runat="server" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblHangarBorderColor" runat="server" Text="Color Borde" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpHangarBorderColor" runat="server" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblHangarBorder" runat="server" Text="Borde" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft">    
                                                <asp:TextBox ID="txtHangarBorder" runat="server" MaxLength="10" Width="30" />
                                                <asp:RequiredFieldValidator ID="rfvHangarBorder" runat="server" ControlToValidate="txtHangarBorder" ValidationGroup="EditNew" Text=" * " ErrorMessage="Borde Bodega es requerido" />
                                                <asp:RangeValidator ID="rvHangarBorder" runat="server" ErrorMessage="Borde Bodega debe estar entre 0 y 1000." Type="Integer" MaximumValue="1000" MinimumValue="0" Text="*" ControlToValidate="txtHangarBorder" ValidationGroup="EditNew" />
                                                <ajaxToolkit:FilteredTextBoxExtender ID="ftHangarBorder" runat="server" TargetControlID="txtHangarBorder" FilterType="Numbers" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
     
                            </div>
                            
                            <%-- Column Section --%>
                            <div id="divColumn" style="float:left;margin:8px;margin-left:14px">
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            <div class="pageHeader"><asp:Label ID="lblColumnTitle" runat="server" Text="Stock" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                         <div class="fieldRightNarrow"><asp:Label ID="lblColumnBackColor5" runat="server" Text="5 Niveles" /></div>
                                        </td>
                                        <td>                                       
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnBackColor5" runat="server" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblColumnBackColor4" runat="server" Text="4 Niveles" /></div>
                                        </td>
                                        <td>                                        
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnBackColor4" runat="server" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblColumnBackColor3" runat="server" Text="3 Niveles" /></div>
                                        </td>
                                        <td>                                        
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnBackColor3" runat="server" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblColumnBackColor2" runat="server" Text="2 Niveles" /></div>
                                        </td>
                                        <td>                                        
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnBackColor2" runat="server" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblColumnBackColor1" runat="server" Text="1 Nivel" /></div>
                                        </td>
                                        <td>                                        
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnBackColor1" runat="server" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblColumnBackColor0" runat="server" Text="Vacía" /></div>
                                        </td>
                                        <td>                                        
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnBackColor0" runat="server" /></div>
                                        </td>
                                   </tr>
                                </table>
                            </div>       
                                     
                            <div id="divColumn2" style="float:left;margin:8px;margin-left:18px" >
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            <div class="pageHeader"><asp:Label ID="lblColumna" runat="server" Text="Columna" /></div>                        
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblColumnBorderColor" runat="server" Text="Color Borde" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnBorderColor" runat="server" /></div>
                                        <td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblColumnBorder" runat="server" Text="Borde" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft">    
                                                <asp:TextBox ID="txtColumnBorder" runat="server" MaxLength="10" Width="30" />
                                                <asp:RequiredFieldValidator ID="rfvColumnBorder" runat="server" ControlToValidate="txtColumnBorder" ValidationGroup="EditNew" Text=" * " ErrorMessage="Borde Columna es requerido" />
                                                <asp:RangeValidator ID="rvColumnBorder" runat="server" ErrorMessage="Borde Columna debe estar entre 0 y 1000." Type="Integer" MaximumValue="1000" MinimumValue="0" Text="*" ControlToValidate="txtColumnBorder" ValidationGroup="EditNew" />
                                                <ajaxToolkit:FilteredTextBoxExtender ID="ftColumnBorder" runat="server" TargetControlID="txtColumnBorder" FilterType="Numbers" />
                                            </div>
                                        </td>
                                    </tr>   
                                    <tr> 
                                        <td>             
                                            <div class="fieldRightNarrow"><asp:Label ID="lblColumnActiveColor" runat="server" Text="Bajo Cursor" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnBackColorActive" runat="server" PopupPosition="BottomRight" /></div>
                                        </td>
                                    </tr> 
                                    <tr>  
                                        <td>          
                                            <div class="fieldRightNarrow"><asp:Label ID="lblColumnActiveBorderColor" runat="server" Text="Borde" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnBorderColorActive" runat="server" /></div>
                                        </td>
                                    </tr>     
                                    <tr> 
                                        <td>
                                            <div class="fieldRightNarrow"><asp:Label ID="lblColumnItemColor" runat="server" Text="Color Item" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnBackColorItem" runat="server" /></div>                
                                        </td>
                                    </tr> 
                                </table>    
                            </div>    
                            
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" ID="tabCol">
                        <ContentTemplate>     
                            <%-- Column Details Preview --%>    
                            <div id="divColPreviewLabel" style="top:73px;right:186px;position:absolute">
                                <input type="button" value="Vista Previa" onclick="GetColPreview();" style="width:80px;height:19px;font-size:11px" />                                         
                            </div> 
                            <div id="divColPreview" style="top:96px;right:32px;position:absolute;width:230px;height:265px" >
                            </div>
                                                              
                            <%-- Column Details --%>       
                            <div style="float:left;margin:8px;">
                                <table>
	                                <tr>
		                                <td colspan="2">
			                                <div class="pageHeader"><asp:Label ID="lblColumnDetailTitle" runat="server" Text="Ventana" /></div>
		                                </td>
	                                </tr>

	                                <tr>   
		                                <td>                             
			                                <div class="fieldRightNarrow"><asp:Label ID="lblColumnDetailBackColor" runat="server" Text="Fondo" /></div>
		                                </td>
		                                <td>
			                                <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnDetailBackColor" runat="server" /></div>
		                                </td>
	                                </tr>                                
                                  
	                                <tr>          
		                                <td>                                      
			                                <div class="fieldRightNarrow"><asp:Label ID="lblColumnDetailBorderColor" runat="server" Text="Color Borde" /></div>
		                                </td>
		                                <td>
			                                <div class="fieldLeft"><webUc:ColorPicker ID="cpColumnDetailBorderColor" runat="server" /></div>
		                                </td>
	                                </tr>

	                                <tr>               
		                                <td>                 
			                                <div class="fieldRightNarrow"><asp:Label ID="lblColumnDetailBorder" runat="server" Text="Borde" /></div>
		                                </td>
		                                <td>
			                                <div class="fieldLeft">    
				                                <asp:TextBox ID="txtColumnDetailBorder" runat="server" MaxLength="10" Width="30" />
				                                <asp:RequiredFieldValidator ID="rfvColumnDetailBorder" runat="server" ControlToValidate="txtColumnDetailBorder" ValidationGroup="EditNew" Text=" * " ErrorMessage="Borde Ventana es requerido" />
				                                <asp:RangeValidator ID="rvColumnDetailBorder" runat="server" ErrorMessage="Borde Ventana debe estar entre 0 y 1000." Type="Integer" MaximumValue="1000" MinimumValue="0" Text="*" ControlToValidate="txtColumnDetailBorder" ValidationGroup="EditNew" />
				                                <ajaxToolkit:FilteredTextBoxExtender ID="ftColumnDetailBorder" runat="server" TargetControlID="txtColumnDetailBorder" FilterType="Numbers" />
			                                </div>
		                                </td>
	                                </tr>

	                                <tr>
		                                <td>
			                                <div class="fieldRightNarrow"><asp:Label ID="lblColumnDetailPadding" runat="server" Text="Padding" /></div>
		                                </td>
		                                <td>
			                                <div class="fieldLeft">    
				                                <asp:TextBox ID="txtColumnDetailPadding" runat="server" MaxLength="10" Width="30" />
				                                <asp:RequiredFieldValidator ID="rfvColumnDetailPadding" runat="server" ControlToValidate="txtColumnDetailPadding" ValidationGroup="EditNew" Text=" * " ErrorMessage="Padding Ventana es requerido" />
				                                <asp:RangeValidator ID="rvColumnDetailPadding" runat="server" ErrorMessage="Padding Ventana debe estar entre 0 y 1000." Type="Integer" MaximumValue="1000" MinimumValue="0" Text="*" ControlToValidate="txtColumnDetailPadding" ValidationGroup="EditNew" />
				                                <ajaxToolkit:FilteredTextBoxExtender ID="ftColumnDetailPadding" runat="server" TargetControlID="txtColumnDetailPadding" FilterType="Numbers" />
			                                </div>
		                                </td>
	                                </tr>

	                                <tr>  
		                                <td>                          
			                                <div class="fieldRightNarrow"><asp:Label ID="lblColumnDetailMinHeight" runat="server" Text="Alto Mín." /></div>
		                                </td>
		                                <td>
			                                <div class="fieldLeft">    
				                                <asp:TextBox ID="txtColumnDetailMinHeight" runat="server" MaxLength="10" Width="30" />
				                                <asp:RequiredFieldValidator ID="rfvColumnDetailMinHeight" runat="server" ControlToValidate="txtColumnDetailMinHeight" ValidationGroup="EditNew" Text=" * " ErrorMessage="Alto Mín. Ventana es requerido" />
				                                <asp:RangeValidator ID="rvColumnDetailMinHeight" runat="server" ErrorMessage="Alto Mín. Ventana debe estar entre 0 y 10000." Type="Integer" MaximumValue="10000" MinimumValue="0" Text="*" ControlToValidate="txtColumnDetailMinHeight" ValidationGroup="EditNew" />
				                                <ajaxToolkit:FilteredTextBoxExtender ID="ftColumnDetailMinHeight" runat="server" TargetControlID="txtColumnDetailMinHeight" FilterType="Numbers" />
			                                </div>
		                                </td>
	                                </tr>

	                                <tr>     
		                                <td>                                           
			                                <div class="fieldRightNarrow"><asp:Label ID="lblColumnDetailMaxHeight" runat="server" Text="Alto Máx." /></div>
		                                </td>
		                                <td>
			                                <div class="fieldLeft">    
				                                <asp:TextBox ID="txtColumnDetailMaxHeight" runat="server" MaxLength="10" Width="30" />
				                                <asp:RequiredFieldValidator ID="rfvColumnDetailMaxHeight" runat="server" ControlToValidate="txtColumnDetailMaxHeight" ValidationGroup="EditNew" Text=" * " ErrorMessage="Alto Máx Ventana es requerido" />
				                                <asp:RangeValidator ID="rvColumnDetailMaxHeight" runat="server" ErrorMessage="Alto Máx Ventana debe estar entre 0 y 10000." Type="Integer" MaximumValue="10000" MinimumValue="0" Text="*" ControlToValidate="txtColumnDetailMaxHeight" ValidationGroup="EditNew" />
				                                <ajaxToolkit:FilteredTextBoxExtender ID="ftColumnDetailMaxHeight" runat="server" TargetControlID="txtColumnDetailMaxHeight" FilterType="Numbers" />
			                                </div> 
		                                </td>           
	                                </tr>

	                                <tr>
		                                <td>
			                                <div class="fieldRightNarrow"><asp:Label ID="lblColumnDetailMinWidth" runat="server" Text="Ancho Mín." /></div>
		                                </td>
		                                <td>
			                                <div class="fieldLeft">    
				                                <asp:TextBox ID="txtColumnDetailMinWidth" runat="server" MaxLength="10" Width="30" />
				                                <asp:RequiredFieldValidator ID="rfvColumnDetailMinWidth" runat="server" ControlToValidate="txtColumnDetailMinWidth" ValidationGroup="EditNew" Text=" * " ErrorMessage="Ancho Mín. Ventana es requerido" />
				                                <asp:RangeValidator ID="rvColumnDetailMinWidth" runat="server" ErrorMessage="Ancho Mín. Ventana debe estar entre 0 y 10000." Type="Integer" MaximumValue="10000" MinimumValue="0" Text="*" ControlToValidate="txtColumnDetailMinWidth" ValidationGroup="EditNew" />
				                                <ajaxToolkit:FilteredTextBoxExtender ID="ftColumnDetailMinWidth" runat="server" TargetControlID="txtColumnDetailMinWidth" FilterType="Numbers" />
			                                </div>   
		                                </td>
	                                </tr>

	                                <tr>      
		                                <td>                          
			                                <div class="fieldRightNarrow"><asp:Label ID="lblColumnDetailMaxWidth" runat="server" Text="Ancho Máx." /></div>
		                                </td>
		                                <td>
			                                <div class="fieldLeft">    
				                                <asp:TextBox ID="txtColumnDetailMaxWidth" runat="server" MaxLength="10" Width="30" />
				                                <asp:RequiredFieldValidator ID="rfvColumnDetailMaxWidth" runat="server" ControlToValidate="txtColumnDetailMaxWidth" ValidationGroup="EditNew" Text=" * " ErrorMessage="Ancho Máx. Ventana es requerido" />
				                                <asp:RangeValidator ID="rvColumnDetailMaxWidth" runat="server" ErrorMessage="Ancho Máx. Ventana debe estar entre 0 y 10000." Type="Integer" MaximumValue="10000" MinimumValue="0" Text="*" ControlToValidate="txtColumnDetailMaxWidth" ValidationGroup="EditNew" />
				                                <ajaxToolkit:FilteredTextBoxExtender ID="ftColumnDetailMaxWidth" runat="server" TargetControlID="txtColumnDetailMaxWidth" FilterType="Numbers" />
			                                </div>
		                                </td>
	                                </tr>
                                </table>
                            </div> 
                                            
                            <%-- Location Details --%>                                               
                            <div style="float:left;margin:8px;margin-left:16px">
                               <table>
                                    <tr>
	                                    <td colspan="2">
		                                    <div class="pageHeader"><asp:Label ID="lblLocationTitle" runat="server" Text=" Niveles" /></div>
	                                    </td>
                                    </tr>
                                    <tr>
	                                    <td>
		                                    <div class="fieldRightNarrow"><asp:Label ID="lblLocationBackColor5" runat="server" Text="Nivel 5" /></div>
	                                    </td>
	                                    <td>
		                                    <div class="fieldLeft"><webUc:ColorPicker ID="cpLocationBackColor5" runat="server" /></div>
	                                    </td>
                                    </tr>
                                    <tr>
	                                    <td>
		                                    <div class="fieldRightNarrow"><asp:Label ID="lblLocationBackColor4" runat="server" Text="Nivel 4" /></div>
	                                    </td>
	                                    <td>
		                                    <div class="fieldLeft"><webUc:ColorPicker ID="cpLocationBackColor4" runat="server" /></div>
	                                    </td>
                                    </tr>
                                    <tr>
	                                    <td>
		                                    <div class="fieldRightNarrow"><asp:Label ID="lblLocationBackColor3" runat="server" Text="Nivel 3" /></div>
	                                    </td>
	                                    <td>
		                                    <div class="fieldLeft"><webUc:ColorPicker ID="cpLocationBackColor3" runat="server" /></div>
	                                    </td>
                                    </tr>
                                    <tr>
	                                    <td>
		                                    <div class="fieldRightNarrow"><asp:Label ID="lblLocationBackColor2" runat="server" Text="Nivel 2" /></div>
	                                    </td>
	                                    <td>
		                                    <div class="fieldLeft"><webUc:ColorPicker ID="cpLocationBackColor2" runat="server" /></div>
	                                    </td>
                                    </tr>    
                                    <tr> 
	                                    <td>                                                                                           
		                                    <div class="fieldRightNarrow"><asp:Label ID="lblLocationBackColor1" runat="server" Text="Nivel 1" /></div>
	                                    </td>
	                                    <td>
		                                    <div class="fieldLeft"><webUc:ColorPicker ID="cpLocationBackColor1" runat="server" /></div>
	                                    </td>
                                    </tr>
                                </table>
                            </div>       
                            
                            <div style="float:left;margin:8px;">
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            <div class="pageHeader"><asp:Label ID="lblLocationDetailTitle" runat="server" Text="Ubicación"/></div>                        
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>                   
                                            <div class="fieldRight"><asp:Label ID="lblLocationDetailBorderColor" runat="server" Text="Color Borde" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft"><webUc:ColorPicker ID="cpLocationDetailBorderColor" runat="server" /></div>                                                        
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>                
                                            <div class="fieldRight"><asp:Label ID="lblLocationDetailBorder" runat="server" Text="Borde" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft">    
                                                <asp:TextBox ID="txtLocationDetailBorder" runat="server" MaxLength="10" Width="30" />
                                                <asp:RequiredFieldValidator ID="rfvLocationDetailBorder" runat="server" ControlToValidate="txtLocationDetailBorder" ValidationGroup="EditNew" Text=" * " ErrorMessage="Borde Ubicación es requerido" />
                                                <asp:RangeValidator ID="rvLocationDetailBorder" runat="server" ErrorMessage="Borde Ubicación debe estar entre 0 y 1000." Type="Integer" MaximumValue="1000" MinimumValue="0" Text="*" ControlToValidate="txtLocationDetailBorder" ValidationGroup="EditNew" />
                                                <ajaxToolkit:FilteredTextBoxExtender ID="ftLocationDetailBorder" runat="server" TargetControlID="txtLocationDetailBorder" FilterType="Numbers" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="fieldRight"><asp:Label ID="lblLocationDetailPadding" runat="server" Text="Padding" /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft">    
                                                <asp:TextBox ID="txtLocationDetailPadding" runat="server" MaxLength="10" Width="30" />
                                                <asp:RequiredFieldValidator ID="rfvLocationDetailPadding" runat="server" ControlToValidate="txtLocationDetailPadding" ValidationGroup="EditNew" Text=" * " ErrorMessage="Padding Ubicación es requerido" />
                                                <asp:RangeValidator ID="rvLocationDetailPadding" runat="server" ErrorMessage="Padding Ubicación debe estar entre 0 y 1000." Type="Integer" MaximumValue="1000" MinimumValue="0" Text="*" ControlToValidate="txtLocationDetailPadding" ValidationGroup="EditNew" />
                                                <ajaxToolkit:FilteredTextBoxExtender ID="ftLocationDetailPadding" runat="server" TargetControlID="txtLocationDetailPadding" FilterType="Numbers" />
                                            </div>                                             
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>                       
                                            <div class="fieldRight"><asp:Label ID="lblLocationDetailMinHeight" runat="server" Text="Alto Mín." /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft">    
                                                <asp:TextBox ID="txtLocationDetailMinHeight" runat="server" MaxLength="10" Width="30" />
                                                <asp:RequiredFieldValidator ID="rfvLocationDetailMinHeight" runat="server" ControlToValidate="txtLocationDetailMinHeight" ValidationGroup="EditNew" Text=" * " ErrorMessage="Alto Mín. Ubicación es requerido" />
                                                <asp:RangeValidator ID="rvLocationDetailMinHeight" runat="server" ErrorMessage="Alto Mín. Ubicación debe estar entre 0 y 10000." Type="Integer" MaximumValue="10000" MinimumValue="0" Text="*" ControlToValidate="txtLocationDetailMinHeight" ValidationGroup="EditNew" />
                                                <ajaxToolkit:FilteredTextBoxExtender ID="ftLocationDetailMinHeight" runat="server" TargetControlID="txtLocationDetailMinHeight" FilterType="Numbers" />
                                            </div>                                                   
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>                                     
                                            <div class="fieldRight"><asp:Label ID="lblLocationDetailMaxHeight" runat="server" Text="Alto Máx." /></div>
                                        </td>
                                        <td>
                                            <div class="fieldLeft">    
                                                <asp:TextBox ID="txtLocationDetailMaxHeight" runat="server" MaxLength="10" Width="30" />
                                                <asp:RequiredFieldValidator ID="rfvLocationDetailMaxHeight" runat="server" ControlToValidate="txtLocationDetailMaxHeight" ValidationGroup="EditNew" Text=" * " ErrorMessage="Alto Máx Ubicación es requerido" />
                                                <asp:RangeValidator ID="rvLocationDetailMaxHeight" runat="server" ErrorMessage="Alto Máx Ubicación debe estar entre 0 y 10000." Type="Integer" MaximumValue="10000" MinimumValue="0" Text="*" ControlToValidate="txtLocationDetailMaxHeight" ValidationGroup="EditNew" />
                                                <ajaxToolkit:FilteredTextBoxExtender ID="ftLocationDetailMaxHeight" runat="server" TargetControlID="txtLocationDetailMaxHeight" FilterType="Numbers" />
                                            </div> 
                                        </td>
                                    </tr>
                                </table> 
                            </div>     
                            

                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>

                    <ajaxToolkit:TabPanel runat="server" ID="tabColorMap">
                        <ContentTemplate> 
                            <div class="container">
                                <div class="row">
                                    <div class="col-md-12">
                                         <div class="pageHeader"><asp:Label ID="lblRangeColorTitle" runat="server" Text="Rango Uso Ubicación" /></div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-2">
                                        <asp:Label ID="lblHighColor" runat="server" Text="Más usado (80 - 100%)" />
                                    </div>
                                    <div class="col-md-4">
                                        <webUc:ColorPicker ID="cpHighColor" runat="server" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-2">
                                        <asp:Label ID="lblNormalHighColor" runat="server" Text="Medianamente alto usado (60 - 79%)" />
                                    </div>
                                    <div class="col-md-4">
                                        <webUc:ColorPicker ID="cpNormalHighColor" runat="server" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-2">
                                        <asp:Label ID="lblNormalColor" runat="server" Text="Medianamente usado (40 - 59%)" />
                                    </div>
                                    <div class="col-md-4">
                                        <webUc:ColorPicker ID="cpNormalColor" runat="server" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-2">
                                        <asp:Label ID="lblNormalLowColor" runat="server" Text="Medianamente poco usado (20 - 39%)" />
                                    </div>
                                    <div class="col-md-4">
                                        <webUc:ColorPicker ID="cpNormaLowColor" runat="server" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-2">
                                        <asp:Label ID="lblLowColor" runat="server" Text="Menos usado (0 - 19%)" />
                                    </div>
                                    <div class="col-md-4">
                                        <webUc:ColorPicker ID="cpLowColor" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </ajaxToolkit:TabContainer>   

            <div style="clear:both;margin:8px">
                <div class="divValidationSummary" runat="server">
                    <asp:ValidationSummary ID="rfvSummaryEditNew" runat="server" ValidationGroup="EditNew" />
                </div>                            
            </div>                                                                            
            </div>                                       
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
      </Triggers>
    </asp:UpdatePanel>  

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>    
    
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%>  
    
            
          
    <%-- Mensajes de Confirmacion y Auxiliares --%>     
    <asp:Label ID="lbltabMapa" runat="server" Text="Mapa" Visible="false" />           
    <asp:Label ID="lbltabCol" runat="server" Text="Detalles Columna" Visible="false" />     
    <asp:Label ID="lblTabColorMap" runat="server" Text="Mapa Color" Visible="false" />        
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>