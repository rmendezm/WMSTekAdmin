using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.CloudLabel;
using Binaria.WMSTek.Framework.Entities.Label;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Services;
using System.Xml;
using Binaria.WMSTek.Framework.Entities.Base;
using System.Linq;

namespace Binaria.WMSTek.IntegrationClient
{
    /// <summary>
    /// Descripción breve de WmsTekLabelCloud
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WmsTekLabelCloud : wsBase
    {
        public int labelCloudPagination
        {
            get
            {
                int pagination = 0;
                int defaultPagination = 10;

                var labelCloudPagination = ConfigurationManager.AppSettings["LabelCloudPagination"];
                var isParsedOk = int.TryParse(labelCloudPagination, out pagination);
                return isParsedOk ? pagination : defaultPagination;
            }
        }

        [WebMethod]
        public ResponseGet<CloudLabelDTO> wsGetNewPrinting(ParamGetPendingLabel parameters)
        {
            Initialize();

            if (initMsg == "OK")
            {
                Dictionary<string, string> subQueryParams = new Dictionary<string, string>();
                subQueryParams.Add("SubQueryCode", "OrderByIdTask");

                var resultViewDTO = iLabelMGR.WsGetNewPrinting(parameters, labelCloudPagination, context, subQueryParams);

                if (resultViewDTO.hasError())
                {
                    theLog.errorMessage(DateTime.Now.ToString(), this.GetType().FullName, resultViewDTO.Errors.OriginalMessage);

                    var response = new ResponseGet<CloudLabelDTO>();
                    response.Error = new ErrorWS() { Message = "Error interno en WS wsGetNewPrinting" };
                    return response;
                }
                else
                {
                    return resultViewDTO.Entities.First();
                }
            }
            else
            {
                var response = new ResponseGet<CloudLabelDTO>();
                response.Error = new ErrorWS() { Message = "Error al inicializar WS wsGetNewPrinting" };
                return response;
            }
        }
        [WebMethod]
        public void wsPutLogLabelClient(List<ParamPutLogLabelClient> parameters)
        {
            try
            {
                Initialize();

                if (initMsg == "OK")
                    iLabelMGR.wsPutLogLabelClient(parameters, context);
                else
                    theLog.errorMessage(this.GetType().FullName, "Error al inicializar WS wsPutLogLabelClient");
            }
            catch (Exception ex)
            {
                base.LogException(ex, "wsPutLogLabelClient");
            }
        }
    }
}
