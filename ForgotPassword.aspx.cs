using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Online_Banking_Transaction
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader reader;
        protected void Page_Load(object sender, EventArgs e)
        {
           if(!IsPostBack)
            {
                getUserSecurityQuestion();
            }
        }

        void getUserSecurityQuestion()
        {
            if (Session["forgotpassword"] != null)
            {
                con = new SqlConnection(Common.GetConnectionString());
                cmd = new SqlCommand(@"Select a.UserName, s.SecurityQuestionName, a.Answer from Account a
                                        inner join SecurityQuestion s on a.SecurityQuestionId = s.SecurityQuestionId
                                        where Username = @Username", con);
                cmd.Parameters.AddWithValue("@Username", Session["forgotpassword"]);
                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    bool isTrue = false;
                    while (reader.Read())
                    {
                        isTrue = true;
                        lblUserName.Text = reader["UserName"].ToString();
                        lblSecurityQuestion.Text = reader["SecurityQuestionName"].ToString();
                        hdnAnswer.Value = reader["Answer"].ToString();
                    }
                    if (!isTrue)
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

        protected void btnForgotPassword_Click(object sender, EventArgs e)
        {
            if(txtAnswer.Text.Trim() == hdnAnswer.Value)
            {
                Response.Redirect("ChangePassword.aspx");
            }
            else
            {
                error.InnerText = "Invalid input";
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}