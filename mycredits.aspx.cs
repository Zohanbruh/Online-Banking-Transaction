using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Online_Banking_Transaction
{
    public partial class mycredits : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("login.aspx");

                }
                getMyCredits();
            }
        }

        void getMyCredits()
        {
            try
            {
                con = new SqlConnection(Common.GetConnectionString());
                cmd = new SqlCommand(@"Select a.AccountNumber, a.UserName, t.Amount, t.Remarks from [Transaction] t
                                        inner join Account a on t.SenderAccountId = a.AccountId
                                        where t.ReceiverAccountId = @ReceiverAccountId", con);
                cmd.Parameters.AddWithValue("@ReceiverAccountId", Session["userId"]);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);
                gvMyCredits.DataSource = dt;
                gvMyCredits.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert(' " + ex.Message + " ');</script>");
            }
        }
    }
}