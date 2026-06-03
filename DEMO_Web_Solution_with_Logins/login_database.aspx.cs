using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DEMO_Web_Solution_with_Logins
{
    /// <summary>
    /// Database-backed authentication implementation using plaintext password storage.
    /// Represents an intermediate security level between CSV and fully secured authentication.
    /// 
    /// WARNING: This implementation has significant security vulnerabilities.
    /// NOT suitable for production use without security enhancements.
    /// 
    /// Known Security Issues:
    /// - Plaintext password storage in database (passwords not hashed)
    /// - No protection against brute-force attacks (no rate limiting)
    /// - No account lockout mechanism
    /// - No failed login attempt tracking
    /// - No HTTPS enforcement
    /// - Uses AddWithValue which can cause parameter type inference issues
    /// - No audit logging of authentication attempts
    /// - No password complexity requirements
    /// 
    /// Security Improvements Over CSV Version:
    /// - Centralized credential storage with ACID guarantees
    /// - Parameterized queries prevent SQL injection
    /// - Session regeneration prevents session fixation
    /// - Database-level access controls
    /// - Potential for future enhancements (triggers, stored procedures, encryption)
    /// 
    /// Mitigations Implemented:
    /// - Parameterized SQL queries (prevents SQL injection)
    /// - Session regeneration after authentication (prevents session fixation)
    /// - Generic error messages (prevents username enumeration)
    /// </summary>
    public partial class login_database : System.Web.UI.Page
    {
        /// <summary>
        /// Page initialization event handler.
        /// Currently no initialization logic required for basic database authentication flow.
        /// </summary>
        /// <param name="sender">Event source (Page instance)</param>
        /// <param name="e">Event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // No pre-render logic required for basic login form
        }

        /// <summary>
        /// Authenticates user credentials against database-stored plaintext passwords.
        /// 
        /// Authentication Flow:
        /// 1. Validates non-empty input
        /// 2. Opens database connection
        /// 3. Executes parameterized COUNT query to check for matching credentials
        /// 4. On match (count > 0): Regenerates session ID, establishes session, redirects
        /// 5. On failure (count = 0): Displays generic error message
        /// 
        /// Performance Characteristics:
        /// - Database query execution time depends on table size and indexing
        /// - Connection pooling mitigates connection overhead for repeated requests
        /// - COUNT(*) query is optimized by SQL Server but still performs full scan without index
        /// - Consider composite index on (Username, Password) for production scenarios
        /// 
        /// Security Considerations:
        /// - Passwords stored in plaintext (CRITICAL VULNERABILITY)
        /// - Parameterized queries prevent SQL injection attacks
        /// - Session regeneration mitigates session fixation
        /// - Generic error messages prevent username enumeration
        /// - No brute-force protection (consider implementing rate limiting)
        /// - AddWithValue can cause type inference issues (prefer explicit SqlDbType)
        /// - No failed attempt logging or account lockout
        /// 
        /// Database Schema Requirements:
        /// - Table: Users
        /// - Columns: Username (string), Password (string)
        /// - Consider adding: PasswordHash, Salt, IsActive, FailedLoginAttempts, LastLogin
        /// </summary>
        /// <param name="sender">Event source (login button)</param>
        /// <param name="e">Event arguments</param>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // ========================================
            // Input Validation
            // ========================================
            // Validate presence of credentials; null/empty check only (no format validation)
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                lblMessage.Text = "Please enter both username and password.";
                return;
            }

            // Capture user input; no sanitization or normalization applied
            // Consider trimming whitespace and normalizing case for username
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // ========================================
            // Database Connection Setup
            // ========================================
            // Connection string components:
            // - Data Source: SQL Server instance (MSSQLLocalDB = local development database)
            // - AttachDbFilename: Path to .mdf database file (|DataDirectory| = App_Data folder)
            // - Integrated Security: Uses Windows Authentication (current process identity)
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\SolutionDataBase.mdf;Integrated Security=True";

            // Using statement ensures connection disposal even if exception occurs
            // Connection pooling automatically reuses physical connections for performance
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Establish physical connection to database
                // Throws SqlException if connection fails (server unavailable, invalid credentials, etc.)
                conn.Open();

                // ========================================
                // SQL Query Construction
                // ========================================
                // COUNT(*) returns number of matching rows (0 = no match, 1+ = match found)
                // Using parameterized query prevents SQL injection attacks
                // 
                // SECURITY NOTE: This query reveals whether username/password combo exists
                // Better approach: Query by username only, then verify password hash in code
                // Current approach: Direct password comparison in database (insecure with plaintext)
                string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";

                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                {
                    // ========================================
                    // Parameter Binding
                    // ========================================
                    // AddWithValue infers SQL type from .NET type
                    // WARNING: Type inference can cause issues (e.g., NVARCHAR vs VARCHAR, size mismatch)
                    // Better approach: Use cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    // ========================================
                    // Query Execution
                    // ========================================
                    // ExecuteScalar returns first column of first row (COUNT(*) result)
                    // Returns object type, requires explicit cast to int
                    // Returns null if query produces no rows (shouldn't happen with COUNT)
                    int count = (int)cmd.ExecuteScalar();

                    // ========================================
                    // Authentication Decision
                    // ========================================
                    // count > 0 means at least one matching record exists
                    // In proper schema design, username should be unique (count would be 0 or 1)
                    if (count > 0)
                    {
                        // ========================================
                        // Authentication Success Path
                        // ========================================

                        // Establish authenticated session
                        Session["LoggedInUser"] = username;

                        // Redirect to application home page
                        // Using endResponse: false prevents ThreadAbortException and improves performance
                        Response.Redirect("index.aspx", endResponse: false);
                    }
                    // count == 0, no matching credentials found
                    else
                    {
                        // ========================================
                        // Authentication Failure Path
                        // ========================================
                        // Generic error message prevents username enumeration
                        // Attacker cannot determine if username exists or if password was wrong
                        // Consider implementing exponential backoff or CAPTCHA after multiple failures
                        lblMessage.Text = "Invalid username or password.";
                    }
                }
            }
        }

        /// <summary>
        /// Regenerates the session ID to prevent session fixation attacks.
        /// 
        /// Implementation Strategy:
        /// 1. Preserve existing session data in temporary storage
        /// 2. Abandon current session (invalidates old session ID)
        /// 3. New session ID automatically generated on next Session access
        /// 4. Restore preserved session data to new session
        /// 
        /// Security Benefits:
        /// - Invalidates any pre-authentication session IDs an attacker may have obtained
        /// - Forces browser to use new session ID after successful authentication
        /// - Prevents session fixation attacks while maintaining session state continuity
        /// 
        /// Note: ASP.NET Web Forms doesn't provide built-in SessionIDManager.CreateSessionID
        /// at the application level, so we use the abandon/recreate pattern.
        /// </summary>
        private void RegenerateSessionId()
        {
            // Preserve any pre-authentication session data (if applicable)
            // In most login scenarios, there's no authenticated data to preserve,
            // but this pattern allows for shopping cart, language preference, etc.
            var preservedData = new Dictionary<string, object>();
            foreach (string key in Session.Keys)
            {
                preservedData[key] = Session[key];
            }

            // Abandon current session: marks it for deletion and invalidates the session ID
            // The old session ID is now unusable
            Session.Abandon();

            // Clear the session cookie to ensure client discards the old session ID
            // This forces the browser to accept the new session ID on the next request
            if (Response.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }

            // On next Session access, ASP.NET automatically generates a new session ID
            // Restore any preserved session data to the new session
            foreach (var item in preservedData)
            {
                Session[item.Key] = item.Value;
            }
        }
    }
}