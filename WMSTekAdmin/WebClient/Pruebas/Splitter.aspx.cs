using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Script.Services;
using System.Runtime.Serialization.Json;
using System.Web.Services;

using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Layout;


namespace Binaria.WMSTek.WebClient.Pruebas
{
    public partial class Splitter : BasePage
    {
        #region "Declaración de Variables"
        private static GenericViewDTO<Hangar> hangarViewDTO = new GenericViewDTO<Hangar>();
        private bool isValidViewDTO = false;
        #endregion

        #region "Eventos"
        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        // Carga inicial del ViewDTO
                        UpdateSession(false);
                        //PopulateLists();
                    }

                    if (ValidateSession(WMSTekSessions.TerminalMgr.TerminalList))
                    {
                        hangarViewDTO = (GenericViewDTO<Hangar>)Session[WMSTekSessions.Map2DConsult.CurrentHangar];
                        isValidViewDTO = true;
                    }

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                        //PopulateGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                hangarViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
        }
        #endregion

        #region "Métodos"
        protected void Initialize()
        {
            /*
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            */
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
                hangarViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            hangarViewDTO = iLayoutMGR.FindAllHangar(context, true);

            if (!hangarViewDTO.hasError() && hangarViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Map2DConsult.CurrentHangar, hangarViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(hangarViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(hangarViewDTO.Errors);
            }
        }

        [WebMethod]
        public static Hangar GetHangar()
        {
            Hangar hangar = new Hangar();

            if (hangarViewDTO.Entities != null && hangarViewDTO.Entities.Count > 0)
            {
                hangar = hangarViewDTO.Entities[0];
            }

            return hangar;
        }
        #endregion
    }
}
