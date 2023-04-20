using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.WebClient.Shared;
using System.Data;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Base;
using System.Web.Script.Services;
using System.Runtime.Serialization.Json;
using System.Web.Services;

namespace Binaria.WMSTek.WebClient.Administration.Configuration
{
    public partial class Map2DMgr : BasePage
    {
        #region "Declaración de Variables"
        //private GenericViewDTO<MapLayout> mapLayoutViewDTO = new GenericViewDTO<MapLayout>();
        private static GenericViewDTO<MapLayout> mapLayoutViewDTO = new GenericViewDTO<MapLayout>();
        private bool isValidViewDTO = false;

        #endregion

        #region "Eventos"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);
                InitializeTaskBar();
                InitializeFilter();
                

                if (!Page.IsPostBack)
                {
                    this.tabMap.HeaderText = this.lbltabMapa.Text;
                    this.tabCol.HeaderText = this.lbltabCol.Text;
                    this.tabColorMap.HeaderText = this.lblTabColorMap.Text;
                }
            }
            catch (Exception ex)
            {
                mapLayoutViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(mapLayoutViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal) Initialize();

            }
            catch (Exception ex)
            {
                mapLayoutViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(mapLayoutViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSession(false);
                PopulateData();
            }
            catch (Exception ex)
            {
                mapLayoutViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(mapLayoutViewDTO.Errors);
            }         
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveChanges();
            }
            catch (Exception ex)
            {
                mapLayoutViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(mapLayoutViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            if (ValidateSession(WMSTekSessions.Map2DConsult.MapLayoutConfig))
            {
                mapLayoutViewDTO = (GenericViewDTO<MapLayout>)Session[WMSTekSessions.Map2DConsult.MapLayoutConfig];
                isValidViewDTO = true;
            }
            else
                UpdateSession(false);

            // Si es un ViewDTO valido, carga los datos en pantalla
            if (isValidViewDTO && !this.Page.IsPostBack)
            {
                PopulateData();
            }
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnSaveClick += new EventHandler(btnSave_Click);

            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnSaveVisible = true;
        }

        private void InitializeFilter()
        {
            // Oculta el filtro
            Master.ucMainFilter.searchVisible = false;
        }

        /// <summary>
        /// Carga en sesion la lista de mapLayouts
        /// </summary>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(mapLayoutViewDTO.Errors);
                mapLayoutViewDTO.ClearError();
            }

            // Carga configuración actual del Mapa 2D
            mapLayoutViewDTO = iLayoutMGR.GetMapLayout(context);

