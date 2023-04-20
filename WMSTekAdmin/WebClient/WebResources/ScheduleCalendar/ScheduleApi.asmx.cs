using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Base;

namespace Binaria.WMSTek.WebClient.WebResources.ScheduleCalendar
{
    /// <summary>
    /// Descripción breve de ScheduleApi
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class ScheduleApi : System.Web.Services.WebService
    {
        IReceptionMGR iReceptionMGR;
        IWarehousingMGR iWarehousingMGR;
        ApiUtils apiUtils;
        protected ContextViewDTO context;

        public ScheduleApi()
        {
            var objectInstances = (InstanceFactory)Session[WMSTekSessions.Global.ObjectInstances];
            iReceptionMGR = (IReceptionMGR)objectInstances.getObject("receptionMGR");
            iWarehousingMGR = (IWarehousingMGR)objectInstances.getObject("warehousingMGR");
            apiUtils = new ApiUtils();
            context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public GenericViewDTO<Schedule> GetSchedule(string dateStart, string dateEnd)
        {
            context.MainFilter = new List<EntityFilter>();

            var filter = new EntityFilter()
            {
                Name = "DateRange",
                FilterValues = new List<FilterItem>()
            };

            context.MainFilter.Add(filter);

            apiUtils.CreateStartDateFilter(dateStart, context);
            apiUtils.CreateEndDateFilter(dateEnd, context);

            var data = iReceptionMGR.FindAllSchedule(context);

            if (data.hasError())
                throw new Exception(data.Errors.Message);
            else
                return data;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public GenericViewDTO<Schedule> GetScheduleById(int id)
        {
            context.MainFilter = new List<EntityFilter>();

            context.MainFilter.Add(apiUtils.CreateGenericFilter(id.ToString(), "IdSchedule"));

            var data = iReceptionMGR.FindAllSchedule(context);

            if (data.hasError())
                throw new Exception(data.Errors.Message);
            else
                return data;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public void SaveAppointment(string startDate, string endDate, string documentNumber, string licensePlate, string driverName, string title, string comment, decimal loadQty, int loadType)
        {
            DateTime startDateFilter = DateTime.MinValue;
            DateTime.TryParseExact(startDate, "dd/MM/yyyyTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out startDateFilter);

            DateTime endDateFilter = DateTime.MinValue;
            DateTime.TryParseExact(endDate, "dd/MM/yyyyTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out endDateFilter);

            var newSchedule = new Schedule()
            {
                ScheduledDateStart = startDateFilter,
                ScheduledDateEnd = endDateFilter,
                ReceivedDate = DateTime.MinValue,
                Type = new ScheduleType() { Id = (int)eScheduleType.Inbound },
                DocumentNumberBound = documentNumber,
                Driver = new Driver() { Name = driverName },
                Truck = new Truck() { IdCode = licensePlate.ToUpper() },
                Title = title,
                Comment = string.IsNullOrEmpty(comment) ? null : comment,
                LoadQty = loadQty,
                LoadType = loadType
            };

            var data = iReceptionMGR.MaintainSchedule(CRUD.Create, newSchedule, context);

            if (data.hasError())
                throw new Exception(data.Errors.Message);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public void UpdateAppointment(int scheduleId, string startDate, string endDate, string documentNumber, string licensePlate, string driverName, string title, string comment, decimal loadQty, int loadType)
        {
            DateTime startDateFilter = DateTime.MinValue;
            DateTime.TryParseExact(startDate, "dd/MM/yyyyTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out startDateFilter);

            DateTime endDateFilter = DateTime.MinValue;
            DateTime.TryParseExact(endDate, "dd/MM/yyyyTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out endDateFilter);

            var scheduleToUpdate = new Schedule()
            {
                Id = scheduleId,
                ScheduledDateStart = startDateFilter,
                ScheduledDateEnd = endDateFilter,
                DocumentNumberBound = documentNumber,
                Driver = new Driver() { Name = driverName },
                Truck = new Truck() { IdCode = licensePlate.ToUpper() },
                Title = title,
                Comment = string.IsNullOrEmpty(comment) ? null : comment,
                LoadQty = loadQty,
                LoadType = loadType
            };

            var data = iReceptionMGR.MaintainSchedule(CRUD.Update, scheduleToUpdate, context);

            if (data.hasError())
                throw new Exception(data.Errors.Message);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public bool ValidateIsNonWorkingDay(string date)
        {
            DateTime dateFilter = DateTime.MinValue;
            DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dateFilter);

            var data = iWarehousingMGR.GetByDate(dateFilter, context);

            if (data.hasError())
                throw new Exception(data.Errors.Message);
            else
                return data.Entities.Count > 0;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public void DeleteSchedule(int id)
        {
            var data = iReceptionMGR.MaintainSchedule(CRUD.Delete, new Schedule() { Id = id }, context);

            if (data.hasError())
                throw new Exception(data.Errors.Message);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public GenericViewDTO<Driver> GetDrivers(string name)
        {
            context.MainFilter = new List<EntityFilter>();

            var data = iWarehousingMGR.GetDriverByName(context, name);

            if (data.hasError())
                throw new Exception(data.Errors.Message);
            else
                return data;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public int GetMinutesPerAppointment()
        {
            return MinutesPerAppointment();
        }

        private int MinutesPerAppointment()
        {
            int defaultMinutesPerAppointment = 15;
            var minutesPerAppointment = new BasePage() { baseControl = BaseControl.getInstance(HttpContext.Current.Request.PhysicalApplicationPath, context) }.GetConst("MinutesPerAppointment");

            if (minutesPerAppointment != null && minutesPerAppointment.Count > 0)
                return int.Parse(minutesPerAppointment.First());
            else
               return defaultMinutesPerAppointment;
        }
    }
}
