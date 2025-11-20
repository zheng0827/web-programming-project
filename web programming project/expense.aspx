<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="expense.aspx.cs" Inherits="web_programming_project.expense" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" type="text/css" href="style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="title">
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        </div>
    <div class="content">
        <div class="left">
            <asp:Button ID="Button1" runat="server" Text="Button" />
            <br />
        </div>
        <div class="main">
            <asp:Button ID="Button2" runat="server" Text="Button" />
            <br />
        </div>
    </div>
    </form>
    </body>
</html>
