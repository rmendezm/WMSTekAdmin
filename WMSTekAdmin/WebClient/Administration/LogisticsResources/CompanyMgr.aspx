<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="CompanyMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.LogisticsResources.CompanyMgr" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

     <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate> 
            <div class="modalBox">
                <div class="modalBoxContent">
                    <div class="divCtrsFloatLeft">
        
                    <div class="pageHeader">
                        <asp:Label ID="lblCompanyNameTitle" runat="server"/>
                    </div>
                           
                    <div class="divSeparador"></div> 
                    <div id="divIdCompany" runat="server" visible="false" class="divControls">
                        <asp:Label ID="lblIdCompany" runat="server" Text="Id Compañia" Visible="false" />
                        <asp:TextBox ID="txtIdCompany" runat="server" Enabled="False" Width="85px"  
                            MaxLength="20" Visible="false" />            
                    </div>
                    
                    <div id="divCompanyCode" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblCompanyCode" runat="server" Text="Código" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtCompanyCode" runat="server" 
                                Width="100px"  MaxLength="20" />
                            <asp:requiredfieldvalidator ID="rfvCompanyCode" runat="server"  
                                ValidationGroup="EditNew" Text=" * " controltovalidate="txtCompanyCode" 
                                display="dynamic" ErrorMessage="Código es requerido" />
                        </div>
                    </div>
                    
                    <div class="divSeparador"></div>
                            
                    <div id="divCompanyName" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblCompanyName" runat="server" Text="Nombre" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtCompanyName" runat="server" 
                                Width="200px"  MaxLength="100" />
                            <asp:requiredfieldvalidator ID="rfvCompanyName" runat="server"  
                                ValidationGroup="EditNew" Text=" * " controltovalidate="txtCompanyName" 
                                display="dynamic" ErrorMessage="Nombre es requerido" />
                        </div>
                    </div>
                    
                    <div id="divShortCompanyName" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblShortCompanyName" runat="server" Text="Nombre Corto" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtShortCompanyName" runat="server"  MaxLength="10" />
                            <asp:requiredfieldvalidator ID="rfvShortCompanyName" runat="server"  
                            ValidationGroup="EditNew" Text=" * " controltovalidate="txtShortCompanyName" 
                            display="dynamic" ErrorMessage="Nombre Corto es requerido" />
                         </div>   
                    </div>

                    <div id="divTradeName" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblTradeName" runat="server" Text="Nombre Fantasía" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtTradeName" runat="server"  MaxLength="60" />
                            <asp:requiredfieldvalidator ID="rfvTradeName" runat="server"  
                                ValidationGroup="EditNew" Text=" * " controltovalidate="txtTradeName" 
                                display="dynamic" ErrorMessage="Nombre Fantasía es requerido" /> 
                        </div>                       
                    </div>

                    <div id="divGLN" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblGLN" runat="server" Text="GLN" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtGLN" runat="server"  MaxLength="13" />
                             <asp:RegularExpressionValidator ID="revtxtGLN" runat="server" 
                                ControlToValidate="txtGLN" ErrorMessage="GLN permite ingresar solo números" ValidationGroup="EditNew" 
                                ValidationExpression="[0-9999999999999]*" Text="*">
                           </asp:RegularExpressionValidator>
                           <%-- <asp:RegularExpressionValidator ID="revtxtGLN" runat="server" 
                                ControlToValidate="txtGLN" ErrorMessage="GLN permite ingresar solo números" 
                                ValidationExpression="[0-9999999999999]*"
                                ValidationGroup="EditNew" Text=" * ">--%>
                        </div>                       
                    </div>
                    
                    <div class="divSeparador"></div>        
                    
                    <div id="divAddress1" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblAddress1" runat="server" Text="Dirección 1" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtAddress1" runat="server" MaxLength="100" />
                            <asp:requiredfieldvalidator ID="rfvAddress1" runat="server"  
                                ValidationGroup="EditNew" Text=" * " controltovalidate="txtAddress1" 
                                display="dynamic" ErrorMessage="Dirección 1 es requerido" /> 
                        </div>                       
                    </div>
                    
                    <div id="divAddress2" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblAddress2" runat="server" Text="Dirección 2" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtAddress2" runat="server" MaxLength="100" />
                            <asp:requiredfieldvalidator ID="rfvAddress2" runat="server"  
                                ValidationGroup="EditNew" Text=" * " controltovalidate="txtAddress1" 
                                display="dynamic" ErrorMessage="Dirección 2 es requerido" /> 
                        </div>                       
                    </div>
                    
                    <div id="divZipCode" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblZipCode" runat="server" Text="Cód. Postal" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtZipCode" runat="server" Width="50px" MaxLength="20" />
                            <asp:requiredfieldvalidator ID="rfvZipCode" runat="server"  ValidationGroup="EditNew" 
                                Text=" * " controltovalidate="txtAddress1" display="dynamic" 
                                ErrorMessage="Código Postal es requerido" /> 
                        </div>                       
                    </div>
                    
                    <div class="divSeparador"></div>                

                    <div id="divIdCountry" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblIdCountry" runat="server" Text="País" />
                        </div>
                        <div class="fieldLeft">
                            <asp:DropDownList ID="ddlCountry" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" />
                            <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="ddlCountry" 
                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " Display="Dynamic" ErrorMessage="País es requerido">
                            </asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div id="divIdState" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblIdState" runat="server" Text="Región" />
                        </div>
                        <div class="fieldLeft">
                            <asp:DropDownList ID="ddlState" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlState_SelectedIndexChanged" />
                            <asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="ddlState" InitialValue="-1" 
                            ValidationGroup="EditNew"
                            Text=" * " Display="Dynamic" ErrorMessage="Estado es requerido">
                            </asp:RequiredFieldValidator>
                        </div>
                    </div>
                    
                    <div id="divIdCity" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblIdCity" runat="server" Text="Comuna" />
                        </div>
                        <div class="fieldLeft">
                            <asp:DropDownList ID="ddlCity" runat="server" />
                            <asp:RequiredFieldValidator ID="rfvCity" runat="server" 
                                ControlToValidate="ddlCity" 
                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " Display="Dynamic"
                                ErrorMessage="Ciudad es requerido">
                            </asp:RequiredFieldValidator>
                        </div>
                     </div>                        
                    
                    <div class="divSeparador"></div>                
                            
                    <div id="divPhone1" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblPhone1" runat="server" Text="Teléfono 1" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtPhone1" runat="server" Width="80px"  MaxLength="20" />
                            <asp:RequiredFieldValidator ID="rfvPhone1" runat="server" 
                                ControlToValidate="txtPhone1" 
                                ValidationGroup="EditNew" Text=" * " Display="Dynamic"
                                ErrorMessage="Teléfono 1 es requerido">
                            </asp:RequiredFieldValidator>                
                        </div>                    
                    </div>
                    
                    <div id="divPhone2" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblPhone2" runat="server" Text="Teléfono 2" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtPhone2" runat="server" Width="80px"  MaxLength="20" />
                            <asp:RequiredFieldValidator ID="rfvPhone2" runat="server" 
                                ControlToValidate="txtPhone2" 
                                ValidationGroup="EditNew" Text=" * " Display="Dynamic"
                                ErrorMessage="Teléfono 2 es requerido">
                            </asp:RequiredFieldValidator>                  
                        </div>                   
                    </div>
                    
                    <div id="divFax1" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblFax1" runat="server" Text="Fax 1" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtFax1" runat="server" Width="80px"  MaxLength="20" />
                            <asp:RequiredFieldValidator ID="rfvFax1" runat="server" 
                            ControlToValidate="txtFax1" 
                            ValidationGroup="EditNew" Text=" * " Display="Dynamic"
                            ErrorMessage="Fax 1 es requerido">
                            </asp:RequiredFieldValidator>            
                        </div>                      
                    </div>
                    
                    <div id="divFax2" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblFax2" runat="server" Text="Fax 2" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtFax2" runat="server" Width="80px"  MaxLength="20" />
                            <asp:RequiredFieldValidator ID="rfvFax2" runat="server" 
                            ControlToValidate="txtFax2" 
                            ValidationGroup="EditNew" Text=" * " Display="Dynamic"
                            ErrorMessage="Fax 2 es requerido">
                            </asp:RequiredFieldValidator>              
                        </div>                      
                    </div>
                    
                    <div id="divEmail" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblEmail" runat="server" Text="E-mail" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtEmail" runat="server"  MaxLength="100" />
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                                ControlToValidate="txtEmail" 
                                ValidationGroup="EditNew" Text=" * " Display="Dynamic"
                                ErrorMessage="E-mail es requerido">
                            </asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revMail" runat="server" 
                                ControlToValidate="txtEmail" ErrorMessage="Email Inválido" ValidationGroup="EditNew" 
                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">
                           </asp:RegularExpressionValidator>
                        </div>                       
                    </div>
                    
                    <div class="divSeparador"></div>                
                            
                  <%--FIN REGLAS--%>
                  
                    <div id="divReglas" runat="server">
                        <div id="divRulePutCode" runat="server">
                            <asp:Label ID="lblRulePutCode" runat="server" Text="Rule PutCode"  visible="false" />
                        </div>
                        
                        <div id="divRulePickCode" runat="server">
                        </div>
                        
                        <div id="divRuleRplCode" runat="server">
                            <asp:Label ID="lblRuleRplCode" runat="server" Text="Rule RplCode"  visible="false" />
                        </div>
                        
                        <div id="divRuleCDockCode" runat="server">
                            <asp:Label ID="lblRuleCDockCode" runat="server" Text="Rule CDockCode"  visible="false" />
                        </div>
                    </div>
                    
                    <div class="divSeparador"></div>     
                    
                   </div> 
          
                    <div id="Div1" class="divValidationSummary" runat="server">
                        <asp:ValidationSummary ID="rfvSummaryEditNew" runat="server" ValidationGroup="EditNew" />
                    </div>
                </div>
            </div>
                                       
        
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
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
    
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
    	<asp:Label id="lblEmptyRow" runat="server" Text="(Seleccione... )" Visible="false" />        
    <%-- Barra de Estado --%>       
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- FIN Barra de Estado --%>        
    <webUc:ucStatus id="ucStatus" runat="server"/>      
    <%-- FIN Barra de Estado --%>   
</asp:Content>