using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DEMO_Web_Solution_with_Logins
{
    public partial class login_basic : System.Web.UI.Page
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

            //open loginDetails.CSV text file and read username and password data
            //Read all lines from the file and split each line by comma to get username and password
            string[] lines = System.IO.File.ReadAllLines(Server.MapPath("~/loginDetails.csv"));

            // Check if the username and password match any entry in the file
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2)
                {
                    if (parts[0] == username && parts[1] == password)
                    {
                        // Login successful
                        // set session variable to indicate user is logged in
                        Session["LoggedInUser"] = username;
                        // Redirect to index page
                        Response.Redirect("index.aspx");
                        return;
                    }
                }
            }
            // Login failed
            lblMessage.Text = "Invalid username or password.";
        }
    }
}