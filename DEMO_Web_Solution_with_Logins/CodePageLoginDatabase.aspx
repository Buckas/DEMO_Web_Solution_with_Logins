<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CodePageLoginDatabase.aspx.cs" Inherits="DEMO_Web_Solution_with_Logins.CodePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    This is the SQL code to create the &quot;Users&quot; table to hold the users info for the &quot;LoginDatabase&quot;:<br />
    <pre style="font-family: 'Cascadia Mono'; font-size: 9pt; color: #000000"><span style="color: #0000ff">CREATE</span> <span style="color: #0000ff">TABLE</span> [dbo]<span style="color: #808080">.</span>[Users]<span style="color: #0000ff"> </span><span style="color: #808080">(</span>
    [Id]        <span style="color: #0000ff">INT</span>            <span style="color: #0000ff">IDENTITY </span><span style="color: #808080">(</span>1<span style="color: #808080">,</span> 1<span style="color: #808080">)</span> <span style="color: #808080">NOT</span> <span style="color: #808080">NULL,</span>
    [Username]  <span style="color: #0000ff">NVARCHAR </span><span style="color: #808080">(</span>100<span style="color: #808080">)</span> <span style="color: #808080">NOT</span> <span style="color: #808080">NULL,</span>
    [Password]  <span style="color: #0000ff">NVARCHAR </span><span style="color: #808080">(</span>100<span style="color: #808080">)</span> <span style="color: #808080">NOT</span> <span style="color: #808080">NULL,</span>
    [Surname]   <span style="color: #0000ff">NVARCHAR </span><span style="color: #808080">(</span>100<span style="color: #808080">)</span> <span style="color: #808080">NULL,</span>
    [GivenName] <span style="color: #0000ff">NVARCHAR </span><span style="color: #808080">(</span>100<span style="color: #808080">)</span> <span style="color: #808080">NULL,</span>
    <span style="color: #0000ff">PRIMARY</span> <span style="color: #0000ff">KEY</span> <span style="color: #0000ff">CLUSTERED </span><span style="color: #808080">(</span>[Id] <span style="color: #0000ff">ASC</span><span style="color: #808080">)</span>
<span style="color: #808080">);</span></pre>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
