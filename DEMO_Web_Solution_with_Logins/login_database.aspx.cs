using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DEMO_Web_Solution_with_Logins
{
    public partial class login_database : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // validate data present
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                lblMessage.Text = "Please enter both username and password.";
                return;
            }
            // read username and password from textboxes
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            //open database connection and check if username and password match any entry in the database
            //create database connection
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\SolutionDataBase.mdf;Integrated Security=True";

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                conn.Open();
                //create SQL command to check if username and password match any entry in the database
                string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                {
                    // Add parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    // Execute the command and get the count of matching records
                    int count = (int)cmd.ExecuteScalar();

                    // Check if count is greater than 0, which means a matching record was found
                    if (count > 0)
                    {
                        // Login successful
                        // set session variable to indicate user is logged in
                        Session["LoggedInUser"] = username;
                        // Redirect to index page
                        Response.Redirect("index.aspx");
                    }
                    // Count is 0, which means no matching record was found
                    else
                    {
                        // Login failed
                        lblMessage.Text = "Invalid username or password.";
                    }
                }
            }
        }
    }
}