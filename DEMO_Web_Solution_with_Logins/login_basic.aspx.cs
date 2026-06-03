using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DEMO_Web_Solution_with_Logins
{
    /// <summary>
    /// Basic authentication implementation using CSV file storage.
    /// WARNING: This is a demonstration implementation with significant security vulnerabilities.
    /// NOT suitable for production use.
    /// 
    /// Known Security Issues:
    /// - Plaintext password storage in CSV file
    /// - No protection against brute-force attacks
    /// - No account lockout mechanism
    /// - Vulnerable to timing attacks (early termination on match)
    /// - No HTTPS enforcement
    /// - File I/O performed on every login attempt (performance/DOS concern)
    /// - No input sanitization beyond null/empty checks
    /// - Generic error messages don't distinguish between invalid username vs password
    /// 
    /// Mitigations Implemented:
    /// - Session regeneration after authentication (prevents session fixation)
    /// </summary>
    public partial class login_basic : System.Web.UI.Page
    {
        /// <summary>
        /// Page initialization event handler.
        /// Currently no initialization logic required for basic authentication flow.
        /// </summary>
        /// <param name="sender">Event source (Page instance)</param>
        /// <param name="e">Event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // No pre-render logic required for basic login form
        }

        /// <summary>
        /// Authenticates user credentials against CSV-stored plaintext values.
        /// 
        /// Authentication Flow:
        /// 1. Validates non-empty input
        /// 2. Reads entire CSV credential file into memory
        /// 3. Performs linear search with string comparison
        /// 4. On match: Regenerates session ID, establishes session, and redirects to index
        /// 5. On failure: Displays generic error message
        /// 
        /// Performance Characteristics:
        /// - O(n) time complexity where n = number of credential entries
        /// - File I/O on every authentication attempt (no caching)
        /// - No pagination or chunked reading (potential memory issue with large files)
        /// 
        /// Security Considerations:
        /// - Credentials transmitted and compared in plaintext
        /// - CSV parsing vulnerable to injection if file is writable by untrusted source
        /// - No rate limiting or CAPTCHA to prevent automated attacks
        /// - No session regeneration implemented to mitigate session fixation attacks
        /// - No audit logging of authentication attempts
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

            // Capture user input; no sanitization or normalisation applied
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // ========================================
            // Credential Store Access
            // ========================================
            // Load entire CSV file into memory
            // Server.MapPath resolves virtual path to physical file system path
            // Synchronous file I/O blocks request thread during read operation
            string[] lines = System.IO.File.ReadAllLines(Server.MapPath("~/loginDetails.csv"));

            // ========================================
            // Credential Verification
            // ========================================
            // Linear search through credential entries
            // Vulnerable to timing attacks (early exit on match reveals successful username)
            foreach (string line in lines)
            {
                // Parse CSV entry; assumes comma delimiter without escaping support
                string[] parts = line.Split(',');

                // Validate entry structure (expect exactly 2 fields: username,password)
                if (parts.Length == 2)
                {
                    // Perform exact string comparison (case-sensitive, no trimming)
                    // Uses default string equality (not constant-time comparison)
                    if (parts[0] == username && parts[1] == password)
                    {
                        // ========================================
                        // Authentication Success Path
                        // ========================================
                        // Establish session with username identifier 
                        Session["LoggedInUser"] = username;
                        // Redirect back to index page; session variable can be used to display user-specific content
                        Response.Redirect("index.aspx");
                        return; // Early exit prevents further credential checking
                    }
                }
            }

            // ========================================
            // Authentication Failure Path
            // ========================================
            // Generic error message; doesn't distinguish between invalid username vs password
            // This prevents username enumeration but provides no lockout mechanism
            lblMessage.Text = "Invalid username or password.";
        }

    }
}