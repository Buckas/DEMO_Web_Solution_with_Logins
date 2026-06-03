using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DEMO_Web_Solution_with_Logins
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in by checking session variable
            // and if logged in set lable to usernamne
            if (Session["LoggedInUser"] != null)
            {
                lblWelcome.Text = Session["LoggedInUser"].ToString() + "!";
            }
        }

        protected void btnLogin_Basic_Click(object sender, EventArgs e)
        {
            Response.Redirect("login_basic.aspx");
        }

        protected void btnLoginDataBase_Click(object sender, EventArgs e)
        {
            Response.Redirect("login_database.aspx");
        }

        protected void btnLoginDataBaseComplex_Click(object sender, EventArgs e)
        {
            Response.Redirect("login_database_complex.aspx");
        }

        protected void btnAddUserToDB_Click(object sender, EventArgs e)
        {
            Response.Redirect("manageUsers.aspx");
        }
    }
}