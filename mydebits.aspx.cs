using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Online_Banking_Transaction
{
    public partial class mydebits : System.Web.UI.Page
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
                getMyDebits();
            }
        }

        void getMyDebits()
        {
            try
            {
                con = new SqlConnection(Common.GetConnectionString());
                cmd = new SqlCommand(@"Select a.AccountNumber, a.UserName, t.Amount, t.Remarks from [Transaction] t
                                        inner join Account a on t.ReceiverAccountId = a.AccountId
                                        where t.SenderAccountId = @SenderAccountId", con);
                cmd.Parameters.AddWithValue("@SenderAccountId", Session["userId"]);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);
                gvMyDebits.DataSource = dt;
                gvMyDebits.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert(' " + ex.Message + " ');</script>");
            }
        }
    }
}