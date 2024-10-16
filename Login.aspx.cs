using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Online_Banking_Transaction
{
    public partial class Login : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader reader;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx");
        }

        protected void lbForgotPassword_Click(object sender, EventArgs e)
        {
            if(txtUserName.Text.Trim() == string.Empty)
            {
                error.InnerText = "Invalid input";
                txtUserName.Focus();

            }
            else
            {
                Session["forgotpassword"] = txtUserName.Text.Trim();
                Response.Redirect("forgotPassword.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            con = new SqlConnection(Common.GetConnectionString());
            cmd = new SqlCommand(@"Select * from Account where UserName=@UserName and Password=@Password", con);
            cmd.Parameters.AddWithValue("@UserName", txtUserName.Text.Trim());
            cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                bool isTrue = false;
                while(reader.Read())
                {
                    isTrue = true;
                    Session["userId"] = reader["AccountId"].ToString();
                }
                if (isTrue)
                {
                    Response.Redirect("PerformTransaction.aspx", false);
                }
                else
                {
                    error.InnerText = "Invalid input";
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error - " + ex.Message + " ');</script>");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
        }
    }
}