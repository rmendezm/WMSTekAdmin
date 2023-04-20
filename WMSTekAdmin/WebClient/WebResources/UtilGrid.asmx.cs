using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Configuration;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace Binaria.WMSTek.WebClient.WebResources
{
    /// <summary>
    /// Descripción breve de UtilGrid
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class UtilGrid : System.Web.Services.WebService
    {
        IConfigurationMGR iConfigurationMGR;
        IProfileMGR iProfileMGR;
        protected ContextViewDTO context;
        public BaseControl baseControl;

        public UtilGrid()
        {
            var objectInstances = (InstanceFactory)Session[WMSTekSessions.Global.ObjectInstances];
            if(objectInstances != null)
            { 
                iConfigurationMGR = (IConfigurationMGR)objectInstances.getObject("configurationMGR");
                iProfileMGR = (IProfileMGR)objectInstances.getObject("profileMGR");
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public BaseViewDTO GetConfigurationColumnsGrid(string nameQuery)
        {
            var context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
            //context = iProfileMGR.LoadContextDetails(context);

            String result = context.CfgParameters.FirstOrDefault(w => w.ParameterCode.ToUpper().Equals(("PruebaDeParamWeb").ToUpper())).ParameterValue.Trim();
            if (result == "1")
            {
                return iConfigurationMGR.GetLayoutConfigurationSimple(nameQuery);
            }
            else
            {
                return new BaseViewDTO() { Errors = new ErrorDTO() { Message = "PruebaDeParamWeb tiene un valor que deshabilita configuración de grilla" }, Configuration = iConfigurationMGR.GetLayoutConfigurationSimple(nameQuery).Configuration };
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public void UpdateVisibilityColumn(bool visibilityGrid, string fieldName, string queryName, string idPage)
        {
            iConfigurationMGR.UpdateVisibilityColumn(visibilityGrid, fieldName, queryName, idPage);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public void UpdateOrderColumns(int fieldOrderMinRange, int fieldOrderMaxRange, int typeDragAndDrop, string fieldName, string queryName, string idPage)
        {
            iConfigurationMGR.UpdateOrderColumns(fieldOrderMinRange, fieldOrderMaxRange, typeDragAndDrop, fieldName, queryName, idPage);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public GenericViewDTO<MenuItem> GetMenuByUser(int idUser)
        {
            var context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
            return iProfileMGR.GetMenuByUser(idUser, context);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public GenericViewDTO<MenuItem> GetMenuByUrl(string url)
        {
            var context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
            return iProfileMGR.GetMenuByUrl(url, context);
        }
    }
}
