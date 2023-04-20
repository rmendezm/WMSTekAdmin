using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class StatusBarContent : System.Web.UI.UserControl
    {
        #region "Declaración de Variables"

        public event EventHandler pageChanged;
        // public event EventHandler pageSizeChanged;
        public event EventHandler pageFirst;
        public event EventHandler pagePrevious;
        public event EventHandler pageNext;
        public event EventHandler pageLast;
        public bool imgBtnFirst;
        public bool imgBtnPrevious;
        public bool imgBtnNext;
        public bool imgBtnpageLast;

        public bool DivPagerVisible
        {
            get { return this.divPager.Visible; }
            set { this.divPager.Visible = value; }
        }

        public int SelectedPage
        {
            get { return selectedPage; }
            set { selectedPage = value; }
        }

        // TODO: Implementar en Fase 3
        //public int PageSize
        //{
        //    get { return pageSize; }
        //    set { pageSize = value; }
        //}

        //private int pageSize
        //{
        //    get { return (int)(ViewState["pageSize"] ?? 0); }
        //    set { ViewState["pageSize"] = value; }
        //}

        private int selectedPage
        {
            get { return (int)(ViewState["selectedPage"] ?? 0); }
            set { ViewState["selectedPage"] = value; }
        }

        #endregion

        #region "Eventos"

        protected void ddlPagesSelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPage = ddlPages.SelectedIndex;

            if (pageChanged != null)
            {
                pageChanged(this, e);
            }
        }

        // TODO: Implementar en Fase 3
        //protected void ddlPageSizeSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    pageSize = Convert.ToInt16(ddlPages.SelectedValue);

        //    if (pageSizeChanged != null)
        //    {
        //        pageSizeChanged(this, e);
        //    }
        //}

        protected void btnFirst_Click(object sender, ImageClickEventArgs e)
        {
            if (pageFirst != null)
            {
                pageFirst(this, e);
            }
        }

        protected void btnPrevious_Click(object sender, ImageClickEventArgs e)
        {
            if (pagePrevious != null)
            {
                pagePrevious(this, e);
            }
        }

        protected void btnNext_Click(object sender, ImageClickEventArgs e)
        {
            if (pageNext != null)
            {
                pageNext(this, e);
            }
        }

        protected void btnLast_Click(object sender, ImageClickEventArgs e)
        {
            if (pageLast != null)
            {
                pageLast(this, e);
            }
        }

        #endregion

        #region "Métodos"

        public void HideRecordInfo()
        {
            lblRecordCount.Text = string.Empty;
            lblTotalRecordCount.Text = string.Empty;

            this.divPager.Visible = false;
        }

        public void ShowRecordInfo(int recordCount, int pageSize, int pageCount, int currentPage, bool allowPaging)
        {
            int pageNumber;
            int firstItem;
            int lastItem;

            if (recordCount > 0)
            {
                // Paginador
                ddlPages.Items.Clear();

                if (pageCount > 1)
                {
                    this.divPager.Visible = true;

                    for (int i = 0; i < pageCount; i++)
                    {
                        pageNumber = i + 1;
                        ListItem lstItem = new ListItem(pageNumber.ToString());

                        if (i == currentPage) lstItem.Selected = true;

                        ddlPages.Items.Add(lstItem);
                    }

                    this.lblPageCount.Text = pageCount.ToString();
                }
                else
                {
                    this.divPager.Visible = false;
                }

                // Total de registros ("Items 10-18 de 40")
                if (allowPaging)
                {
                    firstItem = (pageSize * currentPage) + 1;
                    lastItem = firstItem + pageSize - 1;

                    if (lastItem > recordCount) lastItem = recordCount;
                }
                else
                {
                    firstItem = 1;
                    lastItem = recordCount;
                }


                lblRecordCount.Text = lblItem.Text + (firstItem).ToString() + "-" + (lastItem).ToString() + lblDe.Text;
                lblTotalRecordCount.Text = recordCount.ToString();

                // Controles de página
                if (this.divPager.Visible)
                {
                    if (currentPage == 0)
                    {
                        btnFirst.Enabled = false;
                        btnFirst.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";

                        btnPrevious.Enabled = false;
                        btnPrevious.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    }
                    else
                    {
                        btnFirst.Enabled = true;
                        btnFirst.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";

                        btnPrevious.Enabled = true;
                        btnPrevious.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    }

                    if (currentPage == pageCount - 1)
                    {
                        btnNext.Enabled = false;
                        btnNext.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";

                        btnLast.Enabled = false;
                        btnLast.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                    }
                    else
                    {
                        btnNext.Enabled = true;
                        btnNext.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";

                        btnLast.Enabled = true;
                        btnLast.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    }
                }
            }
            else
            {
                lblRecordCount.Text = string.Empty;
                lblTotalRecordCount.Text = string.Empty;

                this.divPager.Visible = false;
            }
        }

        public void ClearStatus()
        {
            lblStatusBar.Text = string.Empty;
        }

        public void ShowMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "successMessage", "successMessage('" + message + "');", true);
            }
        }

        public void ShowWarning(string message, int milisecondsTimeout = 3000)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "warningMessage", "warningMessage('" + message + "', " + milisecondsTimeout + ");", true);
            }
        }

        public void ShowError(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "errorMessage", "errorMessage('" + message + "');", true);
            }
        }

        #endregion


    }
}
