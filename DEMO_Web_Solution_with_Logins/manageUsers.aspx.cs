using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DEMO_Web_Solution_with_Logins
{
    public partial class manageUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {

            // validate data present
            if (!IsPostBack)
            {
            }
            else
            {
                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
                {
                    lblMessage.Text = "Please enter both username and password.";
                    return;
                }
                else
                {
                    // read username and password from textboxes
                    string username = txtUsername.Text;
                    string password = txtPassword.Text;
                    string surname = txtSurname.Text;
                    string givenName = txtGivenName.Text;


                    // Add new user to SolutionDataBase in the Users table using SQL connection and SQL command
                    //Create the connection string to connect to the database
                    string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\SolutionDataBase.mdf;Integrated Security=True";



                    //connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\SolutionDataBase.mdf;Integrated Security=True";


                    //Use the connection string to open a connection to the database file
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Create a SQL command to insert the new user into the Users table - use SQL parameters to prevent SQL injection
                        string query = "INSERT INTO Users (Username, Password, Surname, GivenName) VALUES (@Username, @Password, @Surname, @GivenName)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Username", username);
                            command.Parameters.AddWithValue("@Password", password);
                            command.Parameters.AddWithValue("@Surname", surname);
                            command.Parameters.AddWithValue("@GivenName", givenName);

                            // Open the connection, execute the command and check if the user was added successfully
                            try
                            {
                                connection.Open();
                                int result = command.ExecuteNonQuery();
                                if (result > 0)
                                {
                                    lblMessage.Text = "User added successfully.";
                                    lblMessage.ForeColor = System.Drawing.Color.Green;
                                }
                                else
                                {
                                    lblMessage.Text = "Error adding user.";
                                    lblMessage.ForeColor = System.Drawing.Color.Red;
                                }
                            }
                            catch (Exception ex)
                            {
                                lblMessage.Text = "Error: " + ex.Message;
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                            }
                        }

                    }
                }
            }
        }

        protected void btnComplexAddUser_Click(object sender, EventArgs e)
        {

        }
    }
}