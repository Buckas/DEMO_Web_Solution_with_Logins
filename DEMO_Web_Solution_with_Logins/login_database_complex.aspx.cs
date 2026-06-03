using System; 
using System.Collections.Generic; 
using System.Data; // For working with data types like SqlDbType
using System.Data.SqlClient; // For connecting to SQL Server databases
using System.Web; 
using System.Web.UI; 
using System.Web.UI.WebControls;
using System.Web.SessionState; // For managing session variables

namespace DEMO_Web_Solution_with_Logins
{
    /// <summary>
    /// LOGIN PAGE - COMPLEX VERSION WITH SECURITY FEATURES
    /// 
    /// This page handles user login with advanced security features including:
    /// - Password hashing (passwords are never stored in plain text)
    /// - Salt-based encryption (makes each password hash unique even if two users have same password)
    /// - Account lockout after failed attempts (prevents brute force attacks)
    /// - Constant-time password comparison (prevents timing attacks)
    /// </summary>
    public partial class login_database_complex : System.Web.UI.Page
    {
        /// <summary>
        /// PAGE LOAD EVENT
        /// 
        /// This method runs automatically when the page first loads in the user's browser.
        /// 
        /// BEGINNER NOTE: In ASP.NET Web Forms, this is like the "starting point" for page logic.
        /// Right now it's empty because we don't need to do anything when the page first loads.
        /// All our logic happens when the user clicks the Login button.
        /// </summary>
        /// <param name="sender">The object that triggered this event (the page itself)</param>
        /// <param name="e">Additional information about the event (not used here)</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Nothing needs to happen when page loads
            // The user will fill in username/password and click the login button
        }

