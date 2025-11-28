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
            <div class="sidebar">
                <asp:DropDownList ID="chooseMonth" runat="server" CssClass="chooseMonth">
                </asp:DropDownList>

                <div class="changeYear">
                    <asp:Button ID="prevYear" runat="server" Text="&lt;" CssClass="yearButton" />
                    <asp:Label ID="yearLabel" runat="server" Text="2025" CssClass="yearLabel"></asp:Label>
                    <asp:Button ID="nextYear" runat="server" Text="&gt;" CssClass="yearButton" />
                </div>
                <div class="chooseMonth2">
                    <asp:RadioButtonList ID="RadioButtonList2" runat="server" RepeatDirection="Horizontal"
                        CssClass="RBLChooseMonth" RepeatLayout="Flow">
                        <asp:ListItem Value="1" Text="1"></asp:ListItem>
                        <asp:ListItem Value="2" Text="2"></asp:ListItem>
                        <asp:ListItem Value="3" Text="3"></asp:ListItem>
                        <asp:ListItem Value="4" Text="4"></asp:ListItem>
                        <asp:ListItem Value="5" Text="5"></asp:ListItem>
                        <asp:ListItem Value="6" Text="6"></asp:ListItem>
                        <asp:ListItem Value="7" Text="7"></asp:ListItem>
                        <asp:ListItem Value="8" Text="8"></asp:ListItem>
                        <asp:ListItem Value="9" Text="9"></asp:ListItem>
                        <asp:ListItem Value="10" Text="10"></asp:ListItem>
                        <asp:ListItem Value="11" Text="11"></asp:ListItem>
                        <asp:ListItem Value="12" Text="12"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </div>
            <!-- 中間 -->
            <div class="main">
                <!-- 收支結餘顯示 -->
                <div class="top">
                    <!-- 收 -->
                    <div class="top-box">
                        <asp:Label ID="i" runat="server" Text="收入"></asp:Label>
                        <div class="top-text">
                            <asp:Label ID="Label3" runat="server" Text="0" ForeColor="#198754"></asp:Label>
                        </div>
                        <br />
                    </div>
                    <!-- 支 -->
                    <div class="top-box">
                        <asp:Label ID="e" runat="server" Text="支出"></asp:Label>
                        <div class="top-text">
                            <asp:Label ID="Label2" runat="server" Text="0" ForeColor="#ff0000"></asp:Label>
                        </div>
                    </div>
                    <!-- 結餘 -->
                    <div class="top-box">
                        <asp:Label ID="t" runat="server" Text="結餘"></asp:Label>
                        <div class="top-text">
                            <asp:Label ID="Label4" runat="server" Text="0" ForeColor="#0d6efd"></asp:Label>
                        </div>
                        <br />
                    </div>
                </div>
                <!-- 明細顯示 -->
                <hr class="line" />
                <div class="bottom" id="main">
                    <!-- 藉由Repeater顯示所有資料 -->
                    <asp:Repeater ID="DateRepeater" runat="server" OnItemDataBound="DateRepeater_ItemDataBound">
                        <ItemTemplate>
                            <div class="date">
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("date") %>' CssClass="date-label"></asp:Label>
                                <br />
                                <asp:Repeater ID="DetailRepeater" runat="server">
                                    <ItemTemplate>
                                        <div class="record">
                                            <div class="category">
                                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("category") %>'></asp:Label>
                                            </div>
                                            <div class="description">
                                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("description") %>'></asp:Label>
                                            </div>
                                            <div class="amount">
                                                <asp:Label ID="Label11" runat="server" Text='<%# Eval("amount") %>'></asp:Label>
                                            </div>
                                            <div class="button">
                                                <asp:Button ID="edit" runat="server" Text="編輯" />
                                                <asp:Button ID="delete" runat="server" Text="刪除" />
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <!-- 右側 -->
            <div class="right">
                <!-- 收支選擇 -->
                <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" CssClass="chooseType" RepeatLayout="Flow">
                    <asp:ListItem Value="expense" Selected="True" Text="支出"></asp:ListItem>
                    <asp:ListItem Value="income" Text="收入"></asp:ListItem>
                </asp:RadioButtonList>

                <br />
                <!-- 類別選擇 -->
                <div class="chooseCategory">
                    <asp:Label ID="Label5" runat="server" Text="類別"></asp:Label>
                    <br />
                    <asp:DropDownList ID="chooseCategory" runat="server" CssClass="DDLChooseCategory"></asp:DropDownList>
                </div>
                <br />

                <!-- 輸入日期、金額、備註 -->
                <div class="form">
                    <asp:Label ID="DATE_" runat="server" Text="日期"></asp:Label>
                    <br />
                    <asp:TextBox ID="date0" runat="server" TextMode="Date"></asp:TextBox>
                    <br />
                    <asp:Label ID="AMOUNT_" runat="server" Text="金額"></asp:Label>
                    <br />
                    <asp:TextBox ID="amount0" runat="server" TextMode="Number"></asp:TextBox>
                    <br />
                    <asp:Label ID="Label1" runat="server" Text="備註"></asp:Label>
                    <br />
                    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                </div>
                <br />
                <div class="actionButton">
                    <asp:Button ID="cancel" runat="server" Text="取消" />
                    <asp:Button ID="save" runat="server" Text="儲存" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>