            //si tiene registros entonces se cargaran los ddls con el valor que tiene el objeto
            if (!mapLayoutViewDTO.hasError() && mapLayoutViewDTO.Entities != null && mapLayoutViewDTO.Entities.Count > 0)
            {
                Session.Add(WMSTekSessions.Map2DConsult.MapLayoutConfig, mapLayoutViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(mapLayoutViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(mapLayoutViewDTO.Errors);
            }
        }

        private void PopulateData()
        {
            if (mapLayoutViewDTO.Entities != null && mapLayoutViewDTO.Entities.Count == 1)
            {
                // Map
                cpBackColor.Color = mapLayoutViewDTO.Entities[0].BackColor;
                txtMargin.Text = mapLayoutViewDTO.Entities[0].Margin.ToString();

                // Hangar
                cpHangarBackColor.Color = mapLayoutViewDTO.Entities[0].HangarBackColor;
                cpHangarBorderColor.Color = mapLayoutViewDTO.Entities[0].HangarBorderColor;
                txtHangarBorder.Text = mapLayoutViewDTO.Entities[0].HangarBorder.ToString();

                // Column
                cpColumnBorderColor.Color = mapLayoutViewDTO.Entities[0].ColumnBorderColor;
                txtColumnBorder.Text = mapLayoutViewDTO.Entities[0].ColumnBorder.ToString();
                cpColumnBackColorActive.Color = mapLayoutViewDTO.Entities[0].ColumnBackColorActive;
                cpColumnBorderColorActive.Color = mapLayoutViewDTO.Entities[0].ColumnBorderColorActive;
                cpColumnBackColorItem.Color = mapLayoutViewDTO.Entities[0].ColumnBackColorItem;
                cpColumnBackColor0.Color = mapLayoutViewDTO.Entities[0].ColumnBackColor[0];
                cpColumnBackColor1.Color = mapLayoutViewDTO.Entities[0].ColumnBackColor[1];
                cpColumnBackColor2.Color = mapLayoutViewDTO.Entities[0].ColumnBackColor[2];
                cpColumnBackColor3.Color = mapLayoutViewDTO.Entities[0].ColumnBackColor[3];
                cpColumnBackColor4.Color = mapLayoutViewDTO.Entities[0].ColumnBackColor[4];
                cpColumnBackColor5.Color = mapLayoutViewDTO.Entities[0].ColumnBackColor[5];

                // Column Details
                cpColumnDetailBackColor.Color = mapLayoutViewDTO.Entities[0].ColumnDetail.BackColor[0];
                txtColumnDetailBorder.Text = mapLayoutViewDTO.Entities[0].ColumnDetail.Border.ToString();
                cpColumnDetailBorderColor.Color = mapLayoutViewDTO.Entities[0].ColumnDetail.BorderColor;
                txtColumnDetailPadding.Text = mapLayoutViewDTO.Entities[0].ColumnDetail.Padding.ToString();
                txtColumnDetailMaxHeight.Text = mapLayoutViewDTO.Entities[0].ColumnDetail.MaxHeight.ToString();
                txtColumnDetailMinHeight.Text = mapLayoutViewDTO.Entities[0].ColumnDetail.MinHeight.ToString();
                txtColumnDetailMaxWidth.Text = mapLayoutViewDTO.Entities[0].ColumnDetail.MaxWidth.ToString();
                txtColumnDetailMinWidth.Text = mapLayoutViewDTO.Entities[0].ColumnDetail.MinWidth.ToString();

                // Location Details
                cpLocationBackColor1.Color = mapLayoutViewDTO.Entities[0].LocationDetail.BackColor[0];
                cpLocationBackColor2.Color = mapLayoutViewDTO.Entities[0].LocationDetail.BackColor[1];
                cpLocationBackColor3.Color = mapLayoutViewDTO.Entities[0].LocationDetail.BackColor[2];
                cpLocationBackColor4.Color = mapLayoutViewDTO.Entities[0].LocationDetail.BackColor[3];
                cpLocationBackColor5.Color = mapLayoutViewDTO.Entities[0].LocationDetail.BackColor[4];

                txtLocationDetailBorder.Text = mapLayoutViewDTO.Entities[0].LocationDetail.Border.ToString();
                cpLocationDetailBorderColor.Color = mapLayoutViewDTO.Entities[0].LocationDetail.BorderColor;
                txtLocationDetailPadding.Text = mapLayoutViewDTO.Entities[0].LocationDetail.Padding.ToString();
                txtLocationDetailMaxHeight.Text = mapLayoutViewDTO.Entities[0].LocationDetail.MaxHeight.ToString();
                txtLocationDetailMinHeight.Text = mapLayoutViewDTO.Entities[0].LocationDetail.MinHeight.ToString();

                cpHighColor.Color = mapLayoutViewDTO.Entities[0].HighUsedLocation;
                cpNormalHighColor.Color = mapLayoutViewDTO.Entities[0].NormalHighUsedLocation;
                cpNormalColor.Color = mapLayoutViewDTO.Entities[0].NormalUsedLocation;
                cpNormaLowColor.Color = mapLayoutViewDTO.Entities[0].NormalLowUsedLocation;
                cpLowColor.Color = mapLayoutViewDTO.Entities[0].LowUsedLocation;
            }
        }

        protected void SaveChanges()
        {
            MapLayout mapLayout = new MapLayout();

            // Map
            mapLayout.BackColor = cpBackColor.Color;
            mapLayout.Margin = Convert.ToUInt16(txtMargin.Text);
            mapLayout.MaxLevelCount = mapLayoutViewDTO.Entities[0].MaxLevelCount;

            // Hangar
            mapLayout.HangarBackColor = cpHangarBackColor.Color;
            mapLayout.HangarBorderColor = cpHangarBorderColor.Color;
            mapLayout.HangarBorder = Convert.ToUInt16(txtHangarBorder.Text);

            // Column
            mapLayout.ColumnBorderColor = cpColumnBorderColor.Color;
            mapLayout.ColumnBorder = Convert.ToUInt16(txtColumnBorder.Text);
            mapLayout.ColumnBackColorActive = cpColumnBackColorActive.Color;
            mapLayout.ColumnBorderColorActive = cpColumnBorderColorActive.Color;
            mapLayout.ColumnBackColorItem = cpColumnBackColorItem.Color;

            mapLayout.ColumnBackColor = new string[mapLayout.MaxLevelCount];
            mapLayout.ColumnBackColor[0] = cpColumnBackColor0.Color;
            mapLayout.ColumnBackColor[1] = cpColumnBackColor1.Color;
            mapLayout.ColumnBackColor[2] = cpColumnBackColor2.Color;
            mapLayout.ColumnBackColor[3] = cpColumnBackColor3.Color;
            mapLayout.ColumnBackColor[4] = cpColumnBackColor4.Color;
            mapLayout.ColumnBackColor[5] = cpColumnBackColor5.Color;

            // Column Details
            mapLayout.ColumnDetail = new MapAttributes();
            mapLayout.ColumnDetail.BackColor = new string[1];
            mapLayout.ColumnDetail.BackColor[0] = cpColumnDetailBackColor.Color;
            mapLayout.ColumnDetail.Border = Convert.ToUInt16(txtColumnDetailBorder.Text);
            mapLayout.ColumnDetail.BorderColor = cpColumnDetailBorderColor.Color;
            mapLayout.ColumnDetail.Padding = Convert.ToUInt16(txtColumnDetailPadding.Text);
            mapLayout.ColumnDetail.MaxHeight = Convert.ToUInt16(txtColumnDetailMaxHeight.Text);
            mapLayout.ColumnDetail.MinHeight = Convert.ToUInt16(txtColumnDetailMinHeight.Text);
            mapLayout.ColumnDetail.MaxWidth = Convert.ToUInt16(txtColumnDetailMaxWidth.Text);
            mapLayout.ColumnDetail.MinWidth = Convert.ToUInt16(txtColumnDetailMinWidth.Text);

            // Location Details
            mapLayout.LocationDetail = new MapAttributes();
            mapLayout.LocationDetail.BackColor = new string[mapLayout.MaxLevelCount - 1];
            mapLayout.LocationDetail.BackColor[0] = cpLocationBackColor1.Color;
            mapLayout.LocationDetail.BackColor[1] = cpLocationBackColor2.Color;
            mapLayout.LocationDetail.BackColor[2] = cpLocationBackColor3.Color;
            mapLayout.LocationDetail.BackColor[3] = cpLocationBackColor4.Color;
            mapLayout.LocationDetail.BackColor[4] = cpLocationBackColor5.Color;

            mapLayout.LocationDetail.Border = Convert.ToUInt16(txtLocationDetailBorder.Text);
            mapLayout.LocationDetail.BorderColor = cpLocationDetailBorderColor.Color;
            mapLayout.LocationDetail.Padding = Convert.ToUInt16(txtLocationDetailPadding.Text);
            mapLayout.LocationDetail.MaxHeight = Convert.ToUInt16(txtLocationDetailMaxHeight.Text);
            mapLayout.LocationDetail.MinHeight = Convert.ToUInt16(txtLocationDetailMinHeight.Text);

            mapLayout.HighUsedLocation = cpHighColor.Color;
            mapLayout.NormalHighUsedLocation = cpNormalHighColor.Color;
            mapLayout.NormalUsedLocation = cpNormalColor.Color;
            mapLayout.NormalLowUsedLocation = cpNormaLowColor.Color;
            mapLayout.LowUsedLocation = cpLowColor.Color;

            mapLayoutViewDTO = iLayoutMGR.MaintainMapLayout(CRUD.Update, mapLayout, context);

            if (mapLayoutViewDTO.hasError())
            {
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(mapLayoutViewDTO.MessageStatus.Message);
                UpdateSession(false);
                PopulateData();
            }

        }

        [WebMethod]
        public static void GetPreviewMap()
        {
           // int i;

            /*
            if (hangarViewDTO.Entities != null && hangarViewDTO.Entities.Count > 0 && mapLayoutViewDTO.Entities != null && mapLayoutViewDTO.Entities.Count > 0)
            {
                if (!String.IsNullOrEmpty(index))
                {
                    i = Convert.ToInt32(index) - 1;
                    mapLayoutViewDTO.Entities[0].Hangar = hangarViewDTO.Entities[i];
                }
            }*/

            //return mapLayoutViewDTO.Entities[0];
        }

        #endregion
    }
}