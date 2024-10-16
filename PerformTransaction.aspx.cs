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
    public partial class Perform_Transaction : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;
        SqlDataReader dr;
        SqlTransaction transaction = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("login.aspx");
                }
                getAccountNumber();
            }
        }

        void getAccountNumber()
        {
            try
            {
                con = new SqlConnection(Common.GetConnectionString());
                cmd = new SqlCommand(@"Select AccountId, AccountNumber from Account where AccountId  != @AccountId", con);
                cmd.Parameters.AddWithValue("@AccountId", Session["userId"]);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);
                ddlPayeeAccountNumber.DataSource = dt;
                ddlPayeeAccountNumber.DataTextField = "AccountNumber";
                ddlPayeeAccountNumber.DataValueField = "AccountId";
                ddlPayeeAccountNumber.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert(' " + ex.Message + " ');</script>");
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (Session["userId"] != null)
            {
                con = new SqlConnection(Common.GetConnectionString());
                con.Open();
                try
                {
                    int r = 0;
                    Utils utils = new Utils();
                    int balanceAmount = utils.accountBalance(Convert.ToInt32(Session["userId"]));
                    if (Convert.ToInt32(txtAmount.Text.Trim()) <= balanceAmount)
                    {
                        transaction = con.BeginTransaction();
                        cmd = new SqlCommand(@"Insert into [Transaction](SenderAccountId,ReceiverAccountId,MobileNo,Amount,TransactionType, Remarks)
                                                values(@SenderAccountId, @ReceiverAccountId, @UserMobileNo, @Amount, @TransactionType, @Remarks)", con, transaction);
                        cmd.Parameters.AddWithValue("@SenderAccountId", Session["userId"]);
                        cmd.Parameters.AddWithValue("@ReceiverAccountId", ddlPayeeAccountNumber.SelectedValue);
                        cmd.Parameters.AddWithValue("@UserMobileNo", txtMobileNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                        cmd.Parameters.AddWithValue("TransactionType", "DR");
                        cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());
                        r = cmd.ExecuteNonQuery();

                        UpdateSenderAccountBalance(Convert.ToInt32(Session["userId"]), balanceAmount, Convert.ToInt32(txtAmount.Text.Trim()), con, transaction);

                        UpdateReceiverAccountBalance(Convert.ToInt32(ddlPayeeAccountNumber.SelectedValue), Convert.ToInt32(txtAmount.Text.Trim()), con, transaction);
                        transaction.Commit();
                        r = 1;
                        if (r > 0)
                        {
                            Response.Redirect("mydebits.aspx", false);

                        }
                        else
                        {
                            error.InnerText = "Invalid Input";
                        }
                    }
                    else
                    {
                        error.InnerText = "Invalid Input";
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<scrit>alert(' " + ex.Message + " ');</script>");
                    }
                }
                finally
                {
                    con.Close();
                }
            }
        }

        void UpdateSenderAccountBalance(int _senderId, int _dbAmount, int _amount, SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            try
            {
                if (_dbAmount >= _amount)
                {
                    _dbAmount -= _amount;
                    cmd = new SqlCommand("Update Account set Amount=@Amount where AccountId=@AccountId", sqlConnection, sqlTransaction);
                    cmd.Parameters.AddWithValue("@Amount", _dbAmount);
                    cmd.Parameters.AddWithValue("@AccountId", _senderId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert(' " + ex.Message + " ');</script>");
            }
        }

        void UpdateReceiverAccountBalance(int _receiverId, int _amount, SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {

            int _dbAmount = 0;

            cmd = new SqlCommand("Select Amount from Account where AccountId=@AccountId", sqlConnection, sqlTransaction);
            
            cmd.Parameters.AddWithValue("@AccountId", _receiverId);
            try
            {
                dr = cmd.ExecuteReader();
                while( dr.Read())
                {
                    _dbAmount = (int)dr["Amount"];
                    _dbAmount = _dbAmount + _amount;
                    cmd = new SqlCommand("Update Account set Amount=@Amount where AccountId=@AccountId", sqlConnection, sqlTransaction);
                    cmd.Parameters.AddWithValue("@Amount", _dbAmount);
                    cmd.Parameters.AddWithValue("@AccountId", _receiverId);
                    cmd.ExecuteNonQuery();
                }
                dr.Close();
            }
            catch(Exception ex)
            {
                Response.Write("<script>alert(' " + ex.Message + " ');</script>");
            }
        }
        
            
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("PerformTransaction.aspx");
        }
    }
}