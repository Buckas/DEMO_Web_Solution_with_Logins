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
            bool success = InsertUser(
                    txtGivenName.Text,
                    txtSurname.Text,
                    txtUsername.Text,
                    txtPassword.Text,
                    txtEmail.Text,
                    true);

            if (success)
            {
                lblMessage.Text = "User created successfully";
            }
            else
            {
                lblMessage.Text = "Username already exists";
            }

        }

        public bool InsertUser(
    string givenName,
    string surname,
    string username,
    string plainTextPassword,
    string email,
    bool isActive)
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\SolutionDataBase.mdf;Integrated Security=True";

            // ✅ Step 1: Generate salt
            byte[] salt = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // ✅ Step 2: Generate password hash (64 bytes)
            byte[] passwordHash;
            using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(
                plainTextPassword,
                salt,
                100000,
                System.Security.Cryptography.HashAlgorithmName.SHA256))
            {
                passwordHash = pbkdf2.GetBytes(64);
            }

            string sql = @"
        INSERT INTO UsersComplexVersion
        (GivenName, Surname, Username, PasswordHash, Salt, Email, IsActive, FailedLoginAttempts)
        VALUES
        (@GivenName, @Surname, @Username, @PasswordHash, @Salt, @Email, @IsActive, @FailedLoginAttempts)";

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
            {
                // ✅ Parameters
                cmd.Parameters.Add("@GivenName", System.Data.SqlDbType.NVarChar, 100).Value = givenName;
                cmd.Parameters.Add("@Surname", System.Data.SqlDbType.NVarChar, 100).Value = surname;
                cmd.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar, 100).Value = username;

                cmd.Parameters.Add("@PasswordHash", System.Data.SqlDbType.VarBinary, 64).Value = passwordHash;
                cmd.Parameters.Add("@Salt", System.Data.SqlDbType.VarBinary, 32).Value = salt;

                if (string.IsNullOrWhiteSpace(email))
                    cmd.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 255).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 255).Value = email;

                cmd.Parameters.Add("@IsActive", System.Data.SqlDbType.Bit).Value = isActive;
                cmd.Parameters.Add("@FailedLoginAttempts", System.Data.SqlDbType.Int).Value = 0;

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected == 1;
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    // ✅ Handle duplicate username (UNIQUE constraint)
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        // Username already exists
                        return false;
                    }

                    throw; // rethrow anything else
                }
            }
        }
    }
}