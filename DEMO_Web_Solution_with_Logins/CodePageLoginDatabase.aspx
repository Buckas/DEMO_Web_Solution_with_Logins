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
    <p>
        This is the SQL code to create the &quot;&quot; table to hold the more complex users info for the &quot;LoginDatabaseComplex&quot;</p>
    <!--EndFragment-->
<pre style="font-family:'Cascadia Mono';font-size:9pt;color:#000000"><span style="color:#0000ff">CREATE</span> <span style="color:#0000ff">TABLE</span> [dbo]<span style="color:#808080">.</span>[UsersComplexVersion]<span style="color:#0000ff"> </span><span style="color:#808080">(</span>
    [Id]                  <span style="color:#0000ff">INT</span>            <span style="color:#0000ff">IDENTITY </span><span style="color:#808080">(</span>1<span style="color:#808080">,</span> 1<span style="color:#808080">)</span> <span style="color:#808080">NOT</span> <span style="color:#808080">NULL,</span>
    [GivenName]           <span style="color:#0000ff">NVARCHAR </span><span style="color:#808080">(</span>100<span style="color:#808080">)</span> <span style="color:#808080">NOT</span> <span style="color:#808080">NULL,</span>
    [Surname]             <span style="color:#0000ff">NVARCHAR </span><span style="color:#808080">(</span>100<span style="color:#808080">)</span> <span style="color:#808080">NOT</span> <span style="color:#808080">NULL,</span>
    [Username]            <span style="color:#0000ff">NVARCHAR </span><span style="color:#808080">(</span>100<span style="color:#808080">)</span> <span style="color:#808080">NOT</span> <span style="color:#808080">NULL,</span>
    [PasswordHash]        <span style="color:#0000ff">VARBINARY </span><span style="color:#808080">(</span>64<span style="color:#808080">)</span> <span style="color:#808080">NOT</span> <span style="color:#808080">NULL,</span>
    [Salt]                <span style="color:#0000ff">VARBINARY </span><span style="color:#808080">(</span>32<span style="color:#808080">)</span> <span style="color:#808080">NOT</span> <span style="color:#808080">NULL,</span>
    [CreatedAt]           <span style="color:#0000ff">DATETIME2 </span><span style="color:#808080">(</span>7<span style="color:#808080">)</span>  <span style="color:#0000ff">DEFAULT </span><span style="color:#808080">(</span><span style="color:#ff00ff">getdate</span><span style="color:#808080">())</span> <span style="color:#808080">NOT</span> <span style="color:#808080">NULL,</span>
    [UpdatedAt]           <span style="color:#0000ff">DATETIME2 </span><span style="color:#808080">(</span>7<span style="color:#808080">)</span>  <span style="color:#808080">NULL,</span>
    [Email]               <span style="color:#0000ff">NVARCHAR </span><span style="color:#808080">(</span>255<span style="color:#808080">)</span> <span style="color:#808080">NULL,</span>
    [IsActive]            <span style="color:#0000ff">BIT</span>            <span style="color:#0000ff">DEFAULT </span><span style="color:#808080">((</span>1<span style="color:#808080">))</span> <span style="color:#808080">NOT</span> <span style="color:#808080">NULL,</span>
    [FailedLoginAttempts] <span style="color:#0000ff">INT</span>            <span style="color:#0000ff">DEFAULT </span><span style="color:#808080">((</span>0<span style="color:#808080">))</span> <span style="color:#808080">NOT</span> <span style="color:#808080">NULL,</span>
    [LastLogin]           <span style="color:#0000ff">DATETIME2 </span><span style="color:#808080">(</span>7<span style="color:#808080">)</span>  <span style="color:#808080">NULL,</span>
    <span style="color:#0000ff">PRIMARY</span> <span style="color:#0000ff">KEY</span> <span style="color:#0000ff">CLUSTERED </span><span style="color:#808080">(</span>[Id] <span style="color:#0000ff">ASC</span><span style="color:#808080">),</span>
    <span style="color:#0000ff">UNIQUE</span> <span style="color:#0000ff">NONCLUSTERED </span><span style="color:#808080">(</span>[Username] <span style="color:#0000ff">ASC</span><span style="color:#808080">)</span>
<span style="color:#808080">);</span>


<span style="color:#0000ff">GO</span>
<span style="color:#0000ff">CREATE</span> <span style="color:#0000ff">NONCLUSTERED</span> <span style="color:#0000ff">INDEX</span> [IX_Users_Username]
    <span style="color:#0000ff">ON</span> [dbo]<span style="color:#808080">.</span>[UsersComplexVersion]<span style="color:#808080">(</span>[Username] <span style="color:#0000ff">ASC</span><span style="color:#808080">);</span>

</pre>
    <!--EndFragment-->
<p>
    &nbsp;</p>
</body>
</html>
