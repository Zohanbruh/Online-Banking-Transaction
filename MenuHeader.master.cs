using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Online_Banking_Transaction
{
    public partial class MenuHeader : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                if (Session["userId"] != null)
                {
                    Utils utils = new Utils();
                    lblBalance.Text = utils.accountBalance(Convert.ToInt32(Session["userId"])).ToString();
                }
            }

        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("login.aspx");
        }
    }
}