<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="web_programming_project.home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="Style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
        <!-- 標題 -->
        <div class="title">
            <asp:Label ID="monthTitle" runat="server" Text="monthTitle"></asp:Label>
        </div>
        <!-- 主內容-->
        <div class="content">
            <!-- 左側 -->
            <div class="left">
                <asp:DropDownList ID="chooseMonth" runat="server" CssClass="chooseMonth">
                </asp:DropDownList>
                <br />
            </div>
            <!-- 中間 -->
            <div class="main">
                <!-- 收支結餘顯示 -->
                <div class="top">
                    <!-- 收 -->
                    <div class="top-box">
                        <asp:Label ID="e" runat="server" Text="支出"></asp:Label>
                        <div class="top-text">
                            <asp:Label ID="Label2" runat="server" Text="0" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                    <!-- 支 -->
                    <div class="top-box">
                        <asp:Label ID="i" runat="server" Text="收入"></asp:Label>
                        <div class="top-text">
                            <asp:Label ID="Label3" runat="server" Text="0" ForeColor="Green"></asp:Label>
                        </div>
                        <br />
                    </div>
                    <!-- 結餘 -->
                    <div class="top-box">
                        <asp:Label ID="t" runat="server" Text="結餘"></asp:Label>
                        <div class="top-text">
                            <asp:Label ID="Label4" runat="server" Text="0" ForeColor="Blue"></asp:Label>
                        </div>
                        <br />
                    </div>
                </div>
                <!-- 明細顯示 -->
                <div class="bottom" id="main">
                    <!-- 藉由Repeater顯示所有資料 -->
                    <asp:Repeater ID="Repeater1" runat="server">
                        <ItemTemplate>
                            <div class="record">
                                <div class="record-left">
                                    <asp:Label ID="Label8" runat="server" Text='<%# Eval("date") %>'></asp:Label>
                                </div>
                                <div class="record-center">
                                    <asp:Label ID="Label9" runat="server" Text='<%# Eval("category") %>'></asp:Label>
                                    &nbsp;&nbsp;
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("description") %>'></asp:Label>
                                </div>
                                <div class="record-right">
                                    <asp:Label ID="Label11" runat="server" Text='<%# Eval("amount") %>'></asp:Label>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <!-- 右側 -->
            <div class="right">
                <!-- 收支選擇 -->
                <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" CssClass="chooseType">
                    <asp:ListItem Value="expense" Selected="True" Text="支出"></asp:ListItem>
                    <asp:ListItem Value="income" Text="收入"></asp:ListItem>
                </asp:RadioButtonList>

                <br />
                <!-- 類別選擇 -->
                <div class="chooseCategory">
                    <asp:Label ID="Label5" runat="server" Text="Label"></asp:Label>
                    <br />
                    <asp:DropDownList ID="chooseCategory" runat="server"></asp:DropDownList>
                </div>
                <br />

                <!-- 輸入日期、金額、備註 -->
                <div class="inin">
                    <asp:Label ID="DATE_" runat="server" Text="日期"></asp:Label>
                    <br />
                    <asp:TextBox ID="date0" runat="server"></asp:TextBox>
                    <br />
                    <asp:Label ID="AMOUNT_" runat="server" Text="金額"></asp:Label>
                    <br />
                    <asp:TextBox ID="amount0" runat="server"></asp:TextBox>
                    <br />
                    <asp:Label ID="NOTE_" runat="server" Text="備註"></asp:Label>
                    <br />
                    <asp:TextBox ID="note0" runat="server"></asp:TextBox>
                </div>
                <br />
                <div class="actionButton">
                    <asp:Button ID="Button3" runat="server" Text="Button" />
                    <asp:Button ID="Button4" runat="server" Text="Button" />
                    <asp:Button ID="Button5" runat="server" Text="Button" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
