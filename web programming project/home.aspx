<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="web_programming_project.home" %>

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
            <asp:Label ID="monthTitle" runat="server" Text="monthTitle"></asp:Label>
        </div>
    <div class="content">
        <div class="left">
            <asp:DropDownList ID="DropDownList1" runat="server" CssClass="chooseMonth">
            </asp:DropDownList>
            <br />
        </div>
        <div class="main">
            <div class="top">
                <div class="top-box">
                    <asp:Label ID="e" runat="server" Text="支出"></asp:Label>
                    <div class="top-text">
                        <asp:Label ID="Label2" runat="server" Text="0" ForeColor="Red"></asp:Label>
                    </div>
                </div>
                <div class="top-box">
                    <asp:Label ID="i" runat="server" Text="收入"></asp:Label>
                    <div class="top-text">
                        <asp:Label ID="Label3" runat="server" Text="0" ForeColor="Green"></asp:Label>
                    </div>
                    <br />
                </div>
                <div class="top-box">
                    <asp:Label ID="t" runat="server" Text="結餘"></asp:Label>
                    <div class="top-text">
                        <asp:Label ID="Label4" runat="server" Text="0" ForeColor="Blue"></asp:Label>
                    </div>
                    <br />
                </div>
            </div>
            <div class="bottom">
            </div>
            <br />
        </div>
        <div class="right">
        </div>
    </div>
    </form>
    </body>
</html>
