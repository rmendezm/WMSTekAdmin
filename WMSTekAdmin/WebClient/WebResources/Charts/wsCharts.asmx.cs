using Binaria.WMSTek.AdminApp.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dashboard;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using System.Globalization;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using System.Data;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.WebResources.Charts
{
    /// <summary>
    /// Descripción breve de wsCharts
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class wsCharts : System.Web.Services.WebService
    {
        IDashboardMGR iDashboardMGR;
        IWarehousingMGR iWarehousingMGR;
        protected ContextViewDTO context;

        public wsCharts()
        {
            var objectInstances = (InstanceFactory)Session[WMSTekSessions.Global.ObjectInstances];
            iDashboardMGR = (IDashboardMGR)objectInstances.getObject("dashboardMGR");
            iWarehousingMGR = (IWarehousingMGR)objectInstances.getObject("warehousingMGR");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<KpiPickingModel> KpiPicking(int idWhs, string user, string typeUnid)
        {
            GenericViewDTO<KpiProductivity> taskKpiProductivityViewDTO = new GenericViewDTO<KpiProductivity>();
            DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
            context = new ContextViewDTO();
            context.MainFilter = new List<EntityFilter>();
            var arrEnum = Enum.GetValues(typeof(EntityFilterName));

            foreach (var item in arrEnum)
            {
                context.MainFilter.Add(new EntityFilter(item.ToString(), new FilterItem()));
            }

            if (user == "-1")
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Clear();
            }
            else
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].AddFilterItem(new FilterItem("", user));
            }

            taskKpiProductivityViewDTO = iDashboardMGR.GetKpiProductivityByUserWhs(idWhs, context);

            if (typeUnid.ToUpper() == "CANTIDAD")
            {
                //Suma las Cantidades
                var lst = from i in taskKpiProductivityViewDTO.Entities.ToList()
                          group i by new { i.DateCreated.Month, i.DateCreated.Year } into g
                          select new KpiPickingModel()
                          {
                              Qty = g.Sum(a => a.Qty),
                              UserWms = g.First().UserWms,
                              Month = formatoFecha.GetMonthName(g.First().DateCreated.Month),
                              Year = g.First().DateCreated.Year.ToString()
                          };

                return lst.ToList();
            }

            if (typeUnid.ToUpper() == "LINEAS")
            {
                //Cuenta la Cantidad de Lineas
                var lstLine = from i in taskKpiProductivityViewDTO.Entities.ToList()
                              group i by new { i.DateCreated.Month, i.DateCreated.Year } into g
                              select new KpiPickingModel()
                              {
                                  Qty = g.Count(),
                                  UserWms = g.First().UserWms,
                                  Month = formatoFecha.GetMonthName(g.First().DateCreated.Month),
                                  Year = g.First().DateCreated.Year.ToString()
                              };

                return lstLine.ToList();
            }

            if (typeUnid.ToUpper() == "LPN")
            {
                //Cuenta la Cantidad de LPNS
                var lstLine = from i in taskKpiProductivityViewDTO.Entities.ToList()
                              group i by new { i.DateCreated.Month, i.DateCreated.Year } into g
                              select new KpiPickingModel()
                              {
                                  Qty = g.Select(l => l.IdLpnCode).Distinct().Count(),
                                  UserWms = g.First().UserWms,
                                  Month = formatoFecha.GetMonthName(g.First().DateCreated.Month),
                                  Year = g.First().DateCreated.Year.ToString()
                              };

                return lstLine.ToList();
            }

            return null;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<KpiZoneModel> KpiZone(int idWhs, string zoneCode, string typeCharts)
        {
            GenericViewDTO<KpiProductivityForZone> kpiProductivityForZoneViewDTO = new GenericViewDTO<KpiProductivityForZone>();
            TypeUnitKPI typeUnit = (TypeUnitKPI)Enum.Parse(typeof(TypeUnitKPI), typeCharts);
            context = new ContextViewDTO();
            context.MainFilter = new List<EntityFilter>();
            var arrEnum = Enum.GetValues(typeof(EntityFilterName));

            foreach (var item in arrEnum)
            {
                context.MainFilter.Add(new EntityFilter(item.ToString(), new FilterItem()));
            }

            context.MainFilter[Convert.ToInt16(EntityFilterName.WorkZone)].FilterValues.Clear();
            context.MainFilter[Convert.ToInt16(EntityFilterName.WorkZone)].AddFilterItem(new FilterItem("", zoneCode));

            switch (typeUnit)
            {
                case TypeUnitKPI.Cantidad:
                    kpiProductivityForZoneViewDTO = iDashboardMGR.GetKpiZoneQty(idWhs, context);
                    break;
                case TypeUnitKPI.Peso:
                    kpiProductivityForZoneViewDTO = iDashboardMGR.GetKpiZoneWeight(idWhs, context);
                    break;
                case TypeUnitKPI.Volumen:
                    kpiProductivityForZoneViewDTO = iDashboardMGR.GetKpiZoneVolumen(idWhs, context);
                    break;
            }

            var lstForZone = from i in kpiProductivityForZoneViewDTO.Entities
                             group i by i.WorkZoneName into g
                             select new KpiZoneModel()
                             {
                                 PercentageZone = decimal.Round((g.Sum(a => a.PercentageZone) / g.Count()), 2),
                                 WorkZoneName = g.First().WorkZoneName,
                                 IdWorkZone = g.First().IdWorkZone
                             };

            return lstForZone.ToList();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<KpiFillRateModel> KpiFillRate(int idOwn, int idWhs, string customerCode)
        {
            GenericViewDTO<KpiFillRate> kpiFillRateViewDTO = new GenericViewDTO<KpiFillRate>();
            DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;

            context = new ContextViewDTO();
            context.MainFilter = new List<EntityFilter>();
            var arrEnum = Enum.GetValues(typeof(EntityFilterName));

            foreach (var item in arrEnum)
            {
                context.MainFilter.Add(new EntityFilter(item.ToString(), new FilterItem()));
            }

            context.MainFilter[Convert.ToInt16(EntityFilterName.Customer)].FilterValues.Clear();
            context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();

            if (customerCode != "-1")
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Customer)].AddFilterItem(new FilterItem("", customerCode));
            }

            context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].AddFilterItem(new FilterItem("", idOwn.ToString()));

            kpiFillRateViewDTO = iDashboardMGR.GetKpiFillRate(idWhs, context);

            var lst = from i in kpiFillRateViewDTO.Entities.ToList()
                      group i by new { i.DateCreated.Month, i.DateCreated.Year } into g
                      select new KpiFillRateModel()
                      {
                          PercentSatisfaction = decimal.Round((g.Sum(i => i.PercentSatisfaction) / g.Count()), 2),
                          DateCreated = formatoFecha.GetMonthName(g.First().DateCreated.Month)
                      };

            return lst.ToList();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<KpiLeadTimeModel> KpiLeadTime(int idOwn, int idWhs, string typoLead, string typeCustomer)
        {
            GenericViewDTO<KpiLeadTime> kpiLeadTimeViewDTO = new GenericViewDTO<KpiLeadTime>();

            context = new ContextViewDTO();
            context.MainFilter = new List<EntityFilter>();
            var arrEnum = Enum.GetValues(typeof(EntityFilterName));

            foreach (var item in arrEnum)
            {
                context.MainFilter.Add(new EntityFilter(item.ToString(), new FilterItem()));
            }

            context.MainFilter[Convert.ToInt16(EntityFilterName.Customer)].FilterValues.Clear();
            context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();

            if (typeCustomer != "-1")
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Customer)].AddFilterItem(new FilterItem("", typeCustomer));
            }

            context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].AddFilterItem(new FilterItem("", idOwn.ToString()));

            if (typoLead == "D")
            {
                //typeTitulo = "Delta Dias";
                kpiLeadTimeViewDTO = iDashboardMGR.GetKpiLeadTime(idWhs, true, context);
            }
            else
            {
                //typeTitulo = "Delta Horas";
                kpiLeadTimeViewDTO = iDashboardMGR.GetKpiLeadTime(idWhs, false, context);
            }

            //Agrupa los valores del LeadTime 
            DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
            var lst = from a in kpiLeadTimeViewDTO.Entities.ToList()
                      group a by new { a.SpecialField1, a.DateCreated.Month, a.DateCreated.Year } into g
                      select new KpiLeadTimeModel()
                      {
                          Delta = Math.Round(((double)g.Sum(i => i.Delta) / (double)g.Count()), 2),
                          Month = formatoFecha.GetMonthName(g.First().DateCreated.Month),
                          CustomerName = g.First().CustomerName,
                          TypeCustomer = g.First().SpecialField1
                      };

            return lst.ToList();

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<Customer> loadCustomer(int idOwn)
        {
            GenericViewDTO<Customer> customerViewDTO = new GenericViewDTO<Customer>();

            if (Session[WMSTekSessions.Shared.CustomerList] != null && Session[WMSTekSessions.Shared.CustomerList].ToString() != string.Empty)
            {
                customerViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.Shared.CustomerList];
            }
            else
            {
                customerViewDTO = iWarehousingMGR.FindCustomerByOwner(context, idOwn);
                Session.Add(WMSTekSessions.Shared.CustomerList, customerViewDTO);
            }

            List<Customer> listCutomer = new List<Customer>();
            listCutomer.Add(new Customer { Code = "-1", Name = "(Todos)" });

            foreach (var cust in customerViewDTO.Entities.OrderBy(s => s.Name).ToList())
            {
                listCutomer.Add(cust);
            }

            return listCutomer;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public ListItemCollection loadTypeCustomer(int idOwn)
        {
            GenericViewDTO<Customer> customerViewDTO = new GenericViewDTO<Customer>();

            if (Session[WMSTekSessions.Shared.CustomerList] != null && Session[WMSTekSessions.Shared.CustomerList].ToString() != string.Empty)
            {
                customerViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.Shared.CustomerList];
            }
            else
            {
                customerViewDTO = iWarehousingMGR.FindCustomerByOwner(context, idOwn);
                Session.Add(WMSTekSessions.Shared.CustomerList, customerViewDTO);
            }

            //Si el Campo SpecialField1 viene null lo reemplazamos por el texto 'Otros'
            for (int i = 0; i < customerViewDTO.Entities.Count(); i++)
            {
                customerViewDTO.Entities[i].SpecialField1 = (string.IsNullOrEmpty(customerViewDTO.Entities[i].SpecialField1) ? "Otros" : customerViewDTO.Entities[i].SpecialField1);
            }

            var varLst = customerViewDTO.Entities.Select(s => s.SpecialField1).Distinct();

            ListItemCollection lstItem = new ListItemCollection();
            lstItem.Add(new ListItem( "Todos", "-1"));

            foreach (var item in varLst)
            {
                lstItem.Add(new ListItem(item));
            }

            return lstItem;
        }
    }


    public class KpiPickingModel
    {
        public decimal Qty { get; set; }
        public string UserWms { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
    }

    public class KpiZoneModel
    {
        public decimal PercentageZone { get; set; }
        public string WorkZoneName { get; set; }
        public int IdWorkZone { get; set; }
    }

    public class KpiFillRateModel
    {
        public decimal PercentSatisfaction { get; set; }
        public string DateCreated { get; set; }
    }

    public class KpiLeadTimeModel
    {
        public double Delta { get; set; }
        public string Month { get; set; }
        public string CustomerName { get; set; }
        public string TypeCustomer { get; set; }
    }
}