        /// <summary>
        /// LOGIN BUTTON CLICK EVENT - THE MAIN LOGIN LOGIC
        /// 
        /// This method runs when the user clicks the "Login" button on the web page.
        /// 
        /// HOW IT WORKS (Step-by-step):
        /// 1. Validate that username and password were entered
        /// 2. Connect to the database and look up the user by username
        /// 3. Check if the user account exists
        /// 4. Check if the account is active (not locked)
        /// 5. Hash the password they entered and compare it to the stored hash
        /// 6. If passwords match: Log them in, reset failed attempts, update last login time
        /// 7. If passwords don't match: Increment failed attempts, maybe lock account
        /// 
        /// SECURITY FEATURES:
        /// - Passwords are hashed (encrypted one-way) and never stored as plain text
        /// - Each password uses a unique "salt" to make hashing more secure
        /// - After 3 failed attempts, the account locks automatically
        /// - Uses parameterized SQL queries to prevent SQL injection attacks
        /// - Uses constant-time comparison to prevent timing attacks
        /// </summary>
        /// <param name="sender">The object that triggered this event (the login button)</param>
        /// <param name="e">Additional information about the event</param>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // ========================================
            // STEP 1: VALIDATE INPUT
            // ========================================
            // Check if the user left the username or password textbox empty
            // BEGINNER NOTE: string.IsNullOrEmpty() returns true if the text is null or ""
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                // Show an error message on the page if either field is empty
                lblMessage.Text = "Please enter both username and password.";
                // Exit this method early - don't continue with login process
                return;
            }

            // ========================================
            // STEP 2: GET USERNAME AND PASSWORD
            // ========================================
            // Read the text that the user typed into the textboxes
            // Store them in variables so we can use them later
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // ========================================
            // STEP 3: CONNECT TO DATABASE
            // ========================================
            // Create a connection string - this tells the program where the database is and how to connect
            // BEGINNER NOTE: This connection string points to a local SQL Server database file
            // - Data Source: Which SQL Server to use (MSSQLLocalDB is a local development database)
            // - AttachDbFilename: Where the database file (.mdf) is located
            // - |DataDirectory|: A placeholder that points to the App_Data folder
            // - Integrated Security: Use Windows authentication (current user's login)
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\SolutionDataBase.mdf;Integrated Security=True";

            // Create a connection object using the connection string
            // BEGINNER NOTE: The "using" statement ensures the connection closes automatically when done
            // This is important because database connections are limited resources
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Open the connection to the database
                // Now we can send commands to the database
                conn.Open();

                // ========================================
                // STEP 4: CREATE SQL QUERY
                // ========================================
                // Create a SQL SELECT statement to find the user in the database
                // BEGINNER NOTE: This query retrieves user information from the UsersComplexVersion table
                // - SELECT: Gets specific columns (fields) from the database
                // - WHERE: Filters results to only get the row where Username matches
                // - @Username: A parameter placeholder (safer than putting the username directly in the query)
                //   This prevents SQL injection attacks (a common security vulnerability)
                string sql = "SELECT Username, PasswordHash, Salt, IsActive, FailedLoginAttempts, GivenName,Surname, Email FROM UsersComplexVersion WHERE Username = @Username";


                // Create a SQL command object that will execute our query
                // BEGINNER NOTE: SqlCommand represents a SQL statement to execute against the database
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // ========================================
                    // STEP 5: ADD PARAMETERS SECURELY
                    // ========================================
                    // Replace @Username in the query with the actual username the user typed
                    // BEGINNER NOTE: This is the SAFE way to include user input in SQL queries
                    // Using parameters prevents SQL injection attacks where hackers try to inject malicious SQL code
                    // SqlDbType.NVarChar: The data type (text)
                    // 100: Maximum length of the username (100 characters)
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username;

                    // ========================================
                    // STEP 6: EXECUTE QUERY AND READ RESULTS
                    // ========================================
                    // ExecuteReader() runs the query and gives us a "reader" to access the results
                    // BEGINNER NOTE: SqlDataReader lets us read the rows returned by the query one at a time
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // ========================================
                        // STEP 7: CHECK IF USER EXISTS
                        // ========================================
                        // Try to read the first (and only) row from the results
                        // BEGINNER NOTE: reader.Read() returns true if there's a row to read, false if no rows
                        // If it returns false, that means no user with that username was found
                        if (!reader.Read())
                        {
                            // No user found in database with this username
                            // SECURITY NOTE: We say "Invalid username or password" instead of "Username not found"
                            // This prevents attackers from figuring out which usernames exist in the system
                            lblMessage.Text = "Invalid username or password.";
                            return; // Exit the method - login failed
                        }

                        // ========================================
                        // STEP 8: EXTRACT USER DATA FROM DATABASE
                        // ========================================
                        string dbUsername = reader["Username"].ToString();
                        byte[] passwordHash = (byte[])reader["PasswordHash"];
                        byte[] salt = (byte[])reader["Salt"];  // Random data that makes each password hash unique
                        bool isActive = (bool)reader["IsActive"];
                        int failedAttempts = (int)reader["FailedLoginAttempts"];
                        string givenName = reader["GivenName"].ToString();
                        string surname = reader["Surname"].ToString();
                        string email = reader["Email"] as string;  // Handles NULL safely

                        // Close reader before executing more commands
                        reader.Close();

                        // ========================================
                        // STEP 9: CHECK IF ACCOUNT IS LOCKED
                        // ========================================
                        if (!isActive)
                        {
                            lblMessage.Text = "Account is locked. Please contact an administrator.";
                            return;
                        }

                        // ========================================
                        // STEP 10: VERIFY PASSWORD
                        // ========================================
                        // Take the password the user typed and hash it using the same salt from the database
                        // BEGINNER NOTE: We never store plain text passwords in the database
                        // Instead we store a "hash" (one-way encryption) of the password
                        // To check if passwords match, we hash the input password and compare the hashes
                        // We MUST use the same salt that was used when originally hashing the password
                        byte[] hashedInputPassword = HashPassword(password, salt);

                        // ========================================
                        // STEP 12: COMPARE PASSWORD HASHES
                        // ========================================
                        // Check if the hash of the password user entered matches the hash stored in database
                        // BEGINNER NOTE: CompareHashes() is a special method that compares in "constant time"
                        // This prevents timing attacks where hackers measure how long comparisons take
                        if (CompareHashes(hashedInputPassword, passwordHash))
                        {
                            // ========================================
                            // PASSWORD MATCHES - SUCCESSFUL LOGIN!
                            // ========================================

                            // ========================================
                            // STEP 13: RESET FAILED ATTEMPTS AND UPDATE LAST LOGIN
                            // ========================================
                            // Update the database to:
                            // 1. Reset FailedLoginAttempts to 0 (successful login clears the counter)
                            // 2. Set LastLogin to current date/time (track when user logged in)
                            string updateSql = "UPDATE UsersComplexVersion SET FailedLoginAttempts = 0, LastLogin = @LastLogin WHERE Username = @Username";

                            // Create and execute the UPDATE command
                            using (SqlCommand updateCmd = new SqlCommand(updateSql, conn))
                            {
                                // Add parameter for current date/time
                                // BEGINNER NOTE: DateTime.Now gets the current date and time from the computer
                                updateCmd.Parameters.Add("@LastLogin", SqlDbType.DateTime).Value = DateTime.Now;

                                // Add parameter for username
                                updateCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username;

                                // ExecuteNonQuery() runs an UPDATE, INSERT, or DELETE query
                                // BEGINNER NOTE: "NonQuery" means it doesn't return data rows (unlike SELECT)
                                updateCmd.ExecuteNonQuery();
                            }

                            // ========================================
                            // STEP 14: CREATE SESSION VARIABLES
                            // ========================================
                            // Store user information in Session so it's available across all pages
                            // BEGINNER NOTE: Session is like a temporary storage area for each user
                            // It lasts until the user closes their browser or logs out
                            // We can access these values from any page while user is logged in

                            // Store user data in session
                            Session["Username"] = dbUsername;
                            Session["GivenName"] = givenName;
                            Session["Surname"] = surname;
                            Session["Email"] = email;
                            Session["IsLoggedIn"] = true;
                            Session["LoggedInUser"] = dbUsername;


                            // ========================================
                            // STEP 15: REDIRECT TO HOME PAGE
                            // ========================================
                            // Redirect to index page
                            // BEGINNER NOTE: Response.Redirect() loads a different page - like clicking a link
                            // Using endResponse: false prevents ThreadAbortException (much faster and safer)
                            // We must allow the request to complete naturally so the new session cookie reaches the browser
                            Response.Redirect("index.aspx", endResponse: false);
                        }
                        else
                        {
                            // ========================================
                            // PASSWORD DOES NOT MATCH - FAILED LOGIN
                            // ========================================

                            // Increment failed attempts counter
                            // BEGINNER NOTE: failedAttempts++ is shorthand for failedAttempts = failedAttempts + 1
                            failedAttempts++;

                            // Check if account should be locked (3 or more failed attempts)
                            if (failedAttempts >= 3)
                            {
                                // Lock the account by setting IsActive = 0 and saving failed attempts count
                                string lockSql = "UPDATE UsersComplexVersion SET FailedLoginAttempts = @FailedAttempts, IsActive = 0 WHERE Username = @Username";

                                using (SqlCommand lockCmd = new SqlCommand(lockSql, conn))
                                {
                                    lockCmd.Parameters.Add("@FailedAttempts", SqlDbType.Int).Value = failedAttempts;
                                    lockCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username;
                                    lockCmd.ExecuteNonQuery();
                                }

                                // BEGINNER NOTE: Account locking prevents brute-force attacks where hackers try many passwords rapidly
                                lblMessage.Text = "Account has been locked due to multiple failed login attempts. Please contact an administrator.";
                            }
                            else
                            {
                                // Update failed attempts counter (not yet at lockout threshold)
                                string updateSql = "UPDATE UsersComplexVersion SET FailedLoginAttempts = @FailedAttempts WHERE Username = @Username";

                                using (SqlCommand updateCmd = new SqlCommand(updateSql, conn))
                                {
                                    updateCmd.Parameters.Add("@FailedAttempts", SqlDbType.Int).Value = failedAttempts;
                                    updateCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username;
                                    updateCmd.ExecuteNonQuery();
                                }

                                // SECURITY NOTE: Generic error prevents attackers from knowing how close they are to lockout
                                lblMessage.Text = "Invalid username or password.";
                            }
                        }
                    }
                }

            }

        }

        /// <summary>
        /// HASH PASSWORD METHOD
        /// 
        /// This method takes a plain text password and converts it to a secure hash.
        /// 
        /// HOW IT WORKS:
        /// 1. Takes the password and salt (random data) as input
        /// 2. Uses PBKDF2 algorithm (Password-Based Key Derivation Function 2)
        /// 3. Runs the hashing process 100,000 times (iterations) - this makes it very slow
        ///    to crack passwords by brute force
        /// 4. Uses SHA256 as the underlying hash algorithm (very secure)
        /// 5. Returns a 64-byte (512-bit) hash
        /// 
        /// SECURITY FEATURES:
        /// - Salt makes each hash unique even if two users have the same password
        /// - Multiple iterations (100,000) make brute-force attacks very slow
        /// - SHA256 is a strong cryptographic hash algorithm
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <param name="salt">Random bytes that make the hash unique</param>
        /// <returns>A 64-byte hash of the password</returns>
        private byte[] HashPassword(string password, byte[] salt)
        {
            // Create PBKDF2 hasher with the password, salt, iterations, and hash algorithm
            // BEGINNER NOTE: Rfc2898DeriveBytes is the .NET implementation of PBKDF2
            // The "using" statement ensures the hasher is properly cleaned up after use
            using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, 100000,
                System.Security.Cryptography.HashAlgorithmName.SHA256))
            {
                // Generate and return 64 bytes (512 bits) of hashed data
                // BEGINNER NOTE: The comment says "256-bit hash" but we're actually getting 512 bits
                // This is extra secure - more bits means harder to crack
                return pbkdf2.GetBytes(64); // 512-bit hash (64 bytes × 8 bits/byte)
            }
        }

        /// <summary>
        /// COMPARE HASHES METHOD - CONSTANT TIME COMPARISON
        /// 
        /// This method compares two password hashes to see if they're identical.
        /// 
        /// BEGINNER NOTE: You might think "why not just use hash1 == hash2?"
        /// The reason is SECURITY - this method prevents "timing attacks".
        /// 
        /// WHAT IS A TIMING ATTACK?
        /// A timing attack is when a hacker measures how long it takes to compare values.
        /// If we stopped comparing as soon as we found a different byte, the comparison would
        /// be faster or slower depending on which byte was different. A hacker could use
        /// these tiny time differences to figure out parts of the password hash!
        /// 
        /// HOW THIS PREVENTS TIMING ATTACKS:
        /// This method ALWAYS checks EVERY byte in both arrays, no matter what.
        /// Whether the hashes match perfectly or are completely different, it takes
        /// the exact same amount of time. This means hackers can't learn anything
        /// from measuring how long the comparison takes.
        /// 
        /// HOW IT WORKS:
        /// 1. First check if arrays are different lengths (instant fail)
        /// 2. Initialize diff to 0
        /// 3. Loop through every byte in both arrays
        /// 4. XOR (exclusive OR) each byte pair - gives 0 if same, non-zero if different
        /// 5. OR the result with diff - if any bytes differ, diff becomes non-zero
        /// 6. After checking ALL bytes, return true only if diff is still 0
        /// </summary>
        /// <param name="hash1">First hash to compare</param>
        /// <param name="hash2">Second hash to compare</param>
        /// <returns>True if hashes are identical, false if different</returns>
        private bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            // ========================================
            // STEP 1: CHECK IF LENGTHS ARE DIFFERENT
            // ========================================
            // If the arrays are different sizes, they can't be equal
            // BEGINNER NOTE: We can check this quickly because length alone doesn't reveal
            // information about the actual password
            if (hash1.Length != hash2.Length)
                return false;

            // ========================================
            // STEP 2: COMPARE EVERY BYTE
            // ========================================
            // Initialize diff to 0. It will stay 0 only if ALL bytes match
            int diff = 0;

            // Loop through every single byte in both arrays
            // BEGINNER NOTE: This loop ALWAYS runs the same number of times
            // (the length of the arrays) regardless of where differences are found
            for (int i = 0; i < hash1.Length; i++)
            {
                // XOR (^) the bytes: returns 0 if they're the same, non-zero if different
                // OR (|=) with diff: if result is non-zero, diff becomes and stays non-zero
                // BEGINNER NOTE: This is the key line that makes constant-time comparison work
                // - If bytes match: hash1[i] ^ hash2[i] = 0, so diff stays the same
                // - If bytes differ: hash1[i] ^ hash2[i] = non-zero, so diff becomes non-zero
                // - Once diff is non-zero, it stays non-zero for the rest of the loop
                diff |= hash1[i] ^ hash2[i];
            }

            // ========================================
            // STEP 3: RETURN RESULT
            // ========================================
            // Return true if diff is 0 (all bytes matched), false if diff is non-zero (any byte differed)
            // BEGINNER NOTE: diff == 0 means every single byte in both hashes was identical
            return diff == 0;
        }

        ///// <summary>
        ///// Regenerates session ID to prevent session fixation attacks.
        ///// 
        ///// Uses SessionIDManager to create a new session ID while preserving session data.
        ///// This is more reliable than Session.Abandon() for authentication scenarios.
        ///// </summary>
        //private void RegenerateSessionId()
        //{
        //    // Get the session ID manager
        //    var manager = new System.Web.SessionState.SessionIDManager();

        //    // Create a new session ID
        //    string newSessionId = manager.CreateSessionID(Context);

        //    // Remove the old session ID cookie
        //    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", "")
        //    {
        //        Expires = DateTime.Now.AddDays(-1)
        //    });

        //    // Add new session ID cookie
        //    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", newSessionId));
        //}
    }
}