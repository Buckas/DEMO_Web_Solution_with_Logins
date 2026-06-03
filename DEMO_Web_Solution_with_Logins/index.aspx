<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="DEMO_Web_Solution_with_Logins.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form2" runat="server">
        <div>
            <%-- Welcome message or user-specific content --%>
            <h2>Welcome,
                <asp:Label ID="lblWelcome" runat="server" Text="User"></asp:Label>
                !</h2>
        </div>
        <asp:Button ID="btnLogin_Basic" runat="server" OnClick="btnLogin_Basic_Click" Text="Login Basic" />
        <asp:Button ID="btnLoginDataBase" runat="server" OnClick="btnLoginDataBase_Click" Text="Login Database" />
        <asp:Button ID="btnLoginDataBaseComplex" runat="server" OnClick="btnLoginDataBaseComplex_Click" Text="Login Database Complex" />
        <br />
        <asp:Button ID="btnAddUserToDB" runat="server" OnClick="btnAddUserToDB_Click" Text="Add User to Database" />
    </form>
    
</body>
</html>
