using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Binaria.WMSTek.Framework.Entities.Base;
using System.Web.UI.WebControls;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class LookUpFilterContent : UserControl
    {
        public event EventHandler BtnSearchClick;

        private List<FilterItem> filterItems;

        public List<FilterItem> FilterItems
        {
            get { return filterItems; }
            set { filterItems = value; }
        }

        public string FilterCode
        {
            get { return rblSearchCriteria.Items[0].Text; }
            set { rblSearchCriteria.Items[0].Text = value; }
        }

        public string FilterDescription
        {
            get { return rblSearchCriteria.Items[1].Text; }
            set { rblSearchCriteria.Items[1].Text = value; }
        }

        public int WidthTxtSearchValue 
        {
            set { this.txtSearchValue.Width = value; }        
        }

        protected void btnSearch_Click(object sender, ImageClickEventArgs e)
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                // Setea el filtro ingresado
                if (rblSearchCriteria.Items[0].Selected == true)
                {
                    this.filterItems[0].Value = "%" + txtSearchValue.Text.Trim() + "%";
                    this.filterItems[0].Selected = true;
                    this.filterItems[1].Selected = false;
                }
                else
                {
                    this.filterItems[1].Value = "%" + txtSearchValue.Text.Trim() + "%";
                    this.filterItems[0].Selected = false;
                    this.filterItems[1].Selected = true;
                }

                // Dispara el evento que sera capturado por las paginas que implementen este control
                OnBtnSearchClick(e);
            }
        }

        protected void OnBtnSearchClick(EventArgs e)
        {
            if (BtnSearchClick != null)
            {
                BtnSearchClick(this, e);
            }
        }

        public void Clear()
        {
            this.txtSearchValue.Text = string.Empty;
        }

        public void Initialize()
        {
            this.FilterItems = new List<FilterItem>();

            this.FilterItems.Add(new FilterItem("%%"));
            this.FilterItems.Add(new FilterItem("%%"));
        }

        public void LoadCurrentFilter(List<FilterItem> filterItems)
        {
            this.rblSearchCriteria.SelectedIndex = 0;

            if (filterItems[0].Selected)
            {
                this.rblSearchCriteria.SelectedIndex = 0;
                this.txtSearchValue.Text = filterItems[0].Value.Replace("%", string.Empty);
            }
            else
            {
                if (filterItems[1].Selected)
                {
                    this.rblSearchCriteria.SelectedIndex = 1;
                    this.txtSearchValue.Text = filterItems[1].Value.Replace("%", string.Empty);
                }
            }
        }

    }
}
