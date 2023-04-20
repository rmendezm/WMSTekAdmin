<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskBarContent.ascx.cs" Inherits="Binaria.WMSTek.WebClient.Shared.TaskBarContent" %>

<div id="divTaskBar" class="taskBarPanel" runat="server">
    <div class="taskBarPanelItem">
        <asp:ImageButton ID="btnRefresh" runat="server" onclick="btnRefresh_Click"
            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_refresh.png" 
            Visible="false"
            ToolTip="Actualizar" />	  
    </div>              
    <div class="taskBarPanelItem">    
        <asp:ImageButton ID="btnNew" runat="server" onclick="btnNew_Click"  
            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_new.png" 
            Visible="false"
            ToolTip="Nuevo" />
    </div>    
    <div class="taskBarPanelItem">    
            <img src="../WebResources/Images/Buttons/TaskBar/icon_new.png" ID="imgClientNew" runat="server" onclick="showModalNew()" 
                onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_new_on.png';"
                onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_new.png';"
                visible="false"
                alt="Nuevo"                 
            />
    </div>                
    <div class="taskBarPanelItem">               
        <asp:ImageButton ID="btnSave" runat="server" onclick="btnSave_Click" 
            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_save.png" CausesValidation="true" ValidationGroup="EditNew"
            Visible="false"
            ToolTip="Salvar cambios" />
    </div>              
    <div class="taskBarPanelItem">               
        <asp:ImageButton ID="btnExcel" runat="server" onclick="btnExcel_Click" 
            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_export_excel.png" CausesValidation="true" ValidationGroup="EditNew"
            Visible="false"
            ToolTip="Exportar a Excel" />        
    </div>
    <div class="taskBarPanelItem">
        <asp:ImageButton ID="btnExcelDetail" runat="server" OnClick="btnExcelDetail_Click"
            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_export_excel.png" CausesValidation="true" ValidationGroup="EditNew"
            Visible="false"
            ToolTip="Exportar Detalle a Excel" />
    </div>
    <div class="taskBarPanelItem">               
        <asp:ImageButton ID="btnPrint" runat="server" onclick="btnPrint_Click" 
            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_print.png" CausesValidation="true" ValidationGroup="valPrint"
            Visible="false"
            ToolTip="Imprimir" />        
    </div>
    <div class="taskBarPanelItem">               
        <asp:ImageButton ID="btnAdd" runat="server" onclick="btnAdd_Click" 
            ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png" CausesValidation="true" ValidationGroup="valPrint"
            Visible="false"
            ToolTip="Añadir(Masivo)" />        
    </div>       

     <div class="taskBarPanelItem">               
        <asp:ImageButton ID="btnDownload" runat="server" onclick="btnDownload_Click" 
            ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_apply_inventory.png" CausesValidation="true" ValidationGroup="valPrint"
            Visible="false"
            ToolTip="Descargar Archivo" />        
    </div> 
    
    <div class="taskBarPanelItem">               
        <asp:ImageButton ID="btnPrintAll" runat="server" onclick="btnPrintAll_Click" 
            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_print_more.png" CausesValidation="true" ValidationGroup="valPrint"
            Visible="false"
            ToolTip="Imprimir Todos" />        
    </div>
        <div class="taskBarPanelItem">               
        <asp:ImageButton ID="btnSendData" runat="server" onclick="btnSendData_Click" 
            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_up" CausesValidation="true" ValidationGroup="valPrint"
            Visible="false"
            ToolTip="Enviar Datos" />        
    </div>
</div>