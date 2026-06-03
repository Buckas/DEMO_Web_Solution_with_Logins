<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="manageUsers.aspx.cs" Inherits="DEMO_Web_Solution_with_Logins.manageUsers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%-- User management content - to add users to database --%>
            <h2>Manage Users</h2>
            <asp:Label ID="lblUsername" runat="server" Text="Username:"></asp:Label>
            <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="lblPassword" runat="server" Text="Password:"></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
            <br />
            <asp:Label ID="lblGivenName" runat="server" Text="GivenName:"></asp:Label>
            <asp:TextBox ID="txtGivenName" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="lblSurname" runat="server" Text="Surname:"></asp:Label>
            <asp:TextBox ID="txtSurname" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
            <br />
            <asp:Button ID="btnAddUser" runat="server" Text="Add User" OnClick="btnAddUser_Click" />

            <br />

            <%-- User management content for complex database operations - to add users to database --%>
            <h3>Complex User Management</h3>
            <asp:Label ID="lblComplexMessage" runat="server" Text=""></asp:Label>
            <br />
            <asp:Label ID="lblEmail" runat="server" Text="Email:"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
            <br />
            <asp:Button ID="btnComplexAddUser" runat="server" Text="Add User (Complex)" OnClick="btnComplexAddUser_Click" />
        </div>
    </form>
</body>
</html>
